using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;
using Telerik.Web.UI;

    public partial class MasterPage_MasterPage : System.Web.UI.MasterPage
    {
        protected SentinelFMSession sn = null;
        public List<SideMenu> sideMenus;
        public string resizeScript = "";
        public Boolean isHideScroll = false;
        string currentMenuID = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (sideMenus != null)
                {
                    FillSideMenu();
                }

                if (isHideScroll) AddHideScrollbarStyle();
            }
        }

        private void AddHideScrollbarStyle()
        {
            Literal literal = new Literal();
            literal.Text = "<style type=\"text/css\" >html, body" +
                         "{" +
                             "margin: 0;" +
                             "padding: 0;" +
                             "height : 100%;" +
                             "overflow: hidden;" +
                          "}</style>";
            this.head.Controls.Add(literal);
        }
             
        private void FillSideMenu()
        {
            foreach (SideMenu sideMenu in sideMenus)
            {
                RadPanelItem item = new RadPanelItem();
                item.Text = sideMenu.MenuText;
                if (!string.IsNullOrEmpty(sideMenu.MenuImage))
                {
                    item.ImageUrl = sideMenu.MenuImage;
                }
                if (!string.IsNullOrEmpty(sideMenu.MenuUrl))
                {
                    item.NavigateUrl = sideMenu.MenuUrl;
                }
                if (sideMenu.SubSideMenu != null)
                {
                    item.Expanded = 
                    FillSubSideMenu(sideMenu.SubSideMenu, item);
                }
                else
                {
                    item.Expanded = false;
                }
                if (item.NavigateUrl.ToLower().Contains(GetPageName()))
                {
                    item.Selected = true;
                }
                else
                {
                    if (currentMenuID == sideMenu.MenuID && sideMenu.MenuID != null)
                    {
                        item.Selected = true;
                    }
                    else 
                       item.Selected = false;
                }

                radSideMenu.Items.Add(item);
            }
        }

        private Boolean FillSubSideMenu(List<SideMenu> listSideMenu,  RadPanelItem panelItem)
        {
            Boolean isSelect = false;
            foreach (SideMenu sideMenu in listSideMenu)
            {
                RadPanelItem item = new RadPanelItem();
                item.Text = sideMenu.MenuText;
                if (!string.IsNullOrEmpty(sideMenu.MenuImage))
                {
                    item.ImageUrl = sideMenu.MenuImage;
                }
                if (!string.IsNullOrEmpty(sideMenu.MenuUrl))
                {
                    item.NavigateUrl = sideMenu.MenuUrl;
                }
                if (sideMenu.SubSideMenu != null)
                {
                    item.Expanded = true;
                }
                else
                {
                    item.Expanded = false;
                }
                if (item.NavigateUrl.ToLower().Contains(GetPageName()))
                {
                    item.Selected = true;
                    isSelect = true;
                }
                else
                {
                    if (currentMenuID == sideMenu.MenuID && sideMenu.MenuID != null)
                    {
                        item.Selected = true;
                        isSelect = true;

                    }
                    else 
                       item.Selected = false;
                }

                panelItem.Items.Add(item);
            }
            return isSelect;
        }

        public string GetPageName()
        {
            string url = HttpContext.Current.Request.Url.ToString().ToLower();
            if (url.IndexOf("&")  > 0)
                url = url.Substring(0, url.IndexOf("&"));
            else url = url.Substring(0, url.IndexOf(".aspx")) + ".aspx";
            url = url.Substring(url.LastIndexOf("/") + 1);
            return url;
        }

        public void CreateMaintenenceMenu(string _currentMenuID)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            currentMenuID = _currentMenuID;
            sideMenus = new List<SideMenu>();
            SideMenu menu = new SideMenu();
            menu.MenuText = (string)base.GetLocalResourceObject("mnuMaintenance");
            menu.MenuUrl = "../Maintenance/frmMaintenanceGrid.aspx";
            sideMenus.Add(menu);

            menu = new SideMenu();
            menu.MenuText = (string)base.GetLocalResourceObject("mnuHistory");
            menu.MenuUrl = "../Maintenance/frmMaintenanceHst.aspx";
            sideMenus.Add(menu);



            if (sn.User.OrganizationId != 999763 && sn.User.OrganizationId != 999955 && sn.User.OrganizationId != 999956 && sn.User.OrganizationId != 999957)
            {
                menu = new SideMenu();
                menu.MenuText = (string)base.GetLocalResourceObject("mnuArchive");
                menu.MenuUrl = "../Maintenance/Old_His.aspx";
                sideMenus.Add(menu);
            }


            menu = new SideMenu();
            menu.MenuText = (string)base.GetLocalResourceObject("mnuNotification");
            menu.MenuUrl = "../Maintenance/frmMaintenanceNoti.aspx";
            sideMenus.Add(menu);

            menu = new SideMenu();
            menu.MenuText = (string)base.GetLocalResourceObject("mnuDTCNotification");
            menu.MenuUrl = "../Maintenance/frmMaintenanceDTC.aspx";
            sideMenus.Add(menu);

            menu = new SideMenu();
            menu.MenuText = (string)base.GetLocalResourceObject("mnuSetEngineHours");
            menu.MenuUrl = "../Maintenance/frmMaintenanceUpdateEhr.aspx";
            sideMenus.Add(menu);

            bool CmdStatus = false;
            if (sn != null)
              CmdStatus = sn.User.ControlEnable(sn, 58);

            SideMenu adminMenu = new SideMenu();
            adminMenu.MenuText = (string)base.GetLocalResourceObject("mnuSetup");
            adminMenu.SubSideMenu = new List<SideMenu>();
            
            menu = new SideMenu();
            menu.MenuText = (string)base.GetLocalResourceObject("mnuPMServices");
            menu.MenuUrl = "../Maintenance/frmMaintenances.aspx";
            adminMenu.SubSideMenu.Add(menu);

            menu = new SideMenu();
            menu.MenuText = (string)base.GetLocalResourceObject("mnuPMGroup");
            menu.MenuUrl = "../Maintenance/frmMaintenanceGroup.aspx";
            adminMenu.SubSideMenu.Add(menu);

            menu = new SideMenu();
            menu.MenuText = (string)base.GetLocalResourceObject("mnuServiceAssignments");
            menu.MenuUrl = "../Maintenance/frmMaintenanceGroupAssign.aspx";
            adminMenu.SubSideMenu.Add(menu);

            menu = new SideMenu();
            menu.MenuText = (string)base.GetLocalResourceObject("mnuAddVehicleAssignments");
            menu.MenuUrl = "../Maintenance/frmMaintenanceVehicle.aspx";
            adminMenu.SubSideMenu.Add(menu);

            menu = new SideMenu();
            menu.MenuText = (string)base.GetLocalResourceObject("mnuViewVehicleAssignments");
            menu.MenuUrl = "../Maintenance/frmMaintenanceVehicleView.aspx";
            adminMenu.SubSideMenu.Add(menu);

            //menu = new SideMenu();
            //menu.MenuText = "Set Engine Hours";
            //menu.MenuUrl = "../Maintenance/frmMaintenanceUpdateEhr.aspx";
            //adminMenu.SubSideMenu.Add(menu);

            //menu = new SideMenu();
            //menu.MenuText = "Box Setting";
            //menu.MenuUrl = "../Maintenance/MaintenanceBoxUserSettings.aspx";
            //adminMenu.SubSideMenu.Add(menu);

            menu = new SideMenu();
            menu.MenuText = (string)base.GetLocalResourceObject("mnuOperationLog");
            menu.MenuUrl = "../Maintenance/frmMaintenanceOperation.aspx";
            adminMenu.SubSideMenu.Add(menu);

            if (CmdStatus)  sideMenus.Add(adminMenu);
        }

        public void CreateMaintenenceAdminMenu()
        {
            //sideMenus = new List<SideMenu>();
            //sideMenus.Add(new SideMenu() { MenuText = "Notification Policy", MenuUrl = "../Maintenance/frmMaintenanceGrid.aspx?type=NotificationPolicy&" });
            //sideMenus.Add(new SideMenu() { MenuText = "Services", MenuUrl = "../Maintenance/frmMaintenanceGrid.aspx?type=Services&" });
            //sideMenus.Add(new SideMenu() { MenuText = "MCC Group", MenuUrl = "../Maintenance/frmMaintenanceGrid.aspx?type=MCCGroup&" });
            //sideMenus.Add(new SideMenu() { MenuText = "MCC Maintenances", MenuUrl = "../Maintenance/frmMaintenanceGrid.aspx?type=MCCMaintenances&" });
            //sideMenus.Add(new SideMenu() { MenuText = "MCC Notification Type", MenuUrl = "../Maintenance/frmMaintenanceGrid.aspx?type=MCCNotificationType&" });
            //sideMenus.Add(new SideMenu() { MenuText = "MCC Group Maintenances Assignment", MenuUrl = "../Maintenance/frmMaintenanceGrid.aspx?type=MCCAssignment&" });
            //sideMenus.Add(new SideMenu() { MenuText = "Vehicle Maintenances Assignment", MenuUrl = "../Maintenance/frmMaintenanceGrid.aspx?type=MCCVehicleMaintenance&" });
        }

    }
