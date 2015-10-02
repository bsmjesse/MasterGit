<%@ WebService Language="C#" Class="AddNewMapGoezone" %>

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
using System.Data;
using SentinelFM;
using VLF.CLS;
using System.Collections;
using Telerik.Web.UI.GridExcelBuilder;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.IO;
using VLF.MAP;
using SentinelFM.ServerDBOrganization;


[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class AddNewMapGoezone  : System.Web.Services.WebService {
    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
    VLF.DAS.Logic.LandmarkPointSetManager landPointMgr = null;

    [WebMethod]
    public string HelloWorld() {
        return "Hello World";
    }
    
    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public string GetAllGoezons()
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

        try
        {
            if (sn.GeoZone.DsGeoZone == null || sn.GeoZone.DsGeoDetails == null)
                DsGeoZone_Fill(sn);


            if (sn.GeoZone.DsGeoZone != null && sn.GeoZone.DsGeoDetails != null)
            {
                try
                {

                    string tableName = "";

                    ArrayList geoZones = new ArrayList();
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
                                geoZone.Add("geozoneName", dr["GeoZoneName"].ToString());
                                geoZone.Add("desc", dr["Description"].ToString());
                                geoZone.Add("type", "0");
                                geoZone.Add("Assigned", dr["Assigned"].ToString());
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
                        JavaScriptSerializer js = new JavaScriptSerializer();
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
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

                }

            }            

        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddNewMapGoezone"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return "0";
        }
        return "1";
    }

    // Changes for TimeZone Feature start
    private void DsGeoZone_Fill_NewTZ(SentinelFMSession sn)
    {
        try
        {
            sn.GeoZone.DsGeoDetails = new DataSet();
            sn.GeoZone.DsGeoZone_Fill_NewTZ(sn);

        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
        }

        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

        }
    }


    // Changes for TimeZone Feature end

    private void DsGeoZone_Fill(SentinelFMSession sn)
    {
        try
        {
            sn.GeoZone.DsGeoDetails = new DataSet();
            sn.GeoZone.DsGeoZone_Fill(sn);
            
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
        }

        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

        }
    }

    // Changes for TimeZone Feature start
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public string AddNewGeozoneLandmark_NewTZ()
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];

        clsUtility objUtil;
        objUtil = new clsUtility(sn);

        _results _r = new _results();

        string pointSets = HttpUtility.HtmlDecode(HttpContext.Current.Request["pointSets"]);
        string centerPoint = HttpUtility.HtmlDecode(HttpContext.Current.Request["centerPoint"]);

        bool isNew = Convert.ToInt16(HttpContext.Current.Request["isNew"] ?? "0") == 1;

        if (Int16.Parse(HttpContext.Current.Request["lstAddOptions"]) == 0) // Landmark
        {

            clsMap mp = new clsMap();
            landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);
            string oldLandmarkName = HttpUtility.HtmlDecode(HttpContext.Current.Request["oldLandmarkName"]);
            string landmarkName = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtLandmarkName"]);
            string txtX = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtX"]);
            string txtY = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtY"]);
            string txtLandmarkDesc = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtLandmarkDesc"]);
            string txtContactName = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtContactName"]);
            string txtPhone = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtPhone"]);
            string txtEmail = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtEmail"]);
            string txtPhoneSMS = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtPhoneSMS"] ?? string.Empty);
            bool chkDayLight = HttpContext.Current.Request["chkDayLight"] != null;
            string strAddress = mp.ResolveStreetAddressTelogis(txtY, txtX);
            bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight));
            string cboTimeZone = HttpContext.Current.Request["cboTimeZone"];
            int radius = Convert.ToInt32(HttpContext.Current.Request["txtRadius"] ?? "-1");

            if (radius > 0)
                pointSets = string.Empty;

            try
            {
                if (isNew)
                {
                    _r.message = "Sucessfully created: " + landmarkName;
                    _r.isNew = 1;
                    landPointMgr.vlfLandmarkPointSet_Add_NewTZ(sn.User.OrganizationId, landmarkName,
                                 Convert.ToDouble(txtY), Convert.ToDouble(txtX),
                                 txtLandmarkDesc,
                                 txtContactName,
                                 txtPhone,
                                 radius,
                                 txtEmail,
                                 txtPhoneSMS,
                                 Convert.ToSingle(cboTimeZone), DayLightSaving, chkDayLight, strAddress,
                                 pointSets, true,0
                                 );
                }
                else
                {
                    _r.message = "The changes are saved.";
                    _r.isNew = 0;
                    landPointMgr.vlfLandmarkPointSet_Update_NewTZ(sn.User.OrganizationId, oldLandmarkName,
                                landmarkName,
                                Convert.ToDouble(txtY), Convert.ToDouble(txtX),
                                txtLandmarkDesc,
                                txtContactName,
                                txtPhone,
                                radius,
                                txtEmail,
                                txtPhoneSMS,
                                Convert.ToSingle(cboTimeZone), DayLightSaving, chkDayLight, strAddress,
                                pointSets, true,0
                                );

                }

                _r.status = 200;
                _r.objectType = "Landmark";
                sn.DsLandMarks = null;
            }
            catch { _r.status = 500; }
            _r.geozonelandmarkname = landmarkName;
        }
        else if (Int16.Parse(HttpContext.Current.Request["lstAddOptions"]) == 1)    // Geozone
        {
            string txtGeoZoneName = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtGeoZoneName"]);
            short cboDirection = Convert.ToInt16(HttpContext.Current.Request["cboDirection"] ?? "0");
            short geozoneType = 2;
            short cboGeoZoneSeverity = Convert.ToInt16(HttpContext.Current.Request["cboGeoZoneSeverity"] ?? "0");
            string txtGeoZoneDesc = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtGeoZoneDesc"]);
            string txtEmail = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtEmail"]);
            string txtPhone = "";
            string cboTimeZone = HttpContext.Current.Request["cboTimeZone"];
            bool chkDayLight = HttpContext.Current.Request["chkDayLight"] != null;
            bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight));
            bool chkWarning = HttpContext.Current.Request["chkWarning"] != null;
            bool chkCritical = HttpContext.Current.Request["chkCritical"] != null;
            bool chkNotify = HttpContext.Current.Request["chkNotify"] != null;

            string[] points = pointSets.Split(',');
            string pointsXml = "<NewDataSet>";

            DBOrganization dbo = new DBOrganization();
            for (int i = 0; i < points.Length; i++)
            {
                pointsXml = pointsXml + "<Table1><Latitude>" + points[i].Split('|')[0] + "</Latitude><Longitude>" + points[i].Split('|')[1] + "</Longitude>";
                pointsXml = pointsXml + "<SequenceNum>" + (i + 1).ToString() + "</SequenceNum></Table1>";
            }
            pointsXml = pointsXml + "</NewDataSet>";

            _r.status = 200;
            _r.objectType = "Geozone";
            _r.geozonelandmarkname = txtGeoZoneName;

            if (isNew)
            {
                _r.message = "Sucessfully created: " + txtGeoZoneName;
                //if (objUtil.ErrCheck(dbo.AddGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight,0), false))
                //  if (objUtil.ErrCheck(dbo.AddGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight,0), true))
                //{
                //   _r.status = 500;
                //  _r.message = "Failed to create Geozone.";
                //}
            }
            else
            {
                _r.message = "The changes is saved.";
                //short geozoneId = Convert.ToInt16(HttpContext.Current.Request["oid"]);
                //if (objUtil.ErrCheck(dbo.UpdateGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, geozoneId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight), false))
                //   if (objUtil.ErrCheck(dbo.UpdateGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, geozoneId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight), true))
                //  {
                //     _r.status = 500;
                //    _r.message = "Failed to edit Geozone";
                // }
            }

            sn.GeoZone.DsGeoZone = null;
            sn.GeoZone.DsGeoDetails = new DataSet();
        }
        return new JavaScriptSerializer().Serialize(_r);
    }



    // Changes for TimeZone Feature end

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet=true)]
    public string AddNewGeozoneLandmark()
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];
        
        clsUtility objUtil;
        objUtil = new clsUtility(sn);
        
        _results _r = new _results();
        
        string pointSets = HttpUtility.HtmlDecode(HttpContext.Current.Request["pointSets"]);
        string centerPoint = HttpUtility.HtmlDecode(HttpContext.Current.Request["centerPoint"]);

        bool isNew = Convert.ToInt16(HttpContext.Current.Request["isNew"] ?? "0") == 1;

        if (Int16.Parse(HttpContext.Current.Request["lstAddOptions"]) == 0) // Landmark
        {

            clsMap mp = new clsMap();
            landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);
            string oldLandmarkName = HttpUtility.HtmlDecode(HttpContext.Current.Request["oldLandmarkName"]);
            string landmarkName = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtLandmarkName"]);
            string txtX = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtX"]);
            string txtY = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtY"]);
            string txtLandmarkDesc = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtLandmarkDesc"]);
            string txtContactName = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtContactName"]);
            string txtPhone = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtPhone"]);
            string txtEmail = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtEmail"]);
            string txtPhoneSMS = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtPhoneSMS"]??string.Empty);
            bool chkDayLight = HttpContext.Current.Request["chkDayLight"] != null;
            string strAddress = mp.ResolveStreetAddressTelogis(txtY, txtX);
            bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight));
            string cboTimeZone = HttpContext.Current.Request["cboTimeZone"];
            int radius = Convert.ToInt32(HttpContext.Current.Request["txtRadius"] ?? "-1");

            if (radius > 0) 
                pointSets = string.Empty;            

            try
            {
                if (isNew)
                {
                    _r.message = "Sucessfully created: " + landmarkName;
                    _r.isNew = 1;
                    landPointMgr.vlfLandmarkPointSet_Add(sn.User.OrganizationId, landmarkName,
                                 Convert.ToDouble(txtY), Convert.ToDouble(txtX),
                                 txtLandmarkDesc,
                                 txtContactName,
                                 txtPhone,
                                 radius,
                                 txtEmail,
                                 txtPhoneSMS,
                                 Convert.ToInt16(cboTimeZone), DayLightSaving, chkDayLight, strAddress,
                                 pointSets,true, 0);
                }
                else
                {
                    _r.message = "The changes are saved.";
                    _r.isNew = 0;
                    landPointMgr.vlfLandmarkPointSet_Update(sn.User.OrganizationId, oldLandmarkName,
                                landmarkName,
                                Convert.ToDouble(txtY), Convert.ToDouble(txtX),
                                txtLandmarkDesc,
                                txtContactName,
                                txtPhone,
                                radius,
                                txtEmail,
                                txtPhoneSMS,
                                Convert.ToInt16(cboTimeZone), DayLightSaving, chkDayLight, strAddress,
                                pointSets, true, 0
                                );
                    
                }

                _r.status = 200;
                _r.objectType = "Landmark";
                sn.DsLandMarks = null;
            }
            catch { _r.status = 500; }
            _r.geozonelandmarkname = landmarkName;
        }
        else if (Int16.Parse(HttpContext.Current.Request["lstAddOptions"]) == 1)    // Geozone
        {
            string txtGeoZoneName = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtGeoZoneName"]);
            short cboDirection = Convert.ToInt16(HttpContext.Current.Request["cboDirection"] ?? "0");
            short geozoneType = 2;
            short cboGeoZoneSeverity = Convert.ToInt16(HttpContext.Current.Request["cboGeoZoneSeverity"] ?? "0");
            string txtGeoZoneDesc = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtGeoZoneDesc"]);
            string txtEmail = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtEmail"]);
            string txtPhone = "";
            string cboTimeZone = HttpContext.Current.Request["cboTimeZone"];
            bool chkDayLight = HttpContext.Current.Request["chkDayLight"] != null;
            bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight));
            bool chkWarning = HttpContext.Current.Request["chkWarning"] != null;
            bool chkCritical = HttpContext.Current.Request["chkCritical"] != null;
            bool chkNotify = HttpContext.Current.Request["chkNotify"] != null;

            string[] points = pointSets.Split(',');
            string pointsXml = "<NewDataSet>";

            DBOrganization dbo = new DBOrganization();
            for (int i = 0; i < points.Length; i++)
            {
                pointsXml = pointsXml + "<Table1><Latitude>" + points[i].Split('|')[0] + "</Latitude><Longitude>" + points[i].Split('|')[1] + "</Longitude>";
                pointsXml = pointsXml + "<SequenceNum>" + (i + 1).ToString() + "</SequenceNum></Table1>";
            }
            pointsXml = pointsXml + "</NewDataSet>";

            _r.status = 200;
            _r.objectType = "Geozone";
            _r.geozonelandmarkname = txtGeoZoneName;

            if (isNew)
            {
                _r.message = "Sucessfully created: " + txtGeoZoneName;
                //if (objUtil.ErrCheck(dbo.AddGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight,0), false))
                  //  if (objUtil.ErrCheck(dbo.AddGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight,0), true))
                    //{
                     //   _r.status = 500;
                      //  _r.message = "Failed to create Geozone.";
                    //}
            }
            else
            {
                _r.message = "The changes is saved.";
                //short geozoneId = Convert.ToInt16(HttpContext.Current.Request["oid"]);
                //if (objUtil.ErrCheck(dbo.UpdateGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, geozoneId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight), false))
                 //   if (objUtil.ErrCheck(dbo.UpdateGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, geozoneId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight), true))
                  //  {
                   //     _r.status = 500;
                    //    _r.message = "Failed to edit Geozone";
                   // }
            }

            sn.GeoZone.DsGeoZone = null;
            sn.GeoZone.DsGeoDetails = new DataSet();
        }
        return new JavaScriptSerializer().Serialize(_r);
    }

    // Changes for TimeZone Feature start
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string EditGeozoneLandmark_NewTZ(string otype, string oid, string pointSets)
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];

        clsUtility objUtil;
        objUtil = new clsUtility(sn);

        _results _r = new _results();

        pointSets = HttpUtility.HtmlDecode(pointSets).Replace("%7C", "|");
        oid = HttpUtility.HtmlDecode(oid);

        if (otype.ToLower() == "landmark") // Landmark
        {
            if (sn.DsLandMarks == null)
                DgLandmarks_Fill_NewTZ(sn);

            _r.status = 200;
            _r.message = pointSets;
            clsMap mp = new clsMap();
            landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);

            foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
            {
                if (oid.TrimEnd() == dr["LandmarkName"].ToString().TrimEnd())
                {
                    string txtX = dr["Longitude"].ToString().TrimEnd();
                    string txtY = dr["Latitude"].ToString().TrimEnd();
                    string txtLandmarkDesc = dr["Description"].ToString().TrimEnd();
                    string txtContactName = dr["ContactPersonName"].ToString().TrimEnd();
                    string txtPhone = dr["ContactPhoneNum"].ToString().TrimEnd();
                    string txtEmail = dr["Email"].ToString().TrimEnd();
                    string txtPhoneSMS = dr["Phone"].ToString().TrimEnd();
                    bool chkDayLight = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                    string strAddress = dr["StreetAddress"].ToString().TrimEnd();
                    bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight));
                    string cboTimeZone = dr["TimeZone"].ToString().TrimEnd();
                    int radius = Convert.ToInt32(dr["Radius"]);

                    try
                    {
                        landPointMgr.vlfLandmarkPointSet_Update_NewTZ(sn.User.OrganizationId, oid,
                                        oid,
                                        Convert.ToDouble(txtY), Convert.ToDouble(txtX),
                                        txtLandmarkDesc,
                                        txtContactName,
                                        txtPhone,
                                        radius,
                                        txtEmail,
                                        txtPhoneSMS,
                                        Convert.ToSingle(cboTimeZone), DayLightSaving, chkDayLight, strAddress,
                                        pointSets, true,0
                                     );

                        _r.status = 200;
                        _r.objectType = "Landmark";
                        sn.DsLandMarks = null;
                    }
                    catch { _r.status = 500; }
                    break;
                }
            }
            _r.geozonelandmarkname = oid;
        }
        else if (otype.ToLower() == "geozone") // Geozone
        {
            _r.status = 200;

            if (sn.GeoZone.DsGeoZone == null || sn.GeoZone.DsGeoDetails == null)
                DsGeoZone_Fill(sn);

            string[] points = pointSets.Split(',');
            string pointsXml = "<NewDataSet>";

            DBOrganization dbo = new DBOrganization();
            for (int i = 0; i < points.Length; i++)
            {
                pointsXml = pointsXml + "<Table1><Latitude>" + points[i].Split('|')[0] + "</Latitude><Longitude>" + points[i].Split('|')[1] + "</Longitude>";
                pointsXml = pointsXml + "<SequenceNum>" + (i + 1).ToString() + "</SequenceNum></Table1>";
            }
            pointsXml = pointsXml + "</NewDataSet>";

            foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
            {
                if (oid.TrimEnd() == dr["GeozoneName"].ToString().TrimEnd())
                {
                    string geozoneId = dr["GeozoneId"].ToString().TrimEnd();
                    string txtGeoZoneName = dr["GeozoneName"].ToString().TrimEnd();
                    string cboDirection = dr["Type"].ToString().TrimEnd();
                    string typeId = dr["GeozoneType"].ToString().TrimEnd();
                    string cboGeoZoneSeverity = dr["SeverityId"].ToString().TrimEnd();
                    string txtGeoZoneDesc = dr["Description"].ToString().TrimEnd();
                    string txtEmail = dr["Email"].ToString().TrimEnd();
                    string txtPhone = dr["Phone"].ToString().TrimEnd();
                    string cboTimeZone = dr["TimeZone"].ToString().TrimEnd();
                    bool chkDayLight = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                    bool DayLightSaving = Convert.ToBoolean(dr["DayLightSaving"].ToString().TrimEnd());
                    bool chkNotify = Convert.ToBoolean(dr["Notify"].ToString().TrimEnd());
                    bool chkWarning = Convert.ToBoolean(dr["Warning"].ToString().TrimEnd());
                    bool chkCritical = Convert.ToBoolean(dr["Critical"].ToString().TrimEnd());

                    //if (objUtil.ErrCheck(dbo.UpdateGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(geozoneId), txtGeoZoneName, Convert.ToInt16(cboDirection), Convert.ToInt16(typeId), pointsXml, Convert.ToInt16(cboGeoZoneSeverity), txtGeoZoneDesc, txtEmail, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight), false))
                    //   if (objUtil.ErrCheck(dbo.UpdateGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(geozoneId), txtGeoZoneName, Convert.ToInt16(cboDirection), Convert.ToInt16(typeId), pointsXml, Convert.ToInt16(cboGeoZoneSeverity), txtGeoZoneDesc, txtEmail, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight), true))
                    //  {
                    //     _r.status = 500;
                    //}
                    _r.geozonelandmarkname = txtGeoZoneName;
                    sn.GeoZone.DsGeoZone = null;
                    sn.GeoZone.DsGeoDetails = new DataSet();
                    break;
                }
            }

        }
        return new JavaScriptSerializer().Serialize(_r);
    }



    // Changes for TimeZone Feature end

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string EditGeozoneLandmark(string otype, string oid, string pointSets)
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];

        clsUtility objUtil;
        objUtil = new clsUtility(sn);

        _results _r = new _results();

        pointSets = HttpUtility.HtmlDecode(pointSets).Replace("%7C", "|");
        oid = HttpUtility.HtmlDecode(oid);

        if (otype.ToLower() == "landmark") // Landmark
        {
            if (sn.DsLandMarks == null)
                DgLandmarks_Fill(sn);
            
            _r.status = 200;
            _r.message = pointSets;
            clsMap mp = new clsMap();
            landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);

            foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
            {
                if (oid.TrimEnd() == dr["LandmarkName"].ToString().TrimEnd())
                {
                    string txtX = dr["Longitude"].ToString().TrimEnd();
                    string txtY = dr["Latitude"].ToString().TrimEnd();
                    string txtLandmarkDesc = dr["Description"].ToString().TrimEnd();
                    string txtContactName = dr["ContactPersonName"].ToString().TrimEnd();
                    string txtPhone = dr["ContactPhoneNum"].ToString().TrimEnd();
                    string txtEmail = dr["Email"].ToString().TrimEnd();
                    string txtPhoneSMS = dr["Phone"].ToString().TrimEnd();
                    bool chkDayLight = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                    string strAddress = dr["StreetAddress"].ToString().TrimEnd();
                    bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight));
                    string cboTimeZone = dr["TimeZone"].ToString().TrimEnd();
                    int radius = Convert.ToInt32(dr["Radius"]);

                    try
                    {
                        landPointMgr.vlfLandmarkPointSet_Update(sn.User.OrganizationId, oid,
                                        oid,
                                        Convert.ToDouble(txtY), Convert.ToDouble(txtX),
                                        txtLandmarkDesc,
                                        txtContactName,
                                        txtPhone,
                                        radius,
                                        txtEmail,
                                        txtPhoneSMS,
                                        Convert.ToInt16(cboTimeZone), DayLightSaving, chkDayLight, strAddress,
                                        pointSets, true, 0
                                     );

                        _r.status = 200;
                        _r.objectType = "Landmark";
                        sn.DsLandMarks = null;
                    }
                    catch { _r.status = 500; }
                    break;
                }
            }
            _r.geozonelandmarkname = oid;
        }
        else if (otype.ToLower() == "geozone") // Geozone
        {
            _r.status = 200;
            
            if (sn.GeoZone.DsGeoZone == null || sn.GeoZone.DsGeoDetails == null)
                DsGeoZone_Fill(sn);

            string[] points = pointSets.Split(',');
            string pointsXml = "<NewDataSet>";

            DBOrganization dbo = new DBOrganization();
            for (int i = 0; i < points.Length; i++)
            {
                pointsXml = pointsXml + "<Table1><Latitude>" + points[i].Split('|')[0] + "</Latitude><Longitude>" + points[i].Split('|')[1] + "</Longitude>";
                pointsXml = pointsXml + "<SequenceNum>" + (i + 1).ToString() + "</SequenceNum></Table1>";
            }
            pointsXml = pointsXml + "</NewDataSet>";

            foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
            {
                if (oid.TrimEnd() == dr["GeozoneName"].ToString().TrimEnd())
                {
                    string geozoneId = dr["GeozoneId"].ToString().TrimEnd();
                    string txtGeoZoneName = dr["GeozoneName"].ToString().TrimEnd();
                    string cboDirection = dr["Type"].ToString().TrimEnd();
                    string typeId = dr["GeozoneType"].ToString().TrimEnd();
                    string cboGeoZoneSeverity = dr["SeverityId"].ToString().TrimEnd();
                    string txtGeoZoneDesc = dr["Description"].ToString().TrimEnd();
                    string txtEmail = dr["Email"].ToString().TrimEnd();
                    string txtPhone = dr["Phone"].ToString().TrimEnd();
                    string cboTimeZone = dr["TimeZone"].ToString().TrimEnd();
                    bool chkDayLight = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                    bool DayLightSaving = Convert.ToBoolean(dr["DayLightSaving"].ToString().TrimEnd());
                    bool chkNotify = Convert.ToBoolean(dr["Notify"].ToString().TrimEnd());
                    bool chkWarning = Convert.ToBoolean(dr["Warning"].ToString().TrimEnd());
                    bool chkCritical = Convert.ToBoolean(dr["Critical"].ToString().TrimEnd());

                    //if (objUtil.ErrCheck(dbo.UpdateGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(geozoneId), txtGeoZoneName, Convert.ToInt16(cboDirection), Convert.ToInt16(typeId), pointsXml, Convert.ToInt16(cboGeoZoneSeverity), txtGeoZoneDesc, txtEmail, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight), false))
                     //   if (objUtil.ErrCheck(dbo.UpdateGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(geozoneId), txtGeoZoneName, Convert.ToInt16(cboDirection), Convert.ToInt16(typeId), pointsXml, Convert.ToInt16(cboGeoZoneSeverity), txtGeoZoneDesc, txtEmail, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight), true))
                      //  {
                       //     _r.status = 500;
                        //}
                    _r.geozonelandmarkname = txtGeoZoneName;
                    sn.GeoZone.DsGeoZone = null;
                    sn.GeoZone.DsGeoDetails = new DataSet();
                    break;
                }
            }
        
        }
        return new JavaScriptSerializer().Serialize(_r);
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string DeleteGeozoneLandmark(string otype, string oid)
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];
        landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);

        clsUtility objUtil;
        objUtil = new clsUtility(sn);

        _results _r = new _results();

        oid = HttpUtility.HtmlDecode(oid);

        if (otype.ToLower() == "landmark") // Landmark
        {
            _r.status = 200;
            try
            {
                landPointMgr.vlfLandmarkPointSet_Delete(sn.User.OrganizationId, oid);
                sn.DsLandMarks = null;

            }
            catch
            {
                _r.status = 500;
            }
        }
        else if (otype.ToLower() == "geozone") // Geozone
        {
            _r.status = 200;

            DBOrganization dbo = new DBOrganization();
            if (objUtil.ErrCheck(dbo.DeleteGeozoneFromOrganization(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(oid)), false))
                if (objUtil.ErrCheck(dbo.DeleteGeozoneFromOrganization(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(oid)), true))
                {
                    _r.status = 500;
                }
            
            sn.GeoZone.DsGeoZone = null;
            sn.GeoZone.DsGeoDetails = new DataSet();

        }
        return new JavaScriptSerializer().Serialize(_r);
    }

    // Changes for TimeZone Feature start


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public string GetAllLandmarks_NewTZ()
    {
        string status0 = "{\"status\":0}";
        string status1 = "{\"status\":1}";
        string status_1 = "{\"status\":-1}";
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null)
            sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else
            return status_1;

        if (sn.User == null || String.IsNullOrEmpty(sn.UserName))
            return status_1;

        try
        {
            if (sn.DsLandMarks == null || sn.Landmark.DsLandmarkPointDetails == null)
                DgLandmarks_Fill_NewTZ(sn);

            if (sn.DsLandMarks != null && sn.DsLandmarkPoints != null)
            {
                try
                {

                    string tableName = "";

                    ArrayList landmarks = new ArrayList();
                    foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
                    {
                        tableName = dr["LandmarkName"].ToString();

                        Dictionary<string, string> landmark = new Dictionary<string, string>();
                        landmark.Add("id", tableName);
                        int radius = Int32.Parse(dr["Radius"].ToString());
                        landmark.Add("lat", dr["Latitude"].ToString());
                        landmark.Add("lon", dr["Longitude"].ToString());
                        landmark.Add("radius", radius.ToString());
                        landmark.Add("LandmarkName", tableName);

                        if (radius > 0)
                        {
                            landmarks.Add(landmark);
                        }
                        else if (sn.DsLandmarkPoints != null && sn.DsLandmarkPoints.Tables[tableName] != null && sn.DsLandmarkPoints.Tables[tableName].Rows.Count > 2)
                        {
                            try
                            {

                                if (radius == -1)   //Polygon
                                {
                                    if (sn.DsLandmarkPoints.Tables[tableName].Rows.Count <= 2) continue;

                                    StringBuilder coords = new StringBuilder();
                                    coords.Append("[");
                                    foreach (DataRow rowItem in sn.DsLandmarkPoints.Tables[tableName].Rows)
                                    {
                                        if (coords.Length == 1)
                                            coords.Append(
                                                string.Format("[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
                                        else
                                            coords.Append(
                                                string.Format(",[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
                                    }
                                    coords.Append("]");
                                    landmark.Add("coords", coords.ToString());
                                    landmarks.Add(landmark);
                                }
                            }
                            catch { }
                        }

                    }
                    if (landmarks.Count > 0)
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        js.MaxJsonLength = int.MaxValue;
                        return js.Serialize(landmarks);

                    }
                }
                catch (NullReferenceException Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                }

                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

                }

            }

        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddNewMapGoezone"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return status0;
        }
        return status1;
    }

    // Changes for TimeZone Feature end
    
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public string GetAllLandmarks()
    {
        string status0 = "{\"status\":0}";
        string status1 = "{\"status\":1}";
        string status_1 = "{\"status\":-1}";
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) 
            sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else
            return status_1;
        
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName))
            return status_1;

        try
        {
            if (sn.DsLandMarks == null || sn.Landmark.DsLandmarkPointDetails == null)
                DgLandmarks_Fill(sn);

            if (sn.DsLandMarks != null && sn.DsLandmarkPoints != null)
            {
                try
                {

                    string tableName = "";

                    ArrayList landmarks = new ArrayList();
                    foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
                    {
                        tableName = dr["LandmarkName"].ToString();

                        Dictionary<string, string> landmark = new Dictionary<string, string>();
                        landmark.Add("id", tableName);
                        int radius = Int32.Parse(dr["Radius"].ToString());
                        landmark.Add("lat", dr["Latitude"].ToString());
                        landmark.Add("lon", dr["Longitude"].ToString());
                        landmark.Add("radius", radius.ToString());
                        landmark.Add("LandmarkName", tableName); 

                        if (radius > 0)
                        {
                            landmarks.Add(landmark);
                        }
                        else if (sn.DsLandmarkPoints != null && sn.DsLandmarkPoints.Tables[tableName] != null && sn.DsLandmarkPoints.Tables[tableName].Rows.Count > 2)
                        {
                            try
                            {
                               
                                if (radius == -1)   //Polygon
                                {
                                    if (sn.DsLandmarkPoints.Tables[tableName].Rows.Count <= 2) continue;                                    

                                    StringBuilder coords = new StringBuilder();
                                    coords.Append("[");
                                    foreach (DataRow rowItem in sn.DsLandmarkPoints.Tables[tableName].Rows)
                                    {
                                        if (coords.Length == 1)
                                            coords.Append(
                                                string.Format("[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
                                        else
                                            coords.Append(
                                                string.Format(",[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
                                    }
                                    coords.Append("]");
                                    landmark.Add("coords", coords.ToString());                                                                           
                                    landmarks.Add(landmark);
                                }
                            }
                            catch  { }
                        }

                    }
                    if (landmarks.Count > 0)
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        js.MaxJsonLength = int.MaxValue;
                        return js.Serialize(landmarks);

                    }
                }
                catch (NullReferenceException Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                }

                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

                }

            }         

        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddNewMapGoezone"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return status0;
        }
        return status1;
    }

    // Changes for TimeZone Feature start
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetGeozoneById_NewTZ(string oid)
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];

        if (sn.GeoZone.DsGeoZone == null || sn.GeoZone.DsGeoDetails == null)
            DsGeoZone_Fill_NewTZ(sn);

        _geozone_NewTZ _g = new _geozone_NewTZ();

        foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
        {
            if (oid.TrimEnd() == dr["GeoZoneName"].ToString().TrimEnd())
            {
                _g.GeozoneNo = Convert.ToInt32(dr["GeozoneNo"].ToString().TrimEnd());
                _g.OrganizationId = Convert.ToInt32(dr["OrganizationId"].ToString().TrimEnd());
                _g.GeozoneId = Convert.ToInt32(dr["GeozoneId"].ToString().TrimEnd());
                _g.GeozoneName = dr["GeozoneName"].ToString().TrimEnd();
                _g.Direction = Convert.ToInt32(dr["Type"].ToString().TrimEnd());
                _g.GeozoneType = Convert.ToInt32(dr["GeozoneType"].ToString().TrimEnd());
                _g.SeverityId = Convert.ToInt32(dr["SeverityId"].ToString().TrimEnd());
                _g.Description = dr["Description"].ToString().TrimEnd();
                _g.Email = dr["Email"].ToString().TrimEnd();
                _g.Phone = dr["Phone"].ToString().TrimEnd();
                _g.TimeZone = Convert.ToSingle(dr["TimeZoneNew"].ToString().TrimEnd());
                _g.DayLightSaving = Convert.ToBoolean(dr["DayLightSaving"].ToString().TrimEnd());
                _g.Notify = Convert.ToBoolean(dr["Notify"].ToString().TrimEnd());
                _g.Warning = Convert.ToBoolean(dr["Warning"].ToString().TrimEnd());
                _g.Critical = Convert.ToBoolean(dr["Critical"].ToString().TrimEnd());
                _g.AutoAdjustDayLightSaving = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());

                break;
            }
        }
        return new JavaScriptSerializer().Serialize(_g);
    }


    // Changes for TimeZone Feature end

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetGeozoneById(string oid)
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];
        
        if (sn.GeoZone.DsGeoZone == null || sn.GeoZone.DsGeoDetails == null)
                DsGeoZone_Fill(sn);

        _geozone _g = new _geozone();
        
        foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
        {
            if (oid.TrimEnd() == dr["GeoZoneName"].ToString().TrimEnd())
            {
                _g.GeozoneNo = Convert.ToInt32(dr["GeozoneNo"].ToString().TrimEnd());
                _g.OrganizationId = Convert.ToInt32(dr["OrganizationId"].ToString().TrimEnd());
                _g.GeozoneId = Convert.ToInt32(dr["GeozoneId"].ToString().TrimEnd());
                _g.GeozoneName = dr["GeozoneName"].ToString().TrimEnd();
                _g.Direction = Convert.ToInt32(dr["Type"].ToString().TrimEnd());
                _g.GeozoneType = Convert.ToInt32(dr["GeozoneType"].ToString().TrimEnd());
                _g.SeverityId = Convert.ToInt32(dr["SeverityId"].ToString().TrimEnd());
                _g.Description = dr["Description"].ToString().TrimEnd();
                _g.Email = dr["Email"].ToString().TrimEnd();
                _g.Phone = dr["Phone"].ToString().TrimEnd();
                _g.TimeZone = Convert.ToInt32(dr["TimeZone"].ToString().TrimEnd());
                _g.DayLightSaving = Convert.ToBoolean(dr["DayLightSaving"].ToString().TrimEnd());
                _g.Notify = Convert.ToBoolean(dr["Notify"].ToString().TrimEnd());
                _g.Warning = Convert.ToBoolean(dr["Warning"].ToString().TrimEnd());
                _g.Critical = Convert.ToBoolean(dr["Critical"].ToString().TrimEnd());
                _g.AutoAdjustDayLightSaving = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());

                break;
            }
        }
        return new JavaScriptSerializer().Serialize(_g);
    }

    // Changes for TimeZone Feature start
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetLandmarkById_NewTZ(string oid)
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];

        if (sn.DsLandMarks == null)
            DgLandmarks_Fill_NewTZ(sn);

        _landmark_NewTZ _l = new _landmark_NewTZ();

        foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
        {
            if (oid.TrimEnd() == dr["LandmarkName"].ToString().TrimEnd())
            {
                _l.LandmarkId = Convert.ToInt32(dr["LandmarkId"].ToString().TrimEnd());
                _l.OrganizationId = Convert.ToInt32(dr["OrganizationId"].ToString().TrimEnd());
                _l.LandmarkName = dr["LandmarkName"].ToString().TrimEnd();
                _l.Latitude = Convert.ToDouble(dr["Latitude"].ToString().TrimEnd());
                _l.Longitude = Convert.ToDouble(dr["Longitude"].ToString().TrimEnd());
                _l.Description = dr["Description"].ToString().TrimEnd();
                _l.ContactPersonName = dr["ContactPersonName"].ToString().TrimEnd();
                _l.ContactPhoneNum = dr["ContactPhoneNum"].ToString().TrimEnd();
                _l.Radius = Convert.ToInt32(dr["Radius"].ToString().TrimEnd());
                _l.Email = dr["Email"].ToString().TrimEnd();
                _l.Phone = dr["Phone"].ToString().TrimEnd();
                _l.TimeZone = Convert.ToSingle(dr["TimeZone"].ToString().TrimEnd());
                _l.DayLightSaving = Convert.ToBoolean(dr["DayLightSaving"].ToString().TrimEnd());
                _l.AutoAdjustDayLightSaving = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                _l.StreetAddress = dr["StreetAddress"].ToString().TrimEnd();

                break;
            }
        }
        return new JavaScriptSerializer().Serialize(_l);
    }


    // Changes for TimeZone Feature end

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetLandmarkById(string oid)
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];

        if (sn.DsLandMarks == null)
            DgLandmarks_Fill(sn);
        
        _landmark _l = new _landmark();            
            
        foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
        {
            if (oid.TrimEnd() == dr["LandmarkName"].ToString().TrimEnd())
            {
                _l.LandmarkId = Convert.ToInt32(dr["LandmarkId"].ToString().TrimEnd());
                _l.OrganizationId = Convert.ToInt32(dr["OrganizationId"].ToString().TrimEnd());
                _l.LandmarkName = dr["LandmarkName"].ToString().TrimEnd();
                _l.Latitude = Convert.ToDouble(dr["Latitude"].ToString().TrimEnd());
                _l.Longitude = Convert.ToDouble(dr["Longitude"].ToString().TrimEnd());
                _l.Description = dr["Description"].ToString().TrimEnd();
                _l.ContactPersonName = dr["ContactPersonName"].ToString().TrimEnd();
                _l.ContactPhoneNum = dr["ContactPhoneNum"].ToString().TrimEnd();
                _l.Radius = Convert.ToInt32(dr["Radius"].ToString().TrimEnd());
                _l.Email = dr["Email"].ToString().TrimEnd();
                _l.Phone = dr["Phone"].ToString().TrimEnd();
                _l.TimeZone = Convert.ToInt32(dr["TimeZone"].ToString().TrimEnd());
                _l.DayLightSaving = Convert.ToBoolean(dr["DayLightSaving"].ToString().TrimEnd());
                _l.AutoAdjustDayLightSaving = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                _l.StreetAddress = dr["StreetAddress"].ToString().TrimEnd();                
                
                break;
            }
        }
        return new JavaScriptSerializer().Serialize(_l);
    }

    private class _results
    {
        public int status;
        public string geozonelandmarkname;
        public string objectType;
        public string message;
        public int isNew = 0;
    }

    // Changes for TimeZone Feature start

    private class _landmark_NewTZ
    {
        public int LandmarkId;
        public int OrganizationId;
        public string LandmarkName;
        public double Latitude;
        public double Longitude;
        public string Description;
        public string ContactPersonName;
        public string ContactPhoneNum;
        public int Radius;
        public string Email;
        public string Phone;
        public float? TimeZone;
        public bool DayLightSaving;
        public bool AutoAdjustDayLightSaving;
        public string StreetAddress;
    }

    // Changes for TimeZone Feature end

    private class _landmark
    {
        public int LandmarkId;
        public int OrganizationId;
        public string LandmarkName;
        public double Latitude;
        public double Longitude;
        public string Description;
        public string ContactPersonName;
        public string ContactPhoneNum;
        public int Radius;
        public string Email;
        public string Phone;
        public int? TimeZone;
        public bool DayLightSaving;
        public bool AutoAdjustDayLightSaving;
        public string StreetAddress;
    }

    // Changes for TimeZone Feature start

    private class _geozone_NewTZ
    {
        public int GeozoneNo;
        public int OrganizationId;
        public int GeozoneId;
        public string GeozoneName;
        public int Direction;
        public int GeozoneType;
        public int SeverityId;
        public string Description;
        public string Email;
        public string Phone;
        public float? TimeZone;
        public bool DayLightSaving;
        public bool Notify;
        public bool Warning;
        public bool Critical;
        public bool AutoAdjustDayLightSaving;
    }

    // Changes for TimeZone Feature end

    private class _geozone
    {
        public int GeozoneNo;
        public int OrganizationId;
        public int GeozoneId;
        public string GeozoneName;
        public int Direction;
        public int GeozoneType;
        public int SeverityId;
        public string Description;
        public string Email;
        public string Phone;
        public int? TimeZone;
        public bool DayLightSaving;
        public bool Notify;
        public bool Warning;
        public bool Critical;
        public bool AutoAdjustDayLightSaving; 
    }

    // Changes for TimeZone Feature start

    private void DgLandmarks_Fill_NewTZ(SentinelFMSession sn)
    {
        try
        {
            sn.Landmark.DgLandmarks_Fill_NewTZ(sn);
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
        }

        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

        }
    }

    // Changes for TimeZone Feature end

    private void DgLandmarks_Fill(SentinelFMSession sn)
    {
        try
        {
            sn.Landmark.DgLandmarks_Fill(sn);
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
        }

        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

        }
    }
    
}