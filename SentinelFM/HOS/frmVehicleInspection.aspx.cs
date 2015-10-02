using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;
using System.Web.Script.Services;
using System.IO;
using SentinelFM;
using System.Net;
using System.Web.Security;
using System.Security.Principal;
using System.Runtime.InteropServices;

public partial class HOS_frmVehicleInspection : SentinelFMBasePage
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

    private static string defaultImageFolder;

    public static string DefaultImageFolder
    {
        get
        {
            if(string.IsNullOrEmpty(defaultImageFolder))
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

    [System.Web.Services.WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = true, XmlSerializeString = false)]
    public static List<Item> GetUploadFolderTree()
    {
        List<Item> nodes = new List<Item>();
        string directory = "";
        int id = 0;
        try
        {
            nodes = getSubFolders(directory, ref id);
        }
        catch (Exception e)
        {
            throw;
        }

        return nodes;
    }

    private static List<Item> getSubFolders(string dir, ref int id)
    {
        string url = getFTPUrl(dir);
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
        request.KeepAlive = false;
        request.UseBinary = true;
        request.Method = WebRequestMethods.Ftp.ListDirectory;

        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        Stream responseStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(responseStream);
        string names = reader.ReadToEnd();

        reader.Close();
        response.Close();

        List<string> list = new List<string>();
        if(!string.IsNullOrEmpty(names)){
            list = names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(a => a.IndexOf('.') < 0).ToList();
        }

        List<Item> nodes = new List<Item>();
        
        for (int i = 0; i < list.Count; i++)
        {
            id++;
            string subdir = dir + "/" + list[i];
            Item node = new Item(id.ToString(), HttpUtility.HtmlEncode(list[i]), HttpUtility.HtmlEncode(subdir));
            List<Item> children = getSubFolders(subdir, ref id);
            if (children.Count == 0) 
            {
                node.children = new List<Item>(); 
            }
            else
            {
                node.children = children;
            }
            nodes.Add(node);

        }

        return nodes;
    }



    [System.Web.Services.WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = true, XmlSerializeString = false)]
    public static List<string> GetUploadFolderList()
    {
        string imageFolder = DefaultImageFolder;
        try
        {
            string url = getFTPUrl("");
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.UseBinary = true;
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string names = reader.ReadToEnd();

            reader.Close();
            response.Close();

            return names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(a => a.IndexOf('.') < 0).ToList();
        }
        catch (Exception e)
        {
            throw;
        }

    }

    [System.Web.Services.WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = true, XmlSerializeString = false)]
    public static List<string> GetImageList(string path)
    {

        string imageFolder = DefaultImageFolder;
        try
        {
            string url = getFTPUrl(path);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
            request.UseBinary = true;
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string names = reader.ReadToEnd();

            reader.Close();
            response.Close();

            return names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(a => a.ToLower().EndsWith(".tif") || a.ToLower().EndsWith(".tiff")).ToList();
        }
        catch (Exception e)
        {
            throw;
        } 
    }

    [System.Web.Services.WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = true)]
    public static string GetImage(string folder, string fileName)
    {
        try
        {
            string url = getFTPUrl(folder + "/" + fileName);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
            request.UseBinary = true;
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            byte[] bytes = new byte[2048];
            int i = 0;
            byte[] imgData = null;
            using (MemoryStream mStream = new MemoryStream())
            {
                do
                {
                    i = stream.Read(bytes, 0, bytes.Length);
                    mStream.Write(bytes, 0, i);

                } while (i != 0);

                imgData = ConvertToJPG(mStream);
            }
            return Convert.ToBase64String(imgData);
        }

        catch (Exception e)
        {
            throw;
        } 


    }


    /*
    [System.Web.Services.WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = true, XmlSerializeString = false)]
    public static List<string> GetUploadFolderList()
    {
        string imageFolder = DefaultImageFolder;
        List<string> folders = null;
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
                    folders = new DirectoryInfo(imageFolder).GetDirectories().Select(o => o.Name).ToList<string>();
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

        return folders;
    }
    

    [System.Web.Services.WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = true, XmlSerializeString = false)]
    public static List<string> GetImageList(string path)
    {

        string imageFolder = DefaultImageFolder + @"\" + path;
        List<string> filePaths = null;
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
                    impersonationContext = windowsIdentity.Impersonate();

                    filePaths = new DirectoryInfo(imageFolder).GetFiles("*.tif*", SearchOption.TopDirectoryOnly).Select(o => o.Name).ToList<string>();

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

        return filePaths;
    }
    
    [System.Web.Services.WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = true)]
    public static string GetImage(string folder, string fileName)
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
                    string imageFolder = DefaultImageFolder + @"\" + folder + @"\" + fileName;

                    //string json = Newtonsoft.Json.JsonConvert.SerializeObject(ReadTifImage(imageFolder));
                    byte[] data = ReadTifImage(imageFolder);

                    return Convert.ToBase64String(data);
                }
                return null;
            }
            return null;
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

    private static byte[] ConvertToJPG(byte[] srcByte)
    {
        if (srcByte == null)
        {
            return null;
        }
        byte[] bytes = null;

        using (MemoryStream ms = new MemoryStream(srcByte))
        {
            using (System.Drawing.Image img = System.Drawing.Image.FromStream(ms))
            {
                MemoryStream msDest = new MemoryStream();
                img.Save(msDest, System.Drawing.Imaging.ImageFormat.Jpeg);
                bytes = msDest.ToArray();
                msDest.Dispose();
            }
        }

        return bytes;
    }

    private static byte[] ReadTifImage(string ImagePath)
    {
        byte[] imgData = null;
        //filepath = @"\201306\dbcabcf9-b4a0-473c-8b23-c4489158bf45.pdf";
        try
        {

            if (File.Exists(ImagePath))
            {
                FileStream fs = null;
                fs = File.OpenRead(ImagePath);
                imgData = new byte[fs.Length];

                if (imgData != null)
                {
                    fs.Read(imgData, 0, Convert.ToInt32(fs.Length));
                    imgData = ConvertToJPG(imgData);
                }
                fs.Dispose();
            }
        }
        catch (Exception ex)
        {
        }

        return imgData;
    }
*/


    private static byte[] ConvertToJPG(MemoryStream ms)
    {
        byte[] bytes = null;
        using (System.Drawing.Image img = System.Drawing.Image.FromStream(ms))
        {
            MemoryStream msDest = new MemoryStream();
            img.Save(msDest, System.Drawing.Imaging.ImageFormat.Jpeg);
            bytes = msDest.ToArray();
            msDest.Dispose();
        }
        return bytes;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../../Login.aspx','_top')</SCRIPT>", false);
        }

    }

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
    }

    protected void dgErrorLog_NeedDataSource(object sender, EventArgs e)
    {
        //BindErrorlog(false);
    }

}

public class Item
{
    public string id = null;
    public string text = null;
    public bool leaf = false;
    public string value = null;
    public List<Item> children = null;
    public Item(string id, string text, string value)
    {
        this.id = id;
        this.text = text;
        this.value= value;
    }
}