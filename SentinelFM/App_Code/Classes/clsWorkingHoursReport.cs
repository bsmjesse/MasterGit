using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for clsWorkingHoursReport
/// </summary>
namespace SentinelFM
{
    public class WorrkingHoursReport
    {
        public string _Timestamp = string.Empty;
        public string _Action = string.Empty;
        public string _TimeZone = string.Empty;
        public string _from = string.Empty;
        public string _To = string.Empty;
        public string _WorkingDays = string.Empty;
        public string _VehicleName = string.Empty;
        public string _UserName = string.Empty;
        public string _Email = string.Empty;

        public string Timestamp 
        {
            get { return _Timestamp;}
            set { _Timestamp = value; }
        }
        public string Action {
            get { return _Action; }
            set { _Action = value; }
        }
        public string TimeZone {
            get { return _TimeZone; }
            set { _TimeZone = value; }
        }
        public string from {
            get { return _from; }
            set { _from = value; }
        }
        public string To {
            get { return _To; }
            set { _To = value; }
        }
        public string WorkingDays {
            get { return _WorkingDays; }
            set { _WorkingDays = value; }
        }
        public string VehicleName {
            get { return _VehicleName; }
            set { _VehicleName = value; }
        }
        public string UserName {
            get { return _UserName; }
            set { _UserName = value; }
        }
        public string Email {
            get { return _Email; }
            set { _Email = value; }
        }
    }
}
