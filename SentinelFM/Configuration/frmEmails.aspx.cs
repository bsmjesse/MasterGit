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
namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmEmails.
    /// </summary>
    public partial class frmEmails : SentinelFMBasePage
    {
        protected ServerDBFleet.DBFleet dbf;
        string confirm;
        public bool IsHydroQuebec = false;
        protected DataSet dsTimeZone = new DataSet();
        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string OrganizationHierarchyPath = "";
        public bool MutipleUserHierarchyAssignment = false;
        private VLF.PATCH.Logic.PatchOrganizationHierarchy poh;
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                #region Hide Drive Message Forward Column
                const string DRIVER_MESSAGE_FORWARD_KEY = "18";
                GetOrganizationPreferences(DRIVER_MESSAGE_FORWARD_KEY);
                if (!sn.User.DriverMessageForward)
                {
                    dgEmails.Columns[8].Visible = false;
                }
                #endregion
                if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956)
                    IsHydroQuebec = true;
                else
                    IsHydroQuebec = false;
                MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
                bool ShowOrganizationHierarchy = false;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                    ShowOrganizationHierarchy = true;
                else
                {
                    ShowOrganizationHierarchy = false;
                }
                if (ShowOrganizationHierarchy)
                {
                    trFleetSelectOption.Visible = true;
                    string defaultnodecode = string.Empty;
                    ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();
                    string xml = "";
                    if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), false))
                        if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                        {
                        }
                    StringReader strrXML = new StringReader(xml);
                    DataSet dsPref = new DataSet();
                    dsPref.ReadXml(strrXML);
                    foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                    {
                        if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode))
                        {
                            defaultnodecode = rowItem["PreferenceValue"].ToString().TrimEnd();
                        }
                    }
                    defaultnodecode = defaultnodecode ?? string.Empty;
                    if (defaultnodecode == string.Empty)
                    {
                        if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 480)
                            defaultnodecode = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID);
                        else
                            defaultnodecode = poh.GetRootNodeCode(sn.User.OrganizationId);
                    }
                    if (!IsPostBack)
                    {
                        DefaultOrganizationHierarchyNodeCode = defaultnodecode;
                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                        hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;
                        hidOrganizationHierarchyFleetId.Value = DefaultOrganizationHierarchyFleetId.ToString();
                        if (MutipleUserHierarchyAssignment)
                        {
                            hidOrganizationHierarchyFleetId.Value = sn.User.PreferFleetIds;
                            hidOrganizationHierarchyNodeCode.Value = sn.User.PreferNodeCodes;
                            //DefaultOrganizationHierarchyFleetId = sn.User.PreferFleetIds;
                        }
                    }
                    else
                    {
                        hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());
                        btnOrganizationHierarchyNodeCode.Text = hidOrganizationHierarchyNodeCode.Value.ToString();
                        DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();
                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                    }
                    if (MutipleUserHierarchyAssignment)
                    {
                        if (hidOrganizationHierarchyFleetId.Value.Trim() == string.Empty)
                            DefaultOrganizationHierarchyFleetName = "";
                        else if (hidOrganizationHierarchyFleetId.Value.Contains(","))
                            DefaultOrganizationHierarchyFleetName = GetLocalResourceObject("ResMultipleHierarchies").ToString();
                        else
                            DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(hidOrganizationHierarchyFleetId.Value));
                    }
                    OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);
                    //btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == "" ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                    btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName;
                    hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                    //if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyFleetId.Value.Contains(","))
                    //    btnOrganizationHierarchyNodeCode.Text = GetLocalResourceObject("ResMultipleHierarchies").ToString(); // "Multiple Hierarchies";
                }
                else
                {
                    trFleetSelectOption.Visible = false;
                }
                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref Form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    this.tblEmailAdd.Visible = false;
                    ViewState["ConfirmDelete"] = "0";
                    CboFleet_Fill();
                    DsTimeZone_Fill();
                    this.cboTimeZoneAdd.DataSource = dsTimeZone;
                    this.cboTimeZoneAdd.DataBind();
                    if (sn.User.DefaultFleet != -1)
                    {
                        cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                        DgEmails_Fill();
                    }
                    if (sn.User.SmsSupport)
                        txtPhone.Enabled = true;
                    if (IsHydroQuebec || sn.User.OrganizationId == 952)
                    {
                        cmdDriverSkill.Visible = false;
                    }
					/*
                    if ((sn.User.OrganizationId == 480 || sn.User.OrganizationId == 999956) && (sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2))
                    {
                        cmdGarminDevices.Visible = true;
                    }
					*/
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
        /// <summary>
        /// HL, Developed for Driver Message Forward BRD
        /// Get organization Preference by preferenceId 
        /// </summary>
        /// <param name="preferenceId"></param>
        private void GetOrganizationPreferences(string preferenceId)
        {
            DataSet ds = new DataSet();
            string xml = "";
            using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
            {
                if (
                objUtil.ErrCheck(
                dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml),
                false))
                    if (
                    objUtil.ErrCheck(
                      dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml),
                      true))
                    {
                        return;
                    }
            }
            if (xml == "")
            {
                return;
            }
            StringReader strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);
            sn.User.DriverMessageForward = false;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr["OrgPreferenceId"].ToString() == preferenceId) //Driver message Forward
                {
                    if (dr["PreferenceValue"].ToString() == "1")
                    {
                        sn.User.DriverMessageForward = true;
                    }
                    else
                    {
                        sn.User.DriverMessageForward = false;
                    }
                    break;
                }
            }
        }

        private void DgEmails_Fill()
        {
            try
            {
                StringReader strrXML = null;
                DataSet dsEmails = new DataSet();
                string xml = "";
                dbf = new ServerDBFleet.DBFleet();
                int fleetid = -1;
                if (optAssignBased.SelectedItem.Value == "0")
                {
                    if (this.cboFleet.SelectedItem != null)
                        fleetid = Convert.ToInt32(this.cboFleet.SelectedItem.Value);
                }
                else
                {
                    if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyFleetId.Value.Contains(","))
                    {
                        this.dgEmails.DataSource = null;
                        this.dgEmails.DataBind();
                        return;
                    }
                    fleetid = Convert.ToInt32(hidOrganizationHierarchyFleetId.Value);
                }
                if (fleetid == -1)
                {
                    this.dgEmails.DataSource = null;
                    this.dgEmails.DataBind();
                }
                if (objUtil.ErrCheck(dbf.GetFleetEmailsXML(sn.UserID, sn.SecId, fleetid, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetFleetEmailsXML(sn.UserID, sn.SecId, fleetid, ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    this.dgEmails.DataSource = null;
                    this.dgEmails.DataBind();
                    return;
                }
                strrXML = new StringReader(xml);
                dsEmails.ReadXml(strrXML);
                // Show TimeZoneName
                DataColumn dc = new DataColumn();
                dc.ColumnName = "TimeZoneName";
                dc.DataType = Type.GetType("System.String");
                dc.DefaultValue = "";
                dsEmails.Tables[0].Columns.Add(dc);
                foreach (DataRow rowItem in dsEmails.Tables[0].Rows)
                {
                    if (Convert.ToInt32(rowItem["TimeZone"]) != 0)
                    {
                        if (Convert.ToInt32(rowItem["TimeZone"]) < 0)
                            rowItem["TimeZoneName"] = "GMT" + rowItem["TimeZone"].ToString();
                        else
                            rowItem["TimeZoneName"] = "GMT+" + rowItem["TimeZone"].ToString();
                    }
                    else
                        rowItem["TimeZoneName"] = "GMT";
                }
                this.dgEmails.DataSource = dsEmails;
                this.dgEmails.DataBind();
                this.dgEmails.Visible = true;
                this.cmdAddEmail.Visible = true;
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
            this.dgEmails.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgEmails_PageIndexChanged);
            this.dgEmails.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgEmails_CancelCommand);
            this.dgEmails.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgEmails_EditCommand);
            this.dgEmails.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgEmails_UpdateCommand);
            this.dgEmails.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgEmails_DeleteCommand);
            this.dgEmails.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgEmails_ItemDataBound);
        }
        #endregion
        protected void dgEmails_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {
                //Check security
                bool cmdDelete = sn.User.ControlEnable(sn, 26);
                if (!cmdDelete)
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = (string)base.GetLocalResourceObject("NoPermissions");
                    return;
                }
                if (confirm == "")
                    return;
                lblMessage.Visible = true;
                dbf = new ServerDBFleet.DBFleet();
                int fleetid = -1;
                if (optAssignBased.SelectedItem.Value == "0")
                    fleetid = Convert.ToInt32(this.cboFleet.SelectedItem.Value);
                else
                    fleetid = Convert.ToInt32(hidOrganizationHierarchyFleetId.Value);
                //	int rowsAffected = 0 ;
                if (objUtil.ErrCheck(dbf.DeleteEmailFromFleet(sn.UserID, sn.SecId, fleetid, dgEmails.DataKeys[e.Item.ItemIndex].ToString()), false))
                    if (objUtil.ErrCheck(dbf.DeleteEmailFromFleet(sn.UserID, sn.SecId, fleetid, dgEmails.DataKeys[e.Item.ItemIndex].ToString()), true))
                    {
                        return;
                    }
                //	if (rowsAffected==1)
                //	{
                dgEmails.SelectedIndex = -1;
                lblMessage.Text = (string)base.GetLocalResourceObject("EmailDeleted");
                dgEmails.CurrentPageIndex = 0;
                DgEmails_Fill();
                ViewState["ConfirmDelete"] = "0";
                confirm = "";
                //	}
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
        protected void dgEmails_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            dgEmails.EditItemIndex = -1;
            DgEmails_Fill();
            this.cmdAddEmail.Enabled = true;
        }
        protected void dgEmails_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            //Check security 
            bool cmd = sn.User.ControlEnable(sn, 27);
            if (!cmd)
            {
                lblMessage.Visible = true;
                lblMessage.Text = (string)base.GetLocalResourceObject("NoPermissions");
                return;
            }
            this.cmdAddEmail.Enabled = true;
            this.tblEmailAdd.Visible = false;
            this.lblMessage.Text = "";
            dgEmails.EditItemIndex = Convert.ToInt32(e.Item.ItemIndex);
            DgEmails_Fill();
            dgEmails.SelectedIndex = -1;
            this.cmdAddEmail.Enabled = false;
        }
        protected void dgEmails_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {
                TextBox txtNewEmail;
                TextBox txtPhone;
                DropDownList cboTimeZone;
                CheckBox chkDayLightSaving;
                CheckBox chkCritical;
                CheckBox chkNotify;
                CheckBox chkWarning;
                CheckBox chkMaintenance;
                CheckBox chkDriverMessage;
                CheckBox chkAutosubscription;
                CheckBox chkReminder;

                ViewState["ConfirmDelete"] = "0";
                string OldEmail = dgEmails.DataKeys[e.Item.ItemIndex].ToString();
                txtNewEmail = (TextBox)e.Item.FindControl("txtEmail");
                //txtPhone =  (TextBox)e.Item.FindControl("txtPhone");
                cboTimeZone = (DropDownList)e.Item.FindControl("cboTimeZone");
                chkDayLightSaving = (CheckBox)e.Item.FindControl("chkDaylight");
                chkCritical = (CheckBox)e.Item.FindControl("chkCritical");
                chkNotify = (CheckBox)e.Item.FindControl("chkNotify");
                chkWarning = (CheckBox)e.Item.FindControl("chkWarning");
                chkMaintenance = (CheckBox)e.Item.FindControl("chkMaintenance");
                chkDriverMessage = (CheckBox)e.Item.FindControl("chkDriverMessage");
                chkAutosubscription = (CheckBox)e.Item.FindControl("chkAutosubscription");
                chkReminder = (CheckBox)e.Item.FindControl("chkReminder");
                dbf = new ServerDBFleet.DBFleet();
                Int16 intDayLightSaving = objUtil.IsDayLightSaving(chkDayLightSaving.Checked);
                int fleetid = -1;
                if (optAssignBased.SelectedItem.Value == "0")
                    fleetid = Convert.ToInt32(this.cboFleet.SelectedItem.Value);
                else
                    fleetid = Convert.ToInt32(hidOrganizationHierarchyFleetId.Value);
                if (sn.User.DriverMessageForward)
                {
                    //  Do NOT Check This in
                    //  --------------------------------------------------------
                    if (objUtil.ErrCheck(dbf.ModifyEmail(sn.UserID, sn.SecId, fleetid, OldEmail, txtNewEmail.Text, "", Convert.ToInt16(cboTimeZone.SelectedItem.Value), intDayLightSaving, 0, chkNotify.Checked, chkWarning.Checked, chkCritical.Checked, chkDayLightSaving.Checked, chkMaintenance.Checked, chkDriverMessage.Checked, chkAutosubscription.Checked, chkReminder.Checked), false))
                        if (objUtil.ErrCheck(dbf.ModifyEmail(sn.UserID, sn.SecId, fleetid, OldEmail, txtNewEmail.Text, "", Convert.ToInt16(cboTimeZone.SelectedItem.Value), intDayLightSaving, 0, chkNotify.Checked, chkWarning.Checked, chkCritical.Checked, chkDayLightSaving.Checked, chkMaintenance.Checked, chkDriverMessage.Checked, chkAutosubscription.Checked, chkReminder.Checked), true))
                        {
                            this.cmdAddEmail.Enabled = true;
                            return;
                        }

                    //  Do NOT Check This in
                    //  --------------------------------------------------------
                }
                else
                {
                    if (objUtil.ErrCheck(dbf.UpdateEmail(sn.UserID, sn.SecId, fleetid, OldEmail, txtNewEmail.Text, "", Convert.ToInt16(cboTimeZone.SelectedItem.Value), intDayLightSaving, 0, chkNotify.Checked, chkWarning.Checked, chkCritical.Checked, chkDayLightSaving.Checked, chkMaintenance.Checked, chkAutosubscription.Checked, chkReminder.Checked), false))
                        if (objUtil.ErrCheck(dbf.UpdateEmail(sn.UserID, sn.SecId, fleetid, OldEmail, txtNewEmail.Text, "", Convert.ToInt16(cboTimeZone.SelectedItem.Value), intDayLightSaving, 0, chkNotify.Checked, chkWarning.Checked, chkCritical.Checked, chkDayLightSaving.Checked, chkMaintenance.Checked, chkAutosubscription.Checked, chkReminder.Checked), true))
                        {
                            this.cmdAddEmail.Enabled = true;
                            return;
                        }
                }
                this.cmdAddEmail.Enabled = true;
                lblMessage.Visible = true;
                dgEmails.EditItemIndex = -1;
                DgEmails_Fill();
                lblMessage.Text = (string)base.GetLocalResourceObject("EmailUpdated");
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
        protected void dgEmails_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            dgEmails.CurrentPageIndex = e.NewPageIndex;
            DgEmails_Fill();
            dgEmails.SelectedIndex = -1;
        }
        protected void cmdAddEmail_Click(object sender, System.EventArgs e)
        {
            this.lblMessage.Text = "";
            this.txtNewEmail.Text = "";
            this.txtPhone.Text = "";
            this.cmdAddEmail.Enabled = false;
            this.tblEmailAdd.Visible = true;
            if (!sn.User.DriverMessageForward)
            {
                //release: chkDriverMessage.Visible = false;
                chkDriverMessage.Attributes.Add("onclick", "javascript: return false;");
                chkDriverMessage.ForeColor = System.Drawing.Color.Gray;
            }
            else
            {
                chkDriverMessage.ForeColor = new System.Drawing.Color();
            }
        }
        protected void cmdSaveEmail_Click(object sender, System.EventArgs e)
        {
            try
            {
                Page.Validate();
                if (!Page.IsValid)
                    return;
                int fleetid = -1;
                if (optAssignBased.SelectedItem.Value == "0")
                    fleetid = Convert.ToInt32(this.cboFleet.SelectedItem.Value);
                else
                    fleetid = Convert.ToInt32(hidOrganizationHierarchyFleetId.Value);
                if (fleetid == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");
                    return;
                }
                this.lblMessage.Text = "";
                dbf = new ServerDBFleet.DBFleet();
                Int16 intDayLightSaving = objUtil.IsDayLightSaving(chkAddDayLight.Checked);

                //  Do NOT Check This in
                //  --------------------------------------------------------

                if (objUtil.ErrCheck(dbf.SaveEmail(sn.UserID, sn.SecId, fleetid, this.txtNewEmail.Text, this.txtPhone.Text, Convert.ToInt16(this.cboTimeZoneAdd.SelectedItem.Value), intDayLightSaving, 0, chkAddNotify.Checked, chkAddWarning.Checked, chkAddCritical.Checked, chkAddDayLight.Checked, chkMaintenance.Checked, chkDriverMessage.Checked, chkAddAutoSubscription.Checked, chkAddReminder.Checked), false))
                    if (objUtil.ErrCheck(dbf.SaveEmail(sn.UserID, sn.SecId, fleetid, this.txtNewEmail.Text, this.txtPhone.Text, Convert.ToInt16(this.cboTimeZoneAdd.SelectedItem.Value), intDayLightSaving, 0, chkAddNotify.Checked, chkAddWarning.Checked, chkAddCritical.Checked, chkAddDayLight.Checked, chkMaintenance.Checked, chkDriverMessage.Checked, chkAddAutoSubscription.Checked, chkAddReminder.Checked), true))
                    {
                        this.cmdAddEmail.Enabled = true;
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("ErrEmailExist");
                        return;
                    }

                //  Do NOT Check This in
                //  --------------------------------------------------------

                dgEmails.SelectedIndex = -1;
                dgEmails.CurrentPageIndex = 0;
                DgEmails_Fill();
                this.txtNewEmail.Text = "";
                this.txtPhone.Text = "";
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("EmailSaved");
                this.cmdAddEmail.Enabled = true;
                this.tblEmailAdd.Visible = false;
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
        protected void cmdCancelAddEmal_Click(object sender, System.EventArgs e)
        {
            this.cmdAddEmail.Enabled = true;
            this.tblEmailAdd.Visible = false;
        }
        private void cmdLandmarkLink_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmLandmark.aspx");
        }
        protected void cmdVehicles_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmVehicleInfo.aspx");
        }
        private void cmdPreference_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmPreference.aspx");
        }
        protected void cmdDriverSkill_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmdriverskill.aspx");
        }
        protected void cmdGarminDevices_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmGarminDevices.aspx");
        }
        protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.cboFleet.SelectedItem.Value != "-1")
            {
                dgEmails.CurrentPageIndex = 0;
                DgEmails_Fill();
                ViewState["ConfirmDelete"] = "0";
                dgEmails.SelectedIndex = -1;
                this.lblMessage.Text = "";
            }
            else
            {
                this.dgEmails.DataSource = null;
                this.dgEmails.DataBind();
            }
        }
        protected void hidOrganizationHierarchyPostBack_Click(object sender, System.EventArgs e)
        {
            if (hidOrganizationHierarchyFleetId.Value != "-1")
            {
                dgEmails.CurrentPageIndex = 0;
                DgEmails_Fill();
                ViewState["ConfirmDelete"] = "0";
                dgEmails.SelectedIndex = -1;
                this.lblMessage.Text = "";
            }
            else
            {
                this.dgEmails.DataSource = null;
                this.dgEmails.DataBind();
            }
        }
        private void CboFleet_Fill()
        {
            try
            {
                DataSet dsFleets = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                dbf = new ServerDBFleet.DBFleet();
                if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        cboFleet.DataSource = null;
                        return;
                    }
                strrXML = new StringReader(xml);
                dsFleets.ReadXml(strrXML);
                DataView FleetView = dsFleets.Tables[0].DefaultView;
                FleetView.RowFilter = "FleetType<>'oh'";
                //cboFleet.DataSource=dsFleets  ;
                cboFleet.DataSource = FleetView;
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
        public int GetTimeZone(short TimeZone)
        {
            try
            {
                DropDownList cboTimeZone = new DropDownList();
                cboTimeZone.DataValueField = "TimeZoneId";
                cboTimeZone.DataTextField = "TimeZoneName";
                DsTimeZone_Fill();
                cboTimeZone.DataSource = dsTimeZone;
                cboTimeZone.DataBind();
                cboTimeZone.SelectedIndex = -1;
                cboTimeZone.Items.FindByValue(TimeZone.ToString()).Selected = true;
                return cboTimeZone.SelectedIndex;
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
                return 0;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                return 0;
            }
        }
        private void DsTimeZone_Fill()
        {
            try
            {
                DataTable tblTimeZone = dsTimeZone.Tables.Add("TimeZone");
                tblTimeZone.Columns.Add("TimeZoneId", typeof(short));
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
                    dsTimeZone.Tables[0].Rows.Add(objRow);
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
        private void dgEmails_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                e.Item.Cells[11].ToolTip = (string)base.GetLocalResourceObject("ToolTipEdit");
                e.Item.Cells[12].ToolTip = (string)base.GetLocalResourceObject("ToolTipDelete");
            }
        }
        protected void cmdFleetMng_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmFleets.aspx");
        }
        protected void cmdFleetVehicle_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmFleetVehicles.aspx");
        }
        protected void cmdFleetUsers_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmFleetUsers.aspx");
        }
        protected void cmdUsers_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmUsers.aspx");
        }
        protected void dgEmails_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                confirm = "return confirm('" + (string)base.GetLocalResourceObject("ConfirmDelete") + "')";
                LinkButton deleteBtn = (LinkButton)e.Item.Cells[12].Controls[0];
                deleteBtn.Attributes.Add("onclick", confirm);
            }
        }
        protected void cmdDriver_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmdrivers.aspx");
        }
        protected void cmdScheduledTasks_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmTaskScheduler.aspx");
        }
        protected void cmdOrganization_Click(object sender, EventArgs e)
        {
        }
        protected void optAssignBased_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (optAssignBased.SelectedItem.Value == "0")
            {
                trBasedOnNormalFleet.Visible = true;
                trBasedOnHierarchyFleet.Visible = false;
            }
            else
            {
                trBasedOnNormalFleet.Visible = false;
                trBasedOnHierarchyFleet.Visible = true;
            }
            DgEmails_Fill();
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            //GridExportTableData();
            DataTableExportExcel();
        }
        public void GridExportTableData()
        {
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=FleetEmailsExport.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.xls";
            StringWriter StringWriter = new System.IO.StringWriter();
            HtmlTextWriter HtmlTextWriter = new HtmlTextWriter(StringWriter);
            dgEmails.RenderControl(HtmlTextWriter);
            Response.Write(StringWriter.ToString());
            Response.End();
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
        }
        public void DataTableExportExcel()
        {
            try
            {
                StringReader strrXML = null;
                DataSet dsEmails = new DataSet();
                string xml = "";
                dbf = new ServerDBFleet.DBFleet();
                int fleetid = -1;
                if (optAssignBased.SelectedItem.Value == "0")
                {
                    if (this.cboFleet.SelectedItem != null)
                        fleetid = Convert.ToInt32(this.cboFleet.SelectedItem.Value);
                }
                else
                {
                    if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyFleetId.Value.Contains(","))
                    {
                        return;
                    }
                    fleetid = Convert.ToInt32(hidOrganizationHierarchyFleetId.Value);
                }
                if (objUtil.ErrCheck(dbf.GetFleetEmailsXML(sn.UserID, sn.SecId, fleetid, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetFleetEmailsXML(sn.UserID, sn.SecId, fleetid, ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    return;
                }
                strrXML = new StringReader(xml);
                dsEmails.ReadXml(strrXML);

                ExportDataTableList(dsEmails.Tables[0]);
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
        public void ExportDataTableList(DataTable datatable)
        {
            string fileNameMajorPart = "FleetEmailsReport-";
            string fileSuffix = DateTime.Now.ToShortDateString();
            string fileExtension = ".xls";
            string fileName = fileNameMajorPart + fileSuffix + fileExtension;
            string attach = "attachment;filename=" + fileName;
            Response.ClearContent();
            Response.AddHeader("content-disposition", attach);
            Response.ContentType = "application/ms-excel";
            if (datatable != null)
            {
                foreach (DataColumn dc in datatable.Columns)
                {
                    Response.Write(dc.ColumnName + "\t");
                }
                Response.Write(System.Environment.NewLine);
                foreach (DataRow dr in datatable.Rows)
                {
                    for (int i = 0; i < datatable.Columns.Count; i++)
                    {
                        Response.Write(dr[i].ToString() + "\t");
                    }
                    Response.Write("\n");
                }
                Response.End();
            }
        }
    }
}
