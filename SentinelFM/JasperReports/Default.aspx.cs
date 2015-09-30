using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;
using System.Data;
using VLF.DAS.Logic;

public partial class JasperReports_Default : System.Web.UI.Page
{
    protected SentinelFMSession sn;
    public static string BiPublicDashboard;
    public static string BiPublicReports;
    public static string BiPublicAdHoc;
    public static string BiOrganizationDashboard;
    public static string BiOrganizationReports;
    public static string BiDemo;
    public static string Token;

    public string FLEET_DATA = "[]";
    public string DRIVER_DATA = "[]";
    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        //Lets move new values outside of the web.config and create a seperate json based settings file.
        BiPublicDashboard = ConfigurationManager.AppSettings["BiPublicDashboard"];
        BiPublicReports = ConfigurationManager.AppSettings["BiPublicReports"];
        BiOrganizationDashboard = ConfigurationManager.AppSettings["BiOrganizationDashboard"];
        BiOrganizationReports = ConfigurationManager.AppSettings["BiOrganizationReports"];
        BiPublicAdHoc = ConfigurationManager.AppSettings["BiPublicAdHoc"];
        BiDemo = ConfigurationManager.AppSettings["BiDemo"];

        sn = (SentinelFMSession)Session["SentinelFMSession"];
        int organizationId = (sn == null ? 0 : sn.User.OrganizationId);
        if (organizationId == 0)
        {
            Response.Redirect("../Login.aspx");
        }
        if (sn != null)
        {
            Token = CalculateMD5Hash(string.Format("{0}-{1}-{2}-{3}", sn.UserID, sn.User.OrganizationId, sn.UserName, sn.Password));
        }

        FleetFill();
        LoadDrivers();
    }

    public string CalculateMD5Hash(string input)
    {
        // step 1, calculate MD5 hash from input
        MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);

        // step 2, convert byte array to hex string
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }
        return sb.ToString();
    }

    private void FleetFill()
    {
        try
        {

            DataSet dsFleets = new DataSet();
            dsFleets = sn.User.GetUserFleets(sn);
            FLEET_DATA = "{data:[[";
            for (int i = 0; i < dsFleets.Tables[0].Rows.Count; i++)
            {
                if (i > 0)
                {
                    FLEET_DATA += ",";
                }
                FLEET_DATA += "{id: " + dsFleets.Tables[0].Rows[i]["fleetId"].ToString() + ", title:\"" + dsFleets.Tables[0].Rows[i]["fleetName"].ToString() + "\"}";
            }
            FLEET_DATA += "]]";
            FLEET_DATA += ",default:[" + sn.User.DefaultFleet.ToString() + "]";
            FLEET_DATA += "}";
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            Response.Redirect("../Login.aspx");
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            FLEET_DATA = "{data: []}";
        }
    }

    /// <summary>
    /// Load drivers for the company
    /// </summary>
    private void LoadDrivers()
    {
        DataSet dsDrivers = new DataSet();
        dsDrivers = GetDrivers();

    }

    /// <summary>
    /// Get drivers for the company
    /// </summary>
    private DataSet GetDrivers()
    {


        DataSet dsDrivers = new DataSet();

        try
        {
            ContactManager contactMsg = new ContactManager(sConnectionString);
            dsDrivers = contactMsg.GetOrganizationDrivers(sn.User.OrganizationId);
            var totalRows = dsDrivers.Tables[0].Rows.Count;
            DRIVER_DATA = "{data:[[";
            for (int i = 0; i < 100; i++)
            {
                if (i > 0)
                {
                    DRIVER_DATA += ",";
                }
                DRIVER_DATA += "{id: " + dsDrivers.Tables[0].Rows[i]["DriverId"].ToString() + ", title:\"" + dsDrivers.Tables[0].Rows[i]["FirstName"].ToString() + " " + dsDrivers.Tables[0].Rows[i]["LastName"].ToString() + "\"}";
            }
            DRIVER_DATA += "]]}";
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            //lblMessage.Text = Ex.Message;
            Response.Redirect("../Login.aspx");
        }

        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            DRIVER_DATA = "{data:[]}";

        }

        return dsDrivers;

    }
}