using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace VLF.DAS.Logic.models
{
  [JsonObject("vehicle_info")]
  public class VehicleInfoModel: AuditModel
  {
    /// <param name="vehicleId"></param>
    /// <param name="weekdayStart"></param>
    /// <param name="weekdayEnd"></param>
    /// <param name="weekendStart"></param>
    /// <param name="weekendEnd"></param>
    [JsonProperty]
    public int VehicleId { get; set; }

    [JsonProperty]
    public DateTime WeekdatStart { get; set; }

    [JsonProperty]
    public DateTime WeekdayEnd { get; set; }

    [JsonProperty]
    public DateTime WeekendStart { get; set; }

  }
}
