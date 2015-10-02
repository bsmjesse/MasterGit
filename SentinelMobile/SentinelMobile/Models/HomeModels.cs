using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace SentinelMobile.Models
{
    public class VehicleInfoModel
    {
        public string vehicleInfoVIN { get; set; }
        public string vehicleInfoOldLicense { get; set; }
        public string vehicleInfoLicensePlate { get; set; }
        public string selVehiceInfoVehicleType { get; set; }
        public string selVehiceInfoMake { get; set; }
        public string selVehiceInfoModel { get; set; }
        public string vehicleInfoYear { get; set; }
        public string vehicleInfoColor { get; set; }
        public string vehicleInfoCost { get; set; }
        public string vehicleInfoDescription { get; set; }
        public string vehicleInfoProvince { get; set; }
        public string vehicleInfoVehicleId { get; set; }
        public string vehicleInfoBoxId { get; set; }
        public string selVehiceInfoBox { get; set; }
        public string vehicleInfoIconTypeId { get; set; }
        public string vehicleInfoEmail { get; set; }
        public string vehicleInfoPhone { get; set; }
        public string vehicleInfoTimeZone { get; set; }
        public string vehicleInfoAutoAdjustDayLightSaving { get; set; }
        public string vehicleInfoFuelType { get; set; }
        public string vehicleInfoNotify { get; set; }
        public string vehicleInfoWarning { get; set; }
        public string vehicleInfoCritical { get; set; }
        public string vehicleInfoMaintenance { get; set; }
        public string vehicleInfoServiceConfigID { get; set; }
    }

    public class EmailModel
    {
        public string sendEmailTRecipiants { get; set; }
        public string sendEmailSubject { get; set; }
        public string sendEmailBody { get; set; }
        public string sendEmailInstaller { get; set; }
        public string sendEmailInstallerEmail { get; set; }
    }
}