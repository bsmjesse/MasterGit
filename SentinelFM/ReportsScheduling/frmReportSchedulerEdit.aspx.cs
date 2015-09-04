using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;

using Telerik.Web.UI;

using VLF.CLS;
using VLF.CLS.Def;
using VLF.Reports;

namespace SentinelFM
{
    public partial class ReportsScheduling_frmReportSchedulerEdit : ReportSchedulerPage
    {
        //private const string dateFormat = "MM/dd/yyyy";
        private const string DTFORMAT_FR = "dd/MM/yyyy";
        private const string DTFORMAT_EN = "MM/dd/yyyy";

        private string psMessage = string.Empty;
        private int piScheduleID = 0;

        public string Hours = "0";
        public string errresFillStartDate = string.Empty;
        public string errresValidStartDate = string.Empty;
        public string errresFillEndDate = string.Empty;
        public string errdateValidatorResource1 = string.Empty;
        public string ContentWidth = "280";
        public string WindowWidth = "440";
        public int piSelectedScheduleTypeIndex = 0;
        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!PageInitialization())
                {
                    this.cmdSubmit.Enabled = false;
                }
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
            lblMessage.Visible = true;
            int iScheduleNumber = clsUtility.StringToInt(this.txtSchedule.Text);

            if (!ReportScheduleUpdate(iScheduleNumber))
                ShowMessage(this.lblMessage, this.GetLocalResourceObject("lblMessage_Text_ScheduledSuccess").ToString(), Color.Green);
//                ShowMessage(lblMessage, "Report schedule[" + iSchedule.ToString() + "] update successful.", Color.Blue);
            else
                ShowMessage(lblMessage, "Report schedule[" + iScheduleNumber.ToString() + "] update failed.", Color.Red);

            ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "returnToParent('2');", true);
           
        }

        protected void btnOneTimeReport_Click(object sender, EventArgs e)
        {
                ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "returnToParent('1');", true);
                return;
        }

        protected void btnMyReport_Click(object sender, EventArgs e)
        {
            //sn.Report.XmlDOC = clsAsynGenerateReport.MakePair("ReportName", txtMyReportName.Text.Trim()) + clsAsynGenerateReport.MakePair("ReportDescription", txtMyReportDesc.Text.Trim());
            //    ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "returnToParent('3');", true);
            //    return;
        }

        private bool IsValidParameters(int ScheduleID, out string xmlString)
        {
            psMessage = "";
            xmlString = "";

            DateTime SchedStart = clsUtility.StringToDateTime(this.txtFrom.ToString(), DTFORMAT_EN);
            DateTime SchedEnd = clsUtility.StringToDateTime(this.txtTo.ToString(), DTFORMAT_EN);

            Int16 ifreqParam = 0;

            if (this.txtFrom.Visible && this.txtFrom.Enabled && !ValidStartDate(ref SchedStart))
                return false;
            else if (this.txtTo.Visible && this.txtTo.Enabled && !ValidEndDate(ref SchedEnd))
                return false;
            else if (this.txtTo.Visible && this.txtTo.Enabled && this.txtFrom.Visible && this.txtFrom.Enabled && !ValidPeriodDate(this.txtFrom.ToString(), this.txtTo.ToString()))
                return false;
            else
            {
                Int16 iScheduleType = clsUtility.StringToInt16(this.lstReportType.SelectedValue);

                switch (iScheduleType)
                {
                    case 1:                 // 1:Daily
                        break;
                    case 2:                 // 2:Weekly
                        ifreqParam = Convert.ToInt16(this.cboWeekDay.SelectedValue);
                        // start on sched. wday
                        if ((int)SchedStart.DayOfWeek < ifreqParam)
                            SchedStart.AddDays(ifreqParam - (int)SchedStart.DayOfWeek);
                        else // start next week
                            SchedStart.AddDays(7 - (int)SchedStart.DayOfWeek + ifreqParam);
                        break;
                    case 3:                 // 3:Monthly
                        ifreqParam = Convert.ToInt16(this.cboMonthlyDay.Text);
                        // start on sched. day
                        if ((int)SchedStart.Day < ifreqParam)
                            SchedStart.AddDays(ifreqParam - (int)SchedStart.Day);
                        else // start next month
                            SchedStart.AddDays(DateTime.DaysInMonth(SchedStart.Year, SchedStart.Month) - (int)SchedStart.Day + ifreqParam);
                        break;
                    default:                // 0:Onetime
                        break;
                
                }

                clsXmlUtil xmlDoc = new clsXmlUtil();
                //clsXmlUtil xmlDoc = new clsXmlUtil(sn.Report.XmlDOC);
                //Comment out by Devin Begin
                xmlDoc.CreateRoot("Schedule");
                xmlDoc.CreateNode("ScheduleID", ScheduleID);
                xmlDoc.CreateNode("IsFleet", sn.Report.IsFleet);
                xmlDoc.CreateNode("FleetId", sn.Report.FleetId);
                xmlDoc.CreateNode("XmlParams", sn.Report.XmlParams);
                xmlDoc.CreateNode("Email", this.txtEmail.Text);
                xmlDoc.CreateNode("GuiId", sn.Report.GuiId);
                xmlDoc.CreateNode("Status", "New");
                xmlDoc.CreateNode("StatusDate", DateTime.Now);
                xmlDoc.CreateNode("Frequency", iScheduleType);
                xmlDoc.CreateNode("FrequencyParam", ifreqParam);
                xmlDoc.CreateNode("StartDate", SchedStart);
                xmlDoc.CreateNode("EndDate", SchedEnd);
                xmlDoc.CreateNode("DeliveryMethod", Convert.ToInt16(optDeliveryMethod.SelectedItem.Value));
                xmlDoc.CreateNode("ReportLanguage", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                xmlDoc.CreateNode("ReportFormat", sn.Report.ReportFormat); // new 2008 - 05 - 05
                xmlDoc.CreateNode("FromDate", SchedStart);
                xmlDoc.CreateNode("ToDate", SchedEnd);

                if (String.IsNullOrEmpty(xmlDoc.Xml))
                {
                    ShowMessage(lblMessage, this.GetLocalResourceObject("resXMLCreateError").ToString(), Color.Red);
                    return false;
                }
                else
                {
                    sn.Report.XmlDOC = xmlDoc.Xml;
                    xmlString = xmlDoc.Xml;
                    return true;
                }
            }
        }

        /// <summary>
        /// ReportID,  
        /// GUIID,  
        /// GUIName,  
        /// UserID,  
        /// UserName,  
        /// DateFrom:           report period from date
        /// DateTo:             report Period to date 
        /// IsFleet,  
        /// FleetID,  
        /// FleetName,  
        /// Params,  
        /// Email,  
        /// LinkUrl,  
        /// CurrentStatus,  
        /// LastSendDate:       Status Date  
        /// NextSendDate:       Start Scheduled Date  
        /// EndScheduledDate:   End Schedule Date  
        /// ReportFrequency:  
        /// FrequencyParam,  
        /// DeliveryMethod,  
        /// ReportLanguage,  
        /// ReportFormat
        /// </summary>
        /// <returns></returns>
        private bool PageInitialization() 
        {
            try{
                
                sn.Report.ReportAddType = Request.QueryString["back"];

                piScheduleID = clsUtility.StringToInt(Request.QueryString["ScheduleID"].ToString());

                if (piScheduleID > 0)
                {
                    psMessage = "";

                    int SelectedItemIndex = 0;

                    string ScheduleInfo = "";

                    ReportWebService rws = new ReportWebService();

                    if (rws.ReportScheduleRecordUpload(piScheduleID, out ScheduleInfo, out psMessage))
                    {
                        XmlDocument xd = new XmlDocument();

                        xd.LoadXml(ScheduleInfo);

                        XmlNode xn = xd.SelectSingleNode("Schedule");

                        #region Date / Time 

                        this.txtFrom.SelectedDate = DateTime.Now.AddHours(1);
                        this.txtTo.SelectedDate = DateTime.Now.AddMonths(1);

                        txtFrom.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                        txtFrom.DateInput.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;

                        txtTo.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                        txtTo.DateInput.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;

                        if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
                        {
                            txtTo.DateInput.DateFormat = DTFORMAT_FR;
                            txtTo.DateInput.DisplayDateFormat = DTFORMAT_FR;
                            txtTo.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_fr;

                            txtFrom.DateInput.DateFormat = DTFORMAT_FR;
                            txtFrom.DateInput.DisplayDateFormat = DTFORMAT_FR;
                            txtFrom.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_fr;
                        }
                        else
                        {
                            txtTo.DateInput.DateFormat = DTFORMAT_EN;
                            txtTo.DateInput.DisplayDateFormat = DTFORMAT_EN;
                            txtTo.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_en;

                            txtFrom.DateInput.DateFormat = DTFORMAT_EN;
                            txtFrom.DateInput.DisplayDateFormat = DTFORMAT_EN;
                            txtFrom.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_en;
                        }
                        // Initial Schedule Start Date
                        DateTime dtNow = DateTime.Now.AddDays(1);
                        // Set Schedule start date min/max
                        this.txtFrom.MinDate = dtNow;
                        this.txtFrom.MaxDate = dtNow.AddYears(40);
                        // Get next sending date as initial start date
                        DateTime dtStart = clsUtility.StringToDateTime(xmlNodeValue(xn, "LastSendDate"), DTFORMAT_EN);
                        // Save current setting
                        this.txtFrom.ToolTip = dtStart.ToString();
                        // If Next Sending date >= Now + 1 day, display next sending date, else Now + 1 Day
                        if (DateTime.Compare(dtStart, dtNow) < 0) dtStart = dtNow;
                        // Set display start date
                        this.txtFrom.SelectedDate = dtStart;

                        // Initial Schedule End Date
                        this.txtTo.MinDate = dtStart.AddDays(1);          // Now + 2 days
                        this.txtFrom.MaxDate = dtStart.AddYears(40);
                        // Get configured end date as initial end date
                        DateTime dtEnd = clsUtility.StringToDateTime(xmlNodeValue(xn, "EndScheduledDate"), DTFORMAT_EN);
                        // Save current setting
                        this.txtTo.ToolTip = dtEnd.ToString();
                        // If schedule end date < current date, reset start date = current date
                        if (DateTime.Compare(dtEnd, dtStart) < 0) dtEnd = dtStart.AddDays(1);
                        // Set display end date
                        this.txtFrom.SelectedDate = dtEnd;

                        #endregion

                        ContentWidth = "450";
                        WindowWidth = "570";

                        //tblWeekly.Width = ContentWidth;
                        //tblMonthly.Width = ContentWidth;
                        //Table1.Width = ContentWidth;
                        //Table3.Width = ContentWidth;
                        lstReportType.Width = new Unit(ContentWidth);

                        // bind client events
                        //radReportType.Attributes.Add("OnClick", "javascript: EnableSchduleControls('" + radReportType.ClientID + "','fast');");
                        lstReportType.Attributes.Add("OnClick", "javascript: EnableTypeControls('" + lstReportType.ClientID + "');");
                        
                        #region Control value initial

                        this.txtSchedule.Text = piScheduleID.ToString();
                        this.txtReportName.Text = xmlNodeValue(xn, "GUIName");     // GUIName=\"Fleet Violation Details Report\"
                        this.txtReportName.ToolTip = xmlNodeValue(xn, "GUIID");    // GUIID=\"17\"
                        this.txtUser.Text = xmlNodeValue(xn, "UserName");          // UserName=\"hgi_up\"
                        this.txtUser.ToolTip = xmlNodeValue(xn, "UserID");         // UserID=\"8441\"
                        this.txtStatus.Text = xmlNodeValue(xn, "CurrentStatus");   // CurrentStatus=\"Sent\"
                        this.txtEmail.Text = xmlNodeValue(xn, "Email");            // Email=\"tjprice@up.com\"

                        //this.tblYearly.Visible = false;
                        this.cboOccursHour.Items[0].Selected = true;
                        //this.tblMonthly.Visible = false;
                        this.cboMonthlyDay.Items[0].Selected = true;
                        //this.tblWeekly.Visible = false;
                        this.cboWeekDay.Items[0].Selected = true;

                        // Report Formats - ReportFormat = {1:PDF | 2:Excel | 3:Word}
                        SelectedItemIndex = getSelectedItemIndex(xmlNodeValue(xn, "ReportFormat"), 1, rblFormat.Items.Count) - 1;
                        this.rblFormat.Items[SelectedItemIndex].Selected = true;    //ReportFormat=\"1\"

                        // Schedule Types - ReportFrequency = {0:Once | 1:Daily | 2:Weekly | 3:Monthly}
                        SelectedItemIndex = getSelectedItemIndex(xmlNodeValue(xn, "ReportFrequency"), 0, lstReportType.Items.Count - 1);
                        this.lstReportType.Items[SelectedItemIndex].Selected = true;

                        piSelectedScheduleTypeIndex = SelectedItemIndex;

                        int dayIndex = clsUtility.StringToInt(xmlNodeValue(xn, "FrequencyParam"));

                        switch (SelectedItemIndex)
                        {
                            case 2:     // Weekly
                                if (dayIndex < 0 || dayIndex > 6) dayIndex = 6;
                                this.cboWeekDay.Items[dayIndex].Selected = true;
                                break;
                            case 3:     // Monthly
                                if (dayIndex < 1 || dayIndex > 31) dayIndex = 1;
                                this.cboMonthlyDay.Items[dayIndex].Selected = true;
                                break;
                            default:    // Once
                                break;
                        }

                        // Delivery Methods - DeliveryMethod = {0:Email | 1:Disk}
                        SelectedItemIndex = getSelectedItemIndex(xmlNodeValue(xn, "DeliveryMethod"), 0, lstReportType.Items.Count - 1);
                        this.optDeliveryMethod.Items[SelectedItemIndex].Selected = true;

                        #endregion

                        ControlStatusInitial(xmlNodeValue(xn, "CurrentStatus").ToLower());


                    }
                    else{
                
                    }
                }
                else
                {
                }
            }
            catch(Exception ex){
            } 
            finally{
            
            }
            return true;
        }
        //"<Schedule ReportID=\"9250\" DateFrom=\"2013-08-17T00:00:00\" DateTo=\"2013-08-25T00:00:00\" IsFleet=\"1\" FleetID=\"14029\" FleetName=\"EC723 - MGR TRK MTCE - ODESSA\" Params=\"63*75\"  LastSendDate=\"2013-09-16T04:18:12.817\" NextSendDate=\"2013-09-23T00:00:00\" EndScheduledDate=\"2018-05-21T05:00:00\"  ReportLanguage=\"en\"  />"


        private bool ControlStatusInitial(string ScheduleStatus) 
        {
            // No change allowed to schedule type and date time
            this.lstReportType.Enabled = false;
            this.txtFrom.Enabled = false;
            this.cboOccursHour.Enabled = false;
            this.cboMonthlyDay.Enabled = false;
            this.cboWeekDay.Enabled = false;

            if (string.IsNullOrEmpty(ScheduleStatus) || ScheduleStatus.IndexOf("finish") >= 0)
            {
                this.cmdSubmit.Enabled = false;

                this.lstReportType.Enabled = false;

                this.optDeliveryMethod.Enabled = false;
                
                this.rblFormat.Enabled = false;
                
                this.txtSchedule.Enabled = false;
                this.txtReportName.Enabled = false;
                this.txtReportName.Enabled = false;
                this.txtUser.Enabled = false;
                this.txtUser.Enabled = false;
                this.txtStatus.Enabled = false;
                this.txtEmail.Enabled = false;
                this.txtTo.Enabled = false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ReportScheduleUpdate(){
            return ReportScheduleUpdate(clsUtility.StringToInt(this.txtSchedule.Text)); 
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iSchedule"></param>
        /// <returns></returns>
        private bool ReportScheduleUpdate(int iSchedule)
        {
            string xmlParameters = "";

            try
            {
                if (IsValidParameters(iSchedule, out xmlParameters))
                {
                    using (ReportWebService rws = new ReportWebService())
                    {
                        if (rws.ReportScheduleRecordUpDate(sn.UserID, iSchedule, xmlParameters, out psMessage))
                        {
                            psMessage = "";
                            this.cmdSubmit.Enabled = false;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(psMessage)) psMessage = "Failed";
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning,
                            VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Report schedule update failed. User[" + sn.UserID.ToString() + "] Form:frmReportSchedulerEdit.aspx. Exception:" + psMessage));
                        }
                    }
                }
                else
                {
                    psMessage = "Invalid parameters";
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning,
                    VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Report schedule update failed. User[" + sn.UserID.ToString() + "] Form:frmReportSchedulerEdit.aspx. Exception: " + psMessage));
                }
            }
            catch (NullReferenceException Ex)
            {
                psMessage = Ex.Message;
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "returnToParent('-1');", true);
            }
            catch (Exception Ex)
            {
                psMessage = Ex.Message;
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                   Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmReportScheduler.aspx->cmdSubmit_Click"));
                ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "returnToParent('-1');", true);
            }
            finally 
            { 
            
            }
            return (string.IsNullOrEmpty(psMessage));
        }

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
                date = Convert.ToDateTime(this.txtFrom.SelectedDate.Value.ToString(DTFORMAT_EN) + " " + this.cboOccursHour.Text + this.cboOccursHoursType.Text,
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
                txtFrom.SelectedDate  = null;
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
                    date = Convert.ToDateTime(this.txtTo.SelectedDate.Value.ToString(DTFORMAT_EN) + " " + this.cboOccursHour.Text + this.cboOccursHoursType.Text,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datefr"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        private bool ValidPeriodDate(string datefr, string dateTo) 
        {
            if (this.txtFrom.SelectedDate.Value > this.txtTo.SelectedDate.Value)
            {
                ShowMessage(lblMessage, this.GetLocalResourceObject("dateValidatorResource1").ToString(), Color.Red);
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datefr"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        private bool ValidPeriodDate(DateTime datefr, DateTime dateTo)
        {
            if (datefr > dateTo)
            {
                ShowMessage(lblMessage, this.GetLocalResourceObject("dateValidatorResource1").ToString(), Color.Red);
                return false;
            }
            else
                return true;
        }

        private int getSelectedItemIndex(string IndexString, int Lower, int Upper)
        {
            int Index = clsUtility.StringToInt(IndexString);
            return getSelectedItemIndex(Index, Lower, Upper);
        }

        private int getSelectedItemIndex(int Index, int Lower, int Upper)
        {
            if (Index < Lower)
                return Lower;
            else if (Index > Upper)
                return Upper;
            else
                return Index;
        }

        private string xmlNodeValue(XmlNode ParentNode, string ChildNodeName) {
            if (ParentNode.SelectSingleNode(ChildNodeName) != null)
                return ParentNode.SelectSingleNode(ChildNodeName).InnerText;
            else
                return "";
        }

        //private int xmlNodeValue(XmlNode ParentNode, string ChildNodeName)
        //{
        //    return clsUtility.StringToInt(ParentNode.SelectSingleNode(ChildNodeName).InnerText, 0);
        //}
    }
}

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
//if (Request.QueryString["hideSchedule"] != null && Request.QueryString["hideSchedule"].ToString() == "1")
//radReportType.Items.FindByValue("1").Enabled = false;
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