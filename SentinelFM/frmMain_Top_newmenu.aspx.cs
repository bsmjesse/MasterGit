using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;
using System.Globalization;

public partial class frmMain_Top_newmenu : System.Web.UI.Page
{
    public string Report_Type = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        Report_Type = clsReportMessage.Report_Type;
    }
}