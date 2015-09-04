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

    public partial class Maintenance_MaintenanceBoxUserSettings : SentinelFMBasePage
    {
        string stringNoFleets =	"No valid fleets";
        string stringSelectFleet = 	"Please select fleet";
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        public string Error_Load = "Failed to load data.";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
                }
                if (!IsPostBack)
                {
                    this.Master.CreateMaintenenceMenu(null);
                    GetFleets();
                }
                dgMaintenance.RadAjaxManagerControl = RadAjaxManager1;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                base.RedirectToLogin();
            }
            this.Master.resizeScript = SetResizeScript(dgMaintenance.ClientID);
            this.Master.isHideScroll = true;
        }

        protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {

        }

        protected void GetFleets()
        {

            try
            {
                //if (objUtil.ErrCheck(dbFleet.GetFleetsInfoXMLByUserId(sn.UserID, sn.SecId, ref xml), false))
                //    if (objUtil.ErrCheck(dbFleet.GetFleetsInfoXMLByUserId(sn.UserID, sn.SecId, ref xml), true))
                //    {
                //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                //           VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetFleetsInfoXMLByUserId Error : User:" +
                //           sn.UserID.ToString() + " FleetVehiclesControl.ascx"));
                //        return;
                //    }
                //if (String.IsNullOrEmpty(xml))
                //    return;


                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                if (Util.IsDataSetValid(dsFleets))
                {
                    cboFleet.DataSource = dsFleets;
                    cboFleet.DataBind();

                    cboFleet.Items.Insert(0, new RadComboBoxItem(stringSelectFleet, "-1"));
                }
                else
                {
                    cboFleet.Items.Insert(0, new RadComboBoxItem(stringNoFleets, "-1"));
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " FleetVehiclesControl.ascx"));
            }
        }
        protected void cboFleet_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            BindMaintenance(true);
        }
        protected void dgMaintenance_ItemDataBound(object sender, GridItemEventArgs e)
        {

        }

        protected void dgMaintenance_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindMaintenance(false);
        }

        private void BindMaintenance(Boolean isBind)
        {
            try
            {
                MCCManager mccMgr = new MCCManager(sConnectionString);
                if (cboFleet.SelectedIndex > 0)
                {
                    DataSet ds = mccMgr.MaintenanceBoxUserSettings_Get(int.Parse(cboFleet.SelectedValue));

                    //to be changed
                    //if (ds.Tables[0].Rows.Count > 0) ds.Tables[0].Rows[0]["OperationTypeID"] = "3";
                    dgMaintenance.DataSource = ds;
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("VehicleId");
                    dgMaintenance.DataSource = dt;
                }

                if (isBind)
                {
                    dgMaintenance.DataBind();
                }
            }
            catch (Exception Ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("VehicleId");
                dgMaintenance.DataSource = dt;
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", Error_Load));
            }
        }

}
}