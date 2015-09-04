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
    public partial class Maintenance_frmMaintenanceNoti : SentinelFMBasePage
    {
        ServerDBOrganization.DBOrganization orgProxy = new ServerDBOrganization.DBOrganization();
        ServerDBVehicle.DBVehicle vehicleProxy = new ServerDBVehicle.DBVehicle();
        ServerDBFleet.DBFleet fleetProxy = new ServerDBFleet.DBFleet();
        public string adminText = "Admin ▼";
        string errorSave = "Save failed.";
        string errorDelete = "Delete failed.";
        public string selectFleet = "Select a Fleet";
        string errorDeleteAssign = "Can not be deleted because it has been assigned to vehicle(s).";
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        public string strUnitOfMes = "";
        public string showFilter = "Show Filter";
        public string hideFilter = "Hide Filter";
        public static string notification1Txt = "Notification1: ";
        public static string notification2Txt = "Notification2: ";
        public static string notification3Txt = "Notification3: ";
        public string Error_Load = "Failed to load data.";
        public string selectGroup = "Please select MCC group";
        public string selectMaintenance = "Please select MCC maintenance";

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
                    Notification_Fill_NewTZ();
                }
                this.Master.resizeScript = SetResizeScript(dgNotification.ClientID);
                this.Master.isHideScroll = true;
                dgNotification.RadAjaxManagerControl = RadAjaxManager1;

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

	dgNotification.HidePDFColumnID = "selectNotificationCheckBox"; //Remove CheckBox From NotificationGrid Export
        }

        protected void dgNotification_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BinddgNotification(false);
        }

        protected void cmdCloseNotification_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ServerDBSystem.DBSystem dbSystem = new ServerDBSystem.DBSystem();
            this.lblMessageNotifications.Text = "";
            // Changes for TimeZone Feature start
            DateTime dtTime = DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving); // Changes for TimeZone Feature end
            ArrayList keyValues = new ArrayList();
            ArrayList typeValues = new ArrayList();
            foreach (GridDataItem gridItem in dgNotification.MasterTableView.Items)
            {
                if (gridItem.ItemType == GridItemType.Item || gridItem.ItemType == GridItemType.AlternatingItem)
                {
                    CheckBox chkNotification = (CheckBox)gridItem.FindControl("chkNotification");
                    if (chkNotification != null && chkNotification.Checked)
                    {
                        keyValues.Add(gridItem.GetDataKeyValue("NotificationId"));
                        typeValues.Add(gridItem.GetDataKeyValue("TypeId"));
                    }
                }
            }


            if (keyValues.Count == 0)
            {
                this.lblMessageNotifications.Text = "Please select a Notification";
                return;
            }
            int index = 0;
	     VLF.DAS.Logic.Notification noti = new Notification(sConnectionString, false);	
            foreach (Int32 keyValue in keyValues)
            {
                
		//if (objUtil.ErrCheck(dbSystem.AcknowledgeNotification(sn.UserID, sn.SecId, keyValue, short.Parse(typeValues[index].ToString()), dtTime), false))
                //    if (objUtil.ErrCheck(dbSystem.AcknowledgeNotification(sn.UserID, sn.SecId, keyValue, short.Parse(typeValues[index].ToString()), dtTime), true))
                //    {
                //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Alarm Accept failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //    }
                noti.AckNotificationMaintenance (keyValue,short.Parse(typeValues[index].ToString()),sn.UserID,dtTime,"");

                index = index + 1;
            }

            Notification_Fill_NewTZ();
            this.lblMessageNotifications.Text = "Notification(s) have been acknowledged";
        }
        private void BinddgNotification(bool isBind)
        {
            if (sn.History.DsNotifications != null)
                dgNotification.DataSource = sn.History.DsNotifications;
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("NotificationId");

                dgNotification.DataSource = dt;
            }

            if (isBind) dgNotification.DataBind();
        }

        //Changes forTimeZone feature start
        private void Notification_Fill_NewTZ()
        {
            try
            {
                string strFromDT = "";
                string strToDT = "";

                int hours = 120;
                // Changes for TimeZone Feature start
                strFromDT = DateTime.Now.AddHours(-hours - sn.User.NewFloatTimeZone - sn.User.DayLightSaving).AddMinutes(-5).ToString("MM/dd/yyyy HH:mm:ss");
                strToDT = DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");

                strToDT = DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"); // Changes for TimeZone Feature end
                ServerDBUser.DBUser dbUser = new ServerDBUser.DBUser();
                string xml = "";
                if (objUtil.ErrCheck(dbUser.GetAllNotificationsForUserId_NewTZ(sn.UserID, sn.SecId, strFromDT, strToDT, ref xml), false))
                    if (objUtil.ErrCheck(dbUser.GetAllNotificationsForUserId_NewTZ(sn.UserID, sn.SecId, strFromDT, strToDT, ref xml), true))
                    {
                        sn.History.DsNotifications = null;
                        return;
                    }

                if (xml == "")
                {
                    DataSet ds1 = new DataSet();
                    string strPath1 = MapPath("..\\Maintenance\\Datasets") + @"\dstNotifications.xsd";
                    ds1.ReadXmlSchema(strPath1);
                    sn.History.DsNotifications = ds1;
                    BinddgNotification(true);
                    return;
                }

                if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
                    xml = xml.Replace("MIL light on", "TID allumé").Replace("MIL Light", "Témoin indicateur de dyscfonctionnement").Replace("Overdue", "Valeur dépassée").Replace("Prenotification", "Prénotifications");



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

                //dgNotification.ClearCachedDataSource();
                //dgNotification.RebindDataSource();
                BinddgNotification(true);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                string errorMsg = string.Format("alert(\"{0}\");", Error_Load);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                string errorMsg = string.Format("alert(\"{0}\");", Error_Load);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);

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
                {
                    DataSet ds1 = new DataSet();
                    string strPath1 = MapPath("..\\Maintenance\\Datasets") + @"\dstNotifications.xsd";
                    ds1.ReadXmlSchema(strPath1);
                    sn.History.DsNotifications = ds1;
                    BinddgNotification(true);
                    return;
                }

                if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
                    xml = xml.Replace("MIL light on", "TID allumé").Replace("MIL Light", "Témoin indicateur de dyscfonctionnement").Replace("Overdue", "Valeur dépassée").Replace("Prenotification", "Prénotifications");



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

                //dgNotification.ClearCachedDataSource();
                //dgNotification.RebindDataSource();
                BinddgNotification(true);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                string errorMsg = string.Format("alert(\"{0}\");", Error_Load);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                string errorMsg = string.Format("alert(\"{0}\");", Error_Load);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);

            }
        }

        public bool GetCheckBox(object drv)
        {
            if (drv != DBNull.Value) return bool.Parse(drv.ToString());
            else return false;

        }
    }

}