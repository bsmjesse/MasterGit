namespace SentinelFM.Components
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.IO;
    using System.Configuration;

    public partial class Configuration_Components_ctlOrganizationHierarchyFleetVehicles : BaseControl
    {

        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public static int IsParentNode = 1;
        public static string AssignedVehiclesList = string.Empty;
        public string OrganizationHierarchyPath = "";

        public bool MutipleUserHierarchyAssignment = false;
       
        private bool ShowEnableEditManagerName = false;

        private VLF.PATCH.Logic.PatchOrganizationHierarchy poh;
        private VLF.PATCH.Logic.PatchFleet pf;
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                pf = new VLF.PATCH.Logic.PatchFleet(sConnectionString);
                this.lblMessage.Visible = false;

                MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
                ShowEnableEditManagerName = clsPermission.FeaturePermissionCheck(sn, "EnableEditManagerName");
                if(ShowEnableEditManagerName == true)
                {
                    lblManagerName.Visible = true;
                    txtboxManagerName.Visible = true;
                    btnSave.Visible = true;
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

                }

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
                    hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;
                    hidOrganizationHierarchyFleetId.Value = DefaultOrganizationHierarchyFleetId.ToString();                   
                   
                                      

                    if (MutipleUserHierarchyAssignment)
                    {
                        hidOrganizationHierarchyFleetId.Value = sn.User.PreferFleetIds;
                        hidOrganizationHierarchyNodeCode.Value = sn.User.PreferNodeCodes;
                        //DefaultOrganizationHierarchyFleetId = sn.User.PreferFleetIds;
                    }
                }
                else
                {
                   
                    hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());
                    btnOrganizationHierarchyNodeCode.Text = hidOrganizationHierarchyNodeCode.Value.ToString();

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
                OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);

                //btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == "" ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName;
                hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);

                //if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyNodeCode.Value.Contains(","))
                //    btnOrganizationHierarchyNodeCode.Text = GetLocalResourceObject("ResMultipleHierarchies").ToString(); // "Multiple Hierarchies";

                if (!Page.IsPostBack)
                {
                    GuiSecurity(this);
                    this.tblVehicles.Visible = false;
                    //CboFleet_Fill();

                    if (sn.User.DefaultFleet != -1)
                    {
                        //cboToFleet.SelectedIndex = cboToFleet.Items.IndexOf(cboToFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                        this.tblVehicles.Visible = true;
                        lstUnAssVehicles_Fill();
                        lstAssVehicles_Fill();
                        if (ShowEnableEditManagerName == true)
                        {
                            EnableEditManagerName();
                        }
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmVehicleFleet.aspx"));
            }
        }

        private void EnableEditManagerName()
        {
            try
            {
                if (!string.IsNullOrEmpty(DefaultOrganizationHierarchyNodeCode))
                {
                    IsParentNode = poh.CheckWhetherOrganizationHierarchyNodeCodeIsLeaf(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);                    
                }

                string mgrnamexml = "";               
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();                
               
                if (IsParentNode == 0 )
                {
                    btnSave.Enabled = false;
                    txtboxManagerName.Enabled = false;
                    if (this.lstAss.Items.Count > 0)
                    {
                        btnCancel.Visible = false;
                        this.btnSave.Text = GetLocalResourceObject("cmdEditManagerName").ToString();
                        int FirstVehicleID = Convert.ToInt32(sn.History.DsAssOHVehicle.Tables[0].Rows[0]["VehicleId"]);
                        if (objUtil.ErrCheck(dbf.GetManagerNameByVehicleId(sn.User.OrganizationId, FirstVehicleID, ref mgrnamexml), false))
                            if (objUtil.ErrCheck(dbf.GetManagerNameByVehicleId(sn.User.OrganizationId, FirstVehicleID, ref mgrnamexml), true))
                            {
                                this.lstAss.Items.Clear();
                                this.lblMessage.Visible = true;
                                txtboxManagerName.Text = "";
                                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessageError");// "Some problem occured.";
                                return;
                            }

                        if (string.IsNullOrEmpty(mgrnamexml))
                        {
                            this.lstAss.Items.Clear();
                            string errorMessage = this.lblMessage.Text = (string)base.GetLocalResourceObject("ErrorGettingManagerName");
                            errorMessage = errorMessage + "for vehicle Id = " + FirstVehicleID + " Form:frmVehicleFleet.aspx";
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, errorMessage));
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessageError");// "Some problem occured.";
                            txtboxManagerName.Text = "";
                            return;
                        }
                        StringReader strXML = new StringReader(mgrnamexml);
                        DataSet dsManagerName = new DataSet();
                        dsManagerName.ReadXml(strXML);
                        DataColumnCollection columns = dsManagerName.Tables[0].Columns;
                        string ManagerName = "";
                        if (columns.Contains("ManagerName"))
                        {
                            ManagerName = Convert.ToString((dsManagerName.Tables[0].Rows[0]["ManagerName"]));
                            btnSave.Enabled = true;
                            txtboxManagerName.Enabled = false;
                            txtboxManagerName.Text = ManagerName;
                        }
                        else
                        {
                            btnSave.Enabled = true;
                            txtboxManagerName.Enabled = false;
                        }
                    }

                }                
                else 
                {
                    txtboxManagerName.Text = "";                    
                    txtboxManagerName.Enabled = false;
                    btnCancel.Visible = false;
                    this.btnSave.Text = GetLocalResourceObject("cmdEditManagerName").ToString();
                    if (this.lstAss.Items.Count > 0)
                    {                        
                        btnSave.Enabled = true; 
                    }
                    else
                    {
                        btnSave.Enabled = false;
                    }
                   
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmVehicleFleet.aspx"));
            }
        }


        /*private void CboFleet_Fill()
        {
            try
            {

                DataSet dsFleets = new DataSet();
                //dsFleets = sn.User.GetUserFleets(sn);
                //dsFleets = poh.GetOrganizationHierarchyAllFleetsByUserId(sn.User.OrganizationId, sn.UserID);
                dsFleets = sn.User.GetUserFleets(sn);
                DataView FleetView = dsFleets.Tables[0].DefaultView;
                FleetView.RowFilter = "FleetType='oh'";
                cboToFleet.DataSource = FleetView;
                cboToFleet.DataBind();
                cboToFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectFleet"), "-1"));

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmVehicleFleet.aspx"));
            }

        }*/


        private void lstUnAssVehicles_Fill()
        {

            try
            {

                //StringReader strrXML = null;
                DataSet dsVehicle = new DataSet();

                /*string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetAllUnassingToFleetVehiclesInfoXML(sn.UserID, sn.SecId, Convert.ToInt32(sn.User.OrganizationId), Convert.ToInt32(cboToFleet.SelectedItem.Value), ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetAllUnassingToFleetVehiclesInfoXML(sn.UserID, sn.SecId, Convert.ToInt32(sn.User.OrganizationId), Convert.ToInt32(cboToFleet.SelectedItem.Value), ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    this.lstUnAss.Items.Clear();
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);*/
                if (Convert.ToInt32(hidOrganizationHierarchyFleetId.Value) != -1)
                {
                    dsVehicle = poh.GetAllUnassingToOrganizationHierarchyFleetVehiclesInfo(sn.User.OrganizationId);
                    sn.History.DsUnAssOHVehicle = dsVehicle;
                    this.lstUnAss.DataSource = dsVehicle;
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmVehicleFleet.aspx"));
            }

        }



        private void lstAssVehicles_Fill()
        {
            try
            {

                StringReader strrXML = null;
                DataSet dsVehicle = new DataSet();
                
                string xml = "";
                
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    this.lstAss.Items.Clear();
                    btnSave.Enabled = false;
                    txtboxManagerName.Enabled = false;
                    txtboxManagerName.Text = "";
                    btnCancel.Visible = false;
                    this.btnSave.Text = GetLocalResourceObject("cmdEditManagerName").ToString();
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);
                sn.History.DsAssOHVehicle = dsVehicle;
                this.lstAss.DataSource = dsVehicle;
                this.lstAss.DataBind();
                
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmVehicleFleet.aspx"));
            }

        }


        protected void hidOrganizationHierarchyPostBack_Click(object sender, System.EventArgs e)
        {

            if (Convert.ToInt32(hidOrganizationHierarchyFleetId.Value) != -1)
            {
                this.tblVehicles.Visible = true;
                lstUnAssVehicles_Fill();
                lstAssVehicles_Fill();
                if (ShowEnableEditManagerName == true)
                {
                    EnableEditManagerName();
                }
              
            }
            else
            {
                this.lstAss.Items.Clear();
                this.lstUnAss.Items.Clear();
            }
        }


        protected void cmdCancel_Click(object sender, System.EventArgs e)
        {
            try
            {
                txtboxManagerName.Enabled = false;
                this.btnSave.Text = GetLocalResourceObject("cmdEditManagerName").ToString();
                btnCancel.Visible = false;
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmVehicleFleet.aspx"));
            }


        } 

        protected void cmdSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (this.btnSave.Text == GetLocalResourceObject("cmdSaveManagerName").ToString())
                {
                    this.btnSave.Text = GetLocalResourceObject("cmdEditManagerName").ToString();
                    txtboxManagerName.Enabled = false;
                    btnCancel.Visible = false;
                    SaveManagerNameForAssignedVehicles();
                }
                else
                {
                    txtboxManagerName.Enabled = true;
                    this.btnSave.Text = GetLocalResourceObject("cmdSaveManagerName").ToString();
                    btnCancel.Visible = true;
                    if (IsParentNode != 0)
                    {
                        this.lblMessage.Visible = true;
                        txtboxManagerName.Enabled = false;
                        this.btnSave.Text = GetLocalResourceObject("cmdEditManagerName").ToString();
                        btnSave.Enabled = false;
                        btnCancel.Visible = false;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessageEditNotApplicable");// "Manager Name could be set only for Leaf-Level organization node.";
                        return;
                    }

                }

            }


            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmVehicleFleet.aspx"));
            }


        }

        private void SaveManagerNameForAssignedVehicles()
        {
            try
            {                
                int rowsAffected = 0;
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
                AssignedVehiclesList = "";
                foreach (DataRow row in sn.History.DsAssOHVehicle.Tables[0].Rows)
                {
                    AssignedVehiclesList += row["VehicleId"].ToString() + ",";
                }
                AssignedVehiclesList = AssignedVehiclesList.Substring(0, AssignedVehiclesList.Length - 1);
                if (!string.IsNullOrEmpty(AssignedVehiclesList))
                {
                    if (objUtil.ErrCheck(dbf.SetManagerNameByVehicleId(sn.User.OrganizationId, AssignedVehiclesList, txtboxManagerName.Text, ref rowsAffected), false))
                        if (objUtil.ErrCheck(dbf.SetManagerNameByVehicleId(sn.User.OrganizationId, AssignedVehiclesList, txtboxManagerName.Text, ref rowsAffected), true))
                        {                            
                            this.lblMessage.Visible = true;                           
                            txtboxManagerName.Enabled = true;
                            this.btnSave.Text = GetLocalResourceObject("cmdSaveManagerName").ToString();
                            btnCancel.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessageError");// "Some problem occured.";
                            return;
                        }



                    if (rowsAffected != 0)
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessageManagerNameSaved");// "Manager Name saved successfully.";
                    }
                    else
                    {                       
                        string errorMessage = this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessageManagerNameNotSaved");
                        errorMessage = errorMessage + "for fleet = " + hidOrganizationHierarchyFleetId.Value + " Form:frmVehicleFleet.aspx";
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, errorMessage));
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessageManagerNameNotSaved");// "Unable to save Manager Name.";
                        txtboxManagerName.Enabled = true;
                        this.btnSave.Text = GetLocalResourceObject("cmdSaveManagerName").ToString();
                        btnCancel.Visible = true;
                        return;
                    }
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmVehicleFleet.aspx"));
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

                if (Convert.ToInt32(hidOrganizationHierarchyFleetId.Value) == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");// "Please select a fleet.";
                    return;
                }


                if (this.lstUnAss.SelectedIndex == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectVehicle");  //"Please select a vehicle.";
                    return;
                }

                foreach (ListItem li in lstUnAss.Items)
                {
                    if (li.Selected)
                    {
                        //if (objUtil.ErrCheck(dbf.AddVehicleToFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(li.Value)), false))
                        //    if (objUtil.ErrCheck(dbf.AddVehicleToFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(li.Value)), true))
                        //    {
                        //        return;
                        //    }
                        if (pf.AddVehicleToFleet(sn.User.OrganizationId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), sn.UserID, Convert.ToInt32(li.Value)) == -1)
                            return;
                        poh.AssignFleetToAllParents(sn.User.OrganizationId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(li.Value));
                    }

                }


                lstUnAssVehicles_Fill();
                lstAssVehicles_Fill();
                if (ShowEnableEditManagerName == true)
                {
                    EnableEditManagerName();
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }


            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmVehicleFleet.aspx"));
            }


        }

        protected void cmdRemove_Click(object sender, System.EventArgs e)
        {
            try
            {
                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

                this.lblMessage.Text = "";

                if (Convert.ToInt32(hidOrganizationHierarchyFleetId.Value) == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");// "Please select a fleet.";
                    return;
                }


                if (this.lstAss.SelectedIndex == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");  //"Please select a vehicle.";
                    return;
                }


                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (!pf.ValidateUserAssignedToFleet(sn.User.OrganizationId, sn.UserID, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value)))
                    return;

                foreach (ListItem li in lstAss.Items)
                {
                    if (li.Selected)
                    {
                        //if (objUtil.ErrCheck(dbf.DeleteVehicleFromFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(li.Value)), false))
                        //    if (objUtil.ErrCheck(dbf.DeleteVehicleFromFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(li.Value)), false))
                        //    {
                        //        return;
                        //    }

                        poh.RemoveVehicleFromFleet(sn.User.OrganizationId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(li.Value));
                    }

                }


                lstUnAssVehicles_Fill();
                lstAssVehicles_Fill();
                if (ShowEnableEditManagerName == true)
                {
                    EnableEditManagerName();
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmVehicleFleet.aspx"));
            }

        }

        protected void cmdAddAll_Click(object sender, System.EventArgs e)
        {
            try
            {
                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

                if (Convert.ToInt32(hidOrganizationHierarchyFleetId.Value) == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");// "Please select a fleet.";
                    return;
                }

                this.lblMessage.Text = "";


                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                DataSet DsUnAssOHVehicle = new DataSet();
                DsUnAssOHVehicle = sn.History.DsUnAssOHVehicle;



                if (DsUnAssOHVehicle != null)
                {
                    foreach (DataRow rowItem in DsUnAssOHVehicle.Tables[0].Rows)
                    {
                        //if (objUtil.ErrCheck(dbf.AddVehicleToFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(rowItem["VehicleId"])), false))
                        //    if (objUtil.ErrCheck(dbf.AddVehicleToFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(rowItem["VehicleId"])), false))
                        //    {
                        //        return;
                        //    }
                        if (pf.AddVehicleToFleet(sn.User.OrganizationId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), sn.UserID, Convert.ToInt32(rowItem["VehicleId"])) == -1)
                            return;

                        poh.AssignFleetToAllParents(sn.User.OrganizationId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(rowItem["VehicleId"]));
                    }

                    lstUnAssVehicles_Fill();
                    lstAssVehicles_Fill();
                    if (ShowEnableEditManagerName == true)
                    {
                        EnableEditManagerName();
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmVehicleFleet.aspx"));
            }

        }

        protected void cmdRemoveAll_Click(object sender, System.EventArgs e)
        {
            try
            {
                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

                if (Convert.ToInt32(hidOrganizationHierarchyFleetId.Value) == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet"); // "Please select a fleet.";
                    return;
                }

                this.lblMessage.Text = "";


                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                DataSet DsAssOHVehicle = new DataSet();
                DsAssOHVehicle = sn.History.DsAssOHVehicle;

                if (!pf.ValidateUserAssignedToFleet(sn.User.OrganizationId, sn.UserID, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value)))
                    return;

                foreach (DataRow rowItem in DsAssOHVehicle.Tables[0].Rows)
                {
                    //if (objUtil.ErrCheck(dbf.DeleteVehicleFromFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(rowItem["VehicleId"])), false))
                    //    if (objUtil.ErrCheck(dbf.DeleteVehicleFromFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(rowItem["VehicleId"])), false))
                    //    {
                    //        return;
                    //    }
                    poh.RemoveVehicleFromFleet(sn.User.OrganizationId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(rowItem["VehicleId"]));
                }

                lstUnAssVehicles_Fill();
                lstAssVehicles_Fill();
                if (ShowEnableEditManagerName == true)
                {
                    EnableEditManagerName();
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmVehicleFleet.aspx"));
            }
        }


        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}