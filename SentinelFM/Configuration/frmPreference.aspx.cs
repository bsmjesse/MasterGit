using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Configuration;
using System.Globalization;
using VLF.PATCH.Logic;
  
namespace SentinelFM
{
	/// <summary>
	/// Summary description for Configuration.
	/// </summary>
    public partial class frmPreference : SentinelFMBasePage
    {
       
        protected ServerDBUser.DBUser dbu;        
        public string msgPsw_Medium = "";
        public string msgPsw_Weak = "";
        public string msgPsw_MoreCharacters = "";
        public string msgPsw_Strong = "";
        public string msgPsw_TypePassword = "";
        
        public string msgPsw_OldNewNotSame = "";
      public string OrganizationHierarchyPath = "";
      public string OrganizationHierarchyNamePath = "";
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;
        public bool ShowOrganizationHierarchy;

        public string LandmarkLineColor = "#cc6633";
        public string LandmarkBgColor = "#ffcc66";
        public string ToTop = "4px";
        public int userIdToUpdate = -1;
        public string username = "";
        public bool ShowTemperatureType = false;
      public string RootOrganizationHierarchyNodeCode = string.Empty;

      public bool MutipleUserHierarchyAssignment;

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

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                ShowTemperatureType = clsPermission.FeaturePermissionCheck(sn, "ShowTemperatureType");
                if (ShowTemperatureType == true)
                {
                    optTemperature.Visible = true; 
                    lblTemprature.Visible = true;
                }
                else
                {
                    optTemperature.Visible = false;
                    lblTemprature.Visible = false;
                }

                if (Request.QueryString["errormsg"] != null)
                    ToTop = "70px";

		if ((Request.QueryString["errormsg"] == "1") && (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 951))//errormsg=1
                {
                    this.txtNewPassword.Enabled = true;
                    this.txtNewPassword1.Enabled = true;
                    this.txtOldPassword.Enabled = true;
                    this.vlComp.Enabled = true;
                    this.valNewPassword.Enabled = true;
                    this.valOldPassword.Enabled = true;
                    this.cmdSavePsw.Text = (string)base.GetLocalResourceObject("cmdSavePsw");
                }


                if (Request.QueryString["userIdToUpdate"] != null)
                {
                    userIdToUpdate = Convert.ToInt32(Request.QueryString["userIdToUpdate"]);
                    this.cmdHome.Visible = false;
                    this.cmdExit.Visible = true;
                    username = Request.QueryString["username"];
                }
                else
                {
                    userIdToUpdate = sn.UserID;
                    this.cmdHome.Visible = true;
                    this.cmdExit.Visible = false;
                }


                MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

                
                msgPsw_OldNewNotSame = GetScriptEscapeString((string)base.GetLocalResourceObject("msgPsw_OldNewNotSame"));
                cmdExit.Text = (string)base.GetLocalResourceObject("cmdExit");
                msgPsw_Medium = GetScriptEscapeString((string)base.GetLocalResourceObject("msgPsw_Medium"));
                msgPsw_Weak = GetScriptEscapeString((string)base.GetLocalResourceObject("msgPsw_Weak"));
                msgPsw_MoreCharacters = GetScriptEscapeString((string)base.GetLocalResourceObject("msgPsw_MoreCharacters"));
                msgPsw_Strong = GetScriptEscapeString((string)base.GetLocalResourceObject("msgPsw_Strong"));
                msgPsw_TypePassword = GetScriptEscapeString((string)base.GetLocalResourceObject("msgPsw_TypePassword"));
                lblLoadLandmarkByDefault.Text = (string)base.GetLocalResourceObject("lblLoadLandmarkByDefault.Text");
                this.txtNewPassword.Attributes.Add("onkeyup", "return passwordChanged();");
                this.landmarkstyleLineColor.Value = LandmarkLineColor;
                this.landmarkstyleBackgroundColor.Value = LandmarkBgColor;

            VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

            if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
            {
                ShowOrganizationHierarchy = true;
                RootOrganizationHierarchyNodeCode = MutipleUserHierarchyAssignment ? "" : poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID, MutipleUserHierarchyAssignment);
            }
            else
            {
                ShowOrganizationHierarchy = false;
            }
            if (ShowOrganizationHierarchy) { 
                this.trDefaultOrganizationHierarchy.Visible = true;
                this.trLoadVehiclesBasedOn.Visible = true;
            }

                //Commented for Mantis# 2740
                //if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956)
                //{
                //    cboLoadVehiclesBasedOn.Items.Remove(cboLoadVehiclesBasedOn.Items.FindByValue("fleet"));                
                //}

                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref Configuration, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    CboFleet_Fill();
                    cboRefreshFreq_Fill();
                    ddlDateFormat_Fill();
                    if (sn.User.UserGroupId == 27 || sn.User.UserGroupId == 28 || sn.User.UserGroupId == 36)
                    {
                        this.lblAlarmRefresh.Visible = false;
                        this.cboAlarmFreq.Visible = false;
                        this.lblViewAlarmScrolling.Visible = false;
                        this.optMapView.Visible = false;
                        this.tblChangePswMain.Visible = false;
                        this.lblChangePassword.Visible = false;
                    }

                    //Set Default Preferences
                    try
                    {
                        if (Request.QueryString["errormsg"] != null)
                        {
                            lblMessageError.Visible = true;
                            //tdExpiredPwd.Visible = true;
                            trPreferences.Visible = false;
                            pswRules.Visible = true;
                            logo.Visible = true;
                        }
                        TimeSpan ExpiredTimeDateTime = new TimeSpan(sn.User.PositionExpiredTime * TimeSpan.TicksPerMinute);
                        cboExpDays.Items.FindByValue((ExpiredTimeDateTime.Days).ToString()).Selected = true;
                        cboExpHours.Items.FindByValue(ExpiredTimeDateTime.Hours.ToString()).Selected = true;
                        cboExpMin.Items.FindByValue(ExpiredTimeDateTime.Minutes.ToString()).Selected = true;
                        MapTypeSet();

                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }

                    FindExistingPreference();
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

            /*if (chkMapAssets.Checked)
                vehicleClusteringPreferences.Style["display"] = "";
            else
                vehicleClusteringPreferences.Style["display"] = "none";*/

            if (chkVehicleClustering.Checked)
                vehicleClusteringOptions.Style["display"] = "";
            else
                vehicleClusteringOptions.Style["display"] = "none";
        }


        protected void cmdSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (this.cmdSave.Text == (string)base.GetLocalResourceObject("cmdSavePreferences"))
                {
                    this.cboTimeZone.Enabled = false;
                    this.cboUnits.Enabled = false;
                    this.cboVolumeUnits.Enabled = false;
                    this.cboFleet.Enabled = false;
                    this.chkMapAssets.Enabled = false;
                    this.chkLoadLandmarkByDefault.Enabled = false;
                    //this.txtMaxVehiclesOnMap.Enabled = false;
                    this.chkVehicleClustering.Enabled = false;
                    this.txtVehicleClusteringDistance.Enabled = false;
                    this.cboVehicleClusteringThreshold.Enabled = false;
                    //this.chkLandmark.Enabled = false;
                    this.chkDaylight.Enabled = false;
                    this.chkShowReadMess.Enabled = false;
                    this.cboAlarmFreq.Enabled = false;
                    this.cboRefreshFreq.Enabled = false;
                    this.cboExpDays.Enabled = false;
                    this.cboExpHours.Enabled = false;
                    this.cboExpMin.Enabled = false;
                    this.chkShowLandmark.Enabled = false;
                    this.chkShowLandmarkname.Enabled = false;
                    this.chkShowVehicleName.Enabled = false;
                    this.cboHistoryGridRows.Enabled = false;
                    this.cboMapGridRows.Enabled = false;
                    this.cmdSave.Text = (string)base.GetLocalResourceObject("cmdEditPreferences");
                    this.optMapView.Enabled = false;
                    this.chkShowMapGridFilter.Enabled = false;
                    this.ddlDateFormat.Enabled = false;
                    this.ddlTimeFormat.Enabled = false;
                    this.cboDefaultMapView.Enabled = false;
                    this.chkRememberLastPage.Enabled = false;
                    this.cboLoadVehiclesBasedOn.Enabled = false;
                    this.trDefaultOrganizationHierarchyTree.Visible = false;
                    this.PreferencePageMode.Value = "0";
                    this.txtVehicleNotReported.Enabled = false;
                    this.chkShowRetiredVehicles.Enabled = false;
                    this.optTemperature.Enabled = false;
                }
                else
                {
                    this.cboTimeZone.Enabled = true;
                    this.cboUnits.Enabled = true;
                    this.cboVolumeUnits.Enabled = true;
                    this.cboFleet.Enabled = true;
                    this.chkMapAssets.Enabled = true;
                    this.chkLoadLandmarkByDefault.Enabled = true;
                    //this.txtMaxVehiclesOnMap.Enabled = true;
                    this.chkVehicleClustering.Enabled = true;
                    this.txtVehicleClusteringDistance.Enabled = true;
                    this.cboVehicleClusteringThreshold.Enabled = true;
                    //this.chkLandmark.Enabled = true;
                    this.chkDaylight.Enabled = true;
                    this.cmdSave.Text = (string)base.GetLocalResourceObject("cmdSavePreferences");
                    this.chkShowReadMess.Enabled = true;
                    this.cboAlarmFreq.Enabled = true;
                    this.cboRefreshFreq.Enabled = true;
                    this.cboExpDays.Enabled = true;
                    this.cboExpHours.Enabled = true;
                    this.cboExpMin.Enabled = true;
                    this.chkShowLandmark.Enabled = true;
                    this.chkShowLandmarkname.Enabled = true;
                    this.chkShowVehicleName.Enabled = true;
                    this.cboHistoryGridRows.Enabled = true;
                    this.cboMapGridRows.Enabled = true;
                    this.optMapView.Enabled = true;
                    this.chkShowMapGridFilter.Enabled = true;
                    this.ddlDateFormat.Enabled = true;
                    this.ddlTimeFormat.Enabled = true;
                    this.cboDefaultMapView.Enabled = true;
                    this.chkRememberLastPage.Enabled = true;
                    this.txtVehicleNotReported.Enabled = true;
                    this.cboLoadVehiclesBasedOn.Enabled = true;
                    if (ShowOrganizationHierarchy) this.trDefaultOrganizationHierarchyTree.Visible = true;
                    this.PreferencePageMode.Value = "1";
                    this.chkShowRetiredVehicles.Enabled = true;
                    this.optTemperature.Enabled = true;
                    FindExistingPreference();
                    return;
                }
                

                
                sn.User.UnitOfMes = Convert.ToDouble(this.cboUnits.SelectedItem.Value);
                sn.User.VolumeUnits = Convert.ToDouble(this.cboVolumeUnits.SelectedItem.Value);

                // Getting value of Previous DateFormat
                if (sn.User.DateFormat != this.ddlDateFormat.SelectedValue)
                {
                    sn.PreviousDateFormat = sn.User.DateFormat;
                }
                sn.User.DateFormat = this.ddlDateFormat.SelectedValue;
                sn.User.TimeFormat = this.ddlTimeFormat.SelectedValue;
                sn.User.VehicleNotReported = Convert.ToInt16(this.txtVehicleNotReported.Text);

                dbu = new ServerDBUser.DBUser();

                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MeasurementUnits), this.cboUnits.SelectedItem.Value), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MeasurementUnits), this.cboUnits.SelectedItem.Value), true))
                    {
                        //return;
                    }


                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VolumeUnits), this.cboVolumeUnits.SelectedItem.Value), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VolumeUnits), this.cboVolumeUnits.SelectedItem.Value), true))
                    {
                        //return;
                    }

                // Changes for TimeZone Feature start
                int value;
                sn.User.NewFloatTimeZone = Convert.ToSingle(this.cboTimeZone.SelectedItem.Value);
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.TimeZoneNew), this.cboTimeZone.SelectedItem.Value), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.TimeZoneNew), this.cboTimeZone.SelectedItem.Value), true))
                    {
                        //return;
                    }
                if (int.TryParse(this.cboTimeZone.SelectedItem.Value, out value))
                {
                    sn.User.TimeZone = Convert.ToInt16(this.cboTimeZone.SelectedItem.Value);
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.TimeZone), this.cboTimeZone.SelectedItem.Value), false))
                        if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.TimeZone), this.cboTimeZone.SelectedItem.Value), true))
                        {
                            //return;
                        }
                }
                // Changes for TimeZone Feature end             
                
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, 44, this.ddlDateFormat.SelectedValue), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, 44, this.ddlDateFormat.SelectedValue), true))
                    {
                        //return;
                    }
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, 45, this.ddlTimeFormat.SelectedValue), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, 45, this.ddlTimeFormat.SelectedValue), true))
                    {
                        //return;
                    }

                if (this.cboFleet.SelectedIndex != -1)
                {
                    sn.User.DefaultFleet = Convert.ToInt32(this.cboFleet.SelectedItem.Value);

                    if (this.cboFleet.SelectedItem.Value != "-1")
                    {
                        if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultFleet), this.cboFleet.SelectedItem.Value), false))
                            if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultFleet), this.cboFleet.SelectedItem.Value), true))
                            {
                                //return;
                            }
                    }
                }

                if (this.chkMapAssets.Checked)
                {
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapAssets), "1"), false))
                        if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapAssets), "1"), false))
                        {
                            //return;
                        }
                }
                else
                {
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapAssets), "0"), false))
                        if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapAssets), "0"), false))
                        {
                            //return;
                        }
                }

                if (this.chkLoadLandmarkByDefault.Checked)
                {
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.LoadLandmarkByDefault), "1"), false))
                        if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.LoadLandmarkByDefault), "1"), false))
                        {
                            //return;
                        }
                }
                else
                {
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.LoadLandmarkByDefault), "0"), false))
                        if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.LoadLandmarkByDefault), "0"), false))
                        {
                            //return;
                        }
                }

                if (this.chkVehicleClustering.Checked)
                {
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleClustering), "1"), false))
                        if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleClustering), "1"), false))
                        {
                            //return;
                        }
                }
                else
                {
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleClustering), "0"), false))
                        if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleClustering), "0"), false))
                        {
                            //return;
                        }
                }

                int vehicleClusteringDistance = 20;
                Int32.TryParse(this.txtVehicleClusteringDistance.Text, out vehicleClusteringDistance);

                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleClusteringDistance), vehicleClusteringDistance.ToString()), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleClusteringDistance), vehicleClusteringDistance.ToString()), false))
                    {
                        //return;
                    }

                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleClusteringThreshold), this.cboVehicleClusteringThreshold.SelectedItem.Value), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleClusteringThreshold), this.cboVehicleClusteringThreshold.SelectedItem.Value), false))
                    {
                        //return;
                    }

                /*int maxVehiclesOnMap = 500;
                Int32.TryParse(this.txtMaxVehiclesOnMap.Text, out maxVehiclesOnMap);
                maxVehiclesOnMap = (maxVehiclesOnMap > 3000) ? 3000 : maxVehiclesOnMap;
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MaxVehiclesOnMap), maxVehiclesOnMap.ToString()), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MaxVehiclesOnMap), maxVehiclesOnMap.ToString()), false))
                    {
                        //return;
                    }
                */
                string _defaultMapView = this.cboDefaultMapView.SelectedItem.Value;
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultMapView), _defaultMapView), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultMapView), _defaultMapView), false))
                    {
                        //return;
                    }

                if (_defaultMapView != this.OriginalDefaultMapView.Value)
                {
                    HttpCookie myCookie = Request.Cookies[sn.User.OrganizationId.ToString() + "DefaultMapView"];
                    if (myCookie != null)
                    {
                        myCookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(myCookie);
                    }

                    myCookie = Request.Cookies[sn.User.OrganizationId.ToString() + "mapGridDefaultHeight"];
                    if (myCookie != null)
                    {
                        myCookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(myCookie);
                    }

                    myCookie = Request.Cookies[sn.User.OrganizationId.ToString() + "mapGridDefaultWidth"];
                    if (myCookie != null)
                    {
                        myCookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(myCookie);
                    }
                }

                string _loadVehiclesBasedOn = this.cboLoadVehiclesBasedOn.SelectedItem.Value;
                sn.User.LoadVehiclesBasedOn = _loadVehiclesBasedOn;
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, 36, _loadVehiclesBasedOn), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, 36, _loadVehiclesBasedOn), false))
                    {
                        //return;
                    }

                string _defaultOrganizationHierarchyNodeCode = this.DefaultOrganizationHierarchyNodeCode.Value;
                this.defaultOrganizationHierarchy.Text = _defaultOrganizationHierarchyNodeCode;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, _defaultOrganizationHierarchyNodeCode);
                OrganizationHierarchyNamePath = poh.GetOrganizationHierarchyNamePath(sn.User.OrganizationId, _defaultOrganizationHierarchyNodeCode); 
                if (MutipleUserHierarchyAssignment)
                {
                    //OrganizationHierarchyMultiplePath = getOrganizationHierarchyMultiplePath(_defaultOrganizationHierarchyNodeCode);
                    OrganizationHierarchyPath = getOrganizationHierarchyMultiplePath(_defaultOrganizationHierarchyNodeCode);
                    OrganizationHierarchyNamePath = getOrganizationHierarchyMultipleNamePath(_defaultOrganizationHierarchyNodeCode);
                    this.defaultOrganizationHierarchy.Text = OrganizationHierarchyNamePath.Replace(";","<div style='border-bottom:1px solid #aaaaaa;height:1px;width:100%;'></div>");
                }
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode), _defaultOrganizationHierarchyNodeCode), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode), _defaultOrganizationHierarchyNodeCode), false))
                    {
                        //return;
                    }

                sn.User.PreferNodeCodes = _defaultOrganizationHierarchyNodeCode;
                if (MutipleUserHierarchyAssignment)
                {
                    string[] ns = sn.User.PreferNodeCodes.Split(',');
                    string multipleFleetIds = string.Empty;
                    foreach (string s in ns)
                    {
                        int fid = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, s);
                        multipleFleetIds = multipleFleetIds + "," + fid.ToString();
                    }
                    multipleFleetIds = multipleFleetIds.Trim(',');
                    sn.User.PreferFleetIds = multipleFleetIds;
                }
                //if (this.chkLandmark.Checked)
                //{
                //    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ResolveLandmark), VLF.CLS.Def.Const.resolveLandmarksYes.ToString()), false))
                //        if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ResolveLandmark), VLF.CLS.Def.Const.resolveLandmarksYes.ToString()), true))
                //        {
                //            //return;
                //        }
                //}
                //else
                //{
                //    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ResolveLandmark), VLF.CLS.Def.Const.resolveLandmarksNo.ToString()), false))
                //        if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ResolveLandmark), VLF.CLS.Def.Const.resolveLandmarksNo.ToString()), true))
                //        {
                //            //return;
                //        }
                //}


                if (this.chkShowReadMess.Checked)
                {
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ShowReadMessages), "1"), false))
                        if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ShowReadMessages), "1"), false))
                        {
                            //return;
                        }
                }
                else
                {
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ShowReadMessages), "0"), false))
                        if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ShowReadMessages), "0"), false))
                        {
                            //return;
                        }
                }


                sn.User.AlarmRefreshFrequency = Convert.ToInt32(this.cboAlarmFreq.SelectedItem.Value);


                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.AlarmRefreshFrequency), this.cboAlarmFreq.SelectedItem.Value), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.AlarmRefreshFrequency), this.cboAlarmFreq.SelectedItem.Value), false))
                    {
                        //return;
                    }


                sn.User.GeneralRefreshFrequency = Convert.ToInt32(this.cboRefreshFreq.SelectedItem.Value);

                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.GeneralRefreshFrequency), this.cboRefreshFreq.SelectedItem.Value), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.GeneralRefreshFrequency), this.cboRefreshFreq.SelectedItem.Value), false))
                    {
                        //return;
                    }

                sn.Map.DgVisibleRows = Convert.ToInt32(this.cboMapGridRows.SelectedItem.Value);

                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapGridDefaultRows), this.cboMapGridRows.SelectedItem.Value), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapGridDefaultRows), this.cboMapGridRows.SelectedItem.Value), false))
                    {
                        //return;
                    }


                sn.History.DgVisibleRows = Convert.ToInt32(this.cboHistoryGridRows.SelectedItem.Value);

                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.HistoryGridDefaultRows), this.cboHistoryGridRows.SelectedItem.Value), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.HistoryGridDefaultRows), this.cboHistoryGridRows.SelectedItem.Value), false))
                    {
                        //return;
                    }


                if (optMapView.SelectedItem.Value == "0" || optMapView.SelectedItem.Value == "2")
                    sn.User.ViewMDTMessagesScrolling = 1;
                else
                    sn.User.ViewMDTMessagesScrolling = 0;

                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ViewMDTMessagesScrolling), sn.User.ViewMDTMessagesScrolling.ToString()), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ViewMDTMessagesScrolling), sn.User.ViewMDTMessagesScrolling.ToString()), false))
                    {
                        //return;
                    }


                if (optMapView.SelectedItem.Value == "1" || optMapView.SelectedItem.Value == "2")
                    sn.User.ViewAlarmScrolling = 1;
                else
                    sn.User.ViewAlarmScrolling = 0;


                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ViewAlarmMessagesScrolling), sn.User.ViewAlarmScrolling.ToString()), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ViewAlarmMessagesScrolling), sn.User.ViewAlarmScrolling.ToString()), false))
                    {
                        //return;
                    }

                
                if (optTemperature.SelectedItem.Value == "0")
                    sn.User.TemperatureType = "Fahrenheit";
                else
                    sn.User.TemperatureType = "Celsius";

                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.TemperatureType), sn.User.TemperatureType), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.TemperatureType), sn.User.TemperatureType), false))
                    {
                        //return;
                    }
                


                if (this.chkShowMapGridFilter.Checked)
                    sn.User.ShowMapGridFilter = 1;
                else
                    sn.User.ShowMapGridFilter = 0;

                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ShowMapGridFilter), sn.User.ShowMapGridFilter.ToString()), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ShowMapGridFilter), sn.User.ShowMapGridFilter.ToString()), false))
                    {
                        //return;
                    }

                if (this.chkRememberLastPage.Checked)
                    sn.User.RemberLastPage = 1;
                else
                    sn.User.RemberLastPage = 0;

                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.RememberLastPage), sn.User.RemberLastPage.ToString()), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.RememberLastPage), sn.User.RemberLastPage.ToString()), false))
                    {
                        //return;
                    }


                if (this.chkShowReadMess.Checked)
                    sn.User.ShowReadMess = 1;
                else
                    sn.User.ShowReadMess = 0;


                int TotalMinutes = Convert.ToInt32(this.cboExpDays.SelectedItem.Value) * 1440 + Convert.ToInt32(this.cboExpHours.SelectedItem.Value) * 60 + Convert.ToInt32(this.cboExpMin.SelectedItem.Value);
                sn.User.PositionExpiredTime = TotalMinutes;

                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.PositionExpiredTime), TotalMinutes.ToString()), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.PositionExpiredTime), TotalMinutes.ToString()), false))
                    {
                        //return;
                    }

                sn.Map.ShowLandmark = this.chkShowLandmark.Checked;
                sn.Map.ShowLandmarkname = this.chkShowLandmarkname.Checked;
                sn.Map.ShowVehicleName = this.chkShowVehicleName.Checked;
                sn.User.ShowRetiredVehicles = this.chkShowRetiredVehicles.Checked;


                string ShowLandmarkName = sn.Map.ShowLandmarkname == true ? "1" : "0";
                string ShowLandmark = sn.Map.ShowLandmark == true ? "1" : "0";
                string ShowVehicleName = sn.Map.ShowVehicleName == true ? "1" : "0";
                string ShowRetiredVehicles = sn.User.ShowRetiredVehicles == true ? "1" : "0";


                sn.Message.MsgsCheckSum = "-1";
                sn.Map.AlarmsCheckSum = "-1";

                //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb) 
                //{
                //   if (sn.Map.ShowLandmark)
                //      sn.MapSolute.LoadDefaultMap(sn);
                //   else
                //      sn.MapSolute.LoadDefaultMapWithoutLandmarks(sn);
                //}


                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmark), ShowLandmark), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmark), ShowLandmark), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " MapOptShowLandmark . User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }


                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmarkName), ShowLandmarkName), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmarkName), ShowLandmarkName), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " MapOptShowLandmarkName . User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }


                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowVehicleName), ShowVehicleName), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowVehicleName), ShowVehicleName), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " MapOptShowVehicleName . User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }


                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultLanguage), sn.SelectedLanguage), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultLanguage), sn.SelectedLanguage), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " MapOptShowVehicleName . User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }

                //for vehicle Not Reported
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleNotReported), this.txtVehicleNotReported.Text), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleNotReported), this.txtVehicleNotReported.Text), true))
                    {
                        //return;
                    }

                //Show Retired Vehicles
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ShowRetiredVehicles), ShowRetiredVehicles), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ShowRetiredVehicles), ShowRetiredVehicles), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " ShowRetiredVehicles . User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }


                //if (userIdToUpdate == sn.UserID)
                ////Response.Write("<script language='javascript'>window.close()</script>");// Commented By Salman, June 26, 2014 and added following {}
                //{
                //    System.Web.Security.FormsAuthentication.SignOut(); // The SignOut method invalidates the authentication cookie.

                //    string destination = "../Login.aspx";
                //    Response.Write("<SCRIPT Language='javascript'>window.open('" + destination + "','_top') </SCRIPT>");
                //}
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


        private void FindExistingPreference()
        {
            try
            {
                DataSet dsPref = new DataSet();
                StringReader strrXML = null;
                dbu = new ServerDBUser.DBUser();

                string xml = "";
                if (objUtil.ErrCheck(dbu.GetUserPreferencesXML_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserPreferencesXML_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    return;
                }

                strrXML = new StringReader(xml);
                dsPref.ReadXml(strrXML);
                bool alarmSelected = false;
                bool mdtSelected = false;
                string tempratureSelected = "Fahrenheit";

                //this.txtMaxVehiclesOnMap.Text = "500";

                foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                {

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MeasurementUnits))
                    {
                        if (rowItem["PreferenceValue"].ToString() != "")
                        {
                            this.cboUnits.SelectedIndex = -1;
                            cboUnits.Items.FindByValue(rowItem["PreferenceValue"].ToString().TrimEnd()).Selected = true;
                        }
                    }

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VolumeUnits))
                    {
                        if (rowItem["PreferenceValue"].ToString() != "")
                        {
                            this.cboVolumeUnits.SelectedIndex = -1;
                            cboVolumeUnits.Items.FindByValue(rowItem["PreferenceValue"].ToString().TrimEnd()).Selected = true;
                        }
                    }
                    // Changes For TimeZone Feature start

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.TimeZoneNew))
                    {
                        if (rowItem["PreferenceValue"].ToString() != "")
                        {
                            this.cboTimeZone.SelectedIndex = -1;
                            cboTimeZone.Items.FindByValue(rowItem["PreferenceValue"].ToString().TrimEnd()).Selected = true;
                        }
                    }
                    // Changes For TimeZone Feature end

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.TimeZone))
                    {
                        if (rowItem["PreferenceValue"].ToString() != "")
                        {
                            this.cboTimeZone.SelectedIndex = -1;
                            cboTimeZone.Items.FindByValue(rowItem["PreferenceValue"].ToString().TrimEnd()).Selected = true;
                        }
                    }


                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultFleet))
                    {
                        if (rowItem["PreferenceValue"].ToString() != "")
                        {
                            this.cboFleet.SelectedIndex = -1;
                            try
                            {
                                cboFleet.Items.FindByValue(rowItem["PreferenceValue"].ToString().TrimEnd()).Selected = true;
                            }
                            catch
                            {
                            }
                        }
                    }


                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapAssets))
                    {
                        if (Convert.ToInt32(rowItem["PreferenceValue"]) == VLF.CLS.Def.Const.mapAssetsYes)
                            this.chkMapAssets.Checked = true;
                        else
                            this.chkMapAssets.Checked = false;
                    }

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.LoadLandmarkByDefault))
                    {
                        if (Convert.ToInt32(rowItem["PreferenceValue"]) == 1)
                            this.chkLoadLandmarkByDefault.Checked = true;
                        else
                            this.chkLoadLandmarkByDefault.Checked = false;
                    }

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleClustering))
                    {
                        if (Convert.ToInt32(rowItem["PreferenceValue"]) == VLF.CLS.Def.Const.VehicleClusteringYes)
                            this.chkVehicleClustering.Checked = true;
                        else
                            this.chkVehicleClustering.Checked = false;
                    }

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleClusteringDistance))
                    {
                        int vehicleClusteringDistance = 20;
                        Int32.TryParse(rowItem["PreferenceValue"].ToString(), out vehicleClusteringDistance);
                        this.txtVehicleClusteringDistance.Text = vehicleClusteringDistance.ToString();
                    }

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleClusteringThreshold))
                    {
                        if (rowItem["PreferenceValue"].ToString() != "")
                        {
                            this.cboVehicleClusteringThreshold.SelectedIndex = -1;
                            try
                            {
                                this.cboVehicleClusteringThreshold.Items.FindByValue(rowItem["PreferenceValue"].ToString().TrimEnd()).Selected = true;
                            }
                            catch
                            {
                            }
                        }
                    }

                    /*if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MaxVehiclesOnMap))
                    {
                        int maxVehiclesOnMap = 500;
                        Int32.TryParse(rowItem["PreferenceValue"].ToString(), out maxVehiclesOnMap);
                        this.txtMaxVehiclesOnMap.Text = maxVehiclesOnMap.ToString();
                    }*/

                    //if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ResolveLandmark))
                    //{
                    //    if (Convert.ToInt32(rowItem["PreferenceValue"]) == VLF.CLS.Def.Const.resolveLandmarksYes)
                    //        this.chkLandmark.Checked = true;
                    //    else
                    //        this.chkLandmark.Checked = false;
                    //}


                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.AutoAdjustDayLightSaving))
                    {
                        if (Convert.ToInt32(rowItem["PreferenceValue"]) == 1)
                            this.chkDaylight.Checked = true;
                        else
                            this.chkDaylight.Checked = false;
                    }

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DayLightSaving))
                        ViewState["DayLightSavingExist"] = "true";


                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ShowReadMessages))
                    {
                        if (Convert.ToInt32(rowItem["PreferenceValue"]) == 1)
                            this.chkShowReadMess.Checked = true;
                        else
                            this.chkShowReadMess.Checked = false;
                    }


                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.AlarmRefreshFrequency))
                    {
                        sn.User.AlarmRefreshFrequency = Convert.ToInt32(rowItem["PreferenceValue"]);
                        this.cboAlarmFreq.SelectedIndex = -1;
                        cboAlarmFreq.Items.FindByValue(rowItem["PreferenceValue"].ToString().TrimEnd()).Selected = true;
                    }


                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.GeneralRefreshFrequency))
                    {
                        try
                        {
                            sn.User.GeneralRefreshFrequency = Convert.ToInt32(rowItem["PreferenceValue"]);
                            this.cboRefreshFreq.SelectedIndex = -1;
                            cboRefreshFreq.Items.FindByValue(rowItem["PreferenceValue"].ToString().TrimEnd()).Selected = true;
                        }
                        catch
                        {
                        }
                    }


                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.PositionExpiredTime))
                    {
                        try
                        {
                            cboExpDays.SelectedIndex = -1;
                            cboExpHours.SelectedIndex = -1;
                            cboExpMin.SelectedIndex = -1;

                            sn.User.PositionExpiredTime = Convert.ToInt32(rowItem["PreferenceValue"]);
                            TimeSpan ExpiredDateTime = new TimeSpan(sn.User.PositionExpiredTime * TimeSpan.TicksPerMinute);
                            cboExpDays.Items.FindByValue((ExpiredDateTime.Days).ToString()).Selected = true;
                            cboExpHours.Items.FindByValue(ExpiredDateTime.Hours.ToString()).Selected = true;
                            cboExpMin.Items.FindByValue(ExpiredDateTime.Minutes.ToString()).Selected = true;
                        }
                        catch
                        {
                        }
                    }


                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapGridDefaultRows))
                    {

                        sn.Map.DgVisibleRows = Convert.ToInt32(rowItem["PreferenceValue"]);
                        this.cboMapGridRows.SelectedIndex = -1;
                        cboMapGridRows.Items.FindByValue(rowItem["PreferenceValue"].ToString().TrimEnd()).Selected = true;
                    }
                    

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.HistoryGridDefaultRows))
                    {

                        sn.History.DgVisibleRows = Convert.ToInt32(rowItem["PreferenceValue"]);
                        this.cboHistoryGridRows.SelectedIndex = -1;
                        cboHistoryGridRows.Items.FindByValue(rowItem["PreferenceValue"].ToString().TrimEnd()).Selected = true;
                    }


                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ViewMDTMessagesScrolling))
                    {
                        ViewState["ViewMDTMessagesScrolling"] = "true";
                        if (Convert.ToInt16(rowItem["PreferenceValue"]) == 1)
                            mdtSelected = true;
                    }


                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ViewAlarmMessagesScrolling))
                    {
                        ViewState["ViewAlarmMessagesScrolling"] = "true";
                        if (Convert.ToInt16(rowItem["PreferenceValue"]) == 1)
                            alarmSelected = true;
                    }

                  
                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.TemperatureType))
                    {
                        tempratureSelected = rowItem["PreferenceValue"].ToString();
                    }


                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ShowMapGridFilter))
                    {
                        ViewState["ShowMapGridFilter"] = "true";
                        if (Convert.ToInt16(rowItem["PreferenceValue"]) == 1)
                            this.chkShowMapGridFilter.Checked = true;
                        else
                            this.chkShowMapGridFilter.Checked = false;
                    }

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.RememberLastPage))
                    {
                        ViewState["RememberLastPage"] = "true";
                        if (Convert.ToInt16(rowItem["PreferenceValue"]) == 1)
                            this.chkRememberLastPage.Checked = true;
                        else
                            this.chkRememberLastPage.Checked = false;
                    }

                    //For Vehicle Not Reported
                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleNotReported))
                    {
                        if (rowItem["PreferenceValue"].ToString() != "")
                        {
                            //this.ddlVehicleNotReported.SelectedIndex = -1;
                            //ddlVehicleNotReported.Items.FindByValue(rowItem["PreferenceValue"].ToString().TrimEnd()).Selected = true;
                            txtVehicleNotReported.Text = rowItem["PreferenceValue"].ToString();
                        }
                    }

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultMapView))
                    {
                        try
                        {
                            this.cboDefaultMapView.SelectedIndex = -1;
                            this.cboDefaultMapView.Items.FindByValue(rowItem["PreferenceValue"].ToString().TrimEnd()).Selected = true;
                        }
                        catch
                        {
                        }
                    }

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == 36)
                    {
                        this.cboLoadVehiclesBasedOn.SelectedIndex = -1;
                        this.cboLoadVehiclesBasedOn.Items.FindByValue(rowItem["PreferenceValue"].ToString().TrimEnd()).Selected = true;
                    }

                    //Show Retired Vehicles
                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.ShowRetiredVehicles))
                    {
                        if (Convert.ToInt32(rowItem["PreferenceValue"]) == 1)
                            this.chkShowRetiredVehicles.Checked = true;
                        else
                            this.chkShowRetiredVehicles.Checked = false;
                    }

               if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode))
               {
                   string nodecode = rowItem["PreferenceValue"].ToString().TrimEnd();
                   //this.DefaultOrganizationHierarchyNodeCode.Value = nodecode;
                   poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                   nodecode = poh.ValidatedNodeCodes(sn.User.OrganizationId, sn.UserID, nodecode);
                   this.DefaultOrganizationHierarchyNodeCode.Value = nodecode;// poh.ValidatedNodeCodes(sn.User.OrganizationId, sn.UserID, nodecode);
                   this.defaultOrganizationHierarchy.Text = nodecode;
                   
                   OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, nodecode);
                   OrganizationHierarchyNamePath = poh.GetOrganizationHierarchyNamePath(sn.User.OrganizationId, nodecode);
                   if (MutipleUserHierarchyAssignment)
                   {
                       //OrganizationHierarchyMultiplePath = getOrganizationHierarchyMultiplePath(nodecode);
                       OrganizationHierarchyPath = getOrganizationHierarchyMultiplePath(nodecode);
                       OrganizationHierarchyNamePath = getOrganizationHierarchyMultipleNamePath(nodecode);
                       this.defaultOrganizationHierarchy.Text = OrganizationHierarchyNamePath.Replace(";", "<div style='border-bottom:1px solid #aaaaaa;height:1px;width:100%;'></div>");
                   }
               }

                    this.chkShowLandmark.Checked = sn.Map.ShowLandmark;
                    this.chkShowLandmarkname.Checked = sn.Map.ShowLandmarkname;
                    this.chkShowVehicleName.Checked = sn.Map.ShowVehicleName;
                    

                }
                if (alarmSelected && mdtSelected)
                    optMapView.SelectedIndex = 2;
                else if (alarmSelected && !mdtSelected)
                    optMapView.SelectedIndex = 1;
                else if (!alarmSelected && mdtSelected)
                    optMapView.SelectedIndex = 0;
                else
                    optMapView.SelectedIndex = 3;

                if (tempratureSelected == "Fahrenheit")
                    optTemperature.SelectedIndex = 0;
                else
                    optTemperature.SelectedIndex = 1;

               
                ddlDateFormat.SelectedValue = sn.User.DateFormat;
                ddlTimeFormat.SelectedValue = sn.User.TimeFormat;
            }

            catch
            {
                //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
                //RedirectToLogin();
            }
        }


        private void cmdVehicles_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmVehicleInfo.aspx");
        }

        protected void cmdSavePsw_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.lblPswMsg.Text = "";

                if (this.cmdSavePsw.Text == (string)base.GetLocalResourceObject("cmdEditPsw"))
                {
                    this.txtNewPassword.Enabled = true;
                    this.txtNewPassword1.Enabled = true;
                    this.txtOldPassword.Enabled = true;
                    this.vlComp.Enabled = true;
                    this.valNewPassword.Enabled = true;
                    this.valOldPassword.Enabled = true;
                    this.cmdSavePsw.Text = (string)base.GetLocalResourceObject("cmdSavePsw");
                    return;
                }


                string txtPasswordStatus = Request.Form["txtPasswordStatus"];
                if (txtPasswordStatus == "")
                    return;
                else if (txtPasswordStatus != "1")
                {
                    this.lblPswMsg.Text = (string)base.GetLocalResourceObject("msgWeakPassword");
                    this.lblPswMsg.Visible = true;
                    return;
                }

                dbu = new ServerDBUser.DBUser();
                int changeResult = dbu.ChangeUserPasswordByUserName(sn.UserID, sn.SecId, sn.UserName, this.txtOldPassword.Text, this.txtNewPassword.Text);
                if (objUtil.ErrCheck(changeResult, false))
                    if (objUtil.ErrCheck(changeResult, true))
                    {
                        if (changeResult == 21)
                        {
                            this.lblPswMsg.Text = (string)base.GetLocalResourceObject("AddUserPasswordFailed21");
                        }
                        else if (changeResult == 22)
                        {
                            this.lblPswMsg.Text = (string)base.GetLocalResourceObject("AddUserPasswordFailed22");
                        }
                        else if (changeResult == 4)
                        {
                            this.lblPswMsg.Text = (string)base.GetLocalResourceObject("msgPsw_OldPswIncorrect");
                        }
                        else
                        {
                            this.lblPswMsg.Text = (string)base.GetLocalResourceObject("MsgUpdateFailed");
                        }
                        this.lblMessageError.Visible = false;
                        this.lblPswMsg.Visible = true;
                        return;
                    }


                this.lblMessageError.Visible = false;
		        this.lblPswMsg.Visible = true;

                if (sn.User.OrganizationId == 1000065) // Only Burnbrae has the mobile link
                {
                    this.lblPswMsg.Text = (string)base.GetLocalResourceObject("cmdPswUpdatedAmeco");
                }
                else
                {
                    this.lblPswMsg.Text = (string)base.GetLocalResourceObject("cmdPswUpdated");
                }
                sn.Password = this.txtNewPassword.Text;


                if (this.cmdSavePsw.Text == (string)base.GetLocalResourceObject("cmdSavePsw"))
                {
                    this.txtNewPassword.Enabled = false;
                    this.txtNewPassword1.Enabled = false;
                    this.txtOldPassword.Enabled = false;
                    this.vlComp.Enabled = false;
                    this.valNewPassword.Enabled = false;
                    this.valOldPassword.Enabled = false;
                    this.cmdSavePsw.Text = (string)base.GetLocalResourceObject("cmdEditPsw");
                }

                //Session["SentinelFMSession"] = null;
                //Session.Abandon();

                //if (userIdToUpdate == sn.UserID)
                ////Response.Write("<script language='javascript'>window.close()</script>");// Commented By Salman, June 26, 2014 and added following {}
                //{
                //    System.Web.Security.FormsAuthentication.SignOut(); // The SignOut method invalidates the authentication cookie.

                //    string destination = "../Login.aspx";
                //    Response.Write("<SCRIPT Language='javascript'>window.open('" + destination + "','_top') </SCRIPT>");
                //}
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


        private void CboFleet_Fill()
        {
            try
            {
                DataSet dsFleets = new DataSet();
                StringReader strrXML = null;


                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        cboFleet.DataSource = null;
                        return;
                    }

                if (xml == "")
                {
                    return;
                }
                strrXML = new StringReader(xml);
                dsFleets.ReadXml(strrXML);

                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();

                cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectFleet"), "-1"));
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

        private void cmdCompany_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmEmails.aspx");
        }


        private void cboRefreshFreq_Fill()
        {

            cboRefreshFreq.Items.Clear();

            if (sn.User.MapEngine[0].MapEngineID == VLF.MAP.MapType.MapPointWeb)
            {
                ListItem ls = new ListItem();
                ls.Value = "1800000";
                ls.Text = "30 min";
                cboRefreshFreq.Items.Add(ls);

                ListItem ls1 = new ListItem();
                ls1.Value = "2700000";
                ls1.Text = "45 min";
                cboRefreshFreq.Items.Add(ls1);

                ListItem ls2 = new ListItem();
                ls2.Value = "3600000";
                ls2.Text = "1 hour";
                cboRefreshFreq.Items.Add(ls2);
            }
            else
            {

                ListItem ls0 = new ListItem();
                ls0.Value = "0";
                ls0.Text = "0";
                cboRefreshFreq.Items.Add(ls0);

                ListItem ls15s = new ListItem();
                ls15s.Value = "15000";
                ls15s.Text = "15 sec";
                cboRefreshFreq.Items.Add(ls15s);

                ListItem ls30s = new ListItem();
                ls30s.Value = "30000";
                ls30s.Text = "30 sec";
                cboRefreshFreq.Items.Add(ls30s);

                ListItem ls = new ListItem();
                ls.Value = "60000";
                ls.Text = "1 min";
                cboRefreshFreq.Items.Add(ls);

                ListItem ls1 = new ListItem();
                ls1.Value = "120000";
                ls1.Text = "2 min";
                cboRefreshFreq.Items.Add(ls1);

                ListItem ls2 = new ListItem();
                ls2.Value = "180000";
                ls2.Text = "3 min";
                cboRefreshFreq.Items.Add(ls2);

                ListItem ls3 = new ListItem();
                ls3.Value = "300000";
                ls3.Text = "5 min";
                cboRefreshFreq.Items.Add(ls3);

                ListItem ls4 = new ListItem();
                ls4.Value = "600000";
                ls4.Text = "10 min";
                cboRefreshFreq.Items.Add(ls4);
            }
        }


        protected override void InitializeCulture()
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(sn.SelectedLanguage);
            //base.InitializeCulture();
        }
        protected void lnkEnglish_Click(object sender, EventArgs e)
        {
            dbu = new ServerDBUser.DBUser();
            sn.SelectedLanguage = "en-US";
            Session["PreferredCulture"] = sn.SelectedLanguage;
            if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultLanguage), sn.SelectedLanguage), false))
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultLanguage), sn.SelectedLanguage), true))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " MapOptShowVehicleName . User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                }

            Response.Write("<script language='javascript'>parent.menu.window.location.href='../frmTopMenu.aspx'; parent.main.window.location.href='frmpreference.aspx'; </script>");
        }

        protected void lnkFrench_Click(object sender, EventArgs e)
        {
            dbu = new ServerDBUser.DBUser();
            sn.SelectedLanguage = "fr-CA";
            Session["PreferredCulture"] = sn.SelectedLanguage;

            if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultLanguage), sn.SelectedLanguage), false))
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, userIdToUpdate, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultLanguage), sn.SelectedLanguage), true))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " MapOptShowVehicleName . User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                }

            Response.Write("<script language='javascript'>parent.menu.window.location.href='../frmTopMenu.aspx'; parent.main.window.location.href='frmpreference.aspx'; </script>");
        }


        protected void cmdHome_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["errormsg"] != null)
            {
                if (sn.User.OrganizationId == 1000065)
                    Response.Redirect("../Loginameco.aspx");
                else
                    Response.Redirect("../login.aspx");
            }
            else
            {
                // Response.Redirect("../Home/frmMainHome.aspx");
                this.cmdSavePsw.Text = (string)base.GetLocalResourceObject("cmdEditPsw");                
                this.txtNewPassword.Text = string.Empty;
                this.txtNewPassword1.Text = string.Empty;
                this.txtOldPassword.Text = string.Empty;
                this.lblPswMsg.Text = string.Empty;
                this.txtNewPassword.Enabled = false;
                this.txtNewPassword1.Enabled = false;
                this.txtOldPassword.Enabled = false;
                this.vlComp.Enabled = false;
                this.valNewPassword.Enabled = false;
                this.valOldPassword.Enabled = false;
            }
        }


        private void MapTypeSet()
        {
            try
            {
                StringReader strrXML = null;
                ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
                DataSet dsMapEngines = new DataSet();

                string xml = "";
                if (objUtil.ErrCheck(dbs.GetUserMapEngineInfoXML(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbs.GetUserMapEngineInfoXML(sn.UserID, sn.SecId, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetUserMapEngineInfoXML. User:" + sn.UserID.ToString() + Page.GetType().Name));
                    }
                if (xml != "")
                {
                    xml = xml.Replace("Standard Maps", Resources.Const.StandardMaps);

                    strrXML = new StringReader(xml);
                    dsMapEngines.ReadXml(strrXML);
                    this.cboMapType.DataSource = dsMapEngines;

                    cboMapType.DataBind();
                    cboMapType.SelectedIndex = cboMapType.Items.IndexOf(cboMapType.Items.FindByValue(Convert.ToInt16(sn.User.MapType).ToString()));
                    ViewState["dsMapEngines"] = dsMapEngines;
                }
                else
                {
                    this.cboMapType.DataSource = null;
                    cboMapType.DataBind();
                    ViewState["dsMapEngines"] = null;
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void cboMapType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                StringReader strrXML = null;
                dbu = new ServerDBUser.DBUser();
                string xml = "";
                DataSet dsMapEngines = new DataSet();
                DataSet dsGeoCodeEngines = new DataSet();


                sn.User.MapType = (VLF.MAP.MapType)Convert.ToInt16(this.cboMapType.SelectedItem.Value);
                dsMapEngines = (DataSet)ViewState["dsMapEngines"];
                if (dsMapEngines == null)
                    return;

                DataRow[] drArr = dsMapEngines.Tables[0].Select("MapId=" + cboMapType.SelectedItem.Value);
                if (drArr == null || drArr.Length == 0)
                    return;


                xml = "<System><GetUserMapEngineInfo><MapId>" + drArr[0]["MapId"] + "</MapId><Path>" + drArr[0]["Path"] + "</Path><ExternalPath>" + drArr[0]["ExternalPath"] + "</ExternalPath></GetUserMapEngineInfo></System>";
                strrXML = new StringReader(xml);
                dsMapEngines.Clear();
                dsMapEngines.ReadXml(strrXML);
                sn.User.MapEngine = VLF.MAP.MapUtilities.ConvertMapsToMapEngine(dsMapEngines);
                sn.Map.ShowDefaultMap = true;

                //Geocode engine
                ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
                if (objUtil.ErrCheck(dbs.GetUserGeoCodeEngineInfoXML(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbs.GetUserGeoCodeEngineInfoXML(sn.UserID, sn.SecId, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetUserGeoCodeEngineInfoXML. User:" + sn.UserID.ToString() + Page.GetType().Name));
                        return;
                    }
                if (xml != "")
                {
                    strrXML = new StringReader(xml);
                    dsGeoCodeEngines.ReadXml(strrXML);
                }

                if (dsGeoCodeEngines == null)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetUserGeoCodeEngineInfoXML. User:" + sn.UserID.ToString() + Page.GetType().Name));
                    return;
                }

                DataRow[] drArrGeoCode = dsGeoCodeEngines.Tables[0].Select("Priority=" + drArr[0]["Priority"]);
                if (drArrGeoCode == null || drArrGeoCode.Length == 0)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No GeoCodeEngine define with same priority as map. User:" + sn.UserID.ToString() + Page.GetType().Name));
                    return;
                }

                xml = "<System><GetUserGeoCodeEngineInfo><GeoCodeId>" + drArrGeoCode[0]["GeoCodeId"] + "</GeoCodeId><Path>" + drArrGeoCode[0]["Path"] + "</Path></GetUserGeoCodeEngineInfo></System>";
                strrXML = new StringReader(xml);
                dsGeoCodeEngines.ReadXml(strrXML);
                sn.User.GeoCodeEngine = VLF.MAP.MapUtilities.ConvertGeoCodersToMapEngine(dsGeoCodeEngines);
            }

            catch (Exception Ex)
            {

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

       private string getOrganizationHierarchyMultiplePath(string _nodecodes)
       {
           string[] nlist = _nodecodes.Split(',');
           string paths = string.Empty;
           foreach (string s in nlist)
           {
               string p = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, s);
               paths = paths + (paths == string.Empty ? "" : ";") + p;
           }
           return paths;
       }

       private string getOrganizationHierarchyMultipleNamePath(string _nodecodes)
       {
           string[] nlist = _nodecodes.Split(',');
           string paths = string.Empty;
           foreach (string s in nlist)
           {
               string p = poh.GetOrganizationHierarchyNamePath(sn.User.OrganizationId, s);
               paths = paths + (paths == string.Empty ? "" : ";") + p;
           }
           return paths;
       }

       private void ddlDateFormat_Fill()
       {

           try
           {
               clsUtility objUtil;
               objUtil = new clsUtility(sn);
               DataSet dsInfo=null;
               dbu = new ServerDBUser.DBUser();
               if (objUtil.ErrCheck(dbu.GetDateTimeFormats(sn.UserID, sn.SecId, ref dsInfo), false))
               {
                   if (objUtil.ErrCheck(dbu.GetDateTimeFormats(sn.UserID, sn.SecId, ref dsInfo), true))
                   {
                   }
               }
               if(dsInfo!=null)
               {
                   ddlDateFormat.DataSource=dsInfo.Tables[0].Select("Type= 'Date'").CopyToDataTable();
                   ddlDateFormat.DataTextField = "Format";
                   ddlDateFormat.DataValueField = "Format";
                   ddlDateFormat.DataBind();
               }
           }
           catch (Exception ex)
           {
           }
       }
    }
}
