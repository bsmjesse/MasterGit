/** \file      VehicleSummaryReport.cs
 *  \brief     the function is called for every vehicle per day to run for 24 hours
 *             it calculates all the numbers for the activity between 0..24 hours of the timezone of the vehicle
 *  \questions how do you insure that the procedure is not run more often than 24 hours for every vehicle ???           
 */ 
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using VLF.CLS;
using VLF.CLS.Def;
using VLF.ERR;
using System.Data.SqlClient;	// for SqlException


namespace VLF.DAS.DB
{
   /// <summary>
   ///      this class will create a Vehicle Summary Utilization Report
   ///
   ///     •	Vehicle Id
   //      •	Vehicle’s Name
   //      •	License Plate
   //      •	TimeZone
   //         o	The time zone of the vehicle
   //      •	DateTime
   //         o	For which day of the year is the summary
   //      •	InServiceHours
   //         o	The amount in seconds between the first Ignition ON and last Ignition OFF
   //      •	EngineOnHours
   //         o	The sum of all trips (duration between Ignition ON, Ignition OFF) for that day
   //      •	Idling
   //         o	The sum of all idle duration for that day
   //      •	PTOOnHours
   //         o	The sum of all trips (duration between PTO ON, PTO OFF) for that day
   //      •	TravelledDistance
   //         o	The sum of all distance trips for that day
   //      •	FuelConsumption
   //         o	The amount of fuel consumption for the day (if available)
   //      •	TypeOfFuel
   //         o	Description of fuel type
   //      •	HasPTO
   //         o	If the vehicle has PTO
   //      •	ViolationsCounter
   //         o	The number of violations messages for that day but not speed violations
   //      •	ViolationsDescription
   //         o	Is a string describing for Harsh Acceleration=60/Harsh Braking=58/Extreme Acceleration=61/Extreme Braking=59/SeatBelt=62
   ///
   /// </summary>
   public class VehicleSummaryReport
   {

      const int DIAG_NO_SENSOR_OFF = 100;
      const int DIAG_NO_SENSOR_ON  = 101;
      const int TRIP_NOT_FINISHED  = 102;


      // standard messages
      static Enums.MessageType[] hasSensorMask = 
            {
               Enums.MessageType.Coordinate,
               Enums.MessageType.Sensor,
               Enums.MessageType.Speed,
               Enums.MessageType.Speeding,
               Enums.MessageType.GeoFence,
               Enums.MessageType.KeyFobArm,
               Enums.MessageType.KeyFobDisarm,
               Enums.MessageType.KeyFobPanic,
               Enums.MessageType.Idling,
//               Enums.MessageType.GeoZone,
               Enums.MessageType.Alarm,
               Enums.MessageType.PositionUpdate,
               Enums.MessageType.Status,
               Enums.MessageType.BadSensor,         
               Enums.MessageType.HarshBraking,      
               Enums.MessageType.ExtremeBraking,
               Enums.MessageType.HarshAcceleration,
               Enums.MessageType.ExtremeAcceleration,
               Enums.MessageType.SeatBelt,
               Enums.MessageType.ExtendedIdling,
               Enums.MessageType.SensorExtended,
               Enums.MessageType.SendSMC,
               Enums.MessageType.TetheredState,
               Enums.MessageType.StoredPosition,  // last valid GPS ??
               Enums.MessageType.GPSAntennaOpen,
               Enums.MessageType.GPSAntennaShort,
               Enums.MessageType.GPSAntennaOK,
               Enums.MessageType.MainBatteryDisconnected,
               Enums.MessageType.SMCWriteDone
            };

      public const int IGNTION_ON = 0x04;
      public const int PTO_ON = 0x80;
      public string CT_VIOLATION_DESCRIPTION = string.Format("SP110={0};SP120={1};SP130={2};HB={3};HA={4};EB={5};EA={6}", 0, 0, 0, 0, 0, 0, 0);

      private static DateTime CT_SUMMARY_DATE = new DateTime(2000, 01, 01);  ///< used for markers of the summary of the data inside of the table
      private string connectionString = null;
      private DataTable tblVehicleSummary = null;   // this is a report per day for every boxid
      private DataTable tblTripSummary = null;      // this is a summary for every trip
      private DataSet ds = null;

      #region properties
      
      private int _vehicleId;
      private int _organizationId;
      private string _vehicleName, _licensePlate;
      private int _timeZone;
      private int _dayLightSaving ;
//      private string _driversName ;
      private bool _hasPTO ;

      #endregion
      /// <summary>
      /// Constructor
      /// </summary>
      public VehicleSummaryReport(int vehicleId, /*int organizationid, */ string connectionString)
      {
         Array.Sort(hasSensorMask);

         this.connectionString = connectionString;

         _vehicleId = vehicleId;
//         _organizationId = organizationid;

         FillProperties(vehicleId) ;

         #region DailyActivity table definition        
         tblVehicleSummary = new DataTable("VehicleSummary");
         // this fields are static 
         tblVehicleSummary.Columns.Add("BoxId", typeof(int));
         tblVehicleSummary.Columns.Add("VehicleId", typeof(int));
         tblVehicleSummary.Columns.Add("VehicleName", typeof(string));
         tblVehicleSummary.Columns.Add("TimeZone", typeof(int));
         tblVehicleSummary.Columns.Add("LicensePlate", typeof(string));

         tblVehicleSummary.Columns.Add("Date", typeof(DateTime));             // this is GMT + TimeZone
         tblVehicleSummary.Columns.Add("InServiceSeconds", typeof(int));      // in seconds
         tblVehicleSummary.Columns.Add("EngineOnSeconds", typeof(int));
         tblVehicleSummary.Columns.Add("IdlingInSeconds", typeof(int));
         tblVehicleSummary.Columns.Add("PTOOnSeconds", typeof(int));
         tblVehicleSummary.Columns.Add("TravelledDistanceKm", typeof(int));   // difference between odometer in the first packet and odometer in the last packet
         tblVehicleSummary.Columns.Add("FuelConsumption", typeof(int));       // the amount of fuel consumed during the day
         tblVehicleSummary.Columns.Add("HasPTO", typeof(bool));
//         tblVehicleSummary.Columns.Add("ViolationsCounter", typeof(int));     // 
//         tblVehicleSummary.Columns.Add("ViolationDescription", typeof(string));
         #endregion DailyActivity table definition

         #region TripSummary table definition
         tblTripSummary = new DataTable("TripSummary");
         // this fields are static 
         tblTripSummary.Columns.Add("VehicleId", typeof(int));
         tblTripSummary.Columns.Add("VehicleName", typeof(string));
         tblTripSummary.Columns.Add("LicensePlate", typeof(string));
         tblTripSummary.Columns.Add("TimeZone", typeof(int));

         // pointers in history if you want to expand the summary 
         tblTripSummary.Columns.Add("StartDate", typeof(DateTime));
         tblTripSummary.Columns.Add("EndDate", typeof(DateTime));
         tblTripSummary.Columns.Add("Distance", typeof(int));        // in km
         tblTripSummary.Columns.Add("IdlingTime", typeof(int));      // in seconds
         tblTripSummary.Columns.Add("FuelConsumption", typeof(int)); // in liters
         tblTripSummary.Columns.Add("PTOOnSeconds", typeof(int));
         tblTripSummary.Columns.Add("Diagnostic", typeof(int));   // used to diagnose if packets between ON.OFF are missing

         #endregion  TripSummary table definition
         ds = new DataSet();
         ds.Tables.Add(tblVehicleSummary);
         ds.Tables.Add(tblTripSummary);


      }

      /// <summary>
      ///      the store procedure return all extra information
      ///         _vehicleName, _licensePlate, _timeZone, _dayLightSaving, _driversName, _hasPTO
      /// </summary>
      /// <comment>
      ///      the exception should be caught at the calling level and handled there 
      ///      added the daylight saving logic for the vehicle info
      /// </comment>
      /// <param name="vehicleId"></param>
      public void FillProperties(int vehicleId)
      {
          using (SqlConnection conn = new SqlConnection(connectionString))
          {
               conn.Open();
               using (SqlCommand cmd = new SqlCommand("GetInfoForVehicle", conn))
               {
                  cmd.Parameters.Clear();
                  cmd.CommandType = CommandType.StoredProcedure;
                  cmd.Parameters.Add("@vehicleId", SqlDbType.Int);
                  cmd.Parameters["@vehicleId"].Value = vehicleId;

                  // output params
                  SqlParameter param = new SqlParameter("@vehicleName", SqlDbType.NVarChar, 32);
                  param.Direction = ParameterDirection.Output;
                  cmd.Parameters.Add(param);

                  param = new SqlParameter("@licensePlate", SqlDbType.NVarChar, 32);
                  param.Direction = ParameterDirection.Output;
                  cmd.Parameters.Add(param);
/*
                  param = new SqlParameter("@driversName", SqlDbType.NVarChar, 32);
                  param.Direction = ParameterDirection.Output;
                  cmd.Parameters.Add(param);
*/
                  param = new SqlParameter("@timeZone", SqlDbType.Int);
                  param.Direction = ParameterDirection.Output;
                  cmd.Parameters.Add(param);

                  param = new SqlParameter("@dayLightSaving", SqlDbType.Int);
                  param.Direction = ParameterDirection.Output;
                  cmd.Parameters.Add(param);

                  param = new SqlParameter("@hasPTO", SqlDbType.Bit);
                  param.Direction = ParameterDirection.Output;
                  cmd.Parameters.Add(param);

                  cmd.ExecuteNonQuery() ;

                  if (cmd.Parameters["@hasPTO"].Value == System.DBNull.Value      ||
                      cmd.Parameters["@timeZone"].Value == System.DBNull.Value    ||
//                      cmd.Parameters["@driversName"].Value == System.DBNull.Value ||
                      cmd.Parameters["@licensePlate"].Value == System.DBNull.Value ||
                      cmd.Parameters["@vehicleName"].Value == System.DBNull.Value )
                  throw new ApplicationException("SQL error") ;
                      
                  _hasPTO     = Convert.ToBoolean(cmd.Parameters["@hasPTO"].Value.ToString());
                  _timeZone   = Convert.ToInt32(cmd.Parameters["@timeZone"].Value.ToString());
                  _dayLightSaving = Convert.ToInt32(cmd.Parameters["@dayLightSaving"].Value.ToString());
//                  _driversName = cmd.Parameters["@driversName"].Value.ToString();
                  _licensePlate = cmd.Parameters["@licensePlate"].Value.ToString();
                  _vehicleName = cmd.Parameters["@vehicleName"].Value.ToString();
               }
          }
      }

      /// <summary>
      ///      returns the tblVehicleSummary and  tblIdlingDuration in a dataset
      /// </summary>
      /// <returns></returns>

      public DataSet result
      {
         get
         {
            return ds;
         }
      }

      /// <summary>
      ///      it returns boxId, OriginDateTime, BoxMsgInTypeId, SensorMask, CustomProp, 
      ///            ordered by OrigindDateTime
      ///      vehicleId, VehicleName,licensePlate, TimeZone, Driver's Name are coming from a different function
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="userId"></param>
      /// <param name="dtFrom">
      ///         this is in the timezone of the user
      /// </param>
      /// <param name="dtTo">
      ///          this is in the timezone of the user
      /// </param>
      /// <returns></returns>
      public DataSet Exec_GetAllMessages(int vehicleId, int userId, DateTime dtFrom, DateTime dtTo, ref bool hasPTO)
      {
         DataSet dsResult = null;
         hasPTO = false;
         try
         {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
               using (SqlCommand cmd = new SqlCommand("GetAllMessagesPerVehicleByUser", conn))
               {
                  cmd.Parameters.Clear();
                  cmd.CommandType = CommandType.StoredProcedure;
                  cmd.Parameters.Add("@vehicleId", SqlDbType.Int);
                  cmd.Parameters["@vehicleId"].Value = vehicleId;
                  cmd.Parameters.Add("@userId", SqlDbType.Int);
                  cmd.Parameters["@userId"].Value = userId;
                  cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                  cmd.Parameters["@fromDate"].Value = dtFrom;
                  cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                  cmd.Parameters["@toDate"].Value = dtTo;

                  SqlParameter param = new SqlParameter("@hasPTO", SqlDbType.Bit);
                  param.Direction = ParameterDirection.Output;
                  cmd.Parameters.Add(param);

                  // Create a new adapter and initialize it with the new SQL command
                  SqlDataAdapter sda = new SqlDataAdapter(cmd);
                  // Create a new data set
                  dsResult = new DataSet();

                  // It is time to update the data set with information from the data adapter
                  sda.Fill(dsResult, "MessagesPerVehicle");
                  hasPTO = (cmd.Parameters["@hasPTO"].Value == System.DBNull.Value) ? false :
                                 Convert.ToBoolean(cmd.Parameters["@hasPTO"].Value.ToString());
               }
            }
         }
         catch (Exception exc)
         {
            Util.BTrace(Util.ERR2, "-- VehicleSummaryReport.Exec_GetAllMessages -> EXC[{0}]", exc.Message);
         }

         return dsResult;
      }


      /// <summary>
      ///      it returns boxId, OriginDateTime, BoxMsgInTypeId, SensorMask, CustomProp, 
      ///            ordered by OrigindDateTime
      ///      vehicleId, VehicleName,licensePlate, TimeZone, Driver's Name are coming from a different function
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="dtFrom">
      ///         this is in the timezone of the vehicle
      /// </param>
      /// <param name="dtTo">
      ///         this is in the timezone of the vehicle
      /// </param>
      /// <param name="hasPTO"></param>
      /// <returns></returns>
      public DataSet Exec_GetAllMessages(int vehicleId, DateTime dtFrom, DateTime dtTo, ref bool hasPTO)
      {
         DataSet dsResult = null;
         hasPTO = false;
         try
         {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
               using (SqlCommand cmd = new SqlCommand("GetAllMessagesPerVehicle", conn))
               {
                  cmd.Parameters.Clear();
                  cmd.CommandType = CommandType.StoredProcedure;
                  cmd.Parameters.Add("@vehicleId", SqlDbType.Int);
                  cmd.Parameters["@vehicleId"].Value = vehicleId;
                  cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                  cmd.Parameters["@fromDate"].Value = dtFrom;
                  cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                  cmd.Parameters["@toDate"].Value = dtTo;

                  SqlParameter param = new SqlParameter("@hasPTO", SqlDbType.Bit);
                  param.Direction = ParameterDirection.Output;
                  cmd.Parameters.Add(param);

                  // Create a new adapter and initialize it with the new SQL command
                  SqlDataAdapter sda = new SqlDataAdapter(cmd);
                  // Create a new data set
                  dsResult = new DataSet();

                  // It is time to update the data set with information from the data adapter
                  sda.Fill(dsResult, "MessagesPerVehicle");
                  hasPTO = (cmd.Parameters["@hasPTO"].Value == System.DBNull.Value) ? false :
                                 Convert.ToBoolean(cmd.Parameters["@hasPTO"].Value.ToString());
               }
            }
         }
         catch (Exception exc)
         {
            Util.BTrace(Util.ERR2, "-- VehicleSummaryReport.Exec_GetAllMessages -> EXC[{0}]", exc.Message);
         }

         return dsResult;
      }

      /// <summary>
      ///      add an entry per day for DailyUtilizationReport
      /// </summary>
      public void AddRowPerDayForUtlization(DateTime dtFirstIgnOn, 
                                            DateTime dtLastIgnOff, 
                                            int engineOnSeconds, 
                                            int idlingSeconds, 
                                            int ptoONSeconds, 
                                            int distancePerDay, 
                                            int fuelConsumed)
      {
         using (SqlConnection conn = new SqlConnection(connectionString))
         {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
               cmd.CommandText =
                     "INSERT INTO dbo.DailyUtilizationReport(" +
                     " VehicleId, DateTime, VehicleName, LicensePlate, TimeZone, " +
                     " InServiceHours, EngineOnHours, IdlingHours, PTOOnHours, TravelledDistance, FuelConsumed, HasPTO " + 
                     ") VALUES (" +
                     " @vehicleId, @dateTime, @vehicleName, @licensePlate, @timeZone, " +
                     " @inServiceHours, @engineOnHours, @idlingHours, @ptoOnHours, @travelledDistance, @fuelConsumed, @hasPTO" + 
                     ")";
             

               cmd.Parameters.Add("@vehicleId", SqlDbType.Int);
               cmd.Parameters["@vehicleId"].Value = _vehicleId;

               cmd.Parameters.Add("@dateTime", SqlDbType.DateTime);
               cmd.Parameters["@dateTime"].Value = dtFirstIgnOn.AddHours(_timeZone);

               cmd.Parameters.Add("@vehicleName", SqlDbType.NVarChar, 32);
               cmd.Parameters["@vehicleName"].Value = _vehicleName;

               cmd.Parameters.Add("@licensePlate", SqlDbType.NVarChar, 32);
               cmd.Parameters["@licensePlate"].Value = _licensePlate;

               cmd.Parameters.Add("@timeZone", SqlDbType.Int);
               cmd.Parameters["@timeZone"].Value = _timeZone;

               cmd.Parameters.Add("@inServiceHours", SqlDbType.Int);
               cmd.Parameters["@inServiceHours"].Value = (int)(new TimeSpan(dtLastIgnOff.Ticks - dtFirstIgnOn.Ticks)).TotalSeconds;

               cmd.Parameters.Add("@engineOnHours", SqlDbType.Int);
               cmd.Parameters["@engineOnHours"].Value = engineOnSeconds;

               cmd.Parameters.Add("@idlingHours", SqlDbType.Int);
               cmd.Parameters["@idlingHours"].Value = idlingSeconds;

               cmd.Parameters.Add("@ptoOnHours", SqlDbType.Int);
               cmd.Parameters["@ptoOnHours"].Value = ptoONSeconds;

               cmd.Parameters.Add("@travelledDistance", SqlDbType.Int);
               cmd.Parameters["@travelledDistance"].Value = distancePerDay;

               cmd.Parameters.Add("@fuelConsumed", SqlDbType.Int);
               cmd.Parameters["@fuelConsumed"].Value = fuelConsumed;

               cmd.Parameters.Add("@hasPTO", SqlDbType.Bit);
               cmd.Parameters["@hasPTO"].Value = _hasPTO;

               cmd.ExecuteNonQuery();
            }
         }
      }

      
      /// <summary>
      ///         this could be called several times to update the counters and intervalSum variables
      ///         until the trip is ended
      /// </summary>
      /// <param name="dtIgnOn"></param>
      /// <param name="dtIgnOff"></param>
      /// <param name="distanceinTrip"></param>
      /// <param name="fuelReading"></param>
      /// <param name="idlingPerTrip"></param>
      /// <param name="tripDiagnostic"></param>
      public void AddTrip(DateTime dtIgnOn, DateTime dtIgnOff,
                          int distanceinTrip, int fuelReading, int idlingPerTrip, int ptoPerTrip, 
                          int tripDiagnostic)
      {
         Util.BTrace(Util.INF0, "AddTrip -> VID={7} from={0} to={1} distance={2} fuel={3} idling={4} pto={5} diag={6}",
          dtIgnOn, dtIgnOff, distanceinTrip, fuelReading, idlingPerTrip, ptoPerTrip, tripDiagnostic, _vehicleId);

         using (SqlConnection conn = new SqlConnection(connectionString))
         {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
               cmd.CommandText =
                     "INSERT INTO dbo.TripSummaryReport(" +
                     " VehicleId, VehicleName, LicensePlate, TimeZone, " +
                     " StartDate, EndDate, Distance, IdlingTime, FuelConsumption, PTOTime, Diagnostic " +
                     ") VALUES (" +
                     " @vehicleId, @vehicleName, @licensePlate, @timeZone, " +
                     " @startDate, @endDate, @distance, @idlingSeconds, @fuelConsumed, @ptoSeconds, @diagnostic" +
                     ")";
           

               cmd.Parameters.Add("@vehicleId", SqlDbType.Int);
               cmd.Parameters["@vehicleId"].Value = _vehicleId;
            
               cmd.Parameters.Add("@vehicleName", SqlDbType.NVarChar, 32);
               cmd.Parameters["@vehicleName"].Value = _vehicleName;

               cmd.Parameters.Add("@licensePlate", SqlDbType.NVarChar, 32);
               cmd.Parameters["@licensePlate"].Value = _licensePlate;

               cmd.Parameters.Add("@timeZone", SqlDbType.Int);
               cmd.Parameters["@timeZone"].Value = _timeZone;

               cmd.Parameters.Add("@startDate", SqlDbType.DateTime);
               cmd.Parameters["@startDate"].Value = dtIgnOn;

               cmd.Parameters.Add("@endDate", SqlDbType.DateTime);
               cmd.Parameters["@endDate"].Value = dtIgnOff;

               cmd.Parameters.Add("@distance", SqlDbType.Int);
               cmd.Parameters["@distance"].Value = distanceinTrip;

               cmd.Parameters.Add("@idlingSeconds", SqlDbType.Int);
               cmd.Parameters["@idlingSeconds"].Value = idlingPerTrip;

               cmd.Parameters.Add("@fuelConsumed", SqlDbType.Int);
               cmd.Parameters["@fuelConsumed"].Value = fuelReading;

               cmd.Parameters.Add("@ptoSeconds", SqlDbType.Int);
               cmd.Parameters["@ptoSeconds"].Value = ptoPerTrip;

               cmd.Parameters.Add("@diagnostic", SqlDbType.Int);
               cmd.Parameters["@diagnostic"].Value = tripDiagnostic;

               cmd.ExecuteNonQuery();
            }
         }

      }


   

      class MPredicate<T> where T : IComparable
      {
         T _type ;
         public MPredicate(T type)
         {
            _type = type ;
         }

         public bool Exists(T arg)
         {
            return arg.Equals(_type);
         }
      }

      public static bool HasSensorInfo(Enums.MessageType msgType)
      {
         return Array.Exists<Enums.MessageType>(hasSensorMask, (new MPredicate<Enums.MessageType>(msgType)).Exists); 
      }

      /// <summary>
      ///          this function should fill in the rows for Reports.DailyUtilizationReport
      //      Vehicle Id
      //      Vehicle’s Name
      //      License Plate
      //      TimeZone              The time zone of the vehicle
      //       --------------------- above only static information ------------------------------   
      //      DateTime              For which day of the year is the summary
      //      InServiceHours        The amount in seconds between the first Ignition ON and last Ignition OFF
      //      EngineOnHours         The sum of all trips (duration between Ignition ON, Ignition OFF) for that day
      //      Idling                The sum of all idle duration for that day
      //      PTOOnHours            The sum of all trips (duration between PTO ON, PTO OFF) for that day
      //      TravelledDistance     The sum of all distance trips for that day
      //      FuelConsumption       The amount of fuel consumption for the day (if available)
      //      TypeOfFuel            Description of fuel type
      //      HasPTO                If the vehicle has PTO
      //      ViolationsCounter     The number of violations messages for that day but not speed violations
      //      ViolationsDescription Is a string describing for Harsh Acceleration=60/Harsh Braking=58/Extreme Acceleration=61/Extreme Braking=59/SeatBelt=62
      // 
      //       AND
      //     fill in the rows for a TripSummaryReport  
      /// </summary>
      /// <param name="firstDateTime_"></param>
      /// <param name="lastDateTime_"></param>
      /// <param name="rowData">
      ///         contains
      ///            boxId, OriginDateTime, BoxMsgInTypeId, SensorMask, CustomProp,
      /// </param>
      public void FillVehicleSummary(DataSet rowData,           //<information about every row with all data from MsgIn  
                                     DateTime dtFrom,           // used in cross-day cases
                                     DateTime dtTo)
      {

         DateTime dtFirstIgnOn = DateTime.Now,        // the FIRST
                  dtLastIgnOff = DateTime.Now,        // the LAST
                  dtIgnOn = DateTime.Now, dtIgnOff = DateTime.Now,    // for the trip
                  dtPTOOn = DateTime.Now, dtPTOOff = DateTime.Now,
                  currentDateTime;

         int engineOnSeconds = 0,      // between IGN_ON .. IGN_OFF
             idlingSeconds = 0,        // just add all the numbers
             ptoONSeconds = 0,         // between PTO_ON .. PTO_OFF
             ptoONSecondsTrip = 0, 
             distanceinTrip = 0,       // distance between IGN_ON, IGN_OFF
             prevDistanceinTrip = 0,
             distancePerDay = 0,       // sum of distanceTrip          
             fuelConsumed = 0;

         int currentOdometer = -1,         // 
             beginningOdometer = -1,        // at the beginning of the trip
             idlingPerTrip = 0,
             prevIdlingPerTrip = 0,
             fuelReading = 0,
             prevFuelReading = 0,
             tripDiagnostic = 0;


         long currentSensorMask = 0L ;
         bool isIgnitionOn = false, 
              isPTOOn = false, 
              isFirstIgnitionOn = false,
              continueReading = false;

         // check arguments
         if (!Util.IsTable(rowData))
         {
            Util.BTrace(Util.WARN1, " << VehicleSummaryReport.FillVehicleSummary -> empty/wrong arguments");
            return;
         }

         // delete the previous data report 
         if (tblVehicleSummary.Rows.Count > 0)
            tblVehicleSummary.Rows.Clear();

         foreach (DataRow ittr in rowData.Tables[0].Rows)
         {
//            Util.BTrace(Util.INF0, Util.DumpRow(ittr, false));

            Enums.MessageType msgType = (Enums.MessageType)Util.Field2Int16(ittr, "BoxMsgInTypeId", 0);
            if (!HasSensorInfo(msgType))
               continue;

            currentSensorMask = Convert.ToInt64(ittr["SensorMask"]);
            currentDateTime = Convert.ToDateTime(ittr["OriginDateTime"]);

            string customProp = "";

            Util.Field2String(ittr, "CustomProp", ref customProp, "");

            if ((CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp) != ""))
            {
               try
               {
                  currentOdometer = Convert.ToInt32(CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp));
               }
               catch
               {
                  currentOdometer = -1;
               }
            }

            if (-1 == currentOdometer)
            {
               // store the current GPS position and calculate the distance trip based on latitude/longitude
            }

            #region handle PTO
            if ((currentSensorMask & PTO_ON) != 0)
            {
               if (!isPTOOn)
               {
                  // you have a new trip
                  isPTOOn = true;
                  dtPTOOn = currentDateTime;
               }
            }
            else
            {
               if (isPTOOn)
               {
                  dtPTOOff = currentDateTime;

                  // add all numbers to the fields -> engineOnHours, IdlingHours, travelledDistance, FuelConsumed
                  ptoONSeconds += (int)(new TimeSpan(dtPTOOff.Ticks - dtPTOOn.Ticks)).TotalSeconds;
                  if (isIgnitionOn)
                     ptoONSecondsTrip += (int)(new TimeSpan(dtPTOOff.Ticks - dtPTOOn.Ticks)).TotalSeconds;

                  Util.BTrace(Util.INF0, "PTO trip -> from={0} to={1} duration={2}",
                                    dtPTOOn, dtPTOOff, (int)(new TimeSpan(dtPTOOff.Ticks - dtPTOOn.Ticks)).TotalSeconds);

                  // reset all values for the trip
                  isPTOOn = false;
               }
            }
            #endregion handle PTO

            // this should be the same as ignition on
            if ((currentSensorMask & IGNTION_ON) != 0)
            {
               if (continueReading)       /// you were waiting on IGN OFF which didn't arrive
               {
                  continueReading = false;
                  // save the previous trip parameters with the diagnostic
                  Util.BTrace(Util.INF0, "Trip OFF-> VID={6} from={0} to={1} distance={2} fuel={3} idling={4} diag={5}",
                                             dtIgnOn, dtIgnOff, prevDistanceinTrip, prevFuelReading, prevIdlingPerTrip, DIAG_NO_SENSOR_OFF, _vehicleId);

                  tripDiagnostic = 0;
               }

               // compute idling per trip 
               if (msgType == Enums.MessageType.Idling /* || msgType == Enums.MessageType.ExtendedIdling */)
               {
                  if ((CLS.Util.PairFindValue(CLS.Def.Const.keyIdleDuration, customProp) != ""))
                  {
                     idlingPerTrip += Convert.ToInt32(CLS.Util.PairFindValue(CLS.Def.Const.keyIdleDuration, customProp));
                  }
               }

               if (isIgnitionOn)  // in the middle of the trip
               {
                  // or you analyze the PTO sensor

                  // add the segment of the trip if the odometer is not reported 
                  continue;
               }
               else
               {
                   // if it is the first packet with ignitionOn
                  if (isFirstIgnitionOn == false)
                  {
                     dtFirstIgnOn = currentDateTime;
                     isFirstIgnitionOn = true;
                  }
                  else
                  {
                     // save the parameters for the previous trip
                     AddTrip(dtIgnOn, dtIgnOff, prevDistanceinTrip, prevFuelReading, prevIdlingPerTrip, ptoONSecondsTrip, tripDiagnostic);
                     prevDistanceinTrip = prevFuelReading = prevIdlingPerTrip = 0;
                  }

                  // you have a new trip
                  ptoONSecondsTrip = 0;
                  isIgnitionOn = true;
                  beginningOdometer = currentOdometer;
                  dtIgnOn = currentDateTime;
                  tripDiagnostic = 0; 
               }

            }
            else
            {
               dtLastIgnOff = currentDateTime;

               // compute idling per trip 
               // this covers the case when idling is coming with sensor off
               if (msgType == Enums.MessageType.Idling /* || msgType == Enums.MessageType.ExtendedIdling */)
               {
                  if ((CLS.Util.PairFindValue(CLS.Def.Const.keyIdleDuration, customProp) != ""))
                  {
                     idlingPerTrip += Convert.ToInt32(CLS.Util.PairFindValue(CLS.Def.Const.keyIdleDuration, customProp));
                  }
               }

               // ignition is off, if you have information just save it in the table
               if (isIgnitionOn)
               {
                  dtIgnOff = currentDateTime;

                  // add all numbers to the fields -> engineOnHours, IdlingHours, travelledDistance, FuelConsumed
                  engineOnSeconds += (int)(new TimeSpan(dtIgnOff.Ticks - dtIgnOn.Ticks)).TotalSeconds;
                  if (-1 != beginningOdometer)
                  {
                     distanceinTrip += (currentOdometer - beginningOdometer);
                     distancePerDay += distanceinTrip;
                  }

                  if (msgType == Enums.MessageType.Sensor ||
                      msgType == Enums.MessageType.SensorExtended)
                  {
                     try
                     {
                        fuelReading = Convert.ToInt32(CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp));
                        fuelConsumed += fuelReading;
                     }
                     catch (Exception exc)
                     {
                        fuelReading = 0;
                     }
                  }
                  else
                  {
                     continueReading = true;       ///< indicates that IGN OFF didn't arrive yet 
                     tripDiagnostic = DIAG_NO_SENSOR_OFF;                                                  
                  }

                  idlingSeconds += idlingPerTrip;

                  // I am not expecting Ignition OFF before the packet SENSOR OFF is coming
                  // or after SENSOR OFF
                  prevDistanceinTrip = distanceinTrip;
                  prevFuelReading = fuelReading;
                  prevIdlingPerTrip = idlingPerTrip;
                  Util.BTrace(Util.INF0, "Trip1 -> VID={6} from={0} to={1} distance={2} fuel={3} idling={4} diag={5}",
                                    dtIgnOn, dtIgnOff, distanceinTrip, fuelReading, idlingPerTrip, tripDiagnostic, _vehicleId);
                  
//                  AddFuelTrip(_vehicleId, dtIgnOn, dtIgnOff, distanceinTrip, fuelReading, idlingPerTrip, tripDiagnostic);

                  // reset all values for the trip
                  distanceinTrip = 0;
                  fuelReading = 0;
                  idlingPerTrip = 0;
                  isIgnitionOn = false;

               }
               else
               {

                  // it could be the case on crosing the day and receiving an OFF packet
                  if (isFirstIgnitionOn == false)
                  {
                     Util.BTrace(Util.INF0, "Day starting with OFF ...");
/*
                     // I assume the time of the beginning I had sensor ON
                     dtFirstIgnOn = dtFrom;
                     dtIgnOff = currentDateTime;
                     isFirstIgnitionOn = true;

                     // add all numbers to the fields -> engineOnHours, IdlingHours, travelledDistance, FuelConsumed
                     engineOnSeconds += (int)(new TimeSpan(dtIgnOff.Ticks - dtIgnOn.Ticks)).TotalSeconds;

                     // I am not expecting Ignition OFF before the packet SENSOR OFF is coming
                     // or after SENSOR OFF
                     Util.BTrace(Util.INF0, "End Trip -> from={0} to={1} distance={2} fuel={3} idling={4}",
                                       dtIgnOn, dtIgnOff, distanceinTrip, fuelReading, idlingPerTrip);

                     // reset all values for the trip
                     distanceinTrip = 0;
                     fuelReading = 0;
                     idlingPerTrip = 0;
                     isIgnitionOn = false;
*/
                  }

                  // you have to recognize when there are packets which are in the buffer
                  // coming with bit 3 OFF, but the last packet received should be IGN_OFF

                  if (msgType == Enums.MessageType.Sensor ||
                      msgType == Enums.MessageType.SensorExtended)
                  {
                     try
                     {
                        fuelReading = Convert.ToInt32(CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp));
                        fuelConsumed += fuelReading;

                        // here you should update the trip info
                     }
                     catch (Exception exc)
                     {
                        Util.BTrace(Util.WARN0, "No fuel information, EXC={0}", exc.Message);
                        fuelReading = 0;
                     }

                     if (continueReading)
                     {
                        continueReading = false;
                        // now you can save the trip information, after fuelreading
                        tripDiagnostic = 0;
                        dtIgnOff = currentDateTime ;
                        Util.BTrace(Util.WARN0, "VID ={0} Sensor OFF received", _vehicleId);                        
                     }

                     Util.BTrace(Util.INF0, "SHOULD have saved the trip");                        
//                   AddTrip(_vehicleId, dtIgnOn, dtIgnOff, distanceinTrip, fuelReading, idlingPerTrip, tripDiagnostic);

                  }
                  else
                  {
                     Util.BTrace(Util.WARN0, " - VehicleSummaryReport.FillVehicleSummary -> VID={1} messages after IGN_OFF={0}",
                        Util.DumpRow(ittr, false), _vehicleId);
                  }
               }

            }

         } // end foreach

         // this is the case when IGN_OFF is not received 
         if (isIgnitionOn)
         {
            dtLastIgnOff = dtIgnOff = dtTo.AddHours(-_timeZone - _dayLightSaving);
            // add all numbers to the fields -> engineOnHours, IdlingHours, travelledDistance, FuelConsumed
            engineOnSeconds += (int)(new TimeSpan(dtIgnOff.Ticks - dtIgnOn.Ticks)).TotalSeconds;

            if (-1 != beginningOdometer)
            {
               distanceinTrip += (currentOdometer - beginningOdometer);
               distancePerDay += distanceinTrip;
            }

            idlingSeconds += idlingPerTrip;
            // I am not expecting Ignition OFF before the packet SENSOR OFF is coming
            // or after SENSOR OFF
            Util.BTrace(Util.INF0, "BEG Trip -> VID={5} from={0} to={1} distance={2} fuel={3} idling={4}",
                              dtIgnOn, dtIgnOff, distanceinTrip, fuelReading, idlingPerTrip, _vehicleId);

            AddTrip(dtIgnOn, dtIgnOff, distanceinTrip, 0, idlingSeconds, ptoONSecondsTrip, TRIP_NOT_FINISHED);      // the trip is continued next day

         }
         else
         {
            if (prevDistanceinTrip != 0)
            {
               AddTrip(dtIgnOn, dtIgnOff, prevDistanceinTrip, prevFuelReading, prevIdlingPerTrip, ptoONSecondsTrip, tripDiagnostic);
            }
         }

         if (isFirstIgnitionOn)        // you had at least one ignition on
         {
            // here you close the record for the whole period, saving the record in the table
            // the procedure is called every 24 hours adjusted for the timezone of the vehicle
            Util.BTrace(Util.INF0, "Period stats -> VID={6} from={0} to={1} distance={2} fuel={3} idling={4} PTO={5}",
                                     dtFirstIgnOn, dtLastIgnOff, distancePerDay, fuelConsumed, idlingSeconds, ptoONSeconds, _vehicleId);

            AddRowPerDayForUtlization(dtFirstIgnOn, dtLastIgnOff, engineOnSeconds, idlingSeconds, ptoONSeconds, distancePerDay, fuelConsumed);
         }
         else
         {
            Util.BTrace(Util.INF0, "VID={2} NO activity between {0} AND {1}", dtFrom.AddHours(-_timeZone - _dayLightSaving), dtTo.AddHours(-_timeZone - _dayLightSaving), _vehicleId);
         }
      }

   }
}
