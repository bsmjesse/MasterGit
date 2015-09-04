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
    public partial class UserControl_FleetVehicleOrganizationHierarchy : System.Web.UI.UserControl
    {
        public event EventHandler Fleet_SelectedIndexChanged;
        public event EventHandler OrganizationHierarchySelectChanged;
        public event EventHandler Vehicle_SelectedIndexChanged;

        public RadAjaxManager radAjaxManager1 = null;
        public string radAjaxLoadingPanel1 = null;
        public string radUpdatedControl = null;
        public bool isLoadDefault = false;
        protected SentinelFMSession sn = null;
        clsUtility objUtil = null;

        public bool ShowOrganizationHierarchy;
        public string DefaultOrganizationHierarchyFleetId = "0";
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        //public string LoadVehiclesBasedOn = "fleet";
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
                    defaultnodecode = rowItem["PreferenceValue"].ToString().TrimEnd();

                }

                if (Convert.ToInt16(rowItem["PreferenceId"]) == 36)
                {
                    string d = rowItem["PreferenceValue"].ToString().ToLower();
                    if (d == "hierarchy")
                        LoadVehiclesBasedOn = "hierarchy";
                    else
                        LoadVehiclesBasedOn = "fleet";
                }
            }*/

            defaultnodecode = sn.User.PreferNodeCodes;

            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

            VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
            if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                ShowOrganizationHierarchy = true;
            else
            {
                ShowOrganizationHierarchy = false;
                //LoadVehiclesBasedOn = "fleet";
            }

            if (sn.User.LoadVehiclesBasedOn == "hierarchy")
            {
                MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
                if(MutipleUserHierarchyAssignment)
                    RootOrganizationHierarchyNodeCode = sn.User.PreferNodeCodes;

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
                        DefaultOrganizationHierarchyFleetName = GetLocalResourceObject("ResMultipleHierarchies").ToString();
                    else
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(hidOrganizationHierarchyFleetId.Value));
                }

                OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);

                //btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == string.Empty ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName;

                if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyNodeCode.Value.Contains(","))
                    btnOrganizationHierarchyNodeCode.Text = GetLocalResourceObject("ResMultipleHierarchies").ToString(); // "Multiple Hierarchies";

                lblFleetTitle.Visible = false;
                cboFleet.Visible = false;
                lblOhTitle.Visible = true;
                btnOrganizationHierarchyNodeCode.Visible = true;
                hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
            }
            
            Uri MyUrl = Request.Url;
            RootUrl = Request.Url.Scheme + "://" + Request.Url.Authority +  Request.ApplicationPath.TrimEnd('/') + "/";
            vehicletreeviewIni.Value = "false";
            
            if (!IsPostBack)
            {
                CboFleet_Fill();

                if (Request.QueryString["DashBoardFleetID"] != null && Request.QueryString["DashBoardFleetID"].Trim() != string.Empty)
                {
                    string dashboardFleetID = Request.QueryString["DashBoardFleetID"].ToString();
                    RadComboBoxItem rFleetItem = cboFleet.Items.FindItemByValue(dashboardFleetID);
                    if (rFleetItem != null)
                    {
                        cboFleet.SelectedIndex = rFleetItem.Index;
                        cboFleet_SelectedIndexChanged(null, null);
                        isLoadDefault = false;
                    }
                }

                if (isLoadDefault)
                {
                    if (sn.User.DefaultFleet != -1)
                    {
                        cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindItemByValue(sn.User.DefaultFleet.ToString()));
                    }
                    else this.cboFleet.SelectedIndex = 0;

                    if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                    {
                        if (MutipleUserHierarchyAssignment)
                        {
                            CboVehicle_FillByMultipleFleetIds(DefaultOrganizationHierarchyFleetId);
                        }
                        else
                            CboVehicle_Fill(int.Parse(DefaultOrganizationHierarchyFleetId));
                    }
                    else
                    {
                        CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
                    }
                    
                    this.cboVehicle.SelectedIndex = 0;
                }
            }
            if (Vehicle_SelectedIndexChanged != null)
            {
                cboVehicle.AutoPostBack = true;
                cboVehicle.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(Vehicle_SelectedIndexChanged);
            }
            //SetLoadingPanel();

        }

        private void CboFleet_Fill()
        {
            try
            {
                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();
                cboFleet.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("StringSelectFleet"), "0"));
                //cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void CboVehicle_Fill(int fleetId)
        {
            try
            {
                DataSet dsVehicle;
                dsVehicle = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                    {
                        cboVehicle.Items.Clear();
                        cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("StringSelectVehicle"), "0"));
                        return;
                    }
                if (xml == "")
                {
                    cboVehicle.Items.Clear();
                    cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("StringSelectVehicle"), "0"));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();
                cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("EntireFleet"), "-999"));
                cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("StringSelectVehicle"), "0"));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void CboVehicle_FillByMultipleFleetIds(string fleetIds)
        {
            try
            {
                DataSet dsVehicle;
                dsVehicle = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByMultipleFleetIds(sn.UserID, sn.SecId, fleetIds, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByMultipleFleetIds(sn.UserID, sn.SecId, fleetIds, ref xml), true))
                    {
                        cboVehicle.Items.Clear();
                        cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("StringSelectVehicle"), "0"));
                        return;
                    }
                if (xml == "")
                {
                    cboVehicle.Items.Clear();
                    cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("StringSelectVehicle"), "0"));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();
                cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("EntireFleet"), "-999"));
                cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("StringSelectVehicle"), "0"));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
            else
                this.cboVehicle.Items.Clear();

            if (Fleet_SelectedIndexChanged != null)
            {
                Fleet_SelectedIndexChanged(sender, e);
            }
        }

        private void SetLoadingPanel()
        {
            if (radAjaxManager1 == null || radAjaxLoadingPanel1 == null || radUpdatedControl == null) return;
            if (cboFleet.AutoPostBack && Fleet_SelectedIndexChanged != null)
            {
                AjaxSetting ajs = new AjaxSetting(cboFleet.ID);
                ajs.UpdatedControls.Add(new AjaxUpdatedControl(radUpdatedControl, radAjaxLoadingPanel1));
                radAjaxManager1.AjaxSettings.Add(ajs);
            }

            if (cboVehicle.AutoPostBack)
            {
                AjaxSetting ajs = new AjaxSetting(cboVehicle.ID);
                ajs.UpdatedControls.Add(new AjaxUpdatedControl(radUpdatedControl, radAjaxLoadingPanel1));
                radAjaxManager1.AjaxSettings.Add(ajs);
            }

        }

        public string GetSelectedFleet()
        {
            return cboFleet.SelectedValue;
        }

        public string GetSelectedVehicle()
        {
            return cboVehicle.SelectedValue;
        }

        public string GetAllSelectedVehicle()
        {
            string selectedVehicles = string.Empty;
            if (cboVehicle.SelectedValue != "-999")
            {
                if (cboVehicle.SelectedValue != "0")
                    return cboVehicle.SelectedValue;
                else return string.Empty;
            }
            else
            {
                foreach (RadComboBoxItem radItem in cboVehicle.Items)
                {
                    if (radItem.Value == "-999") continue;
                    if (radItem.Value == "0") continue;
                    if (selectedVehicles == string.Empty) selectedVehicles = radItem.Value;
                    else selectedVehicles = selectedVehicles + "," + radItem.Value;
                }
            }
            return selectedVehicles;
        }


        protected void hidOrganizationHierarchyPostBack_Click(object sender, EventArgs e)
        {
            if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
            {
                string fleetIds = hidOrganizationHierarchyFleetId.Value.ToString().Trim();
                if (fleetIds != string.Empty)
                    CboVehicle_FillByMultipleFleetIds(fleetIds);
                else
                    this.cboVehicle.Items.Clear();
            }
            else
            {
                int fleetId = 0;
                int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
                if (fleetId > 0)
                    CboVehicle_Fill(fleetId);
                else
                    this.cboVehicle.Items.Clear();
            }
            
        }
}
}