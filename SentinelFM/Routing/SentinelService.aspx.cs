using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Xml;
using System.Web.Script.Serialization;
using SentinelFM;
using System.Configuration;
using VLF.DAS.Logic;

public partial class SentinelAPI : System.Web.UI.Page
{
    protected SentinelFMSession sn;
    private const string GoogleKey = "AIzaSyA4Bsl89cKuhy13bfuyyZazX3Y9GvrNfjo";
    private const string app_id="v5HljYEynPujgUUkNmny";
    private const string app_code = "x14y1MmQaoSerjNQKGsABw";
    private static string Smtp = ConfigurationManager.AppSettings["SMTPServer"];
    private static int SmtpPort = 25;
    private static string FromAddress = "alerts@sentinelfm.com";

    protected void Page_Load(object sender, EventArgs e)
    {       
        try
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            int organizationId = sn.User.OrganizationId;
            if (organizationId == 0)
            {
                Response.Redirect("../Login.aspx");
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        string action = Request.QueryString["action"] ?? Request.Form["action"];
        string json = null;
        if (action.Equals("autocomplete"))
        {
            string input = Request.QueryString["input"];
            string language = Request.QueryString["language"] ?? "en";
            if (string.IsNullOrEmpty(input))
            {
                throw new Exception("Input cannot be empty");
            }
            bool NeedGoogleApi = true;
            IList<Dictionary<string, string>> parsedResults = new List<Dictionary<string, string>>();
            string[] inputs = input.Split(',');
            if (inputs.Length == 2)
            {
                double lat = 0;
                double lng = 0;
                if (Double.TryParse(inputs[0], out lat) && Double.TryParse(inputs[1], out lng))
                {
                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                    tmpRowResults.Add("description", Convert.ToString(input));
                    tmpRowResults.Add("reference", Convert.ToString(input)); 
                    parsedResults.Add(tmpRowResults);
                    NeedGoogleApi = false;
                }
            }

            if (NeedGoogleApi)
            {
                string url =
                            string.Format("https://maps.googleapis.com/maps/api/place/autocomplete/xml?input={0}&types=geocode&location=43.67746,-79.5850766666667&language={1}&sensor=true&key={2}", input, language, GoogleKey);
                string googleResponse = null;
                using (WebClient client = new WebClient())
                {
                    googleResponse = client.DownloadString(url);
                }

                parsedResults = ParseXml(googleResponse, "prediction", new List<string>() { "description", "reference" });
            }
           
            IList<Dictionary<string, string>> bsmResults = GetSentinelFmLandmarkGeozone(sn.User.OrganizationId, input);
            Dictionary<string, string> breakElement = new Dictionary<string, string>();
            breakElement.Add("description", "------------------------------");
            breakElement.Add("reference", "");
            if (bsmResults.Count > 0)
            {
                if (parsedResults.Count > 0)
                {
                    parsedResults.Add(breakElement);
                }

                foreach (Dictionary<string, string> bsmRow in bsmResults)
                {
                    parsedResults.Add(bsmRow);
                }
            }
            IList<Dictionary<string, string>> finaleResults = new List<Dictionary<string, string>>();
            foreach (Dictionary<string, string> aResult in parsedResults)
            {
                if (aResult["description"].Contains("----"))
                {
                    aResult.Add("value", "");
                }
                else
                {
                    aResult.Add("value", aResult["description"]);
                }
                finaleResults.Add(aResult);
            }
            Dictionary<string, IList<Dictionary<string, string>>> results = new Dictionary<string, IList<Dictionary<string, string>>>();
            results.Add("predictions", finaleResults);
            var jss = new JavaScriptSerializer();
            json = jss.Serialize(results);
        }
        else if(action.Equals("getCoord"))
        {
            string reference = Request.QueryString["reference"];
            string latlonUrl =
                string.Format("https://maps.googleapis.com/maps/api/place/details/json?reference={0}&sensor=true&key={1}", reference, GoogleKey);
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString(latlonUrl);
            }
        }
        else if (action.Equals("geocode"))
        {
            string latlng = Request.QueryString["latlng"];
            string latlonUrl =
                string.Format("http://maps.googleapis.com/maps/api/geocode/json?latlng={0}&sensor=true", latlng);
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString(latlonUrl);
            }
        }
        else if (action.Equals("queryRoute"))
        {
            string query = Request.QueryString.ToString();
            query = query.Replace("&action=queryRoute", string.Format("&app_id={0}&app_code={1}", app_id, app_code));
            string url = null;
            if (string.IsNullOrEmpty(Request.QueryString["routeId"]))
            {
                url = string.Format("http://route.api.here.com/routing/7.2/calculateroute.json?{0}", query);    
            }
            else
            {
                url = string.Format("http://route.api.here.com/routing/7.2/getroute.json?{0}", query);
            }
            
            using (WebClient client = new WebClient())
            {                
                try
                {
                    json = client.DownloadString(url);
                }
                catch (Exception exception)
                {
                    IDictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("status", "failed");
                    result.Add("Reason" , exception.Message);
                    var jss = new JavaScriptSerializer();
                    json = jss.Serialize(result);
                }
            }
        }
        else if(action.Equals("email"))
        {
            string subject = Request.Form["subject"];
            string name = Request.Form["name"];
            string to = Request.Form["to"];
            string body = Request.Form["body"];
            subject = string.Format("{0} sends route ({0}) description to you", name, subject);
            IDictionary<string, string> result = new Dictionary<string, string>();
            if (SendEmail(FromAddress, to, subject, body))
            {
                result.Add("status", "success");
            }
            else
            {
                result.Add("status", "failed");
            }
            var jss = new JavaScriptSerializer();
            json = jss.Serialize(result);
        }
        
        Response.Clear();
        Response.ContentType = "application/json; charset=utf-8";
        Response.Write(json);
        Response.End();
    }

    private IList<Dictionary<string, string>> GetSentinelFmLandmarkGeozone(int organizationId, string input)
    {
        return PostGisLandmark.GetBsmLandmarksAndGeozones(organizationId, input);
    }

    private IList<Dictionary<string, string>> ParseXml(string myXmlString, string searchSection, IList<string> searchNodes)
    {
        List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
        try
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(myXmlString); // suppose that myXmlString contains "<Names>...</Names>"
            XmlNodeList xnList = xml.GetElementsByTagName(searchSection);
            foreach (XmlNode sNode in xnList)
            {
                try
                {

                    if (searchNodes != null)
                    {
                        Dictionary<string, string> result = new Dictionary<string, string>();
                        foreach (string nodeName in searchNodes)
                        {
                            result.Add(nodeName, (sNode[nodeName] != null ? sNode[nodeName].InnerText.Trim() : null));
                        }
                        results.Add(result);
                    }

                }
                catch (Exception exception)
                {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("ERROR", exception.Message);
                    results.Add(result);
                }

            }
        }
        catch (Exception exception)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("ERROR", exception.Message);
            results.Add(result);
        }
        return results;
    }

    private bool SendEmail(string from, string to, string subject, string body)
    {
        try
        {

            SmtpClient smtpClient = new SmtpClient(Smtp, SmtpPort);
                string[] tos = to.Split(';');
                MailMessage mailMsg = new MailMessage();
                mailMsg.From = new MailAddress(from);
                mailMsg.Subject = subject;
                mailMsg.Body = HttpUtility.HtmlDecode(body);
                mailMsg.IsBodyHtml = true;
                foreach (string toMe in tos)
                {
                    if (!string.IsNullOrEmpty(toMe))
                    {
                        if (IsValidEmail(toMe))
                        {
                            mailMsg.To.Add(toMe);
                        }
                    }
                }

                NetworkCredential credentials = new NetworkCredential("", "");
                smtpClient.Credentials = credentials;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Timeout = 60000;
                smtpClient.Send(mailMsg);
            return true;
        }
        catch (Exception exception)
        {
            //Console.WriteLine(exception.Message);
            throw new Exception(exception.StackTrace);
        }
        return false;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}