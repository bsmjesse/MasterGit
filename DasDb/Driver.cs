using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;


namespace VLF.DAS.DB
{
   /// <summary>
   /// Provides interfaces to vlfPersonInfo table.
   /// </summary>
   public class Driver : TblGenInterfaces
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public Driver(SQLExecuter sqlExec) : base("vlfDriver", sqlExec)
      {

      }

      /// <summary>
      /// Add new driver information
      /// </summary>
      /// <param name="info"></param>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if driver license or person alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
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
      /// <param name="description"></param>
      /// <returns>Rows affected</returns>
      public int AddDriver(string firstName, string lastName, string license, string classLicense, DateTime licenseIssued,
         DateTime licenseExpired, int orgId, char gender, short height, string homePhone, string cellPhone, string additionalPhone,
         string smsPwd,
         string smsid, string email, string address, string city, string zipcode, string state, string country, 
         string description)
      {
         // 1. Prepares SQL statement
         string sql = "DriverAdd";

         sqlExec.ClearCommandParameters();

         sqlExec.AddCommandParam("@firstName", SqlDbType.VarChar, firstName, 50);
         sqlExec.AddCommandParam("@lastName", SqlDbType.VarChar, lastName, 50);
         sqlExec.AddCommandParam("@license", SqlDbType.VarChar, license, 50);
         sqlExec.AddCommandParam("@class", SqlDbType.VarChar, classLicense, 20);
         sqlExec.AddCommandParam("@licenseIssued", SqlDbType.DateTime, licenseIssued);
         sqlExec.AddCommandParam("@licenseExpired", SqlDbType.DateTime, licenseExpired);
         sqlExec.AddCommandParam("@orgId", SqlDbType.Int, orgId);
         sqlExec.AddCommandParam("@gender", SqlDbType.Char, gender.ToString(), 1);
         sqlExec.AddCommandParam("@height", SqlDbType.SmallInt, height);
         sqlExec.AddCommandParam("@homePhone", SqlDbType.VarChar, homePhone, 20);
         sqlExec.AddCommandParam("@cellPhone", SqlDbType.VarChar, cellPhone, 20);
         sqlExec.AddCommandParam("@additionalPhone", SqlDbType.VarChar, additionalPhone, 20);
         sqlExec.AddCommandParam("@smsPwd", SqlDbType.VarChar, smsPwd, 50);
         sqlExec.AddCommandParam("@smsid", SqlDbType.VarChar, smsid, 50);
         sqlExec.AddCommandParam("@email", SqlDbType.VarChar, email, 250);
         sqlExec.AddCommandParam("@address", SqlDbType.VarChar, address, 100);
         sqlExec.AddCommandParam("@city", SqlDbType.VarChar, city, 50);
         sqlExec.AddCommandParam("@zipcode", SqlDbType.VarChar, zipcode, 20);
         sqlExec.AddCommandParam("@state", SqlDbType.VarChar, state, 50);
         sqlExec.AddCommandParam("@country", SqlDbType.VarChar, country, 50);
         sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, description, 100);

         if (sqlExec.RequiredTransaction())
         {
            // 4. Attach current command SQL to transaction
            sqlExec.AttachToTransaction(sql);
         }
         // 5. Executes SQL statement
         return sqlExec.SPExecuteNonQuery(sql);
      }

      /// <summary>
      /// Update driver information.
      /// </summary>
      /// <param name="info"></param>
      /// <returns>rows affected</returns>
      /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if person does not exist.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
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
      /// <param name="description"></param>
      /// <returns>Rows affected</returns>
      public int UpdateDriver(int driverId, string firstName, string lastName, string license, string classLicense, DateTime licenseIssued,
         DateTime licenseExpired, int orgId, char gender, short height, string homePhone, string cellPhone, string additionalPhone,
         string smsPwd, 
         string smsid, string email, string address, string city, string zipcode, string state, string country, string description)
      {
         int rowsAffected = 0;

         // 1. Prepares SQL statement
         string sql = "DriverUpdate";

         sqlExec.ClearCommandParameters();

         sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);
         sqlExec.AddCommandParam("@firstName", SqlDbType.VarChar, firstName, 50);
         sqlExec.AddCommandParam("@lastName", SqlDbType.VarChar, lastName, 50);
         sqlExec.AddCommandParam("@license", SqlDbType.VarChar, license, 50);
         sqlExec.AddCommandParam("@class", SqlDbType.VarChar, classLicense, 20);
         sqlExec.AddCommandParam("@licenseIssued", SqlDbType.DateTime, licenseIssued);
         sqlExec.AddCommandParam("@licenseExpired", SqlDbType.DateTime, licenseExpired);
         sqlExec.AddCommandParam("@orgId", SqlDbType.Int, orgId);
         sqlExec.AddCommandParam("@gender", SqlDbType.Char, gender.ToString(), 1);
         sqlExec.AddCommandParam("@height", SqlDbType.SmallInt, height);
         sqlExec.AddCommandParam("@homePhone", SqlDbType.VarChar, homePhone, 20);
         sqlExec.AddCommandParam("@cellPhone", SqlDbType.VarChar, cellPhone, 20);
         sqlExec.AddCommandParam("@additionalPhone", SqlDbType.VarChar, additionalPhone, 20);
         sqlExec.AddCommandParam("@smsPwd", SqlDbType.VarChar, smsPwd, 50);
         sqlExec.AddCommandParam("@smsid", SqlDbType.VarChar, smsid, 50);
         sqlExec.AddCommandParam("@email", SqlDbType.VarChar, email, 250);
         sqlExec.AddCommandParam("@address", SqlDbType.VarChar, address, 100);
         sqlExec.AddCommandParam("@city", SqlDbType.VarChar, city, 50);
         sqlExec.AddCommandParam("@zipcode", SqlDbType.VarChar, zipcode, 20);
         sqlExec.AddCommandParam("@state", SqlDbType.VarChar, state, 50);
         sqlExec.AddCommandParam("@country", SqlDbType.VarChar, country, 50);
         sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, description, 100);

         //Executes SQL statement
         rowsAffected = sqlExec.SPExecuteNonQuery(sql);

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
      public DataSet GetDriverById(int driverId)
      {
         return GetRowsByIntField("DriverId", driverId, "Driver Id");
      }

      /// <summary>
      /// Retrieves driver info
      /// </summary>
      /// <returns>
      /// </returns>
      /// <param name="organizationId"></param> 
      /// <param name="driverLicense"></param> 
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetDriverByDriverLicense(int organizationId, string driverLicense)
      {
         DataSet sqlDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = String.Format("SELECT * FROM {0} WHERE License='{1}' AND OrganizationId={2}",
               this.tableName, driverLicense, organizationId);

            //Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve driver by license=" + driverLicense + ".";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve driver by license=" + driverLicense + ".";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return sqlDataSet;
      }

      /// <summary>
      /// Retrieves all drivers per organization
      /// </summary>
      /// <returns>
      /// </returns>
      /// <param name="organizationId"></param> 
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllDriversForOrganization(int organizationId)
      {
         DataSet sqlDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "DriversGetPerOrganization";

            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@orgId", SqlDbType.Int, organizationId);

            //Executes SQL statement
            sqlDataSet = sqlExec.SPExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve drivers by organization id=" + organizationId.ToString() + ".";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve drivers by organization id=" + organizationId.ToString() + ".";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return sqlDataSet;
      }


      /// <summary>
      /// Retrieves all drivers per organization
      /// </summary>
      /// <returns>
      /// </returns>
      /// <param name="organizationId"></param> 
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllDriversForOrganizationByUser(int userId, string keyFobId)
      {
          DataSet sqlDataSet = null;
          try
          {
              //Prepares SQL statement
              string sql = "GetAllDriversPerOrganization";

              sqlExec.ClearCommandParameters();
              sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
              sqlExec.AddCommandParam("@keyFobId", SqlDbType.VarChar, keyFobId);

              //Executes SQL statement
              sqlDataSet = sqlExec.SPExecuteDataset(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve drivers by user id=" + userId.ToString() + ".";
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve drivers by user id=" + userId.ToString() + ".";
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return sqlDataSet;
      }

      /// <summary>
      /// Delete existing driver including assignments and history
      /// </summary>
      /// <returns>rows affected</returns>
      /// <param name="driverId"></param> 
      /// <exception cref="DASAppResultNotFoundException">Thrown if person with person id not exist</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteDriver(int driverId)
      {
         int rowsAffected = 0;

         //Prepares SQL statement
         string sql = "DriverDelete";

         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);

         //Executes SQL statement
         rowsAffected = sqlExec.SPExecuteNonQuery(sql);
         return rowsAffected;
      }

      /// <summary>
      /// Checks if person with the given driver license already exists.
      /// </summary>
      /// <returns>bool</returns>
      /// <param name="driverLicense"></param> 
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public bool IsDriverLicenseExist(string driverLicense)
      {
         int retResult = 0;
         try
         {
            //Prepares SQL statement
            string sql = String.Format("SELECT COUNT(*) FROM {0} WHERE DriverLicense='{1}'", tableName, driverLicense);

            //Executes SQL statement
            retResult = (int)sqlExec.SQLExecuteScalar(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve info by driver license=" + driverLicense + ".";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve info by driver license=" + driverLicense + ".";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return (retResult > 0);
      }

      /// <summary>
      /// Add Driver 
      /// </summary>
      /// <param name="row">Table with drivers info</param>
      public int AddDriver(DataRow row)
      {
         // 1. Prepares SQL statement
         string sql = "DriverAdd";
         sqlExec.ClearCommandParameters();

         sqlExec.AddCommandParam("@firstName", SqlDbType.VarChar, row["FirstName"]);
         sqlExec.AddCommandParam("@lastName", SqlDbType.VarChar, row["LastName"]);
         sqlExec.AddCommandParam("@license", SqlDbType.VarChar, row["License"]);
         sqlExec.AddCommandParam("@class", SqlDbType.VarChar, row["Class"]);
         sqlExec.AddCommandParam("@licenseIssued", SqlDbType.DateTime, row["LicenseIssued"]);
         sqlExec.AddCommandParam("@licenseExpired", SqlDbType.DateTime, row["LicenseExpired"]);
         sqlExec.AddCommandParam("@orgId", SqlDbType.Int, row["OrganizationId"]);
         sqlExec.AddCommandParam("@gender", SqlDbType.Char, row["Gender"]);
         sqlExec.AddCommandParam("@height", SqlDbType.SmallInt, row["Height"]);
         sqlExec.AddCommandParam("@homePhone", SqlDbType.VarChar, row["HomePhone"]);
         sqlExec.AddCommandParam("@cellPhone", SqlDbType.VarChar, row["CellPhone"]);
         sqlExec.AddCommandParam("@additionalPhone", SqlDbType.VarChar, row["AdditionalPhone"]);
         sqlExec.AddCommandParam("@smsPWD", SqlDbType.VarChar, row["SMSPwd"]);
         sqlExec.AddCommandParam("@smsid", SqlDbType.VarChar, row["SMSID"]);
         sqlExec.AddCommandParam("@email", SqlDbType.VarChar, row["Email"]);
         sqlExec.AddCommandParam("@address", SqlDbType.VarChar, row["Address"]);
         sqlExec.AddCommandParam("@city", SqlDbType.VarChar, row["City"]);
         sqlExec.AddCommandParam("@zipcode", SqlDbType.VarChar, row["ZipCode"]);
         sqlExec.AddCommandParam("@state", SqlDbType.VarChar, row["State"]);
         sqlExec.AddCommandParam("@country", SqlDbType.VarChar, row["Country"]);
         sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, row["Description"]);

         if (sqlExec.RequiredTransaction())
         {
            // 4. Attach current command SQL to transaction
            sqlExec.AttachToTransaction(sql);
         }
         // 5. Executes SQL statement
         return sqlExec.SPExecuteNonQuery(sql);
      }

      /// <summary>
      /// Add multiple Drivers
      /// </summary>
      /// <param name="table"></param>
      public void AddDrivers(DataTable table)
      {
         this.sqlExec.InsertData(table, "vlfDriver", SqlBulkCopyOptions.Default, null); 
      }

      /// <summary>
      /// Retrieves all drivers per organization
      /// </summary>
      /// <returns>
      /// </returns>
      /// <param name="userId"></param> 
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
       public DataSet GetDriversListByUserFleetAssigment(int userId)
      {
          DataSet sqlDataSet = null;
          try
          {
              //Prepares SQL statement
              string sql = "sp_GetDriversListByUserFleetAssigment";

              sqlExec.ClearCommandParameters();
              sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);

              //Executes SQL statement
              sqlDataSet = sqlExec.SPExecuteDataset(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve drivers by By User Fleet Assigment. User id=" + userId.ToString() + ".";
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve drivers by ByUserFleetAssigment user id=" + userId.ToString() + ".";
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return sqlDataSet;
      }

      /// <summary>
      ///      this function returns if the SMSID and SMSPwd uniquely define a driverId (from vlfDriver)
      ///      and the vehicle currently associated with boxid (VehicleId), from vlfVehicleAssignment
      ///      are forming the current record in vlfDriverAssignment (VehicleId, driverId)
      /// 
      ///      if SMSID/SMSPwd are not the same, it returns -1
      ///      if SMSID/SMSPwd are the same and the record in vlfDriverAssignment is verified , it returns 0
      ///      if SMSID/SMSPwd the same and the record in vlfDriverAssignment is not verified, it returns 1
      ///         and - create an entry in vlfDriverAssignmentHst with the current entry in vlfDriverAssignment, filling UnaasignedDateTime
      ///             - update the entry in vlfDriverAssignment with PersonId = -1, THUS SIGNALLING the MDT/peripheral assignment
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="SMSID"> coming from MDT/peripheral message </param>
      /// <param name="SMSPwd"> coming from MDT/peripheral message </param>
      /// <returns>
      ///      in case of an SQL exception, returns -2
      ///      if SMSID/SMSPwd are not the same, it returns -1
      ///      if SMSID/SMSPwd are the same and the record in vlfDriverAssignment is verified , it returns 0
      ///      if SMSID/SMSPwd the same and the record in vlfDriverAssignment is not verified, it returns 1       
      /// </returns>
      public int AuthenticateDriverSignIn (int boxId, string SMSID, string SMSPwd)
      {
         int ret = -2 ;    // SQL exception
         try
         {
            //Prepares SQL statement
            string sql = "AuthenticateDriverSignIn";            

            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@boxId", SqlDbType.Int, boxId);
            sqlExec.AddCommandParam("@SMSId", SqlDbType.VarChar, SMSID);
            sqlExec.AddCommandParam("@SMSPwd", SqlDbType.VarChar, SMSPwd);
            sqlExec.AddCommandParam("@ret",SqlDbType.Int, ParameterDirection.Output, ret ) ;

            //Executes SQL statement
            sqlExec.SPExecuteNonQuery(sql);

            ret = Convert.ToInt32(sqlExec.ReadCommandParam("@ret"));
         }
         catch (SqlException objException)
         {
            string prefixMsg = string.Format("AuthenticateDriverSignIn -> SqlException: Boxid={0} ({1},{2})", boxId, SMSID, SMSPwd);
            Util.ProcessDbException(prefixMsg, objException);
            throw new DASException(objException.Message);            
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = string.Format("AuthenticateDriverSignIn -> EXC: Boxid={0} ({1},{2})", boxId, SMSID, SMSPwd); ;
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         return ret;
      }




      public string GetTripDriver(string drID, long vehicleID, DateTime fromDate, DateTime toDate)
      {
          string driverName = "";
          try
          {
              //Prepares SQL statement
              string sql = String.Format("SELECT dbo.GetTripDriver('{0}',{1},'{2}','{3}')", drID, vehicleID, fromDate, toDate);

              //Executes SQL statement
              driverName = (string)sqlExec.SQLExecuteScalar(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve Trip driver";
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve Trip driver";
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return driverName;
      }

       // Changes for TimeZone Feature start
      public string GetTripDriverWithTimezone_NewTZ(int userId, string drID, long vehicleID, DateTime fromDate, DateTime toDate)
      {
          string driverName = "";
          try
          {
              //Prepares SQL statement
              string sqlHeader = " DECLARE @Timezone float" +
                    " DECLARE @DayLightSaving int DEClARE @dtFrom DateTime DECLARE @dtTo DateTime " +
                    " SET @dtFrom = '" + Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'; SET @dtTo = '" + Convert.ToDateTime(toDate).ToString("yyyy-MM-dd HH:mm:ss.fff") + "';" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SELECT @DayLightSaving=dbo.fn_GetDSTOffset('" + Convert.ToDateTime(fromDate).ToString("MM/dd/yyyy HH:mm:ss.fff") + "') " +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving;";


              string sql = sqlHeader + String.Format(" SELECT dbo.GetTripDriver('{0}',{1},DATEADD(minute,-(@Timezone * 60),@dtFrom),DATEADD(minute,-(@Timezone * 60),@dtTo))", drID, vehicleID);

              //Executes SQL statement
              driverName = (string)sqlExec.SQLExecuteScalar(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve Trip driver";
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve Trip driver";
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return driverName;
      }
       // Changes for TimeZone Feature end

      public string GetTripDriverWithTimezone(int userId, string drID, long vehicleID, DateTime fromDate, DateTime toDate)
      {
          string driverName = "";
          try
          {
              //Prepares SQL statement
              string sqlHeader = " DECLARE @Timezone float" +
                    " DECLARE @DayLightSaving int DEClARE @dtFrom DateTime DECLARE @dtTo DateTime " +
                    " SET @dtFrom = '" + Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'; SET @dtTo = '" + Convert.ToDateTime(toDate).ToString("yyyy-MM-dd HH:mm:ss.fff") + "';" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SELECT @DayLightSaving=dbo.fn_GetDSTOffset('" + Convert.ToDateTime(fromDate).ToString("MM/dd/yyyy HH:mm:ss.fff") + "') " +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving;";


              string sql = sqlHeader + String.Format(" SELECT dbo.GetTripDriver('{0}',{1},DATEADD(hour,-@Timezone,@dtFrom),DATEADD(hour,-@Timezone,@dtTo))", drID, vehicleID);

              //Executes SQL statement
              driverName = (string)sqlExec.SQLExecuteScalar(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve Trip driver";
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve Trip driver";
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return driverName;
      }

      // Changes for TimeZone Feature start
      public string GetTripDriverKeyFobIdWithTimezone_NewTZ(int userId, string drID, long vehicleID, DateTime fromDate, DateTime toDate)
      {
          string driverNameKeyFobId = "";
          try
          {
              ////Prepares SQL statement
              string sqlHeader = " DECLARE @Timezone float" +
                    " DECLARE @DayLightSaving int DEClARE @dtFrom DateTime DECLARE @dtTo DateTime " +
                    " SET @dtFrom = '" + Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'; SET @dtTo = '" + Convert.ToDateTime(toDate).ToString("yyyy-MM-dd HH:mm:ss.fff") + "';" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SELECT @DayLightSaving=dbo.fn_GetDSTOffset('" + Convert.ToDateTime(fromDate).ToString("MM/dd/yyyy HH:mm:ss.fff") + "') " +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving;";


              string sql = sqlHeader + String.Format(" SELECT dbo.GetTripDriverKeyFobId('{0}',{1},DATEADD(minute,-(@Timezone* 60),@dtFrom),DATEADD(minute,-(@Timezone* 60),@dtTo))", drID, vehicleID);

              //Executes SQL statement
              driverNameKeyFobId = (string)sqlExec.SQLExecuteScalar(sql);


          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve Trip driver KeyFobId";
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve Trip driver KeyFobId";
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return driverNameKeyFobId;
      }

      // Changes for TimeZone Feature end

      public string GetTripDriverKeyFobIdWithTimezone(int userId, string drID, long vehicleID, DateTime fromDate, DateTime toDate)
      {
          string driverNameKeyFobId = "";
          try
          {
              ////Prepares SQL statement
              string sqlHeader = " DECLARE @Timezone int" +
                    " DECLARE @DayLightSaving int DEClARE @dtFrom DateTime DECLARE @dtTo DateTime " +
                    " SET @dtFrom = '" + Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'; SET @dtTo = '" + Convert.ToDateTime(toDate).ToString("yyyy-MM-dd HH:mm:ss.fff") + "';" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SELECT @DayLightSaving=dbo.fn_GetDSTOffset('" + Convert.ToDateTime(fromDate).ToString("MM/dd/yyyy HH:mm:ss.fff") + "') " +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving;";


              string sql = sqlHeader + String.Format(" SELECT dbo.GetTripDriverKeyFobId('{0}',{1},DATEADD(hour,-@Timezone,@dtFrom),DATEADD(hour,-@Timezone,@dtTo))", drID, vehicleID);

              //Executes SQL statement
              driverNameKeyFobId = (string)sqlExec.SQLExecuteScalar(sql);


          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve Trip driver KeyFobId";
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve Trip driver KeyFobId";
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return driverNameKeyFobId;
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
       public int AddDriverMsg(int driverId, int userId,string email,string msgBody, DateTime msgDateTime)
       {
           int rowsAffected = VLF.CLS.Def.Const.unassignedIntValue;
           try
           {
               // Set SQL command
               string sql = "INSERT INTO vlfDriverMsgs( " +
                               "DriverId" +
                               ",UserId" +
                               ",Email" +
                               ",MsgBody" +
                               ",MsgDateTime)";
               sql += " VALUES ( @driverId,@userId,@email,@msgBody,@msgDateTime)";
               sql += " Select Max(MsgId) from vlfDriverMsgs"; 
               sqlExec.ClearCommandParameters();
               // Add parameters to SQL statement
               sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);
               sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
               sqlExec.AddCommandParam("@email", SqlDbType.Text  , email);
               sqlExec.AddCommandParam("@msgBody", SqlDbType.Text , msgBody);
               sqlExec.AddCommandParam("@msgDateTime", SqlDbType.DateTime, msgDateTime);

               //Executes SQL statement
               object currentMsgId = sqlExec.SQLExecuteScalar(sql);
               if (currentMsgId != null)
                   rowsAffected = Convert.ToInt32(currentMsgId);
           }
           catch (SqlException objException)
           {
               string prefixMsg = "Unable to add new driver text msg.";
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               string prefixMsg = "Unable to add driver new text msg.";
               throw new DASException(prefixMsg + " " + objException.Message);
           }
           return rowsAffected;
       }

       /// <summary>
       /// Get Driver Messages
       /// </summary>
       /// <param name="driverId"></param>
       /// <param name="fromDate"></param>
       /// <param name="toDate"></param>
       /// <returns></returns>
       public DataSet GetDriverMsgs(int driverId, DateTime fromDate, DateTime toDate )
       {
           DataSet sqlDataSet = null;
           try
           {
               //Prepares SQL statement
               string sql = String.Format("SELECT * FROM vlfDriverMsgs WHERE driverId={0} AND MsgDateTime between '{1}' and '{2}'",
                   driverId, fromDate,toDate);

               //Executes SQL statement
               sqlDataSet = sqlExec.SQLExecuteDataset(sql);
           }
           catch (SqlException objException)
           {
               string prefixMsg = "Unable to retrieve driver messages: DriverId=" + driverId + ".";
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               string prefixMsg = "Unable to retrieve driver messages: DriverId=" + driverId + ".";
               throw new DASException(prefixMsg + " " + objException.Message);
           }
           return sqlDataSet;
       }


       #endregion
   }
}
