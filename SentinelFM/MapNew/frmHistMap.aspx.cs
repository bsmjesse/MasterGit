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
using System.Configuration;
using System.IO;
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace SentinelFM
{

    public partial class MapNew_frmHistMap : SentinelMapBasePage
    {
        public string errorSave = "Failed to load map.";
        Boolean isDrawLabels = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (sn.Map.MapSearch)
                {
                    return;
                }
            }

        }

        [WebMethod]
        public static string LoadHistoryData()
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            ArrayList vehicleMain = new ArrayList();
            try
            {
                string IconType = sn.History.IconTypeName;
                string UnitOfMes = sn.User.UnitOfMes == 1 ? "km/h" : "mi/h";

                if (sn.History.VehicleId == 0)
                {
                    IconType = "Circle";
                    sn.History.IconTypeName = "Circle";
                }

                if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables.Count > 0))
                {

                    //map.BreadCrumbPoints.DrawLabels = false;

                    if (!sn.History.ShowStops && !sn.History.ShowStopsAndIdle && !sn.History.ShowIdle)
                    {
                        if ((!sn.History.ShowBreadCrumb) || (sn.History.VehicleId == 0))
                        {
                            DataTable dt = new DataTable();
                            if (sn.History.ShowTrips)
                            {
                                if (sn.History.DsHistoryInfo.Tables[0].Rows.Count == 0) //no trips 
                                    return string.Empty ;

                                dt = sn.History.DsHistoryInfo.Tables[1];
                            }
                            else
                                dt = sn.History.DsHistoryInfo.Tables[0];

                            foreach (DataRow dr in dt.Rows)
                            {
                                Double lon = 0;
                                Double lat = 0;
                                Double.TryParse(dr["Longitude"].ToString().Trim(), out lon);
                                Double.TryParse(dr["Latitude"].ToString().Trim(), out lat);

                                if (Convert.ToBoolean(dr["chkBoxShow"]) && (dr["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (dr["Longitude"].ToString().TrimEnd() != VLF.CLS.Def.Const.blankValue) && (lon != 0 && lat != 0))
                                {
                                    Dictionary<string, string> vehicleDic = new Dictionary<string, string>();
                                    vehicleDic.Add("id", dr["VehicleId"] is DBNull ? string.Empty : dr["VehicleId"].ToString());
                                    //vehicleDic.Add("date", dr["OriginDateTime"] is DBNull ? string.Empty : dr["OriginDateTime"].ToString());
                                    vehicleDic.Add("lat", dr["Latitude"] is DBNull ? string.Empty : dr["Latitude"].ToString());
                                    vehicleDic.Add("lon", dr["Longitude"] is DBNull ? string.Empty : dr["Longitude"].ToString());
                                    //vehicleDic.Add("spd", dr["Speed"] is DBNull ? string.Empty : dr["Speed"].ToString());
                                    //vehicleDic.Add("head", dr["MyHeading"] is DBNull ? string.Empty : dr["MyHeading"].ToString());
                                    //vehicleDic.Add("addr", dr["StreetAddress"] is DBNull ? string.Empty : dr["StreetAddress"].ToString());
                                    //try
                                    //{
                                    //    vehicleDic.Add("stat", dr["Remarks"] is DBNull ? string.Empty : dr["Remarks"].ToString());
                                    //}
                                    //catch {
                                    //    vehicleDic.Add("stat", "N");
                                    //}
                                    string icon = string.Empty;

                                    if (dr["MsgDetails"].ToString().Contains("PTO") && sn.User.OrganizationId != 343)
                                        icon = "Blue" + IconType + ".ico";
                                    else if (dr["Speed"].ToString() != "0")
                                        icon = "Green" + IconType + dr["MyHeading"].ToString().TrimEnd() + ".ico";
                                    else
                                        icon = "Red" + IconType + ".ico";
                                    vehicleDic.Add("icon", icon);
                                    vehicleDic.Add("trail", "0");


                                    string toolTip = string.Empty;
                                    if (dr["MsgDetails"].ToString().TrimEnd() != "")
                                    {
                                        //if (sn.Map.VehiclesToolTip.Length == 0)
                                        //    sn.Map.VehiclesToolTip += "<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + " " + dr["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        //else
                                        //    sn.Map.VehiclesToolTip += "\"<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + " " + dr["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        //toolTip = "<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + " " + dr["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline; font-weight:bold;' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes;
                                        toolTip = "<B>" + " [" + Convert.ToDateTime(dr["MyDateTime"]).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat).TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + " " + dr["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline; font-weight:bold;' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes;
                                       
                                    }
                                    else
                                    {
                                        //if (sn.Map.VehiclesToolTip.Length == 0)
                                        //    sn.Map.VehiclesToolTip += "<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + dr["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        //else
                                        //    sn.Map.VehiclesToolTip += "\"<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + dr["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        //toolTip = "<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + dr["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes;
                                        toolTip = "<B>" + " [" + Convert.ToDateTime(dr["MyDateTime"]).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat).TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + dr["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes;
                                    }
                                    vehicleDic.Add("toolTip", toolTip);
                                    vehicleMain.Add(vehicleDic);
                                }
                            }
                        }
                        else
                        {

                            DataTable dt = new DataTable();
                            if (sn.History.ShowTrips)
                            {
                                if (sn.History.DsHistoryInfo.Tables[0].Rows.Count == 0) //no trips 
                                    return string.Empty ;

                                dt = sn.History.DsHistoryInfo.Tables[1];
                            }
                            else
                                dt = sn.History.DsHistoryInfo.Tables[0];

                            foreach (DataRow dr in dt.Rows)
                            {
                                Double lon = 0;
                                Double lat = 0;
                                Double.TryParse(dr["Longitude"].ToString().Trim(), out lon);
                                Double.TryParse(dr["Latitude"].ToString().Trim(), out lat);

                                if (Convert.ToBoolean(dr["chkBoxShow"]) && (dr["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (dr["Longitude"].ToString().TrimEnd() != VLF.CLS.Def.Const.blankValue) && (lon != 0 && lat != 0))
                                {
                                    Dictionary<string, string> vehicleDic = new Dictionary<string, string>();
                                    vehicleDic.Add("id", dr["VehicleId"] is DBNull ? string.Empty : dr["VehicleId"].ToString());
                                    //vehicleDic.Add("date", dr["OriginDateTime"] is DBNull ? string.Empty : dr["OriginDateTime"].ToString());
                                    vehicleDic.Add("lat", dr["Latitude"] is DBNull ? string.Empty : dr["Latitude"].ToString());
                                    vehicleDic.Add("lon", dr["Longitude"] is DBNull ? string.Empty : dr["Longitude"].ToString());
                                    //vehicleDic.Add("spd", dr["Speed"] is DBNull ? string.Empty : dr["Speed"].ToString());
                                    //vehicleDic.Add("head", dr["MyHeading"] is DBNull ? string.Empty : dr["MyHeading"].ToString());
                                    //vehicleDic.Add("addr", dr["StreetAddress"] is DBNull ? string.Empty : dr["StreetAddress"].ToString());
                                    //try
                                    //{
                                    //    vehicleDic.Add("stat", dr["Remarks"] is DBNull ? string.Empty : dr["Remarks"].ToString());
                                    //}
                                    //catch
                                    //{
                                    //    vehicleDic.Add("stat", "N");
                                    //}

                                    string icon = string.Empty;
                                    if (dr["MsgDetails"].ToString().Contains("PTO") && sn.User.OrganizationId != 343)
                                        icon = "Blue" + sn.History.IconTypeName + ".ico";
                                    else if (dr["Speed"].ToString() != "0")
                                        icon = "Green" + sn.History.IconTypeName + dr["MyHeading"].ToString().TrimEnd() + ".ico";
                                    else
                                        icon = "Red" + sn.History.IconTypeName + ".ico";
                                    vehicleDic.Add("icon", icon);
                                    vehicleDic.Add("trail", "1");
                                    string toolTip = string.Empty;
                                    if (dr["MsgDetails"].ToString().TrimEnd() != "")
                                    {
                                        //if (sn.Map.VehiclesToolTip.Length == 0)
                                        //    sn.Map.VehiclesToolTip += "<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + " " + dr["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        //else
                                        //    sn.Map.VehiclesToolTip += "\"<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + " " + dr["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        toolTip = "<B>" + " [" + Convert.ToDateTime(dr["MyDateTime"]).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat).TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + " " + dr["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes;
                                    }
                                    else
                                    {
                                        //if (sn.Map.VehiclesToolTip.Length == 0)
                                        //    sn.Map.VehiclesToolTip += "<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + dr["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        //else
                                        //    sn.Map.VehiclesToolTip += "\"<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + dr["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        toolTip = "<B>" + " [" + Convert.ToDateTime(dr["MyDateTime"]).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat).TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + dr["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes;
                                    }
                                    vehicleDic.Add("toolTip", toolTip);

                                    vehicleMain.Add(vehicleDic);

                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (DataRow dr in sn.History.DsHistoryInfo.Tables["StopData"].Rows)
                        {
                            TimeSpan TripIndling;
                            TripIndling = new TimeSpan(Convert.ToInt64(dr["StopDurationVal"]) * TimeSpan.TicksPerSecond);
                            double StopDurationVal = TripIndling.TotalMinutes;
                            string icon = "";

                            Double lon = 0;
                            Double lat = 0;
                            Double.TryParse(dr["Longitude"].ToString().Trim(), out lon);
                            Double.TryParse(dr["Latitude"].ToString().Trim(), out lat);

                            if (Convert.ToBoolean(dr["chkBoxShow"]) && (lon != 0 && lat != 0))
                            {
                                if (dr["Remarks"].ToString() == "Idling")
                                    icon = "Idle.ico";
                                else if (StopDurationVal < 15)
                                    icon = "Stop_3.ico";
                                else if ((StopDurationVal > 15) && (StopDurationVal < 60))
                                    icon = "Stop_15.ico";
                                else if (StopDurationVal > 60)
                                    icon = "Stop_60.ico";

                                Dictionary<string, string> vehicleDic = new Dictionary<string, string>();
                                vehicleDic.Add("id", dr["VehicleId"] is DBNull ? string.Empty : dr["VehicleId"].ToString());
                                //vehicleDic.Add("date", dr["OriginDateTime"] is DBNull ? string.Empty : dr["OriginDateTime"].ToString());
                                vehicleDic.Add("lat", dr["Latitude"] is DBNull ? string.Empty : dr["Latitude"].ToString());
                                vehicleDic.Add("lon", dr["Longitude"] is DBNull ? string.Empty : dr["Longitude"].ToString());
                                //vehicleDic.Add("spd", dr["Speed"] is DBNull ? string.Empty : dr["Speed"].ToString());
                                //vehicleDic.Add("head", dr["MyHeading"] is DBNull ? string.Empty : dr["MyHeading"].ToString());
                                //vehicleDic.Add("addr", dr["StreetAddress"] is DBNull ? string.Empty : dr["StreetAddress"].ToString());
                                //try
                                //{
                                //    vehicleDic.Add("stat", dr["Remarks"] is DBNull ? string.Empty : dr["Remarks"].ToString());
                                //}
                                //catch
                                //{
                                //    vehicleDic.Add("stat", "N");
                                //}

                                //if (dr["MsgDetails"].ToString().Contains("PTO") && sn.User.OrganizationId != 343)
                                //    icon = "Blue" + sn.History.IconTypeName + ".ico";
                                //else if (dr["Speed"].ToString() != "0")
                                //    icon = "Green" + sn.History.IconTypeName + dr["MyHeading"].ToString().TrimEnd() + ".ico";
                                //else
                                //    icon = "Red" + sn.History.IconTypeName + ".ico";
                                vehicleDic.Add("icon", icon);
                                vehicleDic.Add("trail", "0");

                                string toolTip = string.Empty;
                                dr["Location"] = dr["Location"].ToString().TrimEnd().Replace(Convert.ToString(Convert.ToChar(13)), " ").Replace(Convert.ToString(Convert.ToChar(10)), " ");

                                //if (sn.Map.VehiclesToolTip.Length == 0)
                                //    sn.Map.VehiclesToolTip += "<B>" + " [" + rowItem["StopIndex"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + rowItem["Remarks"].ToString().TrimEnd() + " (" + rowItem["StopDuration"].ToString().TrimEnd() + ") ," + rowItem["ArrivalDateTime"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["Location"].ToString().TrimEnd() + "<BR>\",";
                                //else
                                //    sn.Map.VehiclesToolTip += "\"<B>" + " [" + rowItem["StopIndex"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + rowItem["Remarks"].ToString().TrimEnd() + " (" + rowItem["StopDuration"].ToString().TrimEnd() + ") ," + rowItem["ArrivalDateTime"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["Location"].ToString().TrimEnd() + "<BR> \",";
                                toolTip = "<B>" + " [" + dr["StopIndex"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + dr["Remarks"].ToString().TrimEnd() + " (" + dr["StopDuration"].ToString().TrimEnd() + ") ," + dr["ArrivalDateTime"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' > Address:</FONT> " + dr["Location"].ToString().TrimEnd();
                                vehicleDic.Add("toolTip", toolTip);
                                vehicleMain.Add(vehicleDic);

                            }
                        }
                    }
                }

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                return "0";
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmHistMap.aspx"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                return "0";
            }
            if (vehicleMain.Count > 0)
            {

                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = int.MaxValue;
                string json = js.Serialize(vehicleMain);
                return json;
            }
            else return string.Empty;
        }
    }
}