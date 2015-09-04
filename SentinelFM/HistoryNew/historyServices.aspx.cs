using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Text;
using System.IO;
using System.Data;
using System.Globalization;
using System.Configuration;
using VLF.Reports;
using VLF.DAS.Logic;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using ClosedXML.Excel;

namespace SentinelFM
{
    public partial class HistoryNew_historyServices : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;

        protected clsUtility objUtil;

        public string _xml = "";

        public string emptyCommMode;

        private bool ShowHistoryDetails = false;

        private int vlStart = 0;
        private int vlLimit = 10000;//Edited by Rohit Mittal

        public bool MutipleUserHierarchyAssignment;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

                sn = (SentinelFMSession)Session["SentinelFMSession"];
                emptyCommMode = "<Box><BoxConfigInfo></BoxConfigInfo></Box>";
                if (!Page.IsPostBack)
                {
                    string request = Request.QueryString["st"];
                    if (string.IsNullOrEmpty(request))
                    {
                        request = "GetVehicleListByFleetId";
                    }
                    if (request == "GetVehicleListByFleetId")
                    {
                        request = Request.QueryString["fleetID"];

                        if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
                        {
                            string fleetIds = request;
                            if (fleetIds != null && fleetIds.Trim() != string.Empty)
                            {
                                GetVehicleListByMultipleFleetIds_NewTZ(fleetIds);
                            }
                        }
                        else
                        {
                            int fleetID = 0;
                            if (!string.IsNullOrEmpty(request))
                            {
                                Int32.TryParse(request, out fleetID);
                            }
                            if (fleetID == 0)
                                fleetID = sn.User.DefaultFleet;
                            if (fleetID > 0)
                            {
                                GetVehicleListByFleetId_NewTZ(fleetID);
                            }
                        }

                    }
                    else if (request.ToLower().Trim() == "getcommmode")
                    {
                        Response.ContentType = "text/xml";
                        Response.ContentEncoding = Encoding.Default;

                        request = Request.QueryString["vehicleID"];
                        int vehicleID = 0;
                        if (!string.IsNullOrEmpty(request))
                        {
                            Int32.TryParse(request, out vehicleID);
                        }

                        if (vehicleID > 0)
                        {

                            DataRow[] DataRows = null; ;
                            //DataRows = sn.History.DsHistoryVehicles.Tables[0].Select("VehicleId=" + Convert.ToInt32(this.cboVehicle.SelectedItem.Value.ToString().TrimEnd()));
                            DataRows = sn.History.DsHistoryVehicles.Tables[0].Select("VehicleId='" + vehicleID.ToString() + "'");
                            if ((DataRows != null) && (DataRows.Length > 0))
                            {
                                DataRow drVehicle = DataRows[0];
                                sn.History.LicensePlate = drVehicle["LicensePlate"].ToString().TrimEnd();
                                GetCommModeByBoxId(Convert.ToInt32(drVehicle["BoxId"]));

                            }
                            else
                            {
                                Response.Write(emptyCommMode.Trim());
                            }
                        }
                        else
                        {
                            Response.Write(emptyCommMode.Trim());
                        }
                    }
                    else if (request.ToLower().Trim() == "getmessagelist")
                    {
                        GetMessageList();
                    }
                    else if (request.ToLower().Trim() == "gethistoryrecords")
                    {
                        GetHistoryRecords_NewTZ();
                    }
                    else if (request.ToLower().Trim() == "gettripdetails")
                    {
                        GetHistoryTripDetails_NewTZ();
                    }
                    else if (request == "GetFilteredRecord") //Edited by Rohit Mittal
                    {
                        getFilteredRecord_NewTZ();
                    }

                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            objUtil = new clsUtility(sn);
        }

        protected override void InitializeCulture()
        {
            if (Session["PreferredCulture"] != null)
            {
                string UserCulture = Session["PreferredCulture"].ToString();
                if (UserCulture != "")
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(UserCulture);
                }
            }
        }
        // Changes for TimeZone Feature start
        private void GetVehicleListByFleetId_NewTZ(int fleetId)
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.Default;

            //string xml = "<root><goods>Car</goods><fleetId>" + fleetId.ToString() + "</fleetId></root>";
            string xml = "";
            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

            if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                {
                    return;
                }
            if (xml == "")
            {
                return;
            }

            StringReader strrXML = new StringReader(xml);
            DataSet dsVehicle;
            dsVehicle = new DataSet();
            dsVehicle.ReadXml(strrXML);

            sn.History.DsHistoryVehicles = dsVehicle;
            xml = xml.Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
            Response.Write(xml.Trim());

        }
        // Changes for TimeZone Feature end

        private void GetVehicleListByFleetId(int fleetId)
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.Default;

            //string xml = "<root><goods>Car</goods><fleetId>" + fleetId.ToString() + "</fleetId></root>";
            string xml = "";
            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

            if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                {
                    return;
                }
            if (xml == "")
            {
                return;
            }

            StringReader strrXML = new StringReader(xml);
            DataSet dsVehicle;
            dsVehicle = new DataSet();
            dsVehicle.ReadXml(strrXML);

            sn.History.DsHistoryVehicles = dsVehicle;
            xml = xml.Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
            Response.Write(xml.Trim());

        }

        // Changes For TimeZone Feature start
        private void GetVehicleListByMultipleFleetIds_NewTZ(string fleetIds)
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.Default;

            //string xml = "<root><goods>Car</goods><fleetId>" + fleetId.ToString() + "</fleetId></root>";
            string xml = "";
            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

            if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByMultipleFleetIds(sn.UserID, sn.SecId, fleetIds, ref xml), false))
                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByMultipleFleetIds(sn.UserID, sn.SecId, fleetIds, ref xml), true))
                {
                    return;
                }
            if (xml == "")
            {
                return;
            }

            StringReader strrXML = new StringReader(xml);
            DataSet dsVehicle;
            dsVehicle = new DataSet();
            dsVehicle.ReadXml(strrXML);

            sn.History.DsHistoryVehicles = dsVehicle;
            xml = xml.Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
            Response.Write(xml.Trim());

        }
        // Changes for TimeZone Feature end

        private void GetCommModeByBoxId(int BoxId)
        {
            try
            {
                DataSet ds = new DataSet();
                //StringReader strrXML = null;
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
                //strrXML = new StringReader(xml);
                Response.Write(xml.Trim());


            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            }

        }

        private void GetMessageList()
        {
            try
            {
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.Default;

                //DataSet dsMsgTypes = new DataSet();
                //StringReader strrXML = null;
                string xml = "";
                ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();

                //CultureInfo.CurrentUICulture.TwoLetterISOLanguageName
                string lng = ((sn.SelectedLanguage != null && sn.SelectedLanguage.Length > 0) ? sn.SelectedLanguage.Substring(0, 2) : "en");

                if (objUtil.ErrCheck(dbs.GetBoxMsgInTypesByLang(sn.UserID, sn.SecId, lng, ref xml), false))
                    if (objUtil.ErrCheck(dbs.GetBoxMsgInTypesByLang(sn.UserID, sn.SecId, lng, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Failed to GetBoxMsgInTypes for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        return;
                    }

                if (xml == "")
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Failed to GetBoxMsgInTypes for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    return;
                }

                Response.Write(xml.Trim());
                //strrXML = new StringReader(xml);
                //dsMsgTypes.ReadXml(strrXML);

                //lstMsgTypes.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("lstMsgTypes_Item_0"), "-1"));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                //ExceptionLogger(trace);
            }
        }

        // Changes for TimeZone Feature start
        private void GetHistoryRecords_NewTZ()
        {
            result _r = new result();
            try
            {
                Response.ContentType = "text/html";
                Response.ContentEncoding = Encoding.Default;

                string xml = "";

                vlStart = 0;
                vlLimit = 10000;
                string request = Request.QueryString["start"];
                if (!string.IsNullOrEmpty(request))
                {
                    Int32.TryParse(request, out vlStart);
                    if (vlStart < 0) vlStart = 0;
                }

                request = Request.QueryString["limit"];
                if (!string.IsNullOrEmpty(request))
                {
                    Int32.TryParse(request, out vlLimit);
                    if (vlLimit <= 0) vlStart = vlLimit;
                }

                request = Request.QueryString["fromsession"];
                if (!string.IsNullOrEmpty(request) && request.Trim() == "1" && sn.History.DsHistoryInfo != null)
                {
                    //sn.History.DsHistoryInfo = dsHistory;


                    _r.success = true;
                    _r.msg = sn.History.DsHistoryInfo.Tables[0].Rows.Count.ToString() + " records";
                    if (vlStart == 0 && vlLimit == 10000)
                    {
                        _r.data = sn.History.DsHistoryInfo.GetXml().Replace("\r\n", "").Replace("&#x0;", "");
                        _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    }
                    else
                    {
                        DataSet dstemp = new DataSet();
                        DataView dv = sn.History.DsHistoryInfo.Tables["VehicleStatusHistory"].DefaultView;
                        //dv.Sort = "OriginDateTime DESC";
                        DataTable sortedTable = dv.ToTable();
                        DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                        dt.TableName = "VehicleStatusHistory";
                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "MsgInHistory";
                        _r.data = dstemp.GetXml().Replace("<MsgInHistory>", "<MsgInHistory><totalCount>" + sn.History.DsHistoryInfo.Tables["VehicleStatusHistory"].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "");
                        _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    }
                    _r.iconTypeName = sn.History.IconTypeName;
                    Response.Write(new JavaScriptSerializer().Serialize(_r));

                    return;

                }

                string historytype = Request["historyType"].ToString();
                switch (historytype)
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
                        //sn.History.ReportstopDuration = Convert.ToInt32(this.cboStopSequence.SelectedItem.Value);
                        sn.History.ReportstopDuration = 0;
                        break;
                    case "2":
                        sn.History.ShowTrips = false;
                        sn.History.ShowBreadCrumb = false;
                        sn.History.ShowStopsAndIdle = false;
                        sn.History.ShowStops = true;
                        sn.History.ShowIdle = false;
                        //sn.History.ReportstopDuration = Convert.ToInt32(this.cboStopSequence.SelectedItem.Value);
                        sn.History.ReportstopDuration = 0;
                        break;
                    case "3":
                        sn.History.ShowTrips = false;
                        sn.History.ShowBreadCrumb = false;
                        sn.History.ShowStopsAndIdle = false;
                        sn.History.ShowStops = false;
                        sn.History.ShowIdle = true;
                        //sn.History.ReportstopDuration = Convert.ToInt32(this.cboStopSequence.SelectedItem.Value);
                        sn.History.ReportstopDuration = 0;
                        break;
                    case "4":
                        sn.History.ShowTrips = true;
                        sn.History.ShowStops = false;
                        sn.History.ShowIdle = false;
                        sn.History.ShowStopsAndIdle = false;
                        break;
                }

                string strFromDate = "";
                string strToDate = "";

                strFromDate = Request["historyDateFrom"].ToString() + " " + Request["historyTimeFrom"].ToString();
                strToDate = Request["historyDateTo"].ToString() + " " + Request["historyTimeTo"].ToString();

                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    _r.success = false;
                    _r.msg = "Session Expired";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
                    return;
                }

                DateTime dtsd = DateTime.ParseExact(strFromDate, sn.User.DateFormat + " h:mm tt", CultureInfo.InvariantCulture);
                DateTime dttd = DateTime.ParseExact(strToDate, sn.User.DateFormat + " h:mm tt", CultureInfo.InvariantCulture);
                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");


                strFromDate = Convert.ToDateTime(dtsd, ci).ToString();
                strToDate = Convert.ToDateTime(dttd, ci).ToString();

                if (Convert.ToDateTime(strFromDate) > Convert.ToDateTime(strToDate))
                {
                    _r.success = false;
                    _r.msg = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidDate"); //"The From Date should be earlier than the To Date!";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
                    return;
                }

                string DisableFullHistoryData = ConfigurationManager.AppSettings["DisableFullHistoryData"].ToString();
                TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(strToDate).Ticks - Convert.ToDateTime(strFromDate).Ticks);
                if (Convert.ToBoolean(DisableFullHistoryData) && (currDuration.TotalHours > 24))
                {
                    _r.success = false;
                    _r.msg = "Due to service loads, HISTORY requests have been temporarily limited to a period of 24 hours.";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
                    return;
                }

                if (Request["lastmessageonly"] != null && Request["lastmessageonly"].ToString().Trim() == "1")
                    sn.History.SqlTopMsg = "TOP 1";
                else
                    sn.History.SqlTopMsg = "";


                sn.History.MsgList = "";
                if (Request["historyMessageList"] != null && !(Request["historyMessageList"].ToString().Contains("-1")))
                {
                    sn.History.MsgList = Request["historyMessageList"].ToString();
                }

                //_r.success = false;
                //_r.msg = sn.History.MsgList;
                //_r.data = "";
                //Response.Write(new JavaScriptSerializer().Serialize(_r));
                //return;

                sn.History.FromDate = Request["historyDateFrom"].ToString();
                sn.History.ToDate = Request["historyDateTo"].ToString();
                sn.History.FromHours = Request["historyTimeFrom"].ToString();
                sn.History.ToHours = Request["historyTimeTo"].ToString();
                sn.History.VehicleId = Convert.ToInt64(Request["historyVehicle"].ToString());

                if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
                    sn.History.MultiFleetIDs = Request["historyFleet"].ToString();
                else
                    sn.History.FleetId = Convert.ToInt64(Request["historyFleet"].ToString());
                sn.History.ShowToolTip = true;
                sn.History.CarLatitude = "";
                sn.History.CarLongitude = "";
                sn.History.DsHistoryInfo = null;
                //sn.History.VehicleName = this.cboVehicle.SelectedItem.Text.Replace("'", "''");
                //sn.History.FleetName = this.cboFleet.SelectedItem.Text.Replace("'", "''");
                sn.History.DclId = Convert.ToInt16(Request["historyCommMode"].ToString());
                //if (cboCommMode.Visible)
                //    sn.History.DclId = Convert.ToInt16(this.cboCommMode.SelectedItem.Value);
                //else
                //    sn.History.DclId = -1;

                sn.History.FromDateTime = strFromDate;
                sn.History.ToDateTime = strToDate;
                if (Request["historyByLocation"].ToString().Trim() == "1" && Request["historyLocation"].ToString().Trim() != "")
                    sn.History.Address = "%" + Request["historyLocation"].ToString().Trim() + "%";
                else
                    sn.History.Address = "";

                sn.History.TripSensor = Convert.ToInt16(Request["historytrip"].ToString());


                string vehiclePlate = string.Empty;
                if (sn.History.LicensePlate != null)
                    vehiclePlate = sn.History.LicensePlate;

                if ((sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
                {
                    dgStops_Fill_NewTZ(sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);
                }
                else if (sn.History.ShowTrips)
                {
                    dgTrips_Fill_NewTZ(sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);
                }
                else
                {
                    dgHistory_Fill_NewTZ(sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);
                }
                /*if (!sn.History.RedirectFromMapScreen)
                {
                    RadAjaxManager1.ResponseScripts.Add("parent.frmHis.GenerateGridsDate('" + vehiclePlate + "');");
                }
                else
                {
                    //Disable loading data when redirecting from map
                    //RadAjaxManager1.ResponseScripts.Add("LoadHistoryGridData('" + vehiclePlate + "');");
                    RadAjaxManager1.ResponseScripts.Add("SetSendCommandEvent('" + vehiclePlate + "');");
                }
                

                Response.Write(xml.Trim());*/

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                _r.success = false;
                _r.msg = Ex.Message;
                _r.data = "";
                Response.Write(new JavaScriptSerializer().Serialize(_r));
                return;
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                _r.success = false;
                _r.msg = Ex.Message;
                _r.msgDetails = Ex.StackTrace.ToString();
                _r.data = "";
                Response.Write(new JavaScriptSerializer().Serialize(_r));
                return;
                //ExceptionLogger(trace);
            }
        }

        // Changes for TimeZone Feature end

        private void GetHistoryRecords()
        {
            result _r = new result();
            try
            {
                Response.ContentType = "text/html";
                Response.ContentEncoding = Encoding.Default;

                string xml = "";

                vlStart = 0;
                vlLimit = 10000;
                string request = Request.QueryString["start"];
                if (!string.IsNullOrEmpty(request))
                {
                    Int32.TryParse(request, out vlStart);
                    if (vlStart < 0) vlStart = 0;
                }

                request = Request.QueryString["limit"];
                if (!string.IsNullOrEmpty(request))
                {
                    Int32.TryParse(request, out vlLimit);
                    if (vlLimit <= 0) vlStart = vlLimit;
                }

                request = Request.QueryString["fromsession"];
                if (!string.IsNullOrEmpty(request) && request.Trim() == "1" && sn.History.DsHistoryInfo != null)
                {
                    //sn.History.DsHistoryInfo = dsHistory;


                    _r.success = true;
                    _r.msg = sn.History.DsHistoryInfo.Tables[0].Rows.Count.ToString() + " records";
                    if (vlStart == 0 && vlLimit == 10000)
                    {
                        _r.data = sn.History.DsHistoryInfo.GetXml().Replace("\r\n", "").Replace("&#x0;", "");
                        _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    }
                    else
                    {
                        DataSet dstemp = new DataSet();
                        DataView dv = sn.History.DsHistoryInfo.Tables["VehicleStatusHistory"].DefaultView;
                        //dv.Sort = "OriginDateTime DESC";
                        DataTable sortedTable = dv.ToTable();
                        DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                        dt.TableName = "VehicleStatusHistory";
                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "MsgInHistory";
                        _r.data = dstemp.GetXml().Replace("<MsgInHistory>", "<MsgInHistory><totalCount>" + sn.History.DsHistoryInfo.Tables["VehicleStatusHistory"].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "");
                        _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    }
                    _r.iconTypeName = sn.History.IconTypeName;
                    Response.Write(new JavaScriptSerializer().Serialize(_r));

                    return;

                }

                string historytype = Request["historyType"].ToString();
                switch (historytype)
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
                        //sn.History.ReportstopDuration = Convert.ToInt32(this.cboStopSequence.SelectedItem.Value);
                        sn.History.ReportstopDuration = 0;
                        break;
                    case "2":
                        sn.History.ShowTrips = false;
                        sn.History.ShowBreadCrumb = false;
                        sn.History.ShowStopsAndIdle = false;
                        sn.History.ShowStops = true;
                        sn.History.ShowIdle = false;
                        //sn.History.ReportstopDuration = Convert.ToInt32(this.cboStopSequence.SelectedItem.Value);
                        sn.History.ReportstopDuration = 0;
                        break;
                    case "3":
                        sn.History.ShowTrips = false;
                        sn.History.ShowBreadCrumb = false;
                        sn.History.ShowStopsAndIdle = false;
                        sn.History.ShowStops = false;
                        sn.History.ShowIdle = true;
                        //sn.History.ReportstopDuration = Convert.ToInt32(this.cboStopSequence.SelectedItem.Value);
                        sn.History.ReportstopDuration = 0;
                        break;
                    case "4":
                        sn.History.ShowTrips = true;
                        sn.History.ShowStops = false;
                        sn.History.ShowIdle = false;
                        sn.History.ShowStopsAndIdle = false;
                        break;
                }

                string strFromDate = "";
                string strToDate = "";

                strFromDate = Request["historyDateFrom"].ToString() + " " + Request["historyTimeFrom"].ToString();
                strToDate = Request["historyDateTo"].ToString() + " " + Request["historyTimeTo"].ToString();

                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    _r.success = false;
                    _r.msg = "Session Expired";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
                    return;
                }

                DateTime dtsd = DateTime.ParseExact(strFromDate, sn.User.DateFormat + " h:mm tt", CultureInfo.InvariantCulture);
                DateTime dttd = DateTime.ParseExact(strToDate, sn.User.DateFormat + " h:mm tt", CultureInfo.InvariantCulture);
                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");


                strFromDate = Convert.ToDateTime(dtsd, ci).ToString();
                strToDate = Convert.ToDateTime(dttd, ci).ToString();

                if (Convert.ToDateTime(strFromDate) > Convert.ToDateTime(strToDate))
                {
                    _r.success = false;
                    _r.msg = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidDate"); //"The From Date should be earlier than the To Date!";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
                    return;
                }

                string DisableFullHistoryData = ConfigurationManager.AppSettings["DisableFullHistoryData"].ToString();
                TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(strToDate).Ticks - Convert.ToDateTime(strFromDate).Ticks);
                if (Convert.ToBoolean(DisableFullHistoryData) && (currDuration.TotalHours > 24))
                {
                    _r.success = false;
                    _r.msg = "Due to service loads, HISTORY requests have been temporarily limited to a period of 24 hours.";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
                    return;
                }

                if (Request["lastmessageonly"] != null && Request["lastmessageonly"].ToString().Trim() == "1")
                    sn.History.SqlTopMsg = "TOP 1";
                else
                    sn.History.SqlTopMsg = "";


                sn.History.MsgList = "";
                if (Request["historyMessageList"] != null && !(Request["historyMessageList"].ToString().Contains("-1")))
                {
                    sn.History.MsgList = Request["historyMessageList"].ToString();
                }

                //_r.success = false;
                //_r.msg = sn.History.MsgList;
                //_r.data = "";
                //Response.Write(new JavaScriptSerializer().Serialize(_r));
                //return;

                sn.History.FromDate = Request["historyDateFrom"].ToString();
                sn.History.ToDate = Request["historyDateTo"].ToString();
                sn.History.FromHours = Request["historyTimeFrom"].ToString();
                sn.History.ToHours = Request["historyTimeTo"].ToString();
                sn.History.VehicleId = Convert.ToInt64(Request["historyVehicle"].ToString());

                if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
                    sn.History.MultiFleetIDs = Request["historyFleet"].ToString();
                else
                    sn.History.FleetId = Convert.ToInt64(Request["historyFleet"].ToString());
                sn.History.ShowToolTip = true;
                sn.History.CarLatitude = "";
                sn.History.CarLongitude = "";
                sn.History.DsHistoryInfo = null;
                //sn.History.VehicleName = this.cboVehicle.SelectedItem.Text.Replace("'", "''");
                //sn.History.FleetName = this.cboFleet.SelectedItem.Text.Replace("'", "''");
                sn.History.DclId = Convert.ToInt16(Request["historyCommMode"].ToString());
                //if (cboCommMode.Visible)
                //    sn.History.DclId = Convert.ToInt16(this.cboCommMode.SelectedItem.Value);
                //else
                //    sn.History.DclId = -1;

                sn.History.FromDateTime = strFromDate;
                sn.History.ToDateTime = strToDate;
                if (Request["historyByLocation"].ToString().Trim() == "1" && Request["historyLocation"].ToString().Trim() != "")
                    sn.History.Address = "%" + Request["historyLocation"].ToString().Trim() + "%";
                else
                    sn.History.Address = "";

                sn.History.TripSensor = Convert.ToInt16(Request["historytrip"].ToString());


                string vehiclePlate = string.Empty;
                if (sn.History.LicensePlate != null)
                    vehiclePlate = sn.History.LicensePlate;

                if ((sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
                {
                    dgStops_Fill(sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);
                }
                else if (sn.History.ShowTrips)
                {
                    dgTrips_Fill(sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);
                }
                else
                {
                    dgHistory_Fill(sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);
                }
                /*if (!sn.History.RedirectFromMapScreen)
                {
                    RadAjaxManager1.ResponseScripts.Add("parent.frmHis.GenerateGridsDate('" + vehiclePlate + "');");
                }
                else
                {
                    //Disable loading data when redirecting from map
                    //RadAjaxManager1.ResponseScripts.Add("LoadHistoryGridData('" + vehiclePlate + "');");
                    RadAjaxManager1.ResponseScripts.Add("SetSendCommandEvent('" + vehiclePlate + "');");
                }
                

                Response.Write(xml.Trim());*/

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                _r.success = false;
                _r.msg = Ex.Message;
                _r.data = "";
                Response.Write(new JavaScriptSerializer().Serialize(_r));
                return;
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                _r.success = false;
                _r.msg = Ex.Message;
                _r.msgDetails = Ex.StackTrace.ToString();
                _r.data = "";
                Response.Write(new JavaScriptSerializer().Serialize(_r));
                return;
                //ExceptionLogger(trace);
            }
        }

        // Changes for TimeZone Feature start
        private void GetHistoryTripDetails_NewTZ()
        {
            try
            {
                Response.ContentType = "text/html";
                Response.ContentEncoding = Encoding.Default;
                result _r = new result();

                AddSpeed_1Column();
                string tripid = Request["tripId"];
                string filter = "TripId=" + tripid;
                DataTable dt = sn.History.DsHistoryInfo.Tables["TripDetails"].Select(filter).CopyToDataTable();
                dt.TableName = "TripDetails";

                DataSet dstemp = new DataSet();
                dstemp.Tables.Add(dt);
                dstemp.DataSetName = "HistoryTripDetailed";
                _r.data = dstemp.GetXml().Replace("<HistoryTripDetailed>", "<HistoryTripDetailed><totalCount>" + dt.Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "");
                _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());

                _r.success = true;
                _r.msg = "";
                Response.Write(new JavaScriptSerializer().Serialize(_r));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                //ExceptionLogger(trace);
            }
        }
        // Changes for TimeZone Feature end


        private void GetHistoryTripDetails()
        {
            try
            {
                Response.ContentType = "text/html";
                Response.ContentEncoding = Encoding.Default;
                result _r = new result();

                AddSpeed_1Column();
                string tripid = Request["tripId"];
                string filter = "TripId=" + tripid;
                DataTable dt = sn.History.DsHistoryInfo.Tables["TripDetails"].Select(filter).CopyToDataTable();
                dt.TableName = "TripDetails";

                DataSet dstemp = new DataSet();
                dstemp.Tables.Add(dt);
                dstemp.DataSetName = "HistoryTripDetailed";
                _r.data = dstemp.GetXml().Replace("<HistoryTripDetailed>", "<HistoryTripDetailed><totalCount>" + dt.Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "");
                _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());

                _r.success = true;
                _r.msg = "";
                Response.Write(new JavaScriptSerializer().Serialize(_r));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                //ExceptionLogger(trace);
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

        // Changes for TimeZone Feature start
        private void dgHistory_Fill_NewTZ(Int64 VehicleId, string strFromDate, string strToDate)
        {
            result _r = new result();
            try
            {
                DataSet dsHistory = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();
                dbh.Timeout = -1;
                bool RequestOverflowed = false;
                string strPath = Server.MapPath("../Datasets/Hist.xsd");

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

                                if (RequestOverflowed)
                                {
                                    sn.MessageText = (string)base.GetLocalResourceObject("lblTooManyRecordsMessage1Resource1") + ". " + (string)base.GetLocalResourceObject("lblTooManyRecordsMessage2Resource1");
                                    _r.success = false;
                                    _r.msg = sn.MessageText;
                                    _r.data = "";
                                    Response.Write(new JavaScriptSerializer().Serialize(_r));

                                    return;
                                }
                                else
                                {
                                    _r.success = false;
                                    _r.msg = sn.MessageText;
                                    _r.data = "";
                                    Response.Write(new JavaScriptSerializer().Serialize(_r));

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
                                    _r.success = false;
                                    _r.msg = sn.MessageText;
                                    _r.data = "";
                                    Response.Write(new JavaScriptSerializer().Serialize(_r));
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
                        _r.success = false;
                        _r.msg = sn.MessageText;
                        _r.data = "";
                        Response.Write(new JavaScriptSerializer().Serialize(_r));
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
                        _r.success = false;
                        _r.msg = (string)base.GetLocalResourceObject("lblMessage_Text_NoHistory");
                        _r.data = "";
                        Response.Write(new JavaScriptSerializer().Serialize(_r));
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
                    dsHistory.DataSetName = "MsgInHistory";
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

                //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get History -->Database Call(sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + sn.History.VehicleId + "," + sn.History.FromDateTime + "," + sn.History.ToDateTime));

                ProcessHistoryData(ref dsHistory);

                sn.History.DsHistoryInfo = dsHistory;

                //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get History -->Data manipulation (sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + sn.History.VehicleId + "," + sn.History.FromDateTime + "," + sn.History.ToDateTime));

                if ((sn.MessageText != "") && (sn.Message.ToString().Length > 0))
                {
                    //ShowErrorMessage();
                    _r.success = false;
                    _r.msg = sn.MessageText;
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
                }
                else
                {
                    /*
                    _r.success = true;
                    _r.msg = dsHistory.Tables[0].Rows.Count.ToString() + " records";
                    if (vlStart == 0 && vlLimit == 10000)
                    {
                        _r.data = dsHistory.GetXml().Replace("\r\n", "").Replace("&#x0;", "");
                        _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    }
                    else
                    {
                        DataSet dstemp = new DataSet();
                        DataView dv = dsHistory.Tables["VehicleStatusHistory"].DefaultView;
                        //dv.Sort = "OriginDateTime DESC";
                        DataTable sortedTable = dv.ToTable();
                        DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                        dt.TableName = "VehicleStatusHistory";
                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "MsgInHistory";
                        _r.data = dstemp.GetXml().Replace("<MsgInHistory>", "<MsgInHistory><totalCount>" + dsHistory.Tables["VehicleStatusHistory"].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                        _r.wholedata = dsHistory.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    }
                    _r.iconTypeName = sn.History.IconTypeName;
                    var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
                    Response.Write(serializer.Serialize(_r));
                    //Response.Write(new JavaScriptSerializer().Serialize(_r));
                    */


                    Response.Write("{\"success\":true,\"data\":\"");
                    if (vlStart == 0 && vlLimit == 10000)
                    {
                        getXmlFromDs(dsHistory, 0);
                        Response.Write("\"");
                    }
                    else
                    {
                        DataSet dstemp = new DataSet();
                        DataView dv = dsHistory.Tables["VehicleStatusHistory"].DefaultView;
                        //dv.Sort = "OriginDateTime DESC";
                        DataTable sortedTable = dv.ToTable();
                        DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                        dt.TableName = "VehicleStatusHistory";
                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "MsgInHistory";

                        getXmlFromDs(dstemp, dsHistory.Tables["VehicleStatusHistory"].Rows.Count);
                        Response.Write("\"");

                        Response.Write(",\"wholedata\":\"");
                        getXmlFromDs(dsHistory, 0);
                        Response.Write("\"");

                        //_r.data = dstemp.GetXml().Replace("<MsgInHistory>", "<MsgInHistory><totalCount>" + dsHistory.Tables["VehicleStatusHistory"].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                        //_r.wholedata = dsHistory.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());

                    }
                    Response.Write("}");
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                _r.success = false;
                _r.msg = Ex.Message;
                _r.data = "";
                Response.Write(new JavaScriptSerializer().Serialize(_r));
                return;
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " " + Ex.StackTrace.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                _r.success = false;
                _r.msg = Ex.Message;
                _r.msgDetails = Ex.StackTrace.ToString();
                _r.data = "";
                Response.Write(new JavaScriptSerializer().Serialize(_r));
                return;
                //ExceptionLogger(trace);
            }
        }
        // Changes for TimeZone Feature end

        private void dgHistory_Fill(Int64 VehicleId, string strFromDate, string strToDate)
        {
            result _r = new result();
            try
            {
                DataSet dsHistory = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();
                dbh.Timeout = -1;
                bool RequestOverflowed = false;
                string strPath = Server.MapPath("../Datasets/Hist.xsd");

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
                                    _r.success = false;
                                    _r.msg = sn.MessageText;
                                    _r.data = "";
                                    Response.Write(new JavaScriptSerializer().Serialize(_r));

                                    return;
                                }
                                else
                                {
                                    _r.success = false;
                                    _r.msg = sn.MessageText;
                                    _r.data = "";
                                    Response.Write(new JavaScriptSerializer().Serialize(_r));

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
                                    _r.success = false;
                                    _r.msg = sn.MessageText;
                                    _r.data = "";
                                    Response.Write(new JavaScriptSerializer().Serialize(_r));
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
                        _r.success = false;
                        _r.msg = sn.MessageText;
                        _r.data = "";
                        Response.Write(new JavaScriptSerializer().Serialize(_r));
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
                        _r.success = false;
                        _r.msg = (string)base.GetLocalResourceObject("lblMessage_Text_NoHistory");
                        _r.data = "";
                        Response.Write(new JavaScriptSerializer().Serialize(_r));
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
                    dsHistory.DataSetName = "MsgInHistory";
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

                //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get History -->Database Call(sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + sn.History.VehicleId + "," + sn.History.FromDateTime + "," + sn.History.ToDateTime));

                ProcessHistoryData(ref dsHistory);

                sn.History.DsHistoryInfo = dsHistory;

                //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get History -->Data manipulation (sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + sn.History.VehicleId + "," + sn.History.FromDateTime + "," + sn.History.ToDateTime));

                if ((sn.MessageText != "") && (sn.Message.ToString().Length > 0))
                {
                    //ShowErrorMessage();
                    _r.success = false;
                    _r.msg = sn.MessageText;
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
                }
                else
                {
                    /*
                    _r.success = true;
                    _r.msg = dsHistory.Tables[0].Rows.Count.ToString() + " records";
                    if (vlStart == 0 && vlLimit == 10000)
                    {
                        _r.data = dsHistory.GetXml().Replace("\r\n", "").Replace("&#x0;", "");
                        _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    }
                    else
                    {
                        DataSet dstemp = new DataSet();
                        DataView dv = dsHistory.Tables["VehicleStatusHistory"].DefaultView;
                        //dv.Sort = "OriginDateTime DESC";
                        DataTable sortedTable = dv.ToTable();
                        DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                        dt.TableName = "VehicleStatusHistory";
                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "MsgInHistory";
                        _r.data = dstemp.GetXml().Replace("<MsgInHistory>", "<MsgInHistory><totalCount>" + dsHistory.Tables["VehicleStatusHistory"].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                        _r.wholedata = dsHistory.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    }
                    _r.iconTypeName = sn.History.IconTypeName;
                    var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
                    Response.Write(serializer.Serialize(_r));
                    //Response.Write(new JavaScriptSerializer().Serialize(_r));
                    */


                    Response.Write("{\"success\":true,\"data\":\"");
                    if (vlStart == 0 && vlLimit == 10000)
                    {
                        getXmlFromDs(dsHistory, 0);
                        Response.Write("\"");
                    }
                    else
                    {
                        DataSet dstemp = new DataSet();
                        DataView dv = dsHistory.Tables["VehicleStatusHistory"].DefaultView;
                        //dv.Sort = "OriginDateTime DESC";
                        DataTable sortedTable = dv.ToTable();
                        DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                        dt.TableName = "VehicleStatusHistory";
                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "MsgInHistory";

                        getXmlFromDs(dstemp, dsHistory.Tables["VehicleStatusHistory"].Rows.Count);
                        Response.Write("\"");

                        Response.Write(",\"wholedata\":\"");
                        getXmlFromDs(dsHistory, 0);
                        Response.Write("\"");

                        //_r.data = dstemp.GetXml().Replace("<MsgInHistory>", "<MsgInHistory><totalCount>" + dsHistory.Tables["VehicleStatusHistory"].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                        //_r.wholedata = dsHistory.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());

                    }
                    Response.Write("}");
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                _r.success = false;
                _r.msg = Ex.Message;
                _r.data = "";
                Response.Write(new JavaScriptSerializer().Serialize(_r));
                return;
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " " + Ex.StackTrace.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                _r.success = false;
                _r.msg = Ex.Message;
                _r.msgDetails = Ex.StackTrace.ToString();
                _r.data = "";
                Response.Write(new JavaScriptSerializer().Serialize(_r));
                return;
                //ExceptionLogger(trace);
            }
        }
        private void ProcessHistoryData(ref DataSet dsHistory)
        {
            bool ShowMultiColor = clsPermission.FeaturePermissionCheck(sn, "ShowMultiColor");
            DataSet dbtemp = null;
            if (ShowMultiColor)
            {
                string vehicle = dsHistory.Tables[0].Rows[0]["VehicleId"].ToString();
                dbtemp = ServiceAssignment.GetColorRuleByVehicle(vehicle);
            }

            int RowsCount = dsHistory.Tables[0].Rows.Count - 1;
            sn.MessageText = "";

            if (dsHistory.Tables[0].Columns.IndexOf("StreetAddress") == -1)
            {
                DataColumn colStreetAddress = new DataColumn("StreetAddress", Type.GetType("System.String"));
                dsHistory.Tables[0].Columns.Add(colStreetAddress);
            }

            DataColumn colDateTime = new DataColumn("MyDateTime", Type.GetType("System.DateTime"));
            dsHistory.Tables[0].Columns.Add(colDateTime);

            DataColumn colPTO = new DataColumn("PTO", Type.GetType("System.String"));
            dsHistory.Tables[0].Columns.Add(colPTO);

            DataColumn colColor = new DataColumn("TripColor", Type.GetType("System.String"));
            dsHistory.Tables[0].Columns.Add(colColor);

            DataColumn custSpeed = new DataColumn("custSpeed", Type.GetType("System.Double"));
            dsHistory.Tables[0].Columns.Add(custSpeed);

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

            // icon
            if (dsHistory.Tables[0].Columns.IndexOf("icon") == -1)
            {
                dc = new DataColumn("icon", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dsHistory.Tables[0].Columns.Add(dc);
            }

            // HistoryInfoId
            dc = new DataColumn();
            dc.ColumnName = "HistoryInfoId";
            dc.DataType = Type.GetType("System.Int32");
            dc.DefaultValue = 0;
            dsHistory.Tables[0].Columns.Add(dc);

            // TimeDifference
            dc = new DataColumn();
            dc.ColumnName = "TimeDifference";
            dc.DataType = Type.GetType("System.Int64");
            dc.DefaultValue = 0L;
            dsHistory.Tables[0].Columns.Add(dc);

            if (!dsHistory.Tables[0].Columns.Contains("LatLon"))
            {
                dc = new DataColumn("LatLon", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dsHistory.Tables[0].Columns.Add(dc);
            }

            if (ShowHistoryDetails)
            {
                //'CP_Fuel', 'CP_Odometer', 'CP_RPM', 'CP_FLIP', 'CP_FLIS', 'CP_SeatBelt', 'CP_MIL', 'CP_CLT', 'CP_EOT', 'CP_EOP'
                dsHistory.Tables[0].Columns.Add("CP_Fuel", typeof(String));
                dsHistory.Tables[0].Columns.Add("CP_Odometer", typeof(String));
                dsHistory.Tables[0].Columns.Add("CP_RPM", typeof(String));
                dsHistory.Tables[0].Columns.Add("CP_FLIP", typeof(String));
                dsHistory.Tables[0].Columns.Add("CP_FLIS", typeof(String));
                dsHistory.Tables[0].Columns.Add("CP_SeatBelt", typeof(String));
                dsHistory.Tables[0].Columns.Add("CP_MIL", typeof(String));
                dsHistory.Tables[0].Columns.Add("CP_CLT", typeof(String));
                dsHistory.Tables[0].Columns.Add("CP_EOT", typeof(String));
                dsHistory.Tables[0].Columns.Add("CP_EOP", typeof(String));
            }

            int i = 0;
            string strStreetAddress = "";

            UInt64 checkBit = 0;
            Int16 bitnum = 31;
            UInt64 shift = 1;
            UInt64 intSensorMask = 0;

            if (sn.History.IconTypeName == "")
                sn.History.IconTypeName = "Circle";

            foreach (DataRow rowItem in dsHistory.Tables[0].Rows)
            {
                //Key 
                rowItem["dgKey"] = i.ToString();
                //CustomUrl 
                rowItem["CustomUrl"] = "javascript:var w =HistoryInfo('" + i.ToString() + "')";
                rowItem["HistoryInfoId"] = i;
                // Date
                rowItem["MyDateTime"] = Convert.ToDateTime(rowItem["OriginDateTime"].ToString());

                if (rowItem["CustomProp"] != null)
                    rowItem["CustomProp"] = rowItem["CustomProp"].ToString().Replace(System.Environment.NewLine, " ").Replace("\n", " ");

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

                        rowItem["icon"] = "Green" + sn.History.IconTypeName + rowItem["MyHeading"] + ".ico";
                    }
                }
                if (dsHistory.Tables[0].Columns.IndexOf("Speed") != -1)
                {
                    try
                    {
                        rowItem["custSpeed"] = Convert.ToDouble(rowItem["Speed"].ToString());
                    }
                    catch
                    {
                        rowItem["custSpeed"] = Convert.ToDouble("0");
                    }

                }
                if (rowItem["icon"] == "")
                {
                    rowItem["icon"] = "red" + sn.History.IconTypeName + ".ico";
                }

                if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.Idling)))
                {
                    rowItem["icon"] = "orange" + sn.History.IconTypeName + ".ico";
                }

                if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.ExtremeAcceleration)) ||
                    (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.ExtremeBraking)) ||
                    (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.HarshAcceleration)) ||
                    (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.HarshBraking)) ||
                    (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.SeatBelt)))
                    rowItem["Speed"] = VLF.CLS.Def.Const.defNA.ToString();

                if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.Idling))
                      && (rowItem["MsgDetails"].ToString().TrimEnd() == "Duration: 00:00:00"))
                {
                    rowItem["MsgDetails"] = (string)base.GetLocalResourceObject("Text_StartIdling");
                }


                if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == 254))
                {
                    rowItem["MsgDetails"] = rowItem["CustomProp"];
                }

                if (sn.User.OrganizationId == 999650 && (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == 1 || Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == 2))
                {
                    rowItem["MsgDetails"] += "Temp:" + VLF.CLS.Util.PairFindValue("TEMP", rowItem["CustomProp"].ToString());
                }

                if (VLF.CLS.Util.PairFindValue("ATEMP", rowItem["CustomProp"].ToString().Trim()) != "")
                {
                    rowItem["MsgDetails"] = "Air:" + VLF.CLS.Util.PairFindValue("ATEMP", rowItem["CustomProp"].ToString().Trim()) + " °C";
                    rowItem["MsgDetails"] += "; " + "Road:" + VLF.CLS.Util.PairFindValue("RTEMP", rowItem["CustomProp"].ToString().Trim()) + " °C";
                }

                //Harsh Drive
                if (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == 119 && VLF.CLS.Util.PairFindValue("3Axis", rowItem["CustomProp"].ToString().Trim()) != "")
                {
                    string tmpHarshDrive = VLF.CLS.Util.PairFindValue("3Axis", rowItem["CustomProp"].ToString().Trim());
                    tmpHarshDrive = tmpHarshDrive.Replace(":", "=").Replace("\"", "").Replace(",", ";");

                    rowItem["MsgDetails"] += "Peak:" + VLF.CLS.Util.PairFindValue("PEAK", tmpHarshDrive) + " ";
                }

                if (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.Spreader) && rowItem["CustomProp"].ToString().Contains("RRCS"))
                {
                    string tmpSpreaderData = rowItem["CustomProp"].ToString().Replace(":", "=").Replace("\"", "").Replace(",", ";");
                    rowItem["MsgDetails"] = "Status:" + VLF.CLS.Util.PairFindValue("SDCStatus", tmpSpreaderData) + ";Mode:" + VLF.CLS.Util.PairFindValue("SDCSMode", tmpSpreaderData) + ";Solid Rate:" + VLF.CLS.Util.PairFindValue("SolidRate", tmpSpreaderData) + ";Liquid Rate:" + VLF.CLS.Util.PairFindValue("LiquidRate", tmpSpreaderData)
                    + ";Air Temp:" + VLF.CLS.Util.PairFindValue("AirTemperature", tmpSpreaderData) + ";Road Temp:" + VLF.CLS.Util.PairFindValue("RoadTemperature", tmpSpreaderData) + ";Solid Rate Set Point:" + VLF.CLS.Util.PairFindValue("SolidRateSetpoint", tmpSpreaderData)
                    + ";Liquid Rate Set Point:" + VLF.CLS.Util.PairFindValue("LiquidRateSetpoint", tmpSpreaderData) + ";Spreading Width Set Point:" + VLF.CLS.Util.PairFindValue("SpreadingWidthSetpoint", tmpSpreaderData)
                    + ";Gate Position:" + VLF.CLS.Util.PairFindValue("GatePosition", tmpSpreaderData) + ";Solid Material:" + VLF.CLS.Util.PairFindValue("SolidMaterial", tmpSpreaderData) + ";Liquid Material:" + VLF.CLS.Util.PairFindValue("LiquidMaterial", tmpSpreaderData);
                }

                if (sn.User.UserGroupId == 1)
                {
                    rowItem["MsgDetails"] += "[" + rowItem["CustomProp"].ToString() + "]";
                }

                if (rowItem["MsgDetails"] != null)
                    rowItem["MsgDetails"] = rowItem["MsgDetails"].ToString().Replace(System.Environment.NewLine, " ").Replace("\n", " ");


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

                //Geting and Inserting PTO Value
                intSensorMask = 0;
                try
                {
                    if (!rowItem["SensorMask"].ToString().Contains("-"))
                        intSensorMask = Convert.ToUInt64(rowItem["SensorMask"]);
                    else
                    {
                        string getSensor;
                        int j = i + 1;
                        do
                        {
                            try
                            {
                                getSensor = dsHistory.Tables[0].Rows[j]["SensorMask"].ToString();
                                j++;
                            }
                            catch
                            {
                                getSensor = "0";
                            }
                        } while (getSensor.ToString().Contains("-"));
                        intSensorMask = Convert.ToUInt64(getSensor);
                    }
                }
                catch
                {
                }
                checkBit = 0x80;
                if ((intSensorMask & checkBit) != 0)
                    rowItem["PTO"] = "On";
                else
                    rowItem["PTO"] = "Off";
                if (rowItem["PTO"] == "On")
                {
                    rowItem["icon"] = "Blue" + sn.History.IconTypeName + ".ico";
                }

                if (rowItem["ValidGps"].ToString() == "0")
                {
                    rowItem["LatLon"] = rowItem["Latitude"].ToString() + ", " + rowItem["Longitude"].ToString();
                }
                else
                    rowItem["LatLon"] = "N/A";

                if (ShowMultiColor)
                {
                    if (dbtemp != null && dbtemp.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in dbtemp.Tables[0].Rows)
                        {
                            rowItem["TripColor"] = getColor(rowItem["SensorMask"].ToString(), r[0].ToString());
                            if (rowItem["TripColor"] != "")
                            {
                                break;
                            }
                        }
                    }
                    if (rowItem["TripColor"].ToString() == "")
                    {
                        rowItem["TripColor"] = "#0000ff";
                    }
                }

                i++;
                if (ShowHistoryDetails)
                {
                    //'CP_Fuel', 'CP_Odometer', 'CP_RPM', 'CP_FLIP', 'CP_FLIS', 'CP_SeatBelt', 'CP_MIL', 'CP_CLT', 'CP_EOT', 'CP_EOP'
                    string CP_Fuel = string.Empty;
                    string CP_Odometer = string.Empty;
                    string CP_RPM = string.Empty;
                    string CP_FLIP = string.Empty;
                    string CP_FLIS = string.Empty;
                    string CP_SeatBelt = string.Empty;
                    string CP_MIL = string.Empty;
                    string CP_CLT = string.Empty;
                    string CP_EOT = string.Empty;
                    string CP_EOP = string.Empty;

                    string s = string.Empty;
                    int _value;
                    double _dbvalue;

                    //CP_Fuel = VLF.CLS.Util.PairFindValue("FUEL", rowItem["CustomProp"].ToString().Trim());
                    CP_Fuel = FindValueFromPair("FUEL", rowItem["CustomProp"].ToString().Trim(), ";", "=");
                    CP_Odometer = VLF.CLS.Util.PairFindValue("Odometer", rowItem["CustomProp"].ToString().Trim());
                    //CP_RPM = VLF.CLS.Util.PairFindValue("RPM", rowItem["CustomProp"].ToString().Trim());
                    CP_RPM = FindValueFromPair("RPM", rowItem["CustomProp"].ToString().Trim(), ";", "=");

                    //CP_FLIP = FindValueFromPair("FLIP", rowItem["CustomProp"].ToString().Trim(), ";", "=");
                    s = FindValueFromPair("FLIP", rowItem["CustomProp"].ToString().Trim(), ";", "=");
                    _dbvalue = -1;
                    double.TryParse(s, out _dbvalue);
                    if (_dbvalue > 0)
                        CP_FLIP = _dbvalue.ToString();

                    //CP_FLIS = FindValueFromPair("FLIS", rowItem["CustomProp"].ToString().Trim(), ";", "=");
                    s = FindValueFromPair("FLIS", rowItem["CustomProp"].ToString().Trim(), ";", "=");
                    _value = -1;
                    int.TryParse(s, out _value);
                    if (_value > 0)
                        CP_FLIS = _value.ToString();

                    //s = VLF.CLS.Util.PairFindValue("STA", rowItem["CustomProp"].ToString().Trim());
                    s = FindValueFromPair("STA", rowItem["CustomProp"].ToString().Trim(), ";", "=");
                    int _sta = -1;
                    if (s != "")
                    {
                        int.TryParse(s, out _sta);
                        if (_sta >= 0)
                        {
                            _sta = _sta & 3;
                            if (_sta == 0)
                                CP_SeatBelt = "NOT Buckled";
                            else if (_sta == 1)
                                CP_SeatBelt = "OK";
                            else if (_sta == 2)
                                CP_SeatBelt = "Error";
                            else if (_sta == 3)
                                CP_SeatBelt = "NA";
                        }
                    }


                    CP_MIL = FindValueFromPair("MIL", rowItem["CustomProp"].ToString().Trim(), ";", "=");
                    CP_CLT = FindValueFromPair("CLT", rowItem["CustomProp"].ToString().Trim(), ";", "=");

                    CP_EOT = FindValueFromPair("EOT", rowItem["CustomProp"].ToString().Trim(), ";", "=");
                    /*s = FindValueFromPair("EOT", rowItem["CustomProp"].ToString().Trim(), ";", "=");
                    if (s != "")
                    {
                        _value = int.Parse(s);
                        CP_EOT = String.Format("{0:0.00}", _value * 0.00001);
                    }*/

                    CP_EOP = FindValueFromPair("EOP", rowItem["CustomProp"].ToString().Trim(), ";", "=");

                    rowItem["CP_Fuel"] = CP_Fuel;
                    rowItem["CP_Odometer"] = CP_Odometer;
                    rowItem["CP_RPM"] = CP_RPM;
                    rowItem["CP_FLIP"] = CP_FLIP;
                    rowItem["CP_FLIS"] = CP_FLIS;
                    rowItem["CP_SeatBelt"] = CP_SeatBelt;
                    rowItem["CP_MIL"] = CP_MIL;
                    rowItem["CP_CLT"] = CP_CLT;
                    rowItem["CP_EOT"] = CP_EOT;
                    rowItem["CP_EOP"] = CP_EOP;
                }

                long duration = 0L;
                try
                {
                    duration = Convert.ToDateTime(rowItem["DateTimeReceived"]).Ticks - Convert.ToDateTime(rowItem["OriginDateTime"]).Ticks;
                }
                catch
                {
                    duration = Convert.ToDateTime(rowItem["DateTimeReceived"]).Ticks - Convert.ToDateTime(rowItem["OriginDateTime"], new System.Globalization.CultureInfo("fr-FR")).Ticks;
                }
                duration = duration / TimeSpan.TicksPerMinute;
                if (sn.User.UserGroupId == 1 || sn.User.OrganizationId == 952)
                    if ((duration > 2 && (Int16)rowItem["BoxMsgInTypeId"] != 25) || (duration > 5 && (Int16)rowItem["BoxMsgInTypeId"] == 25))
                        rowItem["TimeDifference"] = duration;
            }
        }

        string getColor(string SensorMask, string ruleset)
        {
            string color = "";
            try
            {
                if (!SensorMask.Contains("-"))
                {
                    if (ruleset.Contains("SensorMaskBit"))
                    {
                        string[] rules = ruleset.Split(';');
                        Dictionary<string, string> d = new Dictionary<string, string>();
                        foreach (string s in rules)
                        {
                            d.Add(s.Split('=')[0], s.Split('=')[1]);
                        }
                        string[] sensormasks = (d["SensorMaskBit"] + ",").Split(',');
                        foreach (string sm in sensormasks)
                        {
                            bool ifbiton = false;
                            if (String.IsNullOrEmpty(sm))
                                continue;
                            if (sm.Contains("!"))
                            {
                                if (sm.Contains("|"))
                                {
                                    foreach (string s in sm.Split('|'))
                                    {
                                        if (s.StartsWith("!"))
                                        {
                                            if ((Convert.ToUInt64(SensorMask) & Convert.ToUInt64(s.Replace("!", ""))) != Convert.ToUInt64(s.Replace("!", "")))
                                                ifbiton = true;
                                            else
                                            {
                                                ifbiton = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if ((Convert.ToUInt64(SensorMask) & Convert.ToUInt64(s)) == Convert.ToUInt64(s))
                                                ifbiton = true;
                                            else
                                            {
                                                ifbiton = false;
                                                break;
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    if ((Convert.ToUInt64(SensorMask) & Convert.ToUInt64(sm.Replace("!", ""))) != Convert.ToUInt64(sm.Replace("!", "")))
                                        ifbiton = true;
                                    else
                                        ifbiton = false;
                                }
                            }
                            else
                            {
                                if (sm.Contains("|"))
                                {
                                    foreach (string s in sm.Split('|'))
                                    {
                                        if ((Convert.ToUInt64(SensorMask) & Convert.ToUInt64(s)) == Convert.ToUInt64(s))
                                            ifbiton = true;
                                        else
                                        {
                                            ifbiton = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                    if ((Convert.ToUInt64(SensorMask) & Convert.ToUInt64(sm)) == Convert.ToUInt64(sm))
                                        ifbiton = true;
                                    else
                                        ifbiton = false;
                            }
                            if (ifbiton)
                            {
                                color = d["Color"];
                                break;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                color = null;
            }

            return color;
        }

        // Changes For TimeZone Feature start
        private void dgStops_Fill_NewTZ(Int64 VehicleId, string strFromDate, string strToDate)
        {
            try
            {
                DataSet dsHistory = new DataSet();
                StringReader strrXML = null;
                result _r = new result();

                string xml = "";

                DataSet ds = new DataSet();
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId_NewTZ(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId_NewTZ(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                    {
                        sn.History.ImgPath = sn.Map.DefaultImgPath;
                        sn.Map.VehiclesMappings = "";
                        sn.Map.VehiclesToolTip = "";

                        _r.success = false;
                        _r.msg = "";
                        _r.data = "";
                        Response.Write(new JavaScriptSerializer().Serialize(_r));

                        return;
                    }

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";

                    _r.success = false;
                    _r.msg = "";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));

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

                            _r.success = false;
                            _r.msg = "";
                            _r.data = "";
                            Response.Write(new JavaScriptSerializer().Serialize(_r));

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

                    _r.success = false;
                    _r.msg = "";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));

                    return;
                }

                strrXML = new StringReader(xml);

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";

                    _r.success = true;
                    _r.msg = "";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
                    return;
                }

                dsHistory.ReadXml(strrXML);

                if (dsHistory.Tables.IndexOf("StopData") == -1)
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";

                    _r.success = true;
                    _r.msg = "";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));

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

                //dgStops.Visible = true;
                //dgHistoryDetails.Visible = false;

                sn.History.DsHistoryInfo = dsHistory;
                //sn.MapSolute.LoadVehicles(sn, dsHistory.Tables["StopData"], "HistoryStops");
                //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb && !sn.History.Animated)
                //    sn.MapSolute.LoadHistory(sn, dsHistory.Tables["StopData"], "HistoryStops");


                _r.success = true;
                _r.msg = dsHistory.Tables[0].Rows.Count.ToString() + " records";
                //if (vlStart == 0 && vlLimit == 10000)
                //{
                _r.data = dsHistory.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                //}
                /*else
                {
                    DataSet dstemp = new DataSet();
                    DataView dv = dsHistory.Tables["VehicleStatusHistory"].DefaultView;
                    //dv.Sort = "OriginDateTime DESC";
                    DataTable sortedTable = dv.ToTable();
                    DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                    dt.TableName = "VehicleStatusHistory";
                    dstemp.Tables.Add(dt);
                    dstemp.DataSetName = "MsgInHistory";
                    _r.data = dstemp.GetXml().Replace("<MsgInHistory>", "<MsgInHistory><totalCount>" + dsHistory.Tables["VehicleStatusHistory"].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "");
                }*/

                var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
                Response.Write(serializer.Serialize(_r));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                //ExceptionLogger(trace);
            }
        }
        // Changes for TimeZone Feature end

        private void dgStops_Fill(Int64 VehicleId, string strFromDate, string strToDate)
        {
            try
            {
                DataSet dsHistory = new DataSet();
                StringReader strrXML = null;
                result _r = new result();

                string xml = "";

                DataSet ds = new DataSet();
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                    {
                        sn.History.ImgPath = sn.Map.DefaultImgPath;
                        sn.Map.VehiclesMappings = "";
                        sn.Map.VehiclesToolTip = "";

                        _r.success = false;
                        _r.msg = "";
                        _r.data = "";
                        Response.Write(new JavaScriptSerializer().Serialize(_r));

                        return;
                    }

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";

                    _r.success = false;
                    _r.msg = "";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));

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

                            _r.success = false;
                            _r.msg = "";
                            _r.data = "";
                            Response.Write(new JavaScriptSerializer().Serialize(_r));

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

                    _r.success = false;
                    _r.msg = "";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));

                    return;
                }

                strrXML = new StringReader(xml);

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";

                    _r.success = true;
                    _r.msg = "";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
                    return;
                }

                dsHistory.ReadXml(strrXML);

                if (dsHistory.Tables.IndexOf("StopData") == -1)
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";

                    _r.success = true;
                    _r.msg = "";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));

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

                //dgStops.Visible = true;
                //dgHistoryDetails.Visible = false;

                sn.History.DsHistoryInfo = dsHistory;
                //sn.MapSolute.LoadVehicles(sn, dsHistory.Tables["StopData"], "HistoryStops");
                //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb && !sn.History.Animated)
                //    sn.MapSolute.LoadHistory(sn, dsHistory.Tables["StopData"], "HistoryStops");


                _r.success = true;
                _r.msg = dsHistory.Tables[0].Rows.Count.ToString() + " records";
                //if (vlStart == 0 && vlLimit == 10000)
                //{
                _r.data = dsHistory.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                //}
                /*else
                {
                    DataSet dstemp = new DataSet();
                    DataView dv = dsHistory.Tables["VehicleStatusHistory"].DefaultView;
                    //dv.Sort = "OriginDateTime DESC";
                    DataTable sortedTable = dv.ToTable();
                    DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                    dt.TableName = "VehicleStatusHistory";
                    dstemp.Tables.Add(dt);
                    dstemp.DataSetName = "MsgInHistory";
                    _r.data = dstemp.GetXml().Replace("<MsgInHistory>", "<MsgInHistory><totalCount>" + dsHistory.Tables["VehicleStatusHistory"].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "");
                }*/

                var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
                Response.Write(serializer.Serialize(_r));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                //ExceptionLogger(trace);
            }
        }

        // Changes for TimeZone Feature start
        private void dgTrips_Fill_NewTZ(Int64 VehicleId, string strFromDate, string strToDate)
        {
            try
            {
                DataSet dsHistory = new DataSet();
                dsHistory = new DataSet();
                StringReader strrXML = null;
                result _r = new result();

                string xml = "";

                DataSet ds = new DataSet();
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                    {
                        sn.History.ImgPath = sn.Map.DefaultImgPath;
                        sn.Map.VehiclesMappings = "";
                        sn.Map.VehiclesToolTip = "";

                        _r.success = false;
                        _r.msg = "";
                        _r.data = "";
                        Response.Write(new JavaScriptSerializer().Serialize(_r));
                        return;
                    }

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";

                    _r.success = true;
                    _r.msg = "";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
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
                        _r.success = false;
                        _r.msg = "";
                        _r.data = "";
                        Response.Write(new JavaScriptSerializer().Serialize(_r));
                        return;
                    }

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    _r.success = true;
                    _r.msg = "";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
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
                if (sn.History.DsHistoryInfo.Tables[0].Rows.Count > 0) //has trips 
                    GetTripDetails(VehicleId, strFromDate, strToDate);

                _r.success = true;
                _r.msg = dsHistory.Tables[0].Rows.Count.ToString() + " records";

                DataSet tripsummary = new DataSet();
                DataSet tripdetails = new DataSet();

                tripsummary.DataSetName = "dstTripSummaryPerVehicle";
                tripdetails.DataSetName = "MsgInHistory";

                DataTable dtTemp = sn.History.DsHistoryInfo.Tables[0].Copy();
                tripsummary.Tables.Add(dtTemp);

                if (sn.History.DsHistoryInfo.Tables[0].Rows.Count > 0) //has trips 
                {
                    dtTemp = sn.History.DsHistoryInfo.Tables[1].Copy();
                    tripdetails.Tables.Add(dtTemp);
                }

                //_r.data = dsHistory.GetXml().Replace("\r\n", "").Replace("&#x0;", "");
                _r.data = tripsummary.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                _r.tripdata = tripdetails.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("<TripDetails>", "<VehicleStatusHistory>").Replace("</TripDetails>", "</VehicleStatusHistory>").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());

                var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
                Response.Write(serializer.Serialize(_r));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                //ExceptionLogger(trace);
            }
        }
        // Changes for TimeZone Feature end

        private void dgTrips_Fill(Int64 VehicleId, string strFromDate, string strToDate)
        {
            try
            {
                DataSet dsHistory = new DataSet();
                dsHistory = new DataSet();
                StringReader strrXML = null;
                result _r = new result();

                string xml = "";

                DataSet ds = new DataSet();
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                    {
                        sn.History.ImgPath = sn.Map.DefaultImgPath;
                        sn.Map.VehiclesMappings = "";
                        sn.Map.VehiclesToolTip = "";

                        _r.success = false;
                        _r.msg = "";
                        _r.data = "";
                        Response.Write(new JavaScriptSerializer().Serialize(_r));
                        return;
                    }

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";

                    _r.success = true;
                    _r.msg = "";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
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
                        _r.success = false;
                        _r.msg = "";
                        _r.data = "";
                        Response.Write(new JavaScriptSerializer().Serialize(_r));
                        return;
                    }

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    _r.success = true;
                    _r.msg = "";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
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
                if (sn.History.DsHistoryInfo.Tables[0].Rows.Count > 0) //has trips 
                    GetTripDetails_NewTZ(VehicleId, strFromDate, strToDate);

                _r.success = true;
                _r.msg = dsHistory.Tables[0].Rows.Count.ToString() + " records";

                DataSet tripsummary = new DataSet();
                DataSet tripdetails = new DataSet();

                tripsummary.DataSetName = "dstTripSummaryPerVehicle";
                tripdetails.DataSetName = "MsgInHistory";

                DataTable dtTemp = sn.History.DsHistoryInfo.Tables[0].Copy();
                tripsummary.Tables.Add(dtTemp);

                if (sn.History.DsHistoryInfo.Tables[0].Rows.Count > 0) //has trips 
                {
                    dtTemp = sn.History.DsHistoryInfo.Tables[1].Copy();
                    tripdetails.Tables.Add(dtTemp);
                }

                //_r.data = dsHistory.GetXml().Replace("\r\n", "").Replace("&#x0;", "");
                _r.data = tripsummary.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                _r.tripdata = tripdetails.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("<TripDetails>", "<VehicleStatusHistory>").Replace("</TripDetails>", "</VehicleStatusHistory>").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());

                var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
                Response.Write(serializer.Serialize(_r));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                //ExceptionLogger(trace);
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
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                //ExceptionLogger(trace);
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
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                //ExceptionLogger(trace);
            }
        }

        private string Heading(string heading)
        {
            return sn.Map.Heading(heading);
        }

        private class result
        {
            public bool success;
            public string data;
            public string msg;
            public string msgDetails = "";
            public string iconTypeName = "";
            public string tripdata = "";
            public string wholedata = "";
        }
        // Changes for TimeZone Feature start
        private string getUserTimezone_NewTZ()
        {
            return "";

            if (sn.User.NewFloatTimeZone >= 0 && sn.User.NewFloatTimeZone < 10)
                return "+0" + sn.User.NewFloatTimeZone + ":00";
            else if (sn.User.NewFloatTimeZone >= 10)
                return "+" + sn.User.NewFloatTimeZone + ":00";
            else if (sn.User.NewFloatTimeZone < 0 && sn.User.NewFloatTimeZone > -10)
                return "-0" + Math.Abs(sn.User.NewFloatTimeZone) + ":00";
            else
                return sn.User.NewFloatTimeZone + ":00";
        }
        // Changes for TimeZone Feature end

        private string getUserTimezone()
        {
            return "";

            if (sn.User.TimeZone >= 0 && sn.User.TimeZone < 10)
                return "+0" + sn.User.TimeZone + ":00";
            else if (sn.User.TimeZone >= 10)
                return "+" + sn.User.TimeZone + ":00";
            else if (sn.User.TimeZone < 0 && sn.User.TimeZone > -10)
                return "-0" + Math.Abs(sn.User.TimeZone) + ":00";
            else
                return sn.User.TimeZone + ":00";
        }

        private string FindValueFromPair(string key, string src, string seperator, string equalSign)
        {
            try
            {
                int val_start, val_end;
                int key_pos = src.IndexOf(key + equalSign);

                if (key_pos > 0)
                    key_pos = src.IndexOf(seperator + key + equalSign);

                if (key_pos != -1)
                {
                    val_start = src.IndexOf(equalSign, key_pos);
                    if (key_pos != -1)
                    {
                        val_end = src.IndexOf(seperator, val_start);

                        if (val_end != -1)
                            return src.Substring(val_start + 1, val_end - val_start - 1);
                        else
                            return src.Substring(val_start + 1, src.Length - val_start - 1);
                    }
                }
            }
            catch (Exception exc)
            {

            }
            return "";
        }
        // Changes for TimeZone Feature start
        private void getFilteredRecord_NewTZ()
        {
            try
            {
                string xml = "";
                if (sn.History.DsHistoryInfo != null)
                {
                    DataSet dstemp = new DataSet();
                    dstemp = sn.History.DsHistoryInfo.Copy();
                    DataTable sortedTable = dstemp.Tables[0];
                    int intialRowCount = sortedTable.Rows.Count;
                    string filters = Request.QueryString["filters"];
                    if (string.IsNullOrEmpty(filters))
                        filters = String.Empty;
                    string[] filterarray = filters.Split(',');
                    if (!String.IsNullOrEmpty(filters) && filterarray.Length > 0)
                    {
                        foreach (string s in filterarray)
                        {
                            string filtercol = s.Split(':')[0];
                            string filtervalue = s.Split(':')[1].Replace("\"", "");
                            if (filtervalue.Contains("type int"))
                            {
                                if (filtervalue.Contains("lt"))
                                {
                                    string col = filtervalue.Substring(filtervalue.LastIndexOf("lt")).Split(' ')[1];
                                    sortedTable = sortedTable.Select(filtercol + " <" + col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("gt"))
                                {
                                    string col = filtervalue.Substring(filtervalue.LastIndexOf("gt")).Split(' ')[1];
                                    sortedTable = sortedTable.Select(filtercol + " >" + col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("eq"))
                                {
                                    string col = filtervalue.Substring(filtervalue.LastIndexOf("eq")).Split(' ')[1];
                                    sortedTable = sortedTable.Select(string.Format("{0} = {1}", filtercol, col)).CopyToDataTable();
                                }

                            }
                            else if (filtervalue.Contains("type date"))
                            {
                                if (filtervalue.Contains("before"))
                                {
                                    string col = filtercol + " < #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("before")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + "#";
                                    sortedTable = sortedTable.Select(col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("after"))
                                {

                                    string col = filtercol + " > #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("after")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + "#";
                                    sortedTable = sortedTable.Select(col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("on"))
                                {
                                    string col = filtercol + " < #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("on")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + " 23:59:59" + "#";
                                    col += " AND " + filtercol + " > #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("on")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + " 00:00:00" + "#";
                                    sortedTable = sortedTable.Select(col).CopyToDataTable();
                                }

                            }
                            else
                                sortedTable = sortedTable.Select(string.Format("{0} LIKE '%{1}%'", filtercol, filtervalue)).CopyToDataTable();
                        }
                    }
                    string request = Request.QueryString["sorting"];
                    if (!string.IsNullOrEmpty(request))
                    {
                        sortedTable.DefaultView.Sort = request.Split(',')[0] + " " + request.Split(',')[1];
                        sortedTable = sortedTable.DefaultView.ToTable();
                    }
                    if (Request.QueryString["operation"] == "Export" && !String.IsNullOrEmpty(Request.QueryString["formattype"]))
                    {
                        request = Request.QueryString["columns"];
                        if (!string.IsNullOrEmpty(request))
                        {
                            exportDatatable(sortedTable, Request.QueryString["formattype"], request);
                            return;
                        }
                    }
                    int finalRowCount = sortedTable.Rows.Count;
                    vlStart = 0;
                    vlLimit = 10000;
                    request = Request.QueryString["start"];
                    if (!string.IsNullOrEmpty(request))
                    {
                        Int32.TryParse(request, out vlStart);
                        if (vlStart < 0) vlStart = 0;
                    }

                    request = Request.QueryString["limit"];
                    if (!string.IsNullOrEmpty(request))
                    {
                        Int32.TryParse(request, out vlLimit);
                        if (vlLimit <= 0) vlStart = vlLimit;
                    }
                    if (finalRowCount > vlStart)
                        sortedTable = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                    sortedTable.TableName = "VehicleStatusHistory";
                    if (dstemp.Tables.CanRemove(dstemp.Tables[0]))
                    {
                        dstemp.Tables.Remove(dstemp.Tables[0]);
                    }
                    dstemp.Tables.Add(sortedTable);
                    dstemp.DataSetName = "MsgInHistory";
                    xml = dstemp.GetXml();
                    if (!String.IsNullOrEmpty(filters) && filterarray.Length > 0)
                        xml = xml.Replace("<MsgInHistory>", "<MsgInHistory><totalCount>" + finalRowCount.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "");
                    else
                        xml = xml.Replace("<MsgInHistory>", "<MsgInHistory><totalCount>" + intialRowCount.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "");
                    xml = xml.Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    Response.ContentType = "text/xml";
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.Write(xml.Trim());
                }
            }
            catch
            {
            }
        }
        // Changes For TimeZone Feature end
        //Edited by Rohit Mittal
        private void getFilteredRecord()
        {
            try
            {
                string xml = "";
                if (sn.History.DsHistoryInfo != null)
                {
                    DataSet dstemp = new DataSet();
                    dstemp = sn.History.DsHistoryInfo.Copy();
                    DataTable sortedTable = dstemp.Tables[0];
                    int intialRowCount = sortedTable.Rows.Count;
                    string filters = Request.QueryString["filters"];
                    if (string.IsNullOrEmpty(filters))
                        filters = String.Empty;
                    string[] filterarray = filters.Split(',');
                    if (!String.IsNullOrEmpty(filters) && filterarray.Length > 0)
                    {
                        foreach (string s in filterarray)
                        {
                            string filtercol = s.Split(':')[0];
                            string filtervalue = s.Split(':')[1].Replace("\"", "");
                            if (filtervalue.Contains("type int"))
                            {
                                if (filtervalue.Contains("lt"))
                                {
                                    string col = filtervalue.Substring(filtervalue.LastIndexOf("lt")).Split(' ')[1];
                                    sortedTable = sortedTable.Select(filtercol + " <" + col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("gt"))
                                {
                                    string col = filtervalue.Substring(filtervalue.LastIndexOf("gt")).Split(' ')[1];
                                    sortedTable = sortedTable.Select(filtercol + " >" + col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("eq"))
                                {
                                    string col = filtervalue.Substring(filtervalue.LastIndexOf("eq")).Split(' ')[1];
                                    sortedTable = sortedTable.Select(string.Format("{0} = {1}", filtercol, col)).CopyToDataTable();
                                }

                            }
                            else if (filtervalue.Contains("type date"))
                            {
                                if (filtervalue.Contains("before"))
                                {
                                    string col = filtercol + " < #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("before")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + "#";
                                    sortedTable = sortedTable.Select(col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("after"))
                                {

                                    string col = filtercol + " > #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("after")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + "#";
                                    sortedTable = sortedTable.Select(col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("on"))
                                {
                                    string col = filtercol + " < #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("on")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + " 23:59:59" + "#";
                                    col += " AND " + filtercol + " > #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("on")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + " 00:00:00" + "#";
                                    sortedTable = sortedTable.Select(col).CopyToDataTable();
                                }

                            }
                            else
                                sortedTable = sortedTable.Select(string.Format("{0} LIKE '%{1}%'", filtercol, filtervalue)).CopyToDataTable();
                        }
                    }
                    string request = Request.QueryString["sorting"];
                    if (!string.IsNullOrEmpty(request))
                    {
                        sortedTable.DefaultView.Sort = request.Split(',')[0] + " " + request.Split(',')[1];
                        sortedTable = sortedTable.DefaultView.ToTable();
                    }
                    if (Request.QueryString["operation"] == "Export" && !String.IsNullOrEmpty(Request.QueryString["formattype"]))
                    {
                        request = Request.QueryString["columns"];
                        if (!string.IsNullOrEmpty(request))
                        {
                            exportDatatable(sortedTable, Request.QueryString["formattype"], request);
                            return;
                        }
                    }
                    int finalRowCount = sortedTable.Rows.Count;
                    vlStart = 0;
                    vlLimit = 10000;
                    request = Request.QueryString["start"];
                    if (!string.IsNullOrEmpty(request))
                    {
                        Int32.TryParse(request, out vlStart);
                        if (vlStart < 0) vlStart = 0;
                    }

                    request = Request.QueryString["limit"];
                    if (!string.IsNullOrEmpty(request))
                    {
                        Int32.TryParse(request, out vlLimit);
                        if (vlLimit <= 0) vlStart = vlLimit;
                    }
                    if (finalRowCount > vlStart)
                        sortedTable = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                    sortedTable.TableName = "VehicleStatusHistory";
                    if (dstemp.Tables.CanRemove(dstemp.Tables[0]))
                    {
                        dstemp.Tables.Remove(dstemp.Tables[0]);
                    }
                    dstemp.Tables.Add(sortedTable);
                    dstemp.DataSetName = "MsgInHistory";
                    xml = dstemp.GetXml();
                    if (!String.IsNullOrEmpty(filters) && filterarray.Length > 0)
                        xml = xml.Replace("<MsgInHistory>", "<MsgInHistory><totalCount>" + finalRowCount.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "");
                    else
                        xml = xml.Replace("<MsgInHistory>", "<MsgInHistory><totalCount>" + intialRowCount.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "");
                    xml = xml.Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    Response.ContentType = "text/xml";
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.Write(xml.Trim());
                }
            }
            catch
            {
            }
        }

        private void getXmlFromDs(DataSet ds, int c)
        {
            string lng = ((sn.SelectedLanguage != null && sn.SelectedLanguage.Length > 0) ? sn.SelectedLanguage.Substring(0, 2) : "en");
            Response.Write("\\u003c" + ds.DataSetName + "\\u003e");
            if (c > 0) Response.Write("\\u003ctotalCount\\u003e" + c.ToString() + "\\u003c/totalCount\\u003e");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Response.Write("\\u003c" + ds.Tables[0].TableName + "\\u003e");
                foreach (DataColumn column in ds.Tables[0].Columns)
                {
                    Response.Write("\\u003c" + column.ColumnName + "\\u003e");
                    string v = "";
                    if (column.DataType.Name == "DateTime")
                        v = String.Format("{0:yyyy-MM-ddTHH:mm:ss.ff}", dr[column]);
                    else
                    {
                        v = dr[column].ToString();
                        if (lng == "fr")
                        {
                            if (column.ColumnName == "BoxArmed")
                            {
                                v = v.Replace("true", "voir");
                                v = v.Replace("false", "faux");
                            }
                        }
                        v = v.Replace("&#x0", "").Replace("&", "&amp;").Replace("\0", string.Empty).Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "\\\"");
                    }
                    //byte[] data = Encoding.Default.GetBytes(v);
                    //v = Encoding.UTF8.GetString(data);
                    Response.Write(v);
                    Response.Write("\\u003c/" + column.ColumnName + "\\u003e");
                }
                Response.Write("\\u003c/" + ds.Tables[0].TableName + "\\u003e");
            }
            Response.Write("\\u003c/" + ds.DataSetName + "\\u003e");
        }

        private string getExcelDateFormat()
        {
            string dformat = "yyyy-mm-dd";
            string tformat;
            if (sn.User.DateFormat == "dd/MM/yyyy")
                dformat = "dd/mm/yyyy";
            else if (sn.User.DateFormat == "d/M/yyyy")
                dformat = "d/m/yyyy";
            else if (sn.User.DateFormat == "dd/MM/yy")
                dformat = "dd/mm/yy";
            else if (sn.User.DateFormat == "d/M/yy")
                dformat = "d/m/yy";
            else if (sn.User.DateFormat == "d MMM yyyy")
                dformat = "d mmmm yyyy";
            else if (sn.User.DateFormat == "MM/dd/yyyy")
                dformat = "mm/dd/yyyy";
            else if (sn.User.DateFormat == "M/d/yyyy")
                dformat = "m/d/yyyy";
            else if (sn.User.DateFormat == "MM/dd/yy")
                dformat = "mm/dd/yy";
            else if (sn.User.DateFormat == "M/d/yy")
                dformat = "m/d/yy";
            else if (sn.User.DateFormat == "MMMM d yy")
                dformat = "mmmm d yy";
            else if (sn.User.DateFormat == "yyyy/MM/dd")
                dformat = "yyyy/mm/dd";

            if (sn.User.TimeFormat == "hh:mm:ss tt")
                tformat = "h:mm:ss AM/PM";
            else
                tformat = "h:mm:ss";

            return dformat + " " + tformat;
        }

        private void exportDatatable(DataTable dt, string formatter, string columns)
        {
            try
            {
                string exceldtformat = getExcelDateFormat();
                if (formatter == "csv")
                {

                    StringBuilder sresult = new StringBuilder();
                    sresult.Append("sep=,");
                    sresult.Append(Environment.NewLine);
                    string header = string.Empty;
                    foreach (string column in columns.Split(';'))
                    {
                        string s = column.Split(':')[0];
                        header += "\"" + s + "\",";
                    }
                    header = header.Substring(0, header.Length - 1);
                    sresult.Append(header);
                    sresult.Append(Environment.NewLine);

                    foreach (DataRow row in dt.Rows)
                    {
                        string data = string.Empty;
                        foreach (string column in columns.Split(';'))
                        {
                            string s = row[column.Split(':')[1]].ToString();
                            if (column.Split(':')[1] == "ValidGps")
                            {
                                string validgps = (string)base.GetLocalResourceObject("Text_NA");// "N/A";
                                if (s == "0")
                                    validgps = (string)base.GetLocalResourceObject("Text_True");//"True";
                                else if (s == "1")
                                    validgps = (string)base.GetLocalResourceObject("Text_False"); //"False";

                                s = validgps;
                            }
                            else if (column.Split(':')[1] == "OriginDateTime")
                                s = Convert.ToDateTime(s).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                            data += "\"" + s.Replace("[br]", Environment.NewLine).Replace("\"", "\"\"") + "\",";
                        }
                        data = data.Substring(0, data.Length - 1);
                        sresult.Append(data);
                        sresult.Append(Environment.NewLine);
                    }

                    Response.Clear();
                    Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.csv", "vehicles"));
                    Response.Charset = Encoding.GetEncoding("iso-8859-1").BodyName;
                    Response.ContentType = "application/csv";
                    Response.ContentEncoding = Encoding.GetEncoding("iso-8859-1");
                    Response.Write(sresult.ToString());
                    Response.Flush();
                }
                else if (formatter == "excel2003")
                {
                    HSSFWorkbook wb = new HSSFWorkbook();
                    ISheet ws = wb.CreateSheet("Sheet1");
                    ICellStyle cellstyle1 = wb.CreateCellStyle();
                    ICellStyle cellstyle2 = wb.CreateCellStyle();
                    ICellStyle cellstyle3 = wb.CreateCellStyle();
                    ICellStyle cellstyle4 = wb.CreateCellStyle();
                    ICellStyle cellstyle5 = wb.CreateCellStyle();
                    cellstyle1.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    cellstyle2.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    cellstyle3.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    cellstyle4.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    cellstyle5.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    HSSFPalette palette = wb.GetCustomPalette();
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.SEA_GREEN.index, (byte)123, (byte)178, (byte)115);
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.YELLOW.index, (byte)239, (byte)215, (byte)0);
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.LIGHT_ORANGE.index, (byte)255, (byte)166, (byte)74);
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.ROSE.index, (byte)222, (byte)121, (byte)115);
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.INDIGO.index, (byte)99, (byte)125, (byte)165);
                    cellstyle1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.SEA_GREEN.index;
                    cellstyle2.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
                    cellstyle3.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_ORANGE.index;
                    cellstyle4.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.ROSE.index;
                    cellstyle5.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.INDIGO.index;
                    IRow row = ws.CreateRow(0);
                    foreach (string column in columns.Split(';'))
                    {
                        string s = column.Split(':')[0];
                        row.CreateCell(row.Cells.Count).SetCellValue(s);
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string data = string.Empty;
                        IRow rowData = ws.CreateRow(i + 1);
                        foreach (string column in columns.Split(';'))
                        {
                            if (column.Split(':')[1] == "ValidGps")
                            {
                                string validgps = (string)base.GetLocalResourceObject("Text_NA");// "N/A";
                                if (dt.Rows[i][column.Split(':')[1]].ToString() == "0")
                                    validgps = (string)base.GetLocalResourceObject("Text_True");//"True";
                                else if (dt.Rows[i][column.Split(':')[1]].ToString() == "1")
                                    validgps = (string)base.GetLocalResourceObject("Text_False"); //"False";

                                rowData.CreateCell(rowData.Cells.Count).SetCellValue(validgps);
                            }
                            else if (dt.Rows[i][column.Split(':')[1]].GetType() == System.Type.GetType("System.DateTime"))
                            {
                                ICell cell = rowData.CreateCell(rowData.Cells.Count);
                                cell.SetCellValue(Convert.ToDateTime(dt.Rows[i][column.Split(':')[1]].ToString()));
                                ICellStyle cs = wb.CreateCellStyle();
                                cs.DataFormat = wb.CreateDataFormat().GetFormat(exceldtformat);
                                cell.CellStyle = cs;
                            }
                            else
                            {
                                rowData.CreateCell(rowData.Cells.Count).SetCellValue(dt.Rows[i][column.Split(':')[1]].ToString().Replace("[br]", Environment.NewLine));
                            }

                        }
                    }

                    for (int i = 0; i < columns.Split(';').Length; i++)
                    {
                        try
                        {
                            ws.AutoSizeColumn(i);
                        }
                        catch { }
                    }

                    HttpResponse httpResponse = Response;
                    //httpResponse.Clear();
                    //httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //httpResponse.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xls", "Vehicle"));

                    HttpContext.Current.Response.Clear();
                    Response.AddHeader("Content-Type", "application/Excel");
                    Response.ContentType = "application/vnd.xls";
                    HttpContext.Current.Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xls", "Vehicle"));

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        wb.Write(memoryStream);
                        memoryStream.WriteTo(httpResponse.OutputStream);
                        memoryStream.Close();
                    }

                    HttpContext.Current.Response.End();
                }
                else if (formatter == "excel2007")
                {
                    try
                    {
                        var wb = new XLWorkbook();
                        var ws = wb.Worksheets.Add("Sheet1");
                        foreach (string column in columns.Split(';'))
                        {
                            string s = column.Split(':')[0];
                            ws.Cell(1, ws.Row(1).CellsUsed().Count() + 1).Value = s;
                        }

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string data = string.Empty;
                            int iColumn = 1;
                            foreach (string column in columns.Split(';'))
                            {

                                if (column.Split(':')[1] == "ValidGps")
                                {
                                    string validgps = (string)base.GetLocalResourceObject("Text_NA");// "N/A";
                                    if (dt.Rows[i][column.Split(':')[1]].ToString() == "0")
                                        validgps = (string)base.GetLocalResourceObject("Text_True");//"True";
                                    else if (dt.Rows[i][column.Split(':')[1]].ToString() == "1")
                                        validgps = (string)base.GetLocalResourceObject("Text_False"); //"False";

                                    ws.Cell(i + 2, iColumn).DataType = XLCellValues.Text;
                                    ws.Cell(i + 2, iColumn).Value = validgps;
                                }
                                else if (dt.Rows[i][column.Split(':')[1]].GetType() == System.Type.GetType("System.DateTime"))
                                {
                                    ws.Cell(i + 2, iColumn).DataType = XLCellValues.DateTime;
                                    ws.Cell(i + 2, iColumn).Value = dt.Rows[i][column.Split(':')[1]];
                                    ws.Cell(i + 2, iColumn).Style.DateFormat.Format = exceldtformat;
                                }
                                else
                                {
                                    ws.Cell(i + 2, iColumn).DataType = XLCellValues.Text;
                                    ws.Cell(i + 2, iColumn).Value = "'" + dt.Rows[i][column.Split(':')[1]].ToString().Replace("[br]", Environment.NewLine);
                                    if (ws.Cell(i + 2, iColumn).Value.ToString().Contains("\0\0\0\0\0\0"))
                                    {
                                        ws.Cell(i + 2, iColumn).Value = ws.Cell(i + 2, iColumn).Value.ToString().Replace("\0\0\0\0\0\0", "");
                                    }
                                }

                                iColumn++;

                            }
                        }

                        try
                        {
                            var files = new DirectoryInfo(Server.MapPath("../TempReports/")).GetFiles("*.xlsx");
                            foreach (var file in files)
                            {
                                if (DateTime.UtcNow - file.LastWriteTimeUtc > TimeSpan.FromDays(30))
                                {
                                    File.Delete(file.FullName);
                                }
                            }
                        }
                        catch { }

                        Response.Clear();
                        Response.AddHeader("Content-Type", "application/Excel");
                        Response.ContentType = "application/force-download";
                        Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xlsx", "vehicles"));
                        string filemame = string.Format(@"{0}.xlsx", Guid.NewGuid());
                        wb.SaveAs(Server.MapPath("../TempReports/") + filemame);
                        Response.TransmitFile("../TempReports/" + filemame);
                    }
                    //Peter Editted
                    catch (Exception Ex)
                    {
                        Response.Write("<script type='text/javascript'>alert('Failed to generate the file, please try it again or choose another Excel format to export.');</script>");
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }

                }
            }
            //Peter Editted
            catch (Exception Ex)
            {
                Response.Write("<script type='text/javascript'>alert('Failed to generate the file, please try it again or choose another Excel format to export.');</script>");
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
    }
}
