using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using System.Xml;
using Telogis.GeoBase.Authentication;
using System.Collections.Specialized;
using System.Web.Services;


/// <summary>
/// Base page for Map 
/// </summary>
public class SentinelMapBasePage:SentinelFMBasePage

{
    //private const string API_SRC = "telogis.geobase.js";
    private const string USERNAME = "guest";
    private const string PASSWORD = "guest";
    private const long LIFETIME_SECS = 3600;

    private List<string> servers = new List<string>();

    private DateTime expiry;
    private string token = string.Empty;

    /// <summary>
    /// Generates an authentication token for the page when it is loaded, from the supplied username and
    /// password. This token is saved and can be later accessed with the <code>GetToken()</code> method.
    /// </summary>


	public SentinelMapBasePage()
	{
        this.Load += new EventHandler(AuthenticatedPage_Load);
	}

    void AuthenticatedPage_Load(object sender, EventArgs e)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Page.Server.MapPath("servers.xml"));

            foreach (XmlNode child in doc.SelectNodes("//Server"))
            {
                //by devin
                if (Request.Url.ToString().ToLower().Contains("https://"))
                {
                    child.InnerText = child.InnerText.ToLower().Replace("http://", "https://");
                }

                servers.Add(child.InnerText);
            }
        }
        catch (Exception ex)
        {
            servers.Add(ex.Message);
        }

        foreach (string s in servers)
        {
            if (Authenticate(s))
            {
                break;
            }
        }
    }

    /// <summary>
    /// Performs the appropriate requests to retrieve an authentication token from the server by passing it
    /// a username and password to successive GetLoginToken and GetAuthToken calls.
    /// </summary>
    /// <param name="server">the URL of the server to attempt to get a token from.</param>
    /// <returns>true if the token was retrieved successfully; false otherwise.</returns>

    public bool Authenticate(string server)
    {

        try
        {
            int servermask;
            token = Authenticator.GetAuthToken(server, USERNAME, PASSWORD, IPAddress.Parse(Request.UserHostAddress), 0, DateTime.Now + TimeSpan.FromDays(1), out servermask, out expiry).ToString();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Writes to the page an array of GeoStream service URLs, as specified by <code>servers</code>. This
    /// can be called to generate a parameter to <code>Telogis.GeoBase.setService</code> that is consistent
    /// with the services used to get the page's authentication token.
    /// </summary>

    public void GetService()
    {

        StringBuilder js = new StringBuilder("[");
        js.Append("'");
        js.Append(servers[0]);
        js.Append("'");
        if (servers.Count > 1)
        {
            js.Append(",");
        }
        for (int i = 1; i < servers.Count; i++)
        {
            js.Append("'");
            js.Append(servers[i]);
            js.Append("'");
            if (i != servers.Count - 1)
            {
                js.Append(",");
            }
        }

        js.Append("]");
        Response.Write(js.ToString());
    }

    /// <summary>
    /// Writes to the page a string containing the authentication token generated when the page was loaded, and its 
    /// duration, in milliseconds. This should be called to generate parameters to the
    /// <code>Telogis.GeoBase.setAuthToken()</code> function.
    /// </summary>

    public void GetToken()
    {

        long duration = expiry.Ticks - DateTime.Now.Ticks;
        duration = 1000 * duration / TimeSpan.TicksPerSecond;

        Response.Write("'" + token + "', " + duration);
    }
}