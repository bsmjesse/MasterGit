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
    public class PatchServices : TblOneIntPrimaryKey
    {
        /// <summary>
        /// Provides interfaces to vlfServices table.
        /// </summary>
        public PatchServices(SQLExecuter sqlExec) : base("vlfMapLayers", sqlExec)
        {
        }

        public DataSet GetAllServices()
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "SELECT * FROM vlfServices ORDER BY ServiceName";

                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve vlfServices";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vlfServices";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet GetAllAvailableServices(int organizationId, int landmarkId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = string.Format(@"SELECT * FROM vlfServices WHERE ServiceConfigId NOT IN (SELECT s.ServiceConfigId FROM vlfServices s
                                            INNER JOIN vlfServiceAssignment sa ON s.ServiceConfigId = sa.ServiceConfigId
                                            WHERE sa.OrganizationId={0} AND sa.LandmarkId={1}) ORDER BY ServiceName"
                                            , organizationId, landmarkId);

                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve vlfServices";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vlfServices";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet GetAppliedServicesByLandmarkId(int organizationId, int landmarkId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = string.Format(@"SELECT s.*, sa.Recepients, sa.Subjects FROM vlfServices s
                                            INNER JOIN vlfServiceAssignment sa ON s.ServiceConfigId = sa.ServiceConfigId
                                            WHERE sa.OrganizationId={0} AND sa.LandmarkId={1} ORDER BY ServiceName"
                                           , organizationId, landmarkId);

                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve vlfServices";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vlfServices";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public int AssignServiceToLandmark(int organizationId, int landmarkId, int serviceConfigId, string recepients, string subjects)
        {
            string sql = string.Format(@"INSERT INTO vlfServiceAssignment(ServiceConfigId, OrganizationId, LandmarkId, Recepients, Subjects) VALUES({0}, {1}, {2}, '{3}','{4}')",
                            serviceConfigId.ToString(), organizationId.ToString(), landmarkId.ToString(), recepients.Replace("'", "''"), subjects.Replace("'", "''"));
            return sqlExec.SQLExecuteNonQuery(sql);
        }

        public bool DeleteAssignmentByLandmarkId(int organizationId, int landmarkId)
        {
            string sql = string.Format(@"DELETE vlfServiceAssignment WHERE organizationId={0} AND LandmarkId={1}", organizationId, landmarkId);
            try
            {
                sqlExec.SQLExecuteNonQuery(sql);
                return true;
            }
            catch { return false; }
        }

        public DataSet GetAllRules()
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "SELECT * FROM vlfRules";

                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve vlfRules";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vlfRules";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public int InsertNewService(string name, string rules)
        {
            try
            {
                string sql = string.Format("INSERT INTO vlfServices(ServiceName, RulesApplied) VALUES('{0}','{1}')", name.Replace("'", "''"), rules);
                sqlExec.SQLExecuteNonQuery(sql);
                sql = "SELECT SCOPE_IDENTITY()";
                return int.Parse(sqlExec.SQLExecuteScalar(sql).ToString());
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve vlfRules";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vlfRules";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return 0;
        }

        public DataSet GetHardcodedCallTimerServices()
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "select * From vlfServices WHERE ServiceName like 'StopOver%'";

                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetHardcodedCallTimerServices";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetHardcodedCallTimerServices";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public int DeleteHardcodedCallTimerServices(int organizationId, int landmarkId)
        {
            try
            {
                string sql = "DELETE vlfServiceAssignment WHERE OrganizationId=" + organizationId.ToString() + " AND LandmarkId=" + landmarkId.ToString() + " AND ServiceConfigId IN (SELECT ServiceConfigId FROM vlfServices WHERE ServiceName like 'StopOver%')";
                return sqlExec.SQLExecuteNonQuery(sql);                
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to DeleteHardcodedCallTimerServices";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to DeleteHardcodedCallTimerServices";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return 0;
        }

        public int GetHardcodedTimerServiceId(int organizationId, int landmarkId)
        {
            try
            {
                string sql = "SELECT TOP 1 ServiceConfigId FROM vlfServiceAssignment WHERE OrganizationId=" + organizationId.ToString() + " AND LandmarkId=" + landmarkId.ToString() + " AND ServiceConfigId IN (SELECT ServiceConfigId FROM vlfServices WHERE ServiceName like 'StopOver%')";
                DataSet dt = sqlExec.SQLExecuteDataset(sql);
                if (dt.Tables[0].Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Tables[0].Rows[0][0].ToString());
                }
                return 0;
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to DeleteHardcodedCallTimerServices";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to DeleteHardcodedCallTimerServices";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return 0;
        }
    }
}
