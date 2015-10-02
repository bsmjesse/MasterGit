using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SentinelMobile.Models;
using VLF.CLS.Interfaces;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net.Mail;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using System.Threading;
using System.Net;

namespace SentinelMobile.Controllers
{
    public class HomeController : Controller
    {
        private SmtpClient mailClient;       

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        [Authorize]
        public ActionResult About(int? id)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);

            ViewBag.LastUpdatedCustomTheme = System.IO.File.GetLastWriteTime(Server.MapPath("~/Content/themes/custom/sfmjquerymobileCustom.css")).ToString("yyyyMMddHHmmss");            

            ViewBag.Title = SentinelMobile.Resources.Resources.About;
            ViewBag.DefaultLanguage = user.DefaultLanguage;

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult FleetViewHome()
        {
            return View();
        }

        [Authorize]
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ViewResult _vehicleList(int? id, int? pageIndex, int? isSearch, string searchString, string checkedVehicleIds)
        {
            int fleetId = id == null ? 0 : (int)id;
            
            IList<Vehicle> vehicles = new List<Vehicle>();
            //vehicles = SentinelFM.GetVehicles(fleetId);

            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);

            int? sessionFleetId;
            if(HttpContext.Session["FleetId"] == null)
                sessionFleetId = null;
            else
                sessionFleetId = int.Parse(HttpContext.Session["FleetId"].ToString());

            string sessionSearchString = HttpContext.Session["VehicleSearchString"] == null ? string.Empty : HttpContext.Session["VehicleSearchString"].ToString();

            bool fromSession = false;

            // try to get vehicles from session
            if (sessionFleetId == fleetId && (pageIndex??1) != 1 && ((isSearch == null || isSearch != 1) || (isSearch != null && isSearch == 1 && sessionSearchString == searchString)))
            {
                if (HttpContext.Session["SessionVehicles"] != null)
                {
                    vehicles = (IList<Vehicle>)HttpContext.Session["SessionVehicles"];
                    fromSession = true;
                }
            }

            if (!fromSession || vehicles == null || vehicles.Count == 0)
            {

                string c = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                IList<Vehicle> unsortedVehicles = SentinelMobile.Models.SentinelFM.GetVehicles(fleetId, c);

                if (isSearch != null && isSearch == 1 && unsortedVehicles != null)
                {
                    unsortedVehicles = unsortedVehicles.ToList<Vehicle>().FindAll(item => item.Description.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
                        //|| item.StreetAddress.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
                        //|| item.BoxId.ToString().IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
                        );
                }

                if (unsortedVehicles != null)
                {
                    IEnumerable<Vehicle> sortedEnum = unsortedVehicles.OrderByDescending(f => f.OriginDateTime);
                    vehicles = sortedEnum.ToList();
                }

                HttpContext.Session.Add("FleetId", fleetId);
                HttpContext.Session.Add("VehicleSearchString", searchString);
                HttpContext.Session.Add("SessionVehicles", vehicles);
            }
            
            var viewModel = new Vehicles
            {
                VehicleList = vehicles,
                PageIndex = pageIndex ?? 1,
                PageSize = 10,
                ShowCheckbox = true,
                ShowDistance = false,
                CheckedVehicleIds = checkedVehicleIds,
                UnitOfMes = user.UnitOfMes,
                UnitOfMesName = user.UnitOfMes < 0.9 ? SentinelMobile.Resources.Resources.SpeedMile : SentinelMobile.Resources.Resources.SpeedKm
            };

            return View(viewModel);
        }

        [Authorize]
        public ViewResult _fleetList(int? fleetId, int? pageIndex, int? isSearch, string searchString)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }
            
            IList<Fleet> fleets = SentinelMobile.Models.SentinelFM.GetFleets();
            ViewBag.FleetId = fleetId ?? 0;

            if (user.FleetType == "hierarchy" && searchString == string.Empty)
                return null;

            if (user.FleetType == "hierarchy")
            {
                string constr = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ToString();
                OrganizationHierarchy.Hierarchy _hie = new OrganizationHierarchy.Hierarchy(constr);

                DataSet ds = _hie.SearchOrganizationHierarchy(user.OrganizationId, searchString);
                ViewBag.Fleets = SentinelMobile.Models.SentinelFM.GetFleetsList(ds);
            }
            else
            {

                if (isSearch != null && isSearch == 1)
                {
                    ViewBag.Fleets = fleets.ToList<Fleet>().FindAll(item => item.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
                        );
                }
                else
                {
                    ViewBag.Fleets = fleets;
                }
            } 
            ViewBag.PageIndex = pageIndex ?? 1;
            ViewBag.FleetType = user.FleetType;

            return View("_fleetList");
        }

        [Authorize]
        public ActionResult FleetView(int? id)
        {   
            int fleetId = 0;
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }
            
            if (user.DefaultFleet > 0)
                fleetId = user.DefaultFleet;
            else
            {
                IList<Fleet> fleets = SentinelMobile.Models.SentinelFM.GetFleets();
                if (fleets.Count > 0)
                    fleetId = fleets[0].FleetId;
            }
            ViewBag.OrganizationId = user.OrganizationId;
            ViewBag.OrganziationName = user.OrganizationName;
            ViewBag.Reefer = false;
            ViewBag.UserFullName = user.FirstName + " " + user.LastName;
            ViewBag.UserName = user.UserName;

            //string ReeferOrganizationId = System.Configuration.ConfigurationManager.AppSettings["ReeferOrganizationId"].ToString();
            //char[] delimiters = new char[] { ',', ';' };
            //List<int> organizations = ReeferOrganizationId.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Where(x => Regex.IsMatch(x, @"\d")).Select(n => int.Parse(n)).ToList();
            
            //if (organizations.Contains(user.OrganizationId))
            //{
            //    ViewBag.Reefer = true;
            //}

            ViewBag.Reefer = user.ControlEnable(69);

            string constr = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ToString();
            OrganizationHierarchy.Hierarchy _hie = new OrganizationHierarchy.Hierarchy(constr);
            ViewBag.FleetName = _hie.GetFleetNameByFleetId(user.OrganizationId, fleetId);

            //vehicles = SentinelFM.GetVehicles(fleetId);
            ViewBag.FleetId = fleetId;
            
            ViewBag.LastUpdatedmobilebaseFile = System.IO.File.GetLastWriteTime(Server.MapPath("~/Scripts/mobile-base.js")).ToString("yyyyMMddHHmmss");
            ViewBag.LastUpdatedmobilejqFile = System.IO.File.GetLastWriteTime(Server.MapPath("~/Scripts/mobile-jq.js")).ToString("yyyyMMddHHmmss");
            ViewBag.LastUpdatedCustomTheme = System.IO.File.GetLastWriteTime(Server.MapPath("~/Content/themes/custom/sfmjquerymobileCustom.css")).ToString("yyyyMMddHHmmss");

            ViewBag.FleetType = user.FleetType;
            

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);

            if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
                ViewBag.SelectedLanguage = "fr";
            else
                ViewBag.SelectedLanguage = "en";

            ViewBag.DefaultLanguage = user.DefaultLanguage;

            ViewBag.FleetPageTitle = user.FleetType == "flat" ? SentinelMobile.Resources.Resources.Fleets : SentinelMobile.Resources.Resources.Hierarchy;
            ViewBag.FleetPageSearchPlaceHolder = user.FleetType == "flat" ? SentinelMobile.Resources.Resources.SearchFleets : SentinelMobile.Resources.Resources.EnterHierarchyName;

            ViewBag.TimeZone = user.TimeZone;
            ViewBag.DayLightSaving = user.DayLightSaving;

            //string ConfigPageAllowedUserGroupIDs = System.Configuration.ConfigurationManager.AppSettings["ConfigPageAllowedUserGroupIDs"] ?? "1,2";

            //if (ConfigPageAllowedUserGroupIDs.ToLower().Trim() == "all")
            //{
            //    ViewBag.EnableConfigPage = true;
            //}
            //else
            //{

            //    //if (user.UserGroupId == 1 || user.UserGroupId == 2 || user.UserGroupId == 7)    // Hgi, Security Admin, Armed Car Security Admin
            //    List<string> allowedIDs = ConfigPageAllowedUserGroupIDs.Split(new char[] { ' ', ',', ';' }).Select(p => p.Trim()).ToList();
            //    if (allowedIDs.Contains(user.UserGroupId.ToString()))
            //    {
            //        ViewBag.EnableConfigPage = true;
            //    }
            //    else
            //    {
            //        ViewBag.EnableConfigPage = false;
            //    }
            //}
            ViewBag.EnableConfigPage = user.ControlEnable(113);

            ViewBag.EmailRecipients = System.Configuration.ConfigurationManager.AppSettings["EmailRecipients"] ?? "test@bsmwireless.com";
            
            return View();
        }

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult _GetAddressLatLon(int id, string s)
        {

            double lat = 0;
            double lon = 0;
            string rsAddress = null;

            Resolver.Resolver resolver = new Resolver.Resolver();

            bool find = resolver.Location(s, ref lat, ref lon, ref rsAddress);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("lat", Convert.ToString(lat));
            result.Add("lon", Convert.ToString(lon));
            result.Add("Address", rsAddress);
            //var oSerializer = new JavaScriptSerializer();
            //string json = oSerializer.Serialize(result);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public string _FindMyVehiclesByPosition(double lon, double lat, int fleetId)
        //{
        //    User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
        //    if (user == null)
        //    {
        //        return null;
        //    }
            
        //    DBFleet.DBFleet fleet = new DBFleet.DBFleet();
        //    string xml = string.Empty;
        //    fleet.GetVehiclesLastKnownPositionInfoNearestToLatLon(user.UserId, user.SecId, user.OrganizationId, fleetId, 100, lat, lon, ref xml);
        //    return xml;
        //}

        [Authorize]
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult _FindMyVehiclesByPosition(double lon, double lat, int fleetId, int searchNumVehicles, int searchRadius)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }
            
            DBFleet.DBFleet fleet = new DBFleet.DBFleet();
            string xml = string.Empty;
            fleet.GetVehiclesLastKnownPositionInfoNearestToLatLon(user.UserId, user.SecId, user.OrganizationId, fleetId, searchRadius * 1000, lat, lon, ref xml);

            IList<Vehicle> unsortedVehicles = new List<Vehicle>();
            unsortedVehicles = SentinelMobile.Models.SentinelFM.GetVehiclesList(xml, lon, lat);

            IEnumerable<Vehicle> sortedEnum = unsortedVehicles.OrderBy(f => f.Distance);
            IList<Vehicle> vehicles = sortedEnum.ToList();

            

            var viewModel = new Vehicles
            {
                VehicleList = vehicles,
                PageIndex = 1,
                PageSize = searchNumVehicles,
                ShowCheckbox = false,
                ShowDistance = true,
                CheckedVehicleIds = "",
                UnitOfMes = user.UnitOfMes,
                UnitOfMesName = user.UnitOfMes < 0.9 ? SentinelMobile.Resources.Resources.SpeedMile : SentinelMobile.Resources.Resources.SpeedKm
            };

            return View("_vehicleList", viewModel);

            //return xml;
        }

        public ActionResult Test()
        {
            return View();
        }

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult GetVehicleLastKnown(int id, string startDateTime)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                Dictionary<string, string> r = new Dictionary<string, string>();
                r.Add("success", "0");
                r.Add("Msg", "sessiontimeout");
                return Json(r, JsonRequestBehavior.AllowGet);
            }

            int vehicleId = id;

            

            string cstart = Convert.ToDateTime(startDateTime).AddHours(-user.TimeZone - user.DayLightSaving).AddSeconds(1).ToString("MM/dd/yyyy HH:mm:ss");
            //string cstart = Convert.ToDateTime(startDateTime).AddHours(-user.TimeZone - user.DayLightSaving).AddSeconds(0.1).ToString("MM/dd/yyyy HH:mm:ss");
            string cend = DateTime.Now.AddHours(24).ToString("MM/dd/yyyy HH:mm:ss");

            Dictionary<string, object> result = this._getHistoryRecords(vehicleId, cstart, cend, user);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult SearchVehicleHistory(int id, string startDateTime, string toDateTime)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                Dictionary<string, string> r = new Dictionary<string, string>();
                r.Add("success", "0");
                r.Add("Msg", "sessiontimeout");
                return Json(r, JsonRequestBehavior.AllowGet);
            }

            int vehicleId = id;

            string dateTimeFormat = string.Empty;
            if (user.DefaultLanguage.ToLower().IndexOf("fr") >= 0)
                dateTimeFormat = "dd/MM/yyyy HH:mm";
            else
                dateTimeFormat = "M/d/yyyy hh:mm tt";

            startDateTime = DateTime.ParseExact(startDateTime, dateTimeFormat, null).AddHours(-user.TimeZone - user.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");
            toDateTime = DateTime.ParseExact(toDateTime, dateTimeFormat, null).AddHours(-user.TimeZone - user.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");

            //string cstart = Convert.ToDateTime(startDateTime).AddHours(-user.TimeZone - user.DayLightSaving).AddSeconds(1).ToString("MM/dd/yyyy HH:mm:ss");            
            //string cend = DateTime.Now.AddHours(24).ToString("MM/dd/yyyy HH:mm:ss");

            Dictionary<string, object> result = this._getHistoryRecords(vehicleId, startDateTime, toDateTime, user);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IsAuthorized()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();            

            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null || !User.Identity.IsAuthenticated)
            {
                result.Add("IsAuthorized", "0");
            }
            else
            {
                result.Add("IsAuthorized", "1");
            }
            
            return Json(result, JsonRequestBehavior.AllowGet);            
        }

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult _sendCommand(int boxId, string cmdName, string cmdValue, int vehicleId)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                Dictionary<string, string> r = new Dictionary<string, string>();
                r.Add("success", "0");
                r.Add("Msg", "sessiontimeout");
                return Json(r, JsonRequestBehavior.AllowGet);
            }

            Cmd cmd = HttpContext.Session != null ? (Cmd)HttpContext.Session["Cmd"] : null;
            if (cmd == null)
            {
                return null;
            }

            short commandId = -1;

            if (cmdName.ToLower().Trim() == "setodometer")
            {
                commandId = 59;
            }
            else if (cmdName.ToLower().Trim() == "getboxstatus")
            {
                commandId = 16;
            }
            else if (cmdName.ToLower().Trim() == "updateposition")
            {
                commandId = 13;
            }
            else if (cmdName.ToLower().Trim() == "setfuelconfiguration")
            {
                //commandId = 100;                
                commandId = (Int16)VLF.CLS.Def.Enums.CommandType.WriteEEPROMData;
            }
            else if (cmdName.ToLower().Trim() == "setseatbeltodometer" || cmdName.ToLower().Trim() == "setseatbeltodometernext1" || cmdName.ToLower().Trim() == "setseatbeltodometernext2")
            {
                commandId = (Int16)VLF.CLS.Def.Enums.CommandType.WriteEEPROMData;
            }
            else if (cmdName.ToLower().Trim() == "reeferchangeoperatingmode")
            {
                commandId = 115;
            }
            else if (cmdName.ToLower().Trim() == "reeferChangeSetPoint".ToLower())
            {
                commandId = 112;
            }
            else if (cmdName.ToLower().Trim() == "reeferClearAlarm".ToLower())
            {
                commandId = 113;
            }
            else if (cmdName.ToLower().Trim() == "reeferOpenFreshAirDoor".ToLower())
            {
                commandId = 116;
            }
            else if (cmdName.ToLower().Trim() == "reeferCloseFreshAirDoor".ToLower())
            {
                commandId = 117;
            }
            else if (cmdName.ToLower().Trim() == "reeferInitiatePreTrip".ToLower())
            {
                commandId = 114;
            }
            else if (cmdName.ToLower().Trim() == "reeferInitiateDefrost".ToLower())
            {
                commandId = 111;
            }
            else if (cmdName.ToLower().Trim() == "reeferMultipleAlarmRead".ToLower())
            {
                commandId = 120;
            }
            else if (cmdName.ToLower().Trim() == "reeferSoftwareIdentification".ToLower())
            {
                commandId = 121;
            }
            else if (cmdName.ToLower().Trim() == "reeferTurnPowerOff".ToLower())
            {
                commandId = 118;
            }
            else if (cmdName.ToLower().Trim() == "reeferTurnPowerOn".ToLower())
            {
                commandId = 119;
            }

            List<string> allowedCommands = cmd.CommandList.Split(new char[] { ' ', ',', ';' }).Select(p => p.Trim()).ToList();
            if (!allowedCommands.Contains(commandId.ToString()))
            {
                return null;
            }
            

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);
            
            Dictionary<string, string> result = new Dictionary<string,string>();
            //if (cmdName.ToLower().Trim() == "setodometer")
                result = _sendCommandToBox(boxId, cmdName, cmdValue);
                result.Add("VehicleId", vehicleId.ToString());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult _checkCommandStatus(int boxId, short ProtocolTypeId, short CommModeId, int vehicleId)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }

            Cmd cmd = HttpContext.Session != null ? (Cmd)HttpContext.Session["Cmd"] : null;
            if (cmd == null)
            {
                return null;
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);

            Dictionary<string, string> result = new Dictionary<string, string>();

            int cmdStatus = 0;

            LocationMgr.Location dbl = new LocationMgr.Location();
            
            if (dbl.GetCommandStatus(user.UserId, user.SecId, boxId, ProtocolTypeId, ref cmdStatus) != 0)
                if (dbl.GetCommandStatus(user.UserId, user.SecId, boxId, ProtocolTypeId, ref cmdStatus) != 0)
                {

                    result.Add("Status", "500");
                    result.Add("Msg", SentinelMobile.Resources.Resources.MsgGetCommanStatusFailed);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            if ((cmdStatus == (int)CommandStatus.Ack) || (cmdStatus == (int)CommandStatus.CommTimeout) || (cmdStatus == (int)CommandStatus.Pending))
            {
                switch (cmdStatus)
                {
                    case (int)CommandStatus.Ack:                        
                    case (int)CommandStatus.Pending:
                        result.Add("Status", "200");
                        if (CommModeId == 0)
                            result.Add("Msg", SentinelMobile.Resources.Resources.CommandSuccessful);
                        else
                        {
                            result.Add("Msg", SentinelMobile.Resources.Resources.CommandSuccessfulWith + " " + (VLF.CLS.Def.Enums.CommMode)CommModeId + ".");
                        }

                        if (cmd.CommandId == 16)    // get box status
                        {
                            string ss = GetBoxStatus(false, cmd.LicensePlate);
                            result.Add("boxStatus", ss);
                        }
                        else if (cmd.CommandId == 13) // updateposition
                        {
                            string ss = GetBoxPosition(vehicleId);
                            result.Add("boxPosition", ss);
                        }

                        break;
                    case (int)CommandStatus.CommTimeout:
                        result.Add("Status", "500");
                        result.Add("Msg", SentinelMobile.Resources.Resources.CommandTimeOut + ".");
                        break;
                    
                }

                result.Add("Waiting", "0");
            }
            else
            {
                result.Add("Status", "200");
                result.Add("Waiting", "1");
            }
            

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private Dictionary<string, string> _sendCommandToBox(int boxId, string cmdName, string cmdValue)
        {
            string paramList = "";
            Dictionary<string, string> result = new Dictionary<string, string>();

            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                result.Add("Status", "500");
                result.Add("Msg", SentinelMobile.Resources.Resources.SessionTimeOut);

                return result;
            }

            LocationMgr.Location dbl = new LocationMgr.Location();

            short commandId = 59;

            if (cmdName.ToLower().Trim() == "setodometer")
            {
                if (Convert.ToInt32(Convert.ToSingle(cmdValue)) > 0 && Convert.ToInt32(Convert.ToSingle(cmdValue)) < 16777215)
                {
                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyOdometerValue, cmdValue);
                    commandId = 59;
                }
                else
                {
                    result.Add("Status", "500");
                    result.Add("Msg", SentinelMobile.Resources.Resources.InvalidOdometer2);
                    return result;
                }
            }
            else if (cmdName.ToLower().Trim() == "getboxstatus")
            {
                commandId = 16;
            }
            else if (cmdName.ToLower().Trim() == "updateposition")
            {
                commandId = 13;
            }
            else if (cmdName.ToLower().Trim() == "setfuelconfiguration")
            {
                //commandId = 100;
                commandId = (Int16)VLF.CLS.Def.Enums.CommandType.WriteEEPROMData;
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMOffset, "0xA0");
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMLength, "12");
                string EEPPROM = GenerateFuelEEPROMData(cmdValue);
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMData, EEPPROM);
            }
            else if (cmdName.ToLower().Trim() == "setseatbeltodometer")
            {
                string fileName = Server.MapPath("~/App_Data/") + "Ford.xls";
                DataTable dtOHData = ReadDataFromExcelUsingNPOI(fileName);
                string[] p = cmdValue.Split(';');

                if (p.Length == 3)
                {
                    if (p[2].Length != 4)
                    {
                        result.Add("Status", "500");
                        result.Add("Msg", "na");
                        return result;
                    }

                    string eepromOffset = "";
                    string eepromLength = "";
                    string eepromData = "";
                    DataRow[] foundRows = dtOHData.Select("Make = '" + p[0] + "' AND '" + p[1] + "' LIKE '%' + Model + '%' AND Year LIKE '%" + p[2] + "%'");
                    if (foundRows.Length > 0)
                    {
                        eepromOffset = foundRows[0]["Offset"].ToString();
                        eepromData = foundRows[0]["EEPROM"].ToString();
                        eepromLength = eepromData.Split(' ').Length.ToString();
                    }
                    else
                    {
                        result.Add("Status", "500");
                        result.Add("Msg", "na");

                        return result;
                    }

                    //commandId = 100;
                    commandId = (Int16)VLF.CLS.Def.Enums.CommandType.WriteEEPROMData;
                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMOffset, eepromOffset);
                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMLength, eepromLength);
                    //string EEPPROM = GenerateFuelEEPROMData(cmdValue);
                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMData, eepromData);
                }
                else
                {
                    result.Add("Status", "500");
                    result.Add("Msg", "na");

                    return result;
                }
            }
            else if (cmdName.ToLower().Trim() == "setseatbeltodometernext1")
            {
                commandId = (Int16)VLF.CLS.Def.Enums.CommandType.WriteEEPROMData;
                string eepromOffset = "0X286";
                string eepromData = "00 86";
                string eepromLength = "2";

                commandId = (Int16)VLF.CLS.Def.Enums.CommandType.WriteEEPROMData;
                paramList = VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMOffset, eepromOffset);
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMLength, eepromLength);
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMData, eepromData);
            }
            else if (cmdName.ToLower().Trim() == "setseatbeltodometernext2")
            {
                commandId = (Int16)VLF.CLS.Def.Enums.CommandType.WriteEEPROMData;
                string eepromOffset = "0x1F8";
                string eepromData = "13 49 69 01 CE";
                string eepromLength = "5";

                commandId = (Int16)VLF.CLS.Def.Enums.CommandType.WriteEEPROMData;
                paramList = VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMOffset, eepromOffset);
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMLength, eepromLength);
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMData, eepromData);
            }
            else if (cmdName.ToLower().Trim() == "reeferChangeOperatingMode".ToLower())
            {
                commandId = 115;
                paramList += VLF.CLS.Util.MakePair("OP_MODE", cmdValue);
            }
            else if (cmdName.ToLower().Trim() == "reeferChangeSetPoint".ToLower())
            {
                commandId = 112;
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyTemperature, cmdValue);
                paramList += VLF.CLS.Util.MakePair("ZONE", "1");
            }
            else if (cmdName.ToLower().Trim() == "reeferClearAlarm".ToLower())
            {
                commandId = 113;
            }
            else if (cmdName.ToLower().Trim() == "reeferOpenFreshAirDoor".ToLower())
            {
                commandId = 116;
            }
            else if (cmdName.ToLower().Trim() == "reeferCloseFreshAirDoor".ToLower())
            {
                commandId = 117;
            }
            else if (cmdName.ToLower().Trim() == "reeferInitiatePreTrip".ToLower())
            {
                commandId = 114;
            }
            else if (cmdName.ToLower().Trim() == "reeferInitiateDefrost".ToLower())
            {
                commandId = 111;
                paramList += VLF.CLS.Util.MakePair("ZONE", "1");
            }
            else if (cmdName.ToLower().Trim() == "reeferMultipleAlarmRead".ToLower())
            {
                commandId = 120;
            }
            else if (cmdName.ToLower().Trim() == "reeferSoftwareIdentification".ToLower())
            {
                commandId = 121;
            }
            else if (cmdName.ToLower().Trim() == "reeferTurnPowerOff".ToLower())
            {
                commandId = 118;
            }
            else if (cmdName.ToLower().Trim() == "reeferTurnPowerOn".ToLower())
            {
                commandId = 119;
            }
            else
            {
                result.Add("Status", "500");
                result.Add("Msg", "na");

                return result;
            }
            
            short ProtocolId = 23;
            short CommModeId = 6;
            bool cmdSent = false;
            Int64 sessionTimeOut = 0;

            CheckCommMode(boxId, user, ref CommModeId, ref ProtocolId);
            
            if (dbl.SendCommand(user.UserId, user.SecId, DateTime.Now, boxId, commandId, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut) != 0)
                if (dbl.SendCommand(user.UserId, user.SecId, DateTime.Now, boxId, commandId, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut) != 0)
                {
                    result.Add("Status", "500");
                    //result.Add("Msg", SentinelMobile.Resources.Resources.FailedToSetOdometer);
                    result.Add("Msg", SentinelMobile.Resources.Resources.SendCommandFailedUsing + " " + (VLF.CLS.Def.Enums.CommMode)CommModeId);

                    return result;
                }

            Cmd cmd = HttpContext.Session != null ? (Cmd)HttpContext.Session["Cmd"] : null;
            if (cmd == null)
            {
                cmd = new Cmd();
                cmd.CommandId = commandId;
                HttpContext.Session.Add("Cmd", cmd);
            }
            else
            {
                cmd.CommandId = commandId;
                HttpContext.Session["Cmd"] = cmd;
            }

            // Hard-coded to send two more commands if it's setseatbeltodometer
            /*if (cmdName.ToLower().Trim() == "setseatbeltodometer")
            {
                string eepromOffset = "";
                string eepromLength = "";
                string eepromData = "";

                eepromOffset = "0X286";
                eepromData = "00 86";
                eepromLength = "2";
                
                commandId = (Int16)VLF.CLS.Def.Enums.CommandType.WriteEEPROMData;
                paramList = VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMOffset, eepromOffset);
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMLength, eepromLength);                
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMData, eepromData);

                Int64 TaskId = 0;
                if (dbl.SendAutomaticCommandWithoutCommParams(user.UserId, user.SecId, boxId, commandId, DateTime.Now, paramList, 10800, 60, false, ref TaskId) != 0)
                    if (dbl.SendAutomaticCommandWithoutCommParams(user.UserId, user.SecId, boxId, commandId, DateTime.Now, paramList, 10800, 60, false, ref TaskId) != 0)
                    {
                        result.Add("Status", "500");
                        //result.Add("Msg", SentinelMobile.Resources.Resources.FailedToSetOdometer);
                        result.Add("Msg", SentinelMobile.Resources.Resources.SendCommandFailedUsing + " " + (VLF.CLS.Def.Enums.CommMode)CommModeId);

                        return result;
                    }

                eepromOffset = "0x1F8";
                eepromData = "13 49 69 01 CE";
                eepromLength = "5";

                commandId = (Int16)VLF.CLS.Def.Enums.CommandType.WriteEEPROMData;
                paramList = VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMOffset, eepromOffset);
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMLength, eepromLength);
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMData, eepromData);

                //Thread.Sleep(20000);                
                //if (dbl.SendCommand(user.UserId, user.SecId, DateTime.Now, boxId, commandId, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut) != 0)
                //    if (dbl.SendCommand(user.UserId, user.SecId, DateTime.Now, boxId, commandId, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut) != 0)
                //    {
                //        result.Add("Status", "500");
                //        //result.Add("Msg", SentinelMobile.Resources.Resources.FailedToSetOdometer);
                //        result.Add("Msg", SentinelMobile.Resources.Resources.SendCommandFailedUsing + " " + (VLF.CLS.Def.Enums.CommMode)CommModeId);

                //        return result;
                //    }
                TaskId = 0;
                if (dbl.SendAutomaticCommandWithoutCommParams(user.UserId, user.SecId, boxId, commandId, DateTime.Now, paramList, 10800, 60, false, ref TaskId) != 0)
                    if (dbl.SendAutomaticCommandWithoutCommParams(user.UserId, user.SecId, boxId, commandId, DateTime.Now, paramList, 10800, 60, false, ref TaskId) != 0)
                    {
                        result.Add("Status", "500");
                        //result.Add("Msg", SentinelMobile.Resources.Resources.FailedToSetOdometer);
                        result.Add("Msg", SentinelMobile.Resources.Resources.SendCommandFailedUsing + " " + (VLF.CLS.Def.Enums.CommMode)CommModeId);

                        return result;
                    }
                
            }*/
            
            result.Add("Status", "200");
            result.Add("Msg", "");
            result.Add("BoxId", boxId.ToString());
            result.Add("ProtocolTypeId", ProtocolId.ToString());
            result.Add("CommModeId", CommModeId.ToString());

            return result;

            
        }


        private void CheckCommMode(int boxId, User user, ref short CommModeId, ref short ProtocolId)
        {
            try
            {

                DataSet ds = new DataSet();

                StringReader strrXML = null;
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (dbv.GetBoxConfigInfo(user.UserId, user.SecId, boxId, ref xml) != 0)
                    if (dbv.GetBoxConfigInfo(user.UserId, user.SecId, boxId, ref xml) !=0 )
                    {
                        
                        return;
                    }

                if (xml == "")
                {

                    return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);

                CommModeId = Convert.ToInt16(ds.Tables[0].Rows[0]["CommModeId"]);
                ProtocolId = Convert.ToInt16(ds.Tables[0].Rows[0]["BoxProtocolTypeId"]);

            }

            catch (Exception Ex)
            {
                //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        [Authorize]
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult _updateVehicleDescription(Int64 vehicleId, string vehicleDescription)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);

            Dictionary<string, string> result = new Dictionary<string, string>();

            string constr = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ToString();

            int _r = SentinelMobile.Models.SentinelFM.UpdateVehicleDescription(user.UserId, constr, vehicleId, vehicleDescription);

            result.Add("result", _r.ToString());

            if(_r > 0)
                result.Add("Msg", SentinelMobile.Resources.Resources.UpdateVehicleNameSuccess);
            else
                result.Add("Msg", SentinelMobile.Resources.Resources.UpdateVehicleNameFail);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult _swapVehicle(Int64 vehicleId)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);

            Dictionary<string, string> result = new Dictionary<string, string>();

            string constr = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ToString();

            //int _r = SentinelMobile.Models.SentinelFM.UpdateVehicleDescription(user.UserId, constr, vehicleId, "Box " + boxId.ToString());

            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ToString();
            //VLF.DAS.Logic.Vehicle vehicle = new VLF.DAS.Logic.Vehicle(sConnectionString);
            OrganizationHierarchy.Hierarchy vehicle = new OrganizationHierarchy.Hierarchy(sConnectionString);
            int _r = vehicle.swapVehicle(vehicleId);

            result.Add("result", _r.ToString());

            if (_r > 0)
            {
                result.Add("Msg", SentinelMobile.Resources.Resources.UpdateVehicleNameSuccess);
            }
            else
                result.Add("Msg", SentinelMobile.Resources.Resources.UpdateVehicleNameFail);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult _getVehicleCommands(int boxId, string licensePlate)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);
            Dictionary<string, string> result = new Dictionary<string, string>();

            try
            {

                DataSet ds = new DataSet();
                StringReader strrXML = null;
                
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                //DumpBeforeCall(sn, string.Format("CboCommand_Fill : LicensePlate = {0}", LicensePlate));
                if (dbv.GetVehicleCommandsXMLByLang(user.UserId, user.SecId, licensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml) != 0 )
                    if (dbv.GetVehicleCommandsXMLByLang(user.UserId, user.SecId, licensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml) != 0 )
                    {
                        result.Add("hasCommands", "0");
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                if (xml == "")
                {
                    result.Add("hasCommands", "0");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);

                short ProtocolId = 23;
                short CommModeId = 6;
                
                CheckCommMode(boxId, user, ref CommModeId, ref ProtocolId);

                ds.Tables[0].Select();
                ds.Tables[0].Select("BoxProtocolTypeId=" + ProtocolId.ToString());
                
                bool setOdometer = false;
                bool updatePosition = false;
                bool setFuelConfiguration = false;
                bool getboxstatus = false;

                string commandList = "";

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (Convert.ToInt32(dr["BoxCmdOutTypeId"].ToString()) == 59)
                    {
                        setOdometer = true;
                        commandList += "59;";
                    }
                    else if (Convert.ToInt32(dr["BoxCmdOutTypeId"].ToString()) == 13)
                    {
                        updatePosition = true;
                        commandList += "13;";
                    }
                    else if (Convert.ToInt32(dr["BoxCmdOutTypeId"].ToString()) == 16)
                    {
                        getboxstatus = true;
                        commandList += "16;";
                    }                        
                    else if (Convert.ToInt32(dr["BoxCmdOutTypeId"].ToString()) == 100)
                    {
                        setFuelConfiguration = true;
                        commandList += "100;";
                        commandList += "75;";
                    }
                    else if (Convert.ToInt32(dr["BoxCmdOutTypeId"].ToString()) > 100)
                    {
                        commandList += dr["BoxCmdOutTypeId"].ToString() + ";";
                    }
                }
                result.Add("setOdometer", setOdometer.ToString().ToLower());
                result.Add("updatePosition", updatePosition.ToString().ToLower());
                result.Add("getboxstatus", getboxstatus.ToString().ToLower());
                result.Add("setFuelConfiguration", setFuelConfiguration.ToString().ToLower());
                result.Add("hasCommands", "1");

                Cmd cmd = HttpContext.Session != null ? (Cmd)HttpContext.Session["Cmd"] : null;
                if (cmd == null)
                {
                    cmd = new Cmd();
                    cmd.CommandList = commandList;
                    cmd.LicensePlate = licensePlate;
                    HttpContext.Session.Add("Cmd", cmd);
                }
                else
                {
                    cmd.CommandList = commandList;
                    cmd.LicensePlate = licensePlate;
                    HttpContext.Session["Cmd"] = cmd;
                }

                return Json(result, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception Ex)
            {
                //TODO: log exception
            }

            result.Add("hasCommands", "0");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult _getVehicleInfoByLicensePlate(string licensePlate)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);
            Dictionary<string, string> result = new Dictionary<string, string>();

            try
            {

                DataSet ds = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                //DumpBeforeCall(sn, string.Format("CboCommand_Fill : LicensePlate = {0}", LicensePlate));
                if (dbv.GetVehicleInfoXML(user.UserId, user.SecId, licensePlate, ref xml) != 0)
                    if (dbv.GetVehicleInfoXML(user.UserId, user.SecId, licensePlate, ref xml) != 0)                        
                    {
                        result.Add("hasVehicleInfo", "0");
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                if (xml == "")
                {
                    result.Add("hasVehicleInfo", "0");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    result.Add("VIN", ds.Tables[0].Rows[0]["VinNum"].ToString());
                    result.Add("LicensePlate", licensePlate);
                    result.Add("VehicleTypeName", ds.Tables[0].Rows[0]["VehicleTypeName"].ToString().TrimEnd());
                    result.Add("MakeName", ds.Tables[0].Rows[0]["MakeName"].ToString().TrimEnd());
                    result.Add("MakeModelId", ds.Tables[0].Rows[0]["MakeModelId"].ToString());
                    result.Add("ModelYear", ds.Tables[0].Rows[0]["ModelYear"].ToString());
                    result.Add("Color", ds.Tables[0].Rows[0]["Color"].ToString());
                    result.Add("Cost", ds.Tables[0].Rows[0]["CostPerMile"].ToString());
                    result.Add("Description", ds.Tables[0].Rows[0]["Description"].ToString());
                    result.Add("StateProvince", ds.Tables[0].Rows[0]["StateProvince"].ToString());
                    result.Add("VehicleId", ds.Tables[0].Rows[0]["VehicleId"].ToString());
                    result.Add("BoxId", ds.Tables[0].Rows[0]["BoxId"].ToString());
                    result.Add("IconTypeId", ds.Tables[0].Rows[0]["IconTypeId"].ToString());
                    result.Add("Email", ds.Tables[0].Rows[0]["Email"].ToString());
                    result.Add("Phone", ds.Tables[0].Rows[0]["Phone"].ToString());
                    result.Add("TimeZone", ds.Tables[0].Rows[0]["TimeZone"].ToString());
                    result.Add("AutoAdjustDayLightSaving", ds.Tables[0].Rows[0]["AutoAdjustDayLightSaving"].ToString());
                    result.Add("FuelType", ds.Tables[0].Rows[0]["FormatType"] != DBNull.Value ? ds.Tables[0].Rows[0]["FormatType"].ToString() : "0");
                    result.Add("Notify", ds.Tables[0].Rows[0]["Notify"].ToString());
                    result.Add("Warning", ds.Tables[0].Rows[0]["Warning"].ToString());
                    result.Add("Critical", ds.Tables[0].Rows[0]["Critical"].ToString());                    
                    result.Add("Maintenance", ds.Tables[0].Rows[0]["Maintenance"].ToString());
                    result.Add("ServiceConfigID", ds.Tables[0].Rows[0]["ServiceConfigID"] != DBNull.Value ? ds.Tables[0].Rows[0]["ServiceConfigID"].ToString() : "-1");
                    result.Add("hasVehicleInfo", "1");
                }
                else
                {
                    result.Add("hasVehicleInfo", "0");
                }

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {
                //TODO: log exception
            }

            result.Add("hasVehicleInfo", "0");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult _getVehicleInfoComboList()
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);
            Dictionary<string, string> result = new Dictionary<string, string>();

            try
            {

                DataSet ds = new DataSet();
                StringReader strrXML = null;

                // Fill Vehicle Type

                string xml = "";
                ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();                
                if (dbs.GetAllVehicleTypes(user.UserId, user.SecId, ref xml) != 0)
                    if (dbs.GetAllVehicleTypes(user.UserId, user.SecId, ref xml) != 0)
                    {
                        //result.Add("hasVehicleVehicelType", "0");                        
                    }

                if (xml == "")
                {
                    result.Add("hasVehicleVehicelType", "0");                    
                }
                else
                {
                    strrXML = new StringReader(xml);
                    ds.ReadXml(strrXML);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        result.Add("hasVehicleVehicelType", "1");
                        string vehicletypeoptions = "";
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            vehicletypeoptions += "<option value=\"" + dr["VehicleTypeId"].ToString() + "\">" + dr["VehicleTypeName"].ToString().TrimEnd() + "</option>";
                        }
                        result.Add("vehicletypeoptions", vehicletypeoptions);
                    }
                    else
                    {
                        result.Add("hasVehicleVehicelType", "0");
                    }
                }

                // fill Make

                xml = "";

                if (dbs.GetAllMakesInfo(user.UserId, user.SecId, ref xml) != 0)
                    if (dbs.GetAllMakesInfo(user.UserId, user.SecId, ref xml) != 0)
                    {
                        //result.Add("hasMake", "0");    
                    }
                if (xml == "")
                {
                    result.Add("hasMake", "0");
                }
                else
                {
                    if (user.DefaultLanguage == "fr-CA")
                        xml = xml.Replace("Not Listed", "Pas dans la liste");

                    ds = new DataSet();


                    strrXML = null;
                    strrXML = new StringReader(xml);
                    ds.ReadXml(strrXML);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        result.Add("hasMake", "1");
                        string makeoptions = "";
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            makeoptions += "<option value=\"" + dr["MakeId"].ToString() + "\">" + dr["MakeName"].ToString().TrimEnd() + "</option>";
                        }
                        result.Add("makeoptions", makeoptions);
                    }
                    else
                    {
                        result.Add("hasMake", "0");
                    }
                    
                }

                // Fill State Province

                xml = "";

                if (dbs.GetAllStateProvinces(user.UserId, user.SecId, ref xml) != 0)
                    if (dbs.GetAllStateProvinces(user.UserId, user.SecId, ref xml) != 0)
                    {
                        //result.Add("hasProvince", "0");
                    }
                if (xml == "")
                {
                    result.Add("hasProvince", "0");
                }
                else
                {
                    ds = new DataSet();


                    strrXML = null;
                    strrXML = new StringReader(xml);
                    ds.ReadXml(strrXML);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        result.Add("hasProvince", "1");
                        string provinceoptions = "";
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            provinceoptions += "<option value=\"" + dr["StateProvince"].ToString() + "\">" + dr["StateProvince"].ToString().TrimEnd() + "</option>";
                        }
                        result.Add("provinceoptions", provinceoptions);
                    }
                    else
                    {
                        result.Add("hasProvince", "0");
                    }

                }

                // Fill All Unassigned BoxIds
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                if (dbo.GetOrganizationAllUnassignedBoxIdsXml(user.UserId, user.SecId, user.OrganizationId, ref xml) != 0)
                    if (dbo.GetOrganizationAllUnassignedBoxIdsXml(user.UserId, user.SecId, user.OrganizationId, ref xml) != 0)
                    {
                        //result.Add("hasUnassignedBoxId", "0");
                    }
                if (xml == "")
                {
                    result.Add("hasUnassignedBoxId", "0");
                }

                else
                {
                    ds = new DataSet();


                    strrXML = null;
                    strrXML = new StringReader(xml);
                    ds.ReadXml(strrXML);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        result.Add("hasUnassignedBoxId", "1");
                        result.Add("UnassignedBoxIds", ds.GetXml());
                        string unassignedBoxIdOptions = "";
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            unassignedBoxIdOptions += "<option value=\"" + dr["BoxId"].ToString() + "\">" + dr["BoxId"].ToString().TrimEnd() + "</option>";
                        }
                        result.Add("unassignedBoxIdOptions", unassignedBoxIdOptions);
                    }
                    else
                    {
                        result.Add("hasUnassignedBoxId", "0");
                    }

                }

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {
                //TODO: log exception
                result.Add("msg", Ex.Message);
            }

            if (!result.ContainsKey("hasVehicleVehicelType"))
            {
                result.Add("hasVehicleVehicelType", "0");
            }
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult _getModelByMakeId(int makeId)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);
            Dictionary<string, string> result = new Dictionary<string, string>();

            try
            {

                DataSet ds = new DataSet();
                StringReader strrXML = null;

                string xml = "";

                ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();         

                if (dbs.GetModelsInfoByMakeId(user.UserId, user.SecId, makeId, ref xml) != 0)
                    if (dbs.GetModelsInfoByMakeId(user.UserId, user.SecId, makeId, ref xml) != 0)
                    {
                        result.Add("hasModelList", "0");    
                    }
                if (xml == "")
                {
                    result.Add("hasModelList", "0");
                }
                else
                {
                    if (user.DefaultLanguage == "fr-CA")
                        xml = xml.Replace("Not Listed", "Pas dans la liste");

                    ds = new DataSet();


                    strrXML = null;
                    strrXML = new StringReader(xml);
                    ds.ReadXml(strrXML);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        result.Add("hasModelList", "1");
                        string modeloptions = "";
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            modeloptions += "<option value=\"" + dr["MakeModelId"].ToString() + "\">" + dr["ModelName"].ToString().TrimEnd() + "</option>";
                        }
                        result.Add("modeloptions", modeloptions);
                    }
                    else
                    {
                        result.Add("hasModelList", "0");
                    }
                }

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {
                //TODO: log exception
            }

            result.Add("hasVehicleVehicelType", "0");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult _saveVehicleInfo(VehicleInfoModel model)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }

            Dictionary<string, string> result = new Dictionary<string, string>();

            try
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);

                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                model.vehicleInfoColor = model.vehicleInfoColor ?? "";
                model.vehicleInfoPhone = model.vehicleInfoPhone ?? "";
                model.vehicleInfoProvince = model.vehicleInfoProvince ?? "";
                model.vehicleInfoEmail = model.vehicleInfoEmail ?? "";

                bool DayLightSaving = Convert.ToBoolean(IsDayLightSaving(Convert.ToBoolean(model.vehicleInfoAutoAdjustDayLightSaving)));

                if (dbv.UpdateVehicleInfo(user.UserId, user.SecId, model.vehicleInfoOldLicense, model.vehicleInfoLicensePlate.Trim(), model.vehicleInfoColor, Convert.ToDouble(model.vehicleInfoCost), model.vehicleInfoDescription, Convert.ToInt32(model.selVehiceInfoModel), Convert.ToInt16(model.vehicleInfoYear), model.vehicleInfoProvince, Convert.ToInt16(model.selVehiceInfoVehicleType), model.vehicleInfoVIN, Convert.ToInt64(model.vehicleInfoVehicleId), Convert.ToInt32(model.vehicleInfoBoxId), Convert.ToInt32(model.selVehiceInfoBox), Convert.ToInt16(model.vehicleInfoIconTypeId), model.vehicleInfoEmail, model.vehicleInfoPhone, Convert.ToInt32(model.vehicleInfoTimeZone), DayLightSaving, Convert.ToInt16(model.vehicleInfoFuelType), Convert.ToBoolean(model.vehicleInfoNotify), Convert.ToBoolean(model.vehicleInfoWarning), Convert.ToBoolean(model.vehicleInfoCritical), Convert.ToBoolean(model.vehicleInfoAutoAdjustDayLightSaving), Convert.ToBoolean(model.vehicleInfoMaintenance), Convert.ToInt32(model.vehicleInfoServiceConfigID),"") != 0)
                    if (dbv.UpdateVehicleInfo(user.UserId, user.SecId, model.vehicleInfoOldLicense, model.vehicleInfoLicensePlate.Trim(), model.vehicleInfoColor, Convert.ToDouble(model.vehicleInfoCost), model.vehicleInfoDescription, Convert.ToInt32(model.selVehiceInfoModel), Convert.ToInt16(model.vehicleInfoYear), model.vehicleInfoProvince, Convert.ToInt16(model.selVehiceInfoVehicleType), model.vehicleInfoVIN, Convert.ToInt64(model.vehicleInfoVehicleId), Convert.ToInt32(model.vehicleInfoBoxId), Convert.ToInt32(model.selVehiceInfoBox), Convert.ToInt16(model.vehicleInfoIconTypeId), model.vehicleInfoEmail, model.vehicleInfoPhone, Convert.ToInt32(model.vehicleInfoTimeZone), DayLightSaving, Convert.ToInt16(model.vehicleInfoFuelType), Convert.ToBoolean(model.vehicleInfoNotify), Convert.ToBoolean(model.vehicleInfoWarning), Convert.ToBoolean(model.vehicleInfoCritical), Convert.ToBoolean(model.vehicleInfoAutoAdjustDayLightSaving), Convert.ToBoolean(model.vehicleInfoMaintenance), Convert.ToInt32(model.vehicleInfoServiceConfigID),"") != 0)
                    {
                        result.Add("success", "0");
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                

                result.Add("success", "1");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                result.Add("success", "0");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult _sendEmail(EmailModel model)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }

            Dictionary<string, string> result = new Dictionary<string, string>();

            try
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);

                if (String.IsNullOrEmpty(model.sendEmailSubject)) model.sendEmailSubject = "";
                
                try
                {
                    SetSMTP(false);
                    
                    MailMessage message = new MailMessage();
                    MailAddress fromMail = new MailAddress("noreply@bsmwireless.com");
                    message.From = fromMail;
                    foreach (var address in model.sendEmailTRecipiants.Split(new[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        message.To.Add(address);
                    }
                    message.Subject = model.sendEmailSubject;
                    message.Body = "Installer: " + model.sendEmailInstaller + "\nEmail: " + model.sendEmailInstallerEmail + "\n" + model.sendEmailBody;
                    
                    mailClient.Send(message);

                    if (message != null) message.Dispose();
                    
                }
                catch (SmtpFailedRecipientException sfre)
                {
                    result.Add("success", "0");
                    result.Add("msg", "SendMail : Error delivering: " + sfre.Message);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (SmtpException e)
                {
                    switch (e.StatusCode)
                    {
                        case SmtpStatusCode.ExceededStorageAllocation:
                            result.Add("success", "0");
                            result.Add("msg", "Mail Attachment file size exceeded its limit");
                            return Json(result, JsonRequestBehavior.AllowGet);                            
                        default:
                            result.Add("success", "0");
                            result.Add("msg", "SendMail : Error delivering: " + e.Message);
                            return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    
                }
                catch (FormatException fe)
                {
                    result.Add("success", "0");
                    result.Add("msg", "SendMail : Error delivering: " + fe.Message);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (ArgumentException ae)
                {
                    result.Add("success", "0");
                    result.Add("msg", "SendMail : Error delivering: " + ae.Message);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    result.Add("success", "0");
                    result.Add("msg", "SendMail : Error delivering: " + e.Message);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                
                result.Add("success", "1");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                result.Add("success", "0");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult _checkSetSeatBeltOdometer(string make, string model, string year)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }

            Dictionary<string, string> result = new Dictionary<string, string>();

            if (year.Length != 4)
            {
                result.Add("found", "0");
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            try
            {
                string fileName = Server.MapPath("~/App_Data/") + "Ford.xls";
                DataTable dtOHData = ReadDataFromExcelUsingNPOI(fileName);

                DataRow[] foundRows = dtOHData.Select("Make = '" + make + "' AND '" + model + "' LIKE '%' + Model + '%' AND Year LIKE '%" + year + "%'");
                if(foundRows.Length > 0)
                    result.Add("found", "1");
                else
                    result.Add("found", "0");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                result.Add("found", "0");
                result.Add("errorMsg", ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
       
        [Authorize]
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ViewResult _getVehicleSensors(int boxId, string licensePlate)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }
            
            DataTable dtSensorsVehicle = GetAllSensorsForVehicle(user, boxId, true);

            if (dtSensorsVehicle == null)
            {
                return null;
            }

            if (dtSensorsVehicle != null)
            {
                List<string> defaultsensorsUP = new List<string>();
                defaultsensorsUP.Add("Ignition");
                defaultsensorsUP.Add("Main Battery");
                defaultsensorsUP.Add("Main Batterie");
                defaultsensorsUP.Add("Main Power");
                defaultsensorsUP.Add("Main Box Power");                
                //defaultsensorsUP.Add("Seat Belt");
                //defaultsensorsUP.Add("Seatbelt");
                defaultsensorsUP.Add("ECM Cable");
                defaultsensorsUP.Add("Alert-Power");
                defaultsensorsUP.Add("Alert Power");
                defaultsensorsUP.Add("Alert - Power");

                List<string> defaultsensorsBNSF = new List<string>();
                defaultsensorsUP.Add("Ignition");
                defaultsensorsUP.Add("Main Battery");
                defaultsensorsUP.Add("Main Batterie");
                defaultsensorsUP.Add("Main Power");
                defaultsensorsUP.Add("Main Box Power");
                //defaultsensorsUP.Add("Seat Belt");
                //defaultsensorsUP.Add("Seatbelt");
                defaultsensorsUP.Add("ECM Cable");
                defaultsensorsUP.Add("Alert-Power");
                defaultsensorsUP.Add("Alert Power");
                defaultsensorsUP.Add("Alert - Power");

                dtSensorsVehicle.Columns.Add("ISDefault", typeof(bool));
                dtSensorsVehicle.Columns.Add("ISInUse", typeof(bool));
                foreach (DataRow row in dtSensorsVehicle.Rows)
                {
                    if (user.OrganizationId == 951)
                    {
                        if (defaultsensorsUP.Contains(row["SensorName"].ToString()) && !row["SensorName"].ToString().StartsWith("unused", true, null))
                            row["ISDefault"] = true;
                        else
                            row["ISDefault"] = false;
                    }
                    if (user.OrganizationId == 1000148)
                    {
                        if (defaultsensorsUP.Contains(row["SensorName"].ToString()) && !row["SensorName"].ToString().StartsWith("unused", true, null))
                            row["ISDefault"] = true;
                        else
                            row["ISDefault"] = false;
                    }
                    if (user.OrganizationId == 999994)
                    {
                        if (defaultsensorsBNSF.Contains(row["SensorId"].ToString()))
                            row["ISDefault"] = true;
                        else
                            row["ISDefault"] = false;
                    }
                    if (row["SensorName"].ToString().StartsWith("unused", true, null))
                        row["ISInUse"] = false;
                    else
                        row["ISInUse"] = true;
                }
            }
             IList<VehicleSensor> sensors = SentinelMobile.Models.SentinelFM.GetSensorsList(dtSensorsVehicle);

            var viewModel = new VehicleSensors
            {
                SensorList = sensors,
                LicensePlate = licensePlate,
                BoxId = boxId
            };

            return View("_sensorList", viewModel); //  First Parameter = Name of the model. _vehicleList, for example
        }

        [Authorize]
        public int _saveSensorInfo(string selected, string unselected, string selectedName, string unselectedName, string LicensePlate)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return -1;
            }
            try
            {
             ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            int i = 0;
            //  INSERT
            foreach (string s in selected.Split(','))
            {
                 string id = s.Replace("[", "").Replace("]", "").Replace("\"", "");
                if (String.IsNullOrEmpty(id))
                    continue;
                 string name = selectedName.Split(',')[i].Replace("[", "").Replace("]", "").Replace("\"","");
                if (String.IsNullOrEmpty(name))
                    continue;
                if (dbv.UpdateSensorNameByLicencePlate(user.UserId, LicensePlate, Convert.ToInt16(id), name) != 0)
                    if (dbv.UpdateSensorNameByLicencePlate(user.UserId, LicensePlate, Convert.ToInt16(id), name) != 0)
                    {
                        //TODO: log exception
                    }
                i++;
            }
            i = 0;
            //  DELETE
            foreach (string s in unselected.Split(','))
            {
                string id = s.Replace("[", "").Replace("]", "").Replace("\"", "");
                if (String.IsNullOrEmpty(id))
                    continue;
                string name = unselectedName.Split(',')[i].Replace("[", "").Replace("]", "").Replace("\"", "");
                if (String.IsNullOrEmpty(name))
                    continue;
                if (dbv.UpdateSensorNameByLicencePlate(user.UserId, LicensePlate, Convert.ToInt16(id), name) != 0)
                    if (dbv.UpdateSensorNameByLicencePlate(user.UserId, LicensePlate, Convert.ToInt16(id), name) != 0)
                    {
                        //TODO: log exception
                    }
                i++;
            }


          
           
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);

                //ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                //sensor.SensorName = sensor.SensorName ?? "";
                //sensor.SensorAction = sensor.SensorAction ?? "";

                //if (dbv.UpdateSensorByLicencePlate(user.UserId, user.SecId, licensePlate, Convert.ToInt16(sensor.SensorId), sensor.SensorName, sensor.SensorAction, Convert.ToInt16(sensor.AlarmLevelOn), Convert.ToInt16(sensor.AlarmLevelOff)) != 0)
                //    if (dbv.UpdateSensorByLicencePlate(user.UserId, user.SecId, licensePlate, Convert.ToInt16(sensor.SensorId), sensor.SensorName, sensor.SensorAction, Convert.ToInt16(sensor.AlarmLevelOn), Convert.ToInt16(sensor.AlarmLevelOff)) != 0)
                //    {
                //        return -1;
                //    }

                return 0;

            }
            catch
            {
                return -1;
            }
        }


        [Authorize]
        public ActionResult _getVehicleEditableSensors(int boxId, string licensePlate)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }
            DataTable dtSensorsVehicle = GetAllSensorsForVehicle(user, boxId, true);
            Dictionary<string, string> editableSensors = new Dictionary<string, string>();                

            if (dtSensorsVehicle == null)
            {
                return null;
            }

            if (dtSensorsVehicle != null)
            {
                List<string> defaultsensors = new List<string>();
                defaultsensors.Add("Ignition");
                defaultsensors.Add("Main Battery");
                defaultsensors.Add("Main Batterie");
                defaultsensors.Add("Main Power");
                defaultsensors.Add("Main Box Power");
                //defaultsensors.Add("Seat Belt");
                //defaultsensors.Add("Seatbelt");
                defaultsensors.Add("ECM Cable");
                defaultsensors.Add("Alert-Power");
                defaultsensors.Add("Alert Power");
                defaultsensors.Add("Alert - Power");

                foreach (DataRow row in dtSensorsVehicle.Rows)
                {

                    if (!defaultsensors.Contains(row["SensorName"].ToString()))
                        if(!row["SensorName"].ToString().StartsWith("unused", true, null))
                        editableSensors.Add(row["SensorId"].ToString(),row["SensorName"].ToString());      
                }
            }
            return Json(editableSensors, JsonRequestBehavior.AllowGet);
        }

        private Dictionary<string, object> _getHistoryRecords(long vehicleId, string dtFrom, string dtTo, User user)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);

                ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();

                string xml = string.Empty;
                bool RequestOverflowed = false;

                int r = dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended(user.UserId, user.SecId, vehicleId, dtFrom, dtTo, true, true, true, true, -1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, "", "", ref xml, ref RequestOverflowed);
                

                if (RequestOverflowed || xml == string.Empty)
                {
                    result.Add("success", "0");
                    result.Add("hasData", "0");

                }
                else
                {

                    DataSet dsLastKnown = new DataSet();

                    StringReader strrXML = new StringReader(xml);
                    dsLastKnown.ReadXml(strrXML);

                    result.Add("success", "1");
                    result.Add("hasData", "1");

                    List<Dictionary<string, string>> datalist = new List<Dictionary<string, string>>();
                    foreach (DataRow dr in dsLastKnown.Tables[0].Rows)
                    {
                        Dictionary<string, string> _h = new Dictionary<string, string>();

                        string originDateTime = string.Empty;
                        originDateTime = Convert.ToDateTime(dr["OriginDateTime"].ToString()).ToString();

                        string displayDateTime = string.Empty;
                        if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
                            displayDateTime = Convert.ToDateTime(dr["OriginDateTime"].ToString()).ToString("dd/MM/yyyy HH:mm:ss");
                        else
                            displayDateTime = Convert.ToDateTime(dr["OriginDateTime"].ToString()).ToString("MM/dd/yyyy hh:mm:ss tt");

                        double speed = -1;
                        try
                        {
                            speed = double.Parse(dr["Speed"].ToString());
                        }
                        catch { }

                        string customSpeed = "n/a";
                        if (speed >= 0)
                            customSpeed = String.Format("{0:0}", speed * user.UnitOfMes) + " " + (user.UnitOfMes < 0.9 ? SentinelMobile.Resources.Resources.SpeedMile : SentinelMobile.Resources.Resources.SpeedKm);

                        _h.Add("originDateTime", originDateTime);
                        _h.Add("displayDateTime", displayDateTime);
                        _h.Add("Speed", customSpeed);
                        _h.Add("Address", dr["StreetAddress"].ToString().Replace("Non GPS Data", Resources.Resources.NonGPSData));
                        _h.Add("Lon", dr["Longitude"].ToString());
                        _h.Add("Lat", dr["Latitude"].ToString());
                        _h.Add("ValidGps", dr["ValidGps"]==null?"-1":dr["ValidGps"].ToString());

                        if (speed > 0)
                        {
                            string MyHeading = SentinelMobile.Models.SentinelFM.Heading(dr["Heading"].ToString());
                            _h.Add("MyHeading", MyHeading.Replace("West", SentinelMobile.Resources.Resources.West).Replace("NW", SentinelMobile.Resources.Resources.NW).Replace("SW", SentinelMobile.Resources.Resources.SW).Replace("W", SentinelMobile.Resources.Resources.DirectionW));
                            _h.Add("MyHeadingIcon", MyHeading);
                            _h.Add("icon", "followBlue" + MyHeading + ".png");
                        }
                        else
                        {
                            _h.Add("MyHeading", "");
                            _h.Add("MyHeadingIcon", "");
                            _h.Add("icon", "followBrown.png");
                        }
                        _h.Add("Status", "");
                        _h.Add("MsgDetails", dr["MsgDetails"].ToString().Trim());

                        _h.Add("CustomProp", dr["CustomProp"].ToString());

                        string reeferData = "0";

                        if (user.OrganizationId == 1000173) // Add reefer data for Sobeys
                        {
                            if (dr["CustomProp"].ToString().Contains("RD_ZONE1") && (new List<int>(new int[] { 1, 9, 114, 2, 73 })).Contains(Convert.ToInt16(dr["BoxMsgInTypeId"])))
                            {
                                reeferData = "1";
                                string ps = string.Empty;
                                string tether = string.Empty;
                                string powerSensorMask = string.Empty;
                                int psensormask = 0;
                                int.TryParse(dr["SensorMask"].ToString(), out psensormask);

                                if ((psensormask & 4) >> 2 == 1)
                                    powerSensorMask = "On";
                                else
                                    powerSensorMask = "Off";


                                
                                string _RD_STA = FindValueFromPair("RD_STA", dr["CustomProp"].ToString(), ";", "=");
                                int RD_STA = 0;
                                int.TryParse(_RD_STA, out RD_STA);

                                string RD_ZONE1 = string.Empty;
                                RD_ZONE1 = FindValueFromPair("RD_ZONE1", dr["CustomProp"].ToString(), ";", "=");
                                string RD_ZONE2 = string.Empty;
                                RD_ZONE2 = FindValueFromPair("RD_ZONE2", dr["CustomProp"].ToString(), ";", "=");
                                string RD_ZONE3 = string.Empty;
                                RD_ZONE3 = FindValueFromPair("RD_ZONE3", dr["CustomProp"].ToString(), ";", "=");

                                string RD_ZONE1_OM = FindValueFromPair("OM", RD_ZONE1, ",", ":");
                                int intRD_ZONE1_OM = 0;
                                int.TryParse(RD_ZONE1_OM, out intRD_ZONE1_OM);

                                string RD_ZONE2_OM = FindValueFromPair("OM", RD_ZONE2, ",", ":");
                                int intRD_ZONE2_OM = 0;
                                int.TryParse(RD_ZONE2_OM, out intRD_ZONE2_OM);

                                string RD_ZONE3_OM = FindValueFromPair("OM", RD_ZONE3, ",", ":");
                                int intRD_ZONE3_OM = 0;
                                int.TryParse(RD_ZONE3_OM, out intRD_ZONE3_OM);

                                string ControllerType = string.Empty;

                                ps = FindValueFromPair("Power", dr["CustomProp"].ToString(), ";", "=");
                                if (ps.ToLower() == "sleepmode")
                                {
                                    tether = "Off";
                                }
                                else
                                {
                                    tether = "Off";
                                    if (((RD_STA & 16) >> 4) == 1)
                                        tether = "On";

                                }

                                string v = string.Empty;
                                double dv = 0;

                                string power = "-";
                                string reeferState = "-";
                                string reeferState2 = "-";
                                string reeferState3 = "-";
                                string ModeOfOp = "-";
                                string ModeOfOp2 = "-";
                                string ModeOfOp3 = "-";
                                string Door = "-";
                                string AFAX = "-";
                                string Setpt = "-";
                                string Ret = "-";
                                string Dis = "-";
                                string Amb = "-";
                                string SensorProbe = "-";
                                string Setpt2 = "-";
                                string Ret2 = "-";
                                string Dis2 = "-";
                                string SensorProbe2 = "-";
                                string Setpt3 = "-";
                                string Ret3 = "-";
                                string Dis3 = "-";
                                string SensorProbe3 = "-";
                                string SpareTemp = "-";
                                string FuelLevel = "-";
                                string EngineHours = "-";
                                string ControllerTypeName = "-";
                                string RPM = "-";
                                string BatteryVolt = "-";

                                v = FindValueFromPair("RD_STA", dr["CustomProp"].ToString(), ";", "="); //getValueByKey('RD_STA', value);
                                if (v.Trim() != string.Empty)
                                {
                                    if ((int.Parse(v) & 15) == 1)
                                    {    // Device Type = 1 (TK)              
                                        ControllerType = FindValueFromPair("RD_CT", dr["CustomProp"].ToString(), ";", "=");
                                    }
                                }

                                if (tether == "Off")
                                {
                                    //power = "-";
                                }
                                else
                                {

                                    v = FindValueFromPair("RD_STA", dr["CustomProp"].ToString(), ";", "="); //getValueByKey('RD_STA', value);

                                    if ((RD_STA & 15) == 1)
                                    {    // Device Type = 1 (TK)              

                                        # region Power, RPM
                                        if (",9,11,12,13,14,15,16,17,19,20,".IndexOf("," + ControllerType + ",") >= 0)
                                        {
                                            if ((RD_STA & 64) > 0)
                                                power = "On";
                                            else
                                                power = "Off";

                                            RPM = FindValueFromPair("RD_RPM", dr["CustomProp"].ToString(), ";", "=");
                                            if (RPM.Trim() == "32767") RPM = "";
                                        }
                                        # endregion
                                    }

                                    if (powerSensorMask != "Off")
                                    {


                                        if (RD_ZONE1 != "")
                                        {

                                            if (RD_ZONE1_OM != "")
                                            {

                                                # region ReeferState
                                                reeferState = getReeferState(intRD_ZONE1_OM);
                                                # endregion

                                                # region ModeOfOp
                                                if (((intRD_ZONE1_OM & 16) >> 4) == 1)
                                                    ModeOfOp = "Continuous";
                                                else
                                                    ModeOfOp = "Cycle Sentry";
                                                # endregion

                                                # region Door
                                                if (((intRD_ZONE1_OM & 4) >> 2) == 1)
                                                    Door = "Open";
                                                else
                                                    Door = "Closed";
                                                # endregion

                                                # region AFAX
                                                if (",14,15,16,17,19,20,21,22,".IndexOf("," + ControllerType + ",") >= 0)
                                                {

                                                    if ((intRD_ZONE1_OM & 1) == 1)
                                                        AFAX = "Open";
                                                    else
                                                        AFAX = "Closed";

                                                }
                                                # endregion
                                            }

                                            # region Setpt.
                                            v = FindValueFromPair("TSP", RD_ZONE1, ",", ":");
                                            if (v.Trim() != "")
                                                Setpt = String.Format("{0:0}", double.Parse(v));
                                            # endregion

                                            # region Ret
                                            v = FindValueFromPair("RAT1", RD_ZONE1, ",", ":");
                                            if (v.Trim() != "")
                                                Ret = String.Format("{0:0.00}", double.Parse(v));
                                            # endregion

                                            # region Dis
                                            v = FindValueFromPair("SDT1", RD_ZONE1, ",", ":");
                                            if (v.Trim() != "")
                                                Dis = String.Format("{0:0.00}", double.Parse(v));
                                            # endregion

                                            # region Sensor Probe
                                            v = FindValueFromPair("ECT", RD_ZONE1, ",", ":");
                                            if (v.Trim() != "")
                                                SensorProbe = String.Format("{0:0.00}", double.Parse(v));
                                            # endregion
                                        }

                                        if (RD_ZONE2 != "")
                                        {
                                            if (RD_ZONE2_OM != "")
                                            {
                                                # region ReeferState
                                                reeferState2 = getReeferState(intRD_ZONE2_OM);
                                                # endregion

                                                # region ModeOfOp
                                                if (((intRD_ZONE2_OM & 16) >> 4) == 1)
                                                    ModeOfOp2 = "Continuous";
                                                else
                                                    ModeOfOp2 = "Cycle Sentry";
                                                # endregion
                                            }

                                            # region Setpt2.
                                            v = FindValueFromPair("TSP", RD_ZONE2, ",", ":");
                                            if (v.Trim() != "")
                                                Setpt2 = String.Format("{0:0}", double.Parse(v));
                                            # endregion

                                            # region Ret2
                                            v = FindValueFromPair("RAT1", RD_ZONE2, ",", ":");
                                            if (v.Trim() != "")
                                                Ret2 = String.Format("{0:0.00}", double.Parse(v));
                                            # endregion

                                            # region Dis2
                                            v = FindValueFromPair("SDT1", RD_ZONE2, ",", ":");
                                            if (v.Trim() != "")
                                                Dis2 = String.Format("{0:0.00}", double.Parse(v));
                                            # endregion

                                            # region Sensor Probe2
                                            v = FindValueFromPair("ECT", RD_ZONE2, ",", ":");
                                            if (v.Trim() != "")
                                                SensorProbe2 = String.Format("{0:0.00}", double.Parse(v));
                                            # endregion
                                        }

                                        if (RD_ZONE3 != "")
                                        {
                                            if (RD_ZONE3_OM != "")
                                            {
                                                # region ReeferState
                                                reeferState3 = getReeferState(intRD_ZONE3_OM);
                                                # endregion

                                                # region ModeOfOp
                                                if (((intRD_ZONE3_OM & 16) >> 4) == 1)
                                                    ModeOfOp3 = "Continuous";
                                                else
                                                    ModeOfOp3 = "Cycle Sentry";
                                                # endregion
                                            }

                                            # region Setpt3.
                                            v = FindValueFromPair("TSP", RD_ZONE3, ",", ":");
                                            if (v.Trim() != "")
                                                Setpt3 = String.Format("{0:0}", double.Parse(v));
                                            # endregion

                                            # region Ret3
                                            v = FindValueFromPair("RAT1", RD_ZONE3, ",", ":");
                                            if (v.Trim() != "")
                                                Ret3 = String.Format("{0:0.00}", double.Parse(v));
                                            # endregion

                                            # region Dis3
                                            v = FindValueFromPair("SDT1", RD_ZONE3, ",", ":");
                                            if (v.Trim() != "")
                                                Dis3 = String.Format("{0:0.00}", double.Parse(v));
                                            # endregion

                                            # region Sensor Probe3
                                            v = FindValueFromPair("ECT", RD_ZONE3, ",", ":");
                                            if (v.Trim() != "")
                                                SensorProbe3 = String.Format("{0:0.00}", double.Parse(v));
                                            # endregion
                                        }

                                        # region Amb
                                        string _amt = FindValueFromPair("RD_AMT", dr["CustomProp"].ToString(), ";", "=");
                                        if (_amt.Trim() != "")
                                        {
                                            Amb = String.Format("{0:0}", double.Parse(_amt));
                                        }
                                        # endregion

                                        # region Spare Temperature of Sensor #1
                                        v = FindValueFromPair("RD_STS1", dr["CustomProp"].ToString(), ";", "=");
                                        if (v != "")
                                        {
                                            dv = 0;
                                            double.TryParse(v, out dv);
                                            SpareTemp = String.Format("{0:0.00}", dv);
                                        }
                                        # endregion

                                    }
                                    else
                                    {
                                        RPM = "-";
                                    }
                                }

                                v = FindValueFromPair("RD_STA", dr["CustomProp"].ToString(), ";", "="); //getValueByKey('RD_STA', value);

                                # region Controller Type
                                if (ControllerType != string.Empty)
                                {
                                    ControllerTypeName = getControllerTypeById(int.Parse(ControllerType));
                                }
                                # endregion

                                # region Fuel Level
                                v = FindValueFromPair("RD_FUEL", dr["CustomProp"].ToString(), ";", "=");
                                if (v != "")
                                {
                                    dv = 300;
                                    double.TryParse(v, out dv);
                                    dv = dv * 1;
                                    if (dv <= 100)
                                        FuelLevel = String.Format("{0:0}", dv);
                                    else
                                        FuelLevel = "Invalid";
                                }
                                # endregion

                                # region Engine Hours
                                v = FindValueFromPair("RD_EH", dr["CustomProp"].ToString(), ";", "=");
                                if (v != "")
                                {
                                    dv = 0;
                                    double.TryParse(v, out dv);
                                    EngineHours = String.Format("{0:0}", dv);
                                }
                                # endregion

                                # region Battery Voltage
                                v = FindValueFromPair("RD_BATTERY", dr["CustomProp"].ToString(), ";", "=");
                                if (v.Trim() != string.Empty)
                                {
                                    dv = double.Parse(v);
                                    BatteryVolt = String.Format("{0:0.00}", dv);
                                }
                                # endregion

                                /*rowItem["Tether"] = tether;
                                rowItem["Power"] = "<span style='color:" + (powerSensorMask == power ? "#000000" : "#666666") + ";'>" + powerSensorMask + "</span>";
                                rowItem["ReeferState"] = reeferState;
                                rowItem["ModeOfOp"] = ModeOfOp;
                                rowItem["Door"] = Door;
                                rowItem["AFAX"] = AFAX;
                                rowItem["Setpt"] = Setpt;
                                rowItem["Ret"] = Ret;
                                rowItem["Dis"] = Dis;
                                rowItem["Amb"] = Amb;
                                rowItem["SensorProbe"] = SensorProbe;
                                rowItem["Setpt2"] = Setpt2;
                                rowItem["Ret2"] = Ret2;
                                rowItem["Dis2"] = Dis2;
                                rowItem["SensorProbe2"] = SensorProbe2;
                                rowItem["Setpt3"] = Setpt3;
                                rowItem["Ret3"] = Ret3;
                                rowItem["Dis3"] = Dis3;
                                rowItem["SensorProbe3"] = SensorProbe3;
                                rowItem["SpareTemp"] = SpareTemp;
                                rowItem["FuelLevel"] = FuelLevel;
                                rowItem["EngineHours"] = EngineHours;
                                rowItem["ControllerType"] = ControllerTypeName;
                                rowItem["RPM"] = RPM;
                                rowItem["BatteryVolt"] = BatteryVolt;
                                 */

                                _h.Add("Tether", tether);
                                _h.Add("Power", powerSensorMask);
                                _h.Add("Amb", Amb);
                                _h.Add("BatteryVolt", BatteryVolt);
                                _h.Add("Setpt", Setpt);
                                _h.Add("Ret", Ret);                                
                                _h.Add("reeferState", reeferState);
                                _h.Add("ModeOfOp", ModeOfOp);
                                _h.Add("Setpt2", Setpt2);
                                _h.Add("Ret2", Ret2);
                                _h.Add("reeferState2", reeferState2);
                                _h.Add("ModeOfOp2", ModeOfOp2);
                                _h.Add("Setpt3", Setpt3);
                                _h.Add("Ret3", Ret3);
                                _h.Add("reeferState3", reeferState3);
                                _h.Add("ModeOfOp3", ModeOfOp3);                                
                            }
                        }

                        _h.Add("reeferData", reeferData);

                        datalist.Add(_h);
                    }
                    result.Add("data", datalist.ToArray());
                    //result.Add("lon", Convert.ToString(lon));
                    //result.Add("Address", rsAddress);
                    //var oSerializer = new JavaScriptSerializer();
                    //string json = oSerializer.Serialize(result);
                }
                result.Add("Status", "200");
            }
            catch (Exception e)
            {
                result.Add("Status", "500");
            }

            return result;
        }

        private string GetBoxStatus(bool isExtendedVersion, string LicensePlate)
        {
            try
            {
                User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
                if (user == null)
                {
                    return "";
                }

                DataSet dsStatus = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (dbv.GetLastBoxStatusFromHistoryByLicensePlateExtended(user.UserId, user.SecId, LicensePlate, ref xml, isExtendedVersion) != 0)
                    if (dbv.GetLastBoxStatusFromHistoryByLicensePlateExtended(user.UserId, user.SecId, LicensePlate, ref xml, isExtendedVersion) != 0)
                    {
                        return "";
                    }


                if (xml == "")
                {
                    return "";
                }

                strrXML = new StringReader(xml);
                dsStatus.ReadXml(strrXML);

                string statusInfo = "<table>";

                string CustomProp = dsStatus.Tables[0].Rows[0]["CustomProp"].ToString();

                string StatusWaypoint = string.Empty;
                if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyWaypointsUsage, CustomProp) == "")
                    StatusWaypoint = "0 %";
                else
                    StatusWaypoint = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyWaypointsUsage, CustomProp);

                statusInfo += "<tr><td>" + Resources.Resources.Armed + "</td><td>" + dsStatus.Tables[0].Rows[0]["BoxArmed"].ToString() + "</td></tr>";
                statusInfo += "<tr><td>" + Resources.Resources.MainBattery + "</td><td>" + VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyMainBattery, CustomProp) + " " + Resources.Resources.Volt + "</td></tr>";
                statusInfo += "<tr><td>" + Resources.Resources.BackupBattery + "</td><td>" + VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyBackupBattery, CustomProp) + " " + Resources.Resources.Volt + "</td></tr>";
                statusInfo += "<tr><td>" + Resources.Resources.SN + "</td><td>" + VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keySerialNumber, CustomProp) + "</td></tr>";
                statusInfo += "<tr><td>" + Resources.Resources.FirmwareVersion + "</td><td>" + VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyFirmwareVersion, CustomProp) + "</td></tr>";
                statusInfo += "<tr><td>" + Resources.Resources.MemoryUsage + "</td><td>" + StatusWaypoint + "</td></tr>";
                statusInfo += "<tr><td>" + Resources.Resources.MDTmessagesinmemory + "</td><td>" + VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyMDTNumMessages, CustomProp) + "</td></tr>";
                statusInfo += "<tr><td>" + Resources.Resources.SIMESN + "</td><td>" + VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keySIM, CustomProp) + "</td></tr>";
                statusInfo += "<tr><td>" + Resources.Resources.Cell + "</td><td>" + VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyCell, CustomProp) + "</td></tr>";

                if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyPRLNumber, CustomProp) != "")
                {
                    statusInfo += "<tr><td>" + Resources.Resources.PRL + "</td><td>" + VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyPRLNumber, CustomProp) + "</td></tr>";
                }

                statusInfo += "</table>";

                DataSet dsSensors = new DataSet();
                DataTable dtResult = this.GetAllSensorsForVehicle(user, LicensePlate, false);
                

                DataSet dsVehicleInfo = new DataSet();
                strrXML = new StringReader(xml);
                dsVehicleInfo.ReadXml(strrXML);
                string SensorMask = dsStatus.Tables[0].Rows[0]["SensorMask"].ToString();

                if ((dtResult != null) && (dtResult.Rows.Count > 0))
                {
                    DataTable tblSensors = dsSensors.Tables.Add("SensorsInformation");
                    tblSensors.Columns.Add("SensorId", typeof(short));
                    tblSensors.Columns.Add("SensorName", typeof(string));
                    tblSensors.Columns.Add("SensorAction", typeof(string));
                    tblSensors.Columns.Add("SensorStatus", typeof(bool));

                    // move over all sensors and set current status
                    short snsId = 0;
                    int index = 0;
                    object[] objRow = null;
                    string snsAction = "";
                    UInt64 checkBit = 1;
                    foreach (DataRow ittr in dtResult.Rows)
                    {

                        if (!ittr["SensorName"].ToString().TrimEnd().Contains(VLF.CLS.Def.Const.keySensorNotInUse))
                        {
                            snsAction = ittr["SensorAction"].ToString().TrimEnd();
                            index = snsAction.IndexOf("/");
                            if (index < 1)
                            {
                                // wrong sensors format in the database (should be action1/action2)
                                break;
                            }
                            objRow = new object[4];
                            snsId = Convert.ToInt16(ittr["SensorId"]);
                            objRow[0] = snsId;
                            objRow[1] = ittr["SensorName"].ToString().TrimEnd();
                            UInt64 shift = 1;
                            checkBit = shift << (snsId - 1);
                            if ((Convert.ToUInt64(SensorMask) & checkBit) == 0)
                            {
                                objRow[2] = snsAction.Substring(index + 1);
                                objRow[3] = false;
                            }
                            else
                            {
                                objRow[2] = snsAction.Substring(0, index);
                                objRow[3] = true;
                            }
                            tblSensors.Rows.Add(objRow);
                        }
                    }
                }

                dsSensors.Tables["SensorsInformation"].DefaultView.Sort = "SensorId";
                //this.dgBoxStatusInfo.DataSource = dsSensors.Tables["SensorsInformation"];
                //this.dgBoxStatusInfo.DataBind();
                statusInfo += "<table><tr><th>" + Resources.Resources.Sensors + "</th><th>" + Resources.Resources.SensorStatus + "</th></tr>";
                foreach (DataRow dr in dsSensors.Tables["SensorsInformation"].Rows)
                {
                    statusInfo += "<tr><td>" + dr["SensorName"].ToString() + "</td><td>" + dr["SensorAction"].ToString() + "</td></tr>";
                }
                statusInfo += "</table>";

                return statusInfo;
            }
            catch (Exception Ex)
            {
                //TODO: log exception
            }

            return "";
        }

        private string GetBoxPosition(long vehicleId)
        {
            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return "";
            }
            
            Dictionary<string, object> result = new Dictionary<string, object>();
            string statusInfo = "<table>";

            try
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);

                ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();

                string xml = string.Empty;
                bool RequestOverflowed = false;

                string cstart = DateTime.Now.AddHours(-12).ToString("MM/dd/yyyy HH:mm:ss");
                //string cstart = Convert.ToDateTime("2014-05-29 00:00:00").AddHours(-user.TimeZone - user.DayLightSaving).AddSeconds(1).ToString("MM/dd/yyyy HH:mm:ss");
                string cend = DateTime.Now.AddHours(24).ToString("MM/dd/yyyy HH:mm:ss");

                int r = dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended(user.UserId, user.SecId, vehicleId, cstart, cend, true, true, true, true, -1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, "9", "TOP 1", ref xml, ref RequestOverflowed);


                if (RequestOverflowed || xml == string.Empty)
                {
                    statusInfo += "<tr><td>GPS:</td><td>N/A</td>";
                    statusInfo += "<tr><td>Lat Lon:</td><td>N/A</td>";
                    statusInfo += "</table>";
                    return statusInfo;

                }
                else
                {

                    DataSet dsLastKnown = new DataSet();

                    StringReader strrXML = new StringReader(xml);
                    dsLastKnown.ReadXml(strrXML);

                    List<Dictionary<string, string>> datalist = new List<Dictionary<string, string>>();
                    //foreach (DataRow dr in dsLastKnown.Tables[0].Rows)
                    if (dsLastKnown.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = dsLastKnown.Tables[0].Rows[0];
                        Dictionary<string, string> _h = new Dictionary<string, string>();
                        string validgps = dr["ValidGps"] == null ? "-1" : dr["ValidGps"].ToString();
                        string svg = "N/A";
                        if (validgps.Trim() == "0")
                            svg = Resources.Resources.GpsValid;
                        else if (validgps.Trim() != "-1")
                            svg = Resources.Resources.GpsInvalid;
                        statusInfo += "<tr><td>" + svg + "</td>";
                        statusInfo += "<tr><td>Lat Lon: " + dr["Latitude"].ToString() + " " + dr["Longitude"].ToString() + "</td>";

                    }
                    else
                    {
                        statusInfo += "<tr><td>GPS:</td><td>N/A</td>";
                        statusInfo += "<tr><td>Lat Lon:</td><td>N/A</td>";
                    }                    
                }                
            }
            catch (Exception e)
            {
                statusInfo += "<tr><td>Valid GPS:</td><td>N/A</td>";
                statusInfo += "<tr><td>Lat Lon:</td><td>N/A</td>";
            }

            statusInfo += "</table>";
            return statusInfo;
        }

        private DataTable GetAllSensorsForOrganization(User user, string licensePlate, bool getAllSensors)
        {
            DataSet dsSensors = new DataSet("Sensors");
            DataTable dtSensors = new DataTable("BoxSensors");

            string xml = "";

            ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

            if (dbo.GetAllSensorByOrganizationId(user.UserId, user.OrganizationId, ref xml) != 0)
                if (dbo.GetAllSensorByOrganizationId(user.UserId, user.OrganizationId, ref xml) != 0)
                {
                    //TODO: log exception
                }

            if (String.IsNullOrEmpty(xml))
            {
                return null;
            }

            dsSensors.ReadXml(new StringReader(xml));

            dtSensors.Columns.Add("SensorId", typeof(short));
            dtSensors.Columns.Add("SensorName", typeof(string));
            dtSensors.Columns.Add("SensorAction", typeof(string));
            dtSensors.Columns.Add("AlarmLevelOn", typeof(short));
            dtSensors.Columns.Add("AlarmLevelOff", typeof(short));

            if (VLF.CLS.Util.IsDataSetValid(dsSensors))
            {
                foreach (DataRow rowSensor in dsSensors.Tables[0].Rows)
                {
                    if (!getAllSensors)
                        if (Convert.ToInt16(rowSensor["SensorId"]) > VLF.CLS.Def.Enums.ReeferBase)
                            continue;

                    dtSensors.ImportRow(rowSensor);
                }
            }

            return dtSensors;
        }

        private DataTable GetAllSensorsForVehicle(User user, int box, bool getAllSensors)
        {

            DataSet dsSensors = new DataSet("Sensors");
            DataTable dtSensors = new DataTable("BoxSensors");

            string xml = "";

            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            if (dbv.GetVehicleSensorsXMLByBoxId(user.UserId, user.SecId, box, ref xml) != 0)
                if (dbv.GetVehicleSensorsXMLByBoxId(user.UserId, user.SecId, box, ref xml) != 0)
                {
                    //TODO: log exception
                }

            if (String.IsNullOrEmpty(xml))
            {
                return null;
            }

            dsSensors.ReadXml(new StringReader(xml));

            dtSensors.Columns.Add("SensorId", typeof(short));
            dtSensors.Columns.Add("SensorName", typeof(string));
            dtSensors.Columns.Add("SensorAction", typeof(string));
            dtSensors.Columns.Add("AlarmLevelOn", typeof(short));
            dtSensors.Columns.Add("AlarmLevelOff", typeof(short));

            if (VLF.CLS.Util.IsDataSetValid(dsSensors))
            {
                foreach (DataRow rowSensor in dsSensors.Tables[0].Rows)
                {
                    if (!getAllSensors)
                        if (Convert.ToInt16(rowSensor["SensorId"]) > VLF.CLS.Def.Enums.ReeferBase)
                            continue;

                    dtSensors.ImportRow(rowSensor);
                }
            }
            return dtSensors;
        }

        private DataTable GetAllSensorsForVehicle(User user, string licensePlate, bool getAllSensors)
        {
            DataSet dsSensors = new DataSet("Sensors");
            DataTable dtSensors = new DataTable("BoxSensors");
            
            string xml = "";

            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            if (dbv.GetVehicleSensorsXMLByLang(user.UserId, user.SecId, licensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml) != 0)
                if (dbv.GetVehicleSensorsXMLByLang(user.UserId, user.SecId, licensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml) != 0)
                {
                    //TODO: log exception
                }

            if (String.IsNullOrEmpty(xml))
            {
                return null;
            }

            dsSensors.ReadXml(new StringReader(xml));

            dtSensors.Columns.Add("SensorId", typeof(short));
            dtSensors.Columns.Add("SensorName", typeof(string));
            dtSensors.Columns.Add("SensorAction", typeof(string));
            dtSensors.Columns.Add("AlarmLevelOn", typeof(short));
            dtSensors.Columns.Add("AlarmLevelOff", typeof(short));

            if (VLF.CLS.Util.IsDataSetValid(dsSensors))
            {
                foreach (DataRow rowSensor in dsSensors.Tables[0].Rows)
                {
                    if (!getAllSensors)
                        if (Convert.ToInt16(rowSensor["SensorId"]) > VLF.CLS.Def.Enums.ReeferBase)
                            continue;

                    dtSensors.ImportRow(rowSensor);
                }
            }

            return dtSensors;
        }

        private string GenerateFuelEEPROMData(string cmdValue)
        {

            string customProperties = null;
            byte[] Message = new byte[13];
            Message[0] = 0xAB;
            Message[1] = 0xCD;

            string[] ps = cmdValue.Split(';');
            if (ps.Length < 5) return "";

            System.BitConverter.GetBytes(Convert.ToUInt32(ps[4])).CopyTo(Message, 2); // Denominator
            System.BitConverter.GetBytes(Convert.ToUInt16(ps[1])).CopyTo(Message, 6); // Displacement
            System.BitConverter.GetBytes(Convert.ToUInt16(ps[2])).CopyTo(Message, 8); // VolumeEfficency
            System.BitConverter.GetBytes(Convert.ToByte(ps[0])).CopyTo(Message, 10); // FuelType
            System.BitConverter.GetBytes(Convert.ToByte(ps[3])).CopyTo(Message, 11); // AirFuelRatio
            customProperties = VLF.CLS.Util.ByteArrayAsHexDumpToString(Message);
            return customProperties.Substring(1, customProperties.Length - 6);

        }

        private Int16 IsDayLightSaving(bool AutoAdjustDayLightSaving)
        {
            if (AutoAdjustDayLightSaving)
            {
                TimeZone timeZone = System.TimeZone.CurrentTimeZone;
                return Convert.ToInt16(timeZone.IsDaylightSavingTime(DateTime.Now));
            }
            else
            {
                return 0;
            }

        }

        private DataTable ReadDataFromExcelUsingNPOI(string filePath)
        {
            HSSFWorkbook hssfworkbook;

            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            DataTable dt = new DataTable();
            
            if (rows.MoveNext())
            {
                HSSFRow row0 = (HSSFRow)rows.Current;
                for (int i = 0; i < row0.LastCellNum; i++)
                {
                    HSSFCell cell = (HSSFCell)row0.GetCell(i);

                    dt.Columns.Add(cell.ToString());
                    //dr[i] = cell.ToString();

                }
            }

            while (rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                DataRow dr = dt.NewRow();

                for (int i = 0; i < row.LastCellNum; i++)
                {
                    HSSFCell cell = (HSSFCell)row.GetCell(i);

                    dr[i] = (cell == null) ? "" : cell.ToString();

                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// Set up SMTP client
        /// </summary>
        private void SetSMTP(bool pickupIIS)
        {
            // server
            string server = "";
            try
            {
                server = ConfigurationManager.AppSettings["SMTPServer"].ToString();
            }
            catch
            {
                server = "localhost";
            }
            if (String.IsNullOrEmpty(server)) server = "localhost";
            mailClient = new SmtpClient(server);

            // timeout
            int timeout = 120000;
            try
            {
                timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPTimeout"]) * 60000;
            }
            catch
            {
                timeout = 120000;
            }
            mailClient.Timeout = timeout;

            // credentials
            string username = ConfigurationManager.AppSettings["UserName"];
            string password = ConfigurationManager.AppSettings["Pwd"];

            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                //if (String.Compare(server, "localhost", true) == 0 ||
                //   String.Compare(server, "127.0.0.1") == 0 ||
                // String.Compare(server, "192.168.8.59") == 0) 
                mailClient.UseDefaultCredentials = true;
            else
            {
                try
                {
                    NetworkCredential newCredential =
                       new System.Net.NetworkCredential(username, password, server);
                    mailClient.UseDefaultCredentials = false;
                    mailClient.Credentials = newCredential;
                }
                catch
                {
                    mailClient.UseDefaultCredentials = true;
                }
            }

            if (pickupIIS)
                mailClient.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;

            //LogFileWriter.Write(TraceLevel.Verbose, "SMTP :: Domain: [{0}] - User: [{1}]",
            //   ((NetworkCredential)mailClient.Credentials).Domain,
            //   ((NetworkCredential)mailClient.Credentials).UserName);
        }

        private string FindValueFromPair(string key, string src, string seperator, string equalSign)
        {
            try
            {
                int val_start, val_end;
                int key_pos = src.IndexOf(key);

                if (key_pos != -1)
                {
                    val_start = src.IndexOf(equalSign, key_pos);
                    if (key_pos != -1)
                    {
                        val_end = src.IndexOf(seperator, val_start);

                        if (val_end != -1)
                            return src.Substring(val_start + 1, val_end - val_start - 1);
                        else
                            return src.Substring(val_start + 1, src.Length - val_start - 1);
                    }
                }
            }
            catch (Exception exc)
            {

            }
            return "";
        }

        private string getReeferState(int intRD_ZONE1_OM)
        {
            int r = intRD_ZONE1_OM >> 5;
            string reeferState = string.Empty;

            switch (r)
            {
                case 0:
                    reeferState = "Unknown";
                    break;
                case 1:
                    reeferState = "Cooling";
                    break;
                case 2:
                    reeferState = "Heating";
                    break;

                case 3:
                    reeferState = "Defrost";
                    break;
                case 4:
                    reeferState = "Null";
                    break;
                case 5:
                    reeferState = "Pre-trip";
                    break;
                case 6:
                    reeferState = "Sleep";
                    break;
                case 7:
                    reeferState = "N/A";
                    break;
            }

            if (reeferState == "Cooling" || reeferState == "Heating")
            {
                int engineSpeed = intRD_ZONE1_OM & 8; //bit 4, Engine Speed. Only if cooling or heating. 0: low, 1: high
                if (engineSpeed == 0)
                    reeferState = "Low Speed " + reeferState;
                else
                    reeferState = "High Speed " + reeferState;
            }

            return reeferState;
        }

        private string getControllerTypeById(int controllerTypeId)
        {
            string c = "N/A";

            switch (controllerTypeId)
            {
                case 0:
                    c = "Invalid";
                    break;
                case 1:
                    c = "MP4";
                    break;
                case 2:
                    c = "MP5";
                    break;
                case 3:
                    c = "MP6";
                    break;
                case 4:
                    c = "TG6";
                    break;
                case 5:
                    c = "TTMT";
                    break;
                case 6:
                    c = "DAS";
                    break;
                case 7:
                    c = "TCI";
                    break;
                case 8:
                    c = "MPT";
                    break;
                case 9:
                    c = "SR2";
                    break;
                case 10:
                    c = "N/A";
                    break;
                case 11:
                    c = "SR2 M/T";
                    break;
                case 12:
                    c = "SR2 Truck";
                    break;
                case 13:
                    c = "SR2 Truck M/T";
                    break;
                case 14:
                    c = "SR3";
                    break;
                case 15:
                    c = "SR3 MT";
                    break;
                case 16:
                    c = "SR3 ST Truck";
                    break;
                case 17:
                    c = "SR3 MT Truck";
                    break;
                case 18:
                    c = "DAS IV";
                    break;
                case 19:
                    c = "SR4 ST";
                    break;
                case 20:
                    c = "SR4 MT";
                    break;
                case 21:
                    c = "SR4 ST Truck";
                    break;
                case 22:
                    c = "SR4 MT Truck";
                    break;
                case 23:
                    c = "Cryo Trailer";
                    break;
                case 24:
                    c = "Cryo Truck";
                    break;
                default:
                    c = "N/A";
                    break;

            }
            return c;
        }
    }


}
