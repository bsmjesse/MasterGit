using System;
using System.Data;			   // for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.Logic
{
    public class Integrations : Das
    {
        private VLF.DAS.DB.Integrations _Integrations = null;

        public Integrations(string connectionString)
            : base(connectionString)
        {
            _Integrations = new VLF.DAS.DB.Integrations(sqlExec);

        }

        /// <summary>
        /// Gets Web Service User by Hash Password
        /// </summary>
        /// <param name="HashPassword"></param>
        /// <returns>UserId</returns>
        public int GetUserByHashPassword(string HashPassword)
        {
            return _Integrations.GetUserByHashPassword(HashPassword);
        }

        /// <summary>
        /// Adds XML Audit Log to table AuditLogXml
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="AuditLogXml"></param>
        /// <returns>New record AuditLogXmlID</returns>
        public int AddAuditLogXml(int UserId, string AuditLogXml, string AuditLogStatus, string AuditLogBatchNumber, int AuditLogRecordsCount, int AuditLogRecordsProcessed, string AuditLogStatusDetails)
        {
            return _Integrations.AddAuditLogXml(UserId, AuditLogXml, AuditLogStatus, AuditLogBatchNumber, AuditLogRecordsCount, AuditLogRecordsProcessed, AuditLogStatusDetails);
        }

        /// <summary>
        /// Gets Integration Web Service settings
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>[TransactionsSubmitted], [RecordsPerBatch], [NumberOfTransactions]</returns>
        public DataSet GetIntegrationWebServiceSettings(int UserId)
        {
            return _Integrations.GetIntegrationWebServiceSettings(UserId);
        }

        /// <summary>
        /// Adds/Updates vehicle with data from WSI web service
        /// </summary>
        /// <param name="Action_Flag"></param>
        /// <param name="Serial_Number"></param>
        /// <param name="Equipment_Yr"></param>
        /// <param name="Equipment_Ds"></param>
        /// <param name="EQP_Weight"></param>
        /// <param name="Fuel_Capacity"></param>
        /// <param name="Avg_Fuel_Burn_Rate"></param>
        /// <param name="EQP_Weight_Unit"></param>
        /// <param name="Create_Date"></param>
        /// <param name="Update_Date"></param>
        /// <param name="Equipment_Id"></param>
        /// <param name="SAP_Equipment_Nr"></param>
        /// <param name="Legacy_Equipment_Nr"></param>
        /// <param name="Object_Type"></param>
        /// <param name="DOT_Nbr"></param>
        /// <param name="EQP_Category"></param>
        /// <param name="EQP_ACQ_DT"></param>
        /// <param name="EQP_Retire_DT"></param>
        /// <param name="SOLD_DT"></param>
        /// <param name="Make"></param>
        /// <param name="Model"></param>
        /// <param name="Project_Nr"></param>
        /// <param name="UserId"></param>
        /// <returns>VehicleId</returns>
        public int UpdateVehicleFromWebService(string Action_Flag, string Serial_Number, Int16 Equipment_Yr, string Equipment_Ds, int EQP_Weight,
            double Fuel_Capacity, double Avg_Fuel_Burn_Rate, string EQP_Weight_Unit, string Create_Date, string Update_Date, string Equipment_Id,
            string SAP_Equipment_Nr, string Legacy_Equipment_Nr, string Object_Type, string DOT_Nbr, string EQP_Category, string EQP_ACQ_DT,
            string EQP_Retire_DT, string SOLD_DT, string Make, string Model, string Project_Nr, string Object_Prefix, string Owning_Dist, 
            double Total_Ctr_Reading, string Total_Ctr_Reading_UOM, string Short_Desc, int UserId)
        {
            return _Integrations.UpdateVehicleFromWebService(Action_Flag, Serial_Number, Equipment_Yr, Equipment_Ds, EQP_Weight, Fuel_Capacity,
                    Avg_Fuel_Burn_Rate, EQP_Weight_Unit, Create_Date, Update_Date, Equipment_Id, SAP_Equipment_Nr, Legacy_Equipment_Nr,
                    Object_Type, DOT_Nbr, EQP_Category, EQP_ACQ_DT, EQP_Retire_DT, SOLD_DT, Make, Model, Project_Nr, Object_Prefix, Owning_Dist,
                    Total_Ctr_Reading, Total_Ctr_Reading_UOM, Short_Desc, UserId);

        }

        /// <summary>
        /// Adds/Updates driver with data from WSI web service
        /// </summary>
        /// <param name="Action_Flag"></param>
        /// <param name="First_NM"></param>
        /// <param name="Last_NM"></param>
        /// <param name="Email_Address"></param>
        /// <param name="Employee_Nr"></param>
        /// <param name="Termination_Dt"></param>
        /// <param name="Position_Ds"></param>
        /// <param name="Create_Date"></param>
        /// <param name="Update_Date"></param>
        /// <param name="Employee_Type"></param>
        /// <param name="UserId"></param>
        /// <returns>DriverId</returns>
        public int UpdateDriverFromWebService(string Action_Flag, string First_NM, string Last_NM, string Email_Address, string Employee_Nr,
            string Termination_Dt, string Position_Ds, string Create_Date, string Update_Date, string Employee_Type, int UserId)
        {
            return _Integrations.UpdateDriverFromWebService(Action_Flag, First_NM, Last_NM, Email_Address, Employee_Nr, Termination_Dt, Position_Ds,
                    Create_Date, Update_Date, Employee_Type, UserId);
        }

        /// <summary>
        /// Adds/Updates hierarchy data from WSI web service
        /// </summary>
        /// <param name="Action_Flag"></param>
        /// <param name="Hierarchy_Nr"></param>
        /// <param name="Hierarchy_Level"></param>
        /// <param name="Parent_Node"></param>
        /// <param name="Record_Source_ID"></param>
        /// <param name="Phone_Nr"></param>
        /// <param name="Project_Equipment_Superintendent"></param>
        /// <param name="Project_Equipment_SuperintendentID"></param>
        /// <param name="Create_Date"></param>
        /// <param name="Update_Date"></param>
        /// <param name="UserId"></param>
        /// <returns>ID</returns>
        public int UpdateHierarchyFromWebService(string Action_Flag, string Hierarchy_Nr, string Hierarchy_Name, int Hierarchy_Level, string Parent_Node, string Record_Source_ID,
            string Phone_Nr, string Project_Equipment_Superintendent, string Project_Equipment_SuperintendentID, string Create_Date, string Update_Date, double Project_Longitude,
            double Project_Latitude, int UserId)
        {
            return _Integrations.UpdateHierarchyFromWebService(Action_Flag, Hierarchy_Nr, Hierarchy_Name, Hierarchy_Level, Parent_Node, Record_Source_ID, Phone_Nr,
                    Project_Equipment_Superintendent, Project_Equipment_SuperintendentID, Create_Date, Update_Date, Project_Longitude, Project_Latitude, UserId);
        }


        /// <summary>
        /// Gets Inspection List for Defined Period Web Service settings
        /// </summary>
        /// <param name="FromDate"></param>
        ///  <param name="ToDate"></param>
        ///  <param name="HasSent"></param>
        /// <returns>[RowID], [InsTime]</returns>
        public DataSet GetInspectionList(int UserId, string FromDate, string ToDate, int HasSent)
        {
            return _Integrations.GetInspectionList(UserId, FromDate, ToDate, HasSent);
        }

        /// <summary>
        /// Gets Inspection Detail for Defined Period and mentioned InspectionIds Web Service settings
        /// </summary>
        ///  <param name="UserId"></param>
        /// <param name="FromDate"></param>
        ///  <param name="ToDate"></param>

        /// <returns>[RowID], [InsTime]</returns>
        public DataSet GetInspectionDetailList(int UserId, string FromDate, string ToDate, string InspectionIds, int HasSent)
        {
            return _Integrations.GetInspectionDetailList(UserId, FromDate, ToDate, InspectionIds, HasSent);
        }

        /// <summary>
        /// Update Inspection status to Sent
        /// </summary>
        ///  <param name="InspectionIds"></param>

        public void UpdateInspectionSentStatus(string InspectionIds)
        {
            _Integrations.UpdateInspectionSentStatus(InspectionIds);
        }

        /// <summary>
        /// Gets Inspection List for Defined Period Web Service settings
        /// </summary>
        ///  <param name="Inspection_id"></param>
        /// <param name="Defect_Id"></param>        
        /// <returns> medias</returns>
        public DataSet GetInspectionDefectImage(int Inspection_id, int Defect_Id)
        {
            return _Integrations.GetInspectionDefectImage(Inspection_id, Defect_Id);
        }

        /// <summary>
        /// Clear Defect status to Sent
        /// </summary>
        ///  <param name="Inspection_id"></param>
        ///  <param name="Defect_Id"></param>
        ///  <param name="ClearedNote"></param>
        ///  <param name="ClearedDate"></param>
        ///  <param name="ClearedDriverId"></param>
        ///  <param name="ClearedDriverName"></param>
        ///  
        public int ClearInspectionDefect(int Inspection_id, int Defect_Id, string ClearedNote, string ClearedDate, string ClearedDriverId, string ClearedDriverName)
        {
            return _Integrations.ClearInspectionDefect(Inspection_id, Defect_Id, ClearedNote, ClearedDate, ClearedDriverId, ClearedDriverName);
        }

        // /// <summary>
        ///// Get KiewitXMLNode By Requesttype
        ///// </summary>
        /////  <param name="RequestType"></param>          
        ///// <returns> Nodes and displaynames</returns>
        //public DataSet GetWebserviceXMLConfiguration(int UserId,string RequestType)
        //{
        //    return _Integrations.GetWebserviceXMLConfiguration(UserId, RequestType);
        //}

        /// <summary>
        /// Gets Configuration detail
        /// </summary>
        ///  <param name="UserId"></param>       
        /// <returns> configuration</returns>
        public DataSet GetvlfOrganizationPushConfiguration(int UserId)
        {
            return _Integrations.GetvlfOrganizationPushConfiguration(UserId);
        }

        /// <summary>
        /// Gets Vehicles for ServiceCloud
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns>[Name],[Vehicle_ID],[Make],[Model],[Class],[Year],[Color],[VIN_Number],[License_Plate],[Vehicle_Type],[ECM],[Account]</returns>
        public DataSet GetVehiclesForServiceCloud(int PageSize, int PageNumber, int OrganizationId)
        {
            return _Integrations.GetVehiclesForServiceCloud(PageSize, PageNumber, OrganizationId);
        }

        /// <summary>
        /// Gets Boxes for ServiceCloud
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns>[BoxId],[Firmware],[Hardware_Name],[Account]</returns>
        public DataSet GetBoxesForServiceCloud(int PageSize, int PageNumber, int OrganizationId)
        {
            return _Integrations.GetBoxesForServiceCloud(PageSize, PageNumber, OrganizationId);
        }

        /// <summary>
        /// Gets Vehicle Assigments for ServiceCloud
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns>[License_Plate],[Vehicle],[Asset],[Assigned_Date]</returns>
        public DataSet GetVehicleAssigmentsForServiceCloud(int PageSize, int PageNumber, int OrganizationId)
        {
            return _Integrations.GetVehicleAssigmentsForServiceCloud(PageSize, PageNumber, OrganizationId);
        }

        /// <summary>
        /// Gets Equipment Hours Details
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="VehicleDesc"></param>
        /// <param name="UserID"></param>
        public DataSet GetHoursDetailsByRegistrationDateRange(string FromDate, string ToDate, string VehicleDesc, int UserID)
        {
            return _Integrations.GetHoursDetailsByRegistrationDateRange(FromDate, ToDate, VehicleDesc, UserID);
        }

        /// <summary>
        /// Gets Equipment Hours
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="VehicleDesc"></param>
        /// <param name="UserID"></param>
        /// <returns>[VehicleId],[VehicleDesc],[VehicleNumber],[VehicleDriver],[HoursReading],[MinRunTime],[MaxRunTime]</returns>
        public DataSet GetHoursByRegistrationDateRange(string FromDate, string ToDate, string VehicleDesc, int UserID)
        {
            return _Integrations.GetHoursByRegistrationDateRange(FromDate, ToDate, VehicleDesc, UserID);
        }

        /// <summary>
        /// Gets Last VAlue of Equipment Hours
        /// </summary>
        /// <param name="VehicleDesc"></param>
        /// <param name="UserID"></param>
        /// <returns>[ID],[VehLastKnownHours],[DeviceOffsetHrs],[GPSTime],[EasternTime]</returns>
        public DataSet GetCurrentHoursByRegistration(string VehicleDesc, int UserID)
        {
            return _Integrations.GetCurrentHoursByRegistration(VehicleDesc, UserID);
        }
    }
}
