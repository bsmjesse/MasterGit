using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

public partial class Top20Vehicles : System.Web.UI.Page
{

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {

            // Set the processing mode for the ReportViewer to Remote
            this.ServerReportViewer.ProcessingMode = ProcessingMode.Remote;
            this.ServerReportViewer.AsyncRendering = false;
            this.ServerReportViewer.SizeToReportContent = true;
            this.ServerReportViewer.ZoomMode = ZoomMode.FullPage;
            this.ServerReportViewer.ShowParameterPrompts = true;
            this.ServerReportViewer.ShowPrintButton = true;

            ServerReport serverReport = this.ServerReportViewer.ServerReport;
            //Credential
            ReportServerCredentials c = new ReportServerCredentials("bsmreports", "T0ybhARQ", "production");
            serverReport.ReportServerCredentials = c;

            // RUI of Report Service Server
            serverReport.ReportServerUrl = new Uri("http://192.168.9.73/ReportServer_DWH");
            // Path of Report file
            serverReport.ReportPath = "/Top20Vehicles/Top20Vehicles";

            // Parameter
            //this.ServerReportViewer.ServerReport.GetParameters();
            //this.ServerReportViewer.LocalReport.GetParameters(); 
            //ReportParameter salesOrderNumber = new ReportParameter("Category", "3", false);
            //salesOrderNumber.Name = "Category";
            //salesOrderNumber.Values.Add("3");

            //serverReport.SetParameters(salesOrderNumber);

            serverReport.Refresh();

        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        // 
    }

    public class ReportServerCredentials : IReportServerCredentials
    {
        protected string username;
        protected string pwd;
        protected string domain;

        public ReportServerCredentials(string UserName, string Password, string Domain)
        {
            this.username = UserName;
            this.pwd = Password;
            this.domain = Domain;
        }

        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;  // Use default identity.
            }
        }

        public System.Net.ICredentials NetworkCredentials
        {
            get
            {
                return new System.Net.NetworkCredential(username, pwd, domain);
            }
        }

        public bool GetFormsCredentials(out System.Net.Cookie authCookie,
                    out string user, out string password, out string authority)
        {
            authCookie = null;
            user = password = authority = null;
            return false;  // Not use forms credentials to authenticate.
        }
    }
}