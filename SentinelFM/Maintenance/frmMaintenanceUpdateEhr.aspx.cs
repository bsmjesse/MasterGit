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
using SentinelFM;

namespace SentinelFM
{
    public partial class Maintenance_frmMaintenanceUpdateEhr : SentinelFMBasePage
    {

        public string Error_Load = "Failed to load data.";  //(string)base.GetLocalResourceObject("Error_Load");
        string errorSave = "Save failed."; //(string)base.GetLocalResourceObject("errorSave");
        public string saveSucceed = "Saved Successfully."; //(string)base.GetLocalResourceObject("saveSucceed");
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
             Error_Load = (string)base.GetLocalResourceObject("Error_Load");
             errorSave = (string)base.GetLocalResourceObject("errorSave");
            saveSucceed = (string)base.GetLocalResourceObject("saveSucceed");
        

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

                FleetVehicle1.Fleet_SelectedIndexChanged += new EventHandler(FleetVehicle1_Fleet_SelectedIndexChanged);
                FleetVehicle1.OrganizationHierarchySelectChanged += new EventHandler(FleetVehicle1_OrganizationHierarchySelectChanged);
                FleetVehicle1.isLoadDefault = true;
		RadComboBox fleetsCombs = (RadComboBox)FleetVehicle1.FindControl("cboFleet");
                Session["EngineFleetSelected"] = fleetsCombs.SelectedValue;

                FleetVehicle1.Vehicle_SelectedIndexChanged += new EventHandler(FleetVehicle1_Vehicle_SelectedIndexChanged);
                dgMaintenance.RadAjaxManagerControl = RadAjaxManager1;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                base.RedirectToLogin();
            }
            this.Master.resizeScript = SetResizeScript(dgMaintenance.ClientID);
            this.Master.isHideScroll = true;

            try
            {
                Error_Load = GetScriptEscapeString(GetGlobalResourceObject("Const", "Error_Load").ToString());
            }
            catch (Exception ex)
            { }
        }

        void FleetVehicle1_Fleet_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadComboBox fleetCombs = (RadComboBox)FleetVehicle1.FindControl("cboVehicle");
	    RadComboBox comboBox = (RadComboBox) sender;
	    Session["EngineFleetSelected"] = comboBox.SelectedValue;
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

        protected void dgMaintenance_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BinddgMaintenance(false);
        }

        protected void dgMaintenance_ItemDataBound(object sender, GridItemEventArgs e)
        {
            RadNumericTextBox txtCurrentOdoHrs = (RadNumericTextBox)e.Item.FindControl("txtCurrentOdoHrs");
            if (txtCurrentOdoHrs != null)
            {
                txtCurrentOdoHrs.MaxValue = int.MaxValue;
                DataRowView drv =  (DataRowView)e.Item.DataItem;
                if (drv != null) 
                {
                    txtCurrentOdoHrs.Text = drv["CurrentEngineHours"].ToString();
                }
            }
        }

        void FleetVehicle1_Vehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            BinddgMaintenance(true);
        }

        void BinddgMaintenance(Boolean isBind)
        {
            try
            {
                MCCManager mccMgr = new MCCManager(sConnectionString);
                string selectedVehicles = FleetVehicle1.GetAllSelectedVehicle();
                if (selectedVehicles != string.Empty)
                {
                    DataSet ds = mccMgr.Maintenance_GetVehiclesEngineHours(selectedVehicles);
                    dgMaintenance.DataSource = ds;
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("VehicleId");
                    dgMaintenance.DataSource = dt;
                }
                if (isBind)
                {
                    dgMaintenance.DataBind();
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", Error_Load));
            }
        }

        protected void dgMaintenance_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Update")
            {
                try
                {
                    string VehicleId = ((GridDataItem)e.Item).GetDataKeyValue("VehicleId").ToString();
                    RadNumericTextBox txtCurrentOdoHrs = (RadNumericTextBox)e.Item.FindControl("txtCurrentOdoHrs");
                    Label lblCurrentEngineHours = (Label)e.Item.FindControl("lblCurrentEngineHours");
                    if (txtCurrentOdoHrs != null && txtCurrentOdoHrs.Value.HasValue &&
                        lblCurrentEngineHours.Text != txtCurrentOdoHrs.Value.ToString())
                    {
                        int engineHours = (int)txtCurrentOdoHrs.Value.Value;
                        MCCManager mccMgr = new MCCManager(sConnectionString);
                        mccMgr.Maintenance_UpdateEngineHours(long.Parse(VehicleId), engineHours, sn.UserID);
                        RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", saveSucceed));
                        BinddgMaintenance(true);
                    }
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", errorSave));
                }

            }
        }

    }
}