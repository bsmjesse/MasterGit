using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentinelFM
{
    public partial class DashBoard_portal : SentinelFMBasePage
    {
        
        public string portalURL = "portalDefault.js";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (sn.User.OrganizationId == 951 || sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999618 || sn.User.OrganizationId == 999955)
                portalURL = "portalUP.js";
        }
    }
}