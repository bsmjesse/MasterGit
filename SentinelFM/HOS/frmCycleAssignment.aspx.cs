using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Data;
using System.Web.Services;
using System.Text;
using System.Web.Script.Services;

namespace SentinelFM
{
    public partial class HOS_frmCycleAssignment : SentinelFMBasePage
    {
        public bool OrganizationHierarchySelectVehicle = false;

        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string OrganizationHierarchyPath = "";
        public string PreferOrganizationHierarchyNodeCode = string.Empty;
        public bool IniHierarchyPath = false;
        public bool ShowOrganizationHierarchy;
        public bool MutipleUserHierarchyAssignment = false;
        private bool hiddenHierarchy;
        private VLF.PATCH.Logic.PatchOrganizationHierarchy poh;
        public int VehiclePageSize = 10;
        public string msgVehicle = "Vehicle";
        public string msgSearchResult = "Search Result";
        public string msgSelectAfleet = "Please select a fleet or vehcile to assign.";
        public string msgSelectQuestionet = "Please select a question set.";
        public string cyclesJson = "";

        public string msgFailedtoSave = "Failed to save";
        public string msgFailedtoLoadData = "Failed to load data.";
        public string OrganizationDOT = "";
        public string OrganizationType = "";
        public string msgCycle = "Cycle-";
        public string msgException = "Exception-";
        clsHOSManager hosManager = new clsHOSManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

            sn.Report.ReportActiveTab = 0;

            ShowOrganizationHierarchy = true;

            poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

            if (ShowOrganizationHierarchy)
            {
                //MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
                if (ConfigurationManager.AppSettings["VehicleListTreePageSize"] != null)
                    int.TryParse(ConfigurationManager.AppSettings["VehicleListTreePageSize"].ToString(), out VehiclePageSize);

                clsUtility objUtil;
                objUtil = new clsUtility(sn);
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                string defaultnodecode = string.Empty;

                string xml = "";
                if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                    {

                    }


                /*StringReader strrXML = new StringReader(xml);
                DataSet dsPref = new DataSet();
                dsPref.ReadXml(strrXML);

                foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                {

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode))
                    {
                        string nodecode = rowItem["PreferenceValue"].ToString().TrimEnd();
                        poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                        //Devin added
                        if (!Page.IsPostBack)
                            OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, nodecode);
                        else
                            OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, Request.Form["OrganizationHierarchyNodeCode"].ToString());

                        defaultnodecode = rowItem["PreferenceValue"].ToString().TrimEnd();

                    }

                }*/
                defaultnodecode = sn.User.PreferNodeCodes;


                defaultnodecode = defaultnodecode ?? string.Empty;
                if (defaultnodecode == string.Empty)
                {
                    if (sn.RootOrganizationHierarchyNodeCode == string.Empty)
                    {
                        defaultnodecode = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID, MutipleUserHierarchyAssignment);
                        sn.RootOrganizationHierarchyNodeCode = defaultnodecode;
                    }
                    else
                        defaultnodecode = sn.RootOrganizationHierarchyNodeCode;
                }
                PreferOrganizationHierarchyNodeCode = defaultnodecode;
                if (!IsPostBack)
                {
                    DefaultOrganizationHierarchyNodeCode = defaultnodecode;
                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                    hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;
                    hidOrganizationHierarchyFleetId.Value = DefaultOrganizationHierarchyFleetId.ToString();
                    hidOrganizationHierarchyFleetName.Value = DefaultOrganizationHierarchyFleetName;
                    GellAllCyclesandExceptions();
                }
                else
                {
                    hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());

                    DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();

                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);

                }
                //OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);



                hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                //ReportBasedOption();
                OrganizationHierarchyPath = getPathByNodeCode(OrganizationHierarchyNodeCode.Value);
                btnAssign.Attributes.Add("tag", "dynamicInspec");
                btnUnassign.Attributes.Add("tag", "dynamicInspec");
                GetOrganizationDOT();
                GetOrganizationVehicleType();
            }
        }

        private void GetOrganizationDOT()
        {
            DataTable dt = hosManager.GetOrganizationDOT(sn.User.OrganizationId);
            StringBuilder sb = new StringBuilder();
            //string newQuote1 = @"\""";
            string str = "";
            var index = 1;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["DotNbr"] != DBNull.Value && dr["DotNbr"].ToString().Trim() != "")
                {
                    str = GetScriptEscapeString(dr["DotNbr"].ToString().Trim());
                    //str.Replace(@"""", newQuote1);

                    sb.Append(
                    string.Format("<li id = '{2}' class='directory collapsed' style='margin-left:20px;display:none'><a id = {0} href='#' onclick=btnDot_Click('{0}')>{1}</a></li>", "lia_dot_" + index.ToString(), str, "li_dot_" + index.ToString()));
                    index = index + 1;
                }
            }
            if (sb.Length > 0)
            {
                sb.Insert(0, "<li class='directory collapsed'  ><a id = 'lia_dot_all' href='#' onclick=btnDot_All_Click()>DOT</a></li>");
                OrganizationDOT = "<ul class='jqueryFileTree' id='ul_dot_all' >" + sb.ToString() + "</ul>";
            }
        }

        private void GetOrganizationVehicleType()
        {
            DataTable dt = hosManager.GetOrganizationVehicleType(sn.User.OrganizationId);
            StringBuilder sb = new StringBuilder();
            //string newQuote1 = @"\""";
            string str = "";
            var index = 1;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["ObjectType"] != DBNull.Value && dr["ObjectType"].ToString().Trim() != "")
                {
                    str = GetScriptEscapeString(dr["ObjectType"].ToString().Trim());
                    //str.Replace(@"""", newQuote1);

                    sb.Append(
                    string.Format("<li id = '{2}' class='directory collapsed' style='margin-left:20px;display:none') ><a id = {0} href='#' onclick=btnObjectType_Click('{0}')>{1}</a></li>", "lia_type_" + index.ToString(), str, "li_type_" + index.ToString()));
                    index = index + 1;
                }
            }
            if (sb.Length > 0)
            {
                sb.Insert(0, "<li class='directory collapsed'  ><a id = 'lia_type_all' href='#' onclick=btnObjectType_All_Click()>Vehicle Type</a></li>");
                OrganizationType = "<ul class='jqueryFileTree' id='ul_type_all'>" + sb.ToString() + "</ul>";
            }
        }

        private void GellAllCyclesandExceptions()
        {
            DataTable dt = hosManager.GellAllCyclesandExceptions(null);
            List<Dictionary<String, String>> forms = new List<Dictionary<string, string>>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Name"] != DBNull.Value && dr["Name"].ToString().Trim() != "")
                    {
                        Dictionary<String, String> form = new Dictionary<string, string>();
                        if (dr["Type"].ToString() == "1")
                        {
                            form.Add("Name", msgCycle + dr["Name"].ToString());
                        }
                        if (dr["Type"].ToString() == "2")
                        {
                            form.Add("Name", msgException + dr["Name"].ToString());
                        }
                        form.Add("Rule", dr["RuleId"].ToString() + "-" + dr["Type"].ToString());
                        forms.Add(form);
                    }
                }
            }
            //DataTable dt_assign = hosManager.GetInspectionGroupByHierarchy(
            JavaScriptSerializer js = new JavaScriptSerializer();
            cyclesJson = js.Serialize(forms);
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

        [WebMethod]
        public static string GetCycleAndExceptionByHierarchy(string currentSelection, Boolean isBox, bool isDOT, int dotType)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                clsHOSManager hosManager = new clsHOSManager();
                DataTable dt = hosManager.GetCycleAndExceptionByHierarchy (sn.User.OrganizationId, (!isBox && !isDOT) ? currentSelection : "", isBox ? int.Parse(currentSelection) : -1, isDOT ? currentSelection : null, dotType);
                List<Dictionary<String, String>> forms = new List<Dictionary<string, string>>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["Name"] != DBNull.Value && dr["Name"].ToString().Trim() != "")
                        {
                            Dictionary<String, String> form = new Dictionary<string, string>();
                            form.Add("Name", dr["Name"].ToString());
                            form.Add("Rule", dr["RuleId"].ToString() + "-" + dr["RuleType"].ToString());
                            forms.Add(form);
                        }
                    }
                }
                //DataTable dt_assign = hosManager.GetInspectionGroupByHierarchy(
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Serialize(forms);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddOrUpdateInspectionGroup() Page:frmDynamicInspections"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "";
        }

        [WebMethod]
        public static string AddOrUpdateLogData_CycleAndExceptionpAssignment(string rule, string currentSelection, Boolean isBox, bool isAdd, bool isDOT, int dotType)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                string[] rules = rule.Split('-');
                clsHOSManager hosManager = new clsHOSManager();
                hosManager.AddOrUpdateLogData_CycleAssignment(int.Parse(rules[0]),int.Parse(rules[1]),
                   isBox ? int.Parse(currentSelection) : -1,
                   (!isBox && !isDOT) ? currentSelection : "",
                   sn.User.OrganizationId,
                   isAdd,
                   isDOT ? currentSelection : null, dotType);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddOrUpdateInspectionGroup() Page:frmDynamicInspections"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "";
        }
    }
}