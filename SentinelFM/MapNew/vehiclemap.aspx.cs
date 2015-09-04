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

namespace SentinelFM
{
    public partial class vehiclemap : SentinelMapBasePage
    {
        public VLF.MAP.ClientMapProxy map;
        //public int imageW = 600;
        //public int imageH = 325;
        public int AutoRefreshTimer = 60000;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (Request.QueryString["clientWidth"] != null)
                    sn.User.ScreenWidth = Convert.ToInt32(Request.QueryString["clientWidth"]) + 220;

                //imageW = Convert.ToInt32(sn.User.ScreenWidth) - 310;

                AutoRefreshTimer = sn.User.GeneralRefreshFrequency;



                if (!Page.IsPostBack)
                {

                    //LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmVehicleMapForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                    //GuiSecurity(this);
                    //Clear Tooltips
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    if (sn.Map.ShowDefaultMap)
                    {
                        sn.Map.ShowDefaultMap = false;
                        LoadDefaultMap();
                        return;
                    }


                    // Get user selected area coordinates

                    //Check for refresh

                    //if (sn.Map.MapRefresh)
                       // Response.Write("<script language='javascript'>window.setTimeout('AutoReloadMap()'," + AutoRefreshTimer.ToString() + ")</script>");
                    //else
                        //Response.Write("<script language='javascript'> clearTimeout();</script>");

                    // TimeSpan currDuration;
                    // Redraw map by coorinates

                    if (sn.Map.MapSearch)
                    {
                        return;
                    }

                    // Redraw map on "Map It" and autorefresh	
                    if ((sn != null) && (sn.Map.DsFleetInfo != null) && (sn.Map.DsFleetInfo.Tables.Count > 0))
                    {
                        Map1.MapData = LoadVehiclesMap();
                    }
                    else
                    {
                        LoadDefaultMap();
                        Map1.MapData = string.Empty;
                    }

                    if (sn.Map.MapRefresh && Request.QueryString["isBig"] != null)
                        Map1.StartupScript = "window.setTimeout('AutoReloadMap()'," + AutoRefreshTimer.ToString() + ");";
                    //else
                    //    Map1.StartupScript = "window.clearTimeout();";
                   

                    //currDuration = new TimeSpan(System.DateTime.Now.Ticks - dt.Ticks);
                    //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Map screen - Load vehicles map ended <- Duration: " + currDuration.TotalSeconds.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));

                }

            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }


        private void LoadDefaultMap()
        {

        }

        private string  LoadVehiclesMap()
        {
            try
            {
                 return  CreateMapData();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
            return string.Empty;
        }

        private string CreateSearchMapData()
        {
            ArrayList vehicleMain = new ArrayList();
            Dictionary<string, string> vehicleDic = new Dictionary<string, string>();

            vehicleMain.Add(vehicleDic);
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(vehicleMain);
        }
        private string CreateMapData()
        {
            ArrayList vehicleMain = new ArrayList();
            string UnitOfMes = sn.User.UnitOfMes == 1 ? "km/h" : "mi/h";
            foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
            {
                if (rowItem["VehicleId"] != DBNull.Value &&
                    rowItem["chkBoxShow"].ToString().ToLower() == "true")
                {
                    Dictionary<string, string> vehicleDic = new Dictionary<string, string>();
                    string icon = string.Empty;
                    if (rowItem["IconTypeName"] != DBNull.Value && rowItem["OriginDateTime"] != DBNull.Value)
                    {
                        if (Convert.ToDateTime(rowItem["OriginDateTime"]) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                        {
                            icon = "Grey" + rowItem["IconTypeName"].ToString().TrimEnd() + ".ico";
                        }
                        else
                        {
                            if (Convert.ToBoolean(rowItem["chkPTO"]))
                                icon = "Blue" + rowItem["IconTypeName"].ToString().TrimEnd() + ".ico";
                            else if (rowItem["Speed"].ToString() != "0")
                                icon = "Green" + rowItem["IconTypeName"].ToString().TrimEnd() + rowItem["MyHeading"].ToString().TrimEnd() + ".ico";
                            else
                                icon = "Red" + rowItem["IconTypeName"].ToString().TrimEnd() + ".ico";
                        }
                    }

                    vehicleDic.Add("id", rowItem["VehicleId"] is DBNull ? string.Empty : rowItem["VehicleId"].ToString());
                    vehicleDic.Add("date", rowItem["OriginDateTime"] is DBNull ? string.Empty : rowItem["OriginDateTime"].ToString());
                    vehicleDic.Add("lat", rowItem["Latitude"] is DBNull ? string.Empty : rowItem["Latitude"].ToString());
                    vehicleDic.Add("lon", rowItem["Longitude"] is DBNull ? string.Empty : rowItem["Longitude"].ToString());
                    vehicleDic.Add("desc", rowItem["Description"] is DBNull ? string.Empty : rowItem["Description"].ToString());
                    vehicleDic.Add("icon", icon);
                    vehicleDic.Add("head", rowItem["MyHeading"] is DBNull ? string.Empty : rowItem["MyHeading"].ToString());
                    vehicleDic.Add("chkPTO", rowItem["chkPTO"] is DBNull ? string.Empty : rowItem["chkPTO"].ToString());
                    vehicleDic.Add("spd", rowItem["CustomSpeed"] is DBNull ? string.Empty : rowItem["CustomSpeed"].ToString() + " " + UnitOfMes);
                    vehicleDic.Add("addr", rowItem["StreetAddress"] is DBNull ? string.Empty : rowItem["StreetAddress"].ToString());
                    vehicleDic.Add("stat", rowItem["VehicleStatus"] is DBNull ? string.Empty : rowItem["VehicleStatus"].ToString());

                    vehicleMain.Add(vehicleDic);

                }
            }
            if (vehicleMain.Count > 0)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = int.MaxValue;
                return js.Serialize(vehicleMain);
            }
            else return "";
        }
    }
}