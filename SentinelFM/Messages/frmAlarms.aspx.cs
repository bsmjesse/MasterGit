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
using System.Globalization;
using System.IO;
using VLF.CLS;
using GarminFMI;

namespace SentinelFM
{
    public partial class Messages_frmAlarms : SentinelFMBasePage
    {
        public string ALARM_DATA = "{}";        
        public string QueryType = "";

        private string CurrentUICulture = "en-US";
        //private string DateFormat = sn.User.DateFormat;
        //private string TimeFormat = sn.User.TimeFormat;

        protected void Page_Load(object sender, EventArgs e)
        {
            string request = Request["QueryType"];
            if (!string.IsNullOrEmpty(request))
                QueryType = request;

            if (QueryType == "export")
            {
                exportAlarm(Request["exportformat"].ToString());
                return;
            }

            try
            {
                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmMessagesForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);

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
                    
                    sn.Message.FromDate = txtFrom.SelectedDate.Value.ToString(sn.User.DateFormat) + " " + string.Format("{0:t}", cboHoursFrom.SelectedDate.Value);
                    sn.Message.ToDate = this.txtTo.SelectedDate.Value.ToString(sn.User.DateFormat) + " " + string.Format("{0:t}", this.cboHoursTo.SelectedDate.Value);
                    dgAlarms_Fill_NewTZ();
                    //this.BusyReport.Title = (string)base.GetLocalResourceObject("BusyPleaseWaitMessage");

                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
        }
       

      
        protected void cmdViewAlarms_Click(object sender, EventArgs e)
        {
            try
            {
                dgAlarms_Fill_NewTZ();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
        }
        // Changes for TimeZone Feature start

        private void dgAlarms_Fill_NewTZ()
        {
            string strFromDate = "";
            string strToDate = "";
            CultureInfo ci = new CultureInfo("en-US");
            //strFromDate = txtFrom.SelectedDate.Value.ToString(sn.User.DateFormat) + " " +cboHoursFrom.SelectedDate.Value.ToString(sn.User.TimeFormat);
            //strToDate = this.txtTo.SelectedDate.Value.ToString(sn.User.DateFormat) + " " + this.cboHoursTo.SelectedDate.Value.ToString(sn.User.TimeFormat);
            strFromDate = txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + string.Format("{0:t}", cboHoursFrom.SelectedDate.Value);
            strToDate = this.txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + string.Format("{0:t}", this.cboHoursTo.SelectedDate.Value);

            //strFromDate = Convert.ToDateTime(strFromDate).ToString();
            //strToDate = Convert.ToDateTime(strToDate).ToString();

            //this.dgAlarms.LayoutSettings.ClientVisible = true;

            string xml = "";
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
            float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);

            if (objUtil.ErrCheck(alarms.GetAlarmsXML_NewTZ(sn.UserID, sn.SecId, Convert.ToDateTime(strFromDate), Convert.ToDateTime(strToDate), ref xml), false))
                if (objUtil.ErrCheck(alarms.GetAlarmsXML_NewTZ(sn.UserID, sn.SecId, Convert.ToDateTime(strFromDate), Convert.ToDateTime(strToDate), ref xml), true))
                {
                    sn.Message.DsHistoryAlarms = null;
                    //dgAlarms.ClearCachedDataSource();
                    //dgAlarms.RebindDataSource();
                    ALARM_DATA = @"{
                              ""draw"": 1,
                              ""recordsTotal"": 0,
                              ""recordsFiltered"": 0,
                              ""data"": []}";

                    return;
                }

            if (xml == "")
            {
                sn.Message.DsHistoryAlarms = null;
                //dgAlarms.ClearCachedDataSource();
                //dgAlarms.RebindDataSource();
                ALARM_DATA = @"{
                              ""draw"": 1,
                              ""recordsTotal"": 0,
                              ""recordsFiltered"": 0,
                              ""data"": []}";
                return;
            }


            if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
                xml = xml.Replace("Critical", "Critique").Replace("Warning", "Majeure").Replace("Notify", "Mineure").Replace("New", "Nouvelle").Replace("Accepted", "Reconnue").Replace("Closed", "Fermée").Replace("DTC codes", "Codes de diagnostic d'anomalie");


            StringReader strrXML = new StringReader(xml);

            DataSet ds = new DataSet();

            //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
            //string strPath = MapPath(DataSetPath) + @"\Alarms.xsd";
            //ds.ReadXmlSchema(strPath);


            ds.ReadXml(strrXML);

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

            DataColumn AlarmDate = new DataColumn("AlarmDate", Type.GetType("System.Object"));

            ds.Tables[0].Columns.Add(AlarmDate);
            string strStreetAddress = "";
            clsMap mp = new clsMap();

            foreach (DataRow rowItem in ds.Tables[0].Rows)
            {
                rowItem["AlarmDate"] = Convert.ToDateTime(rowItem["TimeCreated"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                //if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
                //{
                //    rowItem["AlarmDate"] = Convert.ToDateTime(rowItem["TimeCreated"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                //}
                //else
                //{
                //    rowItem["AlarmDate"] = Convert.ToDateTime(rowItem["TimeCreated"].ToString(), ci);
                //}

                strStreetAddress = rowItem["StreetAddress"].ToString().TrimEnd();
                if ((strStreetAddress == VLF.CLS.Def.Const.addressNA))
                {
                    try
                    {
                        rowItem["StreetAddress"] = mp.ResolveStreetAddressTelogis(rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString());
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }
                }


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


                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                {
                    rowItem["AlarmDescription"] = LocalizationLayer.GUILocalizationLayer.LocalizeAlarms(rowItem["AlarmDescription"].ToString(), System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    // rowItem["AlarmLevel"] = LocalizationLayer.GUILocalizationLayer.LocalizeSeverity(rowItem["AlarmLevel"].ToString(), System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    // rowItem["AlarmState"] = LocalizationLayer.GUILocalizationLayer.LocalizeAlarmState(rowItem["AlarmState"].ToString(), System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                }
            }



            // Show Combobox
            DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
            dc.DefaultValue = false;
            ds.Tables[0].Columns.Add(dc);

            //this.dgAlarms.LayoutSettings.ClientVisible = true;
            sn.Message.DsHistoryAlarms = ds;
            //dgAlarms.ClearCachedDataSource();
            //dgAlarms.RebindDataSource();            
            
            string sdata = "";
            foreach (DataRow rowItem in sn.Message.DsHistoryAlarms.Tables[0].Rows)
            {
                sdata += (sdata == "" ? "" : ",") + "[\"<span>" + Convert.ToDateTime(rowItem["TimeCreated"].ToString()).ToString("yyyyMMddHHmmss") + "</span>" + Convert.ToDateTime(rowItem["TimeCreated"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat) + "\"" +
                            ", \"" + clsUtility.EscapeStringValue(rowItem["StreetAddress"].ToString()).Replace("\r", " ").Replace("\n", " ") + "\"" +
                            ", \"" + clsUtility.EscapeStringValue(rowItem["vehicleDescription"].ToString()) + "\"" +
                            ", \"" + clsUtility.EscapeStringValue(rowItem["AlarmDescription"].ToString().Replace("\r", " ").Replace("\n", " ")) + "\"" +
                            ", \"" + rowItem["AlarmLevel"].ToString() + "\"" +
                            ", \"" + rowItem["AlarmState"].ToString() + "\"" +
                            ", \"" + rowItem["UserName"].ToString() + "\"" +
                            ", \"" + clsUtility.EscapeStringValue(rowItem["Notes"].ToString()).Replace("\r", " ").Replace("\n", " ") + "\"" +
                            ", \"" + rowItem["AlarmId"].ToString() + "\"" +
                        "]";
            }

            ALARM_DATA = @"{
                              ""draw"": 1,
                              ""recordsTotal"": " + sn.Message.DsHistoryAlarms.Tables[0].Rows.Count.ToString() + @",
                              ""recordsFiltered"": " + sn.Message.DsHistoryAlarms.Tables[0].Rows.Count.ToString() + @",
                              ""data"": [" + sdata;


            ALARM_DATA += "]}";

        }

        // Changes for TimeZone Feature end

        //replaced with dgAlarms_Fill_NewTZ
 //       private void dgAlarms_Fill()
 //       {
 //           string strFromDate = "";
 //           string strToDate = "";
 //           CultureInfo ci = new CultureInfo("en-US");
 //           //strFromDate = txtFrom.SelectedDate.Value.ToString(sn.User.DateFormat) + " " +cboHoursFrom.SelectedDate.Value.ToString(sn.User.TimeFormat);
 //           //strToDate = this.txtTo.SelectedDate.Value.ToString(sn.User.DateFormat) + " " + this.cboHoursTo.SelectedDate.Value.ToString(sn.User.TimeFormat);
 //           strFromDate = txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + string.Format("{0:t}", cboHoursFrom.SelectedDate.Value);
 //           strToDate = this.txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + string.Format("{0:t}", this.cboHoursTo.SelectedDate.Value);

 //           //strFromDate = Convert.ToDateTime(strFromDate).ToString();
 //           //strToDate = Convert.ToDateTime(strToDate).ToString();

 //           this.dgAlarms.LayoutSettings.ClientVisible = true;

 //           string xml = "";
 //           ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
 //           Int16 timeZone = Convert.ToInt16(sn.User.TimeZone + sn.User.DayLightSaving);

 //           if (objUtil.ErrCheck(alarms.GetAlarmsXML(sn.UserID, sn.SecId, Convert.ToDateTime(strFromDate), Convert.ToDateTime(strToDate), ref xml), false))
 //               if (objUtil.ErrCheck(alarms.GetAlarmsXML(sn.UserID, sn.SecId, Convert.ToDateTime(strFromDate), Convert.ToDateTime(strToDate), ref xml), true))
 //               {
 //                   sn.Message.DsHistoryAlarms = null;
 //                   dgAlarms.ClearCachedDataSource();
 //                   dgAlarms.RebindDataSource();
 //                   return;
 //               }

 //           if (xml == "")
 //           {
 //               sn.Message.DsHistoryAlarms = null;
 //               dgAlarms.ClearCachedDataSource();
 //               dgAlarms.RebindDataSource();
 //               return;
 //           }
            

 //if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
 //               xml = xml.Replace("Critical", "Critique").Replace("Warning", "Majeure").Replace("Notify", "Mineure").Replace("New", "Nouvelle").Replace("Accepted", "Reconnue").Replace("Closed", "Fermée").Replace("DTC codes", "Codes de diagnostic d'anomalie"); 


 //           StringReader strrXML = new StringReader(xml);

 //           DataSet ds = new DataSet();

 //           //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
 //           //string strPath = MapPath(DataSetPath) + @"\Alarms.xsd";
 //           //ds.ReadXmlSchema(strPath);


 //           ds.ReadXml(strrXML);

 //           #region Resolve street address in Batch
 //           //try
 //           //{
 //           //    string[] addresses = null;
 //           //    DataRow[] drArrAddress = ds.Tables[0].Select("StreetAddress='" + VLF.CLS.Def.Const.addressNA + "'");

 //           //    if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicroWeb")) && drArrAddress.Length > 0)
 //           //        addresses = clsMap.ResolveStreetAddressGeoMicroWebBatch(drArrAddress);
 //           //    else if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicro")) && drArrAddress.Length > 0)
 //           //        addresses = clsMap.ResolveStreetAddressGeoMicroBatch(drArrAddress);

 //           //    int i = 0;
 //           //    foreach (DataRow dr in drArrAddress)
 //           //    {
 //           //        dr["StreetAddress"] = addresses[i];
 //           //        i++;
 //           //    }

 //           //}
 //           //catch (Exception Ex)
 //           //{
 //           //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:clsMap"));
 //           //}
 //           #endregion

 //           DataColumn AlarmDate = new DataColumn("AlarmDate", Type.GetType("System.Object"));

 //           ds.Tables[0].Columns.Add(AlarmDate);
 //           string strStreetAddress = "";
 //           clsMap mp = new clsMap();
            
 //           foreach (DataRow rowItem in ds.Tables[0].Rows)
 //           {
 //               rowItem["AlarmDate"] = Convert.ToDateTime(rowItem["TimeCreated"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
 //               //if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
 //               //{
 //               //    rowItem["AlarmDate"] = Convert.ToDateTime(rowItem["TimeCreated"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
 //               //}
 //               //else
 //               //{
 //               //    rowItem["AlarmDate"] = Convert.ToDateTime(rowItem["TimeCreated"].ToString(), ci);
 //               //}

 //               strStreetAddress = rowItem["StreetAddress"].ToString().TrimEnd();
 //               if ((strStreetAddress == VLF.CLS.Def.Const.addressNA))
 //               {
 //                   try
 //                   {
 //                       rowItem["StreetAddress"] = mp.ResolveStreetAddressTelogis(rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString());
 //                   }
 //                   catch (Exception Ex)
 //                   {
 //                       System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
 //                   }
 //               }


 //               switch (strStreetAddress)
 //               {
 //                   case VLF.CLS.Def.Const.addressNA:
 //                       rowItem["StreetAddress"] = Resources.Const.InvalidAddress_addressNA;
 //                       break;

 //                   case VLF.CLS.Def.Const.noGPSData:
 //                       rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noGPSData;
 //                       break;

 //                   //case VLF.CLS.Def.Const.noValidAddress:
 //                   //    rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noValidAddress;
 //                   //    break;

 //                   default:
 //                       break;
 //               }


 //               if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
 //               {
 //                   rowItem["AlarmDescription"] = LocalizationLayer.GUILocalizationLayer.LocalizeAlarms(rowItem["AlarmDescription"].ToString(), System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
 //                  // rowItem["AlarmLevel"] = LocalizationLayer.GUILocalizationLayer.LocalizeSeverity(rowItem["AlarmLevel"].ToString(), System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
 //                  // rowItem["AlarmState"] = LocalizationLayer.GUILocalizationLayer.LocalizeAlarmState(rowItem["AlarmState"].ToString(), System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
 //               }
 //           }



 //           // Show Combobox
 //           DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
 //           dc.DefaultValue = false;
 //           ds.Tables[0].Columns.Add(dc);

 //           this.dgAlarms.LayoutSettings.ClientVisible = true;
 //           sn.Message.DsHistoryAlarms = ds;
 //           dgAlarms.ClearCachedDataSource();
 //           dgAlarms.RebindDataSource();

 //       }

        protected void cmdAccept_Click(object sender, EventArgs e)
        {
            try
            {

                DataSet ds = new DataSet();
                ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();

                
                //foreach (string keyValue in dgAlarms.RootTable.GetCheckedRows())
                foreach (string keyValue in SelectedAlarmIds.Value.Split('|'))
                {
                    if (objUtil.ErrCheck(alarms.AcceptCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), false))
                        if (objUtil.ErrCheck(alarms.AcceptCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Alarm Accept failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        }

                }


                dgAlarms_Fill_NewTZ();
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
        protected void cmdCloseAlarms_Click(object sender, EventArgs e)
        {
            try
            {

                DataSet ds = new DataSet();
                ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();

                
                //foreach (string keyValue in dgAlarms.RootTable.GetCheckedRows())
                foreach (string keyValue in SelectedAlarmIds.Value.Split('|'))
                {
                    if (objUtil.ErrCheck(alarms.CloseCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), false))
                        if (objUtil.ErrCheck(alarms.CloseCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Alarm Accept failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        }

                }


                dgAlarms_Fill_NewTZ();
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

        //protected void dgAlarms_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        //{
        //    if (sn.Message.DsHistoryAlarms != null) 
        //        e.DataSource = sn.Message.DsHistoryAlarms;
        //    else
        //        e.DataSource = null;
        //}

        //protected void dgAlarms_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
        //{
        //    try
        //    {
        //        if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
        //        {
        //            e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
        //            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
        //                e.Layout.TextSettings.UseLanguage = "fr-FR";
        //            //else
        //            //   e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

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
        protected void txtFrom_Load(object sender, EventArgs e)
        {
            txtFrom.DateInput.DateFormat = sn.User.DateFormat;
            txtTo.DateInput.DateFormat = sn.User.DateFormat;
        }

        private void exportAlarm(string exportformat)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null)
                sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else
                return;
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName))
                return;

            if (sn.Message.DsHistoryAlarms == null)
            {
                Response.Write("<script type='text/javascript'>alert('Sorry, it has no data to export.');</script>");
                return;
            }

            string alarmIdsForExport = Request["alarmIdsForExport"];

            DataTable dt = sn.Message.DsHistoryAlarms.Tables[0].Clone();

            DataRow[] drArray = sn.Message.DsHistoryAlarms.Tables[0].Copy().Select("AlarmId IN (" + alarmIdsForExport + ")");
            if (drArray.Length == 0)
            {
                Response.Write("<script type='text/javascript'>alert('Sorry, it has no data to export.');</script>");
                return;
            }
            foreach(DataRow dr in drArray)
            {
                dt.ImportRow(dr);
            }

            dt.DefaultView.Sort = "TimeCreated DESC";
            dt = dt.DefaultView.ToTable();

            string columns = (string)base.GetLocalResourceObject("dgAlarms_Date") + ":TimeCreated" +
                            "," + (string)base.GetLocalResourceObject("dgAlarms_Address") + ":StreetAddress" +
                            "," + (string)base.GetLocalResourceObject("dgAlarms_vehicleDescription") + ":vehicleDescription" +
                            "," + (string)base.GetLocalResourceObject("dgAlarms_AlarmDescription") + ":AlarmDescription" +
                            "," + (string)base.GetLocalResourceObject("dgAlarms_AlarmLevel") + ":AlarmLevel" +
                            "," + (string)base.GetLocalResourceObject("dgAlarms_AlarmState") + ":AlarmState" +
                            "," + (string)base.GetLocalResourceObject("dgAlarms_UserName") + ":UserName" +
                            "," + (string)base.GetLocalResourceObject("dgAlarms_Notes") + ":Notes";

            //exportDatatable(sn.Message.DsHistoryAlarms.Tables[0], exportformat, columns, "Alarms", "alarms");
            exportDatatable(dt, exportformat, columns, "Alarms", "alarms");
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
                    filefullpath = clsUtility.CreateCSV(filepath, filename, fileextension, dt, columns, "TimeCreated", sn.User.DateFormat + " " + sn.User.TimeFormat, true, title);
                    filecontenttype = "application/Excel";
                }
                else if (formatter == "excel2003")
                {
                    fileextension = "xls";

                    filefullpath = clsUtility.CreateExcel2003(filepath, filename, fileextension, dt, columns, "TimeCreated", sn.User.DateFormat + " " + sn.User.TimeFormat, true, title);
                    filecontenttype = "application/Excel";
                }
                else if (formatter == "excel2007")
                {
                    fileextension = "xlsx";

                    filefullpath = clsUtility.CreateExcel2007(filepath, filename, fileextension, dt, columns, "TimeCreated", sn.User.DateFormat + " " + sn.User.TimeFormat, true, title);
                    filecontenttype = "application/Excel";
                }

                else if (formatter == "pdf")
                {
                    fileextension = "PDF";

                    filefullpath = clsUtility.CreatePDFFile(filepath, filename, fileextension, dt, columns, "TimeCreated", sn.User.DateFormat + " " + sn.User.TimeFormat, true, title);
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
    }
}

      
