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
    public partial class frmHOSGraph : System.Web.UI.Page
    {
        

        protected void Page_Load(object sender, EventArgs e)
        {

          // sn = (SentinelFMSession)Session["SentinelFMSession"];
              
        }
        protected void dgData_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            //if (sn.User.DsDrivers != null)
            //    e.DataSource = sn.User.DsDrivers;
            //else
            //    e.DataSource = null;
        }

       
}
}
