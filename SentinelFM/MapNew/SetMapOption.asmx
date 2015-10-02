<%@ WebService Language="C#" Class="SetMapOption" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.EnterpriseServices;
using System.Text;
using System.Data.Common;
using Telerik.Web.UI;
using System.Web.UI;
using System.Data.SqlClient;
using System.Configuration;
using VLF3.Services.ActiveState;
using VLF3.Domain.ActiveState;
using System.Data;
using SentinelFM;
using VLF.CLS;
using VLF3.Domain.Class;
using Telerik.Web.UI.GridExcelBuilder;
using VLF3.Domain.ActiveState.Reports;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
[ScriptService]        
public class SetMapOption  : System.Web.Services.WebService {

    [WebMethod(EnableSession = true)]
    public string SetShowLandmark(string ChkShowLandmark)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

        try
        {
            sn.Map.ShowLandmark = Boolean.Parse(ChkShowLandmark);
            string ShowLandmark = "";
            ShowLandmark = sn.Map.ShowLandmark ? "1" : "0";
            SentinelFM.ServerDBUser.DBUser dbu = new SentinelFM.ServerDBUser.DBUser();
            //ShowLandmark
            clsUtility objUtil;
            objUtil = new clsUtility(sn);
            if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmark), ShowLandmark), false))
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmark), ShowLandmark), true))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " SetShowLandmark . User:" + sn.UserID.ToString() + " Form:SetMapOption.asmx"));
                }
                        
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: SetShowLandmark() Page:SetMapOption.asmx"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return "0";
        }
        return "1";
    }

   [WebMethod(EnableSession = true)]
    public string SetShowLandmarkName(string ChkShowLandmarkname)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

        try
        {
            sn.Map.ShowLandmarkname = Boolean.Parse(ChkShowLandmarkname);
            string ShowLandmarkname = "";
            ShowLandmarkname = sn.Map.ShowLandmarkname ? "1" : "0";
            SentinelFM.ServerDBUser.DBUser dbu = new SentinelFM.ServerDBUser.DBUser();
            //ShowLandmark
            clsUtility objUtil;
            objUtil = new clsUtility(sn);
            if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmarkName), ShowLandmarkname), false))
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmarkName), ShowLandmarkname), true))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " SetShowLandmarkName . User:" + sn.UserID.ToString() + " Form:SetMapOption.asmx"));
                }

        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: SetShowLandmarkName() Page:SetMapOption.asmx"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return "0";
        }
        return "1";
    }

    [WebMethod(EnableSession = true)]
    public string SetShowVehicleName(string ChkShowVehicleName)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

        try
        {
            sn.Map.ShowVehicleName = Boolean.Parse(ChkShowVehicleName);
            string ShowVehicleName = "";
            ShowVehicleName = sn.Map.ShowVehicleName ? "1" : "0";
            SentinelFM.ServerDBUser.DBUser dbu = new SentinelFM.ServerDBUser.DBUser();
            //ShowLandmark
            clsUtility objUtil;
            objUtil = new clsUtility(sn);
            if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowVehicleName), ShowVehicleName), false))
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowVehicleName), ShowVehicleName), true))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " SetShowVehicleName . User:" + sn.UserID.ToString() + " Form:SetMapOption.asmx"));
                }

        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: SetShowVehicleName() Page:SetMapOption.asmx"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return "0";
        }
        return "1";
    }

    [WebMethod(EnableSession = true)]
    public string SetShowGeoZones(string ChkShowGeoZones, string isQueryData)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

        try
        {
            sn.Map.ShowGeoZone = Boolean.Parse(ChkShowGeoZones);
            SentinelFM.ServerDBUser.DBUser dbu = new SentinelFM.ServerDBUser.DBUser();
            string ShowGeoZones = "";
            ShowGeoZones = sn.Map.ShowGeoZone ? "1" : "0";
            //ShowLandmark
            clsUtility objUtil;
            objUtil = new clsUtility(sn);
            if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowGeoZone), ShowGeoZones), false))
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowGeoZone), ShowGeoZones), true))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " ShowGeoZones . User:" + sn.UserID.ToString() + " Form:SetMapOption.asmx"));
                }


            if (isQueryData == "1")
            {
                if (sn.GeoZone.DsGeoZone == null || sn.GeoZone.DsGeoDetails == null)
                    sn.GeoZone.DsGeoZone_Fill(sn);

                if (sn.GeoZone.DsGeoZone != null && sn.GeoZone.DsGeoDetails != null)
                {
                    try
                    {

                        string tableName = "";

                        System.Collections.ArrayList geoZones = new System.Collections.ArrayList();
                        int index = 0;
                        foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
                        {
                            tableName = dr["GeoZoneId"].ToString();
                            sn.GeoZone.GeozoneTypeId = (VLF.CLS.Def.Enums.GeozoneType)Convert.ToInt16(dr["GeozoneType"].ToString().TrimEnd());
                            if (sn.GeoZone.DsGeoDetails != null && sn.GeoZone.DsGeoDetails.Tables[tableName] != null && sn.GeoZone.DsGeoDetails.Tables[tableName].Rows.Count > 0)
                            {
                                try
                                {
                                    Dictionary<string, string> geoZone = new Dictionary<string, string>();
                                    geoZone.Add("id", tableName);
                                    geoZone.Add("desc", dr["GeoZoneName"].ToString());
                                    geoZone.Add("type", "0");
                                    if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                                    {

                                        DataRow rowItem1 = sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[0];
                                        DataRow rowItem2 = sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[1];

                                        VLF.MAP.GeoPoint geopoint1 = new VLF.MAP.GeoPoint(Convert.ToDouble(rowItem1["Latitude"]), Convert.ToDouble(rowItem1["Longitude"]));
                                        VLF.MAP.GeoPoint geopoint2 = new VLF.MAP.GeoPoint(Convert.ToDouble(rowItem2["Latitude"]), Convert.ToDouble(rowItem2["Longitude"]));
                                        VLF.MAP.GeoPoint center = VLF.MAP.MapUtilities.GetGeoCenter(geopoint1, geopoint2);

                                        geoZone.Add("lat", center.Latitude.ToString());
                                        geoZone.Add("lon", center.Longitude.ToString());
                                        String coords = string.Format("[[{0},{1}],[{2},{3}],[{4},{5}],[{6},{7}]]",
                                               rowItem1["Latitude"].ToString(),
                                               rowItem1["Longitude"].ToString(),

                                               rowItem1["Latitude"].ToString(),
                                               rowItem2["Longitude"].ToString(),


                                               rowItem2["Latitude"].ToString(),
                                               rowItem2["Longitude"].ToString(),

                                               rowItem2["Latitude"].ToString(),
                                               rowItem1["Longitude"].ToString()

                                            );

                                        geoZone.Add("coords", coords);
                                        //map.GeoZones.Add(new GeoZoneIcon(geopoint1, geopoint2, (VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt16(dr["SeverityId"]), (VLF.CLS.Def.Enums.GeoZoneDirection)Convert.ToInt16(dr["Type"]), dr["GeoZoneName"].ToString().TrimEnd()));
                                        geoZones.Add(geoZone);
                                        index = index + 1;
                                    }
                                    else //Polygon
                                    {

                                        //GeoPoint[] points = new GeoPoint[sn.GeoZone.DsGeoDetails.Tables[tableName].Rows.Count];
                                        //int i = 0;
                                        if (sn.GeoZone.DsGeoDetails.Tables[tableName].Rows.Count <= 0) continue;
                                        geoZone.Add("lat", sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[0]["Latitude"].ToString());
                                        geoZone.Add("lon", sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[0]["Longitude"].ToString());

                                        StringBuilder coords = new StringBuilder();
                                        coords.Append("[");
                                        foreach (DataRow rowItem in sn.GeoZone.DsGeoDetails.Tables[tableName].Rows)
                                        {
                                            if (coords.Length == 1)
                                                coords.Append(
                                                    string.Format("[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
                                            else
                                                coords.Append(
                                                    string.Format(",[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
                                        }
                                        coords.Append("]");
                                        geoZone.Add("coords", coords.ToString());
                                        geoZones.Add(geoZone);
                                        // TODO: put proper severity
                                        //map.Polygons.Add(new PoligonIcon(points, (VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt16(dr["SeverityId"]), (VLF.CLS.Def.Enums.GeoZoneDirection)Convert.ToInt16(dr["Type"]), dr["GeoZoneName"].ToString().TrimEnd(), true));
                                        index = index + 1;

                                    }
                                }
                                catch (Exception ex) { }
                            }

                        }
                        //if (geoZones.Count > 6) break;                        
                        // geoZones.RemoveAt(7);
                        //ArrayList al = new ArrayList();
                        //al.Add(geoZones[7]);
                        ///geoZones = al;
                        //geoZones.RemoveAt(7);
                        //geoZones.RemoveAt(6);
                        //geoZones.RemoveAt(6);
                        if (geoZones.Count > 0)
                        {
                            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                            js.MaxJsonLength = int.MaxValue;
                            return js.Serialize(geoZones);

                        }
                    }
                    catch (NullReferenceException Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                    }

                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: ShowGeoZones() Page:SetMapOption.asmx"));
                        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

                    }

                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: ShowGeoZones() Page:SetMapOption.asmx"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return "0";
        }
        return "1";
    }

    [WebMethod(EnableSession = true)]
    public string SetShowLandmarkRadius(string ChkShowLandmarkRadius)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

        try
        {
            sn.Map.ShowLandmarkRadius = Boolean.Parse(ChkShowLandmarkRadius);
            SentinelFM.ServerDBUser.DBUser dbu = new SentinelFM.ServerDBUser.DBUser();
            string ShowLandmarkRadius = "";
            ShowLandmarkRadius = sn.Map.ShowLandmarkRadius ? "1" : "0";
            //ShowLandmark
            clsUtility objUtil;
            objUtil = new clsUtility(sn);
            if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmarkRadius), ShowLandmarkRadius), false))
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmarkRadius), ShowLandmarkRadius), true))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " ShowLandmarkRadius . User:" + sn.UserID.ToString() + " Form:SetMapOption.asmx"));
                }

        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: ShowLandmarkRadius() Page:SetMapOption.asmx"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return "0";
        }
        return "1";
    }          
}