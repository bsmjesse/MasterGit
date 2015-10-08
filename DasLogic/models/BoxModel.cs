using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using VLF.CLS;

namespace VLF.DAS.Logic.models
{
  [JsonObject("box")]
  public class BoxModel: AuditModel
  {
    [JsonProperty]
    public int BoxId { get; set; }
  }
}
