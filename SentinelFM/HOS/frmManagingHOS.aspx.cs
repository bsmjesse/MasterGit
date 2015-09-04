using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using HOS_DBTableAdapters;
using SentinelFM;
using System.Net;
using System.ComponentModel;
using VLF.ERRSecurity;
using System.Text;
using System.Configuration;
using HOS_WS;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using Telerik.Web.UI;
using System.Data;
using System.Data.SqlClient;

public partial class frmManagingHOS : SentinelFMBasePage
{
    byte[] currentFileData = null;
    private string hosConnectionString = ConfigurationManager.ConnectionStrings["SentinelHOSConnectionString"].ConnectionString;
    public string resizeScript = "";
    public string showFilter = "Show Filter";
    public string hideFilter = "Hide Filter";
    protected SentinelFMSession sn = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        //string filePath = "\\201111\\9d771cb0-ad5a-4213-a211-34f84cc536dd.pdf";
        //DownloadFile(filePath);
        sn = (SentinelFMSession)Session["SentinelFMSession"];
        this.lblMessage.Visible = false;
        //this.iframe.Visible = false;

        string DriverName = Request.QueryString["DriverName"];

        if (sn.User.LoadVehiclesBasedOn == "hierarchy")
        {
            lblFleet.Visible = false;
            FleetColumn.Visible = false;
            cboFleet.Visible = false;
            FleetVehicle1.Visible = true;
            lblFromTitle.Width = 63;
            lblDriver.Width = 63;
        }
        else
        {
            lblFleet.Visible = true;
            FleetColumn.Visible = true;
            cboFleet.Visible = true;
            FleetVehicle1.Visible = false;
            chkDefaultNode.Visible = false;

        }

        if (!Page.IsPostBack)
        {
            this.txtFrom.SelectedDate = DateTime.Now;
            //this.txtFrom.SelectedDate = Convert.ToDateTime(DateTime.Now.ToString(sn.User.DateFormat+" "+sn.User.TimeFormat));
            this.txtTo.SelectedDate = DateTime.Now;
            LogSheetsGrid.Visible = false;
            using (GetDriversTableAdapter getDrivers = new GetDriversTableAdapter())
            {
                cboDrivers.DataSource = getDrivers.GetDrivers(sn.User.OrganizationId);
                CboFleet_Fill();
                //getDrivers.GetDrivers(134);            
                cboDrivers.DataBind();
                dgErrorLog.Visible = false;

                if (!string.IsNullOrEmpty(DriverName))
                {
                    cboDrivers.Items.FindByText(DriverName).Selected = true;
                }
            }
        }
        //resizeScript = SetResizeScript(dgErrorLog.ClientID);
    }
    public byte[] CurrentFileData
    {
        get
        {
            return this.currentFileData;
        }
    }

    byte[] ConvertToJPG(byte[] srcByte)
    {
        if (srcByte == null)
        {
            return null;
        }
        byte[] ret = null;

        using (MemoryStream ms = new MemoryStream(srcByte))
        {
            using (System.Drawing.Image img = System.Drawing.Image.FromStream(ms))
            {
                MemoryStream msDest = new MemoryStream();
                img.Save(msDest, System.Drawing.Imaging.ImageFormat.Jpeg);
                ret = msDest.ToArray();
                msDest.Dispose();
            }
        }

        return ret;
    }

    private string DownloadFile(string filepath)
    {
        string result = string.Empty;
        try
        {
            HOSFileReteriver ws = new HOSFileReteriver();
            byte[] test = null;


            if (filepath != null && filepath.ToLower().EndsWith("tif"))
            {
                string ImagePath = ConfigurationManager.AppSettings["RapidLogImageFolder"] + filepath;
                if (File.Exists(ImagePath))
                {
                    FileStream fs = null;
                    fs = File.OpenRead(ImagePath);
                    test = new byte[fs.Length];
                    fs.Read(test, 0, Convert.ToInt32(fs.Length));
                    fs.Dispose();
                    test = ConvertToJPG(test);
                }
            }
            else
            {
                test = ws.ReadFile(filepath);
            }

            if (test != null)
            {
                //this.currentFileData = test;
                Session["CurrentPDFFileData"] = test;
                //this.iframe.Visible = true;
                // Response.Write("<html><body><iframe src=\"./HOSPDF.aspx\" width=\"100%\" height=\"1000\">");
                //Response.Write("<p>Your browser does not support iframes.</p>");
                //Response.Write("</iframe></body></html>");
                //Response.Buffer = false; //transmitfile self buffers
                //Response.Clear();
                //Response.ClearContent();
                //Response.ClearHeaders();
                //Response.ContentType = "application/pdf";
                ////Response.OutputStream.Write(test, 0, test.Length);
                ////Response.Flush();      
                //Response.OutputStream.Write(test, 0, test.Length);           
                result = "Success";
            }
            // if (File.Exists(ConfigurationManager.AppSettings["HOS_PDFFilePath"] + filepath))
            //{
            //    Response.Buffer = false; //transmitfile self buffers
            //    Response.Clear();
            //    Response.ClearContent();
            //    Response.ClearHeaders();
            //    Response.ContentType = "application/pdf";
            //    byte[] test = File.ReadAllBytes(ConfigurationManager.AppSettings["HOS_PDFFilePath"] + filepath);
            //    //+ path);
            //    Response.OutputStream.Write(test, 0, test.Length);
            //    Response.Flush();
            //    result="Success";
            //    //Response.WriteFile(@"\\192.168.9.45\UploadingService\pdffolder" + path);             
            //    // Create an instance of WebClient
            //    //WebClient client = new WebClient();
            //    //client.UseDefaultCredentials = true;
            //    //string name = Path.GetFileName(path);
            //    // Hookup DownloadFileCompleted Event
            //    //client.DownloadFileCompleted +=
            //    //     new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            //    // Start the download and copy the file to c:\temp
            //    //client.DownloadFileAsync(new Uri(@"\\192.168.9.45\UploadingService\pdffolder"+fname), @"c:\temp\" + fname);
            //    //client.DownloadFile(new Uri(@"\\192.168.9.45\UploadingService\pdffolder" + path), @"c:\temp\" + name);
            //    //resultFilePath = @"c:\temp\" + name;
            //}
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            result = string.Empty;
        }
        return result;
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        FleetVehicle1.OnOkClick += ActionControl_OnOkClick;
    }

    protected void ActionControl_OnOkClick()
    {
        if (FleetVehicle1.GetSelectedFleet() == "" && sn.User.LoadVehiclesBasedOn == "hierarchy")
        {
            validateMultiSelection();
        }
        else
        {
            if (Convert.ToInt32(FleetVehicle1.GetSelectedFleet()) != -1)
            {
                int selectedHeirarchy = (Convert.ToInt32(FleetVehicle1.GetSelectedFleet()));
            }
        }
    }
    private bool validateMultiSelection()
    {
        bool result = true;

        this.lblMessage.Visible = true;
        this.lblMessage.Text = "Please select only one node.";


        return !result;
    }

    private void CboFleet_Fill()
    {
        try
        {

            DataSet dsFleets = new DataSet();
            dsFleets = sn.User.GetUserFleets(sn);
            cboFleet.DataSource = dsFleets;
            cboFleet.DataBind();
            cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            ExceptionLogger(trace);

        }
    }



    protected void cmdViewData_Click(object sender, EventArgs e)
    {
        //tblErrorLog.Visible = false;
        dgErrorLog.Visible = false;
        tbllogsheet.Visible = true;

        bool valid = ValidateInputs();
        LogSheetsGrid.Visible = true;
        if (valid)
        {
            try
            {
                DateTime to = txtTo.SelectedDate.Value;                
                DateTime from = txtFrom.SelectedDate.Value;
                //GetReportLogSheet_ByDriverTableAdapter logSheetsAdapter = new GetReportLogSheet_ByDriverTableAdapter();
                //LogSheetsGrid.DataSource = logSheetsAdapter.GetLogSheets(sn.User.OrganizationId, from, to, cboDrivers.SelectedValue);

                clsHOSManager hosManager = new clsHOSManager();
                DataSet dt = hosManager.GetReportLogSheetByDriver(sn.User.OrganizationId, from, to, cboDrivers.SelectedValue);
                LogSheetsGrid.DataSource = dt;
                //(134, from, from, cboDrivers.SelectedValue);                    
                LogSheetsGrid.DataBind();

                if (LogSheetsGrid.Rows.Count == 0)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "No logsheets found for given selection!";
                    LogSheetsGrid.Visible = false;
                }
                else
                {
                    GridViewRow selectedRow = LogSheetsGrid.Rows[0];
                    Label lblFilePath = (Label)selectedRow.FindControl("lblFilePath");
                    if (lblFilePath != null)
                    {
                        DownloadFile(lblFilePath.Text);
                    }
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
    }


    private bool ValidateInputs()
    {
        bool result = true;
        try
        {
            if (this.txtFrom.SelectedDate == null
            || this.txtFrom.SelectedDate == null)
            {
                //Response.Write("<script type='text/javascript'>window.alert('Error: Please select from and to date range for logsheets.')</script>");
                this.lblMessage.Visible = true;
                this.lblMessage.Text = "Please select date for logsheets.";
                return !result;
            }
            if (string.IsNullOrEmpty(cboDrivers.SelectedValue))
            {
                //Response.Write("<script type='text/javascript'>window.alert('Error: Please select any driver or use all logsheet button.')</script>");
                this.lblMessage.Visible = true;
                this.lblMessage.Text = "Please select any driver or use all logsheet button.";
                return !result;
            }
            DateTime to = txtTo.SelectedDate.Value;
            DateTime from = txtFrom.SelectedDate.Value;


            TimeSpan ts = to - from;
            if (ts.Days > 31)
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = "Please decrease report date range! A maximum of 31 days allowed";
                return !result;
            }


            //DateTime to = Convert.ToDateTime(txtTo.Text);
            //DateTime from = Convert.ToDateTime(txtFrom.Text);
            //int compare = from.CompareTo(to);
            //if (compare > 0)
            //{
            //    //Response.Write("<script type='text/javascript'>window.alert('Error: From date cant be greater than to date.')</script>");
            //    this.lblMessage.Visible = true;
            //    this.lblMessage.Text = "Start date cant be greater than end date.";
            //    return !result;                
            //}
            // compare = to.CompareTo(DateTime.Today);
            // if (compare > 0)
            // {
            //     //Response.Write("<script type='text/javascript'>window.alert('Error: You cannot select future date for toDate field.')</script>");
            //     this.lblMessage.Visible = true;
            //     this.lblMessage.Text = "You cannot select future date for end date.";
            //     return !result;
            // }
        }
        catch (Exception ex)
        {
            return !result;
        }
        return result;
    }

    protected void cmdViewAllData_Click(object sender, EventArgs e)
    {
        //tblErrorLog.Visible = false;
        dgErrorLog.Visible = false;
        tbllogsheet.Visible = true;
        bool check = chkDefaultNode.Checked;

        bool valid = ValidateInputs();
        if (valid)
        {
            LogSheetsGrid.Visible = true;
            try
            {
                //DateTime to = DateTime.Now;
                //Convert.ToDateTime(txtTo.Text);
                DateTime from = txtFrom.SelectedDate.Value;                
                GetReportLogSheetTableAdapter logSheetsAdapter = new GetReportLogSheetTableAdapter();
                int selectedFleet;
                if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                {
                    if (check)
                    {
                        if (!(FleetVehicle1.GetSelectedFleet() == ""))
                        {
                            selectedFleet = (Convert.ToInt32(FleetVehicle1.GetSelectedFleet()));
                        }
                        else
                            selectedFleet = (Convert.ToInt32(FleetVehicle1.DefaultOrganizationHierarchyFleetId));
                    }
                    else
                        selectedFleet = -1;
                }
                else
                {
                    if (sn.User.LoadVehiclesBasedOn == "fleet")
                    {
                        selectedFleet = Convert.ToInt32(cboFleet.SelectedValue);
                    }
                    else
                        selectedFleet = -1;
                }

                DateTime to = txtTo.SelectedDate.Value;

                clsHOSManager hosManager = new clsHOSManager();
                DataSet dt = hosManager.GetReportLogSheet(sn.User.OrganizationId, from, to, selectedFleet);
                LogSheetsGrid.DataSource = dt;


                //LogSheetsGrid.DataSource = logSheetsAdapter.GetLogSheets(sn.User.OrganizationId, from, to);
                //logSheetsAdapter.GetLogSheets(134, from, to);  
                 
                LogSheetsGrid.DataBind();

                if (LogSheetsGrid.Rows.Count == 0)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "No logsheets found for given selection!";
                    LogSheetsGrid.Visible = false;
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
        else
        {
            LogSheetsGrid.Visible = false;
        }
    }


    protected void phlInspections_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Inspection")
        {
            string downloadResult = DownloadFile(e.CommandArgument.ToString());
            if (string.IsNullOrEmpty(downloadResult))
            {
                //Response.Write("<script type='text/javascript'>window.alert('Error in downloading! File not found on server')</script>");                                                  
                this.lblMessage.Visible = true;
                this.lblMessage.Text = "Error in downloading! File not found on server.";
                LogSheetsGrid.Visible = false;
            }
            else
            {
                //string scriptString = "document.getElementById('" + iframe.ClientID + "').contentWindow.location.reload(true)";
                //string scriptString = "document.getElementById('" + iframe.ClientID + "').src='./HOSPDF.aspx'";

               // ClientScript.RegisterStartupScript(this.GetType(), "Startup", scriptString);
            }

        }

    }

    protected void LogSheetsGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow selectedRow = ((GridView)sender).Rows[index];
            Label lblFilePath = (Label)selectedRow.FindControl("lblFilePath");
            //Label lblFilePathInspect 
            if (lblFilePath != null)
            {
                if (e.CommandName.CompareTo("FileDownload") == 0)
                {
                    string downloadResult = DownloadFile(lblFilePath.Text);
                    if (string.IsNullOrEmpty(downloadResult))
                    {
                        //Response.Write("<script type='text/javascript'>window.alert('Error in downloading! File not found on server')</script>");                                                  
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = "Error in downloading! File not found on server.";
                        LogSheetsGrid.Visible = false;
                    }
                    else
                    {
                        //string scriptString = "document.getElementById('" + iframe.ClientID + "').contentWindow.location.reload(true)";
                        //string scriptString = "document.getElementById('" + iframe.ClientID + "').src='./HOSPDF.aspx'";
                        //ClientScript.RegisterStartupScript(this.GetType(), "Startup", scriptString);
                    }
                }
            }

        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        }
    }

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
    }
    protected void dgErrorLog_NeedDataSource(object sender, EventArgs e)
    {
        BindErrorlog(false);
    }


    private DataSet GetErrorlog()
    {
        DataSet dataSet = new DataSet();
        try
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            try
            {
                SqlConnection connection = new SqlConnection(hosConnectionString);
                adapter.SelectCommand = new SqlCommand();
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.SelectCommand.CommandText = "GetDeviceErrorLogs";
                adapter.SelectCommand.Connection = connection;

                SqlParameter sqlPara = new SqlParameter("@OrganizationID", SqlDbType.VarChar, 100);
                sqlPara.Value = sn.User.OrganizationId.ToString();
                adapter.SelectCommand.Parameters.Add(sqlPara);

                DateTime from = txtFrom.SelectedDate.Value;
                DateTime to = txtTo.SelectedDate.Value;

                sqlPara = new SqlParameter("@Logtime1", SqlDbType.NVarChar, 20);
                sqlPara.Value = String.Format("{0:yyyyMMddHHmmss}", from);
                adapter.SelectCommand.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@Logtime2", SqlDbType.NVarChar, 20);
                sqlPara.Value = String.Format("{0:yyyyMMddHHmmss}", to.AddHours(24));
                adapter.SelectCommand.Parameters.Add(sqlPara);


                adapter.Fill(dataSet);
            }
            catch (Exception e)
            {
                lblMessage.Text = "Falied to load data.";
                this.lblMessage.Visible = true;
                return null;
            }
            if (dataSet.Tables.Count <= 0 || dataSet.Tables[0].Rows.Count <= 0)
            {
                lblMessage.Text = "";
                this.lblMessage.Visible = true;
                return dataSet;
            }
        }
        catch (Exception ex) { }

        return dataSet;
    }

    protected void btnErrorLog_Click(object sender, EventArgs e)
    {
        lblMessage.Text = "";
        dgErrorLog.Visible = true;
        tblErrorLog.Attributes["style"] ="border-collapse:collapse;visibility:block;";
        BindErrorlog(true);
    }

    private void BindErrorlog(Boolean isBind)
    {
        DataSet ds = GetErrorlog();
        if (ds != null)
        {
            dgErrorLog.DataSource = ds;
            if (isBind) dgErrorLog.DataBind();
            tbllogsheet.Visible = false;
            tblErrorLog.Visible = true;
        }
    }

    protected void phlInspections_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.FindControl("lnkInspection") != null)
        {
            LinkButton lnkInspection = (LinkButton)e.Row.FindControl("lnkInspection");
            DataRowView dr = (DataRowView)e.Row.DataItem;
            if (dr["Color"] != null && dr["Color"].ToString() == "1")
            {
                lnkInspection.ForeColor = System.Drawing.Color.Red;
            }
        }
    }

    protected void LogSheetsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {       
        
        if (e.Row.FindControl("phlInspections") != null)
        {
            GridView phlInspections = (GridView)e.Row.FindControl("phlInspections");
            DataRowView dr = (DataRowView)e.Row.DataItem;            
            
            if (dr["Inspection"] != null)
            {
                string inspection = dr["Inspection"].ToString();
                string refId = dr["RefId"].ToString();
                string[] instpections = inspection.Split(',');
                DataTable dt = new DataTable();
                dt.Columns.Add("Inspection");
                dt.Columns.Add("FileName");
                dt.Columns.Add("Color");
                dt.Columns.Add("Time");
                dt.Columns.Add("RefId");
                foreach (string insp in instpections)
                {
                    if (!string.IsNullOrEmpty(insp))
                    {
                        try
                        {
                            string[] inspDetail = insp.Split('#');
                            DataRow newdr = dt.NewRow();
                            newdr["Inspection"] = inspDetail[0] + " " + inspDetail[1];
                            newdr["Time"] = inspDetail[1];
                            newdr["FileName"] = inspDetail[2];
                            newdr["Color"] = inspDetail[3];
                            newdr["RefId"] = refId;
                            dt.Rows.Add(newdr);
                        }
                        catch (Exception ex) { }
                    }
                }
                dt.DefaultView.Sort = "Time";
                phlInspections.DataSource = dt.DefaultView;
                phlInspections.DataBind();

            }
        }
    }

    protected void txtFrom_Load(object sender, EventArgs e)
    {
        txtFrom.DateInput.DateFormat = sn.User.DateFormat;
        txtTo.DateInput.DateFormat = sn.User.DateFormat;
    }
}
