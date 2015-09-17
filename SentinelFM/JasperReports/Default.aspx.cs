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
    protected void Page_Load(object sender, EventArgs e)
    {


        

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
}