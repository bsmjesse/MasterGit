using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentinelFM
{

    public partial class Help_Validate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SentinelFMSession sn = (SentinelFMSession)Session["SentinelFMSession"];
            //Response.Write("0");
            if (sn == null || sn.User == null || string.IsNullOrEmpty(sn.UserName))
            {
                Response.Write("0");
            }
            else
            {
                Response.Write("1");
            }

        }
    }
}