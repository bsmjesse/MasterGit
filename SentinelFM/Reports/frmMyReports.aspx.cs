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
using System.Configuration;
using System.Text;
using VLF.CLS;
using Telerik.Web.UI;
using System.Collections.Generic;

namespace SentinelFM
{
    public partial class Reports_frmMyReports : SentinelFMBasePage
    {
        public string ExecuteText = "Execute";
        public string ModifyText = "Modify";
        public string LoadFailed = string.Empty;
        public string LoadingText = "Loading";
        public string BackTextResource = "Back";
        List<RadDatePicker> datePickers = new List<RadDatePicker>();
        public CultureInfo currentCulture = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                EnableExtendedReports();
            }
            ExecuteText = (string)base.GetLocalResourceObject("ExecuteResource");
            ModifyText = (string)base.GetLocalResourceObject("ModifyResource");
            LoadFailed = (string)base.GetLocalResourceObject("LoadFailedResource");
            LoadingText = (string)base.GetLocalResourceObject("txtLoadingResource");
            BackTextResource = (string)base.GetLocalResourceObject("BackTextResource");
        }
        /// <summary>
        /// Enable extended reports according to organization ID
        /// </summary>
        private void EnableExtendedReports()
        {
            if (sn.User.OrganizationId == 123 || sn.SuperOrganizationId == 382 || sn.User.OrganizationId == 327 || sn.User.OrganizationId == 489 || sn.User.OrganizationId == 622 || sn.User.OrganizationId == 570 || sn.User.OrganizationId == 18 || sn.User.OrganizationId == 951 || sn.User.OrganizationId == 999620 || sn.User.OrganizationId == 698)
            {
                ddlReport.Items.FindItemByValue("1").Visible = true;
            }
        }

        protected void ddlReport_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (ddlReport.SelectedIndex == 1)
                Response.Redirect("frmReportMasterExtended_new.aspx");
            else if (ddlReport.SelectedIndex == 0)
                Response.Redirect("frmReports_new.aspx");
            else if (ddlReport.SelectedIndex == 3)
                Response.Redirect("frmSSRS.aspx");
        }

        protected void gdMyReports_Init(object sender, EventArgs e)
        {
            clsGridFilterMenu clsGridMenu = new clsGridFilterMenu();
            clsGridMenu.CreateGridFilterMenu((RadGrid)sender);
        }
        protected void gdScheduleReports_ItemCreated(object sender, GridItemEventArgs e)
        {
            clsGridFilterMenu clsGridMenu = new clsGridFilterMenu();
            clsGridMenu.NoWrapFilterMenu(e);
        }
        protected void Grid_PreRend(object sender, EventArgs e)
        {
            clsGridFilterMenu clsGridMenu = new clsGridFilterMenu();
            clsGridMenu.PreRender_Grid((RadGrid)sender, datePickers);
            //For filter calendar globalization
            foreach (RadDatePicker rd in datePickers)
            {
                rd.PreRender += new EventHandler(rd_PreRender);
                rd.Unload += new EventHandler(rd_unload);
            }

        }

        void rd_PreRender(object sender, EventArgs e)
        {
            if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
            {
                if (currentCulture == null) currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CurrentUICulture;
            }
        }

        protected void rd_unload(object sender, EventArgs e)
        {
            if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
            {
                if (currentCulture != null) 
                System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }
    }
}