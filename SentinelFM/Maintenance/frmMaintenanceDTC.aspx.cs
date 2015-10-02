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

    public partial class Maintenance_frmMaintenanceDTC : SentinelFMBasePage
    {
        ServerDBOrganization.DBOrganization orgProxy = new ServerDBOrganization.DBOrganization();
        ServerDBVehicle.DBVehicle vehicleProxy = new ServerDBVehicle.DBVehicle();
        ServerDBFleet.DBFleet fleetProxy = new ServerDBFleet.DBFleet();
        //public string adminText = "Admin ▼"; //(string)base.GetLocalResourceObject("adminText");
        //string errorSave = "Save failed."; //(string)base.GetLocalResourceObject("errorSave");
        //string errorDelete = "Delete failed."; //(string)base.GetLocalResourceObject("errorDelete");
        //public string selectFleet = "Select a Fleet"; //(string)base.GetLocalResourceObject("selectFleet");
        //string errorDeleteAssign = "Can not be deleted because it has been assigned to vehicle(s)."; //(string)base.GetLocalResourceObject("errorDeleteAssign");
        //string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        //public string strUnitOfMes = "";
        //public string showFilter = "Show Filter"; //(string)base.GetLocalResourceObject("showFilter");
        //public string hideFilter = "Hide Filter";  //(string)base.GetLocalResourceObject("hideFilter");
        //public static string notification1Txt = "Notification1: "; //(string)base.GetLocalResourceObject("notification1Txt");
        //public static string notification2Txt = "Notification2: "; //(string)base.GetLocalResourceObject("notification2Txt");
        //public static string notification3Txt = "Notification3: "; //(string)base.GetLocalResourceObject("notification3Txt");
        //public string Error_Load = "Failed to load data.";  //(string)base.GetLocalResourceObject("Error_Load");
        //public string selectGroup = "Please select MCC group"; //(string)base.GetLocalResourceObject("selectGroup");
        //public string selectMaintenance = "Please select MCC maintenance"; //(string)base.GetLocalResourceObject("selectMaintenance");


        
        public string adminText = "";//(string)GetLocalResourceObject("adminText");
        string errorSave = "";//(string)base.GetLocalResourceObject("errorSave");
        string errorDelete = "";//(string)base.GetLocalResourceObject("errorDelete");
        public string selectFleet = "";//(string)base.GetLocalResourceObject("selectFleet");
        string errorDeleteAssign = "";//(string)base.GetLocalResourceObject("errorDeleteAssign");
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        public string strUnitOfMes = "";
        public string showFilter = "";//(string)base.GetLocalResourceObject("showFilter");
        public string hideFilter = "";//(string)base.GetLocalResourceObject("hideFilter");
        public static string notification1Txt = "";// (string)base.GetLocalResourceObject("notification1Txt");
        public static string notification2Txt = "";//(string)base.GetLocalResourceObject("notification2Txt");
        public static string notification3Txt = "";//(string)base.GetLocalResourceObject("notification3Txt");
        public string Error_Load = "";//(string)base.GetLocalResourceObject("Error_Load");
        public string selectGroup = "";//(string)base.GetLocalResourceObject("selectGroup");
        public string selectMaintenance = "";//(string)base.GetLocalResourceObject("selectMaintenance");

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
            adminText = (string)GetLocalResourceObject("adminText");
         errorSave = (string)base.GetLocalResourceObject("errorSave");
         errorDelete = (string)base.GetLocalResourceObject("errorDelete");
          selectFleet =(string)base.GetLocalResourceObject("selectFleet");
         errorDeleteAssign = (string)base.GetLocalResourceObject("errorDeleteAssign");
          showFilter = (string)base.GetLocalResourceObject("showFilter");
          hideFilter = (string)base.GetLocalResourceObject("hideFilter");
           notification1Txt = (string)base.GetLocalResourceObject("notification1Txt");
           notification2Txt = (string)base.GetLocalResourceObject("notification2Txt");
          notification3Txt = (string)base.GetLocalResourceObject("notification3Txt");
         Error_Load = (string)base.GetLocalResourceObject("Error_Load");
        selectGroup = (string)base.GetLocalResourceObject("selectGroup");
         selectMaintenance =(string)base.GetLocalResourceObject("selectMaintenance");

            try
            {
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
                }


                strUnitOfMes = sn.User.UnitOfMes == 1 ? " (Km)" : " (Mi)";

                if (!IsPostBack)
                {
                    //using (Organization org = new Organization(connectionString))
                    //{
                    //BindNotifications();
                    //BindOrganizationServices();
                    //}

                    clsMisc.cboHoursFill(ref cboHoursFrom);
                    clsMisc.cboHoursFill(ref cboHoursTo);

                    this.txtFrom.SelectedDate = DateTime.Now.AddHours(-24);
                    this.txtFrom.DateInput.DateFormat = sn.User.DateFormat;
                    this.txtTo.SelectedDate = DateTime.Now.AddHours(1);
                    this.txtTo.DateInput.DateFormat = sn.User.DateFormat;

                    this.Master.CreateMaintenenceMenu(null);

                }

                FleetVehicle1.isLoadDefault = true;

                this.Master.resizeScript = SetResizeScript(dgDTCCode.ClientID);
                this.Master.isHideScroll = true;
                dgDTCCode.RadAjaxManagerControl = RadAjaxManager1;

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
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                base.RedirectToLogin();
            }

        }


        private void BinddgDTCCode(bool isBind)
        {
            if ((sn.Maint.DsDTCcodes != null) && (sn.Maint.DsDTCcodes.Tables[0] != null) )
            {
                dgDTCCode.DataSource = sn.Maint.DsDTCcodes;
            }
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Description");

                dgDTCCode.DataSource = dt;

            }

            if (isBind) dgDTCCode.DataBind();
        }

        protected void dgDTCCode_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
                BinddgDTCCode(false);
        }

        protected void dgDTCCode_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)// to access a row 
            {
                ((GridDataItem)e.Item)["MaxDateTime"].Text = Convert.ToDateTime(((GridDataItem)e.Item)["MaxDateTime"].Text).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
            }
        }


        protected void cmdViewDTCCodes_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDTCcodes_NewTZ();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                string errorMsg = string.Format("alert(\"{0}\");", Error_Load);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);

            }

        }

        // Changes for TimeZone Feature start
        private void LoadDTCcodes_NewTZ()
        {
            string xml = "";

            string strFromDate = "";
            string strToDate = "";

            if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
                strFromDate = this.txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

            if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
                strFromDate = this.txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

            if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
                strFromDate = this.txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

            if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
                strToDate = this.txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

            if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
                strToDate = this.txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

            if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
                strToDate = this.txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";

            try
            {
                RadComboBox vehicleCombs = (RadComboBox)FleetVehicle1.FindControl("cboVehicle");
                lblMessage.Visible = false;
                int fleetId = 0;
                if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                {                    
                    HiddenField _hidOrganizationHierarchyFleetId = (HiddenField)FleetVehicle1.FindControl("hidOrganizationHierarchyFleetId");
                    if (_hidOrganizationHierarchyFleetId.Value.ToString().Contains(",") && Convert.ToInt32(vehicleCombs.SelectedItem.Value)==-999)
                    {
                        lblMessage.Visible = true;
                        lblMessage.Text = (string)base.GetLocalResourceObject("lblOnlyOneHierarchyForEntireFleet");
                        return;
                    }
                    else
                        int.TryParse(_hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);                    
                }
                else
                {
                    RadComboBox fleetCombs = (RadComboBox)FleetVehicle1.FindControl("cboFleet");
                    fleetId = Convert.ToInt32(fleetCombs.SelectedItem.Value);
                }
                
                if (objUtil.ErrCheck(vehicleProxy.GetJ1708CodesVehicleFleet_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(vehicleCombs.SelectedItem.Value), fleetId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(vehicleProxy.GetJ1708CodesVehicleFleet_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(vehicleCombs.SelectedItem.Value), fleetId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        sn.Maint.DsDTCcodes = null;
                        //dgDTCCode.RootTable.Rows.Clear();
                        //dgDTCCode.ClearCachedDataSource();
                        //dgDTCCode.RebindDataSource();
                        BinddgDTCCode(true);
                        return;
                    }

                if (xml == "")
                {
                    //dgDTCCode.RootTable.Rows.Clear();
                    //dgDTCCode.ClearCachedDataSource();
                    //dgDTCCode.RebindDataSource();
                    DataSet ds1 = new DataSet();

                    string strPath1 = Server.MapPath("../Datasets/DTCcodes.xsd");
                    ds1.ReadXmlSchema(strPath1);
                    sn.Maint.DsDTCcodes = ds1;

                    BinddgDTCCode(true);
                    return;
                }

                StringReader strrXML = new StringReader(xml);
                DataSet ds = new DataSet();

                string strPath = Server.MapPath("../Datasets/DTCcodes.xsd");
                ds.ReadXmlSchema(strPath);
                ds.ReadXml(strrXML);
                sn.Maint.DsDTCcodes = ds;
                //dgDTCCode.ClearCachedDataSource();
                //dgDTCCode.RebindDataSource();
                BinddgDTCCode(true);
            }
            catch
            {
            }
        }

        // Changes for TimeZone Feature end

        private void LoadDTCcodes()
        {
            string xml = "";

            string strFromDate = "";
            string strToDate = "";

            if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
                strFromDate = this.txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

            if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
                strFromDate = this.txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

            if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
                strFromDate = this.txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

            if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
                strToDate = this.txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

            if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
                strToDate = this.txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

            if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
                strToDate = this.txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";

            try
            {
                RadComboBox vehicleCombs = (RadComboBox)FleetVehicle1.FindControl("cboVehicle");
                lblMessage.Visible = false;
                int fleetId = 0;
                if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                {
                    HiddenField _hidOrganizationHierarchyFleetId = (HiddenField)FleetVehicle1.FindControl("hidOrganizationHierarchyFleetId");
                    if (_hidOrganizationHierarchyFleetId.Value.ToString().Contains(",") && Convert.ToInt32(vehicleCombs.SelectedItem.Value) == -999)
                    {
                        lblMessage.Visible = true;
                        lblMessage.Text = (string)base.GetLocalResourceObject("lblOnlyOneHierarchyForEntireFleet");
                        return;
                    }
                    else
                        int.TryParse(_hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
                }
                else
                {
                    RadComboBox fleetCombs = (RadComboBox)FleetVehicle1.FindControl("cboFleet");
                    fleetId = Convert.ToInt32(fleetCombs.SelectedItem.Value);
                }
                if (objUtil.ErrCheck(vehicleProxy.GetJ1708CodesVehicleFleet(sn.UserID, sn.SecId, Convert.ToInt32(vehicleCombs.SelectedItem.Value), fleetId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(vehicleProxy.GetJ1708CodesVehicleFleet(sn.UserID, sn.SecId, Convert.ToInt32(vehicleCombs.SelectedItem.Value), fleetId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        sn.Maint.DsDTCcodes = null;
                        //dgDTCCode.RootTable.Rows.Clear();
                        //dgDTCCode.ClearCachedDataSource();
                        //dgDTCCode.RebindDataSource();
                        BinddgDTCCode(true);
                        return;
                    }

                if (xml == "")
                {
                    //dgDTCCode.RootTable.Rows.Clear();
                    //dgDTCCode.ClearCachedDataSource();
                    //dgDTCCode.RebindDataSource();
                    DataSet ds1 = new DataSet();

                    string strPath1 = Server.MapPath("../Datasets/DTCcodes.xsd");
                    ds1.ReadXmlSchema(strPath1);
                    sn.Maint.DsDTCcodes = ds1;

                    BinddgDTCCode(true);
                    return;
                }

                StringReader strrXML = new StringReader(xml);
                DataSet ds = new DataSet();

                string strPath = Server.MapPath("../Datasets/DTCcodes.xsd");
                ds.ReadXmlSchema(strPath);
                ds.ReadXml(strrXML);
                sn.Maint.DsDTCcodes = ds;
                //dgDTCCode.ClearCachedDataSource();
                //dgDTCCode.RebindDataSource();
                BinddgDTCCode(true);
            }
            catch
            {
            }
        }
    }
}
