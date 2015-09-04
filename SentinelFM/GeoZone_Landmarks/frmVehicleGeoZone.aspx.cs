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
    /// <summary>
    /// Summary description for frmVehicleGeoZone.
    /// </summary>
    public partial class frmVehicleGeoZone : SentinelFMBasePage
    {
        
        
        protected DataSet dsSeverity = new DataSet();

        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

        public bool ShowOrganizationHierarchy;
        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        //public string LoadVehiclesBasedOn = "fleet";
        public string OrganizationHierarchyPath = "";
        VLF.PATCH.Logic.PatchOrganizationHierarchy poh;

        public bool MutipleUserHierarchyAssignment = false;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
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

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);


                if ((sn == null) || (sn.UserName == ""))
                {
                    RedirectToLogin();
                    return;
                }

                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmVehicleGeozoneForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    tblGeoZones.Visible = false;
                    this.tblWait.Visible = false;
                    this.tblGeoZoneHeader.Visible = false;
                    this.tblGeoZoneHeaderUnAss.Visible = false;


                    CboFleet_Fill();
                    DsSeverity_Fill();
                    ViewState["ConfirmDelete"] = "0";

                    int fleetId;
                    if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                        fleetId = DefaultOrganizationHierarchyFleetId;
                    else
                        fleetId = sn.User.DefaultFleet;

                    if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
                    {
                        CboVehicle_MultipleFleet_Fill(hidOrganizationHierarchyFleetId.Value);
                        this.lblVehicleName.Visible = true;
                        this.cboVehicle.Visible = true;
                    }
                    else if (fleetId != -1)
                    {
                        cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                        CboVehicle_Fill(fleetId);
                        this.lblVehicleName.Visible = true;
                        this.cboVehicle.Visible = true;
                    }

                    if ((sn.Cmd.GeoZoneSync) || (sn.Cmd.CommandId == Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.GetGeoZoneIDs)))
                    {
                        cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.GeoZone.SelectedFleetId.ToString()));
                        CboVehicle_Fill(sn.GeoZone.SelectedFleetId);
                        cboVehicle.SelectedIndex = cboVehicle.Items.IndexOf(cboVehicle.Items.FindByValue(sn.GeoZone.SelectedVehicleId.ToString()));
                        cboVehicle.Visible = true;


                        if ((sn.GeoZone.DsVehicleGeoZone == null) || (sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Count == 0))
                        {
                            this.tblGeoZoneHeader.Visible = true;
                            this.dgAssGeoZone.DataSource = null;
                            this.dgAssGeoZone.DataBind();
                        }
                        else
                        {
                            this.tblGeoZoneHeader.Visible = false;
                            this.dgAssGeoZone.DataSource =sn.GeoZone.DsVehicleGeoZone;
                            this.dgAssGeoZone.DataBind();
                        }


                        if ((sn.GeoZone.DsUnAssVehicleGeoZone == null) || (sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows.Count == 0))
                        {
                            this.tblGeoZoneHeaderUnAss.Visible = true;
                            this.dgUnAssGeoZone.DataSource = null;
                            this.dgUnAssGeoZone.DataBind();


                        }
                        else
                        {
                            this.tblGeoZoneHeaderUnAss.Visible = false;
                            this.dgUnAssGeoZone.DataSource =sn.GeoZone.DsUnAssVehicleGeoZone;
                            this.dgUnAssGeoZone.DataBind();
                        }

                        this.tblGeoZones.Visible = true;

                        // Check if not Delete all geozone 
                        if (sn.GeoZone.GeozoneId == VLF.CLS.Def.Const.allGeozones || sn.Cmd.CommandId == Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.GetGeoZoneIDs))
                            StopSync();
                        else
                            SyncGeozone();


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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgUnAssGeoZone.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgUnAssGeoZone_PageIndexChanged);
            this.dgAssGeoZone.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgAssGeoZone_ItemCommand);
            this.dgAssGeoZone.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgAssGeoZone_PageIndexChanged);
            this.dgAssGeoZone.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgAssGeoZone_CancelCommand);
            this.dgAssGeoZone.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgAssGeoZone_EditCommand);
            this.dgAssGeoZone.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgAssGeoZone_UpdateCommand);
            this.dgAssGeoZone.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgAssGeoZone_ItemDataBound);

        }
        #endregion



        protected void cboToFleet_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            /*if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            {

                this.cboVehicle.Visible = true;
                this.lblVehicleName.Visible = true;
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
            }
            else
            {
                this.cboVehicle.Visible = false;
                this.lblVehicleName.Visible = false;
                this.tblGeoZones.Visible = false;
            }*/
            refillCboVehicle();
        }

        protected void hidOrganizationHierarchyPostBack_Click(object sender, System.EventArgs e)
        {
            refillCboVehicle();
        }

        private void refillCboVehicle()
        {
            int fleetId = -1;
            string fleetids = string.Empty;

            if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
            {
                fleetids = hidOrganizationHierarchyFleetId.Value.ToString();
            }
            else if (sn.User.LoadVehiclesBasedOn == "hierarchy")
            {
                int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
            }
            else
            {
                fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);
            }

            if (fleetId != -1 || fleetids != string.Empty)
            {

                this.cboVehicle.Visible = true;
                this.lblVehicleName.Visible = true;

                if (fleetids != string.Empty)
                    CboVehicle_MultipleFleet_Fill(fleetids);
                else
                    CboVehicle_Fill(fleetId);
            }
            else
            {
                this.cboVehicle.Visible = false;
                this.lblVehicleName.Visible = false;
                this.tblGeoZones.Visible = false;
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
                        cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
                        return;
                    }
                if (xml == "")
                {
                    cboVehicle.Items.Clear();
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();

                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        private void CboVehicle_MultipleFleet_Fill(string fleetIds)
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
                        cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
                        return;
                    }
                if (xml == "")
                {
                    cboVehicle.Items.Clear();
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();

                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
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


        private void DgUnAssGeoZone_Fill()
        {
            DataRow[] drCollections = null;
            DataSet dsGeoZone = new DataSet();
            StringReader strrXML = null;
            string xml = "";

            try
            {
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetAllUnassignedToVehicleGeozonesInfo(sn.UserID, sn.SecId, Convert.ToInt64(cboVehicle.SelectedItem.Value), ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetAllUnassignedToVehicleGeozonesInfo(sn.UserID, sn.SecId, Convert.ToInt64(cboVehicle.SelectedItem.Value), ref xml), true))
                    {
                        this.dgUnAssGeoZone.DataSource = null;
                        this.dgUnAssGeoZone.DataBind();
                        this.tblGeoZoneHeaderUnAss.Visible = true;
                        if (sn.GeoZone.DsUnAssVehicleGeoZone != null)
                           sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows.Clear();
                        return;
                    }

                if (xml == "")
                {
                    this.dgUnAssGeoZone.DataSource = null;
                    this.dgUnAssGeoZone.DataBind();
                    this.tblGeoZoneHeaderUnAss.Visible = true;

                    if (sn.GeoZone.DsUnAssVehicleGeoZone != null)
                       sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows.Clear();
                    return;
                }

                this.tblGeoZoneHeaderUnAss.Visible = false;
                strrXML = new StringReader(xml);
                dsGeoZone.ReadXml(strrXML);


                // Show Combobox
                DataColumn dc = new DataColumn();
                dc.ColumnName = "chkBox";
                dc.DataType = Type.GetType("System.Boolean");
                dc.DefaultValue = false;

               if (sn.User.ControlEnable(sn, 93))
               {
                   DataTable dt = dsGeoZone.Tables[0].Clone();

                   string filter = String.Format("Public=false AND CreateUserID={0}", sn.UserID.ToString());
                   drCollections = dsGeoZone.Tables[0].Select(filter);

                   if (drCollections != null && drCollections.Length > 0)
                   {
                       foreach (DataRow dr in drCollections)
                       {
                           dt.ImportRow(dr);
                       }
                   }

                   dt.Columns.Add(dc);

                   DataSet ds = new DataSet();
                   ds.Tables.Add(dt);

                   this.dgUnAssGeoZone.DataSource = ds;
                   this.dgUnAssGeoZone.DataBind();

                   sn.GeoZone.DsUnAssVehicleGeoZone = ds;   
               }
               else
               {
                   dsGeoZone.Tables[0].Columns.Add(dc);

                   this.dgUnAssGeoZone.DataSource = dsGeoZone;
                   this.dgUnAssGeoZone.DataBind();

                   sn.GeoZone.DsUnAssVehicleGeoZone = dsGeoZone;
               }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }

        private void DgAssGeoZone_Fill()
        {
            DataSet dsGeoZone = new DataSet();
            DataRow[] drCollections = null;

            try
            {    
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                using (VLF.DAS.Logic.Vehicle dbVehicle = new VLF.DAS.Logic.Vehicle(sConnectionString))
                {
                    dsGeoZone = dbVehicle.GetAllAssignedToVehicleGeozonesInfo(Convert.ToInt64(cboVehicle.SelectedItem.Value));
                }

                //if (objUtil.ErrCheck(dbv.GetAllAssignedToVehicleGeozonesInfo(sn.UserID, sn.SecId, Convert.ToInt64(cboVehicle.SelectedItem.Value), ref xml), false))
                //    if (objUtil.ErrCheck(dbv.GetAllAssignedToVehicleGeozonesInfo(sn.UserID, sn.SecId, Convert.ToInt64(cboVehicle.SelectedItem.Value), ref xml), true))
                //    {
                //        this.tblGeoZoneHeader.Visible = true;
                //        this.dgAssGeoZone.DataSource = null;
                //        this.dgAssGeoZone.DataBind();
                //        if (sn.GeoZone.DsVehicleGeoZone != null)
                //           sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Clear();
                //        return;
                //    }

                //if (xml == "")
                //{
                //    this.dgAssGeoZone.DataSource = null;
                //    this.dgAssGeoZone.DataBind();
                //    this.tblGeoZoneHeader.Visible = true;
                //    if (sn.GeoZone.DsVehicleGeoZone != null)
                //       sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Clear();
                //    return;
                //}


                //strrXML = new StringReader(xml);
                //dsGeoZone.ReadXml(strrXML);

                this.tblGeoZoneHeader.Visible = false;

                if (dsGeoZone == null || dsGeoZone.Tables.Count == 0)
                {
                    this.tblGeoZoneHeader.Visible = true;
                    return;
                } 

                // Assigned Geozone to Vehicle
                DataColumn dcAssigned = new DataColumn();
                dcAssigned.ColumnName = "Assigned";
                dcAssigned.DataType = Type.GetType("System.Boolean");
                dcAssigned.DefaultValue = false;

                // Severity
                DataColumn dcSeverity = new DataColumn();
                dcSeverity.ColumnName = "Severity";
                dcSeverity.DataType = Type.GetType("System.String");
                dcSeverity.DefaultValue = "";

                // Status
                DataColumn dcStatus = new DataColumn();
                dcStatus.ColumnName = "Status";
                dcStatus.DataType = Type.GetType("System.String");
                dcStatus.DefaultValue = "";

               if (sn.User.ControlEnable(sn, 93))
               {
                   DataTable dt = dsGeoZone.Tables[0].Clone();

                   string filter = String.Format("Public=false AND CreateUserID={0}", sn.UserID.ToString());
                   drCollections = dsGeoZone.Tables[0].Select(filter);

                   if (drCollections != null && drCollections.Length > 0)
                   {
                       foreach (DataRow dr in drCollections)
                       {
                           dt.ImportRow(dr);
                       }
                   }

                   dt.Columns.Add(dcAssigned);
                   dt.Columns.Add(dcSeverity);
                   dt.Columns.Add(dcStatus);

                   foreach (DataRow rowItem in dt.Rows)
                   {
                       Int16 enumId = Convert.ToInt16(rowItem["SeverityId"]);
                       rowItem["Severity"] = Enum.GetName(typeof(Enums.AlarmSeverity), (Enums.AlarmSeverity)enumId);

                       if (Convert.ToDateTime(rowItem["SyncDate"]) == Convert.ToDateTime("1/1/1970 12:00:00 AM"))
                           rowItem["Status"] = clsGeoZone.AddPendingGeoZone;
                       else
                           rowItem["Status"] = clsGeoZone.GeoZoneSync;

                       rowItem["Assigned"] = "true";
                   }

                   DataSet ds = new DataSet();
                   ds.Tables.Add(dt);

                   this.dgAssGeoZone.DataSource = ds;
                   this.dgAssGeoZone.DataBind();

                   sn.GeoZone.DsVehicleGeoZone = ds;
               }
               else
               {
                   dsGeoZone.Tables[0].Columns.Add(dcAssigned);
                   dsGeoZone.Tables[0].Columns.Add(dcSeverity);
                   dsGeoZone.Tables[0].Columns.Add(dcStatus);

                   foreach (DataRow rowItem in dsGeoZone.Tables[0].Rows)
                   {
                       Int16 enumId = Convert.ToInt16(rowItem["SeverityId"]);
                       rowItem["Severity"] = Enum.GetName(typeof(Enums.AlarmSeverity), (Enums.AlarmSeverity)enumId);

                       if (Convert.ToDateTime(rowItem["SyncDate"]) == Convert.ToDateTime("1/1/1970 12:00:00 AM"))
                           rowItem["Status"] = clsGeoZone.AddPendingGeoZone;
                       else
                           rowItem["Status"] = clsGeoZone.GeoZoneSync;

                       rowItem["Assigned"] = "true";
                   }

                   this.dgAssGeoZone.DataSource = dsGeoZone;
                   this.dgAssGeoZone.DataBind();

                   sn.GeoZone.DsVehicleGeoZone = dsGeoZone;
               }
            }
            catch (NullReferenceException)
            {
                //this.tblGeoZoneHeader.Visible = true;
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                //this.tblGeoZoneHeader.Visible = true;
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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




            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }

        public void SaveCheckBoxes()
        {
            for (int i = 0; i < dgUnAssGeoZone.Items.Count; i++)
            {
                CheckBox ch = (CheckBox)(dgUnAssGeoZone.Items[i].Cells[1].Controls[1]);

                foreach (DataRow rowItem in sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows)
                {
                    if (dgUnAssGeoZone.Items[i].Cells[0].Text.ToString() == rowItem["GeoZoneId"].ToString())
                    {
                        rowItem["chkBox"] = ch.Checked;
                        break;
                    }

                }

            }

        }

        protected void cboVehicle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            LoadVehiclesGeoZones();
        }

        protected void cmdAdd_Click(object sender, System.EventArgs e)
        {
            if ((sn.GeoZone.DsUnAssVehicleGeoZone == null) || (sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows.Count == 0))
                return;

            SaveCheckBoxes();
            AddGeoZones();
        }

        private void dgAssGeoZone_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {

            try
            {

                if ((sn.Cmd.GeoZoneSync) || (sn.Cmd.CommandId == Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.GetGeoZoneIDs)))
                    return;

                if (e.CommandName == "Delete")
                {

                    //this.cmdSync.Enabled = true;

                    //LinkButton btn;

                    //if (e.Item.Cells[4].Text.ToString().ToLower() == "true")
                    //{
                    //    btn = (LinkButton)e.Item.FindControl("lnkDelete");

                    //    if (btn.Text == (string)base.GetLocalResourceObject("btn_Text_Cancel"))
                    //    {
                    //        btn.Text = (string)base.GetLocalResourceObject("btn_Text_Delete");
                    //        e.Item.Cells[3].Text = clsGeoZone.GeoZoneSync;

                    //        foreach (DataRow rowItem in sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows)
                    //        {
                    //            if (e.Item.Cells[0].Text.ToString() == rowItem["GeoZoneId"].ToString())
                    //            {
                    //                rowItem["Status"] = clsGeoZone.GeoZoneSync;
                    //                break;
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        btn.Text = (string)base.GetLocalResourceObject("btn_Text_Cancel");
                    //        e.Item.Cells[3].Text = clsGeoZone.DeletePendingGeoZone;

                    //        foreach (DataRow rowItem in sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows)
                    //        {
                    //            if (e.Item.Cells[0].Text.ToString() == rowItem["GeoZoneId"].ToString())
                    //            {
                    //                rowItem["Status"] = clsGeoZone.DeletePendingGeoZone;
                    //                break;
                    //            }
                    //        }
                    //    }



                    //}
                    //else
                    //{

                        foreach (DataRow rowItem in sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows)
                        {
                            if (e.Item.Cells[0].Text.ToString() == rowItem["GeoZoneId"].ToString())
                            {

                                if (sn.GeoZone.DsUnAssVehicleGeoZone == null)
                                {
                                    string strPath = Server.MapPath("../Datasets/dstGeoZones.xsd");
                                    DataSet ds = new DataSet();
                                    ds.ReadXmlSchema(strPath);

                                    // Show Combobox
                                    DataColumn dc = new DataColumn();
                                    dc.ColumnName = "chkBox";
                                    dc.DataType = Type.GetType("System.Boolean");
                                    dc.DefaultValue = false;
                                    ds.Tables[0].Columns.Add(dc);

                                    sn.GeoZone.DsUnAssVehicleGeoZone = ds;


                                }

                                sn.Cmd.BoxId = Convert.ToInt32(rowItem["BoxId"]);


                                //Add Geozone to Unassigned list						
                                DataRow dr =sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].NewRow();
                                dr["GeozoneId"] = rowItem["GeoZoneId"].ToString();
                                dr["GeoZoneName"] = rowItem["GeoZoneName"].ToString();
                                dr["SeverityId"] = rowItem["SeverityId"].ToString();
                                dr["GeozoneNo"] = rowItem["GeozoneNo"];
                                dr["OrganizationId"] = rowItem["OrganizationId"];
                                dr["Type"] = rowItem["Type"];
                                dr["Description"] = rowItem["Description"];
                                dr["GeozoneType"] = rowItem["GeozoneType"];

                               sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows.Add(dr);
                               sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].DefaultView.Sort = "GeoZoneName";



                                if ((sn.GeoZone.DsUnAssVehicleGeoZone != null) || (sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows.Count > 0))
                                {
                                    // check for invalid page index
                                    ResetPageIndex(dgUnAssGeoZone,sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].DefaultView);

                                    this.dgUnAssGeoZone.DataSource =sn.GeoZone.DsUnAssVehicleGeoZone;
                                    this.dgUnAssGeoZone.DataBind();

                                    this.tblGeoZoneHeaderUnAss.Visible = false;
                                }


                              


                               ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                               DataSet dsVehicle = new DataSet();


                               int rowsAffected = 0;
                               //if (objUtil.ErrCheck(dbv.DeleteGeozoneFromVehicleByVehicleId(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["VehicleId"]), Convert.ToInt16(rowItem["GeozoneId"]), ref rowsAffected), false))
                               //    if (objUtil.ErrCheck(dbv.DeleteGeozoneFromVehicleByVehicleId(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["VehicleId"]), Convert.ToInt16(rowItem["GeozoneId"]), ref rowsAffected), true))
                               //    {
                               //        return;
                               //    }

                               using (VLF.DAS.Logic.Vehicle dbVehicle = new VLF.DAS.Logic.Vehicle(sConnectionString))
                               {
                                   rowsAffected = dbVehicle.DeleteGeozoneFromVehicle(Convert.ToInt64(rowItem["VehicleId"]), Convert.ToInt16(rowItem["GeozoneId"]));
                               }


                              

                               //Delete Geozone
                               sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Remove(rowItem);

                                if ((sn.GeoZone.DsVehicleGeoZone == null) || (sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Count == 0))
                                {
                                    this.dgAssGeoZone.DataSource = null;
                                    this.dgAssGeoZone.DataBind();
                                    this.tblGeoZoneHeader.Visible = true;
                                }
                                else
                                {
                                   sn.GeoZone.DsVehicleGeoZone.Tables[0].DefaultView.Sort = "GeoZoneName";
                                    // check for invalid page index
                                    ResetPageIndex(dgAssGeoZone,sn.GeoZone.DsVehicleGeoZone.Tables[0].DefaultView);
                                    this.dgAssGeoZone.DataSource =sn.GeoZone.DsVehicleGeoZone;
                                    this.dgAssGeoZone.DataBind();
                                    dgAssGeoZone.SelectedIndex = -1;
                                }

                                break;
                            }
                        //}
                    }

                        SendGetBoxStatus( sn.Cmd.BoxId);
                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }

        private void dgUnAssGeoZone_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            SaveCheckBoxes();
            dgUnAssGeoZone.CurrentPageIndex = e.NewPageIndex;
            this.dgUnAssGeoZone.DataSource =sn.GeoZone.DsUnAssVehicleGeoZone;
            this.dgUnAssGeoZone.DataBind();
            dgUnAssGeoZone.SelectedIndex = -1;
        }

        private void dgAssGeoZone_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            dgAssGeoZone.CurrentPageIndex = e.NewPageIndex;


            DataView dv =sn.GeoZone.DsVehicleGeoZone.Tables[0].DefaultView;
            dv.Sort = "Status" + " ASC";


            this.dgAssGeoZone.DataSource = dv;
            this.dgAssGeoZone.DataBind();
            dgAssGeoZone.SelectedIndex = -1;
        }

        private void dgAssGeoZone_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                if ((e.Item.Cells[4].Text.ToString().ToLower() == "true") && (e.Item.Cells[3].Text == clsGeoZone.DeletePendingGeoZone))
                {
                    LinkButton btn = (LinkButton)e.Item.FindControl("lnkDelete");
                    btn.Text = (string)base.GetLocalResourceObject("btn_Text_Cancel");
                }
            }
        }


        private void DsSeverity_Fill()
        {
            try
            {
                DataTable tblSeverity = dsSeverity.Tables.Add("Severity");
                tblSeverity.Columns.Add("SeverityId", typeof(short));
                tblSeverity.Columns.Add("SeverityName", typeof(string));

                Array enmArr = Enum.GetValues(typeof(Enums.AlarmSeverity));
                string AlarmSeverity;
                object[] objRow;
                foreach (Enums.AlarmSeverity ittr in enmArr)
                {
                    AlarmSeverity = Enum.GetName(typeof(Enums.AlarmSeverity), ittr);
                    objRow = new object[2];
                    objRow[0] = Convert.ToInt16(ittr);
                    objRow[1] = AlarmSeverity;
                    dsSeverity.Tables[0].Rows.Add(objRow);

                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }


        public int GetSeverity(int SeverityId)
        {
            try
            {
                DropDownList cboSeverity = new DropDownList();
                cboSeverity.DataValueField = "SeverityId";
                cboSeverity.DataTextField = "SeverityName";
                DsSeverity_Fill();
                cboSeverity.DataSource = dsSeverity;
                cboSeverity.DataBind();

                cboSeverity.SelectedIndex = -1;
                cboSeverity.Items.FindByValue(SeverityId.ToString()).Selected = true;
                return cboSeverity.SelectedIndex;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                return 0;
            }

        }

        private void dgAssGeoZone_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {

                if ((sn.Cmd.GeoZoneSync) || (sn.Cmd.CommandId == Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.GetGeoZoneIDs)))
                    return;

                dgAssGeoZone.EditItemIndex = Convert.ToInt32(e.Item.ItemIndex);
                this.dgAssGeoZone.DataSource =sn.GeoZone.DsVehicleGeoZone;
                this.dgAssGeoZone.DataBind();
                dgAssGeoZone.SelectedIndex = -1;

            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        private void dgAssGeoZone_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {
                dgAssGeoZone.EditItemIndex = -1;
                this.dgAssGeoZone.DataSource =sn.GeoZone.DsVehicleGeoZone;
                this.dgAssGeoZone.DataBind();

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        private void dgAssGeoZone_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {


                DropDownList cboSeverity;

                string GeoZoneId = dgAssGeoZone.DataKeys[e.Item.ItemIndex].ToString();
                cboSeverity = (DropDownList)e.Item.FindControl("cboSeverity");


                foreach (DataRow rowItem in sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows)
                {
                    if (rowItem["GeoZoneId"].ToString() == GeoZoneId)
                    {
                        rowItem["SeverityId"] = cboSeverity.SelectedItem.Value.ToString();
                        rowItem["Severity"] = cboSeverity.SelectedItem.Text.ToString();

                        if (rowItem["Assigned"].ToString().ToLower() == "true")
                        {
                            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                            

                            if (objUtil.ErrCheck(dbv.SetGeozoneSeverity(sn.User.OrganizationId, sn.UserID, sn.SecId, Convert.ToInt64(this.cboVehicle.SelectedItem.Value), Convert.ToInt16(GeoZoneId), Convert.ToInt16(cboSeverity.SelectedItem.Value)), false))
                                if (objUtil.ErrCheck(dbv.SetGeozoneSeverity(sn.User.OrganizationId, sn.UserID, sn.SecId, Convert.ToInt64(this.cboVehicle.SelectedItem.Value), Convert.ToInt16(GeoZoneId), Convert.ToInt16(cboSeverity.SelectedItem.Value)), true))
                                {
                                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Update Geozone Severity Failed. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                                    return;
                                }

                        }

                        break;
                    }

                }

                dgAssGeoZone.EditItemIndex = -1;
                this.dgAssGeoZone.DataSource =sn.GeoZone.DsVehicleGeoZone;
                this.dgAssGeoZone.DataBind();

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }



        protected void cmdSync_Click(object sender, System.EventArgs e)
        {
            if (IsGeozoneNumerValid())
            {
               sn.GeoZone.GeozoneId = 0;
                SaveCheckBoxes();
                SyncGeozone();
            }
        }

        private void SyncGeozone()
        {
            try
            {
                short ProtocolId = -1;
                short CommandType = 0;
                string paramList = "";
                bool cmdSent = false;
                bool Reload = false;
                int GeoZoneIndex = 0;

                LocationMgr.Location dbl = new LocationMgr.Location();
                


                if ((sn.GeoZone.DsVehicleGeoZone == null) || (sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Count == 0))
                    return;


                bool clearAll = true;
                paramList = "";
                string GeoZoneList = "";

                //check if clear all Geozone
                foreach (DataRow rowItem in sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows)
                {
                    GeoZoneIndex++;
                    sn.Cmd.BoxId = Convert.ToInt32(rowItem["BoxId"]);
                    GeoZoneList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyGeozoneId.ToString() + GeoZoneIndex.ToString(), rowItem["GeozoneId"].ToString());

                    if (rowItem["Status"].ToString() == clsGeoZone.GeoZoneSync
                        || rowItem["Status"].ToString() == clsGeoZone.AddPendingGeoZone
                        || rowItem["Status"].ToString() == clsGeoZone.FailedAddPendingGeoZone)
                    {
                        clearAll = false;
                        break;
                    }
                }


                //Delete All Geozone
                if (clearAll)
                {
                    paramList = GeoZoneList;
                   sn.GeoZone.GeozoneId = VLF.CLS.Def.Const.allGeozones;
                    CommandType = Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.DeleteGeoZone);

                    short CommModeId = -1;
                    Int64 sessionTimeOut = 0;
                    if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(sn.Cmd.BoxId), CommandType, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false))
                        if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(sn.Cmd.BoxId), CommandType, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true))
                        {
                            sn.Cmd.GeoZoneSync = false;
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Add/Delete Geozone failed. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                            return;
                        }

                    if (cmdSent)
                    {
                        Reload = true;
                        sn.Cmd.GeoZoneSync = true;
                        sn.Cmd.ProtocolTypeId = ProtocolId;
                        sn.Cmd.CommModeId = CommModeId;
                        SendGeoZoneControlStatus();
                        return;
                    }
                }

                //-------------------------

                DataView dvGeoZone =sn.GeoZone.DsVehicleGeoZone.Tables[0].DefaultView;
                dvGeoZone.Sort = "GeoZoneId";
                GeoZoneIndex = 0;
                foreach (DataRowView rowItem in dvGeoZone)
                {

                    if (sn.GeoZone.GeozoneId < Convert.ToInt32(rowItem["GeozoneId"]))
                    {
                        if (rowItem["Status"].ToString() == clsGeoZone.AddPendingGeoZone
                            || rowItem["Status"].ToString() == clsGeoZone.DeletePendingGeoZone
                            || rowItem["Status"].ToString() == clsGeoZone.FailedAddPendingGeoZone
                            || rowItem["Status"].ToString() == clsGeoZone.FailedDeletePendingGeoZone)
                        {

                            ProtocolId = 0;
                            CommandType = 0;
                            paramList = "";

                           sn.GeoZone.GeozoneId = Convert.ToInt32(rowItem["GeozoneId"]);

                            if (rowItem["Status"].ToString() == clsGeoZone.AddPendingGeoZone || rowItem["Status"].ToString() == clsGeoZone.FailedAddPendingGeoZone)
                            {
                                CommandType = Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.AddGeoZone);
                                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyGeozoneId.ToString(), rowItem["GeozoneId"].ToString());
                                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyGeozoneSeverity.ToString(), rowItem["SeverityId"].ToString());
                            }
                            else if (rowItem["Status"].ToString() == clsGeoZone.DeletePendingGeoZone || rowItem["Status"].ToString() == clsGeoZone.FailedDeletePendingGeoZone)
                            {
                                GeoZoneIndex++;
                                CommandType = Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.DeleteGeoZone);
                                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyGeozoneId.ToString() + GeoZoneIndex.ToString(), rowItem["GeozoneId"].ToString());
                            }
                            else
                            {
                                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Add/Delete Geozone failed. User:" + sn.UserID.ToString() + " Form:frmVehicleGeoZone.aspx wrong command status: " + rowItem["Status"].ToString() + " for geozoneId: " + rowItem["GeozoneId"].ToString()));
                            }
                            cmdSent = false;

                            short CommModeId = -1;
                            if (CommandType != 0)
                            {
                                Int64 sessionTimeOut = 0;
                                if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), CommandType, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false))
                                    if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), CommandType, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true))
                                    {
                                        sn.Cmd.GeoZoneSync = false;
                                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Add/Delete Geozone failed. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                                    }
                            }
                            if (cmdSent)
                            {
                                Reload = true;
                                sn.Cmd.GeoZoneSync = true;
                                sn.Cmd.ProtocolTypeId = ProtocolId;
                                sn.Cmd.CommModeId = CommModeId;
                                SendGeoZoneControlStatus();
                                return;
                            }
                            else
                            {

                                if (rowItem["Status"].ToString() == clsGeoZone.AddPendingGeoZone)
                                {
                                    rowItem["Status"] = clsGeoZone.FailedAddPendingGeoZone;
                                }


                                if (rowItem["Status"].ToString() == clsGeoZone.DeletePendingGeoZone)
                                {
                                    rowItem["Status"] = clsGeoZone.FailedDeletePendingGeoZone;
                                }

                            }
                        }
                    }
                }

                if (!Reload)
                    StopSync();
            }


            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        // ResetPageIndex resets invalid page index to last page
        // ASSERT grid and view NOT NULL
        protected void ResetPageIndex(DataGrid grid, DataView view)
        {
            try
            {
                // check for invalid page index
                if ((grid.CurrentPageIndex != 0) && (((grid.CurrentPageIndex) * grid.PageSize) >= view.Count))
                {
                    // invalid so leave at last page
                    if ((view.Count % grid.PageSize) == 0)
                    { // ends on page border
                        grid.CurrentPageIndex = (view.Count / grid.PageSize) - 1;
                    }
                    else // partial page
                    {
                        grid.CurrentPageIndex = (view.Count / grid.PageSize);
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        protected void cmdClearAll_Click(object sender, System.EventArgs e)
        {
            try
            {

                if ((sn.GeoZone.DsVehicleGeoZone == null) || (sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Count == 0))
                    return;

                int RowsCount =sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Count - 1;

                for (int index = RowsCount; index >= 0; --index)
                {
                    try
                    {
                        DataRow rowItem = sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows[index];
                        
                   
                    //if (rowItem["Assigned"].ToString().ToLower() == "true")
                    //{
                    //    rowItem["Status"] = clsGeoZone.DeletePendingGeoZone;
                    //    this.cmdSync.Enabled = true;
                    //}
                    //else
                    //{
                        //Add Geozone to Unassigned list						
                        DataRow dr =sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].NewRow();
                        dr["GeozoneId"] = rowItem["GeoZoneId"].ToString();
                        dr["GeoZoneName"] = rowItem["GeoZoneName"].ToString();
                        dr["SeverityId"] = rowItem["SeverityId"].ToString();
                        dr["GeozoneNo"] = rowItem["GeozoneNo"];
                        dr["OrganizationId"] = rowItem["OrganizationId"];
                        dr["Type"] = rowItem["Type"];
                        dr["Description"] = rowItem["Description"];

                       sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows.Add(dr);

                  


                        //						sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].DefaultView.Sort="GeoZoneName";


                        //Delete GeoZone

                       ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                       DataSet dsVehicle = new DataSet();
                    

                       int rowsAffected = 0;
                       //if (objUtil.ErrCheck(dbv.DeleteGeozoneFromVehicleByVehicleId(sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicle.SelectedItem.Value), Convert.ToInt16(rowItem["GeozoneId"]), ref rowsAffected), false))
                       //    if (objUtil.ErrCheck(dbv.DeleteGeozoneFromVehicleByVehicleId(sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicle.SelectedItem.Value), Convert.ToInt16(rowItem["GeozoneId"]), ref rowsAffected), true))
                       //    {
                       //        return;
                       //    }


                       


                       using (VLF.DAS.Logic.Vehicle dbVehicle = new VLF.DAS.Logic.Vehicle(sConnectionString))
                       {
                           rowsAffected = dbVehicle.DeleteGeozoneFromVehicle(Convert.ToInt64(this.cboVehicle.SelectedItem.Value), Convert.ToInt16(rowItem["GeozoneId"]));
                       }


                       sn.Cmd.BoxId = Convert.ToInt32(rowItem["BoxId"]);
                      

                        //Delete Geozone
                       sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows[index].Delete();
                       sn.GeoZone.DsVehicleGeoZone.Tables[0].DefaultView.Sort = "GeoZoneName";
                    //}

                    }
                    catch
                    {
                        return;
                    }
                }



                SendGetBoxStatus(sn.Cmd.BoxId);

                if (sn.GeoZone.DsUnAssVehicleGeoZone == null)
                {
                    this.dgUnAssGeoZone.DataSource = null;
                    this.dgUnAssGeoZone.DataBind();
                }
                else
                {

                   sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].DefaultView.Sort = "GeoZoneId";

                    this.tblGeoZoneHeaderUnAss.Visible = false;
                    // check for invalid page index
                    ResetPageIndex(dgUnAssGeoZone,sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].DefaultView);

                    this.dgUnAssGeoZone.DataSource =sn.GeoZone.DsUnAssVehicleGeoZone;
                    this.dgUnAssGeoZone.DataBind();
                }


                // check for invalid page index
                if ((sn.GeoZone.DsVehicleGeoZone == null) || (sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Count == 0))
                {
                    this.dgAssGeoZone.DataSource = null;
                    this.dgAssGeoZone.DataBind();
                    this.tblGeoZoneHeader.Visible = true;
                }
                else
                {
                    // check for invalid page index
                    ResetPageIndex(dgAssGeoZone,sn.GeoZone.DsVehicleGeoZone.Tables[0].DefaultView);
                    this.dgAssGeoZone.DataSource =sn.GeoZone.DsVehicleGeoZone;
                    this.dgAssGeoZone.DataBind();

                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }


        }

        protected void cmdAddAll_Click(object sender, System.EventArgs e)
        {
            if ((sn.GeoZone.DsUnAssVehicleGeoZone == null) || (sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows.Count == 0))
                return;

            foreach (DataRow rowItem in sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows)
            {
                rowItem["chkBox"] = "true";
            }


            this.dgUnAssGeoZone.DataSource =sn.GeoZone.DsUnAssVehicleGeoZone;
            ViewState["DsUnAssVehicleGeoZoneClone"] =sn.GeoZone.DsUnAssVehicleGeoZone;
            this.dgUnAssGeoZone.DataBind();
            //this.cmdSync.Enabled = true;
            
            AddGeoZones();

            if ((sn.GeoZone.DsUnAssVehicleGeoZone == null) || (sn.GeoZone.DsUnAssVehicleGeoZone.Tables.Count == 0))
            {
                this.dgUnAssGeoZone.DataSource = null;
                this.dgUnAssGeoZone.DataBind();
                this.tblGeoZoneHeaderUnAss.Visible = true;
            }


        }

        private void AddGeoZones()
        {
            try
            {
                
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                DataSet dsVehicle = new DataSet();
                string xml = "";

                if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(this.cboVehicle.SelectedItem.Value), ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(this.cboVehicle.SelectedItem.Value), ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {
                    return;
                }

                int BoxId = 0;
                StringReader strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);
                if (dsVehicle.Tables[0].Rows.Count > 0)
                {
                    BoxId = Convert.ToInt32(dsVehicle.Tables[0].Rows[0]["BoxId"]);
                }


                int RowsCount =sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows.Count - 1;



                // UnAssigned Geozone Clone
                strrXML = new StringReader(sn.GeoZone.DsUnAssVehicleGeoZone.GetXml());

                if (sn.GeoZone.DsUnAssVehicleGeoZoneClone.Tables.Count > 0)
                   sn.GeoZone.DsUnAssVehicleGeoZoneClone.Tables[0].Rows.Clear();

               sn.GeoZone.DsUnAssVehicleGeoZoneClone.ReadXml(strrXML);



                //Assigned Geozone Clone
                strrXML = new StringReader(sn.GeoZone.DsVehicleGeoZone.GetXml());

                if (sn.GeoZone.DsAssVehicleGeoZoneClone.Tables.Count > 0)
                   sn.GeoZone.DsAssVehicleGeoZoneClone.Tables[0].Rows.Clear();

               sn.GeoZone.DsAssVehicleGeoZoneClone.ReadXml(strrXML);



                for (int index = RowsCount; index >= 0; --index)
                {
                    DataRow rowItem =sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows[index];
                    if (rowItem["chkBox"].ToString().ToLower() == "true")
                    {
                        //Add Geozone

                        if (sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Count > 49)
                        {

                            LoadVehiclesGeoZones();

                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_MaximumGeozonesError");

                            //UnAssigned Geozone Restore
                            strrXML = new StringReader(sn.GeoZone.DsUnAssVehicleGeoZoneClone.GetXml());

                            if (sn.GeoZone.DsUnAssVehicleGeoZone.Tables.Count > 0)
                               sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows.Clear();

                           sn.GeoZone.DsUnAssVehicleGeoZone.ReadXml(strrXML);



                            //Assigned Geozone Restore
                            strrXML = new StringReader(sn.GeoZone.DsAssVehicleGeoZoneClone.GetXml());

                            if (sn.GeoZone.DsVehicleGeoZone.Tables.Count > 0)
                               sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Clear();

                           sn.GeoZone.DsVehicleGeoZone.ReadXml(strrXML);


                            if (sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows.Count > 0)
                            {
                                this.dgUnAssGeoZone.DataSource =sn.GeoZone.DsUnAssVehicleGeoZone;
                                this.dgUnAssGeoZone.DataBind();
                                this.tblGeoZoneHeaderUnAss.Visible = false;
                            }

                            if (sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Count > 0)
                            {
                                this.dgAssGeoZone.DataSource =sn.GeoZone.DsVehicleGeoZone;
                                this.dgAssGeoZone.DataBind();
                                this.tblGeoZoneHeader.Visible = false;
                            }

                            return;
                        }
                        this.lblMessage.Text = "";

                        DataRow dr =sn.GeoZone.DsVehicleGeoZone.Tables[0].NewRow();
                        dr["VehicleId"] = this.cboVehicle.SelectedItem.Value.ToString();
                        dr["GeozoneId"] = rowItem["GeozoneId"];
                        dr["GeozoneName"] = rowItem["GeozoneName"];
                        dr["SeverityId"] = rowItem["SeverityId"];
                        Int16 enumId = Convert.ToInt16(dr["SeverityId"]);
                        dr["Severity"] = Enum.GetName(typeof(Enums.AlarmSeverity), (Enums.AlarmSeverity)enumId);
                        dr["GeozoneNo"] = rowItem["GeozoneNo"];
                        dr["OrganizationId"] = rowItem["OrganizationId"];
                        dr["Type"] = rowItem["Type"];
                        dr["Description"] = rowItem["Description"];
                        dr["Status"] = clsGeoZone.AddPendingGeoZone;
                        dr["Assigned"] = false;
                        dr["BoxId"] = BoxId.ToString();
                        dr["GeozoneType"] = rowItem["GeozoneType"];

                       sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Add(dr);

                        //Add GeoZone

                       //if (objUtil.ErrCheck(dbv.AddGeozoneToVehicle(sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicle.SelectedItem.Value), Convert.ToInt16(rowItem["GeozoneId"]), Convert.ToInt16(rowItem["SeverityId"]), 0), false))
                       //    if (objUtil.ErrCheck(dbv.AddGeozoneToVehicle(sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicle.SelectedItem.Value), Convert.ToInt16(rowItem["GeozoneId"]), Convert.ToInt16(rowItem["SeverityId"]), 0), true))
                       //    {
                       //        return;
                       //    }


                       using (VLF.DAS.Logic.Vehicle dbVehicle = new VLF.DAS.Logic.Vehicle(sConnectionString))
                       {
                           dbVehicle.AddGeozone(Convert.ToInt64(this.cboVehicle.SelectedItem.Value), Convert.ToInt16(rowItem["GeozoneId"]), Convert.ToInt16(rowItem["SeverityId"]), 0);
                       }


                      

                        // Delete Geozone
                       sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows[index].Delete();

                    }
                }

                SendGetBoxStatus(BoxId);

                if ((sn.GeoZone.DsVehicleGeoZone == null) || (sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Count == 0))
                {
                    this.tblGeoZoneHeader.Visible = true;
                   // this.cmdSync.Enabled = false;
                }
                else
                {
                    this.tblGeoZoneHeader.Visible = false;
                   sn.GeoZone.DsVehicleGeoZone.Tables[0].DefaultView.Sort = "GeoZoneName";
                    this.dgAssGeoZone.DataSource =sn.GeoZone.DsVehicleGeoZone;
                    this.dgAssGeoZone.DataBind();
                    this.dgAssGeoZone.Visible = true;
                    //this.cmdSync.Enabled = true;
                }


                if ((sn.GeoZone.DsUnAssVehicleGeoZone == null) || (sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows.Count == 0))
                {
                    this.dgUnAssGeoZone.DataSource = null;
                    this.dgUnAssGeoZone.DataBind();
                    this.tblGeoZoneHeaderUnAss.Visible = true;
                }
                else
                {
                   sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].DefaultView.Sort = "GeoZoneName";
                    // check for invalid page index
                    ResetPageIndex(dgUnAssGeoZone,sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].DefaultView);
                    this.dgUnAssGeoZone.DataSource =sn.GeoZone.DsUnAssVehicleGeoZone;
                    this.dgUnAssGeoZone.DataBind();
                }

            }


            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        private void SendGeoZoneControlStatus()
        {
            int fleetId = -1;
            if (sn.User.LoadVehiclesBasedOn == "hierarchy")
            {
                int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
            }
            else
            {
                fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);
            }
            
            sn.GeoZone.SelectedFleetId = fleetId;
           sn.GeoZone.SelectedVehicleId = Convert.ToInt64(this.cboVehicle.SelectedItem.Value);

            tblWait.Visible = true;
            this.cboFleet.Enabled = false;
            this.cboVehicle.Enabled = false;
           // this.cmdSync.Visible = false;
            this.cmdCheckSync.Visible = false;
            this.tblAddRemoveBtns.Visible = false;
            Response.Write("<script language='javascript'> parent.frametimer.location.href='frmTimerGeoZone.aspx' </script>");
        }

        protected void cmdCancelSync_Click(object sender, System.EventArgs e)
        {
            try
            {
                LocationMgr.Location dbl = new LocationMgr.Location();
                


                if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId), false))
                    if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId), true))
                    {
                        //return;
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " cannot Cancel command for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                    }



                sn.Cmd.GeoZoneSync = false; ;
                this.cboFleet.Enabled = true;
                this.cboVehicle.Enabled = true;
                //this.cmdSync.Visible = true;
                this.cmdCheckSync.Visible = true;
                this.tblWait.Visible = false;
                this.tblAddRemoveBtns.Visible = true;
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }


            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }


        private void StopSync()
        {
            sn.Cmd.GeoZoneSync = false; ;
            this.cboFleet.Enabled = true;
            this.cboVehicle.Enabled = true;
         //   this.cmdSync.Enabled = true;
            this.tblWait.Visible = false;
            this.tblAddRemoveBtns.Visible = true;
           sn.GeoZone.GeozoneId = 0;
            sn.Cmd.CommandId = 0;

            DataView dv =sn.GeoZone.DsVehicleGeoZone.Tables[0].DefaultView;
            dv.Sort = "Status" + " ASC";

            this.dgAssGeoZone.DataSource = dv;
            this.dgAssGeoZone.DataBind();
        }

        private void cmdPreference_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmPreference.aspx");
        }



        protected void cmdCheckSync_Click(object sender, System.EventArgs e)
        {
            //short ProtocolId = -1;
            //string paramList = "";
            //bool cmdSent = false;


            
            //ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
            //DataSet dsVehicle = new DataSet();
            //string xml = "";

            //if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(this.cboVehicle.SelectedItem.Value), ref xml), false))
            //    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(this.cboVehicle.SelectedItem.Value), ref xml), true))
            //    {
            //        return;
            //    }

            //if (xml == "")
            //    return;


            //StringReader strrXML = new StringReader(xml);
            //dsVehicle.ReadXml(strrXML);
            //if (dsVehicle.Tables[0].Rows.Count > 0)
            //    sn.Cmd.BoxId = Convert.ToInt32(dsVehicle.Tables[0].Rows[0]["BoxId"]);


            //sn.Cmd.CommandId = Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.GetGeoZoneIDs);
            //LocationMgr.Location dbl = new LocationMgr.Location();
            //short CommModeId = -1;

            //Int64 sessionTimeOut = 0;
            //if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.CommandId, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false))
            //    if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.CommandId, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true))
            //    {
            //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Add/Delete Geozone failed. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //        return;
            //    }

            //if (cmdSent)
            //{
            //    sn.Cmd.ProtocolTypeId = ProtocolId;
            //    sn.Cmd.CommModeId = CommModeId;
            //    SendGeoZoneControlStatus();
            //    return;
            //}


            LoadVehiclesGeoZones();
        }

        private void cmdLandmarks_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmEmails.aspx");
        }


        private bool IsGeozoneNumerValid()
        {

            this.lblMessage.Text = "";
            
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
            DataSet dsConf = new DataSet();
            int MaxGeoZoneNumber = 0;

            if (objUtil.ErrCheck(dbv.GetMaxGeozonesByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(this.cboVehicle.SelectedItem.Value), ref MaxGeoZoneNumber), false))
                if (objUtil.ErrCheck(dbv.GetMaxGeozonesByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(this.cboVehicle.SelectedItem.Value), ref MaxGeoZoneNumber), true))
                {
                    return false;
                }

            if (MaxGeoZoneNumber == 0)
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_GeozonesNotSupportedError");
                return false;

            }

            if (sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Count > MaxGeoZoneNumber)
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_MaximumGeozoneCount") + " " + MaxGeoZoneNumber.ToString();
                return false;
            }

            return true;

            //			DataTable dtTypes=new DataTable();
            //			DataColumn colGeozoneType= new DataColumn("GeozoneType",Type.GetType("System.Int32"));
            //			dtTypes.Columns.Add(colGeozoneType);
            //
            //
            //			DataColumn colQty= new DataColumn("Qty",Type.GetType("System.Int32"));
            //			dtTypes.Columns.Add(colQty);
            //
            //			bool TypeExist;
            //
            //			//Calculate number of Geozones by Type
            //			foreach(DataRow rowItem in sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows)
            //			{
            //				if (dtTypes.Rows.Count>0)
            //				{
            //					TypeExist=false;
            //
            //					foreach(DataRow rowType in dtTypes.Rows)
            //					{
            //						if (Convert.ToInt16(rowType["GeozoneType"])==Convert.ToInt16(rowItem["GeozoneType"]))
            //						{
            //							rowType["Qty"]=Convert.ToInt32(rowType["Qty"])+1;
            //							TypeExist=true;
            //							break;
            //						}
            //					}
            //
            //					if (!TypeExist)
            //					{
            //						DataRow dr=dtTypes.NewRow();
            //						dr["GeozoneType"]=rowItem["GeozoneType"];
            //						dr["Qty"]=1;
            //						dtTypes.Rows.Add(dr);
            //					}
            //				}
            //				else
            //				{
            //					DataRow dr=dtTypes.NewRow();
            //					dr["GeozoneType"]=rowItem["GeozoneType"];
            //					dr["Qty"]=1;
            //					dtTypes.Rows.Add(dr);  
            //				}
            //			}
            //
            //
            //			//Get DataBase Geozone Configurations
            //
            //			
            //			ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle() ;
            //			DataSet dsConf=new DataSet();
            //			string xml="";
            //
            //			if( objUtil.ErrCheck( dbv.GetMaxGeozonesByGeozoneType ( sn.UserID, sn.SecId ,Convert.ToInt64(this.cboVehicle.SelectedItem.Value),VLF.CLS.Def.Const.unassignedShortValue , ref xml ),false ) )
            //				if( objUtil.ErrCheck( dbv.GetMaxGeozonesByGeozoneType ( sn.UserID, sn.SecId ,Convert.ToInt64(this.cboVehicle.SelectedItem.Value),VLF.CLS.Def.Const.unassignedShortValue , ref xml ),true ) )
            //				{
            //					return false;
            //				}
            //
            //			if (xml == "")
            //				return false;
            //
            //
            //			StringReader strrXML = new StringReader( xml ) ;
            //			dsConf.ReadXml (strrXML) ;
            //
            //
            //			TypeExist=false;
            //			foreach(DataRow rowItem in dtTypes.Rows)
            //			{
            //				foreach(DataRow rowConf in dsConf.Tables[0].Rows)
            //				{
            //					if (Convert.ToInt16(rowConf["GeozoneType"])==Convert.ToInt16(rowItem["GeozoneType"]))
            //					{
            //						if (Convert.ToInt32(rowConf["MaxGeozones"])>=Convert.ToInt32(rowItem["Qty"]))
            //						{
            //							break;
            //						}
            //						else
            //						{
            //							return false;
            //						}
            //					}
            //				}
            //			}
            //
            //			return true;

        }

        private void LoadVehiclesGeoZones()
        {
            try
            {
                dgAssGeoZone.CurrentPageIndex = 0;
                dgUnAssGeoZone.CurrentPageIndex = 0;

                if (this.cboVehicle.SelectedItem.Value != "-1")
                {
                    DgUnAssGeoZone_Fill();
                    DgAssGeoZone_Fill();
                    this.tblGeoZones.Visible = true;
                   // this.cmdSync.Enabled = true;
                }
                else
                {
                    this.tblGeoZones.Visible = false;
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }


        private void SendGetBoxStatus(int boxId)
        {
            Int64 sessionTimeOut = 0;
            string errMsg = "";
            
            short CommModeId = -1;
            short ProtocolId = -1;
            bool cmdSent = false;
            string paramList = "";
            LocationMgr.Location dbl = new LocationMgr.Location();


            short CommandId = 16; // GetBoxStatus
            if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, boxId, CommandId, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, boxId, CommandId, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                {
                    return;
                }


            // CommandId = 79; // GetExtendedBoxStatus
            //if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, boxId, CommandId, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
            //    if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, boxId, CommandId, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
            //    {
            //        return;
            //    }

          



        }
    }
}
