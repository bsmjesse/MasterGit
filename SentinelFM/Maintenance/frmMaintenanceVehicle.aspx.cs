using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Text;
using VLF.CLS;
using System.IO;
using System.Globalization;
using Telerik.Web.UI;
using VLF.DAS.Logic;
using System.Web.Services;
using System.Collections.Generic;
using System.Web.Script.Serialization;


namespace SentinelFM
{
    public partial class Maintenance_frmMaintenanceVehicle : SentinelFMBasePage
    {
        public string inputCalError = "Date is required.";  //(string)base.GetLocalResourceObject("inputCalError");
        public string selectGroup = "Select a Group"; //(string)base.GetLocalResourceObject("selectGroup");
        string errorInsert = "Save failed."; //(string)base.GetLocalResourceObject("errorSave");
        public string saveSucceed = "Saved Successfully."; //(string)base.GetLocalResourceObject("saveSucceed");
        public string entireFleet = "Entire Fleet"; //(string)base.GetLocalResourceObject("entireFleet");
        int entireValue = -1;
        public string selectFleet = "Select a Fleet"; //(string)base.GetLocalResourceObject("selectFleet");
        public string selectMaintenance = "Please Select Services."; //(string)base.GetLocalResourceObject("selectMaintenance");
        public string selectMaintenanceGroup = "Please Select a Group."; //(string)base.GetLocalResourceObject("selectMaintenanceGroup");
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        string selectedMaintenances = string.Empty;
        Boolean isTimeBase = false;
        Boolean isTimeBaseAll = false;
        Boolean isOdoandEng = false;
        public string Error_Load = "Failed to load data.";  //(string)base.GetLocalResourceObject("Error_Load");
        List<string> maintenanceDue = new List<string>();
        DataTable selectedMaintainenceForLast = null;
        protected void Page_Load(object sender, EventArgs e)
        {

          inputCalError = (string)base.GetLocalResourceObject("inputCalError");
         selectGroup = (string)base.GetLocalResourceObject("selectGroup");
         errorInsert = (string)base.GetLocalResourceObject("errorSave");
         saveSucceed = (string)base.GetLocalResourceObject("saveSucceed");
         entireFleet = (string)base.GetLocalResourceObject("entireFleet");
         selectFleet = (string)base.GetLocalResourceObject("selectFleet");
         selectMaintenance = (string)base.GetLocalResourceObject("selectMaintenance");
         selectMaintenanceGroup = (string)base.GetLocalResourceObject("selectMaintenanceGroup");
         Error_Load = (string)base.GetLocalResourceObject("Error_Load");

            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
            }
            if (!IsPostBack)
            {
                this.Master.CreateMaintenenceMenu(null);
                MCCManager mccMgr = new MCCManager(sConnectionString);
                combMccGroup.DataSource = mccMgr.GetOrganizationMCCGroup(sn.User.OrganizationId);
                combMccGroup.DataBind();
                combMccGroup.Items.Insert(0, new RadComboBoxItem(selectGroup, "-1"));

                if (sn.User.LoadVehiclesBasedOn != "hierarchy")
                {
                    DataSet dsFleets = sn.User.GetUserFleets(sn);
                    cboFleet.DataSource = dsFleets;
                    cboFleet.DataBind();
                    cboFleet.Items.Insert(0, new RadComboBoxItem(selectFleet, "-1"));
                }
            }
            
            
            this.Master.isHideScroll = true;
            gdMCCMaintenances.RadAjaxManagerControl = RadAjaxManager1;


                //gdMCCMaintenances.VerticalOffSetControl = pnlVehicle.ClientID;
            this.Master.resizeScript = SetResizeScript(gdSource.ClientID) + SetResizeScript(gdDest.ClientID) + SetResizeScript(gdMCCMaintenances.ClientID);
            gdDest.VerticalOffSetControl = "tblVehicle";
            gdSource.VerticalOffSetControl = "tblVehicle";
            if (ViewState["vehicleGrid"] == null || ViewState["vehicleGrid"].ToString() == "0")
            {
                pnlVehicle.Visible = false;
                gdMCCMaintenances.IsShow = true;
                gdSource.IsShow = false;
                gdDest.IsShow = false;
            }
            else
            {
                pnlVehicle.Visible = true;
                gdMCCMaintenances.IsShow = false;
                gdSource.IsShow = true;
                gdDest.IsShow = true;

            }

            inputCalError = GetScriptEscapeString(inputCalError);
        }
        protected void combMccGroup_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (combMccGroup.SelectedIndex > 0)
            {
                BindgdMCCMaintenances(true);
            }
            btnClose_Click(null, null);
            cboFleet.SelectedIndex = 0;
            pnlFleetVehicle.Attributes["Style"] = "display:none";
            FleetVehicle1.ReSetFleet();
        }
        
        private void BindgdMCCMaintenances(Boolean isBind)
        {
            try
            {
                MCCManager mccMgr = new MCCManager(sConnectionString);
                gdMCCMaintenances.DataSource = mccMgr.GetMCCMaintenanceByMccId(sn.User.OrganizationId, long.Parse(combMccGroup.SelectedValue), clsAsynGenerateReport.GetOperationTypeStr(), sn.UserID);
                if (isBind)
                    gdMCCMaintenances.DataBind();
            }
            catch (OutOfMemoryException Exout)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Exout.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name + "_BindgdMCCMaintenances"));
                RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", "Too many records....Please select another fleet."));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name+"_BindgdMCCMaintenances"));
                RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", Error_Load));
            }

        }
        
        protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            if (e.Argument.ToLower() == "rebindvehicles")
            {
                if (pnlVehicle.Visible)
                {
                    cboFleet_SelectedIndexChanged(null, null);
                }
            }

            if (e.Argument.ToLower() == "closevehicles")
            {
                btnClose_Click(null, null);
                cboFleet.SelectedIndex = 0;

            }
        }
        protected void gdMCCMaintenances_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindgdMCCMaintenances(false);
        }

        protected void gdMCCMaintenances_ItemCommand(object sender, GridCommandEventArgs e)
        {
        }

        protected void cboFleet_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            BindgdDest(true);
            BindgdSource(true);
            Page_Load(null,null);
        }

        protected void gvServicesSource_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "assign" || e.CommandName.ToLower() == "assignall")
            {
                try
                {
                    string maintenanceIDs = GetSelectedMaintainence();
                        string vehicleIDs = string.Empty;
                        if (e.CommandName.ToLower() == "assign")
                        {
                            if (e.Item is GridDataItem)
                            {
                                vehicleIDs = ((GridDataItem)e.Item).GetDataKeyValue("VehicleId").ToString();
                                RadDatePicker calValue = (RadDatePicker)e.Item.FindControl("calValue");
                                if (isTimeBaseAll)
                                {
                                    DateTime dtNow = System.DateTime.Now.Date;
                                    if (calValue.Visible)
                                    {
                                        dtNow = calValue.SelectedDate.Value;
                                    }
                                    vehicleIDs = GetVehicleDue(dtNow, vehicleIDs);
                                    //FindVehicleLastServices((GridDataItem)e.Item, ref vehicleIDs);
                                    
                                }
                                else
                                {
                                    if (calValue.Visible)
                                    {
                                        if (calValue.SelectedDate.HasValue)
                                        {
                                            vehicleIDs = GetVehicleDue(calValue.SelectedDate.Value, vehicleIDs);
                                            //FindVehicleLastServices((GridDataItem)e.Item, ref vehicleIDs);
                                        }
                                        else vehicleIDs = string.Empty;
                                    }
                                }
                                if (vehicleIDs != string.Empty )
                                     FindVehicleLastServices((GridDataItem)e.Item, ref vehicleIDs);
                            }
                        }
                        else vehicleIDs = GetSelectedUnAssignVehicles();
                        MCCManager mccMgr = new MCCManager(sConnectionString);

                        maintenanceIDs = sn.UserID.ToString() +  "$" + maintenanceIDs;
                    mccMgr.VehicleMaintenanceAssignment(vehicleIDs, maintenanceIDs, int.Parse(combMccGroup.SelectedValue));
                    BindgdSource(true);
                    BindgdDest(true);
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", errorInsert));
                }
                
            }
        }

        private void FindVehicleLastServices(GridDataItem gItem, ref string vehicleIDs)
        {
            string lastServices = String.Empty;
            RadGrid dgLastService = (RadGrid)gItem.FindControl("dgLastService");
            if (dgLastService != null )
            {
                foreach (GridDataItem gItemLast in dgLastService.Items)
                {
                    RadNumericTextBox txtOdoHrsLast = (RadNumericTextBox)gItemLast.FindControl("txtOdoHrsLast");
                    RadDatePicker calValueLast = (RadDatePicker)gItemLast.FindControl("calValueLast");
                    string maintenanceID = gItemLast.GetDataKeyValue("MaintenanceId").ToString();
                    if (txtOdoHrsLast != null && calValueLast != null)
                    {
                        if (txtOdoHrsLast.Value.HasValue && calValueLast.SelectedDate.HasValue && txtOdoHrsLast.Value > 0)
                        {
                            if (lastServices == string.Empty)
                                lastServices = maintenanceID + " " + txtOdoHrsLast.Value.ToString() + " " + string.Format("{0:yyyyMMdd}", calValueLast.SelectedDate.Value); 
                            else
                                lastServices = lastServices + "*" + maintenanceID + " " + txtOdoHrsLast.Value.ToString() + " " + string.Format("{0:yyyyMMdd}", calValueLast.SelectedDate.Value); 
                        }
                    }
                }

            }
            if (lastServices != string.Empty) vehicleIDs = lastServices + '$' + vehicleIDs;
        }

        private string GetVehicleDue(DateTime? dtOrg, string vehicleID)
        {
            StringBuilder dues = new StringBuilder();
            foreach(string mdue in maintenanceDue)
            {
                if (mdue.IndexOf("-") <= 0)
                {
                    DateTime dt = dtOrg.Value;
                    string[] mdues = mdue.Split('#');
                    dt = dt.AddDays(int.Parse(mdues[1]));

                    if (dues.Length > 0) dues.Append("#");
                    dues.Append(string.Format("{0}#{1}", mdues[0], dt.Subtract(new DateTime(1970, 1, 1)).TotalDays));
                }
                else
                {
                    try
                    {
                        string[] mdues = mdue.Split('#');
                        string[] timeSpans = mdues[1].Split('-');
                        string[] monthDay = timeSpans[1].Split('/');
                        DateTime dt = System.DateTime.Now.Date;
                        if (timeSpans[0] == "4")
                        {
                            DateTime dt_1 = new DateTime(dt.Year, dt.Month, 1).AddDays(int.Parse(monthDay[1]) - 1);
                            if (dt_1 < dt) dt = new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(int.Parse(monthDay[1]) - 1);
                            else dt = dt_1;
                        }
                        int i_mode = 0;
                        if (timeSpans[0] == "5" || timeSpans[0] == "6" || timeSpans[0] == "7")
                        {
                            if (timeSpans[0] == "5") i_mode = 2;
                            if (timeSpans[0] == "6") i_mode = 3;
                            if (timeSpans[0] == "7") i_mode = 6;

                            int lowMonth = (dt.Month - 1) / i_mode;
                            int hightMonth = (dt.Month - 1) / i_mode + 1;

                            DateTime dt_1 = new DateTime(dt.Year, 1, 1).AddMonths(lowMonth * i_mode).AddDays(int.Parse(monthDay[1]) - 1);
                            if (dt_1 < dt)
                            {
                                dt = new DateTime(dt.Year, 1, 1).AddMonths(hightMonth * i_mode).AddDays(int.Parse(monthDay[1]) - 1);
                            }
                            else dt = dt_1;
                        }
                        if (timeSpans[0] == "8")
                        {
                            DateTime dt_1 = new DateTime(dt.Year, 1, 1).AddMonths(int.Parse(monthDay[0]) - 1).AddDays(int.Parse(monthDay[1]) - 1);
                            if (dt_1 < dt) dt = new DateTime(dt.Year + 1, 1, 1).AddMonths(int.Parse(monthDay[0]) - 1).AddDays(int.Parse(monthDay[1]) - 1);
                            else dt = dt_1;
                        }
                        if (dues.Length > 0) dues.Append("#");
                        dues.Append(string.Format("{0}#{1}", mdues[0], dt.Subtract(new DateTime(1970, 1, 1)).TotalDays));

                    }
                    catch (Exception ex) { }
                }
            }

            return vehicleID + "#" + dues.ToString();
        }

        protected void gdSource_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindgdSource(false);
        }

        private void BindgdSource(Boolean isBind)
        {
            try
            {
                if (sn.User.LoadVehiclesBasedOn == "hierarchy" && isBind)
                {
                    string selectedFleet = FleetVehicle1.GetSelectedFleet();
                    if (!String.IsNullOrEmpty(selectedFleet.Trim()) && selectedFleet!="-1")
                    {
                        MCCManager mccMgr = new MCCManager(sConnectionString);
                        if (selectedMaintenances == string.Empty) selectedMaintenances = GetSelectedMaintainence();
                        if (selectedMaintenances == string.Empty) return;
                        if (isTimeBase)
                            gdSource.Columns.FindByUniqueName("ColcalValue").Visible = true;
                        else gdSource.Columns.FindByUniqueName("ColcalValue").Visible = false;

                        if (isOdoandEng)
                        {
                            gdSource.Columns.FindByUniqueName("LastService").Visible = true;
                        }
                        else gdSource.Columns.FindByUniqueName("LastService").Visible = false;

                        if (selectedFleet.Contains(","))
                            gdSource.DataSource = mccMgr.GetVehicleUnAssignedMCCMultiFleet(selectedFleet, selectedMaintenances, long.Parse(combMccGroup.SelectedValue));
                        else
                            gdSource.DataSource = mccMgr.GetVehicleUnAssignedMCC(int.Parse(selectedFleet), selectedMaintenances, long.Parse(combMccGroup.SelectedValue));
                    }
                    else
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("Description");
                        dt.Columns.Add("VehicleId");
                        gdSource.DataSource = dt;
                    }                
                }
                else if (cboFleet.SelectedIndex > 0)
                {
                    MCCManager mccMgr = new MCCManager(sConnectionString);
                    if (selectedMaintenances == string.Empty) selectedMaintenances = GetSelectedMaintainence();
                    if (isTimeBase)
                        gdSource.Columns.FindByUniqueName("ColcalValue").Visible= true;
                    else gdSource.Columns.FindByUniqueName("ColcalValue").Visible = false;

                    if (isOdoandEng)
                    {
                        gdSource.Columns.FindByUniqueName("LastService").Visible = true;
                    }
                    else gdSource.Columns.FindByUniqueName("LastService").Visible = false;

                    gdSource.DataSource = mccMgr.GetVehicleUnAssignedMCC(int.Parse(cboFleet.SelectedValue), selectedMaintenances, long.Parse(combMccGroup.SelectedValue));
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Description");
                    dt.Columns.Add("VehicleId");
                   
                    gdSource.DataSource = dt;
                }

                if (isBind) gdSource.DataBind();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name+"_BindgdSource"));
                RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", Error_Load));
            }

        }

        private void BindgdDest(Boolean isBind)
        {
            try
            {
                if (sn.User.LoadVehiclesBasedOn == "hierarchy" && isBind)
                {
                     string selectedFleet = FleetVehicle1.GetSelectedFleet();
                    if (!String.IsNullOrEmpty(selectedFleet.Trim()) && selectedFleet!="-1")
                    {
                        MCCManager mccMgr = new MCCManager(sConnectionString);
                        if (selectedMaintenances == string.Empty) selectedMaintenances = GetSelectedMaintainence();
                        if (selectedMaintenances == string.Empty) return;
                        if (selectedFleet.Contains(","))
                            gdDest.DataSource = mccMgr.GetVehicleAssignedMCCMultiFleet(selectedFleet, selectedMaintenances, long.Parse(combMccGroup.SelectedValue));
                        else
                            gdDest.DataSource = mccMgr.GetVehicleAssignedMCC(int.Parse(selectedFleet), selectedMaintenances, long.Parse(combMccGroup.SelectedValue));
                    }
                    else
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("Description");
                        dt.Columns.Add("VehicleId");
                        gdDest.DataSource = dt;
                    }
                }
                else if (cboFleet.SelectedIndex > 0)
                {
                    MCCManager mccMgr = new MCCManager(sConnectionString);
                    if (selectedMaintenances == string.Empty) selectedMaintenances = GetSelectedMaintainence();
                    gdDest.DataSource = mccMgr.GetVehicleAssignedMCC(int.Parse(cboFleet.SelectedValue), selectedMaintenances, long.Parse(combMccGroup.SelectedValue));

                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Description");
                    dt.Columns.Add("VehicleId");
                    gdDest.DataSource = dt;
                }

                if (isBind) gdDest.DataBind();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name+"_BindgdDest"));
                RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", Error_Load));
            }

        }


        protected void gvDest_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "unassign" || e.CommandName.ToLower() == "unassignall")
            {
                try
                {
                    string maintenanceIDs = GetSelectedMaintainence();
                    string vehicleIDs = string.Empty;
                    if (e.CommandName.ToLower() == "unassign")
                    {
                        if (e.Item is GridDataItem)
                        {

                            vehicleIDs = ((GridDataItem)e.Item).GetDataKeyValue("VehicleId").ToString();
                        }
                    }
                    else vehicleIDs = GetSelectedAssignVehicles();
                    MCCManager mccMgr = new MCCManager(sConnectionString);
                    mccMgr.MaintenanceVehiclesUnAssignment(vehicleIDs, maintenanceIDs, sn.UserID, long.Parse(combMccGroup.SelectedValue));
                    BindgdSource(true);
                    BindgdDest(true);
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", errorInsert));
                }

            }

        }
        protected void gvDest_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindgdDest(false);
            
        }

        private void GetSelectedMaintainenceForLastService()
        {
            if (selectedMaintainenceForLast == null)
            {
                selectedMaintainenceForLast = new DataTable();
                selectedMaintainenceForLast.Columns.Add("MaintenanceId");
                selectedMaintainenceForLast.Columns.Add("Description");
                selectedMaintainenceForLast.Columns.Add("lstdate", System.Type.GetType("System.DateTime"));
                int inr = 0;
                selectedMaintainenceForLast.Columns.Add("value", inr.GetType());

                foreach (GridDataItem gItem in gdMCCMaintenances.Items)
                {

                    if (gItem.FindControl("hidOperationType") != null)
                    {

                        CheckBox btnAssign = (CheckBox)gItem.FindControl("btnAssign");

                        if (btnAssign != null && !btnAssign.Checked) continue; 
                        if (((HiddenField)gItem.FindControl("hidOperationType")).Value == "1" ||
                            ((HiddenField)gItem.FindControl("hidOperationType")).Value == "2")
                        {
                            DataRow dr = selectedMaintainenceForLast.NewRow();
                            dr["MaintenanceId"] = gItem.GetDataKeyValue("MaintenanceId").ToString();
                            dr["Description"] = gItem["Description"].Text;
                            dr["value"] = 0;
                            dr["lstdate"] = System.DateTime.Now;
                            selectedMaintainenceForLast.Rows.Add(dr);
                        }

                    }
                }
            }
        }

        private string GetSelectedMaintainence()
        {
            maintenanceDue = new List<string>();
            foreach(GridDataItem gItem in gdMCCMaintenances.Items) 
            {
               
                CheckBox btnAssign = (CheckBox)gItem.FindControl("btnAssign");
                if (btnAssign != null && btnAssign.Checked)
                {
                    if (selectedMaintenances == string.Empty) selectedMaintenances = gItem.GetDataKeyValue("MaintenanceId").ToString();
                    else selectedMaintenances = selectedMaintenances + "," + gItem.GetDataKeyValue("MaintenanceId").ToString();
                    if (gItem.FindControl("hidOperationType") != null)
                    {
                        if (((HiddenField)gItem.FindControl("hidOperationType")).Value == "1" ||
                            ((HiddenField)gItem.FindControl("hidOperationType")).Value == "2")
                        {
                            isOdoandEng = true;
                        }
                        if (((HiddenField)gItem.FindControl("hidOperationType")).Value == "3")
                        {
                            HiddenField hidFixDate = (HiddenField)gItem.FindControl("hidFixDate");
                            if (hidFixDate != null && hidFixDate.Value.ToLower() == "true")
                            {
                                isTimeBaseAll = true;
                                HiddenField hidTimespanId = (HiddenField)gItem.FindControl("hidTimespanId");
                                if (hidTimespanId != null)
                                {
                                    maintenanceDue.Add(gItem.GetDataKeyValue("MaintenanceId").ToString() + "#" + hidTimespanId.Value + "-" + gItem["FixedServiceDate"].Text);
                                }
                            }
                            else
                            {
                                isTimeBase = true;
                                HiddenField hidInterval = (HiddenField)gItem.FindControl("hidInterval");
                                if (hidInterval != null)
                                {
                                    maintenanceDue.Add(gItem.GetDataKeyValue("MaintenanceId").ToString() + "#" + hidInterval.Value);
                                }
                            }
                         }
                    }

                }
            }
            return selectedMaintenances;
        }

        private string GetSelectedUnAssignVehicles()
        {
            string selectedVehicles = string.Empty;
            foreach (GridDataItem gItem in gdSource.Items)
            {
                if (gItem is GridDataItem)
                {
                    string vehicleIDs = ((GridDataItem)gItem).GetDataKeyValue("VehicleId").ToString();
                    RadDatePicker calValue = (RadDatePicker)gItem.FindControl("calValue");
                    if (isTimeBaseAll)
                    {
                        DateTime dtNow = System.DateTime.Now.Date;
                        if (calValue.Visible)
                        {
                            dtNow = calValue.SelectedDate.Value;
                        }

                        vehicleIDs = GetVehicleDue(dtNow, vehicleIDs);
                        //FindVehicleLastServices((GridDataItem)gItem, ref vehicleIDs);
                    }
                    else
                    {
                        if (calValue.Visible)
                        {
                            if (calValue.SelectedDate.HasValue)
                            {
                                vehicleIDs = GetVehicleDue(calValue.SelectedDate.Value, vehicleIDs);
                                //FindVehicleLastServices((GridDataItem)gItem, ref vehicleIDs);
                            }

                            else vehicleIDs = string.Empty;
                        }
                    }


                    if (vehicleIDs != string.Empty)
                    {
                        FindVehicleLastServices((GridDataItem)gItem, ref vehicleIDs);
                        if (selectedVehicles == string.Empty)
                        {
                            selectedVehicles = vehicleIDs;
                        }
                        else selectedVehicles = selectedVehicles + "," + vehicleIDs;
                    }
                }
            }
            return selectedVehicles;
        }

        private string GetSelectedAssignVehicles()
        {
            string selectedVehicles = string.Empty;
            foreach (GridDataItem gItem in gdDest.Items)
            {
                if (gItem.GetDataKeyValue("VehicleId") != null)
                {
                    if (selectedVehicles == string.Empty) selectedVehicles = gItem.GetDataKeyValue("VehicleId").ToString();
                    else selectedVehicles = selectedVehicles + "," + gItem.GetDataKeyValue("VehicleId").ToString();
                }
            }
            return selectedVehicles;
        }

        protected void dgLastService_ItemDataBound(object sender, GridItemEventArgs e)
        {
            RadDatePicker calValueLast = (RadDatePicker)e.Item.FindControl("calValueLast");
            if (calValueLast != null)
            {
                calValueLast.SelectedDate = System.DateTime.Now.Date;
            }
        }

        protected void gdSource_ItemDataBound(object sender, GridItemEventArgs e)
        {
            try
            {
                RadDatePicker calValue = (RadDatePicker)e.Item.FindControl("calValue");
                if (calValue != null)
                {
                    if (isTimeBase)
                    {
                        calValue.Visible = true;
                        //calValue.MinDate = System.DateTime.Now;
                        if (calValue.SelectedDate == null) calValue.SelectedDate = System.DateTime.Now.Date;

                    }
                    else calValue.Visible = false;
                }
                if (isOdoandEng)
                {
                    if (selectedMaintainenceForLast == null) GetSelectedMaintainenceForLastService();
                    RadGrid dgLastService = (RadGrid)e.Item.FindControl("dgLastService");
                    if (dgLastService != null)
                    {
                        dgLastService.DataSource = selectedMaintainenceForLast;
                        dgLastService.DataBind();
                    }
                }
            }
            catch
            {
                RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", Error_Load));
            }
        }
        protected void btnClose_Click(object sender, EventArgs e)
        {
            ViewState["vehicleGrid"] = "0"; 
            cboFleet.SelectedIndex = 0;
            btnAdd.Visible = true;
            Page_Load(null, null);

        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (combMccGroup.SelectedIndex <= 0)
                {
                    RadAjaxManager1.ResponseScripts.Add("alert('" + selectMaintenanceGroup + "');");
                    return;
                }

                if (GetSelectedMaintainence() == string.Empty)
                {
                    RadAjaxManager1.ResponseScripts.Add("alert('" + selectMaintenance + "');");
                    return;
                }
                btnAdd.Visible = false;
                if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                {
                    pnlFleetVehicle.Attributes["Style"] = "display:block";
                    lblFleet.Visible = false;
                    cboFleet.Visible = false;
                }
                else
                {
                    pnlFleetVehicle.Attributes["Style"] = "display:none";
                    lblFleet.Visible = true;
                    cboFleet.Visible = true;
                }
                ViewState["vehicleGrid"] = "1";
                Page_Load(null, null);
                BindgdSource(true);
                BindgdDest(true);
            }
            catch
            {
                RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", Error_Load));
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            FleetVehicle1.OnOkClick += ActionControl_OnOkClick;
        }

        protected void ActionControl_OnOkClick()
        {
            BindgdDest(true);
            BindgdSource(true);
            Page_Load(null,null);
        }


 }
}
