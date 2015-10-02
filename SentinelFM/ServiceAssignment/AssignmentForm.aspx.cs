using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using MapPoint;
using SentinelFM;
using SentinelFM.GeomarkServiceRef;
using SentinelFM.ServerDBFleet;
using SentinelFM.ServerDBOrganization;
using SentinelFM.ServerDBVehicle;
using VLF.DAS;
using VLF.DAS.Logic;
using Vehicle = VLF.DAS.Logic.Vehicle;

public partial class ServiceAssignment_AssignmentForm : System.Web.UI.Page
{
    protected SentinelFMSession sn;
    private DBFleet dbf = null;
    private DBOrganization dbo = null;
    private DBVehicle dbv = null;
    public int VehicleId;
    public string ObjectName;
    public string ServiceName;
    public bool Saved = false;
    public string Service = "speed";
    public int ServiceId = 2;
    public int HgiUser = 1;
    public string StartTimeColumnName = "Event Time";
    public string EndTimeColumnName = "Exception Time";
    public string DurationUnit = "Minute";
    protected void Page_Init(object sender, EventArgs e)
    {
        try
        {
            dbf = new DBFleet();
            dbo = new DBOrganization();
            dbv = new DBVehicle();

        }
        catch (Exception exception)
        {

            throw new Exception(exception.StackTrace);
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        sn = (SentinelFMSession)Session["SentinelFMSession"];
        int organizationId = sn.User.OrganizationId;
        if (organizationId == 0)
        {
            Response.Redirect("../Login.aspx");
        }

        HgiUser = (sn.UserName.ToLower().Contains("hgi_") ? 1 : 0);
        
        VehicleId = Convert.ToInt32(Request.QueryString["vehicleId"] ?? Request.Form["vehicleId"]);
        ObjectName = Request.QueryString["objectname"];        
        int serviceId = 0;
        if (Int32.TryParse(Request.QueryString["serviceid"], out serviceId))
        {
            ServiceId = Convert.ToInt32(Request.QueryString["serviceid"]);
            Dictionary<string, string> ServiceInfo = ServiceAssignment.GetServiceInfo(ServiceId);
            if (ServiceInfo.ContainsKey(Request.QueryString["serviceid"]))
            {
                ServiceName = ServiceInfo[Request.QueryString["serviceid"]];
            }
        }
        else
        {
            ServiceName = Request.QueryString["servicename"];    
        }
       
        Service = !string.IsNullOrEmpty(Request.QueryString["service"]) ? Request.QueryString["service"].ToLower() : "speed"; //Landmark, Speed, Sensor and Route
        switch (Service) {
                case "speed":
                    ServiceId = 2;
                    StartTimeColumnName = "Exception Start Time";
                    EndTimeColumnName = "Exception End Time";
                    DurationUnit = "Second";
                    break;
                case "landmark":
                    ServiceId = 1;
                    break;
                case "route":
                    ServiceId = 4;
                    break;
                case "sensor":
                    ServiceId = 3;
                    break;
            }
        //FillFleets();
        //FillConfiguredService();
    }

    [System.Web.Services.WebMethod(EnableSession = true)]
    public static string SaveServiceAssignment(string servicesStr, string vehiclesStr, string objects)
    {
        
        try
        {
            SentinelFMSession sn = HttpContext.Current.Session["SentinelFMSession"] as SentinelFMSession;
            if (sn == null) return "Fail";

            if (!string.IsNullOrEmpty(servicesStr) && !string.IsNullOrEmpty(vehiclesStr))
            {                          
                foreach (var service in servicesStr.Split(','))
                {
                    int serviceId = Convert.ToInt32(service.Trim());
                    foreach (var vehicle in vehiclesStr.Split(','))
                    {
                        ServiceAssignment.SaveSingleAssignment(serviceId, Convert.ToInt32(vehicle.Trim()), objects, DateTime.Now);                      
                    }
                    
                }
                SentinelFM.GeomarkServiceRef.GeomarkServiceClient geomarkServiceClient = new GeomarkServiceClient("httpbasic");
                geomarkServiceClient.ReleaseCache();
            }
            return "Success";
        }
        catch (Exception exception)
        {
            
            throw new Exception(exception.StackTrace);
        }
       
    }


    [System.Web.Services.WebMethod(EnableSession = true)]
    public static string DeleteAssignment(string serviceId, string vehicleId)
    {        
        try
        {
            SentinelFMSession sn = HttpContext.Current.Session["SentinelFMSession"] as SentinelFMSession;
            if (sn == null) return "Fail";

            ServiceAssignment.DeleteObject(Convert.ToInt32(serviceId), Convert.ToInt32(vehicleId), "Vehicle");
            SentinelFM.GeomarkServiceRef.GeomarkServiceClient geomarkServiceClient = new GeomarkServiceClient("httpbasic");
            geomarkServiceClient.ReleaseCache();
            return "Success";
        }
        catch (Exception exception)
        {
            
            throw new Exception(exception.StackTrace);
        }
    }

    [System.Web.Services.WebMethod(EnableSession = true)]
    public static string GetFleetVehicles(string fleetId)
    {
        try
        {
            SentinelFMSession sn = HttpContext.Current.Session["SentinelFMSession"] as SentinelFMSession;
            if (sn == null) return "Fail";

            Dictionary<string, string> fleetVehicles = FillVehicles(Convert.ToInt32(fleetId));
            if (fleetVehicles != null)
            {
                var oSerializer = new JavaScriptSerializer();
                string json = oSerializer.Serialize(fleetVehicles);
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
    public static string GetServices(string input, string lookfor)
    {
        try
        {
            SentinelFMSession sn = HttpContext.Current.Session["SentinelFMSession"] as SentinelFMSession;
            if (sn == null) return "Fail";
            Dictionary<string, string> results = new Dictionary<string, string>();
            results = FilterService(input, lookfor);            

            if (results.Any())
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
    public static string GetConfiguredServices(string input, string lookfor)
    {
        try
        {
            SentinelFMSession sn = HttpContext.Current.Session["SentinelFMSession"] as SentinelFMSession;
            if (sn == null) return "Fail";
            Dictionary<string, string> results = new Dictionary<string, string>();            
            switch (lookfor.ToLower())
            {
                case "route":
                    //results = FilterRouteService(input);
                    results = GetConfiguredServices(4, input);
                    break;
                case "speed":
                    results = GetConfiguredServices(2, input);
                    break;
                case "sensor":
                    results = GetConfiguredServices(3, input);
                    break;
                case "landmark":
                    results = GetConfiguredServices(1, input);
                    break;

            }

            if (results.Any())
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

    private static IList<Dictionary<string, string>> ParseXml(string myXmlString, string searchSection, IList<string> searchNodes)
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

    private static Dictionary<string, string> FillVehicles(int fleetId)
    {
        SentinelFMSession sn = HttpContext.Current.Session["SentinelFMSession"] as SentinelFMSession;
        if (sn == null) return null;
        
        Dictionary<string, string> fleetVehicles = new Dictionary<string, string>();
        if (fleetId > 0)
        {
            string vehicleXml = null;
            DBFleet dbf = new DBFleet();
            dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref vehicleXml);
            if (string.IsNullOrEmpty(vehicleXml))
            {
                return null;
            }
            IList<Dictionary<string, string>> vehicles = ParseXml(vehicleXml, "VehiclesInformation",
                                                                         new string[] { "VehicleId", "Description" });
           
            ListItem listItem = new ListItem();
            foreach (var vehicle in vehicles)
            {                
                if (!fleetVehicles.ContainsKey(vehicle["VehicleId"]))
                {
                    fleetVehicles.Add(vehicle["VehicleId"], vehicle["Description"]);
                }
            }           
        }

        return fleetVehicles;
    }

    private void FillConfiguredService()
    {
        IList<Dictionary<string, string> > services = ServiceAssignment.GetConfiguredServices(ServiceId, sn.User.OrganizationId, sn.UserID, null);
        foreach (Dictionary<string, string> service in services)
        {
            ListItem item = new ListItem();
            item.Text = service["ServiceConfigName"];
            item.Value = service["ServiceConfigID"];
            if (!servicesList.Items.Contains(item))
            {
                servicesList.Items.Add(item);
            }
        }
        servicesList.DataBind();
    }

    public static Dictionary<string, string> GetConfiguredServices(int serviceId, string inputValue)
    {
        SentinelFMSession sn = HttpContext.Current.Session["SentinelFMSession"] as SentinelFMSession;
        if (sn == null) return null;
        IList<Dictionary<string, string>> services = ServiceAssignment.GetConfiguredServices(serviceId, sn.User.OrganizationId, sn.UserID, inputValue);
        Dictionary<string, string> results = new Dictionary<string, string>();
        foreach (Dictionary<string, string> service in services)
        {            
            if (!results.ContainsKey(service["ServiceConfigID"]))
            {
                results.Add(service["ServiceConfigID"], service["ServiceConfigName"]);
            }
        }
        return results;
    }

    private static Dictionary<string, string> FilterRouteService(string routeName)
    {
        SentinelFMSession sn = HttpContext.Current.Session["SentinelFMSession"] as SentinelFMSession;
        if (sn == null) return null;
        Dictionary<string, string> routeServices = new Dictionary<string, string>();
        IList<Dictionary<string, string>> services = ServiceAssignment.GetConfiguredServices(4, sn.User.OrganizationId, sn.UserID, routeName);
        foreach (Dictionary<string, string> service in services)
        {           
            if (!routeServices.ContainsKey(service["ServiceConfigID"]))
            {
                routeServices.Add(service["ServiceConfigID"], service["ServiceConfigName"]);
            }
        }

        return routeServices;
    }

    private static Dictionary<string, string> FilterService(string inputVal, string table)
    {
        SentinelFMSession sn = HttpContext.Current.Session["SentinelFMSession"] as SentinelFMSession;
        if (sn == null) return null;

        Dictionary<string, string> services = ServiceAssignment.GetFilteredData(sn.User.OrganizationId, sn.UserID,
            inputVal, table);

        foreach (KeyValuePair<string, string> service in services)
        {
            if (!services.ContainsKey(service.Key))
            {
                services.Add(service.Key, service.Value);
            }
        }

        return services;
    }



    /***
    private void FillFleets()
    {
        string xml = "";
        dbf.GetFleetsInfoXMLByUserId(sn.UserID, sn.SecId, ref xml);
        IList<Dictionary<string, string>> fleets = ParseXml(xml, "FleetsInformation", new string[] { "FleetId", "FleetName" });
        ListItem item = new ListItem();
        item.Text = "Please select a fleet";
        item.Value = "0";
        item.Selected = true;
        fleetsList.Items.Add(item);
        foreach (var fleet in fleets)
        {
            item = new ListItem();
            item.Text = fleet["FleetName"];
            item.Value = fleet["FleetId"];
            fleetsList.Items.Add(item);
        }
        fleetsList.DataBind();
    }
     * ***/
}