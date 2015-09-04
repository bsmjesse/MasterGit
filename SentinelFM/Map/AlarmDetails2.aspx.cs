using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentinelFM
{
    public partial class AlarmDetails2 : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int AlarmId = Convert.ToInt32(Request.QueryString["AlarmId"]);
                ViewState["AlarmId"] = AlarmId;
                Session["AlarmId"] = AlarmId;
            }
        }
    }
}