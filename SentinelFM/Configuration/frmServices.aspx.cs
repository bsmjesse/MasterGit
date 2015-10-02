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
using System.Text.RegularExpressions;
using System.Text;

namespace SentinelFM
{
    public partial class Configuration_frmServices : SentinelFMBasePage
    {
        public string ExportToExcel = String.Empty;
        string confirm;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);

                ExportToExcel = (string)base.GetLocalResourceObject("ExportToExcel");

                if ((sn == null) || (sn.UserName == ""))
                {
                    RedirectToLogin();
                    return;
                }

                if (!Page.IsPostBack)
                {
                    SetOrgsDropdwon();
                    SetFeaturesDropdwon(0);
                    dgOrganizationFeatures_Fill(0, 0, false);
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmVehicleInfo, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
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

        private void dgOrganizationFeatures_Fill(int OrganizationID, int ServiceID, bool IsBillable)
        {
            DataSet dsOrganizationFeatures = new DataSet();
            StringReader strrXML = null;
            string xml = String.Empty;
            StringBuilder _sOrganizationFeaturess = new StringBuilder();
            string sOrganizationFeatures = String.Empty;
            string sServiceName = String.Empty;
            string sOrganizationName = String.Empty;
            string sUserName = String.Empty;
            string sServiceStartDate = String.Empty;
            string sServiceEndtDate = String.Empty;
            string sIsBillable = String.Empty;
            string sStatus = String.Empty;

            try
            {
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                if (objUtil.ErrCheck(dbo.GetOrganizationServices(sn.UserID, sn.SecId, OrganizationID, ServiceID, IsBillable, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetOrganizationServices(sn.UserID, sn.SecId, OrganizationID, ServiceID, IsBillable, ref xml), true))
                    {
                        this.dgOrganizationFeatures.DataSource = null;
                        this.dgOrganizationFeatures.DataBind();
                        imgExcel.Visible = false;
                        return;
                    }

                if (xml == "")
                {
                    this.dgOrganizationFeatures.DataSource = null;
                    this.dgOrganizationFeatures.DataBind();
                    lblMsg.Text = (string)base.GetLocalResourceObject("NoFeaturesForOrg");
                    plhMsg.Visible = true;
                    imgExcel.Visible = false;
                    return;
                }

                strrXML = new StringReader(xml);
                dsOrganizationFeatures.ReadXml(strrXML);

                dgOrganizationFeatures.DataSource = dsOrganizationFeatures;
                dgOrganizationFeatures.DataBind();
                plhMsg.Visible = false;
                dgOrganizationFeatures.CurrentPageIndex = 0;

                if (dsOrganizationFeatures != null && dsOrganizationFeatures.Tables.Count > 0)
                {
                    foreach (DataRow drOrganizationFeatures in dsOrganizationFeatures.Tables[0].Rows)
                    {
                        if (drOrganizationFeatures["ServiceName"] != null)
                            sServiceName = drOrganizationFeatures["ServiceName"].ToString();
                        else
                            sServiceName = string.Empty;
                        if (drOrganizationFeatures["OrganizationName"] != null)
                            sOrganizationName = drOrganizationFeatures["OrganizationName"].ToString();
                        else
                            sOrganizationName = string.Empty;
                        if (drOrganizationFeatures["UserName"] != null)
                            sUserName = drOrganizationFeatures["UserName"].ToString();
                        else
                            sUserName = string.Empty;
                        if (drOrganizationFeatures["ServiceStartDate"] != null)
                            sServiceStartDate = drOrganizationFeatures["ServiceStartDate"].ToString();
                        else
                            sServiceStartDate = string.Empty;
                        if (drOrganizationFeatures["ServiceEndDate"] != null)
                            sServiceEndtDate = drOrganizationFeatures["ServiceEndDate"].ToString();
                        else
                            sServiceEndtDate = string.Empty;
                        if (drOrganizationFeatures["IsBillable"] != null)
                            sIsBillable = drOrganizationFeatures["IsBillable"].ToString();
                        else
                            sIsBillable = string.Empty;
                        if (drOrganizationFeatures["ServiceStatus"] != null)
                            sStatus = drOrganizationFeatures["ServiceStatus"].ToString();
                        else
                            sStatus = string.Empty;

                        _sOrganizationFeaturess.Append(String.Format("[\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"],", sServiceName, sOrganizationName, sUserName, sServiceStartDate, sServiceEndtDate, sIsBillable, sStatus));
                    }
                }
                if (_sOrganizationFeaturess.Length > 0)
                {
                    sOrganizationFeatures = _sOrganizationFeaturess.ToString().Substring(0, _sOrganizationFeaturess.Length - 1);
                    expdata.Value = "{\"Header\":[\"" + (string)base.GetLocalResourceObject("dgOrganizationFeatures_ServiceName") + "\",\"" + 
                        (string)base.GetLocalResourceObject("dgOrganizationFeatures_Organization") + "\",\"" + (string)base.GetLocalResourceObject("dgOrganizationFeatures_EnabledByUser") +
                        "\",\"" + (string)base.GetLocalResourceObject("dgOrganizationFeatures_StartDate") + "\",\"" + (string)base.GetLocalResourceObject("dgOrganizationFeatures_EndDate") +
                        "\",\"" + (string)base.GetLocalResourceObject("dgOrganizationFeatures_IsBillable") + "\",\"" + (string)base.GetLocalResourceObject("dgOrganizationFeatures_ServiceStatus") +
                        "\"],\"Data\":[" + sOrganizationFeatures + "]}";
                    imgExcel.Visible = true;
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
            this.dgOrganizationFeatures.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgOrganizationFeatures_PageIndexChanged);
            this.dgOrganizationFeatures.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgOrganizationFeatures_ItemDataBound);
        }

        #endregion

        private void dgOrganizationFeatures_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            int OrganizationID = 0;
            int ServiceID = 0;
            bool IsBillable = chkBillable.Checked;

            dgOrganizationFeatures.CurrentPageIndex = e.NewPageIndex;

            if (!String.IsNullOrEmpty(cboOrganization.SelectedValue))
                OrganizationID = Convert.ToInt32(cboOrganization.SelectedValue);

            if (OrganizationID == 0 && !String.IsNullOrEmpty(cboService.SelectedValue))
            {
                ServiceID = Convert.ToInt32(cboService.SelectedValue);
            }

            dgOrganizationFeatures_Fill(OrganizationID, ServiceID, IsBillable);
        }

        protected void dgOrganizationFeatures_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                LinkButton deleteBtn = (LinkButton)e.Item.Cells[7].Controls[0];
                if (e.Item.Cells[6].Text.Trim() != "not active")
                {
                    confirm = "return confirm('" + (string)base.GetLocalResourceObject("ConfirmDisable") + "')";
                    deleteBtn.Attributes.Add("onclick", confirm);
                } 
            }
        }

        protected void dgOrganizationFeatures_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            int OrganizationID = 0;
            int ServiceID = 0;
            bool IsBillable = chkBillable.Checked;

            try
            {
                if (!String.IsNullOrEmpty(cboOrganization.SelectedValue))
                    OrganizationID = Convert.ToInt32(cboOrganization.SelectedValue);

                if (OrganizationID == 0 && !String.IsNullOrEmpty(cboService.SelectedValue))
                {
                    ServiceID = Convert.ToInt32(cboService.SelectedValue);
                }

                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                if (objUtil.ErrCheck(dbo.DeleteOrganizationService(sn.UserID, sn.SecId, Convert.ToInt32(dgOrganizationFeatures.DataKeys[e.Item.ItemIndex].ToString())), false))
                    if (objUtil.ErrCheck(dbo.DeleteOrganizationService(sn.UserID, sn.SecId, Convert.ToInt32(dgOrganizationFeatures.DataKeys[e.Item.ItemIndex].ToString())), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Disable organization feature failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        return;
                    }

                //display organization features
                dgOrganizationFeatures_Fill(OrganizationID, ServiceID, IsBillable);
                //set Features for add for Organization
                SetFeaturesDropdwon(OrganizationID);
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

        private void SetOrgsDropdwon()
        {
            DataSet dsOrganizations = new DataSet();
            StringReader strrXML = null;
            string xml = String.Empty;

            ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

            if (objUtil.ErrCheck(dbo.GetAllOrganizationsInfoXML(sn.UserID, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbo.GetAllOrganizationsInfoXML(sn.UserID, sn.SecId, ref xml), true))
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

            strrXML = new StringReader(xml);
            dsOrganizations.ReadXml(strrXML);

            cboOrganization.DataSource = dsOrganizations;
            cboOrganization.DataBind();

            cboOrganization.Items.Insert(0, new ListItem(String.Empty, String.Empty));
            cboOrganization.SelectedIndex = 0;
        }

        private void SetFeaturesDropdwon(int OrganizationID)
        {
            DataSet dsFeatures = new DataSet();
            StringReader strrXML = null;
            string xml = String.Empty;

            cboService.Items.Clear();

            ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

            if (objUtil.ErrCheck(dbo.GetOrganizationServicesForAdd(sn.UserID, sn.SecId, OrganizationID, ref xml), false))
                if (objUtil.ErrCheck(dbo.GetOrganizationServicesForAdd(sn.UserID, sn.SecId, OrganizationID, ref xml), true))
                {
                    this.cboService.DataSource = null;
                    this.cboService.DataBind();
                    return;
                }

            if (xml == "")
            {
                this.cboService.DataSource = null;
                this.cboService.DataBind();
                return;
            }

            strrXML = new StringReader(xml);
            dsFeatures.ReadXml(strrXML);

            cboService.DataSource = dsFeatures;
            cboService.DataBind();

            cboService.Items.Insert(0, new ListItem(String.Empty, String.Empty));
            cboService.SelectedIndex = 0;
           
        }

        private void dgOrganizationFeatures_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                e.Item.Cells[7].ToolTip = (string)base.GetLocalResourceObject("EditToolTip");
                LinkButton deleteBtn = (LinkButton)e.Item.Cells[7].Controls[0];
                if (e.Item.Cells[6].Text.Trim() == "not active")
                    deleteBtn.Visible = false;
            }
        }

        protected void cmdClear_Click(object sender, EventArgs e)
        {
            //this.txtSearchParam.Text = "";
            dgOrganizationFeatures.SelectedIndex = -1;
            dgOrganizationFeatures.CurrentPageIndex = 0;
            this.dgOrganizationFeatures.DataSource = sn.Misc.ConfDsControlsGrid;
            this.dgOrganizationFeatures.DataBind();
        }

        protected void cmdEnableFeature_Click(object sender, EventArgs e)
        {
            int OrganizationServiceID = 0;
            int OrganizationID = 0;
            int FeatureID = 0;
            int ServiceID = 0;
            bool IsBillable = chkBillable.Checked;

            if (!String.IsNullOrEmpty(cboOrganization.SelectedValue))
                OrganizationID = Convert.ToInt32(cboOrganization.SelectedValue);
            if (!String.IsNullOrEmpty(cboService.SelectedValue))
                FeatureID = Convert.ToInt32(cboService.SelectedValue);

            if (OrganizationID > 0 && FeatureID > 0)
            {
                try
                {
                    ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                    OrganizationServiceID = dbo.AddOrganizationService(sn.UserID, sn.SecId, OrganizationID, FeatureID);

                    if (OrganizationServiceID > 0)
                    {
                        lblMsg.Text = (string)GetLocalResourceObject("SelectedFeatureEnabledMsg");
                        plhMsg.Visible = true;

                        //display organization features
                        dgOrganizationFeatures_Fill(OrganizationID, ServiceID, IsBillable);
                        //set Features for add for Organization
                        SetFeaturesDropdwon(OrganizationID);
                    }
                    else
                    {
                        lblMsg.Text = (string)GetLocalResourceObject("ErrorToEnableSelectedFeatureMsg");
                        plhMsg.Visible = true;
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
        }

        protected void cboOrganization_SelectedIndexChanged(object sender, EventArgs e)
        {
            int OrganizationID = 0;
            int ServiceID = 0;
            bool IsBillable = chkBillable.Checked;

            if (!String.IsNullOrEmpty(cboOrganization.SelectedValue))
            {
                OrganizationID = Convert.ToInt32(cboOrganization.SelectedValue);
                cmdEnableFeature.Visible = true;
            }
            else
            {
                cmdEnableFeature.Visible = false;
                if (!String.IsNullOrEmpty(cboService.SelectedValue))
                {
                    ServiceID = Convert.ToInt32(cboService.SelectedValue);
                }
            }

            //display organization features
            dgOrganizationFeatures_Fill(OrganizationID, ServiceID, IsBillable);
            //set Features for add for Organization
            SetFeaturesDropdwon(OrganizationID);
        }

        protected void chkBillable_CheckedChanged(object sender, EventArgs e)
        {
            int OrganizationID = 0;
            int ServiceID = 0;
            bool IsBillable = chkBillable.Checked;

            if (!String.IsNullOrEmpty(cboOrganization.SelectedValue))
                OrganizationID = Convert.ToInt32(cboOrganization.SelectedValue);

            if (OrganizationID == 0 && !String.IsNullOrEmpty(cboService.SelectedValue))
            {
                ServiceID = Convert.ToInt32(cboService.SelectedValue);
            }

            dgOrganizationFeatures_Fill(OrganizationID, ServiceID, IsBillable);
        }

        protected void cboService_SelectedIndexChanged(object sender, EventArgs e)
        {
            int OrganizationID = 0;
            int ServiceID = 0;
            bool IsBillable = chkBillable.Checked;

            if (!String.IsNullOrEmpty(cboOrganization.SelectedValue))
                OrganizationID = Convert.ToInt32(cboOrganization.SelectedValue);

            if (OrganizationID == 0 && !String.IsNullOrEmpty(cboService.SelectedValue))
            {
                ServiceID = Convert.ToInt32(cboService.SelectedValue);
            }

            dgOrganizationFeatures_Fill(OrganizationID, ServiceID, IsBillable);
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

        protected void cmdGroups_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmgroups.aspx");
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