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
using VLF.CLS;
using VLF.Reports;

public partial class Reports_HOS_Report : SentinelFMBasePage
{
    DateTime from, to;
     protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            int DriverId = Convert.ToInt32(Util.PairFindValue(ReportTemplate.RptParamDriverId, sn.Report.XmlParams));
            if (DriverId != 0)
            {
                hideHeaders();
                from = Convert.ToDateTime(sn.Report.FromDate);
                to = Convert.ToDateTime(sn.Report.ToDate);
                GetLogSheets(sn.User.OrganizationId, from, to, DriverId.ToString(), false);
            }
            else
            {
                showHeaders();      
                GetDriversTableAdapter getDrivers = new GetDriversTableAdapter();
                cboDrivers.DataSource = getDrivers.GetDrivers(sn.User.OrganizationId);
                ////getDrivers.GetDrivers(134);            
                cboDrivers.DataBind();
            }
        }
    }

     private void showHeaders()
     {
         LogSheetsGrid.Visible = false;
         cboDrivers.Visible = true;
         cmdViewAllData.Visible = true;
         cmdViewData.Visible = true;
     }

     private void hideHeaders()
     {
         cboDrivers.Visible = false;
         LogSheetsGrid.Visible = false;
         cmdViewAllData.Visible = false;
         cmdViewData.Visible = false;
         lblFromTitle.Visible = false;
         lblToTitle.Visible = false;
         lblDriver.Visible = false;
         txtFrom.Visible = false;
         txtTo.Visible = false;         
     }

     private string DownloadFile(string path)
     {
         string result = string.Empty;
         try
         {
             if (File.Exists(ConfigurationManager.AppSettings["HOS_PDFFilePath"] + path))
             {
                 Response.Buffer = false; //transmitfile self buffers
                 Response.Clear();
                 Response.ClearContent();
                 Response.ClearHeaders();
                 Response.ContentType = "application/pdf";
                 byte[] test = File.ReadAllBytes(ConfigurationManager.AppSettings["HOS_PDFFilePath"] + path);
                 //+ path);
                 Response.OutputStream.Write(test, 0, test.Length);
                 Response.Flush();
                 result = "Success";
                 //Response.WriteFile(@"\\192.168.9.45\UploadingService\pdffolder" + path);             
                 // Create an instance of WebClient
                 //WebClient client = new WebClient();
                 //client.UseDefaultCredentials = true;
                 //string name = Path.GetFileName(path);
                 // Hookup DownloadFileCompleted Event
                 //client.DownloadFileCompleted +=
                 //     new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                 // Start the download and copy the file to c:\temp
                 //client.DownloadFileAsync(new Uri(@"\\192.168.9.45\UploadingService\pdffolder"+fname), @"c:\temp\" + fname);
                 //client.DownloadFile(new Uri(@"\\192.168.9.45\UploadingService\pdffolder" + path), @"c:\temp\" + name);
                 //resultFilePath = @"c:\temp\" + name;
             }
         }
         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             result = string.Empty;
         }
         return result;
     }

     protected void GetLogSheets(int orgId, DateTime fromDate, DateTime toDate, string driverID,bool all)
     {
         if (!all)
         {
             GetReportLogSheet_ByDriverTableAdapter logSheetsAdapter = new GetReportLogSheet_ByDriverTableAdapter();
             LogSheetsGrid.DataSource = logSheetsAdapter.GetLogSheets(orgId, fromDate, toDate, driverID);
             //logSheetsAdapter.GetLogSheets(134, from, to, cboDrivers.SelectedValue);                        
             LogSheetsGrid.DataBind();
         }
         else
         {
             GetReportLogSheetTableAdapter logSheetsAdapter = new GetReportLogSheetTableAdapter();
             LogSheetsGrid.DataSource = logSheetsAdapter.GetLogSheets(sn.User.OrganizationId, from, to);
             //logSheetsAdapter.GetLogSheets(134, from, to);                                 
         }
         LogSheetsGrid.DataBind();
         LogSheetsGrid.Visible = true;
     }

    protected void cmdViewData_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(cboDrivers.SelectedValue))
        {
            this.txtFrom.Text = Request[this.txtFrom.UniqueID];
            this.txtTo.Text = Request[this.txtTo.UniqueID];
            try
            {
                if (!string.IsNullOrEmpty(this.txtFrom.Text))
                {
                    from = Convert.ToDateTime(this.txtFrom.Text);
                }
                if (!string.IsNullOrEmpty(this.txtTo.Text))
                {
                    to = Convert.ToDateTime(this.txtTo.Text);
                }
           
                    GetLogSheets(sn.User.OrganizationId, from, to, cboDrivers.SelectedValue,false);
            }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                }            
        }
        else
        {
            //Select driver first
        }
    }

    protected void cmdViewAllData_Click(object sender, EventArgs e)
    {
        this.txtFrom.Text = Request[this.txtFrom.UniqueID];
        this.txtTo.Text = Request[this.txtTo.UniqueID];            
            try
            {
                if (!string.IsNullOrEmpty(this.txtFrom.Text))
                {
                    from = Convert.ToDateTime(this.txtFrom.Text);
                }
                if (!string.IsNullOrEmpty(this.txtTo.Text))
                {
                    to = Convert.ToDateTime(this.txtTo.Text);
                }                
                GetLogSheets(sn.User.OrganizationId, from, to, cboDrivers.SelectedValue, true);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }        
    }
    protected void LogSheetsGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    { 
        try
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow selectedRow = LogSheetsGrid.Rows[index];
                //LinkButton fileButton = (LinkButton)selectedRow.Cells[3].Controls[0];
                //string filePath = selectedRow.Cells[3].Text;
                Label lblFilePath = (Label)selectedRow.FindControl("lblFilePath");
                if (lblFilePath!= null)
                {
                     if (e.CommandName.CompareTo("FileDownload") == 0)
                    {
                     string downloadResult = DownloadFile(lblFilePath.Text);
                     if (string.IsNullOrEmpty(downloadResult))
                     {
                         //newFilePath="Error in downloading";
                         Response.Write("<script type='text/javascript'>window.alert('Error in downloading! File not found on server')</script>");                               
                     }
                     //fileButton.Text=newFilePath;
                    }
                }
            }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        }
    }  
}