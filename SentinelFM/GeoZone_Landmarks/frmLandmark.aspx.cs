using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web.Services;
using SentinelFM.GeomarkServiceRef;


namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmLandmark.
    /// </summary>
    public partial class frmLandmark : SentinelFMBasePage
    {


        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        protected ServerDBOrganization.DBOrganization dbo;
        protected System.Web.UI.HtmlControls.HtmlForm Configuration;
        string confirm;
        public string LandmarkMap = "";
        public string ViewLandmarkMap = "";
        public string drawPolygonText = "Please draw a Rectangle or Polygon.";
        public string deleteFail = "Failed to delete.";
        VLF.DAS.Logic.LandmarkPointSetManager landPointMgr = null;
        private VLF.PATCH.Logic.PatchServices _ps = null;        

        public bool enableAssignment = false;
        public bool enableTimer = false;

        public int LandmarkId = 0;
        public bool isLandmarkUpdatableByUser = true;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!sn.User.ControlEnable(sn, 102))
                Response.Redirect("../Home/frmHome.aspx");

            try
            {
                landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);
                //Clear IIS cache
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);

                //Enable Search on pressing Enter Key
                txtSearchParam.Attributes.Add("onkeypress", "return clickButton(event,'" + cmdSearch.ClientID + "')");

                _ps = new VLF.PATCH.Logic.PatchServices(sConnectionString);

                enableAssignment = clsPermission.FeaturePermissionCheck(sn, "LandmarkFleetAssignment");
                bool enableCallTimer = clsPermission.FeaturePermissionCheck(sn, "CallTimer");

                if (sn.User.UserGroupId == 36 && sn.User.OrganizationId == 952)
                {
                    cmdSearch.Enabled = false;
                    enableAssignment = true;
                }
                  

                //if ((sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2 || sn.User.UserGroupId == 7 || sn.UserID == 11967) && (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 952 || sn.User.OrganizationId == 999952 || sn.User.OrganizationId == 999954)) //Hgi and Security Administrator user 
                //{
                //    enableAssignment = true;                    
                //}

                if (!Page.IsPostBack)
                {
                    trAssignmentPopup.Visible = false;
                    
                    //if (sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2) //Hgi and Security Administrator user 
                    if (enableAssignment) //Hgi and Security Administrator user 
                    {
                        trAssignment.Visible = true;
                        CboFleet_Fill();
                    }

                    DisableOptSelection();
                }

                //if ((sn.UserID == 962) || ((sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2 || sn.User.UserGroupId == 7 || sn.UserID == 11967) && (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 952 || sn.User.OrganizationId == 999952 || sn.User.OrganizationId == 999954))) //Hgi and Security Administrator user 
                if(enableCallTimer)
                {
                    enableTimer = true;
                    trTimer.Visible = true;
                    if (!Page.IsPostBack)
                        CboServices_Fill();
                }

                if (sn.User.UserGroupId == 27 && sn.UserID != 11967)
                {
                    cmdSaveLandmark.Enabled = false;
                    cmdCancelLandmark.Enabled = false;
                    return;
                }

                if ((sn == null) || (sn.UserName == ""))
                {
                    RedirectToLogin();
                    return;
                }

                //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                //{
                //   if (ViewState["LandmarkId"]!=null && ViewState["LandmarkId"]!= "")
                //      LandmarkMap = "frmMapsoluteLandmarks.aspx?LandmarkId=" + ViewState["LandmarkId"].ToString() ;
                //   else
                //      LandmarkMap = "frmMapsoluteLandmarks.aspx";

                //   ViewLandmarkMap = "frmMapsoluteLandmarks.aspx";
                //}
                if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                {
                    //LandmarkMap = "frmLandmarkGeoZoneVE.aspx?FormName=Landmark";
                    //ViewLandmarkMap = "frmLandmarkGeoZoneVE.aspx";

                    LandmarkMap = "../MapVE/VELandmark.aspx?FormName=Landmark";
                    ViewLandmarkMap = "../MapVE/VELandmarks_GeoZones.aspx";
                }
                else
                {
                    //LandmarkMap = "frmMap.aspx?FormName=Landmark";
                    //ViewLandmarkMap = "frmViewLandMark.aspx";

                    //LandmarkMap = "../MapNew/frmDrawLandmarkGeozone.aspx?FormName=Landmark";

                    if (sn.Landmark.EditLandmarkMode)
                    {
                        VLF.PATCH.Logic.PatchLandmark _landmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
                        int oLandmarkId = _landmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, txtLandmarkName.Text);
                        if (oLandmarkId > 0)
                        {
                            dbo = new ServerDBOrganization.DBOrganization();
                            dbo.IsLandmarkUpdatableByUser(sn.UserID, sn.SecId, sn.User.OrganizationId, oLandmarkId, ref isLandmarkUpdatableByUser);
                        }
                        //DisableOrLockControls(isLandmarkUpdatableByUser);
                        LandmarkMap = "../MapNew/frmDrawLandmarkGeozone.aspx?FormName=Landmark&ShowControl=" + isLandmarkUpdatableByUser.ToString();
                    }

                    ViewLandmarkMap = "../MapNew/frmViewLandmarkGeozone.aspx";
                }

                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmLandmarkForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                    if (sn.User.SmsSupport)
                        txtPhoneSMS.Enabled = true;

                    GuiSecurity(this);
                    FindExistingPreference();
                    cboTimeZone_Fill();

                    ddlCategory_Fill();


                    if ((sn.Landmark.AddLandmarkMode) || (sn.Landmark.EditLandmarkMode))
                    {
                        this.tblLandmarkAdd.Visible = true;
                        this.tblLandmarks.Visible = false;
                    }
                    else
                    {
                        this.tblLandmarks.Visible = true;
                        this.tblLandmarkAdd.Visible = false;
                    }

                    if (sn.Landmark.LandmarkByAddress)
                        lstAddOptions.SelectedIndex = lstAddOptions.Items.IndexOf(lstAddOptions.Items.FindByValue("0"));
                    else
                        lstAddOptions.SelectedIndex = lstAddOptions.Items.IndexOf(lstAddOptions.Items.FindByValue("1"));

                    ViewByOptions();

                    this.lblUnit.Text = ViewState["UnitName"].ToString();
                    ViewState["ConfirmDelete"] = "0";

                    if (sn.Landmark.X != 0)
                        this.txtX.Text = sn.Landmark.X.ToString();
                    if (sn.Landmark.Y != 0)
                        this.txtY.Text = sn.Landmark.Y.ToString();

                    if (sn.User.UserGroupId != 36 || sn.User.OrganizationId != 952)
                    {
                        sn.DsLandMarks = null;
                        //if (sn.DsLandMarks==null || sn.DsLandMarks.Tables[0].Rows.Count==0)
                        DgLandmarks_Fill_NewTZ();

                        //else
                        //{
                        //	this.dgLandmarks.DataSource= sn.DsLandMarks;
                        //	this.dgLandmarks.DataBind();  
                        //}
                    }
                    
                    //By devin
                    if (Request.QueryString["create"] != null)
                    {
                        cmdLandMarkAdd_Click(null, null);
                        lstAddOptions.ClearSelection();
                        lstAddOptions.SelectedIndex = 1;
                        lstAddOptions_SelectedIndexChanged(null, null);
                        txtX.Text = Request.QueryString["x"];
                        txtY.Text = Request.QueryString["y"];
                    }

                    if (sn.User.UserGroupId == 28 && sn.UserID != 11967)
                    {
                        this.cmdLandMarkAdd.Enabled = false;   
                    }
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
            this.dgLandmarks.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgLandmarks_ItemCommand);
            this.dgLandmarks.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgLandmarks_PageIndexChanged);
            this.dgLandmarks.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgLandmarks_DeleteCommand);
            this.dgLandmarks.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgLandmarks_ItemDataBound);

        }
        #endregion

        protected void cmdSaveLandmark_Click(object sender, System.EventArgs e)
        {
            try
            {
                string xml = "";
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();
                if (objUtil.ErrCheck(dbu.GetAssignedGroupsByUser(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetAssignedGroupsByUser(sn.UserID, sn.SecId, ref xml), true))
                    {
                        this.lblMessage.Visible = true;
                        if (!sn.Landmark.EditLandmarkMode)
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AddLandmarkAuthFailed");
                        else
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_EditLandmarkAuthFailed");
                        return;
                    }


                if (xml == "")
                {
                    this.lblMessage.Visible = true;
                    if (!sn.Landmark.EditLandmarkMode)
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AddLandmarkAuthFailed");
                    else
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_EditLandmarkAuthFailed");
                    return;
                }
                else
                {
                    if (System.Xml.Linq.XDocument.Parse(xml).Root.Elements("UserAllGroups").Count() == 1)
                    {
                        if (System.Xml.Linq.XDocument.Parse(xml).Root.Element("UserAllGroups").Element("UserGroupName").Value.ToString() == "View Only")
                        {
                           this.lblMessage.Visible = true;
                           this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AddLandmarkViewOnly");
                           return;
                        }
                    }
                }
                LandmarkSaveTelogis_NewTZ();                
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

        // Changes for TimeZone Feature start

        private void LandmarkSaveTelogis_NewTZ()
        {
            bool landmarkPublic = false;

            Page.Validate();
            if (!Page.IsValid)
                return;

            this.lblAddMessage.Text = "";

            dbo = new ServerDBOrganization.DBOrganization();


            lblMessage.Visible = true;
            string strAddress = this.txtStreet.Text + "," + this.txtCity.Text + "," + this.txtState.Text + ",," + this.cboCountry.SelectedItem.Value;
            string resolvedAddress = "";
            clsMap mp = new clsMap();
            if ((this.lstAddOptions.SelectedItem.Value == "0") && (this.txtX.Text == "0"))
            {
                double X = 0; double Y = 0;

                mp.ResolveCooridnatesByAddressTelogis(strAddress, ref X, ref Y, ref resolvedAddress);

                this.txtX.Text = X.ToString();
                this.txtY.Text = Y.ToString();

                if ((this.txtX.Text == "0") && (this.txtY.Text == "0"))
                {
                    this.lblAddMessage.Visible = true;
                    this.lblAddMessage.Text = (string)base.GetLocalResourceObject("lblAddMessage_Text_InvalidAddressError");
                    return;
                }
                if ((this.txtX.Text != "0") && (this.txtY.Text != "0"))
                {

                    if (!CheckIfNumberic(this.txtX.Text))
                    {
                        this.lblAddMessage.Visible = true;
                        this.lblAddMessage.Text = (string)base.GetLocalResourceObject("lblAddMessage_Text_InvalidLongitudeError");
                        return;
                    }

                    if (!CheckIfNumberic(this.txtY.Text))
                    {
                        this.lblAddMessage.Visible = true;
                        this.lblAddMessage.Text = (string)base.GetLocalResourceObject("lblAddMessage_Text_InvalidLatitudeError");
                        return;
                    }
                }
            }
            else if ((this.lstAddOptions.SelectedItem.Value == "1") && (this.txtX.Text != "0"))
            {
                resolvedAddress = mp.ResolveStreetAddressTelogis(this.txtX.Text, this.txtY.Text);
            }

            this.lblAddMessage.Text = "";
            if (resolvedAddress != null && resolvedAddress.Trim() != "")
                strAddress = resolvedAddress;

            bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight.Checked));
            landmarkPublic = this.optLandmarkPublicPrivate.SelectedValue == "1" ? true : false;

            if (!sn.Landmark.EditLandmarkMode)
            {
                //if (ErrCheck(dbo.AddOrganizationLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text), txtLandmarkName.Text, txtLandmarkDesc.Text, this.txtContactName.Text, this.txtPhone.Text, Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"])), this.txtEmail.Text, this.txtPhoneSMS.Text, Convert.ToInt16(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress), false))
                try
                {
                    Int32 radius = -1;
                    string pointSets = string.Empty;
                    if (LandmarkOptions.SelectedValue == "0")
                    {
                        if (Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text)) < 0)
                        {
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("valRangeRadiusResource1.ErrorMessage");
                            return;
                        }
                        radius = Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"]));
                    }

                    if (LandmarkOptions.SelectedValue == "1" || LandmarkOptions.SelectedValue == "2")
                    {
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
                            this.lblMessage.Text = drawPolygonText + " " + (string)base.GetLocalResourceObject("lblMessage_Text_AddLandmarkFailed");
                            return;
                        }
                    }

                    landPointMgr.vlfLandmarkPointSet_Add_NewTZ(sn.User.OrganizationId, txtLandmarkName.Text,
                    Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text),
                    txtLandmarkDesc.Text,
                    this.txtContactName.Text,
                    this.txtPhone.Text,
                    radius,
                    this.txtEmail.Text,
                    this.txtPhoneSMS.Text,
                    Convert.ToSingle(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress,
                    pointSets, landmarkPublic, Convert.ToInt64(this.ddlCategory.SelectedValue)
                    );
                   

                    VLF.PATCH.Logic.PatchLandmark _landmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
                    int _landmarkid = _landmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, txtLandmarkName.Text);
                    ServerDBUser.DBUser _dbu = new ServerDBUser.DBUser();

                    if (_landmarkid > 0)
                    {
                        _landmark.UpdateLandmarkCreater(_landmarkid, sn.UserID);
                        
                    }
                    int result = _dbu.RecordUserAction("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + _landmarkid.ToString(), "Add", this.Context.Request.UserHostAddress, "Geozone_landmark/frmLandmark.aspx", "Landmark " + txtLandmarkName.Text + " Added By: " + sn.LoginUserID);                         
                }
                catch (Exception Ex)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AddLandmarkFailed");
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    return;
                }

                //if (objUtil.ErrCheck(dbo.AddOrganizationLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text), txtLandmarkName.Text, txtLandmarkDesc.Text, this.txtContactName.Text, this.txtPhone.Text, Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"])), this.txtEmail.Text, this.txtPhoneSMS.Text, Convert.ToInt16(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress), true))
                //{
                //}
            }
            else
            {

                try
                {
                    VLF.PATCH.Logic.PatchLandmark _landmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
                    int _landmarkid = _landmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, txtLandmarkName.Text);

                    ServerDBUser.DBUser _dbu = new ServerDBUser.DBUser();
                    //check if landmark audit log already exists in the AuditLogs table
                    int result = _dbu.RecordInitialValues("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + _landmarkid.ToString(), "Edit", this.Context.Request.UserHostAddress, "Geozone_landmark/frmLandmark.aspx", "Landmark " + txtLandmarkName.Text + " Edited By: " + sn.LoginUserID);

                    Int32 radius = -1;
                    string pointSets = string.Empty;
                    if (LandmarkOptions.SelectedValue == "0")
                        radius = Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"]));
                    if (LandmarkOptions.SelectedValue == "1" || LandmarkOptions.SelectedValue == "2")
                    {
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
                            this.lblMessage.Text = drawPolygonText + " " + (string)base.GetLocalResourceObject("lblMessage_Text_UpdateLandmarkFailedError");
                        }
                    }

                    landPointMgr.vlfLandmarkPointSet_Update_NewTZ(sn.User.OrganizationId, sn.Landmark.LandmarkName,
                        txtLandmarkName.Text,
                     Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text),
                     txtLandmarkDesc.Text,
                     this.txtContactName.Text,
                     this.txtPhone.Text,
                     radius,
                     this.txtEmail.Text,
                     this.txtPhoneSMS.Text,
                     Convert.ToSingle(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress,
                     pointSets, landmarkPublic, Convert.ToInt64(this.ddlCategory.SelectedValue)
                     );

                    //update AuditLogs table with update
                    result = _dbu.RecordUserAction("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + _landmarkid.ToString(), "Edit", this.Context.Request.UserHostAddress, "Geozone_landmark/frmLandmark.aspx", "Landmark " + txtLandmarkName.Text + " Edited By: " + sn.LoginUserID);                    
                }
                catch (Exception Ex)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateLandmarkFailedError");
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    return;
                }
                //if (objUtil.ErrCheck(dbo.UpdateLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Landmark.LandmarkName, this.txtLandmarkName.Text, Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text), txtLandmarkDesc.Text, this.txtContactName.Text, this.txtPhone.Text, Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"])), this.txtEmail.Text, this.txtPhoneSMS.Text, Convert.ToInt16(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress), false))
                //    if (objUtil.ErrCheck(dbo.UpdateLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Landmark.LandmarkName, this.txtLandmarkName.Text, Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text), txtLandmarkDesc.Text, this.txtContactName.Text, this.txtPhone.Text, Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"])), this.txtEmail.Text, this.txtPhoneSMS.Text, Convert.ToInt16(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress), true))
                //    {
                //        this.lblMessage.Visible = true;
                //        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateLandmarkFailedError");
                //        return;
                //    }
            }

            if (sn.User.UserGroupId != 36 || sn.User.OrganizationId != 952)
                DgLandmarks_Fill_NewTZ();

            VLF.PATCH.Logic.PatchLandmark _lankmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);

            if (enableAssignment && trAssignment.Visible)
            {
                int fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);
                if (fleetId != -1)
                {
                    //this.txtLandmarkName.Text
                    //VLF.PATCH.Logic.PatchLandmark _lankmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
                    _lankmark.AssignLandmarkToFleetByLandmarkName(sn.User.OrganizationId, this.txtLandmarkName.Text, fleetId);
                }

            }

            int landmarkId = _lankmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, this.txtLandmarkName.Text);
            if (enableTimer)
            {
                int serviceConfigId = Convert.ToInt32(Request[cboServices.ClientID]);


                if (landmarkId > 0)
                {
                    _ps.DeleteHardcodedCallTimerServices(sn.User.OrganizationId, landmarkId);

                    if (serviceConfigId > 0)
                        _ps.AssignServiceToLandmark(sn.User.OrganizationId, landmarkId, serviceConfigId, "4160129305@e.pagenet.ca;mnancharla@bsmwireless.com;", "[VehicleDescription] Stop Exception: [ServiceName], occurred at [EVENT_TIME] Location: [LANDMARK_NAME]. [GOOGLE_LINK]");
                }
            }

            this.dgAddress.DataSource = null;
            this.dgAddress.DataBind();
            this.lblMessage.Visible = true;

            if (sn.Landmark.EditLandmarkMode)
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateLandmarkSuccess");
            else
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AddLandmarkSuccess");

            this.tblLandmarkAdd.Visible = false;
            this.tblLandmarks.Visible = true;
            sn.Landmark.AddLandmarkMode = false;
            sn.Landmark.EditLandmarkMode = false;
            this.tblLandmarkAdd.Visible = false;
        }

        // Changes for TimeZone Feature end

        private void LandmarkSaveTelogis()
        {
            bool landmarkPublic = false;

            Page.Validate();
            if (!Page.IsValid)
                return;

            this.lblAddMessage.Text = "";

            dbo = new ServerDBOrganization.DBOrganization();


            lblMessage.Visible = true;
            string strAddress = this.txtStreet.Text + "," + this.txtCity.Text + "," + this.txtState.Text + ",," + this.cboCountry.SelectedItem.Value ;
            string resolvedAddress = "";
            clsMap mp = new clsMap();
            if ((this.lstAddOptions.SelectedItem.Value == "0") && (this.txtX.Text == "0"))
            {
                double X = 0; double Y = 0;

                mp.ResolveCooridnatesByAddressTelogis(strAddress, ref X, ref Y, ref resolvedAddress);

                this.txtX.Text = X.ToString();
                this.txtY.Text = Y.ToString();

                if ((this.txtX.Text == "0") && (this.txtY.Text == "0"))
                {
                    this.lblAddMessage.Visible = true;
                    this.lblAddMessage.Text = (string)base.GetLocalResourceObject("lblAddMessage_Text_InvalidAddressError");
                    return;
                }
                if ((this.txtX.Text != "0") && (this.txtY.Text != "0"))
                {

                    if (!CheckIfNumberic(this.txtX.Text))
                    {
                        this.lblAddMessage.Visible = true;
                        this.lblAddMessage.Text = (string)base.GetLocalResourceObject("lblAddMessage_Text_InvalidLongitudeError");
                        return;
                    }

                    if (!CheckIfNumberic(this.txtY.Text))
                    {
                        this.lblAddMessage.Visible = true;
                        this.lblAddMessage.Text = (string)base.GetLocalResourceObject("lblAddMessage_Text_InvalidLatitudeError");
                        return;
                    }
                }
            }
            else if ((this.lstAddOptions.SelectedItem.Value == "1") && (this.txtX.Text != "0"))
            {
                resolvedAddress = mp.ResolveStreetAddressTelogis(this.txtX.Text,this.txtY.Text);
            }

            this.lblAddMessage.Text = "";
            if (resolvedAddress.Trim()!="")
                strAddress = resolvedAddress;

            bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight.Checked)); 
            landmarkPublic = this.optLandmarkPublicPrivate.SelectedValue=="1" ? true: false;

            if (!sn.Landmark.EditLandmarkMode)
            {
                //if (ErrCheck(dbo.AddOrganizationLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text), txtLandmarkName.Text, txtLandmarkDesc.Text, this.txtContactName.Text, this.txtPhone.Text, Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"])), this.txtEmail.Text, this.txtPhoneSMS.Text, Convert.ToInt16(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress), false))
                try
                {
                    Int32 radius = -1;
                    string pointSets = string.Empty;
                    if (LandmarkOptions.SelectedValue == "0")
                    {
                        if (Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text)) < 0)
                        {
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("valRangeRadiusResource1.ErrorMessage");
                            return;
                        }
                        radius = Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"]));
                    }

                    if (LandmarkOptions.SelectedValue == "1" || LandmarkOptions.SelectedValue == "2")
                    {
                        if (sn.Landmark.DsLandmarkDetails  != null)
                        {
                           foreach(DataRow dr in sn.Landmark.DsLandmarkDetails.Rows)
                           {
                               try
                               {
                                   string pointLatitude = dr["Latitude"].ToString();
                                   string pointLongitude = dr["Longitude"].ToString();
                                   if (pointSets != string.Empty)
                                   {
                                       pointSets = pointSets + "," + pointLatitude + "|" + pointLongitude;
                                   }
                                   else pointSets =  pointLatitude + "|" + pointLongitude;
                               }
                               catch (Exception ex)
                               { 
                               }
                           }
                        }
                        if (pointSets == string.Empty)
                        {
                            this.lblMessage.Text = drawPolygonText + " " + (string)base.GetLocalResourceObject("lblMessage_Text_AddLandmarkFailed");
                            return;
                        }
                    }      

                    landPointMgr.vlfLandmarkPointSet_Add(sn.User.OrganizationId, txtLandmarkName.Text,
                     Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text),
                     txtLandmarkDesc.Text,
                     this.txtContactName.Text,
                     this.txtPhone.Text,
                     radius,
                     this.txtEmail.Text,
                     this.txtPhoneSMS.Text,
                     Convert.ToInt16(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress,
                     pointSets, landmarkPublic, Convert.ToInt64(this.ddlCategory.SelectedValue)
                     );

                    VLF.PATCH.Logic.PatchLandmark _landmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
                    int _landmarkid = _landmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, txtLandmarkName.Text);
                    ServerDBUser.DBUser _dbu = new ServerDBUser.DBUser();

                    if (_landmarkid > 0)
                    {
                        _landmark.UpdateLandmarkCreater(_landmarkid, sn.UserID);
                    }
                    int result = _dbu.RecordUserAction("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + _landmarkid.ToString(), "Add", this.Context.Request.UserHostAddress, "Geozone_landmark/frmLandmark.aspx", "Landmark " + txtLandmarkName.Text + " Added By: " + sn.LoginUserID);                         
                }
                catch (Exception Ex)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AddLandmarkFailed");
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    return;
                }

                    //if (objUtil.ErrCheck(dbo.AddOrganizationLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text), txtLandmarkName.Text, txtLandmarkDesc.Text, this.txtContactName.Text, this.txtPhone.Text, Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"])), this.txtEmail.Text, this.txtPhoneSMS.Text, Convert.ToInt16(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress), true))
                    //{
                    //}
            }
            else
            {

                try
                {
                    
                    Int32 radius = -1;
                    string pointSets = string.Empty;
                    if (LandmarkOptions.SelectedValue == "0")
                        radius = Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"]));
                    if (LandmarkOptions.SelectedValue == "1" || LandmarkOptions.SelectedValue == "2")
                    {
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
                            this.lblMessage.Text = drawPolygonText + " " + (string)base.GetLocalResourceObject("lblMessage_Text_UpdateLandmarkFailedError");
                        }
                    }
                    
                    landPointMgr.vlfLandmarkPointSet_Update(sn.User.OrganizationId, sn.Landmark.LandmarkName,
                        txtLandmarkName.Text,
                     Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text),
                     txtLandmarkDesc.Text,
                     this.txtContactName.Text,
                     this.txtPhone.Text,
                     radius,
                     this.txtEmail.Text,
                     this.txtPhoneSMS.Text,
                     Convert.ToInt16(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress,
                     pointSets, landmarkPublic, Convert.ToInt64(this.ddlCategory.SelectedValue)
                     );

                    VLF.PATCH.Logic.PatchLandmark _landmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
                    int _landmarkid = _landmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, txtLandmarkName.Text);
                    ServerDBUser.DBUser _dbu = new ServerDBUser.DBUser();
                    int result = _dbu.RecordUserAction("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + _landmarkid.ToString(), "Edit", this.Context.Request.UserHostAddress, "Geozone_landmark/frmLandmark.aspx", "Landmark " + txtLandmarkName.Text + " Edited By: " + sn.LoginUserID);                    
                }
                catch (Exception Ex)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateLandmarkFailedError") ;
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    return;
                }
                //if (objUtil.ErrCheck(dbo.UpdateLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Landmark.LandmarkName, this.txtLandmarkName.Text, Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text), txtLandmarkDesc.Text, this.txtContactName.Text, this.txtPhone.Text, Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"])), this.txtEmail.Text, this.txtPhoneSMS.Text, Convert.ToInt16(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress), false))
                //    if (objUtil.ErrCheck(dbo.UpdateLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Landmark.LandmarkName, this.txtLandmarkName.Text, Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text), txtLandmarkDesc.Text, this.txtContactName.Text, this.txtPhone.Text, Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"])), this.txtEmail.Text, this.txtPhoneSMS.Text, Convert.ToInt16(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress), true))
                //    {
                //        this.lblMessage.Visible = true;
                //        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateLandmarkFailedError");
                //        return;
                //    }
            }

            if (sn.User.UserGroupId != 36 || sn.User.OrganizationId != 952)
                DgLandmarks_Fill_NewTZ();

            VLF.PATCH.Logic.PatchLandmark _lankmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);

            if (enableAssignment && trAssignment.Visible)
            {
                int fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);
                if (fleetId != -1)
                {
                    //this.txtLandmarkName.Text
                    //VLF.PATCH.Logic.PatchLandmark _lankmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
                    _lankmark.AssignLandmarkToFleetByLandmarkName(sn.User.OrganizationId, this.txtLandmarkName.Text, fleetId);
                }

            }

            int landmarkId = _lankmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, this.txtLandmarkName.Text);
            if (enableTimer)
            {
                int serviceConfigId = Convert.ToInt32(Request[cboServices.ClientID]);
                
                
                if (landmarkId > 0)
                {
                    _ps.DeleteHardcodedCallTimerServices(sn.User.OrganizationId, landmarkId);

                    if (serviceConfigId > 0)
                        _ps.AssignServiceToLandmark(sn.User.OrganizationId, landmarkId, serviceConfigId, "4160129305@e.pagenet.ca;mnancharla@bsmwireless.com;", "[VehicleDescription] Stop Exception: [ServiceName], occurred at [EVENT_TIME] Location: [LANDMARK_NAME]. [GOOGLE_LINK]");
                }
            }

            this.dgAddress.DataSource = null;
            this.dgAddress.DataBind();
            this.lblMessage.Visible = true;

            if (sn.Landmark.EditLandmarkMode)
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateLandmarkSuccess");
            else
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AddLandmarkSuccess");

            this.tblLandmarkAdd.Visible = false;
            this.tblLandmarks.Visible = true;
            sn.Landmark.AddLandmarkMode = false;
            sn.Landmark.EditLandmarkMode = false;
            this.tblLandmarkAdd.Visible = false;
        }

        //private void LandmarkSaveGeoMicro()
        //{
        //    bool landmarkPublic = false;

        //    Page.Validate();
        //    if (!Page.IsValid)
        //        return;

        //    this.lblAddMessage.Text = "";

        //    dbo = new ServerDBOrganization.DBOrganization();
        //    lblMessage.Visible = true;

        //    if ((this.lstAddOptions.SelectedItem.Value == "0") && (this.txtX.Text == "0"))
        //    {

        //        //Get Coordinates by street address
        //        string xml = "";
        //        string strAddress = "";
        //        DataSet ds = new DataSet();
        //        //strAddress = this.txtStreet.Text + "|" + this.txtCity.Text + "|" + this.txtState.Text + "|" + this.cboCountry.SelectedItem.Value;

        //        strAddress = this.txtStreet.Text + "|" + this.txtCity.Text + ", " + this.txtState.Text + ", " + this.cboCountry.SelectedItem.Value;

        //        // create ClientMapProxy only for geocoding
        //        VLF.MAP.ClientMapProxy geoMap = new VLF.MAP.ClientMapProxy(sn.User.GeoCodeEngine);
        //        if (geoMap == null)
        //        {
        //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        //            return;
        //        }
        //        xml = geoMap.GetAddressMatches(strAddress);

        //        ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
        //        if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.StreetAddress), (short)geoMap.LastUsedGeoCodeID), false))
        //            if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.StreetAddress), (short)geoMap.LastUsedGeoCodeID), true))
        //            {
        //                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Update user map usage info failed."));
        //            }

        //        if (xml != "")
        //        {
        //            ds = new DataSet();
        //            ds.ReadXml(new StringReader(xml));
        //        }

        //        if ((xml == "") || (ds.Tables.Count == 0))
        //        {
        //            this.lblAddMessage.Visible = true;
        //            this.lblAddMessage.Text = (string)base.GetLocalResourceObject("lblAddMessage_Text_AddressNotFoundError");
        //            this.dgAddress.DataSource = null;
        //            this.dgAddress.DataBind();
        //            return;
        //        }
        //        else
        //        {
        //            this.lblAddMessage.Visible = true;
        //            this.lblAddMessage.Text = (string)base.GetLocalResourceObject("lblAddMessage_Text_SelectAddressAndSave");
        //        }

        //        this.dgAddress.DataSource = ds;
        //        this.dgAddress.DataBind();
        //    }


        //    if ((this.txtX.Text != "0") && (this.txtY.Text != "0"))
        //    {

        //        if (!CheckIfNumberic(this.txtX.Text))
        //        {
        //            this.lblAddMessage.Visible = true;
        //            this.lblAddMessage.Text = (string)base.GetLocalResourceObject("lblAddMessage_Text_InvalidLongitudeError");
        //            return;
        //        }

        //        if (!CheckIfNumberic(this.txtY.Text))
        //        {
        //            this.lblAddMessage.Visible = true;
        //            this.lblAddMessage.Text = (string)base.GetLocalResourceObject("lblAddMessage_Text_InvalidLatitudeError");
        //            return;
        //        }

        //        // create ClientMapProxy only for geocoding
        //        VLF.MAP.ClientMapProxy geoCode = new VLF.MAP.ClientMapProxy(sn.User.GeoCodeEngine);
        //        if (geoCode == null)
        //        {
        //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        //            return;
        //        }
        //        string xml = geoCode.GetStreetAddressXML(new GeoPoint(Convert.ToDouble(this.txtY.Text), Convert.ToDouble(this.txtX.Text)));


        //        ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
        //        if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.StreetAddress), (short)geoCode.LastUsedGeoCodeID), false))
        //            if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.StreetAddress), (short)geoCode.LastUsedGeoCodeID), true))
        //            {
        //                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Update user map usage info failed."));
        //            }

        //        StringReader strrXML = new StringReader(xml);
        //        DataSet ds = new DataSet();
        //        string strAddress = "";
        //        if (xml != "")
        //        {
        //            ds.ReadXml(strrXML);
        //            this.txtCity.Text = ds.Tables[0].Rows[0]["City"].ToString().TrimEnd();
        //            cboCountry.SelectedIndex = cboCountry.Items.IndexOf(cboCountry.Items.FindByValue(ds.Tables[0].Rows[0]["Country"].ToString().TrimEnd()));
        //            this.txtStreet.Text = ds.Tables[0].Rows[0]["Street_Address"].ToString().TrimEnd();
        //            this.txtState.Text = ds.Tables[0].Rows[0]["State"].ToString().TrimEnd();
        //            strAddress = txtStreet.Text + "," + txtCity.Text + "," + txtState.Text;

        //        }
        //        else if (xml == "")
        //        {
        //            //this.lblAddMessage.Visible = true;
        //            //this.lblAddMessage.Text = (string)base.GetLocalResourceObject("lblAddMessage_Text_InvalidAddressError");
        //            strAddress = "";
        //        }


        //        // string strAddress = ViewState["Address"].ToString() ;

        //        this.lblAddMessage.Text = "";

        //        bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight.Checked));
        //        landmarkPublic = this.optLandmarkPublicPrivate.SelectedValue == "1" ? true : false;
  
        //        if (!sn.Landmark.EditLandmarkMode)
        //        {
        //            if (objUtil.ErrCheck(dbo.AddOrganizationLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text), txtLandmarkName.Text, txtLandmarkDesc.Text, this.txtContactName.Text, this.txtPhone.Text, Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"])), this.txtEmail.Text, this.txtPhoneSMS.Text, Convert.ToInt16(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress, landmarkPublic), false))
        //                if (objUtil.ErrCheck(dbo.AddOrganizationLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text), txtLandmarkName.Text, txtLandmarkDesc.Text, this.txtContactName.Text, this.txtPhone.Text, Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"])), this.txtEmail.Text, this.txtPhoneSMS.Text, Convert.ToInt16(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress, landmarkPublic), true))
        //                {
        //                    this.lblMessage.Visible = true;
        //                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AddLandmarkFailed");
        //                    return;
        //                }
        //        }
        //        else
        //        {
        //            if (objUtil.ErrCheck(dbo.UpdateLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Landmark.LandmarkName, this.txtLandmarkName.Text, Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text), txtLandmarkDesc.Text, this.txtContactName.Text, this.txtPhone.Text, Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"])), this.txtEmail.Text, this.txtPhoneSMS.Text, Convert.ToInt16(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress, landmarkPublic), false))
        //                if (objUtil.ErrCheck(dbo.UpdateLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Landmark.LandmarkName, this.txtLandmarkName.Text, Convert.ToDouble(txtY.Text), Convert.ToDouble(txtX.Text), txtLandmarkDesc.Text, this.txtContactName.Text, this.txtPhone.Text, Convert.ToInt32(Convert.ToInt32(this.txtRadius.Text) / Convert.ToDouble(ViewState["UnitValue"])), this.txtEmail.Text, this.txtPhoneSMS.Text, Convert.ToInt16(this.cboTimeZone.SelectedItem.Value), DayLightSaving, chkDayLight.Checked, strAddress, landmarkPublic), true))
        //                {
        //                    this.lblMessage.Visible = true;
        //                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateLandmarkFailedError");
        //                    return;
        //                }
        //        }

        //        if (sn.User.UserGroupId != 36 || sn.User.OrganizationId != 952)
        //            DgLandmarks_Fill();

        //        this.dgAddress.DataSource = null;
        //        this.dgAddress.DataBind();
        //        this.lblMessage.Visible = true;

        //        if (sn.Landmark.EditLandmarkMode)
        //            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateLandmarkSuccess");
        //        else
        //            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AddLandmarkSuccess");

        //        this.tblLandmarkAdd.Visible = false;
        //        this.tblLandmarks.Visible = true;
        //        sn.Landmark.AddLandmarkMode = false;
        //        sn.Landmark.EditLandmarkMode = false;
        //        this.tblLandmarkAdd.Visible = false;
        //    }
        //}


        protected void lstAddOptions_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ViewByOptions();
        }

        private void cmdEmails_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmEmails.aspx");
        }

        private void cmdPreference_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmPreference.aspx");
        }
        // Changes for TimeZone Feature start

        private void DgLandmarks_Fill_NewTZ()
        {
            DataRow[] drCollections = null;
            StringReader strrXML = null;
            DataSet dsLandmarks = new DataSet();
            DataSet _landmarks = null;
            int i = 0;

            string xml = "";

            try
            {
                dbo = new ServerDBOrganization.DBOrganization();

                //if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                //    if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                //    {
                //        return;
                //    }

                string sConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                using (VLF.PATCH.Logic.PatchLandmark pog = new VLF.PATCH.Logic.PatchLandmark(sConnectionString))
                {
                    _landmarks = pog.PatchGetLandmarksInfoByOrganizationIdUserId_NewTZ(sn.User.OrganizationId, sn.UserID);

                    if (VLF.CLS.Util.IsDataSetValid(_landmarks))
                    {
                        xml = clsXmlUtil.GetXmlIncludingNull(_landmarks);
                    }
                }

                if (xml == "")
                {
                    this.dgLandmarks.DataSource = null;
                    this.dgLandmarks.DataBind();
                    return;
                }

                strrXML = new StringReader(xml);
                dsLandmarks.ReadXml(strrXML);

                // Current Status
                DataColumn dc = new DataColumn("LandmarkId", Type.GetType("System.Int32"));
                dc.DefaultValue = 0;

                if (sn.User.ControlEnable(sn, 92))
                {
                    DataTable dt = dsLandmarks.Tables[0].Clone();

                    string filter = String.Format("Public=false AND CreateUserID={0}", sn.UserID.ToString());
                    drCollections = dsLandmarks.Tables[0].Select(filter);

                    if (drCollections != null && drCollections.Length > 0)
                    {
                        foreach (DataRow dr in drCollections)
                        {
                            dt.ImportRow(dr);
                        }
                    }

                    dt.Columns.Add(dc);

                    foreach (DataRow rowItem in dt.Rows)
                    {
                        i++;
                        rowItem["LandmarkId"] = i;
                    }

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    sn.DsLandMarks = ds;
                }
                else
                {
                    sn.DsLandMarks = dsLandmarks;
                    sn.DsLandMarks.Tables[0].Columns.Add(dc);

                    foreach (DataRow rowItem in dsLandmarks.Tables[0].Rows)
                    {
                        i++;
                        rowItem["LandmarkId"] = i;
                    }
                }

                this.dgLandmarks.DataSource = sn.DsLandMarks;
                this.dgLandmarks.DataBind();
                sn.Misc.ConfDtLandmarks = sn.DsLandMarks.Tables[0];
                sn.Landmark.DsLandmarkPointDetails = landPointMgr.GetLandmarkPointSetByOrganizationId(sn.User.OrganizationId).Tables[0];
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

        // Changes for TimeZone Feature end

        private void DgLandmarks_Fill()
        {
            DataRow[] drCollections = null;
            StringReader strrXML = null;
            DataSet dsLandmarks = new DataSet();
            DataSet _landmarks = null;
            int i = 0;

            string xml = "";

            try
            {
                dbo = new ServerDBOrganization.DBOrganization();

                //if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                //    if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                //    {
                //        return;
                //    }
                
                string sConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                using (VLF.PATCH.Logic.PatchLandmark pog = new VLF.PATCH.Logic.PatchLandmark(sConnectionString))
                {
                    _landmarks = pog.PatchGetLandmarksInfoByOrganizationIdUserId(sn.User.OrganizationId, sn.UserID);

                    if (VLF.CLS.Util.IsDataSetValid(_landmarks))
                    {
                        xml = clsXmlUtil.GetXmlIncludingNull(_landmarks);
                    }
                }

                if (xml == "")
                {
                    this.dgLandmarks.DataSource = null;
                    this.dgLandmarks.DataBind();
                    return;
                }

                strrXML = new StringReader(xml);
                dsLandmarks.ReadXml(strrXML);

                // Current Status
                DataColumn dc = new DataColumn("LandmarkId", Type.GetType("System.Int32"));
                dc.DefaultValue = 0;

                if (sn.User.ControlEnable(sn, 92))
                {
                    DataTable dt = dsLandmarks.Tables[0].Clone();

                    string filter = String.Format("Public=false AND CreateUserID={0}", sn.UserID.ToString());
                    drCollections = dsLandmarks.Tables[0].Select(filter);

                    if (drCollections != null && drCollections.Length > 0)
                    {
                        foreach (DataRow dr in drCollections)
                        {
                            dt.ImportRow(dr);
                        }
                    }

                    dt.Columns.Add(dc);

                    foreach (DataRow rowItem in dt.Rows)
                    {
                        i++;
                        rowItem["LandmarkId"] = i;
                    }

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    sn.DsLandMarks = ds;   
                }
                else
                {
                    sn.DsLandMarks = dsLandmarks;
                    sn.DsLandMarks.Tables[0].Columns.Add(dc);

                    foreach (DataRow rowItem in dsLandmarks.Tables[0].Rows)
                    {
                        i++;
                        rowItem["LandmarkId"] = i;
                    }
                }

                this.dgLandmarks.DataSource = sn.DsLandMarks;
                this.dgLandmarks.DataBind();
                sn.Misc.ConfDtLandmarks = sn.DsLandMarks.Tables[0];
                sn.Landmark.DsLandmarkPointDetails = landPointMgr.GetLandmarkPointSetByOrganizationId(sn.User.OrganizationId).Tables[0];
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

        protected void cmdLandMarkAdd_Click(object sender, System.EventArgs e)
        {
            if (enableAssignment)
            {
                trAssignmentPopup.Visible = false;
                trAssignment.Visible = true;
            }
            this.txtCity.Text = "";
            this.txtStreet.Text = "";
            this.txtState.Text = "";
            this.txtContactName.Text = "";
            this.txtLandmarkDesc.Text = "";
            this.txtLandmarkName.Text = "";
            this.lblAddMessage.Text = "";
            this.txtPhone.Text = "";
            this.txtRadius.Text = "0";
            this.txtX.Text = "0";
            sn.GeoZone.ImgConfPath = "";
            this.txtY.Text = "0";
            this.dgAddress.DataSource = null;
            this.dgAddress.DataBind();
            this.tblLandmarkAdd.Visible = true;
            this.lblMessage.Text = "";
            this.lblMessage.Visible = false;
            this.tblLandmarks.Visible = false;
            sn.Landmark.AddLandmarkMode = true;
            sn.Landmark.EditLandmarkMode = false;
            sn.Landmark.X = 0;
            sn.Landmark.Y = 0;
            sn.Landmark.LandmarkName = "";
            sn.Landmark.LandmarkType = VLF.CLS.Def.Enums.GeozoneType.None;
            sn.Landmark.LandmarkDescription = "";
            ViewState["LandmarkId"] = "";
            cboTimeZone_Fill();
            ddlCategory_Fill();
            this.cboTimeZone.SelectedIndex = -1;
            sn.Map.LandmarksMapVE = "";
            sn.Map.GeozonesMapVE = "";
            sn.Map.EditMapVE = "false";
            dgLandmarkCoordinates.Visible = false;
            dgLandmarkCoordinates.DataSource = null;
            dgLandmarkCoordinates.DataBind();
            if (sn.Landmark.DsLandmarkDetails != null)
                sn.Landmark.DsLandmarkDetails.Rows.Clear();
            LandmarkOptions.SelectedValue = "0";
            txtRadius.Enabled = true;
            sn.Landmark.LandmarkType = VLF.CLS.Def.Enums.GeozoneType.None;
            if(enableTimer)
                cboServices.SelectedValue = "-1";


            DisableOptSelection();
            DisableOrLockControls(true);
        }

        private void DisableOptSelection()
        {
            if (sn.User.ControlEnable(sn, 92))
            {
                this.optLandmarkPublicPrivate.SelectedValue = "0";
                this.optLandmarkPublicPrivate.Visible = false;
            }
            //foreach (DataRow rowItem in sn.User.DsGUIControls.Tables[0].Rows)
            //{
            //    if (Convert.ToInt32(rowItem["ControlId"]) == 92) //Disable public landmark option 
            //    {
            //        this.optLandmarkPublicPrivate.SelectedValue = "0";
            //        this.optLandmarkPublicPrivate.Enabled = false;
            //        break;
            //    }
            //}
        }

        private void DisableOrLockControls(bool status)
        {
            //this.lstAddOptions.Enabled = status;
            this.txtLandmarkName.Enabled = status;
            this.txtContactName.Enabled = status;
            this.txtLandmarkDesc.Enabled = status;
            this.txtPhone.Enabled = status;
            this.txtRadius.Enabled = status;
            this.txtStreet.Enabled = status;
            this.txtCity.Enabled = status;
            this.txtState.Enabled = status;
            this.cboCountry.Enabled = status;
            this.txtY.Enabled = status;
            this.txtX.Enabled = status;
            this.cboServices.Enabled = status;
            this.cboFleet.Enabled = status;
            this.LandmarkOptions.Enabled = status;
            this.txtEmail.Enabled = status;
            this.txtPhoneSMS.Enabled = status;
            this.cboTimeZone.Enabled = status;
            this.chkDayLight.Enabled = status;
            this.cmdSaveLandmark.Enabled = status;
            LandmarkMap = "../MapNew/frmDrawLandmarkGeozone.aspx?FormName=Landmark&ShowControl=" + isLandmarkUpdatableByUser.ToString();

            if (!status)
            {
                lblAddMessage.Visible = true;
                lblAddMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_LandmarkReadOnlyErrorSecurity");
            }
            else
            {
                lblAddMessage.Visible = false;
            }
        }

        protected void dgLandmarks_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            int result = 0;

            try
            {
                if (sn.User.UserGroupId == 28 && sn.UserID != 11967)
                    return; 

                lblMessage.Visible = true;
                if (confirm == "")
                    return;

                dbo = new ServerDBOrganization.DBOrganization();
                dbo.IsLandmarkUpdatableByUser(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt64(((Label)dgLandmarks.Items[e.Item.ItemIndex].FindControl("lid")).Text), ref isLandmarkUpdatableByUser);

                if (!isLandmarkUpdatableByUser)
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_DeleteLandmarkFailedErrorSecurity");
                    return;
                }

                string xml = "";
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();
                if (objUtil.ErrCheck(dbu.GetAssignedGroupsByUser(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetAssignedGroupsByUser(sn.UserID, sn.SecId, ref xml), true))
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_DeleteLandmarkAuthFailed");
                        return;
                    }


                if (xml == "")
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_DeleteLandmarkAuthFailed");
                    return;
                }
                else
                {
                    if (System.Xml.Linq.XDocument.Parse(xml).Root.Elements("UserAllGroups").Count() == 1)
                    {
                        if (System.Xml.Linq.XDocument.Parse(xml).Root.Element("UserAllGroups").Element("UserGroupName").Value.ToString() == "View Only")
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AddLandmarkViewOnly");
                            return;
                        }
                    }
                }

                //dbo = new ServerDBOrganization.DBOrganization();

                ServerDBUser.DBUser _dbu = new ServerDBUser.DBUser();
                VLF.PATCH.Logic.PatchLandmark _landmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
                int _landmarkid = _landmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, dgLandmarks.DataKeys[e.Item.ItemIndex].ToString());
                try
                {
                    GeomarkServiceClient _lc = new GeomarkServiceClient("httpbasic");
                    bool rsd = _lc.DeleteFromSpatialTable(_landmarkid);

                    if (rsd)
                    {
                        //check if landmark audit log already exists in the AuditLogs table
                        result = _dbu.RecordInitialValues("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + _landmarkid.ToString(), "Edit", this.Context.Request.UserHostAddress, "Geozone_landmark/frmLandmark.aspx", "Landmark " + txtLandmarkName.Text + " Edited By: " + sn.LoginUserID);
                        
                        landPointMgr.vlfLandmarkPointSet_Delete(sn.User.OrganizationId, dgLandmarks.DataKeys[e.Item.ItemIndex].ToString());

                        //update AuditLogs table with delete
                        result = _dbu.RecordUserAction("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + _landmarkid.ToString(), "Delete", this.Context.Request.UserHostAddress, "MapNew/NewMapGeozoneLandmark.asmx", "Landmark " + dgLandmarks.DataKeys[e.Item.ItemIndex].ToString() + "; Postgis deleted " + rsd.ToString() + "; Deleted By: " + sn.LoginUserID);
                    }
                    else
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = deleteFail;
                    }

                    

                    if (!rsd)
                    {
                        ViewState["ConfirmDelete"] = "0";
                        confirm = "";
                        return;
                    }
                    
                }
                catch (Exception Ex)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = deleteFail; ;
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));

                    return;
                }


                //if (objUtil.ErrCheck(dbo.DeleteOrganizationLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, dgLandmarks.DataKeys[e.Item.ItemIndex].ToString()), false))
                //    if (objUtil.ErrCheck(dbo.DeleteOrganizationLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId, dgLandmarks.DataKeys[e.Item.ItemIndex].ToString()), true))
                //    {
                //        return;
                //    }

                dgLandmarks.SelectedIndex = -1;
                lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_DeleteLandmarkSuccess");
                this.dgLandmarks.CurrentPageIndex = 0;
                if (sn.User.UserGroupId != 36 || sn.User.OrganizationId != 952)
                    DgLandmarks_Fill_NewTZ();
                ViewState["ConfirmDelete"] = "0";
                confirm = "";
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

        protected void dgAddress_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.txtX.Text = dgAddress.SelectedItem.Cells[2].Text;
            this.txtY.Text = dgAddress.SelectedItem.Cells[1].Text;
            ViewState["Address"] = dgAddress.SelectedItem.Cells[0].Text;
        }

        protected void cmdCancelLandmark_Click(object sender, System.EventArgs e)
        {
            this.tblLandmarkAdd.Visible = false;
            this.tblLandmarks.Visible = true;
            sn.Landmark.AddLandmarkMode = false;
            sn.Landmark.EditLandmarkMode = false;
        }

        private void dgLandmarks_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            dgLandmarks.CurrentPageIndex = e.NewPageIndex;
            dgLandmarks.DataSource = sn.Misc.ConfDtLandmarks;
            dgLandmarks.DataBind();
            dgLandmarks.SelectedIndex = -1;
            this.lblAddMessage.Text = "";
            this.lblAddMessage.Visible = false;
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

        private void lnkShowLabel_Click(object sender, System.EventArgs e)
        {
            Response.Write("<SCRIPT Language='javascript'>ShowMap();</SCRIPT>");
        }

        protected void dgLandmarks_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            string strCountry = String.Empty;

            try
            {

                if (sn.User.UserGroupId == 28 && sn.UserID != 11967)
                    return; 

                if (e.CommandName == "cmdEdit")
                {
                    trAssignment.Visible = false;
                    sn.GeoZone.ImgConfPath = "";

                    if (enableAssignment)
                        trAssignmentPopup.Visible = true;

                    foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
                    {
                        if (dgLandmarks.DataKeys[e.Item.ItemIndex].ToString().TrimEnd() == dr["LandmarkName"].ToString().TrimEnd())
                        {

                            //// create ClientMapProxy only for geocoding
                            //VLF.MAP.ClientMapProxy geoMap = new VLF.MAP.ClientMapProxy(sn.User.GeoCodeEngine);
                            //if (geoMap == null)
                            //{
                            //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                            //    return;
                            //}
                            //string xml = geoMap.GetStreetAddressXML(new GeoPoint(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"])));



                            //ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
                            //if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.StreetAddress), (short)geoMap.LastUsedGeoCodeID), false))
                            //    if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.StreetAddress), (short)geoMap.LastUsedGeoCodeID), true))
                            //    {
                            //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Update user map usage info failed."));
                            //    }

                            //StringReader strrXML = new StringReader(xml);
                            //DataSet ds = new DataSet();
                            //string strCountry = "";
                            //if (xml != "")
                            //{
                            //    ds.ReadXml(strrXML);
                            //    strCountry = ds.Tables[0].Rows[0]["Country"].ToString().TrimEnd();

                            //    if (strCountry.IndexOf("geoCountry") == 0)
                            //        strCountry = strCountry.Substring(10);

                            //    this.txtCity.Text = ds.Tables[0].Rows[0]["City"].ToString().TrimEnd();

                            //    cboCountry.SelectedIndex = cboCountry.Items.IndexOf(cboCountry.Items.FindByValue(strCountry));
                            //    this.txtStreet.Text = ds.Tables[0].Rows[0]["Street_Address"].ToString().TrimEnd();
                            //    this.txtState.Text = ds.Tables[0].Rows[0]["State"].ToString().TrimEnd();
                            //}
                            //else
                            //{
                            //    this.txtCity.Text = "";
                            //    this.txtStreet.Text = "";
                            //    this.txtState.Text = "";
                            //}

                            clsMap mp = new clsMap();

                            if (dr["StreetAddress"].ToString().TrimEnd() != "")
                            {
                                try
                                {
                                    string[] address = dr["StreetAddress"].ToString().TrimEnd().Split(',');

                                    if (address.Length == 1)
                                    {
                                        this.txtStreet.Text = address[0];
                                        this.txtCity.Text = String.Empty;
                                        this.txtState.Text = String.Empty;
                                        cboCountry.SelectedIndex = 0;
                                    }
                                    else if (address.Length == 3)
                                    {
                                        this.txtCity.Text = address[1];
                                        this.txtState.Text = address[2];
                                        strCountry = address[4];
                                        cboCountry.SelectedIndex = cboCountry.Items.IndexOf(cboCountry.Items.FindByValue(strCountry));
                                    }
                                    else if (address.Length == 4)
                                    {
                                        this.txtStreet.Text = address[0];
                                        this.txtCity.Text = address[1];
                                        this.txtState.Text = address[2];
                                        strCountry = address[4];
                                        cboCountry.SelectedIndex = cboCountry.Items.IndexOf(cboCountry.Items.FindByValue(strCountry));
                                    }
                                    else if (address.Length == 5)
                                    {
                                        this.txtStreet.Text = address[0];
                                        this.txtCity.Text = address[1];
                                        this.txtState.Text = address[2];
                                        strCountry = address[4].Trim();
                                        cboCountry.SelectedIndex = cboCountry.Items.IndexOf(cboCountry.Items.FindByValue(strCountry));
                                    }
                                    else if (address.Length == 6)
                                    {
                                        this.txtStreet.Text = address[0];
                                        this.txtCity.Text = address[1];
                                        this.txtState.Text = address[3];
                                        strCountry = address[5].Trim();
                                        cboCountry.SelectedIndex = cboCountry.Items.IndexOf(cboCountry.Items.FindByValue(strCountry));
                                    }
                                    else
                                    {
                                        this.txtStreet.Text = String.Empty;
                                        this.txtCity.Text = String.Empty;
                                        this.txtState.Text = String.Empty;
                                        cboCountry.SelectedIndex = 0;
                                    }
                                }
                                catch
                                {
                                }

                            }
                            else
                            {
                                this.txtStreet.Text = String.Empty;
                                this.txtCity.Text = String.Empty;
                                this.txtState.Text = String.Empty;
                                cboCountry.SelectedIndex = 0;
                            }

                            sn.Landmark.LandmarkName = dr["LandmarkName"].ToString().TrimEnd();
                            sn.Landmark.EditLandmarkMode = true;
                            sn.Landmark.AddLandmarkMode = false;
                            ViewState["LandmarkId"] = dr["LandmarkId"].ToString().TrimEnd();
                            VLF.PATCH.Logic.PatchLandmark _landmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
                            LandmarkId = _landmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, sn.Landmark.LandmarkName);

                            this.lblAddMessage.Text = "";
                            this.lblMessage.Text = "";

                            this.txtContactName.Text = dr["ContactPersonName"].ToString().TrimEnd();
                            this.txtLandmarkDesc.Text = dr["Description"].ToString().TrimEnd();
                            this.txtLandmarkName.Text = dr["LandmarkName"].ToString().TrimEnd();
                            this.lblAddMessage.Text = "";
                            this.txtPhone.Text = dr["ContactPhoneNum"].ToString().TrimEnd();
                            this.txtX.Text = dr["Longitude"].ToString().TrimEnd();
                            this.txtY.Text = dr["Latitude"].ToString().TrimEnd();

                            ddlCategory_Fill();
                            this.ddlCategory.SelectedValue = dr["CategoryId"].ToString();
                            //setDDLCurrentLandmarkCategoryValue(LandmarkId);

                            sn.Landmark.X = Convert.ToDouble(dr["Longitude"]);
                            sn.Landmark.Y = Convert.ToDouble(dr["Latitude"]);
                            sn.Landmark.Radius = Convert.ToInt32(dr["Radius"]);
                           
                            if (sn.Landmark.Radius >= 0)
                            {
                                LandmarkOptions.SelectedIndex = 0;
                                txtRadius.Enabled = true;
                                dgLandmarkCoordinates.Visible = false;
                                sn.Landmark.LandmarkType = VLF.CLS.Def.Enums.GeozoneType.None;
                            }
                            else
                            {
                                txtRadius.Enabled = false;
                                //string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                                VLF.DAS.Logic.LandmarkPointSetManager pointSetMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);
                                DataSet ds = pointSetMgr.GetLandmarkPointSetByLandmarkName(sn.Landmark.LandmarkName, sn.User.OrganizationId);
                                dgLandmarkCoordinates.DataSource = ds;
                                dgLandmarkCoordinates.DataBind();
                                sn.Landmark.DsLandmarkDetails = ds.Tables[0];
                                dgLandmarkCoordinates.Visible = true;
                                LandmarkOptions.SelectedIndex = 2;
                                sn.Landmark.LandmarkType = VLF.CLS.Def.Enums.GeozoneType.Polygon;
                                if (ds.Tables[0].Rows.Count == 4)
                                {
                                    if (ds.Tables[0].Rows[0]["Latitude"] == ds.Tables[0].Rows[1]["Latitude"] &&
                                        ds.Tables[0].Rows[1]["Longitude"] == ds.Tables[0].Rows[2]["Longitude"] &&
                                        ds.Tables[0].Rows[2]["Latitude"] == ds.Tables[0].Rows[3]["Latitude"] &&
                                        ds.Tables[0].Rows[3]["Longitude"] == ds.Tables[0].Rows[0]["Longitude"]
                                        )
                                    {
                                        LandmarkOptions.SelectedIndex = 1;
                                        sn.Landmark.LandmarkType = VLF.CLS.Def.Enums.GeozoneType.Rectangle;
                                    }

                                }
                                //VLF.DAS.DB.Landmark lannmarkDB = new VLF.DAS.DB.Landmark()
                                // .
                                //DataSet landmarkPoints = 
                            }

                            cboTimeZone_Fill();
                            this.cboTimeZone.SelectedIndex = -1;
                            for (int i = 0; i < cboTimeZone.Items.Count; i++)
                            {
                                //if (Convert.ToInt16(cboTimeZone.Items[i].Value.TrimEnd()) == Convert.ToInt32(dr["TimeZone"].ToString().TrimEnd()))
                                if (Convert.ToSingle(cboTimeZone.Items[i].Value.TrimEnd()) == Convert.ToSingle(dr["TimeZone"].ToString().TrimEnd()))
                                {
                                    cboTimeZone.SelectedIndex = i;
                                    break;
                                }
                            }

                            this.txtEmail.Text = dr["Email"].ToString().TrimEnd();
                            this.txtPhoneSMS.Text = dr["Phone"].ToString().TrimEnd();
                            this.chkDayLight.Checked = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());


                            this.tblLandmarkAdd.Visible = true;
                            this.tblLandmarks.Visible = false;
                            this.txtRadius.Text = (Convert.ToInt32(Convert.ToInt32(dr["Radius"].ToString()) * Convert.ToDecimal(ViewState["UnitValue"]))).ToString();
                            

                            if (sn.Landmark.LandmarkByAddress)
                                this.tblShowMap.Visible = false;
                            else
                                this.tblShowMap.Visible = true;

                            setDefaultCallTimer();

                            DisableOptSelection();
                            if (sn.Landmark.EditLandmarkMode)
                            {
                                int oLandmarkId = _landmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, txtLandmarkName.Text);
                                if (oLandmarkId > 0)
                                {
                                    dbo = new ServerDBOrganization.DBOrganization();
                                    dbo.IsLandmarkUpdatableByUser(sn.UserID, sn.SecId, sn.User.OrganizationId, oLandmarkId, ref isLandmarkUpdatableByUser);
                                }
                                DisableOrLockControls(isLandmarkUpdatableByUser);
                            }
                            if (Convert.ToBoolean(dr["Public"].ToString())) 
                                    this.optLandmarkPublicPrivate.SelectedValue="1";
                            else
                                    this.optLandmarkPublicPrivate.SelectedValue="0";

                            return;
                        }
                    }
                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
        }

        private void setDefaultCallTimer()
        {
            VLF.PATCH.Logic.PatchLandmark _lankmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
            int landmarkId = _lankmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, this.txtLandmarkName.Text);
            int serviceId = _ps.GetHardcodedTimerServiceId(sn.User.OrganizationId, landmarkId);
            if (serviceId > 0)
            {
                cboServices.SelectedValue = serviceId.ToString();
            }
			else
			{
				cboServices.SelectedValue = "-1";
			}
        }

        protected void txtStreet_TextChanged(object sender, System.EventArgs e)
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

        private void dgLandmarks_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                if (e.Item.Cells[0].Text.TrimEnd().Length > 16)
                {
                    e.Item.Cells[0].ToolTip = e.Item.Cells[0].Text.TrimEnd();
                    e.Item.Cells[0].Text = e.Item.Cells[0].Text.TrimEnd().Substring(0, 17) + "...";
                }
                else
                {
                    e.Item.Cells[0].ToolTip = e.Item.Cells[0].Text.TrimEnd();
                }

                if (e.Item.Cells[1].Text.TrimEnd().Length > 22)
                {
                    e.Item.Cells[1].ToolTip = e.Item.Cells[1].Text.TrimEnd();
                    e.Item.Cells[1].Text = e.Item.Cells[1].Text.TrimEnd().Substring(0, 19) + "...";
                }
                else
                {
                    e.Item.Cells[1].ToolTip = e.Item.Cells[1].Text.TrimEnd();
                }


                if (e.Item.Cells[4].Text.TrimEnd().Length > 40)
                {
                    e.Item.Cells[4].ToolTip = e.Item.Cells[4].Text.TrimEnd();
                    e.Item.Cells[4].Text = e.Item.Cells[4].Text.TrimEnd().Substring(0, 37) + "...";
                }
                else
                {
                    e.Item.Cells[4].ToolTip = e.Item.Cells[4].Text.TrimEnd();
                }


                if (e.Item.Cells[5].Text.TrimEnd().Length > 20)
                {
                    e.Item.Cells[5].ToolTip = e.Item.Cells[5].Text.TrimEnd();
                    e.Item.Cells[5].Text = e.Item.Cells[5].Text.TrimEnd().Substring(0, 17) + "...";
                }
                else
                {
                    e.Item.Cells[5].ToolTip = e.Item.Cells[5].Text.TrimEnd();
                }


                if (e.Item.Cells[6].Text.TrimEnd().Length > 20)
                {
                    e.Item.Cells[6].ToolTip = e.Item.Cells[6].Text.TrimEnd();
                    e.Item.Cells[6].Text = e.Item.Cells[6].Text.TrimEnd().Substring(0, 17) + "...";
                }
                else
                {
                    e.Item.Cells[6].ToolTip = e.Item.Cells[6].Text.TrimEnd();
                }

                if (e.Item.Cells[7].Text.TrimEnd().Length > 20)
                {
                    e.Item.Cells[7].ToolTip = e.Item.Cells[7].Text.TrimEnd();
                    e.Item.Cells[7].Text = e.Item.Cells[7].Text.TrimEnd().Substring(0, 17) + "...";
                }
                else
                {
                    e.Item.Cells[7].ToolTip = e.Item.Cells[7].Text.TrimEnd();
                }


                e.Item.Cells[9].ToolTip = (string)base.GetLocalResourceObject("dgLandmarks_Item_Edit");
                e.Item.Cells[10].ToolTip = (string)base.GetLocalResourceObject("dgLandmarks_Item_Delete");

            }

            if (e.Item.ItemType == ListItemType.Header)
            {
                e.Item.Cells[8].Text = (string)base.GetLocalResourceObject("dgLandmarks_Item_Radius") + " (" + ViewState["UnitName"] + ")";
            }
        }

        private void FindExistingPreference()
        {
            if (sn.User.UnitOfMes == 1)
            {
                ViewState["UnitName"] = "m";
                ViewState["UnitValue"] = "1";
            }
            else if (sn.User.UnitOfMes > 0.6213 && sn.User.UnitOfMes <= 0.6214)
            {
                ViewState["UnitName"] = "yrd";
                ViewState["UnitValue"] = "1.094";
            }
        }

        private void cboTimeZone_Fill()
        {
            try
            {
                cboTimeZone.SelectedIndex = -1;
                cboTimeZone.DataSource = null;

                DataTable tblTimeZone = new DataTable();
                tblTimeZone.Columns.Add("TimeZoneId", typeof(float));
                tblTimeZone.Columns.Add("TimeZoneName", typeof(string));

                object[] objRow;
                for (int i = -12; i < 14; i++)
                {

                    objRow = new object[2];
                    objRow[0] = i;
                    if (i != 0)
                    {
                        if (i < 0)
                        {
                            objRow[1] = "GMT" + i.ToString();
                        }
                        else
                        {
                            objRow[1] = "GMT+" + i.ToString();
                        }
                    }
                    else
                    {
                        objRow[1] = "GMT";
                    }

                    tblTimeZone.Rows.Add(objRow);
                    this.cboTimeZone.DataSource = tblTimeZone;
                    this.cboTimeZone.DataBind();
                }

                //  Changes For TimeZone Feature start
                //  Adding New Time Zone GMT - 3:30 (-3.5)  | NewFoundLand

                objRow = new object[2];
                DataRow dtRow = tblTimeZone.NewRow();
                dtRow[0] = -3.5;
                dtRow[1] = "GMT-3:30";
                tblTimeZone.Rows.InsertAt(dtRow, 9);
                this.cboTimeZone.DataSource = tblTimeZone;
                this.cboTimeZone.DataBind();
                //  Changes For TimeZone Feature end
                cboTimeZone.SelectedIndex = cboTimeZone.Items.IndexOf(cboTimeZone.Items.FindByText(string.Format("GMT{0}", sn.User.TimeZone)));
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        protected void dgLandmarks_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (sn.User.UserGroupId != 28 || sn.UserID == 11967)
	   {		
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                confirm = "return confirm('" + (string)base.GetLocalResourceObject("dgLandmarks_DeleteConfirmation") + "')";
                LinkButton deleteBtn = (LinkButton)e.Item.Cells[10].Controls[0];
                deleteBtn.Attributes.Add("onclick", confirm);
            }
	   }	
        }
        protected void cmdClear_Click(object sender, EventArgs e)
        {
            this.txtSearchParam.Text = "";
            dgLandmarks.SelectedIndex = -1;
            dgLandmarks.CurrentPageIndex = sn.Misc.LandmarkSelectedGridPage;
            this.dgLandmarks.DataSource = sn.DsLandMarks;
            this.dgLandmarks.DataBind();
        }
        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow[] drCollections = null;
                DataTable dt = sn.DsLandMarks.Tables[0].Clone();
                sn.Misc.LandmarkSelectedGridPage = dgLandmarks.CurrentPageIndex;
                dgLandmarks.CurrentPageIndex = 0;
                string filter = "";
                switch (cboSearchType.SelectedItem.Value)
                {
                    case "0":
                        if (!String.IsNullOrEmpty(this.txtSearchParam.Text.Trim()))
                            filter = String.Format("LandmarkName like '%{0}%'", this.txtSearchParam.Text.Replace("'", "''"));
                        break;
                    case "1":
                        if (!String.IsNullOrEmpty(this.txtSearchParam.Text.Trim()))
                            filter = String.Format("Description like '%{0}%'", this.txtSearchParam.Text);
                        break;
                    case "2":
                        if (!String.IsNullOrEmpty(this.txtSearchParam.Text.Trim()))
                            filter = String.Format("StreetAddress like '%{0}%'", this.txtSearchParam.Text);
                        break;
                }

                drCollections = sn.DsLandMarks.Tables[0].Select(filter);
                if (drCollections != null && drCollections.Length > 0)
                {
                    foreach (DataRow dr in drCollections)
                        dt.ImportRow(dr);
                }

                this.dgLandmarks.DataSource = dt;
                this.dgLandmarks.DataBind();

                sn.Misc.ConfDtLandmarks = dt;
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
        
        protected void ValidateEmail(Object sender, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            string strRegex = @"^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$";
            Regex re = new Regex(strRegex);
            if (txtEmail.Text.Trim() != string.Empty)
            {
                string[] strs = txtEmail.Text.Trim().Split(';');
                foreach (String myStr in strs)
                {
                    if (!re.IsMatch(myStr.Trim()))
                        args.IsValid = false;
                }
            }
        }

        protected void dgLandmarks_ItemDataBound1(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.DataItem is DataRowView)
            {
                Label lblRadius = (Label)e.Item.FindControl("lblRadius");
                if (lblRadius != null && ((DataRowView)e.Item.DataItem)["Radius"] != DBNull.Value)
                {
                    Int32 radius = Convert.ToInt32(((DataRowView)e.Item.DataItem)["Radius"].ToString());
                    lblRadius.Text = (Convert.ToInt32(radius * Convert.ToDecimal(ViewState["UnitValue"]))).ToString();
                }
            }
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

        private void SetHidAddress()
        {
            if (txtStreet.Text.Trim() != string.Empty &&
                txtCity.Text.Trim() != string.Empty &&
                txtState.Text.Trim() != string.Empty)
            {
                hidAddress.Value =
                this.txtStreet.Text + "," + this.txtCity.Text + "," + this.txtState.Text + ",," + this.cboCountry.SelectedItem.Value;
            }
            else hidAddress.Value = "";
        }

        private void CboFleet_Fill()
        {
            try
            {
                if (sn.User.OrganizationId == 952 && sn.User.UserGroupId == 36) //NCC user hard-coded NCC fleet only.
                {
                    cboFleet.Items.Insert(0, new ListItem("NCC", "16945"));
                }
                else
                {
                    DataSet dsFleets = new DataSet();
                    dsFleets = sn.User.GetUserFleets(sn);
                    cboFleet.DataSource = dsFleets;
                    cboFleet.DataBind();

                    cboFleet.Items.Insert(0, new ListItem((string)this.GetLocalResourceObject("cboFleet_Item_0"), "-1"));

                    foreach (ListItem li in cboFleet.Items)
                    {
                        if (li.Text == "All Vehicles" || li.Text == "Tous les véhicules" || li.Text == "Tous les vehicules")
                        {
                            cboFleet.Items.Remove(li);
                            break;
                        }

                    }
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

        private void CboServices_Fill()
        {
            try
            {
                DataSet dsServices = new DataSet();
                dsServices = _ps.GetHardcodedCallTimerServices();
                cboServices.DataSource = dsServices;
                cboServices.DataBind();
                
                //cboFleet.Items.Insert(0, new ListItem((string)this.GetLocalResourceObject("cboFleet_Item_0"), "-1"));
                cboServices.Items.Insert(0, new ListItem("Select a service", "-1"));     
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

        private void ddlCategory_Fill()
        {
            try
            {
                VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(this.sConnectionString);
                DataSet dsOrganization = org.ListOrganizationLandmarkCategory(sn.UserID, sn.User.OrganizationId);
                DataTable dsLandmarkCategory = dsOrganization.Tables["LandmarkCategory"];

                ListItem oneItem = null;
                this.ddlCategory.Items.Clear();
                this.ddlCategory.Items.Add(new ListItem((string)base.GetLocalResourceObject("ddlCategory_Text_Prompt_Select_Category"), "0"));
                foreach (DataRow oneRow in dsLandmarkCategory.Rows)
                {
                    oneItem = new ListItem(oneRow["MetadataValue"].ToString(), oneRow["DomainMetadataId"].ToString());
                    this.ddlCategory.Items.Add(oneItem);
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

        private void setDDLCurrentLandmarkCategoryValue(long landmarkId)
        {
            try
            {
                VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(this.sConnectionString);
                DataSet dsOrganization = org.GetLandmarkCategory(sn.UserID, sn.User.OrganizationId, landmarkId);
                DataTable dsLandmarkCategory = dsOrganization.Tables["LandmarkCategory"];
                string landmarkCategoryMappingId = "0";
                string landmarkCategoryId = "0";
                if (dsLandmarkCategory.Rows.Count > 0)
                {
                    DataRow oneRow = dsLandmarkCategory.Rows[0];
                    landmarkCategoryMappingId = oneRow["LandmarkCategoryMappingId"].ToString();
                    landmarkCategoryId = oneRow["LandmarkCategoryId"].ToString();
                }

                this.ddlCategory.SelectedValue = landmarkCategoryId;
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

        private void saveLandmarkCategory(long landmarkId, long categoryId)
        {
            try
            {
                VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(this.sConnectionString);
                int affectedRecords = org.SaveLandmarkCategory(sn.UserID, sn.User.OrganizationId, landmarkId, categoryId);
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

        private void deleteLandmarkCategory(long landmarkId)
        {
            try
            {
                VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(this.sConnectionString);
                int affectedRecords = org.DeleteLandmarkCategory(sn.UserID, sn.User.OrganizationId, landmarkId);
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
    }
}
