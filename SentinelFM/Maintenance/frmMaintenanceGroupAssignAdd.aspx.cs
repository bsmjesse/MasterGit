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

    public partial class Maintenance_frmMaintenanceGroupAssignAdd : SentinelFMBasePage
    {
        public string errorLoad = "Failed to load data.";
        public string errorSave = "Save Failed.";
        public string errorDelete = "Delete Failed.";
        public string selectService = "Plase select a service.";

        public string notiTxt = "Notification Type: ";
        public string n1Txt = "Notification1: ";
        public string n2Txt = "Notification2: ";
        public string n3Txt = "Notification3: ";
        public string freqTxt = "Frequency ID:";
        public string inteTxt = "inteTxt";
        public string assignText = "Assign";
        public string unAssignText = "Unassign";
        public string showFilter = "Show Filter";
        public string hideFilter = "Hide Filter";
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        string operationStr = null;
        public long mccID = -1;
        List<string> maintenanceDue = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["m"] != null)
                mccID = long.Parse(Request.QueryString["m"]);
            if (Request.QueryString["n"] != null)
                lblMccNameText.Text = Server.UrlDecode(Request.QueryString["n"]);
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
            }

            if (!IsPostBack)
            {
                this.Master.CreateMaintenenceMenu("M503");
            }
            try
            {
                errorLoad = HttpContext.GetGlobalResourceObject("Const", "Error_Load").ToString();
                errorSave = HttpContext.GetGlobalResourceObject("Const", "Error_Save").ToString();
                errorDelete = HttpContext.GetGlobalResourceObject("Const", "Error_Delete").ToString();
            }
            catch { }

            //gvServicesSource.RadAjaxManagerControl = RadAjaxManager1;
            //gvServicesDest.RadAjaxManagerControl = RadAjaxManager1;
            this.Master.isHideScroll = true;
            this.Master.resizeScript = GetResizeScript();

            AjaxSetting ajx = new AjaxSetting();
            ajx.AjaxControlID = "RadAjaxManager1";
            AjaxUpdatedControl ajxUp = new AjaxUpdatedControl();
            ajxUp.ControlID = ((RadGrid)RadWindowManager1.FindControl("RadWindowContentTemplate_Dt").Controls[0].FindControl("ListMessage_Dt").FindControl("gdVehicle_Dt")).ID;
            ajx.UpdatedControls.Add(ajxUp);
            RadAjaxManager1.AjaxSettings.Add(ajx);
        }


        public string GetResizeScript()
        {
            return SetResizeScript(gvServicesDest.ClientID) + SetResizeScript(gvServicesSource.ClientID);
        }

        public void BindgvServicesSource(Boolean isBind)
        {
            hidTimeBaseMaintenance.Value = "";
                MCCManager mccMg = new MCCManager(sConnectionString);
                if (string.IsNullOrEmpty(operationStr)) operationStr = clsAsynGenerateReport.GetOperationTypeStr();
                DataTable dt = mccMg.GetMCCMaintenanceUnAssigment(sn.User.OrganizationId, mccID, operationStr,sn.UserID).Tables[0];
                gvServicesSource.DataSource = dt;
                if (isBind) gvServicesSource.DataBind();
        }

        public void BindgvServicesDest(Boolean isBind)
        {
                MCCManager mccMg = new MCCManager(sConnectionString);
                if (string.IsNullOrEmpty(operationStr)) operationStr = clsAsynGenerateReport.GetOperationTypeStr();
                DataTable dt = mccMg.GetMCCMaintenanceAssigment(sn.User.OrganizationId, mccID, operationStr, sn.UserID).Tables[0];
                gvServicesDest.DataSource = dt;
                if (isBind) gvServicesDest.DataBind();
        }

        protected void gvServicesSource_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
             BindgvServicesSource(false);
        }
        protected void gvServicesDest_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
             BindgvServicesDest(false);
        }
        protected void gvServicesSource_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Header)
            {
                Button btn = (Button)e.Item.FindControl("btnAssignAll");
                Button btnTmp = (Button)e.Item.FindControl("btnAssignAllTmp");
                if (btn != null) btn.Attributes.Add("onclick", "return AssignAllServices('" + btnTmp.ClientID + "');");
            }
            if (e.Item.DataItem != null && e.Item.DataItem is DataRowView && e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                int isTimeBase = 0;
                if (drv["OperationTypeID"] != DBNull.Value)
                {
                    if (drv["OperationTypeID"].ToString() == "3")
                    {
                        if (drv["FixedDate"] == DBNull.Value || drv["FixedDate"].ToString().ToLower() != "true")
                        {
                            isTimeBase = 1;
                            hidTimeBaseMaintenance.Value = "1";
                        }
                    }
                }
                Button btn = (Button)e.Item.FindControl("btnAssign");
                Button btnTmp = (Button)e.Item.FindControl("btnAssignTmp");
                if (btn != null) btn.Attributes.Add("onclick", "return AssignServices(" + drv["MaintenanceId"].ToString() + ",'" + btnTmp.ClientID + "'," + isTimeBase.ToString() + ");");
            }

        }
        protected void gvServicesDest_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Header)
            {
                Button btn = (Button)e.Item.FindControl("btnunAssignAll");
                Button btnTmp = (Button)e.Item.FindControl("btnunAssignAllTmp");
                if (btn != null) btn.Attributes.Add("onclick", "return UnAssignAllServices('" + btnTmp.ClientID + "');");
            }
            if (e.Item.DataItem != null && e.Item.DataItem is DataRowView && e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;

                Button btn = (Button)e.Item.FindControl("btnunAssign");
                Button btnTmp = (Button)e.Item.FindControl("btnunAssignTmp");
                if (btn != null) btn.Attributes.Add("onclick", "return UnAssignServices(" + drv["MaintenanceId"].ToString() + ",'" + btnTmp.ClientID + "');");
            }


        }

        protected void RadGrid_PreRender(object sender, EventArgs e)
        {
            RadGrid rgGrid = (RadGrid)sender;

            //if (!IsPostBack)
            //{
            string hidStr = "_hid_" + rgGrid.ClientID + "_1";
            if (Request[hidStr] == null)
            {
                ScriptManager.RegisterHiddenField(this.Page, hidStr, "");
            }
            //}

            GridItem[] gridItems = rgGrid.MasterTableView.GetItems(GridItemType.CommandItem);
            if (gridItems != null && gridItems.Length == 0)
            {
                GenerateGridCreated(rgGrid, "");
                return;
            }

            int itemLength = gridItems.Length;
            if (gridItems[itemLength - 1].FindControl("tblCustomerCommand") != null)
            {
                HtmlTable tblCustomerCommand = (HtmlTable)gridItems[itemLength - 1].FindControl("tblCustomerCommand");
                HtmlTableCell tbl = new HtmlTableCell();
                tbl.ID = "customerCommand";
                ImageButton imgFilter = new ImageButton();
                imgFilter.ID = "imgFilter";
                imgFilter.ImageUrl = "~/images/filter.gif";

                LinkButton hplFilter = new LinkButton();
                hplFilter.ID = "hplFilter_" + rgGrid.ClientID ; //new 
                //hplFilter.Text = "Show Filter";
                hplFilter.Font.Underline = true;
                string scriptPara = string.Format("javascript:return showFilterItemScript('{0}','{1}','{2}','{3}','{4}')",
                          hidStr,
                          rgGrid.ClientID,
                          hplFilter.ClientID,
                          hideFilter,
                          showFilter
                );
                Label lbl = new Label();
                lbl.Text = "&nbsp;";
                imgFilter.OnClientClick = scriptPara;
                hplFilter.OnClientClick = scriptPara;
                tbl.Align = "right";
                tbl.Controls.Add(imgFilter);
                tbl.Controls.Add(hplFilter);
                tbl.Controls.Add(lbl);
                tblCustomerCommand.Rows[0].Cells.Add(tbl);

                scriptPara = string.Format("SetFilterWhenCreatedScript('{0}','{1}','{2}','{3}','{4}');",
                          hidStr,
                          rgGrid.ClientID,
                          hplFilter.ClientID,
                          hideFilter,
                          showFilter
                );

                GenerateGridCreated(rgGrid, scriptPara);
            }
        }
        //GridCreated
        private void GenerateGridCreated(RadGrid rgGrid, string scriptPara)
        {
            string name = "_gridCreate_" + rgGrid.ClientID + "_1";

            string createScript = "function " + name + "(sender, eventArgs) {" + scriptPara ;
            if (rgGrid.ClientSettings.Scrolling.AllowScroll)
            {
                string sscript = "";
                string columnResizescript = "";
                    sscript = "_gridWidth = $telerik.$(window).width() - $telerik.$('#' + _gridID).offset().left - 10;" +
                              "_realWidth =    $telerik.$(window).width() - $telerik.$('#' + _gridID).offset().left - 10;" +
                              "_realWidth = _realWidth.toString() + 'px';" +
                              "var _gridHeight = ($telerik.$(window).height() - $telerik.$('#" + gvServicesSource.ClientID + "').offset().top )/2 - 10;" +
                              //"var _gridHeight = $telerik.$(window).height()/" + 2 + "  - $telerik.$('#' + _gridID).offset().top - 10;" +
                              "var _gridheaderHeight = _gridElement.find(\".rgHeaderDiv\").height();" +
                              "var _gridfootHeight = _gridElement.find(\".rgPager\").height();" +
                              // "_gridElement.find(\".rgDataDiv\").height(_gridHeight - _gridheaderHeight );";
                              "_gridElement.find(\".rgDataDiv\").height(_gridHeight - _gridheaderHeight - _gridfootHeight - 20);";

                if (rgGrid.ClientSettings.Resizing.AllowColumnResize)
                {
                    string columnFuncName = "_columnResized_" + rgGrid.ClientID + "_1";
                    rgGrid.ClientSettings.ClientEvents.OnColumnResized = columnFuncName;
                    string resizeNameStr = string.Format("{0}($find('{1}'), null);", "_gridCreate_" + rgGrid.ClientID + "_1", rgGrid.ClientID);
                    columnResizescript = "function " + columnFuncName +
                                         "(sender, eventArgs){" + 
                                         resizeNameStr + 
                                         "}";
                    
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), columnFuncName, columnResizescript, true);
                }
                string subGridStr = "";

                createScript = createScript +
            "var _gridID =  sender.get_id() ; " +
            "var _gridWidth = $telerik.$(sender.get_element()).width();" +
            "var _gridElement = $telerik.$(sender.get_element());" +
            "var _realWidth = $telerik.$('#' + _gridID).css('width').toLowerCase();" +
            "var _diffWidth = 0; " + sscript +
            "if (_realWidth.indexOf('%') < 0) {" +
                "_realWidth.replace(\"px\", \"\"); " +
                "try {" +
                    "_diffWidth = parseInt(_realWidth) - _gridWidth;" +
                "}" +
                "catch (err) { }" +
            "}" +
            "else {" +
                "_realWidth = _gridWidth;" +
                "_gridWidth = _gridWidth - 2;" +
                "_diffWidth = _realWidth - _gridWidth;" +
                "var masterTable = $find(_gridID).get_masterTableView();" +
                "$telerik.$(masterTable).width(_realWidth -20 );" +
            "}" +

            "_gridElement.find(\".rgFilterBox[type='text']\").each(function () {" +
                "if ($telerik.$(this).css(\"visibility\") != \"hidden\") {" +
                    "var buttonWidth = 0;" +
                    "if ($telerik.$(this).next(\"[type='submit']\").length > 0) {" +
                        "buttonWidth = $telerik.$(this).next(\"[type='submit']\").width();" +
                    "}" +
                    "if ($telerik.$(this).next(\"[type='button']\").length > 0) {" +
                        "buttonWidth = $telerik.$(this).next(\"[type='button']\").width();" +
                    "}" +

                    "if (buttonWidth > 0) {" +
                        "$telerik.$(this).width($telerik.$(this).parent().width() - buttonWidth - 5);" +
                    "}" +
                    "else {" +
                        "$telerik.$(this).width($telerik.$(this).parent().width() - 40);" +
                    "}" +
                "}" +
            "});" +


            "if (_gridElement.find(\".rgDataDiv\")[0].scrollHeight > _gridElement.find(\".rgDataDiv\")[0].clientHeight)" +  //check if scroll bar exists " 
            "{" +
                "var scrollWidth = 20 - _diffWidth;" +
                "_gridElement.find(\"table[id^='\" + _gridID + \"']\").each(function () {" +
                    "var id = $telerik.$(this).attr(\"id\").toLowerCase();" +
                    subGridStr + 
                    "if (id.substring(id.length - 5, id.length) != \"pager\") { " +
                         "if (id.indexOf('_calendar') <= 0) {" + 
                            "$telerik.$(this).width(_gridWidth - scrollWidth);}" +
                    "}" +
                    "else" + 
                    "{" +
                        "$telerik.$(this).width(_gridWidth - 3);" +
                     "}" + 
                "});" +
                "_gridElement.find(\".rgHeaderDiv\").width(_gridWidth - scrollWidth);" +
            "}" +
            "else {" +
                "_gridElement.find(\"table[id^='\" + _gridID + \"']\").each(function (){ " + 
                   "var id = $telerik.$(this).attr(\"id\").toLowerCase();" +
                   subGridStr + 
                    "if (id.indexOf('_calendar') <= 0) {" +
                        "$telerik.$(this).width(_gridWidth -3);}" +
                "});" +
                "_gridElement.find(\".rgDataDiv\").find(\"tr:last\").find(\"td\").css(\"border-bottom\", \"1px solid #e4e4e4\");" +
            "}" + 
            "_gridElement.width(_gridWidth);";
            }

            string gridCreateFilterMenu = "";


            createScript = createScript +  gridCreateFilterMenu + "}";

            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), name, createScript, true);
            rgGrid.ClientSettings.ClientEvents.OnGridCreated = name;

        }

        private DateTime FindStartDateByID(string vehicleID)
        {
            DateTime reDate = System.DateTime.Now.Date;
            RadGrid gdVehicle_1 = ((RadGrid)RadWindowManager1.FindControl("RadWindowContentTemplate_Dt").Controls[0].FindControl("ListMessage_Dt").FindControl("gdVehicle_Dt"));
            foreach(GridDataItem gItem in gdVehicle_1.Items) 
            {
                if (gItem.GetDataKeyValue("VehicleId").ToString() == vehicleID)
                {
                    RadDatePicker calValue = (RadDatePicker)gItem.FindControl("calValue");
                    if (calValue != null)
                    {
                        if (calValue.SelectedDate.HasValue) reDate = calValue.SelectedDate.Value.Date;
                    }
                    break;
                }
            }
            return reDate;
        }

        private string GetVehicleDue(DateTime dtOrg, string vehicleID)
        {
            dtOrg = FindStartDateByID(vehicleID);
            StringBuilder dues = new StringBuilder();
            foreach (string mdue in maintenanceDue)
            {
                if (mdue.IndexOf("-") > 0)
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
                else
                {
                    DateTime dt = dtOrg;
                    string[] mdues = mdue.Split('#');
                    dt = dt.AddDays(int.Parse(mdues[1]));

                    if (dues.Length > 0) dues.Append("#");
                    dues.Append(string.Format("{0}#{1}", mdues[0], dt.Subtract(new DateTime(1970, 1, 1)).TotalDays));
                }
            }

            return vehicleID + "#" + dues.ToString();
        }

        protected void gvServicesSource_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "assignall" && mccID != -1)
            {
                try
                {
                    string maintenanceIds = string.Empty;
                    Boolean isTimeBase = false;
                    StringBuilder strBuilder = new StringBuilder();
                    foreach (GridDataItem item in gvServicesSource.Items)
                    {
                        if (item.GetDataKeyValue("MaintenanceId") != null)
                        {
                            if (maintenanceIds == string.Empty) maintenanceIds = item.GetDataKeyValue("MaintenanceId").ToString();
                            else maintenanceIds = maintenanceIds + "," + item.GetDataKeyValue("MaintenanceId").ToString();

                            if (item.FindControl("hidOperationType") != null)
                            {
                                if (((HiddenField)item.FindControl("hidOperationType")).Value == "3")
                                {
                                    HiddenField hidInterval = (HiddenField)item.FindControl("hidInterval");
                                    HiddenField hidFixedDate = (HiddenField)item.FindControl("hidFixedDate");
                                    if (hidFixedDate != null && hidFixedDate.Value.ToLower() == "true")
                                    {
                                        HiddenField hidTimespanId = (HiddenField)item.FindControl("hidTimespanId");
                                        string timespanId = string.Empty;
                                        if (hidTimespanId != null) timespanId = hidTimespanId.Value;
                                        maintenanceDue.Add(item.GetDataKeyValue("MaintenanceId").ToString() + "#" + timespanId + "-" + item["FixedDate"].Text);
                                        isTimeBase = true;
                                    }
                                    else
                                    {
                                        if (hidInterval != null)
                                        {
                                            maintenanceDue.Add(item.GetDataKeyValue("MaintenanceId").ToString() + "#" + hidInterval.Value);
                                            isTimeBase = true;
                                        }
                                    }
                                 }
                            }
                        }
                    }
                    MCCManager mccMg = new MCCManager(sConnectionString);

                    DataSet ds = mccMg.MaintenanceGetUnAssignedVehiclesByMccIDAndMaintenanceId(mccID, maintenanceIds);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach(DataRow dr in ds.Tables[0].Rows)
                        {
                            string vehicleIDs = string.Empty;
                            if (isTimeBase)
                                vehicleIDs = GetVehicleDue(System.DateTime.Now.Date, dr["VehicleId"].ToString());
                            else vehicleIDs = dr["VehicleId"].ToString();
                            if (strBuilder.Length == 0) strBuilder.Append(vehicleIDs);
                            else strBuilder.Append("," + vehicleIDs);
                        }
                    }


                    mccMg.MCCMaintenanceAssigment_Add(mccID, maintenanceIds, sn.User.OrganizationId, strBuilder.ToString());
                    BindgvServicesDest(true);
                    BindgvServicesSource(true);
                }
                catch(Exception Ex)
                {
                    BindgvServicesDest(true);
                    BindgvServicesSource(true);
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    string msg = string.Format("alert('{0}');", errorSave);
                    RadAjaxManager1.ResponseScripts.Add(msg);
                }

            }
            if (e.CommandName.ToLower() == "assign" && e.Item is GridDataItem && mccID != -1)
            {
                try
                {
                    string maintenanceIds = ((GridDataItem)e.Item).GetDataKeyValue("MaintenanceId").ToString();
                    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                    StringBuilder strBuilder = new StringBuilder();
                    int interval = -1;
                    string  operationType = "";

                    if (e.Item.FindControl("hidOperationType") != null)
                    {
                        operationType = ((HiddenField)e.Item.FindControl("hidOperationType")).Value;
                        HiddenField hidInterval = (HiddenField)e.Item.FindControl("hidInterval");
                        if (hidInterval != null)
                        {
                            interval = int.Parse(hidInterval.Value);
                        }
                    }
                    maintenanceDue.Clear();
                    Boolean isFixedDate = false;
                    HiddenField hidFixDate = (HiddenField)e.Item.FindControl("hidFixedDate");
                    HiddenField hidTimespanId = (HiddenField)e.Item.FindControl("hidTimespanId");
                    if (hidFixDate != null && hidFixDate.Value.ToLower() == "true" && hidTimespanId != null)
                    {
                        maintenanceDue.Add(maintenanceIds + "#" + hidTimespanId.Value + "-" + e.Item.Cells[gvServicesSource.Columns.FindByUniqueName("FixedDate").OrderIndex].Text);
                        isFixedDate = true;
                    }
                    MCCManager mccMg = new MCCManager(sConnectionString);
                    DataSet ds = mccMg.MaintenanceGetUnAssignedVehiclesByMccIDAndMaintenanceId(mccID, maintenanceIds);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            string vehicleIDs = string.Empty;
                            if (operationType == "3")
                            {
                                if (isFixedDate)
                                {
                                    vehicleIDs = GetVehicleDue(System.DateTime.Now.Date, dr["VehicleId"].ToString());
                                }
                                else
                                {
                                    vehicleIDs = dr["VehicleId"].ToString() + "#" + string.Format("{0}#{1}", maintenanceIds, FindStartDateByID(dr["VehicleId"].ToString()).AddDays(interval).Subtract(new DateTime(1970, 1, 1)).TotalDays);
                                }
                            }
                            else
                                vehicleIDs = dr["VehicleId"].ToString();
                            if (strBuilder.Length == 0) strBuilder.Append(vehicleIDs);
                            else strBuilder.Append("," + vehicleIDs);
                        }
                    }
                    mccMg.MCCMaintenanceAssigment_Add(mccID, maintenanceIds, sn.User.OrganizationId, strBuilder.ToString());
                    BindgvServicesDest(true);
                    BindgvServicesSource(true);
                }
                catch (Exception Ex)
                {
                    BindgvServicesDest(true);
                    BindgvServicesSource(true);
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    string msg = string.Format("alert('{0}');", errorSave);
                    RadAjaxManager1.ResponseScripts.Add(msg);
                }

            }
        }

        protected void gvServicesDest_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "unassignall" && mccID != -1)
            {
                try
                {
                string maintenanceIds = string.Empty;
                foreach(GridDataItem item in  gvServicesDest.Items) 
                {
                    if (item.GetDataKeyValue("MaintenanceId") != null)
                    {
                        if (maintenanceIds == string.Empty ) maintenanceIds = item.GetDataKeyValue("MaintenanceId").ToString();
                        else maintenanceIds = maintenanceIds + "," + item.GetDataKeyValue("MaintenanceId").ToString();
                    }
                }
                MCCManager mccMg = new MCCManager(sConnectionString);
                mccMg.MCCMaintenanceAssigment_Delete(mccID, maintenanceIds, sn.User.OrganizationId, sn.UserID);
                BindgvServicesDest(true);
                BindgvServicesSource(true);
                }
                catch (Exception Ex)
                {
                    BindgvServicesDest(true);
                    BindgvServicesSource(true);
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    string msg = string.Format("alert('{0}');", errorDelete);
                    RadAjaxManager1.ResponseScripts.Add(msg);
                }

            }
            if (e.CommandName.ToLower() == "unassign" && e.Item is GridDataItem && mccID != -1)
            {
                try
                {
                    string maintenanceIds = ((GridDataItem)e.Item).GetDataKeyValue("MaintenanceId").ToString();
                    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                    MCCManager mccMg = new MCCManager(sConnectionString);
                    mccMg.MCCMaintenanceAssigment_Delete(mccID, maintenanceIds, sn.User.OrganizationId, sn.UserID);
                    BindgvServicesDest(true);
                    BindgvServicesSource(true);
                }
                catch (Exception Ex)
                {
                    BindgvServicesDest(true);
                    BindgvServicesSource(true);
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    string msg = string.Format("alert('{0}');", errorDelete);
                    RadAjaxManager1.ResponseScripts.Add(msg);
                }

            }
        }
        protected void btnClose_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmMaintenanceGroupAssign.aspx");
        }


        [WebMethod]
        public static string GetunAssignedVehicles(long MccId, string MaintenanceIds)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                MCCManager mccMg = new MCCManager(sConnectionString);
                DataSet ds = mccMg.MaintenanceGetUnAssignedVehiclesByMccIDAndMaintenanceId(MccId, MaintenanceIds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ArrayList vehicles = new ArrayList();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Dictionary<string, string> vehicle = new Dictionary<string, string>();
                        vehicle.Add("BoxId", dr["BoxId"] is DBNull ? string.Empty : dr["BoxId"].ToString());
                        vehicle.Add("Description", dr["Description"] is DBNull ? string.Empty : dr["Description"].ToString());
                        vehicle.Add("dtStart", System.DateTime.Now.Date.ToString("MM/dd/yyyy"));
                        vehicles.Add(vehicle);
                    }

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    js.MaxJsonLength = int.MaxValue;
                    return js.Serialize(vehicles);
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

        [WebMethod]
        public static string GetAssignedVehicles(long MccId, string MaintenanceIds)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                MCCManager mccMg = new MCCManager(sConnectionString);
                DataSet ds = mccMg.MaintenanceGetAssignedVehiclesByMccIDAndMaintenanceId(MccId, MaintenanceIds);
                if (ds.Tables[0].Rows.Count > 0)
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
                    return js.Serialize(vehicles);
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

        protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            DataSet ds;
            if (e.Argument == "-1")
            {
                string maintenances = string.Empty;
                foreach (GridDataItem item in gvServicesSource.Items)
                {
                    if (item.GetDataKeyValue("MaintenanceId") != null)
                    {
                        string key = item.GetDataKeyValue("MaintenanceId").ToString();
                        if (maintenances == string.Empty ) maintenances = key;
                        else maintenances = maintenances + "," + key;
                    }
                }
                MCCManager mccMg = new MCCManager(sConnectionString);
                ds = mccMg.MaintenanceGetUnAssignedVehiclesByMccIDAndMaintenanceId(mccID, maintenances);
            }
            else
            {
                MCCManager mccMg = new MCCManager(sConnectionString);
                ds = mccMg.MaintenanceGetUnAssignedVehiclesByMccIDAndMaintenanceId(mccID, e.Argument);
            }

            RadGrid gdVehicle_1 = ((RadGrid)RadWindowManager1.FindControl("RadWindowContentTemplate_Dt").Controls[0].FindControl("ListMessage_Dt").FindControl("gdVehicle_Dt"));
            if (gdVehicle_1 != null)
            {
                gdVehicle_1.DataSource = ds;
                gdVehicle_1.DataBind();

            }

         }
 }
}