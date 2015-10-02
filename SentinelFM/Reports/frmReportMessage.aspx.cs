using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;
using System.Globalization;

public partial class Reports_frmReportMessage : System.Web.UI.Page
{
    public string ReadingText = "I have read all messages";
    public string Report_Type = string.Empty;
    public string Message_Type = string.Empty;
    public string TitleBackColor = "green";
    protected void Page_Load(object sender, EventArgs e)
    {
        SentinelFMSession snMain = (SentinelFMSession)Session["SentinelFMSession"];

        if (snMain.SelectedLanguage != null)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new
                CultureInfo(snMain.SelectedLanguage);

        }

        Report_Type = clsReportMessage.Report_Type;
        Message_Type = clsReportMessage.Message_Type;
        ReadingText = base.GetLocalResourceObject("ReadingResource").ToString();
        if (snMain.User.MenuColor != "") TitleBackColor = snMain.User.MenuColor;
    }
}