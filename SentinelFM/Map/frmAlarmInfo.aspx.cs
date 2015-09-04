using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Configuration;
using VLF.MAP;
using VLF.CLS.Interfaces;


namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmAlarmInfo.
    /// </summary>
    public partial class frmAlarmInfo : SentinelFMBasePage
    {
        public ClientMapProxy map;
        
        
        public string redirectURL;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                
                
                if (!Page.IsPostBack)
                {
                    GuiSecurity(this);
                    int AlarmId = Convert.ToInt32(Request.QueryString["AlarmId"]);
                    ViewState["AlarmId"] = AlarmId;
                    AlarmInfoLoad_NewTZ(AlarmId);
                    ViewState["ConfirmCloseAll"] = "0";
                }
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }


        }

        // Changes for TimeZone Feature start
        private void AlarmInfoLoad_NewTZ(int AlarmId)
        {

            try
            {

                DataSet ds = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();

                if (objUtil.ErrCheck(alarms.GetCurrentAlarmInfoXML_NewTZ(sn.UserID, sn.SecId, AlarmId, ref xml), false))
                    if (objUtil.ErrCheck(alarms.GetCurrentAlarmInfoXML_NewTZ(sn.UserID, sn.SecId, AlarmId, ref xml), true))
                    {
                        return;
                    }
                strrXML = new StringReader(xml);


                if (xml == "")
                {
                    return;
                }

                ds.ReadXml(strrXML);


                string AlarmDescription = ds.Tables[0].Rows[0]["AlarmDescription"].ToString().TrimEnd();
                if (AlarmDescription.Contains("DTC codes:")) //DTC Code
                    AlarmDescription = AlarmDescription.TrimEnd().Replace("\n", " ").Replace(",", "\r\n").Replace(";", " ");


                this.lblAlarmDesc.Text = ds.Tables[0].Rows[0]["AlarmDescription"].ToString().TrimEnd();
                this.lblAlarmState.Text = ds.Tables[0].Rows[0]["AlarmState"].ToString().TrimEnd();


                if (ds.Tables[0].Rows[0]["AlarmState"].ToString().TrimEnd() == "New")
                {
                    this.cmdAccept.Enabled = true;
                    this.cmdCloseAlarm.Enabled = false;
                }

                if (ds.Tables[0].Rows[0]["AlarmState"].ToString().TrimEnd() == "Accepted")
                {
                    this.cmdAccept.Enabled = false;
                    this.cmdCloseAlarm.Enabled = true;
                }

                if (ds.Tables[0].Rows[0]["AlarmState"].ToString().TrimEnd() == "Closed")
                {
                    this.cmdCloseAlarm.Enabled = false;
                    this.cmdAccept.Enabled = false;
                }

                if (ds.Tables[0].Rows[0]["AlarmLevel"].ToString().TrimEnd() == "Critical")
                {
                    this.lblAlarmDesc.ForeColor = Color.Red;
                    this.lblAlarmLevel.ForeColor = Color.Red;
                    this.lblAlarmState.ForeColor = Color.Red;
                    this.lblStreetAddress.ForeColor = Color.Red;
                    this.lblTimeAccepted.ForeColor = Color.Red;
                    this.lblTimeCreated.ForeColor = Color.Red;
                    this.lblVehicleDesc.ForeColor = Color.Red;
                    this.lblOperatorName.ForeColor = Color.Red;
                }

                this.lblAlarmLevel.Text = ds.Tables[0].Rows[0]["AlarmLevel"].ToString().TrimEnd();
                this.lblStreetAddress.Text = ds.Tables[0].Rows[0]["StreetAddress"].ToString().TrimEnd();

                if ((ds.Tables[0].Rows[0]["TimeAccepted"].ToString().TrimEnd() != "1/1/0001 12:00:00 AM") && (ds.Tables[0].Rows[0]["TimeAccepted"].ToString().TrimEnd() != "01/01/1900 12:00:00 AM"))
                    this.lblTimeAccepted.Text = ds.Tables[0].Rows[0]["TimeAccepted"].ToString().TrimEnd();
                else
                    this.lblTimeAccepted.Text = "";

                this.lblTimeCreated.Text = ds.Tables[0].Rows[0]["TimeCreated"].ToString().TrimEnd();
                this.lblVehicleDesc.Text = ds.Tables[0].Rows[0]["vehicleDescription"].ToString().TrimEnd();
                this.lblVehicleId.Text = ds.Tables[0].Rows[0]["VehicleId"].ToString().TrimEnd();
                this.lblOperatorName.Text = ds.Tables[0].Rows[0]["UserName"].ToString().TrimEnd();
                this.txtNotes.Text = VLF.CLS.Util.GetDTCCodes(ds.Tables[0].Rows[0]["CustomProp"].ToString());
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }
        // Changes for TimeZone Feature end

        private void AlarmInfoLoad(int AlarmId)
        {

            try
            {
                
                DataSet ds = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();

                if (objUtil.ErrCheck(alarms.GetCurrentAlarmInfoXML(sn.UserID, sn.SecId, AlarmId, ref xml), false))
                    if (objUtil.ErrCheck(alarms.GetCurrentAlarmInfoXML(sn.UserID, sn.SecId, AlarmId, ref xml), true))
                    {
                        return;
                    }
                strrXML = new StringReader(xml);


                if (xml == "")
                {
                    return;
                }

                ds.ReadXml(strrXML);


                string AlarmDescription = ds.Tables[0].Rows[0]["AlarmDescription"].ToString().TrimEnd();
                if (AlarmDescription.Contains("DTC codes:")) //DTC Code
                    AlarmDescription = AlarmDescription.TrimEnd().Replace("\n", " ").Replace(",", "\r\n").Replace(";", " ");


                this.lblAlarmDesc.Text = ds.Tables[0].Rows[0]["AlarmDescription"].ToString().TrimEnd();
                this.lblAlarmState.Text = ds.Tables[0].Rows[0]["AlarmState"].ToString().TrimEnd();


                if (ds.Tables[0].Rows[0]["AlarmState"].ToString().TrimEnd() == "New")
                {
                    this.cmdAccept.Enabled = true;
                    this.cmdCloseAlarm.Enabled = false;
                }

                if (ds.Tables[0].Rows[0]["AlarmState"].ToString().TrimEnd() == "Accepted")
                {
                    this.cmdAccept.Enabled = false;
                    this.cmdCloseAlarm.Enabled = true;
                }

                if (ds.Tables[0].Rows[0]["AlarmState"].ToString().TrimEnd() == "Closed")
                {
                    this.cmdCloseAlarm.Enabled = false;
                    this.cmdAccept.Enabled = false;
                }

                if (ds.Tables[0].Rows[0]["AlarmLevel"].ToString().TrimEnd() == "Critical")
                {
                    this.lblAlarmDesc.ForeColor = Color.Red;
                    this.lblAlarmLevel.ForeColor = Color.Red;
                    this.lblAlarmState.ForeColor = Color.Red;
                    this.lblStreetAddress.ForeColor = Color.Red;
                    this.lblTimeAccepted.ForeColor = Color.Red;
                    this.lblTimeCreated.ForeColor = Color.Red;
                    this.lblVehicleDesc.ForeColor = Color.Red;
                    this.lblOperatorName.ForeColor = Color.Red;
                }

                this.lblAlarmLevel.Text = ds.Tables[0].Rows[0]["AlarmLevel"].ToString().TrimEnd();
                this.lblStreetAddress.Text = ds.Tables[0].Rows[0]["StreetAddress"].ToString().TrimEnd();

                if ((ds.Tables[0].Rows[0]["TimeAccepted"].ToString().TrimEnd() != "1/1/0001 12:00:00 AM") && (ds.Tables[0].Rows[0]["TimeAccepted"].ToString().TrimEnd() != "01/01/1900 12:00:00 AM"))
                    this.lblTimeAccepted.Text = ds.Tables[0].Rows[0]["TimeAccepted"].ToString().TrimEnd();
                else
                    this.lblTimeAccepted.Text = "";

                this.lblTimeCreated.Text = ds.Tables[0].Rows[0]["TimeCreated"].ToString().TrimEnd();
                this.lblVehicleDesc.Text = ds.Tables[0].Rows[0]["vehicleDescription"].ToString().TrimEnd();
                this.lblVehicleId.Text = ds.Tables[0].Rows[0]["VehicleId"].ToString().TrimEnd();
                this.lblOperatorName.Text = ds.Tables[0].Rows[0]["UserName"].ToString().TrimEnd();
                this.txtNotes.Text= VLF.CLS.Util.GetDTCCodes(ds.Tables[0].Rows[0]["CustomProp"].ToString() );  
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

        }
        #endregion

        protected void cmdAccept_Click(object sender, System.EventArgs e)
        {
            try
            {
                
                DataSet ds = new DataSet();
                ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();

                if (objUtil.ErrCheck(alarms.AcceptCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(ViewState["AlarmId"])), false))
                    if (objUtil.ErrCheck(alarms.AcceptCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(ViewState["AlarmId"])), true))
                    {
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AcceptAlarmFailed");
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Alarm Accept failed. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                        return;
                    }

                //AlarmInfoLoad(Convert.ToInt32(ViewState["AlarmId"]));

                MapIt();
                System.Threading.Thread.Sleep(2000);
                sn.Map.ReloadMap = true;


                string str = "";
                int AlarmCount = 0;

                sn.Map.LoadAlarms_NewTZ(sn, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref str, ref AlarmCount);
                sn.Map.AlarmsHTML = str;
                sn.Map.AlarmCount = AlarmCount; 

                Response.Write("<script language='javascript'>window.close()</script>");
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }

        protected void cmdCloseAlarm_Click(object sender, System.EventArgs e)
        {
            try
            {
                
                DataSet ds = new DataSet();
                ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();

                if (objUtil.ErrCheck(alarms.CloseCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(ViewState["AlarmId"])), false))
                    if (objUtil.ErrCheck(alarms.CloseCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(ViewState["AlarmId"])), true))
                    {
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_CloseAlarmFailed");
                        return;
                    }

                //AlarmInfoLoad(Convert.ToInt32(ViewState["AlarmId"]));


                 string str = "";
                 int AlarmCount = 0;

                 sn.Map.LoadAlarms_NewTZ(sn, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref str, ref AlarmCount);
                 sn.Map.AlarmsHTML = str;
                 sn.Map.AlarmCount = AlarmCount; 

                Response.Write("<script language='javascript'>window.close()</script>");
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }


        private void MapIt()
        {
            try
            {
                
                DataSet ds = new DataSet();
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
                StringReader strrXML = null;
                string xml = "";

                if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "GetFleetsInfoXMLByVehicleId. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                        return;
                    }


                strrXML = new StringReader(xml);


                if (xml == "")
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "GetFleetsInfoXMLByVehicleId - Empty. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                    return;
                }

                ds.ReadXml(strrXML);


                if (ds.Tables.Count == 0)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Fleet Table  - Empty. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                    return;
                }

                // Set FleetId
                sn.Map.SelectedFleetID = Convert.ToInt32(ds.Tables[0].Rows[0]["FleetId"]);
                DsFleetInfo_Fill(sn.Map.SelectedFleetID);
                // Set checkbox for vehicle
                foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (this.lblVehicleId.Text == rowItem["VehicleId"].ToString())
                    {
                        rowItem["chkBoxShow"] = true;
                        break;
                    }
                    else
                        rowItem["chkBoxShow"] = false;

                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }


        private void DsFleetInfo_Fill(int fleetId)
        {
            try
            {

                DataSet dsFleetInfo = new DataSet();

                
                StringReader strrXML = null;


                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    sn.Map.DsFleetInfo = null;
                    return;
                }

                strrXML = new StringReader(xml);
                dsFleetInfo.ReadXml(strrXML);
                sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                dsFleetInfo.Tables[0].DefaultView.Sort = "chkBoxShow" + " DESC";
                sn.Map.DsFleetInfo = dsFleetInfo;

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }



        protected void cmdAcceptAll_Click(object sender, System.EventArgs e)
        {
            
            StringReader strrXML = null;


            string xml = "";
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
            // Changes for TimeZone Feature start
            float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);

            if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true)) // Changes for TimeZone Feature end
                {
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AcceptAllAlarmsFailed");
                    return;
                }

            if (xml == "")
            {
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AcceptAllAlarmsFailed");
                return;
            }

            strrXML = new StringReader(xml);

            DataSet ds = new DataSet();
            ds.ReadXml(strrXML);

            foreach (DataRow rowItem in ds.Tables[0].Rows)
            {
                if (this.lblVehicleDesc.Text.TrimEnd() == rowItem["vehicledescription"].ToString().TrimEnd()
                    && (ds.Tables[0].Rows[0]["AlarmState"].ToString().TrimEnd() == "New"))
                {
                    if (objUtil.ErrCheck(alarms.AcceptCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["AlarmId"])), false))
                        if (objUtil.ErrCheck(alarms.AcceptCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["AlarmId"])), true))
                        {
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AcceptAllAlarmsFailed");
                            return;
                        }
                }
            }


            string str = "";
            int AlarmCount = 0;

            sn.Map.LoadAlarms_NewTZ(sn, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref str, ref AlarmCount);
            sn.Map.AlarmsHTML = str;
            sn.Map.AlarmCount = AlarmCount; 

            Response.Write("<script language='javascript'>window.close()</script>");
        }

        protected void cmdCloseAll_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (ViewState["ConfirmCloseAll"].ToString() == "0")
                {
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_CloseAllAlarmsWarning1") + " " + this.lblVehicleDesc.Text + ". " + (string)base.GetLocalResourceObject("lblMessage_Text_CloseAllAlarmsWarning2");
                    ViewState["ConfirmCloseAll"] = "1";
                    return;
                }

                
                StringReader strrXML = null;


                string xml = "";
                ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
                // Changes for TimeZone Feature start
                float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);

                if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))// Changes for TimeZone Feature end
                    {
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_CloseAllAlarmsFailed");
                        return;
                    }

                if (xml == "")
                {
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_CloseAllAlarmsFailed");
                    return;
                }

                strrXML = new StringReader(xml);

                DataSet ds = new DataSet();
                ds.ReadXml(strrXML);
                string VehicleDesc = lblVehicleDesc.Text.TrimEnd();

                foreach (DataRow rowItem in ds.Tables[0].Rows)
                {
                    if ((VehicleDesc == rowItem["vehicledescription"].ToString().TrimEnd()) && (rowItem["AlarmState"].ToString().TrimEnd() == VLF.CLS.Def.Enums.AlarmState.New.ToString()))
                    {

                        if (objUtil.ErrCheck(alarms.AcceptCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["AlarmId"])), false))
                            if (objUtil.ErrCheck(alarms.AcceptCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["AlarmId"])), true))
                            {
                                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_CloseAllAlarmsFailed");
                                return;
                            }

                        if (objUtil.ErrCheck(alarms.CloseCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["AlarmId"])), false))
                            if (objUtil.ErrCheck(alarms.CloseCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["AlarmId"])), true))
                            {
                                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_CloseAllAlarmsFailed");
                                return;
                            }
                    }


                    if ((VehicleDesc == rowItem["vehicledescription"].ToString().TrimEnd())
                        && (rowItem["AlarmState"].ToString().TrimEnd() == VLF.CLS.Def.Enums.AlarmState.Accepted.ToString()))
                    {
                        if (objUtil.ErrCheck(alarms.CloseCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["AlarmId"])), false))
                            if (objUtil.ErrCheck(alarms.CloseCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["AlarmId"])), true))
                            {
                                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_CloseAllAlarmsFailed");
                                return;
                            }
                    }
                }


                ViewState["ConfirmCloseAll"] = "0";
                this.lblMessage.Text = "";

                string str = "";
                int AlarmCount = 0;

                sn.Map.LoadAlarms_NewTZ(sn,CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref str, ref AlarmCount);
                sn.Map.AlarmsHTML = str;
                sn.Map.AlarmCount = AlarmCount; 
               
                Response.Write("<script language='javascript'>window.close()</script>");
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }

        protected void cmdMapIt_Click(object sender, System.EventArgs e)
        {
            try
            {
                MapIt();
                System.Threading.Thread.Sleep(2000);
                sn.Map.ReloadMap = true;
                Response.Write("<script language='javascript'>window.close()</script>");
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }

        private void GuiSecurity(System.Web.UI.Control obj)
        {

            foreach (System.Web.UI.Control ctl in obj.Controls)
            {
                try
                {
                    if (ctl.HasControls())
                        GuiSecurity(ctl);

                    System.Web.UI.WebControls.Button CmdButton = (System.Web.UI.WebControls.Button)ctl;
                    bool CmdStatus = false;
                    if (CmdButton.CommandName != "")
                    {
                        CmdStatus = sn.User.ControlEnable(sn, Convert.ToInt32(CmdButton.CommandName));
                        CmdButton.Enabled = CmdStatus;
                    }

                }
                catch
                {
                }
            }
        }

      
}
}
