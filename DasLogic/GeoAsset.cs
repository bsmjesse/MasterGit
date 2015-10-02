using System;
using System.Collections.Generic;
using System.Text;
using VLF.DAS.DB;
using System.Data; 

namespace VLF.DAS.Logic
{

    public class GeoAssetManager : Das
    {
        private VLF.DAS.DB.GeoAsset _geoAsset = null;

        public GeoAssetManager(string connectionString)
            : base(connectionString)
        {
            _geoAsset = new VLF.DAS.DB.GeoAsset(sqlExec);

        }

        public DataSet GetGeoAssetsWithBoundaries(int orgId,
                    double topleftlat, double topleftlong, double bottomrightlat, double bottomrightlong,
                    double topleftlatnor, double topleftlongnor, double bottomrightlatnor, double bottomrightlongnor)
        {
            return _geoAsset.GetGeoAssetsWithBoundaries( orgId,
                      topleftlat,  topleftlong,  bottomrightlat,  bottomrightlong,
                     topleftlatnor,  topleftlongnor,  bottomrightlatnor,  bottomrightlongnor);
        }
    }
}
