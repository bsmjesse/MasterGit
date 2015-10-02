using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using VLF.DAS;
using VLF.DAS.DB;

namespace VLF.PATCH.DB
{
    public class PatchScheduledReports : TblOneIntPrimaryKey
    {
        public PatchScheduledReports(SQLExecuter sqlExec)
            : base("vlfReportSchedules", sqlExec)
        {
        }

        public DataSet GetScheduledReportsVehicleEmail(int organizationId, string email)
        {
            string sql = "GetScheduledReportsVehicleEmail";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@email", SqlDbType.VarChar, email);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteDataset(sql);
        }

        public DataSet GetScheduledReportsCostCenterFleetEmail(int organizationId, string email)
        {
            string sql = "GetScheduledReportsCostCenterFleetEmail";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@email", SqlDbType.VarChar, email);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteDataset(sql);
        }

        public DataSet GetScheduledReports(int organizationId, string email)
        {
            string sql = "GetScheduledReports";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@email", SqlDbType.VarChar, email);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteDataset(sql);
        }

        public DataSet GetVehicleEmailByVehicle(int organizationId, int vehicleType, string searchParam)
        {
            string sql = "GetVehicleEmailByVehicle";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@vehicleType", SqlDbType.Int, vehicleType);
            sqlExec.AddCommandParam("@searchParam", SqlDbType.VarChar, searchParam);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteDataset(sql);
        }

        public DataSet GetCosterFleetEmailByVehicle(int organizationId, int vehicleType, string searchParam)
        {
            string sql = "GetCosterFleetEmailByVehicle";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@vehicleType", SqlDbType.Int, vehicleType);
            sqlExec.AddCommandParam("@searchParam", SqlDbType.VarChar, searchParam);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteDataset(sql);
        }

        public DataSet GetScheduledReportsByVehicle(int organizationId, int vehicleType, string searchParam)
        {
            string sql = "GetScheduledReportsByVehicle";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@vehicleType", SqlDbType.Int, vehicleType);
            sqlExec.AddCommandParam("@searchParam", SqlDbType.VarChar, searchParam);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteDataset(sql);
        }
    }
}
