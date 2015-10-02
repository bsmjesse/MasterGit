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

namespace SentinelFM
{
    public partial class Vehicles : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;
        
        public string _xml = "";
        protected clsUtility objUtil;

        protected void Page_Load(object sender, EventArgs e)
        {
             try
            {
                sn = (SentinelFMSession)Session["SentinelFMSession"];                 
                if (!Page.IsPostBack)
                {
                    string request=Request.QueryString["QueryType"];
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
                            VehicleList_Fill();
                        }
                        else if (request.Equals("GetAllFleets", StringComparison.CurrentCultureIgnoreCase))
                        {
                            //sn.Map.LastKnownXML = string.Empty;
                            Fleets_Fill();
                        }  
                        else if(request.Equals("GetfleetPosition", StringComparison.CurrentCultureIgnoreCase))
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
                           FleetVehicles_Fill();
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

            if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, " No Fleets for User:" + sn.UserID.ToString() + " Form:clsUser "));
                    return;
                }


            if (xml == "")
                return;

            Response.ContentType = "text/xml";
            //byte[] data = Encoding.Default.GetBytes(xml.Trim());
            //xml = Encoding.UTF8.GetString(data);
            Response.Write(xml.Trim());
        }

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
                        xml = xml.Trim().Replace("<br>", "");
                        sn.Map.LastKnownXML = xml.Trim();
                    }
               
                if (string.IsNullOrEmpty(xml))
                //(xml == "" || xml == null)
                {
                    //sn.Map.DsFleetInfoNew = null;
                    if (sn.Map.SelectedFleetID == sn.User.DefaultFleet)
                    {
                        xml = sn.Map.DsFleetInfoNew.GetXml();
                        xml = xml.Trim().Replace("<br>", "");
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
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                                    }
                                    catch (Exception Ex)
                                    {
                                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                                    }
                                }
                            }
                        }
                        xml = dsFleetInfo.GetXml();
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
                    xml = xml.Replace("&#x0", "");
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
                if (string.IsNullOrEmpty(sn.Map.LastKnownXML) || sn.Map.LastStatusChecked==null)
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
                        xml = xml.Trim().Replace("<br>", "");
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
                        xml = xml.Trim().Replace("<br>", "").Trim();
                    }
                }

                if (string.IsNullOrEmpty(xml))
                //(xml == "" || xml == null)
                {
                    //sn.Map.DsFleetInfoNew = null;
                    if (sn.Map.SelectedFleetID == sn.User.DefaultFleet)
                    {
                        xml = sn.Map.DsFleetInfoNew.GetXml();
                        xml = xml.Trim().Replace("<br>", "").Trim();
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
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                                    }
                                    catch (Exception Ex)
                                    {
                                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                                    }
                                }
                            }
                        }
                        xml = dsFleetInfo.GetXml();
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
    }
}