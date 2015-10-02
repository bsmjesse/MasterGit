using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using SentinelFM.Resolver;

namespace SentinelFM
{
    public partial class DriverFinder_GetAddressLatLon : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string address = HttpUtility.UrlDecode(Request.QueryString["address"]);
            address = address.Replace(", ", ",");
            double lat = 0;
            double lon = 0;
            string rsAddress = null;

            Resolver.Resolver resolver = new Resolver.Resolver();

            bool find = resolver.Location(address, ref lat, ref lon, ref rsAddress);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("lat", Convert.ToString(lat));
            result.Add("lon", Convert.ToString(lon));
            result.Add("Address", rsAddress);
            var oSerializer = new JavaScriptSerializer();
            string json = oSerializer.Serialize(result);
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write(json);
            Response.End();
        }
    }
}