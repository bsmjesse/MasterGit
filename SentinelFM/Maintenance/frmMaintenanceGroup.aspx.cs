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
    public partial class Maintenance_frmMaintenanceGroup : SentinelFMBasePage
    {
        public string Error_Load = "Failed to load data."; //(string)base.GetLocalResourceObject("Error_Load");
        string errorSave = "Save failed."; //(string)base.GetLocalResourceObject("errorSave");
        string errorDelete = "Delete failed.";  //(string)base.GetLocalResourceObject("errorDelete");
        public string selectFleet = "Select a Fleet"; //(string)base.GetLocalResourceObject("selectFleet");
        string errorDeleteAssign = "Can not be deleted because it has been assigned to vehicle(s).";  //(string)base.GetLocalResourceObject("errorDeleteAssign");
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        public string showFilter = "Show Filter"; //(string)base.GetLocalResourceObject("showFilter");
        public string hideFilter = "Hide Filter"; //(string)base.GetLocalResourceObject("hideFilter");
        public string sameNameError = "A group with the same name has existed in database. Please enter another name."; //(string)base.GetLocalResourceObject("sameNameError");
        public string deleteGroup = "Delete this PM Group?";  //(string)base.GetLocalResourceObject("deleteGroup");

        protected void Page_Load(object sender, EventArgs e)
        {

              Error_Load = (string)base.GetLocalResourceObject("Error_Load");
         errorSave = (string)base.GetLocalResourceObject("errorSave");
         errorDelete = (string)base.GetLocalResourceObject("errorDelete");
         selectFleet = (string)base.GetLocalResourceObject("selectFleet");
         errorDeleteAssign = (string)base.GetLocalResourceObject("errorDeleteAssign");
         showFilter = (string)base.GetLocalResourceObject("showFilter");
         hideFilter = (string)base.GetLocalResourceObject("hideFilter");
         sameNameError = (string)base.GetLocalResourceObject("sameNameError");
         deleteGroup = (string)base.GetLocalResourceObject("deleteGroup");

            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
            }

            if (!IsPostBack)
            {
                this.Master.CreateMaintenenceMenu(null);
            }
            this.Master.resizeScript = SetResizeScript(gdMCC.ClientID);
            this.Master.isHideScroll = true;
            gdMCC.RadAjaxManagerControl = RadAjaxManager1;
        }

        protected void gdMCC_OnInsertCommand(object source, GridCommandEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridEditFormInsertItem)
            {
                TextBox txtMccName = (TextBox)e.Item.FindControl("txtMccName");
                if (!txtMccName.Equals(null))
                {
                    string mccName = txtMccName.Text.Trim();

                    try
                    {
                        MCCManager mccMg = new MCCManager(sConnectionString);
                        DataSet ds =  mccMg.GetMccGroupByName(mccName, sn.User.OrganizationId, null);
                        if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            e.Canceled = true;
                            string errorMsg = string.Format("alert(\"{0}\");", sameNameError);
                            RadAjaxManager1.ResponseScripts.Add(errorMsg);

                            return;
                        }
                        else mccMg.AddMCCGroup(sn.User.OrganizationId, mccName);
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


        private void BindMCC(bool isBind)
        {
            MCCManager mccMg = new MCCManager(sConnectionString);
            gdMCC.DataSource = mccMg.GetOrganizationMCCGroup(sn.User.OrganizationId);
            if (isBind)
            {
                gdMCC.DataBind();
            }
        }

        protected void gdMCC_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
                BindMCC(false);
        }

        protected void gdMCC_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            if (!(e.Item is Telerik.Web.UI.GridEditFormItem)) return;
            if (((GridEditFormItem)e.Item).GetDataKeyValue("MccId") != null)
            {
                TextBox txtMccName = (TextBox)e.Item.FindControl("txtMccName");
                if (!txtMccName.Equals(null))
                {
                    string mccName = txtMccName.Text.Trim();
                    try
                    {
                        long mccId = long.Parse(((GridEditFormItem)e.Item).GetDataKeyValue("MccId").ToString());
                        MCCManager mccMg = new MCCManager(sConnectionString);

                        DataSet ds = mccMg.GetMccGroupByName(mccName, sn.User.OrganizationId, mccId);
                        if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            e.Canceled = true;
                            string errorMsg = string.Format("alert(\"{0}\");", sameNameError);
                            RadAjaxManager1.ResponseScripts.Add(errorMsg);

                            return;
                        }


                        mccMg.UpdateMCCGroup(sn.User.OrganizationId, mccName, mccId);
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

        protected void gdMCC_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            if (((GridDataItem)e.Item).GetDataKeyValue("MccId") != null)
            {
                try
                {
                    long mccId = long.Parse(((GridDataItem)e.Item).GetDataKeyValue("MccId").ToString());
                    MCCManager mccMg = new MCCManager(sConnectionString);
                    int ret = mccMg.DeleteMCCGroup(mccId, sn.User.OrganizationId, sn.UserID);
                    if (ret == -1)
                        RadAjaxManager1.ResponseScripts.Add(string.Format("alert(\"{0}\");", errorDeleteAssign));
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
        protected void gdMCC_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "InitInsert" || e.CommandName == "Edit" || e.CommandName == "Delete")
            {
                if (e.CommandName == "Edit") gdMCC.MasterTableView.IsItemInserted = false;
                gdMCC.MasterTableView.ClearEditItems();
            }

        }

        protected void gdMCC_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.DataItem != null && e.Item.DataItem is DataRowView && e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                GridDataItem item = (GridDataItem)e.Item;
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
                
                ImageButton deleteColumn = (ImageButton)item["DeleteColumn"].Controls[0];
                Button btnDelete = (Button)item.FindControl("btnDelete");
                deleteColumn.Attributes.Add("OnClick", "return ClickDeleteEvent(" + drv["MccId"].ToString() + ",'" + btnDelete.ClientID + "');"); 
 
            }
        }

        [WebMethod]
        public static string GetVehiclesandGroup(int MccId)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                MCCManager mccMg = new MCCManager(sConnectionString);
                DataSet ds = mccMg.MaintenanceGetVehiclesByMccId(MccId);
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
    }
}