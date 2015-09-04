using System;
using System.Data;			   // for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.DAS.DB;
using VLF.PATCH.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.PATCH.Logic
{
    public class PatchLandmarkPointSet : VLF.DAS.Das
    {
        private VLF.PATCH.DB.PatchLandmarkPointSet _landmark = null;

        public PatchLandmarkPointSet(string connectionString)
            : base(connectionString)
        {
            _landmark = new VLF.PATCH.DB.PatchLandmarkPointSet(sqlExec);            
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        public DataSet GetLandmarkPointSetByLandmarkId(long LandmarkId)
        {
            return _landmark.PatchGetLandmarkPointSetByLandmarkId(LandmarkId);
        }

        public DataSet PatchGetLandmarkPointSetByOrganizationId(int OrganizationId)
        {
            return _landmark.PatchGetLandmarkPointSetByOrganizationId(OrganizationId);
        }

        // Changes for TimeZone Feature start
        public int PatchVlfLandmarkPointSet_Add_NewTZ(int organizationId, string landmarkName, double latitude,
         double longitude, string description, string contactPersonName,
         string contactPhoneNum, int radius, string email, string phone, float timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
            string streetAddress, string pointSets, int createUserID, bool isPublic, long categoryId)
        {
            return _landmark.PatchVlfLandmarkPointSet_Add_NewTZ(organizationId, landmarkName, latitude,
            longitude, description, contactPersonName,
            contactPhoneNum, radius, email, phone, timeZone, dayLightSaving, autoAdjustDayLightSaving,
            streetAddress, pointSets, createUserID, isPublic, categoryId);
        }



        // Changes for TimeZone Feature end

        public int PatchVlfLandmarkPointSet_Add(int organizationId, string landmarkName, double latitude,
         double longitude, string description, string contactPersonName,
         string contactPhoneNum, int radius, string email, string phone, short timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
            string streetAddress, string pointSets, int createUserID, bool isPublic, long categoryId)
        {
            int rvResult = 0;

            try
            {
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

                rvResult = _landmark.PatchVlfLandmarkPointSet_Add(organizationId, landmarkName, latitude,
                            longitude, description, contactPersonName,
                            contactPhoneNum, radius, email, phone, timeZone, dayLightSaving, autoAdjustDayLightSaving,
                            streetAddress, pointSets, createUserID, isPublic, categoryId);

                sqlExec.CommitTransaction();
            }
            catch (Exception ex)
            {
                // Rollback all changes
                sqlExec.RollbackTransaction();
                throw;
            }

            return rvResult;
        }

        // Changes for TimeZone Feature start
        public int PatchVlfLandmarkPointSet_Update_NewTZ(int organizationId, string currLandmarkName, string landmarkName, double latitude,
                                                    double longitude, string description, string contactPersonName,
                                                    string contactPhoneNum, int radius, string email, string phone, float timeZone,
                                                    bool dayLightSaving, bool autoAdjustDayLightSaving,
                                                    string streetAddress, string pointSets, int createUserID, bool isPublic)
        {
            return _landmark.PatchVlfLandmarkPointSet_Update_NewTZ(organizationId, currLandmarkName, landmarkName, latitude,
                                                    longitude, description, contactPersonName,
                                                    contactPhoneNum, radius, email, phone, timeZone,
                                                    dayLightSaving, autoAdjustDayLightSaving,
                                                    streetAddress, pointSets, createUserID, isPublic);
        }


        // Changes for TimeZone Feature end

        public int PatchVlfLandmarkPointSet_Update(int organizationId, string currLandmarkName, string landmarkName, double latitude,
                                                     double longitude, string description, string contactPersonName,
                                                     string contactPhoneNum, int radius, string email, string phone, short timeZone, 
                                                     bool dayLightSaving, bool autoAdjustDayLightSaving,
                                                     string streetAddress, string pointSets, int createUserID, bool isPublic)
        {
            return _landmark.PatchVlfLandmarkPointSet_Update(organizationId, currLandmarkName, landmarkName, latitude,
                                                    longitude, description, contactPersonName,
                                                    contactPhoneNum, radius, email, phone, timeZone, 
                                                    dayLightSaving, autoAdjustDayLightSaving,
                                                    streetAddress, pointSets, createUserID, isPublic);
        }

        // Changes for TimeZone Feature start
        // This method would update landmark's meta data only. It would not update point sets.
        public int PatchVlfLandmarkMetaData_Update_NewTZ(int organizationId, string currLandmarkName, string landmarkName, double latitude,
                                                     double longitude, string description, string contactPersonName,
                                                     string contactPhoneNum, int radius, string email, string phone, float timeZone,
                                                     bool dayLightSaving, bool autoAdjustDayLightSaving,
                                                     string streetAddress, int createUserID, bool isPublic, long landmarkCategoryID)
        {
            return _landmark.PatchVlfLandmarkMetaData_Update_NewTZ(organizationId, currLandmarkName, landmarkName, latitude,
                                                    longitude, description, contactPersonName,
                                                    contactPhoneNum, radius, email, phone, timeZone,
                                                    dayLightSaving, autoAdjustDayLightSaving,
                                                    streetAddress, createUserID, isPublic, landmarkCategoryID);
        }


        // Changes for TimeZone Feature end

        // This method would update landmark's meta data only. It would not update point sets.
        public int PatchVlfLandmarkMetaData_Update(int organizationId, string currLandmarkName, string landmarkName, double latitude,
                                                     double longitude, string description, string contactPersonName,
                                                     string contactPhoneNum, int radius, string email, string phone, short timeZone,
                                                     bool dayLightSaving, bool autoAdjustDayLightSaving,
                                                     string streetAddress, int createUserID, bool isPublic, long landmarkCategoryID)
        {
            int rvResult = 0;
            try
            {
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

                rvResult = _landmark.PatchVlfLandmarkMetaData_Update(organizationId, currLandmarkName, landmarkName, latitude,
                                                    longitude, description, contactPersonName,
                                                    contactPhoneNum, radius, email, phone, timeZone,
                                                    dayLightSaving, autoAdjustDayLightSaving,
                                                    streetAddress, createUserID, isPublic, landmarkCategoryID);

                sqlExec.CommitTransaction();
            }
            catch (Exception ex)
            {
                // Rollback all changes
                sqlExec.RollbackTransaction();
                throw;
            }

            return rvResult;
        }

        public int PatchVlfLandmarkPointSet_Delete(int organizationId, string LandmarkName)
        {
            int rvResult = 0;

            try
            {
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                rvResult = _landmark.PatchVlfLandmarkPointSet_Delete(organizationId, LandmarkName);
                sqlExec.CommitTransaction();
            }
            catch (Exception ex)
            {
                // Rollback all changes
                sqlExec.RollbackTransaction();
                throw;
            }

            return rvResult;
        }

        public DataSet PatchGetLandmarkPointSetByLandmarkName(string LandmarkName, int OrganizationId)
        {
            return _landmark.PatchGetLandmarkPointSetByLandmarkName(LandmarkName, OrganizationId);
        }

        public long PatchVlfLandmarkPostCommOnTheFly_Add(int OrganizationId, int UserId, int FleetId, DateTime Start,
            DateTime Finish, double Latitude, double Longitude, int Radius, string Email, string pointSets, string StreetAddress)
        {
            return _landmark.PatchVlfLandmarkPostCommOnTheFly_Add(OrganizationId, UserId, FleetId, Start,
                   Finish, Latitude, Longitude, Radius, Email, pointSets, StreetAddress);
        }

        public int PatchGetLandmarkIdByLandmarkName(int organizationId, string landmarkName)
        {
            return _landmark.PatchGetLandmarkIdByLandmarkName(organizationId, landmarkName);
        }
    }
}
