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
using System.Text;
using VLF.CLS;
using Telerik.Web.UI;
using System.Drawing;
using VLF.DAS.Logic;
using System.Collections.Generic;
using VLF.DAS.DB;

namespace SentinelFM
{
    public partial class Configuration_CustomerReport_CustomerReportEmail : SentinelFMBasePage
    {
        TextBox txtEmails = null;
        RadComboBox cboFleet = null;
        DataSet dsEmails = null;
        Button btnSave = null;
        Button btnCancel = null;
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        public string Error_Load = "Failed to load data.";
        public string errorSave = "Save failed.";
        public string saveSucceed = "Saved Successfully.";
        public string showFilter = "Show Filter";
        public string hideFilter = "Hide Filter";
        public string deleteEmailText = "Are you sure you want to delete the email.";
        public string inputEmailText = "Please input email(s) and select vehicle(s).";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../../Login.aspx','_top')</SCRIPT>", false);
            }
            System.Web.HttpBrowserCapabilities browser = Request.Browser;

            if (browser.Browser.ToLower() == "ie")
            { 
                dgEmail.Columns.FindByUniqueName("EmailIE").Visible = true;
                dgEmail.Columns.FindByUniqueName("Email").Visible = false;
            }
            else
            {
                dgEmail.Columns.FindByUniqueName("EmailIE").Visible = false;
                dgEmail.Columns.FindByUniqueName("Email").Visible = true;
            }
        }

        private void GetControls(System.Web.UI.UserControl usercontrol)
        {
            txtEmails = (TextBox)usercontrol.FindControl("txtEmails");
            cboFleet = (RadComboBox)usercontrol.FindControl("cboFleet");
            btnSave = (Button)usercontrol.FindControl("btnSave");
            btnCancel = (Button)usercontrol.FindControl("btnCancel");
        }

        protected void dgEmail_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindEmails(false);
        }

        private void BindEmails(bool isBind)
        {
            try
            {
                CustomReportEmailManager customMg = new CustomReportEmailManager(sConnectionString);
                dsEmails = customMg.CustomReportEmail_Get(sn.User.OrganizationId, false);
                dgEmail.DataSource = dsEmails.Tables[0].DefaultView.ToTable(true, "CustomReportEmailId", "Email");
                if (isBind)
                {
                    dgEmail.DataBind();
                }
            }
            catch (Exception Ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("CustomReportEmailId");

                dgEmail.DataSource = dt;
                if (isBind) dgEmail.DataBind();

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                string errorMsg = string.Format("alert(\"{0}\");", Error_Load);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);

            }
        }

        protected void dgEmail_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditFormItem && e.Item.IsInEditMode)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
                System.Web.UI.UserControl editFormUserControl = (System.Web.UI.UserControl)
                    item.FindControl(GridEditFormItem.EditFormUserControlID);

                if (editFormUserControl != null)
                {
                    GetControls(editFormUserControl);
                    btnCancel.CommandName = "Cancel";
                    DataSet dsFleets = sn.User.GetUserFleets(sn);

                    cboFleet.DataSource = dsFleets;
                    cboFleet.DataBind();

                    if (e.Item.DataItem is DataRowView)
                    {
                        btnSave.CommandName = "Update";

                        string customReportEmailId = ((DataRowView)e.Item.DataItem)["CustomReportEmailId"].ToString();

                        foreach (RadComboBoxItem fleetItem in cboFleet.Items)
                        {
                            string strExpr = "CustomReportEmailId=" + customReportEmailId +
                                " and FleetId=" + fleetItem.Value;
                            DataRow[] drs = dsEmails.Tables[0].Select(strExpr);

                            if (drs.Length > 0)
                            {
                                CheckBox chkFleet = (CheckBox)fleetItem.FindControl("chkFleet");
                                if (chkFleet != null)
                                {
                                    chkFleet.Checked = true;
                                }
                            }
                        }
                        txtEmails.Text = ((DataRowView)e.Item.DataItem)["Email"].ToString();
                    }
                    else
                    {
                        btnSave.CommandName = "PerformInsert";
                    }
                }
            }

            if (e.Item.DataItem != null && e.Item.DataItem is DataRowView && e.Item is GridDataItem)
            {
                //Button  btnDeleteEmail = (Button)e.Item.FindControl("btnDeleteEmail");
                //if (btnDeleteEmail != null)
                //{
                    //btnDeleteEmail.OnClientClick = "return confirm('" + deleteEmailText + "')";
                //}
                RadGrid dgEmailFleet = (RadGrid)e.Item.FindControl("dgEmailFleet");
                if (dgEmailFleet != null)
                {
                    string strExpr = "CustomReportEmailId=" + ((GridDataItem)e.Item).GetDataKeyValue("CustomReportEmailId").ToString();
                    DataRow[] drs = dsEmails.Tables[0].Select(strExpr);
                    if (drs.Length == 0)
                    {
                        DataTable dt = dsEmails.Tables[0].Clone();
                        dt.Rows.Add(dt.NewRow());
                        dgEmailFleet.DataSource = dt;
                    }
                    else
                    {
                        dgEmailFleet.DataSource = drs;
                    }
                    dgEmailFleet.DataBind();
                }
            }



        }
        protected void dgEmail_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "DeleteEmail")
            {
                try
                {
                    long customReportEmailId = long.Parse(((GridDataItem)e.Item).GetDataKeyValue("CustomReportEmailId").ToString());
                    CustomReportEmailManager CRmgr = new CustomReportEmailManager(sConnectionString);
                    CRmgr.CustomReportEmail_Delete(customReportEmailId, sn.User.OrganizationId, sn.UserID);
                    BindEmails(true);
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                    ExceptionLogger(trace);
                    e.Canceled = true;
                    string errorMsg = string.Format("alert(\"{0}\");", errorSave);
                    RadAjaxManager1.ResponseScripts.Add(errorMsg);
                }

                
                return;

            }
            if (e.CommandName == "InitInsert" || e.CommandName == "Edit" || e.CommandName == "Delete")
            {
                if (e.CommandName == "Edit") dgEmail.MasterTableView.IsItemInserted = false;
                dgEmail.MasterTableView.ClearEditItems();
            }

            if (e.Item is GridEditFormItem && e.Item.IsInEditMode)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
                System.Web.UI.UserControl editFormUserControl = (System.Web.UI.UserControl)
                    item.FindControl(GridEditFormItem.EditFormUserControlID);
                if ((e.CommandName == "PerformInsert" || e.CommandName == "Update") && editFormUserControl != null)
                {
                    if (!Page.IsValid) return;
                    GetControls(editFormUserControl);
                    string emails = txtEmails.Text.Trim();
                    string fleets = string.Empty;
                    foreach (RadComboBoxItem fleetItem in cboFleet.Items)
                    {
                        CheckBox chkFleet = (CheckBox)fleetItem.FindControl("chkFleet");
                        if (chkFleet != null && chkFleet.Checked)
                        {
                            if (fleets == string.Empty) fleets = fleetItem.Value;
                            else fleets = fleets + "," + fleetItem.Value;
                        }

                    }


                    if (emails == string.Empty || fleets == string.Empty )
                    {
                            e.Canceled = true;
                            string errorMsg = string.Format("alert(\"{0}\");", inputEmailText);
                            RadAjaxManager1.ResponseScripts.Add(errorMsg);
                            return;
                    }
                    if (e.CommandName == "PerformInsert")
                    {
                        try
                        {
                            CustomReportEmailManager CRmgr = new CustomReportEmailManager(sConnectionString);
                            int hgi_userId = 0;
                            DataSet ds = CRmgr.CustomReportEmail_GetHGI_USER(sn.User.OrganizationId);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                hgi_userId = int.Parse(ds.Tables[0].Rows[0]["UserId"].ToString());
                            }
                            CRmgr.CustomReportEmail_Add(emails, sn.User.OrganizationId, fleets, hgi_userId, sn.UserID);
                        }
                        catch (Exception Ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                            ExceptionLogger(trace);
                            e.Canceled = true;
                            string errorMsg = string.Format("alert(\"{0}\");", errorSave);
                            RadAjaxManager1.ResponseScripts.Add(errorMsg);
                        }
                    }
                    if (e.CommandName == "Update")
                    {
                        try
                        {
                            long customReportEmailId = long.Parse(((GridEditFormItem)e.Item).GetDataKeyValue("CustomReportEmailId").ToString());
                            CustomReportEmailManager CRmgr = new CustomReportEmailManager(sConnectionString);
                            CRmgr.CustomReportEmail_Update(emails,customReportEmailId, sn.User.OrganizationId, fleets, sn.UserID);
                        }
                        catch (Exception Ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                            ExceptionLogger(trace);
                            e.Canceled = true;
                            string errorMsg = string.Format("alert(\"{0}\");", errorSave);
                            RadAjaxManager1.ResponseScripts.Add(errorMsg);
                        }

                    }
                }
            }
        }
}
}