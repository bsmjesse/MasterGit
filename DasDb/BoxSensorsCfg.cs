using System;
using System.Collections;	// for ArrayList
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfBoxSensorsCfg table.
	/// </summary>
	public class BoxSensorsCfg : TblTwoPrimaryKeys
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public BoxSensorsCfg(SQLExecuter sqlExec) : base ("vlfBoxSensorsCfg",sqlExec)
		{
		}
		/// <summary>
		/// Add new sensor.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxHwTypeId"></param>
		/// <param name="sensorId"></param>
		/// <param name="sensorName"></param>
		/// <param name="sensorAction"></param>
		/// <param name="alarmLevelOn"></param>
		/// <param name="alarmLevelOff"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppException">Maximal number of supported sensors has been reached.</exception>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if sensor name or id alredy exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddSensor(int boxId,short boxHwTypeId,short sensorId,string sensorName,string sensorAction,short alarmLevelOn,short alarmLevelOff)
		{
			int maxSupportedSensors = GetMaxSupportedSensorsByHwType(boxHwTypeId);
			int currSupportedSensors = GetCurrentSupportedSensorsByBoxId(boxId);
			if(currSupportedSensors + 1 > maxSupportedSensors)
			{
				string errMsg =	"Unable to add to box " + boxId + 
									" new sensor sensor id=" + sensorId + 
									" sensor name=" + sensorName + 
									" and sensor action =" + sensorAction + 
									". Maximal number " + maxSupportedSensors + 
									" of supported sensors has been reached.";
				throw new DASAppException(errMsg);
			}

			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName + 
						" ( BoxId,SensorId,SensorName,SensorAction,AlarmLevelOn,AlarmLevelOff)" +
						" VALUES ( {0}, {1} , '{2}','{3}',{4} ,{5})", 
						boxId,sensorId,sensorName.Replace("'","''"),sensorAction.Replace("'","''"),alarmLevelOn,alarmLevelOff);
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
				string prefixMsg = "Unable to add new sensor with box id=" + boxId + " sensor id=" + sensorId + " sensor name=" + sensorName + " sensor action =" + sensorAction + " alarmLevelOn=" + alarmLevelOn + " alarmLevelOff=" + alarmLevelOff + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new sensor with box id=" + boxId + " sensor id=" + sensorId + " sensor name=" + sensorName + " sensor action =" + sensorAction + " alarmLevelOn=" + alarmLevelOn + " alarmLevelOff=" + alarmLevelOff + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new sensor with box id=" + boxId + " sensor id=" + sensorId + " sensor name=" + sensorName + " sensor action =" + sensorAction + " alarmLevelOn=" + alarmLevelOn + " alarmLevelOff=" + alarmLevelOff + ".";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This sensor already exists.");
			}
		}		
		/// <summary>
		/// Add new sensors.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxHwTypeId"></param>
		/// <param name="dsSensorsCfg"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if sensor id and name alredy exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddSensors(int boxId,short boxHwTypeId,DataSet dsSensorsCfg)
		{
			if((dsSensorsCfg != null) && (dsSensorsCfg.Tables.Count > 0))
			{
				foreach(DataRow ittr in dsSensorsCfg.Tables[0].Rows)
				{
					AddSensor(boxId,
						boxHwTypeId,
						Convert.ToInt16(ittr[0]),
						ittr[1].ToString().TrimEnd(),
						ittr[2].ToString().TrimEnd(),
						Convert.ToInt16(ittr[3]),
						Convert.ToInt16(ittr[4]));
				}
			}
		}
		/// <summary>
		/// Update sensor information.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="sensorId"></param>
		/// <param name="sensorName"></param>
		/// <param name="sensorAction"></param>
		/// <param name="alarmLevelOn"></param>
		/// <param name="alarmLevelOff"></param>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not have info for current sensor.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateSensor(int boxId,short sensorId,string sensorName,string sensorAction,short alarmLevelOn,short alarmLevelOff)
		{
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "UPDATE " + tableName + 
				" SET SensorName='" + sensorName.Replace("'","''") + "'" +
				", SensorAction='" + sensorAction.Replace("'","''") + "'" +
				", AlarmLevelOn=" + alarmLevelOn + 
				", AlarmLevelOff=" + alarmLevelOff + 
				" WHERE BoxId=" + boxId +
				" AND SensorId=" + sensorId;
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update sensor info for box=" + boxId + " sensorId=" + sensorId + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update sensor info for box=" + boxId + " sensorId=" + sensorId + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to update sensor info for box=" + boxId + " sensorId=" + sensorId + ".";
				throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " Box " + boxId + " does not have info for sensor " + sensorId + ".");
			}
		}

        public void UpdateSensorName(int boxId, short sensorId, string sensorName)
        {
            int rowsAffected = 0;
            //Prepares SQL statement
            string sql = "UPDATE " + tableName +
                " SET SensorName='" + sensorName.Replace("'", "''") + "'" +
                " WHERE BoxId=" + boxId +
                " AND SensorId=" + sensorId;
            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update sensor info for box=" + boxId + " sensorId=" + sensorId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update sensor info for box=" + boxId + " sensorId=" + sensorId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update sensor info for box=" + boxId + " sensorId=" + sensorId + ".";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " Box " + boxId + " does not have info for sensor " + sensorId + ".");
            }
        }		
		
		/// <summary>
		/// Delete all sensors related to the box.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="boxId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if box id does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteSensorsByBoxId(int boxId)
		{
			return DeleteRowsByIntField("BoxId",boxId, "box");		
		}
		/// <summary>
		/// Retrieves sensor info by box id
		/// </summary>
		/// <param name="boxId"></param> 
		/// <returns>DataSet [SensorId][SensorName][SensorAction][AlarmLevelOn][AlarmLevelOff]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetSensorsInfoByBoxId(int boxId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT SensorId,"+
							"CASE WHEN SensorName IS NOT NULL then RTRIM(SensorName) END AS SensorName,"+
							"CASE WHEN SensorAction IS NOT NULL then RTRIM(SensorAction) END AS SensorAction,"+
							"AlarmLevelOn,AlarmLevelOff FROM vlfBoxSensorsCfg WHERE BoxId=" + boxId;
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve sensor info by box id=" + boxId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve sensor info by box id=" + boxId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
		/// <summary>
		/// Retrieves sensor info structure by box id
		/// </summary>
		/// <param name="boxId"></param> 
		/// <returns>string[,] of [SensorId][SensorName][SensorAction][AlarmLevelOn][AlarmLevelOff]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string[,] GetSensorsInfoStructByBoxId(int boxId)
		{
			string[,] resultArr = null;
			DataSet sqlDataSet = GetSensorsInfoByBoxId(boxId);
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				resultArr = new string[sqlDataSet.Tables[0].Rows.Count,sqlDataSet.Tables[0].Columns.Count];
				int index = 0;
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					resultArr[index,0] = Convert.ToString(currRow[0]);
					resultArr[index,1] = Convert.ToString(currRow[1]).TrimEnd();
					resultArr[index,2] = Convert.ToString(currRow[2]).TrimEnd();
					resultArr[index,3] = Convert.ToString(currRow[3]);
					resultArr[index,4] = Convert.ToString(currRow[4]);
					++index;
				}
			}
			return resultArr;
		}
		
		/// <summary>
		/// Retrieves number of sensors by box id
		/// </summary>
		/// <param name="boxId"></param> 
		/// <returns>int </returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int GetNumberOfSensorsByBoxId(int boxId)
		{
			int retResult = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT COUNT(SensorId) FROM " + tableName + " WHERE BoxId=" + boxId;
				//Executes SQL statement
				retResult = (int)sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve number of sensors by box id=" + boxId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve number of sensors by box id=" + boxId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}
		/// <summary>
		/// Retrieves sensors names by box id.
		/// </summary>
		/// <param name="boxId"></param> 
		/// <returns>ArrayList</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public ArrayList GetSensorsNamesByBoxId(int boxId)
		{
			ArrayList resultList = null;
			DataSet sqlDataSet = GetFieldByBoxId("SensorName",boxId);;
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				resultList = new ArrayList(sqlDataSet.Tables[0].Rows.Count);
				//Retrieves info from Table[0].[0][0]
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					resultList.Add(Convert.ToString(currRow[0]).TrimEnd());
				}
			}
			return resultList;
		}
		/// <summary>
		/// Retrieves sensors ids by box id
		/// </summary>
		/// <param name="boxId"></param> 
		/// <returns>DataSet</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public ArrayList GetSensorsIdsArrayByBoxId(int boxId)
		{
			ArrayList resultList = null;
			DataSet sqlDataSet = GetFieldByBoxId("SensorId",boxId);
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				resultList = new ArrayList(sqlDataSet.Tables[0].Rows.Count);
				//Retrieves info from Table[0].[0][0]
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					resultList.Add(Convert.ToString(currRow[0]).TrimEnd());
				}
			}
			return resultList;
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
				string sql = "SELECT MaxSensorsNum" + 
					" FROM vlfBoxHwType" + 
					" WHERE BoxHwTypeId=" + boxHwTypeId;
				//Executes SQL statement
				maxSensors = Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve max number supported of sensors for Hw type " + boxHwTypeId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve max number supported of sensors for Hw type " + boxHwTypeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return maxSensors;
		}
		/// <summary>
		/// Retrieves current number of supported sensors for specific box id.
		/// Note: if box does not exist return VLF.CLS.Def.Const.unassignedIntValue
		/// </summary>
		/// <param name="boxId"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int GetCurrentSupportedSensorsByBoxId(int boxId)
		{
			int recordCount = 0;
			try
			{
				//Prepares SQL statement
				//string sql = "SELECT * FROM " + tableName;
				string sql = "SELECT COUNT(BoxId) FROM " + tableName + " WHERE BoxId=" + boxId;
				//Executes SQL statement
				recordCount =  Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve current number supported of sensors for box id=" + boxId + ". ";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve current number supported of sensors for box id=" + boxId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}				
			return recordCount;
		}
		/// <summary>
		/// Retrieves field by box id
		/// </summary>
		/// <param name="fieldName"></param> 
		/// <param name="boxId"></param> 
		/// <returns>DataSet</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected DataSet GetFieldByBoxId(string fieldName,int boxId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT " + fieldName + " FROM " + tableName + 
					" WHERE BoxId=" + boxId + " ORDER BY " + fieldName + " ASC";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve " + fieldName + " by box id=" + boxId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve " + fieldName + " by box id=" + boxId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
	}
}
