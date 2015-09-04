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
using System.Globalization;

namespace SentinelFM
{
    public partial class Configuration_frmgroups : SentinelFMBasePage
    {
        string confirm;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);
                //Enable Search on pressing Enter Key
                txtSearchParam.Attributes.Add("onkeypress", "return clickButton(event,'" + cmdSearch.ClientID + "')");

                if ((sn == null) || (sn.UserName == ""))
                {
                    RedirectToLogin();
                    return;
                }

                dgGroups_Fill();

                if (!Page.IsPostBack)
                {
                    sn.Misc.DtGrid = null;
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmVehicleInfo, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    //this.tblAddUsers.Visible = false;
                    ViewState["ConfirmDelete"] = "0";  
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

                    cmdCreateDefaultGroups.Visible = false;
                    cmdAddGroup.Visible = true;
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

        private void dgGroups_Fill()
        {
            DataSet dsGroupsGrid = new DataSet();
            StringReader strrXML = null;

            string xml = "";

            try
            { 
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                if (objUtil.ErrCheck(dbu.GetUserGroupsForUpdateByUser(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserGroupsForUpdateByUser(sn.UserID, sn.SecId, ref xml), true))
                    {
                        this.dgGroups.DataSource = null;
                        this.dgGroups.DataBind();
                        //cmdCreateDefaultGroups.Visible = true;
                        //cmdAddGroup.Visible = false;
                        return;
                    }

                if (xml == "")
                {
                    this.dgGroups.DataSource = null;
                    this.dgGroups.DataBind();
                    //cmdCreateDefaultGroups.Visible = true;
                    //cmdAddGroup.Visible = false;
                    return;
                }

                strrXML = new StringReader(xml);
                dsGroupsGrid.ReadXml(strrXML);

                this.dgGroups.DataSource = dsGroupsGrid;
                dgGroups.DataBind();
                //cmdCreateDefaultGroups.Visible = false;
                //cmdAddGroup.Visible = true;
                sn.Misc.ConfDsGroupsGrid = dsGroupsGrid;
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
            this.dgGroups.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgGroups_PageIndexChanged);
            this.dgGroups.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgGroups_ItemDataBound);
            this.dgGroups.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgGroups_DeleteCommand);
        }

        #endregion

        protected void dgGroups_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {
            //Check security 
            bool cmd = sn.User.ControlEnable(sn, 88);
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

            if (objUtil.ErrCheck(dbu.DeleteUserGroup(sn.UserID, sn.SecId, Convert.ToInt32(dgGroups.DataKeys[e.Item.ItemIndex].ToString())), false))
                if (objUtil.ErrCheck(dbu.DeleteUserGroup(sn.UserID, sn.SecId, Convert.ToInt32(dgGroups.DataKeys[e.Item.ItemIndex].ToString())), true))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("DeleteUserGroupError");
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Delete User Group failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    return;
                }

            dgGroups.SelectedIndex = -1;
            //lblMessage.Text = (string)base.GetLocalResourceObject("GroupDeleted");
            dgGroups.CurrentPageIndex = 0;
            dgGroups_Fill();
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

        private void dgGroups_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            dgGroups.CurrentPageIndex = e.NewPageIndex;
            if (sn.Misc.DtGrid != null)
            {
                dgGroups.DataSource = sn.Misc.DtGrid;
                dgGroups.DataBind();
            }
            else
            {
                dgGroups_Fill();
                dgGroups.SelectedIndex = -1;
            }
        }

        protected void dgGroups_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }

        private void ClearFields()
        {
            this.lblMessage.Text = "";
        }

        private void dgGroups_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            string windowOpen = String.Empty;
            
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                LinkButton EditSettings = (LinkButton)e.Item.Cells[4].Controls[0];
                e.Item.Cells[4].ToolTip = (string)base.GetLocalResourceObject("EditToolTip");
                
                windowOpen = "controlsWindow('" + dgGroups.DataKeys[e.Item.ItemIndex].ToString() + "')";
                EditSettings.Attributes.Add("onclick", windowOpen);

                LinkButton DeleteSettings = (LinkButton)e.Item.Cells[5].Controls[0];
                if (e.Item.Cells[1].Text.Trim() == "false")
                    DeleteSettings.Visible = false;
                else
                    e.Item.Cells[5].ToolTip = (string)base.GetLocalResourceObject("DeleteToolTip");
            }
        }

        protected void dgGroups_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                confirm = "return confirm('" + (string)base.GetLocalResourceObject("ConfirmDelete") + "')";
                LinkButton deleteBtn = (LinkButton)e.Item.Cells[5].Controls[0];
                deleteBtn.Attributes.Add("onclick", confirm);
            }
        }

        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";
                DataRow[] drCollections = null;
                if (sn.Misc.ConfDsGroupsGrid == null || sn.Misc.ConfDsGroupsGrid.Tables.Count == 0)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "Error to load User Groups. Please try again.";
                    return;
                }

                DataTable dt = sn.Misc.ConfDsGroupsGrid.Tables[0].Clone();
                sn.Misc.ConfUsersSelectedGridPage = dgGroups.CurrentPageIndex;
                dgGroups.CurrentPageIndex = 0;
                string filter = "";
                
                if (!String.IsNullOrEmpty(this.txtSearchParam.Text.Trim()))
                    filter = String.Format("UserGroupName like '%{0}%'", this.txtSearchParam.Text.Replace("'", "''"));

                if (filter == "")
                {
                    this.dgGroups.DataSource = sn.Misc.ConfDsGroupsGrid;
                    this.dgGroups.DataBind();
                }

                else
                {
                    drCollections = sn.Misc.ConfDsGroupsGrid.Tables[0].Select(filter);
                    if (drCollections != null && drCollections.Length > 0)
                    {
                        foreach (DataRow dr in drCollections)
                            dt.ImportRow(dr);
                    }

                    this.dgGroups.DataSource = dt;
                    this.dgGroups.DataBind();
                    sn.Misc.DtGrid = dt;
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
            dgGroups.SelectedIndex = -1;
            dgGroups.CurrentPageIndex = sn.Misc.ConfUsersSelectedGridPage;
            this.dgGroups.DataSource = sn.Misc.ConfDsGroupsGrid;
            this.dgGroups.DataBind();
            sn.Misc.DtGrid = null;
        }

        protected void cmdCreateDefaultGroups_Click(object sender, EventArgs e)
        {
            try
            {
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                if (objUtil.ErrCheck(dbo.AddUserGroupsDefault(sn.UserID, sn.SecId, sn.User.OrganizationId), false))
                    if (objUtil.ErrCheck(dbo.AddUserGroupsDefault(sn.UserID, sn.SecId, sn.User.OrganizationId), true))
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("CreateDefaultGroupsError");
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Create Default User Groups failed. OrganizationId: " + sn.User.OrganizationId.ToString() + " Form:" + Page.GetType().Name));
                        return;
                    }

                dgGroups_Fill();
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

        protected void cmdUserDashBoards_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmuserdashboards.aspx");
        }

        protected void cmdUserInfo_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmUsers.aspx");
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

}
}