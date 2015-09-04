using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace SentinelFM
{
    public partial class Configuration_frmGroupControls : SentinelFMBasePage
    {
        #region Page Variables

        public string sRefreshParent = "true";
        protected string sConnectionString;
        protected bool MultipleHierarchySetting = false;
        //protected bool OrganizationHasHierarchy = false;
        VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;

        public int UserGroupId
        {
            get
            {
                if (ViewState["UserGroupId"] != null)
                    return Convert.ToInt32(ViewState["UserGroupId"]);
                else
                    return 0;
            }
            set
            {
                ViewState["UserGroupId"] = value;
            }
        }

        public string UserGroupName
        {
            get
            {
                if (ViewState["UserGroupName"] != null)
                    return ViewState["UserGroupName"].ToString();
                else
                    return String.Empty;
            }
            set
            {
                ViewState["UserGroupName"] = value;
            }
        }

        public int ParentUserGroupId
        {
            get
            {
                if (ViewState["ParentUserGroupId"] != null)
                    return Convert.ToInt32(ViewState["ParentUserGroupId"]);
                else
                    return 0;
            }
            set
            {
                ViewState["ParentUserGroupId"] = value;
            }
        }

        public bool CheckboxesEnabled
        {
            get
            {
                if (ViewState["CheckboxesEnabled"] != null)
                    return Convert.ToBoolean(ViewState["CheckboxesEnabled"]);
                else
                    return false;
            }
            set
            {
                ViewState["CheckboxesEnabled"] = value;
            }
        }

        public string SelectedControls
        {
            get
            {
                if (ViewState["SelectedControls"] != null)
                    return ViewState["SelectedControls"].ToString();
                else
                    return String.Empty;
            }
            set
            {
                ViewState["SelectedControls"] = value;
            }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            string FleetNodeCodes = String.Empty;

            MultipleHierarchySetting = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

            //sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            //poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
            //OrganizationHasHierarchy = poh.HasOrganizationHierarchy(sn.User.OrganizationId);

            if (!IsPostBack)
            {
                if (Request.QueryString["GId"] != null)
                {
                    string sUserGroupId = Request.QueryString["GId"].ToString();
                    if (IsNaturalNumber(sUserGroupId))
                        UserGroupId = Convert.ToInt32(sUserGroupId);
                    CheckboxesEnabled = true;
                }
                else if (Request.QueryString["GrId"] != null)
                {
                    string sUserGroupId = Request.QueryString["GrId"].ToString();
                    if (IsNaturalNumber(sUserGroupId))
                        UserGroupId = Convert.ToInt32(sUserGroupId);
                    cmdUpdate.Visible = false;
                    CheckboxesEnabled = false;
                    sRefreshParent = "false";
                    plhGroupName.Visible = false;
                }

                sn.Misc.ConfDsUserGroupControlSettings = null;
                sn.Misc.ConfDsUserGroupReportSettings = null;
                sn.Misc.ConfDsUserGroupCommandSettings = null;
                SetDropdowns();

                //if (UserGroupId > 0 && CheckboxesEnabled && sn.UserName.ToLower().Contains("hgi_"))
                //{
                //    plhUserGroupControls.Visible = true;
                //    plhUserGroupReports.Visible = true;
                //    plhUserGroupCommands.Visible = true;
                //    LoadControlAddSettings();
                //    LoadReportAddSettings();
                //    LoadCommandAddSettings();
                //}

                cmdAdd.Enabled = CheckboxesEnabled;
                cmdRemove.Enabled = CheckboxesEnabled;

                //if (OrganizationHasHierarchy)
                //{
                //    organizationHierarchy.Visible = true;

                //    if (UserGroupId > 0)
                //    {
                //        FleetNodeCodes = SetFleetsByUserGroup();
                //        if (!String.IsNullOrEmpty(FleetNodeCodes))
                //        {
                //            HierarchyTree1.Field_OrganizationHierarchyNodeCode = SetFleetsByUserGroup();
                //        }
                //    }
                //}

                DefaultFleet.Visible = false;

                //lstUnassignedFleets_Fill();
                //lstAssignedFleets_Fill();
            }

            if (UserGroupId > 0)
            {
                plhAddNewGroup.Visible = false;
                cmdUpdate.Text = (string)GetLocalResourceObject("Update");        
            }
            else
            {
                plhAddNewGroup.Visible = true;
                lblUserGroupName.Text = (string)GetLocalResourceObject("AddNewGroupSettings");
                cmdUpdate.Text = (string)GetLocalResourceObject("Add");
            }

            if (UserGroupId > 0 || ParentUserGroupId > 0)
            {
                DisplayUserGroupSettings();
            }
        }

        #endregion

        #region Events

        protected void cmdAdd_Click(object sender, System.EventArgs e)
        {
            if (lstUnassignedFleets.SelectedIndex > -1)
            {
                TransferSelectedListItems(lstUnassignedFleets, lstAssignedFleets);
            }
        }

        protected void cmdRemove_Click(object sender, System.EventArgs e)
        {
            if (lstAssignedFleets.SelectedIndex > -1)
            {
                TransferSelectedListItems(lstAssignedFleets, lstUnassignedFleets);
            }
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            string sCheckboxValue = String.Empty;
            StringBuilder _sControlsCheckboxValues = new StringBuilder();
            StringBuilder _sReportsCheckboxValues = new StringBuilder();
            StringBuilder _sCommandsCheckboxValues = new StringBuilder();
            StringBuilder _sFleetIDs = new StringBuilder();
            string sCheckboxControlsValuesParams = String.Empty;
            string sCheckboxReportsValuesParams = String.Empty;
            string sCheckboxCommandsValuesParams = String.Empty;
            string sFleetIDs = String.Empty;
            DataSet dsUserGroupControlSettings = new DataSet();
            DataSet dsUserGroupReportSettings = new DataSet();
            DataSet dsUserGroupCommandSettings = new DataSet();
            int ControlId = 0;
            int OperationType = 3; //gui controls
            int OrganizationId = 0;
            int iParentUserGroupId = 0;
            int ExistingUserGroupId = 0;

            try
            {
                OrganizationId = sn.User.OrganizationId;
                dsUserGroupControlSettings = GetUserGroupControlSettings();
                dsUserGroupReportSettings = GetUserGroupReportSettings();
                dsUserGroupCommandSettings = GetUserGroupCommandSettings();

                //get all checkbox values for controls
                if (dsUserGroupControlSettings != null && dsUserGroupControlSettings.Tables.Count > 0)
                {
                    foreach (DataRow drUserGroupSettings in dsUserGroupControlSettings.Tables[0].Rows)
                    {
                        ControlId = Convert.ToInt32(drUserGroupSettings["ControlId"]);
                        sCheckboxValue = getCheckboxInputValue(ControlId, "UserGroupControl", pnlUserGroupControlSettings);
                        if (!String.IsNullOrEmpty(sCheckboxValue))
                        {
                            _sControlsCheckboxValues.Append(sCheckboxValue);
                            _sControlsCheckboxValues.Append(";");
                        }
                    }

                    if (_sControlsCheckboxValues.Length > 0)
                        sCheckboxControlsValuesParams = _sControlsCheckboxValues.ToString().Substring(0, _sControlsCheckboxValues.Length - 1);
                }

                //get all checkbox values for reports
                if (dsUserGroupReportSettings != null && dsUserGroupReportSettings.Tables.Count > 0)
                {
                    foreach (DataRow drUserGroupReportSettings in dsUserGroupReportSettings.Tables[0].Rows)
                    {
                        ControlId = Convert.ToInt32(drUserGroupReportSettings["ReportTypesId"]);
                        sCheckboxValue = getCheckboxInputValue(ControlId, "UserGroupReport", pnlUserGroupReportSettings);
                        if (!String.IsNullOrEmpty(sCheckboxValue))
                        {
                            _sReportsCheckboxValues.Append(sCheckboxValue);
                            _sReportsCheckboxValues.Append(";");
                        }
                    }

                    if (_sReportsCheckboxValues.Length > 0)
                        sCheckboxReportsValuesParams = _sReportsCheckboxValues.ToString().Substring(0, _sReportsCheckboxValues.Length - 1);
                }

                //get all checkbox values for commands
                if (dsUserGroupCommandSettings != null && dsUserGroupCommandSettings.Tables.Count > 0)
                {
                    foreach (DataRow drUserGroupCommandSettings in dsUserGroupCommandSettings.Tables[0].Rows)
                    {
                        ControlId = Convert.ToInt32(drUserGroupCommandSettings["BoxCmdOutTypeId"]);
                        sCheckboxValue = getCheckboxInputValue(ControlId, "UserGroupCommand", pnlUserGroupCommandSettings);
                        if (!String.IsNullOrEmpty(sCheckboxValue))
                        {
                            _sCommandsCheckboxValues.Append(sCheckboxValue);
                            _sCommandsCheckboxValues.Append(";");
                        }
                    }

                    if (_sCommandsCheckboxValues.Length > 0)
                        sCheckboxCommandsValuesParams = _sCommandsCheckboxValues.ToString().Substring(0, _sCommandsCheckboxValues.Length - 1);
                }

                UserGroupName = txtGroupName.Text.Trim();

                //get FleetIDs
                //if (organizationHierarchy.Visible)
                //{
                //    sFleetIDs = HierarchyTree1.Field_OrganizationHierarchyFleetId.Replace(",", ";") + ";";
                //}

                //foreach (ListItem item in lstAssignedFleets.Items)
                //{
                //    sFleetIDs = sFleetIDs + item.Value + ";";
                //}

                //if (sFleetIDs.Length > 0)
                //    sFleetIDs = sFleetIDs.Substring(0, sFleetIDs.Length - 1);  

                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();
                if (UserGroupId == 0)
                {
                    ExistingUserGroupId = dbu.GetUserGroupIdByName(sn.UserID, sn.SecId, UserGroupName);
                    if (ExistingUserGroupId > 0)
                    {
                        lblMsg.Text = String.Format((string)GetLocalResourceObject("GroupAlreadyExistsMsg"), UserGroupName);
                        plhMsg.Visible = true;
                        return;
                    }

                    if (!String.IsNullOrEmpty(cboInheritFromGroup.SelectedValue))
                        iParentUserGroupId = Convert.ToInt32(cboInheritFromGroup.SelectedValue);
                    if (iParentUserGroupId > 0 && !String.IsNullOrEmpty(UserGroupName) && (!String.IsNullOrEmpty(sCheckboxControlsValuesParams) || !String.IsNullOrEmpty(sCheckboxReportsValuesParams)))
                    {
                        UserGroupId = dbu.AddUserGroupSettings(sn.UserID, sn.SecId, sCheckboxControlsValuesParams, sCheckboxReportsValuesParams,
                             sCheckboxCommandsValuesParams, sFleetIDs, OperationType, UserGroupName, OrganizationId, iParentUserGroupId);

                        if (UserGroupId > 0)
                        {
                            sn.Misc.ConfDsUserGroupControlSettings = null;
                            sn.Misc.ConfDsUserGroupReportSettings = null;
                            sn.Misc.ConfDsUserGroupCommandSettings = null;

                            lblMsg.Text = String.Format((string)GetLocalResourceObject("NewGroupCreatedMsg"), UserGroupName);
                            plhMsg.Visible = true;
                            ParentUserGroupId = 0;
                            if (CheckboxesEnabled)
                                lblUserGroupName.Text = String.Format((string)GetLocalResourceObject("UpdateGroupSettings"), UserGroupName);
                            else
                                lblUserGroupName.Text = String.Format((string)GetLocalResourceObject("GroupSettings"), UserGroupName);
                            cboInheritFromGroup.SelectedValue = String.Empty;
                            plhAddNewGroup.Visible = false;
                            cmdUpdate.Text = "Update";
                        }
                        else
                        {
                            lblMsg.Text = String.Format((string)GetLocalResourceObject("ErrorToAddGroupMsg"), UserGroupName);
                            plhMsg.Visible = true;
                        }
                    }
                    else
                    {
                        lblMsg.Text = String.Format((string)GetLocalResourceObject("EnterMandatoryFieldsMsg"), UserGroupName);
                        plhMsg.Visible = true;
                    }
                }
                else
                {
                    int updateResult = dbu.UpdateUserGroupSettings(sn.UserID, sn.SecId, UserGroupId, sCheckboxControlsValuesParams,
                         sCheckboxReportsValuesParams, sCheckboxCommandsValuesParams, sFleetIDs, OperationType, UserGroupName);
                    if (updateResult == 0)
                    {
                        sn.Misc.ConfDsUserGroupControlSettings = null;
                        sn.Misc.ConfDsUserGroupReportSettings = null;
                        sn.Misc.ConfDsUserGroupCommandSettings = null;

                        lblMsg.Text = String.Format((string)GetLocalResourceObject("GroupUpdatedMsg"), UserGroupName);
                        plhMsg.Visible = true;
                    }
                    else
                    {
                        lblMsg.Text = String.Format((string)GetLocalResourceObject("ErrorToUpdateGroupMsg"), UserGroupName);
                        plhMsg.Visible = true;
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

        protected void cboInheritFromGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            sn.Misc.ConfDsUserGroupControlSettings = null;
            sn.Misc.ConfDsUserGroupReportSettings = null;
            sn.Misc.ConfDsUserGroupCommandSettings = null;

            if (!String.IsNullOrEmpty(cboInheritFromGroup.SelectedValue))
            {
                ParentUserGroupId = Convert.ToInt32(cboInheritFromGroup.SelectedValue);
                DisplayUserGroupSettings();
            }
            else
            {
                pnlUserGroupControlSettings.Controls.Clear();
                pnlUserGroupReportSettings.Controls.Clear();
                pnlUserGroupCommandSettings.Controls.Clear();
                ParentUserGroupId = 0;
            }
        }

        protected void btnUserGroupControls_Click(object sender, ImageClickEventArgs e)
        {
            int OperationId = 0;
            int OperationType = 3; //gui controls

            if (!String.IsNullOrEmpty(cboUserGroupControls.SelectedValue))
            {
                OperationId = Convert.ToInt32(cboUserGroupControls.SelectedValue);

                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();
                int updateResult = dbu.AddUserGroupSetting(sn.UserID, sn.SecId, UserGroupId, OperationId, OperationType);

                if (updateResult == 0)
                {
                    sn.Misc.ConfDsUserGroupControlSettings = null;
                    LoadControlAddSettings();
                    DisplayUserGroupSettings();
                }
            }
        }

        protected void UserGroupReports_Click(object sender, ImageClickEventArgs e)
        {
            int OperationId = 0;
            int OperationType = 4; //reports

            if (!String.IsNullOrEmpty(cboUserGroupReports.SelectedValue))
            {
                OperationId = Convert.ToInt32(cboUserGroupReports.SelectedValue);

                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();
                int updateResult = dbu.AddUserGroupSetting(sn.UserID, sn.SecId, UserGroupId, OperationId, OperationType);

                if (updateResult == 0)
                {
                    sn.Misc.ConfDsUserGroupReportSettings = null;
                    LoadReportAddSettings();
                    DisplayUserGroupSettings();
                }
            }
        }

        protected void btnUserGroupCommands_Click(object sender, ImageClickEventArgs e)
        {
            int OperationId = 0;
            int OperationType = 2; //commands

            if (!String.IsNullOrEmpty(cboUserGroupCommands.SelectedValue))
            {
                OperationId = Convert.ToInt32(cboUserGroupCommands.SelectedValue);

                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();
                int updateResult = dbu.AddUserGroupSetting(sn.UserID, sn.SecId, UserGroupId, OperationId, OperationType);

                if (updateResult == 0)
                {
                    sn.Misc.ConfDsUserGroupCommandSettings = null;
                    LoadCommandAddSettings();
                    DisplayUserGroupSettings();
                }

            }
        }

        #endregion

        #region Methods

        private void lstUnassignedFleets_Fill()
        {
            try
            {
                this.lstUnassignedFleets.Items.Clear();

                StringReader strrXML = null;
                DataSet dsUnassignedFleets = new DataSet();

                string xml = "";

                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                if (objUtil.ErrCheck(dbo.GetFleetsInfoByOrganizationIdAndType(sn.UserID, sn.SecId, sn.User.OrganizationId, "", ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetFleetsInfoByOrganizationIdAndType(sn.UserID, sn.SecId, sn.User.OrganizationId, "", ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                    return;

                strrXML = new StringReader(xml);
                dsUnassignedFleets.ReadXml(strrXML);

                this.lstUnassignedFleets.DataSource = dsUnassignedFleets;
                this.lstUnassignedFleets.DataBind();
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

        private void lstAssignedFleets_Fill()
        {
            lstAssignedFleets.Items.Clear();
            lstAssignedFleets.Items.AddRange(LoadFleets(UserGroupId).ToArray());
            TransferListItems(lstUnassignedFleets, lstAssignedFleets);
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

        private List<ListItem> LoadFleets(int SelectedUserGroupID)
        {
            List<ListItem> list = new List<ListItem>();
            string xml = "";
            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();
            StringReader strrXML = null;

            if (objUtil.ErrCheck(dbu.GetFleetsByUserGroup(sn.UserID, sn.SecId, SelectedUserGroupID, "", ref xml), false))
                if (objUtil.ErrCheck(dbu.GetFleetsByUserGroup(sn.UserID, sn.SecId, SelectedUserGroupID, "", ref xml), true))
                {
                    //can't load dataset
                    return list;
                }

            if (xml == "")
            {
                //can't load dataset
                return list;
            }

            DataSet ds = new DataSet();
            strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);

            DataTable dt = ds.Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (!String.IsNullOrEmpty(dr["FleetId"].ToString()))
                        list.Add(new ListItem(dr["FleetName"].ToString(), dr["FleetId"].ToString()));
                }
            }
            return list;
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

        private void LoadControlAddSettings()
        {
            DataSet dsUserGroupControlAddSettings = new DataSet();
            StringReader strrControlXML = null;
            string xml = "";

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            if (objUtil.ErrCheck(dbu.GetUserGroupControlAddSettings(sn.UserID, UserGroupId, ParentUserGroupId, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbu.GetUserGroupControlAddSettings(sn.UserID, UserGroupId, ParentUserGroupId, sn.SecId, ref xml), true))
                {
                    this.cboUserGroupControls.DataSource = null;
                    this.cboUserGroupControls.DataBind();
                    return;
                }

            if (xml == "")
            {
                this.cboUserGroupControls.DataSource = null;
                this.cboUserGroupControls.DataBind();
                return;
            }

            strrControlXML = new StringReader(xml);
            dsUserGroupControlAddSettings.ReadXml(strrControlXML);

            cboUserGroupControls.DataSource = dsUserGroupControlAddSettings;
            cboUserGroupControls.DataBind();

            cboUserGroupControls.Items.Insert(0, new ListItem(String.Empty, String.Empty));
            cboUserGroupControls.SelectedIndex = 0;
        }

        private void LoadReportAddSettings()
        {
            DataSet dsUserGroupReportAddSettings = new DataSet();
            StringReader strrReportXML = null;
            string xml = "";

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            if (objUtil.ErrCheck(dbu.GetUserGroupReportAddSettings(sn.UserID, UserGroupId, ParentUserGroupId, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbu.GetUserGroupReportAddSettings(sn.UserID, UserGroupId, ParentUserGroupId, sn.SecId, ref xml), true))
                {
                    this.cboUserGroupReports.DataSource = null;
                    this.cboUserGroupReports.DataBind();
                    return;
                }

            if (xml == "")
            {
                this.cboUserGroupReports.DataSource = null;
                this.cboUserGroupReports.DataBind();
                return;
            }

            strrReportXML = new StringReader(xml);
            dsUserGroupReportAddSettings.ReadXml(strrReportXML);

            cboUserGroupReports.DataSource = dsUserGroupReportAddSettings;
            cboUserGroupReports.DataBind();

            cboUserGroupReports.Items.Insert(0, new ListItem(String.Empty, String.Empty));
            cboUserGroupReports.SelectedIndex = 0;
        }

        private void LoadCommandAddSettings()
        {
            DataSet dsUserGroupCommandAddSettings = new DataSet();
            StringReader strrCommandXML = null;
            string xml = "";

            cboUserGroupCommands.Items.Clear();

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            if (objUtil.ErrCheck(dbu.GetUserGroupCommandAddSettings(sn.UserID, UserGroupId, ParentUserGroupId, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbu.GetUserGroupCommandAddSettings(sn.UserID, UserGroupId, ParentUserGroupId, sn.SecId, ref xml), true))
                {
                    this.cboUserGroupCommands.DataSource = null;
                    this.cboUserGroupCommands.DataBind();
                    return;
                }

            if (xml == "")
            {
                this.cboUserGroupCommands.DataSource = null;
                this.cboUserGroupCommands.DataBind();
                return;
            }

            strrCommandXML = new StringReader(xml);
            dsUserGroupCommandAddSettings.ReadXml(strrCommandXML);

            cboUserGroupCommands.DataSource = dsUserGroupCommandAddSettings;
            cboUserGroupCommands.DataBind();

            cboUserGroupCommands.Items.Insert(0, new ListItem(String.Empty, String.Empty));
            cboUserGroupCommands.SelectedIndex = 0;
        }

        private void SetDropdowns()
        {
            DataSet dsGroups = new DataSet();
            StringReader strrXML = null;
            bool GetUserGroupsbyUser = true; //false - only system groups, true - plus organizatin groups 

            string xml = "";

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            if (objUtil.ErrCheck(dbu.GetUserGroupsbyUser(sn.UserID, GetUserGroupsbyUser, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbu.GetUserGroupsbyUser(sn.UserID, GetUserGroupsbyUser, sn.SecId, ref xml), true))
                {
                    this.cboInheritFromGroup.DataSource = null;
                    this.cboInheritFromGroup.DataBind();
                    return;
                }

            if (xml == "")
            {
                this.cboInheritFromGroup.DataSource = null;
                this.cboInheritFromGroup.DataBind();
                return;
            }

            strrXML = new StringReader(xml);
            dsGroups.ReadXml(strrXML);

            cboInheritFromGroup.DataSource = dsGroups;
            cboInheritFromGroup.DataBind();

            cboInheritFromGroup.Items.Insert(0, new ListItem(String.Empty, String.Empty));
            cboInheritFromGroup.SelectedIndex = 0;
        }

        private void DisplayUserGroupSettings()
        {
            DataSet dsUserGroupControlSettings = new DataSet();
            DataSet dsUserGroupReportSettings = new DataSet();
            DataSet dsUserGroupCommandSettings = new DataSet();
            int ControlId = 0;
            bool SelectedControl = false;
            int FormID = 0;
            int OldFormID = -1;
            int ReportGroupID = 0;
            int OldReportGroupID = 0;
            int CommandGroupID = 0;
            int OldCommandGroupID = 0;
            string FormName = String.Empty;
            string ControlDescription = String.Empty;
            string ChildControlIDs = String.Empty;
            string ParentControlIDs = String.Empty;
            string ReportGroupName = String.Empty;
            string CommandGroupName = String.Empty;

            try
            {
                dsUserGroupControlSettings = GetUserGroupControlSettings();
                dsUserGroupReportSettings = GetUserGroupReportSettings();
                dsUserGroupCommandSettings = GetUserGroupCommandSettings();

                pnlUserGroupControlSettings.Controls.Clear();
                pnlUserGroupReportSettings.Controls.Clear();
                pnlUserGroupCommandSettings.Controls.Clear();

                if (dsUserGroupControlSettings.Tables[0].Rows.Count > 0 && UserGroupId > 0)
                {
                    UserGroupName = dsUserGroupControlSettings.Tables[0].Rows[0]["UserGroupName"].ToString();
                    if (CheckboxesEnabled)
                    {
                        lblUserGroupName.Text = String.Format((string)GetLocalResourceObject("UpdateGroupSettings"), UserGroupName);
                        if (String.IsNullOrEmpty(txtGroupName.Text.Trim()))
                            txtGroupName.Text = UserGroupName;
                    }
                    else
                        lblUserGroupName.Text = String.Format((string)GetLocalResourceObject("GroupSettings"), UserGroupName);
                }
                else if (dsUserGroupReportSettings.Tables[0].Rows.Count > 0 && UserGroupId > 0)
                {
                    UserGroupName = dsUserGroupReportSettings.Tables[0].Rows[0]["UserGroupName"].ToString();
                    if (CheckboxesEnabled)
                    {
                        lblUserGroupName.Text = String.Format((string)GetLocalResourceObject("UpdateGroupSettings"), UserGroupName);
                        if (String.IsNullOrEmpty(txtGroupName.Text.Trim()))
                            txtGroupName.Text = UserGroupName;
                    }
                    else
                        lblUserGroupName.Text = String.Format((string)GetLocalResourceObject("GroupSettings"), UserGroupName);
                }
                else if (dsUserGroupCommandSettings.Tables[0].Rows.Count > 0 && UserGroupId > 0)
                {
                    UserGroupName = dsUserGroupCommandSettings.Tables[0].Rows[0]["UserGroupName"].ToString();
                    if (CheckboxesEnabled)
                    {
                        lblUserGroupName.Text = String.Format((string)GetLocalResourceObject("UpdateGroupSettings"), UserGroupName);
                        if (String.IsNullOrEmpty(txtGroupName.Text.Trim()))
                            txtGroupName.Text = UserGroupName;
                    }
                    else
                        lblUserGroupName.Text = String.Format((string)GetLocalResourceObject("GroupSettings"), UserGroupName);
                }

                //control settings
                if (dsUserGroupControlSettings != null && dsUserGroupControlSettings.Tables.Count > 0)
                {
                    foreach (DataRow drUserGroupControlSettings in dsUserGroupControlSettings.Tables[0].Rows)
                    {
                        ControlId = Convert.ToInt32(drUserGroupControlSettings["ControlId"]);
                        SelectedControl = Convert.ToBoolean(drUserGroupControlSettings["SelectedControl"]);
                        ChildControlIDs = drUserGroupControlSettings["ChildControlIDs"].ToString();
                        ParentControlIDs = drUserGroupControlSettings["ParentControlIDs"].ToString();

                        if (drUserGroupControlSettings["FormID"] != DBNull.Value)
                        {
                            FormID = Convert.ToInt32(drUserGroupControlSettings["FormID"]);
                            FormName = drUserGroupControlSettings["FormName"].ToString();
                            if (drUserGroupControlSettings["ControlDescription"] != DBNull.Value)
                                ControlDescription = drUserGroupControlSettings["ControlDescription"].ToString();
                            //else
                            //    ControlDescription = "???";

                            if (FormID != OldFormID)
                                addFormName("lblControlGroupName" + FormID.ToString(), FormName, pnlUserGroupControlSettings);

                            addUserGroupSetting(ControlId, ControlDescription, SelectedControl, "UserGroupControl", pnlUserGroupControlSettings, ChildControlIDs, ParentControlIDs);

                            OldFormID = FormID;
                        }
                    }
                }

                //report settings
                if (dsUserGroupReportSettings != null && dsUserGroupReportSettings.Tables.Count > 0)
                {
                    foreach (DataRow drUserGroupReportSettings in dsUserGroupReportSettings.Tables[0].Rows)
                    {
                        ControlId = Convert.ToInt32(drUserGroupReportSettings["ReportTypesId"]);
                        SelectedControl = Convert.ToBoolean(drUserGroupReportSettings["SelectedReport"]);

                        if (drUserGroupReportSettings["ReportGroupID"] != DBNull.Value)
                        {
                            ReportGroupID = Convert.ToInt32(drUserGroupReportSettings["ReportGroupID"]);
                            ReportGroupName = drUserGroupReportSettings["ReportGroupName"].ToString();
                        }

                        if (ReportGroupID != OldReportGroupID)
                            addFormName("lblReportGroupName" + ReportGroupID.ToString(), ReportGroupName, pnlUserGroupReportSettings);

                        if (drUserGroupReportSettings["ReportName"] != DBNull.Value)
                            ControlDescription = drUserGroupReportSettings["ReportName"].ToString();

                        //if (drUserGroupReportSettings["WebReport"] != DBNull.Value)
                        //    if (!Convert.ToBoolean(drUserGroupReportSettings["WebReport"]))
                        //        ControlDescription = ControlDescription + "<b><sup>1</sup></b>";

                        if (drUserGroupReportSettings["ExtendedReport"] != DBNull.Value)
                            if (Convert.ToBoolean(drUserGroupReportSettings["ExtendedReport"]))
                                ControlDescription = ControlDescription + "<b><sup>*</sup></b>";

                        addUserGroupSetting(ControlId, ControlDescription, SelectedControl, "UserGroupReport", pnlUserGroupReportSettings, String.Empty, String.Empty);

                        OldReportGroupID = ReportGroupID;
                    }
                }

                //command settings
                if (dsUserGroupCommandSettings != null && dsUserGroupCommandSettings.Tables.Count > 0)
                {
                    foreach (DataRow drUserGroupCommandSettings in dsUserGroupCommandSettings.Tables[0].Rows)
                    {
                        ControlId = Convert.ToInt32(drUserGroupCommandSettings["BoxCmdOutTypeId"]);
                        SelectedControl = Convert.ToBoolean(drUserGroupCommandSettings["SelectedCommand"]);

                        if (drUserGroupCommandSettings["CommandGroupID"] != DBNull.Value)
                        {
                            CommandGroupID = Convert.ToInt32(drUserGroupCommandSettings["CommandGroupID"]);
                            CommandGroupName = drUserGroupCommandSettings["CommandGroupName"].ToString();
                        }

                        if (CommandGroupID != OldCommandGroupID)
                            addFormName("lblCommandGroupName" + CommandGroupID.ToString(), CommandGroupName, pnlUserGroupCommandSettings);

                        if (drUserGroupCommandSettings["BoxCmdOutType"] != DBNull.Value)
                            ControlDescription = drUserGroupCommandSettings["BoxCmdOutType"].ToString();

                        addUserGroupSetting(ControlId, ControlDescription, SelectedControl, "UserGroupCommand", pnlUserGroupCommandSettings, String.Empty, String.Empty);
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

        private string SetFleetsByUserGroup()
        {
            DataSet dsFleets = new DataSet();
            StringReader strrXML = null;
            string xml = "";
            string FleetNodeCode = String.Empty;
            StringBuilder _sFleetNodeCodes = new StringBuilder();
            string sFleetNodeCodes = String.Empty;

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            if (objUtil.ErrCheck(dbu.GetFleetsByUserGroup(sn.UserID, sn.SecId, UserGroupId, "oh", ref xml), false))
                if (objUtil.ErrCheck(dbu.GetFleetsByUserGroup(sn.UserID, sn.SecId, UserGroupId, "oh", ref xml), true))
                {
                    //can't load dataset
                    return sFleetNodeCodes;
                }

            if (xml == "")
            {
                //can't load dataset
                return sFleetNodeCodes;
            }

            strrXML = new StringReader(xml);
            dsFleets.ReadXml(strrXML);

            foreach (DataRow drFleets in dsFleets.Tables[0].Rows)
            {
                FleetNodeCode = drFleets["NodeCode"].ToString();

                if (!String.IsNullOrEmpty(FleetNodeCode))
                {
                    _sFleetNodeCodes.Append(FleetNodeCode);
                    _sFleetNodeCodes.Append(",");
                }
            }

            if (_sFleetNodeCodes.Length > 0)
                sFleetNodeCodes = _sFleetNodeCodes.ToString().Substring(0, _sFleetNodeCodes.Length - 1);

            return sFleetNodeCodes;
        }

        private DataSet GetUserGroupControlSettings()
        {
            DataSet dsUserGroupSettings = new DataSet();
            StringReader strrXML = null;
            string xml = "";
            int iUserGroupId = 0;

            if (UserGroupId > 0)
            {
                iUserGroupId = UserGroupId;
                ParentUserGroupId = 0;
            }
            else
                iUserGroupId = ParentUserGroupId;

            if (sn.Misc.ConfDsUserGroupControlSettings == null || sn.Misc.ConfDsUserGroupControlSettings.Tables.Count == 0)
            {
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                if (objUtil.ErrCheck(dbu.GetUserGroupControlSettings(sn.UserID, iUserGroupId, ParentUserGroupId, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserGroupControlSettings(sn.UserID, iUserGroupId, ParentUserGroupId, sn.SecId, ref xml), true))
                    {
                        //can't load page
                        return dsUserGroupSettings;
                    }

                if (xml == "")
                {
                    //can't load page
                    return dsUserGroupSettings;
                }

                strrXML = new StringReader(xml);
                dsUserGroupSettings.ReadXml(strrXML);

                sn.Misc.ConfDsUserGroupControlSettings = dsUserGroupSettings;
            }
            else
            {
                dsUserGroupSettings = sn.Misc.ConfDsUserGroupControlSettings;
            }

            return dsUserGroupSettings;
        }

        private DataSet GetUserGroupReportSettings()
        {
            DataSet dsUserGroupSettings = new DataSet();
            StringReader strrXML = null;
            string xml = "";
            int iUserGroupId = 0;

            if (UserGroupId > 0)
            {
                iUserGroupId = UserGroupId;
                ParentUserGroupId = 0;
            }
            else
                iUserGroupId = ParentUserGroupId;

            if (sn.Misc.ConfDsUserGroupReportSettings == null || sn.Misc.ConfDsUserGroupReportSettings.Tables.Count == 0)
            {
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                if (objUtil.ErrCheck(dbu.GetUserGroupReportSettings(sn.UserID, iUserGroupId, ParentUserGroupId, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserGroupReportSettings(sn.UserID, iUserGroupId, ParentUserGroupId, sn.SecId, ref xml), true))
                    {
                        //can't load page
                        return dsUserGroupSettings;
                    }

                if (xml == "")
                {
                    //can't load page
                    return dsUserGroupSettings;
                }

                strrXML = new StringReader(xml);
                dsUserGroupSettings.ReadXml(strrXML);

                sn.Misc.ConfDsUserGroupReportSettings = dsUserGroupSettings;
            }
            else
            {
                dsUserGroupSettings = sn.Misc.ConfDsUserGroupReportSettings;
            }

            return dsUserGroupSettings;
        }

        private DataSet GetUserGroupCommandSettings()
        {
            DataSet dsUserGroupSettings = new DataSet();
            StringReader strrXML = null;
            string xml = "";
            int iUserGroupId = 0;

            if (UserGroupId > 0)
            {
                iUserGroupId = UserGroupId;
                ParentUserGroupId = 0;
            }
            else
                iUserGroupId = ParentUserGroupId;

            if (sn.Misc.ConfDsUserGroupCommandSettings == null || sn.Misc.ConfDsUserGroupCommandSettings.Tables.Count == 0)
            {
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                if (objUtil.ErrCheck(dbu.GetUserGroupCommandSettings(sn.UserID, iUserGroupId, ParentUserGroupId, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserGroupCommandSettings(sn.UserID, iUserGroupId, ParentUserGroupId, sn.SecId, ref xml), true))
                    {
                        //can't load page
                        return dsUserGroupSettings;
                    }

                if (xml == "")
                {
                    //can't load page
                    return dsUserGroupSettings;
                }

                strrXML = new StringReader(xml);
                dsUserGroupSettings.ReadXml(strrXML);

                sn.Misc.ConfDsUserGroupCommandSettings = dsUserGroupSettings;
            }
            else
            {
                dsUserGroupSettings = sn.Misc.ConfDsUserGroupCommandSettings;
            }

            return dsUserGroupSettings;
        }

        private void addFormName(string sGroupName, string sFormName, Panel pnlUserGroupSettings)
        {
            LiteralControl _trOpen = new LiteralControl("<tr>");
            LiteralControl _trClose = new LiteralControl("</tr>");
            LiteralControl _tdInputOpen = new LiteralControl("<td class=\"formtext\" colspan=\"2\">");
            LiteralControl _tdClose = new LiteralControl("</td>");

            Label _labelInput = new Label();
            _labelInput.ID = sGroupName; // "lblFormName" + iFormID.ToString();
            _labelInput.Font.Bold = true;
            _labelInput.Text = sFormName;

            pnlUserGroupSettings.Controls.Add(_trOpen);
            pnlUserGroupSettings.Controls.Add(_tdInputOpen);

            pnlUserGroupSettings.Controls.Add(_labelInput);

            pnlUserGroupSettings.Controls.Add(_tdClose);
            pnlUserGroupSettings.Controls.Add(_trClose);

        }

        protected void CheckBox_CheckChanged(object sender, EventArgs e)
        {
            int ControlId = 0;
            //client id of the control that triggered the event
            string sCheckboxClientID = ((CheckBox)sender).ClientID;
            if (!String.IsNullOrEmpty(sCheckboxClientID))
                ControlId = Convert.ToInt32(sCheckboxClientID.Replace("chkUserGroupControl", ""));
            string sCheckboxValue = getCheckboxInputValue(ControlId, "UserGroupControl", pnlUserGroupControlSettings);
            //uncheck depended checkboxes
            //if (String.IsNullOrEmpty(sCheckboxValue))

            string b = sCheckboxClientID;

        }

        private void addUserGroupSetting(int iControlId, string sControlDescription, bool bSelectedControl, string sControlName, Panel pnlUserGroupSettings,
            string sChildControlIDs, string sParentControlIDs)
        {
            LiteralControl _trOpen = new LiteralControl("<tr>");
            LiteralControl _trClose = new LiteralControl("</tr>");
            LiteralControl _tdOpen = new LiteralControl("<td class=\"formtext\" colspan=\"2\">");
            LiteralControl _space = new LiteralControl("&nbsp;");
            LiteralControl _tdClose = new LiteralControl("</td>");

            Label _labelInput = new Label();
            _labelInput.ID = "lbl" + sControlName + iControlId.ToString();
            _labelInput.Text = sControlDescription;

            CheckBox _checkboxInput = new CheckBox();
            _checkboxInput.ID = "chk" + sControlName + iControlId.ToString();
            _checkboxInput.Checked = bSelectedControl;
            _checkboxInput.Enabled = CheckboxesEnabled;
            if (!String.IsNullOrEmpty(sChildControlIDs) && !String.IsNullOrEmpty(sParentControlIDs) && CheckboxesEnabled)
            {
                _checkboxInput.Attributes.Add("onclick", "SetChildCheckboxes(this, '" + sChildControlIDs + "');SetParentCheckboxes(this, '" + sParentControlIDs + "')");
            }
            else if (!String.IsNullOrEmpty(sChildControlIDs) && String.IsNullOrEmpty(sParentControlIDs) && CheckboxesEnabled)
            {
                _checkboxInput.Attributes.Add("onclick", "SetChildCheckboxes(this, '" + sChildControlIDs + "')");
            }
            else if (String.IsNullOrEmpty(sChildControlIDs) && !String.IsNullOrEmpty(sParentControlIDs) && CheckboxesEnabled)
            {
                _checkboxInput.Attributes.Add("onclick", "SetParentCheckboxes(this, '" + sParentControlIDs + "')");
            }

            //_checkboxInput.AutoPostBack = true; 
            //_checkboxInput.CausesValidation = false;
            //_checkboxInput.CheckedChanged += new EventHandler(this.CheckBox_CheckChanged);
            pnlUserGroupSettings.Controls.Add(_trOpen);
            //Checkbox
            pnlUserGroupSettings.Controls.Add(_tdOpen);
            pnlUserGroupSettings.Controls.Add(_checkboxInput);
            pnlUserGroupSettings.Controls.Add(_space);
            //Label
            pnlUserGroupSettings.Controls.Add(_labelInput);
            pnlUserGroupSettings.Controls.Add(_tdClose);

            pnlUserGroupSettings.Controls.Add(_trClose);
        }

        // Function to test for Positive Integers.
        private bool IsNaturalNumber(String strNumber)
        {
            Regex objNotNaturalPattern = new Regex("[^0-9]");
            Regex objNaturalPattern = new Regex("0*[1-9][0-9]*");
            return !objNotNaturalPattern.IsMatch(strNumber) && objNaturalPattern.IsMatch(strNumber);
        }

        private string getCheckboxInputValue(int iControlId, string sControlName, Panel pnlUserGroupSettings)
        {
            string sInputVal = String.Empty;

            CheckBox chk = new CheckBox();
            string sInputID = "chk" + sControlName + iControlId.ToString();
            chk = ((CheckBox)pnlUserGroupSettings.FindControl(sInputID));

            if (chk != null)
            {
                if (chk.Checked)
                    sInputVal = iControlId.ToString();
                //else
                //    sInputVal = iControlId.ToString() + ":0";
            }

            return sInputVal;
        }

        #endregion

    }
}