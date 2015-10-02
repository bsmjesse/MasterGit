using System;
using System.Data ;			   // for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using System.Collections.Generic;

namespace VLF.DAS.Logic
{
	/// <summary>
	/// Provides interface to driver functionality in database
	/// </summary>
	public class DriverManager : Das
	{
      private VLF.DAS.DB.Driver _driver = null;
      private VLF.DAS.DB.DriverAssignment _driverAssignment = null;
		private VLF.DAS.DB.DriverAssignmentHst _driverAssignmentHst = null;

	  #region General Interfaces

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString"></param>
      public DriverManager(string connectionString) : base(connectionString)
		{
         _driver = new VLF.DAS.DB.Driver(sqlExec);
         _driverAssignment = new VLF.DAS.DB.DriverAssignment(sqlExec);
			_driverAssignmentHst = new DriverAssignmentHst(sqlExec);
		}

/*
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlEx"></param>
        public DriverManager(SQLExecuter sqlEx)
            : base(sqlEx.ConnectionString)
        {
            sqlExec = sqlEx;
            _driver = new VLF.DAS.DB.Driver(sqlExec);
            _driverAssignment = new VLF.DAS.DB.DriverAssignment(sqlExec);
            _driverAssignmentHst = new DriverAssignmentHst(sqlExec);
        }
        */
		/// <summary>
		/// Destructor
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();
		}

		#endregion

      # region Driver general

      /// <summary>
      /// Add a new driver
      /// </summary>
      /// <param name="firstName"></param>
      /// <param name="lastName"></param>
      /// <param name="license"></param>
      /// <param name="classLicense"></param>
      /// <param name="licenseIssued"></param>
      /// <param name="licenseExpired"></param>
      /// <param name="orgId"></param>
      /// <param name="gender"></param>
      /// <param name="height"></param>
      /// <param name="homePhone"></param>
      /// <param name="cellPhone"></param>
      /// <param name="additionalPhone"></param>
      /// <param name="smsPwd"></param>       
      /// <param name="smsid"></param>
      /// <param name="email"></param>
      /// <param name="address"></param>
      /// <param name="city"></param>
      /// <param name="zipcode"></param>
      /// <param name="state"></param>
      /// <param name="country"></param>
      public void AddDriver(string firstName, string lastName, string license, string classLicense, DateTime licenseIssued,
         DateTime licenseExpired, int orgId, char gender, short height, string homePhone, string cellPhone, string additionalPhone,
         string smsPwd,
         string smsid, string email, string address, string city, string zipcode, string state, string country, string description)
      {
         int rowsAffected = 0;
         try
         {
            # region Validate params

            ValidateField("First name", firstName, 1, 50);
            ValidateField("Last name", lastName, 1, 50);
            ValidateField("License", license, 1, 50);
            ValidateField("Class", classLicense, 1, 20);
            ValidateField("OrganizationId", orgId, 1, System.Int32.MaxValue);
            ValidateField("Height", height, 0, System.Int16.MaxValue);
            ValidateField("Home Phone", homePhone, 0, 20);
            ValidateField("Cell Phone", cellPhone, 0, 20);
            ValidateField("Additional Phone", additionalPhone, 0, 20);
            ValidateField("SMSPwd", smsPwd, 0, 50);
            ValidateField("SMSID", smsid, 0, 50);
            ValidateField("E-mail", email, 0, 50);
            ValidateField("Address", address, 0, 100);
            ValidateField("City", city, 0, 50);
            ValidateField("Zip code", zipcode, 0, 20);
            ValidateField("State", state, 0, 50);
            ValidateField("Country", country, 0, 50);
            ValidateField("Description", description, 0, 100);

            # endregion

            rowsAffected = _driver.AddDriver(
               firstName, lastName, license, classLicense, licenseIssued, licenseExpired, orgId, gender, height, homePhone,
               cellPhone, additionalPhone, smsPwd, smsid, email, address, city, zipcode, state, country, description);

            if (rowsAffected == 0)
            {
               throw new DASAppDataAlreadyExistsException("Cannot add a new driver. This person already exists.");
            }
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("Unable to add a new driver ", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(objException.Message);
         }
      }
            
      /// <summary>
      /// 
      /// </summary>
      /// <param name="driverInfo"></param>
      public void CreateDriver(DriverInfo driverInfo)
      {
          AddDriver(driverInfo.firstName, driverInfo.lastName, driverInfo.license, driverInfo.classLicense,
              driverInfo.licenseIssued, driverInfo.licenseExpired, driverInfo.orgId, driverInfo.gender, driverInfo.height, driverInfo.homePhone,
              driverInfo.cellPhone, driverInfo.additionalPhone, driverInfo.smsPwd, driverInfo.smsid, driverInfo.email, driverInfo.address,
              driverInfo.city, driverInfo.zipcode, driverInfo.state, driverInfo.country, driverInfo.description);
      }

      /// <summary>
      /// Add multiple Drivers
      /// </summary>
      /// <param name="dataTable">Table containing drivers info</param>
      public void AddDrivers(DataTable dataTable)
      {
         int rowsAffected = 0;
         try
         {
            //this._driver.AddDrivers(dataTable);
            foreach (DataRow row in dataTable.Rows)
            {
               rowsAffected += this._driver.AddDriver(row);
            }
            if (rowsAffected == 0)
               throw new DASDbException("Failed to add drivers");
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("Unable to add drivers ", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(objException.Message);
         }
      }

      /// <summary>
      /// Update driver data
      /// </summary>
      /// <param name="driverId"></param>
      /// <param name="firstName"></param>
      /// <param name="lastName"></param>
      /// <param name="license"></param>
      /// <param name="classLicense"></param>
      /// <param name="licenseIssued"></param>
      /// <param name="licenseExpired"></param>
      /// <param name="orgId"></param>
      /// <param name="gender"></param>
      /// <param name="height"></param>
      /// <param name="homePhone"></param>
      /// <param name="cellPhone"></param>
      /// <param name="additionalPhone"></param>
      /// <param name="smsPwd"></param>       
      /// <param name="smsid"></param>
      /// <param name="email"></param>
      /// <param name="address"></param>
      /// <param name="city"></param>
      /// <param name="zipcode"></param>
      /// <param name="state"></param>
      /// <param name="country"></param>
      public void UpdateDriver(int driverId, string firstName, string lastName, string license, string classLicense, DateTime licenseIssued,
         DateTime licenseExpired, int orgId, char gender, short height, string homePhone, string cellPhone, string additionalPhone,
         string smsPwd,
         string smsid, string email, string address, string city, string zipcode, string state, string country, string description)
      {
         int rowsAffected = 0;
         string prefixMsg = "Unable to update driver by Id=" + driverId.ToString() + ".";
         try
         {
            # region Validate params

            ValidateField("DriverId", driverId, 1, System.Int32.MaxValue);
            ValidateField("First name", firstName, 1, 50);
            ValidateField("Last name", lastName, 1, 50);
            ValidateField("License", license, 1, 50);
            ValidateField("Class", classLicense, 1, 20);
            ValidateField("OrganizationId", orgId, 1, System.Int32.MaxValue);
            ValidateField("Height", height, 0, System.Int16.MaxValue);
            ValidateField("Home Phone", homePhone, 0, 20);
            ValidateField("Cell Phone", cellPhone, 0, 20);
            ValidateField("Additional Phone", additionalPhone, 0, 20);
            ValidateField("SMSPwd", smsPwd, 0, 50);
            ValidateField("SMSID", smsid, 0, 50);
            ValidateField("E-mail", email, 0, 50);
            ValidateField("Address", address, 0, 100);
            ValidateField("City", city, 0, 50);
            ValidateField("Zip code", zipcode, 0, 20);
            ValidateField("State", state, 0, 50);
            ValidateField("Country", country, 0, 50);
            ValidateField("Description", description, 0, 100);

            # endregion

            rowsAffected = _driver.UpdateDriver(driverId,
               firstName, lastName, license, classLicense, licenseIssued, licenseExpired, orgId, gender, height, homePhone,
               cellPhone, additionalPhone, smsPwd, smsid, email, address, city, zipcode, state, country, description);
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
            throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This person does not exist.");
         }
      }


      /// <summary>
      /// 
      /// </summary>
      /// <param name="driverInfo"></param>
      public void UpdateDriver(DriverInfo driverInfo)
      {
          UpdateDriver(driverInfo.driverId, driverInfo.firstName, driverInfo.lastName, driverInfo.license, driverInfo.classLicense,
              driverInfo.licenseIssued, driverInfo.licenseExpired, driverInfo.orgId, driverInfo.gender, driverInfo.height, driverInfo.homePhone,
              driverInfo.cellPhone, driverInfo.additionalPhone, driverInfo.smsPwd, driverInfo.smsid, driverInfo.email, driverInfo.address, driverInfo.city,
              driverInfo.zipcode, driverInfo.state, driverInfo.country, driverInfo.description);
      }

      /// <summary>
      /// Delete driver
      /// </summary>
      /// <param name="driverId"></param>
      public int DeleteDriver(int driverId)
      {
         int rowsAffected = 0;
         string prefixMsg = "Unable to delete driver by Id=" + driverId.ToString() + ".";
         try
         {
            ValidateField("DriverId", driverId, 1, System.Int32.MaxValue);
            rowsAffected = _driver.DeleteDriver(driverId);
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
            throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This person does not exist.");
         }
         return rowsAffected;
      }

      /// <summary>
      /// Retrieves driver info
      /// </summary>
      /// <returns>
      /// </returns>
      /// <param name="driverId"></param> 
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetDriver(int driverId)
      {
         DataSet sqlDataSet = null;
         string prefixMsg = "Unable to retrieve driver Id=" + driverId.ToString() + ".";
         try
         {
            sqlDataSet = _driver.GetDriverById(driverId);
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
      /// Get all drivers for the organization
      /// </summary>
      /// <param name="orgId"></param>
      /// <returns></returns>
      public DataSet GetDriversForOrganization(int orgId)
      {
         DataSet sqlDataSet = null;
         string prefixMsg = "Unable to retrieve drivers by organization id=" + orgId.ToString() + ".";
         try
         {
            sqlDataSet = _driver.GetAllDriversForOrganization(orgId);
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
      /// Get all drivers for the organization
      /// </summary>
      /// <param name="orgId"></param>
      /// <returns></returns>
      public DataSet GetAllDriversForOrganizationByUser(int userId,string keyFobId)
      {
          DataSet sqlDataSet = null;
          string prefixMsg = "Unable to retrieve drivers by user id=" + userId.ToString() + ".";
          try
          {
              sqlDataSet = _driver.GetAllDriversForOrganizationByUser(userId, keyFobId);
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

      #region Driver Assignment Interfaces

      /// <summary>
		/// Add new driver assignment.
		/// </summary>
		/// <param name="personId"></param>
		/// <param name="licensePlate"></param>
		/// <param name="description"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if driver assignment with license plate alredy exists</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
/*       
      [Obsolete("Method replaced by the overloaded method AddDriverAssignment(int userId, long vehicleId, string personId, string description)", true)]
		public void AddDriverAssignment(string personId, string licensePlate, string description)
		{
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				
				// 2. Assign driver to license plate
				_driverAssignment.AddDriverAssignment(personId,licensePlate,description);
				
				// 3. Add new driver/license plate assignment to the history
				_driverAssignmentHst.AddDriverAssignment(personId,licensePlate,description);
				
				// 4. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 4. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to add new driver ", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 6. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 4. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}
		}
*/
      /// <summary>
      /// Add new driver assignment.
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="driverId"></param>
      /// <param name="description"></param>
      /// <returns>void</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if driver assignment with license plate alredy exists</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public void AddDriverAssignment(int userId, long vehicleId, int driverId, string description)
      {
         int rowsAffected = 0;
         try
         {
            rowsAffected = _driverAssignment.AddDriverAssignment(userId, vehicleId, driverId, description);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(String.Format("Unable to add a new driver assignment with driver Id={0} Vehicle Id={1}.",
               driverId, vehicleId), objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(String.Format("Unable to add a new driver assignment with driver Id={0} Vehicle Id={1}. Error: {2}",
               driverId, vehicleId, objException.Message));
         }
         if (rowsAffected == 0)
         {
            throw new DASAppDataAlreadyExistsException(
               String.Format("Unable to add a new driver assignment with driver Id={0} Vehicle Id={1}. This driver is already assigned.",
               driverId, vehicleId));
         }
      }

      /// <summary>
      /// Add new driver assignment.
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="driverId"></param>
      /// <param name="assgnDate"></param>
      /// <param name="description"></param>
      /// <returns>void</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if driver assignment with license plate alredy exists</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public void AddDriverAssignment(int userId, long vehicleId, int driverId, DateTime assgnDate, string description)
      {
         int rowsAffected = 0;
         try
         {
            rowsAffected = _driverAssignment.AddDriverAssignment(userId, vehicleId, driverId, assgnDate, description);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(String.Format("Unable to add a new driver assignment with driverId={0} Vehicle Id={1}.",
               driverId, vehicleId), objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(String.Format("Unable to add a new driver assignment with driverId={0} Vehicle Id={1}. Error: {2}",
               driverId, vehicleId, objException.Message));
         }
         if (rowsAffected == 0)
         {
            throw new DASAppDataAlreadyExistsException(
               String.Format("Unable to add a new driver assignment with driverId={0} Vehicle Id={1}. This driver is already assigned.",
               driverId, vehicleId));
         }
      }
      
      /// <summary>
		/// Delete existing driver assignment.
		/// </summary>
		/// <param name="licensePlate"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASException">Thrown DASException in exception cases.</exception>
      /// <param name="personId"></param>
/*       
      [Obsolete("Method replaced by the overloaded method DeleteActiveDriverAssignment(int userId, string licensePlate, string description)", true)]
      public int DeleteActiveDriverAssignment(string licensePlate)
		{
			int rowsAffected = 0;
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
			
				// 2. Retrieves driver assignment by license plate
				DataSet dsDriverAssignment = _driverAssignment.GetDriverAssignment(licensePlate);
				if(dsDriverAssignment == null || dsDriverAssignment.Tables.Count == 0 || dsDriverAssignment.Tables[0].Rows.Count == 0)
					throw new Exception("No drivers assigned to the license plate " + licensePlate);
				
				// 3. Unassign driver from license plate
				rowsAffected = _driverAssignment.DeleteDriverAssignment(licensePlate);

				// 4. Save old driver/license plate assignment to the history
				_driverAssignmentHst.UpdateDriverAssignment(dsDriverAssignment.Tables[0].Rows[0]["PersonId"].ToString().TrimEnd(),licensePlate);
				
				// 5. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				rowsAffected = 0;
				string prefixMsg = "Unable to delete driver assignment. ";
				// 5. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 6. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				rowsAffected = 0;
				string prefixMsg = "Unable to delete driver assignment. ";
				// 5. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
*/
        /*
        [Obsolete("Use DeleteActiveDriverAssignment(int, long, int, string,bool) instead")]
      public int DeleteActiveDriverAssignment(int userId, long vehicleId, int driverId, string description)
      {
          return DeleteActiveDriverAssignment(userId, vehicleId, driverId, description, true);
      }
      /// <summary>
      /// Delete existing driver assignment
      /// 
      /// Updates both assign. and history tables
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="driverId"></param>
      /// <param name="description"></param>
      /// <returns>rows affected</returns>
      /// <exception cref="DASException">Thrown DASException in exception cases</exception>
      public int DeleteActiveDriverAssignment(int userId, long vehicleId, int driverId, string description)
      {
          int rowsAffected = 0;
          string prefixMsg = "Unable to delete driver assignment.";
          try
          {

              // 1. Begin transaction
              //if( useTransaction)
              // sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
              _driverAssignment = new DriverAssignment(this.sqlExec);
              // 3. Unassign driver from license plate
              rowsAffected = _driverAssignment.DeleteDriverAssignment(userId, vehicleId, driverId, description);

              // 5. Save all changes
              // if(useTransaction)            
              //  sqlExec.CommitTransaction();
          }
          catch (SqlException objException)
          {
              //rowsAffected = 0;
              // 5. Rollback all changes
              // if (useTransaction)
              //sqlExec.RollbackTransaction();
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              // 6. Rollback all changes
              // if (useTransaction)
              //   sqlExec.RollbackTransaction();
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              //rowsAffected = 0;
              // 5. Rollback all changes
              // if (useTransaction)
              //  sqlExec.RollbackTransaction();
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return rowsAffected;
      }

<<<<<<< .mine
=======
            // 5. Save all changes
           // if(useTransaction)            
                sqlExec.CommitTransaction();
         }
         catch (SqlException objException)
         {
            rowsAffected = 0;
            // 5. Rollback all changes
           // if (useTransaction)
                sqlExec.RollbackTransaction();
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            // 6. Rollback all changes
            // if (useTransaction)
                 sqlExec.RollbackTransaction();
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            rowsAffected = 0;
            // 5. Rollback all changes
           // if (useTransaction)
                sqlExec.RollbackTransaction();
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return rowsAffected;
      }*/
      /// <summary>
      /// Delete existing driver assignment
      /// 
      /// Updates both assign. and history tables
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="driverId"></param>
      /// <param name="description"></param>
      /// <returns>rows affected</returns>
      /// <exception cref="DASException">Thrown DASException in exception cases</exception>
      public int DeleteActiveDriverAssignment(int userId, long vehicleId, int driverId, string description)
      {
          int rowsAffected = 0;
          string prefixMsg = "Unable to delete driver assignment.";
          try
          {

              // 1. Begin transaction
              //if( useTransaction)
              // sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
              _driverAssignment = new DriverAssignment(this.sqlExec);
              // 3. Unassign driver from license plate
              rowsAffected = _driverAssignment.DeleteDriverAssignment(userId, vehicleId, driverId, description);

              // 5. Save all changes
              // if(useTransaction)            
              //  sqlExec.CommitTransaction();
          }
          catch (SqlException objException)
          {
              //rowsAffected = 0;
              // 5. Rollback all changes
              // if (useTransaction)
              //sqlExec.RollbackTransaction();
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              // 6. Rollback all changes
              // if (useTransaction)
              //   sqlExec.RollbackTransaction();
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              //rowsAffected = 0;
              // 5. Rollback all changes
              // if (useTransaction)
              //  sqlExec.RollbackTransaction();
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return rowsAffected;
      }

		/// <summary>
		/// Delete all existing driver assignments.
		/// </summary>
      /// <param name="userId"></param> 
      /// <param name="driverId"></param> 
      /// <param name="description"></param> 
      /// <returns>void</returns>
		/// <exception cref="DASException">Thrown DASException in exception cases.</exception>
      public int DeleteActiveDriverAssignments(int userId, int driverId, string description)
		{
         int rowsAffected = 0;
         string prefixMsg = "Unable to delete driver assignments.";
         try
         {
            // 1. Begin transaction
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

            // 3. Unassign driver from license plate
            rowsAffected = _driverAssignment.DeleteDriverAssignments(userId, driverId, description);

            // 5. Save all changes
            sqlExec.CommitTransaction();
         }
         catch (SqlException objException)
         {
            rowsAffected = 0;
            // 5. Rollback all changes
            sqlExec.RollbackTransaction();
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            // 6. Rollback all changes
            sqlExec.RollbackTransaction();
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            rowsAffected = 0;
            // 5. Rollback all changes
            sqlExec.RollbackTransaction();
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return rowsAffected;
      }
		
		/// <summary>
		/// Get all active drivers assignments
		/// </summary>
		/// <remarks>
		/// TableName	= "AllActiveDriversAssignments"
		/// DataSetName = "Driver"
		/// </remarks>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		/// <returns>DataSet [AssignedDateTime],[LicensePlate],[PersonId],[Description]</returns>
		public DataSet GetAllActiveDriversAssignments()
		{
         DataSet dsResult = new DataSet();
			// 1. Retrieves all vehicle assignments
         try
         {
            dsResult = _driverAssignment.GetAllRecords();
            if (dsResult != null)
            {
               if (dsResult.Tables.Count > 0)
               {
                  dsResult.Tables[0].TableName = "AllActiveDriversAssignments";
               }
               dsResult.DataSetName = "Driver";
            }
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to delete driver assignments.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
			return dsResult;
		}

		/// <summary>
      /// Returns driver assignment by vehicle Id 
		/// </summary>
      /// <param name="vehicleId"></param>
		/// <returns>DataSet [AssignedDateTime],[PersonId],[DriverLicense],[FirstName],[LastName],[Description]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetDriverActiveAssignment(long vehicleId)
		{
         DataSet dsResult = new DataSet();
         try
         {
            dsResult = _driverAssignment.GetDriverAssignmentForVehicle(vehicleId);
            if (dsResult != null)
            {
               if (dsResult.Tables.Count > 0)
               {
                  dsResult.Tables[0].TableName = "DriverAssignmentInfo";
               }
               dsResult.DataSetName = "Driver";
            }
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to get driver assignment.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
			return dsResult;
		}			

		/// <summary>
      /// Returns all driver active assignments by driver Id. 
		/// </summary>
      /// <param name="driverId"></param>
		/// <returns>DataSet [AssignedDateTime],[PersonId],[LicensePlate],[DriverLicense],[FirstName],[LastName],[Description]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetDriverActiveAssignments(int driverId)
		{
         DataSet dsResult = new DataSet();
         try
         {
            dsResult = _driverAssignment.GetDriverAssignments(driverId);
            if (dsResult != null)
            {
               if (dsResult.Tables.Count > 0)
               {
                  dsResult.Tables[0].TableName = "DriverAssignmentsInfo";
               }
               dsResult.DataSetName = "Driver";
            }
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to get driver assignments.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return dsResult;
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
         string prefixMsg = "Unable to retrieve all unassigned organization drivers by organization id=" + organizationId.ToString() + ".";
         DataSet sqlDataSet = null;
         try
         {
            //Executes SQL statement
            sqlDataSet = _driverAssignment.GetUnassignedDriversForOrganization(organizationId);
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
      /// Retrieves all assigned drivers per organization
      /// </summary>
      /// <returns>
      /// </returns>
      /// <param name="organizationId"></param> 
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAssignedDriversForOrganization(int organizationId)
      {
         string prefixMsg = "Unable to retrieve all unassigned organization drivers by organization id=" + organizationId.ToString() + ".";
         DataSet sqlDataSet = null;
         try
         {
            //Executes SQL statement
            sqlDataSet = _driverAssignment.GetAssignedDriversForOrganization(organizationId);
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
      /// Retrieves all drivers by user fleet assigment 
      /// </summary>
      /// <returns>
      /// </returns>
      /// <param name="userId"></param> 
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetDriversListByUserFleetAssigment(int userId)
      {
          string prefixMsg = "Unable to retrieve drivers by By User Fleet Assigment. User id=" + userId.ToString() + ".";
          DataSet sqlDataSet = null;
          try
          {
              //Executes SQL statement
              sqlDataSet = _driver.GetDriversListByUserFleetAssigment(userId);
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
		#endregion

      # region Driver assignment history

      /// <summary>
      /// Add driver Assignment History (asgn. + unassgn.)
      /// </summary>
      /// <param name="driverId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="userId"></param>
      /// <param name="description"></param>
      /// <param name="startDate"></param>
      /// <param name="endDate"></param>
      /// <returns>Rows affected</returns>
      public int AddAssignmentHistory(int driverId, long vehicleId, int userId, string description, DateTime startDate, DateTime endDate)
      {
         return _driverAssignmentHst.AddDriverAssignmentHistory(driverId, vehicleId, userId, description, startDate, endDate);
      }

      /// <summary>
      /// Get Driver Assignment History
      /// </summary>
      /// <param name="driverId"></param>
      /// <returns></returns>
      public DataSet GetDriverAssignmentHistory(int driverId)
      {
         DataSet dsResult = new DataSet("Driver");
         try
         {
            dsResult = _driverAssignmentHst.GetAssignmentHistoryByDriverId(driverId);
            if (dsResult != null)
            {
               if (dsResult.Tables.Count > 0)
               {
                  dsResult.Tables[0].TableName = "DriverAssignmentHst";
               }
            }
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to get driver assignment history.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return dsResult;

      }

      /// <summary>
      /// Get Driver Assignment History by dates
      /// </summary>
      /// <param name="driverId"></param>
      /// <returns></returns>
      public DataSet GetDriverAssignmentHistory(int driverId, DateTime from, DateTime to)
      {
         DataSet dsResult = new DataSet("Driver");
         try
         {
            dsResult = _driverAssignmentHst.GetAssignmentHistoryByDriverId(driverId, from, to);
            if (dsResult != null)
            {
               if (dsResult.Tables.Count > 0)
               {
                  dsResult.Tables[0].TableName = "DriverAssignmentHst";
               }
            }
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to get driver assignment history.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return dsResult;

      }

      /// <summary>
      /// Get Driver Assignment History
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <returns></returns>
      public DataSet GetVehicleAssignmentHistory(long vehicleId)
      {
         DataSet dsResult = new DataSet("Driver");
         try
         {
            dsResult = _driverAssignmentHst.GetAssignmentHistoryByVehicleId(vehicleId);
            if (dsResult != null)
            {
               if (dsResult.Tables.Count > 0)
               {
                  dsResult.Tables[0].TableName = "DriverAssignmentHst";
               }
            }
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to get vehicle assignment history.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return dsResult;
      }

      /// <summary>
      /// Get Driver Assignment History by dates
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <returns></returns>
      public DataSet GetVehicleAssignmentHistory(long vehicleId, DateTime from, DateTime to)
      {
         DataSet dsResult = new DataSet("Driver");
         try
         {
            dsResult = _driverAssignmentHst.GetAssignmentHistoryByVehicleId(vehicleId, from, to);
            if (dsResult != null)
            {
               if (dsResult.Tables.Count > 0)
               {
                  dsResult.Tables[0].TableName = "DriverAssignmentHst";
               }
            }
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to get vehicle assignment history.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return dsResult;
      }

      /// <summary>
      /// Get Driver Assignment History
      /// </summary>
      /// <param name="driverId"></param>
      /// <param name="vehicleId"></param>
      /// <returns></returns>
      public DataSet GetDriverVehicleAssignmentHistory(int driverId, long vehicleId)
      {
         DataSet dsResult = new DataSet("Driver");
         try
         {
            dsResult = _driverAssignmentHst.GetAssignmentHistoryByDriverIdVehicleId(driverId, vehicleId);
            if (dsResult != null)
            {
               if (dsResult.Tables.Count > 0)
               {
                  dsResult.Tables[0].TableName = "DriverAssignmentHst";
               }
            }
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to get driver - vehicle assignment history.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return dsResult;
      }

      /// <summary>
      /// Get Driver Assignment History by dates
      /// </summary>
      /// <param name="driverId"></param>
      /// <param name="vehicleId"></param>
      /// <returns></returns>
      public DataSet GetDriverVehicleAssignmentHistory(int driverId, long vehicleId, DateTime from, DateTime to)
      {
         DataSet dsResult = new DataSet("Driver");
         try
         {
            dsResult = _driverAssignmentHst.GetAssignmentHistoryByDriverIdVehicleId(driverId, vehicleId, from, to);
            if (dsResult != null)
            {
               if (dsResult.Tables.Count > 0)
               {
                  dsResult.Tables[0].TableName = "DriverAssignmentHst";
               }
            }
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to get driver - vehicle assignment history.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return dsResult;
      }

      /// <summary>
      /// Get all Assigned Vehicles List
      /// </summary>
      /// <param name="driverId"></param>
      /// <param name="from"></param>
      /// <param name="to"></param>
      /// <returns></returns>
      public DataSet GetAssignmentVehiclesByDriverId(int driverId, DateTime from, DateTime to)
      {
         return this._driverAssignmentHst.GetAssignmentVehiclesByDriverId(driverId, from, to);
      }

      # endregion

      # region Utilities

      /// <summary>
      /// Field validation
      /// </summary>
      /// <param name="fieldName"></param>
      /// <param name="field"></param>
      /// <param name="minLength"></param>
      /// <param name="maxLength"></param>
      private void ValidateField(string fieldName, string field, int minLength, int maxLength)
      {
         if (field.Length < minLength || field.Length > maxLength)
            throw new ArgumentException(String.Format("{0} error: [{1}]", fieldName, field));
      }

      /// <summary>
      /// Field validation
      /// </summary>
      /// <param name="fieldName"></param>
      /// <param name="field"></param>
      /// <param name="minValue"></param>
      /// <param name="maxValue"></param>
      private void ValidateField(string fieldName, int field, int minValue, int maxValue)
      {
         if (field < minValue || field > maxValue)
            throw new ArgumentException(String.Format("{0} error: [{1}]", fieldName, field));
      }

      # endregion

      #region HOS

        

     /// <summary>
        /// Retrieves list of HoursOfService based on DateTime
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetHOSSummarybyDateTime(int driverId, DateTime from, DateTime to)
        {
            DataSet dsResult = new DataSet();
            try
            {
                HoursOfService hos = new HoursOfService(this.sqlExec);
                SortedList<DateTime,int> refids = hos.GetHOSRefIDs(driverId, from, to);
                        TimeSpan ts;
                        TimeSpan[] tss = new TimeSpan[4];
                        tss[0] = new TimeSpan(0); tss[1] = new TimeSpan(0); tss[2] = new TimeSpan(0); tss[3] = new TimeSpan(0);
                        int lastev = -1; DateTime lastdt = from;
                        int ev = 0; DateTime dt = from;
                        string location; int stodo = -1, endodo = -1; string logstop;
                        int lstodo = -1; int distance = 0;
                        for(int row=0;row<refids.Count;row++)
                        {
                            DateTime refdate = refids.Keys[row];
                            int refid = refids[refdate];                            
                            lastdt = refdate;
                            from = refdate;
                            TimeSpan[] tssTemp = new TimeSpan[4];
                            tssTemp[0] = new TimeSpan(0); tssTemp[1] = new TimeSpan(0); tssTemp[2] = new TimeSpan(0); tssTemp[3] = new TimeSpan(0);
                            SqlDataReader sdr = hos.GetHOSSummarybyDateTime(refid, refdate);
                            if (sdr != null)
                            {
                                if (sdr.HasRows)
                                {
                                    while (sdr.Read())
                                    {
                                        ev = sdr.GetInt32(0);
                                        dt = sdr.GetDateTime(1);
                                        location = sdr.GetString(2);
                                        endodo = sdr.GetInt32(3);
                                        logstop = sdr.GetString(4) + ",";
                                        int type = sdr.GetInt16(5);
                                        if (type != 1) //1==auto mode
                                        {
                                            if (stodo < 0) stodo = endodo;
                                        }
                                        if (ev == 2) //driving
                                        {
                                            lstodo = endodo;
                                        }
                                        if (ev != 2)
                                        {
                                            if (lstodo > 0)
                                            {
                                                distance += endodo - lstodo;
                                                lstodo = -1;
                                            }
                                        }
                                        //ts = dt - from;
                                        if (lastev != -1)
                                        {
                                            tssTemp[lastev] += dt - lastdt;
                                        }
                                        lastev = ev;
                                        lastdt = dt;
                                    }
                                    tssTemp[ev] += from.AddDays(1) - dt;
                                    //lastdt = from.AddDays(1);
                                }
                                sdr.Close();
                                for (int l = 0; l < 4; l++)
                                {
                                    tss[l] += tssTemp[l];
                                }
                            }
                        }
                        
                        DataTable SummaryTable = dsResult.Tables.Add("HOSSummaryInfo");
                        SummaryTable.Columns.Add("Status", typeof(string));
                        SummaryTable.Columns.Add("Hours", typeof(double));

                        DataRow dRow = dsResult.Tables["HOSSummaryInfo"].NewRow();
                        dRow[0] = "Off Duty";
                        dRow[1] = tss[0].TotalHours;
                        dsResult.Tables["HOSSummaryInfo"].Rows.Add(dRow);
                        dRow = dsResult.Tables["HOSSummaryInfo"].NewRow();
                        dRow[0] = "Sleeping";
                        dRow[1] = tss[1].TotalHours;
                        dsResult.Tables["HOSSummaryInfo"].Rows.Add(dRow);
                        dRow = dsResult.Tables["HOSSummaryInfo"].NewRow();
                        dRow[0] = "Driving";
                        dRow[1] = tss[2].TotalHours;
                        dsResult.Tables["HOSSummaryInfo"].Rows.Add(dRow);
                        dRow = dsResult.Tables["HOSSummaryInfo"].NewRow();
                        dRow[0] = "On Duty";
                        dRow[1] = tss[3].TotalHours;
                        dsResult.Tables["HOSSummaryInfo"].Rows.Add(dRow);                 
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get HOS Info.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return dsResult;
        }	

         /// <summary>
        /// Add new HoursOfService process record
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="timestamp"></param>       
        /// <param name="state"></param>
        /// <param name="cycle"></param>
        /// <param name="IsSecondDay"></param>
        public int AddHoursOfService(int driverId, DateTime timestamp, Int16 state, Int16 cycle, bool IsSecondDay, bool IsCalculated, bool IsSignedOff, int userId, string description,Int16 ruleId)
        {
            int serviceId = 0;
            try
            {
                HoursOfService hos = new HoursOfService(this.sqlExec);

                serviceId =
                hos.AddHoursOfService(
                   driverId,
                   timestamp,
                   state,
                   cycle,
                  IsSecondDay, IsCalculated, IsSignedOff, userId, description,ruleId );

             }
             catch (SqlException objException)
             {
                Util.ProcessDbException("Failed to add a HOS", objException);
             }
             catch (DASDbConnectionClosed exCnn)
             {
                throw new DASDbConnectionClosed(exCnn.Message);
             }
             catch (Exception objException)
             {
                throw new DASException(objException.Message);
             }

             return serviceId;
        }


        /// <summary>
        /// Add new HoursOfService process record
        /// </summary>
        /// <param name="hoursOfServiceId"></param>
        /// <param name="changedByServiceId"></param>       
        public void DisableHOS(int hoursOfServiceId, int changedByServiceId,int userId)
        {
           
            try
            {
                HoursOfService hos = new HoursOfService(this.sqlExec);
                hos.DisableHOS(hoursOfServiceId,changedByServiceId,userId);

            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Failed to disable a HOS", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(objException.Message);
            }


        }
        // Changes for TimeZone Feature start

        /// <summary>
        /// Retrieves list of HoursOfService based on DateTime
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetHOSbyDateTime_NewTZ(int driverId, int userId, DateTime from, DateTime to)
        {
            DataSet dsResult = new DataSet();
            try
            {
                HoursOfService hos = new HoursOfService(this.sqlExec);
                dsResult = hos.GetHOSbyDateTime_NewTZ(driverId, userId, from, to);
                if (dsResult != null)
                {
                    if (dsResult.Tables.Count > 0)
                    {
                        dsResult.Tables[0].TableName = "HOSInfo";
                    }
                    dsResult.DataSetName = "HOSInfo";
                }
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get HOS Info.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return dsResult;
        }			


        // Changes for TimeZone Feature end



        /// <summary>
        /// Retrieves list of HoursOfService based on DateTime
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetHOSbyDateTime(int driverId,int userId, DateTime from, DateTime to)
        {
            DataSet dsResult = new DataSet();
            try
            {
                HoursOfService hos = new HoursOfService(this.sqlExec);
                dsResult=hos.GetHOSbyDateTime(driverId,userId,  from, to);
                if (dsResult != null)
                {
                    if (dsResult.Tables.Count > 0)
                    {
                        dsResult.Tables[0].TableName = "HOSInfo";
                    }
                    dsResult.DataSetName = "HOSInfo";
                }
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get HOS Info.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return dsResult;
        }			



          /// <summary>
        /// Get Hours Of Service State Types
        /// </summary>
        /// <returns></returns>
        public DataSet GetHoursOfServiceStateTypes()
        {

            DataSet dsResult = new DataSet();
            try
            {
                HoursOfService hos = new HoursOfService(this.sqlExec);
                dsResult=hos.GetHoursOfServiceStateTypes ();
                if (dsResult != null)
                {
                    if (dsResult.Tables.Count > 0)
                    {
                        dsResult.Tables[0].TableName = "HoursOfServiceStateTypes";
                    }
                    dsResult.DataSetName = "HoursOfServiceStateTypes";
                }
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get HoursOfServiceStateTypes Info.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return dsResult;

        }


        /// <summary>
        /// Get Hours Of Service Duty Cycles
        /// </summary>
        /// <returns></returns>
        public DataSet GetHoursOfServiceRules()
        {


            DataSet dsResult = new DataSet();
            try
            {
                HoursOfService hos = new HoursOfService(this.sqlExec);
                dsResult = hos.GetHoursOfServiceRules();
                if (dsResult != null)
                {
                    if (dsResult.Tables.Count > 0)
                    {
                        dsResult.Tables[0].TableName = "HoursOfServiceRules";
                    }
                    dsResult.DataSetName = "HoursOfServiceRules";
                }
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get HoursOfServiceRules Info.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return dsResult;

        }



        /// <summary>
        /// Validate HOS entry
        /// </summary>

        public void ValidateHOSentry(int driverId, DateTime timestamp, int stateTypeId, ref int existingHoursOfServiceId, ref Int16 flagOverride)
        {
        
            string errMsg = "Failed to validate entry ";
            try
            {
                HoursOfService hos = new HoursOfService(this.sqlExec);
                hos.ValidateHOSentry(driverId, timestamp, stateTypeId, ref  existingHoursOfServiceId, ref  flagOverride); 

            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(errMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(objException.Message);
            }
        }



        /// <summary>
        /// Retrieves list of HoursOfService based on DateTime
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetHOSnotificationsByDateTime(int driverId, int userId ,DateTime from, DateTime to)
        {
            DataSet dsResult = new DataSet();
            try
            {
                HoursOfServiceNotification hos = new HoursOfServiceNotification(this.sqlExec);
                dsResult = hos.GetHOSnotificationsByDateTime(driverId,userId, from, to);
                if (dsResult != null)
                {
                    if (dsResult.Tables.Count > 0)
                    {
                        dsResult.Tables[0].TableName = "HOSNotification";
                    }
                    dsResult.DataSetName = "HOSNotificationo";
                }
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get HOS Notification.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return dsResult;
        }			
    
      #endregion


      /// <summary>
      /// Add multiple assignments
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="dataTable">Table containing assignments info</param>
      public void AssignDrivers(int userId, DataTable dataTable)
      {
         int rowsAffected = 0;
         string errMsg = "Failed to add assignments' list ";
         try
         {
            VehicleInfo veh = new VehicleInfo(this.sqlExec);
            User user = new User(this.ConnectionString);
            long vehicleId = 0;
            foreach (DataRow row in dataTable.Rows)
            {
               vehicleId = 
                  veh.GetVehicleIdByDescription(row["Description"].ToString(), user.GetOrganizationIdByUserId(userId));
               // vehicle id not found - vehicle does not exist
               if (vehicleId <= 0) continue;

               rowsAffected +=
                  this._driverAssignment.ForceDriverAssignment(
                     userId,
                     vehicleId,
                     Convert.ToInt32(row["ID"]),
                     Convert.ToDateTime(row["AssignedDate"]),
                     dataTable.Columns.Contains("Comments") ? row["Comments"].ToString() : "");
            }

            if (rowsAffected == 0)
               throw new DASDbException(errMsg);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(errMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(objException.Message);
         }
      }


        public void AssignDriversInBulk(int userId, string xml)
        {
            string errMsg = "Failed to add assignments' list ";
            string errCode = "";
            try
            {
                errCode=this._driverAssignment.AssignDriversInBulk(userId, xml);

                if (errCode!="")
                    throw new DASDbException(errMsg);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(errMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(objException.Message);
            }
        }
      /// <summary>
      ///      
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="SMSID"></param>
      /// <param name="SMSPwd"></param>
      /// <returns></returns>
      public int AuthenticateDriver(int boxId, string SMSID, string SMSPwd)
      {
         return _driver.AuthenticateDriverSignIn(boxId, SMSID, SMSPwd);
     }

           /// <summary>
          /// Retrieves driver info
          /// </summary>
          /// <returns>
          /// </returns>
          /// <param name="driverId"></param> 
          /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
          /// <exception cref="DASException">Thrown in all other exception cases.</exception>
          
        public int GetDriverIdByBoxId(int boxId)
        {
            return _driverAssignment.GetDriverIdByBoxId(boxId);
        }

     #region Driver Text Messages
     /// <summary>
     /// Add new driver text msgs (email) 
     /// </summary>
     /// <param name="driverId"></param>
     /// <param name="userId"></param>
     /// <param name="email"></param>
     /// <param name="msgBody"></param>
     /// <param name="msgDateTime"></param>
     /// <returns></returns>
        public int AddDriverMsg(int driverId, int userId, string email, string msgBody, DateTime msgDateTime)
        {
            return _driver.AddDriverMsg(driverId, userId, email, msgBody, msgDateTime); 
        }
        /// <summary>
       /// Get Driver Messages
       /// </summary>
       /// <param name="driverId"></param>
       /// <param name="fromDate"></param>
       /// <param name="toDate"></param>
       /// <returns></returns>
        public DataSet GetDriverMsgs(int driverId, DateTime fromDate, DateTime toDate)
        {
            return _driver.GetDriverMsgs(driverId, fromDate, toDate); 
        }

        /// <summary>
        /// Retrive Driver by BoxId
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
      
     #endregion


        /// <summary>
        /// Add new driver assignment.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="driverId"></param>
        /// <param name="description"></param>
        /// <returns>void</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if driver assignment with license plate alredy exists</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public void AddHOSDriverAssignment(int userId, long vehicleId, int driverId, string description)
        {
            int rowsAffected = 0;
            try
            {
                rowsAffected = _driverAssignment.AddHOSDriverAssignment(userId, vehicleId, driverId, description);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(String.Format("Unable to add a new driver assignment with driver Id={0} Vehicle Id={1}.",
                   driverId, vehicleId), objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(String.Format("Unable to add a new driver assignment with driver Id={0} Vehicle Id={1}. Error: {2}",
                   driverId, vehicleId, objException.Message));
            }
            if (rowsAffected == 0)
            {
                throw new DASAppDataAlreadyExistsException(
                   String.Format("Unable to add a new driver assignment with driver Id={0} Vehicle Id={1}. This driver is already assigned.",
                   driverId, vehicleId));
            }
        }

        /// <summary>
        /// Add new driver assignment.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="driverId"></param>
        /// <param name="assgnDate"></param>
        /// <param name="description"></param>
        /// <returns>void</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if driver assignment with license plate alredy exists</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public void AddHOSDriverAssignment(int userId, long vehicleId, int driverId, DateTime assgnDate, string description)
        {
            int rowsAffected = 0;
            try
            {
                _driverAssignment = new DriverAssignment(this.sqlExec);
                rowsAffected = _driverAssignment.AddHOSDriverAssignment(userId, vehicleId, driverId, assgnDate, description);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(String.Format("Unable to add a new driver assignment with driverId={0} Vehicle Id={1}.",
                   driverId, vehicleId), objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(String.Format("Unable to add a new driver assignment with driverId={0} Vehicle Id={1}. Error: {2}",
                   driverId, vehicleId, objException.Message));
            }
            if (rowsAffected == 0)
            {
                throw new DASAppDataAlreadyExistsException(
                   String.Format("Unable to add a new driver assignment with driverId={0} Vehicle Id={1}. This driver is already assigned.",
                   driverId, vehicleId));
            }
        }

        /// <summary>
        /// Delete existing driver assignment
        /// 
        /// Updates both assign. and history tables
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="driverId"></param>
        /// <param name="description"></param>
        /// <returns>rows affected</returns>
        /// <exception cref="DASException">Thrown DASException in exception cases</exception>
        public int DeleteHOSActiveDriverAssignment(int userId, long vehicleId, int driverId, string description)
        {
            int rowsAffected = 0;
            string prefixMsg = "Unable to delete driver assignment.";
            try
            {

                // 1. Begin transaction
                //if( useTransaction)
                // sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                _driverAssignment = new DriverAssignment(this.sqlExec);
                // 3. Unassign driver from license plate
                rowsAffected = _driverAssignment.DeleteHOSDriverAssignment(userId, vehicleId, driverId, description);

                // 5. Save all changes
                // if(useTransaction)            
                //  sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                //rowsAffected = 0;
                // 5. Rollback all changes
                // if (useTransaction)
                //sqlExec.RollbackTransaction();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                // 6. Rollback all changes
                // if (useTransaction)
                //   sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                //rowsAffected = 0;
                // 5. Rollback all changes
                // if (useTransaction)
                //  sqlExec.RollbackTransaction();
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }



        /// <summary>
        /// Create, update and delete driver skill
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="organizationId"></param>
        /// <param name="skillId"></param>
        /// <param name="description"></param>
        /// <param name="delete"></param>
        /// <returns>bool</returns>
        public string SaveDriverSkill(int driverId, int organizationId, int skillId, int oldSkillId, string description, int delete = 0)
        {
            try
            {
                DataSet dataResult = _driverAssignment.AssignDriverSkill(driverId, organizationId, skillId, oldSkillId, description, delete);
                string mySkillId = Convert.ToString(dataResult.Tables[0].Rows[0]["SkillId"]);
                string myDriverId = Convert.ToString(dataResult.Tables[0].Rows[0]["DriverId"]);
                if (mySkillId != null && myDriverId != null)
                {
                    return myDriverId + '_' + mySkillId;
                }
            }
            catch (Exception exception)
            {
                if (delete == 1)
                {
                    return Convert.ToString(driverId) + '_' + Convert.ToString(skillId);
                }
            }
            return null;
        }



        /// <summary>
        /// Get the list of driver skills for organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataSet GetDriverSkills(int organizationId)
        {
            return _driverAssignment.GetDriverSkills(organizationId);
        }

        public string GetTripDriver(string drID, int vehicleID, DateTime fromDate, DateTime toDate)
        {
            return _driver.GetTripDriver(drID, vehicleID, fromDate, toDate);  
        }


 }
}
