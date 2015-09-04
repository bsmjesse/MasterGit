<%@ WebHandler Language="C#" Class="ScheduleData" %>

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using System.Text;
using System.Configuration;
using System.Data;
using System.Web.Script.Serialization;
using SentinelFM;
using System.Globalization;
using VLF.DAS.Logic;

public class ScheduleData : IHttpHandler, IRequiresSessionState
{
    private SentinelFMSession sn = null;
    public void ProcessRequest (HttpContext context) {
        sn = context.Session["SentinelFMSession"] as SentinelFMSession;
        if (sn == null) return;
        string req = context.Request["Req"];
        SARequestResult response = null;
        switch (req)
        {
            case "GetSetting":
                response = GetSASetting();
                break;
            case "GetFleets":
                response = GetFleets();
                break;
            case "GetVehicles":
                response = GetVehicles();
                break;
            case "GetDeptList":
                response = GetDeptList(context);
                break;
            case "GetStationList":
                response = GetStationList(context);
                break;
            case "GetReasonList":
                response = GetReasonList(context);
                break;
            case "GetGroupList":
                {
                    int depotId = 0;
                    if (!int.TryParse(context.Request["DepotId"], out depotId)) break;
                    response = GetGroupList(depotId);
                }
                break;
            case "GetGroupById":
                response = GetGroupById(context);
                break;
            case "SaveGroup":
                response = SaveGroup(context);
                break;
            case "CopyGroup":
                response = CopyGroup(context);
                break;
            case "DeleteGroup":
                response = DeleteGroup(context);
                break;
            case "GetScheduleList":
                response = GetScheduleist(context);
                break;
            case "CopySchedule":
                response = CopySchedule(context);
                break;
            case "GetRouteList":
                response = GetRouteList(context);
                break;
            case "DeleteRoute":
                response = DeleteRoute(context);
                break;
            case "GetRouteById":
                response = GetRouteById(context);
                break;
            case "SaveRoute":
                response = SaveRoute(context);
                break;
            case "GetRouteStationList":
                response = GetRouteStationList(context);
                break;
            case "DeleteRouteStation":
                response = DeleteRouteStation(context);
                break;
            case "GetRouteStationById":
                response = GetRouteStationById(context);
                break;
            case "SaveRouteStation":
                response = SaveRouteStation(context);
                break;
            case "GetReport":
                response = GetReport(context);
                break;
            case "GetLiveChartReport":
                response = GetLiveChartReport(context);
                break;
            case "UpdateRoute":
                response = UpdateRoute(context);
                break;
            case "UpdateRouteStation":
                response = UpdateRouteStation(context);
                break;
        }
        context.Response.ContentType = "application/json";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetExpires(DateTime.Now);
        if (response != null)
            context.Response.Write(response.ToJson());
    }

    private SARequestResult GetSASetting()
    {
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsSetting = db.GetSASetting(sn.User.OrganizationId);
        if (dsSetting == null || dsSetting.Tables.Count == 0) return SARequestResult.GetFailResult("fail");
        return SARequestResult.GetOKResult(dsSetting);
    }
    
    private SARequestResult GetVehicles()
    {
        SentinelFM.ServerDBOrganization.DBOrganization dbo = new SentinelFM.ServerDBOrganization.DBOrganization();
        clsUtility util = new clsUtility(sn);
        string xml = string.Empty;
        if (util.ErrCheck(dbo.GetOrganizationAllActiveVehiclesXmlByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
            if (util.ErrCheck(dbo.GetOrganizationAllActiveVehiclesXmlByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
            {
                return SARequestResult.GetFailResult("fail");
            }

        if (xml == "")
            return SARequestResult.GetFailResult("fail");
        DataSet dsVehicle = new DataSet();
        dsVehicle.ReadXml(new System.IO.StringReader(xml));
        return SARequestResult.GetOKResult(dsVehicle);
    }

    private SARequestResult GetFleets()
    {
        SentinelFM.ServerDBOrganization.DBOrganization dbo = new SentinelFM.ServerDBOrganization.DBOrganization();
        clsUtility util = new clsUtility(sn);
        string xml = string.Empty;
        if (util.ErrCheck(dbo.GetOrganizationAllActiveVehiclesXmlByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
            if (util.ErrCheck(dbo.GetOrganizationAllActiveVehiclesXmlByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
            {
                return SARequestResult.GetFailResult("fail");
            }

        if (xml == "")
            return SARequestResult.GetFailResult("fail");
        DataSet dsVehicle = new DataSet();
        dsVehicle.ReadXml(new System.IO.StringReader(xml));
        return SARequestResult.GetOKResult(dsVehicle);
    }

    private SARequestResult GetDeptList(HttpContext context)
    {
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsStations = db.GetStationsByOrganizationId(sn.User.OrganizationId);
        if (dsStations == null || dsStations.Tables.Count == 0) return SARequestResult.GetFailResult("fail");
        DataView view = dsStations.Tables[0].DefaultView;
        view.RowFilter = "TypeId=2";
        return SARequestResult.GetOKResult(view);
    }

    private SARequestResult GetStationList(HttpContext context)
    {
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsStations = db.GetStationsByOrganizationId(sn.User.OrganizationId);
        if (dsStations == null || dsStations.Tables.Count == 0) return SARequestResult.GetFailResult("fail");
        DataView view = dsStations.Tables[0].DefaultView;
        view.RowFilter = "TypeId=1";
        return SARequestResult.GetOKResult(view);
    }

    private SARequestResult GetReasonList(HttpContext context)
    {
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsReasons = db.GetReasonCodesByOrganizationId(sn.User.OrganizationId);
        if (dsReasons == null || dsReasons.Tables.Count == 0) return SARequestResult.GetFailResult("fail");
        DataView view = dsReasons.Tables[0].DefaultView;
        return SARequestResult.GetOKResult(view);
    }
    
    private SARequestResult GetGroupList(int depotId)
    {
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsStations = db.GetScheduleGroupsByDepotId(depotId);
        if (dsStations == null || dsStations.Tables.Count == 0) return SARequestResult.GetFailResult("fail");
        return SARequestResult.GetOKResult(dsStations.Tables[0]);
    }

    private SARequestResult GetGroupById(HttpContext ctx)
    {
        int groupId = -1;
        int.TryParse(ctx.Request["GroupId"], out groupId);
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsGroup = db.GetScheduleGroupById(groupId);
        if (dsGroup == null || dsGroup.Tables.Count == 0) return SARequestResult.GetFailResult("fail");
        return SARequestResult.GetOKResult(dsGroup.Tables[0]);
    }
    
    private SARequestResult SaveGroup(HttpContext ctx)
    {
        int depotId = 0;
        if (!int.TryParse(ctx.Request["DepotId"], out depotId)) return SARequestResult.GetFailResult("fail");
        int groupId = -1;
        int.TryParse(ctx.Request["GroupId"], out groupId);
        DateTime beginDate = DateTime.Now;
        if (!DateTime.TryParse(ctx.Request["BeginDate"], out beginDate)) return SARequestResult.GetFailResult("fail");
        int duration = -1;
        if (!int.TryParse(ctx.Request["Duration"], out duration)) return SARequestResult.GetFailResult("fail");
        string description = ctx.Request["Description"];
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        if (groupId == 0)
            db.AddScheduleGroup(depotId, beginDate, duration, sn.UserID, description);
        else
            db.UpdateScheduleGroup(groupId, description, sn.UserID);
        return SARequestResult.GetOKResult();
    }

    private SARequestResult CopyGroup(HttpContext ctx)
    {
        int groupId = -1;
        int.TryParse(ctx.Request["GroupId"], out groupId);
        DateTime beginDate = DateTime.Now;
        if (!DateTime.TryParse(ctx.Request["BeginDate"], out beginDate)) return SARequestResult.GetFailResult("fail");
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        db.CopyScheduleGroup(groupId, beginDate, sn.UserID);
        return SARequestResult.GetOKResult();
    }

    private SARequestResult DeleteGroup(HttpContext ctx)
    {
        int groupId = -1;
        int.TryParse(ctx.Request["GroupId"], out groupId);
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        db.DeleteScheduleGroup(groupId);
        return SARequestResult.GetOKResult();
    }

    private SARequestResult DeleteRoute(HttpContext ctx)
    {
        int routeId = -1;
        int.TryParse(ctx.Request["RouteId"], out routeId);
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        db.DeleteRouteById(routeId);
        return SARequestResult.GetOKResult();
    }

    private SARequestResult DeleteRouteStation(HttpContext ctx)
    {
        int routeStationId = -1;
        int.TryParse(ctx.Request["RouteStationId"], out routeStationId);
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        db.DeleteRouteStation(routeStationId);
        return SARequestResult.GetOKResult();
    }
    
    private SARequestResult CopySchedule(HttpContext ctx)
    {
        int orgScheduleId = -1;
        int.TryParse(ctx.Request["OrgScheduleId"], out orgScheduleId);
        int desScheduleId = -1;
        int.TryParse(ctx.Request["DesScheduleId"], out desScheduleId);
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        db.CopySchedule(orgScheduleId, desScheduleId, sn.UserID);
        return SARequestResult.GetOKResult();
    }

    private SARequestResult GetScheduleist(HttpContext ctx)
    {
        int groupId = 0;
        if (!int.TryParse(ctx.Request["GroupId"], out groupId)) return SARequestResult.GetFailResult("fail");
        
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsStations = db.GetSchedulesByGroupId(groupId);
        if (dsStations == null || dsStations.Tables.Count == 0) return SARequestResult.GetFailResult("fail");
        return SARequestResult.GetOKResult(dsStations.Tables[0]);
    }

    private SARequestResult GetRouteList(HttpContext ctx)
    {
        int scheduleId = 0;
        if (!int.TryParse(ctx.Request["ScheduleId"], out scheduleId)) return SARequestResult.GetFailResult("fail");

        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsRoutes = db.GetRoutesByScheduleId(scheduleId);
        if (dsRoutes == null || dsRoutes.Tables.Count == 0) return SARequestResult.GetFailResult("fail");
        return SARequestResult.GetOKResult(dsRoutes.Tables[0]);
    }

    private SARequestResult GetRouteById(HttpContext ctx)
    {
        int routeId = 0;
        if (!int.TryParse(ctx.Request["RouteId"], out routeId)) return SARequestResult.GetFailResult("fail");

        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsRoute = db.GetRouteById(routeId);
        if (dsRoute == null || dsRoute.Tables.Count == 0) return SARequestResult.GetFailResult("fail");
        return SARequestResult.GetOKResult(dsRoute.Tables[0]);
    }

    private SARequestResult SaveRoute(HttpContext ctx)
    {
        int scheduleId = 0;
        if (!int.TryParse(ctx.Request["ScheduleId"], out scheduleId)) return SARequestResult.GetFailResult("fail");
        int routeId = 0;
        if (!int.TryParse(ctx.Request["RouteId"], out routeId)) return SARequestResult.GetFailResult("fail");
        string name = ctx.Request["Name"];
        int vehicleId = 0;
        if (!int.TryParse(ctx.Request["VehicleId"], out vehicleId)) return SARequestResult.GetFailResult("fail");
        int departureTime = 0;
        if (!int.TryParse(ctx.Request["DepartureTime"], out departureTime)) return SARequestResult.GetFailResult("fail");
        int arrivalTime = 0;
        if (!int.TryParse(ctx.Request["ArrivalTime"], out arrivalTime)) return SARequestResult.GetFailResult("fail");
        string description = ctx.Request["Description"];
        if (string.IsNullOrEmpty(description) || description == "null")
            description = null;

        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        if (routeId == 0)
            db.AddRoute(scheduleId, name, vehicleId, departureTime, arrivalTime, sn.UserID, description);
        else
            db.UpdateRoute(routeId, name, vehicleId, departureTime, arrivalTime, sn.UserID, description);
        return SARequestResult.GetOKResult();
    }

    private SARequestResult GetRouteStationList(HttpContext ctx)
    {
        int routeId = 0;
        if (!int.TryParse(ctx.Request["RouteId"], out routeId)) return SARequestResult.GetFailResult("fail");

        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsStations = db.GetRouteStationsByRouteId(routeId);
        if (dsStations == null || dsStations.Tables.Count == 0) return SARequestResult.GetFailResult("fail");
        return SARequestResult.GetOKResult(dsStations.Tables[0]);
    }

    private SARequestResult GetRouteStationById(HttpContext ctx)
    {
        int routeStationId = 0;
        if (!int.TryParse(ctx.Request["RouteStationId"], out routeStationId)) return SARequestResult.GetFailResult("fail");

        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsStation = db.GetRouteStationById(routeStationId);
        if (dsStation == null || dsStation.Tables.Count == 0) return SARequestResult.GetFailResult("fail");
        return SARequestResult.GetOKResult(dsStation.Tables[0]);
    }

    private SARequestResult SaveRouteStation(HttpContext ctx)
    {
        int routeId = 0;
        if (!int.TryParse(ctx.Request["RouteId"], out routeId)) return SARequestResult.GetFailResult("fail");
        int routeStationId = 0;
        if (!int.TryParse(ctx.Request["RouteStationId"], out routeStationId)) return SARequestResult.GetFailResult("fail");
        int stationId = 0;
        if (!int.TryParse(ctx.Request["StationId"], out stationId)) return SARequestResult.GetFailResult("fail");
        int deliveryTypeId = 0;
        if (!int.TryParse(ctx.Request["DevliveryTypeId"], out deliveryTypeId)) return SARequestResult.GetFailResult("fail");
        int departureTime = 0;
        if (!int.TryParse(ctx.Request["DepartureTime"], out departureTime)) return SARequestResult.GetFailResult("fail");
        int arrivalTime = 0;
        if (!int.TryParse(ctx.Request["ArrivalTime"], out arrivalTime)) return SARequestResult.GetFailResult("fail");
        string description = ctx.Request["Description"];
        if (string.IsNullOrEmpty(description) || description == "null")
            description = null;

        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        if (routeStationId == 0)
            db.AddRouteStation(routeId, stationId, deliveryTypeId, departureTime, arrivalTime, sn.UserID, description);
        else
            db.UpdateRouteStation(routeStationId, stationId, deliveryTypeId, departureTime, arrivalTime, sn.UserID, description);
        return SARequestResult.GetOKResult();
    }

    public SARequestResult UpdateRoute(HttpContext ctx)
    {
        int routeId = 0;
        if (!int.TryParse(ctx.Request["RouteId"], out routeId)) return SARequestResult.GetFailResult("fail");
        int? departureReasonId = null;
        int i = 0;
        if (int.TryParse(ctx.Request["DepartureReasonId"], out i))
            departureReasonId = i;
        int? arrivalReasonId = null;
        if (int.TryParse(ctx.Request["ArrivalReasonId"], out i))
            arrivalReasonId = i;
        string descrption = ctx.Request["Description"];
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        db.UpdateRoute(routeId, departureReasonId, arrivalReasonId, descrption);
        return SARequestResult.GetOKResult();
    }
    
    public SARequestResult UpdateRouteStation(HttpContext ctx)
    {
        int routeStationId = 0;
        if (!int.TryParse(ctx.Request["RouteStationId"], out routeStationId)) return SARequestResult.GetFailResult("fail");
        int? departureReasonId = null;
        int i = 0;
        if (int.TryParse(ctx.Request["DepartureReasonId"], out i))
            departureReasonId = i;
        int? arrivalReasonId = null;
        if (int.TryParse(ctx.Request["ArrivalReasonId"], out i))
            arrivalReasonId = i;
        string descrption = ctx.Request["Description"];
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        db.UpdateRouteStation(routeStationId, departureReasonId, arrivalReasonId, descrption);
        return SARequestResult.GetOKResult();
    }
    
    private SARequestResult GetReport(HttpContext ctx)
    {
        int depotId = 0;
        int.TryParse(ctx.Request["DepotId"], out depotId);
        int stationId = 0;
        int.TryParse(ctx.Request["StatiionId"], out stationId);
        int vehicleId = 0;
        int.TryParse(ctx.Request["VehicleId"], out vehicleId);
        int statusId = 0;
        if (!int.TryParse(ctx.Request["StatusId"], out statusId)) return SARequestResult.GetFailResult("fail");
        DateTime startDate = DateTime.MinValue;
        if (!DateTime.TryParse(ctx.Request["StartDate"], out startDate)) return SARequestResult.GetFailResult("fail");
        DateTime endDate = DateTime.MinValue;
        if (!DateTime.TryParse(ctx.Request["EndDate"], out endDate)) return SARequestResult.GetFailResult("fail");

        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet ds = db.GetReport(depotId, stationId, vehicleId, stationId, startDate, endDate);
        return SARequestResult.GetOKResult(ds);
    }

    private SARequestResult GetLiveChartReport(HttpContext ctx)
    {
        int organizationId = sn.User.OrganizationId;
        string command = ctx.Request["Command"];
        DateTime startDate = DateTime.MinValue;
        if (!DateTime.TryParse(ctx.Request["StartDate"], out startDate)) return SARequestResult.GetFailResult("fail");
        DateTime endDate = DateTime.MinValue;
        if (!DateTime.TryParse(ctx.Request["EndDate"], out endDate)) return SARequestResult.GetFailResult("fail");
        string routeName = ctx.Request["RouteName"];
        long? vehicleId = null;
        long l;
        if (long.TryParse(ctx.Request["VehicleId"], out l))
            vehicleId = l;
        int? stationId = null;
        int i;
        if (int.TryParse(ctx.Request["StationId"], out i))
            stationId = i;
        
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet ds = null;
        switch (command)
        {
            case "StArrival":
                ds = db.GetStArrivalReport(organizationId, startDate, endDate, routeName, vehicleId, stationId);
                break;
        } 
        return SARequestResult.GetOKResult(ds);
    }
    
    protected string RealTimeDB_ConnectionString
    {
        get
        {
            return ConfigurationManager.ConnectionStrings["SentinelFMStaging"].ConnectionString;
        }
    }
    protected string ReportDB_ConnectionString
    {
        get
        {
            return ConfigurationManager.ConnectionStrings["DWH"].ConnectionString;
        }
    }
    protected string SpatialDB_ConnectionString
    {
        get
        {
            return ConfigurationManager.AppSettings["SpatialDB"];
        }
    }
    public bool IsReusable
    {
        get {
            return false;
        }
    }

}

