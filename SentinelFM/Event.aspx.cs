using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using ClosedXML.Excel;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using Newtonsoft.Json;
using VLF.CLS.Def;
using VLF.DAS.Logic;
using System.Web.SessionState;
using System.Net.Mail;
using System.Net;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using System.Text.RegularExpressions;
using VLF.Reports;


namespace SentinelFM
{
    public partial class Event : SentinelFMBasePage
    {
        
        string defaultFleetName = "";
        //static string strFromDate;
        //static string strToDate;
        //static string FleetIds;
        //static string operation;
        //static string events;
        //static string vehicleId;
        //static DateTime dtsd;
        //static DateTime dttd;
        //static DataSet DSGlobal = new DataSet();

        public delegate void FillData(string events, string vehicleId, DateTime dtsd, DateTime dttd, int OrganizationId, int currentPage, int limit, int box);
        public bool VehcileNotReported = false;
        public List<int> ScheduleTimeShowHide;
        public List<int> defaultDays;
        static int NoOfDays = 0;
        //static readonly object _object = new object();
        int box = 0;
        static int limit = 0;
        static int boxDataCount = 0;
        static int ct = 1;
        static string sConnectionString2 = ConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;
        EvtEvents eeventForBuffer = new EvtEvents(sConnectionString2);
        static string sConnectionString3 = ConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;
        EvtEvents eeventBoxEvent = new EvtEvents(sConnectionString3);
        FillData fillDataEvent;
        FillData fillDataViolation;
        IAsyncResult tagEvent;
        IAsyncResult tagViolation;
        int flag = 0;
        string RecordsToFetch = "";
        int nDefaultFleetIndex = -1;
       

       
        protected void Page_Load(object sender, EventArgs e)
        {
           
            RecordsToFetch = sn.User.RecordsToFetch;               
           
            string request = Request.QueryString["QueryType"];
            if (request == "GetFleet")
            {
                GetFleet();
            }
            if (request == "GetVehicle")
            {
                GetVehicle();
            }
            if (request == "GetEventType")
            {
                GetEventType();
            }
            if (request == "GetViolationList")
            {
                GetViolationList();
            }
            if (request == "GetEvents")
            {
                GetEvents();
            }
            if (request == "shareDatatable")
            {
                shareDatatable();
            }
            if (request == "scheduleReport")
            {
                scheduleReport();
            }
            if (request == "FillBufferRecursivelyEvent")
            {
                GetDataEvents();
            }
            if (request == "FillBufferRecursivelyViolation")
            {
                GetDataViolation();
            }

            if (request == "SetDefaultColumnPreferences")
            {
                SetDefaultColumnPreferences();
            }

            if (!IsPostBack)
            {

                if (Session["cacheId"] == null)
                {
                    string cacheId = "User" + sn.UserID + DateTime.Now;
                    Session["cacheId"] = cacheId;                

                }

        }
        }

        private void GetFleet()
        {

            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.Default;
            int pageno = Convert.ToInt32(Request.QueryString["pageno"]);
            int start = Convert.ToInt32(Request.QueryString["start"]);
            int limit = Convert.ToInt32(Request.QueryString["limit"]);
            string Search = "";
            if (Request.QueryString["Search"] != null)
                Search = Request.QueryString["Search"];
            if (Session["FleetDataset"] == null)
            {
                Session["FleetDataset"] = getFleetSupport();
            }

            DataTable dt = ((DataSet)Session["FleetDataset"]).Tables[0];
            try
            {
                defaultFleetName = dt.Select("FleetId=" + sn.User.DefaultFleet)[0]["FleetName"].ToString();
            }
            catch
            {
                defaultFleetName = "Unknown";
            }
            if (dt.Rows.Count > 1 && !String.IsNullOrEmpty(Search))
            {
                try
                {
                    dt = dt.Select("FleetName like '%" + Search + "%'").CopyToDataTable();
                }
                catch
                {
                    dt = new DataTable();
                }
            }

            int _nDefaultFleetId = -100;
            nDefaultFleetIndex = -1;
            try
            {
                _nDefaultFleetId = Convert.ToInt32(sn.User.DefaultFleet);
                if (_nDefaultFleetId != -100)
                    nDefaultFleetIndex = ((DataSet)Session["FleetDataset"]).Tables[0].Rows.IndexOf(((DataSet)Session["FleetDataset"]).Tables[0].Rows.Find(_nDefaultFleetId));
            }
            catch
            {
                _nDefaultFleetId = -1;
                nDefaultFleetIndex = -1;
            }

            if (pageno > 0 && start < pageno)
                dt = dt.AsEnumerable().Skip(start * limit).Take(limit).CopyToDataTable();
            else
                dt = dt.AsEnumerable().Skip(start).Take(limit).CopyToDataTable();           
            dt.TableName = "Fleet";
            StringWriter sw = new StringWriter();
            dt.WriteXml(sw);
            string xml = sw.ToString();
            xml = xml.Replace("<DocumentElement>", "<DocumentElement><totalCount>" + ((DataSet)Session["FleetDataset"]).Tables[0].Rows.Count.ToString() + "</totalCount><message>" + defaultFleetName + "</message>");
            xml = xml.Replace("<DocumentElement>", "<DocumentElement><DefaultFleetIndex>" + nDefaultFleetIndex + "</DefaultFleetIndex>");  
            Response.Write(xml);
        }

        private DataSet getFleetSupport()
        {
            DataSet dsFleets = new DataSet();
            try
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                EvtEvents eevent = new EvtEvents(sConnectionString);
                dsFleets = eevent.GetFleets(Convert.ToInt32(sn.UserID));          
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            }
            return dsFleets;
        }

        private void GetVehicle()
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.Default;
            string strFleetIDs = Request.QueryString["Fleet"];
            string Search = Request.QueryString["SearchVehicle"];
            int pageno = Convert.ToInt32(Request.QueryString["pageno"]);
            int start = Convert.ToInt32(Request.QueryString["start"]);
            int limit = Convert.ToInt32(Request.QueryString["limit"]);            
            string FleetFirstTime = Convert.ToString(Request.QueryString["FleetFirstTime"]);
            string sExcludeFleetIDs = "";

            if (Request.QueryString["ExcludeFleetIDs"] != null)
                sExcludeFleetIDs = Request.QueryString["ExcludeFleetIDs"].ToString();

            
            try
            {
                string SelectAllFleetFlag = Convert.ToString(Request.QueryString["SelectAllFleetFlag"]);
            }
            catch { }
            string xml = "";
            DataTable dt = new DataTable();
            DataTable dtVehicle = new DataTable();
            
            if (String.IsNullOrEmpty(Request.QueryString["Fleet"].ToString()) || Request.QueryString["Fleet"].ToString() == ",-1,")
            {
                strFleetIDs = "";
                foreach (DataRow r in ((DataSet)Session["FleetDataset"]).Tables[0].Rows)
                {
                    strFleetIDs += "," + r["FleetId"].ToString();
                }
                strFleetIDs += ",";
            }
            else
            {
                if (sExcludeFleetIDs.ToLower() == "true")
                {
                    try
                    {
                        strFleetIDs = "";
                        string RequestStringFleetIDs = Request.QueryString["Fleet"].ToString().Trim();
                        if (RequestStringFleetIDs.StartsWith(","))
                            RequestStringFleetIDs = RequestStringFleetIDs.Substring(1);
                        if (RequestStringFleetIDs.EndsWith(","))
                            RequestStringFleetIDs = RequestStringFleetIDs.Substring(0, (RequestStringFleetIDs.Length - 1));

                        DataRow[] drActualFleetIDs = ((DataSet)Session["FleetDataset"]).Tables[0].Select("FleetId NOT IN (" + RequestStringFleetIDs + ")");

                        foreach (DataRow dr in drActualFleetIDs)
                        {
                            strFleetIDs += ("," + dr["FleetId"].ToString());
                        }
                        strFleetIDs += ","; 
                    }
                    catch (Exception Ex)
                    {

                    }
                }

                else
                {
                    strFleetIDs = Request.QueryString["Fleet"].ToString().Trim();                    
                } 
            }

            Session["FleetIds"] = strFleetIDs;

            if (Session["dsVehicles"] == null || FleetFirstTime.ToLower() == "true")
            {
                if(getVehicleSupport(strFleetIDs).Tables.Count > 0)
                    dtVehicle = getVehicleSupport(strFleetIDs).Tables[0];
             
            }
            else
            {
                if (Session["dsVehicles"] != null)
                    dtVehicle = ((DataSet)Session["dsVehicles"]).Tables[0];
            }
            if (dtVehicle.Rows.Count > 0)
            {
                dt = dtVehicle.DefaultView.ToTable(false, "VehicleId", "Description");
                dt.Columns["Description"].ColumnName = "VehicleName";
               
                var UniqueRows = dt.AsEnumerable().Distinct(DataRowComparer.Default);
                dt = UniqueRows.CopyToDataTable();
                
                if (dt.Rows.Count > 1 && !String.IsNullOrEmpty(Search))
                {
                    try
                    {
                        dt = dt.Select("VehicleName like '%" + Search + "%'").CopyToDataTable();
                    }
                    catch
                    {
                        dt = new DataTable();
                    }
                }
                Session["dsVehiclesUpdate"] = dt;
                
                int count = 0;
                if (dt != null)
                    count = dt.Rows.Count;
                if (count > 0)
                {
                    if (pageno > 0 && start < pageno)
                        dt = dt.AsEnumerable().Skip(start * limit).Take(limit).CopyToDataTable();
                    else
                        dt = dt.AsEnumerable().Skip(start).Take(limit).CopyToDataTable();
                }
                dt.TableName = "Vehicle";
                StringWriter sw = new StringWriter();
                dt.WriteXml(sw);
                xml = sw.ToString();
                xml = xml.Replace("<DocumentElement>", "<DocumentElement><totalCount>" + count.ToString() + "</totalCount>");
                Response.Write(xml);
            }
        }

        private DataSet getVehicleSupport(string FleetId)
        {
            DataSet dsVehicles = new DataSet();
            bool MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
            
                if (FleetId.Contains(","))
                {
                    string fleetIds = FleetId;
                    if (fleetIds != null && fleetIds.Trim() != string.Empty)
                    {
                        dsVehicles = GetVehicleListByMultipleFleetIds(fleetIds);
                    Session["dsVehicles"] = dsVehicles;
                    }
                }
                else
                {
                    int fleetID = 0;
                    if (!string.IsNullOrEmpty(FleetId))
                    {
                        Int32.TryParse(FleetId, out fleetID);
                    }
                    if (fleetID > 0)
                    {
                        dsVehicles = GetVehicleListByFleetId(fleetID);
                    Session["dsVehicles"] = dsVehicles;
                    }
                }

            return dsVehicles;
        }

        private DataSet GetVehicleListByFleetId(int fleetId)
        {
            DataSet dsVehicle = new DataSet();
            try
            {
                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                    {
                        return dsVehicle;
                    }
                if (xml == "")
                {
                    return dsVehicle;
                }

                StringReader strrXML = new StringReader(xml);
                dsVehicle = new DataSet();
                dsVehicle.ReadXml(strrXML);

                sn.History.DsHistoryVehicles = dsVehicle;
            }
            catch
            {
            }
            return dsVehicle;

        }

        private DataSet GetVehicleListByMultipleFleetIds(string fleetIds)
        {
            DataSet dsVehicle = new DataSet();
            try
            {
                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByMultipleFleetIds(sn.UserID, sn.SecId, fleetIds, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByMultipleFleetIds(sn.UserID, sn.SecId, fleetIds, ref xml), true))
                    {
                        return dsVehicle;
                    }
                if (xml == "")
                {
                    return dsVehicle;
                }

                StringReader strrXML = new StringReader(xml);

                dsVehicle = new DataSet();
                dsVehicle.ReadXml(strrXML);

                sn.History.DsHistoryVehicles = dsVehicle;
            }
            catch
            {
            }
            return dsVehicle;
        }

        private void GetEventType()
        {
            try
            {
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.Default;

                string xml = "";
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                EvtEvents eevent = new EvtEvents(sConnectionString);
                xml = eevent.GetEventType().GetXml();

                if (xml == "")
                {
                    return;
                }
                xml = xml.Trim();
                Response.Write(xml.Trim());
            }
            catch
            {
            }
        }


        private void GetViolationList()
        {
            try
            {

                //organizationDays = clsPermission.GetFeaturePermissionData(sn, "NotReportedforXDays");
                //NoOfDays = organizationDays[0];
                NoOfDays = sn.User.VehicleNotReported;

            }
            catch
            {
                NoOfDays = 3;
            }
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.Default;

            string table = "";
            string xml = "";
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            EvtEvents eevent = new EvtEvents(sConnectionString);
            table = Request.QueryString["Action"];
            if (table == null)
                table = "Violation";
            //xml = eevent.GetViolationByOrganization(Convert.ToInt32(sn.User.OrganizationId)).GetXml();
            DataSet dataset = new DataSet();
             dataset = eevent.GetSelectedEventViolationByOrganization(Convert.ToInt32(sn.User.OrganizationId), table);
             if (dataset == null || dataset.Tables[0].Rows.Count == 0)
             {
                 dataset = eevent.GetViolationByOrganization(Convert.ToInt32(sn.User.OrganizationId), table);
                 dataset.Tables[0].Rows.Add("-2", "* Vehicle Not Reported for last " + NoOfDays + " days");
             }

            //dataset.Tables[0].Rows.Add("-2", "* Vehicle Not Reported for last " + NoOfDays + " days");

            //Apply sorting
            DataView dv = dataset.Tables[0].DefaultView;
            dv.Sort = "Description ASC";
            DataTable sortedDT = dv.ToTable();
            DataSet dsSorted = new DataSet();
            dsSorted.Tables.Add(sortedDT);

            xml = dsSorted.GetXml();
            if (xml == "")
            {
                return;
            }
            xml = xml.Trim();
            Response.Write(xml.Trim());
        }

       
       
        private void GetEvents()
        {

            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.Default;


            try
            {
                string strFromDate;
                string strToDate;
                string FleetIds;
                string operation;
                string vehicleId;
                string events;
                string FetchMoreRecords;
                DateTime dtsd;
                DateTime dttd;
                DataSet DSGlobal = new DataSet();
                int totalrecords = 0;
                //string cacheId = Convert.ToString(Session["cacheId"]);
                string xml = "";


                DataSet dataSet = new DataSet();
                string FromDB = "";
                string FirstTime = "";
                if (Request.QueryString["FromDB"] != null)
                    FromDB = Request.QueryString["FromDB"].ToString();
                if (Request.QueryString["FirstTime"] != null)
                    FirstTime = Request.QueryString["FirstTime"].ToString();
                string searchText = "";
                if (Request.QueryString["Search"] != null)
                    searchText = Request.QueryString["Search"].ToString();
                string searchCol = "";
                if (Request.QueryString["Columns"] != null)
                    searchCol = Request.QueryString["Columns"].ToString();

                string sConnectionString = ConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;
                EvtEvents eevent = new EvtEvents(sConnectionString);

                string sBoxConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                EvtEvents boxevent = new EvtEvents(sBoxConnectionString);
                DataSet BoxDataSet = new DataSet();
                int start = Convert.ToInt32(Convert.ToDouble(Request.QueryString["start"]));
               
                limit = Convert.ToInt32(Request.QueryString["limit"]);
                operation = Request.QueryString["Action"];
                Session["operation"] = operation;
                
               
                FetchMoreRecords = Request.QueryString["FetchMoreRecords"];
                if (String.IsNullOrEmpty(FetchMoreRecords))
                    FetchMoreRecords = "false";
                
                try
                {

                    //organizationDays = clsPermission.GetFeaturePermissionData(sn, "NotReportedforXDays");
                    NoOfDays = sn.User.VehicleNotReported;

                }
                catch
                {
                    NoOfDays = 3;
                }
                int currentPage = 0;
                if (limit > 0)
                {
                    if (start == 0)
                        currentPage = (start + 1);

                    if (currentPage == 0)
                    {
                        currentPage = 1;
                    }
                }
                if (String.IsNullOrEmpty(operation))
                    return;
                strFromDate = Request.QueryString["startDate"].ToString();
                Session["strFromDate"] = strFromDate;
                strToDate = Request.QueryString["EndDate"].ToString();
                Session["strToDate"] = strToDate;

                try
                {
                    dtsd = DateTime.ParseExact(strFromDate, sn.User.DateFormat + " " + sn.User.TimeFormat, CultureInfo.InvariantCulture);
                    Session["dtsd"] = dtsd;
                }
                catch
                {
                    xml = "<DocumentElement><message>Session Expired</message></DocumentElement>";
                    xml = xml.Trim();
                    Response.Write(xml.Trim());
                    return;
                }
                dttd = DateTime.ParseExact(strToDate, sn.User.DateFormat + " " + sn.User.TimeFormat, CultureInfo.InvariantCulture);
                Session["dttd"] = dttd;
                string SelectAllVehicleFlag = "";
                if (Request.QueryString["SelectAllVehicleFlag"] != null)
                    SelectAllVehicleFlag = Request.QueryString["SelectAllVehicleFlag"].ToString();
                
                if (FromDB == "true")
                {
                    vehicleId = "";
                    if (SelectAllVehicleFlag == "true")
                    {
                        
                        string DeselectVehicleIds = Request.QueryString["VehicleIds"].ToString();
                        if (DeselectVehicleIds.StartsWith(","))
                            DeselectVehicleIds = DeselectVehicleIds.Substring(1);
                        if (DeselectVehicleIds.EndsWith(","))
                            DeselectVehicleIds = DeselectVehicleIds.Substring(0, (DeselectVehicleIds.Length - 1));

                        foreach (DataRow r in ((DataTable)Session["dsVehiclesUpdate"]).Select("VehicleId NOT IN (" + DeselectVehicleIds + ")"))
                        {
                            vehicleId += "," + r["VehicleId"].ToString();
                        }
                        vehicleId += ",";
                    }
                    else if (String.IsNullOrEmpty(Request.QueryString["VehicleIds"].ToString()) || Request.QueryString["VehicleIds"].ToString() == ",-1,")
                    {
                        foreach (DataRow r in ((DataTable)Session["dsVehiclesUpdate"]).Rows)
                        {
                            vehicleId += "," + r["VehicleId"].ToString();
                        }
                        vehicleId += ",";
                    }
                    else
                    {
                    try
                    {
                            //vehicleId += String.Join(",", getVehicleSupport(FleetIds).Tables[0].AsEnumerable().Select(x => x.Field<string>("VehicleId").ToString()).Distinct().ToArray()) + ",";
                            vehicleId = Request.QueryString["VehicleIds"].ToString();
                        
                    }
                    catch
                    {
                        xml = "<DocumentElement><message>No vehicle for selected fleet.</message></DocumentElement>";
                        xml = xml.Trim();
                        Response.Write(xml.Trim());
                        return;
                    }
                    }
                    Session["vehicleId"] = vehicleId;
                    //fleetIds for getting fleetname
                    //string fleetIds = Convert.ToString(Session["FleetIds"]);
                    events = "," + Request.QueryString["Events"].ToString() + ",";
                    Session["events"] = events;
                    if (events.IndexOf("-2") > 0 && start == 0)
                    {
                        box = 1;
                    }

                    if (FirstTime.ToLower() == "true")
                    {
                        Session["count"] = RecordsToFetch;
                    }

                    if (operation == "Event" && FirstTime.ToLower() == "true")
                    {
                        if (start == 0)
                        {
                           
                                Cache.Remove(Convert.ToString(Session["cacheId"]));
                                DSGlobal = null;
                                if (NoOfDays > 0 && box == 1)
                                {
                                    BoxDataSet = boxevent.GetBoxData(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, NoOfDays, sn.UserID, out totalrecords, operation);
                                }

                                DSGlobal = eevent.GetEvent(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, NoOfDays, sn.UserID, BoxDataSet, Convert.ToInt32(RecordsToFetch));
                                //Remove & insert dataset in cache
                                //lock (_object)
                                {
                                    //Cache.Remove(cacheId);
                                    Cache.Insert(Convert.ToString(Session["cacheId"]), DSGlobal, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
                                }
                                boxDataCount = totalrecords;

                                
                            if (DSGlobal.Tables.Count == 0)
                            {
                                string xmlMsg = "<DocumentElement><message>No Data are available for selected search criteria </message></DocumentElement>";
                                xmlMsg = xmlMsg.Trim();
                                Response.Write(xml.Trim());
                                return;
                            }
                            else if (DSGlobal.Tables[0].Rows.Count == 0)
                            {
                                string xmlMsg = "<DocumentElement><message>No Data are available for selected search criteria </message></DocumentElement>";
                                xmlMsg = xmlMsg.Trim();
                                Response.Write(xml.Trim());
                                return;
                            }
                        }                       


                    }
                    if (operation == "Event" && FetchMoreRecords.ToLower() == "true")
                    {                        
                      GetDataEvents(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box);   
                    }


                    if (operation == "Violation" && FirstTime.ToLower() == "true")
                    {
                        if (start == 0)
                        {
                            Cache.Remove(Convert.ToString(Session["cacheId"]));
                            DSGlobal = null;
                            if (NoOfDays > 0 && box == 1)
                            {
                                BoxDataSet = boxevent.GetBoxData(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, NoOfDays, sn.UserID, out totalrecords, operation);
                            }
                            DSGlobal = eevent.GetViolation(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, NoOfDays, sn.UserID, BoxDataSet, Convert.ToInt32(RecordsToFetch));
                            //lock (_object)
                            {
                                //Cache.Remove(cacheId);
                                Cache.Insert(Convert.ToString(Session["cacheId"]), DSGlobal, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
                            }
                            boxDataCount = totalrecords;
                            
                            if (DSGlobal.Tables.Count == 0)
                            {
                                string xmlMsg = "<DocumentElement><message>No Data are available for selected search criteria </message></DocumentElement>";
                                xmlMsg = xmlMsg.Trim();
                                Response.Write(xml.Trim());
                                return;
                            }
                            else if (DSGlobal.Tables[0].Rows.Count == 0)
                            {
                                string xmlMsg = "<DocumentElement><message>No Data are available for selected search criteria </message></DocumentElement>";
                                xmlMsg = xmlMsg.Trim();
                                Response.Write(xml.Trim());
                                return;
                            }

                        }                        

                    }
                    if (operation == "Violation" && FetchMoreRecords.ToLower() == "true")
                    {                       
                     GetDataViolation(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box);                    
                    }


                }
                StringWriter sw = new StringWriter();
                DataTable dt = new DataTable();

                //lock (DSGlobal)
                //{
                DSGlobal = null;
                DSGlobal = (DataSet)Cache[Convert.ToString(Session["cacheId"])];
                //dt = DSGlobal.Tables[0].Copy();
                //lock (_object)
                {

                    if (DSGlobal != null && DSGlobal.Tables.Count > 0)
                    {
                        // dt = DSGlobal.Tables[0].Copy();
                        foreach (DataColumn dc in DSGlobal.Tables[0].Columns)
                        {

                            if (dc.DataType.ToString() == "System.DateTime")
                            {
                                dt.Columns.Add(dc.ColumnName.ToString(), dc.DataType);
                            }
                            else
                            {
                                dt.Columns.Add(dc.ColumnName.ToString(), typeof(string));

                            }
                        }

                        foreach (DataRow dr in DSGlobal.Tables[0].Rows)
                        {
                            dt.ImportRow(dr);
                        }
                    }


                }
                DataTable dtc = new DataTable();
                if (!String.IsNullOrEmpty(searchText))
                {
                    if (!String.IsNullOrEmpty(searchCol))
                    {
                        //if (searchCol.Contains("Details"))
                        //    searchCol += "MakeName,ModelName,LicensePlate";
                        foreach (DataColumn dc in dt.Columns)
                        {
                            dtc.Columns.Add(dc.ColumnName.ToString(), dc.DataType);
                        }
                        foreach (string s in searchCol.Split(','))
                        {
                            if (String.IsNullOrEmpty(s))
                                continue;
                            string colname = s;
                            if (dt.Columns.Contains(s))
                            {
                                int n;
                                if (dt.Columns[s].DataType.Name.Contains("Int") && int.TryParse(searchText, out n))
                                {
                                    if (dt.Select(string.Format("{0} = {1}", colname, searchText)).Length > 0)
                                        dtc.Merge(dt.Select(string.Format("{0} = {1}", colname, searchText)).CopyToDataTable());
                                }
                                else if (dt.Columns[s].DataType.Name.Contains("String"))
                                {
                                    if (dt.Select(string.Format("{0} LIKE '%{1}%'", colname, searchText)).Length > 0)
                                        dtc.Merge(dt.Select(string.Format("{0} LIKE '%{1}%'", colname, searchText)).CopyToDataTable());
                                }
                            }
                        }
                        dt = dtc.DefaultView.ToTable(true);
                        Session["TotalSearchRecords"] = dt.Rows.Count;


                    }
                }

                if (dt.Rows.Count > 0)
                {

                    if (searchText != "")
                    {
                        if (Session["TotalSearchRecords"] != null)
                        {
                            totalrecords = (int)Session["TotalSearchRecords"];
                        }

                    }
                    else
                    {

                        totalrecords = dt.Rows.Count;
                    }
                    string SortBy = Request.QueryString["SortBy"];
                    if (!string.IsNullOrEmpty(SortBy))
                    {
                        dt.DefaultView.Sort = SortBy.Split(',')[0] + " " + SortBy.Split(',')[1];
                        dt = dt.DefaultView.ToTable();
                    }

                    if (searchText != "")
                    {
                        start = 0;
                    }

                  
                    int nCurrentRecordCount = 0;
                    try
                    {
                        nCurrentRecordCount = Convert.ToInt32(Session["Count"]);
                    }
                    catch
                    {
                        nCurrentRecordCount = 0;
                    }

                        dt = dt.Rows.Cast<System.Data.DataRow>().Skip(start * limit).Take(limit).CopyToDataTable();
                        dt.TableName = "Table";
                        dt.WriteXml(sw);
                        xml = sw.ToString();
   
                    
                    xml = xml.Replace("<DocumentElement>", "<DocumentElement><totalCount>" + totalrecords + "</totalCount>");
                    xml = xml.Replace("<DocumentElement>", "<DocumentElement><EventsFetched>" + nCurrentRecordCount + "</EventsFetched>");                    
                    
                }
                //else if (totalrecords > 6000)
                //    xml = "<DocumentElement><message>Too many records requested.Narrow down search criterion.</message></DocumentElement>";
                if (xml == "")
                {
                    return;
                }
                xml = xml.Trim();
                Response.Write(xml.Trim());

            }
            catch (Exception Ex)
            {
                string xml = "<DocumentElement><message>" + Ex.Message + "</message></DocumentElement>";
                xml = xml.Trim();
                Response.Write(xml.Trim());
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString() + " From: GetEvents"));
            }
        }

        //private void GetEvents()
        //{
            
        //    Response.ContentType = "text/xml";
        //    Response.ContentEncoding = Encoding.Default;
            

        //    try
        //    {
        //        string strFromDate;
        //        string strToDate;
        //        string FleetIds;
        //        string operation;
        //        string vehicleId;
        //        string events;
        //        DateTime dtsd;
        //        DateTime dttd;
        //        DataSet DSGlobal = new DataSet();
        //        int totalrecords = 0;
        //        //string cacheId = Convert.ToString(Session["cacheId"]);
        //        string xml = "";
               
                
        //        DataSet dataSet = new DataSet();
        //        string FromDB = "";
        //        string FirstTime = "";
        //        if (Request.QueryString["FromDB"] != null)
        //            FromDB = Request.QueryString["FromDB"].ToString();
        //        if (Request.QueryString["FirstTime"] != null)
        //            FirstTime = Request.QueryString["FirstTime"].ToString();
        //        string searchText = "";
        //        if (Request.QueryString["Search"] != null)
        //            searchText = Request.QueryString["Search"].ToString();
        //        string searchCol = "";
        //        if (Request.QueryString["Columns"] != null)
        //            searchCol = Request.QueryString["Columns"].ToString();
               
        //        string sConnectionString = ConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;
        //        EvtEvents eevent = new EvtEvents(sConnectionString);
              
        //        string sBoxConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        //        EvtEvents boxevent = new EvtEvents(sBoxConnectionString);
        //        DataSet BoxDataSet = new DataSet();
        //        int start = Convert.ToInt32(Request.QueryString["start"]);
        //        limit = Convert.ToInt32(Request.QueryString["limit"]);
        //        operation = Request.QueryString["Action"];
        //        Session["operation"] = operation;
        //        try
        //        {

        //            //organizationDays = clsPermission.GetFeaturePermissionData(sn, "NotReportedforXDays");
        //            NoOfDays = sn.User.VehicleNotReported;

        //        }
        //        catch
        //        {
        //            NoOfDays = 3;
        //        }
        //        int currentPage = 0;
        //        if (limit > 0)
        //        {
        //            if (start == 0)
        //                currentPage = (start + 1);
                    
        //            if (currentPage == 0)
        //            {
        //                currentPage = 1;
        //            }
        //        }
        //        if (String.IsNullOrEmpty(operation))
        //            return;
        //        strFromDate = Request.QueryString["startDate"].ToString();
        //        Session["strFromDate"] = strFromDate;
        //        strToDate = Request.QueryString["EndDate"].ToString();
        //        Session["strToDate"] = strToDate;

        //        try
        //        {
        //            dtsd = DateTime.ParseExact(strFromDate, sn.User.DateFormat + " " + sn.User.TimeFormat, CultureInfo.InvariantCulture);
        //            Session["dtsd"] = dtsd;
        //        }
        //        catch
        //        {
        //            xml = "<DocumentElement><message>Session Expired</message></DocumentElement>";
        //            xml = xml.Trim();
        //            Response.Write(xml.Trim());
        //            return;
        //        }
        //        dttd = DateTime.ParseExact(strToDate, sn.User.DateFormat + " " + sn.User.TimeFormat, CultureInfo.InvariantCulture);
        //        Session["dttd"] = dttd;
        //        if (FromDB == "true")
        //        {
        //            FleetIds = "";
        //            if (String.IsNullOrEmpty(Request.QueryString["FleetIds"].ToString()) || Request.QueryString["FleetIds"].ToString() == ",-1,")
        //            {
        //                foreach (DataRow r in ((DataSet)Session["FleetDataset"]).Tables[0].Rows)
        //                {
        //                    FleetIds += "," + r["FleetId"].ToString();
        //                }
        //                FleetIds += ",";
        //            }
        //            else
        //                FleetIds = Request.QueryString["FleetIds"].ToString();
        //            Session["FleetIds"] = FleetIds;
        //            vehicleId = ",";
        //            try
        //            {
        //                vehicleId += String.Join(",", getVehicleSupport(FleetIds).Tables[0].AsEnumerable().Select(x => x.Field<string>("VehicleId").ToString()).Distinct().ToArray()) + ",";
        //                Session[vehicleId] = vehicleId;
        //            }
        //            catch
        //            {
        //                xml = "<DocumentElement><message>No vehicle for selected fleet.</message></DocumentElement>";
        //                xml = xml.Trim();
        //                Response.Write(xml.Trim());
        //                return;
        //            }
                    
        //            events = "," + Request.QueryString["Events"].ToString() + ",";
        //            Session["events"] = events;
        //            if (events.IndexOf("-2") > 0 && start == 0)
        //            {
        //                box = 1;
        //            }
                    
        //            if (FirstTime.ToLower() == "true")
        //            {
        //                string cacheId = "User" + sn.UserID + DateTime.Now;
        //                Session["cacheId"] = cacheId;
        //            }
                    
        //            if (operation == "Event" && FirstTime == "true")
        //            {
        //                if (start == 0)
        //                {                            
        //                    Cache.Remove(Convert.ToString(Session["cacheId"]));
        //                    DSGlobal = null;
        //                    if (NoOfDays > 0)
        //                    {
        //                        BoxDataSet = boxevent.GetBoxData(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, NoOfDays, sn.UserID, out totalrecords, operation);
        //                    }

        //                    DSGlobal = eevent.GetEvent(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, NoOfDays, sn.UserID, BoxDataSet);
        //                    //Remove & insert dataset in cache
        //                    //lock (_object)
        //                    {
        //                        //Cache.Remove(cacheId);
        //                        Cache.Insert(Convert.ToString(Session["cacheId"]), DSGlobal, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
        //                    }
        //                    boxDataCount = totalrecords;
        //                    if (DSGlobal.Tables.Count > 0 && DSGlobal.Tables[0].Rows.Count > 0)
        //                    {
        //                        if ((DSGlobal.Tables[0].Rows.Count - boxDataCount) >= 500)
        //                        {
        //                            Session["EVflag"] = 1;
        //                            fillDataEvent = new FillData(GetDataEvents);

        //                            tagEvent = fillDataEvent.BeginInvoke(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, null, null);
        //                        }
        //                    }
        //                    if (DSGlobal.Tables.Count == 0)
        //                    {
        //                        string xmlMsg = "<DocumentElement><message>No Data are available for selected search criteria </message></DocumentElement>";
        //                        xmlMsg = xmlMsg.Trim();
        //                        Response.Write(xml.Trim());
        //                        return;
        //                    }
        //                    else if (DSGlobal.Tables[0].Rows.Count == 0)
        //                    {
        //                        string xmlMsg = "<DocumentElement><message>No Data are available for selected search criteria </message></DocumentElement>";
        //                        xmlMsg = xmlMsg.Trim();
        //                        Response.Write(xml.Trim());
        //                        return;
        //                    }
        //                }
                        
        //            }
        //            if (operation == "Violation" && FirstTime == "true")
        //            {
        //                if (start == 0)
        //                {
        //                    Cache.Remove(Convert.ToString(Session["cacheId"]));
        //                    DSGlobal = null;

        //                    BoxDataSet = boxevent.GetBoxData(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, NoOfDays, sn.UserID, out totalrecords, operation);
        //                    DSGlobal = eevent.GetViolation(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, NoOfDays, sn.UserID,  BoxDataSet);
        //                    //lock (_object)
        //                    {
        //                        //Cache.Remove(cacheId);
        //                        Cache.Insert(Convert.ToString(Session["cacheId"]), DSGlobal, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
        //                    }
        //                    boxDataCount = totalrecords;
        //                    if (DSGlobal.Tables.Count > 0 && DSGlobal.Tables[0].Rows.Count > 0)
        //                    {
        //                        if ((DSGlobal.Tables[0].Rows.Count - boxDataCount) >= 500)
        //                        {

        //                            Session["EVflag"] = 1;
        //                            fillDataViolation = new FillData(GetDataViolation);
        //                            tagViolation = fillDataViolation.BeginInvoke(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, null, null);
        //                        }
        //                    }
        //                    if (DSGlobal.Tables.Count == 0)
        //                    {
        //                        string xmlMsg = "<DocumentElement><message>No Data are available for selected search criteria </message></DocumentElement>";
        //                        xmlMsg = xmlMsg.Trim();
        //                        Response.Write(xml.Trim());
        //                        return;
        //                    }
        //                    else if (DSGlobal.Tables[0].Rows.Count == 0)
        //                    {
        //                        string xmlMsg = "<DocumentElement><message>No Data are available for selected search criteria </message></DocumentElement>";
        //                        xmlMsg = xmlMsg.Trim();
        //                        Response.Write(xml.Trim());
        //                        return;
        //                    }
                           
        //                }                       
                        
        //            }
                   

        //        }
        //        StringWriter sw = new StringWriter();
        //        DataTable dt = new DataTable();

        //        //lock (DSGlobal)
        //        //{
        //        DSGlobal = null;
        //        DSGlobal = (DataSet)Cache[Convert.ToString(Session["cacheId"])];
        //        //dt = DSGlobal.Tables[0].Copy();
        //        //lock (_object)
        //        {

        //            if (DSGlobal != null && DSGlobal.Tables.Count > 0)
        //            {
        //                // dt = DSGlobal.Tables[0].Copy();
        //                foreach (DataColumn dc in DSGlobal.Tables[0].Columns)
        //                {
                           
        //                    if (dc.DataType.ToString() == "System.DateTime")
        //                    {
        //                        dt.Columns.Add(dc.ColumnName.ToString(), dc.DataType);
        //                    }
        //                    else
        //                    {
        //                        dt.Columns.Add(dc.ColumnName.ToString(), typeof(string));

        //                    }
        //                }

        //                foreach (DataRow dr in DSGlobal.Tables[0].Rows)
        //                {
        //                    dt.ImportRow(dr);
        //                }
        //            }


        //        }
        //        DataTable dtc = new DataTable();
        //        if (!String.IsNullOrEmpty(searchText))
        //        {
        //            if (!String.IsNullOrEmpty(searchCol))
        //            {
        //                //if (searchCol.Contains("Details"))
        //                //    searchCol += "MakeName,ModelName,LicensePlate";
        //                foreach (DataColumn dc in dt.Columns)
        //                {
        //                    dtc.Columns.Add(dc.ColumnName.ToString(), dc.DataType);
        //                }
        //                foreach (string s in searchCol.Split(','))
        //                {
        //                    if (String.IsNullOrEmpty(s))
        //                        continue;
        //                    string colname = s;
        //                    if (dt.Columns.Contains(s))
        //                    {
        //                        int n;
        //                        if (dt.Columns[s].DataType.Name.Contains("Int") && int.TryParse(searchText, out n))
        //                        {
        //                            if (dt.Select(string.Format("{0} = {1}", colname, searchText)).Length > 0)
        //                                dtc.Merge(dt.Select(string.Format("{0} = {1}", colname, searchText)).CopyToDataTable());
        //                        }
        //                        else if (dt.Columns[s].DataType.Name.Contains("String"))
        //                        {
        //                            if (dt.Select(string.Format("{0} LIKE '%{1}%'", colname, searchText)).Length > 0)
        //                                dtc.Merge(dt.Select(string.Format("{0} LIKE '%{1}%'", colname, searchText)).CopyToDataTable());
        //                        }
        //                    }
        //                }
        //                dt = dtc.DefaultView.ToTable(true);
        //                Session["TotalSearchRecords"] = dt.Rows.Count;
                        
                        
        //            }
        //        }
               
        //        if (dt.Rows.Count > 0)
        //        {
                  
        //            if (searchText != "")
        //            {
        //                if (Session["TotalSearchRecords"] != null)
        //                {
        //                    totalrecords = (int)Session["TotalSearchRecords"];
        //                }

        //            }
        //            else
        //            {
                       
        //                totalrecords = dt.Rows.Count;
        //            }
        //            string SortBy = Request.QueryString["SortBy"];
        //            if (!string.IsNullOrEmpty(SortBy))
        //            {
        //                dt.DefaultView.Sort = SortBy.Split(',')[0] + " " + SortBy.Split(',')[1];
        //                dt = dt.DefaultView.ToTable();
        //            }

        //            if (searchText != "")
        //            {
        //                start = 0;
        //            }                   

        //            dt = dt.Rows.Cast<System.Data.DataRow>().Skip(start * limit).Take(limit).CopyToDataTable();
                                       
        //            dt.TableName = "Table";
        //            dt.WriteXml(sw);
        //            xml = sw.ToString();

        //            xml = xml.Replace("<DocumentElement>", "<DocumentElement><totalCount>" + totalrecords + "</totalCount>");
        //        }
        //        //else if (totalrecords > 6000)
        //        //    xml = "<DocumentElement><message>Too many records requested.Narrow down search criterion.</message></DocumentElement>";
        //        if (xml == "")
        //        {
        //            return;
        //        }
        //        xml = xml.Trim();
        //        Response.Write(xml.Trim());
            
        //}
        //    catch (Exception Ex)
        //    {
        //        string xml = "<DocumentElement><message>" + Ex.Message + "</message></DocumentElement>";
        //        xml = xml.Trim();
        //        Response.Write(xml.Trim());
        //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString() + " From: GetEvents"));
        //    }
        //}

        private void shareDatatable()
        {
            try
            {
                DataSet DSGlobal = new DataSet();
                //Session["EVflag"] = 0;
                string cacheId = Convert.ToString(Session["cacheId"]);
                //lock (_object)
                {
                    DSGlobal = (DataSet)Cache[cacheId];
                }
                if (DSGlobal != null)
                {
                    if (DSGlobal.Tables[0].Rows.Count > 0)
                    {

                        DataTable dt = new DataTable();
                        dt = ((DataSet)Cache[cacheId]).Tables[0];


                        string tableType = Request["tableType"];
                        string headerChecked = Request["HeaderChecked"];
                        string selected = Request["selected"];
                        string SortBy = Request["SortBy"];
                        string SearchBy = Request["SearchBy"];
                        string searchIn = Request["searchIn"];
                        string action = Request["action"];
                        string format = Request["format"];
                        string columnsList = Request["columnsList"];

                        //if (tableType == "Event")
                        //{
                        //    string s1= sn.User.EventColumns.ToString();
                        //    string[] arr = s1.Split(',');
                        //    foreach (var a in arr)
                        //    {
                        //        dt.Columns.Add(a);
                        //    }
                        //   // dt = newTable.DefaultView.ToTable(false, "ColumnName1", "ColimnName2", "ColimnName3", "ColimnName4", "ColimnName5"); 
                        //}
                        //else
                        //{
                        //    string s1 = sn.User.ViolationColumns.ToString();
                        //    string[] arr = s1.Split(',');
                        //    foreach (var a in arr)
                        //    {
                        //        dt.Columns.Add(a);
                        //    }
                        //}

                        if (!String.IsNullOrEmpty(SearchBy))
                        {
                            if (!String.IsNullOrEmpty(searchIn))
                            {
                                //if (searchIn.Contains("Details"))
                                //    searchIn += "MakeName,ModelName,LicensePlate";
                                DataTable dtc = new DataTable();
                                foreach (DataColumn dc in dt.Columns)
                                {
                                    dtc.Columns.Add(dc.ColumnName.ToString(), dc.DataType);
                                }
                                foreach (string s in searchIn.Split(','))
                                {
                                    if (String.IsNullOrEmpty(s))
                                        continue;
                                    string colname = s;
                                    if (dt.Columns.Contains(s))
                                    {
                                        int n;
                                        if (dt.Columns[s].DataType.Name.Contains("Int") && int.TryParse(SearchBy, out n))
                                        {

                                            if (dt.Select(string.Format("{0} = {1}", colname, SearchBy)).Length > 0)
                                                dtc.Merge(dt.Select(string.Format("{0} = {1}", colname, SearchBy)).CopyToDataTable());
                                        }
                                        else if (dt.Columns[s].DataType.Name.Contains("String"))
                                        {
                                            if (dt.Select(string.Format("{0} LIKE '%{1}%'", colname, SearchBy)).Length > 0)
                                                dtc.Merge(dt.Select(string.Format("{0} LIKE '%{1}%'", colname, SearchBy)).CopyToDataTable());
                                        }
                                    }
                                }
                                dt = dtc.DefaultView.ToTable(true);
                            }
                        }
                        DataTable sortedDT = new DataTable();
                        if (!String.IsNullOrEmpty(SortBy) && !SortBy.Contains("undefined"))
                        {
                            // dt.DefaultView.Sort = SortBy;
                            DataView dv = dt.DefaultView;
                            dv.Sort = SortBy;
                            sortedDT = dv.ToTable();
                            dt = sortedDT.Copy();

                        }


                        if (headerChecked != "1")
                        {
                            if (!String.IsNullOrEmpty(selected))
                            {
                                string[] s = selected.Split(',');
                                dt = dt.AsEnumerable().Where((r, i) => s.Contains(i.ToString())).CopyToDataTable();
                            }
                        }
                        if (action == "Export")
                            exportDataTable(dt, format, columnsList);
                        else if (action == "Email")
                        {
                            string address = Request["address"];
                            if (!String.IsNullOrEmpty(address))
                                sendEmail(dt, address, format, columnsList);
                        }
                        else if (action == "Print")
                            printDataTable(dt, columnsList);
                    }
                    else
                    {
                        //Session["EVflag"] = 1;
                        return;

                    }
                }
                //Session["EVflag"] = 1;
            }
            catch (Exception Ex)
            {
                //Session["EVflag"] = 1;
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                return;
            }
        }

        private void exportDataTable(DataTable dt, string format, string columnsList)
        {
            if (String.IsNullOrEmpty(format))
                format = "CSV";
            try
            {
                if (format == "CSV")
                {
                    string filepath = createFile(dt, format, columnsList);
                    Response.Clear();
                    Response.ContentType = "application/csv";
                    Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.csv", "Events"));
                    Response.TransmitFile(filepath);
                    Response.Flush();
                }
                else if (format == "Excel2003")
                {
                    string filepath = createFile(dt, format, columnsList);
                    HSSFWorkbook hssfwb;
                    using (var file = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        hssfwb = new HSSFWorkbook(file);
                    }
                    MemoryStream fs = new MemoryStream();
                    hssfwb.Write(fs);
                    Response.Clear();
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.xls", "Events"));
                    Response.BinaryWrite(fs.GetBuffer());
                    fs.Close();
                    Response.End();
                }
                else if (format == "Excel2007")
                {
                    string filepath = createFile(dt, format, columnsList);
                    Response.Clear();
                    Response.AddHeader("Content-Type", "application/Excel");
                    Response.ContentType = "application/force-download";
                    Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xlsx", "Events"));
                    Response.TransmitFile(filepath);
                }
                else if (format == "PDF")
                {
                    string filepath = createFile(dt, format, columnsList);
                    Response.Clear();
                    Response.AddHeader("Content-Type", "application/pdf");
                    Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.pdf", "Events"));
                    Response.TransmitFile(filepath);
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception Ex)
            {
                
                return;
            }
        }

        private string createFile(DataTable dt, string format, string columnsList)
        {
            string strFromDate = (String)Session["strFromDate"];
            string strToDate = (String)Session["strToDate"];
            string FleetIds = (String)Session["FleetIds"];
            string operation = (String)Session["operation"];
            if (Session["FleetDataset"] == null)
            {
                Session["FleetDataset"] = getFleetSupport();
            }
            string filepath = Server.MapPath("TempReports/");
            string filename = "";
            try
            {
                if (format == "CSV")
                {
                    filename = string.Format(@"{0}.csv", Guid.NewGuid());
                    StringBuilder sresult = new StringBuilder();
                    sresult.Append("sep=,");
                    sresult.Append(Environment.NewLine);
                    string header = string.Empty;
                    foreach (string column in columnsList.Split(','))
                    {
                        string s = column.Split(':')[0];
                        var r = new Regex(@"(?<=[a-z])(?=[A-Z])", RegexOptions.IgnorePatternWhitespace);
                        s = r.Replace(s, " ");
                        header += "\"" + s + "\",";
                    }
                    header = header.Substring(0, header.Length - 1);
                    sresult.Append(header);
                    sresult.Append(Environment.NewLine);
                    foreach (DataRow row in dt.Rows)
                    {
                        string data = string.Empty;
                        foreach (string column in columnsList.Split(','))
                        {
                            string s = row[column.Split(':')[1]].ToString();
                            if (column.Split(':')[1] == "StDate")
                                s = Convert.ToDateTime(s).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                            else if (column.Split(':')[0] == "Details")
                                s = row["MakeName"].ToString() + "/" + row["ModelName"].ToString() + " " + row[column.Split(':')[1]].ToString() + " " + row["VehicleDescription"].ToString();
                            data += "\"" + s.Replace("[br]", Environment.NewLine).Replace("\"", "\"\"") + "\",";
                        }
                        data = data.Substring(0, data.Length - 1);
                        sresult.Append(data);
                        sresult.Append(Environment.NewLine);
                    }
                    try
                    {
                        var files = new DirectoryInfo(Server.MapPath("TempReports/")).GetFiles("*.csv");
                        foreach (var file in files)
                        {
                            if (DateTime.UtcNow - file.LastWriteTimeUtc > TimeSpan.FromDays(30))
                            {
                                File.Delete(file.FullName);
                            }
                        }
                    }
                    catch { }
                    File.WriteAllText(filepath + filename, sresult.ToString());
                }
                else if (format == "Excel2003")
                {
                    filename = string.Format(@"{0}.xls", Guid.NewGuid());
                    HSSFWorkbook wb = new HSSFWorkbook();
                    ISheet ws = wb.CreateSheet("Sheet1");
                    ICellStyle cellstyle1 = wb.CreateCellStyle();
                    ICellStyle cellstyle2 = wb.CreateCellStyle();
                    cellstyle1.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    HSSFPalette palette = wb.GetCustomPalette();
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.INDIGO.index, (byte)83, (byte)141, (byte)213);
                    cellstyle1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.INDIGO.index;
                    cellstyle2.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
                    cellstyle2.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.INDIGO.index;

                    IRow row1 = ws.CreateRow(0);
                    ICellStyle styleSubHeader = wb.CreateCellStyle();
                    styleSubHeader.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
                    ICell r1c1 = row1.CreateCell(0);
                    r1c1.CellStyle = styleSubHeader;
                    if (FleetIds.Split(',').Length == 3)
                        r1c1.SetCellValue("Event/Violation" + System.Environment.NewLine + "From:              " + strFromDate + System.Environment.NewLine + "To:                  " + strToDate + System.Environment.NewLine + "Fleet:             " + ((DataSet)Session["FleetDataset"]).Tables[0].Select("FleetId=" + FleetIds.Split(',')[1])[0]["FleetName"].ToString() + System.Environment.NewLine + "Operation:      " + operation);
                    else
                        r1c1.SetCellValue("Event/Violation" + System.Environment.NewLine + "From:              " + strFromDate + System.Environment.NewLine + "To:                  " + strToDate + System.Environment.NewLine + "Fleet:             MultiFleets" + System.Environment.NewLine + "Operation: " + operation);
                    r1c1.CellStyle.WrapText = true;
                    r1c1.Row.Height = 800;
                    NPOI.SS.Util.CellRangeAddress cra = new NPOI.SS.Util.CellRangeAddress(0, 5, 0, 2);
                    ws.AddMergedRegion(cra);

                    IRow row = ws.CreateRow(7);
                    row.RowStyle = wb.CreateCellStyle();
                    row.RowStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
                    row.RowStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    row.RowStyle.WrapText = true;
                    IFont font = wb.CreateFont();
                    font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;
                    cellstyle1.SetFont(font);
                    row.RowStyle.SetFont(font);

                    foreach (string column in columnsList.Split(','))
                    {
                        string s = column.Split(':')[0];
                        var r = new Regex(@"(?<=[a-z])(?=[A-Z])", RegexOptions.IgnorePatternWhitespace);
                        s = r.Replace(s, " ");
                        row.CreateCell(row.Cells.Count).SetCellValue(s);
                        row.Cells[row.Cells.Count - 1].CellStyle = cellstyle1;
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string data = string.Empty;
                        IRow rowData = ws.CreateRow(i + 9);
                        foreach (string column in columnsList.Split(','))
                        {
                            if (column.Split(':')[1] == "StDate")
                            {
                                string datadate = Convert.ToDateTime(dt.Rows[i][column.Split(':')[1]].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                                rowData.CreateCell(rowData.Cells.Count).SetCellValue(datadate.Replace("[br]", Environment.NewLine));
                            }
                            else if (column.Split(':')[0] == "Details")
                                rowData.CreateCell(rowData.Cells.Count).SetCellValue((dt.Rows[i]["MakeName"].ToString() + "/" + dt.Rows[i]["ModelName"].ToString() + " " + dt.Rows[i][column.Split(':')[1]].ToString() + " " + dt.Rows[i]["VehicleDescription"].ToString()).Replace("[br]", Environment.NewLine));
                            else
                                rowData.CreateCell(rowData.Cells.Count).SetCellValue(dt.Rows[i][column.Split(':')[1]].ToString().Replace("[br]", Environment.NewLine));
                            rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle2;
                        }
                    }
                    try
                    {
                        var files = new DirectoryInfo(Server.MapPath("TempReports/")).GetFiles("*.xls");
                        foreach (var file in files)
                        {
                            if (DateTime.UtcNow - file.LastWriteTimeUtc > TimeSpan.FromDays(30))
                            {
                                File.Delete(file.FullName);
                            }
                        }
                    }
                    catch { }
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        try
                        {
                            ws.AutoSizeColumn(i);
                        }
                        catch { }
                    }
                    var fileData = new FileStream(filepath + filename, FileMode.Create);
                    wb.Write(fileData);
                    fileData.Close();

                }
                else if (format == "Excel2007")
                {
                    filename = string.Format(@"{0}.xlsx", Guid.NewGuid());
                    XLWorkbook wb = new XLWorkbook();
                    IXLWorksheet ws = wb.Worksheets.Add("Events");
                    ws.Columns().AdjustToContents();
                    if (FleetIds.Split(',').Length == 3)
                        ws.Cell("A1").RichText.AddText("Event/Violation" + System.Environment.NewLine + "From:            " + strFromDate + System.Environment.NewLine + "To:                  " + strToDate + System.Environment.NewLine + "Fleet:            " + ((DataSet)Session["FleetDataset"]).Tables[0].Select("FleetId=" + FleetIds.Split(',')[1])[0]["FleetName"].ToString() + System.Environment.NewLine + "Operation: " + operation);
                    else
                        ws.Cell("A1").RichText.AddText("Event/Violation" + System.Environment.NewLine + "From:            " + strFromDate + System.Environment.NewLine + "To:                  " + strToDate + System.Environment.NewLine + "Fleet:            MultiFleets" + System.Environment.NewLine + "Operation: " + operation);
                    ws.Cell("A1").RichText.Substring(0, 15).SetFontSize(20);
                    ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Cell("A1").Style.Alignment.SetWrapText();
                    ws.Range("A1:C6").Merge();                    
                    ws.Row(8).Style.Font.Bold = true;
                    foreach (string column in columnsList.Split(','))
                    {
                        string s = column.Split(':')[0];
                        var r = new Regex(@"(?<=[a-z])(?=[A-Z])", RegexOptions.IgnorePatternWhitespace);
                        s = r.Replace(s, " ");
                        ws.Cell(8, ws.Row(8).CellsUsed().Count() + 1).Value = s;
                        ws.Cell(8, ws.Row(8).CellsUsed().Count()).Style.Fill.BackgroundColor = XLColor.FromHtml("#538DD5");
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string data = string.Empty;
                        int iColumn = 1;
                        foreach (string column in columnsList.Split(','))
                        {
                            ws.Cell(i + 10, iColumn).DataType = XLCellValues.Text;
                            if (column.Split(':')[1] == "StDate")
                            {
                               string datadate = Convert.ToDateTime(dt.Rows[i][column.Split(':')[1]].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                                ws.Cell(i + 10, iColumn).Value = "'" + datadate.Replace("[br]", Environment.NewLine);
                            }
                            else if (column.Split(':')[0] == "Details")
                                 ws.Cell(i + 10, iColumn).Value = dt.Rows[i]["MakeName"].ToString() + "/" + dt.Rows[i]["ModelName"].ToString() + " " + dt.Rows[i][column.Split(':')[1]].ToString() + " " + dt.Rows[i]["VehicleDescription"].ToString();
                            else
                                ws.Cell(i + 10, iColumn).Value = "'" + dt.Rows[i][column.Split(':')[1]].ToString().Replace("[br]", Environment.NewLine);
                            ws.Cell(i + 10, iColumn).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i + 10, iColumn).Style.Border.BottomBorderColor = XLColor.FromHtml("#538DD5");
                            iColumn++;
                        }
                    }
                    ws.Columns().AdjustToContents();
                    try
                    {
                        var files = new DirectoryInfo(Server.MapPath("TempReports/")).GetFiles("*.xlsx");
                        foreach (var file in files)
                        {
                            if (DateTime.UtcNow - file.LastWriteTimeUtc > TimeSpan.FromDays(30))
                            {
                                File.Delete(file.FullName);
                            }
                        }
                    }
                    catch { }
                    wb.SaveAs(filepath + filename);
                }
                else if (format == "PDF")
                {
                    filename = string.Format(@"{0}.PDF", Guid.NewGuid());
                    try
                    {
                        var files = new DirectoryInfo(Server.MapPath("TempReports/")).GetFiles("*.pdf");
                        foreach (var file in files)
                        {
                            if (DateTime.UtcNow - file.LastWriteTimeUtc > TimeSpan.FromDays(30))
                            {
                                File.Delete(file.FullName);
                            }
                        }
                    }
                    catch { }

                    try
                    {
                        Document pdfDoc = new Document(new Rectangle(288f, 144f), 10, 10, 10, 10);
                        pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                        PdfWriter.GetInstance(pdfDoc, new FileStream(filepath + filename, FileMode.Create));
                        pdfDoc.Open();
                        Chunk c = new Chunk("", FontFactory.GetFont("Verdana", 11));
                        Font font8 = FontFactory.GetFont("ARIAL", 7);
                        Font fontBold = FontFactory.GetFont("ARIAL", 7, Font.BOLD);
                        var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
                        var boldTableFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                        pdfDoc.Add(new Paragraph("Event/Violation", titleFont));
                        
                        var DetailTable = new PdfPTable(2);
                        DetailTable.HorizontalAlignment = 0;
                        DetailTable.SpacingBefore = 10;
                        DetailTable.SpacingAfter = 10;
                        DetailTable.DefaultCell.Border = 0;
                        DetailTable.AddCell(new Phrase("From:", boldTableFont));
                        DetailTable.AddCell(strFromDate);
                        DetailTable.AddCell(new Phrase("To:", boldTableFont));
                        DetailTable.AddCell(strToDate);
                        DetailTable.AddCell(new Phrase("Fleet:", boldTableFont));
                        if (FleetIds.Split(',').Length == 3)
                        {
                            
                            DetailTable.AddCell(((DataSet)Session["FleetDataset"]).Tables[0].Select("FleetId=" + FleetIds.Split(',')[1])[0]["FleetName"].ToString());
                        }
                        else
                        {
                            DetailTable.AddCell("MultiFleets");
                        }
                        DetailTable.AddCell(new Phrase("Operation:", boldTableFont));
                        DetailTable.AddCell(operation);
                        pdfDoc.Add(DetailTable);
                        
                        if (dt != null)
                        {
                            PdfPTable PdfTable = new PdfPTable(columnsList.Split(',').Length);
                            PdfTable.WidthPercentage = 100;
                            PdfTable.DefaultCell.Border = Rectangle.NO_BORDER;
                            PdfTable.SplitRows = true;
                            PdfPCell PdfPCell = null;
                            foreach (string column in columnsList.Split(','))
                            {
                                string s = column.Split(':')[0];
                                var r = new Regex(@"(?<=[a-z])(?=[A-Z])", RegexOptions.IgnorePatternWhitespace);
                                s = r.Replace(s, " ");
                                PdfPCell = new PdfPCell(new Phrase(new Chunk(s, fontBold)));
                                PdfPCell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#538DD5"));
                                PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                PdfPCell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                                PdfTable.AddCell(PdfPCell);
                            }
                            for (int rows = 0; rows < dt.Rows.Count; rows++)
                            {
                                foreach (string column in columnsList.Split(','))
                                {
                                    if (column.Split(':')[0] == "Details")
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows]["MakeName"].ToString() + "/" + dt.Rows[rows]["ModelName"].ToString() + " " + dt.Rows[rows][column.Split(':')[1]].ToString() + " " + dt.Rows[rows]["VehicleDescription"].ToString(), font8)));
                                    else
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column.Split(':')[1]].ToString(), font8)));
                                    PdfPCell.BorderColorBottom = new Color(System.Drawing.ColorTranslator.FromHtml("#538DD5"));
                                    if (rows % 2 != 0)
                                    {
                                        PdfPCell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#C5D9F1"));
                                    }
                                    PdfTable.AddCell(PdfPCell);
                                }
                            }
                            pdfDoc.Add(PdfTable);
                        }
                        pdfDoc.Close();
                    }
                    catch (DocumentException de)
                    {
                        System.Web.HttpContext.Current.Response.Write(de.Message);
                    }
                    catch (IOException ioEx)
                    {
                        System.Web.HttpContext.Current.Response.Write(ioEx.Message);
                    }
                    catch (Exception ex)
                    {
                        System.Web.HttpContext.Current.Response.Write(ex.Message);
                    }
                }

            }
            catch (Exception ex) 
            { 
                System.Web.HttpContext.Current.Response.Write(ex.Message); 
            }
            bool fileCreated = false;
            while (!fileCreated)
            {
                try
                {
                    using (FileStream inputStream = File.Open(filepath + filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        fileCreated = true;
                    }
                }
                catch (Exception)
                {
                    fileCreated = false;
                }
            }
            return filepath + filename;
        }

        private void sendEmail(DataTable dt, string address, string format, string columnsList)
        {
            try
            {
                //creating Attachement
                string filePath = createFile(dt, format, columnsList);
                SmtpClient mailClient;
                // server
                string server = "";
                try
                {
                    server = ConfigurationManager.AppSettings["SMTPServer"].ToString();
                }
                catch
                {
                    server = "localhost";
                }
                if (String.IsNullOrEmpty(server))
                    server = "localhost";
                mailClient = new SmtpClient(server);
                // timeout
                int timeout = 120000;
                try
                {
                    timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPTimeout"]) * 60000;
                }
                catch
                {
                    timeout = 120000;
                }
                mailClient.Timeout = timeout;
                // credentials
                string username = ConfigurationManager.AppSettings["UserName"];
                string password = ConfigurationManager.AppSettings["pwd"];

                if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                    mailClient.UseDefaultCredentials = true;
                else
                {
                    try
                    {
                        NetworkCredential newCredential = new System.Net.NetworkCredential(username, password, server);
                        mailClient.UseDefaultCredentials = false;
                        mailClient.Credentials = newCredential;
                    }
                    catch
                    {
                        mailClient.UseDefaultCredentials = true;
                    }
                }
                MailMessage message = new MailMessage();
                MailAddress fromMail = new MailAddress("noreply@bsmwireless.com");
                message.From = fromMail;
                message.To.Add(address);
                message.Subject = "SentinelFM events report";
                message.Body = "This is an auto genreated mail. Please do not reply.";
                if (filePath != null)
                    message.Attachments.Add(new Attachment(filePath));
                mailClient.Send(message);
                if (message != null) message.Dispose();
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes("Send Succcessfuly...");
                Response.Clear();
                Response.OutputStream.Write(bytes, 0, bytes.Length);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest(); 
            }
            catch (SmtpFailedRecipientException sfre)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes("Error delivering e-mail to " + address + "->" + sfre.Message);
                Response.Clear();
                Response.OutputStream.Write(bytes, 0, bytes.Length);
                Response.End(); 
            }
            catch (SmtpException e)
            {
                byte[] bytes;
                switch (e.StatusCode)
                {
                    case SmtpStatusCode.ExceededStorageAllocation:
                        bytes = System.Text.Encoding.UTF8.GetBytes("Mail Attachment file size exceeded its limit");
                        Response.Clear();
                        Response.OutputStream.Write(bytes, 0, bytes.Length);
                        Response.End(); 
                        break;
                    default:
                        bytes = System.Text.Encoding.UTF8.GetBytes(e.Message + e.InnerException.Message);
                        Response.Clear();
                        Response.OutputStream.Write(bytes, 0, bytes.Length);
                        Response.End(); 
                        break;
                }
            }
            catch (FormatException fe)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes("Error creating message " + fe.Message);
                Response.Clear();
                Response.OutputStream.Write(bytes, 0, bytes.Length);
                Response.End();
            }
            catch (ArgumentException ae)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes("Error creating message  " + ae.Message);
                Response.Clear();
                Response.OutputStream.Write(bytes, 0, bytes.Length);
                Response.End();
            }
            catch (Exception e)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes("Error creating message " + e.Message);
                Response.Clear();
                Response.OutputStream.Write(bytes, 0, bytes.Length);
                Response.End();
            }
        }

        private void printDataTable(DataTable dt, string columnsList)
        {
            try
            {
                string filepath = createFile(dt, "PDF", columnsList);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(Path.GetFileName(filepath));
                Response.Clear();
                Response.OutputStream.Write(bytes, 0, bytes.Length); 
            }
            catch (Exception Ex)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes("Failed to print, please try it again...");
                Response.Clear();
                Response.OutputStream.Write(bytes, 0, bytes.Length);
                Response.End(); 
            }
        }

        //Changes
        private void SetDefaultColumnPreferences()
        {
            try
            {
                int preferenceId;
                string sSelectedColumnPreference = Request["SelectedColumnPreference"].ToString();
                string sOperation = Request["Operation"].ToString();
                

                if (sOperation == "Event")
                {
                    preferenceId = 49;
                    sn.User.EventColumns = sSelectedColumnPreference;
                    
                }
                else
                {
                    preferenceId = 50;
                    sn.User.ViolationColumns = sSelectedColumnPreference;
                    
                }

                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                EvtEvents eevent = new EvtEvents(sConnectionString);
                int RowsAffected = eevent.SetDefaultColumnPreference(Convert.ToInt32(sn.UserID), sSelectedColumnPreference, preferenceId);

                if (RowsAffected == 0)
                {
                    return;
                }
            }
            catch (Exception ex)
            { }
        }
        //Changes
        private void scheduleReport()
        {
           
            try
            {
                string sVisibleColumns = Request["VisibleColumns"].ToString();
                string events = Request["SchEventIds"].ToString();
                string sFreqType = Request["FreqType"].ToString();
                string sSchedStart = Request["SchedStart"].ToString();
                string sSchedEnd = Request["SchedEnd"].ToString();
                string sFreqParamM = Request["FreqParamM"].ToString();
                string sFreqParamW = Request["FreqParamW"].ToString();
                string sEmail = Request["Email"].ToString();
                string sDelivery = Request["DeliveryType"].ToString();
                string sFromDate = Request["FromDate"].ToString();
                string sToDate = Request["ToDate"].ToString();
                string sFleetId = Request["FleetIds"].ToString();
                string sExportFormat = Request["ExportFormat"].ToString();  
                string sEventCategory = Request["EventCategory"].ToString();
                string SchedStartTime = Request["SchedStartTime"].ToString();
                var len = sEmail.Length;
                
                Int16 freqType = Convert.ToInt16(sFreqType);
                Int16 freqParam = 0;
                DateTime SchedStart = new DateTime();
                DateTime SchedEnd = new DateTime();
                DateTime schFromDate = new DateTime();
                DateTime schToDate = new DateTime();
                DateTime schFromDateTime = new DateTime();
               
                schFromDate = DateTime.ParseExact(sFromDate, sn.User.DateFormat + " " + sn.User.TimeFormat, CultureInfo.InvariantCulture);
                schToDate = DateTime.ParseExact(sToDate, sn.User.DateFormat + " " + sn.User.TimeFormat, CultureInfo.InvariantCulture);

                //setting  schFromDateTime1 default to 00:00 as scheduled time feature was removed

                //schFromDateTime = DateTime.ParseExact(SchedStartTime, sn.User.TimeFormat, CultureInfo.CurrentCulture);
                //string schFromDateTime1 = schFromDateTime.ToString("HH:mm");

                
                string schFromDateTime1 = "00:00";
                
                if (!String.IsNullOrEmpty(sSchedStart))
                    SchedStart = DateTime.ParseExact(sSchedStart, sn.User.DateFormat, CultureInfo.CurrentCulture);
                else
                    return;
                if (freqType == 0) // once
                    SchedEnd = SchedStart;
                else
                {
                    if (!String.IsNullOrEmpty(sSchedStart))
                        SchedEnd = DateTime.ParseExact(sSchedEnd, sn.User.DateFormat, CultureInfo.InvariantCulture);
                    else
                        return;
                    if (Convert.ToDateTime(schFromDate) > Convert.ToDateTime(schToDate))
                        return;
                }
                
                if (freqType == 2)
                {
                    freqParam = Convert.ToInt16(GetDayOfWeek(sFreqParamW));
                    if ((int)SchedStart.DayOfWeek < freqParam)
                        SchedStart.AddDays(freqParam - (int)SchedStart.DayOfWeek);
                    else
                        SchedStart.AddDays(7 - (int)SchedStart.DayOfWeek + freqParam);
                }
                else
                    if (freqType == 3)
                    {
                        freqParam = Convert.ToInt16(sFreqParamM);
                        if ((int)SchedStart.Day < freqParam)
                            SchedStart.AddDays(freqParam - (int)SchedStart.Day);
                        else
                            SchedStart.AddDays(DateTime.DaysInMonth(SchedStart.Year, SchedStart.Month) - (int)SchedStart.Day + freqParam);
                    }

               
                sn.Report.GuiId = 10092;

                string xmlParams = "{";
                xmlParams += "reportid: " + sn.Report.GuiId + ", ";
                xmlParams += "reportformat: " + sExportFormat + ", ";
                xmlParams += "reportformatcode: " + sn.Report.ReportFormat + ", ";
                xmlParams += "userid: " + sn.UserID + ", ";
                xmlParams += "organization: " + sn.User.OrganizationId + ", ";
                xmlParams += "datefrom: " + DateTime.ParseExact(sFromDate, sn.User.DateFormat + " " + sn.User.TimeFormat, CultureInfo.InvariantCulture) + ", ";
                xmlParams += "dateto: " + DateTime.ParseExact(sToDate, sn.User.DateFormat + " " + sn.User.TimeFormat, CultureInfo.InvariantCulture) + ", ";
                xmlParams += "fleetid: " + sFleetId + ", ";
                xmlParams += "eventid: " + events + ", ";               
                //xmlParams += "vehicleId:" + vehicleId + ",";
                xmlParams += "operation: " + sEventCategory + ", ";
                if (events.Contains("-2"))
                    xmlParams += "noofdays: " + NoOfDays + ", ";
                else
                    xmlParams += "noofdays: " + 0 + ", ";
                xmlParams += "scheduleDateTime: " + schFromDateTime1 + ", ";
                xmlParams += "visiblecolumns: " + sVisibleColumns + ", ";
                
                xmlParams += "}";

                clsXmlUtil xmlDoc = new clsXmlUtil();
                xmlDoc.CreateRoot("Schedule");
                xmlDoc.CreateNode("XmlParams", xmlParams);
                xmlDoc.CreateNode("IsFleet", false);
                xmlDoc.CreateNode("FleetId", 0);
                xmlDoc.CreateNode("Email", sEmail);
                xmlDoc.CreateNode("GuiId", sn.Report.GuiId);
                xmlDoc.CreateNode("Status", "New");
                xmlDoc.CreateNode("StatusDate", DateTime.Now);
                xmlDoc.CreateNode("Frequency", freqType);
                xmlDoc.CreateNode("FrequencyParam", freqParam);
                xmlDoc.CreateNode("StartDate", SchedStart);
                xmlDoc.CreateNode("EndDate", SchedEnd);
                xmlDoc.CreateNode("DeliveryMethod", Convert.ToInt16(sDelivery));
                xmlDoc.CreateNode("ReportLanguage", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                xmlDoc.CreateNode("ReportFormat", sn.Report.ReportFormat); // new 2008 - 05 - 05

                if (String.IsNullOrEmpty(xmlDoc.Xml))
                {
                    return;
                }

                xmlDoc.CreateNode("FromDate", DateTime.ParseExact(sFromDate, sn.User.DateFormat + " " + sn.User.TimeFormat, CultureInfo.InvariantCulture));
                xmlDoc.CreateNode("ToDate", DateTime.ParseExact(sToDate, sn.User.DateFormat + " " + sn.User.TimeFormat, CultureInfo.InvariantCulture));

                ServerReports.Reports reportProxy = new ServerReports.Reports();
                if (objUtil.ErrCheck(reportProxy.AddReportSchedule(sn.UserID, sn.SecId, xmlDoc.Xml), false))
                    if (objUtil.ErrCheck(reportProxy.AddReportSchedule(sn.UserID, sn.SecId, xmlDoc.Xml), true))
                    {
                        return;
                    }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                   Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmReportScheduler.aspx->cmdSubmit_Click"));              
                
            }
         
            
        }

        public int GetDayOfWeek(string day)
        {
            int DayNumber = 0;
            switch (day)
            {
                case "Monday": 
                    DayNumber = 1;
                    break;
                case "Tuesday":
                    DayNumber = 2;
                    break;
                case "Wednesday":
                    DayNumber = 3;
                    break;
                case "Thursday":
                    DayNumber = 4;
                    break;
                case "Friday":
                    DayNumber = 5;
                    break;
                case "Saturday":
                    DayNumber = 6;
                    break;
                case "Sunday":
                    DayNumber = 7;
                    break;

            }
            return DayNumber;
        }

        public void GetDataEvents()
        {                        
            string events = (string)Session["events"];
            string vehicleId = (string)Session["vehicleId"];
            DateTime dtsd = DateTime.Now;
            DateTime dttd = DateTime.Now;
            DataSet DSGlobal = new DataSet();
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.Default;
            string xml = "";
            if (DSGlobal.Tables.Count > 0 && DSGlobal.Tables[0].Rows.Count > 0)
               {
                int box = 0;
                int currentPage;
            int totalrecords = 0;

                               
               if (DSGlobal != null)
                {
                  currentPage = ((DSGlobal.Tables[0].Rows.Count - boxDataCount) / limit) + 1;
                box = 0;
                DataSet dataSet = new DataSet();
                DataSet BoxDataSet = new DataSet();
                BoxDataSet = null;
                    dataSet = eeventBoxEvent.GetEvent(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, NoOfDays, sn.UserID, BoxDataSet, Convert.ToInt32(RecordsToFetch));
                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    DataTable _dt = new DataTable();
                    DSGlobal.Tables[0].Merge(dataSet.Tables[0]);
                    DSGlobal.AcceptChanges();                    
                            
                        }

                        ct = dataSet.Tables[0].Rows.Count;
                }
                  
            }
            xml = "<DocumentElement><message>" + ct.ToString() + "</message></DocumentElement>";
            //xml = ct.ToString();
            Response.Write(xml.Trim());            
            
        }

        public void GetDataViolation()
        {
            string events = (string)Session["events"];
            string vehicleId = (string)Session["vehicleId"];
            DateTime dtsd = DateTime.Now;
            DateTime dttd = DateTime.Now;
            DataSet DSGlobal = new DataSet();
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.Default;
            string xml = "0";
            if (DSGlobal.Tables.Count > 0 && DSGlobal.Tables[0].Rows.Count > 0)
        {
             int box = 0;
             int currentPage;
            int totalrecords = 0;
                                      
                 if (DSGlobal != null)
            {
                     currentPage = ((DSGlobal.Tables[0].Rows.Count - boxDataCount) / limit) + 1;
                box = 0;
                DataSet dataSet = new DataSet();
                DataSet BoxDataSet = new DataSet();
                BoxDataSet = null;
                dataSet = eeventForBuffer.GetViolation(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, NoOfDays, sn.UserID, BoxDataSet, Convert.ToInt32(RecordsToFetch));
                    if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                    {
                        DataTable _dt = new DataTable();
                        DSGlobal.Tables[0].Merge(dataSet.Tables[0]);
                        DSGlobal.AcceptChanges();
                        
                    }
                
                     ct = dataSet.Tables[0].Rows.Count;
                 }                                        
            }
            xml = "<DocumentElement><message>" + ct.ToString() + "</message></DocumentElement>";
           // xml = ct.ToString();
            Response.Write(xml.Trim());
        }

        
        public void GetDataEvents(string events, string vehicleId, DateTime dtsd, DateTime dttd, int OrganizationId, int currentPage, int limit, int box)
        {
            string sConnectionString = ConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;
            EvtEvents eevent = new EvtEvents(sConnectionString);          
            DataSet DSGlobal = new DataSet();
            string cacheId = Convert.ToString(Session["cacheId"]);          
            DSGlobal = (DataSet)Cache[cacheId];

            if (Convert.ToInt32(Session["Count"]) == Convert.ToInt32(RecordsToFetch))
            {
                box = 0;
                DataSet dataSet = new DataSet();
                DataSet BoxDataSet = new DataSet();
                BoxDataSet = null;
                currentPage = ((DSGlobal.Tables[0].Rows.Count - boxDataCount) / limit) + 1;

                dataSet = eevent.GetEvent(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, NoOfDays, sn.UserID, BoxDataSet, Convert.ToInt32(RecordsToFetch));
                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    DataTable _dt = new DataTable();
                    if (DSGlobal != null)
                    {
                        lock (DSGlobal)
                        {
                            if (dataSet != null)
                            {
                                DSGlobal.Tables[0].Merge(dataSet.Tables[0]);
                                DSGlobal.AcceptChanges();
                                {
                                    Cache.Insert(cacheId, DSGlobal, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
                                }
                            }

                        }
                    }
                }
                Session["count"] = dataSet.Tables[0].Rows.Count;
            }         
         
        }

       
        //public void GetDataEvents(string events, string vehicleId, DateTime dtsd, DateTime dttd, int OrganizationId, int currentPage, int limit, int box)
        //{
        //    string oldCacheId;
        //    int totalrecords = 0;
        //    string sConnectionString = ConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;
        //    EvtEvents eevent = new EvtEvents(sConnectionString);
        //    int count = 500;
        //    DataSet DSGlobal = new DataSet();
        //    string cacheId = Convert.ToString(Session["cacheId"]);
        //    //lock (_object)
        //    {
        //        DSGlobal = (DataSet)Cache[cacheId];
        //    }
        //    oldCacheId = Session["cacheId"].ToString();

        //    while (count == 500 && (int)Session["EVflag"] == 1 && DSGlobal != null && oldCacheId == Convert.ToString(Session["cacheId"]))
        //    {
                
        //            box = 0;
        //            DataSet dataSet = new DataSet();
        //        DataSet BoxDataSet = new DataSet();
        //        BoxDataSet =  null;
        //            currentPage = ((DSGlobal.Tables[0].Rows.Count - boxDataCount) / limit) + 1;

        //            dataSet = eevent.GetEvent(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, NoOfDays, sn.UserID, BoxDataSet);
        //            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
        //            {
        //                DataTable _dt = new DataTable();
        //                if (DSGlobal != null)
        //                {
        //                    lock (DSGlobal)
        //                    {
        //                    if (dataSet != null)
        //                    {
        //                        DSGlobal.Tables[0].Merge(dataSet.Tables[0]);
        //                        DSGlobal.AcceptChanges();
        //                        totalrecords = (DSGlobal.Tables[0].Rows.Count) / 100;
        //                        //lock (_object)
        //                        {
        //                            //Cache.Remove(cacheId);
        //                            Cache.Insert(cacheId, DSGlobal, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
        //                        }
        //                    }

        //                    }
        //                }
        //            }
        //            count = dataSet.Tables[0].Rows.Count;
                
        //    }

            
        //}

        public void GetDataViolation(string events, string vehicleId, DateTime dtsd, DateTime dttd, int OrganizationId, int currentPage, int limit, int box)
        {
            
            string sConnectionString = ConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;
            EvtEvents eevent2 = new EvtEvents(sConnectionString);
           
            DataSet DSGlobal = new DataSet();
            string cacheId = Convert.ToString(Session["cacheId"]);
           
                DSGlobal = (DataSet)Cache[cacheId];
                if (Convert.ToInt32(Session["Count"]) == Convert.ToInt32(RecordsToFetch))
                {
                box = 0;
                DataSet BoxDataSet = new DataSet();
                BoxDataSet = null;
                DataSet dataSet = new DataSet();
                currentPage = ((DSGlobal.Tables[0].Rows.Count - boxDataCount) / limit) + 1;

                dataSet = eevent2.GetViolation(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, NoOfDays, sn.UserID, BoxDataSet, Convert.ToInt32(RecordsToFetch));
                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    DataTable _dt = new DataTable();
                    if (DSGlobal != null)
                    {
                        lock (DSGlobal)
                        {
                            if (dataSet != null)
                            {
                            DSGlobal.Tables[0].Merge(dataSet.Tables[0]);
                            DSGlobal.AcceptChanges();
                               
                                {
                                    //Cache.Remove(cacheId);
                                    Cache.Insert(cacheId, DSGlobal, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
                                }
                            }
                        }
                    }
                }
               Session["count"] = dataSet.Tables[0].Rows.Count;
            } 

              
        }
        //public void GetDataViolation(string events, string vehicleId, DateTime dtsd, DateTime dttd, int OrganizationId, int currentPage, int limit, int box)
        //{

        //    string sConnectionString = ConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;
        //    EvtEvents eevent2 = new EvtEvents(sConnectionString);

        //    DataSet DSGlobal = new DataSet();
        //    string cacheId = Convert.ToString(Session["cacheId"]);

        //    DSGlobal = (DataSet)Cache[cacheId];


        //    box = 0;
        //    DataSet BoxDataSet = new DataSet();
        //    BoxDataSet = null;
        //    DataSet dataSet = new DataSet();
        //    currentPage = ((DSGlobal.Tables[0].Rows.Count - boxDataCount) / limit) + 1;

        //    dataSet = eevent2.GetViolation(events, vehicleId, dtsd, dttd, sn.User.OrganizationId, currentPage, limit, box, NoOfDays, sn.UserID, BoxDataSet);
        //    if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
        //    {
        //        DataTable _dt = new DataTable();
        //        if (DSGlobal != null)
        //        {
        //            lock (DSGlobal)
        //            {
        //                if (dataSet != null)
        //                {
        //                    DSGlobal.Tables[0].Merge(dataSet.Tables[0]);
        //                    DSGlobal.AcceptChanges();

        //                    {
        //                        //Cache.Remove(cacheId);
        //                        Cache.Insert(cacheId, DSGlobal, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
        //                    }
        //                }
        //            }
        //        }
        //    }


        //}

        protected void Page_Unload(object sender, EventArgs e)
        {
            //if (Session["Menu"] != null)
            //{
            //    if ((string)Session["Menu"] != "EventViewer.aspx")
            //    {
            //        Session["EVflag"] = 0;
            //    }
            //}
            
        }


    }

    
}

