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
    public partial class Configuration_frmControls : SentinelFMBasePage
    {
        public int ControlId
        {
            get
            {
                if (ViewState["ControlId"] != null)
                    return Convert.ToInt32(ViewState["ControlId"]);
                else
                    return -1;
            }
            set
            {
                ViewState["ControlId"] = value;
            }
        }

        string GenericLabel = "lblDesc";
        string GenericText = "txtDesc";
        string GenericRfv = "rfvDesc";

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);

                if ((sn == null) || (sn.UserName == ""))
                {
                    RedirectToLogin();
                    return;
                }

                if (!Page.IsPostBack)
                {
                    SetDropdwons();
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmVehicleInfo, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    sn.Misc.DtGrid = null;
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

                if (ControlId > -1)
                {
                    plhControls.Visible = false;
                    plhControlForm.Visible = true;
                    plhCommandNumber.Visible = true;
                    if (ControlId > 0)
                    {
                        cmdUpdate.Text = (string)GetLocalResourceObject("Update");
                    }
                    else
                        cmdUpdate.Text = (string)GetLocalResourceObject("Add");
                }
                else
                {
                    plhControls.Visible = true;
                    plhControlForm.Visible = false;
                    plhCommandNumber.Visible = false;
                    if (sn.Misc.DtGrid != null)
                    {
                        dgControls.DataSource = sn.Misc.DtGrid;
                        dgControls.DataBind();
                    }
                    else
                    {
                        dgControls_Fill();
                        dgControls.SelectedIndex = -1;
                    }
                }
                SetControls();

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

        private void dgControls_Fill()
        {
            DataSet dgControlsGrid = new DataSet();
            StringReader strrXML = null;
            string xml = "";

            try
            {
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                if (objUtil.ErrCheck(dbu.GetControlsForUpdate(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetControlsForUpdate(sn.UserID, sn.SecId, ref xml), true))
                    {
                        this.dgControls.DataSource = null;
                        this.dgControls.DataBind();
                        return;
                    }

                if (xml == "")
                {
                    this.dgControls.DataSource = null;
                    this.dgControls.DataBind();
                    return;
                }

                strrXML = new StringReader(xml);
                dgControlsGrid.ReadXml(strrXML);

                this.dgControls.DataSource = dgControlsGrid;
                dgControls.DataBind();
                sn.Misc.ConfDsControlsGrid = dgControlsGrid;
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
            this.dgControls.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgControls_PageIndexChanged);
            this.dgControls.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgControls_ItemDataBound);
        }

        #endregion

        private void dgControls_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            dgControls.CurrentPageIndex = e.NewPageIndex;
            if (sn.Misc.DtGrid != null)
            {
                dgControls.DataSource = sn.Misc.DtGrid;
                dgControls.DataBind();
            }
            else
            {
                dgControls_Fill();
                dgControls.SelectedIndex = -1;
            }
        }

        protected void dgControls_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ControlId = Convert.ToInt32(dgControls.DataKeys[dgControls.SelectedIndex]);
            ControlId = Convert.ToInt32(dgControls.SelectedItem.Cells[0].Text);
            plhControls.Visible = false;
            plhControlForm.Visible = true;
            cboControls_Fill(ControlId.ToString());
            ControlForm_Fill();
            lblMsg.Text = String.Empty;
            plhMsg.Visible = false;
        }

        // Function to test for Positive Integers.
        private bool IsNaturalNumber(String strNumber)
        {
            Regex objNotNaturalPattern = new Regex("[^0-9]");
            Regex objNaturalPattern = new Regex("0*[1-9][0-9]*");
            return !objNotNaturalPattern.IsMatch(strNumber) && objNaturalPattern.IsMatch(strNumber);
        }

        private void SetDropdwons()
        {
            DataSet dsGroups = new DataSet();
            StringReader strrXML = null;
            string xml = String.Empty;

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            if (objUtil.ErrCheck(dbu.GetForms(sn.UserID, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbu.GetForms(sn.UserID, sn.SecId, ref xml), true))
                {
                    this.cboForm.DataSource = null;
                    this.cboForm.DataBind();
                    return;
                }

            if (xml == "")
            {
                this.cboForm.DataSource = null;
                this.cboForm.DataBind();
                return;
            }

            strrXML = new StringReader(xml);
            dsGroups.ReadXml(strrXML);

            cboForm.DataSource = dsGroups;
            cboForm.DataBind();

            cboForm.Items.Insert(0, new ListItem(String.Empty, String.Empty));
            cboForm.SelectedIndex = 0;
        }

        private void ControlForm_Fill()
        {
            DataSet dsControl = new DataSet();
            StringReader strrXML = null;
            string xml = String.Empty;
            string sParentControlId = String.Empty;

            ResetFields();

            try
            {
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                if (objUtil.ErrCheck(dbu.GetControl(sn.UserID, sn.SecId, ControlId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetControl(sn.UserID, sn.SecId, ControlId, ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                    return;

                strrXML = new StringReader(xml);
                dsControl.ReadXml(strrXML);

                DataTable dtControl = dsControl.Tables[0];
                if (dtControl.Rows.Count > 0)
                {
                    plhCommandNumber.Visible = true;
                    lblCommandNumberValue.Text = dtControl.Rows[0]["ControlId"].ToString();
                    if (dtControl.Rows[0]["FormID"] != DBNull.Value)
                        cboForm.SelectedValue = dtControl.Rows[0]["FormID"].ToString();
                    else
                        cboForm.SelectedIndex = 0;
                    txtControlID.Text = dtControl.Rows[0]["ControlName"].ToString();
                    txtDescription.Text = dtControl.Rows[0]["Description"].ToString();
                    txtURL.Text = dtControl.Rows[0]["ControlURL"].ToString();
                    chkIsActive.Checked = Convert.ToBoolean(dtControl.Rows[0]["ControlIsActive"]);
                    foreach (DataRow dr in dtControl.Rows)
                    {
                        foreach (Control ctl in pnlControlNames.Controls)
                        {
                            if (ctl is TextBox && ctl.ID.Contains(GenericText + dr["LanguageID"].ToString()))
                            {
                                ((TextBox)ctl).Text = dr["ControlDescription"].ToString();
                            }
                        }

                    }
                    try
                    {
                        sParentControlId = dtControl.Rows[0]["ParentControlId"].ToString();
                        if (sParentControlId == "0")
                            sParentControlId = String.Empty;
                        cboParentControl.SelectedValue = sParentControlId;
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
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

        private void ResetFields()
        {
            foreach (Control ctl in pnlControlNames.Controls)
            {
                if (ctl is TextBox)
                {
                    ((TextBox)ctl).Text = String.Empty;
                }
            }
            plhCommandNumber.Visible = false;
            lblCommandNumberValue.Text = String.Empty;
            cboForm.SelectedIndex = 0;
            txtControlID.Text = String.Empty;
            txtDescription.Text = String.Empty;
            txtURL.Text = String.Empty;
            chkIsActive.Checked = false;
        }

        private void SetControls()
        {
            DataSet dsLanguages = new DataSet();
            StringReader strrXML = null;
            string xml = String.Empty;
            LiteralControl RowStart;
            LiteralControl RowEnd;
            LiteralControl ColumnStart;
            LiteralControl SecondColumnStart;
            LiteralControl ColumnEnd;
            LiteralControl WhiteSpace;
            LiteralControl MandatoryLabel;
            Label DescriptionLabel;
            TextBox DescriptionText;
            RequiredFieldValidator DescriptionRfv;
            int LanguageID = 0;

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            if (objUtil.ErrCheck(dbu.GetLanguages(sn.UserID, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbu.GetLanguages(sn.UserID, sn.SecId, ref xml), true))
                {
                    return;
                }

            if (xml == "")
                return;

            strrXML = new StringReader(xml);
            dsLanguages.ReadXml(strrXML);

            DataTable dt = dsLanguages.Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                LanguageID = Convert.ToInt32(dr["LanguageID"]);
                RowStart = new LiteralControl("<tr>");
                RowEnd = new LiteralControl("</tr>");
                ColumnStart = new LiteralControl("<td class=\"formtext\" style=\"text-align: right\">");
                SecondColumnStart = new LiteralControl("<td class=\"formtext\">");
                ColumnEnd = new LiteralControl("</td>");
                MandatoryLabel = new LiteralControl("<span class=\"ast\">*</span>&nbsp;");
                WhiteSpace = new LiteralControl("&nbsp;");

                DescriptionLabel = new Label();
                DescriptionLabel.ID = GenericLabel + dr["LanguageID"].ToString();
                DescriptionLabel.Text = (string)base.GetLocalResourceObject("ControlName") + " " + dr["Language"].ToString() + ":";

                DescriptionText = new TextBox();
                DescriptionText.ID = GenericText + dr["LanguageID"].ToString();
                DescriptionText.MaxLength = 200;
                DescriptionText.CssClass = "formtext TextboxStyle";

                DescriptionRfv = new RequiredFieldValidator();
                DescriptionRfv.ID = GenericRfv + dr["LanguageID"].ToString();
                DescriptionRfv.ControlToValidate = DescriptionText.ID;
                DescriptionRfv.ErrorMessage = (string)base.GetLocalResourceObject("MandatoryField");
                DescriptionRfv.ForeColor = System.Drawing.Color.Firebrick;
                DescriptionRfv.Display = ValidatorDisplay.Dynamic;
                if (LanguageID == 1)
                    DescriptionRfv.Enabled = true;
                else
                    DescriptionRfv.Enabled = false;

                pnlControlNames.Controls.Add(RowStart);
                pnlControlNames.Controls.Add(ColumnStart);
                //pnlControlNames.Controls.Add(MandatoryLabel);
                pnlControlNames.Controls.Add(DescriptionLabel);
                pnlControlNames.Controls.Add(ColumnEnd);

                ColumnEnd = new LiteralControl("</td>");

                pnlControlNames.Controls.Add(SecondColumnStart);
                pnlControlNames.Controls.Add(DescriptionText);
                pnlControlNames.Controls.Add(WhiteSpace);
                pnlControlNames.Controls.Add(DescriptionRfv);
                pnlControlNames.Controls.Add(ColumnEnd);
                pnlControlNames.Controls.Add(RowEnd);
            }
            dt.Dispose();
        }

        private void dgControls_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            //string windowOpen = String.Empty;

            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                LinkButton settings = (LinkButton)e.Item.Cells[4].Controls[0];
                e.Item.Cells[4].ToolTip = (string)base.GetLocalResourceObject("EditToolTip"); // +" " + dgControls.DataKeys[e.Item.ItemIndex].ToString();

                //windowOpen = "controlsWindow('" + dgControls.DataKeys[e.Item.ItemIndex].ToString() + "')";
                //settings.Attributes.Add("onclick", windowOpen);
            }
        }

        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow[] drCollections = null;

                DataTable dt = sn.Misc.ConfDsControlsGrid.Tables[0].Clone();
                sn.Misc.ConfUsersSelectedGridPage = dgControls.CurrentPageIndex;
                dgControls.CurrentPageIndex = 0;
                string filter = "";

                if (!String.IsNullOrEmpty(this.txtSearchParam.Text.Trim()))
                    filter = String.Format("ControlDescription like '%{0}%'", this.txtSearchParam.Text.Replace("'", "''"));

                if (filter == "")
                {
                    this.dgControls.DataSource = sn.Misc.ConfDsControlsGrid;
                    this.dgControls.DataBind();
                }
                else
                {
                    drCollections = sn.Misc.ConfDsControlsGrid.Tables[0].Select(filter);
                    if (drCollections != null && drCollections.Length > 0)
                    {
                        foreach (DataRow dr in drCollections)
                            dt.ImportRow(dr);
                    }

                    this.dgControls.DataSource = dt;
                    this.dgControls.DataBind();
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
            dgControls.SelectedIndex = -1;
            dgControls.CurrentPageIndex = 0;
            this.dgControls.DataSource = sn.Misc.ConfDsControlsGrid;
            this.dgControls.DataBind();
            sn.Misc.DtGrid = null;
        }

        private void cboControls_Fill(string sCurrentControlId)
        {
            int iOperationType = 3;
            DataSet dsOperationControls = new DataSet();
            StringReader strrControlXML = null;
            string xml = "";

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            if (objUtil.ErrCheck(dbu.GetOperationControls(sn.UserID, sn.SecId, iOperationType, ref xml), false))
                if (objUtil.ErrCheck(dbu.GetOperationControls(sn.UserID, sn.SecId, iOperationType, ref xml), true))
                {
                    this.cboParentControl.DataSource = null;
                    this.cboParentControl.DataBind();
                    return;
                }

            if (xml == "")
            {
                this.cboParentControl.DataSource = null;
                this.cboParentControl.DataBind();
                return;
            }

            strrControlXML = new StringReader(xml);
            dsOperationControls.ReadXml(strrControlXML);

            cboParentControl.DataSource = dsOperationControls;
            cboParentControl.DataBind();

            cboParentControl.Items.Insert(0, new ListItem(String.Empty, String.Empty));
            cboParentControl.SelectedIndex = 0;

            try
            {
                foreach (ListItem li in cboParentControl.Items)
                {
                    if (li.Value.ToString() == sCurrentControlId)
                    {
                        cboParentControl.Items.Remove(li);
                        return;
                    }
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string ControlLangNames = String.Empty;
            StringReader strrXML = null;
            string xml = "";
            DataSet dsLanguages = new DataSet();
            int FormID = 0;
            string ControlName = String.Empty;
            int ParentControlId = 0;

            try
            {
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                if (objUtil.ErrCheck(dbu.GetLanguages(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetLanguages(sn.UserID, sn.SecId, ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                    return;

                strrXML = new StringReader(xml);
                dsLanguages.ReadXml(strrXML);

                DataTable dt = dsLanguages.Tables[0];
                TextBox tb = new TextBox();
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        tb = ((TextBox)pnlControlNames.FindControl(GenericText + dr["LanguageID"].ToString()));
                        if (tb != null && !String.IsNullOrEmpty(tb.Text.Trim()))
                        {
                            sb.Append(dr["LanguageID"].ToString());
                            sb.Append(":");
                            sb.Append(tb.Text.Trim());
                            sb.Append(";");
                        }
                        if (Convert.ToInt32(dr["LanguageID"]) == 1)
                            ControlName = tb.Text.Trim();
                    }
                    dt.Dispose();

                    if (sb.Length > 0)
                        ControlLangNames = sb.ToString().Substring(0, sb.Length - 1);

                    if (!String.IsNullOrEmpty(cboForm.SelectedValue))
                        FormID = Convert.ToInt32(cboForm.SelectedValue);
                    if (!String.IsNullOrEmpty(cboParentControl.SelectedValue))
                        ParentControlId = Convert.ToInt32(cboParentControl.SelectedValue);

                    if (ControlId > 0)
                    {
                        int rowsAffected = dbu.UpdateControl(sn.UserID, sn.SecId, txtControlID.Text.Trim(), txtDescription.Text.Trim(), FormID, txtURL.Text.Trim(), chkIsActive.Checked, ControlLangNames, ControlId, ParentControlId);
                        if (rowsAffected == 0)
                        {
                            lblMsg.Text = String.Format((string)GetLocalResourceObject("ControlUpdatedMsg"), ControlName);
                            plhMsg.Visible = true;
                            dgControls_Fill();
                        }
                        else
                        {
                            lblMsg.Text = String.Format((string)GetLocalResourceObject("ErrorToUpdateControlMsg"), ControlName);
                            plhMsg.Visible = true;
                        }
                    }
                    else
                    {
                        ControlId = dbu.AddControl(sn.UserID, sn.SecId, txtControlID.Text.Trim(), txtDescription.Text.Trim(), FormID, txtURL.Text.Trim(), chkIsActive.Checked, ControlLangNames, ParentControlId);

                        if (ControlId > 0)
                        {
                            lblMsg.Text = String.Format((string)GetLocalResourceObject("NewControlCreatedMsg"), ControlName);
                            plhMsg.Visible = true;
                            lblCommandNumberValue.Text = ControlId.ToString();
                            plhCommandNumber.Visible = true;
                            cmdUpdate.Text = (string)GetLocalResourceObject("Update");
                            dgControls_Fill();
                        }
                        else
                        {
                            lblMsg.Text = String.Format((string)GetLocalResourceObject("ErrorToAddControlMsg"), ControlName);
                            plhMsg.Visible = true;
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

        protected void cmAddControl_Click(object sender, EventArgs e)
        {
            ControlId = 0;
            plhControls.Visible = false;
            plhControlForm.Visible = true;
            cboControls_Fill(ControlId.ToString());
            ResetFields();
            lblMsg.Text = String.Empty;
            plhMsg.Visible = false;
            cmdUpdate.Text = (string)GetLocalResourceObject("Add");
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            ControlId = -1;
            plhControls.Visible = true;
            plhControlForm.Visible = false;
            sn.Misc.ConfUsersSelectedGridPage = dgControls.CurrentPageIndex;
            if (sn.Misc.DtGrid != null)
            {
                dgControls.DataSource = sn.Misc.DtGrid;
                dgControls.DataBind();
            }
            else
            {
                dgControls_Fill();
                dgControls.SelectedIndex = -1;
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