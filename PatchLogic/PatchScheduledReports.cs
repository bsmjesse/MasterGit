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
    public class PatchScheduledReports : VLF.DAS.Das
    {
        private VLF.PATCH.DB.PatchScheduledReports _scheduledReports = null;

        public PatchScheduledReports(string connectionString)
           : base(connectionString)
		{
            _scheduledReports = new VLF.PATCH.DB.PatchScheduledReports(sqlExec);        
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        public DataSet GetScheduledReportsVehicleEmail(int organizationId, string email)
        {
            return _scheduledReports.GetScheduledReportsVehicleEmail(organizationId, email);
        }

        public DataSet GetScheduledReportsCostCenterFleetEmail(int organizationId, string email)
        {
            return _scheduledReports.GetScheduledReportsCostCenterFleetEmail(organizationId, email);
        }

        public DataSet GetScheduledReports(int organizationId, string email)
        {
            return _scheduledReports.GetScheduledReports(organizationId, email);
        }

        public DataSet GetVehicleEmailByVehicle(int organizationId, int vehicleType, string searchParam)
        {
            return _scheduledReports.GetVehicleEmailByVehicle(organizationId, vehicleType, searchParam);
        }

        public DataSet GetCosterFleetEmailByVehicle(int organizationId, int vehicleType, string searchParam)
        {
            return _scheduledReports.GetCosterFleetEmailByVehicle(organizationId, vehicleType, searchParam);
        }

        public DataSet GetScheduledReportsByVehicle(int organizationId, int vehicleType, string searchParam)
        {
            return _scheduledReports.GetScheduledReportsByVehicle(organizationId, vehicleType, searchParam);
        }
    }
}
