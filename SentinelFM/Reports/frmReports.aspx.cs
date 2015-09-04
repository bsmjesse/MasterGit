using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using VLF.ERRSecurity;
using VLF.Reports;
using System.Configuration;
using System.Text;
namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmTripReport.
    /// </summary>
    public partial class frmReports : SentinelFMBasePage
    {
        
        protected System.Web.UI.WebControls.CheckBox chkHistIncludeInvalid;
        protected System.Web.UI.WebControls.DropDownList cboFromHoursSOS;
        protected System.Web.UI.WebControls.DropDownList cboToHoursSOS;
        


        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {

                this.txtFrom.Text = Request[this.txtFrom.UniqueID];
                this.txtTo.Text = Request[this.txtTo.UniqueID];
                int i = 0;
                

                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmTripReport, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    GetUserReportsTypes();
                    clsMisc.cboHoursFill(ref cboHoursFrom);
                    clsMisc.cboHoursFill(ref cboHoursTo);
                    clsMisc.cboHoursFill(ref cboFromDayH);
                    clsMisc.cboHoursFill(ref cboToDayH);
                    clsMisc.cboHoursFill(ref cboWeekEndFromH);
                    clsMisc.cboHoursFill(ref cboWeekEndToH);

                    //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                    //System.Threading.Thread.CurrentThread.CurrentUICulture = ci ;


                    this.txtFrom.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                    this.txtTo.Text = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");


                    //this.txtFrom.Text=DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy hh:mm");
                    //this.txtTo.Text=DateTime.Now.AddDays(1).ToString("MM/dd/yyyy hh:mm");




                    lblReportFormat.Text = (string)base.GetLocalResourceObject("lblReportFormat_Text_Suggested");
                    this.lblReportFormat.Visible = true;

                    this.cboHoursFrom.SelectedIndex = -1;
                    for (i = 0; i <= cboHoursFrom.Items.Count - 1; i++)
                    {
                        if (Convert.ToInt32(cboHoursFrom.Items[i].Value) == 8)
                        {
                            cboHoursFrom.Items[i].Selected = true;
                            break;
                        }
                    }


                    this.cboHoursTo.SelectedIndex = -1;
                    for (i = 0; i <= cboHoursTo.Items.Count - 1; i++)
                    {
                        if (Convert.ToInt32(cboHoursFrom.Items[i].Value) == Convert.ToInt32(DateTime.Now.AddHours(1).Hour.ToString()))
                        {
                            this.cboHoursTo.SelectedIndex = i;
                            break;
                        }
                    }


                    this.tblHistoryOptions.Visible = false;
                    this.tblException.Visible = false;
                    this.tblException1.Visible = false;
                    this.tblStopReport.Visible = false;
                    this.tblViolationReport.Visible = false;
                    this.tblOffHours.Visible = false;
                    this.tblFleetMaintenance.Visible = false;
                    this.cboFromDayH.SelectedIndex = 8;
                    this.cboToDayH.SelectedIndex = 18;
                    this.cboWeekEndFromH.SelectedIndex = 8;
                    this.cboWeekEndToH.SelectedIndex = 18;
                    CboFleet_Fill();


                    if (sn.User.DefaultFleet != -1)
                    {
                        cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                        CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));
                        this.lblVehicleName.Visible = true;
                        this.cboVehicle.Visible = true;
                    }




                    //if (sn.User.OrganizationId == 123)
                    //{
                    //    cboReports.Items.Insert(11, new ListItem("Fleet Utilization Report - Daily", "13"));
                    //    cboReports.Items.Insert(12, new ListItem("Fleet Utilization Report by Vehicle Type", "14"));
                    //    cboReports.Items.Insert(13, new ListItem("Fleet Utilization Report - Weekday", "15"));
                    //    cboReports.Items.Insert(14, new ListItem("Fleet Utilization Report - Weekly", "16"));
                    //}
                    //cboReports.Items.Insert(18, new ListItem("Violation Summary Report", "20"));


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

                this.cboMaintenanceFleet.DataSource = dsFleets;
                this.cboMaintenanceFleet.DataBind();

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

        private void cldTo_SelectionChanged(object sender, System.EventArgs e)
        {
            //this.txtTo.Text=cldTo.SelectedDate.ToShortDateString()    ;  
        }

        private void cldFrom_SelectionChanged(object sender, System.EventArgs e)
        {
            //this.txtFrom.Text=cldFrom.SelectedDate.ToShortDateString() ;
        }


        private void CboVehicle_Fill(int fleetId)
        {
            try
            {
                DataSet dsVehicle;
                dsVehicle = new DataSet();
                
                StringReader strrXML = null;
                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                        RedirectToLogin();
                        return;
                    }
                if (xml == "")
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();

                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
            }
            catch (NullReferenceException Ex)
            {
                // System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }



        protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            {
                this.cboVehicle.Visible = true;
                this.lblVehicleName.Visible = true;
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
            }
        }
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

        }
        #endregion

        protected void cmdShow_Click(object sender, System.EventArgs e)
        {
            try
            {

                string strFromDate = "";
                string strToDate = "";
                string WindowUrl = "";
                this.lblMessage.Text = "";
                


                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
                    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
                    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 0)
                    strFromDate = this.txtFrom.Text + " " + "12:00 AM";

                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
                    strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
                    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
                    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 0)
                    strToDate = this.txtTo.Text + " " + "12:00 AM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
                    strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";


                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

                strFromDate = Convert.ToDateTime(strFromDate, ci).ToString();
                strToDate = Convert.ToDateTime(strToDate, ci).ToString();



                if (Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving) > DateTime.Now)
                    strToDate = DateTime.Now.ToString();


                if (!clsUtility.IsNumeric(this.txtAccExtreme.Text)
                    || !clsUtility.IsNumeric(this.txtAccHarsh.Text)
                    || !clsUtility.IsNumeric(this.txtBrakingExtreme.Text)
                    || !clsUtility.IsNumeric(this.txtBrakingHarsh.Text)
                    || !clsUtility.IsNumeric(this.txtSeatBelt.Text)
                    || !clsUtility.IsNumeric(this.txtSpeed120.Text)
                    || !clsUtility.IsNumeric(this.txtSpeed130.Text)
                    || !clsUtility.IsNumeric(this.txtSpeed140.Text))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_Params");
                    return;
                }

                if (Convert.ToDateTime(strFromDate) >= Convert.ToDateTime(strToDate))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidDate");
                    return;
                }
                else
                {
                    this.lblMessage.Visible = false;
                    this.lblMessage.Text = "";
                }


                if (((this.cboVehicle.SelectedIndex == 0 && this.cboReports.SelectedItem.Value == "3") ||
                    this.cboVehicle.SelectedIndex == -1) && (this.cboReports.SelectedItem.Value != "19"))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SelectVehicle");
                    return;
                }
                else
                {
                    this.lblMessage.Visible = false;
                    this.lblMessage.Text = "";

                }


                string xmlParams = "";



                switch (cboReports.SelectedItem.Value)
                {
                    case "0": //--Trip Report
                        {
                            if (this.cboVehicle.SelectedItem.Value == "0")
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetDetailedTripFirstParamName, this.cboFleet.SelectedItem.Value.ToString());
                            else
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpDetailedTripFirstParamName, this.cboVehicle.SelectedItem.Value.ToString());



                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripSecondParamName : ReportTemplate.RpFleetDetailedTripSecondParamName, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"));
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripThirdParamName : ReportTemplate.RpFleetDetailedTripThirdParamName, Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"));
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripFourthParamName : ReportTemplate.RpFleetDetailedTripFourthParamName, chkIncludeStreetAddress.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripFifthParamName : ReportTemplate.RpFleetDetailedTripFifthParamName, chkIncludeSensors.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripSixthParamName : ReportTemplate.RpFleetDetailedTripSixthParamName, chkIncludePosition.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripSeventhParamName : ReportTemplate.RpFleetDetailedTripSeventhParamName, chkIncludeIdleTime.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripEighthParamName : ReportTemplate.RpFleetDetailedTripEighthParamName, chkIncludeSummary.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripNinthParamName : ReportTemplate.RpFleetDetailedTripNinthParamName, chkShowStorePosition.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripTenthParamName : ReportTemplate.RpFleetDetailedTripTenthParamName, this.optEndTrip.SelectedItem.Value.ToString()    );




                            if (chkIncludeSummary.Checked)
                                WindowUrl = "Report_TripReportData.aspx";
                            else
                                WindowUrl = "Report_TripReportData_NoSummary.aspx";



                            break;
                        }
                    case "1":   //--   Summary Report 
                        {

                            if (this.cboVehicle.SelectedItem.Value == "0")
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetTripFirstParamName, this.cboFleet.SelectedItem.Value.ToString());
                            else
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpTripFirstParamName, this.cboVehicle.SelectedItem.Value.ToString());


                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpTripSecondParamName : ReportTemplate.RpFleetTripSecondParamName, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"));
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpTripThirdParamName : ReportTemplate.RpFleetTripThirdParamName, Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"));
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpTripFourthParamName : ReportTemplate.RpFleetTripFourthParamName, chkShowStorePosition.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpTripFifthParamName : ReportTemplate.RpFleetTripFifthParamName, this.optEndTrip.SelectedItem.Value.ToString());

                            WindowUrl = "Report_TripActivity.aspx";

                            break;
                        }

                    case "2":   //--    Alarms Report
                        {

                            if (this.cboVehicle.SelectedItem.Value == "0")
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetAlarmsFirstParamName, this.cboFleet.SelectedItem.Value.ToString());
                            else
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpAlarmFirstParamName, this.cboVehicle.SelectedItem.Value.ToString());


                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpAlarmSecondParamName : ReportTemplate.RpFleetAlarmsSecondParamName, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"));
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpAlarmThirdParamName : ReportTemplate.RpFleetAlarmsThirdParamName, Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"));



                            if (this.cboVehicle.SelectedItem.Value != "0")
                                WindowUrl = "Report_Alarms.aspx";
                            else
                                WindowUrl = "Report_AlarmsFleet.aspx";

                            break;
                        }

                    case "3": //-- History Report
                        {
                            xmlParams = this.chkHistIncludeCoordinate.Checked.ToString() + ";" + this.chkHistIncludeSensors.Checked.ToString() + ";" + this.chkHistIncludePositions.Checked.ToString() + ";" + this.chkHistIncludeInvalidGPS.Checked.ToString();
                            WindowUrl = "Report_History.aspx";
                            break;
                        }
                    case "4":   //--   Stop Report 
                        {
                            string blnShowsStops = "false";
                            string blnShowsIdles = "false";

                            blnShowsStops = this.optStopFilter.Items[0].Selected.ToString();
                            blnShowsIdles = this.optStopFilter.Items[1].Selected.ToString();
                            if (optStopFilter.Items[2].Selected)
                            {
                                blnShowsStops = "true";
                                blnShowsIdles = "true";
                            }


                            if (this.cboVehicle.SelectedItem.Value == "0")
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetStopFirstParamName, this.cboFleet.SelectedItem.Value.ToString());
                            else
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpStopFirstParamName, this.cboVehicle.SelectedItem.Value.ToString());



                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopSecondParamName : ReportTemplate.RpFleetStopSecondParamName, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"));
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopThirdParamName : ReportTemplate.RpFleetStopThirdParamName, Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"));
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopFourthParamName : ReportTemplate.RpFleetStopFourthParamName, chkShowStorePosition.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopFifthParamName : ReportTemplate.RpFleetStopFifthParamName, this.cboStopSequence.SelectedItem.Value.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopSixthParamName : ReportTemplate.RpFleetStopSixthParamName, blnShowsStops);
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopSeventhParamName : ReportTemplate.RpFleetStopSeventhParamName, blnShowsIdles);
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopEighthParamName : ReportTemplate.RpFleetStopEighthParamName, this.optEndTrip.SelectedItem.Value.ToString());

                            WindowUrl = "Report_StopReport.aspx";
                            break;
                        }

                    case "5": //-- Messages Report
                        {

                            sn.Message.BoxId = 0;
                            if (this.cboVehicle.SelectedItem.Value != "0")
                            {
                                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                                DataSet dsVehicle = new DataSet();

                                string xml = "";
                                if (objUtil.ErrCheck(dbv.GetVehicleInfoXML(sn.UserID, sn.SecId, this.cboVehicle.SelectedItem.Value, ref xml), false))
                                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXML(sn.UserID, sn.SecId, this.cboVehicle.SelectedItem.Value, ref xml), true))
                                    {
                                        return;
                                    }

                                if (xml != "")
                                {
                                    StringReader strrXML = null;
                                    strrXML = new StringReader(xml);
                                    dsVehicle.ReadXml(strrXML);
                                    sn.Message.BoxId = Convert.ToInt32(dsVehicle.Tables[0].Rows[0]["BoxId"]);

                                }
                                else
                                {
                                    return;
                                }
                            }

                            sn.Message.FromDate = strFromDate;
                            sn.Message.ToDate = strToDate;
                            sn.Message.FleetId = Convert.ToInt32(this.cboFleet.SelectedItem.Value);
                            sn.Message.FleetName = this.cboFleet.SelectedItem.Text.ToString();
                            sn.Message.VehicleName = this.cboVehicle.SelectedItem.Text.ToString();

                            xmlParams = "";
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpMessagesFirstParamName, strFromDate);
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpMessagesSecondParamName, strToDate);
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpMessagesThirdParamName, sn.Message.FleetId.ToString());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpMessagesFourthParamName, sn.Message.FleetName.ToString());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpMessagesFifthParamName, sn.Message.BoxId.ToString());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpMessagesSixthParamName, sn.Message.VehicleName.ToString());


                            WindowUrl = "Report_Messages.aspx";
                            break;
                        }
                    case "6":
                        {


                            if (this.cboVehicle.SelectedItem.Value == "0")
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetExceptionFirstParamName, this.cboFleet.SelectedItem.Value.ToString());
                            else
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpExceptionFirstParamName, this.cboVehicle.SelectedItem.Value.ToString());



                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionSecondParamName : ReportTemplate.RpFleetExceptionSecondParamName, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"));
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionThirdParamName : ReportTemplate.RpFleetExceptionThirdParamName, Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"));

                            if (this.cboVehicle.SelectedItem.Value == "0")
                            {
                                xmlParams += ReportTemplate.MakePair(ReportTemplate.RpExceptionFourthParamName, chkSOSMode.Checked ? this.cboSOSLimit.SelectedItem.Value.ToString() : "-1");
                                if (this.chkDriverDoorExc.Checked || this.chkPassengerDoorExc.Checked || this.chkRearHopperDoorExc.Checked || this.chkSideHopperDoorExc.Checked)
                                    xmlParams += ReportTemplate.MakePair(ReportTemplate.RpExceptionFifthParamName, this.cboDoorPeriod.SelectedItem.Value.ToString());
                                else
                                    xmlParams += ReportTemplate.MakePair(ReportTemplate.RpExceptionFifthParamName, "-1");
                            }
                            else
                            {
                                xmlParams += ReportTemplate.MakePair(ReportTemplate.RpFleetExceptionFourthParamName, chkSOSMode.Checked ? this.cboSOSLimit.SelectedItem.Value.ToString() : "-1");
                                if (this.chkDriverDoorExc.Checked || this.chkPassengerDoorExc.Checked || this.chkRearHopperDoorExc.Checked || this.chkSideHopperDoorExc.Checked)
                                    xmlParams += ReportTemplate.MakePair(ReportTemplate.RpFleetExceptionFifthParamName, this.cboDoorPeriod.SelectedItem.Value.ToString());
                                else
                                    xmlParams += ReportTemplate.MakePair(ReportTemplate.RpFleetExceptionFifthParamName, "-1");
                            }


                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionSixthParamName : ReportTemplate.RpFleetExceptionSixthParamName, chkTAR.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionSeventhParamName : ReportTemplate.RpFleetExceptionSeventhParamName, chkImmobilization.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionEightParamName : ReportTemplate.RpFleetExceptionEightParamName, chkDriverDoor.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionNineParamName : ReportTemplate.RpFleetExceptionNineParamName, chkLeash.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTenParamName : ReportTemplate.RpFleetExceptionTenParamName, chkExcBattery.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionElevenParamName : ReportTemplate.RpFleetExceptionElevenParamName, chkExcTamper.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTwelveParamName : ReportTemplate.RpFleetExceptionTwelveParamName, chkExcPanic.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionThirteenParamName : ReportTemplate.RpFleetExceptionThirteenParamName, chkExcKeypad.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionFourteenParamName : ReportTemplate.RpFleetExceptionFourteenParamName, chkExcGPS.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionFifteenParamName : ReportTemplate.RpFleetExceptionFifteenParamName, chkExcAVL.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionSixteenParamName : ReportTemplate.RpFleetExceptionSixteenParamName, chkExcLeash.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionSeventeenParamName : ReportTemplate.RpFleetExceptionSeventeenParamName, chkDriverDoorExc.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionEighteenParamName : ReportTemplate.RpFleetExceptionEighteenParamName, chkPassengerDoorExc.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionNineteenParamName : ReportTemplate.RpFleetExceptionNineteenParamName, chkSideHopperDoorExc.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTwentyParamName : ReportTemplate.RpFleetExceptionTwentyParamName, chkRearHopperDoorExc.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTwentyFirstParamName : ReportTemplate.RpFleetExceptionTwentyFirstParamName, this.chkIncCurTARMode.Checked.ToString());

                            WindowUrl = "Report_Exception.aspx";
                            break;
                        }

                    case "8":  //-- OffHours Report
                        {
                            if (this.cboVehicle.SelectedItem.Value == "0")
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetOffHourFirstParamName, this.cboFleet.SelectedItem.Value.ToString());
                            else
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpOffHourFirstParamName, this.cboVehicle.SelectedItem.Value.ToString());


                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourSecondParamName, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"));
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourThirdParamName, Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"));
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourFourthParamName, chkShowStorePosition.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourFifthParamName, this.cboFromDayH.SelectedItem.Value.ToString());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourSixthParamName, this.cboFromDayM.SelectedItem.Value.ToString());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourSeventhParamName, this.cboToDayH.SelectedItem.Value.ToString());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourEightParamName, this.cboToDayM.SelectedItem.Value.ToString());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourNineParamName, this.cboWeekEndFromH.SelectedItem.Value.ToString());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourTenParamName, this.cboWeekEndFromM.SelectedItem.Value.ToString());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourElevenParamName, this.cboWeekEndToH.SelectedItem.Value.ToString());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourTwelveParamName, this.cboWeekEndToM.SelectedItem.Value.ToString());

                            WindowUrl = "Report_OffHours.aspx";

                            break;
                        }


                    case "9":  //-- Landmark Activity Report
                        {
                            if (this.cboVehicle.SelectedItem.Value == "0")
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetStopFirstParamName, this.cboFleet.SelectedItem.Value.ToString());
                            else
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpStopFirstParamName, this.cboVehicle.SelectedItem.Value.ToString());


                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopSecondParamName : ReportTemplate.RpFleetStopSecondParamName, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"));
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopThirdParamName : ReportTemplate.RpFleetStopThirdParamName, Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"));
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopFourthParamName : ReportTemplate.RpFleetStopFourthParamName, chkShowStorePosition.Checked.ToString());
                            xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopFifthParamName : ReportTemplate.RpFleetStopFifthParamName, cboStopSequence.SelectedItem.Value.ToString());

                            WindowUrl = "Report_LandmarkActivity.aspx";

                            break;
                        }

                    case "11":
                        {
                            WindowUrl = "Report_Utilization.aspx";
                            break;
                        }


                    case "12":
                        {
                            WindowUrl = "Report_UtilizationSummary.aspx";
                            break;
                        }


                    case "13":
                        {
                            WindowUrl = "Report_DailyFleetUtilization.aspx";
                            break;
                        }

                    case "14":
                        {
                            WindowUrl = "Report_VehicleTypeUtilization.aspx";
                            break;
                        }

                    case "15":
                        {
                            WindowUrl = "Report_DailyDetailUtilizationReport.aspx";
                            break;
                        }

                    case "16":
                        {
                            WindowUrl = "Report_WeeklyUtilizationReport.aspx";
                            break;
                        }

                    case "17":
                        {
                            int intCriteria = 0;

                            if (chkSpeedViolation.Checked)
                                intCriteria = intCriteria | VLF.Reports.ReportGenerator.CT_VIOLATION_SPEED;
                            if (chkHarshAcceleration.Checked)
                                intCriteria = intCriteria | VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHACCELERATION;
                            if (chkHarshBraking.Checked)
                                intCriteria = intCriteria | VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHBRAKING;
                            if (chkExtremeAcceleration.Checked)
                                intCriteria = intCriteria | VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEACCELERATION;
                            if (chkExtremeBraking.Checked)
                                intCriteria = intCriteria | VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEBRAKING;
                            if (chkSeatBeltViolation.Checked)
                                intCriteria = intCriteria | VLF.Reports.ReportGenerator.CT_VIOLATION_SEATBELT;

                            xmlParams = intCriteria.ToString();

                            WindowUrl = "Report_Violation.aspx";
                            break;
                        }

                    case "18":
                        {
                            WindowUrl = "Report_IdlingDetailsFleet.aspx";
                            break;
                        }

                    case "19":
                        {
                            WindowUrl = "Report_IdlingSummaryOrg.aspx";
                            break;
                        }

                    case "20":
                        {
                            int intCriteria = 0;

                            if (chkSpeedViolation.Checked)
                                intCriteria = intCriteria | VLF.Reports.ReportGenerator.CT_VIOLATION_SPEED;
                            if (chkHarshAcceleration.Checked)
                                intCriteria = intCriteria | VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHACCELERATION;
                            if (chkHarshBraking.Checked)
                                intCriteria = intCriteria | VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHBRAKING;
                            if (chkExtremeAcceleration.Checked)
                                intCriteria = intCriteria | VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEACCELERATION;
                            if (chkExtremeBraking.Checked)
                                intCriteria = intCriteria | VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEBRAKING;
                            if (chkSeatBeltViolation.Checked)
                                intCriteria = intCriteria | VLF.Reports.ReportGenerator.CT_VIOLATION_SEATBELT;

                            StringBuilder Params = new StringBuilder();

                            Params.Append(intCriteria.ToString());
                            Params.Append(";");

                            Params.Append(this.txtSpeed120.Text);
                            Params.Append(";");

                            Params.Append(this.txtSpeed130.Text);
                            Params.Append(";");

                            Params.Append(this.txtSpeed140.Text);
                            Params.Append(";");

                            Params.Append(this.txtAccExtreme.Text);
                            Params.Append(";");

                            Params.Append(this.txtAccHarsh.Text);
                            Params.Append(";");

                            Params.Append(this.txtBrakingExtreme.Text);
                            Params.Append(";");

                            Params.Append(this.txtBrakingHarsh.Text);
                            Params.Append(";");

                            Params.Append(this.txtSeatBelt.Text);

                            xmlParams = Params.ToString();

                            WindowUrl = "Report_ViolationSummary.aspx";
                            break;
                        }
                }





                string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                strUrl = strUrl + "	var myname='Report';";
                strUrl = strUrl + " var w=800;";
                strUrl = strUrl + " var h=480;";
                strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,scrollbars=1,menubar=1,'; ";
                strUrl = strUrl + " win = window.open(mypage, myname, winprops); }";



                sn.Report.XmlParams = xmlParams;
                sn.Report.FromDate = strFromDate;
                sn.Report.ToDate = strToDate;
                sn.Report.ReportFormat = Convert.ToInt32(this.cboFormat.SelectedItem.Value.ToString());

                if (this.cboReports.SelectedItem.Value != "19")
                {
                    sn.Report.FleetReport = "false";
                    if (this.cboVehicle.SelectedItem.Value == "0")
                        sn.Report.FleetReport = "true";

                    sn.Report.FleetId = Convert.ToInt32(this.cboFleet.SelectedItem.Value);
                    sn.Report.LicensePlate = this.cboVehicle.SelectedItem.Value.ToString();
                    sn.Report.FleetName = this.cboFleet.SelectedItem.Text.ToString();

                }







                //strUrl=strUrl+" NewWindow('frmReportWait.aspx?WindowUrl="+WindowUrl+"');</script>";
                //Response.Write(strUrl) ;


                sn.Report.ReportURL = WindowUrl;
                Response.Redirect("frmReportWait.aspx");
                Response.Redirect(WindowUrl);





                // Response.Redirect("frmReportMain.aspx?WindowUrl="+WindowUrl); 

            }

            catch (NullReferenceException Ex)
            {
                //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }



        private void cboVehicle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.lblMessage.Text = "";
        }

        protected void cboReports_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {


                this.lblTripSummaryReportDesc.Visible = false;
                this.lblHistoryReportDesc.Visible = false;
                this.lblAlarmReportDesc.Visible = false;
                this.lblStopReportDesc.Visible = false;
                this.lblTripReportDesc.Visible = false;
                this.lblLandmarkActivityReportDesc.Visible = false;
                this.lblMessageReportDescription.Visible = false;
                lblOffHoursReportDesc.Visible = false;
                this.tblHistoryOptions.Visible = false;
                this.tblException.Visible = false;
                this.tblException1.Visible = false;
                this.tblOptions1.Visible = false;
                this.tblOptions2.Visible = false;
                this.tblStopReport.Visible = false;
                this.tblOffHours.Visible = false;
                this.tblFleetMaintenance.Visible = false;
                this.tblViolationReport.Visible = false;
                chkShowStorePosition.Visible = false;
                this.tblGeneralCriteria.Visible = true;
                this.cboVehicle.Enabled = true;
                this.cboFleet.Visible = true;
                this.lblFleet.Visible = true;
                this.lblVehicleName.Visible = true;
                this.cboVehicle.Visible = true;
                this.tblPoints.Visible = false;
                this.lblIdlingDetailsReportDesc.Visible = false;
                this.lblIdlingSummaryReportDesc.Visible = false;
                this.lblFleetViolationDetailsReportDesc.Visible = false;
                this.lblFleetViolationSummaryReportDesc.Visible = false;
                this.lblFleetMaintenanceReportDesc.Visible = false;
                tblIgnition.Visible = false;
   
                if (cboReports.SelectedItem.Value.ToString() == "0")
                {
                    this.tblOptions1.Visible = true;
                    this.tblOptions2.Visible = true;
                    chkShowStorePosition.Visible = true;
                    this.lblTripReportDesc.Visible = true;
                    this.tblIgnition.Visible = true;  
                }

                if (cboReports.SelectedItem.Value.ToString() == "3")
                    this.tblHistoryOptions.Visible = true;

                if ((cboReports.SelectedItem.Value.ToString() == "4") || (cboReports.SelectedItem.Value.ToString() == "1"))
                    this.chkShowStorePosition.Visible = true;

                 if (cboReports.SelectedItem.Value.ToString() == "1")
                 {
                    this.lblTripSummaryReportDesc.Visible = true;
                    this.tblIgnition.Visible = true;  
                 }

                if (cboReports.SelectedItem.Value.ToString() == "2")
                    this.lblAlarmReportDesc.Visible = true;

                if (cboReports.SelectedItem.Value.ToString() == "3")
                    this.lblHistoryReportDesc.Visible = true;

                if (cboReports.SelectedItem.Value.ToString() == "4")
                {
                    this.tblStopReport.Visible = true;
                    this.lblStopReportDesc.Visible = true;
                    this.tblIgnition.Visible = true;  
                }

                if (cboReports.SelectedItem.Value.ToString() == "5")
                {
                    this.lblMessageReportDescription.Visible = true;
                    lblReportFormat.Text = (string)base.GetLocalResourceObject("lblReportFormat_Text_Suggested");
                    this.lblReportFormat.Visible = true;
                }
                else
                {
                    lblReportFormat.Text = (string)base.GetLocalResourceObject("lblReportFormat_Text_SuggestedLandscape");
                    this.lblReportFormat.Visible = true;
                }



                if (cboReports.SelectedItem.Value.ToString() == "6")
                {
                    this.tblException.Visible = true;
                    this.tblException1.Visible = true;
                }


                if (cboReports.SelectedItem.Value.ToString() == "8")
                {
                    this.tblHistoryOptions.Visible = true;
                    this.tblOffHours.Visible = true;
                    lblOffHoursReportDesc.Visible = true;
                }


                if (cboReports.SelectedItem.Value.ToString() == "9")
                {
                    this.lblLandmarkActivityReportDesc.Visible = true;
                }

                if (cboReports.SelectedItem.Value.ToString() == "10")
                {
                    this.tblGeneralCriteria.Visible = false;
                    this.tblFleetMaintenance.Visible = true;
                    this.lblFleetMaintenanceReportDesc.Visible = true;  
                }


                if ((cboReports.SelectedItem.Value.ToString() == "11") || (cboReports.SelectedItem.Value.ToString() == "12") || (cboReports.SelectedItem.Value.ToString() == "13") || (cboReports.SelectedItem.Value.ToString() == "14") || (cboReports.SelectedItem.Value.ToString() == "15") || (cboReports.SelectedItem.Value.ToString() == "16") || (cboReports.SelectedItem.Value.ToString() == "18"))
                {
                    this.cboVehicle.Enabled = false;
                }


                if (cboReports.SelectedItem.Value.ToString() == "17") 
                {
                    this.tblViolationReport.Visible = true;
                    this.cboVehicle.Enabled = false;
                    this.lblFleetViolationDetailsReportDesc.Visible = true;   
                }

                if (cboReports.SelectedItem.Value.ToString() == "18")
                {
                   this.lblIdlingDetailsReportDesc.Visible = true;   
                }

                if (cboReports.SelectedItem.Value.ToString() == "19")
                {
                    this.cboFleet.Visible = false;
                    this.cboFleet.Visible = false;
                    this.lblFleet.Visible = false;
                    this.lblVehicleName.Visible = false;
                    this.cboVehicle.Visible = false;
                    this.lblIdlingSummaryReportDesc.Visible = true ;  
                }

                if (cboReports.SelectedItem.Value.ToString() == "20")
                {
                   this.tblViolationReport.Visible = true;
                   this.cboVehicle.Enabled = false;
                   this.tblPoints.Visible = true;
                   this.lblFleetViolationSummaryReportDesc.Visible = true;  
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
        }

        private void chkHistIncludeInvalidGPS_CheckedChanged(object sender, System.EventArgs e)
        {
            if (chkHistIncludeInvalidGPS.Checked)
            {
                this.chkHistIncludeCoordinate.Checked = true;
                this.chkHistIncludeCoordinate.Enabled = false;
            }
            else
            {
                this.chkHistIncludeCoordinate.Enabled = true;
            }
        }

       


        private void GuiSecurity(System.Web.UI.Control obj)
        {

            foreach (System.Web.UI.Control ctl in obj.Controls)
            {
                try
                {
                    if (ctl.HasControls())
                        GuiSecurity(ctl);

                    System.Web.UI.WebControls.Button CmdButton = (System.Web.UI.WebControls.Button)ctl;
                    bool CmdStatus = false;
                    if (CmdButton.CommandName != "")
                    {
                        CmdStatus = sn.User.ControlEnable(sn, Convert.ToInt32(CmdButton.CommandName));
                        CmdButton.Enabled = CmdStatus;
                    }

                }
                catch
                {
                }
            }
        }

        private void GetUserReportsTypes()
        {
            string xml = "";
            
            ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();

            if (objUtil.ErrCheck(dbs.GetUserReportsByLang(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
               if (objUtil.ErrCheck(dbs.GetUserReportsByLang(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
               {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No GUI Controls setttngs for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                  return;
               }

  

            if (xml == "")
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "No GUI Controls settings for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                return;
            }

            StringReader strrXML = new StringReader(xml);
            DataSet ds = new DataSet();
            ds.ReadXml(strrXML);
            this.cboReports.DataSource = ds;
            this.cboReports.DataBind();
        }

        private void chkShowStorePosition_CheckedChanged(object sender, System.EventArgs e)
        {

        }



        protected void chkWeekend_CheckedChanged(object sender, System.EventArgs e)
        {
            this.cboWeekEndToH.SelectedIndex = 0;
            this.cboWeekEndFromH.SelectedIndex = 0;

            if (chkWeekend.Checked)
            {
                this.cboWeekEndToM.Enabled = false;
                this.cboWeekEndToH.Enabled = false;
                this.cboWeekEndFromM.Enabled = false;
                this.cboWeekEndFromH.Enabled = false;
            }
            else
            {
                this.cboWeekEndToM.Enabled = true;
                this.cboWeekEndToH.Enabled = true;
                this.cboWeekEndFromM.Enabled = true;
                this.cboWeekEndFromH.Enabled = true;
            }
        }


        protected void cmdPreviewFleetMaintenanceReport_Click(object sender, EventArgs e)
        {

            string xmlParams = "";
            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetMaintenanceFirstParamName, this.cboMaintenanceFleet.SelectedItem.Value.ToString());

            string WindowUrl = "Report_FleetMaintenance.aspx";

            sn.Report.XmlParams = xmlParams;
            sn.Report.FleetId = Convert.ToInt32(this.cboMaintenanceFleet.SelectedItem.Value);
            sn.Report.FleetName = this.cboMaintenanceFleet.SelectedItem.Text;
            sn.Report.ReportFormat = Convert.ToInt32(this.cboFleetReportFormat.SelectedItem.Value.ToString());

            sn.Report.ReportURL = WindowUrl;
            Response.Redirect("frmReportWait.aspx");



        }

        protected void cboVehicle_SelectedIndexChanged1(object sender, EventArgs e)
        {

        }

      

    }
}
