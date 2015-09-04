using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Text;

/// <summary>
/// Summary description for GarminDestinationHandlerService
/// </summary>
public class GarminHandlerService
{
    static GarminHandlerService _Instance;

    public static GarminHandlerService Get
    {
        get
        {
            if (_Instance == null)
                _Instance = new GarminHandlerService();
            return _Instance;
        }
    }

    long _RequestID;
    public long RequestCounter { get { return _RequestID; } }

    Dictionary<string, string> _MsgMap;
    Dictionary<string, string> _UnitMap;
    Dictionary<string, string> _UserMap;
    object _SYNC;

    private GarminHandlerService()
    {
        _RequestID = 0;
        _SYNC = new object();
        _MsgMap = new Dictionary<string, string>();
        _UnitMap = new Dictionary<string, string>();
        _UserMap = new Dictionary<string, string>();
        lock (_SYNC)
        {
            //TXT=Pickup at Queen's Quay (42 boxes);LAT=43.6426764474564;LON=-79.3751480249319;ADD=;LNAME=
            _MsgMap.Add("msg", "TXT");
            _MsgMap.Add("lat", "LAT");
            _MsgMap.Add("lon", "LON");
            _MsgMap.Add("add", "ADD");
            _MsgMap.Add("y-n", "YESNO");
            _MsgMap.Add("int", "RETRYINTERVAL");
            _MsgMap.Add("lmk", "LNAME");
            _MsgMap.Add("lid", "LID");
            _UnitMap.Add("veh", "VEH");
            _UnitMap.Add("uid", "VID");
            _UserMap.Add("usr", "UID");
            _UserMap.Add("sid", "SID");
            _UserMap.Add("tkn", "TKN");
        }
    }

    public long Send(HttpRequest request, string token, string xml)
    {
        long messageID = -1;
        try
        {
            LogXML(request, xml);
            messageID = DBAccess.Get.Send(token, xml);
        }
        catch (Exception exc)
        {
            Trace.TraceError("ERROR -> [{0}]", exc.Message);
        }
        return messageID;
    }

    public int Send(HttpRequest request, out long requestID, out string output)
    {
        LogXML(request);
        requestID = ++_RequestID;
        output = string.Empty;
        int responseID = 200;
        long messageID = -1;
        try
        {
            if (request.RequestType == "POST")
            {
                string encxml = request.Params["xml"];
                string xml = HttpUtility.UrlDecode(encxml);
                string enctkn = request.Params["Token"];
                string tkn = HttpUtility.UrlDecode(enctkn);
                messageID = DBAccess.Get.Send(tkn, xml);
            }
            else
            {
                if (!string.IsNullOrEmpty(request.QueryString["xml"]))
                {
                    string tkn = request.QueryString["Token"];
                    string encxml = request.QueryString["xml"];
                    string xml = HttpUtility.UrlDecode(encxml);
                    messageID = DBAccess.Get.Send(tkn, xml);
                }
                else
                {

                    SessionHandler.ExceptionEvent += new SessionHandler.ExceptionHandler(SessionHandler_ExceptionEvent);
                    DBAccess.ExceptionEvent += new DBAccess.ExceptionHandler(SessionHandler_ExceptionEvent);
                    SessionState ss = null;
                    StringBuilder sb = new StringBuilder();


                    string unitName = string.Empty;
                    int unitID = 0;
                    string sessionID = string.Empty;
                    string token = string.Empty;
                    int userID = 0;
                    string address = string.Empty;
                    double lat = 0.0;
                    double lon = 0.0;
                    string landmark = string.Empty;
                    int orgID = 0;
                    long lid = 0;

                    foreach (string key in request.QueryString.Keys)
                    {
                        if (_MsgMap.ContainsKey(key))
                        {
                            switch (_MsgMap[key])
                            {
                                case "ADD":
                                    address = request.QueryString[key];
                                    break;
                                case "LAT":
                                    double.TryParse(request.QueryString[key], out lat);
                                    break;
                                case "LON":
                                    double.TryParse(request.QueryString[key], out lon);
                                    break;
                                case "LID":
                                    long.TryParse(request.QueryString[key], out lid);
                                    sb.AppendFormat("{0}={1};", _MsgMap[key], request.QueryString[key]);
                                    break;
                                case "LNAME":
                                    landmark = request.QueryString[key];
                                    sb.AppendFormat("{0}={1};", _MsgMap[key], request.QueryString[key]);
                                    break;
                                default:
                                    sb.AppendFormat("{0}={1};", _MsgMap[key], request.QueryString[key]);
                                    break;
                            }
                        }
                        else if (_UnitMap.ContainsKey(key))
                        {
                            switch (_UnitMap[key])
                            {
                                case "VEH":
                                    unitName = request.QueryString[key];
                                    break;
                                case "VID":
                                    int.TryParse(request.QueryString[key], out unitID);
                                    break;
                            }
                        }
                        else if (_UserMap.ContainsKey(key))
                        {
                            switch (_UserMap[key])
                            {
                                case "SID":
                                    sessionID = request.QueryString[key];
                                    break;
                                case "TKN":
                                    token = request.QueryString[key];
                                    orgID = SessionHandler.Get.Organization(token);
                                    break;
                                case "UID":
                                    int.TryParse(request.QueryString[key], out userID);
                                    break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(token))
                    {

                        string id = token.PadRight(24, '=');
                        ss = SessionHandler.Get.Session(id);

                    }
                    else if (!string.IsNullOrEmpty(sessionID) && userID > 0)
                    {

                    }

                    if (ss == null)
                        return 400;


                    output = sb.ToString();

                    GeoCoder.ResolverSoapClient g = new GeoCoder.ResolverSoapClient();

                    if (output.Contains("LNAME"))
                    {
                        Landmark l = DBAccess.Get.GetLandmarkInfo(landmark, orgID);
                        if (l == null)
                        {
                            if (!output.Contains("LAT") || !output.Contains("LON"))
                            {
                                sb.AppendFormat("{0}={1};", "LAT", string.Empty);
                                sb.AppendFormat("{0}={1};", "LON", string.Empty);
                                sb.AppendFormat("{0}={1};", "ADD", string.Empty);
                            }
                        }
                        else
                        {
                            lat = l.Latitude;
                            lon = l.Longitude;
                            lid = l.LandmarkId;
                            if (!output.Contains("LAT") || !output.Contains("LON"))
                            {
                                sb.AppendFormat("{0}={1};", "LAT", l.Latitude);
                                sb.AppendFormat("{0}={1};", "LON", l.Longitude);
                            }

                            if (!g.StreetAddress(lat, lon, ref address))
                            {
                                sb.AppendFormat("{0}={1};", "ADD", string.Empty);
                            }
                            else
                            {
                                sb.AppendFormat("{0}={1};", "ADD", address.Replace(';', ':'));
                            }
                        }
                    }
                    else
                    {
                        sb.AppendFormat("{0}={1};", "LNAME", string.Empty);
                        if (!string.IsNullOrEmpty(address))
                        {
                            if (!g.Location(address, ref lat, ref lon, ref address))
                            {
                                //fire error...
                                sb.AppendFormat("{0}={1};", "ADD", string.Empty);
                            }
                            if (!output.Contains("ADD"))
                            {
                                sb.AppendFormat("{0}={1};", "ADD", address.Replace(';', ':'));
                            }

                        }
                        else
                        {
                            if (!output.Contains("ADD"))
                            {
                                if (!output.Contains("LAT") || !output.Contains("LON"))
                                {
                                    sb.AppendFormat("{0}={1};", "LAT", string.Empty);
                                    sb.AppendFormat("{0}={1};", "LON", string.Empty);
                                    sb.AppendFormat("{0}={1};", "ADD", string.Empty);
                                }
                                else
                                {

                                    if (!g.StreetAddress(lat, lon, ref address))
                                    {
                                        //fire error...
                                        sb.AppendFormat("{0}={1};", "ADD", string.Empty);
                                    }
                                    else
                                    {
                                        sb.AppendFormat("{0}={1};", "ADD", address.Replace(';', ':'));
                                    }
                                }
                            }
                        }
                    }
                    if (!output.Contains("TXT"))
                        sb.AppendFormat("{0}={1};", "TXT", address);
                    if (!output.Contains("LID"))
                        sb.AppendFormat("{0}={1};", "LID", lid);
                    if (!output.Contains("RETRYINTERVAL"))
                        sb.AppendFormat("{0}={1};", "RETRYINTERVAL", 5);
                    if (!output.Contains("LIFETIME"))
                        sb.AppendFormat("{0}={1};", "LIFETIME", 300);

                    if (lat != null && lon != null)
                    {
                        if (string.IsNullOrEmpty(landmark))
                            landmark = "UNKNOWN";
                        if (!DBAccess.Get.CreateVirtualLandmark(orgID, landmark, lat, lon, address))
                        {

                        }

                    }

                    output = sb.ToString();
                    //output = string.Format("user:[{0}] sid:[{1}] unit:[{2}] unitID:[{3}] Properties:[{4}] Authorized:[{5}]", ss.UserID, ss.SessionID, unitName, unitID, sb, ss.IsAuthenticated);
                    messageID = DBAccess.Get.Send(userID, unitName, output);
                }
                output = string.Format("{0}\"mid\":{1}{2}", '{', messageID, '}');
            }
        }
        catch (Exception exc)
        {
            Trace.TraceError("ERROR -> [{0}]", exc.Message);
        }
        finally
        {
            output = string.Format("{0}\"mid\":{1}{2}", '{', messageID, '}');
        }
        return responseID;
    }

    void SessionHandler_ExceptionEvent(Exception exception)
    {
        Trace.TraceError("ERROR -> [{0}]", exception.Message);
    }


    void LogXML(HttpRequest request)
    {
        try
        {
            string xml = string.Empty;
            if (request.HttpMethod == "POST")
            {
                if (!string.IsNullOrEmpty(request.Params["xml"]))
                {
                    xml = request.Params["xml"];
                }
            }
            else if (request.HttpMethod == "GET")
            {
                if (!string.IsNullOrEmpty(request.QueryString["xml"]))
                {
                    xml = request.QueryString["xml"];
                }
            }
            if (!string.IsNullOrEmpty(xml))
            {
                xml = HttpUtility.UrlDecode(xml);
                string datapath = @"c:\SentinelFM\Log\xml\";
                string filename = string.Format("{0}-{1}.xml", DateTime.Now.ToString("yyyyMMddHHmmss"), request.UserHostAddress.Replace(":", "_"));
                using (StreamWriter sw = new StreamWriter(Path.Combine(datapath, filename)))
                {
                    sw.Write(xml);
                    sw.Flush();
                }
            }
        }
        catch { }
    }

    void LogXML(HttpRequest request, string xml)
    {
        try
        {
            if (!string.IsNullOrEmpty(xml))
            {
                xml = HttpUtility.UrlDecode(xml);
                string datapath = @"c:\SentinelFM\Log\xml\";
                string filename = string.Format("{0}-{1}.xml", DateTime.Now.ToString("yyyyMMddHHmmss"), request.UserHostAddress.Replace(":", "_"));
                using (StreamWriter sw = new StreamWriter(Path.Combine(datapath, filename)))
                {
                    sw.Write(xml);
                    sw.Flush();
                }
            }
        }
        catch { }
    }

}