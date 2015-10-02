using System;
using System.Collections;	// for ArrayList
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
   /// <summary>
   /// Provides interfaces to vlfSensor table.
   /// </summary>
   public class Sensor : TblOneIntPrimaryKey
   {
      /// <summary>
      /// Constructor
      /// </summary>
      public Sensor(SQLExecuter sqlExec)
         : base("vlfSensor", sqlExec)
      {
      }

      /// <summary>
      /// Adds new sensor.
      /// </summary>
      /// <param name="sensorName"></param>
      /// <param name="sensorAction"></param>
      /// <returns>int next sensor id</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if sensor name alredy exists.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int AddSensor(string sensorName, string sensorAction)
      {
         int rowsAffected = 0;
         // 1. Get next availible index
         int sensorId = (int)GetMaxRecordIndex("SensorId") + 1;
         // 2. Prepares SQL statement
         string sql = string.Format("INSERT INTO " + tableName
            + " (SensorId,SensorName,SensorAction) VALUES ( {0}, '{1}', '{2}' )",
            sensorId, sensorName.Replace("'", "''"), sensorAction.Replace("'", "''"));
         try
         {
            // 3. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to add new sensor with sensor name " + sensorName + " .";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to add new sensor with sensor name " + sensorName + " .";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            string prefixMsg = "Unable to add new sensor with sensor name " + sensorName + " .";
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This sensor already exists.");
         }
         return sensorId;
      }

      /// <summary>
      /// Add new Sensor for a device
      /// </summary>
      /// <param name="deviceId">Device Id (0 - 7)</param>
      /// <param name="deviceTypeId">Device Type Id (0 - 15)</param>
      /// <param name="sensorName">Sensor name</param>
      /// <param name="sensorAction">Sensor action</param>
      /// <returns>New sensor ID</returns>
      public int AddSensor(short deviceId, short deviceTypeId, string sensorName, string sensorAction)
      {
         SqlParameter[] sqlParams = new SqlParameter[]
         {
            new SqlParameter("@deviceId", deviceId),
            new SqlParameter("@typeId", deviceTypeId),
            new SqlParameter("@name", sensorName.Replace("'", "''")),
            new SqlParameter("@action", sensorAction.Replace("'", "''"))
         };
         return sqlExec.SPExecuteNonQuery("SensorAdd", sqlParams);
      }

      /// <summary>
      /// Deletes existing sensor.
      /// </summary>
      /// <returns>rows affected</returns>
      /// <param name="sensorName"></param> 
      /// <exception cref="DASAppResultNotFoundException">Thrown if sensor name does not exist</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteSensorByName(string sensorName)
      {
         return DeleteRowsByStrField("SensorName", sensorName, "sensor");
      }

      /// <summary>
      /// Deletes existing sensor.
      /// </summary>
      /// <returns>rows affected</returns>
      /// <param name="sensorId"></param> 
      /// <exception cref="DASAppResultNotFoundException">Thrown if sensor id does not exist</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteSensorById(short sensorId)
      {
         return DeleteRowsByIntField("SensorId", sensorId, "sensor");
      }

      /// <summary>
      /// Returns formated sensor status
      /// </summary>
      /// <param name="customProp"></param>
      /// <param name="tblUserDefinedSensors"></param>
      /// <returns>sensor description</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public static string GetSensorDescription(string customProp, DataTable tblUserDefinedSensors)
      {
         string sensorDescription = "";
         try
         {
            if (CLS.Util.PairFindValue(CLS.Def.Const.keyAlarmNum, customProp) != "")
            {
               //dsVehicleSensors
               int sensorId = Convert.ToInt32(Util.PairFindValue(VLF.CLS.Def.Const.keySensorNum, customProp));
               if (tblUserDefinedSensors == null || tblUserDefinedSensors.Rows.Count == 0)
               {
                  sensorDescription = sensorId.ToString();
                  sensorDescription += " ";
                  sensorDescription += Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
               }
               else
               {
                  string sensorAction = "";
                  int delim = 0;
                  foreach (DataRow ittr in tblUserDefinedSensors.Rows)
                  {
                     if (sensorId == Convert.ToInt32(ittr["SensorId"]))
                     {
                        sensorDescription = ittr["SensorName"].ToString().TrimEnd();
                        sensorDescription += " ";

                        sensorAction = ittr["SensorAction"].ToString().TrimEnd();
                        delim = sensorAction.IndexOf('/', 0);
                        if (Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp) == VLF.CLS.Def.Const.valON)
                           sensorDescription += sensorAction.Substring(0, delim);
                        else if (Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp) == VLF.CLS.Def.Const.valOFF)
                           sensorDescription += sensorAction.Substring(delim + 1, sensorAction.Length - delim - 1);
                        break;
                     }
                  }
                  if (sensorDescription == "")
                  {
                     sensorDescription = sensorId.ToString();
                     sensorDescription += " ";
                     sensorDescription += Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
                  }
               }
            }
            else if (CLS.Util.PairFindValue(CLS.Def.Const.keyFenceDir, customProp) != "")
            {
               sensorDescription = "Geofence broken";
            }
         }
         catch
         {
            //VLF.ERR.LOG.LogFile(logDir,"DAS","GetSensorDescription error: " +  exp.Message);
         }
         return sensorDescription;
      }

      /// <summary>
      /// Get all sensors from DB
      /// </summary>
      /// <param name="alarms" type="boolean">Include or not default alarm levels on and off</param>
      /// <returns></returns>
      public DataSet GetAllSensors(bool alarms)
      {
         DataSet sqlDataSet = null;
         string prefixMsg = "Unable to retrieve a sensors list";
         //Prepares SQL statement
         try
         {
            //Prepares SQL statement
            System.Text.StringBuilder sql = 
               new System.Text.StringBuilder("SELECT SensorId As ID, SensorName As Name, SensorAction As Action");
            // append default alarm levels
            if (alarms) sql.Append(", 0 As [Alarm On], 0 As [Alarm Off]");
            sql.Append(" FROM ");
            sql.Append(tableName);
            sql.Append(" ORDER BY SensorId ASC");
            //Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql.ToString());
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
         return sqlDataSet;
      }

   }
}
