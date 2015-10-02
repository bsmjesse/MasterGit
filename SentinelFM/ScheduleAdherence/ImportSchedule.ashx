<%@ WebHandler Language="C#" Class="ImportSchedule" %>

using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Data;
using System.Configuration;
using System.Web.SessionState;
using SentinelFM;
using System.Globalization;
using VLF.DAS.Logic;

public class ImportSchedule : IHttpHandler, IRequiresSessionState
{
    private SentinelFMSession sn = null;
    private const string EngineSessionName = "_UPLOADCONTENT_";
    public void ProcessRequest (HttpContext context) {
        SARequestResult response = null;
        sn = context.Session["SentinelFMSession"] as SentinelFMSession;
        if (sn != null)
        {
            string command = context.Request["command"];
            switch (command)
            {
                case "upload":
                    response = Upload(context);
                    break;
                case "Ignore":
                    response = Ignore(context);
                    break;
                case "cancel":
                    response = Cancel(context);
                    break;
                case "inquiry":
                    response = inquiry(context);
                    break;
            }
        }
        context.Response.ContentType = "application/json";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetExpires(DateTime.Now);
        if (response != null)
            context.Response.Write(response.ToJson());
    }

    private SARequestResult Cancel(HttpContext ctx)
    {
        ImportEngine engine = ctx.Session[EngineSessionName] as ImportEngine;
        if (engine == null)
            return SARequestResult.GetFailResult("fail.");
        ctx.Session[EngineSessionName] = null;
        return SARequestResult.GetFailResult("Cancel at " + engine.CurLineNum + " line.");
    }
    
    private SARequestResult Ignore(HttpContext ctx)
    {
        ImportEngine engine = ctx.Session[EngineSessionName] as ImportEngine;
        if (engine == null)
            return SARequestResult.GetFailResult("fail.");
        bool result = engine.Ignore();
        ctx.Session[EngineSessionName] = engine;
        if (result)
            return SARequestResult.GetOKResult();
        else
            return SARequestResult.GetFailResult(engine.ErrorState.ToString() + " at " + engine.CurLineNum + " line.");
    }

    private SARequestResult inquiry(HttpContext ctx)
    {
        ImportEngine engine = ctx.Session[EngineSessionName] as ImportEngine;
        SARequestResult result = SARequestResult.GetOKResult();
        result.Object = engine._progress;
        return result;
    }

    private SARequestResult Upload(HttpContext ctx)
    {
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsSetting = db.GetSASetting(sn.User.OrganizationId);
        if (dsSetting.Tables.Count == 0 || dsSetting.Tables[0].Rows.Count == 0) 
            return SARequestResult.GetFailResult(UploadState.NoPermission.ToString());
        if (dsSetting.Tables[0].Rows[0]["ImportFormat"] == DBNull.Value)
            return SARequestResult.GetFailResult(UploadState.NoPermission.ToString());
        string format = dsSetting.Tables[0].Rows[0]["ImportFormat"].ToString();
        
        HttpPostedFile file = ctx.Request.Files[0];
        List<string> lines = new List<string>();
        using (StreamReader sr = new StreamReader(file.InputStream))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
                lines.Add(line);
        }
        ImportEngine engine = null;
        switch (format)
        {
            case "Sobeys":
                engine = new ImportEngineBySobey(lines, ReportDB_ConnectionString);
                break;
        }
        if (engine == null) return SARequestResult.GetFailResult(UploadState.NoPermission.ToString());
        
        int groupId = -1;
        if (!int.TryParse(ctx.Request["GroupId"], out groupId)) return SARequestResult.GetFailResult(UploadState.FormatError.ToString());
        bool overlap = false;
        if (!bool.TryParse(ctx.Request["overlap"], out overlap)) return SARequestResult.GetFailResult(UploadState.FormatError.ToString());
        engine.Reload(sn.User.OrganizationId, groupId, overlap, sn);
        bool result = engine.PreProgress();
        ctx.Session[EngineSessionName] = engine;
        if (result)
            return SARequestResult.GetOKResult();
        else
            return SARequestResult.GetFailResult(engine.ErrorState.ToString() + " at " + engine.CurLineNum + " line");
    }

    protected string ReportDB_ConnectionString
    {
        get
        {
            return ConfigurationManager.ConnectionStrings["DWH"].ConnectionString;
        }
    }
    
    public bool IsReusable
    {
        get {
            return false;
        }
    }

    public enum UploadState
    {
        NoPermission,
        FormatError,
        NoStation,
        NoVehicle,
        LineError,
        StationInvalid,
        DeliveryInvalid,
        None
    }
    
    public class SAStation
    {
        public int stationId;
        public int deliveryId = -1;
        public int arrivaltime = -1;
        public int departuretime = -1;
    }
    public class SARoute
    {
        public string name;
        public int vehicleId = -1;
        public int departuretime = -1;
        public int arrivaltime = -1;
        public string description;
        public List<SAStation> stations = new List<SAStation>();
    }
    public class SAProgress
    {
        public string Message = "";
        public int Progress = 0;
        public void SetProgress(double d, string curJob)
        {
            Message = curJob;
            Progress = Convert.ToInt32(d);
        }
    }
    
    public abstract class ImportEngine
    {
        private int _userId;
        private int _groupId;
        protected string[] _lines;
        protected string _connString;
        protected int _curLineNum = 0;
        private bool _bOverlap;
        private int _duration = 0;
        private DateTime _groupBeginDate;
        protected DataTable _dtSchedule;
        private DataTable _dtVehicle;
        private DataTable _dtStation;
        protected Dictionary<DateTime, Dictionary<string, SARoute>> _scheduleDictionary = new Dictionary<DateTime, Dictionary<string, SARoute>>();
        protected Dictionary<DateTime, int> _scheduleIdDictionary = new Dictionary<DateTime, int>();
        public UploadState ErrorState = UploadState.None;
        public SAProgress _progress = new SAProgress();
        private const int LoadingFileData_Completed = 10;
        private const int LoadingBasicData_Completed = 30;
        private const int LoadingImportData_Completed = 80;

        protected int GroupDuration
        {
            get { return _duration; }
        }
        protected DateTime GroupBeginDate
        {
            get { return _groupBeginDate; } 
        }
        public int CurLineNum
        {
            get { return _curLineNum; }
        }
        public ImportEngine(List<string> lines, string connectionString)
        {
            _lines = lines.ToArray();
            _connString = connectionString;
            _progress.SetProgress(0, "Loading");
        }

        private DataSet GetVehicles(SentinelFMSession sn)
        {
            SentinelFM.ServerDBOrganization.DBOrganization dbo = new SentinelFM.ServerDBOrganization.DBOrganization();
            clsUtility util = new clsUtility(sn);
            string xml = string.Empty;
            if (util.ErrCheck(dbo.GetOrganizationAllActiveVehiclesXmlByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                if (util.ErrCheck(dbo.GetOrganizationAllActiveVehiclesXmlByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                {
                    return null;
                }

            if (xml == "")
                return null;
            DataSet dsVehicle = new DataSet();
            dsVehicle.ReadXml(new System.IO.StringReader(xml));
            return dsVehicle;
        }

        public virtual bool Reload(int organizationId, int groupId, bool overlap, SentinelFMSession sn)
        {
            _progress.SetProgress(LoadingFileData_Completed, "Loading base data.");
            _userId = sn.UserID;
            _bOverlap = overlap;
            _groupId = groupId;
            ScheduleAdherence db = new ScheduleAdherence(_connString);
            DataSet dsGroup = db.GetScheduleGroupById(groupId);
            if (dsGroup.Tables.Count == 0 || dsGroup.Tables[0].Rows.Count == 0)
            {
                ErrorState = UploadState.FormatError;
                return false;
            }
            _duration = 0;
            if (!int.TryParse(dsGroup.Tables[0].Rows[0]["Duration"].ToString(), out _duration))
            {
                ErrorState = UploadState.FormatError;
                return false;
            }
            _groupBeginDate = DateTime.MinValue;
            if (!DateTime.TryParse(dsGroup.Tables[0].Rows[0]["ScheduleBeginDate"].ToString(), out _groupBeginDate))
            {
                ErrorState = UploadState.FormatError;
                return false;
            }
            
            DataSet dsSchedule = db.GetSchedulesByGroupId(groupId);
            _dtSchedule = dsSchedule.Tables[0];
            foreach (DataRow row in _dtSchedule.Rows)
            {
                DateTime date = DateTime.Parse(row["ScheduleBeginDate"].ToString());
                int scheduleId = int.Parse(row["ScheduleId"].ToString());
                _scheduleDictionary[date] = new Dictionary<string,SARoute>();
                _scheduleIdDictionary[date] = scheduleId;
            }

            DataSet dsStation = db.GetStationsByOrganizationId(organizationId);
            if (dsStation.Tables.Count == 0)
            {
                ErrorState = UploadState.NoStation;
                return false;
            }
            _dtStation = dsStation.Tables[0];

            DataSet dsVehicle = GetVehicles(sn);
            if (dsVehicle.Tables.Count == 0)
            {
                ErrorState = UploadState.NoVehicle;
                return false;
            }
            _dtVehicle = dsVehicle.Tables[0];
            _progress.SetProgress(LoadingBasicData_Completed, "Loading import data.");
            return true;
        }

        public bool PreProgress()
        {
            return Continue();
        }

        public bool Ignore()
        {
            _curLineNum++;
            return Continue();
        }
        
        private bool Continue()
        {
            int lineNum;
            while ((lineNum = DoNextLine()) != 0)
            {
                if (lineNum < 0)
                {
                    return false;
                }
                _curLineNum += lineNum;
            }
            _progress.SetProgress(LoadingImportData_Completed, "Submitting import data.");
            return Submit();
        }
        
        protected virtual int DoNextLine()
        {
            return 1;
        }

        protected int GetStationIdByNumber(string stationNumber)
        {
            DataRow[] stations = _dtStation.Select("StationNumber = '" + stationNumber + "'");
            if (stations == null || stations.Length == 0)
                return -1;
            int stationId = int.Parse(stations[0]["StationId"].ToString());
            return stationId;
        }
        
        public bool Submit()
        {
            ScheduleAdherence db = new ScheduleAdherence(_connString);
            foreach (DateTime date in _scheduleDictionary.Keys)
            {
                Dictionary<string, SARoute> schedule = _scheduleDictionary[date];
                int scheduleId = _scheduleIdDictionary[date];
                if (_bOverlap)
                    db.DeleteRouteByScheduleId(scheduleId);
                foreach (string routeName in schedule.Keys)
                {
                    SARoute route = schedule[routeName];
                    int? vehicleId = null;
                    if (route.vehicleId > 0)
                        vehicleId = route.vehicleId;
                    int? depart_sec = null;
                    if (route.departuretime >= 0)
                        depart_sec = route.departuretime;
                    int? arrival_sec = null;
                    if (route.arrivaltime >= 0)
                        arrival_sec = route.arrivaltime;
                    int routeId = db.AddRoute(scheduleId, route.name, vehicleId, depart_sec, arrival_sec, _userId, null);
                    foreach (SAStation station in route.stations)
                    {
                        db.AddRouteStation(routeId, station.stationId, station.deliveryId, station.departuretime, station.arrivaltime, _userId, null);
                    }
                }
            }
            return true;
        }
    }

    public class ImportEngineBySobey : ImportEngine
    {
        public ImportEngineBySobey(List<string> lines, string connectionString)
            : base(lines, connectionString)
        {
        }

        public override bool Reload(int organizationId, int groupId, bool overlap, SentinelFMSession sn)
        {
            bool result = base.Reload(organizationId, groupId, overlap, sn);
            if (!result) return false;
            if (GroupDuration != 7 || GroupBeginDate.Date.DayOfWeek != DayOfWeek.Sunday)
            {
                ErrorState = UploadState.FormatError;
                return false;
            }
            return true;
        }

        private DateTime? GetDateByDayofweek(string dayofweek)
        {
            dayofweek = dayofweek.ToLower();
            int offset = -1;
            switch (dayofweek)
            {
                case "sun":
                case "sunday":
                    offset = 0;
                    break;
                case "mon":
                case "monday":
                    offset = 1;
                    break;
                case "tue":
                case "tuesday":
                    offset = 2;
                    break;
                case "wed":
                case "wednesday":
                    offset = 3;
                    break;
                case "thu":
                case "thursday":
                    offset = 4;
                    break;
                case "fri":
                case "friday":
                    offset = 5;
                    break;
                case "sat":
                case "saturday":
                    offset = 6;
                    break;
            }
            if (offset != -1)
                return GroupBeginDate.Date.AddDays(offset);
            else
                return null;
        }

        private DateTime? BuildDateTime(string dayPart, string timePart)
        {
            DateTime? date = GetDateByDayofweek(dayPart);
            if (date == null)
            {
                ErrorState = UploadState.FormatError;
                return null;
            }
            DateTime datetime;
            if (!DateTime.TryParse(date.Value.ToLongDateString() + " " + timePart, out datetime))
            {
                ErrorState = UploadState.FormatError;
                return null;
            }
            return datetime;
        }
        
        protected override int DoNextLine()
        {
            if (_curLineNum > _lines.Length - 1) return 0;
            string line = _lines[_curLineNum];
            if (line[0] == '-') return 1;
            if (line[0] != '|') return 1;
            string[] strList = line.Split('|');
            if (strList.Length != 14)
            {
                ErrorState = UploadState.LineError;
                return -1;
            }
            if (strList[2].Trim() == "Customer Name") return 1;
            for (int i = 1; i <= 10; i++)
                strList[i] = strList[i].Trim();

            if (strList[1] == "")
            {
                ErrorState = UploadState.StationInvalid;
                return -1;
            }
            if (strList[3] == "")
            {
                ErrorState = UploadState.DeliveryInvalid;
                return -1;
            }
            if (strList[5] == "" || strList[6] == "" || strList[7] == "" || strList[8] == "" || strList[9] == "" || strList[10] == "" || strList[11] == "")
            {
                ErrorState = UploadState.FormatError;
                return -1;
            }
            int stationId = GetStationIdByNumber(strList[1]);
            if (stationId < 0)
            {
                ErrorState = UploadState.StationInvalid;
                return -1;
            }
            int deliveryId = 0;
            if (!int.TryParse(strList[3], out deliveryId))
            {
                ErrorState = UploadState.DeliveryInvalid;
                return -1;
            }

            DateTime? scheduleDate = GetDateByDayofweek(strList[5]);
            if (scheduleDate == null)
            {
                ErrorState = UploadState.FormatError;
                return -1;
            }
            Dictionary<string, SARoute> routeDictionay = _scheduleDictionary[scheduleDate.Value];
            SARoute route = null;
            if (!routeDictionay.TryGetValue(strList[6], out route))
            {
                route = new SARoute();
                route.name = strList[6];
                DateTime? rscDepartureDate = BuildDateTime(strList[8], strList[9]);
                if (rscDepartureDate == null)
                {
                    ErrorState = UploadState.FormatError;
                    return -1;
                }
                route.departuretime = Convert.ToInt32((rscDepartureDate.Value - scheduleDate.Value).TotalSeconds);
                routeDictionay[strList[6]] = route;
            }
            route = routeDictionay[strList[6]];
            SAStation station = new SAStation();
            DateTime? stationArrivalDate = BuildDateTime(strList[10], strList[11]);
            if (stationArrivalDate == null)
            {
                ErrorState = UploadState.FormatError;
                return -1;
            }
            station.arrivaltime = Convert.ToInt32((stationArrivalDate.Value - scheduleDate.Value).TotalSeconds);
            station.departuretime = Convert.ToInt32((stationArrivalDate.Value.AddHours(1) - scheduleDate.Value).TotalSeconds);
            station.stationId = stationId;
            station.deliveryId = deliveryId;
            route.stations.Add(station);            
            return 1;
        }
    }
}