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
    public class PatchServices : VLF.DAS.Das
    {
        private VLF.PATCH.DB.PatchServices _services = null;

        public PatchServices(string connectionString)
           : base(connectionString)
		{
            _services = new VLF.PATCH.DB.PatchServices(sqlExec);
        
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// GetAllServices
        /// </summary>
        /// <returns></returns>
        public DataSet GetAllServices()
        {
            return _services.GetAllServices();
        }

        public DataSet GetAllAvailableServices(int organizationId, int landmarkId)
        {
            return _services.GetAllAvailableServices(organizationId, landmarkId);
        }

        public DataSet GetAppliedServicesByLandmarkId(int organizationId, int landmarkId)
        {
            return _services.GetAppliedServicesByLandmarkId(organizationId, landmarkId);
        }

        public int AssignServiceToLandmark(int organizationId, int landmarkId, int serviceConfigId, string recepients, string subjects)
        {
            return _services.AssignServiceToLandmark(organizationId, landmarkId, serviceConfigId, recepients, subjects);
        }

        public bool DeleteAssignmentByLandmarkId(int organizationId, int landmarkId)
        {
            return _services.DeleteAssignmentByLandmarkId(organizationId, landmarkId);
        }

        public DataSet GetAllRules()
        {
            return _services.GetAllRules();
        }

        public int InsertNewService(string name, string rules)
        {
            return _services.InsertNewService(name, rules);
        }

        public DataSet GetHardcodedCallTimerServices()
        {
            return _services.GetHardcodedCallTimerServices();
        }

        public int DeleteHardcodedCallTimerServices(int organizationId, int landmarkId)
        {
            return _services.DeleteHardcodedCallTimerServices(organizationId, landmarkId);
        }

        public int GetHardcodedTimerServiceId(int organizationId, int landmarkId)
        {
            return _services.GetHardcodedTimerServiceId(organizationId, landmarkId);
        }
    }
}
