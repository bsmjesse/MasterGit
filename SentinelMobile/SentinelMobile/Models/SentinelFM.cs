using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace SentinelMobile.Models
{
    public static class SentinelFM
    {
        public static void setupUser(string userName, int userId, string secId,int superOrganizationId)
        {
            User user = new User(userName, userId, secId);
            user.SuperOrganizationId = superOrganizationId;
            //user.RealOrganizationId = user.OrganizationId;
            user.ExistingPreference();
            HttpContext.Current.Session.Add("SentinelUser", user);
        }
        public static bool ValidateUser(string userName, string HashPassword, string IpAddr, string authseed)
        {
            try
            {
                #region start check IP for hgi user
                string[] ip;
                if (userName.Contains("hgi") == true && IpAddr.Contains("127.0.0.1") == false && IpAddr.Contains("::1") == false)
                {

                    if (IpAddr.Contains("184.94.19") == true || IpAddr.Contains("142.46.86") == true)
                    {
                        //ip = IpAddr.Split('.');
                        //if (Convert.ToInt16(ip[3]) < 97 || Convert.ToInt16(ip[3]) > 110)
                        // {
                        //        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AuthFailed");
                        //        this.lblMessage.Visible = true;
                        //        return;
                        //}
                    }
                    else if (IpAddr.Contains("192.168.199") == true)
                    {
                        ip = IpAddr.Split('.');
                        if (Convert.ToInt16(ip[3]) < 1 || Convert.ToInt16(ip[3]) > 254)
                        {
                            return false;
                        }
                    }
                    else if (IpAddr.Contains("67.70.185") == true)
                    {
                        ip = IpAddr.Split('.');
                        if (Convert.ToInt16(ip[3]) < 121 || Convert.ToInt16(ip[3]) > 126)
                        {
                            return false;
                        }
                    }
                    else if (IpAddr.Contains("172.16.3") == true)
                    {
                        ip = IpAddr.Split('.');
                        if (Convert.ToInt16(ip[3]) < 1 || Convert.ToInt16(ip[3]) > 254)
                        {
                            return false;
                        }
                    }

                    else if (IpAddr.Contains("184.94.19") == false || IpAddr.Contains("192.168.199") == false || IpAddr.Contains("67.70.185") == false || IpAddr.Contains("172.16.3") == false)
                    {
                        return false;

                    }
                }
                else if ((userName.ToLower().Trim() == "ncc1" || userName.ToLower().Trim() == "ncc2") && IpAddr.Contains("209.171.44.131") == false && IpAddr.Contains("206.162.182.113") == false)
                {
                    return false;
                }


                #endregion end check IP for hgi user

                SentinelMobile.SecurityManager.SecurityManager securityManager = new SecurityManager.SecurityManager();
                int userId = 0;
                string secId = null;
                
                //securityManager.Login(userName, password, "", ref userId, ref secId);
                
                
                int superOrganizationId = 1;

                int errCode = securityManager.LoginMD5Extended(authseed, userName, HashPassword, IpAddr, ref userId, ref secId, ref superOrganizationId);
                //int errCode = securityManager.Login(userName, HashPassword, "", ref userId, ref secId);
                if (userId.Equals(0) || string.IsNullOrEmpty(secId))
                {
                    return false;
                }

                setupUser( userName,  userId,  secId, superOrganizationId);

                #region Garda IP restrictions
                User user = HttpContext.Current.Session != null ? (User)HttpContext.Current.Session["SentinelUser"] : null;
                if (user == null)
                {
                    return false;
                }
                if (user.OrganizationId == 952 && (user.UserGroupId != 1 && IpAddr.Contains("184.94.19.2") == false))
                {
                    if (IpAddr.Contains("209.171.44") == false && IpAddr.Contains("209.202.108") == false && IpAddr.Contains("206.162.182") == false && IpAddr.Contains("68.67.62") == false && IpAddr.Contains("64.187.177") == false && IpAddr.Contains("64.187.178") == false)
                    {
                        return false;
                    }
                    else
                    {

                        if (IpAddr.Contains("209.171.44") == true)
                        {
                            ip = IpAddr.Split('.');
                            if (Convert.ToInt16(ip[3]) < 129 || Convert.ToInt16(ip[3]) > 142)
                            {
                                return false;
                            }
                        }

                        if (IpAddr.Contains("209.202.108") == true)
                        {
                            ip = IpAddr.Split('.');
                            if (Convert.ToInt16(ip[3]) < 105 || Convert.ToInt16(ip[3]) > 110)
                            {
                                return false;
                            }
                        }

                        if (IpAddr.Contains("206.162.182") == true)
                        {
                            ip = IpAddr.Split('.');
                            if (Convert.ToInt16(ip[3]) < 113 || Convert.ToInt16(ip[3]) > 118)
                            {
                                return false;
                            }
                        }

                        if (IpAddr.Contains("68.67.62") == true)
                        {
                            ip = IpAddr.Split('.');
                            if (Convert.ToInt16(ip[3]) < 161 || Convert.ToInt16(ip[3]) > 190)
                            {
                                return false;
                            }
                        }

                        if (IpAddr.Contains("64.187.177") == true)
                        {
                            ip = IpAddr.Split('.');
                            if (Convert.ToInt16(ip[3]) < 129 || Convert.ToInt16(ip[3]) > 190)
                            {
                                return false;
                            }
                        }

                        if (IpAddr.Contains("64.187.178") == true)
                        {
                            ip = IpAddr.Split('.');
                            if (Convert.ToInt16(ip[3]) < 81 || Convert.ToInt16(ip[3]) > 86)
                            {
                                return false;
                            }
                        }
                    }
                }
                #endregion


                return true;
            }
            catch (Exception exception)
            {

            }
            return false;
        }

        public static IList<Fleet> GetFleets()
        {
            try
            {
                User user = HttpContext.Current != null ? (User)HttpContext.Current.Session["SentinelUser"] : null;
                if (user == null)
                {
                    return null;
                }

                DBFleet.DBFleet dbFleet = new DBFleet.DBFleet();
                string xml = null;
                int i = dbFleet.GetFleetsInfoXMLByUserId(user.UserId, user.SecId, ref xml);
                return GetFleetsList(xml);
            }
            catch (Exception exception)
            {

            }
            return null;
        }

        public static IList<Vehicle> GetVehicles(int fleetId, string c)
        {
            try
            {
                User user = HttpContext.Current != null ? (User)HttpContext.Current.Session["SentinelUser"] : null;
                if (user == null)
                {
                    return null;
                }

                DBFleet.DBFleet dbFleet = new DBFleet.DBFleet();
                string xml = null;
                dbFleet.GetVehiclesLastKnownPositionInfoByLang(user.UserId, user.SecId, fleetId, c, ref xml);
                return GetVehiclesList(xml, c);
            }
            catch (Exception exception)
            {

            }
            return null;
        }

        public static int UpdateVehicleDescription(int userId, string constr, Int64 vehicleId, string vehicleDescription)
        {
            //VLF.DAS.Logic.User dbUser = new VLF.DAS.Logic.User(constr);
            //if (!dbUser.ValidateUserVehicle(userId, vehicleId))
            //    return 0;

            string sql = "";

            sql = "SELECT * FROM vlfVehicleInfo WHERE VehicleId=" + vehicleId.ToString();

            DataSet dataSet = new DataSet();
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(sql, con);
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                    adapter.Fill(dataSet);

                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        dataSet.Tables[0].Rows[0]["Description"] = vehicleDescription.Replace("'", "''");
                    }

                    adapter.UpdateCommand = builder.GetUpdateCommand();
                    int result = adapter.Update(dataSet);
                    con.Close();
                    return result;
                }
                catch(Exception e)
                {
                    return 0;
                }

            }
        }

        /*public static VehicleInfo GetVehicleInfoByLicensePlate(string LicensePlate)
        {
            try
            {
                User user = HttpContext.Current != null ? (User)HttpContext.Current.Session["SentinelUser"] : null;
                if (user == null)
                {
                    return null;
                }

                StringReader strrXML = null;
                DataSet dsVehicle = new DataSet();

                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (dbv.GetVehicleInfoXML(user.UserId, user.SecId, LicensePlate, ref xml) != 0)
                    if (dbv.GetVehicleInfoXML(user.UserId, user.SecId, LicensePlate, ref xml) != 0)
                    {
                        return null;
                    }

                if (xml == "")
                {
                    return null;
                }


                if (user.DefaultLanguage == "fr-CA")
                {
                    xml = xml.Replace("Not Listed", "Pas dans la liste");
                    xml = xml.Replace("Truck", "Camion");
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                if (dsVehicle.Tables[0].Rows.Count > 0)
                {
                    VehicleInfo vehicle = new VehicleInfo();
                    DataRow dr = dsVehicle.Tables[0].Rows[0];

                    vehicle.BoxId = dr["BoxId"] != null ? Convert.ToInt32(dr["BoxId"].ToString()) : 0;
                    vehicle.VehicleId = dr["VehicleId"] != null ? Convert.ToInt32(dr["VehicleId"].ToString()) : 0;
                    vehicle.LicensePlate = dr["LicensePlate"] != null ? dr["LicensePlate"].ToString() : null;
                    vehicle.Description = dr["Description"] != null ? dr["Description"].ToString() : null;
                    //vehicle.SensorMask = sNode["SensorMask"] != null ? Convert.ToInt64(sNode["SensorMask"].InnerText.Trim()) : 0;
                    //vehicle.MyHeading = sNode["MyHeading"] != null ? sNode["MyHeading"].InnerText.Trim() : null;

                    vehicle.Color = dr["Color"].ToString();
                    vehicle.ModelYear = dr["ModelYear"] == null ? (short)0 : Convert.ToInt16(dr["ModelYear"].ToString());
                    vehicle.CostPerMile = dr["CostPerMile"] == null ? 0 : Convert.ToDouble(dr["CostPerMile"].ToString());
                    vehicle.VinNum = dr["VinNum"].ToString();
                    vehicle.StateProvince = dr["StateProvince"].ToString();

                    vehicle.VehicleTypeId = 0;
                    string xmlVehicleTypes = "";
                    ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
                    if (dbs.GetAllVehicleTypes(user.UserId, user.SecId, ref xmlVehicleTypes)!=0)
                        if (dbs.GetAllVehicleTypes(user.UserId, user.SecId, ref xmlVehicleTypes)!=0)
                        {
                            
                        }
                    if (xmlVehicleTypes != "")
                    {
                        if (user.DefaultLanguage == "fr-CA")
                            xmlVehicleTypes = xmlVehicleTypes.Replace("Truck", "Camion");

                        StringReader strrVehicleTypeXML = null;
                        strrVehicleTypeXML = new StringReader(xmlVehicleTypes);
                        DataSet dsVehicleType;
                        dsVehicleType = new DataSet();
                        dsVehicleType.ReadXml(strrXML);
                        foreach (DataRow drvt in dsVehicleType.Tables[0].Rows)
                        {
                            if (drvt["VehicleTypeName"].ToString().Trim() == dr["VehicleTypeName"].ToString().Trim())
                            {
                                vehicle.VehicleTypeId = drvt["VehicleTypeId"] == null ? (short)0 : Convert.ToInt16(drvt["VehicleTypeId"].ToString());
                            }
                        }
                    }


                    


                    CboMake_Fill();
                    this.cboMake.SelectedIndex = -1;
                    for (int i = 0; i < cboMake.Items.Count; i++)
                    {
                        if (cboMake.Items[i].Text.TrimEnd() == dsVehicle.Tables[0].Rows[0]["MakeName"].ToString().TrimEnd())
                        {
                            cboMake.SelectedIndex = i;
                            break;
                        }
                    }


                    CboModel_Fill();
                    this.cboModel.SelectedIndex = -1;
                    cboModel.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["MakeModelId"].ToString()).Selected = true;


                    
                    this.cboBox.SelectedIndex = -1;
                    cboBox.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["BoxId"].ToString()).Selected = true;
                    this.lblOldBox.Text = dsVehicle.Tables[0].Rows[0]["BoxId"].ToString();

                    this.txtEmail.Text = dsVehicle.Tables[0].Rows[0]["Email"].ToString().TrimEnd();
                    this.txtPhone.Text = dsVehicle.Tables[0].Rows[0]["Phone"].ToString().TrimEnd();
                    cboTimeZone_Fill();
                    try
                    {
                        this.cboTimeZone.SelectedIndex = -1;
                        for (int i = 0; i < cboTimeZone.Items.Count; i++)
                        {
                            if (Convert.ToInt16(cboTimeZone.Items[i].Value.TrimEnd()) == Convert.ToInt32(dsVehicle.Tables[0].Rows[0]["TimeZone"].ToString().TrimEnd()))
                            {
                                cboTimeZone.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    catch
                    {
                    }

                    this.chkDayLight.Checked = Convert.ToBoolean(dsVehicle.Tables[0].Rows[0]["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                    this.chkNotify.Checked = Convert.ToBoolean(dsVehicle.Tables[0].Rows[0]["Notify"].ToString().TrimEnd());
                    this.chkWarning.Checked = Convert.ToBoolean(dsVehicle.Tables[0].Rows[0]["Warning"].ToString().TrimEnd());
                    this.chkCritical.Checked = Convert.ToBoolean(dsVehicle.Tables[0].Rows[0]["Critical"].ToString().TrimEnd());
                    this.chkMaintenance.Checked = Convert.ToBoolean(dsVehicle.Tables[0].Rows[0]["Maintenance"].ToString().TrimEnd());

                    this.tblVehicleInfo.Visible = true;
                    optVehicleIcons.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["IconTypeId"].ToString()).Selected = true;

                    //Devin
                    if (dsVehicle.Tables[0].Rows[0]["FormatType"] != DBNull.Value)
                    {
                        try
                        {
                            cboFuelCategory.ClearSelection();
                            cboFuelCategory.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["FormatType"].ToString()).Selected = true;
                        }
                        catch (Exception ex) { }
                    }

                    //Salman Mar 06, 2013
                    this.cboPostedSpeed.SelectedIndex = -1;
                    if (dsVehicle.Tables[0].Rows[0]["ServiceConfigID"] != DBNull.Value)
                    {
                        try
                        {
                            cboPostedSpeed.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["ServiceConfigID"].ToString()).Selected = true;
                        }
                        catch { }
                    }

                }
                else
                {
                    return null;
                }
            }
            catch { }

            return null;
        }
        */
        public static IList<Vehicle> FindMyVehiclesByPosition(double lat, double lon)
        {
            try
            {
                User user = HttpContext.Current != null ? (User)HttpContext.Current.Session["SentinelUser"] : null;
                if (user == null)
                {
                    return null;
                }

                DBFleet.DBFleet dbFleet = new DBFleet.DBFleet();
                string xml = null;
                //dbFleet.GetVehiclesLastKnownPositionInfoNearestToLatLon(user.UserId, user.SecId, user.
                return GetVehiclesList(xml);
            }
            catch (Exception exception)
            {

            }
            return null;
        }

        private static IList<Fleet> GetFleetsList(string myXmlString)
        {
            List<Fleet> fleets = new List<Fleet>();
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(myXmlString); // suppose that myXmlString contains "<Names>...</Names>"
                XmlNodeList xnList = xml.GetElementsByTagName("FleetsInformation");
                foreach (XmlNode sNode in xnList)
                {
                    try
                    {
                        Fleet fleet = new Fleet();
                        fleet.Name = sNode["FleetName"] != null ? sNode["FleetName"].InnerText.Trim() : null;
                        fleet.FleetId = sNode["FleetId"] != null ? Convert.ToInt32(sNode["FleetId"].InnerText.Trim()) : 0;
                        fleet.Description = sNode["Description"] != null ? sNode["Description"].InnerText.Trim() : null;
                        fleet.OrganizationName = sNode["OrganizationName"] != null ? sNode["OrganizationName"].InnerText.Trim() : null;
                        fleets.Add(fleet);
                    }
                    catch (Exception exception)
                    {

                    }

                }
            }
            catch (Exception exception)
            {

            }
            return fleets;
        }

        public static IList<Fleet> GetFleetsList(DataSet ds)
        {
            List<Fleet> fleets = new List<Fleet>();
            try
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        Fleet fleet = new Fleet();
                        fleet.Name = dr["NodeName"].ToString();
                        fleet.FleetId = dr["FleetId"] != null ? Convert.ToInt32(dr["FleetId"].ToString().Trim()) : 0;
                        fleet.Description = dr["fleetDescription"] != null ? dr["fleetDescription"].ToString().Trim() : null;
                        fleet.OrganizationName = "";
                        fleet.Path = dr["NodePath"] != null ? dr["NodePath"].ToString().Trim() : string.Empty;
                        fleets.Add(fleet);
                    }
                    catch (Exception exception)
                    {

                    }

                }
            }
            catch (Exception exception)
            {

            }
            return fleets;
        }

        public static IList<Vehicle> GetVehiclesList(string myXmlString, string culture)
        {
            return GetVehiclesList(myXmlString, -1, -1, culture);
        }

        public static IList<Vehicle> GetVehiclesList(string myXmlString)
        {            
            return GetVehiclesList(myXmlString, -1, -1, "en");
        }

        public static IList<Vehicle> GetVehiclesList(string myXmlString, double lon, double lat)
        {
            return GetVehiclesList(myXmlString, lon, lat, "en");
        }

        public static IList<Vehicle> GetVehiclesList(string myXmlString, double lon, double lat, string culture)
        {
            List<Vehicle> vehicles = new List<Vehicle>();
            myXmlString = myXmlString.Replace("-05:00", "").Replace("-04:00", "");
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(myXmlString); // suppose that myXmlString contains "<Names>...</Names>"
                XmlNodeList xnList = xml.GetElementsByTagName("VehiclesLastKnownPositionInformation");
                foreach (XmlNode sNode in xnList)
                {
                    try
                    {
                        Vehicle vehicle = new Vehicle();
                        vehicle.BoxId = sNode["BoxId"] != null ? Convert.ToInt32(sNode["BoxId"].InnerText.Trim()) : 0;
                        vehicle.VehicleId = sNode["VehicleId"] != null ? Convert.ToInt32(sNode["VehicleId"].InnerText.Trim()) : 0;
                        vehicle.LicensePlate = sNode["LicensePlate"] != null ? sNode["LicensePlate"].InnerText.Trim() : null;
                        vehicle.OriginDateTime = sNode["OriginDateTime"] != null ? Convert.ToDateTime(sNode["OriginDateTime"].InnerText.Trim()) : DateTime.Now;
                        if (culture.ToLower().IndexOf("fr") >= 0)
                            vehicle.DisplayDateTime = vehicle.OriginDateTime.ToString("dd/MM/yyyy HH:mm:ss");
                        else
                            vehicle.DisplayDateTime = vehicle.OriginDateTime.ToString("MM/dd/yyyy hh:mm:ss tt");
                        vehicle.Latitude = sNode["Latitude"] != null ? Convert.ToDouble(sNode["Latitude"].InnerText.Trim()) : 0;
                        vehicle.Longitude = sNode["Longitude"] != null ? Convert.ToDouble(sNode["Longitude"].InnerText.Trim()) : 0;
                        vehicle.Speed = sNode["Speed"] != null ? Convert.ToInt32(sNode["Speed"].InnerText.Trim()) : 0;
                        vehicle.StreetAddress = sNode["StreetAddress"] != null ? sNode["StreetAddress"].InnerText.Trim() : null;
                        vehicle.Description = sNode["Description"] != null ? sNode["Description"].InnerText.Trim() : null;
                        vehicle.VehicleStatus = sNode["VehicleStatus"] != null ? sNode["VehicleStatus"].InnerText.Trim() : null;
                        vehicle.SensorMask = sNode["SensorMask"] != null ? Convert.ToInt64(sNode["SensorMask"].InnerText.Trim()) : 0;
                        vehicle.MyHeading = sNode["MyHeading"] != null ? sNode["MyHeading"].InnerText.Trim() : null;
                        vehicle.Driver = sNode["Driver"] != null ? sNode["Driver"].InnerText.Trim() : null;

                        if (lon != -1 && lat != -1 && vehicle.Latitude != 0 && vehicle.Longitude != 0)
                        {
                            vehicle.Distance = DistanceAlgorithm.DistanceBetweenPlaces(lon, lat, vehicle.Longitude, vehicle.Latitude);
                        }

                        vehicles.Add(vehicle);
                    }
                    catch (Exception exception)
                    {

                    }

                }
            }
            catch (Exception exception)
            {

            }
            return vehicles;
        }

        public static DataSet GetSuperOrganizationList(string constr, int superOrganizationId)
        {
            string sql = "";
            if (superOrganizationId == 410)
                sql = "SELECT * FROM vlfOrganization ORDER BY OrganizationName";
            else
                sql = string.Format("SELECT * FROM vlfOrganization WHERE SuperOrganizationId ={0} ORDER BY OrganizationName", superOrganizationId);

            DataSet dsOrganizations = new DataSet();
            dsOrganizations = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand com = new SqlCommand(sql, con);
                adapter.SelectCommand = com;
                adapter.Fill(dsOrganizations);
                con.Close();

            }

            return dsOrganizations;
        }

        public static string Heading(string heading)
        {
            int intHeading = 0;
            try
            {
                intHeading = Convert.ToInt16(heading);

            }
            catch
            {
                return "";
            }


            try
            {
                if (intHeading > 400)
                    return "";


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
                    return "";
            }
            catch
            {
                return "";
            }



        }

        public static IList<VehicleSensor> GetSensorsList(DataTable dt)
        {
            List<VehicleSensor> sensors = new List<VehicleSensor>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        VehicleSensor sensor = new VehicleSensor();

                        sensor.SensorId = dr["SensorId"] != null ? Convert.ToInt32(dr["SensorId"].ToString().Trim()) : 0;
                        sensor.SensorName = dr["SensorName"].ToString();
                        sensor.SensorAction = dr["SensorAction"].ToString();
                        sensor.AlarmLevelOn = dr["AlarmLevelOn"] != null ? Convert.ToInt16(dr["AlarmLevelOn"].ToString().Trim()) : (short)0;
                        sensor.AlarmLevelOff = dr["AlarmLevelOff"] != null ? Convert.ToInt16(dr["AlarmLevelOff"].ToString().Trim()) : (short)0;
                        sensor.ISInUse = Convert.ToBoolean(dr["ISInUse"]);
                        sensor.ISDefault = Convert.ToBoolean(dr["ISDefault"]);

                        sensors.Add(sensor);

                        System.Diagnostics.Debug.WriteLine("SensorId : " + sensor.SensorId);
                        System.Diagnostics.Debug.WriteLine("SensorName : " + sensor.SensorName);
                        System.Diagnostics.Debug.WriteLine("SensorAction : " + sensor.SensorAction);
                        System.Diagnostics.Debug.WriteLine("AlarmLevelOn : " + sensor.AlarmLevelOn);
                        System.Diagnostics.Debug.WriteLine("AlarmLevelOff : " + sensor.AlarmLevelOff);
                    }

                    catch (Exception exception)
                    {
                        sensors.Clear();
                    }

                }
            }
            catch (Exception exception)
            {
                sensors.Clear();
            }
            return sensors;
        }



        
    }

    public static class DistanceAlgorithm
    {
        const double PIx = 3.141592653589793;
        const double RADIUS = 6378.16;

        /// <summary>
        /// This class cannot be instantiated.
        /// </summary>
        //private DistanceAlgorithm() { }

        /// <summary>
        /// Convert degrees to Radians
        /// </summary>
        /// <param name="x">Degrees</param>
        /// <returns>The equivalent in radians</returns>
        public static double Radians(double x)
        {
            return x * PIx / 180;
        }

        /// <summary>
        /// Calculate the distance between two places.
        /// </summary>
        /// <param name="lon1"></param>
        /// <param name="lat1"></param>
        /// <param name="lon2"></param>
        /// <param name="lat2"></param>
        /// <returns></returns>
        public static double DistanceBetweenPlaces(
            double lon1,
            double lat1,
            double lon2,
            double lat2)
        {
            double dlon = Radians(lon2 - lon1);
            double dlat = Radians(lat2 - lat1);

            double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + Math.Cos(Radians(lat1)) * Math.Cos(Radians(lat2)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
            double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return angle * RADIUS;
        }
    }
}