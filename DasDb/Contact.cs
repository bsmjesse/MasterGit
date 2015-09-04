using System;
using System.Collections.Generic;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.DB
{
    public class Contact : TblGenInterfaces
    {
      public Contact(SQLExecuter sqlExec)
            : base("ContactInfo", sqlExec)
      {
      }

      /// <summary>
      /// Add Contact Info
      /// </summary>
      /// <param name="OrganizationId"></param>
      /// <param name="Company"></param>
      /// <param name="FirstName"></param>
      /// <param name="MiddleName"></param>
      /// <param name="LastName"></param>
      /// <param name="TimeZone"></param>
      /// <param name="Contacts"></param>
      /// <returns></returns>
      public int ContactInfo_Add(int OrganizationId, Boolean IsCompany, string Company, string FirstName,
          string MiddleName, string LastName, int TimeZone, string Contacts)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@IsCompany", SqlDbType.Bit, IsCompany);
                sqlExec.AddCommandParam("@Company", SqlDbType.VarChar, Company);
                sqlExec.AddCommandParam("@FirstName", SqlDbType.VarChar, FirstName);
                sqlExec.AddCommandParam("@MiddleName", SqlDbType.VarChar, MiddleName);
                sqlExec.AddCommandParam("@LastName", SqlDbType.VarChar, LastName);
                sqlExec.AddCommandParam("@TimeZone", SqlDbType.Int, TimeZone);
                sqlExec.AddCommandParam("@Contacts", SqlDbType.VarChar, Contacts);

                rowsAffected = sqlExec.SPExecuteNonQuery("[ContactInfo_Add]");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new contact info Last Name=" + LastName + " Company=" + Company;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new contact info Last Name=" + LastName + " Company=" + Company;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Update ContactInfo
        /// </summary>
        /// <param name="ContactInfoId"></param>
        /// <param name="Company"></param>
        /// <param name="FirstName"></param>
        /// <param name="MiddleName"></param>
        /// <param name="LastName"></param>
        /// <param name="TimeZone"></param>
        /// <param name="Contacts"></param>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public int ContactInfo_Update(Int64 ContactInfoId, Boolean IsCompany, string Company, string FirstName,
          string MiddleName, string LastName, int TimeZone, string DeletedIds, string Contacts, int OrganizationId)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ContactInfoId", SqlDbType.BigInt, ContactInfoId);
                sqlExec.AddCommandParam("@IsCompany", SqlDbType.Bit, IsCompany);
                sqlExec.AddCommandParam("@Company", SqlDbType.VarChar, Company);
                sqlExec.AddCommandParam("@FirstName", SqlDbType.VarChar, FirstName);
                sqlExec.AddCommandParam("@MiddleName", SqlDbType.VarChar, MiddleName);
                sqlExec.AddCommandParam("@LastName", SqlDbType.VarChar, LastName);
                sqlExec.AddCommandParam("@TimeZone", SqlDbType.Int, TimeZone);
                sqlExec.AddCommandParam("@DeletedIds", SqlDbType.VarChar, DeletedIds);
                sqlExec.AddCommandParam("@Contacts", SqlDbType.VarChar, Contacts);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);

                rowsAffected = sqlExec.SPExecuteNonQuery("[ContactInfo_Update]");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update contact info ContactInfoId" + ContactInfoId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update contact info ContactInfoId" + ContactInfoId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }
        /// <summary>
        /// GetCommunicationTypes 
        /// </summary>
        /// <returns></returns>
        public DataSet GetCommunicationTypes()
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "GetCommunicationType";

                sqlExec.ClearCommandParameters();

                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Communication Type.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Communication Type.";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Get Organization Contacts By OrganizationId
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationContacts(int OrganizationId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "GetOrganizationContacts";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Organization Contacts By OrganizationId=" + OrganizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Organization Contacts By OrganizationId=" + OrganizationId;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Get Organization Contact Plans
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationContactPlan(int OrganizationId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "GetOrganizationContactPlan";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Organization Contact Plans By OrganizationId=" + OrganizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Organization Contacts Plans By OrganizationId=" + OrganizationId;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Get Vehicle Contact Communications By ContactId
        /// </summary>
        /// <param name="ContactId"></param>
        /// <returns></returns>
        public DataSet GetVehicleContactCommunicationsByContactId(Int64 ContactId)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = "GetVehicleContactCommunicationsByContactId";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ContactId", SqlDbType.BigInt, ContactId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Vehicle Contact Communications By ContactId=" + ContactId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Vehicle Contact Communications By ContactId=" + ContactId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet GetVehicleContactCommunicationsByContactIdOrgID(Int64 ContactId, int OrganizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = "GetVehicleContactCommunicationsByContactIdOrgID";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ContactId", SqlDbType.BigInt, ContactId);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Vehicle Contact Communications By ContactId=" + ContactId + " OrganizationId=" + OrganizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Vehicle Contact Communications By ContactId=" + ContactId + " OrganizationId=" + OrganizationId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// <summary>
        /// Delete ContactInfo
        /// </summary>
        /// <param name="ContactInfoId"></param>
        /// <returns></returns>
        public int ContactInfo_Delete(Int64 ContactInfoId)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ContactInfoId", SqlDbType.BigInt, ContactInfoId);
                rowsAffected = sqlExec.SPExecuteNonQuery("ContactInfo_Delete");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Delete ContactInfo by ContactInfoId =" + ContactInfoId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Delete ContactInfo by ContactInfoId =" + ContactInfoId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Add ContactPlan
        /// </summary>
        /// <param name="ContactPlanName"></param>
        /// <param name="OrganizationId"></param>
        /// <param name="ContactCommunicationID"></param>
        /// <returns></returns>
        public int ContactPlan_Add(string ContactPlanName, int OrganizationId, string ContactCommunicationIDs)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ContactPlanName", SqlDbType.VarChar, ContactPlanName);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@ContactCommunicationIDs", SqlDbType.VarChar, ContactCommunicationIDs);

                rowsAffected = sqlExec.SPExecuteNonQuery("[ContactPlan_Add]");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add ContactPlan, ContactPlanName=" + ContactPlanName;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add ContactPlan, ContactPlanName=" + ContactPlanName;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }

        /// <summary>
        /// Get Contact Communication Data By Organization
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="CommunicationTypeId"></param>
        /// <returns></returns>
        public DataSet GetContactCommunicationDataByOrganization(int OrganizationId, int CommunicationTypeId)
        {
            DataSet sqlDataSet = null;
            try
            {
                string nameSql = "CASE a.IsCompany " +
                                    "WHEN 0 THEN isnull(a.FirstName, '') + ' ' + isnull(a.MiddleName, '') + ' ' + isnull(a.LastName, '') " +
                                    "ELSE a.Company " +
                                 "END AS Name";

                string sql = "Select {0},b.contactCommunicationID, b.CommunicationData from ContactInfo a " +
                       "inner join ContactCommunications b on a.ContactInfoId = b.ContactId and " +
                       "a.OrganizationId = {1} and b.CommunicationTypeId = {2} order by name ";
                sql = string.Format(sql, nameSql, OrganizationId, CommunicationTypeId);
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Contact Communication Data By OrganizationId=" + OrganizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Contact Communication Data By OrganizationId=" + OrganizationId;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// <summary>
        /// Get Contact Communication Data By Organization
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="CommunicationTypeId"></param>
        /// <param name="ContactPlanId"></param>
        /// <returns></returns>
        public DataSet GetContactCommunicationDataByOrganization(int OrganizationId, int CommunicationTypeId, int ContactPlanId)
        {
            DataSet sqlDataSet = null;
            try
            {
                string nameSql = "CASE a.IsCompany " +
                                    "WHEN 0 THEN isnull(a.FirstName, '') + ' ' + isnull(a.MiddleName, '') + ' ' + isnull(a.LastName, '') " +
                                    "ELSE a.Company " +
                                 "END AS Name";

                string sql = "Select {0},b.contactCommunicationID, b.CommunicationData from ContactInfo a " +
                       "inner join ContactCommunications b on a.ContactInfoId = b.ContactId and " +
                       "a.OrganizationId = {1} and b.CommunicationTypeId = {2} " +
                       "and b.ContactCommunicationID not in (Select ContactCommunicationID from ContactPlanCommunications where ContactPlanId = {3}) " + 
                       "order by name ";
                sql = string.Format(sql, nameSql, OrganizationId, CommunicationTypeId, ContactPlanId);
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Contact Communication Data By OrganizationId=" + OrganizationId + " ContactPlanId = " + ContactPlanId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Contact Communication Data By OrganizationId=" + OrganizationId + " ContactPlanId = " + ContactPlanId;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// <summary>
        /// Get Communication Data By contactCommunicationID
        /// </summary>
        /// <param name="contactCommunicationID"></param>
        /// <param name="CommunicationTypeId"></param>
        /// <returns></returns>
        public DataSet GetCommunicationDataByID(Int64 contactCommunicationID, int CommunicationTypeId, int OrganizationId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "Declare @ContactId bigint; " +
                             "Select @ContactId = ContactId from ContactCommunications where contactCommunicationID = {0}; " +
                             "if exists(Select ContactInfoId from ContactInfo where ContactInfoId = @ContactId and OrganizationId = {2}) " +
                             "Begin " +
                                "Select contactCommunicationID, CommunicationTypeId, CommunicationData from ContactCommunications where  " +
                                "contactCommunicationID = {0} or (ContactId=@ContactId and CommunicationTypeId = {1})" +
                             "End";
                sql = string.Format(sql, contactCommunicationID, CommunicationTypeId, OrganizationId);
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Communication Data By contactCommunicationID=" + contactCommunicationID;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Communication Data By contactCommunicationID=" + contactCommunicationID;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Get Plan Communication Data By ContactPlanId
        /// </summary>
        /// <param name="ContactPlanId"></param>
        /// <returns></returns>
        public DataSet GetPlanCommunicationDataByContactPlanId(int ContactPlanId, int CommunicationTypeId)
        {
            DataSet sqlDataSet = null;
            try
            {
                string nameSql = "CASE c.IsCompany " +
                                    "WHEN 0 THEN isnull(c.FirstName, '') + ' ' + isnull(c.MiddleName, '') + ' ' + isnull(c.LastName, '') " +
                                    "ELSE c.Company " +
                                 "END AS Name";

                string sql = "Select {0},a.Priority, b.contactCommunicationID, b.CommunicationData from ContactPlanCommunications a " +
                       "inner join ContactCommunications b on a.ContactCommunicationID = b.ContactCommunicationID " +
                       "and b.CommunicationTypeId = {1}" +
                       "inner join ContactInfo c on c.ContactInfoId = b.ContactId " +
                       "where a.ContactPlanId = {2} order by a.Priority";
                sql = string.Format(sql, nameSql, CommunicationTypeId, ContactPlanId);
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Plan Communication Data By ContactPlanId =" + ContactPlanId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Plan Communication Data By ContactPlanId =" + ContactPlanId;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Delete ContactPlan
        /// </summary>
        /// <param name="ContactPlanId"></param>
        /// <returns></returns>
        public int ContactPlan_Delete(int ContactPlanId)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ContactPlanId", SqlDbType.Int, ContactPlanId);

                rowsAffected = sqlExec.SPExecuteNonQuery("[ContactPlan_Delete]");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Delete ContactPlan, ContactPlanId=" + ContactPlanId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Delete ContactPlan, ContactPlanId=" + ContactPlanId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;

        }

        /// <summary>
        /// Delete ContactPlanCommunications
        /// 
        /// </summary>
        /// <param name="ContactPlanId"></param>
        /// <param name="ContactCommunicationID"></param>
        /// <returns></returns>
        public int ContactPlanCommunications_Delete(int ContactPlanId, Int64 ContactCommunicationID)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ContactPlanId", SqlDbType.Int, ContactPlanId);
                sqlExec.AddCommandParam("@ContactCommunicationID", SqlDbType.BigInt, ContactCommunicationID);

                rowsAffected = sqlExec.SPExecuteNonQuery("[ContactPlanCommunications_Delete]");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Delete ContactPlanCommunications, ContactPlanId=" + ContactPlanId + " ContactCommunicationID=" + ContactCommunicationID;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Delete ContactPlanCommunications, ContactPlanId=" + ContactPlanId + " ContactCommunicationID=" + ContactCommunicationID;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;

        }

        /// <summary>
        /// Add ContactPlanCommunications
        /// </summary>
        /// <param name="ContactPlanId"></param>
        /// <param name="ContactCommunicationIDs"></param>
        /// <returns></returns>
        public int ContactPlanCommunications_Add(int ContactPlanId, string ContactCommunicationIDs)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ContactPlanId", SqlDbType.Int, ContactPlanId);
                sqlExec.AddCommandParam("@ContactCommunicationIDs", SqlDbType.VarChar, @ContactCommunicationIDs);

                rowsAffected = sqlExec.SPExecuteNonQuery("[ContactPlanCommunications_Add]");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add ContactPlanCommunications, ContactPlanId=" + ContactPlanId + " ContactCommunicationIDs=" + ContactCommunicationIDs;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add ContactPlanCommunications, ContactPlanId=" + ContactPlanId + " ContactCommunicationIDs=" + ContactCommunicationIDs;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;

        }

        /// <summary>
        /// IsContactInUsed
        /// </summary>
        /// <param name="ContactInfoId"></param>
        /// <returns></returns>
        public Boolean IsContactInUsed(Int64 ContactInfoId)
        {
            Boolean isContactInUsed = true;
            try
            {
                string sql = "Select count(ContactInfoId) from ContactInfo a inner join " +
                              "ContactCommunications b on a.ContactInfoId={0} and a.ContactInfoId = b.ContactId inner join " +
                              "ContactPlanCommunications c on  b.ContactCommunicationID = c.ContactCommunicationID";

                object ret = sqlExec.SQLExecuteScalar(string.Format(sql,ContactInfoId));
                if (ret == null || ((Int32)ret) == 0)
                {
                    isContactInUsed = false;
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve IsContactInUsed By ContactInfoId=" + ContactInfoId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve IsContactInUsed By ContactInfoId=" + ContactInfoId;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return isContactInUsed;
        }

        /// <summary>
        /// IsContactCommunicationIDsInOrganization
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="ContactCommunicationIDs"></param>
        /// <returns></returns>
        public Boolean IsContactCommunicationIDsInOrganization(int OrganizationId, string ContactCommunicationIDs)
        {
            Boolean isInOrganization = false;
            try
            {
                string sql = "if exists(Select ContactInfoId from ContactInfo a inner join " +
                              "ContactCommunications b on a.OrganizationId <> {0} and a.ContactInfoId = b.ContactId and " +
                              "b.ContactCommunicationID  in ({1})) select 0; " +
                              "else select 1; ";

                DataSet ds = sqlExec.SQLExecuteDataset(string.Format(sql, OrganizationId, ContactCommunicationIDs));
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                    {
                        if ((int)ds.Tables[0].Rows[0][0] == 1) isInOrganization = true;
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve IsContactCommunicationIDsInOrganization By OrganizationId=" + OrganizationId + " ContactCommunicationIDs=" + ContactCommunicationIDs;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve IsContactCommunicationIDsInOrganization By OrganizationId=" + OrganizationId + " ContactCommunicationIDs=" + ContactCommunicationIDs;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return isInOrganization;

        }

        /// <summary>
        /// GetOrganizationDriverContactPlan(int OrganizationId)
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationDriverContactPlan(int OrganizationId, int DriverId)
        {
            DataSet sqlDataSet = null;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, DriverId);

                sqlDataSet = sqlExec.SPExecuteDataset("GetOrganizationDriverContactPlan");

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Get Organization Driver Contact Plan, OrganizationId=" + OrganizationId ;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Get Organization Driver Contact Plan, OrganizationId=" + OrganizationId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Add Driver Contact Plan
        /// </summary>
        /// <param name="DriverId"></param>
        /// <param name="EmergencyPhone"></param>
        /// <param name="ContactPlanId"></param>
        /// <param name="IsAssignPanic"></param>
        /// <returns></returns>
        public int DriverAndContactPlanAdd(string firstName, string lastName, string license, string classLicense, DateTime? licenseIssued,
         DateTime? licenseExpired, int orgId, char gender, short height, string homePhone, string cellPhone, string additionalPhone,
         string smsPwd,
         string smsid, string email, string address, string city, string zipcode, string state, string country, string description,
         string EmergencyPhone, int? ContactPlanId, Boolean IsAssignPanic, string KeyFobId)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@firstName", SqlDbType.VarChar, firstName);
                sqlExec.AddCommandParam("@lastName", SqlDbType.VarChar, lastName);
                sqlExec.AddCommandParam("@license", SqlDbType.VarChar, license);
                sqlExec.AddCommandParam("@class", SqlDbType.VarChar, classLicense);
                sqlExec.AddCommandParam("@licenseIssued", SqlDbType.DateTime, licenseIssued);
                sqlExec.AddCommandParam("@licenseExpired", SqlDbType.DateTime, licenseExpired);
                sqlExec.AddCommandParam("@orgId", SqlDbType.Int, orgId);
                sqlExec.AddCommandParam("@gender", SqlDbType.Char, gender.ToString());
                sqlExec.AddCommandParam("@height", SqlDbType.SmallInt, height);
                sqlExec.AddCommandParam("@homePhone", SqlDbType.VarChar, homePhone);
                sqlExec.AddCommandParam("@cellPhone", SqlDbType.VarChar, cellPhone);
                sqlExec.AddCommandParam("@additionalPhone", SqlDbType.VarChar, additionalPhone);
                sqlExec.AddCommandParam("@smsPwd", SqlDbType.VarChar, smsPwd);
                sqlExec.AddCommandParam("@smsid", SqlDbType.VarChar, smsid);
                sqlExec.AddCommandParam("@email", SqlDbType.VarChar, email);
                sqlExec.AddCommandParam("@address", SqlDbType.VarChar, address);
                sqlExec.AddCommandParam("@city", SqlDbType.VarChar, city);
                sqlExec.AddCommandParam("@zipcode", SqlDbType.VarChar, zipcode);
                sqlExec.AddCommandParam("@state", SqlDbType.VarChar, state);
                sqlExec.AddCommandParam("@country", SqlDbType.VarChar, country);
                sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, description);

                sqlExec.AddCommandParam("@EmergencyPhone", SqlDbType.VarChar, EmergencyPhone);
                sqlExec.AddCommandParam("@ContactPlanId", SqlDbType.Int, ContactPlanId);
                sqlExec.AddCommandParam("@IsAssignPanic", SqlDbType.Bit, IsAssignPanic);
                sqlExec.AddCommandParam("@KeyFobId", SqlDbType.VarChar, KeyFobId);

                rowsAffected = sqlExec.SPExecuteNonQuery("[DriverAndContactPlanAdd]");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Add Driver Contact Plan, firstName=" + firstName + " lastName=" + lastName;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Add Driver Contact Plan, firstName=" + firstName + " lastName=" + lastName;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;

        }

        public int DriverAndContactPlanUpdate(int driverId, string firstName, string lastName, string license, string classLicense, DateTime? licenseIssued,
                 DateTime? licenseExpired, int orgId, char gender, short height, string homePhone, string cellPhone, string additionalPhone,
                 string smsPwd,
                 string smsid, string email, string address, string city, string zipcode, string state, string country, string description,
                 string EmergencyPhone, int? ContactPlanId, Boolean IsAssignPanic, string KeyFobId)
        {
            int rowsAffected = 0;

            // 1. Prepares SQL statement
            string sql = "[DriverAndContactPlanUpdate]";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);
            sqlExec.AddCommandParam("@firstName", SqlDbType.VarChar, firstName, 50);
            sqlExec.AddCommandParam("@lastName", SqlDbType.VarChar, lastName, 50);
            sqlExec.AddCommandParam("@license", SqlDbType.VarChar, license, 50);
            sqlExec.AddCommandParam("@class", SqlDbType.VarChar, classLicense, 20);
            sqlExec.AddCommandParam("@licenseIssued", SqlDbType.DateTime, licenseIssued);
            sqlExec.AddCommandParam("@licenseExpired", SqlDbType.DateTime, licenseExpired);
            sqlExec.AddCommandParam("@orgId", SqlDbType.Int, orgId);
            sqlExec.AddCommandParam("@gender", SqlDbType.Char, gender.ToString(), 1);
            sqlExec.AddCommandParam("@height", SqlDbType.SmallInt, height);
            sqlExec.AddCommandParam("@homePhone", SqlDbType.VarChar, homePhone, 20);
            sqlExec.AddCommandParam("@cellPhone", SqlDbType.VarChar, cellPhone, 20);
            sqlExec.AddCommandParam("@additionalPhone", SqlDbType.VarChar, additionalPhone, 20);
            sqlExec.AddCommandParam("@smsPwd", SqlDbType.VarChar, smsPwd, 50);
            sqlExec.AddCommandParam("@smsid", SqlDbType.VarChar, smsid, 50);
            sqlExec.AddCommandParam("@email", SqlDbType.VarChar, email, 250);
            sqlExec.AddCommandParam("@address", SqlDbType.VarChar, address, 100);
            sqlExec.AddCommandParam("@city", SqlDbType.VarChar, city, 50);
            sqlExec.AddCommandParam("@zipcode", SqlDbType.VarChar, zipcode, 20);
            sqlExec.AddCommandParam("@state", SqlDbType.VarChar, state, 50);
            sqlExec.AddCommandParam("@country", SqlDbType.VarChar, country, 50);
            sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, description, 100);

            sqlExec.AddCommandParam("@EmergencyPhone", SqlDbType.VarChar, EmergencyPhone);
            sqlExec.AddCommandParam("@ContactPlanId", SqlDbType.Int, ContactPlanId);
            sqlExec.AddCommandParam("@IsAssignPanic", SqlDbType.Bit, IsAssignPanic);
            sqlExec.AddCommandParam("@KeyFobId", SqlDbType.VarChar, KeyFobId);

            //Executes SQL statement
            rowsAffected = sqlExec.SPExecuteNonQuery(sql);

            return rowsAffected;
        }

       /// <summary>
        /// Checks if driver is unique
       /// </summary>
       /// <param name="DriverId"></param>
       /// <param name="OrganizationID"></param>
       /// <param name="FirstName"></param>
       /// <param name="LastName"></param>
       /// <param name="smsid"></param>
        /// <returns>IsValidDriverName</returns>
        public int CheckIfDriverUnique(int DriverId, int OrganizationID, string FirstName, string LastName, string smsid)
        {
            int IsValidDriverName = 1;
            try
            {
                sqlExec.ClearCommandParameters();
                if (DriverId == 0)
                    sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, ParameterDirection.Input, DBNull.Value);
                else
                    sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, ParameterDirection.Input, DriverId);
                sqlExec.AddCommandParam("@OrganizationID", SqlDbType.Int, ParameterDirection.Input, OrganizationID);
                sqlExec.AddCommandParam("@FirstName", SqlDbType.VarChar, ParameterDirection.Input, FirstName);
                sqlExec.AddCommandParam("@LastName", SqlDbType.VarChar, ParameterDirection.Input, LastName);
                if (String.IsNullOrEmpty(smsid))
                    sqlExec.AddCommandParam("@smsid", SqlDbType.VarChar, ParameterDirection.Input, DBNull.Value);
                else
                    sqlExec.AddCommandParam("@smsid", SqlDbType.VarChar, ParameterDirection.Input, smsid);
                sqlExec.AddCommandParam("@IsValidDriverName", SqlDbType.Int, ParameterDirection.Output, 2, IsValidDriverName);

                int res = sqlExec.SPExecuteNonQuery("usp_DriverUnique_Check");

                IsValidDriverName = (DBNull.Value == sqlExec.ReadCommandParam("@IsValidDriverName")) ?
                              IsValidDriverName : Convert.ToInt32(sqlExec.ReadCommandParam("@IsValidDriverName"));
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to check if driver is unique - FirstName={0}, LastName={1}, DriverId={2}.", FirstName.ToString(), LastName.ToString(), DriverId.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to check if driver is unique - FirstName={0}, LastName={1}, DriverId={2}.", FirstName.ToString(), LastName.ToString(), DriverId.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return IsValidDriverName;
        }

        /// <summary>
        /// Add Driver Contact Plan
        /// </summary>
        /// <param name="DriverId"></param>
        /// <param name="EmergencyPhone"></param>
        /// <param name="ContactPlanId"></param>
        /// <param name="IsAssignPanic"></param>
        /// <returns></returns>
        public int DriverAndContactPlanCycleAdd(string firstName, string lastName, string license, string classLicense, DateTime? licenseIssued,
         DateTime? licenseExpired, int orgId, char gender, short height, string homePhone, string cellPhone, string additionalPhone,
         string smsPwd,
         string smsid, string email, string address, string city, string zipcode, string state, string country, string description,
         string EmergencyPhone, int? ContactPlanId, Boolean IsAssignPanic, string KeyFobId,
         int? caCycle, int? usCycle, Boolean isSupervisor, string positionInfo, float? timzone = null
         )
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@firstName", SqlDbType.VarChar, firstName);
                sqlExec.AddCommandParam("@lastName", SqlDbType.VarChar, lastName);
                sqlExec.AddCommandParam("@license", SqlDbType.VarChar, license);
                sqlExec.AddCommandParam("@class", SqlDbType.VarChar, classLicense);
                sqlExec.AddCommandParam("@licenseIssued", SqlDbType.DateTime, licenseIssued);
                sqlExec.AddCommandParam("@licenseExpired", SqlDbType.DateTime, licenseExpired);
                sqlExec.AddCommandParam("@orgId", SqlDbType.Int, orgId);
                sqlExec.AddCommandParam("@gender", SqlDbType.Char, gender.ToString());
                sqlExec.AddCommandParam("@height", SqlDbType.SmallInt, height);
                sqlExec.AddCommandParam("@homePhone", SqlDbType.VarChar, homePhone);
                sqlExec.AddCommandParam("@cellPhone", SqlDbType.VarChar, cellPhone);
                sqlExec.AddCommandParam("@additionalPhone", SqlDbType.VarChar, additionalPhone);
                sqlExec.AddCommandParam("@smsPwd", SqlDbType.VarChar, smsPwd);
                sqlExec.AddCommandParam("@smsid", SqlDbType.VarChar, smsid);
                sqlExec.AddCommandParam("@email", SqlDbType.VarChar, email);
                sqlExec.AddCommandParam("@address", SqlDbType.VarChar, address);
                sqlExec.AddCommandParam("@city", SqlDbType.VarChar, city);
                sqlExec.AddCommandParam("@zipcode", SqlDbType.VarChar, zipcode);
                sqlExec.AddCommandParam("@state", SqlDbType.VarChar, state);
                sqlExec.AddCommandParam("@country", SqlDbType.VarChar, country);
                sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, description);

                sqlExec.AddCommandParam("@EmergencyPhone", SqlDbType.VarChar, EmergencyPhone);
                sqlExec.AddCommandParam("@ContactPlanId", SqlDbType.Int, ContactPlanId);
                sqlExec.AddCommandParam("@IsAssignPanic", SqlDbType.Bit, IsAssignPanic);
                sqlExec.AddCommandParam("@KeyFobId", SqlDbType.VarChar, KeyFobId);

                sqlExec.AddCommandParam("@CACycle", SqlDbType.SmallInt, caCycle);
                sqlExec.AddCommandParam("@USCycle", SqlDbType.SmallInt, usCycle);
                sqlExec.AddCommandParam("@IsSupervisor", SqlDbType.SmallInt, isSupervisor);

                //Salman June 27, 2014
                sqlExec.AddCommandParam("@positionInfo", SqlDbType.VarChar, positionInfo);

                //Devin Nov 26, 2014
                sqlExec.AddCommandParam("@Timezone", SqlDbType.Float, timzone);

                rowsAffected = sqlExec.SPExecuteNonQuery("[DriverAndContactPlanAdd]");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Add Driver Contact Plan, firstName=" + firstName + " lastName=" + lastName;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Add Driver Contact Plan, firstName=" + firstName + " lastName=" + lastName;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;

        }

        public int DriverAndContactPlanCycleUpdate(int driverId, string firstName, string lastName, string license, string classLicense, DateTime? licenseIssued,
                 DateTime? licenseExpired, int orgId, char gender, short height, string homePhone, string cellPhone, string additionalPhone,
                 string smsPwd,
                 string smsid, string email, string address, string city, string zipcode, string state, string country, string description,
                 string EmergencyPhone, int? ContactPlanId, Boolean IsAssignPanic, string KeyFobId,
                 int? caCycle, int? usCycle, Boolean isSupervisor, string positionInfo, DateTime? terminationDate, float? timzone = null)
        {
            int rowsAffected = 0;

            // 1. Prepares SQL statement
            string sql = "[DriverAndContactPlanUpdate]";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);
            sqlExec.AddCommandParam("@firstName", SqlDbType.VarChar, firstName, 50);
            sqlExec.AddCommandParam("@lastName", SqlDbType.VarChar, lastName, 50);
            sqlExec.AddCommandParam("@license", SqlDbType.VarChar, license, 50);
            sqlExec.AddCommandParam("@class", SqlDbType.VarChar, classLicense, 20);
            sqlExec.AddCommandParam("@licenseIssued", SqlDbType.DateTime, licenseIssued);
            sqlExec.AddCommandParam("@licenseExpired", SqlDbType.DateTime, licenseExpired);
            sqlExec.AddCommandParam("@orgId", SqlDbType.Int, orgId);
            sqlExec.AddCommandParam("@gender", SqlDbType.Char, gender.ToString(), 1);
            sqlExec.AddCommandParam("@height", SqlDbType.SmallInt, height);
            sqlExec.AddCommandParam("@homePhone", SqlDbType.VarChar, homePhone, 20);
            sqlExec.AddCommandParam("@cellPhone", SqlDbType.VarChar, cellPhone, 20);
            sqlExec.AddCommandParam("@additionalPhone", SqlDbType.VarChar, additionalPhone, 20);
            sqlExec.AddCommandParam("@smsPwd", SqlDbType.VarChar, smsPwd, 50);
            sqlExec.AddCommandParam("@smsid", SqlDbType.VarChar, smsid, 50);
            sqlExec.AddCommandParam("@email", SqlDbType.VarChar, email, 250);
            sqlExec.AddCommandParam("@address", SqlDbType.VarChar, address, 100);
            sqlExec.AddCommandParam("@city", SqlDbType.VarChar, city, 50);
            sqlExec.AddCommandParam("@zipcode", SqlDbType.VarChar, zipcode, 20);
            sqlExec.AddCommandParam("@state", SqlDbType.VarChar, state, 50);
            sqlExec.AddCommandParam("@country", SqlDbType.VarChar, country, 50);
            sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, description, 100);

            sqlExec.AddCommandParam("@EmergencyPhone", SqlDbType.VarChar, EmergencyPhone);
            sqlExec.AddCommandParam("@ContactPlanId", SqlDbType.Int, ContactPlanId);
            sqlExec.AddCommandParam("@IsAssignPanic", SqlDbType.Bit, IsAssignPanic);
            sqlExec.AddCommandParam("@KeyFobId", SqlDbType.VarChar, KeyFobId);

            sqlExec.AddCommandParam("@CACycle", SqlDbType.SmallInt, caCycle);
            sqlExec.AddCommandParam("@USCycle", SqlDbType.SmallInt, usCycle);
            sqlExec.AddCommandParam("@IsSupervisor", SqlDbType.SmallInt, isSupervisor);

            //Salman June 27, 2014
            sqlExec.AddCommandParam("@positionInfo", SqlDbType.VarChar, positionInfo);
            sqlExec.AddCommandParam("@terminationDate", SqlDbType.DateTime, terminationDate);

            //Devin Nov 26, 2014
            sqlExec.AddCommandParam("@Timezone", SqlDbType.Float, timzone);

            //Executes SQL statement
            rowsAffected = sqlExec.SPExecuteNonQuery(sql);

            return rowsAffected;
        }

        /// <summary>
        /// Delete Driver And ContactPlan
        /// </summary>
        /// <param name="driverId"></param>
        /// <returns></returns>
        public int DriverAndContactPlanDelete(int driverId)
        {
            int rowsAffected = 0;

            //Prepares SQL statement
            string sql = "DriverAndContactPlanDelete";

            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);

            //Executes SQL statement
            rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            return rowsAffected;
        }
        
        /// <summary>
        /// Get Drivers by OrganizationId
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetDriversEmployeeInfo(int OrganizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = "SELECT DriverId, EmployeeId, DriverName from EmployeeInfo (nolock) " +
                             "WHERE CompanyRowId = (select RowID from RegisteredCompany (nolock) where CompanyID={0}) and IsActive=1";
                sql = string.Format(sql, OrganizationId);
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Organization Drivers Employee Info By OrganizationId =" + OrganizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Organization Drivers Employee Info By OrganizationId =" + OrganizationId;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
        /// <summary>
        /// Get Drivers by OrganizationId
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationDrivers(int OrganizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = "SELECT vlfDriver.*, RTRIM(FirstName) + SPACE(3) + LTRIM(LastName) AS FullName, " +
                             "ISNULL(vlfVehicleInfo.[Description], 'N/A') AS VehicleDescription, DriverContactPlan.DriverContactPlanId " +
                             //"ContactPlan.ContactPlanId, ContactPlan.ContactPlanName " +
	                         "FROM vlfDriver " + 
	                         "LEFT JOIN vlfDriverAssignment ON vlfDriver.DriverId = vlfDriverAssignment.DriverId " + 
	                         "LEFT JOIN vlfVehicleInfo ON vlfDriverAssignment.VehicleId = vlfVehicleInfo.VehicleId " +
                             "LEFT JOIN DriverContactPlan ON DriverContactPlan.DriverId = vlfDriver.DriverId " +
                             //"LEFT JOIN ContactPlan on DriverContactPlan.ContactPlanId = ContactPlan.ContactPlanId " + 
                             "WHERE vlfDriver.OrganizationId = {0} and UPPER(LTRIM(RTRIM(ISNULL(KeyFobId,''))))<>'UNKNOWN'";
                sql = string.Format(sql, OrganizationId);
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Organization Drivers By OrganizationId =" + OrganizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Organization Drivers By OrganizationId =" + OrganizationId;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Get Driver by DriverID
        /// </summary>
        /// <param name="DriverId"></param>
        /// <returns></returns>
        public DataSet GetDriverByDriverID(int DriverId)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = "SELECT vlfDriver.*, DriverContactPlan.ContactPlanId, DriverContactPlan.DriverContactPlanId, " +
                             "DriverContactPlan.EmergencyPhone,ISNULL(KeyFobId,'') as KeyFobId " +
                             "FROM vlfDriver " +
                             "LEFT JOIN DriverContactPlan ON DriverContactPlan.DriverId = vlfDriver.DriverId " +
                             "WHERE vlfDriver.DriverId = {0} ";
                sql = string.Format(sql, DriverId);
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Driver By DriverID =" + DriverId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Driver By DriverID =" + DriverId;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// GetDriverNameByDriverID
        /// </summary>
        /// <param name="DriverId"></param>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public string GetDriverNameByDriverID(int DriverId, int OrganizationId)
        {
            string sqlString = null;
            try
            {
                string sql = "SELECT LTRIM(RTRIM(vlfDriver.FirstName)) + SPACE(1) + LTRIM(RTRIM(vlfDriver.LastName)) AS Name " +
                                 "From vlfDriver WHERE vlfDriver.DriverId = {0} and  vlfDriver.OrganizationId= {1} ";
                sql = string.Format(sql, DriverId, OrganizationId);
                DataSet sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                if (sqlDataSet != null && sqlDataSet.Tables.Count > 0 && sqlDataSet.Tables[0].Rows.Count > 0 &&
                    sqlDataSet.Tables[0].Rows[0]["Name"] != DBNull.Value)
                    sqlString = sqlDataSet.Tables[0].Rows[0]["Name"].ToString();
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Driver Name By DriverID =" + DriverId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Driver Name By DriverID =" + DriverId;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlString;
        }

        /// <summary>
        /// GetDriverEmergencyPhoneOrEmailByDriverID
        /// </summary>
        /// <param name="DriverId"></param>
        /// <param name="CommunicationTypeId"></param>
        /// <param name="isPhone"></param>
        /// <returns></returns>
        public DataSet GetDriverEmergencyPhoneOrEmailByDriverID(int DriverId, int CommunicationTypeId, Boolean isPhone, int OrganizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                string nameSql = "CASE c.IsCompany " +
                                    "WHEN 0 THEN isnull(c.FirstName, '') + ' ' + isnull(c.MiddleName, '') + ' ' + isnull(c.LastName, '') " +
                                    "ELSE c.Company " +
                                 "END AS Name";

                string sql = "";
                if (isPhone)
                {
                    sql = "SELECT LTRIM(RTRIM(vlfDriver.FirstName)) + SPACE(1) + LTRIM(RTRIM(vlfDriver.LastName)) AS Name, " +
                                 "-1 as Priority, -1 as contactCommunicationID, DriverContactPlan.EmergencyPhone as CommunicationData " +
                                 "From vlfDriver inner join DriverContactPlan ON DriverContactPlan.DriverId = vlfDriver.DriverId and " +
                                 "Ltrim(Rtrim(isnull(DriverContactPlan.EmergencyPhone, ''))) <> '' " +
                                 "WHERE vlfDriver.DriverId = {1} and  vlfDriver.OrganizationId= {3} " +
                                 "union " +
                           "(Select {0},a.Priority, b.contactCommunicationID, b.CommunicationData from DriverContactPlan  " +
                           "inner join ContactPlanCommunications a on a.ContactPlanId = DriverContactPlan.ContactPlanId " +
                           "inner join ContactCommunications b on a.ContactCommunicationID = b.ContactCommunicationID " +
                           "and b.CommunicationTypeId = {2} " +
                           "inner join ContactInfo c on c.ContactInfoId = b.ContactId and c.OrganizationId= {3} " +
                           "where DriverContactPlan.DriverId = {1} and  DriverContactPlan.ContactPlanId is not null) order by Priority ";
                }
                else
                {
                    sql = "Select {0},a.Priority, b.contactCommunicationID, b.CommunicationData from DriverContactPlan  " +
                           "inner join ContactPlanCommunications a on a.ContactPlanId = DriverContactPlan.ContactPlanId " +
                           "inner join ContactCommunications b on a.ContactCommunicationID = b.ContactCommunicationID " +
                           "and b.CommunicationTypeId = {2} " +
                           "inner join ContactInfo c on c.ContactInfoId = b.ContactId and c.OrganizationId= {3} " +
                           "where DriverContactPlan.DriverId = {1} and  DriverContactPlan.ContactPlanId is not null order by a.Priority ";


                }
                sql = string.Format(sql, nameSql, DriverId, CommunicationTypeId, OrganizationId);
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Driver Emergency Phones By DriverID =" + DriverId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Driver Emergency Phones By DriverID =" + DriverId;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Exchange ContactPlanCommunications Priority
        /// </summary>
        /// <param name="ContactPlanId"></param>
        /// <param name="ContactCommunicationID1"></param>
        /// <param name="ContactCommunicationID2"></param>
        /// <returns></returns>
        public int ContactPlanCommunicationsExchange_Priority(int ContactPlanId, Int64 ContactCommunicationID1, Int64 ContactCommunicationID2)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ContactPlanId", SqlDbType.Int, ContactPlanId);
                sqlExec.AddCommandParam("@ContactCommunicationID1", SqlDbType.BigInt, ContactCommunicationID1);
                sqlExec.AddCommandParam("@ContactCommunicationID2", SqlDbType.BigInt, ContactCommunicationID2);

                rowsAffected = sqlExec.SPExecuteNonQuery("[ContactPlanCommunicationsExchange_Priority]");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to exchange ContactPlanCommunications Priority, ContactPlanId=" + ContactPlanId + " ContactCommunicationID1=" + ContactCommunicationID1 + " ContactCommunicationID2" + ContactCommunicationID2;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to exchange ContactPlanCommunications Priority, ContactPlanId=" + ContactPlanId + " ContactCommunicationID1=" + ContactCommunicationID1 + " ContactCommunicationID2" + ContactCommunicationID2;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;

        }
        

        /// <summary>
        /// IsContactPlanInUsed
        /// </summary>
        /// <param name="ContactPlanId"></param>
        /// <returns></returns>
        public Boolean IsContactPlanInUsed(int ContactPlanId)
        {
            Boolean isContactPlanInUsed = false;
            try
            {
                string sql = "if exists(Select DriverContactPlanId from DriverContactPlan where ContactPlanId = {0} ) select 1; " +
                              "else select 0; ";

                DataSet ds = sqlExec.SQLExecuteDataset(string.Format(sql, ContactPlanId));
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                    {
                        if ((int)ds.Tables[0].Rows[0][0] == 1) isContactPlanInUsed = true;
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve IsContactPlanInUsed By ContactPlanId=" + ContactPlanId ;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve IsContactPlanInUsed By ContactPlanId=" + ContactPlanId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return isContactPlanInUsed;

        }

        /// <summary>
        /// GetTimeZones
        /// </summary>
        /// <returns></returns>
        public DataSet GetTimeZones()
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = "Select * from TimeZones Order By ID ";
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve TimeZones";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve TimeZones";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public string GetTimeZonesByID(string Id)
        {
            string timeZoneId = string.Empty;
            DataSet sqlDataSet = null;
            try
            {
                string sql = "Select TimeZoneId from TimeZones where Id = " + Id;
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                if (sqlDataSet != null && sqlDataSet.Tables[0].Rows.Count > 0)
                {
                    if (sqlDataSet.Tables[0].Rows[0]["TimeZoneId"] != DBNull.Value)
                    {
                        timeZoneId = sqlDataSet.Tables[0].Rows[0]["TimeZoneId"].ToString();
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetTimeZones By ID =  " + Id;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetTimeZones By ID =  " + Id;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return timeZoneId;

        }
    }
}
