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
    public partial class Configuration_Equipment_frmAssignment : SentinelFMBasePage
    {
        public string selectFleet = "Select a Fleet";
        public string selectEquipment = "Select a Equipment";
        public string entireMedia = "Entire Media";
        public string entireFleet = "Entire Fleet";
        string errorInsert = "Save Failed.";
        string saveSucceed = "Saved Successfully.";
        int entireValue = -1;
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
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
                FillComboBox();
            }
        }

        private void FillComboBox()
        {
            DataSet dsFleets = sn.User.GetUserFleets(sn);
            cboFleet.DataSource = dsFleets;
            cboFleet.DataBind();
            cboFleet.Items.Insert(0, new RadComboBoxItem(selectFleet, "-1"));
            Boolean hasPassValue = false;
            if (Request.QueryString["f"] != null)
            {
                try
                {
                    string qysFleetID = Request.QueryString["f"];
                    cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindItemByValue(qysFleetID));
                    CboVehicle_Fill(Convert.ToInt32(qysFleetID));
                    hasPassValue = true;
                }
                catch {  }
            }
            if (!hasPassValue)
            {
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
            }

            cboEquipment.DataSource = GetOrganizationEquipments();
            cboEquipment.DataBind();
            cboEquipment.Text = string.Empty;
            cboMedia.DataSource = GetOrganizationMedias();
            cboMedia.DataBind();
            cboMedia.Text = string.Empty;
            valReqcboEquipment.InitialValue = selectEquipment;
        }

        private DataTable GetOrganizationEquipments()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("EquipmentId");
            dt.Columns.Add("Description");
            EquipmentManager equipmentMg = new EquipmentManager(sConnectionString);
            DataSet ds = equipmentMg.GetOrganizationEquipments(sn.User.OrganizationId);
            if (!ds.Equals(null) && ds.Tables.Count > 0)
            {
                ds.Tables[0].DefaultView.Sort = "Description";
                dt = ds.Tables[0].DefaultView.ToTable();
                DataRow dr = dt.NewRow();
                dr["Description"] = selectEquipment;
                dr["EquipmentId"] = "0";
                dt.Rows.InsertAt(dr, 0);

            }

            return dt;
        }

        private DataTable GetOrganizationMedias()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MediaId");
            dt.Columns.Add("Description");
            MediaManager mediaMg = new MediaManager(sConnectionString);
            DataSet ds = mediaMg.GetOrganizationMedias(sn.User.OrganizationId, sn.UserID);
            if (!ds.Equals(null) && ds.Tables.Count > 0)
            {
                ds.Tables[0].DefaultView.Sort = "Description";
                dt = ds.Tables[0].DefaultView.ToTable();

            }
            DataRow dr = dt.NewRow();
            dr["Description"] = entireMedia;
            dr["MediaId"] = entireValue;
            dt.Rows.InsertAt(dr, 0);
            return dt;
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

                DataRow dr = dsVehicle.Tables[0].NewRow();
                dr["Description"] = entireFleet;
                dr["VehicleID"] = entireValue;
                dsVehicle.Tables[0].Rows.InsertAt(dr, 0);
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
        protected void cboFleet_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (cboFleet.SelectedIndex > 0) CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedValue.ToString()));
            RadAjaxManager1.ResponseScripts.Add("SetCheckBox()");
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                List<string> vehicleIDs = GetCheckedVehicle();
                List<string> mediaIDs = GetCheckedMedia();
                string equipment = cboEquipment.SelectedValue;
                if (string.IsNullOrEmpty(equipment) || mediaIDs.Count <= 0 || vehicleIDs.Count <= 0) return;
                if (equipment.Trim() == string.Empty) return;
                StringBuilder equipmentMedias = new StringBuilder();
                StringBuilder vehicleIDssb = new StringBuilder();
                foreach (string mediaID in mediaIDs)
                {
                    if (string.IsNullOrEmpty(mediaID)) continue;
                    if (mediaID.Trim() == string.Empty ) continue;
                    if (equipmentMedias.Length == 0)
                        equipmentMedias.Append(string.Format("{0}#{1}", equipment, mediaID));
                    else
                        equipmentMedias.Append(string.Format(",{0}#{1}", equipment, mediaID));
                }
                foreach (string vehicleID in vehicleIDs)
                {
                    if (string.IsNullOrEmpty(vehicleID)) continue;
                    if (vehicleID.Trim() == string.Empty) continue;

                    if (vehicleIDssb.Length == 0)
                        vehicleIDssb.Append(string.Format("{0}", vehicleID));
                    else
                        vehicleIDssb.Append(string.Format(",{0}", vehicleID));
                }
                if (equipmentMedias.ToString() != string.Empty && vehicleIDssb.ToString() != string.Empty)
                {
                    try
                    {
                        VehicleEquipmentAssignmentManager vehicleEquipmentAssignmentManager = new VehicleEquipmentAssignmentManager(sConnectionString);
                        string unSavedVehicleIDs = vehicleEquipmentAssignmentManager.AddVehicleEquipmentAssignment(vehicleIDssb.ToString(),
                            equipmentMedias.ToString());
                        if (!string.IsNullOrEmpty(unSavedVehicleIDs))
                        {
                            string unSavedVehicleNames = GetunSavedVehicleNames(unSavedVehicleIDs);
                            if (!string.IsNullOrEmpty(unSavedVehicleNames))
                            {
                                lblMessage.Text = unSavedVehicleNames + " " + errorInsert;
                                lblMessage.ForeColor = Color.Red;
                                return;
                            }
                        }
                        cboEquipment.SelectedIndex = -1;
                        ClearCheckedVehicle();
                        ClearCheckedMedia();
                        //string msg = string.Format("alert(\"{0}\");", saveSucceed);
                        lblMessage.ForeColor = Color.Green;
                        lblMessage.Text = saveSucceed;
                        
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                        ExceptionLogger(trace);
                        string errorMsg = string.Format("alert(\"{0}\");", errorInsert);
                        RadAjaxManager1.ResponseScripts.Add(errorMsg);
                    }
                }
            }
            RadAjaxManager1.ResponseScripts.Add("SetCheckBox()");
        }
        public void ServerValidateMedia(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (GetCheckedMedia().Count > 0) args.IsValid = true;
                else args.IsValid = false;
            }
            catch 
            {
                args.IsValid = false;
            }
        }

        public void ServerValidateVehicle(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (GetCheckedVehicle().Count > 0) args.IsValid = true;
                else args.IsValid = false;
            }
            catch 
            {
                args.IsValid = false;
            }
        }

        private string GetunSavedVehicleNames(string unSavedVehicleIds)
        {
            unSavedVehicleIds = "," + unSavedVehicleIds + ",";
            StringBuilder sb = new StringBuilder();
            foreach (RadComboBoxItem item in cboVehicle.Items)
            {
                if (unSavedVehicleIds.Contains("," + item.Value + ","))
                {
                    if (sb.Length == 0) sb.Append(item.Text);
                    else sb.Append("," + item.Text);
                }
            }
            return sb.ToString();
        }

        private List<string> GetCheckedVehicle()
        {
            List<string> vehicleIDs = new List<string>();
            foreach (RadComboBoxItem item in cboVehicle.Items)
            {
                CheckBox chkVehicle = (CheckBox)item.FindControl("chkVehicle");
                if (chkVehicle != null && chkVehicle.Checked)
                {
                    if (item.Value != entireValue.ToString()) vehicleIDs.Add(item.Value);
                }
            }
            return vehicleIDs;
        }
        private List<string> GetCheckedMedia()
        {
            List<string> mediaIDs = new List<string>();
            foreach (RadComboBoxItem item in cboMedia.Items)
            {
                CheckBox chkMedia = (CheckBox)item.FindControl("chkMedia");
                if (chkMedia != null && chkMedia.Checked)
                {
                    if (item.Value != entireValue.ToString()) mediaIDs.Add(item.Value);
                }
            }
            return mediaIDs;
        }

        private void ClearCheckedVehicle()
        {
            foreach (RadComboBoxItem item in cboVehicle.Items)
            {
                CheckBox chkVehicle = (CheckBox)item.FindControl("chkVehicle");
                if (chkVehicle != null && chkVehicle.Checked)
                {
                    chkVehicle.Checked = false;
                }
            }
        }
        private void ClearCheckedMedia()
        {
            foreach (RadComboBoxItem item in cboMedia.Items)
            {
                CheckBox chkMedia = (CheckBox)item.FindControl("chkMedia");
                if (chkMedia != null && chkMedia.Checked)
                {
                    chkMedia.Checked = false;
                }
            }
        }


}
}