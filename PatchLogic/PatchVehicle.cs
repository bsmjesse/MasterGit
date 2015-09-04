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
    public class PatchVehicle : VLF.DAS.Das
    {
        private VLF.PATCH.DB.PatchVehicle _vehicle = null;

        public PatchVehicle(string connectionString)
           : base(connectionString)
		{
            _vehicle = new VLF.PATCH.DB.PatchVehicle(sqlExec);        
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        public DataSet GetVehiclePeripheralInfoByBoxId(int BoxId)
        {
            return _vehicle.GetVehiclePeripheralInfoByBoxId(BoxId);
        }

        public int SaveImagePath(long vehicleId, string imagePath)
        {
            return _vehicle.SaveImagePath(vehicleId, imagePath);
        }

        public string GetImagePath(long vehicleId)
        {
            return _vehicle.GetImagePath(vehicleId);
        }

        public int UpdateExcludeFromLandmarkProcessing(bool exclude, int organizationId, long vehicleId)
        {
            return _vehicle.UpdateExcludeFromLandmarkProcessing(exclude, organizationId, vehicleId);
        }

        public bool IfExcludeFromLandmarkProcessing(int organizationId, long vehicleId)
        {
            return _vehicle.IfExcludeFromLandmarkProcessing(organizationId, vehicleId);
        }

        public string GetVehicleLicensePlateByBoxId(int BoxId)
        {
            return _vehicle.GetVehicleLicensePlateByBoxId(BoxId);
        }
    }
}
