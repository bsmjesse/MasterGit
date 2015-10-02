using System;
using System.Data ;			   // for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.Logic
{
    public class LandmarkPointSetManager : Das
    {
        private VLF.DAS.DB.LandmarkPointSet _landmark = null;
        public LandmarkPointSetManager(string connectionString)
            : base(connectionString)
        {
            _landmark = new VLF.DAS.DB.LandmarkPointSet(sqlExec);

        }
        public DataSet GetLandmarkPointSetByLandmarkId(long LandmarkId)
        {
            return _landmark.GetLandmarkPointSetByLandmarkId(LandmarkId);
        }

        public DataSet GetLandmarkPointSetByOrganizationId(int OrganizationId)
        {
            return _landmark.GetLandmarkPointSetByOrganizationId(OrganizationId);
        }

        // Changes for TimeZone Feature start

        public int vlfLandmarkPointSet_Add_NewTZ(int organizationId, string landmarkName, double latitude,
        double longitude, string description, string contactPersonName,
        string contactPhoneNum, int radius, string email, string phone, float timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
           string streetAddress, string pointSets, bool landmarkPublic, long categoryId)
        {
            return _landmark.vlfLandmarkPointSet_Add_NewTZ(organizationId, landmarkName, latitude,
            longitude, description, contactPersonName,
            contactPhoneNum, radius, email, phone, timeZone, dayLightSaving, autoAdjustDayLightSaving,
            streetAddress, pointSets, landmarkPublic, categoryId);
        }

        // Changes for TimeZone Feature end

        public int vlfLandmarkPointSet_Add(int organizationId, string landmarkName, double latitude,
         double longitude, string description, string contactPersonName,
         string contactPhoneNum, int radius, string email, string phone, short timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
            string streetAddress, string pointSets, bool landmarkPublic, long categoryId)
        {
            int rvResult = 0;

            try
            {
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

                rvResult = _landmark.vlfLandmarkPointSet_Add(organizationId, landmarkName, latitude,
                                longitude, description, contactPersonName,
                                contactPhoneNum, radius, email, phone, timeZone, dayLightSaving, autoAdjustDayLightSaving,
                                streetAddress, pointSets, landmarkPublic, categoryId);

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

        public int vlfLandmarkPointSet_Update_NewTZ(int organizationId, string currLandmarkName, string landmarkName, double latitude,
        double longitude, string description, string contactPersonName,
        string contactPhoneNum, int radius, string email, string phone, float timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
           string streetAddress, string pointSets, bool landmarkPublic, long categoryId)
        {
            return _landmark.vlfLandmarkPointSet_Update_NewTZ(organizationId, currLandmarkName, landmarkName, latitude,
            longitude, description, contactPersonName,
            contactPhoneNum, radius, email, phone, timeZone, dayLightSaving, autoAdjustDayLightSaving,
            streetAddress, pointSets, landmarkPublic, categoryId);
        }


        // Changes for TimeZone Feature end

        public int vlfLandmarkPointSet_Update(int organizationId, string currLandmarkName, string landmarkName, double latitude,
                                    double longitude, string description, string contactPersonName,
                                    string contactPhoneNum, int radius, string email, string phone, 
                                    short timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
                                    string streetAddress, string pointSets, bool landmarkPublic, long categoryId)
        {
            int rvResult = 0;

            try
            {
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

                rvResult = _landmark.vlfLandmarkPointSet_Update(organizationId, currLandmarkName, landmarkName, latitude,
                                    longitude, description, contactPersonName,
                                    contactPhoneNum, radius, email, phone, timeZone, 
                                    dayLightSaving, autoAdjustDayLightSaving,
                                    streetAddress, pointSets, landmarkPublic, categoryId);

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

        public int vlfLandmarkPointSet_Delete(int organizationId, string LandmarkName)
        {
            int rvResult = 0;

            try
            {
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                rvResult = _landmark.vlfLandmarkPointSet_Delete(organizationId, LandmarkName);
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

        public DataSet GetLandmarkPointSetByLandmarkName(string LandmarkName, int OrganizationId)
        {
            return _landmark.GetLandmarkPointSetByLandmarkName(LandmarkName, OrganizationId);
        }

        public long vlfLandmarkPostCommOnTheFly_Add(int OrganizationId, int UserId, int FleetId, DateTime Start,
            DateTime Finish, double Latitude, double Longitude, int Radius, string Email, string pointSets, string StreetAddress)
        {
            return _landmark.vlfLandmarkPostCommOnTheFly_Add(OrganizationId, UserId, FleetId, Start,
                   Finish, Latitude, Longitude, Radius, Email, pointSets, StreetAddress);
        }
    }
}