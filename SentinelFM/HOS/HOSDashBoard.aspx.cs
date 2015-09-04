using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Globalization;
using System.Configuration;

namespace SentinelFM
{
    public partial class HOS_HOSDashBoard : System.Web.UI.Page
    {
        public string LastUpdatedHOSDriverStatusJS = System.IO.File.GetLastWriteTime(System.Web.HttpContext.Current.Server.MapPath("~/Map/ExtJS/HOSDriverStatus.js")).ToString("yyyyMMddHHmmss");
        protected SentinelFMSession sn = null;
        public string DefaultFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string LoadVehiclesBasedOn = "fleet";
        public bool MutipleUserHierarchyAssignment;
        string defaultnodecode = string.Empty;
        public string DefaultOrganizationHierarchyFleetId = "0";
        public string OrganizationHierarchyPath = "";
        public string ResfleetButtonOpenwindowMessage;
        public string ReshistoryFleetButtonsetText;
        public string ResMultipleHierarchy;
        //string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(ConfigurationManager.AppSettings.Get("SentinelFMConnection"));

        protected void Page_Load(object sender, EventArgs e)
        {
            //var h = LastUpdatedHOSDriverStatusJS;

            sn = (SentinelFMSession)Session["SentinelFMSession"];
            DefaultFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, sn.User.DefaultFleet);

            clsUtility objUtil;
            objUtil = new clsUtility(sn);
            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            string xml = "";
            if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                {                    
                }
            if (xml == "")
            {
            }

            System.IO.StringReader strrXML = new System.IO.StringReader(xml);
            System.Data.DataSet dsPref = new System.Data.DataSet();
            dsPref.ReadXml(strrXML);

            foreach (System.Data.DataRow rowItem in dsPref.Tables[0].Rows)
            {
                if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode))
                {
                    defaultnodecode = rowItem["PreferenceValue"].ToString().TrimEnd();

                }
                
                if (Convert.ToInt16(rowItem["PreferenceId"]) == 36)
                {
                    string d = rowItem["PreferenceValue"].ToString().ToLower();
                    if (d == "hierarchy")
                    {
                        LoadVehiclesBasedOn = "hierarchy";
                        ResfleetButtonOpenwindowMessage = "Sélectionner une flotte";
                    }
                    else
                    {
                        LoadVehiclesBasedOn = "fleet";
                        ResfleetButtonOpenwindowMessage = "Select a fleet";
                    }
                }
            }
            
            //MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
             MutipleUserHierarchyAssignment = false; //Already set true in FeaturePermission.xml, sets false for the requirement
            if (LoadVehiclesBasedOn == "hierarchy")
            {
                defaultnodecode = defaultnodecode ?? string.Empty;
                if (defaultnodecode == string.Empty)
                    defaultnodecode = poh.GetRootNodeCode(sn.User.OrganizationId);
                DefaultOrganizationHierarchyNodeCode = defaultnodecode;
                DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode).ToString();
                DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(DefaultOrganizationHierarchyFleetId));
                OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);
               
                if (MutipleUserHierarchyAssignment)
                {
                    DefaultOrganizationHierarchyFleetId = sn.User.PreferFleetIds;
                    DefaultOrganizationHierarchyNodeCode = sn.User.PreferNodeCodes;
                    if (DefaultOrganizationHierarchyFleetId.Trim() == string.Empty)
                       DefaultOrganizationHierarchyFleetName = "";
                    else if (DefaultOrganizationHierarchyFleetId.Contains(','))
                        DefaultOrganizationHierarchyFleetName = ResMultipleHierarchy;
                    else
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(DefaultOrganizationHierarchyFleetId));
                }
            }
            //else
            //{
            //    DefaultFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, sn.User.DefaultFleet).Replace(VLF.CLS.Def.Const.defaultFleetName, ReshistoryFleetButtonsetText);
            //}
        }

        protected override void InitializeCulture()
        {

            if (Session["PreferredCulture"] != null)
            {
                string UserCulture = Session["PreferredCulture"].ToString();
                if (UserCulture != "")
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(UserCulture);
                }
            }
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                RedirectToLogin();
                return;
            }
        }


        public void RedirectToLogin()
        {
            Session.Abandon();
            Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('../Login.aspx','_top')</SCRIPT>");
            return;
        }
    }
}