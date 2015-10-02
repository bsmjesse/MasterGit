using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SentinelFM
{

  /// <summary>
    /// This class for represent EEPROP Feature in the system.
    /// </summary>
    public class WiFiUpdateCommand
    {
        //--properties
      public int WiFiCommandId { get; set; }
      public int WiFiCommandCode { get; set; }
      public string WiFiCommandName { get; set; }
      public string WiFiCommandDefault { get; set; }
      public int BoxModel { get; set; }
      
        //Methods:
        public WiFiUpdateCommand()
        {
            //Initial 
        }

        public WiFiUpdateCommand(int boxModel)
        {
            this.BoxModel = boxModel;
        }



    } //class
}
