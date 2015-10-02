using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using System.Collections;	// for ArrayList


namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfVehicleAssignment table.
	/// </summary>
	public class VehicleAssignment : TblOneIntPrimaryKey
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public VehicleAssignment(SQLExecuter sqlExec) : base ("vlfVehicleAssignment", sqlExec)
		{
		}

		/// <summary>
		/// Add new vehicle assignment.
		/// </summary>
		/// <param name="vehicAssign"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with license plate alredy exists</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddVehicleAssignment(VehicAssign vehicAssign)
		{
			// 1. validates parameters
			if(	(vehicAssign.licensePlate == VLF.CLS.Def.Const.unassignedStrValue)||
				(vehicAssign.boxId == VLF.CLS.Def.Const.unassignedIntValue)||
				(vehicAssign.vehicleId == VLF.CLS.Def.Const.unassignedIntValue))
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: LicensePlate=" + 
					vehicAssign.licensePlate + 
					" BoxId=" + vehicAssign.boxId +
					" VehicleId=" + vehicAssign.vehicleId);
			}

			int rowsAffected = 0;
			// 2. Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName +
				" (LicensePlate,BoxId,VehicleId,AssignedDateTime)" +
				" VALUES ( '{0}',{1},{2},'{3}' )",
				vehicAssign.licensePlate.Replace("'","''"),
				vehicAssign.boxId,
				vehicAssign.vehicleId,
				DateTime.Now);
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
				string prefixMsg = "Unable to add new vehicle assignment with license plate '" + vehicAssign.licensePlate + "'.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new vehicle assignment with license plate '" + vehicAssign.licensePlate + "'.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new vehicle assignment with license plate '" + vehicAssign.licensePlate + "'.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This box already exists.");
			}
		}	
	
		/// <summary>
		/// Deletes existing vehicle assignment.
		/// </summary>
		/// <param name="licensePlate"></param> 
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteVehicleAssignment(string licensePlate)
		{
			return DeleteRowsByStrField("LicensePlate",licensePlate, "license plate");		
		}

		/// <summary>
		/// Deletes existing vehicle assignment.
		/// </summary>
		/// <param name="vehicleId"></param> 
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteVehicleAssignment(Int64 vehicleId)
		{
			return DeleteRowsByIntField("VehicleId",vehicleId, "vehicle Id");		
		}
		
		/// <summary>
		/// Returns vehicle assignment by license plate. 
		/// </summary>
		/// <param name="licensePlate"></param>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetVehicleAssignment(string licensePlate)
		{
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT * FROM " + tableName + 
							" WHERE (LicensePlate='" + licensePlate.Replace("'","''") + "')";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retreive vehicle assignment by license plate " + licensePlate + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retreive vehicle assignment by license plate " + licensePlate + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			return resultDataSet;
		}			

		/// <summary>
		/// Returns vehicle assignment structure by license plate. 
		/// </summary>
		/// <param name="licensePlate"></param>
		/// <returns>VehicAssign</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public VehicAssign GetVehicleAssignmentVA(string licensePlate)
		{
			DataSet sqlDataSet = GetVehicleAssignment(licensePlate);
			VehicAssign vehicAssign;
			vehicAssign.licensePlate = VLF.CLS.Def.Const.unassignedStrValue;
			vehicAssign.boxId	= VLF.CLS.Def.Const.unassignedIntValue;
			vehicAssign.vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
         if (Util.IsDataSetValid(sqlDataSet))
         {
            // Fill "VehicAssign" structure from dataset
            vehicAssign.licensePlate = Convert.ToString(sqlDataSet.Tables[0].Rows[0]["LicensePlate"]);
            vehicAssign.boxId = Convert.ToInt32(sqlDataSet.Tables[0].Rows[0]["BoxId"]);
            vehicAssign.vehicleId = Convert.ToInt64(sqlDataSet.Tables[0].Rows[0]["VehicleId"]);
         }
			return vehicAssign;
		}

		/// <summary>
		/// Gets all active assignments license plates.
		/// </summary>
		/// <returns>ArrayList</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public ArrayList GetAllLicencePlates()
		{
			ArrayList resultList = null;
			DataSet sqlDataSet = null;
			try
			{
				// 1. Retrieves vehicle assignment information.
				sqlDataSet = sqlExec.SQLExecuteDataset("SELECT LicensePlate FROM " + tableName + " ORDER BY LicensePlate");
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retreive all licence plates.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retreive all license plates.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				resultList = new ArrayList();
				//Retrieves info from Table[0]
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					resultList.Add(Convert.ToString(currRow[0]).TrimEnd());	//licensePlate
				}
			}
			return resultList;
		}

		/// <summary>
		/// Gets vehicle active assignment information by license plate as dataset
		///	In case of empty result returns null.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetVehicleAssignmentBy(string fieldName,Int64 fieldValue)
		{
			DataSet sqlDataSet = null;
			try
			{
				// lookup in the active assignments.
				// 1. Prepares SQL statement to VehicleAssignment table.
				string sql = "SELECT * FROM " + tableName +
					" WHERE (" + fieldName + "=" + fieldValue + ")";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Retrieves vehicle assignment information.
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retreive vehicle assignment information by " + fieldName + "=" + fieldValue + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retreive vehicle assignment information by " + fieldName + "=" + fieldValue + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

		/// <summary>
		/// Gets vehicle active assignment information by license plate as dataset
		///	In case of empty result returns null.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetVehicleAssignmentBy(string fieldName,string fieldValue)
		{
			DataSet sqlDataSet = null;
			try
			{
				// lookup in the active assignments.
				// 1. Prepares SQL statement to VehicleAssignment table.
				string sql = "SELECT * FROM " + tableName +
					" WHERE (" + fieldName + "='" + fieldValue.Replace("'","''") + "')";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Retrieves vehicle assignment information.
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retreive vehicle assignment information by " + fieldName + "=" + fieldValue + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retreive vehicle assignment information by " + fieldName + "=" + fieldValue + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

		/// <summary>
		/// Gets active Vehicle Assignment information ("resultFieldName") by specific "byFieldName"
		/// In case of empty result returns null.
		/// </summary>
		/// <param name="resultFieldName"></param>
		/// <param name="byFieldName"></param>
		/// <param name="byFieldValue"></param>
		/// <returns>object</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public object GetVehicleAssignmentField(string resultFieldName,string byFieldName,Int64 byFieldValue)
		{
			DataSet sqlDataSet = null;
			object retResult = null;
			try
			{
				// lookup in the active assignments.
				// 1. Prepares SQL statement to VehicleAssignment table.
				string sql = "SELECT " + resultFieldName + " FROM " + tableName +
							" WHERE (" + byFieldName + "=" + byFieldValue + ")";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Retrieves vehicle assignment information.
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retreive vehicle assignment information by " + byFieldName + "=" + byFieldValue + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retreive vehicle assignment information by " + byFieldName + "=" + byFieldValue + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if((sqlDataSet != null)&&(sqlDataSet.Tables.Count > 0)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				retResult = sqlDataSet.Tables[0].Rows[0][0];
			}
			return retResult;
		}

		/// <summary>
		/// Gets active Vehicle Assignment information ("resultFieldName") by specific "byFieldName"
		/// In case of empty result returns null.
		/// </summary>
		/// <param name="resultFieldName"></param>
		/// <param name="byFieldName"></param>
		/// <param name="byFieldValue"></param>
		/// <returns>object</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public object GetVehicleAssignmentField(string resultFieldName,string byFieldName,string byFieldValue)
		{
			DataSet sqlDataSet = null;
			object retResult = null;
			try
			{
				// lookup in the active assignments.
				// 1. Prepares SQL statement to VehicleAssignment table.
				string sql = "SELECT " + resultFieldName + " FROM " + tableName +
							" WHERE (" + byFieldName + "='" + byFieldValue.Replace("'","''") + "')";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Retrieves vehicle assignment information.
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retreive vehicle assignment information by " + byFieldName + "=" + byFieldValue + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retreive vehicle assignment information by " + byFieldName + "=" + byFieldValue + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if((sqlDataSet != null)&&(sqlDataSet.Tables.Count > 0)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				retResult = sqlDataSet.Tables[0].Rows[0][0];
			}
			return retResult;
		}

		/// <summary>
		/// Checks active vehicle assignment by field
		/// If exists return true, otherwise return false
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <returns>bool</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool IsActiveAssignmentBy(string fieldName,Int64 fieldValue)
		{
			bool retResult = false;
			int sqlResult = 0;
			//Prepares SQL statement
			string sql = "SELECT COUNT(" + fieldName + ") FROM " + tableName +
				" WHERE (" + fieldName + "=" + fieldValue + ")";
			try
			{
				//Executes SQL statement
				sqlResult = (int)sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to retreive DateTime by " + fieldName + "=" + fieldValue + ". ";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to retreive DateTime by " + fieldName + "=" + fieldValue + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			// Retrieves second row from SQL result.
			if(sqlResult > 0)
			{
				retResult = true;
			}
			return retResult;
		}

		/// <summary>
		/// Checks active vehicle assignment by field
		/// If exists return true, otherwise return false
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <returns>bool</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool IsActiveAssignmentBy(string fieldName,string fieldValue)
		{
			bool retResult = false;
			int sqlResult = 0;
			//Prepares SQL statement
			string sql = "SELECT COUNT(" + fieldName + ") FROM " + tableName +
				" WHERE (" + fieldName + "='" + fieldValue.Replace("'","''") + "')";
			try
			{
				//Executes SQL statement
				sqlResult = (int)sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to retreive DateTime by " + fieldName + "=" + fieldValue + ". ";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to retreive DateTime by " + fieldName + "=" + fieldValue + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			// Retrieves second row from SQL result.
			if(sqlResult > 0)
			{
				retResult = true;
			}
			return retResult;
		}

		/// <summary>
		/// Gets all Vehicles active assignment information
		/// </summary>
		/// <remarks>
		/// TableName	= "AllActiveVehiclesAssignments"
		/// DataSetName = "Vehicle"
		/// </remarks>
		/// <returns>DataSet [LicensePlate][BoxId][VehicleId]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllVehiclesActiveAssignments(int organizationId)
		{
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT vlfVehicleAssignment.LicensePlate,vlfVehicleAssignment.BoxId,vlfVehicleAssignment.VehicleId"+
                     " FROM vlfBox with (nolock) INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId=vlfVehicleAssignment.BoxId" +
							" WHERE vlfBox.OrganizationId=" + organizationId;
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve vehicle assignment for organization " + organizationId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve vehicle assignment for organization " + organizationId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			return resultDataSet;
		}					

      /// <summary>
      /// Get Vehicle Id
      /// </summary>
      /// <param name="licensePlate">Vehicle License Plate</param>
      /// <returns>Vehicle Id or -1 if not found</returns>
      public long GetVehicleIdByLicensePlate(string licensePlate)
      {
         object result = null;
         long vid = -1;

         try
         {
            result = base.GetValueByFilter("VehicleId", "LicensePlate = @plate", new SqlParameter("@plate", licensePlate));
            if (result != null) 
               vid = Convert.ToInt64(result);
         }
         catch
         {
         }

         return vid;
      }
	}
}
