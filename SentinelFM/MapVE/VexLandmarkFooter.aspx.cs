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
    public partial class MapVE_VexLandmarkFooter : SentinelFMBasePage
    {

        public string startCap = "Start Drawing";
        public string EndCap = "Finish Drawing";
        public string ClearCap = "Clear";
        public string CloseCap = "Close";
        public string SaveCap = "Save";
        public string InsetMapCap = "Inset Map";
        public string HelpCap = "Help";

        protected void Page_Load(object sender, EventArgs e)
        {

            startCap = (string)base.GetLocalResourceObject("StartDrawResource1"); ;
            EndCap = (string)base.GetLocalResourceObject("FinishDrawResource1"); ;
            ClearCap = (string)base.GetLocalResourceObject("ClearDrawResource1"); ;
            CloseCap = (string)base.GetLocalResourceObject("CloseDrawResource1"); ;
            SaveCap = (string)base.GetLocalResourceObject("SaveDrawResource1"); ;
            InsetMapCap = (string)base.GetLocalResourceObject("ShowMiniMapResource1"); ;
            HelpCap = (string)base.GetLocalResourceObject("HelpResource1"); ;

        }
    }
}