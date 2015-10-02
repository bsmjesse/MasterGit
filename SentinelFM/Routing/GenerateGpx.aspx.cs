using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Routing_GenerateGpx : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string gpxOnly = Request.QueryString["gpxonly"];
        string routerPoints = Request.QueryString["NavTeqResponseData"] ?? Request.Form["NavTeqResponseData"];
        if (!String.IsNullOrEmpty(gpxOnly) && !String.IsNullOrEmpty(routerPoints))
        {
            string filename = "route.xml";
            string xml = GenerateXml(routerPoints);
            byte[] buffer = Encoding.Unicode.GetBytes(xml);
            Response.Clear(); //optional: if we've sent anything before
            Response.ContentType = "text/xml"; //must be 'text/xml'
            Response.ContentEncoding = Encoding.UTF8; //we'd like UTF-8
            Response.OutputStream.Write(buffer, 0, buffer.Length);
            Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
        }
    }

    public static string GenerateXml(string xmlStr)
    {
        string[] latlonStrings = xmlStr.Split(',');
        string pointsStr = "";
        foreach (string latlonString in latlonStrings)
        {
            string aPointStr = "<rtept lat=\"{0}\" lon=\"{1}\"></rtept>";
            string[] latlon = latlonString.Split(' ');
            aPointStr = String.Format(aPointStr, latlon[1], latlon[0]);
            pointsStr += aPointStr;
        }
        string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><gpx creator=\"BSMwireless Routing Engine\" version=\"1.1\" xmlns=\"http://www.topografix.com/GPX/1/1\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.topografix.com/GPX/1/1 gpx.xsd\"><metadata></metadata><rte>{0}</rte></gpx>";
        return String.Format(xml, pointsStr);
    }
}