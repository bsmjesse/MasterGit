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
    public partial class Maintenance_frmMaintenanceGroupAssign : SentinelFMBasePage
    {
        string errorSave = "Save failed.";
        DataTable mccAssignmentDt = null;
        public string selectFleet = "Select a Fleet";
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        public string strUnitOfMes = "";
        public string showFilter = "Show Filter";
        public string hideFilter = "Hide Filter";
        public static string notification1Txt = "Notification1: ";
        public static string notification2Txt = "Notification2: ";
        public static string notification3Txt = "Notification3: ";
        public string Error_Load = "Failed to load data.";
        public string selectGroup = "Please select PM Group";
        public string selectMaintenance = "Please select PM Service";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
            }

            if (!IsPostBack)
            {
                this.Master.CreateMaintenenceMenu(null);
            }

            dgMCCAssignment.RadAjaxManagerControl = RadAjaxManager1;

            this.Master.resizeScript = SetResizeScript(dgMCCAssignment.ClientID);
            dgMCCAssignment.SubGridID = "dgServiceAssignment";
            this.Master.isHideScroll = true;

            Literal lit = new Literal();

            lit.Text = "<link href='../Scripts/css/tooltip.css' type='text/css' rel='stylesheet'></link> ";
            this.Page.Header.Controls.Add(lit);

        }

        private void BindMCCAssignment(bool isBind)
        {
            try
            {
                MCCManager mccMg = new MCCManager(sConnectionString);
                mccAssignmentDt = mccMg.GetMCCMaintenanceAssigment(sn.User.OrganizationId, null, GetOperationTypeStr(), sn.UserID).Tables[0];
                dgMCCAssignment.DataSource = mccAssignmentDt.DefaultView.ToTable(true, "MccId", "MccName", "OrganizationID");
                if (isBind)
                {
                    dgMCCAssignment.DataBind();
                }
            }
            catch (Exception Ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("MccId");

                dgMCCAssignment.DataSource = dt;
                if (isBind) dgMCCAssignment.DataBind();

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                string errorMsg = string.Format("alert(\"{0}\");", Error_Load);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);

            }
        }

        protected void dgMCCAssignment_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindMCCAssignment(false);
        }

        protected void dgMCCAssignment_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.DataItem != null && e.Item.DataItem is DataRowView && e.Item is GridDataItem)
            {
                RadGrid dgServiceAssignment = (RadGrid)e.Item.FindControl("dgServiceAssignment");
                if (dgServiceAssignment != null)
                {
                    string strExpr = "MccId=" + ((GridDataItem)e.Item).GetDataKeyValue("MccId").ToString();
                    DataRow[] drs = mccAssignmentDt.Select(strExpr);
                    if (drs.Length == 0)
                    {
                        DataTable dt = mccAssignmentDt.Clone();
                        dt.Rows.Add(dt.NewRow());
                        dgServiceAssignment.DataSource = dt;
                    }
                    else dgServiceAssignment.DataSource = drs;

                    dgServiceAssignment.DataBind();
                }

            }
        }
        private string GetOperationTypeStr()
        {
            return clsAsynGenerateReport.GetOperationTypeStr();
        }

        protected void dgServiceAssignment_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.DataItem != null && e.Item.DataItem is DataRowView && e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                //NotificationTooptip(item);
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
                Control lnk = item["NotificationType"].Controls[0];
                if (lnk is HyperLink)
                {
                    ((HyperLink)lnk).Attributes.Add("onclick", "ShowToolTipScreen(135, '" + notification + "', this)");
                }
            }
        }


        protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
        }

        protected void dgMCCAssignment_InsertCommand(object sender, GridCommandEventArgs e)
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
                        mccMg.AddMCCGroup(sn.User.OrganizationId, mccName);
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

        protected void dgMCCAssignment_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Modify")
            {
                if (e.Item is GridDataItem)
                {
                    Label  lblMccName = (Label)e.Item.FindControl("lblMccName");
                    string desc="";
                    if (lblMccName!= null) desc = Server.UrlEncode(lblMccName.Text);

                    Response.Redirect("frmMaintenanceGroupAssignAdd.aspx?m=" + ((GridDataItem)e.Item).GetDataKeyValue("MccId").ToString() + "&n=" + desc);
                }
            }
        }

    }
}