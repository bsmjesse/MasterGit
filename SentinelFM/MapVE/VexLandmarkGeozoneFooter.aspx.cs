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

namespace SentinelFM
{
    public partial class MapVE_VexLandmarkGeozoneFooter : SentinelFMBasePage 
    {
        public string CloseCap = "Close";
        public string InsetMapCap = "Inset Map";
        public string HelpCap = "Help";

        protected void Page_Load(object sender, EventArgs e)
        {

            CloseCap = (string)base.GetLocalResourceObject("CloseDrawResource1"); ;
            InsetMapCap = (string)base.GetLocalResourceObject("ShowMiniMapResource1"); ;
            HelpCap = (string)base.GetLocalResourceObject("HelpResource1"); ;

        }
    }
}
