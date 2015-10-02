using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.Script.Serialization;
using ADODB;
using SentinelFM;
using System.Configuration;
using VLF.DAS.Logic;
using Newtonsoft.Json.Linq;

public partial class Widgets_GeoService : System.Web.UI.Page
{
    protected SentinelFMSession sn;
    private const string GoogleKey = "AIzaSyA4Bsl89cKuhy13bfuyyZazX3Y9GvrNfjo";
    private const string app_id = "v5HljYEynPujgUUkNmny";
    private const string app_code = "x14y1MmQaoSerjNQKGsABw";

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
            string layernames = Request.QueryString["layernames"];
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
            if (!layernames.Contains("GoogleAddress"))
                NeedGoogleApi = false;

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

            Dictionary<string, string> breakElement = new Dictionary<string, string>();
            breakElement.Add("description", "------------------------------");
            breakElement.Add("reference", "");

            string geoserver = Request.QueryString["geoserver"] ?? "https://ugeomap.sentinelfm.com/geoserver/wfs";
            //string layer = "oiltrax:oiltrax_poi_water";
            geoserver = geoserver.Replace("/wms", "/wfs");

            bool addedBreakElement = false;

            if (layernames != null && layernames.Trim() != string.Empty)
            {
                foreach (string layer in layernames.Split(';'))
                {
                    string cql = "";
                    switch (layer.Trim())
                    {
                        case "toronto:TDSBSchool":                        
                            cql = "strToLowerCase(SchName)%20like%20%27%25" + input.ToLower() + "%25%27";
                            break;
                        case "oiltrax:oiltrax_poi_water":
                        case "oiltrax:oiltrax_poi_wellsite":
                        case "oiltrax:oiltrax_poi_batteries":
                        case "oiltrax:oiltrax_poi_otherfacilities":
                        case "oiltrax:sask_lsd":
                        case "oiltrax:manbc_lsd":
                        case "oiltrax:alberta_lsd":
                        //case "oiltrax:trail": // don't search this layer, because all feature's name are same: "TRAIL"
                        //case "oiltrax:goat": // don't search this layer, because all feature's name are same: ""
                        //case "oiltrax:goat2": // don't search this layer, because all feature's name are same: "LEASE"
                        case "milton:PARKS_FACILITY1":
                        case "toronto:city_wards":
                            cql = "strToLowerCase(NAME)%20like%20%27%25" + input.ToLower() + "%25%27";
                            break;
                        case "milton:MILTON_ADDRESS_PTS":
                            cql = "strToLowerCase(DISPLAY_NA)%20like%20%27%25" + input.ToLower() + "%25%27";
                            break;
                        case "milton:Milton_Roads":
                            cql = "strToLowerCase(STREET_NAM)%20like%20%27%25" + input.ToLower() + "%25%27";
                            break;
                        case "Lirr:lirr_stations":
                            cql = "strToLowerCase(STATION_NA)%20like%20%27%25" + input.ToLower() + "%25%27";
                            break;
                        case "JMMGlobal:jmm_apec":
                            cql = "strToLowerCase(SiteName)%20like%20%27%25" + input.ToLower() + "%25%27";
                            break;
                        default:
                            cql = "";
                            break;
                    }

                    if (cql != "")
                    {
                        IList<Dictionary<string, string>> bsmResults = GetSentinelFmOverlays(geoserver, layer, cql);

                        if (bsmResults.Count > 0)
                        {
                            if (parsedResults.Count > 0 && !addedBreakElement)
                            {
                                parsedResults.Add(breakElement);
                                addedBreakElement = true;
                            }

                            foreach (Dictionary<string, string> bsmRow in bsmResults)
                            {
                                parsedResults.Add(bsmRow);
                            }
                        }
                    }
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
        else if (action.Equals("getCoord"))
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
            //string url = string.Format("http://route.nlp.nokia.com/routing/6.2/calculateroute.json?{0}", query);
            string url = string.Format("https://route.api.here.com/routing/7.2/calculateroute.json?{0}", query);
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
                    result.Add("Reason", exception.Message);
                    var jss = new JavaScriptSerializer();
                    json = jss.Serialize(result);
                }
            }
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

    private IList<Dictionary<string, string>> GetSentinelFmOverlays(string geoserver, string layer, string cql)
    {

        List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
        try
        {
            string url = string.Format(geoserver + "?service=WFS&version=1.0.0&request=GetFeature&typename={0}&outputFormat=json&CQL_FILTER={1}", layer, cql);
            string wfsResponse = null;
            using (WebClient client = new WebClient())
            {
                wfsResponse = client.DownloadString(url);
            }

            if (wfsResponse != null && wfsResponse.Trim() != string.Empty)
            {
                //overlay_attributes features = js.Deserialize<overlay_attributes>(wfsResponse);
                JObject features = JObject.Parse(wfsResponse);
                
                if (features["features"].Count() > 0)
                {
                    //foreach (overlay_feature f in features.features)
                    for (int i = 0; i < features["features"].Count() && i < 5; i++)
                    {
                        //overlay_feature f = features.features[i];
                        Dictionary<string, string> tmpResult = new Dictionary<string, string>();
                        string description = "";
                        switch (layer.Trim())
                        {
                            case "toronto:TDSBSchool":
                                //description = f.properties.SchName.Trim();
                                description = (string)(features["features"][i]["properties"]["SchName"]);
                                break;
                            case "oiltrax:oiltrax_poi_water":
                            case "oiltrax:oiltrax_poi_wellsite":
                            case "oiltrax:oiltrax_poi_batteries":
                            case "oiltrax:oiltrax_poi_otherfacilities":
                            case "oiltrax:sask_lsd":
                            case "oiltrax:manbc_lsd":
                            case "oiltrax:alberta_lsd":
                            //case "oiltrax:trail": // don't search this layer, because all feature's name are same: "TRAIL"
                            //case "oiltrax:goat": // don't search this layer, because all feature's name are same: ""
                            //case "oiltrax:goat2": // don't search this layer, because all feature's name are same: "LEASE"
                            case "milton:PARKS_FACILITY1":
                            case "toronto:city_wards":
                                description = (string)(features["features"][i]["properties"]["NAME"]);
                                break;
                            case "milton:MILTON_ADDRESS_PTS":
                                description = (string)(features["features"][i]["properties"]["DISPLAY_NA"]);
                                break;
                            case "milton:Milton_Roads":
                                description = (string)(features["features"][i]["properties"]["STREET_NAM"]);
                                break;
                            case "Lirr:lirr_stations":
                                description = (string)(features["features"][i]["properties"]["STATION_NA"]);
                                break;
                            case "JMMGlobal:jmm_apec":
                                description = (string)(features["features"][i]["properties"]["SiteName"]);
                                break;
                            default:
                                description = "";
                                break;
                        }
                        tmpResult.Add("description", description);
                        double lat = 0.0;
                        double lon = 0.0;
                        if ((string)(features["features"][i]["geometry"]["type"]) == "Point")
                        {
                            //lat = ((double[])f.geometry.coordinates)[1];
                            //lon = ((double[])f.geometry.coordinates)[0];
                            lat = (double)(features["features"][i]["geometry"]["coordinates"][1]);
                            lon = (double)(features["features"][i]["geometry"]["coordinates"][0]);
                        }
                        else if ((string)(features["features"][i]["geometry"]["type"]) == "MultiPolygon" && features["features"][i]["geometry"]["coordinates"][0][0].Count() > 0)
                        {
                            //double[][] coords = f.geometry.
                            double latsum = 0;
                            double lonsum = 0;
                            for (int icoord = 0; icoord < features["features"][i]["geometry"]["coordinates"][0][0].Count(); icoord++)
                            {
                                latsum += (double)(features["features"][i]["geometry"]["coordinates"][0][0][icoord][1]);
                                lonsum += (double)(features["features"][i]["geometry"]["coordinates"][0][0][icoord][0]);
                            }
                            lat = latsum / features["features"][i]["geometry"]["coordinates"][0][0].Count();
                            lon = lonsum / features["features"][i]["geometry"]["coordinates"][0][0].Count();
                        }
                        else if ((string)(features["features"][i]["geometry"]["type"]) == "MultiLineString" && features["features"][i]["geometry"]["coordinates"][0].Count() > 0)
                        {
                            int midleindex = (int)(Math.Floor(features["features"][i]["geometry"]["coordinates"][0].Count() / 2.0));

                            lat = (double)(features["features"][i]["geometry"]["coordinates"][0][midleindex][1]);
                            lon = (double)(features["features"][i]["geometry"]["coordinates"][0][midleindex][0]);
                        }
                        tmpResult.Add("reference", lat.ToString() + "," + lon.ToString());
                        results.Add(tmpResult);
                    }
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
}