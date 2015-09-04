using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualEarth
{

    [Serializable]
    public enum ValueModel { AvlPin, LandmarkPin, GeozoneShape }

    [Serializable]
    public class AvlPin
    {
        const ValueModel model = ValueModel.AvlPin;
        public string description = string.Empty;
        public double latitude = 0.0;
        public double longitude = 0.0;
        public string address = string.Empty;
        public string timestamp = string.Empty;
        public string status = "Unknown";
        public string iconName = "RedCircle";
        public string speed = "0 km/h";
        public string duration = new TimeSpan();

        public override string ToString()
        {
            return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}!", (int)model, description, latitude, longitude, timestamp, address, status, speed, duration, iconName);
        }


    }
}
