using System;
using System.Data ;			   // for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.Logic
{
    public class ContactManager : Das
    {
        public static int EmergencyPhoneId = 1;
        public static int EmailId = 7;

        private VLF.DAS.DB.Contact _contact = null;

        public ContactManager(string connectionString)
            : base(connectionString)
		{
            _contact = new VLF.DAS.DB.Contact(sqlExec);
        
        }

        public int ContactInfo_Add(int OrganizationId, Boolean IsCompany, string Company, string FirstName,
          string MiddleName, string LastName, int TimeZone, string Contacts)
        {
            return _contact.ContactInfo_Add(OrganizationId, IsCompany, Company, FirstName,
                    MiddleName, LastName, TimeZone, Contacts);
        }

        public int ContactInfo_Update(Int64 ContactInfoId, Boolean IsCompany, string Company, string FirstName,
          string MiddleName, string LastName, int TimeZone, string DeletedIds, string Contacts, int OrganizationId)
        {
            return _contact.ContactInfo_Update(ContactInfoId, IsCompany, Company, FirstName,
                MiddleName, LastName, TimeZone, DeletedIds, Contacts, OrganizationId);
        }


        /// <summary>
        /// GetCommunicationTypes 
        /// </summary>
        /// <returns></returns>
        public DataSet GetCommunicationTypes()
        {
            return _contact.GetCommunicationTypes();
        }

        /// <summary>
        /// Get Organization Contacts
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationContacts(int OrganizationId)
        {
            return _contact.GetOrganizationContacts(OrganizationId);
        }

        /// <summary>
        /// Get Vehicle Contact Communications By ContactId
        /// </summary>
        /// <param name="ContactId"></param>
        /// <returns></returns>
        public DataSet GetVehicleContactCommunicationsByContactId(Int64 ContactId)
        {
            return _contact.GetVehicleContactCommunicationsByContactId(ContactId);
        }

        /// <summary>
        /// Delete ContactInfo
        /// </summary>
        /// <param name="ContactInfoId"></param>
        /// <returns></returns>
        public int ContactInfo_Delete(Int64 ContactInfoId)
        {
            return _contact.ContactInfo_Delete(ContactInfoId);
        }
        /// <summary>
        /// Get Organization Contact Plans
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationContactPlan(int OrganizationId)
        {
            return _contact.GetOrganizationContactPlan(OrganizationId);
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
            return _contact.ContactPlan_Add(ContactPlanName, OrganizationId, ContactCommunicationIDs);
        }

                /// <summary>
        /// Get Contact Communication Data By Organization
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="CommunicationTypeId"></param>
        /// <returns></returns>
        public DataSet GetContactCommunicationDataByOrganization(int OrganizationId, int CommunicationTypeId)
        {
            return _contact.GetContactCommunicationDataByOrganization(OrganizationId, CommunicationTypeId);
        }

        public DataSet GetVehicleContactCommunicationsByContactIdOrgID(Int64 ContactId, int OrganizationId)
        {
            return _contact.GetVehicleContactCommunicationsByContactIdOrgID(ContactId, OrganizationId);
        }

        /// <summary>
        /// Get Communication Data By contactCommunicationID
        /// </summary>
        /// <param name="contactCommunicationID"></param>
        /// <param name="CommunicationTypeId"></param>
        /// <returns></returns>
        public DataSet GetCommunicationDataByID(Int64 contactCommunicationID, int CommunicationTypeId, int OrganizationId)
        {
            return _contact.GetCommunicationDataByID(contactCommunicationID, CommunicationTypeId, OrganizationId);
        }

        /// <summary>
        /// Get Plan Communication Data By ContactPlanId
        /// </summary>
        /// <param name="ContactPlanId"></param>
        /// <returns></returns>
        public DataSet GetPlanCommunicationDataByContactPlanId(int ContactPlanId, int CommunicationTypeId)
        {
            return _contact.GetPlanCommunicationDataByContactPlanId(ContactPlanId, CommunicationTypeId);
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
            return _contact.GetContactCommunicationDataByOrganization(OrganizationId, CommunicationTypeId, ContactPlanId);
        }

        /// <summary>
        /// Delete ContactPlan
        /// </summary>
        /// <param name="ContactPlanId"></param>
        /// <returns></returns>
        public int ContactPlan_Delete(int ContactPlanId)
        {
            return _contact.ContactPlan_Delete(ContactPlanId);

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
            return _contact.ContactPlanCommunications_Delete(ContactPlanId, ContactCommunicationID);
        }

        /// <summary>
        /// Add ContactPlanCommunications
        /// </summary>
        /// <param name="ContactPlanId"></param>
        /// <param name="ContactCommunicationIDs"></param>
        /// <returns></returns>
        public int ContactPlanCommunications_Add(int ContactPlanId, string ContactCommunicationIDs)
        {
            return _contact.ContactPlanCommunications_Add(ContactPlanId, ContactCommunicationIDs);
        }

        /// <summary>
        /// IsContactInUsed
        /// </summary>
        /// <param name="ContactInfoId"></param>
        /// <returns></returns>
        public Boolean IsContactInUsed(Int64 ContactInfoId)
        {
            return _contact.IsContactInUsed(ContactInfoId);
        }

        /// <summary>
        /// IsContactCommunicationIDsInOrganization
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="ContactCommunicationIDs"></param>
        /// <returns></returns>
        public Boolean IsContactCommunicationIDsInOrganization(int OrganizationId, string ContactCommunicationIDs)
        {
            return _contact.IsContactCommunicationIDsInOrganization(OrganizationId, ContactCommunicationIDs);
        }

        /// <summary>
        /// GetOrganizationDriverContactPlan(int OrganizationId)
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationDriverContactPlan(int OrganizationId, int DriverId)
        {
            return _contact.GetOrganizationDriverContactPlan(OrganizationId, DriverId);
        }

        public int DriverAndContactPlanAdd(string firstName, string lastName, string license, string classLicense, DateTime? licenseIssued,
                DateTime? licenseExpired, int orgId, char gender, short height, string homePhone, string cellPhone, string additionalPhone,
                string smsPwd,
                string smsid, string email, string address, string city, string zipcode, string state, string country, string description,
                string EmergencyPhone, int? ContactPlanId, Boolean IsAssignPanic, string KeyFobId)
        {
            return _contact.DriverAndContactPlanAdd(firstName, lastName, license, classLicense, licenseIssued,
                licenseExpired, orgId, gender, height, homePhone, cellPhone, additionalPhone,
                smsPwd,
                smsid, email, address, city, zipcode, state, country, description,
                EmergencyPhone, ContactPlanId, IsAssignPanic, KeyFobId);

        }
        /// <summary>
        /// Update DriverContactPlan
        /// </summary>
        /// <param name="DriverContactPlanId"></param>
        /// <param name="DriverId"></param>
        /// <param name="EmergencyP
        /// hone"></param>
        /// <param name="ContactPlanId"></param>
        /// <param name="IsAssignPanic"></param>
        /// <returns></returns>
        public int DriverAndContactPlanUpdate(int driverId, string firstName, string lastName, string license, string classLicense, DateTime? licenseIssued,
                        DateTime? licenseExpired, int orgId, char gender, short height, string homePhone, string cellPhone, string additionalPhone,
                        string smsPwd,
                        string smsid, string email, string address, string city, string zipcode, string state, string country, string description,
                        string EmergencyPhone, int? ContactPlanId, Boolean IsAssignPanic, string KeyFobId)
        {
            return _contact.DriverAndContactPlanUpdate(driverId, firstName, lastName, license, classLicense, licenseIssued,
                        licenseExpired, orgId, gender, height, homePhone, cellPhone, additionalPhone,
                        smsPwd,
                        smsid, email, address, city, zipcode, state, country, description,
                        EmergencyPhone, ContactPlanId, IsAssignPanic, KeyFobId);
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
            return _contact.CheckIfDriverUnique(DriverId, OrganizationID, FirstName, LastName, smsid);
        }

        public int DriverAndContactPlanCycleAdd(string firstName, string lastName, string license, string classLicense, DateTime? licenseIssued,
                DateTime? licenseExpired, int orgId, char gender, short height, string homePhone, string cellPhone, string additionalPhone,
                string smsPwd,
                string smsid, string email, string address, string city, string zipcode, string state, string country, string description,
                string EmergencyPhone, int? ContactPlanId, Boolean IsAssignPanic, string KeyFobId,
                int? caCycle,
                int? usCycle,
                Boolean isSupervisor,
                string positionInfo,
                float? timzone = null
)
        {
            return _contact.DriverAndContactPlanCycleAdd(firstName, lastName, license, classLicense, licenseIssued,
                licenseExpired, orgId, gender, height, homePhone, cellPhone, additionalPhone,
                smsPwd,
                smsid, email, address, city, zipcode, state, country, description,
                EmergencyPhone, ContactPlanId, IsAssignPanic, KeyFobId, caCycle, usCycle, isSupervisor, positionInfo, timzone);
        }
        /// <summary>
        /// Update DriverContactPlan
        /// </summary>
        /// <param name="DriverContactPlanId"></param>
        /// <param name="DriverId"></param>
        /// <param name="EmergencyP
        /// hone"></param>
        /// <param name="ContactPlanId"></param>
        /// <param name="IsAssignPanic"></param>
        /// <returns></returns>
        public int DriverAndContactPlanCycleUpdate(int driverId, string firstName, string lastName, string license, string classLicense, DateTime? licenseIssued,
                        DateTime? licenseExpired, int orgId, char gender, short height, string homePhone, string cellPhone, string additionalPhone,
                        string smsPwd,
                        string smsid, string email, string address, string city, string zipcode, string state, string country, string description,
                        string EmergencyPhone, int? ContactPlanId, Boolean IsAssignPanic, string KeyFobId,
                        int? caCycle, int? usCycle, Boolean isSupervisor, string positionInfo, DateTime? terminationDate,
                        float? timzone = null
)
        {
            return _contact.DriverAndContactPlanCycleUpdate(driverId, firstName, lastName, license, classLicense, licenseIssued,
                        licenseExpired, orgId, gender, height, homePhone, cellPhone, additionalPhone,
                        smsPwd,
                        smsid, email, address, city, zipcode, state, country, description,
                        EmergencyPhone, ContactPlanId, IsAssignPanic, KeyFobId, caCycle, usCycle, isSupervisor, positionInfo, terminationDate, timzone);
        }    
        /// <summary>
        /// Delete Driver And ContactPlan
        /// </summary>
        /// <param name="driverId"></param>
        /// <returns></returns>
        public int DriverAndContactPlanDelete(int driverId)
        {
            return _contact.DriverAndContactPlanDelete(driverId);
        }

        /// <summary>
        /// Get Drivers by OrganizationId
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetDriversEmployeeInfo(int OrganizationId)
        {

            return _contact.GetDriversEmployeeInfo(OrganizationId);
        }

        /// <summary>
        /// Get Drivers by OrganizationId
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationDrivers(int OrganizationId)
        {

            return _contact.GetOrganizationDrivers(OrganizationId);
        }

        /// <summary>
        /// Get Driver by DriverID
        /// </summary>
        /// <param name="DriverId"></param>
        /// <returns></returns>
        public DataSet GetDriverByDriverID(int DriverId)
        {
            return _contact.GetDriverByDriverID(DriverId);
        }

        /// <summary>
        /// GetDriverEmergencyPhoneOrEmailByDriverID
        /// </summary>
        /// <param name="DriverId"></param>
        /// <param name="CommunicationTypeId"></param>
        /// <param name="isPhone"></param>
        /// <returns></returns>
        public DataSet GetDriverEmergencyPhoneOrEmailByDriverID(int DriverId, Boolean isPhone, int OrganizationId)
        {
            int communicationTypeId = 0;
            if (isPhone) communicationTypeId = EmergencyPhoneId;
            else communicationTypeId = EmailId;
            return _contact.GetDriverEmergencyPhoneOrEmailByDriverID(DriverId, communicationTypeId, isPhone, OrganizationId);
        }

        public string GetDriverNameByDriverID(int DriverId, int OrganizationId)
        {
            return _contact.GetDriverNameByDriverID(DriverId, OrganizationId);
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
            return _contact.ContactPlanCommunicationsExchange_Priority(ContactPlanId, ContactCommunicationID1, ContactCommunicationID2);
        }

        /// <summary>
        /// IsContactPlanInUsed
        /// </summary>
        /// <param name="ContactPlanId"></param>
        /// <returns></returns>
        public Boolean IsContactPlanInUsed(int ContactPlanId)
        {
            return _contact.IsContactPlanInUsed(ContactPlanId);
        }
        /// <summary>
        /// GetTimeZones
        /// </summary>
        /// <returns></returns>
        public DataSet GetTimeZones()
        {
            return _contact.GetTimeZones();
        }

        public string GetTimeZonesByID(string Id) {
            return _contact.GetTimeZonesByID(Id);
        }
    }
}
