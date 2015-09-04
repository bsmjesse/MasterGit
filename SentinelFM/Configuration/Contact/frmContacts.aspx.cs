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
    public partial class Configuration_Contact_frmContacts : SentinelFMBasePage
    {
        public string showFilter = "Show Filter";
        public string hideFilter = "Hide Filter";
        string errorDelete = "Delete Failed";
        public string errorLoad = "Failed to load data.";
        public string contactInfo_del_used = "This contact has been used in emergency plan(s). Are you sure you want to delete?";
        public string contactInfo_del = "Delete this contact?";
        public string cboTimeZoneIninId = "85";
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        ContactManager contactManager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            contactManager = new ContactManager(sConnectionString);
            try
            {
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../../Login.aspx','_top')</SCRIPT>", false);
                }
                if (!IsPostBack)
                {
                    RadComboBox cboTimeZone = (RadComboBox)EditContact1.FindControl("cboTimeZone");
                    cboTimeZone.Items.Clear();
                    DataView dv = contactManager.GetTimeZones().Tables[0].DefaultView;
                    dv.Sort = "Id";
                    cboTimeZone.DataSource = dv;
                    cboTimeZone.DataTextField = "DisplayName";
                    cboTimeZone.DataValueField = "Id";

                    cboTimeZone.DataBind();
                    RadComboBoxItem comboItem = cboTimeZone.Items.FindItemByValue(cboTimeZoneIninId);
                    if (comboItem != null )
                        comboItem.Selected = true; 
                    else cboTimeZone.SelectedIndex = 0;
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
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }

            try
            {
                showFilter = HttpContext.GetGlobalResourceObject("Const", "RadGrid_ShowFilter").ToString();
                hideFilter = HttpContext.GetGlobalResourceObject("Const", "RadGrid_HideFilter").ToString(); 
            }
            catch { };
        }

        private DataSet GetContacts()
        {
            DataSet ds = new DataSet();
            ds = contactManager.GetOrganizationContacts(sn.User.OrganizationId);
            return ds;
        }

        protected void gdVehicleContact_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            gdVehicleContact.DataSource = GetContacts();
        }
        protected void gdVehicleContact_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.DataItem != null && e.Item.DataItem is DataRowView)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                RadGrid gdContact = (RadGrid)e.Item.FindControl("gdContact");
                Label lblTimeZone = (Label)e.Item.FindControl("lblTimeZone");
                ImageButton imgEdit = (ImageButton)e.Item.FindControl("imgEdit");
                ImageButton imgDelete = (ImageButton)e.Item.FindControl("imgDelete");
                if (gdContact != null) gdContact.Visible = false;
                Literal ltlNone = (Literal)e.Item.FindControl("ltlNone");
                if (ltlNone != null) ltlNone.Visible = true;
                if (imgEdit != null)
                {
                    imgEdit.OnClientClick = string.Format("javascript:return OpenEdit({0})", e.Item.ItemIndex);
                }
                if(imgDelete != null)
                {
                    imgDelete.OnClientClick = string.Format("javascript:if (!confirmDelete({0})) return false;", e.Item.ItemIndex);
                }

                if (!(drv["ContactInfoId"] is DBNull))
                {
                    DataSet ds = contactManager.GetVehicleContactCommunicationsByContactId(Int64.Parse(drv["ContactInfoId"].ToString()));
                    if (ds != null && ds.Tables[0].Rows.Count > 0 && gdContact != null)
                    {
                        gdContact.Visible = true;
                        gdContact.DataSource = ds.Tables[0];
                        gdContact.DataBind();
                        if (ltlNone != null) ltlNone.Visible = false;
                    }
                }
                RadComboBox cboTimeZone = (RadComboBox)EditContact1.FindControl("cboTimeZone");
                if (!(drv["TimeZone"] is DBNull) && lblTimeZone != null && cboTimeZone!= null)
                {
                    RadComboBoxItem cboItem = cboTimeZone.Items.FindItemByValue(drv["TimeZone"].ToString());
                    if (cboItem != null)
                    {
                        lblTimeZone.Text = cboItem.Text;
                    }
                    else lblTimeZone.Text = "&nbsp;";

                }
            }
        }

        protected void gdContact_ItemDataBound(object sender, GridItemEventArgs e)
        {
            Label lblTypeData = (Label)e.Item.FindControl("lblTypeData");
            if (lblTypeData != null)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                if (drv != null)
                {
                    if (drv["CommunicationTypeId"] != DBNull.Value &&
                        drv["CommunicationTypeId"].ToString() == ContactManager.EmergencyPhoneId.ToString())
                    {
                        lblTypeData.CssClass = "emergencyStyle";
                    }
                }
            }
        }
        //protected void gdVehicleContact_DeleteCommand(object sender, GridCommandEventArgs e)
        //{
        //    if (((GridDataItem)e.Item).GetDataKeyValue("ContactInfoId") != null)
        //    {
        //        try
        //        {
        //            Int64 ContactInfoId = int.Parse(((GridDataItem)e.Item).GetDataKeyValue("ContactInfoId").ToString());

        //            int ret = contactManager.ContactInfo_Delete(ContactInfoId);
        //        }
        //        catch (Exception Ex)
        //        {
        //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        //            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
        //            ExceptionLogger(trace);
        //            e.Canceled = true;
        //            string errorMsg = string.Format("alert(\"{0}\");", errorDelete);
        //            RadAjaxManager1.ResponseScripts.Add(errorMsg);
        //        }
        //    }

        //}

        [WebMethod]
        public static string ContactInfo_Update(Int64 ContactInfoId, Boolean isCompany, string Company, string FirstName,
         string MiddleName, string LastName, int TimeZone, string DeletedIds, object[] Contacts)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";
            Company = Company.Trim();
            FirstName = FirstName.Trim();
            MiddleName = MiddleName.Trim();
            LastName = LastName.Trim();
            if (isCompany)
            {
                FirstName = "";
                MiddleName = "";
                LastName = "";
            }
            else Company = "";

            if (Company == string.Empty &&
                FirstName == string.Empty &&
                LastName == string.Empty)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Invalid entry. " + " User:" + sn.UserID.ToString() + "Web method: ContactInfo_Update()"));
                return "0";
            }

            try
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                StringBuilder contactSB = new StringBuilder();

                if (Contacts.Length > 0)
                {
                    contactSB.Append("<ROOT>");
                    foreach (object contactInfo in Contacts)
                    {
                        Dictionary<string, object> dicValues = new Dictionary<string, object>();
                        dicValues = (Dictionary<string, object>)contactInfo;
                        contactSB.Append(
                           string.Format("<Contact><Id>{0}</Id><TypeId>{1}</TypeId><TypeData>{2}</TypeData></Contact>",
                           dicValues["Id"].ToString(), dicValues["TypeId"].ToString(), dicValues["TypeData"].ToString())
                            );
                    }
                    contactSB.Append("</ROOT>");
                }
                DeletedIds = DeletedIds.Trim();
                ContactManager ContactMg = new ContactManager(sConnectionString);
                ContactMg.ContactInfo_Update(ContactInfoId, isCompany, Company,
                    FirstName, MiddleName, LastName, TimeZone, DeletedIds, contactSB.ToString(), sn.User.OrganizationId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: ContactInfo_Update()"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";
        }

        [WebMethod]
        public static string ContactInfo_Add(Boolean isCompany,  string Company, string FirstName,
         string MiddleName, string LastName, int TimeZone, object[] Contacts)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            Company = Company.Trim();
            FirstName = FirstName.Trim();
            MiddleName = MiddleName.Trim();
            LastName = LastName.Trim();
            if (isCompany)
            {
                FirstName = "";
                MiddleName = "";
                LastName = "";
            }
            else Company = "";
            if (Company == string.Empty &&
                FirstName == string.Empty &&
                LastName == string.Empty)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,  "Invalid entry."  + " User:" + sn.UserID.ToString() + "Web method: ContactInfo_Add() Invalid entry. "));
                return "0";
            }

            try
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                StringBuilder contactSB = new StringBuilder();

                if (Contacts.Length > 0)
                {
                    contactSB.Append("<ROOT>");
                    foreach (object contactInfo in Contacts)
                    {
                        Dictionary<string, object> dicValues = new Dictionary<string, object>();
                        dicValues = (Dictionary<string, object>)contactInfo;
                        contactSB.Append(
                           string.Format("<Contact><TypeId>{0}</TypeId><TypeData>{1}</TypeData></Contact>",
                           dicValues["TypeId"].ToString(), dicValues["TypeData"].ToString())
                            );
                    }
                    contactSB.Append("</ROOT>");
                }
                ContactManager ContactMg = new ContactManager(sConnectionString);
                ContactMg.ContactInfo_Add(sn.User.OrganizationId, isCompany,  Company,
                    FirstName, MiddleName, LastName, TimeZone, contactSB.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: ContactInfo_Add()" ));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";
        }

        [WebMethod]
        public static string GetCommunications(Int64 ContactId)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            ContactManager ContactMg = new ContactManager(sConnectionString);

            string strJSON = string.Empty;
            try
            {
                DataSet dsCommunication = ContactMg.GetVehicleContactCommunicationsByContactIdOrgID(ContactId, sn.User.OrganizationId);
                if (dsCommunication != null && dsCommunication.Tables.Count > 0 && dsCommunication.Tables[0].Rows.Count > 0)
                {
                    string[][] retArray = new string[dsCommunication.Tables[0].Rows.Count][];
                    int i = 0;
                    foreach (DataRow rs in dsCommunication.Tables[0].Rows)
                    {
                        string contactCommunicationID = string.Empty;
                        string communicationTypeId = string.Empty;
                        string communicationData = string.Empty;
                        string isUsed = "0";
                        if (!(rs["ContactCommunicationID"] is DBNull))
                            contactCommunicationID = rs["ContactCommunicationID"].ToString();
                        if (!(rs["CommunicationTypeId"] is DBNull))
                            communicationTypeId = rs["CommunicationTypeId"].ToString();
                        if (!(rs["CommunicationData"] is DBNull))
                            communicationData = rs["CommunicationData"].ToString();
                        if (!(rs["UsedContactCommunicationID"] is DBNull)) isUsed = "1";


                        retArray[i] = new string[] { contactCommunicationID, communicationTypeId, communicationData, isUsed };
                        i = i + 1;
                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    strJSON = js.Serialize(retArray);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: ContactInfo_Add()"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }

            return strJSON;

        }


        protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            if (e.Argument == "Rebind")
            {
                //gdVehicleContact.MasterTableView.SortExpressions.Clear();
                //gdVehicleContact.MasterTableView.GroupByExpressions.Clear();
                gdVehicleContact.Rebind();
            }
            if (e.Argument.Substring(0, 6) == "Delete")
            {
                try
                {
                    Int64 ContactInfoId = Int64.Parse(e.Argument.Substring(7));

                    int ret = contactManager.ContactInfo_Delete(ContactInfoId);
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
            gdVehicleContact.Rebind();
        }

         [WebMethod]
        public static string IsContactInUsed(Int64 ContactInfoId)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";
            string ret = "";
            try
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                ContactManager ContactMg = new ContactManager(sConnectionString);
                ret = ContactMg.IsContactInUsed(ContactInfoId).ToString().ToLower();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: ContactInfo_Update()"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return ret;
        }
}
}