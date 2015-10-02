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

    public partial class ctlOrganizationHierarchyFleetUsers : BaseControl
    {
        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string RootOrganizationHierarchyNodeCode = string.Empty;
        public string OrganizationHierarchyPath = "";

        public bool MutipleUserHierarchyAssignment;
        
        private VLF.PATCH.Logic.PatchOrganizationHierarchy poh;
        private VLF.PATCH.Logic.PatchFleet pf;
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                pf = new VLF.PATCH.Logic.PatchFleet(sConnectionString);

                MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

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
                    if (MutipleUserHierarchyAssignment)
                    {
                        defaultnodecode = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID, true);                        
                    }
                    else if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 480)
                        defaultnodecode = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID);
                    else
                        defaultnodecode = poh.GetRootNodeCode(sn.User.OrganizationId);

                }

                RootOrganizationHierarchyNodeCode = defaultnodecode;

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
                    this.tblUsers.Visible = false;
                    
                    if (DefaultOrganizationHierarchyFleetId != -1)
                    {
                        this.tblUsers.Visible = true;
                        lstUnAssUsers_Fill();
                        lstAssUsers_Fill();
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:ctlFleetUsers.aspx"));
            }
        }

        private void lstUnAssUsers_Fill()
        {
            if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyFleetId.Value.Contains(","))
                return;

            try
            {

                //StringReader strrXML = null;
                DataSet ds = new DataSet();

                /*string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetUnassignedUsersInfoXMLByFleetId(sn.UserID, sn.SecId, Convert.ToInt32(cboToFleet.SelectedItem.Value), ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetUnassignedUsersInfoXMLByFleetId(sn.UserID, sn.SecId, Convert.ToInt32(cboToFleet.SelectedItem.Value), ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    this.lstUnAss.Items.Clear();
                    return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);*/
                if (Convert.ToInt32(hidOrganizationHierarchyFleetId.Value) != -1)
                {
                    ds = poh.GetOrganizationHierarchyUnassignedUsersInfo(sn.User.OrganizationId, MutipleUserHierarchyAssignment, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value));
                    DataColumn UserFullName = new DataColumn("UserFullName", Type.GetType("System.String"));
                    ds.Tables[0].Columns.Add(UserFullName);


                    foreach (DataRow rw in ds.Tables[0].Rows)
                        rw["UserFullName"] = rw["LastName"].ToString() + " " + rw["FirstName"].ToString();

                    //sn.History.DsUnAssUsers = ds;
                    sn.History.DsUnAssOHUsers = ds;
                    this.lstUnAss.DataSource = ds;
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:ctlFleetUsers.aspx"));
            }

        }

        private void lstAssUsers_Fill()
        {
            if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyFleetId.Value.Contains(","))
                return;

            try
            {

                StringReader strrXML = null;
                DataSet ds = new DataSet();

                /*string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetUsersInfoXMLByFleetId(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetUsersInfoXMLByFleetId(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    this.lstAss.Items.Clear();
                    return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);
                
                DataColumn UserFullName = new DataColumn("UserFullName", Type.GetType("System.String"));
                ds.Tables[0].Columns.Add(UserFullName);

                foreach (DataRow rw in ds.Tables[0].Rows)
                    rw["UserFullName"] = rw["LastName"].ToString() + " " + rw["FirstName"].ToString();


                //sn.History.DsAssUsers = ds;
                sn.History.DsAssOHUsers = ds;
                this.lstAss.DataSource = ds;
                this.lstAss.DataBind();
                 */

                if (Convert.ToInt32(hidOrganizationHierarchyFleetId.Value) != -1)
                {
                    ds = pf.GetUsersInfoByFleetId(Convert.ToInt32(hidOrganizationHierarchyFleetId.Value));
                    DataColumn UserFullName = new DataColumn("UserFullName", Type.GetType("System.String"));
                    ds.Tables[0].Columns.Add(UserFullName);


                    foreach (DataRow rw in ds.Tables[0].Rows)
                        rw["UserFullName"] = rw["LastName"].ToString() + " " + rw["FirstName"].ToString();

                    //sn.History.DsUnAssUsers = ds;
                    sn.History.DsAssOHUsers = ds;
                    this.lstAss.DataSource = ds;
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:ctlFleetUsers.aspx"));
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

        protected void cmdAdd_Click(object sender, System.EventArgs e)
        {
            try
            {


                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                this.lblMessage.Text = "";

                if (Convert.ToInt32(hidOrganizationHierarchyFleetId.Value) == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");
                    return;
                }


                if (this.lstUnAss.SelectedIndex == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectUser");
                    return;
                }

                foreach (ListItem li in lstUnAss.Items)
                {
                    if (li.Selected)
                    {
                        //if (objUtil.ErrCheck(dbf.AddUserToFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(li.Value)), false))
                        //    if (objUtil.ErrCheck(dbf.AddUserToFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(li.Value)), false))
                        //    {
                        //        return;
                        //    }
                        if (pf.AddUserToFleet(sn.User.OrganizationId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), sn.UserID, Convert.ToInt32(li.Value)) == -1)
                            return;
                    }

                }


                lstUnAssUsers_Fill();
                lstAssUsers_Fill();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }


            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:ctlFleetUsers.aspx"));
            }
        }

        protected void cmdAddAll_Click(object sender, System.EventArgs e)
        {
            try
            {

                if (Convert.ToInt32(hidOrganizationHierarchyFleetId.Value) == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");
                    return;
                }

                this.lblMessage.Text = "";


                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                DataSet dsUnAssUsers = new DataSet();
                //dsUnAssUsers = sn.History.DsUnAssUsers;
                dsUnAssUsers = sn.History.DsUnAssOHUsers;

                foreach (DataRow rowItem in dsUnAssUsers.Tables[0].Rows)
                {
                    //if (objUtil.ErrCheck(dbf.AddUserToFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(rowItem["UserId"])), false))
                    //    if (objUtil.ErrCheck(dbf.AddUserToFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(rowItem["UserId"])), false))
                    //    {
                    //        return;
                    //    }
                    if (pf.AddUserToFleet(sn.User.OrganizationId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), sn.UserID, Convert.ToInt32(rowItem["UserId"])) == -1)
                        return;
                }

                lstUnAssUsers_Fill();
                lstAssUsers_Fill();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:ctlFleetUsers.aspx"));
            }
        }

        protected void cmdRemove_Click(object sender, System.EventArgs e)
        {
            try
            {

                this.lblMessage.Text = "";

                if (this.lstAss.SelectedIndex == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectUser");
                    return;
                }


                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                foreach (ListItem li in lstAss.Items)
                {
                    if (li.Selected)
                    {
                        //if (objUtil.ErrCheck(dbf.DeleteUserFromFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(li.Value)), false))
                        //    if (objUtil.ErrCheck(dbf.DeleteUserFromFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(li.Value)), false))
                        //    {
                        //        return;
                        //    }

                        if (pf.DeleteUserFromFleet(sn.User.OrganizationId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), sn.UserID, Convert.ToInt32(li.Value)) == -1)
                            return;
                    }

                }


                lstUnAssUsers_Fill();
                lstAssUsers_Fill();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:ctlFleetUsers.aspx"));
            }

        }

        protected void cmdRemoveAll_Click(object sender, System.EventArgs e)
        {
            try
            {

                if (Convert.ToInt32(hidOrganizationHierarchyFleetId.Value) == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");
                    return;
                }

                this.lblMessage.Text = "";


                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                DataSet dsAssUsers = new DataSet();
                //dsAssUsers = sn.History.DsAssUsers;
                dsAssUsers = sn.History.DsAssOHUsers;

                foreach (DataRow rowItem in dsAssUsers.Tables[0].Rows)
                {
                    //if (objUtil.ErrCheck(dbf.DeleteUserFromFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(rowItem["UserId"])), false))
                    //    if (objUtil.ErrCheck(dbf.DeleteUserFromFleet(sn.UserID, sn.SecId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), Convert.ToInt32(rowItem["UserId"])), true))
                    //    {
                    //        return;
                    //    }
                    if (pf.DeleteUserFromFleet(sn.User.OrganizationId, Convert.ToInt32(hidOrganizationHierarchyFleetId.Value), sn.UserID, Convert.ToInt32(rowItem["UserId"])) == -1)
                        return;
                }

                lstUnAssUsers_Fill();
                lstAssUsers_Fill();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:ctlFleetUsers.aspx"));
            }
        }

        protected void hidOrganizationHierarchyPostBack_Click(object sender, System.EventArgs e)
        {
            if (Convert.ToInt32(hidOrganizationHierarchyFleetId.Value) != -1)
            {
                this.tblUsers.Visible = true;
                lstUnAssUsers_Fill();
                lstAssUsers_Fill();
            }
            else
            {
                this.lstAss.Items.Clear();
                this.lstUnAss.Items.Clear();
            }
        }
    }
}
