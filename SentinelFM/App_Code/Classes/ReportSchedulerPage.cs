using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for ReportSchedulerPage
/// </summary>
public class ReportSchedulerPage : SentinelFMBasePage
{
   protected SentinelFM.ServerReports.Reports reportProxy = null;
   
   public ReportSchedulerPage()
   {
      reportProxy = new SentinelFM.ServerReports.Reports();
   }

   public override void Dispose()
   {
      if (reportProxy != null) 
         reportProxy.Dispose();
      base.Dispose();
   }
}
