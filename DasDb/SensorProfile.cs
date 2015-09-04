using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
   /// <summary>
   /// Sensor Profile 
   /// </summary>
   public class SensorProfile : TblConnect2TblsWithRules
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public SensorProfile(SQLExecuter sqlExec) : base("vlfBoxHwSensorProfiles", "vlfBoxHwSensorProfileDetails", sqlExec)
      {
      }

      /// <summary>
      /// Add new Sensor Profile
      /// </summary>
      /// <param name="hwTypeId"></param>
      /// <param name="profileName"></param>
      /// <param name="profileDescription"></param>
      /// <param name="dtSensors"></param>
      /// <returns></returns>
      public short AddSensorProfile(short hwTypeId, string profileName, string profileDescription, DataTable dtSensors)
      {
         int profileId = -1, maxSensors = 0;
         try
         {
            if (dtSensors == null || dtSensors.Rows.Count == 0)
               throw new ArgumentNullException("Sensors data is null or empty");
            if (String.IsNullOrEmpty(profileName))
               throw new ArgumentNullException("Profile Name is null or empty");
            if (hwTypeId < 1)
               throw new ArgumentException("Hardware Type is invalid: " + hwTypeId);

            BoxSensorsCfg boxConfig = new BoxSensorsCfg(sqlExec);
            maxSensors = boxConfig.GetMaxSupportedSensorsByHwType(hwTypeId);
            if (maxSensors < dtSensors.Rows.Count)
               throw new ArgumentException(String.Format("Max number of sensors supported by hardware: {0}\nNew number of sensors: {1}", maxSensors, dtSensors.Rows.Count));

            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@hwTypeId", SqlDbType.SmallInt, hwTypeId);
            sqlExec.AddCommandParam("@name", SqlDbType.VarChar, profileName.Replace("'", "''"));
            sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, profileDescription.Replace("'", "''"));
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadUncommitted);
            string sql = "INSERT " + this.tableName + " VALUES(@hwTypeId, @name, @descr); SELECT SCOPE_IDENTITY();";
            sqlExec.AttachToTransaction(sql);
            Object result = sqlExec.SQLExecuteScalar(sql);
            if (result != null)
            {
               profileId = Convert.ToInt16(result);
               // update profile id value
               foreach (DataRow drow in dtSensors.Rows)
               {
                  drow["ProfileId"] = profileId;
                  this.AddSensor(drow);
               }
               //sqlExec.InsertData(dtSensors, this.tableDetailsName);
            }
            sqlExec.CommitTransaction();
         }
         catch (SqlException)
         {
            sqlExec.RollbackTransaction();
         }
         return (short)profileId;
      }

      /// <summary>
      /// Update profile name, description and sensors set
      /// </summary>
      /// <param name="profileId">Profile Id to update</param>
      /// <param name="profileName">Profile new Name</param>
      /// <param name="profileDescription">Profile new Description</param>
      /// <param name="dtSensors">Profile new sensors DataTable</param>
      public void UpdateSensorProfile(short profileId, string profileName, string profileDescription, DataTable dtSensors)
      {
         try
         {
            if (String.IsNullOrEmpty(profileName))
               throw new ArgumentNullException("Profile Name is null or empty");
            if (profileId < 1)
               throw new ArgumentException("Profile ID is invalid: " + profileId);

            sqlExec.ClearCommandParameters();
            // update name and description
            string sqlUpdate = "UPDATE " + this.tableName + " SET ProfileName = @pname, ProfileDescription = @pdescr WHERE ProfileId = @pid";
            sqlExec.AddCommandParam("@pid", SqlDbType.SmallInt, profileId);
            sqlExec.AddCommandParam("@pname", SqlDbType.VarChar, profileName);
            sqlExec.AddCommandParam("@pdescr", SqlDbType.VarChar, profileDescription);

            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

            if (sqlExec.RequiredTransaction())
               sqlExec.AttachToTransaction(sqlUpdate);

            sqlExec.SQLExecuteNonQuery(sqlUpdate);

            // add new sensors
            if (dtSensors != null && dtSensors.Rows.Count > 0)
            {
               // delete existing profile sensors
               string sqlDelete = "DELETE FROM " + this.tableDetailsName + " WHERE ProfileId = @pid";
               sqlExec.ClearCommandParameters();
               sqlExec.AddCommandParam("@pid", SqlDbType.SmallInt, profileId);
               if (sqlExec.RequiredTransaction())
                  sqlExec.AttachToTransaction(sqlDelete);
               sqlExec.SQLExecuteNonQuery(sqlDelete);
               
               //sqlExec.InsertData(dtSensors, this.tableDetailsName);
               foreach (DataRow drSensor in dtSensors.Rows)
               {
                  AddSensor(drSensor);
               }
            }
            sqlExec.CommitTransaction();
         }
         catch (SqlException)
         {
            sqlExec.RollbackTransaction();
         }
      }

      /// <summary>
      /// Update profile sensors set only
      /// </summary>
      /// <param name="profileId">Profile Id</param>
      /// <param name="dtSensors">Profile new sensors DataTable</param>
      public void UpdateSensorProfile(short profileId, DataTable dtSensors)
      {
         try
         {
            if (profileId < 1)
               throw new ArgumentException("Profile ID is invalid: " + profileId);
            if (dtSensors == null || dtSensors.Rows.Count == 0)
               throw new ArgumentNullException("Sensors data is null or empty");

            string sql = "DELETE FROM " + this.tableDetailsName + " WHERE ProfileId = @pid";
            // delete existing profile sensors
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@pid", SqlDbType.SmallInt, profileId);

            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

            if (sqlExec.RequiredTransaction())
               sqlExec.AttachToTransaction(sql);

            sqlExec.SQLExecuteNonQuery(sql);
            //sqlExec.InsertData(dtSensors, this.tableDetailsName);
            foreach (DataRow drSensor in dtSensors.Rows)
            {
               AddSensor(drSensor);
            }
            sqlExec.CommitTransaction();
         }
         catch (SqlException)
         {
            sqlExec.RollbackTransaction();
         }
      }

      /// <summary>
      /// Adds new sensor.
      /// </summary>
      /// <param name="drSensor"></param>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if sensor name alredy exists.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      private void AddSensor(DataRow drSensor)
      {
         int rowsAffected = 0;
         // 1. Get next availible index
         // 2. Prepares SQL statement
         string prefixMsg = "Unable to add new sensor: " + drSensor["SensorName"].ToString() + ". ";
         try
         {
            if (drSensor == null || drSensor.ItemArray.Length == 0 || drSensor.HasErrors)
               throw new ArgumentNullException("Sensor data is null or empty");

            string sql = string.Format("INSERT " + this.tableDetailsName +
               " VALUES ( {0}, {1}, {2}, {3}, '{4}', '{5}' )",
               drSensor["ProfileId"], drSensor["SensorId"], drSensor["DefaultAlarmLevelOn"],
               drSensor["DefaultAlarmLevelOff"], drSensor["SensorName"], drSensor["SensorAction"]);
            // 3. Executes SQL statement
            if (sqlExec.RequiredTransaction())
               sqlExec.AttachToTransaction(sql);
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
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
         if (rowsAffected == 0)
         {
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This sensor already exists.");
         }
      }
   }
}
