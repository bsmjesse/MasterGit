using System;
using VLF.ERR;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.DAS.DB;
using VLF.CLS.Def.Structures;
using VLF.CLS;
using VLF.CLS.Def;
using System.Text;


namespace VLF.DAS.Logic
{
    /** \class     Wex
     *  \brief     for now is used only for fuelTransaction
     *             in the future I see at least two other interfaces
     *             - push locations and info for special landmarks (fuel stations)
     *             - 
     */
    public class Wex : Das
    {
        const long ONE_MONTH_IN_SECONDS = 30 * 24 * 60 * 60; // days * hours * minutes * seconds
        const int FUEL_TRANSACTION_MINIMUM_DISTANCE = 100;
        readonly DateTime FIXED_DATETIME = new DateTime(2000, 1, 1);

        FuelTransaction fuelTransaction = null;


        #region General Interfaces
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public Wex(string connectionString)
            : base(connectionString)
        {
            //         Util.BTrace(Util.INF0, ">> Wex -> {0}", connectionString);

            fuelTransaction = new DB.FuelTransaction(base.sqlExec);

            //         Util.BTrace(Util.INF0, "<< Wex -> {0}", connectionString);
        }
        /// <summary>
        /// Destructor
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
        }
        #endregion

        private static double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')           // kilometers
                dist = dist * 1.609344;
            else if (unit == 'N')      // nautical miles
                dist = dist * 0.8684;
            else if (unit == 'M')      // meters
                dist = dist * 1609.344;

            if (System.Double.IsNaN(dist))
                return 0;
            else
                return (dist);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }



        private static double GreatCircleDistanceFormulaMeters(double lat1, double long1, double lat2, double long2)
        {
            double dlon = long2 - long1;
            double dlat = lat2 - lat1;
            double a = (Math.Sin(dlat / 2)) * (Math.Sin(dlat / 2)) +
                            (Math.Cos(lat1) * Math.Cos(lat2) * (Math.Sin(dlon / 2))) *
                                 (Math.Cos(lat1) * Math.Cos(lat2) * (Math.Sin(dlon / 2)));
            double c = 2 * Math.Asin(Math.Min(1.0, Math.Sqrt(a)));
            return 6371.0 * c * 1000;
        }

        public long AddFuelTransaction(string vinNum, DateTime dtWhen, double latitude, double longitude, string xmlData)
        {
            return fuelTransaction.AddFuelTransaction(vinNum, dtWhen, latitude, longitude, xmlData);
        }


        public long AddFuelTransactionByUnitNum(string unitNum, DateTime dtWhen, double latitude, double longitude, string xmlData)
        {
            return fuelTransaction.AddFuelTransactionByUnitNum(unitNum, dtWhen, latitude, longitude, xmlData);
        }


        public long AddFuelCardTransaction(string cardNum, DateTime dtWhen, double latitude, double longitude, string xmlData)
        {
            return fuelTransaction.AddFuelCardTransaction(cardNum, dtWhen, latitude, longitude, xmlData);
        }

        public void CloseFuelTransaction(FuelData fuelData)
        {
            fuelTransaction.CloseFuelTransaction(fuelData);
        }


        public void CloseFuelTransaction_New(FuelData fuelData)
        {
            fuelTransaction.CloseFuelTransaction_New(fuelData);
        }

        public bool ScheduleFuelTransaction(FuelData fuelData)
        {
            return fuelTransaction.ScheduleFuelTransaction(fuelData);
        }

        public bool UpdateFuelTransaction(long id, string data)
        {
            return fuelTransaction.UpdateFuelTransaction(id, data);
        }

        /// <summary>
        ///      updates the vlfFuelTransactionHist with the address based on id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="adressTransaction"></param>
        /// <param name="addressFound"></param>
        /// <returns></returns>
        public int UpdateFuelTransactionAddresses(long id, string adressTransaction, bool gpsAddress)
        {
            int rowsAffected = 0;
            try
            {
                string sql = gpsAddress ? "set addressFound = @address WHERE id = @id" :
                                           "set address = @address WHERE id = @id";
                SqlParameter[] paramArray = new SqlParameter[2];
                if (gpsAddress)
                    paramArray[0] = new SqlParameter("@address", adressTransaction);
                else
                    paramArray[0] = new SqlParameter("@address", adressTransaction);

                paramArray[1] = new SqlParameter("@id", id);

                rowsAffected = fuelTransaction.UpdateRow(sql, paramArray);

                if (rowsAffected == 0)
                    throw new DASAppDataAlreadyExistsException("Cannot update vlfFuelTransactionHist");
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to update vlfFuelTransactionHist ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(objException.Message);
            }

            return rowsAffected;
        }

        public int RecoverPendingTransactions()
        {
            return fuelTransaction.RecoverPendingTransactions();
        }

#if false
      /** \fn     public void ValidateTransactionByIgnitionOff(Object param)
       *  \brief  this is about collecting ONLY sensor extended messages (IGNITION OFF) and applying the same logic 
       * 
       */ 
      public bool ValidateTransactionByIgnitionOff(Object param)
      {
         if (null != param && param is FuelData)
         {
            FuelData fuelData = (FuelData)param;

            // the transactions expired
            if (fuelData._dtLastSchedule <= DateTime.UtcNow)
               return false;

            using (MsgInLite msg = new MsgInLite(ConnectionString, true))
            {
               string sql = string.Format("where boxid={0} and originDateTime >= '{1}' and  originDateTime <= '{2}' and BoxMsgInTypeId=73 ORDER BY originDateTime DESC",
                              fuelData._boxId, fuelData._dtTransaction.AddMinutes(-fuelData._historyTimeRange),
                              fuelData._dtTransaction.AddSeconds(120));
               DataSet ds = msg.RunDynamicSQL("select * from vlfMsgInHst", sql);
               if (Util.IsDataSetValid(ds))
               {
                  // find the closest packet in time and calculate the distance between points
                  long timeProximity = ONE_MONTH_IN_SECONDS;
                  double spaceProximity = 50000000000.0, latFound, longFound;   // 500 km
                  int cnt = 0,
                      shortestDistanceIdx = -1,
                      shortestTimeIdx = -1;
                  DateTime dtLocation;
                  foreach (DataRow dr in ds.Tables[0].Rows)
                  {
                     dtLocation = Convert.ToDateTime(dr["originDateTime"].ToString());
                     latFound = Convert.ToDouble(dr["latitude"].ToString());
                     longFound = Convert.ToDouble(dr["longitude"].ToString());


                     long timeFromTransaction = (long)fuelData._dtTransaction.Subtract(dtLocation).TotalSeconds;
                     double distFromTransaction = distance(latFound, longFound, fuelData._latitude, fuelData._longitude, 'M'); // GreatCircleDistanceFormulaMeters(latFound, longFound, fuelData._latitude,fuelData._longitude);

                     Util.BTrace(Util.INF0, " at={0}, [{1}, {2}], proximity[T={3} secs, D={4} meters]",
                                          dtLocation, latFound, longFound, timeFromTransaction, distFromTransaction);

                     if (distFromTransaction < spaceProximity)
                     {
                        shortestDistanceIdx = cnt;
                        spaceProximity = distFromTransaction;
                     }

                     if (timeFromTransaction < timeProximity)
                     {
                        shortestTimeIdx = cnt;
                        timeProximity = timeFromTransaction;

                        // if the points are within a declared area, then validate transaction
                        if (distFromTransaction < FUEL_TRANSACTION_MINIMUM_DISTANCE ||
                            distFromTransaction < fuelData._maximumDistance)
                        {
                           fuelData._latitudeFound = latFound;
                           fuelData._longitudeFound = longFound;
                           fuelData._dtLocationFound = dtLocation;
                           fuelData._status = fuelTransactionStatus.FuelTransaction_OK;
                           Util.BTrace(Util.INF0, "- ValidateTransactionByIgnitionOff -> OK for {0}", fuelData.ToString());
                           return true;
                        }
                     }
                     ++cnt;
                  }
               }
            }
         }

         return false;

      }
#endif

        /// <summary>
        ///      look at the first message before the time window
        ///      if it is an Ignition OFF and within radius, everything is fine
        ///      else send an alert
        /// </summary>
        /// <param name="fuelData"></param>
        /// <returns></returns>
        private bool IsLastMessageIgnitionOff(FuelData fuelData)
        {
            #region Last Message contains Ignition OFF in sensor mask
            // if the previous message found in history, previous to the search interval is IGNTION OFF
            // send it as a fuel fraud
            using (MsgInLite msg = new MsgInLite(ConnectionString, true))
            {
                string sql = string.Format("where boxid={0} and originDateTime <= '{1}' and BoxMsgInTypeId not in (10,13,36,76,900) and CustomProp not like '%SleepMode%' ORDER BY originDateTime DESC",
                              fuelData._boxId, fuelData._dtTransaction.AddMinutes(-fuelData._historyTimeRange));

                Util.BTrace(Util.INF0, "- WexService.IsLastMessageIgnitionOff [1. Run Query:]");


                DataSet ds = msg.RunDynamicSQL("select top 1 * from vlfMsgInHst with (nolock) ", sql);
                if (Util.IsDataSetValid(ds))
                {

                    Util.BTrace(Util.INF0, "- WexService.IsLastMessageIgnitionOff [2. Data Found]");

                    DataRow dr = ds.Tables[0].Rows[0];
                    if ((int)Enums.MessageType.SensorExtended ==
                                Convert.ToInt32(dr["BoxMsgInTypeId"].ToString()) &&
                         (Convert.ToUInt64(dr["SensorMask"].ToString()) & 0x4) == 0L)
                    {
                        Util.BTrace(Util.INF0, "- WexService.IsLastMessageIgnitionOff [3. Ignition OFF Found]");

                        fuelData._status = fuelTransactionStatus.FuelTransaction_VEHICLEPARKED;
                        fuelData._dtLocationFound = Convert.ToDateTime(dr["originDateTime"].ToString());
                        fuelData._latitudeFound = Convert.ToDouble(dr["latitude"].ToString());
                        fuelData._longitudeFound = Convert.ToDouble(dr["longitude"].ToString());


                        int proximity = (int)distance(fuelData._latitudeFound,
                                                      fuelData._longitudeFound,
                                                      fuelData._latitude,
                                                      fuelData._longitude, 'M');

                        if (proximity > fuelData._maximumDistance)
                        {
                            fuelData._status = fuelTransactionStatus.FuelTransaction_VEHICLEPARKED;
                            Util.BTrace(Util.INF0, "- WexService.IsLastMessageIgnitionOff [4. Vehicle Parked]");
                        }
                        else
                        {
                            fuelData._status = fuelTransactionStatus.FuelTransaction_OK;
                            Util.BTrace(Util.INF0, "- WexService.IsLastMessageIgnitionOff [5. Transaction OK]");
                        }

                        Util.BTrace(Util.INF0, "Special case (nothing found in the time range) : last message with Ignition OFF for boxid={0} at {1} type={2}, proximity={3}, radius={4}, result={5}",
                                    fuelData._boxId, fuelData._dtLocationFound, Enums.MessageType.SensorExtended.ToString(), proximity, fuelData._maximumDistance, fuelData._status.ToString());

                        fuelData._maximumDistance = proximity;

                        return true;
                    }
                    else
                        Util.BTrace(Util.INF0, "- WexService.IsLastMessageIgnitionOff = [NO] -> Message Type found is: [" + dr["BoxMsgInTypeId"].ToString() + "]");
                }
            }
            #endregion Last Message contains Ignition OFF message

            Util.BTrace(Util.INF0, "- WexService.IsLastMessageIgnitionOff [6. No Data Found - Scheduled]");

            return false;
        }


        private bool IsLastMessageIgnitionOff_New(FuelData fuelData)
        {
            #region Last Message contains Ignition OFF in sensor mask
            // if the previous message found in history, previous to the search interval is IGNTION OFF
            // send it as a fuel fraud

            //Temp to get data from 9.71
            string conString = "Initial Catalog=SentinelFM;Data Source=192.168.9.71;User ID=sa;Password=BSMwireless1;Pooling=true;Max Pool Size=300;";
            string odometerStr = "";
            //using (MsgInLite msg = new MsgInLite(ConnectionString, true))
            using (MsgInLite msg = new MsgInLite(conString, true))
            {
                string sql = string.Format("where boxid={0} and originDateTime <= '{1}' and BoxMsgInTypeId not in (10,13,36,900) and CustomProp not like '%SleepMode%' ORDER BY originDateTime DESC",
                              fuelData._boxId, fuelData._dtTransaction.AddMinutes(-fuelData._historyTimeRange));

                Util.BTrace(Util.INF0, "- WexService.IsLastMessageIgnitionOff [1. Run Query:]");


                DataSet ds = msg.RunDynamicSQL("select top 1 * from vlfMsgInHst with(nolock)", sql);
                if (Util.IsDataSetValid(ds))
                {

                    Util.BTrace(Util.INF0, "- WexService.IsLastMessageIgnitionOff [2. Data Found]");

                    DataRow dr = ds.Tables[0].Rows[0];
                    if ((int)Enums.MessageType.SensorExtended ==
                                Convert.ToInt32(dr["BoxMsgInTypeId"].ToString()) &&
                         (Convert.ToUInt64(dr["SensorMask"].ToString()) & 0x4) == 0L)
                    {
                        Util.BTrace(Util.INF0, "- WexService.IsLastMessageIgnitionOff [3. Ignition OFF Found]");

                        fuelData._status = fuelTransactionStatus.FuelTransaction_VEHICLEPARKED;
                        fuelData._dtLocationFound = Convert.ToDateTime(dr["originDateTime"].ToString());
                        fuelData._latitudeFound = Convert.ToDouble(dr["latitude"].ToString());
                        fuelData._longitudeFound = Convert.ToDouble(dr["longitude"].ToString());

                        try
                        {
                            odometerStr = Util.PairFindValue("Odometer", dr["CustomProp"].ToString());
                        }
                        catch
                        {
                        }

                        if (odometerStr != "")
                            fuelData._data = fuelData._data + " Odometer_Value:" + odometerStr;

                        int proximity = (int)distance(fuelData._latitudeFound,
                                                      fuelData._longitudeFound,
                                                      fuelData._latitude,
                                                      fuelData._longitude, 'M');

                        if (proximity > fuelData._maximumDistance)
                        {
                            fuelData._status = fuelTransactionStatus.FuelTransaction_VEHICLEPARKED;
                            Util.BTrace(Util.INF0, "- WexService.IsLastMessageIgnitionOff [4. Vehicle Parked]");
                        }
                        else
                        {
                            fuelData._status = fuelTransactionStatus.FuelTransaction_OK;
                            Util.BTrace(Util.INF0, "- WexService.IsLastMessageIgnitionOff [5. Transaction OK]");
                        }

                        Util.BTrace(Util.INF0, "Special case (nothing found in the time range) : last message with Ignition OFF for boxid={0} at {1} type={2}, proximity={3}, radius={4}, result={5}",
                                    fuelData._boxId, fuelData._dtLocationFound, Enums.MessageType.SensorExtended.ToString(), proximity, fuelData._maximumDistance, fuelData._status.ToString());

                        fuelData._maximumDistance = proximity;

                        return true;
                    }
                    else
                        Util.BTrace(Util.INF0, "- WexService.IsLastMessageIgnitionOff = [NO] -> Message Type found is: [" + dr["BoxMsgInTypeId"].ToString() + "]");
                }
            }
            #endregion Last Message contains Ignition OFF message

            Util.BTrace(Util.INF0, "- WexService.IsLastMessageIgnitionOff [6. No Data Found - Scheduled]");

            return false;
        }

        /** \fn           public void ValidateTransaction(Object param)
         * 
         *  \brief        it handles the records picked based on a certain criteria
         *  \comments     it should remove the record not returning GPS or with GPS not valid
         *   05/14/2010   (gb)  added the case where if there are no records found in the searching interval             
         *                   or 
         *                  added the case where there is no information, the transaction expired but the last message
         *                   was ignition off, outside of the searched interval
         */
        public void ValidateTransaction(Object param)
        {
            if (null != param && param is FuelData)
            {
                FuelData fuelData = (FuelData)param;

                // the transactions expired
                if (fuelData._dtLastSchedule <= DateTime.UtcNow)
                {
                    fuelData._status = fuelTransactionStatus.FuelTransaction_EXPIRED;
                    fuelData._dtLocationFound = FIXED_DATETIME;
                    Util.BTrace(Util.INF0, "- WexService.ValidateTransaction Transaction Expired <-");
                    return;
                }

                using (MsgInLite msg = new MsgInLite(ConnectionString, true))
                {
                    #region Search in the time range interval for packets
                    string sql = string.Format("where boxid={0} and originDateTime >= '{1}' and  originDateTime <= '{2}' and BoxMsgInTypeId not in (10,13,36,900) and ValidGPS=0 ORDER BY originDateTime DESC",
                                   fuelData._boxId, fuelData._dtTransaction.AddMinutes(-fuelData._historyTimeRange),
                                   fuelData._dtTransaction.AddSeconds(120));
                    DataSet ds = msg.RunDynamicSQL("select * from vlfMsgInHst with (nolock)", sql);

                    Util.BTrace(Util.INF0, "- WexService.ValidateTransaction  [1. Run Query]", sql);

                    if (Util.IsDataSetValid(ds))
                    {
                        Util.BTrace(Util.INF0, "- WexService.ValidateTransaction [2. Data Found in Range]}");

                        // find the closest packet in time and calculate the distance between points
                        long timeProximity = ONE_MONTH_IN_SECONDS;
                        double spaceProximity = 50000000000.0, latFound, longFound;   // 500 km
                        int cnt = 0,
                            shortestDistanceIdx = -1,
                            shortestTimeIdx = -1;
                        DateTime dtLocation;
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            dtLocation = Convert.ToDateTime(dr["originDateTime"].ToString());
                            latFound = Convert.ToDouble(dr["latitude"].ToString());
                            longFound = Convert.ToDouble(dr["longitude"].ToString());


                            long timeFromTransaction = (long)fuelData._dtTransaction.Subtract(dtLocation).TotalSeconds;
                            double distFromTransaction = distance(latFound, longFound, fuelData._latitude, fuelData._longitude, 'M'); // GreatCircleDistanceFormulaMeters(latFound, longFound, fuelData._latitude,fuelData._longitude);

                            Util.BTrace(Util.INF0, " at={0}, [{1}, {2}], proximity[T={3} secs, D={4} meters]",
                                                 dtLocation, latFound, longFound, timeFromTransaction, distFromTransaction);

                            //if (distFromTransaction < spaceProximity)
                            //{
                            //   shortestDistanceIdx = cnt;
                            //   //spaceProximity = distFromTransaction;  //comment out due to shortest time being our targe. see comment #1
                            //}

                            if (timeFromTransaction < timeProximity)
                            {
                                shortestTimeIdx = cnt;
                                timeProximity = timeFromTransaction;
                                spaceProximity = distFromTransaction;  //comment #1 -> this proximity is relevant to the short timeProximity 

                                //NO DECISION ON PROXIMITY YET -> removed for timeProximity check only
                                //// if the points are within a declared area, then validate transaction
                                //if (distFromTransaction < FUEL_TRANSACTION_MINIMUM_DISTANCE || 
                                //    distFromTransaction < fuelData._maximumDistance)
                                //{
                                //   fuelData._latitudeFound  = latFound;
                                //   fuelData._longitudeFound = longFound ;
                                //   fuelData._dtLocationFound = dtLocation;
                                //   fuelData._status = fuelTransactionStatus.FuelTransaction_OK;
                                //   Util.BTrace(Util.INF0, "- ValidateTransaction -> OK for {0}", fuelData.ToString());
                                //   return ;
                                //}
                            }
                            ++cnt;
                        }
                        if (shortestTimeIdx > -1)
                        {
                            dtLocation = Convert.ToDateTime(ds.Tables[0].Rows[shortestTimeIdx]["originDateTime"].ToString());
                            latFound = Convert.ToDouble(ds.Tables[0].Rows[shortestTimeIdx]["latitude"].ToString());
                            longFound = Convert.ToDouble(ds.Tables[0].Rows[shortestTimeIdx]["longitude"].ToString());

                            fuelData._latitudeFound = latFound;
                            fuelData._longitudeFound = longFound;
                            fuelData._dtLocationFound = dtLocation;

                            // check with the most closed point in time and see if the rule is sufficient
                            // if the distance for the closest point int time is higher than fuelData._maximumDistance
                            // send an alarm
                            // but wasn't the minimum distance in time from when the transaction happened
                            if (spaceProximity < FUEL_TRANSACTION_MINIMUM_DISTANCE ||
                                spaceProximity < fuelData._maximumDistance)
                                fuelData._status = fuelTransactionStatus.FuelTransaction_OK;
                            else
                            {
                                // you might have a fuel fraud 
                                fuelData._status = fuelTransactionStatus.FuelTransaction_FRAUD;
                                fuelData._maximumDistance = (int)distance(latFound, longFound, fuelData._latitude, fuelData._longitude, 'M'); // GreatCircleDistanceFormulaMeters(latFound, longFound, fuelData._latitude,fuelData._longitude);
                            }
                            Util.BTrace(Util.INF0, "- ValidateTransaction -> resolution found for {0}", fuelData.ToString());
                            return;
                        }
                        //if (shortestDistanceIdx > -1)
                        //{
                        //   dtLocation = Convert.ToDateTime(ds.Tables[0].Rows[shortestDistanceIdx]["originDateTime"].ToString());
                        //   latFound = Convert.ToDouble(ds.Tables[0].Rows[shortestDistanceIdx]["latitude"].ToString());
                        //   longFound = Convert.ToDouble(ds.Tables[0].Rows[shortestDistanceIdx]["longitude"].ToString());

                        //   fuelData._latitudeFound = latFound;
                        //   fuelData._longitudeFound = longFound;
                        //   fuelData._dtLocationFound = dtLocation;

                        //   // check with the most closed point in time and see if the rule is sufficient
                        //   // if the distance for the closest point int time is higher than fuelData._maximumDistance
                        //   // send an alarm
                        //   // but wasn't the minimum distance in time from when the transaction happened
                        //   if (spaceProximity < FUEL_TRANSACTION_MINIMUM_DISTANCE ||
                        //       spaceProximity < fuelData._maximumDistance)
                        //      fuelData._status = fuelTransactionStatus.FuelTransaction_PARTIALLY_OK;
                        //   else
                        //   {
                        //      // you might have a fuel fraud 
                        //      fuelData._status = fuelTransactionStatus.FuelTransaction_FRAUD;
                        //      fuelData._maximumDistance = (int)distance(latFound, longFound, fuelData._latitude, fuelData._longitude, 'M'); // GreatCircleDistanceFormulaMeters(latFound, longFound, fuelData._latitude,fuelData._longitude);
                        //   }
                        //}
                        //else
                        //{
                        //   // you might have a fuel fraud which is at a very long distance but no point is found 
                        //   // within the radius of the fuel transaction
                        //   fuelData._status = fuelTransactionStatus.FuelTransaction_FRAUD;
                        //   fuelData._dtLocationFound = FIXED_DATETIME;
                        //}



                    } //  if (Util.IsDataSetValid(ds))
                    #endregion Search in the time range interval for packets
                } // end

                // reschedule the operation, there is no data available
                // because 
                //    1. you didn't get data -> empty dataset

                Util.BTrace(Util.INF0, "- WexService.ValidateTransaction [3. NO Data Found in Range. Check for Last Message Ignition OFF]}");
                if (false == IsLastMessageIgnitionOff(fuelData))
                    fuelData._status = fuelTransactionStatus.FuelTransaction_SCHEDULED;

            }

        }

        public void ValidateTransaction_New(Object param)
        {
            if (null != param && param is FuelData)
            {
                FuelData fuelData = (FuelData)param;

                // the transactions expired
                if (fuelData._dtLastSchedule <= DateTime.UtcNow)
                {
                    fuelData._status = fuelTransactionStatus.FuelTransaction_EXPIRED;
                    fuelData._dtLocationFound = FIXED_DATETIME;
                    Util.BTrace(Util.INF0, "-FuelTransactionService.ValidateTransaction Transaction Expired <-");
                    return;
                }


                //Temp to get data from 9.71
                string conString = "Initial Catalog=SentinelFM;Data Source=192.168.9.71;User ID=sa;Password=BSMwireless1;Pooling=true;Max Pool Size=300;";
                string odometerStr = "";
                //using (MsgInLite msg = new MsgInLite(ConnectionString, true))
                using (MsgInLite msg = new MsgInLite(conString, true))
                {
                    #region Search in the time range interval for packets
                    string sql = string.Format("where boxid={0} and originDateTime >= '{1}' and  originDateTime <= '{2}' and BoxMsgInTypeId not in (10,13,36,900) and ValidGPS=0 ORDER BY originDateTime DESC",
                                   fuelData._boxId, fuelData._dtTransaction.AddMinutes(-fuelData._historyTimeRange),
                                   fuelData._dtTransaction.AddSeconds(120));
                    DataSet ds = msg.RunDynamicSQL("select * from vlfMsgInHst with(nolock) ", sql);

                    Util.BTrace(Util.INF0, "- FuelTransactionService.ValidateTransaction  [1. Run Query]", sql);

                    if (Util.IsDataSetValid(ds))
                    {
                        Util.BTrace(Util.INF0, "- FuelTransactionService.ValidateTransaction [2. Data Found in Range]}");

                        // find the closest packet in time and calculate the distance between points
                        long timeProximity = ONE_MONTH_IN_SECONDS;
                        double spaceProximity = 50000000000.0, latFound, longFound;   // 500 km
                        int cnt = 0,
                            shortestDistanceIdx = -1,
                            shortestTimeIdx = -1;
                        DateTime dtLocation;
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            dtLocation = Convert.ToDateTime(dr["originDateTime"].ToString());
                            latFound = Convert.ToDouble(dr["latitude"].ToString());
                            longFound = Convert.ToDouble(dr["longitude"].ToString());

                            long timeFromTransaction = (long)fuelData._dtTransaction.Subtract(dtLocation).TotalSeconds;
                            double distFromTransaction = distance(latFound, longFound, fuelData._latitude, fuelData._longitude, 'M'); // GreatCircleDistanceFormulaMeters(latFound, longFound, fuelData._latitude,fuelData._longitude);

                            Util.BTrace(Util.INF0, " at={0}, [{1}, {2}], proximity[T={3} secs, D={4} meters]",
                                                 dtLocation, latFound, longFound, timeFromTransaction, distFromTransaction);

                            //if (distFromTransaction < spaceProximity)
                            //{
                            //   shortestDistanceIdx = cnt;
                            //   //spaceProximity = distFromTransaction;  //comment out due to shortest time being our targe. see comment #1
                            //}

                            if (timeFromTransaction < timeProximity)
                            {
                                shortestTimeIdx = cnt;
                                timeProximity = timeFromTransaction;
                                spaceProximity = distFromTransaction;  //comment #1 -> this proximity is relevant to the short timeProximity 

                                //NO DECISION ON PROXIMITY YET -> removed for timeProximity check only
                                //// if the points are within a declared area, then validate transaction
                                //if (distFromTransaction < FUEL_TRANSACTION_MINIMUM_DISTANCE || 
                                //    distFromTransaction < fuelData._maximumDistance)
                                //{
                                //   fuelData._latitudeFound  = latFound;
                                //   fuelData._longitudeFound = longFound ;
                                //   fuelData._dtLocationFound = dtLocation;
                                //   fuelData._status = fuelTransactionStatus.FuelTransaction_OK;
                                //   Util.BTrace(Util.INF0, "- ValidateTransaction -> OK for {0}", fuelData.ToString());
                                //   return ;
                                //}
                            }
                            ++cnt;
                        }
                        if (shortestTimeIdx > -1)
                        {
                            dtLocation = Convert.ToDateTime(ds.Tables[0].Rows[shortestTimeIdx]["originDateTime"].ToString());
                            latFound = Convert.ToDouble(ds.Tables[0].Rows[shortestTimeIdx]["latitude"].ToString());
                            longFound = Convert.ToDouble(ds.Tables[0].Rows[shortestTimeIdx]["longitude"].ToString());
                            try
                            {
                                odometerStr = Util.PairFindValue("Odometer", ds.Tables[0].Rows[shortestTimeIdx]["CustomProp"].ToString());
                            }
                            catch
                            {
                            }

                            fuelData._latitudeFound = latFound;
                            fuelData._longitudeFound = longFound;
                            fuelData._dtLocationFound = dtLocation;

                            try
                            {
                                odometerStr = Util.PairFindValue("Odometer", ds.Tables[0].Rows[shortestTimeIdx]["CustomProp"].ToString());
                            }
                            catch
                            {
                            }

                            if (odometerStr != "")
                                fuelData._data = fuelData._data + " Odometer_Value:" + odometerStr;

                            // check with the most closed point in time and see if the rule is sufficient
                            // if the distance for the closest point int time is higher than fuelData._maximumDistance
                            // send an alarm
                            // but wasn't the minimum distance in time from when the transaction happened
                            if (spaceProximity < FUEL_TRANSACTION_MINIMUM_DISTANCE ||
                                spaceProximity < fuelData._maximumDistance)
                                fuelData._status = fuelTransactionStatus.FuelTransaction_OK;
                            else
                            {
                                // you might have a fuel fraud 
                                fuelData._status = fuelTransactionStatus.FuelTransaction_FRAUD;
                                fuelData._maximumDistance = (int)distance(latFound, longFound, fuelData._latitude, fuelData._longitude, 'M'); // GreatCircleDistanceFormulaMeters(latFound, longFound, fuelData._latitude,fuelData._longitude);
                            }
                            Util.BTrace(Util.INF0, "- ValidateTransaction -> resolution found for {0}", fuelData.ToString());
                            return;
                        }
                        //if (shortestDistanceIdx > -1)
                        //{
                        //   dtLocation = Convert.ToDateTime(ds.Tables[0].Rows[shortestDistanceIdx]["originDateTime"].ToString());
                        //   latFound = Convert.ToDouble(ds.Tables[0].Rows[shortestDistanceIdx]["latitude"].ToString());
                        //   longFound = Convert.ToDouble(ds.Tables[0].Rows[shortestDistanceIdx]["longitude"].ToString());

                        //   fuelData._latitudeFound = latFound;
                        //   fuelData._longitudeFound = longFound;
                        //   fuelData._dtLocationFound = dtLocation;

                        //   // check with the most closed point in time and see if the rule is sufficient
                        //   // if the distance for the closest point int time is higher than fuelData._maximumDistance
                        //   // send an alarm
                        //   // but wasn't the minimum distance in time from when the transaction happened
                        //   if (spaceProximity < FUEL_TRANSACTION_MINIMUM_DISTANCE ||
                        //       spaceProximity < fuelData._maximumDistance)
                        //      fuelData._status = fuelTransactionStatus.FuelTransaction_PARTIALLY_OK;
                        //   else
                        //   {
                        //      // you might have a fuel fraud 
                        //      fuelData._status = fuelTransactionStatus.FuelTransaction_FRAUD;
                        //      fuelData._maximumDistance = (int)distance(latFound, longFound, fuelData._latitude, fuelData._longitude, 'M'); // GreatCircleDistanceFormulaMeters(latFound, longFound, fuelData._latitude,fuelData._longitude);
                        //   }
                        //}
                        //else
                        //{
                        //   // you might have a fuel fraud which is at a very long distance but no point is found 
                        //   // within the radius of the fuel transaction
                        //   fuelData._status = fuelTransactionStatus.FuelTransaction_FRAUD;
                        //   fuelData._dtLocationFound = FIXED_DATETIME;
                        //}



                    } //  if (Util.IsDataSetValid(ds))
                    #endregion Search in the time range interval for packets
                } // end

                // reschedule the operation, there is no data available
                // because 
                //    1. you didn't get data -> empty dataset

                Util.BTrace(Util.INF0, "- FuelTransactionService.ValidateTransaction [3. NO Data Found in Range. Check for Last Message Ignition OFF]}");
                if (false == IsLastMessageIgnitionOff_New(fuelData))
                    fuelData._status = fuelTransactionStatus.FuelTransaction_SCHEDULED;

            }

        }


        public FuelData[] GetNextRawFuelTransactions(int size)
        {
            return fuelTransaction.GetNextRawFuelTransactions(size);
        }


        public FuelData[] GetNextRawFuelTransactions(int size, int superOrgId)
        {
            return fuelTransaction.GetNextRawFuelTransactions(size, superOrgId);
        }

        /// <summary>
        /// Get Fuel Transaction History
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public DataSet GetFuelTransactionHist(int organizationId, int userId, DateTime from, DateTime to)
        {
            return fuelTransaction.GetFuelTransactionHist(organizationId, userId, from, to);
        }


        public DataSet GetFuelFraudTransactionHist(int organizationId, int userId, DateTime from, DateTime to)
        {
            return fuelTransaction.GetFuelFraudTransactionHist(organizationId, userId, from, to);
        }
    }
}
