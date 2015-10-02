using System;
using System.Text;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
   /// <summary>
   /// Vehicle Services
   /// </summary>
   public partial class VehicleServices : TblTwoPrimaryKeys
   {
      private const string MainTable = "vlfVehicleServices";
      public string HistoryTable = "vlfVehicleServicesHistory";

      /// <summary>
      /// Vehicle Services constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public VehicleServices(SQLExecuter sqlExec) : base(MainTable, sqlExec)
      {
      }

      public DataSet GetVehicleCurrentServices(int boxId)
      {
         DataSet resultDataSet = new DataSet();
         string prefixMsg = "Unable to retrieve vehicle current services for " + boxId + ". ";
         try
         {
            // 1. Prepares SQL statement
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT     dbo.vlfOrganizationNotifications.Notification1 AS Notification1, dbo.vlfOrganizationNotifications.Notification2 AS Notification2,");
            sql.AppendLine("dbo.vlfOrganizationNotifications.Notification3 AS Notification3, dbo.vlfVehicleMaintenance.CurrentOdo AS CurrentOdo,"); 
            sql.AppendLine("dbo.vlfVehicleMaintenance.CurrentEngHrs AS CurrentEngHrs, dbo.vlfVehicleMaintenance.CurrentIdlingHrs AS CurrentIdlingHrs,");
            sql.AppendLine("dbo.vlfVehicleAssignment.BoxId AS BoxId, dbo.vlfVehicleInfo.Description, dbo.vlfVehicleServices.* ");
            sql.AppendLine("FROM         dbo.vlfVehicleServices INNER JOIN");
            sql.AppendLine("          dbo.vlfVehicleMaintenance ON dbo.vlfVehicleServices.VehicleId = dbo.vlfVehicleMaintenance.VehicleId INNER JOIN");
            sql.AppendLine("          dbo.vlfVehicleInfo ON dbo.vlfVehicleServices.VehicleId = dbo.vlfVehicleInfo.VehicleId INNER JOIN");
            sql.AppendLine("          dbo.vlfOrganizationNotifications ON dbo.vlfVehicleServices.NotificationID = dbo.vlfOrganizationNotifications.NotificationID INNER JOIN");
            sql.AppendLine("          dbo.vlfVehicleAssignment ON dbo.vlfVehicleServices.VehicleId = dbo.vlfVehicleAssignment.VehicleId");
            sql.AppendFormat("WHERE     (dbo.vlfVehicleAssignment.BoxId = {0})", boxId);
                       
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attaches SQL to transaction
               sqlExec.AttachToTransaction(sql.ToString());
            }
            // 3. Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql.ToString());
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
         return resultDataSet;
      }
   }
}
