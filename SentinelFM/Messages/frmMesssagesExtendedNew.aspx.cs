using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Globalization;
using System.IO;
using VLF.CLS;
using GarminFMI;

namespace SentinelFM
{
    public partial class Messages_frmMesssagesExtendedNew : SentinelFMBasePage
    {

        public bool ShowOrganizationHierarchy;
        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        //public string LoadVehiclesBasedOn = "fleet";
        public string OrganizationHierarchyPath = "";
        VLF.PATCH.Logic.PatchOrganizationHierarchy poh;

        public bool MutipleUserHierarchyAssignment = false;

        public string MESSAGE_DATA = "{}";
        public string DESTINATION_DATA = "{}";

        public string QueryType = "";

        private string CurrentUICulture = "en-US";
        private string DateFormat = "MM/dd/yyyy";
        private string TimeFormat = "hh:mm:ss tt";

        protected void Page_Load(object sender, EventArgs e)
        {
            string request = Request["QueryType"];
            if (!string.IsNullOrEmpty(request))
                QueryType = request;

            request = Request["exportType"];
            string exportType = string.Empty;
            if (!string.IsNullOrEmpty(request))
                exportType = request;

            if (QueryType == "export" && exportType == "message")
            {
                exportMessage(Request["exportformat"].ToString());
                return;
            }
            else if (QueryType == "export" && exportType == "destination")
            {
                exportDestination(Request["exportformat"].ToString());
                return;
            }

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
                        hidOrganizationHierarchyFleetId.Value = DefaultOrganizationHierarchyFleetId.ToString();

                        if (MutipleUserHierarchyAssignment)
                        {
                            hidOrganizationHierarchyFleetId.Value = sn.User.PreferFleetIds;
                            hidOrganizationHierarchyNodeCode.Value = sn.User.PreferNodeCodes;
                            //DefaultOrganizationHierarchyFleetId = sn.User.PreferFleetIds;
                            DefaultOrganizationHierarchyFleetId = -1;
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
                    valFleet.Enabled = false;
                    lblOhTitle.Visible = true;
                    btnOrganizationHierarchyNodeCode.Visible = true;
                    hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);

                    //if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyNodeCode.Value.Contains(","))
                    //    btnOrganizationHierarchyNodeCode.Text = GetLocalResourceObject("ResMultipleHierarchies").ToString(); // "Multiple Hierarchies";
                }


                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmMessagesForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    this.tblDestinationsButtons.Visible = false;


                    string datetime = "";

                    if (Request[this.txtFrom.UniqueID] != null)
                        datetime = Convert.ToDateTime(Request[this.txtFrom.UniqueID].ToString()).ToString(sn.User.DateFormat);
                    else if (!String.IsNullOrEmpty(sn.Report.FromDate))
                        datetime = Convert.ToDateTime(sn.Report.FromDate).ToString(sn.User.DateFormat);
                    else
                        datetime = DateTime.Now.ToString(sn.User.DateFormat);

                    this.txtFrom.SelectedDate = ConvertStringToDateTime(datetime, sn.User.DateFormat, CurrentUICulture);
                    sn.Message.FromDate = txtFrom.SelectedDate.ToString();

                    if (Request[this.txtTo.UniqueID] != null)
                        datetime = Convert.ToDateTime(Request[this.txtTo.UniqueID].ToString()).ToString(sn.User.DateFormat);
                    else if (!String.IsNullOrEmpty(sn.Report.ToDate))
                        datetime = Convert.ToDateTime(sn.Report.ToDate).ToString(sn.User.DateFormat);
                    else
                        datetime = DateTime.Now.AddDays(1).ToString(sn.User.DateFormat);

                    this.txtTo.SelectedDate = ConvertStringToDateTime(datetime, sn.User.DateFormat, CurrentUICulture);
                    sn.Message.ToDate = txtTo.SelectedDate.ToString();

                    DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

                    cboHoursFrom.TimeView.TimeFormat = sn.User.TimeFormat;
                    cboHoursFrom.DateInput.DateFormat = sn.User.TimeFormat;
                    cboHoursFrom.DateInput.DisplayDateFormat = sn.User.TimeFormat;
                    cboHoursFrom.SelectedDate = ConvertStringToDateTime(time.ToString(), sn.User.DateFormat + " " + sn.User.TimeFormat);
                    sn.History.FromHours = cboHoursFrom.SelectedDate.ToString();

                    cboHoursTo.TimeView.TimeFormat = sn.User.TimeFormat;
                    cboHoursTo.DateInput.DateFormat = sn.User.TimeFormat;
                    cboHoursTo.DateInput.DisplayDateFormat = sn.User.TimeFormat;
                    cboHoursTo.SelectedDate = ConvertStringToDateTime(time.ToString(), sn.User.DateFormat + " " + sn.User.TimeFormat);
                    sn.History.ToHours = cboHoursTo.SelectedDate.ToString();

                    CurrentUICulture = System.Globalization.CultureInfo.CurrentUICulture.ToString();

                    txtFrom.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                    txtFrom.DateInput.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;

                    txtTo.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                    txtTo.DateInput.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;

                    if (CurrentUICulture.ToLower().IndexOf("fr") >= 0)
                    {
                        //DateFormat = "dd/MM/yyyy";
                        //TimeFormat = "HH:mm:ss";

                        txtFrom.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_fr;
                        txtFrom.DateInput.DateFormat = sn.User.DateFormat;
                        txtFrom.DateInput.DisplayDateFormat = sn.User.DateFormat;
                        txtFrom.Calendar.DayCellToolTipFormat = "dddd dd, MMMM yyyy";

                        txtTo.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_fr;
                        txtTo.DateInput.DateFormat = sn.User.DateFormat;
                        txtTo.DateInput.DisplayDateFormat = sn.User.DateFormat;
                        txtTo.Calendar.DayCellToolTipFormat = "dddd dd, MMMM yyyy";

                        cboHoursFrom.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButtonToolTip");
                        cboHoursTo.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButtonToolTip");
                        cboHoursFrom.TimeView.HeaderText = (string)base.GetLocalResourceObject("cboHoursResource1.TimeView-HeaderText");
                        cboHoursTo.TimeView.HeaderText = (string)base.GetLocalResourceObject("cboHoursResource1.TimeView-HeaderText");
                    }
                    else
                    {
                        //DateFormat = "MM/dd/yyyy";
                        //TimeFormat = "hh:mm:ss tt";

                        txtFrom.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_en;
                        txtFrom.DateInput.DateFormat = sn.User.DateFormat;
                        txtFrom.DateInput.DisplayDateFormat = sn.User.DateFormat;

                        txtTo.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_en;
                        txtTo.DateInput.DateFormat = sn.User.DateFormat;
                        txtTo.DateInput.DisplayDateFormat = sn.User.DateFormat;
                    }

                    cboHoursFrom.TimeView.TimeFormat = sn.User.TimeFormat;
                    cboHoursFrom.DateInput.DateFormat = sn.User.TimeFormat;
                    cboHoursFrom.DateInput.DisplayDateFormat = sn.User.TimeFormat;
                    cboHoursFrom.SelectedDate = ConvertStringToDateTime(time.ToString(), sn.User.DateFormat + " " + sn.User.TimeFormat);

                    cboHoursTo.TimeView.TimeFormat = sn.User.TimeFormat;
                    cboHoursTo.DateInput.DateFormat = sn.User.TimeFormat;
                    cboHoursTo.DateInput.DisplayDateFormat = sn.User.TimeFormat;
                    cboHoursTo.SelectedDate = ConvertStringToDateTime(time.ToString(), sn.User.DateFormat + " " + sn.User.TimeFormat);

                    CboFleet_Fill();

                    if (sn.Message.FleetId != 0)
                    {
                        this.cboFleet.SelectedIndex = -1;
                        for (int i = 0; i <= cboFleet.Items.Count - 1; i++)
                        {
                            if (cboFleet.Items[i].Value == sn.Message.FleetId.ToString())
                                cboFleet.Items[i].Selected = true;
                        }
                    }

                    else
                    {
                        if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                        {
                            if (MutipleUserHierarchyAssignment)
                            {
                                CboVehicle_MultipleFleet_Fill(hidOrganizationHierarchyFleetId.Value);
                                this.lblVehicleName.Visible = true;
                                this.cboVehicle.Visible = true;
                            }
                            else if (DefaultOrganizationHierarchyFleetId > 0)
                            {
                                CboVehicle_Fill(DefaultOrganizationHierarchyFleetId);
                                this.lblVehicleName.Visible = true;
                                this.cboVehicle.Visible = true;
                            }
                        }
                        else if (sn.User.DefaultFleet != -1)
                        {
                            cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                            CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));
                            this.lblVehicleName.Visible = true;
                            this.cboVehicle.Visible = true;
                        }
                    }

                    int fleetId = (sn.User.LoadVehiclesBasedOn == "hierarchy") ? DefaultOrganizationHierarchyFleetId : Convert.ToInt32(cboFleet.SelectedItem.Value);
                    string fleetIds = string.Empty;
                    if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
                        fleetIds = hidOrganizationHierarchyFleetId.Value;

                    if (fleetId != -1 || fleetIds != string.Empty)
                    {

                        if (fleetIds != string.Empty)
                            CboVehicle_MultipleFleet_Fill(fleetIds);
                        else
                            CboVehicle_Fill(fleetId);

                        this.cboVehicle.SelectedIndex = -1;

                        for (int i = 0; i <= cboVehicle.Items.Count - 1; i++)
                        {
                            if (cboVehicle.Items[i].Value == sn.Message.BoxId.ToString())
                                cboVehicle.Items[i].Selected = true;
                        }

                        this.lblVehicleName.Visible = true;
                        this.cboVehicle.Visible = true;


                        SetDirection();


                        sn.Message.FromDate = Convert.ToDateTime(sn.Message.FromDate).ToShortDateString() + " " + sn.Message.FromHours;
                        sn.Message.ToDate = Convert.ToDateTime(sn.Message.ToDate).ToShortDateString() + " " + sn.Message.ToHours;
                        dgMessages_Fill_NewTZ();
                        //this.BusyReport.Title = (string)base.GetLocalResourceObject("BusyPleaseWaitMessage");
                    }
                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
        }
    
        protected void cmdShowMessages_Click(object sender, EventArgs e)
        {
            try
            {
                string strFromDate = "";
                string strToDate = "";

                PrepareDates(ref strFromDate, ref strToDate);

                this.lblMessage.Text = "";
                CultureInfo ci = new CultureInfo("en-US");

                if (Convert.ToDateTime(strFromDate, ci) > Convert.ToDateTime(strToDate, ci))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidFromToDate");
                    return;
                }
                else
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "";
                }

                string fleetname = string.Empty;
                if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                {
                    int fleetId = -1;
                    int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
                    fleetname = poh.GetFleetNameByFleetId(sn.User.OrganizationId, fleetId).Replace("'", "''");
                }
                else
                {
                    fleetname = this.cboFleet.SelectedItem.Text.Replace("'", "''");
                }

                string fromDate = Convert.ToDateTime(strFromDate).AddHours(cboHoursFrom.SelectedDate.Value.Hour + cboHoursFrom.SelectedDate.Value.Minute + cboHoursFrom.SelectedDate.Value.Second).ToString();
                string toDate = Convert.ToDateTime(strToDate).AddHours(cboHoursTo.SelectedDate.Value.Hour + cboHoursTo.SelectedDate.Value.Minute + cboHoursTo.SelectedDate.Value.Second).ToString();

                sn.Message.FromDate = fromDate;
                sn.Message.ToDate = toDate;

                //sn.Message.FromDate = strFromDate;
                //sn.Message.ToDate = strToDate;
                //sn.Message.FromHours = this.cboHoursFrom.SelectedItem.Value;
                //sn.Message.ToHours = this.cboHoursTo.SelectedItem.Value;
                sn.Message.DsMessages = null;
                sn.Message.VehicleName = this.cboVehicle.SelectedItem.Text.Replace("'", "''");
                sn.Message.FleetName = fleetname;
                sn.Message.BoxId = Convert.ToInt32(this.cboVehicle.SelectedItem.Value);

                SetDirection();

                dgMessages_Fill_NewTZ();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
        }

       
        protected void cmdMarkAsRead_Click(object sender, EventArgs e)
        {
            try
            {

                DataSet ds = new DataSet();
                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();

                //foreach (string keyValue in dgMessages.RootTable.GetCheckedRows())
                foreach (string keyValue in SelectedKeyValues.Value.Split('|'))
                {
#if MDT_NEW

                string[] tmp = keyValue.Split(';');

                DataRow[] drArr = sn.Message.DsHistoryMessages.Tables[0].Select("VehicleId='" + tmp[1] + "' and MsgId='" + tmp[0]+"'");
                if (drArr == null || drArr.Length == 0)
                    continue;
                BoxId =Convert.ToInt32(drArr[0]["BoxId"].ToString());

                if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(tmp[0]), BoxId), false))
                    if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(tmp[0]),BoxId), true ))
                  {
                     System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Error in closing text message. User:" + sn.UserID.ToString() + " Form:frmMessageInfo.aspx"));
                  }


#else



                    //if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), false))
                    //    if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), true))
                    //    {
                    //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Error in closing text message. User:" + sn.UserID.ToString() + " Form:frmMessageInfo.aspx"));
                    //    }




                    DataRow[] drArr = sn.Message.DsHistoryMessages.Tables["Master"].Select("MsgKey='" + keyValue + "'");
                    if (drArr == null || drArr.Length == 0)
                        continue;



                    if (objUtil.ErrCheck(hist.SetMsgUserIdExtended(sn.UserID, sn.SecId, Convert.ToInt32(drArr[0]["MsgId"]), Convert.ToInt32(drArr[0]["DeviceType"]), Convert.ToDateTime(drArr[0]["MsgDateTime"]).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), System.DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToInt32(drArr[0]["PeripheralId"]), Convert.ToInt64(drArr[0]["checksumId"])), false))
                        if (objUtil.ErrCheck(hist.SetMsgUserIdExtended(sn.UserID, sn.SecId, Convert.ToInt32(drArr[0]["MsgId"]), Convert.ToInt32(drArr[0]["DeviceType"]), Convert.ToDateTime(drArr[0]["MsgDateTime"]).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), System.DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToInt32(drArr[0]["PeripheralId"]), Convert.ToInt64(drArr[0]["checksumId"])), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Error in closing text message. User:" + sn.UserID.ToString() + " Form:frmMessageInfo.aspx"));
                        }
#endif


                }


                dgMessages_Fill_NewTZ();
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


        protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            {
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
                this.lblVehicleName.Visible = true;
                this.cboVehicle.Visible = true;
            }
            else
                this.cboVehicle.Items.Clear();*/
            refillCboVehicle();
        }

        protected void hidOrganizationHierarchyPostBack_Click(object sender, EventArgs e)
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
                if (fleetids != string.Empty)
                    CboVehicle_MultipleFleet_Fill(fleetids);
                else
                    CboVehicle_Fill(fleetId);
                this.lblVehicleName.Visible = true;
                this.cboVehicle.Visible = true;
            }
            else
                this.cboVehicle.Items.Clear();
        }
        // Changes for TimeZone Feature start

        private void dgMessages_Fill_NewTZ()
        {
            try
            {
                //this.dgMessages.LayoutSettings.ClientVisible = true;
                StringReader strrXML = null;
                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
                CultureInfo ci = new CultureInfo("en-US");

                string strFromDT = sn.Message.FromDate;
                string strToDT = sn.Message.ToDate;
                string xml = "";

                int fleetId = -1;
                if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                {
                    int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
                }
                else
                {
                    fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);
                }

                if ((fleetId != -1) && (Convert.ToInt32(this.cboVehicle.SelectedItem.Value) == 0))
                {
                    if (objUtil.ErrCheck(hist.GetFleetAllTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, fleetId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                       if (objUtil.ErrCheck(hist.GetFleetAllTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, fleetId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
                        {
                            return;
                        }
                }
                else
                {
                    if (objUtil.ErrCheck(hist.GetVehicleAllTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                        if (objUtil.ErrCheck(hist.GetVehicleAllTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
                        {
                            return;
                        }
                }

                if (xml == "")
                {
                    sn.Message.DsHistoryMessages = null;
                    //dgMessages.RootTable.Rows.Clear();
                    //dgMessages.ClearCachedDataSource();
                    //dgMessages.RebindDataSource();
                    
                    MESSAGE_DATA = @"{
                              ""draw"": 1,
                              ""recordsTotal"": 0,
                              ""recordsFiltered"": 0,
                              ""data"": []}";
                    sn.Message.MESSAGE_DATA = MESSAGE_DATA;
                    return;
                }

                strrXML = new StringReader(xml);


                DataSet ds = new DataSet();
                ds.ReadXml(strrXML);


                // Show Combobox
                DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
                dc.DefaultValue = false;
                ds.Tables[0].Columns.Add(dc);

                DataColumn MsgKey = new DataColumn("MsgKey", Type.GetType("System.String"));
                MsgKey.DefaultValue = "";
                ds.Tables[0].Columns.Add(MsgKey);



                //DataColumn MsgDate = new DataColumn("MsgDate", Type.GetType("System.DateTime"));
                DataColumn MsgDate = new DataColumn("MsgDate", Type.GetType("System.Object"));
                ds.Tables[0].Columns.Add(MsgDate);

                if (ds.Tables[0].Columns.IndexOf("To") == -1)
                {
                    DataColumn colTo = new DataColumn("To", Type.GetType("System.String"));
                    colTo.DefaultValue = "";
                    ds.Tables[0].Columns.Add(colTo);
                }


                dc = new DataColumn("ImgUrl", Type.GetType("System.String"));
                dc.DefaultValue = "";
                ds.Tables[0].Columns.Add(dc);

                #region Resolve street address in Batch
                //try
                //{
                //    string[] addresses = null;
                //    DataRow[] drArrAddress = ds.Tables[0].Select("StreetAddress='" + VLF.CLS.Def.Const.addressNA + "'");

                //    if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicroWeb")) && drArrAddress.Length > 0)
                //        addresses = clsMap.ResolveStreetAddressGeoMicroWebBatch(drArrAddress);
                //    else if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicro")) && drArrAddress.Length > 0)
                //        addresses = clsMap.ResolveStreetAddressGeoMicroBatch(drArrAddress);

                //    int i = 0;
                //    foreach (DataRow dr in drArrAddress)
                //    {
                //        dr["StreetAddress"] = addresses[i];
                //        i++;
                //    }

                //}
                //catch (Exception Ex)
                //{
                //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:clsMap"));
                //}
                #endregion

                string strStreetAddress = "";
                string strStatus = "";
                int RowIndex = 0;
                foreach (DataRow rowItem in ds.Tables[0].Rows)
                {
                    //rowItem["MsgKey"] = rowItem["MsgId"].ToString() + ";" + rowItem["Vehicleid"].ToString();


                    rowItem["MsgKey"] = RowIndex.ToString();
                    RowIndex++;

                    //rowItem["MsgDate"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString(), ci);
                    if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
                    {
                        rowItem["MsgDate"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                    }
                    else
                    {
                        rowItem["MsgDate"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);//Convert.ToDateTime(rowItem["MsgDateTime"].ToString(), ci);
                    }

                    if (Convert.ToInt16(rowItem["DeviceType"].ToString()) == (Int16)VLF.CLS.Def.Enums.PeripheralTypes.Garmin)
                    {
                        strStatus = VLF.CLS.Util.PairFindValue("RES", rowItem["MsgBody"].ToString()).Replace("_", " ").ToUpper();
                        rowItem["MsgBody"] = VLF.CLS.Util.PairFindValue("TXT", rowItem["MsgBody"].ToString());
                        if (rowItem["status"].ToString() == "")
                            rowItem["status"] = strStatus;

                        //if ((strStatus != "") && rowItem["MsgBody"].ToString() == "")
                        //    rowItem["MsgBody"] = strStatus;
                    }

                    if (rowItem["MsgDirection"].ToString() == "In")
                        rowItem["ImgUrl"] = "images/gmin.png";
                    else
                        rowItem["ImgUrl"] = "images/gmout.png";

                    strStreetAddress = rowItem["StreetAddress"].ToString().TrimEnd();
                    //if ((strStreetAddress == VLF.CLS.Def.Const.addressNA))
                    //{
                    //    try
                    //    {
                    //        rowItem["StreetAddress"] = clsMap.ResolveStreetAddress(sn, rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString());
                    //    }
                    //    catch (Exception Ex)
                    //    {
                    //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    //    }

                    //}


                    switch (strStreetAddress)
                    {
                        case VLF.CLS.Def.Const.addressNA:
                            rowItem["StreetAddress"] = Resources.Const.InvalidAddress_addressNA;
                            break;

                        case VLF.CLS.Def.Const.noGPSData:
                            rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noGPSData;
                            break;

                        //case VLF.CLS.Def.Const.noValidAddress:
                        //    rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noValidAddress;
                        //    break;

                        default:
                            break;
                    }

                }


                DataTable dtMaster = new DataTable();
                DataTable dtDetails = new DataTable();

                dtMaster = ds.Tables[0].Clone();
                dtDetails = ds.Tables[0].Clone();
                dtMaster.TableName = "Master";
                dtDetails.TableName = "Details";

                DataRow[] drArr = ds.Tables[0].Select("RowId=1");
                if (drArr != null && drArr.Length > 0)
                {
                    foreach (DataRow dr in drArr)
                        dtMaster.ImportRow(dr);
                }

                drArr = ds.Tables[0].Select("RowId<>1");
                if (drArr != null && drArr.Length > 0)
                {
                    foreach (DataRow dr in drArr)
                        dtDetails.ImportRow(dr);
                }



                DataSet dsTmp = new DataSet();
                dsTmp.Tables.Add(dtMaster);
                dsTmp.Tables.Add(dtDetails);

                //DataColumn parentCol_MsgId = dsTmp.Tables["Master"].Columns["MsgId"];
                //DataColumn childCol_MsgId = dsTmp.Tables["Details"].Columns["MsgId"];

                //DataRelation relMsg = new DataRelation("relMsg", parentCol_MsgId, childCol_MsgId);



                DataColumn[] parentCol;
                DataColumn[] childCol;

                parentCol = new DataColumn[] { dsTmp.Tables["Master"].Columns["MsgId"], dsTmp.Tables["Master"].Columns["BoxId"] };
                childCol = new DataColumn[] { dsTmp.Tables["Details"].Columns["MsgId"], dsTmp.Tables["Details"].Columns["BoxId"] };
                DataRelation relMsg = new DataRelation("relMsg", parentCol, childCol);

                dsTmp.Relations.Add(relMsg);

                sn.Message.DsHistoryMessages = dsTmp;
                //dgMessages.ClearCachedDataSource();
                //dgMessages.RebindDataSource();
                
                string sdata = "";
                foreach (DataRow rowItem in dtMaster.Rows)
                {
                    bool rowExpandable = true;
                    DataRow[] drArray = sn.Message.DsHistoryMessages.Tables["Details"].Select("MsgId='" + rowItem["MsgId"].ToString() + "'");
                    if (drArray.Length < 1)
                    {
                        rowExpandable = false;

                    }
                    sdata += (sdata == "" ? "" : ",") + "[" +
                            "\"" + (rowExpandable ? "<img class='expandable' src=images/SD7Dz.png />" : "") + "\"" +
                            ",\"<span>" + Convert.ToDateTime(rowItem["MsgDate"].ToString()).ToString("yyyyMMddHHmmss") + "</span>" + rowItem["MsgDate"].ToString() + "\"" +
                            ", \"" + rowItem["StreetAddress"].ToString() + "\"" +
                            ", \"" + rowItem["From"].ToString() + "\"" +
                            ", \"" + rowItem["To"].ToString() + "\"" +
                            ", \"" + rowItem["MsgDirection"].ToString() + "\"" +
                            ", \"" + rowItem["MsgBody"].ToString().Replace("\"", "\\\"").Replace("\r", " ").Replace("\n", " ") + "\"" +
                            ", \"" + rowItem["Status"].ToString() + "\"" +
                            ", \"" + rowItem["Acknowledged"].ToString() + "\"" +
                            ", \"" + rowItem["UserName"].ToString() + "\"" +
                            ", \"<img src=" + rowItem["ImgUrl"].ToString() + " />\"" +
                            ", \"" + rowItem["MsgId"].ToString() + "\"" +
                            ", \"" + rowItem["BoxID"].ToString() + "\"" +
                            ", \"" + rowItem["MsgKey"].ToString() + "\"" +
                        "]";
                }

                MESSAGE_DATA = @"{
                              ""draw"": 1,
                              ""recordsTotal"": " + dtMaster.Rows.Count.ToString() + @",
                              ""recordsFiltered"": " + dtMaster.Rows.Count.ToString() + @",
                              ""data"": [" + sdata;


                MESSAGE_DATA += "]}";

                sn.Message.MESSAGE_DATA = MESSAGE_DATA;

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

        // Changes for TimeZone Feature end

        // Replaced by deMessages_Fill_NewTZ
        //private void dgMessages_Fill()
        //{
        //    try
        //    {

        //        this.dgMessages.LayoutSettings.ClientVisible = true;
        //        StringReader strrXML = null;
        //        ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
        //        CultureInfo ci = new CultureInfo("en-US");

        //        string strFromDT = sn.Message.FromDate;
        //        string strToDT = sn.Message.ToDate;
        //        string xml = "";

        //        int fleetId = -1;
        //        if (sn.User.LoadVehiclesBasedOn == "hierarchy")
        //        {
        //            int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
        //        }
        //        else
        //        {
        //            fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);
        //        }

        //        if ((fleetId != -1) && (Convert.ToInt32(this.cboVehicle.SelectedItem.Value) == 0))
        //        {
        //            if (objUtil.ErrCheck(hist.GetFleetAllTextMessagesFullInfo(sn.UserID, sn.SecId, fleetId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
        //                if (objUtil.ErrCheck(hist.GetFleetAllTextMessagesFullInfo(sn.UserID, sn.SecId, fleetId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
        //                {
        //                    return;
        //                }
        //        }
        //        else
        //        {
        //            if (objUtil.ErrCheck(hist.GetVehicleAllTextMessagesFullInfo(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
        //                if (objUtil.ErrCheck(hist.GetVehicleAllTextMessagesFullInfo(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
        //                {
        //                    return;
        //                }
        //        }

        //        if (xml == "")
        //        {
        //            sn.Message.DsHistoryMessages = null;
        //            dgMessages.RootTable.Rows.Clear();
        //            dgMessages.ClearCachedDataSource();
        //            dgMessages.RebindDataSource();

        //            return;
        //        }

        //        strrXML = new StringReader(xml);


        //        DataSet ds = new DataSet();
        //        ds.ReadXml(strrXML);


        //        // Show Combobox
        //        DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
        //        dc.DefaultValue = false;
        //        ds.Tables[0].Columns.Add(dc);

        //        DataColumn MsgKey = new DataColumn("MsgKey", Type.GetType("System.String"));
        //        MsgKey.DefaultValue = "";
        //        ds.Tables[0].Columns.Add(MsgKey);



        //        //DataColumn MsgDate = new DataColumn("MsgDate", Type.GetType("System.DateTime"));
        //        DataColumn MsgDate = new DataColumn("MsgDate", Type.GetType("System.Object"));
        //        ds.Tables[0].Columns.Add(MsgDate);

        //        if (ds.Tables[0].Columns.IndexOf("To") == -1)
        //        {
        //            DataColumn colTo = new DataColumn("To", Type.GetType("System.String"));
        //            colTo.DefaultValue = "";
        //            ds.Tables[0].Columns.Add(colTo);
        //        }


        //        dc = new DataColumn("ImgUrl", Type.GetType("System.String"));
        //        dc.DefaultValue = "";
        //        ds.Tables[0].Columns.Add(dc);

        //        #region Resolve street address in Batch
        //        //try
        //        //{
        //        //    string[] addresses = null;
        //        //    DataRow[] drArrAddress = ds.Tables[0].Select("StreetAddress='" + VLF.CLS.Def.Const.addressNA + "'");

        //        //    if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicroWeb")) && drArrAddress.Length > 0)
        //        //        addresses = clsMap.ResolveStreetAddressGeoMicroWebBatch(drArrAddress);
        //        //    else if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicro")) && drArrAddress.Length > 0)
        //        //        addresses = clsMap.ResolveStreetAddressGeoMicroBatch(drArrAddress);

        //        //    int i = 0;
        //        //    foreach (DataRow dr in drArrAddress)
        //        //    {
        //        //        dr["StreetAddress"] = addresses[i];
        //        //        i++;
        //        //    }

        //        //}
        //        //catch (Exception Ex)
        //        //{
        //        //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:clsMap"));
        //        //}
        //        #endregion

        //        string strStreetAddress = "";
        //        string strStatus = "";
        //        int RowIndex = 0;
        //        foreach (DataRow rowItem in ds.Tables[0].Rows)
        //        {
        //            //rowItem["MsgKey"] = rowItem["MsgId"].ToString() + ";" + rowItem["Vehicleid"].ToString();


        //            rowItem["MsgKey"] = RowIndex.ToString();
        //            RowIndex++;

        //            //rowItem["MsgDate"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString(), ci);
        //            if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
        //            {
        //                rowItem["MsgDate"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
        //            }
        //            else
        //            {
        //                rowItem["MsgDate"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);//Convert.ToDateTime(rowItem["MsgDateTime"].ToString(), ci);
        //            }

        //            if (Convert.ToInt16(rowItem["DeviceType"].ToString()) == (Int16)VLF.CLS.Def.Enums.PeripheralTypes.Garmin)
        //            {
        //                strStatus = VLF.CLS.Util.PairFindValue("RES", rowItem["MsgBody"].ToString()).Replace("_", " ").ToUpper();
        //                rowItem["MsgBody"] = VLF.CLS.Util.PairFindValue("TXT", rowItem["MsgBody"].ToString());
        //                if (rowItem["status"].ToString()=="") 
        //                    rowItem["status"] = strStatus;

        //                //if ((strStatus != "") && rowItem["MsgBody"].ToString() == "")
        //                //    rowItem["MsgBody"] = strStatus;
        //            }

        //            if (rowItem["MsgDirection"].ToString()  == "In")
        //                rowItem["ImgUrl"] = "images/gmin.png";
        //            else
        //                rowItem["ImgUrl"] = "images/gmout.png";

        //            strStreetAddress = rowItem["StreetAddress"].ToString().TrimEnd();
        //            //if ((strStreetAddress == VLF.CLS.Def.Const.addressNA))
        //            //{
        //            //    try
        //            //    {
        //            //        rowItem["StreetAddress"] = clsMap.ResolveStreetAddress(sn, rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString());
        //            //    }
        //            //    catch (Exception Ex)
        //            //    {
        //            //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        //            //    }

        //            //}


        //            switch (strStreetAddress)
        //            {
        //                case VLF.CLS.Def.Const.addressNA:
        //                    rowItem["StreetAddress"] = Resources.Const.InvalidAddress_addressNA;
        //                    break;

        //                case VLF.CLS.Def.Const.noGPSData:
        //                    rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noGPSData;
        //                    break;

        //                //case VLF.CLS.Def.Const.noValidAddress:
        //                //    rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noValidAddress;
        //                //    break;

        //                default:
        //                    break;
        //            }

        //        }


        //        DataTable dtMaster = new DataTable();
        //        DataTable dtDetails = new DataTable();

        //        dtMaster = ds.Tables[0].Clone();
        //        dtDetails = ds.Tables[0].Clone();
        //        dtMaster.TableName = "Master";
        //        dtDetails.TableName = "Details";

        //        DataRow[] drArr = ds.Tables[0].Select("RowId=1");
        //        if (drArr != null && drArr.Length > 0)
        //        {
        //            foreach (DataRow dr in drArr)
        //                dtMaster.ImportRow(dr);
        //        }

        //        drArr = ds.Tables[0].Select("RowId<>1");
        //        if (drArr != null && drArr.Length > 0)
        //        {
        //            foreach (DataRow dr in drArr)
        //                dtDetails.ImportRow(dr);
        //        }



        //        DataSet dsTmp = new DataSet();
        //        dsTmp.Tables.Add(dtMaster);
        //        dsTmp.Tables.Add(dtDetails);

        //        //DataColumn parentCol_MsgId = dsTmp.Tables["Master"].Columns["MsgId"];
        //        //DataColumn childCol_MsgId = dsTmp.Tables["Details"].Columns["MsgId"];

        //        //DataRelation relMsg = new DataRelation("relMsg", parentCol_MsgId, childCol_MsgId);



        //        DataColumn[] parentCol;
        //        DataColumn[] childCol;

        //        parentCol = new DataColumn[] {dsTmp.Tables["Master"].Columns["MsgId"], dsTmp.Tables["Master"].Columns["BoxId"]};
        //        childCol = new DataColumn[] { dsTmp.Tables["Details"].Columns["MsgId"], dsTmp.Tables["Details"].Columns["BoxId"] };
        //        DataRelation relMsg = new DataRelation("relMsg", parentCol, childCol);

        //        dsTmp.Relations.Add(relMsg);

        //        sn.Message.DsHistoryMessages = dsTmp;
        //        dgMessages.ClearCachedDataSource();
        //        dgMessages.RebindDataSource();



        //    }
        //    catch (NullReferenceException Ex)
        //    {
        //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
        //        RedirectToLogin();
        //    }

        //    catch (Exception Ex)
        //    {
        //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        //    }
        //}

        // Changes for TimeZone Feature start

        private void dgDestinations_Fill_NewTZ()
        {
            try
            {

                //this.dgDestinations.LayoutSettings.ClientVisible = true;
                StringReader strrXML = null;
                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
                CultureInfo ci = new CultureInfo("en-US");

                string strFromDT = sn.Message.FromDate;
                string strToDT = sn.Message.ToDate;
                string xml = "";

                int fleetId = -1;
                if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                {
                    int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
                }
                else
                {
                    fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);
                }

                if ((fleetId != -1) && (Convert.ToInt32(this.cboVehicle.SelectedItem.Value) == 0))
                {
                    if (objUtil.ErrCheck(hist.GetFleetAllDestinations_NewTZ(sn.UserID, sn.SecId, fleetId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                        if (objUtil.ErrCheck(hist.GetFleetAllDestinations_NewTZ(sn.UserID, sn.SecId, fleetId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
                        {
                            return;
                        }
                }
                else
                {
                    if (objUtil.ErrCheck(hist.GetVehicleAllDestinations_NewTZ(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                        if (objUtil.ErrCheck(hist.GetVehicleAllDestinations_NewTZ(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString(), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
                        {
                            return;
                        }
                }

                if (xml == "")
                {
                    sn.Message.DsGarminLocations = null;
                    //dgDestinations.RootTable.Rows.Clear();
                    //dgDestinations.ClearCachedDataSource();
                    //dgDestinations.RebindDataSource();
                    
                    DESTINATION_DATA = @"{
                              ""draw"": 1,
                              ""recordsTotal"": 0,
                              ""recordsFiltered"": 0,
                              ""data"": []}";                    
                    return;
                }

                strrXML = new StringReader(xml);


                DataSet ds = new DataSet();
                ds.ReadXml(strrXML);


                // Show Combobox
                DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
                dc.DefaultValue = false;
                ds.Tables[0].Columns.Add(dc);

                DataColumn MsgKey = new DataColumn("MsgKey", Type.GetType("System.String"));
                MsgKey.DefaultValue = "";
                ds.Tables[0].Columns.Add(MsgKey);


                dc = new DataColumn("location", Type.GetType("System.String"));
                dc.DefaultValue = false;
                ds.Tables[0].Columns.Add(dc);

                dc = new DataColumn("CustomUrl", Type.GetType("System.String"));
                dc.DefaultValue = false;
                ds.Tables[0].Columns.Add(dc);



                //DataColumn MsgDate = new DataColumn("MsgDate", Type.GetType("System.DateTime"));
                DataColumn MsgDate = new DataColumn("MsgDate", Type.GetType("System.Object"));
                ds.Tables[0].Columns.Add(MsgDate);

                if (ds.Tables[0].Columns.IndexOf("To") == -1)
                {
                    DataColumn colTo = new DataColumn("To", Type.GetType("System.String"));
                    colTo.DefaultValue = "";
                    ds.Tables[0].Columns.Add(colTo);
                }


                foreach (DataRow rowItem in ds.Tables[0].Rows)
                {
                    rowItem["MsgKey"] = rowItem["MsgId"].ToString() + ";" + rowItem["Vehicleid"].ToString();
                    //rowItem["MsgDate"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString(), ci);
                    if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
                    {
                        rowItem["MsgDate"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                    }
                    else
                    {
                        rowItem["MsgDate"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                    }


                    rowItem["status"] = VLF.CLS.Util.PairFindValue("STOP_STATUS", rowItem["MsgBody"].ToString()).Replace("_", " ").ToUpper();


                    //if (VLF.CLS.Util.PairFindValue(GarminFMI.JKeyWords.LNAME.ToString(), rowItem["MsgBody"].ToString()) != "")
                    //    rowItem["location"] = VLF.CLS.Util.PairFindValue(GarminFMI.JKeyWords.LNAME.ToString(), rowItem["MsgBody"].ToString());
                    //else if (VLF.CLS.Util.PairFindValue(GarminFMI.JKeyWords.ADD.ToString(), rowItem["MsgBody"].ToString()) != "")
                    //    rowItem["location"] = VLF.CLS.Util.PairFindValue(GarminFMI.JKeyWords.ADD.ToString(), rowItem["MsgBody"].ToString());
                    //else
                    //    rowItem["location"] = "LAT:" + VLF.CLS.Util.PairFindValue("LAT", rowItem["MsgBody"].ToString()) + " LON:" + VLF.CLS.Util.PairFindValue("LON", rowItem["MsgBody"].ToString());


                    rowItem["location"] = rowItem["StreetAddress"].ToString();
                    rowItem["MsgBody"] = VLF.CLS.Util.PairFindValue("TXT", rowItem["MsgBody"].ToString());

                }

                DataTable dtMaster = new DataTable();
                DataTable dtDetails = new DataTable();

                dtMaster = ds.Tables[0].Clone();
                dtDetails = ds.Tables[0].Clone();
                dtMaster.TableName = "Master";
                dtDetails.TableName = "Details";

                DataRow[] drArr = ds.Tables[0].Select("RowId=1");
                if (drArr != null && drArr.Length > 0)
                {
                    foreach (DataRow dr in drArr)
                        dtMaster.ImportRow(dr);
                }

                drArr = ds.Tables[0].Select("RowId<>1");
                if (drArr != null && drArr.Length > 0)
                {
                    foreach (DataRow dr in drArr)
                        dtDetails.ImportRow(dr);
                }

                DataSet dsTmp = new DataSet();
                dsTmp.Tables.Add(dtMaster);
                dsTmp.Tables.Add(dtDetails);

                DataColumn parentCol = dsTmp.Tables["Master"].Columns["MsgId"];
                DataColumn childCol = dsTmp.Tables["Details"].Columns["MsgId"];

                DataRelation relMsg = new DataRelation("relMsg", parentCol, childCol);
                dsTmp.Relations.Add(relMsg);

                sn.Message.DsGarminLocations = dsTmp;
                //sn.Message.DsGarminLocations.Tables["Master"].DefaultView.Sort = "MsgDate" + " DESC";  
                //dgDestinations.ClearCachedDataSource();
                //dgDestinations.RebindDataSource();
                
                string sdata = "";
                foreach (DataRow rowItem in dtMaster.Rows)
                {
                    bool rowExpandable = true;
                    DataRow[] drArray = sn.Message.DsGarminLocations.Tables["Details"].Select("MsgId='" + rowItem["MsgId"].ToString() + "'");
                    if (drArray.Length < 1)
                    {
                        rowExpandable = false;

                    }
                    sdata += (sdata == "" ? "" : ",") + "[" +
                            "\"" + (rowExpandable ? "<img class='expandable' src=images/SD7Dz.png />" : "") + "\"" +
                            ",\"<span>" + Convert.ToDateTime(rowItem["MsgDate"].ToString()).ToString("yyyyMMddHHmmss") + "</span>" + rowItem["MsgDate"].ToString() + "\"" +
                            ", \"" + rowItem["From"].ToString() + "\"" +
                            ", \"" + rowItem["To"].ToString() + "\"" +
                            ", \"" + rowItem["location"].ToString() + "\"" +
                            ", \"" + rowItem["MsgBody"].ToString().Replace("\"", "\\\"").Replace("\r", " ").Replace("\n", " ") + "\"" +
                            ", \"" + rowItem["Status"].ToString() + "\"" +
                            ", \"" + rowItem["Acknowledged"].ToString() + "\"" +
                            ", \"" + rowItem["UserName"].ToString() + "\"" +
                            ", \"" + rowItem["MsgId"].ToString() + "\"" +
                            ", \"" + rowItem["MsgKey"].ToString() + "\"" +
                        "]";
                }

                DESTINATION_DATA = @"{
                              ""draw"": 1,
                              ""recordsTotal"": " + dtMaster.Rows.Count.ToString() + @",
                              ""recordsFiltered"": " + dtMaster.Rows.Count.ToString() + @",
                              ""data"": [" + sdata;


                DESTINATION_DATA += "]}";



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


        // Changes for TimeZone Feature end

        //Replaced with dgDestinations_Fill_NewTZ
        //private void dgDestinations_Fill()
        //{
        //    try
        //    {

        //        this.dgDestinations.LayoutSettings.ClientVisible = true;
        //        StringReader strrXML = null;
        //        ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
        //        CultureInfo ci = new CultureInfo("en-US");

        //        string strFromDT = sn.Message.FromDate;
        //        string strToDT = sn.Message.ToDate;
        //        string xml = "";

        //        int fleetId = -1;
        //        if (sn.User.LoadVehiclesBasedOn == "hierarchy")
        //        {
        //            int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
        //        }
        //        else
        //        {
        //            fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);
        //        }

        //        if ((fleetId != -1) && (Convert.ToInt32(this.cboVehicle.SelectedItem.Value) == 0))
        //        {
        //            if (objUtil.ErrCheck(hist.GetFleetAllDestinations(sn.UserID, sn.SecId, fleetId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
        //                if (objUtil.ErrCheck(hist.GetFleetAllDestinations(sn.UserID, sn.SecId, fleetId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
        //                {
        //                    return;
        //                }
        //        }
        //        else
        //        {
        //            if (objUtil.ErrCheck(hist.GetVehicleAllDestinations (sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
        //                if (objUtil.ErrCheck(hist.GetVehicleAllDestinations(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
        //                {
        //                    return;
        //                }
        //        }

        //        if (xml == "")
        //        {
        //            sn.Message.DsGarminLocations = null;
        //            dgDestinations.RootTable.Rows.Clear();
        //            dgDestinations.ClearCachedDataSource();
        //            dgDestinations.RebindDataSource();

        //            return;
        //        }

        //        strrXML = new StringReader(xml);


        //        DataSet ds = new DataSet();
        //        ds.ReadXml(strrXML);


        //        // Show Combobox
        //        DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
        //        dc.DefaultValue = false;
        //        ds.Tables[0].Columns.Add(dc);

        //        DataColumn MsgKey = new DataColumn("MsgKey", Type.GetType("System.String"));
        //        MsgKey.DefaultValue = "";
        //        ds.Tables[0].Columns.Add(MsgKey);


        //        dc = new DataColumn("location", Type.GetType("System.String"));
        //        dc.DefaultValue = false;
        //        ds.Tables[0].Columns.Add(dc);

        //        dc = new DataColumn("CustomUrl", Type.GetType("System.String"));
        //        dc.DefaultValue = false;
        //        ds.Tables[0].Columns.Add(dc);



        //        //DataColumn MsgDate = new DataColumn("MsgDate", Type.GetType("System.DateTime"));
        //        DataColumn MsgDate = new DataColumn("MsgDate", Type.GetType("System.Object"));
        //        ds.Tables[0].Columns.Add(MsgDate);

        //        if (ds.Tables[0].Columns.IndexOf("To") == -1)
        //        {
        //            DataColumn colTo = new DataColumn("To", Type.GetType("System.String"));
        //            colTo.DefaultValue = "";
        //            ds.Tables[0].Columns.Add(colTo);
        //        }

               
        //        foreach (DataRow rowItem in ds.Tables[0].Rows)
        //        {
        //            rowItem["MsgKey"] = rowItem["MsgId"].ToString() + ";" + rowItem["Vehicleid"].ToString();
        //            //rowItem["MsgDate"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString(), ci);
        //            if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
        //            {
        //                rowItem["MsgDate"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString()).ToString(sn.User.DateFormat + " "+sn.User.TimeFormat);
        //            }
        //            else
        //            {
        //                rowItem["MsgDate"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
        //            }


        //            rowItem["status"] = VLF.CLS.Util.PairFindValue("STOP_STATUS", rowItem["MsgBody"].ToString()).Replace("_", " ").ToUpper();


        //                //if (VLF.CLS.Util.PairFindValue(GarminFMI.JKeyWords.LNAME.ToString(), rowItem["MsgBody"].ToString()) != "")
        //                //    rowItem["location"] = VLF.CLS.Util.PairFindValue(GarminFMI.JKeyWords.LNAME.ToString(), rowItem["MsgBody"].ToString());
        //                //else if (VLF.CLS.Util.PairFindValue(GarminFMI.JKeyWords.ADD.ToString(), rowItem["MsgBody"].ToString()) != "")
        //                //    rowItem["location"] = VLF.CLS.Util.PairFindValue(GarminFMI.JKeyWords.ADD.ToString(), rowItem["MsgBody"].ToString());
        //                //else
        //                //    rowItem["location"] = "LAT:" + VLF.CLS.Util.PairFindValue("LAT", rowItem["MsgBody"].ToString()) + " LON:" + VLF.CLS.Util.PairFindValue("LON", rowItem["MsgBody"].ToString());


        //            rowItem["location"] = rowItem["StreetAddress"].ToString();
        //            rowItem["MsgBody"] = VLF.CLS.Util.PairFindValue("TXT", rowItem["MsgBody"].ToString());

        //        }

        //        DataTable dtMaster = new DataTable();
        //        DataTable dtDetails = new DataTable();

        //        dtMaster = ds.Tables[0].Clone();
        //        dtDetails = ds.Tables[0].Clone();
        //        dtMaster.TableName = "Master";
        //        dtDetails.TableName = "Details";

        //        DataRow[] drArr = ds.Tables[0].Select("RowId=1");
        //        if (drArr != null && drArr.Length > 0)
        //        {
        //            foreach (DataRow dr in drArr)
        //                dtMaster.ImportRow(dr);
        //        }

        //        drArr = ds.Tables[0].Select("RowId<>1");
        //        if (drArr != null && drArr.Length > 0)
        //        {
        //            foreach (DataRow dr in drArr)
        //                dtDetails.ImportRow(dr);
        //        }

        //        DataSet dsTmp = new DataSet();
        //        dsTmp.Tables.Add(dtMaster);
        //        dsTmp.Tables.Add(dtDetails);

        //        DataColumn parentCol = dsTmp.Tables["Master"].Columns["MsgId"];
        //        DataColumn childCol = dsTmp.Tables["Details"].Columns["MsgId"];

        //        DataRelation relMsg = new DataRelation("relMsg", parentCol, childCol);
        //        dsTmp.Relations.Add(relMsg);

        //        sn.Message.DsGarminLocations = dsTmp;
        //        //sn.Message.DsGarminLocations.Tables["Master"].DefaultView.Sort = "MsgDate" + " DESC";  
        //        dgDestinations.ClearCachedDataSource();
        //        dgDestinations.RebindDataSource();



        //    }
        //    catch (NullReferenceException Ex)
        //    {
        //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
        //        RedirectToLogin();
        //    }

        //    catch (Exception Ex)
        //    {
        //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        //    }
        //}


        private void CboFleet_Fill()
        {
            try
            {

                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();
                cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));
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
                        cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                        return;
                    }
                if (xml == "")
                {
                    cboVehicle.Items.Clear();
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();

                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));

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
                        cboVehicle.Items.Clear();
                        cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                        return;
                    }
                if (xml == "")
                {
                    cboVehicle.Items.Clear();
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();

                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));

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


        private void SetDirection()
        {
            if (Convert.ToInt16(this.cboDirection.SelectedItem.Value) == 0)
                sn.Message.MessageDirectionId = Convert.ToInt32(VLF.CLS.Def.Enums.TxtMsgDirectionType.Both);

            if (Convert.ToInt16(this.cboDirection.SelectedItem.Value) == 1)
                sn.Message.MessageDirectionId = Convert.ToInt32(VLF.CLS.Def.Enums.TxtMsgDirectionType.In);

            if (Convert.ToInt16(this.cboDirection.SelectedItem.Value) == 2)
                sn.Message.MessageDirectionId = Convert.ToInt32(VLF.CLS.Def.Enums.TxtMsgDirectionType.Out);

        }

        protected void dgMessages_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {

            if (sn.Message.DsHistoryMessages != null) 
                e.DataSource = sn.Message.DsHistoryMessages;
            else
                e.DataSource = null;
        }

        protected void dgMessages_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
        {
            try
            {
                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
                {
                    e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
                    if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                        e.Layout.TextSettings.UseLanguage = "fr-FR";
                    else
                        e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();
                }
            }
            catch
            {
            }
        }
        
        protected void cmdMessages_Click(object sender, EventArgs e)
        {
            this.MultiviewMessages.ActiveViewIndex = 0;
            this.cmdMessages.CssClass = "selectedbutton";
            this.cmdDestinations.CssClass = "confbutton";

            tblDestinationsButtons.Visible = false;
            tblMessagesButtons.Visible = true;

            MESSAGE_DATA = sn.Message.MESSAGE_DATA;
        }

        protected void cmdDestinations_Click(object sender, EventArgs e)
        {
            this.MultiviewMessages.ActiveViewIndex = 1;
            this.cmdDestinations.CssClass = "selectedbutton";
            this.cmdMessages.CssClass = "confbutton";

            tblDestinationsButtons.Visible = true ;
            tblMessagesButtons.Visible = false;

            dgDestinations_Fill_NewTZ();
        }

        private void PrepareDates(ref string strFromDate, ref string strToDate)
       {
            CultureInfo ci = new CultureInfo("en-US");

            if (this.chkAuto.Checked)
            {
                string datetime = "";

                if (Request[this.txtFrom.UniqueID] != null)
                    datetime = Convert.ToDateTime(Request[this.txtFrom.UniqueID].ToString()).ToString(sn.User.DateFormat);
                else if (!String.IsNullOrEmpty(sn.Report.FromDate))
                    datetime = Convert.ToDateTime(sn.Report.FromDate).ToString(sn.User.DateFormat);
                else
                    datetime = DateTime.Now.ToString(sn.User.DateFormat);

                this.txtFrom.SelectedDate = ConvertStringToDateTime(datetime, sn.User.DateFormat, CurrentUICulture);
                sn.Message.FromDate = txtFrom.SelectedDate.ToString();

                if (Request[this.txtTo.UniqueID] != null)
                    datetime = Convert.ToDateTime(Request[this.txtTo.UniqueID].ToString()).ToString(sn.User.DateFormat);
                else if (!String.IsNullOrEmpty(sn.Report.ToDate))
                    datetime = Convert.ToDateTime(sn.Report.ToDate).ToString(sn.User.DateFormat);
                else
                    datetime = DateTime.Now.AddDays(1).ToString(sn.User.DateFormat);

                this.txtTo.SelectedDate = ConvertStringToDateTime(datetime, sn.User.DateFormat, CurrentUICulture);
                sn.Message.ToDate = txtTo.SelectedDate.ToString();

                DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

                cboHoursFrom.TimeView.TimeFormat = sn.User.TimeFormat;
                cboHoursFrom.DateInput.DateFormat = sn.User.TimeFormat;
                cboHoursFrom.DateInput.DisplayDateFormat = sn.User.TimeFormat;
                cboHoursFrom.SelectedDate = ConvertStringToDateTime(time.ToString(), sn.User.DateFormat + " " + sn.User.TimeFormat);
                sn.History.FromHours = cboHoursFrom.SelectedDate.ToString();

                cboHoursTo.TimeView.TimeFormat = sn.User.TimeFormat;
                cboHoursTo.DateInput.DateFormat = TimeFormat;
                cboHoursTo.DateInput.DisplayDateFormat = TimeFormat;
                cboHoursTo.SelectedDate = ConvertStringToDateTime(time.ToString(), DateFormat + " " + TimeFormat);
                sn.History.ToHours = cboHoursTo.SelectedDate.ToString();

                //this.txtFrom.Text = DateTime.Now.AddHours(-12).ToShortDateString();
                //sn.Message.FromDate = DateTime.Now.AddHours(-12).ToShortDateString();

                //this.txtTo.Text = DateTime.Now.ToShortDateString();
                //sn.Message.ToDate = DateTime.Now.ToShortDateString();

                //this.cboHoursFrom.SelectedIndex = -1;
                //for (int i = 0; i <= cboHoursFrom.Items.Count - 1; i++)
                //{
                //    if (Convert.ToInt32(cboHoursFrom.Items[i].Value) == DateTime.Now.AddHours(-12).Hour)
                //    {
                //        cboHoursFrom.Items[i].Selected = true;
                //        sn.History.FromHours = DateTime.Now.AddHours(-12).Hour.ToString();
                //        break;
                //    }
                //}

                //this.cboHoursTo.SelectedIndex = -1;
                //for (int i = 0; i <= cboHoursTo.Items.Count - 1; i++)
                //{
                //    if (Convert.ToInt32(cboHoursTo.Items[i].Value) == DateTime.Now.AddHours(1).Hour)
                //    {
                //        cboHoursTo.Items[i].Selected = true;
                //        sn.History.ToHours = DateTime.Now.AddHours(1).Hour.ToString();
                //        break;
                //    }
                //}

                //if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
                //    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

                //if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
                //    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

                //if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
                //    strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

                //if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
                //    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

                //if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
                //    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

                //if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
                //    strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";
            }

            else
            {
                //if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
                //    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

                //if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
                //    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

                //if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
                //    strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

                //if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
                //    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

                //if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
                //    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

                //if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
                //    strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";
            }

            //strFromDate = txtFrom.SelectedDate.Value.ToString(sn.User.DateFormat) + " " + cboHoursFrom.SelectedDate.Value.ToString(sn.User.TimeFormat);//string.Format("{0:t}", cboHoursFrom.SelectedDate.Value);
            //strToDate = this.txtTo.SelectedDate.Value.ToString(sn.User.DateFormat) + " " + this.cboHoursTo.SelectedDate.Value.ToString(sn.User.TimeFormat);//string.Format("{0:t}", this.cboHoursTo.SelectedDate.Value);

            strFromDate = txtFrom.SelectedDate.Value.ToString(); // +" " + cboHoursFrom.SelectedDate.Value.ToString(sn.User.TimeFormat);
            strToDate = txtTo.SelectedDate.Value.ToString(); // +" " + cboHoursTo.SelectedDate.Value.ToString(sn.User.TimeFormat);

            //strFromDate = Convert.ToDateTime(strFromDate).ToString();
            //strToDate = Convert.ToDateTime(strToDate).ToString();
        }

        protected void dgDestinations_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            if (sn.Message.DsGarminLocations != null)
                e.DataSource = sn.Message.DsGarminLocations;
            else
                e.DataSource = null;
        }

        protected void dgDestinations_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
        {
            try
            {
                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
                {
                    e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
                    if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                        e.Layout.TextSettings.UseLanguage = "fr-FR";
                    else
                        e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();
                }
            }
            catch
            {
            }
        }

        protected void cmdShowDestinations_Click(object sender, EventArgs e)
        {
            try
            {
                string strFromDate = "";
                string strToDate = "";

                PrepareDates(ref strFromDate, ref strToDate);

                this.lblMessage.Text = "";
                CultureInfo ci = new CultureInfo("en-US");

                if (Convert.ToDateTime(strFromDate, ci) > Convert.ToDateTime(strToDate, ci))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidFromToDate");
                    return;
                }
                else
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "";
                }

                string fleetname = string.Empty;
                if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                {
                    int fleetId = -1;
                    int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
                    fleetname = poh.GetFleetNameByFleetId(sn.User.OrganizationId, fleetId).Replace("'", "''");
                }
                else
                {
                    fleetname = this.cboFleet.SelectedItem.Text.Replace("'", "''");
                }              

                string fromDate = Convert.ToDateTime(strFromDate).AddHours(cboHoursFrom.SelectedDate.Value.Hour + cboHoursFrom.SelectedDate.Value.Minute + cboHoursFrom.SelectedDate.Value.Second).ToString();
                string toDate = Convert.ToDateTime(strToDate).AddHours(cboHoursTo.SelectedDate.Value.Hour + cboHoursTo.SelectedDate.Value.Minute + cboHoursTo.SelectedDate.Value.Second).ToString();

                sn.Message.FromDate = fromDate;
                sn.Message.ToDate = toDate;
                //sn.Message.FromHours = this.cboHoursFrom.SelectedItem.Value;
                //sn.Message.ToHours = this.cboHoursTo.SelectedItem.Value;
                sn.Message.DsGarminLocations = null;
                sn.Message.VehicleName = this.cboVehicle.SelectedItem.Text.Replace("'", "''");
                sn.Message.FleetName = fleetname;
                sn.Message.BoxId = Convert.ToInt32(this.cboVehicle.SelectedItem.Value);



                dgDestinations_Fill_NewTZ();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
        }
        //protected void cmdNewDestination_Click(object sender, EventArgs e)
        //{
        //    string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
        //    strUrl = strUrl + "	var myname='Message';";
        //    strUrl = strUrl + " var w=560;";
        //    strUrl = strUrl + " var h=420;";
        //    strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
        //    strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
        //    strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,toolbar=0,scrollbars=0,menubar=0,'; ";
        //    strUrl = strUrl + " win = window.open(mypage, myname, winprops);} ";

        //    strUrl = strUrl + " NewWindow('frmNewLocation.aspx?boxId=" + this.cboVehicle.SelectedItem.Value + "');</script>";
        //    Response.Write(strUrl);
        //}
        protected void dgMessages_InitializeRow(object sender, ISNet.WebUI.WebGrid.RowEventArgs e)
        {
            if (e.Row.Table.DataMember == "Master")
            {
                if (e.Row.Type == ISNet.WebUI.WebGrid.RowType.Record)
                {
                    DataRow[] drArr = sn.Message.DsHistoryMessages.Tables["Details"].Select("MsgId='" + e.Row.Cells.GetNamedItem("MsgId").Text + "'");
                    if (drArr.Length < 1)
                    {
                        e.Row.ChildNotExpandable = true ; 

                    }
                }
            } 

        }

        protected void dgDestinations_InitializeRow(object sender, ISNet.WebUI.WebGrid.RowEventArgs e)
        {
            if (e.Row.Table.DataMember == "Master")
            {
                if (e.Row.Type == ISNet.WebUI.WebGrid.RowType.Record)
                {
                    DataRow[] drArr = sn.Message.DsGarminLocations.Tables["Details"].Select("MsgId='" + e.Row.Cells.GetNamedItem("MsgId").Text + "'");
                    if (drArr.Length < 1)
                    {
                        e.Row.ChildNotExpandable = true;

                    }
                }
            } 
        }

        public DateTime ConvertStringToDateTime(string DateValue, string Dateformat)
        {
            return ConvertStringToDateTime(DateValue, Dateformat, System.Globalization.CultureInfo.CurrentUICulture.ToString());
        }

        public DateTime ConvertStringToDateTime(string value, string format, string cultureinfo)
        {

            CultureInfo culture = new CultureInfo(cultureinfo);
            DateTime date = DateTime.Now;
            string err = "";

            try
            {
                if (format.ToLower().IndexOf("hh") >= 0)
                    value = Convert.ToDateTime(value).ToString(format);

                date = DateTime.ParseExact(value, format, null);
                err = "";
            }
            catch (FormatException fx)
            {
                err = fx.Message;
                date = DateTime.Now;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                date = DateTime.Now;
            }
            finally
            {
            }

            return date;
        }

        private void exportMessage(string exportformat)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null)
                sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else
                return;
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName))
                return;

            if (sn.Message.DsHistoryMessages == null)
            {
                Response.Write("<script type='text/javascript'>alert('Sorry, it has no data to export.');</script>");
                return;
            }

            string MsgKeysForExport = Request["MsgKeysForExport"];
            DataTable dt = sn.Message.DsHistoryMessages.Tables["Master"].Clone();

            DataRow[] drArray = sn.Message.DsHistoryMessages.Tables["Master"].Copy().Select("MsgKey IN (" + MsgKeysForExport + ")");
            if (drArray.Length == 0)
            {
                Response.Write("<script type='text/javascript'>alert('Sorry, it has no data to export.');</script>");
                return;
            }
            foreach (DataRow dr in drArray)
            {
                dt.ImportRow(dr);
            }

            dt.DefaultView.Sort = "MsgDate DESC";
            dt = dt.DefaultView.ToTable();

            //string columns = "Date:MsgDate,Address:StreetAddress,Vehicle:vehicleDescription,Description:AlarmDescription,Level:AlarmLevel,State:AlarmState,User Name:UserName,Notes:Notes";
            string columns = (string)base.GetLocalResourceObject("dgMessages_MsgDateTime") + ":MsgDate" +
                            "," + (string)base.GetLocalResourceObject("dgMessages_Address") + ":StreetAddress" +
                            "," + (string)base.GetLocalResourceObject("dgMessages_From") + ":From" +
                            "," + (string)base.GetLocalResourceObject("dgMessages_To") + ":To" +
                            "," + (string)base.GetLocalResourceObject("dgMessages_MsgDirection") + ":MsgDirection" +
                            "," + (string)base.GetLocalResourceObject("dgMessages_MsgBody") + ":MsgBody" +
                            "," + (string)base.GetLocalResourceObject("dgMessages_Status") + ":Status" +
                            "," + (string)base.GetLocalResourceObject("dgMessages_Acknowledged") + ":Acknowledged" +
                            "," + (string)base.GetLocalResourceObject("dgMessages_UserName") + ":UserName";
            //Response.Write("test it: " + exportformat);            
            exportDatatable(dt, exportformat, columns, (string)base.GetLocalResourceObject("cmdMessagesResource2.Text"), "messages");
        }

        private void exportDestination(string exportformat)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null)
                sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else
                return;
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName))
                return;

            if (sn.Message.DsGarminLocations == null)
            {
                Response.Write("<script type='text/javascript'>alert('Sorry, it has no data to export.');</script>");
                return;
            }

            string MsgKeysForExport = Request["MsgKeysForExport"];
            DataTable dt = sn.Message.DsGarminLocations.Tables["Master"].Clone();

            DataRow[] drArray = sn.Message.DsGarminLocations.Tables["Master"].Copy().Select("MsgKey IN (" + MsgKeysForExport + ")");
            if (drArray.Length == 0)
            {
                Response.Write("<script type='text/javascript'>alert('Sorry, it has no data to export.');</script>");
                return;
            }
            foreach (DataRow dr in drArray)
            {
                dt.ImportRow(dr);
            }

            dt.DefaultView.Sort = "MsgDate DESC";
            dt = dt.DefaultView.ToTable();

            //string columns = "Date:MsgDate,Address:StreetAddress,Vehicle:vehicleDescription,Description:AlarmDescription,Level:AlarmLevel,State:AlarmState,User Name:UserName,Notes:Notes";
            string columns = (string)base.GetLocalResourceObject("dgMessages_MsgDateTime") + ":MsgDate" +
                            "," + (string)base.GetLocalResourceObject("dgMessages_From") + ":From" +
                            "," + (string)base.GetLocalResourceObject("dgMessages_To") + ":To" +
                            "," + (string)base.GetLocalResourceObject("dgGarminHist_Address") + ":location" +
                            "," + (string)base.GetLocalResourceObject("dgMessages_MsgBody") + ":MsgBody" +
                            "," + (string)base.GetLocalResourceObject("dgMessages_Status") + ":Status" +
                            "," + (string)base.GetLocalResourceObject("dgMessages_Acknowledged") + ":Acknowledged" +
                            "," + (string)base.GetLocalResourceObject("dgMessages_UserName") + ":UserName";
            //Response.Write("test it: " + exportformat);            
            exportDatatable(dt, exportformat, columns, (string)base.GetLocalResourceObject("cmdDestinationsResource2.Text"), "destinations");
        }

        private void exportDatatable(DataTable dt, string formatter, string columns, string title, string filenamePrefix)
        {
            try
            {
                if (columns == "")
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        columns += (columns == "" ? "" : ",") + column.ColumnName + ":" + column.ColumnName;
                    }
                }

                string filepath = Server.MapPath("../TempReports/");
                string filename = string.Format(@"{0}_{1}", filenamePrefix, Guid.NewGuid());
                string fileextension = "";
                string filefullpath = "";
                string filecontenttype = "";

                if (formatter == "csv")
                {
                    fileextension = "csv";
                    filefullpath = clsUtility.CreateCSV(filepath, filename, fileextension, dt, columns, "MsgDate", sn.User.DateFormat + " " + sn.User.TimeFormat, true, title);
                    filecontenttype = "application/Excel";
                }
                else if (formatter == "excel2003")
                {
                    fileextension = "xls";

                    filefullpath = clsUtility.CreateExcel2003(filepath, filename, fileextension, dt, columns, "MsgDate", sn.User.DateFormat + " " + sn.User.TimeFormat, true, title);
                    filecontenttype = "application/Excel";
                }
                else if (formatter == "excel2007")
                {
                    fileextension = "xlsx";

                    filefullpath = clsUtility.CreateExcel2007(filepath, filename, fileextension, dt, columns, "MsgDate", sn.User.DateFormat + " " + sn.User.TimeFormat, true, title);
                    filecontenttype = "application/Excel";
                }

                else if (formatter == "pdf")
                {
                    fileextension = "PDF";

                    filefullpath = clsUtility.CreatePDFFile(filepath, filename, fileextension, dt, columns, "MsgDate", sn.User.DateFormat + " " + sn.User.TimeFormat, true, title);
                    filecontenttype = "application/pdf";
                }
                else
                {
                    Response.Write("<script type='text/javascript'>alert('The file format is not supported.');</script>");
                    return;
                }

                Response.Clear();
                Response.AddHeader("Content-Type", filecontenttype);
                Response.ContentType = "application/force-download";
                Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.{1}", filename, fileextension));
                Response.TransmitFile("../TempReports/" + filename + "." + fileextension);
            }

            catch (Exception Ex)
            {
                Response.Write("<script type='text/javascript'>alert('Failed to generate the file, please try it again or choose another Excel format to export.');</script>");
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = false, XmlSerializeString = false)]
        public static string GetMessageDetails(string MsgId, string BoxID)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            clsUtility objUtil1;
            objUtil1 = new clsUtility(sn);

            try
            {
                DataRow[] drArray = sn.Message.DsHistoryMessages.Tables["Details"].Select("MsgId='" + MsgId + "' AND BoxID=" + BoxID);

                string sdata = "";

                foreach (DataRow rowItem in drArray)
                {
                    sdata += (sdata == "" ? "" : ",") + "[\"<span>" + Convert.ToDateTime(rowItem["MsgDate"].ToString()).ToString("yyyyMMddHHmmss") + "</span>" + rowItem["MsgDate"].ToString() + "\"" +
                            ", \"" + rowItem["StreetAddress"].ToString() + "\"" +
                            ", \"" + rowItem["From"].ToString() + "\"" +
                            ", \"" + rowItem["To"].ToString() + "\"" +
                            ", \"" + rowItem["MsgDirection"].ToString() + "\"" +
                            ", \"" + rowItem["MsgBody"].ToString().Replace("\"", "\\\"").Replace("\r", " ").Replace("\n", " ") + "\"" +
                            ", \"" + rowItem["Status"].ToString() + "\"" +
                            ", \"" + rowItem["UserName"].ToString() + "\"" +
                            ", \"<img src=" + rowItem["ImgUrl"].ToString() + " />\"" +
                        "]";

                }

                string sreturn = "";
                sreturn = @"{
                              ""draw"": 1,
                              ""recordsTotal"": " + drArray.Length.ToString() + @",
                              ""recordsFiltered"": " + drArray.Length.ToString() + @",
                              ""data"": [" + sdata;


                sreturn += "]}";


                return sreturn;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetMessageDetails() Page:frmMessagesExtendedNew.aspx"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "{}";
                //return "0";
            }            
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = false, XmlSerializeString = false)]
        public static string getDestinationDetails(string MsgId)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            clsUtility objUtil1;
            objUtil1 = new clsUtility(sn);

            try
            {
                DataRow[] drArray = sn.Message.DsGarminLocations.Tables["Details"].Select("MsgId='" + MsgId + "'");

                string sdata = "";

                foreach (DataRow rowItem in drArray)
                {
                    sdata += (sdata == "" ? "" : ",") + "[\"<span>" + Convert.ToDateTime(rowItem["MsgDate"].ToString()).ToString("yyyyMMddHHmmss") + "</span>" + rowItem["MsgDate"].ToString() + "\"" +
                            ", \"" + rowItem["From"].ToString() + "\"" +
                            ", \"" + rowItem["To"].ToString() + "\"" +
                            ", \"" + rowItem["MsgBody"].ToString().Replace("\"", "\\\"").Replace("\r", " ").Replace("\n", " ") + "\"" +
                            ", \"" + rowItem["Status"].ToString() + "\"" +
                            ", \"" + rowItem["UserName"].ToString() + "\"" +                            
                        "]";

                }

                string sreturn = "";
                sreturn = @"{
                              ""draw"": 1,
                              ""recordsTotal"": " + drArray.Length.ToString() + @",
                              ""recordsFiltered"": " + drArray.Length.ToString() + @",
                              ""data"": [" + sdata;


                sreturn += "]}";


                return sreturn;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: getDestinationDetails() Page:frmMessagesExtendedNew.aspx"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "{}";
                //return "0";
            }
        }
    }
}
