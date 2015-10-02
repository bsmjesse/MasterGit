using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;


namespace VLF.DAS.DB
{
    public class DashBoard : TblGenInterfaces
    {
        public DashBoard(SQLExecuter sqlExec)
            : base("DashBoard", sqlExec)
        {
        }




        public DataSet DashBoardPermissions_Get(int userId)
        {


            DataSet sqlDataSet = null;

            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);

                sqlDataSet = sqlExec.SPExecuteDataset("DashBoardPermissions_Get");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Get Dashboard for User=" + userId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Get Dashboard for User=" + userId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return sqlDataSet;
        }


        public int DashBoardPermissions_Add(int DashboardId, int OrganizationId, int UserGroupId, int UserId, string DashboardConfig)
        {

            int rowsAffected = 0;
            try
            {

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DashboardId", SqlDbType.Int, DashboardId);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@UserGroupId", SqlDbType.Int, UserGroupId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);
                sqlExec.AddCommandParam("@DashboardConfig", SqlDbType.VarChar, DashboardConfig);

                rowsAffected = sqlExec.SPExecuteNonQuery("DashBoardPermissions_Add");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add DashBoard to user=" + UserId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add DashBoard to user=" + UserId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }


        public int DashBoardPermissions_Delete(int DashboardId)
        {
            
            int rowsAffected = 0;
            try
            {

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DashboardId", SqlDbType.Int, DashboardId);

                rowsAffected = sqlExec.SPExecuteNonQuery("DashBoardPermissions_Delete");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to delete DashBoard:  " + DashboardId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to delete DashBoard:  " + DashboardId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
      }
    }
}
    

