/** \file      SensorReport.cs
 *  \comment   from this table you have to produce a summary for every day on sensor utilization and 
 *             idling duration for a fleet
 *  \author    gb, 2006/07/06
 *
 */

using System;
using System.Data;
using VLF.CLS;
using VLF.CLS.Def;
using VLF.MAP;
using VLF.ERR;
using System.Data.SqlClient;	// for SqlException


namespace VLF.DAS.DB
{
	/// <summary>
	///      Generates sensors reporting functionality
	/// </summary>
   /// 
public class SensorReport
	{
      private static DateTime CT_SUMMARY_DATE = new DateTime(2000, 01, 01);  ///< used for markers of the summary of the data inside of the table
      private string connectionString = null;
      private DataTable tblSensorsUsage = null;
      private DataTable tblIdlingDuration = null;
      private DataSet ds = null;
		/// <summary>
		/// Constructor
		/// </summary>
      public SensorReport(string connectionString)
		{
         this.connectionString = connectionString;

         #region SensorsUtilization table definition
         // BoxId --- SensorId --- DateTime(Off) --- MinutesInUse  
         tblSensorsUsage = new DataTable("SensorsUtilization");
			tblSensorsUsage.Columns.Add("BoxId", typeof( string ) ) ;	
			tblSensorsUsage.Columns.Add("SensorId", typeof( int ) ) ;// this could be ignition on..off, idling on..off, other sensors on..off   
         tblSensorsUsage.Columns.Add("StartTime", typeof(DateTime));   // the first ON and last OFF message for the sensor 
         tblSensorsUsage.Columns.Add("EndTime", typeof(DateTime));     // the first ON and last OFF message for the sensor 
			tblSensorsUsage.Columns.Add("MinutesInUse", typeof(int) ) ; // how many minutes the device was in an ON..OFF cycle
//         tblSensorsUsage.Columns.Add("IsTrailer", typeof(bool)); // if it's trailer or not 
         #endregion

         #region Idling Duration table definition
         tblIdlingDuration = new DataTable("IdlingDuration");
         tblIdlingDuration.Columns.Add("BoxId", typeof(string));
         tblIdlingDuration.Columns.Add("StartTime", typeof(DateTime));
         tblIdlingDuration.Columns.Add("EndTime", typeof(DateTime));
         tblIdlingDuration.Columns.Add("MinutesInUse", typeof(int));
         #endregion
         ds = new DataSet();
         ds.Tables.Add(tblSensorsUsage);
         ds.Tables.Add(tblIdlingDuration);
      }

      /// <summary>
      ///      returns the tblSensorsUsage and  tblIdlingDuration in a dataset
      /// </summary>
      /// <returns></returns>
       
      public DataSet result
      {
         get
         {
            return ds ;
         }
      }

   /// <summary>
   ///      run sp_GetIdlingDurationForFleet2 which takes into consideration the user preferences 
   ///      returns all messages containing IDLE_DURATION keyword (or only specific messages type)
   /// </summary>
   /// <param name="fleetName"></param>
   /// <param name="dtFrom"></param>
   /// <param name="dtTo"></param>
   /// <returns>
   ///      [BoxId]
   ///      [OriginDateTime]
   ///      [BoxMsgInTypeId]
   ///      [CustomProp]
   /// </returns>
   public DataSet Exec_GetIdlingDurationForFleet(int fleetId, int userId, DateTime dtFrom, DateTime dtTo)
   {
      DataSet dsResult = null;
      try
      {
         using (SqlConnection conn = new SqlConnection(connectionString))
         {
            using (SqlCommand cmd = new SqlCommand("sp_GetIdlingDurationForFleet2", conn))
            {
               cmd.CommandTimeout = 600; 
               cmd.CommandType = CommandType.StoredProcedure;
               cmd.Parameters.Add("@fleetId", SqlDbType.Int);
               cmd.Parameters["@fleetId"].Value = fleetId;
               cmd.Parameters.Add("@userId", SqlDbType.Int);
               cmd.Parameters["@userId"].Value = userId;
               cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
               cmd.Parameters["@fromDate"].Value = dtFrom;
               cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
               cmd.Parameters["@toDate"].Value = dtTo;

               // Create a new adapter and initialize it with the new SQL command
               SqlDataAdapter sda = new SqlDataAdapter(cmd);
               // Create a new data set
               dsResult = new DataSet();

               // It is time to update the data set with information from the data adapter
               sda.Fill(dsResult, "FleetIdling");
            }
         }
      }
      catch (Exception exc)
      {
         Util.BTrace(Util.ERR2, "-- SensorReport.Exec_GetIdlingDurationForFleet -> EXC[{0}]", exc.Message);
      }

      return dsResult;
   }



   /// <summary>
   ///      returns all messages containing IDLE_DURATION in [CustomProp]
   /// </summary>
   /// <param name="fleetName"></param>
   /// <param name="dtFrom"></param>
   /// <param name="dtTo"></param>
   /// <returns>
   ///      [BoxId], [OriginDateTime], [BoxMsgInTypeId], [CustomProp]
   /// </returns>
   public DataSet Exec_GetIdlingMessagesForFleet(int fleetId, int userId, DateTime dtFrom, DateTime dtTo)
   {
       DataSet dsResult = null;
       try
       {
           using (SqlConnection conn = new SqlConnection(connectionString))
           {
               using (SqlCommand cmd = new SqlCommand("sp_GetIdlingMessagesForFleet", conn))
               {
                   cmd.CommandType = CommandType.StoredProcedure;
                   cmd.Parameters.Add("@fleetId", SqlDbType.Int);
                   cmd.Parameters["@fleetId"].Value = fleetId;
                   cmd.Parameters.Add("@userId", SqlDbType.Int);
                   cmd.Parameters["@userId"].Value = userId;
                   cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                   cmd.Parameters["@fromDate"].Value = dtFrom;
                   cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                   cmd.Parameters["@toDate"].Value = dtTo;

                   // Create a new adapter and initialize it with the new SQL command
                   SqlDataAdapter sda = new SqlDataAdapter(cmd);
                   // Create a new data set
                   dsResult = new DataSet();

                   // It is time to update the data set with information from the data adapter
                   sda.Fill(dsResult, "FleetIdling");
               }
           }
       }
       catch (Exception exc)
       {
           Util.BTrace(Util.ERR2, "-- SensorReport.Exec_GetIdlingDurationForFleet -> EXC[{0}]", exc.Message);
       }

       return dsResult;
   }

   /// <summary>
   ///      calling sp_sp_GetSensorsPerFleet2 @fleetId, @sensorId, @dtFrom, @dtTo
   ///      returns all messages within that datetime interval containing SENSOR_NUM=sensorId
   /// </summary>
   /// <param name="fleetId"></param>
   /// <param name="sensorId"></param>
   /// <param name="dtFrom"></param>
   /// <param name="dtTo"></param>
   /// <returns>
   ///      [BoxId]
   ///      [OriginDateTime]
   ///      [BoxMsgInTypeId]
   ///      [CustomProp]
   /// </returns>
   public DataSet Exec_GetSensorsPerFleet(int fleetId, int userId, int sensorId, DateTime dtFrom, DateTime dtTo) 
   {
      DataSet dsResult = null;
      try
      {
         using (SqlConnection conn = new SqlConnection(connectionString))
         {
            using (SqlCommand cmd = new SqlCommand("sp_GetSensorsPerFleet2", conn))
            {
               cmd.CommandTimeout = 600; 
               cmd.CommandType = CommandType.StoredProcedure;
               cmd.Parameters.Add("@fleetId", SqlDbType.Int);
               cmd.Parameters["@fleetId"].Value = fleetId;
               cmd.Parameters.Add("@userId", SqlDbType.Int);
               cmd.Parameters["@userId"].Value = userId;
               cmd.Parameters.Add("@sensorId", SqlDbType.Int);
               cmd.Parameters["@sensorId"].Value = sensorId;
               cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
               cmd.Parameters["@fromDate"].Value = dtFrom;
               cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
               cmd.Parameters["@toDate"].Value = dtTo;

               // Create a new adapter and initialize it with the new SQL command
               SqlDataAdapter sda = new SqlDataAdapter(cmd);
               // Create a new data set
               dsResult = new DataSet();

               // It is time to update the data set with information from the data adapter
               sda.Fill(dsResult, "FleetSensors");                 
            }
         }
      }
      catch (Exception exc)
      {
         Util.BTrace(Util.ERR2, "-- SensorReport.Exec_GetSensorsPerFleet -> EXC[{0}]", exc.Message);
      }

      return dsResult; 
   }


   public DataSet GetFuelConsumption(int fleetId,  DateTime dtFrom, DateTime dtTo, int userId)
   {
       DataSet dsResult = null;
       try
       {
           using (SqlConnection conn = new SqlConnection(connectionString))
           {
               using (SqlCommand cmd = new SqlCommand("GetFuelConsumption", conn))
               {
                   cmd.CommandTimeout = 600;
                   cmd.CommandType = CommandType.StoredProcedure;
                   cmd.Parameters.Add("@fleetId", SqlDbType.Int);
                   cmd.Parameters["@fleetId"].Value = fleetId;
                   cmd.Parameters.Add("@userId", SqlDbType.Int);
                   cmd.Parameters["@userId"].Value = userId;
                   cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                   cmd.Parameters["@fromDate"].Value = dtFrom;
                   cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                   cmd.Parameters["@toDate"].Value = dtTo;

                   // Create a new adapter and initialize it with the new SQL command
                   SqlDataAdapter sda = new SqlDataAdapter(cmd);
                   // Create a new data set
                   dsResult = new DataSet();

                   // It is time to update the data set with information from the data adapter
                   sda.Fill(dsResult, "FuelConsumption");
               }
           }
       }
       catch (Exception exc)
       {
           Util.BTrace(Util.ERR2, "-- SensorReport.GetFuelConsumption -> EXC[{0}]", exc.Message);
       }

       return dsResult;
   }


   public DataSet GetFuelConsumptionPerOrganization(int orgId, DateTime dtFrom, DateTime dtTo, int userId)
   {
       DataSet dsResult = null;
       try
       {
           using (SqlConnection conn = new SqlConnection(connectionString))
           {
               using (SqlCommand cmd = new SqlCommand("GetFuelConsumptionPerOrganization", conn))
               {
                   cmd.CommandTimeout = 600;
                   cmd.CommandType = CommandType.StoredProcedure;
                   cmd.Parameters.Add("@organizationId", SqlDbType.Int);
                   cmd.Parameters["@organizationId"].Value = orgId;
                   cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                   cmd.Parameters["@fromDate"].Value = dtFrom;
                   cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                   cmd.Parameters["@toDate"].Value = dtTo;
                   cmd.Parameters.Add("@userId", SqlDbType.Int);
                   cmd.Parameters["@userId"].Value = userId;

                   // Create a new adapter and initialize it with the new SQL command
                   SqlDataAdapter sda = new SqlDataAdapter(cmd);
                   // Create a new data set
                   dsResult = new DataSet();

                   // It is time to update the data set with information from the data adapter
                   sda.Fill(dsResult, "FuelConsumption");
               }
           }
       }
       catch (Exception exc)
       {
           Util.BTrace(Util.ERR2, "-- SensorReport.GetFuelConsumption -> EXC[{0}]", exc.Message);
       }

       return dsResult;
   }


   private void Add2TableSensorUsage( int boxId, int sensorId, DateTime firstDateTime, DateTime lastDateTime, int sum )
   {
         object[] objRow = new object[tblSensorsUsage.Columns.Count];
         objRow[0] = boxId.ToString();                // BoxId
	      objRow[1] = sensorId;                        // SensorId
         objRow[2] = firstDateTime;
         objRow[3] = lastDateTime;
         objRow[4] = sum ; 
         tblSensorsUsage.Rows.Add(objRow) ;
   }



   /** \comment  add one/multiple rows to the report 
*          1) the ON..OFF case in the same day
*            if firstDateTime.DayOfYear EQ lastDateTime then add one row  
*          2) the ON..OFF in different days
*            if firstDateTime.DayOfYear LT lastDateTime.DayOfYear
*                add multiple rows 
*/
   private void AddIdlingMessageRow(int boxId, DateTime firstDateTime, DateTime lastDateTime, int sum)
   {
       if (firstDateTime > lastDateTime)
       {
           Util.BTrace(Util.ERR1, "-- SensorReport.AddIdlingMessageRow -> Error box[{0}]  dtFirst[{2}] dtLast[{3}]",
              boxId,  firstDateTime, lastDateTime);
           return;
       }

       if (firstDateTime.DayOfYear == lastDateTime.DayOfYear ||
           firstDateTime == CT_SUMMARY_DATE)
       {
           // compute how many hours are in every day and add one row 
           // BoxId --- SensorId --- firstDateTime --- lastDateTime --- MinutesInUse  
           //    Add2TableSensorUsage(boxId, sensorId, firstDateTime, lastDateTime, sum);

           object[] objRow = new object[tblIdlingDuration.Columns.Count];
           objRow[0] = boxId.ToString();                // BoxId
           objRow[1] = firstDateTime;
           objRow[2] = lastDateTime;
           objRow[3] = sum;
           tblIdlingDuration.Rows.Add(objRow);

       }
       else
           if (firstDateTime.DayOfYear < lastDateTime.DayOfYear)
           {
               DateTime it = new DateTime(firstDateTime.Year, firstDateTime.Month, firstDateTime.Day, 23, 59, 0);
               /*
                           Add2TableSensorUsage(boxId, sensorId, firstDateTime, it,
                                                 sum + 60 * (it.Hour - firstDateTime.Hour) + (it.Minute - firstDateTime.Minute));
               */
               object[] objRow = new object[tblIdlingDuration.Columns.Count];
               objRow[0] = boxId.ToString();                // BoxId
               objRow[1] = firstDateTime;
               objRow[2] = it;
               objRow[3] = sum + 60 * (it.Hour - firstDateTime.Hour) + (it.Minute - firstDateTime.Minute);
               tblIdlingDuration.Rows.Add(objRow);

               // add the summary report
               objRow[1] = CT_SUMMARY_DATE;
               tblIdlingDuration.Rows.Add(objRow);
               it = it.AddHours(24);
               // iterate through all those days
               while (it.DayOfYear < lastDateTime.DayOfYear)
               {
                   /*
                                  Add2TableSensorUsage(boxId, sensorId, it, it.AddHours(24), 24*60 ) ;
                                  it = it.AddHours(24);
                   */
                   objRow = new object[tblIdlingDuration.Columns.Count];
                   objRow[0] = boxId.ToString();                // BoxId
                   objRow[1] = it;
                   it = it.AddHours(24);
                   objRow[2] = it;
                   objRow[3] = 24 * 60;
                   tblIdlingDuration.Rows.Add(objRow);

                   // add the summary report
                   objRow[1] = CT_SUMMARY_DATE;
                   tblIdlingDuration.Rows.Add(objRow);

               }

           }
   }


   /** \comment  add one/multiple rows to the report 
    *          1) the ON..OFF case in the same day
    *            if firstDateTime.DayOfYear EQ lastDateTime then add one row  
    *          2) the ON..OFF in different days
    *            if firstDateTime.DayOfYear LT lastDateTime.DayOfYear
    *                add multiple rows 
    */
   private void AddSensorRow(int boxId, int sensorId, DateTime firstDateTime, DateTime lastDateTime, int sum)
   {
      if (firstDateTime > lastDateTime)
      {
         Util.BTrace(Util.ERR1, "-- SensorReport.AddSensorRow -> Error box[{0}] sensor[{1}] dtFirst[{2}] dtLast[{3}]", 
            boxId, sensorId, firstDateTime, lastDateTime);
         return ;
      }

      if (firstDateTime.DayOfYear == lastDateTime.DayOfYear ||
          firstDateTime == CT_SUMMARY_DATE)
      {
         // compute how many hours are in every day and add one row 
         // BoxId --- SensorId --- firstDateTime --- lastDateTime --- MinutesInUse  
     //    Add2TableSensorUsage(boxId, sensorId, firstDateTime, lastDateTime, sum);

         object[] objRow = new object[tblSensorsUsage.Columns.Count];
         objRow[0] = boxId.ToString();                // BoxId
	      objRow[1] = sensorId;                        // SensorId
         objRow[2] = firstDateTime;
         objRow[3] = lastDateTime;
         objRow[4] = sum ; 
         tblSensorsUsage.Rows.Add(objRow) ;
 
      }
      else
         if (firstDateTime.DayOfYear < lastDateTime.DayOfYear)
         {
            DateTime it = new DateTime(firstDateTime.Year, firstDateTime.Month, firstDateTime.Day, 23, 59, 0 );
/*
            Add2TableSensorUsage(boxId, sensorId, firstDateTime, it,
                                  sum + 60 * (it.Hour - firstDateTime.Hour) + (it.Minute - firstDateTime.Minute));
*/
            object[] objRow = new object[tblSensorsUsage.Columns.Count];
            objRow[0] = boxId.ToString();                // BoxId
            objRow[1] = sensorId;                        // SensorId
            objRow[2] = firstDateTime;
            objRow[3] = it;
            objRow[4] = sum + 60 * (it.Hour - firstDateTime.Hour) + (it.Minute - firstDateTime.Minute);
            tblSensorsUsage.Rows.Add(objRow);

            // add the summary report
            objRow[2] = CT_SUMMARY_DATE;
            tblSensorsUsage.Rows.Add(objRow);
            it = it.AddHours(24);
            // iterate through all those days
            while (it.DayOfYear < lastDateTime.DayOfYear)
            {
/*
               Add2TableSensorUsage(boxId, sensorId, it, it.AddHours(24), 24*60 ) ;
               it = it.AddHours(24);
*/
               objRow = new object[tblSensorsUsage.Columns.Count];
               objRow[0] = boxId.ToString();                // BoxId
               objRow[1] = sensorId;                        // SensorId
               objRow[2] = it;
               it = it.AddHours(24);
               objRow[3] = it;
               objRow[4] = 24*60;
               tblSensorsUsage.Rows.Add(objRow);

               // add the summary report
               objRow[2] = CT_SUMMARY_DATE;
               tblSensorsUsage.Rows.Add(objRow);
  
            }
            
         }
   }




   /** \comment this fucntion is called when you have ON sensor in one day [lastDateTime]
    *           and OFF in another day [newDateTime]
    *       Steps:
    *           close the firstDateTime row
    *           fill all days but newDateTime with 100% utilization 
    *           
    * close the last row for a boxId ONLY when the row has the STATUS ON for sensorId
    *           the value added to the report is computed with a specific date in mind 
    *           here you have to fill also the [WorkingHours] and [MinutesInUse] field
    */
   private void CrossSensorRow(int boxId, int sensorId, 
                               DateTime firstDateTime,   ///< the first time of the day when the sensor was ON
                               DateTime lastDateTime,    ///< the last time when the sensor was ON
                               DateTime newDateTime,     ///< the date untile where you have to fill up with 100% utilization 
                               int sum,
                               bool isLast)
   {
      if (lastDateTime < firstDateTime)
      {
         Util.BTrace(Util.ERR1, "-- SensorReport.CrossSensorRow -> Error box[{0}] sensor[{1}] dtFirst[{2}] dtLast[{3}]", 
            boxId, sensorId, firstDateTime, lastDateTime);
         return ;
      }
      object[] objRow = null ;

      if (lastDateTime.DayOfYear == newDateTime.DayOfYear)
      {
         // this is the case when is the last record computed
         Util.BTrace(Util.ERR1, "-- SensorReport.CrossSensorRow -> Warning box[{0}] sensor[{1}] dtLastDate[{2}] dtNewDate[{3}] in the same day !!",
            boxId, sensorId, lastDateTime, newDateTime);

         sum +=  (newDateTime.Hour - lastDateTime.Hour) * 60 + (newDateTime.Minute - lastDateTime.Minute);
         objRow = new object[tblSensorsUsage.Columns.Count];
         objRow[0] = boxId.ToString();                // BoxId
         objRow[1] = sensorId;                        // SensorId
         objRow[2] = firstDateTime;
         objRow[3] = newDateTime;
         objRow[4] = sum;
         tblSensorsUsage.Rows.Add(objRow);
         return;
      }


      // compute how many hours are in every day and add one row 
      // BoxId --- SensorId --- firstDateTime --- lastDateTime --- MinutesInUse  
      objRow = new object[tblSensorsUsage.Columns.Count];
      objRow[0] = boxId.ToString();                // BoxId
      objRow[1] = sensorId;                        // SensorId
      objRow[2] = lastDateTime;
      DateTime dt = new DateTime(lastDateTime.Year, lastDateTime.Month, lastDateTime.Day).AddDays(1).AddMinutes(-1);
      if (dt > newDateTime)
         dt = newDateTime;

/*
      objRow[2] = firstDateTime;
      DateTime dt = new DateTime(firstDateTime.Year, firstDateTime.Month, firstDateTime.Day).AddDays(1).AddMinutes(-1);
      if (dt > newDateTime)
         dt = newDateTime;
 */ 
      objRow[3] = dt;                              // the midnight for lastDateTime
      TimeSpan span = dt.Subtract(firstDateTime);
      objRow[4] = sum + 60*span.Hours + span.Minutes; 
      tblSensorsUsage.Rows.Add(objRow) ;

      // filling intermediate days with 1440 minutes
      while (dt.DayOfYear < (newDateTime.DayOfYear - 1))
      {
         objRow = new object[tblSensorsUsage.Columns.Count];
         objRow[0] = boxId.ToString();                // BoxId
         objRow[1] = sensorId;                        // SensorId
         dt = dt.AddMinutes(1.00);
         objRow[2] = dt;
         dt = dt.AddDays(1);
         dt = dt.AddMinutes(-1.00);
         objRow[3] = dt;
         objRow[4] =  24*60;
         tblSensorsUsage.Rows.Add(objRow);

      }

      // isLast is showing that you don't have any other records except the upper limit of the query 
      if (isLast && 
          dt.DayOfYear < newDateTime.DayOfYear)
      {
         objRow = new object[tblSensorsUsage.Columns.Count];
         objRow[0] = boxId.ToString();                // BoxId
         objRow[1] = sensorId;                        // SensorId
         dt = dt.AddMinutes(1.00);
         objRow[2] = dt;
         dt = dt.AddDays(1);
         dt = dt.AddMinutes(-1.00);
         if (dt > newDateTime)
            dt = newDateTime;
         objRow[3] = dt;
         span = newDateTime.Subtract((DateTime)objRow[2]);
         objRow[4] = 60 * span.Hours + span.Minutes;
         tblSensorsUsage.Rows.Add(objRow);
      }

   }


   /** \comment this fucntion is called when you have ON sensor in one day [lastDateTime]
  *           and OFF in another day [newDateTime]
  *       Steps:
  *           close the firstDateTime row
  *           fill all days but newDateTime with 100% utilization 
  *           
  * close the last row for a boxId ONLY when the row has the STATUS ON for sensorId
  *           the value added to the report is computed with a specific date in mind 
  *           here you have to fill also the [WorkingHours] and [MinutesInUse] field
  */
   private void CrossIdlingMessageRow(int boxId, 
                               DateTime firstDateTime,   ///< the first time of the day when the sensor was ON
                               DateTime lastDateTime,    ///< the last time when the sensor was ON
                               DateTime newDateTime,     ///< the date untile where you have to fill up with 100% utilization 
                               int sum,
                               bool isLast)
   {
       if (lastDateTime < firstDateTime)
       {
           Util.BTrace(Util.ERR1, "-- SensorReport.CrossIdlingMessageRow -> Error box[{0}]  dtFirst[{2}] dtLast[{3}]",
              boxId,  firstDateTime, lastDateTime);
           return;
       }
       object[] objRow = null;

       if (lastDateTime.DayOfYear == newDateTime.DayOfYear)
       {
           // this is the case when is the last record computed
           Util.BTrace(Util.ERR1, "-- SensorReport.CrossIdlingMessageRow -> Warning box[{0}]  dtLastDate[{2}] dtNewDate[{3}] in the same day !!",
              boxId, lastDateTime, newDateTime);

           sum += (newDateTime.Hour - lastDateTime.Hour) * 60 + (newDateTime.Minute - lastDateTime.Minute);
           objRow = new object[tblIdlingDuration.Columns.Count];
           objRow[0] = boxId.ToString();                // BoxId
           objRow[1] = firstDateTime;
           objRow[2] = newDateTime;
           objRow[3] = sum;
           tblIdlingDuration.Rows.Add(objRow);
           return;
       }

       
       // compute how many hours are in every day and add one row 
       // BoxId --- SensorId --- firstDateTime --- lastDateTime --- MinutesInUse  
       objRow = new object[tblIdlingDuration.Columns.Count];
       objRow[0] = boxId.ToString();                // BoxId
       objRow[1] = lastDateTime;
       DateTime dt = new DateTime(lastDateTime.Year, lastDateTime.Month, lastDateTime.Day).AddDays(1).AddMinutes(-1);
       if (dt > newDateTime)
           dt = newDateTime;

       /*
             objRow[2] = firstDateTime;
             DateTime dt = new DateTime(firstDateTime.Year, firstDateTime.Month, firstDateTime.Day).AddDays(1).AddMinutes(-1);
             if (dt > newDateTime)
                dt = newDateTime;
        */
       objRow[2] = dt;                              // the midnight for lastDateTime
       TimeSpan span = dt.Subtract(firstDateTime);
       objRow[3] = sum + 60 * span.Hours + span.Minutes;
       tblIdlingDuration.Rows.Add(objRow);

       // filling intermediate days with 1440 minutes
       while (dt.DayOfYear < (newDateTime.DayOfYear - 1))
       {
           objRow = new object[tblIdlingDuration.Columns.Count];
           objRow[0] = boxId.ToString();                // BoxId
           dt = dt.AddMinutes(1.00);
           objRow[1] = dt;
           dt = dt.AddDays(1);
           dt = dt.AddMinutes(-1.00);
           objRow[2] = dt;
           objRow[3] = 24 * 60;
           tblIdlingDuration.Rows.Add(objRow);

       }

       // isLast is showing that you don't have any other records except the upper limit of the query 
       if (isLast &&
           dt.DayOfYear < newDateTime.DayOfYear)
       {
           objRow = new object[tblIdlingDuration.Columns.Count];
           objRow[0] = boxId.ToString();                // BoxId
           dt = dt.AddMinutes(1.00);
           objRow[1] = dt;
           dt = dt.AddDays(1);
           dt = dt.AddMinutes(-1.00);
           if (dt > newDateTime)
               dt = newDateTime;
           objRow[2] = dt;
           span = newDateTime.Subtract((DateTime)objRow[2]);
           objRow[3] = 60 * span.Hours + span.Minutes;
           tblIdlingDuration.Rows.Add(objRow);
       }

   }

   /** \fn     private void AddIdlingRow(int boxId, DateTime firstDateTime, DateTime lastDateTime, int duration)
    *  \brief  add a row in tblIdlingDuration table
    */
   private void AddIdlingRow(int boxId, DateTime firstDateTime, DateTime lastDateTime, int duration)
   {
      if (firstDateTime > lastDateTime)
      {
         Util.BTrace(Util.ERR1, "-- SensorReport.AddIdlingRow -> Error box[{0}] dtFirst[{1}] dtLast[{2}]",
            boxId, firstDateTime, lastDateTime);
         return;
      }

      if (firstDateTime.DayOfYear == lastDateTime.DayOfYear ||
          firstDateTime == CT_SUMMARY_DATE)
      {
         // compute how many hours are in every day and add one row 
         // BoxId --- SensorId --- firstDateTime --- lastDateTime --- MinutesInUse  

         object[] objRow = new object[tblIdlingDuration.Columns.Count];
         objRow[0] = boxId.ToString();                // BoxId
         objRow[1] = firstDateTime;
         objRow[2] = lastDateTime;
         objRow[3] = duration;
         tblIdlingDuration.Rows.Add(objRow);
      }
   }
   /// <summary>
   ///      rowData has the following fields
   ///               [vlfMsgInHst.BoxId], [vlfMsgInHst.OriginDateTime], 
   ///               [vlfMsgInHst.BoxMsgInTypeId],	[vlfMsgInHst.CustomProp] 
   /// </summary>
   /// <param name="firstDateTime_"></param>
   /// <param name="lastDateTime_"></param>
   /// <param name="rowData"> this is the data obtained after you run 
   ///   exec       sp_GetIdlingDurationForFleet @fleetName, @fromDate, @toDate
   /// </param>
   /// <comment>  
   ///   (gb) 2006/12/14   added the summary for every day (the sum of all idling periods ) with a date of 2000/01/01
   /// </comment>
   public void FillIdlingDuration( DateTime firstDateTime_,
                                   DateTime lastDateTime_,
                                   DataSet rowData)  
   {
      // calculating idling durations when you get idling messages
      DateTime currentDateTime;	// is the first time the sensor is ON
      DateTime firstDateTime = firstDateTime_, 
               prevDateTime = firstDateTime_;   // previous date
      int currentBoxId,
          prevBoxId = 0;		// previous boxid
                              // I suppose there is no boxid as 0 !!
      int currentSum = 0;	   // where you add the idling periods 
      int allIdling4Period = 0; // where you add all idling segments (for a box) between firstDateTime_ AND lastDateTime_

      // delete the previous data report 
      if (tblIdlingDuration.Rows.Count > 0)
         tblIdlingDuration.Rows.Clear();

      if (!Util.IsTable(rowData))
         Util.BTrace(Util.WARN1, "<< SensorReport.FillIdlingDuration -> empty/wrong arguments");
      

      foreach (DataRow row in rowData.Tables[0].Rows)
      {
         currentBoxId = Convert.ToInt32(row["BoxId"]);
         currentDateTime = Convert.ToDateTime(row["OriginDateTime"]);

         if (currentBoxId != prevBoxId)
         {
            if (0 != currentSum)  		// add the last record   
            {

               AddIdlingRow(prevBoxId, firstDateTime, prevDateTime, currentSum);
               // see comments
               allIdling4Period += currentSum;
               AddIdlingRow(prevBoxId, CT_SUMMARY_DATE, firstDateTime, allIdling4Period);
            }

            allIdling4Period = 0;
            // reinit currentSum and firstDateTime		
            currentSum = Convert.ToInt32(Util.PairFindValue(VLF.CLS.Def.Const.keyIdleDuration, row["CustomProp"].ToString().TrimEnd()));
            firstDateTime = currentDateTime;
         }
         else
         {
            // the same boxId in the same day
            if (currentDateTime.Day == prevDateTime.Day)
            {
               // add the duration 
               currentSum += Convert.ToInt32(Util.PairFindValue(VLF.CLS.Def.Const.keyIdleDuration, row["CustomProp"].ToString().TrimEnd()));
            }
            else
            {
               // different days, write if you have something
               if (0 != currentSum)
               {
                  allIdling4Period += currentSum;
                  AddIdlingRow(prevBoxId, firstDateTime, prevDateTime, currentSum);
               }

               currentSum = Convert.ToInt32(Util.PairFindValue(VLF.CLS.Def.Const.keyIdleDuration, row["CustomProp"].ToString().TrimEnd()));
               firstDateTime = currentDateTime;
            }
         }

         prevBoxId = currentBoxId;
         prevDateTime = currentDateTime;
      }

      if (0 != currentSum)
      {
         AddIdlingRow(prevBoxId, firstDateTime, prevDateTime, currentSum);
         // see comments
         allIdling4Period += currentSum;
         AddIdlingRow(prevBoxId, CT_SUMMARY_DATE, firstDateTime, allIdling4Period);
      }
   }



      /** \fn     public void FillFleetUtilization(int paramSensor, DateTime firstDateTime_, DateTime lastDateTime_, DataSet rowData)
       * 
       *  \brief  this function receives all messages between specific hours and calculates the utilization of the time
       *          based on the messages receives
       *          A normal protocol is like this:
       *             - the device is powered on    - MSG power on
       *             - the sensor X is powered on  - SENSOR status ON
       *             - the sensor X is powered off - the interval between SENSOR ON/OFF is added to a suplimentary column named UtilSensor
       *             - the device is powered off   - MSG power off and the interval between POWER ON/OFF is added to a suplimentary column named UtilPower
       *          Special cases:
       *             - the table is starting with MSG power off - the interval is computed from 0 A.M
       *             - the power off is received without SENSOR status off - it's possible ????
       *             - the difference between ON/OFF is longer than 24 hours - suplimentary rows are added with utilization 100%
       *          This function is producing a table like 
       *          BoxId --- DateTime(Off) --- SensorId --- MinutesInUse  
       *          This table contains for every box a graphic for every day when the sensors was between ON..OFF
       *          For daily reports you have to aggregate those rows in one with ALL sensors like
       *          BoxId --- SensorId --- Date --- MinutesInUse
       *          The general report will have for a given period
       *          BoxId --- Period --- SensorId --- MinutesInUse
       *  \comment rowData is the table returned after you called 
       *           sp_GetSensorsPerFleet2 @fleetName, @sensorId, @dateFrom, @dateTo
       *             having rows 
       *          [vlfMsgInHst.BoxId], [vlfMsgInHst.OriginDateTime], [vlfMsgInHst.BoxMsgInTypeId], 	
       *          [vlfMsgInHst.CustomProp]
       */
      public void FillFleetUtilization(int paramSensor,                 //< it's a mask of the sensors used in computations                            
                                  DateTime firstDateTime_,
                                  DateTime lastDateTime_,
                                  DataSet rowData)  //<information about every row with all data from MsgIn  
                                                   // filtered on specific sensor
                                                   // dataset grouped by boxid 
                                                   // and between time span (firstDateTime_, lastDateTime_ )
      {
         // check arguments
         if (!Util.IsTable(rowData)|| firstDateTime_ > lastDateTime_ )
               Util.BTrace(Util.WARN1, " << SensorReport.FillUtilization -> empty/wrong arguments") ;

         // delete the previous data report 
         if (tblSensorsUsage.Rows.Count > 0)            
            tblSensorsUsage.Rows.Clear();


         // first, the calculation of utilization is made on all messages 
         // for every day
         //       - for power : the utilization between ignition on/off
         //                if you find only off, from 0 a.m
         //                if you find only on during the day it's the beggining of calculation
         DateTime currentDateTime, firstDateTime = firstDateTime_;	 // is the first time the sensor is ON during the day, used mostly for IGNITION ON..OFF
         int currentBoxId = 0;
         DateTime prevDateTime = firstDateTime_;         
         int prevBoxId = 0;		// previous boxid
                                 // I suppose there is no boxid as 0 !!
         int currentSum = 0;	      // where you add the periods when the sensor was on
         
         /** 
          *  where you add all usage segments (for a box) between firstDateTime_ AND lastDateTime_ 
          *  is the summary for the whole period for sensor activity or idling activity 
          */ 
         int allUsage4Period = 0;   
         bool isOpen = false;	// keep score of transition from ON..OFF

         foreach(DataRow ittr in rowData.Tables[0].Rows)
         {   
            short i16 = Util.Field2Int16(ittr, "BoxMsgInTypeId", 0 ) ;
            Enums.MessageType msgType = (Enums.MessageType)i16 ;
            currentBoxId = Convert.ToInt32(ittr["BoxId"]);            // find boxid
            currentDateTime = Convert.ToDateTime(ittr["OriginDateTime"]);

            string sensorStatus = "";
            string customProp = "";

            Util.Field2String(ittr, "CustomProp", ref customProp, "");

#region Read Sensor Status		 
            switch (msgType)
            {
               // this is for chassis/trailers
               case Enums.MessageType.Status:
               {
	               sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keyPower, customProp);
#region ignored   
#if false
                  
                  if (isTrailer)
                  {
                     if (sensorStatus == Enums.PowerReason.Untethered.ToString())
                     {
                        if (isPowerOn)
                        {
                           // else compute the power on timespan between last value and this one
                           AddUtilRow(newBoxId, 3, lastDateTime, ittr);
                           isPowerOn = false;
                        }
                        else
                        {
                           // there is a case here when you got the first message for one box OFF
                           // and you have to add from firstDateTime_ to the currRowDateTime
                           if (lastDateTime == firstDateTime_)
                              AddUtilRow(newBoxId, 3, lastDateTime, ittr);
                           else
                           {
                              Util.BTrace(Util.ERR1, "-- SensorReport.FillUtilization -> 2 Tethered messages DateTime[{0}]", currRowDateTime);
                              lastDateTime = currRowDateTime;
                           }
                        }
                     }
                     else if (sensorStatus == Enums.PowerReason.Tethered.ToString())
                     {
                        if (isPowerOn)
                           Util.BTrace(Util.ERR1, "-- SensorReport.FillUtilization -> 2 Tethered messages DateTime[{0}]", currRowDateTime);
                        else
                           // a new interval with the same boxId                       
                           isPowerOn = true;

                        lastDateTime = currRowDateTime;
                     }
                  }
#endif 
#endregion
                  break;
               }
               case Enums.MessageType.Sensor:
               case Enums.MessageType.SensorExtended:
               {
                  int sensorId = Convert.ToInt32(Util.PairFindValue(VLF.CLS.Def.Const.keySensorNum, customProp));
                  sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
                  break;
               }
            }
#endregion 

//            Util.BTrace(Util.INF1, "-- SensorReport.FillUtilization -> boxId[{0}] DateTime[{1}] SensorStatus[{2}]", currentBoxId, currentDateTime, sensorStatus);

            if (currentBoxId != prevBoxId && 0 != prevBoxId)   // start of a detailed utilization for a new box
            {
               // close the report for the previous box 
               // when the last messages were xxxON, otherwise do nothing 
               if (!isOpen )		// clean stop		 
               {
                  // start a new row, fill the row for the prevBoxId
                  AddSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, currentSum);
                  allUsage4Period += currentSum;
                  AddSensorRow(prevBoxId, paramSensor, CT_SUMMARY_DATE, prevDateTime, allUsage4Period);     // close the report on that day !!!
               }
               else
               {
                  // the last message was open wo/ close so I fill up till _lastDateTime
                  // this is a very interesting case !!!
                  if (lastDateTime_.DayOfYear == prevDateTime.DayOfYear)
                  {
                     currentSum += (lastDateTime_.Hour - prevDateTime.Hour) * 60 +
                                   (lastDateTime_.Minute - prevDateTime.Minute);
                     Util.BTrace(Util.WARN1, "-- SensorReport.FillUtilization -> boxId[{0}] FIRST_ON[{1}] LAST_ON[{2}] and not OFF[{3}] duration[{4}]",
                                    prevBoxId, firstDateTime, prevDateTime, lastDateTime_, currentSum);
                     //                  AddSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, currentSum);     // close the report on that day !!!
                     AddSensorRow(prevBoxId, paramSensor, firstDateTime /*prevDateTime */, lastDateTime_, currentSum);     // close the report on that day !!!

                     allUsage4Period += currentSum;
                     AddSensorRow(prevBoxId, paramSensor, CT_SUMMARY_DATE, firstDateTime, allUsage4Period);
                  }
                  else
                  {
                     //it was ON and OFF is in another day or not found within the interval
                     // adding an activity across multiple days   
                     CrossSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, lastDateTime_, currentSum, true);
                     allUsage4Period += currentSum;
                     allUsage4Period += (lastDateTime_.DayOfYear - prevDateTime.DayOfYear) * 1440 +
                                       (lastDateTime_.Hour - prevDateTime.Hour) * 60 +
                                       (lastDateTime_.Minute - prevDateTime.Minute);
                     AddSensorRow(prevBoxId, paramSensor, CT_SUMMARY_DATE, firstDateTime, allUsage4Period);
                  }
               }

               // reset the values for prevDateTime, prevBoxId, currentSum
               // here you have an interesting situation
               isOpen = (sensorStatus == Const.valON);
               prevBoxId = currentBoxId;
               currentSum = 0;
               allUsage4Period = 0;
               if (isOpen)
               {
                  firstDateTime = prevDateTime = currentDateTime;
               }
               else
               {
                  // you started with an OFF message, so you assume that the period from the beggining of the query
                  // until currentDateTime you've been ON
                  firstDateTime = firstDateTime_;
                  if (firstDateTime.DayOfYear != currentDateTime.DayOfYear)
                  {
                     DateTime previousDay = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day , 0, 0, 0);  // the night previous day
                     CrossSensorRow(prevBoxId, paramSensor, firstDateTime, firstDateTime, previousDay, 0, false);
                     allUsage4Period += (previousDay.DayOfYear - firstDateTime.DayOfYear) * 1440 +
                                       (previousDay.Hour - firstDateTime.Hour) * 60 +
                                       (previousDay.Minute - firstDateTime.Minute);
                     firstDateTime = prevDateTime = previousDay ;
                  }
                  else
                  {
                     prevDateTime = firstDateTime_;
                  }

                  currentSum += (currentDateTime.Hour - prevDateTime.Hour) * 60 +
                                (currentDateTime.Minute - prevDateTime.Minute);

//                  AddSensorRow(prevBoxId, paramSensor, prevDateTime, currentDateTime, currentSum);
//                  allUsage4Period += ((currentDateTime.DayOfYear - firstDateTime_.DayOfYear) * 1440 + currentSum);
               }
            }
            else
            {
               // the same boxId
               if (prevDateTime.DayOfYear == currentDateTime.DayOfYear)
               {
                  if (isOpen)
                  {
                     if (sensorStatus == Const.valOFF)	// add the amount to the MinutesInUse		
                     {
                        currentSum += (currentDateTime.Hour - prevDateTime.Hour) * 60 + 
                                      (currentDateTime.Minute - prevDateTime.Minute);
                        isOpen = false;
                     }
                     else
                     {
                        // two messages ON, the previous one is lost 
                        Util.BTrace(Util.ERR1, "-- SensorReport.FillUtilization -> ON..ON Messages in the SAME day boxId[{0}] FIRST_ON[{1}] NEXT_ON[{2}] sum[{3}]", 
                                    prevBoxId, prevDateTime, currentDateTime, currentSum );

                        currentSum += (currentDateTime.Hour - prevDateTime.Hour) * 60 +
                                      (currentDateTime.Minute - prevDateTime.Minute);
/*                        if (0 == currentSum)
                           firstDateTime = currentDateTime;
 */ 
                     }
                  }
                  else
                  {
                     // SENSOR was OFF
                     if (sensorStatus == Const.valON)
                     {
                        isOpen = true;
                        if (0 == prevBoxId)
                           firstDateTime = currentDateTime;
/*
                        else
                           // start of the day 
                           firstDateTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day) ; 
*/
                     }
                     else
                     {
                        // two messages OFF, the previous one is lost 
                        Util.BTrace(Util.ERR1, "-- SensorReport.FillUtilization -> OFF..OFF Messages in the SAME day boxId[{0}] FIRST_OFF[{1}] NEXT_OFF[{2}] sum[{3}]", 
                                    prevBoxId, prevDateTime, currentDateTime, currentSum );
                     }
                  }
               }
               else
               {
                  // the same boxId but different days 
                  // write the row and reinit just currentSum,
                  if (isOpen)
                  {
                     if (sensorStatus == Const.valOFF)	// add the amount to the MinutesInUse		
                     {
                        Util.BTrace(Util.WARN0, "-- SensorReport.FillUtilization -> ON..OFF Messages in DIFFERENT days boxId[{0}] FIRST_ON[{1}] NEXT_OFF[{2}] sum[{3}]", 
                                    prevBoxId, firstDateTime, currentDateTime, currentSum );

                        DateTime previousDay = currentDateTime.AddDays(-1).AddHours(23 - currentDateTime.Hour).AddMinutes(59 - currentDateTime.Minute); //  .Add(new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day - 1, 23, 59, 01);  // the night previous day
//                        DateTime previousDay = currentDateTime.AddDays(-1).AddHours(23-currentDateTime.Hour).AddMinutes(59-currentDateTime.Minute); //  .Add(new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day - 1, 23, 59, 01);  // the night previous day
//                        previousDay.Hour = 23; previousDay.Minute = 59; previousDay.Second = 1;


                        // adding an activity across multiple days   
                        CrossSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, previousDay, currentSum, false);

                        firstDateTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0) ;

                        allUsage4Period += (currentSum +
                                           (previousDay.DayOfYear - prevDateTime.DayOfYear) * 1440 +
                                           (previousDay.Hour - prevDateTime.Hour) * 60 +
                                           (previousDay.Minute - prevDateTime.Minute));

                        currentSum = currentDateTime.Hour*60 + currentDateTime.Minute;                          
                        isOpen = false;
                     }
                     else
                     {
                        // two messages ON, the previous one is lost
                        // here you could close the day and start a day 
                        Util.BTrace(Util.ERR1, "-- SensorReport.FillUtilization -> ON Messages in DIFFERENT days boxId[{0}] FIRST_ON[{1}] NEXT_ON[{2}] sum[{3}]", 
                                    prevBoxId, firstDateTime, currentDateTime, currentSum );
                        AddSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, currentSum);
                        firstDateTime = prevDateTime = currentDateTime;
                        allUsage4Period += currentSum;
                        prevBoxId = currentBoxId;
                        currentSum = 0;
                        isOpen = (sensorStatus == Const.valON);                        
                     }                       
                  }
                  else // 
                  {
                     // SENSOR was OFF on different days
                     if (sensorStatus == Const.valON)	// add the amount to the MinutesInUse		
                     {
                        // start a new row, fill the row for the prevBoxId
                        AddSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, currentSum);
                        allUsage4Period += currentSum;
                        isOpen = true;
                        currentSum = 0 ;
                     }
                     else
                     {
                        // two messages OFF, on two different days 
                        // the previous one is lost 
                        Util.BTrace(Util.ERR1, "-- SensorReport.FillUtilization -> OFF Messages in DIFFERENT days boxId[{0}] FIRST_OFF[{1}] NEXT_OFF[{2}] sum[{3}]", 
                                    prevBoxId, prevDateTime, currentDateTime, currentSum );

                        // start a new row, fill the row for the prevBoxId
                        AddSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, currentSum);
                        allUsage4Period += currentSum;
                        currentSum = 0;
                     }

                     firstDateTime = currentDateTime ;
                  } // if ( isOpen )			
               } // if ( prevDateTime.Day == 
            } //  if ( currentBoxId != 

            prevDateTime = currentDateTime;
            prevBoxId = currentBoxId;
	  
	      } // foreach

         // if you still have the last box with a sensor open you have to close the loop 
         if (isOpen)
         {            
            CrossSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, lastDateTime_, currentSum, true);
            allUsage4Period += (currentSum +
                              (lastDateTime_.DayOfYear - prevDateTime.DayOfYear) * 1440 +
                              (lastDateTime_.Hour - prevDateTime.Hour) * 60 +
                              (lastDateTime_.Minute - prevDateTime.Minute));
         }
         else
            if (0 != currentSum)
            {
               AddSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, currentSum);
               allUsage4Period += currentSum;
            }

         if ( allUsage4Period != 0 )
            AddSensorRow(prevBoxId, paramSensor, CT_SUMMARY_DATE, firstDateTime, allUsage4Period);

      }




      public void FillFleetIdlingMessages(DateTime firstDateTime_,
                              DateTime lastDateTime_,
                              DataSet rowData)  //<information about every row with all data from MsgIn  
      // filtered on specific sensor
      // dataset grouped by boxid 
      // and between time span (firstDateTime_, lastDateTime_ )
      {
          // check arguments
          if (!Util.IsTable(rowData) || firstDateTime_ > lastDateTime_)
              Util.BTrace(Util.WARN1, " << SensorReport.FillFleetIdlingMessages -> empty/wrong arguments");

          // delete the previous data report 
          if (tblIdlingDuration.Rows.Count > 0)
              tblIdlingDuration.Rows.Clear();

          // first, the calculation of utilization is made on all messages 
          // for every day
          //       - for power : the utilization between ignition on/off
          //                if you find only off, from 0 a.m
          //                if you find only on during the day it's the beggining of calculation
          DateTime currentDateTime, firstDateTime = firstDateTime_;	 // is the first time the sensor is ON during the day, used mostly for IGNITION ON..OFF
          int currentBoxId = 0;
          DateTime prevDateTime = firstDateTime_;
          int prevBoxId = 0;		// previous boxid
          // I suppose there is no boxid as 0 !!
          int currentSum = 0;	      // where you add the periods when the sensor was on
        
          /** 
           *  where you add all usage segments (for a box) between firstDateTime_ AND lastDateTime_ 
           *  is the summary for the whole period for sensor activity or idling activity 
           */
          int allUsage4Period = 0;
          bool isOpen = false;	// keep score of transition from ON..OFF

          foreach (DataRow ittr in rowData.Tables[0].Rows)
          {
              short i16 = Util.Field2Int16(ittr, "BoxMsgInTypeId", 0);
              Enums.MessageType msgType = (Enums.MessageType)i16;
              currentBoxId = Convert.ToInt32(ittr["BoxId"]);            // find boxid
              currentDateTime = Convert.ToDateTime(ittr["OriginDateTime"]);

              string sensorStatus = "";
              string customProp = "";

              Util.Field2String(ittr, "CustomProp", ref customProp, "");

              sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keyParamValue, ittr["status"].ToString());
        

              if (currentBoxId != prevBoxId && 0 != prevBoxId)   // start of a detailed utilization for a new box
              {
                  // close the report for the previous box 
                  // when the last messages were xxxON, otherwise do nothing 
                  if (!isOpen)		// clean stop		 
                  {
                      // start a new row, fill the row for the prevBoxId
                      AddIdlingMessageRow (prevBoxId, firstDateTime, prevDateTime, currentSum);
                      allUsage4Period += currentSum;
                      AddIdlingMessageRow(prevBoxId,  CT_SUMMARY_DATE, prevDateTime, allUsage4Period);     // close the report on that day !!!
                  }
                  else
                  {
                      // the last message was open wo/ close so I fill up till _lastDateTime
                      // this is a very interesting case !!!
                      if (lastDateTime_.DayOfYear == prevDateTime.DayOfYear)
                      {
                          currentSum += (lastDateTime_.Hour - prevDateTime.Hour) * 60 +
                                        (lastDateTime_.Minute - prevDateTime.Minute);
                          Util.BTrace(Util.WARN1, "-- SensorReport.FillFleetIdlingMessages -> boxId[{0}] FIRST_ON[{1}] LAST_ON[{2}] and not OFF[{3}] duration[{4}]",
                                         prevBoxId, firstDateTime, prevDateTime, lastDateTime_, currentSum);
                          //                  AddSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, currentSum);     // close the report on that day !!!
                          AddIdlingMessageRow(prevBoxId, firstDateTime /*prevDateTime */, lastDateTime_, currentSum);     // close the report on that day !!!

                          allUsage4Period += currentSum;
                          AddIdlingMessageRow(prevBoxId,  CT_SUMMARY_DATE, firstDateTime, allUsage4Period);
                      }
                      else
                      {
                          //it was ON and OFF is in another day or not found within the interval
                          // adding an activity across multiple days   
                          CrossIdlingMessageRow(prevBoxId, firstDateTime, prevDateTime, lastDateTime_, currentSum, true);
                          allUsage4Period += currentSum;
                          allUsage4Period += (lastDateTime_.DayOfYear - prevDateTime.DayOfYear) * 1440 +
                                            (lastDateTime_.Hour - prevDateTime.Hour) * 60 +
                                            (lastDateTime_.Minute - prevDateTime.Minute);
                          AddIdlingMessageRow(prevBoxId, CT_SUMMARY_DATE, firstDateTime, allUsage4Period);
                      }
                  }

                  // reset the values for prevDateTime, prevBoxId, currentSum
                  // here you have an interesting situation
                  isOpen = (sensorStatus == Const.valON);
                  prevBoxId = currentBoxId;
                  currentSum = 0;
                  allUsage4Period = 0;
                  if (isOpen)
                  {
                      firstDateTime = prevDateTime = currentDateTime;
                  }
                  else
                  {
                      // you started with an OFF message, so you assume that the period from the beggining of the query
                      // until currentDateTime you've been ON
                      firstDateTime = firstDateTime_;
                      if (firstDateTime.DayOfYear != currentDateTime.DayOfYear)
                      {
                          DateTime previousDay = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0);  // the night previous day
                          CrossIdlingMessageRow(prevBoxId, firstDateTime, firstDateTime, previousDay, 0, false);
                          allUsage4Period += (previousDay.DayOfYear - firstDateTime.DayOfYear) * 1440 +
                                            (previousDay.Hour - firstDateTime.Hour) * 60 +
                                            (previousDay.Minute - firstDateTime.Minute);
                          firstDateTime = prevDateTime = previousDay;
                      }
                      else
                      {
                          prevDateTime = firstDateTime_;
                      }

                      currentSum += (currentDateTime.Hour - prevDateTime.Hour) * 60 +
                                    (currentDateTime.Minute - prevDateTime.Minute);

                      //                  AddSensorRow(prevBoxId, paramSensor, prevDateTime, currentDateTime, currentSum);
                      //                  allUsage4Period += ((currentDateTime.DayOfYear - firstDateTime_.DayOfYear) * 1440 + currentSum);
                  }
              }
              else
              {
                  // the same boxId
                  if (prevDateTime.DayOfYear == currentDateTime.DayOfYear)
                  {
                      if (isOpen)
                      {
                          if (sensorStatus == Const.valOFF)	// add the amount to the MinutesInUse		
                          {
                              currentSum += (currentDateTime.Hour - prevDateTime.Hour) * 60 +
                                            (currentDateTime.Minute - prevDateTime.Minute);
                              isOpen = false;
                          }
                          else
                          {
                              // two messages ON, the previous one is lost 
                              Util.BTrace(Util.ERR1, "-- SensorReport.FillFleetIdlingMessages -> ON..ON Messages in the SAME day boxId[{0}] FIRST_ON[{1}] NEXT_ON[{2}] sum[{3}]",
                                          prevBoxId, prevDateTime, currentDateTime, currentSum);

                              currentSum += (currentDateTime.Hour - prevDateTime.Hour) * 60 +
                                            (currentDateTime.Minute - prevDateTime.Minute);
                              /*                        if (0 == currentSum)
                                                         firstDateTime = currentDateTime;
                               */
                          }
                      }
                      else
                      {
                          // SENSOR was OFF
                          if (sensorStatus == Const.valON)
                          {
                              isOpen = true;
                              if (0 == prevBoxId)
                                  firstDateTime = currentDateTime;
                              /*
                                                      else
                                                         // start of the day 
                                                         firstDateTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day) ; 
                              */
                          }
                          else
                          {
                              // two messages OFF, the previous one is lost 
                              Util.BTrace(Util.ERR1, "-- SensorReport.FillFleetIdlingMessages -> OFF..OFF Messages in the SAME day boxId[{0}] FIRST_OFF[{1}] NEXT_OFF[{2}] sum[{3}]",
                                          prevBoxId, prevDateTime, currentDateTime, currentSum);
                          }
                      }
                  }
                  else
                  {
                      // the same boxId but different days 
                      // write the row and reinit just currentSum,
                      if (isOpen)
                      {
                          if (sensorStatus == Const.valOFF)	// add the amount to the MinutesInUse		
                          {
                              Util.BTrace(Util.WARN0, "-- SensorReport.FillFleetIdlingMessages -> ON..OFF Messages in DIFFERENT days boxId[{0}] FIRST_ON[{1}] NEXT_OFF[{2}] sum[{3}]",
                                          prevBoxId, firstDateTime, currentDateTime, currentSum);

                              DateTime previousDay = currentDateTime.AddDays(-1).AddHours(23 - currentDateTime.Hour).AddMinutes(59 - currentDateTime.Minute); //  .Add(new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day - 1, 23, 59, 01);  // the night previous day
                              //                        DateTime previousDay = currentDateTime.AddDays(-1).AddHours(23-currentDateTime.Hour).AddMinutes(59-currentDateTime.Minute); //  .Add(new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day - 1, 23, 59, 01);  // the night previous day
                              //                        previousDay.Hour = 23; previousDay.Minute = 59; previousDay.Second = 1;


                              // adding an activity across multiple days   
                              CrossIdlingMessageRow(prevBoxId, firstDateTime, prevDateTime, previousDay, currentSum, false);

                              firstDateTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0);

                              allUsage4Period += (currentSum +
                                                 (previousDay.DayOfYear - prevDateTime.DayOfYear) * 1440 +
                                                 (previousDay.Hour - prevDateTime.Hour) * 60 +
                                                 (previousDay.Minute - prevDateTime.Minute));

                              currentSum = currentDateTime.Hour * 60 + currentDateTime.Minute;
                              isOpen = false;
                          }
                          else
                          {
                              // two messages ON, the previous one is lost
                              // here you could close the day and start a day 
                              Util.BTrace(Util.ERR1, "-- SensorReport.FillFleetIdlingMessages -> ON Messages in DIFFERENT days boxId[{0}] FIRST_ON[{1}] NEXT_ON[{2}] sum[{3}]",
                                          prevBoxId, firstDateTime, currentDateTime, currentSum);
                              AddIdlingMessageRow (prevBoxId, firstDateTime, prevDateTime, currentSum);
                              firstDateTime = prevDateTime = currentDateTime;
                              allUsage4Period += currentSum;
                              prevBoxId = currentBoxId;
                              currentSum = 0;
                              isOpen = (sensorStatus == Const.valON);
                          }
                      }
                      else // 
                      {
                          // SENSOR was OFF on different days
                          if (sensorStatus == Const.valON)	// add the amount to the MinutesInUse		
                          {
                              // start a new row, fill the row for the prevBoxId
                              AddIdlingMessageRow(prevBoxId, firstDateTime, prevDateTime, currentSum);
                              allUsage4Period += currentSum;
                              isOpen = true;
                              currentSum = 0;
                          }
                          else
                          {
                              // two messages OFF, on two different days 
                              // the previous one is lost 
                              Util.BTrace(Util.ERR1, "-- SensorReport.FillFleetIdlingMessages -> OFF Messages in DIFFERENT days boxId[{0}] FIRST_OFF[{1}] NEXT_OFF[{2}] sum[{3}]",
                                          prevBoxId, prevDateTime, currentDateTime, currentSum);

                              // start a new row, fill the row for the prevBoxId
                              AddIdlingMessageRow(prevBoxId, firstDateTime, prevDateTime, currentSum);
                              allUsage4Period += currentSum;
                              currentSum = 0;
                          }

                          firstDateTime = currentDateTime;
                      } // if ( isOpen )			
                  } // if ( prevDateTime.Day == 
              } //  if ( currentBoxId != 

              prevDateTime = currentDateTime;
              prevBoxId = currentBoxId;

          } // foreach

          // if you still have the last box with a sensor open you have to close the loop 
          if (isOpen)
          {
              CrossIdlingMessageRow(prevBoxId, firstDateTime, prevDateTime, lastDateTime_, currentSum, true);
              allUsage4Period += (currentSum +
                                (lastDateTime_.DayOfYear - prevDateTime.DayOfYear) * 1440 +
                                (lastDateTime_.Hour - prevDateTime.Hour) * 60 +
                                (lastDateTime_.Minute - prevDateTime.Minute));
          }
          else
              if (0 != currentSum)
              {
                  AddIdlingMessageRow(prevBoxId, firstDateTime, prevDateTime, currentSum);
                  allUsage4Period += currentSum;
              }

          if (allUsage4Period != 0)
              AddIdlingMessageRow(prevBoxId, CT_SUMMARY_DATE, firstDateTime, allUsage4Period);

      }
    

   public void FillFleetUtilization(DateTime firstDateTime_,
                                  DateTime lastDateTime_,
                                  DataSet rowData)  //<information about every row with all data from MsgIn  
                                                   // filtered on specific sensor
                                                   // dataset grouped by boxid 
                                                   // and between time span (firstDateTime_, lastDateTime_ )
      {
         // check arguments
         if (!Util.IsTable(rowData)|| firstDateTime_ > lastDateTime_ )
               Util.BTrace(Util.WARN1, " << SensorReport.FillUtilization -> empty/wrong arguments") ;

         // delete the previous data report 
         if (tblSensorsUsage.Rows.Count > 0)            
            tblSensorsUsage.Rows.Clear();


         // first, the calculation of utilization is made on all messages 
         // for every day
         //       - for power : the utilization between ignition on/off
         //                if you find only off, from 0 a.m
         //                if you find only on during the day it's the beggining of calculation
         DateTime currentDateTime, firstDateTime = firstDateTime_;	 // is the first time the sensor is ON during the day, used mostly for IGNITION ON..OFF
         int currentBoxId = 0;
         DateTime prevDateTime = firstDateTime_;         
         int prevBoxId = 0;		// previous boxid
                                 // I suppose there is no boxid as 0 !!
         int currentSum = 0;	      // where you add the periods when the sensor was on
         
         /** 
          *  where you add all usage segments (for a box) between firstDateTime_ AND lastDateTime_ 
          *  is the summary for the whole period for sensor activity or idling activity 
          */ 
         int allUsage4Period = 0;   
         bool isOpen = false;	// keep score of transition from ON..OFF
         int paramSensor = 0;

         foreach(DataRow ittr in rowData.Tables[0].Rows)
         {   
            short i16 = Util.Field2Int16(ittr, "BoxMsgInTypeId", 0 ) ;
            Enums.MessageType msgType = (Enums.MessageType)i16 ;
            currentBoxId = Convert.ToInt32(ittr["BoxId"]);            // find boxid
            currentDateTime = Convert.ToDateTime(ittr["OriginDateTime"]);

            string sensorStatus = "";
            string customProp = "";

            Util.Field2String(ittr, "CustomProp", ref customProp, "");

#region Read Sensor Status		 
            switch (msgType)
            {
               // this is for chassis/trailers
               case Enums.MessageType.Status:
               {
	               sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keyPower, customProp);
#region ignored   
#if false
                  
                  if (isTrailer)
                  {
                     if (sensorStatus == Enums.PowerReason.Untethered.ToString())
                     {
                        if (isPowerOn)
                        {
                           // else compute the power on timespan between last value and this one
                           AddUtilRow(newBoxId, 3, lastDateTime, ittr);
                           isPowerOn = false;
                        }
                        else
                        {
                           // there is a case here when you got the first message for one box OFF
                           // and you have to add from firstDateTime_ to the currRowDateTime
                           if (lastDateTime == firstDateTime_)
                              AddUtilRow(newBoxId, 3, lastDateTime, ittr);
                           else
                           {
                              Util.BTrace(Util.ERR1, "-- SensorReport.FillUtilization -> 2 Tethered messages DateTime[{0}]", currRowDateTime);
                              lastDateTime = currRowDateTime;
                           }
                        }
                     }
                     else if (sensorStatus == Enums.PowerReason.Tethered.ToString())
                     {
                        if (isPowerOn)
                           Util.BTrace(Util.ERR1, "-- SensorReport.FillUtilization -> 2 Tethered messages DateTime[{0}]", currRowDateTime);
                        else
                           // a new interval with the same boxId                       
                           isPowerOn = true;

                        lastDateTime = currRowDateTime;
                     }
                  }
#endif 
#endregion
                  break;
               }
               case Enums.MessageType.Sensor:
               case Enums.MessageType.SensorExtended:
               {
                  int sensorId = Convert.ToInt32(Util.PairFindValue(VLF.CLS.Def.Const.keySensorNum, customProp));
                  sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
                  break;
               }
            }
#endregion 

//            Util.BTrace(Util.INF1, "-- SensorReport.FillUtilization -> boxId[{0}] DateTime[{1}] SensorStatus[{2}]", currentBoxId, currentDateTime, sensorStatus);

            if (currentBoxId != prevBoxId && 0 != prevBoxId)   // start of a detailed utilization for a new box
            {
               // close the report for the previous box 
               // when the last messages were xxxON, otherwise do nothing 
               if (!isOpen )		// clean stop		 
               {
                  // start a new row, fill the row for the prevBoxId
                  AddSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, currentSum);
                  allUsage4Period += currentSum;
                  AddSensorRow(prevBoxId, paramSensor, CT_SUMMARY_DATE, prevDateTime, allUsage4Period);     // close the report on that day !!!
               }
               else
               {
                  // the last message was open wo/ close so I fill up till _lastDateTime
                  // this is a very interesting case !!!
                  if (lastDateTime_.DayOfYear == prevDateTime.DayOfYear)
                  {
                     currentSum += (lastDateTime_.Hour - prevDateTime.Hour) * 60 +
                                   (lastDateTime_.Minute - prevDateTime.Minute);
                     Util.BTrace(Util.WARN1, "-- SensorReport.FillUtilization -> boxId[{0}] FIRST_ON[{1}] LAST_ON[{2}] and not OFF[{3}] duration[{4}]",
                                    prevBoxId, firstDateTime, prevDateTime, lastDateTime_, currentSum);
                     //                  AddSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, currentSum);     // close the report on that day !!!
                     AddSensorRow(prevBoxId, paramSensor, firstDateTime /*prevDateTime */, lastDateTime_, currentSum);     // close the report on that day !!!

                     allUsage4Period += currentSum;
                     AddSensorRow(prevBoxId, paramSensor, CT_SUMMARY_DATE, firstDateTime, allUsage4Period);
                  }
                  else
                  {
                     //it was ON and OFF is in another day or not found within the interval
                     // adding an activity across multiple days   
                     CrossSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, lastDateTime_, currentSum, true);
                     allUsage4Period += currentSum;
                     allUsage4Period += (lastDateTime_.DayOfYear - prevDateTime.DayOfYear) * 1440 +
                                       (lastDateTime_.Hour - prevDateTime.Hour) * 60 +
                                       (lastDateTime_.Minute - prevDateTime.Minute);
                     AddSensorRow(prevBoxId, paramSensor, CT_SUMMARY_DATE, firstDateTime, allUsage4Period);
                  }
               }

               // reset the values for prevDateTime, prevBoxId, currentSum
               // here you have an interesting situation
               isOpen = (sensorStatus == Const.valON);
               prevBoxId = currentBoxId;
               currentSum = 0;
               allUsage4Period = 0;
               if (isOpen)
               {
                  firstDateTime = prevDateTime = currentDateTime;
               }
               else
               {
                  // you started with an OFF message, so you assume that the period from the beggining of the query
                  // until currentDateTime you've been ON
                  firstDateTime = firstDateTime_;
                  if (firstDateTime.DayOfYear != currentDateTime.DayOfYear)
                  {
                     DateTime previousDay = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day , 0, 0, 0);  // the night previous day
                     CrossSensorRow(prevBoxId, paramSensor, firstDateTime, firstDateTime, previousDay, 0, false);
                     allUsage4Period += (previousDay.DayOfYear - firstDateTime.DayOfYear) * 1440 +
                                       (previousDay.Hour - firstDateTime.Hour) * 60 +
                                       (previousDay.Minute - firstDateTime.Minute);
                     firstDateTime = prevDateTime = previousDay ;
                  }
                  else
                  {
                     prevDateTime = firstDateTime_;
                  }

                  currentSum += (currentDateTime.Hour - prevDateTime.Hour) * 60 +
                                (currentDateTime.Minute - prevDateTime.Minute);

//                  AddSensorRow(prevBoxId, paramSensor, prevDateTime, currentDateTime, currentSum);
//                  allUsage4Period += ((currentDateTime.DayOfYear - firstDateTime_.DayOfYear) * 1440 + currentSum);
               }
            }
            else
            {
               // the same boxId
               if (prevDateTime.DayOfYear == currentDateTime.DayOfYear)
               {
                  if (isOpen)
                  {
                     if (sensorStatus == Const.valOFF)	// add the amount to the MinutesInUse		
                     {
                        currentSum += (currentDateTime.Hour - prevDateTime.Hour) * 60 + 
                                      (currentDateTime.Minute - prevDateTime.Minute);
                        isOpen = false;
                     }
                     else
                     {
                        // two messages ON, the previous one is lost 
                        Util.BTrace(Util.ERR1, "-- SensorReport.FillUtilization -> ON..ON Messages in the SAME day boxId[{0}] FIRST_ON[{1}] NEXT_ON[{2}] sum[{3}]", 
                                    prevBoxId, prevDateTime, currentDateTime, currentSum );

                        currentSum += (currentDateTime.Hour - prevDateTime.Hour) * 60 +
                                      (currentDateTime.Minute - prevDateTime.Minute);
/*                        if (0 == currentSum)
                           firstDateTime = currentDateTime;
 */ 
                     }
                  }
                  else
                  {
                     // SENSOR was OFF
                     if (sensorStatus == Const.valON)
                     {
                        isOpen = true;
                        if (0 == prevBoxId)
                           firstDateTime = currentDateTime;
/*
                        else
                           // start of the day 
                           firstDateTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day) ; 
*/
                     }
                     else
                     {
                        // two messages OFF, the previous one is lost 
                        Util.BTrace(Util.ERR1, "-- SensorReport.FillUtilization -> OFF..OFF Messages in the SAME day boxId[{0}] FIRST_OFF[{1}] NEXT_OFF[{2}] sum[{3}]", 
                                    prevBoxId, prevDateTime, currentDateTime, currentSum );
                     }
                  }
               }
               else
               {
                  // the same boxId but different days 
                  // write the row and reinit just currentSum,
                  if (isOpen)
                  {
                     if (sensorStatus == Const.valOFF)	// add the amount to the MinutesInUse		
                     {
                        Util.BTrace(Util.WARN0, "-- SensorReport.FillUtilization -> ON..OFF Messages in DIFFERENT days boxId[{0}] FIRST_ON[{1}] NEXT_OFF[{2}] sum[{3}]", 
                                    prevBoxId, firstDateTime, currentDateTime, currentSum );

                        DateTime previousDay = currentDateTime.AddDays(-1).AddHours(23 - currentDateTime.Hour).AddMinutes(59 - currentDateTime.Minute); //  .Add(new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day - 1, 23, 59, 01);  // the night previous day
//                        DateTime previousDay = currentDateTime.AddDays(-1).AddHours(23-currentDateTime.Hour).AddMinutes(59-currentDateTime.Minute); //  .Add(new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day - 1, 23, 59, 01);  // the night previous day
//                        previousDay.Hour = 23; previousDay.Minute = 59; previousDay.Second = 1;


                        // adding an activity across multiple days   
                        CrossSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, previousDay, currentSum, false);

                        firstDateTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0) ;

                        allUsage4Period += (currentSum +
                                           (previousDay.DayOfYear - prevDateTime.DayOfYear) * 1440 +
                                           (previousDay.Hour - prevDateTime.Hour) * 60 +
                                           (previousDay.Minute - prevDateTime.Minute));

                        currentSum = currentDateTime.Hour*60 + currentDateTime.Minute;                          
                        isOpen = false;
                     }
                     else
                     {
                        // two messages ON, the previous one is lost
                        // here you could close the day and start a day 
                        Util.BTrace(Util.ERR1, "-- SensorReport.FillUtilization -> ON Messages in DIFFERENT days boxId[{0}] FIRST_ON[{1}] NEXT_ON[{2}] sum[{3}]", 
                                    prevBoxId, firstDateTime, currentDateTime, currentSum );
                        AddSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, currentSum);
                        firstDateTime = prevDateTime = currentDateTime;
                        allUsage4Period += currentSum;
                        prevBoxId = currentBoxId;
                        currentSum = 0;
                        isOpen = (sensorStatus == Const.valON);                        
                     }                       
                  }
                  else // 
                  {
                     // SENSOR was OFF on different days
                     if (sensorStatus == Const.valON)	// add the amount to the MinutesInUse		
                     {
                        // start a new row, fill the row for the prevBoxId
                        AddSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, currentSum);
                        allUsage4Period += currentSum;
                        isOpen = true;
                        currentSum = 0 ;
                     }
                     else
                     {
                        // two messages OFF, on two different days 
                        // the previous one is lost 
                        Util.BTrace(Util.ERR1, "-- SensorReport.FillUtilization -> OFF Messages in DIFFERENT days boxId[{0}] FIRST_OFF[{1}] NEXT_OFF[{2}] sum[{3}]", 
                                    prevBoxId, prevDateTime, currentDateTime, currentSum );

                        // start a new row, fill the row for the prevBoxId
                        AddSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, currentSum);
                        allUsage4Period += currentSum;
                        currentSum = 0;
                     }

                     firstDateTime = currentDateTime ;
                  } // if ( isOpen )			
               } // if ( prevDateTime.Day == 
            } //  if ( currentBoxId != 

            prevDateTime = currentDateTime;
            prevBoxId = currentBoxId;
	  
	      } // foreach

         // if you still have the last box with a sensor open you have to close the loop 
         if (isOpen)
         {            
            CrossSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, lastDateTime_, currentSum, true);
            allUsage4Period += (currentSum +
                              (lastDateTime_.DayOfYear - prevDateTime.DayOfYear) * 1440 +
                              (lastDateTime_.Hour - prevDateTime.Hour) * 60 +
                              (lastDateTime_.Minute - prevDateTime.Minute));
         }
         else
            if (0 != currentSum)
            {
               AddSensorRow(prevBoxId, paramSensor, firstDateTime, prevDateTime, currentSum);
               allUsage4Period += currentSum;
            }

         if ( allUsage4Period != 0 )
            AddSensorRow(prevBoxId, paramSensor, CT_SUMMARY_DATE, firstDateTime, allUsage4Period);

      }
    

}



}