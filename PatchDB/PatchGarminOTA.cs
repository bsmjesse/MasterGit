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

    public class PatchGarminOTA : TblOneIntPrimaryKey
    {
        public PatchGarminOTA(SQLExecuter sqlExec)
            : base("vlfGarminOTA", sqlExec)
        {
        }

        public int SaveGarminOTA(int organizationId, int userId, string fileName, string path, long fileSize, string comments, string fileVersion)
        {
            string sql = "aota_AddGarminFile";
            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@FileName", SqlDbType.VarChar, fileName);
            sqlExec.AddCommandParam("@Path", SqlDbType.VarChar, path);
            sqlExec.AddCommandParam("@FileVersion", SqlDbType.VarChar, fileVersion);
            sqlExec.AddCommandParam("@FileSize", SqlDbType.Int, fileSize);
            sqlExec.AddCommandParam("@Comments", SqlDbType.Text, comments);
            
            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteNonQuery(sql);
        }

        public DataSet GetGarminFileByOrgId(int organizationId, int fileTypeId)
        {
            string sql = "aota_GetGarminFileByOrgId";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@orgId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@fileTypeId", SqlDbType.Int, fileTypeId);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteDataset(sql);
        }

        public DataSet GetGarminFileByFwId(int fwId)
        {
            string sql = "aota_GetGarminFileByFwId";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@FwId", SqlDbType.Int, fwId);            

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteDataset(sql);
        }

        public int DeleteGarminFile(int fileId)
        {
            //Prepares SQL statement
            string sql = "aota_GarminFileDelete";

            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@fileId", SqlDbType.Int, fileId);

            //Executes SQL statement
            return sqlExec.SPExecuteNonQuery(sql);
            
        }

        public int AddGarminFileTask(int organizationId, int fleetId, int fwId, int userId)
        {
            string sql = "aota_AddGarminFileTask";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
            sqlExec.AddCommandParam("@fwId", SqlDbType.Int, fwId);
            sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteNonQuery(sql);
        }

        public DataSet GetGarminFileFleetByFwId(int fwId, bool isHierarchy)
        {
            string sql = "aota_GetGarminFileFleetByFwId";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@FwId", SqlDbType.Int, fwId);
            sqlExec.AddCommandParam("@IsHierarchy", SqlDbType.Bit, isHierarchy);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteDataset(sql);
        }

        public int DeleteGarminFileTask(int organizationId, int fleetId, int fwId)
        {
            string sql = "aota_DeleteGarminFileTask";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
            sqlExec.AddCommandParam("@fwId", SqlDbType.Int, fwId);            

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteNonQuery(sql);
        }

        public DataSet GetGarminFileByFilenameVersion(int organizationId, string fileName, string fileVersion)
        {
            string sql = "aota_GetGarminFileByFilenameVersion";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@fileName", SqlDbType.VarChar, fileName);
            sqlExec.AddCommandParam("@fileVersion", SqlDbType.VarChar, fileVersion);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteDataset(sql);
        }
    }
}
