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
    public partial class HOS_frmHOSReport : SentinelFMBasePage 
    {
        
        

        protected void Page_Load(object sender, EventArgs e)
        {

            sn = (SentinelFMSession)Session["SentinelFMSession"];
            
            foreach (DataRow dr in  sn.User.DsDrivers.Tables[0].Rows)
            {
                dr["ServiceDateEnd"] =Convert.ToDateTime(dr["ServiceDate"]).AddHours(2)  ;
            }
           
            repHOS.ReportDocument.SetDataSource(sn.User.DsDrivers);
        }
    }
}
