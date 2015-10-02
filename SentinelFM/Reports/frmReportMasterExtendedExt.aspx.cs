using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using VLF.ERRSecurity;
using VLF.Reports;
using System.Configuration;
using System.Text;
using VLF.CLS;
using System.Web.Services;

namespace SentinelFM
{
    public partial class Reports_frmReportMasterExtendedExt : SentinelFMBasePage
    {
        //Devin Begin
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        public string LandmarkMap = "../GeoZone_Landmarks/frmMap.aspx?FormName=Landmark";
        public string drawPolygonText = "Please draw a Rectangle or Polygon.";
        public string emailText = "Please enter emails.";
        public string executeSucceed = "Your report is in process. The report will be emailed to you once it is done. ";
        string notcircleLandmarkText = "Non-Circular Landmark";
        //end

        //
        //protected System.Web.UI.WebControls.CheckBox chkHistIncludeInvalid;
        //protected System.Web.UI.WebControls.DropDownList cboFromHoursSOS;
        //protected System.Web.UI.WebControls.DropDownList cboToHoursSOS;


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                sn.Report.ReportActiveTab = 1;



                //Show Busy Message
                cmdShow.Attributes.Add("onclick", BusyReport.ShowFunctionCall);

                this.BusyReport.Title = (string)base.GetLocalResourceObject("BusyPleaseWaitMessage");
                this.BusyReport.Text = (string)base.GetLocalResourceObject("BusyPreparingMessage");



                int i = 0;

                if (!Page.IsPostBack)
                {
                    //Devin Begin
                    FindExistingPreference();
                    this.lblUnit.Text = ViewState["UnitName"].ToString();
                    pnlWorkSite.Visible = false;
                    lstAddOptions.SelectedIndex = 0;
                    lstAddOptions_SelectedIndexChanged(null, null);

                    //End


                    try
                    {
                        if (Request[this.txtFrom.UniqueID] != null)
                        {
                            this.txtFrom.SelectedDate = Convert.ToDateTime(Request[this.txtFrom.UniqueID]);
                            this.txtTo.SelectedDate = Convert.ToDateTime(Request[this.txtTo.UniqueID]);
                        }
                    }
                    catch
                    {
                        if (!String.IsNullOrEmpty(sn.Report.FromDate))
                            this.txtFrom.SelectedDate = Convert.ToDateTime(sn.Report.FromDate);
                        if (!String.IsNullOrEmpty(sn.Report.ToDate))
                            this.txtTo.SelectedDate = Convert.ToDateTime(sn.Report.ToDate);
                    }


                    if (System.Globalization.CultureInfo.CurrentUICulture.ToString() == "fr-CA")
                    {
                        txtFrom.Culture = new CultureInfo("fr-FR");
                        txtTo.Culture = new CultureInfo("fr-FR"); ;
                    }
                    else
                    {
                        txtFrom.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                        txtTo.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                    }



                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmReportMaster, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);

                    //Commented and added by Devin
                    //GetUserReportsTypes();
                    cboReports.Items.Add(new ListItem("Street Lookup", "64"));
                    cboReports.SelectedIndex = cboReports.Items.Count - 1;
                    try
                    {
                        //cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindByValue(sn.Report.GuiId.ToString()));
                    }
                    catch
                    {
                    }


                    if (sn.Report.FromDate != "")
                        this.txtFrom.SelectedDate = Convert.ToDateTime(sn.Report.FromDate);
                    else
                        this.txtFrom.SelectedDate = DateTime.Now;


                    if (sn.Report.ToDate != "")
                        this.txtTo.SelectedDate = Convert.ToDateTime(sn.Report.ToDate);
                    else
                        this.txtTo.SelectedDate = DateTime.Now.AddDays(1);




                    this.tblSpeedViolation.Visible = false;
                    this.tblCost.Visible = true;
                    this.tblFilter.Visible = false;
                    this.lblLandmarkCaption.Visible = false;
                    this.ddlLandmarks.Visible = false;
                    this.tblPoints.Visible = false;

                    CboFleet_Fill();
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));

                    if ((sn.Report.FleetId != 0) && (sn.Report.FleetId != -1))
                    {
                        cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.Report.FleetId.ToString()));
                        CboVehicle_Fill(Convert.ToInt32(sn.Report.FleetId));

                        if (sn.Report.LicensePlate != "")
                            cboVehicle.SelectedIndex = cboVehicle.Items.IndexOf(cboVehicle.Items.FindByValue(sn.Report.LicensePlate.ToString()));

                        this.lblVehicleName.Visible = true;
                        this.cboVehicle.Visible = true;
                    }
                    else if (sn.User.DefaultFleet != -1)
                    {
                        cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                        CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));
                        this.lblVehicleName.Visible = true;
                        this.cboVehicle.Visible = true;
                    }
                    else
                    {
                        this.lblVehicleName.Visible = false;
                        this.cboVehicle.Visible = false;
                    }



                    //cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindByValue(sn.Report.GuiId.ToString()));

                    ReportCriteria();

                    // if (sn.User.OrganizationId == 1)
                    // {
                    //cboReports.Items.Add(new ListItem("Fleet Monthly Utilization Report", "42"));
                    //cboReports.Items.Add(new ListItem("Average Days Utilized Per Vehicle", "43"));
                    //cboReports.Items.Add(new ListItem("Average Service Hrs for Utilized Vehicle", "44"));
                    //cboReports.Items.Add(new ListItem("Average Engine ON Hrs for Utilized Vehicle", "45"));
                    //cboReports.Items.Add(new ListItem("Unnecessary Idling Hrs. Per Vehicle Per Month", "46"));
                    //cboReports.Items.Add(new ListItem("Total Unnecessary Idling Fuel Costs", "47"));
                    //cboReports.Items.Add(new ListItem("Average Travelled Distance ", "48"));
                    //cboReports.Items.Add(new ListItem("Geozone Report", "22"));
                    //cboReports.Items.Insert(13, new ListItem("Fleet Utilization Report - Weekday", "15"));
                    //cboReports.Items.Insert(14, new ListItem("Fleet Utilization Report - Weekly", "16"));
                    // }

                    if (sn.User.OrganizationId == 570)
                    {
                        cboReports.Items.Add(new ListItem("Vehicle Status Report (HeartBeat)", "69"));
                        //cboReports.Items.Add(new ListItem("Worksite Activity Report - July", "70"));
                        //cboReports.Items.Add(new ListItem("Timesheet Validation Details Report - July", "71"));
                    }

                    //devin begin
                    if (cboReports.SelectedValue == "64")
                    {
                        cboReports_SelectedIndexChanged(null, null);
                        //cboReports.Visible = false;
                        pnlHide.Visible = false;
                    }
                    //end

                }
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

        private void CboFleet_Fill()
        {
            try
            {

                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();
                cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));


                if (sn.Misc.DsReportAllFleets != null && sn.Misc.DsReportAllFleets.Tables.Count > 0 && sn.Misc.DsReportAllFleets.Tables[0].Rows.Count > 0)
                {
                    sn.Misc.DsReportAllFleets.Tables[0].DefaultView.Sort = "FleetName";
                    this.lstUnAss.DataSource = sn.Misc.DsReportAllFleets.Tables[0].DefaultView;
                    lstUnAss.DataBind();
                }
                else
                {
                    dsFleets.Tables[0].DefaultView.Sort = "FleetName";
                    this.lstUnAss.DataSource = dsFleets.Tables[0].DefaultView;
                    lstUnAss.DataBind();
                    sn.Misc.DsReportAllFleets = dsFleets;
                }

                if (sn.Misc.DsReportSelectedFleets != null && sn.Misc.DsReportSelectedFleets.Tables.Count > 0)
                {
                    this.lstAss.DataSource = sn.Misc.DsReportSelectedFleets.Tables[0].DefaultView;
                    lstAss.DataBind();
                }


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

        private void CboVehicle_Fill(int fleetId)
        {
            try
            {
                cboVehicle.Items.Clear();

                DataSet dsVehicle = new DataSet();

                string xml = "";

                using (ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet())
                {
                    if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                        if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
                            return;
                        }
                }

                if (String.IsNullOrEmpty(xml))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
                    return;
                }

                dsVehicle.ReadXml(new StringReader(xml));

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();

                //cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
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

        //By Devin Begin
        private void FindExistingPreference()
        {
            if (sn.User.UnitOfMes == 1)
            {
                ViewState["UnitName"] = "m";
                ViewState["UnitValue"] = "1";
            }
            else if (sn.User.UnitOfMes == 0.6214)
            {
                ViewState["UnitName"] = "yrd";
                ViewState["UnitValue"] = "1.094";
            }
        }

        bool CheckIfNumberic(string myNumber)
        {
            try
            {
                double dbl = Convert.ToDouble(myNumber);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private long SaveToPointsSets(int FleetId, DateTime start, DateTime finish)
        {
            string StreetAddress = string.Empty;
            start = start.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);
            finish = finish.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);
            clsMap mp = new clsMap();

            if (this.txtX.Text == "0")
            {

                string strAddress = this.txtStreet.Text + "," + this.txtCity.Text + "," + this.txtState.Text + ",," + this.cboCountry.SelectedItem.Value ;
                StreetAddress = this.txtStreet.Text + "," + this.txtCity.Text + "," + this.txtState.Text + "," + this.cboCountry.SelectedItem.Value ;
                double X = 0; double Y = 0;
                string resolvedAddress = "";
                mp.ResolveCooridnatesByAddressTelogis(strAddress, ref X, ref Y, ref resolvedAddress);

                this.txtX.Text = X.ToString();
                this.txtY.Text = Y.ToString();

                if ((this.txtX.Text != "0") && (this.txtY.Text != "0"))
                {

                    if (!CheckIfNumberic(this.txtX.Text))
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblAddMessage_Text_InvalidLongitudeError");
                        return -1;
                    }

                    if (!CheckIfNumberic(this.txtY.Text))
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblAddMessage_Text_InvalidLatitudeError");
                        return -1;
                    }
                }
                else
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblAddMessage_Text_InvalidAddressError");
                    return -1;
                }
                StreetAddress = StreetAddress + " Radius: " + this.txtRadius.Text + ViewState["UnitName"].ToString();
            }
            else StreetAddress = txtY.Text.ToString() + ", " +  txtX.Text.ToString() +  " Radius: " + this.txtRadius.Text + ViewState["UnitName"].ToString();
            long ret = -1;
            try
            {
                string email = txtEmail.Text.Trim();
                if (email == string.Empty)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = emailText;
                    return ret;
                }
                Int32 radius = -1;
                string pointSets = string.Empty;
                if (LandmarkOptions.SelectedValue == "0")
                {
                    if ((int)(Double.Parse(this.txtRadius.Text)) < 0)
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("valRangeRadiusResource1.ErrorMessage");
                        return ret;
                    }
                    radius = Convert.ToInt32(Double.Parse(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"].ToString()));
                }

                if (LandmarkOptions.SelectedValue == "1" || LandmarkOptions.SelectedValue == "2")
                {
                    StreetAddress = notcircleLandmarkText;
                    if (sn.Landmark.DsLandmarkDetails != null)
                    {
                        foreach (DataRow dr in sn.Landmark.DsLandmarkDetails.Rows)
                        {
                            try
                            {
                                string pointLatitude = dr["Latitude"].ToString();
                                string pointLongitude = dr["Longitude"].ToString();
                                if (pointSets != string.Empty)
                                {
                                    pointSets = pointSets + "," + pointLatitude + "|" + pointLongitude;
                                }
                                else pointSets = pointLatitude + "|" + pointLongitude;
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    if (pointSets == string.Empty)
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = drawPolygonText;
                        return ret;
                    }
                }

                this.lblMessage.Visible = false;
                this.lblMessage.Text = string.Empty;
                VLF.DAS.Logic.LandmarkPointSetManager landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);
                ret = landPointMgr.vlfLandmarkPostCommOnTheFly_Add(sn.User.OrganizationId, sn.UserID,
                    FleetId, start, finish,
                    Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text), radius, email,
                   pointSets, StreetAddress
                );

                hidAddress.Value = "";
                sn.Landmark.X = 0;
                sn.Landmark.Y = 0;
            }
            catch (Exception Ex)
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AddLandmarkFailed");
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                return ret;
            }
            return ret;
        }

        protected void txtStreet_TextChanged(object sender, System.EventArgs e)
        {
            this.txtY.Text = "0";
            this.txtX.Text = "0";
            SetHidAddress();
        }

        protected void txtCity_TextChanged(object sender, System.EventArgs e)
        {
            this.txtY.Text = "0";
            this.txtX.Text = "0";
            SetHidAddress();
        }

        protected void cboCountry_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.txtY.Text = "0";
            this.txtX.Text = "0";
            SetHidAddress();
        }

        protected void txtState_TextChanged(object sender, System.EventArgs e)
        {
            this.txtY.Text = "0";
            this.txtX.Text = "0";
            SetHidAddress();
        }
        protected void btnHidden_Click(object sender, System.EventArgs e)
        {
            if (sn.Landmark.LandmarkType == VLF.CLS.Def.Enums.GeozoneType.None)
            {
                txtRadius.Text = (sn.Landmark.Radius * Convert.ToDecimal(ViewState["UnitValue"])).ToString();
                txtX.Text = sn.Landmark.X.ToString();
                txtY.Text = sn.Landmark.Y.ToString();
                LandmarkOptions.ClearSelection();
                LandmarkOptions.SelectedValue = "0";
                dgLandmarkCoordinates.Visible = false;
                txtRadius.Enabled = true;
            }

            if (sn.Landmark.LandmarkType == VLF.CLS.Def.Enums.GeozoneType.Polygon ||
                sn.Landmark.LandmarkType == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
            {
                txtRadius.Text = "-1";
                txtX.Text = sn.Landmark.X.ToString();
                txtY.Text = sn.Landmark.Y.ToString();
                LandmarkOptions.ClearSelection();
                if (sn.Landmark.LandmarkType == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                    LandmarkOptions.SelectedValue = "1";

                if (sn.Landmark.LandmarkType == VLF.CLS.Def.Enums.GeozoneType.Polygon)
                    LandmarkOptions.SelectedValue = "2";
                dgLandmarkCoordinates.Visible = true;
                dgLandmarkCoordinates.DataSource = sn.Landmark.DsLandmarkDetails;
                dgLandmarkCoordinates.DataBind();
                txtRadius.Enabled = false;
            }

        }

        protected void lstAddOptions_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ViewByOptions();
        }
        private void ViewByOptions()
        {
            try
            {


                if (this.lstAddOptions.SelectedItem.Value == "0")
                {

                    this.tblStreet.Visible = true;
                    this.lblX.Visible = false;
                    this.lblY.Visible = false;
                    this.txtX.Visible = false;
                    this.txtY.Visible = false;
                    this.ValAddress.Enabled = true;
                    this.valX.Enabled = false;
                    this.valY.Enabled = false;
                    this.valRangeLong.Enabled = false;
                    this.valRangLat.Enabled = false;
                    this.tblShowMap.Visible = false;
                    sn.Landmark.LandmarkByAddress = true;
                }


                if (this.lstAddOptions.SelectedItem.Value == "1")
                {

                    this.tblStreet.Visible = false;
                    this.lblX.Visible = true;
                    this.lblY.Visible = true;
                    this.txtX.Visible = true;
                    this.txtY.Visible = true;
                    this.ValAddress.Enabled = false;
                    this.valX.Enabled = true;
                    this.valY.Enabled = true;
                    this.valRangeLong.Enabled = true;
                    this.valRangLat.Enabled = true;
                    this.tblShowMap.Visible = true;
                    sn.Landmark.LandmarkByAddress = false;
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void cmdExecute_Click(object sender, System.EventArgs e)
        {
            CreateReportParams("");
        }
        //End

        protected void cmdShow_Click(object sender, System.EventArgs e)
        {

            CreateReportParams("frmReportViewer.aspx");

        }

        private void cboVehicle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.lblMessage.Text = "";
        }

        protected void cboReports_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                //Devin 
                if (cboReports.SelectedValue == "64")
                {
                    pnlWorkSite.Visible = true;
                    sn.Landmark.X = 0;
                    sn.Landmark.Y = 0;
                    sn.Landmark.LandmarkName = "";
                    sn.Landmark.LandmarkType = VLF.CLS.Def.Enums.GeozoneType.None;
                    cmdExecute.Visible = true;
                    btnBack.Visible = true;
                    cmdShow.Visible = false;
                    cmdSchedule.Visible = false;
                    cmdViewScheduled.Visible = false;
                    sn.Landmark.AddLandmarkMode = true;
                }
                else
                {
                    cmdExecute.Visible = false;
                    btnBack.Visible = false;
                    cmdShow.Visible = true;
                    cmdSchedule.Visible = true;
                    cmdViewScheduled.Visible = true;
                    pnlWorkSite.Visible = false;
                }
                //Devin end

                ReportCriteria();
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

        /// <summary>
        /// Show/hide controls according to selected report
        /// </summary>
        private void ReportCriteria()
        {


            this.cboVehicle.Enabled = true;
            FleetVehicleShow(true);

            // build descr. name
            this.LabelReportDescription.Text = "";

            string resourceDescriptionName = "";
            string filter = String.Format("GuiId = '{0}'", this.cboReports.SelectedValue);
            DataRow[] rowsReport = null;
            if (Util.IsDataSetValid(sn.Report.UserExtendedReportsDataSet))
            {
                rowsReport = sn.Report.UserExtendedReportsDataSet.Tables[0].Select(filter);
                if (rowsReport != null && rowsReport.Length > 0)
                {
                    resourceDescriptionName = String.Format("Description_{0}", rowsReport[0]["ReportTypesName"]);
                    this.LabelReportDescription.Text = base.GetResource(resourceDescriptionName);
                    if (String.IsNullOrEmpty(this.LabelReportDescription.Text))
                        this.LabelReportDescription.Text = rowsReport[0]["GuiName"].ToString();
                }
            }
            this.lblToTitle3.Visible = true;
            this.txtTo.Visible = true;
            this.lblToTitle3.Visible = true;
            this.txtTo.Visible = true;
            this.txtFrom.Visible = true;
            this.lblFromTitle3.Visible = true;
            this.cboFleet.Enabled = true;
            this.tblCost.Visible = false;
            this.tblFleets.Visible = false;
            this.tblFilter.Visible = false;
            tblIgnition.Visible = false;
            tblViolationReport.Visible = false;
            this.lblGeozoneCaption.Visible = false;
            this.ddlGeozones.Visible = false;
            this.tblSpeedViolation.Visible = false;
            tblPoints.Visible = false;
            tblViolationReport.Visible = false;

            switch (this.cboReports.SelectedValue)
            {
                case "29":
                case "55":
                    FleetVehicleShow(false);

                    break;
                case "31":
                    this.cboVehicle.Enabled = true;
                    this.lblVehicleName.Enabled = true;
                    this.tblSpeedViolation.Visible = false;
                    this.tblCost.Visible = true;
                    break;
                case "32":
                case "37":
                    this.tblFleets.Visible = true;
                    this.cboFleet.Enabled = false;
                    this.cboVehicle.Enabled = false;
                    this.lblVehicleName.Enabled = false;
                    this.tblSpeedViolation.Visible = false;
                    this.tblCost.Visible = true;
                    this.tblFilter.Visible = true;
                    break;

                case "33":
                case "35":
                    this.tblFleets.Visible = true;
                    this.cboFleet.Enabled = false;
                    this.cboVehicle.Enabled = false;
                    this.lblVehicleName.Enabled = false;
                    this.tblSpeedViolation.Visible = true;
                    this.tblCost.Visible = false;
                    this.tblFilter.Visible = true;
                    break;

                case "38":

                    FleetVehicleShow(false);
                    tblIgnition.Visible = true;
                    break;

                case "39":
                    this.cboVehicle.Enabled = false;
                    tblIgnition.Visible = true;
                    break;

                case "42":
                    this.lblToTitle3.Visible = false;
                    this.txtTo.Visible = false;
                    this.cboVehicle.Enabled = false;
                    this.lblVehicleName.Enabled = false;
                    this.tblCost.Visible = false;
                    break;

                case "43":
                case "44":
                case "45":
                case "46":
                    this.lblToTitle3.Visible = false;
                    this.txtTo.Visible = false;
                    this.cboVehicle.Enabled = false;
                    this.lblVehicleName.Enabled = false;
                    this.cboFleet.Enabled = false;
                    this.tblCost.Visible = false;
                    break;

                case "47":
                    this.lblToTitle3.Visible = false;
                    this.txtTo.Visible = false;
                    this.cboVehicle.Enabled = false;
                    this.lblVehicleName.Enabled = false;
                    this.cboFleet.Enabled = false;
                    this.tblCost.Visible = true;
                    break;

                case "49":
                    this.lblToTitle3.Visible = false;
                    this.txtTo.Visible = false;
                    this.lblFromTitle3.Visible = false;
                    this.txtFrom.Visible = false;
                    this.cboVehicle.Enabled = false;
                    this.lblVehicleName.Enabled = false;
                    this.cboFleet.Enabled = false;
                    this.tblCost.Visible = false;
                    break;

                case "52":
                    this.lblToTitle3.Visible = true;
                    this.txtTo.Visible = true;
                    this.lblFromTitle3.Visible = true;
                    this.txtFrom.Visible = true;
                    this.cboVehicle.Enabled = true;
                    this.lblVehicleName.Enabled = true;
                    this.cboFleet.Enabled = true;
                    this.tblCost.Visible = false;
                    break;
                case "56":
                    this.cboVehicle.Enabled = true;
                    tblIgnition.Visible = true;
                    break;

                case "57":
                    this.cboVehicle.Enabled = false;
                    this.tblViolationReport.Visible = true;
                    break;
                case "63":
                    this.cboVehicle.Enabled = true;
                    break;
                case "64":
                case "65":
                case "70":
                case "71":
                    //this.lblLandmarkCaption.Visible = true ;
                    //this.ddlLandmarks.Visible = true ;
                    this.cboVehicle.Enabled = false;
                    //LoadLandmarks();
                    break;

                case "66":
                case "67":
                case "68":
                case "69":

                    // this.lblToTitle3.Visible = false;
                    //this.txtTo.Visible = false;
                    //this.lblFromTitle3.Visible = false;
                    //this.txtFrom.Visible = false;
                    this.cboVehicle.Enabled = false;
                    //  this.lblGeozoneCaption.Visible = true;
                    //  this.ddlGeozones.Visible = true;  
                    break;

                case "72":
                    this.tblPoints.Visible = true;
                    tblViolationReport.Visible = true;
                    this.cboVehicle.Enabled = false;
                    break;
            }

        }

        /// <summary>
        /// Show or hide fleet / vehicle lables and ddl
        /// </summary>
        /// <param name="showControls"></param>
        private void FleetVehicleShow(bool showControls)
        {
            this.cboFleet.Visible = showControls;
            this.lblFleet.Visible = showControls;
            this.lblVehicleName.Visible = showControls;
            this.cboVehicle.Visible = showControls;
        }


        private void LoadLandmarks()
        {
            DataSet dsLandmarks = new DataSet();
            string xmlResult = "";

            using (ServerDBOrganization.DBOrganization organ = new ServerDBOrganization.DBOrganization())
            {

                if (objUtil.ErrCheck(organ.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
                    if (objUtil.ErrCheck(organ.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        //RedirectToLogin();
                        return;
                    }
            }

            if (String.IsNullOrEmpty(xmlResult))
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                return;
            }

            dsLandmarks.ReadXml(new StringReader(xmlResult));

            this.ddlLandmarks.Items.Clear();

            if (Util.IsDataSetValid(dsLandmarks))
            {
                this.ddlLandmarks.DataSource = dsLandmarks;
                this.ddlLandmarks.DataBind();
                this.ddlLandmarks.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlLandmarks_Item_0").ToString(), "-1"));
            }
            else
                this.ddlLandmarks.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlLandmarks_NoAvailable").ToString(), "-100"));


            if ((sn.Report.LandmarkName != "") && (sn.Report.LandmarkName != " -1"))
                ddlLandmarks.SelectedIndex = ddlLandmarks.Items.IndexOf(ddlLandmarks.Items.FindByValue(sn.Report.LandmarkName.ToString()));

        }




        /// <summary>
        /// Get user reports dataset from session, if not valid - use web method
        /// </summary>
        private void GetUserReportsTypes()
        {
            string xml = "";
            Int16 categoryId = 1;

            if (sn.User.OrganizationId == 123) //CN
                categoryId = 1;
            if (sn.User.OrganizationId == 570) //Brickman
                categoryId = 4;
            else if (sn.SuperOrganizationId == 382) //Wex
                categoryId = 2;
            else if (sn.User.OrganizationId == 622) //CP Rail
                categoryId = 3;
            else if (sn.User.OrganizationId == 18) //Aecon
                categoryId = 5;
            else if (sn.User.OrganizationId == 951) //UP
                categoryId = 6;
            else if (sn.User.OrganizationId == 327) //Badger Daylighting Inc
                categoryId = 7;

            if (Util.IsDataSetValid(sn.Report.UserExtendedReportsDataSet))
            {
                this.cboReports.DataSource = sn.Report.UserExtendedReportsDataSet;
                this.cboReports.DataBind();
            }
            else
            {
                DataSet dsReports = new DataSet();

                using (ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem())
                {
                    if (objUtil.ErrCheck(dbs.GetUserReportsByCategory(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, categoryId, ref xml), false))
                        if (objUtil.ErrCheck(dbs.GetUserReportsByCategory(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, categoryId, ref xml), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No GUI Controls setttngs for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            return;
                        }
                }

                if (String.IsNullOrEmpty(xml))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "No GUI Controls settings for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    // return;
                }
                else
                {

                    dsReports.ReadXml(new StringReader(xml));
                    this.cboReports.DataSource = dsReports;
                    sn.Report.UserExtendedReportsDataSet = dsReports;
                    this.cboReports.DataBind();
                }

            }



            //  if (categoryId == 1)
            //      cboReports.Items.Add(new ListItem("Sensor Activity Report", "70"));


            //  if (categoryId == 6)
            //   {
            //       cboReports.Items.Clear();  
            //       cboReports.Items.Add(new ListItem("Sensor Activity Report", "70"));
            //       cboReports.Items.Add(new ListItem("Activity Summary Report by Vehicle", "71"));
            //   }
        }



        protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cboFleet.SelectedValue) != -1)
            {
                this.cboVehicle.Visible = true;
                this.lblVehicleName.Visible = true;
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedValue));
            }
        }


        /// <summary>
        /// Create report params and redirect to the next page
        /// </summary>
        /// <param name="redirectUrl">Web page to redirect to</param>
        private void CreateReportParams(string redirectUrl)
        {
            try
            {
                string strFromDate = this.txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " 12:00 AM";
                string strToDate = this.txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " 11:59 PM";
                DateTime from, to;
                sn.Report.IsFleet = this.cboVehicle.SelectedValue == "0" ? true : false;

                const string dateFormat = "MM/dd/yyyy HH:mm:ss";

                this.lblMessage.Text = "";

                # region Validation

                if (!clsUtility.IsNumeric(this.txtCost.Text))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "Cost should be numeric";
                    return;
                }

                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                //System.Threading.Thread.CurrentThread.CurrentUICulture = ci;

                from = Convert.ToDateTime(strFromDate, ci);
                to = Convert.ToDateTime(strToDate, ci);

                if (from >= to)
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

                if ((this.cboVehicle.SelectedIndex == 0 && (this.cboReports.SelectedValue == "3")) ||
                    this.cboVehicle.SelectedIndex == -1)
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
                # endregion

                this.BusyReport.Visible = true;


                # region Reports
                switch (this.cboReports.SelectedValue)
                {

                    case "31":
                        sn.Report.XmlParams = String.Format("{0}={1};", ReportTemplate.RptParamVehicleId, this.cboVehicle.SelectedValue.Trim());
                        sn.Report.XmlParams += String.Format("{0}={1};", ReportTemplate.RptParamCost, this.txtCost.Text);
                        break;
                    case "32":
                    case "37":
                        //sn.Report.XmlParams = String.Format("{0}={1};", ReportTemplate.RptParamCost, this.txtCost.Text);
                        sn.Report.XmlParams = String.Format("{0};{1}", this.txtCost.Text, this.txtColorFilter.Text);
                        break;
                    case "33":
                    case "35":
                        //sn.Report.XmlParams = String.Format("{0}={1};", ReportTemplate.RptParamIncludeSummary, this.cboViolationSpeed.SelectedItem.Value);

                        sn.Report.XmlParams = String.Format("{0};{1}", this.cboViolationSpeed.SelectedItem.Value, this.txtColorFilter.Text);

                        break;

                    #region Organization summary
                    case "38":
                        sn.Report.XmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                        break;
                    # endregion
                    #region Activity Summary Report per Vehicle
                    case "39":
                        sn.Report.XmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                        break;
                    # endregion
                    case "47":
                        sn.Report.XmlParams = String.Format("{0}={1};", ReportTemplate.RptParamCost, this.txtCost.Text);
                        break;

                    #region Vehicle summary
                    case "56":
                        sn.Report.XmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                        break;
                    # endregion

                    case "57":
                    case "72":
                        sn.Report.XmlParams = CreateViolationParameters(this.cboReports.SelectedValue);
                        break;

                    #region New Trips Summary Report per Vehicle
                    case "63":
                        // xmlParams = String.Format("{0}={1};{2}={3}",
                        //       ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue,
                        //       ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);

                        sn.Report.XmlParams = this.cboVehicle.SelectedValue.Trim();
                        // sn.Report.XmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);


                        break;
                    # endregion


                    #region Worksite Activity Report
                    case "64":
                        sn.Report.XmlParams = this.ddlLandmarks.SelectedValue;
                        break;
                    # endregion

                    //#region Timesheet Validation
                    //case "66":
                    //  sn.Report.XmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamGeozone, this.ddlGeozones.SelectedValue);
                    //  break; 
                    //#endregion
                }
                # endregion Reports

                sn.Report.GuiId = Convert.ToInt16(this.cboReports.SelectedValue);
                sn.Report.FromDate = from.ToString();
                sn.Report.ToDate = to.ToString();
                sn.Report.ReportFormat = Convert.ToInt32(this.cboFormat.SelectedValue);
                sn.Report.FleetId = Convert.ToInt32(this.cboFleet.SelectedValue);
                sn.Report.IsFleet = this.cboVehicle.SelectedValue == "0" ? true : false;
                sn.Report.VehicleId = Convert.ToInt32(this.cboVehicle.SelectedValue);
                sn.Report.FleetName = this.cboFleet.SelectedItem.Text;
                sn.Report.ReportType = cboReports.SelectedValue;

                //Devin Begin
                if (cboReports.SelectedValue == "64")
                {
                    long landmarkId = SaveToPointsSets(sn.Report.FleetId, from, to);
                    if (landmarkId == -1) return;
                    else
                    {
                        txtEmail.Text = string.Empty;
                        txtX.Text = string.Empty;
                        txtY.Text = string.Empty;
                        txtRadius.Text = "-1";

                        txtStreet.Text = string.Empty;
                        txtCity.Text = string.Empty;
                        txtState.Text = string.Empty;

                        LandmarkOptions.SelectedIndex = 0;
                        dgLandmarkCoordinates.Visible = false;
                        dgLandmarkCoordinates.DataSource = null;
                        dgLandmarkCoordinates.DataBind();
                        //string script = "<script type='text/javascript'>alert('" + executeSucceed + "');</script>";
                        string script = "alert('" + executeSucceed + "');";
                        //ClientScript.RegisterStartupScript(this.GetType(), "alertExecute", script);
                        RadAjaxManager1.ResponseScripts.Add(script);
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "alertExecute", script, false);
                        return;
                    }
                }
                //Devin End

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

            Response.Redirect(redirectUrl);
        }





        protected void cmdViewScheduled_Click(object sender, EventArgs e)
        {
            Response.Redirect("../ReportsScheduling/frmScheduleReportList.aspx?back=frmReportMasterExtended.aspx");
        }

        protected void cmdSchedule_Click(object sender, EventArgs e)
        {
            CreateReportParams("../ReportsScheduling/frmReportScheduler.aspx?back=frmReportMasterExtended.aspx");
        }


        protected void cmdAdd_Click(object sender, EventArgs e)
        {

            DataSet ds = sn.Misc.DsReportAllFleets;
            if (sn.Misc.DsReportSelectedFleets == null || sn.Misc.DsReportSelectedFleets.Tables.Count == 0)
                sn.Misc.DsReportSelectedFleets = ds.Clone();

            foreach (ListItem li in lstUnAss.Items)
            {
                if (li.Selected)
                {
                    DataRow dr = sn.Misc.DsReportSelectedFleets.Tables[0].NewRow();
                    dr["FleetId"] = li.Value;
                    dr["FleetName"] = li.Text;
                    sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Add(dr);

                    DataRow[] drColl = ds.Tables[0].Select("FleetId='" + li.Value + "'");
                    ds.Tables[0].Rows.Remove(drColl[0]);
                }

            }

            if (sn.Misc.DsReportSelectedFleets.Tables.Count > 0)
            {
                sn.Misc.DsReportSelectedFleets.Tables[0].DefaultView.Sort = "FleetName";
                this.lstAss.DataSource = sn.Misc.DsReportSelectedFleets.Tables[0].DefaultView;
                lstAss.DataBind();
            }
            else
            {
                lstAss.Items.Clear();

            }

            if (ds.Tables.Count > 0)
            {
                ds.Tables[0].DefaultView.Sort = "FleetName";
                lstUnAss.DataSource = ds.Tables[0].DefaultView;
                lstUnAss.DataBind();
            }
            else
            {

                lstUnAss.Items.Clear();
            }

            sn.Misc.DsReportAllFleets = ds;


        }
        protected void cmdAddAll_Click(object sender, EventArgs e)
        {
            sn.Misc.DsReportSelectedFleets = sn.Misc.DsReportAllFleets;
            sn.Misc.DsReportSelectedFleets.Tables[0].DefaultView.Sort = "FleetName";
            this.lstAss.DataSource = sn.Misc.DsReportSelectedFleets.Tables[0].DefaultView;
            lstAss.DataBind();
            sn.Misc.DsReportAllFleets.Clear();
            lstUnAss.Items.Clear();

        }
        protected void cmdRemove_Click(object sender, EventArgs e)
        {
            DataSet ds = sn.Misc.DsReportSelectedFleets;
            if (sn.Misc.DsReportAllFleets == null || sn.Misc.DsReportAllFleets.Tables.Count == 0)
                sn.Misc.DsReportAllFleets = ds.Clone();


            foreach (ListItem li in lstAss.Items)
            {
                if (li.Selected)
                {
                    DataRow dr = sn.Misc.DsReportAllFleets.Tables[0].NewRow();
                    dr["FleetId"] = li.Value;
                    dr["FleetName"] = li.Text;


                    sn.Misc.DsReportAllFleets.Tables[0].Rows.Add(dr);

                    DataRow[] drColl = ds.Tables[0].Select("FleetId='" + li.Value + "'");
                    ds.Tables[0].Rows.Remove(drColl[0]);
                }


            }

            if (ds.Tables.Count > 0)
            {
                ds.Tables[0].DefaultView.Sort = "FleetName";
                this.lstAss.DataSource = ds.Tables[0].DefaultView;
                lstAss.DataBind();
            }
            else
            {
                lstAss.Items.Clear();
            }

            if (sn.Misc.DsReportAllFleets.Tables.Count > 0)
            {
                sn.Misc.DsReportAllFleets.Tables[0].DefaultView.Sort = "FleetName";
                lstUnAss.DataSource = sn.Misc.DsReportAllFleets.Tables[0].DefaultView;
                lstUnAss.DataBind();
            }
            else
            {
                lstUnAss.Items.Clear();
            }

            sn.Misc.DsReportSelectedFleets = ds;
        }

        protected void cmdRemoveAll_Click(object sender, EventArgs e)
        {

            sn.Misc.DsReportAllFleets = sn.Misc.DsReportSelectedFleets;
            sn.Misc.DsReportSelectedFleets.Clear();
            this.lstAss.DataSource = sn.Misc.DsReportSelectedFleets;
            lstAss.DataBind();
            CboFleet_Fill();
        }


        private string CreateViolationParameters(string reportGuiId)
        {
            int intCriteria = 0;

            if (this.chkSpeedViolation.Checked)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_SPEED;
            if (this.chkHarshAcceleration.Checked)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHACCELERATION;
            if (this.chkHarshBraking.Checked)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHBRAKING;
            if (this.chkExtremeAcceleration.Checked)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEACCELERATION;
            if (this.chkExtremeBraking.Checked)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEBRAKING;
            if (this.chkSeatBeltViolation.Checked)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_SEATBELT;

            StringBuilder Params = new StringBuilder();
            Params.Append(intCriteria);

            // violation summary
            if (reportGuiId == "72")
            {
                Params.AppendFormat(";{0};{1};{2};{3};{4};{5};{6};{7}",
                   this.txtSpeed120.Text, this.txtSpeed130.Text, this.txtSpeed140.Text,
                   this.txtAccExtreme.Text, this.txtAccHarsh.Text, this.txtBrakingExtreme.Text,
                   this.txtBrakingHarsh.Text, this.txtSeatBelt.Text);
            }

            else
            {

                Params.Append("*");
                string tmpSpeed = "";
                switch (this.cboViolationSpeed.SelectedValue)
                {
                    case "1":
                        tmpSpeed = (sn.User.UnitOfMes == 1 ? "100" : "62");
                        break;
                    case "2":
                        tmpSpeed = (sn.User.UnitOfMes == 1 ? "105" : "65");
                        break;
                    case "3":
                        tmpSpeed = (sn.User.UnitOfMes == 1 ? "110" : "68");
                        break;
                    case "4":
                        tmpSpeed = (sn.User.UnitOfMes == 1 ? "120" : "75");
                        break;
                    case "5":
                        tmpSpeed = (sn.User.UnitOfMes == 1 ? "130" : "80");
                        break;
                    case "6":
                        tmpSpeed = (sn.User.UnitOfMes == 1 ? "140" : "85");
                        break;
                }
                Params.Append(tmpSpeed);
            }
            return Params.ToString();
        }

        /// <summary>
        /// Load geozones into ddl
        /// </summary>
        private void LoadGeozones()
        {
            DataSet dsGeozones = new DataSet();
            string xmlResult = "";

            using (ServerDBOrganization.DBOrganization organ = new ServerDBOrganization.DBOrganization())
            {

                if (objUtil.ErrCheck(organ.GetOrganizationGeozones(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
                    if (objUtil.ErrCheck(organ.GetOrganizationGeozones(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Geozones for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        //RedirectToLogin();
                        return;
                    }
            }

            if (String.IsNullOrEmpty(xmlResult))
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Geozones for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                return;
            }

            dsGeozones.ReadXml(new StringReader(xmlResult));

            this.ddlGeozones.Items.Clear();

            if (Util.IsDataSetValid(dsGeozones))
            {
                DataView view = dsGeozones.Tables[0].DefaultView;
                view.Sort = "GeozoneName";
                this.ddlGeozones.DataSource = view;
                this.ddlGeozones.DataBind();
                this.ddlGeozones.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlGeozones_Item_0").ToString(), "-1"));
            }
            else
                this.ddlGeozones.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlGeozones_NoAvailable").ToString(), "-100"));
        }

        private void SetHidAddress()
        {
            if (txtStreet.Text.Trim() != string.Empty &&
                txtCity.Text.Trim() != string.Empty &&
                txtState.Text.Trim() != string.Empty)
            {
                hidAddress.Value =
                this.txtStreet.Text + "," + this.txtCity.Text + "," + this.txtState.Text + ",," + this.cboCountry.SelectedItem.Value ;
            }
            else hidAddress.Value = "";
        }

        [WebMethod]
        public static string SetSearchMap(string street)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                
                //street = HttpContext.Current.Server.HtmlDecode(street);
                street = Uri.UnescapeDataString(street);
                if (street != string.Empty)
                {

                    double X = 0; double Y = 0;
                    string resolvedAddress = "";
                    clsMap mp = new clsMap();
                    mp.ResolveCooridnatesByAddressTelogis(street, ref X, ref Y, ref resolvedAddress);

                    if (X != 0 && Y != 0)
                    {
                        sn.Map.MapSearch = true;
                        sn.History.MapCenterLatitude = Y.ToString();
                        sn.History.MapCenterLongitude = X.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: SelectVehicle() Page:Map\frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";
        }

    }
}