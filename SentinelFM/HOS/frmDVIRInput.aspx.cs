using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using HOS_DBTableAdapters;
using VLF.DAS.DB;
using VLF.DAS;
using VLF.DAS.Logic;
using System.Configuration;
using System.Web.Script;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web.Security;
using System.Security.Principal;
using System.Runtime.InteropServices;

public partial class HOS_frmDVIRInput : SentinelFMBasePage
{
    static string sFMConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
    static string sHOSConnectionString = ConfigurationManager.ConnectionStrings["SentinelHOSConnectionString"].ConnectionString;

    private static string defaultImageFolder;
    private static string rapidlogImageFolder;

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


    private static string FTPInspectionRoot
    {
        get
        {
            return ConfigurationManager.AppSettings["RapidLogFTPInspectionRoot"];
        }
    }

    private static string FTPSite
    {
        get
        {
            return ConfigurationManager.AppSettings["RapidLogFTPSite"];
        }
    }

    private static string FTPUser
    {
        get
        {
            return ConfigurationManager.AppSettings["RapidLogFTPUser"];
        }
    }

    private static string FTPPassword
    {
        get
        {
            return ConfigurationManager.AppSettings["RapidLogFTPPassword"];
        }
    }

    private static string getFTPUrl(string url)
    {
        url = string.Format("ftp://{0}:{1}@" + FTPSite + FTPInspectionRoot + "/" + url, FTPUser, HttpUtility.UrlEncode(FTPPassword));
        return url;
    }


    public static string DefaultImageFolder
    {
        get
        {
            if (string.IsNullOrEmpty(defaultImageFolder))
            {
                defaultImageFolder = ConfigurationManager.AppSettings["InspectionImageUpload"];
            }
            return defaultImageFolder;
        }
        set
        {
            defaultImageFolder = value;
        }
    }

    public static string RapidLogImageFolder
    {
        get
        {
            if (string.IsNullOrEmpty(rapidlogImageFolder))
            {
                rapidlogImageFolder = ConfigurationManager.AppSettings["RapidLogImageFolder"];
            }
            return rapidlogImageFolder;
        }
        set
        {
            defaultImageFolder = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
        }
        string fileName = Request.QueryString["fileName"];
        this.Title = fileName;
    }

    private static DateTime getDateTime(string date, string time)
    {
        string s = date + " " + time;
        DateTime dt = DateTime.ParseExact(s, "d/M/yyyy HH:mm", CultureInfo.InvariantCulture);

        return dt;
    }

    private static int getInt(object value)
    {
        int ret;
        if (value == null)
        {
            ret = -1;
        }
        else
        {
            if (!int.TryParse(value.ToString(), out ret))
            {
                ret = 0;
            }
        }
        return ret;
    }

    private static bool getBoolean(object value)
    {
        bool ret = false;
        if (value == null)
        {
            ret = false;
        }
        else
        {
            if (value.ToString().Equals("1"))
            {
                ret = true;
            }
        }
        return ret;
    }

    private static string getString(object obj)
    {
        if (obj == null)
        {
            return null;
        }
        else
        {
            return obj.ToString();
        }
    }

    private static string getEquipId(object vehicleId, object insType)
    {
        if (vehicleId == null || insType == null)
        {
            return null;
        }
        else
        {
            return vehicleId.ToString() + "@@@@" + insType.ToString() + ",";
        }
    }

    private static string getDefect(ArrayList inspection)
    {
        object[] objects = inspection.ToArray();
        return string.Join(",", Array.ConvertAll(objects, p => (p ?? String.Empty).ToString()));
    }

    private static void ImportInspectionImage(string folder, string fileName, string fileDestPath, string fileDestName)
    {
        string source = DefaultImageFolder + @"\" + folder + @"\" + fileName;
        string destFolder = RapidLogImageFolder + @"\" + fileDestPath;
        string destPath = destFolder + @"\" + fileDestName;

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

                    if (!Directory.Exists(destFolder))
                    {
                        Directory.CreateDirectory(destFolder);
                    }

                    string url = getFTPUrl(folder + "/" + fileName);
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                    request.UseBinary = true;
                    request.KeepAlive = false;
                    request.Method = WebRequestMethods.Ftp.DownloadFile;

                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    FileStream writer = new FileStream(destPath, FileMode.Create);

                    long length = response.ContentLength;
                    int bufferSize = 2048;
                    int readCount;
                    byte[] buffer = new byte[2048];

                    readCount = responseStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        writer.Write(buffer, 0, readCount);
                        readCount = responseStream.Read(buffer, 0, bufferSize);
                    }

                    responseStream.Close();
                    response.Close();
                    writer.Close();

                    request = (FtpWebRequest)WebRequest.Create(url);
                    request.Method = WebRequestMethods.Ftp.DeleteFile;

                    response = (FtpWebResponse)request.GetResponse();
                    response.Close();

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

    /*
        private static void ImportInspectionImage(string folder, string fileName, string fileDestPath, string fileDestName)
        {
            string source = DefaultImageFolder + @"\" + folder + @"\" + fileName;
            string destFolder = RapidLogImageFolder + @"\" + fileDestPath;
            string destPath = destFolder + @"\" + fileDestName;

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

                        if (!Directory.Exists(destFolder))
                        {
                            Directory.CreateDirectory(destFolder);
                        }

                        File.Copy(source, destPath);
                        File.Delete(source);
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
    */
    private static object getValue(Dictionary<string, object> inspectionInfo, string key)
    {
        if (inspectionInfo.Keys.Contains(key))
        {
            return inspectionInfo[key];
        }
        else
        {
            return null;
        }
    }

    private static void SaveInspectionData(Dictionary<string, object> inspectionInfo)
    {
        SQLExecuter sql = new SQLExecuter(sHOSConnectionString);
        VehicleInfo vehicleInfo = new VehicleInfo(sql);

        string inspectionGroupId = getString(inspectionInfo["inspectionGroup"]);
        DateTime insDatetime = getDateTime(inspectionInfo["insDate"].ToString(), inspectionInfo["insTime"].ToString());
        string equipId = getEquipId(inspectionInfo["vehicleId"], inspectionInfo["insType"]);
        string defect = getDefect((ArrayList)inspectionInfo["insItems"]);
        string folder = inspectionInfo["folder"].ToString();
        string fileName = inspectionInfo["fileName"].ToString();
        string driverId = getString(inspectionInfo["driverId"]);
        string vehicleId = getString(inspectionInfo["vehicleId"]);

        string fileDestPath = insDatetime.Year.ToString("0000") + insDatetime.Month.ToString("00");
        string fileDestName = driverId + "_" + insDatetime.ToString("yyyyMMdd_HHmm") + "_" + vehicleId + ".tif";

        vehicleInfo.SaveInspectionCN(
            insDatetime,
            equipId,
            getInt(getValue(inspectionInfo, "odometer")),
            defect,
            driverId,
            getBoolean(getValue(inspectionInfo, "signed")),
            getInt(getValue(inspectionInfo, "trailerId")),
            getBoolean(getValue(inspectionInfo, "satisfied")),
            getBoolean(getValue(inspectionInfo, "defectsCorrected")),
            getBoolean(getValue(inspectionInfo, "defectsSignedRepairer")),
            getBoolean(getValue(inspectionInfo, "defectsSignedDriver")),
            getString(getValue(inspectionInfo, "remark")),
            @"\" + fileDestPath + @"\" + fileDestName,
            getInt(inspectionGroupId)
        );

        vehicleInfo = null;
        ImportInspectionImage(folder, fileName, fileDestPath, fileDestName);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = false, XmlSerializeString = false)]
    public static void SaveInspection(object obj)
    {
        var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        Dictionary<string, object> inspectionInfo = serializer.Deserialize<Dictionary<string, object>>(obj.ToString());

        SaveInspectionData(inspectionInfo);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = true, XmlSerializeString = false)]
    public static string existingInsepection(string driverId, string date, string dateFormat, string time)
    {
        string[] stime = time.Split(':');
        int hour = 0;
        int minute= 0;
        DateTime insDateTime;

        if(!int.TryParse(stime[0],out hour)){
            throw new Exception("Inspection time is not valid");
        }
        if(!int.TryParse(stime[1],out minute)){
            throw new Exception("Inspection time is not valid");
        }
        try{
            insDateTime = DateTime.ParseExact(date, dateFormat, CultureInfo.InvariantCulture);
            insDateTime.AddHours(hour);
            insDateTime.AddMinutes(minute);
        }catch(Exception ex){
            throw new Exception("Inspection date is not valid");
        }

        return checkExistingInspection(driverId, insDateTime);
    }

    private static string checkExistingInspection(string driverId, DateTime insDateTime)
    {
        SQLExecuter sql = new SQLExecuter(sHOSConnectionString);
        VehicleInfo vehicleInfo = new VehicleInfo(sql);

        return vehicleInfo.checkPPCID(driverId, insDateTime);
    }


    //Created for customer CN
    [System.Web.Services.WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = true, XmlSerializeString = false)]
    public static ArrayList GetDrivers()
    {
        ArrayList ret = null;

        int companyId = 123;
        ret = GetDriverList(companyId);
        return ret;
    }

    private static ArrayList GetDriverList(int companyId)
    {
        ArrayList ret = new ArrayList();
        DataSet dsDrivers = new DataSet();

        try
        {
            using (ContactManager contactMsg = new ContactManager(sHOSConnectionString))
            {
                dsDrivers = contactMsg.GetDriversEmployeeInfo(companyId);

                foreach (DataRow dtRow in dsDrivers.Tables[0].Rows)
                {
                    ret.Add(dtRow.ItemArray);
                }
            }
        }
        catch (Exception Ex)
        {
        }
        dsDrivers = null;
        return ret;
    }

    //Created for customer CN
    [System.Web.Services.WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = true, XmlSerializeString = false)]
    public static ArrayList GetInspectionItems(int groupId)
    {
        ArrayList ret = null;

        ret = GetInspectionItemList(groupId);
        return ret;
    }

    public static ArrayList GetInspectionItemList(int groupId)
    {
        ArrayList ret = new ArrayList();
        DataSet ds = new DataSet();

        try
        {
            using (SQLExecuter sql = new SQLExecuter(sHOSConnectionString))
            {
                VehicleInfo vehicleInfo = new VehicleInfo(sql);
                ds = vehicleInfo.GetInspectionItemsByGroup(groupId);

                foreach (DataRow dtRow in ds.Tables[0].Rows)
                {
                    ret.Add(dtRow.ItemArray);
                }

                vehicleInfo = null;
            }
        }
        catch (Exception Ex)
        {
        }

        return ret;

    }

    //Created for customer CN
    [System.Web.Services.WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = true, XmlSerializeString = false)]
    public static ArrayList GetVehicles(int type)
    {
        ArrayList ret = null;

        ret = GetVehicleList(type);
        return ret;
    }


    public static ArrayList GetVehicleList(int type)
    {
        ArrayList ret = new ArrayList();
        DataSet ds = new DataSet();

        try
        {
            using (SQLExecuter sql = new SQLExecuter(sFMConnectionString))
            {
                VehicleInfo vehicleInfo = new VehicleInfo(sql);
                ds = vehicleInfo.GetCNVehicleInfoByType(type);

                foreach (DataRow dtRow in ds.Tables[0].Rows)
                {
                    ret.Add(dtRow.ItemArray);
                }
                vehicleInfo = null;
            }

        }
        catch (Exception Ex)
        {
        }

        return ret;

    }

    private class Driver
    {
        public int ID { get; set; }
        public int DriverID { get; set; }
        public string Name { get; set; }
    }

}