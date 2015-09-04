using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.Reporting.WebForms;

namespace SentinelFM
{
    public partial class ServerReportMain : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Session["OID"] == null)
            //    Server.Transfer("Login.aspx");

            if (!IsPostBack)
            {
                PageInitialization();
            }
        }
        protected void ViewReport_Clicked(object sender, EventArgs e)
        {
            Button cmd = (Button)sender;
            switch (cmd.ID.ToLower())
            {
                case "cmdreport":
                    ServerReportRender();
                    break;
                //case "cmdreturn":
                //    Server.Transfer("Login.aspx");
                //    break;
                default:
                    break;
            }
        }
        protected void ReportSelectedIndex_Changed(object sender, EventArgs e)
        {
            //get ID of selected report
            string ReportID = this.ddlReports.SelectedValue;
            //set parameters of selected report
            ServerReportParameterInitial(ReportID);
        }
        /// <summary>
        /// Private methods region
        /// </summary>
        /// 
        #region Private methods
        /// <summary>
        /// Initial report
        /// </summary>
        private void PageInitialization()
        {
            this.lblOrganization.Text = sn.User.OrganizationName;// Session["ONAME"].ToString();

            SystemConfiguration osc = new SystemConfiguration();

            // Load reports
            this.ddlReports.Items.Clear();
            if (osc.ReportList())
            {
                this.ddlReports.DataSource = osc.DataSource;
                this.ddlReports.DataTextField = "Label";
                this.ddlReports.DataValueField = "Value";
                this.ddlReports.DataBind();
            }
            else
            {
                ListItem li = new ListItem("No report", "", true);
                li.Selected = true;
                this.ddlReports.Items.Add(li);
            }
            this.ddlReports.SelectedIndex = 0;

            //Load Organiztion fleets 
            this.ddlFleet.Items.Clear();
            if (osc.FleetList(Convert.ToInt32(sn.User.OrganizationId  )))
            {
                this.ddlFleet.DataSource = osc.DataSource;
                this.ddlFleet.DataTextField = "Label";
                this.ddlFleet.DataValueField = "Value";
                this.ddlFleet.DataBind();
            }
            else
            {
                this.ddlReports.Items.Add(new ListItem("No fleet", "", true));
            }
            this.ddlFleet.SelectedIndex = 0;

            this.txtFromDate.Text = "";
            this.txtToDate.Text = "";

            //this.cmdReport.Enabled = false;

            //this.ServerReportViewer.ProcessingMode = ProcessingMode.Remote;
            //this.ServerReportViewer.AsyncRendering = false;
            //this.ServerReportViewer.SizeToReportContent = true;
            //this.ServerReportViewer.ZoomMode = ZoomMode.FullPage;
            //this.ServerReportViewer.ShowParameterPrompts = false;
            //this.ServerReportViewer.ShowBackButton = true;
            //this.ServerReportViewer.ShowPageNavigationControls = true;
            //this.ServerReportViewer.ShowPrintButton = true;
            //this.ServerReportViewer.ShowRefreshButton = true;
            //this.ServerReportViewer.ShowReportBody = true;
            //this.ServerReportViewer.ShowToolBar = true; 
            //this.ServerReportViewer.ServerReport.Refresh(); 
        }
        /// <summary>
        /// Initial server report parameters
        /// </summary>
        /// <param name="ReportID"></param>
        private void ServerReportParameterInitial(string ReportID)
        {
            string sUri = "";
            string sPath = "";
            string sType = "";

            SystemConfiguration osc = new SystemConfiguration();

            if (osc.ReportDetail(ReportID))
            {
                sUri = osc.ReportUri;
                sPath = osc.ReportPath;
                sType = osc.ReportType;

                ReportParameterInfoCollection rp = this.ServerReportViewer.ServerReport.GetParameters();
            }

        }
        /// <summary>
        /// Render report
        /// </summary>
        private void ServerReportRender()
        {
            string value = this.ddlReports.SelectedValue.ToString();
            string sPath = "";
            string sOrganization = sn.User.OrganizationId.ToString() ;   // Session["OID"].ToString();
            string sFleet = this.ddlFleet.SelectedValue.ToString();

            Int32 index = 0;

            if (int.TryParse(value, out index))
            {
                switch (index)
                {
                    case 1:
                        sPath = "";
                        break;
                    case 10003:
                        sPath = "/SpeedViolation/Speed Violation Detail (Road Speed)";
                        // Path of Report file
                        break;
                    case 10000:
                        //Server.Transfer("SpeedViolationSummary.aspx");
                        sPath = "/SpeedSummary_DWH/ViolationSummary";
                        break;
                    case 4:
                        //Server.Transfer("Top20Vehicles.aspx");
                        sPath = "/Top20Vehicles/Top20Vehicles";
                        break;
                    case 10004:
                        //Server.Transfer("VehiclePerformance.aspx");
                        sPath = "/sqluser1_test/VehiclePerformance";
                        break;
                    case 10002:
                        sPath = "/ActivitySummaryDWH/ActivitySummaryPerVehicle";
                        break;
                    default:
                        sPath = "";
                        break;
                }

                if (sPath != string.Empty)
                {

                    //Server.Transfer("SpeedViolationDetail.aspx");
                    // Set the processing mode for the ReportViewer to Remote
                    //ReportViewer1.ProcessingMode = ProcessingMode.Remote;
                    this.ServerReportViewer.ProcessingMode = ProcessingMode.Remote;
                    this.ServerReportViewer.AsyncRendering = false;
                    this.ServerReportViewer.SizeToReportContent = true;
                    this.ServerReportViewer.ZoomMode = ZoomMode.FullPage;
                    this.ServerReportViewer.ShowParameterPrompts = true;
                    this.ServerReportViewer.ShowPrintButton = true;
                    this.ServerReportViewer.ShowToolBar = true;
                    //this.ServerReportViewer.ZoomPercent = 120;
                    //this.ServerReportViewer.ServerReport.GetParameters();
                    //this.ServerReportViewer.LocalReport.GetParameters(); 

                    //ServerReportViewer.ServerReport  serverReport = this.ServerReportViewer.ServerReport;
                    //Credential
                    ReportServerCredentials c = new ReportServerCredentials("bsmreports", "T0ybhARQ", "production");
                    ServerReportViewer.ServerReport.ReportServerCredentials = c;
                    // RUI of Report Service Server
                    ServerReportViewer.ServerReport.ReportServerUrl = new Uri("http://192.168.9.73/ReportServer_DWH");
                    ServerReportViewer.ServerReport.ReportPath = sPath;

                    ReportParameterInfoCollection rp = ServerReportViewer.ServerReport.GetParameters();
                    if (rp["OrganizationID"] != null)
                    {
                        ReportParameter oid = new ReportParameter("OrganizationID", sOrganization, false);
                        ServerReportViewer.ServerReport.SetParameters(oid);
                    }

                    if (rp["Organization"] != null)
                    {
                        ReportParameter oid = new ReportParameter("Organization", sOrganization, false);
                        ServerReportViewer.ServerReport.SetParameters(oid);
                    }

                    if (rp["TimeZone"] != null)
                    {
                        ReportParameter otz = new ReportParameter("TimeZone", "UTC-5", false);
                        ServerReportViewer.ServerReport.SetParameters(otz);
                    }
                    //else if (rp["FleetID"] != null)
                    //{
                    //    ReportParameter fid = new ReportParameter("FleetID", sFleet, false);
                    //    serverReport.SetParameters(fid);
                    //}
                    // Parameter
                    //ReportParameter salesOrderNumber = new ReportParameter("Category", "3", false);
                    //salesOrderNumber.Name = "Category";
                    //salesOrderNumber.Values.Add("3");

                    ServerReportViewer.ServerReport.Refresh();

                    //= this.ServerReportViewer.Height;


                }
            }

            if (value == "1")
            {
                // Set the processing mode for the ReportViewer to Remote
                this.ServerReportViewer.ProcessingMode = ProcessingMode.Remote;
                this.ServerReportViewer.AsyncRendering = false;
                this.ServerReportViewer.SizeToReportContent = true;
                this.ServerReportViewer.ZoomMode = ZoomMode.FullPage;
                this.ServerReportViewer.ShowParameterPrompts = true;
                this.ServerReportViewer.ShowPrintButton = true;
                //Instance Server Report
                //ServerReport serverReport = this.ServerReportViewer.ServerReport;
                //Instance Credetial
                ReportServerCredentials rsc = new ReportServerCredentials("bsmreports", "T0ybhARQ", "production");
                //Credential
                ServerReportViewer.ServerReport.ReportServerCredentials = rsc;
                // RUI of Report Service Server
                ServerReportViewer.ServerReport.ReportServerUrl = new Uri("http://192.168.9.73/ReportServer_DWH");
                // Path of Report file
                ServerReportViewer.ServerReport.ReportPath = "/ActivitySummaryDWH/ActivitySummaryVehicle";

                // Parameter
                //this.ServerReportViewer.ServerReport.GetParameters();
                //this.ServerReportViewer.LocalReport.GetParameters(); 
                //ReportParameter salesOrderNumber = new ReportParameter("Category", "3", false);
                //salesOrderNumber.Name = "Category";
                //salesOrderNumber.Values.Add("3");

                //serverReport.SetParameters(salesOrderNumber);

                ServerReportViewer.ServerReport.Refresh();
            }
            else
            {

            }
        }
        /// <summary>
        /// Initial server report viewer
        /// </summary>
        private bool ServerReportViewerInitial(string ReportUri, string ReportPath)
        {
            try
            {
                this.ServerReportViewer.ProcessingMode = ProcessingMode.Remote;
                this.ServerReportViewer.AsyncRendering = false;
                this.ServerReportViewer.SizeToReportContent = true;
                this.ServerReportViewer.ZoomMode = ZoomMode.FullPage;
                this.ServerReportViewer.ShowParameterPrompts = true;
                this.ServerReportViewer.ShowPrintButton = true;
                this.ServerReportViewer.ShowToolBar = true;

                //ServerReport serverReport = this.ServerReportViewer.ServerReport;
                //Credential
                ReportServerCredentials c = new ReportServerCredentials("bsmreports", "T0ybhARQ", "production");
                ServerReportViewer.ServerReport.ReportServerCredentials = c;
                ServerReportViewer.ServerReport.ReportServerUrl = new Uri(ReportUri);
                ServerReportViewer.ServerReport.ReportPath = ReportPath;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            finally
            {
            }

            return true;

        }


        #endregion
    }
}