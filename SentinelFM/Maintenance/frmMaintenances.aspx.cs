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
    public partial class Maintenance_frmMaintenances : SentinelFMBasePage
    {
        public string Error_Load = "Failed to load data.";  //(string)base.GetLocalResourceObject("Error_Load");
        string errorSave = "Save failed."; //(string)base.GetLocalResourceObject("errorSave");
        string errorDelete = "Delete failed."; //(string)base.GetLocalResourceObject("errorDelete");
        public string selectFleet = "Select a Fleet"; //(string)base.GetLocalResourceObject("selectFleet");
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        public string showFilter = "Show Filter"; //(string)base.GetLocalResourceObject("showFilter");
        public string hideFilter = "Hide Filter";  //(string)base.GetLocalResourceObject("hideFilter");

        public string deleteMaintenance = "Delete this PM Service?"; //(string)base.GetLocalResourceObject("deleteMaintenance");
        DropDownList ddlMCCMaintenanceOperationTypes = null;
        DropDownList ddlMCCMaintenanceNotificationType = null;
        DropDownList ddlMCCMaintenanceFrequencyID = null;
        RadNumericTextBox txtMCCMaintenanceInterval = null;
        DropDownList ddlMCCMaintenanceInterval = null;
        CheckBox chkFixedInterval = null;
        CheckBox chkFixedDate = null;
        DropDownList ddlMonth = null;
        DropDownList ddlDay = null;
        TextBox txtMCCMaintenanceDescription = null;
        Button btnMCCMaintenanceSave = null;
        Button btnMCCMaintenanceSaveTmp = null;
        Button btnMCCMaintenanceCancel = null;
        Panel pnlDate = null;
        Panel pnlMonthDay = null;

        public static string notification1Txt = "Notification1: ";
        public static string notification2Txt = "Notification2: ";
        public static string notification3Txt = "Notification3: ";
        DataTable timespanDt = null;
        DataTable frequencyDt = null;
        protected void Page_Load(object sender, EventArgs e)
        {

              Error_Load = (string)base.GetLocalResourceObject("Error_Load");
         errorSave = (string)base.GetLocalResourceObject("errorSave");
         errorDelete = (string)base.GetLocalResourceObject("errorDelete");
          selectFleet = (string)base.GetLocalResourceObject("selectFleet");
        showFilter = (string)base.GetLocalResourceObject("showFilter");
        hideFilter = (string)base.GetLocalResourceObject("hideFilter");
         deleteMaintenance = (string)base.GetLocalResourceObject("deleteMaintenance");

            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
            }

            if (!IsPostBack)
            {
                this.Master.CreateMaintenenceMenu(null);
            }
            this.Master.resizeScript = SetResizeScript(gdMCCMaintenances.ClientID);
            this.Master.isHideScroll = true;
            gdMCCMaintenances.RadAjaxManagerControl = RadAjaxManager1;
            
            Literal lit = new Literal();

            lit.Text = "<link href='../Scripts/css/tooltip.css' type='text/css' rel='stylesheet'></link> " ;
            this.Page.Header.Controls.Add(lit);
        }

        protected void gdMCCMaintenances_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
                BindMCCMaintenances(false);
        }
        private void BindMCCMaintenances(bool isBind)
        {
            try
            {
                MCCManager mccMg = new MCCManager(sConnectionString);
                //string xml = mccMg.GetMCCMaintenances(sn.User.OrganizationId, GetOperationTypeStr(), sn.UserID).GetXml().Replace("Once", "Une seule fois").Replace("Recurring", "Récurrent");
                //DataSet ds = new DataSet();
                //ds.ReadXml(new StringReader(xml));
                DataSet ds = mccMg.GetMCCMaintenances(sn.User.OrganizationId, GetOperationTypeStr(), sn.UserID);
                gdMCCMaintenances.DataSource = ds;
                if (isBind) gdMCCMaintenances.DataBind();
            }
            catch (Exception Ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("MaintenanceId");
                     
                gdMCCMaintenances.DataSource = dt;
                if (isBind) gdMCCMaintenances.DataBind();

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                string errorMsg = string.Format("alert(\"{0}\");", Error_Load);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);

            }

        }


        protected void gdMCCMaintenances_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            if (((GridDataItem)e.Item).GetDataKeyValue("MaintenanceId") != null)
            {
                try
                {
                    int maintenanceId = int.Parse(((GridDataItem)e.Item).GetDataKeyValue("MaintenanceId").ToString());
                    MCCManager mccMgr = new MCCManager(sConnectionString);
                    mccMgr.DeleteMCCMaintenances(maintenanceId, sn.User.OrganizationId, sn.UserID);
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                    ExceptionLogger(trace);
                    e.Canceled = true;
                    string errorMsg = string.Format("alert(\"{0}\");", errorDelete);
                    RadAjaxManager1.ResponseScripts.Add(errorMsg);
                }
            }

        }
        protected void gdMCCMaintenances_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "InitInsert" || e.CommandName == "Edit" || e.CommandName == "Delete")
            {
                if (e.CommandName == "Edit") gdMCCMaintenances.MasterTableView.IsItemInserted = false;
                gdMCCMaintenances.MasterTableView.ClearEditItems();
            }

            if (e.Item is GridEditFormItem && e.Item.IsInEditMode)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
                System.Web.UI.UserControl editFormUserControl = (System.Web.UI.UserControl)
                    item.FindControl(GridEditFormItem.EditFormUserControlID);
                if ((e.CommandName == "PerformInsert" || e.CommandName == "Update") && editFormUserControl != null)
                {
                    if (!Page.IsValid) return;
                    GetMCCMaintenancesControls(editFormUserControl);

                    int operationTypeID = int.Parse(ddlMCCMaintenanceOperationTypes.SelectedValue);
                    int frequencyID = 0;
                    frequencyID = int.Parse(ddlMCCMaintenanceFrequencyID.SelectedValue);
                    int interval = 0;
                    int? timespanId = null;
                    bool fixedInterval = false;
                    bool fixedDate = false;
                    string fixedServiceDate = null;
                    fixedInterval = chkFixedInterval.Checked;
                    if (operationTypeID != 3)
                    {
                        if (txtMCCMaintenanceInterval.Value.HasValue)
                            interval = (int)txtMCCMaintenanceInterval.Value;
                    }
                    else
                    {
                        if (timespanDt == null )
                        {
                            MCCManager mccMgr = new MCCManager(sConnectionString);
                            timespanDt = mccMgr.GetTimespanConventionTypes().Tables[0];
                        }
                        foreach (DataRow dr in timespanDt.Rows)
                        {
                            if (dr["TimespanId"].ToString() == ddlMCCMaintenanceInterval.SelectedValue)
                            {
                                timespanId = int.Parse(ddlMCCMaintenanceInterval.SelectedValue);
                                interval = int.Parse(dr["NumberOfDays"].ToString());
                                break;
                            }
                        }
                        fixedDate = chkFixedDate.Checked;
                        if (fixedDate)
                        {
                            fixedServiceDate = ddlMonth.SelectedValue + "/" + ddlDay.SelectedValue;
                        }
                    }
                    string description = txtMCCMaintenanceDescription.Text.Trim();
                    int? notificationType = null;
                    if (ddlMCCMaintenanceNotificationType.SelectedIndex >= 0)
                    {
                        notificationType = int.Parse(ddlMCCMaintenanceNotificationType.SelectedValue);
                    }

                    if (e.CommandName == "PerformInsert")
                    {
                        try
                        {
                            MCCManager mccMgr = new MCCManager(sConnectionString);
                            mccMgr.AddMCCMaintenances(sn.User.OrganizationId, operationTypeID, notificationType, frequencyID, interval, description, timespanId, fixedInterval, sn.UserID, fixedServiceDate, fixedDate);
                        }
                        catch (Exception Ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                            ExceptionLogger(trace);
                            e.Canceled = true;
                            string errorMsg = string.Format("alert(\"{0}\");", errorSave);
                            RadAjaxManager1.ResponseScripts.Add(errorMsg);
                        }
                    }
                    if (e.CommandName == "Update")
                    {
                        try
                        {
                            int maintenanceId = int.Parse(((GridEditFormItem)e.Item).GetDataKeyValue("MaintenanceId").ToString());
                            MCCManager mccMgr = new MCCManager(sConnectionString);
                            int fixedDueDate = 0;
                            if (fixedDate)
                            {
                                fixedDueDate = GetDueDateByFixedDate(timespanId, fixedServiceDate);
                            }
                            mccMgr.UpdateMCCMaintenances(maintenanceId, sn.User.OrganizationId, operationTypeID, notificationType, frequencyID, interval, description, timespanId, fixedInterval, sn.UserID, fixedServiceDate, fixedDate, fixedDueDate);
                        }
                        catch (Exception Ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                            ExceptionLogger(trace);
                            e.Canceled = true;
                            string errorMsg = string.Format("alert(\"{0}\");", errorSave);
                            RadAjaxManager1.ResponseScripts.Add(errorMsg);
                        }

                    }
                }
            }
        }

        private void GetMCCMaintenancesControls(System.Web.UI.UserControl usercontrol)
        {
            ddlMCCMaintenanceOperationTypes = (DropDownList)usercontrol.FindControl("ddlMCCMaintenanceOperationTypes");
            ddlMCCMaintenanceNotificationType = (DropDownList)usercontrol.FindControl("ddlNotificationType");
            ddlMCCMaintenanceFrequencyID = (DropDownList)usercontrol.FindControl("ddlFrequencyID");
            txtMCCMaintenanceInterval = (RadNumericTextBox)usercontrol.FindControl("txtInterval");
            txtMCCMaintenanceDescription = (TextBox)usercontrol.FindControl("txtDescription");
            btnMCCMaintenanceSave = (Button)usercontrol.FindControl("btnSave");
            btnMCCMaintenanceSaveTmp = (Button)usercontrol.FindControl("btnSaveTmp");
            btnMCCMaintenanceCancel = (Button)usercontrol.FindControl("btnCancel");
            ddlMCCMaintenanceInterval = (DropDownList)usercontrol.FindControl("ddlInterval");
            chkFixedInterval = (CheckBox)usercontrol.FindControl("chkFixedInterval");
            chkFixedDate = (CheckBox)usercontrol.FindControl("chkFixedDate");
            ddlMonth = (DropDownList)usercontrol.FindControl("ddlMonth");
            ddlDay = (DropDownList)usercontrol.FindControl("ddlDay");
            pnlDate = (Panel)usercontrol.FindControl("pnlDate");
            pnlMonthDay = (Panel)usercontrol.FindControl("pnlMonthDay");

            if (timespanDt == null && ddlMCCMaintenanceInterval.Items.Count <= 0)
            {
                MCCManager mccMgr = new MCCManager(sConnectionString);
               // timespanDt = mccMgr.GetTimespanConventionTypes().Tables[0];
               // ddlMCCMaintenanceInterval.DataSource = timespanDt;

                string xml = mccMgr.GetTimespanConventionTypes().GetXml();

                if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                     xml = xml.Replace("Daily", "Quotidien").Replace("Weekly", "Hebdomadaire")
                        .Replace("Bi-Weekly", "Bi-hebdomadaire").Replace("Monthly", "Mensuel")
                        .Replace("Bi-Monthly", "Bi-mensuel").Replace("Quaterly", "Trimestriel")
                        .Replace("Semi-Annually", "Semestrie").Replace("Annual", "Annuel")
                        .Replace("Bi-Annual", "Bi-annuel").Replace("None", "Aucun");


                DataSet dsMgr = new DataSet();
                dsMgr.ReadXml(new StringReader(xml));
                ddlMCCMaintenanceInterval.DataSource = dsMgr.Tables[0];
                ddlMCCMaintenanceInterval.DataBind();
                ddlMCCMaintenanceInterval.SelectedIndex = 0;
            }

            if (frequencyDt == null && ddlMCCMaintenanceFrequencyID.Items.Count <= 0)
            {
                MCCManager mccMgr = new MCCManager(sConnectionString);
                string xml = mccMgr.GetMaintenanceFrequency().GetXml();
                 if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                     xml = xml.Replace("Once", "Une seule fois").Replace("Recurring", "Récurrent");

                DataSet ds = new DataSet();
                ds.ReadXml(new StringReader(xml));

                frequencyDt = ds.Tables[0];  //mccMgr.GetMaintenanceFrequency().Tables[0];
                ddlMCCMaintenanceFrequencyID.DataSource = frequencyDt;
                ddlMCCMaintenanceFrequencyID.DataBind();
                ddlMCCMaintenanceFrequencyID.SelectedIndex = 0;
            }

        }

        protected void gdMCCMaintenances_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditFormItem && e.Item.IsInEditMode)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
                System.Web.UI.UserControl editFormUserControl = (System.Web.UI.UserControl)
                    item.FindControl(GridEditFormItem.EditFormUserControlID);

                if (editFormUserControl != null)
                {
                    GetMCCMaintenancesControls(editFormUserControl);
                    btnMCCMaintenanceCancel.CommandName = "Cancel";
                    DataTable dtOperationTye = clsAsynGenerateReport.GetOperationTypeTable();

                    DataRow deleteDrow = null;
                    foreach (DataRow drO in dtOperationTye.Rows)
                    {
                        if (drO["id"].ToString() == "0")
                        {
                            deleteDrow = drO;
                            break;
                        }
                    }
                    dtOperationTye.Rows.Remove(deleteDrow);
                    ddlMCCMaintenanceOperationTypes.DataSource = dtOperationTye;

                    ddlMCCMaintenanceOperationTypes.ClearSelection();
                    ddlMCCMaintenanceOperationTypes.DataBind();
                    ddlMCCMaintenanceOperationTypes.SelectedIndex = 0;
                    if (e.Item.DataItem is DataRowView)
                    {
                        btnMCCMaintenanceSave.CommandName = "Update";
                        btnMCCMaintenanceSaveTmp.CommandName = "Update";
                        
                        DataRowView dr = (DataRowView)e.Item.DataItem;
                        btnMCCMaintenanceSave.Attributes.Add("onclick", "return ClickEditEvent(" + dr["MaintenanceId"].ToString() + ",'" + btnMCCMaintenanceSaveTmp.ClientID + "','" + btnMCCMaintenanceSave.ValidationGroup + "')");
                        //while (ddlMCCMaintenanceNotificationType.Items.Count >= 1)
                        //    ddlMCCMaintenanceNotificationType.Items.RemoveAt(0);

                        ddlMCCMaintenanceNotificationType.ClearSelection();
                        ddlMCCMaintenanceNotificationType.Items.Clear();

                        if (dr["OperationTypeID"] != DBNull.Value)
                        {
                            ListItem listItem = ddlMCCMaintenanceOperationTypes.Items.FindByValue(dr["OperationTypeID"].ToString());
                            if (listItem != null)
                            {
                                ddlMCCMaintenanceOperationTypes.ClearSelection();
                                listItem.Selected = true;
                            }
                            MCCManager mccMg = new MCCManager(sConnectionString);

                            ddlMCCMaintenanceNotificationType.DataSource = mccMg.GetMCCNotificationType(sn.User.OrganizationId, int.Parse(dr["OperationTypeID"].ToString()), null, string.Empty).Tables[0];
                            ddlMCCMaintenanceNotificationType.DataBind();

                        }

                        if (dr["OperationTypeID"].ToString() == "3")
                        {
                            txtMCCMaintenanceInterval.Visible = false;
                            ddlMCCMaintenanceInterval.Visible = true;
                            pnlDate.Visible = true;
                            if (dr["TimespanId"] != DBNull.Value)
                            {
                                ListItem listItem = ddlMCCMaintenanceInterval.Items.FindByValue(dr["TimespanId"].ToString());
                                if (listItem != null)
                                {
                                    ddlMCCMaintenanceInterval.ClearSelection();
                                    listItem.Selected = true;
                                }
                            }

                            if (dr["FixedDate"] != DBNull.Value)
                            {
                                if ((Boolean)dr["FixedDate"]) chkFixedDate.Checked = true;
                                if (chkFixedDate.Checked)
                                {
                                    if (dr["FixedServiceDate"] != DBNull.Value)
                                    {
                                        string fixServiceDate = dr["FixedServiceDate"].ToString();
                                        string[] monthdays = fixServiceDate.Split('/');
                                        try
                                        {
                                            ddlMonth.SelectedValue = monthdays[0];
                                            ddlDay.SelectedValue = monthdays[1];
                                        }
                                        catch (Exception ex) { }

                                        ListItem itemlist = ddlMCCMaintenanceInterval.Items.FindByValue("1");
                                        if (itemlist != null) itemlist.Enabled = false;
                                        itemlist = ddlMCCMaintenanceInterval.Items.FindByValue("2");
                                        if (itemlist != null) itemlist.Enabled = false;
                                        itemlist = ddlMCCMaintenanceInterval.Items.FindByValue("3");
                                        if (itemlist != null) itemlist.Enabled = false;
                                        itemlist = ddlMCCMaintenanceInterval.Items.FindByValue("9");
                                        if (itemlist != null) itemlist.Enabled = false;
                                        itemlist = ddlMCCMaintenanceInterval.Items.FindByValue("999");
                                        if (itemlist != null) itemlist.Enabled = false;

                                        if (chkFixedDate.Checked && ddlMCCMaintenanceInterval.SelectedValue != "8")
                                        {
                                            ListItem listItem = ddlMonth.Items.FindByValue("None");
                                            if (listItem != null)
                                            {
                                                ddlMonth.ClearSelection();
                                                listItem.Selected = true;
                                            }
                                            ddlMonth.Enabled = false;
                                        }

                                        //ddlInterval.Enabled = false;
                                        //chkFixedInterval.Checked = true;
                                        chkFixedInterval.Enabled = false;
                                        //pnlMonthDay.Visible = true;
                                    }
                                    //pnlDate.Visible = true;
                                    pnlMonthDay.Visible = true;
                                    chkFixedInterval.Enabled = false;
                                }
                                else
                                {
                                    //pnlDate.Visible = false;
                                    pnlMonthDay.Visible = false;
                                }
                            }
                        }
                        else
                        {
                            txtMCCMaintenanceInterval.Visible = true;
                            ddlMCCMaintenanceInterval.Visible = false;
                            pnlDate.Visible = false;
                        }

                        if (dr["FixedInterval"] != DBNull.Value)
                        {
                            if ((Boolean)dr["FixedInterval"]) chkFixedInterval.Checked = true;
                        }
                        if (dr["NotificationTypeID"] != DBNull.Value)
                        {
                            ListItem listItem = ddlMCCMaintenanceNotificationType.Items.FindByValue(dr["NotificationTypeID"].ToString());
                            if (listItem != null)
                            {
                                ddlMCCMaintenanceNotificationType.ClearSelection();
                                listItem.Selected = true;
                            }
                        }


                        if (dr["FrequencyID"] != DBNull.Value)
                        {
                            ListItem listItem = ddlMCCMaintenanceFrequencyID.Items.FindByValue(dr["FrequencyID"].ToString());
                            if (listItem != null)
                            {
                                ddlMCCMaintenanceFrequencyID.ClearSelection();
                                listItem.Selected = true;
                            }

                        }

                        if (dr["IntervalDesc"] != DBNull.Value)
                        {
                            txtMCCMaintenanceInterval.Text = dr["IntervalDesc"].ToString();
                        }

                        if (dr["Description"] != DBNull.Value)
                            txtMCCMaintenanceDescription.Text = dr["Description"].ToString();


                    }
                    else
                    {
                        MCCManager mccMg = new MCCManager(sConnectionString);

                        ddlMCCMaintenanceNotificationType.DataSource = mccMg.GetMCCNotificationType(sn.User.OrganizationId, int.Parse(ddlMCCMaintenanceOperationTypes.SelectedValue), null, string.Empty).Tables[0];
                        ddlMCCMaintenanceNotificationType.DataBind();
                        ddlMCCMaintenanceNotificationType.ClearSelection();
                        ddlMCCMaintenanceNotificationType.SelectedIndex = 0;

                        btnMCCMaintenanceSave.CommandName = "PerformInsert";
                    }


                }
            }

            if (e.Item.DataItem != null && e.Item.DataItem is DataRowView && e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                DataRowView drv = (DataRowView)e.Item.DataItem;
                if (drv["OrganizationId"] != DBNull.Value && drv["OrganizationId"].ToString() == "0")
                {
                    item["EditCommandColumn"].Controls[0].Visible = false;
                    item["DeleteColumn"].Controls[0].Visible = false;
                    Label lbl = new Label();
                    lbl.Text = "&nbsp;";
                    item["EditCommandColumn"].Controls.Add(lbl);
                    Label lbl1 = new Label();
                    lbl1.Text = "&nbsp;";
                    item["DeleteColumn"].Controls.Add(lbl1);
                }
                NotificationTooptip(item);

                ImageButton deleteColumn = (ImageButton)item["DeleteColumn"].Controls[0];
                Button btnDelete = (Button)item.FindControl("btnDelete");
                deleteColumn.Attributes.Add("OnClick", "return ClickDeleteEvent(" + drv["MaintenanceId"].ToString() + ",'" + btnDelete.ClientID + "');"); 

            }

        }

        private static void NotificationTooptip(GridDataItem item)
        {
            DataRowView drv = (DataRowView)item.DataItem;
            if (drv["NotificationTypeID"] != DBNull.Value)
            {
                string notification = string.Empty;
                if (drv["Notification1"] != DBNull.Value && drv["Notification1"].ToString() != string.Empty)
                {
                    notification = "<b>" + notification1Txt + "</b>" + drv["Notification1"].ToString();
                }
                else notification = "<b>" + notification1Txt + "</b>";

                if (drv["Notification2"] != DBNull.Value && drv["Notification2"].ToString() != string.Empty)
                {
                    notification = notification + "<br /><b>" + notification2Txt + "</b>" + drv["Notification2"].ToString();
                }
                else notification = notification + "<br /><b>" + notification2Txt + "</b>";

                if (drv["Notification3"] != DBNull.Value && drv["Notification3"].ToString() != string.Empty)
                {
                    notification = notification + "<br /><b>" + notification3Txt + "</b>" + drv["Notification3"].ToString();
                }
                else notification = notification + "<br /><b>" + notification3Txt + "</b>";
                notification.Replace("'", "/'");
                //Control lnk = item["NotificationType"].Controls[0];
                //if (lnk is HyperLink)
                //{
                //    ((HyperLink)lnk).Attributes.Add("onclick", "ShowToolTipScreen(135, '" + notification + "', this)");
                //}
            }
        }

        private string GetOperationTypeStr()
        {
            return clsAsynGenerateReport.GetOperationTypeStr();
        }

        private int GetDueDateByFixedDate(int? timeSpan, string monthDaysStr)
        {
            string[] monthDay = monthDaysStr.Split('/');
            DateTime dt = System.DateTime.Now.Date;
            if (timeSpan == 4)
            {
                DateTime dt_1 = new DateTime(dt.Year, dt.Month, 1).AddDays(int.Parse(monthDay[1]) - 1);
                if (dt_1 < dt) dt = new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(int.Parse(monthDay[1]) - 1);
                else dt = dt_1;
            }
            int i_mode = 0;
            if (timeSpan == 5 || timeSpan == 6 || timeSpan == 7)
            {
                if (timeSpan == 5) i_mode = 2;
                if (timeSpan == 6) i_mode = 3;
                if (timeSpan == 7) i_mode = 6;

                int lowMonth = (dt.Month - 1) / i_mode;
                int hightMonth = (dt.Month - 1) / i_mode + 1;

                DateTime dt_1 = new DateTime(dt.Year, 1, 1).AddMonths(lowMonth * i_mode).AddDays(int.Parse(monthDay[1]) - 1);
                if (dt_1 < dt)
                {
                    dt = new DateTime(dt.Year, 1, 1).AddMonths(hightMonth * i_mode).AddDays(int.Parse(monthDay[1]) - 1);
                }
                else dt = dt_1;
            }
            if (timeSpan == 8)
            {
                DateTime dt_1 = new DateTime(dt.Year, 1, 1).AddMonths(int.Parse(monthDay[0]) - 1).AddDays(int.Parse(monthDay[1]) - 1);
                if (dt_1 < dt) dt = new DateTime(dt.Year + 1, 1, 1).AddMonths(int.Parse(monthDay[0]) - 1).AddDays(int.Parse(monthDay[1]) - 1);
                else dt = dt_1;
            }
            return (int)dt.Subtract(new DateTime(1970, 1, 1)).TotalDays;
        }
        [WebMethod]
        public static string GetVehiclesandGroup(int MaintenanceId)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                ArrayList result = new ArrayList();
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                MCCManager mccMg = new MCCManager(sConnectionString);
                DataSet ds = mccMg.MaintenanceGetVehiclesByMaintenanceId(MaintenanceId);
                if (ds.Tables[0].Rows.Count  > 0)
                {
                    ArrayList vehicles = new ArrayList();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Dictionary<string, string> vehicle = new Dictionary<string, string>();
                        vehicle.Add("BoxId", dr["BoxId"] is DBNull ? string.Empty : dr["BoxId"].ToString());
                        vehicle.Add("Description", dr["Description"] is DBNull ? string.Empty : dr["Description"].ToString());
                        vehicles.Add(vehicle);
                    }

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    js.MaxJsonLength = int.MaxValue;
                    result.Add(js.Serialize(vehicles));
                }
                else result.Add("");

                ds = mccMg.MaintenanceGetMccGroupByMaintenanceId(MaintenanceId, sn.User.OrganizationId);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ArrayList groups = new ArrayList();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Dictionary<string, string> group = new Dictionary<string, string>();
                        group.Add("MccName", dr["MccName"] is DBNull ? string.Empty : dr["MccName"].ToString());
                        groups.Add(group);
                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    js.MaxJsonLength = int.MaxValue;
                    string javaScript = js.Serialize(groups);
                    result.Add(javaScript);
                }
                else result.Add("");

                if (result.Count > 0)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    js.MaxJsonLength = int.MaxValue;
                    return (js.Serialize(result));
                }
                else return "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: MccServiceAssigment_Add() Page:frmMaintenanceNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";
        }
    }
}