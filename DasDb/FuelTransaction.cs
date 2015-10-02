using System;
using System.Data;			   // for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	   // for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;


namespace VLF.DAS.DB
{

   public enum fuelTransactionStatus : byte
   {
      FuelTransaction_NEW           = 0,
      FuelTransaction_PENDING       = 1,
      FuelTransaction_SCHEDULED     = 2,
      FuelTransaction_OK            = 3,
      FuelTransaction_PARTIALLY_OK  = 4,
      FuelTransaction_FRAUD         = 5, 
      FuelTransaction_EXPIRED       = 6,
      FuelTransaction_VEHICLEPARKED = 7
   }

   public class FuelData
   {
      public long _id;
      public int _boxId;
      public double  _latitude,              ///< coming from the web call
                     _latitudeFound,         ///< found in vlfMsgInHst
                     _longitude,             ///< coming from the web call   
                     _longitudeFound;        ///< found in vlfMsgInHst
      public DateTime _dtTransaction,        ///< when the location was read 
                      _dtLastSchedule,       ///< scheduled for last time
                      _dtLocationFound;      ///< when the location was found in history   
      public long _resolutionTimeInterval;   ///< how often the check will be scheduled, minutes 
      public int _historyTimeRange;         ///< how far back in time are you going to search for a reported location, minutes
      public int _maximumDistance ;          ///< if you find a location at a distance smaller than  _resolutionDistance                                        
      public string _address;                ///< read from history 
      public string _emailAddress;           ///< the email address where the notification is sent 
      public int _timeZone;                  ///< timezone
      public fuelTransactionStatus _status;

      public string _vehicleDescription;
      public string _licensePlate;
      public string _data;                   ///< you extract address from field 5, 6, 7, 10 (street, city, country, state)


      public FuelData(FuelData fuelData)
      {
         _id = fuelData._id;
         _boxId = fuelData._boxId;
         _latitude = fuelData._latitude;
         _longitude = fuelData._longitude;
         _dtTransaction = fuelData._dtTransaction;
         _dtLastSchedule = fuelData._dtLastSchedule;
         _resolutionTimeInterval = fuelData._resolutionTimeInterval;
         _historyTimeRange = fuelData._historyTimeRange;
         _maximumDistance = fuelData._maximumDistance;
         _emailAddress = fuelData._emailAddress;
         _timeZone = fuelData._timeZone;

         _status = fuelData._status;
         _vehicleDescription = fuelData._vehicleDescription;
         _licensePlate = fuelData._licensePlate;
         _data = fuelData._data;
      }

      public FuelData(long id, int boxId, double latitude, double longitude, long resolutionTimeInterval)
      {
         _id = id;
         _boxId = boxId;
         _latitude = latitude;
         _longitude = longitude;
         _resolutionTimeInterval = resolutionTimeInterval;
         _status = fuelTransactionStatus.FuelTransaction_PENDING;
      }



      public string FuelFraudEmail()
      {
         // this gets vehicleInformation, driver information, licensePlate         
         switch (_status)
         {
            case fuelTransactionStatus.FuelTransaction_VEHICLEPARKED:
               return string.Format(@"Vehicle:{0}, license plate:{7} fueled at {1}, BUT the vehicle was PARKED at {6}, distance:{8:0.00} miles, see map <http://maps.google.com/maps?q=from:{2},{3}+to:{4},{5}> ",
                                 _vehicleDescription,
                                 _dtTransaction.AddHours(_timeZone),
                                 _latitude,
                                 _longitude,
                                 _latitudeFound,
                                 _longitudeFound,
                                 _dtLocationFound.AddHours(_timeZone),
                                 _licensePlate,
                                 (double)(_maximumDistance) * FuelTransaction.CT_MILES_PER_KM);

            case fuelTransactionStatus.FuelTransaction_FRAUD:
               return string.Format(@"Vehicle:{0}, license plate:{7} fueled at {1}, BUT the closest location was found at {6}, distance:{8:0.00} miles, see map <http://maps.google.com/maps?q=from:{2},{3}+to:{4},{5}> ",
                                 _vehicleDescription,
                                 _dtTransaction.AddHours(_timeZone),
                                 _latitude,
                                 _longitude,                                 
                                 _latitudeFound,
                                 _longitudeFound,
                                 _dtLocationFound.AddHours(_timeZone),
                                 _licensePlate,
                                 (double)(_maximumDistance) * FuelTransaction.CT_MILES_PER_KM);
            case fuelTransactionStatus.FuelTransaction_EXPIRED:
               return string.Format(@"No GPS location was found for vehicle:{0}, license plate:{1} fueled at {2}, location <http://maps.google.com/maps?q={3},{4}> , last attempt was at {5}",
                                 _vehicleDescription,
                                 _licensePlate,
                                 _dtTransaction.AddHours(_timeZone),
                                 _latitude,
                                 _longitude,
                                 _dtLastSchedule.AddHours(_timeZone));
            default:
               return ToString();
         }                           
      }

      public override string ToString()
      {
         return string.Format("id:{0}, boxId:{1} fueled at {2} located at ({3},{4}) but the closest location at that time was ({5},{6}) at {7}, RES={8} INT={9} MAX={10} email={11} status={12}",
                           _id,
                           _boxId,
                           _dtTransaction,
                           _latitude,
                           _longitude,                           
                           _latitudeFound,
                           _longitudeFound,
                           _dtLocationFound,
                           _resolutionTimeInterval,
                           _historyTimeRange,      
                           _maximumDistance,
                           _emailAddress,
                           _status.ToString() ) ;      

      }
   }


   /**  
    *   the fields for the vlfFuelTransaction
    *       - id              bigint          autogenerated
    *       - vinNum          varchar(32)     not null
    *       - latitude        double          not null, valid
    *       - longitude       double          not null, valid
    *       - data            varchar(256)    extra data in XML format
    *       - dtTransaction   datetime        when the call happened
    *       - dtReceived      datetime        when it was received
    *       - status          byte            0-NEW, 1-PENDING, 2-SCHEDULED, 3-OK, 4-FAILED
    *    ------------     
    *       - _maximumDistance      int       what is the maximum distance you need to draw a conclusion regarding the transaction
    *       - historyTimeInterval   int       in seconds, from preferences, it allows the engine 
    *                                         to look around dtTransaction - historyTimeInterval, in history and find 
    *                                         the closest location from the one provided by the transaction
    *       - resolutionInterval    bigint    next time you schedule to try and solve the transaction
    *       - dtScheduled     datetime        when it will be visited again - added from the organization's policy 
    *       - dtLastSchedule  dateTime        when you will try last time   - added from the organization's policy 
    *       - boxId           int             added by the trigger
    *       - driverId        int             added by the trigger
    *       - driversName     varchar         added by the trigger
    *       - LicensePlate    varchar         added by the trigger
    *       - VehicleDescription              added by the trigger
    *       - OrganizationId  int             added by the trigger
    *       - email           varchar         where to send the email notifications
    * 
    *  \comments     all datetimes are in GMT format 
    * 
    */
   public class FuelTransaction : TblGenInterfaces
   {
      public static double CT_MILES_PER_KM = 0.000621371192237;
      static object _SYNC=new object();
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public FuelTransaction(SQLExecuter sqlExec) : base("vlfFuelTransaction", sqlExec)
      {
      }

      public static bool ValidDateTime(DateTime dtWhen)
      {
         return (dtWhen > DateTime.Now.AddDays(1)) || (dtWhen < DateTime.Now.AddDays(58));
      }

      public static bool ValidLocation( double latitude, double longitude)
      {
         return (-90.0 < latitude && latitude < 90.0 && -180 < longitude && longitude < 180);
      }
      /// <summary>
      ///         the store procedure will first attempt to see if there is a VINNUM
      ///         in our database and then save the transaction
      /// </summary>
      /// <param name="vinNum"></param>
      /// <param name="dtWhen"></param>       
      /// <param name="latitude"></param>
      /// <param name="longitude"></param>
      /// <param name="data"></param>
      /// <returns>
      ///          0       exception in our database, data not saved
      ///         -1       VINNUM does not exist in our tables, data not saved
      ///         -2       location not valid in the world/USA, data not saved
      ///         -3       dtWhen is not within our range (-60 days, +1 day), data not saved
      ///         -4       the extra data is not useful, data saved
      /// </returns>
      public long AddFuelTransaction(string vinNum, DateTime dtWhen, double latitude, double longitude, string xmlData)
      {
         string prefixMsg = string.Format(" AddFuelTransaction -> {0} at={1}, location [{2},{3}]  {4}", vinNum, dtWhen, latitude, longitude, xmlData);
         long id = 0;

         if (!ValidDateTime(dtWhen))
         {
            Util.BTrace(Util.ERR0, prefixMsg + " DateTime not valid") ;
                     
            return -3;
         }

         if (!ValidLocation(latitude, longitude))
         {
           Util.BTrace(Util.ERR0, prefixMsg + " location not valid") ;
           return -2;
         }


         if (null != xmlData)
         {
            int len = xmlData.Length > 255 ? 255 : xmlData.Length;
            xmlData = xmlData.Substring(0, len);
         }
         else
            xmlData = string.Empty;

         try
         {
            
            string sql = "AddFuelTransaction";
            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@vinNum", SqlDbType.VarChar, vinNum, 32);
            sqlExec.AddCommandParam("@dtTransaction", SqlDbType.DateTime, dtWhen);
            sqlExec.AddCommandParam("@latitude", SqlDbType.Float, latitude);
            sqlExec.AddCommandParam("@longitude", SqlDbType.Float, longitude);
            sqlExec.AddCommandParam("@xmlData", SqlDbType.VarChar, xmlData, 256);
            sqlExec.AddCommandParam("@id", SqlDbType.BigInt, ParameterDirection.Output, id);

            if (sqlExec.RequiredTransaction())
            {
               // 4. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 5. Executes SQL statement
            int res = sqlExec.SPExecuteNonQuery(sql);

            id = (DBNull.Value == sqlExec.ReadCommandParam("@id")) ?
                                          0 : Convert.ToInt64(sqlExec.ReadCommandParam("@id").ToString());
            
         }
         catch (SqlException objException)
         {            
            Util.ProcessDbException(prefixMsg, objException);
            throw new DASException(objException.Message);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {          
            throw new DASException(prefixMsg + " " + objException.Message);
         }    

         return id;
      }




      /// <summary>
      ///         the store procedure will first attempt to see if there is a VINNUM
      ///         in our database and then save the transaction
      /// </summary>
      /// <param name="vinNum"></param>
      /// <param name="dtWhen"></param>       
      /// <param name="latitude"></param>
      /// <param name="longitude"></param>
      /// <param name="data"></param>
      /// <returns>
      ///          0       exception in our database, data not saved
      ///         -1       VINNUM does not exist in our tables, data not saved
      ///         -2       location not valid in the world/USA, data not saved
      ///         -3       dtWhen is not within our range (-60 days, +1 day), data not saved
      ///         -4       the extra data is not useful, data saved
      /// </returns>
      public long AddFuelTransactionByUnitNum(string unitNum, DateTime dtWhen, double latitude, double longitude, string xmlData)
      {
          string prefixMsg = string.Format(" AddFuelTransactionByUnitNum -> {0} at={1}, location [{2},{3}]  {4}", unitNum, dtWhen, latitude, longitude, xmlData);
          long id = 0;

          if (!ValidDateTime(dtWhen))
          {
              Util.BTrace(Util.ERR0, prefixMsg + " DateTime not valid");

              return -3;
          }

          if (!ValidLocation(latitude, longitude))
          {
              Util.BTrace(Util.ERR0, prefixMsg + " location not valid");
              return -2;
          }


          if (null != xmlData)
          {
              int len = xmlData.Length > 255 ? 255 : xmlData.Length;
              xmlData = xmlData.Substring(0, len);
          }
          else
              xmlData = string.Empty;

          try
          {

              string sql = "AddFuelTransactionByUnitNum";
              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@unitNum", SqlDbType.VarChar, unitNum, 32);
              sqlExec.AddCommandParam("@dtTransaction", SqlDbType.DateTime, dtWhen);
              sqlExec.AddCommandParam("@latitude", SqlDbType.Float, latitude);
              sqlExec.AddCommandParam("@longitude", SqlDbType.Float, longitude);
              sqlExec.AddCommandParam("@xmlData", SqlDbType.VarChar, xmlData, 256);
              sqlExec.AddCommandParam("@id", SqlDbType.BigInt, ParameterDirection.Output, id);

              if (sqlExec.RequiredTransaction())
              {
                  // 4. Attach current command SQL to transaction
                  sqlExec.AttachToTransaction(sql);
              }
              // 5. Executes SQL statement
              int res = sqlExec.SPExecuteNonQuery(sql);

              id = (DBNull.Value == sqlExec.ReadCommandParam("@id")) ?
                                            0 : Convert.ToInt64(sqlExec.ReadCommandParam("@id").ToString());

          }
          catch (SqlException objException)
          {
              Util.ProcessDbException(prefixMsg, objException);
              throw new DASException(objException.Message);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              throw new DASException(prefixMsg + " " + objException.Message);
          }

          return id;
      }




      /// <summary>
      ///         the store procedure will first attempt to see if there is a VINNUM
      ///         in our database and then save the transaction
      /// </summary>
      /// <param name="vinNum"></param>
      /// <param name="dtWhen"></param>       
      /// <param name="latitude"></param>
      /// <param name="longitude"></param>
      /// <param name="data"></param>
      /// <returns>
      ///          0       exception in our database, data not saved
      ///         -1       VINNUM does not exist in our tables, data not saved
      ///         -2       location not valid in the world/USA, data not saved
      ///         -3       dtWhen is not within our range (-60 days, +1 day), data not saved
      ///         -4       the extra data is not useful, data saved
      /// </returns>
      public long AddFuelCardTransaction(string cardNum, DateTime dtWhen, double latitude, double longitude, string xmlData)
      {
          string prefixMsg = string.Format(" AddFuelCardTransaction -> {0} at={1}, location [{2},{3}]  {4}", cardNum, dtWhen, latitude, longitude, xmlData);
          long id = 0;

          if (!ValidDateTime(dtWhen))
          {
              Util.BTrace(Util.ERR0, prefixMsg + " DateTime not valid");

              return -3;
          }

          if (!ValidLocation(latitude, longitude))
          {
              Util.BTrace(Util.ERR0, prefixMsg + " location not valid");
              return -2;
          }


          if (null != xmlData)
          {
              int len = xmlData.Length > 255 ? 255 : xmlData.Length;
              xmlData = xmlData.Substring(0, len);
          }
          else
              xmlData = string.Empty;

          try
          {

              string sql = "AddFuelCardTransaction";
              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@cardNum", SqlDbType.VarChar, cardNum, 16);
              sqlExec.AddCommandParam("@dtTransaction", SqlDbType.DateTime, dtWhen);
              sqlExec.AddCommandParam("@latitude", SqlDbType.Float, latitude);
              sqlExec.AddCommandParam("@longitude", SqlDbType.Float, longitude);
              sqlExec.AddCommandParam("@xmlData", SqlDbType.VarChar, xmlData, 256);
              sqlExec.AddCommandParam("@id", SqlDbType.BigInt, ParameterDirection.Output, id);

              if (sqlExec.RequiredTransaction())
              {
                  // 4. Attach current command SQL to transaction
                  sqlExec.AttachToTransaction(sql);
              }
              // 5. Executes SQL statement
              int res = sqlExec.SPExecuteNonQuery(sql);

              id = (DBNull.Value == sqlExec.ReadCommandParam("@id")) ?
                                            0 : Convert.ToInt64(sqlExec.ReadCommandParam("@id").ToString());

          }
          catch (SqlException objException)
          {
              Util.ProcessDbException(prefixMsg, objException);
              throw new DASException(objException.Message);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              throw new DASException(prefixMsg + " " + objException.Message);
          }

          return id;
      }

      public int UpdateFuelTransaction(string sql, SqlParameter[] param)
      {
         string prefixMsg = "Unable to update fuel transaction with= " + sql;
         Util.BTrace(Util.ERR0, "-- UpdateFuelTransaction -> {0}", sql);
         try
         {
            return UpdateRow(sql, param);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }

         return 0;
      }

      /// <summary>
      ///         if they want to revisit the data and add it 
      /// </summary>
      /// <param name="id"></param>
      /// <param name="data"></param>
      /// <returns></returns>
      public bool UpdateFuelTransaction(long id, string data)
      {
         Util.BTrace(Util.ERR0, ">> UpdateFuelTransaction -> {0} {1}", id, data);

         string prefixMsg = "Unable to update fuel transaction with=[" + data +  "], for id=" +id ;
         int rowsAffected = 0;

         string sql = "UPDATE " + tableName +
                     " SET data='" + data.Replace("'", "''") + "'" +
                     " WHERE id=" + id;
         try
         {
            //Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

            Util.BTrace(Util.ERR0, "<< UpdateFuelTransaction -> OK {0} {1}", id, data);
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
            throw new DASAppViolatedIntegrityConstraintsException(prefixMsg);
         }

         return true;
      }

      /// <summary>
      ///      in the case you couldn't find a resolution
      /// </summary>
      /// <param name="fuelData"></param>
      /// <returns></returns>
      public bool ScheduleFuelTransaction(FuelData fuelData)
      {
         if (null != fuelData)
         {
            Util.BTrace(Util.ERR0, ">> ScheduleFuelTransaction -> {0}", fuelData.ToString() );

            string prefixMsg = "Unable to schedule fuel transaction for=" + fuelData.ToString();
            int rowsAffected = 0;

            string sql = "UPDATE vlfFuelTransaction SET dtScheduled='" +
                        DateTime.UtcNow.AddMinutes(fuelData._resolutionTimeInterval).ToString("MM/dd/yyyy HH:mm:ss.fff") +
                        "', status=" + (int)fuelData._status +
                        " WHERE Id=" + fuelData._id;
            try
            {
               //Executes SQL statement
               rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
               Util.BTrace(Util.ERR0, " ScheduleFuelTransaction -> OK {0}", fuelData.ToString());
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
               throw new DASAppViolatedIntegrityConstraintsException(prefixMsg);
            }

            return true;
         }

         return false;
      }


      /// <summary>
      ///      in case you found a resolution
      ///      - if OK, then move the transaction in a history table ; in case there is a violation, send an email
      ///      - if expired, then move the transaction in a history table ; in case there is the setting, send an email to warn about that transaction
      /// </summary>
      /// <param name="fuelData"></param>
      /// <returns></returns>
      public void CloseFuelTransaction(FuelData fuelData)
      {
         if (null != fuelData)
         {
            Util.BTrace(Util.ERR0, ">> CloseFuelTransaction -> {0}", fuelData.ToString());


            string prefixMsg = "Unable to close fuel transaction for=" + fuelData.ToString();

            try
            {
               long newId = 0L;
               // call the store procedure to move the record
               string sql = "CloseFuelTransaction";
               sqlExec.ClearCommandParameters();

               sqlExec.AddCommandParam("@id", SqlDbType.BigInt, fuelData._id);
               sqlExec.AddCommandParam("@status", SqlDbType.TinyInt, (byte)fuelData._status);
               sqlExec.AddCommandParam("@dtLocationFound", SqlDbType.DateTime, fuelData._dtLocationFound);
               sqlExec.AddCommandParam("@latitudeFound", SqlDbType.Float, fuelData._latitudeFound);
               sqlExec.AddCommandParam("@longitudeFound", SqlDbType.Float, fuelData._longitudeFound);
//             sqlExec.AddCommandParam("@address", SqlDbType.VarChar, fuelData._address, 128);
               sqlExec.AddCommandParam("@newId", SqlDbType.BigInt, ParameterDirection.Output, newId);

               if (sqlExec.RequiredTransaction())
               {
                  // 4. Attach current command SQL to transaction
                  sqlExec.AttachToTransaction(sql);
               }
               // 5. Executes SQL statement
               int res = sqlExec.SPExecuteNonQuery(sql);

               newId = (DBNull.Value == sqlExec.ReadCommandParam("@newId")) ?
                                                         0 : Convert.ToInt64(sqlExec.ReadCommandParam("@newId").ToString());

               Util.BTrace(Util.ERR0, "<< CloseFuelTransaction -> OK {0}", fuelData.ToString());
            }
            catch (SqlException objException)
            {
               Util.ProcessDbException(prefixMsg, objException);
               throw new DASException(objException.Message);
            }
            catch (DASDbConnectionClosed exCnn)
            {
               throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
               throw new DASException(prefixMsg + " " + objException.Message);
            }
         }
         else
            Util.BTrace(Util.ERR0, " CloseFuelTransaction -> null param");
      }



      /// <summary>
      ///      in case you found a resolution
      ///      - if OK, then move the transaction in a history table ; in case there is a violation, send an email
      ///      - if expired, then move the transaction in a history table ; in case there is the setting, send an email to warn about that transaction
      /// </summary>
      /// <param name="fuelData"></param>
      /// <returns></returns>
      public void CloseFuelTransaction_New(FuelData fuelData)
      {
          if (null != fuelData)
          {
              Util.BTrace(Util.ERR0, ">> CloseFuelTransaction -> {0}", fuelData.ToString());


              string prefixMsg = "Unable to close fuel transaction for=" + fuelData.ToString();

              try
              {
                  long newId = 0L;
                  // call the store procedure to move the record
                  string sql = "CloseFuelTransaction_New";
                  sqlExec.ClearCommandParameters();

                  sqlExec.AddCommandParam("@id", SqlDbType.BigInt, fuelData._id);
                  sqlExec.AddCommandParam("@status", SqlDbType.TinyInt, (byte)fuelData._status);
                  sqlExec.AddCommandParam("@dtLocationFound", SqlDbType.DateTime, fuelData._dtLocationFound);
                  sqlExec.AddCommandParam("@latitudeFound", SqlDbType.Float, fuelData._latitudeFound);
                  sqlExec.AddCommandParam("@longitudeFound", SqlDbType.Float, fuelData._longitudeFound);
                  sqlExec.AddCommandParam("@data", SqlDbType.VarChar, fuelData._data, 512);
                  sqlExec.AddCommandParam("@newId", SqlDbType.BigInt, ParameterDirection.Output, newId);

                  if (sqlExec.RequiredTransaction())
                  {
                      // 4. Attach current command SQL to transaction
                      sqlExec.AttachToTransaction(sql);
                  }
                  // 5. Executes SQL statement
                  int res = sqlExec.SPExecuteNonQuery(sql);

                  newId = (DBNull.Value == sqlExec.ReadCommandParam("@newId")) ?
                                                            0 : Convert.ToInt64(sqlExec.ReadCommandParam("@newId").ToString());

                  Util.BTrace(Util.ERR0, "<< CloseFuelTransaction -> OK {0}", fuelData.ToString());
              }
              catch (SqlException objException)
              {
                  Util.ProcessDbException(prefixMsg, objException);
                  throw new DASException(objException.Message);
              }
              catch (DASDbConnectionClosed exCnn)
              {
                  throw new DASDbConnectionClosed(exCnn.Message);
              }
              catch (Exception objException)
              {
                  throw new DASException(prefixMsg + " " + objException.Message);
              }
          }
          else
              Util.BTrace(Util.ERR0, " CloseFuelTransaction -> null param");
      }


      public int RecoverPendingTransactions()
      {
         Util.BTrace(Util.ERR0, ">> RecoverPendingTransactions ...");

         string sql = "UPDATE " + tableName + " SET status = " + (int)fuelTransactionStatus.FuelTransaction_SCHEDULED  + 
                      " WHERE status = " + (int)fuelTransactionStatus.FuelTransaction_PENDING ;
         int rowsAffected = 0;
         string prefixMsg = String.Format("Unable to update pending transaction ");


         try
         {

            if(sqlExec.RequiredTransaction())
            {
            	sqlExec.AttachToTransaction(sql);
            }
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

            Util.BTrace(Util.ERR0, "<< RecoverPendingTransactions -> got {0}", rowsAffected);

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

         return rowsAffected;
      }


      /// <summary>
      ///      it change the status to InProcess and post the stamp for DateTimeHandled 
      ///      it extract all new transactions (status=0) AND
      ///      all scheduled transactions with status as SCHEDULED and dtScheduled higher then the current DateTime
      ///      the stored procedure returns 
      /// 
      ///            id, 
		///		      latitude,
		///		      longitude,
		///		         data,
		///		      dtTransaction,
		///		         status,
		///		         resolution,
		///		      boxId,
		///	            driverId,                           -- for email
      ///		         driversName,                        -- for email
      ///		         vehicleId,                          -- for email
      ///		         licensePlate,                       -- for email
      ///		         vehicleDescription,                 -- for email
		///		      emailAddress,             - for notification 
		///		      maximumDistance,          - for resolution
      ///		      resolutionInterval,       - for resolution
      ///		      historyTimeRange,         - for resolution
      ///		      dtLastSchedule            - for resolution
      /// 
      /// </summary>
      /// <returns></returns>
      public FuelData[] GetNextRawFuelTransactions(int size)
      {
         string prefixMsg = "Unable to get fuel transaction"; 

         if (size > 0)
         {
            try
            {
               long newId = 0L;
               // call the store procedure to move the record
               string sql = "GetNextFuelTransactions";
               sqlExec.ClearCommandParameters();

               sqlExec.AddCommandParam("@RowNum", SqlDbType.Int, size);
               if (sqlExec.RequiredTransaction())
               {
                  // 4. Attach current command SQL to transaction
                  sqlExec.AttachToTransaction(sql);
               }
               // 5. Executes SQL statement
               DataSet ds = sqlExec.SPExecuteDataset(sql);
               if (Util.IsDataSetValid(ds))
               {
                  int i = 0 ;
                  FuelData[] arr = new FuelData[ds.Tables[0].Rows.Count];
                  foreach (DataRow dr in ds.Tables[0].Rows)
                  {
                     Util.BTrace(Util.INF0, Util.DumpRow(dr, false));
                  
                     arr[i] = new FuelData(Convert.ToInt64(dr["id"].ToString()),
                                           Convert.ToInt32(dr["boxId"].ToString()),
                                           Convert.ToDouble(dr["latitude"].ToString()),
                                           Convert.ToDouble(dr["longitude"].ToString()),
                                           Convert.ToInt64(dr["resolutionInterval"].ToString()));

                     arr[i]._dtTransaction = Convert.ToDateTime(dr["dtTransaction"].ToString());
                     arr[i]._dtLastSchedule = Convert.ToDateTime(dr["dtLastSchedule"].ToString());
                     arr[i]._timeZone = Convert.ToInt32(dr["timeZone"].ToString());
                     arr[i]._emailAddress = dr["emailAddress"].ToString();
                     arr[i]._maximumDistance = Convert.ToInt32(dr["maximumDistance"].ToString());
                     arr[i]._historyTimeRange = Convert.ToInt32(dr["historyTimeRange"].ToString());
                     arr[i]._vehicleDescription = dr["vehicleDescription"].ToString();
                     arr[i]._licensePlate = dr["licensePlate"].ToString();
                     arr[i]._data = dr["data"].ToString();

                     ++i;
                  }

                  Util.BTrace(Util.INF0, " - GetNextRawFuelTransactions -> {0}", arr.Length);
                  return arr;

               }

            }
            catch (SqlException objException)
            {
               Util.ProcessDbException(prefixMsg, objException);
               throw new DASException(objException.Message);
            }
            catch (DASDbConnectionClosed exCnn)
            {
               throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
               throw new DASException(prefixMsg + " " + objException.Message);
            }
         }

         return null;
      }


      /// <summary>
      ///      it change the status to InProcess and post the stamp for DateTimeHandled 
      ///      it extract all new transactions (status=0) AND
      ///      all scheduled transactions with status as SCHEDULED and dtScheduled higher then the current DateTime
      ///      the stored procedure returns 
      /// 
      ///            id, 
      ///		      latitude,
      ///		      longitude,
      ///		         data,
      ///		      dtTransaction,
      ///		         status,
      ///		         resolution,
      ///		      boxId,
      ///	            driverId,                           -- for email
      ///		         driversName,                        -- for email
      ///		         vehicleId,                          -- for email
      ///		         licensePlate,                       -- for email
      ///		         vehicleDescription,                 -- for email
      ///		      emailAddress,             - for notification 
      ///		      maximumDistance,          - for resolution
      ///		      resolutionInterval,       - for resolution
      ///		      historyTimeRange,         - for resolution
      ///		      dtLastSchedule            - for resolution
      /// 
      /// </summary>
      /// <returns></returns>
      public FuelData[] GetNextRawFuelTransactions(int size, int superOrgId)
      {
          string prefixMsg = "Unable to get fuel transaction";

          lock (_SYNC)
          {
              if (size > 0)
              {
                  try
                  {
                      long newId = 0L;
                      // call the store procedure to move the record
                      string sql = "GetNextFuelTransactionsBySuperOrgId";
                      sqlExec.ClearCommandParameters();

                      sqlExec.AddCommandParam("@RowNum", SqlDbType.Int, size);
                      sqlExec.AddCommandParam("@SuperOrgId", SqlDbType.Int, superOrgId);
                      // 5. Executes SQL statement
                      DataSet ds = sqlExec.SPExecuteDataset(sql);
                      if (Util.IsDataSetValid(ds))
                      {
                          int i = 0;
                          FuelData[] arr = new FuelData[ds.Tables[0].Rows.Count];
                          foreach (DataRow dr in ds.Tables[0].Rows)
                          {
                              Util.BTrace(Util.INF0, Util.DumpRow(dr, false));

                              arr[i] = new FuelData(Convert.ToInt64(dr["id"].ToString()),
                                                    Convert.ToInt32(dr["boxId"].ToString()),
                                                    Convert.ToDouble(dr["latitude"].ToString()),
                                                    Convert.ToDouble(dr["longitude"].ToString()),
                                                    Convert.ToInt64(dr["resolutionInterval"].ToString()));

                              arr[i]._dtTransaction = Convert.ToDateTime(dr["dtTransaction"].ToString());
                              arr[i]._dtLastSchedule = Convert.ToDateTime(dr["dtLastSchedule"].ToString());
                              arr[i]._timeZone = Convert.ToInt32(dr["timeZone"].ToString());
                              arr[i]._emailAddress = dr["emailAddress"].ToString();
                              arr[i]._maximumDistance = Convert.ToInt32(dr["maximumDistance"].ToString());
                              arr[i]._historyTimeRange = Convert.ToInt32(dr["historyTimeRange"].ToString());
                              arr[i]._vehicleDescription = dr["vehicleDescription"].ToString();
                              arr[i]._licensePlate = dr["licensePlate"].ToString();
                              arr[i]._data = dr["data"].ToString();

                              ++i;
                          }

                          Util.BTrace(Util.INF0, " - GetNextRawFuelTransactions -> {0}", arr.Length);
                          return arr;

                      }

                  }
                  catch (SqlException objException)
                  {
                      Util.ProcessDbException(prefixMsg, objException);
                      throw new DASException(objException.Message);
                  }
                  catch (DASDbConnectionClosed exCnn)
                  {
                      throw new DASDbConnectionClosed(exCnn.Message);
                  }
                  catch (Exception objException)
                  {
                      throw new DASException(prefixMsg + " " + objException.Message);
                  }
              }
          }

          return null;
      }

      /// <summary>
      ///      this is useful because not everything is stored in FuelData
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public string GetMoreInfoFromFuelTransaction(long id)
      {
         try
         {
            DataSet ds = base.GetRowsByFilter("vlfFuelTransaction", string.Format("WHERE id={0}", id));
            if (Util.IsDataSetValid(ds))
            {
               DataRow dr = ds.Tables[0].Rows[0];
               // it should be only one row
               return string.Format("vin# {0} driver {1}",
                                    dr["vinNum"].ToString(),
                                    dr["driversName"].ToString());
                                    
            }
         }
         catch (Exception objException)
         {
            Util.BTrace(Util.ERR0, " - GetMoreInfoFromFuelTransaction -> for {0} EXC={1}", id, objException.Message);
         }

         return string.Empty;
      }


     /// <summary>
     /// Get Fuel Transaction History
     /// </summary>
     /// <param name="organizationId"></param>
     /// <param name="userId"></param>
     /// <param name="from"></param>
     /// <param name="to"></param>
     /// <returns></returns>
       public DataSet GetFuelTransactionHist(int organizationId, int userId,DateTime from,DateTime to )
      {
          DataSet ds = null;
          try
          {
              // 1. Set SQL command
              string sql = "sp_GetFuelTransactionHist";

              // 2. Add parameters to SQL statement
              sqlExec.ClearCommandParameters();
              sqlExec.AddCommandParam("@organizationId ", SqlDbType.Int, organizationId);
              sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
              sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
              sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
              ds = sqlExec.SPExecuteDataset(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = string.Format("Unable to retrieve GetFuelTransactionHist by organizationId '{0}',user id '{1}' ", organizationId,userId);
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = string.Format("Unable to retrieve GetFuelTransactionHist by organizationId '{0}',user id '{1}' ", organizationId, userId);
              throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
          }
          return ds;

      }



       /// <summary>
       /// Get Fuel Transaction History
       /// </summary>
       /// <param name="organizationId"></param>
       /// <param name="userId"></param>
       /// <param name="from"></param>
       /// <param name="to"></param>
       /// <returns></returns>
       public DataSet GetFuelFraudTransactionHist(int organizationId, int userId, DateTime from, DateTime to)
       {
           DataSet ds = null;
           try
           {
               // 1. Set SQL command
               string sql = "sp_GetFuelFraudTransactionHist";

               // 2. Add parameters to SQL statement
               sqlExec.ClearCommandParameters();
               sqlExec.AddCommandParam("@organizationId ", SqlDbType.Int, organizationId);
               sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
               sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
               sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
               ds = sqlExec.SPExecuteDataset(sql);
           }
           catch (SqlException objException)
           {
               string prefixMsg = string.Format("Unable to retrieve GetFuelTransactionHist by organizationId '{0}',user id '{1}' ", organizationId, userId);
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               string prefixMsg = string.Format("Unable to retrieve GetFuelTransactionHist by organizationId '{0}',user id '{1}' ", organizationId, userId);
               throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
           }
           return ds;

       }

   }
}
