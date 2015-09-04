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
using System.IO;
using VLF.CLS;
namespace SentinelFM
{
   public partial class ReportsScheduling_frmScheduleReportList : ReportSchedulerPage
   {
      //string confirm;
      //private ServerReport.Reports reportProxy = new ServerReport.Reports();
       
      protected void Page_Load(object sender, EventArgs e)
      {
         try
         {
            if (!Page.IsPostBack)
            {
               DgReports_Fill();
               //DgStoredReports_Fill();
               sn.Report.ReportAddType=Request.QueryString["back"];    
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, 
               Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      /// <summary>
      /// Fill datagrid of scheduled reports
      /// </summary>
      private void DgReports_Fill()
      {
         try
         {
            DataSet dsReports = new DataSet();
            string xml = "";
            
            //using (ServerReport.Reports reportProxy = new ServerReport.Reports())
            //{
               if (objUtil.ErrCheck(reportProxy.GetScheduledReportsByUserID(sn.UserID, sn.SecId, ref xml), false))
                  if (objUtil.ErrCheck(reportProxy.GetScheduledReportsByUserID(sn.UserID, sn.SecId, ref xml), true))
                  {
                     this.lblNoDataMessage.Visible = true;
                     return;
                  }
            //}
            if (String.IsNullOrEmpty(xml))
            {
               this.dgReports.DataSource = null;
               this.dgReports.DataBind();
               this.lblNoDataMessage.Visible = true;
               this.lblScheduledReports.Visible = false;
               return;
            }

            dsReports.ReadXml(new StringReader(xml));
            if (!Util.IsDataSetValid(dsReports))
            {
               this.dgReports.DataSource = null;
               this.dgReports.DataBind();
               this.lblNoDataMessage.Visible = true;
               this.lblScheduledReports.Visible = false;
               return;
            }

            this.dgReports.DataSource = dsReports.Tables[0];
            this.dgReports.DataBind();
            this.lblScheduledReports.Visible = true;
            this.lblNoDataMessage.Visible = false;

         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      /// <summary>
      /// Fill datagrid of scheduled reports stored in files
      /// </summary>
      private void DgStoredReports_Fill()
      {
         try
         {
            DataSet dsReports = new DataSet();
            
            string xml = "";

            //using (ServerReport.Reports reportProxy = new ServerReport.Reports())
            //{
               if (objUtil.ErrCheck(reportProxy.GetReportFilesByUserID(sn.UserID, sn.SecId, ref xml), false))
                  if (objUtil.ErrCheck(reportProxy.GetReportFilesByUserID(sn.UserID, sn.SecId, ref xml), true))
                  {
                     return;
                  }
            //}

            if (String.IsNullOrEmpty(xml))
            {
               this.dgStoredreports.DataSource = null;
               this.dgStoredreports.DataBind();
               this.lblStoredReports.Visible = false;
               return;
            }

            dsReports.ReadXml(new StringReader(xml));
            if (!Util.IsDataSetValid(dsReports))
            {
               this.dgStoredreports.DataSource = null;
               this.dgStoredreports.DataBind();
               this.lblStoredReports.Visible = false;
               return;
            }

            this.dgStoredreports.DataSource = dsReports.Tables[0];
            this.dgStoredreports.DataBind();
            this.lblStoredReports.Visible = true;

         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      protected void dgReports_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
      {
         dgReports.CurrentPageIndex = e.NewPageIndex;
         DgReports_Fill();
         dgReports.SelectedIndex = -1;
      }

      protected void dgReports_ItemCreated(object sender, DataGridItemEventArgs e)
      {
         /*
         try
         {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
               if (e.Item.Cells[6].Text.Contains("Error") || e.Item.Cells[6].Text.Contains("error"))
                  e.Item.ForeColor = System.Drawing.Color.Red;
               else 
                  e.Item.ForeColor = System.Drawing.Color.Black;

               LinkButton deleteBtn = (LinkButton)e.Item.Cells[7].Controls[0];
               deleteBtn.Attributes.Add("onclick", 
                  String.Format("return confirm('{0}')", base.GetLocalResourceObject("Text_ConfirmDelete")));
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            RedirectToLogin();
         }
         */
      }

      protected void dgReports_DeleteCommand(object source, DataGridCommandEventArgs e)
      {
         try
         {
             
            //using (ServerReport.Reports reportProxy = new ServerReport.Reports())
            //{
               if (objUtil.ErrCheck(reportProxy.DeleteScheduledReportByReportID(sn.UserID, sn.SecId, Convert.ToInt32(dgReports.DataKeys[e.Item.ItemIndex].ToString())), false))
                  if (objUtil.ErrCheck(reportProxy.DeleteScheduledReportByReportID(sn.UserID, sn.SecId, Convert.ToInt32(dgReports.DataKeys[e.Item.ItemIndex].ToString())), true))
                  {
                     return;
                  }
            //}

            dgReports.SelectedIndex = -1;

            dgReports.CurrentPageIndex = 0;
            DgReports_Fill();
            //confirm = "";
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      protected void dgStoredreports_DeleteCommand(object source, DataGridCommandEventArgs e)
      {
         int rowid = 0;
         try
         {
            
            rowid = Convert.ToInt32(dgStoredreports.DataKeys[e.Item.ItemIndex]);

            //using (ServerReport.Reports reportProxy = new ServerReport.Reports())
            //{
               if (objUtil.ErrCheck(reportProxy.DeleteReportFileByRowID(sn.UserID, sn.SecId, rowid), false))
                  if (objUtil.ErrCheck(reportProxy.DeleteReportFileByRowID(sn.UserID, sn.SecId, rowid), true))
                  {
                     return;
                  }
            //}

            dgStoredreports.SelectedIndex = -1;
            dgStoredreports.CurrentPageIndex = 0;
            DgStoredReports_Fill();
            //confirm = "";
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message + " User:" + sn.UserID.ToString() + "RowID: " + rowid.ToString() + " Form:"+Page.GetType().Name));
            //RedirectToLogin();
         }
      }

      protected void cmdBack_Click(object sender, EventArgs e)
      {
             if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 999955)
          {
              Response.Redirect("../Reports/frmReportMaster_hr.aspx");

          }
          else
          {
           if (sn.Report.ReportAddType != "" && sn.Report.ReportAddType != null)
              Response.Redirect("../Reports/" + sn.Report.ReportAddType);
           else
              Response.Redirect("../Reports/frmReportMaster.aspx");
	  }
      }

      protected void dgReports_ItemDataBound(object sender, DataGridItemEventArgs e)
      {
         try
         {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
               if (e.Item.Cells[6].Text.Contains("Error") || e.Item.Cells[6].Text.Contains("error"))                  
                  e.Item.ForeColor = System.Drawing.Color.Red;
               else 
                  e.Item.ForeColor = System.Drawing.Color.Black;

               LinkButton deleteBtn = (LinkButton)e.Item.Cells[7].Controls[0];
               deleteBtn.Attributes.Add("onclick",
                  String.Format("return confirm('{0}')", base.GetLocalResourceObject("Text_ConfirmDelete")));
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.StackTrace.ToString()));
            RedirectToLogin();
         }


         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            
         }
      }

      protected void dgStoredreports_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
      {
         dgStoredreports.CurrentPageIndex = e.NewPageIndex;
         DgStoredReports_Fill();
         dgStoredreports.SelectedIndex = -1;
      }

      protected void dgReports_SelectedIndexChanged(object sender, EventArgs e)
      {
         try
         {
            string xml = "";
            
            //using (ServerReport.Reports reportProxy = new ServerReport.Reports())
            //{
               if (objUtil.ErrCheck(reportProxy.GetReportFilesByReportID(sn.UserID, Convert.ToInt32(dgReports.DataKeys[dgReports.SelectedIndex].ToString()), sn.SecId, ref xml), false))
                  if (objUtil.ErrCheck(reportProxy.GetReportFilesByReportID(sn.UserID, Convert.ToInt32(dgReports.DataKeys[dgReports.SelectedIndex].ToString()), sn.SecId, ref xml), true))
                  {
                     return;
                  }
            //}

            if (String.IsNullOrEmpty(xml))
               return;

            DataSet dsResult = new DataSet("ReportFiles");
            dsResult.ReadXml(new StringReader(xml));

            this.lblStoredReports.Visible = true;
            if (!Util.IsDataSetValid(dsResult))
            {
               this.lblNoFilesMessage.Visible = true;
               this.dgStoredreports.DataSource = null;
               this.dgStoredreports.DataBind();
            }
            else
            {
               this.lblNoFilesMessage.Visible = false;
               this.dgStoredreports.DataSource = dsResult.Tables[0];
               this.dgStoredreports.DataBind();
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
         finally
         {
            dgReports.SelectedIndex = -1;
            dgReports.CurrentPageIndex = 0;
         }
      }

      protected void dgStoredreports_ItemDataBound(object sender, DataGridItemEventArgs e)
      {
         try
         {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
               //confirm = String.Format("return confirm('{0}')", base.GetLocalResourceObject("Text_ConfirmDelete"));
               LinkButton deleteBtn = (LinkButton)e.Item.Cells[2].Controls[0];
               deleteBtn.Attributes.Add("onclick",
                  String.Format("return confirm('{0}')", base.GetLocalResourceObject("Text_ConfirmDelete")));
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            
         }
      }
     
      protected void dgStoredreports_ItemDataBound1(object sender, DataGridItemEventArgs e)
      {
          if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
          {
              if (e.Item.Cells[2].Text == "0") //Email Type
                  e.Item.Cells[3].Visible = false;
              else
                  e.Item.Cells[3].Visible = true;

          }
      }
}
}