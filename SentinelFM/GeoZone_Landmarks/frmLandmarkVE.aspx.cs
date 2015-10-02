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
    public partial class GeoZone_Landmarks_frmLandmarkVE : SentinelFMBasePage
    {
        public string strLandmarkData = null;
        public string strUnitOfMes = "Km";
        protected void Page_Load(object sender, EventArgs e)
        {
            strUnitOfMes = sn.User.UnitOfMes == 1 ? "Km" : "Mi";

        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            string landmarkPoint = Request.Form["landmarkPoint"];
        }
    }
}
