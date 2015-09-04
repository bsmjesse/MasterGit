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
    public class PatchGridPagesize : VLF.DAS.Das
    {
        private VLF.PATCH.DB.PatchGridPagesize _gridpagesize = null;

        public PatchGridPagesize(string connectionString)
           : base(connectionString)
		{
            _gridpagesize = new VLF.PATCH.DB.PatchGridPagesize(sqlExec);        
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        public int UpdateInsertPagesize(int organizationId, string ptype, int pagesize)
        {
            return _gridpagesize.UpdateInsertPagesize(organizationId, ptype, pagesize);
        }

        public DataSet GetPagesizeSettingsByOrganizationId(int organizationId)
        {
            return _gridpagesize.GetPagesizeSettingsByOrganizationId(organizationId);
        }
    }
}
