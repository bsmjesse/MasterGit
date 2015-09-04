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
    public partial class Maintenance_AddVehicleMaintenance : SentinelFM
    {
        public string selectFleet = "Select a Fleet";
        public string selectGroup = "Select a MCC Group";
        string errorInsert = "Save Failed.";
        string saveSucceed = "Saved Successfully.";
        public string entireFleet = "Entire Fleet";
        int entireValue = -1;
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
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
                MCCManager mccMgr = new MCCManager(sConnectionString);
                combMccGroup.DataSource = mccMgr.GetOrganizationMCCGroup(sn.User.OrganizationId);
                combMccGroup.DataBind();
                combMccGroup.Items.Insert(0, new RadComboBoxItem(selectGroup, "-1"));
            }
            try
            {
                errorInsert = HttpContext.GetGlobalResourceObject("Const", "Error_Save").ToString();
                saveSucceed = HttpContext.GetGlobalResourceObject("Const", "Succeed_Save").ToString();
            }
            catch { }

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
                catch { }
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
                string vehicleIDs = GetCheckedVehicle();
                string maintenances = GetCheckedMaintenances();

                if (vehicleIDs != string.Empty && maintenances != string.Empty)
                {
                    try
                    {

                        MCCManager mccMgr = new MCCManager(sConnectionString);
                        //string unSavedVehicleIDs = vehicleEquipmentAssignmentManager.AddVehicleEquipmentAssignment(vehicleIDssb.ToString(),
                        //    equipmentMedias.ToString());
                        mccMgr.VehicleMaintenanceAssignment(vehicleIDs, maintenances, int.Parse(combMccGroup.SelectedValue));
                        ClearCheckedVehicle();
                        //string msg = string.Format("alert(\"{0}\");", saveSucceed);
                        lblMessage.ForeColor = Color.Green;
                        lblMessage.Text = saveSucceed;
                        combMccGroup.SelectedIndex = 0;

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

        public void ServerValidateVehicle(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (GetCheckedVehicle().Length > 0) args.IsValid = true;
                else args.IsValid = false;
            }
            catch
            {
                args.IsValid = false;
            }
        }

        public void ServerValidateMCCMaintenances(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (GetCheckedMaintenances().Length > 0) args.IsValid = true;
                else args.IsValid = false;
            }
            catch
            {
                args.IsValid = false;
            }
        }

        private string GetCheckedMaintenances()
        {
            string selectedMaintenances = string.Empty;
            foreach (GridDataItem gdItem in gdMCCMaintenances.MasterTableView.Items)
            {
                if (gdItem.FindControl("chkSelectMaintenance") != null)
                {
                    if (((CheckBox)gdItem.FindControl("chkSelectMaintenance")).Checked)
                    {
                        if (selectedMaintenances == string.Empty) selectedMaintenances = gdItem.GetDataKeyValue("MaintenanceId").ToString();
                        else selectedMaintenances = selectedMaintenances + "," + gdItem.GetDataKeyValue("MaintenanceId").ToString();
                    }
                }
            }
            return selectedMaintenances;
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

        private string GetCheckedVehicle()
        {
            StringBuilder vehicleIDs = new StringBuilder();
            foreach (RadComboBoxItem item in cboVehicle.Items)
            {
                CheckBox chkVehicle = (CheckBox)item.FindControl("chkVehicle");
                if (chkVehicle != null && chkVehicle.Checked)
                {
                    if (item.Value != entireValue.ToString())
                    {
                        if (vehicleIDs.Length == 0) vehicleIDs.Append(item.Value);
                        else vehicleIDs.Append("," + item.Value) ;
                    }
                }
            }
            return vehicleIDs.ToString();
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

        protected void combMccGroup_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            string vehicles = GetCheckedVehicle();
            MCCManager mccMgr = new MCCManager(sConnectionString);
            gdMCCMaintenances.DataSource = mccMgr.GetMCCMaintenanceByMccIdandVehicleID(sn.User.OrganizationId, int.Parse(combMccGroup.SelectedValue), clsAsynGenerateReport.GetOperationTypeStr(), vehicles);
            gdMCCMaintenances.DataBind();
            gdMCCMaintenances.Visible = true;
        }

        //public static string GetOperationTypeStr()
        //{
        //    string path = HttpContext.Current.Server.MapPath("~/Maintenance/App_LocalResources/frmMaintenanceNew.aspx");
        //    string OperationTypefileName = clsAsynGenerateReport.GetResourceObject(path, "XmlDSOperationType_DataFile");
        //    DataSet operationsData = new DataSet();
        //    operationsData.ReadXml(HttpContext.Current.Server.MapPath(OperationTypefileName));
        //    return clsAsynGenerateReport.GetOperationTypeStr_1(operationsData);
        //}


        protected void gdMCCMaintenances_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.DataItem != null && e.Item.FindControl("chkSelectMaintenance") != null)
            {
                if (((DataRowView)e.Item.DataItem)["isAssigned"] != DBNull.Value)
                {
                    CheckBox chkSelectMaintenance = (CheckBox)e.Item.FindControl("chkSelectMaintenance");
                    chkSelectMaintenance.Checked = true;
                    chkSelectMaintenance.Enabled = false;
                }
            }
        }
}
}