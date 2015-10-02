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

namespace VLF.ASI.Interfaces
{
    /// <summary>
    /// Summary description for BSMCustomService
    /// </summary>
    [WebService(Namespace = "http://www.sentinelfm.com", Description = "Sentinel Web Service Methods Exposer.", Name = "SentinelWSI")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class SentinelWSI : System.Web.Services.WebService
    {
        public SentinelWSI()
        {
            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

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

        [WebMethod(Description = "Used to log on to the system. XML File format :[userName],[password],[userIp],[userId],[SID]")]//,EnableSession=true)]
        
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
        /// <returns>XML [OrganizationName],[FleetId],[FleetName],[FleetDescription]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = @"Returns XML of Fleets by User ID. XML [OrganizationName],[FleetId],[FleetName],[FleetDescription]")]
        public int GetFleetsInfoXMLByUserId(int userId, string SID, ref string xml)
        {
            using (DBFleet iDBFleet = new DBFleet())
            {
                return iDBFleet.GetFleetsInfoXMLByUserId(userId, SID, ref xml);
            }
        }

        /// <summary>
        /// Returns vehicles last known position information by Fleet Id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>
        /// XML
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],[FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Fleet ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetVehiclesLastPositionInfo(int userId, string SID, int fleetId, ref string xml)
        {
            using (DBFleet iDBFleet = new DBFleet())
            {
                return iDBFleet.GetVehiclesLastKnownPositionInfo(userId, SID, fleetId, ref xml);
            }
            
        }

        /// <summary>
        /// Returns vehicle information by Fleet Id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>XML [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles for a particular Fleet by Fleet ID. XML file format:   [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description] ")]
        public int GetVehiclesInfoXMLByFleetId(int userId, string SID, int fleetId, ref string xml)
        {
            using (DBFleet iDBFleet = new DBFleet())
            {
                return iDBFleet.GetVehiclesInfoXMLByFleetId(userId, SID, fleetId, ref xml);
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
        [WebMethod(Description = "Route and Send Text Message to Device/Box. Input Message XML Format: <[BoxID] [MessageTypeID]> <[Name] [Latitude] [Longitude]>.")]
        public void SendRoute(int userId, string SID, string message)
        {
            int BoxID = 0;
            int MessageTypeID = 0;
            string Name = string.Empty;
            double LAT = 0.0;
            double LON = 0.0;

            using (System.IO.TextReader iTextReader = new System.IO.StringReader(message))
            {
                using (System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(iTextReader))//  Load the XML file
                {
                    while (xmlReader.Read())//  Loop over the XML file
                    {
                        //  Here we check the type of the node, in this case we are looking for element
                        if (xmlReader.NodeType == System.Xml.XmlNodeType.Element)
                        {
                            if (xmlReader.Name == "Vehicle")//  If the element is "Vehicle"
                            {
                                //BoxID = int.Parse(xmlReader.GetAttribute("BoxID").ToString());
                                int.TryParse(xmlReader.GetAttribute("BoxID").ToString(), out BoxID);
                                //MessageTypeID = int.Parse(xmlReader.GetAttribute("MessageTypeID").ToString());
                                int.TryParse(xmlReader.GetAttribute("MessageTypeID").ToString(), out MessageTypeID);
                            }
                            else if (xmlReader.Name == "Stop")
                            {
                                Name = xmlReader.GetAttribute("Name").ToString();
                                //LAT = double.Parse(xmlReader.GetAttribute("Lat").ToString());
                                //LON = double.Parse(xmlReader.GetAttribute("Lon").ToString());
                                double.TryParse(xmlReader.GetAttribute("Lat").ToString(), out LAT);
                                double.TryParse(xmlReader.GetAttribute("Lon").ToString(), out LON);
                                //MessageBox.Show("Route>> BX:" + BoxID + " MT:" + MessageTypeID + " MS:" + Name + " LT:" + LAT + " LN:" + LON);
                                RouteMessage(userId, SID, BoxID, MessageTypeID, Name, LAT, LON);
                            }
                        }
                    }
                }
            }
            //MessageBox.Show(F);
        }

        private void RouteMessage(int userId, string SID, int boxID, int messageTypeID, string name, double lat, double lon)
        {
            using (Garmin iGarmin = new Garmin())
            {
                if (messageTypeID.Equals(42)) // Garmin
                {
                    iGarmin.SendTextMessage(userId, SID, boxID, messageTypeID, name);
                }
                else if (messageTypeID.Equals(257)) // Location
                {
                    iGarmin.SendLocationMessage(userId, SID, boxID, messageTypeID, name, lat, lon, string.Empty, string.Empty);
                }
            }
        }
        #endregion
    }
}