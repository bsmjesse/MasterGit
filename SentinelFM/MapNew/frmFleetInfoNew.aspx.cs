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
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using Telerik.Web.UI;
using System.Web.Services;
using System.Text;
using System.Web.Script.Serialization;

namespace SentinelFM
{
    public partial class MapNew_frmMap : SentinelMapBasePage
    {
        public Boolean isAlarm = false;
        public Boolean isMessage = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref FleetForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                GuiSecurity(this);
            }
            if (sn.User.ViewAlarmScrolling == 1)
            {
                isAlarm = true;
            }

            if (sn.User.ViewMDTMessagesScrolling == 1)
            {
                isMessage = true;
            }
        }


        [WebMethod]
        public static string SelectVehicle(string fleetID, string vehicleID, bool isChecked)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                if (fleetID != "-1")
                    sn.History.FleetId = Convert.ToInt64(fleetID);
                sn.History.FromDate = DateTime.Now.ToString("MM/dd/yyyy");
                sn.History.ToDate = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
                sn.History.FromHours = "0";
                sn.History.ToHours = "0";



                DataRow[] drCollections = null;
                drCollections = sn.Map.DsFleetInfo.Tables[0].Select("VehicleId=" + vehicleID);
                if (drCollections != null && drCollections.Length > 0)
                {
                    DataRow dRow = drCollections[0];
                    dRow["chkBoxShow"] = isChecked;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: SelectVehicle() Page:frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";
        }

        [WebMethod]
        public static string MapIt(string vehicleIDs)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                return MapNew_UserControl_FleetInfoNew.SaveShowCheckBoxes_1(vehicleIDs, true, sn);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: MapIt() Page:frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";
        }

        [WebMethod]
        public static string UpdatePosition(string vehicleIDs)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                Int64 sessionTimeOut = 0;
                clsUtility objUtil = new clsUtility(sn);

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                DataColumn dc;

                dc = new DataColumn("Freq", Type.GetType("System.Int64"));
                dc.DefaultValue = 0;
                dt.Columns.Add(dc);


                bool cmdSent = false;


                sn.MessageText = "";
                LocationMgr.Location dbl = new LocationMgr.Location();

                MapNew_UserControl_FleetInfoNew.SaveShowCheckBoxes_1(vehicleIDs, false, sn);
                bool ShowTimer = false;
                //Delete old timeouts
                sn.Cmd.DtUpdatePositionFails.Rows.Clear();


                if (vehicleIDs == string.Empty) return "1";

                //Replace
                DataRow[] drArr = sn.Map.DsFleetInfo.Tables[0].Select("chkBoxShow=true", "", DataViewRowState.CurrentRows);
                if (drArr == null || drArr.Length == 0)
                {
                    return "1";
                }
                //{
                //    sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle");
                //    ShowErrorMessage();
                //    this.tblWait.Visible = false;
                //    return;
                //}

                //DumpBeforeCall(sn, string.Format("frmFleetInfoNew -- cmdUpdatePosition"));


#if NEW_SLS
              


               int[] boxid=new int[drArr.Length];
               short[] protocolType=new short[drArr.Length];
               short[] commMode=new short[drArr.Length];
               bool[] sent = new bool[drArr.Length];
               Int64[] timeOut=new Int64[drArr.Length];
               short[] results = new short[drArr.Length];
               string[] vehicles = new string[drArr.Length];
               

               int i=0;
               foreach (DataRow rowItem in drArr)
                {
                       boxid[i]=Convert.ToInt32(rowItem["BoxId"]);
                       protocolType[i]=-1;
                       commMode[i]=-1;
                       sent[i]=false;
                       timeOut[i]=0;
                       results[i]=0;
                       vehicles[i]=rowItem["Description"].ToString() ;
                       i++;
                }

                if (objUtil.ErrCheck(dbl.SendCommandToMultipleVehicles(sn.UserID, sn.SecId, DateTime.Now,boxid , Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "",false, ref protocolType  , ref commMode  , ref sent, ref timeOut,ref results), false))
                   if (objUtil.ErrCheck(dbl.SendCommandToMultipleVehicles(sn.UserID, sn.SecId, DateTime.Now,boxid , Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "",false, ref protocolType  , ref commMode  , ref sent, ref timeOut,ref results), true ))
                     {
                         System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Send update position failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                         return;
                     }


                for (i = 0; i < sent.Length; i++)
                {
                    if (!sent[i])
                    {
                        DataRow drErr;
                        drErr = sn.Cmd.DtUpdatePositionFails.NewRow();
                        drErr["VehicleDesc"] = vehicles[i];
                        drErr["Status"] = (string)base.GetLocalResourceValue("sn_MessageText_SendCommandFailedError") + ": " + vehicles[i]; 
                        sn.Cmd.DtUpdatePositionFails.Rows.Add(drErr);
                    }
                    else
                    {
                        sn.Cmd.ArrBoxId.Add(boxid[i]);
                        sn.Cmd.ArrProtocolType.Add(protocolType[i]);
                        sn.Cmd.ArrVehicle.Add(vehicles[i]);   
                        ShowTimer = true;
                    }

                }
                
                 sessionTimeOut = FindMax(timeOut);

                
#else

                SentinelFMBasePage basePage = new SentinelFMBasePage();
                foreach (DataRow rowItem in drArr)
                {
                    short ProtocolId = -1;
                    short CommModeId = -1;
                    string errMsg = "";
                    if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "", ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                        if (errMsg == "")
                        {
                            if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "", ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                            {
                                if (errMsg != "")
                                    sn.MessageText = errMsg;
                                else
                                    sn.MessageText = MapNew_UserControl_FleetInfoNew.GetLocalResourceValue("sn_MessageText_SendCommandFailedError") + ": " + rowItem["Description"];

                                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, " User:" + sn.UserID.ToString() + "Web method: UpdatePosition() Page:frmFleetInfoNew"));

                                DataRow drErr;
                                drErr = sn.Cmd.DtUpdatePositionFails.NewRow();
                                drErr["VehicleDesc"] = rowItem["Description"];
                                drErr["Status"] = sn.MessageText;
                                sn.Cmd.DtUpdatePositionFails.Rows.Add(drErr);
                                rowItem["Updated"] = CommandStatus.CommTimeout;

                            }
                        }
                        else
                        {
                            sn.MessageText = errMsg;
                            DataRow drErr;
                            drErr = sn.Cmd.DtUpdatePositionFails.NewRow();
                            drErr["VehicleDesc"] = rowItem["Description"];
                            drErr["Status"] = sn.MessageText;
                            sn.Cmd.DtUpdatePositionFails.Rows.Add(drErr);
                            rowItem["Updated"] = CommandStatus.CommTimeout;
                        }


                    DataRow dr = dt.NewRow();
                    dr["Freq"] = sessionTimeOut;// sessionTimeOut > 0 ? Convert.ToInt64(Math.Round(sessionTimeOut / 15.0)) * 1000 : 2000;
                    dt.Rows.Add(dr);

                    rowItem["ProtocolId"] = ProtocolId;
                    sn.Cmd.ProtocolTypeId = ProtocolId;

                    if (cmdSent)
                        ShowTimer = true;
                    else
                    {

                        //Create pop up message
                        sn.MessageText = MapNew_UserControl_FleetInfoNew.GetLocalResourceValue("sn_MessageText_SendCommandToVehicle1") + " :" + rowItem["Description"].ToString().TrimEnd() + " " + MapNew_UserControl_FleetInfoNew.GetLocalResourceValue("sn_MessageText_SendCommandToVehicle2");

                        DataRow drErr;
                        drErr = sn.Cmd.DtUpdatePositionFails.NewRow();
                        drErr["VehicleDesc"] = rowItem["Description"];
                        drErr["Status"] = sn.MessageText;
                        sn.Cmd.DtUpdatePositionFails.Rows.Add(drErr);
                        rowItem["Updated"] = CommandStatus.CommTimeout;

                    }

                }


                try
                {
                    ds.Tables.Add(dt);
                    DataView dv = ds.Tables[0].DefaultView;
                    dv.Sort = "Freq" + " DESC";
                    sessionTimeOut = Convert.ToInt64(dv[0].Row[0]);
                    sn.Cmd.GetCommandStatusRefreshFreq = sessionTimeOut > 0 ? Convert.ToInt64(Math.Round(sessionTimeOut / 15.0)) * 1000 : 2000;
                }
                catch
                {
                    sn.Cmd.GetCommandStatusRefreshFreq = 2000;
                }


#endif

                if (ShowTimer)
                {


                    //this.tblWait.Visible = true;
                    //this.tblFleetActions.Visible = false;
                    sn.Cmd.GetCommandStatusRefreshFreq = sessionTimeOut > 0 ? Convert.ToInt64(Math.Round(sessionTimeOut / 15.0)) * 1000 : 2000;
                    string msg = string.Empty;
                    if (sessionTimeOut > 60)
                        msg = MapNew_UserControl_FleetInfoNew.GetLocalResourceValue("lblUpdatePosition_Text_Minutes1") + " " + Convert.ToInt64(Math.Round(sessionTimeOut / 60.0)).ToString() + " " + MapNew_UserControl_FleetInfoNew.GetLocalResourceValue("lblUpdatePosition_Text_Minutes2");
                    else
                        msg = MapNew_UserControl_FleetInfoNew.GetLocalResourceValue("lblUpdatePosition_Text_Seconds1") + " " + sessionTimeOut + " " + MapNew_UserControl_FleetInfoNew.GetLocalResourceValue("lblUpdatePosition_Text_Seconds2");

                    sn.Map.TimerStatus = true;
                    sn.Cmd.UpdatePositionSend = true;
                    return msg;

                }
                else
                {
                    return MapNew_UserControl_FleetInfoNew.NoShowString; //Not Show
                }

            }


            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: UpdatePosition() Page:frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";

            }
        }


        [WebMethod]
        public static string CancelUpdatePos()
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                clsUtility objUtil = new clsUtility(sn);
                LocationMgr.Location dbl = new LocationMgr.Location();


                foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                    {
                        if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), sn.Cmd.ProtocolTypeId), false))
                            if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), sn.Cmd.ProtocolTypeId), true))
                            {
                                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, " cannot Cancel command for User:" + sn.UserID.ToString() + " Form:frmFleetInfoNew.aspx"));
                            }

                    }
                }

                sn.Map.TimerStatus = false;
            }

            catch (NullReferenceException ex)
            {
                sn.Map.TimerStatus = false;
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: CancelUpdatePos() Page:frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

                return "0";
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: CancelUpdatePos() Page:frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";

            }
            return "1";
        }

        [WebMethod]
        public static string History(string vehicleID, string vehicleIDs)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                sn.History.VehicleId = Int64.Parse(vehicleID);
                MapNew_UserControl_FleetInfoNew.SaveShowCheckBoxes_1(vehicleIDs, false, sn);
                sn.History.DsSelectedData = null;
                sn.History.RedirectFromMapScreen = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: UpdatePositionResult() Page:frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";
        }

        [WebMethod]
        public static string UpdatePositionResult()
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                if (sn.Map.TimerStatus == false)
                    return "";

                int cmdStatus = 0;

                LocationMgr.Location dbl = new LocationMgr.Location();
                bool blnReload = false;

#if NEW_SLS  
                    int[] ArrCmdStatus =new int[sn.Cmd.ArrBoxId.Count];
                    int[] ArrBoxId = sn.Cmd.ArrBoxId.ToArray();
                    short [] ArrProtocolType = sn.Cmd.ArrProtocolType.ToArray();

                    if (objUtil.ErrCheck(dbl.GetCommandStatusFromMultipleVehicles(sn.UserID, sn.SecId, ArrBoxId, ArrProtocolType, ref ArrCmdStatus), false))
                        if (objUtil.ErrCheck(dbl.GetCommandStatusFromMultipleVehicles(sn.UserID, sn.SecId, ArrBoxId, ArrProtocolType, ref ArrCmdStatus), true))
                     {
                     }

                    sn.Cmd.ArrCmdStatus.Clear();
                    sn.Cmd.ArrBoxId.Clear();
                    sn.Cmd.ArrProtocolType.Clear(); 

                    for (int i = 0; i < ArrCmdStatus.Length; i++)
                    {
                        cmdStatus = ArrCmdStatus[i];
                        if ((cmdStatus == (int)CommandStatus.Ack) || (cmdStatus == (int)CommandStatus.CommTimeout) || (cmdStatus == (int)CommandStatus.Pending))
                        {
                            if ((cmdStatus == (int)CommandStatus.Pending) || (cmdStatus == (int)CommandStatus.CommTimeout))
                            {
                                DataRow dr;
                                dr = sn.Cmd.DtUpdatePositionFails.NewRow();
                                dr["VehicleDesc"] = sn.Cmd.ArrVehicle[i];
                                dr["Status"] = cmdStatus.ToString();
                                sn.Cmd.DtUpdatePositionFails.Rows.Add(dr);

                                // Clear flag GPS not valid for time-out vechicles.
                                //rowItem["LastCommunicatedDateTime"] = rowItem["OriginDateTime"];
                            }
                        }
                        else
                        {
                            sn.Cmd.ArrBoxId.Add(ArrBoxId[i]);
                            sn.Cmd.ArrProtocolType.Add(ArrProtocolType[i]);
                            sn.Cmd.ArrCmdStatus.Add(ArrCmdStatus[i]);   
                            blnReload = true;
                        }
                    }


#else
                clsUtility objUtil = new clsUtility(sn);
                foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (rowItem["chkBoxShow"].ToString().ToLower() == "true" && !Convert.ToBoolean(rowItem["Updated"]))
                    {
                        if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(rowItem["ProtocolId"]), ref cmdStatus), false))
                            if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(rowItem["ProtocolId"]), ref cmdStatus), true))
                            {
                                cmdStatus = (int)CommandStatus.CommTimeout;
                                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Session for Box: " + rowItem["BoxId"].ToString() + " and User: " + sn.UserID.ToString() + " does not exist. Form:frmTimerPosition.aspx"));
                            }

                        sn.Cmd.Status = (CommandStatus)cmdStatus;

                        if ((cmdStatus == (int)CommandStatus.Ack) || (cmdStatus == (int)CommandStatus.CommTimeout) || (cmdStatus == (int)CommandStatus.Pending))
                        {
                            if ((cmdStatus == (int)CommandStatus.Pending) || (cmdStatus == (int)CommandStatus.CommTimeout))
                            {
                                DataRow dr;
                                dr = sn.Cmd.DtUpdatePositionFails.NewRow();
                                dr["VehicleDesc"] = rowItem["Description"];
                                dr["Status"] = cmdStatus.ToString();
                                sn.Cmd.DtUpdatePositionFails.Rows.Add(dr);

                                // Clear flag GPS not valid for time-out vechicles.
                                rowItem["LastCommunicatedDateTime"] = rowItem["OriginDateTime"];
                            }

                            rowItem["Updated"] = (int)cmdStatus;
                        }
                        else
                        {
                            blnReload = true;
                        }
                    }
                }

#endif


                if (blnReload)
                {
                    sn.Map.TimerStatus = true;
                    return MapNew_UserControl_FleetInfoNew.start_characters_begin + sn.Cmd.GetCommandStatusRefreshFreq.ToString();
                }
                else
                {

                    sn.Map.TimerStatus = false;
                    return MapNew_UserControl_FleetInfoNew.start_characters_end + MapNew_UserControl_FleetInfoNew.CheckUpdatePositionResult(sn);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: FreshUpdateGrid() Page:frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";
        }



    }
}
