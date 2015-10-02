using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfDriverAssignment table.
	/// </summary>
	public class DriverAssignment : TblGenInterfaces 
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public DriverAssignment(SQLExecuter sqlExec) : base ("vlfDriverAssignment", sqlExec)
		{
		}

		/// <summary>
		/// Add new driver assignment.
		/// </summary>
		/// <param name="personId"></param>
		/// <param name="licensePlate"></param>
		/// <param name="description"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with license plate alredy exists</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
/*      
      [Obsolete("Replaced by the overloaded method")]
		public void AddDriverAssignment(string personId, string licensePlate, string description)
		{
			// 1. validates parameters
			if((personId == VLF.CLS.Def.Const.unassignedStrValue)||
				(licensePlate == VLF.CLS.Def.Const.unassignedStrValue))
			{
				throw new DASAppInvalidValueException(
               String.Format("Wrong value for Insert SQL: Person Id={0} License Plate={1}", personId, licensePlate));
			}

			int rowsAffected = 0;
			// 2. Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName +
				" (PersonId,LicensePlate,AssignedDateTime,Description)" +
				" VALUES ( '{0}','{1}','{2}','{3}' )",
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
				string prefixMsg = "Unable to add new driver assignment with personId=" + personId + " license plate=" + licensePlate + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new driver assignment with personId=" + personId + " license plate=" + licensePlate + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new driver assignment with personId=" + personId + " license plate=" + licensePlate + ".";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This driver already assigned.");
			}
		}
*/
      /// <summary>
      /// Add new driver assignment + history
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="driverId"></param>
      /// <param name="description"></param>
      /// <returns>rows</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with license plate alredy exists</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int AddDriverAssignment(int userId, long vehicleId, int driverId, string description)
      {
         return AddDriverAssignment(userId, vehicleId, driverId, DateTime.UtcNow, description);
      }

      /// <summary>
      /// Add new driver assignment + history
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="driverId"></param>
      /// <param name="description"></param>
      /// <returns>rows</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with license plate alredy exists</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int AddDriverAssignment(int userId, long vehicleId, int driverId, DateTime assgnDate, string description)
      {
         string sql = "DriverAssign_1";

         if (sqlExec.RequiredTransaction())
         {
            // 3. Attach current command SQL to transaction
            sqlExec.AttachToTransaction(sql);
         }
         // 4. Executes SQL statement
         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@userid", SqlDbType.Int, userId);
         sqlExec.AddCommandParam("@vehicleid", SqlDbType.BigInt, vehicleId);
         sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);
         sqlExec.AddCommandParam("@assgndt", SqlDbType.DateTime, assgnDate);
         sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, description);

         return sqlExec.SPExecuteNonQuery(sql);
      }

   	/// <summary>
		/// Delete existing driver assignment.
		/// </summary>
      /// <param name="userId"></param> 
      /// <param name="vehicleId"></param> 
      /// <param name="driverId"></param> 
      /// <param name="description"></param> 
      /// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteDriverAssignment(int userId, long vehicleId, int driverId, string description)
		{
         string sql = "DriverUnassign_1";

         if (sqlExec.RequiredTransaction())
         {
            // 3. Attach current command SQL to transaction
            sqlExec.AttachToTransaction(sql);
         }
         // 4. Executes SQL statement
         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@userid", SqlDbType.Int, userId);
         sqlExec.AddCommandParam("@vehicleid", SqlDbType.BigInt, vehicleId);
         sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);
         sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, description);

         return sqlExec.SPExecuteNonQuery(sql);
      }
		
		/// <summary>
		/// Delete all driver assignments.
		/// </summary>
      /// <param name="userId"></param> 
      /// <param name="driverId"></param> 
      /// <param name="description"></param> 
      /// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteDriverAssignments(int userId, int driverId, string description)
		{
         string sql = "DriverUnassignAll_1";

         if (sqlExec.RequiredTransaction())
         {
            // 3. Attach current command SQL to transaction
            sqlExec.AttachToTransaction(sql);
         }
         // 4. Executes SQL statement
         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@userid", SqlDbType.Int, userId);
         sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);
         sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, description);

         return sqlExec.SPExecuteNonQuery(sql);
      }

      /// <summary>
      /// Delete all driver assignments.
      /// </summary>
      /// <param name="personId"></param> 
      /// <returns>rows affected</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteDriverAssignments(string personId)
      {
         return DeleteRowsByStrField("PersonId", personId, "person Id");
      }
	
		/// <summary>
		/// Returns driver assignment by vehicle Id
		/// </summary>
      /// <param name="vehicleId"></param>
		/// <returns>DataSet [AssignedDateTime],[PersonId],[DriverLicense],[FirstName],[LastName],[Description]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetDriverAssignmentForVehicle(long vehicleId)
		{
         string sql = "DriverGetAssignedPerVehicle";

         if (sqlExec.RequiredTransaction())
         {
            // 3. Attach current command SQL to transaction
            sqlExec.AttachToTransaction(sql);
         }

         // 4. Executes SQL statement
         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@vehicleid", SqlDbType.BigInt, vehicleId);

         return sqlExec.SPExecuteDataset(sql);
		}	

		/// <summary>
		/// Returns all driver assignments by driver Id. 
		/// </summary>
      /// <param name="driverId"></param>
		/// <returns>DataSet [AssignedDateTime],[PersonId],[LicensePlate],[DriverLicense],[FirstName],[LastName],[Description]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetDriverAssignments(int driverId)
		{
         string sql = "DriverGetAssignments";

         if (sqlExec.RequiredTransaction())
         {
            // 3. Attach current command SQL to transaction
            sqlExec.AttachToTransaction(sql);
         }

         // 4. Executes SQL statement
         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@driverId", SqlDbType.BigInt, driverId);

         return sqlExec.SPExecuteDataset(sql);
      }

      /// <summary>
      /// Returns all driver assignments by lic. plate
      /// </summary>
      /// <param name="licensePlate"></param>
      /// <returns> returns DataSet 
      ///            [VehicleId]
      ///            [DriverId],
      ///            [LicensePlate],
      ///            [AssigneDateTime],
      ///            [Description],
      ///            [PersonId]
      /// </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetDriverAssignment(string licensePlate)
      {
         return GetRowsByStringField("LicensePlate", licensePlate, "License Plate");
      }

      /// <summary>
      /// Retrieves all unassigned drivers per organization
      /// </summary>
      /// <returns>
      /// </returns>
      /// <param name="organizationId"></param> 
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetUnassignedDriversForOrganization(int organizationId)
      {
         // SQL statement
         string sql = "DriverGetUnassignedPerOrganization";

         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@orgId", SqlDbType.Int, organizationId);

         //Executes SQL statement
         return sqlExec.SPExecuteDataset(sql);
      }

      /// <summary>
      /// Retrieves all assigned drivers per organization
      /// </summary>
      /// <returns>
      /// </returns>
      /// <param name="organizationId"></param> 
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAssignedDriversForOrganization(int organizationId)
      {
         // SQL statement
         string sql = "DriverGetAssignedPerOrganization";

         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@orgId", SqlDbType.Int, organizationId);

         //Executes SQL statement
         return sqlExec.SPExecuteDataset(sql);
      }

      /// <summary>
      /// Delete existing driver assignment by license plate.
      /// </summary>
      /// <param name="licensePlate"></param> 
      /// <returns>rows affected</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteDriverAssignment(string licensePlate)
      {
         return DeleteRowsByStrField("LicensePlate", licensePlate, "License Plate");
      }

      /// <summary>
      ///         Change current assignment
      ///         this essentially moves the current driver assignment in vlfDriverAssignmentHst_1
      ///         and add a new entry in vlfDriverAssigment
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="driverId"></param>
      /// <param name="assgnDate"></param>
      /// <param name="description"></param>
      /// <returns></returns>
      public int ForceDriverAssignment(int userId, long vehicleId, int driverId, DateTime assgnDate, string description)
      {
         string sql = "DriverAssignForced";

         if (sqlExec.RequiredTransaction())
         {
            // 3. Attach current command SQL to transaction
            sqlExec.AttachToTransaction(sql);
         }
         // 4. Executes SQL statement
         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@userid", SqlDbType.Int, userId);
         sqlExec.AddCommandParam("@vehicleid", SqlDbType.BigInt, vehicleId);
         sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);
         sqlExec.AddCommandParam("@assgndt", SqlDbType.DateTime, assgnDate);
         sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, description.Replace("'", "''"));

         return sqlExec.SPExecuteNonQuery(sql);
      }

        public string AssignDriversInBulk(int userId, string xml)
        {
            string sql = "sp_DriverAssigmentBulkInsert";
            string errCode = string.Empty;

            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@userid", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@doc", SqlDbType.VarChar, xml);
            sqlExec.AddCommandParam("@errorCode", SqlDbType.VarChar, ParameterDirection.Output, 1, errCode);
            sqlExec.SPExecuteNonQuery(sql);
            return (DBNull.Value == sqlExec.ReadCommandParam("@errorCode")) ? string.Empty : sqlExec.ReadCommandParam("@errorCode").ToString();

        }

        /// <summary>
        /// Retrive Driver by BoxId
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public int GetDriverIdByBoxId(int boxId)
        {
            int driverId = 0;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT TOP 1 vlfDriverAssignment.DriverId " +
                             " FROM   vlfDriverAssignment INNER JOIN " +
                             " vlfVehicleAssignment ON vlfDriverAssignment.VehicleId = vlfVehicleAssignment.VehicleId " +
                             " WHERE vlfVehicleAssignment.BoxId=" + boxId;

                //Executes SQL statement
                object result = sqlExec.SQLExecuteScalar(sql);
                if (result!=null && !Convert.IsDBNull(result))
                    driverId = (int)result;
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve driver by boxId=" + boxId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve driver by boxId=" + boxId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return driverId;
        }


        /// <summary>
        /// Add new driver assignment + history
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="driverId"></param>
        /// <param name="description"></param>
        /// <returns>rows</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with license plate alredy exists</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int AddHOSDriverAssignment(int userId, long vehicleId, int driverId, DateTime assgnDate, string description)
        {
            string sql = "DriverAssign_HOS";

            if (sqlExec.RequiredTransaction())
            {
                // 3. Attach current command SQL to transaction
                sqlExec.AttachToTransaction(sql);
            }
            // 4. Executes SQL statement
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@userid", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@vehicleid", SqlDbType.BigInt, vehicleId);
            sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);
            sqlExec.AddCommandParam("@assgndt", SqlDbType.DateTime, assgnDate);
            sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, description);

            return sqlExec.SPExecuteNonQuery(sql);
        }

       

        /// <summary>
        /// Delete existing driver assignment.
        /// </summary>
        /// <param name="userId"></param> 
        /// <param name="vehicleId"></param> 
        /// <param name="driverId"></param> 
        /// <param name="description"></param> 
        /// <returns>rows affected</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteHOSDriverAssignment(int userId, long vehicleId, int driverId, string description)
        {
            string sql = "DriverUnassign_HOS";

            if (sqlExec.RequiredTransaction())
            {
                // 3. Attach current command SQL to transaction
                sqlExec.AttachToTransaction(sql);
            }
            // 4. Executes SQL statement
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@userid", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@vehicleid", SqlDbType.BigInt, vehicleId);
            sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);
            sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, description);

            return sqlExec.SPExecuteNonQuery(sql);
        }


        /// <summary>
        /// Add new driver assignment + history
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="driverId"></param>
        /// <param name="description"></param>
        /// <returns>rows</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with license plate alredy exists</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int AddHOSDriverAssignment(int userId, long vehicleId, int driverId, string description)
        {
            return AddHOSDriverAssignment(userId, vehicleId, driverId, DateTime.UtcNow, description);
        }


        /// <summary>
        /// Assign a skill to driver skill
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="organizationId"></param>
        /// <param name="skillId"></param>
        /// <param name="oldSkillId"></param>
        /// <param name="description"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        public DataSet AssignDriverSkill(int driverId, int organizationId, int skillId, int oldSkillId, string description = null, int delete = 0)
        {
            DataSet rowAffected = null;
            try
            {
                string sql = "exec vlfSaveDriverSkill @driverId, @organizationId, @skillId, @oldSkillId, @dsdescription, @delete";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);
                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@skillId", SqlDbType.Int, skillId);
                sqlExec.AddCommandParam("@oldSkillId", SqlDbType.Int, oldSkillId);
                sqlExec.AddCommandParam("@dsdescription", SqlDbType.VarChar, description);
                sqlExec.AddCommandParam("@delete", SqlDbType.Int, delete);
                rowAffected = sqlExec.SQLExecuteDataset(sql);//sqlExec.SQLExecuteDataset(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to save driver skill, Driver=" + driverId + ", SkillID=" + skillId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to driver skill, driver id=" + driverId + ", skill id=" + skillId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowAffected;

        }


        /// <summary>
        /// get the list of driver skills for the organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataSet GetDriverSkills(int organizationId)
        {
            // SQL statement
            string sql = "DriversGetSkillsPerOrganization";

            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@orgId", SqlDbType.Int, organizationId);

            //Executes SQL statement
            return sqlExec.SPExecuteDataset(sql);
        }



   }
}
