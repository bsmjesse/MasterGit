<%@ WebService Language="C#" Class="vehicleListTree" %>

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
using VLF.PATCH.Logic;
using VLF.ERRSecurity;
using VLF.ERR;
using VLF.CLS.Def;
using System.Globalization;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class vehicleListTree : System.Web.Services.WebService
{
    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
    VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;
    VLF.PATCH.Logic.PatchFleet _pf = null;
    
    [WebMethod]
    public string HelloWorld() {
        return "Hello World";
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string FetchVehicleList(string nodecode, bool MutipleUserHierarchyAssignment, bool LoadVehicleData)
    {
        return _FetchVehicleList(nodecode, MutipleUserHierarchyAssignment, "", LoadVehicleData);
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string FetchVehicleListByPreferNodeCodes(string nodecode, bool MutipleUserHierarchyAssignment, string PreferNodeCodes, bool LoadVehicleData)
    {
        return _FetchVehicleList(nodecode, MutipleUserHierarchyAssignment, PreferNodeCodes, LoadVehicleData);
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string FetchVehicleListByPage(int fleetid, int page)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null)
            sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else
            return "";

        int pageSize = 100;

        if (ConfigurationManager.AppSettings["VehicleListTreePageSize"] != null)
            int.TryParse(ConfigurationManager.AppSettings["VehicleListTreePageSize"].ToString(), out pageSize);


        _pf = new VLF.PATCH.Logic.PatchFleet(sConnectionString);
        string v = getVehicleListByFleetId(fleetid, pageSize, page);

        _pf.Dispose();

        _results _r = new _results();
        _r.folderlist = "";
        _r.vehiclelist = v;
        _r.fleetId = fleetid;
        _r.totalVehicles = 0;
        _r.vehiclePageSize = pageSize;
        _r.vehicleCurrentPage = page;

        var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
        return serializer.Serialize(_r);
        
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void FetchVehicleListFilterByPage(int fleetid, bool manager, int page, string[] filter0)
    {
        // filter[0]: vehicle description
        // filter[1]: manager name
        
        HttpContext.Current.Response.AddHeader("Content-type", "application/json");

        string filterstring = "";
        if (filter0.Length > 0)
            filterstring = string.Join(";;;", filter0);
            //filterstring = filter0[0];
        
        
        page++;
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null)
            sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else
        {
            //return "";
            HttpContext.Current.Response.Write("");
            return;
        }

        int pageSize = 10;

        if (ConfigurationManager.AppSettings["VehicleListTreePageSize"] != null)
            int.TryParse(ConfigurationManager.AppSettings["VehicleListTreePageSize"].ToString(), out pageSize);


        _pf = new VLF.PATCH.Logic.PatchFleet(sConnectionString);
        string v = getVehicleListByFilterByFleetId(fleetid, manager, pageSize, page, filterstring);

        _results _r = new _results();
        _r.folderlist = "";
        _r.vehiclelist = v;
        _r.fleetId = fleetid;
        _r.totalVehicles = _pf.GetVehiclesInfoTotalNumberByFilterByFleetId(fleetid, filterstring);
        _r.vehiclePageSize = pageSize;
        _r.vehicleCurrentPage = page;

        _pf.Dispose();

        var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
        //return serializer.Serialize(_r);
        HttpContext.Current.Response.Write(serializer.Serialize(_r));
        HttpContext.Current.Response.End();

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void Hello(string[] filter)
    {
        HttpContext.Current.Response.AddHeader("Content-type", "application/json");

        string filter0 = "";
        if (filter.Length > 0)
            filter0 = filter[0];

        

        _results _r = new _results();
        _r.folderlist = "";
        _r.vehiclelist = filter0;
        

        var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
        //return serializer.Serialize(_r);
        HttpContext.Current.Response.Write(serializer.Serialize(_r));
        HttpContext.Current.Response.End();

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string OrganizationHierarchyGetHierarchyByVehicleId(int vehicleId)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null)
            sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else
        {
            return "";
        }

        poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
        
        _results _r = new _results();
        _r.folderlist = "";
        _r.vehiclelist = "";
        _r.fleetId = 0;
        _r.totalVehicles = 0;
        _r.vehiclePageSize = 0;
        _r.vehicleCurrentPage = 0;
        _r.fleetPath = poh.OrganizationHierarchyGetHierarchyByVehicleId(sn.User.OrganizationId, vehicleId);
        
        var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };

        poh.Dispose();
        
        return serializer.Serialize(_r);
    }

    private string _FetchVehicleList(string nodecode, bool MutipleUserHierarchyAssignment, string PreferNodeCodes, bool LoadVehicleData)
    {
        int pageSize = 100;

        if (ConfigurationManager.AppSettings["VehicleListTreePageSize"] != null)
            int.TryParse(ConfigurationManager.AppSettings["VehicleListTreePageSize"].ToString(), out pageSize);
            
        
        //string emptyresult = "<ul class=\"jqueryFileTree\" style=\"display: none;\"></ul>";
        string emptyresult = "";
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null)
            sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else
            return emptyresult;

        string nodecodelist = string.Empty;
        string vehiclelist = string.Empty;
        int totalVehicles = 0;

        poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
        
        _pf = new VLF.PATCH.Logic.PatchFleet(sConnectionString);
        if (nodecode == null || nodecode.Length <= 0 || nodecode == "/")
            nodecode = "";

        bool ByUserPermission = true;       //always by user permission
        //if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 480 || sn.User.OrganizationId == 999988 || sn.User.OrganizationId == 1000026)
        //if (clsPermission.FeaturePermissionCheck(sn, "LoadOrganizationHierarchyByUserHierarchyAssignment"))
        //{
        //    ByUserPermission = true;
        //}
        bool ByMultipleHierarchyPermission = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
        
        DataSet dsOH = new DataSet();
        /*if (nodecode == "" || !ByUserPermission)
        {
            dsOH = poh.GetChildrenByNodeCode(sn.User.OrganizationId, nodecode);
        }
        else
        {
            dsOH = poh.GetChildrenByNodeCodeUserId(sn.User.OrganizationId, nodecode, sn.UserID);
        }*/

        if (nodecode == "" && PreferNodeCodes == "" && ByMultipleHierarchyPermission)
        {
            dsOH = poh.GetOrganizationHierarchyRootByUserID(sn.User.OrganizationId, sn.UserID, true);
        }
        else if (PreferNodeCodes != "")
        {
            dsOH = poh.GetOrganizationHierarchyByPreferNodeCode(sn.User.OrganizationId, PreferNodeCodes);
        }
        else if (nodecode == "" && ByUserPermission)
        {
            if (sn.RootOrganizationHierarchy == null)
            {
                dsOH = poh.GetOrganizationHierarchyRootByUserID(sn.User.OrganizationId, sn.UserID);
                sn.RootOrganizationHierarchy = dsOH;
            }
            else
                dsOH = sn.RootOrganizationHierarchy;
        }
        else
            dsOH = poh.GetChildrenByNodeCode(sn.User.OrganizationId, nodecode);


        if (dsOH.Tables[0].Rows.Count == 0)
            nodecodelist = emptyresult;
        else
        {

            nodecodelist = "<ul class=\"jqueryFileTree\" style=\"display: none;\">";
            string _class = MutipleUserHierarchyAssignment ? " class='inlineblock' " : "";
            foreach (DataRow row in dsOH.Tables[0].Rows)
            {
                string s = row["FleetName"].ToString() == "" ? row["NodeName"].ToString() : row["FleetName"].ToString();
                string fleetPath = row["PathFleetName"].ToString();

                
                string liitem = "<a href=\"#\"" + _class + " rel=\"" + row["NodeCode"].ToString() + "\" fleetId=\"" + row["FleetId"].ToString() + "\" fleetPath=\"" + fleetPath + "\">" + s + "</a>";

                if (MutipleUserHierarchyAssignment)
                {
                    liitem = "<input type='checkbox' class='chkfleet' id='chk" + row["NodeCode"].ToString() + "' data-nodecode='" + row["NodeCode"].ToString() + "' />" + liitem;
                }
                
                nodecodelist += "\t<li class=\"directory collapsed\">" + liitem + "</li>";
            }
            nodecodelist += "</ul>";
        }


        int fleetid = (nodecode != "" && !nodecode.Contains(',')) ? poh.GetFleetIdByNodeCode(sn.User.OrganizationId, nodecode) : -1;


        //if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 480)
        //{
        if (LoadVehicleData && false)
        {
            vehiclelist = getVehicleListByFleetId(fleetid, pageSize, 1);
            totalVehicles = _pf.GetVehiclesInfoTotalNumberByFleetId(fleetid);
        }
        //}
        /*else
        {
            DataSet dsOHA = poh.GetVehicleListByNodeCode(sn.User.OrganizationId, nodecode);

            foreach (DataRow row in dsOHA.Tables[0].Rows)
            {
                vehiclelist += "\t<tr rel=\"" + row["BoxId"].ToString() + "\" class=\"vehicleitem\"><td>" + row["Description"].ToString() + "</td>";
                // vehiclelist += "<td>" + row["Description"].ToString() + "</td>";            
                vehiclelist += "</tr>\n";
            }                     
            
        }*/

        _results _r = new _results();
        _r.folderlist = nodecodelist;
        _r.vehiclelist = vehiclelist;
        _r.fleetId = fleetid;
        _r.totalVehicles = totalVehicles;
        _r.vehiclePageSize = pageSize;
        _r.vehicleCurrentPage = 1;

        poh.Dispose();
        _pf.Dispose();

        var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
        return serializer.Serialize(_r);
    }

    private string getVehicleListByFleetId(int fleetId, int pageSize, int page)
    {
        try
        {
            DataSet dsVehicle;
            dsVehicle = new DataSet();
           /* StringReader strrXML = null;

            string xml = "";

            SentinelFM.ServerDBFleet.DBFleet dbf = new SentinelFM.ServerDBFleet.DBFleet();

            SentinelFMSession sn = null;
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            clsUtility objUtil;
            objUtil = new clsUtility(sn);

            if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                {
                    return string.Empty;
                }
            //if (objUtil.ErrCheck(dbf.GetVehiclesByFleetUser(sn.UserID, sn.SecId, fleetId, ref xml), false))
            //    if (objUtil.ErrCheck(dbf.GetVehiclesByFleetUser(sn.UserID, sn.SecId, fleetId, ref xml), true))
            //    {
            //        return string.Empty;
            //    }
            if (xml == "")
            {                
                return string.Empty;
            }

            strrXML = new StringReader(xml);
            dsVehicle.ReadXml(strrXML);*/
            dsVehicle = _pf.GetVehiclesInfoByFleetIdByPage(fleetId, pageSize, page);

            string s = string.Empty;

            foreach (DataRow row in dsVehicle.Tables[0].Rows)
            {
                s += "\t<tr rel=\"" + row["BoxId"].ToString() + "\" class=\"vehicleitem\"><td>" + row["Description"].ToString() + "</td>";
                s += "</tr>\n";
            }
            return s;
            
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            return string.Empty;
        }
        catch (Exception Ex)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            return string.Empty;
        }
    }

    private string getVehicleListByFilterByFleetId(int fleetId, bool manager, int pageSize, int page, string filter)
    {
        try
        {
            DataSet dsVehicle;
            dsVehicle = new DataSet();

            dsVehicle = _pf.GetVehiclesInfoByFleetIdByFilterByPage(fleetId, pageSize, page, filter);

            string s = string.Empty;

            foreach (DataRow row in dsVehicle.Tables[0].Rows)
            {
                s += "\t<tr data-vehilceid=\"" + row["VehicleId"] + "\" data-licenseplate=\"" + row["LicensePlate"] + "\" rel=\"" + row["BoxId"].ToString() + "\" class=\"vehicleitem\"><td>" + row["Description"].ToString() + "</td>";
                if(manager) s += "<td>" + row["ManagerName"].ToString() + "</td>";
                s += "</tr>\n";
            }
            return s;

        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            return string.Empty;
        }
        catch (Exception Ex)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            return string.Empty;
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string SearchOrganizationHierarchy(string searchString)
    {
        string emptyresult = "<li>No Result Found.</li>";
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null)
            sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else
            return emptyresult;

        string resultlist = string.Empty;
        
        poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
        if (searchString == null || searchString.Length <= 0)
            searchString = "";

        if (searchString != "")
        {
            bool ByMultipleHierarchyPermission = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
            DataSet dsOH = poh.SearchOrganizationHierarchy(sn.User.OrganizationId, searchString, ByMultipleHierarchyPermission ? sn.UserID : -1);
            string scriptpath = HttpContext.Current.Request.Url.AbsolutePath.Replace("SearchOrganizationHierarchy", "ValidateNodeCodeInUserPreference");    
            
            if (dsOH.Tables[0].Rows.Count > 0)                
            {

                foreach (DataRow row in dsOH.Tables[0].Rows)
                {
                    string nodepath = row["NodePath"].ToString();
                    nodepath = nodepath.Replace(searchString.ToUpper(), "<span class='matchsearch'>" + searchString.ToUpper() + "</span>");
                    resultlist += "\t<li rel=\"" + row["NodePath"].ToString() + "\" data-nodecode=\"" + row["NodeCode"].ToString() + "\"><a href='javascript:void(0)' onclick='gotooh(this," + ByMultipleHierarchyPermission.ToString().ToLower() + ",\"" + scriptpath + "\")'>" + row["NodeName"].ToString() + "</a><br>" + nodepath + "</li>";
                }                
            }
            
        }
        
        if (resultlist.Trim() == string.Empty) resultlist = emptyresult;

        _optResult _r = new _optResult();
        _r.resultList = resultlist;

        poh.Dispose();

        return new JavaScriptSerializer().Serialize(_r);
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string ValidateNodeCodeInUserPreference(string nodecode)
    {
        string emptyresult = "0";
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null)
            sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else
            return emptyresult;

        System.Threading.Thread.CurrentThread.CurrentUICulture = new
                    CultureInfo(sn.SelectedLanguage);

        poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
        
        _optResult _r = new _optResult();
        
        if(poh.ValidateNodeCodeInUserPreference(sn.User.OrganizationId, sn.UserID, nodecode))
        {
            _r.status = 1;
            _r.resultList = "";
        }
        else
        {
            _r.status = 0;
            _r.resultList = Resources.Const.ConfirmAddHierarchyToUserPreference;
        }

        poh.Dispose();

        return new JavaScriptSerializer().Serialize(_r);
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string AddNodeCodeInUserPreference(string nodecode)
    {
        _optResult _r = new _optResult();

        try
        {
            string emptyresult = "0";
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null)
                sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else
                return emptyresult;

            System.Threading.Thread.CurrentThread.CurrentUICulture = new
                        CultureInfo(sn.SelectedLanguage);

            poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

            SentinelFM.ServerDBUser.DBUser dbu;
            dbu = new SentinelFM.ServerDBUser.DBUser();

            string _defaultOrganizationHierarchyNodeCode = string.Empty;
            if (sn.User.PreferNodeCodes == null || sn.User.PreferNodeCodes == "")
                _defaultOrganizationHierarchyNodeCode = nodecode;
            else
            {
                string[] ss = sn.User.PreferNodeCodes.Split(',');
                bool addnodecode = true;

                foreach (string s in ss)
                {
                    bool adds = true;
                    if (poh.OrganizationHierarchyParentChildNodeCode(sn.User.OrganizationId, s, nodecode))
                    {
                        addnodecode = false;
                    }
                    if (poh.OrganizationHierarchyParentChildNodeCode(sn.User.OrganizationId, nodecode, s))
                    {
                        adds = false;
                    }
                    if (adds)
                        _defaultOrganizationHierarchyNodeCode += "," + s;
                }
                if (addnodecode)
                    _defaultOrganizationHierarchyNodeCode += "," + nodecode;

                _defaultOrganizationHierarchyNodeCode = _defaultOrganizationHierarchyNodeCode.Trim(',');
            }

            clsUtility objUtil;
            objUtil = new clsUtility(sn);

            if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, sn.UserID, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode), _defaultOrganizationHierarchyNodeCode), false))
                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, sn.UserID, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode), _defaultOrganizationHierarchyNodeCode), true))
                {
                    _r.status = 0;
                    _r.resultList = Resources.Const.FailedAddHierarchyToUserPreference;
                    return new JavaScriptSerializer().Serialize(_r);
                }

            sn.User.PreferNodeCodes = _defaultOrganizationHierarchyNodeCode;

            string[] ns = sn.User.PreferNodeCodes.Split(',');
            string multipleFleetIds = string.Empty;
            foreach (string s in ns)
            {
                int fid = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, s);
                multipleFleetIds = multipleFleetIds + "," + fid.ToString();
            }
            multipleFleetIds = multipleFleetIds.Trim(',');
            sn.User.PreferFleetIds = multipleFleetIds;

            _r.status = 1;
            _r.resultList = Resources.Const.SuccessAddHierarchyToUserPreference;
        }
        catch
        {
            _r.status = 0;
            _r.resultList = Resources.Const.FailedAddHierarchyToUserPreference;
        }
        
        poh.Dispose();

        return new JavaScriptSerializer().Serialize(_r);
        
        
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string AddOrganizationHierarchyNode(string nodeCode, string nodeName, string parentNodeCode)
    {
        string emptyresult = "";
        _optResult _r = new _optResult();
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null)
            sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else
        {
            _r.status = 500;
            _r.resultList = "Session time out.";
            return emptyresult;
        }

        string resultlist = string.Empty;

        poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
        if (nodeCode == null || nodeCode.Length <= 0)
            nodeCode = "";

        if (parentNodeCode == null || parentNodeCode.Length <= 0)
            parentNodeCode = "";

        nodeName = nodeName ?? "";

        _r.status = 500;

        if (nodeCode != "" && parentNodeCode != "")
        {

            string s = poh.AddOrganizationHierarchyNode(sn.User.OrganizationId, nodeCode, nodeName, parentNodeCode, sn.UserID);

            if (s == "1")
            {
                _r.status = 500;
                _r.resultList = "Node code already exists";
            }
            else if (s == "2")
            {
                _r.status = 500;
                _r.resultList = "Node name already exists";
            }
            else
            {
                _r.status = 200;
                _r.resultList = s;
            }

        }

        poh.Dispose();

        return new JavaScriptSerializer().Serialize(_r);
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string DeleteOrganizationHierarchyNode(string nodeCode)
    {
        string emptyresult = "";
        _optResult _r = new _optResult();
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null)
            sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else
        {
            _r.status = 500;
            _r.resultList = "Session time out.";
            return emptyresult;
        }

        string resultlist = string.Empty;

        poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
        if (nodeCode == null || nodeCode.Length <= 0)
            nodeCode = "";

        _r.status = 500;

        if (nodeCode != "")
        {
            try
            {
                string s = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, nodeCode);
                if (s.Split('/').Length > 0)
                    s = s.Replace("/" + nodeCode, "");
                else
                    s = string.Empty;
                poh.DeleteOrganizationHierarchyNode(sn.User.OrganizationId, nodeCode);
                 _r.status = 200;                
                _r.resultList = s;
            }
            catch{}
        }

        poh.Dispose();

        return new JavaScriptSerializer().Serialize(_r);
    }

    

    private class _results
    {
        public string folderlist;
        public string vehiclelist;
        public int fleetId;
        public string fleetPath;
        public int totalVehicles = 0;
        public int vehiclePageSize = 100;
        public int vehicleCurrentPage = 1;
    }

    private class _optResult
    {
        public int status;
        public string resultList;
    }
    
    //Devin Added on 2014-08-14 For Kiewit Question set assign to DOT
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetVehicleListByFilterByFleetIdForDOT(string fleetid, bool manager, int page, string[] filter0)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";
        try
        {
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            VLF.PATCH.Logic.PatchFleet _pf = null;
            HttpContext.Current.Response.AddHeader("Content-type", "application/json");
            string filterstring = "";
            if (filter0.Length > 0)
                filterstring = string.Join(";;;", filter0);
            //filterstring = filter0[0];
            page++;
            int curfleetId = 0;
            int.TryParse(fleetid, out curfleetId);
            if (curfleetId == 0)
                fleetid = Server.UrlDecode(fleetid);
            int pageSize = 10;
            if (ConfigurationManager.AppSettings["VehicleListTreePageSize"] != null)
                int.TryParse(ConfigurationManager.AppSettings["VehicleListTreePageSize"].ToString(), out pageSize);
            clsHOSManager hosManager = new clsHOSManager();
            string v = getVehicleListByFilterByFleetIdForDOT(sn.User.OrganizationId, fleetid, manager, pageSize, page, filterstring, curfleetId, sConnectionString);
            _results _r = new _results();
            _r.folderlist = "";
            _r.vehiclelist = v;
            if (curfleetId > 0)
            {
                _r.fleetId = curfleetId;
                _pf = new VLF.PATCH.Logic.PatchFleet(sConnectionString);
                _r.totalVehicles = _pf.GetVehiclesInfoTotalNumberByFilterByFleetId(curfleetId, filterstring);
                _pf.Dispose();
            }
            else
            {
                //fleetid store dot number start with @@DOT_
                _r.totalVehicles = hosManager.GetVehiclesInfoTotalNumberByDOT(sn.User.OrganizationId, fleetid.Substring(6), filterstring);
            }
            _r.vehiclePageSize = pageSize;
            _r.vehicleCurrentPage = page;
           var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            //return serializer.Serialize(_r);
            HttpContext.Current.Response.Write(serializer.Serialize(_r));
            HttpContext.Current.Response.End();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddOrUpdateInspectionGroup() Page:frmDynamicInspections"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return "0";
        }
        return "";
    }
    private static string getVehicleListByFilterByFleetIdForDOT(int organizationId, string fleet, bool manager, int pageSize, int page, string filter, int curfleetId, string sConnectionString)
    {
        try
        {
            VLF.PATCH.Logic.PatchFleet _pf = null;
            DataSet dsVehicle = null;
            if (curfleetId > 0)
            {
                _pf = new VLF.PATCH.Logic.PatchFleet(sConnectionString);
                dsVehicle = _pf.GetVehiclesInfoByFleetIdByFilterByPage(curfleetId, pageSize, page, filter);
                _pf.Dispose();
            }
            else
            {
                clsHOSManager hosManager = new clsHOSManager();
                fleet = fleet.Substring(6); ////fleetid store dot number start with @@DOT_
                dsVehicle = hosManager.GetVehiclesInfoByDOTByPage(organizationId, fleet, pageSize, page, filter);
            }
            string s = string.Empty;
            foreach (DataRow row in dsVehicle.Tables[0].Rows)
            {
                s += "\t<tr data-vehilceid=\"" + row["VehicleId"] + "\" data-licenseplate=\"" + row["LicensePlate"] + "\" rel=\"" + row["BoxId"].ToString() + "\" class=\"vehicleitem\"><td>" + row["Description"].ToString() + "</td>";
                if (manager) s += "<td>" + row["ManagerName"].ToString() + "</td>";
                s += "</tr>\n";
            }
            return s;
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            return string.Empty;
        }
        catch (Exception Ex)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            return string.Empty;
        }
    } 
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetVehicleListByFilterByFleetIdForType(string fleetid, bool manager, int page, string[] filter0)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";
        try
        {
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            VLF.PATCH.Logic.PatchFleet _pf = null;
            HttpContext.Current.Response.AddHeader("Content-type", "application/json");
            string filterstring = "";
            if (filter0.Length > 0)
                filterstring = string.Join(";;;", filter0);
            //filterstring = filter0[0];
            page++;
            int curfleetId = 0;
            int.TryParse(fleetid, out curfleetId);
            if (curfleetId == 0)
                fleetid = Server.UrlDecode(fleetid);
            int pageSize = 10;
            if (ConfigurationManager.AppSettings["VehicleListTreePageSize"] != null)
                int.TryParse(ConfigurationManager.AppSettings["VehicleListTreePageSize"].ToString(), out pageSize);
            clsHOSManager hosManager = new clsHOSManager();
            string v = getVehicleListByFilterByFleetIdForType(sn.User.OrganizationId, fleetid, manager, pageSize, page, filterstring, curfleetId, sConnectionString);
            _results _r = new _results();
            _r.folderlist = "";
            _r.vehiclelist = v;
            if (curfleetId > 0)
            {
                _r.fleetId = curfleetId;
                _pf = new VLF.PATCH.Logic.PatchFleet(sConnectionString);
                _r.totalVehicles = _pf.GetVehiclesInfoTotalNumberByFilterByFleetId(curfleetId, filterstring);
                _pf.Dispose();
            }
            else
            {
                //fleetid store dot number start with @@TYP_
                _r.totalVehicles = hosManager.GetVehiclesInfoTotalNumberByType(sn.User.OrganizationId, fleetid.Substring(6), filterstring);
            }
            _r.vehiclePageSize = pageSize;
            _r.vehicleCurrentPage = page;
            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            //return serializer.Serialize(_r);
            HttpContext.Current.Response.Write(serializer.Serialize(_r));
            HttpContext.Current.Response.End();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddOrUpdateInspectionGroup() Page:frmDynamicInspections"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return "0";
        }
        return "";
    }    
    private static string getVehicleListByFilterByFleetIdForType(int organizationId, string fleet, bool manager, int pageSize, int page, string filter, int curfleetId, string sConnectionString)
    {
        try
        {
            VLF.PATCH.Logic.PatchFleet _pf = null;
            DataSet dsVehicle = null;
            if (curfleetId > 0)
            {
                _pf = new VLF.PATCH.Logic.PatchFleet(sConnectionString);
                dsVehicle = _pf.GetVehiclesInfoByFleetIdByFilterByPage(curfleetId, pageSize, page, filter);
                _pf.Dispose();
            }
            else
            {
                clsHOSManager hosManager = new clsHOSManager();
                fleet = fleet.Substring(6); ////fleetid store dot number start with @@TYP_
                dsVehicle = hosManager.GetVehiclesInfoByTypeByPage(organizationId, fleet, pageSize, page, filter);
            }
            string s = string.Empty;
            foreach (DataRow row in dsVehicle.Tables[0].Rows)
            {
                s += "\t<tr data-vehilceid=\"" + row["VehicleId"] + "\" data-licenseplate=\"" + row["LicensePlate"] + "\" rel=\"" + row["BoxId"].ToString() + "\" class=\"vehicleitem\"><td>" + row["Description"].ToString() + "</td>";
                if (manager) s += "<td>" + row["ManagerName"].ToString() + "</td>";
                s += "</tr>\n";
            }
            return s;
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            return string.Empty;
        }
        catch (Exception Ex)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            return string.Empty;
        }
    }           
    
}