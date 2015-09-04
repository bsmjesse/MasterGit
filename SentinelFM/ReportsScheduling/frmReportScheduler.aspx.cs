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
using VLF.Reports;
using System.IO;
using System.Globalization;
using System.Drawing;

namespace SentinelFM
{
   public partial class ReportsScheduling_frmReportScheduler : ReportSchedulerPage 
   {
      private const string dateFormat = "MM/dd/yyyy";
      
      protected void Page_Load(object sender, EventArgs e)
      {
         try
         {
            if (!Page.IsPostBack)
            {
               if (System.Globalization.CultureInfo.CurrentUICulture.ToString() == "fr-CA")
               {
                  txtFrom.CultureInfo.CultureName = "fr-FR";
                  txtTo.CultureInfo.CultureName = "fr-FR";
               }
               else
               {
                  txtFrom.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();
                  txtTo.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();
               }
               this.txtFrom.Text = DateTime.Now.AddHours(1).ToString(dateFormat);
               this.txtTo.Text = DateTime.Now.AddMonths(1).ToString(dateFormat);
               sn.Report.ReportAddType=Request.QueryString["back"];    
               EnableControls(false, false, false);
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message + " User:" + sn.UserID.ToString() + " Form:frmReportScheduler.aspx->Page_Load"));
            RedirectToLogin();
         }
      }

      /// <summary>
      /// Change type of a report
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void lstReportType_SelectedIndexChanged(object sender, EventArgs e)
      {
         try
         {
            switch (lstReportType.SelectedItem.Value)
            {
               case "0": // once
                  EnableControls(false, false, false);
                  break;
               case "1": // dayly
                  EnableControls(false, false, true);
                  break;
               case "2": // weekly
                  EnableControls(true, false, true);
                  break;
               case "3": // monthly
                  EnableControls(false, true, true);
                  break;
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message + " User:" + sn.UserID.ToString() + " Form:frmReportScheduler.aspx->lstReportType_SelectedIndexChanged"));
            RedirectToLogin();
         }
      }

      /// <summary>
      /// Change control's state
      /// </summary>
      /// <param name="weeklyVisible"></param>
      /// <param name="monthlyVisible"></param>
      /// <param name="endEnabled"></param>
      private void EnableControls(bool weeklyVisible, bool monthlyVisible, bool endEnabled)
      {
         this.tblWeekly.Visible = weeklyVisible;
         this.tblMonthly.Visible = monthlyVisible;
         this.txtTo.Enabled = endEnabled;
         this.lblEnd.Enabled = endEnabled;
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




      protected void cmdSubmit_Click(object sender, EventArgs e)
      {
         try
         {
            
            DateTime SchedStart = new DateTime();
            DateTime SchedEnd = new DateTime();

            Int16 freqType = Convert.ToInt16(this.lstReportType.SelectedValue);
            Int16 freqParam = 0; // weekday or month day

            if (!ValidStartDate(ref SchedStart)) return;
            if (freqType == 0) // once
               SchedEnd = SchedStart;
            else
            {
               if (!ValidEndDate(ref SchedEnd)) return;
               if (Convert.ToDateTime(this.txtFrom.Value) > Convert.ToDateTime(this.txtTo.Value))
                  ShowMessage(lblMessage, this.GetLocalResourceObject("dateValidatorResource1").ToString(), Color.Red);
            }

            if (freqType == 2) // weekly
            {
               freqParam = Convert.ToInt16(this.cboWeekDay.SelectedValue);
               // start on sched. wday
               if ((int)SchedStart.DayOfWeek < freqParam)
                  SchedStart.AddDays(freqParam - (int)SchedStart.DayOfWeek);
               else // start next week
                  SchedStart.AddDays(7 - (int)SchedStart.DayOfWeek + freqParam);
            }
            else
               if (freqType == 3) // monthly
               {
                  freqParam = Convert.ToInt16(this.cboMonthlyDay.Text);
                  // start on sched. day
                  if ((int)SchedStart.Day < freqParam)
                     SchedStart.AddDays(freqParam - (int)SchedStart.Day);
                  else // start next month
                     SchedStart.AddDays(DateTime.DaysInMonth(SchedStart.Year, SchedStart.Month) - (int)SchedStart.Day + freqParam);
               }

            clsXmlUtil xmlDoc = new clsXmlUtil();
            xmlDoc.CreateRoot("Schedule");
            xmlDoc.CreateNode("IsFleet", sn.Report.IsFleet);
            xmlDoc.CreateNode("FleetId", sn.Report.FleetId);
            xmlDoc.CreateNode("XmlParams", sn.Report.XmlParams);
            xmlDoc.CreateNode("Email", this.txtEmail.Text);
            xmlDoc.CreateNode("GuiId", sn.Report.GuiId);
            xmlDoc.CreateNode("Status", "New");
            xmlDoc.CreateNode("StatusDate", DateTime.Now);
            xmlDoc.CreateNode("Frequency", freqType);
            xmlDoc.CreateNode("FrequencyParam", freqParam);
            xmlDoc.CreateNode("StartDate", SchedStart);
            xmlDoc.CreateNode("EndDate", SchedEnd);
            xmlDoc.CreateNode("DeliveryMethod", Convert.ToInt16(optDeliveryMethod.SelectedItem.Value));
            xmlDoc.CreateNode("ReportLanguage", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            xmlDoc.CreateNode("ReportFormat", sn.Report.ReportFormat); // new 2008 - 05 - 05

            if (String.IsNullOrEmpty(xmlDoc.Xml))
            {
               ShowMessage(lblMessage, this.GetLocalResourceObject("resXMLCreateError").ToString(), Color.Red);
               return;
            }

            //if (String.IsNullOrEmpty(sn.Report.XmlParams))
            //{
            //   xmlDoc.CreateNode("FromDate", Convert.ToDateTime(sn.Report.FromDate, new CultureInfo("en-US")));
            //   xmlDoc.CreateNode("ToDate", Convert.ToDateTime(sn.Report.ToDate, new CultureInfo("en-US")));              
            //}
            //else
            //{



               //xmlDoc.CreateNode("FromDate", Convert.ToDateTime(sn.Report.FromDate, new CultureInfo("en-US")).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving));
               //xmlDoc.CreateNode("ToDate", Convert.ToDateTime(sn.Report.ToDate, new CultureInfo("en-US")).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving));



            xmlDoc.CreateNode("FromDate", Convert.ToDateTime(sn.Report.FromDate, new CultureInfo("en-US")));
            xmlDoc.CreateNode("ToDate", Convert.ToDateTime(sn.Report.ToDate, new CultureInfo("en-US")));



            //}

               //using (ServerReports.Reports reportProxy = new ServerReports.Reports())
               //{
                  if (objUtil.ErrCheck(reportProxy.AddReportSchedule(sn.UserID, sn.SecId, xmlDoc.Xml), false))
                     if (objUtil.ErrCheck(reportProxy.AddReportSchedule(sn.UserID, sn.SecId, xmlDoc.Xml), true))
                     {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning,
                           VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning,
                           "Schedule report failed. User: " + sn.UserID.ToString() + " Form:frmReportScheduler.aspx"));

                        ShowMessage(lblMessage, this.GetLocalResourceObject("resScheduleFailed").ToString(), Color.Red);
                        return;
                     }
               //}

            ShowMessage(this.lblMessage, this.GetLocalResourceObject("lblMessage_Text_ScheduledSuccess").ToString(), Color.Green);
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmReportScheduler.aspx->cmdSubmit_Click"));
            RedirectToLogin();
         }
      }

      /// <summary>
      /// Select delivery method
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      //protected void optDeliveryMethod_SelectedIndexChanged(object sender, EventArgs e)
      //{
         //try
         //{
         //   if (this.optDeliveryMethod.SelectedIndex == 0)
         //      this.tblEmail.Visible = true;
         //   else
         //      this.tblEmail.Visible = false;
         //}
         //catch(Exception ex)
         //{
         //   return;
         //}
      //}

      /// <summary>
      /// Validates start date
      /// </summary>
      /// <returns>True if valid</returns>
      private bool ValidStartDate(ref DateTime date)
      {
         lblDateValidation.Text = "";
         if (String.IsNullOrEmpty(txtFrom.Text))
         {
            lblDateValidation.Text = this.GetLocalResourceObject("resFillStartDate").ToString();//"The start date is required";
            txtFrom.Focus();
            return false;
         }
         try
         {
            date = Convert.ToDateTime(this.txtFrom.Text + " " + this.cboOccursHour.Text + this.cboOccursHoursType.Text,
               new System.Globalization.CultureInfo("en-US")).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);
         }
         catch (Exception ex)
         {
            lblDateValidation.Text = this.GetLocalResourceObject("resInvalidStartDate").ToString();//"Invalid start date";
            txtFrom.Text = "";
            txtFrom.Focus();
            return false;
         }

         //if (lstReportType.SelectedIndex > 0)
         //{
            if (date < System.DateTime.Now)
            {
               lblDateValidation.Text = this.GetLocalResourceObject("resValidStartDate").ToString();// "Start Report Life time should be greater that current date/time";
               txtFrom.Text = "";
               txtFrom.Focus();
               return false;
            }
         //}
         return true;
      }

      /// <summary>
      /// Validates end date
      /// </summary>
      /// <returns>True if valid</returns>
      private bool ValidEndDate(ref DateTime date)
      {
         lblDateValidation.Text = "";

         if (Convert.ToInt16(this.lstReportType.SelectedValue) > 0)
         {
            if (String.IsNullOrEmpty(txtTo.Text))
            {
               lblDateValidation.Text = this.GetLocalResourceObject("resFillEndDate").ToString();//"The end date is required";
               txtTo.Focus();
               return false;
            }

            try
            {
               date = Convert.ToDateTime(this.txtTo.Text + " " + this.cboOccursHour.Text + this.cboOccursHoursType.Text, 
                  new System.Globalization.CultureInfo("en-US")).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);
            }
            catch (Exception ex)
            {
               lblDateValidation.Text = this.GetLocalResourceObject("resInvalidEndDate").ToString();//"Invalid end date";
               txtTo.Focus();
               return false;
            }
         }

         return true;
      }
   }
}