using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB.Production
{
    /// <summary>
    /// Provides interfaces to EepromMap table.
    /// </summary>
    public class EepromMap : TblOneIntPrimaryKey
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public EepromMap(SQLExecuter sqlExec)
            : base("EepromMap", sqlExec)
        {
        }


        /// <summary>
        /// Retrieves an ArrayList(DisplayIntValueItem) of EepromMaps to populate dropdown and listboxes
        /// </summary>
        /// <param name="orderByName"></param>
        /// <returns></returns>
        public System.Collections.ArrayList GetListItemsForEepromMaps() { return GetListItemsForEepromMaps(true, true); }
        public System.Collections.ArrayList GetListItemsForEepromMaps(bool orderByName, bool onlyWriteableAttributes)
        {
            System.Collections.ArrayList arr = new System.Collections.ArrayList();
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Empty;

                if (onlyWriteableAttributes)
                    sql = string.Format("SELECT EepromMapId, Attribute FROM psEepromMap WHERE ReadOnly = 0 ORDER BY {0}", (orderByName) ? "Attribute" : "EepromMapId");
                else
                    sql = string.Format("SELECT EepromMapId, Attribute FROM psEepromMap ORDER BY {0}", (orderByName) ? "Attribute" : "EepromMapId");


                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                if (sqlDataSet != null && sqlDataSet.Tables.Count > 0)
                {
                    for (int a = 0; a < sqlDataSet.Tables[0].Rows.Count; a++)
                    {
                        int orgId = 0;
                        int.TryParse(sqlDataSet.Tables[0].Rows[a]["EepromMapId"].ToString(), out orgId);
                        arr.Add(new DisplayIntValueItem(orgId, sqlDataSet.Tables[0].Rows[a]["Attribute"].ToString()));
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve EepromMaps. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve EepromMaps. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return arr;
        }


        /// <summary>
        /// Retrieves an ArrayList(DisplayIntValueItem) of EepromMaps to populate dropdown and listboxes
        /// </summary>
        /// <param name="orderByName"></param>
        /// <returns></returns>
        public DataRow GetEepromMapById(int id)
        {
            DataRow row = null;
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT * FROM psEepromMap WHERE EepromMapId={0}", id);
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                if (sqlDataSet != null && sqlDataSet.Tables.Count > 0)
                    row = sqlDataSet.Tables[0].Rows[0];
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve EepromMap. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve EepromMap. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return row;
        }

        /// <summary>
        /// Retrieves an Dataset of EepromMaps base on modelId
        /// </summary>
        /// <param name="orderByName"></param>
        /// <returns></returns>
        public DataSet GetEepromMappingsByModelId(int modelId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format("SELECT psEepromAddress.Attribute, psEepromMap.Location, psEepromMap.Length, psEepromAddress.ReadOnly FROM psEepromAddress LEFT OUTER JOIN psEepromMap ON psEepromAddress.EepromAddressId = psEepromMap.EepromAddressId WHERE (psEepromMap.ModelId = {0} AND psEepromAddress.ReadOnly = 0)", modelId);
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve EepromMap items for model:[{0}].", modelId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve EepromMap items for model:[{0}].", modelId);
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

    }
}
