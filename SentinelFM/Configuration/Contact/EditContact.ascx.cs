using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VLF.DAS.Logic;
using System.Configuration;
using System.Data;
namespace SentinelFM
{
public partial class Configuration_Contact_EditContact : System.Web.UI.UserControl
{
    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
    DataTable CommunicationType = null;
    public string selectCommunicationType = "Select Contact Type";
    public string errorSelectType = "Please select a contact type";
    public string ContactStr = "Contact information";
    public string errorEmail = "Invalid email address";
    public string saveError = "Save Failed.";
    public string succeedsave = "Saved Successfully.";
    public string companynameRequired = "Company name is required.";
    public string nameRequired = "First name or last name is required.";
    public int emergencyPhoneId = ContactManager.EmergencyPhoneId;
    public int emailId = ContactManager.EmailId;
    public string deleteInfo_tel = "This telephone number has been used in emergency plan(s). Are you sure you want to delete?";
    public string changeInfo_tel = "You can not change the contact type because this telephone number has been used in emergency plan(s).";
    public string deleteInfo_email = "This email address has been used in emergency plan(s). Are you sure you want to delete?";
    public string changeInfo_email = "You can not change the contact type because this email address has been used in emergency plan(s).";
    public string saveMessage = "You did not enter emergency phone, are you sure you want to save?";
    public string cboTimeZoneIninId = "85";
    protected void Page_Load(object sender, EventArgs e)
    {
            ContactManager contactMg = new ContactManager(sConnectionString);
            if (!IsPostBack)
            {
                DataSet ds = contactMg.GetCommunicationTypes();
                if (ds != null && ds.Tables.Count > 0) CommunicationType = ds.Tables[0];
                else
                {
                    CommunicationType.Columns.Add("CommunicationTypeName");
                    CommunicationType.Columns.Add("CommunicationTypeId");
                }
                DataRow dr = CommunicationType.NewRow();
                dr["CommunicationTypeName"] = selectCommunicationType;
                dr["CommunicationTypeId"] = -1;
                CommunicationType.Rows.InsertAt(dr, 0);
                ddlTypeName.DataSource = CommunicationType;
                ddlTypeName.DataBind();

                ddlTypeName.SelectedIndex = 0;


            }
            legendID.InnerText = ContactStr;
            chkIsCompany.Attributes.Add("onclick", "javascript:return CheckIsCompany()");
    }

}

}