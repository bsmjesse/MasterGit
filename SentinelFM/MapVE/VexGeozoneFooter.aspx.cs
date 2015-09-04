using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
namespace SentninelFM
{
    public partial class MapVE_VexGeozoneFooter : SentinelFMBasePage
    {
        public string startCap = "Start Drawing";
        public string EndCap = "Finish Drawing";
        public string ClearCap = "Clear";
        public string CloseCap = "Close";
        public string SaveCap = "Save";
        public string InsetMapCap = "Inset Map";
        public string HelpCap = "Help";

        public string lblGeoZoneTypeResource = "Geozone Type:";
        public string lblPointsResource = "Points:";
        public string lblPolygonResource = "Polygon:";
        public string lblRectangeResource = "Rectangle:";


        protected void Page_Load(object sender, EventArgs e)
        {

            startCap = (string)base.GetLocalResourceObject("StartDrawResource1"); 
            EndCap = (string)base.GetLocalResourceObject("FinishDrawResource1"); 
            ClearCap = (string)base.GetLocalResourceObject("ClearDrawResource1"); 
            CloseCap = (string)base.GetLocalResourceObject("CloseDrawResource1"); 
            SaveCap = (string)base.GetLocalResourceObject("SaveDrawResource1"); 
            InsetMapCap = (string)base.GetLocalResourceObject("ShowMiniMapResource1"); 
            HelpCap = (string)base.GetLocalResourceObject("HelpResource1");

            lblGeoZoneTypeResource = (string)base.GetLocalResourceObject("lblGeoZoneTypeResource1");
            lblPointsResource = (string)base.GetLocalResourceObject("lblPointsResource1");
            lblPolygonResource = (string)base.GetLocalResourceObject("lblPolygonResource1");
            lblRectangeResource = (string)base.GetLocalResourceObject("lblRectangeResource1"); 




        }
    }
}
