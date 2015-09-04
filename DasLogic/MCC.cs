using System;
using System.Data ;			   // for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.Logic
{
    public class MCCManager : Das
    {
        private VLF.DAS.DB.MCC _mcc = null;
        public MCCManager(string connectionString)
            : base(connectionString)
		{
            _mcc = new VLF.DAS.DB.MCC(sqlExec);
        
        }

        /// <summary>
        /// Add Equipment 
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="Description"></param>
        /// <param name="EquipmentTypeId"></param>
        /// <returns></returns>
        /// 
        public int AddMCCGroup(int OrganizationId, string MccName)
        {
            return _mcc.AddMCCGroup(OrganizationId, MccName);
        }

        /// <summary>
        /// Update Equipment
        /// </summary>
        /// <param name="EquipmentId"></param>
        /// <param name="OrganizationId"></param>
        /// <param name="Description"></param>
        /// <param name="EquipmentTypeId"></param>
        /// <returns></returns>
        public int UpdateMCCGroup(int OrganizationId, string MccName, long MccId)
        {
            return _mcc.UpdateMCCGroup(OrganizationId, MccName, MccId);
        }

        /// <summary>
        /// Delete Equipment
        /// </summary>
        /// <param name="EquipmentId"></param>
        /// <returns></returns>
        public int DeleteMCCGroup(long MccId, int OrganizationId, int UserId)
        {
            return _mcc.DeleteMCCGroup(MccId, OrganizationId, UserId);
        }

        /// <summary>
        /// Get Organization Equipments
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationMCCGroup(int OrganizationId)
        {
            return _mcc.GetOrganizationMCCGroup(OrganizationId);
        }

        /// <summary>
        /// MccServiceAssigment_Add
        /// </summary>
        /// <param name="MccId"></param>
        /// <param name="ServiceTypeIDs"></param>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public int MCCMaintenanceAssigment_Add(long MccId, string MaintenanceIds, int OrganizationId, string vehicleIDs)
        {
            return _mcc.MCCMaintenanceAssigment_Add(MccId, MaintenanceIds, OrganizationId, vehicleIDs);
        }

        /// <summary>
        /// MccServiceAssigment_Delete
        /// </summary>
        /// <param name="MccId"></param>
        /// <param name="ServiceTypeIDs"></param>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public int MCCMaintenanceAssigment_Delete(long MccId, string MaintenanceIds, int OrganizationId, int UserID)
        {
            return _mcc.MCCMaintenanceAssigment_Delete(MccId, MaintenanceIds, OrganizationId, UserID);
        }

        /// <summary>
        /// GetMccServiceAssigments
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="MccId"></param>
        /// <returns></returns>
        public DataSet GetMCCMaintenanceAssigment(int? OrganizationId, long? MccId, string operationTypeStr, int UserId)
        {
            return _mcc.GetMCCMaintenanceAssigment(OrganizationId, MccId, operationTypeStr, UserId);
        }

        /// <summary>
        /// GetMccUnAssignedService
        /// </summary>
        /// <param name="MccId"></param>
        /// <returns></returns>
        public DataSet GetMCCMaintenanceUnAssigment(int OrganizationId, long MccId, string operationTypeStr, int UserId)
        {
            return _mcc.GetMCCMaintenanceUnAssigment(OrganizationId, MccId, operationTypeStr, UserId);
        }

        /// <summary>
        /// DeleteMCCMaintenances
        /// </summary>
        /// <param name="MaintenanceId"></param>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public int DeleteMCCMaintenances(long MaintenanceId, int OrganizationId, int UserId)
        {
            return _mcc.DeleteMCCMaintenances(MaintenanceId, OrganizationId, UserId);
        }

        /// <summary>
        /// UpdateMCCMaintenances
        /// </summary>
        /// <param name="MaintenanceId"></param>
        /// <param name="OrganizationId"></param>
        /// <param name="OperationTypeID"></param>
        /// <param name="FrequencyID"></param>
        /// <param name="Interval"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public int UpdateMCCMaintenances(int MaintenanceId, int OrganizationId, int OperationTypeID, int? NotificationTypeID, int FrequencyID, int Interval, string Description, int? TimespanId, Boolean FixedInterval, int userID, string FixedServiceDate, Boolean FixedDate, int FixedDueDate)
        {
            return _mcc.UpdateMCCMaintenances(MaintenanceId, OrganizationId, OperationTypeID, NotificationTypeID, FrequencyID, Interval, Description, TimespanId, FixedInterval, userID, FixedServiceDate, FixedDate, FixedDueDate);
        }


        /// <summary>
        /// AddMCCMaintenances
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="OperationTypeID"></param>
        /// <param name="FrequencyID"></param>
        /// <param name="Interval"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public int AddMCCMaintenances(int OrganizationId, int OperationTypeID, int? NotificationTypeID, int FrequencyID, int Interval, string Description, int? TimespanId, Boolean FixedInterval, int userID, string FixedServiceDate, Boolean FixedDate)
        {
            return _mcc.AddMCCMaintenances(OrganizationId, OperationTypeID, NotificationTypeID, FrequencyID, Interval, Description, TimespanId, FixedInterval, userID, FixedServiceDate, FixedDate);
        }

        /// <summary>
        /// GetMCCMaintenances
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="operationTypeStr"></param>
        /// <returns></returns>
        public DataSet GetMCCMaintenances(int OrganizationId, string operationTypeStr, int userID)
        {
            return _mcc.GetMCCMaintenances(OrganizationId, operationTypeStr, userID);

        }


        /// <summary>
        /// AddMCCNotificationType
        /// </summary>
        /// <param name="OrganizationID"></param>
        /// <param name="OperationTypeID"></param>
        /// <param name="Notification1"></param>
        /// <param name="Notification2"></param>
        /// <param name="Notification3"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public int AddMCCNotificationType(int OrganizationID, int OperationTypeID, int Notification1,
                   int Notification2, int Notification3, string Description)
        {
            return _mcc.AddMCCNotificationType(OrganizationID, OperationTypeID, Notification1,
                   Notification2, Notification3, Description);
        }

        /// <summary>
        /// DeleteMCCNotificationType
        /// </summary>
        /// <param name="OrganizationID"></param>
        /// <param name="NotificationID"></param>
        /// <returns></returns>
        public int DeleteMCCNotificationType(int OrganizationID, int NotificationID)
        {
            return _mcc.DeleteMCCNotificationType(OrganizationID, NotificationID);
        }

        /// <summary>
        /// UpdateMCCNotificationType
        /// </summary>
        /// <param name="OrganizationID"></param>
        /// <param name="NotificationID"></param>
        /// <param name="OperationTypeID"></param>
        /// <param name="Notification1"></param>
        /// <param name="Notification2"></param>
        /// <param name="Notification3"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public int UpdateMCCNotificationType(int OrganizationID, int NotificationID, int OperationTypeID, int Notification1,
                   int Notification2, int Notification3, string Description)
        {
            return _mcc.UpdateMCCNotificationType(OrganizationID, NotificationID, OperationTypeID, Notification1,
                   Notification2, Notification3, Description);
        }

        /// <summary>
        /// GetMCCNotificationType
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="OperationTypeID"></param>
        /// <param name="NotificationID"></param>
        /// <param name="operationTypeStr"></param>
        /// <returns></returns>
        public DataSet GetMCCNotificationType(int OrganizationId, int? OperationTypeID, int? NotificationID, string operationTypeStr)
        {
            return _mcc.GetMCCNotificationType(OrganizationId, OperationTypeID, NotificationID, operationTypeStr);

        }





        /// <summary>
        /// Get Vehicle Maintenance Services
        /// </summary>
        /// <param name="FleetId"></param>
        /// <param name="VehicleId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public DataSet MaintenanceGetVehicleServices(int FleetId, long VehicleId, int UserId)
        {
            return _mcc.MaintenanceGetVehicleServices(FleetId, VehicleId, UserId);
        }



        /// <summary>
        /// Get Vehicle Maintenance Report
        /// </summary>
        /// <param name="FleetId"></param>
        /// <param name="VehicleId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public DataSet MaintenanceVehicleReport(int FleetId, int UserId)
        {
            return _mcc.MaintenanceVehicleReport(FleetId, UserId);
        }


        /// <summary>
        /// GetMCCMaintenanceByMccIdandVehicleID
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="MccId"></param>
        /// <param name="operationTypeStr"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public DataSet GetMCCMaintenanceByMccIdandVehicleID(int OrganizationId, long MccId, string operationTypeStr, string vehicleIds)
        {
            return _mcc.GetMCCMaintenanceByMccIdandVehicleID(OrganizationId, MccId, operationTypeStr, vehicleIds);
        }

        /// <summary>
        /// VehicleMaintenanceAssignment
        /// </summary>
        /// <param name="VehiclesList"></param>
        /// <param name="MaintenancesList"></param>
        /// <param name="MccId"></param>
        /// <returns></returns>
        public int VehicleMaintenanceAssignment(string VehiclesList, string MaintenancesList, int MccId)
        {
            return _mcc.VehicleMaintenanceAssignment(VehiclesList, MaintenancesList, MccId);
        }




        /// <summary>
        /// MaintenanceGetVehicleServices
        /// </summary>
        /// <param name="VehiclesList"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public DataSet MaintenanceGetVehicleServices(string VehiclesList, int UserId)
        {
            return _mcc.MaintenanceGetVehicleServices(VehiclesList, UserId);
        }


        /// <summary>
        /// MaintenanceGetVehicleServices_DashBoard
        /// </summary>
        /// <param name="VehiclesList"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public DataSet MaintenanceGetVehicleServices_DashBoard( int UserId,int FleetId)
        {
            return _mcc.MaintenanceGetVehicleServices_DashBoard(UserId, FleetId);
        }

        /// <summary>
        /// MaintenanceClose
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="MaintenanceID"></param>
        /// <param name="ServiceValue"></param>
        /// <returns></returns>
        public int MaintenanceClose(int UserId, long VehicleId, int MaintenanceID, int ServiceValue, DateTime MaintenanceDateTime, Int64 MccId, int dueValue)
        {
            return _mcc.MaintenanceClose(UserId, VehicleId, MaintenanceID, ServiceValue, MaintenanceDateTime, MccId, dueValue);
        }

        /// <summary>
        /// GetTimespanConventionTypes()
        /// </summary>
        /// <returns></returns>
        public DataSet GetTimespanConventionTypes()
        {
            return _mcc.GetTimespanConventionTypes();
        }

        /// <summary>
        /// GetMaintenanceFrequency
        /// </summary>
        /// <returns></returns>
        public DataSet GetMaintenanceFrequency()
        {
            return _mcc.GetMaintenanceFrequency();
        }

        /// <summary>
        /// GetMCCMaintenanceByMccId
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="MccId"></param>
        /// <param name="operationTypeStr"></param>
        /// <returns></returns>
        public DataSet GetMCCMaintenanceByMccId(int OrganizationId, long MccId, string operationTypeStr, int UserId)
        {
            return _mcc.GetMCCMaintenanceByMccId(OrganizationId, MccId, operationTypeStr, UserId);
        }

        /// <summary>
        /// GetVehicleAssignedMCC
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="maintenanceIds"></param>
        /// <returns></returns>
        public DataSet GetVehicleAssignedMCC(int fleetId, string maintenanceIds, long MccId)
        {
            return _mcc.GetVehicleAssignedMCC(fleetId, maintenanceIds, MccId);
        }

        public DataSet GetVehicleAssignedMCCMultiFleet(string fleetId, string maintenanceIds, long MccId)
        {
            return _mcc.GetVehicleAssignedMCCMultiFleet(fleetId, maintenanceIds, MccId);
        }
        /// <summary>
        /// GetVehicleUnAssignedMCC
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="maintenanceIds"></param>
        /// <returns></returns>
        public DataSet GetVehicleUnAssignedMCC(int fleetId, string maintenanceIds, long MccId)
        {
            return _mcc.GetVehicleUnAssignedMCC(fleetId, maintenanceIds, MccId);
        }

        public DataSet GetVehicleUnAssignedMCCMultiFleet(string fleetId, string maintenanceIds, long MccId)
        {
            return _mcc.GetVehicleUnAssignedMCCMultiFleet(fleetId, maintenanceIds, MccId);
        }

        public int MaintenanceVehiclesUnAssignment(string VehiclesList, string MaintenanceList, int UserID, long MccId)
        {
            return _mcc.MaintenanceVehiclesUnAssignment(VehiclesList, MaintenanceList, UserID, MccId);
        }

        /// <summary>
        /// MaintenanceGetVehicleServicesHistory
        /// </summary>
        /// <param name="VehiclesList"></param>
        /// <returns></returns>
        public DataSet MaintenanceGetVehicleServicesHistory(string VehiclesList, int UserId, DateTime BeginDate)
        {
            return _mcc.MaintenanceGetVehicleServicesHistory(VehiclesList, UserId, BeginDate);
        }

        /// <summary>
        /// VehicleMaintenanceUpdateComment
        /// </summary>
        /// <param name="MaintenanceID"></param>
        /// <param name="VehicleId"></param>
        /// <param name="Comments"></param>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public int VehicleMaintenanceUpdateComment(int MaintenanceID, long VehicleId, string Comments, int OrganizationId)
        {
            return _mcc.VehicleMaintenanceUpdateComment(MaintenanceID, VehicleId, Comments, OrganizationId);
        }

        /// <summary>
        /// GetMccGroupByName(string name, int OrganizationId)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetMccGroupByName(string name, int OrganizationId, long? Mccid)
        {
            return _mcc.GetMccGroupByName(name, OrganizationId, Mccid);
        }


        /// <summary>
        /// MaintenanceGetServicesByVehicles
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="MccId"></param>
        /// <returns></returns>
        public DataSet MaintenanceGetServicesByVehicles(int OrganizationId, string VehiclesList, int UserID)
        {
            return _mcc.MaintenanceGetServicesByVehicles(OrganizationId, VehiclesList, UserID);
        }

        /// <summary>
        /// MaintenanceGetVehiclesByMaintenanceId
        /// </summary>
        /// <param name="MaintenanceId"></param>
        /// <returns></returns>
        public DataSet MaintenanceGetVehiclesByMaintenanceId(int MaintenanceId)
        {
            return _mcc.MaintenanceGetVehiclesByMaintenanceId(MaintenanceId);
        }

        public DataSet MaintenanceGetMccGroupByMaintenanceId(int MaintenanceId,int OrganizationId )
        {
            return _mcc.MaintenanceGetMccGroupByMaintenanceId(MaintenanceId, OrganizationId);
        }
        
        public DataSet MaintenanceGetVehiclesByMccId(int MccId)
        {
            return _mcc.MaintenanceGetVehiclesByMccId(MccId);
        }

        public DataSet MaintenanceGetUnAssignedVehiclesByMccIDAndMaintenanceId(long MccId, string MaintenanceIds)
        {
            return _mcc.MaintenanceGetUnAssignedVehiclesByMccIDAndMaintenanceId(MccId, MaintenanceIds);
        }

        public DataSet MaintenanceGetAssignedVehiclesByMccIDAndMaintenanceId(long MccId, string MaintenanceIds)
        {
            return _mcc.MaintenanceGetAssignedVehiclesByMccIDAndMaintenanceId(MccId, MaintenanceIds);
        }


        public DataSet MaintenanceBoxUserSettings_Get(int FleetId)
        {
            return _mcc.MaintenanceBoxUserSettings_Get(FleetId);
        }

        public DataSet MaintenanceGetOperationReport(DateTime dt, int OrganizationId)
        {
            return _mcc.MaintenanceGetOperationReport(dt, OrganizationId);
        }

        public DataSet Maintenance_GetVehiclesEngineHours(string VehiclesList)
        {
            return _mcc.Maintenance_GetVehiclesEngineHours(VehiclesList);            
        }

        public int Maintenance_UpdateEngineHours(long VehicleId, int CurrentEngineHours, int UserId)
        {
            return _mcc.Maintenance_UpdateEngineHours(VehicleId, CurrentEngineHours, UserId);            
        }
    }
}
