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
using System.Collections;
using System.IO;

namespace SentinelFM
{
    public class InspectionBarCode
    {
        public int BarCode{get;set;}
        public int InspectionId { get; set; }
        public Boolean IsCategory { get; set; }
        public Boolean isAdd {get;set;}
    }

    public partial class HOS_frmPrintQrCode : SentinelFMBasePage
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
        public string inspectionFormsJson = "";
        public string OrganizationDOT = "";
        public string msgFailedtoLoadData = "Failed to load data.";
        public string msgSelectVehicle = "Select a vehicle.";
        public string msgNodataFound = "No data found.";
        public string msgNQRcodeDefined = "No QR code defined.";
        public string msgVehicleInspectionForm = "Inspection form for ";
        clsHOSManager hosManager = new clsHOSManager();
        public string msgQuestionSet = "Question Form";
       
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
                    }
                    else
                    {
                        CboFleet_Fill(); // To load the fleet dropdown 
                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
                        
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

                }

                hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                OrganizationHierarchyPath = getPathByNodeCode(OrganizationHierarchyNodeCode.Value);
                btnPrintQRCode.Attributes.Add("tag", "dynamicInspec");
                btnShowQRCode.Attributes.Add("tag", "dynamicInspec");
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
        public static string GetInspectionCategory(int boxId, Boolean isPrint)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                clsHOSManager hosManager = new clsHOSManager();
                DataSet dtCategorys = hosManager.GetInspectionCategory(boxId);

                if (dtCategorys == null && dtCategorys.Tables.Count <= 0) return "";
                DataTable dtCategory = dtCategorys.Tables[0];
                if (dtCategory == null || dtCategory.Rows.Count == 0) return "";

                List<InspectionBarCode> barcodes = new List<InspectionBarCode>();

                if (dtCategorys.Tables.Count >= 2)
                {
                    foreach (DataRow dr in dtCategorys.Tables[1].Rows)
                    {
                        InspectionBarCode inspectionBarCode = new InspectionBarCode();
                        if (dr["BarCode"] != DBNull.Value) inspectionBarCode.BarCode = (int)dr["BarCode"];
                        if (dr["InspectionId"] != DBNull.Value) inspectionBarCode.InspectionId = (int)dr["InspectionId"];
                        if (dr["IsCategory"] != DBNull.Value) inspectionBarCode.IsCategory = (Boolean)dr["IsCategory"];
                        barcodes.Add(inspectionBarCode);
                    }
                }

                Boolean scannable = false;
                int categoryId = 0;
                String category = "";
                int barcode = -1;
                List<Dictionary<string, string>> categoryDic = new List<Dictionary<string, string>>();
                List<int> categoryIds = new List<int>();
                List<int> existentsBarCode = new List<int>();
                foreach (DataRow dr in dtCategory.Rows)
                {
                    Dictionary<string, string> categoryRow = new Dictionary<string, string>();
                    scannable = false;
                    categoryId = 0;
                    category = "";
                    barcode = -1;

                    if (dr["Scannable"] != DBNull.Value) scannable = (Boolean)dr["Scannable"];
                    if (dr["CategoryID"] != DBNull.Value) categoryId = (int)dr["CategoryID"];
                    if (dr["Category"] != DBNull.Value) category = (String)dr["Category"];
                    if (categoryIds.Contains(categoryId)) continue;
                    else categoryIds.Add(categoryId);
                    if (scannable)
                    {
                        barcode = GetBarCode(categoryId, true, barcodes);
                    }

                    categoryRow.Add("ID", dr["CategoryID"].ToString());
                    categoryRow.Add("Defect", category);
                    if (barcode > 0)
                    {
                        categoryRow.Add("BarCode", barcode.ToString());
                        existentsBarCode.Add(barcode);
                    }
                    else
                        categoryRow.Add("BarCode", "");

                    categoryDic.Add(categoryRow);
                }
                
                StringBuilder barCodeSB = new StringBuilder();
                if (isPrint)
                {
                    foreach (int var_barcode in existentsBarCode)
                    {
                        if (var_barcode > 0) barCodeSB.Append(",").Append(var_barcode);
                        else barCodeSB.Append(var_barcode);

                    }
                    /*foreach (InspectionBarCode var_barcode in barcodes)
                    {
                        if (var_barcode.BarCode > 0)
                        {
                            if (barCodeSB.Length > 0) barCodeSB.Append(",").Append(var_barcode.BarCode);
                            else barCodeSB.Append(var_barcode.BarCode);
                        }
                    }*/
                    return barCodeSB.ToString();
                }


                DataTable dtInspectionItem = hosManager.GetInspectionItem(boxId);
                List<Dictionary<String, String>> inspectionDic = new List<Dictionary<string, string>>();
                foreach (DataRow dr in dtInspectionItem.Rows)
                {
                    Dictionary<String, String> inspection = new Dictionary<string, string>();
                    inspection.Add("ID", dr["InspectionItemID"].ToString());
                    inspection.Add("Defect", dr["Defect"] is DBNull ? "" : dr["Defect"].ToString());
                    string parentItemID = "";
                    if (!(dr["ParentItemID"] is DBNull)) parentItemID = dr["ParentItemID"].ToString();
                    inspection.Add("ParentItemID", parentItemID);
                    inspection.Add("CategoryID", dr["CategoryID"].ToString());

                    inspectionDic.Add(inspection);

                }

                foreach (InspectionBarCode var_barcode in barcodes)
                {
                    if (var_barcode.isAdd && var_barcode.BarCode > 0)
                    {
                        barCodeSB.Append(
                           String.Format("<BarCodeT><Boxid>{0}</Boxid>" +
                              "<BarCode>{1}</BarCode>" +
                              "<InspectionId>{2}</InspectionId>" +
                              "<IsCategory>{3}</IsCategory>" +
                              "</BarCodeT>", boxId, var_barcode.BarCode, var_barcode.InspectionId, var_barcode.IsCategory ? "True" : "False")
                        );
                    }
                }
                if (barCodeSB.Length > 0)
                {
                    barCodeSB.Insert(0, "<ROOT>");
                    barCodeSB.Append("</ROOT>");
                    hosManager.AddOrUpdateBarcode(boxId, barCodeSB.ToString());
                }

                ArrayList list = new ArrayList();
                list.Add(categoryDic);
                list.Add(inspectionDic);
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Serialize(list);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddOrUpdateInspectionGroup() Page:HOS_frmPrintQrCode"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "";
        }


        protected void cboVehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cboVehicle.SelectedValue) != -1)
            {
                this.cboVehicle.Visible = true;
                this.lblVehicleName.Visible = true;
                hidSelectedFleet.Value = cboVehicle.SelectedValue;
                hidSelectedText.Value = cboVehicle.SelectedItem.Text;
            }
            else
                hidSelectedFleet.Value = "";
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
                //hidSelectedFleet.Value = DefaultOrganizationHierarchyFleetId.ToString();
            }
            else
            {
                hidSelectedFleet.Value = "";
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedValue));
            }
        }

        private static int GetBarCode(int inspectionId, Boolean isCategory, List<InspectionBarCode> barCodes)
        {
            //if barcode is not in the list, then add barcode for the maxiumn number 
            int maxBarcode = 0;
            foreach (InspectionBarCode barcode in barCodes)
            {
                if (barcode.InspectionId == inspectionId &&
                    barcode.IsCategory == isCategory)
                    return barcode.BarCode;
                if (maxBarcode < barcode.BarCode) maxBarcode = barcode.BarCode;
            }

            InspectionBarCode newBarcode = new InspectionBarCode();
            maxBarcode = maxBarcode + 1;
            newBarcode.BarCode = maxBarcode;
            newBarcode.InspectionId = inspectionId;
            newBarcode.isAdd = true;
            newBarcode.IsCategory = isCategory;
            barCodes.Add(newBarcode);
            return maxBarcode;
        }
    }
}