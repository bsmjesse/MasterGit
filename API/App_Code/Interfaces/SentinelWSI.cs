using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;

//using System.ComponentModel;
using VLF.ERRSecurity;
using VLF.DAS.Logic;
using System.Data;
using System.Diagnostics;
//using System.Security.Cryptography;
using VLF.CLS;

using GeoCoder;
//using GSI;

namespace VLF.ASI.Interfaces
{
    //////// Define a SOAP header by deriving from the SoapHeader base class.
    //////public class AuthenticationHeader : System.Web.Services.Protocols.SoapHeader
    //////{
    //////    public string UserName;
    //////    public string UserPassword;
    //////}

    /// <summary>
    /// Summary description for BSMCustomService
    /// </summary>
    [WebService(Namespace = "http://www.sentinelfm.com", Description = "Sentinel Web Service Methods Exposer.", Name = "SentinelWSI")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    //[WebServiceBinding(ConformsTo = WsiProfiles.None)]
    public class SentinelWSI : System.Web.Services.WebService
    {
        //GSI.GarminInterface iGarminInterface = new GarminInterface();
        string hierarchyIDs = string.Empty;
        string fleetIDs = string.Empty;
        string udf = string.Empty;
        string customOutputFields = string.Empty;

        string boxIDs = string.Empty;
        string vehicleIDs = string.Empty;
        string vehicleDescriptions = string.Empty;

        //for GetSelectedVehiclesLastKnownPositionInfo
        string sFleetIDs = String.Empty;
        string sVehicleIDs = String.Empty;
        string sBoxIDs = String.Empty;
        string sVehicleDescriptions = String.Empty;
        string sOptionalOutputFields = String.Empty;

        //PreTemplated
        string reqDataXmlns = string.Empty;
        string reqDataSysId = string.Empty;
        string reqDataVersion = string.Empty;
        string reqDataRptTime = string.Empty;

        string reqMsgDateTime = string.Empty;
        string reqMsgSubject = string.Empty;
        string reqMsgType = string.Empty;
        string reqMsgFrom = string.Empty;
        string reqMsgTo = string.Empty;

        string reqBodyDateTime = string.Empty;

        string resErrMsg = string.Empty;
        string resErrorMsg = @"<?xml version=""1.0"" encoding=""UTF-8"" ?><data xmlns=""https://www.aff.gov/affSchema"" version=""{0}"" sysID=""{1}"" rptTime=""{2}""> <msgList> <msg to=""{3}"" from=""{4}"" msgType=""{5}"" subject=""{6}"" dateTime=""{7}""> <body>{8}</body></msg></msgList></data>";

        System.Xml.XmlReaderSettings readerSettings = new System.Xml.XmlReaderSettings();
        System.IO.StringReader strXML;
        DataSet iDataSet;

        System.Text.StringBuilder builderOuter;
        System.Text.StringBuilder builderInnerOuter;
        System.Text.StringBuilder builderInner;

        public SentinelWSI()
        {
            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        //[WebMethod (MessageName = "AddInt", Description = "Add two integer Value", EnableSession = true)]
        //public int Add(int a, int b)
        //{
        //    return (a + b);
        //}

        //[WebMethod(MessageName = "AddFloat", Description = "Add two Float Value", EnableSession = true)]
        //public float Add(float a, float b)
        //{
        //    return (a + b);
        //}

        #region Component Designer generated code

        ////Required by the Web Services Designer 
        //private IContainer components = null;

        ///// <summary>
        ///// Required method for Designer support - do not modify
        ///// the contents of this method with the code editor.
        ///// </summary>
        //private void InitializeComponent()
        //{
        //}

        ///// <summary>
        ///// Clean up any resources being used.
        ///// </summary>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && components != null)
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        #endregion

        #region USER LOG ON

        [WebMethod(Description = "Used to log on to the system. XML Format :[userName],[password],[userIp],[userId],[SID]")]//,EnableSession=true)]

        public int Login(string userName, string password, string userIp, ref int userId, ref string SID)
        {
            using (SecurityManager iSecurityManager = new SecurityManager())
            {
                return iSecurityManager.Login(userName, password, userIp, ref userId, ref SID);
            }
        }
        #endregion

        #region FLEET/VEHICLE
        /// <summary>
        /// Returns fleets information by User Id. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns> XML File Format: [OrganizationName],[FleetId],[FleetName],[FleetDescription]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = @"Returns XML of Fleets by User ID. XML Format: [OrganizationName],[FleetId],[FleetName],[FleetDescription]")]
        public int GetFleetsInfoXMLByUserId(int userId, string SID, ref string xml)
        {
            using (DBFleet iDBFleet = new DBFleet())
            {
                return iDBFleet.GetFleetsInfoXMLByUserId(userId, SID, ref xml);
            }
        }

        /// <summary>
        /// Returns vehicles last position information by Fleet Id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>
        /// XML File Format: 
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],[FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Fleet ID. XML Format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetVehiclesLastPositionInfo(int userId, string SID, int fleetId, ref string xml) 
        {
            using (DBFleet iDBFleet = new DBFleet())
            {
                return iDBFleet.GetVehiclesLastKnownPositionInfo(userId, SID, fleetId, ref xml);
            }
        }

        /// <summary>
        /// Returns vehicles last position information by Fleet Id or list of vehicle ids
        /// or list of box ids or list of vehicle descriptions. Optional fields are available.
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>
        /// XML File Format: 
        /// FleetId, LicensePlate, VehicleId, BoxId, LastValidDateTime, Latitude, Longitude, Speed, Heading, StreetAddress
        /// Optional Fields: SensorMask, BoxArmed, GeoFenceEnabled, IconTypeName, VehicleTypeName
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position optional information by <Filter>FleetIDs:{};VehicleIDs:{};BoxIDs:{};sVehicleDescriptions:{};OptionalOutputFields:{}</Filter>. XML Format: [VehicleId],[VehicleDescription],[LastValidDateTime],[Latitude],[Longitude],[Speed],[Heading],[StreetAddress]")]
        public int GetSelectedVehiclesLastKnownPositionInfo(int userId, string SID, string filter, ref string xml)
        {
            Log("<< filter: {0}", filter);
            GetFilterValues(filter);

            using (DBFleet iDBFleet = new DBFleet())
            {
                return iDBFleet.GetSelectedVehiclesLastKnownPositionInfo(userId, SID, sFleetIDs, sVehicleIDs, sBoxIDs, sVehicleDescriptions, sOptionalOutputFields, ref xml);
            }
        }

        void GetFilterValues(string filter)
        {
            try
            {
                //string message = "<Filter> FleetIDs:{5ZE,159}; BoxIDs:{}; VehicleDescriptions:{rnd@bsmwireless.com}; OptionalOutputFields:{Hierarchy,Field1,Field2};</Filter>";
                using (System.IO.TextReader iTextReader = new System.IO.StringReader(filter))
                {
                    using (System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(iTextReader))//  Load the XML file
                    {
                        while (xmlReader.Read())//  Loop over the XML file
                        {
                            //  Here we check the type of the node, in this case we are looking for element
                            if (xmlReader.NodeType == System.Xml.XmlNodeType.Element)
                            {
                                if (xmlReader.Name.ToUpper() == "FILTER")//  If the element is "*****"
                                {
                                    string elementContent = xmlReader.ReadElementString();
                                    GetFilterValues(elementContent, new string[] { "FleetIDs", "VehicleIDs", "BoxIDs", "VehicleDescriptions", "OptionalOutputFields" });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Returns devices last position information by selection criteria. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="selectionCriteria"></param>
        /// <param name="xml"></param>
        /// <returns>
        /// XML File Format: 
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description],[IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],
        /// [PrevStatusSensor],[PrevStatusSpeed],[FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by selection criteria. XML Format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetDevicesLastPosition(int userId, string SID, string selectionCriteria, ref string xml)
        {
            using (DBFleet iDBFleet = new DBFleet())
            {
                return iDBFleet.GetDevicesLastKnownPosition(userId, SID, selectionCriteria, ref xml);
            }
        }
        
        /// <summary>
        /// Returns vehicles last position custom information by Hierarchy, Fleet, UserName (UDF1 like abc@bsm.com). 
        /// </summary>
        /// <param name="filterHierarchy"></param>
        /// <returns>
        /// XML File Format: 
        /// [VehicleId],[Latitude],[Longitude],[LastCommunicatedDateTime],
        /// [CustomOutputFields] (like Hierarchy, Field1, Field2)
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>

        [WebMethod(Description = "Returns XML of vehicles with last known position custom information by <Filter> Hierarchy:{}; Fleet:{}; UDF:{}; CustomOutputFields:{} </Filter> (UDF like F1=abc@bsm.com,F2=abc where F1 is UserName and CustomOutputFields like Hierarchy, Field1, Field2 etc.). XML Format: [VehicleId],[Latitude],[Longitude],[LastCommunicatedDateTime],[CustomOutputFields]")]
        public int GetVehiclesLastPositionCustomInfo(int userId, string SID, string filter, ref string xml)
        {
            Log("<< FilterPayload: {0}", filter);
            ReadFilter(filter);
            using (DBFleet iDBFleet = new DBFleet())
            {
                return iDBFleet.GetVehiclesLastKnownPositionCustomInfo(userId, SID, hierarchyIDs, fleetIDs, udf, customOutputFields, ref xml);
            }
        }

        //MNNR IS Pre-Templated
        [WebMethod(Description = "Returns Pre-Templated XML of devices with last known position custom information (pre-templated).", BufferResponse = false)]
        //by <Filter> Hierarchy:{}; Fleet:{}; UDF:{}; CustomOutputFields:{} </Filter> (UDF like F1=abc@bsm.com,F2=abc where F1 is UserName and CustomOutputFields like Hierarchy, Field1, Field2 etc.). XML Format: [VehicleId],[Latitude],[Longitude],[LastCommunicatedDateTime],[CustomOutputFields]")]
        //public int GetVehiclesLastKnownPositionPreTemplatedInfo(int userId, string SID, string filter, ref string xml)
        public void GetDevicesLastKnownPositionPreTemplatedInfo_AutomotedAgent(string inputRequest)
        {
            string xml = string.Empty;
            //string body = "";
            System.Collections.Specialized.NameValueCollection query = HttpUtility.ParseQueryString(HttpContext.Current.Request.QueryString.ToString());

            if (query.Count > 0)
            {
                GetDevicesLastKnownPositionPreTemplatedInfo_Internal(query["uId"], query["pwd"], inputRequest, ref xml);
                //Mapping Pre-Template
                if (xml.Length > 0)
                {
                    MapPreTemplate(xml, ref xml);
                }
                else
                {
                    xml = GenerateErrorMessage("INFO", "No records found.", string.Format("No records found for the given datetime {0}.", reqBodyDateTime));
                }
            }
            else
            {
                xml = GenerateErrorMessage("ERROR", "Invalid user/pwd", "");
            }

            //xml = msgRequest + body;
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "text/xml";
            HttpContext.Current.Response.AddHeader("Content-Length", xml.Length.ToString());
            HttpContext.Current.Response.Write(xml);
            HttpContext.Current.Response.End();// .Flush();
            HttpContext.Current.Response.Close();
        }

        
        [WebMethod(Description = "Returns Pre-Templated XML of devices with last known position custom information (pre-templated).", BufferResponse = false)]
        //by <Filter> Hierarchy:{}; Fleet:{}; UDF:{}; CustomOutputFields:{} </Filter> (UDF like F1=abc@bsm.com,F2=abc where F1 is UserName and CustomOutputFields like Hierarchy, Field1, Field2 etc.). XML Format: [VehicleId],[Latitude],[Longitude],[LastCommunicatedDateTime],[CustomOutputFields]")]
        //public int GetVehiclesLastKnownPositionPreTemplatedInfo(int userId, string SID, string filter, ref string xml)
        public void GetDevicesLastKnownPositionPreTemplatedInfo(string userName, string password, string inputRequest, ref string xml)
        {
            //string xml = string.Empty;
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                GetDevicesLastKnownPositionPreTemplatedInfo_Internal(userName, password, inputRequest, ref xml);
                //Mapping Pre-Template
                MapPreTemplate(xml, ref xml);
            }
            else
            {
                xml = GenerateErrorMessage("ERROR", "Invalid user/pwd","");
            }
        }

        int GetDevicesLastKnownPositionPreTemplatedInfo_Internal(string userName, string password, string filter, ref string xml)
        {
            /*User: WSI_MNR
              PWD: TWpJNU1EZz0=1000050@Gps1$us
              PWD: 1000050@Gps1$us*/
            //UserId: 22908 (with single enc 'MjI5MDg=' and dbl enc 'TWpJNU1EZz0='
            //Login(string userName, string password, string userIp, ref int userId, ref string SID)

	    password = "TWpJNU1EZz0=1000050@Gps1$us";
            int userId = SecurityKeyBypassManager.GetUserIdFromPasswordStr(password);
            //string SID = string.Empty;

            //LoginValue = Login(userName, password, string.Empty, ref userId, ref SID);
            //LoginStatusLog(LoginValue);
            int checkReutrnValue = 0;

            Log("<< Pre-TemplatedFilterPayload: {0}", filter);
            ReadPreTemplatedFilter(filter);
            //Convert.ToDateTime("2014-05-28T19:19:32.000Z").ToUniversalTime()
            if (userId > -1)
            {
                using (DBFleet iDBFleet = new DBFleet())
                {
                    //return iDBFleet.GetVehiclesLastKnownPositionInfo(userId, ref xml);
                    checkReutrnValue = iDBFleet.GetDevicesLastKnownPositionPreTemplatedInfo(userId, Convert.ToDateTime(reqBodyDateTime).ToUniversalTime(), ref xml);
                    if (checkReutrnValue != 0)
                    {
                        WriteReturnValueStatusLog(checkReutrnValue);
                        //resErrorMsg = string.Format(resErrorMsg, reqDataVersion, reqDataSysId, reqDataRptTime, reqMsgFrom, reqMsgTo, "ERROR", "Invalid request", DateTime.UtcNow.ToString("o"), ".....body....." + resErrMsg);
                        resErrorMsg = GenerateErrorMessage("ERROR", "Invalid request", "");
                        xml = resErrorMsg;
                    }
                    //xml = "Mapping Goes Here.....";
                    return checkReutrnValue;
                }
            }
            else
            {
                //The to field in the response should be the from field value of the request.
                //The msgType should be the test “ERROR” and the subject may be blank or a categorical description of the error.
                //The dateTime will be the time the msg was created, and the body may contain a verbose description of the specific error.
                
                //string resErrorMsg = @"<?xml version=""1.0"" encoding=""UTF-8"" ?><data xmlns=""https://www.aff.gov/affSchema"" version=""{0}"" sysID=""{1}"" rptTime=""{2}""> <msgList> <msg to=""{3}"" from=""{4}"" msgType=""{5}"" subject=""{6}"" dateTime=""{7}""> <body>{8}</body></msg></msgList></data>";
                //resErrorMsg = string.Format(resErrorMsg, reqDataVersion, reqDataSysId, reqDataRptTime, reqMsgFrom, reqMsgTo, "ERROR", "Invalid user request", DateTime.UtcNow.ToString("o"), ".....body....." + resErrMsg);
                resErrorMsg = GenerateErrorMessage("ERROR", "Invalid user request", "");
                xml = resErrorMsg;
                return -1;
            }
        }

        string GenerateErrorMessage(string msgType, string subject, string body)
        {
            return string.Format(resErrorMsg, reqDataVersion, reqDataSysId, reqDataRptTime, reqMsgFrom, reqMsgTo, msgType, subject, DateTime.UtcNow.ToString("o"), body);
        }

        void ReadFilter(string filter)
        {
            try
            {
                //string message = "<Filter> Hierarchy:{5ZE,159}; Fleet:{}; UserName:{rnd@bsmwireless.com}; CustomOutputFields:{Hierarchy,Field1,Field2};</Filter>";
                using (System.IO.TextReader iTextReader = new System.IO.StringReader(filter))
                {
                    using (System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(iTextReader))//  Load the XML file
                    {
                        while (xmlReader.Read())//  Loop over the XML file
                        {
                            //  Here we check the type of the node, in this case we are looking for element
                            if (xmlReader.NodeType == System.Xml.XmlNodeType.Element)
                            {
                                if (xmlReader.Name.ToUpper() == "FILTER")//  If the element is "*****"
                                {
                                    string elementContent = xmlReader.ReadElementString();
                                    GetFilterValues(elementContent, new string[] { "Hierarchy", "Fleet", "UDF", "CustomOutputFields" });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        void GetFilterValues(string inputStr, string[] filters)
        {
            foreach (string findStr in filters)
            {
                int inputLen = findStr.Length + 1;
                string ret = "";
                if ((inputStr.Contains(string.Format("{0}:", findStr))) && (!string.IsNullOrEmpty(inputStr)))
                {
                    ret = inputStr.Substring(inputStr.IndexOf(findStr + ":"));
                    ret = ret.Remove(0, inputLen).Trim();
                    if (ret.Length > 0)
                    {
                        if (ret.Contains(";"))
                        {
                            ret = ret.Substring(0, ret.IndexOf(';')).Replace("{", "").Replace("}", "");
                        }
                        else
                        {
                            ret = ret.Replace("{", "").Replace("}", "");
                        }
                    }
                }
                SetFilterValues(findStr, ret);
            }
        }

        void SetFilterValues(string key, string value)
        {
            switch (key)
            {
                case "Hierarchy":
                    hierarchyIDs = SetSingleQuote(value);
                    break;
                case "Fleet":
                    fleetIDs = SetSingleQuote(value);
                    break;
                case "UDF":
                    //userNames = SetSingleQuote(value);
                    udf = value;
                    break;
                case "CustomOutputFields":
                    customOutputFields = MapCustomOutputFields(value);
                    break;
                case "FleetIDs":
                    sFleetIDs = value;
                    break;
                case "VehicleIDs":
                    sVehicleIDs = value;
                    break;
                case "BoxIDs":
                    sBoxIDs = value;
                    break;
                case "VehicleDescriptions":
                    sVehicleDescriptions = SetSingleQuote(value);
                    break;
                case "OptionalOutputFields":
                    sOptionalOutputFields = value;
                    break;
                default:
                    break;
            }
        }

        string SetSingleQuote(string input)
        {
            string output = "";
            string[] a = input.Split(',');

            foreach (string str in a)
            {
                if (!string.IsNullOrEmpty(str))
                    output += string.Format("'{0}',", str.Replace("'", "''"));
            }
            if (!string.IsNullOrEmpty(output))
                output = output.Substring(0, output.Length - 1);

            return output;
        }

        string MapCustomOutputFields(string input)
        {
            string output = string.Empty;
            string[] a = null;

            if (input.Length > 1)
            {
                a = input.Split(',');

                foreach (string str in a)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        //Hierarchy,Field1,Field2
                        //output += string.Format("'{0}',", str);
                        switch (str)
                        {
                            case "Hierarchy":
                                output += ",vlfFleet.NodeCode AS Hierarchy";
                                break;
                            case "Field1":
                                //output += ",vlfVehicleInfo.Field1 AS Field1";
                                output += ",Field1";
                                break;
                            case "Field2":
                                output += ",vlfVehicleInfo.Field2 AS Field2";
                                break;
                            case "LicensePlate":
                                output += ",vlfVehicleAssignment.LicensePlate";
                                break;
                            case "BoxId":
                                output += ",vlfBox.BoxId";
                                break;
                            case "OriginDateTime":
                                output += ",CASE WHEN vlfBox.LastValidDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastValidDateTime) END AS OriginDateTime";
                                break;
                            case "Speed":
                                output += ",CASE WHEN vlfBox.LastSpeed IS NULL then 0 ELSE ROUND(vlfBox.LastSpeed * @Unit,1) END AS Speed";
                                break;
                            case "StreetAddress":
                                output += ",ISNULL(CASE WHEN @ResolveLandmark=0 then vlfBox.LastStreetAddress ELSE CASE WHEN vlfBox.NearestLandmark IS NULL then vlfBox.LastStreetAddress ELSE vlfBox.NearestLandmark END END,'Address resolution in progress') AS StreetAddress";
                                break;
                            case "LastStatusDateTime":
                                output += ",vlfBox.LastStatusDateTime";
                                break;
                            //case "VehicleId":
                            //    output += ",vlfVehicleInfo.VehicleId AS Vehicle_Id";//need 2 discuss
                            //    break;
                            //case "Heading":
                            //    output += ",ISNULL(vlfBox.LastHeading,0) AS Heading";
                            //    break;
                            //case "SensorMask":
                            //    output += ",ISNULL(vlfBox.LastSensorMask,0) AS SensorMask";
                            //    break;
                            //case "BoxArmed":
                            //    output += ",CASE WHEN vlfBox.BoxArmed=0 then 'false' ELSE 'true' END AS BoxArmed";
                            //    break;
                            //case "GeoFenceEnabled":
                            //    output += ",vlfBox.GeoFenceEnabled";
                            //    break;
                            //case "IconTypeId":
                            //    output += ",vlfVehicleInfo.IconTypeId";
                            //    break;
                            //case "IconTypeName":
                            //    output += ",vlfIconType.IconTypeName";
                            //    break;
                            //case "VehicleTypeId":
                            //    output += ",vlfVehicleType.VehicleTypeId";
                            //    break;
                            //case "VehicleTypeName":
                            //    output += ",ISNULL(vlfVehicleType.VehicleTypeName,'') AS VehicleTypeName";
                            //    break;
                            //case "LastStatusSensor":
                            //    output += ",ISNULL(LastStatusSensor," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS LastStatusSensor";
                            //    break;
                            //case "LastStatusSpeed":
                            //    output += ",ISNULL(LastStatusSpeed," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS LastStatusSpeed";
                            //    break;
                            //case "PrevStatusDateTime":
                            //    output += ",CASE WHEN vlfBox.PrevStatusDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.PrevStatusDateTime) END AS PrevStatusDateTime";
                            //    break;
                            //case "PrevStatusSensor":
                            //    output += ",ISNULL(PrevStatusSensor," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS PrevStatusSensor";
                            //    break;
                            //case "PrevStatusSpeed":
                            //    output += ",ISNULL(PrevStatusSpeed," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS PrevStatusSpeed";
                            //    break;
                            //case "FwTypeId":
                            //    output += ",vlfFirmware.FwTypeId";
                            //    break;
                            //case "Dormant":
                            //    output += ",ISNULL(Dormant,0) AS Dormant";
                            //    break;
                            //case "DormantDateTime":
                            //    output += ",CASE WHEN vlfBox.DormantDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.DormantDateTime) END AS DormantDateTime";
                            //    break;
                            default:
                                break;
                        }
                    }
                }
            }

            return output;
        }

        // July 19, 2013
        void ReadSelectionCriterion(string selectionCriterion)
        {
            try
            {
                //string message = "<Select> BoxId:{10000,10001,10269,101196,9277,606060};VehicleId:{147,258,369,8118,44131,25411,10807};VehicleDescription:{Randy-2000,test12,9279_Bell_Ravi,939393-QingQing,Albans Mobile,Alban's Rogers G24}</Select>";
                using (System.IO.TextReader iTextReader = new System.IO.StringReader(selectionCriterion))
                {
                    using (System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(iTextReader))//  Load the XML file
                    {
                        while (xmlReader.Read())//  Loop over the XML file
                        {
                            //  Here we check the type of the node, in this case we are looking for element
                            if (xmlReader.NodeType == System.Xml.XmlNodeType.Element)
                            {
                                if (xmlReader.Name.ToUpper() == "SELECT")
                                {
                                    string elementContent = xmlReader.ReadElementString();

                                    GetSelectionValues(elementContent, new string[] { "BoxId", "VehicleId", "VehicleDescription" });
                                }
                                //else if (xmlReader.Name == "Stop")
                                //{
                                //}
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        void GetSelectionValues(string inputStr, string[] selections)
        {
            foreach (string findStr in selections)
            {
                int inputLen = findStr.Length + 1;
                string ret = "";
                if ((inputStr.Contains(string.Format("{0}:", findStr))) && (!string.IsNullOrEmpty(inputStr)))
                {
                    ret = inputStr.Substring(inputStr.IndexOf(findStr + ":"));
                    ret = ret.Remove(0, inputLen).Trim();
                    if (ret.Length > 0)
                    {
                        if (ret.Contains(";"))
                        {
                            ret = ret.Substring(0, ret.IndexOf(';')).Replace("{", "").Replace("}", "");
                        }
                        else
                        {
                            ret = ret.Replace("{", "").Replace("}", "");
                        }
                    }
                }
                SetSelectionValues(findStr, ret);
            }
        }

        void SetSelectionValues(string key, string value)
        {
            switch (key)
            {
                case "BoxId":
                    boxIDs = string.Format(" vlfBox.BoxId IN({0})", value);
                    break;
                case "VehicleId":
                    vehicleIDs = string.Format(" vlfVehicleInfo.VehicleId IN({0})", value);
                    break;
                case "VehicleDescription":
                    vehicleDescriptions = string.Format(" vlfVehicleInfo.Description IN({0})", SetSingleQuote(value));
                    break;
                default:
                    break;
            }
        }
        //----------------------

        /// <summary>
        /// Returns vehicle information by Fleet Id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>XML File Format: [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles for a particular Fleet by Fleet ID. XML Format: [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description] ")]
        public int GetVehiclesInfoXMLByFleetId(int userId, string SID, int fleetId, ref string xml)
        {
            using (DBFleet iDBFleet = new DBFleet())
            {
                return iDBFleet.GetVehiclesInfoXMLByFleetId(userId, SID, fleetId, ref xml);
            }
        }

        //SALMAN (01/03/2012)
        /// <summary>
        /// Returns vehicle information by Vehicle Id. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vehicleId"></param>
        /// <param name="xmlResult"></param>
        /// <returns>XML [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],[Email]</returns>
        ////[WebMethod(Description = "Retrieves vehicle information by Vehicle ID. XML File format:[LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],[Email]")]
        ////public int GetVehicleInfoXMLByVehicleId(int userId, string SID, Int64 vehicleId, ref string xmlResult)
        ////{
        ////    using (DBVehicle iDBVehicle = new DBVehicle())
        ////    {
        ////        return iDBVehicle.GetVehicleInfoXMLByVehicleId(userId, SID, vehicleId, ref xmlResult);
        ////    }
        ////}

        //SALMAN (02/03/2012)
        /// <summary>
        /// Resolve street address and return Lat-Lon-ResolvedAddress.
        /// </summary>
        /// <param name="streetAddresses"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="xmlResolvedAddress"></param>
        /// <returns>ASI Error Code</returns>
        ////[WebMethod(Description = "Resolve latitude and longitude by address")]
        ////public bool GetLatLongByAddress(string streetAddresses, ref double latitude, ref double longitude, ref string xmlResolvedAddress)
        ////{
        ////    using (ResolverSoapClient iResolver = new GeoCoder.ResolverSoapClient())
        ////    {
        ////        return iResolver.Location(streetAddresses, ref latitude, ref longitude, ref xmlResolvedAddress);
        ////    }
        ////}

        void ReadPreTemplatedFilter(string filter)
        {
            try
            {
                //string message = @"<?xml version=""1.0"" encoding=""UTF-8"" ?> <data xmlns=""https://www.aff.gov/affSchema"" sysID=""AFF-1"" version=""2.23"" rptTime=""2004-10-18T19:20:02.000Z""><msgRequest to=""BSM"" from=""AFF-1"" msgType=""Data Request"" subject=""Sync"" dateTime=""2004-10-18T19:20:02.000Z""><body>2004-10-17T19:19:32.000Z</body></msgRequest></data>";
                //filter = message;
                
                //readerSettings.ProhibitDtd = false;

                if (false == string.IsNullOrEmpty(filter))
                {
                    using (System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(new System.IO.StringReader(filter), readerSettings))
                    {
                        if (xmlReader.ReadToFollowing("data"))
                        {
                            reqDataXmlns = xmlReader.GetAttribute("xmlns");
                            reqDataSysId = xmlReader.GetAttribute("sysID");
                            reqDataVersion = xmlReader.GetAttribute("version");
                            reqDataRptTime = xmlReader.GetAttribute("rptTime");
                        }
                        //string wwww = xmlReader.ReadElementContentAsString();// .ReadElementContentAsString();
                        //string anot = xmlReader.Value;
                        if (xmlReader.ReadToFollowing("msgRequest"))
                        {
                            reqMsgDateTime = xmlReader.GetAttribute("dateTime");
                            reqMsgSubject = xmlReader.GetAttribute("subject");
                            reqMsgType = xmlReader.GetAttribute("msgType");
                            reqMsgFrom = xmlReader.GetAttribute("from");
                            reqMsgTo = xmlReader.GetAttribute("to");
                        }

                        if (xmlReader.ReadToFollowing("body"))
                        {
                            reqBodyDateTime = xmlReader.ReadElementContentAsString();//xmlReader.GetAttribute("dateTime");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        void MapPreTemplate(string inputXml, ref string outputXml)
        {
//            string dataInner = @"<acPos dateTime=""2014-05-28T18:13:23Z"" dataCtr=""BlueSky"" PDOP=""2"" fix=""3D"" source=""GPS"" UnitID=""DM10060"" esn=""300124000011300"">
//                                <Lat>32.849335555556</Lat>
//                                <Long>-117.27079833333</Long>
//                                <altitude units=""meters msl"">21</altitude>
//                                <speed units=""meters/sec"">0</speed>
//                                <heading units=""North-True"">158</heading>
//                                </acPos>";
            string dataOuter = @"<?xml version=""1.0"" encoding=""UTF-8""?>{0}<data rptTime=""{1}"" sysID=""{2}"" version=""{3}"" xmlns=""{4}"">{5}{6}</data>";
            string dataInnerOuter = @"<posList listType=""{0}"">{1}{2}</posList>";
            string dataInner = @"<acPos dateTime=""{0}"" dataCtr=""{1}"" PDOP=""{2}"" fix=""{3}"" source=""{4}"" UnitID=""{5}"" esn=""{6}"">
                                <Lat>{7}</Lat>
                                <Long>{8}</Long>
                                <altitude units=""{9}"">{10}</altitude>
                                <speed units=""{11}"">{12}</speed>
                                <heading units=""{13}"">{14}</heading>
                                </acPos>";

            builderOuter = new System.Text.StringBuilder();
            builderInnerOuter = new System.Text.StringBuilder();
            builderInner = new System.Text.StringBuilder();

            strXML = new System.IO.StringReader(inputXml);
            iDataSet = new DataSet();
            iDataSet.ReadXml(strXML);
            //iDataTable = iDataSet.Tables[0];

            foreach (DataRow row in iDataSet.Tables[0].Rows)
            {
                builderInner.AppendLine(
                string.Format(dataInner, Convert.ToDateTime(row["LastCommunicatedDateTime"].ToString()).ToUniversalTime().ToString("o"), "BSM", GetPDOP(row["CustomProp"].ToString()), GetFixType(row["CustomProp"].ToString()), "GPS", row["BoxId"].ToString(), row["ESN"].ToString(),
                                         row["Latitude"].ToString(), row["Longitude"].ToString(), "meters msl", GetAltitude(row["CustomProp"].ToString()),
                                         "meters/sec", GetSpeed(row["Speed"].ToString()), "North-True"/*GetHeadingText(row["Heading"].ToString())*/, row["Heading"].ToString()));
            }

            //outputXml = builderInner.ToString();
            //outputXml = builderInnerOuter.AppendLine(string.Format(dataInnerOuter, "Sync", "\n", builderInner)).ToString();
            builderInnerOuter.AppendLine(string.Format(dataInnerOuter, reqMsgSubject, "\n", builderInner));
            outputXml = builderOuter.Append(string.Format(dataOuter, "\n", DateTime.UtcNow.ToString("o"), reqMsgTo, reqDataVersion, reqDataXmlns, "\n", builderInnerOuter)).ToString();
        }

        string GetPDOP(string input)
        {
            return Util.PairFindValue("GPS_PDOP", input);
        }

        string GetAltitude(string input)
        {
            return Util.PairFindValue("GPS_ALTI", input);
        }

        string GetFixType(string input)
        {
            int flagVal;
            int.TryParse(Util.PairFindValue("GPS_FLAG", input), out flagVal);
            return GetIntToBinaryString(flagVal).Substring(30) == "11" ? "3D": (GetIntToBinaryString(flagVal).Substring(30) == "10" ? "2D" : "Invalid");
        }

        decimal GetSpeed(string input)
        {
            int _speed;
            int.TryParse(input, out _speed);
            return (decimal)((_speed * 1000) / 3600.0);
        }

        string GetHeadingText(string input)
        {
            int intHeading;
            
            int.TryParse(input, out intHeading);

            try
            {
                if (intHeading > 400)
                    return string.Empty;


                if ((intHeading >= 337) || (intHeading < 22))
                    return "N";

                if ((intHeading >= 22) && (intHeading < 67))
                    return "NE";

                if ((intHeading >= 67) && (intHeading < 112))
                    return "E";


                if ((intHeading >= 112) && (intHeading < 157))
                    return "SE";

                if ((intHeading >= 157) && (intHeading < 202))
                    return "S";


                if ((intHeading >= 202) && (intHeading < 247))
                    return "SW";


                if ((intHeading >= 247) && (intHeading < 292))
                    return "W";


                if ((intHeading >= 292) && (intHeading < 337))
                    return "NW";
                else
                    return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
            //return "North-True";
        }

        string GetIntToBinaryString(int n)
        {
            char[] b = new char[32];
            int pos = 31;
            int i = 0;

            while (i < 32)
            {
                if ((n & (1 << i)) != 0)
                {
                    b[pos] = '1';
                }
                else
                {
                    b[pos] = '0';
                }
                pos--;
                i++;
            }
            return new string(b);
        }


        private void WriteReturnValueStatusLog(int checkCode)
        {
            string loginFailure = "Login Failure:";

            switch (checkCode)
            {
                case 0:
                    Log("{0} {1}", "Login Success:", "!!!");
                    break;
                case -99:
                    resErrMsg = "User name and password required to Log In.";
                    Log("{0} {1}", loginFailure, "Not logged on yet. User name and password required to Log In.");
                    break;
                case -88:
                    resErrMsg = "Log on failed (service is not started).";
                    Log("{0} {1}", loginFailure, "Error: Log on failed (service is not started).");
                    break;
                default:
                    resErrMsg = string.Format("Error: ", (InterfaceError)checkCode);
                    Log("{0} {1}", loginFailure, string.Format("Error: ",(InterfaceError)checkCode));
                    break;
            }
        }
        #endregion

        #region ALARMS
        /// <summary>
        /// Get Alarms List
        /// </summary>
        /// <comment>
        /// TO Reexamine !!!
        /// </comment>
        //[WebMethod(Description = "Get Alarms List based on DateTime Range")]
        //public int GetAlarmsXML(int userId, string SID, DateTime fromDate, DateTime toDate, ref string xml)
        //{
        //    using (Alarms iAlarms = new Alarms())
        //    {
        //        return iAlarms.GetAlarmsXML(userId, SID, fromDate, toDate, ref xml);
        //    }
        //}
        #endregion

        #region MESSAGE ROUTE
        /// <summary>
        /// Route Location Message to Vehicle/Device/Box using StreetAddress/Lat-Lon.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="message"></param>
        [WebMethod(Description = "Route Location Message to terminal. Input Message XML Format: <[BoxID] [VehicleID] [MessageTypeID]> <[Message] [Latitude] [Longitude] [StreetAddress]>. Output Message XML Format: [VehicleID, RoutedMessageKey (XXX-XXXXXX-XX),Message]")]
        public int SendRoute(int userId, string SID, string message, ref string xml)
        {
            if (!ValidateUserRequest(userId, SID))
                return Convert.ToInt32(InterfaceError.AuthorizationFailed);
            return MessageRouter(userId, SID, message, ref xml);
        }

        /// <summary>
        /// Send Text Message to Vehicle/Device/Box.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="message"></param>
        [WebMethod(Description = "Send Text Message to terminal. Input Message XML Format: <[BoxID] [VehicleID] [MessageTypeID]> <[Message] [Latitude] [Longitude] [StreetAddress]>. Output Message XML Format: [VehicleID, RoutedMessageKey (XXX-XXXXXX-XX),Message]")]
        public int SendMessage(int userId, string SID, string message, ref string xml)
        {
            if (!ValidateUserRequest(userId, SID))
                return Convert.ToInt32(InterfaceError.AuthorizationFailed);
            return MessageRouter(userId, SID, message, ref xml);
        }

        /// <summary>
        /// Get Route And Message Status
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="keyxml"></param>
        /// <param name="statusxml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get Route And Message Status. Input Key Format: [UserId, SecId, Key (XXX-XXXXXX-XX)]. Output Status Format: [Key],[Status-DateTimeStamp]")]
        public int GetRouteAndMessageStatus(int userId, string SID, string keyxml, ref string statusxml)
        {
            if (!ValidateUserRequest(userId, SID))
                return Convert.ToInt32(InterfaceError.AuthorizationFailed);
            return ReadStatusPayload(userId, keyxml, ref statusxml);
        }

        int MessageRouter(int userId, string SID, string message, ref string xml)
        {
            int BoxID = 0;
            long VehicleID = 0L;
            string VehicleDescriptionAsId = "";
            int MessageTypeID = 0;
            string MessageText = string.Empty;
            string _MessageText = string.Empty;
            double LAT = 0.0;
            double LON = 0.0;
            string StreetAddress = "";
            string ResolvedStreetAddress = "";
            //string PanneId = "";
            int ServiceId = 1;//"Route Dispatch"= 1 (default)
            int ret = -1;
            int MDTCounter = 0;

            string repliedXml = "";
            xml = "";
            System.IO.StringReader strXML;
            DataSet iDataSet;
            DataTable iDataTable;

            Log("<< MessageRoutePayload: {0}", message);

            using (System.IO.TextReader iTextReader = new System.IO.StringReader(message))
            {
                using (System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(iTextReader))//  Load the XML file
                {
                    while (xmlReader.Read())//  Loop over the XML file
                    {
                        try
                        {
                            //  Here we check the type of the node, in this case we are looking for element
                            if (xmlReader.NodeType == System.Xml.XmlNodeType.Element)
                            {
                                if (xmlReader.Name == "Vehicle")//  If the element is "Vehicle"
                                {
                                    ret = 0;
                                    VehicleDescriptionAsId = xmlReader.GetAttribute("VehicleID");
                                    if (!string.IsNullOrEmpty(VehicleDescriptionAsId))
                                    {
                                        //string sql = string.Format("SELECT vlfBox.BoxId FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN vlfBox (NOLOCK) ON vlfVehicleAssignment.BoxId = vlfBox.BoxId WHERE (vlfVehicleInfo.Description = '{0}' AND vlfBox.OrganizationId=999956)", VehicleDescriptionAsId);
                                        string sql = string.Format("DECLARE @UID int DECLARE @ORGID int SET @UID={0} IF @UID=11296 BEGIN SET @ORGID=999956 END ELSE BEGIN SELECT @ORGID=OrganizationId FROM vlfUser WHERE userid=@UID END SELECT vlfBox.BoxId FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN vlfBox (NOLOCK) ON vlfVehicleAssignment.BoxId = vlfBox.BoxId WHERE (vlfVehicleInfo.Description = '{1}' AND vlfBox.OrganizationId=@ORGID)", userId, VehicleDescriptionAsId);
                                        
                                        using (System.Data.SqlClient.SqlConnection scon = new System.Data.SqlClient.SqlConnection(LoginManager.GetConnnectionString(userId)))
                                        {
                                            if (scon.State != ConnectionState.Open) scon.Open();
                                            using (System.Data.SqlClient.SqlCommand scom = new System.Data.SqlClient.SqlCommand(sql, scon))
                                            {
                                                int.TryParse(Convert.ToString(scom.ExecuteScalar()), out BoxID);
                                            }
                                            if (scon.State != ConnectionState.Closed) scon.Close();
                                        }
                                    }
                                    else
                                    {
                                        //BoxID = int.Parse(xmlReader.GetAttribute("BoxID").ToString());
                                        int.TryParse(xmlReader.GetAttribute("BoxID"), out BoxID);
                                    }

                                    //GET VehicleID for vlfJobManagement
                                    using (VLF.DAS.Logic.Vehicle v = new VLF.DAS.Logic.Vehicle(LoginManager.GetConnnectionString(userId)))
                                    {
                                        VehicleID = v.GetVehicleIdByBoxId(BoxID);
                                    }

                                    //MessageTypeID = int.Parse(xmlReader.GetAttribute("MessageTypeID").ToString());
                                    int.TryParse(xmlReader.GetAttribute("MessageTypeID"), out MessageTypeID);
                                }
                                else if (xmlReader.Name == "Stop")
                                {
                                    //PanneId = xmlReader.GetAttribute("PanneId").Trim();
                                    MessageText = string.Empty;
                                    _MessageText = string.Empty;

                                    MessageText = xmlReader.GetAttribute("Message").Trim();
                                    _MessageText = MessageText;
                                    if (MessageTypeID.Equals(42)) // Text (Garmin)
                                    {
                                        MessageText = string.Format("TXT={0}{1}", MessageText, ";YESNO=False;RETRYINTERVAL=5;LIFETIME=60;");
                                    }

                                    else if (MessageTypeID.Equals(3)) // Text (MDT) [Jun 05, 2013]
                                    {
                                        MessageText = string.Format("M= {0}>>{1};A=;", MessageText, "MDT DEMO"); //VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMessage
                                        
                                        //MessageText = string.Format("M= {0} {1};A=;", MessageText, "MDT DEMO");
                                        
                                        //MessageText = "M= Test msg from wsi??\\n;A=;";
                                        //MessageText = " " + MessageText + "\r\n";
                                        //MessageText = VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMessage, MessageText);
                                        //MessageText += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyAnswer, "");
                                    }

                                    //if (MessageText.Length > 0 && MessageText.Substring(0, 3) != "TXT")
                                    else
                                    {
                                        StreetAddress = xmlReader.GetAttribute("StreetAddress").Trim();

                                        if (!string.IsNullOrEmpty(StreetAddress))
                                        {
                                            using (ResolverSoapClient iResolver = new GeoCoder.ResolverSoapClient())
                                            {
                                                iResolver.Location(StreetAddress, ref LAT, ref LON, ref ResolvedStreetAddress);
                                            }
                                        }
                                        else
                                        {
                                            double.TryParse(xmlReader.GetAttribute("Lat").ToString(), out LAT);
                                            double.TryParse(xmlReader.GetAttribute("Lon").ToString(), out LON);
                                        }
                                    }

                                    if (BoxID > 0)
                                    {
                                        if (!ValidateUserBox(userId, SID, BoxID))
                                            return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                                        if (MessageTypeID.Equals(3))
                                        {
                                            MDTCounter++;
                                        }
                                        if (MDTCounter > 1) System.Threading.Thread.Sleep(20020);//Minimum waiting time for relese remoting SLS obj is 20 sec.
                                        string key = RouteMessage(userId, SID, BoxID, MessageTypeID, MessageText, LAT, LON);

                                        if (string.IsNullOrEmpty(VehicleDescriptionAsId))
                                        {
                                            string sql = string.Format("SELECT vlfVehicleInfo.Description FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId WHERE  (vlfVehicleAssignment.BoxId = {0})", BoxID);

                                            using (System.Data.SqlClient.SqlConnection scon = new System.Data.SqlClient.SqlConnection(LoginManager.GetConnnectionString(userId)))
                                            {
                                                if (scon.State != ConnectionState.Open) scon.Open();
                                                using (System.Data.SqlClient.SqlCommand scom = new System.Data.SqlClient.SqlCommand(sql, scon))
                                                {
                                                    VehicleDescriptionAsId = Convert.ToString(scom.ExecuteScalar());
                                                }
                                                if (scon.State != ConnectionState.Closed) scon.Close();
                                            }
                                        }
                                        
                                        //xml += MakeServiceCallXML(VehicleDescriptionAsId, key, _MessageText); //(Salman Jun 05, 2013)
                                        if (!MessageTypeID.Equals(3))
                                        {
                                            xml += MakeServiceCallXML(VehicleDescriptionAsId, key, _MessageText);
                                        }
                                        else
                                        {
                                            xml += MakeServiceCallXML(VehicleDescriptionAsId, key, _MessageText, "MDT");
                                        }

                                        using (WorkOrders iWorkOrders = new WorkOrders(LoginManager.GetConnnectionString(userId)))
                                        {
                                            iWorkOrders.AddJob(0, userId, VehicleID, LAT, LON, StreetAddress, 0, string.Empty, DateTime.UtcNow, ResolvedStreetAddress, ServiceId);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }

            xml = string.Format("<ServiceCall> {0} </ServiceCall>", xml);
            Log(">> DispatchedPayload: {0}", xml);
            //if (xml.Length > 147) ret = 0;
            return ret;
        }

        private int RouteMessageX(int userId, string SID, int boxID, int messageTypeID, string name, double lat, double lon)
        {
            int ret = 0;
            using (Garmin iGarmin = new Garmin())
            {
                if (messageTypeID.Equals(42)) // Garmin
                {
                    ret = iGarmin.SendTextMessage(userId, SID, boxID, messageTypeID, name);
                }
                else if (messageTypeID.Equals(257)) // Location
                {
                    ret = iGarmin.SendLocationMessage(userId, SID, boxID, messageTypeID, name, lat, lon, string.Empty, string.Empty);
                }
            }
            return ret;
        }

        string RouteMessage(int userId, string SID, int boxID, int messageTypeID, string message, double lat, double lon)
        {
            string ret = "";
            if (messageTypeID.Equals(42)) // Garmin Text
            {
                using (System.Data.SqlClient.SqlConnection scon = new System.Data.SqlClient.SqlConnection(LoginManager.GetConnnectionString(userId)))
                {
                    if (scon.State != ConnectionState.Open) scon.Open();
                    using (System.Data.SqlClient.SqlCommand scom = new System.Data.SqlClient.SqlCommand(string.Format("SendPacket2GarminWSI '{0}','{1}','{2}','{3}'", boxID, messageTypeID, message, userId), scon))
                    {
                        ret = Convert.ToString(scom.ExecuteScalar());
                    }
                    if (scon.State != ConnectionState.Closed) scon.Close();
                }
            }
            else if (messageTypeID.Equals(257)) // Garmin Location/Stop
            {
                string props = string.Format("{0}={1};{2}={3};{4}={5};{6}={7};{8}={9};{10}={11};{12}={13}",
                                               GarminFMI.JKeyWords.TXT.ToString(), Convert.ToString(message),
                                               GarminFMI.JKeyWords.LAT.ToString(), lat,
                                               GarminFMI.JKeyWords.LON.ToString(), lon,
                                               GarminFMI.JKeyWords.ADD.ToString(), string.Empty,
                                               GarminFMI.JKeyWords.LNAME.ToString(), string.Empty,
                                               "RETRYINTERVAL", "5",
                                               "LIFETIME", "300");

                using (System.Data.SqlClient.SqlConnection scon = new System.Data.SqlClient.SqlConnection(LoginManager.GetConnnectionString(userId)))
                {
                    if (scon.State != ConnectionState.Open) scon.Open();
                    using (System.Data.SqlClient.SqlCommand scom = new System.Data.SqlClient.SqlCommand(string.Format("SendPacket2GarminWSI '{0}','{1}','{2}','{3}'", boxID, messageTypeID, props, userId), scon))
                    {
                        ret = Convert.ToString(scom.ExecuteScalar());
                    }
                    if (scon.State != ConnectionState.Closed) scon.Close();
                }
            }
            else if (messageTypeID.Equals(3)) // MDT TEXT
            {
                using (Location iLocation = new Location())
                {
                    short pt = -1, cm = -1;
                    bool cs = false;
                    long sto = 0;
                    //int count = 0;

                    if (message.Length <= 230)
                    {
                        //int res = 0;
                        /*res = */iLocation.SendCommand(userId, SID, DateTime.Now, boxID, Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), message, ref pt, ref cm, ref cs, ref sto);
                        //count++;
                        //RETRY:
                        //if (res == 19)
                        //{
                        //    System.Threading.Thread.Sleep(20);
                        //    res = iLocation.SendCommand(userId, SID, DateTime.Now, boxID, Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), message, ref pt, ref cm, ref cs, ref sto);
                        //    count++;
                        //}
                        //if (res == 19 && count < 7) goto RETRY;
                    }
                    ret = cs.ToString();
                    //iLocation.Dispose();
                }
                //System.Threading.Thread.Sleep(5000);
            }
            return ret;
        }

        private string MakeServiceCallXML(int userId, int boxID, int messageTypeID, double lat, double lon, string streetAddress)
        {
            string retXML = string.Empty;
            string ServiceInformation = "ServiceInformation";
            string UserId = "UserId";
            string BoxId = "BoxId";
            string MessageTypeID = "MessageTypeID";
            string Latitude = "Latitude";
            string Longitude = "Longitude";
            string StreetAddress = "StreetAddress";

            retXML = "<" + ServiceInformation + ">" +
                "<" + UserId + ">" + userId + "</" + UserId + ">" +
                "<" + BoxId + ">" + boxID + "</" + BoxId + ">" +
                "<" + MessageTypeID + ">" + messageTypeID + "</" + MessageTypeID + ">" +
                "<" + Latitude + ">" + lat + "</" + Latitude + ">" +
                "<" + Longitude + ">" + lon + "</" + Longitude + ">" +
                "<" + StreetAddress + ">" + streetAddress + "</" + StreetAddress + ">" +
                "</" + ServiceInformation + ">";

            return retXML;
        }

        private string MakeServiceCallXML(string vehicleId, string routedMessageKey, string message)//string panneId, 
        {
            string retXML = string.Empty;
            const string ServiceInformation = "ServiceInformation";
            string VehicleId = "VehicleID";
            const string RoutedMessageKey = "RoutedMessageKey";
            const string Message = "Message";

            //retXML = string.Format("<{0}><{1}>{2}</{3}><{4}>{5}</{6}></{7}>", ServiceInformation, RoutedMessageKey, routedMessageKey, RoutedMessageKey, Message, message, Message, ServiceInformation);
            retXML = string.Format("<{0}><{1}>{2}</{3}><{4}>{5}</{6}><{7}>{8}</{9}></{10}>", ServiceInformation, VehicleId, vehicleId, VehicleId, RoutedMessageKey, routedMessageKey, RoutedMessageKey, Message, message, Message, ServiceInformation);

            return retXML;
        }

        private string MakeServiceCallXML(string vehicleId, string routedMessageStatus, string message, string type)//type=MDT
        {
            string retXML = string.Empty;
            const string ServiceInformation = "ServiceInformation";
            string VehicleId = "VehicleID";
            const string RoutedMessageStatus = "RoutedMessageStatus";
            const string Message = "Message";

            retXML = string.Format("<{0}><{1}>{2}</{3}><{4}>SENT:{5}</{6}><{7}>{8}</{9}></{10}>", ServiceInformation, VehicleId, vehicleId, VehicleId, RoutedMessageStatus, routedMessageStatus, RoutedMessageStatus, Message, message, Message, ServiceInformation);

            return retXML;
        }

        int ReadStatusPayload(int userId, string keyxml, ref string statusxml)
        {
            string inner = "";
            string status = "";
            statusxml = "";
            const string ServiceStatusCall = "ServiceStatusCall";
            const string ServiceStatusInformation = "ServiceStatusInformation";
            int ret = 1;

            //Stopwatch watch = new Stopwatch();
            //watch.Reset();
            //watch.Start();
            Log("<< GetRouteAndMessageStatus [ReadStatusPayload]: {0}", keyxml);

            using (System.IO.TextReader iTextReader = new System.IO.StringReader(keyxml))
            {
                using (System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(iTextReader))//  Load the XML file
                {
                    while (xmlReader.Read())//  Loop over the XML file
                    {
                        //  Here we check the type of the node, in this case we are looking for element
                        if (xmlReader.NodeType == System.Xml.XmlNodeType.Element)
                        {
                            if (xmlReader.Name == "RoutedMessage")//  If the element is "RoutedMessage"
                            {
                                try
                                {
                                    string elementContent = xmlReader.GetAttribute("Key");//xmlReader.ReadElementString();
                                    GetMessageAndRouteStatus(userId, elementContent, ref status);
                                    inner += MakeServiceStatusCallXML(elementContent, status);
                                }
                                catch { throw new Exception("Invalid keyxml."); }
                            }
                        }
                    }
                }
            }

            ret = (inner.Length > 54 ? 0 : 1);
            statusxml = string.Format("<{0}><{1}>{2}</{3}></{4}>", ServiceStatusCall, ServiceStatusInformation, inner, ServiceStatusInformation, ServiceStatusCall);
            //watch.Stop();
            //Log(">> GetRouteAndMessageStatus: {0} [duration={1}]", statusxml, watch.Elapsed.TotalSeconds);
            Log(">> GetRouteAndMessageStatus: {0}", statusxml);
            return ret;
        }

        private string MakeServiceStatusCallXML(string routedMessageKey, string status)
        {
            string retXML = string.Empty;

            const string RoutedMessageKey = "RoutedMessageKey";
            const string Status = "Status";

            retXML = string.Format("<{0}>{1}</{2}><{3}>{4}</{5}>", RoutedMessageKey, routedMessageKey, RoutedMessageKey, Status, status, Status);

            return retXML;
        }

        private int GetMessageAndRouteStatus(int userId, string key, ref string status)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return 1;

                //Key: PeripheralID-MessageId-PeripheralMessageTypeId
                key = string.Format("'{0}'", key.Replace("-", "','"));

                using (System.Data.SqlClient.SqlConnection scon = new System.Data.SqlClient.SqlConnection(LoginManager.GetConnnectionString(userId)))
                {
                    if (scon.State != ConnectionState.Open) scon.Open();
                    using (System.Data.SqlClient.SqlCommand scom = new System.Data.SqlClient.SqlCommand(string.Format("GetGarminMessageStatus '{0}',{1}", userId, key), scon))
                    {
                        status = Convert.ToString(scom.ExecuteScalar());
                    }
                    if (scon.State != ConnectionState.Closed) scon.Close();
                }
            }
            catch { return 1; }
            return 0;
        }
        #endregion

        #region GETLASTMSGFROMHIST-DBHIST
        [WebMethod(Description = "Returns XML of last messages that retrieves from the history by received datetime (maximum return messages should be specified). XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],[SequenceNum],[BoxArmed]")]
        public int GetDeviceHistory(int userId, string SID, short numOfRecords, int boxId, DateTime from, DateTime to, ref string xml)
        {
            short msgType = -1;
            using (DBHistory iDBHistory = new DBHistory())
            {
                return iDBHistory.GetLastMessagesFromHistory(userId, SID, numOfRecords, boxId, msgType, from, to, ref xml);
            }
        }

        //[WebMethod(Description = "Retrieves last messages from the history by originated datetime (Maximum return messages should be specified). XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],[SequenceNum],[BoxArmed]")]
        //public int GetLastMessagesFromHistoryByOriginDateTime(int userId, string SID, short numOfRecords, int boxId, short msgType, DateTime from, DateTime to, ref string xml)
        //{
        //    using (DBHistory iDBHistory = new DBHistory())
        //    {
        //        return iDBHistory.GetLastMessagesFromHistoryByOriginDateTime(userId, SID, numOfRecords, boxId, msgType, from, to, ref xml);
        //    }
        //}
        #endregion

        #region GET AUDIT DATA
        [WebMethod(Description = "Returns XML of Audit Logs. XML Format: [ModuleName],[OrganizationID],[TableName],[ColumnName],[RecordId],[Action],[UpdateDate],[UpdatedBy],[OldValue],[NewValue],[Description],[RemoteAddress],[ApplicationName]")]
        //last messages that retrieves from the history by received datetime (Maximum return messages should be specified). XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],[SequenceNum],[BoxArmed]")]
        public int GetAuditLogs(int userId, string SID, DateTime dtFrom, DateTime dtTo, ref string xml)
        {
            if (userId < 0) return (int)InterfaceError.AuthenticationFailed;
            // Authentication & Authorization
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            // DB Settings
            string _ConnectionString = string.Empty; //"Data Source=192.168.7.17;Initial Catalog=SentinelFM;Persist Security Info=True;User ID=sa;Password=Bsmwireless1;";
            _ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            DataSet dsAuditLogs = new DataSet("AuditLogList");
            string sql = "usp_Get_AuditLogs ";
            sql = string.Format("{0} {1},'{2}','{3}'", sql, userId, dtFrom, dtTo);
            string prefixMsg = string.Empty;

            try
            {
                prefixMsg = string.Format("Unable to retrieve AuditLogs information.");

                using (System.Data.SqlClient.SqlConnection alSqlConnection = new System.Data.SqlClient.SqlConnection(_ConnectionString))
                {
                    if (alSqlConnection.State != ConnectionState.Open) alSqlConnection.Open();

                    using (System.Data.SqlClient.SqlCommand alSqlCommand = new System.Data.SqlClient.SqlCommand(sql, alSqlConnection))
                    {
                        using (System.Data.SqlClient.SqlDataAdapter alSqlDataAdapter = new System.Data.SqlClient.SqlDataAdapter(alSqlCommand))
                        {
                            alSqlDataAdapter.TableMappings.Add("Table", "AuditLogs");

                            int rows = alSqlDataAdapter.Fill(dsAuditLogs);
                            //XTracer.Info("Total Rows:[{0}]", rows.ToString());
                            Log(">> GetAuditLogs( uId={0})", userId);
                        }
                    }

                    if (alSqlConnection.State != ConnectionState.Closed) alSqlConnection.Close();
                }
                if (ASIErrorCheck.IsAnyRecord(dsAuditLogs))
                    xml = dsAuditLogs.GetXml();
                return (int)InterfaceError.NoError;
                //return xml;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAuditLogs : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
                //return xml;
            }
        }
        #endregion GET AUDIT DATA

        #region PINGXML
        /// <summary>
        /// Returns 0 if system is alive.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Used to be sure system is alive. Input : Secret/Private Key. Output/response XML Format :[ResponseTime] (format: MM/dd/yyyy HH:mm:ss.fff)")]
        public int PingXML(string Key, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                if (Key == "9AE62EB9-8Q204064509")//9AE62EB9-HQBSMSFMWSI
                {
                    //xml = string.Format(@"<?xml version=""1.0""?> <ResponseTime> {0} </ResponseTime>", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"));
                    xml = string.Format("<ResponseTime> {0} </ResponseTime>", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"));
                    Log("<< PingXML( key={0}, tSpan={1} )", Key, DateTime.Now.Subtract(dtNow).TotalMilliseconds);
                    return (int)InterfaceError.NoError;
                }
                else
                {
                    xml = "";
                    LogException("<< PingXML : key={0}, time={1}", Key, DateTime.Now);
                    return (int)InterfaceError.AuthenticationFailed;
                }
                // Authenticate & Authorize
                //LoginManager.GetInstance().SecurityCheck(userId, SID);
                //xml = string.Format("<ResponseTime> {0} </ResponseTime>", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"));

                //Log("<< PingXML( key={0}, tSpan={1} )", Key, DateTime.Now.Subtract(dtNow).TotalMilliseconds);
                //return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< PingXML : uId={0}, EXC={1}", Key, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion PINGXML

        #region LOG
        private void Log(string strFormat, params object[] objects)
        {
            try
            {
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces, string.Format(strFormat, objects)));
            }
            catch { }
        }

        private void LogException(string strFormat, params object[] objects)
        {
            try
            {
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceError, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Error, string.Format(strFormat, objects)));
            }
            catch { }
        }
        #endregion LOG

        #region VALIDATION
        private bool ValidateUserRequest(int userId, string SID)
        {
            bool ret = false;
            try
            {
                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //Authorization
                ValidationResult res = SecurityKeyManager.GetInstance().ValidatePasskey(userId, SID);

                switch (res)
                {
                    case ValidationResult.Failed:
                    case ValidationResult.Expired:
                    case ValidationResult.CallFrequencyExceeded:
                        ret = false;
                        break;
                    default:
                        ret = true;
                        break;
                }
            }
            catch
            {
                throw new Exception("SecurityCheck: Validation failed.");
            }

            return ret;
        }

        private bool ValidateUserBox(int userId, string SID, int boxId)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateUserBox(userId, boxId);
            }
        }

        #endregion VALIDATION



        //////public AuthenticationHeader authenticationHeader;
        //////// Receive all SOAP headers besides the MyHeader SOAP header.
        ////////public System.Web.Services.Protocols.SoapUnknownHeader[] unknownHeaders;

        ////////public System.Web.Services.Protocols.SoapHeader[] sh;

        //////[WebMethod]
        //////[System.Web.Services.Protocols.SoapHeader("authenticationHeader", Direction = System.Web.Services.Protocols.SoapHeaderDirection.InOut | System.Web.Services.Protocols.SoapHeaderDirection.Fault)]

        ////////[System.Web.Script.Services.ScriptMethod(UseHttpGet = true, ResponseFormat = System.Web.Script.Services.ResponseFormat.Xml)]

        ////////Receive any SOAP headers other than MyHeader.
        ////////[System.Web.Services.Protocols.SoapHeader("unknownHeaders", Required = false)]
        //////public /*void*/ string MyWebMethodWithHeader()
        //////{
        //////    string un = authenticationHeader.UserName;
        //////    string up = authenticationHeader.UserPassword;
        //////    string wcMsg = "";

        //////    if (authenticationHeader.UserName == "Shayaan")
        //////    {
        //////        wcMsg = "Welcome!";
        //////    }

        //////    ////myHeader.DidUnderstand = true;

        //////    //foreach (System.Web.Services.Protocols.SoapUnknownHeader header in unknownHeaders)
        //////    //{
        //////    //    // Perform some processing on the header.
        //////    //    if (header.Element.Name == "MyKnownHeader")
        //////    //        header.DidUnderstand = true;
        //////    //    else
        //////    //        // For those headers that cannot be  
        //////    //        // processed, set the DidUnderstand property to false.
        //////    //        header.DidUnderstand = false;#96 (416 925 5141 x 2289 Lynn)
        //////    //}

        //////    //throw new HttpException((int) System.Net.HttpStatusCode.BadRequest, "Error Message");

        //////    ////HttpContext.Current.Response.Clear();
        //////    ////HttpContext.Current.Response.StatusCode = (int)System.Net.HttpStatusCode.Unused;
        //////    ////HttpContext.Current.Response.SuppressContent = true;
        //////    ////HttpContext.Current.Response.Flush();
        //////    ////HttpContext.Current.Response.Close();

        //////    //HttpContext.Current.Response.Clear();
        //////    //HttpContext.Current.Response.ContentType = "text/xml";
        //////    //HttpContext.Current.Response.AddHeader("Content-Length", xml.Length.ToString());
        //////    //HttpContext.Current.Response.Write(xml);
        //////    //HttpContext.Current.Response.Flush();
        //////    //HttpContext.Current.Response.Close();

        //////    return "Hello " + un + " " + wcMsg;
        //////}
    }
}