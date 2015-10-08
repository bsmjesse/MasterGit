using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace VLF.DAS.Logic.models
{
    [JsonObject("alarm")]
    public class AlarmModel : AuditModel
    {
        private BoxModel boxModel;
        private VehicleModel vehicleModel;

        public AlarmModel()
        {
            boxModel = new BoxModel();
            vehicleModel = new VehicleModel();
        }

        [JsonProperty]
        public int AlarmId { get; set; }

        [JsonProperty]
        public DateTime DateTimeCreate { get; set; }

        [JsonProperty]
        public int BoxId { get; set; }

        [JsonProperty]
        public string AlarmTypeName { get; set; }

        [JsonProperty]
        public string AlarmLevel { get; set; }

        [JsonProperty]
        public int UserId { get; set; }

        [JsonProperty]
        public DateTime DateTimeClosed { get; set; }

        [JsonProperty]
        public string AlarmDescription { get; set; }

        [JsonProperty]
        public string LicensePlate { get; set; }

        [JsonProperty]
        public int VehicleId { get; set; }

        [JsonProperty]
        public string StreetAddress { get; set; }

        [JsonProperty]
        public bool ValidGPS { get; set; }

        [JsonProperty("vehicleDescription")]
        public string VehicleDescription { get; set; }

        [JsonProperty]
        public string UserName { get; set; }

        [JsonProperty]
        public decimal Latitude { get; set; }

        [JsonProperty]
        public decimal Speed { get; set; }

        [JsonProperty]
        public string Heading { get; set; }

        [JsonProperty]
        public string SensorMask { get; set; }

        [JsonProperty]
        public bool IsArmed { get; set; }
    }

    public class AuditModel
    {
        [JsonProperty]
        public string CreatedBy { get; set; }

        [JsonProperty]
        public DateTime CreatedOn { get; set; }

        [JsonProperty]
        public string ModifiedBy { get; set; }

        [JsonProperty]
        public DateTime ModifiedOn { get; set; }
    }
}
