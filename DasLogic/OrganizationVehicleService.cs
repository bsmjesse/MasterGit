using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.DAS.DB;
using System.Text;

namespace VLF.DAS.Logic
{
   /// <summary>
   /// Vehicle Service Interface
   /// </summary>
   public partial class Organization
   {
      # region Vehicle Service Type 

      /// <summary>
      /// Add new Vehicle Service Type to Organization
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="operationTypeId"></param>
      /// <param name="serviceDescription"></param>
      /// <param name="VRMSCode"></param>
      public void AddVehicleService(int organizationId, short operationTypeId, string serviceDescription, string VRMSCode)
      {
         SqlParameter[] sqlParams = new SqlParameter[4];
         sqlParams[0] = new SqlParameter("@orgId", organizationId);
         //sqlParams[1] = new SqlParameter("@svcId", newServiceId);
         sqlParams[1] = new SqlParameter("@operTypeId", operationTypeId);
         sqlParams[2] = new SqlParameter("@svcDescr", serviceDescription);
         sqlParams[3] = new SqlParameter("@svcCode", VRMSCode);
         svcType.AddRow("(OrganizationId, OperationTypeID, ServiceTypeDescription, VRMSCode) VALUES(@orgId, @operTypeId, @svcDescr, @svcCode)", sqlParams);
      }

      /// <summary>
      /// Update Vehicle Service Type
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="serviceTypeId"></param>
      /// <param name="operationTypeId"></param>
      /// <param name="serviceDescription"></param>
      /// <param name="VRMSCode"></param>
      public void UpdateVehicleService(int organizationId, int serviceTypeId, short operationTypeId, string serviceDescription, string VRMSCode)
      {
         SqlParameter[] sqlParams = new SqlParameter[5];
         sqlParams[0] = new SqlParameter("@orgId", organizationId);
         sqlParams[1] = new SqlParameter("@svcTypeId", serviceTypeId);
         sqlParams[2] = new SqlParameter("@operTypeId", operationTypeId);
         sqlParams[3] = new SqlParameter("@svcDescr", serviceDescription);
         sqlParams[4] = new SqlParameter("@svcCode", VRMSCode);
         svcType.UpdateRow("SET OperationTypeID = @operTypeId, ServiceTypeDescription = @svcDescr, VRMSCode = @svcCode WHERE OrganizationId = @orgId AND ServiceTypeID = @svcTypeId", sqlParams);
      }

      /// <summary>
      /// Delete Vehicle Service Type for Organization
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="serviceTypeId"></param>
      public void DeleteVehicleService(int organizationId, int serviceTypeId)
      {
         svcType.DeleteRowsByFields("OrganizationId", organizationId, "ServiceTypeID", serviceTypeId, "Delete Service Type");
      }

      /// <summary>
      /// Get All Vehicle Services for Organization
      /// </summary>
      /// <param name="organizationId">Organization Id</param>
      /// <returns>DataSet</returns>
      public DataSet GetOrganizationVehicleServices(int organizationId)
      {
         SqlParameter[] sqlParams = new SqlParameter[1];
         sqlParams[0] = new SqlParameter("@orgId", organizationId);
         StringBuilder sql = new StringBuilder("SELECT ServiceTypeID, vlfOrganizationVehicleServiceType.OperationTypeID, OperationTypeDescription, ServiceTypeDescription, VRMSCode FROM vlfOrganizationVehicleServiceType");
         sql.AppendLine();
         sql.AppendLine("INNER JOIN vlfVehicleServiceOperationType ON vlfVehicleServiceOperationType.OperationTypeID = vlfOrganizationVehicleServiceType.OperationTypeID");
         sql.Append("WHERE OrganizationId = @orgId");
         return svcType.GetRowsBySql(sql.ToString(), sqlParams);
      }

      /// <summary>
      /// Get Vehicle Services of selected type for Organization
      /// </summary>
      /// <param name="organizationId">Organization Id</param>
      /// <param name="operationTypeId">Operation Type Id</param>
      /// <returns>DataSet</returns>
      public DataSet GetOrganizationVehicleServices(int organizationId, short operationTypeId)
      {
         SqlParameter[] sqlParams = new SqlParameter[2];
         sqlParams[0] = new SqlParameter("@orgId", organizationId);
         sqlParams[1] = new SqlParameter("@operTypeId", operationTypeId);
         StringBuilder sql = new StringBuilder("SELECT ServiceTypeID, vlfOrganizationVehicleServiceType.OperationTypeID, OperationTypeDescription, ServiceTypeDescription, VRMSCode FROM vlfOrganizationVehicleServiceType");
         sql.AppendLine();
         sql.AppendLine("INNER JOIN vlfVehicleServiceOperationType ON vlfVehicleServiceOperationType.OperationTypeID = vlfOrganizationVehicleServiceType.OperationTypeID");
         sql.Append("WHERE OrganizationId = @orgId AND vlfOrganizationVehicleServiceType.OperationTypeID = @operTypeId");
         return svcType.GetRowsBySql(sql.ToString(), sqlParams);
      }

      /// <summary>
      /// Get Vehicle Service for Organization
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="serviceTypeId"></param>
      public DataSet GetOrganizationVehicleService(int organizationId, int serviceTypeId)
      {
         SqlParameter[] sqlParams = new SqlParameter[2];
         sqlParams[0] = new SqlParameter("@orgId", organizationId);
         sqlParams[1] = new SqlParameter("@svcTypeId", serviceTypeId);
         return svcType.GetRowsByFilter("WHERE OrganizationId = @orgId AND ServiceTypeID = @svcId", sqlParams);
      }

      # endregion

      # region Vehicle Service Notification
      /// <summary>
      /// Add New Notification
      /// </summary>
      /// <param name="organizationId">Organization ID</param>
      /// <param name="operationTypeId">Notification Type ID</param>
      /// <param name="notification1">1 Notification condition</param>
      /// <param name="notification2">2 Notification condition</param>
      /// <param name="notification3">3 Notification condition</param>
      /// <param name="description">Notification description</param>
      public void AddVehicleServiceNotification(int organizationId, short operationTypeId,
         short notification1, short notification2, short notification3, string description)//, string email)
      {
         SqlParameter[] sqlParams = new SqlParameter[6];
         sqlParams[0] = new SqlParameter("@orgId", organizationId);
         sqlParams[1] = new SqlParameter("@operTypeId", operationTypeId);
         sqlParams[2] = new SqlParameter("@notif1", notification1);
         sqlParams[3] = new SqlParameter("@notif2", notification2);
         sqlParams[4] = new SqlParameter("@notif3", notification3);
         sqlParams[5] = new SqlParameter("@descr", description);
         notification.AddRow("(OrganizationId, OperationTypeID, Notification1, Notification2, Notification3, Description) VALUES(@orgId, @operTypeId, @notif1, @notif2, @notif3, @descr)", sqlParams);
      }

      /// <summary>
      /// Update Notification
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="notificationId"></param>
      /// <param name="notificationTypeId"></param>
      /// <param name="notification1"></param>
      /// <param name="notification2"></param>
      /// <param name="notification3"></param>
      /// <param name="description"></param>
      /// <param name="email"></param>
      public void UpdateVehicleServiceNotification(int organizationId, int notificationId, short operationTypeId,
         short notification1, short notification2, short notification3, string description)
      {
         SqlParameter[] sqlParams = new SqlParameter[7];
         sqlParams[0] = new SqlParameter("@orgId", organizationId);
         sqlParams[1] = new SqlParameter("@notifId", notificationId);
         sqlParams[2] = new SqlParameter("@operTypeId", operationTypeId);
         sqlParams[3] = new SqlParameter("@notif1", notification1);
         sqlParams[4] = new SqlParameter("@notif2", notification2);
         sqlParams[5] = new SqlParameter("@notif3", notification3);
         sqlParams[6] = new SqlParameter("@descr", description);
         notification.UpdateRow("SET OperationTypeID = @operTypeId, Notification1 = @notif1, Notification2 = @notif2, Notification3 = @notif3, Description = @descr WHERE OrganizationId = @orgId AND NotificationID = @notifId", sqlParams);
      }

      /// <summary>
      /// Delete Notification
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="notificationId"></param>
      public void DeleteVehicleServiceNotification(int organizationId, int notificationId)
      {
         notification.DeleteRowsByFields("OrganizationId", organizationId, "NotificationID", notificationId, "Delete Notification");
      }

      /// <summary>
      /// Get Organization Notifications
      /// </summary>
      /// <param name="organizationId">Organization Id</param>
      /// <returns>DataSet</returns>
      public DataSet GetOrganizationNotifications(int organizationId)
      {
         StringBuilder sql = new StringBuilder();
         sql.AppendLine("SELECT * FROM vlfOrganizationNotifications");
         sql.AppendLine("INNER JOIN vlfVehicleServiceOperationType ON vlfOrganizationNotifications.OperationTypeID = vlfVehicleServiceOperationType.OperationTypeID");
         sql.AppendLine("WHERE vlfOrganizationNotifications.OrganizationID = @orgId");
         SqlParameter[] sqlParams = new SqlParameter[1];
         sqlParams[0] = new SqlParameter("@orgId", organizationId);
         return notification.GetRowsBySql(sql.ToString(), sqlParams);
      }

      /// <summary>
      /// Get Organization Notifications of selected type
      /// </summary>
      /// <param name="organizationId">Organization Id</param>
      /// <param name="operationTypeId">Notification Id</param>
      /// <returns>DataSet</returns>
      public DataSet GetOrganizationNotifications(int organizationId, short operationTypeId)
      {
         StringBuilder sql = new StringBuilder();
         sql.AppendLine("SELECT OrganizationId, NotificationId, vlfOrganizationNotifications.OperationTypeId, Notification1, Notification2, Notification3, Description, vlfVehicleServiceOperationType.OperationTypeDescription");
         sql.AppendLine("FROM vlfOrganizationNotifications INNER JOIN vlfVehicleServiceOperationType ON vlfOrganizationNotifications.OperationTypeID = vlfVehicleServiceOperationType.OperationTypeID");
         sql.AppendLine("WHERE vlfOrganizationNotifications.OrganizationID = @orgId AND vlfOrganizationNotifications.OperationTypeID = @operTypeId");
         SqlParameter[] sqlParams = new SqlParameter[2];
         sqlParams[0] = new SqlParameter("@orgId", organizationId);
         sqlParams[1] = new SqlParameter("@operTypeId", operationTypeId);
         return notification.GetRowsBySql(sql.ToString(), sqlParams);
      }

      /// <summary>
      /// Get Organization Notification
      /// </summary>
      /// <param name="organizationId">Organization Id</param>
      /// <param name="notificationId">Notification Id</param>
      /// <returns>DataSet</returns>
      public DataSet GetOrganizationNotification(int organizationId, int notificationId)
      {
         SqlParameter[] sqlParams = new SqlParameter[2];
         sqlParams[0] = new SqlParameter("@orgId", organizationId);
         sqlParams[1] = new SqlParameter("@notifId", notificationId);
         return notification.GetRowsByFilter("WHERE OrganizationId = @orgId AND NotificationID = @notifId", sqlParams);
      }
      # endregion
   }
}
