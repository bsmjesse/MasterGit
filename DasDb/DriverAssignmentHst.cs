using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using System.Text;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfDriverAssignment table.
	/// </summary>
	public class DriverAssignmentHst : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public DriverAssignmentHst(SQLExecuter sqlExec) : base ("vlfDriverAssignmentHst_1",sqlExec)
		{
		}

		/// <summary>
		/// Add driver assignment to the history.
		/// </summary>
		/// <param name="personId"></param>
		/// <param name="licensePlate"></param>
		/// <param name="description"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with license plate alredy exists</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
      [Obsolete("Deprecated - all in sp")]
/*
		public void AddDriverAssignment(string personId, string licensePlate, string description)
		{
			// 1. validates parameters
			if(	(personId == VLF.CLS.Def.Const.unassignedStrValue)||
				(licensePlate == VLF.CLS.Def.Const.unassignedStrValue))
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: person Id=" + 
					personId + " license plate=" + licensePlate);
			}

			int rowsAffected = 0;
			// 2. Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName +
				" (PersonId,LicensePlate,AssignedDateTime,Description)" +
				" VALUES ( '{0}','{1}','{2}','{3}')",
				personId.Replace("'","''"),
				licensePlate.Replace("'","''"),
				DateTime.Now,
				description.Replace("'","''"));
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 3. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 4. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add driver assignment with personId=" + personId + " license plate=" + licensePlate + " to the history.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add driver assignment with personId=" + personId + " license plate=" + licensePlate + " to the history.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add driver assignment with personId=" + personId + " license plate=" + licensePlate + " to the history.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This driver already assigned.");
			}
		}				

		/// <summary>
		/// Set driver assignment "DeletedDateTime" field
		/// </summary>
		/// <param name="personId"></param>
		/// <param name="licensePlate"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with license plate alredy exists</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
      [Obsolete("Deprecated - all in sp")]
      public void UpdateDriverAssignment(string personId, string licensePlate)
		{
			// 1. validates parameters
			if(	(personId == VLF.CLS.Def.Const.unassignedStrValue)||
				(licensePlate == VLF.CLS.Def.Const.unassignedStrValue))
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: person Id=" + 
					personId + " license plate=" + licensePlate);
			}

			int rowsAffected = 0;
			// 2. Prepares SQL statement
         string sql = "UPDATE vlfDriverAssignmentHst SET UnassignedDateTime='" + DateTime.Now + "'" +
						" WHERE PersonId='" + personId.Replace("'","''") + "'"+
						" AND LicensePlate='" + licensePlate.Replace("'","''") + "'" +
                  " AND UnassignedDateTime IS NULL";
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 3. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 4. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update driver assignment with personId=" + personId + " license plate=" + licensePlate + " to the history.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update driver assignment with personId=" + personId + " license plate=" + licensePlate + " to the history.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
		}				

		/// <summary>
		/// Set driver assignments "DeletedDateTime" field
		/// </summary>
		/// <param name="personId"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with license plate alredy exists</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
      [Obsolete("Deprecated - all in sp")]
      public void UpdateDriverAssignments(string personId)
		{
			// 1. validates parameters
			if(personId == VLF.CLS.Def.Const.unassignedStrValue)
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: person Id=" + personId);
			}

			int rowsAffected = 0;
			// 2. Prepares SQL statement
         string sql = "UPDATE vlfDriverAssignmentHst SET UnassignedDateTime='" + DateTime.Now + "'" +
            " WHERE PersonId='" + personId.Replace("'", "''") + "'" + " AND UnassignedDateTime IS NULL";
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 3. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 4. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update driver assignments with personId=" + personId + " to the history.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update driver assignments with personId=" + personId + " to the history.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to update driver assignments with personId=" + personId + " to the history.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This driver already assigned.");
			}
		}				
*/
      /// <summary>
		///         Delete all vehicle assignments from the history related to the person
		/// </summary>
		/// <param name="personId"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteDriverAssignments(string personId)
		{
            return DeleteRowsByStrField("PersonId", personId.Trim(), "from " + tableName);
		}

      /// <summary>
      /// Get driver assignments
      /// </summary>
      /// <param name="fieldName"></param>
      /// <param name="fieldValue"></param>
      /// <returns>DataSet [AssignedDateTime],[DeletedDateTime],[PersonId],[LicensePlate],[DriverLicense],[FirstName],[LastName],[Description]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllDriversAssignmentsBy(string fieldName, string fieldValue)
      {
         DataSet sqlDataSet = null;
         try
         {
            // 1. Prepares SQL statement to VehicleAssignment table.
            string sql = "SELECT AssignedDateTime, UnassignedDateTime, vlfPersonInfo.PersonId, LicensePlate, DriverLicense, FirstName, LastName, vlfDriverAssignmentHst.Description" +
               " FROM vlfDriverAssignmentHst_1, vlfPersonInfo" +
               " WHERE " + fieldName + "='" + fieldValue.Replace("'", "''") + "'" +
               " AND DeletedDateTime IS NOT NULL " +
               " AND vlfDriverAssignmentHst.PersonId = vlfPersonInfo.PersonId" +
               " ORDER BY AssignedDateTime DESC";
            // 2. Retrieves vehicle assignment information.
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all drivers assignment by " + fieldName + "=" + fieldValue;
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all drivers assignment by " + fieldName + "=" + fieldValue;
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return sqlDataSet;
      }



      # region Driver assignment history

      /// <summary>
      /// Add Driver Assignment History - both assignment and unassignment
      /// </summary>
      /// <param name="driverId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="userId"></param>
      /// <param name="description"></param>
      /// <param name="startDate"></param>
      /// <param name="endDate"></param>
      /// <returns></returns>
      public int AddDriverAssignmentHistory(int driverId, long vehicleId, int userId, string description, DateTime startDate, DateTime endDate)
      {
         int rows = 0;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Unable to add driver assignment history";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@driverid", SqlDbType.Int, driverId);
            sqlExec.AddCommandParam("@vehicleid", SqlDbType.BigInt, vehicleId);
            sqlExec.AddCommandParam("@userid", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, description);
            sqlExec.AddCommandParam("@startdt", SqlDbType.DateTime, startDate);
            sqlExec.AddCommandParam("@enddt", SqlDbType.DateTime, endDate);
            rows = sqlExec.SPExecuteNonQuery("DriverAddAssignmentHistory_1");
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
         return rows;
      }

      /// <summary>
      /// Get Driver Assignment History
      /// </summary>
      /// <param name="driverId">Driver Id</param>
      /// <returns>[vlfDriver.FirstName, vlfDriver.LastName, vlfDriver.License, vlfDriverAssignmentHst.LicensePlate, vlfVehicleInfo.[Description], 
      /// vlfDriverAssignmentHst.AssignedDateTime, vlfDriverAssignmentHst.Assigned]</returns>
      public DataSet GetAssignmentHistoryByDriverId(int driverId)
      {
         DataSet sqlDataSet = null;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Unable to retrieve driver assignment history";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@driverid", SqlDbType.Int, driverId);
            sqlDataSet = sqlExec.SPExecuteDataset("DriverGetDriverAssignmentHst_1");
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

      /// <summary>
      /// Get Driver Assignment History
      /// </summary>
      /// <param name="driverId">Driver Id</param>
      /// <param name="from">Date from</param>
      /// <param name="to">Date to</param>
      /// <returns>[vlfDriver.FirstName, vlfDriver.LastName, vlfDriver.License, vlfDriverAssignmentHst.LicensePlate, vlfVehicleInfo.[Description], 
      /// vlfDriverAssignmentHst.AssignedDateTime, vlfDriverAssignmentHst.Assigned]</returns>
      public DataSet GetAssignmentHistoryByDriverId(int driverId, DateTime from, DateTime to)
      {
         DataSet sqlDataSet = null;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Unable to retrieve driver assignment history";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@driverid", SqlDbType.Int, driverId);
            sqlExec.AddCommandParam("@from", SqlDbType.DateTime, from);
            sqlExec.AddCommandParam("@to", SqlDbType.DateTime, to);
            sqlDataSet = sqlExec.SPExecuteDataset("DriverGetDriverAssignmentHstDates_1");
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

      /// <summary>
      /// Get Driver Assignment History
      /// </summary>
      /// <param name="vehicleId">Vehicle Id</param>
      /// <returns>[vlfDriver.FirstName, vlfDriver.LastName, vlfDriver.License, vlfDriverAssignmentHst.LicensePlate, vlfVehicleInfo.[Description], 
      /// vlfDriverAssignmentHst.AssignedDateTime, vlfDriverAssignmentHst.Assigned]</returns>
      public DataSet GetAssignmentHistoryByVehicleId(long vehicleId)
      {
         DataSet sqlDataSet = null;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Unable to retrieve driver assignment history";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@vehicleid", SqlDbType.BigInt, vehicleId);
            sqlDataSet = sqlExec.SPExecuteDataset("DriverGetVehicleAssignmentHst_1");
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

      /// <summary>
      /// Get Driver Assignment History
      /// </summary>
      /// <param name="vehicleId">Vehicle Id</param>
      /// <param name="from">Date from</param>
      /// <param name="to">Date to</param>
      /// <returns>[vlfDriver.FirstName, vlfDriver.LastName, vlfDriver.License, vlfDriverAssignmentHst.LicensePlate, vlfVehicleInfo.[Description], 
      /// vlfDriverAssignmentHst.AssignedDateTime, vlfDriverAssignmentHst.Assigned]</returns>
      public DataSet GetAssignmentHistoryByVehicleId(long vehicleId, DateTime from, DateTime to)
      {
         DataSet sqlDataSet = null;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Unable to retrieve driver assignment history";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@vehicleid", SqlDbType.BigInt, vehicleId);
            sqlExec.AddCommandParam("@from", SqlDbType.DateTime, from);
            sqlExec.AddCommandParam("@to", SqlDbType.DateTime, to);
            sqlDataSet = sqlExec.SPExecuteDataset("DriverGetVehicleAssignmentHstDates_1");
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

      /// <summary>
      /// Get Driver Assignment History
      /// </summary>
      /// <param name="driverId">Driver Id</param>
      /// <param name="vehicleId">Vehicle Id</param>
      /// <returns>[vlfDriver.FirstName, vlfDriver.LastName, vlfDriver.License, vlfDriverAssignmentHst.LicensePlate, vlfVehicleInfo.[Description], 
	   /// vlfDriverAssignmentHst.AssignedDateTime, vlfDriverAssignmentHst.Assigned]</returns>
      public DataSet GetAssignmentHistoryByDriverIdVehicleId(int driverId, long vehicleId)
      {
         DataSet sqlDataSet = null;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Unable to retrieve driver assignment history";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@driverid", SqlDbType.Int, driverId);
            sqlExec.AddCommandParam("@vehicleid", SqlDbType.BigInt, vehicleId);
            sqlDataSet = sqlExec.SPExecuteDataset("DriverGetAssignmentHst_1");
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

      /// <summary>
      /// Get Driver Assignment History
      /// </summary>
      /// <param name="driverId">Driver Id</param>
      /// <param name="vehicleId">Vehicle Id</param>
      /// <param name="from">Date from</param>
      /// <param name="to">Date to</param>
      /// <returns>[vlfDriver.FirstName, vlfDriver.LastName, vlfDriver.License, vlfDriverAssignmentHst.LicensePlate, vlfVehicleInfo.[Description], 
      /// vlfDriverAssignmentHst.AssignedDateTime, vlfDriverAssignmentHst.Assigned]</returns>
      public DataSet GetAssignmentHistoryByDriverIdVehicleId(int driverId, long vehicleId, DateTime from, DateTime to)
      {
         DataSet sqlDataSet = null;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Unable to retrieve driver assignment history";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@driverid", SqlDbType.Int, driverId);
            sqlExec.AddCommandParam("@vehicleid", SqlDbType.BigInt, vehicleId);
            sqlExec.AddCommandParam("@from", SqlDbType.DateTime, from);
            sqlExec.AddCommandParam("@to", SqlDbType.DateTime, to);
            sqlDataSet = sqlExec.SPExecuteDataset("DriverGetAssignmentHstDates_1");
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

      /// <summary>
      /// Get Driver Assignment Vehicles FROM History
      /// </summary>
      /// <param name="driverId">Driver Id</param>
      /// <param name="from">Date from</param>
      /// <param name="to">Date to</param>
      /// <returns>[LicensePlate]</returns>
      public DataSet GetAssignmentVehiclesByDriverId(int driverId, DateTime from, DateTime to)
      {
         DataSet sqlDataSet = null;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Unable to retrieve driver vehicles history";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@driverid", SqlDbType.Int, driverId);
            sqlExec.AddCommandParam("@from", SqlDbType.DateTime, from);
            sqlExec.AddCommandParam("@to", SqlDbType.DateTime, to);
            sqlDataSet = sqlExec.SPExecuteDataset("DriverGetAssignedVehiclesHst");
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

      # endregion
	}
}
