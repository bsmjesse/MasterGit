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
    public class PatchGarminOTA : VLF.DAS.Das
    {
        private VLF.PATCH.DB.PatchGarminOTA _ota = null;

        public PatchGarminOTA(string connectionString)
           : base(connectionString)
		{
            _ota = new VLF.PATCH.DB.PatchGarminOTA(sqlExec);        
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        public int SaveGarminOTA(int organizationId, int userId, string fileName, string path, long fileSize, string comments, string fileVersion)
        {
            return _ota.SaveGarminOTA(organizationId, userId, fileName, path, fileSize, comments, fileVersion);
        }

        public DataSet GetGarminFileByOrgId(int organizationId, int fileTypeId)
        {
            return _ota.GetGarminFileByOrgId(organizationId, fileTypeId);
        }

        public DataSet GetGarminFileByFwId(int fwId)
        {
            return _ota.GetGarminFileByFwId(fwId);
        }

        public int DeleteGarminFile(int fileId)
        {
            return _ota.DeleteGarminFile(fileId);
        }

        public int AddGarminFileTask(int organizationId, int fleetId, int fwId, int userId)
        {
            return _ota.AddGarminFileTask(organizationId, fleetId, fwId, userId);
        }

        public DataSet GetGarminFileFleetByFwId(int fwId, bool isHierarchy)
        {
            return _ota.GetGarminFileFleetByFwId(fwId, isHierarchy);
        }

        public int DeleteGarminFileTask(int organizationId, int fleetId, int fwId)
        {
            return _ota.DeleteGarminFileTask(organizationId, fleetId, fwId);
        }
        public DataSet GetGarminFileByFilenameVersion(int organizationId, string fileName, string fileVersion)
        {
            return _ota.GetGarminFileByFilenameVersion(organizationId, fileName, fileVersion);
        }
    }
}
