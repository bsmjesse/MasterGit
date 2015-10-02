using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.IO;

namespace SentinelFM
{
    public partial class UserControl_HierarchyTree : System.Web.UI.UserControl
    {

        private bool _multipleUserHierarchyAssignment = false;
        public bool MultipleUserHierarchyAssignment {
            get { return _multipleUserHierarchyAssignment; }
            set { _multipleUserHierarchyAssignment = value; }
        }

        public bool OrganizationHierarchySelectVehicle = false; // In vehicle list, should we highlight the vehicle when we click it.
        public bool PreSelectHierarchy = false; //For Multiple Hierarchy, if we should pre-check some hierarchies
        public string OrganizationHierarchyPath = ""; //posibble value: 1, existingselection 2, real nodecode 3, if empty, use user preference
        public bool GotoLastNode = true;   // when expand node, should we expand the last node?
        public string GetRootHierarchyBy = ""; // userpreference: get root of the hierarchy tree by User Preference, otherwise it will get root by user's hierarchy assignment.
        public string Width = "100%";   // Hierarchy Tree's width, for example: 100%, 600px, 800px, 800, etc...
        public bool ManagerColumn = false; // Normally, we don't have to set this value, we will set this value from organization settings. but in some cases, we could set this value manually.
        public bool HierarchyEditMode = false;
        public bool LoadVehicleData = true;
        public string HierarchyCheckCallBack = ""; //call back function name when hierarchy checkbox clicked

        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        
        public string PreferOrganizationHierarchyNodeCode = string.Empty;
        
        protected int VehiclePageSize = 10;
        protected string RootHierarchyNodeCodes = "";
        protected string RootUrl;
        protected bool MultipleHierarchySetting = false;        

        private VLF.PATCH.Logic.PatchOrganizationHierarchy poh;
        private SentinelFMSession sn;

        protected clsUtility objUtil;

        protected string msgNoHierarchySelected;
        protected string msgConfirmDelete;
        protected string msgNodeCodeRequired;
        protected string msgNodeNameRequired;
        protected string msgAddNodeCodeSuccess;
        protected string msgAddNodeCodeFail;
        protected string msgDeleteNodeCodeSuccess;
        protected string msgDeleteNodeCodeFail;

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


            if (HttpContext.Current.Session["SentinelFMSession"] != null)
                sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else
                return;

            objUtil = new clsUtility(sn);

            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
            
            Uri MyUrl = Request.Url;
            RootUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";

            MultipleHierarchySetting = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

            RootHierarchyNodeCodes = "";
            if (GetRootHierarchyBy.Trim().ToLower() == "userpreference")
            {
                RootHierarchyNodeCodes = sn.User.PreferNodeCodes;
            }
            if (RootHierarchyNodeCodes == string.Empty)
            {
                if (sn.RootOrganizationHierarchyNodeCode == string.Empty)
                {
                    RootHierarchyNodeCodes = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID, MultipleHierarchySetting);
                    sn.RootOrganizationHierarchyNodeCode = RootHierarchyNodeCodes;
                }
                else
                    RootHierarchyNodeCodes = sn.RootOrganizationHierarchyNodeCode;
            }

            if (PreSelectHierarchy)
            {
                if (OrganizationHierarchyPath.Trim().ToLower() == "existingselection")
                {
                    OrganizationHierarchyPath = getPathByNodeCode(this.OrganizationHierarchyNodeCode.Value);
                }             
            }

            GetOrganizationPreferences();

            
        }

        private string getPathByNodeCode(string defaultnodecode)
        {
            string[] ss = defaultnodecode.Split(',');
            List<string> pathList = new List<string>();
            foreach (string s in ss)
            {
                string p = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, s).Trim('/');
                string[] ps = p.Split('/');
                List<string> tmp = new List<string>(ps);
                if (!GotoLastNode)
                    tmp.RemoveAt(ps.Length - 1);    // remove the last nodecode, we don't want to expand it.
                ps = tmp.ToArray();

                foreach (string s1 in ps)
                {
                    int pos = pathList.FindIndex(f => f == s1);
                    if (pos < 0)
                    {
                        pathList.Add(s1);
                    }
                }

            }
            return String.Join("/", pathList.ToArray());
        }

        /// <summary>
        /// Get Form Field OrganizationHierarchyNodeCode's value
        /// </summary>
        public string Field_OrganizationHierarchyNodeCode
        {
            get { return this.OrganizationHierarchyNodeCode.Value; }
            set { this.OrganizationHierarchyNodeCode.Value = value;}
        }

        /// <summary>
        /// Get Form Field OrganizationHierarchyNodeCode's ClientID
        /// </summary>
        public string Field_OrganizationHierarchyNodeCode_ClientID
        {
            get { return this.OrganizationHierarchyNodeCode.ClientID; }
        }

        /// <summary>
        /// Get Form Field OrganizationHierarchyFleetId's value
        /// </summary>
        public string Field_OrganizationHierarchyFleetId
        {
            get { return this.OrganizationHierarchyFleetId.Value; }
        }

        public string Field_OrganizationHierarchyFleetName
        {
            get { return this.OrganizationHierarchyFleetName.Value; }
        }

        /// <summary>
        /// Get Form Field OrganizationHierarchyFleetId's ClientID
        /// </summary>
        public string Field_OrganizationHierarchyFleetId_ClientID
        {
            get { return this.OrganizationHierarchyFleetId.ClientID; }
        }

        /// <summary>
        /// Get Form Field OrganizationHierarchyBoxId's value
        /// </summary>
        public string Field_OrganizationHierarchyBoxId
        {
            get { return this.OrganizationHierarchyBoxId.Value; }
        }

        /// <summary>
        /// Get Form Field OrganizationHierarchyBoxId's ClientID
        /// </summary>
        public string Field_OrganizationHierarchyBoxId_ClientID
        {
            get { return this.OrganizationHierarchyBoxId.ClientID; }
        }

        /// <summary>
        /// Get Form Field OrganizationHierarchyVehicleDescription's value
        /// </summary>
        public string Field_OrganizationHierarchyVehicleDescription
        {
            get { return this.OrganizationHierarchyVehicleDescription.Value; }
        }

        /// <summary>
        /// Get Form Field OrganizationHierarchyVehicleDescription's ClientID
        /// </summary>
        public string Field_OrganizationHierarchyVehicleDescription_ClientID
        {
            get { return this.OrganizationHierarchyVehicleDescription.ClientID; }
        }

        public void SetOrganizationHierarchyPath()
        {   
            OrganizationHierarchyPath = getPathByNodeCode(this.OrganizationHierarchyNodeCode.Value);
        }

        private void GetOrganizationPreferences()
        {

            DataSet ds = new DataSet();
            string xml = "";

            using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
            {
                if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                    if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                    {
                        return;
                    }
            }


            if (xml == "")
            {
                return;
            }

            StringReader strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {

                try
                {
                    if (dr["OrgPreferenceId"].ToString() == "17")
                    {
                        if (dr["PreferenceValue"].ToString().Trim() == "1")
                        {
                            ManagerColumn = true;
                        }
                    }

                }
                catch
                {

                }

            }

        }

        
    }
}