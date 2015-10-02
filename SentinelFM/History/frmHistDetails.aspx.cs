using System;
using System.Collections;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.IO;
using VLF.CLS.Def;
using VLF.CLS;
using System.Globalization;  

namespace SentinelFM.History
{
   /// <summary>
   /// Summary description for frmHistDetails.
   /// </summary>
   public partial class frmHistDetails : HistoryDetailsPage
   {

       string lp = "";
      protected void Page_Load(object sender, System.EventArgs e)
      {
         try
         {
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetExpires(DateTime.Now);
            string dgKey = Request.QueryString["dgKey"];

            if (!Page.IsPostBack)
            {
                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmHistDetailsForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
               //GuiSecurity(this);

                HideAllTables();
                this.dgSensors.Visible = false;
                DataRow[] drArr = null;
                DataRow rowItem = null;
                this.lblDriver.Visible = false;
                this.lblDriverID.Visible = false;
                if (sn.History.ShowTrips)
                    drArr = sn.History.DsHistoryInfo.Tables[1].Select("dgKey='" + dgKey + "'");
                else
                    drArr = sn.History.DsHistoryInfo.Tables[0].Select("dgKey='" + dgKey + "'");

                if (drArr != null && drArr.Length > 0)
                    rowItem = drArr[0];
                else
                    return; 
           
                lblUnit.Text=sn.User.UnitOfMes == 1 ? " (km)" : " (mi)";

                this.lblBoxId.Text = rowItem["BoxId"].ToString();

                try
                {
                    this.lblOriginatedDate.Text = Convert.ToDateTime(rowItem["OriginDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                    this.lblReceivedDate.Text = Convert.ToDateTime(rowItem["DateTimeReceived"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                    lp = rowItem["LicensePlate"].ToString(); 
                }
                catch {}

                this.lblSpeed.Text = SetStrDefault(rowItem["Speed"].ToString(), "0");
                this.lblStreetAddress.Text = SetStrDefault(rowItem["StreetAddress"].ToString(), (string)base.GetLocalResourceObject("Text_NA"));

                if ((rowItem["Acknowledged"].ToString().TrimEnd() == "Yes") && (rowItem["MsgDetails"].ToString().TrimEnd() == "Arm"))
                    this.lblArmed.Text = (string)base.GetLocalResourceObject("Text_True");
                else
                    this.lblArmed.Text = SetStrDefault(rowItem["BoxArmed"].ToString(), (string)base.GetLocalResourceObject("Text_NA"));

                if ((rowItem["Acknowledged"].ToString().TrimEnd() == "Yes") && (rowItem["MsgDetails"].ToString().TrimEnd() == "Disarm"))
                    this.lblArmed.Text = (string)base.GetLocalResourceObject("Text_False");
                else
                    this.lblArmed.Text = SetStrDefault(rowItem["BoxArmed"].ToString(), (string)base.GetLocalResourceObject("Text_NA"));

                string strOdom = SetStrDefault(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyOdometerValue, rowItem["CustomProp"].ToString().TrimEnd()), (string)base.GetLocalResourceObject("Text_NA"));

                //if (clsUtility.IsNumeric(strOdom) && (sn.User.UnitOfMes == 0.6214))
                if (clsUtility.IsNumeric(strOdom) && (sn.User.UnitOfMes != 1.0))
                    this.lblOdometer.Text = Convert.ToString(Math.Round(Convert.ToInt32(strOdom) * sn.User.UnitOfMes, 0));
                else
                    this.lblOdometer.Text = strOdom;

                string strFuelConsumption = SetStrDefault(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyFuelConsumption, rowItem["CustomProp"].ToString().TrimEnd()), (string)base.GetLocalResourceObject("Text_NA"));

                try
                {
                    if (clsUtility.IsNumeric(strFuelConsumption))
                        strFuelConsumption = Convert.ToString(Math.Round((Convert.ToDouble(strFuelConsumption) / 10)*sn.User.VolumeUnits , 2));
                }
                catch {}

                this.lblFuelConsuptionValue.Text = strFuelConsumption+" " ;
                this.lblFuelConsuptionValue.Text +=  sn.User.VolumeUnits == 1 ? "L" : "Gal"; 
                this.lblMILvalue.Text = SetStrDefault(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyMIL, rowItem["CustomProp"].ToString().TrimEnd()), (string)base.GetLocalResourceObject("Text_NA"));
                string ReasonForIpUpdate = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyIPUpdateReason, rowItem["CustomProp"].ToString().TrimEnd());
                
                try
                {
                    if (ReasonForIpUpdate.TrimEnd() != "")
                        this.lblReasonForIPUpdate.Text = Convert.ToString((VLF.CLS.Def.Enums.IPUpdateReason)Convert.ToInt16(ReasonForIpUpdate));
                    else
                        this.lblReasonForIPUpdate.Text = SetStrDefault(ReasonForIpUpdate, (string)base.GetLocalResourceObject("Text_NA"));
                }
                catch
                {
                    this.lblReasonForIPUpdate.Text = SetStrDefault(ReasonForIpUpdate, (string)base.GetLocalResourceObject("Text_NA"));
                }

                try
                {
                    this.lblLatitude.Text = Convert.ToDouble(rowItem["Latitude"]).ToString("##.#####");
                }
                catch
                {
                    this.lblLatitude.Text = rowItem["Latitude"].ToString().TrimEnd();
                }

                try
                { 
                    this.lblLongitude.Text = Convert.ToDouble(rowItem["Longitude"]).ToString("###.######");
                }
                catch
                {
                    this.lblLongitude.Text = rowItem["Longitude"].ToString().TrimEnd();
                }
                this.lblMessage.Text = SetStrDefault(rowItem["MsgDetails"].ToString(), (string)base.GetLocalResourceObject("Text_NA"));
                this.lblMessageType.Text = SetStrDefault(rowItem["BoxMsgInTypeName"].ToString().TrimEnd(), (string)base.GetLocalResourceObject("Text_NA"));

                if (rowItem["Speed"].ToString() != "" && rowItem["Speed"].ToString() != "N/A" && rowItem["Speed"].ToString() != "0" &&
                    rowItem["Heading"].ToString() != "" && rowItem["Heading"].ToString() != "N/A")
                    this.lblHeading.Text = Heading(rowItem["Heading"].ToString().TrimEnd());
                else
                    this.lblHeading.Text = (string)base.GetLocalResourceObject("Text_NA");

                if (rowItem["ValidGps"].ToString().TrimEnd() == "0")
                    this.lblValidGPS.Text = (string)base.GetLocalResourceObject("Text_True");
                else if (rowItem["ValidGps"].ToString().TrimEnd() == "1")
                    this.lblValidGPS.Text = (string)base.GetLocalResourceObject("Text_False");
                else
                {
                    this.lblValidGPS.Text = (string)base.GetLocalResourceObject("Text_NA");
                    this.lblLatitude.Text = (string)base.GetLocalResourceObject("Text_NA");
                    this.lblLongitude.Text = (string)base.GetLocalResourceObject("Text_NA");
                    this.lblHeading.Text = (string)base.GetLocalResourceObject("Text_NA");
                    //this.lblSpeed.Text = "N/A";
                    this.lblStreetAddress.Text = (string)base.GetLocalResourceObject("Text_NA");
                }
                this.lblMainBatteryValue.Text = SetStrDefault(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyMainBattery, rowItem["CustomProp"].ToString().TrimEnd()), (string)base.GetLocalResourceObject("Text_NA"));
                   
                switch ((VLF.CLS.Def.Enums.TxtMsgDirectionType)Convert.ToInt16(rowItem["MsgDirection"]))
                {//mantis 2919
                    case VLF.CLS.Def.Enums.TxtMsgDirectionType.In:
                        if(System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName=="fr")
                            GetInMsgDetails_NewTZ(Convert.ToInt32(rowItem["BoxId"]), Convert.ToDateTime(rowItem["OriginDateTime"]).ToString("dd/MM/yyyy HH:mm:ss.fff"));
                        else
                        GetInMsgDetails_NewTZ(Convert.ToInt32(rowItem["BoxId"]), Convert.ToDateTime(rowItem["OriginDateTime"]).ToString("MM/dd/yyyy HH:mm:ss.fff"));
                        if (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.ThirdPartyPacket))
                           GetThirdPartyValue(rowItem["CustomProp"].ToString());
                        switch ((VLF.CLS.Def.Enums.MessageType)Convert.ToInt16(rowItem["BoxMsgInTypeId"]))
                        {
                            case VLF.CLS.Def.Enums.MessageType.Speed:
                            case VLF.CLS.Def.Enums.MessageType.Status:
                            case VLF.CLS.Def.Enums.MessageType.SleepModeFailure:
                            case VLF.CLS.Def.Enums.MessageType.Coordinate:
                            case VLF.CLS.Def.Enums.MessageType.KeyFobArm:
                            case VLF.CLS.Def.Enums.MessageType.KeyFobDisarm:
                            case VLF.CLS.Def.Enums.MessageType.KeyFobPanic:
                            case VLF.CLS.Def.Enums.MessageType.GeoFence:
                            case VLF.CLS.Def.Enums.MessageType.GPSAntennaShort:
                            case VLF.CLS.Def.Enums.MessageType.GPSAntennaOpen:
                            case VLF.CLS.Def.Enums.MessageType.GPSAntennaOK:
                            case VLF.CLS.Def.Enums.MessageType.StoredPosition:
                            case VLF.CLS.Def.Enums.MessageType.Alarm:
                            case VLF.CLS.Def.Enums.MessageType.Sensor:
                            case VLF.CLS.Def.Enums.MessageType.ServiceRequired:
                            case VLF.CLS.Def.Enums.MessageType.PositionUpdate:
                            case VLF.CLS.Def.Enums.MessageType.Idling:
                            case VLF.CLS.Def.Enums.MessageType.Speeding:
                            case VLF.CLS.Def.Enums.MessageType.HarshAcceleration:
                            case VLF.CLS.Def.Enums.MessageType.HarshBraking:
                            case VLF.CLS.Def.Enums.MessageType.SeatBelt:
                            case VLF.CLS.Def.Enums.MessageType.ExtremeAcceleration:
                            case VLF.CLS.Def.Enums.MessageType.ExtremeBraking:
                            case VLF.CLS.Def.Enums.MessageType.Heartbeat:
                                short ProtocolId = Convert.ToInt16(this.lblProtocolId.Text);
                                if ((ProtocolId != Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.MBv10)) ||
                                   (ProtocolId != Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.MBv20)) ||
                                   (ProtocolId != Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.MBv21)) ||
                                   (ProtocolId != Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.MBv22)) ||
                                   (ProtocolId != Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.MBv30)))
                                {
                                   //DgSensors_Fill(rowItem["SensorMask"].ToString(), Convert.ToInt32(rowItem["BoxId"]));
                                    DgSensors_Fill(rowItem["SensorMask"].ToString(), lp);
                                   this.dgSensors.Visible = true;
                                }
                                break;
                            case VLF.CLS.Def.Enums.MessageType.BoxStatus:
                            case VLF.CLS.Def.Enums.MessageType.SendExtendedStatus:
                                GetBoxStatus(rowItem["CustomProp"].ToString(), rowItem["SensorMask"].ToString(), Convert.ToInt32(rowItem["BoxId"]));
                                this.lblBoxStatusArmed.Text = rowItem["BoxArmed"].ToString();
                                this.dgSensors.Visible = false;
                                this.tblBoxStatusInfo.Visible = true;
                                break;
                            case VLF.CLS.Def.Enums.MessageType.BoxSetup:
                            case VLF.CLS.Def.Enums.MessageType.SendExtendedSetup:
                                GetBoxSetup(rowItem["CustomProp"].ToString(), rowItem["SensorMask"].ToString(), Convert.ToInt32(rowItem["BoxId"]));
                                this.dgSensors.Visible = false;
                                this.tblBoxSetupInfo.Visible = true;
                                break;
                            case VLF.CLS.Def.Enums.MessageType.ControllerStatus:
                                GetControllerStatus(rowItem["CustomProp"].ToString());
                                this.dgSensors.Visible = true;
                                this.tblControllerStatus.Visible = true;
                                break;
                            case VLF.CLS.Def.Enums.MessageType.TARMode:
                                GetTarMode(rowItem["CustomProp"].ToString());
                                this.dgSensors.Visible = true;
                                this.tblTAR.Visible = true;
                                break;
                            case VLF.CLS.Def.Enums.MessageType.VCROffDelay:
                                GetVCRDelay(rowItem["CustomProp"].ToString());
                                this.dgSensors.Visible = true;
                                this.tblVCRDelay.Visible = true;
                                break;
                            case VLF.CLS.Def.Enums.MessageType.BadSensor:
                                GetBadSensor(rowItem["CustomProp"].ToString());
                                this.dgSensors.Visible = true;
                                this.tblBadSensor.Visible = true;
                                break;
                            // todo: reefer messages
                            case Enums.MessageType.ReeferFuelAlarm:
                            case Enums.MessageType.ReeferSensorAlarm:
                            case Enums.MessageType.ReeferStatusReport:
                            case Enums.MessageType.ReeferTemperatureAlarm:
                            case Enums.MessageType.SendReeferSetup:
                                this.tblReefer.Visible = true;
                                ParseReeferData(rowItem["CustomProp"].ToString().Trim(), (Enums.MessageType)Convert.ToInt16(rowItem["BoxMsgInTypeId"]));
                                // todo: display xs sensors condition
                                string wrapperMask = Util.PairFindValue(Const.keyReeferWrapperMask, rowItem["CustomProp"].ToString().Trim());
                                if (!String.IsNullOrEmpty(wrapperMask) && wrapperMask != "00")
                                {
                                    this.dgSensors.Visible = true;
                                    //DgSensors_Fill(rowItem["SensorMask"].ToString(), Convert.ToInt32(rowItem["BoxId"]));
                                    DgSensors_Fill(rowItem["SensorMask"].ToString(), lp);
                                 }
                                 break;
                            case Enums.MessageType.SensorExtended:
                                this.lblMessage.Text += "; " + (string)base.GetLocalResourceObject("SENSOR_DURATION") + ": " + Util.PairFindValue(Const.keySensorDuration, rowItem["CustomProp"].ToString().Trim()) + " (min)";
                                string distance = Util.PairFindValue("SENSOR_DISTANCE", rowItem["CustomProp"].ToString().Trim());
                                    if (distance!="" && distance!="0")
                                        this.lblMessage.Text += "; " + "SENSOR DISTANCE: " + Util.PairFindValue("SENSOR_DISTANCE", rowItem["CustomProp"].ToString().Trim()) + " (m)";

                                    this.lblMessage.Text += "; " + "Sequence#: " + rowItem["SequenceNum"].ToString().Trim();

                                //DgSensors_Fill(rowItem["SensorMask"].ToString(), Convert.ToInt32(rowItem["BoxId"]));
                                    DgSensors_Fill(rowItem["SensorMask"].ToString(), lp);
                                this.dgSensors.Visible = true;
                                
                                break;
                            case Enums.MessageType.SendSMC:
                                //DgSensors_Fill(rowItem["SensorMask"].ToString(), Convert.ToInt32(rowItem["BoxId"]));
                                DgSensors_Fill(rowItem["SensorMask"].ToString(), lp);
                                this.dgSensors.Visible = true;
                                if (sn.User.UserGroupId==1 || sn.User.UserGroupId==18)
                                   GetSMC(rowItem["CustomProp"].ToString().Trim(), Convert.ToInt32(rowItem["BoxId"]), Convert.ToDateTime(rowItem["OriginDateTime"]), Convert.ToInt16(rowItem["BoxMsgInTypeId"]));
                                break;
                            case Enums.MessageType.SendDTC:
                               // DgSensors_Fill(rowItem["SensorMask"].ToString(), Convert.ToInt32(rowItem["BoxId"]));
                                DgSensors_Fill(rowItem["SensorMask"].ToString(), lp);
                                this.dgSensors.Visible = true;
                                GetDTCValue (rowItem["CustomProp"].ToString().Trim());
                                break;
                            case Enums.MessageType.DeviceTestResult:
                                this.lblMessage.Text   = Util.PairFindValue(Const.keyTestParam, rowItem["CustomProp"].ToString().Trim());
                                this.dgSensors.Visible = true;
                                break;
                            case Enums.MessageType.TetheredState :
                                this.dgSensors.Visible = true;
                                GetTetheredStateValue(rowItem["CustomProp"].ToString().Trim());
                                break;
                            case Enums.MessageType.SendEEPROMData:
                                GetEEPROMValue(rowItem["CustomProp"].ToString().Trim());
                                break;
                            case Enums.MessageType.VirtualLandmark :
                                 try
                                 {
                                       DataRow[] dr=sn.DsLandMarks.Tables[0].Select("LandmarkId="+Util.PairFindValue(Const.keyGeozoneId, rowItem["CustomProp"].ToString().Trim()));
                                      if (dr!=null && dr.Length>0)
                                      {
                                          this.lblMessage.Text = "Landmark:" + dr[0]["LandmarkName"].ToString() ;
                                          this.lblMessage.Text +=","+ Util.PairFindValue(Const.keyGeozoneDir , rowItem["CustomProp"].ToString().Trim());
                                      }
                                 }
                                catch 
                                 {
                                 }
                                
                                break; 
                            }
                            break;
                        case VLF.CLS.Def.Enums.TxtMsgDirectionType.Out:
                            GetOutMsgDetails_NewTZ(Convert.ToInt32(rowItem["BoxId"]), Convert.ToDateTime(rowItem["OriginDateTime"]).ToString("MM/dd/yyyy HH:mm:ss.fff"));
                           if (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.ThirdPartyPacket))
                              GetThirdPartyValue(rowItem["CustomProp"].ToString());
                           switch ((VLF.CLS.Def.Enums.CommandType)Convert.ToInt16(rowItem["BoxMsgInTypeId"]))
                           {
                              case VLF.CLS.Def.Enums.CommandType.Output:
                                 short ProtocolId = Convert.ToInt16(this.lblProtocolId.Text);
                                 if ((ProtocolId != Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.MBv10)) ||
                                     (ProtocolId != Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.MBv20)) ||
                                     (ProtocolId != Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.MBv21)) ||
                                     (ProtocolId != Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.MBv22)) ||
                                     (ProtocolId != Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.MBv30)))
                                 {
                                    DgSensors_Fill(rowItem["SensorMask"].ToString(), lp);
                                    this.dgSensors.Visible = true;
                                 }
                                 break;
                           }
                           break;
                     }
                this.lblDriverID.Text = Util.PairFindValue("DrID", rowItem["CustomProp"].ToString().Trim());
                if (string.IsNullOrEmpty(this.lblDriverID.Text))
                {
                    this.lblDriverID.Text = Util.PairFindValue("DriverID", rowItem["CustomProp"].ToString().Trim());
                }
                if (!string.IsNullOrEmpty(this.lblDriverID.Text))
                {
                    if (this.lblProtocolId.Visible != true)
                    {
                        this.lblProtocolId.Text = "DriverID:" + this.lblDriverID.Text;
                        this.lblProtocolId.Visible = true;
                        //this.lblDriver.Visible = true;
                        //this.lblDriverID.Visible = true;
                    }
                    else
                    {
                        this.lblDriver.Visible = true;
                        this.lblDriverID.Visible = true;
                    }
                }
                }
          }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      /// <summary>
      /// Get Reefer History Data
      /// </summary>
      /// <param name="customProp"></param>
      private void ParseReeferData(string customProp, Enums.MessageType msgType)
      {
         bool rptBound = false;
         string[] keyvaluepairs = customProp.Split(';');
         if (keyvaluepairs == null || keyvaluepairs.Length == 0) return;
         DataTable dtReefer = new DataTable("Reefer");
         if (msgType == Enums.MessageType.SendReeferSetup)
         {
            dtReefer.Columns.Add("Header");
            dtReefer.Columns.Add("Active");
            dtReefer.Columns.Add("Value1");
            dtReefer.Columns.Add("Value2");
            dtReefer.Columns.Add("Value3");
         }
         else
         {
            dtReefer.Columns.Add("Header");
            dtReefer.Columns.Add("Value");
         }
         # region Table filling
         switch (msgType)
         {
            case Enums.MessageType.ReeferFuelAlarm:
               this.lblReeferCaption.Text = "Fuel Thresholds Alarm";
               this.fldCommonSensors.Visible = true;
               GetReeferSensorStatus(customProp, ref dtReefer);
               GetReeferFuelSensorData(customProp, ref dtReefer);
               GetReeferTemperatureZones(customProp, ref dtReefer);
               GetReeferStatusSensors(customProp, ref dtReefer);
               break;
            case Enums.MessageType.ReeferFuelLevelRiseDropAlarm:
               this.lblReeferCaption.Text = "Fuel Level Alarm";
               this.fldCommonSensors.Visible = true;
               GetReeferSensorStatus(customProp, ref dtReefer);
               GetReeferFuelSensorData(customProp, ref dtReefer);
               GetReeferTemperatureZones(customProp, ref dtReefer);
               GetReeferStatusSensors(customProp, ref dtReefer);
               break;
            case Enums.MessageType.ReeferSensorAlarm:
               this.lblReeferCaption.Text = "Sensor Alarm";
               this.fldCommonSensors.Visible = true;
               GetReeferSensorNumber(customProp, ref dtReefer);
               GetReeferSensorStatus(customProp, ref dtReefer);
               GetReeferTemperatureZones(customProp, ref dtReefer);
               GetReeferFuelStatus(customProp, ref dtReefer);
               GetReeferStatusSensors(customProp, ref dtReefer);
               break;
            case Enums.MessageType.ReeferStatusReport:
               this.lblReeferCaption.Text = "Reefer Status Report";
               this.fldCommonSensors.Visible = true;
               GetReeferTemperatureZones(customProp, ref dtReefer);
               GetReeferFuelStatus(customProp, ref dtReefer);
               GetReeferStatusSensors(customProp, ref dtReefer);
               break;
            case Enums.MessageType.ReeferTemperatureAlarm:
               this.lblReeferCaption.Text = "Temperature Alarm";
               this.fldCommonSensors.Visible = true;
               GetReeferSensorStatus(customProp, ref dtReefer);
               GetReeferTemperatureSensorData(customProp, ref dtReefer);
               GetReeferFuelStatus(customProp, ref dtReefer);
               GetReeferStatusSensors(customProp, ref dtReefer);
               break;
            case Enums.MessageType.SendReeferSetup:
               this.lblSetup.Text = "Reefer Setup";
               GetReeferTemperatureTresholds(customProp);
               GetReeferFuelTresholds(customProp);
               GetReeferStatusSensorsFull(customProp);
               GetReporting(customProp);
               rptBound = true;
               break;
            default:
               this.lblReeferCaption.Text = "Undefined Reefer Message";
               break;
         }
         # endregion
         if (!rptBound)
         {
            this.rptCommonSensors.DataSource = dtReefer;
            this.rptCommonSensors.DataBind();
         }
      }

      /// <summary>
      /// Get status sensors for reefer setup
      /// </summary>
      /// <param name="customProp"></param>
      private void GetReeferStatusSensorsFull(string customProp)
      {
         string sensorMask = "", value = "", off = "OFF", on = "ON", activeMask = "", active = "", yes = "YES", no = "NO";
         byte mask = 0, actMask = 0;
         sensorMask = Util.PairFindValue(Const.keyReeferSensorsStatus, customProp);
         activeMask = Util.PairFindValue(Const.keyReeferSensorsEnableMask, customProp);
         if (!String.IsNullOrEmpty(sensorMask))
            mask = Convert.ToByte(sensorMask.Split(' ')[0], 16);
         if (!String.IsNullOrEmpty(activeMask))
            actMask = Convert.ToByte(activeMask.Split(' ')[0], 16);

         DataTable dtStatus = new DataTable("StatusSensors");
         dtStatus.Columns.Add("Header");
         dtStatus.Columns.Add("Active");
         dtStatus.Columns.Add("Value");

         for (byte sns = 0; sns < Const.STATUS_SENSORS.Length; sns++)
         {
            value = Util.GetBit(mask, sns) == 0 ? off : on;
            active = Util.GetBit(actMask, sns) == 0 ? no : yes;
            dtStatus.Rows.Add(Const.STATUS_SENSORS[sns], active, value);
         }

         this.lblStatus.Text = "Status Sensors";
         this.fldStatus.Visible = true;
         this.rptStatusSensors.DataSource = dtStatus;
         this.rptStatusSensors.DataBind();
      }

      /// <summary>
      /// Get fuel sensor data for reefer setup
      /// </summary>
      /// <param name="customProp"></param>
      /// <param name="dtSensors"></param>
      private void GetReeferFuelTresholds(string customProp)
      {
         string currentValue = "", lowerValue = "", upperValue = "", percent = "%", sensorMask = "", active = "";
         byte mask = 0;
         sensorMask = Util.PairFindValue(Const.keyReeferFuelEnableMask, customProp);
         if (!String.IsNullOrEmpty(sensorMask))
         {
            mask = Convert.ToByte(sensorMask, 16);
         }

         DataTable dtFuel = new DataTable("Fuel");
         dtFuel.Columns.Add("Active");
         dtFuel.Columns.Add("Lower");
         dtFuel.Columns.Add("Current");
         dtFuel.Columns.Add("Upper");

         active = Util.GetBit(mask, (byte)0) == 1 ? "YES" : "NO";

         currentValue = Util.PairFindValue(Const.keyReeferFuelStatus, customProp);
         if (!String.IsNullOrEmpty(currentValue))
            currentValue = GetNumericValue(currentValue, percent);

         lowerValue = Util.PairFindValue(Const.keyReeferLowerThresholdOfFuel, customProp);
         if (!String.IsNullOrEmpty(lowerValue))
            lowerValue = GetNumericValue(lowerValue, percent);

         upperValue = Util.PairFindValue(Const.keyReeferUpperThresholdOfFuel, customProp);
         if (!String.IsNullOrEmpty(upperValue))
            upperValue = GetNumericValue(upperValue, percent);

         dtFuel.Rows.Add(active, lowerValue, currentValue, upperValue);
         this.lblFuel.Text = "Fuel Threshold (%)";
         this.fldFuel.Visible = true;
         this.rptFuel.DataSource = dtFuel;
         this.rptFuel.DataBind();
         //dtSensors.Rows.Add("<hr />", "<hr />", "<hr />", "<hr />", "<hr />");
      }
/*
      /// <summary>
      /// Get future mask - incl. position for reefer setup
      /// </summary>
      /// <param name="customProp"></param>
      private void GetFeatureMask(string customProp)
      {
         string value = "", header = "Include Position", status = "YES";
         value = Util.PairFindValue(Const.keyReeferFeatureMask, customProp);
         if (!String.IsNullOrEmpty(value) && value.StartsWith("01"))
         {
            this.divSensors.Visible = true;
            this.divSensors.InnerHtml += 
               String.Format("<hr /><table><tr><td>{0}</td><td>{1}</td></tr></table>", 
                  header, status);

            //dtSensors.Rows.Add("Include Position", "Yes");
            //dtSensors.Rows.Add("<hr />", "<hr />");
         }
      }
*/
      /// <summary>
      /// Get temp. sensor for temp. alarm
      /// </summary>
      /// <param name="customProp"></param>
      /// <param name="dtSensors"></param>
      private void GetReeferTemperatureSensorData(string customProp, ref DataTable dtSensors)
      {
         string value = "", degree = "&deg;C";

         value = Util.PairFindValue(Const.keyReeferTempZoneNumber, customProp);
         if (!String.IsNullOrEmpty(value))
            dtSensors.Rows.Add("Temperature Zone No.", GetNumericValue(value, ""));

         value = Util.PairFindValue(Const.keyReeferTempZoneAlarmType, customProp);
         if (!String.IsNullOrEmpty(value))
            dtSensors.Rows.Add("Temperature Alarm Type", GetRangeValue(value));

         value = Util.PairFindValue(Const.keyReeferTempOfZone, customProp);
         if (!String.IsNullOrEmpty(value))
            dtSensors.Rows.Add("Current Temperature", GetNumericValue(value, degree));

         value = Util.PairFindValue(Const.keyReeferLowerThresholdOfTempZone, customProp);
         if (!String.IsNullOrEmpty(value))
            dtSensors.Rows.Add("Lower Threshold", GetNumericValue(value, degree));

         value = Util.PairFindValue(Const.keyReeferUpperThresholdOfTempZone, customProp);
         if (!String.IsNullOrEmpty(value))
            dtSensors.Rows.Add("Upper Threshold", GetNumericValue(value, degree));

         dtSensors.Rows.Add("<hr />", "<hr />");      
      }

      /// <summary>
      /// Get fuel sensor for fuel alarm
      /// </summary>
      /// <param name="customProp"></param>
      /// <param name="dtSensors"></param>
      private void GetReeferFuelSensorData(string customProp, ref DataTable dtSensors)
      {
         string value = "", percent = "%", unit = "min";
         int min;

         value = Util.PairFindValue(Const.keyReeferFuelAlarmType, customProp);
         if (!String.IsNullOrEmpty(value))
            dtSensors.Rows.Add("Fuel Alarm Type", GetRangeValue(value));
         else
         {
            value = Util.PairFindValue(Const.keyReeferFuelLevelRiseDropAlarmType, customProp);
            if (!String.IsNullOrEmpty(value))
               dtSensors.Rows.Add("Fuel Alarm Type", GetRangeValue(value));
         }
         value = Util.PairFindValue(Const.keyReeferFuelStatus, customProp);
         if (!String.IsNullOrEmpty(value))
            dtSensors.Rows.Add("Fuel Current Status", GetNumericValue(value, percent));
         value = Util.PairFindValue(Const.keyReeferLowerThresholdOfFuel, customProp);
         if (!String.IsNullOrEmpty(value))
            dtSensors.Rows.Add("Lower Fuel Threshold", GetNumericValue(value, percent));
         value = Util.PairFindValue(Const.keyReeferUpperThresholdOfFuel, customProp);
         if (!String.IsNullOrEmpty(value))
            dtSensors.Rows.Add("Upper Fuel Threshold", GetNumericValue(value, percent));
         value = Util.PairFindValue(Const.keyReeferFuelLevelChange, customProp);
         if (!String.IsNullOrEmpty(value))
            dtSensors.Rows.Add("Rise/Drop Fuel Level Change", GetNumericValue(value, percent));
         value = Util.PairFindValue(Const.keyReeferFuelLevelDurationOfChange, customProp);
         if (!String.IsNullOrEmpty(value))
         {
            if (Int32.TryParse(value, out min))
            {
               dtSensors.Rows.Add("Checking Interval", String.Format("{0} {1}", (int)(min / 60), unit));
            }
         }
         dtSensors.Rows.Add("<hr />", "<hr />");
      }

      /// <summary>
      /// Get status of temp. zones
      /// </summary>
      /// <param name="customProp"></param>
      /// <param name="dtSensors"></param>
      private void GetReeferTemperatureZones(string customProp, ref DataTable dtSensors)
      {
         string value = "";
         for (int zone = 1; zone <= 5; zone++)
         {
            value = Util.PairFindValue(Const.keyReeferTempOfZone + zone.ToString(), customProp);
            if (!String.IsNullOrEmpty(value))
               dtSensors.Rows.Add("Temperature of Zone " + zone.ToString(), GetNumericValue(value, "&deg;C"));
         }
         dtSensors.Rows.Add("<hr />", "<hr />");
      }

      /// <summary>
      /// Get temp. zones for reefer setup
      /// </summary>
      /// <param name="customProp"></param>
      /// <param name="dtSensors"></param>
      private void GetReeferTemperatureTresholds(string customProp)
      {
         string currentValue = "", lowerValue = "", upperValue = "", degree = "&deg;C", active = "", sensorMask = "", yes = "YES", no = "NO";
         byte mask = 0;
         sensorMask = Util.PairFindValue(Const.keyReeferTempZoneEnableMask, customProp);
         if (!String.IsNullOrEmpty(sensorMask))
         {
            mask = Convert.ToByte(sensorMask, 16);
         }

         DataTable dtTemperature = new DataTable("Temperature");
         dtTemperature.Columns.Add("Header");
         dtTemperature.Columns.Add("Active");
         dtTemperature.Columns.Add("Lower");
         dtTemperature.Columns.Add("Current");
         dtTemperature.Columns.Add("Upper");

         for (int zone = 1; zone <= 5; zone++)
         {
            active = Util.GetBit(mask, (byte)(zone - 1)) == 1 ? yes : no;

            currentValue = Util.PairFindValue(Const.keyReeferTempOfZone + zone.ToString(), customProp);
            if (!String.IsNullOrEmpty(currentValue))
               currentValue = GetNumericValue(currentValue, degree);

            lowerValue = Util.PairFindValue(Const.keyReeferLowerThresholdOfTempZone + zone.ToString(), customProp);
            if (!String.IsNullOrEmpty(lowerValue))
               lowerValue = GetNumericValue(lowerValue, degree);

            upperValue = Util.PairFindValue(Const.keyReeferUpperThresholdOfTempZone + zone.ToString(), customProp);
            if (!String.IsNullOrEmpty(upperValue))
               upperValue = GetNumericValue(upperValue, degree);

            dtTemperature.Rows.Add("Temperature Zone " + zone.ToString(), active, lowerValue, currentValue, upperValue);
         }

         this.lblTemperature.Text = String.Format("Temperature Tresholds ({0})", degree);
         this.fldTemperature.Visible = true;
         this.rptTemperature.DataSource = dtTemperature;
         this.rptTemperature.DataBind();
         //dtTemperature.Rows.Add("<hr />", "<hr />", "<hr />", "<hr />", "<hr />");
      }

      /// <summary>
      /// Get status sensors
      /// </summary>
      /// <param name="customProp"></param>
      /// <param name="dtSensors"></param>
      private void GetReeferStatusSensors(string customProp, ref DataTable dtSensors)
      {
         string sensorMask = "", value = "", off = "OFF", on = "ON";
         byte mask = 0;
         sensorMask = Util.PairFindValue(Const.keyReeferSensorsStatus, customProp);
         if (!String.IsNullOrEmpty(sensorMask))
            mask = Convert.ToByte(sensorMask.Split(' ')[0], 16);

         for (byte sns = 0; sns < Const.STATUS_SENSORS.Length; sns++)
         {
            value = Util.GetBit(mask, sns) == 0 ? off : on;
            dtSensors.Rows.Add(Const.STATUS_SENSORS[sns], value);
         }
      }

      /// <summary>
      /// Get status of fuel sensor
      /// </summary>
      /// <param name="customProp"></param>
      /// <param name="dtSensors"></param>
      private void GetReeferFuelStatus(string customProp, ref DataTable dtSensors)
      {
         string value = "";
         value = Util.PairFindValue(Const.keyReeferFuelStatus, customProp);
         if (!String.IsNullOrEmpty(value))
         {
            dtSensors.Rows.Add("Fuel Current Status", value + "%");
            dtSensors.Rows.Add("<hr />", "<hr />");
         }
      }

      /// <summary>
      /// Get sensor number
      /// </summary>
      /// <param name="customProp"></param>
      /// <param name="dtSensors"></param>
      private void GetReeferSensorNumber(string customProp, ref DataTable dtSensors)
      {
         string value = "", result = "", error = "N/A";
         int sensorIndex = 0;
         value = Util.PairFindValue(Const.keyReeferSensorNumber, customProp);
         if (!String.IsNullOrEmpty(value))
         {
            if (int.TryParse(value, out sensorIndex))
            {
               if (sensorIndex > 0 && sensorIndex <= 7)
                  result = Const.STATUS_SENSORS[sensorIndex - 1];
               else
                  result = error;
            }
         }
         else
            result = error;

         dtSensors.Rows.Add("Sensor", result);
         dtSensors.Rows.Add("<hr />", "<hr />");
      }

      /// <summary>
      /// Get sensor status
      /// </summary>
      /// <param name="customProp"></param>
      /// <param name="dtSensors"></param>
      private void GetReeferSensorStatus(string customProp, ref DataTable dtSensors)
      {
         string value = "";
         value = Util.PairFindValue(Const.keyReeferSensorStatus, customProp);
         if (!String.IsNullOrEmpty(value))
         {
            dtSensors.Rows.Add("Sensor Status", GetRangeValue(value));
            dtSensors.Rows.Add("<hr />", "<hr />");
         }
      }

      /// <summary>
      /// Get report. int-l for reefer setup
      /// </summary>
      /// <param name="customProp"></param>
      private void GetReporting(string customProp)
      {
         string value = "", unit = "min", status = "";
         int min;
         DataTable dtReefer = new DataTable("Reporting");
         dtReefer.Columns.Add("Header");
         dtReefer.Columns.Add("Value");

         value = Util.PairFindValue(Const.keyReeferReportingInterval, customProp);
         if (!String.IsNullOrEmpty(value))
         {
            if (Int32.TryParse(value, out min))
            {
               dtReefer.Rows.Add("Reporting Interval", String.Format("{0} {1}", (int)(min / 60), unit));
            }
         }
         value = Util.PairFindValue(Const.keyReeferFeatureMask, customProp);
         if (!String.IsNullOrEmpty(value))
         {
            status = value.StartsWith("01") ? "YES" : "NO";
            dtReefer.Rows.Add("Include Position", status);
         }

         this.lblReeferCaption.Text = "Reporting";

         this.fldCommonSensors.Visible = true;
         this.rptCommonSensors.DataSource = dtReefer;
         this.rptCommonSensors.DataBind();
      }

      private string GetRangeValue(string val)
      {
         string result = "";
         switch (val)
         {
            case Const.valInRange:
               result = "In Range";
               break;
            case Const.valOutOfRange:
               result = "Out of Range";
               break;
            default:
               result = val;
               break;
         }
         return result;
      }

      private string GetNumericValue(string val, string add)
      {
         const string invalid = "Invalid";
         string result = "";
         int number;
         if (Int32.TryParse(val, out number))
         {
            result = val;
            if (!String.IsNullOrEmpty(add))
               result += add;
         }
         else
            result = invalid;
         return result;
      }

      // Changes for TimeZone Feature start
      private void GetInMsgDetails_NewTZ(int BoxId, string MsgDate)
      {
          try
          {
              string xml = "";
              ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();

              if (objUtil.ErrCheck(dbh.GetDetailedMessageInFromHistory_NewTZ(sn.UserID, sn.SecId, BoxId, Convert.ToDateTime(MsgDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), ref xml), false))
                  if (objUtil.ErrCheck(dbh.GetDetailedMessageInFromHistory_NewTZ(sn.UserID, sn.SecId, BoxId, Convert.ToDateTime(MsgDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), ref xml), true))
                  {
                      return;
                  }

              if (xml == "")
              {
                  return;
              }

              StringReader strrXML = null;
              strrXML = new StringReader(xml);
              DataSet ds = new DataSet();
              ds.ReadXml(strrXML);

              if (ds.Tables[0].Rows[0]["FirstName"].ToString().TrimEnd() != "N/A")
                  this.lblUser.Text = ds.Tables[0].Rows[0]["FirstName"].ToString().TrimEnd() + " " + ds.Tables[0].Rows[0]["LastName"].ToString().TrimEnd();
              else
                  this.lblUser.Text = (string)base.GetLocalResourceObject("Text_NA");

              this.lblProtocolType.Text = SetStrDefault(ds.Tables[0].Rows[0]["BoxProtocolTypeName"].ToString(), (string)base.GetLocalResourceObject("Text_NA"));

              this.lblProtocolId.Text = ds.Tables[0].Rows[0]["BoxProtocolTypeId"].ToString();
          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
          }
      }
      // Changes for TimeZone Feature end

      private void GetInMsgDetails(int BoxId, string MsgDate)
      {
          try
          {
              string xml = "";
              ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();

              if (objUtil.ErrCheck(dbh.GetDetailedMessageInFromHistory(sn.UserID, sn.SecId, BoxId, Convert.ToDateTime(MsgDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), ref xml), false))
                  if (objUtil.ErrCheck(dbh.GetDetailedMessageInFromHistory(sn.UserID, sn.SecId, BoxId, Convert.ToDateTime(MsgDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), ref xml), true))
                  {
                      return;
                  }

              if (xml == "")
              {
                  return;
              }

              StringReader strrXML = null;
              strrXML = new StringReader(xml);
              DataSet ds = new DataSet();
              ds.ReadXml(strrXML);

              if (ds.Tables[0].Rows[0]["FirstName"].ToString().TrimEnd() != "N/A")
                  this.lblUser.Text = ds.Tables[0].Rows[0]["FirstName"].ToString().TrimEnd() + " " + ds.Tables[0].Rows[0]["LastName"].ToString().TrimEnd();
              else
                  this.lblUser.Text = (string)base.GetLocalResourceObject("Text_NA");

              this.lblProtocolType.Text = SetStrDefault(ds.Tables[0].Rows[0]["BoxProtocolTypeName"].ToString(), (string)base.GetLocalResourceObject("Text_NA"));

              this.lblProtocolId.Text = ds.Tables[0].Rows[0]["BoxProtocolTypeId"].ToString();
          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
          }
      }

      // Changes for TimeZone Feature start
      private void GetOutMsgDetails_NewTZ(int BoxId, string MsgDate)
      {
          try
          {
              string xml = "";
              ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();

              if (objUtil.ErrCheck(dbh.GetDetailedMessageOutFromHistory_NewTZ(sn.UserID, sn.SecId, BoxId, Convert.ToDateTime(MsgDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), ref xml), false))
                  if (objUtil.ErrCheck(dbh.GetDetailedMessageOutFromHistory_NewTZ(sn.UserID, sn.SecId, BoxId, Convert.ToDateTime(MsgDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), ref xml), true))
                  {
                      return;
                  }

              if (xml == "")
              {
                  return;
              }

              StringReader strrXML = null;
              strrXML = new StringReader(xml);
              DataSet ds = new DataSet();
              ds.ReadXml(strrXML);

              if (ds.Tables[0].Rows[0]["FirstName"].ToString().TrimEnd() != "N/A")
                  this.lblUser.Text = ds.Tables[0].Rows[0]["FirstName"].ToString().TrimEnd() + " " + ds.Tables[0].Rows[0]["LastName"].ToString().TrimEnd();
              else
                  this.lblUser.Text = (string)base.GetLocalResourceObject("Text_NA");

              this.lblProtocolType.Text = SetStrDefault(ds.Tables[0].Rows[0]["BoxProtocolTypeName"].ToString(), (string)base.GetLocalResourceObject("Text_NA"));

              this.lblProtocolId.Text = ds.Tables[0].Rows[0]["BoxProtocolTypeId"].ToString().TrimEnd();
          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
          }
      }
      // Changes for TimeZone Feature end

      private void GetOutMsgDetails(int BoxId, string MsgDate)
      {
          try
          {
              string xml = "";
              ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();

              if (objUtil.ErrCheck(dbh.GetDetailedMessageOutFromHistory(sn.UserID, sn.SecId, BoxId, Convert.ToDateTime(MsgDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), ref xml), false))
                  if (objUtil.ErrCheck(dbh.GetDetailedMessageOutFromHistory(sn.UserID, sn.SecId, BoxId, Convert.ToDateTime(MsgDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), ref xml), true))
                  {
                      return;
                  }

              if (xml == "")
              {
                  return;
              }

              StringReader strrXML = null;
              strrXML = new StringReader(xml);
              DataSet ds = new DataSet();
              ds.ReadXml(strrXML);

              if (ds.Tables[0].Rows[0]["FirstName"].ToString().TrimEnd() != "N/A")
                  this.lblUser.Text = ds.Tables[0].Rows[0]["FirstName"].ToString().TrimEnd() + " " + ds.Tables[0].Rows[0]["LastName"].ToString().TrimEnd();
              else
                  this.lblUser.Text = (string)base.GetLocalResourceObject("Text_NA");

              this.lblProtocolType.Text = SetStrDefault(ds.Tables[0].Rows[0]["BoxProtocolTypeName"].ToString(), (string)base.GetLocalResourceObject("Text_NA"));

              this.lblProtocolId.Text = ds.Tables[0].Rows[0]["BoxProtocolTypeId"].ToString().TrimEnd();
          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
          }
      }

      protected DataTable GetAllSensorsForVehicle(string licensePlate, bool getAllSensors)
      {
          DataSet dsSensors = new DataSet("Sensors");
          DataTable dtSensors = new DataTable("BoxSensors");

          string xml = "";

          ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

          if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, licensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
              if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, licensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
              {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                     VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                     "No Sensors for vehicle: " + licensePlate + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
              }

          if (String.IsNullOrEmpty(xml))
          {
              return null;
          }

          dsSensors.ReadXml(new StringReader(xml));



          dtSensors.Columns.Add("SensorId", typeof(short));
          dtSensors.Columns.Add("SensorName", typeof(string));
          dtSensors.Columns.Add("SensorAction", typeof(string));
          dtSensors.Columns.Add("AlarmLevelOn", typeof(short));
          dtSensors.Columns.Add("AlarmLevelOff", typeof(short));

          if (Util.IsDataSetValid(dsSensors))
          {
              foreach (DataRow rowSensor in dsSensors.Tables[0].Rows)
              {
                  if (!getAllSensors)
                      if (Convert.ToInt16(rowSensor["SensorId"]) > VLF.CLS.Def.Enums.ReeferBase)
                          continue;

                  dtSensors.ImportRow(rowSensor);
              }
          }

          return dtSensors;
      }

      private void DgSensors_Fill(string SensorMask, string LicensePlate)
      {
          try
          {
              string xml = "";
              DataSet dsSensors = new DataSet();

              DataTable dtResult = GetAllSensorsForVehicle(LicensePlate, true);
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

//      private void DgSensors_Fill(string SensorMask, int BoxId)
//      {
//         try
//         {           
//            DataSet dsResult = new DataSet("Sensors");
//            dsResult = base.GetAllSensorsForBox(BoxId);
//            if (!Util.IsDataSetValid(dsResult))
//               return;
//            //DataSet dsSensors = new DataSet();

//            //StringReader strrXML = null;

//            //string xml = "";
//            //ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

//            //if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByBoxId(sn.UserID, sn.SecId, BoxId, ref xml), false))
//            //   if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByBoxId(sn.UserID, sn.SecId, BoxId, ref xml), true))
//            //   {
//            //      return;
//            //   }

//            //if (xml == "")
//            //{
//            //   return;
//            //}

//            //strrXML = new StringReader(xml);
//            //dsResult.ReadXml(strrXML);

//            if (SensorMask != VLF.CLS.Def.Const.blankValue)
//            {
//               //Test for wrong Sensor Mask
//               try
//               {
//                  UInt64 intSensorMask = Convert.ToUInt64(SensorMask);
//               }
//               catch
//               {
////                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "SensorMask: " + SensorMask + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
//                  return;
//               }

//               //if ((dsResult != null) && (dsResult.Tables.Count > 0))
//               //{
//                  DataTable tblSensors = new DataTable("SensorsInformation");
//                     //dsSensors.Tables.Add("SensorsInformation");
//                  tblSensors.Columns.Add("SensorId", typeof(short));
//                  tblSensors.Columns.Add("SensorName", typeof(string));
//                  tblSensors.Columns.Add("SensorAction", typeof(string));

//                  // move over all sensors and set current status
//                  short snsId = 0;
//                  int slashIndex = 0;
//                  object[] objRow = new object[3];
//                  string snsAction = "", fldAction = "";
//                  UInt64 checkBit = 1, shift = 1;
//                  foreach (DataRow ittr in dsResult.Tables[0].Rows)
//                  {
//                     if (!ittr["SensorName"].ToString().TrimEnd().Contains(VLF.CLS.Def.Const.keySensorNotInUse))
//                     {
//                        snsId = Convert.ToInt16(ittr["SensorId"]);

//                        // if not AVL sensor, ignore
//                        if ((snsId & (short)Enums.ReeferBase) > 0)
//                           continue;

//                        //objRow[0] = snsId;
//                        //objRow[1] = ittr["SensorName"].ToString().TrimEnd();

//                        checkBit = shift << (snsId - 1);

//                        fldAction = ittr["SensorAction"].ToString().TrimEnd();
//                        slashIndex = fldAction.IndexOf("/");
//                        if (slashIndex < 1)
//                        {
//                           // wrong sensors format in the database (should be action1/action2)
//                           //continue;
//                           snsAction = "Invalid";
//                        }
//                        else
//                        {
//                           if ((Convert.ToUInt64(SensorMask) & checkBit) == 0)
//                              snsAction = fldAction.Substring(slashIndex + 1).ToString().TrimEnd();
//                           else
//                              snsAction = fldAction.Substring(0, slashIndex).ToString().TrimEnd();
//                        }

//                        tblSensors.Rows.Add(snsId, ittr["SensorName"].ToString().TrimEnd(), snsAction);
//                     }
//                  }
//               //}
//               tblSensors.DefaultView.Sort = "SensorId";
//               //dsSensors.Tables["SensorsInformation"].DefaultView.Sort = "SensorId";
//               this.dgSensors.DataSource = tblSensors;
//               //dsSensors.Tables["SensorsInformation"];
//               this.dgSensors.DataBind();
//            }
//         }
//         catch (Exception Ex)
//         {
//            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
//         }
//      }

      private void GetBoxSetup(string CustomProp, string SensorMask, int BoxId)
      {
         try
         {
            //ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGPSFrequency, CustomProp) == "Disabled")
               this.lblBoxSetupGPSFreg.Text = (string)base.GetLocalResourceObject("Text_Disabled");
            else
               this.lblBoxSetupGPSFreg.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGPSFrequency, CustomProp) + " sec.";

            if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGeoFenceRadius, CustomProp) == "Disabled")
               this.lblBoxSetupGeo.Text = (string)base.GetLocalResourceObject("Text_Disabled");
            else
            {
               if (Convert.ToInt32(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGeoFenceRadius, CustomProp)) == 0)
                  this.lblBoxSetupGeo.Text = (string)base.GetLocalResourceObject("Text_Disabled");
               else
                  this.lblBoxSetupGeo.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGeoFenceRadius, CustomProp).ToString() + " m/" + Convert.ToInt32(Convert.ToInt32(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupGeoFenceRadius, CustomProp)) * 3.28).ToString() + " ft";
            }

            if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupSpeedThreshold, CustomProp) == "Disabled")
               this.lblBoxSetupSpeed.Text = (string)base.GetLocalResourceObject("Text_Disabled");
            else
               this.lblBoxSetupSpeed.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupSpeedThreshold, CustomProp) + " kmh/" + Convert.ToInt32(Convert.ToInt32(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.setupSpeedThreshold, CustomProp)) / 1.6094).ToString() + " mph";

            this.tblBoxSetupInfo.Visible = true;

            //
            DataSet dsResult = new DataSet("Sensors");
            dsResult = base.GetAllSensorsForBox(BoxId);
            //DataSet dsSensors = new DataSet();
            string xml = "";
            //if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByBoxId(sn.UserID, sn.SecId, BoxId, ref xml), false))
            //   if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByBoxId(sn.UserID, sn.SecId, BoxId, ref xml), true))
            //   {
            //      return;
            //   }

            //if (xml == "")
            //{
            //   return;
            //}

            //StringReader strrXML = new StringReader(xml);
            //dsResult.ReadXml(strrXML);

            //DataSet dsVehicleInfo = new DataSet();
            //strrXML = new StringReader(xml);
            //dsVehicleInfo.ReadXml(strrXML);
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
            byte tmpsensorMask = 0;
            try { tmpsensorMask = Convert.ToByte(SensorMask); }
            catch { }
            extSensors <<= 4;
            extSensors |= tmpsensorMask;

            object[] objRow = null;

            // Sensors Info
            if (!Util.IsDataSetValid(dsResult))
               return;

            DataTable tblSensors = new DataTable("SensorsInformation");
            tblSensors.Columns.Add("SensorId", typeof(short));
            tblSensors.Columns.Add("SensorName", typeof(string));
            tblSensors.Columns.Add("SensorAction", typeof(string));
            tblSensors.Columns.Add("SensorStatus", typeof(bool));

            // move over all sensors and set current status
            short snsId = 0;
            int index = 0;

            string snsAction = "";
            UInt64 checkBit = 1;
            foreach (DataRow ittr in dsResult.Tables[0].Rows)
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
                     objRow[2] = "Disabled"; // snsAction.Substring(index + 1);
                     objRow[3] = false;
                  }
                  else
                  {
                     objRow[2] = "Enabled";//snsAction.Substring(0,index);
                     objRow[3] = true;
                  }
                  tblSensors.Rows.Add(objRow);
               }
            }
            this.dgBoxSetupInfo.DataSource = tblSensors;
            this.dgBoxSetupInfo.DataBind();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      private void GetBoxStatus(string CustomProp, string SensorMask, int BoxId)
      {
         try
         {
            //
            this.lblBoxStatusMainBattery.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyMainBattery, CustomProp) + " " + (string)base.GetLocalResourceObject("Text_Volt");
            this.lblBoxStatusBackupBattery.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyBackupBattery, CustomProp) + " " + (string)base.GetLocalResourceObject("Text_Volt");

            this.lblBoxStatusBackupBatteryLabel.Text = (string)base.GetLocalResourceObject("Text_BackupBattery") + ":";
            this.lblBoxStatusBackupBattery.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyBackupBattery, CustomProp) + " " + (string)base.GetLocalResourceObject("Text_Volt");

            this.lblBoxStatusFirmware.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyFirmwareVersion, CustomProp);
            this.lblBoxStatusSN.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keySerialNumber, CustomProp);

            if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyWaypointsUsage, CustomProp) == "")
               this.lblBoxStatusWaypoint.Text = "0";
            else
               this.lblBoxStatusWaypoint.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyWaypointsUsage, CustomProp) ;

            this.lblStatusSIM.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keySIM, CustomProp);
            this.lblStatusCell.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyCell, CustomProp);

            if (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyWaypointsUsage, CustomProp) == "")
               this.lblBoxStatusWaypoint.Text = "0";
            else
               this.lblBoxStatusWaypoint.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyWaypointsUsage, CustomProp) ;

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
            this.lblMDTMessagesValue.Text = SetStrDefault(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyMDTNumMessages, CustomProp), (string)base.GetLocalResourceObject("Text_NA"));

            this.tblBoxStatusInfo.Visible = true;

            DataSet dsResult = new DataSet("Sensors");
            dsResult = base.GetAllSensorsForBox(BoxId);
            //DataSet dsSensors = new DataSet();
            string xml = "";
            //ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            //if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByBoxId(sn.UserID, sn.SecId, BoxId, ref xml), false))
            //   if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByBoxId(sn.UserID, sn.SecId, BoxId, ref xml), true))
            //   {
            //      return;
            //   }

            //if (xml == "")
            //{
            //   return;
            //}

            //StringReader strrXML = null;
            //strrXML = new StringReader(xml);
            //dsResult.ReadXml(strrXML);

            //DataSet dsVehicleInfo = new DataSet();
            //strrXML = new StringReader(xml);
            //dsVehicleInfo.ReadXml(strrXML);

            if (!Util.IsDataSetValid(dsResult))
               return;

            DataTable tblSensors = new DataTable("SensorsInformation");
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
            foreach (DataRow ittr in dsResult.Tables[0].Rows)
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
            tblSensors.DefaultView.Sort = "SensorId";
            //dsSensors.Tables["SensorsInformation"].DefaultView.Sort = "SensorId";
            this.dgBoxStatusInfo.DataSource = tblSensors;
            //dsSensors.Tables["SensorsInformation"];
            this.dgBoxStatusInfo.DataBind();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      private string Heading(string heading)
      {
         return sn.Map.Heading(heading);
      }

      private void HideAllTables()
      {
         this.tblBoxStatusInfo.Visible = false;
         this.tblBoxSetupInfo.Visible = false;
         this.tblControllerStatus.Visible = false;
         this.tblVCRDelay.Visible = false;
         this.tblTAR.Visible = false;
         // hide reefer table
         this.tblReefer.Visible = false;
         this.tblDTC.Visible = false;
         this.tblBadSensor.Visible = false;
         this.tblTetheredState.Visible = false;   
      }

      private void GetControllerStatus(string CustomProp)
      {
         try
         {
            string ControllerStatus = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.extControllerVersion, CustomProp).TrimEnd();
            this.lblControllerVersion.Text = ControllerStatus;
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      private void GetBadSensor(string CustomProp)
      {
         try
         {
            this.lblBadSensorValue.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keySensorNum, CustomProp);
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      private void GetVCRDelay(string CustomProp)
      {
         try
         {
            string VCRDelay = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyVCRDelayTime, CustomProp).TrimEnd();
            cboVCRDelayPeriod.SelectedIndex = cboVCRDelayPeriod.Items.IndexOf(cboVCRDelayPeriod.Items.FindByValue(VCRDelay));
            cboVCRDelayPeriod.Enabled = false;
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      private string SetStrDefault(string value, string defaultValue)
      {
         if (value.TrimEnd() == "")
            return defaultValue;
         else
            return value;
      }

      private void GetTarMode(string CustomProp)
      {
         try
         {
            string keyTarMode = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyTARMode, CustomProp).TrimEnd();

            int Days = Convert.ToInt32(keyTarMode) / 24;
            int Hours = Convert.ToInt32(keyTarMode) % 24;
            if (keyTarMode == "0")
               this.lblTarMode.Text = (string)base.GetLocalResourceObject("Text_Off");
            else if (keyTarMode == "255")
               this.lblTarMode.Text = (string)base.GetLocalResourceObject("Text_AlwaysOn");
            else
               this.lblTarMode.Text = Days + " " + (string)base.GetLocalResourceObject("Text_DaysAnd") + " " + Hours + " " + (string)base.GetLocalResourceObject("Text_Hours");
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      private void GetThirdPartyValue(string CustomProp)
      {
         try
         {
            byte[] binaryData;

            binaryData = System.Convert.FromBase64String(CustomProp);
            StringBuilder str = new StringBuilder();
            str.AppendFormat("Command:{0}{1}{2}Packet ID:{3}{4}App ID:{5}{6}DCL ID:{7}{8}Message:{9}",
                (char)binaryData[4], (char)binaryData[5], Environment.NewLine, binaryData[6], Environment.NewLine, binaryData[10], Environment.NewLine, binaryData[11], Environment.NewLine,
                new ASCIIEncoding().GetString(binaryData, 25, binaryData.Length - 25));

            this.lblMessage.Text = str.ToString();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      private void GetSMC(string CustomProp, Int32 boxId, DateTime originDateTime,Int16 boxMsgInTypeId)
      {      
         try
         {
            string keySMC = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keySMC, CustomProp).TrimEnd();
            switch (keySMC.Trim().Length)
            {
                case 1:
                    keySMC = "000" + keySMC;
                    break;

                case 2:
                    keySMC = "00" + keySMC;
                    break;

                case 3:
                    keySMC = "0" + keySMC;
                    break;

                default:
                    break;
            }

           TimeSpan CurrentStatusSpan = new TimeSpan(DateTime.Now.Ticks - originDateTime.Ticks);
            if ((CurrentStatusSpan.TotalMinutes  <60) || (CurrentStatusSpan.TotalDays > 1))
                this.lblMessage.Text = keySMC;
             else
                this.lblMessage.Text = "*********";



            ////if (!CustomProp.Contains("UserId"))
            ////{
            ////   ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();
            ////   string custoPropAddons = "UserId=" + sn.UserID + ";DateTime=" + DateTime.Now.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString();
               
            ////   if (objUtil.ErrCheck(dbh.UpdateCustomPropInHistory(sn.UserID, sn.SecId, boxId, originDateTime.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), boxMsgInTypeId, custoPropAddons), false))
            ////      if (objUtil.ErrCheck(dbh.UpdateCustomPropInHistory(sn.UserID, sn.SecId, boxId, originDateTime.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), boxMsgInTypeId, custoPropAddons), true))
            ////      {
            ////         this.lblMessage.Text = "*********";
            ////         return;
            ////      }
            ////  this.lblMessage.Text = keySMC;
            ////}
            ////else
            ////{
            ////    TimeSpan CurrentStatusSpan = new TimeSpan(DateTime.Now.Ticks- originDateTime.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).Ticks);
            ////    if (CurrentStatusSpan.TotalDays  > 1)
            ////         this.lblMessage.Text = keySMC;
            ////     else
            ////        this.lblMessage.Text = "*********";
            ////}

            if (clsUtility.IsNumeric(keySMC) && Convert.ToInt32(keySMC) == 65535)
                this.lblMessage.Text = "Check Connection to Lock or contact Admin";
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      private void GetDTCValue(string CustomProp)
      {
          try
          {
              int keyDTCInPacket = Convert.ToInt32(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyDTCInPacket, CustomProp).TrimEnd());
              this.tblDTC.Visible = true;
               
              this.lblDTCCountinVehicleValue .Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyDTCInVehicle, CustomProp).TrimEnd();
              this.lblDTCinMsgValue.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyDTCInPacket, CustomProp).TrimEnd();
              this.lblDTCSourceValue.Text = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyDTCSource, CustomProp).TrimEnd();
              
              string DTCCodes= VLF.CLS.Util.GetDTCCodes(CustomProp);
              if (DTCCodes != "" && lblDTCSourceValue.Text.Contains("OBD2"))
              {
                  string xml = "";
                  DataSet ds = new DataSet();
                  string[] tmpDTCCode = DTCCodes.Split(',');

                  if (tmpDTCCode.Length > 0)
                  {
                      using (ServerDBVehicle.DBVehicle dbVehicle = new ServerDBVehicle.DBVehicle())
                      {
                          if (objUtil.ErrCheck(dbVehicle.GetDTCCodesDescription(sn.UserID, sn.SecId, ref xml), false))
                              if (objUtil.ErrCheck(dbVehicle.GetDTCCodesDescription(sn.UserID, sn.SecId, ref xml), true)) {}
                          if (xml != "")
                          {
                              StringReader strrXML = new StringReader(xml);
                              ds.ReadXml(strrXML);
                          }
                       }

                       for (int x = 0; x <= tmpDTCCode.Length; x++)
                       {
                           DataRow[] drArrCode = ds.Tables[0].Select("DTCCode='" + tmpDTCCode[x] + "'");
                           if (drArrCode != null && drArrCode.Length > 0)
                               this.lblDTCValue.Text += tmpDTCCode[x] + "-" + drArrCode[0]["text"] + "\r\n";
                           else
                               this.lblDTCValue.Text += tmpDTCCode[x] + "\r\n";
                       }
                   }
               }
               else
               {
                   this.lblDTCValue.Text = DTCCodes;
               }              
          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
          }
      }

       private void GetTetheredStateValue(string CustomProp)
       {
           DataSet ds= new DataSet();
           string xml = "";
           try
           {
               this.lblPerType.Text=Convert.ToString((VLF.CLS.Def.Enums.PeripheralTypes)(Convert.ToInt16(VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyPeripheralType, CustomProp).TrimEnd())));
               string MdtType = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyMDTType, CustomProp).TrimEnd();

               if (MdtType.Trim() != "")
               {
                   using (ServerDBVehicle.DBVehicle dbVehicle = new ServerDBVehicle.DBVehicle())
                   {
                       if (objUtil.ErrCheck(dbv.GetMDTInfoByTypeId(sn.UserID, sn.SecId, Convert.ToInt32(MdtType), ref xml), false))
                           if (objUtil.ErrCheck(dbv.GetMDTInfoByTypeId(sn.UserID, sn.SecId, Convert.ToInt32(MdtType), ref xml), true))
                           {
                               return;
                           }
                   }

                   if (xml == "")
                       return;

                   StringReader strrXML = null;
                   strrXML = new StringReader(xml);
                   ds.ReadXml(strrXML);
                   this.lblMdtType.Text = ds.Tables[0].Rows[0]["Manufacturer"] + " - " + ds.Tables[0].Rows[0]["Model"];  
               }
               this.lblMDTver.Text =VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyMDTMajorVersion, CustomProp).TrimEnd() + "." + VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyMDTMinorVersion, CustomProp).TrimEnd();; 
               this.tblTetheredState.Visible = true;   
           }
           catch {}
       }

       private void GetEEPROMValue(string CustomProp)
       {
           try
           {
               this.lblMessage.Text = "Offset: " + VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyEEPROMOffset, CustomProp);
               this.lblMessage.Text += "\r\n"+ "Length: " + VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyEEPROMLength, CustomProp);
               this.lblMessage.Text += "\r\n" + "Data: " + VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyEEPROMData, CustomProp);
           }
           catch {}
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
   }
}
