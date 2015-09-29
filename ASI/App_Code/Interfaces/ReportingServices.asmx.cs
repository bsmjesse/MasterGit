
#region Namespace

// System Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
//using System.IO.Packaging; 
using System.IO.Compression;
//using System.Linq;
using System.Resources;
using System.Runtime.Remoting;
//using System.Runtime.Serialization.Json; 
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.UI;
using System.Xml.Serialization;

// Reporting service namespaces

using Microsoft.Reporting;
using Microsoft.Reporting.WebForms;

// BSM namespaces

using VLF.CLS.Interfaces;
using VLF.CLS.Def;
using VLF.DAS.Logic;
using VLF.Reports;

#endregion

namespace VLF.ASI.Interfaces {

    /// <summary>
    /// Summary description for ServerRpt
    /// Parameter should be passed as string in JSON Format
    /// Mandatory Parameters:
    /// reportid : <id>
    /// reportname : <name>
    /// reportpath : <path>
    /// reporturi: <uri>            // URL of Report Service
    /// reporttype: <type>          // SRS | SRX | CRS | CRX | ....
    /// reportformat: <format>      // PDF | XSL | WRD | XML | HML | ....
    /// username : <user name>      // Report Service Credentials login user, not application logon user 
    /// password : <password>
    /// domain: <domain>
    /// userid: <uid>               // application logono user id
    /// [optional parameters]
    /// </summary>
    [WebService(Namespace = "http://www.sentinelfm.com")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ReportingServices : System.Web.Services.WebService
    {
        #region private constants section
        
        const string CS_DTFMT_US = "MM/dd/yyyy";
        const string CS_DTFMT_CA = "dd/MM/yyyy";
        const string CS_DTFMT_GS = "yyyy/MM/dd";
        const string CS_TMFMT_12 = "hh:mm:ss.fff tt";
        const string CS_TMFMT_24 = "HH:mm:ss.fff";

        /* Configured connection string name */
        const string CS_CSN_ACTIVE = "ActiveStateConnectionString";
        const string CS_CSN_REPORT = "DBReportConnectionString";
        const string CS_CSN_DEFAULT = "DBConnectionString";

        /* Reporting Service configured authentication settings */
        // Refer to web.config AppSettings section for detail
        const string CD_ASK_DEFRSURL = "RS_DEF_URL";
        const string CS_ASK_DEFRSUID = "RS_DEF_UID";
        const string CS_ASK_DEFRSPWD = "RS_DEF_PWD";
        const string CS_ASK_DEFRSDMN = "RS_DEF_DMN";
        /* Reporting Service build-in authentication settings */
        const string CS_RS_USERNAME = "bsmreports";
        const string CS_RS_PASSWORD = "T0ybhARQ";
        const string CS_RS_DOMAIN = "production";

        const string CS_WS_SERVER = ">> Server.pro::";
        const char CC_DELIMITOR = '\t';

        enum LogIndex : int {
            Append = 0,
            Status = 1,
            FileLocation = 2,
            Error = 3,
            UserID = 4,
            ReportID = 5,
            ProcessID = 6
        }

        #endregion

        #region Private Variables Section

        string msReportCategory;
        string msReportPage;
        string msReportType;
        string msServerURL;
        string msReportPath;
        string msRSUsername;
        string msRSPassword;
        string msRSDomain;
        string msMessage;
        string msParameters;
        string msDateTimeFormat;
        string msUserID;
        string msReportID; 
        string msReportUri; 
        string msReportFormat;
        string msReportFormatCode;
        string msReportName;
        string msLanguage;
        string msDateFrom;
        string msDateTo;
        string msKeyValues;

        string msFleetName;
        string msFleetOrganization;
        string msFleetDescription;
        string msFleetType;
        string msFleetNodeCode;
        String msInitialTime;
        DateTime dtInitialTime = DateTime.Now;
        // Report Application Settings
        string ReportsRootPath = @ConfigurationManager.AppSettings["ReportsRootPath"];
        string ReportsOutputPath = @ConfigurationManager.AppSettings["ReportsOutputPath"];
        string ReportsDataSetPath = @ConfigurationManager.AppSettings["ReportsDataSetPath"];
        string ReportsOutputPathURL = @ConfigurationManager.AppSettings["ReportsOutputPathURL"];
        // Database Connection String
        string msActiveStateConnectionName = "ActiveStateConnectionString";
        string msReportServiceConnectionName = "DBReportConnectionString";
        string msDefaulDatabaseConnectionName = "DBConnectionString";
        // Reporting Service Credential
        string msCredentialUID = @ConfigurationManager.AppSettings[CS_ASK_DEFRSUID];
        string msCredentialPWD = @ConfigurationManager.AppSettings[CS_ASK_DEFRSPWD];
        string msCredentialDMN = @ConfigurationManager.AppSettings[CS_ASK_DEFRSDMN];
        string msReportService = @ConfigurationManager.AppSettings[CD_ASK_DEFRSURL];

        Int64 miRepositoryID;

        Int64 int64RepositoryNumber;

        DataSet moDataset = new DataSet();
        
        Dictionary<string, string> moParameters = new Dictionary<string, string>();

        #endregion

        #region Delegation Section

        public delegate void MyCallbackDelegate(int data);
        public delegate bool asyncRenderServerReport(string parameters, AsyncState state);
        
        #endregion

        #region Construction section

        public ReportingServices()
        {
            msMessage = "";
            msParameters = "";

            msRSUsername = msCredentialUID;
            msRSPassword = msCredentialPWD;
            msRSDomain = msCredentialDMN;
            msServerURL = msReportService;
        }

        public ReportingServices(string parameters)
        {
            msMessage = "";
            msParameters = parameters;
        }

        #endregion

        #region Public Web Methods Section

        /// <summary>
        /// Return process message
        /// </summary>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public string Message() {
            return msMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public bool ProcessMessage(ref string Message)
        {
            Message = msMessage;
            return true;
        }

        /// <summary>
        /// Render Server Report Synchronouse
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public bool RenderDirectReport(string parameters, ref string reportUrl)
        {
            msParameters = parameters;

            if (ReportingServiceLogInitial("Direct", parameters, out msInitialTime))             //out dtInitialTime))
            {
                // log status
                ReportingServiceLogUpdate(msInitialTime, "Preprocess", 1);

                if (RenderReportPreprocesses())
                {
                    // Log status
                    ReportingServiceLogUpdate(msInitialTime, "prerendering", 1);
                    // User ID
                    ReportingServiceLogUpdate(msInitialTime, msUserID, 4);
                    // Report ID
                    ReportingServiceLogUpdate(msInitialTime, msReportID, 5);
                    // Process id - Direct report is one time w/o process id (pages of Report master & Report Viewer) 
                    ReportingServiceLogUpdate(msInitialTime, "0", 6);

                    if (RenderServerReport("", ref reportUrl)) {
                        // File location
                        ReportingServiceLogUpdate(msInitialTime, reportUrl, 2);
                        msMessage = "";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(msMessage))
                            msMessage = "Render report[" + GetParameterKeyValue_String("reportid", "") +"] failed";
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(msMessage))
                        msMessage = "Preprocess failed";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(msMessage)) 
                    msMessage = "Log Initial failed." ;
            }

            if(string.IsNullOrEmpty(msMessage))
                return true;
            else
	        {
                ReportingServiceLogUpdate(msInitialTime, msMessage, 3);
                return false;
	        }
            

        }

        /// <summary>
        /// Render Server Report Synchronouse
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public bool RenderRepositoryReport(string parameters)
        {
            msParameters = parameters;
            msMessage = "";

            if (ReportingServiceLogInitial("Repository", parameters, out msInitialTime))
            {
                // Status
                ReportingServiceLogUpdate(msInitialTime, "Preprocess", 1);

                if (RenderReportPreprocesses())
                {
                    // Status
                    ReportingServiceLogUpdate(msInitialTime, "RepositoryID", 1);
                    // User ID
                    ReportingServiceLogUpdate(msInitialTime, msUserID, 4);
                    // Report ID
                    ReportingServiceLogUpdate(msInitialTime, msReportID, 5);

                    //KeyValues: FleetID, Date From, Date To, FormatId:  1=PDF, 2=Excel, 3=Word;
                    long RepositoryID = SaveReportRepositoryInfor(msUserID, msLanguage, msReportID, msDateFrom, msDateTo, msKeyValues, msReportFormatCode);

                    if (RepositoryID > 0)
                    {
                        msReportUri = "";
                        // Process id = Reporsitory ID
                        ReportingServiceLogUpdate(msInitialTime, RepositoryID.ToString(), 6);
                        // log status
                        ReportingServiceLogUpdate(msInitialTime, "Prerendering", 1);
                        // Render report
                        if (RenderServerReport(RepositoryID.ToString(), ref msReportUri))
                        {
                            // log file location
                            ReportingServiceLogUpdate(msInitialTime, msReportUri, 2);
                            msMessage = "";
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(msMessage))
                                msMessage = "Render report[" + GetParameterKeyValue_String("reportid", "") + "] failed";
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(msMessage))
                            msMessage = "Generate repository id failed";
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(msMessage))
                        msMessage = "Preprocess failed";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(msMessage))
                    msMessage = "Initial log failed.";
            }

            if (string.IsNullOrEmpty(msMessage))
                return true;
            else
            {
                ReportingServiceLogUpdate(msInitialTime, msMessage, 3);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="SecurityID"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="parameters"></param>
        /// <param name="reportFormat"></param>
        /// <param name="language"></param>
        /// <param name="reportUrl"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public bool RenderScheduleReport(int UserID, string SecurityID, string dateFrom, string dateTo, string parameters, int reportFormat, string language, ref string reportUrl)
        {
            msParameters = parameters;
            msMessage = "";
            // Create log recored
            if (ReportingServiceLogInitial("Schedule", parameters, out msInitialTime))
            {
                // log user id
                ReportingServiceLogUpdate(msInitialTime, UserID.ToString(), 4);
                // log Schedule ID
                ReportingServiceLogUpdate(msInitialTime, SecurityID.ToString(), 6);

                if (reportFormat == 4)
                    msMessage = "Schedule report format(HTTP) is in construction";
                else
                {
                    string FileExtName = ReportFormatIndexToExtName(reportFormat);

                    if (FileExtName == "")
                        msMessage = "Invalid Report Format(" + reportFormat.ToString() + ")";
                    else
                    {
                        msMessage = "";
                        SetParameterKeyValue_String("reportformat", FileExtName);
                    }
                }

                if (string.IsNullOrEmpty(msMessage))
                { 
                    // Update process status
                    ReportingServiceLogUpdate(msInitialTime, "Preprocess", 1);

                    if (RenderReportPreprocesses())
                    {
                        // Replace From Date
                        SetParameterKeyValue_String("datefrom", dateFrom);
                        // Replace To Date
                        SetParameterKeyValue_String("dateto", dateTo);
                        // Report ID
                        ReportingServiceLogUpdate(msInitialTime, GetParameterKeyValue_String("reportid"), 5);
                        // Status
                        ReportingServiceLogUpdate(msInitialTime, "Prerendering", 1);
                        // Render report
                        if (RenderServerReport("", ref reportUrl))
                        {
                            // File location / URL
                            ReportingServiceLogUpdate(msInitialTime, reportUrl, 2);
                            msMessage = "";
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(msMessage))
                                msMessage = "Render report[" + GetParameterKeyValue_String("reportid", "")  + "]: Failed";
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(msMessage))
                        msMessage = "Preprocess Failed";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(msMessage))
                    msMessage = "Initial log failed";
            }

            if (string.IsNullOrEmpty(msMessage))
                return true;
            else
            {
                ReportingServiceLogUpdate(msInitialTime, msMessage, 3);
                return false;
            }
        }

        #region Commentted Code for reference

        //RenderScheduledReport(int UserID, string SecurityID, string dateFrom, string dateTo, string parameters, int reportFormat, string language, ref string reportUrl);

        /// <summary>
        /// Render server report asynchronouse
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="ReportID"></param>
        /// <param name="callback"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        //[System.Web.Services.WebMethod]
        //public IAsyncResult BeginRenderServerReportAsync(string parameters, AsyncCallback callback, object s)
        //{
        //    asyncRenderServerReport asyncEvent = new asyncRenderServerReport(RenderServerReport);
            
        //    AsyncState asyncstate = new AsyncState();
        //    asyncstate.previousState = s;
        //    asyncstate.asyncEvent = asyncEvent;

        //    return asyncEvent.BeginInvoke(parameters, asyncstate, callback, s);
        //}
  
        /// <summary>
        /// Asyn Call Back
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        //[System.Web.Services.WebMethod]
        //public bool EndRenderServerReport(IAsyncResult call)
        //{
        //    //Remove from service wide container
        //    AsyncState ms = (AsyncState)call.AsyncState;
        //    return ms.asyncEvent.EndInvoke(call);
        //}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobID"></param>
        //[System.Web.Services.WebMethod]
        //public void StopJob(Guid jobID)
        //{
        //    //Look for the job in the service wide container
        //    //AsyncState state = GetStateFromServiceWideContainer(jobID);
        //    //state.Abort = true;
        //}

        //[WebMethod]
        //public DataSet ReportList(int OrganizationID)
        //{

        //    return new DataSet();
        //}

        #endregion

        #endregion

        #region Inner classes section
        //This state object is what you can use to track invocations of your method
        //You'll need to store it in a thread safe container.  Add it to the container in the Begin method and remove it in the end method.  While it's in the container other web methods can find it and use it to monitor or stop the executing job.
        public class AsyncState
        {
            public Guid JobID = Guid.NewGuid();
            public object previousState;
            public asyncRenderServerReport asyncEvent;
            public bool Abort = false;
        }
        #endregion

        #region private methods

        /// <summary>
        /// Overload
        /// </summary>
        /// <returns></returns>
        private bool GetDriverInfo() {
            return GetDriverInfo(GetParameterKeyValue_String("driverid", "0"));
        }

        /// <summary>
        /// Overload
        /// </summary>
        /// <returns></returns>
        private bool GetDriverInfo(string DriverID) {
            return (IsNumeric(DriverID))? GetDriverInfo(StringToInt32(DriverID)) : true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DriverId"></param>
        /// <returns></returns>
        private bool GetDriverInfo(int DriverId)
        {
            if (DriverId > 0)
            {
                string qryDriverInfo = "SELECT * FROM vlfDriver WHERE DriverID=" + DriverId.ToString() + "; ";
                string fullname = string.Empty;

                try
                {
                    string dbcs = string.Empty;

                    if (getWebConfigureConnectionString(CS_CSN_REPORT, out dbcs))
                    {
                        using (SqlConnection connection = new SqlConnection(dbcs))
                        {
                            using (SqlCommand command = new SqlCommand(qryDriverInfo, connection))
                            {
                                command.CommandTimeout = 600;
                                command.CommandType = CommandType.Text;

                                connection.Open();

                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        string firstmane = reader["FirstName"].ToString();      // FirstName
                                        string lastname = reader["LastName"].ToString();        //  LastName

                                        if (string.IsNullOrEmpty(firstmane.Trim()) && string.IsNullOrEmpty(lastname.Trim()))
                                        {
                                            SetParameterKeyValue_String("firstname", "Unknown");
                                            SetParameterKeyValue_String("lastname", "Driver");
                                            SetParameterKeyValue_String("drivername", "Unknown Driver");

                                        }
                                        else
                                        {
                                            SetParameterKeyValue_String("firstname", firstmane);
                                            SetParameterKeyValue_String("lastname", lastname);
                                            SetParameterKeyValue_String("drivername", (firstmane + " " + lastname).Trim());
                                        }
                                    }
                                    else
                                    {
                                        msMessage = "ID not found";
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        msMessage = "Configured connection string[" + CS_CSN_REPORT + "] missing.";
                    }
                }
                catch (SqlException Ex)
                {
                    msMessage = Ex.Message.ToString();
                }
                catch (Exception Ex)
                {
                    msMessage = Ex.Message.ToString();
                }
                finally
                {
                    if (!string.IsNullOrEmpty(msMessage))
                        msMessage = ("Failed to get driver info by ID[" + DriverId + "] for exception: " + msMessage);
                }
            }
            else
            {
                msMessage = "";     // No driver info needed.
            }
            return string.IsNullOrEmpty(msMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FleetID"></param>
        /// <returns></returns>
        private bool GetFleetDetails() 
        {
            return GetFleetDetails(GetParameterKeyValue_String("fleetid"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FleetID"></param>
        /// <returns></returns>
        private bool GetFleetDetails(string FleetID)
        {
            string nodecodes = GetParameterKeyValue_String("nodecode");
            /* No fleet and no nodecode, no detail and validation of fleet */
            if (string.IsNullOrEmpty(FleetID) && string.IsNullOrEmpty(nodecodes))
            {
                msMessage = "";             //No Fleet required
            }
            else if (!FleetID.Contains(",") && (FleetID == "0")) {
                msMessage = "";             //No Fleet required
            }
            else
            {
                if (string.IsNullOrEmpty(FleetID)) FleetID = GetNodecodeFleetID(nodecodes);
                /* Validate Fleet ID List if no error and Fleet ID list is not empty */
                if (string.IsNullOrEmpty(msMessage) && !string.IsNullOrEmpty(FleetID))
                {
                    #region Fleet validation

                    string[] fleets = FleetID.Split(',');

                    for (int i = 0; i < fleets.Length; i++)
                    {
                        msMessage = "";
                        /* convert fleet id to integer */
                        int iFleet = StringToInt32(fleets[i]);
                        /* Invalid fleet ID = 0 */
                        if (iFleet <= 0)
                        {
                            msMessage = "Invalid fleet ID: " + fleets[i].ToString();
                            break;
                        }
                        else
                        {
                            /* Verify fleet information */
                            try
                            {
                                string dbcs = string.Empty;

                                if (getWebConfigureConnectionString(CS_CSN_DEFAULT, out dbcs))
                                {
                                    //using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[msDefaulDatabaseConnectionName].ConnectionString))
                                    using (SqlConnection connection = new SqlConnection(dbcs))
                                    {
                                        using (SqlCommand sqlComment = new SqlCommand())
                                        {
                                            /* initial command */
                                            sqlComment.CommandType = CommandType.Text;
                                            sqlComment.Connection = connection;
                                            sqlComment.CommandText = "SELECT FleetName, OrganizationID, Description, FleetType, NodeCode FROM vlfFleet WHERE FleetID = @FleetID;"; // + FleetID.ToString();
                                            sqlComment.CommandTimeout = 600;
                                            /* Initial parameters */
                                            sqlComment.Parameters.Add(new SqlParameter("@FleetID", SqlDbType.Int));
                                            sqlComment.Parameters["@FleetID"].Value = iFleet;  //FleetID;
                                            /* Open connection*/
                                            connection.Open();
                                            /* load data */
                                            using (SqlDataReader reader = sqlComment.ExecuteReader())
                                            {
                                                if (reader.Read())
                                                {
                                                    /* get fleet name */
                                                    msFleetName = reader["FleetName"].ToString();
                                                    /* is valid fleet name */
                                                    if (string.IsNullOrEmpty(msFleetName))
                                                        msMessage = "No name.";
                                                    else
                                                    {
                                                        msFleetOrganization = reader["OrganizationID"].ToString();
                                                        msFleetDescription = reader["Description"].ToString();
                                                        msFleetType = reader["FleetType"].ToString();
                                                        msFleetNodeCode = reader["NodeCode"].ToString();

                                                        SetParameterKeyValue_String("fleetname", msFleetName);
                                                        SetParameterKeyValue_String("Description", msFleetDescription);
                                                    }
                                                }
                                                else
                                                {
                                                    msMessage = "Not found.";
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    msMessage = "Configured connection string [" + CS_CSN_DEFAULT + "] not found.";
                                }
                            }
                            catch (SqlException sx)
                            {
                                msMessage = sx.Message;
                            }
                            catch (Exception ex)
                            {
                                msMessage = ex.Message;
                            }
                            finally
                            {
                                if (!string.IsNullOrEmpty(msMessage))
                                    msMessage = "Failed to get info of fleet[" + iFleet.ToString() + "] for exception: " + msMessage;
                            }//end try
                            /* break loop if error */
                            if (!string.IsNullOrEmpty(msMessage)) break;
                        }//end if (iFleet <= 0)
                    }//next i
                    /* Error check */
                    if (string.IsNullOrEmpty(msMessage))
                    {
                        if (fleets.Length > 1)
                        {
                            SetParameterKeyValue_String("fleetid", FleetID.Replace(",", ", "));
                            SetParameterKeyValue_String("fleetname", "Multiple Hierarchies");
                            SetParameterKeyValue_String("Description", "Multiple Hierarchies");
                        }
                    }
                    #endregion
                }
            }
            return (string.IsNullOrEmpty(msMessage));
        }

        #region Get geozone info
        /// <summary>
        /// Overload
        /// </summary>
        /// <returns></returns>
        private bool GetGeozoneInfo()
        {
            return GetGeozoneInfo(GetParameterKeyValue_String("geozone", "0"));
        }

        /// <summary>
        /// Overload
        /// </summary>
        /// <returns></returns>
        private bool GetGeozoneInfo(string Geozone)
        {
            return (IsNumeric(Geozone)) ? GetGeozoneInfo(StringToInt32(Geozone)) : true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DriverId"></param>
        /// <returns></returns>
        private bool GetGeozoneInfo(int Geozone)
        {
            msMessage = "";

            if (Geozone > 0)
            {
                string qryDriverInfo = "SELECT * FROM vlfDriver WHERE DriverID=" + Geozone.ToString() + "; ";

                #region Extract geozone info from database
                //try
                //{
                //    string dbcs = string.Empty;

                //    if (getWebConfigureConnectionString(CS_CSN_REPORT, out dbcs))
                //    {
                //        using (SqlConnection connection = new SqlConnection(dbcs))
                //        {
                //            using (SqlCommand command = new SqlCommand(qryDriverInfo, connection))
                //            {
                //                command.CommandTimeout = 600;
                //                command.CommandType = CommandType.StoredProcedure;

                //                connection.Open();

                //                using (SqlDataReader reader = command.ExecuteReader())
                //                {
                //                    if (reader.Read())
                //                    {
                //                        string firstmane = reader["FirstName"].ToString();      // FirstName
                //                        string lastname = reader["LastName"].ToString();        //  LastName

                //                        if (string.IsNullOrEmpty(firstmane.Trim()) && string.IsNullOrEmpty(lastname.Trim()))
                //                        {
                //                            SetParameterKeyValue_String("firstname", "Unknown");
                //                            SetParameterKeyValue_String("lastname", "Driver");
                //                            SetParameterKeyValue_String("drivername", "Unknown Driver");

                //                        }
                //                        else
                //                        {
                //                            SetParameterKeyValue_String("firstname", firstmane);
                //                            SetParameterKeyValue_String("lastname", lastname);
                //                            SetParameterKeyValue_String("drivername", (firstmane + " " + lastname).Trim());
                //                        }
                //                    }
                //                    else
                //                    {
                //                        msMessage = "ID not found";
                //                    }
                //                }
                //            }
                //        }
                //    }
                //    else
                //    {
                //        msMessage = "Configured connection string[" + CS_CSN_REPORT + "] missing.";
                //    }
                //}
                //catch (SqlException Ex)
                //{
                //    msMessage = Ex.Message.ToString();
                //}
                //catch (Exception Ex)
                //{
                //    msMessage = Ex.Message.ToString();
                //}
                //finally
                //{
                //    if (!string.IsNullOrEmpty(msMessage))
                //        msMessage = ("Failed to get driver info by ID[" + DriverId + "] for exception: " + msMessage);
                //}
                #endregion
            }
            return string.IsNullOrEmpty(msMessage);
        }
        #endregion
        
        #region Get landmark info
        /// <summary>
        /// Get landmark info
        /// </summary>
        /// <returns></returns>
        private bool GetLandmarkInfo()
        {
            if (this.ParameterKeyExists("landmarkid"))
                return GetLandmarkInfo(StringToInt32(GetParameterKeyValue_String("landmarkid", "0")));
            else if (this.ParameterKeyExists("landmarkname"))
                return GetLandmarkInfo(GetParameterKeyValue_String("landmarkname", ""));
            else
                return true;
        }

        /// <summary>
        /// Get landmark info by name
        /// </summary>
        /// <returns></returns>
        private bool GetLandmarkInfo(string Landmark)
        {
            msMessage = "";

            if (!string.IsNullOrEmpty(Landmark))
            {
                string dbconnection = "";

                if (getWebConfigureConnectionString(CS_CSN_REPORT, out dbconnection))
                {
                    #region get landmark info

                    string query = "SELECT * FROM vlfLandmark WHERE LandmarkName = '" + Landmark + "'; ";

                    try
                    {
                        using (SqlDataReader reader = NewDataReader(dbconnection, query))
                        {
                            if (reader.Read())
                            {
                                SetParameterKeyValue_String("landmarkid", reader["landmarkid"].ToString());
                            }
                            else
                            {
                                //msMessage = string.Format("Landmark Name[{0}] Not found", Landmark);
                                msMessage = "Landmark Name[" + Landmark + "] Not found";
                            }
                        }
                    }
                    catch (SqlException Ex)
                    {
                        msMessage = Ex.Message.ToString();
                    }
                    catch (Exception Ex)
                    {
                        msMessage = Ex.Message.ToString();
                    }
                    finally
                    {
                        if (!string.IsNullOrEmpty(msMessage))
                            msMessage = ("Failed to get Landmark info by name[" + Landmark + "]. Exception: " + msMessage);
                    }
                    #endregion

                }
                else
                {
                    msMessage = "Configured connection string[" + CS_CSN_REPORT + "] missing.";
                }
            }
            return string.IsNullOrEmpty(msMessage);
        }

        /// <summary>
        /// Get landmark info by ID
        /// </summary>
        /// <param name="Landmark"></param>
        /// <returns></returns>
        private bool GetLandmarkInfo(int Landmark)
        {
            msMessage = "";
            ReportingServiceLogUpdate(msInitialTime, "Landmark", 1);

            if (Landmark > 0)
            {
                string dbconnection = "";

                if (getWebConfigureConnectionString(CS_CSN_REPORT, out dbconnection))
                {
                    #region get landmark info

                    string query = "SELECT * FROM vlfLandmark WHERE LandmarkID = " + Landmark.ToString() + "; ";

                    try
                    {
                        using (SqlDataReader reader = NewDataReader(dbconnection, query))
                        {
                            if (reader.Read())
                            {
                                SetParameterKeyValue_String("landmarkname", reader["LandmarkName"].ToString());
                            }
                            else
                            {
                                //msMessage = string.Format("Landmark ID[{0}] Not found", Landmark.ToString());
                                msMessage = "Landmark ID[" + Landmark.ToString() + "] Not found";
                            }
                        }
                    }
                    catch (SqlException Ex)
                    {
                        msMessage = Ex.Message.ToString();
                    }
                    catch (Exception Ex)
                    {
                        msMessage = Ex.Message.ToString();
                    }
                    finally
                    {
                        if (!string.IsNullOrEmpty(msMessage))
                            msMessage = ("Failed to get Landmark info by ID[" + Landmark.ToString() + "]. Exception: " + msMessage);
                    }
                    #endregion
                }
            }

            ReportingServiceLogUpdate(msInitialTime, msMessage, 3);

            return string.IsNullOrEmpty(msMessage);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetNodecodeFleetID()
        {
            return GetNodecodeFleetID(GetParameterKeyValue_String("nodecode"));
        }

        /// <summary>
        /// get fleet id list by nodecodes if nodecode is issued.
        /// </summary>
        /// <param name="Nodecodes"></param>
        /// <returns></returns>
        private string GetNodecodeFleetID(string Nodecodes) 
        {
            string fleets = "";
            // No nodecode, no fleet id list
            if (!string.IsNullOrEmpty(Nodecodes))
            {
                #region get fleet id list from node codes
                try
                {
                    using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[msDefaulDatabaseConnectionName].ConnectionString))
                    {
                        using (SqlCommand sqlComment = new SqlCommand())
                        {
                            sqlComment.CommandTimeout = 600;

                            sqlComment.CommandType = CommandType.Text;
                            sqlComment.Connection = connection;
                            sqlComment.CommandText = "SELECT STUFF(( SELECT ', ' + cast(FleetID as varchar(12)) FROM vlfFleet WHERE CHARINDEX(NodeCode, @Nodecodes) > 0 FOR XML PATH ('')),1,2,'') AS FleetList;";

                            sqlComment.Parameters.Add(new SqlParameter("@Nodecodes", SqlDbType.VarChar, 4000));
                            sqlComment.Parameters["@NodeCodes"].Value = Nodecodes;  //FleetID;

                            connection.Open();

                            using (SqlDataReader reader = sqlComment.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    fleets = reader["FleetList"].ToString();
                                    msMessage = "";
                                    //if (!string.IsNullOrEmpty(idList))  SetParameterKeyValue_String("fleetid", idList);
                                }
                                else
                                {
                                    msMessage = "Not found.";
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    msMessage = ex.Message;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(msMessage))
                        msMessage = "Get fleet id from nodecodes[" + Nodecodes + "] failed. Exception: " + msMessage;
                }
                #endregion
            }
            else
            {
                msMessage = "";
                fleets = "";
            }
            return fleets;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReportID"></param>
        /// <returns></returns>
        private bool GetReportDetails()
        {
            return GetReportDetails(GetParameterKeyValue_String("reportid", ""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReportID"></param>
        /// <returns></returns>
        private bool GetReportDetails(string ReportID)
        {
            if (!GetReportDetails(StringToInt32(ReportID)))
            {
                if (StringToInt32(ReportID) > 0)
                    msMessage = (string.IsNullOrEmpty(msMessage)) ? "Load report info failed for unknow exception." : msMessage;
                else
                    msMessage = "Invalid report id["+ ReportID +"]";
            }
            return string.IsNullOrEmpty(msMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReportId"></param>
        /// <returns></returns>
        private bool GetReportDetails(int ReportId)
        {
            if (ReportId > 0)
            {
                try
                {
                    string dbconnection = string.Empty;

                    if (getWebConfigureConnectionString(CS_CSN_REPORT, out dbconnection))
                    {
                        #region get report info

                        string query = "SELECT * FROM vlfReportTypes WHERE GUIID = " + ReportId.ToString() + "; ";

                        try
                        {
                            using (SqlDataReader reader = NewDataReader(dbconnection, query))
                            {
                                if (reader.Read())
                                {
                                    SetParameterKeyValue_String("reportcategory", reader[0].ToString());                // msReportCategory = ;
                                    SetParameterKeyValue_String("reporturi", reader["ReportTypesName"].ToString());     // msReportUri = ;
                                    SetParameterKeyValue_String("reportpath", reader["Description"].ToString());        // msReportPath = ;
                                    SetParameterKeyValue_String("reportname", reader["GuiName"].ToString());            // msReportName = ;
                                    //SetParameterKeyValue_String("reportpage", reader[4].ToString());                  // msReportPage = ;
                                    //SetParameterKeyValue_String("reporttype", reader[5].ToString());                  // msReportType = ;

                                    msReportUri = GetParameterKeyValue_String("reporturi");
                                    msReportPath = GetParameterKeyValue_String("reportpath");

                                    if (string.IsNullOrEmpty(GetParameterKeyValue_String("reportname")))
                                        msMessage = "No name";
                                    else if (string.IsNullOrEmpty(GetParameterKeyValue_String("reporturi")))
                                        msMessage = "No URI";
                                    else if (string.IsNullOrEmpty(GetParameterKeyValue_String("reportpath")))
                                        msMessage = "No path";
                                    else
                                        msMessage = "";
                                }
                                else
                                {
                                    //msMessage = string.Format("Landmark ID[{0}] Not found", Landmark.ToString());
                                    msMessage = "Report ID[" + ReportId.ToString() + "] Not found";
                                }
                            }
                        }
                        catch (SqlException Ex)
                        {
                            msMessage = Ex.Message.ToString();
                        }
                        catch (Exception Ex)
                        {
                            msMessage = Ex.Message.ToString();
                        }
                        finally
                        {
                            if (!string.IsNullOrEmpty(msMessage))
                                msMessage = ("Failed to get Landmark info by ID. Exception: " + msMessage + " by query " + msReportServiceConnectionName);
                        }
                        #endregion

                        #region Commented out of date code
                        ////using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[msReportServiceConnectionName].ConnectionString))
                        //using (SqlConnection connection = new SqlConnection(dbcs))
                        //{
                        //    using (SqlCommand command = new SqlCommand("sp_ReportDetails", connection))
                        //    {
                        //        command.CommandTimeout = 600;
                        //        command.CommandType = CommandType.StoredProcedure;
                        //        command.Parameters.Add("@ReportID", SqlDbType.Int);
                        //        command.Parameters["@ReportID"].Value = ReportId;

                        //        connection.Open();

                        //        using (SqlDataReader reader = command.ExecuteReader())
                        //        {
                        //            if (reader.Read())
                        //            {
                        //                SetParameterKeyValue_String("reportcategory", reader[0].ToString());            // msReportCategory = ;
                        //                SetParameterKeyValue_String("reporturi", reader[1].ToString());                 // msReportUri = ;
                        //                SetParameterKeyValue_String("reportpath", reader[2].ToString());                // msReportPath = ;
                        //                SetParameterKeyValue_String("reportname", reader[3].ToString());                // msReportName = ;
                        //                SetParameterKeyValue_String("reportpage", reader[4].ToString());                // msReportPage = ;
                        //                SetParameterKeyValue_String("reporttype", reader[5].ToString());                // msReportType = ;

                        //                msReportUri = GetParameterKeyValue_String("reporturi");
                        //                msReportPath = GetParameterKeyValue_String("reportpath");

                        //                if (string.IsNullOrEmpty(GetParameterKeyValue_String("reportname")))
                        //                    msMessage = "No name";
                        //                else if (string.IsNullOrEmpty(GetParameterKeyValue_String("reporturi")))
                        //                    msMessage = "No URI";
                        //                else if (string.IsNullOrEmpty(GetParameterKeyValue_String("reportpath")))
                        //                    msMessage = "No path";
                        //                else
                        //                    msMessage = "";
                        //            }
                        //            else
                        //            {
                        //                msMessage = "Not found";
                        //            }
                        //        }
                        //    }
                        //}
                        #endregion

                    }
                    else
                    {
                        msMessage = "Configured connection string[" + CS_CSN_REPORT + "] missing.";
                    }
                }
                catch (SqlException Ex)
                {
                    msMessage = Ex.Message.ToString();
                }
                catch (Exception Ex)
                {
                    msMessage = Ex.Message.ToString();
                }
                finally
                {
                    if (!string.IsNullOrEmpty(msMessage))
                        msMessage = ("Failed to get report[" + ReportId + "] detail for exception: " + msMessage);
                }
            }
            else
            {
                msMessage = "Invalid report ID = 0";
            }
            return string.IsNullOrEmpty(msMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        private bool GetUserPreferences()
        {
            return GetUserPreferences(GetParameterKeyValue_String("userid"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        private bool GetUserPreferences(string UserId)
        {
            if (!GetUserPreferences(StringToInt32(UserId))) {
                
                    msMessage = "Indalid user ID [" + UserId + "].";
            }
            return string.IsNullOrEmpty(msMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        private bool GetUserPreferences(Int32 UserId)
        {
            msMessage = "";

            try
            {
                DataSet dsPref = new DataSet();

                VLF.DAS.Logic.User dbUser = new VLF.DAS.Logic.User(LoginManager.GetConnnectionString(UserId));
                dsPref = dbUser.GetAllUserPreferencesInfo(UserId);
                Int16 PreferenceId = 0;

                if (dsPref.Tables[0].Rows.Count > 0) 
                {
                    foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                    {
                        PreferenceId = Convert.ToInt16(rowItem["PreferenceId"]);
                        switch (PreferenceId)
                        {
                            case (Int16)VLF.CLS.Def.Enums.Preference.MeasurementUnits:

                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    SetParameterKeyValue_String("UnitOfMes", rowItem["PreferenceValue"].ToString().TrimEnd());
                                else
                                    SetParameterKeyValue_String("UnitOfMes", "1");

                                break;

                            case (Int16)VLF.CLS.Def.Enums.Preference.VolumeUnits:

                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    SetParameterKeyValue_String("VolumeUnit", rowItem["PreferenceValue"].ToString().TrimEnd());
                                else
                                    SetParameterKeyValue_String("VolumeUnit", "1");

                                break;

                            case (Int16)VLF.CLS.Def.Enums.Preference.TimeZone:

                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    SetParameterKeyValue_String("UserTimeZone", rowItem["PreferenceValue"].ToString().TrimEnd());
                                else
                                    SetParameterKeyValue_String("UserTimeZone", "");

                                break;

                            case (Int16)VLF.CLS.Def.Enums.Preference.DayLightSaving:

                                if (rowItem["PreferenceValue"].ToString().TrimEnd() == "1")
                                    SetParameterKeyValue_String("DayLightSaving", "1");
                                else
                                    SetParameterKeyValue_String("DayLightSaving", "0");

                                break;

                            default:

                                break;
                        }
                    }
                }
                else
                {
                    msMessage = "No preference.";
                }
            }
            catch (Exception ex)
            {
                msMessage = ex.Message;
            }
            finally 
            {
                if (!string.IsNullOrEmpty(msMessage))
                    msMessage = "Failed to get user [" + UserId.ToString() + "] preference for Exception: " + msMessage;

            }

            return string.IsNullOrEmpty(msMessage); 
        }

        /// <summary>
        /// [usp_getVehicleDetails] @BoxID = 0, @VehicleID = 0, @LicensePlate = ' A00620' 
        /// </summary>
        /// <param name="BoxID"></param>
        /// <returns></returns>
        private bool GetVehicleDetails()
        {
            string box = GetParameterKeyValue_String("boxid", "");
            string vehicle = GetParameterKeyValue_String("vehicleid", "");
            string license = GetParameterKeyValue_String("licenseplate", "");
            string msg = "";

            msMessage = "";

            if (!string.IsNullOrEmpty(box))
                msg = "Box[" + box + "]";
            else if (!string.IsNullOrEmpty(vehicle))
                msg = "Vehicle[" + vehicle + "]";
            else if (!string.IsNullOrEmpty(license))
                msg = "License[" + license + "]";
            else
                msg = "";

            // One of Box, VID, LP has valid value, get Vehicle info, else step out.
            if (!string.IsNullOrEmpty(msg))
            {
                try
                {
                    string dbcs = string.Empty;
                    if (getWebConfigureConnectionString(CS_CSN_REPORT, out dbcs))
                    {
                        //using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[msReportServiceConnectionName].ConnectionString))

                        using (SqlConnection connection = new SqlConnection(dbcs))
                        {
                            using (SqlCommand command = new SqlCommand("usp_getVehicleDetails", connection))
                            {
                                command.CommandTimeout = 600;
                                command.CommandType = CommandType.StoredProcedure;

                                command.Parameters.Add("@BoxID", SqlDbType.Int);
                                command.Parameters["@BoxID"].Value = StringToInt32(box);

                                command.Parameters.Add("@VehicleID", SqlDbType.Int);
                                command.Parameters["@VehicleID"].Value = StringToInt32(vehicle);

                                command.Parameters.Add("@LicensePlate", SqlDbType.NVarChar, 30);
                                command.Parameters["@LicensePlate"].Value = license;

                                connection.Open();

                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        SetParameterKeyValue_String("vehicleid", reader["VehicleID"].ToString());                       //vi.VehicleID
                                        //SetParameterKeyValue_String("reporturi", reader[1].ToString());                                 //vi.VinNum,
                                        //SetParameterKeyValue_String("reportpath", reader[2].ToString());                                //vi.MakeModelID,
                                        //SetParameterKeyValue_String("reportname", reader[3].ToString());                                //vi.ModelYear,
                                        //SetParameterKeyValue_String("reportname", reader[3].ToString());                                //vi.Color,
                                        SetParameterKeyValue_String("vehiclename", reader["Description"].ToString());                    //vi.[Description],
                                        //SetParameterKeyValue_String("reportname", reader[3].ToString());                                //vi.OrganizationID,
                                        //SetParameterKeyValue_String("reportname", reader[3].ToString());                                //vi.TimeZone,
                                        //SetParameterKeyValue_String("reportname", reader[3].ToString());                                //vi.DaylightSaving,
                                        SetParameterKeyValue_String("licenseplate", reader["LicensePlate"].ToString());                 //va.LicensePlate ;
                                        SetParameterKeyValue_String("boxid", reader["BoxID"].ToString());                               // va.BoxID;

                                        msMessage = "";
                                    }
                                    else
                                    {
                                        msMessage = "Not found.";
                                    }
                                }
                            }
                        }
                    }
                    else
	                {
                        msMessage = "Configured connection string[" + CS_CSN_REPORT + "] not found.";
	                }
                }
                catch (SqlException Ex)
                {
                    msMessage = Ex.Message.ToString();
                }
                catch (Exception Ex)
                {
                    msMessage = Ex.Message.ToString();
                }
                finally
                {
                    if (!string.IsNullOrEmpty(msMessage))
                        msMessage = "Failed to get vechile details by " + msg + ". Exception: " + msMessage;
                }
            }
            else
            {
                msMessage = "";
            }

            return string.IsNullOrEmpty(msMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        private string getWebConfigAppSettingsKey(string KeyName) 
        {
            if (ConfigurationManager.AppSettings[KeyName] != null)
                return ConfigurationManager.AppSettings[KeyName].ToString();
            else
                return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="KeyName"></param>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        private bool getWebConfigAppSettingsKey(string KeyName, out string KeyValue)
        {
            KeyValue = getWebConfigAppSettingsKey(KeyName);
            return !(string.IsNullOrEmpty(KeyValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectionName"></param>
        /// <returns></returns>
        private string getWebConfigureConnectionString(string ConnectionName) {
            if (ConfigurationManager.ConnectionStrings[ConnectionName] != null)
                return ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString;
            else
                return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectionName"></param>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        private bool getWebConfigureConnectionString(string ConnectionName, out string ConnectionString)
        {
            ConnectionString = getWebConfigureConnectionString(ConnectionName);
            return !string.IsNullOrEmpty(ConnectionString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="rp"></param>
        /// <returns></returns>
        private bool InitialReportParameters(Microsoft.Reporting.WebForms.ServerReport sr, ReportParameterInfoCollection rp)
        {
            msMessage = "";

            try
            {
                ReportingServiceLogUpdate(msInitialTime, "Initial report parameters", 1);

                string UserID = GetParameterKeyValue_String("userid");
                string DateFr = GetParameterKeyValue_String("datefrom").ToString();
                string DateTo = GetParameterKeyValue_String("dateto").ToString();

                #region General Parameters

                // Orgznization ID
                if (rp["OrganizationID"] != null)
                {
                    if (ParameterKeyExists("organization"))
                    {
                        string oid = this.GetParameterKeyValue_String("organization", "");
                        if (oid == "0")
                            sr.SetParameters(new ReportParameter("OrganizationID", oid, false));
                        else if(StringToInt32(oid) > 0)
                            sr.SetParameters(new ReportParameter("OrganizationID", oid, false));
                        else
                        {
                            msMessage = "Invalid Organization ID.";
                            return false;
                        }
                    }
                    else
	                {
                        msMessage = "Organization ID Missing.";
                        return false;
                    }
                }

                // UserID
                if (rp["UserID"] != null)
                {
                    if (ParameterKeyExists("userid"))
                    {
                        string uid = this.GetParameterKeyValue_String("userid", "");

                        if (StringToInt32(uid) > 0)
                            sr.SetParameters(new ReportParameter("UserID", this.GetParameterKeyValue_String("userid"), false));    
                        else
                        {
                            msMessage = "INvalid User ID.";
                            return false;
                        }
                    }
                    else
                    {
                        msMessage = "User ID Missing.";
                        return false;
                    }
                }

                // Fleet ID
                if (rp["FleetID"] != null)
                {
                    if (ParameterKeyExists("fleetid"))
                    {
                        string fid = this.GetParameterKeyValue_String("fleetid");
                        //multi-fleet validation
                        if(IsValidFleetID(fid))
                            //sr.SetParameters(new ReportParameter("FleetID", this.GetParameterKeyValue_String("fleetid"), false));
                            sr.SetParameters(new ReportParameter("FleetID", fid, false));
                        else
                        {
                            if (!string.IsNullOrEmpty(msMessage))
                                msMessage = "Invalid Fleet ID.";
                            return false;
                        }
                    }
                    else
                    {
                        msMessage = "Fleet ID Missing";
                        return false;
                    }
                }

                // NodeCode
                if (rp["NodeCode"] != null)
                {
                    if (ParameterKeyExists("nodecode"))
                    {
                        sr.SetParameters(new ReportParameter("NodeCode", this.GetParameterKeyValue_String("nodecode"), false));
                    }
                    else
                    {
                        msMessage = "Node code Missing";
                        return false;
                    }
                }

                // Fleet Name
                if (rp["FleetName"] != null)
                {
                    if (ParameterKeyExists("fleetname"))
                        sr.SetParameters(new ReportParameter("FleetName", this.GetParameterKeyValue_String("fleetname"), false));
                    else
                    {
                        msMessage = "Fleet name Missing";
                        return false;
                    }
                }

                // Hierarchy Node Code
                if (rp["NodeCode"] != null) {
                    if (ParameterKeyExists("NodeCode"))
                        sr.SetParameters(new ReportParameter("NodeCode", this.GetParameterKeyValue_String("NodeCode"), false));
                    else
                    {
                        msMessage = "Node code Missing";
                        return false;
                    }
                }
                    

                //From Date
                if (rp["FromDate"] != null)
                {
                    if (ParameterKeyExists("datefrom"))
                    {
                        string df = this.GetParameterKeyValue_DateTime("datefrom").ToString("yyyy/MM/dd hh:mm:ss tt");
                        sr.SetParameters(new ReportParameter("FromDate", df, false));
                    }
                    else
                    {
                        msMessage = "From-Date Missing";
                        return false;
                    }
                }

                // To Date
                if (rp["ToDate"] != null)
                {
                    if (ParameterKeyExists("dateto"))
                    {
                        string dt = this.GetParameterKeyValue_DateTime("dateto").ToString("yyyy/MM/dd hh:mm:ss tt");
                        sr.SetParameters(new ReportParameter("ToDate", dt, false));
                    }
                    else
                    {
                        msMessage = "To-Date missing";
                        return false;
                    }
                }

                #endregion

                #region User Preferences will be set in reports as internal parameters instead of pass-in from code
                
                // Time Zone
                if (rp["TimeZone"] != null){
                    if ((rp["TimeZone"].Dependencies.Count == 0) && (moParameters.ContainsKey("timezone")))
                        sr.SetParameters(new ReportParameter("TimeZone", this.GetParameterKeyValue_String("timezone"), false));
                }
                
                // Volume Conversion factor
                if (rp["UnitOfVolume"] != null){
                    if ((rp["UnitOfVolume"].Dependencies.Count == 0) && (moParameters.ContainsKey("unitofvolume")))
                        sr.SetParameters(new ReportParameter("UnitOfVolume", this.GetParameterKeyValue_String("unitofvolume"), false));
                }

                // Distance/Speed Conversion factor
                if (rp["UnitOfSpeed"] != null)
                {
                    if ((rp["UnitOfSpeed"].Dependencies.Count == 0) && (moParameters.ContainsKey("unitofspeed")))
                        sr.SetParameters(new ReportParameter("UnitOfSpeed", this.GetParameterKeyValue_String("unitofspeed"), false));
                }

                // Language ID
                if (rp["Language"] != null){
                    if((moParameters.ContainsKey("language")))
                        sr.SetParameters(new ReportParameter("Language", LanguageCodeId(this.GetParameterKeyValue_String("language")), false));
                } 

                #endregion
               
                #region Fleet Violation Reprots

                // Mask of Violations for events 3, 15, 6, 7, 8, 9, 31
                if (rp["MaskViolations"] != null)
                    sr.SetParameters(new ReportParameter("MaskViolations", this.GetParameterKeyValue_String("violationmask", "63"), false));

                // Detail report: lower limitation of speed: 100 (default) 
                if (rp["Speed"] != null)
                    sr.SetParameters(new ReportParameter("Speed", this.GetParameterKeyValue_String("speedlimitation", "100"), false));
                
                // Summary report: point of each violation
                //ScoreOfSpeed120
                if (rp["ScoreOfSpeed120"] != null)
                    sr.SetParameters(new ReportParameter("ScoreOfSpeed120", this.GetParameterKeyValue_String("over120", "10"), false));

                //ScoreOfSpeed130
                if (rp["ScoreOfSpeed130"] != null)
                    sr.SetParameters(new ReportParameter("ScoreOfSpeed130", this.GetParameterKeyValue_String("over130", "20"), false));

                //ScoreOfSpeed140
                if (rp["ScoreOfSpeed140"] != null)
                    sr.SetParameters(new ReportParameter("ScoreOfSpeed140", this.GetParameterKeyValue_String("over140", "50"), false));

                // ScoreOfBrkHarsh 
                if (rp["ScoreOfBrkHarsh"] != null)
                    sr.SetParameters(new ReportParameter("ScoreOfBrkHarsh", this.GetParameterKeyValue_String("brakharsh", "10"), false));

                //ScoreOfBrkXtrem 
                if (rp["ScoreOfBrkXtrem"] != null)
                    sr.SetParameters(new ReportParameter("ScoreOfBrkXtrem", this.GetParameterKeyValue_String("brakiextreme", "20"), false));

                //ScoreOfAccHarsh
                if (rp["ScoreOfAccHarsh"] != null)
                    sr.SetParameters(new ReportParameter("ScoreOfAccHarsh", this.GetParameterKeyValue_String("accharsh", "10"), false));

                //ScoreOfAccXtrem
                if (rp["ScoreOfAccXtrem"] != null)
                    sr.SetParameters(new ReportParameter("ScoreOfAccXtrem", this.GetParameterKeyValue_String("accextreme", "20"), false));

                // ScoreOfSeatBelt 
                if (rp["ScoreOfSeatBelt"] != null)
                    sr.SetParameters(new ReportParameter("ScoreOfSeatBelt", this.GetParameterKeyValue_String("seatbelt", "50"), false));

                if (rp["Braking"] != null)
                    sr.SetParameters(new ReportParameter("Braking", this.GetParameterKeyValue_String("braking", "10"), false));

                if (rp["Reverse"] != null)
                    sr.SetParameters(new ReportParameter("Reverse", this.GetParameterKeyValue_String("reverse", "10"), false));

                if (rp["HyRailReverse"] != null)
                    sr.SetParameters(new ReportParameter("HyRailReverse", this.GetParameterKeyValue_String("hyrailreverse", "10"), false));

                if (rp["HyRailSpeed"] != null)
                    sr.SetParameters(new ReportParameter("HyRailSpeed", this.GetParameterKeyValue_String("hyrailspeed", "10"), false));

                #endregion
               
                #region Speeding for speed violation reports

                // Posted
                if (rp["IsPosted"] != null)
                    sr.SetParameters(new ReportParameter("IsPosted", this.GetParameterKeyValue_String("postedonly", "2"), false));

                // Speed over Road Limit Speed
                if (rp["OverRoadSpeed"] != null)
                    sr.SetParameters(new ReportParameter("OverRoadSpeed", this.GetParameterKeyValue_String("roadspeeddelta", "10"), false));

                // Speed Limitation
                if (rp["SpeedLimit"] != null)
                    sr.SetParameters(new ReportParameter("SpeedLimit", this.GetParameterKeyValue_String("speedlimitation", "100"), false));

                #endregion
                
                #region Fleet Utilization Reports

                // Cost
                if (rp["Cost"] != null)
                    sr.SetParameters(new ReportParameter("Cost", this.GetParameterKeyValue_String("costofidling", "0"), false));

                // 
                if (rp["ColorFilter"] != null)
                    sr.SetParameters(new ReportParameter("ColorFilter", this.GetParameterKeyValue_String("colorfilter", ""), false));

                #endregion

                #region Vehicle: ID, Name, Box, License Plate

                if (rp["VehicleID"] != null)    //if (moParameters.ContainsKey("vehicleid"))
                    sr.SetParameters(new ReportParameter("VehicleID", this.GetParameterKeyValue_String("vehicleid", "-1"), false));

                if(rp["VehicleName"] != null)   //if (moParameters.ContainsKey("vehiclename"))
                        sr.SetParameters(new ReportParameter("VehicleName", this.GetParameterKeyValue_String("vehiclename", ""), false));

                if (rp["BoxID"] != null)        //if (moParameters.ContainsKey("boxid"))
                        sr.SetParameters(new ReportParameter("BoxID", this.GetParameterKeyValue_String("boxid", "-1"), false));

                if (rp["LicensePlate"] != null)
                    sr.SetParameters(new ReportParameter("LicensePlate", this.GetParameterKeyValue_String("licenseplate"), false));

                #endregion

                #region Operational Log

                if (rp["Actions"] != null) 
                    sr.SetParameters(new ReportParameter("Actions", this.GetParameterKeyValue_String("actions", ""), false));

                if (rp["Modules"] != null)
                    sr.SetParameters(new ReportParameter("Modules", this.GetParameterKeyValue_String("modules", ""), false));

                if (rp["UpdateByUsers"] != null)
                    sr.SetParameters(new ReportParameter("UpdateByUsers", this.GetParameterKeyValue_String("updatebyusers", ""), false));

                #endregion

                #region Driver
                if (rp["DriverID"] != null)
                    sr.SetParameters(new ReportParameter("DriverID", this.GetParameterKeyValue_String("driverid", "0"), false));
                #endregion

                #region Stop and Idling

                //Show Store Position
                if (rp["IncAddress"] != null)
                    sr.SetParameters(new ReportParameter("IncAddress", this.GetParameterKeyValue_String("incaddress", "1"), false));
                // Stop/Idling Duration in seconds: 5:300 | 10:600 | ......                
                if (rp["MinDuration"] != null)
                    sr.SetParameters(new ReportParameter("MinDuration", this.GetParameterKeyValue_String("minduration", "0"), false));
                // Stop & Idling: 0:Stop | 1:Idling | 2:Both
                if (rp["Remark"] != null)
                    sr.SetParameters(new ReportParameter("Remark", this.GetParameterKeyValue_String("remark", "2"), false));
                #endregion

                if (rp["SensorNumber"] != null)
                    sr.SetParameters(new ReportParameter("SensorNumber", this.GetParameterKeyValue_String("sensornumber", "3"), false));

                // Landmark name
                if (rp["Landmarkname"] != null)
                    sr.SetParameters(new ReportParameter("Landmarkname", this.GetParameterKeyValue_String("landmarkname", ""), false));

                // Landmark name
                if (rp["LandmarkID"] != null)
                    sr.SetParameters(new ReportParameter("LandmarkID", this.GetParameterKeyValue_String("landmarkid", "0"), false));

                // Category = 0 / All (Default)
                if (rp["CategoryID"] != null)
                    sr.SetParameters(new ReportParameter("CategoryID", this.GetParameterKeyValue_String("categoryid", "0"), false));

                // Battery Threding Voltage Thredhold
                if (rp["FromThres"] != null)
                    sr.SetParameters(new ReportParameter("FromThres", this.GetParameterKeyValue_String("fromthres", "0.0000"), false));
                if (rp["ToThres"] != null)
                    sr.SetParameters(new ReportParameter("ToThres", this.GetParameterKeyValue_String("tothres", "9999"), false));

                //For event viewer report
                if (rp["Operation"] != null)
                    sr.SetParameters(new ReportParameter("Operation", this.GetParameterKeyValue_String("operation", "Event"), false));

                if (rp["EventID"] != null)
                    sr.SetParameters(new ReportParameter("EventID", this.GetParameterKeyValue_String("eventid", "0"), false));

                if (rp["NoOfDays"] != null)
                    sr.SetParameters(new ReportParameter("NoOfDays", this.GetParameterKeyValue_String("noofdays", "0"), false));

                if (rp["visiblecolumns"] != null)
                    sr.SetParameters(new ReportParameter("visiblecolumns", this.GetParameterKeyValue_String("visiblecolumns", "UnitID,LicensePlate,VehicleDescription,VehicleMake,VehicleModel,VINNumber,address,DriverName,ServiceName,ServiceType,DateTime"), false));   

                // Master Landmark Delta
                if (rp["Delta"] != null)
                    sr.SetParameters(new ReportParameter("Delta", this.GetParameterKeyValue_String("delta", "31"), false));

                // Out Of Home Terminal Report
                if (rp["ReportTime"] != null)
                    sr.SetParameters(new ReportParameter("ReportTime", this.GetParameterKeyValue_String("reporttime", ""), false));

            }
            catch (Exception ex)
            {
                msMessage = ex.Message;
            }
            finally
            {

            }

            return (msMessage == string.Empty);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RepositoryID"></param>
        /// <returns></returns>
        private bool RenderServerReport(string RepositoryID)
        {
            string url = "";
 
            return RenderServerReport(RepositoryID, ref url);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool RenderServerReport(string RepositoryID, ref string ReportUrl)
        {

            msMessage = "";

            if (IsValidOutputPath(ref msMessage))
            {
                try
                {
                    string ReportUri = GetParameterKeyValue_String("reporturi");
                    string ReportPath = GetParameterKeyValue_String("reportpath");
                    string ReportName = GetParameterKeyValue_String("reportname").ToString();
                    string ReportFormat = GetParameterKeyValue_String("reportformat");

                    ReportingServiceLogUpdate(msInitialTime, "Credential", 1);

                    Microsoft.Reporting.WebForms.ServerReport sr = new Microsoft.Reporting.WebForms.ServerReport();

                    // credential               
                    ReportServierCredentials rsc = new ReportServierCredentials(msCredentialUID, msCredentialPWD, msCredentialDMN); 
                    sr.ReportServerCredentials = rsc;
                    // Server Uri
                    sr.ReportServerUrl = new Uri(msReportService);
                    // Report path
                    sr.ReportPath = ReportPath;
                    // Report parameter information
                    ReportParameterInfoCollection rp = sr.GetParameters();
                    // Initial parameters of report
                    if (InitialReportParameters(sr, rp))
                    {
                        string mimeType;
                        string encoding;
                        string filenameExtension;
                        string[] streamids;
                        Warning[] warnings;

                        byte[] bytes;

                        ReportingServiceLogUpdate(msInitialTime, "Rendering", 1);

                        // ReportFormat: "PDF", "EXCEL", "WORD"
                        sr.Timeout = 1800000;

                        bytes = sr.Render(ReportFormat, null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

                        string ReportFile = GetReportName(ReportName.Replace("/", "-"), "." + filenameExtension);

                        //string uri = this.GetReportURL(ReportFile);

                        if (SaveReportFile(bytes, this.GetOutputPath(ReportFile)))
                        {
                            ReportUrl = this.GetReportURL(ReportFile);

                            if (IsNumeric(RepositoryID))
                            {
                                ReportingServiceLogUpdate(msInitialTime, "Repository update", 1);

                                if (!UpdateReportStatus(StringToInt64(RepositoryID), "", ReportUrl))
                                {
                                    if (string.IsNullOrEmpty(msMessage))
                                        msMessage = "Repository [" + RepositoryID.ToString() + "] update failed";
                                }
                            }
                        }
                        else
                            msMessage = "Save report file[" + ReportFile + "] failed.";
                    }
                    else
                    {
                        if (msMessage == string.Empty)
                            msMessage = "Mapping parameters failed.";
                    }
                }
                catch (IOException iox)
                {
                    msMessage = iox.Message + "(io)";
                }
                catch (ReportServerException rsx)
                {
                    msMessage = rsx.Message + "(re)";
                }
                catch (ReportSecurityException rsx)
                {
                    msMessage = rsx.Message + "(rs)";
                }
                catch (ReportViewerException rvx)
                {
                    msMessage = rvx.Message + "(rv)";
                }
                catch (Exception ex)
                {
                    msMessage = ex.Message + ": " + msReportService + ":\\" + msCredentialDMN + "\\" + msCredentialUID + "@" + msCredentialPWD;
                }
                finally
                {
                    if ((ReportUrl == "") && (msMessage == ""))
                        msMessage = "Invald URL.";
                }
            }

            return (string.IsNullOrEmpty(msMessage));

        }

        /// <summary>
        /// 
        /// </summary>
        private bool ParameterKeyValueInitial() 
        {
            msUserID = GetParameterKeyValue_String("userid"); 
            msReportID = GetParameterKeyValue_String("reportid");
            msReportUri = GetParameterKeyValue_String("reporturi");
            msReportPath = GetParameterKeyValue_String("reportpath");
            msReportFormat = GetParameterKeyValue_String("reportformat");
            msReportFormatCode = GetParameterKeyValue_String("reportformatcode");
            msReportName = GetParameterKeyValue_String("reportname").ToString();          // +Convert.ToInt32(DateTime.Now).ToString();     // +OutputFileExtName(moParameters["reportformat"].ToString());
            msLanguage = GetParameterKeyValue_String("language").ToString();
            msDateFrom = GetParameterKeyValue_String("datefrom").ToString();
            msDateTo = GetParameterKeyValue_String("dateto").ToString();
            msKeyValues = GetParameterKeyValue_String("keyvalues").ToString();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private bool ParameterParser(string parameters)
        {
            msMessage = "";
            
            ReportingServiceLogUpdate(msInitialTime, "Parameters", 1);

            try
            {
                moParameters.Clear();

                string s = parameters.Replace("{", "").Replace("}", "");	//.Replace(", ", "\t ");

                if (s.IndexOf('\t') <= 0) s = s.Replace(", ", "\t");

                Array a = s.Split('\t');
                int n = a.Length;

                if (n > 0)
                {
                    for (int i = a.GetLowerBound(0); i < n; i++)
                    {
                        string t = a.GetValue(i).ToString();

                        if (string.IsNullOrEmpty(t.Trim()))
                            break;
                        else if (t.IndexOf(":") > 0)
                        {
                            string k = t.Substring(0, t.IndexOf(":")).Trim();
                            string v = t.Substring(t.IndexOf(":") + 1).Trim();

                            moParameters.Add(k, v);
                        }
                        else
                        {
                            n = -1;
                            break;
                        }
                    }

                    if (n < 0)
                        msMessage = "Invalid parameter syntax.";
                    else
                        msMessage = "";
                }
                else
                {
                    msMessage = "";
                }
            }
            catch(Exception ex)
            {
                msMessage = ex.Message;
            }
            finally
            {
                if (moParameters.Count == 0 && string.IsNullOrEmpty(msMessage))
                    msMessage = "Invalid parameter is empty or null.";
            }

            return (string.IsNullOrEmpty(msMessage));

        }
        
        /// <summary>
        /// Prepare parameters for rendering report
        /// </summary>
        /// <returns></returns>
        private bool RenderReportPreprocesses()
        {
            if (ParameterParser(msParameters))
            {
               if (!GetReportDetails())
                    return false;
               if (!GetUserPreferences())
                   return false;
                if (!GetFleetDetails())
                    return false;
                if (!GetVehicleDetails())
                     return false;
                if(!GetDriverInfo())
                    return false;
                if (!GetLandmarkInfo())
                    return false;
                else if (!GetRepositoryKeyValue())
                    return false;
                else if (!ParameterKeyValueInitial())
                    return false;
                else
                    return true;
            }
            else
            {
                return false;
            }
        }
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileFormat"></param>
        /// <returns></returns>
        private string OutputFileExtName(string FileFormat) {
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        private bool IsValidOutputPath(ref string ErrorMessage)
        {
            return IsValidOutputPath(Server.MapPath(this.ReportsOutputPath), ref ErrorMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OutputPath"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        private bool IsValidOutputPath(string OutputPath, ref string ErrorMessage)
        {
            try
            {
                //OutputPath = Server.MapPath(OutputPath);

                ReportingServiceLogUpdate(msInitialTime, "Output path testing", 1);

                if (!string.IsNullOrEmpty(OutputPath))
                {
                    DirectoryInfo di = new DirectoryInfo(OutputPath);

                    if (di.Exists)
                        ErrorMessage = "";
                    else
                        ErrorMessage = "Not found.";
                }
                else
                {
                    ErrorMessage = "Invalid Output path is null or empty";
                }
            }
            catch (IOException io)
            {
                ErrorMessage = io.Message;
            }
            finally
            {
                if (!string.IsNullOrEmpty(ErrorMessage))
                    ReportingServiceLogUpdate(msInitialTime, "Output path[" + OutputPath + "] test failed. Error: " + ErrorMessage, 3);
            }

            return (string.IsNullOrEmpty(ErrorMessage));
        }

        /// <summary>
        /// Save report file
        /// </summary>
        /// <returns></returns>
        private bool SaveReportFile(byte[] bytes, string Path)
        {
            msMessage = "";

            try
            {
                using (FileStream fs = new FileStream(Path, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            catch (IOException ioex)
            {
                msMessage = ioex.Message;
            }
            catch (Exception ex)
            {
                msMessage = ex.Message;
            }
            finally
            {

            }

            return (msMessage == string.Empty);
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ReportTypeId"></param>
        /// <param name="Start"></param>
        /// <param name="Period"></param>
        /// <param name="KeyValues"></param>
        /// <param name="FormatId"></param>
        /// <returns></returns>
        private Int64 SaveReportRepository(Int64 UserId, int ReportTypeId, double Start, double Period, string KeyValues, int FormatId)
        {
            Int64 isSucceed = -1;

            try
            {
                string dbcs = string.Empty;

                if (getWebConfigureConnectionString(CS_CSN_ACTIVE, out dbcs))
                {
                    //using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ActiveStateConnectionString"].ConnectionString))
                    using (SqlConnection connection = new SqlConnection(dbcs))
                    {
                        using (SqlCommand sqlComment = new SqlCommand())
                        {
                            sqlComment.CommandTimeout = 600;
                            sqlComment.CommandType = CommandType.StoredProcedure;
                            sqlComment.Connection = connection;
                            sqlComment.CommandText = "AddReportRepository";

                            SqlParameter para = new SqlParameter("UserId", SqlDbType.BigInt);
                            para.Value = UserId;
                            sqlComment.Parameters.Add(para);

                            para = new SqlParameter("ReportTypeId", SqlDbType.Int);
                            para.Value = ReportTypeId;
                            sqlComment.Parameters.Add(para);

                            para = new SqlParameter("Start", SqlDbType.Float);
                            para.Value = Start;
                            sqlComment.Parameters.Add(para);

                            para = new SqlParameter("Period", SqlDbType.Float);
                            para.Value = Period;
                            sqlComment.Parameters.Add(para);

                            para = new SqlParameter("KeyValues", SqlDbType.NVarChar, 1024);
                            para.Value = KeyValues;
                            sqlComment.Parameters.Add(para);

                            para = new SqlParameter("FormatId", SqlDbType.TinyInt);
                            para.Value = FormatId;
                            sqlComment.Parameters.Add(para);

                            para = new SqlParameter("UpdaterId", SqlDbType.BigInt);
                            para.Value = UserId;
                            sqlComment.Parameters.Add(para);

                            para = new SqlParameter("@RETURN_VALUE", SqlDbType.BigInt);
                            para.Direction = ParameterDirection.ReturnValue;
                            para.Value = 0;
                            sqlComment.Parameters.Add(para);

                            connection.Open();

                            sqlComment.ExecuteNonQuery();

                            if (sqlComment.Parameters["@RETURN_VALUE"] != null)
                                isSucceed = Int64.Parse(sqlComment.Parameters["@RETURN_VALUE"].Value.ToString());

                            if (connection.State == ConnectionState.Open)
                                connection.Close();
                        }
                    }
                }
                else
                {
                    msMessage = "Configured connection string [" + CS_CSN_ACTIVE + "] not found.";
                }
            }
            catch (SqlException sx)
            {
                msMessage = sx.Message;
            }
            catch (Exception ex)
            {
                msMessage = ex.Message;
            }
            finally 
            { 
                if (msMessage != string.Empty)
                    isSucceed = -1;
            }

            return isSucceed;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Language"></param>
        /// <param name="ReportTypeId"></param>
        /// <param name="DateFrom"></param>
        /// <param name="DateTo"></param>
        /// <param name="KeyValues"></param>
        /// <param name="FormatId"></param>
        /// <returns></returns>
        private Int64 SaveReportRepositoryInfor(string UserId, string Language, string ReportTypeId, string DateFrom, string DateTo, string KeyValues, string FormatId) {

            Int64 uid = StringToInt64(UserId);
            int rid = (int)StringToInt64(ReportTypeId);
            int fid = (int)StringToInt64(FormatId);

            return SaveReportRepositoryInfor(uid, Language, rid, DateFrom, DateTo, KeyValues, fid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Language"></param>
        /// <param name="ReportTypeId"></param>
        /// <param name="DateFrom"></param>
        /// <param name="DateTo"></param>
        /// <param name="KeyValues"></param>
        /// <param name="FormatId"></param>
        /// <returns></returns>
        private Int64 SaveReportRepositoryInfor(Int64 UserId, string Language, int ReportTypeId, string DateFrom, string DateTo, string KeyValues, int FormatId)
        {
            msMessage = "";
            Int64 ReportRepositiryID = -1;

            try
            {

                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");            // (Language);

                DateTime from = Convert.ToDateTime(DateFrom, ci);
                DateTime to = Convert.ToDateTime(DateTo, ci);
                double Start = from.Subtract(DateTime.Now).TotalMinutes;
                double Period = to.Subtract(from).TotalMinutes;

                ReportRepositiryID = SaveReportRepository(UserId, ReportTypeId, Start, Period, KeyValues, FormatId);

            }
            catch (Exception ex)
            {
                msMessage = ex.Message;
            }
            finally
            {
                if (msMessage != string.Empty)
                    ReportRepositiryID = -1;
            }

            return ReportRepositiryID;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserReportId"></param>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        /// <param name="Start"></param>
        /// <param name="Period"></param>
        /// <param name="CustomProp"></param>
        /// <param name="FormatId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        private Boolean SaveUserReport(Int64 UserReportId, string Name, string Description, double Start, double Period, string CustomProp, int FormatId, Int64 UserId)
        {
            Boolean isSucceed = true;
           // using (SqlConnection connection =
           //new SqlConnection(
           //    ConfigurationManager.ConnectionStrings["InfoStoreConnectionString"].ConnectionString))
           // {
           //     using (SqlCommand sqlComment = new SqlCommand())
           //     {
           //         try
           //         {
           //             sqlComment.CommandType = CommandType.StoredProcedure;
           //             sqlComment.Connection = connection;
           //             sqlComment.CommandText = "AddUserReport";

           //             SqlParameter para = new SqlParameter("UserReportId", SqlDbType.BigInt);
           //             para.Value = UserReportId;
           //             sqlComment.Parameters.Add(para);

           //             para = new SqlParameter("Name", SqlDbType.NVarChar, 50);
           //             para.Value = Name;
           //             sqlComment.Parameters.Add(para);

           //             para = new SqlParameter("Description", SqlDbType.NVarChar, 255);
           //             para.Value = Description;
           //             sqlComment.Parameters.Add(para);

           //             para = new SqlParameter("Start", SqlDbType.Float);
           //             para.Value = Start;
           //             sqlComment.Parameters.Add(para);

           //             para = new SqlParameter("Period", SqlDbType.Float);
           //             para.Value = Period;
           //             sqlComment.Parameters.Add(para);

           //             para = new SqlParameter("CustomProp", SqlDbType.NVarChar, -1);
           //             para.Value = CustomProp;
           //             sqlComment.Parameters.Add(para);

           //             para = new SqlParameter("FormatId", SqlDbType.TinyInt);
           //             para.Value = FormatId;
           //             sqlComment.Parameters.Add(para);

           //             para = new SqlParameter("UserId", SqlDbType.BigInt);
           //             para.Value = UserId;
           //             sqlComment.Parameters.Add(para);

           //             sqlComment.CommandTimeout = 600;
           //             connection.Open();
           //             sqlComment.ExecuteNonQuery();
           //         }
           //         catch (Exception ex)
           //         {
           //             isSucceed = false;
           //         }
           //         if (connection.State == ConnectionState.Open) connection.Close();
           //     }
           // }
            return isSucceed;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReportRepositoryId"></param>
        /// <param name="Msg"></param>
        /// <param name="Path"></param>
        private bool UpdateReportStatus(Int64 ReportRepositoryId, string Msg, string Path)
        {
            try 
            {
	            int rowsAffected = 0;
	
	            VLF.DAS.SQLExecuter sqlExec = new VLF.DAS.SQLExecuter(ConfigurationManager.ConnectionStrings["ActiveStateConnectionString"].ConnectionString);
	            sqlExec.ClearCommandParameters();
	            sqlExec.AddCommandParam("@ReportRepositoryId", SqlDbType.BigInt, ReportRepositoryId);
	            sqlExec.AddCommandParam("@Path", SqlDbType.VarChar, Path);
	            rowsAffected = sqlExec.SPExecuteNonQuery("UpdateReportRepositoryStatus");

                msMessage = "";

            }
            catch (Exception ex){
                msMessage = ex.Message;
            }
            return (string.IsNullOrEmpty(msMessage));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFormat"></param>
        /// <param name="objects"></param>
        private void Log(string strFormat, params object[] objects)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();

                //if any one point of a geozone exist within boundary retrieve it...

                string sqlb = "ReportLog_Add";
                SqlConnection con = new SqlConnection(constr);
                SqlCommand com = new SqlCommand(sqlb, con);
                com.Parameters.Clear();
                com.Parameters.AddWithValue("@LogData", string.Format(strFormat, objects));
                com.CommandType = CommandType.StoredProcedure;
                con.Open();
                com.ExecuteNonQuery();
                con.Close();
            }
            catch
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Process"></param>
        /// <param name="Message"></param>
        /// <param name="InitialTime"></param>
        /// <returns></returns>
        private bool ReportingServiceLogInitial(string Process, string parameters, out string InitialTime)
        {
            msMessage = string.Empty;
            InitialTime = "";           // DateTime.Now;
            string ss = "";
           
            try
            { //string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString()))
                {
                    using (SqlCommand cm = new SqlCommand())
                    {
                        cm.CommandType = CommandType.StoredProcedure;
                        cm.CommandText = "usp_Reportlog_Initial";
                        cm.CommandTimeout = 600;
                        cm.Connection = cn;

                        //SqlParameter para = new SqlParameter("UserId", SqlDbType.Int);      //@UserID    int,
                        //para.Value = UserID;
                        //cm.Parameters.Add(para);

                        SqlParameter para = new SqlParameter("Process", SqlDbType.VarChar, 12);          //@Process   varchar(12),
                        para.Value = Process;
                        cm.Parameters.Add(para);

                        para = new SqlParameter("Data", SqlDbType.VarChar, 1024);           //@Data      varchar(1024),
                        para.Value = CS_WS_SERVER + parameters;
                        cm.Parameters.Add(para);

                        para = new SqlParameter("Status", SqlDbType.VarChar, 12);           //@Status    varchar(12)
                        para.Value = "Initial";
                        cm.Parameters.Add(para);

                        para = new SqlParameter("@RETURN_VALUE", SqlDbType.Int);
                        para.Direction = ParameterDirection.ReturnValue;
                        para.Value = 0;
                        cm.Parameters.Add(para);

                        cm.CommandTimeout = 600;
                        cn.Open();

                        using (SqlDataReader reader = cm.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //InitialTime = StringToDateTime(reader["InitialTime"].ToString());
                                InitialTime = ((DateTime)reader["InitialTime"]).ToString("MM/dd/yyyy HH:mm:ss.fff");
                            }
                            else
                                InitialTime = "";
                        }
                    }
                    if (cn.State == ConnectionState.Open) cn.Close();
                }
            }
            catch (Exception ex)
            {
                msMessage = ex.Message;
            }
            finally {
                if (InitialTime == "" && string.IsNullOrEmpty(msMessage))
                {
                    msMessage = "Failed to initial log.";
                }
            }

            return (string.IsNullOrEmpty(msMessage));

        }

        /// <summary>
        ///  Update Type Index (@Type):
        ///  0: Append Log Data  
        ///  1: Status
        ///  2: File location 
        ///  3: Error  
        ///  4: User ID  
        ///  5: Report ID  
        ///  6: Process ID  
        /// </summary>
        /// <param name="InitialTime"></param>
        /// <param name="Process"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        private bool ReportingServiceLogUpdate(string InitialTime, string data, int UpdateType)
        {
            msMessage = string.Empty;
           
	        try
            { //string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString()))
                {
                    using (SqlCommand cm = new SqlCommand())
                    {
                        cm.CommandType = CommandType.StoredProcedure;
                        cm.CommandText = "usp_Reportlog_Update";
                        cm.CommandTimeout = 600;
                        cm.Connection = cn;

                        SqlParameter para = new SqlParameter("InitialTime", SqlDbType.DateTime);        // @InitialTime	DateTime,
                        para.Value = InitialTime;
                        cm.Parameters.Add(para);

                        para = new SqlParameter("Data", SqlDbType.VarChar, 1024);                       //@Data      varchar(1024),
                        para.Value = data;
                        cm.Parameters.Add(para);

                        para = new SqlParameter("Type", SqlDbType.Int);                               //@Type    int
                        para.Value = UpdateType;
                        cm.Parameters.Add(para);

                        para = new SqlParameter("@RETURN_VALUE", SqlDbType.Int);
                        para.Direction = ParameterDirection.ReturnValue;
                        para.Value = 0;
                        cm.Parameters.Add(para);

                        cn.Open();
                        cm.CommandTimeout = 600;
                        cm.ExecuteNonQuery();
                    }
                    if (cn.State == ConnectionState.Open) cn.Close();
                }
            }
            catch (Exception ex)
            {
                msMessage = ex.Message;
            }
            finally
            {
            }
            return (string.IsNullOrEmpty(msMessage));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatindex"></param>
        /// <returns></returns>
        private string ReportFormatIndexToExtName(int formatindex) {

            string ExtName = "";

            switch (formatindex)
            {
                case 1:
                    ExtName = ".pdf";
                    break;
                case 2:
                    ExtName = ".xls";
                    break;
                case 3:
                    ExtName = ".doc";
                    break;
                case 4:
                    ExtName = ".html";
                    break;
                default:
                    ExtName = ".pdf";
                    break;
            }

            return ExtName;

        }

        #endregion

        #region Assistant functions

        /// <summary>
        /// Get full output path on server
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetOutputPath(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("File name can not be empty!");

            return Path.Combine(Server.MapPath(this.ReportsOutputPath), fileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        private DateTime GetParameterKeyValue_DateTime(string Key)
        {
            if (moParameters.ContainsKey(Key))
                return Convert.ToDateTime(moParameters[Key].ToString().Trim());
            else
                return DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        private string GetParameterKeyValue_String(string Key)
        {
            return GetParameterKeyValue_String(Key, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        private string GetParameterKeyValue_String(string Key, string DefValue)
        {
            if (moParameters.ContainsKey(Key))
                return moParameters[Key].ToString().Trim();
            else
                return DefValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        private Int64 GetParameterKeyValue_Int64(string Key)
        {
            if (moParameters.ContainsKey(Key))
                return StringToInt64(moParameters[Key].ToString());
            else
                return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        private Int64 GetParameterKeyValue_Int64(string Key, Int64 DefValue)
        {
            if (moParameters.ContainsKey(Key))
                return StringToInt64(moParameters[Key].ToString());
            else
                return DefValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        private double GetParameterKeyValue_Double(string Key)
        {
            if (moParameters.ContainsKey(Key))
                return StringToDouble(moParameters[Key].ToString());
            else
                return 0.00;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        private double GetParameterKeyValue_Double(string Key, double DefValue)
        {
            if (moParameters.ContainsKey(Key))
                return StringToDouble(moParameters[Key].ToString());
            else
                return DefValue;
        }

        /// <summary>
        /// Build report name + day and milliseconds
        /// </summary>
        /// <param name="rptName"></param>
        /// <param name="fileExt"></param>
        /// <returns></returns>
        private string GetReportName(string rptName, string fileExt)
        {
            if (String.IsNullOrEmpty(rptName))
                throw new ArgumentNullException("Report name can not be empty!");
            if (String.IsNullOrEmpty(fileExt))
                throw new ArgumentNullException("File extension can not be empty!");
            return String.Format("{0}{1}{2}{3}", rptName, DateTime.Now.Day, DateTime.Now.Millisecond, fileExt);
        }

        /// <summary>
        /// Build report url string on server
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetReportURL(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("File name can not be empty!");
            return String.Format("http://{0}{1}/TmpReports/{2}", ConfigurationManager.AppSettings["ServerIp"], this.ReportsRootPath, fileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool GetRepositoryKeyValue()
        {
            StringBuilder sb = new StringBuilder();
            // Fleet Name
            if (ParameterKeyExists("fleetname"))
                sb.Append(" <b>Fleet</b>=" + GetParameterKeyValue_String("fleetname"));
            // Vehicle Description
            if (ParameterKeyExists("vehiclename"))
                sb.Append(" <b>Unit</b>=" +  GetParameterKeyValue_String("vehiclename"));
            // Driver Name
            if (ParameterKeyExists("drivername"))
                sb.Append(" <b>Driver</b>=" + GetParameterKeyValue_String("drivername"));
            // Landmark Name
            if (ParameterKeyExists("landmarkname"))
                sb.Append(" <b>Landmark</b>=" + GetParameterKeyValue_String("landmarkname"));
            // Geozone Name
            if (ParameterKeyExists("geozone"))
                sb.Append(" <b>Geozone</b>=" + GetParameterKeyValue_String("geozone"));
            // Date-From
            if (ParameterKeyExists("datefrom"))
                sb.Append(" <b>StartDate</b>=" + GetParameterKeyValue_String("datefrom"));
            // Date-Tp
            if (ParameterKeyExists("dateto"))
                sb.Append(" <b>EndDate</b>=" +  GetParameterKeyValue_String("dateto"));
            // Save key values
            this.SetParameterKeyValue_String("keyvalues", sb.ToString());
            // Return
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        private bool IsBoolean(string Value)
        {
            bool flag;
            return (Boolean.TryParse(Value, out flag));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsDateTime(string value)
        {
            bool result = false;

            try
            {
                DateTime dt = DateTime.Parse(value);
                result = true;
            }
            catch
            {
                result = false;

            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool IsNumeric(String str)
        {
            try
            {
                Double.Parse(str, System.Globalization.NumberStyles.Any);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNumericRegex(string text)
        {
            return string.IsNullOrEmpty(text) ? false : Regex.IsMatch(text, @"^\s*\-?\d+(\.\d+)?\s*$"); 
        }

        /// <summary>
        /// Validate fleet id is int.
        /// </summary>
        /// <param name="Fleets"></param>
        /// <returns></returns>
        private bool IsValidFleetID(string Fleets)
        {
            msMessage = "";
            if (Fleets != "0")
            {
                string[] fid = Fleets.Split(',');

                for (int i = 0; i < fid.Length; i++) {
                    if (StringToInt32(fid[i]) <= 0){
                        msMessage = "Invalid Fleet ID[" + fid[i] + "]";
                        i = -i;
                        break;
                    }
                }
            }
            return (string.IsNullOrEmpty(msMessage));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        private string LanguageCodeId(string languageCode)
        {
            if (languageCode.IndexOf("fr") >= 0)
                return "2";
            else
                return "1";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="Query"></param>
        /// <returns></returns>
        private SqlDataReader NewDataReader(string ConnectionString, string Query)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand command = new SqlCommand(Query, connection);

            command.CommandType = CommandType.Text;
            command.CommandTimeout = 600;

            connection.Open();

            return command.ExecuteReader();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        private bool ParameterKeyExists(string Key) {
            return moParameters.ContainsKey(Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool SetParameterKeyValue_Number(string Key, double value)
        {
            return SetParameterKeyValue_String(Key, value.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool SetParameterKeyValue_Ineger(string Key, Int64 value)
        {
            return SetParameterKeyValue_String(Key, value.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool SetParameterKeyValue_String(string Key, string Value) {

            if (moParameters.ContainsKey(Key))
                moParameters[Key] = Value.ToString();
            else
                moParameters.Add(Key, Value.ToString());

            return true;

        }

        /// <summary>
        /// Convert string to double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double StringToDouble(string value)
        {
            double d = 0.00;
            if (Double.TryParse(value, out d))
                return d;
            else
                return 0.00;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Int64 StringToInt64(string value)
        {
            Int64 i = 0;

            if (Int64.TryParse(value, out i))
                return i;
            else
                return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Int32 StringToInt32(string value)
        {
            Int32 i = 0;

            if (Int32.TryParse(value, out i))
                return i;
            else
                return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private DateTime StringToDateTime(string value) {
            return DateTime.Parse(value);
        }

        #endregion

    }

    #region Inner Classes Section

    /// <summary>
    /// Summary description for ReportServerCredential
    /// </summary>
    public class ReportServierCredentials : IReportServerCredentials 
    {
        protected const string USERNAME = "bsmreports";
        protected const string PASSWORD = "T0ybhARQ";
        protected const string DOMAIN = "production";

        protected string username;
        protected string pwd;
        protected string domain;

        public ReportServierCredentials()
        {
            this.username = USERNAME;
            this.pwd = PASSWORD;
            this.domain = DOMAIN;
        }
	public ReportServierCredentials(string UserName, string Password, string Domain)
        {
            this.username = UserName;
            this.pwd = Password;
            this.domain = Domain;
        }

        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;  // Use default identity.
            }
        }

        public System.Net.ICredentials NetworkCredentials
        {
            get
            {
                return new System.Net.NetworkCredential(username, pwd, domain);
            }
        }

        public bool GetFormsCredentials(out System.Net.Cookie authCookie, out string user, out string password, out string authority)
        {
            authCookie = null;
            user = password = authority = null;
            return false;  // Not use forms credentials to authenticate.
        }
    }
    
    #endregion

}
