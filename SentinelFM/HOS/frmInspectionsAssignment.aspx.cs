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
using System.IO;

namespace SentinelFM
{
    public partial class HOS_frmInspectionsAssignment : SentinelFMBasePage
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
        public string inspectionFormsJson = "";

        public string msgFailedtoSave = "Failed to save";
        public string msgFailedtoLoadData = "Failed to load data.";
        public string OrganizationDOT = "";
        public string OrganizationType = "";
        string node = "";
        string fleetid = "";
        string fleetname = "";
        clsHOSManager hosManager = new clsHOSManager();
        string SelectVehicle = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

            sn.Report.ReportActiveTab = 0;

            ShowOrganizationHierarchy = true;

            poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

           

            if (sn.User.LoadVehiclesBasedOn == "hierarchy")
            {
                fleetTable.Visible = false;
            }
            else
            {
                OrganizationHierarchyTable.Visible = false;
                //cboFleet.Attributes.Add("onchange", "javascript: LoadOnFleetChange(this);");
            }            

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
                    if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                    {
                        DefaultOrganizationHierarchyNodeCode = defaultnodecode;
                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                        hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;
                        hidOrganizationHierarchyFleetId.Value = DefaultOrganizationHierarchyFleetId.ToString();
                        hidOrganizationHierarchyFleetName.Value = DefaultOrganizationHierarchyFleetName;
                        GetInspectionForms();
                    }
                    else
                    {
                        CboFleet_Fill(); // To load the fleet dropdown 
                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
                        GetInspectionForms();
                    }
                }
                else
                {
                    if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                    {
                        hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());

                        DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();

                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                    }
                    else
                        GetInspectionForms();

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


        protected void cboVehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cboVehicle.SelectedValue) != -1)
            {
                this.cboVehicle.Visible = true;
                this.lblVehicleName.Visible = true;
                //CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedValue));
                hdnBoxId.Value = cboVehicle.SelectedValue;
                hdnVehicleDescription.Value = cboVehicle.SelectedItem.Text;
                lblAssignedtoFleet.Text = cboVehicle.SelectedItem.Text;
            }
            else
                hdnFleetId.Value = cboFleet.SelectedValue;
        }

        protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cboFleet.SelectedValue) != -1)
            {
                this.cboVehicle.Visible = true;
                this.lblVehicleName.Visible = true;
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedValue));
                DefaultOrganizationHierarchyFleetId = Int32.Parse(cboFleet.SelectedValue);

                DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                lblAssignedtoFleet.Text = DefaultOrganizationHierarchyFleetName;
                fleetid = cboFleet.SelectedValue;
                fleetname = DefaultOrganizationHierarchyFleetName;
                node = "1";
                hdnNodeCode.Value = node;
                hdnFleetId.Value = DefaultOrganizationHierarchyFleetId.ToString();
                hdnFleetName.Value = DefaultOrganizationHierarchyFleetName;
            }
            else
            {
                hdnFleetId.Value = "";
                hdnFleetName.Value = "";
                lblAssignedtoFleet.Text = "";
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedValue));
            }
        }

        private void CboVehicle_Fill(int fleetId)
        {
            try
            {
                cboVehicle.Items.Clear();
                string SelectVehicle = "Select a Vehicle";
                DataSet dsVehicle = new DataSet();

                string xml = "";

                using (ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet())
                {
                    if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                        if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
                            return;
                        }
                }

                if (String.IsNullOrEmpty(xml))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    //cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
                    return;
                }

                dsVehicle.ReadXml(new StringReader(xml));

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();
                cboVehicle.SelectedIndex = -1;
                cboVehicle.Items.Insert(0, new ListItem(SelectVehicle, "-1"));

                //if ((sn.Report.VehicleId != 0) && (sn.Report.VehicleId != -1))
                //cboVehicle.SelectedIndex = cboVehicle.Items.IndexOf(cboVehicle.Items.FindByValue(sn.Report.VehicleId.ToString()));


            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }

        private void CboFleet_Fill()
        {
            try
            {
                string select = "Select a fleet";
                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();
                cboFleet.Items.Insert(0, new ListItem(select, "-1"));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

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

        private void GetInspectionForms()
        {
            DataTable dt = hosManager.GetLogData_InspectionGroup(sn.User.OrganizationId);
            List<Dictionary<String, String>> forms = new List<Dictionary<string,string>>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Name"] != DBNull.Value && dr["Name"].ToString().Trim() != "")
                    {
                        Dictionary<String, String> form = new Dictionary<string, string>();
                        form.Add("Name", dr["Name"].ToString());
                        form.Add("GroupId", dr["GroupId"].ToString());
                        forms.Add(form);
                    }
                }
            }
            //DataTable dt_assign = hosManager.GetInspectionGroupByHierarchy(
            JavaScriptSerializer js = new JavaScriptSerializer();
            inspectionFormsJson = js.Serialize(forms);
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
        public static string GetInspectionGroupByHierarchy(string currentSelection, Boolean isBox, bool isDOT, int dotType, int fleetId)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                clsHOSManager hosManager = new clsHOSManager();
                DataTable dt = hosManager.GetInspectionGroupByHierarchy(sn.User.OrganizationId, (!isBox && !isDOT) ? currentSelection : "", isBox ? int.Parse(currentSelection) : -1, isDOT ? currentSelection : null, dotType, fleetId);
                List<Dictionary<String, String>> forms = new List<Dictionary<string, string>>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["Name"] != DBNull.Value && dr["Name"].ToString().Trim() != "")
                        {
                            Dictionary<String, String> form = new Dictionary<string, string>();
                            form.Add("Name", dr["Name"].ToString());
                            form.Add("GroupId", dr["GroupId"].ToString());
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetInspectionGroupByHierarchy() Page:frmInspectionsAssignment" + ex.Message));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "";
        }

        [WebMethod]
        public static string AddOrUpdateLogData_InspectionGroupAssignment(int groupId, string currentSelection, Boolean isBox, bool isAdd, bool isDOT, int dotType,int fleetId)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                clsHOSManager hosManager = new clsHOSManager();
                 hosManager.AddOrUpdateLogData_InspectionGroupAssignment(groupId,
                    isBox ? int.Parse(currentSelection) : -1,
                    (!isBox && !isDOT) ? currentSelection : "",
                    sn.User.OrganizationId, 
                    isAdd,
                    isDOT ? currentSelection : null, dotType, fleetId);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddOrUpdateInspectionGroup() Page:frmInspectionsAssignment"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "";
        }

    }
}
