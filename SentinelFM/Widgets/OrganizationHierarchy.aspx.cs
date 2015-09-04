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
using System.Linq;

namespace SentinelFM
{
    public partial class Widgets_OrganizationHierarchy : SentinelFMBasePage
    {
        protected SentinelFMSession sn = null;
        clsUtility objUtil = null;

        public bool ShowOrganizationHierarchy;
        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;        
        public string OrganizationHierarchyPath = "";

        public string RootUrl;

        public bool MutipleUserHierarchyAssignment = false;
        public bool FullTreeView = true;
        public string PreferOrganizationHierarchyNodeCode = string.Empty;
        public string SelectedOrganizationHierarchyNodeCode = string.Empty;

        public string RootOrganizationHierarchyNodeCode = string.Empty;
        public bool LoadVehicles = true;
        public int VehiclePageSize = 10;

        public string ResMultipleHierarchy;

        private bool gotoLastNode = false;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            objUtil = new clsUtility(sn);
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                return;
            }

            ResMultipleHierarchy = GetLocalResourceObject("MultipleHierarchy.Text").ToString(); //"Multiple Hierarchies";

            if (ConfigurationManager.AppSettings["VehicleListTreePageSize"] != null)
                int.TryParse(ConfigurationManager.AppSettings["VehicleListTreePageSize"].ToString(), out VehiclePageSize);

            string defaultnodecode = string.Empty;

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            defaultnodecode = Request["nodecode"] ?? string.Empty;

            RootOrganizationHierarchyNodeCode = Request["rootNodecode"] ?? string.Empty;

            if (clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment"))
                RootOrganizationHierarchyNodeCode = sn.User.PreferNodeCodes;

            if (Request["m"] != null && Request["m"].ToString().Trim() == "1")
                MutipleUserHierarchyAssignment = true;

            if (Request["loadVehicle"] != null && Request["loadVehicle"].ToString().Trim() == "0")
                LoadVehicles = false;  

            if (Request["f"] != null && Request["f"].ToString().Trim() == "0")
                FullTreeView = false;

            if (Request["sl"] != null && Request["sl"].ToString().Trim() == "1")
                gotoLastNode = true;

            PreferOrganizationHierarchyNodeCode = Request["preferNodecode"] ?? string.Empty;

            if (SelectedOrganizationHierarchyNodeCode == string.Empty)
            {
                SelectedOrganizationHierarchyNodeCode = defaultnodecode;
                hidOrganizationHierarchyNodeCode.Value = defaultnodecode;
            }

            if (defaultnodecode == string.Empty)
            {

                string xml = "";
                if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                    {

                    }

                StringReader strrXML = new StringReader(xml);
                DataSet dsPref = new DataSet();
                dsPref.ReadXml(strrXML);

                foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                {
                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode))
                    {
                        defaultnodecode = rowItem["PreferenceValue"].ToString().TrimEnd();

                    }
                }
            }

            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

            VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
            
            if (defaultnodecode == string.Empty)
                defaultnodecode = poh.GetRootNodeCode(sn.User.OrganizationId);

            DefaultOrganizationHierarchyNodeCode = defaultnodecode;
            DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
            
            string[] ss = defaultnodecode.Split(',');
            List<string> pathList = new List<string>();
            if (ss.Length < 20)
            {
                foreach (string s in ss)
                {
                    string p = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, s).Trim('/');
                    string[] ps = p.Split('/');
                    List<string> tmp = new List<string>(ps);
                    if (!gotoLastNode)
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
            }
            OrganizationHierarchyPath = String.Join("/", pathList.ToArray());

            //OrganizationHierarchyPath = OrganizationHierarchyPath.Trim('/');
            //OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);            
        }


    }
}