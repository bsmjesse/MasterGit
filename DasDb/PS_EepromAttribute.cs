using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB.Production
{
    /// <summary>
    /// Provides interfaces to EepromAttribute table.
    /// </summary>
    public class EepromAttribute : TblOneIntPrimaryKey
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public EepromAttribute(SQLExecuter sqlExec)
            : base("EepromAttribute", sqlExec)
        {
        }


        /// <summary>
        /// Retrieves an ArrayList(DisplayIntValueItem) of EepromAttributes to populate dropdown and listboxes
        /// </summary>
        /// <param name="orderByName"></param>
        /// <returns></returns>
        public System.Collections.ArrayList GetListItemsForEepromAttributes() { return GetListItemsForEepromAttributes(true, true); }
        public System.Collections.ArrayList GetListItemsForEepromAttributes(bool orderByName, bool onlyWriteableAttributes)
        {
            System.Collections.ArrayList arr = new System.Collections.ArrayList();
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Empty;

                if (onlyWriteableAttributes)
                    sql = string.Format("SELECT EepromAttributeId, Attribute FROM psEepromAttribute WHERE ReadOnly = 0 ORDER BY {0}", (orderByName) ? "Attribute" : "EepromAttributeId");
                else
                    sql = string.Format("SELECT EepromAttributeId, Attribute FROM psEepromAttribute ORDER BY {0}", (orderByName) ? "Attribute" : "EepromAttributeId");


                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                if (sqlDataSet != null && sqlDataSet.Tables.Count > 0)
                {
                    for (int a = 0; a < sqlDataSet.Tables[0].Rows.Count; a++)
                    {
                        int orgId = 0;
                        int.TryParse(sqlDataSet.Tables[0].Rows[a]["EepromAttributeId"].ToString(), out orgId);
                        arr.Add(new DisplayIntValueItem(orgId, sqlDataSet.Tables[0].Rows[a]["Attribute"].ToString()));
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve EepromAttributes. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve EepromAttributes. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return arr;
        }


        /// <summary>
        /// Retrieves an ArrayList(DisplayIntValueItem) of EepromAttributes to populate dropdown and listboxes
        /// </summary>
        /// <param name="orderByName"></param>
        /// <returns></returns>
        public DataRow GetEepromAttributeById(int id)
        {
            DataRow row = null;
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT * FROM psEepromAttribute WHERE EepromAttributeId={0}", id);
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                if (sqlDataSet != null && sqlDataSet.Tables.Count > 0)
                    row = sqlDataSet.Tables[0].Rows[0];
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve EepromAttribute. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve EepromAttribute. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return row;
        }
    }
}
