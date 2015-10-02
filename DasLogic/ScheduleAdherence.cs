using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VLF.DAS.DB;

namespace VLF.DAS.Logic
{
    public class ScheduleAdherence : Das
    {
        #region General Interfaces
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public ScheduleAdherence(string connectionString)
            : base(connectionString)
        {
        }
        /// <summary>
        /// Destructor
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
        }
        #endregion

        public DataSet GetLandmarksByOrganizationId(int organizationId)
        {
            Landmark db = new Landmark(sqlExec);
            string filter = "where OrganizationId = " + organizationId + " and LandmarkType != 'ROUTE'";
            return db.GetRowsByFilter(filter);
        }

        public DataSet GetLandmarkById(int landmarkId)
        {
            Landmark db = new Landmark(sqlExec);
            string filter = "where LandmarkId = " + landmarkId;
            return db.GetRowsByFilter(filter);
        }

        public DataSet GetStationsByOrganizationId(int organizationId)
        {
            /*
            SAStation db = new SAStation(sqlExec);
            DataSet dsStations = db.GetStationsByOrganizationId(organizationId);
            if (dsStations == null || dsStations.Tables.Count == 0) return null;
            return dsStations;
             */
            StringBuilder sql = new StringBuilder();
            sql.Append("select s.*, l.latitude, l.longitude, l.radius, l.timezone, l.daylightsaving, l.streetaddress ");
            sql.Append("from saStation s ");
            sql.Append("join vlflandmark l on s.landmarkid = l.landmarkid ");
            sql.Append(string.Format("where l.organizationid = {0} ", organizationId));
            return sqlExec.SQLExecuteDataset(sql.ToString());
        }

        public DataSet GetStationById(int stationId)
        {

            SAStation db = new SAStation(sqlExec);
            DataSet dsStation = db.GetStationById(stationId);
            return dsStation;
        }

        public void AddStation(int organizationId, string stationName, long landmarkId, int typeId,
           string stationNumber, string description, int userId, string contractName, string phoneNumber,
            string faxNumber, string address, string emailAddress)
        {
            SAStation db = new SAStation(sqlExec);
            db.AddStation(organizationId, stationName, landmarkId, typeId, stationNumber, description, DateTime.Now, userId, contractName,
                phoneNumber, faxNumber, address, emailAddress);
        }

        public void UpdateStation(int stationId, string stationName, long landmarkId, int typeId,
           string stationNumber, string description, int userId, string contractName, string phoneNumber,
            string faxNumber, string address, string emailAddress)
        {
            SAStation db = new SAStation(sqlExec);
            db.UpdateStation(stationId, stationName, landmarkId, typeId, stationNumber, description, DateTime.Now, userId, contractName,
                phoneNumber, faxNumber, address, emailAddress);
        }

        public int DeleteStation(int stationId)
        {
            SAStation db = new SAStation(sqlExec);
            return db.DeleteStationById(stationId);
        }

        public DataSet GetReasonCodesByOrganizationId(int organizationId)
        {
            SAReasonCode db = new SAReasonCode(sqlExec);
            DataSet dsReasonCodes = db.GetReasonCodesByOrganizationId(organizationId);
            if (dsReasonCodes == null || dsReasonCodes.Tables.Count == 0) return null;
            return dsReasonCodes;
        }

        public DataSet GetReasonCodeById(int reasonCodeId)
        {
            SAReasonCode db = new SAReasonCode(sqlExec);
            DataSet dsReasonCode = db.GetReasonCodeById(reasonCodeId);
            return dsReasonCode;
        }

        public void AddReasonCode(int organizationId, string reasonCode, string description, int userId)
        {
            SAReasonCode db = new SAReasonCode(sqlExec);
            db.AddReasonCode(organizationId, reasonCode, description, DateTime.Now, userId);
        }

        public void UpdateReasonCode(int reasonCodeId, string reasonCode, string description, int userId)
        {
            SAReasonCode db = new SAReasonCode(sqlExec);
            db.UpdateReasonCode(reasonCodeId, reasonCode, description, DateTime.Now, userId);
        }

        public int DeleteReasonCode(int reasonCodeId)
        {
            SAReasonCode db = new SAReasonCode(sqlExec);
            return db.DeleteReasonCodeById(reasonCodeId);
        }

        public int AddScheduleGroup(int stationId, DateTime scheduleBeginDate, int duration, int userId, string description)
        {
            if (stationId <= 0) return -1;
            if (duration < 1) return -1;
            DateTime now = DateTime.Now;
            SAScheduleGroup db = new SAScheduleGroup(sqlExec);
            int groupId = db.AddScheduleGroup(stationId, scheduleBeginDate, duration, DateTime.Now, now, userId, description);
            SASchedule scheduleDB = new SASchedule(sqlExec);
            for (int i = 0; i < duration; i++)
            {
                scheduleDB.AddSchedule(groupId, scheduleBeginDate.Date.AddDays(i), now, userId);
            }
            return groupId;
        }
        public void CopyRouteStation(int org_routeId, int des_routeId, int userId)
        {
            DataSet dsStations = GetRouteStationsByRouteId(org_routeId);
            if (dsStations == null || dsStations.Tables[0].Rows.Count == 0) return;
            foreach (DataRow org_station in dsStations.Tables[0].Rows)
            {
                int stationId = int.Parse(org_station["StationId"].ToString());
                int deliveryId = int.Parse(org_station["DeliveryTypeId"].ToString());
                int departuretime = int.Parse(org_station["DepartureSchedule_sec"].ToString());
                int arrivalTime = int.Parse(org_station["ArrivalSchedule_sec"].ToString());
                string description = org_station["Description"] == DBNull.Value ? null : org_station["Description"].ToString();
                AddRouteStation(des_routeId, stationId, deliveryId, departuretime, arrivalTime, userId, description);
            }
        }

        public void CopySchedule(int org_scheduleId, int des_scheduleId, int userId)
        {
            if (org_scheduleId <= 0 || des_scheduleId <= 0) return;
            DataSet dsRoutes = GetRoutesByScheduleId(org_scheduleId);
            if (dsRoutes == null || dsRoutes.Tables[0].Rows.Count == 0) return;
            DeleteRouteByScheduleId(des_scheduleId);
            foreach (DataRow org_route in dsRoutes.Tables[0].Rows)
            {
                int org_routeId = int.Parse(org_route["RouteId"].ToString());
                string routeName = org_route["Name"].ToString();

                int? vehicleId = null;
                if (org_route["VehicleId"] != DBNull.Value)
                    vehicleId = int.Parse(org_route["VehicleId"].ToString());
                int? departtime = null;
                if (org_route["RSCDepartureTime_sec"] != DBNull.Value)
                    departtime = int.Parse(org_route["RSCDepartureTime_sec"].ToString());
                int? arrivaltime = null;
                if (org_route["RSCArrivalTime_sec"] != DBNull.Value)
                    arrivaltime = int.Parse(org_route["RSCArrivalTime_sec"].ToString());
                string description = org_route["Description"] == DBNull.Value ? null : org_route["Description"].ToString();
                int des_routeId = AddRoute(des_scheduleId, routeName, vehicleId, departtime, arrivaltime, userId, description);
                CopyRouteStation(org_routeId, des_routeId, userId);
            }
        }

        public void CopyScheduleGroup(int org_groupId, DateTime scheduleBeginDate, int userId)
        {
            if (org_groupId <= 0) return;
            DateTime now = DateTime.Now;
            DataSet dsGroup = GetScheduleGroupById(org_groupId);
            if (dsGroup == null || dsGroup.Tables[0].Rows.Count == 0) return;
            DataRow group = dsGroup.Tables[0].Rows[0];
            int depotId = int.Parse(group["RSCStationId"].ToString());
            DateTime org_beginDate = DateTime.Parse(group["ScheduleBeginDate"].ToString());
            int duration = int.Parse(group["Duration"].ToString());
            string description = group["Description"] == DBNull.Value ? null : group["Description"].ToString();
            int des_groupId = AddScheduleGroup(depotId, scheduleBeginDate, duration, userId, description);

            DataSet org_dsSchedules = GetSchedulesByGroupId(org_groupId);
            DataSet des_dsSchedules = GetSchedulesByGroupId(des_groupId);
            for (int i = 0; i < duration; i++)
            {
                DateTime org_date = org_beginDate.Date.AddDays(i);
                DateTime des_date = scheduleBeginDate.Date.AddDays(i);
                DataRow[] org_schedules = org_dsSchedules.Tables[0].Select("ScheduleBeginDate='" + org_date + "'");
                DataRow[] des_schedules = des_dsSchedules.Tables[0].Select("ScheduleBeginDate='" + des_date + "'");
                if (org_schedules.Length == 0 || des_schedules.Length == 0) continue;
                int org_scheduleId = int.Parse(org_schedules[0]["ScheduleId"].ToString());
                int des_scheduleId = int.Parse(des_schedules[0]["ScheduleId"].ToString());
                CopySchedule(org_scheduleId, des_scheduleId, userId);
            }
        }

        public void UpdateScheduleGroup(int groupId, string description, int userId)
        {
            if (groupId <= 0) return;
            SAScheduleGroup db = new SAScheduleGroup(sqlExec);
            db.UpdateScheduleGroup(groupId, description, DateTime.Now, userId);
        }

        public DataSet GetScheduleGroupsByDepotId(int depotId)
        {
            SAScheduleGroup db = new SAScheduleGroup(sqlExec);
            DataSet dsGroups = db.GetScheduleGroupsByRSCStationId(depotId);
            if (dsGroups == null || dsGroups.Tables.Count == 0) return null;
            return dsGroups;
        }

        public DataSet GetScheduleGroupById(int groupId)
        {
            SAScheduleGroup db = new SAScheduleGroup(sqlExec);
            DataSet dsGroup = db.GetScheduleGroupById(groupId);
            return dsGroup;
        }

        public int DeleteScheduleGroup(int groupId)
        {
            if (groupId <= 0) return -1;
            SASchedule dbSchedule = new SASchedule(sqlExec);
            DataSet dsSchedule = dbSchedule.GetSchedulesByGroupId(groupId);
            foreach (DataRow schedule in dsSchedule.Tables[0].Rows)
            {
                int scheduleId = int.Parse(schedule["ScheduleId"].ToString());
                DeleteRouteByScheduleId(scheduleId);
            }
            dbSchedule.DeleteSchedleByGroupId(groupId);
            SAScheduleGroup dbGroup = new SAScheduleGroup(sqlExec);
            return dbGroup.DeleteScheduleGroupById(groupId);
        }

        public DataSet GetSchedulesByGroupId(int groupId)
        {
            SASchedule db = new SASchedule(sqlExec);
            DataSet dsSchedule = db.GetSchedulesByGroupId(groupId);
            return dsSchedule;
        }

        public DataSet GetRoutesByScheduleId(int scheduleId)
        {
            SARoute db = new SARoute(sqlExec);
            DataSet dsRoutes = db.GetRoutesByScheduleId(scheduleId);
            return dsRoutes;
        }

        public DataSet GetRouteById(int routeId)
        {
            SARoute db = new SARoute(sqlExec);
            DataSet dsRoute = db.GetRouteById(routeId);
            return dsRoute;
        }

        public int AddRoute(int scheduleId, string name, int? vehicleId, int? departure_sec, int? arrival_sec,
            int userId, string description)
        {
            SARoute db = new SARoute(sqlExec);
            return db.AddRoute(scheduleId, name, vehicleId, departure_sec, arrival_sec, DateTime.Now, userId, description);
        }

        public void UpdateRoute(int routeId, string name, int? vehicleId, int? departure_sec, int? arrival_sec,
            int userId, string description)
        {
            SARoute db = new SARoute(sqlExec);
            db.UpdateRoute(routeId, name, vehicleId, departure_sec, arrival_sec, DateTime.Now, userId, description);
        }

        public void DeleteRouteById(int routeId)
        {
            SARouteStation dbStation = new SARouteStation(sqlExec);
            dbStation.DeleteRouteStationsByRouteId(routeId);
            SARoute db = new SARoute(sqlExec);
            db.DeleteRouteById(routeId);
        }

        public void DeleteRouteByScheduleId(int scheduleId)
        {
            SARouteStation dbStation = new SARouteStation(sqlExec);
            dbStation.DeleteRouteStationsByScheduleId(scheduleId);
            SARoute db = new SARoute(sqlExec);
            db.DeleteRouteByScheduleId(scheduleId);
        }

        public DataSet GetRouteStationsByRouteId(int routeId)
        {
            SARouteStation db = new SARouteStation(sqlExec);
            DataSet dsRoutes = db.GetRouteStationsByRouteId(routeId);
            return dsRoutes;
        }

        public DataSet GetRouteStationById(int routeStationId)
        {
            SARouteStation db = new SARouteStation(sqlExec);
            DataSet dsRoute = db.GetRouteStationById(routeStationId);
            return dsRoute;
        }

        public void AddRouteStation(int routeId, int stationId, int deliveryTypeId, int departure_sec, int arrival_sec,
            int userId, string description)
        {
            SARouteStation db = new SARouteStation(sqlExec);
            db.AddStation(routeId, stationId, deliveryTypeId, departure_sec, arrival_sec, DateTime.Now, userId, description);
        }

        public void UpdateRouteStation(int routeStationId, int stationId, int deliveryTypeId, int departure_sec, int arrival_sec,
            int userId, string description)
        {
            SARouteStation db = new SARouteStation(sqlExec);
            db.UpdateStation(routeStationId, stationId, deliveryTypeId, departure_sec, arrival_sec,
            DateTime.Now, userId, description);
        }

        public void UpdateRouteStation(int routeStationId, int? departureReasonId, int? arrivalReasonId, string description)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("set ");
            if (departureReasonId != null)
                sql.AppendFormat("DepReasonCodeId={0},", departureReasonId);
            if (arrivalReasonId != null)
                sql.AppendFormat("ArrReasonCodeId={0},", arrivalReasonId);
            if (description != null)
                sql.AppendFormat("Description='{0}',", description);
            sql = sql.Remove(sql.Length - 1, 1);
            sql.AppendFormat(" where RouteStationId={0}", routeStationId);
            SARouteStation db = new SARouteStation(sqlExec);
            db.UpdateRow(sql.ToString());
        }

        public void UpdateRoute(int routeId, int? departureReasonId, int? arrivalReasonId, string description)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("set ");
            if (departureReasonId != null)
                sql.AppendFormat("RSCDepReasonCodeId={0},", departureReasonId);
            if (arrivalReasonId != null)
                sql.AppendFormat("RSCArrReasonCodeId={0},", arrivalReasonId);
            if (description != null)
                sql.AppendFormat("Description='{0}',", description);
            sql = sql.Remove(sql.Length - 1, 1);
            sql.AppendFormat(" where RouteId={0}", routeId);

            SARoute db = new SARoute(sqlExec);
            db.UpdateRow(sql.ToString());
        }

        public void DeleteRouteStation(int routeStationId)
        {
            SARouteStation db = new SARouteStation(sqlExec);
            db.DeleteRouteStationById(routeStationId);
        }

        public DataSet GetReport(int depotId, int stationId, int vehicleId, int statusId, DateTime startDate, DateTime endDate)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select g.RSCStationId, s.ScheduleBeginDate, s.ScheduleId, r.RouteId, r.Name, r.VehicleId, ");
            sql.Append("r.RSCDepartureTime_sec, r.RSCArrivalTime_sec, r.RSCActualDep_sec, r.RSCActualArr_sec, ");
            sql.Append("r.RSCDepReasonCodeId, r.RSCArrReasonCodeId, r.RSCDepStatusId, r.RSCArrStatusId, r.Description RouteDescription, ");
            sql.Append("rs.RouteStationId, RS.StationId, RS.DepartureSchedule_sec, rs.DepartureActual_sec, rs.ArrivalSchedule_sec, ");
            sql.Append("rs.ArrivalActual_sec, rs.DepReasonCodeId, rs.ArrReasonCodeId, rs.DepStatusId, rs.ArrStatusId, rs.Description as StationDescription ");
            sql.Append("from saroute r ");
            sql.Append("join saschedule s on s.scheduleid = r.scheduleid ");
            sql.Append("join saScheduleGroup g on s.groupid = g.groupId ");
            sql.Append("join saRouteStation rs on r.routeid = rs.routeId ");
            sql.AppendFormat("where s.scheduleBeginDate between '{0}' and '{1}' ", startDate, endDate);
            if (depotId != 0)
                sql.AppendFormat("and g.RSCStationId = {0} ", depotId);
            if (vehicleId != 0)
                sql.AppendFormat("and r.VehicleId = {0} ", vehicleId);
            if (stationId != 0)
                sql.AppendFormat("and rs.StationId = {0} ", stationId);
            return sqlExec.SQLExecuteDataset(sql.ToString());
        }

        public DataSet GetStArrivalReport(int organizationId, DateTime startDate, DateTime endDate, string routeName, long? vehicleId, int? stationId)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select rs.arrStatusid StatusId, g.RSCStationId DepotId, count(*) StatusCount ");
            sql.Append("from saroute r ");
            sql.Append("join saschedule s on s.scheduleid = r.scheduleid ");
            sql.Append("join saScheduleGroup g on s.groupid = g.groupId ");
            sql.Append("join saStation st on st.stationid = g.RSCStationId ");
            sql.Append("join saRouteStation rs on r.routeid = rs.routeId ");
            sql.Append(string.Format("where st.OrganizationId = {0} and s.scheduleBeginDate between '{1}' and '{2}' ", organizationId, startDate, endDate));
            if (!string.IsNullOrEmpty(routeName))
                sql.AppendFormat("and r.name like '%{0}%' ", routeName);
            if (vehicleId != null && vehicleId.Value != 0)
                sql.AppendFormat("and r.VehicleId = {0} ", vehicleId.Value);
            if (stationId != null && stationId.Value != 0)
                sql.AppendFormat("and rs.StationId = {0} ", stationId.Value);
            sql.Append("group by rs.arrStatusid, g.RSCStationId ");
            return sqlExec.SQLExecuteDataset(sql.ToString());
        }

        public DataSet GetSASetting(int organizationId)
        {
            SASetting db = new SASetting(sqlExec);
            return db.GetRowsByIntField("OrganizationId", organizationId, "GetSASetting");
        }

        public void SaveSASetting(int organizationId, int winBefore, int winAfter,
            int rscDepartEarly, int rscDepartLate, int rscArrivalEarly, int rscArrivalLate,
            int stopDepartEarly, int stopDepartLate, int stopArrivalEarly, int stopArrivalLate)
        {
            DataSet ds = GetSASetting(organizationId);
            SASetting db = new SASetting(sqlExec);
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                db.AddSetting(organizationId, winBefore, winAfter, rscDepartEarly, rscDepartLate, rscArrivalEarly, rscArrivalLate, stopDepartEarly, stopDepartLate, stopArrivalEarly, stopArrivalLate);
            else
                db.UpdateSetting(organizationId, winBefore, winAfter, rscDepartEarly, rscDepartLate, rscArrivalEarly, rscArrivalLate, stopDepartEarly, stopDepartLate, stopArrivalEarly, stopArrivalLate);
        }

        public void DeleteSASetting(int organizationId)
        {
            SASetting db = new SASetting(sqlExec);
            db.DeleteRowsByIntField("OrganizationId", organizationId, "Delete SASetting");
        }

        public void UpdateFileFormat(int organizationId, string format)
        {
            SASetting db = new SASetting(sqlExec);
            db.UpdateFileFormat(organizationId, format);
        }
    }
}
