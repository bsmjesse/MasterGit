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
    public partial class Maintenance_frmMaintenanceHst : SentinelFMBasePage
    {
        ServerDBOrganization.DBOrganization orgProxy = new ServerDBOrganization.DBOrganization();
        ServerDBVehicle.DBVehicle vehicleProxy = new ServerDBVehicle.DBVehicle();
        ServerDBFleet.DBFleet fleetProxy = new ServerDBFleet.DBFleet();

        public string adminText = "Admin ▼"; //(string)base.GetLocalResourceObject("adminText");
        string errorSave = "Save failed."; //(string)base.GetLocalResourceObject("errorSave");
        string errorDelete = "Delete failed."; //(string)base.GetLocalResourceObject("errorDelete");
        public string selectFleet = "Select a Fleet"; //(string)base.GetLocalResourceObject("selectFleet");
        string errorDeleteAssign = "Can not be deleted because it has been assigned to vehicle(s)."; //(string)base.GetLocalResourceObject("errorDeleteAssign");
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        public string strUnitOfMes = "";
        public string showFilter = "Show Filter"; //(string)base.GetLocalResourceObject("showFilter");
        public string hideFilter = "Hide Filter";  //(string)base.GetLocalResourceObject("hideFilter");
        public static string notification1Txt = "Notification1: "; //(string)base.GetLocalResourceObject("notification1Txt");
        public static string notification2Txt = "Notification2: "; //(string)base.GetLocalResourceObject("notification2Txt");
        public static string notification3Txt = "Notification3: "; //(string)base.GetLocalResourceObject("notification3Txt");
        public string Error_Load = "Failed to load data.";  //(string)base.GetLocalResourceObject("Error_Load");
        public string selectGroup = "Please select MCC group"; //(string)base.GetLocalResourceObject("selectGroup");
        public string selectMaintenance = "Please select MCC maintenance"; //(string)base.GetLocalResourceObject("selectMaintenance");

        DataTable dtOperationType = null;
        DataTable maintenanceServices = null;
        /// <summary>
        /// Storing selected on he grid service id for further saving
        /// </summary>
        private static int SelectedServiceID;

        /// <summary>
        /// Storing Due or Past Maintenance Services mode for grid paging and data binding
        /// </summary>
        private static short ServiceMode = 0;

        /// <summary>
        /// Add new plan = false; modify = true
        /// </summary>
        private static bool _editMode = false;

        DataTable mccAssignmentDt = null;


        public bool MaintenancePlanEditMode
        {
            get
            {
                return _editMode;
            }
            set
            {
                _editMode = value;
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            if (XmlOperationType == null) XmlOperationType = new XmlDataSource();
            this.XmlOperationType.DataFile = base.GetLocalResourceObject("XmlDSOperationType_DataFile").ToString();
            base.OnPreInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

              adminText = (string)base.GetLocalResourceObject("adminText");
         errorSave = (string)base.GetLocalResourceObject("errorSave");
         errorDelete = (string)base.GetLocalResourceObject("errorDelete");
         selectFleet = (string)base.GetLocalResourceObject("selectFleet");
         errorDeleteAssign = (string)base.GetLocalResourceObject("errorDeleteAssign");
         showFilter = (string)base.GetLocalResourceObject("showFilter");
         hideFilter = (string)base.GetLocalResourceObject("hideFilter");
         notification1Txt = (string)base.GetLocalResourceObject("notification1Txt");
        notification2Txt = (string)base.GetLocalResourceObject("notification2Txt");
         notification3Txt = (string)base.GetLocalResourceObject("notification3Txt");
         Error_Load = (string)base.GetLocalResourceObject("Error_Load");
         selectGroup = (string)base.GetLocalResourceObject("selectGroup");
         selectMaintenance = (string)base.GetLocalResourceObject("selectMaintenance");

            try
            {
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
                }


                strUnitOfMes = sn.User.UnitOfMes == 1 ? " (Km)" : " (Mi)";

                if (!IsPostBack)
                {
                    this.Master.CreateMaintenenceMenu(null);


                }
                FleetVehicle1.isLoadDefault = true;
                FleetVehicle1.Vehicle_SelectedIndexChanged += new EventHandler(FleetVehicle1_Vehicle_SelectedIndexChanged);
                FleetVehicle1.Fleet_SelectedIndexChanged += new EventHandler(FleetVehicle1_Fleet_SelectedIndexChanged);
                FleetVehicle1.OrganizationHierarchySelectChanged += new EventHandler(FleetVehicle1_OrganizationHierarchySelectChanged);
                //FleetVehicle1.radAjaxManager1 = RadAjaxManager1;
                //FleetVehicle1.radAjaxLoadingPanel1 = LoadingPanel1.ID;
                //FleetVehicle1.radUpdatedControl = pnl.ID;
                dgMaintenance.RadAjaxManagerControl = RadAjaxManager1;

                this.Master.resizeScript = SetResizeScript(dgMaintenance.ClientID);
                this.Master.isHideScroll = true;
                dgMaintenance.RadAjaxManagerControl = RadAjaxManager1;

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

        void FleetVehicle1_Vehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            BinddgMaintenance(true);
        }

        protected void dgMaintenance_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BinddgMaintenance(false);
        }

        private void BinddgMaintenance(Boolean isBind)
        {
            try
            {
                DateTime dt = System.DateTime.Now.Date;

                if (ddlDateTime.SelectedValue == "1") dt = dt.AddDays(-7);
                if (ddlDateTime.SelectedValue == "2") dt = dt.AddDays(-14);
                if (ddlDateTime.SelectedValue == "3") dt = dt.AddMonths(-1);
                if (ddlDateTime.SelectedValue == "4") dt = dt.AddMonths(-2);
                if (ddlDateTime.SelectedValue == "5") dt = dt.AddMonths(-3);
                if (ddlDateTime.SelectedValue == "6") dt = dt.AddMonths(-6);
                if (ddlDateTime.SelectedValue == "7") dt = dt.AddMonths(-12);
                if (ddlDateTime.SelectedValue == "8") dt = dt.AddYears(-2);
                if (ddlDateTime.SelectedValue == "9") dt = new DateTime(1970,1,1);
                MCCManager mccMgr = new MCCManager(sConnectionString);
                DataSet ds = mccMgr.MaintenanceGetVehicleServicesHistory(FleetVehicle1.GetAllSelectedVehicle(), sn.UserID, dt);
                //to be changed
                dgMaintenance.DataSource = ds;
                if (dtOperationType == null)
                {
                    dtOperationType = clsAsynGenerateReport.GetOperationTypeTable();
                }
                if (isBind)
                {
                    dgMaintenance.DataBind();
                }
            }
            catch (Exception Ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("BoxId");
                dt.Columns.Add("VehicleDescription");
                dgMaintenance.DataSource = dt;
                if (isBind)
                {
                    dgMaintenance.DataBind();
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", Error_Load));
            }
        }

        public string FindOperationType(object obj)
        {
            if (obj == DBNull.Value) return string.Empty;
            if (dtOperationType == null) return obj.ToString();
            DataRow[] dr = dtOperationType.Select("id=" + obj.ToString());
            if (dr != null && dr.Length > 0) return dr[0]["description"].ToString();
            else return string.Empty;
        }
        protected void ddlDateTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            BinddgMaintenance(true);
        }
}
}