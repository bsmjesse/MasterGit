using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB.Production
{
    /// <summary>
    /// Provides interfaces to Model table.
    /// </summary>
    public class Model : TblOneIntPrimaryKey
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public Model(SQLExecuter sqlExec)
            : base("Model", sqlExec)
        {
        }


        /// <summary>
        /// Retrieves an ArrayList(DisplayIntValueItem) of Models to populate dropdown and listboxes
        /// </summary>
        /// <param name="orderByName"></param>
        /// <returns></returns>
        public System.Collections.ArrayList GetListItemsForModels() { return GetListItemsForModels(true); }
        public System.Collections.ArrayList GetListItemsForModels(bool orderByName)
        {
            System.Collections.ArrayList arr = new System.Collections.ArrayList();
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT ModelId, ModelName FROM psModel ORDER BY {0}", (orderByName) ? "ModelName" : "ModelId");
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                if (sqlDataSet != null && sqlDataSet.Tables.Count > 0)
                {
                    for (int a = 0; a < sqlDataSet.Tables[0].Rows.Count; a++)
                    {
                        int orgId = 0;
                        int.TryParse(sqlDataSet.Tables[0].Rows[a]["ModelId"].ToString(), out orgId);
                        arr.Add(new DisplayIntValueItem(orgId, sqlDataSet.Tables[0].Rows[a]["ModelName"].ToString()));
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Models. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Models. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return arr;
        }


        /// <summary>
        /// Retrieves an ArrayList(DisplayIntValueItem) of Models to populate dropdown and listboxes
        /// </summary>
        /// <param name="orderByName"></param>
        /// <returns></returns>
        public DataRow GetModelById(int id)
        {
            DataRow row = null;
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT * FROM psModel WHERE ModelId={0}", id);
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                if (sqlDataSet != null && sqlDataSet.Tables.Count > 0)
                    row = sqlDataSet.Tables[0].Rows[0];
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Model. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Model. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return row;
        }
    }
}
