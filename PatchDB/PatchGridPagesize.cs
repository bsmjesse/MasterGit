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
    public class PatchGridPagesize : TblOneIntPrimaryKey
    {
        /// <summary>
        /// Provides interfaces to vlfServices table.
        /// </summary>
        public PatchGridPagesize(SQLExecuter sqlExec)
            : base("vlfGridPagesize", sqlExec)
        {
        }

        public int UpdateInsertPagesize(int organizationId, string ptype, int pagesize)
        {
            string sql = "GridPagesize_Insert_Update";
            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@pType", SqlDbType.VarChar, ptype);
            sqlExec.AddCommandParam("@pagesize", SqlDbType.Int, pagesize);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteNonQuery(sql);
        }

        public DataSet GetPagesizeSettingsByOrganizationId(int organizationId)
        {
            string sql = "SELECT * FROM vlfGridPagesize WHERE organizationId=" + organizationId.ToString();
            return sqlExec.SQLExecuteDataset(sql);
        }
    }
}
