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
    public partial class Maintenance_frmMaintenanceVehicleView : SentinelFMBasePage
    {
        public string Error_Load = "Failed to load data.";  //(string)base.GetLocalResourceObject("Error_Load");
        string Error_Delete = "Delete failed."; //(string)base.GetLocalResourceObject("errorDelete");
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        DataTable dtVehicle = null;
        protected void Page_Load(object sender, EventArgs e)
        {
              Error_Load =(string)base.GetLocalResourceObject("Error_Load");
            Error_Delete = (string)base.GetLocalResourceObject("errorDelete");

            try
            {
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
                }
                if (!IsPostBack)
                {
                    this.Master.CreateMaintenenceMenu(null);

                }
                FleetVehicle1.isLoadDefault = true;
                FleetVehicle1.Fleet_SelectedIndexChanged += new EventHandler(FleetVehicle1_Fleet_SelectedIndexChanged);
                FleetVehicle1.OrganizationHierarchySelectChanged += new EventHandler(FleetVehicle1_OrganizationHierarchySelectChanged);
                FleetVehicle1.Vehicle_SelectedIndexChanged += new EventHandler(FleetVehicle1_Vehicle_SelectedIndexChanged);
                //FleetVehicle1.radAjaxManager1 = RadAjaxManager1;
                //FleetVehicle1.radAjaxLoadingPanel1 = LoadingPanel1.ID;
                //FleetVehicle1.radUpdatedControl = pnl.ID;
                dgMCCAssignment.RadAjaxManagerControl = RadAjaxManager1;
                //FleetVehicle1.isLoadDefault = true;
                //FleetVehicle1.Fleet_SelectedIndexChanged += new EventHandler(FleetVehicle1_Fleet_SelectedIndexChanged);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                base.RedirectToLogin();
            }
            this.Master.resizeScript = SetResizeScript(dgMCCAssignment.ClientID);
            this.Master.isHideScroll = true;

            try
            {
                Error_Load = GetScriptEscapeString(GetGlobalResourceObject("Const", "Error_Load").ToString());
            }
            catch (Exception ex)
            { }
            dgMCCAssignment.SubGridID = "dgServiceAssignment";
        }

        void FleetVehicle1_Vehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            BinddgMaintenance(true);
        }

        void FleetVehicle1_Fleet_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadComboBox fleetCombs = (RadComboBox)FleetVehicle1.FindControl("cboVehicle");
            if (fleetCombs != null)
            {
                RadComboBoxItem rItem = fleetCombs.Items.FindItemByValue("-999");
                if (rItem != null)
                {
                    fleetCombs.ClearSelection();
                    rItem.Selected = true;
                }
            }
            BinddgMaintenance(true); 
        }

        void FleetVehicle1_OrganizationHierarchySelectChanged(object sender, EventArgs e)
        {
            RadComboBox fleetCombs = (RadComboBox)FleetVehicle1.FindControl("cboVehicle");
            if (fleetCombs != null)
            {
                RadComboBoxItem rItem = fleetCombs.Items.FindItemByValue("-999");
                if (rItem != null)
                {
                    fleetCombs.ClearSelection();
                    rItem.Selected = true;
                }
            }
            BinddgMaintenance(true);
        }

        protected void dgServiceAssignment_ItemDataBound(object sender, GridItemEventArgs e)
        {
        }

        protected void dgServiceAssignment_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (dtVehicle == null)
            {
                MCCManager mccMgr = new MCCManager(sConnectionString);
                string selectedVehicles = FleetVehicle1.GetAllSelectedVehicle();
                dtVehicle = mccMgr.MaintenanceGetServicesByVehicles(sn.User.OrganizationId, selectedVehicles, sn.UserID).Tables[0];
            }
            string vehicleId = ((GridDataItem)((RadGrid)sender).Parent.Parent).GetDataKeyValue("VehicleId").ToString();
            if (vehicleId != null && dtVehicle != null)
            {
                   string strExpr = "VehicleId=" +vehicleId;
                   DataRow[] drs = dtVehicle.Select(strExpr);
                   ((RadGrid)sender).DataSource = drs;
            }
            foreach (GridColumn column in ((RadGrid)sender).Columns)
            {
                column.HeaderText = "<div style='color:White'>" + column.HeaderText + "</div>";
            }
        }

        protected void dgMCCAssignment_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BinddgMaintenance(false);
        }

        protected void dgServiceAssignment_ItemDeleted(object sender, GridCommandEventArgs e)
        {
            if (!(e.Item is GridDataItem)) return;
            try
            {
                string vehicleID = ((GridDataItem)(e.Item)).GetDataKeyValue("VehicleId").ToString();
                string mccID = ((GridDataItem)(e.Item)).GetDataKeyValue("MccId").ToString();
                string maintenanceID = ((GridDataItem)(e.Item)).GetDataKeyValue("MaintenanceID").ToString();
                MCCManager mccMgr = new MCCManager(sConnectionString);
                mccMgr.MaintenanceVehiclesUnAssignment(vehicleID, maintenanceID, sn.UserID, long.Parse(mccID));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", Error_Load));
            }
        }


        private void BinddgMaintenance(Boolean isBind)
        {
            try
            {
                MCCManager mccMgr = new MCCManager(sConnectionString);
                string selectedVehicles = FleetVehicle1.GetAllSelectedVehicle();
                if (selectedVehicles != string.Empty)
                {
                    if (dtVehicle == null)
                        dtVehicle = mccMgr.MaintenanceGetServicesByVehicles(sn.User.OrganizationId, selectedVehicles, sn.UserID).Tables[0];

                    dgMCCAssignment.DataSource = dtVehicle.DefaultView.ToTable(true, "BoxId", "VehicleId", "VehicleDescription"); ;
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("VehicleId");
                    dgMCCAssignment.DataSource = dt;
                }

                if (isBind)
                {
                    dgMCCAssignment.DataBind();
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", Error_Load));
            }
        }

        
    }
}