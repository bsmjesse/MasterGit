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
using Ext.Net;
using VLF.DAS.Logic;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace SentinelFM
{
    public partial class frmAlarmInfo_G4S : SentinelFMBasePage
    {
        public ClientMapProxy map;

        public string redirectURL;
        public string SourcePage = "";
        public string AlarmLandmarkID = "";
        public string AlarmDescriptionText = "";
        public string AlarmBoxId = "";
        public string AlarmVehicleDescription = "";
        public string AlarmLon = "";
        public string AlarmLat = "";

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (Request["s"] != null && Request["s"].ToString().Trim() != "")
                    SourcePage = Request["s"].ToString().Trim();

                cmdMapIt.OnClientClick = "MapIt();" + (SourcePage == "newmap" ? "return false;" : "");

                if (!Page.IsPostBack)
                {
                    GuiSecurity(this);
                    int AlarmId = Convert.ToInt32(Request.QueryString["AlarmId"]);
                    ViewState["AlarmId"] = AlarmId;
                    if (AlarmId == -1 || AlarmId == -2)
                        ViewState["alarmlist"] = Request.QueryString["al"];
                    AlarmInfoLoad_NewTZ(AlarmId);
                    ViewState["ConfirmCloseAll"] = "0";

                    if (AlarmId == -1 || AlarmId == -2)
                    {
                        trAlarmID.Visible = false;
                        trAlarmDescription.Visible = false;
                        trTimeCreated.Visible = false;
                        trAlarmState.Visible = false;
                        trAlarmServerity.Visible = false;
                        trVehicleId.Visible = false;
                        trVehicleDescription.Visible = false;
                        trStreetAddress.Visible = false;
                        cmdMapIt.Visible = false;
                    }

                    if (AlarmId == -1)
                    {
                        alarmlegentTitle.InnerText = "Clear Non-Critical";
                        cmdAccept.Text = "Clear Non-Critical Alarms";
                        cmdAccept.Width = 174;
                    }
                    else if (AlarmId == -2)
                    {
                        alarmlegentTitle.InnerText = "Clear All";
                        cmdAccept.Text = "Clear All Alarms";
                    }

                }
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }


        }

        #region RemoveWhenMVComes

        // Changes for TimeZone Feature start
        public int GetCurrentAlarmInfoXMLDirect_NewTZ(int userId, string SID, int alarmId, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {

                using (Alarm dbAlarm = new Alarm(ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAlarmInfoByAlarmId(alarmId);
                    if ((dsAlarmInfo != null && dsAlarmInfo.Tables.Count > 0 && dsAlarmInfo.Tables[0].Rows.Count > 0)) //if (ASLUtil.IsAnyRecord(dsAlarmInfo)) // != null && dsAlarmInfo.Tables.Count > 0 && dsAlarmInfo.Tables[0].Rows.Count > 0)
                    {
                        using (User dbUser = new User(ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString))
                        {
                            DataSet dsCurrentAlarm = new DataSet(dsAlarmInfo.DataSetName);
                            dsCurrentAlarm.Tables.Add(dsAlarmInfo.Tables[0].TableName);
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("TimeCreated", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("TimeAccepted", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("TimeClosed", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmState", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmLevel", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("UserId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("vehicleDescription", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmDescription", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("VehicleId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("StreetAddress", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("UserName", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("Latitude", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("Longitude", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("Speed", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("Heading", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("SensorMask", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("IsArmed", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("BoxId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("CustomProp", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmLandmarkID", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("NearestLandmark", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("Fleets", typeof(string));

                            foreach (DataRow ittr in dsAlarmInfo.Tables[0].Rows)
                            {
                                // LOCALIZATION
                                object[] objRow = new object[dsCurrentAlarm.Tables[0].Columns.Count];
                                objRow[0] = ittr["AlarmId"].ToString();
                                objRow[1] = dbUser.GetUserLocalTime_NewTZ(userId, Convert.ToDateTime(ittr["DateTimeCreated"])).ToString();

                                if (ittr["DateTimeAck"] == DBNull.Value)
                                    objRow[2] = " ";
                                else
                                    objRow[2] = dbUser.GetUserLocalTime_NewTZ(userId, Convert.ToDateTime(ittr["DateTimeAck"])).ToString();

                                if (ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[3] = " ";
                                else
                                    objRow[3] = dbUser.GetUserLocalTime_NewTZ(userId, Convert.ToDateTime(ittr["DateTimeClosed"])).ToString();

                                if (ittr["DateTimeAck"] == DBNull.Value && ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[4] = VLF.CLS.Def.Enums.AlarmState.New.ToString();
                                else if (ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[4] = VLF.CLS.Def.Enums.AlarmState.Accepted.ToString();
                                else
                                    objRow[4] = VLF.CLS.Def.Enums.AlarmState.Closed.ToString();
                                objRow[5] = ((VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt32(ittr["AlarmLevel"])).ToString();
                                objRow[6] = ittr["UserId"].ToString();
                                objRow[7] = ittr["vehicleDescription"].ToString();
                                objRow[8] = ittr["AlarmDescription"].ToString();
                                objRow[9] = ittr["VehicleId"].ToString();
                                string streetAddress = ittr["StreetAddress"].ToString().TrimEnd();
                                if (Convert.ToInt16(ittr["ValidGPS"]) == 0) // 0 - valid, 1 -invalid
                                {
                                    // If street address is empty, get it from database and update property
                                    if (streetAddress == "")
                                    {
                                        // TODO: retrieves from map engine			
                                    }
                                }
                                else
                                {
                                    streetAddress = VLF.CLS.Def.Const.noValidAddress;
                                }
                                objRow[10] = streetAddress;
                                objRow[11] = ittr["UserName"].ToString();
                                objRow[12] = ittr["Latitude"].ToString();
                                objRow[13] = ittr["Longitude"].ToString();
                                objRow[14] = ittr["Speed"].ToString();
                                objRow[15] = ittr["Heading"].ToString();
                                objRow[16] = ittr["SensorMask"].ToString();
                                objRow[17] = ittr["IsArmed"].ToString();
                                objRow[18] = ittr["BoxId"].ToString();
                                if (ittr["AlarmLandmarkID"] == DBNull.Value)
                                    objRow[20] = "";
                                else
                                    objRow[20] = ittr["AlarmLandmarkID"].ToString();
                                objRow[21] = ittr["NearestLandmark"].ToString();
                                objRow[22] = ittr["Fleets"].ToString();

                                AlarmBoxId = ittr["BoxId"].ToString();
                                AlarmVehicleDescription = ittr["vehicleDescription"].ToString();
                                AlarmLon = ittr["Longitude"].ToString();
                                AlarmLat = ittr["Latitude"].ToString();
                                dsCurrentAlarm.Tables[0].Rows.Add(objRow);
                            }
                            xml = dsCurrentAlarm.GetXml();
                        }

                    } // using( User dbUser =

                } // using( Alarm dbAlarm =

                //LogFinal("<< GetCurrentAlarmInfoXML(uId={0}, alarmId={1}, tSpan={2})",
                //               userId, alarmId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return 0;//(int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                //LogException("<< GetCurrentAlarmInfoXML : uId={0}, EXC={1}", userId, Ex.Message);
                //return (int)ASIErrorCheck.CheckError(Ex);
                return 6; //Db Error
            }
        }


        // Changes for TimeZone Feature end
        public int GetCurrentAlarmInfoXMLDirect(int userId, string SID, int alarmId, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {

                using (Alarm dbAlarm = new Alarm(ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAlarmInfoByAlarmId(alarmId);
                    if ((dsAlarmInfo != null && dsAlarmInfo.Tables.Count > 0 && dsAlarmInfo.Tables[0].Rows.Count > 0)) //if (ASLUtil.IsAnyRecord(dsAlarmInfo)) // != null && dsAlarmInfo.Tables.Count > 0 && dsAlarmInfo.Tables[0].Rows.Count > 0)
                    {
                        using (User dbUser = new User(ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString))
                        {
                            DataSet dsCurrentAlarm = new DataSet(dsAlarmInfo.DataSetName);
                            dsCurrentAlarm.Tables.Add(dsAlarmInfo.Tables[0].TableName);
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("TimeCreated", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("TimeAccepted", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("TimeClosed", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmState", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmLevel", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("UserId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("vehicleDescription", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmDescription", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("VehicleId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("StreetAddress", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("UserName", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("Latitude", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("Longitude", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("Speed", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("Heading", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("SensorMask", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("IsArmed", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("BoxId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("CustomProp", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmLandmarkID", typeof(string));

                            foreach (DataRow ittr in dsAlarmInfo.Tables[0].Rows)
                            {
                                // LOCALIZATION
                                object[] objRow = new object[dsCurrentAlarm.Tables[0].Columns.Count];
                                objRow[0] = ittr["AlarmId"].ToString();
                                objRow[1] = dbUser.GetUserLocalTime(userId, Convert.ToDateTime(ittr["DateTimeCreated"])).ToString();

                                if (ittr["DateTimeAck"] == DBNull.Value)
                                    objRow[2] = " ";
                                else
                                    objRow[2] = dbUser.GetUserLocalTime(userId, Convert.ToDateTime(ittr["DateTimeAck"])).ToString();

                                if (ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[3] = " ";
                                else
                                    objRow[3] = dbUser.GetUserLocalTime(userId, Convert.ToDateTime(ittr["DateTimeClosed"])).ToString();

                                if (ittr["DateTimeAck"] == DBNull.Value && ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[4] = VLF.CLS.Def.Enums.AlarmState.New.ToString();
                                else if (ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[4] = VLF.CLS.Def.Enums.AlarmState.Accepted.ToString();
                                else
                                    objRow[4] = VLF.CLS.Def.Enums.AlarmState.Closed.ToString();
                                objRow[5] = ((VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt32(ittr["AlarmLevel"])).ToString();
                                objRow[6] = ittr["UserId"].ToString();
                                objRow[7] = ittr["vehicleDescription"].ToString();
                                objRow[8] = ittr["AlarmDescription"].ToString();
                                objRow[9] = ittr["VehicleId"].ToString();
                                string streetAddress = ittr["StreetAddress"].ToString().TrimEnd();
                                if (Convert.ToInt16(ittr["ValidGPS"]) == 0) // 0 - valid, 1 -invalid
                                {
                                    // If street address is empty, get it from database and update property
                                    if (streetAddress == "")
                                    {
                                        // TODO: retrieves from map engine			
                                    }
                                }
                                else
                                {
                                    streetAddress = VLF.CLS.Def.Const.noValidAddress;
                                }
                                objRow[10] = streetAddress;
                                objRow[11] = ittr["UserName"].ToString();
                                objRow[12] = ittr["Latitude"].ToString();
                                objRow[13] = ittr["Longitude"].ToString();
                                objRow[14] = ittr["Speed"].ToString();
                                objRow[15] = ittr["Heading"].ToString();
                                objRow[16] = ittr["SensorMask"].ToString();
                                objRow[17] = ittr["IsArmed"].ToString();
                                objRow[18] = ittr["BoxId"].ToString();
                                if (ittr["AlarmLandmarkID"] == DBNull.Value)
                                    objRow[20] = "";
                                else
                                    objRow[20] = ittr["AlarmLandmarkID"].ToString();
                                AlarmBoxId = ittr["BoxId"].ToString();
                                AlarmVehicleDescription = ittr["vehicleDescription"].ToString();
                                AlarmLon = ittr["Longitude"].ToString();
                                AlarmLat = ittr["Latitude"].ToString();
                                dsCurrentAlarm.Tables[0].Rows.Add(objRow);
                            }
                            xml = dsCurrentAlarm.GetXml();
                        }

                    } // using( User dbUser =

                } // using( Alarm dbAlarm =

                //LogFinal("<< GetCurrentAlarmInfoXML(uId={0}, alarmId={1}, tSpan={2})",
                  //               userId, alarmId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return 0;//(int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                //LogException("<< GetCurrentAlarmInfoXML : uId={0}, EXC={1}", userId, Ex.Message);
                //return (int)ASIErrorCheck.CheckError(Ex);
                return 6; //Db Error
            }
        }

#endregion RemoveWhenMVComes

        // Changes for TimeZone Feature start
        private void AlarmInfoLoad_NewTZ(int AlarmId)
        {

            try
            {

                DataSet ds = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();

                //if (objUtil.ErrCheck(alarms.GetCurrentAlarmInfoXML(sn.UserID, sn.SecId, AlarmId, ref xml), false))
                //  if (objUtil.ErrCheck(alarms.GetCurrentAlarmInfoXML(sn.UserID, sn.SecId, AlarmId, ref xml), true))
                if (objUtil.ErrCheck(GetCurrentAlarmInfoXMLDirect_NewTZ(sn.UserID, sn.SecId, AlarmId, ref xml), false))
                    if (objUtil.ErrCheck(GetCurrentAlarmInfoXMLDirect_NewTZ(sn.UserID, sn.SecId, AlarmId, ref xml), true))
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

                this.AlarmID.Text = ds.Tables[0].Rows[0]["AlarmId"].ToString().TrimEnd();
                this.AlarmDescription.Text = ds.Tables[0].Rows[0]["AlarmDescription"].ToString().TrimEnd();
                this.AlarmState.Text = ds.Tables[0].Rows[0]["AlarmState"].ToString().TrimEnd();
                this.Latitude.Text = ds.Tables[0].Rows[0]["Latitude"].ToString().TrimEnd();
                this.Longitude.Text = ds.Tables[0].Rows[0]["Longitude"].ToString().TrimEnd();
                //if (ds.Tables[0].Rows[0]["AlarmLevel"].ToString().TrimEnd() == "Critical")
                //{
                //    this.lblAlarmDesc.ForeColor = Color.Red;
                //    this.lblAlarmLevel.ForeColor = Color.Red;
                //    this.extAlarmState.ForeColor = Color.Red;
                //    this.lblStreetAddress.ForeColor = Color.Red;
                //    this.lblTimeAccepted.ForeColor = Color.Red;
                //    this.lblTimeCreated.ForeColor = Color.Red;
                //    this.lblVehicleDesc.ForeColor = Color.Red;
                //    this.lblOperatorName.ForeColor = Color.Red;
                //}

                this.AlarmSeverity.Text = ds.Tables[0].Rows[0]["AlarmLevel"].ToString().TrimEnd();
                this.StreetAddress.Text = ds.Tables[0].Rows[0]["StreetAddress"].ToString().TrimEnd();
                this.NearestLandmark.Text = ds.Tables[0].Rows[0]["NearestLandmark"].ToString().TrimEnd();
                this.Fleets.Text = ds.Tables[0].Rows[0]["Fleets"].ToString().TrimEnd();

                //if ((ds.Tables[0].Rows[0]["TimeAccepted"].ToString().TrimEnd() != "1/1/0001 12:00:00 AM") && (ds.Tables[0].Rows[0]["TimeAccepted"].ToString().TrimEnd() != "01/01/1900 12:00:00 AM"))
                //    this.TimeAccepted.Text = ds.Tables[0].Rows[0]["TimeAccepted"].ToString().TrimEnd();
                //else
                //    this.TimeAccepted.Text = "";

                this.TimeCreated.Text = ds.Tables[0].Rows[0]["TimeCreated"].ToString().TrimEnd();
                this.VehicleDescription.Text = ds.Tables[0].Rows[0]["vehicleDescription"].ToString().TrimEnd();
                this.VehicleId.Text = ds.Tables[0].Rows[0]["VehicleId"].ToString().TrimEnd();
                //this.OperatorName.Text = ds.Tables[0].Rows[0]["UserName"].ToString().TrimEnd();
                //this.ExtraNotes.Text = VLF.CLS.Util.GetDTCCodes(ds.Tables[0].Rows[0]["CustomProp"].ToString());

                AlarmLandmarkID = ds.Tables[0].Rows[0]["AlarmLandmarkID"].ToString().Trim();
                AlarmDescriptionText = ds.Tables[0].Rows[0]["AlarmDescription"].ToString().TrimEnd();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }
        // Changes for TimeZone Feature start
        

        private void AlarmInfoLoad(int AlarmId)
        {

            try
            {

                DataSet ds = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();

                //if (objUtil.ErrCheck(alarms.GetCurrentAlarmInfoXML(sn.UserID, sn.SecId, AlarmId, ref xml), false))
                  //  if (objUtil.ErrCheck(alarms.GetCurrentAlarmInfoXML(sn.UserID, sn.SecId, AlarmId, ref xml), true))
                if (objUtil.ErrCheck(GetCurrentAlarmInfoXMLDirect(sn.UserID, sn.SecId, AlarmId, ref xml), false))
                    if (objUtil.ErrCheck(GetCurrentAlarmInfoXMLDirect(sn.UserID, sn.SecId, AlarmId, ref xml), true))
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

                this.AlarmID.Text = ds.Tables[0].Rows[0]["AlarmId"].ToString().TrimEnd();
                this.AlarmDescription.Text = ds.Tables[0].Rows[0]["AlarmDescription"].ToString().TrimEnd();
                this.AlarmState.Text = ds.Tables[0].Rows[0]["AlarmState"].ToString().TrimEnd();
                this.Latitude.Text = ds.Tables[0].Rows[0]["Latitude"].ToString().TrimEnd();
                this.Longitude.Text = ds.Tables[0].Rows[0]["Longitude"].ToString().TrimEnd();
                //if (ds.Tables[0].Rows[0]["AlarmLevel"].ToString().TrimEnd() == "Critical")
                //{
                //    this.lblAlarmDesc.ForeColor = Color.Red;
                //    this.lblAlarmLevel.ForeColor = Color.Red;
                //    this.extAlarmState.ForeColor = Color.Red;
                //    this.lblStreetAddress.ForeColor = Color.Red;
                //    this.lblTimeAccepted.ForeColor = Color.Red;
                //    this.lblTimeCreated.ForeColor = Color.Red;
                //    this.lblVehicleDesc.ForeColor = Color.Red;
                //    this.lblOperatorName.ForeColor = Color.Red;
                //}

                this.AlarmSeverity.Text = ds.Tables[0].Rows[0]["AlarmLevel"].ToString().TrimEnd();
                this.StreetAddress.Text = ds.Tables[0].Rows[0]["StreetAddress"].ToString().TrimEnd();

                //if ((ds.Tables[0].Rows[0]["TimeAccepted"].ToString().TrimEnd() != "1/1/0001 12:00:00 AM") && (ds.Tables[0].Rows[0]["TimeAccepted"].ToString().TrimEnd() != "01/01/1900 12:00:00 AM"))
                //    this.TimeAccepted.Text = ds.Tables[0].Rows[0]["TimeAccepted"].ToString().TrimEnd();
                //else
                //    this.TimeAccepted.Text = "";

                this.TimeCreated.Text = ds.Tables[0].Rows[0]["TimeCreated"].ToString().TrimEnd();
                this.VehicleDescription.Text = ds.Tables[0].Rows[0]["vehicleDescription"].ToString().TrimEnd();
                this.VehicleId.Text = ds.Tables[0].Rows[0]["VehicleId"].ToString().TrimEnd();
                //this.OperatorName.Text = ds.Tables[0].Rows[0]["UserName"].ToString().TrimEnd();
                //this.ExtraNotes.Text = VLF.CLS.Util.GetDTCCodes(ds.Tables[0].Rows[0]["CustomProp"].ToString());

                AlarmLandmarkID = ds.Tables[0].Rows[0]["AlarmLandmarkID"].ToString().Trim();
                AlarmDescriptionText = ds.Tables[0].Rows[0]["AlarmDescription"].ToString().TrimEnd();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }



        protected void cmdAccept_Click(object sender, System.EventArgs e)
        //protected void cmdAccept_Click(object sender, DirectEventArgs e)
        {
            try
            {

		        if (!string.IsNullOrEmpty(this.ExtraNotes.Text))
                {

                    DataSet ds = new DataSet();

                    int alarmid = Convert.ToInt32(ViewState["AlarmId"]);
                    if (alarmid == -1 || alarmid == -2)
                    {
                        string[] sl = ViewState["alarmlist"].ToString().Split(',');
                        foreach (string s in sl)
                        {
                            int ia = 0;
                            int.TryParse(s, out ia);
                            if (ia > 0)
                            {
                                if (!acceptalarm(ia))
                                    return;
                            }
                        }
                    }
                    else
                    {
                        if (!acceptalarm(alarmid))
                            return;
                    }
                    
                    //MapIt();
                    //System.Threading.Thread.Sleep(2000);
                    //sn.Map.ReloadMap = true;


                    string str = "";
                    int AlarmCount = 0;

                    sn.Map.LoadAlarmsXML_NewTZ(sn, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref str, ref AlarmCount);
                    sn.Map.AlarmsXML = str;
                    sn.Map.AlarmCount = AlarmCount;

		            Response.Write("<script language='javascript'>window.opener.document.location.reload()</script>");
                    Response.Write("<script language='javascript'>window.close()</script>");
		        }
                else
                {
                    int aid = Convert.ToInt32(ViewState["AlarmId"]);
                    if(aid==-1 || aid==-2)
                        Response.Write("<script language='javascript'>alert('Please provide ExtraNotes for clearing alarms.')</script>");
                    else
                        Response.Write("<script language='javascript'>alert('Please provide ExtraNotes for accepting this Alarm.')</script>");
                }

            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        private bool acceptalarm(int alarmid)
        {
	    string extranote = this.ExtraNotes.Text.Replace("'", "''");
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
            if (objUtil.ErrCheck(alarms.AcceptCurrentAlarmWithNotes(sn.UserID, sn.SecId, alarmid,extranote ), false))
                if (objUtil.ErrCheck(alarms.AcceptCurrentAlarmWithNotes(sn.UserID, sn.SecId, alarmid,extranote ), true))
                {
                    //this.StatusBar1.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AcceptAlarmFailed");
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Alarm Accept failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    return false;
                }

           // string extranote = this.ExtraNotes.Text.Replace("'", "''");
            
            //if (objUtil.ErrCheck(alarms.CloseCurrentAlarmWithNotes(sn.UserID, sn.SecId, alarmid, extranote), false))
              //  if (objUtil.ErrCheck(alarms.CloseCurrentAlarmWithNotes(sn.UserID, sn.SecId, alarmid, extranote), true))
               // {
                 //   //this.StatusBar1.Text = (string)base.GetLocalResourceObject("lblMessage_Text_CloseAlarmFailed");
                   // return false;
               // }

            return true;
        }




        //private void MapIt()
        //{
        //    try
        //    {

        //        DataSet ds = new DataSet();
        //        ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
        //        StringReader strrXML = null;
        //        string xml = "";

        //        if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(this.VehicleId.Text), ref xml), false))
        //            if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(this.VehicleId.Text), ref xml), true))
        //            {
        //                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "GetFleetsInfoXMLByVehicleId. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        //                return;
        //            }


        //        strrXML = new StringReader(xml);


        //        if (xml == "")
        //        {
        //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "GetFleetsInfoXMLByVehicleId - Empty. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        //            return;
        //        }

        //        ds.ReadXml(strrXML);


        //        if (ds.Tables.Count == 0)
        //        {
        //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Fleet Table  - Empty. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        //            return;
        //        }

        //        // Set FleetId
        //        sn.Map.SelectedFleetID = Convert.ToInt32(ds.Tables[0].Rows[0]["FleetId"]);
        //        DsFleetInfo_Fill(sn.Map.SelectedFleetID);
        //        // Set checkbox for vehicle
        //        foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
        //        {
        //            if (this.VehicleId.Text == rowItem["VehicleId"].ToString())
        //            {
        //                rowItem["chkBoxShow"] = true;
        //                break;
        //            }
        //            else
        //                rowItem["chkBoxShow"] = false;

        //        }

        //    }
        //    catch (Exception Ex)
        //    {
        //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        //    }

        //}


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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }       

         protected void cmdMapIt_Click(object sender, System.EventArgs e)
        //protected void cmdMapIt_Click(object sender, DirectEventArgs e)
         {
            try
            {
                Dictionary<string, string> vehicleDic = new Dictionary<string, string>();

                //MapIt();
                //System.Threading.Thread.Sleep(2000);
                //sn.Map.ReloadMap = true;
                //LoadMap();
                 vehicleDic.Add("id", this.VehicleId.Text);
                        vehicleDic.Add("date",this.TimeCreated.Text);
                        vehicleDic.Add("lat",  this.Latitude.Text);
                        vehicleDic.Add("lon", this.Longitude.Text);
                        vehicleDic.Add("desc", this.VehicleDescription.Text);   
                        vehicleDic.Add("icon", "RedCircle.ico");     
                        vehicleDic.Add("head", this.Heading.Text);
                        vehicleDic.Add("chkPTO", string.Empty);
                        vehicleDic.Add("spd", this.Speed.Text);
                        vehicleDic.Add("addr", this.StreetAddress.Text);
                        vehicleDic.Add("stat", this.AlarmDescription.Text);
                        ArrayList vehicleMain = new ArrayList();
                        vehicleMain.Add(vehicleDic);
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        string parameters = js.Serialize(vehicleMain);
                     
                Response.Write("<script language='javascript'>opener.parent.frmFleetInfo.MapAlarm('" + parameters + "')</script>");
                Response.Write("<script language='javascript'>window.close()</script>");
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
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