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
    public class MapLayersManager : VLF.DAS.Das
    {
        private VLF.PATCH.DB.MapLayers _maplayers = null;

        public MapLayersManager(string connectionString)
           : base(connectionString)
		{
            _maplayers = new VLF.PATCH.DB.MapLayers(sqlExec);
        
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// GetAllMapLayers
        /// </summary>
        /// <returns></returns>
        public DataSet GetAllMapLayers()
        {
            return _maplayers.GetAllMapLayers();
        }

        /// <summary>
        /// GetAllMayLayersWithDefaultPremiumByUserID
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="EquipmentMedias"></param>
        /// <returns></returns>
        public DataSet GetAllMapLayersWithDefaultPremiumByOrganizationID(int OrganizationID)
        {
            return GetAllMapLayersWithDefaultPremiumByOrganizationID("BaseLayer", OrganizationID);
        }

        public DataSet GetAllMapLayersWithDefaultPremiumByOrganizationID(string layerType, int OrganizationID)
        {
            return _maplayers.GetAllMapLayersWithDefaultPremiumByOrganizationID(layerType, OrganizationID);
        }

        /// <summary>
        /// Update Default MapLayers
        /// </summary>
        /// <param name="DefaultMaplayerIDs"></param>
        /// <returns></returns>
        public int UpdateDefaultMaplayers(string DefaultMaplayerIDs)
        {
            return UpdateDefaultMaplayers("BaseLayer", DefaultMaplayerIDs);
        }

        public int UpdateDefaultMaplayers(string LayerType, string DefaultMaplayerIDs)
        {
            return _maplayers.UpdateDefaultMaplayers(LayerType, DefaultMaplayerIDs);
        }

        /// <summary>
        /// Update Premium MapLayers
        /// </summary>
        /// <param name="OrganizationID"></param>
        /// <param name="PremiumMaplayerIDs"></param>
        /// <returns></returns>
        public int UpdatePremiumMaplayers(int OrganizationID, string PremiumMaplayerIDs)
        {
            return UpdatePremiumMaplayers("BaseLayer", OrganizationID, PremiumMaplayerIDs);
        }

        public int UpdatePremiumMaplayers(string LayerType, int OrganizationID, string PremiumMaplayerIDs)
        {
            return _maplayers.UpdatePremiumMaplayers(LayerType, OrganizationID, PremiumMaplayerIDs);
        }

        /// <summary>
        /// Update Overlay Visibility
        /// </summary>
        /// <param name="DefaultMaplayerIDs"></param>
        /// <returns></returns>
        public int UpdateOverlayVisibility(string VisibleOverlayIDs)
        {
            return _maplayers.UpdateOverlayVisibility(VisibleOverlayIDs);
        }
    }
}
