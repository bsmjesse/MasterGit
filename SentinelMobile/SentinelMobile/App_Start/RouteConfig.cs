using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SentinelMobile
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default_route",
            //    url: "",
            //    //defaults: new { controller = "Home", action = "Index" }
            //    defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            //    //defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
                name: "Login",
                url: "login",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LoginFromWeb",
                url: "LoginFromWeb",
                defaults: new { controller = "Account", action = "LoginFromWeb", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Index", 
                url: "Home/Index/{id}", 
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "vehicleList",
                url: "Home/_vehicleList/{id},{pageIndex},{isSearch},{searchString},{checkedVehicleIds}",
                defaults: new { controller = "Home", action = "_vehicleList", id = UrlParameter.Optional, pageIndex = UrlParameter.Optional, isSearch = UrlParameter.Optional, searchString = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "_fleetList",
                url: "Home/_fleetList/{fleetId},{pageIndex},{isSearch},{searchString}",
                defaults: new { controller = "Home", action = "_fleetList", fleetId = UrlParameter.Optional, pageIndex = UrlParameter.Optional, isSearch = UrlParameter.Optional, searchString = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "_FindMyVehiclesByPosition",
                url: "Home/_FindMyVehiclesByPosition/{lon},{lat},{fleetId}",
                defaults: new { controller = "Home", action = "_FindMyVehiclesByPosition", lon = UrlParameter.Optional, lat = UrlParameter.Optional, fleetId = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "sendCommand",
                url: "Home/_sendCommand/{boxId},{cmdName},{cmdValue},{vehicleId}",
                defaults: new { controller = "Home", action = "_sendCommand", boxId = UrlParameter.Optional, cmdName = UrlParameter.Optional, cmdValue = UrlParameter.Optional, vehicleId = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "checkCommandStatus",
                url: "Home/_checkCommandStatus/{boxId},{ProtocolTypeId},{CommModeId},{vehicleId}",
                defaults: new { controller = "Home", action = "_checkCommandStatus", boxId = UrlParameter.Optional, ProtocolTypeId = UrlParameter.Optional, CommModeId = UrlParameter.Optional, vehicleId = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "updateVehicleDescription",
                url: "Home/_updateVehicleDescription/{vehicleId},{vehicleDescription}",
                defaults: new { controller = "Home", action = "_updateVehicleDescription", vehicleId = UrlParameter.Optional, vehicleDescription = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "getVehicleCommands",
                url: "Home/_getVehicleCommands/{boxId},{licensePlate}",
                defaults: new { controller = "Home", action = "_getVehicleCommands", boxId = UrlParameter.Optional, licensePlate = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "organizationList",
                url: "Account/_organizatoinList/{pageIndex},{isSearch},{searchString}",
                defaults: new { controller = "Account", action = "_organizatoinList", pageIndex = UrlParameter.Optional, isSearch = UrlParameter.Optional, searchString = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "getVehicleInfo",
                url: "Home/_getVehicleInfoByLicensePlate/{licensePlate}",
                defaults: new { controller = "Home", action = "_getVehicleInfoByLicensePlate", licensePlate = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "getVehicleInfoComboList",
                url: "Home/_getVehicleInfoComboList/",
                defaults: new { controller = "Home", action = "_getVehicleInfoComboList" }
            );

            routes.MapRoute(
                name: "saveVehicleInfo",
                url: "Home/_saveVehicleInfo/",
                defaults: new { controller = "Home", action = "_saveVehicleInfo" }
            );

            routes.MapRoute(
                name: "getModelByMakeId",
                url: "Home/_getModelByMakeId/{makeId}",
                defaults: new { controller = "Home", action = "_getModelByMakeId", makeId = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "checkSetSeatBeltOdometer",
                url: "Home/_checkSetSeatBeltOdometer/{make},{model},{year}",
                defaults: new { controller = "Home", action = "_checkSetSeatBeltOdometer", make = UrlParameter.Optional, model = UrlParameter.Optional, year = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "sendEmail",
                url: "Home/_sendEmail/",
                defaults: new { controller = "Home", action = "_sendEmail" }
            );

            routes.MapRoute(
                name: "swapVehicle",
                url: "Home/_swapVehicle/{vehicleId}",
                defaults: new { controller = "Home", action = "_swapVehicle", vehicleId = UrlParameter.Optional }
            );

            routes.MapRoute(
              name: "getBoxSensorInfo",
              url: "Home/_getVehicleSensors/{boxId},{licensePlate}",
              defaults: new { controller = "Home", action = "_getVehicleSensors", boxId = UrlParameter.Optional, licensePlate = UrlParameter.Optional }
            );

            routes.MapRoute(
              name: "getVehicleEditableSensors",
              url: "Home/_getVehicleEditableSensors/{boxId},{licensePlate}",
              defaults: new { controller = "Home", action = "_getVehicleEditableSensors", boxId = UrlParameter.Optional, licensePlate = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "saveSensorInfo",
                url: "Home/_saveSensorInfo/",
                defaults: new { controller = "Home", action = "_saveSensorInfo" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                //defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}