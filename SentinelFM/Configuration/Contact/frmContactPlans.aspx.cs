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
using System.Web.Services;
using System.Web.Script.Serialization;

namespace SentinelFM
{

    public partial class Configuration_Contact_frmContactPlans : SentinelFMBasePage
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        ContactManager contactMsg = null;
        string telephoneTxt = "Telephone";
        string emailTxt = "Email";
        public int emergencyPhoneId = ContactManager.EmergencyPhoneId;
        public int emailId = ContactManager.EmailId;
        string selectEmail = "Select a Email";
        string selectTelephone = "Select a Telephone";
        public string errorLoad = "Failed to load data.";
        public string confirmDeletePlan = "Are you sure you want to delete the plan?";
        string errorDelete = "Delete Failed";
        string errorMove = "Move Failed";
        public string deleteUsedPlan = "You can not delete the plan because it has been assigned to driver.";
        public string saveError = "Save Failed.";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../../Login.aspx','_top')</SCRIPT>", false);
                }
                contactMsg = new ContactManager(sConnectionString);
                if (!IsPostBack)
                {
                    BindContactPlans();
                }
                legendPhones.InnerText = telephoneTxt;
                legendEmails.InnerText = emailTxt;

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }

        private void BindContactPlans()
        {
            for (int index = cboContactPlans.Items.Count - 1; index > 0; index--)
            {
                cboContactPlans.Items.Remove(cboContactPlans.Items[index]);
            }

            DataSet ds = contactMsg.GetOrganizationContactPlan(sn.User.OrganizationId);
            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (dr["ContactPlanName"] != DBNull.Value && dr["ContactPlanId"] != DBNull.Value)
                    {
                        cboContactPlans.Items.Add(new RadComboBoxItem(dr["ContactPlanName"].ToString(), dr["ContactPlanId"].ToString()));
                    }
                }
                //                        cboContactPlans.DataSource = contactMsg.GetOrganizationContactPlan(sn.User.OrganizationId).Tables[0];
                //                      cboContactPlans.DataBind();
            }
            cboContactPlans.SelectedIndex = 0;
        }


        protected void cboContactPlans_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            try
            {
                if (cboContactPlans.SelectedIndex <= 0)
                {
                    pnlPhones.Visible = false;
                    pnlEmails.Visible = false;
                    btnDeletePlan.Visible = false;
                }
                else
                {
                    BindTelephones();
                    BindEmails();
                    if (contactMsg.IsContactPlanInUsed(int.Parse(cboContactPlans.SelectedValue)))
                    {
                        btnDeletePlan.OnClientClick = "javascript:alert('" + deleteUsedPlan + "'); return false;";

                    }
                    else
                    {
                        btnDeletePlan.OnClientClick = "javascript:if (!ConfirmDeletePlan()) return false";
                    }

                    btnDeletePlan.Visible = true;
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
                string errorMsg = string.Format("alert(\"{0}\");", errorLoad);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);
            }
        }

        private void BindEmails()
        {
            gdEmails.DataSource = contactMsg.GetPlanCommunicationDataByContactPlanId(int.Parse(cboContactPlans.SelectedValue), emailId);
            gdEmails.DataBind();
            

            pnlEmails.Visible = true;
        }

        private void BindTelephones()
        {
            DataSet ds = contactMsg.GetPlanCommunicationDataByContactPlanId(int.Parse(cboContactPlans.SelectedValue), emergencyPhoneId);
            gdPhones.DataSource = ds;
            gdPhones.DataBind();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count >= 2)
            {
                gdPhones.Columns.FindByUniqueName("upDownArrow").Visible = true;
            }
            else gdPhones.Columns.FindByUniqueName("upDownArrow").Visible = false;
            pnlPhones.Visible = true;
        }

        protected void btnDeletePlan_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboContactPlans.SelectedIndex > 0)
                {
                    contactMsg.ContactPlan_Delete(int.Parse(cboContactPlans.SelectedValue));
                    cboContactPlans.Items.Remove(cboContactPlans.SelectedItem);
                    cboContactPlans.SelectedIndex = 0;
                    cboContactPlans_SelectedIndexChanged(null, null);
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
                string errorMsg = string.Format("alert(\"{0}\");", errorDelete);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);
            }


        }
        protected void gdPhones_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            if (((GridDataItem)e.Item).GetDataKeyValue("ContactCommunicationID") != null && cboContactPlans.SelectedIndex > 0)
            {
                try
                {
                    Int64 contactCommunicationID = Int64.Parse(((GridDataItem)e.Item).GetDataKeyValue("ContactCommunicationID").ToString());
                    contactMsg.ContactPlanCommunications_Delete(int.Parse(cboContactPlans.SelectedValue), contactCommunicationID);
                    BindTelephones();
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                    ExceptionLogger(trace);
                    e.Canceled = true;
                    string errorMsg = string.Format("alert(\"{0}\");", errorDelete);
                    RadAjaxManager1.ResponseScripts.Add(errorMsg);
                }
            }

        }
        protected void gdEmails_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            if (((GridDataItem)e.Item).GetDataKeyValue("ContactCommunicationID") != null && cboContactPlans.SelectedIndex > 0)
            {
                try
                {
                    Int64 contactCommunicationID = Int64.Parse(((GridDataItem)e.Item).GetDataKeyValue("ContactCommunicationID").ToString());
                    contactMsg.ContactPlanCommunications_Delete(int.Parse(cboContactPlans.SelectedValue), contactCommunicationID);




                    gdEmails.DataSource = contactMsg.GetPlanCommunicationDataByContactPlanId(int.Parse(cboContactPlans.SelectedValue), emailId);
                    gdEmails.DataBind();

                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                    ExceptionLogger(trace);
                    e.Canceled = true;
                    string errorMsg = string.Format("alert(\"{0}\");", errorDelete);
                    RadAjaxManager1.ResponseScripts.Add(errorMsg);
                }
            }
        }
        protected void gdEmails_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.CommandItem)
            {
                RadComboBox cboEmails = (RadComboBox)e.Item.FindControl("cboEmails");
                if (cboEmails == null) return;
                DataSet ds = contactMsg.GetContactCommunicationDataByOrganization(sn.User.OrganizationId, emailId, int.Parse(cboContactPlans.SelectedValue));
                cboEmails.Items.Clear();
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataRow dr = ds.Tables[0].NewRow();
                    dr["ContactCommunicationID"] = "-1";
                    dr["Name"] = selectEmail;
                    dr["CommunicationData"] = "";
                    ds.Tables[0].Rows.InsertAt(dr, 0); ;

                    foreach (DataRow dataRow in ds.Tables[0].Rows)
                    {
                        RadComboBoxItem item = new RadComboBoxItem();
                        if (!(dataRow["Name"] is DBNull))
                        {
                            item.Text = dataRow["Name"].ToString();
                        }
                        else item.Text = "";

                        if (!(dataRow["ContactCommunicationID"] is DBNull))
                        {
                            item.Value = dataRow["ContactCommunicationID"].ToString();
                        }
                        else item.Value = "";

                        item.Attributes.Add("Name", item.Text);

                        if (!(dataRow["CommunicationData"] is DBNull))
                            item.Attributes.Add("CommunicationData", dataRow["CommunicationData"].ToString());
                        else item.Attributes.Add("CommunicationData", "");
                        cboEmails.Items.Add(item);
                        item.DataBind();
                    }
                }
                cboEmails.SelectedIndex = 0;
            }
        }
        protected void gdPhones_ItemDataBound(object sender, GridItemEventArgs e)
        {
                Label lblPriority = (Label)e.Item.FindControl("lblPriority");
                if (lblPriority != null)
                {
                    lblPriority.Text = lblPriority.Text + " " + (e.Item.ItemIndex + 1).ToString();
                }
                ImageButton btnUpArrow = (ImageButton)e.Item.FindControl("btnUpArrow");
                ImageButton btnDownArrow = (ImageButton)e.Item.FindControl("btnDownArrow");
                if (btnUpArrow != null && e.Item.ItemIndex != 0) btnUpArrow.Visible = true;
                
                if (btnDownArrow != null)
                {
                    if (e.Item.ItemIndex != ((DataSet)gdPhones.DataSource).Tables[0].Rows.Count - 1) btnDownArrow.Visible = true;
                }
            if (e.Item.ItemType == GridItemType.CommandItem)
            {
                RadComboBox cboTels = (RadComboBox)e.Item.FindControl("cboTels");
                if (cboTels == null) return;
                DataSet ds = contactMsg.GetContactCommunicationDataByOrganization(sn.User.OrganizationId, emergencyPhoneId, int.Parse(cboContactPlans.SelectedValue));

                cboTels.Items.Clear();
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataRow dr = ds.Tables[0].NewRow();
                    dr["ContactCommunicationID"] = "-1";
                    dr["Name"] = selectTelephone;
                    dr["CommunicationData"] = "";
                    ds.Tables[0].Rows.InsertAt(dr, 0); ;

                    foreach (DataRow dataRow in ds.Tables[0].Rows)
                    {
                        RadComboBoxItem item = new RadComboBoxItem();
                        if (!(dataRow["Name"] is DBNull))
                        {
                            item.Text = dataRow["Name"].ToString();
                        }
                        else item.Text = "";

                        if (!(dataRow["ContactCommunicationID"] is DBNull))
                        {
                            item.Value = dataRow["ContactCommunicationID"].ToString();
                        }
                        else item.Value = "";

                        item.Attributes.Add("Name", item.Text);

                        if (!(dataRow["CommunicationData"] is DBNull))
                            item.Attributes.Add("CommunicationData", dataRow["CommunicationData"].ToString());
                        else item.Attributes.Add("CommunicationData", "");
                        cboTels.Items.Add(item);
                        item.DataBind();

                    }
                }
                cboTels.SelectedIndex = 0;
            }
        }
        protected void btnAddTele_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboContactPlans.SelectedIndex <= 0) return;

                int contactPlanId = int.Parse(cboContactPlans.SelectedValue);
                GridCommandItem cmdItem = (GridCommandItem)gdPhones.MasterTableView.GetItems(GridItemType.CommandItem)[0];
                RadComboBox cboTels = (RadComboBox)cmdItem.FindControl("cboTels");
                if (cboTels == null) return;
                string contactCommunicationIDs = "";
                foreach (RadComboBoxItem cboItem in cboTels.Items)
                {
                    CheckBox chkCommunicationData = (CheckBox)cboItem.FindControl("chkCommunicationData");
                    if (chkCommunicationData != null && chkCommunicationData.Checked && !string.IsNullOrEmpty(cboItem.Value))
                    {
                        if (contactCommunicationIDs == "") contactCommunicationIDs = cboItem.Value;
                        else contactCommunicationIDs = contactCommunicationIDs + "," + cboItem.Value;
                    }
                }
                if (contactCommunicationIDs != "") contactMsg.ContactPlanCommunications_Add(contactPlanId, contactCommunicationIDs);
                BindTelephones();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
                string errorMsg = string.Format("alert(\"{0}\");", saveError);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);
            }
        }
        protected void btnAddEmail_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboContactPlans.SelectedIndex <= 0) return;

                int contactPlanId = int.Parse(cboContactPlans.SelectedValue);
                GridCommandItem cmdItem = (GridCommandItem)gdEmails.MasterTableView.GetItems(GridItemType.CommandItem)[0];
                RadComboBox cboEmails = (RadComboBox)cmdItem.FindControl("cboEmails");
                if (cboEmails == null) return;
                string contactCommunicationIDs = "";
                foreach (RadComboBoxItem cboItem in cboEmails.Items)
                {
                    CheckBox chkCommunicationData = (CheckBox)cboItem.FindControl("chkCommunicationData");
                    if (chkCommunicationData != null && chkCommunicationData.Checked && !string.IsNullOrEmpty(cboItem.Value))
                    {
                        if (contactCommunicationIDs == "") contactCommunicationIDs = cboItem.Value;
                        else contactCommunicationIDs = contactCommunicationIDs + "," + cboItem.Value;
                    }
                }
                if (contactCommunicationIDs != "") contactMsg.ContactPlanCommunications_Add(contactPlanId, contactCommunicationIDs);
                BindEmails();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
                string errorMsg = string.Format("alert(\"{0}\");", saveError);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);
            }

        }
        protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            if (e.Argument == "RebindPlans")
            {
                string selectedValue = string.Empty;
                if (cboContactPlans.SelectedIndex > 0) selectedValue = cboContactPlans.SelectedValue;
                BindContactPlans();
                if (selectedValue != string.Empty)
                {
                    RadComboBoxItem cboItem = cboContactPlans.Items.FindItemByValue(selectedValue);
                    if (cboItem == null )
                    {
                        cboContactPlans.SelectedIndex = 0;
                        cboContactPlans_SelectedIndexChanged(null, null);
                    }
                    else cboItem.Selected = true;
                }
                
            }
        }

        protected void gdPhones_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "UpPriority")
            {
                if (((GridDataItem)e.Item).GetDataKeyValue("ContactCommunicationID") != null && cboContactPlans.SelectedIndex > 0 && e.Item.ItemIndex != 0)
                {
                    try
                    {
                        Int64 contactCommunicationID1 = Int64.Parse(((GridDataItem)e.Item).GetDataKeyValue("ContactCommunicationID").ToString());
                        Int64 contactCommunicationID2 = Int64.Parse(((GridDataItem)gdPhones.MasterTableView.Items[e.Item.ItemIndex - 1]).GetDataKeyValue("ContactCommunicationID").ToString());



                        contactMsg.ContactPlanCommunicationsExchange_Priority(int.Parse(cboContactPlans.SelectedValue), contactCommunicationID1, contactCommunicationID2);
                        BindTelephones();
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                        ExceptionLogger(trace);
                        e.Canceled = true;
                        string errorMsg = string.Format("alert(\"{0}\");", errorMove);
                        RadAjaxManager1.ResponseScripts.Add(errorMsg);
                    }
                }
            }

            if (e.CommandName == "DownPriority")
            {
                int itemCount = gdPhones.Items.Count;
                if (((GridDataItem)e.Item).GetDataKeyValue("ContactCommunicationID") != null && cboContactPlans.SelectedIndex > 0 && e.Item.ItemIndex != itemCount -  1)
                {
                    try
                    {
                        Int64 contactCommunicationID1 = Int64.Parse(((GridDataItem)e.Item).GetDataKeyValue("ContactCommunicationID").ToString());
                        Int64 contactCommunicationID2 = Int64.Parse(((GridDataItem)gdPhones.MasterTableView.Items[e.Item.ItemIndex + 1]).GetDataKeyValue("ContactCommunicationID").ToString());


                        contactMsg.ContactPlanCommunicationsExchange_Priority(int.Parse(cboContactPlans.SelectedValue), contactCommunicationID1, contactCommunicationID2);
                        BindTelephones();
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                        ExceptionLogger(trace);
                        e.Canceled = true;
                        string errorMsg = string.Format("alert(\"{0}\");", errorMove);
                        RadAjaxManager1.ResponseScripts.Add(errorMsg);
                    }
                }

            }

        }
}
}