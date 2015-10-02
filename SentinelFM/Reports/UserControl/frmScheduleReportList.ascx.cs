using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;
using Telerik.Web.UI;
using SentinelFM.net.mappoint.staging;
namespace SentinelFM
{
    public partial class Reports_UserControl_frmScheduleReportList : System.Web.UI.UserControl
    {
        public string RadTabStripClientID = string.Empty;
        public string ViewRepor_Hidden_Msg = string.Empty;
        public string ViewText = "View";
        public string DownloadText = "Download";
        public string PendingText = "Pending";
        public string LoadFailed = string.Empty;
        public int iOrganizationID = 0;
        public int iSessionUserID = 0;
        public System.Globalization.CultureInfo currentCulture = null;
        private int iParentUserGroupId = 0;
        private int iUserGroupId = 0;  
        private string CurrentLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower();

        # region Properties

        public int ParentUserGroupId
        {
            get { return iParentUserGroupId; }
            set { iParentUserGroupId = value; }
        }

        public int UserGroupId
        {
            get { return iUserGroupId; }
            set { iUserGroupId = value; }
        }

        #endregion Properties

        List<RadDatePicker> datePickers = new List<RadDatePicker>();

        protected void Page_Load(object sender, EventArgs e)
        {
            RadTabStripClientID = Parent.FindControl("RadTabStrip1").ClientID;
            ViewRepor_Hidden_Msg = Parent.FindControl("ViewReport1").FindControl("ViewRepor_Hidden_Msg").ClientID;
            ViewText = (string)base.GetLocalResourceObject("ViewResource");
            DownloadText = (string)base.GetLocalResourceObject("DownloadResource");
            PendingText = (string)base.GetLocalResourceObject("PendingResource");
            LoadFailed = (string)base.GetLocalResourceObject("LoadFailedResource");

            if (!IsPostBack)
            {
                InitialControls();
            }
        }
        protected void gdScheduleReports_Init(object sender, EventArgs e)
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

        private bool InitialControls() 
        {
            for (int i = 0; i < this.gdScheduleReports.Columns.Count; i++)
            {
                if (iOrganizationID == 951 || iOrganizationID == 480)
                {
                    if (this.gdScheduleReports.Columns[i].HeaderText.ToLower() == "edit")
                    {
                        this.gdScheduleReports.Columns[i].Visible = true;
                    }
                }
                if ((iOrganizationID == 951 && (iParentUserGroupId == 2 || iUserGroupId == 2)) || iUserGroupId == 1)
                {
                    if (this.gdScheduleReports.Columns[i].HeaderText.ToLower() == "user")
                    {
                        this.gdScheduleReports.Columns[i].Visible = true;
                    }
                }
            }
            
            return true;
        }
    }
}