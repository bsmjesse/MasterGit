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

namespace SentinelFM
{
    public partial class HistoryNew_historyServices_Reefer : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;

        protected clsUtility objUtil;

        public string _xml = "";

        public string emptyCommMode;

        private int vlStart = 0;
        private int vlLimit = 10000;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
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
                    else if (request.ToLower().Trim() == "gethistoryreeferalarm")
                    {
                        GetHistoryRecords_For_Reefer_Alarm_NewTZ();
                    }
                    else if (request.ToLower().Trim() == "gethistoryreeferpretrip")
                    {
                        GetHistoryRecords_For_Reefer_Pretrip_NewTZ();
                    }
                    else if (request.ToLower().Trim() == "gethistoryreeferimpact")
                    {
                        GetHistoryRecords_For_Reefer_Impact_NewTZ();
                    }
                    else if (request.ToLower().Trim() == "gettripdetails")
                    {
                        GetHistoryTripDetails_NewTZ();
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

        private void SetHistoryVehiclesByFleetId(int fleetId)
        {
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

        }

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
            try
            {
                Response.ContentType = "text/html";
                Response.ContentEncoding = Encoding.Default;

                string xml = "";
                result _r = new result();

                bool reloadFleetVehicles = false;
                if (sn.History.FleetId == null || sn.History.FleetId != Convert.ToInt64(Request["historyFleet"].ToString()))
                    reloadFleetVehicles = true;

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
                    var JsonSerializer = new JavaScriptSerializer();
                    JsonSerializer.MaxJsonLength = Int32.MaxValue;
                    Response.Write(JsonSerializer.Serialize(_r));

                    return;

                }

                string reeferScreenName = "";
                if (Request["reeferScreenName"] != null)
                    reeferScreenName = Request["reeferScreenName"].ToString();

                string historytype = "0";
                if (Request["historyType"] != null)
                    historytype = Request["historyType"].ToString();
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


                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

                strFromDate = DateTime.ParseExact(strFromDate, sn.User.DateFormat + " h:mm tt", CultureInfo.InvariantCulture).ToString(); //Convert.ToDateTime(strFromDate, ci).ToString();
                strToDate = DateTime.ParseExact(strToDate, sn.User.DateFormat + " h:mm tt", CultureInfo.InvariantCulture).ToString(); //Convert.ToDateTime(strToDate, ci).ToString();

                if (Convert.ToDateTime(strFromDate) > Convert.ToDateTime(strToDate))
                {
                    _r.success = false;
                    _r.msg = "Invalid Date/Time";
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
                sn.History.FleetId = Convert.ToInt64(Request["historyFleet"].ToString());
                sn.History.ShowToolTip = true;
                sn.History.CarLatitude = "";
                sn.History.CarLongitude = "";
                sn.History.DsHistoryInfo = null;
                //sn.History.VehicleName = this.cboVehicle.SelectedItem.Text.Replace("'", "''");
                //sn.History.FleetName = this.cboFleet.SelectedItem.Text.Replace("'", "''");
                sn.History.DclId = -1;
                if (Request["historyCommMode"] != null)
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
                    dgHistory_Fill_NewTZ(reeferScreenName, reloadFleetVehicles, sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);
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

        private void GetHistoryRecords()
        {
            try
            {
                Response.ContentType = "text/html";
                Response.ContentEncoding = Encoding.Default;

                string xml = "";
                result _r = new result();

                bool reloadFleetVehicles = false;
                if (sn.History.FleetId == null || sn.History.FleetId != Convert.ToInt64(Request["historyFleet"].ToString()))
                    reloadFleetVehicles = true;                

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
                    var JsonSerializer = new JavaScriptSerializer();
                    JsonSerializer.MaxJsonLength = Int32.MaxValue;
                    Response.Write(JsonSerializer.Serialize(_r));

                    return;

                }

                string reeferScreenName = "";
                if (Request["reeferScreenName"] != null)
                    reeferScreenName = Request["reeferScreenName"].ToString();

                string historytype = "0";
				if (Request["historyType"] != null)
					historytype = Request["historyType"].ToString();
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


                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

                strFromDate = DateTime.ParseExact(strFromDate, sn.User.DateFormat + " h:mm tt", CultureInfo.InvariantCulture).ToString(); //Convert.ToDateTime(strFromDate, ci).ToString();
                strToDate = DateTime.ParseExact(strToDate, sn.User.DateFormat + " h:mm tt", CultureInfo.InvariantCulture).ToString(); //Convert.ToDateTime(strToDate, ci).ToString();

                if (Convert.ToDateTime(strFromDate) > Convert.ToDateTime(strToDate))
                {
                    _r.success = false;
                    _r.msg = "Invalid Date/Time";
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
                sn.History.FleetId = Convert.ToInt64(Request["historyFleet"].ToString());
                sn.History.ShowToolTip = true;
                sn.History.CarLatitude = "";
                sn.History.CarLongitude = "";
                sn.History.DsHistoryInfo = null;
                //sn.History.VehicleName = this.cboVehicle.SelectedItem.Text.Replace("'", "''");
                //sn.History.FleetName = this.cboFleet.SelectedItem.Text.Replace("'", "''");
                sn.History.DclId = -1;
				if (Request["historyCommMode"] != null)
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
                    dgHistory_Fill(reeferScreenName, reloadFleetVehicles, sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);
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

        private void GetHistoryRecords_For_Reefer_Alarm_NewTZ()
        {
            try
            {
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.Default;

                string xml = "";
                result _r = new result();

                bool reloadFleetVehicles = false;
                if (sn.History.FleetId == null || sn.History.FleetId != Convert.ToInt64(Request["historyFleet"].ToString()))
                    reloadFleetVehicles = true;

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
                    //Response.Write(new JavaScriptSerializer().Serialize(_r));
                    Response.Write(_r.data);

                    return;

                }

                //historytype=0
                sn.History.ShowTrips = false;
                sn.History.ShowStops = false;
                sn.History.ShowIdle = false;
                sn.History.ShowStopsAndIdle = false;

                string strFromDate = "";
                string strToDate = "";

                strFromDate = Request["historyDateFrom"].ToString() + " " + Request["historyTimeFrom"].ToString();
                strToDate = Request["historyDateTo"].ToString() + " " + Request["historyTimeTo"].ToString();


                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

                strFromDate = Convert.ToDateTime(strFromDate, ci).ToString();
                strToDate = Convert.ToDateTime(strToDate, ci).ToString();

                if (Convert.ToDateTime(strFromDate) > Convert.ToDateTime(strToDate))
                {
                    _r.success = false;
                    _r.msg = "Invalid Date/Time";
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
                //sn.History.VehicleId = Convert.ToInt64(Request["historyVehicle"].ToString());
                sn.History.VehicleId = 0;
                sn.History.FleetId = Convert.ToInt64(Request["historyFleet"].ToString());
                sn.History.ShowToolTip = true;
                sn.History.CarLatitude = "";
                sn.History.CarLongitude = "";
                sn.History.DsHistoryInfo = null;

                //sn.History.DclId = Convert.ToInt16(Request["historyCommMode"].ToString());
                sn.History.DclId = -1;

                sn.History.FromDateTime = strFromDate;
                sn.History.ToDateTime = strToDate;
                sn.History.Address = "";

                //sn.History.TripSensor = Convert.ToInt16(Request["historytrip"].ToString());
                sn.History.TripSensor = 3;


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
                    dgHistory_Fill_NewTZ("alarm", reloadFleetVehicles, sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);
                }

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

        private void GetHistoryRecords_For_Reefer_Alarm()
        {
            try
            {
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.Default;

                string xml = "";
                result _r = new result();

                bool reloadFleetVehicles = false;
                if (sn.History.FleetId == null || sn.History.FleetId != Convert.ToInt64(Request["historyFleet"].ToString()))
                    reloadFleetVehicles = true;

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
                    //Response.Write(new JavaScriptSerializer().Serialize(_r));
                    Response.Write(_r.data);

                    return;

                }

                //historytype=0
                sn.History.ShowTrips = false;
                sn.History.ShowStops = false;
                sn.History.ShowIdle = false;
                sn.History.ShowStopsAndIdle = false;
                
                string strFromDate = "";
                string strToDate = "";

                strFromDate = Request["historyDateFrom"].ToString() + " " + Request["historyTimeFrom"].ToString();
                strToDate = Request["historyDateTo"].ToString() + " " + Request["historyTimeTo"].ToString();


                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

                strFromDate = Convert.ToDateTime(strFromDate, ci).ToString();
                strToDate = Convert.ToDateTime(strToDate, ci).ToString();

                if (Convert.ToDateTime(strFromDate) > Convert.ToDateTime(strToDate))
                {
                    _r.success = false;
                    _r.msg = "Invalid Date/Time";
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
                //sn.History.VehicleId = Convert.ToInt64(Request["historyVehicle"].ToString());
                sn.History.VehicleId = 0;
                sn.History.FleetId = Convert.ToInt64(Request["historyFleet"].ToString());
                sn.History.ShowToolTip = true;
                sn.History.CarLatitude = "";
                sn.History.CarLongitude = "";
                sn.History.DsHistoryInfo = null;
                
                //sn.History.DclId = Convert.ToInt16(Request["historyCommMode"].ToString());
                sn.History.DclId = -1;

                sn.History.FromDateTime = strFromDate;
                sn.History.ToDateTime = strToDate;
                sn.History.Address = "";

                //sn.History.TripSensor = Convert.ToInt16(Request["historytrip"].ToString());
                sn.History.TripSensor = 3;


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
                    dgHistory_Fill("alarm", reloadFleetVehicles, sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);
                }                

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

        private void GetHistoryRecords_For_Reefer_Pretrip_NewTZ()
        {
            try
            {
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.Default;

                string xml = "";
                result _r = new result();

                bool reloadFleetVehicles = false;
                if (sn.History.FleetId == null || sn.History.FleetId != Convert.ToInt64(Request["historyFleet"].ToString()))
                    reloadFleetVehicles = true;

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
                if (!string.IsNullOrEmpty(request) && request.Trim() == "1" && sn.History.DsReeferPretrip != null)
                {
                    //sn.History.DsHistoryInfo = dsHistory;


                    _r.success = true;
                    _r.msg = sn.History.DsReeferPretrip.Tables[0].Rows.Count.ToString() + " records";
                    if (vlStart == 0 && vlLimit == 10000)
                    {
                        _r.data = sn.History.DsReeferPretrip.GetXml().Replace("\r\n", "").Replace("&#x0;", "");
                        _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    }
                    else
                    {
                        DataSet dstemp = new DataSet();
                        DataView dv = sn.History.DsReeferPretrip.Tables[0].DefaultView;
                        //dv.Sort = "OriginDateTime DESC";
                        DataTable sortedTable = dv.ToTable();
                        DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                        dt.TableName = "ReeferPretrip";
                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "ReeferPretripDataset";
                        _r.data = dstemp.GetXml().Replace("<ReeferPretripDataset>", "<ReeferPretripDataset><totalCount>" + sn.History.DsReeferPretrip.Tables[0].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "");
                        _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    }

                    //Response.Write(new JavaScriptSerializer().Serialize(_r));
                    Response.Write(_r.data);

                    return;

                }


                string strFromDate = "";
                string strToDate = "";

                strFromDate = Request["historyDateFrom"].ToString() + " " + Request["historyTimeFrom"].ToString();
                strToDate = Request["historyDateTo"].ToString() + " " + Request["historyTimeTo"].ToString();


                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

                strFromDate = Convert.ToDateTime(strFromDate, ci).ToString();
                strToDate = Convert.ToDateTime(strToDate, ci).ToString();

                if (Convert.ToDateTime(strFromDate) > Convert.ToDateTime(strToDate))
                {
                    _r.success = false;
                    _r.msg = "Invalid Date/Time";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
                    return;
                }


                //dgHistory_Fill("pretrip", reloadFleetVehicles, sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);
                string sConnectionString = ConfigurationManager.ConnectionStrings["DWH"].ConnectionString;
                VLF.DAS.Logic.EvtEvents evtEvents = new VLF.DAS.Logic.EvtEvents(sConnectionString);
                DataSet dsPretrip = evtEvents.GetAllPretripResult(sn.User.OrganizationId, sn.UserID, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving));
                evtEvents.Dispose();

                dsPretrip.DataSetName = "ReeferPretripDataset";

                if (!dsPretrip.Tables[0].Columns.Contains("PretripResult"))
                {
                    DataColumn dc = new DataColumn("PretripResult", Type.GetType("System.String"));
                    dc.DefaultValue = "";
                    dsPretrip.Tables[0].Columns.Add(dc);
                }
                if (!dsPretrip.Tables[0].Columns.Contains("BatteryVolt"))
                {
                    DataColumn dc = new DataColumn("BatteryVolt", Type.GetType("System.String"));
                    dc.DefaultValue = "";
                    dsPretrip.Tables[0].Columns.Add(dc);
                }
                if (!dsPretrip.Tables[0].Columns.Contains("FuelLevel"))
                {
                    DataColumn dc = new DataColumn("FuelLevel", Type.GetType("System.String"));
                    dc.DefaultValue = "";
                    dsPretrip.Tables[0].Columns.Add(dc);
                }

                foreach (DataRow rowItem in dsPretrip.Tables[0].Rows)
                {
                    string[] ss = rowItem["AlarmDesc"].ToString().Split(new string[] { "--" }, StringSplitOptions.None);
                    if (ss.Length > 2)
                        rowItem["AlarmDesc"] = ss[2];

                    if (rowItem["AlarmDesc"] == null || rowItem["AlarmDesc"].ToString().Trim() == "")
                        rowItem["PretripResult"] = "Pass";
                    else
                        rowItem["PretripResult"] = "Fail";

                    string v = FindValueFromPair("RD_FUEL", rowItem["Notes"].ToString(), ";", "=");
                    double dv = 0;
                    if (v != "")
                    {
                        dv = 300;
                        double.TryParse(v, out dv);
                        dv = dv * 1;
                        if (dv <= 100 && dv >= 0)
                            rowItem["FuelLevel"] = String.Format("{0:0.00}", dv);
                        else
                            rowItem["FuelLevel"] = "Invalid";
                    }

                    v = FindValueFromPair("RD_BATTERY", rowItem["Notes"].ToString(), ";", "=");
                    dv = -1;
                    double.TryParse(v, out dv);
                    rowItem["BatteryVolt"] = dv >= 0 ? String.Format("{0:0.00}", dv) : "-";

                }

                //table1.Rows.Add("FakeData1", "Remote", "2013-09-28 08:00:00", "2013-09-28 09:00:00", "Pass", 13.0, 92,"");
                //table1.Rows.Add("FakeData2", "Local", "2013-09-28 08:00:00", "2013-09-28 09:00:00", "Fail", 13.2, 23, "EVAPORATOR COIL SENSOR<br />RETURN AIR SENSOR<br />DISCHARGE AIR SENSOR");

                //dsReeferPretrip.Tables.Add(table1);
                sn.History.DsReeferPretrip = dsPretrip;

                DataSet dstemppretrip = new DataSet();
                //DataView dv = dsHistory.Tables["VehicleStatusHistory"].DefaultView;
                DataView dvprt = sn.History.DsReeferPretrip.Tables[0].DefaultView;

                DataTable sortedTablePrt = dvprt.ToTable();
                DataTable dtPrt = new DataTable();
                if (sortedTablePrt.Rows.Count > 0)
                    dtPrt = sortedTablePrt.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();

                dtPrt.TableName = "ReeferPretrip";
                dstemppretrip.Tables.Add(dtPrt);
                dstemppretrip.DataSetName = "ReeferPretripDataset";
                _r.data = dstemppretrip.GetXml().Replace("<ReeferPretripDataset>", "<ReeferPretripDataset><totalCount>" + sn.History.DsReeferPretrip.Tables[0].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                //_r.wholedata = sn.History.DsReeferPretrip.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                Response.Write(_r.data);


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

        private void GetHistoryRecords_For_Reefer_Pretrip()
        {
            try
            {
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.Default;

                string xml = "";
                result _r = new result();

                bool reloadFleetVehicles = false;
                if (sn.History.FleetId == null || sn.History.FleetId != Convert.ToInt64(Request["historyFleet"].ToString()))
                    reloadFleetVehicles = true;

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
                if (!string.IsNullOrEmpty(request) && request.Trim() == "1" && sn.History.DsReeferPretrip != null)
                {
                    //sn.History.DsHistoryInfo = dsHistory;


                    _r.success = true;
                    _r.msg = sn.History.DsReeferPretrip.Tables[0].Rows.Count.ToString() + " records";
                    if (vlStart == 0 && vlLimit == 10000)
                    {
                        _r.data = sn.History.DsReeferPretrip.GetXml().Replace("\r\n", "").Replace("&#x0;", "");
                        _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    }
                    else
                    {
                        DataSet dstemp = new DataSet();
                        DataView dv = sn.History.DsReeferPretrip.Tables[0].DefaultView;
                        //dv.Sort = "OriginDateTime DESC";
                        DataTable sortedTable = dv.ToTable();
                        DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                        dt.TableName = "ReeferPretrip";
                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "ReeferPretripDataset";
                        _r.data = dstemp.GetXml().Replace("<ReeferPretripDataset>", "<ReeferPretripDataset><totalCount>" + sn.History.DsReeferPretrip.Tables[0].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "");
                        _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    }
                    
                    //Response.Write(new JavaScriptSerializer().Serialize(_r));
                    Response.Write(_r.data);

                    return;

                }

                
                string strFromDate = "";
                string strToDate = "";

                strFromDate = Request["historyDateFrom"].ToString() + " " + Request["historyTimeFrom"].ToString();
                strToDate = Request["historyDateTo"].ToString() + " " + Request["historyTimeTo"].ToString();


                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

                strFromDate = Convert.ToDateTime(strFromDate, ci).ToString();
                strToDate = Convert.ToDateTime(strToDate, ci).ToString();

                if (Convert.ToDateTime(strFromDate) > Convert.ToDateTime(strToDate))
                {
                    _r.success = false;
                    _r.msg = "Invalid Date/Time";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
                    return;
                }

                
                //dgHistory_Fill("pretrip", reloadFleetVehicles, sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);
                string sConnectionString = ConfigurationManager.ConnectionStrings["DWH"].ConnectionString;
                VLF.DAS.Logic.EvtEvents evtEvents = new VLF.DAS.Logic.EvtEvents(sConnectionString);
                DataSet dsPretrip = evtEvents.GetAllPretripResult(sn.User.OrganizationId, sn.UserID, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving));
                evtEvents.Dispose();

                dsPretrip.DataSetName = "ReeferPretripDataset";

                if (!dsPretrip.Tables[0].Columns.Contains("PretripResult"))
                {
                    DataColumn dc = new DataColumn("PretripResult", Type.GetType("System.String"));
                    dc.DefaultValue = "";
                    dsPretrip.Tables[0].Columns.Add(dc);                    
                }
                if (!dsPretrip.Tables[0].Columns.Contains("BatteryVolt"))
                {
                    DataColumn dc = new DataColumn("BatteryVolt", Type.GetType("System.String"));
                    dc.DefaultValue = "";
                    dsPretrip.Tables[0].Columns.Add(dc);
                }
                if (!dsPretrip.Tables[0].Columns.Contains("FuelLevel"))
                {
                    DataColumn dc = new DataColumn("FuelLevel", Type.GetType("System.String"));
                    dc.DefaultValue = "";
                    dsPretrip.Tables[0].Columns.Add(dc);
                }

                foreach (DataRow rowItem in dsPretrip.Tables[0].Rows)
                {
                    string[] ss = rowItem["AlarmDesc"].ToString().Split(new string[] { "--" }, StringSplitOptions.None);
                    if (ss.Length > 2)
                        rowItem["AlarmDesc"] = ss[2];
                    
                    if (rowItem["AlarmDesc"] == null || rowItem["AlarmDesc"].ToString().Trim() == "")
                        rowItem["PretripResult"] = "Pass";
                    else
                        rowItem["PretripResult"] = "Fail";

                    string v = FindValueFromPair("RD_FUEL", rowItem["Notes"].ToString(), ";", "=");
                    double dv = 0;
                    if (v != "")
                    {
                        dv = 300;
                        double.TryParse(v, out dv);
                        dv = dv * 1;
                        if (dv <= 100 && dv >= 0)
                            rowItem["FuelLevel"] = String.Format("{0:0.00}", dv);
                        else
                            rowItem["FuelLevel"] = "Invalid";
                    }

                    v = FindValueFromPair("RD_BATTERY", rowItem["Notes"].ToString(), ";", "=");
                    dv = -1;
                    double.TryParse(v, out dv);
                    rowItem["BatteryVolt"] = dv >= 0 ? String.Format("{0:0.00}", dv) : "-";

                }
                
                //table1.Rows.Add("FakeData1", "Remote", "2013-09-28 08:00:00", "2013-09-28 09:00:00", "Pass", 13.0, 92,"");
                //table1.Rows.Add("FakeData2", "Local", "2013-09-28 08:00:00", "2013-09-28 09:00:00", "Fail", 13.2, 23, "EVAPORATOR COIL SENSOR<br />RETURN AIR SENSOR<br />DISCHARGE AIR SENSOR");

                //dsReeferPretrip.Tables.Add(table1);
                sn.History.DsReeferPretrip = dsPretrip;

                DataSet dstemppretrip = new DataSet();
                //DataView dv = dsHistory.Tables["VehicleStatusHistory"].DefaultView;
                DataView dvprt = sn.History.DsReeferPretrip.Tables[0].DefaultView;

                DataTable sortedTablePrt = dvprt.ToTable();
                DataTable dtPrt = new DataTable();
                if (sortedTablePrt.Rows.Count > 0)
                    dtPrt = sortedTablePrt.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();

                dtPrt.TableName = "ReeferPretrip";
                dstemppretrip.Tables.Add(dtPrt);
                dstemppretrip.DataSetName = "ReeferPretripDataset";
                _r.data = dstemppretrip.GetXml().Replace("<ReeferPretripDataset>", "<ReeferPretripDataset><totalCount>" + sn.History.DsReeferPretrip.Tables[0].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                //_r.wholedata = sn.History.DsReeferPretrip.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                Response.Write(_r.data);
                

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

        private void GetHistoryRecords_For_Reefer_Impact_NewTZ()
        {
            try
            {
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.Default;

                string xml = "";
                result _r = new result();

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
                if (!string.IsNullOrEmpty(request) && request.Trim() == "1" && sn.History.DsReeferImpact != null)
                {
                    _r.success = true;
                    _r.msg = sn.History.DsReeferImpact.Tables[0].Rows.Count.ToString() + " records";
                    if (vlStart == 0 && vlLimit == 10000)
                    {
                        _r.data = sn.History.DsReeferImpact.GetXml().Replace("\r\n", "").Replace("&#x0;", "");
                        _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    }
                    else
                    {
                        DataSet dstemp = new DataSet();
                        DataView dv = sn.History.DsReeferImpact.Tables[0].DefaultView;
                        //dv.Sort = "OriginDateTime DESC";
                        DataTable sortedTable = dv.ToTable();
                        DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                        dt.TableName = "ReeferImpact";
                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "ReeferImpactDataset";
                        _r.data = dstemp.GetXml().Replace("<ReeferImpactDataset>", "<ReeferImpactDataset><totalCount>" + sn.History.DsReeferImpact.Tables[0].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "");
                        _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    }

                    //Response.Write(new JavaScriptSerializer().Serialize(_r));
                    Response.Write(_r.data);

                    return;

                }


                string strFromDate = "";
                string strToDate = "";

                strFromDate = Request["historyDateFrom"].ToString() + " " + Request["historyTimeFrom"].ToString();
                strToDate = Request["historyDateTo"].ToString() + " " + Request["historyTimeTo"].ToString();


                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

                strFromDate = Convert.ToDateTime(strFromDate, ci).ToString();
                strToDate = Convert.ToDateTime(strToDate, ci).ToString();

                if (Convert.ToDateTime(strFromDate) > Convert.ToDateTime(strToDate))
                {
                    _r.success = false;
                    _r.msg = "Invalid Date/Time";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
                    return;
                }


                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                VLF.DAS.Logic.EvtEvents evtEvents = new VLF.DAS.Logic.EvtEvents(sConnectionString);
                DataSet dsImpact = evtEvents.GetAllImpactResult(sn.User.OrganizationId, sn.UserID, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving));
                evtEvents.Dispose();

                dsImpact.DataSetName = "ReeferImpactDataset";

                if (!dsImpact.Tables[0].Columns.Contains("ImpactType"))
                {
                    DataColumn dc = new DataColumn("ImpactType", Type.GetType("System.String"));
                    dc.DefaultValue = "";
                    dsImpact.Tables[0].Columns.Add(dc);
                }
                if (!dsImpact.Tables[0].Columns.Contains("PeakG"))
                {
                    DataColumn dc = new DataColumn("PeakG", Type.GetType("System.Double"));
                    dc.DefaultValue = 0;
                    dsImpact.Tables[0].Columns.Add(dc);
                }
                if (!dsImpact.Tables[0].Columns.Contains("DeltaV"))
                {
                    DataColumn dc = new DataColumn("DeltaV", Type.GetType("System.Double"));
                    dc.DefaultValue = 0;
                    dsImpact.Tables[0].Columns.Add(dc);
                }

                foreach (DataRow rowItem in dsImpact.Tables[0].Rows)
                {
                    string ACC_DATA = FindValueFromPair("3Axis", rowItem["CustomProp"].ToString(), ";", "=");
                    double x_axis = 0;
                    double y_axis = 0;
                    double z_axis = 0;
                    double.TryParse(FindValueFromPair("X", ACC_DATA, ",", ":"), out x_axis);
                    double.TryParse(FindValueFromPair("Y", ACC_DATA, ",", ":"), out y_axis);
                    double.TryParse(FindValueFromPair("Z", ACC_DATA, ",", ":"), out z_axis);

                    rowItem["PeakG"] = new[] { x_axis, y_axis, z_axis }.Max();
                    rowItem["DeltaV"] = double.Parse(rowItem["PeakG"].ToString()) - 8d;
                    rowItem["ImpactType"] = x_axis >= y_axis ? "Lateral Impact" : "Longitudinal Impact";

                }

                sn.History.DsReeferImpact = dsImpact;

                DataSet dstempimpact = new DataSet();
                //DataView dv = dsHistory.Tables["VehicleStatusHistory"].DefaultView;
                DataView dvimpact = sn.History.DsReeferImpact.Tables[0].DefaultView;

                DataTable sortedTableImpact = dvimpact.ToTable();
                DataTable dtImpact = new DataTable();
                if (sortedTableImpact.Rows.Count > 0)
                    dtImpact = sortedTableImpact.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();

                dtImpact.TableName = "ReeferImpact";
                dstempimpact.Tables.Add(dtImpact);
                dstempimpact.DataSetName = "ReeferImpactDataset";
                _r.data = dstempimpact.GetXml().Replace("<ReeferImpactDataset>", "<ReeferImpactDataset><totalCount>" + sn.History.DsReeferImpact.Tables[0].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());

                Response.Write(_r.data);


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

        private void GetHistoryRecords_For_Reefer_Impact()
        {
            try
            {
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.Default;

                string xml = "";
                result _r = new result();

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
                if (!string.IsNullOrEmpty(request) && request.Trim() == "1" && sn.History.DsReeferImpact != null)
                {
                    _r.success = true;
                    _r.msg = sn.History.DsReeferImpact.Tables[0].Rows.Count.ToString() + " records";
                    if (vlStart == 0 && vlLimit == 10000)
                    {
                        _r.data = sn.History.DsReeferImpact.GetXml().Replace("\r\n", "").Replace("&#x0;", "");
                        _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    }
                    else
                    {
                        DataSet dstemp = new DataSet();
                        DataView dv = sn.History.DsReeferImpact.Tables[0].DefaultView;
                        //dv.Sort = "OriginDateTime DESC";
                        DataTable sortedTable = dv.ToTable();
                        DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                        dt.TableName = "ReeferImpact";
                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "ReeferImpactDataset";
                        _r.data = dstemp.GetXml().Replace("<ReeferImpactDataset>", "<ReeferImpactDataset><totalCount>" + sn.History.DsReeferImpact.Tables[0].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "");
                        _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    }

                    //Response.Write(new JavaScriptSerializer().Serialize(_r));
                    Response.Write(_r.data);

                    return;

                }


                string strFromDate = "";
                string strToDate = "";

                strFromDate = Request["historyDateFrom"].ToString() + " " + Request["historyTimeFrom"].ToString();
                strToDate = Request["historyDateTo"].ToString() + " " + Request["historyTimeTo"].ToString();


                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

                strFromDate = Convert.ToDateTime(strFromDate, ci).ToString();
                strToDate = Convert.ToDateTime(strToDate, ci).ToString();

                if (Convert.ToDateTime(strFromDate) > Convert.ToDateTime(strToDate))
                {
                    _r.success = false;
                    _r.msg = "Invalid Date/Time";
                    _r.data = "";
                    Response.Write(new JavaScriptSerializer().Serialize(_r));
                    return;
                }


                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                VLF.DAS.Logic.EvtEvents evtEvents = new VLF.DAS.Logic.EvtEvents(sConnectionString);
                DataSet dsImpact = evtEvents.GetAllImpactResult(sn.User.OrganizationId, sn.UserID, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving));
                evtEvents.Dispose();

                dsImpact.DataSetName = "ReeferImpactDataset";

                if (!dsImpact.Tables[0].Columns.Contains("ImpactType"))
                {
                    DataColumn dc = new DataColumn("ImpactType", Type.GetType("System.String"));
                    dc.DefaultValue = "";
                    dsImpact.Tables[0].Columns.Add(dc);
                }
                if (!dsImpact.Tables[0].Columns.Contains("PeakG"))
                {
                    DataColumn dc = new DataColumn("PeakG", Type.GetType("System.Double"));
                    dc.DefaultValue = 0;
                    dsImpact.Tables[0].Columns.Add(dc);
                }
                if (!dsImpact.Tables[0].Columns.Contains("DeltaV"))
                {
                    DataColumn dc = new DataColumn("DeltaV", Type.GetType("System.Double"));
                    dc.DefaultValue = 0;
                    dsImpact.Tables[0].Columns.Add(dc);
                }

                foreach (DataRow rowItem in dsImpact.Tables[0].Rows)
                {
                    string ACC_DATA = FindValueFromPair("3Axis", rowItem["CustomProp"].ToString(), ";", "=");
                    double x_axis = 0;
                    double y_axis = 0;
                    double z_axis = 0;
                    double.TryParse(FindValueFromPair("X", ACC_DATA, ",", ":"), out x_axis);
                    double.TryParse(FindValueFromPair("Y", ACC_DATA, ",", ":"), out y_axis);
                    double.TryParse(FindValueFromPair("Z", ACC_DATA, ",", ":"), out z_axis);

                    rowItem["PeakG"] = new[] { x_axis, y_axis, z_axis }.Max();
                    rowItem["DeltaV"] = double.Parse(rowItem["PeakG"].ToString()) - 8d;
                    rowItem["ImpactType"] = x_axis >= y_axis ? "Lateral Impact" : "Longitudinal Impact";

                }

                sn.History.DsReeferImpact = dsImpact;

                DataSet dstempimpact = new DataSet();
                //DataView dv = dsHistory.Tables["VehicleStatusHistory"].DefaultView;
                DataView dvimpact = sn.History.DsReeferImpact.Tables[0].DefaultView;

                DataTable sortedTableImpact = dvimpact.ToTable();
                DataTable dtImpact = new DataTable();
                if (sortedTableImpact.Rows.Count > 0)
                    dtImpact = sortedTableImpact.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();

                dtImpact.TableName = "ReeferImpact";
                dstempimpact.Tables.Add(dtImpact);
                dstempimpact.DataSetName = "ReeferImpactDataset";
                _r.data = dstempimpact.GetXml().Replace("<ReeferImpactDataset>", "<ReeferImpactDataset><totalCount>" + sn.History.DsReeferImpact.Tables[0].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                
                Response.Write(_r.data);


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

        private void dgHistory_Fill_NewTZ(string reeferScreenName, bool reloadFleetVehicles, Int64 VehicleId, string strFromDate, string strToDate)
        {
            try
            {
                DataSet dsHistory = new DataSet();
                StringReader strrXML = null;
                result _r = new result();

                string xml = "";
                ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();
                dbh.Timeout = -1;
                bool RequestOverflowed = false;
                string strPath = Server.MapPath("../Datasets/Hist.xsd");
                //strFromDate = "11/20/2013 12:15:00 PM";
                //strToDate = "11/20/2013 12:45:00 PM";

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
                                    _r.msg = "";
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
                        _r.success = true;
                        _r.msg = "";
                        _r.data = "";
                        Response.Write(new JavaScriptSerializer().Serialize(_r));
                        return;
                    }

                    dsHistory.ReadXmlSchema(strPath);
                    dsHistory.ReadXml(strrXML);

                    xml = "";
                    ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                    //Get Vehicle IconType
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId_NewTZ(sn.UserID, sn.SecId, sn.History.VehicleId, ref xml), false))
                        if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId_NewTZ(sn.UserID, sn.SecId, sn.History.VehicleId, ref xml), true))
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
                        if ((sn.History.DsHistoryVehicles == null || sn.History.DsHistoryVehicles.Tables == null) || reloadFleetVehicles)
                        {
                            SetHistoryVehiclesByFleetId((int)sn.History.FleetId);
                        }
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

                ProcessHistoryData(reeferScreenName, ref dsHistory);

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
                    _r.success = true;
                    _r.msg = dsHistory.Tables[0].Rows.Count.ToString() + " records";
                    if (vlStart == 0 && vlLimit == 10000)
                    {
                        _r.data = dsHistory.GetXml().Replace("\r\n", "").Replace("&#x0;", "");
                        _r.data = _r.data.Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    }
                    else
                    {
                        DataSet dstemp = new DataSet();
                        DataView dv = dsHistory.Tables["VehicleStatusHistory"].DefaultView;
                        //dv.Sort = "OriginDateTime DESC";
                        DataTable sortedTable = dv.ToTable();
                        DataTable dt = new DataTable();
                        if (sortedTable.Rows.Count > 0)
                            dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();

                        dt.TableName = "VehicleStatusHistory";
                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "MsgInHistory";
                        _r.data = dstemp.GetXml().Replace("<MsgInHistory>", "<MsgInHistory><totalCount>" + dsHistory.Tables["VehicleStatusHistory"].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                        _r.wholedata = dsHistory.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    }

                    if (reeferScreenName == "" || reeferScreenName == "command" || reeferScreenName == "reefer")
                    {
                        _r.iconTypeName = sn.History.IconTypeName;
                        var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
                        Response.Write(serializer.Serialize(_r));
                    }
                    else if (reeferScreenName == "pretrip")
                    {
                        DataSet dstemp = new DataSet();
                        //DataView dv = dsHistory.Tables["VehicleStatusHistory"].DefaultView;
                        DataView dv = sn.History.DsReeferPretrip.Tables[0].DefaultView;

                        DataTable sortedTable = dv.ToTable();
                        DataTable dt = new DataTable();
                        if (sortedTable.Rows.Count > 0)
                            dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();

                        dt.TableName = "ReeferPretrip";
                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "ReeferPretripDataset";
                        _r.data = dstemp.GetXml().Replace("<ReeferPretripDataset>", "<ReeferPretripDataset><totalCount>" + sn.History.DsReeferPretrip.Tables[0].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                        //_r.wholedata = sn.History.DsReeferPretrip.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                        Response.Write(_r.data);
                    }
                    else
                        Response.Write(_r.data);
                }
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

        private void dgHistory_Fill(string reeferScreenName, bool reloadFleetVehicles, Int64 VehicleId, string strFromDate, string strToDate)
        {
            try
            {
                DataSet dsHistory = new DataSet();
                StringReader strrXML = null;
                result _r = new result();

                string xml = "";
                ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();
                dbh.Timeout = -1;
                bool RequestOverflowed = false;
                string strPath = Server.MapPath("../Datasets/Hist.xsd");
                //strFromDate = "11/20/2013 12:15:00 PM";
                //strToDate = "11/20/2013 12:45:00 PM";

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
                                    _r.msg = "";
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
                        _r.success = true;
                        _r.msg = "";
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
                        if ((sn.History.DsHistoryVehicles == null || sn.History.DsHistoryVehicles.Tables == null) || reloadFleetVehicles)
                        {
                            SetHistoryVehiclesByFleetId((int)sn.History.FleetId);
                        }
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

                ProcessHistoryData(reeferScreenName, ref dsHistory);

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
                        DataTable dt = new DataTable();
                        if( sortedTable.Rows.Count > 0 )
                            dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();

                        dt.TableName = "VehicleStatusHistory";
                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "MsgInHistory";
                        _r.data = dstemp.GetXml().Replace("<MsgInHistory>", "<MsgInHistory><totalCount>" + dsHistory.Tables["VehicleStatusHistory"].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                        _r.wholedata = dsHistory.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    }
                    
                    if (reeferScreenName == "" || reeferScreenName == "command" || reeferScreenName == "reefer")
                    {
                        _r.iconTypeName = sn.History.IconTypeName;
                        var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
                        Response.Write(serializer.Serialize(_r));
                    }
                    else if (reeferScreenName == "pretrip")
                    {
                        DataSet dstemp = new DataSet();
                        //DataView dv = dsHistory.Tables["VehicleStatusHistory"].DefaultView;
                        DataView dv = sn.History.DsReeferPretrip.Tables[0].DefaultView;
                        
                        DataTable sortedTable = dv.ToTable();
                        DataTable dt = new DataTable();
                        if (sortedTable.Rows.Count > 0)
                            dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();

                        dt.TableName = "ReeferPretrip";
                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "ReeferPretripDataset";
                        _r.data = dstemp.GetXml().Replace("<ReeferPretripDataset>", "<ReeferPretripDataset><totalCount>" + sn.History.DsReeferPretrip.Tables[0].Rows.Count.ToString() + "</totalCount>").Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                        //_r.wholedata = sn.History.DsReeferPretrip.GetXml().Replace("\r\n", "").Replace("&#x0;", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                        Response.Write(_r.data);
                    }
                    else
                        Response.Write(_r.data);
                }
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

        private void ProcessHistoryData(string reeferScreenName, ref DataSet dsHistory)
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

            int i = 0;
            string strStreetAddress = "";

            UInt64 checkBit = 0;
            Int16 bitnum = 31;
            UInt64 shift = 1;
            UInt64 intSensorMask = 0;

            if (sn.History.IconTypeName == "")
                sn.History.IconTypeName = "Circle";

            int max = dsHistory.Tables[0].Rows.Count - 1;

            DataSet dsReeferPretrip = new DataSet("ReeferPretripDataset");
            
            if (reeferScreenName.ToLower() == "alarm")
            {
                if(!dsHistory.Tables[0].Columns.Contains("AlarmDetails"))
					dsHistory.Tables[0].Columns.Add("AlarmDetails", typeof(String));
                if(!dsHistory.Tables[0].Columns.Contains("Shutdown"))
					dsHistory.Tables[0].Columns.Add("Shutdown", typeof(String));
                if(!dsHistory.Tables[0].Columns.Contains("AlarmCount"))
					dsHistory.Tables[0].Columns.Add("AlarmCount", typeof(Int32));
            }
            else if (reeferScreenName.ToLower() == "reefer")
            {
                if(!dsHistory.Tables[0].Columns.Contains("Micro"))
					dsHistory.Tables[0].Columns.Add("Micro", typeof(String));
                if(!dsHistory.Tables[0].Columns.Contains("Tether"))
					dsHistory.Tables[0].Columns.Add("Tether", typeof(String));
                if(!dsHistory.Tables[0].Columns.Contains("Power"))
					dsHistory.Tables[0].Columns.Add("Power", typeof(string));
                if(!dsHistory.Tables[0].Columns.Contains("ReeferState"))
					dsHistory.Tables[0].Columns.Add("ReeferState", typeof(string));
                if(!dsHistory.Tables[0].Columns.Contains("ModeOfOp"))
					dsHistory.Tables[0].Columns.Add("ModeOfOp", typeof(string));
                if(!dsHistory.Tables[0].Columns.Contains("Door"))
					dsHistory.Tables[0].Columns.Add("Door", typeof(string));
                if(!dsHistory.Tables[0].Columns.Contains("AFAX"))
					dsHistory.Tables[0].Columns.Add("AFAX", typeof(string));
                if(!dsHistory.Tables[0].Columns.Contains("Setpt"))
					dsHistory.Tables[0].Columns.Add("Setpt", typeof(string));
                if(!dsHistory.Tables[0].Columns.Contains("Ret"))
					dsHistory.Tables[0].Columns.Add("Ret", typeof(string));
                if(!dsHistory.Tables[0].Columns.Contains("Dis"))
					dsHistory.Tables[0].Columns.Add("Dis", typeof(string));
                if(!dsHistory.Tables[0].Columns.Contains("Amb"))
					dsHistory.Tables[0].Columns.Add("Amb", typeof(string));
                if(!dsHistory.Tables[0].Columns.Contains("SensorProbe"))
					dsHistory.Tables[0].Columns.Add("SensorProbe", typeof(string));
                if (!dsHistory.Tables[0].Columns.Contains("Setpt2"))
                    dsHistory.Tables[0].Columns.Add("Setpt2", typeof(string));
                if (!dsHistory.Tables[0].Columns.Contains("Ret2"))
                    dsHistory.Tables[0].Columns.Add("Ret2", typeof(string));
                if (!dsHistory.Tables[0].Columns.Contains("Dis2"))
                    dsHistory.Tables[0].Columns.Add("Dis2", typeof(string));
                if (!dsHistory.Tables[0].Columns.Contains("SensorProbe2"))
                    dsHistory.Tables[0].Columns.Add("SensorProbe2", typeof(string));
                if (!dsHistory.Tables[0].Columns.Contains("Setpt3"))
                    dsHistory.Tables[0].Columns.Add("Setpt3", typeof(string));
                if (!dsHistory.Tables[0].Columns.Contains("Ret3"))
                    dsHistory.Tables[0].Columns.Add("Ret3", typeof(string));
                if (!dsHistory.Tables[0].Columns.Contains("Dis3"))
                    dsHistory.Tables[0].Columns.Add("Dis3", typeof(string));
                if (!dsHistory.Tables[0].Columns.Contains("SensorProbe3"))
                    dsHistory.Tables[0].Columns.Add("SensorProbe3", typeof(string));
                if(!dsHistory.Tables[0].Columns.Contains("SpareTemp"))
					dsHistory.Tables[0].Columns.Add("SpareTemp", typeof(string));
                if(!dsHistory.Tables[0].Columns.Contains("FuelLevel"))
					dsHistory.Tables[0].Columns.Add("FuelLevel", typeof(string));
				if(!dsHistory.Tables[0].Columns.Contains("EngineHours"))
					dsHistory.Tables[0].Columns.Add("EngineHours", typeof(string));
				if(!dsHistory.Tables[0].Columns.Contains("ControllerType"))
					dsHistory.Tables[0].Columns.Add("ControllerType", typeof(string));
                if(!dsHistory.Tables[0].Columns.Contains("RPM"))
					dsHistory.Tables[0].Columns.Add("RPM", typeof(string));
				if(!dsHistory.Tables[0].Columns.Contains("BatteryVolt"))
					dsHistory.Tables[0].Columns.Add("BatteryVolt", typeof(string));
                if (!dsHistory.Tables[0].Columns.Contains("VehicleTypeName"))
                    dsHistory.Tables[0].Columns.Add("VehicleTypeName", typeof(string));
            }
            else if (reeferScreenName.ToLower() == "pretrip")
            {
                DataTable table1 = new DataTable("ReeferPretrip");
                table1.Columns.Add("ReeferNum", typeof(String));
                table1.Columns.Add("LocalRemote", typeof(String));
                table1.Columns.Add("InitiatedTime", typeof(DateTime));
                table1.Columns.Add("CompletedTime", typeof(DateTime));
                table1.Columns.Add("PretripResult", typeof(String));
                table1.Columns.Add("BatteryVolt", typeof(String));
                table1.Columns.Add("FuelLevel", typeof(String));
                table1.Columns.Add("AlarmDesc", typeof(String));


                //table1.Rows.Add("FakeData1", "Remote", "2013-09-28 08:00:00", "2013-09-28 09:00:00", "Pass", 13.0, 92,"");
                //table1.Rows.Add("FakeData2", "Local", "2013-09-28 08:00:00", "2013-09-28 09:00:00", "Fail", 13.2, 23, "EVAPORATOR COIL SENSOR<br />RETURN AIR SENSOR<br />DISCHARGE AIR SENSOR");

                dsReeferPretrip.Tables.Add(table1);

                sn.History.DsReeferPretrip = dsReeferPretrip;
            }
            else
            {
                if (!dsHistory.Tables[0].Columns.Contains("Temperature"))
                    dsHistory.Tables[0].Columns.Add("Temperature", typeof(string));
            }

            string vehicleDescription = string.Empty;

            Pretrip PRETRIP = new Pretrip("");
            int checktimes = 0;

            Dictionary<int, string> dicBoxVehicleTypeName = new Dictionary<int, string>();

            //foreach (DataRow rowItem in dsHistory.Tables[0].Rows)
            for (int irow = max; irow >= 0; --irow)
            {
                DataRow rowItem = dsHistory.Tables[0].Rows[irow];
                
                //Key 
                rowItem["dgKey"] = i.ToString();
                //CustomUrl 
                rowItem["CustomUrl"] = "javascript:var w =HistoryInfo('" + i.ToString() + "')";
                rowItem["HistoryInfoId"] = i;
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

                        rowItem["icon"] = "Green" + sn.History.IconTypeName + rowItem["MyHeading"] + ".ico";
                    }
                }
                if (rowItem["icon"] == "")
                {
                    rowItem["icon"] = "red" + sn.History.IconTypeName + ".ico";
                }
                if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.ExtremeAcceleration)) ||
                    (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.ExtremeBraking)) ||
                    (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.HarshAcceleration)) ||
                    (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.HarshBraking)) ||
                    (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.SeatBelt)))
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

                if (sn.User.OrganizationId == 999650 && (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == 1 || Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == 2))
                {
                    rowItem["MsgDetails"] += "Temp:" + VLF.CLS.Util.PairFindValue("TEMP", rowItem["CustomProp"].ToString());
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

                if (reeferScreenName.Trim() == "command")
                {
                    if (!rowItem["BoxMsgInTypeName"].ToString().ToLower().Trim().Contains("command"))
                    {
                        rowItem.Delete();
                    }
                }
                #region reefer
                else if (reeferScreenName.Trim() == "reefer")
                {
                    string VehicleTypeName = string.Empty;

                    if (dicBoxVehicleTypeName.ContainsKey(int.Parse(rowItem["BoxId"].ToString())))
                    {
                        VehicleTypeName = dicBoxVehicleTypeName[int.Parse(rowItem["BoxId"].ToString())];
                    }
                    else
                    {
                        DataRow[] _rs = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Select("BoxId=" + rowItem["BoxId"].ToString());
                        if (_rs.Length > 0)
                        {
                            VehicleTypeName = _rs[0]["VehicleTypeName"].ToString();
                            dicBoxVehicleTypeName.Add(int.Parse(rowItem["BoxId"].ToString()), VehicleTypeName);
                        }
                    }
                    
                    string ps = string.Empty;
                    
                    //if (!rowItem["CustomProp"].ToString().Contains("RD_"))
                    //    rowItem.Delete();
                    //else
                    if ((rowItem["CustomProp"].ToString().Contains("RD_ZONE1") || VehicleTypeName.Trim().ToLower() == "dry car") && (new List<int>(new int[] { 1, 9, 114, 2, 73 })).Contains(Convert.ToInt16(rowItem["BoxMsgInTypeId"])))
                    {
                        string tether = string.Empty;
                        string powerSensorMask = string.Empty;
                        int psensormask = 0;
                        int.TryParse(rowItem["SensorMask"].ToString(), out psensormask);

                        if ((psensormask & 4) >> 2 == 1)
                            powerSensorMask = "On";
                        else
                            powerSensorMask = "Off";


                        if (VehicleTypeName.Trim().ToLower() == "dry car") //Dry Car
                            rowItem["Micro"] = "DC";
                        else
                        {
                        ps = FindValueFromPair("RD_DT", rowItem["CustomProp"].ToString(), ";", "=");
                        if (ps == "1")
                            rowItem["Micro"] = "TK";
                        else if (ps == "2")
                            rowItem["Micro"] = "CT";
                        else
                            rowItem["Micro"] = "Unknown";
                        }

                        string _RD_STA = FindValueFromPair("RD_STA", rowItem["CustomProp"].ToString(), ";", "=");
                        int RD_STA = 0;
                        int.TryParse(_RD_STA, out RD_STA);

                        string RD_ZONE1 = string.Empty;
                        RD_ZONE1 = FindValueFromPair("RD_ZONE1", rowItem["CustomProp"].ToString(), ";", "=");
                        string RD_ZONE2 = string.Empty;
                        RD_ZONE2 = FindValueFromPair("RD_ZONE2", rowItem["CustomProp"].ToString(), ";", "=");
                        string RD_ZONE3 = string.Empty;
                        RD_ZONE3 = FindValueFromPair("RD_ZONE3", rowItem["CustomProp"].ToString(), ";", "=");
                        string RD_ZONE1_OM = FindValueFromPair("OM", RD_ZONE1, ",", ":");
                        int intRD_ZONE1_OM = 0;
                        int.TryParse(RD_ZONE1_OM, out intRD_ZONE1_OM);

                        string ControllerType = string.Empty;

                        ps = FindValueFromPair("Power", rowItem["CustomProp"].ToString(), ";", "=");
                        if (ps.ToLower() == "sleepmode") {
                            tether = "Off";
                        }
                        else
                        {
                            tether = "Off";
                            if (((RD_STA & 16) >> 4) == 1)
                                tether = "On";
                            
                        }
                        
                        string v = string.Empty;
                        double dv = 0;

                        string power = "-";
                        string reeferState = "-";
                        string ModeOfOp = "-";
                        string Door = "-";
                        string AFAX = "-";
                        string Setpt = "-";
                        string Ret = "-";
                        string Dis = "-";
                        string Amb = "-";
                        string SensorProbe = "-";
                        string Setpt2 = "-";
                        string Ret2 = "-";
                        string Dis2 = "-";
                        string SensorProbe2 = "-";
                        string Setpt3 = "-";
                        string Ret3 = "-";
                        string Dis3 = "-";
                        string SensorProbe3 = "-";
                        string SpareTemp = "-";
                        string FuelLevel = "-";
                        string EngineHours = "-";
                        string ControllerTypeName = "-";
                        string RPM = "-";
                        string BatteryVolt = "-";

                        v = FindValueFromPair("RD_STA", rowItem["CustomProp"].ToString(), ";", "="); //getValueByKey('RD_STA', value);
                        if (v.Trim() != string.Empty)
                        {
                            if ((int.Parse(v) & 15) == 1)
                            {    // Device Type = 1 (TK)              
                                ControllerType = FindValueFromPair("RD_CT", rowItem["CustomProp"].ToString(), ";", "=");
                            }
                        }

                        if (tether == "Off")
                        {
                            //power = "-";
                        }
                        else
                        {
                            
                            v = FindValueFromPair("RD_STA", rowItem["CustomProp"].ToString(), ";", "="); //getValueByKey('RD_STA', value);
                            
                            if ((RD_STA & 15) == 1)
                            {    // Device Type = 1 (TK)              

                                # region Power, RPM 
                                if (",9,11,12,13,14,15,16,17,19,20,".IndexOf("," + ControllerType + ",") >= 0)
                                {
                                    if ((RD_STA & 64) > 0)
                                        power = "On";
                                    else
                                        power = "Off";

                                    RPM = FindValueFromPair("RD_RPM", rowItem["CustomProp"].ToString(), ";", "="); 
									if (RPM.Trim() == "32767") RPM = "";
                                }
                                # endregion
                            }

                            if (powerSensorMask != "Off")
                            {


                                if (RD_ZONE1 != "")
                                {

                                    if (RD_ZONE1_OM != "")
                                    {

                                        # region ReeferState
                                        reeferState = getReeferState(intRD_ZONE1_OM);
                                        # endregion

                                        # region ModeOfOp
                                        if (((intRD_ZONE1_OM & 16) >> 4) == 1)
                                            ModeOfOp = "Continuous";
                                        else
                                            ModeOfOp = "Cycle Sentry";
                                        # endregion

                                        # region Door
                                        if (((intRD_ZONE1_OM & 4) >> 2) == 1)
                                            Door = "Open";
                                        else
                                            Door = "Closed";
                                        # endregion

                                        # region AFAX
                                        if (",14,15,16,17,19,20,21,22,".IndexOf("," + ControllerType + ",") >= 0)
                                        {

                                            if ((intRD_ZONE1_OM & 1) == 1)
                                                AFAX = "Open";
                                            else
                                                AFAX = "Closed";

                                        }
                                        # endregion
                                    }

                                    # region Setpt.
                                    v = FindValueFromPair("TSP", RD_ZONE1, ",", ":");
                                    if (v.Trim() != "")
                                        Setpt = String.Format("{0:0}", double.Parse(v));
                                    # endregion

                                    # region Ret
                                    v = FindValueFromPair("RAT1", RD_ZONE1, ",", ":");
                                    if (v.Trim() != "")
                                        Ret = String.Format("{0:0.00}", double.Parse(v));
                                    # endregion

                                    # region Dis
                                    v = FindValueFromPair("SDT1", RD_ZONE1, ",", ":");
                                    if (v.Trim() != "")
                                        Dis = String.Format("{0:0.00}", double.Parse(v));
                                    # endregion

                                    # region Sensor Probe
                                    v = FindValueFromPair("ECT", RD_ZONE1, ",", ":");
                                    if (v.Trim() != "")
                                        SensorProbe = String.Format("{0:0.00}", double.Parse(v));
                                    # endregion
                                }

                                if (RD_ZONE2 != "")
                                {

                                    # region Setpt2.
                                    v = FindValueFromPair("TSP", RD_ZONE2, ",", ":");
                                    if (v.Trim() != "")
                                        Setpt2 = String.Format("{0:0}", double.Parse(v));
                                    # endregion

                                    # region Ret2
                                    v = FindValueFromPair("RAT1", RD_ZONE2, ",", ":");
                                    if (v.Trim() != "")
                                        Ret2 = String.Format("{0:0.00}", double.Parse(v));
                                    # endregion

                                    # region Dis2
                                    v = FindValueFromPair("SDT1", RD_ZONE2, ",", ":");
                                    if (v.Trim() != "")
                                        Dis2 = String.Format("{0:0.00}", double.Parse(v));
                                    # endregion

                                    # region Sensor Probe2
                                    v = FindValueFromPair("ECT", RD_ZONE2, ",", ":");
                                    if (v.Trim() != "")
                                        SensorProbe2 = String.Format("{0:0.00}", double.Parse(v));
                                    # endregion
                                }

                                if (RD_ZONE3 != "")
                                {

                                    # region Setpt3.
                                    v = FindValueFromPair("TSP", RD_ZONE3, ",", ":");
                                    if (v.Trim() != "")
                                        Setpt3 = String.Format("{0:0}", double.Parse(v));
                                    # endregion

                                    # region Ret3
                                    v = FindValueFromPair("RAT1", RD_ZONE3, ",", ":");
                                    if (v.Trim() != "")
                                        Ret3 = String.Format("{0:0.00}", double.Parse(v));
                                    # endregion

                                    # region Dis3
                                    v = FindValueFromPair("SDT1", RD_ZONE3, ",", ":");
                                    if (v.Trim() != "")
                                        Dis3 = String.Format("{0:0.00}", double.Parse(v));
                                    # endregion

                                    # region Sensor Probe3
                                    v = FindValueFromPair("ECT", RD_ZONE3, ",", ":");
                                    if (v.Trim() != "")
                                        SensorProbe3 = String.Format("{0:0.00}", double.Parse(v));
                                    # endregion
                                }

                                # region Amb
                                string _amt = FindValueFromPair("RD_AMT", rowItem["CustomProp"].ToString(), ";", "=");
                                if (_amt.Trim() != "")
                                {
                                    Amb = String.Format("{0:0}", double.Parse(_amt));
                                }
                                # endregion
                                
                                # region Spare Temperature of Sensor #1
                                v = FindValueFromPair("RD_STS1", rowItem["CustomProp"].ToString(), ";", "=");
                                if (v != "")
                                {
                                    dv = 0;
                                    double.TryParse(v, out dv);
                                    SpareTemp = String.Format("{0:0.00}", dv);
                                }
                                # endregion                                                                
                                
                            }
                            else
                            {
                                RPM = "-";
                            }
                        }

                        v = FindValueFromPair("RD_STA", rowItem["CustomProp"].ToString(), ";", "="); //getValueByKey('RD_STA', value);

                        # region Controller Type
                        if (ControllerType != string.Empty)
                        {
                            ControllerTypeName = getControllerTypeById(int.Parse(ControllerType));
                        }
                        # endregion

                        # region Fuel Level
                        v = FindValueFromPair("RD_FUEL", rowItem["CustomProp"].ToString(), ";", "=");
                        if (v != "")
                        {
                            dv = 300;
                            double.TryParse(v, out dv);
                            dv = dv * 1;
                            if (dv <= 100)
                                FuelLevel = String.Format("{0:0}", dv);
                            else
                                FuelLevel = "Invalid";
                        }
                        # endregion

                        # region Engine Hours
                        v = FindValueFromPair("RD_EH", rowItem["CustomProp"].ToString(), ";", "=");
                        if (v != "")
                        {
                            dv = 0;
                            double.TryParse(v, out dv);
                            EngineHours = String.Format("{0:0}", dv);
                        }
                        # endregion

                        # region Battery Voltage
                        v = FindValueFromPair("RD_BATTERY", rowItem["CustomProp"].ToString(), ";", "=");
                        if (v.Trim() != string.Empty)
                        {
                            dv = double.Parse(v);
                            BatteryVolt = String.Format("{0:0.00}", dv);
                        }
                        # endregion

                        //if (rowItem["Description"].ToString().Contains("00600"))
                        if (VehicleTypeName.Trim() == "Dry Car") //Dry Car
                        {

                            int analog1 = 0;
                            int analog2 = 0;
                            int.TryParse(FindValueFromPair("Analog1", rowItem["CustomProp"].ToString(), ";", "="), out analog1);
                            int.TryParse(FindValueFromPair("Analog2", rowItem["CustomProp"].ToString(), ";", "="), out analog2);

                            if (analog1 == 0 && analog2 == 0)
                                Amb = "";
                            else
                            {
                                int ADC = (analog2 * 256) + analog1;
                                double _temperature = (double)((ADC - 1120) / 5);
                                _temperature = _temperature * 9.0 / 5 + 32;

                                //Amb = _temperature.ToString();
                                Amb = String.Format("{0:0}", _temperature);
                            }

                            Door = powerSensorMask == "On" ? "Open" : "Closed";

                            if (!rowItem["CustomProp"].ToString().Contains("RD_"))
                            {
                                Door = "-";
                                powerSensorMask = "-";
                                tether = "-";
                            }
                        }

                        rowItem["Tether"] = tether;
                        //rowItem["Power"] = power;
                        rowItem["Power"] = "<span style='color:" + (powerSensorMask == power ? "#000000":"#666666") + ";'>" + powerSensorMask + "</span>";
                        rowItem["ReeferState"] = reeferState;
                        rowItem["ModeOfOp"] = ModeOfOp;                        
                        rowItem["Door"] = Door;
                        rowItem["AFAX"] = AFAX;
                        rowItem["Setpt"] = Setpt;
                        rowItem["Ret"] = Ret;
                        rowItem["Dis"] = Dis;
                        rowItem["Amb"] = Amb;
                        rowItem["SensorProbe"] = SensorProbe;
                        rowItem["Setpt2"] = Setpt2;
                        rowItem["Ret2"] = Ret2;
                        rowItem["Dis2"] = Dis2;
                        rowItem["SensorProbe2"] = SensorProbe2;
                        rowItem["Setpt3"] = Setpt3;
                        rowItem["Ret3"] = Ret3;
                        rowItem["Dis3"] = Dis3;
                        rowItem["SensorProbe3"] = SensorProbe3;
                        rowItem["SpareTemp"] = SpareTemp;
                        rowItem["FuelLevel"] = FuelLevel;
                        rowItem["EngineHours"] = EngineHours;
                        rowItem["ControllerType"] = ControllerTypeName;
                        rowItem["RPM"] = RPM;
                        rowItem["BatteryVolt"] = BatteryVolt;

                    }
                    else
                        rowItem.Delete();


                }
                #endregion
                #region alarm
                else if (reeferScreenName.ToLower() == "alarm")
                {
                    if (!rowItem["CustomProp"].ToString().Contains("RD_"))
                        rowItem.Delete();
                    else
                    {
                        string zone1 = FindValueFromPair("RD_ZONE1", rowItem["CustomProp"].ToString(), ";", "=");
                        string alarmType = FindValueFromPair("AT", zone1, ",",":");
                        string alarmCode = "";
                        if (alarmType.Trim() == "" || alarmType.Trim() == "0")
                            rowItem.Delete();
                        else
                        {
                            bool hasAlarm = false;
                            try
                            {
                                int RD_RLEN = int.Parse(FindValueFromPair("RD_RLEN", rowItem["CustomProp"].ToString(), ";", "="));
                                if (RD_RLEN > 4)
                                {
                                    //string[] RD_RBUF = (FindValueFromPair("RD_RBUF", rowItem["CustomProp"].ToString(), ";", "=")).Split(
                                }
                            }
                            catch { }
                            alarmCode = FindValueFromPair("AC", zone1, ",", ":");
                            int ac = -1;
                            int.TryParse(alarmCode, out ac);
                            Alarm a = new Alarm(ac);
                            if (a.AlarmLevel != (Int32)ReeferAlarmLevel.ALARM_INVALID)
                            {
                                rowItem["AlarmDetails"] = a.AlarmName;
                                if (a.AlarmLevel == (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN || a.AlarmLevel == (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN || a.AlarmLevel == (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY)
                                {
                                    rowItem["Shutdown"] = "Yes";
                                }
                                else
                                {
                                    rowItem["Shutdown"] = "No";
                                }
                            }
                            else
                                rowItem.Delete();
                        }
                    }
                }
                #endregion
                #region pretrip
                else if (reeferScreenName.ToLower() == "pretrip")
                {
                    if (rowItem["CustomProp"].ToString().Contains("RD_"))
                    {
                        if (rowItem["Description"].ToString() != PRETRIP.ReeferNum)
                        {
                            savePretripResult(PRETRIP, ref dsReeferPretrip);
                            PRETRIP = new Pretrip(rowItem["Description"].ToString());
                        }


                        string RD_ZONE1 = string.Empty;
                        RD_ZONE1 = FindValueFromPair("RD_ZONE1", rowItem["CustomProp"].ToString(), ";", "=");
                        string RD_ZONE1_OM = FindValueFromPair("OM", RD_ZONE1, ",", ":");
                        int intRD_ZONE1_OM = 0;
                        int.TryParse(RD_ZONE1_OM, out intRD_ZONE1_OM);

                        string reeferState = string.Empty;

                        if (RD_ZONE1 != "")
                        {
                            if (RD_ZONE1_OM != "")
                            {
                                reeferState = getReeferState(intRD_ZONE1_OM);
                            }
                        }

                        string v = FindValueFromPair("RD_FUEL", rowItem["CustomProp"].ToString(), ";", "=");
                        double dv = 0;
                        if (v != "")
                        {
                            dv = 300;
                            double.TryParse(v, out dv);
                            dv = dv * 1;
                            if (dv <= 100)
                                PRETRIP.FuelLevel = String.Format("{0:0.00}", dv);
                            else
                                PRETRIP.FuelLevel = "Invalid";
                        }

                        v = FindValueFromPair("RD_BATTERY", rowItem["CustomProp"].ToString(), ";", "=");
                        dv = -1;
                        double.TryParse(v, out dv);
                        PRETRIP.BatteryVolt = dv >= 0 ? String.Format("{0:0.00}", dv) : "-";

                        if (reeferState != PRETRIP.ReeferState)
                        {
                            if (reeferState == "Pre-trip" && PRETRIP.ReeferState != "") // ReeferState changed to Pre-trip
                            {
                                PRETRIP.InitiatedTime = Convert.ToDateTime(rowItem["OriginDateTime"]);// rowItem["OriginDateTime"];
                                PRETRIP.ReeferState = "Pre-trip";
                                PRETRIP.HasPretrip = true;
                            }

                            if (reeferState != "Pre-trip" && PRETRIP.ReeferState == "Pre-trip") // ReeferState changed from Pre-trip to others
                            {
                                PRETRIP.CompletedTime = Convert.ToDateTime(rowItem["OriginDateTime"]); //rowItem["OriginDateTime"].ToString();
                                PRETRIP.ReeferState = reeferState;
                                PRETRIP.Alarms = checkReeferAlarms(rowItem["CustomProp"].ToString());
                                checktimes = 0;
                                PRETRIP.HasPretrip = true;
                            }
                        }
                        else if (reeferState == PRETRIP.ReeferState && reeferState != "Pre-trip")
                        {
                            if (PRETRIP.CompletedTime != null)
                            {
                                if (PRETRIP.Alarms.IndexOf(";") < 0 && checktimes < 3) // didn't find multi-alarms and less than 3 records.
                                {
                                    string _alarms = checkReeferAlarms(rowItem["CustomProp"].ToString());
                                    if (_alarms != string.Empty)
                                        PRETRIP.Alarms = _alarms;
                                }

                                if (checktimes >= 5)
                                {
                                    savePretripResult(PRETRIP, ref dsReeferPretrip);
                                    PRETRIP = new Pretrip(rowItem["Description"].ToString());
                                }

                                checktimes++;
                            }
                        }

                        PRETRIP.ReeferState = reeferState;

                    }
                }
                #endregion
                else
                {
                    int analog1 = 0;
                    int analog2 = 0;
                    int.TryParse(FindValueFromPair("Analog1", rowItem["CustomProp"].ToString(), ";", "="), out analog1);                    
                    int.TryParse(FindValueFromPair("Analog2", rowItem["CustomProp"].ToString(), ";", "="), out analog2);

                    if (analog1 == 0 && analog2 == 0)
                        rowItem["Temperature"] = "";
                    else
                    {
                        int ADC = (analog2 * 256) + analog1;
                        //ADC = int.Parse((Math.Floor((double)(ADC - 1120) / 5)).ToString());
                        //double temperature = ADC * 9.0 / 5 + 32;

                        //rowItem["Temperature"] = temperature.ToString();

                        rowItem["Temperature"] = (Math.Floor((double)((ADC - 1120) / 5))).ToString();
                    }
                }
            }

            if (reeferScreenName.ToLower() == "pretrip" && (PRETRIP.InitiatedTime != null || PRETRIP.CompletedTime != null))
            {
                savePretripResult(PRETRIP, ref dsReeferPretrip);
                PRETRIP = new Pretrip("");
            }

            if (reeferScreenName.ToLower() == "pretrip")
            {
                sn.History.DsReeferPretrip = dsReeferPretrip;
            }
        }

        // Changes for TimeZone Feature start

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
                _r.data = tripsummary.GetXml().Replace("\r\n", "").Replace("&#x0;", "");
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
                _r.data = tripsummary.GetXml().Replace("\r\n", "").Replace("&#x0;", "");
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
                ProcessHistoryData("", ref dsHistory);

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
                ProcessHistoryData("", ref dsHistory);

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
                int key_pos = src.IndexOf(key);
                
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

        private string getControllerTypeById(int controllerTypeId) {
            string c = "N/A";
    
            switch (controllerTypeId) {
                case 0:
                    c = "Invalid";
                    break;
                case 1:
                    c = "MP4";
                    break;
                case 2:
                    c = "MP5";
                    break;
                case 3:
                    c = "MP6";
                    break;
                case 4:
                    c = "TG6";
                    break;
                case 5:
                    c = "TTMT";
                    break;
                case 6:
                    c = "DAS";
                    break;
                case 7:
                    c = "TCI";
                    break;
                case 8:
                    c = "MPT";
                    break;
                case 9:
                    c = "SR2";
                    break;
                case 10:
                    c = "N/A";
                    break;
                case 11:
                    c = "SR2 M/T";
                    break;
                case 12:
                    c = "SR2 Truck";
                    break;
                case 13:
                    c = "SR2 Truck M/T";
                    break;
                case 14:
                    c = "SR3";
                    break;
                case 15:
                    c = "SR3 MT";
                    break;
                case 16:
                    c = "SR3 ST Truck";
                    break;
                case 17:
                    c = "SR3 MT Truck";
                    break;
                case 18:
                    c = "DAS IV";
                    break;
                case 19:
                    c = "SR4 ST";
                    break;
                case 20:
                    c = "SR4 MT";
                    break;
                case 21:
                    c = "SR4 ST Truck";
                    break;
                case 22:
                    c = "SR4 MT Truck";
                    break;
                case 23:
                    c = "Cryo Trailer";
                    break;
                case 24:
                    c = "Cryo Truck";
                    break;
                default:
                    c = "N/A";
                    break;

            }
            return c;
        }

        private string getReeferState(int intRD_ZONE1_OM)
        {
            int r = intRD_ZONE1_OM >> 5;
            string reeferState = string.Empty;

            switch (r)
            {
                case 0:
                    reeferState = "Unknown";
                    break;
                case 1:
                    reeferState = "Cooling";
                    break;
                case 2:
                    reeferState = "Heating";
                    break;

                case 3:
                    reeferState = "Defrost";
                    break;
                case 4:
                    reeferState = "Null";
                    break;
                case 5:
                    reeferState = "Pre-trip";
                    break;
                case 6:
                    reeferState = "Sleep";
                    break;
                case 7:
                    reeferState = "N/A";
                    break;
            }

            if (reeferState == "Cooling" || reeferState == "Heating")
            {
                int engineSpeed = intRD_ZONE1_OM & 8; //bit 4, Engine Speed. Only if cooling or heating. 0: low, 1: high
                if (engineSpeed == 0)
                    reeferState = "Low Speed " + reeferState;
                else
                    reeferState = "High Speed " + reeferState;
            }

            return reeferState;
        }

        private string checkReeferAlarms(string customProp)
        {
            if (!customProp.Contains("RD_"))
                return string.Empty;

            string zone1 = FindValueFromPair("RD_ZONE1", customProp, ";", "=");
            string alarmType = FindValueFromPair("AT", zone1, ",", ":");
            string alarmCode = FindValueFromPair("AC", zone1, ",", ":");
            string alarms = string.Empty;

            if (alarmType.Trim() == "" || alarmType.Trim() == "0")
                return string.Empty;

            int ac = -1;
            int.TryParse(alarmCode, out ac);
            Alarm a = new Alarm(ac);
            if (a.AlarmLevel != (Int32)ReeferAlarmLevel.ALARM_INVALID)
            {
                alarms = ac.ToString() + " - "+a.AlarmName;

                try
                {
                    int RD_RLEN = int.Parse(FindValueFromPair("RD_RLEN", customProp, ";", "="));
                    if (RD_RLEN > 4)
                    {
                        string[] RD_RBUF = (FindValueFromPair("RD_RBUF", customProp, ";", "=")).Split(',');
                        if (RD_RBUF.Length > 4 && RD_RBUF[1].Trim() == "207")
                        {
                            bool hasAlarm = false;
                            for (int i = 0; i < (RD_RBUF.Length - 4) / 2; i++)
                            {
                                ac = -1;
                                alarmCode = RD_RBUF[3 + (i*2)];
                                int.TryParse(alarmCode, out ac);
                                a = new Alarm(ac);
                                if (a.AlarmLevel != (Int32)ReeferAlarmLevel.ALARM_INVALID)
                                {
                                    if (!hasAlarm)
                                    {
                                        alarms = string.Empty;
                                        hasAlarm = true;
                                    }
                                    alarms += a.AlarmName + "<br />";
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return alarms;
            
        }

        private void savePretripResult(Pretrip p, ref DataSet dsReeferPretrip)
        {
            //if (p.HasPretrip && p.ReeferNum != "")
            if((p.InitiatedTime != null || p.CompletedTime != null) && p.ReeferNum != "")
            {
                dsReeferPretrip.Tables[0].Rows.Add(p.ReeferNum, p.LocalRemote, p.InitiatedTime, p.CompletedTime, p.Alarms == "" ? "Pass" : "Fail", p.BatteryVolt, p.FuelLevel, p.Alarms);
            }
        }

        private class Pretrip
        {
            public string ReeferNum;
            public string LocalRemote;
            public DateTime? InitiatedTime;
            public DateTime? CompletedTime;
            public string PretripResult;
            public string BatteryVolt;
            public string FuelLevel;
            public string Alarms;
            public string ReeferState;
            public bool HasPretrip;
            
            public Pretrip(string reeferNum)
            {
                this.ReeferNum = reeferNum;
                this.LocalRemote = "";
                this.InitiatedTime = null;
                this.CompletedTime = null;
                this.PretripResult = "";
                this.BatteryVolt = "";
                this.FuelLevel = "";
                this.Alarms = "";
                this.ReeferState = "";
                this.HasPretrip = false;
            }
        }

        #region class Alarm
        private class Alarm
        {
            public int AlarmLevel;
            public int AlarmCode;
            public string AlarmName;

            public Alarm(int alarmCode)
            {
                this.AlarmCode = alarmCode;
                switch (alarmCode)
                {
                    case 1:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "MICROPROCESSOR POWER UP RESET";
                        break;
                    case 2:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "EVAPORATOR COIL SENSOR";
                        break;
                    case 3:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "RETURN AIR SENSOR";
                        break;
                    case 4:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "DISCHARGE AIR SENSOR";
                        break;
                    case 5:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "AMBIENT AIR SENSOR";
                        break;
                    case 6:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "WATER TEMPERATURE SENSOR";
                        break;
                    case 7:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "RPM SENSOR";
                        break;
                    case 8:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "UNIT RUNNING ON COIL SENSOR";
                        break;
                    case 9:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "HIGH EVAPORATOR TEMPERATURE";
                        break;
                    case 10:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_PREVENT;
                        this.AlarmName = "HIGH DISCHARGE PRESSURE";
                        break;
                    case 11:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "UNIT CONTROLLING ON ALTERNATE SENSOR";
                        break;
                    case 12:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "SENSOR SHUTDOWN";
                        break;
                    case 13:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "SENSOR CALIBRATION CHECK";
                        break;
                    case 14:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "DEFROST TERMINATE BY TIME";
                        break;
                    case 15:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "GLOW PLUG CHECK";
                        break;
                    case 16:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "MANUAL START NOT COMPLETED";
                        break;
                    case 17:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "ENGINE FAILED TO CRANK";
                        break;
                    case 18:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "HIGH ENGINE COOLANT TEMPERATURE";
                        break;
                    case 19:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "LOW ENGINE OIL PRESSURE";
                        break;
                    case 20:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "ENGINE FAILED TO START";
                        break;
                    case 21:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "COOLING CYCLE CHECK";
                        break;
                    case 22:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "HEATING CYCLE CHECK";
                        break;
                    case 23:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "COOLING CYCLE FAULT";
                        break;
                    case 24:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "HEATING CYCLE FAULT";
                        break;
                    case 25:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "ALTERNATOR CHECK";
                        break;
                    case 26:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "REFRIGERATION CAPACITY CHECK";
                        break;
                    case 27:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "VAPOR MOTOR RPM HIGH (CR)";
                        break;
                    case 28:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "PRETRIP ABORT";
                        break;
                    case 29:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "DAMPER CIRCUIT";
                        break;
                    case 30:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "DAMPER CLOSE CHECK";
                        break;
                    case 31:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "OIL PRESSURE SWITCH";
                        break;
                    case 32:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "REFRIGERATION CAPACITY LOW";
                        break;
                    case 33:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK ENGINE RPM";
                        break;
                    case 34:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK MODULATION CIRCUIT";
                        break;
                    case 35:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "RUN RELAY CIRCUIT";
                        break;
                    case 36:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "ELECTRIC MOTOR FAILED TO RUN";
                        break;
                    case 37:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "CHECK ENGINE COOLANT LEVEL";
                        break;
                    case 38:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "ELECTRIC PHASE REVERSED";
                        break;
                    case 39:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "WATER VALVE CIRCUIT";
                        break;
                    case 40:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "HIGH SPEED CIRCUIT";
                        break;
                    case 41:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "ENGINE COOLANT TEMPERATURE CHECK";
                        break;
                    case 42:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "UNIT FORCED TO LOW SPEED";
                        break;
                    case 43:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "UNIT FORCED TO LOW SPEED MODULATION";
                        break;
                    case 44:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN;
                        this.AlarmName = "FUEL SYSTEM CHECK *";
                        break;
                    case 45:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "HOT GAS CIRCUIT";
                        break;
                    case 46:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "AIR FLOW CHECK";
                        break;
                    case 47:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "REMOTE SENSOR SHUTDOWN";
                        break;
                    case 48:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "CHECK BELTS OR CLUTCH";
                        break;
                    case 49:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "CHECK SPARE SENSOR 1";
                        break;
                    case 50:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "RESET CLOCK";
                        break;
                    case 51:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "CHECK SHUTDOWN CIRCUIT";
                        break;
                    case 52:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "HEAT CIRCUIT";
                        break;
                    case 53:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "ECONOMIZER VALVE CIRCUIT";
                        break;
                    case 54:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "TEST MODE TIMEOUT";
                        break;
                    case 55:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK ENGINE SPEEDS";
                        break;
                    case 56:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "CHECK EVAPORATOR FAN LOW SPEED";
                        break;
                    case 57:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "CHECK EVAPORATOR FAN HIGH SPEED";
                        break;
                    case 58:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "CHECK CONDENSER FAN LOW SPEED";
                        break;
                    case 59:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "CHECK CONDENSER FAN HIGH SPEED";
                        break;
                    case 60:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "CHECK BOOST CIRCUIT";
                        break;
                    case 61:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "LOW BATTERY VOLTAGE";
                        break;
                    case 62:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "AMMETER OUT OF CALIBRATION";
                        break;
                    case 63:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "ENGINE STOPPED";
                        break;
                    case 64:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "PRETRIP REMINDER";
                        break;
                    case 65:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "ABNORMAL TEMPERATURE DIFFERENTIAL";
                        break;
                    case 66:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "LOW ENGINE OIL LEVEL";
                        break;
                    case 67:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "LIQUID LINE SOLENOID CIRCUIT";
                        break;
                    case 68:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "INTERNAL CONTROLLER FAULT CODE";
                        break;
                    case 69:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "CHECK REMAGNETIZATION CIRCUIT";
                        break;
                    case 70:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "HOURMETERS FAILURE";
                        break;
                    case 71:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "HOURMETER 4 EXCEEDS SET LIMIT";
                        break;
                    case 72:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "HOURMETER 5 EXCEEDS SET TIME LIMIT";
                        break;
                    case 73:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "HOURMETER 6 EXCEEDS SET TIME LIMIT";
                        break;
                    case 74:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CONTROLLER RESET TO DEFAULTS";
                        break;
                    case 75:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "CONTROLLER RAM FAILURE";
                        break;
                    case 76:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "CONTROLLER EPROM FAILURE";
                        break;
                    case 77:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "CONTROLLER EPROM CHECKSUM FAILURE";
                        break;
                    case 78:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "DATA LOG EPROM FAILURE";
                        break;
                    case 79:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "INTERNAL DATALOGGER OVERFLOW";
                        break;
                    case 80:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "COMPRESSOR TEMP SENSOR";
                        break;
                    case 81:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "HIGH COMPRESSOR TEMPERATURE";
                        break;
                    case 82:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "HIGH COMPRESSOR TEMPERATURE SHUTDOWN";
                        break;
                    case 83:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "LOW ENGINE COOLANT TEMPERATURE";
                        break;
                    case 84:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "RESTART 0";
                        break;
                    case 85:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "FORCED UNIT OPERATION";
                        break;
                    case 86:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK DISCHARGE PRESSURE SENSOR";
                        break;
                    case 87:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "SUCTION PRESSURE SENSOR";
                        break;
                    case 88:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "RESERVED FOR CR";
                        break;
                    case 89:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "ETV CIRCUIT";
                        break;
                    case 90:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "ELECTRIC OVERLOAD";
                        break;
                    case 91:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK ELECTRIC READY INPUT";
                        break;
                    case 92:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "SENSOR GRADES NOT SET";
                        break;
                    case 93:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "LOW COMPRESSOR SUCTION PRESSURE";
                        break;
                    case 94:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK LOADER #1 CIRCUIT";
                        break;
                    case 95:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK LOADER #2 CIRCUIT";
                        break;
                    case 96:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "LOW FUEL LEVEL";
                        break;
                    case 97:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "FAILED REMOTE RETURN AIR SENSOR (CR)";
                        break;
                    case 98:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "FUEL LEVEL SENSOR";
                        break;
                    case 99:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "HIGH COMPRESSOR PRESSURE RATIO";
                        break;
                    case 100:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "HEATER FAN FAILURE (CR)";
                        break;
                    case 101:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "CONTROLLING ON EVAP COIL OUTLET TEMP (CR)";
                        break;
                    case 102:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "LOW EVAPORATOR COIL TEMPERATURE (CR)";
                        break;
                    case 103:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "LOW HEATER FUEL LEVEL (CR)";
                        break;
                    case 104:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "CHECK REMOTE FAN SPEED";
                        break;
                    case 105:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK RECEIVER TANK PRESS SOL CIRCUIT";
                        break;
                    case 106:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK PURGE VALVE CIRCUIT";
                        break;
                    case 107:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK CONDENSER INLET SOL CIRCUIT";
                        break;
                    case 108:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "DOOR OPEN TIMEOUT";
                        break;
                    case 109:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "HIGH DISCHARGE PRESSURE SENSOR";
                        break;
                    case 110:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK SUCTION LINE SOL CIRCUIT";
                        break;
                    case 111:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "UNIT NOT CONFIGURED CORRECTLY";
                        break;
                    case 112:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK REMOTE FANS";
                        break;
                    case 113:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK ELECTRIC HEAT CIRCUIT";
                        break;
                    case 114:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "MULTIPLE ALARMS - CAN NOT RUN";
                        break;
                    case 115:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "CHECK HIGH PRESSURE CUT OUT SWITCH";
                        break;
                    case 116:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "CHECK HIGH PRESSURE CUT IN SWITCH";
                        break;
                    case 117:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "AUTO SWITCH FROM DIESEL TO ELECTRIC";
                        break;
                    case 118:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "AUTO SWITCH FROM ELECTRIC TO DIESEL";
                        break;
                    case 119:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "RESERVED FOR CR";
                        break;
                    case 120:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "CHECK ALTERNATOR EXCITE CIRCUIT";
                        break;
                    case 121:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "CHECK PMW LIQUID INJECTION CIRCUIT";
                        break;
                    case 122:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "CHECK DIESEL/ELECTRIC CIRCUIT";
                        break;
                    case 123:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK EVAP COIL INLET TEMP SENSOR";
                        break;
                    case 124:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "CHECK EVAP COIL OUTLET TEMP SENSOR";
                        break;
                    case 125:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_SHUTDOWN;
                        this.AlarmName = "CHECK TANK LEVEL SENSOR";
                        break;
                    case 126:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK BACK PRESSURE REGULATOR";
                        break;
                    case 127:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "SETPOINT NOT ENTERED";
                        break;
                    case 128:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "ENGINE RUN TIME MAINT REMINDER #1";
                        break;
                    case 129:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "ENGINE RUN TIME MAINT REMINDER #2";
                        break;
                    case 130:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "ELECTRIC RUN TIME MAINT REMINDER #1";
                        break;
                    case 131:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "ELECTRIC RUN TIME MAINT REMINDER #2";
                        break;
                    case 132:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "TOTAL UNIT RUN TIME MAINT REMINDER #1";
                        break;
                    case 133:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "TOTAL UNIT RUN TIME MAINT REMINDER #2";
                        break;
                    case 134:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "CONTROLLER POWER ON HOURS";
                        break;
                    case 135:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "CHECK SPARE DIGITAL INPUTS";
                        break;
                    case 136:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "CHECK SPARE DIGITAL OUTPUTS";
                        break;
                    case 137:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "CHECK DAMPER MOTOR HEATER OUTPUT";
                        break;
                    case 139:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "ABORT EVACUATION MODE";
                        break;
                    case 140:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "ELECTRONIC GOVERNOR";
                        break;
                    case 141:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "AUTO-SWITCH DIESEL TO ELECTRIC DISABLED";
                        break;
                    case 142:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "THERMAX VALVE";
                        break;
                    case 143:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "REMOTE ZONE DRAIN HOSE HEATER OUTPUT";
                        break;
                    case 144:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "LOSS OF EXPANSION MODULE CAN COMMUNICATION";
                        break;
                    case 145:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_LOG_ONLY;
                        this.AlarmName = "LOSS OF CONTROLLER 'ON' FEEDBACK SIGNAL";
                        break;
                    case 146:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "SOFTWARE VERSION MISMATCH";
                        break;
                    case 147:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "MULTI-TEMP FAN SPEED CONTROL OUTPUT";
                        break;
                    case 148:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "AUTOSWITCH ELECTRIC TO DIESEL DISABLED";
                        break;
                    case 149:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "ALARM NOT IDENTIFIED";
                        break;
                    case 150:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "OUT OF RANGE LOW";
                        break;
                    case 151:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "OUT OF RANGE HIGH";
                        break;
                    case 153:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_SHUTDOWN_ONLY;
                        this.AlarmName = "EXPANSION MODULE FLASHLOAD FAILURE";
                        break;
                    case 154:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "LOW SUCTION PRESSURE SWITCH FAILURE";
                        break;
                    case 155:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "LOSS OF CAN COMMUNICATION TO SR2";
                        break;
                    case 156:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_NONE;
                        this.AlarmName = "CHECK SUCTION/LIQUID HEAT EXCHANGER BYPASS VALVE CIRCUIT";
                        break;
                    case 203:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK DISPLAY RETURN AIR SENSOR";
                        break;
                    case 204:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_CHECK_ONLY;
                        this.AlarmName = "CHECK DISPLAY DISCHARGE AIR SENSOR";
                        break;
                    default:
                        this.AlarmLevel = (Int32)ReeferAlarmLevel.ALARM_INVALID;
                        this.AlarmName = "Invalid Alarm";
                        break;

                }
            }
        }

        private enum ReeferAlarmLevel
        {
            ALARM_INVALID = -1,
            ALARM_NONE = 0,
            ALARM_CHECK_ONLY = 1,
            ALARM_LOG_ONLY = 2,
            ALARM_CHECK_SHUTDOWN = 3,
            ALARM_CHECK_PREVENT = 4,
            ALARM_SHUTDOWN_ONLY = 5,
            ALARM_SHUTDOWN = 6
        }
        #endregion

    }
}