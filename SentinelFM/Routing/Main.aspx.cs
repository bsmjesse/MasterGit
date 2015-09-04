using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using SentinelFM;
using SentinelFM.GeomarkServiceRef;
using VLF.DAS.Logic;

public partial class RouterBuilder_Main : System.Web.UI.Page
{
    protected SentinelFMSession sn;
    public string LandmarkId = null;
    private static string connStr = ConfigurationManager.AppSettings["SpatialDBProduction"];
    public Dictionary<string, string> LandmarkInfo = new Dictionary<string, string>();
    public string WayPints = "";
    public bool HgiUser = false;
    public bool DisputeLinkAdmin = false;
    public string UserLanguage = "en";
    public int ServiceConfigId = 0;
    public string Preferences = "fastest;car;traffic:disabled;";
    public string RouteId = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        sn = (SentinelFMSession)Session["SentinelFMSession"];
        int organizationId = (sn == null ? 0 : sn.User.OrganizationId);
        
        if (organizationId == 0)
        {
            Response.Redirect("../Login.aspx");
        }
        HgiUser = sn.User.UserGroupId == 1;
        if (sn.User.ControlEnable(sn, 80))
            DisputeLinkAdmin = true;
        string UserLanguageStr = sn.SelectedLanguage;
        if (string.IsNullOrEmpty(UserLanguageStr))
        {
            string[] UserLanguageArray = UserLanguageStr.Split('-');
            UserLanguage = UserLanguageArray[0];
        }

        if (!string.IsNullOrEmpty(Request.QueryString["landmarkid"]))
        {
            LandmarkId = Request.QueryString["landmarkid"];
        }
        if (!string.IsNullOrEmpty(Request.QueryString["routeId"]))
        {
            RouteId = Request.QueryString["routeId"];
        }
        if (!string.IsNullOrEmpty(Request.QueryString["preference"]))
        {
            Preferences = Request.QueryString["preference"];
        }
        int landmarkId = Convert.ToInt32(LandmarkId ?? "0");
        string timezone = null;
        if (landmarkId > 0)
        {
            LandmarkInfo = PostGisLandmark.GetLandmark(connStr, landmarkId);
            timezone = Convert.ToString(LandmarkInfo["timezone"]);
            WayPints = Convert.ToString(LandmarkInfo["waypoints"]);           
        }
        for (int i = -12; i < 14; i++)
        {
            ListItem myTimeZone = new ListItem();
            if (i != 0)
            {
                if (i < 0)
                {
                    myTimeZone.Value = i.ToString();
                    myTimeZone.Text = "GMT" + i.ToString();                    
                }
                else
                {                    
                    myTimeZone.Value = i.ToString();
                    myTimeZone.Text = "GMT+" + i.ToString(); 
                }
            }
            else
            {
                myTimeZone.Value = "0";
                myTimeZone.Text = "GMT";
            }

            if (myTimeZone.Value.Equals(timezone))
            {
                myTimeZone.Selected = true;
            }
            else if (i == -5 && string.IsNullOrEmpty(timezone))
            {
                myTimeZone.Selected = true;
            }

            timeZone.Items.Add(myTimeZone);            
        }
        timeZone.Visible = false;
        ServiceConfigId = ServiceAssignment.RouteServiceConfigId(landmarkId);
    }

    [System.Web.Services.WebMethod(EnableSession = true)]
    public static string DeleteRoute(string landmarkId)
    {
        SentinelFMSession sn = HttpContext.Current.Session["SentinelFMSession"] as SentinelFMSession;
        if (sn == null) return "Fail";

        int iLandmarkId = -1;
        if (string.IsNullOrEmpty(landmarkId) || !int.TryParse(landmarkId, out iLandmarkId))
            return "Fail";
        if (PostGisLandmark.DeleteRoute(connStr, sn.User.OrganizationId, iLandmarkId))
        {
            RenewCache();
            return "Created";
        }
        else
            return "Fail";
    }

    [System.Web.Services.WebMethod(EnableSession = true)]
    public static string AcceptFormData(string funcParam)
    {
        // this is your server side function
        //the data you will get will be in format e.g. a=1&b=2&c=3&d=4&e=5 inside funcParam variable


        int organizationId = 0;
        int uid = 0;
        try
        {
            NameValueCollection qscoll = HttpUtility.ParseQueryString(funcParam);
            if (!string.IsNullOrEmpty(qscoll["uid"]) && !string.IsNullOrEmpty(qscoll["oid"]))
            {
                if (!Int32.TryParse(qscoll["uid"], out uid))
                {
                    return "0";
                }

                if (!Int32.TryParse(qscoll["oid"], out organizationId))
                {
                    return "0";
                }
            }
            else
            {
                SentinelFMSession sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
                if (sn == null) return "Fail";
                organizationId = sn.User.OrganizationId;
                uid = sn.UserID;
            }
            

            string routerPoints = qscoll["NavTeqResponse"];
            if (string.IsNullOrEmpty(routerPoints))
            {
                string baseUrl = "http://router.project-osrm.org/viaroute";
                string url = string.Format("{0}{1}&output=gpx", baseUrl, qscoll["txtQueueString"]);

                using (WebClient client = new WebClient())
                {
                    client.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:22.0) Gecko/20100101 Firefox/22.0";
                    string xml = client.DownloadString(url);
                    if (!string.IsNullOrEmpty(xml))
                    {
                        routerPoints = GetPoints(xml);
                    }
                }
            }
            

            if (string.IsNullOrEmpty(routerPoints))
            {
                throw new Exception("Route geometry cannot be empty");
            }
            
            string routerName = qscoll["routeName"];
            string contactPerson = qscoll["txtName"];
            int buffer = Convert.ToInt32(string.IsNullOrEmpty(qscoll["routeBuffer"]) ? "0" : qscoll["routeBuffer"]);
            int landmarkId = Convert.ToInt32(string.IsNullOrEmpty(qscoll["LandmarkId"]) ? "0" : qscoll["LandmarkId"]);
            string email = qscoll["txtEmail"];
            string contactNo = qscoll["txtPhone"];
            string wayPoints = qscoll["txtWayPoints"];
            string preference = qscoll["txtPreference"] != null ? "&preference=" + qscoll["txtPreference"] : String.Empty;
            string routerLink = qscoll["txtEditLink"] + preference;
           
            if (landmarkId > 0)
            {
                routerLink =  string.Format("{0}&landmarkid={1}", routerLink, landmarkId);   
            }
            //routerLink = routerLink.ToLower();            
            string[] routerLinks = routerLink.Split(new string[] {"main"}, StringSplitOptions.None);
            if (routerLinks.Count() > 1)
            {
                routerLink = string.Format("main{0}", routerLinks[1]);
                
            }
            else
            {
                routerLinks = routerLink.Split(new string[] { "Main" }, StringSplitOptions.None);
                routerLink = string.Format("Main{0}", routerLinks[1]);
            }
            
            int timeZone = Convert.ToInt32(qscoll["timeZone"]);
            int daylightSaving = 1;
            string description = qscoll["txtDescription"];
            double latitude = 0;
            double longitude = 0;
            if (!string.IsNullOrEmpty(qscoll["correctSpeed"]))
            {
                int disputeId = Convert.ToInt32(string.IsNullOrEmpty(qscoll["disputeId"]) ? "0" : qscoll["disputeId"]);
                string disputeComment = Convert.ToString(qscoll["disputeComment"]);
                int correctionId = Convert.ToInt32(string.IsNullOrEmpty(qscoll["correctionid"]) ? "0" : qscoll["correctionid"]);
                int gblSegment = Convert.ToInt32(string.IsNullOrEmpty(qscoll["gblSegment"]) ? "0" : qscoll["gblSegment"]);
                int myOrganization = (gblSegment < 1 ? organizationId : 0);

                IList<Dictionary<string, string>> results = PostGisLandmark.SaveSpeedCorrection(connStr, routerPoints, myOrganization, routerName, contactPerson, buffer, email, contactNo, timeZone, daylightSaving, routerLink, wayPoints, description, latitude, longitude, ref correctionId, uid, Convert.ToInt32(qscoll["correctSpeed"]), disputeId, disputeComment);               
                if (results != null)
                {
                    if (results.Any())
                    {
                        var oSerializer = new JavaScriptSerializer();
                        string json = oSerializer.Serialize(results);
                        return json;
                    }
                    else
                    {
                        return Convert.ToString(correctionId);
                    }
                }
                else
                {
                    return "0";
                }
                
            }
            else
            {                
                bool result = PostGisLandmark.SaveRoute(connStr, routerPoints, organizationId, routerName, contactPerson, buffer, email, contactNo, timeZone, daylightSaving, routerLink, wayPoints, description, latitude, longitude, ref landmarkId, uid);

                if (result)
                {
                    string addToService = qscoll["chkEnableService"];
                    int serviceResult = 0;
                    serviceResult = ServiceAssignment.RouteServiceConfigId(landmarkId);
                    if (!string.IsNullOrEmpty(addToService) && landmarkId > 0)
                    {
                        if (serviceResult < 1)
                        {
                            string expression = string.Format("LandmarkOut = {0};LandmarkOutNow = 1;", landmarkId);
                            serviceResult = ServiceAssignment.SaveServiceConfig(expression, routerName, 4, organizationId, uid, DateTime.Now, 0, false, 1, 1);
                            if (serviceResult > 0)
                            {
                                string subject = string.Format("[VehicleDescription] is out of [ServiceName] @ [StDate]");
                                string message = subject + "<br /> - " + description;
                                ServiceAssignment.SaveNotificationConfig(serviceResult, email, null, subject, message);
                            }
                            ServiceAssignment.SaveServiceRouteMapping(organizationId, serviceResult, landmarkId, false);
                            RenewCache();
                        }
                    }
                    else if (landmarkId > 0 && string.IsNullOrEmpty(addToService))
                    {
                        if (serviceResult > 0)
                        {
                            ServiceAssignment.SaveServiceRouteMapping(organizationId, serviceResult, landmarkId, true);
                        }
                    }

                    return Convert.ToString(landmarkId);
                }
                else
                {
                    return "0";
                }
            }

            return "Failed";
        }
        catch (Exception exception)
        {
            
            throw new Exception(string.Format("Reason: {0}, Debug: {1}", exception.Message, exception.StackTrace));
        }
    }


    [System.Web.Services.WebMethod(EnableSession = true)]
    public static string GetDisputePoints(string organizationId, string filter)
    {
        try
        {
            SentinelFMSession sn = HttpContext.Current.Session["SentinelFMSession"] as SentinelFMSession;
            if (sn == null) return "Fail";
            int oid = Convert.ToInt32(organizationId);
            if (oid.Equals(0))
            {
                oid = sn.User.OrganizationId;
            }
            IList<Dictionary<string, string>> results = PostGisLandmark.GetDisputePoints(oid, filter, 0);
            if (results != null)
            {
                var oSerializer = new JavaScriptSerializer();
                string json = oSerializer.Serialize(results);
                return json;
            }
            else
            {
                return "Failed";
            }
        }
        catch (Exception exception)
        {
            throw new Exception(exception.StackTrace);
        }        
    }
    

    [System.Web.Services.WebMethod(EnableSession = true)]
    public static string DeleteDisputePoint(string pointId, string comment)
    {
        try
        {
            SentinelFMSession sn = HttpContext.Current.Session["SentinelFMSession"] as SentinelFMSession;
            if (sn == null) return "Fail";
            int pid = Convert.ToInt32(pointId);
           
            bool result = PostGisLandmark.DeleteDisputePoint(pid, comment);
            if (result)
            {                
                return "Success";
            }
            else
            {
                return "Failed";
            }
        }
        catch (Exception exception)
        {
            throw new Exception(exception.StackTrace);
        }
    }

    [System.Web.Services.WebMethod(EnableSession = true)]
    public static string GetCorrectRoute(string routeId, string pid)
    {
        try
        {
            SentinelFMSession sn = HttpContext.Current.Session["SentinelFMSession"] as SentinelFMSession;
            if (sn == null) return "Fail";

            Dictionary<string, string> result = PostGisLandmark.GetCorrectionRoute(connStr, Convert.ToInt32(routeId));
            if (result == null)
            {
                IList<Dictionary<string, string>> resultPoint = PostGisLandmark.GetDisputePoints(sn.User.OrganizationId, null, Convert.ToInt32(pid));
                result = resultPoint.FirstOrDefault();
            }
            if (result != null)
            {
                var oSerializer = new JavaScriptSerializer();
                string json = oSerializer.Serialize(result);
                return json;
            }
            else
            {
                return "Failed";
            }
        }
        catch (Exception exception)
        {
            throw new Exception(exception.StackTrace);
        }
    }

    [System.Web.Services.WebMethod(EnableSession = true)]
    public static string DeleteCorrectRoute(string routeId)
    {
        try
        {
            SentinelFMSession sn = HttpContext.Current.Session["SentinelFMSession"] as SentinelFMSession;
            if (sn == null) return "Fail";

            IList<Dictionary<string, string>> results = PostGisLandmark.DeleteCorrectedSegment(Convert.ToInt32(routeId));
            if (results != null)
            {
                var oSerializer = new JavaScriptSerializer();
                string json = oSerializer.Serialize(results);
                return json;
            }
            else
            {
                return "Failed";
            }
        }
        catch (Exception exception)
        {
            throw new Exception(exception.StackTrace);
        }
    }

    public static string GetPoints(string xmlStr)
    {        
        string routerPoints = null;
        try
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStr); // suppose that myXmlString contains "<Names>...</Names>"
            XmlNodeList xnList = xml.GetElementsByTagName("rtept");
            foreach (XmlNode sNode in xnList)
            {
                if (sNode == null)
                {
                    continue;
                }
                try
                {
                    if (string.IsNullOrEmpty(routerPoints))
                    {
                        routerPoints = string.Format("{0} {1}", sNode.Attributes["lon"].Value, sNode.Attributes["lat"].Value);
                    }
                    else
                    {
                        routerPoints += "," + string.Format("{0} {1}", sNode.Attributes["lon"].Value, sNode.Attributes["lat"].Value);
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception(exception.Message);
                }

            }
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message);
        }
        return routerPoints;
    }

    private static void RenewCache()
    {
        try
        {
            SentinelFM.GeomarkServiceRef.GeomarkServiceClient geomarkServiceClient = new GeomarkServiceClient("httpbasic");
            geomarkServiceClient.ReleaseCache();
        }
        catch (Exception exception)
        {

            throw new Exception(exception.StackTrace);
        }
    }

}