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

namespace SentinelFM
{
    public partial class Maintenance_frmMaintenance : SentinelFMBasePage
    {
        //protected static string connectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

        ServerDBOrganization.DBOrganization orgProxy = new ServerDBOrganization.DBOrganization();
        ServerDBVehicle.DBVehicle vehicleProxy = new ServerDBVehicle.DBVehicle();
        ServerDBFleet.DBFleet fleetProxy = new ServerDBFleet.DBFleet();

        public string strUnitOfMes = "";
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

        public bool MaintenancePlanEditMode
        {
            get
            {
                return _editMode;
            }
            set
            {
                _editMode = value;
                this.trNewPlanService.Visible = value;
                this.trNewPlanNotification.Visible = value;
                this.trNewPlanSchedule.Visible = value;
                this.trDueValue.Visible = value;
                this.trInterval.Visible = value;
                this.trEndValue.Visible = value;
            }
        }

        # region General
        protected override void OnPreInit(EventArgs e)
        {
            //if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
            //   this.XmlOperationType.DataFile = "~/App_Data/OperationType.French.xml";
            //else
            //   this.XmlOperationType.DataFile = "~/App_Data/OperationType.xml";
            this.XmlOperationType.DataFile = base.GetLocalResourceObject("XmlDSOperationType_DataFile").ToString();
            base.OnPreInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                strUnitOfMes = sn.User.UnitOfMes == 1 ? " (Km)" : " (Mi)";

                if (!IsPostBack)
                {
                    //using (Organization org = new Organization(connectionString))
                    //{
                    //BindNotifications();
                    //BindOrganizationServices();
                    //}
                    if (MultiView1.ActiveViewIndex == 0)
                    {
                        ClearDataCached();
                        ClearSelectedTab();
                        this.btnMaintenance.CssClass = "selectedbutton";
                        FleetVehicleSelectorMaintenance.ShowEntireFleet = true;

                        if (sn.User.DefaultFleet != -1)
                        {
                            FleetVehicleSelectorMaintenance.SelectedFleet = sn.User.DefaultFleet;
                            FleetVehicleSelectorMaintenance.ShowEntireFleet = true;
                            FleetVehicleSelectorMaintenance.SelectedVehicle = -999;
                            DisplayMaintenanceDataPerSelection();
                        }

                    }

                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmMaintenance, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                    FleetVehicleSelectorMaintenance.ShowEntireFleet = true;
                    CboFleet_Fill();
                    this.cboFleet.SelectedIndex = 0;
                    CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
                    this.cboVehicle.SelectedIndex = 0;

                    clsMisc.cboHoursFill(ref cboHoursFrom);
                    clsMisc.cboHoursFill(ref cboHoursTo);

                    this.txtFrom.Text = DateTime.Now.AddHours(-24).ToString("MM/dd/yyyy");
                    this.txtTo.Text = DateTime.Now.AddHours(1).ToString("MM/dd/yyyy");
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                base.RedirectToLogin();
            }
        }

        protected void btnMaintenance_Click(object sender, EventArgs e)
        {
            this.MultiView1.ActiveViewIndex = 0;
            ClearDataCached();
            ClearSelectedTab();
            this.btnMaintenance.CssClass = "selectedbutton";
            FleetVehicleSelectorMaintenance.ShowEntireFleet = true;
            if (sn.User.DefaultFleet != -1)
            {
                FleetVehicleSelectorMaintenance.SelectedFleet = sn.User.DefaultFleet;
                FleetVehicleSelectorMaintenance.ShowEntireFleet = true;
                FleetVehicleSelectorMaintenance.SelectedVehicle = -999;
                DisplayMaintenanceDataPerSelection();
            }
        }

        protected void btnAdministration_Click(object sender, EventArgs e)
        {
            try
            {
                this.MultiView1.ActiveViewIndex = 1;
                ClearDataCached();
                ClearSelectedTab();
                this.btnAdministration.CssClass = "selectedbutton";
                BindNotifications();
            }
            catch (Exception Ex)
            {
                base.ShowMessage(this.lblNotificationMessage, base.GetLocalResourceObject("NotificationsLoadError").ToString(), Color.Red);
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //base.RedirectToLogin();
            }
        }

        protected void btnMaintenanceHistory_Click(object sender, EventArgs e)
        {
            this.MultiView1.ActiveViewIndex = 3;
            ClearDataCached();
            ClearSelectedTab();
            this.btnMaintenanceHistory.CssClass = "selectedbutton";
            FleetVehicleSelectorMaintenance.ShowEntireFleet = true;
            GetVehicleHistory(sender, e);
        }

        protected void btnServices_Click(object sender, EventArgs e)
        {
            try
            {
                this.MultiView1.ActiveViewIndex = 2;
                ClearDataCached();
                ClearSelectedTab();
                this.btnServices.CssClass = "selectedbutton";
                BindOrganizationServices();
            }
            catch (Exception Ex)
            {
                base.ShowMessage(this.lblServicesMessage, base.GetLocalResourceObject("ServicesLoadError").ToString(), Color.Red);
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //base.RedirectToLogin();
            }
        }

        private void GetOperationTypes(object selectedValue)
        {
            DataSet ds = new DataSet();
            try
            {
                string xml = "";

                if (objUtil.ErrCheck(vehicleProxy.VehicleOperationTypes_Get(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                    if (objUtil.ErrCheck(vehicleProxy.VehicleOperationTypes_Get(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                    {
                        return;
                    }
                if (!String.IsNullOrEmpty(xml))
                    ds.ReadXml(new StringReader(xml));

                if (Util.IsDataSetValid(ds))
                {
                    this.ddlNewPlanType.DataSource = ds;
                    this.ddlNewPlanType.DataBind();

                    if (selectedValue != null)
                        this.ddlNewPlanType.SelectedValue = selectedValue.ToString();
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                return;
            }
        }
        # endregion General

        # region Notifications tab
        protected void btnAddNotification_Click(object sender, EventArgs e)
        {
            this.NotificationDetails.Visible = true;
            this.btnAddNotification.Enabled = false;
            //this.btnSave.Text = "Add";
            this.lblNotificationMessage.Text = "";
            ClearDetails();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // add new notification for the company
            try
            {
                this.lblNotificationMessage.Text = "";
                if (this.ddlNotificationType.SelectedValue == "0")
                {
                    base.ShowMessage(this.lblNotificationMessage, "Invalid Notification Type", Color.Red);
                    return;
                }
                //if (!ValidateData()) return;
                //using (Organization org = new Organization(connectionString))
                //{
                if (objUtil.ErrCheck(orgProxy.Notification_Add(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(this.ddlNotificationType.SelectedValue),
                      ParseNotification(this.txtNotification1.Text),
                      ParseNotification(this.txtWarning.Text),
                      ParseNotification(this.txtLastWarning.Text),
                      this.txtNotificationDescription.Text), false))
                    if (objUtil.ErrCheck(orgProxy.Notification_Add(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(this.ddlNotificationType.SelectedValue),
                       ParseNotification(this.txtNotification1.Text),
                       ParseNotification(this.txtWarning.Text),
                       ParseNotification(this.txtLastWarning.Text),
                       this.txtNotificationDescription.Text), true))
                    {
                        return;
                    }
                //org.AddVehicleServiceNotification(sn.User.OrganizationId,
                //   Convert.ToInt16(this.ddlNotificationType.SelectedValue),
                //   ParseNotification(this.txtNotification1.Text),
                //   ParseNotification(this.txtWarning.Text),
                //   ParseNotification(this.txtLastWarning.Text),
                //   this.txtNotificationDescription.Text);

                this.NotificationDetails.Visible = false;
                this.btnAddNotification.Enabled = true;
                base.ShowMessage(this.lblNotificationMessage, base.GetLocalResourceObject("NewNotificationAdded").ToString(), Color.Green);
                BindNotifications();
                //}
            }
            catch (Exception Ex)
            {
                base.ShowMessage(this.lblNotificationMessage, base.GetLocalResourceObject("SaveNotificationError").ToString(), Color.Red);
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.NotificationDetails.Visible = false;
            this.btnAddNotification.Enabled = true;
            this.lblNotificationMessage.Text = "";
        }

        private void ClearDetails()
        {
            //this.ddlNotificationType.SelectedIndex = 0;
            this.txtNotification1.Text = "";
            this.txtWarning.Text = "";
            this.txtLastWarning.Text = "";
            this.txtNotificationDescription.Text = "";
            //this.txtEmail.Text = "";
        }

        private short ParseNotification(string notifText)
        {
            short not = 0;
            if (!short.TryParse(notifText, out not))
                throw new FormatException("Invalid Notification Value: " + notifText);
            return not;
        }

        protected void gdvNotifications_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                this.lblNotificationMessage.Text = "";
                this.btnAddNotification.Enabled = false;
                gdvNotifications.EditIndex = e.NewEditIndex;
                //DropDownList ddl = (DropDownList)gdvNotifications.Rows[e.NewEditIndex].FindControl("ddlNotTypeEdit");
                //ddl.DataSource = this.GetOperationTypes();
                //ddl.DataBind();
                BindNotifications();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void gdvNotifications_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            this.lblNotificationMessage.Text = "";
            e.Cancel = true;
            gdvNotifications.EditIndex = -1;
            //using (Organization org = new Organization(connectionString))
            //{
            BindNotifications();
            //}
            this.btnAddNotification.Enabled = true;
        }

        protected void gdvNotifications_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            if (e.Exception == null)
                this.lblNotificationMessage.Text = base.GetLocalResourceObject("NotificationUpdatedSuccessfully").ToString();
            else
            {
                e.ExceptionHandled = true;
                this.lblNotificationMessage.Text = base.GetLocalResourceObject("NotificationUpdateError").ToString();
            }
            //gdvNotifications.EditIndex = -1;
            //LoadNotifications();
            //e.KeepInEditMode = false;
            //this.btnAddNotification.Enabled = true;
        }

        protected void gdvNotifications_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                this.lblNotificationMessage.Text = "";
                //StringBuilder kv = new StringBuilder();
                short notif1, notif2, notif3;
                GridViewRow currentRow = gdvNotifications.Rows[gdvNotifications.EditIndex];

                short notifId = Convert.ToInt16(gdvNotifications.DataKeys[e.RowIndex].Value);
                //Convert.ToInt16(currentRow.Cells[0].Text);
                short notifTypeId = Convert.ToInt16(((DropDownList)(currentRow.Cells[2].Controls[1])).SelectedValue);
                if (notifTypeId == 0)
                {
                    base.ShowMessage(this.lblNotificationMessage, "Invalid Notification Type", Color.Red);
                    return;
                }

                if (!Int16.TryParse(((TextBox)(currentRow.Cells[3].Controls[0])).Text, out notif1))
                {
                    base.ShowMessage(this.lblNotificationMessage, base.GetLocalResourceObject("InvalidNotification").ToString(), Color.Red);
                    return;
                }
                if (!Int16.TryParse(((TextBox)(currentRow.Cells[4].Controls[0])).Text, out notif2))
                {
                    base.ShowMessage(this.lblNotificationMessage, base.GetLocalResourceObject("InvalidWarning").ToString(), Color.Red);
                    return;
                }
                if (!Int16.TryParse(((TextBox)(currentRow.Cells[5].Controls[0])).Text, out notif3))
                {
                    base.ShowMessage(this.lblNotificationMessage, base.GetLocalResourceObject("InvalidLastWarning").ToString(), Color.Red);
                    return;
                }
                string descr = (currentRow.Cells[6].Controls[0] as TextBox).Text;

                if (descr == "")
                {
                    base.ShowMessage(this.lblNotificationMessage, "Please enter a description", Color.Red);
                    return;
                }

                //using (Organization org = new Organization(connectionString))
                //{
                if (objUtil.ErrCheck(orgProxy.Notification_Update(sn.UserID, sn.SecId, sn.User.OrganizationId, notifId, notifTypeId, notif1, notif2, notif3, descr), false))
                    if (objUtil.ErrCheck(orgProxy.Notification_Update(sn.UserID, sn.SecId, sn.User.OrganizationId, notifId, notifTypeId, notif1, notif2, notif3, descr), true))
                    {
                        return;
                    }
                //org.UpdateVehicleServiceNotification(sn.User.OrganizationId, notifId, notifTypeId, notif1, notif2, notif3, descr);
                gdvNotifications.EditIndex = -1;
                //gdvNotifications.DataBind();
                BindNotifications();
                this.btnAddNotification.Enabled = true;
                base.ShowMessage(this.lblNotificationMessage, base.GetLocalResourceObject("NotificationUpdated").ToString(), Color.Green);
                //}
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //RedirectToLogin();
                base.ShowMessage(this.lblNotificationMessage, Ex.Message, Color.Red);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //RedirectToLogin();
                base.ShowMessage(this.lblNotificationMessage, Ex.Message, Color.Red);
            }
        }

        protected void gdvNotifications_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //   string confirm = "return confirm('Are you sure you want to delete notification?')";
            //   ImageButton cmdBtn = (ImageButton)e.Row.Cells[7].Controls[2];
            //   if (cmdBtn.CommandName == "Delete")
            //      cmdBtn.Attributes.Add("onclick", confirm);
            //}
        }

        protected void btnDeleteNotification_Clicked(object sender, EventArgs e)
        {
            try
            {
                this.lblNotificationMessage.Text = "";

                GridViewRow row = (((sender as ImageButton).Parent as DataControlFieldCell).Parent as GridViewRow);
                int delIndex = row.RowIndex;
                //using (Organization org = new Organization(connectionString))
                //{
                if (objUtil.ErrCheck(orgProxy.Notification_Delete(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(row.Cells[0].Text)), false))
                    if (objUtil.ErrCheck(orgProxy.Notification_Delete(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(row.Cells[0].Text)), true))
                    {
                        base.ShowMessage(this.lblNotificationMessage, base.GetLocalResourceObject("NotificationAssigned").ToString(), Color.Red);
                        return;
                    }
                //org.DeleteVehicleServiceNotification(sn.User.OrganizationId, Convert.ToInt16(row.Cells[0].Text));
                //gdvNotifications.DeleteRow(delIndex);
                BindNotifications();
                base.ShowMessage(this.lblNotificationMessage, base.GetLocalResourceObject("NotificationDeleted").ToString(), Color.Green);
                //}
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                base.RedirectToLogin();
            }
        }

        private void BindNotifications()
        {
            string xml = "";

            if (objUtil.ErrCheck(orgProxy.Notification_GetAllByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                if (objUtil.ErrCheck(orgProxy.Notification_GetAllByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                {
                    return;
                }
            if (String.IsNullOrEmpty(xml))
                return;

            DataSet ds = new DataSet();
            ds.ReadXml(new StringReader(xml));
            if (Util.IsDataSetValid(ds))
            {
                gdvNotifications.DataSource = ds.Tables[0];
                gdvNotifications.DataBind();
            }
        }

        protected void gdvNotifications_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gdvNotifications.PageIndex = e.NewPageIndex;
                BindNotifications();
                gdvNotifications.SelectedIndex = -1;
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
        # endregion Notifications tab

        # region History tab

        //protected void btnGetHistory_Clicked(object sender, EventArgs e)
        //{
        //   GetVehicleHistory(sender, e);
        //}

        /// <summary>
        /// History DropDownList Vehicle Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GetVehicleHistory(object sender, EventArgs e)
        {
            this.LabelHistoryMessage.Text = "";
            //using (Vehicle vehicle = new Vehicle(connectionString))
            //{
            //this.gvHistory.DataSource = vehicle.GetVehicleServicesHistory(this.FleetVehicleSelectorHistory.SelectedVehicle);
            //this.gvHistory.DataBind();
            //}
            DataSet ds = null;
            string xml = "";
            if (FleetVehicleSelectorMaintenance.SelectedVehicle != -999)
            {
                if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlanHistory_GetAll(sn.UserID, sn.SecId, sn.User.OrganizationId, this.FleetVehicleSelectorHistory.SelectedVehicle, ref xml), false))
                    if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlanHistory_GetAll(sn.UserID, sn.SecId, sn.User.OrganizationId, this.FleetVehicleSelectorHistory.SelectedVehicle, ref xml), true))
                    {
                        //return;
                        base.ShowMessage(this.LabelHistoryMessage, base.GetLocalResourceObject("LoadHistoryError").ToString(), Color.Red);
                    }
            }
            else
            {
                if (objUtil.ErrCheck(fleetProxy.FleetGetServicesHistory(sn.UserID, sn.SecId, this.FleetVehicleSelectorHistory.SelectedFleet, ref xml), false))
                    if (objUtil.ErrCheck(fleetProxy.FleetGetServicesHistory(sn.UserID, sn.SecId, this.FleetVehicleSelectorHistory.SelectedFleet, ref xml), true))
                    {
                        //return;
                        base.ShowMessage(this.LabelHistoryMessage, base.GetLocalResourceObject("LoadHistoryError").ToString(), Color.Red);
                    }
            }

            //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
            //string strPath = MapPath(DataSetPath) + @"\MaintenanceHistory.xsd";

            string strPath = Server.MapPath("../Datasets/MaintenanceHistory.xsd");

            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            base.InitializeCulture();

            if (!String.IsNullOrEmpty(xml))
            {
                ds = new DataSet("History");
                ds.ReadXmlSchema(strPath);
                ds.ReadXml(new StringReader(xml));
                sn.History.DsMaintenanceHistory = ds;
            }
            else
            {
                dgHistory.RootTable.Rows.Clear();
                sn.History.DsMaintenanceHistory = null;
            }

            dgHistory.ClearCachedDataSource();
            dgHistory.RebindDataSource();

            if (!dgHistory.RootTable.Columns[3].Caption.Contains(strUnitOfMes))
                this.dgHistory.RootTable.Columns[3].Caption = this.dgHistory.RootTable.Columns[3].Caption + strUnitOfMes;

            //if (Util.IsDataSetValid(ds))
            //    this.gvHistory.DataSource = ds.Tables[0];
            //else
            //    this.gvHistory.DataSource = null;
            //this.gvHistory.DataBind();
        }
        # endregion History tab

        # region Maintenance tab

        protected void ButtonMaintenanceInit_Clicked(object sender, EventArgs e)
        {
            SetVehicleDetails(true, false, false, false, Color.Transparent);
        }

        protected void ButtonMaintenanceSaveEH_Clicked(object sender, EventArgs e)
        {
            try
            {
                //using (Vehicle vehicle = new Vehicle(connectionString))
                //{
                //vehicle.UpdateVehicleCurrentEngHrsInfo(this.FleetVehicleSelectorMaintenance.SelectedVehicle,
                //   Convert.ToInt32(this.txtEngineHours.Text) * 60);
                //}

                Int32 engHours = Convert.ToInt32(Math.Round(Convert.ToSingle(this.txtEngineHours.Text) * 60, 0));
                if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenance_UpdateEngineHours(sn.UserID, sn.SecId, this.FleetVehicleSelectorMaintenance.SelectedVehicle, engHours), false))
                    if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenance_UpdateEngineHours(sn.UserID, sn.SecId, this.FleetVehicleSelectorMaintenance.SelectedVehicle, engHours), true))
                    {
                        return;
                    }

                SetVehicleDetails(false, true, true, true, Color.Silver);
                SelectVehicle(sender, e);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                base.RedirectToLogin();
            }
        }

        protected void ButtonMaintenanceCancel_Clicked(object sender, EventArgs e)
        {
            try
            {
                SetVehicleDetails(false, true, true, true, Color.Silver);
                SelectVehicle(sender, e);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                base.RedirectToLogin();
            }
        }

        /// <summary>
        /// Set Vehicle Details Panel controls
        /// </summary>
        /// <param name="saveVisible">Button Save Visible</param>
        /// <param name="initVisible">Button Init Visible</param>
        /// <param name="hoursReadOnly">TextBox Engine Hours ReadOnly</param>
        /// <param name="backColor">TextBox Engine Hours BackColor</param>
        private void SetVehicleDetails(bool saveButtonVisible, bool initButtonVisible, bool hoursTextReadOnly, bool fleetVehicleEnabled, Color hoursTextBackColor)
        {
            this.tfrSaveEH.Visible = saveButtonVisible;
            this.ButtonMaintenanceInit.Visible = initButtonVisible;
            this.txtEngineHours.BackColor = hoursTextBackColor;
            this.txtEngineHours.ReadOnly = hoursTextReadOnly;
            this.FleetVehicleSelectorMaintenance.Enabled = fleetVehicleEnabled;
        }

        protected void btnCloseServicePlan_Clicked(object sender, EventArgs e)
        {
            try
            {
                this.LabelMaintenanceMessage.Text = "";
                //using (Vehicle car = new Vehicle(connectionString))
                //{
                GridViewRow row = (((sender as ImageButton).Parent as DataControlFieldCell).Parent as GridViewRow);
                if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlan_Close(sn.UserID, sn.SecId, sn.User.OrganizationId,
                   Convert.ToInt64(gvServices.DataKeys[row.RowIndex].Values[0]),
                   Convert.ToInt32(gvServices.DataKeys[row.RowIndex].Values[1])), false))
                    if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlan_Close(sn.UserID, sn.SecId, sn.User.OrganizationId,
                       Convert.ToInt64(gvServices.DataKeys[row.RowIndex].Values[0]),
                       Convert.ToInt32(gvServices.DataKeys[row.RowIndex].Values[1])), true))
                    {
                        return;
                    }
                //car.CloseVehicleService(sn.UserID,
                //   Convert.ToInt64(gvServices.DataKeys[row.RowIndex].Values[0]),
                //   Convert.ToInt32(gvServices.DataKeys[row.RowIndex].Values[1])
                //   );
                BindServicePlans();
                base.ShowMessage(this.LabelMaintenanceMessage, "Maintenance Plan Closed<br />", Color.Green);
                //}
            }
            catch (Exception ex)
            {
                base.ShowMessage(this.LabelMaintenanceMessage, "Maintenance Plan Close Error<br />", Color.Red);
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //base.RedirectToLogin();
            }
        }

        protected void gvServices_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvServices.PageIndex = e.NewPageIndex;
                BindServicePlans();
                gvServices.SelectedIndex = -1;
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void gvServices_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                //this.lblNotificationMessage.Text = "";
                //GridViewRow row = (((sender as ImageButton).Parent as DataControlFieldCell).Parent as GridViewRow);
                if (Convert.ToInt16(gvServices.DataKeys[0].Values[2]) > 3)
                {
                    base.ShowMessage(this.LabelMaintenanceMessage, "You cannot delete past service!", Color.Red);
                    return;
                }
                //int delIndex = row.RowIndex;
                //using (Vehicle car = new Vehicle(connectionString))
                //{
                if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlan_Delete(sn.UserID, sn.SecId, sn.User.OrganizationId,
                   Convert.ToInt64(gvServices.DataKeys[e.RowIndex].Values[0]),
                   Convert.ToInt32(gvServices.DataKeys[e.RowIndex].Values[1])), false))
                    if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlan_Delete(sn.UserID, sn.SecId, sn.User.OrganizationId,
                       Convert.ToInt64(gvServices.DataKeys[e.RowIndex].Values[0]),
                       Convert.ToInt32(gvServices.DataKeys[e.RowIndex].Values[1])), true))
                    {
                        return;
                    }
                //car.DeleteVehicleService(sn.UserID,
                //   Convert.ToInt64(gvServices.DataKeys[e.RowIndex].Values[0]),
                //   Convert.ToInt32(gvServices.DataKeys[e.RowIndex].Values[1])
                //   );

                //gdvNotifications.DeleteRow(delIndex);
                BindServicePlans();
                base.ShowMessage(this.LabelMaintenanceMessage, "Maintenance Plan Deleted<br />", Color.Green);
                //}
            }
            catch (Exception ex)
            {
                base.ShowMessage(this.LabelMaintenanceMessage, "Maintenance Plan Delete Error<br />", Color.Red);
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //base.RedirectToLogin();
            }
        }

        protected void btnDue_Click(object sender, EventArgs e)
        {
            try
            {
                this.LabelMaintenanceMessage.Text = "";
                ServiceMode = 1;
                BindServicePlans();
            }
            catch (Exception ex)
            {
                base.ShowMessage(this.LabelMaintenanceMessage, "Get Maintenance Plan Error<br />", Color.Red);
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //base.RedirectToLogin();
            }
        }

        protected void btnPast_Click(object sender, EventArgs e)
        {
            try
            {
                this.LabelMaintenanceMessage.Text = "";
                ServiceMode = 2;
                BindServicePlans();
            }
            catch (Exception ex)
            {
                base.ShowMessage(this.LabelMaintenanceMessage, "Get Maintenance Plan Error<br />", Color.Red);
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //base.RedirectToLogin();
            }
        }

        private void BindServicePlans()
        {
            if (!this.CheckOdometerBased.Checked && !this.CheckEngineHoursBased.Checked)
            {
                this.gvServices.DataSource = null;
                this.gvServices.DataBind();
                return;
            }
            sn.History.DsDueServices = null;
            //using (Vehicle car = new Vehicle(connectionString))
            //{
            string xml = "";
            string strPath = "";
            DataSet ds = new DataSet("Services");

            //DataSet dsOperationTypes = new DataSet();
            //string OperationTypesXML = XmlOperationType.Data.ToString()  ;
            //strPath = Server.MapPath("../App_Data/OperationType.xml");
            //dsOperationTypes.ReadXml(strPath);

            // get all services
            if (this.CheckOdometerBased.Checked && this.CheckEngineHoursBased.Checked && this.CheckTimeBased.Checked)
            {
                if (FleetVehicleSelectorMaintenance.SelectedVehicle != -999)
                {
                    //ds = car.GetVehicleServices(this.FleetVehicleSelectorMaintenance.SelectedVehicle, this.ServiceMode);
                    if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlan_GetAllByLang(sn.UserID, sn.SecId, sn.User.OrganizationId,
                       this.FleetVehicleSelectorMaintenance.SelectedVehicle, ServiceMode, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                        if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlan_GetAllByLang(sn.UserID, sn.SecId, sn.User.OrganizationId,
                           this.FleetVehicleSelectorMaintenance.SelectedVehicle, ServiceMode, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                        {
                            return;
                        }
                }
                else
                {
                    if (objUtil.ErrCheck(fleetProxy.FleetMaintenancePlan_GetAllByLang(sn.UserID, sn.SecId, sn.User.OrganizationId,
                    FleetVehicleSelectorMaintenance.SelectedFleet, ServiceMode, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                        if (objUtil.ErrCheck(fleetProxy.FleetMaintenancePlan_GetAllByLang(sn.UserID, sn.SecId, sn.User.OrganizationId,
                        FleetVehicleSelectorMaintenance.SelectedFleet, ServiceMode, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                        {
                            return;
                        }
                }
            }
            else
            {
                short value = 0;
                if (this.CheckOdometerBased.Checked)
                    value = (short)OperationTypeEnum.Odometer;
                else
                    if (this.CheckEngineHoursBased.Checked)
                        value = (short)OperationTypeEnum.EngineHours;
                //ds = car.GetVehicleServices(this.FleetVehicleSelectorMaintenance.SelectedVehicle, value, ServiceMode);

                if (FleetVehicleSelectorMaintenance.SelectedVehicle != -999)
                {
                    if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlan_GetByTypeLang(sn.UserID, sn.SecId, sn.User.OrganizationId,
                       this.FleetVehicleSelectorMaintenance.SelectedVehicle, value, ServiceMode, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                        if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlan_GetByTypeLang(sn.UserID, sn.SecId, sn.User.OrganizationId,
                           this.FleetVehicleSelectorMaintenance.SelectedVehicle, value, ServiceMode, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                        {
                            return;
                        }
                }
                else
                {
                    if (objUtil.ErrCheck(fleetProxy.FleetMaintenancePlan_GetByTypeLang(sn.UserID, sn.SecId, sn.User.OrganizationId,
                       this.FleetVehicleSelectorMaintenance.SelectedFleet, value, ServiceMode, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                        if (objUtil.ErrCheck(fleetProxy.FleetMaintenancePlan_GetByTypeLang(sn.UserID, sn.SecId, sn.User.OrganizationId,
                       this.FleetVehicleSelectorMaintenance.SelectedFleet, value, ServiceMode, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                        {
                            return;
                        }
                }
            }
            switch (ServiceMode)
            {
                case 0: // all
                    this.lblMaintenanceServicesLegend.Text = base.GetLocalResourceObject("lblMaintenanceServicesLegendResource1.Text").ToString();
                    break;
                case 1: // due
                    this.lblMaintenanceServicesLegend.Text = base.GetLocalResourceObject("DueMaintenanceServices").ToString();
                    break;
                case 2: // past
                    this.lblMaintenanceServicesLegend.Text = base.GetLocalResourceObject("PastMaintenanceServices").ToString();
                    break;
                default: // invalid
                    this.lblMaintenanceServicesLegend.Text = base.GetLocalResourceObject("lblMaintenanceServicesLegendResource1.Text").ToString();
                    break;
            }
            if (FleetVehicleSelectorMaintenance.SelectedVehicle != -999)
            {
                if (!String.IsNullOrEmpty(xml))
                    ds.ReadXml(new StringReader(xml));

                this.MaintenanceServices.Visible = true;
                if (!Util.IsDataSetValid(ds))
                    ds = null;

                this.gvServices.DataSource = ds;
                this.gvServices.DataBind();
                sn.History.DsVehicleDueServices = ds;
                this.gvServices.SelectedIndex = -1;
            }
            else
            {
                //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
                //string strPath = MapPath(DataSetPath) + @"\FleetMaintenance.xsd";

                strPath = Server.MapPath("../Datasets/FleetMaintenance.xsd");

                if (xml == "")
                    ds = null;
                else
                {
                    ds.ReadXmlSchema(strPath);
                    ds.ReadXml(new StringReader(xml));
                }

                dgFleetVehiclesInfo.Visible = false;
                sn.History.DsDueServices = ds;
                this.dgFleetDueServices.Visible = true;

                if (!dgFleetDueServices.RootTable.Columns[2].Caption.Contains(strUnitOfMes))
                    this.dgFleetDueServices.RootTable.Columns[2].Caption = this.dgFleetDueServices.RootTable.Columns[2].Caption + strUnitOfMes;

                dgFleetDueServices.ClearCachedDataSource();
                dgFleetDueServices.RebindDataSource();
            }
        }

        protected void btnAddNewPlan_Click(object sender, EventArgs e)
        {
            this.LabelMaintenanceMessage.Text = "";
            MaintenancePlanEditMode = false;
            ddlNewPlanSchedule.SelectedIndex = -1;
            SetPlanDetails(null);
        }

        /// <summary>
        /// Set service plan details panel
        /// </summary>
        /// <param name="rowDetails"></param>
        private void SetPlanDetails(DataRow rowDetails)
        {
            this.TableServiceDetailsSaveCancel.Visible = true;
            this.MaintenanceServices.Visible = false;
            this.MaintenancePlanDetails.Visible = true;
            this.ButtonMaintenanceInit.Enabled = false;
            this.trTimeBasedPlan.Visible = false;
            this.trNewPlanDate.Visible = false;
            this.chkUnlimited.Checked = false;

            short OperationType = 0;
            if (rowDetails == null)
            {
                this.LabelMaintenancePlanCaption.Text = base.GetLocalResourceObject("AddNewMaintenancePlan").ToString();
                this.TextMaintenancePlanDescription.Text = "";
                this.TextMaintenancePlanEmail.Text = "";
                this.TextMaintenancePlanComments.Text = "";
                if (this.ddlNewPlanType.Items.Count > 0)
                    this.ddlNewPlanType.SelectedIndex = 0;
                this.TextDueValue.Text = "0";
                this.TextEndValue.Text = "0";
                this.TextInterval.Text = "0";
            }
            else
            {
                this.LabelMaintenancePlanCaption.Text = base.GetLocalResourceObject("ServiceDetails").ToString();

                if (!Int16.TryParse(rowDetails["OperationTypeID"].ToString(), out OperationType))
                {
                    base.ShowMessage(this.LabelMaintenanceMessage, base.GetLocalResourceObject("GetOperationTypeError").ToString(), Color.Red);
                    return;
                }
                //GetOperationTypes(rowDetails["OperationTypeID"]);
                this.ddlNewPlanType.SelectedValue = OperationType.ToString();

                GetOrganizationVehicleServices(OperationType, rowDetails["ServiceTypeID"]);
                GetOrganizationNotifications(OperationType, rowDetails["NotificationID"]);

                //this.ddlNewPlanSvcType.SelectedValue = rowDetails["ServiceTypeID"].ToString();
                //this.ddlNewPlanNotification.SelectedValue = rowDetails["NotificationID"].ToString();
                this.ddlNewPlanSchedule.SelectedValue = rowDetails["FrequencyID"].ToString();
                if (OperationType == (short)OperationTypeEnum.EngineHours)
                {
                    //this.TextDueValue.Text = (Convert.ToInt32(rowDetails["DueServiceValue"]) / 60).ToString();
                    //this.TextInterval.Text = (Convert.ToInt32(rowDetails["ServiceInterval"]) / 60).ToString();
                    //this.TextEndValue.Text = (Convert.ToInt32(rowDetails["EndServiceValue"]) / 60).ToString();

                    this.TextDueValue.Text = rowDetails["DueServiceValue"].ToString();
                    this.TextInterval.Text = rowDetails["ServiceInterval"].ToString();
                    this.TextEndValue.Text = rowDetails["EndServiceValue"].ToString();
                }
                else if (OperationType == (short)OperationTypeEnum.Odometer)
                {
                    this.TextDueValue.Text = (Convert.ToInt32(rowDetails["DueServiceValue"])).ToString();
                    this.TextInterval.Text = (Convert.ToInt32(rowDetails["ServiceInterval"])).ToString();
                    this.TextEndValue.Text = (Convert.ToInt32(rowDetails["EndServiceValue"])).ToString();
                }
                else if (OperationType == (short)OperationTypeEnum.DateTime)
                {
                    this.trInterval.Visible = false;
                    this.trEndValue.Visible = false;
                    this.trDueValue.Visible = false;
                    this.trNewPlanDate.Visible = false;

                    if (rowDetails["FrequencyID"].ToString() == "0")
                    {
                        this.trNewPlanDate.Visible = true;
                        this.txtNewPlanDate.Text = new DateTime(2000, 1, 1).AddDays(Convert.ToInt32(rowDetails["DueServiceValue"])).ToShortDateString();
                    }
                    else
                    {
                        trTimeBasedPlan.Visible = true;

                        this.txtFromDate.Text = new DateTime(2000, 1, 1).AddDays(Convert.ToInt32(rowDetails["DueServiceValue"])).ToShortDateString();
                        this.txtEndDate.Text = new DateTime(2000, 1, 1).AddDays(Convert.ToInt32(rowDetails["EndServiceValue"])).ToShortDateString();
                        int Interval = 0;

                        if (Convert.ToInt32(rowDetails["ServiceInterval"]) < 1000)//Weekly
                        {
                            Interval = Convert.ToInt32(rowDetails["ServiceInterval"]) - 100;

                            this.lstSceduledType.SelectedIndex = 0;
                            cboWeekDay.SelectedIndex = cboWeekDay.Items.IndexOf(cboWeekDay.Items.FindByValue(Interval.ToString()));
                            this.tblWeekly.Visible = true;
                            this.tblMonthly.Visible = false;
                            this.tblYearly.Visible = false;
                        }
                        else if ((Convert.ToInt32(rowDetails["ServiceInterval"]) > 1000) && (Convert.ToInt32(rowDetails["ServiceInterval"]) < 2000)) //Monthly
                        {
                            Interval = Convert.ToInt32(rowDetails["ServiceInterval"]) - 1000;
                            this.lstSceduledType.SelectedIndex = 1;
                            cboMonthlyDay.SelectedIndex = cboMonthlyDay.Items.IndexOf(cboMonthlyDay.Items.FindByValue(Interval.ToString()));
                            this.tblWeekly.Visible = false;
                            this.tblMonthly.Visible = true;
                            this.tblYearly.Visible = false;
                        }
                        else if (Convert.ToInt32(rowDetails["ServiceInterval"]) > 2000)  //Yearly
                        {
                            Interval = Convert.ToInt32(rowDetails["ServiceInterval"]) - 2000;
                            this.lstSceduledType.SelectedIndex = 2;
                            cboYearly.SelectedIndex = cboYearly.Items.IndexOf(cboYearly.Items.FindByValue(Interval.ToString()));
                            this.tblWeekly.Visible = false;
                            this.tblMonthly.Visible = false;
                            this.tblYearly.Visible = true;
                        }
                    }

                    if (TextEndValue.Text == "10000000")
                    {
                        this.TextEndValue.Enabled = false;
                        this.chkUnlimited.Checked = true;
                    }
                    else
                    {
                        this.TextEndValue.Enabled = true;
                        this.chkUnlimited.Checked = false;
                    }
                }

                try
                {
                    this.TextMaintenancePlanEmail.Text = rowDetails["Email"].ToString();
                    this.TextMaintenancePlanDescription.Text = rowDetails["ServiceDescription"].ToString();
                    this.TextMaintenancePlanComments.Text = rowDetails["Comments"].ToString();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Get Organization Vehicle Services and populate drop down list
        /// </summary>
        /// <param name="operType"></param>
        private void GetOrganizationVehicleServices(short operType, object selectedValue)
        {
            string xml = "";
            if (objUtil.ErrCheck(orgProxy.VehicleService_GetByType(sn.UserID, sn.SecId, sn.User.OrganizationId, operType, ref xml), false))
                if (objUtil.ErrCheck(orgProxy.VehicleService_GetByType(sn.UserID, sn.SecId, sn.User.OrganizationId, operType, ref xml), true))
                {
                    ddlNewPlanSvcType.Items.Clear();
                    this.ddlNewPlanSvcType.DataSource = null;
                    this.ddlNewPlanSvcType.DataBind();
                    return;
                }

            if (String.IsNullOrEmpty(xml))
            {
                ddlNewPlanSvcType.Items.Clear();
                this.ddlNewPlanSvcType.DataSource = null;
                this.ddlNewPlanSvcType.DataBind();
                return;
            }

            DataSet ds = new DataSet();
            ds.ReadXml(new StringReader(xml));
            if (Util.IsDataSetValid(ds))
            {
                this.ddlNewPlanSvcType.DataSource = ds.Tables[0];
                this.ddlNewPlanSvcType.DataBind();
                if (selectedValue != null)
                    this.ddlNewPlanSvcType.SelectedValue = selectedValue.ToString();
                //else
                //   base.ShowMessage(this.LabelMaintenanceMessage, "Get Vehicle Services Error", Color.Red);
            }
            else
            {
                ddlNewPlanSvcType.Items.Clear();
                this.ddlNewPlanSvcType.DataSource = null;
                this.ddlNewPlanSvcType.DataBind();
            }

            //this.ddlNewPlanSvcType.DataSource =
            //   org.GetOrganizationVehicleServices(orgId, operType).Tables[0];
            //this.ddlNewPlanSvcType.DataBind();
        }

        /// <summary>
        /// Get Organization Notifications and populate drop down list
        /// </summary>
        /// <param name="operType"></param>
        private void GetOrganizationNotifications(short operType, object selectedValue)
        {
            string xml = "";

            if (objUtil.ErrCheck(orgProxy.Notification_GetByType(sn.UserID, sn.SecId, sn.User.OrganizationId, operType, ref xml), false))
                if (objUtil.ErrCheck(orgProxy.Notification_GetByType(sn.UserID, sn.SecId, sn.User.OrganizationId, operType, ref xml), true))
                {
                    ddlNewPlanNotification.Items.Clear();
                    this.ddlNewPlanNotification.DataSource = null;
                    this.ddlNewPlanNotification.DataBind();
                    return;
                }

            if (String.IsNullOrEmpty(xml))
            {
                ddlNewPlanNotification.Items.Clear();
                this.ddlNewPlanNotification.DataSource = null;
                this.ddlNewPlanNotification.DataBind();
                return;
            }

            DataSet ds = new DataSet();
            ds.ReadXml(new StringReader(xml));
            if (Util.IsDataSetValid(ds))
            {
                this.ddlNewPlanNotification.DataSource = ds.Tables[0];
                this.ddlNewPlanNotification.DataBind();
                if (selectedValue != null)
                    this.ddlNewPlanNotification.SelectedValue = selectedValue.ToString();
                //else
                //   base.ShowMessage(this.LabelMaintenanceMessage, "Get Notifications Error", Color.Red);
            }
            else
            {
                ddlNewPlanNotification.Items.Clear();
                this.ddlNewPlanNotification.DataSource = null;
                this.ddlNewPlanNotification.DataBind();
            }
            //this.ddlNewPlanNotification.DataSource =
            //   org.GetOrganizationNotifications(orgId, operType).Tables[0];
            //this.ddlNewPlanNotification.DataBind();
            //}
        }

        protected void btnCancelNewPlan_Click(object sender, EventArgs e)
        {
            this.LabelMaintenanceMessage.Text = "";
            this.MaintenanceServices.Visible = true;
            this.MaintenancePlanDetails.Visible = false;
            this.gvServices.SelectedIndex = -1;
            this.TableServiceDetailsSaveCancel.Visible = false;
            this.ButtonMaintenanceInit.Enabled = true;
        }

        protected void btnSaveNewPlan_Click(object sender, EventArgs e)
        {
            //string msg = "";
            //Color msgColor;

            //short OperationType = Convert.ToInt16(this.ddlNewPlanType.SelectedValue);

            //if ((OperationType == (short)OperationTypeEnum.EngineHours) || (OperationType == (short)OperationTypeEnum.Odometer))
            //{
            //  try
            //    {
            //        int DueValue = Convert.ToInt32(this.TextDueValue.Text);
            //        int Interval = Convert.ToInt32(this.TextInterval.Text);
            //        int EndValue = Convert.ToInt32(this.TextEndValue.Text);

            //        if (this.trEndValue.Visible && (EndValue < (DueValue + Interval)))
            //        {
            //            base.ShowMessage(this.LabelMaintenanceMessage, "The 'End Value' should be equal or greater than 'Due Value' plus 'Interval'", Color.Red);
            //            return;
            //        }

            //    }
            //    catch
            //    {
            //    }
            //}
            //else if (OperationType == (short)OperationTypeEnum.DateTime)
            //{
            //    try
            //    {
            //        DateTime dtStart = Convert.ToDateTime(this.txtFromDate.Text);
            //        DateTime dtEnd = Convert.ToDateTime(this.txtEndDate.Text);

            //        if (dtStart > dtEnd)
            //        {
            //            base.ShowMessage(this.LabelMaintenanceMessage, "Invalid end date", Color.Red);
            //            return;
            //        }
            //    }
            //    catch
            //    {
            //    }
            //}

            try
            {
                if (SavePlan())
                {
                    if (FleetVehicleSelectorMaintenance.SelectedVehicle != -999)
                    {
                        this.MaintenanceServices.Visible = true;
                        this.ButtonMaintenanceInit.Enabled = true;
                        BindServicePlans();
                    }

                    this.MaintenancePlanDetails.Visible = false;
                    this.TableServiceDetailsSaveCancel.Visible = false;
                }
            }
            catch (Exception ex)
            {
                base.ShowMessage(this.LabelMaintenanceMessage, base.GetLocalResourceObject("MaintenancePlanUpdateError").ToString(), Color.Red);
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //base.RedirectToLogin();
            }
        }

        /// <summary>
        /// Add or update maintenance plan
        /// </summary>
        private bool SavePlan()
        {
            short ScheduleType = Convert.ToInt16(this.ddlNewPlanSchedule.SelectedValue);
            short OperationType = Convert.ToInt16(this.ddlNewPlanType.SelectedValue);
            int DueValue = 0;
            int Interval = 0;
            int EndValue = 0;
            int ServiceType = Convert.ToInt32(this.ddlNewPlanSvcType.SelectedValue);
            int NotificationType = Convert.ToInt32(this.ddlNewPlanNotification.SelectedValue);

            // if (ddlNewPlanSchedule.SelectedIndex != 2)

            if ((OperationType == (short)OperationTypeEnum.EngineHours) || (OperationType == (short)OperationTypeEnum.Odometer))
            {

                DueValue = Convert.ToInt32(this.TextDueValue.Text);
                Interval = Convert.ToInt32(this.TextInterval.Text);
                EndValue = Convert.ToInt32(this.TextEndValue.Text);
                string[] tmp = txtOdometer.Text.Split(' ');
                int odometer = 0;
                if (clsUtility.IsNumeric(tmp[0]))
                    odometer = Convert.ToInt32(tmp[0]);

                if (FleetVehicleSelectorMaintenance.SelectedVehicle != -999)
                {
                    if (this.trEndValue.Visible && (EndValue < (DueValue + Interval)))
                    {
                        base.ShowMessage(this.LabelMaintenanceMessage, "The 'End Value' should be equal or greater than 'Due Value' plus 'Interval'", Color.Red);
                        return false;
                    }

                    if (OperationType == (short)OperationTypeEnum.EngineHours)
                    {
                        if (Convert.ToSingle(txtEngineHours.Text) >= DueValue)
                        {
                            base.ShowMessage(this.LabelMaintenanceMessage, base.GetLocalResourceObject("InvalidDueValue").ToString(), Color.Red);
                            return false;
                        }
                        DueValue *= 60;
                        Interval *= 60;
                        EndValue *= 60;
                    }
                    else if ((OperationType == (short)OperationTypeEnum.Odometer) && (Convert.ToInt32(odometer) >= DueValue))
                    {
                        base.ShowMessage(this.LabelMaintenanceMessage, base.GetLocalResourceObject("InvalidDueValue").ToString(), Color.Red);
                        return false;
                    }

                    if (ScheduleType == (short)ServiceFrequency.Once)
                    {
                        Interval = 0;
                        EndValue = 0;
                    }
                    else
                    {
                        if (Interval == 0)
                        {
                            base.ShowMessage(this.LabelMaintenanceMessage, base.GetLocalResourceObject("InvalidInterval").ToString(), Color.Red);
                            return false;
                        }
                    }
                }
                else
                {
                    DueValue = 0;

                    if (ScheduleType == (short)ServiceFrequency.Once)
                    {
                        Interval = 0;
                        EndValue = 0;
                    }
                    else
                    {
                        if (Interval == 0)
                        {
                            base.ShowMessage(this.LabelMaintenanceMessage, base.GetLocalResourceObject("InvalidInterval").ToString(), Color.Red);
                            return false;
                        }
                    }
                }
            }
            else if (OperationType == (short)OperationTypeEnum.DateTime)
            {
                DateTime dtAdj = new DateTime(2000, 1, 1);
                TimeSpan ts = new TimeSpan();

                if (ScheduleType == (short)ServiceFrequency.Once)
                {
                    DateTime dtStart = Convert.ToDateTime(this.txtNewPlanDate.Text);

                    if (dtStart < System.DateTime.Now)
                    {
                        base.ShowMessage(this.LabelMaintenanceMessage, "Start date should be greater than current", Color.Red);
                        return false;
                    }

                    ts = dtStart - dtAdj;
                    DueValue = Convert.ToInt32(ts.TotalDays);
                    Interval = 0;
                    EndValue = 0;
                }
                else
                {
                    DateTime dtStart = Convert.ToDateTime(this.txtFromDate.Text);
                    DateTime dtEnd = Convert.ToDateTime(this.txtEndDate.Text);
                    DateTime dtNew = new DateTime(); ;

                    if (dtStart < System.DateTime.Now)
                    {
                        base.ShowMessage(this.LabelMaintenanceMessage, "Start date should be greater than current", Color.Red);
                        return false;
                    }


                    if (dtStart > dtEnd)
                    {
                        base.ShowMessage(this.LabelMaintenanceMessage, "Invalid end date", Color.Red);
                        return false;
                    }

                    switch (lstSceduledType.SelectedItem.Value)
                    {
                        case "2": // weekly
                            AdjDateWeek(ref dtStart);
                            ts = dtStart - dtAdj;
                            DueValue = Convert.ToInt32(ts.TotalDays);
                            ts = dtEnd - dtAdj;
                            EndValue = Convert.ToInt32(ts.TotalDays);
                            Interval = Convert.ToInt16(cboWeekDay.SelectedItem.Value) + 100;   //100 for Weekly
                            break;
                        case "3": // monthly
                            if (cboMonthlyDay.SelectedItem.Value != "32")
                                dtNew = new DateTime(dtStart.Year, dtStart.Month, Convert.ToInt32(this.cboMonthlyDay.SelectedItem.Value));
                            else
                                dtNew = LastDayOfMonthFromDateTime(dtStart); ;

                            ts = dtNew - dtAdj;
                            DueValue = Convert.ToInt32(ts.TotalDays);
                            ts = dtEnd - dtAdj;
                            EndValue = Convert.ToInt32(ts.TotalDays);
                            Interval = Convert.ToInt16(cboMonthlyDay.SelectedItem.Value) + 1000;   //1000 for Monthly
                            break;
                        case "4": // yearly
                            dtNew = new DateTime(dtStart.Year, Convert.ToInt32(this.cboYearly.SelectedItem.Value), 1);
                            if (dtNew < System.DateTime.Now)
                                dtNew = dtNew.AddYears(1);

                            ts = dtNew - dtAdj;
                            DueValue = Convert.ToInt32(ts.TotalDays);
                            ts = dtEnd - dtAdj;
                            EndValue = Convert.ToInt32(ts.TotalDays);
                            Interval = Convert.ToInt16(cboYearly.SelectedItem.Value) + 2000;   //2000 for Yearly
                            break;
                    }
                }
            }

            if (MaintenancePlanEditMode)
            {
                if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlan_Update(sn.UserID, sn.SecId, sn.User.OrganizationId,
                   SelectedServiceID, this.FleetVehicleSelectorMaintenance.SelectedVehicle,
                   ServiceType, OperationType, NotificationType, ScheduleType,
                   DueValue, Interval, EndValue,
                   this.TextMaintenancePlanEmail.Text, this.TextMaintenancePlanDescription.Text, this.TextMaintenancePlanComments.Text), false))
                    if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlan_Update(sn.UserID, sn.SecId, sn.User.OrganizationId,
                       SelectedServiceID, this.FleetVehicleSelectorMaintenance.SelectedVehicle,
                       ServiceType, OperationType, NotificationType, ScheduleType,
                       DueValue, Interval, EndValue,
                       this.TextMaintenancePlanEmail.Text, this.TextMaintenancePlanDescription.Text, this.TextMaintenancePlanComments.Text), true))
                    {
                        base.ShowMessage(this.LabelMaintenanceMessage,
                           base.GetLocalResourceObject("MaintenancePlanUpdateError").ToString(), Color.Red);
                        return false;
                    }
                base.ShowMessage(this.LabelMaintenanceMessage, base.GetLocalResourceObject("MaintenancePlanUpdated").ToString(), Color.Green);

                return true;
            }
            else
            {
                if (FleetVehicleSelectorMaintenance.SelectedVehicle != -999)
                {
                    if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlan_Add(sn.UserID, sn.SecId, sn.User.OrganizationId,
                       this.FleetVehicleSelectorMaintenance.SelectedVehicle,
                       ServiceType, OperationType, NotificationType, ScheduleType,
                       DueValue, Interval, EndValue,
                       this.TextMaintenancePlanEmail.Text, this.TextMaintenancePlanDescription.Text, this.TextMaintenancePlanComments.Text), false))
                        if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlan_Add(sn.UserID, sn.SecId, sn.User.OrganizationId,
                           this.FleetVehicleSelectorMaintenance.SelectedVehicle,
                           ServiceType, OperationType, NotificationType, ScheduleType,
                           DueValue, Interval, EndValue,
                           this.TextMaintenancePlanEmail.Text, this.TextMaintenancePlanDescription.Text, this.TextMaintenancePlanComments.Text), true))
                        {
                            base.ShowMessage(this.LabelMaintenanceMessage,
                               base.GetLocalResourceObject("MaintenancePlanAddError").ToString(), Color.Red);
                            return false;
                        }

                    base.ShowMessage(this.LabelMaintenanceMessage, base.GetLocalResourceObject("MaintenancePlanCreated").ToString(), Color.Green);
                }
                else
                {
                    if (objUtil.ErrCheck(fleetProxy.VehicleMaintenancePlan_AddToFleet(sn.UserID, sn.SecId, sn.User.OrganizationId,
                       this.FleetVehicleSelectorMaintenance.SelectedFleet,
                       ServiceType, OperationType, NotificationType, ScheduleType,
                       DueValue, Interval, EndValue,
                       this.TextMaintenancePlanEmail.Text, this.TextMaintenancePlanDescription.Text, this.TextMaintenancePlanComments.Text), false))
                        if (objUtil.ErrCheck(fleetProxy.VehicleMaintenancePlan_AddToFleet(sn.UserID, sn.SecId, sn.User.OrganizationId,
                           this.FleetVehicleSelectorMaintenance.SelectedFleet,
                           ServiceType, OperationType, NotificationType, ScheduleType,
                           DueValue, Interval, EndValue,
                           this.TextMaintenancePlanEmail.Text, this.TextMaintenancePlanDescription.Text, this.TextMaintenancePlanComments.Text), true))
                        {
                            base.ShowMessage(this.LabelMaintenanceMessage,
                               base.GetLocalResourceObject("MaintenancePlanAddError").ToString(), Color.Red);
                            return false;
                        }
                }
                return true;
            }
        }

        /// <summary>
        /// Change vehicle fleetvehicleselector control event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SelectVehicle(object sender, EventArgs e)
        {
            DisplayMaintenanceDataPerSelection();
        }

        private void DisplayMaintenanceDataPerSelection()
        {
            this.LabelMaintenanceMessage.Text = "";

            if (this.FleetVehicleSelectorMaintenance.SelectedVehicle == -1)
            {
                this.txtEngineHours.Text = "";
                this.txtOdometer.Text = "";
                this.ServicesButtons.Visible = false;
                this.ButtonMaintenanceInit.Visible = false;
                return;
            }
            this.dgFleetDueServices.Visible = false;
            this.dgFleetVehiclesInfo.Visible = false;
            this.MaintenanceServices.Visible = false;
            this.MaintenancePlanDetails.Visible = false;
            this.TableServiceDetailsSaveCancel.Visible = false;

            DataSet dsMaintenance = new DataSet();
            string xml = "";

            if (FleetVehicleSelectorMaintenance.SelectedVehicle != -999)
            {
                if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenance_GetInfo(sn.UserID, sn.SecId,
                   this.FleetVehicleSelectorMaintenance.SelectedVehicle, true, ref xml), false))
                    if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenance_GetInfo(sn.UserID, sn.SecId,
                       this.FleetVehicleSelectorMaintenance.SelectedVehicle, true, ref xml), true))
                    {
                        return;
                    }

                this.ServicesButtons.Visible = true;
                this.btnAddNewPlan.Visible = true;

                if (!String.IsNullOrEmpty(xml))
                    dsMaintenance.ReadXml(new StringReader(xml));

                this.txtEngineHours.Text = "";
                this.txtOdometer.Text = "";
                dgFleetVehiclesInfo.Visible = false;
                this.TableMaintenanceVehicle.Visible = true;
                string strUnitOfMes = sn.User.UnitOfMes == 1 ? " (Km)" : " (Mi)";

                if (Util.IsDataSetValid(dsMaintenance))
                {
                    //int currentEngineHours;
                    this.txtEngineHours.Text = dsMaintenance.Tables[0].Rows[0]["CurrentEngHrs"].ToString();
                    //((int)Math.Floor(Convert.ToDouble(dsMaintenance.Tables[0].Rows[0]["CurrentEngHrs"]) / 60)).ToString();
                    this.txtOdometer.Text = dsMaintenance.Tables[0].Rows[0]["CurrentOdo"].ToString() + " " + strUnitOfMes;
                    this.ButtonMaintenanceInit.Visible = true;

                    if (this.txtOdometer.Text == "0" && this.txtEngineHours.Text == "0")
                    {
                        this.ServicesButtons.Visible = false;
                    }
                    else
                    {
                        this.ServicesButtons.Visible = true;
                    }
                }
                else
                {
                    this.txtEngineHours.Text = "";
                    this.txtOdometer.Text = "";
                    this.ServicesButtons.Visible = false;
                    this.ButtonMaintenanceInit.Visible = false;
                }
            }
            else
            {
                if (objUtil.ErrCheck(fleetProxy.GetFleetMaintenanceInfoXML(sn.UserID, sn.SecId,
                this.FleetVehicleSelectorMaintenance.SelectedFleet, ref xml), false))
                    if (objUtil.ErrCheck(fleetProxy.GetFleetMaintenanceInfoXML(sn.UserID, sn.SecId,
                       this.FleetVehicleSelectorMaintenance.SelectedFleet, ref xml), true))
                    {
                        return;
                    }

                if (!String.IsNullOrEmpty(xml))
                    dsMaintenance.ReadXml(new StringReader(xml));

                if (!dgFleetVehiclesInfo.RootTable.Columns[1].Caption.Contains(strUnitOfMes))
                    this.dgFleetVehiclesInfo.RootTable.Columns[1].Caption = this.dgFleetVehiclesInfo.RootTable.Columns[1].Caption + strUnitOfMes;

                sn.History.DsMaintenance = dsMaintenance;
                dgFleetVehiclesInfo.ClearCachedDataSource();
                dgFleetVehiclesInfo.RebindDataSource();
                dgFleetVehiclesInfo.Visible = true;
                this.TableMaintenanceVehicle.Visible = false;
                this.ServicesButtons.Visible = true;
                //this.btnAddNewPlan.Visible = false;   
            }
        }

        protected void gvServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.LabelMaintenanceMessage.Text = "";

                // control check - should never happen!
                long vid = Convert.ToInt64(gvServices.SelectedDataKey.Values[0]);
                if (vid != this.FleetVehicleSelectorMaintenance.SelectedVehicle)
                {
                    base.ShowMessage(this.LabelMaintenanceMessage, base.GetLocalResourceObject("DataBaseSecurityError_InvalidVehicle").ToString(), Color.Red);
                    return;
                }

                SelectedServiceID = Convert.ToInt32(gvServices.SelectedDataKey.Values[1]);

                DataSet ds = new DataSet("Service");
                string xml = "";
                //using (Vehicle car = new Vehicle(connectionString))
                //{
                //ds = car.GetVehicleService(
                //   Convert.ToInt64(gvServices.SelectedDataKey.Values[0]),
                //   Convert.ToInt16(gvServices.SelectedDataKey.Values[1]));

                if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlan_Get(sn.UserID, sn.SecId, sn.User.OrganizationId,
                   vid, SelectedServiceID, ref xml), false))
                    if (objUtil.ErrCheck(vehicleProxy.VehicleMaintenancePlan_Get(sn.UserID, sn.SecId, sn.User.OrganizationId,
                       vid, SelectedServiceID, ref xml), true))
                    {
                        return;
                    }

                if (!String.IsNullOrEmpty(xml))
                    ds.ReadXml(new StringReader(xml));

                if (!Util.IsDataSetValid(ds))
                    return;

                MaintenancePlanEditMode = true;
                SetPlanDetails(ds.Tables[0].Rows[0]);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                base.ShowMessage(this.LabelMaintenanceMessage, base.GetLocalResourceObject("DataBaseSecurityError").ToString(), Color.Red);
            }
        }

        protected void ddlNewPlanSchedule_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtFromDate.Text = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
            this.txtEndDate.Text = DateTime.Now.AddYears(2).ToString("MM/dd/yyyy");
            this.txtNewPlanDate.Text = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
            this.trTimeBasedPlan.Visible = false;
            switch (ddlNewPlanSchedule.SelectedValue)
            {
                case "0": // once
                    if (this.ddlNewPlanType.SelectedValue == "1" || this.ddlNewPlanType.SelectedValue == "2")
                    {
                        this.trDueValue.Visible = true;
                        this.trNewPlanDate.Visible = false;
                    }
                    else
                    {
                        this.trDueValue.Visible = false;
                        this.trNewPlanDate.Visible = true;
                    }
                    this.trEndValue.Visible = false;
                    this.trInterval.Visible = false;

                    break;
                case "1": // recurring
                    if (this.ddlNewPlanType.SelectedValue == "1" || this.ddlNewPlanType.SelectedValue == "2")
                    {
                        if (FleetVehicleSelectorMaintenance.SelectedVehicle != -999)
                            this.trDueValue.Visible = true;

                        this.trNewPlanDate.Visible = false;
                        this.RequiredDueValidator.Enabled = false;
                        this.RequiredIntervalValidator.Enabled = false;
                        this.trEndValue.Visible = true;
                        this.trInterval.Visible = true;
                    }
                    else
                    {
                        this.trDueValue.Visible = false;
                        this.trNewPlanDate.Visible = false;
                        this.trTimeBasedPlan.Visible = true;
                        this.RequiredDueValidator.Enabled = false;
                        this.RequiredIntervalValidator.Enabled = false;
                    }

                    break;
                default: // pls. select...
                    this.trDueValue.Visible = false;
                    this.trNewPlanDate.Visible = false;
                    this.trInterval.Visible = false;
                    this.trEndValue.Visible = false;
                    break;
            }
        }

        protected void ddlNewPlanType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlNewPlanSchedule.Items.Clear();

            if (FleetVehicleSelectorMaintenance.SelectedVehicle != -999)
            {
                ddlNewPlanSchedule.Items.Add(new ListItem("Please select schedule type", "-1"));
                ddlNewPlanSchedule.Items.Add(new ListItem("Once", "0"));
                ddlNewPlanSchedule.Items.Add(new ListItem("Recurring", "1"));
            }
            else
            {
                ddlNewPlanSchedule.Items.Add(new ListItem("Please select schedule type", "-1"));
                ddlNewPlanSchedule.Items.Add(new ListItem("Recurring", "1"));
            }

            short selectedType = 0, selectedSchedule = 0;
            ddlNewPlanSchedule.SelectedIndex = -1;
            this.trDueValue.Visible = false;
            this.trNewPlanDate.Visible = false;
            this.trInterval.Visible = false;
            this.trEndValue.Visible = false;
            this.trTimeBasedPlan.Visible = false;

            this.trNewPlanSchedule.Visible = false;
            if (!short.TryParse(ddlNewPlanType.SelectedValue, out selectedType))
                return;
            if (!short.TryParse(ddlNewPlanSchedule.SelectedValue, out selectedSchedule))
                return;
            switch (selectedType)
            {
                case (int)OperationTypeEnum.Odometer:
                case (int)OperationTypeEnum.EngineHours:
                    this.trNewPlanService.Visible = true;
                    this.trNewPlanNotification.Visible = true;
                    this.trNewPlanSchedule.Visible = true;

                    switch (selectedSchedule)
                    {
                        case (short)ServiceFrequency.Once:
                            if (FleetVehicleSelectorMaintenance.SelectedVehicle != -999)
                                this.trDueValue.Visible = true;

                            this.trNewPlanDate.Visible = false;
                            this.trInterval.Visible = false;
                            this.trEndValue.Visible = true;
                            break;
                        case (short)ServiceFrequency.Recurring:
                            if (FleetVehicleSelectorMaintenance.SelectedVehicle != -999)
                                this.trDueValue.Visible = true;
                            this.trNewPlanDate.Visible = false;
                            this.trInterval.Visible = true;
                            this.trEndValue.Visible = true;
                            break;
                    }
                    break;
                case (int)OperationTypeEnum.DateTime:
                    this.trNewPlanService.Visible = true;
                    this.trNewPlanNotification.Visible = true;
                    this.trNewPlanSchedule.Visible = true;
                    switch (selectedSchedule)
                    {
                        case (short)ServiceFrequency.Once:
                            this.trDueValue.Visible = false;
                            this.trNewPlanDate.Visible = true;
                            this.trInterval.Visible = false;
                            this.trEndValue.Visible = false;
                            break;
                        case (short)ServiceFrequency.Recurring:
                            this.trDueValue.Visible = false;
                            this.trNewPlanDate.Visible = false;
                            this.trInterval.Visible = false;
                            this.trInterval.Visible = true;
                            this.trEndValue.Visible = false;
                            break;
                    }
                    break;
                default:
                    this.trNewPlanService.Visible = false;
                    this.trNewPlanNotification.Visible = false;
                    this.trNewPlanSchedule.Visible = false;
                    this.trDueValue.Visible = false;
                    this.trNewPlanDate.Visible = false;
                    this.trInterval.Visible = false;
                    this.trEndValue.Visible = false;
                    break;
            }

            GetOrganizationVehicleServices(selectedType, null);
            GetOrganizationNotifications(selectedType, null);
        }
        # endregion Maintenance tab

        # region Services tab

        protected void gvOrgServices_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvOrgServices.PageIndex = e.NewPageIndex;
                BindOrganizationServices();
                gvOrgServices.SelectedIndex = -1;
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //RedirectToLogin();
            }
        }

        protected void btnAddService_Click(object sender, EventArgs e)
        {
            this.AddNewService.Visible = true;
            this.linkAddService.Enabled = false;
            this.lblServicesMessage.Text = "";
        }

        protected void btnSaveService_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblServicesMessage.Text = "";
                if (this.ddlOrgServiceType.SelectedValue == "0")
                {
                    base.ShowMessage(this.lblServicesMessage, "Invalid Service Type", Color.Red);
                    return;
                }
                //if (!ValidateData()) return;
                //using (Organization org = new Organization(connectionString))
                //{
                //orgProxy.AddVehicleService(sn.User.OrganizationId,
                //   Convert.ToInt16(this.ddlOrgServiceType.SelectedValue),
                //   this.txtServiceTypeDescription.Text,
                //   this.txtVRMSCode.Text);

                if (objUtil.ErrCheck(orgProxy.VehicleService_Add(sn.UserID, sn.SecId, sn.User.OrganizationId,
                   Convert.ToInt16(this.ddlOrgServiceType.SelectedValue), this.txtServiceTypeDescription.Text, this.txtVRMSCode.Text), false))
                    if (objUtil.ErrCheck(orgProxy.VehicleService_Add(sn.UserID, sn.SecId, sn.User.OrganizationId,
                       Convert.ToInt16(this.ddlOrgServiceType.SelectedValue), this.txtServiceTypeDescription.Text, this.txtVRMSCode.Text), true))
                    {
                        base.ShowMessage(this.lblServicesMessage, base.GetLocalResourceObject("SaveServiceError").ToString(), Color.Red);
                        return;
                    }

                this.AddNewService.Visible = false;
                this.linkAddService.Enabled = true;
                base.ShowMessage(this.lblServicesMessage, base.GetLocalResourceObject("NewServiceAdded").ToString(), Color.Green);
                BindOrganizationServices();
                //}
            }
            catch (Exception ex)
            {
                base.ShowMessage(this.lblServicesMessage, base.GetLocalResourceObject("SaveServiceError").ToString(), Color.Red);
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //base.RedirectToLogin();
            }
        }

        protected void btnDeleteOrgService_Clicked(object sender, EventArgs e)
        {
            try
            {
                this.lblServicesMessage.Text = "";
                GridViewRow row = (((sender as ImageButton).Parent as DataControlFieldCell).Parent as GridViewRow);
                int delIndex = row.RowIndex;
                //using (Organization org = new Organization(connectionString))
                //{
                if (objUtil.ErrCheck(orgProxy.VehicleService_Delete(sn.UserID, sn.SecId, sn.User.OrganizationId,
                   Convert.ToInt16(row.Cells[0].Text)), false))
                    if (objUtil.ErrCheck(orgProxy.VehicleService_Delete(sn.UserID, sn.SecId, sn.User.OrganizationId,
                       Convert.ToInt16(row.Cells[0].Text)), true))
                    {
                        base.ShowMessage(this.lblServicesMessage, base.GetLocalResourceObject("ServiceAssigned").ToString(), Color.Red);
                        return;
                    }
                //orgProxy.DeleteVehicleService(sn.User.OrganizationId, Convert.ToInt16(row.Cells[0].Text));
                //gvOrgServices.DeleteRow(delIndex);
                BindOrganizationServices();
                base.ShowMessage(this.lblServicesMessage, base.GetLocalResourceObject("ServiceDeleted").ToString(), Color.Green);
                //}
            }
            catch (Exception ex)
            {
                base.ShowMessage(this.lblServicesMessage, base.GetLocalResourceObject("DeleteServiceError").ToString(), Color.Red);
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //base.RedirectToLogin();
            }
        }

        private void BindOrganizationServices()
        {
            string xml = "";
            if (objUtil.ErrCheck(orgProxy.VehicleService_GetAllByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                if (objUtil.ErrCheck(orgProxy.VehicleService_GetAllByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                {
                    return;
                }
            if (String.IsNullOrEmpty(xml))
                return;
            //using (ServerDBSystem.DBSystem  dbs = new ServerDBSystem.DBSystem())
            //{
            //    if (objUtil.ErrCheck(dbs.LocalizationData(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, "OperationTypeId", "OperationTypeDescription", "ServiceOperationType", ref xml), false))
            //        if (objUtil.ErrCheck(dbs.LocalizationData(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, "OperationTypeId", "OperationTypeDescription", "ServiceOperationType", ref xml), true ))
            //          {
            //          }
            //}

            DataSet ds = new DataSet();
            ds.ReadXml(new StringReader(xml));

            if (Util.IsDataSetValid(ds))
            {
                gvOrgServices.DataSource = ds.Tables[0];
                gvOrgServices.DataBind();
            }
        }

        protected void btnCancelService_Click(object sender, EventArgs e)
        {
            this.AddNewService.Visible = false;
            this.linkAddService.Enabled = true;
            this.lblServicesMessage.Text = "";
        }

        protected void gvOrgServices_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //   string confirm = "return confirm('Are you sure you want to delete service?')";
            //   ImageButton cmdBtn = (ImageButton)e.Row.Cells[5].Controls[2];
            //   if (cmdBtn.CommandName == "Delete")
            //   {
            //      cmdBtn.ID = "btnDeleteService";
            //      cmdBtn.Attributes.Add("onclick", confirm);
            //   }
            //}
        }

        protected void gvOrgServices_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                this.lblServicesMessage.Text = "";
                GridViewRow currentRow = gvOrgServices.Rows[gvOrgServices.EditIndex];
                short svcId = Convert.ToInt16(gvOrgServices.DataKeys[e.RowIndex].Value);
                short svcTypeId = Convert.ToInt16(((DropDownList)(currentRow.Cells[2].Controls[1])).SelectedValue);

                if (svcTypeId == 0)
                {
                    base.ShowMessage(this.lblServicesMessage, "Invalid Service Type", Color.Red);
                    return;
                }
                string descr = (currentRow.Cells[3].Controls[0] as TextBox).Text;
                string code = (currentRow.Cells[4].Controls[0] as TextBox).Text;
                //using (Organization org = new Organization(connectionString))
                //{
                //orgProxy.UpdateVehicleService(sn.User.OrganizationId, svcId, svcTypeId, descr, code);
                if (objUtil.ErrCheck(orgProxy.VehicleService_Update(sn.UserID, sn.SecId, sn.User.OrganizationId,
                   svcId, svcTypeId, descr, code), false))
                    if (objUtil.ErrCheck(orgProxy.VehicleService_Update(sn.UserID, sn.SecId, sn.User.OrganizationId,
                       svcId, svcTypeId, descr, code), true))
                    {
                        base.ShowMessage(this.lblServicesMessage, base.GetLocalResourceObject("ServiceUpdateError").ToString(), Color.Red);
                        return;
                    }
                gvOrgServices.EditIndex = -1;
                //gdvNotifications.DataBind();
                BindOrganizationServices();
                this.linkAddService.Enabled = true;
                base.ShowMessage(this.lblServicesMessage, base.GetLocalResourceObject("ServiceUpdated").ToString(), Color.Green);
                //}
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //RedirectToLogin();
                base.ShowMessage(this.lblServicesMessage, base.GetLocalResourceObject("ServiceUpdateError").ToString(), Color.Red);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //RedirectToLogin();
                base.ShowMessage(this.lblServicesMessage, base.GetLocalResourceObject("ServiceUpdateError").ToString(), Color.Red);
            }
        }

        protected void gvOrgServices_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // todo: Check security 
                this.lblServicesMessage.Text = "";
                this.linkAddService.Enabled = false;
                gvOrgServices.EditIndex = e.NewEditIndex;
                //using (Organization org = new Organization(connectionString))
                //{
                BindOrganizationServices();
                //}
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void gvOrgServices_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                this.lblServicesMessage.Text = "";
                e.Cancel = true;
                gvOrgServices.EditIndex = -1;
                //using (Organization org = new Organization(connectionString))
                //{
                BindOrganizationServices();
                //}
                this.linkAddService.Enabled = true;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        # endregion Services tab

        public enum OperationTypeEnum
        {
            None = 0,
            Odometer,
            EngineHours,
            DateTime
        }

        public enum ServiceFrequency
        {
            Once = 0,
            Recurring
        }

        /*
        protected void gdvNotifications_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {

        }
        protected void gvOrgServices_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {

        }
        protected void gvHistory_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
        }
        protected void gvServices_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
        }
        protected void gvServices_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
           using (Vehicle car = new Vehicle(connectionString))
           {
              car.DeleteVehicleService(
                 Convert.ToInt64((sender as GridView).DataKeys[e.RowIndex].Values[0]),
                 Convert.ToInt32((sender as GridView).DataKeys[e.RowIndex].Values[1])
                 );
              BindServices();
           }
        }
        protected void gvServices_RowCommand(object sender, GridViewCommandEventArgs e)
        {
           if (e.CommandName == "Delete")
           {
              //GridViewRow row = sender as (e.CommandSource as ImageButton).Parent as GridViewRow;
              //using (Vehicle car = new Vehicle(connectionString))
              //{
              //   car.DeleteVehicleService(Convert.ToInt64(e.CommandSource.Values[0]), Convert.ToInt32(e.Values[1]));
              //   BindServices();
              //}
           }
        }
        */

        protected void dgFleetVehiclesInfo_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            if ((sn.History.DsMaintenance != null) && (sn.History.DsMaintenance.Tables[0] != null) && MultiView1.ActiveViewIndex == 0)
            {
                e.DataSource = sn.History.DsMaintenance;
            }
            else
            {
                e.DataSource = null;
            }
        }

        protected void btnAll_Click(object sender, EventArgs e)
        {
            try
            {
                this.LabelMaintenanceMessage.Text = "";
                ServiceMode = 0;
                BindServicePlans();
            }
            catch (Exception ex)
            {
                base.ShowMessage(this.LabelMaintenanceMessage, "Get Maintenance Plan Error<br />", Color.Red);
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //base.RedirectToLogin();
            }
        }

        protected void chkUnlimited_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUnlimited.Checked)
            {
                this.TextEndValue.Text = "10000000";
                TextEndValue.Enabled = false;
            }
            else
            {
                this.TextEndValue.Text = "0";
                TextEndValue.Enabled = true;
            }
        }

        protected void gvServices_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HyperLink hl = (HyperLink)e.Row.FindControl("linkClose");
                hl.NavigateUrl = "javascript:var w=VehicleMaintenaceClose(" + gvServices.DataKeys[e.Row.RowIndex].Values[0] + "," + gvServices.DataKeys[e.Row.RowIndex].Values[1] + ")";
            }
        }

        protected void dgFleetDueServices_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            if ((sn.History.DsDueServices != null) && (sn.History.DsDueServices.Tables[0] != null) && (sn.History.DsDueServices.Tables[0].TableName == "Service") && MultiView1.ActiveViewIndex == 0)
            {
                e.DataSource = sn.History.DsDueServices;
            }
            else
            {
                e.DataSource = null;
            }
        }

        protected void lstSceduledType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (lstSceduledType.SelectedItem.Value)
                {
                    case "2": // weekly
                        this.tblWeekly.Visible = true;
                        this.tblMonthly.Visible = false;
                        this.tblYearly.Visible = false;
                        break;
                    case "3": // monthly
                        this.tblWeekly.Visible = false;
                        this.tblMonthly.Visible = true;
                        this.tblYearly.Visible = false;
                        break;
                    case "4": // yearly
                        this.tblWeekly.Visible = false;
                        this.tblMonthly.Visible = false;
                        this.tblYearly.Visible = true;
                        break;
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                   Ex.Message + " User:" + sn.UserID.ToString() + " Form:frmReportScheduler.aspx->lstReportType_SelectedIndexChanged"));
                RedirectToLogin();
            }
        }

        private DateTime AdjDateWeek(ref DateTime dtStart)
        {
            int dayDiff = 0;
            dayDiff = Convert.ToInt16(dtStart.DayOfWeek) - Convert.ToInt16(cboWeekDay.SelectedItem.Value);
            if (dayDiff > 0)
            {
                dtStart = dtStart.AddDays(7 - dayDiff);
            }
            else if (dayDiff < 0)
            {
                dtStart = dtStart.AddDays(-dayDiff);
            }
            return dtStart;
        }

        public DateTime LastDayOfMonthFromDateTime(DateTime dateTime)
        {
            DateTime firstDayOfTheMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }

        protected void dgHistory_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            if ((sn.History.DsMaintenanceHistory != null) && (sn.History.DsMaintenanceHistory.Tables[0] != null) && this.MultiView1.ActiveViewIndex == 3)
            {
                e.DataSource = sn.History.DsMaintenanceHistory;
            }
            else
            {
                e.DataSource = null;
            }
        }

        protected void btnNotifications_Click(object sender, EventArgs e)
        {
            this.MultiView1.ActiveViewIndex = 4;
            ClearDataCached();
            ClearSelectedTab();
            this.btnNotifications.CssClass = "selectedbutton";
            Notification_Fill_NewTZ();
        }

        // Changes for TimeZone Feature start
        private void Notification_Fill_NewTZ()
        {
            try
            {
                string strFromDT = "";
                string strToDT = "";

                int hours = 120;
                strFromDT = DateTime.Now.AddHours(-hours - sn.User.NewFloatTimeZone - sn.User.DayLightSaving).AddMinutes(-5).ToString("MM/dd/yyyy HH:mm:ss");
                strToDT = DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");

                strToDT = DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");
                ServerDBUser.DBUser dbUser = new ServerDBUser.DBUser();
                string xml = "";
                if (objUtil.ErrCheck(dbUser.GetAllNotificationsForUserId_NewTZ(sn.UserID, sn.SecId, strFromDT, strToDT, ref xml), false))
                    if (objUtil.ErrCheck(dbUser.GetAllNotificationsForUserId_NewTZ(sn.UserID, sn.SecId, strFromDT, strToDT, ref xml), true))
                    {
                        sn.History.DsNotifications = null;
                        return;
                    }

                if (xml == "")
                    return;

                StringReader strrXML = new StringReader(xml);
                DataSet ds = new DataSet();
                string strPath = MapPath("..\\Maintenance\\Datasets") + @"\dstNotifications.xsd";
                ds.ReadXmlSchema(strPath);
                ds.ReadXml(strrXML);
                sn.History.DsNotifications = ds;

                DataColumn dc = new DataColumn();
                dc.ColumnName = "CustomUrl";
                dc.DataType = Type.GetType("System.String");
                dc.DefaultValue = "";
                sn.History.DsNotifications.Tables[0].Columns.Add(dc);

                //// Show Combobox
                DataColumn chkBoxShow = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
                dc.DefaultValue = false;
                ds.Tables[0].Columns.Add(chkBoxShow);

                foreach (DataRow dr in sn.History.DsNotifications.Tables[0].Rows)
                {
                    dr["CustomUrl"] = "javascript:var w =InfoWindow('" + dr["NotificationId"].ToString() + "')";
                }

                dgNotification.ClearCachedDataSource();
                dgNotification.RebindDataSource();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
        // Changes for TimeZone Feature end

        private void Notification_Fill()
        {
            try
            {
                string strFromDT = "";
                string strToDT = "";

                int hours = 120;
                strFromDT = DateTime.Now.AddHours(-hours - sn.User.TimeZone - sn.User.DayLightSaving).AddMinutes(-5).ToString("MM/dd/yyyy HH:mm:ss");
                strToDT = DateTime.Now.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");

                strToDT = DateTime.Now.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");
                ServerDBUser.DBUser dbUser = new ServerDBUser.DBUser();
                string xml = "";
                if (objUtil.ErrCheck(dbUser.GetAllNotificationsForUserId(sn.UserID, sn.SecId, strFromDT, strToDT, ref xml), false))
                    if (objUtil.ErrCheck(dbUser.GetAllNotificationsForUserId(sn.UserID, sn.SecId, strFromDT, strToDT, ref xml), true))
                    {
                        sn.History.DsNotifications = null;
                        return;
                    }

                if (xml == "")
                    return;

                StringReader strrXML = new StringReader(xml);
                DataSet ds = new DataSet();
                string strPath = MapPath("..\\Maintenance\\Datasets") + @"\dstNotifications.xsd";
                ds.ReadXmlSchema(strPath);
                ds.ReadXml(strrXML);
                sn.History.DsNotifications = ds;

                DataColumn dc = new DataColumn();
                dc.ColumnName = "CustomUrl";
                dc.DataType = Type.GetType("System.String");
                dc.DefaultValue = "";
                sn.History.DsNotifications.Tables[0].Columns.Add(dc);

                //// Show Combobox
                DataColumn chkBoxShow = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
                dc.DefaultValue = false;
                ds.Tables[0].Columns.Add(chkBoxShow);

                foreach (DataRow dr in sn.History.DsNotifications.Tables[0].Rows)
                {
                    dr["CustomUrl"] = "javascript:var w =InfoWindow('" + dr["NotificationId"].ToString() + "')";
                }

                dgNotification.ClearCachedDataSource();
                dgNotification.RebindDataSource();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void dgNotification_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            if (sn.History.DsNotifications != null && this.MultiView1.ActiveViewIndex == 4)
                e.DataSource = sn.History.DsNotifications;
            else
                e.DataSource = null;
        }

        protected void cmdCloseNotification_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ServerDBSystem.DBSystem dbSystem = new ServerDBSystem.DBSystem();
            this.lblMessageNotifications.Text = "";
            // Changes foe TimeZone Feature start
            DateTime dtTime = DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving);  // Changes foe TimeZone Feature end
            ArrayList rowsCount = dgNotification.RootTable.GetCheckedRows();
            if (rowsCount.Count == 0)
            {
                this.lblMessageNotifications.Text = "Please select a Notification";
                return;
            }

            //foreach (string keyValue in dgNotification.RootTable.GetCheckedRows())
            //{
            //    if (objUtil.ErrCheck(dbSystem.AcknowledgeNotification(sn.UserID, sn.SecId, Convert.ToInt32(keyValue), dtTime), false))
            //        if (objUtil.ErrCheck(dbSystem.AcknowledgeNotification(sn.UserID, sn.SecId, Convert.ToInt32(keyValue), dtTime), true))
            //        {
            //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Alarm Accept failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            //        }
            //}

            Notification_Fill_NewTZ();
            this.lblMessageNotifications.Text = "Notification(s) have been acknowledged";
        }

        protected void dgFleetVehiclesInfo_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
        {
            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
            {
                e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
                if (System.Globalization.CultureInfo.CurrentUICulture.ToString() == "fr-CA")
                    e.Layout.TextSettings.UseLanguage = "fr-FR";
                //else
                //   e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();      
            }
        }

        protected void dgNotification_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
        {
            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
            {
                e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
                if (System.Globalization.CultureInfo.CurrentUICulture.ToString() == "fr-CA")
                    e.Layout.TextSettings.UseLanguage = "fr-FR";
                //else
                //   e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();      
            }
        }

        protected void dgFleetDueServices_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
        {
            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
            {
                e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
                if (System.Globalization.CultureInfo.CurrentUICulture.ToString() == "fr-CA")
                    e.Layout.TextSettings.UseLanguage = "fr-FR";
                //else
                //   e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();      
            }

        }

        protected void dgHistory_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
        {
            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
            {
                e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
                if (System.Globalization.CultureInfo.CurrentUICulture.ToString() == "fr-CA")
                    e.Layout.TextSettings.UseLanguage = "fr-FR";
                //else
                //   e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();      
            }
        }

        private void ClearDataCached()
        {
            sn.History.DsNotifications = null;
            dgNotification.ClearCachedDataSource();
            dgNotification.RebindDataSource();

            sn.History.DsMaintenanceHistory = null;
            dgHistory.ClearCachedDataSource();
            dgHistory.RebindDataSource();

            sn.History.DsDueServices = null;
            dgFleetDueServices.ClearCachedDataSource();
            dgFleetDueServices.RebindDataSource();

            sn.History.DsMaintenance = null;
            dgFleetVehiclesInfo.ClearCachedDataSource();
            dgFleetVehiclesInfo.RebindDataSource();

        }

        private void ClearSelectedTab()
        {
            this.btnNotifications.CssClass = "confbutton";
            this.btnMaintenance.CssClass = "confbutton";
            this.btnAdministration.CssClass = "confbutton";
            this.btnServices.CssClass = "confbutton";
            this.btnMaintenanceHistory.CssClass = "confbutton";
            this.btnDTCNotifications.CssClass = "confbutton";
        }

        protected void btnDTCNotifications_Click(object sender, EventArgs e)
        {
            this.MultiView1.ActiveViewIndex = 5;
            ClearDataCached();
            ClearSelectedTab();
            this.btnDTCNotifications.CssClass = "selectedbutton";
            this.cboHoursFrom.SelectedIndex = -1;
            for (int i = 0; i <= cboHoursFrom.Items.Count - 1; i++)
            {
                if (Convert.ToInt32(cboHoursFrom.Items[i].Value) == DateTime.Now.AddHours(-48).Hour)
                {
                    cboHoursFrom.Items[i].Selected = true;
                    break;
                }
            }

            this.cboHoursTo.SelectedIndex = -1;
            for (int i = 0; i <= cboHoursTo.Items.Count - 1; i++)
            {
                if (Convert.ToInt32(cboHoursTo.Items[i].Value) == DateTime.Now.AddHours(1).Hour)
                {
                    cboHoursTo.Items[i].Selected = true;
                    break;
                }
            }
            LoadDTCcodes_NewTZ();
        }

        protected void cmdViewDTCCodes_Click(object sender, EventArgs e)
        {
            LoadDTCcodes_NewTZ();
        }

        // Changes for TimeZone Feature start
        private void LoadDTCcodes_NewTZ()
        {
            string xml = "";

            string strFromDate = "";
            string strToDate = "";

            if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
                strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

            if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
                strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

            if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
                strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

            if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
                strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

            if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
                strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

            if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
                strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";

            try
            {
                if (objUtil.ErrCheck(vehicleProxy.GetJ1708CodesVehicleFleet_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicle.SelectedItem.Value), Convert.ToInt32(this.cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(vehicleProxy.GetJ1708CodesVehicleFleet_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicle.SelectedItem.Value), Convert.ToInt32(this.cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        sn.Maint.DsDTCcodes = null;
                        dgDTCCode.RootTable.Rows.Clear();
                        dgDTCCode.ClearCachedDataSource();
                        dgDTCCode.RebindDataSource();
                        return;
                    }

                if (xml == "")
                {
                    sn.Maint.DsDTCcodes = null;
                    dgDTCCode.RootTable.Rows.Clear();
                    dgDTCCode.ClearCachedDataSource();
                    dgDTCCode.RebindDataSource();
                    return;
                }

                StringReader strrXML = new StringReader(xml);
                DataSet ds = new DataSet();

                string strPath = Server.MapPath("../Datasets/DTCcodes.xsd");
                ds.ReadXmlSchema(strPath);
                ds.ReadXml(strrXML);
                sn.Maint.DsDTCcodes = ds;
                dgDTCCode.ClearCachedDataSource();
                dgDTCCode.RebindDataSource();
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
                strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

            if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
                strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

            if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
                strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

            if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
                strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

            if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
                strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

            if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
                strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";

            try
            {
                if (objUtil.ErrCheck(vehicleProxy.GetJ1708CodesVehicleFleet(sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicle.SelectedItem.Value), Convert.ToInt32(this.cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(vehicleProxy.GetJ1708CodesVehicleFleet(sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicle.SelectedItem.Value), Convert.ToInt32(this.cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        sn.Maint.DsDTCcodes = null;
                        dgDTCCode.RootTable.Rows.Clear();
                        dgDTCCode.ClearCachedDataSource();
                        dgDTCCode.RebindDataSource();
                        return;
                    }

                if (xml == "")
                {
                    sn.Maint.DsDTCcodes = null;
                    dgDTCCode.RootTable.Rows.Clear();
                    dgDTCCode.ClearCachedDataSource();
                    dgDTCCode.RebindDataSource();
                    return;
                }

                StringReader strrXML = new StringReader(xml);
                DataSet ds = new DataSet();

                string strPath = Server.MapPath("../Datasets/DTCcodes.xsd");
                ds.ReadXmlSchema(strPath);
                ds.ReadXml(strrXML);
                sn.Maint.DsDTCcodes = ds;
                dgDTCCode.ClearCachedDataSource();
                dgDTCCode.RebindDataSource();
            }
            catch
            {
            }
        }

        protected void dgDTCCode_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            if ((sn.Maint.DsDTCcodes != null) && (sn.Maint.DsDTCcodes.Tables[0] != null) && (MultiView1.ActiveViewIndex == 5))
            {
                e.DataSource = sn.Maint.DsDTCcodes;
            }
            else
            {
                e.DataSource = null;
            }
        }

        private void CboFleet_Fill()
        {
            try
            {
                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();
                //cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void CboVehicle_Fill(int fleetId)
        {
            try
            {
                DataSet dsVehicle;
                dsVehicle = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                    {
                        cboVehicle.Items.Clear();
                        cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("StringSelectVehicle"), "0"));
                        return;
                    }
                if (xml == "")
                {
                    cboVehicle.Items.Clear();
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("StringSelectVehicle"), "0"));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();
                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("EntireFleet"), "-999"));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
            else
                this.cboVehicle.Items.Clear();
        }
    }
}