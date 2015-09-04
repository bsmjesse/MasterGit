using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SentinelFM;
using System.Web.Services;
using System.Web.Script.Services;
using VLF.CLS.Interfaces;
using System.Collections;
using System.Web.Script.Serialization;

public partial class Ant_AntLoadData : System.Web.UI.Page
{
    private static string hosConnectionString =
       ConfigurationManager.ConnectionStrings["SentinelHOSConnectionString"].ConnectionString;
    static string loadingSession = "loadingSession";
    static string dumpSession = "dumpSession";
    static string emergencySession = "emergencySession";
    static string pendingStr = "Pending";
    static string succeedStr = "Succeed";
    static string failStr = "Fail";
    protected void Page_Load(object sender, EventArgs e)
    {
        SentinelFMSession sn = null;
        try
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn == null) return;
            if (Request.QueryString["type"] != null)
            {
                if (Request.QueryString["type"] == "GetAllLoadingUnits" && Request.QueryString["fleetid"] != null)
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        try
                        {
                            SqlConnection connection = new SqlConnection(hosConnectionString);
                            adapter.SelectCommand = new SqlCommand();
                            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                            adapter.SelectCommand.CommandText = "usp_ant_GetAllLoadingUnitByFleet";
                            adapter.SelectCommand.Connection = connection;

                            SqlParameter sqlPara = new SqlParameter("@FleetId", SqlDbType.Int);
                            sqlPara.Value = int.Parse(Request.QueryString["fleetid"].ToString());
                            adapter.SelectCommand.Parameters.Add(sqlPara);
                            adapter.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                DataRow dr = dt.NewRow();
                                dr["Boxid"] = 0;
                                dr["Location"] = "";
                                dr["VehicleID"] = 0;
                                dr["Description"] = "All Vehicles";
                                dr["eId"] = 2;
                                dt.Rows.InsertAt(dr, 0);
                            }

                            System.IO.StringWriter writer = new System.IO.StringWriter();
                            dt.TableName = "LoadingUnit";
                            dt.WriteXml(writer, XmlWriteMode.IgnoreSchema, false);
                            string xmlFromDataTable = writer.ToString();

                            Response.ContentType = "text/xml";
                            //byte[] data = Encoding.Default.GetBytes(xml.Trim());
                            //xml = Encoding.UTF8.GetString(data);
                            Response.Write(xmlFromDataTable);

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        }
                    }

                    catch (Exception ex) {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }
                }

                //if (Request.QueryString["type"] == "SendSyncRequest" && Request.QueryString["boxid"] != null)
                //{ 
                    
                //}

                if (Request.QueryString["type"] == "GetAllDumpLocations" && Request.QueryString["fleetid"] != null)
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        try
                        {
                            SqlConnection connection = new SqlConnection(hosConnectionString);
                            adapter.SelectCommand = new SqlCommand();
                            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                            adapter.SelectCommand.CommandText = "usp_ant_GetAllDumpingLocationByFleet";
                            adapter.SelectCommand.Connection = connection;

                            SqlParameter sqlPara = new SqlParameter("@FleetId", SqlDbType.Int);
                            sqlPara.Value = int.Parse(Request.QueryString["fleetid"].ToString());
                            adapter.SelectCommand.Parameters.Add(sqlPara);
                            adapter.Fill(dt);
                            System.IO.StringWriter writer = new System.IO.StringWriter();
                            dt.TableName = "DumpingLocation";
                            dt.WriteXml(writer, XmlWriteMode.IgnoreSchema, false);
                            string xmlFromDataTable = writer.ToString();

                            Response.ContentType = "text/xml";
                            //byte[] data = Encoding.Default.GetBytes(xml.Trim());
                            //xml = Encoding.UTF8.GetString(data);
                            Response.Write(xmlFromDataTable);

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        }
                    }

                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
        UseHttpGet = false, XmlSerializeString = false)]
    public static string UpdateLoadingUnit(int box, int vehicle, string location)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

        try
        {
            //int box_Id = int.Parse(Request.QueryString["box"].ToString());
            //int vehicle_Id = int.Parse(Request.QueryString["vehicle"].ToString());
            //string loading_Location = Request.QueryString["loading"].ToString();
            using (SqlConnection conn = new SqlConnection(hosConnectionString))
            {
                using (SqlCommand sqlComm = new SqlCommand())
                {
                    sqlComm.Connection = conn;
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    sqlComm.CommandText = "usp_ant_UpdateLoadingUnit";
                    SqlParameter para = new SqlParameter("@Box_Id", SqlDbType.Int);
                    para.Value = box;
                    sqlComm.Parameters.Add(para);

                    para = new SqlParameter("@Vehicle_Id", SqlDbType.Int);
                    para.Value = vehicle;
                    sqlComm.Parameters.Add(para);

                    para = new SqlParameter("@Loading_Location", SqlDbType.VarChar);
                    para.Value = location;
                    sqlComm.Parameters.Add(para);

                    para = new SqlParameter("@UserId", SqlDbType.VarChar);
                    para.Value = sn.UserID;
                    sqlComm.Parameters.Add(para);

                    para = new SqlParameter("@OrganizationId", SqlDbType.Int);
                    para.Value = sn.User.OrganizationId;
                    sqlComm.Parameters.Add(para);

                    conn.Open();
                    sqlComm.ExecuteNonQuery();
                    conn.Close();
                }

            }
        }
        catch (Exception ex)
        {
            return "0";
        }
        return "1";
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
        UseHttpGet = false, XmlSerializeString = false)]
    public static string UpdateDumpLocation(int Id, int Fleet, string location, bool IsDelete)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

        try
        {
            //int box_Id = int.Parse(Request.QueryString["box"].ToString());
            //int vehicle_Id = int.Parse(Request.QueryString["vehicle"].ToString());
            //string loading_Location = Request.QueryString["loading"].ToString();
            using (SqlConnection conn = new SqlConnection(hosConnectionString))
            {
                using (SqlCommand sqlComm = new SqlCommand())
                {
                    sqlComm.Connection = conn;
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    sqlComm.CommandText = "usp_ant_UpdateDumpLocation";
                    SqlParameter para = new SqlParameter("@Id", SqlDbType.Int);
                    para.Value = Id;
                    sqlComm.Parameters.Add(para);

                    para = new SqlParameter("@Fleet_Id", SqlDbType.Int);
                    para.Value = Fleet;
                    sqlComm.Parameters.Add(para);

                    para = new SqlParameter("@Dump_Location", SqlDbType.VarChar);
                    para.Value = location;
                    sqlComm.Parameters.Add(para);

                    para = new SqlParameter("@IsDelete", SqlDbType.Bit);
                    para.Value = IsDelete;
                    sqlComm.Parameters.Add(para);

                    para = new SqlParameter("@UserId", SqlDbType.VarChar);
                    para.Value = sn.UserID;
                    sqlComm.Parameters.Add(para);

                    para = new SqlParameter("@OrganizationId", SqlDbType.Int);
                    para.Value = sn.User.OrganizationId;
                    sqlComm.Parameters.Add(para);


                    conn.Open();
                    sqlComm.ExecuteNonQuery();
                    conn.Close();
                }

            }
        }
        catch (Exception ex)
        {
            return "0";
        }
        return "1";
    }

    
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
        UseHttpGet = false, XmlSerializeString = false)]
    public static string CheckSendingData(String sendingType)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";
       
        DataTable dt = null;

        bool isSch = false;
        int interval = 300;
        int period = 86400;

        if (sendingType == "1")
        {
            dt = (DataTable)HttpContext.Current.Session[loadingSession];

            if (HttpContext.Current.Session[loadingSession + "_Sch"] != null)
            {
                if (HttpContext.Current.Session[loadingSession + "_Sch"].ToString() == "1") isSch = true;
            }

            if (HttpContext.Current.Session[loadingSession + "_Interval"] != null)
            {
               interval = int.Parse(HttpContext.Current.Session[loadingSession + "_Interval"].ToString());
            }

            if (HttpContext.Current.Session[loadingSession + "_Period"] != null)
            {
               period = int.Parse(HttpContext.Current.Session[loadingSession + "_Period"].ToString());
            }

        }
        if (sendingType == "2")
        {
            dt = (DataTable)HttpContext.Current.Session[dumpSession];
            if (HttpContext.Current.Session[dumpSession + "_Sch"] != null)
            {
                if (HttpContext.Current.Session[dumpSession + "_Sch"].ToString() == "1") isSch = true;
            }

            if (HttpContext.Current.Session[dumpSession + "_Interval"] != null)
            {
                interval = int.Parse(HttpContext.Current.Session[dumpSession + "_Interval"].ToString());
            }

            if (HttpContext.Current.Session[dumpSession + "_Period"] != null)
            {
                period = int.Parse(HttpContext.Current.Session[dumpSession + "_Period"].ToString());
            }

        }
        if (sendingType == "3")
        {
            dt = (DataTable)HttpContext.Current.Session[emergencySession];
            if (HttpContext.Current.Session[emergencySession + "_Sch"] != null)
            {
                if (HttpContext.Current.Session[emergencySession + "_Sch"].ToString() == "1") isSch = true;
            }

            if (HttpContext.Current.Session[emergencySession + "_Interval"] != null)
            {
                interval = int.Parse(HttpContext.Current.Session[emergencySession + "_Interval"].ToString());
                
            }

            if (HttpContext.Current.Session[emergencySession + "_Period"] != null)
            {
                period = int.Parse(HttpContext.Current.Session[emergencySession + "_Period"].ToString());
            }


        }
        Boolean ret = true;
        try{
            if (dt != null && dt.Rows.Count > 0)
            {
                clsUtility objUtil = new clsUtility(sn);
                SentinelFM.LocationMgr.Location dbl = new SentinelFM.LocationMgr.Location();
                foreach (DataRow rowItem in dt.Rows)
                {
                    if (rowItem["RetStatus"].ToString() != string.Empty)
                        continue;
                    if ((Convert.ToInt16(rowItem["Updated"]) != (short)CommandStatus.Ack) &&
                          (Convert.ToInt16(rowItem["Updated"]) != (short)CommandStatus.CommTimeout) &&
                          (Convert.ToInt16(rowItem["Updated"]) != (short)CommandStatus.Pending))
                    {
                        int cmdStatus = 0;
                        if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(rowItem["ProtocolId"]), ref cmdStatus), false))
                            if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(rowItem["ProtocolId"]), ref cmdStatus), true))
                            {
                                cmdStatus = (int)CommandStatus.CommTimeout;
                                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Session for Box: " + rowItem["BoxId"].ToString() + " and User: " + sn.UserID.ToString() + " does not exist. Form:frmNewMessageTimer.aspx"));
                            }

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
                                if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), sn.Cmd.CommandId, rowItem["Msg"].ToString(), ref ProtocolTypeId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false))
                                    if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), sn.Cmd.CommandId, rowItem["Msg"].ToString(), ref ProtocolTypeId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true))
                                    {
                                        ret = false;
                                    }




                                if (cmdSent)
                                {
                                    rowItem["ProtocolId"] = ProtocolTypeId;
                                    rowItem["RetStatus"] = succeedStr;
                                }
                                else ret = false;
                                continue;
                            }

                            rowItem["Updated"] = cmdStatus;

                            //Time Out Send Message
                            if (cmdStatus == (int)CommandStatus.Ack)
                            {
                                rowItem["RetStatus"] = succeedStr;
                            }

                            if (cmdStatus == (int)CommandStatus.Pending)
                                rowItem["RetStatus"] = "Penging";
                            if (cmdStatus == (int)CommandStatus.CommTimeout)
                            {

                                if (isSch)
                                {

                                    Int64 TaskId = 0;
                                    Int16 CommModeId = Convert.ToInt16(rowItem["ComModeId"]);
                                    Int16 ProtocolTypeId = Convert.ToInt16(rowItem["ProtocolId"]);

                                    if (objUtil.ErrCheck(dbl.SendAutomaticCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), DateTime.Now, rowItem["Msg"].ToString(), ref ProtocolTypeId, ref   CommModeId, period, interval, false, ref TaskId), false))
                                        if (objUtil.ErrCheck(dbl.SendAutomaticCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), DateTime.Now, rowItem["Msg"].ToString(), ref ProtocolTypeId, ref   CommModeId, period, interval, false, ref TaskId), true))
                                        {
                                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Sent Automatic command failed " + Convert.ToString((VLF.CLS.Def.Enums.CommMode)CommModeId) + ". User:" + sn.UserID.ToString() + " Form:frmNewMessageTimer.aspx"));
                                        }

                                    if (TaskId > 0)
                                    {
                                        rowItem["Updated"] = (short)CommandStatus.Ack;
                                        rowItem["RetStatus"] = pendingStr;
                                    }
                                    else ret = false;
                                }
                                else
                                {
                                    rowItem["RetStatus"] = failStr;
                                }
                            }
                        }
                        else
                        {
                            ret = false;
                        }
                    }
                }

            }
        }
        catch (Exception ex)
        {
            return "0";
        }

        string vehicles = string.Empty;
        if (ret)
        {
             ArrayList vehicleMain = new ArrayList();
             foreach (DataRow rowItem in dt.Rows)
             {
                 Dictionary<string, string> vehicleDic = new Dictionary<string, string>();
                 vehicleDic.Add("Vehicle", rowItem["VehicleDesc"].ToString());
                 vehicleDic.Add("Status", rowItem["RetStatus"].ToString());
                 vehicleMain.Add(vehicleDic);
             }
             JavaScriptSerializer js = new JavaScriptSerializer();
             js.MaxJsonLength = int.MaxValue;
             vehicles = js.Serialize(vehicleMain);
        }
        if (!ret) return "0";
        else return vehicles;
    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
        UseHttpGet = false, XmlSerializeString = false)]
    public static string SendSyncRequest(String[] boxids, int fleetId)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";
        string[] boxidArray = boxids[boxids.Length - 1].Split(',');
        bool allError = true;
        DataTable dt = sn.Message.DtSendMessageBoxes.Clone();
        dt.Columns.Add("Msg");
        dt.Columns.Add("RetStatus");
        try
        {
            for (int index = 0; index < boxidArray.Length; index++)
            {
                string ret = SendSyncRequest(fleetId, int.Parse(boxidArray[index]),
                   boxids[index], sn, "Sync LoadingUnit Request!", // + Convert.ToChar(1) + Convert.ToChar(1),
                   300, 86400
                   );
                if (ret == "")
                    allError = false;

                //if (ret == "")
                {
                    DataRow dr = dt.NewRow();
                    dr["BoxId"] = sn.Message.DtSendMessageBoxes.Rows[0]["BoxId"];
                    dr["VehicleDesc"] = sn.Message.DtSendMessageBoxes.Rows[0]["VehicleDesc"];
                    dr["Updated"] = -1;
                    dr["ComModeId"] = sn.Message.DtSendMessageBoxes.Rows[0]["ComModeId"];
                    dr["ProtocolId"] = sn.Message.DtSendMessageBoxes.Rows[0]["ProtocolId"];
                    dr["Msg"] = sn.Cmd.CommandParams;
                    dr["RetStatus"] = ret;
                    dt.Rows.Add(dr);
                }
            }

            HttpContext.Current.Session[loadingSession] = dt;

            HttpContext.Current.Session[loadingSession + "_Sch"] = "1";
            HttpContext.Current.Session[loadingSession + "_Interval"] = 300;
            HttpContext.Current.Session[loadingSession + "_Period"] = 86400;
            if (allError) return "0";
        }
        catch (Exception ex)
        {
            return "0";
        }
        return "1";
    }
    
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
        UseHttpGet = false, XmlSerializeString = false)]
    public static string SendMessageRequest(String[] boxids, int fleetId, string message,
        string schPeriod, string schInterval)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";
        string[] boxidArray = boxids[boxids.Length - 1].Split(',');
        bool allError = true;
        DataTable dt = sn.Message.DtSendMessageBoxes.Clone();
        dt.Columns.Add("Msg");
        dt.Columns.Add("RetStatus");
        try
        {
            for (int index = 0; index < boxidArray.Length; index++)
            {
                string ret = SendSyncRequest(fleetId, int.Parse(boxidArray[index]),
                   boxids[index], sn, "Emergency:" + message, // + Convert.ToChar(1) + Convert.ToChar(1),
                   int.Parse(schInterval), int.Parse(schPeriod)
                   );
                if (ret == "")
                    allError = false;

                //if (ret == "")
                {
                    DataRow dr = dt.NewRow();
                    dr["BoxId"] = sn.Message.DtSendMessageBoxes.Rows[0]["BoxId"];
                    dr["VehicleDesc"] = sn.Message.DtSendMessageBoxes.Rows[0]["VehicleDesc"];
                    dr["Updated"] = -1;
                    dr["ComModeId"] = sn.Message.DtSendMessageBoxes.Rows[0]["ComModeId"];
                    dr["ProtocolId"] = sn.Message.DtSendMessageBoxes.Rows[0]["ProtocolId"];
                    dr["Msg"] = sn.Cmd.CommandParams;
                    dr["RetStatus"] = ret;
                    dt.Rows.Add(dr);
                }
            }

            HttpContext.Current.Session[emergencySession] = dt;

            if (schPeriod != "0" && schInterval != "0")
            {
                HttpContext.Current.Session[emergencySession + "_Sch"] = "1";
                HttpContext.Current.Session[emergencySession + "_Interval"] = int.Parse(schInterval);
                HttpContext.Current.Session[emergencySession + "_Period"] = int.Parse(schPeriod);
            }
            else
            {
                HttpContext.Current.Session[emergencySession + "_Sch"] = "0";
            }
            if (allError) return "0";
        }
        catch (Exception ex)
        {
            return "0";
        }
        return "1";
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
        UseHttpGet = false, XmlSerializeString = false)]
    public static string SendSyncDumpRequest(String[] boxids, int fleetId)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";
        string[] boxidArray = boxids[boxids.Length - 1].Split(',');
        bool allError = true;
        DataTable dt = sn.Message.DtSendMessageBoxes.Clone();
        dt.Columns.Add("Msg");
        dt.Columns.Add("RetStatus");
        try
        {
            for (int index = 0; index < boxidArray.Length; index++)
            {
                string ret = SendSyncRequest(fleetId, int.Parse(boxidArray[index]),
                   boxids[index], sn, "Sync Dump Request!", // + Convert.ToChar(1) + Convert.ToChar(1),
                   300, 86400
                   );
                if (ret == "")
                    allError = false;

                //if (ret == "")
                {
                    DataRow dr = dt.NewRow();
                    dr["BoxId"] = sn.Message.DtSendMessageBoxes.Rows[0]["BoxId"];
                    dr["VehicleDesc"] = sn.Message.DtSendMessageBoxes.Rows[0]["VehicleDesc"];
                    dr["Updated"] = -1;
                    dr["ComModeId"] = sn.Message.DtSendMessageBoxes.Rows[0]["ComModeId"];
                    dr["ProtocolId"] = sn.Message.DtSendMessageBoxes.Rows[0]["ProtocolId"];
                    dr["Msg"] = sn.Cmd.CommandParams;
                    dr["RetStatus"] = ret;
                    dt.Rows.Add(dr);
                }
            }

            HttpContext.Current.Session[dumpSession] = dt;

            HttpContext.Current.Session[dumpSession + "_Sch"] = "1";
            HttpContext.Current.Session[dumpSession + "_Interval"] = 300;
            HttpContext.Current.Session[dumpSession + "_Period"] = 86400;
            if (allError) return "0";
        }
        catch (Exception ex)
        {
            return "0";
        }
        return "1";
    }

    private static string  SendSyncRequest(int fleetId, int boxId, string vehicleName,
        SentinelFMSession sn, string txtMessage, 
        int interval, int period)
    {
        string strMessage = "";
        string strResponse = "";

        sn.Message.DtSendMessageBoxes.Rows.Clear();
        sn.Message.DtSendMessageFails.Rows.Clear();

        //txtMessage = System.Convert.ToChar(13) + this.txtMessage.Text;
        //txtMessage = "\r\n" + this.txtMessage.Text;

        txtMessage = " " + txtMessage + "\r\n";


        strMessage = VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMessage, txtMessage);

        int EnterPos = 0;
        int TotalEnters = 0;


        while (strMessage.IndexOf("\n", EnterPos + 1) != -1)
        {
            EnterPos = strMessage.IndexOf("\n", EnterPos + 1);
            if (EnterPos != -1)
                TotalEnters++;
        }

        //if (TotalEnters > 5)
        //{
        //    this.lblMessage.Visible = true;
        //    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_TooManyLinesError");
        //    return;
        //}

        //if (this.cboVehicle.SelectedItem.Value == "-1")
        //{
        //    this.lblMessage.Visible = true;
        //    this.lblMessage.Text = (string)base.GetLocalResourceObject("cboVehicle_Item_01");
        //    return;
        //}

        //this.lblMessage.Text = "";
        //if (this.lslResponse.Items.Count > 0)
        //{
        //    foreach (ListItem li in this.lslResponse.Items)
        //    {
        //        strResponse += li.Text + "~";
        //    }
        //    strResponse = strResponse.Substring(0, strResponse.Length - 1);
        //    sn.Message.Response = strResponse;
        //}
        //else
        //{
        //    sn.Message.Response = "";
        //}

        sn.Message.Message = txtMessage;
        sn.Message.FleetId = fleetId;
        sn.Message.VehicleId = boxId;


        //this.lblMessage.Text = "";

        strMessage += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyAnswer, strResponse);

        SentinelFM.LocationMgr.Location dbl = new SentinelFM.LocationMgr.Location();


        bool cmdSent = false;


        DataRow dr;
        dr = sn.Message.DtSendMessageBoxes.NewRow();
        dr["BoxId"] = boxId;
        dr["VehicleDesc"] = vehicleName;
        dr["Updated"] = -1;
        sn.Message.DtSendMessageBoxes.Rows.Add(dr);
          //if (this.optCommMode.SelectedItem.Value == "2")
          //      sn.Cmd.DualComm = true;
          //  else
         sn.Cmd.DualComm = false;
         sn.Cmd.BoxId = boxId;
         sn.Cmd.CommandId = Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage);
            //------------------------------------------



        sn.Cmd.CommandParams = strMessage;
        sn.Cmd.CommModeId = -1;
        sn.Cmd.ProtocolTypeId = -1;

            sn.Cmd.SchCommand = true;
            sn.Cmd.SchInterval = interval;
            sn.Cmd.SchPeriod = period;


            //SentinelFMBasePage sbase = new SentinelFMBasePage();
            clsUtility objUtil = new clsUtility(sn);
        
        foreach (DataRow rowVehicle in sn.Message.DtSendMessageBoxes.Rows)
        {
            short ProtocolId = -1;
            short CommModeId = -1;
            Int64 sessionTimeOut = 0;
            //dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowVehicle["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), strMessage, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut);
            if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowVehicle["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), strMessage, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false))
                if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowVehicle["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), strMessage, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true))
                {
                    return failStr;
                }
            sn.Cmd.CommModeId = CommModeId;
            rowVehicle["ComModeId"] = CommModeId;
            rowVehicle["ProtocolId"] = ProtocolId;



        }



        sn.Message.MessageSent = true;
        sn.Message.TimerStatus = true;
        return string.Empty;
    }
}