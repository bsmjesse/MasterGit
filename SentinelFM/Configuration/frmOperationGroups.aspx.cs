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

namespace SentinelFM
{
    public partial class Configuration_frmOperationGroups : SentinelFMBasePage
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmVehicleInfo, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    cboOperation_Fill();
                    cboOrganization_Fill();
                    cboAssignTo_Fill();
                    SetButtonsConfirm();
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

        private void cboOrganization_Fill()
        {
            DataSet dsOrganizations = new DataSet();
            StringReader strrControlXML = null;
            string xml = "";

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            if (objUtil.ErrCheck(dbu.GetOrganizationsWithUserGroups(sn.UserID, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbu.GetOrganizationsWithUserGroups(sn.UserID, sn.SecId, ref xml), true))
                {
                    this.cboOrganization.DataSource = null;
                    this.cboOrganization.DataBind();
                    return;
                }

            if (xml == "")
            {
                this.cboOrganization.DataSource = null;
                this.cboOrganization.DataBind();
                return;
            }

            strrControlXML = new StringReader(xml);
            dsOrganizations.ReadXml(strrControlXML);

            cboOrganization.DataSource = dsOrganizations;
            cboOrganization.DataBind();

            cboOrganization.Items.Insert(0, new ListItem((string)GetLocalResourceObject("SystemUserGroups"), String.Empty));
            cboOrganization.SelectedIndex = 0;
        }

        private void cboAssignTo_Fill()
        {
            cboUserGroups.Items.Add(new ListItem((string)GetLocalResourceObject("AllUserGroups"), "0"));
            cboUserGroups.Items.Add(new ListItem((string)GetLocalResourceObject("AllSecurityAdministrators"), "2"));
            cboUserGroups.Items.Add(new ListItem((string)GetLocalResourceObject("AllSecurityHigh"), "3"));
            cboUserGroups.Items.Add(new ListItem((string)GetLocalResourceObject("AllSecurityLow"), "4"));
        }

        private void SetButtonsConfirm()
        {
            string confirmAssign = "return confirm('" + (string)base.GetLocalResourceObject("ConfirmAssign") + "')";
            cmdAssign.Attributes.Add("onclick", confirmAssign);

            string confirmUnassign = "return confirm('" + (string)base.GetLocalResourceObject("ConfirmUnassign") + "')";
            cmdUnasign.Attributes.Add("onclick", confirmUnassign);
        }

        private void cboOperation_Fill()
        {
            DataSet dsOperations = new DataSet();
            StringReader strrControlXML = null;
            string xml = "";

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            if (objUtil.ErrCheck(dbu.GetOperationTypes(sn.UserID, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbu.GetOperationTypes(sn.UserID, sn.SecId, ref xml), true))
                {
                    this.cboOperation.DataSource = null;
                    this.cboOperation.DataBind();
                    return;
                }

            if (xml == "")
            {
                this.cboOperation.DataSource = null;
                this.cboOperation.DataBind();
                return;
            }

            strrControlXML = new StringReader(xml);
            dsOperations.ReadXml(strrControlXML);

            cboOperation.DataSource = dsOperations;
            cboOperation.DataBind();

            cboOperation.Items.Insert(0, new ListItem(String.Empty, String.Empty));
            cboOperation.SelectedIndex = 0;
        }

        private void cboControls_Fill(int iOperationType)
        {
            //GetOperationControls(int currUserId, string SID, int OperationType, ref string xml)
            DataSet dsOperationControls = new DataSet();
            StringReader strrControlXML = null;
            string xml = "";

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            if (objUtil.ErrCheck(dbu.GetOperationControls(sn.UserID, sn.SecId, iOperationType, ref xml), false))
                if (objUtil.ErrCheck(dbu.GetOperationControls(sn.UserID, sn.SecId, iOperationType, ref xml), true))
                {
                    this.cboControls.DataSource = null;
                    this.cboControls.DataBind();
                    return;
                }

            if (xml == "")
            {
                this.cboControls.DataSource = null;
                this.cboControls.DataBind();
                return;
            }

            strrControlXML = new StringReader(xml);
            dsOperationControls.ReadXml(strrControlXML);

            cboControls.DataSource = dsOperationControls;
            cboControls.DataBind();

            cboControls.Items.Insert(0, new ListItem(String.Empty, String.Empty));
            cboControls.SelectedIndex = 0;
        }

        private void lstUnassignedGroups_Fill()
        {
            int OrganizationID = 0;

            if (!String.IsNullOrEmpty(cboOrganization.SelectedValue))
                OrganizationID = Convert.ToInt32(cboOrganization.SelectedValue);

            try
            {
                this.lstUnassignedGroups.Items.Clear();

                StringReader strrXML = null;
                DataSet dsUnassignedGroups = new DataSet();

                string xml = "";
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                if (objUtil.ErrCheck(dbu.GetUserGroupsOperationAccess(sn.UserID, sn.SecId, Convert.ToInt32(cboControls.SelectedValue), Convert.ToInt32(cboOperation.SelectedValue), OrganizationID, false, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserGroupsOperationAccess(sn.UserID, sn.SecId, Convert.ToInt32(cboControls.SelectedValue), Convert.ToInt32(cboOperation.SelectedValue), OrganizationID, false, ref xml), true))
                    {
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

        private void lstAssignedGroups_Fill()
        {
            int OrganizationID = 0;

            if (!String.IsNullOrEmpty(cboOrganization.SelectedValue))
                OrganizationID = Convert.ToInt32(cboOrganization.SelectedValue);

            try
            {
                this.lstAssignedGroups.Items.Clear();

                StringReader strrXML = null;
                DataSet dsAssignedGroups = new DataSet();

                string xml = "";
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                if (objUtil.ErrCheck(dbu.GetUserGroupsOperationAccess(sn.UserID, sn.SecId, Convert.ToInt32(cboControls.SelectedValue), Convert.ToInt32(cboOperation.SelectedValue), OrganizationID, true, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserGroupsOperationAccess(sn.UserID, sn.SecId, Convert.ToInt32(cboControls.SelectedValue), Convert.ToInt32(cboOperation.SelectedValue), OrganizationID, true, ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                    return;

                strrXML = new StringReader(xml);
                dsAssignedGroups.ReadXml(strrXML);

                ViewState["dsAssignedGroups"] = dsAssignedGroups;
                this.lstAssignedGroups.DataSource = dsAssignedGroups;
                this.lstAssignedGroups.DataBind();
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

        }
        #endregion

        protected void cboControls_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!String.IsNullOrEmpty(cboControls.SelectedValue))
            {
                lstUnassignedGroups_Fill();
                lstAssignedGroups_Fill();
            }
            else
            {
                this.lstAssignedGroups.Items.Clear();
                this.lstUnassignedGroups.Items.Clear();
            }
            lblMsg.Text = String.Empty;
        }

        protected void cboOrganization_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!String.IsNullOrEmpty(cboControls.SelectedValue))
            {
                lstUnassignedGroups_Fill();
                lstAssignedGroups_Fill();
            }
            else
            {
                this.lstAssignedGroups.Items.Clear();
                this.lstUnassignedGroups.Items.Clear();
            }
            lblMsg.Text = String.Empty;
        }

        protected void cboOperation_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int OperationType = 0;

            lstAssignedGroups.Items.Clear();
            lstUnassignedGroups.Items.Clear();

            if (!String.IsNullOrEmpty(cboOperation.SelectedValue))
            {
                OperationType = Convert.ToInt32(cboOperation.SelectedValue);
                cboControls_Fill(OperationType);
                lblControl.Text = cboOperation.SelectedItem + ":";
            }
            else
            {
                cboControls.DataSource = null;
                cboControls.DataBind();
                lblControl.Text = String.Empty;
            }
            lblMsg.Text = String.Empty;
        }

        protected void cmdFleets_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmEmails.aspx");
        }

        protected void cmdVehicles_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmVehicleInfo.aspx");
        }

        protected void cmdUserInfo_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmUsers.aspx");
        }

        protected void cmdGroups_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmgroups.aspx");
        }

        protected void cmdAssign_Click(object sender, System.EventArgs e)
        {
            int OperationType = 0;
            int OperationId = 0;
            int ParentUserGroupId = 0;

            try
            {
                if (!String.IsNullOrEmpty(cboOperation.SelectedValue))
                    OperationType = Convert.ToInt32(cboOperation.SelectedValue);
                if (!String.IsNullOrEmpty(cboControls.SelectedValue))
                    OperationId = Convert.ToInt32(cboControls.SelectedValue);
                ParentUserGroupId = Convert.ToInt32(cboUserGroups.SelectedValue);

                if (OperationType == 0 || OperationId == 0)
                {
                    lblMsg.Text = (string)GetLocalResourceObject("EnterMandatoryFields");
                    return;
                }

                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                int updateResult = dbu.AddUserGroupSettingsAll(sn.UserID, sn.SecId, ParentUserGroupId, OperationId, OperationType);
                if (updateResult == 0)
                {
                    lstUnassignedGroups_Fill();
                    lstAssignedGroups_Fill();
                    lblMsg.Text = (string)GetLocalResourceObject("SuccessToAssign"); ;
                }
                else
                {
                    lblMsg.Text = (string)GetLocalResourceObject("ErrorToAssign");
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

        protected void cmdUnasign_Click(object sender, System.EventArgs e)
        {
            int OperationType = 0;
            int OperationId = 0;
            int ParentUserGroupId = 0;

            try
            {
                if (!String.IsNullOrEmpty(cboOperation.SelectedValue))
                    OperationType = Convert.ToInt32(cboOperation.SelectedValue);
                if (!String.IsNullOrEmpty(cboControls.SelectedValue))
                    OperationId = Convert.ToInt32(cboControls.SelectedValue);
                ParentUserGroupId = Convert.ToInt32(cboUserGroups.SelectedValue);

                if (OperationType == 0 || OperationId == 0)
                {
                    lblMsg.Text = (string)GetLocalResourceObject("EnterMandatoryFields");
                    return;
                }

                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                int updateResult = dbu.DeleteUserGroupSettingsAll(sn.UserID, sn.SecId, ParentUserGroupId, OperationId, OperationType);
                if (updateResult == 0)
                {
                    lstUnassignedGroups_Fill();
                    lstAssignedGroups_Fill();
                    lblMsg.Text = (string)GetLocalResourceObject("SuccessToUnassign");
                    //plhMsg.Visible = true;
                }
                else
                {
                    lblMsg.Text = (string)GetLocalResourceObject("ErrorToUnassign");
                    //plhMsg.Visible = true;
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

        protected void cmdAdd_Click(object sender, System.EventArgs e)
        {
            int OperationType = 0;
            int OperationId = 0;
            int UserGroupId = 0;

            try
            {
                lblMsg.Text = String.Empty;
                if (!String.IsNullOrEmpty(cboOperation.SelectedValue))
                    OperationType = Convert.ToInt32(cboOperation.SelectedValue);
                if (!String.IsNullOrEmpty(cboControls.SelectedValue))
                    OperationId = Convert.ToInt32(cboControls.SelectedValue);

                if (OperationType == 0 || OperationId == 0)
                {
                    lblMsg.Text = (string)GetLocalResourceObject("EnterMandatoryFields");
                    return;
                }

                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                foreach (ListItem li in lstUnassignedGroups.Items)
                {
                    if (li.Selected)
                    {
                        UserGroupId = Convert.ToInt32(li.Value);
                        int updateResult = dbu.AddUserGroupSetting(sn.UserID, sn.SecId, UserGroupId, OperationId, OperationType);
                        if (updateResult == 0)
                        {
                            lstUnassignedGroups_Fill();
                            lstAssignedGroups_Fill();
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

        protected void cmdAddAll_Click(object sender, System.EventArgs e)
        {
            int OperationType = 0;
            int OperationId = 0;
            int UserGroupId = 0;

            try
            {
                lblMsg.Text = String.Empty;
                if (!String.IsNullOrEmpty(cboOperation.SelectedValue))
                    OperationType = Convert.ToInt32(cboOperation.SelectedValue);
                if (!String.IsNullOrEmpty(cboControls.SelectedValue))
                    OperationId = Convert.ToInt32(cboControls.SelectedValue);

                if (OperationType == 0 || OperationId == 0)
                {
                    lblMsg.Text = (string)GetLocalResourceObject("EnterMandatoryFields");
                    return;
                }

                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                DataSet dsUnAssUsers = new DataSet();
                dsUnAssUsers = (DataSet)ViewState["dsUnassignedGroups"];

                foreach (DataRow rowItem in dsUnAssUsers.Tables[0].Rows)
                {
                    UserGroupId = Convert.ToInt32(rowItem["UserGroupId"]);
                    int updateResult = dbu.AddUserGroupSetting(sn.UserID, sn.SecId, UserGroupId, OperationId, OperationType);
                }

                lstUnassignedGroups_Fill();
                lstAssignedGroups_Fill();
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

        protected void cmdRemove_Click(object sender, System.EventArgs e)
        {
            int OperationType = 0;
            int OperationId = 0;
            int UserGroupId = 0;

            try
            {
                lblMsg.Text = String.Empty;
                if (!String.IsNullOrEmpty(cboOperation.SelectedValue))
                    OperationType = Convert.ToInt32(cboOperation.SelectedValue);
                if (!String.IsNullOrEmpty(cboControls.SelectedValue))
                    OperationId = Convert.ToInt32(cboControls.SelectedValue);

                if (OperationType == 0 || OperationId == 0)
                {
                    lblMsg.Text = (string)GetLocalResourceObject("EnterMandatoryFields");
                    return;
                }

                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                foreach (ListItem li in lstAssignedGroups.Items)
                {
                    if (li.Selected)
                    {
                        UserGroupId = Convert.ToInt32(li.Value);
                        int updateResult = dbu.DeleteUserGroupSetting(sn.UserID, sn.SecId, UserGroupId, OperationId, OperationType);
                        if (updateResult == 0)
                        {
                            lstUnassignedGroups_Fill();
                            lstAssignedGroups_Fill();
                        }
                    }
                }

                lstUnassignedGroups_Fill();
                lstAssignedGroups_Fill();
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

        protected void cmdRemoveAll_Click(object sender, System.EventArgs e)
        {
            int OperationType = 0;
            int OperationId = 0;
            int UserGroupId = 0;

            try
            {
                lblMsg.Text = String.Empty;
                if (!String.IsNullOrEmpty(cboOperation.SelectedValue))
                    OperationType = Convert.ToInt32(cboOperation.SelectedValue);
                if (!String.IsNullOrEmpty(cboControls.SelectedValue))
                    OperationId = Convert.ToInt32(cboControls.SelectedValue);

                if (OperationType == 0 || OperationId == 0)
                {
                    lblMsg.Text = (string)GetLocalResourceObject("EnterMandatoryFields");
                    return;
                }
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                DataSet dsAssVehicle = new DataSet();
                dsAssVehicle = (DataSet)ViewState["dsAssignedGroups"];

                foreach (DataRow rowItem in dsAssVehicle.Tables[0].Rows)
                {
                    UserGroupId = Convert.ToInt32(rowItem["UserGroupId"]);
                    int updateResult = dbu.DeleteUserGroupSetting(sn.UserID, sn.SecId, UserGroupId, OperationId, OperationType);
                }

                lstUnassignedGroups_Fill();
                lstAssignedGroups_Fill();
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

        protected void chkMultipleUserGroups_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMultipleUserGroups.Checked)
            {
                plhMultipleUserGroups.Visible = true;
                plhListboxes.Visible = false;
                plhOrganization.Visible = false;
            }
            else
            {
                plhMultipleUserGroups.Visible = false;
                plhListboxes.Visible = true;
                plhOrganization.Visible = true;
            }
            lblMsg.Text = String.Empty;
        }

        protected void cmdDriver_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmdrivers.aspx");
        }
        protected void cmdUserDashBoards_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmuserdashboards.aspx");
        }

        protected void cmdScheduledTasks_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmTaskScheduler.aspx");
        }

        protected void cmdGroupConfiguration_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmOperationGroups.aspx");
        }

        protected void cmdUserGroups_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmUserGroups.aspx");
        }

        protected void cmdControls_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmControls.aspx");
        }

        protected void cmdServices_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmServices.aspx");
        }

}
}