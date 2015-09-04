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
    public class MapLayers : TblOneIntPrimaryKey
    {
        /// <summary>
        /// Provides interfaces to vlfMapLayers table.
        /// </summary>
        public MapLayers(SQLExecuter sqlExec) : base("vlfMapLayers", sqlExec)
        {
        }

        /// <summary>
        /// GetAllMapLayers
        /// </summary>
        /// <returns></returns>
        public DataSet GetAllMapLayers()
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "SELECT * FROM vlfMapLayers";

                //Executes SQL statement
                //sqlDataSet = sqlExec.SPExecuteDataset(sql);
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MapLayers";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MapLayers";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// GetAllMayLayersWithDefaultPremiumByUserID
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="EquipmentMedias"></param>
        /// <returns></returns>
        public DataSet GetAllMapLayersWithDefaultPremiumByOrganizationID(string LayerType, int OrganizationID)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = "GetAllMapLayersWithDefaultPremiumByOrganizationID";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationID", SqlDbType.Int, OrganizationID.ToString(), -1);
                sqlExec.AddCommandParam("@LayerType", SqlDbType.VarChar, LayerType, -1);

                sqlDataSet = sqlExec.SPExecuteDataset(sql);
                
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MapLayers. OrganizationID:" + OrganizationID;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MapLayers. OrganizationID:" + OrganizationID;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return sqlDataSet;
        }

        /// <summary>
        /// Update Default MapLayers
        /// </summary>
        /// <param name="DefaultMaplayerIDs"></param>
        /// <returns></returns>
        public int UpdateDefaultMaplayers(string LayerType, string DefaultMaplayerIDs)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MapLayers_Update";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@LayerType", SqlDbType.VarChar, LayerType);
                sqlExec.AddCommandParam("@DefaultMaplayerIDs", SqlDbType.VarChar, DefaultMaplayerIDs);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update Default MapLayers. DefaultMaplayerIDs:" + DefaultMaplayerIDs;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update Default MapLayers. DefaultMaplayerIDs:" + DefaultMaplayerIDs;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Update Premium MapLayers
        /// </summary>
        /// <param name="OrganizationID"></param>
        /// <param name="PremiumMaplayerIDs"></param>
        /// <returns></returns>
        public int UpdatePremiumMaplayers(string LayerType, int OrganizationID, string PremiumMaplayerIDs)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MapLayersOrganization_Update";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@LayerType", SqlDbType.VarChar, LayerType);
                sqlExec.AddCommandParam("@OrganizationID", SqlDbType.Int, OrganizationID);
                sqlExec.AddCommandParam("@PremiumMaplayerIDs", SqlDbType.VarChar, PremiumMaplayerIDs);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update Premium MapLayers. OrganizationID:" + OrganizationID + ", DefaultMaplayerIDs:" + PremiumMaplayerIDs;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update Premium MapLayers. OrganizationID:" + OrganizationID + ", DefaultMaplayerIDs:" + PremiumMaplayerIDs;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Update Overlay Visibility
        /// </summary>
        /// <param name="DefaultMaplayerIDs"></param>
        /// <returns></returns>
        public int UpdateOverlayVisibility(string VisibleOverlayIDs)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MapLayersOverlayVisibility_Update";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@VisibleOverlayIDs", SqlDbType.VarChar, VisibleOverlayIDs);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update Overlay Visibility. VisibleOverlayIDs:" + VisibleOverlayIDs;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update Overlay Visibility. VisibleOverlayIDs:" + VisibleOverlayIDs;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }
    }
}
