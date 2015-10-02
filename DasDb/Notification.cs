using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;	// for SqlException
using System.Data;			// for DataSet
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;

namespace VLF.DAS.DB
{
    /// <summary>
    ///      see the notifications history (description)
    /// </summary>
    public class NotificationHistory : TblGenInterfaces
    {
        public NotificationHistory(SQLExecuter sqlExec)
            : base("vlfNotificationsHistory", sqlExec)
        {
        }

        /// <summary>
        ///      moving the notification in history 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dateTimeCreated"></param>
        /// <param name="boxID"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="data"></param>
        /// <param name="userId"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int AddNotification(Enums.NotificationType type,
                                  DateTime dateTimeCreated,
                                  int boxID,
                                  double latitude,
                                  double longitude,
                                  string data,
                                  int userId,
                                  string reason)
        {

            int rowsAffected = 0;
            string prefixMsg = "Unable to add new vehicle notification:" + type.ToString() + "for boxId: " + boxID.ToString();
            Util.BTrace(Util.INF0, "NotificationHistory.AddNotification -> type={0} dt={1} bid={2} lat={3} long={4} data = {5} userId={6} reason={7}",
                                          type, dateTimeCreated, boxID, latitude, longitude, data, userId, reason);
            try
            {
                rowsAffected = this.AddRow("(DateTimeCreated, NotificationType, BoxId, Data, Latitude, Longitude, UserId, Reason) VALUES(@dtCreated, @notificationtype, @boxId, @data, @lat, @long, @userId, @reason)",
                                  new SqlParameter("@dtCreated", dateTimeCreated),
                                  new SqlParameter("@notificationtype", (int)type),
                                  new SqlParameter("@boxId", boxID),
                                  new SqlParameter("@data", data),
                                  new SqlParameter("@lat", latitude),
                                  new SqlParameter("@long", longitude),
                                  new SqlParameter("@userId", userId),
                                  new SqlParameter("@reason", reason));

            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public DataSet GetAllNotificationsForFleet(int fleetId, DateTime dtFrom, DateTime dtTo)
        {
            if (fleetId > 0)
            {
                string cond = "";

                if (null == dtFrom && null == dtTo)
                    cond = string.Format("where FleetId = {0}", fleetId);
                else if (null == dtFrom)
                    cond = string.Format("where FleetId = {0} and DateTimeCreated <='{1}'", fleetId, dtTo);
                else if (null == dtTo)
                    cond = string.Format("where FleetId = {0} and DateTimeCreated >='{1}'", fleetId, dtFrom);
                else
                    cond = string.Format("where FleetId = {0} and DateTimeCreated >='{1}' and DateTimeCreated <='{2}'", fleetId, dtFrom, dtTo);

                string sql = @"select vlfNotificationsHist.BoxId, vlfNotificationsHist.NotificatioType, vlfNotificationsHist.DateTimeCreated, 
                              vlfNotificationsHist.Data, vlfNotificationsHist.Latitude, vlfNotificationsHist.Longitude, vlfNotificationsHist.Address,
                              vlfNotificationsHist.UserId, vlfNotificationsHist.DateTimeClosed, vlfNotificationsHist.Reason
                           from vlfFleetVehicles INNER JOIN vlfVehicleAssignment 
                                ON vlfFleetVehicles.VehicleId = vlfVehicleAssignment.VehicleId
                           INNER JOIN vlfBox ON vlfVehicleAssignment.BoxId = vlfBox.BoxId	
                           INNER JOIN vlfNotificationsHist ON vlfNotificationsHist.BoxId = vlfBox.BoxId " + cond;

                return sqlExec.SQLExecuteDataset(sql);
            }

            return null;
        }

        /// <summary>
        ///         it covers all cases : in, less, gt for the period or no period at all
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public DataSet GetAllNotificationForVehicle(int vid, DateTime dtFrom, DateTime dtTo)
        {
            if (vid > 0)
            {
                if (null == dtFrom && null == dtTo)
                    return GetRowsByFilter("WHERE BoxId IN (Select BoxId from vlfVehicleAssignment WHERE VehicleId = @vId)", new SqlParameter("@vId", vid));
                else if (null == dtFrom)
                    return GetRowsByFilter("WHERE BoxId IN (Select BoxId from vlfVehicleAssignment WHERE VehicleId = @vId) AND DateTimeCreated <= @dtTo",
                                new SqlParameter("@vId", vid), new SqlParameter("@dtTo", dtTo));
                else if (null == dtTo)
                    return GetRowsByFilter("WHERE BoxId IN (Select BoxId from vlfVehicleAssignment WHERE VehicleId = @vId)) AND DateTimeCreated >= @dtFrom",
                                new SqlParameter("@vId", vid), new SqlParameter("@dtFrom", dtFrom));
                else
                    return GetRowsByFilter("WHERE BoxId IN (Select BoxId from vlfVehicleAssignment WHERE VehicleId = @vId)) AND DateTimeCreated >= @dtFrom AND DateTimeCreated <= @dtTo",
                          new SqlParameter("@vId", vid), new SqlParameter("@dtFrom", dtFrom), new SqlParameter("@dtTo", dtTo));
            }
            return null;
        }


        /// <summary>
        ///         it covers all cases : in, less, gt for the period or no period at all
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public DataSet GetAllNotificationsForBoxId(int boxId, DateTime dtFrom, DateTime dtTo)
        {
            if (boxId > 0)
            {
                if (null == dtFrom && null == dtTo)
                    return GetRowsByFilter("WHERE BoxId = @boxId", new SqlParameter("@boxId", boxId));
                else if (null == dtFrom)
                    return GetRowsByFilter("WHERE BoxId = @boxId AND DateTimeCreated <= @dtTo",
                                new SqlParameter("@boxId", boxId), new SqlParameter("@dtTo", dtTo));
                else if (null == dtTo)
                    return GetRowsByFilter("WHERE BoxId = @boxId AND DateTimeCreated >= @dtFrom",
                                new SqlParameter("@boxId", boxId), new SqlParameter("@dtFrom", dtFrom));
                else
                    return GetRowsByFilter("WHERE BoxId = @boxId AND DateTimeCreated >= @dtFrom AND DateTimeCreated <= @dtTo",
                          new SqlParameter("@boxId", boxId), new SqlParameter("@dtFrom", dtFrom), new SqlParameter("@dtTo", dtTo));
            }

            return null;

        }

    }


    /// <summary>
    ///      see the notifications (description)
    /// </summary>
    public class Notification : TblGenInterfaces
    {

        #region Public Interfaces
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public Notification(SQLExecuter sqlExec)
            : base("vlfNotifications", sqlExec)
        {
        }

        /// <summary>
        ///         all add operations for strings have to checked against the maximum number of characters added
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dateTimeCreated"></param>
        /// <param name="boxID"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int AddNotification(Enums.NotificationType type,
                                   DateTime dateTimeCreated,
                                   int boxID,
                                   double latitude,
                                   double longitude,
                                   string data)
        {

            int rowsAffected = 0;
            string prefixMsg = "Unable to add new vehicle notification:" + type.ToString() + "for boxId: " + boxID.ToString();
            Util.BTrace(Util.INF0, "Notification.AddNotification -> type={0} dt={1} bid={2} lat={3} long={4} data = {5}",
                                          type, dateTimeCreated, boxID, latitude, longitude, data);
            try
            {
                rowsAffected = this.AddRow("(DateTimeCreated, NotificationType, BoxId, Data, Latitude, Longitude) VALUES(@dtCreated, @notificationtype, @boxId, @data, @lat, @long)",
                                  new SqlParameter("@dtCreated", dateTimeCreated),
                                  new SqlParameter("@notificationtype", (int)type),
                                  new SqlParameter("@boxId", boxID),
                                  new SqlParameter("@data", data),
                                  new SqlParameter("@lat", latitude),
                                  new SqlParameter("@long", longitude));

            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;

        }

        public void UpdateAddress(long id, string address)
        {
            UpdateRow("SET Address = @address WHERE NotificationId=@nid", new SqlParameter("@address", address), new SqlParameter("@nid", id));
        }

        public bool IsNotificationPresent(int boxId, Enums.NotificationType type)
        {
            return (GetRecordCount(string.Format(" BoxId={0} and NotificationType={1} ", boxId, (int)type)) > 0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public DataSet GetAllNotificationsForFleet(int fleetId, DateTime dtFrom, DateTime dtTo)
        {
            if (fleetId > 0)
            {
                string cond = "";

                if (null == dtFrom && null == dtTo)
                    cond = string.Format("where FleetId = {0}", fleetId);
                else if (null == dtFrom)
                    cond = string.Format("where FleetId = {0} and DateTimeCreated <='{1}'", fleetId, dtTo);
                else if (null == dtTo)
                    cond = string.Format("where FleetId = {0} and DateTimeCreated >='{1}'", fleetId, dtFrom);
                else
                    cond = string.Format("where FleetId = {0} and DateTimeCreated >='{1}' and DateTimeCreated <='{2}'", fleetId, dtFrom, dtTo);

                string sql = @"select vlfNotifications.BoxId, vlfNotifications.NotificatioType, vlfNotifications.DateTimeCreated, 
                              vlfNotifications.Data, vlfNotifications.Latitude, vlfNotifications..Longitude, vlfNotifications.Address
                           from vlfFleetVehicles INNER JOIN vlfVehicleAssignment 
                                ON vlfFleetVehicles.VehicleId = vlfVehicleAssignment.VehicleId
                           INNER JOIN vlfBox with (nolock) ON vlfVehicleAssignment.BoxId = vlfBox.BoxId	
                           INNER JOIN vlfNotifications ON vlfNotifications.BoxId = vlfBox.BoxId " + cond;

                return sqlExec.SQLExecuteDataset(sql);
            }

            return null;
        }

        /// <summary>
        ///         it covers all cases : in, less, gt for the period or no period at all
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public DataSet GetAllNotificationForVehicle(int vid, DateTime dtFrom, DateTime dtTo)
        {
            if (vid > 0)
            {
                if (null == dtFrom && null == dtTo)
                    return GetRowsByFilter("WHERE BoxId IN (Select BoxId from vlfVehicleAssignment WHERE VehicleId = @vId)", new SqlParameter("@vId", vid));
                else if (null == dtFrom)
                    return GetRowsByFilter("WHERE BoxId IN (Select BoxId from vlfVehicleAssignment WHERE VehicleId = @vId) AND DateTimeCreated <= @dtTo",
                                new SqlParameter("@vId", vid), new SqlParameter("@dtTo", dtTo));
                else if (null == dtTo)
                    return GetRowsByFilter("WHERE BoxId IN (Select BoxId from vlfVehicleAssignment WHERE VehicleId = @vId)) AND DateTimeCreated >= @dtFrom",
                                new SqlParameter("@vId", vid), new SqlParameter("@dtFrom", dtFrom));
                else
                    return GetRowsByFilter("WHERE BoxId IN (Select BoxId from vlfVehicleAssignment WHERE VehicleId = @vId)) AND DateTimeCreated >= @dtFrom AND DateTimeCreated <= @dtTo",
                          new SqlParameter("@vId", vid), new SqlParameter("@dtFrom", dtFrom), new SqlParameter("@dtTo", dtTo));
            }
            return null;
        }


        /// <summary>
        ///         it covers all cases : in, less, gt for the period or no period at all
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public DataSet GetAllNotificationsForBoxId(int boxId, DateTime dtFrom, DateTime dtTo)
        {
            if (boxId > 0)
            {
                if (null == dtFrom && null == dtTo)
                    return GetRowsByFilter("WHERE BoxId = @boxId", new SqlParameter("@boxId", boxId));
                else if (null == dtFrom)
                    return GetRowsByFilter("WHERE BoxId = @boxId AND DateTimeCreated <= @dtTo",
                                new SqlParameter("@boxId", boxId), new SqlParameter("@dtTo", dtTo));
                else if (null == dtTo)
                    return GetRowsByFilter("WHERE BoxId = @boxId AND DateTimeCreated >= @dtFrom",
                                new SqlParameter("@boxId", boxId), new SqlParameter("@dtFrom", dtFrom));
                else
                    return GetRowsByFilter("WHERE BoxId = @boxId AND DateTimeCreated >= @dtFrom AND DateTimeCreated <= @dtTo",
                          new SqlParameter("@boxId", boxId), new SqlParameter("@dtFrom", dtFrom), new SqlParameter("@dtTo", dtTo));
            }

            return null;

        }

        // Changes for TimeZone Feature start
        /// <summary>
        ///         get all notifications for all vehicles visible to an user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public DataSet GetAllNotificationsForUserId_NewTZ(int userId, DateTime dtFrom, DateTime dtTo)
        {

            //                string cond = "";

            //                if (null == dtFrom && null == dtTo)
            //                    cond = string.Format("where FleetId in ( select FleetId from vlfFLeetUsers where Userid = {0})", userId);
            //                else if (null == dtFrom)
            //                    cond = string.Format("where FleetId in ( select FleetId from vlfFLeetUsers where Userid = {0}) and DateTimeCreated <='{1}'", userId, dtTo);
            //                else if (null == dtTo)
            //                    cond = string.Format("where FleetId in ( select FleetId from vlfFLeetUsers where Userid = {0}) and DateTimeCreated >='{1}'", userId, dtFrom);
            //                else
            //                    cond = string.Format("where FleetId in ( select FleetId from vlfFLeetUsers where Userid = {0}) and DateTimeCreated >='{1}' and DateTimeCreated <='{2}'", userId, dtFrom, dtTo);

            //                string sql = string.Format("DECLARE @Timezone int SET @Timezone = ISNULL(dbo.GetTimeZoneDayLight({0}), 0) ", userId) +
            //                        @"select DISTINCT vlfNotifications.NotificationId, vlfNotifications.BoxId, vlfNotifications.NotificationType, DATEADD(hour, @Timezone,vlfNotifications.DateTimeCreated) as DateTimeCreated, 
            //                              vlfNotifications.Data, vlfNotifications.Latitude, vlfNotifications.Longitude, ISNULL(vlfNotifications.Address,'N/A') as Address,vlfVehicleInfo.Description, vlfNotificationType.notificationDescription,vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.VehicleId
            //                           from vlfFleetVehicles INNER JOIN vlfVehicleAssignment 
            //                                ON vlfFleetVehicles.VehicleId = vlfVehicleAssignment.VehicleId
            //                           INNER JOIN vlfVehicleInfo ON vlfVehicleInfo.VehicleId =vlfVehicleAssignment.VehicleId	
            //                           INNER JOIN vlfBox with (nolock) ON vlfVehicleAssignment.BoxId = vlfBox.BoxId	
            //                           INNER JOIN vlfNotifications ON vlfNotifications.BoxId = vlfBox.BoxId 
            //                           INNER JOIN vlfNotificationType ON vlfNotifications.NotificationType = vlfNotificationType.notificationTypeId " + cond + " and vlfNotificationType.notificationTypeId<>1 and vlfNotificationType.notificationTypeId<>10 ORDER BY DateTimeCreated DESC";



            //                return sqlExec.SQLExecuteDataset(sql);


            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "MaintenanceNotificationsNotAcknowledged_NewTimeZone";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@from", SqlDbType.DateTime, dtFrom);
                sqlExec.AddCommandParam("@to", SqlDbType.DateTime, dtTo);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceNotificationsNotAcknowledged id=" + userId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceNotificationsNotAcknowledged id=" + userId.ToString() + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;


        }

        // Changes for TimeZone Feature end

        /// <summary>
        ///         get all notifications for all vehicles visible to an user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public DataSet GetAllNotificationsForUserId(int userId, DateTime dtFrom, DateTime dtTo)
        {

            //                string cond = "";

            //                if (null == dtFrom && null == dtTo)
            //                    cond = string.Format("where FleetId in ( select FleetId from vlfFLeetUsers where Userid = {0})", userId);
            //                else if (null == dtFrom)
            //                    cond = string.Format("where FleetId in ( select FleetId from vlfFLeetUsers where Userid = {0}) and DateTimeCreated <='{1}'", userId, dtTo);
            //                else if (null == dtTo)
            //                    cond = string.Format("where FleetId in ( select FleetId from vlfFLeetUsers where Userid = {0}) and DateTimeCreated >='{1}'", userId, dtFrom);
            //                else
            //                    cond = string.Format("where FleetId in ( select FleetId from vlfFLeetUsers where Userid = {0}) and DateTimeCreated >='{1}' and DateTimeCreated <='{2}'", userId, dtFrom, dtTo);

            //                string sql = string.Format("DECLARE @Timezone int SET @Timezone = ISNULL(dbo.GetTimeZoneDayLight({0}), 0) ", userId) +
            //                        @"select DISTINCT vlfNotifications.NotificationId, vlfNotifications.BoxId, vlfNotifications.NotificationType, DATEADD(hour, @Timezone,vlfNotifications.DateTimeCreated) as DateTimeCreated, 
            //                              vlfNotifications.Data, vlfNotifications.Latitude, vlfNotifications.Longitude, ISNULL(vlfNotifications.Address,'N/A') as Address,vlfVehicleInfo.Description, vlfNotificationType.notificationDescription,vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.VehicleId
            //                           from vlfFleetVehicles INNER JOIN vlfVehicleAssignment 
            //                                ON vlfFleetVehicles.VehicleId = vlfVehicleAssignment.VehicleId
            //                           INNER JOIN vlfVehicleInfo ON vlfVehicleInfo.VehicleId =vlfVehicleAssignment.VehicleId	
            //                           INNER JOIN vlfBox with (nolock) ON vlfVehicleAssignment.BoxId = vlfBox.BoxId	
            //                           INNER JOIN vlfNotifications ON vlfNotifications.BoxId = vlfBox.BoxId 
            //                           INNER JOIN vlfNotificationType ON vlfNotifications.NotificationType = vlfNotificationType.notificationTypeId " + cond + " and vlfNotificationType.notificationTypeId<>1 and vlfNotificationType.notificationTypeId<>10 ORDER BY DateTimeCreated DESC";



            //                return sqlExec.SQLExecuteDataset(sql);


            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "MaintenanceNotificationsNotAcknowledged";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@from", SqlDbType.DateTime, dtFrom);
                sqlExec.AddCommandParam("@to", SqlDbType.DateTime, dtTo);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceNotificationsNotAcknowledged id=" + userId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceNotificationsNotAcknowledged id=" + userId.ToString() + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;


        }


        /// <summary>
        ///      move the record from notifications to history notifications and add the reason
        ///      it is like an audit of who read and acknowledged the notifications
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="userId"></param>
        /// <param name="when"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool AcknowledgeNotification(long notificationId,
                                            int userId,
                                            DateTime when,
                                            string reason)
        {
            string prefixMsg = string.Format(" AcknowledgeNotification -> ID={0} by={1}, {2} {3}", notificationId, userId, when, reason);

            try
            {

                string sql = "AckNotification";
                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@notificationId", SqlDbType.BigInt, notificationId);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@when", SqlDbType.DateTime, when);
                sqlExec.AddCommandParam("@reason", SqlDbType.VarChar, reason, 32);

                if (sqlExec.RequiredTransaction())
                {
                    // 4. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // Executes SQL statement
                sqlExec.SPExecuteNonQuery(sql);

                return true;

            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
                throw new DASException(objException.Message);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return false;
        }



        public bool AckNotificationMaintenance(long notificationId,Int16 typeId,
                                         int userId,
                                         DateTime when,
                                         string reason)
        {
            string prefixMsg = string.Format(" AckNotificationMaintenance -> ID={0} by={1}, {2} {3}", notificationId, userId, when, reason);

            try
            {

                string sql = "AckNotificationMaintenance";
                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@notificationId", SqlDbType.BigInt, notificationId);
                sqlExec.AddCommandParam("@typeId", SqlDbType.Int, typeId);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@when", SqlDbType.DateTime, when);
                sqlExec.AddCommandParam("@reason", SqlDbType.VarChar, reason, 32);

                if (sqlExec.RequiredTransaction())
                {
                    // 4. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // Executes SQL statement
                sqlExec.SPExecuteNonQuery(sql);

                return true;

            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
                throw new DASException(objException.Message);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return false;
        }

        #endregion
    }
}
