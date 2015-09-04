using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using Telerik.Web.UI;
using System.Configuration;

namespace SentinelFM
{
    public delegate void ActionClick();
    public partial class UserControl_FleetOrganizationHierarchy : System.Web.UI.UserControl
    {
        public event ActionClick OnOkClick;

        public RadAjaxManager radAjaxManager1 = null;
        public string radAjaxLoadingPanel1 = null;
        public string radUpdatedControl = null;
        public bool isLoadDefault = false;
        protected SentinelFMSession sn = null;
        clsUtility objUtil = null;

        public bool ShowOrganizationHierarchy;
        public string DefaultOrganizationHierarchyFleetId = "-1";
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string OrganizationHierarchyPath = "";
        
        public string RootUrl;

        public bool MutipleUserHierarchyAssignment = false;
        public string RootOrganizationHierarchyNodeCode = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            objUtil = new clsUtility(sn);
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                return;
            }
            
            string defaultnodecode = string.Empty;

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            defaultnodecode = sn.User.PreferNodeCodes;
            if (defaultnodecode.Contains(','))
                defaultnodecode = "";

            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

            VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
            if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                ShowOrganizationHierarchy = true;
            else
            {
                ShowOrganizationHierarchy = false;
  
            }

            if (sn.User.LoadVehiclesBasedOn == "hierarchy")
            {
                MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
                if(MutipleUserHierarchyAssignment)
                    RootOrganizationHierarchyNodeCode = sn.User.PreferNodeCodes;
                if (RootOrganizationHierarchyNodeCode.Contains(','))
                    RootOrganizationHierarchyNodeCode = "";
                defaultnodecode = defaultnodecode ?? string.Empty;
                if (defaultnodecode == string.Empty)
                {
                    if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 480)
                        defaultnodecode = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID);
                    else
                        defaultnodecode = poh.GetRootNodeCode(sn.User.OrganizationId);

                }
                if (!IsPostBack)
                {
                    DefaultOrganizationHierarchyNodeCode = defaultnodecode;
                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode).ToString();
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(DefaultOrganizationHierarchyFleetId));
                    hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;

                    if (MutipleUserHierarchyAssignment)
                    {
                        hidOrganizationHierarchyFleetId.Value = sn.User.PreferFleetIds;
						hidOrganizationHierarchyNodeCode.Value = sn.User.PreferNodeCodes;
                        DefaultOrganizationHierarchyFleetId = sn.User.PreferFleetIds;
                    }
                }
                else
                {
                    hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());
                    
                    DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();
                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode).ToString();
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(DefaultOrganizationHierarchyFleetId));
                }

                if (MutipleUserHierarchyAssignment)
                {

                    if (hidOrganizationHierarchyFleetId.Value.Trim() == string.Empty)
                        DefaultOrganizationHierarchyFleetName = "";
                    else if (hidOrganizationHierarchyFleetId.Value.Contains(","))
                    {
                        DefaultOrganizationHierarchyFleetName = "Select a hierarchy";
                        hidOrganizationHierarchyFleetId.Value="";
                        hidOrganizationHierarchyNodeCode.Value = "";
                        DefaultOrganizationHierarchyFleetId = "";
                    }
                    else
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(hidOrganizationHierarchyFleetId.Value));
                }

                OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);

                
                btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName;


                hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
            }
            
            Uri MyUrl = Request.Url;
            RootUrl = Request.Url.Scheme + "://" + Request.Url.Authority +  Request.ApplicationPath.TrimEnd('/') + "/";
            vehicletreeviewIni.Value = "false";
               
        }



       

        public string GetSelectedFleet()
        {
            return hidOrganizationHierarchyFleetId.Value;
        }

        public void ReSetFleet()
        {
            string defaultnodecode = string.Empty;
            defaultnodecode = sn.User.PreferNodeCodes;

            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
            defaultnodecode = defaultnodecode ?? string.Empty;
            if (defaultnodecode == string.Empty)
            {
                if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 480)
                    defaultnodecode = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID);
                else
                    defaultnodecode = poh.GetRootNodeCode(sn.User.OrganizationId);

            }
            DefaultOrganizationHierarchyNodeCode = defaultnodecode;
            DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode).ToString();
            DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, -1);
            hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;

            if (MutipleUserHierarchyAssignment)
            {
                hidOrganizationHierarchyFleetId.Value = sn.User.PreferFleetIds;
                hidOrganizationHierarchyNodeCode.Value = sn.User.PreferNodeCodes;
                DefaultOrganizationHierarchyFleetId = sn.User.PreferFleetIds;
            }
        }

        

        protected void btnOk_Click(object sender, EventArgs e)
        {
            if (OnOkClick != null)
            {
                OnOkClick();
            }
        }        

       
}
}
