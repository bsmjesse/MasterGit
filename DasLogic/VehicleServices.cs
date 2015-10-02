using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using VLF.ERR;
using VLF.DAS.DB;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.Logic
{
   /// <summary>
   /// Vehicle Services Interface
   /// </summary>
   public partial class Vehicle
   {
      /// <summary>
      /// Vehicle Service Status Type
      /// </summary>
      public enum VehicleServiceStatusType : int
      {
         New = 0,
         PreNotification1 = 1,
         PreNotification2 = 2,
         PreNotification3 = 3,
         Overdue = 4,
         Closed = 5

      }

      /// <summary>
      /// Add new Vehicle Service
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="serviceId"></param>
      /// <param name="operationTypeId"></param>
      /// <param name="notificationId"></param>
      /// <param name="frequency"></param>
      /// <param name="serviceValue"></param>
      /// <param name="serviceInterval"></param>
      /// <param name="endValue"></param>
      /// <param name="email"></param>
      /// <param name="description"></param>
      /// <returns></returns>
      public int AddVehicleService(int userId, long vehicleId, int serviceId, short operationTypeId, int notificationId,
         short frequency, int serviceValue, int serviceInterval, int endValue, string email, string description, string comments)
      {
         //int rowsAdded = 0;
         if (String.IsNullOrEmpty(description))
            return -1;
         if (String.IsNullOrEmpty(email))
            return -1;
         if (vehicleId < 0 || serviceId < 0 || operationTypeId < 0 || notificationId < 0 || frequency < 0 || serviceValue < 0)
            return - 1;
         SqlParameter[] sqlParams = new SqlParameter[13];
         sqlParams[0] = new SqlParameter("@userId", userId);
         sqlParams[1] = new SqlParameter("@vehicleId", vehicleId);
         sqlParams[2] = new SqlParameter("@svcId", serviceId);
         sqlParams[3] = new SqlParameter("@operId", operationTypeId);
         sqlParams[4] = new SqlParameter("@notifId", notificationId);
         sqlParams[5] = new SqlParameter("@statusId", Convert.ToInt16(0)); // new
         sqlParams[6] = new SqlParameter("@frequency", frequency);
         sqlParams[7] = new SqlParameter("@svcValue", serviceValue);
         sqlParams[8] = new SqlParameter("@svcInterval", serviceInterval);
         sqlParams[9] = new SqlParameter("@endValue", endValue);
         sqlParams[10] = new SqlParameter("@descr", description);
         sqlParams[11] = new SqlParameter("@email", email);
         sqlParams[12] = new SqlParameter("@comments", comments);
         //
         //string sql = "(VehicleId, ServiceID, OperationTypeID, NotificationID, StatusID, FrequencyID, DueServiceValue, ServiceInterval, EndServiceValue, ServiceDescription, Email) VALUES(@vehicleId, @svcId, @operId, @notifId, @statusId, @frequency, @svcValue, @svcInterval, @endValue, @descr, @email)";
         //rowsAdded = AddVehicleServiceToHistory(vehicleId, serviceId, DateTime.UtcNow, serviceValue, "New service created");
         return this.sqlExec.SPExecuteNonQuery("VehicleServiceAdd", sqlParams);
      }


       public int AddServiceToFleet(int userId, int fleetId, int serviceId, short operationTypeId, int notificationId,
        short frequency,int serviceValue,  int serviceInterval, int endValue, string email, string description, string comments)
       {
           //int rowsAdded = 0;
           if (String.IsNullOrEmpty(description))
               return -1;
           if (String.IsNullOrEmpty(email))
               return -1;
           if ( serviceId < 0 || operationTypeId < 0 || notificationId < 0 || frequency < 0 )
               return -1;
           SqlParameter[] sqlParams = new SqlParameter[13];
           sqlParams[0] = new SqlParameter("@userId", userId);
           sqlParams[1] = new SqlParameter("@fleetId", fleetId);
           sqlParams[2] = new SqlParameter("@svcId", serviceId);
           sqlParams[3] = new SqlParameter("@operId", operationTypeId);
           sqlParams[4] = new SqlParameter("@notifId", notificationId);
           sqlParams[5] = new SqlParameter("@statusId", Convert.ToInt16(0)); // new
           sqlParams[6] = new SqlParameter("@frequency", frequency);
           sqlParams[7] = new SqlParameter("@svcValue", serviceValue);
           sqlParams[8] = new SqlParameter("@svcInterval", serviceInterval);
           sqlParams[9] = new SqlParameter("@endValue", endValue);
           sqlParams[10] = new SqlParameter("@descr", description);
           sqlParams[11] = new SqlParameter("@email", email);
           sqlParams[12] = new SqlParameter("@comments", comments);
           //
           //string sql = "(VehicleId, ServiceID, OperationTypeID, NotificationID, StatusID, FrequencyID, DueServiceValue, ServiceInterval, EndServiceValue, ServiceDescription, Email) VALUES(@vehicleId, @svcId, @operId, @notifId, @statusId, @frequency, @svcValue, @svcInterval, @endValue, @descr, @email)";
           //rowsAdded = AddVehicleServiceToHistory(vehicleId, serviceId, DateTime.UtcNow, serviceValue, "New service created");
           return this.sqlExec.SPExecuteNonQuery("FleetServiceAdd", sqlParams);
       }

      /// <summary>
      /// Update Vehicle Service
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="serviceId"></param>
      /// <param name="operationTypeId"></param>
      /// <param name="notificationId"></param>
      /// <param name="frequency"></param>
      /// <param name="serviceValue"></param>
      /// <param name="serviceInterval"></param>
      /// <param name="endValue"></param>
      /// <param name="email"></param>
      /// <param name="description"></param>
      /// <returns></returns>
      public int UpdateVehicleService(int userId, int serviceId, long vehicleId, int serviceTypeId, short operationTypeId, int notificationId,
         short frequency, int serviceValue, int serviceInterval, int endValue, string email, string description, string comments)
      {
         //int rowsAdded = 0;
         if (String.IsNullOrEmpty(description))
            return -1;
         if (String.IsNullOrEmpty(email))
            return -1;
         if (vehicleId < 0 || serviceId < 0 || operationTypeId < 0 || notificationId < 0 || frequency < 0 || serviceValue < 0)
            return -1;
         SqlParameter[] sqlParams = new SqlParameter[13];
         sqlParams[0] = new SqlParameter("@userId", userId);
         sqlParams[1] = new SqlParameter("@svcId", serviceId);
         sqlParams[2] = new SqlParameter("@vehicleId", vehicleId);
         sqlParams[3] = new SqlParameter("@svcTypeId", serviceTypeId);
         sqlParams[4] = new SqlParameter("@operId", operationTypeId);
         sqlParams[5] = new SqlParameter("@notifId", notificationId);
         //sqlParams[4] = new SqlParameter("@statusId", 0); // new
         sqlParams[6] = new SqlParameter("@frequency", frequency);
         sqlParams[7] = new SqlParameter("@svcValue", serviceValue);
         sqlParams[8] = new SqlParameter("@svcInterval", serviceInterval);
         sqlParams[9] = new SqlParameter("@endValue", endValue);
         sqlParams[10] = new SqlParameter("@descr", description);
         sqlParams[11] = new SqlParameter("@email", email);
         sqlParams[12] = new SqlParameter("@comments", comments);
         //string sql = "(VehicleId, ServiceID, OperationTypeID, NotificationID, StatusID, FrequencyID, DueServiceValue, ServiceInterval, EndServiceValue, ServiceDescription, Email) VALUES(@vehicleId, @svcId, @operId, @notifId, @statusId, @frequency, @svcValue, @svcInterval, @endValue, @descr, @email)";
         //rowsAdded = AddVehicleServiceToHistory(vehicleId, serviceId, DateTime.UtcNow, serviceValue, "New service created");
         return this.sqlExec.SPExecuteNonQuery("VehicleServiceUpdate", sqlParams);
      }

      public int UpdateVehicleServiceStatus(int serviceId, int statusId)
      {
         return sqlExec.SQLExecuteNonQuery(string.Format("UPDATE vlfVehicleServices SET StatusID = {0} WHERE ServiceID = {1}", 
                                    statusId, serviceId ));
      }

      /// <summary>
      /// Delete all Vehicle Services
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="vehicleId"></param>
      /// <returns></returns>
      public int DeleteVehicleServices(int userId, long vehicleId)
      {
         SqlParameter[] sqlParams = new SqlParameter[2];
         sqlParams[0] = new SqlParameter("@userId", userId);
         sqlParams[1] = new SqlParameter("@vehicleId", vehicleId);
         return this.sqlExec.SPExecuteNonQuery("VehicleServicesDelete", sqlParams);
      }

      /// <summary>
      /// Delete Vehicle Service
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="serviceId"></param>
      /// <returns></returns>
      public int DeleteVehicleService(int userId, long vehicleId, int serviceId)
      {
         SqlParameter[] sqlParams = new SqlParameter[3];
         sqlParams[0] = new SqlParameter("@userId", userId);
         sqlParams[1] = new SqlParameter("@vehicleId", vehicleId);
         sqlParams[2] = new SqlParameter("@serviceId", serviceId);
         return this.sqlExec.SPExecuteNonQuery("VehicleServiceDelete", sqlParams);
      }

      /// <summary>
      /// Close Vehicle Service
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="serviceId"></param>
      /// <returns></returns>
      public int CloseVehicleService(int userId, long vehicleId, int serviceId)
      {
         SqlParameter[] sqlParams = new SqlParameter[3];
         sqlParams[0] = new SqlParameter("@userId", userId);
         sqlParams[1] = new SqlParameter("@vehicleId", vehicleId);
         sqlParams[2] = new SqlParameter("@serviceId", serviceId);
         return this.sqlExec.SPExecuteNonQuery("VehicleServiceClose", sqlParams);
      }

       /// <summary>
      /// Close Vehicle Service Extended
       /// </summary>
       /// <param name="userId"></param>
       /// <param name="vehicleId"></param>
       /// <param name="serviceId"></param>
       /// <param name="serviceValue"></param>
       /// <param name="closeType"></param>
       /// <param name="comments"></param>
       /// <returns></returns>
       public int CloseVehicleService_Extended(int userId, long vehicleId, int serviceId, int serviceValue, Int16 closeType, string comments)
       {
           SqlParameter[] sqlParams = new SqlParameter[6];
           sqlParams[0] = new SqlParameter("@userId", userId);
           sqlParams[1] = new SqlParameter("@vehicleId", vehicleId);
           sqlParams[2] = new SqlParameter("@serviceId", serviceId);
           sqlParams[3] = new SqlParameter("@serviceValue", serviceValue);
           sqlParams[4] = new SqlParameter("@closeType", closeType);
           sqlParams[5] = new SqlParameter("@comments", comments);
           return this.sqlExec.SPExecuteNonQuery("VehicleServiceClose_Extended", sqlParams);
       }

      /// <summary>
      /// Get Operation Types
      /// </summary>
      /// <returns></returns>
      public DataSet GetOperationTypes()
      {
         return this.sqlExec.SQLExecuteDataset("SELECT * FROM vlfVehicleServiceOperationType");
      }

      /// <summary>
      /// Get all Vehicle Services
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="servicesFlag">Flag: 0, 1, 2</param>
      /// <returns></returns>
      public DataTable GetVehicleServices(long vehicleId, short servicesFlag)
      {
         SqlParameter[] sqlParams = new SqlParameter[2];
         sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);
         sqlParams[1] = new SqlParameter("@flag", servicesFlag);
         return this.sqlExec.SPExecuteDataTable("VehicleGetAllServices", "VehicleServices", sqlParams);
         //return vehicleServices.GetRowsByFilter("WHERE VehicleId = @vehicleId", sqlParams).Tables[0];
      }


      /// <summary>
      /// Get all Vehicle Services
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="servicesFlag">Flag: 0, 1, 2</param>
      /// <returns></returns>
      public DataTable GetVehicleServices(long vehicleId, short servicesFlag,int userId)
      {
          SqlParameter[] sqlParams = new SqlParameter[3];
          sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);
          sqlParams[1] = new SqlParameter("@flag", servicesFlag);
          sqlParams[2] = new SqlParameter("@userId", userId);
          return this.sqlExec.SPExecuteDataTable("VehicleGetAllServices_New", "VehicleServices", sqlParams);
          //return vehicleServices.GetRowsByFilter("WHERE VehicleId = @vehicleId", sqlParams).Tables[0];
      }

      
      /// <summary>
      /// Get Vehicle Services by type
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="operTypeId">Odometer - 1, Eng. hrs - 2</param>
      /// <param name="servicesFlag">Flag: 0, 1, 2</param>
      /// <returns></returns>
      public DataTable GetVehicleServicebyTypeId(long vehicleId, short operTypeId, short servicesFlag)
      {
         SqlParameter[] sqlParams = new SqlParameter[3];
         sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);
         sqlParams[1] = new SqlParameter("@operTypeId", operTypeId);
         sqlParams[2] = new SqlParameter("@flag", servicesFlag);
         return this.sqlExec.SPExecuteDataTable("VehicleGetServices", "VehicleServices", sqlParams);
         //return vehicleServices.GetRowsByFilter("WHERE VehicleId = @vehicleId AND OperationTypeID = @operTypeId", sqlParams);
      }


      /// <summary>
      /// Get Vehicle Services by type
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="operTypeId">Odometer - 1, Eng. hrs - 2</param>
      /// <param name="servicesFlag">Flag: 0, 1, 2</param>
      /// <returns></returns>
      public DataTable GetVehicleServicebyTypeId(long vehicleId, short operTypeId, short servicesFlag,int userId)
      {
          SqlParameter[] sqlParams = new SqlParameter[4];
          sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);
          sqlParams[1] = new SqlParameter("@operTypeId", operTypeId);
          sqlParams[2] = new SqlParameter("@flag", servicesFlag);
          sqlParams[3] = new SqlParameter("@userId", userId);
          return this.sqlExec.SPExecuteDataTable("VehicleGetServices_New", "VehicleServices", sqlParams);
          //return vehicleServices.GetRowsByFilter("WHERE VehicleId = @vehicleId AND OperationTypeID = @operTypeId", sqlParams);
      }

      /// <summary>
      /// Get Vehicle Services by type
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="operTypeId">Odometer - 1, Eng. hrs - 2</param>
      /// <param name="servicesFlag">Flag: 0, 1, 2</param>
      /// <returns></returns>
      public DataTable GetVehicleServices(long vehicleId, short operTypeId, short servicesFlag,int userId)
      {
          SqlParameter[] sqlParams = new SqlParameter[4];
          sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);
          sqlParams[1] = new SqlParameter("@operTypeId", operTypeId);
          sqlParams[2] = new SqlParameter("@flag", servicesFlag);
          sqlParams[2] = new SqlParameter("@userId", userId);
          return this.sqlExec.SPExecuteDataTable("VehicleGetServices", "VehicleServices", sqlParams);
          //return vehicleServices.GetRowsByFilter("WHERE VehicleId = @vehicleId AND OperationTypeID = @operTypeId", sqlParams);
      }

      /// <summary>
      /// Get Vehicle Service
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="serviceId"></param>
      /// <returns></returns>
      public DataSet GetVehicleService(int serviceId)
      {
         SqlParameter[] sqlParams = new SqlParameter[1];
         sqlParams[0] = new SqlParameter("@ServiceId", serviceId);
         return this.sqlExec.SPExecuteDataset("VehicleGetServiceById", sqlParams);
      }

      /// <param name="serviceId"></param>
      /// <returns></returns>
      public DataSet GetVehicleService(long vehicleId, int serviceId)
      {
          SqlParameter[] sqlParams = new SqlParameter[2];
          sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);
          sqlParams[1] = new SqlParameter("@svcId", serviceId);
          return vehicleServices.GetRowsByFilter("WHERE VehicleId = @vehicleId AND ServiceID = @svcId", sqlParams);
      }


      public DataSet GetVehicleCurrentServicesByBoxId(int boxId)
      {
         return vehicleServices.GetVehicleCurrentServices(boxId);
      }

      /// <summary>
      ///         all vehicle Service entries with StatusID != closed && StatusID != overdue
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <returns></returns>
      public DataSet GetVehicleCurrentServices(long vehicleId)
      {
         SqlParameter[] sqlParams = new SqlParameter[1];
         sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);
         return vehicleServices.GetRowsByFilter(
            String.Format("WHERE VehicleId = @vehicleId AND StatusID < {0}", (int)VehicleServiceStatusType.Overdue), sqlParams);
      }

      /// <summary>
      ///         all vehicle Service entries with StatusID == closed
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <returns></returns>
      public DataSet GetVehiclePastServices(long vehicleId)
      {
         SqlParameter[] sqlParams = new SqlParameter[1];
         sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);
         return vehicleServices.GetRowsByFilter(
            String.Format("WHERE VehicleId = @vehicleId AND StatusID >= {0}", (int)VehicleServiceStatusType.Overdue), sqlParams);
      }


      /// <summary>
      /// Add Vehicle Service To History table
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="serviceId"></param>
      /// <param name="serviceDateTime"></param>
      /// <param name="serviceValue"></param>
      /// <param name="description">this has maximum 400 characters</param>
      /// <returns></returns>
      public int AddVehicleServiceToHistory(int userId, long vehicleId, int serviceId, 
                                            int operationTypeId, int serviceTypeId, 
                                            DateTime serviceDateTime, float serviceValue, string description)
      {
         SqlParameter[] sqlParams = new SqlParameter[8];
         sqlParams[0] = new SqlParameter("@userId", userId);
         sqlParams[1] = new SqlParameter("@vehicleId", vehicleId);
         sqlParams[2] = new SqlParameter("@svcId", serviceId);
         sqlParams[3] = new SqlParameter("@operationTypeId", operationTypeId);
         sqlParams[4] = new SqlParameter("@serviceTypeId", serviceTypeId);
         sqlParams[5] = new SqlParameter("@svcDate", serviceDateTime);
         sqlParams[6] = new SqlParameter("@svcValue", serviceValue);
         sqlParams[7] = new SqlParameter("@descr", description.Substring(0, 399));

         string sql = "(VehicleId, ServiceID, OperationTypeID, ServiceTypeID, ServiceDateTime, UserID, ServiceValue, ServiceDescription) VALUES(@vehicleId, @svcId, @operationTypeId, @serviceTypeId, @svcDate, @userId, @svcValue, @descr)";
         return vehicleServices.AddRow(sql, vehicleServices.HistoryTable, sqlParams);
      }

      /// <summary>
      /// Get Vehicle Services History
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <returns></returns>
      public DataSet GetVehicleServicesHistory(long vehicleId,int userId)
      {
         SqlParameter[] sqlParams = new SqlParameter[2];
         sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);
         sqlParams[1] = new SqlParameter("@userId", userId);
         return this.sqlExec.SPExecuteDataset("VehicleGetServicesHistory_New",  sqlParams);
      }
       /// <summary>
       /// Get DTC Code Descriptions
       /// </summary>
       /// <returns></returns>
       public DataSet GetDTCCodesDescription()
       {
           string sql = "SELECT * from DTCCodes";
           return sqlExec.SQLExecuteDataset(sql);
 
       }

       // Changes for TimeZone Feature start
       /// <summary>
       ///      it returns
       ///      LP, VehicleDescription, Vin#, 
       ///          MaxDateTime, BoxId, Counter, Code1, Code2, Code3, Translation
       /// </summary>
       public DataSet GetJ1708CodesVehicleFleet_NewTZ(int userId, int vehicleId, int fleetId, DateTime from, DateTime to, string lang)
       {
           DB.J1708Codes errCodes = new J1708Codes(sqlExec);
           return errCodes.GetJ1708CodesVehicleFleet_NewTZ(userId, vehicleId, fleetId, from, to, lang);
       }
       // Changes for TimeZone Feature end

       /// <summary>
       ///      it returns
       ///      LP, VehicleDescription, Vin#, 
       ///          MaxDateTime, BoxId, Counter, Code1, Code2, Code3, Translation
       /// </summary>
       public DataSet GetJ1708CodesVehicleFleet(int userId, int vehicleId, int fleetId, DateTime from, DateTime to, string lang)
       {
           DB.J1708Codes errCodes = new J1708Codes(sqlExec);
           return errCodes.GetJ1708CodesVehicleFleet(userId, vehicleId, fleetId, from, to, lang);
       }
   }
}