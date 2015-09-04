using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Configuration;
using VLF.CLS.Def;
using Com.Mapsolute.Webservices.MapletRemoteControl;

namespace SentinelFM.Configuration
{

    public partial class GeoZone_Landmarks_frmFleetLandmark : SentinelFMBasePage
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

        public bool ShowOrganizationHierarchy;
        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        //public string LoadVehiclesBasedOn = "fleet";
        public string OrganizationHierarchyPath = "";
        VLF.PATCH.Logic.PatchOrganizationHierarchy poh;
        VLF.PATCH.Logic.PatchLandmark pog;
        public bool MutipleUserHierarchyAssignment = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                RedirectToLogin();
                return;
            }

            string defaultnodecode = string.Empty;

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

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

                /*if (Convert.ToInt16(rowItem["PreferenceId"]) == 36)
                {
                    string d = rowItem["PreferenceValue"].ToString().ToLower();
                    if (d == "hierarchy")
                        LoadVehiclesBasedOn = "hierarchy";
                    else
                        LoadVehiclesBasedOn = "fleet";
                }*/
            }

            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

            poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
            pog = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
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
                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                    OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);
                    hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;

                    if (MutipleUserHierarchyAssignment)
                    {
                        hidOrganizationHierarchyFleetId.Value = sn.User.PreferFleetIds;
						hidOrganizationHierarchyNodeCode.Value = sn.User.PreferNodeCodes;
                        //DefaultOrganizationHierarchyFleetId = sn.User.PreferFleetIds;
                    }
                }
                else
                {
                    DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();
                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
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

                //btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == string.Empty ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName;
                lblFleetTitle.Visible = false;
                cboFleet.Visible = false;
                //valFleet.Enabled = false;
                lblOhTitle.Visible = true;
                btnOrganizationHierarchyNodeCode.Visible = true;
                hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);

                //if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyNodeCode.Value.Contains(","))
                //    btnOrganizationHierarchyNodeCode.Text = GetLocalResourceObject("ResMultipleHierarchies").ToString(); // "Multiple Hierarchies";
            }

            if ((sn == null) || (sn.UserName == ""))
            {
                RedirectToLogin();
                return;
            }

            if (!Page.IsPostBack)
            {
                GuiSecurity(this);

                CboFleet_Fill();
                ViewState["ConfirmDelete"] = "0";

                int fleetId;
                if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                    fleetId = DefaultOrganizationHierarchyFleetId;
                else
                    fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);

                if (fleetId != -1)
                {
                    cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                    lstUnAssLandmarks_Fill(fleetId);
                    lstAssLandmarks_Fill(fleetId);
                }
            }
        }

        protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            {
                this.tblVehicles.Visible = true;
                lstUnAssLandmarks_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
                lstAssLandmarks_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
            }
            else
            {
                this.lstAss.Items.Clear();
                this.lstUnAss.Items.Clear();
            }
        }

        protected void hidOrganizationHierarchyPostBack_Click(object sender, System.EventArgs e)
        {
            if (Convert.ToInt32(hidOrganizationHierarchyFleetId.Value) != -1)
            {
                this.tblVehicles.Visible = true;
                lstUnAssLandmarks_Fill(Convert.ToInt32(hidOrganizationHierarchyFleetId.Value));
                lstAssLandmarks_Fill(Convert.ToInt32(hidOrganizationHierarchyFleetId.Value));
            }
            else
            {
                this.lstAss.Items.Clear();
                this.lstUnAss.Items.Clear();
            }
        }

        protected void cmdAdd_Click(object sender, System.EventArgs e)
        {
            try
            {
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

                this.lblMessage.Text = "";

                int fleetId;
                if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                    fleetId = DefaultOrganizationHierarchyFleetId;
                else
                    fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);

                if (fleetId == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");// "Please select a fleet.";
                    return;
                }

                if (this.lstUnAss.SelectedIndex == -1)
                {
                    this.lblMessage.Visible = true;
                    //this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectVehicle");  //"Please select a vehicle.";
                    this.lblMessage.Text = "Please select a Landmark";  //"Please select a vehicle.";
                    return;
                }

                foreach (ListItem li in lstUnAss.Items)
                {
                    if (li.Selected)
                    {
                        /*if (objUtil.ErrCheck(dbf.AddVehicleToFleet(sn.UserID, sn.SecId, Convert.ToInt32(cboToFleet.SelectedItem.Value), Convert.ToInt32(li.Value)), false))
                            if (objUtil.ErrCheck(dbf.AddVehicleToFleet(sn.UserID, sn.SecId, Convert.ToInt32(cboToFleet.SelectedItem.Value), Convert.ToInt32(li.Value)), true))
                            {
                                return;
                            }
                        */
                        pog.AssignLandmarkToFleet(sn.User.OrganizationId, Convert.ToInt32(li.Value), fleetId);
                    }
                }

                lstUnAssLandmarks_Fill(fleetId);
                lstAssLandmarks_Fill(fleetId);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " From:frmFleetLandmark.aspx"));
            }
        }

        protected void cmdAddAll_Click(object sender, System.EventArgs e)
        {
            try
            {
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

                this.lblMessage.Text = "";

                int fleetId;
                if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                    fleetId = DefaultOrganizationHierarchyFleetId;
                else
                    fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);

                if (fleetId == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");// "Please select a fleet.";
                    return;
                }

                foreach (ListItem li in lstUnAss.Items)
                {
                    pog.AssignLandmarkToFleet(sn.User.OrganizationId, Convert.ToInt32(li.Value), fleetId);
                }

                lstUnAssLandmarks_Fill(fleetId);
                lstAssLandmarks_Fill(fleetId);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " From:frmFleetLandmark.aspx"));
            }
        }

        protected void cmdClearAll_Click(object sender, System.EventArgs e)
        {
            try
            {
                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

                this.lblMessage.Text = "";

                int fleetId;
                if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                    fleetId = DefaultOrganizationHierarchyFleetId;
                else
                    fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);

                if (fleetId == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");// "Please select a fleet.";
                    return;
                }

                foreach (ListItem li in lstAss.Items)
                {
                    pog.UnassignLandmarkFromFleet(sn.User.OrganizationId, Convert.ToInt32(li.Value), fleetId);
                }

                lstUnAssLandmarks_Fill(fleetId);
                lstAssLandmarks_Fill(fleetId);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " From:frmFleetLandmark.aspx"));
            }
        }

        protected void cmdRemove_Click(object sender, EventArgs e)
        {
            try
            {
                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

                this.lblMessage.Text = "";

                int fleetId;
                if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                    fleetId = DefaultOrganizationHierarchyFleetId;
                else
                    fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);

                if (fleetId == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");// "Please select a fleet.";
                    return;
                }

                if (this.lstAss.SelectedIndex == -1)
                {
                    this.lblMessage.Visible = true;
                    //this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectVehicle");  //"Please select a vehicle.";
                    this.lblMessage.Text = "Please select a Landmark";  //"Please select a vehicle.";
                    return;
                }

                foreach (ListItem li in lstAss.Items)
                {
                    if (li.Selected)
                    {
                        pog.UnassignLandmarkFromFleet(sn.User.OrganizationId, Convert.ToInt32(li.Value), fleetId);
                    }
                }

                lstUnAssLandmarks_Fill(fleetId);
                lstAssLandmarks_Fill(fleetId);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " From:frmFleetLandmark.aspx"));
            }
        }

        private void CboFleet_Fill()
        {
            try
            {
                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();

                cboFleet.Items.Insert(0, new ListItem((string)this.GetLocalResourceObject("cboFleet_Item_0"), "-1"));
                

                foreach (ListItem li in cboFleet.Items)
                {
                    if (li.Text == "All Vehicles" || li.Text == "Tous les véhicules" || li.Text == "Tous les vehicules")
                    {
                        cboFleet.Items.Remove(li);
                        break;
                    }
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void lstUnAssLandmarks_Fill(int fleetId)
        {
            DataTable dt = new DataTable();
            DataRow[] drCollections = null;

            if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyFleetId.Value.Contains(","))
                return;

            try
            {
                DataSet dsAllUnassignedToFleetLandmarks = new DataSet();
                dsAllUnassignedToFleetLandmarks = pog.GetAllUnassignedToFleetLandmarksInfo(sn.User.OrganizationId, fleetId, sn.UserID);

                if (sn.User.ControlEnable(sn, 92))
                {
                    dt = dsAllUnassignedToFleetLandmarks.Tables[0].Clone();

                    string filter = String.Format("Public=false AND CreateUserID={0}", sn.UserID.ToString());
                    drCollections = dsAllUnassignedToFleetLandmarks.Tables[0].Select(filter);

                    if (drCollections != null && drCollections.Length > 0)
                    {
                        foreach (DataRow dr in drCollections)
                            dt.ImportRow(dr);
                    }

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    this.lstUnAss.DataSource = dt;
                    this.lstUnAss.DataBind();
                }
                else
                {
                    this.lstUnAss.DataSource = dsAllUnassignedToFleetLandmarks;
                    this.lstUnAss.DataBind();
                } 
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " From:frmFleetLandmark.aspx"));
            }
        }

        private void lstAssLandmarks_Fill(int fleetId)
        {
            DataTable dt = new DataTable();
            DataRow[] drCollections = null;

            if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyFleetId.Value.Contains(","))
                return;

            try
            {
                DataSet dsAllAssignedToFleetLandmarks = new DataSet();
                dsAllAssignedToFleetLandmarks = pog.GetAllAssignedToFleetLandmarksInfo(sn.User.OrganizationId, fleetId);
                
                if (sn.User.ControlEnable(sn, 92))
                {
                    dt = dsAllAssignedToFleetLandmarks.Tables[0].Clone();

                    string filter = String.Format("Public=false AND CreateUserID={0}", sn.UserID.ToString());
                    drCollections = dsAllAssignedToFleetLandmarks.Tables[0].Select(filter);

                    if (drCollections != null && drCollections.Length > 0)
                    {
                        foreach (DataRow dr in drCollections)
                            dt.ImportRow(dr);
                    }

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    this.lstAss.DataSource = dt;
                    this.lstAss.DataBind();
                }
                else
                {
                    this.lstAss.DataSource = dsAllAssignedToFleetLandmarks;
                    this.lstAss.DataBind();
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " From:frmFleetLandmark.aspx"));
            }
        }
    }
}