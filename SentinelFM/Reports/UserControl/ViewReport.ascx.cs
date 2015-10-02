using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace SentinelFM
{
    public partial class Reports_UserControl_ViewReport : System.Web.UI.UserControl
    {
        public string SelectReportMsg = string.Empty;
        public string RadTabStripClientID = string.Empty;
        public string LoadingResource = string.Empty ;
        protected void Page_Load(object sender, EventArgs e)
        {
            RadTabStripClientID = Parent.FindControl("RadTabStrip1").ClientID;
            SelectReportMsg = (string)base.GetLocalResourceObject("SelectReport");
            LoadingResource = (string)base.GetLocalResourceObject("LoadingResource");
        }
    }
}