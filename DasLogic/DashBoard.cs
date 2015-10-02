using System;
using System.Data;			   // for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.Logic 
{
    public class DashBoard : Das
    {
        private VLF.DAS.DB.DashBoard _dashboard = null;

        public DashBoard(string connectionString)
           : base(connectionString)
		{
            _dashboard = new VLF.DAS.DB.DashBoard(sqlExec);
        
        }


        public DataSet DashBoardPermissions_Get(int userId)
        {
            return _dashboard.DashBoardPermissions_Get(userId);
        }

        public int DashBoardPermissions_Delete(int DashboardId)
        {
            return _dashboard.DashBoardPermissions_Delete(DashboardId);
        }

        public int DashBoardPermissions_Add(int DashboardId, int OrganizationId, int UserGroupId, int UserId, string DashboardConfig)
        {
            return _dashboard.DashBoardPermissions_Add(DashboardId,  OrganizationId,  UserGroupId,  UserId,  DashboardConfig);
        }
    }
}
