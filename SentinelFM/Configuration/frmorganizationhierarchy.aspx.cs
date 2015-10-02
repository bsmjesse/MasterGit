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
using VLF.PATCH.Logic;
using System.Data.OleDb;

namespace SentinelFM
{

    public partial class Configuration_frmorganizationhierarchy : SentinelFMBasePage
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        public string msgNoHierarchySelected;
        public string msgConfirmDelete;
        public string msgNodeCodeRequired;
        public string msgNodeNameRequired;
        public string msgAddNodeCodeSuccess;
        public string msgAddNodeCodeFail;
        public string msgDeleteNodeCodeSuccess;
        public string msgDeleteNodeCodeFail;
        public bool MultipleUserHierarchyAssignment;
        public int VehiclePageSize = 10;

        VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;

        public string RootOrganizationHierarchyNodeCode = string.Empty;
        protected ServerDBUser.DBUser dbu;

        protected void Page_Load(object sender, EventArgs e)
        {
            msgNoHierarchySelected = ((string)base.GetLocalResourceObject("msgNoHierarchySelected")).Replace("'", "\\'");
            msgConfirmDelete = ((string)base.GetLocalResourceObject("msgConfirmDelete")).Replace("'", "\\'");
            msgNodeCodeRequired = ((string)base.GetLocalResourceObject("msgNodeCodeRequired")).Replace("'", "\\'");
            msgAddNodeCodeSuccess = ((string)base.GetLocalResourceObject("msgAddNodeCodeSuccess")).Replace("'", "\\'");
            msgAddNodeCodeFail = ((string)base.GetLocalResourceObject("msgAddNodeCodeFail")).Replace("'", "\\'");
            msgNodeNameRequired = ((string)base.GetLocalResourceObject("msgNodeNameRequired")).Replace("'", "\\'");
            msgDeleteNodeCodeSuccess = ((string)base.GetLocalResourceObject("msgDeleteNodeCodeSuccess")).Replace("'", "\\'");
            msgDeleteNodeCodeFail = ((string)base.GetLocalResourceObject("msgDeleteNodeCodeFail")).Replace("'", "\\'");

            MultipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

            if (ConfigurationManager.AppSettings["VehicleListTreePageSize"] != null)
                int.TryParse(ConfigurationManager.AppSettings["VehicleListTreePageSize"].ToString(), out VehiclePageSize);

            if(MultipleUserHierarchyAssignment)
            {
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                RootOrganizationHierarchyNodeCode = poh.ValidatedNodeCodes(sn.User.OrganizationId, sn.UserID, sn.User.PreferNodeCodes); 
                if(RootOrganizationHierarchyNodeCode == "")
                    RootOrganizationHierarchyNodeCode = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID, MultipleUserHierarchyAssignment);
            }

            
            if (!Page.IsPostBack)
            {
                
            }
        }

        
        

        
}
}