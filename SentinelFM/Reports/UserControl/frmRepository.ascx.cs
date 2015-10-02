using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
namespace SentinelFM
{
    public partial class Reports_UserControl_frmRepository : System.Web.UI.UserControl
    {
        public string ViewRepor_Hidden_Msg = string.Empty;
        public string RadTabStripClientID = string.Empty;
        public string isSecurity = "0";
        public string DateTimeFilterFlage = clsAsynGenerateReport.DateTimeFilterFlage;
        public string DateTimeFilterMin = clsAsynGenerateReport.DateTimeFilterMin;
        public string DateTimeFilterMax = clsAsynGenerateReport.DateTimeFilterMax;
        public string RefreshingResource = string.Empty;
        public string ViewText = "View";
        public string DownloadText = "Download";
        public string PendingText = "Pending";
        public string TimeOutText = "Time out";
        public string LoadFailed = string.Empty;
        List<RadDatePicker> datePickers = new List<RadDatePicker>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsSecureConnection) isSecurity = "1";
            try
            {
                RefreshingResource = (string)base.GetLocalResourceObject("RefreshingResource");

                ViewText = (string)base.GetLocalResourceObject("ViewResource");
                DownloadText = (string)base.GetLocalResourceObject("DownloadResource");
                PendingText = (string)base.GetLocalResourceObject("PendingResource");
                LoadFailed = (string)base.GetLocalResourceObject("LoadFailedResource");
                RadTabStripClientID = Parent.FindControl("RadTabStrip1").ClientID;
                ViewRepor_Hidden_Msg = Parent.FindControl("ViewReport1").FindControl("ViewRepor_Hidden_Msg").ClientID;
            }
            catch (Exception ex) { }
        }
        protected void gdRepository_Init(object sender, EventArgs e)
        {
            clsGridFilterMenu clsGridMenu = new clsGridFilterMenu();
            clsGridMenu.CreateGridFilterMenu((RadGrid)sender);
        }
        protected void gdScheduleReports_ItemCreated(object sender, GridItemEventArgs e)
        {
            clsGridFilterMenu clsGridMenu = new clsGridFilterMenu();
            clsGridMenu.NoWrapFilterMenu(e);

        }

        protected void gdRepository_PreRender(object sender, EventArgs e)
        {
            clsGridFilterMenu clsGridMenu = new clsGridFilterMenu();
            clsGridMenu.PreRender_Grid((RadGrid)sender, datePickers);

        }
    }
}