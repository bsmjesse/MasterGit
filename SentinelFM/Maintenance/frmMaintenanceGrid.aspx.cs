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

    public partial class Maintenance_frmMaintenanceGrid : SentinelFMBasePage
    {

        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        DataTable dtOperationType = null;
        //public string inputCalError_s = @"Must be equal or less than today\'s date."; //(string)base.GetLocalResourceObject("inputCalError_s");
        //public string inputError = "Value must be greater than current value.";  //(string)base.GetLocalResourceObject("inputError");
        //public string inputError_s = "Value must be equal or less than current value."; //(string)base.GetLocalResourceObject("inputError_s");
        //public string inputCalError = "Must be equal or greater than /01/01/1970.";  //(string)base.GetLocalResourceObject("inputCalError");
        //public string selectedError = "Please select vehicle(s)"; //(string)base.GetLocalResourceObject("selectedError");
        //public string closeVehicles1 = "This action will close services for (1) vehicle, continue?";  //(string)base.GetLocalResourceObject("closeVehicles1");
        //public string closeVehicles2 = "This action will close services for (n) vehicles, continue?"; //(string)base.GetLocalResourceObject("closeVehicles2");
        //public string closeError = "Failed to close the following vehicles"; //(string)base.GetLocalResourceObject("closeError");
        //public string updateError = "Update fail because service value is equal or greater next due value.";  //(string)base.GetLocalResourceObject("updateError");
        //public string Error_Load = "Failed to load data."; //(string)base.GetLocalResourceObject("Error_Load");
        //public string errorSave = "Save failed.";  //(string)base.GetLocalResourceObject("errorSave");
        //public string saveSucceed = "Saved Successfully."; //(string)base.GetLocalResourceObject("saveSucceed");



        public string inputCalError_s ="";// (string)base.GetLocalResourceObject("inputCalError_s");
        public string inputError = "";// (string)base.GetLocalResourceObject("inputError");
        public string inputError_s = "";//(string)base.GetLocalResourceObject("inputError_s");
        public string inputCalError = "";//(string)base.GetLocalResourceObject("inputCalError");
        public string selectedError = "";//(string)base.GetLocalResourceObject("selectedError");
        public string closeVehicles1 = "";//(string)base.GetLocalResourceObject("closeVehicles1");
        public string closeVehicles2 = "";//(string)base.GetLocalResourceObject("closeVehicles2");
        public string closeError = "";// (string)base.GetLocalResourceObject("closeError");
        public string updateError = "";//(string)base.GetLocalResourceObject("updateError");
        public string Error_Load = "";//(string)base.GetLocalResourceObject("Error_Load");
        public string errorSave = "";//(string)base.GetLocalResourceObject("errorSave");
        public string saveSucceed = "";//(string)base.GetLocalResourceObject("saveSucceed");
        public string ValueExceedAllowedValueError = "";
        public string RadWindowContentYes = "";
        public string RadWindowContentNo = "";

        protected void Page_Load(object sender, EventArgs e)
        {

              inputCalError_s = GetScriptEscapeString((string)base.GetLocalResourceObject("inputCalError_s"));
         inputError = GetScriptEscapeString((string)base.GetLocalResourceObject("inputError"));
         inputError_s =GetScriptEscapeString((string)base.GetLocalResourceObject("inputError_s"));
         inputCalError =GetScriptEscapeString((string)base.GetLocalResourceObject("inputCalError"));
         selectedError = GetScriptEscapeString((string)base.GetLocalResourceObject("selectedError"));
         closeVehicles1 = GetScriptEscapeString((string)base.GetLocalResourceObject("closeVehicles1"));
         closeVehicles2 = GetScriptEscapeString((string)base.GetLocalResourceObject("closeVehicles2"));
         closeError = GetScriptEscapeString((string)base.GetLocalResourceObject("closeError"));
         updateError =GetScriptEscapeString((string)base.GetLocalResourceObject("updateError"));
         Error_Load = GetScriptEscapeString((string)base.GetLocalResourceObject("Error_Load"));
         errorSave = GetScriptEscapeString((string)base.GetLocalResourceObject("errorSave"));
         saveSucceed =GetScriptEscapeString((string)base.GetLocalResourceObject("saveSucceed"));
         RadWindowContentYes = GetScriptEscapeString((string)base.GetLocalResourceObject("RadWindowContentCommentResource1.Localization.Yes"));
         RadWindowContentNo = GetScriptEscapeString((string)base.GetLocalResourceObject("RadWindowContentCurrentResource1.Localization.No"));

         ValueExceedAllowedValueError = GetScriptEscapeString((string)base.GetLocalResourceObject("ValueExceedAllowedValue"));

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
                FleetVehicle1.Vehicle_SelectedIndexChanged += new EventHandler(FleetVehicle1_Vehicle_SelectedIndexChanged);
                //FleetVehicle1.radAjaxManager1 = RadAjaxManager1;
                //FleetVehicle1.radAjaxLoadingPanel1 = LoadingPanel1.ID;
                //FleetVehicle1.radUpdatedControl = pnl.ID;
                dgMaintenance.RadAjaxManagerControl = RadAjaxManager1;
                //FleetVehicle1.isLoadDefault = true;
                //FleetVehicle1.Fleet_SelectedIndexChanged += new EventHandler(FleetVehicle1_Fleet_SelectedIndexChanged);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                base.RedirectToLogin();
            }
            this.Master.resizeScript = SetResizeScript(dgMaintenance.ClientID);
            this.Master.isHideScroll = true;
            inputCalError = GetScriptEscapeString(inputCalError);
            string redFontStyle = "<style type='text/css'>.MaintenanceGridRedFontStyle input {color:red !important} </style>";
            Literal lit = new Literal();
            lit.Text = redFontStyle;
            this.Header.Controls.Add(lit);

            redFontStyle = "<style type='text/css'>.disabledCss {background-color: #dcdcdc !important; color: #000000 !important; } </style>";
            lit = new Literal();
            lit.Text = redFontStyle;
            this.Header.Controls.Add(lit);

            try
            {
                Error_Load = GetScriptEscapeString(GetGlobalResourceObject("Const", "Error_Load").ToString());
            }
            catch (Exception ex)
            { }
            //inputCalError_s = GetScriptEscapeString(inputCalError_s);

            if (Request.QueryString["FleetID"] != null && Request.QueryString["FleetID"].Trim() != string.Empty)
            {
                string fleetID = Request.QueryString["FleetID"].ToString();
                RadComboBox fleetCombs = (RadComboBox)FleetVehicle1.FindControl("cboFleet");
                if (fleetCombs != null)
                {
                    RadComboBoxItem rItem = fleetCombs.Items.FindItemByValue(fleetID);
                    rItem.Selected = true;
                }
            }
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

        protected void dgMaintenance_NeedDataSource(object sender, EventArgs e)
        {
            BinddgMaintenance(false);
        }

        private void BinddgMaintenance(Boolean isBind)
        {
            try
            {
                MCCManager mccMgr = new MCCManager(sConnectionString);
                string selectedVehicles = FleetVehicle1.GetAllSelectedVehicle();
                if (selectedVehicles != string.Empty)
                {
                    DataSet ds = mccMgr.MaintenanceGetVehicleServices(selectedVehicles, sn.UserID);

                    //to be changed
                    //if (ds.Tables[0].Rows.Count > 0) ds.Tables[0].Rows[0]["OperationTypeID"] = "3";
                    dgMaintenance.DataSource = ds;
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("VehicleId");
                    dt.Columns.Add("MaintenanceID");
                    dgMaintenance.DataSource = dt;
                }
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

        protected void dgMaintenance_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.DataItem != null && e.Item.DataItem is DataRowView)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                RadNumericTextBox txtValue = (RadNumericTextBox)e.Item.FindControl("txtValue");
                RadDatePicker calValue = (RadDatePicker)e.Item.FindControl("calValue");
                HiddenField hidOrgValue = (HiddenField)e.Item.FindControl("hidOrgValue");
                ImageButton imgComments = (ImageButton)e.Item.FindControl("imgComments");
                CheckBox chkselectVehicle = (CheckBox)e.Item.FindControl("chkselectVehicle");
                RadDatePicker calServiceDate = (RadDatePicker)e.Item.FindControl("calServiceDate");
                RadNumericTextBox txtCurrentOdoHrs = (RadNumericTextBox)e.Item.FindControl("txtCurrentOdoHrs");
                ImageButton lnkUseCurrentValue = (ImageButton)e.Item.FindControl("lnkUseCurrentValue");
                HyperLink hplBoxId = (HyperLink)e.Item.FindControl("hplBoxId");
                if (imgComments != null) imgComments.Attributes.Add("onclick", "return ShowEditComment(" + e.Item.ItemIndex + ")");
                if (drv["OperationTypeID"] != DBNull.Value && txtValue != null && calValue != null)
                {
                    if (hplBoxId != null)
                    { 
                        string hrs = string.Empty;
                        string odometer = string.Empty;
                        if (drv["Odometer"] != DBNull.Value) {
                            odometer = drv["Odometer"].ToString();
                        }
                        if (drv["EngineHrs"] != DBNull.Value) 
                        {
                            hrs = drv["EngineHrs"].ToString();
                        }
                        hplBoxId.Attributes.Add("href", "javascript:ShowCurrentValueWin('" + hrs + "','" + odometer + "');");
                    }
                    if (calServiceDate != null)
                       calServiceDate.MaxDate = System.DateTime.Now.Date;
                    if (drv["FixedInterval"] != DBNull.Value && Boolean.Parse(drv["FixedInterval"].ToString()))
                    {
                        //calServiceDate.Enabled = false;
                        txtCurrentOdoHrs.Enabled = false;
                        txtValue.ReadOnly = true;
                        txtValue.CssClass = "disabledCss";
                        lnkUseCurrentValue.Enabled = false;
                        //lnkUseCurrentValue.Visible = false;
                        lnkUseCurrentValue.Style.Add("visibility", "hidden");
                        calValue.DatePopupButton.Enabled = false;
                        //calValue.Calendar.Enabled = false;
                        calValue.DateInput.ReadOnly = true;
                        calValue.DateInput.CssClass = "disabledCss";
                    }


                    if (drv["CurrentVal"] != DBNull.Value)
                    {
                        txtCurrentOdoHrs.Text = drv["CurrentVal"].ToString();
                        txtCurrentOdoHrs.MaxValue = (int)float.Parse(drv["CurrentVal"].ToString());
                    }

                    calServiceDate.SelectedDate = System.DateTime.Now.Date;
                    chkselectVehicle.Attributes.Add("onclick", "SetEnableCalendar(this,'" + calServiceDate.ClientID + "')");
                    int operationTypeID = int.Parse(drv["OperationTypeID"].ToString());
                    if (operationTypeID == 1 || operationTypeID == 2)
                    {
                        if (drv["DueValueBasedOnCurrent"] != DBNull.Value)
                        {
                            txtValue.Text = drv["DueValueBasedOnCurrent"].ToString();
                            hidOrgValue.Value = txtValue.Text;
                        }
                        int minValue = 0;
                        if (operationTypeID == 1)
                        {
                            if (drv["Odometer"] != DBNull.Value) minValue = (int)float.Parse(drv["Odometer"].ToString());
                        }
                        else
                        {
                            if (drv["EngineHrs"] != DBNull.Value) minValue = (int)float.Parse(drv["EngineHrs"].ToString());
                        }

                        txtValue.MinValue = minValue;
                        txtValue.Visible = true;
                        calValue.Visible = false;
                    }
                    if (operationTypeID == 3)
                    {

                        string fixedServiceDate =string.Empty;
                        Boolean fixedDate = false;
                        int timespanId = 0;
                        string dueValue = string.Empty;
                        if (drv["TimespanId"] != DBNull.Value) int.TryParse( drv["TimespanId"].ToString(), out timespanId);
                        if (drv["FixedServiceDate"] != DBNull.Value) fixedServiceDate = drv["FixedServiceDate"].ToString();
                        if (drv["FixedDate"] != DBNull.Value && drv["FixedDate"].ToString().ToLower() == "true") fixedDate = true;
                        if (drv["DueValue"] != DBNull.Value) dueValue = drv["DueValue"].ToString();
                        if (fixedDate)
                        {
                            try
                            {
                                string[] str = dueValue.Split('/');
                                DateTime dtDue = new DateTime(int.Parse(str[2]), int.Parse(str[0]), int.Parse(str[1]));
                                if (timespanId == 4) dtDue = dtDue.AddMonths(1);
                                if (timespanId == 5) dtDue = dtDue.AddMonths(2);
                                if (timespanId == 6) dtDue = dtDue.AddMonths(3);
                                if (timespanId == 7) dtDue = dtDue.AddMonths(6);
                                if (timespanId == 8) dtDue = dtDue.AddMonths(12);
                                calValue.SelectedDate = dtDue;
                                calValue.MinDate = new System.DateTime(1970, 1, 1);
                                if (drv["DueValueBasedOnCurrent"] != DBNull.Value)
                                {
                                    hidOrgValue.Value = drv["DueValueBasedOnCurrent"].ToString();
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            if (drv["DueValueBasedOnCurrent"] != DBNull.Value)
                            {
                                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                                //to be changed
                                string[] str = drv["DueValueBasedOnCurrent"].ToString().Split('/');
                                calValue.SelectedDate = new DateTime(int.Parse(str[2]), int.Parse(str[0]), int.Parse(str[1]));
                                calValue.MinDate = new System.DateTime(1970, 1, 1);
                                hidOrgValue.Value = drv["DueValueBasedOnCurrent"].ToString();
                            }
                        }
                        txtValue.Visible = false;
                        calValue.Visible = true;
                        txtCurrentOdoHrs.Visible = false;
                    }

                    if (drv["DueValueBasedOnInterval"] != DBNull.Value)
                    {
                        float intevalVal = 0;
                        if (drv["Interval"] != DBNull.Value)
                        {
                            float.TryParse(drv["Interval"].ToString(), out intevalVal);
                        }

                        if (operationTypeID == 1 || operationTypeID == 2)
                        {
                            lnkUseCurrentValue.OnClientClick = string.Format("return SetNumControlData('{0}', '{1}', '{2}', {3});", txtValue.ClientID, GetScriptEscapeString(drv["DueValueBasedOnInterval"].ToString()), txtCurrentOdoHrs.ClientID, intevalVal);
                        }
                        else
                        {

                            lnkUseCurrentValue.OnClientClick = string.Format(
                                "return SetCalendarDate('{0}', '{1}', '{2}', {3})", calValue.ClientID, GetScriptEscapeString(drv["DueValueBasedOnInterval"].ToString()), calServiceDate.ClientID, intevalVal);

                        }
                    }

                }
                Panel pnlEmpty = (Panel)e.Item.FindControl("pnlEmpty");
                if (txtCurrentOdoHrs != null && pnlEmpty != null)
                    pnlEmpty.Visible = !txtCurrentOdoHrs.Visible;

            }
        }

        protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            if (e.Argument.ToLower() == "saveselectedvalues")
            {
                if (!ValidateServices()) return;
                SaveSelectedValues();
                BinddgMaintenance(true);
                return;
            }

        }

        private Boolean ValidateServices()
        {
            Boolean isValidate = true;
            ArrayList errors = new ArrayList();
            foreach (GridDataItem item in dgMaintenance.Items)
            {
                RadNumericTextBox txtValue = (RadNumericTextBox)item.FindControl("txtValue");
                RadDatePicker calValue = (RadDatePicker)item.FindControl("calValue");
                CheckBox chkselectVehicle = (CheckBox)item.FindControl("chkselectVehicle");
                RadDatePicker calServiceDate = (RadDatePicker)item.FindControl("calServiceDate");
                RadNumericTextBox txtCurrentOdoHrs = (RadNumericTextBox)item.FindControl("txtCurrentOdoHrs");
                if (txtValue != null && calValue != null && chkselectVehicle != null && chkselectVehicle.Checked)
                {
                    int serviceValue = 0;
                    if (txtCurrentOdoHrs.Visible)
                    {
                        if (txtCurrentOdoHrs.Value.HasValue) serviceValue = (int)txtCurrentOdoHrs.Value;
                    }
                    else
                    {
                        serviceValue = (int)calServiceDate.SelectedDate.Value.Date.Subtract(new DateTime(1970, 1, 1)).TotalDays;
                    }


                    int? value = null;
                    if (txtValue.Visible && txtValue.Value.HasValue) value = (int)txtValue.Value;

                    if (calValue.Visible && calValue.SelectedDate.HasValue) value = (int)calValue.SelectedDate.Value.Date.Subtract(new DateTime(1970, 1, 1)).TotalDays;

                    if (value != null)
                    {
                        string vehicleDesc = ((GridTableCell)item["VehicleDescription"]).Text;
                        string descriptionCell = ((GridTableCell)item["Description"]).Text;
                        if (serviceValue >= value.Value) errors.Add(vehicleDesc + " [" + descriptionCell + "]");
                    }
                }

            }
            if (errors.Count > 0)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                string errorstr = js.Serialize(errors);
                string script = string.Format("ShowErrorMessageWithTitle('{0}', '{1}')", errorstr, updateError);
                RadAjaxManager1.ResponseScripts.Add(script);
                isValidate = false;
            }
            return isValidate;
        }
        private void SaveSelectedValues()
        {

            ArrayList errorList = new ArrayList();
            try
            {

                foreach (GridDataItem item in dgMaintenance.Items)
                {
                    RadNumericTextBox txtValue = (RadNumericTextBox)item.FindControl("txtValue");
                    RadDatePicker calValue = (RadDatePicker)item.FindControl("calValue");
                    CheckBox chkselectVehicle = (CheckBox)item.FindControl("chkselectVehicle");
                    RadDatePicker calServiceDate = (RadDatePicker)item.FindControl("calServiceDate");
                    RadNumericTextBox txtCurrentOdoHrs = (RadNumericTextBox)item.FindControl("txtCurrentOdoHrs");
                    if (txtValue != null && calValue != null && chkselectVehicle != null && chkselectVehicle.Checked)
                    {
                        int serviceValue = 0;
                        if (txtCurrentOdoHrs.Visible )
                        {
                            if (txtCurrentOdoHrs.Value.HasValue) serviceValue = (int)txtCurrentOdoHrs.Value;
                        }
                        else
                        {
                            serviceValue = (int)calServiceDate.SelectedDate.Value.Date.Subtract(new DateTime(1970, 1, 1)).TotalDays;
                        }


                        int? value = null;
                        if (txtValue.Visible && txtValue.Value.HasValue) value = (int)txtValue.Value;

                        if (calValue.Visible && calValue.SelectedDate.HasValue) value = (int)calValue.SelectedDate.Value.Date.Subtract(new DateTime(1970, 1, 1)).TotalDays;

                        if (value != null)
                        {
                            long vehicleId = long.Parse(item.GetDataKeyValue("VehicleId").ToString());
                            int maintenanceID = int.Parse(item.GetDataKeyValue("MaintenanceID").ToString());
                            Int64 MccId = Int64.Parse(item.GetDataKeyValue("MccId").ToString());
                            string vehicleDesc = ((GridTableCell)item["VehicleDescription"]).Text;
                            try
                            {
                                if (calServiceDate.SelectedDate.HasValue)
                                {
                                    MCCManager mccMgr = new MCCManager(sConnectionString);
                                    mccMgr.MaintenanceClose(sn.UserID, vehicleId, maintenanceID, serviceValue, calServiceDate.SelectedDate.Value, MccId, value.Value);
                                }
                            }
                            catch(Exception ex)
                            {
                                errorList.Add(vehicleDesc);
                            }
                            
                        }
                    }

                }
                
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

            if (errorList.Count > 0)
            {
                 JavaScriptSerializer js = new JavaScriptSerializer();
                string errorstr = js.Serialize(errorList);
                string script = string.Format("ShowErrorMessage('{0}')", errorstr);
                RadAjaxManager1.ResponseScripts.Add(script);
            }

        }

        [WebMethod]
        public static string UpdateComment(int MaintenanceID, long VehicleId, string Comments)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                Comments = System.Web.HttpUtility.UrlDecode(Comments);
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                MCCManager mccMg = new MCCManager(sConnectionString);
                mccMg.VehicleMaintenanceUpdateComment(MaintenanceID, VehicleId, Comments, sn.User.OrganizationId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: MccServiceAssigment_Add() Page:frmMaintenanceNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";
        }

        public string RoundServicePerc(object dr)
        {
            string ret = string.Empty;
            if (dr != DBNull.Value)
            {
                ret = Math.Round(double.Parse(dr.ToString()), 2).ToString();
            }
            return ret;
        }
    }
}