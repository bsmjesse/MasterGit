using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class DashBoard_reportWait : System.Web.UI.Page
{

    public string rpt = "";

    protected void Page_Load(object sender, EventArgs e)
    {

          string reportId = Request.QueryString["reportId"].ToString();
            string fleetId = Request.QueryString["fleetId"].ToString();
            string data = Request.QueryString["data"].ToString();

             rpt = "'reportViewer.aspx?reportId=" + reportId + "&fleetId=" + fleetId + "&data=" + data+"'" ;
        
            //Response.AppendHeader("Refresh", "3; url= '" + rpt+"'");
    }
}