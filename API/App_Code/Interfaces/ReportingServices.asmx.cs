
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
        
        const string msDTUSFormat = "MM/dd/yyyy";
        const string msDTCAFormat = "dd/MM/yyyy";
        const string msDTGSFormat = "yyyy/MM/dd";

        const string msActiveStateConnectionName = "ActiveStateConnectionString";
        const string msReportServiceConnectionName = "DBReportConnectionString";

        #endregion

        #region Private Variables Section

        string msServerURL;
        string msReportPath;
        string msUserName;
        string msPaasword;
        string msDomain;
        string msMessage;
        string msParameters;
        string msDateTimeFormat;
        string msUserID;
        string msCredentialUID; 
        string msCredentialPWD; 
        string msCredentialDomain;
        string msReportID; 
        string msReportUri; 
        string msReportFormat;
        string msReportFormatCode;
        string msReportName;
        string msLanguage;
        string msDateFrom;
        string msDateTo;
        string msKeyValues;

        string ReportsRootPath = @ConfigurationManager.AppSettings["ReportsRootPath"];
        string ReportsOutputPath = @ConfigurationManager.AppSettings["ReportsOutputPath"];
        string ReportsDataSetPath = @ConfigurationManager.AppSettings["ReportsDataSetPath"];
        string ReportsOutputPathURL = @ConfigurationManager.AppSettings["ReportsOutputPathURL"];

        Int64 miRepositoryID;

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
            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        public ReportingServices(string parameters)
        {
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
        public string ProcessMessage()
        {
            return msMessage;
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

            if (RenderReportPreprocesses())
            {
                //KeyValues: FleetID, Date From, Date To, FormatId:  1=PDF, 2=Excel, 3=Word;
                long RepositoryID = SaveReportRepositoryInfor(msUserID, msLanguage, msReportID, msDateFrom, msDateTo, msKeyValues, msReportFormatCode);

                return RenderServerReport(RepositoryID.ToString());
            }
            else
                return false;
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
            string FileExtName = ReportFormatIndexToExtName(reportFormat);
            
            //LoginManager.GetInstance().SecurityCheck(UserID, SecurityID);

            if (FileExtName == "")
                Log("Render Schedule Server Report error: Invalid Report Format(" + reportFormat.ToString() + ")");
            else {

                Log("Preprocess for rendering scheduled server report");

                msParameters = parameters;

                if (reportFormat == 4)
                {
                    //RenderReport_HTML();
                }
                else
                {
                    if (RenderReportPreprocesses())
                    {
                        SetParameterKeyValue_String("datefrom", dateFrom);
                        SetParameterKeyValue_String("dateto", dateTo);
                        //SetParameterKeyValue_String("reportformat", FileExtName);
                        //SetParameterKeyValue_String("language", language);

                        Log("Begin to render scheduled server report");

                        if (RenderServerReport("", ref reportUrl))
                        {
                            Log("Scheduled server report at " + reportUrl);
                            return true;
                        }
                        else
                        {
                            Log("Failed to render scheduled server report - " + msReportName + ".");
                            return false;
                        }
                    }
                    else {
                        Log("Failed to render scheduled server report. Error: " + msMessage);
                        return false;
                    }
                    //RenderScheduledReport(int UserID, string SecurityID, string dateFrom, string dateTo, string parameters, int reportFormat, string language, ref string reportUrl);
                }

            }

            return true;

        }

        #region Commentted Code for reference

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
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        private void GetUserPreferences(Int32 UserId)
        {

            DataSet dsPref = new DataSet();

            VLF.DAS.Logic.User dbUser = new VLF.DAS.Logic.User(LoginManager.GetConnnectionString(UserId));
            dsPref = dbUser.GetAllUserPreferencesInfo(UserId);
            Int16 PreferenceId = 0;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        private void GetUserPreferences(string UserId)
        {
            GetUserPreferences(StringToInt32(UserId));
        }

        /// <summary>
        /// Sync Render
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="reportid"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        //private bool RenderServerReport(string parameters)
        //{
        //    msParameters = parameters;

        //    if (RenderReportPreprocesses())
        //        return RenderReport();
        //    else
        //        return false;
        //}

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

            try
            {
                DirectoryInfo di = new DirectoryInfo(ReportsOutputPath);

                if (di.Exists)
                    msMessage = "find";
                else
                    msMessage = "Unknown";
            }
            catch (IOException io)
            {
                msMessage = io.Message;
            }
            finally {

                msMessage = "test";
            }

            string UserID = GetParameterKeyValue_String("userid");
            string CredentialUID = GetParameterKeyValue_String("username");
            string CredentialPWD = GetParameterKeyValue_String("password");
            string CredentialDomain = GetParameterKeyValue_String("domain");
            string ReportID = GetParameterKeyValue_String("reportid");
            string ReportUri = GetParameterKeyValue_String("reporturi");
            string ReportPath = GetParameterKeyValue_String("reportpath");
            string ReportFormat = GetParameterKeyValue_String("reportformat");
            string ReportFormatCode = GetParameterKeyValue_String("reportformatcode");
            string ReportName = GetParameterKeyValue_String("reportname").ToString();  
            string Language = GetParameterKeyValue_String("language").ToString();
            string DateFr = GetParameterKeyValue_String("datefrom").ToString();
            string DateTo = GetParameterKeyValue_String("dateto").ToString();
            string KeyValues = GetParameterKeyValue_String("keyvalues").ToString();

            string msg = ">> Server Report (" + msReportID + ")";

            try
            {
                Microsoft.Reporting.WebForms.ServerReport sr = new Microsoft.Reporting.WebForms.ServerReport();

                // credential               
                ReportServierCredentials rsc = new ReportServierCredentials(msCredentialUID, msCredentialPWD, msCredentialDomain);         //("bsmreports", "T0ybhARQ", "production"); 
                sr.ReportServerCredentials = rsc;

                // Server Uri
                sr.ReportServerUrl = new Uri(ReportUri);
                // Report path
                sr.ReportPath = ReportPath;
                
                // Report parameter information
                ReportParameterInfoCollection rp = sr.GetParameters();

                if (ParameterMapping(sr, rp))
                {
                    string mimeType;
                    string encoding;
                    string filenameExtension;
                    string[] streamids;
                    Warning[] warnings;

                    Log(msg + " Rendering........");

                    byte[] bytes;
                    // ReportFormat: "PDF", "EXCEL", "WORD"
                    bytes = sr.Render(ReportFormat, null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

                    Log(msg + "  outputing");

                    string ReportFile = GetReportName(ReportName, "." + filenameExtension);

                    //string uri = this.GetReportURL(ReportFile);

                    if (SaveReportFile(bytes, this.GetOutputPath(ReportFile)))
                    {
                        ReportUrl = this.GetReportURL(ReportFile);

                        Log(msg + " url = " + ReportUrl + ".");

                        if (IsNumeric(RepositoryID))
                        {
                            UpdateReportStatus(StringToInt64(RepositoryID), "", ReportUrl);
                            Log(msg + " update repository process information of " + ReportName + ".");
                        }
                    }
                    else
                    {
                        Log(msg + " failed to save report file " + ReportFile);
                        ReportUrl = "";
                    }
                        
                }
                else
                {
                    if (msMessage == string.Empty)
                        msMessage = "Report parameters mismatch.";
                }
            }
            catch (IOException iox) 
            {
                msMessage = iox.Message;
            }
            catch (ReportServerException rsx)
            {
                msMessage = rsx.Message;
            }
            catch (ReportSecurityException rsx)
            {
                msMessage = rsx.Message;
            }
            catch (ReportViewerException rvx)
            {
                msMessage = rvx.Message;
            }
            catch (Exception ex)
            {
                msMessage = ex.Message;
            }
            finally
            {
                if ((msMessage == "") && (ReportUrl != ""))
                    Log(msg + " rendered successfully.");
                else {
                    ReportUrl = "";
                    Log(msg + " rendering error: " + msMessage);
                }
            }

            return (msMessage == string.Empty) ? true : false;

        }

        /// <summary>
        /// Render scheduled repoerts
        /// </summary>
        /// <returns></returns>
        private bool RenderScheduledServerReport(int UserID, string SecurityID, string dateFrom, string dateTo, string parameters, int reportFormat, string language, ref string reportUrl)
        {
            msMessage = "";

            try
            {
                DirectoryInfo di = new DirectoryInfo(ReportsOutputPath);

                if (di.Exists)
                    msMessage = "find";
                else
                    msMessage = "Unknown";
            }
            catch (IOException io)
            {
                msMessage = io.Message;
            }
            finally
            {

                msMessage = "test";
            }

            try
            {
                //string UserID = GetParameterKeyValue_String("userid");     // moParameters["userid"];
                string CredentialUID = GetParameterKeyValue_String("username");
                string CredentialPWD = GetParameterKeyValue_String("password");
                string CredentialDomain = GetParameterKeyValue_String("domain");
                string ReportID = GetParameterKeyValue_String("reportid");
                string ReportUri = GetParameterKeyValue_String("reporturi");
                string ReportPath = GetParameterKeyValue_String("reportpath");
                string ReportFormat = GetParameterKeyValue_String("reportformat");
                string ReportFormatCode = GetParameterKeyValue_String("reportformatcode");
                string ReportName = GetParameterKeyValue_String("reportname").ToString();          // +Convert.ToInt32(DateTime.Now).ToString();     // +OutputFileExtName(moParameters["reportformat"].ToString());
                string Language = GetParameterKeyValue_String("language").ToString();
                string DateFr = GetParameterKeyValue_String("datefrom").ToString();
                string DateTo = GetParameterKeyValue_String("dateto").ToString();
                string KeyValues = GetParameterKeyValue_String("keyvalues").ToString();

                ////KeyValues: FleetID, Date From, Date To, and so on
                ////FormatId:  1=PDF, 2=Excel, 3=Word
                //long RepositoryID = SaveReportRepositoryInfor(UserID, Language, ReportID, DateFr, DateTo, KeyValues, ReportFormatCode);

                //if (RepositoryID > 0)
                //{

                    ReportServierCredentials rsc = new ReportServierCredentials(CredentialUID, CredentialPWD, CredentialDomain);         //("bsmreports", "T0ybhARQ", "production"); 

                    //Microsoft.Reporting.WebForms.ReportViewer rv = new Microsoft.Reporting.WebForms.ReportViewer();

                    //ReportViewer rv = new ReportViewer();

                    //rv.AsyncRendering = true;
                    //rv.ProcessingMode = ProcessingMode.Remote;

                    Microsoft.Reporting.WebForms.ServerReport sr = new Microsoft.Reporting.WebForms.ServerReport();

                    // credential               
                    sr.ReportServerCredentials = rsc;
                    // Server Uri
                    sr.ReportServerUrl = new Uri(ReportUri);
                    // Report path
                    sr.ReportPath = ReportPath;
                    // Report parameter information
                    ReportParameterInfoCollection rp = sr.GetParameters();

                    // !!! Reset DateFr and DateTo 

                    if (ParameterMapping(sr, rp))
                    {

                        string mimeType;
                        string encoding;
                        string filenameExtension;
                        string[] streamids;
                        Warning[] warnings;

                        Log("<< Server Report Rendering........");

                        byte[] bytes;
                        // ReportFormat: "PDF", "EXCEL", "WORD"
                        bytes = sr.Render(ReportFormat, null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

                        Log("<< Server Report outputing");

                        string ReportFile = GetReportName(ReportName, "." + filenameExtension);

                        //string uri = this.GetReportURL(ReportFile);

                        //if (SaveReportFile(bytes, this.GetOutputPath(ReportFile)))
                        //    UpdateReportStatus(RepositoryID, "", this.GetReportURL(ReportFile));
                    }

                //}
                //else
                //{
                //    if (msMessage == string.Empty)
                //        msMessage = "Create report repository record failed";
                //}
            }
            catch (ReportServerException rsx)
            {
                msMessage = rsx.Message;
            }
            catch (ReportSecurityException rsx)
            {
                msMessage = rsx.Message;
            }
            catch (ReportViewerException rvx)
            {
                msMessage = rvx.Message;
            }
            catch (Exception ex)
            {
                msMessage = ex.Message;
            }
            finally
            {
                if (msMessage == "")
                    Log("<< Server Report Render successfully.");
                else
                    Log("<< Server Report Render error: " + msMessage);
            }

            return (msMessage == string.Empty) ? true : false;

        }

        /// <summary>
        /// 
        /// </summary>
        private void ParameterKeyValueInitial() 
        {
            msUserID = GetParameterKeyValue_String("userid"); 
            msCredentialUID = GetParameterKeyValue_String("username");
            msCredentialPWD = GetParameterKeyValue_String("password");
            msCredentialDomain = GetParameterKeyValue_String("domain");
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="rp"></param>
        /// <returns></returns>
        private bool ParameterMapping(Microsoft.Reporting.WebForms.ServerReport sr, ReportParameterInfoCollection rp)
        {
            msMessage = "";

            try
            {
                #region General Parameters

                // Orgznization ID
                if (rp["OrganizationID"] != null)
                    sr.SetParameters(new ReportParameter("OrganizationID", this.GetParameterKeyValue_String("organization"), false));

                // UserID
                if (rp["UserID"] != null)
                    sr.SetParameters(new ReportParameter("UserID", this.GetParameterKeyValue_String("userid"), false));

                // Fleet ID
                if (rp["FleetID"] != null)
                    sr.SetParameters(new ReportParameter("FleetID", this.GetParameterKeyValue_String("fleetid"), false));

                // Fleet Name
                if (rp["FleetName"] != null)
                    sr.SetParameters(new ReportParameter("FleetName", this.GetParameterKeyValue_String("fleetname"), false));

                // Hierarchy Node Code
                if (rp["NodeCode"] != null)
                    sr.SetParameters(new ReportParameter("NodeCode", this.GetParameterKeyValue_String("NodeCode"), false));

                //From Date
                if (rp["FromDate"] != null)
                {
                    string df = this.GetParameterKeyValue_DateTime("datefrom").ToString("yyyy/MM/dd");
                    sr.SetParameters(new ReportParameter("FromDate", df, false));
                }

                // To Date
                if (rp["ToDate"] != null && (moParameters.ContainsKey("dateto")))
                {
                    string dt = this.GetParameterKeyValue_DateTime("dateto").ToString("yyyy/MM/dd");
                    sr.SetParameters(new ReportParameter("ToDate", dt, false));
                }
                else
                {
                    msMessage = "DateTo parameter mismatch.";
                }

                #endregion
                
                #region User Preferences will be set in reports as internal parameters instead of pass-in from code
                
                // Time Zone
                if (rp["TimeZone"] != null){

                    Log(">> Server Report error: TimeZone Counter. Error: " + rp["TimeZone"].Dependencies.Count.ToString());

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

                #endregion

                #region Speed Violation Reports

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

                if (moParameters.ContainsKey("licenseplate"))
                    Log(">> Server Report - License Plate: " + this.GetParameterKeyValue_String("licenseplate")); 
                else
                    Log(">> Server Report - License Plate: not found."); 


                if ((rp["SensorNumber"] != null) && (moParameters.ContainsKey("sensornumber")))
                    sr.SetParameters(new ReportParameter("SensorNumber", this.GetParameterKeyValue_String("sensornumber"), false));

                if ((rp["LicensePlate"] != null) && (moParameters.ContainsKey("licenseplate")))
                    sr.SetParameters(new ReportParameter("LicensePlate", this.GetParameterKeyValue_String("licenseplate"), false));

                if ((rp["VehicleID"] != null) && (moParameters.ContainsKey("vehicleid")))
                    sr.SetParameters(new ReportParameter("VehicleID", this.GetParameterKeyValue_String("vehicleid"), false));

                if ((rp["VehicleName"] != null) && (moParameters.ContainsKey("vehiclename")))
                    sr.SetParameters(new ReportParameter("VehicleName", this.GetParameterKeyValue_String("vehiclename"), false));

                if ((rp["BoxID"] != null) && (moParameters.ContainsKey("boxid")))
                    sr.SetParameters(new ReportParameter("BoxID", this.GetParameterKeyValue_String("boxid"), false));

            }
            catch (Exception ex) {
                msMessage = ex.Message;
            }
            finally { 
                if (!string.IsNullOrEmpty(msMessage)){
                    Log(">> Server Report error: parameter mapping mismatch. Error: " + msMessage); 
                } 
            }

            return (msMessage == string.Empty);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private bool ParameterParser(string parameters)
        {
            msMessage = "";
            moParameters.Clear();

            string s = parameters.Replace("{", "").Replace("}", "");
            Array a = s.Split(',');
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
                {
                    msMessage = "Invalid parameter syntax: " + parameters;
                    return false;
                }
                else
                {
                    moParameters.Add("keyvalues", RepositoryKeyValue());
                    return true;
                }
            }
            else
            {
                msMessage = "Invalid parameter is null.";
                return false;
            }
        }
        
        /// <summary>
        /// Prepare parameters for rendering report
        /// </summary>
        /// <returns></returns>
        private bool RenderReportPreprocesses()
        {
            if (ParameterParser(msParameters))
            {

                if (moParameters.ContainsKey("userid"))
                {
                    GetUserPreferences(moParameters["userid"].ToString());
                    ParameterKeyValueInitial();

                }

                return true;

            }
            else{
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
        /// <returns></returns>
        private string RepositoryKeyValue()
        {
            StringBuilder sb = new StringBuilder();

            string fn = GetParameterKeyValue_String("fleetname");

            if (fn.Trim() != string.Empty)
                sb.Append(" <b>Fleet</b>=" + fn);

            string vn = GetParameterKeyValue_String("vehiclename");
            if (vn.Trim() != string.Empty)
                sb.Append(" <b>Unit</b>=" + vn);

            string lm = GetParameterKeyValue_String("landmark");
            if (lm.Trim() != string.Empty)
                sb.Append(" <b>Landmark</b>=" + lm);

            string dr = GetParameterKeyValue_String("driver");
            if (dr != string.Empty)
                sb.Append(" <b>Driver</b>=" + dr);

            string gz = GetParameterKeyValue_String("geozone");
            if (gz != string.Empty)
                sb.Append(" <b>Geozone</b>=" + gz);

            //string startDate = txtFrom.SelectedDate.Value.ToString("M/d/yyyy") + " " + String.Format("{0:HH:mm}", cboHoursFrom.SelectedDate.Value);
            string df = GetParameterKeyValue_String("datefrom");
            if (df.Trim() != string.Empty)
                sb.Append(" <b>StartDate</b>=" + df);
            
            //string endDate = txtTo.SelectedDate.Value.ToString("M/d/yyyy") + " " + String.Format("{0:HH:mm}", cboHoursTo.SelectedDate.Value);
            string dt = GetParameterKeyValue_String("dateto");
            if (dt.Trim() != string.Empty)
                sb.Append(" <b>EndDate</b>=" + dt);

            return sb.ToString();

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
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ActiveStateConnectionString"].ConnectionString))
                {
                    using (SqlCommand sqlComment = new SqlCommand())
                    {
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

                        sqlComment.CommandTimeout = 600;
                        sqlComment.ExecuteNonQuery();

                        if (sqlComment.Parameters["@RETURN_VALUE"] != null)
                            isSucceed = Int64.Parse(sqlComment.Parameters["@RETURN_VALUE"].Value.ToString());

                        if (connection.State == ConnectionState.Open) 
                            connection.Close();
                    }
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
            int rowsAffected = 0;
            VLF.DAS.SQLExecuter sqlExec = new VLF.DAS.SQLExecuter(ConfigurationManager.ConnectionStrings["ActiveStateConnectionString"].ConnectionString);
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@ReportRepositoryId", SqlDbType.BigInt, ReportRepositoryId);
            sqlExec.AddCommandParam("@Path", SqlDbType.VarChar, Path);
            rowsAffected = sqlExec.SPExecuteNonQuery("UpdateReportRepositoryStatus");

            return true;
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
                    ExtName = ".htm";
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
            if (moParameters.ContainsKey(Key))
                return moParameters[Key].ToString().Trim();
            else
                return "";
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
