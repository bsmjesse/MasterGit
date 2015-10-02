using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using HOS_DBTableAdapters;
using SentinelFM;
using System.Net;
using System.ComponentModel;
using VLF.ERRSecurity;
using System.Text;
using System.Configuration;
using HOS_WS;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using Telerik.Web.UI;
using System.Data;
using System.Data.SqlClient;

using ClosedXML.Excel;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using Newtonsoft.Json;
using VLF.CLS.Def;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using VLF.DAS.DB;
using VLF.DAS;
using System.Net;
using System.Web.Security;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Drawing;

public partial class frmManagingExtHOS : SentinelFMBasePage
{
    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern bool LogonUser(string lpszUsername, string lpszDomain,
                                        string lpszPassword, int dwLogonType,
                                        int dwLogonProvider, out IntPtr phToken);

    [DllImport("advapi32.dll", SetLastError = true)]
    public extern static bool DuplicateToken(
        IntPtr ExistingTokenHandle, int SECURITY_IMPERSONATION_LEVEL,
        out IntPtr DuplicateTokenHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool CloseHandle(IntPtr hHandle);

    SentinelFMSession sn = null;
    private int vlStart = 0;
    private int vlLimit = 20;
    string fleetId = string.Empty;

    private string operation;
    private string formattype;

    private string hosConnectionString = ConfigurationManager.ConnectionStrings["SentinelHOSConnectionString"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];//sn = (SentinelFMSession)Session["SentinelFMSession"];

        string request = Request.QueryString["QueryType"];

        if (!Page.IsPostBack)
        {
            if (request.Equals("GetDriverList", StringComparison.CurrentCultureIgnoreCase))
            {
                DriverList_Fill();
            }
            else if (request.Equals("GetDriverLogsheetList", StringComparison.CurrentCultureIgnoreCase))
            {
                request = Request.QueryString["start"];
                if (!string.IsNullOrEmpty(request))
                {
                    Int32.TryParse(request, out vlStart);
                    //if (vlStart < 0) vlStart = 0;
                }

                request = Request.QueryString["limit"];
                if (!string.IsNullOrEmpty(request))
                {
                    Int32.TryParse(request, out vlLimit);
                    //if (vlLimit <= 0) vlStart = vlLimit;
                }

                request = Request.QueryString["FleetId"];
                if (!string.IsNullOrEmpty(request))
                {
                    fleetId = request;
                }

                //DateTime from = Convert.ToDateTime("06/06/2013");
                //DateTime to = Convert.ToDateTime("06/28/2013");

                DateTime from = Convert.ToDateTime(Request.QueryString["fromDate"]);
                DateTime to = Convert.ToDateTime(Request.QueryString["toDate"]);
                string drvID = (string)Request.QueryString["driverID"];

                DriverLogsheetList_Fill(from, to, drvID);
            }
            else if (request.Equals("GetDriverInspectionList", StringComparison.CurrentCultureIgnoreCase))
            {
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

                request = Request.QueryString["FleetId"];
                if (!string.IsNullOrEmpty(request))
                {
                    fleetId = request;
                }

                //DateTime from = Convert.ToDateTime("06/06/2013");
                //DateTime to = Convert.ToDateTime("06/28/2013");

                DateTime from = Convert.ToDateTime(Request.QueryString["fromDate"]);
                DateTime to = Convert.ToDateTime(Request.QueryString["toDate"]);
                string drvID = (string)Request.QueryString["driverID"];

                DriverInspectionList_Fill(from, to, drvID);
            }
            else if (request.Equals("DownloadFile", StringComparison.CurrentCultureIgnoreCase))
            {
                string fileName = Request.QueryString["FileName"];
                if (sn.User.OrganizationId == 123)
                {
                    try
                    {
                        string refId = Request.QueryString["RefId"];
                        if (string.IsNullOrEmpty(refId) || refId.ToLower().Equals("undefined"))
                        {
                            refId = null;
                        }
                        string time = Request.QueryString["Time"];
                        if (string.IsNullOrEmpty(time) || time.ToLower().Equals("undefined"))
                        {
                            time = null;
                        }
                        if (!string.IsNullOrEmpty(refId))
                        {
                            string tifFileName = getOCRImagePath(refId, time);
                            if (!string.IsNullOrEmpty(tifFileName) && tifFileName.ToLower().EndsWith(".tif"))
                            {
                                fileName = tifFileName;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                DownloadFile(fileName);
            }

            else if (request.Equals("ExportDriverLogSheet", StringComparison.CurrentCultureIgnoreCase))
            {
                request = Request.QueryString["operation"];
                if (!string.IsNullOrEmpty(request))
                    operation = request;
                else
                    operation = string.Empty;

                request = Request.QueryString["formattype"];
                if (!string.IsNullOrEmpty(request))
                    formattype = request;
                else
                    formattype = string.Empty;

                string drvID = (string)Request.QueryString["driverID"];

                if (operation == "Export" && !string.IsNullOrEmpty(formattype))
                {
                    request = Request.QueryString["columns"];
                    DateTime from = DateTime.ParseExact(Request.QueryString["fromDate"].Substring(0, 10), sn.User.DateFormat.Substring(0, 10), null);
                    DateTime to = DateTime.ParseExact(Request.QueryString["toDate"].Substring(0, 10), sn.User.DateFormat.Substring(0, 10), null);
                    if (!string.IsNullOrEmpty(request))
                    {
                        DataTable dtDS = GetAllDriverLogSheet(from, to, drvID);
                        if (dtDS != null) exportDatatable(dtDS, formattype, request, "LogSheet");
                        return;
                    }
                }
            }

            else if (request.Equals("ExportDriverInspectionSheet", StringComparison.CurrentCultureIgnoreCase))
            {
                request = Request.QueryString["operation"];
                if (!string.IsNullOrEmpty(request))
                    operation = request;
                else
                    operation = string.Empty;

                request = Request.QueryString["formattype"];
                if (!string.IsNullOrEmpty(request))
                    formattype = request;
                else
                    formattype = string.Empty;

                string drvID = (string)Request.QueryString["driverID"];

                if (operation == "Export" && !string.IsNullOrEmpty(formattype))
                {
                    request = Request.QueryString["columns"];
                    DateTime from = DateTime.ParseExact(Request.QueryString["fromDate"].Substring(0,10), sn.User.DateFormat.Substring(0,10), null);
                    DateTime to = DateTime.ParseExact(Request.QueryString["toDate"].Substring(0, 10), sn.User.DateFormat.Substring(0, 10), null);
                    if (!string.IsNullOrEmpty(request))
                    {
                        DataTable dtDS = GetAllDriverInspectionSheet(from, to, drvID);
                        if (dtDS != null) exportDatatable(dtDS, formattype, request, "InspectionSheet");
                        return;
                    }
                }
            }

            else if (request.Equals("MultiDownloadFile", StringComparison.CurrentCultureIgnoreCase))
            {
                string fileName = Request.QueryString["FileName"];
                string headerChecked = Request.QueryString["headerChecked"];
                string buttonName = Request.QueryString["buttonName"];
                printMultiplePDF(fileName, headerChecked, buttonName);
            }

            //var wht = HttpUtility.ParseQueryString(Request.QueryString["fromDate"]);
            //var janina = new Uri(Request.QueryString["fromDate"]).Query;
        }
    }

    private string getOCRImagePath(string refId, string time)
    {
        string filePath = null;

        using (SQLExecuter sql = new SQLExecuter(hosConnectionString))
        {
            VehicleInfo vehicleInfo = new VehicleInfo(sql);
            filePath = ConfigurationManager.AppSettings["RapidLogImageFolder"] + vehicleInfo.GetOCRImagePath(refId, time);
            vehicleInfo = null;
        }
        return filePath;
    }
    
    private void DriverList_Fill()
    {
        string xml = "";
        GetDriversTableAdapter getDrivers = new GetDriversTableAdapter();
        
        DataSet ds = new DataSet();
        ds.Tables.Add(getDrivers.GetDrivers(sn.User.OrganizationId));
        xml = ds.GetXml();
        if (xml == "")
            return;

        Response.ContentType = "text/xml";
        byte[] data = Encoding.Default.GetBytes(xml.Trim());
        xml = Encoding.UTF8.GetString(data);
        Response.Write(xml.Trim());
    }

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

    private void DriverLogsheetList_Fill(DateTime from, DateTime to, string driverId)
    {
        if (string.IsNullOrEmpty(fleetId)) fleetId = sn.User.DefaultFleet.ToString();

        string xml = "";
        //DateTime to = Convert.ToDateTime("06/25/2013");
        //DateTime from = Convert.ToDateTime("06/06/2013");       

        DataSet iDataSet = new DataSet();
        //if (!string.IsNullOrEmpty(driverId))
        //{
        //    GetReportLogSheet_ByDriverTableAdapter logSheetsAdapter_drv = new GetReportLogSheet_ByDriverTableAdapter();
        //    iDataSet.Tables.Add(logSheetsAdapter_drv.GetLogSheets(sn.User.OrganizationId, from, to, driverId));
        //}
        //else
        //{
        //    GetReportLogSheetTableAdapter logSheetsAdapter = new GetReportLogSheetTableAdapter();
        //    iDataSet.Tables.Add(logSheetsAdapter.GetLogSheets(sn.User.OrganizationId, from, to));
        //}

        GetReportLogSheet_ByDriverTableAdapter logSheetsAdapter_drv = new GetReportLogSheet_ByDriverTableAdapter();
        iDataSet.Tables.Add(logSheetsAdapter_drv.GetLogSheets(sn.User.OrganizationId, from, to, driverId));

        DataSet dstemp = new DataSet();
        //DataView dv = iDataSet.Tables[0].DefaultView;
        DataRow[] drLogSheet_Fleet = iDataSet.Tables[0].Select(string.Format("FleetIds LIKE '%,{0},%'", fleetId));
        DataView dv = drLogSheet_Fleet.CopyToDataTable().DefaultView;

        dv.Sort = "RefID ASC";
        DataTable sortedTable = dv.ToTable();
        DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
        dt.TableName = "GetReportLogSheet";
        dstemp.Tables.Add(dt);
        Session["logSheetData"] = dv;
        dstemp.DataSetName = "NewDataSet";
        xml = dstemp.GetXml();
        xml = xml.Replace("<NewDataSet>", "<NewDataSet><totalCount>" + iDataSet.Tables[0].Rows.Count.ToString() + "</totalCount>");
        xml = xml.Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
        //Response.Write(xml);

        //xml = ds.GetXml();
        
        //if (xml == "")
        //{
        //    xml = to.ToString() + " " + from.ToString();
        //    Response.Write(xml.Trim());
        //}
        if (xml == "")
            return;

        Response.ContentType = "text/xml";
        byte[] data = Encoding.Default.GetBytes(xml.Trim());
        xml = Encoding.UTF8.GetString(data);
        //xml.Replace("GetReportLogSheet_ByDriver", "GetReportLogSheet");
        //xml = xml.Replace("GetReportLogSheet_ByDriver", "GetReportLogSheet");
        Response.Write(xml.Trim());
    }

    private void DriverInspectionList_Fill(DateTime from, DateTime to, string driverId)
    {
        if (string.IsNullOrEmpty(fleetId)) fleetId = sn.User.DefaultFleet.ToString();

        string xml = "";
        //DateTime to = Convert.ToDateTime("06/25/2013");
        //DateTime from = Convert.ToDateTime("06/06/2013");       

        DataSet iDataSet = new DataSet();
        //if (!string.IsNullOrEmpty(driverId))
        //{
        //    GetReportLogSheet_ByDriverTableAdapter logSheetsAdapter_drv = new GetReportLogSheet_ByDriverTableAdapter();
        //    iDataSet.Tables.Add(logSheetsAdapter_drv.GetLogSheets(sn.User.OrganizationId, from, to, driverId));
        //}
        //else
        //{
        //    GetReportLogSheetTableAdapter logSheetsAdapter = new GetReportLogSheetTableAdapter();
        //    iDataSet.Tables.Add(logSheetsAdapter.GetLogSheets(sn.User.OrganizationId, from, to));
        //}


        clsHOSManager hosManager = new clsHOSManager();
        //HOS_GetOrganizationInspectionsTableAdapter inspectionsAdapter = new HOS_GetOrganizationInspectionsTableAdapter();
        iDataSet.Tables.Add(hosManager.GetOrganizationInspections(sn.User.OrganizationId, from, to, driverId));



        DataSet dstemp = new DataSet();
        //DataView dv = iDataSet.Tables[0].DefaultView;
        DataRow[] drInspectionSheet_Fleet = iDataSet.Tables[0].Select(string.Format("FleetIds LIKE '%,{0},%'", fleetId));
        DataView dv = null;

        if (drInspectionSheet_Fleet.Length > 0)
        {
            dv = drInspectionSheet_Fleet.CopyToDataTable().DefaultView;
        }
        else {
            Response.ContentType = "text/xml";

            Response.Write("");
            return;
        }

        Session["InspectionSheetData"] = dv;

        dv.Sort = "InsTime ASC";
        DataTable sortedTable = dv.ToTable();
        DataTable dt = new DataTable();
        if (sortedTable.Rows.Count > 0)
        {
            dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
        }
        dt.TableName = "GetReportInspectionsSheet";
        dstemp.Tables.Add(dt);
        dstemp.DataSetName = "NewDataSet";
        xml = dstemp.GetXml();
        xml = xml.Replace("<NewDataSet>", "<NewDataSet><totalCount>" + iDataSet.Tables[0].Rows.Count.ToString() + "</totalCount>");
        xml = xml.Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
        //Response.Write(xml);

        //xml = ds.GetXml();

        //if (xml == "")
        //{
        //    xml = to.ToString() + " " + from.ToString();
        //    Response.Write(xml.Trim());
        //}
        if (xml == "")
            return;

        Response.ContentType = "text/xml";
        byte[] data = Encoding.Default.GetBytes(xml.Trim());
        xml = Encoding.UTF8.GetString(data);
        //xml.Replace("GetReportLogSheet_ByDriver", "GetReportLogSheet");
        //xml = xml.Replace("GetReportLogSheet_ByDriver", "GetReportLogSheet");
        Response.Write(xml.Trim());
    }

    byte[] ConvertToJPG(byte[] srcByte)
    {
        if (srcByte == null)
        {
            return null;
        }
        byte[] ret = null;

        using (MemoryStream ms = new MemoryStream(srcByte))
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromStream(ms))
            {

                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float maxWidth = 1200;
                float maxHeight = 1000;

                // To preserve the aspect ratio
                float ratioX = (float)maxWidth / (float)originalWidth;
                float ratioY = (float)maxHeight / (float)originalHeight;
                float ratio = Math.Min(ratioX, ratioY);

                // New width and height based on aspect ratio
                int newWidth = (int)(originalWidth * ratio);
                int newHeight = (int)(originalHeight * ratio);

                // Convert other formats (including CMYK) to RGB.
                Bitmap newImage = new Bitmap(newWidth, newHeight);
                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);

                    using (MemoryStream msDest = new MemoryStream())
                    {
                        newImage.Save(msDest, System.Drawing.Imaging.ImageFormat.Jpeg);
                        ret = msDest.ToArray();
                    }
                }
            }
        }

        return ret;
    }

    private void DownloadFile(string filepath)
    {
        //filepath = @"\201306\dbcabcf9-b4a0-473c-8b23-c4489158bf45.pdf";
        try
        {
            byte[] pdfData = null;

            if (filepath != null && filepath.ToLower().EndsWith("tif"))
            {
                IntPtr hToken = IntPtr.Zero;
                IntPtr hTokenDuplicate = IntPtr.Zero;
                WindowsImpersonationContext impersonationContext = null;
                try
                {
                    string[] user = ConfigurationManager.AppSettings["RapidLogUser"].Split('\\');
                    string domain = user[0];
                    string username = user[1];
                    string password = ConfigurationManager.AppSettings["RapidLogPassword"];

                    const int LOGON_TYPE_NEW_CREDENTIALS = 9;
                    const int LOGON32_PROVIDER_WINNT50 = 3;
                    if (LogonUser(username, domain, password,
                                  LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, out hToken))
                    {
                        if (DuplicateToken(hToken, 2, out hTokenDuplicate))
                        {
                            WindowsIdentity windowsIdentity = new WindowsIdentity(hTokenDuplicate);
                            impersonationContext =
                                windowsIdentity.Impersonate();

                            string ImagePath = null;
                            if (File.Exists(filepath))
                            {
                                ImagePath = filepath;
                            }
                            else
                            {
                                ImagePath = ConfigurationManager.AppSettings["RapidLogImageFolder"] + filepath;
                            }
                            if (File.Exists(ImagePath))
                            {
                                using (FileStream fs = File.OpenRead(ImagePath))
                                {
                                    pdfData = new byte[fs.Length];

                                    if (pdfData != null)
                                    {
                                        fs.Read(pdfData, 0, Convert.ToInt32(fs.Length));
                                        pdfData = ConvertToJPG(pdfData);

                                        Response.Buffer = false; //transmitfile self buffers
                                        Response.Clear();
                                        Response.ClearContent();
                                        Response.ClearHeaders();
                                        Response.ContentType = "image/jpeg";
                                        Response.OutputStream.Write(pdfData, 0, pdfData.Length);
                                        Response.Flush();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw (e);
                }
                finally
                {
                    impersonationContext.Undo();
                    if (hToken != IntPtr.Zero) CloseHandle(hToken);
                    if (hTokenDuplicate != IntPtr.Zero) CloseHandle(hTokenDuplicate);
                }
            }
            else
            {
                HOSFileReteriver ws = new HOSFileReteriver();
                pdfData = ws.ReadFile(filepath);

                if (pdfData != null)
                {
                    Response.Buffer = false; //transmitfile self buffers
                    Response.Clear();
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.ContentType = "application/pdf";
                    Response.OutputStream.Write(pdfData, 0, pdfData.Length);
                    Response.Flush();
                }
            }
        }
        catch { }
    }

    private void printMultiplePDF(string filename, string header, string buttonName)
    {
        if (header == "true" && filename.Length == 0)
          {
            DataView dv = new DataView();
            DataSet newDS = new DataSet();
            if(buttonName== "Logsheet")
            {
            dv = (DataView)Session["logSheetData"];
                
            }
            else
                dv = (DataView)Session["InspectionSheetData"];
            DataTable sortedTable = dv.ToTable();
            DataTable dt = new DataTable();
            if (sortedTable.Rows.Count > 0)
            {
                dt = sortedTable.AsEnumerable().CopyToDataTable();
            }
            dt.TableName = "GetFullSheet";
            newDS.Tables.Add(dt);
            if(newDS.Tables.Count > 0)
            {
                List<string> filenames = new List<string>();

                for (int i = 0; i < newDS.Tables[0].Rows.Count; i++)
                {
                    filenames.Add(newDS.Tables[0].Rows[i]["filename"].ToString());
                }
            }

            foreach (DataRow dr in dt.Rows)
            {
                 //string str = "";
                filename = filename + "," + Convert.ToString(dr["filename"]);

            }
          }       
            HOSFileReteriver ws = new HOSFileReteriver();
            List<string> myCollection = new List<string>();
            string path = "MergeReports";
            string tempDirectory = "";
            string[] split = filename.Split(',');
            foreach (string item in split)
            {
                try
                {
                    byte[] pdfData = ws.ReadFile(item);
                    string filenameWithoutPath = Path.GetFileName(item);

                    string directoryPath = Server.MapPath(string.Format("~/{0}", item.Trim()));

                    tempDirectory = Server.MapPath(path);

                    FileInfo fileInfo = new FileInfo(directoryPath);
                    string correctDirectoryFullPath = fileInfo.DirectoryName;

                    if (!Directory.Exists(tempDirectory))
                    {
                        Directory.CreateDirectory(tempDirectory);
                    }
                    else
                    {
                    }
                    if (pdfData != null)
                    {
                        FileStream fs = new FileStream(tempDirectory + "\\" + filenameWithoutPath, FileMode.OpenOrCreate);
                        fs.Write(pdfData, 0, pdfData.Length);
                        fs.Close();

                        myCollection.Add(tempDirectory + "\\" + filenameWithoutPath);
                    }

                }
                catch (Exception ex)
                {

                }
            }
            MergePdfFiles(myCollection, tempDirectory);
       
    }

    protected void MergePdfFiles(IEnumerable<string> pdfsToMerge, string saveMergedFileTo)
    {
        if (pdfsToMerge.Count() > 0)
        {

            PdfDocument document = new PdfDocument();
            PdfPage page1 = document.AddPage();
            string filename = Guid.NewGuid().ToString() + ".pdf";
            document.Save(saveMergedFileTo + "\\" + filename);

            var outputPdfDocument = new PdfDocument();
            foreach (string pdfFile in pdfsToMerge)
            {
                PdfDocument inputPdfDocument = PdfReader.Open(pdfFile, PdfDocumentOpenMode.Import);
                outputPdfDocument.Version = inputPdfDocument.Version;
                foreach (PdfPage page in inputPdfDocument.Pages)
                {
                    outputPdfDocument.AddPage(page);
                }
            }
            outputPdfDocument.Save(saveMergedFileTo + "\\" + filename);
            byte[] outputDoc = File.ReadAllBytes(@saveMergedFileTo + "\\" + filename);

            if (outputDoc != null)
            {
                Response.Buffer = false; //transmitfile self buffers
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.ContentType = "application/pdf";
                Response.OutputStream.Write(outputDoc, 0, outputDoc.Length);
                Response.Flush();
            }
            DirectoryInfo dir = new DirectoryInfo(saveMergedFileTo);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }
        }
    }


    // eXport functionalities
    private DataTable GetAllDriverLogSheet(DateTime from, DateTime to, string driverId)
    {
        if (string.IsNullOrEmpty(fleetId)) fleetId = sn.User.DefaultFleet.ToString();
        if (string.IsNullOrEmpty(driverId)) driverId = string.Empty;

        DataTable dt;
        try
        {
            DataSet iDataSet = new DataSet();

            GetReportLogSheet_ByDriverTableAdapter logSheetsAdapter_drv = new GetReportLogSheet_ByDriverTableAdapter();
            iDataSet.Tables.Add(logSheetsAdapter_drv.GetLogSheets(sn.User.OrganizationId, from, to, driverId));

            DataSet dstemp = new DataSet();
            //DataView dv = iDataSet.Tables[0].DefaultView;
            DataRow[] drLogSheet_Fleet = iDataSet.Tables[0].Select(string.Format("FleetIds LIKE '%,{0},%'", fleetId));
            DataView dv = drLogSheet_Fleet.CopyToDataTable().DefaultView;

            dv.Sort = "RefID ASC";
            dt = dv.ToTable();
            dt.TableName = "GetAllDriverLogSheet";
        }
        catch
        {
            return null;
        }
        return dt;
    }

    private DataTable GetAllDriverInspectionSheet(DateTime from, DateTime to, string driverId)
    {
        if (string.IsNullOrEmpty(fleetId)) fleetId = sn.User.DefaultFleet.ToString();
        if (string.IsNullOrEmpty(driverId)) driverId = string.Empty;

        DataTable dt;
        try
        {
            DataSet iDataSet = new DataSet();

            //HOS_GetOrganizationInspectionsTableAdapter inspectionsAdapter = new HOS_GetOrganizationInspectionsTableAdapter();
            clsHOSManager hosManager = new clsHOSManager();
            iDataSet.Tables.Add(hosManager.GetOrganizationInspections(sn.User.OrganizationId, from, to, driverId));

            DataSet dstemp = new DataSet();
            //DataView dv = iDataSet.Tables[0].DefaultView;
            DataRow[] drInspectionSheet_Fleet = iDataSet.Tables[0].Select(string.Format("FleetIds LIKE '%,{0},%'", fleetId));
            DataView dv = drInspectionSheet_Fleet.CopyToDataTable().DefaultView;

            dv.Sort = "InsTime ASC";
            dt = dv.ToTable();
            string gv;
            foreach (DataRow dr in dt.Rows)
            {
                //gv = Convert.ToString(dr["trip"]);
                dr["trip"] = getTripStr(Convert.ToString(dr["trip"]));
            }
            dt.AcceptChanges();
            dt.TableName = "GetAllDriverInspectionSheet";
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            return null;
        }
        return dt;
    }

    private String FindSplitValue(string txt, char searchChr, int index)
    {
        String ret = "";
        int i_pos = txt.LastIndexOf(searchChr);
        if (i_pos > 0)
        {
            if (index == 0)
              return txt.Substring(0, i_pos);
            else 
            {
                if (txt.Length > i_pos) return txt.Substring(i_pos + 1);
            }
        }
        return ret; 
    }

    private void exportDatatable(DataTable dt, string formatter, string columns, string fname)
    {
        try
        {
            if (formatter == "csv")
            {
                System.Text.StringBuilder sresult = new System.Text.StringBuilder();
                sresult.Append("sep=,");
                sresult.Append(Environment.NewLine);
                string header = string.Empty;
                foreach (string column in columns.Split(','))
                {
                    string s = FindSplitValue(column, ':', 0);// column.Split(':')[0];

                    header += "\"" + s + "\",";
                }
                header = header.Substring(0, header.Length - 1);
                sresult.Append(header);
                sresult.Append(Environment.NewLine);

                foreach (DataRow row in dt.Rows)
                {
                    string data = string.Empty;
                    foreach (string column in columns.Split(','))
                    {
                        string s = row[FindSplitValue(column, ':', 1)].ToString(); //row[column.Split(':')[1]].ToString();
                        if (FindSplitValue(column, ':', 1) == "LastUpdate")
                            s = Convert.ToDateTime(s).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                        data += "\"" + s.Replace("[br]", Environment.NewLine).Replace("\"", "\"\"") + "\",";
                    }
                    data = data.Substring(0, data.Length - 1);
                    sresult.Append(data);
                    sresult.Append(Environment.NewLine);
                }

                Response.Clear();
                Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.csv", fname));
                Response.Charset = System.Text.Encoding.GetEncoding("iso-8859-1").BodyName;
                Response.ContentType = "application/csv";
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("iso-8859-1");
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
                foreach (string column in columns.Split(','))
                {
                    string s = FindSplitValue(column, ':', 0);//column.Split(':')[0];
                    row.CreateCell(row.Cells.Count).SetCellValue(s);
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string data = string.Empty;
                    IRow rowData = ws.CreateRow(i + 1);
                    foreach (string column in columns.Split(','))
                    {
                        if (FindSplitValue(column, ':', 1) == "LastUpdate")
                        {
                            DateTime currentDate = DateTime.Now.ToUniversalTime();
                            //DateTime recordDate;

                            string datadate = Convert.ToDateTime(dt.Rows[i][FindSplitValue(column, ':', 1)].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                            rowData.CreateCell(rowData.Cells.Count).SetCellValue(datadate.Replace("[br]", Environment.NewLine));
                            //recordDate = DateTime.ParseExact(datadate, sn.User.DateFormat + " " + sn.User.TimeFormat, System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();

                            //TimeSpan diffDate = currentDate.Subtract(recordDate);

                            //if (diffDate.TotalHours < 24)
                            //{
                            //    cellstyle1.WrapText = true;
                            //    cellstyle1.VerticalAlignment = VerticalAlignment.TOP;
                            //    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle1;
                            //}
                            //else if (diffDate.TotalHours < 48)
                            //{
                            //    cellstyle2.WrapText = true;
                            //    cellstyle2.VerticalAlignment = VerticalAlignment.TOP;
                            //    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle2;
                            //}
                            //else if (diffDate.TotalHours < 72)
                            //{
                            //    cellstyle3.WrapText = true;
                            //    cellstyle3.VerticalAlignment = VerticalAlignment.TOP;
                            //    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle3;
                            //}
                            //else if (diffDate.TotalHours < 168)
                            //{
                            //    cellstyle4.WrapText = true;
                            //    cellstyle4.VerticalAlignment = VerticalAlignment.TOP;
                            //    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle4;
                            //}
                            //else if (diffDate.TotalHours > 168)
                            //{
                            //    cellstyle5.WrapText = true;
                            //    cellstyle5.VerticalAlignment = VerticalAlignment.TOP;
                            //    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle5;
                            //}

                        }
                        else
                            rowData.CreateCell(rowData.Cells.Count).SetCellValue(dt.Rows[i][FindSplitValue(column, ':', 1)].ToString().Replace("[br]", Environment.NewLine));

                    }
                }

                for (int i = 0; i < columns.Split(',').Length; i++)
                {
                    try
                    {
                        ws.AutoSizeColumn(i);
                    }
                    catch { }
                }

                HttpResponse httpResponse = Response;
                httpResponse.Clear();
                //Response.AddHeader("Content-Type", "application/Excel");
                //httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //httpResponse.AddHeader("content-disposition", string.Format(@"attachment;filename={0}.xls", fname));

                Response.AddHeader("Content-Type", "application/Excel");
                Response.ContentType = "application/vnd.xls";
                HttpContext.Current.Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xls", fname));

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
                    foreach (string column in columns.Split(','))
                    {
                        string s = FindSplitValue(column, ':', 0); //column.Split(':')[0];
                        ws.Cell(1, ws.Row(1).CellsUsed().Count() + 1).Value = s;
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string data = string.Empty;
                        int iColumn = 1;
                        foreach (string column in columns.Split(','))
                        {
                            ws.Cell(i + 2, iColumn).DataType = XLCellValues.Text;

                            if (FindSplitValue(column, ':', 1) == "LastUpdate")
                            {
                                DateTime currentDate = DateTime.Now.ToUniversalTime();
                                //DateTime recordDate;


                                string datadate = Convert.ToDateTime(dt.Rows[i][FindSplitValue(column, ':', 1)].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                                ws.Cell(i + 2, iColumn).Value = "'" + datadate.Replace("[br]", Environment.NewLine);
                                //recordDate = DateTime.ParseExact(datadate, sn.User.DateFormat + " " + sn.User.TimeFormat, System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();

                                //TimeSpan diffDate = currentDate.Subtract(recordDate);

                                //if (diffDate.TotalHours < 24)
                                //{
                                //    ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#7BB273");
                                //}
                                //else if (diffDate.TotalHours < 48)
                                //{
                                //    ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#EFD700");
                                //}
                                //else if (diffDate.TotalHours < 72)
                                //{
                                //    ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFA64A");
                                //}
                                //else if (diffDate.TotalHours < 168)
                                //{
                                //    ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#DE7973");
                                //}
                                //else if (diffDate.TotalHours > 168)
                                //{
                                //    ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#637DA5");
                                //}

                            }
                            else
                                ws.Cell(i + 2, iColumn).Value = "'" + dt.Rows[i][FindSplitValue(column, ':', 1)].ToString().Replace("[br]", Environment.NewLine);

                            iColumn++;
                        }
                    }

                    //ws.Rows().Style.Alignment.SetWrapText();
                    //ws.Rows().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    //ws.Columns().AdjustToContents();

                    try
                    {
                        var files = new System.IO.DirectoryInfo(Server.MapPath("TempReports/")).GetFiles("*.xlsx");
                        foreach (var file in files)
                        {
                            if (DateTime.UtcNow - file.LastWriteTimeUtc > TimeSpan.FromDays(30))
                            {
                                System.IO.File.Delete(file.FullName);
                            }
                        }
                    }
                    catch { }

                    Response.Clear();
                    //Response.AddHeader("Content-Type", "application/Excel");
                    //Response.ContentType = "application/force-download";
                    //Response.AddHeader("content-disposition", string.Format(@"attachment;filename={0}.xlsx", fname));
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Current.Response.AddHeader("content-disposition", string.Format(@"attachment;filename={0}.xlsx", fname));
                    
                    //string filemame = string.Format(@"{0}.xlsx", Guid.NewGuid());
                    //wb.SaveAs(Server.MapPath("TempReports/") + filemame);
                    //Response.TransmitFile("TempReports/" + filemame);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        wb.SaveAs(memoryStream);
                        memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                        memoryStream.Close();
                    }

                    HttpContext.Current.Response.End();
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

    private string getTripStr(string input)
    {
        var arr = input.Split(',');
        var result = "";
        for(int i=0;i<arr.Length;i++) 
        {
            var arrN = arr[i].Split('#');
            if (arrN.Length == 4)
            {
                result += arrN[0] + " " + arrN[1];
            }
        }
        return result;
    }
}
