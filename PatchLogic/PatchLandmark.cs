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
    public class PatchLandmark : VLF.DAS.Das
    {
        private VLF.PATCH.DB.PatchLandmark _landmark = null;

        public PatchLandmark(string connectionString)
            : base(connectionString)
        {
            _landmark = new VLF.PATCH.DB.PatchLandmark(sqlExec);
        }

        public new void Dispose()
        {
            base.Dispose();
        }
        // Changes for TimeZone Feature start

        /// <summary>
        /// Retrieves public or private landmarks info by organization id, user id
        /// </summary>
        /// <returns>
        /// DataSet [OrganizationId],[LandmarkName],[Latitude],[Longitude],
        /// [Description],[ContactPersonName],[ContactPhoneNum],[Radius],
        /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
        /// [StreetAddress]
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet PatchGetLandmarksInfoByOrganizationIdUserId_NewTZ(int organizationId, int userId)
        {
            DataSet dsResult = _landmark.PatchGetLandmarksInfoByOrganizationIdUserId_NewTZ(organizationId, userId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LandmarksInformation";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }



        // Changes for TimeZone Feature end
      
        
        /// <summary>
        /// Retrieves public or private landmarks info by organization id, user id
        /// </summary>
        /// <returns>
        /// DataSet [OrganizationId],[LandmarkName],[Latitude],[Longitude],
        /// [Description],[ContactPersonName],[ContactPhoneNum],[Radius],
        /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
        /// [StreetAddress]
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet PatchGetLandmarksInfoByOrganizationIdUserId(int organizationId, int userId)
        {
            DataSet dsResult = _landmark.PatchGetLandmarksInfoByOrganizationIdUserId(organizationId, userId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LandmarksInformation";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        // Changes for TimeZone Feature start
        public DataSet PatchGetLandmarksInfoWithPointsByOrganizationIdUserId_NewTZ(int organizationId, int userId)
        {
            return PatchGetLandmarksInfoWithPointsByOrganizationIdUserId_NewTZ(organizationId, userId, 0);
        }


        // Changes for TimeZone Feature end

        public DataSet PatchGetLandmarksInfoWithPointsByOrganizationIdUserId(int organizationId, int userId)
        {
            return PatchGetLandmarksInfoWithPointsByOrganizationIdUserId(organizationId, userId, 0);
        }

        // Changes for TimeZone Feature start
        public DataSet PatchGetLandmarksInfoWithPointsByOrganizationIdUserId_NewTZ(int organizationId, int userId, long categoryId)
        {
            DataSet dsResult = _landmark.PatchGetLandmarksInfoWithPointsByOrganizationIdUserId_NewTZ(organizationId, userId, categoryId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LandmarksInformation";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }


        // Changes for TimeZone Feature end

        public DataSet PatchGetLandmarksInfoWithPointsByOrganizationIdUserId(int organizationId, int userId, long categoryId)
        {
            DataSet dsResult = _landmark.PatchGetLandmarksInfoWithPointsByOrganizationIdUserId(organizationId, userId, categoryId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LandmarksInformation";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        public DataSet GetAllUnassignedToFleetLandmarksInfo(int organizationId, int fleetId, int userId)
        {
            return _landmark.GetAllUnassignedToFleetLandmarksInfo(organizationId, fleetId, userId);
        }

        public DataSet GetAllAssignedToFleetLandmarksInfo(int organizationId, int fleetId)
        {
            return _landmark.GetAllAssignedToFleetLandmarksInfo(organizationId, fleetId);
        }

        public int AssignLandmarkToFleet(int organizationId, int landmarkId, int fleetId)
        {
            return _landmark.AssignLandmarkToFleet(organizationId, landmarkId, fleetId);
        }

        public int UnassignLandmarkFromFleet(int organizationId, int landmarkId, int fleetId)
        {
            return _landmark.UnassignLandmarkFromFleet(organizationId, landmarkId, fleetId);
        }

        public int AssignLandmarkToFleetByLandmarkName(int organizationId, string landmarkName, int fleetId)
        {
            int landmarkId = _landmark.GetLandmarkIdByLandmarkName(organizationId, landmarkName);
            return _landmark.AssignLandmarkToFleet(organizationId, landmarkId, fleetId);
        }

        public int GetLandmarkIdByLandmarkName(int organizationId, string LandmarkName)
        {
            return _landmark.GetLandmarkIdByLandmarkName(organizationId, LandmarkName);
        }

        public int UnassignObjectFromAllFleets(string objectName, int objectId)
        {
            return _landmark.UnassignObjectFromAllFleets(objectName, objectId);
        }

        public bool IfObjectAssignedToFleet(int fleetId, string objectName, int objectId)
        {
            return _landmark.IfObjectAssignedToFleet(fleetId, objectName, objectId);
        }

        public int AssignObjectToFleet(int organizationId, int objectId, int fleetId, string objectName)
        {
            return _landmark.AssignObjectToFleet(organizationId, objectId, fleetId, objectName);
        }

        public int UpdateLandmarkCreater(int landmarkId, int userId)
        {
            return _landmark.UpdateLandmarkCreater(landmarkId, userId);
        }
    }
}
