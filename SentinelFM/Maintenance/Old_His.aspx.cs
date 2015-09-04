using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Data;
using System.IO;
using net.mappoint.service;

namespace SentinelFM
{

    public partial class Maintenance_Old_His : SentinelFMBasePage
    {
        ServerDBFleet.DBFleet fleetProxy = new ServerDBFleet.DBFleet();
        ServerDBVehicle.DBVehicle vehicleProxy = new ServerDBVehicle.DBVehicle();
        public string strUnitOfMes = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                strUnitOfMes = sn.User.UnitOfMes == 1 ? " (Km)" : " (Mi)";
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
                }

                if (!IsPostBack)
                {
                    this.Master.CreateMaintenenceMenu(null);
                }
                FleetVehicle1.isLoadDefault = true;
                FleetVehicle1.Vehicle_SelectedIndexChanged += new EventHandler(FleetVehicle1_Vehicle_SelectedIndexChanged);
                FleetVehicle1.Fleet_SelectedIndexChanged += new EventHandler(FleetVehicle1_Fleet_SelectedIndexChanged);

                this.Master.resizeScript = SetResizeScript(dgHistory.ClientID);
                this.Master.isHideScroll = true;
                dgHistory.RadAjaxManagerControl = RadAjaxManager1;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                base.RedirectToLogin();
            }
            Literal lit = new Literal();
            lit.Text = "<link href='MaintenanceStyleSheet.css' type='text/css' rel='stylesheet'></link>";
            this.Page.Header.Controls.Add(lit);
            lit = new Literal();
            lit.Text = "<link href='maintenance.css' type='text/css' rel='stylesheet'></link>";
            this.Page.Header.Controls.Add(lit);
            lit = new Literal();
            lit.Text = "<script type='text/javascript' src='maintenance.js'></script>";
            this.Page.Header.Controls.Add(lit);


        }

        protected void dgHistory_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if ((sn.History.DsMaintenanceHistory != null) && (sn.History.DsMaintenanceHistory.Tables[0] != null) )
            {
                dgHistory.DataSource = sn.History.DsMaintenanceHistory;
            }
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("VehicleDescription");
                dgHistory.DataSource = dt;
            }

        }
        protected void BinddgMaintenance(Boolean isBind)
        {
            //using (Vehicle vehicle = new Vehicle(connectionString))
            //{
            //this.gvHistory.DataSource = vehicle.GetVehicleServicesHistory(this.FleetVehicleSelectorHistory.SelectedVehicle);
            //this.gvHistory.DataBind();
            //}
            DataSet ds = null;
            string xml = "";
            if (FleetVehicle1.GetSelectedVehicle() != "-999")
            {
                if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlanHistory_GetAll(sn.UserID, sn.SecId, sn.User.OrganizationId, long.Parse(this.FleetVehicle1.GetSelectedVehicle()), ref xml), false))
                    if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlanHistory_GetAll(sn.UserID, sn.SecId, sn.User.OrganizationId, long.Parse(this.FleetVehicle1.GetSelectedVehicle()), ref xml), true))
                    {
                        //return;
                        string error = string.Format("alert('{0}')", base.GetLocalResourceObject("LoadHistoryError").ToString());
                        RadAjaxManager1.ResponseScripts.Add(error);
                    }
            }
            else
            {
                if (objUtil.ErrCheck(fleetProxy.FleetGetServicesHistory(sn.UserID, sn.SecId, int.Parse(this.FleetVehicle1.GetSelectedFleet()), ref xml), false))
                    if (objUtil.ErrCheck(fleetProxy.FleetGetServicesHistory(sn.UserID, sn.SecId, int.Parse(this.FleetVehicle1.GetSelectedFleet()), ref xml), true))
                    {
                        //return;
                        string error = string.Format("alert('{0}')", base.GetLocalResourceObject("LoadHistoryError").ToString());
                        RadAjaxManager1.ResponseScripts.Add(error);

                    }
            }

            //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
            //string strPath = MapPath(DataSetPath) + @"\MaintenanceHistory.xsd";

            string strPath = Server.MapPath("../Datasets/MaintenanceHistory.xsd");

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            base.InitializeCulture();

            if (!String.IsNullOrEmpty(xml))
            {
                ds = new DataSet("History");
                ds.ReadXmlSchema(strPath);
                ds.ReadXml(new StringReader(xml));
                sn.History.DsMaintenanceHistory = ds;
                dgHistory.DataSource = ds;
            }
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("VehicleDescription");
                dgHistory.DataSource = dt;
                sn.History.DsMaintenanceHistory = null;
            }

            if (!dgHistory.MasterTableView.Columns[3].HeaderText.Contains(strUnitOfMes))
                this.dgHistory.MasterTableView.Columns[3].HeaderText = this.dgHistory.MasterTableView.Columns[3].HeaderText + strUnitOfMes;

            if (isBind) dgHistory.DataBind();
            //if (Util.IsDataSetValid(ds))
            //    this.gvHistory.DataSource = ds.Tables[0];
            //else
            //    this.gvHistory.DataSource = null;
            //this.gvHistory.DataBind();
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

        void FleetVehicle1_Vehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            BinddgMaintenance(true);
        }

    }
}