using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using VLF.ERRSecurity;
using VLF.Reports;
using System.Text;
using VLF.CLS;
using Telerik.Web.UI;
using System.Drawing;
using VLF.DAS.Logic;
using System.Collections.Generic;
using VLF.DAS.DB;
namespace SentinelFM
{
    public partial class Configuration_WorkingHour_frmWorkingHrs : SentinelFMBasePage
    {
        public string selectFleet = "Select a Fleet";
        public string errorLoad = "Failed to load data.";
        public string errorSave = "Save failed.";
        public string saveSucceed = "Saved Successfully.";
        DataTable dt = null;
        DataTable emaildt = null;
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        public string cboTimeZoneIninId = "85";

        public bool ShowOrganizationHierarchy;
        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string LoadVehiclesBasedOn = "fleet";
        public string OrganizationHierarchyPath = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../../Login.aspx','_top')</SCRIPT>", false);
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

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == 36)
                    {
                        string d = rowItem["PreferenceValue"].ToString().ToLower();
                        if (d == "hierarchy")
                            LoadVehiclesBasedOn = "hierarchy";
                        else
                            LoadVehiclesBasedOn = "fleet";
                    }
                }

                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                    ShowOrganizationHierarchy = true;
                else
                {
                    ShowOrganizationHierarchy = false;
                    LoadVehiclesBasedOn = "fleet";
                }

                if (LoadVehiclesBasedOn == "hierarchy")
                {
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
                    }
                    else
                    {
                        hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());
                        btnOrganizationHierarchyNodeCode.Text = hidOrganizationHierarchyNodeCode.Value.ToString();

                        DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();

                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                    }
                    OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);

                    btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == "" ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                    cboFleet.Visible = false;
                    lblFleet.Visible = false;
                    btnOrganizationHierarchyNodeCode.Visible = true;
                    lblOhTitle.Visible = true;
                    hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                }

                GenerateDateTable();
                if (!IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    FillFleets();
                    cboTimeZone_Fill_NewTZ();
                    rvcboFleet.InitialValue = selectFleet;
                    //DataRow dr = dt.NewRow();
                    //dr["id"] = "";
                    //dt.Rows.Add(dr);
                    gdWorkingHr.DataSource = dt;
                    gdWorkingHr.DataBind();

                    gdEmails.DataSource = emaildt;
                    gdEmails.DataBind();
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
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
            if (IsPostBack)
            {
                ResetExceptionList();
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
                        cboException.Items.Clear();
                        return;
                    }
                if (xml == "")
                {
                    cboException.Items.Clear();
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboException.DataSource = dsVehicle.Tables[0];
                cboException.DataBind();
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

        private void GenerateDateTable()
        {
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("id");
                DateTime? dta = System.DateTime.Now; 
                dt.Columns.Add(new DataColumn("From", dta.GetType()));
                dt.Columns.Add(new DataColumn("To", dta.GetType()));
                dt.Columns.Add("chkSun");
                dt.Columns.Add("chkMon");
                dt.Columns.Add("chkTue");
                dt.Columns.Add("chkWed");
                dt.Columns.Add("chkThu");
                dt.Columns.Add("chkFri");
                dt.Columns.Add("chkSat");
                dt.Columns.Add("chkHoliday");
            }

            if (emaildt == null)
            {
                emaildt = new DataTable();
                emaildt.Columns.Add("id");
                emaildt.Columns.Add("email");
            }
        }

        private void FillFleets()
        {
            DataSet dsFleets = sn.User.GetUserFleets(sn);
            cboFleet.DataSource = dsFleets;
            cboFleet.DataBind();
            cboFleet.Items.Insert(0, new RadComboBoxItem(selectFleet, "-1"));
        }

        // Changes for TimeZone Feature start
        private void cboTimeZone_Fill_NewTZ()
        {
            try
            {
                ContactManager contactManager = new ContactManager(sConnectionString);
                cboTimeZone.Items.Clear();
                DataTable dt = contactManager.GetTimeZones().Tables[0];
                DataView dv = dt.DefaultView;
                //dv.Sort = "Id";
                cboTimeZone.DataSource = dv;
                cboTimeZone.DataTextField = "DisplayName";
                cboTimeZone.DataValueField = "Id";

                cboTimeZone.DataBind();
                RadComboBoxItem comboItem = cboTimeZone.Items.FindItemByValue(cboTimeZoneIninId);
                if (comboItem != null)
                    comboItem.Selected = true;
                else cboTimeZone.SelectedIndex = 0;

                if (!FindUserTimeZone_NewTZ())
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["TimeZoneId"] != DBNull.Value)
                        {
                            TimeZoneInfo timeInfo = System.TimeZoneInfo.FindSystemTimeZoneById(dr["TimeZoneId"].ToString());
                            int dtcDt = (int)timeInfo.BaseUtcOffset.TotalHours;
                            if (dtcDt == sn.User.NewFloatTimeZone)
                            {
                                cboTimeZone.ClearSelection();
                                cboTimeZone.FindItemByValue(dr["Id"].ToString()).Selected = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }
        // Changes for TimeZone Feature end

        private void cboTimeZone_Fill()
        {
            try
            {
                ContactManager contactManager = new ContactManager(sConnectionString);
                cboTimeZone.Items.Clear();
                DataTable dt = contactManager.GetTimeZones().Tables[0];
                DataView dv = dt.DefaultView;
                //dv.Sort = "Id";
                cboTimeZone.DataSource = dv;
                cboTimeZone.DataTextField = "DisplayName";
                cboTimeZone.DataValueField = "Id";

                cboTimeZone.DataBind();
                RadComboBoxItem comboItem = cboTimeZone.Items.FindItemByValue(cboTimeZoneIninId);
                if (comboItem != null)
                    comboItem.Selected = true;
                else cboTimeZone.SelectedIndex = 0;

                if (!FindUserTimeZone())
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["TimeZoneId"] != DBNull.Value)
                        {
                            TimeZoneInfo timeInfo = System.TimeZoneInfo.FindSystemTimeZoneById(dr["TimeZoneId"].ToString());
                            int dtcDt = (int)timeInfo.BaseUtcOffset.TotalHours;
                            if (dtcDt == sn.User.TimeZone)
                            {
                                cboTimeZone.ClearSelection();
                                cboTimeZone.FindItemByValue(dr["Id"].ToString()).Selected = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }
        protected void btnNewWorkinghr_Click(object sender, EventArgs e)
        {
            GenerateTableByGrid(null);
            DataRow dr = dt.NewRow();
            dr["id"] = "";
            dr["From"] = DateTime.Now.Date;
            dr["To"] = DateTime.Now.Date.AddHours(23).AddMinutes(59);
            dt.Rows.Add(dr);
            gdWorkingHr.DataSource = dt;
            gdWorkingHr.DataBind();

        }

        protected void  gdWorkingHr_ItemDataBound(object sender, GridItemEventArgs e)
        {
            RadTimePicker cboFrom = (RadTimePicker)e.Item.FindControl("cboFrom");
            RadTimePicker cboTo = (RadTimePicker)e.Item.FindControl("cboTo");
            ImageButton btnDel = (ImageButton)e.Item.FindControl("btnDel");
            CheckBox chkSun = (CheckBox)e.Item.FindControl("chkSun");
            CheckBox chkMon = (CheckBox)e.Item.FindControl("chkMon");
            CheckBox chkTue = (CheckBox)e.Item.FindControl("chkTue");
            CheckBox chkWed = (CheckBox)e.Item.FindControl("chkWed");
            CheckBox chkThu = (CheckBox)e.Item.FindControl("chkThu");
            CheckBox chkFri = (CheckBox)e.Item.FindControl("chkFri");
            CheckBox chkSat = (CheckBox)e.Item.FindControl("chkSat");
            CheckBox chkHoliday = (CheckBox)e.Item.FindControl("chkHoliday");
            HiddenField hidID = (HiddenField)e.Item.FindControl("hidID");
            CustomValidator cvscboTo = (CustomValidator)e.Item.FindControl("cvscboTo");
            CustomValidator cvscboFrom = (CustomValidator)e.Item.FindControl("cvscboFrom");
            if (cboFrom != null && cboTo != null)
            {
                cboFrom.TimePopupButton.Visible = false;
                //cboFrom.TimeView.Interval = new TimeSpan(60, 0, 0);
                cboTo.TimePopupButton.Visible = false;
                //cboTo.TimeView.Interval = new TimeSpan(60, 0, 0);
                cboFrom.TimeView.RenderDirection = RepeatDirection.Vertical;
                cboTo.TimeView.RenderDirection = RepeatDirection.Vertical;
                cboFrom.TimeView.Columns = 5;
                cboTo.TimeView.Columns = 5;
                cboFrom.DateInput.ToolTip = "hh:mm";
                cboTo.DateInput.ToolTip = "hh:mm";
               // if (e.Item.ItemIndex == 0) btnDel.Visible = false;
                if (cvscboTo != null) {
                    cvscboTo.Attributes.Add("title", cboFrom.ClientID);
                    //cvscboTo.Attributes.Add("title", cboTo.ClientID);
                    cvscboFrom.ErrorMessage = "To value is less than From value. Please check To value in line " + e.Item.ItemIndex.ToString();
                }
                if (cvscboFrom != null)
                {
                    //cvscboFrom.Attributes.Add("title", cboFrom.ClientID);
                    cvscboFrom.Attributes.Add("title", cboTo.ClientID);
                    cvscboFrom.ErrorMessage = "From value is greater than To value. Please check From value in line " + e.Item.ItemIndex.ToString();
                }

                DataRowView drv = e.Item.DataItem as DataRowView;
                if (drv != null)
                {
                    if ((!(drv["From"] is DBNull)) && drv["From"] != null && drv["From"].ToString() != "")
                       cboFrom.SelectedDate = (DateTime)drv["From"];
                    if ((!(drv["To"] is DBNull)) && drv["To"] != null && drv["To"].ToString() != "")
                        cboTo.SelectedDate = (DateTime)drv["To"];
                    if (!(drv["id"] is DBNull) && drv["id"] != null)
                    {
                        hidID.Value = drv["id"].ToString();
                    }

                    if (drv["chkSun"].ToString() == "True")
                        chkSun.Checked = true;
                    if (drv["chkMon"].ToString() == "True")
                        chkMon.Checked = true;
                    if (drv["chkTue"].ToString() == "True")
                        chkTue.Checked = true;
                    if (drv["chkWed"].ToString() == "True")
                        chkWed.Checked = true;

                    if (drv["chkThu"].ToString() == "True")
                        chkThu.Checked = true;
                    if (drv["chkFri"].ToString() == "True")
                        chkFri.Checked = true;
                    if (drv["chkSat"].ToString() == "True")
                        chkSat.Checked = true;
                    if (drv["chkHoliday"].ToString() == "True")
                        chkHoliday.Checked = true;
                    
                    if (drv["id"] != DBNull.Value )
                        hidID.Value = drv["id"].ToString();

                }
            }
               
        }

        private void GenerateTableByGrid(int? deleteItem)
        {
            dt.Rows.Clear();
            foreach (GridDataItem item in gdWorkingHr.Items)
            {
                if (deleteItem != null && item.ItemIndex == deleteItem) continue;
                RadTimePicker cboFrom = (RadTimePicker)item.FindControl("cboFrom");
                RadTimePicker cboTo = (RadTimePicker)item.FindControl("cboTo");
                CheckBox chkSun = (CheckBox)item.FindControl("chkSun");
                CheckBox chkMon = (CheckBox)item.FindControl("chkMon");
                CheckBox chkTue = (CheckBox)item.FindControl("chkTue");
                CheckBox chkWed = (CheckBox)item.FindControl("chkWed");
                CheckBox chkThu = (CheckBox)item.FindControl("chkThu");
                CheckBox chkFri = (CheckBox)item.FindControl("chkFri");
                CheckBox chkSat = (CheckBox)item.FindControl("chkSat");
                CheckBox chkHoliday = (CheckBox)item.FindControl("chkHoliday");
                HiddenField hidID = (HiddenField)item.FindControl("hidID");
                DataRow dr = dt.NewRow();
                if (cboFrom.SelectedDate.HasValue)
                    dr["From"] = cboFrom.SelectedDate.Value;
                else dr["From"] = DBNull.Value;
                if (cboTo.SelectedDate.HasValue)
                    dr["To"] = cboTo.SelectedDate.Value;
                else dr["To"] = DBNull.Value;
                dr["chkSun"] = chkSun.Checked.ToString();
                dr["chkMon"] = chkMon.Checked.ToString();
                dr["chkTue"] = chkTue.Checked.ToString();
                dr["chkWed"] = chkWed.Checked.ToString();
                dr["chkThu"] = chkThu.Checked.ToString();
                dr["chkFri"] = chkFri.Checked.ToString();
                dr["chkSat"] = chkSat.Checked.ToString();
                dr["chkHoliday"] = chkHoliday.Checked.ToString();
                dr["id"] = hidID.Value;
                dt.Rows.Add(dr);
            }
        }

        private void GenerateEmailTableByGrid(int? deleteItem)
        {
            emaildt.Rows.Clear();
            foreach (GridDataItem item in gdEmails.Items)
            {
                if (deleteItem != null && item.ItemIndex == deleteItem) continue;
                TextBox txtEmail = (TextBox)item.FindControl("txtEmail");
                HiddenField hidID = (HiddenField)item.FindControl("hidID");
                DataRow dr = emaildt.NewRow();
                dr["email"] = txtEmail.Text;
                dr["id"] = hidID.Value;
                emaildt.Rows.Add(dr);
            }
        }
        protected void gdWorkingHr_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GenerateTableByGrid(e.Item.ItemIndex);
                gdWorkingHr.DataSource = dt;
                gdWorkingHr.DataBind();
            }
        }
        protected void gdEmails_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GenerateEmailTableByGrid(e.Item.ItemIndex);
                gdEmails.DataSource = emaildt;
                gdEmails.DataBind();
            }
        }
        protected void gdEmails_ItemDataBound(object sender, GridItemEventArgs e)
        {
            TextBox txtEmail = (TextBox)e.Item.FindControl("txtEmail");
            HiddenField hidID = (HiddenField)e.Item.FindControl("hidID");
            if (txtEmail != null)
            {

                DataRowView drv = e.Item.DataItem as DataRowView;
                if (drv != null)
                {
                    if ((!(drv["email"] is DBNull)) && drv["email"] != null )
                        txtEmail.Text = drv["email"].ToString();
                    if (!(drv["id"] is DBNull) && drv["id"] != null)
                    {
                        hidID.Value = drv["id"].ToString();
                    }


                }
            }
        }
        protected void btnAddEmail_Click(object sender, EventArgs e)
        {
            GenerateEmailTableByGrid(null);
            DataRow dr = emaildt.NewRow();
            emaildt.Rows.Add(dr);
            gdEmails.DataSource = emaildt;
            gdEmails.DataBind();
        }
        protected void cboFleet_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (cboFleet.SelectedIndex <= 0)
            {
                cboException.Items.Clear();
                pnlInfo.Visible = false;
            }
            else
            {
                CboVehicle_Fill(int.Parse(cboFleet.SelectedValue));
                GetWorkingHoursUTCByFleetId(cboFleet.SelectedValue);
                pnlInfo.Visible = true;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (LoadVehiclesBasedOn == "fleet" && cboFleet.SelectedIndex <= 0) return;

            try
            {
                int fleedID = -1;
                if(LoadVehiclesBasedOn == "fleet")
                    fleedID = int.Parse(cboFleet.SelectedValue);
                else
                    int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleedID);

                string email = string.Empty;
                StringBuilder sbWorkingHrs = new StringBuilder();
                string exeption = GetCheckedException();
                ArrayList fromTime = new ArrayList();
                ArrayList toTime = new ArrayList();
                ArrayList workingHrs = new ArrayList();
                ArrayList IDs = new ArrayList();
                if (gdEmails.Items.Count > 0)
                {
                    foreach (GridDataItem item in gdEmails.Items)
                    {

                        string id = "-1";
                        HiddenField hidID = (HiddenField)item.FindControl("hidID");
                        if (hidID != null)
                        {
                            if (hidID.Value.Trim() != "") id = hidID.Value;
                        }
                        TextBox txtEmail = (TextBox)item.FindControl("txtEmail");
                        string email_ext = email + ",";
                        if (txtEmail != null && txtEmail.Text.Trim() != string.Empty && !email_ext.Contains("#" + txtEmail.Text.Trim() + ","))
                        {
                            if (email == string.Empty) email = id + "#" + txtEmail.Text.Trim();
                            else email = email + "," + id + "#" + txtEmail.Text.Trim();
                        }
                    }
                }
                else
                {
                    email = "-1#";
                }
                if (email == "") email = "-1#";
                //New
                ContactManager contactMgr = new ContactManager(sConnectionString);
                string tzID = contactMgr.GetTimeZonesByID(cboTimeZone.SelectedValue);
                TimeZoneInfo timeInfo = System.TimeZoneInfo.FindSystemTimeZoneById(tzID);
                int dtcDt = (int)timeInfo.BaseUtcOffset.TotalMinutes;
                foreach (GridDataItem item in gdWorkingHr.Items)
                {
                    RadTimePicker cboFrom = (RadTimePicker)item.FindControl("cboFrom");
                    RadTimePicker cboTo = (RadTimePicker)item.FindControl("cboTo");
                    CheckBox chkSun = (CheckBox)item.FindControl("chkSun");
                    CheckBox chkMon = (CheckBox)item.FindControl("chkMon");
                    CheckBox chkTue = (CheckBox)item.FindControl("chkTue");
                    CheckBox chkWed = (CheckBox)item.FindControl("chkWed");
                    CheckBox chkThu = (CheckBox)item.FindControl("chkThu");
                    CheckBox chkFri = (CheckBox)item.FindControl("chkFri");
                    CheckBox chkSat = (CheckBox)item.FindControl("chkSat");
                    CheckBox chkHoliday = (CheckBox)item.FindControl("chkHoliday");
                    HiddenField hidID = (HiddenField)item.FindControl("hidID");
                    int timezone = int.Parse(cboTimeZone.SelectedValue);
                    if (cboTo.SelectedDate.HasValue && cboFrom.SelectedDate.HasValue)
                    {
                        string days = string.Empty;
                        if (cboTo.SelectedDate.Value > cboFrom.SelectedDate.Value ||
                            (cboTo.SelectedDate.Value.Hour == 0 && cboTo.SelectedDate.Value.Minute == 0 &&
                             cboFrom.SelectedDate.Value.Hour == 0 && cboFrom.SelectedDate.Value.Minute == 0)
                            )
                        {
                            if (chkMon.Checked)
                            {
                                if (days == string.Empty) days = "MON";
                                else days = days + ",MON";
                            }
                            if (chkTue.Checked)
                            {
                                if (days == string.Empty) days = "TUE";
                                else days = days + ",TUE";
                            }
                            if (chkWed.Checked)
                            {
                                if (days == string.Empty) days = "WED";
                                else days = days + ",WED";
                            }
                            if (chkThu.Checked)
                            {
                                if (days == string.Empty) days = "THU";
                                else days = days + ",THU";
                            }
                            if (chkFri.Checked)
                            {
                                if (days == string.Empty) days = "FRI";
                                else days = days + ",FRI";
                            }
                            if (chkSat.Checked)
                            {
                                if (days == string.Empty) days = "SAT";
                                else days = days + ",SAT";
                            }

                            if (chkSun.Checked)
                            {
                                if (days == string.Empty) days = "SUN";
                                else days = days + ",SUN";
                            }
                            if (chkHoliday.Checked)
                            {
                                if (days == string.Empty) days = "HOL";
                                else days = days + ",HOL";
                            }
                            int fromDt = cboFrom.SelectedDate.Value.Hour * 60 + cboFrom.SelectedDate.Value.Minute;
                            int toDt = cboTo.SelectedDate.Value.Hour * 60 + cboTo.SelectedDate.Value.Minute;

                            fromDt = fromDt - dtcDt;
                            toDt = toDt - dtcDt;
                            string id = "-1";
                            if (hidID != null)
                            {
                                if (hidID.Value.Trim() != "") id = hidID.Value;
                            }
                            fromTime.Add(fromDt);
                            toTime.Add(toDt);
                            workingHrs.Add(days);
                            IDs.Add(id);
                        }
                    }
                }


                for (int index_1 = 0; index_1 < fromTime.Count; index_1++)
                {
                    for (int index_2 = index_1 + 1; index_2 < fromTime.Count; index_2++)
                    {
                        if (workingHrs[index_1].ToString() == workingHrs[index_2].ToString())
                        {
                            if ((((int)fromTime[index_1]) <= ((int)toTime[index_2]) && 
                                ((int)toTime[index_1]) >= ((int)fromTime[index_2])) ||
                                (int)toTime[index_1] + 1 == (int)fromTime[index_2] ||
                                (int)toTime[index_2] + 1 == (int)fromTime[index_1])
                            {
                                if ((int)fromTime[index_1] > (int)fromTime[index_2]) fromTime[index_1] = fromTime[index_2];
                                if ((int)toTime[index_1] < (int)toTime[index_2]) toTime[index_1] = toTime[index_2];
                                fromTime.Remove(fromTime[index_2]);
                                toTime.Remove(toTime[index_2]);
                                IDs.Remove(IDs[index_2]);
                            }
                        }
                    }
                }

                //End
                for (int index_1 = 0; index_1 < fromTime.Count; index_1++)
                {
                    string wh = string.Format("<wh><tfrom>{0}</tfrom><tto>{1}</tto><tdays>{2}</tdays><id>{3}</id></wh>", fromTime[index_1], toTime[index_1], workingHrs[index_1], IDs[index_1]);
                    if (sbWorkingHrs.Length == 0)
                    {
                        sbWorkingHrs.Append("<ROOT>");
                        sbWorkingHrs.Append(wh);
                    }
                    else
                    {
                        sbWorkingHrs.Append(wh);
                    }

                }

                //foreach (GridDataItem item in gdWorkingHr.Items)
                //{
                //    RadTimePicker cboFrom = (RadTimePicker)item.FindControl("cboFrom");
                //    RadTimePicker cboTo = (RadTimePicker)item.FindControl("cboTo");
                //    CheckBox chkSun = (CheckBox)item.FindControl("chkSun");
                //    CheckBox chkMon = (CheckBox)item.FindControl("chkMon");
                //    CheckBox chkTue = (CheckBox)item.FindControl("chkTue");
                //    CheckBox chkWed = (CheckBox)item.FindControl("chkWed");
                //    CheckBox chkThu = (CheckBox)item.FindControl("chkThu");
                //    CheckBox chkFri = (CheckBox)item.FindControl("chkFri");
                //    CheckBox chkSat = (CheckBox)item.FindControl("chkSat");
                //    CheckBox chkHoliday = (CheckBox)item.FindControl("chkHoliday");
                //    HiddenField hidID = (HiddenField)item.FindControl("hidID");
                //    int timezone = int.Parse(cboTimeZone.SelectedValue);
                //    if (cboTo.SelectedDate.HasValue && cboFrom.SelectedDate.HasValue)
                //    {
                //        string days = string.Empty;
                //        if (cboTo.SelectedDate.Value > cboFrom.SelectedDate.Value)
                //        {
                //            if (chkMon.Checked)
                //            {
                //                if (days == string.Empty) days = "MON";
                //                else days = days + ",MON";
                //            }
                //            if (chkTue.Checked)
                //            {
                //                if (days == string.Empty) days = "TUE";
                //                else days = days + ",TUE";
                //            }
                //            if (chkWed.Checked)
                //            {
                //                if (days == string.Empty) days = "WED";
                //                else days = days + ",WED";
                //            }
                //            if (chkThu.Checked)
                //            {
                //                if (days == string.Empty) days = "THU";
                //                else days = days + ",THU";
                //            }
                //            if (chkFri.Checked)
                //            {
                //                if (days == string.Empty) days = "FRI";
                //                else days = days + ",FRI";
                //            }
                //            if (chkSat.Checked)
                //            {
                //                if (days == string.Empty) days = "SAT";
                //                else days = days + ",SAT";
                //            }

                //            if (chkSun.Checked)
                //            {
                //                if (days == string.Empty) days = "SUN";
                //                else days = days + ",SUN";
                //            }
                //            if (chkHoliday.Checked)
                //            {
                //                if (days == string.Empty) days = "HOL";
                //                else days = days + ",HOL";
                //            }
                //            ContactManager contactMgr = new ContactManager(sConnectionString);
                //            string tzID = contactMgr.GetTimeZonesByID(cboTimeZone.SelectedValue);
                //            int fromDt = cboFrom.SelectedDate.Value.Hour * 60 + cboFrom.SelectedDate.Value.Minute;
                //            int toDt = cboTo.SelectedDate.Value.Hour * 60 + cboTo.SelectedDate.Value.Minute;
                //            TimeZoneInfo timeInfo = System.TimeZoneInfo.FindSystemTimeZoneById(tzID);
                //            int dtcDt = (int)timeInfo.BaseUtcOffset.TotalMinutes;

                //            fromDt = fromDt - dtcDt;
                //            toDt = toDt - dtcDt;
                //            string id = "-1";
                //            if (hidID != null)
                //            {
                //               if (hidID.Value.Trim() != "") id = hidID.Value;
                //            }

                //            string wh = string.Format("<wh><tfrom>{0}</tfrom><tto>{1}</tto><tdays>{2}</tdays><id>{3}</id></wh>", fromDt, toDt, days, id);
                //            Boolean isNew = false;
                //            if (sbWorkingHrs.Length == 0)
                //            {
                //                sbWorkingHrs.Append("<ROOT>");
                //                sbWorkingHrs.Append(wh);
                //                isNew = true;
                //            }
                //            else
                //            {
                //                if (sbWorkingHrs.ToString().IndexOf(wh) < 0)
                //                {
                //                    sbWorkingHrs.Append(wh);
                //                    isNew = true;
                //                }
                //            }
                //            if (isNew)
                //            {
                //                fromTime.Add(fromDt);
                //                toTime.Add(toDt);
                //                workingHrs.Add(days);
                //            }

                //        }
                //    }
                //}
                if (sbWorkingHrs.Length > 0) sbWorkingHrs.Append("</ROOT>");
                if (fromTime.Count > 0)
                {
                    //System.TimeZoneInfo
                    WorkingHoursManager whMgr = new WorkingHoursManager(sConnectionString);
                    int workingHoursRangeId = -1;
                       //whMgr.CheckWorkingHoursUTC(fromTime, toTime, workingHrs, sn.User.OrganizationId);
                    whMgr.FleetWorkingHours_add(fleedID, email, sbWorkingHrs.ToString(),
                        exeption, sn.User.OrganizationId, workingHoursRangeId, int.Parse(cboTimeZone.SelectedValue), sn.UserID);
                    string saveScript = string.Format("alert('{0}')", saveSucceed);
                    RadAjaxManager1.ResponseScripts.Add(saveScript);

                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                string errorScript = string.Format("alert('{0}')", errorSave);
                RadAjaxManager1.ResponseScripts.Add(errorScript);
            }
        }
        private string GetCheckedException()
        {
            StringBuilder vehicleIDs = new StringBuilder();
            foreach (RadComboBoxItem item in cboException.Items)
            {
                CheckBox chkException = (CheckBox)item.FindControl("chkException");
                if (chkException != null && chkException.Checked)
                {
                    if (vehicleIDs.Length == 0) vehicleIDs.Append(item.Value);
                    else vehicleIDs.Append("," + item.Value);
                }
            }
            return vehicleIDs.ToString();
        }

        private void GetWorkingHoursUTCByFleetId(string fleetID)
        {
            try
            {
                int timeZoneID = -1;
                WorkingHoursManager whMgr = new WorkingHoursManager(sConnectionString);
                DataSet ds = whMgr.GetWorkingHoursUTCByFleetId(fleetID);
                if (ds != null)
                {
                    emaildt.Rows.Clear();
                    int index = 0;
                    foreach (DataRow dr in ds.Tables[0].Rows) 
                    {
                        if (timeZoneID == -1 && !(dr["TimeZone"] is DBNull))
                        {
                            int.TryParse(dr["TimeZone"].ToString(), out timeZoneID);
                        }
                        
                        if (!(dr["Email"] is DBNull))
                        {
                            DataRow newDr = emaildt.NewRow();
                            newDr["email"] = dr["Email"].ToString();
                            if (!(dr["ID"] is DBNull))
                            {
                                newDr["id"] = dr["ID"].ToString();
                            }
                            emaildt.Rows.Add(newDr);
                        }
                    }
                    int dtcDt = 0;
                    if (timeZoneID >= 0)
                    {
                        ContactManager contactMgr = new ContactManager(sConnectionString);
                        string tzID = contactMgr.GetTimeZonesByID(timeZoneID.ToString());
                        TimeZoneInfo timeInfo = System.TimeZoneInfo.FindSystemTimeZoneById(tzID);
                        dtcDt = (int)timeInfo.BaseUtcOffset.TotalMinutes;
                        cboTimeZone.ClearSelection();
                        cboTimeZone.FindItemByValue(timeZoneID.ToString()).Selected = true;
                    }


                    //New Begin
                    for (int index_1 = 0; index_1 < ds.Tables[1].Rows.Count; index_1++)
                    {
                        for (int index_2 = index_1 + 1; index_2 < ds.Tables[1].Rows.Count; index_2++)
                        {
                            int fromTime_1 = -1;
                            int fromTime_2 = -1;
                            int toTime_1 = -1;
                            int toTime_2 = -1;
                            string days_1 = string.Empty;
                            string days_2 = string.Empty;
                            DataRow dr_1 = ds.Tables[1].Rows[index_1];
                            DataRow dr_2 = ds.Tables[1].Rows[index_2];
                            if (!(dr_1["From"] is DBNull)) int.TryParse(dr_1["From"].ToString(), out fromTime_1);
                            if (!(dr_1["To"] is DBNull)) int.TryParse(dr_1["To"].ToString(), out toTime_1);
                            if (!(dr_1["WorkingDays"] is DBNull)) days_1 = dr_1["WorkingDays"].ToString();

                            if (!(dr_2["From"] is DBNull)) int.TryParse(dr_2["From"].ToString(), out fromTime_2);
                            if (!(dr_2["To"] is DBNull)) int.TryParse(dr_2["To"].ToString(), out toTime_2);
                            if (!(dr_2["WorkingDays"] is DBNull)) days_2 = dr_2["WorkingDays"].ToString();

                            if (days_1 == days_2)
                            {
                                if ((fromTime_1 <= toTime_2 && toTime_1 >= fromTime_2) ||
                                    toTime_1 + 1 == fromTime_2 || fromTime_2 + 1 == fromTime_1)
                                {
                                    if (fromTime_1 > fromTime_2) dr_1["From"] = dr_2["From"];
                                    if (toTime_1 < toTime_2) dr_1["To"] = dr_2["To"];
                                    ds.Tables[1].Rows.Remove(dr_2);
                                }
                            }
                        }
                    }

                    //End


                    dt.Rows.Clear();
                    index = 0;
                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        DataRow newDr = dt.NewRow();

                        int fromDt = 0;
                        int toDt = 0;
                        if (!(dr["From"] is DBNull))
                        {
                            int.TryParse(dr["From"].ToString(), out fromDt);
                            fromDt = fromDt + dtcDt;
                            newDr["From"] = DateTime.Now.Date.AddMinutes(fromDt);

                        }
                        if (!(dr["To"] is DBNull))
                        {
                            int.TryParse(dr["To"].ToString(), out toDt);
                            toDt = toDt + dtcDt;
                            newDr["To"] = DateTime.Now.Date.AddMinutes(toDt);
                        }

                        if (!(dr["WorkingDays"] is DBNull))
                        {
                            string workingDays = "," + dr["WorkingDays"].ToString() + ",";
                            if (workingDays.IndexOf(",MON,") >= 0) newDr["chkMon"] = "True";
                            if (workingDays.IndexOf(",TUE,") >= 0) newDr["chkTue"] = "True";
                            if (workingDays.IndexOf(",WED,") >= 0) newDr["chkWed"] = "True";
                            if (workingDays.IndexOf(",THU,") >= 0) newDr["chkThu"] = "True";
                            if (workingDays.IndexOf(",FRI,") >= 0) newDr["chkFri"] = "True";
                            if (workingDays.IndexOf(",SAT,") >= 0) newDr["chkSat"] = "True";
                            if (workingDays.IndexOf(",SUN,") >= 0) newDr["chkSun"] = "True";
                            if (workingDays.IndexOf(",HOL,") >= 0) newDr["chkHoliday"] = "True";
                        }

                        if (!(dr["WorkingHoursId"] is DBNull))
                        {
                            newDr["id"] = dr["WorkingHoursId"].ToString();
                        }
                        dt.Rows.Add(newDr);
                    }

                    StringBuilder exceptionVehicle = new StringBuilder();
                    foreach (DataRow dr in ds.Tables[2].Rows)
                    {
                        if (dr["VehicleId"] != DBNull.Value)
                        {
                            exceptionVehicle.Append("," + dr["VehicleId"].ToString() + ",");
                        }
                    }
                    lstException.Items.Clear();
                    foreach (RadComboBoxItem rcb in cboException.Items)
                    {
                        if (exceptionVehicle.ToString().IndexOf("," + rcb.Value + ",") >= 0)
                        {
                             CheckBox chkException = (CheckBox)rcb.FindControl("chkException");
                             if (chkException != null)
                             {
                                 chkException.Checked = true;
                             }
                             lstException.Items.Add(rcb.Text);
                        }
                    }
                }
                gdEmails.DataSource = emaildt;
                gdEmails.DataBind();

                if (dt.Rows.Count == 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["id"] = "";
                    dr["From"] = DateTime.Now.Date;
                    dr["To"] = DateTime.Now.Date.AddHours(23).AddMinutes(59);
                    dt.Rows.Add(dr);
                }
                gdWorkingHr.DataSource = dt;
                gdWorkingHr.DataBind();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                string errorScript = string.Format("alert('{0}')", errorLoad);
                RadAjaxManager1.ResponseScripts.Add(errorScript);
            }
            
        }

        private void ResetExceptionList()
        {
            lstException.Items.Clear();
            foreach (RadComboBoxItem rcb in cboException.Items)
            {
                    CheckBox chkException = (CheckBox)rcb.FindControl("chkException");
                    if (chkException != null)
                    {
                        if (chkException.Checked)  lstException.Items.Add(rcb.Text);
                    }
            }

        }

        // Changes for TimeZone Feature start
        private Boolean FindUserTimeZone_NewTZ()
        {
            if (sn.User.NewFloatTimeZone == -4)
            {
                cboTimeZone.ClearSelection();
                cboTimeZone.Items.FindItemByValue("77").Selected = true;
                return true;
            }
            if (sn.User.NewFloatTimeZone == -5)
            {
                cboTimeZone.ClearSelection();
                cboTimeZone.Items.FindItemByValue("83").Selected = true;
                return true;
            }
            if (sn.User.NewFloatTimeZone == -6)
            {
                cboTimeZone.ClearSelection();
                cboTimeZone.Items.FindItemByValue("85").Selected = true;
                return true;
            }
            if (sn.User.NewFloatTimeZone == -7)
            {
                cboTimeZone.ClearSelection();
                cboTimeZone.Items.FindItemByValue("91").Selected = true;
                return true;
            }
            if (sn.User.NewFloatTimeZone == -8)
            {
                cboTimeZone.ClearSelection();
                cboTimeZone.Items.FindItemByValue("93").Selected = true;
                return true;
            }
            return false;
        }
        // Changes for TimeZone Feature end

        private Boolean FindUserTimeZone()
        {
            if (sn.User.TimeZone == -4)
            {
                cboTimeZone.ClearSelection();
                cboTimeZone.Items.FindItemByValue("77").Selected = true;
                return true;
            }
            if (sn.User.TimeZone == -5)
            {
                cboTimeZone.ClearSelection();
                cboTimeZone.Items.FindItemByValue("83").Selected = true;
                return true;
            }
            if (sn.User.TimeZone == -6)
            {
                cboTimeZone.ClearSelection();
                cboTimeZone.Items.FindItemByValue("85").Selected = true;
                return true;
            }
            if (sn.User.TimeZone == -7)
            {
                cboTimeZone.ClearSelection();
                cboTimeZone.Items.FindItemByValue("91").Selected = true;
                return true;
            }
            if (sn.User.TimeZone == -8)
            {
                cboTimeZone.ClearSelection();
                cboTimeZone.Items.FindItemByValue("93").Selected = true;
                return true;
            }
            return false;
        }

        protected void hidOrganizationHierarchyPostBack_Click(object sender, EventArgs e)
        {
            int fleetId = 0;
            int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
            //if (fleetId > 0)
            //    CboVehicle_Fill(fleetId);
            //else
            //    this.cboVehicle.Items.Clear();

            if (fleetId <= 0)
            {
                cboException.Items.Clear();
                pnlInfo.Visible = false;
            }
            else
            {
                CboVehicle_Fill(fleetId);
                GetWorkingHoursUTCByFleetId(fleetId.ToString());
                pnlInfo.Visible = true;
            }
        }
}
}