using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using VLF.DAS.DB;
using VLF.CLS.Def.Structures;
using System.Data.SqlClient;
using VLF.CLS;

namespace VLF.DAS.Logic
{
   public partial class SystemConfig
   {
      /// <summary>
      /// Create box, vehicle and assign box to the vehicle if vehicle is not null,
      /// Otherwise unassigned box is created
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="boxId"></param>
      /// <param name="fwChId"></param>
      /// <param name="featureMask"></param>
      /// <param name="hwTypeId"></param>
      /// <param name="dsProtocolMaxMessages"></param>
      /// <param name="dsCommInfo"></param>
      /// <param name="dsOutputs"></param>
      /// <param name="dsSensors"></param>
      /// <param name="dsMessages"></param>
      /// <param name="vInfo"></param>
      /// <param name="licensePlate"></param>
      /// <returns></returns>
      public bool CreateBoxVehicleAssign(int organizationId, int boxId, short fwChId, long featureMask, short hwTypeId,
         DataSet dsProtocolMaxMessages, DataSet dsCommInfo, DataSet dsOutputs, DataSet dsSensors, DataSet dsMessages,
         VehicInfo vInfo, string licensePlate)
      {
         bool result = false;
         Box box = null;
         try
         {
            // 1. Add box record - tran
            box = new Box(sqlExec.ConnectionString);
            box.AddBox(boxId, fwChId, false, true, organizationId, featureMask, dsProtocolMaxMessages);

            // 2. add comm info - tran
            AddCommInfo(boxId, dsCommInfo);

            // 3. Set outputs for the box - tran
            box.SetOutputs(boxId, hwTypeId, dsOutputs);

            // 4. Set defined sensors for the box - tran
            box.SetSensors(boxId, hwTypeId, dsSensors);

            // 5. Set message severity for the box - tran
            box.AddMsgSeverity(boxId, dsMessages);

            // 6. Vehicle assignment structure contains vehicleId, boxId, lic. plate - tran
            if (!String.IsNullOrEmpty(licensePlate) && !vInfo.Equals(null))
            {
               VehicAssign vehicleAssign = new VehicAssign();
               vehicleAssign.boxId = boxId;
               vehicleAssign.licensePlate = licensePlate;
               // vehicle id will be generated automaticaly inside the function
               Vehicle vehicle = new Vehicle(sqlExec.ConnectionString);
               long newVehicleID = vehicle.AddVehicle(vInfo, vehicleAssign, organizationId);

               if (newVehicleID < 1)
                  throw new Exception("Create new vehicle failed");
            }
            result = true;
         }
         catch
         {
            this.DeleteBox(boxId);
            throw;
         }
         return result;
      }

      /// <summary>
      /// Create box, vehicle and assign box to the vehicle if vehicle is not null,
      /// Otherwise unassigned box is created
      /// to make sure all the steps use the same transaction, the steps are executed
      /// in the box class
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="boxId"></param>
      /// <param name="fwChId"></param>
      /// <param name="featureMask"></param>
      /// <param name="hwTypeId"></param>
      /// <param name="dsProtocolMaxMessages"></param>
      /// <param name="dsCommInfo"></param>
      /// <param name="dsOutputs"></param>
      /// <param name="dsSensors"></param>
      /// <param name="dsMessages"></param>
      /// <param name="vInfo"></param>
      /// <param name="licensePlate"></param>
      /// <returns></returns>
       public bool CreateBox(int organizationId, int boxId, short fwChId, long featureMask, short hwTypeId,
        DataSet dsProtocolMaxMessages, DataSet dsCommInfo, DataSet dsOutputs, DataSet dsSensors, DataSet dsMessages,
        VehicInfo vInfo, string licensePlate)
       {

           Box box = new Box(sqlExec.ConnectionString);
           return box.CreateBoxAndVehicle(organizationId, boxId, fwChId, featureMask, hwTypeId,
                                dsProtocolMaxMessages, dsCommInfo, dsOutputs, dsSensors, dsMessages,
                                vInfo, licensePlate);
       }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="organizationId"></param>
       /// <param name="boxId"></param>
       /// <param name="vInfo"></param>
       /// <param name="licensePlate"></param>
       /// <returns></returns>
       public long CreateVehicle(int organizationId, int boxId, VehicInfo vInfo, string licensePlate)
       {
           long vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
           Vehicle veh = new Vehicle(sqlExec.ConnectionString);
           //Vehicle assignment structure contains vehicleId, boxId, lic. plate - tran
           if (!String.IsNullOrEmpty(licensePlate) && !vInfo.Equals(null))
           {
               VehicAssign vehicleAssign = new VehicAssign();
               vehicleAssign.boxId = boxId;
               vehicleAssign.licensePlate = licensePlate;

               vehicleId = veh.AddVehicle(vInfo, vehicleAssign, organizationId);
           }
           return vehicleId;
       }

      /// <summary>
      /// Update Firmware Name - usage MC
      /// </summary>
      /// <param name="fwId">Firmware Id</param>
      /// <param name="fwName">New firmware name</param>
      public int UpdateFirmwareName(short fwId, string fwName)
      {
         BoxConfig boxConfig = new BoxConfig(this.sqlExec);
         SqlParameter[] sqlParams = new SqlParameter[] { new SqlParameter("@fwName", fwName), new SqlParameter("@fwId", fwId) };
         return boxConfig.UpdateRow("SET FwName = @fwName WHERE FwId = @fwId", sqlParams);
      }

      public DataSet GetRules(short typeId)
      {
          Configuration config = new Configuration(this.sqlExec);

          return config.GetRules(); 
      }
   }
}
