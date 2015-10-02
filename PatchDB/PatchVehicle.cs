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
    public class PatchVehicle : TblOneIntPrimaryKey
    {
        public PatchVehicle(SQLExecuter sqlExec) : base("vlfVehicleInfo", sqlExec)
        {
        }

        public DataSet GetVehiclePeripheralInfoByBoxId(int BoxId)
        {
            string sql = "GetVehiclePeripheralInfoByBoxId";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, BoxId);
            
            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteDataset(sql);
        }

        public int SaveImagePath(long vehicleId, string imagePath)
        {
            string sql = "UPDATE vlfVehicleInfo SET ImagePath='" + imagePath + "' WHERE VehicleId=" + vehicleId.ToString();
            return sqlExec.SQLExecuteNonQuery(sql);
        }

        public string GetImagePath(long vehicleId)
        {
            string sql = "SELECT ISNULL(ImagePath, '') FROM vlfVehicleInfo WHERE VehicleId=" + vehicleId.ToString();
            DataSet dt = sqlExec.SQLExecuteDataset(sql);
            return dt.Tables[0].Rows[0][0].ToString();
        }

        public int UpdateExcludeFromLandmarkProcessing(bool exclude, int organizationId, long vehicleId)
        {
            string sql = "sp_UpdateExcludeFromLandmarkProcessing";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@exclude", SqlDbType.Bit, exclude);
            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@vehicleId", SqlDbType.BigInt, vehicleId);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteNonQuery(sql);
        }

        public bool IfExcludeFromLandmarkProcessing(int organizationId, long vehicleId)
        {
            string sql = "sp_IfExcludedFromLandmarkProcessing";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@vehicleId", SqlDbType.BigInt, vehicleId);

            DataSet ds = sqlExec.SPExecuteDataset(sql);

            if (int.Parse(ds.Tables[0].Rows[0][0].ToString()) > 0)
                return true;
            else
                return false;
        }

        public string GetVehicleLicensePlateByBoxId(int BoxId)
        {
            string sql = "SELECT LicensePlate FROM vlfVehicleAssignment WHERE BoxId = " + BoxId.ToString();

            DataSet ds = sqlExec.SQLExecuteDataset(sql);
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            return "";
        }
    }
}
