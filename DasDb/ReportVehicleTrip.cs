using System;
using System.Data;
using VLF.CLS;
using VLF.CLS.Def;
using VLF.MAP;
using VLF.ERR;

namespace VLF.DAS.DB
{
   /// <summary>
   /// rovides interfaces to all reports in the system
   /// </summary>
   public class ReportVehicleTrip
   {
      // Properties
      private DataTable tblTripReportData = null;
      private DataTable tblTripDuration = null;
      private DataTable tblTripStopsDuration = null;
      private DataTable tblTripDistance = null;
      private DataTable tblTripAverageSpeed = null;
      private DataTable tblTripCost = null;
      private DataTable tblTripStart = null;
      private DataTable tblTripEnd = null;
      private DataTable tblTripFuelCons = null;
      private int boxId = VLF.CLS.Def.Const.unassignedIntValue;

      private Int64 vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
      private bool tripStarted = false;
      private string streetAddress = "";
      private double measurementUnits = 1;
      private DateTime currRowDateTime = VLF.CLS.Def.Const.unassignedDateTime;

      // Calculates average speed
      private double currAvgSpeed = 0;
      //private Int64 currSpeedSum = 0;

      // Trip stop variables
      private short idlingRange = 0;
      private DateTime tripStopTime = VLF.CLS.Def.Const.unassignedDateTime;
      private int tripNumber = 0;
      private TimeSpan currTripStopsDuration = new TimeSpan(0, 0, 0);

      // Statistics variables
      private DateTime currTripStarted = VLF.CLS.Def.Const.unassignedDateTime;
      private DateTime currTripPrevIdleTime = VLF.CLS.Def.Const.unassignedDateTime;
      private string lastStoredPosition = "";
      VLF.MAP.GeoPoint firstStoppedCoord = new VLF.MAP.GeoPoint();
      private double currTripDistance = 0;

//      private VLF.MAP.ClientMapProxy map = null;

      private DataTable tblVehicleSensors = null;
      private DataTable tblLandmarks = null;
      // Distance calculation variables
      VLF.MAP.GeoPoint prevCoord = null;
      VLF.MAP.GeoPoint currCoord = null;
      private int userId = VLF.CLS.Def.Const.unassignedIntValue;
      private SQLExecuter sqlExec = null;
      private bool isLandmark = false;


      private int prevBoxId = 0;
      private bool idleStart = false;


      /// <summary>
      /// Constructor
      /// </summary>
      public ReportVehicleTrip(short idlingRange, SQLExecuter sqlExec)
      {
         this.sqlExec = sqlExec;
         this.idlingRange = idlingRange;

         // Distance calculation variables
         prevCoord = new VLF.MAP.GeoPoint();
         currCoord = new VLF.MAP.GeoPoint();
//         map = null;

         #region TripReportData table defenition
         tblTripReportData = new DataTable("TripReportData");
         tblTripReportData.Columns.Add("TripIndex", typeof(string));
         tblTripReportData.Columns.Add("Reason", typeof(string));
         tblTripReportData.Columns.Add("Date/Time", typeof(string));
         tblTripReportData.Columns.Add("Location", typeof(string));
         tblTripReportData.Columns.Add("Speed", typeof(string));
         tblTripReportData.Columns.Add("Description", typeof(string));
         tblTripReportData.Columns.Add("BoxId", typeof(string));
         tblTripReportData.Columns.Add("Latitude", typeof(double));
         tblTripReportData.Columns.Add("Longitude", typeof(double));
         tblTripReportData.Columns.Add("Remarks", typeof(string));
         tblTripReportData.Columns.Add("VehicleId", typeof(string));
         tblTripReportData.Columns.Add("IsLandmark", typeof(bool));
         #endregion

         #region TripDuration table defenition
         tblTripDuration = new DataTable("TripDuration");
         tblTripDuration.Columns.Add("TripIndex", typeof(string));
         tblTripDuration.Columns.Add("Summary", typeof(string));
         tblTripDuration.Columns.Add("Remarks", typeof(string));
         tblTripDuration.Columns.Add("BoxId", typeof(string));
         tblTripDuration.Columns.Add("Latitude", typeof(double));
         tblTripDuration.Columns.Add("Longitude", typeof(double));
         tblTripDuration.Columns.Add("VehicleId", typeof(string));
         tblTripDuration.Columns.Add("IsLandmark", typeof(bool));
         #endregion

         #region TripStopsDuration table defenition
         tblTripStopsDuration = new DataTable("TripStopsDuration");
         tblTripStopsDuration.Columns.Add("TripIndex", typeof(string));
         tblTripStopsDuration.Columns.Add("Summary", typeof(string));
         tblTripStopsDuration.Columns.Add("Remarks", typeof(string));
         tblTripStopsDuration.Columns.Add("BoxId", typeof(string));
         tblTripStopsDuration.Columns.Add("Latitude", typeof(double));
         tblTripStopsDuration.Columns.Add("Longitude", typeof(double));
         tblTripStopsDuration.Columns.Add("VehicleId", typeof(string));
         tblTripStopsDuration.Columns.Add("IsLandmark", typeof(bool));
         #endregion

         #region TripDistance  table defenition
         tblTripDistance = new DataTable("TripDistance");
         tblTripDistance.Columns.Add("TripIndex", typeof(string));
         tblTripDistance.Columns.Add("Summary", typeof(string));
         tblTripDistance.Columns.Add("Remarks", typeof(string));
         tblTripDistance.Columns.Add("BoxId", typeof(string));
         tblTripDistance.Columns.Add("Latitude", typeof(double));
         tblTripDistance.Columns.Add("Longitude", typeof(double));
         tblTripDistance.Columns.Add("VehicleId", typeof(string));
         tblTripDistance.Columns.Add("IsLandmark", typeof(bool));
         #endregion

         #region TripAverageSpeed  table defenition
         tblTripAverageSpeed = new DataTable("TripAverageSpeed");
         tblTripAverageSpeed.Columns.Add("TripIndex", typeof(string));
         tblTripAverageSpeed.Columns.Add("Summary", typeof(string));
         tblTripAverageSpeed.Columns.Add("Remarks", typeof(string));
         tblTripAverageSpeed.Columns.Add("BoxId", typeof(string));
         tblTripAverageSpeed.Columns.Add("Latitude", typeof(double));
         tblTripAverageSpeed.Columns.Add("Longitude", typeof(double));
         tblTripAverageSpeed.Columns.Add("VehicleId", typeof(string));
         tblTripAverageSpeed.Columns.Add("IsLandmark", typeof(bool));
         #endregion

         #region TripCost  table defenition
         tblTripCost = new DataTable("TripCost");
         tblTripCost.Columns.Add("TripIndex", typeof(string));
         tblTripCost.Columns.Add("Summary", typeof(string));
         tblTripCost.Columns.Add("Remarks", typeof(string));
         tblTripCost.Columns.Add("BoxId", typeof(string));
         tblTripCost.Columns.Add("Latitude", typeof(double));
         tblTripCost.Columns.Add("Longitude", typeof(double));
         tblTripCost.Columns.Add("VehicleId", typeof(string));
         tblTripCost.Columns.Add("IsLandmark", typeof(bool));
         #endregion

         #region TripStart table defenition
         tblTripStart = new DataTable("TripStart");
         tblTripStart.Columns.Add("TripIndex", typeof(string));
         tblTripStart.Columns.Add("Summary", typeof(string));
         tblTripStart.Columns.Add("Remarks", typeof(string));
         tblTripStart.Columns.Add("BoxId", typeof(string));
         tblTripStart.Columns.Add("Latitude", typeof(double));
         tblTripStart.Columns.Add("Longitude", typeof(double));
         tblTripStart.Columns.Add("VehicleId", typeof(string));
         tblTripStart.Columns.Add("IsLandmark", typeof(bool));
         #endregion

         #region TripEnd table defenition
         tblTripEnd = new DataTable("TripEnd");
         tblTripEnd.Columns.Add("TripIndex", typeof(string));
         tblTripEnd.Columns.Add("Summary", typeof(string));
         tblTripEnd.Columns.Add("Remarks", typeof(string));
         tblTripEnd.Columns.Add("BoxId", typeof(string));
         tblTripEnd.Columns.Add("Latitude", typeof(double));
         tblTripEnd.Columns.Add("Longitude", typeof(double));
         tblTripEnd.Columns.Add("VehicleId", typeof(string));
         tblTripEnd.Columns.Add("IsLandmark", typeof(bool));
         #endregion

         #region TripFuelCons table defenition
         tblTripFuelCons  = new DataTable("TripFuelCons");
         tblTripFuelCons.Columns.Add("TripIndex", typeof(string));
         tblTripFuelCons.Columns.Add("Summary", typeof(string));
         tblTripFuelCons.Columns.Add("Remarks", typeof(string));
         tblTripFuelCons.Columns.Add("BoxId", typeof(string));
         tblTripFuelCons.Columns.Add("Latitude", typeof(double));
         tblTripFuelCons.Columns.Add("Longitude", typeof(double));
         tblTripFuelCons.Columns.Add("VehicleId", typeof(string));
         tblTripFuelCons.Columns.Add("IsLandmark", typeof(bool));
         #endregion

      }
      /// <summary>
      /// Fill trip report
      /// </summary>
      /// <remarks>
      /// 1. On Ignition On satrt trip
      /// 2. On Ignition Off stop trip
      /// 3. If Street address is selected, include it into report
      /// 4. Shows sensors changes
      /// 5. Shows vehicle stops (Stop interval is configurable)
      /// 6. Calculates trip statistics:
      ///		6.1 Trip Duration
      ///		6.2 Trip Distance
      ///		6.3 Trip Average Speed
      ///		6.4 Trip Stops
      ///		6.5 Trip Cost
      ///		
      /// 7. Calculates all trips statistics:
      ///		7.1 Total Trips
      ///		7.2 Total Distance (mile/kms)
      ///		7.3 Total Trips Duration
      ///		7.4 Total Average Speed
      ///		7.5 Total Cost
      ///	8. isTrailer == true, then calculate trips by "Power" message
      /// </remarks>
      /// <param name="tblVehicleSensors"></param>
      /// <param name="tblVehicleGeozones"></param>
      /// <param name="includeStreetAddress"></param>
      /// <param name="includeSensors"></param>
      /// <param name="includePosition"></param>
      /// <param name="includeIdleTime"></param>
      /// <param name="showLastStoredPosition"></param>
      /// <param name="carCost"></param>
      /// <param name="rowData"></param>
      /// <param name="measurementUnits"></param>
      /// <param name="vehicleId"></param>
      /// <param name="userId"></param>
      /// <param name="isTrailer"></param>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
   

#if NEW_REPORTS
      /** \brief     This function bases all cycles on the changes of sensorId from the param list 
       *  \comment   implemented for Chilliwack
       */
      public void FillReport(DataTable tblVehicleSensors,
                             DataTable tblVehicleGeozones,
                             bool includeStreetAddress,
                             bool includeSensors,
                             bool includePosition,
                             bool includeIdleTime,
                             bool showLastStoredPosition,
                             double carCost,
                             DataSet rowData,
                             double measurementUnits,
                             Int64 vehicleId,
                             int userId,
                             int sensorId, 
                             short vehicleType,
                             string lang)
      {

         //localization settings
         Resources.Const.Culture = new System.Globalization.CultureInfo(lang);


         TimeSpan weekdayStartTime = new TimeSpan(0, 0, 0, 0);
         TimeSpan weekdayEndTime = new TimeSpan(0, 23, 59, 59, 999);
         TimeSpan weekendStartTime = new TimeSpan(0, 0, 0, 0);
         TimeSpan weekendEndTime = new TimeSpan(0, 23, 59, 59, 999);
         try
         {
            this.userId = userId;
            this.vehicleId = vehicleId;
            this.tblVehicleSensors = tblVehicleSensors;
            this.measurementUnits = measurementUnits;
            if (rowData == null || rowData.Tables.Count == 0)
               return; // do not process empty data
            
            #region Local Variables
            Enums.MessageType msgType;
            string customProp = "";
            string description = "";
            bool prevIgnitionOn = false;
            bool currIgnitionOn = false;
            string sensorStatus = "";
            string strCmfCurrentOdo = "";
            int ignOnOdoReading = 0;
            int ignOffOdoReading = 0;
            int fuelReading = 0;
            bool startTripOnFlag = false;
            DayOfWeek currTripDayOfWeek = DayOfWeek.Sunday;
            string msgTypeLang = "";
            string addDescription = "";
            #endregion


            Localization local = new Localization(sqlExec.ConnectionString);
            DataSet dsMessageTypes = new DataSet(); 

      
            if (lang != "en")
            {
               dsMessageTypes = local.GetGuiLocalization("MessageType", lang);
               Box boxCnf = new DB.Box(sqlExec);

               VehicleInfo vehicle = new VehicleInfo(sqlExec);
               DataSet dsVehicle = vehicle.GetVehicleInfoByVehicleId(vehicleId);
  
               Int16 hwTypeId = 0;
               DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(Convert.ToInt32(dsVehicle.Tables[0].Rows[0]["BoxId"])  );
               if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
               {
                  // TODO: Today box has only one HW type
                  hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);
               }
               else
               {
                  // TODO: write to log (cannot retrieve HW type for specific box)
               }

               DataSet dsVehicleSensors = new DataSet();
               dsVehicleSensors.Tables.Add(tblVehicleSensors);
               local.LocalizationData(lang, "SensorId", "SensorName", "Sensors",hwTypeId, ref dsVehicleSensors);
               local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
               this.tblVehicleSensors = dsVehicleSensors.Tables[0]; 
            }

/*
            #region Create map engine instance
            DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
            DataSet dsGeoCodeInfo = mapEngine.GetUserGeoCodeEngineInfo(userId);
            if (dsGeoCodeInfo != null && dsGeoCodeInfo.Tables.Count > 0 && dsGeoCodeInfo.Tables[0].Rows.Count > 0)
               map = new VLF.MAP.ClientMapProxy(MapUtilities.ConvertGeoCodersToMapEngine(dsGeoCodeInfo));
            else
               throw new DASDbConnectionClosed("Unable to retrieve map engine info by user=" + userId);
            #endregion
*/
            rowData.Tables[0].DefaultView.Sort = "BoxId,OriginDateTime ASC, BoxMsgInTypeId DESC ";

            foreach (DataRow ittr in rowData.Tables[0].Rows)
            {
               #region Retrieves general information
               boxId = Convert.ToInt32(ittr["BoxId"]);
               msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);
               currRowDateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
               isLandmark = false;
               addDescription = "";

               //Localization 
               if (lang != "en")
               {
                  foreach (DataRow dr in dsMessageTypes.Tables[0].Rows)
                  {
                     if (Convert.ToInt16(dr["FieldID"]) == Convert.ToInt16(ittr["BoxMsgInTypeId"]))
                     {
                        msgTypeLang = dr["LocalName"].ToString() ;
                        break;
                     }

                  }
               }
               else
                  msgTypeLang = msgType.ToString();


               #region Save last known Stored Position
               if (includeStreetAddress && msgType == Enums.MessageType.StoredPosition)
                  lastStoredPosition = ittr["StreetAddress"].ToString().TrimEnd();
               #endregion

               #region Process Invalid GPS
               // process message with invalid GPS
               if (Convert.ToInt16(ittr["ValidGps"]) == 1)
               {
                  // Set default 
                  ittr["Latitude"] = 0;
                  ittr["Longitude"] = 0;
                  ittr["Speed"] = VLF.CLS.Def.Const.unassignedIntValue;
                  ittr["Heading"] = VLF.CLS.Def.Const.blankValue;
                  if (includeStreetAddress)
                  {
                     if (showLastStoredPosition && lastStoredPosition != "")
                        streetAddress = VLF.CLS.Def.Const.noValidAddress +
                                        Convert.ToChar(13) + Convert.ToChar(10) +
                                        "[" + lastStoredPosition + "]";
                     else
                        streetAddress = VLF.CLS.Def.Const.noValidAddress;
                  }
               }
               else
               {
                  if (ittr["NearestLandmark"].ToString() != "" && ittr["NearestLandmark"].ToString().StartsWith(VLF.CLS.Def.Const.addressNA) == false)
                     isLandmark = true;

                  if (includeStreetAddress)
                  {
                     streetAddress = (msgType == Enums.MessageType.StoredPosition) ? lastStoredPosition :
                                             ittr["StreetAddress"].ToString().TrimEnd();
                     if (streetAddress == "")
                        streetAddress = VLF.CLS.Def.Const.noGPSData;
                  }
               }
               #endregion

               #region Retrieve sensor detailes
               if (currIgnitionOn && (msgType == Enums.MessageType.Sensor           || 
                                      msgType == Enums.MessageType.SensorExtended   ||
                                      msgType == Enums.MessageType.Alarm))
               {
                  // Add sensor info into result dataset
                  try
                  {
                     customProp = ittr["CustomProp"].ToString().TrimEnd();
                  }
                  catch
                  {
                     customProp = "";
                  }



                  sensorStatus = DB.Sensor.GetSensorDescription(customProp, tblVehicleSensors);
                  AddTripCondition(addDescription,currIgnitionOn, sensorId, startTripOnFlag,ref sensorStatus); 
               }
               else if (msgType != Enums.MessageType.Sensor          || 
                        msgType != Enums.MessageType.SensorExtended  || 
                        msgType != Enums.MessageType.Alarm)
               {
                   sensorStatus = TripCondition(currIgnitionOn, sensorId, startTripOnFlag);
               }

               #endregion
               #endregion

               #region Verifies working hours
               TimeSpan currRowTime = new TimeSpan(0, currRowDateTime.Hour, currRowDateTime.Minute, currRowDateTime.Second, currRowDateTime.Millisecond);

               if (
                   ((currRowDateTime.DayOfWeek == DayOfWeek.Sunday || currRowDateTime.DayOfWeek == DayOfWeek.Saturday) && (currRowTime < weekendStartTime || currRowTime > weekendEndTime)) || // not IN weekend working hours
                   ((currRowDateTime.DayOfWeek > DayOfWeek.Sunday || currRowDateTime.DayOfWeek < DayOfWeek.Saturday) && (currRowTime < weekdayStartTime || currRowTime > weekdayEndTime)) // not IN weekday working hours
                   )
               {
                  if (currIgnitionOn)
                  {
                     #region Close Trip
                     #region Retrieves Odometer Reading
                     strCmfCurrentOdo = "";
                     if ((CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp) != ""))
                     {
                        strCmfCurrentOdo = CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp);
                        try
                        {
                           ignOffOdoReading = Convert.ToInt32(strCmfCurrentOdo);
                        }
                        catch
                        {
                        }
                     }
                      #endregion
                     #region Retrive Fuel Consumption
                     fuelReading = 0;
                     if ((CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp) != ""))
                     {
                         try
                         {
                             fuelReading = Convert.ToInt32(CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp));
                         }
                         catch
                         {
                         }
                     }
                     #endregion
                     #region Add message info into trip
                     #region Prepare workDateTime
                     DateTime workDateTime = currRowDateTime.Date;
                     if (currRowDateTime.DayOfWeek == DayOfWeek.Sunday || currRowDateTime.DayOfWeek == DayOfWeek.Saturday)
                     {
                        if (currRowTime < weekendStartTime)
                           workDateTime = workDateTime.AddHours(weekendStartTime.Hours).AddMinutes(weekendStartTime.Minutes).AddSeconds(weekendStartTime.Seconds).AddMilliseconds(weekendStartTime.Milliseconds);
                        else
                           workDateTime = workDateTime.AddHours(weekendEndTime.Hours).AddMinutes(weekendEndTime.Minutes).AddSeconds(weekendEndTime.Seconds).AddMilliseconds(weekendEndTime.Milliseconds);
                     }
                     else
                     {
                        if (currRowTime < weekdayStartTime)
                           workDateTime = workDateTime.AddHours(weekdayStartTime.Hours).AddMinutes(weekdayStartTime.Minutes).AddSeconds(weekdayStartTime.Seconds).AddMilliseconds(weekdayStartTime.Milliseconds);
                        else
                           workDateTime = workDateTime.AddHours(weekdayEndTime.Hours).AddMinutes(weekdayEndTime.Minutes).AddSeconds(weekdayEndTime.Seconds).AddMilliseconds(weekdayEndTime.Milliseconds);
                     }
                     currRowDateTime = workDateTime;
      #endregion
                     // LOCALIZATION
                     AddTripReportRow(tripNumber,
                                     msgTypeLang,
                                     currRowDateTime.ToString(),
                                     streetAddress,
                                     ittr["Speed"].ToString(),
                                     sensorStatus,
                                     Convert.ToDouble(ittr["Latitude"]),
                                     Convert.ToDouble(ittr["Longitude"]),
                                     "");

                     startTripOnFlag = true; 

                     #endregion
                     #region Close previous trip
                     CloseTrip(carCost,
                               Convert.ToDouble(ittr["Latitude"]),
                               Convert.ToDouble(ittr["Longitude"]),
                               Convert.ToInt32(ittr["Speed"]),
                               includeIdleTime,
                               Convert.ToInt16(ittr["ValidGps"]),fuelReading,
                               //GetExtraInfo(userId, "DrID",ittr["customProp"].ToString()),
                               GetExtraInfo(userId, "DrID", ittr["customProp"].ToString(), "", vehicleId, Convert.ToDateTime(ittr["GMT_OriginDateTime"])),
                               ref ignOffOdoReading, ref ignOnOdoReading);
                     #endregion
                     #endregion
                     prevIgnitionOn = currIgnitionOn = false;
                     continue;
                  }
               }
               else
               {
                   if ((msgType != VLF.CLS.Def.Enums.MessageType.MBAlarm) && (msgType != VLF.CLS.Def.Enums.MessageType.GeoZone) && (msgType != VLF.CLS.Def.Enums.MessageType.Idling))// this message does not include sens
                  {
                     if (sensorId != 11) //Tractor Power
                     {
                        currIgnitionOn = ((Convert.ToInt64(ittr["SensorMask"]) & (1 << (sensorId - 1))) == 0) ? false : true;
                        AddTripCondition(addDescription, currIgnitionOn, sensorId, startTripOnFlag, ref sensorStatus);
                     }
                     else
                     {
                        if (msgType != Enums.MessageType.Status)
                        {
                           currIgnitionOn = ((Convert.ToInt64(ittr["SensorMask"]) & (1 << (sensorId - 1))) == 0) ? false : true;
                           AddTripCondition(addDescription, currIgnitionOn, sensorId, startTripOnFlag, ref sensorStatus);
                        }
                     }

                   
                  }
               }
               #endregion

      #region Trip status have changed
               if (prevIgnitionOn != currIgnitionOn)
               {
      #region Retrieve sensor detailes
                  if (currIgnitionOn && (msgType == Enums.MessageType.Sensor         ||
                                         msgType == Enums.MessageType.SensorExtended || 
                                         msgType == Enums.MessageType.Alarm))
                  {
                     // Add sensor info into result dataset
                     try
                     {
                        customProp = ittr["CustomProp"].ToString().TrimEnd();
                     }
                     catch
                     {
                        customProp = "";
                     }

                     sensorStatus = DB.Sensor.GetSensorDescription(customProp, tblVehicleSensors);
                     AddTripCondition(addDescription, currIgnitionOn, sensorId, startTripOnFlag, ref sensorStatus);
                  }
                  else 
                  {
                     try
                     {
                        customProp = ittr["CustomProp"].ToString().TrimEnd();
                     }
                     catch
                     {
                        customProp = "";
                     }
                  }
                 

      #endregion
      #region Trip has been ended
                  if (currIgnitionOn == false)
                  {
      #region Close Trip
      #region Retrieves Odometer Reading
                     strCmfCurrentOdo = "";
                     if ((CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp) != ""))
                     {
                        strCmfCurrentOdo = CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp);
                        sensorStatus = DB.Sensor.GetSensorDescription(customProp, tblVehicleSensors);
                        AddTripCondition(addDescription, currIgnitionOn, sensorId, startTripOnFlag, ref sensorStatus); 

                        try
                        {
                           ignOffOdoReading = Convert.ToInt32(strCmfCurrentOdo);
                        }
                        catch
                        {
                        }
                     }

          
      #endregion
      #region Retrive Fuel Consumption
                     fuelReading = 0;
                     if ((CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp) != ""))
                     {
                         try
                         {
                             fuelReading = Convert.ToInt32(CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp));
                         }
                         catch
                         {
                         }
                     }
      #endregion
      #region Add message info into trip



                     if (msgType==Enums.MessageType.Idling)
                        ProcessIdling(Convert.ToInt32(ittr["boxId"]),Convert.ToInt32(Util.PairFindValue(VLF.CLS.Def.Const.keyIdleDuration, ittr["CustomProp"].ToString().TrimEnd())),
                                    includeIdleTime, includeStreetAddress,
                                    Convert.ToDouble(ittr["Latitude"]),
                                    Convert.ToDouble(ittr["Longitude"]),
                                    streetAddress,
                                    ittr["NearestLandmark"].ToString(), lang, addDescription);

                      else
                        AddTripReportRow(tripNumber,
                                     msgTypeLang,
                                     currRowDateTime.ToString(),
                                     streetAddress,
                                     ittr["Speed"].ToString(),
                                     sensorStatus,
                                     Convert.ToDouble(ittr["Latitude"]),
                                     Convert.ToDouble(ittr["Longitude"]),
                                     "");

                     startTripOnFlag = true;

      #endregion
      #region Close previous trip
                     CloseTrip(carCost,
                               Convert.ToDouble(ittr["Latitude"]),
                               Convert.ToDouble(ittr["Longitude"]),
                               Convert.ToInt32(ittr["Speed"]),
                               includeIdleTime,
                               Convert.ToInt16(ittr["ValidGps"]),fuelReading,
                                //GetExtraInfo(userId, "DrID",ittr["customProp"].ToString()),
                               GetExtraInfo(userId, "DrID", ittr["customProp"].ToString(), "", vehicleId, Convert.ToDateTime(ittr["GMT_OriginDateTime"])),
                               ref ignOffOdoReading, ref ignOnOdoReading);
      #endregion
      #endregion
                  }
      #endregion
      #region Trip has begun
                  else
                  {
      #region Close previous trip
                     if (prevIgnitionOn)
                     {
      #region Close Trip
      #region Retrieves Odometer Reading
                        strCmfCurrentOdo = "";
                        if ((CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp) != ""))
                        {
                           strCmfCurrentOdo = CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp);
                           try
                           {
                              ignOffOdoReading = Convert.ToInt32(strCmfCurrentOdo);
                           }
                           catch
                           {
                           }
                        }
      #endregion
      #region Retrive Fuel Consumption
                        fuelReading = 0;
                        if ((CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp) != ""))
                        {
                            try
                            {
                                fuelReading = Convert.ToInt32(CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp));
                            }
                            catch
                            {
                            }
                        }
      #endregion
      #region Add message info into trip
                        //AddTripReportRow(tripNumber,
                        //                msgTypeLang,
                        //                currRowDateTime.ToString(),
                        //                streetAddress,
                        //                ittr["Speed"].ToString(),
                        //                sensorStatus,
                        //                Convert.ToDouble(ittr["Latitude"]),
                        //                Convert.ToDouble(ittr["Longitude"]),
                        //                "");




                      //without Driver
                        AddTripReportRow(tripNumber,
                                        msgTypeLang,
                                        currRowDateTime.ToString(),
                                        streetAddress,
                                        ittr["Speed"].ToString(),
                                        sensorStatus,
                                        Convert.ToDouble(ittr["Latitude"]),
                                        Convert.ToDouble(ittr["Longitude"]),
                                        "");
      #endregion

                        startTripOnFlag = true;
      #endregion
      #region Close previous trip
                        CloseTrip(carCost,
                                  Convert.ToDouble(ittr["Latitude"]),
                                  Convert.ToDouble(ittr["Longitude"]),
                                  Convert.ToInt32(ittr["Speed"]),
                                  includeIdleTime,
                                  Convert.ToInt16(ittr["ValidGps"]),fuelReading,
                                    //GetExtraInfo(userId, "DrID",ittr["customProp"].ToString()),
                                  GetExtraInfo(userId, "DrID", ittr["customProp"].ToString(), "", vehicleId, Convert.ToDateTime(ittr["GMT_OriginDateTime"])),
                                  ref ignOffOdoReading, ref ignOnOdoReading);

                        
      #endregion
      #endregion
                     }
      #endregion

      #region Open new trip
      #region Retrieves Odometer reading
                     strCmfCurrentOdo = "";
                     if ((CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp) != ""))
                     {
                        strCmfCurrentOdo = CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp);
                        try
                        {
                           ignOnOdoReading = Convert.ToInt32(strCmfCurrentOdo);
                           ignOffOdoReading = ignOnOdoReading;
                        }
                        catch
                        {
                        }
                     }
      #endregion
      #region Add new trip
                     currTripDayOfWeek = currRowDateTime.DayOfWeek;
                     currTripStarted = currRowDateTime;
                     tripStopTime = VLF.CLS.Def.Const.unassignedDateTime;
                     // New trip started
                     currTripDistance = 0;
                     tripStarted = true;
                     // Open new trip
                     ++tripNumber;
                     startTripOnFlag = false;

                     AddTripSummaryRow(tripNumber,
                                     currRowDateTime.ToString(),
                                     streetAddress,
                                     tblTripStart,
                                     Convert.ToDouble(ittr["Latitude"]),
                                     Convert.ToDouble(ittr["Longitude"]));



                     AddTripReportRow(tripNumber, Resources.Const.TripStart, // "Trip Start"/*"Trip " + tripNumber.ToString() + " Start"*/,
                                     currRowDateTime.ToString(),
                                     streetAddress,
                                     ittr["Speed"].ToString(),
                                     "",
                                     Convert.ToDouble(ittr["Latitude"]),
                                     Convert.ToDouble(ittr["Longitude"]),
                                     "");



                  

                
      #endregion
      #endregion
                  }
      #endregion
                  prevIgnitionOn = currIgnitionOn;
               }
             

      #region Add additional info to the trip
               if (currIgnitionOn)
               {
                  if (Convert.ToInt16(ittr["ValidGps"]) == 0)
                     CalculateTripDistance(Convert.ToDouble(ittr["Latitude"]), Convert.ToDouble(ittr["Longitude"]));

                  if (includeStreetAddress && ittr["NearestLandmark"].ToString() != "" && ittr["NearestLandmark"].ToString().StartsWith(VLF.CLS.Def.Const.addressNA) == false)
                     isLandmark = true;

              

                  switch (msgType)
                  {
      #region Process idling message
                     case Enums.MessageType.Idling:


                        addDescription = TripCondition(currIgnitionOn, sensorId, startTripOnFlag);

                        ProcessIdling(Convert.ToInt32(ittr["boxId"]),Convert.ToInt32(Util.PairFindValue(VLF.CLS.Def.Const.keyIdleDuration, ittr["CustomProp"].ToString().TrimEnd())),
                                    includeIdleTime, includeStreetAddress,
                                    Convert.ToDouble(ittr["Latitude"]),
                                    Convert.ToDouble(ittr["Longitude"]),
                                    streetAddress,
                                    ittr["NearestLandmark"].ToString(), lang, addDescription);

                        startTripOnFlag = true;

                        break;
      #endregion
      #region Process position messages
                     case Enums.MessageType.Coordinate:
                     case Enums.MessageType.PositionUpdate:
                     case Enums.MessageType.GeoZone:
                     case Enums.MessageType.StoredPosition:
                        

                        if (includePosition)
                        {
                           string reason = Resources.Const.Position; //  "Position";

                           description = "";
                           // change reason according to msg type
                           if (msgType == Enums.MessageType.StoredPosition)
                              reason = Resources.Const.StoredPosition; // "Stored Position";
                           else if (msgType == Enums.MessageType.GeoZone)
                           {
                              reason = Resources.Const.GeoZone; //  "Geo Zone";
                              if (tblVehicleGeozones != null && tblVehicleGeozones.Rows.Count > 0)
                                 description = DB.VehicleGeozone.GetGeoZoneDescription(ittr["CustomProp"].ToString().TrimEnd(), tblVehicleGeozones);
                              else
                                 description = DB.VehicleGeozone.GetGeoZoneDescription(ittr["CustomProp"].ToString().TrimEnd(), null);

                               
                           }

                        
                           AddTripCondition(addDescription, currIgnitionOn, sensorId, startTripOnFlag, ref description); 

                              // Add sensor info into trip
                           AddTripReportRow(tripNumber,
                                           reason,
                                           currRowDateTime.ToString(),
                                           streetAddress,
                                           ittr["Speed"].ToString(),
                                           description,
                                           Convert.ToDouble(ittr["Latitude"]),
                                           Convert.ToDouble(ittr["Longitude"]),
                                          "");

                           startTripOnFlag = true;
                        }
                        break;
      #endregion
      #region Process sensor/alarm messages
                     case Enums.MessageType.Sensor:
                     case Enums.MessageType.SensorExtended:
                     case Enums.MessageType.Alarm:
                        if (includeSensors)
                        {
                           string reason = "";
                           if (msgType == Enums.MessageType.Sensor || msgType == Enums.MessageType.SensorExtended)
                              reason = Resources.Const.Sensor; // "Sensor";
                           if (msgType == Enums.MessageType.Alarm)
                              reason = Resources.Const.Alarm; // "Alarm";

                           AddTripReportRow(tripNumber,
                                            reason,
                                            currRowDateTime.ToString(),
                                            streetAddress,
                                            ittr["Speed"].ToString(),
                                            sensorStatus,
                                            Convert.ToDouble(ittr["Latitude"]),
                                            Convert.ToDouble(ittr["Longitude"]),
                                            "");

                           startTripOnFlag = true; 
                        }
                        break;
      #endregion
      #region Process speeding message
                     case Enums.MessageType.Speeding:
                     case Enums.MessageType.Speed:
                        description = "";
                        string duration = Util.PairFindValue(VLF.CLS.Def.Const.keySpeedDuration, ittr["CustomProp"].ToString().TrimEnd());
                        if (duration != "")
                        {
                           try
                           {
                              description = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                           }
                           catch
                           {
                           }
                        }

                        AddTripCondition(addDescription, currIgnitionOn, sensorId, startTripOnFlag, ref description); 


                        AddTripReportRow(tripNumber,
                            Resources.Const.Speeding, /*"Speeding", */
                            currRowDateTime.ToString(),
                            streetAddress,
                            ittr["Speed"].ToString(),
                            description,
                            Convert.ToDouble(ittr["Latitude"]),
                            Convert.ToDouble(ittr["Longitude"]),
                            "");

                           startTripOnFlag = true; 

                        break;
      #endregion
                     /*
      #region Process speed message
                     case Enums.MessageType.Speed:
                        AddTripReportRow(tripNumber,
                            "Speeding",
                            currRowDateTime.ToString(),
                            streetAddress,
                            ittr["Speed"].ToString(),
                            "",
                            Convert.ToDouble(ittr["Latitude"]),
                            Convert.ToDouble(ittr["Longitude"]),
                            "");
                        break;
      #endregion
 */
      #region Violation Messages
                     case Enums.MessageType.HarshAcceleration:
                     case Enums.MessageType.HarshBraking:
                     case Enums.MessageType.ExtremeAcceleration:
                     case Enums.MessageType.ExtremeBraking:
                     case Enums.MessageType.SeatBelt:
                        string name = "";
                        if (Enums.MessageType.HarshAcceleration == msgType)
                           name = Resources.Const.HarshAcceleration; // "Harsh Acceleration" ;
                        else if (Enums.MessageType.HarshBraking == msgType)
                           name = Resources.Const.HarshBraking; // "Harsh Braking";
                        else if (Enums.MessageType.ExtremeAcceleration == msgType)
                           name = Resources.Const.ExtremeAcceleration; // "Extreme Acceleration";
                        else if (Enums.MessageType.ExtremeBraking == msgType)
                           name = Resources.Const.ExtremeBraking; // "Extreme Braking";
                        else if (Enums.MessageType.SeatBelt == msgType)
                           name = Resources.Const.SeatBeltViolation; //  "Seat Belt Violation";

                        description = "";
                        duration = Util.PairFindValue("DURATION", ittr["CustomProp"].ToString().TrimEnd());
                        if (duration != "")
                        {
                           try
                           {
                              description = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                           }
                           catch
                           {
                           }
                        }
                        // Add sensor info into trip


                        AddTripCondition(addDescription, currIgnitionOn, sensorId, startTripOnFlag, ref description); 


                        AddTripReportRow(tripNumber,
                            name,
                            currRowDateTime.ToString(),
                            streetAddress,
                            ittr["Speed"].ToString(),
                            description,
                            Convert.ToDouble(ittr["Latitude"]),
                            Convert.ToDouble(ittr["Longitude"]),
                           "");

                           startTripOnFlag = true; 

                        break;
      #endregion

                  }
               }
      #endregion
            }
         }
         catch (ERR.DASDbConnectionClosed exCnn)
         {
            throw new ERR.DASDbConnectionClosed(exCnn.Message);
         }
         finally
         {
         }
      }

      public void FillReport(DataTable tblVehicleSensors,
                             DataTable tblVehicleGeozones,
                             bool includeStreetAddress,
                             bool includeSensors,
                             bool includePosition,
                             bool includeIdleTime,
                             bool showLastStoredPosition,
                             double carCost,
                             DataSet rowData,
                             double measurementUnits,
                             Int64 vehicleId,
                             int userId,
                             short vehicleType,
                             string lang)
      {

         //localization settings
         Resources.Const.Culture = new System.Globalization.CultureInfo(lang);
    

         TimeSpan weekdayStartTime = new TimeSpan(0, 0, 0, 0);
         TimeSpan weekdayEndTime = new TimeSpan(0, 23, 59, 59, 999);
         TimeSpan weekendStartTime = new TimeSpan(0, 0, 0, 0);
         TimeSpan weekendEndTime = new TimeSpan(0, 23, 59, 59, 999);
         try
         {

            Localization local = new Localization(sqlExec.ConnectionString);
            DataSet dsMessageTypes = new DataSet() ;

            if (lang != "en")
            {
               Box boxCnf = new DB.Box(sqlExec);
               dsMessageTypes = local.GetGuiLocalization("MessageType", lang);
               VehicleInfo vehicle = new VehicleInfo(sqlExec);
               DataSet dsVehicle = vehicle.GetVehicleInfoByVehicleId(vehicleId);

               Int16 hwTypeId = 0;
               DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(Convert.ToInt32(dsVehicle.Tables[0].Rows[0]["BoxId"]));
               if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
               {
                  // TODO: Today box has only one HW type
                  hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);
               }
               else
               {
                  // TODO: write to log (cannot retrieve HW type for specific box)
               }

               DataSet dsVehicleSensors = new DataSet();
               dsVehicleSensors.Tables.Add(tblVehicleSensors);
               local.LocalizationData(lang, "SensorId", "SensorName", "Sensors", hwTypeId, ref dsVehicleSensors);
               local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
               this.tblVehicleSensors = dsVehicleSensors.Tables[0];
            }

            this.userId = userId;
            this.vehicleId = vehicleId;
            this.tblVehicleSensors = tblVehicleSensors;
            this.measurementUnits = measurementUnits;
            if (rowData == null || rowData.Tables.Count == 0)
               return; // do not process empty data

      #region Local Variables
            Enums.MessageType msgType;
            string customProp = "";
            string description = "";
            bool prevIgnitionOn = false;
            bool currIgnitionOn = false;
            string sensorStatus = "";
            string strCmfCurrentOdo = "";
            int ignOnOdoReading = 0;
            int ignOffOdoReading = 0;
            int fuelReading = 0;
            DayOfWeek currTripDayOfWeek = DayOfWeek.Sunday;
            string msgTypeLang="";
      #endregion
/*
      #region Create map engine instance
            DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
            DataSet dsGeoCodeInfo = mapEngine.GetUserGeoCodeEngineInfo(userId);
            if (dsGeoCodeInfo != null && dsGeoCodeInfo.Tables.Count > 0 && dsGeoCodeInfo.Tables[0].Rows.Count > 0)
               map = new VLF.MAP.ClientMapProxy(MapUtilities.ConvertGeoCodersToMapEngine(dsGeoCodeInfo));
            else
               throw new DASDbConnectionClosed("Unable to retrieve map engine info by user=" + userId);
      #endregion
*/
            foreach (DataRow ittr in rowData.Tables[0].Rows)
            {
      #region Retrieves general information
               boxId = Convert.ToInt32(ittr["BoxId"]);
               msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);
               currRowDateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
               isLandmark = false;

               //Localization 
               if (lang != "en")
               {
                  foreach (DataRow dr in dsMessageTypes.Tables[0].Rows)
                  {
                     if (Convert.ToInt16(dr["FieldID"]) == Convert.ToInt16(ittr["BoxMsgInTypeId"]))
                     {
                        msgTypeLang = dr["LocalName"].ToString() ;
                        break;
                     }

                  }
               }
               else
                  msgTypeLang = msgType.ToString();
 

      #region Save last known Stored Position
               if (includeStreetAddress && msgType == Enums.MessageType.StoredPosition)
                  lastStoredPosition = ittr["StreetAddress"].ToString().TrimEnd();
      #endregion

      #region Process Invalid GPS
               // process message with invalid GPS
               if (Convert.ToInt16(ittr["ValidGps"]) == 1)
               {
                  // Set default 
                  ittr["Latitude"] = 0;
                  ittr["Longitude"] = 0;
                  ittr["Speed"] = VLF.CLS.Def.Const.unassignedIntValue;
                  ittr["Heading"] = VLF.CLS.Def.Const.blankValue;
                  if (includeStreetAddress)
                  {
                     if (showLastStoredPosition && lastStoredPosition != "")
                        streetAddress = VLF.CLS.Def.Const.noValidAddress +
                                        Convert.ToChar(13) + Convert.ToChar(10) +
                                        "[" + lastStoredPosition + "]";
                     else
                        streetAddress = VLF.CLS.Def.Const.noValidAddress;
                  }
               }
               else
               {
                  if (ittr["NearestLandmark"].ToString() != "" && ittr["NearestLandmark"].ToString().StartsWith(VLF.CLS.Def.Const.addressNA) == false)
                     isLandmark = true;

                  if (includeStreetAddress)
                  {
                     streetAddress = (msgType == Enums.MessageType.StoredPosition) ? lastStoredPosition :
                                             ittr["StreetAddress"].ToString().TrimEnd();
                     if (streetAddress == "")
                        streetAddress = VLF.CLS.Def.Const.noGPSData;
                  }
               }
      #endregion

      #region Retrieve sensor detailes
               if (currIgnitionOn && (msgType == Enums.MessageType.Sensor         || 
                                      msgType == Enums.MessageType.SensorExtended ||
                                      msgType == Enums.MessageType.Alarm))
               {
                  // Add sensor info into result dataset
                  try
                  {
                     customProp = ittr["CustomProp"].ToString().TrimEnd();
                  }
                  catch
                  {
                     customProp = "";
                  }
                  sensorStatus = DB.Sensor.GetSensorDescription(customProp, tblVehicleSensors);
               }
      #endregion
      #endregion

      #region Verifies working hours
               TimeSpan currRowTime = new TimeSpan(0, currRowDateTime.Hour, currRowDateTime.Minute, currRowDateTime.Second, currRowDateTime.Millisecond);

               if (
                   ((currRowDateTime.DayOfWeek == DayOfWeek.Sunday || currRowDateTime.DayOfWeek == DayOfWeek.Saturday) && (currRowTime < weekendStartTime || currRowTime > weekendEndTime)) || // not IN weekend working hours
                   ((currRowDateTime.DayOfWeek > DayOfWeek.Sunday || currRowDateTime.DayOfWeek < DayOfWeek.Saturday) && (currRowTime < weekdayStartTime || currRowTime > weekdayEndTime)) // not IN weekday working hours
                   )
               {
                  if (currIgnitionOn)
                  {
      #region Close Trip
      #region Retrieves Odometer Reading
                     strCmfCurrentOdo = "";
                     if ((CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp) != ""))
                     {
                        strCmfCurrentOdo = CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp);
                        try
                        {
                           ignOffOdoReading = Convert.ToInt32(strCmfCurrentOdo);
                        }
                        catch
                        {
                        }
                     }
      #endregion

      #region Retrive Fuel Consumption
                     fuelReading = 0;
                     if ((CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp) != ""))
                     {
                         try
                         {
                             fuelReading = Convert.ToInt32(CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp));
                         }
                         catch
                         {
                         }
                     }
      #endregion
      #region Add message info into trip
      #region Prepare workDateTime
                     DateTime workDateTime = currRowDateTime.Date;
                     if (currRowDateTime.DayOfWeek == DayOfWeek.Sunday || currRowDateTime.DayOfWeek == DayOfWeek.Saturday)
                     {
                        if (currRowTime < weekendStartTime)
                           workDateTime = workDateTime.AddHours(weekendStartTime.Hours).AddMinutes(weekendStartTime.Minutes).AddSeconds(weekendStartTime.Seconds).AddMilliseconds(weekendStartTime.Milliseconds);
                        else
                           workDateTime = workDateTime.AddHours(weekendEndTime.Hours).AddMinutes(weekendEndTime.Minutes).AddSeconds(weekendEndTime.Seconds).AddMilliseconds(weekendEndTime.Milliseconds);
                     }
                     else
                     {
                        if (currRowTime < weekdayStartTime)
                           workDateTime = workDateTime.AddHours(weekdayStartTime.Hours).AddMinutes(weekdayStartTime.Minutes).AddSeconds(weekdayStartTime.Seconds).AddMilliseconds(weekdayStartTime.Milliseconds);
                        else
                           workDateTime = workDateTime.AddHours(weekdayEndTime.Hours).AddMinutes(weekdayEndTime.Minutes).AddSeconds(weekdayEndTime.Seconds).AddMilliseconds(weekdayEndTime.Milliseconds);
                     }
                     currRowDateTime = workDateTime;
      #endregion
                     // LOCALIZATION
                     AddTripReportRow(tripNumber,
                                     msgTypeLang,
                                     currRowDateTime.ToString(),
                                     streetAddress,
                                     ittr["Speed"].ToString(),
                                     sensorStatus,
                                     Convert.ToDouble(ittr["Latitude"]),
                                     Convert.ToDouble(ittr["Longitude"]),
                                     "");
      #endregion
      #region Close previous trip
                     CloseTrip(carCost,
                               Convert.ToDouble(ittr["Latitude"]),
                               Convert.ToDouble(ittr["Longitude"]),
                               Convert.ToInt32(ittr["Speed"]),
                               includeIdleTime,
                               Convert.ToInt16(ittr["ValidGps"]),fuelReading,
                                //GetExtraInfo(userId, "DrID",ittr["customProp"].ToString()),
                               GetExtraInfo(userId, "DrID", ittr["customProp"].ToString(), "", vehicleId, Convert.ToDateTime(ittr["GMT_OriginDateTime"])),
                               ref ignOffOdoReading, ref ignOnOdoReading);
      #endregion
      #endregion
                     prevIgnitionOn = currIgnitionOn = false;
                     continue;
                  }
               }
               else
               {
                   if ((msgType != VLF.CLS.Def.Enums.MessageType.MBAlarm) && (msgType != VLF.CLS.Def.Enums.MessageType.GeoZone) && (msgType != VLF.CLS.Def.Enums.MessageType.Idling ))// this message does not include sens
                  {
      #region Retrieves trip status
                     switch (vehicleType)
                     {
                         case (short)VLF.CLS.Def.Enums.VehicleType.XS_Trailer:
                            if (msgType == Enums.MessageType.Status)
                            {
                               sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keyPower,
                                              ittr["CustomProp"].ToString().TrimEnd());
                               if (sensorStatus == VLF.CLS.Def.Enums.PowerReason.Untethered.ToString())
                                  currIgnitionOn = false;
                               else if (sensorStatus == VLF.CLS.Def.Enums.PowerReason.Tethered.ToString())
                                  currIgnitionOn = true;
                            }
                            else
                               currIgnitionOn = ((Convert.ToInt64(ittr["SensorMask"]) & (1<<10)) == 0) ? false : true;
                           break;
            
                        case (short)VLF.CLS.Def.Enums.VehicleType.Trailer:
                           if (msgType == Enums.MessageType.Status)
                           {
                              sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keyPower, 
                                             ittr["CustomProp"].ToString().TrimEnd());
                              if (sensorStatus == VLF.CLS.Def.Enums.PowerReason.Untethered.ToString())
                                 currIgnitionOn = false;
                              else if (sensorStatus == VLF.CLS.Def.Enums.PowerReason.Tethered.ToString())
                                 currIgnitionOn = true;
                           }
                           else
                              currIgnitionOn = ((Convert.ToInt64(ittr["SensorMask"]) & 4) == 0) ? false : true;

                           break;
                        default:
                           currIgnitionOn = ((Convert.ToInt64(ittr["SensorMask"]) & 4) == 0) ? false : true;
                           break;
                  }


      #endregion

     
                  }
               }
      #endregion

      #region Trip status have changed
               if (prevIgnitionOn != currIgnitionOn)
               {
      #region Retrieve sensor detailes
                  if (currIgnitionOn && (msgType == Enums.MessageType.Sensor         || 
                                         msgType == Enums.MessageType.SensorExtended ||
                                         msgType == Enums.MessageType.Alarm))
                  {
                     // Add sensor info into result dataset
                     try
                     {
                        customProp = ittr["CustomProp"].ToString().TrimEnd();
                     }
                     catch
                     {
                        customProp = "";
                     }
                     sensorStatus = DB.Sensor.GetSensorDescription(customProp, tblVehicleSensors);
                  }
      #endregion
      #region Trip has been ended
                  if (currIgnitionOn == false)
                  {
      #region Close Trip
      #region Retrieves Odometer Reading
                     strCmfCurrentOdo = "";
                     if ((CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp) != ""))
                     {
                        strCmfCurrentOdo = CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp);
                        try
                        {
                           ignOffOdoReading = Convert.ToInt32(strCmfCurrentOdo);
                        }
                        catch
                        {
                        }
                     }
      #endregion


                       
      #region Process idling message
                     if (msgType == Enums.MessageType.Idling)
        {
                        ProcessIdling(Convert.ToInt32(ittr["boxId"]),Convert.ToInt32(Util.PairFindValue(VLF.CLS.Def.Const.keyIdleDuration, ittr["CustomProp"].ToString().TrimEnd())),
                                    includeIdleTime, includeStreetAddress,
                                    Convert.ToDouble(ittr["Latitude"]),
                                    Convert.ToDouble(ittr["Longitude"]),
                                    streetAddress,
                                    ittr["NearestLandmark"].ToString(),lang );
        }
      #endregion
      
      #region Retrive Fuel Consumption
     fuelReading = 0;
     if ((CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp) != ""))
     {
         try
         {
             fuelReading = Convert.ToInt32(CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp));
         }
         catch
         {
         }
     }
      #endregion
      #region Add message info into trip
     AddTripReportRow(tripNumber,
                                     msgTypeLang,
                                     currRowDateTime.ToString(),
                                     streetAddress,
                                     ittr["Speed"].ToString(),
                                     sensorStatus,
                                     Convert.ToDouble(ittr["Latitude"]),
                                     Convert.ToDouble(ittr["Longitude"]),
                                     "");
      #endregion
      #region Close previous trip
                     CloseTrip(carCost,
                               Convert.ToDouble(ittr["Latitude"]),
                               Convert.ToDouble(ittr["Longitude"]),
                               Convert.ToInt32(ittr["Speed"]),
                               includeIdleTime,
                               Convert.ToInt16(ittr["ValidGps"]),fuelReading,
                                //GetExtraInfo(userId, "DrID",ittr["customProp"].ToString()),
                               GetExtraInfo(userId, "DrID", ittr["customProp"].ToString(), "", vehicleId, Convert.ToDateTime(ittr["GMT_OriginDateTime"])),
                               ref ignOffOdoReading, ref ignOnOdoReading);
      #endregion
      #endregion
                  }
      #endregion
      #region Trip has begun
                  else
                  {
      #region Close previous trip
                     if (prevIgnitionOn)
                     {
      #region Close Trip
      #region Retrieves Odometer Reading
                        strCmfCurrentOdo = "";
                        if ((CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp) != ""))
                        {
                           strCmfCurrentOdo = CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp);
                           try
                           {
                              ignOffOdoReading = Convert.ToInt32(strCmfCurrentOdo);
                           }
                           catch
                           {
                           }
                        }
      #endregion
      #region Retrive Fuel Consumption
        fuelReading = 0;
        if ((CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp) != ""))
        {
            try
            {
                fuelReading = Convert.ToInt32(CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp));
            }
            catch
            {
            }
        }
      #endregion
      #region Add message info into trip
        AddTripReportRow(tripNumber,
                                        msgTypeLang,
                                        currRowDateTime.ToString(),
                                        streetAddress,
                                        ittr["Speed"].ToString(),
                                        sensorStatus,
                                        Convert.ToDouble(ittr["Latitude"]),
                                        Convert.ToDouble(ittr["Longitude"]),
                                        "");
      #endregion
      #region Close previous trip
                        CloseTrip(carCost,
                                  Convert.ToDouble(ittr["Latitude"]),
                                  Convert.ToDouble(ittr["Longitude"]),
                                  Convert.ToInt32(ittr["Speed"]),
                                  includeIdleTime,
                                  Convert.ToInt16(ittr["ValidGps"]),fuelReading,
                                    //GetExtraInfo(userId, "DrID",ittr["customProp"].ToString()),
                                     GetExtraInfo(userId, "DrID", ittr["customProp"].ToString(), "", vehicleId, Convert.ToDateTime(ittr["GMT_OriginDateTime"])),
                                  ref ignOffOdoReading, ref ignOnOdoReading);
      #endregion
      #endregion
                     }
      #endregion

      #region Open new trip
      #region Retrieves Odometer reading
                     strCmfCurrentOdo = "";
                     if ((CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp) != ""))
                     {
                        strCmfCurrentOdo = CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp);
                        try
                        {
                           ignOnOdoReading = Convert.ToInt32(strCmfCurrentOdo);
                           ignOffOdoReading = ignOnOdoReading;
                        }
                        catch
                        {
                        }
                     }
      #endregion
      #region Add new trip
                     currTripDayOfWeek = currRowDateTime.DayOfWeek;
                     currTripStarted = currRowDateTime;
                     tripStopTime = VLF.CLS.Def.Const.unassignedDateTime;
                     // New trip started
                     currTripDistance = 0;
                     tripStarted = true;
                     // Open new trip
                     ++tripNumber;

                 



                     AddTripSummaryRow(tripNumber,
                                     currRowDateTime.ToString(),
                                     streetAddress,
                                     tblTripStart,
                                     Convert.ToDouble(ittr["Latitude"]),
                                     Convert.ToDouble(ittr["Longitude"]));

                    
                     AddTripReportRow(tripNumber, Resources.Const.TripStart, // "Trip Start"/*"Trip " + tripNumber.ToString() + " Start"*/,
                                    currRowDateTime.ToString(),
                                    streetAddress,
                                    ittr["Speed"].ToString(),
                                    "",
                                    Convert.ToDouble(ittr["Latitude"]),
                                    Convert.ToDouble(ittr["Longitude"]),
                                    "");


                 

      #endregion
      #endregion
                  }
      #endregion
                  prevIgnitionOn = currIgnitionOn;
               }
      #endregion

      #region Add additional info to the trip
               if (currIgnitionOn)
               {
                  if (Convert.ToInt16(ittr["ValidGps"]) == 0)
                     CalculateTripDistance(Convert.ToDouble(ittr["Latitude"]), Convert.ToDouble(ittr["Longitude"]));

                  if (includeStreetAddress && ittr["NearestLandmark"].ToString() != "" && ittr["NearestLandmark"].ToString().StartsWith(VLF.CLS.Def.Const.addressNA) == false)
                        isLandmark = true;

                  switch (msgType)
                  {
      #region Process idling message
                     case Enums.MessageType.Idling:
                        ProcessIdling(Convert.ToInt32(ittr["boxId"]),Convert.ToInt32(Util.PairFindValue(VLF.CLS.Def.Const.keyIdleDuration, ittr["CustomProp"].ToString().TrimEnd())),
                                    includeIdleTime, includeStreetAddress,
                                    Convert.ToDouble(ittr["Latitude"]),
                                    Convert.ToDouble(ittr["Longitude"]),
                                    streetAddress,
                                    ittr["NearestLandmark"].ToString(),lang );
                        break;
      #endregion
      #region Process position messages
                     case Enums.MessageType.Coordinate:
                     case Enums.MessageType.PositionUpdate:
                     case Enums.MessageType.GeoZone:
                     case Enums.MessageType.StoredPosition:
                        if (includePosition)
                        {
                           string reason = Resources.Const.Position ; //  "Position";
                           description = "";
                           // change reason according to msg type
                           if (msgType == Enums.MessageType.StoredPosition)
                              reason = Resources.Const.StoredPosition; // "Stored Position";
                           else if (msgType == Enums.MessageType.GeoZone)
                           {
                              reason = Resources.Const.GeoZone; //  "Geo Zone";
                              if (tblVehicleGeozones != null && tblVehicleGeozones.Rows.Count > 0)
                                 description = DB.VehicleGeozone.GetGeoZoneDescription(ittr["CustomProp"].ToString().TrimEnd(), tblVehicleGeozones);
                              else
                                 description = DB.VehicleGeozone.GetGeoZoneDescription(ittr["CustomProp"].ToString().TrimEnd(), null);
                           }
                           // Add sensor info into trip
                           AddTripReportRow(tripNumber,
                                           reason,
                                           currRowDateTime.ToString(),
                                           streetAddress,
                                           ittr["Speed"].ToString(),
                                           description,
                                           Convert.ToDouble(ittr["Latitude"]),
                                           Convert.ToDouble(ittr["Longitude"]),
                               "");
                        }
                        break;
      #endregion
      #region Process sensor/alarm messages
                     case Enums.MessageType.Sensor:
                     case Enums.MessageType.SensorExtended:
                     case Enums.MessageType.Alarm:
                        if (includeSensors)
                        {
                           string reason = "";
                           if (msgType == Enums.MessageType.Sensor || msgType == Enums.MessageType.SensorExtended)
                              reason = Resources.Const.Sensor; // "Sensor";
                           if (msgType == Enums.MessageType.Alarm)
                              reason = Resources.Const.Alarm; // "Alarm";

                           AddTripReportRow(tripNumber,
                                            reason,
                                            currRowDateTime.ToString(),
                                            streetAddress,
                                            ittr["Speed"].ToString(),
                                            sensorStatus,
                                            Convert.ToDouble(ittr["Latitude"]),
                                            Convert.ToDouble(ittr["Longitude"]),
                                           "");
                        }
                        break;
      #endregion
      #region Process speeding message
                     case Enums.MessageType.Speeding:
                     case Enums.MessageType.Speed:
                        description = "";
                        string duration = Util.PairFindValue(VLF.CLS.Def.Const.keySpeedDuration, ittr["CustomProp"].ToString().TrimEnd());
                        if (duration != "")
                        {
                           try
                           {
                              description = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                           }
                           catch
                           {
                           }
                        }

                        AddTripReportRow(tripNumber,
                            Resources.Const.Speeding , /*"Speeding", */
                            currRowDateTime.ToString(),
                            streetAddress,
                            ittr["Speed"].ToString(),
                            description,
                            Convert.ToDouble(ittr["Latitude"]),
                            Convert.ToDouble(ittr["Longitude"]),
                            "");
                            
                        break;
      #endregion
/*
      #region Process speed message
                     case Enums.MessageType.Speed:
                        AddTripReportRow(tripNumber,
                            "Speeding",
                            currRowDateTime.ToString(),
                            streetAddress,
                            ittr["Speed"].ToString(),
                            "",
                            Convert.ToDouble(ittr["Latitude"]),
                            Convert.ToDouble(ittr["Longitude"]),
                            "");
                        break;
      #endregion
 */ 
      #region Violation Messages
                            case Enums.MessageType.HarshAcceleration:
                            case Enums.MessageType.HarshBraking:
                            case Enums.MessageType.ExtremeAcceleration:
                            case Enums.MessageType.ExtremeBraking:
                            case Enums.MessageType.SeatBelt:
                                string name = "" ;
                                if ( Enums.MessageType.HarshAcceleration == msgType)
                                    name = Resources.Const.HarshAcceleration    ; // "Harsh Acceleration" ;
                                else if (Enums.MessageType.HarshBraking == msgType)
                                    name = Resources.Const.HarshBraking    ; // "Harsh Braking";
                                else if (Enums.MessageType.ExtremeAcceleration == msgType)
                                    name = Resources.Const.ExtremeAcceleration   ; // "Extreme Acceleration";
                                else if (Enums.MessageType.ExtremeBraking == msgType)
                                    name = Resources.Const.ExtremeBraking    ; // "Extreme Braking";
                                else if (Enums.MessageType.SeatBelt == msgType)
                                    name = Resources.Const.SeatBeltViolation   ; //  "Seat Belt Violation";
                                
                                description = "";
                                duration = Util.PairFindValue("DURATION", ittr["CustomProp"].ToString().TrimEnd());
                                if (duration != "")
                                {
                                    try
                                    {
                                       description = Resources.Const.Duration    + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                                    }
                                    catch
                                    {
                                    }
                                }
                                // Add sensor info into trip
                                AddTripReportRow(tripNumber,
                                    name,
                                    currRowDateTime.ToString(),
                                    streetAddress,
                                    ittr["Speed"].ToString(),
                                    description,
                                    Convert.ToDouble(ittr["Latitude"]),
                                    Convert.ToDouble(ittr["Longitude"]),
                                   "");
                                break;
      #endregion
      
                  }
               }
      #endregion
            }
         }
         catch (ERR.DASDbConnectionClosed exCnn)
         {
            throw new ERR.DASDbConnectionClosed(exCnn.Message);
         }
         finally
         {
         }
      }
#endif
      /// <summary>
      /// Fill trip report
      /// </summary>
      /// <remarks>
      /// 1. Use Ignition bit in sensor mask as trip flag for all vehicles except trailers.
      /// For trailers use MessageType.Status with Enums.PowerReason value in CustomProp 
      /// 2. If Street address is selected, include it into report
      /// 3. Shows sensors changes
      /// 4. Shows vehicle stops (Stop interval is configurable)
      /// 5. Break bown trip by daily usage, otherwise set weekdayStartTime=weekendStartTime=00:00:00.000
      /// and weekdayEndTime=weekendEndTime=23:59:59.999999
      /// 6. Calculates trip statistics:
      ///		6.1 Trip Duration
      ///		6.2 Trip Distance
      ///		6.3 Trip Average Speed
      ///		6.4 Trip Stops
      ///		6.5 Trip Cost
      ///		
      /// 7. Calculates all trips statistics:
      ///		7.1 Total Trips
      ///		7.2 Total Distance (mile/kms)
      ///		7.3 Total Trips Duration
      ///		7.4 Total Average Speed
      ///		7.5 Total Cost
      /// </remarks>
      /// <param name="tblVehicleSensors"></param>
      /// <param name="tblVehicleGeozones"></param>
      /// <param name="includeStreetAddress"></param>
      /// <param name="includeSensors"></param>
      /// <param name="includePosition"></param>
      /// <param name="includeIdleTime"></param>
      /// <param name="showLastStoredPosition"></param>
      /// <param name="carCost"></param>
      /// <param name="rowData"></param>
      /// <param name="measurementUnits"></param>
      /// <param name="vehicleId"></param>
      /// <param name="userId"></param>
      /// <param name="isTrailer"></param>
      /// <param name="weekdayStartTime"></param>
      /// <param name="weekdayEndTime"></param>
      /// <param name="weekendStartTime"></param>
      /// <param name="weekendEndTime"></param>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void FillReport(DataTable tblVehicleSensors,
          DataTable tblVehicleGeozones,
          bool includeStreetAddress,
          bool includeSensors,
          bool includePosition,
          bool includeIdleTime,
          bool showLastStoredPosition,
          double carCost,
          DataSet rowData,
          double measurementUnits,
          Int64 vehicleId,
          int userId,
          bool isTrailer,
          string lang)
      {

         //localization settings
         Resources.Const.Culture = new System.Globalization.CultureInfo(lang);
    

         TimeSpan weekdayStartTime = new TimeSpan(0, 0, 0, 0);
         TimeSpan weekdayEndTime = new TimeSpan(0, 23, 59, 59, 999);
         TimeSpan weekendStartTime = new TimeSpan(0, 0, 0, 0);
         TimeSpan weekendEndTime = new TimeSpan(0, 23, 59, 59, 999);
         try
         {
            this.userId = userId;
            this.vehicleId = vehicleId;
            this.tblVehicleSensors = tblVehicleSensors;
            this.measurementUnits = measurementUnits;
            if (rowData == null || rowData.Tables.Count == 0)
               return; // do not process empty data


            Localization local = new Localization(sqlExec.ConnectionString);
            DataSet dsMessageTypes = new DataSet(); 

            if (lang != "en")
            {
               Box boxCnf = new DB.Box(sqlExec);
               dsMessageTypes = local.GetGuiLocalization("MessageType", lang);
               VehicleInfo vehicle = new VehicleInfo(sqlExec);
               DataSet dsVehicle = vehicle.GetVehicleInfoByVehicleId(vehicleId);

               Int16 hwTypeId = 0;
               DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(Convert.ToInt32(dsVehicle.Tables[0].Rows[0]["BoxId"]));
               if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
               {
                  // TODO: Today box has only one HW type
                  hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);
               }
               else
               {
                  // TODO: write to log (cannot retrieve HW type for specific box)
               }

               DataSet dsVehicleSensors = new DataSet();
               dsVehicleSensors.Tables.Add(tblVehicleSensors);
               local.LocalizationData(lang, "SensorId", "SensorName", "Sensors", hwTypeId, ref dsVehicleSensors);
               local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
               this.tblVehicleSensors = dsVehicleSensors.Tables[0];
            }
            
            #region Local Variables
            Enums.MessageType msgType;
            string customProp = "";
            string description = "";
            bool prevIgnitionOn = false;
            bool currIgnitionOn = false;
            string sensorStatus = "";

            string strCmfCurrentOdo = "";
            int ignOnOdoReading = 0;
            int ignOffOdoReading = 0;
            DayOfWeek currTripDayOfWeek = DayOfWeek.Sunday;
            string msgTypeLang = "";
            int fuelReading = 0;
            #endregion
/*
            #region Create map engine instance
            DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
            DataSet dsGeoCodeInfo = mapEngine.GetUserGeoCodeEngineInfo(userId);
            if (dsGeoCodeInfo != null && dsGeoCodeInfo.Tables.Count > 0 && dsGeoCodeInfo.Tables[0].Rows.Count > 0)
               map = new VLF.MAP.ClientMapProxy(MapUtilities.ConvertGeoCodersToMapEngine(dsGeoCodeInfo));
            else
               throw new DASDbConnectionClosed("Unable to retrieve map engine info by user=" + userId);
            #endregion
*/
            foreach (DataRow ittr in rowData.Tables[0].Rows)
            {
               #region Retrieves general information
               boxId = Convert.ToInt32(ittr["BoxId"]);
               msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);
               currRowDateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
               isLandmark = false;


               //Localization 
               if (lang != "en")
               {
                  foreach (DataRow dr in dsMessageTypes.Tables[0].Rows)
                  {
                     if (Convert.ToInt16(dr["FieldID"]) == Convert.ToInt16(ittr["BoxMsgInTypeId"]))
                     {
                        msgTypeLang = dr["LocalName"].ToString();
                        break;
                     }

                  }
               }
               else
                  msgTypeLang = msgType.ToString();


               #region Save last known Stored Position
               if (includeStreetAddress && msgType == Enums.MessageType.StoredPosition)
                  lastStoredPosition = ittr["StreetAddress"].ToString().TrimEnd();
               #endregion

               #region Process Invalid GPS
               // process message with invalid GPS
               if (Convert.ToInt16(ittr["ValidGps"]) == 1)
               {
                  // Set default 
                  ittr["Latitude"] = 0;
                  ittr["Longitude"] = 0;
                  ittr["Speed"] = VLF.CLS.Def.Const.unassignedIntValue;
                  ittr["Heading"] = VLF.CLS.Def.Const.blankValue;
                  if (includeStreetAddress)
                  {
                     if (showLastStoredPosition && lastStoredPosition != "")
                        streetAddress = VLF.CLS.Def.Const.noValidAddress +
                                        Convert.ToChar(13) + Convert.ToChar(10) +
                                        "[" + lastStoredPosition + "]";
                     else
                        streetAddress = VLF.CLS.Def.Const.noValidAddress;
                  }
               }
               else
               {
                  if (ittr["NearestLandmark"].ToString() != "" && ittr["NearestLandmark"].ToString().StartsWith(VLF.CLS.Def.Const.addressNA) == false)
                     isLandmark = true;

                  if (includeStreetAddress)
                  {
                     if (msgType == Enums.MessageType.StoredPosition)
                        streetAddress = lastStoredPosition;
                     else
                        streetAddress = ittr["StreetAddress"].ToString().TrimEnd();

                     if (streetAddress == "")
                        streetAddress = VLF.CLS.Def.Const.noGPSData;
                  }
               }
               #endregion

               #region Retrieve sensor detailes
               if (currIgnitionOn && (msgType == Enums.MessageType.Sensor           || 
                                      msgType == Enums.MessageType.SensorExtended   ||
                                      msgType == Enums.MessageType.Alarm))
               {
                  // Add sensor info into result dataset
                  try
                  {
                     customProp = ittr["CustomProp"].ToString().TrimEnd();
                  }
                  catch
                  {
                     customProp = "";
                  }
                  sensorStatus = DB.Sensor.GetSensorDescription(customProp, tblVehicleSensors);
               }
               #endregion
               #endregion

               #region Verifies working hours
               TimeSpan currRowTime = new TimeSpan(0, currRowDateTime.Hour, currRowDateTime.Minute, currRowDateTime.Second, currRowDateTime.Millisecond);

               if (
                   ((currRowDateTime.DayOfWeek == DayOfWeek.Sunday || currRowDateTime.DayOfWeek == DayOfWeek.Saturday) && (currRowTime < weekendStartTime || currRowTime > weekendEndTime)) || // not IN weekend working hours
                   ((currRowDateTime.DayOfWeek > DayOfWeek.Sunday || currRowDateTime.DayOfWeek < DayOfWeek.Saturday) && (currRowTime < weekdayStartTime || currRowTime > weekdayEndTime)) // not IN weekday working hours
                   )
               {
                  if (currIgnitionOn)
                  {
                     #region Close Trip
                     #region Retrieves Odometer Reading
                     strCmfCurrentOdo = "";
                     if ((CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp) != ""))
                     {
                        strCmfCurrentOdo = CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp);
                        try
                        {
                           ignOffOdoReading = Convert.ToInt32(strCmfCurrentOdo);
                        }
                        catch
                        {
                        }
                     }
                     #endregion

                     #region Retrive Fuel Consumption
                     fuelReading = 0;
                     if ((CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp) != ""))
                     {
                         try
                         {
                             fuelReading = Convert.ToInt32(CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp));
                         }
                         catch
                         {
                         }
                     }
                     #endregion

                     #region Add message info into trip
                     #region Prepare workDateTime
                     DateTime workDateTime = currRowDateTime.Date;
                     if (currRowDateTime.DayOfWeek == DayOfWeek.Sunday || currRowDateTime.DayOfWeek == DayOfWeek.Saturday)
                     {
                        if (currRowTime < weekendStartTime)
                           workDateTime = workDateTime.AddHours(weekendStartTime.Hours).AddMinutes(weekendStartTime.Minutes).AddSeconds(weekendStartTime.Seconds).AddMilliseconds(weekendStartTime.Milliseconds);
                        else
                           workDateTime = workDateTime.AddHours(weekendEndTime.Hours).AddMinutes(weekendEndTime.Minutes).AddSeconds(weekendEndTime.Seconds).AddMilliseconds(weekendEndTime.Milliseconds);
                     }
                     else
                     {
                        if (currRowTime < weekdayStartTime)
                           workDateTime = workDateTime.AddHours(weekdayStartTime.Hours).AddMinutes(weekdayStartTime.Minutes).AddSeconds(weekdayStartTime.Seconds).AddMilliseconds(weekdayStartTime.Milliseconds);
                        else
                           workDateTime = workDateTime.AddHours(weekdayEndTime.Hours).AddMinutes(weekdayEndTime.Minutes).AddSeconds(weekdayEndTime.Seconds).AddMilliseconds(weekdayEndTime.Milliseconds);
                     }
                     currRowDateTime = workDateTime;
                     #endregion
                     AddTripReportRow(tripNumber,
                         msgTypeLang,
                         currRowDateTime.ToString(),
                         streetAddress,
                         ittr["Speed"].ToString(),
                         sensorStatus,
                         Convert.ToDouble(ittr["Latitude"]),
                         Convert.ToDouble(ittr["Longitude"]),
                         "");
                     #endregion
                     #region Close previous trip
                     CloseTrip(carCost,
                         Convert.ToDouble(ittr["Latitude"]),
                         Convert.ToDouble(ittr["Longitude"]),
                         Convert.ToInt32(ittr["Speed"]),
                         includeIdleTime,
                         Convert.ToInt16(ittr["ValidGps"]),fuelReading,
                          //GetExtraInfo(userId, "DrID",ittr["customProp"].ToString()),
                          GetExtraInfo(userId, "DrID", ittr["customProp"].ToString(), "", vehicleId, Convert.ToDateTime(ittr["GMT_OriginDateTime"])),
                         ref ignOffOdoReading, ref ignOnOdoReading);
                     #endregion
                     #endregion
                     prevIgnitionOn = currIgnitionOn = false;
                     continue;
                  }
               }
               else
               {
                  if ((msgType != VLF.CLS.Def.Enums.MessageType.MBAlarm) && (msgType != VLF.CLS.Def.Enums.MessageType.GeoZone))// this message does not include sens
                  {
                     #region Retrieves trip status
                     if (isTrailer == false)
                     {
                        currIgnitionOn = (Convert.ToInt64(ittr["SensorMask"]) & 4) != 0 ;
/*
                        if ((Convert.ToInt64(ittr["SensorMask"]) & 4) == 0)
                           currIgnitionOn = false;
                        else
                           currIgnitionOn = true;
 */ 
                     }
                     else if (msgType == Enums.MessageType.Status)
                     {
                        sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keyPower, ittr["CustomProp"].ToString().TrimEnd());
                        if (sensorStatus == VLF.CLS.Def.Enums.PowerReason.Untethered.ToString())
                           currIgnitionOn = false;
                        else if (sensorStatus == VLF.CLS.Def.Enums.PowerReason.Tethered.ToString())
                           currIgnitionOn = true;
                     }
                     #endregion

              
                  }
               }
               #endregion

               #region Trip status have changed
               if (prevIgnitionOn != currIgnitionOn)
               {
                  #region Retrieve sensor detailes
                  if (currIgnitionOn && (msgType == Enums.MessageType.Sensor || 
                                         msgType == Enums.MessageType.SensorExtended ||
                                         msgType == Enums.MessageType.Alarm))
                  {
                     // Add sensor info into result dataset
                     try
                     {
                        customProp = ittr["CustomProp"].ToString().TrimEnd();
                     }
                     catch
                     {
                        customProp = "";
                     }
                     sensorStatus = DB.Sensor.GetSensorDescription(customProp, tblVehicleSensors);
                  }
                  #endregion
                  #region Trip has been ended
                  if (currIgnitionOn == false)
                  {
                     #region Close Trip
                     #region Retrieves Odometer Reading
                     strCmfCurrentOdo = "";
                     if ((CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp) != ""))
                     {
                        strCmfCurrentOdo = CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp);
                        try
                        {
                           ignOffOdoReading = Convert.ToInt32(strCmfCurrentOdo);
                        }
                        catch
                        {
                        }
                     }
                     #endregion
                     #region Retrive Fuel Consumption
                     fuelReading = 0;
                     if ((CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp) != ""))
                     {
                         try
                         {
                             fuelReading = Convert.ToInt32(CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp));
                         }
                         catch
                         {
                         }
                     }
                     #endregion
                     #region Add message info into trip
                     AddTripReportRow(tripNumber,
                         msgTypeLang,
                         currRowDateTime.ToString(),
                         streetAddress,
                         ittr["Speed"].ToString(),
                         sensorStatus,
                         Convert.ToDouble(ittr["Latitude"]),
                         Convert.ToDouble(ittr["Longitude"]),
                         "");
                     #endregion
                     #region Close previous trip
                     CloseTrip(carCost,
                         Convert.ToDouble(ittr["Latitude"]),
                         Convert.ToDouble(ittr["Longitude"]),
                         Convert.ToInt32(ittr["Speed"]),
                         includeIdleTime,
                         Convert.ToInt16(ittr["ValidGps"]),fuelReading,
                         //GetExtraInfo(userId, "DrID",ittr["customProp"].ToString()),
                         GetExtraInfo(userId, "DrID", ittr["customProp"].ToString(), "", vehicleId, Convert.ToDateTime(ittr["GMT_OriginDateTime"])),
                         ref ignOffOdoReading, ref ignOnOdoReading);
                     #endregion
                     #endregion
                  }
                  #endregion
                  #region Trip has begun
                  else
                  {
                     #region Close previous trip
                     if (prevIgnitionOn)
                     {
                        #region Close Trip
                        #region Retrieves Odometer Reading
                        strCmfCurrentOdo = "";
                        if ((CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp) != ""))
                        {
                           strCmfCurrentOdo = CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp);
                           try
                           {
                              ignOffOdoReading = Convert.ToInt32(strCmfCurrentOdo);
                           }
                           catch
                           {
                           }
                        }
                        #endregion
                        #region Retrive Fuel Consumption
                        fuelReading = 0;
                        if ((CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp) != ""))
                        {
                            try
                            {
                                fuelReading = Convert.ToInt32(CLS.Util.PairFindValue(CLS.Def.Const.keyFuelConsumption, customProp));
                            }
                            catch
                            {
                            }
                        }
                        #endregion
                        #region Add message info into trip
                        AddTripReportRow(tripNumber,
                                        msgTypeLang,
                                        currRowDateTime.ToString(),
                                        streetAddress,
                                        ittr["Speed"].ToString(),
                                        sensorStatus,
                                        Convert.ToDouble(ittr["Latitude"]),
                                        Convert.ToDouble(ittr["Longitude"]),
                                        "");
                        #endregion
                        #region Close previous trip
                        CloseTrip(carCost,
                                  Convert.ToDouble(ittr["Latitude"]),
                                  Convert.ToDouble(ittr["Longitude"]),
                                  Convert.ToInt32(ittr["Speed"]),
                                  includeIdleTime,
                                  Convert.ToInt16(ittr["ValidGps"]),fuelReading,
                            //GetExtraInfo(userId, "DrID",ittr["customProp"].ToString()),
                               GetExtraInfo(userId, "DrID", ittr["customProp"].ToString(), "", vehicleId, Convert.ToDateTime(ittr["GMT_OriginDateTime"])),
                                  ref ignOffOdoReading, ref ignOnOdoReading);
                        #endregion
                        #endregion
                     }
                     #endregion

                     #region Open new trip
                     #region Retrieves Odometer reading
                     strCmfCurrentOdo = "";
                     if ((CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp) != ""))
                     {
                        strCmfCurrentOdo = CLS.Util.PairFindValue(CLS.Def.Const.keyOdometerValue, customProp);
                        try
                        {
                           ignOnOdoReading = Convert.ToInt32(strCmfCurrentOdo);
                           ignOffOdoReading = ignOnOdoReading;
                        }
                        catch
                        {
                        }
                     }
                     #endregion
                     #region Add new trip
                     currTripDayOfWeek = currRowDateTime.DayOfWeek;
                     currTripStarted = currRowDateTime;
                     tripStopTime = VLF.CLS.Def.Const.unassignedDateTime;
                     // New trip started
                     currTripDistance = 0;
                     tripStarted = true;
                     // Open new trip
                     ++tripNumber;

                     AddTripSummaryRow(tripNumber,
                                        currRowDateTime.ToString(),
                                        streetAddress,
                                        tblTripStart,
                                        Convert.ToDouble(ittr["Latitude"]),
                                        Convert.ToDouble(ittr["Longitude"]));




                     AddTripReportRow(tripNumber, Resources.Const.TripStart, //"Trip Start"/*"Trip " + tripNumber.ToString() + " Start"*/,
                                     currRowDateTime.ToString(),
                                     streetAddress,
                                     ittr["Speed"].ToString(),
                                     "",
                                     Convert.ToDouble(ittr["Latitude"]),
                                     Convert.ToDouble(ittr["Longitude"]),
                                     "");


                    

                     #endregion
                     #endregion
                  }
                  #endregion
                  prevIgnitionOn = currIgnitionOn;
               }
               #endregion

               #region Add additional info to the trip
               if (currIgnitionOn)
               {
                  if (Convert.ToInt16(ittr["ValidGps"]) == 0)
                     CalculateTripDistance(Convert.ToDouble(ittr["Latitude"]), Convert.ToDouble(ittr["Longitude"]));

                  if (includeStreetAddress && ittr["NearestLandmark"].ToString() != "" && ittr["NearestLandmark"].ToString().StartsWith(VLF.CLS.Def.Const.addressNA) == false)
                        isLandmark = true;

                  switch (msgType)
                  {
                     #region Process idling message
                     case Enums.MessageType.Idling:
                        ProcessIdling(Convert.ToInt32(ittr["boxId"]),Convert.ToInt32(Util.PairFindValue(VLF.CLS.Def.Const.keyIdleDuration, ittr["CustomProp"].ToString().TrimEnd())),
                                    includeIdleTime, includeStreetAddress,
                                    Convert.ToDouble(ittr["Latitude"]),
                                    Convert.ToDouble(ittr["Longitude"]),
                                    streetAddress,
                                    ittr["NearestLandmark"].ToString(),lang );
                        break;
                     #endregion
                     #region Process position messages
                     case Enums.MessageType.Coordinate:
                     case Enums.MessageType.PositionUpdate:
                     case Enums.MessageType.GeoZone:
                     case Enums.MessageType.StoredPosition:
                        if (includePosition)
                        {
                           string reason =Resources.Const.Position   ; //  "Position";
                           description = "";
                           // change reason according to msg type
                           if (msgType == Enums.MessageType.StoredPosition)
                              reason =Resources.Const.StoredPosition   ; //  "Stored Position";
                           else if (msgType == Enums.MessageType.GeoZone)
                           {
                              
                              reason = Resources.Const.GeoZone   ; //  "Geo Zone";
                              if (tblVehicleGeozones != null && tblVehicleGeozones.Rows.Count > 0)
                                 description = DB.VehicleGeozone.GetGeoZoneDescription(ittr["CustomProp"].ToString().TrimEnd(), tblVehicleGeozones);
                              else
                                 description = DB.VehicleGeozone.GetGeoZoneDescription(ittr["CustomProp"].ToString().TrimEnd(), null);
                           }
                           // Add sensor info into trip
                           AddTripReportRow(tripNumber,
                                           reason,
                                           currRowDateTime.ToString(),
                                           streetAddress,
                                           ittr["Speed"].ToString(),
                                           description,
                                           Convert.ToDouble(ittr["Latitude"]),
                                           Convert.ToDouble(ittr["Longitude"]),
                                           "");
                        }
                        break;
                     #endregion
                     #region Process sensor/alarm messages
                     case Enums.MessageType.Sensor:
                     case Enums.MessageType.SensorExtended:
                     case Enums.MessageType.Alarm:
                        if (includeSensors)
                        {
                           string reason = "";
                           if (msgType == Enums.MessageType.Sensor || msgType == Enums.MessageType.SensorExtended)
                              reason = Resources.Const.Sensor; // "Sensor";
                           if (msgType == Enums.MessageType.Alarm)
                              reason = Resources.Const.Alarm; // "Alarm";

                           // LOCALIZATION !!!
                           AddTripReportRow(tripNumber,
                                   reason,
                                   currRowDateTime.ToString(),
                                   streetAddress,
                                   ittr["Speed"].ToString(),
                                   sensorStatus,
                                   Convert.ToDouble(ittr["Latitude"]),
                                   Convert.ToDouble(ittr["Longitude"]),
                                    "");
                        }
                        break;
                     #endregion
                     #region Process speeding message
                     case Enums.MessageType.Speed:
                        description = "";
                        string duration = Util.PairFindValue(VLF.CLS.Def.Const.keySpeedDuration, ittr["CustomProp"].ToString().TrimEnd());
                        if (duration != "")
                        {
                           try
                           {
                              description = Resources.Const.Duration    + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                           }
                           catch
                           {
                           }
                        }

                        AddTripReportRow(tripNumber,
                                        Resources.Const.Speeding    /* "Speeding" */,
                                        currRowDateTime.ToString(),
                                        streetAddress,
                                        ittr["Speed"].ToString(),
                                        description,
                                        Convert.ToDouble(ittr["Latitude"]),
                                        Convert.ToDouble(ittr["Longitude"]),
                                        "");
                        break;
                     #endregion
/*
                     #region Process speed message
                     case Enums.MessageType.Speed:
                        AddTripReportRow(tripNumber,
                            "Speeding",
                            currRowDateTime.ToString(),
                            streetAddress,
                            ittr["Speed"].ToString(),
                            "",
                            Convert.ToDouble(ittr["Latitude"]),
                            Convert.ToDouble(ittr["Longitude"]),
                            "");
                        break;
                     #endregion
 */ 
                     #region Violation Messages
                     case Enums.MessageType.HarshAcceleration:
                     case Enums.MessageType.HarshBraking:
                     case Enums.MessageType.ExtremeAcceleration:
                     case Enums.MessageType.ExtremeBraking:
                     case Enums.MessageType.SeatBelt:
                        string name = "";
                        if (Enums.MessageType.HarshAcceleration == msgType)
                           name = Resources.Const.HarshAcceleration    ; // "Harsh Acceleration";
                        else if (Enums.MessageType.HarshBraking == msgType)
                           name = Resources.Const.HarshBraking    ; // "Harsh Braking";
                        else if (Enums.MessageType.ExtremeAcceleration == msgType)
                           name = Resources.Const.ExtremeAcceleration   ; // "Extreme Acceleration";
                        else if (Enums.MessageType.ExtremeBraking == msgType)
                           name = Resources.Const.ExtremeBraking    ; // "Extreme Braking";
                        else if (Enums.MessageType.SeatBelt == msgType)
                           name = Resources.Const.SeatBeltViolation   ; //  "Seat Belt Violation";

                        description = "";
                        duration = Util.PairFindValue("DURATION", ittr["CustomProp"].ToString().TrimEnd());
                        if (duration != "")
                        {
                           try
                           {
                              description = Resources.Const.Duration  + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                           }
                           catch
                           {
                           }
                        }
                        // Add sensor info into trip
                        AddTripReportRow(tripNumber,
                                        name,
                                        currRowDateTime.ToString(),
                                        streetAddress,
                                        ittr["Speed"].ToString(),
                                        description,
                                        Convert.ToDouble(ittr["Latitude"]),
                                        Convert.ToDouble(ittr["Longitude"]),
                                        "");
                        break;
                     #endregion

                  }
               }
               #endregion
            }
         }
         catch (ERR.DASDbConnectionClosed exCnn)
         {
            throw new ERR.DASDbConnectionClosed(exCnn.Message);
         }
         finally
         {
         }
      }


      #region Properties
      /// <summary>
      /// Retrieves trip report details
      /// </summary>
      /// <remarks>
      /// 1. Triggered sensors information
      /// 2. Trip stop details
      /// 3. Position information
      /// </remarks>
      /// <returns> DataTable [TripIndex],[Reson],[Date/Time],[Location],[Speed],[Description] </returns>		
      public DataTable TripReportDetailes
      {
         get
         {
            return tblTripReportData;
         }
      }

      /// <summary>
      /// Retrieves trips duration
      /// </summary>
      /// <returns> DataTable [TripIndex],[Summary],[Remarks] </returns>		
      public DataTable TripsDuration
      {
         get
         {
            return tblTripDuration;
         }
      }

      /// <summary>
      /// Retrieves trips stops
      /// </summary>
      /// <returns> DataTable [TripIndex],[Summary],[Remarks] </returns>		
      public DataTable TripsStopsDuration
      {
         get
         {
            return tblTripStopsDuration;
         }
      }

      /// <summary>
      /// Retrieves trips distances
      /// </summary>
      /// <returns> DataTable [TripIndex],[Summary],[Remarks] </returns>		
      public DataTable TripsDistance
      {
         get
         {
            return tblTripDistance;
         }
      }

      /// <summary>
      /// Retrieves trips average speed
      /// </summary>
      /// <returns> DataTable [TripIndex],[Summary],[Remarks] </returns>		
      public DataTable TripsAverageSpeed
      {
         get
         {
            return tblTripAverageSpeed;
         }
      }
      /// <summary>
      /// Retrieves trips costs
      /// </summary>
      /// <returns> DataTable [TripIndex],[Summary],[Remarks] </returns>		
      public DataTable TripsCost
      {
         get
         {
            return tblTripCost;
         }
      }
      /// <summary>
      /// Retrieves trips starts
      /// </summary>
      /// <returns> DataTable [TripIndex],[Summary],[Remarks] </returns>		
      public DataTable TripsStart
      {
         get
         {
            return tblTripStart;
         }
      }
      /// <summary>
      /// Retrieves trips ends
      /// </summary>
      /// <returns> DataTable [TripIndex],[Summary],[Remarks] </returns>		
      public DataTable TripsEnd
      {
         get
         {
            return tblTripEnd;
         }
      }


      /// <summary>
      /// Retrieves trips fuel consumption
      /// </summary>
      /// <returns> DataTable [TripIndex],[Summary],[Remarks] </returns>		
      public DataTable TripsFuelCons
      {
          get
          {
              return tblTripFuelCons;
          }
      }
      #endregion

      #region Private functions
      private void CalculateTripDistance(double lat, double lon)
      {
         currCoord.Latitude = lat;
         currCoord.Longitude = lon;

         double distanceBetweenGPS = 0;
         // 19 Jan 2007, gb
         if (false == prevCoord.IsNotValid())
//            distanceBetweenGPS = MapUtilities.GetDistance(prevCoord, currCoord);
            distanceBetweenGPS = MapUtilities.GetDistance(prevCoord, currCoord);
         if (distanceBetweenGPS == VLF.CLS.Def.Const.unassignedIntValue)
            currTripDistance += 0;
         else
            currTripDistance += Math.Round(distanceBetweenGPS);

         prevCoord.Latitude = currCoord.Latitude;
         prevCoord.Longitude = currCoord.Longitude;
      }

      /** \fn     private void AddTripReportRow(int tripIndex, string reason, string dateTime, 
       *                   string location, string speed, string description, double lat, double lon, string remark)
       *  \brief  we have to modify this function to return a localized string as the second argument
       *             LOCALIZATION !!!
       */ 
      private void AddTripReportRow(int tripIndex, string reason, string dateTime, string location, string speed, string description, double lat, double lon, string remark)
      {
         object[] objRow = new object[tblTripReportData.Columns.Count];
         objRow[0] = tripIndex.ToString();
         objRow[1] = reason;
         objRow[2] = dateTime;
         objRow[3] = location;
         if (Convert.ToInt32(speed) == VLF.CLS.Def.Const.unassignedIntValue)
            objRow[4] = VLF.CLS.Def.Const.blankValue;
         else
            objRow[4] = Math.Round(Convert.ToInt32(speed) * measurementUnits, 1);
         objRow[5] = description;
         objRow[6] = boxId;
         objRow[7] = lat;
         objRow[8] = lon;
         objRow[9] = remark;
         objRow[10] = vehicleId;
         objRow[11] = isLandmark;
         tblTripReportData.Rows.Add(objRow);
      }
      private void AddTripSummaryRow(int tripIndex, string summary, string remarks, DataTable tblSummary, double lat, double lon)
      {
         object[] objRow = new object[tblSummary.Columns.Count];
         objRow[0] = tripIndex.ToString();
         objRow[1] = summary;
         objRow[2] = remarks;
         objRow[3] = boxId;
         objRow[4] = lat;
         objRow[5] = lon;
         objRow[6] = vehicleId;
         objRow[7] = isLandmark;
         tblSummary.Rows.Add(objRow);
      }
      private void CloseTrip(double carCost, double lat, double lon, int speed, bool includeIdleTime, int validGps,int fuelReading, string extraInfo,ref int ignOffOdoReading, ref int ignOnOdoReading)
      {
         #region Calculate Trip Distance
         if (ignOffOdoReading > ignOnOdoReading && ignOffOdoReading != 0 && ignOnOdoReading != 0)
            currTripDistance = (ignOffOdoReading - ignOnOdoReading) * 1000;
         else if (validGps == 0)
            CalculateTripDistance(lat, lon);

         ignOffOdoReading = 0;
         ignOnOdoReading = 0;
         #endregion

         tripStarted = false;

         #region Add Trip End To Report
         AddTripReportRow(tripNumber,
                           Resources.Const.TripEnd    /*"Trip " + tripNumber + " End"*/,
                           currRowDateTime.ToString(),
                           streetAddress,
                           "0",
                           "",
                           lat,
                           lon,
                           extraInfo);
         #endregion

         #region Add Trip End Summary To Report
         AddTripSummaryRow(tripNumber,
                           currRowDateTime.ToString(),
                           streetAddress,
                           tblTripEnd,
                           lat,
                           lon);
         #endregion

         #region Add Trip Duration Summary To Report
         // 2. Add trip duration
         TimeSpan tripDuration = new TimeSpan(currRowDateTime.Ticks - currTripStarted.Ticks);
         if (tripDuration.Milliseconds != 0)
         {
            // clear milliseconds
            tripDuration = new TimeSpan(tripDuration.Ticks - tripDuration.Milliseconds * TimeSpan.TicksPerMillisecond);
         }
         AddTripSummaryRow(tripNumber,
                           tripDuration.ToString(),
                           tripDuration.TotalSeconds.ToString(),
                           tblTripDuration,
                           lat,
                           lon);
         #endregion

         #region Add Trip Stop Duration Summary To Report
         // 3. Add trip stops
         if (currTripStopsDuration.Milliseconds != 0)
         {
            // clear milliseconds
            currTripStopsDuration = new TimeSpan(currTripStopsDuration.Ticks - currTripStopsDuration.Milliseconds * TimeSpan.TicksPerMillisecond);
         }
         AddTripSummaryRow(tripNumber,
                           currTripStopsDuration.ToString(),
                           currTripStopsDuration.TotalSeconds.ToString(),
                           tblTripStopsDuration,
                           lat,
                           lon);
         #endregion

         #region Add Trip Distance Summary To Report
         // 5. Add trip distance
         currTripDistance = Math.Round(currTripDistance * measurementUnits, 1); // DB in kms
         // convert meters to km
         if (!Double.IsNaN(currTripDistance) && currTripDistance != 0)
            currTripDistance = Math.Round(currTripDistance / 1000, 3);
         else
            currTripDistance = 0;

         string measurementUnitsName = (measurementUnits != 1) ? Const.CT_MILES: Const.CT_KM;
         AddTripSummaryRow(tripNumber,
                           currTripDistance.ToString(),
                           measurementUnitsName,
                           tblTripDistance,
                           lat,
                           lon);
         #endregion

         #region Add Trip Average Speed Summary To Report
         // 6. Add trip average speed
         if (!Double.IsNaN(currTripDistance) && currTripDistance != 0 && tripDuration.TotalSeconds != 0)
            currAvgSpeed = Math.Round(currTripDistance * 3600 / tripDuration.TotalSeconds, 1);
         else
            currAvgSpeed = 0;

         AddTripSummaryRow(tripNumber,
                           currAvgSpeed.ToString(),
                           measurementUnitsName + Const.CT_PER_HOUR, 
                           tblTripAverageSpeed,
                           lat,
                           lon);
         #endregion

         #region Add Trip Cost Summary To Report         
         // 7. Add trip cost
         double tripCost = currTripDistance * carCost;
         if (measurementUnits != 1) // measurementUnitsName = "mi";
            tripCost *= VLF.CLS.Def.Const.kilometersInMile;// transfer from mi to km

         if (tripCost != 0)
            tripCost = Math.Round(tripCost, 2);

         AddTripSummaryRow(tripNumber,
                           tripCost.ToString(),
                           Const.CT_DOLLAR,
                           tblTripCost,
                           lat,
                           lon);
         #endregion

         #region Add Fuel Consumption Summary To Report
         AddTripSummaryRow(tripNumber,
                          Convert.ToString(Math.Round(Convert.ToDouble(fuelReading) / 10, 2)),
                          "L",
                          tblTripFuelCons,
                          lat,
                          lon);
         #endregion

         // 7. Clear current statistics
         currTripDistance = 0;
         prevCoord = new GeoPoint();
         //currSpeedSum = 0;
         currTripStopsDuration = new TimeSpan(0, 0, 0);
      }
      private void ProcessIdling(int boxId, int idleInSec, bool includeIdleTime, bool includeStreetAddress, double lat, double lon, string streetAddress, string nearestLandmark, string lang, string state)
      {


         DateTime idleStartDateTime;
         //localization settings
         Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

         // Checks if it permanent stop
         if (idleInSec > 0 && idleInSec > idlingRange)
         {
            TimeSpan idleInterval = new TimeSpan(0, 0, idleInSec);
            // calculates total stop time for current trip
            currTripStopsDuration = new TimeSpan(currTripStopsDuration.Ticks + idleInterval.Ticks);
            if (includeIdleTime)
            {
               if (idleInterval.Milliseconds != 0)// clear milliseconds
                  idleInterval = new TimeSpan(idleInterval.Ticks - idleInterval.Milliseconds * TimeSpan.TicksPerMillisecond);

               if (includeStreetAddress && nearestLandmark != "" && nearestLandmark.StartsWith(VLF.CLS.Def.Const.addressNA) == false)
                  isLandmark = true;


               if (idleStart && prevBoxId == boxId)
                  idleStartDateTime = currRowDateTime.AddMinutes(-idleInterval.TotalMinutes);
               else
                  idleStartDateTime = currRowDateTime;

               //clear Start Idling flag (Idling with 0 duration)
               prevBoxId = 0;
               idleStart = false;

               if (state != "")
                  state = " - " + state;

               AddTripReportRow(tripNumber,
                                 Resources.Const.EngineIdle,
                                 idleStartDateTime.ToString(),
                                 streetAddress,
                                 "0",
                                 Resources.Const.Duration + idleInterval.ToString()+state,
                                 lat,
                                 lon,
                                 idleInterval.TotalSeconds.ToString()
                                 );


            }
         }
         else
         {
            //set Start Idling flag (Idling with 0 duration)
            prevBoxId = boxId;
            idleStart = true;
         }
      }
      private void ProcessIdling(int boxId, int idleInSec, bool includeIdleTime, bool includeStreetAddress, double lat, double lon, string streetAddress, string nearestLandmark,string lang)
      {

       
         DateTime idleStartDateTime;
         //localization settings
         Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

         // Checks if it permanent stop
         if (idleInSec > 0 && idleInSec > idlingRange)
         {
            TimeSpan idleInterval = new TimeSpan(0, 0, idleInSec);
            // calculates total stop time for current trip
            currTripStopsDuration = new TimeSpan(currTripStopsDuration.Ticks + idleInterval.Ticks);
            if (includeIdleTime)
            {
               if (idleInterval.Milliseconds != 0)// clear milliseconds
                  idleInterval = new TimeSpan(idleInterval.Ticks - idleInterval.Milliseconds * TimeSpan.TicksPerMillisecond);

               if (includeStreetAddress && nearestLandmark != "" && nearestLandmark.StartsWith(VLF.CLS.Def.Const.addressNA) == false)
                  isLandmark = true;


               if (idleStart && prevBoxId == boxId)
                  idleStartDateTime = currRowDateTime.AddMinutes(-idleInterval.TotalMinutes);
               else
                  idleStartDateTime = currRowDateTime;

               //clear Start Idling flag (Idling with 0 duration)
               prevBoxId =0;
               idleStart = false;

               AddTripReportRow(tripNumber,
                                 Resources.Const.EngineIdle  ,
                                 idleStartDateTime.ToString(),
                                 streetAddress,
                                 "0",
                                 Resources.Const.Duration  + idleInterval.ToString(),
                                 lat,
                                 lon,
                                 idleInterval.TotalSeconds.ToString()
                                 );
            }
         }
         else
         {
            //set Start Idling flag (Idling with 0 duration)
            prevBoxId = boxId ;
            idleStart = true;
         }
      }
      #endregion

      private string TripCondition(bool currIgnitionOn,Int32  sensorId,bool startTripOnFlag)
      {
         string state = "";
         string sensorInfo = "";

         if (currIgnitionOn && startTripOnFlag)
            return "";

         if (currIgnitionOn)
            state = Resources.Const.ON;
         else
            state = Resources.Const.OFF;

         switch (sensorId)
         {
            case 8: //PTO
               sensorInfo = Resources.Const.PTO + " " + state;
               break;
            case 11: //TP
               sensorInfo = Resources.Const.TP + " " + state;
               break;
         }

         return sensorInfo;
      }

      private void AddTripCondition(string desc,bool currIgnitionOn,Int32 sensorId ,bool startTripOnFlag, ref string sensorStatus)
      {
         if (desc == "")
            desc = TripCondition(currIgnitionOn, sensorId, startTripOnFlag);

         if ((desc.TrimEnd() != "" && sensorStatus.IndexOf(desc.TrimEnd() ,0)<0))
            sensorStatus = (sensorStatus != "") ? sensorStatus += " - " + desc.TrimEnd() : desc.TrimEnd();
      }

      private string GetExtraInfo(int userId, string key, string CustomProp)
      {
          string strInfo = "";
          string data = "";
          try
          {
              data = Util.PairFindValue(key, CustomProp);

              if (key == "DrID" && data != "")
              {
                  Driver driver = new Driver(sqlExec);
                  DataSet ds = driver.GetAllDriversForOrganizationByUser(userId, data);

                  if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                      strInfo = ds.Tables[0].Rows[0]["FullName"].ToString();
                  else
                      strInfo = data;

                  return strInfo;
              }
              else
                  return "";

          }

          catch
          {
              return "";
          }
      }




      private string GetExtraInfo(int userId, string key, string CustomProp,string driverId, long  vehicleId,DateTime fromDate )
      {
          string strInfo = "";
          string data = "";
          try
          {
              data = Util.PairFindValue(key, CustomProp);
              TimeSpan duration = new System.TimeSpan(0, 0, 1, 0);
              DateTime toDate = fromDate.Add(duration);
              Driver driver = new Driver(sqlExec);

              if (key == "DrID" && data != "")
              {
                 
                  DataSet ds = driver.GetAllDriversForOrganizationByUser(userId, data);

                  if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                      strInfo = ds.Tables[0].Rows[0]["FullName"].ToString();
                  else
                      strInfo = data;

                  return strInfo;
              }
              else
                  return driver.GetTripDriver(driverId, vehicleId, fromDate, toDate);

          }

          catch
          {
              return "";
          }
      }
   }
}
