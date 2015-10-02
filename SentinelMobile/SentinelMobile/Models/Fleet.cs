using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SentinelMobile.Models
{
    public class Fleet
    {
        public int FleetId { get; set; }
        public string Name { get; set; }
        public string OrganizationName { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public Fleet(string name = null, string organizationName = null, int fleetId = 0, string description = null, string path = null)
        {
            FleetId = fleetId;
            Name = name;
            OrganizationName = organizationName;
            Description = description;
            Path = path??string.Empty;
        }
    }
}