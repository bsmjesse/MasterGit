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

    public partial class Configuration_Contact_frmEditContactPlan : SentinelFMBasePage
    {
        public int emergencyPhoneId = ContactManager.EmergencyPhoneId;
        public int emailId = ContactManager.EmailId;
        public string saveError = "Save Failed.";
        public string succeedsave = "Saved Successfully.";

        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        string telephoneTxt = "Telephone";
        string emailTxt = "Email";
        public string errorLoad = "Failed to load data.";
        string selectEmail = "Select a Email";
        public string priorityText = "Priority";

        public string selectAlreday = "You have selected the contact alreday.";
        ContactManager contactMsg = null;
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

                    cboTelephone.DataSource = contactMsg.GetContactCommunicationDataByOrganization(sn.User.OrganizationId, emergencyPhoneId);
                    cboTelephone.DataBind();

                    AddEmailsToComboBox();
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
            legendPhones.InnerText = telephoneTxt;
            legendEmails.InnerText = emailTxt;
        }

        private void AddEmailsToComboBox()
        {
            DataSet ds = contactMsg.GetContactCommunicationDataByOrganization(sn.User.OrganizationId, emailId);
            cboEmails.Items.Clear();
            if (ds != null && ds.Tables.Count > 0)
            {
                DataRow dr = ds.Tables[0].NewRow();
                dr["ContactCommunicationID"] = "-1";
                dr["Name"] = selectEmail;
                dr["CommunicationData"] = "";
                ds.Tables[0].Rows.InsertAt(dr,0); ;

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
                cboEmails.SelectedIndex = 0;

           }



        }

        [WebMethod]
        public static string GetCommunicationData(Int64 CommunicationID)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            ContactManager ContactMg = new ContactManager(sConnectionString);

            int emailId = 7;
            string strJSON = string.Empty;
            try
            {
                DataSet dsCommunication = ContactMg.GetCommunicationDataByID(CommunicationID, emailId, sn.User.OrganizationId);
                if (dsCommunication != null && dsCommunication.Tables.Count > 0 && dsCommunication.Tables[0].Rows.Count > 0)
                {
                    string[][] retArray = new string[dsCommunication.Tables[0].Rows.Count][];
                    int i = 0;
                    foreach (DataRow rs in dsCommunication.Tables[0].Rows)
                    {
                        string contactCommunicationID = string.Empty;
                        string communicationTypeId = string.Empty;
                        string communicationData = string.Empty;
                        if (!(rs["ContactCommunicationID"] is DBNull))
                            contactCommunicationID = rs["ContactCommunicationID"].ToString();
                        if (!(rs["CommunicationTypeId"] is DBNull))
                            communicationTypeId = rs["CommunicationTypeId"].ToString();
                        if (!(rs["CommunicationData"] is DBNull))
                            communicationData = rs["CommunicationData"].ToString();

                        retArray[i] = new string[] { contactCommunicationID, communicationTypeId, communicationData};
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

        [WebMethod]
        public static string ContactPlan_Add(string ContactPlanName, string ContactCommunicationIDs)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            ContactManager ContactMg = new ContactManager(sConnectionString);

            ContactCommunicationIDs = ContactCommunicationIDs.Trim();
            if (string.IsNullOrEmpty(ContactPlanName)) return "0";
            try
            {
                //Check if ContactCommunicationIDs in my Organization in case malicious ajax call 
                if (ContactCommunicationIDs != string.Empty) 
                {
                   if (!ContactMg.IsContactCommunicationIDsInOrganization(sn.User.OrganizationId, ContactCommunicationIDs)) return "0";
                }
                ContactMg.ContactPlan_Add(ContactPlanName, sn.User.OrganizationId, ContactCommunicationIDs);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: ContactInfo_Add()"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";
        }
    }
}