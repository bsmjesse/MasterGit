using System;
using System.Collections;	// for ArrayList
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfBoxHwDefaultSensorsCfg table.
	/// </summary>
	public class BoxHwDefaultSensorsCfg : TblConnect2TblsWithoutRules
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public BoxHwDefaultSensorsCfg(SQLExecuter sqlExec) : base ("vlfBoxHwDefaultSensorsCfg", sqlExec)
		{
		}

		/// <summary>
		/// Add a new sensor
		/// </summary>
		/// <returns>void</returns>
		/// <param name="boxHwTypeId"></param>
		/// <param name="sensorId"></param>
		/// <param name="sensorName"></param>
		/// <param name="sensorAction"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if sensor name alredy exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddSensor(short boxHwTypeId, short sensorId, string sensorName, string sensorAction)
		{
			int maxSupportedSensors = GetMaxSupportedSensorsByHwType(boxHwTypeId);
			int currSupportedSensors = GetCurrentSupportedSensorsByHwType(boxHwTypeId);
			if(currSupportedSensors + 1 > maxSupportedSensors)
			{
				string errMsg =	"Unable to add new sensor sensor id=" + sensorId + 
					" to Hw type=" + boxHwTypeId +
					". Maximal number " + maxSupportedSensors + 
					" of supported sensors has been reached.";
				throw new DASAppException(errMsg);
			}
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = string.Format("INSERT INTO vlfBoxHwDefaultSensorsCfg (BoxHwTypeId, SensorId, SensorName, SensorAction) VALUES ( {0}, {1}, '{2}', '{3}')", boxHwTypeId,sensorId,sensorName,sensorAction);
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add new sensor.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new sensor.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new sensor.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " Sensor already exists.");
			}	
		}

        /// <summary>
        /// Add a new sensor - overloaded
        /// </summary>
        /// <returns>void</returns>
        /// <param name="boxHwTypeId"></param>
        /// <param name="sensorId"></param>
        /// <param name="sensorName"></param>
        /// <param name="sensorAction"></param>
        /// <param name="alarmOn"></param>
        /// <param name="alarmOff"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if sensor name alredy exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AddSensor(short boxHwTypeId, short sensorId, string sensorName, string sensorAction, short alarmOn, short alarmOff)
        {
            int maxSupportedSensors = GetMaxSupportedSensorsByHwType(boxHwTypeId);
            int currSupportedSensors = GetCurrentSupportedSensorsByHwType(boxHwTypeId);
            if (currSupportedSensors + 1 > maxSupportedSensors)
            {
                string errMsg = "Unable to add new sensor sensor id=" + sensorId +
                    " to Hw type=" + boxHwTypeId +
                    ". Maximal number " + maxSupportedSensors +
                    " of supported sensors has been reached.";
                throw new DASAppException(errMsg);
            }
            int rowsAffected = 0;
            // 1. Prepares SQL statement
            string sql = string.Format("INSERT INTO vlfBoxHwDefaultSensorsCfg (BoxHwTypeId, SensorId, SensorName, SensorAction, DefaultAlarmLevelOn, DefaultAlarmLevelOff) VALUES ( {0}, {1}, '{2}', '{3}', {4}, {5})",
                boxHwTypeId, sensorId, sensorName, sensorAction, alarmOn, alarmOff);
            try
            {
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new sensor.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new sensor.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add new sensor.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " Sensor already exists.");
            }
        }

        /// <summary>
		/// Delete all sensors related to box hardware type.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="boxHwTypeId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if box hardware type does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteSensorsByHwTypeId(short boxHwTypeId)
		{
			return DeleteRowsByIntField("BoxHwTypeId", boxHwTypeId, "box hardware type");		
		}
		
		/// <summary>
		/// Retrieves sensor info by box hardware type
		/// </summary>
		/// <param name="boxHwTypeId"></param> 
		/// <returns>DataSet [SensorId],[SensorName],[SensorAction],[AlarmLevelOn],[AlarmLevelOff]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetSensorsInfoByHwTypeId(short boxHwTypeId)
		{
			DataSet sqlDataSet = null;
			//Prepares SQL statement
			try
			{
				//Prepares SQL statement
				string sql = "SELECT SensorId,SensorName,SensorAction,ISNULL(DefaultAlarmLevelOn,0) AS AlarmLevelOn,ISNULL(DefaultAlarmLevelOff,0) AS AlarmLevelOff FROM vlfBoxHwDefaultSensorsCfg WHERE BoxHwTypeId = " + boxHwTypeId + " ORDER BY SensorId ASC";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve a list of sensors.";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve a list of sensors.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

		/// <summary>
		/// Retrieves max number of supported sensors for specific Hw type.
		/// Note: if box does not exist return 0
		/// </summary>
		/// <param name="boxHwTypeId"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int GetMaxSupportedSensorsByHwType(short boxHwTypeId)
		{
			int maxSensors = 0;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT MaxSensorsNum FROM vlfBoxHwType" + 
						" WHERE BoxHwTypeId=" + boxHwTypeId;
				//Executes SQL statement
				maxSensors = Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
			}
			catch (SqlException objException) 
			{
                string prefixMsg = "Unable to retrieve max number of supported sensors for the HwType=" + boxHwTypeId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
                string prefixMsg = "Unable to retrieve max number of supported sensors for the HwType=" + boxHwTypeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return maxSensors;
		}

		/// <summary>
		/// Retrieves current number of supported sensors for specific Hw type.
		/// Note: if box does not exist return 0
		/// </summary>
		/// <param name="boxHwTypeId"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int GetCurrentSupportedSensorsByHwType(short boxHwTypeId)
		{
			int recordCount = 0;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT COUNT(BoxHwTypeId) FROM vlfBoxHwDefaultSensorsCfg WHERE BoxHwTypeId=" + boxHwTypeId;
				//Executes SQL statement
				recordCount =  Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
			}
			catch (SqlException objException) 
			{
                string prefixMsg = "Unable to retrieve current number of supported sensors for the boxHwTypeId=" + boxHwTypeId + ". ";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
                string prefixMsg = "Unable to retrieve current number of supported sensors the for boxHwTypeId=" + boxHwTypeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}				
			return recordCount;
		}

        /// <summary>
        /// Get all sensors from DB
        /// </summary>
        /// <param name="alarms" type="boolean">Include or not default alarm levels on and off</param>
        /// <returns></returns>
        public DataSet GetAllSensors(bool alarms)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                //Prepares SQL statement
                System.Text.StringBuilder sql = new System.Text.StringBuilder("SELECT SensorId As ID, SensorName As Name, SensorAction As Action");
                // append default alarm levels
                if (alarms) sql.Append(", 0 As [Alarm On], 0 As [Alarm Off]");
                sql.Append(" FROM vlfSensor ORDER BY SensorId ASC");
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql.ToString());
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve a sensors list";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve a sensors list";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
	}
}
