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

namespace SentinelFM
{
    public partial class Configuration_Contact_frmEditContact : SentinelFMBasePage
    {
        public string selectFleet = "Select a Fleet";
        public string selectCommunicationType = "Select Contact Type";
        public DataTable CommunicationType = null;
        public string ContactStr = "Contact information";
        public string errorEmail = "Invalid Email Address";
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        public string saveError = "Save failed.";
        public string saveSucceed = "Saved Successfully.";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../../Login.aspx','_top')</SCRIPT>", false);
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
            if (!IsPostBack)
            {
                FillControls();
                legendID.InnerText = ContactStr;
            }
        }

        private void FillControls()
        {
            DataSet dsFleets = sn.User.GetUserFleets(sn);
            cboFleet.DataSource = dsFleets;
            cboFleet.DataBind();
            cboFleet.Items.Insert(0, new RadComboBoxItem(selectFleet, "-1"));

            if (sn.User.DefaultFleet != -1)
            {
                cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindItemByValue(sn.User.DefaultFleet.ToString()));
                CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));
            }
            else
            {
                cboFleet.SelectedIndex = 1;
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedValue.ToString()));
            }

            ContactManager contactMg = new ContactManager(sConnectionString);
            DataSet ds = contactMg.GetCommunicationTypes();
            if (ds != null && ds.Tables.Count > 0) CommunicationType = ds.Tables[0];
            else
            {
                CommunicationType.Columns.Add("CommunicationTypeName");
                CommunicationType.Columns.Add("CommunicationTypeId");
            }
            DataTable dt = CommunicationType.Clone();
            DataRow dr = dt.NewRow();
            dr["CommunicationTypeName"] = "";
            dr["CommunicationTypeId"] = -1;
            dt.Rows.Add(dr);

            dr = CommunicationType.NewRow();
            dr["CommunicationTypeName"] = selectCommunicationType;
            dr["CommunicationTypeId"] = -1;
            CommunicationType.Rows.InsertAt(dr, 0);
            gdContact.DataSource = dt;
            gdContact.DataBind();
        }

        private void CboVehicle_Fill(int fleetId)
        {
            try
            {
                cboVehicle.Items.Clear();

                DataSet dsVehicle = new DataSet();

                string xml = "";

                using (ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet())
                {
                    if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                        if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            return;
                        }
                }

                if (String.IsNullOrEmpty(xml))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    return;
                }

                dsVehicle.ReadXml(new StringReader(xml));
                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();
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

        protected void gdContact_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.DataItem != null)
            {
                DropDownList ddlTypeName = (DropDownList)e.Item.FindControl("ddlTypeName");
                if (ddlTypeName != null)
                {
                    ddlTypeName.DataSource = CommunicationType;
                    ddlTypeName.DataBind();

                    DataRowView dv = (DataRowView)e.Item.DataItem;
                    ddlTypeName.SelectedValue = dv["CommunicationTypeId"].ToString();
                }
            }
        }
        protected void cboFleet_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (cboFleet.SelectedIndex > 0) CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedValue.ToString()));

        }

        [WebMethod]
        public static string Contact_Add(Int64 VehicleId, string FirstName,
         string MiddleName, string LastName, int TimeZone, object[] Contacts)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

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
                //ContactMg.Contact_Add(VehicleId, sn.User.OrganizationId, FirstName, MiddleName, LastName, TimeZone, contactSB.ToString(), sn.UserID);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: Contact_Add()"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1" ;
        }

}
}