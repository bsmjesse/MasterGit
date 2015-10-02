using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using VLF.ERRSecurity;
using VLF.Reports;
using System.Text;
using VLF.CLS;
using Telerik.Web.UI;
using System.Drawing;
using VLF.DAS.Logic;
using System.Collections.Generic;
using VLF.DAS.DB;
namespace SentinelFM
{
    public partial class Configuration_frmAssignmentList : SentinelFMBasePage
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        VehicleEquipmentAssignmentManager vehicleEquipmentAssignmentMg = null;
        string errorDelete = "Delete failed.";
        public string selectFleet = "Select a Fleet";
        public string showFilter = "Show Filter";
        public string hideFilter = "Hide Filter";
        protected void Page_Load(object sender, EventArgs e)
        {
            //check session, to be changed

            //End
            try
            {
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../../Login.aspx','_top')</SCRIPT>", false);
                }
                if (!IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    //if (sn.User.MenuColor != "")
                    //{
                    //    Literal css = new Literal();
                    //    css.Text = @"<style type='text/css'> " +
                    //               @".RadGridtblHeader" +
                    //               @"{" +
                    //               @"background-color:" + sn.User.MenuColor + "!important;" +
                    //               @"background-image:none !important;" +
                    //               @"}" +
                    //               "</style>";
                    //    this.Header.Controls.Add(css);
                    //}
                    FillFleets();
                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
            vehicleEquipmentAssignmentMg = new VehicleEquipmentAssignmentManager(sConnectionString);
        }

        private void FillFleets()
        {
            DataSet dsFleets = sn.User.GetUserFleets(sn);
            cboFleet.DataSource = dsFleets;
            cboFleet.DataBind();
            cboFleet.Items.Insert(0, new RadComboBoxItem(selectFleet, "-1"));
        }


        private DataTable GetVehicles()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("VehicleId");
            dt.Columns.Add("Description");
            DataSet ds = null;
            if (cboFleet.SelectedIndex > 0)
            {
                 ds = vehicleEquipmentAssignmentMg.GetVehiclesEquipmentAssignmentByFleetId(int.Parse(cboFleet.SelectedValue));
            }
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                
                //for (int index = 0; index < dt.Rows.Count - 1; index ++ )
                    //dt.Rows[index]["Description"] = "Frank (BMW 550i GPRS) Frank (BMW 550i GPRS) Frank (BMW 550i GPRS) Frank (BMW 550i GPRS)";
            }
            return dt;
        }

        protected void gdVehicle_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            gdVehicle.DataSource = GetVehicles();
        }
        protected void gdVehicle_ItemDataBound(object sender, GridItemEventArgs e)
        {
            return;
        }

        protected void gdAssignment_ItemDataBound(object sender, GridItemEventArgs e)
        { 
            if (e.Item.DataItem != null)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                ImageButton imgEdit = (ImageButton)e.Item.FindControl("imgEdit");
                if (imgEdit!= null)
                {
                   Int64 vehicleId = Int64.Parse(drv["VehicleId"].ToString());
                   int mediaId = int.Parse(drv["MediaId"].ToString());
                   int assignmentId = int.Parse(drv["AssignmentId"].ToString());
                   int equipmentMediaAssigmentId = int.Parse(drv["EquipmentMediaAssigmentId"].ToString());
                   int fleetID = -1;
                   if (cboFleet.SelectedIndex > 0) fleetID = int.Parse(cboFleet.SelectedValue);
                   string dateStr = String.Format("yyyymmddHHmmss", DateTime.Now);
                    string boxID = string.Empty;
                   if (((GridDataItem)((RadGrid)sender).Parent.Parent).DataItem is DataRowView)
                   {
                       DataRowView pDrv = ((DataRowView)(((GridDataItem)((RadGrid)sender).Parent.Parent).DataItem));
                       if (!(pDrv["BoxId"] is DBNull)) boxID= pDrv["BoxId"].ToString();
                   }
                   

                    
                   imgEdit.OnClientClick = string.Format("javascript:window.radopen('frmEditFactorValues.aspx?" + 
                       "v={0}&m={1}&a={2}&em={3}&f={4}&b={5}&rmd={6}', 'EditAssignment');return false;",
                       vehicleId, mediaId, assignmentId, equipmentMediaAssigmentId, fleetID, boxID, dateStr);

                }
            }
        }
        
        protected void gdAssignment_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (((GridDataItem)((RadGrid)sender).Parent.Parent).GetDataKeyValue("VehicleId") != null)
            {
                Int64 vehicleId = Int64.Parse(((GridDataItem)((RadGrid)sender).Parent.Parent).GetDataKeyValue("VehicleId").ToString());
                ((RadGrid)sender).DataSource = vehicleEquipmentAssignmentMg.GetVehicleEquipmentAssignmentByID(vehicleId);
            }

        }

        protected void gdAssignment_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            if (((GridDataItem)e.Item).GetDataKeyValue("AssignmentId") != null)
            {
                try
                {
                    int assignmentId = int.Parse(((GridDataItem)e.Item).GetDataKeyValue("AssignmentId").ToString());
                    vehicleEquipmentAssignmentMg.DeleteVehicleEquipmentAssignment(assignmentId);
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
        protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            if (e.Argument == "Rebind")
            {
                //gdVehicle.MasterTableView.SortExpressions.Clear();
                //gdVehicle.MasterTableView.GroupByExpressions.Clear();
                gdVehicle.Rebind();
            }
        }

        protected void cmdVehicleInfo_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("../frmVehicleInfo.aspx");
        }
        protected void cmdAlarms_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("../frmAlarms.aspx");
        }
        protected void cmdFleetVehicle_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("../frmVehicleFleet.aspx");
        }

        protected void cmdOutputs_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("../frmOutputs.aspx");
        }
        protected void cboFleet_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            gdVehicle.DataSource = GetVehicles();
            gdVehicle.DataBind();
        }
}
}