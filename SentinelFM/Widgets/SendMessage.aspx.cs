using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VLF.CLS.Interfaces;
using VLF.PATCH.Logic;
using System.Configuration;

namespace SentinelFM
{
    public partial class Widgets_SendMessage : System.Web.UI.Page
    {
        public string ACTION = string.Empty;
        public int BOXID = -1;
        public int STATUS = 200;
        public string MSG = string.Empty;

        protected SentinelFMSession sn = null;
        protected clsUtility objUtil;

        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        VLF.PATCH.Logic.PatchVehicle _pv = null;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            objUtil = new clsUtility(sn);

            _pv = new VLF.PATCH.Logic.PatchVehicle(sConnectionString);
            
            int.TryParse(Request["BoxId"], out BOXID);
            ACTION = Request["Action"];
            if (string.IsNullOrEmpty(ACTION))
            {
                ACTION = "GetForm";
            }

            if (ACTION == "SendMessage")
            {
                sendMessage();
            }
            else if (ACTION == "CancelSendMessage")
            {
                cancelSendMessage();
            }
            else if (ACTION == "CheckStatus")
            {
                checkStatus();
            }
        }

        private void sendMessage()
        {
            string strMessage = "";
            string strResponse = "";
            string txtMessage = "";
            //string vehicleDesc = Request["vehicleDesc"];

            sn.Message.DtSendMessageBoxes.Rows.Clear();
            sn.Message.DtSendMessageFails.Rows.Clear();

            txtMessage = " " + Request["txtMessage"] + "\r\n";

            strMessage = VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMessage, txtMessage);

            int EnterPos = 0;
            int TotalEnters = 0;


            while (strMessage.IndexOf("\n", EnterPos + 1) != -1)
            {
                EnterPos = strMessage.IndexOf("\n", EnterPos + 1);
                if (EnterPos != -1)
                    TotalEnters++;
            }

            if (TotalEnters > 5)
            {
                STATUS = 500;
                MSG = "Too Many Lines.";                
                return;
            }

            if (BOXID == -1)
            {
                STATUS = 500;
                MSG = "Invalid Vehicle BoxId.";
                return;
            }

            DataSet _dt = _pv.GetVehiclePeripheralInfoByBoxId(BOXID);
            if (_dt.Tables[0].Rows.Count > 0 && (Convert.ToInt16(_dt.Tables[0].Rows[0]["TypeId"]) == (Int16)VLF.CLS.Def.Enums.PeripheralTypes.Garmin))
            {
                sendGarminMessage(txtMessage);
                return;
            }


            sn.Message.Response = "";
            

            sn.Message.Message = txtMessage;
            //sn.Message.FleetId = Convert.ToInt32(this.cboFleet.SelectedItem.Value);
            sn.Message.VehicleId = Convert.ToInt64(BOXID);

            strMessage += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyAnswer, strResponse);

            LocationMgr.Location dbl = new LocationMgr.Location();


            bool cmdSent = false;


            //--- Create table of Boxes for Message Send
            DataRow dr;
            dr = sn.Message.DtSendMessageBoxes.NewRow();
            dr["BoxId"] = Convert.ToInt32(BOXID);
            //dr["VehicleDesc"] = this.cboVehicle.SelectedItem.Text;
            dr["VehicleDesc"] = "";
            dr["Updated"] = -1;
            sn.Message.DtSendMessageBoxes.Rows.Add(dr);
            sn.Cmd.DualComm = false;
            sn.Cmd.BoxId = Convert.ToInt32(BOXID);
            sn.Cmd.CommandId = Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage);
            //------------------------------------------
            
            sn.Cmd.CommandParams = strMessage;
            sn.Cmd.CommModeId = -1;
            sn.Cmd.ProtocolTypeId = -1;

            
            sn.Cmd.SchCommand = false;
            sn.Cmd.SchInterval = 0;
            sn.Cmd.SchPeriod = 0;

            foreach (DataRow rowVehicle in sn.Message.DtSendMessageBoxes.Rows)
            {
                short ProtocolId = -1;
                short CommModeId = -1;
                Int64 sessionTimeOut = 0;
                if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowVehicle["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), strMessage, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false))
                    if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowVehicle["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), strMessage, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true))
                    {
                        STATUS = 500;
                        MSG = "Failed to send message.";
                        return;                        
                    }
                sn.Cmd.CommModeId = CommModeId;
                rowVehicle["ComModeId"] = CommModeId;
                rowVehicle["ProtocolId"] = ProtocolId;
            }

            sn.Message.MessageSent = true;
            sn.Message.TimerStatus = true;
            STATUS = 200;
            MSG = "Succeed";
            
        }

        private void sendGarminMessage(string msg)
        {
            string paramList = VLF.CLS.Util.MakePair(GarminFMI.JKeyWords.TXT.ToString(), msg);
            paramList += VLF.CLS.Util.MakePair(GarminFMI.JKeyWords.YESNO.ToString(), "false");
            paramList += VLF.CLS.Util.MakePair(GarminFMI.JKeyWords.RETRYINTERVAL.ToString(), "5");  // 5 minutes
            paramList += VLF.CLS.Util.MakePair(GarminFMI.JKeyWords.LIFETIME.ToString(), "60");


            using (DBGarmin.Garmin garmin = new DBGarmin.Garmin())
            {
                
                if (objUtil.ErrCheck(garmin.SendTextMessage(sn.UserID, sn.SecId, BOXID, 42, paramList), false))
                    if (objUtil.ErrCheck(garmin.SendTextMessage(sn.UserID, sn.SecId, BOXID, 42, paramList), true))
                    {
                        STATUS = 500;
                        MSG = "Failed to send message.";
                        return;
                    }               

            }
            
            STATUS = 201;
            MSG = "Message Sent";
        }

        private void cancelSendMessage()
        {
            try
            {
                LocationMgr.Location dbl = new LocationMgr.Location();

                foreach (DataRow rowItem in sn.Message.DtSendMessageBoxes.Rows)
                {

                    if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(rowItem["ProtocolId"])), false))
                        if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(rowItem["ProtocolId"])), true))
                        {
                            CancelCommand();
                            return;
                        }
                }


                CancelCommand();
            }

            catch (Exception Ex)
            {
                CancelCommand();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }


        private void CancelCommand()
        {
            sn.Message.TimerStatus = false;

            STATUS = 200;
            MSG = "send message cancelled.";
        }

        private void checkStatus()
        {
            try
            {
                //Clear IIS cache
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);

                sn = (SentinelFMSession)Session["SentinelFMSession"];
                if (sn.Message.TimerStatus == false)
                    return;

                int cmdStatus = 0;


                LocationMgr.Location dbl = new LocationMgr.Location();


                bool blnReload = false;


                foreach (DataRow rowItem in sn.Message.DtSendMessageBoxes.Rows)
                {

                    if ((Convert.ToInt16(rowItem["Updated"]) != (short)CommandStatus.Ack) &&
                          (Convert.ToInt16(rowItem["Updated"]) != (short)CommandStatus.CommTimeout) &&
                          (Convert.ToInt16(rowItem["Updated"]) != (short)CommandStatus.Pending))
                    {
                        if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(rowItem["ProtocolId"]), ref cmdStatus), false))
                            if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(rowItem["ProtocolId"]), ref cmdStatus), true))
                            {
                                cmdStatus = (int)CommandStatus.CommTimeout;
                                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Session for Box: " + rowItem["BoxId"].ToString() + " and User: " + sn.UserID.ToString() + " does not exist. Form:frmNewMessageTimer.aspx"));
                            }

                        sn.Cmd.Status = (CommandStatus)cmdStatus;

                        if ((cmdStatus == (int)CommandStatus.Ack) || (cmdStatus == (int)CommandStatus.CommTimeout) || (cmdStatus == (int)CommandStatus.Pending))
                        {

                            //Dual Communication mode
                            if ((cmdStatus == (int)CommandStatus.CommTimeout) && (sn.Cmd.DualComm == true))
                            {


                                sn.Cmd.CommModeId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[1]["CommModeId"]);
                                sn.Cmd.ProtocolTypeId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[1]["BoxProtocolTypeId"]);


                                bool cmdSent = false;


                                short CommModeId = sn.Cmd.CommModeId;
                                short ProtocolTypeId = sn.Cmd.ProtocolTypeId;
                                Int64 sessionTimeOut = 0;
                                if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), sn.Cmd.CommandId, sn.Cmd.CommandParams, ref ProtocolTypeId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false))
                                    if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), sn.Cmd.CommandId, sn.Cmd.CommandParams, ref ProtocolTypeId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true))
                                    {
                                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Sent command failed " + Convert.ToString((VLF.CLS.Def.Enums.CommMode)CommModeId) + ". User:" + sn.UserID.ToString() + " Form:frmNewMessageTimer.aspx"));
                                    }




                                if (cmdSent)
                                {
                                    rowItem["ProtocolId"] = ProtocolTypeId;
                                    sn.Cmd.DualComm = false;
                                    sn.Message.TimerStatus = true;
                                    Response.Write("<script language='javascript'>window.setTimeout('location.reload(true)',1000)</script>");
                                    return;
                                }
                            }

                            rowItem["Updated"] = cmdStatus;

                            //Time Out Send Message
                            if (cmdStatus == (int)CommandStatus.CommTimeout)
                            {

                                if (sn.Cmd.SchCommand)
                                {
                                    sn.Message.MessageQueued = false;

                                    Int64 TaskId = 0;
                                    Int16 CommModeId = Convert.ToInt16(rowItem["ComModeId"]);
                                    Int16 ProtocolTypeId = Convert.ToInt16(rowItem["ProtocolId"]);

                                    if (objUtil.ErrCheck(dbl.SendAutomaticCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), DateTime.Now, sn.Cmd.CommandParams, ref ProtocolTypeId, ref   CommModeId, sn.Cmd.SchPeriod, sn.Cmd.SchInterval, false, ref TaskId), false))
                                        if (objUtil.ErrCheck(dbl.SendAutomaticCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), DateTime.Now, sn.Cmd.CommandParams, ref ProtocolTypeId, ref   CommModeId, sn.Cmd.SchPeriod, sn.Cmd.SchInterval, false, ref TaskId), true))
                                        {
                                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Sent Automatic command failed " + Convert.ToString((VLF.CLS.Def.Enums.CommMode)CommModeId) + ". User:" + sn.UserID.ToString() + " Form:frmNewMessageTimer.aspx"));
                                        }

                                    if (TaskId > 0)
                                    {
                                        sn.Message.MessageQueued = true;
                                        rowItem["Updated"] = (short)CommandStatus.Ack;
                                        sn.Message.TimerStatus = true;
                                        Response.Write("<script language='javascript'>window.setTimeout('location.reload(true)',1000)</script>");
                                        return;
                                    }
                                }

                                DataRow dr;
                                dr = sn.Message.DtSendMessageFails.NewRow();
                                dr["VehicleDesc"] = rowItem["VehicleDesc"].ToString().Replace("'", "''");
                                sn.Message.DtSendMessageFails.Rows.Add(dr);
                            }



                        }
                        else
                        {
                            blnReload = true;
                        }
                    }
                }


                if (blnReload)
                {
                    sn.Message.TimerStatus = true;
                    STATUS = 300;
                    MSG = "Recheck status.";
                    //Response.Write("<script language='javascript'>window.setTimeout('location.reload(true)',1000)</script>");
                }
                else
                {

                    sn.Message.TimerStatus = false;

                    string commandStatus = string.Empty;
                    if (sn.Message.MessageQueued)
                    {
                        commandStatus = (string)base.GetLocalResourceObject("lblCommandStatus_Text_MessageQueued");
                    }

                    else if ((sn.Cmd.CommModeId == 0) || (sn.Cmd.CommModeId == -1))
                        commandStatus = (string)base.GetLocalResourceObject("lblCommandStatus_Text_MessageSuccess");
                    else
                    {
                        commandStatus = (string)base.GetLocalResourceObject("lblCommandStatus_Text_MessageSuccessOtherMode") + " " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId;
                        sn.Cmd.CommModeId = 0;
                    }

                    STATUS = 200;
                    MSG = commandStatus;
                    //Response.Write("<script language='javascript'> clearTimeout();  parent.OnMessageSent('" + commandStatus + "'); </script>");
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
    }
}