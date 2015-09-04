using System;
using System.Collections.Generic;
using System.Text;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using System.Data;
using System.Data.SqlClient;

namespace VLF.DAS.DB
{
    public class Integrations : TblGenInterfaces
    {
        public Integrations(SQLExecuter sqlExec)
            : base("", sqlExec)
        {
        }

        /// <summary>
        /// Gets Web Service User by Hash Password
        /// </summary>
        /// <param name="HashPassword"></param>
        /// <returns>UserId</returns>
        public int GetUserByHashPassword(string HashPassword)
        {
            int UserID = 0;

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@HashPassword", SqlDbType.VarChar, ParameterDirection.Input, HashPassword);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, ParameterDirection.Output, 4, UserID);

                int res = sqlExec.SPExecuteNonQuery("sp_UserByHashPassword_Get");

                UserID = (DBNull.Value == sqlExec.ReadCommandParam("@UserId")) ?
                              UserID : Convert.ToInt32(sqlExec.ReadCommandParam("@UserId"));
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to get user by Hash Password - HashPassword={0}.", HashPassword);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to get user by Hash Password - HashPassword={0}.", HashPassword);
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return UserID;
        }

        /// <summary>
        /// Adds XML Audit Log to table AuditLogXml
        /// </summary>
        /// <param name="OrganizationID"></param>
        /// <param name="AuditLogXml"></param>
        /// <returns>New record AuditLogXmlID</returns>
        public int AddAuditLogXml(int UserId, string AuditLogXml, string AuditLogStatus, string AuditLogBatchNumber, int AuditLogRecordsCount, int AuditLogRecordsProcessed, string AuditLogStatusDetails)
        {
            int AuditLogXmlID = 0;

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, ParameterDirection.Input, UserId);
                sqlExec.AddCommandParam("@AuditLogXml", SqlDbType.VarChar, ParameterDirection.Input, AuditLogXml);
                sqlExec.AddCommandParam("@AuditLogStatus", SqlDbType.VarChar, ParameterDirection.Input, AuditLogStatus);
                sqlExec.AddCommandParam("@AuditLogBatchNumber", SqlDbType.VarChar, ParameterDirection.Input, AuditLogBatchNumber);
                sqlExec.AddCommandParam("@AuditLogRecordsCount", SqlDbType.Int, ParameterDirection.Input, AuditLogRecordsCount);
                sqlExec.AddCommandParam("@AuditLogRecordsProcessed", SqlDbType.Int, ParameterDirection.Input, AuditLogRecordsProcessed);
                sqlExec.AddCommandParam("@AuditLogStatusDetails", SqlDbType.VarChar, ParameterDirection.Input, AuditLogStatusDetails);
                sqlExec.AddCommandParam("@AuditLogXmlID", SqlDbType.Int, ParameterDirection.Output, 4, AuditLogXmlID);

                int res = sqlExec.SPExecuteNonQuery("sp_AuditLogXml_Add");

                AuditLogXmlID = (DBNull.Value == sqlExec.ReadCommandParam("@AuditLogXmlID")) ?
                              AuditLogXmlID : Convert.ToInt32(sqlExec.ReadCommandParam("@AuditLogXmlID"));
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to add XML audit log - UserId={0}.", UserId.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to add XML audit log - UserId={0}.", UserId.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return AuditLogXmlID;
        }

        /// <summary>
        /// Gets Integration Web Service settings
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>[TransactionsSubmitted], [RecordsPerBatch], [NumberOfTransactions]</returns>
        public DataSet GetIntegrationWebServiceSettings(int UserId)
        {
            DataSet resultSet = new DataSet();
            string sql = "sp_IntegrationWebServiceSettings_Get";

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, ParameterDirection.Input, UserId);

                resultSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to get Integration Web Service Settings - UserId={0}.", UserId.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to get Integration Web Service Settings - UserId={0}.", UserId.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return resultSet;
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
            int VehicleId = 0;

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@Action_Flag", SqlDbType.VarChar, ParameterDirection.Input, Action_Flag);
                sqlExec.AddCommandParam("@Serial_Number", SqlDbType.VarChar, ParameterDirection.Input, Serial_Number);
                sqlExec.AddCommandParam("@Equipment_Yr", SqlDbType.SmallInt, ParameterDirection.Input, Equipment_Yr);
                sqlExec.AddCommandParam("@Equipment_Ds", SqlDbType.VarChar, ParameterDirection.Input, Equipment_Ds);
                sqlExec.AddCommandParam("@EQP_Weight", SqlDbType.Int, ParameterDirection.Input, EQP_Weight);
                sqlExec.AddCommandParam("@Fuel_Capacity", SqlDbType.Float, ParameterDirection.Input, Fuel_Capacity);
                sqlExec.AddCommandParam("@Avg_Fuel_Burn_Rate", SqlDbType.Float, ParameterDirection.Input, Avg_Fuel_Burn_Rate);
                sqlExec.AddCommandParam("@EQP_Weight_Unit", SqlDbType.VarChar, ParameterDirection.Input, EQP_Weight_Unit);
                sqlExec.AddCommandParam("@Create_Date", SqlDbType.VarChar, ParameterDirection.Input, Create_Date);
                sqlExec.AddCommandParam("@Update_Date", SqlDbType.VarChar, ParameterDirection.Input, Update_Date);
                sqlExec.AddCommandParam("@Equipment_Id", SqlDbType.VarChar, ParameterDirection.Input, Equipment_Id);
                sqlExec.AddCommandParam("@SAP_Equipment_Nr", SqlDbType.VarChar, ParameterDirection.Input, SAP_Equipment_Nr);
                sqlExec.AddCommandParam("@Legacy_Equipment_Nr", SqlDbType.VarChar, ParameterDirection.Input, Legacy_Equipment_Nr);
                sqlExec.AddCommandParam("@Object_Type", SqlDbType.VarChar, ParameterDirection.Input, Object_Type);
                sqlExec.AddCommandParam("@DOT_Nbr", SqlDbType.VarChar, ParameterDirection.Input, DOT_Nbr);
                sqlExec.AddCommandParam("@EQP_Category", SqlDbType.VarChar, ParameterDirection.Input, EQP_Category);
                sqlExec.AddCommandParam("@EQP_ACQ_DT", SqlDbType.VarChar, ParameterDirection.Input, EQP_ACQ_DT);
                sqlExec.AddCommandParam("@EQP_Retire_DT", SqlDbType.VarChar, ParameterDirection.Input, EQP_Retire_DT);
                sqlExec.AddCommandParam("@SOLD_DT", SqlDbType.VarChar, ParameterDirection.Input, SOLD_DT);
                sqlExec.AddCommandParam("@Make", SqlDbType.VarChar, ParameterDirection.Input, Make);
                sqlExec.AddCommandParam("@Model", SqlDbType.VarChar, ParameterDirection.Input, Model);
                sqlExec.AddCommandParam("@Project_Nr", SqlDbType.VarChar, ParameterDirection.Input, Project_Nr);
                sqlExec.AddCommandParam("@Object_Prefix", SqlDbType.VarChar, ParameterDirection.Input, Object_Prefix);
                sqlExec.AddCommandParam("@Owning_Dist", SqlDbType.VarChar, ParameterDirection.Input, Owning_Dist);
                sqlExec.AddCommandParam("@Total_Ctr_Reading", SqlDbType.Float, ParameterDirection.Input, Total_Ctr_Reading);
	            sqlExec.AddCommandParam("@Total_Ctr_Reading_UOM", SqlDbType.VarChar, ParameterDirection.Input, Total_Ctr_Reading_UOM);
                sqlExec.AddCommandParam("@Short_Desc", SqlDbType.VarChar, ParameterDirection.Input, Short_Desc);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, ParameterDirection.Input, UserId);
                sqlExec.AddCommandParam("@VehicleId", SqlDbType.Int, ParameterDirection.Output, 4, VehicleId);

                int res = sqlExec.SPExecuteNonQuery("sp_VehicleFromWebService_Update");

                VehicleId = (DBNull.Value == sqlExec.ReadCommandParam("@VehicleId")) ?
                              VehicleId : Convert.ToInt32(sqlExec.ReadCommandParam("@VehicleId"));
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to update Equipment from XML - Equipment_Id={0}.", Equipment_Id.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to update Equipment from XML - Equipment_Id={0}.", Equipment_Id.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return VehicleId;
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
            int DriverId = 0;

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@Action_Flag", SqlDbType.VarChar, ParameterDirection.Input, Action_Flag);
                sqlExec.AddCommandParam("@First_NM", SqlDbType.VarChar, ParameterDirection.Input, First_NM);
                sqlExec.AddCommandParam("@Last_NM", SqlDbType.VarChar, ParameterDirection.Input, Last_NM);
                sqlExec.AddCommandParam("@Email_Address", SqlDbType.VarChar, ParameterDirection.Input, Email_Address);
                sqlExec.AddCommandParam("@Employee_Nr", SqlDbType.VarChar, ParameterDirection.Input, Employee_Nr);
                sqlExec.AddCommandParam("@Termination_Dt", SqlDbType.VarChar, ParameterDirection.Input, Termination_Dt);
                sqlExec.AddCommandParam("@Position_Ds", SqlDbType.VarChar, ParameterDirection.Input, Position_Ds);
                sqlExec.AddCommandParam("@Create_Date", SqlDbType.VarChar, ParameterDirection.Input, Create_Date);
                sqlExec.AddCommandParam("@Update_Date", SqlDbType.VarChar, ParameterDirection.Input, Update_Date);
                sqlExec.AddCommandParam("@Employee_Type", SqlDbType.VarChar, ParameterDirection.Input, Employee_Type);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, ParameterDirection.Input, UserId);
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, ParameterDirection.Output, 4, DriverId);

                int res = sqlExec.SPExecuteNonQuery("sp_DriverFromWebService_Update");

                DriverId = (DBNull.Value == sqlExec.ReadCommandParam("@DriverId")) ?
                              DriverId : Convert.ToInt32(sqlExec.ReadCommandParam("@DriverId"));
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to update Driver from XML - Employee_Nr={0}.", Employee_Nr);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to update Driver from XML - Employee_Nr={0}.", Employee_Nr);
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return DriverId;
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
            int ID = 0;

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@Action_Flag", SqlDbType.VarChar, ParameterDirection.Input, Action_Flag);
                sqlExec.AddCommandParam("@Hierarchy_Nr", SqlDbType.VarChar, ParameterDirection.Input, Hierarchy_Nr);
                sqlExec.AddCommandParam("@Hierarchy_Name", SqlDbType.VarChar, ParameterDirection.Input, Hierarchy_Name);
                sqlExec.AddCommandParam("@Hierarchy_Level", SqlDbType.Int, ParameterDirection.Input, Hierarchy_Level);
                sqlExec.AddCommandParam("@Parent_Node", SqlDbType.VarChar, ParameterDirection.Input, Parent_Node);
                sqlExec.AddCommandParam("@Record_Source_ID", SqlDbType.VarChar, ParameterDirection.Input, Record_Source_ID);
                sqlExec.AddCommandParam("@Phone_Nr", SqlDbType.VarChar, ParameterDirection.Input, Phone_Nr);
                sqlExec.AddCommandParam("@Project_Equipment_Superintendent", SqlDbType.VarChar, ParameterDirection.Input, Project_Equipment_Superintendent);
                sqlExec.AddCommandParam("@Project_Equipment_SuperintendentID", SqlDbType.VarChar, ParameterDirection.Input, Project_Equipment_SuperintendentID);
                sqlExec.AddCommandParam("@Create_Date", SqlDbType.VarChar, ParameterDirection.Input, Create_Date);
                sqlExec.AddCommandParam("@Update_Date", SqlDbType.VarChar, ParameterDirection.Input, Update_Date);
                sqlExec.AddCommandParam("@Project_Longitude", SqlDbType.Float, ParameterDirection.Input, Project_Longitude);
                sqlExec.AddCommandParam("@Project_Latitude", SqlDbType.Float, ParameterDirection.Input, Project_Latitude);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, ParameterDirection.Input, UserId);
                sqlExec.AddCommandParam("@ID", SqlDbType.Int, ParameterDirection.Output, 4, ID);

                int res = sqlExec.SPExecuteNonQuery("sp_HierarchyFromWebService_Update");

                ID = (DBNull.Value == sqlExec.ReadCommandParam("@ID")) ?
                              ID : Convert.ToInt32(sqlExec.ReadCommandParam("@ID"));
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to update Hierarchy from XML - Hierarchy_Nr={0}.", Hierarchy_Nr);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to update Hierarchy from XML  - Hierarchy_Nr={0}.", Hierarchy_Nr);
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return ID;
        }


        /// <summary>
        /// Gets Inspection List for Defined Period Web Service settings
        /// </summary>
        ///  <param name="UserId"></param>
        /// <param name="FromDate"></param>
        ///  <param name="ToDate"></param>

        /// <returns>[RowID], [InsTime]</returns>
        public DataSet GetInspectionList(int UserId, string FromDate, string ToDate, int HasSent)
        {
            DataSet resultSet = new DataSet();
            string sql = "usp_hos_GetUnsentInspectionList";

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, ParameterDirection.Input, UserId);
                sqlExec.AddCommandParam("@From", SqlDbType.DateTime, ParameterDirection.Input, FromDate);
                sqlExec.AddCommandParam("@To", SqlDbType.DateTime, ParameterDirection.Input, ToDate);
                if (HasSent == 0 || HasSent == 1)
                {
                    sqlExec.AddCommandParam("@HasSent ", SqlDbType.Bit, ParameterDirection.Input, HasSent);
                }
                else
                {
                    sqlExec.AddCommandParam("@HasSent ", SqlDbType.Bit, ParameterDirection.Input, DBNull.Value);
                }

                resultSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to get Inspections details");
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to get Inspections details");
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return resultSet;
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
            DataSet resultSet = new DataSet();
            string sql = "usp_hos_GetInspectionDetailList";

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, ParameterDirection.Input, UserId);
                try
                {
                    DateTime dtFrom = Convert.ToDateTime(FromDate);
                    DateTime dtTo = Convert.ToDateTime(FromDate);
                    sqlExec.AddCommandParam("@From", SqlDbType.DateTime, ParameterDirection.Input, FromDate);
                    sqlExec.AddCommandParam("@To", SqlDbType.DateTime, ParameterDirection.Input, ToDate);

                }

                catch
                {
                    sqlExec.AddCommandParam("@From", SqlDbType.DateTime, ParameterDirection.Input, DBNull.Value);
                    sqlExec.AddCommandParam("@To", SqlDbType.DateTime, ParameterDirection.Input, DBNull.Value);
                }


                sqlExec.AddCommandParam("@InspectionIds", SqlDbType.Text, ParameterDirection.Input, InspectionIds);
                if (HasSent == 0 || HasSent == 1)
                {
                    sqlExec.AddCommandParam("@HasSent ", SqlDbType.Bit, ParameterDirection.Input, HasSent);
                }
                else
                {
                    sqlExec.AddCommandParam("@HasSent ", SqlDbType.Bit, ParameterDirection.Input, DBNull.Value);
                }
                resultSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to get Inspections details");
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to get Inspections details");
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return resultSet;
        }

        /// <summary>
        /// Update Inspection status to Sent
        /// </summary>
        ///  <param name="InspectionIds"></param>

        public void UpdateInspectionSentStatus(string InspectionIds)
        {
            DataSet resultSet = new DataSet();
            string sql = "usp_hos_UpdateInspectionSentStatus";

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@Inspection_id", SqlDbType.Text, InspectionIds);
                sqlExec.AddCommandParam("@HasSent", SqlDbType.Bit, true);
                int res = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to update inspection status");
                Util.ProcessDbException(prefixMsg, objException);
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to update inspection status");
                throw new DASException(prefixMsg + " " + objException.Message);
            }


        }

        /// <summary>
        /// Gets Inspection List for Defined Period Web Service settings
        /// </summary>
        ///  <param name="Inspection_id"></param>
        /// <param name="Defect_Id"></param>        
        /// <returns> medias</returns>
        public DataSet GetInspectionDefectImage(int Inspection_id, int Defect_Id)
        {
            DataSet resultSet = new DataSet();
            string sql = "usp_hos_GetInspectionDefectImage";

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@Inspection_id", SqlDbType.Int, ParameterDirection.Input, Inspection_id);
                sqlExec.AddCommandParam("@Defect_Id", SqlDbType.Int, ParameterDirection.Input, Defect_Id);

                resultSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to get Inspections Images");
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to get Inspections Images");
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return resultSet;
        }

        /// <summary>
        /// Gets Configuration detail
        /// </summary>
        ///  <param name="UserId"></param>       
        /// <returns> configuration</returns>
        public DataSet GetvlfOrganizationPushConfiguration(int UserId)
        {
            DataSet resultSet = new DataSet();
            string sql = "usp_hos_GetvlfOrganizationPushConfiguration";

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, ParameterDirection.Input, UserId);

                resultSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to get configuration detail");
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to get configuration detail");
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return resultSet;
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
            DataSet resultSet = new DataSet();
            string sql = "usp_hos_ClearInspectionDefect";

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@Inspection_id", SqlDbType.Int, Inspection_id);
                sqlExec.AddCommandParam("@Defect_Id", SqlDbType.Int, Defect_Id);
                sqlExec.AddCommandParam("@ClearedNote", SqlDbType.Text, ClearedNote);
                sqlExec.AddCommandParam("@ClearedDate", SqlDbType.DateTime, ClearedDate);
                sqlExec.AddCommandParam("@ClearedDriverId", SqlDbType.Text, ClearedDriverId);
                sqlExec.AddCommandParam("@ClearedDriverName", SqlDbType.Text, ClearedDriverName);

                int res = sqlExec.SPExecuteNonQuery(sql);
                return res;
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to update inspection status");
                Util.ProcessDbException(prefixMsg, objException);
                throw new DASException(prefixMsg + " " + objException.Message);

            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);

            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to update inspection status");
                throw new DASException(prefixMsg + " " + objException.Message);
            }


        }

        /// <summary>
        /// Gets Vehicles for ServiceCloud
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns>[Name],[Vehicle_ID],[Make],[Model],[Class],[Year],[Color],[VIN_Number],[License_Plate],[Vehicle_Type],[ECM],[Account]</returns>
        public DataSet GetVehiclesForServiceCloud(int PageSize, int PageNumber, int OrganizationId)
        {
            DataSet resultSet = new DataSet();
            string sql = "sp_VehiclesForServiceCloud_Get";

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@PageSize", SqlDbType.Int, ParameterDirection.Input, PageSize);
                sqlExec.AddCommandParam("@PageNumber", SqlDbType.Int, ParameterDirection.Input, PageNumber);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, ParameterDirection.Input, OrganizationId);

                resultSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to get Vehicles for ServiceCloud - OrganizationId={0}.", OrganizationId.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to get Vehicles for ServiceCloud - OrganizationId={0}.", OrganizationId.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return resultSet;
        }

        /// <summary>
        /// Gets Boxes for ServiceCloud
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns>[BoxId],[Firmware],[Hardware_Name],[Account]</returns>
        public DataSet GetBoxesForServiceCloud(int PageSize, int PageNumber, int OrganizationId)
        {
            DataSet resultSet = new DataSet();
            string sql = "sp_BoxesForServiceCloud_Get";

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@PageSize", SqlDbType.Int, ParameterDirection.Input, PageSize);
                sqlExec.AddCommandParam("@PageNumber", SqlDbType.Int, ParameterDirection.Input, PageNumber);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, ParameterDirection.Input, OrganizationId);

                resultSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to get Boxes for ServiceCloud - OrganizationId={0}.", OrganizationId.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to get Boxes for ServiceCloud - OrganizationId={0}.", OrganizationId.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return resultSet;
        }

        /// <summary>
        /// Gets Vehicle Assigments for ServiceCloud
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns>[License_Plate],[Vehicle],[Asset],[Assigned_Date]</returns>
        public DataSet GetVehicleAssigmentsForServiceCloud(int PageSize, int PageNumber, int OrganizationId)
        {
            DataSet resultSet = new DataSet();
            string sql = "sp_VehicleAssigmentsForServiceCloud_Get";

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@PageSize", SqlDbType.Int, ParameterDirection.Input, PageSize);
                sqlExec.AddCommandParam("@PageNumber", SqlDbType.Int, ParameterDirection.Input, PageNumber);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, ParameterDirection.Input, OrganizationId);

                resultSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to get Vehicle Assigments for ServiceCloud - OrganizationId={0}.", OrganizationId.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to get Vehicle Assigments for ServiceCloud - OrganizationId={0}.", OrganizationId.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return resultSet;
        }

        /// <summary>
        /// Gets Equipment Hours Details
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="VehicleDesc"></param>
        /// <param name="UserID"></param>
        /// <returns>[VehicleHoursID],[id],[tm_telepodidfk],[VehicleDesc],[VehicleId],[HoursReading],[gps_time],[GeoAddress],[createddate],[DeviceHrs],[DeviceOffsetHrs]</returns>
        public DataSet GetHoursDetailsByRegistrationDateRange(string FromDate, string ToDate, string VehicleDesc, int UserID)
        {
            DataSet resultSet = new DataSet();
            string sql = "usp_AssetEngineHoursDetailsWS";

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@FromDate", SqlDbType.VarChar, ParameterDirection.Input, FromDate);
                sqlExec.AddCommandParam("@ToDate", SqlDbType.VarChar, ParameterDirection.Input, ToDate);
                sqlExec.AddCommandParam("@VehicleDesc", SqlDbType.VarChar, ParameterDirection.Input, VehicleDesc);
                sqlExec.AddCommandParam("@UserID", SqlDbType.Int, ParameterDirection.Input, UserID);

                resultSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to get Equipment Hours Details - VehicleDesc={0}, UserID={1}.", VehicleDesc, UserID.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to get Equipment Hours Details - VehicleDesc={0}, UserID={1}.", VehicleDesc, UserID.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return resultSet;
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
            DataSet resultSet = new DataSet();
            string sql = "usp_AssetEngineHoursWS";

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@FromDate", SqlDbType.VarChar, ParameterDirection.Input, FromDate);
                sqlExec.AddCommandParam("@ToDate", SqlDbType.VarChar, ParameterDirection.Input, ToDate);
                sqlExec.AddCommandParam("@VehicleDesc", SqlDbType.VarChar, ParameterDirection.Input, VehicleDesc);
                sqlExec.AddCommandParam("@UserID", SqlDbType.Int, ParameterDirection.Input, UserID);

                resultSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to get Equipment Hours - VehicleDesc={0}, UserID={1}.", VehicleDesc, UserID.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to get Equipment Hours - VehicleDesc={0}, UserID={1}.", VehicleDesc, UserID.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return resultSet;
        }

        /// <summary>
        /// Gets Last VAlue of Equipment Hours
        /// </summary>
        /// <param name="VehicleDesc"></param>
        /// <param name="UserID"></param>
        /// <returns>[ID],[VehLastKnownHours],[DeviceOffsetHrs],[GPSTime],[EasternTime]</returns>
        public DataSet GetCurrentHoursByRegistration(string VehicleDesc, int UserID)
        {
            DataSet resultSet = new DataSet();
            string sql = "usp_LastAssetEngineHoursWS";

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@VehicleDesc", SqlDbType.VarChar, ParameterDirection.Input, VehicleDesc);
                sqlExec.AddCommandParam("@UserID", SqlDbType.Int, ParameterDirection.Input, UserID);

                resultSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to get Current Equipment Hours - VehicleDesc={0}, UserID={1}.", VehicleDesc, UserID.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to get Current Equipment Hours - VehicleDesc={0}, UserID={1}.", VehicleDesc, UserID.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return resultSet;
        }

    }
}
