using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for Landmark
/// </summary>
public class Landmark
{

    public long LandmarkId;
    public double Latitude;
    public double Longitude;
    public string Name = string.Empty;
    public int Radius;

    public Landmark(DataRow row)
    {
        long lid = 0;
        double lat = 0;
        double lon = 0;
        int radius = 0;

        if (row.Table.Columns.Contains("LandmarkId") && row["LandmarkId"] != DBNull.Value)
            long.TryParse(row["LandmarkId"].ToString(), out lid);
        LandmarkId = lid;

        if (row.Table.Columns.Contains("Latitude") && row["Latitude"] != DBNull.Value)
            double.TryParse(row["Latitude"].ToString(), out lat);
        Latitude = lat;

        if (row.Table.Columns.Contains("Longitude") && row["Longitude"] != DBNull.Value)
            double.TryParse(row["Longitude"].ToString(), out lon);
        Longitude = lon;

        if (row.Table.Columns.Contains("Radius") && row["Radius"] != DBNull.Value)
            int.TryParse(row["Radius"].ToString(), out radius);
        Radius = radius;

        if (row.Table.Columns.Contains("LandmarkName") && row["LandmarkName"] != DBNull.Value)
            Name = row["LandmarkName"].ToString();

    }
}