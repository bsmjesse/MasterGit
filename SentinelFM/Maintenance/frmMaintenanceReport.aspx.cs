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

    public partial class Maintenance_frmMaintenanceReport : SentinelFMBasePage
    {
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
                }
                dgMaintenance.RadAjaxManagerControl = RadAjaxManager1;

                this.Master.resizeScript = SetResizeScript(dgMaintenance.ClientID);
                this.Master.isHideScroll = true;
                dgMaintenance.RadAjaxManagerControl = RadAjaxManager1;
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
        }

        protected void dgMaintenance_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BinddgMaintenance(false);
        }

        private void BinddgMaintenance(Boolean isBind)
        {
            try
            {
                DateTime dt = System.DateTime.Now.Date;

                if (ddlDateTime.SelectedValue == "1") dt = dt.AddDays(-7);
                if (ddlDateTime.SelectedValue == "2") dt = dt.AddDays(-14);
                if (ddlDateTime.SelectedValue == "3") dt = dt.AddMonths(-1);
                if (ddlDateTime.SelectedValue == "4") dt = dt.AddMonths(-2);
                if (ddlDateTime.SelectedValue == "5") dt = dt.AddMonths(-3);
                if (ddlDateTime.SelectedValue == "6") dt = dt.AddMonths(-6);
                if (ddlDateTime.SelectedValue == "7") dt = dt.AddMonths(-12);
                if (ddlDateTime.SelectedValue == "8") dt = dt.AddYears(-2);
                if (ddlDateTime.SelectedValue == "9") dt = new DateTime(1970, 1, 1);
                MCCManager mccMgr = new MCCManager(sConnectionString);
                DataSet ds = mccMgr.MaintenanceGetOperationReport(dt, sn.User.OrganizationId);
                //to be changed
                dgMaintenance.DataSource = ds;
                if (isBind)
                {
                    dgMaintenance.DataBind();
                }
            }
            catch (Exception Ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("BoxId");
                dt.Columns.Add("VehicleDescription");
                dgMaintenance.DataSource = dt;
                if (isBind)
                {
                    dgMaintenance.DataBind();
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                RadAjaxManager1.ResponseScripts.Add(string.Format("alert('{0}')", Error_Load));
            }
        }

        protected void ddlDateTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            BinddgMaintenance(true);
        }

    }
}