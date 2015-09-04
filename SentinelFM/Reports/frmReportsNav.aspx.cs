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

public partial class frmReportsNav : SentinelFMBasePage 
{

    public string stReportUrl = "/Reports/frmReportMaster.aspx";
    public bool ExtendedReportEnabled = true;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 1000151)
            stReportUrl = "/Reports/frmReportMaster_hr.aspx";

        //body.Style.Add("overflow", "hidden");
        //if (sn.User.OrganizationId == 123 || sn.SuperOrganizationId == 382 || sn.User.OrganizationId == 327 || sn.User.OrganizationId == 489 
        //        || sn.User.OrganizationId == 622 || sn.User.OrganizationId == 570 || sn.User.OrganizationId == 18 || sn.User.OrganizationId == 951
        //        || sn.User.OrganizationId == 698 || sn.User.OrganizationId == 999630 || sn.User.OrganizationId == 999603 || sn.User.OrganizationId == 999693 || sn.User.OrganizationId == 480 || sn.User.OrganizationId == 999620 || sn.User.OrganizationId == 999692 || sn.User.OrganizationId == 999650 || sn.User.OrganizationId == 952
        // || sn.User.OrganizationId == 999695 || sn.User.OrganizationId == 1000010 || sn.User.OrganizationId == 1000041 || sn.User.OrganizationId == 882 || sn.User.OrganizationId == 563 || sn.User.OrganizationId == 1000088 || sn.User.OrganizationId == 1000096 || sn.User.OrganizationId == 1000110 || sn.User.OrganizationId == 1000056 || sn.User.OrganizationId == 1000148
        //    || sn.User.OrganizationId == 1000144 || sn.User.OrganizationId == 999646 || sn.User.OrganizationId == 1000164 || sn.User.OrganizationId == 1000141 || sn.User.OrganizationId == 999991)

        //    WebReportTab.TabPages[1].IsEnabled = true;        
        //else
        //    WebReportTab.TabPages[1].IsEnabled = false;

        if (sn.User.OrganizationId == 123 || sn.SuperOrganizationId == 382 || sn.User.OrganizationId == 327 || sn.User.OrganizationId == 489
                || sn.User.OrganizationId == 622 || sn.User.OrganizationId == 570 || sn.User.OrganizationId == 18 || sn.User.OrganizationId == 951
                || sn.User.OrganizationId == 698 || sn.User.OrganizationId == 999630 || sn.User.OrganizationId == 999603 || sn.User.OrganizationId == 999693 || sn.User.OrganizationId == 480 || sn.User.OrganizationId == 999620 || sn.User.OrganizationId == 999692 || sn.User.OrganizationId == 999650 || sn.User.OrganizationId == 952
         || sn.User.OrganizationId == 999695 || sn.User.OrganizationId == 1000010 || sn.User.OrganizationId == 1000041 || sn.User.OrganizationId == 882 || sn.User.OrganizationId == 563 || sn.User.OrganizationId == 1000088 || sn.User.OrganizationId == 1000096 || sn.User.OrganizationId == 1000110 || sn.User.OrganizationId == 1000056 || sn.User.OrganizationId == 1000148
            || sn.User.OrganizationId == 1000144 || sn.User.OrganizationId == 999646 || sn.User.OrganizationId == 1000164 || sn.User.OrganizationId == 1000141 || sn.User.OrganizationId == 999991)

            ExtendedReportEnabled = true;
        else
            ExtendedReportEnabled = false;

        //WebReportTab.TabPages[0].ContentURL = stReportUrl;

       


        //if (Request.Cookies[sn.User.OrganizationId.ToString() + "SnReportActiveTab"] != null && Request.Cookies[sn.User.OrganizationId.ToString() + "SnReportActiveTab"].Value.Trim() != "")
        //{
        //    try
        //    {
        //        this.WebReportTab.ActiveTabIndex = int.Parse(Request.Cookies[sn.User.OrganizationId.ToString() + "SnReportActiveTab"].Value);
        //    }
        //    catch
        //    {
        //        this.WebReportTab.ActiveTabIndex = sn.Report.ReportActiveTab;    
        //    }
        //}
        //else
        //    this.WebReportTab.ActiveTabIndex = sn.Report.ReportActiveTab;    
    }
    
}
