using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SentinelMobile.Models
{
    public class Vehicle
    {
        public int BoxId { get; set; }
        public int VehicleId { get; set; }
        public string LicensePlate { get; set; }
        public DateTime OriginDateTime { get; set; }
        public string DisplayDateTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Speed { get; set; }
        public string StreetAddress { get; set; }
        public string Description { get; set; }
        public string VehicleStatus { get; set; }
        public DateTime LastCommunicatedDateTime { get; set; }
        public long SensorMask { get; set; }
        public string MyHeading { get; set; }
        public double Distance { get; set; }
        public string Driver { get; set; }
    }

    public class Vehicles
    {
        public IList<Vehicle> VehicleList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool ShowCheckbox { get; set; }
        public bool ShowDistance { get; set; }
        public string CheckedVehicleIds { get; set; }
        public double UnitOfMes { get; set; }
        public string UnitOfMesName { get; set; }
    }

    public class VehicleInfo
    {
        public int BoxId { get; set; }
        public int VehicleId { get; set; }
        public string LicensePlate { get; set; }
        public string Description { get; set; }
        public long SensorMask { get; set; }
        public string Color { get; set; }
        public short ModelYear { get; set; }
        public double CostPerMile { get; set; }
        public string VinNum { get; set; }
        public string StateProvince { get; set; }
        public short VehicleTypeId { get; set; }
    }

    public class VehicleSensors
    {
        public int BoxId { get; set; }
        public string LicensePlate { get; set; }

        public bool EditMode { get; set; }

        public IList<VehicleSensor> SensorList { get; set; }

        public VehicleSensors()
        {
            BoxId = -1;
            LicensePlate = "";
            EditMode = false;
            SensorList = new List<VehicleSensor>();
        }
    }

    public class VehicleSensor
    {
        public int SensorId { get; set; }
        public string SensorName { get; set; }
        public string SensorAction { get; set; }
        public short AlarmLevelOn { get; set; }
        public short AlarmLevelOff { get; set; }
        public bool ISInUse { get; set; }
        public bool ISDefault { get; set; }
    }
}