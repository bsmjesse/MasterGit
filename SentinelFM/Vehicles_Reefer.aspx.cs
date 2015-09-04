using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using ClosedXML.Excel;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using Newtonsoft.Json;
using VLF.CLS.Def;
using VLF.DAS.Logic;
using SentinelFM.GeomarkServiceRef;
using System.Text.RegularExpressions;

namespace SentinelFM
{

    public partial class Vehicles_Reefer : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;

        public string _xml = "";
        protected clsUtility objUtil;

        private int vlStart;
        private int vlLimit;
        private string filters;
        private string operation;
        private string formattype;
       

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                sn = (SentinelFMSession)Session["SentinelFMSession"];
                
                if (!Page.IsPostBack)
                {
                    string request = Request.QueryString["QueryType"];
                    if (string.IsNullOrEmpty(request))
                    {
                        request = "GetVehiclePosition";
                        sn.Map.LastKnownXML = string.Empty; //Get everything again may be we lost session
                    }
                    if (request.Equals("GetVehiclePosition", StringComparison.CurrentCultureIgnoreCase))
                    {
                        request = Request.QueryString["fleetID"];
                        if (!string.IsNullOrEmpty(request))
                        {
                            int fleetID = 0;
                            Int32.TryParse(request, out fleetID);
                            if (fleetID > 0)
                            {
                                sn.Map.SelectedFleetID = fleetID;
                                sn.Map.LastKnownXML = string.Empty;
                            }
                        }

                        request = Request.QueryString["filters"];
                        if (!string.IsNullOrEmpty(request))
                            filters = request;
                        else
                            filters = String.Empty;

                        vlStart = 0;
                        vlLimit = 10000;
                        request = Request.QueryString["start"];
                        if (!string.IsNullOrEmpty(request))
                        {
                            Int32.TryParse(request, out vlStart);
                            if (vlStart < 0) vlStart = 0;
                        }

                        request = Request.QueryString["limit"];
                        if (!string.IsNullOrEmpty(request))
                        {
                            Int32.TryParse(request, out vlLimit);
                            if (vlLimit <= 0) vlStart = vlLimit;
                        }

                        VehicleList_Fill_NewTZ();
                    }
                    else if (request.Equals("GetAllFleets", StringComparison.CurrentCultureIgnoreCase))
                    {
                        //sn.Map.LastKnownXML = string.Empty;
                        Fleets_Fill();
                    }
                    else if (request.Equals("GetfleetPosition", StringComparison.CurrentCultureIgnoreCase))
                    {
                        request = Request.QueryString["fleetID"];
                        if (!string.IsNullOrEmpty(request))
                        {
                            int fleetID = 0;
                            Int32.TryParse(request, out fleetID);
                            if (fleetID > 0)
                            {
                                sn.Map.SelectedFleetID = fleetID;
                                sn.Map.LastKnownXML = string.Empty;
                            }
                        }

                        vlStart = 0;
                        vlLimit = 10000;
                        request = Request.QueryString["start"];
                        if (!string.IsNullOrEmpty(request))
                        {
                            Int32.TryParse(request, out vlStart);
                            if (vlStart < 0) vlStart = 0;
                        }

                        request = Request.QueryString["limit"];
                        if (!string.IsNullOrEmpty(request))
                        {
                            Int32.TryParse(request, out vlLimit);
                            if (vlLimit <= 0) vlStart = vlLimit;
                        }

                        FleetVehicles_Fill_NewTZ();
                    }
                    else if (request.Equals("getClosestVehicles", StringComparison.CurrentCultureIgnoreCase))
                    {
                        int fleetId = 0;
                        double lon = 0;
                        double lat = 0;
                        int radius = 5;
                        int numofvehicles = 10;
                        request = Request.QueryString["fleetID"] ?? string.Empty;
                        Int32.TryParse(request, out fleetId);
                        if (fleetId <= 0)
                        {
                            return;
                        }
                        request = Request.QueryString["lon"] ?? string.Empty;
                        double.TryParse(request, out lon);
                        request = Request.QueryString["lat"] ?? string.Empty;
                        double.TryParse(request, out lat);
                        request = Request.QueryString["radius"] ?? string.Empty;
                        int.TryParse(request, out radius);
                        request = Request.QueryString["numofvehicles"] ?? string.Empty;
                        int.TryParse(request, out numofvehicles);

                        getClosestVehicles_NewTZ(fleetId, lon, lat, numofvehicles, radius);

                    }
                    else if (request.Equals("searchHistoryAddress", StringComparison.CurrentCultureIgnoreCase))
                    {

                        //searchHistoryAddress();
                        searchHistoryAddressByWebservice_NewTZ();

                    }
                    else if (request.Equals("GetFilteredFleet", StringComparison.CurrentCultureIgnoreCase))
                    {
                        request = Request.QueryString["filters"];
                        if (!string.IsNullOrEmpty(request))
                            filters = request;
                        else
                            filters = String.Empty;

                        vlStart = 0;
                        vlLimit = 10000;
                        request = Request.QueryString["start"];
                        if (!string.IsNullOrEmpty(request))
                        {
                            Int32.TryParse(request, out vlStart);
                            if (vlStart < 0) vlStart = 0;
                        }

                        request = Request.QueryString["limit"];
                        if (!string.IsNullOrEmpty(request))
                        {
                            Int32.TryParse(request, out vlLimit);
                            if (vlLimit <= 0) vlStart = vlLimit;
                        }


                        request = Request.QueryString["operation"];
                        if (!string.IsNullOrEmpty(request))
                        {
                            operation = request;
                        }
                        else
                        {
                            operation = "";
                        }

                        request = Request.QueryString["formattype"];
                        if (!string.IsNullOrEmpty(request))
                            formattype = request;
                        else
                            formattype = "";

                        getFilteredFleet();
                    }
                    else if (request.Equals("GetSensorCommand", StringComparison.CurrentCultureIgnoreCase))
                    {
                        request = Request["boxid"];
                        if (!string.IsNullOrEmpty(request))
                        {
                            sn.Cmd.ArrVehicle.Add(request);
                        }
                        
                    }
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void Fleets_Fill()
        {
            string xml = "";
            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
            objUtil = new clsUtility(sn);

            //if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
            //  if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
            // {
            //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, " No Fleets for User:" + sn.UserID.ToString() + " Form:clsUser "));
            //   return;
            // }


            DataSet dsFleets = new DataSet();
            dsFleets = sn.User.GetUserFleets(sn);
            xml = dsFleets.GetXml();

            if (xml == "")
                return;

            Response.ContentType = "text/xml";
            //byte[] data = Encoding.Default.GetBytes(xml.Trim());
            //xml = Encoding.UTF8.GetString(data);
            Response.Write(xml.Trim());
        }

        // Changes for TimeZone Feature start

        private void FleetVehicles_Fill_NewTZ()
        {
            try
            {
                StringReader strrXML = null;
                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
                objUtil = new clsUtility(sn);

                Response.ContentType = "text/xml";
                //Response.ContentEncoding = Encoding.Default;
                Response.ContentEncoding = Encoding.UTF8;
                int fleetId = 0;
                if (sn.Map.SelectedFleetID != 0)
                {
                    fleetId = Convert.ToInt32(sn.Map.SelectedFleetID);
                    //Convert.ToInt32(Convert.ToInt32(sn.Map.SelectedFleetID));
                }
                else if (sn.User.DefaultFleet != -1)
                {
                    fleetId = Convert.ToInt32(sn.User.DefaultFleet);
                    //sn.Map.SelectedFleetID = sn.User.DefaultFleet;
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->VehicleList_Fill - Fleet Id:" + fleetId + ", User Id:" + sn.UserID.ToString()));
                sn.Map.LastStatusChecked = DateTime.UtcNow;
                //DateTime.Now;

                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang_NewTZ(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang_NewTZ(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        sn.Map.DsFleetInfoNew = null;
                        sn.Map.LastKnownXML = string.Empty;
                        return;
                    }

                if (!string.IsNullOrEmpty(xml))
                {
                    xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    sn.Map.LastKnownXML = xml.Trim();
                }

                if (string.IsNullOrEmpty(xml))
                //(xml == "" || xml == null)
                {
                    //sn.Map.DsFleetInfoNew = null;
                    if (sn.Map.SelectedFleetID == sn.User.DefaultFleet)
                    {
                        xml = sn.Map.DsFleetInfoNew.GetXml();
                        xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                        //byte[] data = Encoding.Default.GetBytes(xml);
                        //xml = Encoding.UTF8.GetString(data);
                        Response.Write(xml.Trim());
                    }
                    else
                        return;
                }
                else
                {
                    strrXML = new StringReader(xml.Trim());

                    string strPath = Server.MapPath("./Datasets/FleetInfo.xsd");
                    DataSet dsFleetInfo = new DataSet();
                    dsFleetInfo.ReadXmlSchema(strPath);
                    dsFleetInfo.ReadXml(strrXML);

                    ProceedReeferData(dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"]);

                    //xml = dsFleetInfo.GetXml();
                    //sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                    //sn.Map.DsFleetInfoNew = dsFleetInfo;
                    if (sn.Map.DsFleetInfoNew != null)
                    {
                        foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                        {
                            clsMap _m = new clsMap();
                            if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                            {
                                row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                            }
                            string filter = string.Format("BoxId = '{0}'", row["BoxId"]);
                            DataRow[] foundRows = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Select(filter);
                            if (foundRows.Length == 0)
                            {
                                // insert here
                                DataRow insertedRow = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].NewRow();
                                insertedRow.ItemArray = row.ItemArray;
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.Add(insertedRow);
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                            }
                            else
                            {
                                // update here
                                for (int i = 0; i < foundRows.Length; i++)
                                {
                                    try
                                    {
                                        int index = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.IndexOf(foundRows[i]);
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["BoxId"] = row["BoxId"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["LastCommunicatedDateTime"] = row["LastCommunicatedDateTime"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OriginDateTime"] = row["OriginDateTime"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Latitude"] = row["Latitude"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Longitude"] = row["Longitude"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["StreetAddress"] = row["StreetAddress"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Description"] = row["Description"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["BoxArmed"] = row["BoxArmed"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["IconTypeName"] = row["IconTypeName"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["VehicleStatus"] = row["VehicleStatus"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["chkBoxShow"] = row["chkBoxShow"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Updated"] = row["Updated"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["CustomUrl"] = row["CustomUrl"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Speed"] = row["Speed"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["CustomSpeed"] = row["CustomSpeed"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["MyHeading"] = row["MyHeading"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ProtocolId"] = row["ProtocolId"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["SensorMask"] = row["SensorMask"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Driver"] = row["Driver"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ImagePath"] = row["ImagePath"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ExtraInfo"] = row["ExtraInfo"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ReeferState"] = row["ReeferState"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Micro"] = row["Micro"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ControllerType"] = row["ControllerType"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["PowerOnOff"] = row["PowerOnOff"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ReeferPower"] = row["ReeferPower"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ModeOfOp"] = row["ModeOfOp"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["SDoor"] = row["SDoor"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["AFAX"] = row["AFAX"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Setpt"] = row["RF_Setpt"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Ret"] = row["RF_Ret"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Dis"] = row["RF_Dis"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_SensorProbe"] = row["RF_SensorProbe"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Amb"] = row["RF_Amb"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Setpt2"] = row["RF_Setpt2"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Ret2"] = row["RF_Ret2"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Dis2"] = row["RF_Dis2"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_SensorProbe2"] = row["RF_SensorProbe2"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Setpt3"] = row["RF_Setpt3"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Ret3"] = row["RF_Ret3"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Dis3"] = row["RF_Dis3"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_SensorProbe3"] = row["RF_SensorProbe3"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["FuelLevel"] = row["FuelLevel"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["FuelLevelGallon"] = row["FuelLevelGallon"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RPM"] = row["RPM"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["BatteryVoltage"] = row["BatteryVoltage"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["EngineHours"] = row["EngineHours"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["TetherOnOff"] = row["TetherOnOff"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                                    }
                                    catch (Exception Ex)
                                    {
                                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                                    }
                                }
                            }
                        }
                        if (vlStart == 0 && vlLimit == 10000)
                        {
                            xml = dsFleetInfo.GetXml();
                        }
                        else
                        {
                            DataSet dstemp = new DataSet();
                            DataView dv = dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].DefaultView;
                            dv.Sort = "OriginDateTime DESC";
                            DataTable sortedTable = dv.ToTable();
                            DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                            dt.TableName = "VehiclesLastKnownPositionInformation";
                            dstemp.Tables.Add(dt);
                            dstemp.DataSetName = "Fleet";
                            xml = dstemp.GetXml();
                            xml = xml.Replace("<Fleet>", "<Fleet><totalCount>" + dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows.Count.ToString() + "</totalCount>");
                        }


                    }
                    else
                    {
                        foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                        {
                            clsMap _m = new clsMap();
                            if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                            {
                                row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                            }
                        }
                        sn.Map.DsFleetInfoNew = dsFleetInfo;
                        //xml = dsFleetInfo.GetXml();
                        if (vlStart == 0 && vlLimit == 10000)
                        {
                            xml = dsFleetInfo.GetXml();
                        }
                        else
                        {
                            DataSet dstemp = new DataSet();
                            DataView dv = dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].DefaultView;
                            dv.Sort = "OriginDateTime DESC";
                            DataTable sortedTable = dv.ToTable();
                            DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                            dt.TableName = "VehiclesLastKnownPositionInformation";
                            dstemp.Tables.Add(dt);
                            dstemp.DataSetName = "Fleet";
                            xml = dstemp.GetXml();
                            xml = xml.Replace("<Fleet>", "<Fleet><totalCount>" + dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows.Count.ToString() + "</totalCount>");
                        }
                    }
                    sn.Map.DsFleetInfo = sn.Map.DsFleetInfoNew;
                    xml = xml.Replace("&#x0", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    //byte[] data = Encoding.Default.GetBytes(xml.Trim());
                    //xml = Encoding.UTF8.GetString(data);
                    Response.Write(xml.Trim());
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- VehicleList_Fill in Vehicles aspx User ID:" + sn.UserID));
                //Response.End();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        // Changes for TimeZone Feature end

        private void FleetVehicles_Fill()
        {
            try
            {
                StringReader strrXML = null;
                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
                objUtil = new clsUtility(sn);

                Response.ContentType = "text/xml";
                //Response.ContentEncoding = Encoding.Default;
                Response.ContentEncoding = Encoding.UTF8;
                int fleetId = 0;
                if (sn.Map.SelectedFleetID != 0)
                {
                    fleetId = Convert.ToInt32(sn.Map.SelectedFleetID);
                    //Convert.ToInt32(Convert.ToInt32(sn.Map.SelectedFleetID));
                }
                else if (sn.User.DefaultFleet != -1)
                {
                    fleetId = Convert.ToInt32(sn.User.DefaultFleet);
                    //sn.Map.SelectedFleetID = sn.User.DefaultFleet;
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->VehicleList_Fill - Fleet Id:" + fleetId + ", User Id:" + sn.UserID.ToString()));
                sn.Map.LastStatusChecked = DateTime.UtcNow;
                //DateTime.Now;

                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        sn.Map.DsFleetInfoNew = null;
                        sn.Map.LastKnownXML = string.Empty;
                        return;
                    }

                if (!string.IsNullOrEmpty(xml))
                {
                    xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    sn.Map.LastKnownXML = xml.Trim();
                }

                if (string.IsNullOrEmpty(xml))
                //(xml == "" || xml == null)
                {
                    //sn.Map.DsFleetInfoNew = null;
                    if (sn.Map.SelectedFleetID == sn.User.DefaultFleet)
                    {
                        xml = sn.Map.DsFleetInfoNew.GetXml();
                        xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                        //byte[] data = Encoding.Default.GetBytes(xml);
                        //xml = Encoding.UTF8.GetString(data);
                        Response.Write(xml.Trim());
                    }
                    else
                        return;
                }
                else
                {
                    strrXML = new StringReader(xml.Trim());

                    string strPath = Server.MapPath("./Datasets/FleetInfo.xsd");
                    DataSet dsFleetInfo = new DataSet();
                    dsFleetInfo.ReadXmlSchema(strPath);
                    dsFleetInfo.ReadXml(strrXML);
                    //xml = dsFleetInfo.GetXml();
                    //sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                    //sn.Map.DsFleetInfoNew = dsFleetInfo;
                    if (sn.Map.DsFleetInfoNew != null)
                    {
                        foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                        {
                            clsMap _m = new clsMap();
                            if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                            {
                                row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                            }
                            string filter = string.Format("BoxId = '{0}'", row["BoxId"]);
                            DataRow[] foundRows = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Select(filter);
                            if (foundRows.Length == 0)
                            {
                                // insert here
                                DataRow insertedRow = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].NewRow();
                                insertedRow.ItemArray = row.ItemArray;
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.Add(insertedRow);
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                            }
                            else
                            {
                                // update here
                                for (int i = 0; i < foundRows.Length; i++)
                                {
                                    try
                                    {
                                        int index = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.IndexOf(foundRows[i]);
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["BoxId"] = row["BoxId"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["LastCommunicatedDateTime"] = row["LastCommunicatedDateTime"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OriginDateTime"] = row["OriginDateTime"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Latitude"] = row["Latitude"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Longitude"] = row["Longitude"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["StreetAddress"] = row["StreetAddress"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Description"] = row["Description"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["BoxArmed"] = row["BoxArmed"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["IconTypeName"] = row["IconTypeName"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["VehicleStatus"] = row["VehicleStatus"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["chkBoxShow"] = row["chkBoxShow"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Updated"] = row["Updated"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["CustomUrl"] = row["CustomUrl"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Speed"] = row["Speed"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["CustomSpeed"] = row["CustomSpeed"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["MyHeading"] = row["MyHeading"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ProtocolId"] = row["ProtocolId"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["SensorMask"] = row["SensorMask"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Driver"] = row["Driver"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ImagePath"] = row["ImagePath"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ExtraInfo"] = row["ExtraInfo"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                                    }
                                    catch (Exception Ex)
                                    {
                                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                                    }
                                }
                            }
                        }
                        if (vlStart == 0 && vlLimit == 10000)
                        {
                            xml = dsFleetInfo.GetXml();
                        }
                        else
                        {
                            DataSet dstemp = new DataSet();
                            DataView dv = dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].DefaultView;
                            dv.Sort = "OriginDateTime DESC";
                            DataTable sortedTable = dv.ToTable();
                            DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                            dt.TableName = "VehiclesLastKnownPositionInformation";
                            dstemp.Tables.Add(dt);
                            dstemp.DataSetName = "Fleet";
                            xml = dstemp.GetXml();
                            xml = xml.Replace("<Fleet>", "<Fleet><totalCount>" + dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows.Count.ToString() + "</totalCount>");
                        }


                    }
                    else
                    {
                        foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                        {
                            clsMap _m = new clsMap();
                            if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                            {
                                row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                            }
                        }
                        sn.Map.DsFleetInfoNew = dsFleetInfo;
                        //xml = dsFleetInfo.GetXml();
                        if (vlStart == 0 && vlLimit == 10000)
                        {
                            xml = dsFleetInfo.GetXml();
                        }
                        else
                        {
                            DataSet dstemp = new DataSet();
                            DataView dv = dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].DefaultView;
                            dv.Sort = "OriginDateTime DESC";
                            DataTable sortedTable = dv.ToTable();
                            DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                            dt.TableName = "VehiclesLastKnownPositionInformation";
                            dstemp.Tables.Add(dt);
                            dstemp.DataSetName = "Fleet";
                            xml = dstemp.GetXml();
                            xml = xml.Replace("<Fleet>", "<Fleet><totalCount>" + dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows.Count.ToString() + "</totalCount>");
                        }
                    }
                    sn.Map.DsFleetInfo = sn.Map.DsFleetInfoNew;
                    xml = xml.Replace("&#x0", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    //byte[] data = Encoding.Default.GetBytes(xml.Trim());
                    //xml = Encoding.UTF8.GetString(data);
                    Response.Write(xml.Trim());
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- VehicleList_Fill in Vehicles aspx User ID:" + sn.UserID));
                //Response.End();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        // Changes for TimeZone Feature start

        private void VehicleList_Fill_NewTZ()
        {
            try
            {
                StringReader strrXML = null;
                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
                objUtil = new clsUtility(sn);

                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;

                int fleetId = 0;
                if (sn.Map.SelectedFleetID != 0)
                {
                    fleetId = Convert.ToInt32(sn.Map.SelectedFleetID);
                    //Convert.ToInt32(Convert.ToInt32(sn.Map.SelectedFleetID));
                }
                else if (sn.User.DefaultFleet != -1)
                {
                    fleetId = Convert.ToInt32(sn.User.DefaultFleet);
                    //sn.Map.SelectedFleetID = sn.User.DefaultFleet;
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->VehicleList_Fill - Fleet Id:" + fleetId + ", User Id:" + sn.UserID.ToString()));
                if (string.IsNullOrEmpty(sn.Map.LastKnownXML) || sn.Map.LastStatusChecked == null)
                {
                    sn.Map.LastStatusChecked = DateTime.UtcNow;
                    //DateTime.Now;

                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang_NewTZ(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                        if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang_NewTZ(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                        {
                            sn.Map.DsFleetInfoNew = null;
                            sn.Map.LastKnownXML = string.Empty;
                            return;
                        }

                    if (!string.IsNullOrEmpty(xml))
                    {
                        xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                        sn.Map.LastKnownXML = xml;
                    }
                }
                else
                {
                    DateTime lastCheckedTime = DateTime.UtcNow;
                    if (objUtil.ErrCheck(dbf.GetVehiclesChangedPositionInfoByLang_NewTZ(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.Map.LastStatusChecked, ref xml), false))
                        if (objUtil.ErrCheck(dbf.GetVehiclesChangedPositionInfoByLang_NewTZ(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.Map.LastStatusChecked, ref xml), true))
                        {
                            sn.Map.DsFleetInfoNew = null;
                            sn.Map.LastKnownXML = string.Empty;
                            return;
                        }

                    //DateTime.Now;                        
                    if (!string.IsNullOrEmpty(xml))
                    {
                        sn.Map.LastStatusChecked = lastCheckedTime;
                        xml = xml.Replace("VehiclesChangedPositionInformation", "VehiclesLastKnownPositionInformation");
                        xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    }
                }

                if (string.IsNullOrEmpty(xml))
                //(xml == "" || xml == null)
                {
                    //sn.Map.DsFleetInfoNew = null;
                    if (sn.Map.SelectedFleetID == sn.User.DefaultFleet)
                    {
                        xml = sn.Map.DsFleetInfoNew.GetXml();
                        xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                        //byte[] data = Encoding.Default.GetBytes(xml.Trim());
                        //xml = Encoding.UTF8.GetString(data);
                        Response.Write(xml.Trim());
                    }
                    else
                        return;
                }
                else
                {
                    strrXML = new StringReader(xml);

                    string strPath = Server.MapPath("./Datasets/FleetInfo.xsd");
                    DataSet dsFleetInfo = new DataSet();
                    dsFleetInfo.ReadXmlSchema(strPath);
                    dsFleetInfo.ReadXml(strrXML);
                    //xml = dsFleetInfo.GetXml();
                    //sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                    //sn.Map.DsFleetInfoNew = dsFleetInfo;

                    ProceedReeferData(dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"]);

                    if (sn.Map.DsFleetInfoNew != null)
                    {
                        foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                        {
                            clsMap _m = new clsMap();
                            if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                            {
                                row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                            }
                            string filter = string.Format("BoxId = '{0}'", row["BoxId"]);
                            DataRow[] foundRows = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Select(filter);
                            if (foundRows.Length == 0)
                            {
                                // insert here
                                DataRow insertedRow = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].NewRow();
                                insertedRow.ItemArray = row.ItemArray;
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.Add(insertedRow);
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                            }
                            else
                            {
                                // update here
                                for (int i = 0; i < foundRows.Length; i++)
                                {
                                    try
                                    {
                                        if (row["ExtraInfo"].ToString().Trim() != string.Empty)
                                        {
                                            int index = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.IndexOf(foundRows[i]);
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["BoxId"] = row["BoxId"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["LastCommunicatedDateTime"] = row["LastCommunicatedDateTime"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OriginDateTime"] = row["OriginDateTime"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Latitude"] = row["Latitude"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Longitude"] = row["Longitude"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["StreetAddress"] = row["StreetAddress"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Description"] = row["Description"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["BoxArmed"] = row["BoxArmed"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["IconTypeName"] = row["IconTypeName"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["VehicleStatus"] = row["VehicleStatus"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["chkBoxShow"] = row["chkBoxShow"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Updated"] = row["Updated"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["CustomUrl"] = row["CustomUrl"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Speed"] = row["Speed"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["CustomSpeed"] = row["CustomSpeed"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["MyHeading"] = row["MyHeading"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ProtocolId"] = row["ProtocolId"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["SensorMask"] = row["SensorMask"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Driver"] = row["Driver"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ImagePath"] = row["ImagePath"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ExtraInfo"] = row["ExtraInfo"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ReeferState"] = row["ReeferState"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Micro"] = row["Micro"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ControllerType"] = row["ControllerType"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["PowerOnOff"] = row["PowerOnOff"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ReeferPower"] = row["ReeferPower"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ModeOfOp"] = row["ModeOfOp"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["SDoor"] = row["SDoor"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["AFAX"] = row["AFAX"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Setpt"] = row["RF_Setpt"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Ret"] = row["RF_Ret"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Dis"] = row["RF_Dis"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_SensorProbe"] = row["RF_SensorProbe"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Amb"] = row["RF_Amb"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Setpt2"] = row["RF_Setpt2"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Ret2"] = row["RF_Ret2"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Dis2"] = row["RF_Dis2"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_SensorProbe2"] = row["RF_SensorProbe2"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Setpt3"] = row["RF_Setpt3"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Ret3"] = row["RF_Ret3"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_Dis3"] = row["RF_Dis3"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RF_SensorProbe3"] = row["RF_SensorProbe3"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["FuelLevel"] = row["FuelLevel"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["FuelLevelGallon"] = row["FuelLevelGallon"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["RPM"] = row["RPM"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["BatteryVoltage"] = row["BatteryVoltage"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["EngineHours"] = row["EngineHours"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["TetherOnOff"] = row["TetherOnOff"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                                        }
                                    }
                                    catch (Exception Ex)
                                    {
                                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                                    }
                                }
                            }
                        }
                        //xml = dsFleetInfo.GetXml();
                        xml = sn.Map.DsFleetInfoNew.GetXml();
                    }
                    else
                    {
                        foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                        {
                            clsMap _m = new clsMap();
                            if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                            {
                                row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                            }
                        }
                        sn.Map.DsFleetInfoNew = dsFleetInfo;
                        xml = dsFleetInfo.GetXml();
                    }
                    sn.Map.DsFleetInfo = sn.Map.DsFleetInfoNew;
                    //byte[] data = Encoding.Default.GetBytes(xml.Trim());
                    //xml = Encoding.UTF8.GetString(data);
                    xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    Response.Write(xml.Trim());
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- VehicleList_Fill in Vehicles aspx User ID:" + sn.UserID));
                //Response.End();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        // Changes for TimeZone Feature end
        private void VehicleList_Fill()
        {
            try
            {
                StringReader strrXML = null;
                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
                objUtil = new clsUtility(sn);

                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;

                int fleetId = 0;
                if (sn.Map.SelectedFleetID != 0)
                {
                    fleetId = Convert.ToInt32(sn.Map.SelectedFleetID);
                    //Convert.ToInt32(Convert.ToInt32(sn.Map.SelectedFleetID));
                }
                else if (sn.User.DefaultFleet != -1)
                {
                    fleetId = Convert.ToInt32(sn.User.DefaultFleet);
                    //sn.Map.SelectedFleetID = sn.User.DefaultFleet;
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->VehicleList_Fill - Fleet Id:" + fleetId + ", User Id:" + sn.UserID.ToString()));
                if (string.IsNullOrEmpty(sn.Map.LastKnownXML) || sn.Map.LastStatusChecked == null)
                {
                    sn.Map.LastStatusChecked = DateTime.UtcNow;
                    //DateTime.Now;

                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                        if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                        {
                            sn.Map.DsFleetInfoNew = null;
                            sn.Map.LastKnownXML = string.Empty;
                            return;
                        }

                    if (!string.IsNullOrEmpty(xml))
                    {
                        xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                        sn.Map.LastKnownXML = xml;
                    }
                }
                else
                {
                    DateTime lastCheckedTime = DateTime.UtcNow;
                    if (objUtil.ErrCheck(dbf.GetVehiclesChangedPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.Map.LastStatusChecked, ref xml), false))
                        if (objUtil.ErrCheck(dbf.GetVehiclesChangedPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.Map.LastStatusChecked, ref xml), true))
                        {
                            sn.Map.DsFleetInfoNew = null;
                            sn.Map.LastKnownXML = string.Empty;
                            return;
                        }

                    //DateTime.Now;                        
                    if (!string.IsNullOrEmpty(xml))
                    {
                        sn.Map.LastStatusChecked = lastCheckedTime;
                        xml = xml.Replace("VehiclesChangedPositionInformation", "VehiclesLastKnownPositionInformation");
                        xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    }
                }

                if (string.IsNullOrEmpty(xml))
                //(xml == "" || xml == null)
                {
                    //sn.Map.DsFleetInfoNew = null;
                    if (sn.Map.SelectedFleetID == sn.User.DefaultFleet)
                    {
                        xml = sn.Map.DsFleetInfoNew.GetXml();
                        xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                        //byte[] data = Encoding.Default.GetBytes(xml.Trim());
                        //xml = Encoding.UTF8.GetString(data);
                        Response.Write(xml.Trim());
                    }
                    else
                        return;
                }
                else
                {
                    strrXML = new StringReader(xml);

                    string strPath = Server.MapPath("./Datasets/FleetInfo.xsd");
                    DataSet dsFleetInfo = new DataSet();
                    dsFleetInfo.ReadXmlSchema(strPath);
                    dsFleetInfo.ReadXml(strrXML);
                    //xml = dsFleetInfo.GetXml();
                    //sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                    //sn.Map.DsFleetInfoNew = dsFleetInfo;
                    if (sn.Map.DsFleetInfoNew != null)
                    {
                        foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                        {
                            clsMap _m = new clsMap();
                            if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                            {
                                row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                            }
                            string filter = string.Format("BoxId = '{0}'", row["BoxId"]);
                            DataRow[] foundRows = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Select(filter);
                            if (foundRows.Length == 0)
                            {
                                // insert here
                                DataRow insertedRow = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].NewRow();
                                insertedRow.ItemArray = row.ItemArray;
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.Add(insertedRow);
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                            }
                            else
                            {
                                // update here
                                for (int i = 0; i < foundRows.Length; i++)
                                {
                                    try
                                    {
                                        if (row["ExtraInfo"].ToString().Trim() != string.Empty)
                                        {
                                            int index = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.IndexOf(foundRows[i]);
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["BoxId"] = row["BoxId"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["LastCommunicatedDateTime"] = row["LastCommunicatedDateTime"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OriginDateTime"] = row["OriginDateTime"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Latitude"] = row["Latitude"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Longitude"] = row["Longitude"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["StreetAddress"] = row["StreetAddress"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Description"] = row["Description"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["BoxArmed"] = row["BoxArmed"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["IconTypeName"] = row["IconTypeName"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["VehicleStatus"] = row["VehicleStatus"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["chkBoxShow"] = row["chkBoxShow"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Updated"] = row["Updated"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["CustomUrl"] = row["CustomUrl"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Speed"] = row["Speed"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["CustomSpeed"] = row["CustomSpeed"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["MyHeading"] = row["MyHeading"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ProtocolId"] = row["ProtocolId"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["SensorMask"] = row["SensorMask"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Driver"] = row["Driver"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ImagePath"] = row["ImagePath"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ExtraInfo"] = row["ExtraInfo"];
                                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                                        }
                                    }
                                    catch (Exception Ex)
                                    {
                                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                                    }
                                }
                            }
                        }
                        //xml = dsFleetInfo.GetXml();
                        xml = sn.Map.DsFleetInfoNew.GetXml();
                    }
                    else
                    {
                        foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                        {
                            clsMap _m = new clsMap();
                            if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                            {
                                row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                            }
                        }
                        sn.Map.DsFleetInfoNew = dsFleetInfo;
                        xml = dsFleetInfo.GetXml();
                    }
                    sn.Map.DsFleetInfo = sn.Map.DsFleetInfoNew;
                    //byte[] data = Encoding.Default.GetBytes(xml.Trim());
                    //xml = Encoding.UTF8.GetString(data);
                    xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    Response.Write(xml.Trim());
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- VehicleList_Fill in Vehicles aspx User ID:" + sn.UserID));
                //Response.End();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void getFilteredFleet()
        {
            try
            {
                //string xml = "";
                if (sn.Map.DsFleetInfo != null)
                {
                    //StringReader strrXML = new StringReader(sn.Map.DsFleetInfo.GetXml().Trim());
                    //string strPath = Server.MapPath("./Datasets/FleetInfo.xsd");          
                    //DataSet dstemp = new DataSet();
                    //dstemp.ReadXmlSchema(strPath);
                    //dstemp.ReadXml(strrXML);
                    DataSet dstemp = new DataSet();
                    dstemp = sn.Map.DsFleetInfo.Copy();

                    DataTable sortedTable = dstemp.Tables[0];
                    //Edited by Rohit Mittal                    
                    int intialRowCount = sortedTable.Rows.Count;
                    string[] filterarray = filters.Split(',');
                    if (!String.IsNullOrEmpty(filters) && filterarray.Length > 0)
                    {
                        foreach (string s in filterarray)
                        {
                            if (String.IsNullOrEmpty(s))
                                continue;
                            string filtercol = s.Split(':')[0];
                            string filtervalue = s.Split(':')[1].Replace("\"", "");
                            if (filtervalue.Contains("type int") || filtervalue.Contains("type float"))
                            {
                                if (filtervalue.Contains("lt"))
                                {
                                    string col = filtervalue.Substring(filtervalue.LastIndexOf("lt")).Split(' ')[1];
                                    sortedTable = sortedTable.Select(filtercol + " <" + col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("gt"))
                                {
                                    string col = filtervalue.Substring(filtervalue.LastIndexOf("gt")).Split(' ')[1];
                                    sortedTable = sortedTable.Select(filtercol + " >" + col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("eq"))
                                {
                                    string col = filtervalue.Substring(filtervalue.LastIndexOf("eq")).Split(' ')[1];
                                    sortedTable = sortedTable.Select(string.Format("{0} = {1}", filtercol, col)).CopyToDataTable();
                                }

                            }
                            else if (filtervalue.Contains("type date"))
                            {
                                if (filtervalue.Contains("before"))
                                {
                                    string col = filtercol + " < #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("before")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + "#";
                                    sortedTable = sortedTable.Select(col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("after"))
                                {

                                    string col = filtercol + " > #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("after")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + "#";
                                    sortedTable = sortedTable.Select(col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("on"))
                                {
                                    string col = filtercol + " < #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("on")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + " 23:59:59" + "#";
                                    col += " AND " + filtercol + " > #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("on")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + " 00:00:00" + "#";
                                    sortedTable = sortedTable.Select(col).CopyToDataTable();
                                }

                            }
                            else
                            {
                                if (sortedTable.Columns[filtercol].DataType == typeof(System.Boolean))
                                {
                                    sortedTable = sortedTable.Select(string.Format("{0} = '{1}'", filtercol, filtervalue)).CopyToDataTable();
                                }
                                else if (filtercol == "OperationalStateName")
                                    sortedTable = sortedTable.Select(string.Format("{0} LIKE '{1}%'", filtercol, filtervalue)).CopyToDataTable();
                                else
                                    sortedTable = sortedTable.Select(string.Format("{0} LIKE '%{1}%'", filtercol, filtervalue)).CopyToDataTable();
                            }
                        }
                    }
                    string request = Request.QueryString["sorting"];
                    if (!string.IsNullOrEmpty(request))
                    {
                        sortedTable.DefaultView.Sort = request.Split(',')[0] + " " + request.Split(',')[1];
                        sortedTable = sortedTable.DefaultView.ToTable();
                    }
                    else
                    {
                        sortedTable.DefaultView.Sort = "OriginDateTime DESC";
                        sortedTable = sortedTable.DefaultView.ToTable();
                    }
                    int finalRowCount = sortedTable.Rows.Count;
                    bool resolveAddress = false;
                    if (finalRowCount > vlStart && operation != "Export")
                    {
                        sortedTable = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                        resolveAddress = true;
                    }
                    else if (operation != "Export")
                    {
                        sortedTable = sortedTable.AsEnumerable().Take(vlLimit).CopyToDataTable();
                        resolveAddress = true;
                    }
                    sortedTable.TableName = "VehiclesLastKnownPositionInformation";

                    if (resolveAddress)
                    {
                        foreach (DataRow row in sortedTable.Rows)
                        {
                            if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                            {
                                clsMap _m = new clsMap();
                                row["StreetAddress"] = _m.ResolveStreetAddressNavteq(row["Latitude"].ToString(), row["Longitude"].ToString());

                                string filter = string.Format("BoxId = '{0}'", row["BoxId"]);
                                DataRow[] foundRows = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Select(filter);
                                for (int i = 0; i < foundRows.Length; i++)
                                {
                                    try
                                    {
                                        int index = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.IndexOf(foundRows[i]);
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["StreetAddress"] = row["StreetAddress"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                                    }
                                    catch (Exception Ex)
                                    {
                                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                                    }
                                }
                            }

                        }
                    }

                    if (operation == "Export" && !String.IsNullOrEmpty(formattype))
                    {
                        request = Request.QueryString["columns"];
                        if (!string.IsNullOrEmpty(request))
                        {
                            exportDatatable(sortedTable, formattype, request);
                            return;
                        }
                    }

                    if (dstemp.Tables.CanRemove(dstemp.Tables[0]))
                    {
                        dstemp.Tables.Remove(dstemp.Tables[0]);
                    }
                    dstemp.Tables.Add(sortedTable);
                    dstemp.DataSetName = "Fleet";
                    //xml = dstemp.GetXml();
                    //if (!String.IsNullOrEmpty(filters) && filterarray.Length > 0)
                    //    xml = xml.Replace("<Fleet>", "<Fleet><totalCount>" + finalRowCount.ToString() + "</totalCount>");
                    //else
                    //    xml = xml.Replace("<Fleet>", "<Fleet><totalCount>" + intialRowCount.ToString() + "</totalCount>");

                    int totalCount = 0;
                    if (!String.IsNullOrEmpty(filters) && filterarray.Length > 0)
                        totalCount = finalRowCount;
                    else
                        totalCount = intialRowCount;

                    Response.ContentType = "text/xml";
                    Response.ContentEncoding = Encoding.UTF8;
                    //Response.Write(xml.Trim());
                    getXmlFromDs(dstemp, totalCount);
                    //Response.End();
                }
            }
            catch (Exception Ex)
            {
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write("<error>" + Ex.Message.ToString() + Ex.StackTrace.ToString() + "</error>");
            }
        }

        private void getXmlFromDs(DataSet ds, int c)
        {
            string lng = ((sn.SelectedLanguage != null && sn.SelectedLanguage.Length > 0) ? sn.SelectedLanguage.Substring(0, 2) : "en");
            Response.Write("<" + ds.DataSetName + "><totalCount>" + c.ToString() + "</totalCount>");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Response.Write("<" + ds.Tables[0].TableName + ">");
                foreach (DataColumn column in ds.Tables[0].Columns)
                {
                    Response.Write("<" + column.ColumnName + ">");
                    string v = "";
                    if (column.DataType.Name == "DateTime")
                        v = String.Format("{0:yyyy-MM-ddTHH:mm:ss.ff}", dr[column]);
                    else
                    {
                        v = dr[column].ToString();
                        if (lng == "fr")
                        {
                            if (column.ColumnName == "BoxArmed")
                            {
                                v = v.Replace("true", "voir");
                                v = v.Replace("false", "faux");
                            }
                        }
                        v = v.Replace("&#x0", "").Replace("&", "&amp;").Replace("\0", string.Empty);
                    }
                    //byte[] data = Encoding.Default.GetBytes(v);
                    //v = Encoding.UTF8.GetString(data);
                    Response.Write(RemoveInvalidXmlChars(v));
                    Response.Write("</" + column.ColumnName + ">");
                }
                Response.Write("</" + ds.Tables[0].TableName + ">");
            }
            Response.Write("</" + ds.DataSetName + ">");
        }

        private string RemoveInvalidXmlChars(string text)
        {
            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(text, re, "");
        }

        private string getExcelDateFormat()
        {
            string dformat = "yyyy-mm-dd";
            string tformat;
            if (sn.User.DateFormat == "dd/MM/yyyy")
                dformat = "dd/mm/yyyy";
            else if (sn.User.DateFormat == "d/M/yyyy")
                dformat = "d/m/yyyy";
            else if (sn.User.DateFormat == "dd/MM/yy")
                dformat = "dd/mm/yy";
            else if (sn.User.DateFormat == "d/M/yy")
                dformat = "d/m/yy";
            else if (sn.User.DateFormat == "d MMM yyyy")
                dformat = "d mmmm yyyy";
            else if (sn.User.DateFormat == "MM/dd/yyyy")
                dformat = "mm/dd/yyyy";
            else if (sn.User.DateFormat == "M/d/yyyy")
                dformat = "m/d/yyyy";
            else if (sn.User.DateFormat == "MM/dd/yy")
                dformat = "mm/dd/yy";
            else if (sn.User.DateFormat == "M/d/yy")
                dformat = "m/d/yy";
            else if (sn.User.DateFormat == "MMMM d yy")
                dformat = "mmmm d yy";
            else if (sn.User.DateFormat == "yyyy/MM/dd")
                dformat = "yyyy/mm/dd";

            if (sn.User.TimeFormat == "hh:mm:ss tt")
                tformat = "h:mm:ss AM/PM";
            else
                tformat = "h:mm:ss";

            return dformat + " " + tformat;
        }

        private void exportDatatable(DataTable dt, string formatter, string columns)
        {
            try
            {
                string exceldtformat = getExcelDateFormat();

                if (formatter == "csv")
                {

                    StringBuilder sresult = new StringBuilder();
                    sresult.Append("sep=,");
                    sresult.Append(Environment.NewLine);
                    string header = string.Empty;
                    foreach (string column in columns.Split(','))
                    {
                        string s = column.Split(':')[0];
                        header += "\"" + s + "\",";
                    }
                    header = header.Substring(0, header.Length - 1);
                    sresult.Append(header);
                    sresult.Append(Environment.NewLine);

                    foreach (DataRow row in dt.Rows)
                    {
                        string data = string.Empty;
                        foreach (string column in columns.Split(','))
                        {
                            string s = string.Empty;
                            if (column.Split(':')[1] == "LatLon")
                            {
                                s = row["Latitude"].ToString() + "," + row["Longitude"].ToString();
                            }
                            else
                                s = row[column.Split(':')[1]].ToString();
                            if (column.Split(':')[1] == "OriginDateTime")
                                s = Convert.ToDateTime(s).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                            data += "\"" + s.Replace("[br]", Environment.NewLine).Replace("\"", "\"\"") + "\",";
                        }
                        data = data.Substring(0, data.Length - 1);
                        sresult.Append(data);
                        sresult.Append(Environment.NewLine);
                    }

                    Response.Clear();
                    Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.csv", "vehicles"));
                    Response.Charset = Encoding.GetEncoding("iso-8859-1").BodyName;
                    Response.ContentType = "application/csv";
                    Response.ContentEncoding = Encoding.GetEncoding("iso-8859-1");
                    Response.Write(sresult.ToString());
                    Response.Flush();
                }
                else if (formatter == "excel2003")
                {
                    HSSFWorkbook wb = new HSSFWorkbook();
                    ISheet ws = wb.CreateSheet("Sheet1");
                    ICellStyle cellstyle1 = wb.CreateCellStyle();
                    ICellStyle cellstyle2 = wb.CreateCellStyle();
                    ICellStyle cellstyle3 = wb.CreateCellStyle();
                    ICellStyle cellstyle4 = wb.CreateCellStyle();
                    ICellStyle cellstyle5 = wb.CreateCellStyle();
                    cellstyle1.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    cellstyle2.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    cellstyle3.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    cellstyle4.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    cellstyle5.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    HSSFPalette palette = wb.GetCustomPalette();
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.SEA_GREEN.index, (byte)123, (byte)178, (byte)115);
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.YELLOW.index, (byte)239, (byte)215, (byte)0);
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.LIGHT_ORANGE.index, (byte)255, (byte)166, (byte)74);
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.ROSE.index, (byte)222, (byte)121, (byte)115);
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.INDIGO.index, (byte)99, (byte)125, (byte)165);
                    cellstyle1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.SEA_GREEN.index;
                    cellstyle2.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
                    cellstyle3.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_ORANGE.index;
                    cellstyle4.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.ROSE.index;
                    cellstyle5.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.INDIGO.index;
                    IRow row = ws.CreateRow(0);
                    foreach (string column in columns.Split(','))
                    {
                        string s = column.Split(':')[0];
                        row.CreateCell(row.Cells.Count).SetCellValue(s);
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string data = string.Empty;
                        IRow rowData = ws.CreateRow(i + 1);
                        foreach (string column in columns.Split(','))
                        {
                            if (column.Split(':')[1] == "OriginDateTime")
                            {
                                DateTime currentDate = DateTime.Now.ToUniversalTime();
                                DateTime recordDate;

                                string datadate = Convert.ToDateTime(dt.Rows[i][column.Split(':')[1]].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                                //rowData.CreateCell(rowData.Cells.Count).SetCellValue(datadate.Replace("[br]", Environment.NewLine));
                                rowData.CreateCell(rowData.Cells.Count).SetCellValue(Convert.ToDateTime(dt.Rows[i][column.Split(':')[1]].ToString()));
                                recordDate = DateTime.ParseExact(datadate, sn.User.DateFormat + " " + sn.User.TimeFormat, System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();

                                TimeSpan diffDate = currentDate.Subtract(recordDate);

                                if (diffDate.TotalHours < 24)
                                {
                                    cellstyle1.WrapText = true;
                                    cellstyle1.VerticalAlignment = VerticalAlignment.TOP;
                                    cellstyle1.DataFormat = wb.CreateDataFormat().GetFormat(exceldtformat);
                                    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle1;
                                }
                                else if (diffDate.TotalHours < 48)
                                {
                                    cellstyle2.WrapText = true;
                                    cellstyle2.VerticalAlignment = VerticalAlignment.TOP;
                                    cellstyle2.DataFormat = wb.CreateDataFormat().GetFormat(exceldtformat);
                                    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle2;
                                }
                                else if (diffDate.TotalHours < 72)
                                {
                                    cellstyle3.WrapText = true;
                                    cellstyle3.VerticalAlignment = VerticalAlignment.TOP;
                                    cellstyle3.DataFormat = wb.CreateDataFormat().GetFormat(exceldtformat);
                                    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle3;
                                }
                                else if (diffDate.TotalHours < 168)
                                {
                                    cellstyle4.WrapText = true;
                                    cellstyle4.VerticalAlignment = VerticalAlignment.TOP;
                                    cellstyle4.DataFormat = wb.CreateDataFormat().GetFormat(exceldtformat);
                                    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle4;
                                }
                                else if (diffDate.TotalHours > 168)
                                {
                                    cellstyle5.WrapText = true;
                                    cellstyle5.VerticalAlignment = VerticalAlignment.TOP;
                                    cellstyle5.DataFormat = wb.CreateDataFormat().GetFormat(exceldtformat);
                                    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle5;
                                }

                            }
                            else if (column.Split(':')[1] == "LatLon")
                            {
                                rowData.CreateCell(rowData.Cells.Count).SetCellValue(dt.Rows[i]["Latitude"].ToString() + "," + dt.Rows[i]["Longitude"].ToString());
                            }
                            else
                                rowData.CreateCell(rowData.Cells.Count).SetCellValue(dt.Rows[i][column.Split(':')[1]].ToString().Replace("[br]", Environment.NewLine));

                        }
                    }

                    for (int i = 0; i < columns.Split(',').Length; i++)
                    {
                        try
                        {
                            ws.AutoSizeColumn(i);
                        }
                        catch { }
                    }

                    HttpResponse httpResponse = Response;
                    httpResponse.Clear();
                    //httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //httpResponse.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xls", "Vehicle"));
                    Response.AddHeader("Content-Type", "application/Excel");
                    Response.ContentType = "application/force-download";
                    Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xls", "vehicles"));

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        wb.Write(memoryStream);
                        memoryStream.WriteTo(httpResponse.OutputStream);
                        memoryStream.Close();
                    }

                    //HttpContext.Current.Response.End();
                }
                else if (formatter == "excel2007")
                {
                    try
                    {
                        var wb = new XLWorkbook();
                        var ws = wb.Worksheets.Add("Sheet1");
                        foreach (string column in columns.Split(','))
                        {
                            string s = column.Split(':')[0];
                            ws.Cell(1, ws.Row(1).CellsUsed().Count() + 1).Value = s;
                        }

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string data = string.Empty;
                            int iColumn = 1;
                            foreach (string column in columns.Split(','))
                            {
                                ws.Cell(i + 2, iColumn).DataType = XLCellValues.Text;

                                if (column.Split(':')[1] == "OriginDateTime")
                                {
                                    DateTime currentDate = DateTime.Now.ToUniversalTime();
                                    DateTime recordDate;


                                    ws.Cell(i + 2, iColumn).DataType = XLCellValues.DateTime;
                                    ws.Cell(i + 2, iColumn).Value = dt.Rows[i][column.Split(':')[1]];
                                    ws.Cell(i + 2, iColumn).Style.DateFormat.Format = exceldtformat;

                                    string datadate = Convert.ToDateTime(dt.Rows[i][column.Split(':')[1]].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                                    //ws.Cell(i + 2, iColumn).Value = "'" + datadate.Replace("[br]", Environment.NewLine);
                                    recordDate = DateTime.ParseExact(datadate, sn.User.DateFormat + " " + sn.User.TimeFormat, System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();

                                    TimeSpan diffDate = currentDate.Subtract(recordDate);

                                    if (diffDate.TotalHours < 24)
                                    {
                                        ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#7BB273");
                                    }
                                    else if (diffDate.TotalHours < 48)
                                    {
                                        ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#EFD700");
                                    }
                                    else if (diffDate.TotalHours < 72)
                                    {
                                        ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFA64A");
                                    }
                                    else if (diffDate.TotalHours < 168)
                                    {
                                        ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#DE7973");
                                    }
                                    else if (diffDate.TotalHours > 168)
                                    {
                                        ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#637DA5");
                                    }

                                }
                                else if (column.Split(':')[1] == "LatLon")
                                {
                                    ws.Cell(i + 2, iColumn).Value = "'" + dt.Rows[i]["Latitude"].ToString() + "," + dt.Rows[i]["Longitude"].ToString();
                                }
                                else
                                    ws.Cell(i + 2, iColumn).Value = "'" + dt.Rows[i][column.Split(':')[1]].ToString().Replace("[br]", Environment.NewLine);

                                iColumn++;
                            }
                        }

                        //ws.Rows().Style.Alignment.SetWrapText();
                        //ws.Rows().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        //ws.Columns().AdjustToContents();

                        try
                        {
                            var files = new DirectoryInfo(Server.MapPath("TempReports/")).GetFiles("*.xlsx");
                            foreach (var file in files)
                            {
                                if (DateTime.UtcNow - file.LastWriteTimeUtc > TimeSpan.FromDays(30))
                                {
                                    File.Delete(file.FullName);
                                }
                            }
                        }
                        catch { }

                        Response.Clear();
                        Response.AddHeader("Content-Type", "application/Excel");
                        Response.ContentType = "application/force-download";
                        Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xlsx", "vehicles"));
                        string filemame = string.Format(@"{0}.xlsx", Guid.NewGuid());
                        wb.SaveAs(Server.MapPath("TempReports/") + filemame);
                        Response.TransmitFile("TempReports/" + filemame);
                    }
                    //Peter Editted
                    catch (Exception Ex)
                    {
                        Response.Write("<script type='text/javascript'>alert('Failed to generate the file, please try it again or choose another Excel format to export.');</script>");
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }

                }
            }
            //Peter Editted
            catch (Exception Ex)
            {
                Response.Write("<script type='text/javascript'>alert('Failed to generate the file, please try it again or choose another Excel format to export.');</script>");
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        // Changes for TimeZone Feature start

        private void getClosestVehicles_NewTZ(int fleetId, double lon, double lat, int searchNumVehicles, int searchRadius)
        {
            try
            {
                StringReader strrXML = null;

                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
                objUtil = new clsUtility(sn);

                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->getClosestVehicles - Fleet Id:" + fleetId + ", User Id:" + sn.UserID.ToString()));

                ServerDBFleet.DBFleet fleet = new ServerDBFleet.DBFleet();
                string xml = string.Empty;
                if (objUtil.ErrCheck(fleet.GetVehiclesLastKnownPositionInfoNearestToLatLon_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, fleetId, searchRadius, lat, lon, ref xml), false))
                    if (objUtil.ErrCheck(fleet.GetVehiclesLastKnownPositionInfoNearestToLatLon_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, fleetId, searchRadius, lat, lon, ref xml), true))
                    {
                        return;
                    }

                if (!string.IsNullOrEmpty(xml))
                {
                    xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                }


                if (string.IsNullOrEmpty(xml))
                //(xml == "" || xml == null)
                {
                    return;
                }
                else
                {
                    strrXML = new StringReader(xml);

                    string strPath = Server.MapPath("./Datasets/FleetInfo.xsd");
                    DataSet dsFleetInfo = new DataSet();
                    dsFleetInfo.ReadXmlSchema(strPath);
                    dsFleetInfo.ReadXml(strrXML);

                    DataColumn dc = new DataColumn("distance", Type.GetType("System.Double"));
                    dc.DefaultValue = 0;
                    dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);

                    foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                    {
                        clsMap _m = new clsMap();
                        if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                        {
                            row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                        }

                        double distance = BSMDistanceAlgorithm.DistanceBetweenPlaces(lon, lat, double.Parse(row["Longitude"].ToString()), double.Parse(row["Latitude"].ToString()));
                        row["distance"] = distance;
                    }

                    DataSet dstemp = new DataSet();
                    DataView dv = dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].DefaultView;
                    dv.Sort = "distance";
                    DataTable sortedTable = dv.ToTable();
                    DataTable dt = sortedTable.AsEnumerable().Skip(0).Take(searchNumVehicles).CopyToDataTable();
                    dt.TableName = "VehiclesLastKnownPositionInformation";
                    dstemp.Tables.Add(dt);
                    dstemp.DataSetName = "Fleet";
                    xml = dstemp.GetXml();

                    //xml = dsFleetInfo.GetXml();

                    xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    Response.Write(xml.Trim());
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- VehicleList_Fill in Vehicles aspx User ID:" + sn.UserID));
                //Response.End();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        // Changes for TimeZone Feature end

        private void getClosestVehicles(int fleetId, double lon, double lat, int searchNumVehicles, int searchRadius)
        {
            try
            {
                StringReader strrXML = null;

                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
                objUtil = new clsUtility(sn);

                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->getClosestVehicles - Fleet Id:" + fleetId + ", User Id:" + sn.UserID.ToString()));

                ServerDBFleet.DBFleet fleet = new ServerDBFleet.DBFleet();
                string xml = string.Empty;
                if (objUtil.ErrCheck(fleet.GetVehiclesLastKnownPositionInfoNearestToLatLon(sn.UserID, sn.SecId, sn.User.OrganizationId, fleetId, searchRadius, lat, lon, ref xml), false))
                    if (objUtil.ErrCheck(fleet.GetVehiclesLastKnownPositionInfoNearestToLatLon(sn.UserID, sn.SecId, sn.User.OrganizationId, fleetId, searchRadius, lat, lon, ref xml), true))
                    {
                        return;
                    }

                if (!string.IsNullOrEmpty(xml))
                {
                    xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                }


                if (string.IsNullOrEmpty(xml))
                //(xml == "" || xml == null)
                {
                    return;
                }
                else
                {
                    strrXML = new StringReader(xml);

                    string strPath = Server.MapPath("./Datasets/FleetInfo.xsd");
                    DataSet dsFleetInfo = new DataSet();
                    dsFleetInfo.ReadXmlSchema(strPath);
                    dsFleetInfo.ReadXml(strrXML);

                    DataColumn dc = new DataColumn("distance", Type.GetType("System.Double"));
                    dc.DefaultValue = 0;
                    dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);

                    foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                    {
                        clsMap _m = new clsMap();
                        if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                        {
                            row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                        }

                        double distance = BSMDistanceAlgorithm.DistanceBetweenPlaces(lon, lat, double.Parse(row["Longitude"].ToString()), double.Parse(row["Latitude"].ToString()));
                        row["distance"] = distance;
                    }

                    DataSet dstemp = new DataSet();
                    DataView dv = dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].DefaultView;
                    dv.Sort = "distance";
                    DataTable sortedTable = dv.ToTable();
                    DataTable dt = sortedTable.AsEnumerable().Skip(0).Take(searchNumVehicles).CopyToDataTable();
                    dt.TableName = "VehiclesLastKnownPositionInformation";
                    dstemp.Tables.Add(dt);
                    dstemp.DataSetName = "Fleet";
                    xml = dstemp.GetXml();

                    //xml = dsFleetInfo.GetXml();

                    xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    Response.Write(xml.Trim());
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- VehicleList_Fill in Vehicles aspx User ID:" + sn.UserID));
                //Response.End();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }


        // Changes for TimeZone Feature start

        private void searchHistoryAddressByWebservice_NewTZ()
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;

            try
            {
                ServerDBHistory.DBHistory dbhistory = new ServerDBHistory.DBHistory();
                //dbhistory.get

                objUtil = new clsUtility(sn);

                double lon = 0;
                double lat = 0;
                string SearchHistoryDateTime = Request["SearchHistoryDateTime"].ToString();
                int SearchHistoryTimeRange = 60;
                double radius = 2000;
                double.TryParse(Request["lon"].ToString(), out lon);
                double.TryParse(Request["lat"].ToString(), out lat);
                int.TryParse(Request["SearchHistoryTimeRange"].ToString(), out SearchHistoryTimeRange);
                double.TryParse(Request["radius"].ToString(), out radius);
                string PolygonPoints = Request["mapSearchPointSets"].ToString();

                string dtfrom = DateTime.ParseExact(SearchHistoryDateTime, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).AddMinutes(-SearchHistoryTimeRange).ToString("MM/dd/yyyy HH:mm:ss");
                string dtto = DateTime.ParseExact(SearchHistoryDateTime, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).AddMinutes(SearchHistoryTimeRange).ToString("MM/dd/yyyy HH:mm:ss");

                /*string xml = "<Fleet>";
                xml += "<VehiclesHistoryAddressInformation>";
                xml += "<BoxId>939393</BoxId>";
                xml += "<VehicleId>44562</VehicleId>";
                xml += "<LicensePlate>LP_939393</LicensePlate>";
                xml += "<Description>939393-QingQing</Description>";
                xml += "</VehiclesHistoryAddressInformation>";
                xml += "<VehiclesHistoryAddressInformation>";
                xml += "<BoxId>200200</BoxId>";
                xml += "<VehicleId>31320</VehicleId>";
                xml += "<LicensePlate>LP_200200</LicensePlate>";
                xml += "<Description>Ravi_3000_Test_Unit</Description>";
                xml += "</VehiclesHistoryAddressInformation>";
                xml += "</Fleet>";*/
                dbhistory.Timeout = -1;
                string xml = string.Empty;
                if (objUtil.ErrCheck(dbhistory.GetVehicleAreaSearch(sn.UserID, sn.SecId, lat, lon, radius, dtfrom, dtto, sn.User.OrganizationId, 0, PolygonPoints, "", "", ref xml), false))
                    if (objUtil.ErrCheck(dbhistory.GetVehicleAreaSearch(sn.UserID, sn.SecId, lat, lon, radius, dtfrom, dtto, sn.User.OrganizationId, 0, PolygonPoints, "", "", ref xml), true))
                    {
                        return;
                    }

                if (!string.IsNullOrEmpty(xml))
                {
                    xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                }
                Response.Write(xml);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                Response.Write("<error>" + Ex.ToString() + "</error>");

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                Response.Write("<error>" + Ex.ToString() + "</error>");
            }
        }

        // Changes for TimeZone Feature end
        private void searchHistoryAddressByWebservice()
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;

            try
            {
                ServerDBHistory.DBHistory dbhistory = new ServerDBHistory.DBHistory();
                //dbhistory.get

                objUtil = new clsUtility(sn);

                double lon = 0;
                double lat = 0;
                string SearchHistoryDateTime = Request["SearchHistoryDateTime"].ToString();
                int SearchHistoryTimeRange = 60;
                double radius = 2000;
                double.TryParse(Request["lon"].ToString(), out lon);
                double.TryParse(Request["lat"].ToString(), out lat);
                int.TryParse(Request["SearchHistoryTimeRange"].ToString(), out SearchHistoryTimeRange);
                double.TryParse(Request["radius"].ToString(), out radius);
                string PolygonPoints = Request["mapSearchPointSets"].ToString();

                string dtfrom = DateTime.ParseExact(SearchHistoryDateTime, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).AddMinutes(-SearchHistoryTimeRange).ToString("MM/dd/yyyy HH:mm:ss");
                string dtto = DateTime.ParseExact(SearchHistoryDateTime, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).AddMinutes(SearchHistoryTimeRange).ToString("MM/dd/yyyy HH:mm:ss");

                /*string xml = "<Fleet>";
                xml += "<VehiclesHistoryAddressInformation>";
                xml += "<BoxId>939393</BoxId>";
                xml += "<VehicleId>44562</VehicleId>";
                xml += "<LicensePlate>LP_939393</LicensePlate>";
                xml += "<Description>939393-QingQing</Description>";
                xml += "</VehiclesHistoryAddressInformation>";
                xml += "<VehiclesHistoryAddressInformation>";
                xml += "<BoxId>200200</BoxId>";
                xml += "<VehicleId>31320</VehicleId>";
                xml += "<LicensePlate>LP_200200</LicensePlate>";
                xml += "<Description>Ravi_3000_Test_Unit</Description>";
                xml += "</VehiclesHistoryAddressInformation>";
                xml += "</Fleet>";*/
                dbhistory.Timeout = -1;
                string xml = string.Empty;
                if (objUtil.ErrCheck(dbhistory.GetVehicleAreaSearch(sn.UserID, sn.SecId, lat, lon, radius, dtfrom, dtto, sn.User.OrganizationId, 0, PolygonPoints, "", "", ref xml), false))
                    if (objUtil.ErrCheck(dbhistory.GetVehicleAreaSearch(sn.UserID, sn.SecId, lat, lon, radius, dtfrom, dtto, sn.User.OrganizationId, 0, PolygonPoints, "", "", ref xml), true))
                    {
                        return;
                    }

                if (!string.IsNullOrEmpty(xml))
                {
                    xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                }
                Response.Write(xml);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                Response.Write("<error>" + Ex.ToString() + "</error>");

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                Response.Write("<error>" + Ex.ToString() + "</error>");
            }
        }

        /*private void searchHistoryAddress()
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;

            try
            {
                

                objUtil = new clsUtility(sn);

                double lon = 0;
                double lat = 0;
                string SearchHistoryDateTime = Request["SearchHistoryDateTime"].ToString();
                int SearchHistoryTimeRange = 60;
                double radius = 2000;
                double.TryParse(Request["lon"].ToString(), out lon);
                double.TryParse(Request["lat"].ToString(), out lat);
                int.TryParse(Request["SearchHistoryTimeRange"].ToString(), out SearchHistoryTimeRange);
                double.TryParse(Request["radius"].ToString(), out radius);

                string dtfrom = DateTime.ParseExact(SearchHistoryDateTime, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).AddMinutes(-SearchHistoryTimeRange).ToString("MM/dd/yyyy HH:mm:ss");
                string dtto = DateTime.ParseExact(SearchHistoryDateTime, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).AddMinutes(SearchHistoryTimeRange).ToString("MM/dd/yyyy HH:mm:ss");

                
                string xml = string.Empty;
                //if (objUtil.ErrCheck(dbhistory.GetVehicleAreaSearch(sn.UserID, sn.SecId, lat, lon, radius, dtfrom, dtto, sn.User.OrganizationId, 0, ref xml), false))
                //    if (objUtil.ErrCheck(dbhistory.GetVehicleAreaSearch(sn.UserID, sn.SecId, lat, lon, radius, dtfrom, dtto, sn.User.OrganizationId, 0, ref xml), true))
                //    {
                //        return;
                //    }
                
                string ConnnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

                DataSet dsResult = null;

                using (VLF.DAS.Logic.MapSearch ms = new VLF.DAS.Logic.MapSearch(ConnnectionString))
                {

                    dsResult = ms.GetVehicleAreaSearch(lat, lon, radius, dtfrom, dtto, sn.User.OrganizationId, 0);

                    if (VLF.CLS.Util.IsDataSetValid(dsResult))
                        xml = dsResult.GetXml();
                    else
                        xml = "";
                }


                if (!string.IsNullOrEmpty(xml))
                {
                    xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                }
                Response.Write(xml);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                Response.Write("<error>" + Ex.ToString() + "</error>");

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                Response.Write("<error>" + Ex.ToString() + "</error>");
            }
        }*/

        public static string GetLocalResourceValue(string key)
        {
            string path = HttpContext.Current.Server.MapPath("App_LocalResources/frmFleetInfoNew.aspx");
            return clsAsynGenerateReport.GetResourceObject(path, key);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = false, XmlSerializeString = false)]
        public static string UpdatePosition(string boxIDs)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";


            StringBuilder replyMesgs = new StringBuilder();
            try
            {
                Int64 sessionTimeOut = 0;
                clsUtility objUtil = new clsUtility(sn);

                StringBuilder successBox = new StringBuilder();
                StringBuilder failedBox = new StringBuilder();
                bool cmdSent = false;
                string replyStr = string.Empty;
                if (boxIDs == string.Empty) return "1";

                string[] boxIDArr = boxIDs.Split(',');
                LocationMgr.Location dbl = new LocationMgr.Location();
                foreach (string boxId in boxIDArr)
                {
                    if (!string.IsNullOrEmpty(boxId))
                    {
                        short ProtocolId = -1;
                        short CommModeId = -1;
                        string errMsg = "";
                        if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(boxId), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "", ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                            if (errMsg == "")
                            {
                                if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(boxId), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "", ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                                {
                                    if (errMsg != "")
                                    {
                                        sn.MessageText = errMsg;
                                        replyStr = errMsg;
                                    }
                                    else
                                    {
                                        sn.MessageText = GetLocalResourceValue("sn_MessageText_SendCommandFailedError") + ": ";
                                        replyStr = errMsg;
                                    }

                                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, " User:" + sn.UserID.ToString() + "Web method: UpdatePosition() Page:Vehicles.aspx"));
                                }
                            }
                            else
                            {
                                replyStr = errMsg;
                            }
                        if (string.IsNullOrEmpty(errMsg))
                        {
                            //replyStr = "Command sent to box successfully";
                            if (successBox.ToString().Length > 0)
                            {
                                successBox.Append("," + boxId);
                            }
                            else
                                successBox.Append(boxId);
                        }
                        else
                        {
                            if (failedBox.ToString().Length > 0)
                            {
                                failedBox.Append("," + boxId);
                            }
                            else
                                failedBox.Append(boxId);
                        }
                    }
                }
                if (successBox.ToString().Length > 0)
                {
                    replyMesgs.AppendLine("Vehicle " + successBox.ToString() + " has received updateposition command successfully.");
                }
                if (failedBox.ToString().Length > 0)
                {
                    replyMesgs.AppendLine("Vehicle " + failedBox.ToString() + " didn't received updateposition command. Error occured... Try again...");
                }

                //return "0";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: UpdatePosition() Page:Vehicles.aspx"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                replyMesgs.AppendLine("Error occured please contact BSM for this error...");
                //return "0";
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            js.MaxJsonLength = int.MaxValue;
            return js.Serialize(replyMesgs.ToString());
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];            
        }

        // Changes for TimeZone Feature start

        private string getUserTimezone_NewTZ()
        {
            return "";

            if (sn.User.NewFloatTimeZone >= 0 && sn.User.NewFloatTimeZone < 10)
                return "+0" + sn.User.NewFloatTimeZone + ":00";
            else if (sn.User.NewFloatTimeZone >= 10)
                return "+" + sn.User.NewFloatTimeZone + ":00";
            else if (sn.User.NewFloatTimeZone < 0 && sn.User.NewFloatTimeZone > -10)
                return "-0" + Math.Abs(sn.User.NewFloatTimeZone) + ":00";
            else
                return sn.User.NewFloatTimeZone + ":00";
        }

        // Changes for TimeZone Feature end

        private string getUserTimezone()
        {
            return "";

            if (sn.User.TimeZone >= 0 && sn.User.TimeZone < 10)
                return "+0" + sn.User.TimeZone + ":00";
            else if (sn.User.TimeZone >= 10)
                return "+" + sn.User.TimeZone + ":00";
            else if (sn.User.TimeZone < 0 && sn.User.TimeZone > -10)
                return "-0" + Math.Abs(sn.User.TimeZone) + ":00";
            else
                return sn.User.TimeZone + ":00";
        }

        private DataTable ProceedReeferData(DataTable dt)
        {
            DataColumn dc;

            if (!dt.Columns.Contains("ReeferState"))
            {
                dc = new DataColumn("ReeferState", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("Micro"))
            {
                dc = new DataColumn("Micro", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("ControllerType"))
            {
                dc = new DataColumn("ControllerType", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("PowerOnOff"))
            {
                dc = new DataColumn("PowerOnOff", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("ReeferPower"))
            {
                dc = new DataColumn("ReeferPower", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("ModeOfOp"))
            {
                dc = new DataColumn("ModeOfOp", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("SDoor"))
            {
                dc = new DataColumn("SDoor", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("AFAX"))
            {
                dc = new DataColumn("AFAX", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("RF_Setpt"))
            {
                dc = new DataColumn("RF_Setpt", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("RF_Ret"))
            {
                dc = new DataColumn("RF_Ret", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("RF_Dis"))
            {
                dc = new DataColumn("RF_Dis", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("RF_SensorProbe"))
            {
                dc = new DataColumn("RF_SensorProbe", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("RF_Amb"))
            {
                dc = new DataColumn("RF_Amb", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("RF_Setpt2"))
            {
                dc = new DataColumn("RF_Setpt2", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("RF_Ret2"))
            {
                dc = new DataColumn("RF_Ret2", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("RF_Dis2"))
            {
                dc = new DataColumn("RF_Dis2", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("RF_SensorProbe2"))
            {
                dc = new DataColumn("RF_SensorProbe2", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("RF_Setpt3"))
            {
                dc = new DataColumn("RF_Setpt3", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("RF_Ret3"))
            {
                dc = new DataColumn("RF_Ret3", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("RF_Dis3"))
            {
                dc = new DataColumn("RF_Dis3", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("RF_SensorProbe3"))
            {
                dc = new DataColumn("RF_SensorProbe3", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("FuelLevel"))
            {
                dc = new DataColumn("FuelLevel", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("FuelLevelGallon"))
            {
                dc = new DataColumn("FuelLevelGallon", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("RPM"))
            {
                dc = new DataColumn("RPM", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("BatteryVoltage"))
            {
                dc = new DataColumn("BatteryVoltage", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("EngineHours"))
            {
                dc = new DataColumn("EngineHours", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("TetherOnOff"))
            {
                dc = new DataColumn("TetherOnOff", Type.GetType("System.String"));
                dc.DefaultValue = "";
                dt.Columns.Add(dc);
            }

            foreach (DataRow row in dt.Rows)
            {
                string customProp = row["ExtraInfo"].ToString();
                string tether = GetTetherOnOff(customProp);
                string reeferOnOff = GetReeferOnOff(row);
                string reeferState = "_";

                string RD_ZONE1 = string.Empty;
                RD_ZONE1 = FindValueFromPair("RD_ZONE1", customProp, ";", "=");
                string RD_ZONE2 = string.Empty;
                RD_ZONE2 = FindValueFromPair("RD_ZONE2", customProp, ";", "=");
                string RD_ZONE3 = string.Empty;
                RD_ZONE3 = FindValueFromPair("RD_ZONE3", customProp, ";", "=");
                string RD_ZONE1_OM = FindValueFromPair("OM", RD_ZONE1, ",", ":");
                int intRD_ZONE1_OM = -1;
                if (RD_ZONE1_OM != "")
                    int.TryParse(RD_ZONE1_OM, out intRD_ZONE1_OM);

                int _rd_sta = 0;
                string v = string.Empty;

                string ps = string.Empty;
                string micro = string.Empty;
                string controllerType = string.Empty;
                int controltypeId = -1;
                //string powerOnoff = string.Empty;
                string reeferPower = string.Empty;
                string modeofop = string.Empty;
                string sdoor = string.Empty;
                string afax = string.Empty;
                string setpt = string.Empty;
                string ret = string.Empty;
                string dis = string.Empty;
                string sensorProbe = string.Empty;
                string amb = string.Empty;

                string setpt2 = string.Empty;
                string ret2 = string.Empty;
                string dis2 = string.Empty;
                string sensorProbe2 = string.Empty;

                string setpt3 = string.Empty;
                string ret3 = string.Empty;
                string dis3 = string.Empty;
                string sensorProbe3 = string.Empty;

                string fuelLevel = string.Empty;
                string fuelLevelGallon = string.Empty;
                string rpm = string.Empty;
                string batteryVoltage = string.Empty;
                string engineHours = string.Empty;               
                double temprature = 0;

                if (row["VehicleTypeName"].ToString().Trim().ToLower() == "dry car") //Dry Car
                {
                    micro = "DC";
                    controllerType = "-";
                }
                else
                {
                    ps = FindValueFromPair("RD_DT", customProp, ";", "=");
                    if (ps == "1")
                        micro = "TK";
                    else if (ps == "2")
                        micro = "CT";
                    else
                        micro = "Unknown";

                    int.TryParse(FindValueFromPair("RD_STA", customProp, ";", "="), out _rd_sta);
                    if ((_rd_sta & 15) == 1) // Device Type = 1 (TK)
                    {
                        if (FindValueFromPair("RD_CT", customProp, ";", "=") != "")
                            int.TryParse(FindValueFromPair("RD_CT", customProp, ";", "="), out controltypeId);
                        controllerType = getControllerTypeById(controltypeId);

                        # region RPM
                        if (",9,11,12,13,14,15,16,17,19,20,".IndexOf("," + controltypeId.ToString() + ",") >= 0)
                            rpm = FindValueFromPair("RD_RPM", customProp, ";", "=");
                        # endregion
                    }
                }

                if (tether == "Off" || reeferOnOff == "Off")
                {
                    reeferState = "-";
                    modeofop = "-";
                    afax = "-";
                    setpt = "-";
                    ret = "-";
                    dis = "-";
                    sensorProbe = "-";
                    amb = "-";
                    setpt2 = "-";
                    ret2 = "-";
                    dis2 = "-";
                    sensorProbe2 = "-";
                    setpt3 = "-";
                    ret3 = "-";
                    dis3 = "-";
                    sensorProbe3 = "-";
                    rpm = "-";                    
                }
                else
                {
                    if (RD_ZONE1 != "")
                    {
                        if (RD_ZONE1_OM != "")
                        {
                            # region ReeferState
                            reeferState = getReeferState(intRD_ZONE1_OM);
                            # endregion

                            # region ModeOfOp
                            var mode = (intRD_ZONE1_OM & 16) >> 4;
                            if (mode == 1)
                                modeofop = "Continuous";
                            else
                                modeofop = "Cycle Sentry";
                            # endregion

                            # region Door
                            if (((intRD_ZONE1_OM & 4) >> 2) == 1)
                                sdoor = "Open";
                            else
                                sdoor = "Closed";
                            # endregion

                            # region AFAX
                            if (",14,15,16,17,19,20,21,22,".IndexOf("," + controltypeId + ",") >= 0)
                            {

                                if ((intRD_ZONE1_OM & 1) == 1)
                                    afax = "Open";
                                else
                                    afax = "Closed";

                            }
                            # endregion
                        }

                        # region Setpt.
                        v = FindValueFromPair("TSP", RD_ZONE1, ",", ":");
                        if (v.Trim() != "")
                        {
                            if (sn.User.TemperatureType == "Celsius")
                            {
                                temprature = Convert.ToSingle(v);
                                v = Convert.ToString(((temprature - 32) * 5 / 9));
                            }
                            setpt = String.Format("{0:0}", double.Parse(v));
                        }
                       
                        # endregion

                        # region Ret
                        v = FindValueFromPair("RAT1", RD_ZONE1, ",", ":");
                        if (v.Trim() != "")
                        {
                            if (sn.User.TemperatureType == "Celsius")
                            {
                                temprature = Convert.ToSingle(v);
                                v = Convert.ToString(((temprature - 32) * 5 / 9));
                            }
                            ret = String.Format("{0:0.00}", double.Parse(v));
                        }
                        # endregion

                        # region Dis
                        v = FindValueFromPair("SDT1", RD_ZONE1, ",", ":");
                        if (v.Trim() != "")
                        {
                            if (sn.User.TemperatureType == "Celsius")
                            {
                                temprature = Convert.ToSingle(v);
                                v = Convert.ToString(((temprature - 32) * 5 / 9));
                            }
                            dis = String.Format("{0:0.00}", double.Parse(v));
                        }
                        # endregion

                        # region Sensor Probe
                        v = FindValueFromPair("ECT", RD_ZONE1, ",", ":");
                        if (v.Trim() != "")
                        {
                            double _v = double.Parse(v);
                            if (_v >= 3276.7 || _v <= -3276.8)
                            {
                                sensorProbe = "-";
                            }
                            else
                            {
                                //sensorProbe = String.Format("{0:0.00}", (_v * 10 / 32) * 9 / 5 + 32);
                                if (sn.User.TemperatureType == "Fahrenheit")
                                    sensorProbe = String.Format("{0:0.00}", (_v * 10 / 32) * 9 / 5 + 32);
                                else
                                    sensorProbe = String.Format("{0:0.00}", (_v * 10 / 32));
                            }                            
                        }
                        # endregion
                      
                    }

                    if (RD_ZONE2 != "")
                    {

                        # region Setpt2.
                        v = FindValueFromPair("TSP", RD_ZONE2, ",", ":");

                        if (v.Trim() != "")
                        {
                            if (sn.User.TemperatureType == "Celsius")
                            {
                                temprature = Convert.ToSingle(v);
                                v = Convert.ToString(((temprature - 32) * 5 / 9));
                            }
                            setpt2 = String.Format("{0:0}", double.Parse(v));
                        }
                        # endregion

                        # region Ret2
                        v = FindValueFromPair("RAT1", RD_ZONE2, ",", ":");
                        if (v.Trim() != "")
                        {
                            if (sn.User.TemperatureType == "Celsius")
                            {
                                temprature = Convert.ToSingle(v);
                                v = Convert.ToString(((temprature - 32) * 5 / 9));
                            }

                            ret2 = String.Format("{0:0.00}", double.Parse(v));
                        }
                        # endregion

                        # region Dis2
                        v = FindValueFromPair("SDT1", RD_ZONE2, ",", ":");
                        if (v.Trim() != "")
                        {
                            if (sn.User.TemperatureType == "Celsius")
                            {
                                temprature = Convert.ToSingle(v);
                                v = Convert.ToString(((temprature - 32) * 5 / 9));
                            }

                            dis2 = String.Format("{0:0.00}", double.Parse(v));
                        }
                        # endregion

                        # region Sensor Probe2
                        v = FindValueFromPair("ECT", RD_ZONE2, ",", ":");
                        if (v.Trim() != "")
                        {
                            double _v = double.Parse(v);
                            if (_v >= 3276.7 || _v <= -3276.8)
                            {
                                sensorProbe2 = "-";
                            }
                            else
                            {
                                //sensorProbe2 = String.Format("{0:0.00}", (_v * 10 / 32) * 9 / 5 + 32);
                                if (sn.User.TemperatureType == "Fahrenheit")
                                    sensorProbe2 = String.Format("{0:0.00}", (_v * 10 / 32) * 9 / 5 + 32);
                                else
                                    sensorProbe2 = String.Format("{0:0.00}", (_v * 10 / 32) );
                            }
                        }
                        # endregion
                    }

                    if (RD_ZONE3 != "")
                    {

                        # region Setpt3.
                        v = FindValueFromPair("TSP", RD_ZONE3, ",", ":");                       

                        if (v.Trim() != "")
                        {                            
                            if (sn.User.TemperatureType == "Celsius")
                            {
                                temprature = Convert.ToSingle(v);
                                v = Convert.ToString(((temprature - 32) * 5 / 9));                                
                            }
                            setpt3 = String.Format("{0:0}", double.Parse(v));
                        }


                        # endregion

                        # region Ret3
                        v = FindValueFromPair("RAT1", RD_ZONE3, ",", ":");
                        if (v.Trim() != "")
                        {
                            if (sn.User.TemperatureType == "Celsius")
                            {
                                temprature = Convert.ToSingle(v);
                                v = Convert.ToString(((temprature - 32) * 5 / 9));
                            }


                            ret3 = String.Format("{0:0.00}", double.Parse(v));
                        }
                        # endregion

                        # region Dis3
                        v = FindValueFromPair("SDT1", RD_ZONE3, ",", ":");
                        if (v.Trim() != "")
                        {
                            if (sn.User.TemperatureType == "Celsius")
                            {
                                temprature = Convert.ToSingle(v);
                                v = Convert.ToString(((temprature - 32) * 5 / 9));
                            }

                            dis3 = String.Format("{0:0.00}", double.Parse(v));
                        }
                        # endregion

                        # region Sensor Probe3
                        v = FindValueFromPair("ECT", RD_ZONE3, ",", ":");
                        if (v.Trim() != "")
                        {
                            double _v = double.Parse(v);
                            if (_v >= 3276.7 || _v <= -3276.8)
                            {
                                sensorProbe3 = "-";
                            }
                            else
                            {
                                //sensorProbe3 = String.Format("{0:0.00}", (_v * 10 / 32) * 9 / 5 + 32);
                                if (sn.User.TemperatureType == "Fahrenheit")
                                    sensorProbe3 = String.Format("{0:0.00}", (_v * 10 / 32) * 9 / 5 + 32);
                                else
                                    sensorProbe3 = String.Format("{0:0.00}", (_v * 10 / 32));
                            }
                        }
                        # endregion
                    }

                    # region Amb
                    string _amt = FindValueFromPair("RD_AMT", customProp, ";", "=");
                    if (_amt.Trim() != "")
                    {
                        if (sn.User.TemperatureType == "Celsius")
                        {
                            temprature = Convert.ToSingle(_amt);
                            _amt = Convert.ToString(((temprature - 32) * 5 / 9));
                        }

                        amb = String.Format("{0:0}", double.Parse(_amt));
                    }
                    # endregion
                }

                double dv = 0;

                # region Fuel Level
                v = FindValueFromPair("RD_FUEL", customProp, ";", "=");
                if (v != "")
                {
                    dv = 300;
                    double.TryParse(v, out dv);
                    
                    if (dv <= 100)
                        fuelLevel = String.Format("{0:0}", dv);
                    else
                        fuelLevel = "Invalid";
                }
                # endregion

                # region Fuel Level Gallon
                v = FindValueFromPair("RD_FUEL", customProp, ";", "=");
                if (v != "")
                {
                    dv = 300;
                    double.TryParse(v, out dv);

                    if (dv <= 100)
                        fuelLevelGallon = String.Format("{0:0}", dv * 450 / 100); //based on 450 gallon tank size
                    else
                        fuelLevelGallon = "Invalid";
                }
                # endregion

                # region BatteryVoltage
                v = FindValueFromPair("RD_BATTERY", customProp, ";", "=");
                batteryVoltage = "";
                if (v != "")
                {
                    dv = 0;
                    double.TryParse(v, out dv);
                    
                    batteryVoltage = String.Format("{0:0.00}", dv);                    
                }
                # endregion

                # region EngineHours
                v = FindValueFromPair("RD_EH", customProp, ";", "=");
                engineHours = "";
                if (v != "")
                {
                    dv = 0;
                    double.TryParse(v, out dv);

                    engineHours = String.Format("{0:0}", dv);
                }
                # endregion


                if (micro == "DC") //Dry Car
                {
                   sdoor = reeferOnOff == "On" ? "Open" : "Closed";

                    # region AMB for Dry Car
                    string analog1 = FindValueFromPair("Analog1", customProp, ";", "=");
                    string analog2 = FindValueFromPair("Analog2", customProp, ";", "=");
                    int intAnalog1 = 0;
                    int intAnalog2 = 0;
                    int.TryParse(analog1, out intAnalog1);
                    int.TryParse(analog2, out intAnalog2);

                    if (analog1 != "" && analog2 != "" && !(intAnalog1 == 0 && intAnalog2 == 0))
                    {
                        double ADC = (intAnalog2 * 256) + (intAnalog1 * 1.0);
                        double temp = Math.Floor((ADC - 1120.0) / 5.0);
                        //temp = temp * 9.0 / 5.0 + 32;
                        if (sn.User.TemperatureType == "Fahrenheit")
                        {
                            temp = temp * 9.0 / 5.0 + 32;
                        }                       
                        amb = String.Format("{0:0}", temp);
                   }
                   else
                       amb = "-";
                    # endregion
                }
                
                reeferPower = "-";
                if (tether == "Off" && micro != "DC")
                {
                    //powerOnoff = "-";
                    reeferPower = "-";
                    sdoor = "-";
                }
                else
                {
                    if ((_rd_sta & 15) == 1) // Device Type = 1 (TK)
                    {
                        int[] cids = {9,11,12,13,14,15,16,17,19,20};
                        if (System.Array.IndexOf(cids, controltypeId) >= 0)
                        {

                            if ((_rd_sta & 64) > 0)
                               reeferPower = "On";
                           else
                               reeferPower = "Off";
                       }
                    }
                }

                row["Micro"] = micro;
                row["ReeferState"] = reeferState;
                row["ControllerType"] = controllerType;
                row["ReeferPower"] = reeferPower;
                row["PowerOnOff"] = reeferOnOff;
                row["ModeOfOp"] = modeofop;
                row["SDoor"] = sdoor;
                row["AFAX"] = afax;
                row["RF_Setpt"] = setpt;
                row["RF_Ret"] = ret;
                row["RF_Dis"] = dis;
                row["RF_SensorProbe"] = sensorProbe;
                row["RF_Amb"] = amb;
                row["RF_Setpt2"] = setpt2;
                row["RF_Ret2"] = ret2;
                row["RF_Dis2"] = dis2;
                row["RF_SensorProbe2"] = sensorProbe2;
                row["RF_Setpt3"] = setpt3;
                row["RF_Ret3"] = ret3;
                row["RF_Dis3"] = dis3;
                row["RF_SensorProbe3"] = sensorProbe3;
                row["FuelLevel"] = fuelLevel;
                row["FuelLevelGallon"] = fuelLevelGallon;
                row["RPM"] = rpm;
                row["BatteryVoltage"] = batteryVoltage;
                row["EngineHours"] = engineHours;
                row["TetherOnOff"] = tether;

            }

            return dt;
        }

        private string GetTetherOnOff(string customProp) {
            string ps = FindValueFromPair("Power", customProp, ";", "=");
            if (ps.ToLower() == "sleepmode") {
                return "Off";
            }
            int RD_STA = 0;
            ps = FindValueFromPair("RD_STA", customProp, ";", "=");
            int.TryParse(ps, out RD_STA);
            string returnvalue = "Off";
            if (ps != "") {
                if (((RD_STA & 16) >> 4) == 1)
                    returnvalue = "On";
            }
            return returnvalue;
        }

        private string GetReeferOnOff(DataRow row) {
            int psensormask = 0;
            int.TryParse(row["SensorMask"].ToString(), out psensormask);
            
            if (row["VehicleStatus"].ToString() == "Ignition Off")
                return "Off";
            if ((psensormask & 4) >> 2 == 1)
                return "On";
            else
                return "Off";
        }

        private string getReeferState(int intRD_ZONE1_OM)
        {
            int r = intRD_ZONE1_OM >> 5;
            string reeferState = string.Empty;

            switch (r)
            {
                case 0:
                    reeferState = "Unknown";
                    break;
                case 1:
                    reeferState = "Cooling";
                    break;
                case 2:
                    reeferState = "Heating";
                    break;

                case 3:
                    reeferState = "Defrost";
                    break;
                case 4:
                    reeferState = "Null";
                    break;
                case 5:
                    reeferState = "Pre-trip";
                    break;
                case 6:
                    reeferState = "Sleep";
                    break;
                case 7:
                    reeferState = "N/A";
                    break;
            }

            if (reeferState == "Cooling" || reeferState == "Heating")
            {
                int engineSpeed = intRD_ZONE1_OM & 8; //bit 4, Engine Speed. Only if cooling or heating. 0: low, 1: high
                if (engineSpeed == 0)
                    reeferState = "Low Speed " + reeferState;
                else
                    reeferState = "High Speed " + reeferState;
            }

            return reeferState;
        }

        private string getControllerTypeById(int controllerTypeId)
        {
            string c = "N/A";

            switch (controllerTypeId)
            {
                case 0:
                    c = "Invalid";
                    break;
                case 1:
                    c = "MP4";
                    break;
                case 2:
                    c = "MP5";
                    break;
                case 3:
                    c = "MP6";
                    break;
                case 4:
                    c = "TG6";
                    break;
                case 5:
                    c = "TTMT";
                    break;
                case 6:
                    c = "DAS";
                    break;
                case 7:
                    c = "TCI";
                    break;
                case 8:
                    c = "MPT";
                    break;
                case 9:
                    c = "SR2";
                    break;
                case 10:
                    c = "N/A";
                    break;
                case 11:
                    c = "SR2 M/T";
                    break;
                case 12:
                    c = "SR2 Truck";
                    break;
                case 13:
                    c = "SR2 Truck M/T";
                    break;
                case 14:
                    c = "SR3";
                    break;
                case 15:
                    c = "SR3 MT";
                    break;
                case 16:
                    c = "SR3 ST Truck";
                    break;
                case 17:
                    c = "SR3 MT Truck";
                    break;
                case 18:
                    c = "DAS IV";
                    break;
                case 19:
                    c = "SR4 ST";
                    break;
                case 20:
                    c = "SR4 MT";
                    break;
                case 21:
                    c = "SR4 ST Truck";
                    break;
                case 22:
                    c = "SR4 MT Truck";
                    break;
                case 23:
                    c = "Cryo Trailer";
                    break;
                case 24:
                    c = "Cryo Truck";
                    break;
                default:
                    c = "N/A";
                    break;

            }
            return c;
        }

        private string FindValueFromPair(string key, string src, string seperator, string equalSign)
        {
            try
            {
                int val_start, val_end;
                int key_pos = src.IndexOf(key);
                
                if (key_pos != -1)
                {
                    val_start = src.IndexOf(equalSign, key_pos);
                    if (key_pos != -1)
                    {
                        val_end = src.IndexOf(seperator, val_start);

                        if (val_end != -1)
                            return src.Substring(val_start + 1, val_end - val_start - 1);
                        else
                            return src.Substring(val_start + 1, src.Length - val_start - 1);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " FindValueFromPair Page:Vehicles_Reefer.aspx"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            }
            return "";
        }
    }

    public static class BSMDistanceAlgorithm
    {
        const double PIx = 3.141592653589793;
        const double RADIUS = 6378.16;

        /// <summary>
        /// This class cannot be instantiated.
        /// </summary>
        //private BSMDistanceAlgorithm() { }

        /// <summary>
        /// Convert degrees to Radians
        /// </summary>
        /// <param name="x">Degrees</param>
        /// <returns>The equivalent in radians</returns>
        public static double Radians(double x)
        {
            return x * PIx / 180;
        }

        /// <summary>
        /// Calculate the distance between two places.
        /// </summary>
        /// <param name="lon1"></param>
        /// <param name="lat1"></param>
        /// <param name="lon2"></param>
        /// <param name="lat2"></param>
        /// <returns></returns>
        public static double DistanceBetweenPlaces(
            double lon1,
            double lat1,
            double lon2,
            double lat2)
        {
            double dlon = Radians(lon2 - lon1);
            double dlat = Radians(lat2 - lat1);

            double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + Math.Cos(Radians(lat1)) * Math.Cos(Radians(lat2)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
            double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return angle * RADIUS;
        }
    }
}