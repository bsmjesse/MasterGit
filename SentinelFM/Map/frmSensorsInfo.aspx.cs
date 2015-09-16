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
using VLF.DAS.Logic;
using VLF.MAP;
using VLF.CLS;
using VLF.CLS.Def;
using VLF.CLS.Interfaces;
using System.Configuration;
using System.Globalization;
using System.Collections.Generic;


namespace SentinelFM
{

    public partial class frmSensorsInfo : SensorInfoPage
    {
        public string redirectURL;
        protected System.Web.UI.HtmlControls.HtmlTable Table2;
        protected System.Web.UI.WebControls.Label lblBoxSetupImageFreg;
        protected System.Web.UI.WebControls.Label lblBoxSetupCommMode;
        protected DataSet dsTrace = new DataSet();

        private DataSet dsFleetInfo;
        private int boxModel;
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        private Dictionary<string, string> wifiCmdDictionary = new Dictionary<string, string>();
      


        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                cboTARPeriod.Enabled = true;
                this.boxModel = BoxProfileHelper.GetBoxMainBoardModelByBoxId(sn.Cmd.BoxId);
             



                if ((sn == null) || (sn.UserName == ""))
                {
                    Response.Write("<SCRIPT Language='javascript'>window.open('../Login.aspx','_top') </SCRIPT>");
                    return;
                }

                if (!Page.IsPostBack)
                {
                    lblUnit.Text = sn.User.UnitOfMes == 1 ? "km" : "mi";
                    this.lblGeoZoneSpeedUnit.Text = sn.User.UnitOfMes == 1 ? "km/h" : "mph";
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmSensorsInfoForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    //GuiSecurity(this);

                    this.cmdSettings.Visible = !sn.User.ControlEnable(sn, 49) ? false : true;
                    this.cmdServices.Visible = !sn.User.ControlEnable(sn, 52) ? false : true;
                    this.cmdUnitInfo.Visible = !sn.User.ControlEnable(sn, 53) ? false : true;


                    this.dgSensors.Visible = true;
                    HideAllTables();
                    this.LicensePlate = Request.QueryString["LicensePlate"];

                    if (!String.IsNullOrEmpty(this.LicensePlate))
                    {
                        sn.Cmd.Status = CommandStatus.Idle;
                        sn.Cmd.SelectedVehicleLicensePlate = this.LicensePlate;
                        this.lblCommandStatus.Text = "";
                    }
                    else
                        this.LicensePlate = sn.Cmd.SelectedVehicleLicensePlate;

                    GetVehicleInfo(LicensePlate);
                    DgSensors_Fill(LicensePlate);
                    sn.Cmd.DualComm = false;
                    CheckCommMode();

                    CboCommand_Fill(LicensePlate);
                    CboOutput_Fill(LicensePlate);

                    if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.UpdatePosition))
                    {
                        DsFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));
                        CheckStatusValidation();
                    }

                    if (sn.Cmd.SchCommand && sn.Cmd.SendCommandMessage != "")
                    {
                        this.lblCommandStatus.ForeColor = Color.Green;
                        this.lblCommandStatus.Text = sn.Cmd.SendCommandMessage;
                        sn.Cmd.SendCommandMessage = "";
                        sn.Cmd.SchCommand = false;
                        return;
                    }
                    else if (sn.Cmd.SendCommandMessage != "")
                    {
                        this.lblCommandStatus.ForeColor = Color.Red;
                        this.lblCommandStatus.Text = sn.Cmd.SendCommandMessage;
                        sn.Cmd.SendCommandMessage = "";
                        return;
                    }

                    switch (sn.Cmd.Status)
                    {
                        case CommandStatus.Idle:
                            this.lblCommandStatus.Text = "";
                            break;
                        case CommandStatus.Ack:
                        case CommandStatus.Pending:
                            this.lblCommandStatus.ForeColor = Color.Green;
                            if (sn.Cmd.CommModeId == 0)
                                this.lblCommandStatus.Text = (string)base.GetLocalResourceObject("lblCommandStatus_Text_CommandSuccessful");
                            else
                            {

                                this.lblCommandStatus.Text = (string)base.GetLocalResourceObject("lblCommandStatus_Text_CommandSentSuccessful") + " " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId;
                                sn.Cmd.CommModeId = 0;
                            }

                            if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetBoxStatus) || sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetExtendedStatus))
                            {
                                this.tblSensors.Visible = false;
                                bool isExtendedVersion = sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetExtendedStatus);
                                GetBoxStatus(isExtendedVersion);
                                this.tblBoxStatusInfo.Visible = true;
                                sn.Cmd.CommandId = 0;
                            }
                            else if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetBoxSetup) || sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetExtendedSetup))
                            {
                                this.tblSensors.Visible = false;
                                bool isExtendedVersion = sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetExtendedSetup);
                                GetBoxSetup(isExtendedVersion);
                                this.tblBoxSetupInfo.Visible = true;
                                sn.Cmd.CommandId = 0;
                            }
                            else if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetGeoZoneIDs))
                            {
                                this.tblSensors.Visible = false;
                                GetBoxGeoZoneIds();
                                this.tblBoxGeozones.Visible = true;
                                sn.Cmd.CommandId = 0;
                            }
                            else if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetVCROffDelay))
                            {
                                this.tblSensors.Visible = true;
                                GetVCRDelay();
                                this.tblVCRDelay.Visible = true;
                                sn.Cmd.CommandId = 0;
                            }
                            else if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetKeyFobStatus))
                            {
                                this.tblSensors.Visible = true;
                                GetKeyFobStatus();
                                sn.Cmd.CommandId = 0;
                            }
                            else if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetTARMode))
                            {
                                this.tblSensors.Visible = true;
                                this.cboTARPeriod.Visible = false;
                                GetTarMode();
                                this.tblTAR.Visible = true;
                                sn.Cmd.CommandId = 0;
                            }
                            else if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetControllerStatus))
                            {
                                this.tblSensors.Visible = true;
                                GetControllerStatus();
                                sn.Cmd.CommandId = 0;
                            }

                            else if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetOdometer))
                            {
                                this.tblSensors.Visible = true;
                                GetOdometer();
                                sn.Cmd.CommandId = 0;
                            }

                            else if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetPowerOffDelay))
                            {
                                this.tblSensors.Visible = true;
                                GetPowerOffDelay();
                                sn.Cmd.CommandId = 0;
                            }

                            else if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.ReadEEPROMData))
                            {
                                this.tblSensors.Visible = true;
                                GetEEPROMData();
                                sn.Cmd.CommandId = 0;
                            }
                            else if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetIridiumFilter))
                            {
                                this.tblSensors.Visible = true;
                                GetIridiumFilterSetup();
                                sn.Cmd.CommandId = 0;
                            }
                            else if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetPowerSaveConfig))
                            {
                                this.tblSensors.Visible = true;
                                GetPowerSaveConfig();
                                sn.Cmd.CommandId = 0;
                            }
                            break;





                        case CommandStatus.CommTimeout:
                            this.tblSensors.Visible = true;
                            this.lblCommandStatus.ForeColor = Color.Red;
                            this.lblCommandStatus.Text = (string)base.GetLocalResourceObject("lblCommandStatus_Text_CommandTimeout");
                            break;
                    }
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void DgSensors_Fill(string LicensePlate)
        {
            try
            {
                string xml = "", SensorMask = "";
                DataSet dsSensors = new DataSet();

                DataTable dtResult = this.GetAllSensorsForVehicle(LicensePlate, true);
                //new DataSet("Sensors");
                /*
                StringReader strrXML = null;
            

                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                   if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                   {
                      return;
                   }

                if (xml == "")
                {
                   return;
                }

                strrXML = new StringReader(xml);
                dsResult.ReadXml(strrXML);
                */

                //LocationMgr.Location location = new LocationMgr.Location();


                //if (objUtil.ErrCheck(location.GetVehicleLocationInfoXmlByLicensePlate(sn.UserID, sn.SecId, LicensePlate, ref xml), false))
                //    if (objUtil.ErrCheck(location.GetVehicleLocationInfoXmlByLicensePlate(sn.UserID, sn.SecId, LicensePlate, ref xml), true))
                //    {
                //        return;
                //    }

                //if (xml == "")
                //    return;


                //DataSet dsVehicleInfo = new DataSet();

                //dsVehicleInfo.ReadXml(new StringReader(xml));
                //SensorMask = dsVehicleInfo.Tables[0].Rows[0]["SensorMask"].ToString();

                SensorMask = this.lblSensorMask.Text;

                //Test for wrong Sensor Mask
                try
                {
                    UInt64 intSensorMask = Convert.ToUInt64(SensorMask);
                }
                catch
                {
                    //               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "SensorMask: " + SensorMask + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                    //return;
                }


                if ((dtResult != null) && (dtResult.Rows.Count > 0))
                {


                    DataTable tblSensors = dsSensors.Tables.Add("SensorsInformation");

                    //dsSensors.Tables.Add("SensorsInformation");
                    tblSensors.Columns.Add("SensorId", typeof(short));
                    tblSensors.Columns.Add("SensorName", typeof(string));
                    tblSensors.Columns.Add("SensorAction", typeof(string));

                    // move over all sensors and set current status
                    short snsId = 0;
                    int slashIndex = 0;
                    object[] objRow = new object[3];
                    string snsAction = "", fldAction = "";
                    UInt64 checkBit = 1, shift = 1;
                    foreach (DataRow ittr in dtResult.Rows)
                    {
                        try
                        {
                            if (!ittr["SensorName"].ToString().TrimEnd().Contains(VLF.CLS.Def.Const.keySensorNotInUse))
                            {
                                snsId = Convert.ToInt16(ittr["SensorId"]);

                                // if not AVL sensor, ignore
                                if ((snsId & (short)Enums.ReeferBase) > 0)
                                    continue;

                                //objRow[0] = snsId;
                                //objRow[1] = ittr["SensorName"].ToString().TrimEnd();

                                checkBit = shift << (snsId - 1);

                                fldAction = ittr["SensorAction"].ToString().TrimEnd();
                                slashIndex = fldAction.IndexOf("/");
                                if (slashIndex < 1)
                                {
                                    // wrong sensors format in the database (should be action1/action2)
                                    //continue;
                                    snsAction = "Invalid";
                                }
                                else
                                {
                                    if ((Convert.ToUInt64(SensorMask) & checkBit) == 0)
                                        snsAction = fldAction.Substring(slashIndex + 1).ToString().TrimEnd();
                                    else
                                        snsAction = fldAction.Substring(0, slashIndex).ToString().TrimEnd();
                                }

                                tblSensors.Rows.Add(snsId, ittr["SensorName"].ToString().TrimEnd(), snsAction);
                            }
                        }
                        catch
                        {
                        }
                    }

                }

                dsSensors.Tables["SensorsInformation"].DefaultView.Sort = "SensorId";
                this.dgSensors.DataSource = dsSensors.Tables["SensorsInformation"];
                this.dgSensors.DataBind();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }


        }

        private void CboCommand_Fill(string LicensePlate)
        {
            try
            {

                DataSet ds = new DataSet();
                StringReader strrXML = null;
                cboCommand.Items.Clear();

                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                //DumpBeforeCall(sn, string.Format("CboCommand_Fill : LicensePlate = {0}", LicensePlate));
                if (objUtil.ErrCheck(dbv.GetVehicleCommandsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleCommandsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No command for vehicle: " + LicensePlate + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        cboCommand.Items.Clear();
                        cboCommand.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboCommand_Item_0"), "-1"));
                        return;
                    }

                if (xml == "")
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No command for vehicle: " + LicensePlate + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    cboCommand.Items.Clear();
                    cboCommand.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboCommand_Item_0"), "-1"));
                    return;
                }


                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);






                //Dual Communication
                //sn.Cmd.DualComm=Convert.ToBoolean(chkDualMode.Checked);  
                //if (!this.chkDualMode.Checked)
                //{


                ds.Tables[0].Select();
                ds.Tables[0].Select("BoxProtocolTypeId=" + sn.Cmd.DsProtocolTypes.Tables[0].Rows[0]["BoxProtocolTypeId"].ToString());
                if (sn.Cmd.DsProtocolTypes.Tables[0].Rows.Count > 1)
                {
                    DataSet dsCommands = new DataSet();
                    clsDataSetHelper objDataSetHelper = new clsDataSetHelper(ref dsCommands);
                    objDataSetHelper.SelectDistinct("BoxProtocolTable", ds.Tables[0], "BoxCmdOutTypeName");


                    this.cboCommand.DataSource = dsCommands.Tables[0];
                    this.cboCommand.DataBind();
                }
                else
                {
                    this.cboCommand.DataSource = ds;
                    this.cboCommand.DataBind();
                }




                cboCommand.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboCommand_Item_0"), "-1"));
                if (sn.User.OrganizationId == 952 && (sn.User.UserGroupId == 1 || sn.User.UserGroupId == 7))
                    cboCommand.Items.Insert(1, new ListItem("Update Position Satellite", "72"));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }

        private void CboOutput_Fill(string LicensePlate)
        {
            try
            {

                DataSet ds = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                //DumpBeforeCall(sn, string.Format("CboOutput_Fill : LicensePlate = {0}", LicensePlate));
                if (objUtil.ErrCheck(dbv.GetVehicleOutputsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleOutputsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "No outputs for vehicle: " + LicensePlate + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        cboOutput.Items.Clear();
                        cboOutput.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboOutput_Item_0"), "-1"));
                        return;
                    }

                if (xml == "")
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "No outputs for vehicle: " + LicensePlate + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    cboOutput.Items.Clear();
                    cboOutput.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboOutput_Item_0"), "-1"));
                    return;
                }

                strrXML = new StringReader(xml);
                FillOutputs(strrXML);

                cboOutput.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboOutput_Item_0"), "-1"));
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

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
            this.dgTraceSensors.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgTraceSensors_CancelCommand);
            this.dgTraceSensors.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgTraceSensors_EditCommand);
            this.dgTraceSensors.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgTraceSensors_UpdateCommand);
            this.dgBoxSetupSensors.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgBoxSetupSensors_CancelCommand);
            this.dgBoxSetupSensors.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgBoxSetupSensors_EditCommand);
            this.dgBoxSetupSensors.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgBoxSetupSensors_UpdateCommand);

        }
        #endregion

        protected void cboCommand_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";
                this.lblMessage.Visible = false;
                this.lblCommandStatus.Text = "";
                this.tblSensors.Visible = true;
                this.cboTARPeriod.Visible = true;
                sn.Cmd.DualComm = false;
                chkScheduleTask.Checked = false;
                chkSendToFleet.ForeColor = Color.Black;

                if (Convert.ToInt32(this.cboCommand.SelectedItem.Value) != -1)
                {
                    this.cboOutput.SelectedIndex = -1;
                    this.cboOutput.DataBind();
                }

                HideAllTables();

                //Box Setup
                if (Convert.ToInt32(this.cboCommand.SelectedItem.Value) == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.Setup)
                    || Convert.ToInt32(this.cboCommand.SelectedItem.Value) == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.ExtendedSetup))
                {

                    if (Convert.ToInt32(this.cboCommand.SelectedItem.Value) == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.ExtendedSetup))
                        this.tblViolations.Visible = true;

                    short ProtocolId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[0]["BoxProtocolTypeId"]);
                    if (ProtocolId == Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.RAPv10))
                    {
                        this.tblBoxSetup.Visible = true;
                        this.tblSensors.Visible = false;
                        this.tblSet.Visible = false;
                        this.tblRapSet.Visible = true;
                        DgBoxSetupSensors_Fill(sn.Cmd.SelectedVehicleLicensePlate);
                        DgTraceSensors_Fill(sn.Cmd.SelectedVehicleLicensePlate);
                        CboReportingFreq_Fill();
                    }
                    else
                    {
                        this.tblBoxSetup.Visible = true;
                        this.tblSensors.Visible = false;
                        this.tblSet.Visible = true;
                        this.tblRapSet.Visible = false;
                        DgBoxSetupSensors_Fill(sn.Cmd.SelectedVehicleLicensePlate);
                        DgTraceSensors_Fill(sn.Cmd.SelectedVehicleLicensePlate);
                        CboSpeed_Fill();
                        CboGeo_Fill();
                        CboReportingFreq_Fill();
                    }
                }



                chkSendToFleet.Enabled = true;


                switch (Convert.ToInt32(this.cboCommand.SelectedItem.Value))
                {
                    case (Int32)VLF.CLS.Def.Enums.CommandType.SecurityCode:
                        this.tblSecCode.Visible = true;
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.VoiceMessage:
                        this.tblVoiceMsg.Visible = true;
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetReportInterval:
                        this.tblReportInterval.Visible = true;
                        CboReportingFreq_Fill();
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetSpeedThreshold:
                        this.tblSpeedThreshold.Visible = true;
                        CboSpeed_Fill();
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetFence:
                        this.tblGeoFence.Visible = true;
                        CboGeo_Fill();
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetEnabledSensor:
                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetExtendedEnabledSensor:
                        DgBoxSetupSensors_Fill(sn.Cmd.SelectedVehicleLicensePlate);
                        this.dgSetSensors.DataSource = sn.Map.SetupSensors;
                        this.dgSetSensors.DataBind();
                        this.tblSetSensors.Visible = true;
                        this.tblSensors.Visible = false;
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetTrace:
                        this.tblSensors.Visible = false;
                        tblSetTrace.Visible = true;
                        DgTraceSensors_Fill(sn.Cmd.SelectedVehicleLicensePlate);
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.ProxCards:
                        this.tblProxyCards.Visible = true;
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetVCROffDelay:
                        this.tblVCRDelay.Visible = true;
                        cboVCRDelayPeriod.Enabled = true;
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetTARMode:
                        this.tblTAR.Visible = true;
                        this.cboTARPeriod.SelectedIndex = -1;
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.ChangeBoxID:
                        this.tblChangeBoxID.Visible = true;
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetServiceRequired:
                        this.tblServiceRequired.Visible = true;
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetIdle:
                        this.tblSetIdle.Visible = true;
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetRecordsCount:
                        this.tblSetRecordCount.Visible = true;
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetOdometer:
                        this.tblSetOdometer.Visible = true;
                        this.txtSetOdometer.Visible = true;
                        this.lblOdometer.Visible = false;
                        this.cboMesUnits.Visible = true;
                        this.lblUnit.Visible = false;
                        this.chkSendToFleet.Enabled = false;
                        this.chkSendToFleet.ForeColor = Color.Gray;
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.WiFiUpgrade:
                        this.tblWiFiUpgrade.Visible = true;
                        this.txtWiFiUpgradeOutput.Visible = true;
                        this.selectWiFiUpgradeFileType.Visible = true;

                        BoxProfileHelper.ConnectionString = sConnectionString;
                        boxModel = BoxProfileHelper.GetBoxMainBoardModelByBoxId(sn.Cmd.BoxId);
                        selectWiFiUpgradeFileType_Fill(boxModel, sn.Cmd.BoxId, sn.UserID);
                        break;

                    case (Int32)VLF.CLS.Def.Enums.CommandType.WriteEEPROMData:
                        BoxProfileHelper.ConnectionString = sConnectionString;
                        boxModel = BoxProfileHelper.GetBoxMainBoardModelByBoxId(sn.Cmd.BoxId);
                        string boxModelName = "Unknown";
                        switch (boxModel)
                        {
                            case 2:
                                boxModelName = "SFM2000";
                                break;
                            case 3:
                                boxModelName = "SFM3000";
                                break;
                            case 7:
                                boxModelName = "SFM7000";
                                break;

                            default:
                                boxModelName = "Unsupported Box";
                                break;
                        }

                        lblEEPROMFeature.Text = "Select EEPROM Feature: [" + sn.Cmd.BoxId + "]/" + boxModelName;
                        this.tblEEPROMFeature.Visible = true;
                        this.tblRowBuzzerOutput.Visible = false;

                        this.tblEEPROMSettings.Visible = true;
                        this.lblEEPROMData.Visible = true;
                        this.txtEEPROMData.Visible = true;

                        CboEEPROMFeature_Fill(boxModel, sn.Cmd.BoxId, sn.UserID);
                        break;

                    case (Int32)VLF.CLS.Def.Enums.CommandType.ReadEEPROMData:
                        this.tblEEPROMSettings.Visible = true;
                        this.lblEEPROMData.Visible = false;
                        this.txtEEPROMData.Visible = false;
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.KeyFobSetup:
                        this.tblKeyFobSetup.Visible = true;
                        break;

                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetBoxSleepTime:
                        this.tblBoxSleepTime.Visible = true;
                        break;

                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetIridiumFilter:
                        this.tblIridiumFilter.Visible = true;
                        break;

                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetPowerSaveConfig:
                        this.tblPowerSave.Visible = true;
                        break;

                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetExtendedReportInterval:
                        this.tblExtendedReportInterval.Visible = true;
                        break;

                    case (Int32)VLF.CLS.Def.Enums.CommandType.AddGeoZone:
                        CboUnAssGeoZone_Fill();
                        this.tblAddGeoZone.Visible = true;
                        break;

                    case (Int32)VLF.CLS.Def.Enums.CommandType.DeleteGeoZone:
                        CboAssGeoZone_Fill();
                        this.tblDeleteGeoZones.Visible = true;
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetFuelConfiguration:
                        this.tblFuelConfiguration.Visible = true;
                        this.tblDiesel.Visible = false;
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.StartDeviceTest:
                        this.tblDeviceTest.Visible = true;
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.WriteSMC:
                        this.tblWriteSMC.Visible = true;
                        break;

                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetTrailerWakeSleepTimers:
                        this.tblTrailerWakeSleep.Visible = true;
                        break;

                }


                //Clear Scheduled options	
                EnableDualComm(true);

                // Check Dual mode
                DataSet ds = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                if (objUtil.ErrCheck(dbv.GetCommandProtocolTypesInfo(sn.UserID, sn.SecId, sn.Cmd.BoxId, Convert.ToInt16(this.cboCommand.SelectedItem.Value), ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetCommandProtocolTypesInfo(sn.UserID, sn.SecId, sn.Cmd.BoxId, Convert.ToInt16(this.cboCommand.SelectedItem.Value), ref xml), true))
                    {
                        EnableDualComm(false);
                        sn.Cmd.ProtocolTypeId = -1;
                        sn.Cmd.CommModeId = -1;
                        return;
                    }

                if (xml == "")
                {
                    EnableDualComm(false);
                    sn.Cmd.ProtocolTypeId = -1;
                    sn.Cmd.CommModeId = -1;
                    return;
                }

                ViewState["ChPriority"] = "-1";

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);

                if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count == 1))
                {
                    sn.Cmd.ProtocolTypeId = Convert.ToInt16(ds.Tables[0].Rows[0]["BoxProtocolTypeId"]);
                    sn.Cmd.CommModeId = Convert.ToInt16(ds.Tables[0].Rows[0]["CommModeId"]);
                    ViewState["ChPriority"] = Convert.ToString(ds.Tables[0].Rows[0]["ChPriority"]);
                }
                else
                {
                    sn.Cmd.ProtocolTypeId = -1;
                    sn.Cmd.CommModeId = -1;

                }

                if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 1))
                    EnableDualComm(true);
                else
                    EnableDualComm(false);



                //TAR Mode
                if (Convert.ToInt32(this.cboCommand.SelectedItem.Value) == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.SetTARMode))
                    EnableDualComm(false);

                //PowerOffDelay
                if (Convert.ToInt32(this.cboCommand.SelectedItem.Value) == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.SetPowerOffDelay))
                {
                    this.tblPowerOffDelay.Visible = true;
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }

        }

        protected void cmdSendCommand_Click(object sender, System.EventArgs e)
        {



            try
            {
                this.lblMessage.Text = "";
                sn.Cmd.SendCommandMessage = "";
                this.lblMessage.Visible = false;



                if (Convert.ToInt32(this.cboCommand.SelectedItem.Value) == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SelectCommand");
                    return;
                }


                if ((this.chkScheduleTask.Checked) && (this.cboSchInterval.SelectedItem.Value == "0" || this.cboSchPeriod.SelectedItem.Value == "0"))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidScheduledPeriodIntervalError");
                    return;
                }

                LocationMgr.Location dbl = new LocationMgr.Location();


                if (sn.Cmd.BoxId == 0)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SendCommandFailed");
                    return;
                }


                int boxId = sn.Cmd.BoxId;
                //Dual Communication
                sn.Cmd.DualComm = Convert.ToBoolean(optCommMode.Items[2].Selected);

                bool cmdSent = false;
                string paramList = "";
                Int16 cmdType = Convert.ToInt16(this.cboCommand.SelectedItem.Value);

                switch (cmdType)
                {
                    case (Int16)VLF.CLS.Def.Enums.CommandType.Setup:// Box Setup 
                    case (Int16)VLF.CLS.Def.Enums.CommandType.ExtendedSetup:// Box Setup 
                        this.lblMessage.Text = "";
                        if (Convert.ToInt64(this.cboTracePeriodSetup.SelectedItem.Value) < Convert.ToInt64(this.cboTraceIntervalSetup.SelectedItem.Value))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidTraceNumbers");
                            return;
                        }

                        if (cmdType == (Int16)VLF.CLS.Def.Enums.CommandType.ExtendedSetup)
                            paramList = GenerateExtendedBoxSetupParams();
                        else
                            paramList = GenerateBoxSetupParams();

                        this.tblBoxSetup.Visible = false;

                        break;

                    case (Int16)VLF.CLS.Def.Enums.CommandType.SecurityCode:// Security Code
                        if (ValidSecurityCode())
                        {
                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.extSecurityCode + "1", this.txtGlobalUnarmCode.Text);
                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.extSecurityCode + "2", this.txtTARCode.Text);
                            this.tblSecCode.Visible = false;
                        }
                        else
                            return;

                        break;

                    case (Int16)VLF.CLS.Def.Enums.CommandType.VoiceMessage: // Voice Message
                        if (this.txtVoiceMsg.Text != "")
                        {
                            this.lblMessage.Text = "";
                            paramList += VLF.CLS.Util.MakePair("Voice Message", this.txtVoiceMsg.Text);
                        }
                        else
                        {
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_EnterVoiceMessage");
                            this.lblMessage.Visible = true;
                            return;
                        }

                        break;

                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetReportInterval: // Reporting Interval  
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyReportInterval, this.cboReportingFreq.SelectedItem.Value.ToString());
                        break;
                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetSpeedThreshold: // SetSpeed Threshold
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupSpeedThreshold, this.cboSpeedThreshold.SelectedItem.Value.ToString());
                        break;
                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetFence: // Set GeoFence Radius
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupGeoFenceRadius, this.cboGeoFence.SelectedItem.Value.ToString());
                        break;
                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetEnabledSensor: // SetEnabledSensor
                        GenerateSensorsMask(ref paramList);
                        break;
                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetExtendedEnabledSensor: // SetExtendedEnabledSensor
                        GenerateExtendedSensorsMask(ref paramList);
                        break;
                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetTrace: // SetTrace
                        this.lblMessage.Text = "";
                        if (Convert.ToInt64(this.cboTracePeriod.SelectedItem.Value) < Convert.ToInt64(this.cboTraceInterval.SelectedItem.Value))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_EnterVoiceMessage");
                            return;
                        }

                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupTracePeriod, this.cboTracePeriod.SelectedItem.Value.ToString());

                        if (this.cboTracePeriod.SelectedItem.Value.ToString() != "0")
                        {
                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupTraceInterval, this.cboTraceInterval.SelectedItem.Value.ToString());
                        }

                        string strTraceMask = "";
                        foreach (DataRow rowItem in sn.Cmd.DsSensorsTraceStates.Tables[0].Rows)
                        {
                            strTraceMask += rowItem["TraceStateId"].ToString();
                        }

                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupTraceStates, strTraceMask);
                        break;


                    case (Int16)VLF.CLS.Def.Enums.CommandType.ProxCards: // ProxCards
                        if (ValidProxyCards())
                        {
                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.extFacilityCode.ToString() + "1", this.txtFC1.Text);
                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.extFacilityCode.ToString() + "2", this.txtFC2.Text);
                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.extFacilityCode.ToString() + "3", this.txtFC3.Text);
                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.extFacilityCode.ToString() + "4", this.txtFC4.Text);
                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.extFacilityCode.ToString() + "5", this.txtFC5.Text);
                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.extIDNumber.ToString() + "1", this.txtIDN1.Text);
                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.extIDNumber.ToString() + "2", this.txtIDN2.Text);
                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.extIDNumber.ToString() + "3", this.txtIDN3.Text);
                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.extIDNumber.ToString() + "4", this.txtIDN4.Text);
                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.extIDNumber.ToString() + "5", this.txtIDN5.Text);
                            this.tblProxyCards.Visible = false;
                        }
                        else
                            return;

                        break;

                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetVCROffDelay: // SetVCROffDelay
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyVCRDelayTime, this.cboVCRDelayPeriod.SelectedItem.Value);
                        break;
                    case (Int16)VLF.CLS.Def.Enums.CommandType.ChangeBoxID: // ChangeBoxID
                        if (ValidChangeBoxId())
                        {

                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyBoxID, this.txtBoxId.Text);
                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keySIM, this.txtSIMNumber.Text);

                        }
                        break;

                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetTARMode: // SetTARMode
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyTARMode, this.cboTARPeriod.SelectedItem.Value);
                        this.tblTAR.Visible = false;
                        break;

                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetServiceRequired: // SetServiceRequired
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyServiceRequired, this.cboServiceRequired.SelectedItem.Value);
                        this.tblServiceRequired.Visible = false;
                        break;
                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetIdle: // SetIdle
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyIdleTime, this.cboSetIdle.SelectedItem.Value);
                        this.tblSetIdle.Visible = false;
                        break;
                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetRecordsCount: // SetRecordsCount
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyRecordsCount, this.cboSetRecordCount.SelectedItem.Value);
                        this.tblSetRecordCount.Visible = false;
                        break;
                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetPowerOffDelay: // SetPowerOffDelay
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyPowerOffDelay, this.txtPowerOffDelay.Text);
                        this.tblPowerOffDelay.Visible = false;
                        break;

                    case (Int16)VLF.CLS.Def.Enums.CommandType.WiFiUpgrade: // Flash WiFiUpgrade
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.KEY_WIFI_UPGRADE_COMMAND_CODE, this.selectWiFiUpgradeFileType.SelectedItem.Value);
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.KEY_WIFI_UPGRADE_COMMAND_OUTPUT, this.txtWiFiUpgradeOutput.Text);
                        this.tblWiFiUpgrade.Visible = false;
                        break;

                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetOdometer: // SetOdometer
                        if (!CheckIfNumberic(this.txtSetOdometer.Text))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidOdometer1");
                            return;
                        }

                        Int32 Odometer = 0;
                        Odometer = Convert.ToInt32(Convert.ToSingle(this.txtSetOdometer.Text) / Convert.ToDouble(this.cboMesUnits.SelectedItem.Value));

                        //if (sn.User.UnitOfMes == 0.6214)
                        //{
                        //   Odometer = Convert.ToInt32(Convert.ToSingle(this.txtSetOdometer.Text) / 0.6214);
                        //}
                        //else
                        //{
                        //   Odometer = Convert.ToInt32(Convert.ToSingle(this.txtSetOdometer.Text));
                        //}


                        if (Convert.ToInt32(Convert.ToSingle(this.txtSetOdometer.Text)) > 0 && Convert.ToInt32(Convert.ToSingle(this.txtSetOdometer.Text)) < 16777215)
                        {
                            paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyOdometerValue, Odometer.ToString());
                        }
                        else
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidOdometer2");
                            return;
                        }

                        this.tblSetOdometer.Visible = false;
                        break;

                    case (Int16)VLF.CLS.Def.Enums.CommandType.WriteEEPROMData:
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMOffset, this.txtEEPROMOffset.Text);
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMLength, this.txtEEPROMLength.Text);
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMData, this.txtEEPROMData.Text);
                        break;

                    case (Int16)VLF.CLS.Def.Enums.CommandType.ReadEEPROMData:
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMOffset, this.txtEEPROMOffset.Text);
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMLength, this.txtEEPROMLength.Text);
                        break;

                    case (Int16)VLF.CLS.Def.Enums.CommandType.KeyFobSetup:
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyRemoteControlSettings, this.optKeyFobSetup.SelectedItem.Value.ToString());
                        break;

                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetBoxSleepTime: // SetSleepTime
                        if (!CheckIfNumberic(this.txtSleepTime.Text) || (Convert.ToInt32(this.txtSleepTime.Text) < 0 || Convert.ToInt32(this.txtSleepTime.Text) > 24))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("msgInvalidBoxSleepTime");
                            return;
                        }
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyBoxSleepTimeValue, txtSleepTime.Text.ToString());
                        break;


                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetIridiumFilter:

                        Int64 setupIridiumSensorMask = -1;
                        Int64 setupIridiumMessageMask = -1;
                        Int64 setupIridiumSensorAlarmMask = -1;

                        try
                        {
                            setupIridiumSensorMask = (txtIridiumSensorMask.Text.Substring(0, 2) == "0x") ? Convert.ToInt64(txtIridiumSensorMask.Text.Substring(2, txtIridiumSensorMask.Text.ToString().Length - 2), 16) : Convert.ToInt64(txtIridiumSensorMask.Text);
                            setupIridiumMessageMask = (txtIridiumMsgMask.Text.Substring(0, 2) == "0x") ? Convert.ToInt64(txtIridiumMsgMask.Text.Substring(2, txtIridiumMsgMask.Text.ToString().Length - 2), 16) : Convert.ToInt64(txtIridiumMsgMask.Text);
                            setupIridiumSensorAlarmMask = (txtIridiumAlarmMask.Text.Substring(0, 2) == "0x") ? Convert.ToInt64(txtIridiumAlarmMask.Text.Substring(2, txtIridiumAlarmMask.Text.ToString().Length - 2), 16) : Convert.ToInt64(txtIridiumAlarmMask.Text);

                            //setupIridiumSensorMask = Convert.ToInt64(txtIridiumSensorMask.Text, 16);
                            //setupIridiumMessageMask = Convert.ToInt64(txtIridiumMsgMask.Text, 16);
                            //setupIridiumSensorAlarmMask = Convert.ToInt64(txtIridiumAlarmMask.Text, 16);
                        }
                        catch
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = "Invalid Mask parameters. Parameters should be in Hex (like: FFFFF)";
                            return;
                        }


                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupIridiumSensorMask, setupIridiumSensorMask.ToString());
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupIridiumMessageMask, setupIridiumMessageMask.ToString());
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupIridiumSensorAlarmMask, setupIridiumSensorAlarmMask.ToString());
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupIridiumInitDelay, this.txtIridiumInitDelay.Text);
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupUpdateMultiplier, this.txtIridiumMultiplier.Text);
                        break;


                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetPowerSaveConfig:
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setPowerCfgSleepTime, this.cboPowerCfgSleepTime.SelectedItem.Value);
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setMDTSleepTime, this.cboMDTSleepTime.SelectedItem.Value);
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setModemTurnOffTime, this.cboModemTurnOffTime.SelectedItem.Value);
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setPeriodicWakeupTime, this.cboPeriodicWakeupTime.SelectedItem.Value);
                        break;

                    case (Int16)VLF.CLS.Def.Enums.CommandType.SynchronizeGeoZone:
                        if (!this.chkScheduleTask.Checked)
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = "This command should be scheduled";
                            return;
                        }
                        break;

                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetExtendedReportInterval:
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyNumberReports, this.txtExtNumOfReport.Text);
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyReportInterval, this.txtExtRptInterval.Text);
                        break;


                    case (Int16)VLF.CLS.Def.Enums.CommandType.AddGeoZone:

                        if (!CheckIfNumberic(this.txtGeoZoneSpeed.Text))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = "GeoZone speed should be numeric";
                            return;
                        }

                        Int32 speed = 0;
                        speed = Convert.ToInt32(this.txtGeoZoneSpeed.Text);

                        if (sn.User.UnitOfMes == 0.6214)
                            speed = Convert.ToInt32(Convert.ToSingle(this.txtGeoZoneSpeed.Text) / 0.6214);


                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyGeozoneId.ToString(), this.cboUnAssignedGeoZones.SelectedItem.Value.ToString());
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyGeozoneSpeed.ToString(), speed.ToString());
                        DataRow[] drCollections = null;
                        drCollections = sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Select("GeozoneId='" + this.cboUnAssignedGeoZones.SelectedItem.Value.ToString() + "'", "", DataViewRowState.CurrentRows);
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyGeozoneSeverity.ToString(), drCollections[0]["SeverityId"].ToString());

                        break;


                    case (Int16)VLF.CLS.Def.Enums.CommandType.DeleteGeoZone:
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyGeozoneId.ToString() + "1", this.cboAssGeoZones.SelectedItem.Value.ToString());
                        break;
                    case (Int16)VLF.CLS.Def.Enums.CommandType.SetFuelConfiguration:
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMOffset, "0xA0");
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMLength, "12");
                        string EEPPROM = GenerateFuelEEPROMData();
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyEEPROMData, EEPPROM);
                        break;
                    case (Int16)VLF.CLS.Def.Enums.CommandType.StartDeviceTest:
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyTestId, this.cboDeviceTest.SelectedItem.Value);
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyTestParam, "0");
                        break;

                    case (Int16)VLF.CLS.Def.Enums.CommandType.WriteSMC:

                        if (!CheckIfNumberic(this.txtSMC_Code1.Text) || (Convert.ToInt32(this.txtSMC_Code1.Text) < 0 || Convert.ToInt32(this.txtSMC_Code1.Text) > 255))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = "Invalid Code 1";
                            return;
                        }

                        if (!CheckIfNumberic(this.txtSMC_Code2.Text) || (Convert.ToInt32(this.txtSMC_Code2.Text) < 0 || Convert.ToInt32(this.txtSMC_Code2.Text) > 255))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = "Invalid Code 2";
                            return;
                        }

                        if (!CheckIfNumberic(this.txtSMC_Code3.Text) || (Convert.ToInt32(this.txtSMC_Code3.Text) < 0 || Convert.ToInt32(this.txtSMC_Code3.Text) > 255))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = "Invalid Code 3";
                            return;
                        }

                        if (!CheckIfNumberic(this.txtSMC_Code4.Text) || (Convert.ToInt32(this.txtSMC_Code4.Text) < 0 || Convert.ToInt32(this.txtSMC_Code4.Text) > 255))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = "Invalid Code 4";
                            return;
                        }

                        if (!CheckIfNumberic(this.txtSMC_Code5.Text) || (Convert.ToInt32(this.txtSMC_Code5.Text) < 0 || Convert.ToInt32(this.txtSMC_Code5.Text) > 255))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = "Invalid Code 5";
                            return;
                        }
                        string smcCode = String.Format("{0:x2}", Convert.ToByte(this.txtSMC_Code1.Text)) + String.Format("{0:x2}", Convert.ToByte(this.txtSMC_Code2.Text)) + String.Format("{0:x2}", Convert.ToByte(this.txtSMC_Code3.Text)) + String.Format("{0:x2}", Convert.ToByte(this.txtSMC_Code4.Text)) + String.Format("{0:x2}", Convert.ToByte(this.txtSMC_Code5.Text));
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keySMCCodes, smcCode);
                        break;


                    case (Int32)VLF.CLS.Def.Enums.CommandType.SetTrailerWakeSleepTimers:
                        if (!CheckIfNumberic(this.txtTrailerWakeUpTime.Text) || Convert.ToInt32(this.txtTrailerWakeUpTime.Text) < 0 || !CheckIfNumberic(this.txtTrailerSleepTime.Text) || Convert.ToInt32(this.txtTrailerSleepTime.Text) < 0)
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = "Trailer Wake Up Time and Trailer Sleep Time should be positive numeric value";
                            return;
                        }
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyTrailerWakeTime, this.txtTrailerWakeUpTime.Text);
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyTrailerSleepTime, this.txtTrailerSleepTime.Text);
                        break;

                    case (Int32)VLF.CLS.Def.Enums.CommandType.Output_2_Single_Tap:
                        paramList += "SENSOR_NUM=6;SENSOR_STATUS=ON;";
                        //sn.Cmd.CommandId = 4;//Output
                        break;
                    case (Int32)VLF.CLS.Def.Enums.CommandType.Output_2_Double_Tap:
                        paramList += "SENSOR_NUM=6;SENSOR_STATUS=OFF;";
                        //sn.Cmd.CommandId = 4;//Output
                        break;

                    // case "08 Open Voute ON" //(Int32)VLF.CLS.Def.Enums.CommandType.Output_2_Double_Tap:
                    //    paramList += "SENSOR_NUM=3;SENSOR_STATUS=ON;";
                    //sn.Cmd.CommandId = 4;//Output
                    //  break;
                }


                short CommModeId = -1;
                short ProtocolId = -1;


                //CommModeId = sn.Cmd.CommModeId;
                //ProtocolId = sn.Cmd.ProtocolTypeId; ;


                if (Convert.ToBoolean(this.optCommMode.Items[0].Selected) || Convert.ToBoolean(this.optCommMode.Items[2].Selected))
                {

                    if (Convert.ToInt32(this.cboCommand.SelectedItem.Value) == (Int32)VLF.CLS.Def.Enums.CommandType.UpdatePositionSatellite)
                    {
                        if (sn.User.OrganizationId == 952)
                        {
                            ProtocolId = 21;
                            CommModeId = 7;
                        }
                        else
                        {
                            ProtocolId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[1]["BoxProtocolTypeId"]);
                            CommModeId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[1]["CommModeId"]);
                        }
                    }
                    else
                    {
                        ProtocolId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[0]["BoxProtocolTypeId"]);
                        CommModeId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[0]["CommModeId"]);
                    }
                }
                else if (Convert.ToBoolean(this.optCommMode.Items[1].Selected))
                {
                    ProtocolId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[1]["BoxProtocolTypeId"]);
                    CommModeId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[1]["CommModeId"]);
                }

                sn.Cmd.ProtocolTypeId = ProtocolId;
                sn.Cmd.CommModeId = CommModeId;


                //Scheduled Task
                if ((cboSchInterval.SelectedItem.Value != "0") && (cboSchPeriod.SelectedItem.Value != "0"))
                {
                    sn.Cmd.SchCommand = true;
                    sn.Cmd.SchInterval = Convert.ToInt32(cboSchInterval.SelectedItem.Value);
                    sn.Cmd.SchPeriod = Convert.ToInt32(cboSchPeriod.SelectedItem.Value);
                }
                else
                {
                    sn.Cmd.SchCommand = false;
                    sn.Cmd.SchInterval = 0;
                    sn.Cmd.SchPeriod = 0;
                }

                //Scheduled for entire fleet
                if (chkSendToFleet.Checked)
                {
                    //Synchronize vehicle geozone
                    if (cmdType == (Int16)VLF.CLS.Def.Enums.CommandType.SynchronizeGeoZone)
                        SyncGeoZoneforFleet();
                    else
                        FleetScheduler(cmdType, paramList);

                    return;
                }

                //Synchronize vehicle geozone
                if (cmdType == (Int16)VLF.CLS.Def.Enums.CommandType.SynchronizeGeoZone)
                {
                    this.lblMessage.Visible = true;
                    if (SyncGeoZoneforVehicle(Convert.ToInt32(lblVehicleId.Text), sn.Cmd.BoxId))
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("msgScheduledCommandSend");
                    else
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("msgScheduledCommandFailed");
                    return;
                }


                Int64 sessionTimeOut = 0;
                string errMsg = "";
                sn.Cmd.CommandId = Convert.ToInt16(this.cboCommand.SelectedItem.Value);

                if (Convert.ToInt16(this.cboCommand.SelectedItem.Value) == (Int16)VLF.CLS.Def.Enums.CommandType.SetFuelConfiguration)
                    sn.Cmd.CommandId = (Int16)VLF.CLS.Def.Enums.CommandType.WriteEEPROMData;

                if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, boxId, sn.Cmd.CommandId, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                    if (errMsg == "")
                    {
                        if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, boxId, sn.Cmd.CommandId, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                        {
                            this.lblMessage.Visible = true;

                            if (errMsg != "")
                                this.lblMessage.Text = errMsg;
                            else
                                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SendCommandFailedUsing") + " " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId;


                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Sent command failed.  User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            return;
                        }
                    }
                    else
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = errMsg;
                        return;
                    }

                if (cmdType == (Int16)VLF.CLS.Def.Enums.CommandType.AddGeoZone && sn.Map.SelectedVehicleID > 0)
                    AssignSelectedGeozoneToVehicle(sn.Map.SelectedVehicleID);


                sn.Cmd.CommandParams = paramList;
                sn.Cmd.ProtocolTypeId = ProtocolId;
                sn.Cmd.CommModeId = CommModeId;


                if (sn.User.OrganizationId == 952 && sn.Cmd.CommandId == 72)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "The command has been sent successfully";
                    return;
                }

                int cmdStatus = 0;

                // Box Reset Command with DualMode
                if (
                    ((Convert.ToInt32(this.cboCommand.SelectedItem.Value) == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.BoxReset)) && (optCommMode.Items[1].Selected))
                    || ((Convert.ToInt32(this.cboCommand.SelectedItem.Value) == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.BoxHardReset)) && (optCommMode.Items[1].Selected))
                    )
                {

                    //Get Command Status for GPRS
                    if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId, ref cmdStatus), false))
                        if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId, ref cmdStatus), true))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SendCommandFailedUsing") + " " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId;
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Sent command failed.  User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            return;
                        }

                    string Msg = (string)base.GetLocalResourceObject("lblMessage_Text_CommandSentUsing") + " ";
                    sn.Cmd.Status = (CommandStatus)cmdStatus;

                    if (cmdStatus == (int)CommandStatus.Ack)
                        Msg += (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId + " " + (string)base.GetLocalResourceObject("Msg_Success");
                    else
                        Msg += (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId + " " + (string)base.GetLocalResourceObject("Msg_Failed");

                    short AlternativeProtocol = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[1]["BoxProtocolTypeId"]);
                    sn.Cmd.ProtocolTypeId = AlternativeProtocol;
                    CommModeId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[1]["CommModeId"]);
                    sn.Cmd.CommModeId = CommModeId;


                    sessionTimeOut = 0;
                    if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, sn.Cmd.BoxId, sn.Cmd.CommandId, sn.Cmd.CommandParams, ref AlternativeProtocol, ref CommModeId, ref cmdSent, ref sessionTimeOut), false))
                        if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, sn.Cmd.BoxId, sn.Cmd.CommandId, sn.Cmd.CommandParams, ref AlternativeProtocol, ref CommModeId, ref cmdSent, ref sessionTimeOut), true))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SendCommandFailedUsing") + " " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId;
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Sent command failed.  User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            return;
                        }

                    //Get Command Status for Alternative Protocol
                    if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId, ref cmdStatus), false))
                        if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId, ref cmdStatus), true))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SendCommandFailedUsing") + " " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId;
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Sent command failed.  User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            return;
                        }

                    sn.Cmd.Status = (CommandStatus)cmdStatus;

                    if ((cmdStatus == (int)CommandStatus.Ack) || (cmdStatus == (int)CommandStatus.Pending))
                        Msg += " " + (string)base.GetLocalResourceObject("Msg_And") + " " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId + " " + (string)base.GetLocalResourceObject("Msg_Success");
                    else
                        Msg += " " + (string)base.GetLocalResourceObject("Msg_And") + " " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId + " " + (string)base.GetLocalResourceObject("Msg_Failed");


                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = Msg;
                    return;
                }

                this.cmdSendCommand.Enabled = false;
                this.cmdSendOutput.Enabled = false;
                this.cboCommand.Enabled = false;
                this.cboOutput.Enabled = false;
                //EnableDualComm(false);
                this.lblMessage.Visible = false;
                this.lblMessage.Text = "";
                this.lblCommandStatus.Text = "";
                this.dgSensors.Visible = false;
                this.tblWait.Visible = true;
                sn.Map.TimerStatus = true;


                if (sessionTimeOut > 0)
                    sn.Cmd.GetCommandStatusRefreshFreq = Convert.ToInt64(Math.Round(sessionTimeOut / 15.0)) * 1000;
                else
                    sn.Cmd.GetCommandStatusRefreshFreq = 1000;

                Response.Write("<script language='javascript'> parent.frmSensorTimer.location.href='frmSensorTimer.aspx' </script>");
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }

        }

        private void AssignSelectedGeozoneToVehicle(long vehicleId)
        {
            try
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                using (VLF.DAS.Logic.Vehicle dbVehicle = new VLF.DAS.Logic.Vehicle(sConnectionString))
                {
                    Int32 speed = 0;
                    speed = Convert.ToInt32(this.txtGeoZoneSpeed.Text);

                    if (sn.User.UnitOfMes == 0.6214)
                        speed = Convert.ToInt32(Convert.ToSingle(this.txtGeoZoneSpeed.Text) / 0.6214);

                    DataRow[] drCollections = null;
                    drCollections = sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Select("GeozoneId='" + this.cboUnAssignedGeoZones.SelectedItem.Value.ToString() + "'", "", DataViewRowState.CurrentRows);

                    dbVehicle.AddGeozone(vehicleId, Convert.ToInt16(this.cboUnAssignedGeoZones.SelectedItem.Value.ToString()), Convert.ToInt16(drCollections[0]["SeverityId"].ToString()), speed);
                }
            }
            catch { }
        }

        private void FillOutputs(StringReader xml)
        {
            try
            {
                cboOutput.Items.Clear();
                DataSet dsOutputs = new DataSet();
                DataSet dsResult = new DataSet();
                dsResult.ReadXml(xml);
                if ((dsResult != null) && (dsResult.Tables.Count > 0))
                {
                    DataTable tblOutputs = dsOutputs.Tables.Add("Outputs");
                    tblOutputs.Columns.Add("OutputId", typeof(short));
                    tblOutputs.Columns.Add("OutputName", typeof(string));
                    tblOutputs.Columns.Add("OutputAction", typeof(string));

                    string outputAction = "";
                    int index = 0;
                    int len = 0;
                    // split each source row to two destination rows (action 1/action2)
                    foreach (DataRow ittr in dsResult.Tables[0].Rows)
                    {
                        // first action
                        outputAction = ittr["OutputAction"].ToString().TrimEnd();
                        index = outputAction.IndexOf("/");
                        if (index < 1)
                        {
                            // wrong outputs format in database (should be action1/action2)
                            break;
                        }
                        len = outputAction.Length;
                        outputAction = outputAction.Substring(0, index);


                        object[] objRow;

                        if (outputAction.ToString().TrimEnd() != "*")
                        {
                            objRow = new object[3];
                            objRow[0] = Convert.ToInt32(ittr["OutputId"]) * 10 + 1;
                            objRow[1] = ittr["OutputName"].ToString().TrimEnd() + " " + outputAction;
                            objRow[2] = outputAction;
                            tblOutputs.Rows.Add(objRow);
                        }

                        // second action
                        if ((index < len) && (index > 0))
                        {
                            outputAction = ittr["OutputAction"].ToString().TrimEnd();
                            outputAction = outputAction.Substring(index + 1, len - index - 1);

                            if (outputAction.ToString().TrimEnd() != "*")
                            {
                                objRow = new object[3];
                                objRow[0] = Convert.ToInt32(ittr["OutputId"]) * 10;
                                objRow[1] = ittr["OutputName"].ToString().TrimEnd() + " " + outputAction;
                                objRow[2] = outputAction;
                                tblOutputs.Rows.Add(objRow);
                            }
                        }
                    }

                    cboOutput.DataSource = dsOutputs;
                    cboOutput.DataBind();
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }

        protected void cmdSendOutput_Click(object sender, System.EventArgs e)
        {
            try
            {

                if (Convert.ToInt32(this.cboOutput.SelectedItem.Value) == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SelectOutput");
                    return;
                }

                this.lblCommandStatus.Text = "";
                this.lblMessage.Text = "";

                short outputID = Convert.ToInt16(cboOutput.SelectedItem.Value);

                string paramList = "";
                // Off values
                if ((outputID & 1) == 0)
                {
                    paramList = VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keySensorNum, (outputID / 10).ToString());
                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keySensorStatus, VLF.CLS.Def.Const.valOFF);

                }
                else // On values
                {
                    paramList = VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keySensorNum, ((outputID - 1) / 10).ToString());
                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keySensorStatus, VLF.CLS.Def.Const.valON);
                }
                // send command
                bool cmdSent = false;
                short commandID = (short)(outputID / 10); //(short)VLF.CLS.Def.Enums.CommandType.Output;

                LocationMgr.Location dbl = new LocationMgr.Location();

                short ProtocolId = -1;
                short CommModeId = -1;
                sn.Cmd.CommModeId = -1;
                sn.Cmd.ProtocolTypeId = -1;

                //Dual Communication
                if (Convert.ToBoolean(optCommMode.Items[1].Selected) || Convert.ToBoolean(optCommMode.Items[2].Selected))
                    sn.Cmd.DualComm = true;

                //Scheduled Task
                if ((cboSchInterval.SelectedItem.Value != "0") && (cboSchPeriod.SelectedItem.Value != "0"))
                {
                    sn.Cmd.SchCommand = true;
                    sn.Cmd.SchInterval = Convert.ToInt32(cboSchInterval.SelectedItem.Value);
                    sn.Cmd.SchPeriod = Convert.ToInt32(cboSchPeriod.SelectedItem.Value);
                }
                else
                {
                    sn.Cmd.SchCommand = false;
                    sn.Cmd.SchInterval = 0;
                    sn.Cmd.SchPeriod = 0;
                }

                if (chkSendToFleet.Checked)
                {
                    FleetScheduler(commandID, paramList);
                    return;
                }

                Int64 sessionTimeOut = 0;

                if (sn.Cmd.BoxId == 0)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SendOutputFailed");
                    return;
                }

                if (objUtil.ErrCheck(dbl.SendOutput(sn.UserID, sn.SecId, DateTime.Now, sn.Cmd.BoxId, commandID, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false))
                    if (objUtil.ErrCheck(dbl.SendOutput(sn.UserID, sn.SecId, DateTime.Now, sn.Cmd.BoxId, commandID, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true))
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SendOutputFailed");
                        return;
                    }

                this.cmdSendCommand.Enabled = false;
                this.cmdSendOutput.Enabled = false;
                this.cboCommand.Enabled = false;
                this.cboOutput.Enabled = false;

                sn.Cmd.ProtocolTypeId = ProtocolId;
                sn.Cmd.CommModeId = CommModeId;
                sn.Cmd.CommandParams = paramList;
                sn.Cmd.CommandId = commandID;
                sn.Cmd.CommModeId = 0;

                this.dgSensors.Visible = false;
                this.tblWait.Visible = true;

                sn.Map.TimerStatus = true;
                Response.Write("<script language='javascript'> parent.frmSensorTimer.location.href='frmSensorTimer.aspx' </script>");
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }

        }

        protected void cmdCancelSend_Click(object sender, System.EventArgs e)
        {
            try
            {
                LocationMgr.Location dbl = new LocationMgr.Location();


                if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId), false))
                    if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId), true))
                    {
                        CancelCommand();
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Cancel command failed " + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        return;
                    }

                CancelCommand();
                sn.Map.TimerStatus = false;
            }

            catch (Exception Ex)
            {
                CancelCommand();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void CancelCommand()
        {
            this.dgSensors.Visible = true;
            this.tblWait.Visible = false;
            this.cmdSendCommand.Enabled = true;
            this.cmdSendOutput.Enabled = true;
            this.cboCommand.Enabled = true;
            this.cboOutput.Enabled = true;
            sn.Map.TimerStatus = false;

        }

        private void GetVehicleInfo(string LicensePlate)
        {
            try
            {

                //DataSet ds = new DataSet();
                //StringReader strrXML = null;
                //string xml = "";
                //ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                //if (objUtil.ErrCheck(dbv.GetVehicleInfoXML(sn.UserID, sn.SecId, LicensePlate, ref xml), false))
                //    if (objUtil.ErrCheck(dbv.GetVehicleInfoXML(sn.UserID, sn.SecId, LicensePlate, ref xml), true))
                //    {
                //        return;
                //    }

                //if (xml == "")
                //{
                //    return;
                //}

                //strrXML = new StringReader(xml);
                //ds.ReadXml(strrXML);
                //if (ds.Tables[0].Columns.Count > 0)
                //{
                //    this.lblVehicleName.Text = ds.Tables[0].Rows[0]["Description"].ToString();
                //    this.lblBoxId.Text = ds.Tables[0].Rows[0]["BoxId"].ToString();
                //    sn.Cmd.BoxId = Convert.ToInt32(ds.Tables[0].Rows[0]["BoxId"]);
                //    this.lblVehicleId.Text = ds.Tables[0].Rows[0]["VehicleId"].ToString();
                //}


                if (sn.Map.DsFleetInfo == null)
                {
                    DgFleetInfo_Fill();
                }
                if (sn.Map.DsFleetInfo.Tables.Count < 1)
                {
                    DgFleetInfo_Fill();
                }



                DataRow[] drArr = sn.Map.DsFleetInfo.Tables[0].Select("LicensePlate='" + LicensePlate + "'");
                if (drArr == null || drArr.Length == 0)
                {
                    sn.Cmd.BoxId = -1;
                    return;
                }
                DataRow rowItem = drArr[0];

                this.lblVehicleName.Text = rowItem["Description"].ToString();
                this.lblBoxId.Text = rowItem["BoxId"].ToString();
                sn.Cmd.BoxId = Convert.ToInt32(rowItem["BoxId"]);
                sn.Map.SelectedVehicleID = Convert.ToInt64(rowItem["VehicleId"]);
                this.lblVehicleId.Text = rowItem["VehicleId"].ToString();
                this.lblLastComm.Text = Convert.ToDateTime(rowItem["LastCommunicatedDateTime"]).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                this.lblSensorMask.Text = rowItem["SensorMask"].ToString();



            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }



        private void DgFleetInfo_Fill()
        {
            try
            {


                int fleetId = Convert.ToInt32(Session["EngineFleetSelected"]);
                if (fleetId == 0 && sn.History.FleetId > 0) fleetId = (int)sn.History.FleetId;
                dsFleetInfo = new DataSet();



                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "GetVehiclesLastKnownPositionInfo.User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        return;
                    }
                StringReader strrXML = new StringReader(xml);
                dsFleetInfo.ReadXml(strrXML);
                sn.Map.DsFleetInfo = null;
                sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                dsFleetInfo.Tables[0].DefaultView.Sort = "chkBoxShow" + " DESC";
                sn.Map.DsFleetInfo = dsFleetInfo;
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

        private void DgBoxSetupSensors_Fill(string LicensePlate)
        {
            try
            {
                DataTable dtSensors = this.GetAllSensorsForVehicle(LicensePlate, false);
                /*
                   new DataSet();
                string xml = "";
            
                StringReader strrXML = null;
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                   if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                   {
                      return;
                   }

                if (xml == "")
                {
                   return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);
                */

                // Show Combobox
                DataColumn dc = new DataColumn();
                dc.ColumnName = "chkSet";
                dc.DataType = Type.GetType("System.Boolean");
                dc.DefaultValue = true;
                dtSensors.Columns.Add(dc);

                DataColumn TraceStateId = new DataColumn("TraceStateId", Type.GetType("System.Int16"));
                TraceStateId.DefaultValue = VLF.CLS.Def.Enums.SensorsTraceState.Disable;
                dtSensors.Columns.Add(TraceStateId);
                DataColumn TraceStateName = new DataColumn("TraceStateName", Type.GetType("System.String"));
                TraceStateName.DefaultValue = VLF.CLS.Def.Enums.SensorsTraceState.Disable;
                dtSensors.Columns.Add(TraceStateName);

                if (sn.Map.SetupSensors != null)
                {
                    foreach (DataRow rowSN in sn.Map.SetupSensors.Rows)
                    {
                        if (rowSN["chkSet"].ToString().ToLower() == "true")
                        {

                            foreach (DataRow rowLast in dtSensors.Rows)
                            {
                                if (rowLast["SensorId"].ToString() == rowSN["SensorId"].ToString())
                                {
                                    rowLast["chkSet"] = "true";
                                    break;
                                }
                            }
                        }
                    }
                }



                DataView myView = dtSensors.DefaultView;
                myView.RowFilter = "SensorName not like '%" + VLF.CLS.Def.Const.keySensorNotInUse + "%'";

                this.dgBoxSetupSensors.DataSource = myView;
                this.dgBoxSetupSensors.DataBind();
                sn.Map.SetupSensors = dtSensors;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }

        }

        protected void cmdSelectAll_Click(object sender, System.EventArgs e)
        {
            SelectAllSensors();
            this.dgBoxSetupSensors.DataSource = sn.Map.SetupSensors;
            this.dgBoxSetupSensors.DataBind();
        }

        private void SelectAllSensors()
        {
            try
            {
                DgBoxSetupSensors_Fill(sn.Cmd.SelectedVehicleLicensePlate);

                foreach (DataRow rowItem in sn.Map.SetupSensors.Rows)
                {
                    rowItem["chkSet"] = true;

                }


            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }

        private void UnselectAllSensors()
        {
            try
            {
                DgBoxSetupSensors_Fill(sn.Cmd.SelectedVehicleLicensePlate);

                foreach (DataRow rowItem in sn.Map.SetupSensors.Rows)
                {
                    rowItem["chkSet"] = false;

                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }

        protected void cmdUnselect_Click(object sender, System.EventArgs e)
        {
            UnselectAllSensors();
            this.dgBoxSetupSensors.DataSource = sn.Map.SetupSensors;
            this.dgBoxSetupSensors.DataBind();
        }

        private string GenerateExtendedBoxSetupParams()
        {
            try
            {
                string paramList = "";

                DataSet ds = new DataSet();
                StringReader strrXML = null;
                string xml = "";

                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                //Get Box communication mode
                if (objUtil.ErrCheck(dbv.GetCommInfoByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetCommInfoByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), true))
                    {
                        return "";
                    }

                if (xml == "")
                {
                    return "";
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);


                string IP = "";
                string Port = "";

                string phoneNum = "0";

                if (ds != null && ds.Tables.Count > 0)
                {

                    foreach (DataRow ittr in ds.Tables[0].Rows)
                    {
                        switch ((VLF.CLS.Def.Enums.CommAddressType)Convert.ToInt16(ittr["CommAddressTypeId"]))
                        {
                            case VLF.CLS.Def.Enums.CommAddressType.IP:
                                IP = ittr["CommAddressValue"].ToString().TrimEnd();
                                break;
                            case VLF.CLS.Def.Enums.CommAddressType.Port:
                                Port = ittr["CommAddressValue"].ToString().TrimEnd();
                                break;
                            case VLF.CLS.Def.Enums.CommAddressType.PhoneNum:
                                phoneNum = ittr["CommAddressValue"].ToString().TrimEnd().Replace("\0", "9");
                                break;
                        }
                    }
                }

                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupPhoneNumber, phoneNum);

                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupHarshAcceleration, GetViolationValue(this.txtsetupHarshAcceleration.Text));
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupHarshBraking, GetViolationValue(this.txtsetupHarshBraking.Text));
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupExtremeAcceleration, GetViolationValue(this.txtsetupExtremeAcceleration.Text));
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupExtremeBraking, GetViolationValue(this.txtsetupExtremeBraking.Text));

                //	paramList += VLF.CLS.Util.MakePair("IP address",IP+"/"+Port);

                ds.Tables.Clear();

                // define sensors mask
                // 0x40 for visible character
                byte[] bSensors = { 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40 };
                for (int i = 0; i < dgBoxSetupSensors.Items.Count; i++)
                {
                    CheckBox ch = (CheckBox)(dgBoxSetupSensors.Items[i].Cells[1].Controls[1]);
                    if (ch.Checked)
                    {
                        int sensorID = Convert.ToInt32(dgBoxSetupSensors.Items[i].Cells[0].Text);
                        // position in bytes rows
                        int idxRow = (sensorID - 1) / 4;
                        // position in bits order
                        int idxCol = (sensorID - 1) % 4;
                        byte bit = 1;
                        bSensors[idxRow] |= (byte)(bit << idxCol);
                    }
                    foreach (DataRow rowItem in sn.Map.SetupSensors.Rows)
                    {
                        if (dgBoxSetupSensors.Items[i].Cells[0].Text.ToString() == rowItem["SensorId"].ToString())
                        {
                            rowItem["chkSet"] = ch.Checked;
                        }
                    }
                }


                byte[] bMainSensors = new Byte[8];
                Array.Copy(bSensors, 0, bMainSensors, 0, 8);

                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupSensorsMask, System.Text.Encoding.ASCII.GetString(bMainSensors));

                short ProtocolId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[0]["BoxProtocolTypeId"]);
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupGPSFrequency, this.cboFreguency.SelectedItem.Value.ToString());
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupSpeedThreshold, this.cboSpeed.SelectedItem.Value);
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupGeoFenceRadius, this.cboGeo.SelectedItem.Value);
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupCommMode, this.cboCommMode.SelectedItem.Value.ToString().TrimEnd());


                //Trace

                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupTracePeriod, this.cboTracePeriodSetup.SelectedItem.Value.ToString());
                if (this.cboTracePeriodSetup.SelectedItem.Value.ToString() != "0")
                {
                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupTraceInterval, this.cboTraceIntervalSetup.SelectedItem.Value.ToString());
                }

                string strTraceMask = "";
                foreach (DataRow rowItem in sn.Map.SetupSensors.Rows)
                {
                    strTraceMask += rowItem["TraceStateId"].ToString();
                }

                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupTraceStates, strTraceMask);


                return paramList;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                return "";
            }

        }

        private string GenerateBoxSetupParams()
        {
            try
            {

                string paramList = "";

                DataSet ds = new DataSet();
                StringReader strrXML = null;
                string xml = "";

                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                //Get Box communication mode
                if (objUtil.ErrCheck(dbv.GetCommInfoByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetCommInfoByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), true))
                    {
                        return "";
                    }

                if (xml == "")
                {
                    return "";
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);


                string IP = "";
                string Port = "";

                string phoneNum = "0";

                if (ds != null && ds.Tables.Count > 0)
                {

                    foreach (DataRow ittr in ds.Tables[0].Rows)
                    {
                        switch ((VLF.CLS.Def.Enums.CommAddressType)Convert.ToInt16(ittr["CommAddressTypeId"]))
                        {
                            case VLF.CLS.Def.Enums.CommAddressType.IP:
                                IP = ittr["CommAddressValue"].ToString().TrimEnd();
                                break;
                            case VLF.CLS.Def.Enums.CommAddressType.Port:
                                Port = ittr["CommAddressValue"].ToString().TrimEnd();
                                break;
                            case VLF.CLS.Def.Enums.CommAddressType.PhoneNum:
                                phoneNum = ittr["CommAddressValue"].ToString().TrimEnd().Replace("\0", "9");
                                break;
                        }
                    }
                }

                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupPhoneNumber, phoneNum);
                //	paramList += VLF.CLS.Util.MakePair("IP address",IP+"/"+Port);

                ds.Tables.Clear();

                // define sensors mask
                // 0x40 for visible character
                byte[] bSensors = { 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40 };
                for (int i = 0; i < dgBoxSetupSensors.Items.Count; i++)
                {
                    CheckBox ch = (CheckBox)(dgBoxSetupSensors.Items[i].Cells[1].Controls[1]);
                    if (ch.Checked)
                    {
                        int sensorID = Convert.ToInt32(dgBoxSetupSensors.Items[i].Cells[0].Text);
                        // position in bytes rows
                        int idxRow = (sensorID - 1) / 4;
                        // position in bits order
                        int idxCol = (sensorID - 1) % 4;
                        byte bit = 1;
                        bSensors[idxRow] |= (byte)(bit << idxCol);
                    }
                    foreach (DataRow rowItem in sn.Map.SetupSensors.Rows)
                    {
                        if (dgBoxSetupSensors.Items[i].Cells[0].Text.ToString() == rowItem["SensorId"].ToString())
                        {
                            rowItem["chkSet"] = ch.Checked;
                        }
                    }
                }

                // first 2 sensors
                byte[] bMainSensors = new Byte[2];
                Array.Copy(bSensors, 0, bMainSensors, 0, 2);
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupSensorsMask, System.Text.Encoding.ASCII.GetString(bMainSensors));

                short ProtocolId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[0]["BoxProtocolTypeId"]);
                if (ProtocolId == Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.RAPv10))
                {

                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupGPSFrequency, this.cboGPSFrequency.SelectedItem.Value.ToString());
                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupGPSFrequencyStationary, this.cboGPSFreqStat.SelectedItem.Value.ToString());
                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupDistanceInterval, this.cboDistInterval.SelectedItem.Value.ToString());

                }
                else
                {
                    // next 8 sensors
                    byte[] bExtSensors = new Byte[8];
                    Array.Copy(bSensors, 2, bExtSensors, 0, 8);
                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupExtendedSensorsMask, System.Text.Encoding.ASCII.GetString(bExtSensors));
                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupGPSFrequency, this.cboFreguency.SelectedItem.Value.ToString());
                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupSpeedThreshold, this.cboSpeed.SelectedItem.Value);
                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupGeoFenceRadius, this.cboGeo.SelectedItem.Value);
                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupCommMode, this.cboCommMode.SelectedItem.Value.ToString().TrimEnd());


                    //Trace


                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupTracePeriod, this.cboTracePeriodSetup.SelectedItem.Value.ToString());
                    if (this.cboTracePeriodSetup.SelectedItem.Value.ToString() != "0")
                    {
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupTraceInterval, this.cboTraceIntervalSetup.SelectedItem.Value.ToString());
                    }

                    string strTraceMask = "";
                    foreach (DataRow rowItem in sn.Map.SetupSensors.Rows)
                    {
                        strTraceMask += rowItem["TraceStateId"].ToString();
                    }

                    paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupTraceStates, strTraceMask);
                }

                return paramList;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                return "";
            }

        }

        private void CboSpeed_Fill()
        {
            try
            {
                ListItem ls;
                this.cboSpeed.Items.Clear();
                this.cboSpeedThreshold.Items.Clear();

                ls = new ListItem();
                ls.Value = "0";
                ls.Text = "Disabled";
                this.cboSpeed.Items.Add(ls);
                this.cboSpeedThreshold.Items.Add(ls);


                DataSet ds = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetBoxConfigInfo(sn.UserID, sn.SecId, Convert.ToInt32(this.lblBoxId.Text), ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetBoxConfigInfo(sn.UserID, sn.SecId, Convert.ToInt32(this.lblBoxId.Text), ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {
                    return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);

                Int16 BoxProtocolTypeId = Convert.ToInt16(ds.Tables[0].Rows[0]["BoxProtocolTypeId"].ToString());


                if ((BoxProtocolTypeId == Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.HGIv40)) ||
                    (BoxProtocolTypeId == Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.HGIv60)) ||
                    (BoxProtocolTypeId == Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.HGIv70)) ||
                    (BoxProtocolTypeId == Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.HGIv80)) ||
                    (BoxProtocolTypeId == Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.XSv10)))
                {

                    if (sn.User.UnitOfMes == 1)
                    {
                        foreach (string str in VLF.CLS.Def.Const.SpeedTriggerKMH)
                        {
                            ls = new ListItem();
                            ls.Value = str;
                            ls.Text = str + " kmh";
                            this.cboSpeed.Items.Add(ls);
                            this.cboSpeedThreshold.Items.Add(ls);
                        }
                    }
                    else
                    {
                        foreach (string str in VLF.CLS.Def.Const.SpeedTriggerMPH)
                        {
                            ls = new ListItem();
                            ls.Value = Convert.ToString(Convert.ToInt32(Math.Round(Convert.ToInt32(str) * 1.6094)));
                            ls.Text = str + " mph";
                            this.cboSpeed.Items.Add(ls);
                            this.cboSpeedThreshold.Items.Add(ls);
                        }
                    }
                }
                else
                {

                    foreach (string str in VLF.CLS.Def.Const.SpeedTrigger)
                    {
                        ls = new ListItem();
                        ls.Value = str;
                        ls.Text = str + " kmh/" + Convert.ToInt32(Convert.ToInt32(str) / 1.6094).ToString() + " mph";
                        this.cboSpeed.Items.Add(ls);
                        this.cboSpeedThreshold.Items.Add(ls);
                    }
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }

        private void CboGeo_Fill()
        {
            try
            {
                ListItem ls;
                this.cboGeo.Items.Clear();
                this.cboGeoFence.Items.Clear();
                int i = 1;
                int GeoVal = 0;
                int mtr = 0;

                ls = new ListItem();
                if (Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[0]["BoxProtocolTypeId"]) != Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.HGIv50))
                {
                    ls.Value = "0";
                    ls.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                    this.cboGeo.Items.Add(ls);
                    this.cboGeoFence.Items.Add(ls);
                }

                for (i = 1; i <= 15; i++)
                {
                    ls = new ListItem();
                    GeoVal = i * 200;
                    mtr = Convert.ToInt32(GeoVal * 0.3048);
                    ls.Value = mtr.ToString();
                    ls.Text = GeoVal.ToString() + " ft/" + mtr.ToString() + " m";
                    this.cboGeo.Items.Add(ls);
                    this.cboGeoFence.Items.Add(ls);
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void cboOutput_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.tblSensors.Visible = true;
            this.tblBoxSetup.Visible = false;
            this.tblBoxSetupInfo.Visible = false;
            this.tblBoxStatusInfo.Visible = false;
            this.tblSecCode.Visible = false;
            this.tblVoiceMsg.Visible = false;
            this.lblMessage.Text = "";
            this.lblCommandStatus.Text = "";
            sn.Cmd.DualComm = false;


            if (Convert.ToInt32(this.cboOutput.SelectedItem.Value) != -1)
            {
                this.cboCommand.SelectedIndex = -1;
                this.cboCommand.DataBind();
                EnableDualComm(true);
            }

            ////// Check Dual mode
            DataSet ds = new DataSet();
            StringReader strrXML = null;
            string xml = "";
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
            if (objUtil.ErrCheck(dbv.GetCommandProtocolTypesInfo(sn.UserID, sn.SecId, sn.Cmd.BoxId, 17, ref xml), false))
                if (objUtil.ErrCheck(dbv.GetCommandProtocolTypesInfo(sn.UserID, sn.SecId, sn.Cmd.BoxId, 17, ref xml), true))
                {
                    EnableDualComm(false);
                    sn.Cmd.ProtocolTypeId = -1;
                    sn.Cmd.CommModeId = -1;
                    return;
                }

            if (xml == "")
            {
                EnableDualComm(false);
                sn.Cmd.ProtocolTypeId = -1;
                sn.Cmd.CommModeId = -1;
                return;
            }

            ViewState["ChPriority"] = "-1";

            strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);

            if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count == 1))
            {
                sn.Cmd.ProtocolTypeId = Convert.ToInt16(ds.Tables[0].Rows[0]["BoxProtocolTypeId"]);
                sn.Cmd.CommModeId = Convert.ToInt16(ds.Tables[0].Rows[0]["CommModeId"]);
                ViewState["ChPriority"] = Convert.ToString(ds.Tables[0].Rows[0]["ChPriority"]);
            }
            else
            {
                sn.Cmd.ProtocolTypeId = -1;
                sn.Cmd.CommModeId = -1;

            }

        }

        protected void cmdRefresh_Click(object sender, System.EventArgs e)
        {
            string LicensePlate = sn.Cmd.SelectedVehicleLicensePlate;
            this.lblMessage.Text = "";
            DgSensors_Fill(LicensePlate);
        }

        private void GetBoxStatus(bool isExtendedVersion)
        {
            try
            {

                DataSet dsStatus = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetLastBoxStatusFromHistoryByLicensePlateExtended(sn.UserID, sn.SecId, sn.Cmd.SelectedVehicleLicensePlate, ref xml, isExtendedVersion), false))
                    if (objUtil.ErrCheck(dbv.GetLastBoxStatusFromHistoryByLicensePlateExtended(sn.UserID, sn.SecId, sn.Cmd.SelectedVehicleLicensePlate, ref xml, isExtendedVersion), true))
                    {
                        return;
                    }


                if (xml == "")
                {
                    return;
                }

                strrXML = new StringReader(xml);
                dsStatus.ReadXml(strrXML);

                string CustomProp = dsStatus.Tables[0].Rows[0]["CustomProp"].ToString();
                this.lblBoxStatusArmed.Text = dsStatus.Tables[0].Rows[0]["BoxArmed"].ToString();
                this.lblBoxStatusMainBattery.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyMainBattery, CustomProp) + " " + (string)base.GetLocalResourceObject("Text_Volt");


                this.lblBoxStatusBackupBatteryLabel.Text = (string)base.GetLocalResourceObject("lblBoxStatusBackupBatteryLabel_Text_BackupBattery");
                this.lblBoxStatusBackupBattery.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyBackupBattery, CustomProp) + " " + (string)base.GetLocalResourceObject("Text_Volt");


                this.lblBoxStatusFirmware.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyFirmwareVersion, CustomProp);
                this.lblBoxStatusSN.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keySerialNumber, CustomProp);
                this.lblStatusSIM.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keySIM, CustomProp);
                this.lblStatusCell.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyCell, CustomProp);

                if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyWaypointsUsage, CustomProp) == "")
                    this.lblBoxStatusWaypoint.Text = "0 %";
                else
                    this.lblBoxStatusWaypoint.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyWaypointsUsage, CustomProp);



                if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyPRLNumber, CustomProp) == "")
                {
                    this.lblPRLLabel.Visible = false;
                    this.lblPRL.Visible = false;
                }
                else
                {
                    this.lblPRLLabel.Visible = true;
                    this.lblPRL.Visible = true;
                    this.lblPRL.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyPRLNumber, CustomProp);
                }

                this.lblMDTMessagesValue.Text = (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyMDTNumMessages, CustomProp));

                this.tblBoxStatusInfo.Visible = true;

                DataSet dsSensors = new DataSet();
                DataTable dtResult = this.GetAllSensorsForVehicle(LicensePlate, false);
                /*
                   new DataSet("Sensors");

                if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                   if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                   {
                      return;
                   }

                if (xml == "")
                {
                   return;
                }

                strrXML = new StringReader(xml);
                dsResult.ReadXml(strrXML);
                */

                DataSet dsVehicleInfo = new DataSet();
                strrXML = new StringReader(xml);
                dsVehicleInfo.ReadXml(strrXML);
                string SensorMask = dsStatus.Tables[0].Rows[0]["SensorMask"].ToString();



                if ((dtResult != null) && (dtResult.Rows.Count > 0))
                {
                    DataTable tblSensors = dsSensors.Tables.Add("SensorsInformation");
                    tblSensors.Columns.Add("SensorId", typeof(short));
                    tblSensors.Columns.Add("SensorName", typeof(string));
                    tblSensors.Columns.Add("SensorAction", typeof(string));
                    tblSensors.Columns.Add("SensorStatus", typeof(bool));

                    // move over all sensors and set current status
                    short snsId = 0;
                    int index = 0;
                    object[] objRow = null;
                    string snsAction = "";
                    UInt64 checkBit = 1;
                    foreach (DataRow ittr in dtResult.Rows)
                    {

                        if (!ittr["SensorName"].ToString().TrimEnd().Contains(VLF.CLS.Def.Const.keySensorNotInUse))
                        {
                            snsAction = ittr["SensorAction"].ToString().TrimEnd();
                            index = snsAction.IndexOf("/");
                            if (index < 1)
                            {
                                // wrong sensors format in the database (should be action1/action2)
                                break;
                            }
                            objRow = new object[4];
                            snsId = Convert.ToInt16(ittr["SensorId"]);
                            objRow[0] = snsId;
                            objRow[1] = ittr["SensorName"].ToString().TrimEnd();
                            UInt64 shift = 1;
                            checkBit = shift << (snsId - 1);
                            if ((Convert.ToUInt64(SensorMask) & checkBit) == 0)
                            {
                                objRow[2] = snsAction.Substring(index + 1);
                                objRow[3] = false;
                            }
                            else
                            {
                                objRow[2] = snsAction.Substring(0, index);
                                objRow[3] = true;
                            }
                            tblSensors.Rows.Add(objRow);
                        }
                    }
                }

                dsSensors.Tables["SensorsInformation"].DefaultView.Sort = "SensorId";
                this.dgBoxStatusInfo.DataSource = dsSensors.Tables["SensorsInformation"];
                this.dgBoxStatusInfo.DataBind();




            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void GetBoxSetup(bool isExtendedVersion)
        {
            try
            {

                DataSet dsSetup = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetLastBoxSetupFromHistoryByLicensePlateExtended(sn.UserID, sn.SecId, sn.Cmd.SelectedVehicleLicensePlate, ref xml, isExtendedVersion), false))
                    if (objUtil.ErrCheck(dbv.GetLastBoxSetupFromHistoryByLicensePlateExtended(sn.UserID, sn.SecId, sn.Cmd.SelectedVehicleLicensePlate, ref xml, isExtendedVersion), true))
                    {
                        return;
                    }


                if (xml == "")
                {
                    return;
                }

                strrXML = new StringReader(xml);
                dsSetup.ReadXml(strrXML);
                string CustomProp = dsSetup.Tables[0].Rows[0]["CustomProp"].ToString();



                short ProtocolId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[0]["BoxProtocolTypeId"]);
                if (ProtocolId == Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.RAPv10))
                {


                    this.tblGetBoxSet.Visible = false;
                    this.tblGetBoxSetRAP.Visible = true;


                    if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGPSFrequency, CustomProp) == "Disabled")
                        this.lblBoxSetupGPSFreqRAP.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                    else
                        this.lblBoxSetupGPSFreqRAP.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGPSFrequency, CustomProp) + " sec.";


                    if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGPSFrequencyStationary, CustomProp) == "Disabled")
                        this.lblBoxSetupGPSFregStatRAP.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                    else
                        this.lblBoxSetupGPSFregStatRAP.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGPSFrequencyStationary, CustomProp) + " min.";



                    if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupDistanceInterval, CustomProp) == "Disabled")
                        this.lblBoxSetupDistIntRAP.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                    else
                        this.lblBoxSetupDistIntRAP.Text = Convert.ToDouble(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupDistanceInterval, CustomProp)) / 1000 + " km.";

                }
                else
                {


                    this.tblGetBoxSet.Visible = true;
                    this.tblGetBoxSetRAP.Visible = false;

                    try
                    {
                        if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGPSFrequency, CustomProp) == "Disabled")
                            this.lblBoxSetupGPSFreg.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                        else
                            this.lblBoxSetupGPSFreg.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGPSFrequency, CustomProp) + " sec.";
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }


                    try
                    {
                        if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyIdleDuration, CustomProp) == "Disabled")
                            this.lblIdle.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                        else
                            this.lblIdle.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyIdleDuration, CustomProp) + " sec.";
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }



                    try
                    {
                        if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGeoFenceRadius, CustomProp) == "Disabled")
                            this.lblBoxSetupGeo.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                        else
                        {
                            if (Convert.ToInt32(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGeoFenceRadius, CustomProp)) == 0)
                                this.lblBoxSetupGeo.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                            else
                                this.lblBoxSetupGeo.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGeoFenceRadius, CustomProp).ToString() + " m/" + Convert.ToInt32(Convert.ToInt32(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGeoFenceRadius, CustomProp)) * 3.28).ToString() + " ft";
                        }
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }


                    try
                    {
                        if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupSpeedThreshold, CustomProp) == "Disabled")
                            this.lblBoxSetupSpeed.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                        else if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupSpeedThreshold, CustomProp) == "")
                            this.lblBoxSetupSpeed.Text = "";
                        else
                            this.lblBoxSetupSpeed.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupSpeedThreshold, CustomProp) + " kmh/" + Convert.ToInt32(Convert.ToInt32(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupSpeedThreshold, CustomProp)) / 1.6094).ToString() + " mph";
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }


                    try
                    {
                        if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupTraceInterval, CustomProp) == "Disabled")
                            this.lblTraceInterval.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                        else if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupTraceInterval, CustomProp) == "")
                            this.lblTraceInterval.Text = "";
                        else
                            this.lblTraceInterval.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupTraceInterval, CustomProp) + " sec";
                    }

                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }


                    try
                    {
                        if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupTracePeriod, CustomProp) == "Disabled")
                        {
                            this.lblTracePeriod.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                            this.lblTraceInterval.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                        }
                        else if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupTracePeriod, CustomProp) == "")
                            this.lblTracePeriod.Text = "";
                        else
                            this.lblTracePeriod.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupTracePeriod, CustomProp) + " sec";
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }


                    try
                    {
                        for (int i = 0; i < cboCommMode.Items.Count; i++)
                        {
                            if (cboCommMode.Items[i].Text.TrimEnd() == VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupCommMode, CustomProp).TrimEnd())
                            {
                                cboCommMode.SelectedIndex = i;
                                break;
                            }
                        }

                        this.lblCommMode.Text = cboCommMode.SelectedItem.Text.TrimEnd();
                    }

                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }




                }


                this.tblBoxSetupInfo.Visible = true;

                DataSet dsSensors = new DataSet();

                DataTable dtResult = this.GetAllSensorsForVehicle(LicensePlate, false);
                /*
                   new DataSet("Sensors");

                if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                   if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                   {
                      return;
                   }

                if (xml == "")
                {
                   return;
                }

                strrXML = new StringReader(xml);
                dsResult.ReadXml(strrXML);
                */
                DataSet dsVehicleInfo = new DataSet();
                strrXML = new StringReader(xml);
                dsVehicleInfo.ReadXml(strrXML);
                string ExtendedSensorsMask = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupExtendedSensorsMask, CustomProp);
                UInt64 extSensors = 0;
                byte sensQuater = 0;

                if (ExtendedSensorsMask.Length == 8)
                {
                    for (int k = 7; k >= 0; k--)
                    {
                        sensQuater = (byte)Convert.ToChar(ExtendedSensorsMask[k]);
                        extSensors |= (ulong)(sensQuater & 0xF);
                        extSensors <<= 4;
                    }
                }
                UInt64 sensorMask = 0;
                try { sensorMask = Convert.ToUInt64(dsSetup.Tables[0].Rows[0]["SensorMask"].ToString()); }
                catch { }
                extSensors <<= 4;
                extSensors |= sensorMask;

                object[] objRow = null;

                // Sensors Info
                if ((dtResult != null) && (dtResult.Rows.Count > 0))
                {
                    DataTable tblSensors = dsSensors.Tables.Add("SensorsInformation");
                    tblSensors.Columns.Add("SensorId", typeof(short));
                    tblSensors.Columns.Add("SensorName", typeof(string));
                    tblSensors.Columns.Add("SensorAction", typeof(string));
                    tblSensors.Columns.Add("SensorStatus", typeof(bool));

                    // move over all sensors and set current status
                    short snsId = 0;
                    int index = 0;

                    string snsAction = "";
                    UInt64 checkBit = 1;
                    foreach (DataRow ittr in dtResult.Rows)
                    {
                        if (!ittr["SensorName"].ToString().TrimEnd().Contains(VLF.CLS.Def.Const.keySensorNotInUse))
                        {
                            snsAction = ittr["SensorAction"].ToString().TrimEnd();
                            index = snsAction.IndexOf("/");
                            if (index < 1)
                            {
                                // wrong sensors format in the database (should be action1/action2)
                                break;
                            }
                            objRow = new object[4];
                            snsId = Convert.ToInt16(ittr["SensorId"]);
                            objRow[0] = snsId;
                            objRow[1] = ittr["SensorName"].ToString().TrimEnd();
                            UInt64 shift = 1;
                            checkBit = shift << (snsId - 1);
                            if ((extSensors & checkBit) == 0)
                            {
                                objRow[2] = (string)base.GetLocalResourceObject("Text_Disabled"); // snsAction.Substring(index + 1);
                                objRow[3] = false;
                            }
                            else
                            {
                                objRow[2] = (string)base.GetLocalResourceObject("Text_Enabled");//snsAction.Substring(0,index);
                                objRow[3] = true;
                            }
                            tblSensors.Rows.Add(objRow);
                        }
                    }
                }



                if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetExtendedSetup))
                {
                    try
                    {
                        this.lblHarshAccelerationText.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupHarshAcceleration, CustomProp);
                        this.lblHarshBrakingText.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupHarshBraking, CustomProp);
                        this.lblExtremeAccelerationText.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupExtremeAcceleration, CustomProp);
                        this.lblExtremeBrakingText.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupExtremeBraking, CustomProp);
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " User:" + sn.UserID.ToString() + ". Violations params error.  Form:frmSensorsInfo.aspx"));
                    }
                }


                //Trace Info


                string strTrace = VLF.CLS.Util.PairFindValue(Const.setupTraceStates, CustomProp);

                if (strTrace != "")
                {
                    DataTable tblTrace = new DataTable();
                    tblTrace.Columns.Add("SensorName", typeof(string));
                    tblTrace.Columns.Add("TraceState", typeof(string));


                    Array enmArr = Enum.GetValues(typeof(Enums.SensorsTraceState));
                    objRow = null;




                    // set length not more than 8 (sensors)
                    int length = (strTrace.Length >= 8) ? 8 : strTrace.Length;
                    int i = 0;

                    if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.GetExtendedSetup))
                        length = (strTrace.Length >= 32) ? 32 : strTrace.Length;


                    try
                    {

                        foreach (DataRow ittr in dtResult.Rows)
                        {

                            if (i < strTrace.Length)
                            {
                                if (!ittr["SensorName"].ToString().TrimEnd().Contains(VLF.CLS.Def.Const.keySensorNotInUse))
                                {
                                    objRow = new object[2];
                                    byte mask = (byte)strTrace[i];
                                    mask -= 0x30;

                                    objRow[0] = ittr["SensorName"].ToString().TrimEnd();
                                    objRow[1] = ((Enums.SensorsTraceState)mask).ToString();
                                    tblTrace.Rows.Add(objRow);
                                }
                                i++;
                            }
                            else
                            {
                                break;
                            }

                        }

                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " User:" + sn.UserID.ToString() + ". Trace info error.  Form:frmSensorsInfo.aspx"));
                    }

                    dgTraceBoxSetup.DataSource = tblTrace;
                    dgTraceBoxSetup.DataBind();
                }

                this.dgBoxSetupSensorsInfo.DataSource = dsSensors.Tables["SensorsInformation"];
                this.dgBoxSetupSensorsInfo.DataBind();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        protected void cmdCloseSetupInfo_Click(object sender, System.EventArgs e)
        {
            this.tblSensors.Visible = true;
            this.tblBoxSetupInfo.Visible = false;
        }

        protected void cmdCloseStatusInfo_Click(object sender, System.EventArgs e)
        {
            this.tblSensors.Visible = true;
            this.tblBoxStatusInfo.Visible = false;
        }

        private void HideAllTables()
        {
            this.tblWait.Visible = false;
            this.tblSecCode.Visible = false;
            this.tblBoxSetup.Visible = false;
            this.tblBoxSetupInfo.Visible = false;
            this.tblBoxStatusInfo.Visible = false;
            this.tblVoiceMsg.Visible = false;
            this.tblReportInterval.Visible = false;
            this.tblSpeedThreshold.Visible = false;
            this.tblGeoFence.Visible = false;
            this.tblSetSensors.Visible = false;
            this.tblSetTrace.Visible = false;
            this.tblBoxGeozones.Visible = false;
            this.tblProxyCards.Visible = false;
            this.tblVCRDelay.Visible = false;
            this.tblTAR.Visible = false;
            this.tblControllerStatus.Visible = false;
            this.tblChangeBoxID.Visible = false;
            this.tblServiceRequired.Visible = false;
            this.tblSetIdle.Visible = false;
            this.tblSetRecordCount.Visible = false;
            this.tblSetOdometer.Visible = false;
            this.tblWiFiUpgrade.Visible = false;
            this.tblPowerOffDelay.Visible = false;
            this.tblEEPROMFeature.Visible = false;
            this.tblEEPROMSettings.Visible = false;
            this.tblViolations.Visible = false;
            this.tblKeyFobSetup.Visible = false;
            this.tblBoxSleepTime.Visible = false;
            this.tblIridiumFilter.Visible = false;
            tblPowerSave.Visible = false;
            tblExtendedReportInterval.Visible = false;
            tblAddGeoZone.Visible = false;
            this.tblFuelConfiguration.Visible = false;
            tblDeviceTest.Visible = false;
            tblDeleteGeoZones.Visible = false;
            this.tblWriteSMC.Visible = false;
            tblTrailerWakeSleep.Visible = false;
            //Clear labels
            this.lblTarMode.Text = "";
        }

        protected void cmdSetAllSensors_Click(object sender, System.EventArgs e)
        {
            SelectAllSensors();
            this.dgSetSensors.DataSource = sn.Map.SetupSensors;
            this.dgSetSensors.DataBind();
        }

        protected void cmdUnselectAllSensors_Click(object sender, System.EventArgs e)
        {
            UnselectAllSensors();
            this.dgSetSensors.DataSource = sn.Map.SetupSensors;
            this.dgSetSensors.DataBind();
        }

        private void GenerateSensorsMask(ref string paramList)
        {
            try
            {
                byte[] bSensors = { 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40 };
                for (int i = 0; i < dgSetSensors.Items.Count; i++)
                {
                    CheckBox ch = (CheckBox)(dgSetSensors.Items[i].Cells[1].Controls[1]);
                    if (ch.Checked)
                    {
                        int sensorID = Convert.ToInt32(dgSetSensors.Items[i].Cells[0].Text);
                        // position in bytes rows
                        int idxRow = (sensorID - 1) / 4;
                        // position in bits order
                        int idxCol = (sensorID - 1) % 4;
                        byte bit = 1;
                        bSensors[idxRow] |= (byte)(bit << idxCol);
                    }
                    foreach (DataRow rowItem in sn.Map.SetupSensors.Rows)
                    {
                        if (dgSetSensors.Items[i].Cells[0].Text.ToString() == rowItem["SensorId"].ToString())
                        {
                            rowItem["chkSet"] = ch.Checked;
                        }
                    }
                }

                // first 2 sensors
                byte[] bMainSensors = new Byte[2];
                Array.Copy(bSensors, 0, bMainSensors, 0, 2);

                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupSensorsMask, System.Text.Encoding.ASCII.GetString(bMainSensors));

                // next 8 sensors
                byte[] bExtSensors = new Byte[8];
                Array.Copy(bSensors, 2, bExtSensors, 0, 8);
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupExtendedSensorsMask, System.Text.Encoding.ASCII.GetString(bExtSensors));



            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void GenerateExtendedSensorsMask(ref string paramList)
        {
            try
            {
                byte[] bSensors = { 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40 };
                for (int i = 0; i < dgSetSensors.Items.Count; i++)
                {
                    CheckBox ch = (CheckBox)(dgSetSensors.Items[i].Cells[1].Controls[1]);
                    if (ch.Checked)
                    {
                        int sensorID = Convert.ToInt32(dgSetSensors.Items[i].Cells[0].Text);
                        // position in bytes rows
                        int idxRow = (sensorID - 1) / 4;
                        // position in bits order
                        int idxCol = (sensorID - 1) % 4;
                        byte bit = 1;
                        bSensors[idxRow] |= (byte)(bit << idxCol);
                    }
                    foreach (DataRow rowItem in sn.Map.SetupSensors.Rows)
                    {
                        if (dgSetSensors.Items[i].Cells[0].Text.ToString() == rowItem["SensorId"].ToString())
                        {
                            rowItem["chkSet"] = ch.Checked;
                            break;
                        }
                    }
                }

                byte[] bMainSensors = new Byte[8];
                Array.Copy(bSensors, 0, bMainSensors, 0, 8);

                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.setupSensorsMask, System.Text.Encoding.ASCII.GetString(bMainSensors));
            }
            catch (Exception Ex)
            {
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ShowException", "alert('" + Ex.Message + "')", true);
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void DsTrace_Fill()
        {
            try
            {
                DataTable tblTrace = dsTrace.Tables.Add("TraceState");
                tblTrace.Columns.Add("TraceStateId", typeof(short));
                tblTrace.Columns.Add("TraceStateName", typeof(string));

                Array enmArr = Enum.GetValues(typeof(Enums.SensorsTraceState));
                string TraceState;
                object[] objRow;
                foreach (Enums.SensorsTraceState ittr in enmArr)
                {
                    TraceState = Enum.GetName(typeof(Enums.SensorsTraceState), ittr);
                    objRow = new object[2];
                    objRow[0] = Convert.ToInt16(ittr);
                    objRow[1] = TraceState;
                    dsTrace.Tables[0].Rows.Add(objRow);

                }
            }


            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        public int GetTrace(short TraceStateId)
        {
            try
            {
                DropDownList cboTraceState = new DropDownList();
                cboTraceState.DataValueField = "TraceStateId";
                cboTraceState.DataTextField = "TraceStateName";
                DsTrace_Fill();
                cboTraceState.DataSource = dsTrace;
                cboTraceState.DataBind();

                cboTraceState.SelectedIndex = -1;
                cboTraceState.Items.FindByValue(TraceStateId.ToString()).Selected = true;
                return cboTraceState.SelectedIndex;
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                return 0;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                return 0;
            }

        }

        private void DgTraceSensors_Fill(string LicensePlate)
        {
            try
            {
                DataTable dtSensors = this.GetAllSensorsForVehicle(LicensePlate, false);
                /*
                   new DataSet();

            
                StringReader strrXML = null;
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                   if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                   {
                      return;
                   }

                if (xml == "")
                {
                   return;
                }

                strrXML = new StringReader(xml);
                dsSensors.ReadXml(strrXML);
                */
                if (sn.Cmd.DsSensorsTraceStates.Tables[0].Rows.Count > 0)
                    sn.Cmd.DsSensorsTraceStates.Tables[0].Rows.Clear();

                foreach (DataRow rowItem in dtSensors.Rows)
                {
                    DataRow dr = sn.Cmd.DsSensorsTraceStates.Tables[0].NewRow();
                    dr["SensorId"] = rowItem["SensorId"].ToString().TrimEnd();
                    dr["SensorName"] = rowItem["SensorName"].ToString().TrimEnd();
                    sn.Cmd.DsSensorsTraceStates.Tables[0].Rows.Add(dr);
                }


                this.dgTraceSensors.DataSource = sn.Cmd.DsSensorsTraceStates.Tables[0];
                this.dgTraceSensors.DataBind();


            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }


        }

        private void dgTraceSensors_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            this.lblMessage.Text = "";
            dgTraceSensors.EditItemIndex = Convert.ToInt32(e.Item.ItemIndex);
            this.dgTraceSensors.DataSource = sn.Cmd.DsSensorsTraceStates;
            this.dgTraceSensors.DataBind();
            dgTraceSensors.SelectedIndex = -1;
        }

        private void dgTraceSensors_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            dgTraceSensors.EditItemIndex = -1;
            this.dgTraceSensors.DataSource = sn.Cmd.DsSensorsTraceStates;
            this.dgTraceSensors.DataBind();
        }

        private void dgTraceSensors_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {
                DropDownList cboTrace;
                string SensorId = dgTraceSensors.DataKeys[e.Item.ItemIndex].ToString();
                cboTrace = (DropDownList)e.Item.FindControl("cboTrace");
                foreach (DataRow rowItem in sn.Cmd.DsSensorsTraceStates.Tables[0].Rows)
                {
                    if (rowItem["SensorId"].ToString().TrimEnd() == SensorId)
                    {
                        rowItem["TraceStateName"] = cboTrace.SelectedItem.Text;
                        rowItem["TraceStateId"] = cboTrace.SelectedItem.Value;
                    }
                }

                dgTraceSensors.EditItemIndex = -1;
                this.dgTraceSensors.DataSource = sn.Cmd.DsSensorsTraceStates;
                this.dgTraceSensors.DataBind();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void CheckCommMode()
        {
            try
            {

                DataSet ds = new DataSet();

                StringReader strrXML = null;
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetBoxConfigInfo(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetBoxConfigInfo(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No configuration info for Box:" + sn.Cmd.BoxId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        return;
                    }

                if (xml == "")
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No configuration info for Box:" + sn.Cmd.BoxId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);

                sn.Cmd.DsProtocolTypes = ds;
                this.cboCommMode.DataSource = ds;
                this.cboCommMode.DataBind();


                if (ds.Tables[0].Rows.Count > 1)
                    EnableDualComm(true);
                else
                    EnableDualComm(false);


            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        protected void cmdCloseGeoZoneInfo_Click(object sender, System.EventArgs e)
        {
            this.tblSensors.Visible = true;
            this.tblBoxGeozones.Visible = false;
        }

        private void GetBoxGeoZoneIds()
        {
            try
            {

                DataSet dsGeoZone = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                if (objUtil.ErrCheck(dbv.GetLastGeoZoneIDsFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetLastGeoZoneIDsFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), true))
                    {
                        return;
                    }


                if (xml == "")
                    return;


                string CustomProp = "";
                strrXML = new StringReader(xml);

                dsGeoZone.ReadXml(strrXML);
                if (dsGeoZone.Tables[0].Rows.Count > 0)
                    CustomProp = dsGeoZone.Tables[0].Rows[0]["CustomProp"].ToString().TrimEnd();

                if (CustomProp == "")
                {
                    return;
                }

                //Create GeoZoneXML
                string GeoId = "";
                string strXML = "<ROOT>";
                int i = 1;
                while (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyGeozoneId.ToString() + i, CustomProp) != "")
                {
                    strXML += "<Geozones>";
                    GeoId = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyGeozoneId.ToString() + i, CustomProp);
                    strXML += "<GeozoneId>" + GeoId + "</GeozoneId>";
                    strXML += "</Geozones>";
                    i++;
                }
                strXML += "</ROOT>";

                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();
                if (objUtil.ErrCheck(dbo.GetOrganizationGeozonesInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, strXML, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetOrganizationGeozonesInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, strXML, ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                    return;


                DataSet dsBoxGeoZones = new DataSet();
                strrXML = new StringReader(xml);
                dsBoxGeoZones.ReadXml(strrXML);
                this.dgGeoZone.DataSource = dsBoxGeoZones;
                this.dgGeoZone.DataBind();

            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
        /*
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
        */
        private void dgBoxSetupSensors_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            dgBoxSetupSensors.EditItemIndex = -1;
            this.dgBoxSetupSensors.DataSource = sn.Map.SetupSensors;
            this.dgBoxSetupSensors.DataBind();
        }

        private void dgBoxSetupSensors_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            this.lblMessage.Text = "";
            dgBoxSetupSensors.EditItemIndex = Convert.ToInt32(e.Item.ItemIndex);
            this.dgBoxSetupSensors.DataSource = sn.Map.SetupSensors;
            this.dgBoxSetupSensors.DataBind();
            dgBoxSetupSensors.SelectedIndex = -1;
        }

        private void dgBoxSetupSensors_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {
                DropDownList cboTrace;
                string SensorId = dgTraceSensors.DataKeys[e.Item.ItemIndex].ToString();
                cboTrace = (DropDownList)e.Item.FindControl("cboTraceSetup");
                foreach (DataRow rowItem in sn.Map.SetupSensors.Rows)
                {
                    if (rowItem["SensorId"].ToString().TrimEnd() == SensorId)
                    {
                        rowItem["TraceStateName"] = cboTrace.SelectedItem.Text;
                        rowItem["TraceStateId"] = cboTrace.SelectedItem.Value;
                    }
                }

                dgBoxSetupSensors.EditItemIndex = -1;
                this.dgBoxSetupSensors.DataSource = sn.Map.SetupSensors;
                this.dgBoxSetupSensors.DataBind();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        bool CheckIfNumberic(string myNumber)
        {
            try
            {
                double dbl = Convert.ToDouble(myNumber);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidProxyCards()
        {
            if ((!CheckIfNumberic(this.txtIDN1.Text))
                || (!CheckIfNumberic(this.txtIDN2.Text))
                || (!CheckIfNumberic(this.txtIDN3.Text))
                || (!CheckIfNumberic(this.txtIDN4.Text))
                || (!CheckIfNumberic(this.txtIDN5.Text)))
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidIDNumber");
                return false;
            }

            if ((!CheckIfNumberic(this.txtFC1.Text))
                || (!CheckIfNumberic(this.txtFC2.Text))
                || (!CheckIfNumberic(this.txtFC3.Text))
                || (!CheckIfNumberic(this.txtFC4.Text))
                || (!CheckIfNumberic(this.txtFC5.Text)))
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidFacilityCode");
                return false;
            }

            if ((Convert.ToDouble(this.txtFC1.Text) < 0)
                || (Convert.ToDouble(this.txtFC2.Text) < 0)
                || (Convert.ToDouble(this.txtFC3.Text) < 0)
                || (Convert.ToDouble(this.txtFC4.Text) < 0)
                || (Convert.ToDouble(this.txtFC5.Text) < 0)
                || (Convert.ToDouble(this.txtFC1.Text) > 255)
                || (Convert.ToDouble(this.txtFC2.Text) > 255)
                || (Convert.ToDouble(this.txtFC3.Text) > 255)
                || (Convert.ToDouble(this.txtFC4.Text) > 255)
                || (Convert.ToDouble(this.txtFC5.Text) < 0))
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidFacilityCode");
                return false;
            }


            if ((Convert.ToDouble(this.txtIDN1.Text) < 0)
                || (Convert.ToDouble(this.txtIDN2.Text) < 0)
                || (Convert.ToDouble(this.txtIDN3.Text) < 0)
                || (Convert.ToDouble(this.txtIDN4.Text) < 0)
                || (Convert.ToDouble(this.txtIDN5.Text) < 0)
                || (Convert.ToDouble(this.txtIDN1.Text) > 65535)
                || (Convert.ToDouble(this.txtIDN2.Text) > 65535)
                || (Convert.ToDouble(this.txtIDN3.Text) > 65535)
                || (Convert.ToDouble(this.txtIDN4.Text) > 65535)
                || (Convert.ToDouble(this.txtIDN5.Text) > 65535))
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidIDNumber");
                return false;
            }


            return true;
        }

        private bool ValidSecurityCode()
        {
            if ((!CheckIfNumberic(this.txtGlobalUnarmCode.Text))
                || (Convert.ToDouble(this.txtGlobalUnarmCode.Text) < 0)
                || (Convert.ToDouble(this.txtGlobalUnarmCode.Text) > 65535))
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidGlobalUnarmCode");
                return false;
            }



            if ((!CheckIfNumberic(this.txtTARCode.Text))
                || (Convert.ToDouble(this.txtTARCode.Text) < 0)
                || (Convert.ToDouble(this.txtTARCode.Text) > 65535))
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidGlobalUnarmCode");
                return false;
            }


            return true;
        }

        private void GetVCRDelay()
        {
            try
            {

                DataSet ds = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetLastVCROffDelayFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetLastVCROffDelayFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), true))
                    {
                        return;
                    }


                if (xml == "")
                {
                    return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);
                string CustomProp = ds.Tables[0].Rows[0]["CustomProp"].ToString();
                string VCRDelay = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyVCRDelayTime, CustomProp).TrimEnd();
                cboVCRDelayPeriod.SelectedIndex = cboVCRDelayPeriod.Items.IndexOf(cboVCRDelayPeriod.Items.FindByValue(VCRDelay));
                cboVCRDelayPeriod.Enabled = false;

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void GetKeyFobStatus()
        {
            try
            {

                DataSet ds = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetLastKeyFobStatusFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetLastKeyFobStatusFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), true))
                    {
                        return;
                    }


                if (xml == "")
                {
                    return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);
                string CustomProp = ds.Tables[0].Rows[0]["CustomProp"].ToString();
                string keyKeyFobStatus = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyKeyFobStatus, CustomProp).TrimEnd();
                this.lblCommandStatus.Text = (string)base.GetLocalResourceObject("lblCommandStatus_Text_KeyFob") + " " + keyKeyFobStatus;
                this.lblCommandStatus.Font.Bold = true;

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void GetTarMode()
        {
            try
            {

                DataSet ds = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetLastTARModeFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetLastTARModeFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), true))
                    {
                        return;
                    }


                if (xml == "")
                {
                    return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);
                string CustomProp = ds.Tables[0].Rows[0]["CustomProp"].ToString();
                string keyTarMode = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyTARMode, CustomProp).TrimEnd();

                int Days = Convert.ToInt32(keyTarMode) / 24;
                int Hours = Convert.ToInt32(keyTarMode) % 24;
                if (keyTarMode == "0")
                    this.lblTarMode.Text = (string)base.GetLocalResourceObject("lblTarMode_Text_Off");
                else if (keyTarMode == "255")
                    this.lblTarMode.Text = (string)base.GetLocalResourceObject("lblTarMode_Text_AlwaysOn");
                else
                    this.lblTarMode.Text = Days + " " + (string)base.GetLocalResourceObject("lblTarMode_Text_DaysAnd") + " " + Hours + " " + (string)base.GetLocalResourceObject("lblTarMode_Text_Hours");


            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void GetControllerStatus()
        {
            try
            {

                DataSet ds = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetLastControllerStatusFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetLastControllerStatusFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), true))
                    {
                        return;
                    }


                if (xml == "")
                {
                    return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);
                string CustomProp = ds.Tables[0].Rows[0]["CustomProp"].ToString();
                string keyTarMode = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.extControllerVersion, CustomProp).TrimEnd();
                this.lblControllerVersion.Text = keyTarMode;
                this.tblControllerStatus.Visible = true;

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void cmdVehicleInfo_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmVehicleDescription.aspx?VehicleId=" + lblVehicleId.Text);
        }

        private void CheckStatusValidation()
        {

            try
            {
                foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (rowItem["VehicleId"].ToString() == this.lblVehicleId.Text)
                    {
                        if (Convert.ToDateTime(rowItem["LastCommunicatedDateTime"]) > Convert.ToDateTime(rowItem["OriginDateTime"]))
                            this.lblNotes.Text = "*" + (string)base.GetLocalResourceObject("lblNotes_Text_NoGPSAvailable");
                        else
                            this.lblNotes.Text = "";
                        return;
                    }

                }
            }
            catch
            {
                this.lblNotes.Text = "";
                return;
            }


        }

        private bool ValidChangeBoxId()
        {
            if ((!CheckIfNumberic(this.txtBoxId.Text))
                || (this.txtBoxId.Text.Length > 6))
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidBoxId");
                return false;
            }



            if ((!CheckIfNumberic(this.txtSIMNumber.Text))
                || (this.txtSIMNumber.Text.Length > 32))
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidSIM");
                return false;
            }


            return true;
        }

        protected void cboServiceRequired_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }

        private void CboReportingFreq_Fill()
        {
            short ProtocolId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[0]["BoxProtocolTypeId"]);
            if (ProtocolId == Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.TCPv10))
            {
                cboReportingFreq.Items.Clear();
                cboFreguency.Items.Clear();
                ListItem ls = new ListItem();
                ls.Value = "0";
                ls.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                cboReportingFreq.Items.Add(ls);
                cboFreguency.Items.Add(ls);
                ListItem ls1 = new ListItem();
                ls1.Value = "60";
                ls1.Text = "1 " + (string)base.GetLocalResourceObject("Text_Minutes");
                cboReportingFreq.Items.Add(ls1);
                cboFreguency.Items.Add(ls1);
                ListItem ls2 = new ListItem();
                ls2.Value = "300";
                ls2.Text = "5 " + (string)base.GetLocalResourceObject("Text_Minutes");
                ls2.Selected = true;
                cboReportingFreq.Items.Add(ls2);
                cboFreguency.Items.Add(ls2);
                ListItem ls3 = new ListItem();
                ls3.Value = "9000";
                ls3.Text = "15 " + (string)base.GetLocalResourceObject("Text_Minutes");
                cboReportingFreq.Items.Add(ls3);
                cboFreguency.Items.Add(ls3);
                ListItem ls4 = new ListItem();
                ls4.Value = "12000";
                ls4.Text = "20 " + (string)base.GetLocalResourceObject("Text_Minutes");
                cboReportingFreq.Items.Add(ls4);
                cboFreguency.Items.Add(ls4);
                ListItem ls5 = new ListItem();
                ls5.Value = "18000";
                ls5.Text = "30 " + (string)base.GetLocalResourceObject("Text_Minutes");
                cboReportingFreq.Items.Add(ls5);
                cboFreguency.Items.Add(ls5);
                ListItem ls6 = new ListItem();
                ls6.Value = "3600";
                ls6.Text = "1 " + (string)base.GetLocalResourceObject("Text_Hour");
                cboReportingFreq.Items.Add(ls6);
                cboFreguency.Items.Add(ls6);
                ListItem ls7 = new ListItem();
                ls7.Value = "7200";
                ls7.Text = "2 " + (string)base.GetLocalResourceObject("Text_Hours");
                cboReportingFreq.Items.Add(ls7);
                cboFreguency.Items.Add(ls7);
                ListItem ls8 = new ListItem();
                ls8.Value = "10800";
                ls8.Text = "3 " + (string)base.GetLocalResourceObject("Text_Hours");
                cboReportingFreq.Items.Add(ls8);
                cboFreguency.Items.Add(ls8);
                ListItem ls9 = new ListItem();
                ls9.Value = "14400";
                ls9.Text = "4 " + (string)base.GetLocalResourceObject("Text_Hours");
                cboReportingFreq.Items.Add(ls9);
                cboFreguency.Items.Add(ls9);
            }
            else
            {
                cboReportingFreq.Items.Clear();
                cboFreguency.Items.Clear();
                cboGPSFrequency.Items.Clear();
                ListItem ls = new ListItem();
                ls.Value = "0";
                ls.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                cboReportingFreq.Items.Add(ls);
                cboFreguency.Items.Add(ls);
                cboGPSFrequency.Items.Add(ls);
                ListItem ls1 = new ListItem();
                ls1.Value = "30";
                ls1.Text = "30 " + (string)base.GetLocalResourceObject("Text_Seconds");
                cboReportingFreq.Items.Add(ls1);
                cboFreguency.Items.Add(ls1);
                cboGPSFrequency.Items.Add(ls1);
                ListItem ls2 = new ListItem();
                ls2.Value = "120";
                ls2.Text = "2 " + (string)base.GetLocalResourceObject("Text_Minutes");
                cboReportingFreq.Items.Add(ls2);
                cboFreguency.Items.Add(ls2);
                cboGPSFrequency.Items.Add(ls2);
                ListItem ls3 = new ListItem();
                ls3.Value = "300";
                ls3.Text = "5 " + (string)base.GetLocalResourceObject("Text_Minutes");
                ls3.Selected = true;
                cboReportingFreq.Items.Add(ls3);
                cboFreguency.Items.Add(ls3);
                cboGPSFrequency.Items.Add(ls3);
                ListItem ls4 = new ListItem();
                ls4.Value = "600";
                ls4.Text = "10 " + (string)base.GetLocalResourceObject("Text_Minutes");
                cboReportingFreq.Items.Add(ls4);
                cboFreguency.Items.Add(ls4);
                cboGPSFrequency.Items.Add(ls4);
                ListItem ls5 = new ListItem();
                ls5.Value = "900";
                ls5.Text = "15 " + (string)base.GetLocalResourceObject("Text_Minutes");
                cboReportingFreq.Items.Add(ls5);
                cboFreguency.Items.Add(ls5);
                cboGPSFrequency.Items.Add(ls5);
                ListItem ls6 = new ListItem();
                ls6.Value = "1800";
                ls6.Text = "30 " + (string)base.GetLocalResourceObject("Text_Minutes");
                cboReportingFreq.Items.Add(ls6);
                cboFreguency.Items.Add(ls6);
                cboGPSFrequency.Items.Add(ls6);
                ListItem ls7 = new ListItem();
                ls7.Value = "3600";
                ls7.Text = "1 " + (string)base.GetLocalResourceObject("Text_Hour");
                cboReportingFreq.Items.Add(ls7);
                cboFreguency.Items.Add(ls7);
                cboGPSFrequency.Items.Add(ls7);
                ListItem ls8 = new ListItem();
                ls8.Value = "7200";
                ls8.Text = "2 " + (string)base.GetLocalResourceObject("Text_Hours");
                cboReportingFreq.Items.Add(ls8);
                cboFreguency.Items.Add(ls8);
                cboGPSFrequency.Items.Add(ls8);
                ListItem ls9 = new ListItem();
                ls9.Value = "14400";
                ls9.Text = "4 " + (string)base.GetLocalResourceObject("Text_Hours");
                cboReportingFreq.Items.Add(ls9);
                cboFreguency.Items.Add(ls9);
                cboGPSFrequency.Items.Add(ls9);
                ListItem ls10 = new ListItem();
                ls10.Value = "21600";
                ls10.Text = "6 " + (string)base.GetLocalResourceObject("Text_Hours");
                cboReportingFreq.Items.Add(ls10);
                cboFreguency.Items.Add(ls10);
                cboGPSFrequency.Items.Add(ls10);
                ListItem ls11 = new ListItem();
                ls11.Value = "28800";
                ls11.Text = "8 " + (string)base.GetLocalResourceObject("Text_Hours");
                cboReportingFreq.Items.Add(ls11);
                cboFreguency.Items.Add(ls11);
                cboGPSFrequency.Items.Add(ls11);
                ListItem ls12 = new ListItem();
                ls12.Value = "36000";
                ls12.Text = "10 " + (string)base.GetLocalResourceObject("Text_Hours");
                cboReportingFreq.Items.Add(ls12);
                cboFreguency.Items.Add(ls12);
                cboGPSFrequency.Items.Add(ls12);
                ListItem ls13 = new ListItem();
                ls13.Value = "43200";
                ls13.Text = "12 " + (string)base.GetLocalResourceObject("Text_Hours");
                cboReportingFreq.Items.Add(ls13);
                cboGPSFrequency.Items.Add(ls13);
                cboFreguency.Items.Add(ls13);
                ListItem ls14 = new ListItem();
                ls14.Value = "86400";
                ls14.Text = "24 " + (string)base.GetLocalResourceObject("Text_Hours");
                cboReportingFreq.Items.Add(ls14);
                cboFreguency.Items.Add(ls14);
                cboGPSFrequency.Items.Add(ls14);
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
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "GetVehiclesLastKnownPositionInfo.User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
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
                sn.Map.DsFleetInfo = dsFleetInfo;


            }



            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void GetOdometer()
        {

            DataSet ds = new DataSet();
            StringReader strrXML = null;

            string xml = "";
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            if (objUtil.ErrCheck(dbv.GetLastOdometerFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), false))
                if (objUtil.ErrCheck(dbv.GetLastOdometerFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), true))
                {
                    return;
                }


            if (xml == "")
            {
                return;
            }

            this.lblOdometer.Text = "";
            strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);
            string CustomProp = ds.Tables[0].Rows[0]["CustomProp"].ToString();
            this.lblOdometer.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyOdometerValue, CustomProp).TrimEnd();
            this.tblSetOdometer.Visible = true;
            this.txtSetOdometer.Visible = false;
            this.cboMesUnits.Visible = false;
            this.lblUnit.Visible = true;

        }

        private void GetEEPROMData()
        {

            DataSet ds = new DataSet();
            StringReader strrXML = null;

            string xml = "";
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            if (objUtil.ErrCheck(dbv.GetLastEEPROMDataFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), false))
                if (objUtil.ErrCheck(dbv.GetLastEEPROMDataFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), true))
                {
                    return;
                }


            if (xml == "")
            {
                return;
            }

            this.lblOdometer.Text = "";
            strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);
            string CustomProp = ds.Tables[0].Rows[0]["CustomProp"].ToString();
            this.txtEEPROMOffset.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyEEPROMOffset, CustomProp).TrimEnd();
            this.txtEEPROMLength.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyEEPROMLength, CustomProp).TrimEnd();
            this.txtEEPROMData.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyEEPROMData, CustomProp).TrimEnd();

            HideAllTables();
            this.tblEEPROMSettings.Visible = true;
            this.lblEEPROMData.Visible = true;
            this.txtEEPROMData.Visible = true;
            sn.Cmd.ProtocolTypeId = -1;
            sn.Cmd.CommModeId = -1;
            sn.Cmd.CommandId = Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.WriteEEPROMData);
            cboCommand.SelectedIndex = cboCommand.Items.IndexOf(cboCommand.Items.FindByValue(sn.Cmd.CommandId.ToString()));
        }

        private void GetIridiumFilterSetup()
        {

            DataSet ds = new DataSet();
            StringReader strrXML = null;

            string xml = "";
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            if (objUtil.ErrCheck(dbv.GetLastBoxMessageFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, Convert.ToInt32(VLF.CLS.Def.Enums.MessageType.SendIridiumFilter), ref xml), false))
                if (objUtil.ErrCheck(dbv.GetLastBoxMessageFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, Convert.ToInt32(VLF.CLS.Def.Enums.MessageType.SendIridiumFilter), ref xml), true))
                {
                    return;
                }


            if (xml == "")
            {
                return;
            }

            this.lblOdometer.Text = "";
            strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);
            string CustomProp = ds.Tables[0].Rows[0]["CustomProp"].ToString();
            try
            {
                this.txtIridiumSensorMask.Text = "0x" + Int64.Parse(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupIridiumSensorMask, CustomProp).TrimEnd()).ToString("X");
            }

            catch
            {
                this.txtIridiumSensorMask.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupIridiumSensorMask, CustomProp).TrimEnd();
            }


            try
            {
                this.txtIridiumMsgMask.Text = "0x" + Int64.Parse(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupIridiumMessageMask, CustomProp).TrimEnd()).ToString("X");
            }

            catch
            {
                this.txtIridiumMsgMask.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupIridiumMessageMask, CustomProp).TrimEnd();
            }



            try
            {
                this.txtIridiumAlarmMask.Text = "0x" + Int64.Parse(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupIridiumSensorAlarmMask, CustomProp).TrimEnd()).ToString("X");
            }

            catch
            {
                this.txtIridiumAlarmMask.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupIridiumSensorAlarmMask, CustomProp).TrimEnd();
            }

            this.txtIridiumInitDelay.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupIridiumInitDelay, CustomProp).TrimEnd();
            this.txtIridiumMultiplier.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupUpdateMultiplier, CustomProp).TrimEnd();
            this.tblIridiumFilter.Visible = true;
        }

        private void GetPowerSaveConfig()
        {

            DataSet ds = new DataSet();
            StringReader strrXML = null;

            string xml = "";
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            if (objUtil.ErrCheck(dbv.GetLastBoxMessageFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, Convert.ToInt32(VLF.CLS.Def.Enums.MessageType.SendPowerSaveConfiguration), ref xml), false))
                if (objUtil.ErrCheck(dbv.GetLastBoxMessageFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, Convert.ToInt32(VLF.CLS.Def.Enums.MessageType.SendPowerSaveConfiguration), ref xml), true))
                {
                    return;
                }


            if (xml == "")
            {
                return;
            }

            this.lblOdometer.Text = "";
            strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);
            string CustomProp = ds.Tables[0].Rows[0]["CustomProp"].ToString();

            string setPowerCfgSleepTime = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setPowerCfgSleepTime, CustomProp).TrimEnd();
            string setMDTSleepTime = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setMDTSleepTime, CustomProp).TrimEnd();
            string setModemTurnOffTime = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setModemTurnOffTime, CustomProp).TrimEnd();
            string setPeriodicWakeupTime = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setPeriodicWakeupTime, CustomProp).TrimEnd();


            try
            {
                cboPowerCfgSleepTime.SelectedIndex = cboPowerCfgSleepTime.Items.IndexOf(cboPowerCfgSleepTime.Items.FindByValue(setPowerCfgSleepTime));
                cboMDTSleepTime.SelectedIndex = cboMDTSleepTime.Items.IndexOf(cboMDTSleepTime.Items.FindByValue(setMDTSleepTime));
                cboModemTurnOffTime.SelectedIndex = cboModemTurnOffTime.Items.IndexOf(cboModemTurnOffTime.Items.FindByValue(setModemTurnOffTime));
                cboPeriodicWakeupTime.SelectedIndex = cboPeriodicWakeupTime.Items.IndexOf(cboPeriodicWakeupTime.Items.FindByValue(setPeriodicWakeupTime));

            }
            catch
            {
                this.tblPowerSave.Visible = false;
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Cannot get power save configuration. UserId: " + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

            this.tblPowerSave.Visible = true;
        }

        private void GetPowerOffDelay()
        {
            //
            //DataSet ds = new DataSet();
            //StringReader strrXML = null;

            //string xml = "";
            //ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            //if (objUtil.ErrCheck(dbv.GetLastOdometerFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), false))
            //    if (objUtil.ErrCheck(dbv.GetLastOdometerFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), true))
            //    {
            //        return;
            //    }


            //if (xml == "")
            //{
            //    return;
            //}

            //this.lblOdometer.Text = "";
            //strrXML = new StringReader(xml);
            //ds.ReadXml(strrXML);
            //string CustomProp = ds.Tables[0].Rows[0]["CustomProp"].ToString();
            //this.lblOdometer.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyOdometerValue, CustomProp).TrimEnd();
            //this.tblSetOdometer.Visible = true;
            //this.txtSetOdometer.Visible = false;
        }

        protected void cboTARPeriod_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if ((sn.Cmd.DsProtocolTypes.Tables[0].Rows.Count > 1) && (cboTARPeriod.SelectedItem.Value == "24"))
            {
                EnableDualComm(true);
            }
            else
            {
                EnableDualComm(false);
            }
        }

        protected void cboTracePeriodSetup_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cboTracePeriodSetup.SelectedItem.Value == "0")
            {
                this.cboTraceIntervalSetup.Enabled = false;
                this.cboTraceIntervalSetup.SelectedIndex = -1;
                cboTraceIntervalSetup.SelectedItem.Value = "0";
            }
            else
                this.cboTraceIntervalSetup.Enabled = true;

        }

        protected void cboTracePeriod_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cboTracePeriod.SelectedItem.Value == "0")
            {
                this.cboTraceInterval.Enabled = false;
                this.cboTraceInterval.SelectedIndex = -1;
                cboTraceInterval.SelectedItem.Value = "0";
            }
            else
                this.cboTraceInterval.Enabled = true;
        }

        protected void cboSchPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSchPeriod.SelectedItem.Value == "0")
                this.cboSchInterval.SelectedIndex = 0;

        }

        protected void cboSchInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSchInterval.SelectedItem.Value == "0")
                this.cboSchPeriod.SelectedIndex = 0;
        }

        protected void optCommMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboSchInterval.SelectedIndex = 0;
            cboSchPeriod.SelectedIndex = 0;

            this.chkScheduleTask.Checked = false;
            this.cboSchInterval.Enabled = false;
            this.cboSchPeriod.Enabled = false;
            this.cboSchPeriod.Enabled = false;
            this.chkSendToFleet.Enabled = false;
            this.chkSendToFleet.Checked = false;
            sn.Cmd.DualComm = false;

            if ((this.optCommMode.Items[0].Selected) || (this.optCommMode.Items[1].Selected))
                this.chkScheduleTask.Enabled = true;
            else
                this.chkScheduleTask.Enabled = false;
        }

        private void EnableDualComm(bool flg)
        {
            this.optCommMode.SelectedIndex = 0;
            this.optCommMode.Items[1].Enabled = flg;
            this.optCommMode.Items[2].Enabled = flg;
            this.optCommMode.Items[0].Selected = true;

            sn.Cmd.DualComm = false;
            cboSchInterval.SelectedIndex = 0;
            cboSchPeriod.SelectedIndex = 0;
            this.chkScheduleTask.Enabled = true;
            this.cboSchInterval.Enabled = false;
            this.cboSchPeriod.Enabled = false;
            this.chkSendToFleet.Enabled = false;
            this.chkSendToFleet.Checked = false;
        }

        protected void chkScheduleTask_CheckedChanged(object sender, EventArgs e)
        {
            this.chkSendToFleet.Enabled = chkScheduleTask.Checked;
            this.cboSchInterval.Enabled = chkScheduleTask.Checked;
            this.cboSchPeriod.Enabled = chkScheduleTask.Checked;
            cboSchInterval.SelectedIndex = 0;
            cboSchPeriod.SelectedIndex = 0;
            this.chkSendToFleet.Checked = false;
            chkSendToFleet.ForeColor = Color.Black;
            if (Convert.ToInt32(this.cboCommand.SelectedItem.Value) == 59)
            {
                    chkSendToFleet.Enabled = false;
                    chkSendToFleet.ForeColor = Color.Gray;

            }
        }

        private void FleetScheduler(Int16 cmdType, string paramList)
        {
            DataSet dsVehicle;
            dsVehicle = new DataSet();
            LocationMgr.Location dbl = new LocationMgr.Location();


            StringReader strrXML = null;
            string xml = "";

            Int64 TaskId = 0;
            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
            bool MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
            if (MutipleUserHierarchyAssignment)
            {
                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByMultipleFleetIds(sn.UserID, sn.SecId, sn.Map.SelectedMultiFleetIDs, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByMultipleFleetIds(sn.UserID, sn.SecId, sn.Map.SelectedMultiFleetIDs, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "GetVehiclesLastKnownPositionInfo.User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        return;
                    }
            }
            else
            {
                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, sn.Map.SelectedFleetID, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, sn.Map.SelectedFleetID, ref xml), true))
                    {
                        return;
                    }
            }
            if (xml == "")
            {
                return;
            }

            strrXML = new StringReader(xml);
            dsVehicle.ReadXml(strrXML);

            string strBoxFailed = "";
            foreach (DataRow dr in dsVehicle.Tables[0].Rows)
            {
                if ((Convert.ToBoolean(optCommMode.Items[0].Selected)) && (ViewState["ChPriority"].ToString() != "1"))
                {
                    if (objUtil.ErrCheck(dbl.SendAutomaticCommandWithoutCommParams(sn.UserID, sn.SecId, Convert.ToInt32(dr["BoxId"]), cmdType, DateTime.Now, paramList, sn.Cmd.SchPeriod, sn.Cmd.SchInterval, false, ref TaskId), false))
                        if (objUtil.ErrCheck(dbl.SendAutomaticCommandWithoutCommParams(sn.UserID, sn.SecId, Convert.ToInt32(dr["BoxId"]), cmdType, DateTime.Now, paramList, sn.Cmd.SchPeriod, sn.Cmd.SchInterval, false, ref TaskId), true))
                        {
                            strBoxFailed += dr["BoxId"].ToString() + ";";
                        }
                }
                else if ((Convert.ToBoolean(optCommMode.Items[1].Selected)) || (ViewState["ChPriority"].ToString() == "1"))
                {
                    if (objUtil.ErrCheck(dbl.SendAutomaticCommandWithoutCommParamsOnSecondaryMode(sn.UserID, sn.SecId, Convert.ToInt32(dr["BoxId"]), cmdType, DateTime.Now, paramList, sn.Cmd.SchPeriod, sn.Cmd.SchInterval, false, ref TaskId), false))
                        if (objUtil.ErrCheck(dbl.SendAutomaticCommandWithoutCommParamsOnSecondaryMode(sn.UserID, sn.SecId, Convert.ToInt32(dr["BoxId"]), cmdType, DateTime.Now, paramList, sn.Cmd.SchPeriod, sn.Cmd.SchInterval, false, ref TaskId), true))
                        {

                            strBoxFailed += dr["BoxId"].ToString() + ";";
                        }
                }

                if (cmdType == (Int16)VLF.CLS.Def.Enums.CommandType.AddGeoZone)
                {
                    AssignSelectedGeozoneToVehicle(Convert.ToInt64(dr["VehicleId"]));
                }
            }

            if (strBoxFailed != "")
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("msgFleetScheduledCommandsFailed") + " for boxes : " + strBoxFailed;
                return;
            }

            this.lblMessage.Visible = true;
            this.lblMessage.Text = (string)base.GetLocalResourceObject("msgFleetScheduledCommandsSent");
        }

        protected void cmdReefer_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmReefer.aspx?LicensePlate=" + this.LicensePlate);
        }

        private void SyncGeoZoneforFleet()
        {


            DataSet dsVehicle = new DataSet();
            StringReader strrXML = null;
            string xml = "";
            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

            if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, sn.Map.SelectedFleetID, ref xml), false))
                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, sn.Map.SelectedFleetID, ref xml), true))
                {
                    return;
                }
            if (xml == "")
            {
                return;
            }

            strrXML = new StringReader(xml);
            dsVehicle.ReadXml(strrXML);

            string strBoxFailed = "";

            foreach (DataRow dr in dsVehicle.Tables[0].Rows)
            {
                if (!SyncGeoZoneforVehicle(Convert.ToInt32(dr["VehicleId"]), Convert.ToInt32(dr["BoxId"])))
                    strBoxFailed += dr["BoxId"].ToString() + ";";
            }

            if (strBoxFailed != "")
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("msgFleetScheduledCommandsFailed") + " for boxes : " + strBoxFailed;
                return;
            }

            this.lblMessage.Visible = true;
            this.lblMessage.Text = (string)base.GetLocalResourceObject("msgFleetScheduledCommandsSent");


        }


        private bool SyncGeoZoneforVehicle(Int32 VehicleId, Int32 BoxId)
        {

            StringReader strrXML = null;
            string xml = "";
            Int64 TaskId = 0;
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
            LocationMgr.Location dbl = new LocationMgr.Location();

            DataSet dsGeoZones = new DataSet();
            string paramList = "";
            Int16 cmdType = Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.AddGeoZone);

            if (objUtil.ErrCheck(dbv.GetAllAssignedToVehicleGeozonesInfo(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                if (objUtil.ErrCheck(dbv.GetAllAssignedToVehicleGeozonesInfo(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                {
                    return false;
                }

            if (xml == "")
                return false;

            strrXML = new StringReader(xml);
            dsGeoZones.ReadXml(strrXML);

            foreach (DataRow dr in dsGeoZones.Tables[0].Rows)
            {
                paramList = "";
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyGeozoneId.ToString(), dr["GeozoneId"].ToString());
                paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyGeozoneSeverity.ToString(), dr["SeverityId"].ToString());


                if (objUtil.ErrCheck(dbl.SendAutomaticCommandWithoutCommParams(sn.UserID, sn.SecId, BoxId, cmdType, DateTime.Now, paramList, sn.Cmd.SchPeriod, sn.Cmd.SchInterval, false, ref TaskId), false))
                    if (objUtil.ErrCheck(dbl.SendAutomaticCommandWithoutCommParams(sn.UserID, sn.SecId, BoxId, cmdType, DateTime.Now, paramList, sn.Cmd.SchPeriod, sn.Cmd.SchInterval, false, ref TaskId), true))
                    {
                        return false;
                    }
            }


            return true;

        }


        private void CboUnAssGeoZone_Fill()
        {
            try
            {

                DataSet ds = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetAllUnassignedToVehicleGeozonesInfo(sn.UserID, sn.SecId, Convert.ToInt64(lblVehicleId.Text), ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetAllUnassignedToVehicleGeozonesInfo(sn.UserID, sn.SecId, Convert.ToInt64(lblVehicleId.Text), ref xml), true))
                    {
                        this.cboUnAssignedGeoZones.DataSource = null;
                        this.cboUnAssignedGeoZones.DataBind();
                        return;
                    }

                if (xml == "")
                {
                    this.cboUnAssignedGeoZones.DataSource = null;
                    this.cboUnAssignedGeoZones.DataBind();
                    return;
                }



                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);
                this.cboUnAssignedGeoZones.DataSource = ds;
                this.cboUnAssignedGeoZones.DataBind();
                sn.GeoZone.DsUnAssVehicleGeoZone = ds;
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
        private void CboAssGeoZone_Fill()
        {
            try
            {

                DataSet ds = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetAllAssignedToVehicleGeozonesInfo(sn.UserID, sn.SecId, Convert.ToInt64(lblVehicleId.Text), ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetAllAssignedToVehicleGeozonesInfo(sn.UserID, sn.SecId, Convert.ToInt64(lblVehicleId.Text), ref xml), true))
                    {
                        this.cboAssGeoZones.DataSource = null;
                        this.cboAssGeoZones.DataBind();
                        cboAssGeoZones.Items.Insert(0, new ListItem("Select a Geozone", "-1"));
                        return;
                    }

                if (xml == "")
                {
                    this.cboAssGeoZones.DataSource = null;
                    this.cboAssGeoZones.DataBind();
                    cboAssGeoZones.Items.Insert(0, new ListItem("Select a Geozone", "-1"));
                    return;
                }



                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);
                this.cboAssGeoZones.DataSource = ds;
                this.cboAssGeoZones.DataBind();
                sn.GeoZone.DsVehicleGeoZone = ds;
                cboAssGeoZones.Items.Insert(0, new ListItem("Select a Geozone", "-1"));
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

        protected void cboFuelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboFuelType.SelectedItem.Value.ToString() == "4")
            {
                this.tblDiesel.Visible = true;
                this.tblFuel.Visible = false;
            }
            else
            {
                this.tblFuel.Visible = true;
                this.tblDiesel.Visible = false;
            }
        }

        private string GenerateFuelEEPROMData()
        {

            string customProperties = null;
            byte[] Message = new byte[13];
            Message[0] = 0xAB;
            Message[1] = 0xCD;
            System.BitConverter.GetBytes(Convert.ToUInt32(this.txtDenominator.Text)).CopyTo(Message, 2);
            System.BitConverter.GetBytes(Convert.ToUInt16(this.txtDisplacement.Text)).CopyTo(Message, 6);
            System.BitConverter.GetBytes(Convert.ToUInt16(this.txtVolumeEfficency.Text)).CopyTo(Message, 8);
            System.BitConverter.GetBytes(Convert.ToByte(this.cboFuelType.SelectedItem.Value.ToString())).CopyTo(Message, 10);
            System.BitConverter.GetBytes(Convert.ToByte(this.txtAirFuelRatio.Text)).CopyTo(Message, 11);
            customProperties = VLF.CLS.Util.ByteArrayAsHexDumpToString(Message);
            return customProperties.Substring(1, customProperties.Length - 6);

        }
        protected void cmdSettings_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleAttributes.aspx?VehicleId=" + lblVehicleId.Text);
        }
        protected void cmdServices_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleExtraServices.aspx?VehicleId=" + lblVehicleId.Text);
        }
        protected void cmdUnitInfo_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleExtraInfo.aspx?VehicleId=" + lblVehicleId.Text);
        }


        public static byte DataChecksum(byte[] arrSource)
        {
            byte checksum = 0;
            if (null != arrSource)
            {
                for (int i = 0; i < arrSource.Length; i++)
                    checksum ^= arrSource[i];
                if (checksum == '<' || checksum == '>' || checksum == 0)
                    checksum = 0x40;
            }
            return checksum;
        }

        public static byte GetDataCheckSum(string hexEEPROMOffsetInitial, byte length, string strBuzzerOutput, byte[] eepromIntialBytes)
        {
            byte[] dataSource = new byte[5];
            int intBuzzerOutput = Convert.ToInt32(strBuzzerOutput);
            string hexString2 = hexEEPROMOffsetInitial.Substring(2);

            //fake: var hexEEPROMOffset = hexEEPROMOffsetInitial + buzzerOutput -1
            int buzzerOffset = Int32.Parse(hexString2, System.Globalization.NumberStyles.HexNumber) + intBuzzerOutput - 1;

            byte[] offsetBytes = BitConverter.GetBytes(buzzerOffset);

            dataSource[0] = offsetBytes[0];
            dataSource[1] = offsetBytes[1];
            dataSource[2] = length;

            for (int i = 0; i < eepromIntialBytes.Length; i++)
            {
                dataSource[3 + i] = eepromIntialBytes[i];
            }

            return DataChecksum(dataSource);
        }

        protected void ClearEEPROMFields()
        {
            this.txtEEPROMOffset.Text = "";
            this.txtEEPROMLength.Text = "";
            this.txtEEPROMData.Text = "";
        }

        protected clsEEPROMFeature InitialEEPROMFileds(int feature)
        {
            clsEEPROMFeature eepromfeature = LoadEEPROMFeature(feature);

            this.txtEEPROMOffset.Text = eepromfeature.Offset;
            this.txtEEPROMLength.Text = eepromfeature.Datalength.ToString();
            this.txtEEPROMData.Text = eepromfeature.DataText;

            return eepromfeature;
        }

        protected clsEEPROMFeature LoadEEPROMFeature(int feature)
        {
            clsEEPROMFeature efeature = BoxProfileHelper.GetEEPROMFeaturById(feature);
            efeature.OutputIndex = 2;           //refresh later by input box.
            efeature.RefreshDataLength(this.boxModel);

            efeature.ProcessBuzzer();
            return efeature;
        }

        protected void cboEEPromFeature_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsEEPROMFeature eepromChosed = InitialEEPROMFileds(Convert.ToInt32(this.cboEEPromFeature.SelectedItem.Value));

            tblRowBuzzerOutput.Visible = false;
            ClearEEPROMFields();

            int buzzerOutput = 1;
            switch (Convert.ToInt32(this.cboEEPromFeature.SelectedItem.Value))
            {
                case 1:
                    #region Enable Buzzer
                    tblRowBuzzerOutput.Visible = true;
                    clsEEPROMFeature eep1 = InitialEEPROMFileds(1);

                    try
                    {
                        buzzerOutput = Int32.Parse(txtBuzzerOutput.Text);
                    }
                    catch (FormatException)
                    {
                        txtBuzzerOutput.Text = "1";
                    }

                    eep1.OutputIndex = buzzerOutput;
                    eep1.ProcessBuzzer();

                    this.txtEEPROMOffset.Text = eep1.Offset;
                    this.txtEEPROMData.Text = eep1.DataText;
                    #endregion Enable Buzzer
                    break;

                case 2:
                    #region Disable Buzzer
                    tblRowBuzzerOutput.Visible = true;
                    clsEEPROMFeature eep2 = InitialEEPROMFileds(2);

                    try
                    {
                        buzzerOutput = Int32.Parse(txtBuzzerOutput.Text);
                    }
                    catch (FormatException)
                    {
                        txtBuzzerOutput.Text = "1";
                    }

                    eep2.OutputIndex = buzzerOutput;
                    eep2.ProcessBuzzer();

                    this.txtEEPROMOffset.Text = eep2.Offset;
                    this.txtEEPROMData.Text = eep2.DataText;
                    #endregion Disable Buzzer
                    break;

                default:
                    tblRowBuzzerOutput.Visible = false;
                    eepromChosed.Populate();

                    this.txtEEPROMOffset.Text = eepromChosed.Offset;
                    this.txtEEPROMLength.Text = eepromChosed.Datalength.ToString();
                    this.txtEEPROMData.Text = eepromChosed.DataText;

                    break;
            }
        }

        protected void txtBuzzerOutput_TextChanged(object sender, EventArgs e)
        {
            int buzzerOutput = 1;
            try
            {
                buzzerOutput = Int32.Parse(txtBuzzerOutput.Text);
            }
            catch (FormatException)
            {
                txtBuzzerOutput.Text = "1";
            }

            if (buzzerOutput < 1)
            {
                buzzerOutput = 1;
                txtBuzzerOutput.BackColor = System.Drawing.Color.OrangeRed;
                txtBuzzerOutput.ForeColor = System.Drawing.Color.Yellow;
            }

            txtBuzzerOutput.Text = "" + buzzerOutput;

            int feture = Convert.ToInt32(this.cboEEPromFeature.SelectedItem.Value);
            clsEEPROMFeature eep = InitialEEPROMFileds(feture);
            eep.OutputIndex = buzzerOutput;
            eep.ProcessBuzzer();

            this.txtEEPROMOffset.Text = eep.Offset;
            this.txtEEPROMData.Text = eep.DataText;
        }

        private void CboEEPROMFeature_Fill(int boxModel, int boxId, int userId)
        {
            List<clsEEPROMFeature> featurList = BoxProfileHelper.GetEEPROMCommandList(boxModel, boxId, userId);
            cboEEPromFeature.Items.Clear();
            cboEEPromFeature.Items.Insert(0, new ListItem("--select--", "0"));

            int itemIndex = 0;
            foreach (clsEEPROMFeature ee in featurList)
            {
                itemIndex++;
                cboEEPromFeature.Items.Insert(itemIndex, new ListItem(ee.FeatureName, ee.FeatureId.ToString()));
            }
        }

        private void selectWiFiUpgradeFileType_Fill(int boxModel, int boxId, int userId)
        {
            txtCommandBoxId.Text = boxId.ToString();
            BoxProfile bp = new BoxProfile(sConnectionString);
            List<WiFiUpdateCommand> wifiCommandList = bp.GetWiFiUpdateCommandList(boxModel);

            selectWiFiUpgradeFileType.Items.Clear();
            selectWiFiUpgradeFileType.Items.Insert(0, new ListItem("--select--", "0"));

            int itemIndex = 0;
            wifiCmdDictionary.Clear();
            foreach (WiFiUpdateCommand wifiCmd in wifiCommandList)
            {
                itemIndex++;
                selectWiFiUpgradeFileType.Items.Insert(itemIndex, new ListItem(wifiCmd.WiFiCommandName, wifiCmd.WiFiCommandCode.ToString()));
                wifiCmdDictionary.Add(wifiCmd.WiFiCommandCode.ToString(), wifiCmd.WiFiCommandDefault);
            }

            bool isWiFiBox = bp.IsLinuxWiFiSupportBox(boxId);

            if (isWiFiBox)
            {
                lblLinuxWiFiBoxModel.Text = @"Wi-Fi Support Box";
                Random rnd = new Random();
                lblLinuxWiFiBoxModel.ForeColor = Color.FromArgb(rnd.Next(0), rnd.Next(137), rnd.Next(10));  // System.Drawing.Color.LightGreen; 
                lblLinuxWiFiBoxModel.Font.Bold = true;
                lblWiFiUpgradeOutput.Visible = false;
            }
            else
            {
                lblLinuxWiFiBoxModel.Text = @"Not a Wi-Fi Support Box, command will NOT be accepted.";
            }

        }

        protected void selectWiFiUpgradeFileType_SelectedIndexChanged1(object sender, EventArgs e)
        {
            string wifiCommandCode = this.selectWiFiUpgradeFileType.SelectedItem.Value;

            if (Convert.ToInt32(wifiCommandCode) > 0)
            {
                //List<clsWiFiUpdateCommand> wifiCommandList = BoxProfileHelper.GetWiFiUpdateCommandList(boxModel);
                BoxProfile bp = new BoxProfile(sConnectionString);
                List<WiFiUpdateCommand> wifiCommandList = bp.GetWiFiUpdateCommandList(boxModel);

                wifiCmdDictionary.Clear();

                foreach (WiFiUpdateCommand wifiCmd in wifiCommandList)
                {
                    wifiCmdDictionary.Add(wifiCmd.WiFiCommandCode.ToString(), wifiCmd.WiFiCommandDefault);
                }

                txtWiFiUpgradeOutput.Text = wifiCmdDictionary[wifiCommandCode];
                if (wifiCommandCode == "10")  //Wi-Fi Setting command code
                {
                    this.lblWiFiUpgradeOutput.Visible = true;
                    lblWiFiUpgradeOutput.Text = "Click to Build Wi-Fi Parameters";
                }
                else
                {
                    this.lblWiFiUpgradeOutput.Visible = false;
                }

            }
            else
            {
                txtWiFiUpgradeOutput.Text = "";
            }

        }


    }
}
