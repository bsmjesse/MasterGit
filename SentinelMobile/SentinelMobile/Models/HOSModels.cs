using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VLF.DAS;

namespace SentinelMobile.Models
{
    public class Driver
    {
        public string DriverName = string.Empty;
        public string DriverID = string.Empty;
        public Driver(string driverId, string driverName)
        {
            DriverID = driverId;
            DriverName = driverName;
        }

    }

    public class DriverPending
    {
        
        public string dID,dName,rID,time;
        public bool chked;
        public DriverPending()
        {
            
            dID = "";
            dName="";
            rID = "";
            time = "";
            chked = false;
        }

    }

    public class States
    {
        public string FullName = string.Empty;
        public string State = string.Empty;
        public States(string fullName, string state)
        {
            FullName = fullName;
            State = state;
        }

    }

    public class Duty
    {
        [System.ComponentModel.DefaultValue("")]
        public string time { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string duty { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string action { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string province { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string city  {get;set;}
        [System.ComponentModel.DefaultValue("")]
        public string remark  {get;set;}
        [System.ComponentModel.DefaultValue("")]
        public string dutyTxt  {get;set;}
        [System.ComponentModel.DefaultValue("")]
        public string actionTxt  {get;set;}
        [System.ComponentModel.DefaultValue("")]
        public string provinceTxt {get;set;}
        [System.ComponentModel.DefaultValue("")]
        public string cityTxt  {get;set;}
        [System.ComponentModel.DefaultValue("")]
        public string odometer {get;set;}
        [System.ComponentModel.DefaultValue("")]
        public string TLID { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string date { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string cycle { get; set; }
     };

     public class TripInfo {
            [System.ComponentModel.DefaultValue("")]
            public string date {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string cycle {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string company {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string shiftStart {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string odoStart  {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string odoEnd  {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string coDriver  {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string hometerm  {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string equipmentNo  {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string shippingDoc  {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string truckNo  {get;set;}
            [System.ComponentModel.DefaultValue("")]           
            public string tailerNo  {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string deferredHours  {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string deferredDay  {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string emergency  {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string adverseDriving  {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string personalStart  {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string personalEnd  {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string remark  {get;set;}
            [System.ComponentModel.DefaultValue("")]
            public string approved { get; set; } 
     };
}