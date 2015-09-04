using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;



namespace SentinelFM
{
    public partial class Reports_SSRSReports_frmSaltSpreaderSummaryReport : SentinelFMBasePage                      
    {
        #region Private Variables

        private Int32 miOrganizationID = 0;
        private string msOrganizationName = "";
        private Int32 miUserID = 0;
        private string msUserTimeZone = "";
        private Int32 miFleetID = 0;
        private string msFleetName = "";
        private string msReportID = "";
        private string msReportUri = "";
        private string msReportPath = "";
        private string msPreviousePage = "";

        #endregion
        private SystemConfiguration osc = new SystemConfiguration();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RenderReport();
            }
        }


        /// <summary>
        /// Render report
        /// </summary>
        private void RenderReport()
        {

         
            Microsoft.Reporting.WebForms.ServerReport serverReport = this.ServerReportViewer.ServerReport;
            //Instance Credetial
            ReportServerCredentials rsc = new ReportServerCredentials("bsmreports", "T0ybhARQ", "production");
            serverReport.ReportServerCredentials = rsc;
            // RUI of Report Service Server
            serverReport.ReportServerUrl = new Uri("http://192.168.9.73/ReportServer_DWH"); 
            // Path of Report file
            serverReport.ReportPath = "/SaltSpeader/SaltSpreaderSummary";
            // Parameter
            ReportParameterInfoCollection rp = ServerReportViewer.ServerReport.GetParameters();
            serverReport.SetParameters(new ReportParameter("userId", sn.UserID.ToString() , false));
            serverReport.SetParameters(new ReportParameter("fleetId", sn.Report.FleetId.ToString() , false));
            serverReport.SetParameters(new ReportParameter("fromDate", sn.Report.FromDate, false));
            serverReport.SetParameters(new ReportParameter("toDate", sn.Report.ToDate, false));
            
            serverReport.Refresh();
         
        }

    }
     
}