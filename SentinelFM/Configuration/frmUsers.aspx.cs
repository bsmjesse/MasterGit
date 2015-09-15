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
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmUsers.
    /// </summary>
    public partial class frmUsers : SentinelFMBasePage
    {
        public string Password
        {
            get
            {
                if (ViewState["Password"] != null)
                    return ViewState["Password"].ToString();
                else
                    return String.Empty;
            }
            set
            {
                ViewState["Password"] = value;
            }
        }

        public string ReenterPassword
        {
            get
            {
                if (ViewState["ReenterPassword"] != null)
                    return ViewState["ReenterPassword"].ToString();
                else
                    return String.Empty;
            }
            set
            {
                ViewState["ReenterPassword"] = value;
            }
        }

        public string PasswordStatus
        {
            get
            {
                if (ViewState["PasswordStatus"] != null)
                    return ViewState["PasswordStatus"].ToString();
                else
                    return String.Empty;
            }
            set
            {
                ViewState["PasswordStatus"] = value;
            }
        }

        public string msgPsw_Medium = "";
        public string msgPsw_Weak = "";
        public string msgPsw_MoreCharacters = "";
        public string msgPsw_Strong = "";
        public string msgPsw_TypePassword = "";
        public string confirmDeletePlan = "a";
        public string ExportToExcel = String.Empty;

        protected string sConnectionString;
        protected bool MultipleHierarchySetting = false;
        VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;
        string confirm;
        bool ChkbxViewDeletedUser = false;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);
                //Enable Search on pressing Enter Key
                txtSearchParam.Attributes.Add("onkeypress", "return clickButton(event,'" + cmdSearch.ClientID + "')");
                //Changes
                //cboStatusSearch.SelectedIndex = cboStatusSearch.Items.IndexOf(cboStatusSearch.Items.FindByValue("0"));
                if (chkbxViewDeletedUser.Checked == true)
                    ChkbxViewDeletedUser = true;

                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                {

                    if (ChkbxViewDeletedUser == false)
                    {

                        cboStatusSearch.Items.Cast<ListItem>().Where(i => i.Value == "Effacer").ToList().ForEach(i => cboStatusSearch.Items.Remove(i));
                    }
                    else
                    {
                        if (cboStatusSearch.Items.FindByText("Effacer") == null)
                        {
                            cboStatusSearch.Items.Insert(2, "Effacer");

                        }
                    }
                }
                else
                {
                    if (ChkbxViewDeletedUser == false)
                    {

                        cboStatusSearch.Items.Cast<ListItem>().Where(i => i.Value == "Deleted").ToList().ForEach(i => cboStatusSearch.Items.Remove(i));
                    }
                    else
                    {
                        if (cboStatusSearch.Items.FindByText("Deleted") == null)
                        {
                            cboStatusSearch.Items.Insert(2, "Deleted");

                        }
                    }

                }



                MultipleHierarchySetting = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
                if (MultipleHierarchySetting)
                    organizationHierarchy.Visible = true;

                if ((sn == null) || (sn.UserName == ""))
                {
                    RedirectToLogin();
                    return;
                }
                confirmDeletePlan = (string)base.GetLocalResourceObject("Delete.Confirm");

                sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

                msgPsw_Medium = (string)base.GetLocalResourceObject("msgPsw_Medium");
                msgPsw_Weak = (string)base.GetLocalResourceObject("msgPsw_Weak");
                msgPsw_MoreCharacters = (string)base.GetLocalResourceObject("msgPsw_MoreCharacters");
                msgPsw_Strong = (string)base.GetLocalResourceObject("msgPsw_Strong");
                msgPsw_TypePassword = (string)base.GetLocalResourceObject("msgPsw_TypePassword");
                ExportToExcel = (string)base.GetLocalResourceObject("ExportToExcel");
                this.txtPassword.Attributes.Add("onkeyup", "return passwordChanged();");

                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmVehicleInfo, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    this.chkbxViewDeletedUser.Visible = true;
                    this.tblAddUsers.Visible = false;
                    ViewState["ConfirmDelete"] = "0";
                    DgUsers_Fill();
                    //UserGroupsFill();
                    if (!sn.User.ControlEnable(sn, 71))
                        cmdGroups.Visible = false;
                    if (!sn.User.ControlEnable(sn, 21))
                        cmdUserGroups.Visible = false;
                    if (!sn.User.ControlEnable(sn, 17))
                        cmdUserInfo.Visible = false;
                    if (!sn.User.ControlEnable(sn, 79))
                        cmdGroupConfiguration.Visible = false;
                    if (!sn.User.ControlEnable(sn, 70))
                        cmdControls.Visible = false;
                    if (!sn.User.ControlEnable(sn, 90))
                        cmdServices.Visible = false;
                }
                else
                {
                    //if (!String.IsNullOrEmpty(Password))
                    //    txtPassword.Attributes["value"] = Password;
                    //if (!String.IsNullOrEmpty(ReenterPassword))
                    //    txtReenterPassword.Attributes["value"] = ReenterPassword;
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

        //Changes
        protected void chkbxViewDeletedUser_CheckChanged(object sender, System.EventArgs e)
        {
            cboSearchType.SelectedIndex = cboSearchType.Items.IndexOf(cboSearchType.Items.FindByValue("0"));
            cboStatusSearch.SelectedIndex = cboStatusSearch.Items.IndexOf(cboStatusSearch.Items.FindByValue("0"));
            this.cboStatusSearch.Visible = false;
            txtSearchParam.Visible = true;
            txtSearchParam.Text = "";

            if (ChkbxViewDeletedUser)
            {
                DgUsers_Fill();

                if (!sn.User.ControlEnable(sn, 71))
                    cmdGroups.Visible = false;
                if (!sn.User.ControlEnable(sn, 21))
                    cmdUserGroups.Visible = false;
                if (!sn.User.ControlEnable(sn, 17))
                    cmdUserInfo.Visible = false;
                if (!sn.User.ControlEnable(sn, 79))
                    cmdGroupConfiguration.Visible = false;
                if (!sn.User.ControlEnable(sn, 70))
                    cmdControls.Visible = false;
                if (!sn.User.ControlEnable(sn, 90))
                    cmdServices.Visible = false;
            }
            else
            {
                DgUsers_Fill();

                if (!sn.User.ControlEnable(sn, 71))
                    cmdGroups.Visible = false;
                if (!sn.User.ControlEnable(sn, 21))
                    cmdUserGroups.Visible = false;
                if (!sn.User.ControlEnable(sn, 17))
                    cmdUserInfo.Visible = false;
                if (!sn.User.ControlEnable(sn, 79))
                    cmdGroupConfiguration.Visible = false;
                if (!sn.User.ControlEnable(sn, 70))
                    cmdControls.Visible = false;
                if (!sn.User.ControlEnable(sn, 90))
                    cmdServices.Visible = false;
            }

        }

        private void UserGroupsFill()
        {
            //bool GetUserGroupsbyUser = true;
            DataSet dsGroups = new DataSet();

            try
            {
                string xml = "";
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                if (objUtil.ErrCheck(dbu.GetAllUserGroupsByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetAllUserGroupsByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        ddlUserGroup.DataSource = null;
                        return;
                    }

                if (xml == "")
                    return;

                dsGroups.ReadXml(new StringReader(xml));

                this.ddlUserGroup.DataSource = dsGroups;
                this.ddlUserGroup.DataBind();

                this.ddlUserGroup.Items.Insert(0, new ListItem(base.GetLocalResourceObject("SelectGroup").ToString(), "-1"));
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

        private void DgUsers_Fill()
        {
            try
            {

                DataSet dsUsers = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                if (objUtil.ErrCheck(dbo.GetUsersInfoByOrganizationByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ChkbxViewDeletedUser, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetUsersInfoByOrganizationByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ChkbxViewDeletedUser, ref xml), true))
                    {
                        this.dgUsers.DataSource = null;
                        this.dgUsers.DataBind();
                        imgExcel.Visible = false;
                        return;
                    }


                if (xml == "")
                {
                    this.dgUsers.DataSource = null;
                    this.dgUsers.DataBind();
                    imgExcel.Visible = false;
                    return;
                }

                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                    xml = xml.Replace("Active", "Actif").Replace("Reset Password", "Réinitialiser le mot de passe").Replace("Deactivated", "Désactivé").Replace("Deleted", "Effacé");


                strrXML = new StringReader(xml);
                dsUsers.ReadXml(strrXML);
                foreach (DataRow dr in dsUsers.Tables[0].Rows)
                {
                    try
                    {
                        var lookup = sn.SelectedLanguage.Contains("fr-CA");
                        if (lookup)
                        {
                            if (!(dr["ExpiredDate"] == "Unlimited" || dr["ExpiredDate"] == "Illimitée" || dr["ExpiredDate"] == ""))
                            {
                                DateTime tmpDate = DateTime.ParseExact(dr["ExpiredDate"].ToString(), (string)base.GetLocalResourceObject("resGlobalDateFormat"), CultureInfo.InvariantCulture);
                                dr["ExpiredDate"] = tmpDate.ToString((string)base.GetLocalResourceObject("resDateFormat"));
                            }
                        }
                        //datetime format issues changes
                        else if (!(dr["ExpiredDate"] == "Unlimited" || dr["ExpiredDate"] == "Illimitée" || dr["ExpiredDate"] == ""))
                        {

                            dr["ExpiredDate"] = Convert.ToDateTime(dr["ExpiredDate"]).ToString(sn.User.DateFormat);
                        }
                    }
                    catch
                    {
                    }
                }

                this.dgUsers.DataSource = dsUsers;
                dgUsers.DataBind();
                sn.Misc.ConfDsUsers = dsUsers;

                SetExcel(dsUsers.Tables[0]);
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

        private void SetExcel(DataTable dtUsers)
        {
            StringBuilder _sUsers = new StringBuilder();
            string sUsers = String.Empty;
            string sFirstName = String.Empty;
            string sLastName = String.Empty;
            string sUserName = String.Empty;
            string sExpiredDate = String.Empty;
            string sUserStatus = String.Empty;
            string sUserGroups = String.Empty;
            string sFleets = String.Empty;

            foreach (DataRow drUsers in dtUsers.Rows)
            {
                if (drUsers["FirstName"] != null)
                    sFirstName = drUsers["FirstName"].ToString();
                else
                    sFirstName = string.Empty;
                if (drUsers["LastName"] != null)
                    sLastName = drUsers["LastName"].ToString();
                else
                    sLastName = string.Empty;
                if (drUsers["UserName"] != null)
                    sUserName = drUsers["UserName"].ToString();
                else
                    sUserName = string.Empty;
                if (drUsers["ExpiredDate"] != null)
                    sExpiredDate = drUsers["ExpiredDate"].ToString();
                else
                    sExpiredDate = string.Empty;
                if (drUsers["UserStatus"] != null)
                    sUserStatus = drUsers["UserStatus"].ToString();
                else
                    sUserStatus = string.Empty;
                if (drUsers["UserGroups"] != null)
                    sUserGroups = drUsers["UserGroups"].ToString();
                else
                    sUserGroups = string.Empty;
                if (drUsers["Fleets"] != null)
                    sFleets = drUsers["Fleets"].ToString();
                else
                    sFleets = string.Empty;

                _sUsers.Append(String.Format("[\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"],", sFirstName, sLastName, sUserName, sExpiredDate, sUserStatus, sUserGroups, sFleets));
            }

            if (_sUsers.Length > 0)
            {
                sUsers = _sUsers.ToString().Substring(0, _sUsers.Length - 1);
                expdata.Value = "{\"Header\":[\"" + (string)base.GetLocalResourceObject("dgUsers_FirstName") + "\",\"" + (string)base.GetLocalResourceObject("dgUsers_LastName") + "\",\"" + (string)base.GetLocalResourceObject("dgUsers_UserName") + "\",\"" + (string)base.GetLocalResourceObject("dgUsers_ExpiredDate") + "\",\"" + (string)base.GetLocalResourceObject("dgUsers_Status") + "\",\"" + (string)base.GetLocalResourceObject("dgUsers_UserGroups") + "\",\"" + (string)base.GetLocalResourceObject("dgUsers_Fleets") + "\"],\"Data\":[" + sUsers + "]}";
                imgExcel.Visible = true;
            }
            else
                expdata.Value = String.Empty;
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
            this.cmdCancelExpire.Click += new System.Web.UI.ImageClickEventHandler(this.cmdCancelExpire_Click);
            this.dgUsers.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgUsers_PageIndexChanged);
            this.dgUsers.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgUsers_DeleteCommand);
            this.dgUsers.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgUsers_ItemDataBound);

        }
        #endregion

        protected void cmdFleets_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmEmails.aspx");
        }

        protected void cmdVehicles_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmVehicleInfo.aspx");
        }

        private void dgUsers_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            dgUsers.CurrentPageIndex = e.NewPageIndex;
            DataSet dsUsers = sn.Misc.ConfDsUsers;
            this.dgUsers.DataSource = dsUsers;
            dgUsers.DataBind();
            dgUsers.SelectedIndex = -1;
        }

        protected void cmdAddUser_Click(object sender, System.EventArgs e)
        {
            this.chkbxViewDeletedUser.Visible = false;
            this.tblAddUsers.Visible = true;
            imgExcel.Visible = false;
            ClearFields();
            this.dgUsers.Visible = false;
            this.cmdAddUser.Visible = false;

            this.tblPassword.Visible = true;
            this.valPassword.Enabled = true;
            this.valPasswordComp.Enabled = true;

            this.tblSearch.Visible = false;
            this.lnkResetPassword.Visible = false;

            lstUnassignedGroups_Fill();
            lstAssignedGroups_Fill();

            //this.lblUserGroup.Visible = true;
            //this.ddlUserGroup.Visible = true;
            //this.CompareUserGroup.Enabled = true;

            HierarchyTree1.Field_OrganizationHierarchyNodeCode = "";
            HierarchyTree1.SetOrganizationHierarchyPath();
            //this.lblUserGroup.Visible = true;
            //this.ddlUserGroup.Visible = true;
            //this.CompareUserGroup.Enabled = true;

            Password = String.Empty;
            ReenterPassword = String.Empty;
            PasswordStatus = String.Empty;
            txtPassword.Attributes["value"] = Password;
            txtReenterPassword.Attributes["value"] = ReenterPassword;
            txtPasswordStatus.Value = PasswordStatus;
        }

        protected void cmdCancelAddUser_Click(object sender, System.EventArgs e)
        {
            this.chkbxViewDeletedUser.Visible = true;
            this.tblAddUsers.Visible = false;
            imgExcel.Visible = true;
            this.dgUsers.Visible = true;
            this.tblSearch.Visible = true;
            this.cmdAddUser.Visible = true;
            ClearFields();
        }

        protected void cmdSaveUser_Click(object sender, System.EventArgs e)
        {
            string sUserGroupIDs = string.Empty;

            try
            {
                int pageInd = dgUsers.CurrentPageIndex;
                Page.Validate();
                if (!Page.IsValid)
                    return;

                if (lstAssignedGroups.Items.Count < 1)
                {
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectUserGroupError");
                    this.lblMessage.Visible = true;
                    return;
                }

                ViewState["ConfirmDelete"] = "0";
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();
                //DumpBeforeCall(sn, string.Format("frmUsers -- cmdSaveUser_Click : SystemUserName={0}  UpdatedUsername = {1}", sn.UserName, this.txtUserName.Text));

                string ExpireDate = VLF.CLS.Def.Const.unassignedDateTime.ToString();

                try
                {
                    if (this.txtExpire.Text != "Unlimited" && this.txtExpire.Text != "Illimitée" && this.txtExpire.Text != "Illimité")
                    {
                        try
                        {
                            var lookup = sn.SelectedLanguage.Contains("fr-CA");
                            if (lookup)
                            {
                                DateTime date1 = DateTime.ParseExact(txtExpire.Text, (string)base.GetLocalResourceObject("resDateFormat"), CultureInfo.InvariantCulture);
                                ExpireDate = date1.ToString();
                            }
                            else
                            {

                                ExpireDate = txtExpire.Text;
                                DateTime date1 = Convert.ToDateTime(ExpireDate);
                            }
                        }
                        catch
                        {
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("DateConversion.ErrorMessage");
                            this.lblMessage.Visible = true;
                            return;
                        }
                    }
                }
                catch { ExpireDate = VLF.CLS.Def.Const.unassignedDateTime.ToString(); }

                foreach (ListItem item in lstAssignedGroups.Items)
                {
                    sUserGroupIDs = sUserGroupIDs + item.Value + ";";
                }

                if (sUserGroupIDs.Length > 0)
                    sUserGroupIDs = sUserGroupIDs.Substring(0, sUserGroupIDs.Length - 1);

                int _UserID;

                if (this.lblUserId.Text == "0") // add new user
                {
                    string PasswordStatus = txtPasswordStatus.Value; //Request.Form["txtPasswordStatus"];
                    if (PasswordStatus == "")
                        return;
                    else if (PasswordStatus != "1")
                    {
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("AddUserPasswordFailed21");
                        this.lblMessage.Visible = true;
                        return;
                    }

                    int addResult = dbu.AddUserToGroups(sn.UserID, sn.SecId, sn.User.OrganizationId, this.txtUserName.Text, "", this.txtPassword.Text, this.txtFirstName.Text, this.txtLastName.Text, ExpireDate, sUserGroupIDs);
                    if (objUtil.ErrCheck(addResult, false))
                        if (objUtil.ErrCheck(addResult, true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Add New User failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            this.cmdAddUser.Enabled = true;
                            this.lblMessage.Visible = true;
                            if (addResult == 21)
                            {
                                this.lblMessage.Visible = true;
                                this.lblMessage.Text = (string)base.GetLocalResourceObject("AddUserPasswordFailed21");
                            }
                            else if (addResult == 22)
                            {
                                this.lblMessage.Visible = true;
                                this.lblMessage.Text = (string)base.GetLocalResourceObject("AddUserPasswordFailed22");
                            }
                            else
                            {
                                this.lblMessage.Visible = true;
                                this.lblMessage.Text = (string)base.GetLocalResourceObject("AddUserFailed");
                            }
                            return;
                        }


                    #region Assign User to Top UP fleet
                    try
                    {
                        ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
                        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                        if (sn.User.OrganizationId == 951)
                        {
                            VLF.DAS.Logic.User dlU = new VLF.DAS.Logic.User(sConnectionString);
                            int userIdNew = dlU.GetUserIdByUserName(this.txtUserName.Text);
                            int topUPFleetId = 11881;

                            if (objUtil.ErrCheck(dbf.AddUserToFleet(sn.UserID, sn.SecId, topUPFleetId, userIdNew), false))
                                if (objUtil.ErrCheck(dbf.AddUserToFleet(sn.UserID, sn.SecId, topUPFleetId, userIdNew), false))
                                {
                                    return;
                                }
                        }
                    }
                    catch
                    {
                    }
                    #endregion



                    this.lblMessage.Text = (string)base.GetLocalResourceObject("UserAdded");
                    _UserID = poh.GetUserIdByUserName(this.txtUserName.Text, sn.User.OrganizationId);
                }
                else // update selected user
                {
                    string UserId = this.lblUserId.Text;
                    string userStatus = "";

                    switch (this.cboStatus.SelectedItem.Value)
                    {
                        case "1": //Activate
                            if (!(this.txtExpire.Text == "Unlimited" || this.txtExpire.Text == "Illimitée" || this.txtExpire.Text == "" || this.txtExpire.Text == "Illimité"))
                            {
                                if (Convert.ToDateTime(ExpireDate) <= DateTime.UtcNow)
                                {
                                    this.lblMessage.Text = (string)base.GetLocalResourceObject("FutureExpirationDate.ErrorMessage");
                                    this.lblMessage.Visible = true;
                                    return;
                                }
                            }
                            else
                            {
                                ExpireDate = VLF.CLS.Def.Const.unassignedDateTime.ToString();
                            }
                            userStatus = "Active";
                            break;
                        case "2": //Deleted
                            if (this.txtExpire.Text == "Unlimited" || this.txtExpire.Text == "Illimitée" || this.txtExpire.Text == "" || this.txtExpire.Text == "Illimité")
                            {
                                ExpireDate = VLF.CLS.Def.Const.unassignedDateTime.ToString();
                            }
                            else
                            {
                                ExpireDate = txtExpire.Text;
                            }
                            userStatus = "Deleted";
                            break;
                        case "3": //Reset Password
                            if (!(this.txtExpire.Text == "Unlimited" || this.txtExpire.Text == "Illimitée" || this.txtExpire.Text == "" || this.txtExpire.Text == "Illimité"))
                            {
                                if (Convert.ToDateTime(ExpireDate) <= DateTime.UtcNow)
                                {
                                    this.lblMessage.Text = (string)base.GetLocalResourceObject("FutureExpirationDate.ErrorMessage");
                                    this.lblMessage.Visible = true;
                                    return;
                                }
                            }
                            else
                            {
                                ExpireDate = VLF.CLS.Def.Const.unassignedDateTime.ToString();
                            }
                            userStatus = "Locked";
                            break;

                        case "4": //Deactivated

                            if (!(this.txtExpire.Text == "Unlimited" || this.txtExpire.Text == "Illimitée" || this.txtExpire.Text == "" || this.txtExpire.Text == "Illimité"))
                            {
                                if (Convert.ToDateTime(ExpireDate) > DateTime.UtcNow)
                                {
                                    this.lblMessage.Text = (string)base.GetLocalResourceObject("PastExpirationDate.ErrorMessage");
                                    this.lblMessage.Visible = true;
                                    return;
                                }
                            }
                            else
                            {
                                this.lblMessage.Text = (string)base.GetLocalResourceObject("PastExpirationDate.ErrorMessage");
                                this.lblMessage.Visible = true;
                                return;
                            }

                            ExpireDate = txtExpire.Text;
                            userStatus = "Deactivated";
                            break;

                        case "0":

                            if (this.lblUserStatusText.Text == "Active" || this.lblUserStatusText.Text == "Actif")
                            {
                                userStatus = "Active";
                                if (!(this.txtExpire.Text == "Unlimited" || this.txtExpire.Text == "Illimitée" || this.txtExpire.Text == "" || this.txtExpire.Text == "Illimité"))
                                {

                                    if (Convert.ToDateTime(ExpireDate) < DateTime.UtcNow)
                                    {
                                        this.lblMessage.Text = (string)base.GetLocalResourceObject("FutureExpirationDate.ErrorMessage");
                                        this.lblMessage.Visible = true;
                                        return;
                                    }
                                }
                                else
                                {
                                    ExpireDate = VLF.CLS.Def.Const.unassignedDateTime.ToString();
                                }

                            }

                            if (this.lblUserStatusText.Text == "Reset Password" || this.lblUserStatusText.Text == "Réinitialiser le mot de passe")
                            {
                                userStatus = "Locked";
                                if (!(this.txtExpire.Text == "Unlimited" || this.txtExpire.Text == "Illimitée" || this.txtExpire.Text == "" || this.txtExpire.Text == "Illimité"))
                                {
                                    if (Convert.ToDateTime(ExpireDate) < DateTime.UtcNow)
                                    {
                                        this.lblMessage.Text = (string)base.GetLocalResourceObject("FutureExpirationDate.ErrorMessage");
                                        this.lblMessage.Visible = true;
                                        return;
                                    }
                                }
                                else
                                {
                                    ExpireDate = VLF.CLS.Def.Const.unassignedDateTime.ToString();
                                }

                            }

                            if (this.lblUserStatusText.Text == "Deactivated" || this.lblUserStatusText.Text == "Désactivé")
                            {
                                userStatus = "Deactivated";
                                if (!(this.txtExpire.Text == "Unlimited" || this.txtExpire.Text == "Illimitée" || this.txtExpire.Text == "" || this.txtExpire.Text == "Illimité"))
                                {
                                    if (Convert.ToDateTime(ExpireDate) > DateTime.UtcNow)
                                    {
                                        this.lblMessage.Text = (string)base.GetLocalResourceObject("PastExpirationDate.ErrorMessage");
                                        this.lblMessage.Visible = true;
                                        return;

                                    }

                                }
                                else
                                {
                                    this.lblMessage.Text = (string)base.GetLocalResourceObject("PastExpirationDate.ErrorMessage");
                                    this.lblMessage.Visible = true;
                                    return;
                                }
                            }

                            if (this.lblUserStatusText.Text == "Deleted" || this.lblUserStatusText.Text == "Effacé")
                            {
                                userStatus = "Deleted";
                                if (!(this.txtExpire.Text == "Unlimited" || this.txtExpire.Text == "Illimitée" || this.txtExpire.Text == "" || this.txtExpire.Text == "Illimité"))
                                {


                                }
                                else
                                {
                                    ExpireDate = VLF.CLS.Def.Const.unassignedDateTime.ToString();
                                }

                            }
                            break;
                    }



                    if (objUtil.ErrCheck(dbu.UpdateUserInfoStatusAndGroups(sn.UserID, sn.SecId, Convert.ToInt32(UserId), txtUserName.Text, txtFirstName.Text, txtLastName.Text, ExpireDate, userStatus, sUserGroupIDs), false))
                        if (objUtil.ErrCheck(dbu.UpdateUserInfoStatusAndGroups(sn.UserID, sn.SecId, Convert.ToInt32(UserId), txtUserName.Text, txtFirstName.Text, txtLastName.Text, ExpireDate, userStatus, sUserGroupIDs), true))
                        {
                            this.lblMessage.Visible = true;
                            this.cmdAddUser.Enabled = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("UserUpdateFailed.ErrorMessage");
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Update User failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            this.cmdAddUser.Enabled = true;
                            return;
                        }

                    if (sn.UserID == int.Parse(UserId))
                    {
                        Session["IsReset"] = true;
                    }
                    lblMessage.Text = (string)base.GetLocalResourceObject("UserUpdated");

                    _UserID = Convert.ToInt32(UserId);
                }

                if (MultipleHierarchySetting && _UserID > 0)
                {
                    string[] assignedNodeCodes = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, _UserID, true).Split(',');
                    int[] assignedFleets = new int[assignedNodeCodes.Length];

                    ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                    for (int i = 0; i < assignedNodeCodes.Length; i++)
                    {
                        if (assignedNodeCodes[i] != string.Empty)
                        {
                            assignedFleets[i] = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, assignedNodeCodes[i]);
                        }
                    }
                    string[] toAssignedFleetIds = HierarchyTree1.Field_OrganizationHierarchyFleetId.Split(',');
                    foreach (string toids in toAssignedFleetIds)
                    {
                        if (toids != string.Empty)
                        {
                            int pos = Array.IndexOf(assignedFleets, Convert.ToInt32(toids));

                            //Already Assigned, remove from assignedFleets
                            if (pos > -1)
                            {
                                assignedFleets = assignedFleets.Where(val => val != Convert.ToInt32(toids)).ToArray();
                            }
                            // otherwise we'll assign the fleet to the user
                            else
                            {
                                if (objUtil.ErrCheck(dbf.AddUserToFleet(sn.UserID, sn.SecId, Convert.ToInt32(toids), _UserID), false))
                                    if (objUtil.ErrCheck(dbf.AddUserToFleet(sn.UserID, sn.SecId, Convert.ToInt32(toids), _UserID), false))
                                    {
                                        return;
                                    }
                            }
                        }
                    }
                    foreach (int todeletedId in assignedFleets)
                    {
                        if (todeletedId > 0)
                        {
                            if (objUtil.ErrCheck(dbf.DeleteUserFromFleet(sn.UserID, sn.SecId, todeletedId, _UserID), false))
                                if (objUtil.ErrCheck(dbf.DeleteUserFromFleet(sn.UserID, sn.SecId, todeletedId, _UserID), false))
                                {
                                    return;
                                }
                        }
                    }
                }

                cmdSearch_Click1(sender, e);

                //DgUsers_Fill();
                ClearFields();
                this.chkbxViewDeletedUser.Visible = true;
                this.lblMessage.Visible = true;
                this.cmdAddUser.Enabled = true;
                this.tblAddUsers.Visible = false;
                imgExcel.Visible = true;
                this.dgUsers.Visible = true;
                this.tblSearch.Visible = true;
                this.cmdAddUser.Visible = true;

                // dgUsers.SelectedIndex = -1;
                if (pageInd + 1 <= dgUsers.PageCount)
                {
                    dgUsers.CurrentPageIndex = pageInd;

                }
                else
                {
                    dgUsers.CurrentPageIndex = 0;
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

        protected void dgUsers_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {
                //Check security 
                bool cmd = sn.User.ControlEnable(sn, 19);
                if (!cmd)
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = (string)base.GetLocalResourceObject("NoPermissions");
                    return;
                }

                if (confirm == "")
                    return;

                lblMessage.Visible = true;
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                if (objUtil.ErrCheck(dbu.DeleteUserByUserId(sn.UserID, sn.SecId, Convert.ToInt32(dgUsers.DataKeys[e.Item.ItemIndex].ToString())), false))
                    if (objUtil.ErrCheck(dbu.DeleteUserByUserId(sn.UserID, sn.SecId, Convert.ToInt32(dgUsers.DataKeys[e.Item.ItemIndex].ToString())), true))
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = "Delete user failed.";
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Delete User failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        return;
                    }


                dgUsers.SelectedIndex = -1;
                lblMessage.Text = (string)base.GetLocalResourceObject("UserDeleted");
                dgUsers.CurrentPageIndex = 0;
                DgUsers_Fill();
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

        protected void cmdUserGroups_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmUserGroups.aspx");
        }

        protected void cmdUsers_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmVehicleInfo.aspx");
        }

        protected void dgUsers_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //Check security 
            bool cmdUpdate = sn.User.ControlEnable(sn, 20);

            //if (dgUsers.SelectedItem.Cells[5].Text == "Deleted" || dgUsers.SelectedItem.Cells[5].Text == "Effacé")
            //{
            //    lblMessage.Visible = true;
            //    lblMessage.Text = (string)base.GetLocalResourceObject("Deleteduserupdate.WarningMessage");
            //    return;
            //}
            if (!cmdUpdate)
            {
                lblMessage.Visible = true;
                lblMessage.Text = (string)base.GetLocalResourceObject("NoPermissions");
                return;
            }

            HierarchyTree1.Field_OrganizationHierarchyNodeCode = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, int.Parse(dgUsers.DataKeys[dgUsers.SelectedIndex].ToString()), true);
            HierarchyTree1.SetOrganizationHierarchyPath();

            this.chkbxViewDeletedUser.Visible = false;
            this.tblAddUsers.Visible = true;
            imgExcel.Visible = false;
            this.lblMessage.Text = "";
            this.dgUsers.Visible = false;
            this.tblSearch.Visible = false;
            this.cmdAddUser.Visible = false;

            this.tblPassword.Visible = false;
            this.valPassword.Enabled = false;
            this.valPasswordComp.Enabled = false;

            //this.lblUserGroup.Visible = false;
            //this.ddlUserGroup.Visible = false;
            //this.CompareUserGroup.Enabled = false;

            //if (sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2 || sn.User.UserGroupId == 7 || sn.User.UserGroupId == 14)
            if (sn.User.ControlEnable(sn, 81))
                this.lnkResetPassword.Visible = true;
            else
                this.lnkResetPassword.Visible = false;

            this.cboStatus.Items.Clear();

            this.lblUserId.Text = dgUsers.DataKeys[dgUsers.SelectedIndex].ToString();
            this.lnkResetPassword.OnClientClick = "javascript:ResetPassword(" + this.lblUserId.Text + ")";
            this.txtFirstName.Text = dgUsers.SelectedItem.Cells[1].Text;
            this.txtLastName.Text = dgUsers.SelectedItem.Cells[2].Text;
            this.txtUserName.Text = dgUsers.SelectedItem.Cells[3].Text;
            this.txtExpire.Text = dgUsers.SelectedItem.Cells[4].Text;
            this.lblUserStatusText.Text = dgUsers.SelectedItem.Cells[5].Text;

            Password = String.Empty;
            ReenterPassword = String.Empty;
            PasswordStatus = String.Empty;
            txtPassword.Attributes["value"] = Password;
            txtReenterPassword.Attributes["value"] = ReenterPassword;
            txtPasswordStatus.Value = PasswordStatus;

            lstUnassignedGroups_Fill();
            lstAssignedGroups_Fill();

            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                this.cboStatus.Items.Add(new ListItem("Sélectionner s’il vous plaît", "0"));
            else
                this.cboStatus.Items.Add(new ListItem("Please select", "0"));

            this.cboStatus.Visible = true;
            this.lblChangeStatus.Visible = true;
            this.tblExpire.Visible = true;


            if (this.lblUserStatusText.Text == "Active" || this.lblUserStatusText.Text == "Actif")
            {
                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                {
                    this.cboStatus.Items.Add(new ListItem("Effacer", "2"));
                    this.cboStatus.Items.Add(new ListItem("Réinitialiser le mot de passe", "3"));
                    this.cboStatus.Items.Add(new ListItem("Désactiver", "4"));
                }
                else
                {
                    this.cboStatus.Items.Add(new ListItem("Delete", "2"));
                    this.cboStatus.Items.Add(new ListItem("Reset Password", "3"));
                    this.cboStatus.Items.Add(new ListItem("Deactivate", "4"));
                }
            }

            else if (this.lblUserStatusText.Text == "Deleted" || this.lblUserStatusText.Text == "Effacé")
            {
                //this.tblExpire.Visible = false;
                // this.cmdCancelExpire.Enabled = false;

                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                {
                    this.cboStatus.Items.Add(new ListItem("Activer", "1"));
                }
                else
                {
                    this.cboStatus.Items.Add(new ListItem("Activate", "1"));
                }
            }
            else if (this.lblUserStatusText.Text == "Deactivated" || this.lblUserStatusText.Text == "Désactivé")
            {
                //this.tblExpire.Visible = false;
                // this.cmdCancelExpire.Enabled = false;

                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                {
                    this.cboStatus.Items.Add(new ListItem("Activer", "1"));
                    this.cboStatus.Items.Add(new ListItem("Effacer", "2"));
                    this.cboStatus.Items.Add(new ListItem("Réinitialiser le mot de passe", "3"));
                }
                else
                {
                    this.cboStatus.Items.Add(new ListItem("Activate", "1"));
                    this.cboStatus.Items.Add(new ListItem("Delete", "2"));
                    this.cboStatus.Items.Add(new ListItem("Reset Password", "3"));
                }
            }
            else if (this.lblUserStatusText.Text == "Reset Password" || this.lblUserStatusText.Text == "Réinitialiser le mot de passe")
            {
                //this.tblExpire.Visible = false;
                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                {
                    this.cboStatus.Items.Add(new ListItem("Activer", "1"));
                    this.cboStatus.Items.Add(new ListItem("Effacer", "2"));
                    this.cboStatus.Items.Add(new ListItem("Désactiver", "4"));
                }
                else
                {
                    this.cboStatus.Items.Add(new ListItem("Activate", "1"));
                    this.cboStatus.Items.Add(new ListItem("Delete", "2"));
                    this.cboStatus.Items.Add(new ListItem("Deactivate", "4"));
                }
            }

            else if (this.lblUserStatusText.Text == "Deactivate" || this.lblUserStatusText.Text == "Désactiver")
            {
                //this.tblExpire.Visible = false;
                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                {
                    this.cboStatus.Items.Add(new ListItem("Activer", "1"));
                    this.cboStatus.Items.Add(new ListItem("Effacer", "2"));
                    this.cboStatus.Items.Add(new ListItem("Réinitialiser le mot de passe", "3"));
                }
                else
                {
                    this.cboStatus.Items.Add(new ListItem("Activate", "1"));
                    this.cboStatus.Items.Add(new ListItem("Delete", "2"));
                    this.cboStatus.Items.Add(new ListItem("Reset Password", "3"));
                }
            }
            else
            {
                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                {
                    this.cboStatus.Items.Add(new ListItem("Activer", "1"));
                    this.cboStatus.Items.Add(new ListItem("Effacer", "2"));
                    this.cboStatus.Items.Add(new ListItem("Réinitialiser le mot de passe", "3"));
                    this.cboStatus.Items.Add(new ListItem("Désactiver", "4"));
                }
                else
                {
                    this.cboStatus.Items.Add(new ListItem("Activate", "1"));
                    this.cboStatus.Items.Add(new ListItem("Delete", "2"));
                    this.cboStatus.Items.Add(new ListItem("Reset Password", "3"));
                    this.cboStatus.Items.Add(new ListItem("Deactivate", "4"));
                }

            }
        }

        private void cmdCancelExpire_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                this.txtExpire.Text = "Illimitée";
            else
                this.txtExpire.Text = "Unlimited";
        }

        private void ClearFields()
        {
            this.lblUserId.Text = "0";
            this.txtFirstName.Text = "";
            this.txtLastName.Text = "";
            this.txtPassword.Text = "";
            this.txtUserName.Text = "";
            this.lblMessage.Text = "";
            this.txtExpire.Text = (string)base.GetLocalResourceObject("ExpireDate");
        }

        private void dgUsers_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                e.Item.Cells[8].ToolTip = (string)base.GetLocalResourceObject("EditToolTip");
                //e.Item.Cells[7].ToolTip = (string)base.GetLocalResourceObject("DeleteToolTip");

                LinkButton settings = (LinkButton)e.Item.Cells[9].Controls[0];

                //if (sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2)
                if (sn.User.ControlEnable(sn, 78))
                {
                    string windowOpen = "preferenceWindow('" + dgUsers.DataKeys[e.Item.ItemIndex].ToString() + "','" + e.Item.Cells[3].Text + "')";
                    settings.Attributes.Add("onclick", windowOpen);
                }
                else
                {
                    settings.Visible = false;
                }
            }
        }

        protected void dgUsers_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            //if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            //{
            //   confirm = "return confirm('" + (string)base.GetLocalResourceObject("ConfirmDelete") + "')";
            //   LinkButton deleteBtn = (LinkButton)e.Item.Cells[6].Controls[0];
            //   deleteBtn.Attributes.Add("onclick", confirm);
            //}
        }

        protected void cmdSearch_Click1(object sender, EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";
                DataRow[] drCollections = null;
                //if (sn.Misc.ConfDsUsers == null || sn.Misc.ConfDsUsers.Tables.Count == 0)
                //{
                //    this.lblMessage.Visible = true;
                //    this.lblMessage.Text = "Load users error!";
                //    return;
                //}
                //DataTable dt = sn.Misc.ConfDsUsers.Tables[0].Clone();
                //sn.Misc.ConfUsersSelectedGridPage = dgUsers.CurrentPageIndex;

                DataSet dsUsers = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();


                if (objUtil.ErrCheck(dbo.GetUsersInfoByOrganizationByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ChkbxViewDeletedUser, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetUsersInfoByOrganizationByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ChkbxViewDeletedUser, ref xml), true))
                    {
                        this.dgUsers.DataSource = null;
                        this.dgUsers.DataBind();
                        return;
                    }


                if (xml == "")
                {
                    this.dgUsers.DataSource = null;
                    this.dgUsers.DataBind();
                    return;
                }

                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                    xml = xml.Replace("Active", "Actif").Replace("Reset Password", "Réinitialiser le mot de passe").Replace("Deactivated", "Désactivé").Replace("Deleted", "Effacé");


                strrXML = new StringReader(xml);
                dsUsers.ReadXml(strrXML);
                DataTable dt = dsUsers.Tables[0].Clone();

                //dgUsers.CurrentPageIndex = 0;
                string filter = "";
                switch (cboSearchType.SelectedItem.Value)
                {
                    case "0":
                        if (!String.IsNullOrEmpty(this.txtSearchParam.Text.Trim()))
                            filter = String.Format("UserName like '%{0}%'", this.txtSearchParam.Text.Replace("'", "''"));
                        break;
                    case "1":
                        if (!String.IsNullOrEmpty(this.txtSearchParam.Text.Trim()))
                            filter = String.Format("LastName like '%{0}%'", this.txtSearchParam.Text);
                        break;
                    case "3":
                        if (this.cboStatusSearch.SelectedItem.Value != "0")
                            filter = String.Format("UserStatus like '%{0}%'", this.cboStatusSearch.SelectedItem.Text);
                        else
                            filter = "";
                        break;
                }

                if (filter == "")
                {
                    foreach (DataRow dr in dsUsers.Tables[0].Rows)
                    {
                        try
                        {
                            var lookup = sn.SelectedLanguage.Contains("fr-CA");
                            if (lookup)
                            {
                                if (!(dr["ExpiredDate"] == "Unlimited" || dr["ExpiredDate"] == "Illimitée" || dr["ExpiredDate"] == ""))
                                {
                                    DateTime tmpDate = DateTime.ParseExact(dr["ExpiredDate"].ToString(), (string)base.GetLocalResourceObject("resGlobalDateFormat"), CultureInfo.InvariantCulture);
                                    dr["ExpiredDate"] = tmpDate.ToString((string)base.GetLocalResourceObject("resDateFormat"));
                                }
                            }
                            //datetime format issues changes
                            else if (!(dr["ExpiredDate"] == "Unlimited" || dr["ExpiredDate"] == "Illimitée" || dr["ExpiredDate"] == ""))
                            {

                                dr["ExpiredDate"] = Convert.ToDateTime(dr["ExpiredDate"]).ToString(sn.User.DateFormat);
                            }
                        }
                        catch
                        {
                        }
                    }

                    this.dgUsers.DataSource = dsUsers;
                    this.dgUsers.DataBind();
                    sn.Misc.ConfDsUsers = dsUsers;

                    SetExcel(dsUsers.Tables[0]);
                }

                else
                {
                    drCollections = dsUsers.Tables[0].Select(filter);
                    if (drCollections != null && drCollections.Length > 0)
                    {
                        foreach (DataRow dr in drCollections)
                            dt.ImportRow(dr);
                    }
                    //

                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {
                            var lookup = sn.SelectedLanguage.Contains("fr-CA");
                            if (lookup)
                            {
                                if (!(dr["ExpiredDate"] == "Unlimited" || dr["ExpiredDate"] == "Illimitée" || dr["ExpiredDate"] == ""))
                                {
                                    DateTime tmpDate = DateTime.ParseExact(dr["ExpiredDate"].ToString(), (string)base.GetLocalResourceObject("resGlobalDateFormat"), CultureInfo.InvariantCulture);
                                    dr["ExpiredDate"] = tmpDate.ToString((string)base.GetLocalResourceObject("resDateFormat"));
                                }
                            }
                            //datetime format issues changes
                            else if (!(dr["ExpiredDate"] == "Unlimited" || dr["ExpiredDate"] == "Illimitée" || dr["ExpiredDate"] == ""))
                            {

                                dr["ExpiredDate"] = Convert.ToDateTime(dr["ExpiredDate"]).ToString(sn.User.DateFormat);
                            }
                        }
                        catch
                        {
                        }
                    }


                    this.dgUsers.DataSource = dt;
                    this.dgUsers.DataBind();
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    sn.Misc.ConfDsUsers = ds;

                    SetExcel(dt);
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
        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";
                DataRow[] drCollections = null;
                //if (sn.Misc.ConfDsUsers == null || sn.Misc.ConfDsUsers.Tables.Count == 0)
                //{
                //    this.lblMessage.Visible = true;
                //    this.lblMessage.Text = "Load users error!";
                //    return;
                //}
                //DataTable dt = sn.Misc.ConfDsUsers.Tables[0].Clone();
                //sn.Misc.ConfUsersSelectedGridPage = dgUsers.CurrentPageIndex;

                DataSet dsUsers = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();


                if (objUtil.ErrCheck(dbo.GetUsersInfoByOrganizationByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ChkbxViewDeletedUser, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetUsersInfoByOrganizationByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ChkbxViewDeletedUser, ref xml), true))
                    {
                        this.dgUsers.DataSource = null;
                        this.dgUsers.DataBind();
                        imgExcel.Visible = false;
                        return;
                    }


                if (xml == "")
                {
                    this.dgUsers.DataSource = null;
                    this.dgUsers.DataBind();
                    imgExcel.Visible = false;
                    return;
                }

                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                    xml = xml.Replace("Active", "Actif").Replace("Reset Password", "Réinitialiser le mot de passe").Replace("Deactivated", "Désactivé").Replace("Deleted", "Effacé");


                strrXML = new StringReader(xml);
                dsUsers.ReadXml(strrXML);
                DataTable dt = dsUsers.Tables[0].Clone();

                dgUsers.CurrentPageIndex = 0;
                string filter = "";
                switch (cboSearchType.SelectedItem.Value)
                {
                    case "0":
                        if (!String.IsNullOrEmpty(this.txtSearchParam.Text.Trim()))
                            filter = String.Format("UserName like '%{0}%'", this.txtSearchParam.Text.Replace("'", "''"));
                        break;
                    case "1":
                        if (!String.IsNullOrEmpty(this.txtSearchParam.Text.Trim()))
                            filter = String.Format("LastName like '%{0}%'", this.txtSearchParam.Text);
                        break;
                    case "3":
                        if (this.cboStatusSearch.SelectedItem.Value != "0")
                            filter = String.Format("UserStatus like '%{0}%'", this.cboStatusSearch.SelectedItem.Text);
                        else
                            filter = "";
                        break;
                }

                if (filter == "")
                {
                    foreach (DataRow dr in dsUsers.Tables[0].Rows)
                    {
                        try
                        {
                            var lookup = sn.SelectedLanguage.Contains("fr-CA");
                            if (lookup)
                            {
                                if (!(dr["ExpiredDate"] == "Unlimited" || dr["ExpiredDate"] == "Illimitée" || dr["ExpiredDate"] == ""))
                                {
                                    DateTime tmpDate = DateTime.ParseExact(dr["ExpiredDate"].ToString(), (string)base.GetLocalResourceObject("resGlobalDateFormat"), CultureInfo.InvariantCulture);
                                    dr["ExpiredDate"] = tmpDate.ToString((string)base.GetLocalResourceObject("resDateFormat"));
                                }
                            }
                            //datetime format issues changes
                            else if (!(dr["ExpiredDate"] == "Unlimited" || dr["ExpiredDate"] == "Illimitée" || dr["ExpiredDate"] == ""))
                            {

                                dr["ExpiredDate"] = Convert.ToDateTime(dr["ExpiredDate"]).ToString(sn.User.DateFormat);
                            }
                        }
                        catch
                        {
                        }
                    }

                    this.dgUsers.DataSource = dsUsers;
                    this.dgUsers.DataBind();
                    sn.Misc.ConfDsUsers = dsUsers;

                    SetExcel(dsUsers.Tables[0]);
                }
                else
                {
                    drCollections = dsUsers.Tables[0].Select(filter);
                    if (drCollections != null && drCollections.Length > 0)
                    {
                        foreach (DataRow dr in drCollections)
                            dt.ImportRow(dr);
                    }
                    //

                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {
                            var lookup = sn.SelectedLanguage.Contains("fr-CA");
                            if (lookup)
                            {
                                if (!(dr["ExpiredDate"] == "Unlimited" || dr["ExpiredDate"] == "Illimitée" || dr["ExpiredDate"] == ""))
                                {
                                    DateTime tmpDate = DateTime.ParseExact(dr["ExpiredDate"].ToString(), (string)base.GetLocalResourceObject("resGlobalDateFormat"), CultureInfo.InvariantCulture);
                                    dr["ExpiredDate"] = tmpDate.ToString((string)base.GetLocalResourceObject("resDateFormat"));
                                }
                            }
                            //datetime format issues changes
                            else if (!(dr["ExpiredDate"] == "Unlimited" || dr["ExpiredDate"] == "Illimitée" || dr["ExpiredDate"] == ""))
                            {

                                dr["ExpiredDate"] = Convert.ToDateTime(dr["ExpiredDate"]).ToString(sn.User.DateFormat);
                            }
                        }
                        catch
                        {
                        }
                    }


                    this.dgUsers.DataSource = dt;
                    this.dgUsers.DataBind();
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    sn.Misc.ConfDsUsers = ds;

                    SetExcel(dt);
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

        protected void cmdClear_Click(object sender, EventArgs e)
        {
            this.txtSearchParam.Text = "";
            this.lblMessage.Text = "";
            dgUsers.SelectedIndex = -1;
            dgUsers.CurrentPageIndex = sn.Misc.ConfUsersSelectedGridPage;
            this.dgUsers.DataSource = sn.Misc.ConfDsUsers;
            this.dgUsers.DataBind();

            SetExcel(sn.Misc.ConfDsUsers.Tables[0]);
        }

        protected void cmdUserDashBoards_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmuserdashboards.aspx");
        }

        protected void cmdScheduledTasks_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmTaskScheduler.aspx");
        }

        protected void cmdGroups_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmgroups.aspx");
        }

        protected void cmdGroupConfiguration_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmOperationGroups.aspx");
        }

        protected void cmdControls_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmControls.aspx");
        }

        protected void cmdServices_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmServices.aspx");
        }

        protected void cboSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSearchType.SelectedIndex == 2)
            {
                cboStatusSearch.SelectedIndex = cboStatusSearch.Items.IndexOf(cboStatusSearch.Items.FindByValue("0"));
                txtSearchParam.Visible = false;
                this.cboStatusSearch.Visible = true;
                //    if (chkbxViewDeletedUser.Checked == false)
                //    {               

                //        cboStatusSearch.Items.Cast<ListItem>().Where(i => i.Value == "Deleted").ToList().ForEach(i => cboStatusSearch.Items.Remove(i));
                //    }            
                //    else
                //    {
                //        if (cboStatusSearch.Items.FindByText("Deleted") == null)
                //        {
                //            cboStatusSearch.Items.Insert(2, "Deleted");

                //        }
                //}



            }
            else
            {
                txtSearchParam.Visible = true;
                this.cboStatusSearch.Visible = false;
            }
        }

        protected void cmdAdd_Click(object sender, System.EventArgs e)
        {
            Password = txtPassword.Text;
            ReenterPassword = txtReenterPassword.Text;
            PasswordStatus = txtPasswordStatus.Value;

            if (lstUnassignedGroups.SelectedIndex > -1)
            {
                TransferSelectedListItems(lstUnassignedGroups, lstAssignedGroups);
            }

            if (!String.IsNullOrEmpty(Password))
                txtPassword.Attributes["value"] = Password;
            if (!String.IsNullOrEmpty(ReenterPassword))
                txtReenterPassword.Attributes["value"] = ReenterPassword;
            if (!String.IsNullOrEmpty(PasswordStatus))
                txtPasswordStatus.Value = PasswordStatus;
        }

        protected void cmdRemove_Click(object sender, System.EventArgs e)
        {
            Password = txtPassword.Text;
            ReenterPassword = txtReenterPassword.Text;
            PasswordStatus = txtPasswordStatus.Value;

            if (lstAssignedGroups.SelectedIndex > -1)
            {
                TransferSelectedListItems(lstAssignedGroups, lstUnassignedGroups);
            }

            if (!String.IsNullOrEmpty(Password))
                txtPassword.Attributes["value"] = Password;
            if (!String.IsNullOrEmpty(ReenterPassword))
                txtReenterPassword.Attributes["value"] = ReenterPassword;
            if (!String.IsNullOrEmpty(PasswordStatus))
                txtPasswordStatus.Value = PasswordStatus;
        }

        private void TransferSelectedListItems(ListBox lstFrom, ListBox lstTo)
        {
            ListItemCollection items = new ListItemCollection();
            foreach (int index in lstFrom.GetSelectedIndices())
            {
                items.Add(new ListItem(lstFrom.Items[index].Text, lstFrom.Items[index].Value));
            }
            foreach (ListItem item in items)
            {
                lstTo.Items.Add(item);
                lstFrom.Items.Remove(item);
            }
            items.Clear();
        }

        private void lstUnassignedGroups_Fill()
        {
            bool GetUserGroupsbyUser = true;

            try
            {
                this.lstUnassignedGroups.Items.Clear();

                StringReader strrXML = null;
                DataSet dsUnassignedGroups = new DataSet();

                string xml = "";
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                if (objUtil.ErrCheck(dbu.GetUserGroupsbyUser(sn.UserID, GetUserGroupsbyUser, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserGroupsbyUser(sn.UserID, GetUserGroupsbyUser, sn.SecId, ref xml), true))
                    {
                        ddlUserGroup.DataSource = null;
                        ddlUserGroup.DataBind();
                        return;
                    }
                if (xml == "")
                    return;

                strrXML = new StringReader(xml);
                dsUnassignedGroups.ReadXml(strrXML);

                ViewState["dsUnassignedGroups"] = dsUnassignedGroups;
                this.lstUnassignedGroups.DataSource = dsUnassignedGroups;
                this.lstUnassignedGroups.DataBind();
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

        private List<ListItem> LoadGroups(int SelectedUserID)
        {
            List<ListItem> list = new List<ListItem>();
            string xml = "";
            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();
            StringReader strrXML = null;

            if (objUtil.ErrCheck(dbu.GetAssignedGroupsForSelectedUser(sn.UserID, sn.SecId, SelectedUserID, ref xml), false))
                if (objUtil.ErrCheck(dbu.GetAssignedGroupsForSelectedUser(sn.UserID, sn.SecId, SelectedUserID, ref xml), true))
                {
                    return list;
                }

            if (xml == "")
                return list;

            DataSet ds = new DataSet();
            strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);

            DataTable dt = ds.Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (!String.IsNullOrEmpty(dr["UserGroupId"].ToString()))
                        list.Add(new ListItem(dr["UserGroupName"].ToString(), dr["UserGroupId"].ToString()));
                }
            }
            return list;
        }

        private void lstAssignedGroups_Fill()
        {
            int EditParam = 0;

            if (!String.IsNullOrEmpty(lblUserId.Text))
                EditParam = Convert.ToInt32(lblUserId.Text);

            lstAssignedGroups.Items.Clear();
            lstAssignedGroups.Items.AddRange(LoadGroups(EditParam).ToArray());
            TransferListItems(lstUnassignedGroups, lstAssignedGroups);
        }

        private void TransferListItems(ListBox lstFrom, ListBox lstTo)
        {
            ListItemCollection items = new ListItemCollection();
            foreach (ListItem itemFrom in lstFrom.Items)
            {
                foreach (ListItem itemTo in lstTo.Items)
                {
                    if (itemTo.Value == itemFrom.Value)
                        items.Add(itemTo);
                }
            }
            foreach (ListItem item in items)
            {
                lstFrom.Items.Remove(item);
            }
            items.Clear();
        }
    }
}