using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualEarth
{

    [Serializable]
    public enum ValueModel { AvlPin, AvlHistoryPin, LandmarkPin, GeozoneShape }

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
        public string iconName = "RedCircle.ico";
        public string speed = "0 km/h";
        public string duration = string.Empty;
        public string heading = string.Empty;
        

        public override string ToString()
        {
            return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}|", (int)model, description, latitude, longitude, timestamp, address, status, speed, duration, iconName,heading );
        }

        public string ToVEstring()
        {
            return string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}|", (int)model, description, latitude, longitude, timestamp, address, status, speed, duration, iconName,heading );
        }
    }


    [Serializable]
    public class AvlHistoryPin
    {
        const ValueModel model = ValueModel.AvlHistoryPin;
        public string description = string.Empty;
        public double latitude = 0.0;
        public double longitude = 0.0;
        public string address = string.Empty;
        public string timestamp = string.Empty;
        public string messageType = "Unknown";
        public string iconName = "RedCircle.ico";
        public string speed = "0 km/h";
        public string customProp = string.Empty;
        public string heading = string.Empty;

        public override string ToString()
        {
            return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}|", (int)model, description, latitude, longitude, timestamp, address, messageType, speed,customProp, iconName,heading );
        }

        public  string ToVEstring()
        {
            return string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}|", (int)model, description, latitude, longitude, timestamp, address, messageType, speed,customProp, iconName,heading );
        }

    }

    [Serializable]
    public class LandmarkPin
    {
        const ValueModel model = ValueModel.LandmarkPin;
        public string description = string.Empty;
        public double latitude = 0.0;
        public double longitude = 0.0;
        public Int32  radius = 0;
        public string iconName = "Landmark.gif";

        public override string ToString()
        {
            return string.Format("{0};{1};{2};{3};{4};{5}|", (int)model, description, latitude, longitude,radius, iconName);
        }
        public string ToVEstring()
        {
            return string.Format("{0}^{1}^{2}^{3}^{4}^{5}|", (int)model, description, latitude, longitude, radius, iconName);
        }


    }

    [Serializable]
    public class GeozoneShape
    {
        const ValueModel model = ValueModel.GeozoneShape;
        public string description = string.Empty;
        public Int32  numPoints =0;
        public string pointList = string.Empty;
        public Int16 type = 0;
        public Int16 severity =0;

        public override string ToString()
        {
            return string.Format("{0};{1};{2};{3};{4};{5}|", (int)model, description, numPoints, pointList, type, severity);
        }
        public string ToVEstring()
        {
            return string.Format("{0}^{1}^{2}^{3}^{4}^{5}|", (int)model, description, numPoints, pointList, type, severity);
        }


    }



    [Serializable]
    public class AvlMobilePin
    {
        public string description = string.Empty;
        public double latitude = 0.0;
        public double longitude = 0.0;
        public string pinColor = string.Empty;
        

        public string ToVEstring()
        {
            return string.Format("{0}~{1}~{2}~{3}^", description, latitude, longitude, pinColor);
        }

    }
}
