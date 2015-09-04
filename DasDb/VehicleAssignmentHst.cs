using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.DB
{
	/// <summary>
   /// Provides interfaces to vlfVehicleAssignmentHst table.
	/// </summary>
	public class VehicleAssignmentHst : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public VehicleAssignmentHst(SQLExecuter sqlExec) : base ("vlfVehicleAssignmentHst", sqlExec)
		{
		}
		
		/// <summary>
		/// Adds vehicle assignment status to the history.
		/// </summary>
		/// <param name="vehicAssign"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with same datetime alredy exists</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddVehicleAssignment(VehicAssign vehicAssign)
		{
			// 1. validates parameters
			if (vehicAssign.licensePlate == VLF.CLS.Def.Const.unassignedStrValue ||
				vehicAssign.boxId == VLF.CLS.Def.Const.unassignedIntValue ||
				vehicAssign.vehicleId == VLF.CLS.Def.Const.unassignedIntValue)
			{
				throw new DASAppInvalidValueException(
               String.Format("Wrong value for insert SQL: LicensePlate={0} BoxId={1} VehicleId={2}", 
                  vehicAssign.licensePlate, vehicAssign.boxId, vehicAssign.vehicleId));
			}
         string prefixMsg = "", sql = "";
			int rowsAffected = 0;
			// 2. Prepares SQL statement
         try
         {
            prefixMsg = "Unable to add new vehicle assignment with license plate '" + vehicAssign.licensePlate + "' into the history.";
            /*
            sql = String.Format("INSERT INTO {0} (AssignedDateTime, LicensePlate, BoxId, VehicleId) VALUES(@dtAssigned, @plate, @boxId, @vehicleId)", tableName);
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@dtAssigned", SqlDbType.DateTime, DateTime.Now);
            sqlExec.AddCommandParam("@plate", SqlDbType.VarChar, vehicAssign.licensePlate.Replace("'", "''"));
            sqlExec.AddCommandParam("@boxId", SqlDbType.Int, vehicAssign.boxId);
            sqlExec.AddCommandParam("@vehicleId", SqlDbType.Int, vehicAssign.vehicleId);
            if (sqlExec.RequiredTransaction())
				{
					// 3. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 4. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            */
            rowsAffected = this.AddRow("(AssignedDateTime, LicensePlate, BoxId, VehicleId) VALUES(@dtAssigned, @plate, @boxId, @vehicleId)",
               new SqlParameter("@dtAssigned", DateTime.Now),
               new SqlParameter("@plate", vehicAssign.licensePlate.Replace("'", "''")),
               new SqlParameter("@boxId", vehicAssign.boxId),
               new SqlParameter("@vehicleId", vehicAssign.vehicleId));
         }
			catch (SqlException objException) 
			{
            Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
            throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This box already exists.");
			}
		}		

		/// <summary>
		/// Sets vehicle assignments "DeletedDateTime" field
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with same datetime alredy exists</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateVehicleAssignment(int boxId)
		{
			// 1. validates parameters
			if(boxId == VLF.CLS.Def.Const.unassignedIntValue)
			{
				throw new DASAppInvalidValueException("Invalid box Id = " + boxId);
			}
         string prefixMsg = "";
			int rowsAffected = 0;
			string sql = "SET DeletedDateTime = @dtDeleted WHERE BoxId = @boxId AND DeletedDateTime IS NULL";
			try
			{
            prefixMsg = String.Format("Unable to update vehicle assignment history with boxId = {0}.", boxId);
            
				//if(sqlExec.RequiredTransaction())
				//{
					// 3. Attach current command SQL to transaction
				   //	sqlExec.AttachToTransaction(sql);
				//}
				// 4. Executes SQL statement
				//rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            rowsAffected = this.UpdateRow(sql, new SqlParameter("@dtDeleted", DateTime.Now), new SqlParameter("@boxId", boxId));
         }
			catch (SqlException objException) 
			{
            Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
            throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This box already exists.");
			}
		}

		/// <summary>
		/// Gets vehicle assignment by ...
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <param name="orderBy"></param>
		/// <returns>DataSet [AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllVehiclesAssignmentsBy(string fieldName, string fieldValue, string orderBy)
		{
			DataSet sqlDataSet = null;
         string prefixMsg = "", sql = "";
			try
			{
            prefixMsg = "Unable to retreive all vehicles assignment by " + fieldName + "=" + fieldValue;
            
				// 1. Prepares SQL statement to VehicleAssignment table.
				sql = String.Format("SELECT * FROM {0} WHERE {1} = @param {2}", tableName, fieldName, orderBy);
            /*
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Retrieves vehicle assignment information.
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            */
            sqlDataSet = this.GetRowsBySql(sql, new SqlParameter("@param", fieldValue.Replace("'", "''")));
			}
			catch (SqlException objException) 
			{
            Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

		/// <summary>
		/// Gets vehicle assignment by ...
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <param name="orderBy"></param>
		/// <returns>DataSet [AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllVehiclesAssignmentsBy(string fieldName, Int64 fieldValue, string orderBy)
		{
			DataSet sqlDataSet = null;
         string prefixMsg = "", sql = "";
			try
			{
            prefixMsg = "Unable to retreive all vehicles assignment by " + fieldName + "=" + fieldValue;
            
				// 1. Prepares SQL statement to VehicleAssignment table.
            //string sql = "SELECT * FROM " + tableName +
            //   " WHERE (" + fieldName + "=" + fieldValue + ")" +
            //   orderBy;
            sql = String.Format("SELECT * FROM {0} WHERE {1} = @param {2}", tableName, fieldName, orderBy);
            /*
            if (sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Retrieves vehicle assignment information.
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            */
            sqlDataSet = this.GetRowsBySql(sql, new SqlParameter("@param", fieldValue));
         }
			catch (SqlException objException) 
			{
            Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

		/// <summary>
		/// Gets vehicle assignment by ...
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <param name="dtFrom"></param>
		/// <param name="dtTo"></param>
		/// <param name="orderBy"></param>
		/// <returns>DataSet [AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllVehiclesAssignmentsBy(string fieldName, string fieldValue, DateTime dtFrom, DateTime dtTo, string orderBy)
		{
			DataSet sqlDataSet = null;
         string prefixMsg = "", sql = "";
			try
			{
            prefixMsg = "Unable to retreive all vehicles assignment by " + fieldName + "=" + fieldValue;
            
				// 1. Prepares SQL statement to VehicleAssignment table.
            //string sql = "SELECT * FROM " + tableName +
            //         " WHERE " + fieldName + "='" + fieldValue.Replace("'","''") + "'" +
            //         " AND DeletedDateTime>='" + dtFrom + "' AND AssignedDateTime<='" + dtTo + "'"+
            //         " Order By AssignedDateTime";

            sql = String.Format("SELECT * FROM {0} WHERE {1} = @param AND DeletedDateTime >= @dtDeleted AND AssignedDateTime <= @dtAssigned ORDER BY AssignedDateTime", 
               tableName, fieldName);

				// 2. Retrieves vehicle assignment information.
				//sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            sqlDataSet = this.GetRowsBySql(sql,
               new SqlParameter("@param", fieldValue.Replace("'", "''")),
               new SqlParameter("@dtDeleted", dtFrom),
               new SqlParameter("@dtAssigned", dtTo));
			}
			catch (SqlException objException) 
			{
            Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
		
      /// <summary>
		/// Gets vehicle assignment by ...
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <param name="dtFrom"></param>
		/// <param name="dtTo"></param>
		/// <param name="orderBy"></param>
		/// <returns>DataSet [AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllVehiclesAssignmentsBy(string fieldName, Int64 fieldValue, DateTime dtFrom, DateTime dtTo, string orderBy)
		{
			DataSet sqlDataSet = null;
         string sql = "", prefixMsg = "";
			try
			{
            prefixMsg = "Unable to retreive all vehicles assignment by " + fieldName + "=" + fieldValue;
            
				// 1. Prepares SQL statement to VehicleAssignment table.
            //sql = "SELECT * FROM " + tableName + " WHERE " + fieldName + "=" + fieldValue + "" +
            //         " AND DeletedDateTime>='" + dtFrom + "' AND AssignedDateTime<='" + dtTo + "'"+
            //         " Order By AssignedDateTime";
            sql = String.Format("SELECT * FROM {0} WHERE {1} = @param AND DeletedDateTime >= @dtDeleted AND AssignedDateTime <= @dtAssigned ORDER BY AssignedDateTime",
               tableName, fieldName);


				// 2. Retrieves vehicle assignment information.
				//sqlDataSet = sqlExec.SQLExecuteDataset(sql);
             sqlDataSet = this.GetRowsBySql(sql,
               new SqlParameter("@param", fieldValue),
               new SqlParameter("@dtDeleted", dtFrom),
               new SqlParameter("@dtAssigned", dtTo));
			}
			catch (SqlException objException) 
			{
            Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
		
      /// <summary>
		/// Deletes all vehicle assignments from the history related to the box
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteAllVehicleAssignmentsForBox(int boxId)
		{
			int rowsAffected = 0;
         string prefixMsg = "";
			try
			{
            prefixMsg = "Unable to delete all box " + boxId + " assignments history.";
            
				// 1. Prepares SQL statement
            //string sql = "DELETE FROM vlfVehicleAssignmentHst WHERE BoxId=" + boxId;
            //if(sqlExec.RequiredTransaction())
            //{
            //   // 2. Attach current command SQL to transaction
            //   sqlExec.AttachToTransaction(sql);
            //}
				// 3. Executes SQL statement
            rowsAffected = this.DeleteRowsByField("BoxId", boxId);
               //sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
            Util.ProcessDbException(prefixMsg,objException);
			}

			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}

		/// <summary>
		/// Deletes all vehicle assignments from the history related to the vehicle
		/// </summary>
		/// <param name="vehicleId"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteAllVehicleAssignmentsForVehicle(Int64 vehicleId)
		{
			int rowsAffected = 0;
         string prefixMsg = "";
			try
			{
            prefixMsg = "Unable to delete all vehicle " + vehicleId + " assignments history.";
            
				// 1. Prepares SQL statement
            /*
				string sql = "DELETE FROM vlfVehicleAssignmentHst WHERE VehicleId=" + vehicleId;
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            */
            rowsAffected = this.DeleteRowsByField("VehicleId", vehicleId);
			}
			catch (SqlException objException) 
			{
            Util.ProcessDbException(prefixMsg,objException);
			}

			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}



        public DataSet GetAllVehiclesAssigmentByVehicleId(Int64 VehicleId, DateTime from, DateTime to)
        {
             //return GetFleetInfoBy("OrganizationName",organizationName);
         DataSet sqlDataSet = null;
         try
         {
            // 1. Prepares SQL statement
            string sql = "GetAllVehiclesAssigmentByVehicleId";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@vehicleId", SqlDbType.BigInt , VehicleId);
            sqlExec.AddCommandParam("@from", SqlDbType.DateTime , from);
             sqlExec.AddCommandParam("@to", SqlDbType.DateTime , to );

            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            sqlDataSet = sqlExec.SPExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve AllVehiclesAssigmentByVehicleId " + VehicleId + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
             string prefixMsg = "Unable to retrieve AllVehiclesAssigmentByVehicleId " + VehicleId + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return sqlDataSet;
        }
	}
}
