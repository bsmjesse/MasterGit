/** \file      VioaltionsFleetReport.cs
 *  \comment   from this table you have to produce a summary for every day, 
 *             every vehicle in the fleet based on the mask of violations
 *             
 *  \author    gb, 2006/11/06
 *
 */

using System;
using System.Data;
using VLF.CLS;
using VLF.CLS.Def;
using VLF.MAP;
using VLF.ERR;
using System.Data.SqlClient;	// for SqlException
using System.Collections;
using System.Reflection;

namespace VLF.DAS.DB
{

   #region Class StringValueAttribute

   /// <summary>
   /// Simple attribute class for storing String Values
   /// </summary>
   public class StringValueAttribute : Attribute
   {
      private string _value;

      /// <summary>
      /// Creates a new <see cref="StringValueAttribute"/> instance.
      /// </summary>
      /// <param name="value">Value.</param>
      public StringValueAttribute(string value)
      {
         _value = value;
      }

      /// <summary>
      /// Gets the value.
      /// </summary>
      /// <value></value>
      public string Value
      {
         get { return _value; }
      }
   }

   #endregion

   /// <summary>
   ///      Generates sensors reporting functionality
   /// </summary>
   /// 
   public class ViolationsFleetReport
   {
      private string connectionString = null;
      private DataTable tblDailyActivity = null;   // this is a report per day for every boxid
      private DataSet ds = null;

      /** \enum      VIOLATIONS_DESCRIPTION
       *  \comment   I've used attributes to assign string values to an enum 
       *             see http://www.codeproject.com/csharp/StringEnum.asp 
       */
      private enum VIOLATIONS_DESCRIPTION : short
      {
         [StringValue("High Speed Violation")]
         SPEED = 3,
         [StringValue("Harsh Braking")]
         HARSH_BRAKING = 58,
         [StringValue("Harsh Acceleration")]
         HARSH_ACCELERATION = 60,
         [StringValue("Extreme Acceleration")]
         EXTREME_ACCELERATION = 61,
         [StringValue("Extreme Braking")]
         EXTREME_BRAKING = 59,
         [StringValue("Seat Belt")]
         SEATBELT = 62
      }

      /*
            public static string GetStringValue(Enum value)
            {
               string output = null;
               Type type = value.GetType();

               //Check first in our cached results...
               if (_stringValues.ContainsKey(value))
                  output = (_stringValues[value] as StringValueAttribute).Value;
               else
               {
                  //Look for our 'StringValueAttribute' 
                  //in the field's custom attributes
                  FieldInfo fi = type.GetField(value.ToString());
                  StringValueAttribute[] attrs =
                     fi.GetCustomAttributes(typeof(StringValueAttribute),
                                             false) as StringValueAttribute[];
                  if (attrs.Length > 0)
                  {
                     _stringValues.Add(value, attrs[0]);
                     output = attrs[0].Value;
                  }
               }

               return output;
            }
       */
      /// <summary>
      /// Constructor
      /// </summary>
      public ViolationsFleetReport(string connectionString)
      {
         this.connectionString = connectionString;

         #region DailyActivity table definition
         //    [BoxId],
         //    [VehicleDescription], 
         //    [DateTime], 
         //    [Duration], 
         //    [Description]
         tblDailyActivity = new DataTable("DailyViolations");
         tblDailyActivity.Columns.Add("BoxId", typeof(string));
         tblDailyActivity.Columns.Add("VehicleDescription", typeof(string));
         tblDailyActivity.Columns.Add("DateTime", typeof(DateTime));
         tblDailyActivity.Columns.Add("Duration", typeof(long));
         tblDailyActivity.Columns.Add("Description", typeof(string));        // what violation is here 
         #endregion

         ds = new DataSet();
         ds.Tables.Add(tblDailyActivity);
      }

      /// <summary>
      ///      returns the tblDailyActivity and  tblIdlingDuration in a dataset
      /// </summary>
      /// <returns></returns>

      public DataSet result
      {
         get
         {
            return ds;
         }
      }

      // Changes for TimeZone Feature start
      /// <summary>
      ///            run Exec_GetViolationsReportForFleet which takes into consideration the user preferences
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations">
      ///            is a bitmask of below values 
      ///      public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
      ///      public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
      ///      public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
      ///      public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
      ///      public static int CT_VIOLATION_SEATBELT = 0x0010;
      ///      public static int CT_VIOLATION_SPEED = 0x0020;
      /// 
      /// </param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns>
      ///          [BoxId], [VehicleDescription], [OriginDateTime], [BoxMsgInTypeId], [CustomProp]
      ///         order by BoxId, BoxMsgInTypeId, OriginDateTime
      /// </returns>
      public DataSet Exec_GetViolationsReportForFleet_NewTZ(int fleetId, int userId, int maskViolations,
                                                      DateTime dtFrom, DateTime dtTo, int speed)
      {
          DataSet dsResult = null;
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleet_New_NewTimeZone", conn))
                  {
                      cmd.CommandTimeout = 72000;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@fleetId", SqlDbType.Int);
                      cmd.Parameters["@fleetId"].Value = fleetId;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;
                      cmd.Parameters.Add("@speed", SqlDbType.Int);
                      cmd.Parameters["@speed"].Value = speed;

                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");

                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleet -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }




      // Changes for TimeZone Feature end

      /// <summary>
      ///            run Exec_GetViolationsReportForFleet which takes into consideration the user preferences
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations">
      ///            is a bitmask of below values 
      ///      public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
      ///      public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
      ///      public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
      ///      public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
      ///      public static int CT_VIOLATION_SEATBELT = 0x0010;
      ///      public static int CT_VIOLATION_SPEED = 0x0020;
      /// 
      /// </param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns>
      ///          [BoxId], [VehicleDescription], [OriginDateTime], [BoxMsgInTypeId], [CustomProp]
      ///         order by BoxId, BoxMsgInTypeId, OriginDateTime
      /// </returns>
      public DataSet Exec_GetViolationsReportForFleet(int fleetId, int userId, int maskViolations,
                                                      DateTime dtFrom, DateTime dtTo, int speed)
      {
         DataSet dsResult = null;
         try
         {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleet_New", conn))
               {
                  cmd.CommandTimeout =72000;
                  cmd.CommandType = CommandType.StoredProcedure;
                  cmd.Parameters.Add("@fleetId", SqlDbType.Int);
                  cmd.Parameters["@fleetId"].Value = fleetId;
                  cmd.Parameters.Add("@userId", SqlDbType.Int);
                  cmd.Parameters["@userId"].Value = userId;
                  cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                  cmd.Parameters["@maskViolations"].Value = maskViolations;
                  cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                  cmd.Parameters["@fromDate"].Value = dtFrom;
                  cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                  cmd.Parameters["@toDate"].Value = dtTo;
                  cmd.Parameters.Add("@speed", SqlDbType.Int);
                  cmd.Parameters["@speed"].Value = speed;

                  // Create a new adapter and initialize it with the new SQL command
                  SqlDataAdapter sda = new SqlDataAdapter(cmd);
                  // Create a new data set
                  dsResult = new DataSet();

                  // It is time to update the data set with information from the data adapter
                  //sda.Fill(dsResult, "ReportViolationsFleet");
                  sda.Fill(dsResult, "DailyViolations");

               }
            }
         }
         catch (Exception exc)
         {
            Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleet -> EXC[{0}]", exc.Message);
         }

         return dsResult;
      }




      /// <summary>
      ///            run Exec_GetViolationsReportForFleet which takes into consideration the user preferences
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations">
      ///            is a bitmask of below values 
      ///      public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
      ///      public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
      ///      public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
      ///      public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
      ///      public static int CT_VIOLATION_SEATBELT = 0x0010;
      ///      public static int CT_VIOLATION_SPEED = 0x0020;
      /// 
      /// </param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns>
      ///          [BoxId], [VehicleDescription], [OriginDateTime], [BoxMsgInTypeId], [CustomProp]
      ///         order by BoxId, BoxMsgInTypeId, OriginDateTime
      /// </returns>
      public DataSet Exec_GetViolationsReportForFleet_Exteneded(int fleetId, int userId, int maskViolations,
                                                      DateTime dtFrom, DateTime dtTo, int speed)
      {
          DataSet dsResult = null;
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleet_Exteneded", conn))
                  {
                      cmd.CommandTimeout = 0;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@fleetId", SqlDbType.Int);
                      cmd.Parameters["@fleetId"].Value = fleetId;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;
                      cmd.Parameters.Add("@speed", SqlDbType.Int);
                      cmd.Parameters["@speed"].Value = speed;

                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");

                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleet -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }



      public DataSet Exec_GetViolationsReportForFleet_NewSafetyMatrix(int fleetId, int userId, int maskViolations,
                                               DateTime dtFrom, DateTime dtTo, int speed, string extraParams)
      {
          DataSet dsResult = null;
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleet_Exteneded_NewSafetyMatrix_", conn))
                  {
                      cmd.CommandTimeout = 0;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@fleetId", SqlDbType.Int);
                      cmd.Parameters["@fleetId"].Value = fleetId;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;
                      cmd.Parameters.Add("@speed", SqlDbType.Int);
                      cmd.Parameters["@speed"].Value = speed;
                      cmd.Parameters.Add("@extraParams", SqlDbType.VarChar);
                      cmd.Parameters["@extraParams"].Value = extraParams;

                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");

                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleet -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }



      public DataSet Exec_GetViolationsReportForFleet_Exteneded_NewSafetyMatrix(int fleetId, int userId, int maskViolations,
                                               DateTime dtFrom, DateTime dtTo, int speed, string extraParams)
      {
          DataSet dsResult = null;
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleet_Exteneded_NewSafetyMatrix_Rail", conn))
                  {
                      cmd.CommandTimeout = 0;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@fleetId", SqlDbType.Int);
                      cmd.Parameters["@fleetId"].Value = fleetId;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;
                      cmd.Parameters.Add("@speed", SqlDbType.Int);
                      cmd.Parameters["@speed"].Value = speed;
                      cmd.Parameters.Add("@extraParams", SqlDbType.VarChar);
                      cmd.Parameters["@extraParams"].Value = extraParams;

                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");

                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleet -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }


      /// <summary>
      ///            run Exec_GetViolationsReportForFleetWithScore which takes into consideration the user preferences
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations">
      ///            is a bitmask of below values 
      ///      public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
      ///      public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
      ///      public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
      ///      public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
      ///      public static int CT_VIOLATION_SEATBELT = 0x0010;
      ///      public static int CT_VIOLATION_SPEED = 0x0020;
      /// 
      /// </param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns>
      ///          [BoxId], [VehicleDescription], [DateTime], [Duration] [Description] 
      ///         
      /// </returns>
      public DataSet Exec_GetViolationsReportForFleetWithScore_Extended(int fleetId, int userId, int maskViolations,
                                                      DateTime dtFrom, DateTime dtTo, string ViolationPoints)
      {
          DataSet dsResult = null;

          string[] points = ViolationPoints.Split(';');
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleetWithPointsScore_Extended", conn))
                  {
                      cmd.CommandTimeout = 72000;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@fleetId", SqlDbType.Int);
                      cmd.Parameters["@fleetId"].Value = fleetId;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;

                      cmd.Parameters.Add("@Speed65", SqlDbType.Int);
                      cmd.Parameters["@Speed65"].Value = points[0];

                      cmd.Parameters.Add("@Speed80", SqlDbType.Int);
                      cmd.Parameters["@Speed80"].Value = points[1];

                      cmd.Parameters.Add("@Speed85", SqlDbType.Int);
                      cmd.Parameters["@Speed85"].Value = points[2];

                      cmd.Parameters.Add("@SpeedPosted4", SqlDbType.Int);
                      cmd.Parameters["@SpeedPosted4"].Value = points[3];

                      cmd.Parameters.Add("@SpeedPosted10", SqlDbType.Int);
                      cmd.Parameters["@SpeedPosted10"].Value = points[4];


                      cmd.Parameters.Add("@SpeedPosted15", SqlDbType.Int);
                      cmd.Parameters["@SpeedPosted15"].Value = points[5];

                      cmd.Parameters.Add("@Braking", SqlDbType.Int);
                      cmd.Parameters["@Braking"].Value = points[6];

                      cmd.Parameters.Add("@Acceleration", SqlDbType.Int);
                      cmd.Parameters["@Acceleration"].Value = points[7];


                      cmd.Parameters.Add("@SeatBelt", SqlDbType.Int);
                      cmd.Parameters["@SeatBelt"].Value = points[8];

                      cmd.Parameters.Add("@RevSpeed", SqlDbType.Int);
                      cmd.Parameters["@RevSpeed"].Value = points[9];


                      cmd.Parameters.Add("@HRRevSpeed", SqlDbType.Int);
                      cmd.Parameters["@HRRevSpeed"].Value = points[10];


                      cmd.Parameters.Add("@HRSpeed", SqlDbType.Int);
                      cmd.Parameters["@HRSpeed"].Value = points[11];


                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");
                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleetWithScore -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }






      public DataSet Exec_GetViolationsReportForFleetWithScore_NewSafetyMatrix(int fleetId, int userId, int maskViolations,
                                                   DateTime dtFrom, DateTime dtTo, string ViolationPoints, string extraParams)
      {
          DataSet dsResult = null;

          string[] points = ViolationPoints.Split(';');
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleetWithPointsScore_Extended_NewSafetyMatrix_", conn))
                  {
                      cmd.CommandTimeout = 72000;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@fleetId", SqlDbType.Int);
                      cmd.Parameters["@fleetId"].Value = fleetId;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;

                      cmd.Parameters.Add("@Speed65", SqlDbType.Int);
                      cmd.Parameters["@Speed65"].Value = points[0];

                      cmd.Parameters.Add("@Speed80", SqlDbType.Int);
                      cmd.Parameters["@Speed80"].Value = points[1];

                      cmd.Parameters.Add("@Speed85", SqlDbType.Int);
                      cmd.Parameters["@Speed85"].Value = points[2];

                      cmd.Parameters.Add("@SpeedPosted4", SqlDbType.Int);
                      cmd.Parameters["@SpeedPosted4"].Value = points[3];

                      cmd.Parameters.Add("@SpeedPosted10", SqlDbType.Int);
                      cmd.Parameters["@SpeedPosted10"].Value = points[4];


                      cmd.Parameters.Add("@SpeedPosted15", SqlDbType.Int);
                      cmd.Parameters["@SpeedPosted15"].Value = points[5];

                      cmd.Parameters.Add("@HarshBraking", SqlDbType.Int);
                      cmd.Parameters["@HarshBraking"].Value = points[6];

                      cmd.Parameters.Add("@ExtrBraking", SqlDbType.Int);
                      cmd.Parameters["@ExtrBraking"].Value = points[7];


                      cmd.Parameters.Add("@HarshAccel", SqlDbType.Int);
                      cmd.Parameters["@HarshAccel"].Value = points[8];

                      cmd.Parameters.Add("@ExtrAccel", SqlDbType.Int);
                      cmd.Parameters["@ExtrAccel"].Value = points[9];


                      cmd.Parameters.Add("@SeatBelt", SqlDbType.Int);
                      cmd.Parameters["@SeatBelt"].Value = points[10];


                      cmd.Parameters.Add("@RevSpeed", SqlDbType.Int);
                      cmd.Parameters["@RevSpeed"].Value = points[11];

                          
                      cmd.Parameters.Add("@RevDistance", SqlDbType.Int);
                      cmd.Parameters["@RevDistance"].Value = points[12];

                      cmd.Parameters.Add("@HRSpeed", SqlDbType.Int);
                      cmd.Parameters["@HRSpeed"].Value = points[13];

                      cmd.Parameters.Add("@extraParams", SqlDbType.VarChar);
                      cmd.Parameters["@extraParams"].Value = extraParams;


                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");
                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleetWithScore -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }





      public DataSet Exec_GetViolationsReportForFleetWithScore_Extended_NewSafetyMatrix(int fleetId, int userId, int maskViolations,
                                                   DateTime dtFrom, DateTime dtTo, string ViolationPoints, string extraParams)
      {
          DataSet dsResult = null;

          string[] points = ViolationPoints.Split(';');
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleetWithPointsScore_Extended_NewSafetyMatrix_Rail", conn))
                  {
                      cmd.CommandTimeout = 72000;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@fleetId", SqlDbType.Int);
                      cmd.Parameters["@fleetId"].Value = fleetId;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;

                      cmd.Parameters.Add("@Speed65", SqlDbType.Int);
                      cmd.Parameters["@Speed65"].Value = points[0];

                      cmd.Parameters.Add("@Speed80", SqlDbType.Int);
                      cmd.Parameters["@Speed80"].Value = points[1];

                      cmd.Parameters.Add("@Speed85", SqlDbType.Int);
                      cmd.Parameters["@Speed85"].Value = points[2];

                      cmd.Parameters.Add("@SpeedPosted4", SqlDbType.Int);
                      cmd.Parameters["@SpeedPosted4"].Value = points[3];

                      cmd.Parameters.Add("@SpeedPosted10", SqlDbType.Int);
                      cmd.Parameters["@SpeedPosted10"].Value = points[4];


                      cmd.Parameters.Add("@SpeedPosted15", SqlDbType.Int);
                      cmd.Parameters["@SpeedPosted15"].Value = points[5];

                      cmd.Parameters.Add("@Braking", SqlDbType.Int);
                      cmd.Parameters["@Braking"].Value = points[6];

                      cmd.Parameters.Add("@Acceleration", SqlDbType.Int);
                      cmd.Parameters["@Acceleration"].Value = points[7];


                      cmd.Parameters.Add("@SeatBelt", SqlDbType.Int);
                      cmd.Parameters["@SeatBelt"].Value = points[8];

                      cmd.Parameters.Add("@RevSpeed", SqlDbType.Int);
                      cmd.Parameters["@RevSpeed"].Value = points[9];


                      cmd.Parameters.Add("@HRRevSpeed", SqlDbType.Int);
                      cmd.Parameters["@HRRevSpeed"].Value = points[10];


                      cmd.Parameters.Add("@HRSpeed", SqlDbType.Int);
                      cmd.Parameters["@HRSpeed"].Value = points[11];

                      cmd.Parameters.Add("@extraParams", SqlDbType.VarChar);
                      cmd.Parameters["@extraParams"].Value = extraParams;


                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");
                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleetWithScore -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }

      /// <summary>
      ///            run Exec_GetViolationsReportForFleet which takes into consideration the user preferences
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations">
      ///            is a bitmask of below values 
      ///      public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
      ///      public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
      ///      public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
      ///      public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
      ///      public static int CT_VIOLATION_SEATBELT = 0x0010;
      ///      public static int CT_VIOLATION_SPEED = 0x0020;
      /// 
      /// </param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns>
      ///          [BoxId], [VehicleDescription], [OriginDateTime], [BoxMsgInTypeId], [CustomProp]
      ///         order by BoxId, BoxMsgInTypeId, OriginDateTime
      /// </returns>
      public DataSet Exec_GetViolationsReportForFleet_Main(int fleetId, int userId, int maskViolations,
                                                      DateTime dtFrom, DateTime dtTo, int speed)
      {
          DataSet dsResult = null;
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleet_New_Main", conn))
                  {
                      cmd.CommandTimeout = 72000;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@fleetId", SqlDbType.Int);
                      cmd.Parameters["@fleetId"].Value = fleetId;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;
                      cmd.Parameters.Add("@speed", SqlDbType.Int);
                      cmd.Parameters["@speed"].Value = speed;

                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");

                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleet -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }



      /// <summary>
      ///            run Exec_GetViolationsReportForFleet which takes into consideration the user preferences
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations">
      ///            is a bitmask of below values 
      ///      public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
      ///      public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
      ///      public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
      ///      public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
      ///      public static int CT_VIOLATION_SEATBELT = 0x0010;
      ///      public static int CT_VIOLATION_SPEED = 0x0020;
      /// 
      /// </param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns>
      ///          [BoxId], [VehicleDescription], [OriginDateTime], [BoxMsgInTypeId], [CustomProp]
      ///         order by BoxId, BoxMsgInTypeId, OriginDateTime
      /// </returns>
      public DataSet Exec_GetViolationsReportForVehicle(int boxId, int userId, int maskViolations,
                                                      DateTime dtFrom, DateTime dtTo, int speed)
      {
          DataSet dsResult = null;
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForVehicle_New_Main", conn))
                  //using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleet_New", conn))
                  {
                      cmd.CommandTimeout = 72000;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@boxId", SqlDbType.Int);
                      cmd.Parameters["@boxId"].Value = boxId;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;
                      cmd.Parameters.Add("@speed", SqlDbType.Int);
                      cmd.Parameters["@speed"].Value = speed;

                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");

                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleet -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }

      /// <summary>
      ///            run Exec_GetViolationsReportForFleet which takes into consideration the user preferences
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations">
      ///            is a bitmask of below values 
      ///      public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
      ///      public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
      ///      public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
      ///      public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
      ///      public static int CT_VIOLATION_SEATBELT = 0x0010;
      ///      public static int CT_VIOLATION_SPEED = 0x0020;
      /// 
      /// </param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns>
      ///          [BoxId], [VehicleDescription], [OriginDateTime], [BoxMsgInTypeId], [CustomProp]
      ///         order by BoxId, BoxMsgInTypeId, OriginDateTime
      /// </returns>
      public DataSet Exec_GetViolationsReportForFleet_Hierarchy(string nodeCode, int userId, int maskViolations,
                                                      DateTime dtFrom, DateTime dtTo, int speed)
      {
          DataSet dsResult = null;
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleet_New_Hierarchy", conn))
                  {
                      cmd.CommandTimeout = 72000;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@nodeCode", SqlDbType.VarChar);
                      cmd.Parameters["@nodeCode"].Value = nodeCode;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;
                      cmd.Parameters.Add("@speed", SqlDbType.Int);
                      cmd.Parameters["@speed"].Value = speed;

                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");

                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleet -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }


      /// <summary>
      ///            run Exec_GetViolationsReportForFleetWithScore which takes into consideration the user preferences
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations">
      ///            is a bitmask of below values 
      ///      public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
      ///      public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
      ///      public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
      ///      public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
      ///      public static int CT_VIOLATION_SEATBELT = 0x0010;
      ///      public static int CT_VIOLATION_SPEED = 0x0020;
      /// 
      /// </param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns>
      ///          [BoxId], [VehicleDescription], [DateTime], [Duration] [Description] 
      ///         
      /// </returns>
      public DataSet Exec_GetViolationsReportForFleetWithScore(int fleetId, int userId, int maskViolations,
                                                      DateTime dtFrom, DateTime dtTo, string ViolationPoints)
      {
         DataSet dsResult = null;

         string[] points = ViolationPoints.Split(';');
         try
         {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleetWithPointsScore_New", conn))
               {
                  cmd.CommandTimeout = 72000;
                  cmd.CommandType = CommandType.StoredProcedure;
                  cmd.Parameters.Add("@fleetId", SqlDbType.Int);
                  cmd.Parameters["@fleetId"].Value = fleetId;
                  cmd.Parameters.Add("@userId", SqlDbType.Int);
                  cmd.Parameters["@userId"].Value = userId;
                  cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                  cmd.Parameters["@maskViolations"].Value = maskViolations;
                  cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                  cmd.Parameters["@fromDate"].Value = dtFrom;
                  cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                  cmd.Parameters["@toDate"].Value = dtTo;

                  cmd.Parameters.Add("@Speed120", SqlDbType.Int);
                  cmd.Parameters["@Speed120"].Value = points[0];

                  cmd.Parameters.Add("@Speed130", SqlDbType.Int);
                  cmd.Parameters["@Speed130"].Value = points[1];

                  cmd.Parameters.Add("@Speed140", SqlDbType.Int);
                  cmd.Parameters["@Speed140"].Value = points[2];

                  cmd.Parameters.Add("@AccExtreme", SqlDbType.Int);
                  cmd.Parameters["@AccExtreme"].Value = points[3];

                  cmd.Parameters.Add("@AccHarsh", SqlDbType.Int);
                  cmd.Parameters["@AccHarsh"].Value = points[4];


                  cmd.Parameters.Add("@BrakingExtreme", SqlDbType.Int);
                  cmd.Parameters["@BrakingExtreme"].Value = points[5];

                  cmd.Parameters.Add("@BrakingHarsh", SqlDbType.Int);
                  cmd.Parameters["@BrakingHarsh"].Value = points[6];

                  cmd.Parameters.Add("@SeatBelt", SqlDbType.Int);
                  cmd.Parameters["@SeatBelt"].Value = points[7];


                  //if (points.Length > 8)
                  //{
                  //    cmd.Parameters.Add("@RevSpeed", SqlDbType.Int);
                  //    cmd.Parameters["@RevSpeed"].Value = points[8];

                  //    cmd.Parameters.Add("@RevDist", SqlDbType.Int);
                  //    cmd.Parameters["@RevDist"].Value = points[9];

                  //    cmd.Parameters.Add("@HRSpeed", SqlDbType.Int);
                  //    cmd.Parameters["@HRSpeed"].Value = points[10];
                  //}

                  // Create a new adapter and initialize it with the new SQL command
                  SqlDataAdapter sda = new SqlDataAdapter(cmd);
                  // Create a new data set
                  dsResult = new DataSet();

                  // It is time to update the data set with information from the data adapter
                  //sda.Fill(dsResult, "ReportViolationsFleet");
                  sda.Fill(dsResult, "DailyViolations");
               }
            }
         }
         catch (Exception exc)
         {
            Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleetWithScore -> EXC[{0}]", exc.Message);
         }

         return dsResult;
      }




      /// <summary>
      ///            run Exec_GetViolationsReportForFleetWithScore which takes into consideration the user preferences
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations">
      ///            is a bitmask of below values 
      ///      public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
      ///      public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
      ///      public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
      ///      public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
      ///      public static int CT_VIOLATION_SEATBELT = 0x0010;
      ///      public static int CT_VIOLATION_SPEED = 0x0020;
      /// 
      /// </param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns>
      ///          [BoxId], [VehicleDescription], [DateTime], [Duration] [Description] 
      ///         
      /// </returns>
      public DataSet Exec_GetViolationsReportForFleetWithScore_Main(int fleetId, int userId, int maskViolations,
                                                      DateTime dtFrom, DateTime dtTo, string ViolationPoints)
      {
          DataSet dsResult = null;

          string[] points = ViolationPoints.Split(';');
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleetWithPointsScore_New_Main", conn))
                  {
                      cmd.CommandTimeout = 72000;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@fleetId", SqlDbType.Int);
                      cmd.Parameters["@fleetId"].Value = fleetId;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;

                      cmd.Parameters.Add("@Speed120", SqlDbType.Int);
                      cmd.Parameters["@Speed120"].Value = points[0];

                      cmd.Parameters.Add("@Speed130", SqlDbType.Int);
                      cmd.Parameters["@Speed130"].Value = points[1];

                      cmd.Parameters.Add("@Speed140", SqlDbType.Int);
                      cmd.Parameters["@Speed140"].Value = points[2];

                      cmd.Parameters.Add("@AccExtreme", SqlDbType.Int);
                      cmd.Parameters["@AccExtreme"].Value = points[3];

                      cmd.Parameters.Add("@AccHarsh", SqlDbType.Int);
                      cmd.Parameters["@AccHarsh"].Value = points[4];


                      cmd.Parameters.Add("@BrakingExtreme", SqlDbType.Int);
                      cmd.Parameters["@BrakingExtreme"].Value = points[5];

                      cmd.Parameters.Add("@BrakingHarsh", SqlDbType.Int);
                      cmd.Parameters["@BrakingHarsh"].Value = points[6];

                      cmd.Parameters.Add("@SeatBelt", SqlDbType.Int);
                      cmd.Parameters["@SeatBelt"].Value = points[7];


                      if (points.Length > 8)
                      {
                          cmd.Parameters.Add("@RevSpeed", SqlDbType.Int);
                          cmd.Parameters["@RevSpeed"].Value = points[8];

                          cmd.Parameters.Add("@RevDist", SqlDbType.Int);
                          cmd.Parameters["@RevDist"].Value = points[9];

                          cmd.Parameters.Add("@HRSpeed", SqlDbType.Int);
                          cmd.Parameters["@HRSpeed"].Value = points[10];
                      }

                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");
                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleetWithScore -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }





      






        /// <summary>
        ///            run Exec_GetViolationsReportForFleetWithScore which takes into consideration the user preferences
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations">
        ///            is a bitmask of below values 
        ///      public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
        ///      public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
        ///      public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
        ///      public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
        ///      public static int CT_VIOLATION_SEATBELT = 0x0010;
        ///      public static int CT_VIOLATION_SPEED = 0x0020;
        /// 
        /// </param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns>
        ///          [BoxId], [VehicleDescription], [DateTime], [Duration] [Description] 
        ///         
        /// </returns>
        public DataSet Exec_GetViolationsReportForVehicleWithScore_Extended(int boxId, int userId, int maskViolations,
                                                        DateTime dtFrom, DateTime dtTo, string ViolationPoints)
        {
            DataSet dsResult = null;

            string[] points = ViolationPoints.Split(';');
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForVehicleWithPointsScore_Extended", conn))
                    {
                        cmd.CommandTimeout = 72000;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@boxId", SqlDbType.Int);
                        cmd.Parameters["@boxId"].Value = boxId;
                        cmd.Parameters.Add("@userId", SqlDbType.Int);
                        cmd.Parameters["@userId"].Value = userId;
                        cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                        cmd.Parameters["@maskViolations"].Value = maskViolations;
                        cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                        cmd.Parameters["@fromDate"].Value = dtFrom;
                        cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                        cmd.Parameters["@toDate"].Value = dtTo;

                        cmd.Parameters.Add("@Speed65", SqlDbType.Int);
                        cmd.Parameters["@Speed65"].Value = points[0];

                        cmd.Parameters.Add("@Speed80", SqlDbType.Int);
                        cmd.Parameters["@Speed80"].Value = points[1];

                        cmd.Parameters.Add("@Speed85", SqlDbType.Int);
                        cmd.Parameters["@Speed85"].Value = points[2];

                        cmd.Parameters.Add("@SpeedPosted4", SqlDbType.Int);
                        cmd.Parameters["@SpeedPosted4"].Value = points[3];

                        cmd.Parameters.Add("@SpeedPosted10", SqlDbType.Int);
                        cmd.Parameters["@SpeedPosted10"].Value = points[4];


                        cmd.Parameters.Add("@SpeedPosted15", SqlDbType.Int);
                        cmd.Parameters["@SpeedPosted15"].Value = points[5];

                        cmd.Parameters.Add("@Braking", SqlDbType.Int);
                        cmd.Parameters["@Braking"].Value = points[6];

                        cmd.Parameters.Add("@Acceleration", SqlDbType.Int);
                        cmd.Parameters["@Acceleration"].Value = points[7];


                        cmd.Parameters.Add("@SeatBelt", SqlDbType.Int);
                        cmd.Parameters["@SeatBelt"].Value = points[8];

                        cmd.Parameters.Add("@RevSpeed", SqlDbType.Int);
                        cmd.Parameters["@RevSpeed"].Value = points[9];


                        cmd.Parameters.Add("@HRRevSpeed", SqlDbType.Int);
                        cmd.Parameters["@HRRevSpeed"].Value = points[10];


                        cmd.Parameters.Add("@HRSpeed", SqlDbType.Int);
                        cmd.Parameters["@HRSpeed"].Value = points[11];


                        // Create a new adapter and initialize it with the new SQL command
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        // Create a new data set
                        dsResult = new DataSet();

                        // It is time to update the data set with information from the data adapter
                        //sda.Fill(dsResult, "ReportViolationsFleet");
                        sda.Fill(dsResult, "DailyViolations");
                    }
                }
            }
            catch (Exception exc)
            {
                Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleetWithScore -> EXC[{0}]", exc.Message);
            }

            return dsResult;
        }



        /// <summary>
        ///            run Exec_GetViolationsReportForFleetWithScore which takes into consideration the user preferences
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="maskViolations">
        ///            is a bitmask of below values 
        ///      public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
        ///      public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
        ///      public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
        ///      public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
        ///      public static int CT_VIOLATION_SEATBELT = 0x0010;
        ///      public static int CT_VIOLATION_SPEED = 0x0020;
        /// 
        /// </param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns>
        ///          [BoxId], [VehicleDescription], [DateTime], [Duration] [Description] 
        ///         
        /// </returns>
        public DataSet Exec_GetViolationsReportForVehicleWithScore(int boxId, int userId, int maskViolations,
                                                        DateTime dtFrom, DateTime dtTo, string ViolationPoints)
        {
            DataSet dsResult = null;

          string[] points = ViolationPoints.Split(';');
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  
                 using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleetWithPointsScore_New", conn))
                  {
                      cmd.CommandTimeout = 72000;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@boxId", SqlDbType.Int);
                      cmd.Parameters["@boxId"].Value = boxId;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;

                      cmd.Parameters.Add("@Speed120", SqlDbType.Int);
                      cmd.Parameters["@Speed120"].Value = points[0];

                      cmd.Parameters.Add("@Speed130", SqlDbType.Int);
                      cmd.Parameters["@Speed130"].Value = points[1];

                      cmd.Parameters.Add("@Speed140", SqlDbType.Int);
                      cmd.Parameters["@Speed140"].Value = points[2];

                      cmd.Parameters.Add("@AccExtreme", SqlDbType.Int);
                      cmd.Parameters["@AccExtreme"].Value = points[3];

                      cmd.Parameters.Add("@AccHarsh", SqlDbType.Int);
                      cmd.Parameters["@AccHarsh"].Value = points[4];


                      cmd.Parameters.Add("@BrakingExtreme", SqlDbType.Int);
                      cmd.Parameters["@BrakingExtreme"].Value = points[5];

                      cmd.Parameters.Add("@BrakingHarsh", SqlDbType.Int);
                      cmd.Parameters["@BrakingHarsh"].Value = points[6];

                      cmd.Parameters.Add("@SeatBelt", SqlDbType.Int);
                      cmd.Parameters["@SeatBelt"].Value = points[7];


                      if (points.Length > 8)
                      {
                          cmd.Parameters.Add("@RevSpeed", SqlDbType.Int);
                          cmd.Parameters["@RevSpeed"].Value = points[8];

                          cmd.Parameters.Add("@RevDist", SqlDbType.Int);
                          cmd.Parameters["@RevDist"].Value = points[9];

                          cmd.Parameters.Add("@HRSpeed", SqlDbType.Int);
                          cmd.Parameters["@HRSpeed"].Value = points[10];
                      }

                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");
                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleetWithScore -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }




      /// <summary>
      ///            run Exec_GetViolationsReportForFleetWithScore which takes into consideration the user preferences
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations">
      ///            is a bitmask of below values 
      ///      public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
      ///      public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
      ///      public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
      ///      public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
      ///      public static int CT_VIOLATION_SEATBELT = 0x0010;
      ///      public static int CT_VIOLATION_SPEED = 0x0020;
      /// 
      /// </param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns>
      ///          [BoxId], [VehicleDescription], [DateTime], [Duration] [Description] 
      ///         
      /// </returns>
      public DataSet Exec_GetViolationsReportForVehicleWithScore_Main(int boxId, int userId, int maskViolations,
                                                      DateTime dtFrom, DateTime dtTo, string ViolationPoints)
      {
          DataSet dsResult = null;

          string[] points = ViolationPoints.Split(';');
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForVehicleWithPointsScore_New_Main", conn))
                  // using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleetWithPointsScore_New", conn))
                  {
                      cmd.CommandTimeout = 72000;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@boxId", SqlDbType.Int);
                      cmd.Parameters["@boxId"].Value = boxId;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;

                      cmd.Parameters.Add("@Speed120", SqlDbType.Int);
                      cmd.Parameters["@Speed120"].Value = points[0];

                      cmd.Parameters.Add("@Speed130", SqlDbType.Int);
                      cmd.Parameters["@Speed130"].Value = points[1];

                      cmd.Parameters.Add("@Speed140", SqlDbType.Int);
                      cmd.Parameters["@Speed140"].Value = points[2];

                      cmd.Parameters.Add("@AccExtreme", SqlDbType.Int);
                      cmd.Parameters["@AccExtreme"].Value = points[3];

                      cmd.Parameters.Add("@AccHarsh", SqlDbType.Int);
                      cmd.Parameters["@AccHarsh"].Value = points[4];


                      cmd.Parameters.Add("@BrakingExtreme", SqlDbType.Int);
                      cmd.Parameters["@BrakingExtreme"].Value = points[5];

                      cmd.Parameters.Add("@BrakingHarsh", SqlDbType.Int);
                      cmd.Parameters["@BrakingHarsh"].Value = points[6];

                      cmd.Parameters.Add("@SeatBelt", SqlDbType.Int);
                      cmd.Parameters["@SeatBelt"].Value = points[7];


                      if (points.Length > 8)
                      {
                          cmd.Parameters.Add("@RevSpeed", SqlDbType.Int);
                          cmd.Parameters["@RevSpeed"].Value = points[8];

                          cmd.Parameters.Add("@RevDist", SqlDbType.Int);
                          cmd.Parameters["@RevDist"].Value = points[9];

                          cmd.Parameters.Add("@HRSpeed", SqlDbType.Int);
                          cmd.Parameters["@HRSpeed"].Value = points[10];
                      }

                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");
                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleetWithScore -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }


      /// <summary>
      ///            run Exec_GetViolationsReportForFleetWithScore which takes into consideration the user preferences
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations">
      ///            is a bitmask of below values 
      ///      public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
      ///      public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
      ///      public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
      ///      public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
      ///      public static int CT_VIOLATION_SEATBELT = 0x0010;
      ///      public static int CT_VIOLATION_SPEED = 0x0020;
      /// 
      /// </param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns>
      ///          [BoxId], [VehicleDescription], [DateTime], [Duration] [Description] 
      ///         
      /// </returns>
      public DataSet Exec_GetViolationsReportForFleetWithScore_Hierarchy(string  nodeCode, int userId, int maskViolations,
                                                      DateTime dtFrom, DateTime dtTo, string ViolationPoints)
      {
          DataSet dsResult = null;

          string[] points = ViolationPoints.Split(';');
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleetWithPointsScore_New_Hierarchy", conn))
                  {
                      cmd.CommandTimeout = 72000;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@nodeCode", SqlDbType.VarChar);
                      cmd.Parameters["@nodeCode"].Value = nodeCode;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;

                      cmd.Parameters.Add("@Speed120", SqlDbType.Int);
                      cmd.Parameters["@Speed120"].Value = points[0];

                      cmd.Parameters.Add("@Speed130", SqlDbType.Int);
                      cmd.Parameters["@Speed130"].Value = points[1];

                      cmd.Parameters.Add("@Speed140", SqlDbType.Int);
                      cmd.Parameters["@Speed140"].Value = points[2];

                      cmd.Parameters.Add("@AccExtreme", SqlDbType.Int);
                      cmd.Parameters["@AccExtreme"].Value = points[3];

                      cmd.Parameters.Add("@AccHarsh", SqlDbType.Int);
                      cmd.Parameters["@AccHarsh"].Value = points[4];


                      cmd.Parameters.Add("@BrakingExtreme", SqlDbType.Int);
                      cmd.Parameters["@BrakingExtreme"].Value = points[5];

                      cmd.Parameters.Add("@BrakingHarsh", SqlDbType.Int);
                      cmd.Parameters["@BrakingHarsh"].Value = points[6];

                      cmd.Parameters.Add("@SeatBelt", SqlDbType.Int);
                      cmd.Parameters["@SeatBelt"].Value = points[7];


                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");
                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleetWithScore -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }



      /// <summary>
      ///            run Exec_GetViolationsReportForFleetWithScore which takes into consideration the user preferences
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations">
      ///            is a bitmask of below values 
      ///      public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
      ///      public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
      ///      public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
      ///      public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
      ///      public static int CT_VIOLATION_SEATBELT = 0x0010;
      ///      public static int CT_VIOLATION_SPEED = 0x0020;
      /// 
      /// </param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns>
      ///          [BoxId], [VehicleDescription], [DateTime], [Duration] [Description] 
      ///         
      /// </returns>
      public DataSet ViolationsMonthlyData(int fleetId, int userId, int maskViolations,
                                                      DateTime dtFrom, DateTime dtTo, string ViolationPoints)
      {
          DataSet dsResult = null;

          string[] points = ViolationPoints.Split(';');
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("sp_ViolationsMonthlyData", conn))
                  {
                      cmd.CommandTimeout = 72000;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@fleetId", SqlDbType.Int);
                      cmd.Parameters["@fleetId"].Value = fleetId;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;

                      cmd.Parameters.Add("@Speed120", SqlDbType.Int);
                      cmd.Parameters["@Speed120"].Value = points[0];

                      cmd.Parameters.Add("@Speed130", SqlDbType.Int);
                      cmd.Parameters["@Speed130"].Value = points[1];

                      cmd.Parameters.Add("@Speed140", SqlDbType.Int);
                      cmd.Parameters["@Speed140"].Value = points[2];

                      cmd.Parameters.Add("@AccExtreme", SqlDbType.Int);
                      cmd.Parameters["@AccExtreme"].Value = points[3];

                      cmd.Parameters.Add("@AccHarsh", SqlDbType.Int);
                      cmd.Parameters["@AccHarsh"].Value = points[4];


                      cmd.Parameters.Add("@BrakingExtreme", SqlDbType.Int);
                      cmd.Parameters["@BrakingExtreme"].Value = points[5];

                      cmd.Parameters.Add("@BrakingHarsh", SqlDbType.Int);
                      cmd.Parameters["@BrakingHarsh"].Value = points[6];

                      cmd.Parameters.Add("@SeatBelt", SqlDbType.Int);
                      cmd.Parameters["@SeatBelt"].Value = points[7];


                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");
                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleetWithScore -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }



      /// <summary>
      ///            run Exec_GetViolationsReportForFleetWithScore which takes into consideration the user preferences
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations">
      ///            is a bitmask of below values 
      ///      public static int CT_VIOLATION_HARSHBRAKING = 0x0001;
      ///      public static int CT_VIOLATION_HARSHACCELERATION = 0x0002;
      ///      public static int CT_VIOLATION_EXTREMEACCELERATION = 0x0004;
      ///      public static int CT_VIOLATION_EXTREMEBRAKING = 0x0008;
      ///      public static int CT_VIOLATION_SEATBELT = 0x0010;
      ///      public static int CT_VIOLATION_SPEED = 0x0020;
      /// 
      /// </param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns>
      ///          [BoxId], [VehicleDescription], [DateTime], [Duration] [Description] 
      ///         
      /// </returns>
      public DataSet Exec_GetViolationsReportForFleetWithScore_Special(int fleetId, int userId, int maskViolations,
                                                      DateTime dtFrom, DateTime dtTo, string ViolationPoints)
      {
          DataSet dsResult = null;

          string[] points = ViolationPoints.Split(';');
          try
          {
              using (SqlConnection conn = new SqlConnection(connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("sp_GetViolationsReportForFleetWithPointsScore_Special", conn))
                  {
                      cmd.CommandTimeout = 72000;
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Add("@fleetId", SqlDbType.Int);
                      cmd.Parameters["@fleetId"].Value = fleetId;
                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;

                      cmd.Parameters.Add("@Speed120", SqlDbType.Int);
                      cmd.Parameters["@Speed120"].Value = points[0];

                      cmd.Parameters.Add("@Speed130", SqlDbType.Int);
                      cmd.Parameters["@Speed130"].Value = points[1];

                      cmd.Parameters.Add("@Speed140", SqlDbType.Int);
                      cmd.Parameters["@Speed140"].Value = points[2];

                      cmd.Parameters.Add("@AccExtreme", SqlDbType.Int);
                      cmd.Parameters["@AccExtreme"].Value = points[3];

                      cmd.Parameters.Add("@AccHarsh", SqlDbType.Int);
                      cmd.Parameters["@AccHarsh"].Value = points[4];


                      cmd.Parameters.Add("@BrakingExtreme", SqlDbType.Int);
                      cmd.Parameters["@BrakingExtreme"].Value = points[5];

                      cmd.Parameters.Add("@BrakingHarsh", SqlDbType.Int);
                      cmd.Parameters["@BrakingHarsh"].Value = points[6];

                      cmd.Parameters.Add("@SeatBelt", SqlDbType.Int);
                      cmd.Parameters["@SeatBelt"].Value = points[7];


                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);
                      // Create a new data set
                      dsResult = new DataSet();

                      // It is time to update the data set with information from the data adapter
                      //sda.Fill(dsResult, "ReportViolationsFleet");
                      sda.Fill(dsResult, "DailyViolations");
                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForFleetWithScore -> EXC[{0}]", exc.Message);
          }

          return dsResult;
      }

      private void AddRow(int boxId, short messageId, string vehicleDescription,
                          DateTime dateTime, long currentSum)
      {
         try
         {
            VIOLATIONS_DESCRIPTION msgId = (VIOLATIONS_DESCRIPTION)messageId;
            //   [BoxId], [VehicleDescription], [DateTime], [Duration], [Description]
            object[] objRow = new object[tblDailyActivity.Columns.Count];
            objRow[0] = boxId.ToString();                // BoxId
            objRow[1] = vehicleDescription;
            objRow[2] = dateTime;
            objRow[3] = new TimeSpan(currentSum * TimeSpan.TicksPerSecond).Seconds;
            objRow[4] = msgId.ToString();

            tblDailyActivity.Rows.Add(objRow);
         }
         catch (Exception exc)
         {
         }
      }

      /** \fn     public void FillReport(int paramSensor, DateTime firstDateTime_, DateTime lastDateTime_, DataSet rowData)
        * 
        *  \brief  this function receives all messages IDs between specific hours and 
        *           calculates the total utilization of the time for the day
        *          
        *          This function is producing table DailyActivity
        *                   [BoxId], [VehicleDescription], [DateTime], [Duration], [Description]
        * 
        *  \comment rowData is the table returned after you called 
        *           Exec_GetSensorsPerFleet @fleetName, @sensorId, @dateFrom, @dateTo
        *             having rows 
        *           [BoxId], [VehicleDescription], [OriginDateTime], [BoxMsgInTypeId], [CustomProp]
        *           order by BoxId, BoxMsgInTypeId, OriginDateTime
        */
      public void FillReport(DateTime firstDateTime_,
                             DateTime lastDateTime_,
                             DataSet rowData)  //<information about every row with all data from Exec_GetViolationsReportForFleet
      {
         // check arguments
         if (!Util.IsTable(rowData) || firstDateTime_ > lastDateTime_)
            Util.BTrace(Util.WARN1, " << ViolationsFleetReport.FillReport -> empty/wrong arguments");

         // delete the previous data report 
         if (tblDailyActivity.Rows.Count > 0)
            tblDailyActivity.Rows.Clear();


         // first, the calculation of utilization is made on all messages 
         // for every record
         DateTime currentDateTime,
                  prevDateTime = firstDateTime_;
         int currentBoxId = 0, prevBoxId = 0;
         short currentMessageId = 0, prevMessageId = 0;

         // I suppose there is no boxid as 0 !!
         long currentSum = 0;	         // where you add the periods when the same messageId you have it on the same day 
         string vehicleDescription = null;
         string customProp = "", keyWord = "";

         foreach (DataRow ittr in rowData.Tables[0].Rows)
         {
            currentMessageId = Util.Field2Int16(ittr, "BoxMsgInTypeId", 0);
            currentBoxId = Convert.ToInt32(ittr["BoxId"]);            // find boxid
            currentDateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
            vehicleDescription = ittr["VehicleDescription"].ToString().TrimEnd();

            customProp = "";
            keyWord = (currentMessageId == (short)Enums.MessageType.Speed) ?
                        VLF.CLS.Def.Const.keySpeedDuration : VLF.CLS.Def.Const.keyDuration;

            Util.Field2String(ittr, "CustomProp", ref customProp, "");

            if (currentBoxId != prevBoxId && 0 != prevBoxId)   // start of a detailed utilization for a new box
            {
               // close the report for the previous box 
               AddRow(prevBoxId, prevMessageId, vehicleDescription, prevDateTime, currentSum + Util.ExtractDuration(keyWord, customProp));
               currentSum = 0;
            }
            else
            {
               // the same boxId
               if (currentMessageId == prevMessageId)
               {
                  if (prevDateTime.DayOfYear == currentDateTime.DayOfYear)
                  {
                     // add the amount to the MinutesInUse		
                     currentSum += Util.ExtractDuration(keyWord, customProp);
                  }
                  else
                  {
                     // add the row and continue 
                     AddRow(prevBoxId, prevMessageId, vehicleDescription, prevDateTime, currentSum + Util.ExtractDuration(keyWord, customProp));
                     currentSum = 0;
                  }
               }
               else
               {
                  // another messageId, the same boxId
                  // add the row and continue 
                  AddRow(prevBoxId, prevMessageId, vehicleDescription, prevDateTime, currentSum + Util.ExtractDuration(keyWord, customProp));
                  currentSum = 0;
               }
            }

            prevDateTime = currentDateTime;
            prevBoxId = currentBoxId;
            prevMessageId = currentMessageId;

         } // foreach

         // if you still have the last box row not closed
         if (currentSum != 0)
         {
            AddRow(prevBoxId, prevMessageId, vehicleDescription, prevDateTime, currentSum + Util.ExtractDuration(keyWord, customProp));
         }
      }

      /// <summary>
      /// Exec ReportDriverViolations
      /// </summary>
      /// <param name="driverId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations"></param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <param name="speed"></param>
      /// <returns></returns>
      public DataSet Exec_GetViolationsReportForDriver(int driverId, int userId, int maskViolations, DateTime dtFrom, DateTime dtTo, int speed)
      {
         DataSet dsResult = new DataSet();
         try
         {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                
                using (SqlCommand cmd = new SqlCommand("ReportDriverViolations", conn))
               {
                  cmd.CommandTimeout = 72000;
                  cmd.CommandType = CommandType.StoredProcedure;

                  cmd.Parameters.Add("@userId", SqlDbType.Int);
                  cmd.Parameters["@userId"].Value = userId;
                  cmd.Parameters.Add("@driverId", SqlDbType.Int);
                  cmd.Parameters["@driverId"].Value = driverId;
                  cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                  cmd.Parameters["@maskViolations"].Value = maskViolations;
                  cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                  cmd.Parameters["@fromDate"].Value = dtFrom;
                  cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                  cmd.Parameters["@toDate"].Value = dtTo;
                  cmd.Parameters.Add("@speed", SqlDbType.Int);
                  cmd.Parameters["@speed"].Value = speed;

                  // Create a new adapter and initialize it with the new SQL command
                  SqlDataAdapter sda = new SqlDataAdapter(cmd);

                  // fill data adapter
                  sda.Fill(dsResult, "DailyViolations");
               }
            }
         }
         catch (Exception exc)
         {
            Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForDriver -> EXC[{0}]", exc.Message);
         }
         return dsResult;
      }



      /// <summary>
      /// Exec ReportDriverViolations
      /// </summary>
      /// <param name="driverId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations"></param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <param name="speed"></param>
      /// <returns></returns>
      public DataSet Exec_GetViolationsReportForDriver_Main(int driverId, int userId, int maskViolations, DateTime dtFrom, DateTime dtTo, int speed)
      {
          DataSet dsResult = new DataSet();
          try
          {
              using (SqlConnection conn = new SqlConnection(this.connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("ReportDriverViolations_Main", conn))
                  //using (SqlCommand cmd = new SqlCommand("ReportDriverViolations", conn))
                  {
                      cmd.CommandTimeout = 72000;
                      cmd.CommandType = CommandType.StoredProcedure;

                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@driverId", SqlDbType.Int);
                      cmd.Parameters["@driverId"].Value = driverId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;
                      cmd.Parameters.Add("@speed", SqlDbType.Int);
                      cmd.Parameters["@speed"].Value = speed;

                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);

                      // fill data adapter
                      sda.Fill(dsResult, "DailyViolations");
                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForDriver -> EXC[{0}]", exc.Message);
          }
          return dsResult;
      }

      /// <summary>
      /// Exec ReportDriverViolationsSummary
      /// </summary>
      /// <param name="driverId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations"></param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <param name="ViolationPoints"></param>
      /// <returns></returns>
      public DataSet Exec_GetViolationsReportForDriverWithScore(int driverId, int userId, int maskViolations, DateTime dtFrom, DateTime dtTo, string[] ViolationPoints)
      {
         DataSet dsResult = new DataSet();
         try
         {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ReportDriverViolationsSummary", conn))
               {
                  cmd.CommandTimeout = 72000;
                  cmd.CommandType = CommandType.StoredProcedure;

                  cmd.Parameters.Add("@userId", SqlDbType.Int);
                  cmd.Parameters["@userId"].Value = userId;
                  cmd.Parameters.Add("@driverId", SqlDbType.Int);
                  cmd.Parameters["@driverId"].Value = driverId;
                  cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                  cmd.Parameters["@maskViolations"].Value = maskViolations;
                  cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                  cmd.Parameters["@fromDate"].Value = dtFrom;
                  cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                  cmd.Parameters["@toDate"].Value = dtTo;

                  cmd.Parameters.Add("@Speed120", SqlDbType.Int);
                  cmd.Parameters["@Speed120"].Value = ViolationPoints[0];

                  cmd.Parameters.Add("@Speed130", SqlDbType.Int);
                  cmd.Parameters["@Speed130"].Value = ViolationPoints[1];

                  cmd.Parameters.Add("@Speed140", SqlDbType.Int);
                  cmd.Parameters["@Speed140"].Value = ViolationPoints[2];

                  cmd.Parameters.Add("@AccExtreme", SqlDbType.Int);
                  cmd.Parameters["@AccExtreme"].Value = ViolationPoints[3];

                  cmd.Parameters.Add("@AccHarsh", SqlDbType.Int);
                  cmd.Parameters["@AccHarsh"].Value = ViolationPoints[4];

                  cmd.Parameters.Add("@BrakingExtreme", SqlDbType.Int);
                  cmd.Parameters["@BrakingExtreme"].Value = ViolationPoints[5];

                  cmd.Parameters.Add("@BrakingHarsh", SqlDbType.Int);
                  cmd.Parameters["@BrakingHarsh"].Value = ViolationPoints[6];

                  cmd.Parameters.Add("@SeatBelt", SqlDbType.Int);
                  cmd.Parameters["@SeatBelt"].Value = ViolationPoints[7];

                  // Create a new adapter and initialize it with the new SQL command
                  SqlDataAdapter sda = new SqlDataAdapter(cmd);

                  // fill data adapter
                  sda.Fill(dsResult, "DailyViolations");
               }
            }
         }
         catch (Exception exc)
         {
            Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForDriverWithScore -> EXC[{0}]", exc.Message);
         }
         return dsResult;
      }



      /// <summary>
      /// Exec ReportDriverViolationsSummary
      /// </summary>
      /// <param name="driverId"></param>
      /// <param name="userId"></param>
      /// <param name="maskViolations"></param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <param name="ViolationPoints"></param>
      /// <returns></returns>
      public DataSet Exec_GetViolationsReportForDriverWithScore_Main(int driverId, int userId, int maskViolations, DateTime dtFrom, DateTime dtTo, string[] ViolationPoints)
      {
          DataSet dsResult = new DataSet();
          try
          {
              using (SqlConnection conn = new SqlConnection(this.connectionString))
              {
                  using (SqlCommand cmd = new SqlCommand("ReportDriverViolationsSummary_Main", conn))
                  //using (SqlCommand cmd = new SqlCommand("ReportDriverViolationsSummary", conn))
                  {
                      cmd.CommandTimeout = 72000;
                      cmd.CommandType = CommandType.StoredProcedure;

                      cmd.Parameters.Add("@userId", SqlDbType.Int);
                      cmd.Parameters["@userId"].Value = userId;
                      cmd.Parameters.Add("@driverId", SqlDbType.Int);
                      cmd.Parameters["@driverId"].Value = driverId;
                      cmd.Parameters.Add("@maskViolations", SqlDbType.Int);
                      cmd.Parameters["@maskViolations"].Value = maskViolations;
                      cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                      cmd.Parameters["@fromDate"].Value = dtFrom;
                      cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                      cmd.Parameters["@toDate"].Value = dtTo;

                      cmd.Parameters.Add("@Speed120", SqlDbType.Int);
                      cmd.Parameters["@Speed120"].Value = ViolationPoints[0];

                      cmd.Parameters.Add("@Speed130", SqlDbType.Int);
                      cmd.Parameters["@Speed130"].Value = ViolationPoints[1];

                      cmd.Parameters.Add("@Speed140", SqlDbType.Int);
                      cmd.Parameters["@Speed140"].Value = ViolationPoints[2];

                      cmd.Parameters.Add("@AccExtreme", SqlDbType.Int);
                      cmd.Parameters["@AccExtreme"].Value = ViolationPoints[3];

                      cmd.Parameters.Add("@AccHarsh", SqlDbType.Int);
                      cmd.Parameters["@AccHarsh"].Value = ViolationPoints[4];

                      cmd.Parameters.Add("@BrakingExtreme", SqlDbType.Int);
                      cmd.Parameters["@BrakingExtreme"].Value = ViolationPoints[5];

                      cmd.Parameters.Add("@BrakingHarsh", SqlDbType.Int);
                      cmd.Parameters["@BrakingHarsh"].Value = ViolationPoints[6];

                      cmd.Parameters.Add("@SeatBelt", SqlDbType.Int);
                      cmd.Parameters["@SeatBelt"].Value = ViolationPoints[7];

                      // Create a new adapter and initialize it with the new SQL command
                      SqlDataAdapter sda = new SqlDataAdapter(cmd);

                      // fill data adapter
                      sda.Fill(dsResult, "DailyViolations");
                  }
              }
          }
          catch (Exception exc)
          {
              Util.BTrace(Util.ERR2, "-- ViolationsFleetReport.Exec_GetViolationsReportForDriverWithScore -> EXC[{0}]", exc.Message);
          }
          return dsResult;
      }
   }
}
