using System;
using System.Collections.Generic;
using System.Text;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using System.Data;
using System.Data.SqlClient;

namespace VLF.DAS.DB 
{
    public class GeoAsset : TblGenInterfaces
    {
        public GeoAsset(SQLExecuter sqlExec)
            : base("vlfLandmark", sqlExec)
        {
        }

        public DataSet GetGeoAssetsWithBoundaries(int orgId,
                    double topleftlat, double topleftlong, double bottomrightlat, double bottomrightlong,
                    double topleftlatnor, double topleftlongnor, double bottomrightlatnor, double bottomrightlongnor)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[9];
                sqlParams[0] = new SqlParameter("@orgId", orgId);
                sqlParams[1] = new SqlParameter("@topleftlat", topleftlat);
                sqlParams[2] = new SqlParameter("@topleftlong", topleftlong);
                sqlParams[3] = new SqlParameter("@bottomrightlat", bottomrightlat);
                sqlParams[4] = new SqlParameter("@bottomrightlong", bottomrightlong);
                sqlParams[5] = new SqlParameter("@topleftlatnor", topleftlatnor);
                sqlParams[6] = new SqlParameter("@topleftlongnor", topleftlongnor);
                sqlParams[7] = new SqlParameter("@bottomrightlatnor", bottomrightlatnor);
                sqlParams[8] = new SqlParameter("@bottomrightlongnor", bottomrightlongnor);

                // SQL statement
                string sql = "GetGeoAssetsWithBoundaries";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultSet;
        }
    }
}
