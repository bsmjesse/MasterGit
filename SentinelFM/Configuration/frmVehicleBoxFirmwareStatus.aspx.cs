using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using VLF.CLS;

namespace SentinelFM
{
    public partial class frmVehicleBoxFirmwareStatus : SentinelFMBasePage
    {
        string strConnection = System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ToString();
        string strMessage = string.Empty;
        int intOrganization = 0;
        int intFleet = 0;
        int intStatus = 0;

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //this.dgDetails.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgDetails_PageIndexChanged);
            //this.dgTotal.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.GridTotalSelectedIndex_Changed);
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Page_Initialization();
            }
        }

        #region Command Events
        protected void cmdVehicleInfo_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmVehicleInfo.aspx");
        }

        protected void cmdAlarms_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmAlarms.aspx");
        }

        protected void cmdBoxFirmware_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmVehicleBoxFirmwareStatus.aspx");
        }

        protected void cmdOutputs_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmOutputs.aspx");
        }

        private void cmdLandmarks_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmEmails.aspx");
        }

        private void cmdPreference_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmPreference.aspx");
        }

        protected void cmdFleetVehicle_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmVehicleFleet.aspx");
        }
        #endregion

        protected void cmdExport_Clicked(object sender, EventArgs e)
        {
            InitialDownloadHyperLink(false, "", "cmdDisabled", "");

            this.Alert(ExportBoxFirmwareStatus());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FleetSelectedIndex_Changed(Object sender, EventArgs e)
        {
            InitialDownloadHyperLink(false, "", "cmdDisabled", "");

            intStatus = 0;
            intFleet = StringToInt(this.dlFleet.SelectedItem.Value);
            intOrganization = StringToInt(this.dlOrgamization.SelectedItem.Value);

            if (InitialTaskTotalList(intOrganization, intFleet))
            {
                InitialTaskDetailList(intStatus, intOrganization, intFleet);
            }

            if (!string.IsNullOrEmpty(strMessage))
                this.Message.Text = strMessage;
            else
                this.Message.Text = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OrganizationSelectedIndex_Changed(Object sender, EventArgs e)
        {
            InitialDownloadHyperLink(false, "", "cmdDisabled", "");

            intStatus = 0;
            intFleet = 0;
            intOrganization = StringToInt(this.dlOrgamization.SelectedItem.Value);

            if (intOrganization > 0)
            {
                InitialOrganizationFleetList(intOrganization);
                this.dlFleet.Enabled = true;
            }
            else
            {
                this.dlFleet.DataSource = null;
                this.dlFleet.DataBind();
                this.dlFleet.Items.Clear();
                this.dlFleet.Enabled = false;
            }

            if (string.IsNullOrEmpty(strMessage))
            {
                if (InitialTaskTotalList(intOrganization, intFleet))
                {
                    InitialTaskDetailList(intStatus, intOrganization, intFleet);
                }
            }

            if (!string.IsNullOrEmpty(strMessage))
                this.Message.Text = strMessage;
            else
                this.Message.Text = "";

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TaskTotalSelectedIndex_Changed(Object sender, DataGridCommandEventArgs e)
        {
            try
            {
                InitialDownloadHyperLink(false, "", "cmdDisabled", "");

                intStatus = e.Item.ItemIndex;
                intFleet = (this.dlFleet.Enabled) ? StringToInt(this.dlFleet.SelectedItem.Value) : 0;
                intOrganization = StringToInt(this.dlOrgamization.SelectedItem.Value);

                if (InitialTaskDetailList(intStatus, intOrganization, intFleet))
                {
                    this.dgTotal.SelectedIndex = intStatus;
                    this.Message.Text = "";
                }
                else
                {
                    this.Message.Text = strMessage;
                }
            }
            catch (Exception Ex)
            {
                intStatus = 0;
                this.Message.Text = Ex.Message + "(" + intStatus.ToString() + " | " + intFleet.ToString() + " | " + intOrganization.ToString() + ")";
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        #region Private Methods Section

        /// <summary>
        /// Creates frameset page with url and back button
        /// </summary>
        /// <param name="url"></param>
        private void CreateResponsePage(string url)
        {
            StringBuilder pageBuilder =
               new StringBuilder("<html><frameset id=\"TopFrame\" rows=\"*,24px\" frameSpacing=\"0\" border=\"0\" bordercolor=\"gray\" frameBorder=\"0\">");
            pageBuilder.AppendFormat("<frame name=\"report\" src='{0}' scrolling=auto  frameborder=\"0\" noresize />", url);
            pageBuilder.AppendLine("<frame name=\"reportback\" src=\"frmReportBack.aspx\" scrolling=\"no\" frameborder=\"1\" noresize /></frameset></html>");

            Response.Write(pageBuilder.ToString());
            //return pageBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string ExportBoxFirmwareStatus() 
        {
            this.BusyReport.Title = "Export";
            this.BusyReport.Title = "Rendering report.............";
            this.BusyReport.Visible = true;

            string message = "";
            string ReportPath = "";
            string parameters = InitialReportParameters();

            sn.Report.GuiId = 10038;                                    //Convert.ToInt16(this.cboReports.SelectedValue);
            sn.Report.XmlParams = InitialReportParameters();            //xmlParams;
            sn.Report.FromDate = DateTime.Now.ToString();               //date. from.ToString();
            sn.Report.ToDate = DateTime.Now.ToString();                 //to.ToString();
            sn.Report.ReportFormat = 2;                                 //Convert.ToInt32(this.cboFormat.SelectedValue);
            sn.Report.ReportType = "10038";                             //cboReports.SelectedValue;
            sn.Report.FleetId = 0;                                      //Convert.ToInt32(this.cboFleet.SelectedValue);
            sn.Report.FleetName = "";                                   //this.cboFleet.SelectedItem.Text;
            sn.Report.DriverId = 0;                                     //this.ddlDrivers.SelectedValue != "" ? Convert.ToInt32(this.ddlDrivers.SelectedValue) : 0;
            sn.Report.LandmarkName = "";                                 //this.ddlLandmarks.SelectedValue.ToString();
            sn.Report.LicensePlate = "";                                //this.cboVehicle.SelectedValue;

            //Response.Redirect("frmReportViewer.aspx");

            try
            {

                ReportingServices.ReportingServices sr = new ReportingServices.ReportingServices();

                sr.Timeout = 1800000;

                if (!sr.RenderDirectReport(parameters, ref ReportPath))                // 1st
                {
                    if (!sr.RenderDirectReport(parameters, ref ReportPath))            // 2nd in case db server is busy.
                    {
                        message = sr.Message().ToString();
                        ReportPath = "";
                    }
                }
            }
            catch (Exception ex)
            {
                ReportPath = "";
                message = ex.Message.ToString();
            }
            
            if (!String.IsNullOrEmpty(ReportPath))
            {
                ReportPath = ReportPath.Replace("https", "http");

                InitialDownloadHyperLink(true, ReportPath, "cmdEnabled", ReportPath);

                //this.cmdDownload.NavigateUrl = ReportPath;      // this.cmdExport.ToolTip.ToString();
                //this.cmdDownload.ToolTip = ReportPath;           //this.cmdExport.ToolTip.ToString();
                //this.cmdDownload.Enabled = true;
                //this.cmdDownload.CssClass = "cmdEnabled";

                message = "File export successful.";
            }
            else 
            {
                InitialDownloadHyperLink(false, "", "cmdDisabled", "");

                //this.cmdDownload.NavigateUrl = "";
                //this.cmdDownload.ToolTip = "";
                //this.cmdDownload.Enabled = false;
                //this.cmdDownload.CssClass = "cmdDisabled";

                if (string.IsNullOrEmpty(message))
                    message = Resources.Const.Reports_NoData;
            }

            this.BusyReport.Visible = false;
            this.BusyReport.Title = "";
            this.BusyReport.Title = "";

            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool Page_Initialization()
        {
            this.cmdExport.Attributes.Add("onclick", BusyReport.ShowFunctionCall);
            this.BusyReport.Title = "Export";
            this.BusyReport.Text = "Please wait............";

            intOrganization = 0;
            intFleet = 0;
            intStatus = 0;

            this.dlFleet.DataSource = null;
            this.dlFleet.Enabled = false;

            
            InitialOrganizationList();
            InitialTaskTotalList(intOrganization, intFleet);
            InitialDownloadHyperLink(false, "", "cmdDisabled", "");
            InitialTaskDetailList(intStatus, intOrganization, intFleet);

            return true;
        }

        private void InitialDownloadHyperLink(bool Status, string Url, string StyleName, string Tip) 
        {
            this.cmdDownload.NavigateUrl = Url;      // this.cmdExport.ToolTip.ToString();
            this.cmdDownload.ToolTip = Tip;           //this.cmdExport.ToolTip.ToString();
            this.cmdDownload.Enabled = Status;
            this.cmdDownload.CssClass = StyleName;
        }

        /// <summary>
        /// ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString
        /// </summary>
        /// <returns></returns>
        private bool InitialTaskDetailList(int Status, int Organization, int Fleet)
        {
            try
            {
                this.dgDetails.DataSource = null;
                this.dgDetails.DataBind();

                using (SqlConnection connection = new SqlConnection(strConnection))
                {
                    using (SqlCommand sqlComment = new SqlCommand())
                    {
                        sqlComment.CommandTimeout = 600;
                        sqlComment.CommandType = CommandType.StoredProcedure;
                        sqlComment.Connection = connection;
                        sqlComment.CommandText = "usp_BoxFirmwareStatusDetails";

                        //@Status            int
                        sqlComment.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
                        sqlComment.Parameters["@Status"].Value = Status;

                        // @OrganizationID    int = 0,
                        sqlComment.Parameters.Add(new SqlParameter("@OrganizationID", SqlDbType.Int));
                        sqlComment.Parameters["@OrganizationID"].Value = Organization;

                        // @OrganizationID    int = 0,
                        sqlComment.Parameters.Add(new SqlParameter("@FleetID", SqlDbType.Int));
                        sqlComment.Parameters["@FleetID"].Value = Fleet;

                        //@FromDate          datetime,
                        sqlComment.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime));
                        sqlComment.Parameters["@FromDate"].Value = DateTime.Today;

                        //@ToDate            datetime,
                        sqlComment.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime));
                        sqlComment.Parameters["@ToDate"].Value = DateTime.Today.AddDays(1);

                        connection.Open();
                        using (SqlDataReader reader = sqlComment.ExecuteReader())
                        {
                            DataTable dtDetail = new DataTable("Detail");

                            dtDetail.Load(reader);

                            this.dgDetails.DataSource = dtDetail;
                            this.dgDetails.DataBind();

                            if (Organization == 0)
                                this.dgDetails.Columns[1].Visible = true;
                            else
                                this.dgDetails.Columns[1].Visible = false;
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                strMessage = "Task Detail Initial failed for " + ex.Message;
            }

            return true;
        }

        /// <summary>
        /// ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString
        /// </summary>
        /// <returns></returns>
        private bool InitialTaskTotalList(int Organization, int Fleet)
        {
            try
            {
                this.dgTotal.DataSource = null;
                this.dgTotal.DataBind();

                using (SqlConnection connection = new SqlConnection(strConnection))
                {
                    using (SqlCommand sqlComment = new SqlCommand())
                    {
                        sqlComment.CommandTimeout = 600;
                        sqlComment.CommandType = CommandType.StoredProcedure;
                        sqlComment.Connection = connection;
                        sqlComment.CommandText = "usp_BoxFirmwareStatusTotal";

                        // @OrganizationID    int = 0,
                        sqlComment.Parameters.Add(new SqlParameter("@OrganizationID", SqlDbType.Int));
                        sqlComment.Parameters["@OrganizationID"].Value = Organization;

                        // @OrganizationID    int = 0,
                        sqlComment.Parameters.Add(new SqlParameter("@FleetID", SqlDbType.Int));
                        sqlComment.Parameters["@FleetID"].Value = Fleet;

                        //@FromDate          datetime,
                        sqlComment.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime));
                        sqlComment.Parameters["@FromDate"].Value = DateTime.Today;

                        //@ToDate            datetime,
                        sqlComment.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime));
                        sqlComment.Parameters["@ToDate"].Value = DateTime.Today.AddDays(1);

                        connection.Open();
                        using (SqlDataReader reader = sqlComment.ExecuteReader())
                        {
                            DataTable dtTotal = new DataTable("Total");

                            dtTotal.Load(reader);

                            this.dgTotal.DataSource = dtTotal;
                            this.dgTotal.DataBind();
                            this.dgTotal.SelectedIndex = 0;
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                strMessage = "Task Total Initial Failed for " + ex.Message;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool InitialOrganizationList()
        {
            try
            {
                this.dlOrgamization.Items.Clear();

                using (SqlConnection connection = new SqlConnection(strConnection))
                {
                    using (SqlCommand sqlComment = new SqlCommand())
                    {
                        sqlComment.CommandTimeout = 600;
                        sqlComment.CommandType = CommandType.StoredProcedure;
                        sqlComment.Connection = connection;
                        sqlComment.CommandText = "usp_BoxFirmwareStatusOrganization";

                        connection.Open();

                        using (SqlDataReader reader = sqlComment.ExecuteReader())
                        {
                            DataTable dt = new DataTable("Detail");

                            dt.Load(reader);

                            this.dlOrgamization.DataSource = dt;
                            this.dlOrgamization.DataBind();
                            this.dlOrgamization.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strMessage = "Organization Initial failed for " + ex.Message;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool InitialOrganizationFleetList(int OrganizationID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(strConnection))
                {
                    using (SqlCommand sqlComment = new SqlCommand())
                    {
                        sqlComment.CommandTimeout = 600;
                        sqlComment.CommandType = CommandType.StoredProcedure;
                        sqlComment.Connection = connection;
                        sqlComment.CommandText = "usp_BoxFirmwareStatusFleet";

                        // @OrganizationID    int = 0,
                        sqlComment.Parameters.Add(new SqlParameter("@OrganizationID", SqlDbType.Int));
                        sqlComment.Parameters["@OrganizationID"].Value = OrganizationID;
                        connection.Open();

                        using (SqlDataReader reader = sqlComment.ExecuteReader())
                        {
                            DataTable dt = new DataTable("Detail");

                            dt.Load(reader);

                            this.dlFleet.DataSource = dt;
                            this.dlFleet.DataBind();
                            this.dlFleet.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strMessage = "Fleet Initial Failed for " + ex.Message;
            }
            return true;
        }

        /// <summary>
        /// Build parameters string in json format 
        /// </summary>
        /// <returns></returns>
        private string InitialReportParameters()      //string ReportName, string ReportPath, string ReportUri, string ReportType)
        {

            StringBuilder sbp = new StringBuilder();

            // Application Logon User
            sbp.Append("userid: " + sn.UserID + ", ");
            // Report's parameters
            sbp.Append("reportid: 10038, ");             // this.cboReports.SelectedItem.Value + ", ");
            sbp.Append("reportformat: EXCEL, ");              // PDF; EXCEL; WORD;....   .SelectedValue.ToString()
            sbp.Append("reportformatcode: 2, ");         // 1;   2;     3;   ....   .SelectedValue.ToString()

            // Credencial Information
            //sbp.Append("username: bsmreports" + ", ");
            //sbp.Append("password: T0ybhARQ" + ", ");
            //sbp.Append("domain: production" + ", ");

            sbp.Append("sensornumber: " + this.dgTotal.SelectedIndex.ToString() + ", ");
            sbp.Append("organization: " + this.dlOrgamization.SelectedItem.Value  + ", ");

            if (this.dlFleet.Enabled)
                sbp.Append("fleetid: " + this.dlFleet.SelectedItem.Value + ", ");
            else
                sbp.Append("fleetid: 0, ");
            
            //DateTime dtFrom = Convert.ToDateTime(sFrom, ci);
            //DateTime dtTo = Convert.ToDateTime(sTo, ci);

            //sbp.Append("datefrom: " + this.txtDateFr.Text + ", ");
            //sbp.Append("dateto: " + this.txtDateTo.Text + ", ");

            return "{" + sbp.ToString() + "}";
            
        }

        private void MessagePopup(string Key, string ScriptType, string ScriptMessage)
        {
            string script = String.Format("<script>{0}('{1}');</script>", ScriptType, ScriptMessage);
            //Page.ClientScript.RegisterStartupScript(this.GetType(), Key, script, true);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "newWindow", String.Format("<script>window.open('{0}');</script>", ScriptMessage));
        }

        #endregion

        #region Assistance Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        private void Alert(string Message)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", String.Format("<script language=JavaScript>alert('{0}')</script>", Message));
        }

        /// <summary>
        /// DataTable to XML String
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        private string ToStringAsXml(DataTable dt)
        {
            StringWriter sw = new StringWriter();
            dt.WriteXml(sw, XmlWriteMode.IgnoreSchema);
            string s = sw.ToString();
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int StringToInt(string value)
        {
            int i = 0;
            if (int.TryParse(value, out i))
                return i;
            else
                return 0;
        }

        #endregion
    }
}