using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB.Production
{
    /// <summary>
    /// Provides interfaces to Carrier table.
    /// </summary>
    public class Carrier : TblOneIntPrimaryKey
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public Carrier(SQLExecuter sqlExec)
            : base("Carrier", sqlExec)
        {
        }


        /// <summary>
        /// Retrieves an ArrayList(DisplayIntValueItem) of Carriers to populate dropdown and listboxes
        /// </summary>
        /// <param name="orderByName"></param>
        /// <returns></returns>
        public System.Collections.ArrayList GetListItemsForCarriers() { return GetListItemsForCarriers(true); }
        public System.Collections.ArrayList GetListItemsForCarriers(bool orderByName)
        {
            System.Collections.ArrayList arr = new System.Collections.ArrayList();
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT CarrierId, CarrierName FROM psCarrier ORDER BY {0}", (orderByName) ? "CarrierName" : "CarrierId");
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                if (sqlDataSet != null && sqlDataSet.Tables.Count > 0)
                {
                    for (int a = 0; a < sqlDataSet.Tables[0].Rows.Count; a++)
                    {
                        int orgId = 0;
                        int.TryParse(sqlDataSet.Tables[0].Rows[a]["CarrierId"].ToString(), out orgId);
                        arr.Add(new DisplayIntValueItem(orgId, sqlDataSet.Tables[0].Rows[a]["CarrierName"].ToString()));
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve carriers. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve carriers. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return arr;
        }


        /// <summary>
        /// Retrieves an ArrayList(DisplayIntValueItem) of Carriers to populate dropdown and listboxes
        /// </summary>
        /// <param name="orderByName"></param>
        /// <returns></returns>
        public DataRow GetCarrierById(int id)
        {
            DataRow row = null;
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT * FROM psCarrier WHERE CarrierId={0}", id);
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                if (sqlDataSet != null && sqlDataSet.Tables.Count > 0)
                    row = sqlDataSet.Tables[0].Rows[0];
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve carrier. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve carrier. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return row;
        }
    }
}
