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
using Telerik.Web.UI;
using System.Collections.Generic;
using VLF.CLS;
using VLF.CLS.Def;

namespace SentinelFM
{
    public partial class ReportsScheduling_frmReportScheduler_new : ReportSchedulerPage
    {
        private const string dateFormat = "MM/dd/yyyy";
        public string Hours = "0";
        public string errresFillStartDate = string.Empty;
        public string errresValidStartDate = string.Empty;
        public string errresFillEndDate = string.Empty;
        public string errdateValidatorResource1 = string.Empty;
        public string errEmailLengthValidatorResource1 = string.Empty;       
        public string errEmailValidatoreWithEmailTextResource1 = string.Empty;
        public string errInvalidEmailTextResource1 = string.Empty;
        public string errValidEmailValidatoreResource1 = string.Empty;
        public string ContentWidth = "280";
        public string WindowWidth = "500";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    //if (System.Globalization.CultureInfo.CurrentUICulture.ToString() == "fr-CA")
                    //{
                    //    txtFrom.CultureInfo.CultureName = "fr-FR";
                    //    txtTo.CultureInfo.CultureName = "fr-FR";
                    //}
                    //else
                    //{
                    //    txtFrom.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();
                    //    txtTo.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();
                    //}

                    //Hide schedule if report is Fleet Maintenance Report 
                    if (Request.QueryString["hideSchedule"] != null && Request.QueryString["hideSchedule"].ToString() == "1")
                        radReportType.Items.FindByValue("1").Enabled = false;
                    txtFrom.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                    txtTo.Culture = System.Globalization.CultureInfo.CurrentUICulture;

                    this.txtFrom.SelectedDate = DateTime.Now.AddHours(1);
                    this.txtTo.SelectedDate = DateTime.Now.AddMonths(1);
                    sn.Report.ReportAddType = Request.QueryString["back"];
                    //EnableControls(false, false, false);
                    radReportType.Attributes.Add("OnClick", "javascript: EnableSchduleControls('" + radReportType.ClientID + "','fast');");
                    lstReportType.Attributes.Add("OnClick", "javascript: EnableTypeControls('" + lstReportType.ClientID + "');");

                    txtFrom.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                    txtFrom.DateInput.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;
                    txtTo.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                    txtTo.DateInput.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;
                    if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
                    {
                        txtTo.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_fr;
                        txtFrom.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_fr;
                        ContentWidth = "450";
                        WindowWidth = "570";
                    }
                    tblWeekly.Width = ContentWidth;
                    tblMonthly.Width = ContentWidth;
                    Table1.Width = ContentWidth;
                    Table3.Width = ContentWidth;
                    lstReportType.Width = new Unit(ContentWidth);

                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "returnToParent('-1');", true);

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                   Ex.Message + " User:" + sn.UserID.ToString() + " Form:frmReportScheduler.aspx->Page_Load"));
                ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "returnToParent('-1');", true);

            }


            try //For date client validation
            {
                Hours = (-sn.User.TimeZone - sn.User.DayLightSaving).ToString();
            }
            catch (Exception ex)
            { }
            // cvDate.ValidationGroup = "vgSchedule";
            errresFillStartDate = this.GetLocalResourceObject("resFillStartDate").ToString();
            errresValidStartDate = this.GetLocalResourceObject("resValidStartDate").ToString();
            errresFillEndDate = this.GetLocalResourceObject("resFillEndDate").ToString();
            errdateValidatorResource1 = this.GetLocalResourceObject("dateValidatorResource1").ToString();
            errEmailLengthValidatorResource1 = this.GetLocalResourceObject("emailLengthValidatorResource1").ToString();           
            errEmailValidatoreWithEmailTextResource1 = this.GetLocalResourceObject("emailValidatoreWithEmailTextResource1").ToString();
            errInvalidEmailTextResource1 = this.GetLocalResourceObject("InvalidEmailTextResource1").ToString();
            errValidEmailValidatoreResource1 = this.GetLocalResourceObject("validEmailValidatoreResource1").ToString();

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

            ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "returnToParent('0');", true);
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
                    if (this.txtFrom.SelectedDate.Value > this.txtTo.SelectedDate.Value)
                    {
                        ShowMessage(lblMessage, this.GetLocalResourceObject("dateValidatorResource1").ToString(), Color.Red);
                        return;
                    }
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
                //Comment out by Devin Begin
                xmlDoc.CreateRoot("Schedule");
                //xmlDoc.CreateNode("IsFleet", sn.Report.IsFleet);
                //xmlDoc.CreateNode("FleetId", sn.Report.FleetId);
                //xmlDoc.CreateNode("XmlParams", sn.Report.XmlParams);
                xmlDoc.CreateNode("Email", this.txtEmail.Text);
                //xmlDoc.CreateNode("GuiId", sn.Report.GuiId);
                xmlDoc.CreateNode("Status", "New");
                xmlDoc.CreateNode("StatusDate", DateTime.Now);
                xmlDoc.CreateNode("Frequency", freqType);
                xmlDoc.CreateNode("FrequencyParam", freqParam);
                xmlDoc.CreateNode("StartDate", SchedStart);
                xmlDoc.CreateNode("EndDate", SchedEnd);
                xmlDoc.CreateNode("DeliveryMethod", Convert.ToInt16(optDeliveryMethod.SelectedItem.Value));
                xmlDoc.CreateNode("ReportLanguage", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                //xmlDoc.CreateNode("ReportFormat", sn.Report.ReportFormat); // new 2008 - 05 - 05
                //End
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


                //Comment by devin to be added in the future
                //xmlDoc.CreateNode("FromDate", Convert.ToDateTime(sn.Report.FromDate, new CultureInfo("en-US")));
                //xmlDoc.CreateNode("ToDate", Convert.ToDateTime(sn.Report.ToDate, new CultureInfo("en-US")));
                //End

                //Comment by devin
                //if (objUtil.ErrCheck(reportProxy.AddReportSchedule(sn.UserID, sn.SecId, xmlDoc.Xml), false))
                //    if (objUtil.ErrCheck(reportProxy.AddReportSchedule(sn.UserID, sn.SecId, xmlDoc.Xml), true))
                //    {
                //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning,
                //           VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning,
                //           "Schedule report failed. User: " + sn.UserID.ToString() + " Form:frmReportScheduler.aspx"));

                //        ShowMessage(lblMessage, this.GetLocalResourceObject("resScheduleFailed").ToString(), Color.Red);
                //        return;
                //    }
                //
                //ShowMessage(this.lblMessage, this.GetLocalResourceObject("lblMessage_Text_ScheduledSuccess").ToString(), Color.Green);
                sn.Report.XmlDOC = xmlDoc.Xml;
                ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "returnToParent('2');", true);

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "returnToParent('-1');", true);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                   Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmReportScheduler.aspx->cmdSubmit_Click"));
                ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "returnToParent('-1');", true);
            }
        }

        protected void btnOneTimeReport_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "returnToParent('1');", true);
            return;
        }

        protected void btnMyReport_Click(object sender, EventArgs e)
        {
            sn.Report.XmlDOC = clsAsynGenerateReport.MakePair("ReportName", txtMyReportName.Text.Trim()) + clsAsynGenerateReport.MakePair("ReportDescription", txtMyReportDesc.Text.Trim());
            ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "returnToParent('3');", true);
            return;
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
            if (txtFrom.SelectedDate == null)
            {
                lblDateValidation.Text = this.GetLocalResourceObject("resFillStartDate").ToString();//"The start date is required";
                txtFrom.Focus();
                return false;
            }
            try
            {
                date = Convert.ToDateTime(this.txtFrom.SelectedDate.Value.ToString(dateFormat) + " " + this.cboOccursHour.Text + this.cboOccursHoursType.Text,
                   new System.Globalization.CultureInfo("en-US")).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);
            }
            catch (Exception ex)
            {
                lblDateValidation.Text = this.GetLocalResourceObject("resInvalidStartDate").ToString();//"Invalid start date";
                txtFrom.SelectedDate = null;
                txtFrom.Focus();
                return false;
            }

            //if (lstReportType.SelectedIndex > 0)
            //{
            if (date < System.DateTime.Now)
            {
                lblDateValidation.Text = this.GetLocalResourceObject("resValidStartDate").ToString();// "Start Report Life time should be greater that current date/time";
                txtFrom.SelectedDate = null;
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
                if (txtTo.SelectedDate == null)
                {
                    lblDateValidation.Text = this.GetLocalResourceObject("resFillEndDate").ToString();//"The end date is required";
                    txtTo.Focus();
                    return false;
                }

                try
                {
                    date = Convert.ToDateTime(this.txtTo.SelectedDate.Value.ToString(dateFormat) + " " + this.cboOccursHour.Text + this.cboOccursHoursType.Text,
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
        protected void optDeliveryMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (optDeliveryMethod.SelectedValue == "1")
            {
                txtEmail.Enabled = false;
                txtEmail.Visible = false;
                lblEmail.Visible = false;
                EmailSuggestions.Visible = false;
                //lblSuggestionMultipleEmail.Visible = false;
                //lblSuggestionEmailLength.Visible = false;
            }
            else
            {
                txtEmail.Enabled = true;
                lblEmail.Visible = true;
                txtEmail.Visible = true;
                EmailSuggestions.Visible = true;
               // lblSuggestionMultipleEmail.Visible = true;
               // lblSuggestionEmailLength.Visible = true;
            }
        }
    }
}
