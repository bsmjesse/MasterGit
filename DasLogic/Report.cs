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
    /// <summary>
    /// Provides interface to report functionality in database
    /// </summary>
    public class Report : Das
    {
        DB.Report report = null;

        #region General Interfaces
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public Report(string connectionString)
            : base(connectionString)
        {
            report = new DB.Report(base.sqlExec);
        }
        /// <summary>
        /// Destructor
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
        }
        #endregion

        #region Reports

        /// <summary>
        /// Prepares detailed trip report
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="includeStreetAddress"></param>
        /// <param name="includeSensors"></param>
        /// <param name="includePosition"></param>
        /// <param name="includeIdleTime"></param>
        /// <param name="includeSummary"></param>
        /// <param name="showLastStoredPosition"></param>
        /// <param name="userId"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <param name="outMaxOverflowed"></param>
        /// <param name="outMaxFleetRecords"></param>
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
        /// </remarks>
        /// <returns> DataSet [TripIndex],[Reson],[Date/Time],[Location],[Speed],[Description] </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetFleetDetailedTripReport(int fleetId,
                                                  string fromDateTime,
                                                  string toDateTime,
                                                  bool includeStreetAddress,
                                                  bool includeSensors,
                                                  bool includePosition,
                                                  bool includeIdleTime,
                                                  bool includeSummary,
                                                  bool showLastStoredPosition,
                                                  int userId,
                                                  string lang,
                                                  ref bool requestOverflowed,
                                                  ref int totalSqlRecords,
                                                  ref bool outMaxOverflowed,
                                                  ref int outMaxFleetRecords)
        {
            int sqlMaxOutRecords = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Max Output Records", 1000);
            int outMaxRecords = 0;
            int currSqlRecords = 0;
            outMaxFleetRecords = 0;
            outMaxOverflowed = false;
            DataSet dsFleetTips = new DataSet();
            DataSet dsCurrVehicleTrips = null;
            // 1. Retrieve fleet vehicles info (license plates)
            DB.FleetVehicles fleet = new DB.FleetVehicles(sqlExec);
            DataSet dsVehicles = fleet.GetVehiclesInfoByFleetId(fleetId);
            if ((dsVehicles != null) && (dsVehicles.Tables.Count > 0))
            {
                string licensePlate = "";
                foreach (DataRow ittrRow in dsVehicles.Tables[0].Rows)
                {
                    licensePlate = ittrRow["LicensePlate"].ToString().TrimEnd();
                    currSqlRecords = 0;
                    // 2. Retrieves trip info for every vehicle
                    dsCurrVehicleTrips = GetDetailedTripReport(licensePlate,
                                                                 fromDateTime,
                                                                 toDateTime,
                                                                 includeStreetAddress,
                                                                 includeSensors,
                                                                 includePosition,
                                                                 includeIdleTime,
                                                                 includeSummary,
                                                                 showLastStoredPosition,
                                                                 userId, lang,
                                                                 ref requestOverflowed,
                                                                 ref currSqlRecords,
                                                                 ref outMaxOverflowed,
                                                                 ref outMaxRecords);
                    // 3. Merge vehicle trips info into fleet trips info
                    totalSqlRecords += currSqlRecords;
                    outMaxFleetRecords += outMaxRecords;
                    if (sqlMaxOutRecords < outMaxFleetRecords)
                        outMaxOverflowed = true;

                    if (requestOverflowed || outMaxOverflowed)
                    {
                        dsFleetTips.Clear();
                        break;
                    }

                    if (dsFleetTips != null && dsCurrVehicleTrips != null)
                    {
                        dsFleetTips.Merge(dsCurrVehicleTrips);
                    }
                }
            }
            dsFleetTips.DataSetName = "DsFleetDetailedTripReport";
            return dsFleetTips;
        }

        // Changes for TimeZone Feature start
        public DataSet GetFleetDetailedTripReport_NewTZ(int fleetId,
                                          string fromDateTime,
                                          string toDateTime,
                                          bool includeStreetAddress,
                                          bool includeSensors,
                                          bool includePosition,
                                          bool includeIdleTime,
                                          bool includeSummary,
                                          bool showLastStoredPosition,
                                          int userId,
                                          int sensorId,
                                          string lang,
                                          ref bool requestOverflowed,
                                          ref int totalSqlRecords,
                                          ref bool outMaxOverflowed,
                                          ref int outMaxFleetRecords)
        {
            int sqlMaxOutRecords = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Max Output Records", 1000);
            int outMaxRecords = 0;
            int currSqlRecords = 0;
            outMaxFleetRecords = 0;
            outMaxOverflowed = false;
            DataSet dsFleetTips = new DataSet();
            DataSet dsCurrVehicleTrips = null;
            // 1. Retrieve fleet vehicles info (license plates)
            DB.FleetVehicles fleet = new DB.FleetVehicles(sqlExec);
            DataSet dsVehicles = fleet.GetVehiclesInfoByFleetId(fleetId);
            if ((dsVehicles != null) && (dsVehicles.Tables.Count > 0))
            {
                string licensePlate = "";
                foreach (DataRow ittrRow in dsVehicles.Tables[0].Rows)
                {
                    licensePlate = ittrRow["LicensePlate"].ToString().TrimEnd();
                    currSqlRecords = 0;
                    // 2. Retrieves trip info for every vehicle
                    dsCurrVehicleTrips = GetDetailedTripReport_NewTZ(licensePlate,
                                                              fromDateTime,
                                                              toDateTime,
                                                              includeStreetAddress,
                                                              includeSensors,
                                                              includePosition,
                                                              includeIdleTime,
                                                              includeSummary,
                                                              showLastStoredPosition,
                                                              userId,
                                                              sensorId,
                                                              lang,
                                                              ref requestOverflowed,
                                                              ref currSqlRecords,
                                                              ref outMaxOverflowed,
                                                              ref outMaxRecords);
                    // 3. Merge vehicle trips info into fleet trips info
                    totalSqlRecords += currSqlRecords;
                    outMaxFleetRecords += outMaxRecords;
                    if (sqlMaxOutRecords < outMaxFleetRecords)
                        outMaxOverflowed = true;

                    if (requestOverflowed || outMaxOverflowed)
                    {
                        dsFleetTips.Clear();
                        break;
                    }

                    if (dsFleetTips != null && dsCurrVehicleTrips != null)
                    {
                        dsFleetTips.Merge(dsCurrVehicleTrips);
                    }
                }
            }
            dsFleetTips.DataSetName = "DsFleetDetailedTripReport";
            return dsFleetTips;
        }
        // Changes for TimeZone Feature end

        public DataSet GetFleetDetailedTripReport(int fleetId,
                                                  string fromDateTime,
                                                  string toDateTime,
                                                  bool includeStreetAddress,
                                                  bool includeSensors,
                                                  bool includePosition,
                                                  bool includeIdleTime,
                                                  bool includeSummary,
                                                  bool showLastStoredPosition,
                                                  int userId,
                                                  int sensorId,
                                                  string lang,
                                                  ref bool requestOverflowed,
                                                  ref int totalSqlRecords,
                                                  ref bool outMaxOverflowed,
                                                  ref int outMaxFleetRecords)
        {
            int sqlMaxOutRecords = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Max Output Records", 1000);
            int outMaxRecords = 0;
            int currSqlRecords = 0;
            outMaxFleetRecords = 0;
            outMaxOverflowed = false;
            DataSet dsFleetTips = new DataSet();
            DataSet dsCurrVehicleTrips = null;
            // 1. Retrieve fleet vehicles info (license plates)
            DB.FleetVehicles fleet = new DB.FleetVehicles(sqlExec);
            DataSet dsVehicles = fleet.GetVehiclesInfoByFleetId(fleetId);
            if ((dsVehicles != null) && (dsVehicles.Tables.Count > 0))
            {
                string licensePlate = "";
                foreach (DataRow ittrRow in dsVehicles.Tables[0].Rows)
                {
                    licensePlate = ittrRow["LicensePlate"].ToString().TrimEnd();
                    currSqlRecords = 0;
                    // 2. Retrieves trip info for every vehicle
                    dsCurrVehicleTrips = GetDetailedTripReport(licensePlate,
                                                              fromDateTime,
                                                              toDateTime,
                                                              includeStreetAddress,
                                                              includeSensors,
                                                              includePosition,
                                                              includeIdleTime,
                                                              includeSummary,
                                                              showLastStoredPosition,
                                                              userId,
                                                              sensorId,
                                                              lang,
                                                              ref requestOverflowed,
                                                              ref currSqlRecords,
                                                              ref outMaxOverflowed,
                                                              ref outMaxRecords);
                    // 3. Merge vehicle trips info into fleet trips info
                    totalSqlRecords += currSqlRecords;
                    outMaxFleetRecords += outMaxRecords;
                    if (sqlMaxOutRecords < outMaxFleetRecords)
                        outMaxOverflowed = true;

                    if (requestOverflowed || outMaxOverflowed)
                    {
                        dsFleetTips.Clear();
                        break;
                    }

                    if (dsFleetTips != null && dsCurrVehicleTrips != null)
                    {
                        dsFleetTips.Merge(dsCurrVehicleTrips);
                    }
                }
            }
            dsFleetTips.DataSetName = "DsFleetDetailedTripReport";
            return dsFleetTips;
        }

        /// <summary>
        /// Prepares trip report
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="showLastStoredPosition"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <param name="outMaxOverflowed"></param>
        /// <param name="outMaxFleetRecords"></param>
        /// <remarks>
        /// 1. On Ignition On satrt trip
        /// 2. On Ignition Off stop trip
        /// 3. Calculates trip statistics:
        ///		3.1 Trip Duration
        ///		3.2 Trip Distance
        ///		3.3 Trip Average Speed
        ///		3.4 Trip Stops
        ///		3.5 Trip Cost
        /// </remarks>
        /// <returns> DataSet [TripIndex],[Reson],[Date/Time],[Location],[Speed],[Description] </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetFleetTripReport(int fleetId,
           string fromDateTime,
           string toDateTime,
           int userId,
           bool showLastStoredPosition, string lang,
           ref bool requestOverflowed,
           ref int totalSqlRecords,
           ref bool outMaxOverflowed,
           ref int outMaxFleetRecords)
        {
            int sqlMaxOutRecords = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Max Output Records", 1000);
            int outMaxRecords = 0;
            int currSqlRecords = 0;
            outMaxFleetRecords = 0;
            outMaxOverflowed = false;
            DataSet dsFleetTips = new DataSet();
            DataSet dsCurrVehicleTrips = null;
            // 1. Retrieve fleet vehicles info (license plates)
            DB.FleetVehicles fleet = new DB.FleetVehicles(sqlExec);
            DataSet dsVehicles = fleet.GetVehiclesInfoByFleetId(fleetId);
            if ((dsVehicles != null) && (dsVehicles.Tables.Count > 0))
            {
                string licensePlate = "";
                foreach (DataRow ittrRow in dsVehicles.Tables[0].Rows)
                {
                    licensePlate = ittrRow["LicensePlate"].ToString().TrimEnd();
                    currSqlRecords = 0;
                    // 2. Retrieves trip info for every vehicle
                    dsCurrVehicleTrips = GetTripReport(licensePlate,
                       fromDateTime,
                       toDateTime,
                       userId,
                       showLastStoredPosition, lang,
                       ref requestOverflowed,
                       ref currSqlRecords,
                       ref outMaxOverflowed,
                       ref outMaxRecords);
                    totalSqlRecords += currSqlRecords;
                    outMaxFleetRecords += outMaxRecords;

                    if (sqlMaxOutRecords < outMaxFleetRecords)
                        outMaxOverflowed = true;

                    if (requestOverflowed || outMaxOverflowed)
                    {
                        dsFleetTips.Clear();
                        break;
                    }

                    // 3. Merge vehicle trips info into fleet trips info
                    if (dsFleetTips != null && dsCurrVehicleTrips != null)
                        dsFleetTips.Merge(dsCurrVehicleTrips);
                }
            }
            dsFleetTips.DataSetName = "DsFleetDetailedTripReport";
            return dsFleetTips;
        }

        // Changes for TimeZone Feature start
        public DataSet GetFleetTripReport_NewTZ(int fleetId,
                                 string fromDateTime,
                                 string toDateTime,
                                 int userId,
                                 bool showLastStoredPosition,
                                 int sensorId,
                                 string lang,
                                 ref bool requestOverflowed,
                                 ref int totalSqlRecords,
                                 ref bool outMaxOverflowed,
                                 ref int outMaxFleetRecords)
        {
            int sqlMaxOutRecords = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Max Output Records", 1000);
            int outMaxRecords = 0;
            int currSqlRecords = 0;
            outMaxFleetRecords = 0;
            outMaxOverflowed = false;
            DataSet dsFleetTips = new DataSet();
            DataSet dsCurrVehicleTrips = null;
            // 1. Retrieve fleet vehicles info (license plates)
            DB.FleetVehicles fleet = new DB.FleetVehicles(sqlExec);
            DataSet dsVehicles = fleet.GetVehiclesInfoByFleetId(fleetId);
            if ((dsVehicles != null) && (dsVehicles.Tables.Count > 0))
            {
                string licensePlate = "";
                foreach (DataRow ittrRow in dsVehicles.Tables[0].Rows)
                {
                    licensePlate = ittrRow["LicensePlate"].ToString().TrimEnd();
                    currSqlRecords = 0;
                    // 2. Retrieves trip info for every vehicle
                    dsCurrVehicleTrips = GetTripReport_NewTZ(licensePlate,
                                                        fromDateTime,
                                                        toDateTime,
                                                        userId,
                                                        showLastStoredPosition, sensorId, lang,
                                                        ref requestOverflowed,
                                                        ref currSqlRecords,
                                                        ref outMaxOverflowed,
                                                        ref outMaxRecords);
                    totalSqlRecords += currSqlRecords;
                    outMaxFleetRecords += outMaxRecords;

                    if (sqlMaxOutRecords < outMaxFleetRecords)
                        outMaxOverflowed = true;

                    if (requestOverflowed || outMaxOverflowed)
                    {
                        dsFleetTips.Clear();
                        break;
                    }

                    // 3. Merge vehicle trips info into fleet trips info
                    if (dsFleetTips != null && dsCurrVehicleTrips != null)
                        dsFleetTips.Merge(dsCurrVehicleTrips);
                }
            }
            dsFleetTips.DataSetName = "DsFleetDetailedTripReport";
            return dsFleetTips;
        }
        // Changes for TimeZone Feature end


        public DataSet GetFleetTripReport(int fleetId,
                                         string fromDateTime,
                                         string toDateTime,
                                         int userId,
                                         bool showLastStoredPosition,
                                         int sensorId,
                                         string lang,
                                         ref bool requestOverflowed,
                                         ref int totalSqlRecords,
                                         ref bool outMaxOverflowed,
                                         ref int outMaxFleetRecords)
        {
            int sqlMaxOutRecords = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Max Output Records", 1000);
            int outMaxRecords = 0;
            int currSqlRecords = 0;
            outMaxFleetRecords = 0;
            outMaxOverflowed = false;
            DataSet dsFleetTips = new DataSet();
            DataSet dsCurrVehicleTrips = null;
            // 1. Retrieve fleet vehicles info (license plates)
            DB.FleetVehicles fleet = new DB.FleetVehicles(sqlExec);
            DataSet dsVehicles = fleet.GetVehiclesInfoByFleetId(fleetId);
            if ((dsVehicles != null) && (dsVehicles.Tables.Count > 0))
            {
                string licensePlate = "";
                foreach (DataRow ittrRow in dsVehicles.Tables[0].Rows)
                {
                    licensePlate = ittrRow["LicensePlate"].ToString().TrimEnd();
                    currSqlRecords = 0;
                    // 2. Retrieves trip info for every vehicle
                    dsCurrVehicleTrips = GetTripReport(licensePlate,
                                                        fromDateTime,
                                                        toDateTime,
                                                        userId,
                                                        showLastStoredPosition, sensorId, lang,
                                                        ref requestOverflowed,
                                                        ref currSqlRecords,
                                                        ref outMaxOverflowed,
                                                        ref outMaxRecords);
                    totalSqlRecords += currSqlRecords;
                    outMaxFleetRecords += outMaxRecords;

                    if (sqlMaxOutRecords < outMaxFleetRecords)
                        outMaxOverflowed = true;

                    if (requestOverflowed || outMaxOverflowed)
                    {
                        dsFleetTips.Clear();
                        break;
                    }

                    // 3. Merge vehicle trips info into fleet trips info
                    if (dsFleetTips != null && dsCurrVehicleTrips != null)
                        dsFleetTips.Merge(dsCurrVehicleTrips);
                }
            }
            dsFleetTips.DataSetName = "DsFleetDetailedTripReport";
            return dsFleetTips;
        }

        /// <summary>
        /// Prepares fleet stop  report
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="showLastStoredPosition"></param>
        /// <param name="minStopDuration"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns> DataSet [StopIndex],[ArrivalDateTime],[Location],
        /// [DepartureDateTime],[StopDuration],[Remarks],[Latitude],[Longitude] 
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetFleetStopReport(int fleetId,
                                         string fromDateTime,
                                         string toDateTime,
                                         int userId,
                                         bool showLastStoredPosition,
                                         int minStopDuration,//in sec
                                         bool inclStop,
                                         bool inclIdling,
                                         string lang,
                                         ref bool requestOverflowed,
                                         ref int totalSqlRecords)
        {
            DataSet dsFleetStops = new DataSet();
            DataSet dsCurrVehicleStops = null;
            int currSqlRecords = 0;
            // 1. Retrieve fleet vehicles info (license plates)
            DB.FleetVehicles fleet = new DB.FleetVehicles(sqlExec);
            DataSet dsVehicles = fleet.GetVehiclesInfoByFleetId(fleetId);
            if ((dsVehicles != null) && (dsVehicles.Tables.Count > 0))
            {
                string licensePlate = "";
                foreach (DataRow ittrRow in dsVehicles.Tables[0].Rows)
                {
                    licensePlate = ittrRow["LicensePlate"].ToString().TrimEnd();
                    currSqlRecords = 0;
                    // 2. Retrieves stop info for every vehicle
                    dsCurrVehicleStops = GetStopReport(licensePlate,
                                                        fromDateTime,
                                                        toDateTime,
                                                        userId,
                                                        showLastStoredPosition,
                                                        minStopDuration,
                                                        inclStop, inclIdling, lang,
                                                        ref requestOverflowed,
                                                        ref currSqlRecords);
                    totalSqlRecords += currSqlRecords;
                    if (requestOverflowed)
                    {
                        dsFleetStops.Clear();
                        break;
                    }

                    // 3. Merge vehicle stops info into fleet stops info
                    if (dsFleetStops != null && dsCurrVehicleStops != null)
                        dsFleetStops.Merge(dsCurrVehicleStops);
                }
            }
            dsFleetStops.DataSetName = "DsFleetStopReport";
            return dsFleetStops;
        }

        // Changes for TimeZone Feature start
        public DataSet GetFleetStopReport_NewTZ(int fleetId,
                                  string fromDateTime,
                                  string toDateTime,
                                  int userId,
                                  bool showLastStoredPosition,
                                  int minStopDuration,//in sec
                                  bool inclStop,
                                  bool inclIdling,
                                  int sensorId,
                                  string lang,
                                  ref bool requestOverflowed,
                                  ref int totalSqlRecords)
        {
            DataSet dsFleetStops = new DataSet();
            DataSet dsCurrVehicleStops = null;
            int currSqlRecords = 0;
            // 1. Retrieve fleet vehicles info (license plates)
            DB.FleetVehicles fleet = new DB.FleetVehicles(sqlExec);
            DataSet dsVehicles = fleet.GetVehiclesInfoByFleetId(fleetId);
            if ((dsVehicles != null) && (dsVehicles.Tables.Count > 0))
            {
                string licensePlate = "";
                foreach (DataRow ittrRow in dsVehicles.Tables[0].Rows)
                {
                    licensePlate = ittrRow["LicensePlate"].ToString().TrimEnd();
                    currSqlRecords = 0;
                    // 2. Retrieves stop info for every vehicle
                    dsCurrVehicleStops = GetStopReport_NewTZ(licensePlate,
                                                        fromDateTime,
                                                        toDateTime,
                                                        userId,
                                                        showLastStoredPosition,
                                                        minStopDuration,
                                                        inclStop, inclIdling, sensorId, lang,
                                                        ref requestOverflowed,
                                                        ref currSqlRecords);
                    totalSqlRecords += currSqlRecords;
                    if (requestOverflowed)
                    {
                        dsFleetStops.Clear();
                        break;
                    }

                    // 3. Merge vehicle stops info into fleet stops info
                    if (dsFleetStops != null && dsCurrVehicleStops != null)
                        dsFleetStops.Merge(dsCurrVehicleStops);
                }
            }
            dsFleetStops.DataSetName = "DsFleetStopReport";
            return dsFleetStops;
        }
        // Changes for TimeZone Feature end


        public DataSet GetFleetStopReport(int fleetId,
                                         string fromDateTime,
                                         string toDateTime,
                                         int userId,
                                         bool showLastStoredPosition,
                                         int minStopDuration,//in sec
                                         bool inclStop,
                                         bool inclIdling,
                                         int sensorId,
                                         string lang,
                                         ref bool requestOverflowed,
                                         ref int totalSqlRecords)
        {
            DataSet dsFleetStops = new DataSet();
            DataSet dsCurrVehicleStops = null;
            int currSqlRecords = 0;
            // 1. Retrieve fleet vehicles info (license plates)
            DB.FleetVehicles fleet = new DB.FleetVehicles(sqlExec);
            DataSet dsVehicles = fleet.GetVehiclesInfoByFleetId(fleetId);
            if ((dsVehicles != null) && (dsVehicles.Tables.Count > 0))
            {
                string licensePlate = "";
                foreach (DataRow ittrRow in dsVehicles.Tables[0].Rows)
                {
                    licensePlate = ittrRow["LicensePlate"].ToString().TrimEnd();
                    currSqlRecords = 0;
                    // 2. Retrieves stop info for every vehicle
                    dsCurrVehicleStops = GetStopReport(licensePlate,
                                                        fromDateTime,
                                                        toDateTime,
                                                        userId,
                                                        showLastStoredPosition,
                                                        minStopDuration,
                                                        inclStop, inclIdling, sensorId, lang,
                                                        ref requestOverflowed,
                                                        ref currSqlRecords);
                    totalSqlRecords += currSqlRecords;
                    if (requestOverflowed)
                    {
                        dsFleetStops.Clear();
                        break;
                    }

                    // 3. Merge vehicle stops info into fleet stops info
                    if (dsFleetStops != null && dsCurrVehicleStops != null)
                        dsFleetStops.Merge(dsCurrVehicleStops);
                }
            }
            dsFleetStops.DataSetName = "DsFleetStopReport";
            return dsFleetStops;
        }

        /// <summary>
        /// Prepares fleet stop report by landmark
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="showLastStoredPosition"></param>
        /// <param name="minStopDuration"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns> DataSet [StopIndex],[ArrivalDateTime],[Location],
        /// [DepartureDateTime],[StopDuration],[Remarks],[Latitude],[Longitude] 
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetFleetStopReportByLandmark(int fleetId,
           string fromDateTime,
           string toDateTime,
           int userId,
           bool showLastStoredPosition,
           int minStopDuration, string lang,
              ref bool requestOverflowed,
           ref int totalSqlRecords)
        {
            DataSet dsFleetStops = new DataSet();
            DataSet dsCurrVehicleStops = null;
            int currSqlRecords = 0;
            // 1. Retrieve fleet vehicles info (license plates)
            DB.FleetVehicles fleet = new DB.FleetVehicles(sqlExec);
            DataSet dsVehicles = fleet.GetVehiclesInfoByFleetId(fleetId);
            if ((dsVehicles != null) && (dsVehicles.Tables.Count > 0))
            {
                string licensePlate = "";
                foreach (DataRow ittrRow in dsVehicles.Tables[0].Rows)
                {
                    licensePlate = ittrRow["LicensePlate"].ToString().TrimEnd();
                    currSqlRecords = 0;
                    // 2. Retrieves stop info for every vehicle
                    dsCurrVehicleStops = GetStopReportByLandmark(licensePlate,
                       fromDateTime,
                       toDateTime,
                       userId,
                       showLastStoredPosition,
                       minStopDuration, lang,
                             ref requestOverflowed,
                       ref currSqlRecords);
                    totalSqlRecords += currSqlRecords;
                    if (requestOverflowed)
                    {
                        dsFleetStops.Clear();
                        break;
                    }

                    // 3. Merge vehicle stops info into fleet stops info
                    if (dsFleetStops != null && dsCurrVehicleStops != null)
                        dsFleetStops.Merge(dsCurrVehicleStops);
                }
            }
            dsFleetStops.DataSetName = "DsFleetStopReportByLandmark";
            return dsFleetStops;
        }

        /// <summary>
        /// Prepares detailed trip report
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="includeStreetAddress"></param>
        /// <param name="includeSensors"></param>
        /// <param name="includePosition"></param>
        /// <param name="includeIdleTime"></param>
        /// <param name="includeSummary"></param>
        /// <param name="showLastStoredPosition"></param>
        /// <param name="userId"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <param name="outMaxOverflowed"></param>
        /// <param name="outMaxRecords"></param>
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
        /// </remarks>
        /// <returns> DataSet [TripIndex],[Reson],[Date/Time],[Location],[Speed],[Description] </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetDetailedTripReport(string licensePlate,
           string fromDateTime,
           string toDateTime,
           bool includeStreetAddress,
           bool includeSensors,
           bool includePosition,
           bool includeIdleTime,
           bool includeSummary,
           bool showLastStoredPosition,
           int userId, string lang,
           ref bool requestOverflowed,
           ref int totalSqlRecords,
           ref bool outMaxOverflowed,
           ref int outMaxRecords)
        {
            int sqlMaxOutRecords = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Max Output Records", 1000);
            DataSet dsResult = new DataSet();
            DataTable tblVehicleSensors = null;
            DataTable tblVehicleGeozones = null;
            double carCost = 1;
            double measurementUnits = 1;
            outMaxRecords = 0;
#if NEW_REPORTS
            short vehicleType = (short)VLF.CLS.Def.Enums.VehicleType.NotKnown;
            FillTripReportParams(licensePlate, dsResult, userId, false, ref tblVehicleSensors, ref tblVehicleGeozones, ref carCost, ref measurementUnits, ref vehicleType);
            report.GetDetailedTripReport(licensePlate,
                                          fromDateTime,
                                          toDateTime,
                                          tblVehicleSensors,
                                          tblVehicleGeozones,
                                          includeStreetAddress,
                                          includeSensors,
                                          includePosition,
                                          includeIdleTime,
                                          includeSummary,
                                          showLastStoredPosition,
                                          carCost,
                                          userId,
                                          measurementUnits,
                                          dsResult,
                                          vehicleType, lang,
                                          ref requestOverflowed,
                                          ref totalSqlRecords);
#else
         bool isTrailer = false;
			FillTripReportParams(licensePlate,dsResult,userId,false,ref tblVehicleSensors,ref tblVehicleGeozones,ref carCost,ref measurementUnits,ref isTrailer);
			report.GetDetailedTripReport(licensePlate,
				fromDateTime,
				toDateTime,
				tblVehicleSensors,
				tblVehicleGeozones,
				includeStreetAddress,
				includeSensors,
				includePosition,
				includeIdleTime,
				includeSummary,
				showLastStoredPosition,
				carCost,
				userId,
				measurementUnits,
				dsResult,
				isTrailer,
				ref requestOverflowed,
				ref totalSqlRecords);
#endif
            dsResult.DataSetName = "DsDetailedTripReport";
            foreach (DataTable ittr in dsResult.Tables)
                outMaxRecords += ittr.Rows.Count;

            if (sqlMaxOutRecords < outMaxRecords)
            {
                outMaxOverflowed = true;
                dsResult.Clear();
            }

            return dsResult;
        }

        // Changes for TimeZone Feature start
        public DataSet GetDetailedTripReport_NewTZ(string licensePlate,
         string fromDateTime,
         string toDateTime,
         bool includeStreetAddress,
         bool includeSensors,
         bool includePosition,
         bool includeIdleTime,
         bool includeSummary,
         bool showLastStoredPosition,
         int userId,
         int sensorId,
         string lang,
         ref bool requestOverflowed,
         ref int totalSqlRecords,
         ref bool outMaxOverflowed,
         ref int outMaxRecords)
        {
            int sqlMaxOutRecords = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Max Output Records", 1000);
            DataSet dsResult = new DataSet();
            DataTable tblVehicleSensors = null;
            DataTable tblVehicleGeozones = null;
            double carCost = 1;
            double measurementUnits = 1;
            outMaxRecords = 0;

            short vehicleType = (short)VLF.CLS.Def.Enums.VehicleType.NotKnown;
            FillTripReportParams_NewTZ(licensePlate, dsResult, userId, false, ref tblVehicleSensors, ref tblVehicleGeozones, ref carCost, ref measurementUnits, ref vehicleType);
            report.GetDetailedTripReport_NewTZ(licensePlate,
                                          fromDateTime,
                                          toDateTime,
                                          tblVehicleSensors,
                                          tblVehicleGeozones,
                                          includeStreetAddress,
                                          includeSensors,
                                          includePosition,
                                          includeIdleTime,
                                          includeSummary,
                                          showLastStoredPosition,
                                          carCost,
                                          userId,
                                          measurementUnits,
                                          dsResult,
                                          vehicleType,
                                          sensorId,
                                          lang,
                                          ref requestOverflowed,
                                          ref totalSqlRecords);

            dsResult.DataSetName = "DsDetailedTripReport";
            foreach (DataTable ittr in dsResult.Tables)
                outMaxRecords += ittr.Rows.Count;

            if (sqlMaxOutRecords < outMaxRecords)
            {
                outMaxOverflowed = true;
                dsResult.Clear();
            }

            return dsResult;
        }
        // Changes for TimeZone Feature end

        public DataSet GetDetailedTripReport(string licensePlate,
                 string fromDateTime,
                 string toDateTime,
                 bool includeStreetAddress,
                 bool includeSensors,
                 bool includePosition,
                 bool includeIdleTime,
                 bool includeSummary,
                 bool showLastStoredPosition,
                 int userId,
                 int sensorId,
                 string lang,
                 ref bool requestOverflowed,
                 ref int totalSqlRecords,
                 ref bool outMaxOverflowed,
                 ref int outMaxRecords)
        {
            int sqlMaxOutRecords = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Max Output Records", 1000);
            DataSet dsResult = new DataSet();
            DataTable tblVehicleSensors = null;
            DataTable tblVehicleGeozones = null;
            double carCost = 1;
            double measurementUnits = 1;
            outMaxRecords = 0;

            short vehicleType = (short)VLF.CLS.Def.Enums.VehicleType.NotKnown;
            FillTripReportParams(licensePlate, dsResult, userId, false, ref tblVehicleSensors, ref tblVehicleGeozones, ref carCost, ref measurementUnits, ref vehicleType);
            report.GetDetailedTripReport(licensePlate,
                                          fromDateTime,
                                          toDateTime,
                                          tblVehicleSensors,
                                          tblVehicleGeozones,
                                          includeStreetAddress,
                                          includeSensors,
                                          includePosition,
                                          includeIdleTime,
                                          includeSummary,
                                          showLastStoredPosition,
                                          carCost,
                                          userId,
                                          measurementUnits,
                                          dsResult,
                                          vehicleType,
                                          sensorId,
                                          lang,
                                          ref requestOverflowed,
                                          ref totalSqlRecords);

            dsResult.DataSetName = "DsDetailedTripReport";
            foreach (DataTable ittr in dsResult.Tables)
                outMaxRecords += ittr.Rows.Count;

            if (sqlMaxOutRecords < outMaxRecords)
            {
                outMaxOverflowed = true;
                dsResult.Clear();
            }

            return dsResult;
        }

        /// <summary>
        /// Prepares trip report
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="showLastStoredPosition"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <param name="outMaxOverflowed"></param>
        /// <param name="outMaxRecords"></param>
        /// <remarks>
        /// 1. On Ignition On satrt trip
        /// 2. On Ignition Off stop trip
        /// 3. Calculates trip statistics:
        ///		3.1 Trip Duration
        ///		3.2 Trip Distance
        ///		3.3 Trip Average Speed
        ///		3.4 Trip Stops
        ///		3.5 Trip Cost
        /// </remarks>
        /// <returns> DataSet [TripIndex],[Reson],[Date/Time],[Location],[Speed],[Description] </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetTripReport(string licensePlate,
                                      string fromDateTime,
                                      string toDateTime,
                                      int userId,
                                      bool showLastStoredPosition, string lang,
                                      ref bool requestOverflowed,
                                      ref int totalSqlRecords,
                                      ref bool outMaxOverflowed,
                                      ref int outMaxRecords)
        {
            int sqlMaxOutRecords = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Max Output Records", 1000);
            outMaxRecords = 0;
            DataSet dsResult = new DataSet();
            DataTable tblVehicleSensors = null;
            DataTable tblVehicleGeozones = null;
            double carCost = 1;
            double measurementUnits = 1;

#if NEW_REPORTS
            short vehicleType = (short)VLF.CLS.Def.Enums.VehicleType.NotKnown;
            FillTripReportParams(licensePlate, dsResult, userId, false, ref tblVehicleSensors, ref tblVehicleGeozones, ref carCost, ref measurementUnits, ref vehicleType);
            report.GetTripReport(licensePlate,
                                 fromDateTime,
                                 toDateTime,
                                 tblVehicleSensors,
                                 tblVehicleGeozones,
                                 carCost,
                                 userId,
                                 measurementUnits,
                                 dsResult,
                                 showLastStoredPosition,
                                 vehicleType, lang,
                                 ref requestOverflowed,
                                 ref totalSqlRecords);

#else
         bool isTrailer = false;
			FillTripReportParams(licensePlate,dsResult,userId,false,ref tblVehicleSensors,ref tblVehicleGeozones,ref carCost,ref measurementUnits,ref isTrailer);
			report.GetTripReport(licensePlate,
				fromDateTime,
				toDateTime,
				tblVehicleSensors,
				tblVehicleGeozones,
				carCost,
				userId,
				measurementUnits,
				dsResult,
				showLastStoredPosition,
				isTrailer,
				ref requestOverflowed,
				ref totalSqlRecords);
#endif
            dsResult.DataSetName = "DsTripReport";

            foreach (DataTable ittr in dsResult.Tables)
                outMaxRecords += ittr.Rows.Count;

            if (sqlMaxOutRecords < outMaxRecords)
            {
                outMaxOverflowed = true;
                dsResult.Clear();
            }

            return dsResult;
        }
        // Changes for TimeZone Feature start
        public DataSet GetTripReport_NewTZ(string licensePlate,
                              string fromDateTime,
                              string toDateTime,
                              int userId,
                              bool showLastStoredPosition,
                              int sensorId,
                              string lang,
                              ref bool requestOverflowed,
                              ref int totalSqlRecords,
                              ref bool outMaxOverflowed,
                              ref int outMaxRecords)
        {
            int sqlMaxOutRecords = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Max Output Records", 1000);
            outMaxRecords = 0;
            DataSet dsResult = new DataSet();
            DataTable tblVehicleSensors = null;
            DataTable tblVehicleGeozones = null;
            double carCost = 1;
            double measurementUnits = 1;

#if NEW_REPORTS
            short vehicleType = (short)VLF.CLS.Def.Enums.VehicleType.NotKnown;
            FillTripReportParams_NewTZ(licensePlate, dsResult, userId, false, ref tblVehicleSensors, ref tblVehicleGeozones, ref carCost, ref measurementUnits, ref vehicleType);
            report.GetTripReport_NewTZ(licensePlate,
                                 fromDateTime,
                                 toDateTime,
                                 tblVehicleSensors,
                                 tblVehicleGeozones,
                                 carCost,
                                 userId,
                                 measurementUnits,
                                 dsResult,
                                 showLastStoredPosition,
                                 vehicleType, sensorId, lang,
                                 ref requestOverflowed,
                                 ref totalSqlRecords);

#else
         bool isTrailer = false;
			FillTripReportParams_NewTZ(licensePlate,dsResult,userId,false,ref tblVehicleSensors,ref tblVehicleGeozones,ref carCost,ref measurementUnits,ref isTrailer);
			report.GetTripReport_NewTZ(licensePlate,
				fromDateTime,
				toDateTime,
				tblVehicleSensors,
				tblVehicleGeozones,
				carCost,
				userId,
				measurementUnits,
				dsResult,
				showLastStoredPosition,
				isTrailer,
				ref requestOverflowed,
				ref totalSqlRecords);
#endif
            dsResult.DataSetName = "DsTripReport";

            foreach (DataTable ittr in dsResult.Tables)
                outMaxRecords += ittr.Rows.Count;

            if (sqlMaxOutRecords < outMaxRecords)
            {
                outMaxOverflowed = true;
                dsResult.Clear();
            }

            return dsResult;
        }

        // Changes for TimeZone Feature end


        public DataSet GetTripReport(string licensePlate,
                                      string fromDateTime,
                                      string toDateTime,
                                      int userId,
                                      bool showLastStoredPosition,
                                      int sensorId,
                                      string lang,
                                      ref bool requestOverflowed,
                                      ref int totalSqlRecords,
                                      ref bool outMaxOverflowed,
                                      ref int outMaxRecords)
        {
            int sqlMaxOutRecords = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Max Output Records", 1000);
            outMaxRecords = 0;
            DataSet dsResult = new DataSet();
            DataTable tblVehicleSensors = null;
            DataTable tblVehicleGeozones = null;
            double carCost = 1;
            double measurementUnits = 1;

#if NEW_REPORTS
            short vehicleType = (short)VLF.CLS.Def.Enums.VehicleType.NotKnown;
            FillTripReportParams(licensePlate, dsResult, userId, false, ref tblVehicleSensors, ref tblVehicleGeozones, ref carCost, ref measurementUnits, ref vehicleType);
            report.GetTripReport(licensePlate,
                                 fromDateTime,
                                 toDateTime,
                                 tblVehicleSensors,
                                 tblVehicleGeozones,
                                 carCost,
                                 userId,
                                 measurementUnits,
                                 dsResult,
                                 showLastStoredPosition,
                                 vehicleType, sensorId, lang,
                                 ref requestOverflowed,
                                 ref totalSqlRecords);

#else
         bool isTrailer = false;
			FillTripReportParams(licensePlate,dsResult,userId,false,ref tblVehicleSensors,ref tblVehicleGeozones,ref carCost,ref measurementUnits,ref isTrailer);
			report.GetTripReport(licensePlate,
				fromDateTime,
				toDateTime,
				tblVehicleSensors,
				tblVehicleGeozones,
				carCost,
				userId,
				measurementUnits,
				dsResult,
				showLastStoredPosition,
				isTrailer,
				ref requestOverflowed,
				ref totalSqlRecords);
#endif
            dsResult.DataSetName = "DsTripReport";

            foreach (DataTable ittr in dsResult.Tables)
                outMaxRecords += ittr.Rows.Count;

            if (sqlMaxOutRecords < outMaxRecords)
            {
                outMaxOverflowed = true;
                dsResult.Clear();
            }

            return dsResult;
        }

        /// <summary>
        /// Prepares stop report
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="showLastStoredPosition"></param>
        /// <param name="minStopDuration"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        /// <returns> DataSet [StopIndex],[ArrivalDateTime],[Location],[DepartureDateTime],
        /// [StopDuration],[Remarks],[Latitude],[Longitude] </returns>
        public DataSet GetStopReport(string licensePlate,
                                      string fromDateTime,
                                      string toDateTime,
                                      int userId,
                                      bool showLastStoredPosition,
                                      int minStopDuration,          //in sec
                                      bool inclStop,
                                      bool inclIdling, string lang,
                                      ref bool requestOverflowed,
                                      ref int totalSqlRecords)
        {
            DataSet dsResult = new DataSet();
            DataTable tblVehicleSensors = null;
            DataTable tblVehicleGeozones = null;
            double carCost = 1;
            double measurementUnits = 1;

#if NEW_REPORTS
            short vehicleType = (short)VLF.CLS.Def.Enums.VehicleType.NotKnown;
            FillTripReportParams(licensePlate, dsResult, userId, false, ref tblVehicleSensors, ref tblVehicleGeozones, ref carCost, ref measurementUnits, ref vehicleType);
            report.GetStopReport(licensePlate,
                                 fromDateTime,
                                 toDateTime,
                                 tblVehicleSensors,
                                 tblVehicleGeozones,
                                 carCost,
                                 userId,
                                 measurementUnits,
                                 dsResult,
                                 showLastStoredPosition,
                                 minStopDuration,//in sec
                                 vehicleType,
                                 inclStop, inclIdling, lang,
                                 ref requestOverflowed,
                                 ref totalSqlRecords);

#else
         bool isTrailer = false;
			FillTripReportParams(licensePlate,dsResult,userId,false,ref tblVehicleSensors,ref tblVehicleGeozones,ref carCost,ref measurementUnits,ref isTrailer);
			report.GetStopReport(licensePlate,
				                  fromDateTime,
				                  toDateTime,
				                  tblVehicleSensors,
				                  tblVehicleGeozones,
				                  carCost,
				                  userId,
				                  measurementUnits,
				                  dsResult,
				                  showLastStoredPosition,
				                  minStopDuration,//in sec
				                  isTrailer,
                              inclStop, inclIdling,
				                  ref requestOverflowed,
				                  ref totalSqlRecords);
#endif
            dsResult.DataSetName = "DsStopReport";
            return dsResult;
        }

        // Changes for TimeZone Feature start
        public DataSet GetStopReport_NewTZ(string licensePlate,
                                    string fromDateTime,
                                    string toDateTime,
                                    int userId,
                                    bool showLastStoredPosition,
                                    int minStopDuration,          //in sec
                                    bool inclStop,
                                    bool inclIdling,
                                    int sensorId,
                                    string lang,
                                    ref bool requestOverflowed,
                                    ref int totalSqlRecords)
        {
            DataSet dsResult = new DataSet();
            DataTable tblVehicleSensors = null;
            DataTable tblVehicleGeozones = null;
            double carCost = 1;
            double measurementUnits = 1;

#if NEW_REPORTS
            short vehicleType = (short)VLF.CLS.Def.Enums.VehicleType.NotKnown;
            FillTripReportParams_NewTZ(licensePlate, dsResult, userId, false, ref tblVehicleSensors, ref tblVehicleGeozones, ref carCost, ref measurementUnits, ref vehicleType);
            report.GetStopReport_NewTZ(licensePlate,
                                 fromDateTime,
                                 toDateTime,
                                 tblVehicleSensors,
                                 tblVehicleGeozones,
                                 carCost,
                                 userId,
                                 measurementUnits,
                                 dsResult,
                                 showLastStoredPosition,
                                 minStopDuration,//in sec
                                 vehicleType,
                                 inclStop, inclIdling,
                                 sensorId, lang,
                                 ref requestOverflowed,
                                 ref totalSqlRecords);

#else
         bool isTrailer = false;
			FillTripReportParams_NewTZ(licensePlate,dsResult,userId,false,ref tblVehicleSensors,ref tblVehicleGeozones,ref carCost,ref measurementUnits,ref isTrailer);
			report.GetStopReport_NewTZ(licensePlate,
				                  fromDateTime,
				                  toDateTime,
				                  tblVehicleSensors,
				                  tblVehicleGeozones,
				                  carCost,
				                  userId,
				                  measurementUnits,
				                  dsResult,
				                  showLastStoredPosition,
				                  minStopDuration,//in sec
				                  isTrailer,
                              inclStop, inclIdling,
				                  ref requestOverflowed,
				                  ref totalSqlRecords);
#endif
            dsResult.DataSetName = "DsStopReport";
            return dsResult;
        }
        // Changes for TimeZone Feature end

        public DataSet GetStopReport(string licensePlate,
                                      string fromDateTime,
                                      string toDateTime,
                                      int userId,
                                      bool showLastStoredPosition,
                                      int minStopDuration,          //in sec
                                      bool inclStop,
                                      bool inclIdling,
                                      int sensorId,
                                      string lang,
                                      ref bool requestOverflowed,
                                      ref int totalSqlRecords)
        {
            DataSet dsResult = new DataSet();
            DataTable tblVehicleSensors = null;
            DataTable tblVehicleGeozones = null;
            double carCost = 1;
            double measurementUnits = 1;

#if NEW_REPORTS
            short vehicleType = (short)VLF.CLS.Def.Enums.VehicleType.NotKnown;
            FillTripReportParams(licensePlate, dsResult, userId, false, ref tblVehicleSensors, ref tblVehicleGeozones, ref carCost, ref measurementUnits, ref vehicleType);
            report.GetStopReport(licensePlate,
                                 fromDateTime,
                                 toDateTime,
                                 tblVehicleSensors,
                                 tblVehicleGeozones,
                                 carCost,
                                 userId,
                                 measurementUnits,
                                 dsResult,
                                 showLastStoredPosition,
                                 minStopDuration,//in sec
                                 vehicleType,
                                 inclStop, inclIdling,
                                 sensorId, lang,
                                 ref requestOverflowed,
                                 ref totalSqlRecords);

#else
         bool isTrailer = false;
			FillTripReportParams(licensePlate,dsResult,userId,false,ref tblVehicleSensors,ref tblVehicleGeozones,ref carCost,ref measurementUnits,ref isTrailer);
			report.GetStopReport(licensePlate,
				                  fromDateTime,
				                  toDateTime,
				                  tblVehicleSensors,
				                  tblVehicleGeozones,
				                  carCost,
				                  userId,
				                  measurementUnits,
				                  dsResult,
				                  showLastStoredPosition,
				                  minStopDuration,//in sec
				                  isTrailer,
                              inclStop, inclIdling,
				                  ref requestOverflowed,
				                  ref totalSqlRecords);
#endif
            dsResult.DataSetName = "DsStopReport";
            return dsResult;
        }

        /// <summary>
        /// Prepares Off Hour report
        /// </summary>
        /// <summary>
        /// Prepares trip report
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="showLastStoredPosition"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <param name="outMaxOverflowed"></param>
        /// <param name="outMaxRecords"></param>
        /// <remarks>
        /// 1. On Ignition On satrt trip
        /// 2. On Ignition Off stop trip
        /// 3. Calculates trip statistics:
        ///		3.1 Trip Duration
        ///		3.2 Trip Distance
        ///		3.3 Trip Average Speed
        ///		3.4 Trip Stops
        ///		3.5 Trip Cost
        /// </remarks>
        /// <returns> DataSet [TripIndex],[Reson],[Date/Time],[Location],[Speed],[Description] </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetOffHourReport(
           string licensePlate,
           string fromDateTime,
           string toDateTime,
           int userId,
           bool showLastStoredPosition,
           short dayFromHour, short dayFromMin, short dayToHour, short dayToMin, short weekendFromHour, short weekendFromMin, short weekendToHour, short weekendToMin, string lang,
           ref bool requestOverflowed,
           ref int totalSqlRecords,
           ref bool outMaxOverflowed,
           ref int outMaxRecords)
        {
            DataSet dsCurrVehicleTrips = new DataSet();
            dsCurrVehicleTrips = GetTripReport(licensePlate, fromDateTime, toDateTime, userId, showLastStoredPosition, lang, ref requestOverflowed, ref totalSqlRecords, ref outMaxOverflowed, ref outMaxRecords);
            #region Search for trips inside working hours
            if (dsCurrVehicleTrips != null && dsCurrVehicleTrips.Tables.Contains("TripStart") && dsCurrVehicleTrips.Tables.Contains("TripEnd"))
            {
                for (int startIndex = 0; startIndex < dsCurrVehicleTrips.Tables["TripStart"].Rows.Count; ++startIndex)
                {
                    if (startIndex < dsCurrVehicleTrips.Tables["TripEnd"].Rows.Count)
                    {
                        #region Setup from DateTime borders
                        DateTime departureDateTime = Convert.ToDateTime(dsCurrVehicleTrips.Tables["TripStart"].Rows[startIndex]["Summary"]);
                        DateTime departureDateTimeFrom = departureDateTime;
                        DateTime departureDateTimeTo = departureDateTime;

                        if (departureDateTime.DayOfWeek == DayOfWeek.Sunday || departureDateTime.DayOfWeek == DayOfWeek.Sunday)// weekend
                        {
                            departureDateTimeFrom = departureDateTimeFrom.AddHours(weekendFromHour - departureDateTimeFrom.Hour).AddMinutes(weekendFromMin - departureDateTimeFrom.Minute).AddSeconds(-departureDateTimeFrom.Second);
                            departureDateTimeTo = departureDateTimeTo.AddHours(weekendToHour - departureDateTimeTo.Hour).AddMinutes(weekendToMin - departureDateTimeTo.Minute).AddSeconds(-departureDateTimeTo.Second);
                        }
                        else // weekdays
                        {
                            departureDateTimeFrom = departureDateTimeFrom.AddHours(dayFromHour - departureDateTimeFrom.Hour).AddMinutes(dayFromMin - departureDateTimeFrom.Minute).AddSeconds(-departureDateTimeFrom.Second);
                            departureDateTimeTo = departureDateTimeTo.AddHours(dayToHour - departureDateTimeTo.Hour).AddMinutes(dayToMin - departureDateTimeTo.Minute).AddSeconds(-departureDateTimeTo.Second);
                        }
                        #endregion

                        #region Setup to DateTime borders
                        DateTime arrivalDateTime = Convert.ToDateTime(dsCurrVehicleTrips.Tables["TripEnd"].Rows[startIndex]["Summary"]);
                        DateTime arrivalDateTimeFrom = arrivalDateTime;
                        DateTime arrivalDateTimeTo = arrivalDateTime;
                        if (arrivalDateTime.DayOfWeek == DayOfWeek.Saturday || arrivalDateTime.DayOfWeek == DayOfWeek.Sunday)// weekend
                        {
                            arrivalDateTimeFrom = arrivalDateTimeFrom.AddHours(weekendFromHour - arrivalDateTimeFrom.Hour).AddMinutes(weekendFromMin - arrivalDateTimeFrom.Minute).AddSeconds(-arrivalDateTimeFrom.Second);
                            arrivalDateTimeTo = arrivalDateTimeTo.AddHours(weekendToHour - arrivalDateTimeTo.Hour).AddMinutes(weekendToMin - arrivalDateTimeTo.Minute).AddSeconds(-arrivalDateTimeTo.Second);
                        }
                        else // weekdays
                        {
                            arrivalDateTimeFrom = arrivalDateTimeFrom.AddHours(dayFromHour - arrivalDateTimeFrom.Hour).AddMinutes(dayFromMin - arrivalDateTimeFrom.Minute).AddSeconds(-arrivalDateTimeFrom.Second);
                            arrivalDateTimeTo = arrivalDateTimeTo.AddHours(dayToHour - arrivalDateTimeTo.Hour).AddMinutes(dayToMin - arrivalDateTimeTo.Minute).AddSeconds(-arrivalDateTimeTo.Second);
                        }
                        #endregion

                        #region Do not include to report if all trip was inside working hours
                        //if (departureDateTimeFrom > arrivalDateTimeTo)
                        //    arrivalDateTimeTo = arrivalDateTimeTo.AddDays(1);


                        //TimeSpan ts = departureDateTimeFrom - departureDateTime;

                        // if (ts.Days>0)
                        //  departureDateTimeFrom=departureDateTimeFrom.AddDays(1); 




                        if (departureDateTime.CompareTo(departureDateTimeFrom) >= 0 &&
                           departureDateTime.CompareTo(departureDateTimeTo) <= 0 &&
                           arrivalDateTime.CompareTo(arrivalDateTimeFrom) >= 0 &&
                           arrivalDateTime.CompareTo(arrivalDateTimeTo) <= 0)
                        {
                            dsCurrVehicleTrips.Tables["TripStart"].Rows[startIndex]["IsLandmark"] = false;
                            continue;
                        }




                        //if (departureDateTime > departureDateTimeFrom && arrivalDateTime < arrivalDateTimeTo)
                        //{
                        //    dsCurrVehicleTrips.Tables["TripStart"].Rows[startIndex]["IsLandmark"] = false;
                        //    continue;
                        //}
                        //else if (departureDateTime < departureDateTimeFrom && arrivalDateTime>departureDateTimeFrom &&  arrivalDateTime < arrivalDateTimeTo)
                        //{
                        //    dsCurrVehicleTrips.Tables["TripStart"].Rows[startIndex]["IsLandmark"] = false;
                        //    continue;
                        //}
                        //else if (departureDateTime > departureDateTimeFrom && departureDateTime< arrivalDateTimeTo && arrivalDateTime > arrivalDateTimeTo)
                        //{
                        //    dsCurrVehicleTrips.Tables["TripStart"].Rows[startIndex]["IsLandmark"] = false;
                        //    continue;
                        //}

                        #endregion
                    }
                    dsCurrVehicleTrips.Tables["TripStart"].Rows[startIndex]["IsLandmark"] = true;
                }
            }
            #endregion
            dsCurrVehicleTrips.DataSetName = "DsOffHourReport";
            return dsCurrVehicleTrips;
        }

        /// <summary>
        /// Prepares fleet Off Hour report
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="showLastStoredPosition"></param>
        /// <param name="dayFromHour"></param>
        /// <param name="dayFromMin"></param>
        /// <param name="dayToHour"></param>
        /// <param name="dayToMin"></param>
        /// <param name="weekendFromHour"></param>
        /// <param name="weekendFromMin"></param>
        /// <param name="weekendToHour"></param>
        /// <param name="weekendToMin"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <param name="outMaxOverflowed"></param>
        /// <param name="outMaxFleetRecords"></param>
        /// <returns> DataSet [StopIndex],[ArrivalDateTime],[Location],
        /// [DepartureDateTime],[StopDuration],[Remarks],[Latitude],[Longitude] 
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetFleetOffHourReport(int fleetId,
           string fromDateTime,
           string toDateTime,
           int userId,
           bool showLastStoredPosition,
           short dayFromHour, short dayFromMin, short dayToHour, short dayToMin, short weekendFromHour, short weekendFromMin, short weekendToHour, short weekendToMin, string lang,
           ref bool requestOverflowed,
           ref int totalSqlRecords,
           ref bool outMaxOverflowed,
              ref int outMaxFleetRecords)
        {
            DataSet dsFleetStops = new DataSet();
            DataSet dsCurrVehicleStops = null;
            int sqlMaxOutRecords = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Max Output Records", 1000);
            int outMaxRecords = 0;
            int currSqlRecords = 0;
            outMaxFleetRecords = 0;
            outMaxOverflowed = false;
            // 1. Retrieve fleet vehicles info (license plates)
            DB.FleetVehicles fleet = new DB.FleetVehicles(sqlExec);
            DataSet dsVehicles = fleet.GetVehiclesInfoByFleetId(fleetId);
            if ((dsVehicles != null) && (dsVehicles.Tables.Count > 0))
            {
                string licensePlate = "";
                foreach (DataRow ittrRow in dsVehicles.Tables[0].Rows)
                {
                    licensePlate = ittrRow["LicensePlate"].ToString().TrimEnd();
                    currSqlRecords = 0;
                    // 2. Retrieves stop info for every vehicle
                    dsCurrVehicleStops = GetOffHourReport(licensePlate, fromDateTime, toDateTime, userId, showLastStoredPosition, dayFromHour, dayFromMin, dayToHour, dayToMin, weekendFromHour, weekendFromMin, weekendToHour, weekendToMin, lang, ref requestOverflowed, ref currSqlRecords, ref outMaxOverflowed, ref outMaxRecords);

                    #region Calculate totals
                    totalSqlRecords += currSqlRecords;
                    outMaxFleetRecords += outMaxRecords;

                    if (sqlMaxOutRecords < outMaxFleetRecords)
                        outMaxOverflowed = true;

                    if (requestOverflowed || outMaxOverflowed)
                    {
                        dsFleetStops.Clear();
                        break;
                    }
                    #endregion

                    if (requestOverflowed)
                    {
                        dsFleetStops.Clear();
                        break;
                    }

                    // 3. Merge vehicle stops info into fleet stops info
                    if (dsFleetStops != null && dsCurrVehicleStops != null)
                        dsFleetStops.Merge(dsCurrVehicleStops);
                }
            }
            dsFleetStops.DataSetName = "DsFleetOffHourReport";
            return dsFleetStops;
        }

        /// <summary>
        /// Prepares stop report by landmark
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="showLastStoredPosition"></param>
        /// <param name="minStopDuration"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        /// <returns> DataSet [BoxId],[Location],[StopDuration],[StopDurationVal],[VehicleId] </returns>
        public DataSet GetStopReportByLandmark(string licensePlate, string fromDateTime, string toDateTime, int userId, bool showLastStoredPosition, int minStopDuration, string lang, ref bool requestOverflowed, ref int totalSqlRecords)
        {
            DataSet dsResult = new DataSet();
            DataSet dsStopReport = new DataSet();
            DataTable tblVehicleSensors = null;
            DataTable tblVehicleGeozones = null;
            double carCost = 1;
            double measurementUnits = 1;
#if NEW_REPORTS
            short vehicleType = (short)VLF.CLS.Def.Enums.VehicleType.NotKnown;
            // 1. Resolve landmarks
            FillTripReportParams(licensePlate, dsResult, userId, true,
                                 ref tblVehicleSensors,
                                 ref tblVehicleGeozones,
                                 ref carCost,
                                 ref measurementUnits,
                                 ref vehicleType);

            // 2. Get stop report
            DataTable tblStopData = new DataTable("StopReportByLandmark");
            tblStopData.Columns.Add("BoxId", typeof(string));
            tblStopData.Columns.Add("Location", typeof(string));
            tblStopData.Columns.Add("StopDurationVal", typeof(string));
            tblStopData.Columns.Add("VehicleId", typeof(string));
            dsResult.Tables.Add(tblStopData);
            report.GetStopReport(licensePlate, fromDateTime, toDateTime, tblVehicleSensors, tblVehicleGeozones, carCost, userId, measurementUnits, dsStopReport, showLastStoredPosition, minStopDuration, vehicleType, true, true, lang, ref requestOverflowed, ref totalSqlRecords);

#else
         bool isTrailer = false;
			// 1. Resolve landmarks
			FillTripReportParams(licensePlate,dsResult,userId,true,
                              ref tblVehicleSensors,
                              ref tblVehicleGeozones,
                              ref carCost,
                              ref measurementUnits,
                              ref isTrailer);
			// 2. Get stop report
			DataTable tblStopData = new DataTable("StopReportByLandmark");
			tblStopData.Columns.Add("BoxId", typeof( string ) ) ;	
			tblStopData.Columns.Add("Location", typeof( string ) ) ;	
			tblStopData.Columns.Add("StopDurationVal", typeof( string ) ) ;	
			tblStopData.Columns.Add("VehicleId", typeof( string ) ) ;
			dsResult.Tables.Add(tblStopData);
			report.GetStopReport(licensePlate,fromDateTime,toDateTime,tblVehicleSensors,tblVehicleGeozones,carCost,userId,measurementUnits,dsStopReport,showLastStoredPosition,minStopDuration,isTrailer,true,true,ref requestOverflowed,ref totalSqlRecords);
#endif
            // 3. Calculate stops by landmarks
            if (dsStopReport != null && dsStopReport.Tables.Count > 0 && dsStopReport.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow ittr in dsStopReport.Tables[0].Rows)
                {
                    #region Include landmarks only
                    if (Convert.ToBoolean(ittr["IsLandmark"]))
                    {
                        bool newLocation = true;
                        if (dsResult.Tables["StopReportByLandmark"].Rows.Count > 0)
                        {
                            foreach (DataRow resIttr in dsResult.Tables["StopReportByLandmark"].Rows)
                            {
                                #region Same location
                                if (ittr["Location"].ToString().TrimEnd() == resIttr["Location"].ToString().TrimEnd())
                                {
                                    // same location
                                    resIttr["StopDurationVal"] = Convert.ToInt64(resIttr["StopDurationVal"]) + Convert.ToInt64(ittr["StopDurationVal"]);
                                    newLocation = false;
                                    break;
                                }
                                #endregion
                            }
                        }
                        #region new location
                        if (newLocation)
                        {
                            object[] objRow = new object[dsResult.Tables["StopReportByLandmark"].Columns.Count];
                            objRow[0] = ittr["BoxId"].ToString();
                            objRow[1] = ittr["Location"].ToString();
                            objRow[2] = ittr["StopDurationVal"].ToString();
                            objRow[3] = ittr["VehicleId"].ToString();
                            dsResult.Tables["StopReportByLandmark"].Rows.Add(objRow);
                        }
                        #endregion
                    }
                    #endregion
                }
            }
            dsResult.DataSetName = "DsStopReportByLandmark";
            return dsResult;
        }

        /// <summary>
        /// Prepares trip activity report
        /// </summary>
        /// <remarks>
        /// 1. On Ignition On satrt trip
        /// 2. On Ignition Off stop trip
        /// 3. Calculates trip statistics:
        ///		3.1 Trip Duration
        ///		3.2 Trip Distance
        ///		3.3 Trip Average Speed
        ///		3.4 Trip Stops
        ///		3.5 Trip Cost
        /// </remarks>
        /// <param name="licensePlate"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="showLastStoredPosition"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns> DataSet [TripIndex],[Reson],[Date/Time],[Location],[Speed],[Description] </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetTripActivityReport(string licensePlate,
           string fromDateTime,
           string toDateTime,
           int userId,
           bool showLastStoredPosition, string lang,
           ref bool requestOverflowed,
           ref int totalSqlRecords)
        {
            DataSet dsResult = new DataSet();
            DataTable tblVehicleSensors = null;
            DataTable tblVehicleGeozones = null;
            double carCost = 1;
            double measurementUnits = 1;
#if NEW_REPORTS
            short vehicleType = (short)VLF.CLS.Def.Enums.VehicleType.NotKnown;
            FillTripReportParams(licensePlate, dsResult, userId, false,
                                 ref tblVehicleSensors,
                                 ref tblVehicleGeozones,
                                 ref carCost,
                                 ref measurementUnits,
                                 ref vehicleType);

            report.GetTripActivityReport(licensePlate,
               fromDateTime,
               toDateTime,
               tblVehicleSensors,
               tblVehicleGeozones,
               carCost,
               userId,
               measurementUnits,
               dsResult,
               showLastStoredPosition,
               vehicleType, lang,
               ref requestOverflowed,
               ref totalSqlRecords);
#else
         bool isTrailer = false;
			FillTripReportParams(licensePlate,dsResult,userId,false,
                              ref tblVehicleSensors,
                              ref tblVehicleGeozones,
                              ref carCost,
                              ref measurementUnits,
                              ref isTrailer);
			report.GetTripActivityReport(licensePlate,
				fromDateTime,
				toDateTime,
				tblVehicleSensors,
				tblVehicleGeozones,
				carCost,
				userId,
				measurementUnits,
				dsResult,
				showLastStoredPosition,
				isTrailer,
				ref requestOverflowed,
				ref totalSqlRecords);
#endif
            dsResult.DataSetName = "DsTripActivityReport";
            return dsResult;
        }

        // Changes for TimeZone Feature start

        public DataSet GetTripActivityReport_NewTZ(string licensePlate,
                                              string fromDateTime,
                                              string toDateTime,
                                              int userId,
                                              bool showLastStoredPosition,
                                              int sensorId,
                                              string lang,
                                              ref bool requestOverflowed,
                                              ref int totalSqlRecords)
        {
            DataSet dsResult = new DataSet();
            DataTable tblVehicleSensors = null;
            DataTable tblVehicleGeozones = null;
            double carCost = 1;
            double measurementUnits = 1;
#if NEW_REPORTS
            short vehicleType = (short)VLF.CLS.Def.Enums.VehicleType.NotKnown;
            FillTripReportParams_NewTZ(licensePlate, dsResult, userId, false,
                                 ref tblVehicleSensors,
                                 ref tblVehicleGeozones,
                                 ref carCost,
                                 ref measurementUnits,
                                 ref vehicleType);

            report.GetTripActivityReport_NewTZ(licensePlate,
               fromDateTime,
               toDateTime,
               tblVehicleSensors,
               tblVehicleGeozones,
               carCost,
               userId,
               measurementUnits,
               dsResult,
               showLastStoredPosition,
               vehicleType,
               sensorId,
               lang,
               ref requestOverflowed,
               ref totalSqlRecords);
#else
         bool isTrailer = false;
			FillTripReportParams_NewTZ(licensePlate,dsResult,userId,false,
                              ref tblVehicleSensors,
                              ref tblVehicleGeozones,
                              ref carCost,
                              ref measurementUnits,
                              ref isTrailer);
			report.GetTripActivityReport_NewTZ(licensePlate,
				fromDateTime,
				toDateTime,
				tblVehicleSensors,
				tblVehicleGeozones,
				carCost,
				userId,
				measurementUnits,
				dsResult,
				showLastStoredPosition,
				isTrailer,
				ref requestOverflowed,
				ref totalSqlRecords);
#endif
            dsResult.DataSetName = "DsTripActivityReport";
            return dsResult;
        }
        // Changes for TimeZone Feature end

        public DataSet GetTripActivityReport(string licensePlate,
                                              string fromDateTime,
                                              string toDateTime,
                                              int userId,
                                              bool showLastStoredPosition,
                                              int sensorId,
                                              string lang,
                                              ref bool requestOverflowed,
                                              ref int totalSqlRecords)
        {
            DataSet dsResult = new DataSet();
            DataTable tblVehicleSensors = null;
            DataTable tblVehicleGeozones = null;
            double carCost = 1;
            double measurementUnits = 1;
#if NEW_REPORTS
            short vehicleType = (short)VLF.CLS.Def.Enums.VehicleType.NotKnown;
            FillTripReportParams(licensePlate, dsResult, userId, false,
                                 ref tblVehicleSensors,
                                 ref tblVehicleGeozones,
                                 ref carCost,
                                 ref measurementUnits,
                                 ref vehicleType);

            report.GetTripActivityReport(licensePlate,
               fromDateTime,
               toDateTime,
               tblVehicleSensors,
               tblVehicleGeozones,
               carCost,
               userId,
               measurementUnits,
               dsResult,
               showLastStoredPosition,
               vehicleType,
               sensorId,
               lang,
               ref requestOverflowed,
               ref totalSqlRecords);
#else
         bool isTrailer = false;
			FillTripReportParams(licensePlate,dsResult,userId,false,
                              ref tblVehicleSensors,
                              ref tblVehicleGeozones,
                              ref carCost,
                              ref measurementUnits,
                              ref isTrailer);
			report.GetTripActivityReport(licensePlate,
				fromDateTime,
				toDateTime,
				tblVehicleSensors,
				tblVehicleGeozones,
				carCost,
				userId,
				measurementUnits,
				dsResult,
				showLastStoredPosition,
				isTrailer,
				ref requestOverflowed,
				ref totalSqlRecords);
#endif
            dsResult.DataSetName = "DsTripActivityReport";
            return dsResult;
        }

        /// <summary>
        /// Retrieves alarm information. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>
        /// DataSet [AlarmId],[DateTimeCreated],[AlarmSeverity],[AlarmType],
        /// [DateTimeAck],[DateTimeClosed],[Description],
        /// [BoxId],[OriginDateTime],[ValidGps],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],
        /// [BoxProtocolTypeId],[BoxProtocolTypeName]
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress],
        /// [UserId],[UserName],[DriverLicense],[FirstName],[LastName]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetAlarmReport(int userId, string licensePlate, string fromDateTime, string toDateTime, ref bool requestOverflowed, ref int totalSqlRecords)
        {
            DataSet dsResult = new DataSet();
            DB.VehicleAssignment vehicAssgn = new DB.VehicleAssignment(sqlExec);
            Int64 vehicleId = Convert.ToInt64(vehicAssgn.GetVehicleAssignmentField("VehicleId", "LicensePlate", licensePlate));
            // Retrieve all historical box/licensePlate assignments during report period (from,to)
            DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
            //[AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]
            DataSet dsAssignHst = assignHst.GetAllVehiclesAssignmentsBy("VehicleId", vehicleId, Convert.ToDateTime(fromDateTime), Convert.ToDateTime(toDateTime), " ORDER BY AssignedDateTime DESC");


            if (dsAssignHst != null && dsAssignHst.Tables.Count > 0)
            {
                // 3. For each assignment in the range of from/to datetime merge results
                //		in the history (does not include current assignment)
                DataSet currResult = null;
                DateTime currAssignTo;
                DateTime currAssignFrom;
                foreach (DataRow ittr in dsAssignHst.Tables[0].Rows)
                {
                    currAssignFrom = Convert.ToDateTime(ittr["AssignedDateTime"]);
                    if (currAssignFrom.Ticks == 0)
                        currAssignFrom = VLF.CLS.Def.Const.unassignedDateTime;

                    object obj = ittr["DeletedDateTime"];

                    if (obj != System.DBNull.Value)
                        currAssignTo = Convert.ToDateTime(obj);
                    else
                        //if(currAssignTo.Ticks == 0)
                        currAssignTo = Convert.ToDateTime(toDateTime);

                    if (currAssignFrom.Ticks < Convert.ToDateTime(fromDateTime).Ticks)
                        currAssignFrom = new DateTime(Convert.ToDateTime(fromDateTime).Ticks);

                    currResult = report.GetAlarmReport(userId,
                       Convert.ToInt32(ittr["BoxId"]),
                       currAssignFrom.ToString(), currAssignTo.ToString(),
                       ref requestOverflowed);

                    if (currResult != null && currResult.Tables.Count > 0)
                    {
                        // Add VehicleId column to result dataset
                        currResult.Tables[0].Columns.Add("VehicleId", typeof(Int64));
                        // Add vehicle id to every row
                        foreach (DataRow alrmIttr in currResult.Tables[0].Rows)//for every alarm related to the vehicle
                            alrmIttr["VehicleId"] = vehicleId;
                        // 3. Merge vehicle trips info into fleet trips info
                        if (dsResult != null && currResult != null)
                            dsResult.Merge(currResult);
                    }
                }
                // 3.2	Add current assignment 
                // [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
                DB.VehicleAssignment assign = new DB.VehicleAssignment(sqlExec);
                DataSet dsAssign = assign.GetVehicleAssignmentBy("LicensePlate", licensePlate);
                if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
                {
                    currAssignFrom = Convert.ToDateTime(dsAssign.Tables[0].Rows[0]["AssignedDateTime"]);
                    if (currAssignFrom.Ticks < Convert.ToDateTime(fromDateTime).Ticks)
                        currAssignFrom = new DateTime(Convert.ToDateTime(fromDateTime).Ticks);
                    currResult = report.GetAlarmReport(userId,
                       Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]),
                       currAssignFrom.ToString(), toDateTime,
                       ref requestOverflowed);
                    if (currResult != null && currResult.Tables.Count > 0)
                    {
                        // Add VehicleId column to result dataset
                        currResult.Tables[0].Columns.Add("VehicleId", typeof(Int64));
                        // Add vehicle id to every row
                        foreach (DataRow alrmIttr in currResult.Tables[0].Rows)//for every alarm related to the vehicle
                            alrmIttr["VehicleId"] = vehicleId;
                        // 3. Merge vehicle trips info into fleet trips info
                        if (dsResult != null && currResult != null)
                            dsResult.Merge(currResult);
                    }
                }
            }
            dsResult.DataSetName = "DsAlarmReport";
            DataSet dsFilteredResult = null;
            #region Sort result by DateTimeCreated DESC
            if (dsResult != null)
            {
                dsFilteredResult = dsResult.Clone();
                if (dsResult.Tables.Count > 0)
                {
                    dsFilteredResult.Tables[0].TableName = "VehicleAlarms";
                    DataRow[] filteredRows = dsResult.Tables[0].Select("", "DateTimeCreated DESC");
                    foreach (DataRow ittr in filteredRows)
                        dsFilteredResult.Tables[0].ImportRow(ittr);
                }
            }
            #endregion
            return dsFilteredResult;
        }

        /// <summary>
        /// Prepares vehicle alarms report
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>
        /// DataSet [AlarmId],[DateTimeCreated],[AlarmSeverity],[AlarmType],
        /// [DateTimeAck],[DateTimeClosed],[Description],
        /// [BoxId],[OriginDateTime],[ValidGps],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],
        /// [BoxProtocolTypeId],[BoxProtocolTypeName]
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress],
        /// [UserId],[UserName],[DriverLicense],[FirstName],[LastName]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetVehicleAlarmsReport(int userId, string licensePlate, string fromDateTime, string toDateTime)
        {
            DataSet dsBoxAlarms = new DataSet("BoxAlarmsReport");

            try
            {
                VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);

                dsBoxAlarms = rpt.GetVehicleAlarmReport(userId, licensePlate, fromDateTime, toDateTime);

                //if (Util.IsDataSetValid(dsBoxAlarms))
                //   totalSqlRecords = dsBoxAlarms.Tables[0].Rows.Count;
            }
            catch (SqlException se)
            {
                Util.ProcessDbException("Error getting alarms report for a vehicle ", se);

            }
            catch (DASDbConnectionClosed ex)
            {
                throw new DASDbConnectionClosed(ex.Message);
            }
            catch (Exception ex)
            {
                throw new DASException("Alarms report for the vehicle [" + licensePlate + "] failed. " + ex.Message);
            }

            return dsBoxAlarms;
        }

        /// <summary>
        /// Prepares fleet alarms report
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>
        /// DataSet [AlarmId],[DateTimeCreated],[AlarmSeverity],[AlarmType],
        /// [DateTimeAck],[DateTimeClosed],[Description],
        /// [BoxId],[OriginDateTime],[ValidGps],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],
        /// [BoxProtocolTypeId],[BoxProtocolTypeName]
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress],
        /// [UserId],[UserName],[DriverLicense],[FirstName],[LastName]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetFleetAlarmsReport(int userId, int fleetId, string fromDateTime, string toDateTime, ref bool requestOverflowed, ref int totalSqlRecords)
        {
            DataSet dsFleetAlarms = new DataSet("FleetAlarmsReport");

            try
            {
                VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);

                dsFleetAlarms = rpt.GetFleetAlarmReport(userId, fleetId, fromDateTime, toDateTime, ref requestOverflowed);

                if (Util.IsDataSetValid(dsFleetAlarms))
                    totalSqlRecords = dsFleetAlarms.Tables[0].Rows.Count;
            }
            catch (SqlException se)
            {
                Util.ProcessDbException("Error getting alarms report for fleet ", se);
            }
            catch (DASDbConnectionClosed ex)
            {
                throw new DASDbConnectionClosed(ex.Message);
            }
            catch (Exception ex)
            {
                throw new DASException("Alarms report for fleet [Id=" + fleetId.ToString() + "] failed. " + ex.Message);
            }
            /*
            DataSet dsCurrVehicleAlarms = null;
            // 1. Retrieve fleet vehicles info (license plates)
            DB.FleetVehicles fleet = new DB.FleetVehicles(sqlExec);
            //[LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]
            DataSet dsVehicles = fleet.GetVehiclesInfoByFleetId(fleetId);
            int currSqlRecords = 0;
            if ((dsVehicles != null) && (dsVehicles.Tables.Count > 0))
            {
               foreach (DataRow ittrRow in dsVehicles.Tables[0].Rows)// for every vehicle in the fleet
               {
                  //[AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]
                  // 2. Retrieves trip info for every vehicle
                  currSqlRecords = 0;
                  dsCurrVehicleAlarms = GetAlarmReport(userId, ittrRow["LicensePlate"].ToString().TrimEnd(), fromDateTime, toDateTime, ref requestOverflowed, ref currSqlRecords);
                  totalSqlRecords += currSqlRecords;
                  if (requestOverflowed)
                  {
                     dsFleetAlarms.Clear();
                     break;
                  }
                  // 3. Merge vehicle trips info into fleet trips info
                  if (dsFleetAlarms != null && dsCurrVehicleAlarms != null)
                     dsFleetAlarms.Merge(dsCurrVehicleAlarms);
               }
            }
            if (dsFleetAlarms != null)
            {
               if (dsFleetAlarms.Tables.Count > 0)
               {
                  #region Sort result by DateTimeCreated
                  DataRow[] filterResult = dsFleetAlarms.Tables[0].Select("", "DateTimeCreated DESC");
                  DataTable tblResult = dsFleetAlarms.Tables[0].Clone();
                  foreach (DataRow updateRow in filterResult)
                     tblResult.ImportRow(updateRow);
                  dsFleetAlarms.Tables.Clear();
                  dsFleetAlarms.Tables.Add(tblResult);
                  #endregion
               }
            }
            */

            return dsFleetAlarms;
        }

        /// <summary>
        /// Retrieves system usage exseption report. 
        /// </summary>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns>
        /// DataSet [OrganizationName],[Description],[BoxId],[MaxMsgs],[MaxTxtMsgs],[BoxProtocolTypeName],[OrganizationId],[TotalMsgInSize],[TotalMsgOutSize]
        /// </returns>
        /// <remarks> order by OrganizationName</remarks>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetSystemUsageExceptionReportForAllOrganizations(string fromDateTime, string toDateTime)
        {
            DataSet dsResult = new DataSet();
            dsResult = report.GetSystemUsageExceptionReportForAllOrganizations(fromDateTime, toDateTime);
            if (Util.IsDataSetValid(dsResult))
                dsResult.DataSetName = "DsSystemUsageExceptionReportForAllOrganizations";
            return dsResult;
        }

        /// <summary>
        /// Retrieves system usage report by organization. 
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="showExceptionOnly"></param>
        /// <returns>
        /// DataSet [Description],[BoxId],[MaxMsgs],[MaxTxtMsgs],[BoxProtocolTypeName],[TotalMsgInSize],[TotalMsgOutSize]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetSystemUsageReportByOrganization(int organizationId, string fromDateTime, string toDateTime, bool showExceptionOnly)
        {
            DataSet dsResult = new DataSet();
            //                GetSystemUsageReportByOrganization
            //dsResult = report.GetSystemUsageReportByOrganization(organizationId, fromDateTime, toDateTime, showExceptionOnly);
            //if (dsResult != null)
            //{
            //   dsResult.DataSetName = "DsSystemUsageReport";
            //}
            string prefixMsg = "Unable to retrieve system usage report by organization id=" + organizationId +
               " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[4];
                sqlParams[0] = new SqlParameter("@orgId", organizationId);
                sqlParams[1] = new SqlParameter("@dtFrom", Convert.ToDateTime(fromDateTime));
                sqlParams[2] = new SqlParameter("@dtTo", Convert.ToDateTime(toDateTime));
                sqlParams[3] = new SqlParameter("@excOnly", showExceptionOnly);
                //Executes SQL statement
                dsResult = sqlExec.SPExecuteDataset("OrganizationGetSystemAirUsage", sqlParams);
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
            return dsResult;
        }

        /// <summary>
        /// Retrieves system usage report by box. 
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="showExceptionOnly"></param>
        /// <returns>
        /// DataSet [Description],[BoxId],[MaxMsgs],[MaxTxtMsgs],[BoxProtocolTypeName],[TotalMsgInSize],[TotalMsgOutSize]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetSystemUsageReportByBox(int boxId, string fromDateTime, string toDateTime, bool showExceptionOnly)
        {
            DataSet dsResult = new DataSet();
            //dsResult = report.GetSystemUsageReportByBox(boxId, fromDateTime, toDateTime, showExceptionOnly);
            //if (dsResult != null)
            //{
            //   dsResult.DataSetName = "DsSystemUsageReport";
            //}
            string prefixMsg = "Unable to retrieve system usage report by box id=" + boxId +
              " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[4];
                sqlParams[0] = new SqlParameter("@boxId", boxId);
                sqlParams[1] = new SqlParameter("@dtFrom", Convert.ToDateTime(fromDateTime));
                sqlParams[2] = new SqlParameter("@dtTo", Convert.ToDateTime(toDateTime));
                sqlParams[3] = new SqlParameter("@excOnly", showExceptionOnly);
                //Executes SQL statement
                dsResult = sqlExec.SPExecuteDataset("BoxGetSystemAirUsage_New", sqlParams);
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
            return dsResult;
        }

        /// <summary>
        /// Retrieves exception report
        /// </summary>
        /// <remarks>
        /// 1. If sosLimit=-1 do not include intto report
        /// 2. If noDoorSnsHrs=-1 do not include into report
        /// </remarks>
        /// <returns>[Type],[DateTime],[VehicleDescription],[Remarks1],
        /// [Remarks2],[BoxId"],[VehicleId]</returns>
        /// <param name="licensePlate"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="sosLimit"></param>
        /// <param name="noDoorSnsHrs"></param>
        /// <param name="includeTar"></param>
        /// <param name="includeMobilize"></param>
        /// <param name="fifteenSecDoorSns"></param>
        /// <param name="leash50"></param>
        /// <param name="mainAndBackupBatterySns"></param>
        /// <param name="tamperSns"></param>
        /// <param name="anyPanicSns"></param>
        /// <param name="threeKeypadAttemptsSns"></param>
        /// <param name="altGPSAntennaSns"></param>
        /// <param name="controllerStatus"></param>
        /// <param name="leashBrokenSns"></param>
        /// <param name="driverDoor"></param>
        /// <param name="passengerDoor"></param>
        /// <param name="sideHopperDoor"></param>
        /// <param name="rearHopperDoor"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns></returns>
        public DataSet GetExceptionReport(string licensePlate,
           string fromDateTime,
           string toDateTime,
           int userId,
           short sosLimit,
           int noDoorSnsHrs,
           bool includeTar,
           bool includeMobilize,
           bool fifteenSecDoorSns,
           bool leash50,
           bool mainAndBackupBatterySns,
           bool tamperSns,
           bool anyPanicSns,
           bool threeKeypadAttemptsSns,
           bool altGPSAntennaSns,
           bool controllerStatus,
           bool leashBrokenSns,
           bool driverDoor, bool passengerDoor, bool sideHopperDoor, bool rearHopperDoor,
              bool includeCurrentTar, bool Locker1, bool Locker2, bool Locker3, bool Locker4, bool Locker5, bool Locker6,
            bool Locker7, bool Locker8, bool Locker9,
           ref bool requestOverflowed,
           ref int totalSqlRecords)
        {
            DataSet dsResult = new DataSet();

            string vehicleDescription = "";
            Int64 vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
            DB.VehicleInfo vehicle = new DB.VehicleInfo(sqlExec);
            //[LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],
            //[MakeName],[ModelName],[VehicleTypeName],[StateProvince],
            //[ModelYear],[Color],[Description]
            int boxId = VLF.CLS.Def.Const.unassignedIntValue;
            DataSet dsVehicle = vehicle.GetVehicleInfoByLicensePlate(licensePlate);
            if (dsVehicle != null && dsVehicle.Tables.Count > 0 && dsVehicle.Tables[0].Rows.Count > 0)
            {
                vehicleDescription = dsVehicle.Tables[0].Rows[0]["Description"].ToString().TrimEnd();
                vehicleId = Convert.ToInt64(dsVehicle.Tables[0].Rows[0]["VehicleId"]);
                boxId = Convert.ToInt32(dsVehicle.Tables[0].Rows[0]["BoxId"]);
            }

            // Retrieve user preference
            DB.UserPreference userPref = new DB.UserPreference(sqlExec);
            DataSet dsUserPref = userPref.GetUserPreferenceInfo(userId, (int)VLF.CLS.Def.Enums.Preference.TimeZone);
            short userTimezone = 0;
            if (dsUserPref != null && dsUserPref.Tables.Count > 0 && dsUserPref.Tables[0].Rows.Count > 0)
                userTimezone = Convert.ToInt16(dsUserPref.Tables[0].Rows[0]["PreferenceValue"]);

            dsUserPref = userPref.GetUserPreferenceInfo(userId, (int)VLF.CLS.Def.Enums.Preference.DayLightSaving);
            if (dsUserPref != null && dsUserPref.Tables.Count > 0 && dsUserPref.Tables[0].Rows.Count > 0)
                userTimezone += Convert.ToInt16(dsUserPref.Tables[0].Rows[0]["PreferenceValue"]);
            report.GetExceptionReport(licensePlate, boxId,
            fromDateTime,
            toDateTime,
            userId,
            sosLimit,
            noDoorSnsHrs,
            dsResult,
            vehicleDescription,
            vehicleId,
            includeTar,
            includeMobilize,
            fifteenSecDoorSns,
            leash50,
            mainAndBackupBatterySns,
            tamperSns,
            anyPanicSns,
            threeKeypadAttemptsSns,
            altGPSAntennaSns,
            controllerStatus,
            leashBrokenSns,
            userTimezone, driverDoor, passengerDoor, sideHopperDoor, rearHopperDoor,
                includeCurrentTar, Locker1, Locker2, Locker3, Locker4, Locker5, Locker6,
              Locker7, Locker8, Locker9,
            ref requestOverflowed,
            ref totalSqlRecords);
            if (dsResult != null && dsResult.Tables.Count > 0)
            {
                #region Sort result by DateTime
                DataRow[] filterResult = dsResult.Tables[0].Select("", "DateTime DESC");
                DataTable tblResult = dsResult.Tables[0].Clone();
                foreach (DataRow updateRow in filterResult)
                    tblResult.ImportRow(updateRow);
                dsResult.Tables.Clear();
                dsResult.Tables.Add(tblResult);
                #endregion
            }
            dsResult.DataSetName = "DsExceptionReport";
            return dsResult;
        }

        /// <summary>
        /// Retrieves exception report
        /// </summary>
        /// <remarks>
        /// 1. If sosLimit=-1 do not include intto report
        /// 2. If noDoorSnsHrs=-1 do not include into report
        /// </remarks>
        /// <returns>[Type],[DateTime],[VehicleDescription],[Remarks1],
        /// [Remarks2],[BoxId"],[VehicleId]</returns>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="sosLimit"></param>
        /// <param name="noDoorSnsHrs"></param>
        /// <param name="includeTar"></param>
        /// <param name="includeMobilize"></param>
        /// <param name="fifteenSecDoorSns"></param>
        /// <param name="leash50"></param>
        /// <param name="mainAndBackupBatterySns"></param>
        /// <param name="tamperSns"></param>
        /// <param name="anyPanicSns"></param>
        /// <param name="threeKeypadAttemptsSns"></param>
        /// <param name="altGPSAntennaSns"></param>
        /// <param name="controllerStatus"></param>
        /// <param name="leashBrokenSns"></param>
        /// <param name="driverDoor"></param>
        /// <param name="passengerDoor"></param>
        /// <param name="sideHopperDoor"></param>
        /// <param name="rearHopperDoor"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns></returns>
        public DataSet GetFleetExceptionReport(int fleetId,
           string fromDateTime,
           string toDateTime,
           int userId,
           short sosLimit,
           int noDoorSnsHrs,
           bool includeTar,
           bool includeMobilize,
           bool fifteenSecDoorSns,
           bool leash50,
           bool mainAndBackupBatterySns,
           bool tamperSns,
           bool anyPanicSns,
           bool threeKeypadAttemptsSns,
           bool altGPSAntennaSns,
           bool controllerStatus,
           bool leashBrokenSns,
           bool driverDoor, bool passengerDoor, bool sideHopperDoor, bool rearHopperDoor,
              bool includeCurrentTar,
             bool Locker1, bool Locker2, bool Locker3, bool Locker4, bool Locker5, bool Locker6,
            bool Locker7, bool Locker8, bool Locker9,
           ref bool requestOverflowed,
           ref int totalSqlRecords)
        {
            DataSet dsResult = new DataSet();
            DataSet dsCurrVehicle = null;
            int currSqlRecords = 0;
            // 1. Retrieve fleet vehicles info (license plates)
            DB.FleetVehicles fleet = new DB.FleetVehicles(sqlExec);
            DataSet dsVehicles = fleet.GetVehiclesInfoByFleetId(fleetId);
            if (dsVehicles != null && dsVehicles.Tables.Count > 0)
            {
                string licensePlate = "";
                foreach (DataRow ittrRow in dsVehicles.Tables[0].Rows)
                {
                    licensePlate = ittrRow["LicensePlate"].ToString().TrimEnd();
                    // 2. Retrieves trip info for every vehicle
                    currSqlRecords = 0;
                    dsCurrVehicle = GetExceptionReport(licensePlate,
                       fromDateTime,
                       toDateTime,
                       userId,
                       sosLimit,
                       noDoorSnsHrs,
                       includeTar,
                       includeMobilize,
                       fifteenSecDoorSns,
                       leash50,
                       mainAndBackupBatterySns,
                       tamperSns,
                       anyPanicSns,
                       threeKeypadAttemptsSns,
                       altGPSAntennaSns,
                       controllerStatus,
                       leashBrokenSns,
                       driverDoor, passengerDoor, sideHopperDoor, rearHopperDoor,
                       includeCurrentTar, Locker1, Locker2, Locker3, Locker4, Locker5, Locker6,
                       Locker7, Locker8, Locker9,
                       ref requestOverflowed,
                       ref currSqlRecords);
                    totalSqlRecords += currSqlRecords;
                    if (requestOverflowed)
                    {
                        dsResult.Clear();
                        break;
                    }
                    // 3. Merge vehicle exceptions info into fleet exceptions info
                    if (dsResult != null && dsCurrVehicle != null)
                        dsResult.Merge(dsCurrVehicle);
                }
            }
            dsResult.DataSetName = "DsFleetExceptionsReport";
            return dsResult;
        }

        /// <summary>
        /// Prepares Latency report
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="dsParams"></param>
        /// <returns>
        /// DataSet [OrganizationName],[Description],[BoxId],[CommModeName],[NumOfMsgs],[DiffInSec]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetVehicleLatencyReport(Int64 vehicleId, string fromDateTime, string toDateTime, DataSet dsParams)
        {
            DataSet dsResult = report.GetVehicleLatencyInformation(vehicleId, fromDateTime, toDateTime, dsParams);
            if (dsResult != null)
                dsResult.DataSetName = "DsVehicleLatencyReport";
            return dsResult;
        }

        /// <summary>
        /// Prepares Fleet Latency report
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="dsParams"></param>
        /// <returns>
        /// DataSet [OrganizationName],[Description],[BoxId],[CommModeName],[NumOfMsgs],[DiffInSec]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetFleetLatencyReport(int fleetId, string fromDateTime, string toDateTime, DataSet dsParams)
        {
            DataSet dsResult = report.GetFleetLatencyReport(fleetId, fromDateTime, toDateTime, dsParams);
            if (dsResult != null)
                dsResult.DataSetName = "DsFleetLatencyReport";
            return dsResult;
        }

        /// <summary>
        /// Prepares Organization Latency report
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="dsParams"></param>
        /// <returns>
        /// DataSet [OrganizationName],[Description],[BoxId],[CommModeName],[NumOfMsgs],[DiffInSec]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetOrganizationLatencyReport(int organizationId, string fromDateTime, string toDateTime, DataSet dsParams)
        {
            DataSet dsResult = report.GetOrganizationLatencyReport(organizationId, fromDateTime, toDateTime, dsParams);
            if (dsResult != null)
                dsResult.DataSetName = "DsOrganizationLatencyReport";
            return dsResult;
        }

        # region Obsolete
        /// <summary>
        /// Populates vlfReport table. 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="periodStart"></param>        
        /// <param name="periodEnd"></param>
        /// <param name="deliveryDeadLine"></param>
        /// <param name="xmlParams"></param>
        /// <param name="emails"></param>
        /// <param name="url"></param>
        /// <param name="reportType"></param>
        /// <param name="statusDate"></param>
        /// <returns>
        /// int 
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        [Obsolete("Use ReportScheduler class methods")]
        public int InsertSecheduledReport(int userID, DateTime periodStart, DateTime periodEnd,
                                          DateTime deliveryDeadLine, string xmlParams, string emails,
                                          string url, int reportType, DateTime statusDate, string deliveryPeriod, bool fleet)
        {
            int dsResult = report.InsertSecheduledReport(userID, periodStart, periodEnd,
                                          deliveryDeadLine, xmlParams, emails,
                                          url, reportType, statusDate, deliveryPeriod, fleet);
            return dsResult;
        }

        /// <summary>
        /// Gets All scheduled reports for. 
        /// </summary>
        /// <param name="userID"></param>        
        /// <returns>
        /// DataSet 
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        [Obsolete("Use ReportScheduler class methods")]
        public DataSet GetScheuledReportsByUser(int userID)
        {
            DataSet dsResult = report.GetScheduledReportsByUser(userID);
            if (dsResult != null)
                dsResult.DataSetName = "DsScheduledForUser";
            return dsResult;
        }

        /// <summary>
        /// Gets All scheduled reports for. 
        /// </summary>
        /// <param name="userID"></param>        
        /// <returns>
        /// DataSet 
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        [Obsolete("Use ReportScheduler class methods")]
        public DataSet GetScheuledReportsByStatus(int status)
        {
            DataSet dsResult = report.GetScheduledReportsByStatus(status);
            if (dsResult != null)
                dsResult.DataSetName = "DsScheduledForStus";
            return dsResult;
        }

        /// <summary>
        /// Updates report record . 
        /// </summary>
        /// <param name="reportID"></param>  
        /// <param name="newDelivery"></param> 
        /// <param name="newFrom"></param> 
        /// <param name="newTo"></param> 
        /// <param name="url"></param>
        /// <param name="status"></param>
        /// <returns>
        /// int 
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        [Obsolete("Use ReportScheduler class methods")]
        public int UpdateScheuledReportDatesAndURL(int reportID, DateTime newDelivery, DateTime newFrom, DateTime newTo, string url, int status)
        {
            int result = report.UpdateScheduledReportDatesAndURL(reportID, newDelivery, newFrom, newTo, url, status);
            return result;
        }

        /// <summary>
        /// Deletes a scheduled report. 
        /// </summary>
        /// <param name="reportID"></param>
        /// <returns>
        /// int 
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        [Obsolete("Use ReportScheduler class methods")]
        public int DeleteSecheduledReport(int reportID)
        {
            int dsResult = report.DeleteByReportID(reportID);
            return dsResult;
        }

        # endregion

        /// <summary>
        /// gets gui report name reports. 
        /// </summary>
        /// <param name="guiID"></param>        
        /// <returns>
        /// string 
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public string GetReportsName(int guiID)
        {
            return report.GetReportName(guiID);
            //string dsResult = (report.GetReportsName(guiID)).Tables[0].Rows[0][0].ToString();
            //return dsResult;
        }


        /// <summary>
        /// Get Vehicle Geozone Details Report
        /// </summary>
        /// <param name="userId">User Id for timezone</param>
        /// <param name="licensePlate">Vehicle License Plate</param>
        /// <param name="geozoneNo">Geozone No</param>
        /// <param name="dtFrom">Start date for search</param>
        /// <param name="dtTo">End date for search</param>
        /// <returns>DataSet -> Tables: GeozoneDetails [VehicleId, Description, GeozoneNo, GeozoneName, DateIn, DateOut, Duration];</returns>
        public DataSet GetVehicleGeozoneDetailsReport(int userId, string licensePlate, long geozoneNo, DateTime dtFrom, DateTime dtTo)
        {
            const int DEF_YEAR = 1900;
            DataSet dsResult = new DataSet();
            DataTable dtGzDetails = new DataTable("GeozoneDetails");
            dtGzDetails.Columns.Add("VehicleId", typeof(long));
            dtGzDetails.Columns.Add("Description", typeof(string));
            dtGzDetails.Columns.Add("LicensePlate", typeof(string));
            dtGzDetails.Columns.Add("GeozoneNo", typeof(long));
            dtGzDetails.Columns.Add("GeozoneName", typeof(string));
            //dtGzDetails.Columns.Add("DateIn", typeof(DateTime));
            //dtGzDetails.Columns.Add("DateOut", typeof(DateTime));
            dtGzDetails.Columns.Add("DateIn", typeof(string));
            dtGzDetails.Columns.Add("DateOut", typeof(string));
            dtGzDetails.Columns.Add("Duration", typeof(long));
            dtGzDetails.Columns.Add("DurationFmt", typeof(string));
            dtGzDetails.Columns.Add("SpeedIn", typeof(string));
            dtGzDetails.Columns.Add("SpeedOut", typeof(string));
            dtGzDetails.Columns.Add("OdometerIn", typeof(long));
            dtGzDetails.Columns.Add("OdometerOut", typeof(long));

            // get history messages
            VehicleGeozone vehicleGeozone = new VehicleGeozone(this.sqlExec);
            DataTable dtGzHst = vehicleGeozone.GetVehicleGeozoneMessages(userId, licensePlate, geozoneNo, dtFrom, dtTo);
            DateTime from = new DateTime(DEF_YEAR, 1, 1), to = new DateTime(DEF_YEAR, 1, 1);
            object gzNo = null, vid = null;
            string SpeedIn = ""; string SpeedOut = ""; int OdometerIn=0; int OdometerOut=0;
            // calculate gz durations
            foreach (DataRow drow in dtGzHst.Rows)
            {

                from = new DateTime(DEF_YEAR, 1, 1);
                to = new DateTime(DEF_YEAR, 1, 1);
                if (drow["CustomProp"].ToString().Contains("GZ_DIR=2"))
                {
                   // from = Convert.ToDateTime(drow["OriginDateTime"]);

                    from = Convert.ToDateTime(drow["OriginDateTime_NoCulture"]);  
                    SpeedIn = drow["Speed"].ToString();
                    OdometerIn = Convert.ToInt32(drow["Odometer"]);
                    OdometerOut = 0;
                }
                else if (drow["CustomProp"].ToString().Contains("GZ_DIR=1"))
                {
                    //to = Convert.ToDateTime(drow["OriginDateTime"]);
                    to = Convert.ToDateTime(drow["OriginDateTime_NoCulture"]);  
                    SpeedOut = drow["Speed"].ToString();
                    OdometerIn = 0;
                    OdometerOut = Convert.ToInt32(drow["Odometer"]);
                }
                // calculate total time
                gzNo = drow["GeozoneNo"];
                vid = drow["VehicleId"];
                dtGzDetails.Rows.Add(drow["VehicleId"], drow["Description"], licensePlate, drow["GeozoneNo"], drow["GeozoneName"],
                   from, to, 0, "", SpeedIn, SpeedOut, OdometerIn, OdometerOut);
            }
            dsResult.Tables.Add(dtGzDetails);

            // gz info additional info?
            return dsResult;
        }


        /// <summary>
        /// Get Vehicle Geozone Report
        /// </summary>
        /// <param name="userId">User Id for timezone</param>
        /// <param name="licensePlate">Vehicle License Plate</param>
        /// <param name="geozoneNo">Geozone No</param>
        /// <param name="dtFrom">Start date for search</param>
        /// <param name="dtTo">End date for search</param>
        /// <returns>DataSet -> Tables: GeozoneDetails [VehicleId, Description, GeozoneNo, GeozoneName, DateIn, DateOut, Duration]; GeozoneTotal [GeozoneNo, TotalTime]</returns>
        public DataSet GetVehicleGeozoneReport(int userId, string licensePlate, long geozoneNo, DateTime dtFrom, DateTime dtTo)
        {
            const int DEF_YEAR = 1900;
            DataSet dsResult = new DataSet();
            DataTable dtGzDetails = new DataTable("GeozoneDetails"), dtGzTotal = new DataTable("GeozoneTotal");
            dtGzDetails.Columns.Add("VehicleId", typeof(long));
            dtGzDetails.Columns.Add("Description", typeof(string));
            dtGzDetails.Columns.Add("LicensePlate", typeof(string));
            dtGzDetails.Columns.Add("GeozoneNo", typeof(long));
            dtGzDetails.Columns.Add("GeozoneName", typeof(string));
            //dtGzDetails.Columns.Add("DateIn", typeof(DateTime));
            //dtGzDetails.Columns.Add("DateOut", typeof(DateTime));
            dtGzDetails.Columns.Add("DateIn", typeof(string));
            dtGzDetails.Columns.Add("DateOut", typeof(string));

            dtGzDetails.Columns.Add("Duration", typeof(long));
            dtGzDetails.Columns.Add("DurationFmt", typeof(string));
            dtGzDetails.Columns.Add("SpeedIn", typeof(string));
            dtGzDetails.Columns.Add("SpeedOut", typeof(string));

            dtGzTotal.Columns.Add("GeozoneNo", typeof(long));
            dtGzTotal.Columns.Add("VehicleId", typeof(long));
            dtGzTotal.Columns.Add("TotalTime", typeof(string));

            // get history messages
            VehicleGeozone vehicleGeozone = new VehicleGeozone(this.sqlExec);
            DataTable dtGzHst = vehicleGeozone.GetVehicleGeozoneMessages(userId, licensePlate, geozoneNo, dtFrom, dtTo);
            TimeSpan tsDuration = new TimeSpan(), tsTotal = new TimeSpan();
            DateTime from = new DateTime(DEF_YEAR, 1, 1), to = new DateTime(DEF_YEAR, 1, 1);
            string SpeedIn = ""; string SpeedOut = "";
            object gzNo = null, vid = null;
            // calculate gz durations
            foreach (DataRow drow in dtGzHst.Rows)
            {
                // store from dt
                if (drow["CustomProp"].ToString().Contains("GZ_DIR=2"))
                {
                   // from = Convert.ToDateTime(drow["OriginDateTime"]);
                    from = Convert.ToDateTime(drow["OriginDateTime_NoCulture"]);
                    SpeedIn = drow["Speed"].ToString();
                }
                // calculate duration
                if (drow["CustomProp"].ToString().Contains("GZ_DIR=1"))
                {
                    //to = Convert.ToDateTime(drow["OriginDateTime"]);
                    to = Convert.ToDateTime(drow["OriginDateTime_NoCulture"]);
                    if (from.Year == DEF_YEAR || to.Year == DEF_YEAR)
                        continue;
                    tsDuration = to.Subtract(from);
                    // calculate total time

                    if (tsDuration.Ticks  > 0)
                    {
                        tsTotal += tsDuration;
                        gzNo = drow["GeozoneNo"];
                        vid = drow["VehicleId"];
                        SpeedOut = drow["Speed"].ToString();
                        dtGzDetails.Rows.Add(drow["VehicleId"], drow["Description"], licensePlate, drow["GeozoneNo"], drow["GeozoneName"],
                           from, to, tsDuration.TotalSeconds, FormatTimeString((long)tsDuration.TotalSeconds), SpeedIn, SpeedOut);
                    }
                }
            }
            dtGzTotal.Rows.Add(gzNo, vid, FormatTimeString((long)tsTotal.TotalSeconds));
            dsResult.Tables.Add(dtGzDetails);
            dsResult.Tables.Add(dtGzTotal);

            // gz info additional info?
            return dsResult;
        }





        /// <summary>
        /// Vehicle Geozone Time Sheet Report
        /// </summary>
        /// <param name="userId">User Id for timezone</param>
        /// <param name="fleetId">Fleet Id</param>
        /// <param name="geozoneNo">Geozone No</param>
        /// <param name="dtFrom">Start date for search</param>
        /// <param name="dtTo">End date for search</param>

        public DataSet GetVehicleGeozoneTimeSheetReport(int userId, int fleetId, DateTime dtFrom, DateTime dtTo)
        {
            VehicleGeozone vehicleGeozone = new VehicleGeozone(this.sqlExec);
            DataSet dsResult = vehicleGeozone.GetVehicleGeozoneTimeSheet(userId, fleetId, dtFrom, dtTo);
            return dsResult;
        }



        /// <summary>
        /// Vehicle Geozone Time Sheet Report
        /// </summary>
        /// <param name="userId">User Id for timezone</param>
        /// <param name="fleetId">Fleet Id</param>
        /// <param name="geozoneNo">Geozone No</param>
        /// <param name="dtFrom">Start date for search</param>
        /// <param name="dtTo">End date for search</param>

        public DataSet GetVehicleGeozoneTimeSheetReport(int userId, int fleetId, DateTime dtFrom, DateTime dtTo, Int16 activeVehicles)
        {
            VehicleGeozone vehicleGeozone = new VehicleGeozone(this.sqlExec);
            DataSet dsResult = vehicleGeozone.GetVehicleGeozoneTimeSheet(userId, fleetId, dtFrom, dtTo, activeVehicles);
            return dsResult;
        }





        public DataSet GetVehicleGeozoneTimeSheetReport201107(int userId, int fleetId, DateTime dtFrom, DateTime dtTo)
        {
            VehicleGeozone vehicleGeozone = new VehicleGeozone(this.sqlExec);
            DataSet dsResult = vehicleGeozone.GetVehicleGeozoneTimeSheet201107(userId, fleetId, dtFrom, dtTo);
            return dsResult;
        }


        public DataSet DashBoard_HeartBeat(int userId)
        {
            return report.DashBoard_HeartBeat(userId);
        }



        public DataSet DashBoard_AHA(int userId, int Top, int Hours)
        {
            return report.DashBoard_AHA(userId, Top, Hours);
        }

        // Changes for TimeZone Feature start

        public DataSet GetVehiclesStatusByDateReport_NewTZ(int userId, int fleetId)
        {
            return report.GetVehiclesStatusReport_NewTZ(userId, fleetId);

        }

        // Changes for TimeZone Feature end


        public DataSet GetVehiclesStatusByDateReport(int userId, int fleetId)
        {
            return report.GetVehiclesStatusReport(userId, fleetId);

        }

        // Changes for TimeZone Feature start

        public DataSet GetVehiclesStatusByDateReport_NewTZ(int userId, int fleetId, Int16 activeVehicles)
        {
            return report.GetVehiclesStatusReport_NewTZ(userId, fleetId, activeVehicles);

        }

        // Changes for TimeZone Feature end

        public DataSet GetVehiclesStatusByDateReport(int userId, int fleetId, Int16 activeVehicles)
        {
            return report.GetVehiclesStatusReport(userId, fleetId, activeVehicles);

        }


        public DataSet GetBoxStartEndOdometerEngHrs(DateTime fromDate, DateTime toDate, int fleetId)
        {
            return report.GetBoxStartEndOdometerEngHrs(fromDate, toDate, fleetId);

        }

        public DataSet OrganizationHistoryDateRangeValidation(int userId, DateTime fromDate, DateTime toDate)
        {
            return report.OrganizationHistoryDateRangeValidation(userId, fromDate, toDate);

        }

        /// <summary>
        /// Vehicle Work Site Activity Per Day Report
        /// </summary>
        /// <param name="userId">User Id for timezone</param>
        /// <param name="fleetId">Fleet Id</param>
        /// <param name="geozoneNo">Geozone No</param>
        /// <param name="dtFrom">Start date for search</param>
        /// <param name="dtTo">End date for search</param>

        public DataSet GetWorksiteActivityPerDay(int userId, int fleetId, DateTime dtFrom, DateTime dtTo)
        {
            VehicleGeozone vehicleGeozone = new VehicleGeozone(this.sqlExec);
            DataSet dsResult = vehicleGeozone.GetWorksiteActivityPerDay(userId, fleetId, dtFrom, dtTo);
            return dsResult;
        }


        /// <summary>
        /// Get Fleet Geozone Details Report
        /// </summary>
        /// <param name="userId">User Id for timezone</param>
        /// <param name="fleetId">Fleet Id</param>
        /// <param name="geozoneNo">Geozone No</param>
        /// <param name="dtFrom">Start date for search</param>
        /// <param name="dtTo">End date for search</param>
        /// <returns>DataSet -> Tables: GeozoneDetails [VehicleId, Description, GeozoneNo, GeozoneName, DateIn, DateOut, Duration]; GeozoneTotal [GeozoneNo, TotalTime]</returns>
        public DataSet GetFleetGeozoneDetailsReport(int userId, int fleetId, long geozoneNo, DateTime dtFrom, DateTime dtTo)
        {
            DataSet dsResult = new DataSet();
            DataTable dtGzDetails = new DataTable("GeozoneDetails");
            dtGzDetails.Columns.Add("VehicleId", typeof(long));
            dtGzDetails.Columns.Add("Description", typeof(string));
            dtGzDetails.Columns.Add("LicensePlate", typeof(string));
            dtGzDetails.Columns.Add("GeozoneNo", typeof(long));
            dtGzDetails.Columns.Add("GeozoneName", typeof(string));
            //dtGzDetails.Columns.Add("DateIn", typeof(DateTime));
            //dtGzDetails.Columns.Add("DateOut", typeof(DateTime));

            dtGzDetails.Columns.Add("DateIn", typeof(string ));
            dtGzDetails.Columns.Add("DateOut", typeof(string));

            dtGzDetails.Columns.Add("Duration", typeof(long));
            dtGzDetails.Columns.Add("SpeedIn", typeof(string));
            dtGzDetails.Columns.Add("SpeedOut", typeof(string));
            dtGzDetails.Columns.Add("OdometerIn", typeof(long));
            dtGzDetails.Columns.Add("OdometerOut", typeof(long));

            // get fleet vehicles
            DAS.DB.FleetVehicles fv = new VLF.DAS.DB.FleetVehicles(this.sqlExec);
            DataSet dsVehicles = fv.GetVehiclesInfoByFleetId(fleetId);

            // calculate gz durations
            foreach (DataRow drVehicle in dsVehicles.Tables[0].Rows)
            {
                DataSet dsGzn =
                   GetVehicleGeozoneDetailsReport(userId, drVehicle["LicensePlate"].ToString().Trim(), geozoneNo, dtFrom, dtTo);

                if (Util.IsDataSetValid(dsGzn))
                {
                    dtGzDetails.Merge(dsGzn.Tables[0]);
                }
            }

            dsResult.Tables.Add(dtGzDetails);


            // gz info additional info?
            return dsResult;
        }


        /// <summary>
        /// Get Fleet Geozone Report
        /// </summary>
        /// <param name="userId">User Id for timezone</param>
        /// <param name="fleetId">Fleet Id</param>
        /// <param name="geozoneNo">Geozone No</param>
        /// <param name="dtFrom">Start date for search</param>
        /// <param name="dtTo">End date for search</param>
        /// <returns>DataSet -> Tables: GeozoneDetails [VehicleId, Description, GeozoneNo, GeozoneName, DateIn, DateOut, Duration]; GeozoneTotal [GeozoneNo, TotalTime]</returns>
        public DataSet GetFleetGeozoneReport(int userId, int fleetId, long geozoneNo, DateTime dtFrom, DateTime dtTo)
        {
            DataSet dsResult = new DataSet();
            DataTable dtGzDetails = new DataTable("GeozoneDetails"), dtGzTotal = new DataTable("GeozoneTotal");
            dtGzDetails.Columns.Add("VehicleId", typeof(long));
            dtGzDetails.Columns.Add("Description", typeof(string));
            dtGzDetails.Columns.Add("LicensePlate", typeof(string));
            dtGzDetails.Columns.Add("GeozoneNo", typeof(long));
            dtGzDetails.Columns.Add("GeozoneName", typeof(string));
            //dtGzDetails.Columns.Add("DateIn", typeof(DateTime));
            //dtGzDetails.Columns.Add("DateOut", typeof(DateTime));
            dtGzDetails.Columns.Add("DateIn", typeof(string));
            dtGzDetails.Columns.Add("DateOut", typeof(string));

            dtGzDetails.Columns.Add("Duration", typeof(long));

            dtGzTotal.Columns.Add("GeozoneNo", typeof(long));
            dtGzTotal.Columns.Add("VehicleId", typeof(long));
            dtGzTotal.Columns.Add("TotalTime", typeof(string));

            // get fleet vehicles
            DAS.DB.FleetVehicles fv = new VLF.DAS.DB.FleetVehicles(this.sqlExec);
            DataSet dsVehicles = fv.GetVehiclesInfoByFleetId(fleetId);

            // calculate gz durations
            foreach (DataRow drVehicle in dsVehicles.Tables[0].Rows)
            {
                DataSet dsGzn =
                   GetVehicleGeozoneReport(userId, drVehicle["LicensePlate"].ToString().Trim(), geozoneNo, dtFrom, dtTo);

                if (Util.IsDataSetValid(dsGzn))
                {
                    dtGzDetails.Merge(dsGzn.Tables[0]);
                    dtGzTotal.Merge(dsGzn.Tables[1]);
                }
            }

            dsResult.Tables.Add(dtGzDetails);
            dsResult.Tables.Add(dtGzTotal);

            // gz info additional info?
            return dsResult;
        }

        ///// <summary>
        ///// Get Landmark Report for a vehicle
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="vehicleId"></param>
        ///// <param name="landmarkName"></param>
        ///// <param name="dtFrom"></param>
        ///// <param name="dtTo"></param>
        ///// <returns>[Description][StopDuration - sec][IdlingDuration - sec]</returns>
        //public DataSet GetLandmarkVehicleSummaryReport(int userId, string licensePlate, string landmarkName, DateTime dtFrom, DateTime dtTo)
        //{
        //   DataSet dsResult = new DataSet();
        //   DataTable dtLandmarkDetails = new DataTable("LandmarkDetails");
        //   dtLandmarkDetails.Columns.Add("Description", typeof(string));
        //   dtLandmarkDetails.Columns.Add("StopDuration", typeof(string));
        //   dtLandmarkDetails.Columns.Add("IdlingDuration", typeof(string));

        //   // get history messages
        //   Landmark landmark = new Landmark(this.sqlExec);
        //   DataTable dtHst = landmark.GetLandmarkVehicleMessages(userId, licensePlate, landmarkName, dtFrom, dtTo);
        //   if (dtHst.Rows.Count == 0)
        //      return null;
        //   TimeSpan tsStopDuration = new TimeSpan();
        //   long idlingDuration = 0;
        //   string customProp = "", vdescr = "";
        //   DateTime from = new DateTime(), to = new DateTime();
        //   short sensorStatusOn = -1; // -1 - 1st time, 0 - off, 1 - on
        //   foreach (DataRow dRow in dtHst.Rows)
        //   {
        //      // ignore messages without sensor data - 1, 9, 24
        //      if (dRow["CustomProp"] == null || String.IsNullOrEmpty(dRow["CustomProp"].ToString().Trim()))
        //         continue;
        //      customProp = dRow["CustomProp"].ToString().Trim();
        //      vdescr = dRow["Description"].ToString().Trim();

        //      // calc idling
        //      if (customProp.Contains(Const.keyIdleDuration))
        //      {
        //         idlingDuration += Convert.ToInt64(Util.PairFindValue(Const.keyIdleDuration, customProp));
        //         continue;
        //      }
        //      else
        //         if (customProp.Contains(Const.keySensorNum + "=3") && customProp.Contains(Const.keySensorStatus))
        //         {
        //            // store off time
        //            if (Util.PairFindValue(Const.keySensorStatus, customProp).ToLower() == "off")
        //            {
        //               from = Convert.ToDateTime(dRow["OriginDateTime"]);
        //               sensorStatusOn = 0;
        //            }
        //            else
        //               // calc stop duration
        //               if (Util.PairFindValue(Const.keySensorStatus, customProp).ToLower() == "on")
        //               {
        //                  if (sensorStatusOn == -1) // if on is the 1st msg - calc from the beginning of the period
        //                  {
        //                     to = Convert.ToDateTime(dRow["OriginDateTime"]);
        //                     tsStopDuration += to.Subtract(dtFrom);
        //                  }
        //                  if (sensorStatusOn == 0) // calc from the previous off only
        //                  {
        //                     to = Convert.ToDateTime(dRow["OriginDateTime"]);
        //                     tsStopDuration += to.Subtract(from);
        //                  }
        //                  sensorStatusOn = 1;
        //               }
        //         }
        //   }
        //   // if off is the last msg - calc until the end of the period
        //   if (sensorStatusOn == 0)
        //   {
        //      tsStopDuration += dtTo.Subtract(from);
        //   }
        //   dtLandmarkDetails.Rows.Add(vdescr, FormatTimeString((long)tsStopDuration.TotalSeconds), FormatTimeString(idlingDuration));
        //   dsResult.Tables.Add(dtLandmarkDetails);

        //   return dsResult;
        //}

        ///// <summary>
        ///// Get Landmark Report for a fleet
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="fleetId"></param>
        ///// <param name="landmarkName"></param>
        ///// <param name="dtFrom"></param>
        ///// <param name="dtTo"></param>
        ///// <returns>FleetLandmarkReport->LandmarkDetails->[Description][StopDuration - sec][IdlingDuration - sec]</returns>
        //public DataSet GetLandmarkFleetSummaryReport(int userId, int fleetId, string landmarkName, DateTime dtFrom, DateTime dtTo)
        //{
        //   DataSet dsResult = new DataSet("FleetLandmarkReport");
        //   DataTable dtLandmarkDetails = new DataTable("LandmarkDetails");
        //   dtLandmarkDetails.Columns.Add("Description", typeof(string));
        //   dtLandmarkDetails.Columns.Add("StopDuration", typeof(string));
        //   dtLandmarkDetails.Columns.Add("IdlingDuration", typeof(string));

        //   // get fleet vehicles
        //   DAS.DB.FleetVehicles fv = new VLF.DAS.DB.FleetVehicles(this.sqlExec);
        //   DataSet dsVehicles = fv.GetVehiclesInfoByFleetId(fleetId);

        //   foreach (DataRow drVehicle in dsVehicles.Tables[0].Rows)
        //   {
        //      DataSet dsLmk =
        //         GetLandmarkVehicleSummaryReport(userId, drVehicle["LicensePlate"].ToString(), landmarkName, dtFrom, dtTo);

        //      if (Util.IsDataSetValid(dsLmk))
        //         dtLandmarkDetails.ImportRow(dsLmk.Tables[0].Rows[0]);
        //   }

        //   dsResult.Tables.Add(dtLandmarkDetails);
        //   return dsResult;
        //}

        /// <summary>
        /// Get Landmark Details Report for all vehicles
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="landmarkName"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns>[Description][StopDuration - sec][IdlingDuration - sec]</returns>
        public DataSet GetLandmarkDetailsReport(int userId, string landmarkName, DateTime dtFrom, DateTime dtTo)
        {
            DataSet dsResult = new DataSet();
            DataTable dtLandmarkDetails = new DataTable("LandmarkDetails");
            dtLandmarkDetails.Columns.Add("Description", typeof(string));
            dtLandmarkDetails.Columns.Add("Type", typeof(string));
            //dtLandmarkDetails.Columns.Add("DateIn", typeof(DateTime));
            //dtLandmarkDetails.Columns.Add("DateOut", typeof(DateTime));
            dtLandmarkDetails.Columns.Add("DateIn", typeof(string));
            dtLandmarkDetails.Columns.Add("DateOut", typeof(string));
            dtLandmarkDetails.Columns.Add("Duration", typeof(string));
            //dtLandmarkDetails.Columns.Add("IdlingDuration", typeof(string));

            // get history messages
            Landmark landmark = new Landmark(this.sqlExec);
            DataTable dtHst = landmark.GetLandmarkMessages(userId, landmarkName, dtFrom, dtTo);
            if (dtHst.Rows.Count == 0)
                return null;
            TimeSpan tsStopDuration = new TimeSpan();
            long idlingDuration = 0;
            string customProp = "", vdescr = "";
            DateTime from = new DateTime(), to = new DateTime();
            short sensorStatusOn = -1; // -1 - 1st time, 0 - off, 1 - on
            foreach (DataRow dRow in dtHst.Rows)
            {
                // ignore messages without sensor data - 1, 9, 24
                if (dRow["CustomProp"] == null || String.IsNullOrEmpty(dRow["CustomProp"].ToString().Trim()))
                    continue;
                customProp = dRow["CustomProp"].ToString().Trim();
                vdescr = dRow["Description"].ToString().Trim();

                // calc idling
                if (customProp.Contains(Const.keyIdleDuration))
                {
                    idlingDuration = Convert.ToInt64(Util.PairFindValue(Const.keyIdleDuration, customProp));
                    dtLandmarkDetails.Rows.Add(vdescr, "Idling", dRow["OriginDateTime"], "", idlingDuration);
                    //continue;
                }
                else
                    if (customProp.Contains(Const.keySensorNum + "=3") && customProp.Contains(Const.keySensorStatus))
                    {
                        // store off time
                        if (Util.PairFindValue(Const.keySensorStatus, customProp).ToLower() == "off")
                        {
                            from = Convert.ToDateTime(dRow["OriginDateTime"]);
                            sensorStatusOn = 0;
                        }
                        else
                            // calc stop duration
                            if (Util.PairFindValue(Const.keySensorStatus, customProp).ToLower() == "on")
                            {
                                if (sensorStatusOn == -1) // if 'on' is the 1st msg - calc from the beginning of the period
                                {
                                    to = Convert.ToDateTime(dRow["OriginDateTime"]);
                                    from = dtFrom;
                                }
                                if (sensorStatusOn == 0) // calc from the previous 'off' only
                                {
                                    to = Convert.ToDateTime(dRow["OriginDateTime"]);
                                }
                                tsStopDuration = to.Subtract(from);
                                sensorStatusOn = 1;
                                dtLandmarkDetails.Rows.Add(vdescr, "Stop", from, to, tsStopDuration.TotalSeconds);
                            }
                    }
            }
            // if off is the last msg - calc until the end of the period
            if (sensorStatusOn == 0)
            {
                tsStopDuration = dtTo.Subtract(from);
                dtLandmarkDetails.Rows.Add(vdescr, "Stop", from, dtTo, tsStopDuration.TotalSeconds);
            }
            //dtLandmarkDetails.Rows.Add(vdescr, FormatTimeString((long)tsStopDuration.TotalSeconds), FormatTimeString(idlingDuration));
            dsResult.Tables.Add(dtLandmarkDetails);

            return dsResult;
        }

        /// <summary>
        /// Get Landmark Vehicle Details Report for all vehicles
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="landmarkName"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns>[Description][StopDuration - sec][IdlingDuration - sec]</returns>
        public DataSet GetLandmarkVehicleDetailsReport(int userId, string licensePlate, string landmarkName, DateTime dtFrom, DateTime dtTo)
        {
            Landmark landmark = new Landmark(this.sqlExec);
            return landmark.GetVehicleAtLandmarkStopIdlingAcitivity(userId, licensePlate, landmarkName, dtFrom, dtTo);

            /*
           DataSet dsResult = new DataSet();
           // duration details table
           DataTable dtLandmarkDetails = new DataTable("LandmarkDetails");
           dtLandmarkDetails.Columns.Add("Description", typeof(string));
           dtLandmarkDetails.Columns.Add("LicensePlate", typeof(string));
           dtLandmarkDetails.Columns.Add("Type", typeof(string));
           dtLandmarkDetails.Columns.Add("DateIn", typeof(string));
           dtLandmarkDetails.Columns.Add("DateOut", typeof(string));
           dtLandmarkDetails.Columns.Add("Duration", typeof(string));
           //dtLandmarkDetails.Columns.Add("IdlingDuration", typeof(string));

           // total duration table
           DataTable dtLandmarkTotal = new DataTable("TotalDuration");
           dtLandmarkTotal.Columns.Add("Description", typeof(string));
           dtLandmarkTotal.Columns.Add("TotalIdling", typeof(string));
           dtLandmarkTotal.Columns.Add("TotalStop", typeof(string));

           // get history messages
           Landmark landmark = new Landmark(this.sqlExec);
           DataTable dtHst = landmark.GetLandmarkVehicleMessages(userId, licensePlate, landmarkName, dtFrom, dtTo);
           if (dtHst.Rows.Count == 0)
              return null;
           TimeSpan tsStopDuration = new TimeSpan(), tsTotalStop = new TimeSpan();
           long idlingDuration = 0, totalIdling = 0;
           string customProp = "", vdescr = "";
           DateTime from = new DateTime(), to = new DateTime();
           short sensorStatusOn = -1; // -1 - 1st time, 0 - off, 1 - on
           foreach (DataRow dRow in dtHst.Rows)
           {
              // ignore messages without sensor data - 1, 9, 24
              if (dRow["CustomProp"] == null || String.IsNullOrEmpty(dRow["CustomProp"].ToString().Trim()))
                 continue;
              customProp = dRow["CustomProp"].ToString().Trim();
              vdescr = dRow["Description"].ToString().Trim();

              // calc idling
              if (customProp.Contains(Const.keyIdleDuration))
              {
                 idlingDuration = Convert.ToInt64(Util.PairFindValue(Const.keyIdleDuration, customProp));
                 if (idlingDuration == 0)
                    from = Convert.ToDateTime(dRow["OriginDateTime"]);
                 else
                 {
                    dtLandmarkDetails.Rows.Add(vdescr, licensePlate, "Idling", from, dRow["OriginDateTime"], FormatTimeString(idlingDuration));
                    totalIdling += idlingDuration;
                 }
              }
              else
                 if (customProp.Contains(Const.keySensorNum + "=3") && customProp.Contains(Const.keySensorStatus))
                 {
                    // store off time
                    if (Util.PairFindValue(Const.keySensorStatus, customProp).Equals("off", StringComparison.InvariantCultureIgnoreCase))
                    {
                       from = Convert.ToDateTime(dRow["OriginDateTime"]);
                       sensorStatusOn = 0;
                    }
                    else
                       // calc stop duration
                       if (Util.PairFindValue(Const.keySensorStatus, customProp).Equals("on", StringComparison.InvariantCultureIgnoreCase))
                       {
                          if (sensorStatusOn == -1) // if 'on' is the 1st msg - calc from the midnight
                          {
                             to = Convert.ToDateTime(dRow["OriginDateTime"]);
                             from = to.Subtract(new TimeSpan(to.Hour, to.Minute, to.Second));
                          }
                          if (sensorStatusOn == 0) // calc from the previous 'off' only
                          {
                             to = Convert.ToDateTime(dRow["OriginDateTime"]);
                          }
                          tsStopDuration = to.Subtract(from);
                          sensorStatusOn = 1;
                          dtLandmarkDetails.Rows.Add(vdescr, licensePlate, "Stop", from, to, FormatTimeString((long)tsStopDuration.TotalSeconds));
                          tsTotalStop += tsStopDuration;
                       }
                 }
           }
           // if off is the last msg - calc until the end of the period
           if (sensorStatusOn == 0)
           {
              //int tz = 0;
              //using (User user = new User(this.ConnectionString))
              //{
              //   tz = -1 * user.GetUserTimeZoneDayLightSaving(userId);
              //}
              to = dtTo < DateTime.Now ? dtTo : DateTime.Now;
              tsStopDuration = to.Subtract(from);
              dtLandmarkDetails.Rows.Add(vdescr, licensePlate, "Stop", from, to, FormatTimeString((long)tsStopDuration.TotalSeconds));
              tsTotalStop += tsStopDuration;
           }
           //dtLandmarkDetails.Rows.Add(vdescr, FormatTimeString((long)tsStopDuration.TotalSeconds), FormatTimeString(idlingDuration));
           dsResult.Tables.Add(dtLandmarkDetails);

           dtLandmarkTotal.Rows.Add(vdescr, FormatTimeString(totalIdling), FormatTimeString((long)tsTotalStop.TotalSeconds));
           dsResult.Tables.Add(dtLandmarkTotal);
         
           return dsResult;
           */
        }

        /// <summary>
        /// Get Landmark Details Report for a fleet
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="landmarkName"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns>FleetLandmarkReport->LandmarkDetails->[Description][StopDuration - sec][IdlingDuration - sec]</returns>
        public DataSet GetLandmarkFleetDetailsReport(int userId, int fleetId, string landmarkName, DateTime dtFrom, DateTime dtTo)
        {
            DataSet dsResult = new DataSet("FleetLandmarkReport");
            // duration details table
            DataTable dtLandmarkDetails = new DataTable("LandmarkDetails");
            dtLandmarkDetails.Columns.Add("Description", typeof(string));
            dtLandmarkDetails.Columns.Add("LicensePlate", typeof(string));
            dtLandmarkDetails.Columns.Add("Type", typeof(string));
            //dtLandmarkDetails.Columns.Add("DateIn", typeof(DateTime));
            //dtLandmarkDetails.Columns.Add("DateOut", typeof(DateTime));
            dtLandmarkDetails.Columns.Add("DateIn", typeof(string));
            dtLandmarkDetails.Columns.Add("DateOut", typeof(string));
            dtLandmarkDetails.Columns.Add("Duration", typeof(string));
            dtLandmarkDetails.Columns.Add("_Driver", typeof(string));

            //// total duration table
            //DataTable dtLandmarkTotal = new DataTable("TotalDuration");
            //dtLandmarkTotal.Columns.Add("Description", typeof(string));
            //dtLandmarkTotal.Columns.Add("TotalIdling", typeof(string));
            //dtLandmarkTotal.Columns.Add("TotalStop", typeof(string));

            // get fleet vehicles
            DAS.DB.FleetVehicles fv = new VLF.DAS.DB.FleetVehicles(this.sqlExec);
            DataSet dsVehicles = fv.GetVehiclesInfoByFleetId(fleetId);

            foreach (DataRow drVehicle in dsVehicles.Tables[0].Rows)
            {
                DataSet dsLmk =
                   GetLandmarkVehicleDetailsReport(userId, drVehicle["LicensePlate"].ToString(), landmarkName, dtFrom, dtTo);

                if (Util.IsDataSetValid(dsLmk))
                {
                    foreach (DataRow row in dsLmk.Tables["LandmarkDetails"].Rows)
                        dtLandmarkDetails.ImportRow(row);

                    //foreach (DataRow row in dsLmk.Tables["TotalDuration"].Rows)
                    //   dtLandmarkTotal.ImportRow(row);
                }
            }

            dsResult.Tables.Add(dtLandmarkDetails);
            //dsResult.Tables.Add(dtLandmarkTotal);
            return dsResult;
        }

        /// <summary>
        /// Calculate Vehicle Inactivity Report for CN company
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="licensePlate"></param>
        /// <param name="sensorId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns>DataSet</returns>
        public DataSet GetVehicleInactivityReport(int userID, string licensePlate, short sensorId, DateTime dtFrom, DateTime dtTo)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(this.sqlExec);
            SqlParameter[] sqlParams = new SqlParameter[5];
            sqlParams[0] = new SqlParameter("@user", userID);
            sqlParams[1] = new SqlParameter("@sensorId", sensorId);
            sqlParams[2] = new SqlParameter("@licenseP", licensePlate);
            sqlParams[3] = new SqlParameter("@dtFrom", dtFrom);
            sqlParams[4] = new SqlParameter("@dtTo", dtTo);
            return this.sqlExec.SPExecuteDataset("ReportInactivity_CN", sqlParams);
        }

        /// <summary>
        /// Inactivity Report 4 Fleet
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="fleetId"></param>
        /// <param name="sensorId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns>Fleet vehicles dataset with added column</returns>
        public DataSet GetFleetInactivityReport(int userID, int fleetId, short sensorId, DateTime dtFrom, DateTime dtTo)
        {
            DataSet dsReport = new DataSet("InactivityReport");
            dsReport.Tables.Add("Vehicles");
            dsReport.Tables[0].Columns.Add("Description", typeof(string));
            dsReport.Tables[0].Columns.Add("LicensePlate", typeof(string));
            dsReport.Tables[0].Columns.Add("VehicleTypeName", typeof(string));
            dsReport.Tables[0].Columns.Add("Field3", typeof(string));
            dsReport.Tables[0].Columns.Add("Field1", typeof(string));
            dsReport.Tables[0].Columns.Add("StateProvince", typeof(string));
            dsReport.Tables[0].Columns.Add("Field4", typeof(string));
            dsReport.Tables[0].Columns.Add("Field2", typeof(string));
            dsReport.Tables[0].Columns.Add("Color", typeof(string));
            dsReport.Tables[0].Columns.Add("DaysInactive", typeof(int));
            dsReport.Tables[0].Columns.Add("PercentInactive", typeof(int));
            // get fleet vehicles
            VLF.DAS.DB.FleetVehicles flt = new VLF.DAS.DB.FleetVehicles(this.sqlExec);
            StringBuilder sql =
               new StringBuilder("SELECT vlfVehicleAssignment.LicensePlate FROM vlfFleetVehicles");
            sql.AppendLine();
            sql.AppendLine("INNER JOIN vlfVehicleAssignment ON vlfFleetVehicles.VehicleId = vlfVehicleAssignment.VehicleId");
            sql.AppendLine("WHERE vlfFleetVehicles.FleetId = @fleetId");
            DataSet dsVehList = flt.GetRowsBySql(sql.ToString(), new SqlParameter("@fleetId", fleetId));

            if (Util.IsDataSetValid(dsVehList))
            {
                foreach (DataRow row in dsVehList.Tables[0].Rows)
                {
                    dsReport.Tables[0].ImportRow(
                       GetVehicleInactivityReport(userID, row["LicensePlate"].ToString(), sensorId, dtFrom, dtTo).Tables[0].Rows[0]);
                }
            }
            return dsReport;
        }

        #endregion

        #region Protected Functions

        // Changes for TimeZone Feature start
        /// <summary>
        /// Fill trip report parameters
        /// </summary>
        protected void FillTripReportParams_NewTZ(string licensePlate,
           DataSet dsResult,
           int userId,
           bool autoResolveLandmarks,
           ref DataTable tblVehicleSensors,
           ref DataTable tblVehicleGeozones,
           ref double carCost,
           ref double measurementUnits,
           ref short vehicleType)
        {
            try
            {
                // 1. Prepares report header (vehicle information)
                DB.VehicleInfo vehicle = new DB.VehicleInfo(sqlExec);
                /// DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],
                /// [MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],
                /// [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],
                /// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],
                /// [VehicleTypeId]
                DataSet dsVehicle = vehicle.GetVehicleInfoByLicensePlate_NewTZ(licensePlate);
                int boxId = VLF.CLS.Def.Const.unassignedIntValue;
                Int64 vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
                if (dsVehicle != null && dsVehicle.Tables.Count > 0 && dsVehicle.Tables[0].Rows.Count > 0)
                {
                    vehicleType = Convert.ToInt16(dsVehicle.Tables[0].Rows[0]["VehicleTypeId"]); // (short)VLF.CLS.Def.Enums.VehicleType.Trailer)

                    dsVehicle.Tables[0].TableName = "TripReportVehicleInfo";
                    DataTable dtVehicleInfo = new DataTable();
                    dtVehicleInfo = dsVehicle.Tables[0].Copy();
                    dsResult.Tables.Add(dtVehicleInfo);
                    boxId = Convert.ToInt32(dsVehicle.Tables[0].Rows[0]["BoxId"]);
                    vehicleId = Convert.ToInt64(dsResult.Tables[0].Rows[0]["VehicleId"]);

                    // 2. Prepares report header (driver information)
                    DB.DriverAssignment driver = new DB.DriverAssignment(sqlExec);
                    //[AssignedDateTime],[PersonId],[DriverLicense],[FirstName],[LastName],[Description]
                    DataSet dsDriverInfo = driver.GetDriverAssignmentForVehicle(vehicleId);
                    if ((dsDriverInfo != null) && (dsDriverInfo.Tables.Count > 0) && (dsDriverInfo.Tables[0].Rows.Count > 0))
                    {
                        DataTable dtDriverInfo = new DataTable();
                        dtDriverInfo = dsDriverInfo.Tables[0].Copy();
                        // add additional column "BoxId"
                        dtDriverInfo.Columns.Add("BoxId", typeof(int));
                        dtDriverInfo.Rows[0]["BoxId"] = boxId;
                        dtDriverInfo.TableName = "TripReportDriverInfo";
                        dsResult.Tables.Add(dtDriverInfo);
                    }
                }

                // 3. Prepares user-defined sensor info
                // [SensorId][SensorName][SensorAction][AlarmLevelOn][AlarmLevelOff]</returns>
                DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
                DataSet dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);
                if ((dsVehicleSensors != null) && (dsVehicleSensors.Tables.Count > 0))
                {
                    tblVehicleSensors = dsVehicleSensors.Tables[0].Copy();
                }

                // 4. Retrieve user preference
                DB.UserPreference userPref = new DB.UserPreference(sqlExec);
                DataSet dsUserPref = userPref.GetUserPreferenceInfo(userId, (int)VLF.CLS.Def.Enums.Preference.MeasurementUnits);
                if (dsUserPref != null && dsUserPref.Tables.Count > 0 && dsUserPref.Tables[0].Rows.Count > 0)
                {
                    measurementUnits = Convert.ToDouble(dsUserPref.Tables[0].Rows[0]["PreferenceValue"]);
                }

                // 6. Retrievs car cost
                carCost = Convert.ToDouble(vehicle.GetVehicleInfoStrFieldByLicensePlate("CostPerMile", licensePlate));
                // 7. Retrieves vehicle geozones
                DB.VehicleGeozone vehicleGeozone = new DB.VehicleGeozone(sqlExec);
                DataSet dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
                if ((dsGeozonesInfo != null) && (dsGeozonesInfo.Tables.Count > 0))
                {
                    tblVehicleGeozones = dsGeozonesInfo.Tables[0].Copy();
                }
            }
            catch
            {
                tblVehicleSensors = null;
                carCost = 1;
                measurementUnits = 1;
            }

        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Fill trip report parameters
        /// </summary>
        protected void FillTripReportParams(string licensePlate,
           DataSet dsResult,
           int userId,
           bool autoResolveLandmarks,
           ref DataTable tblVehicleSensors,
           ref DataTable tblVehicleGeozones,
           ref double carCost,
           ref double measurementUnits,
           ref short vehicleType)
        {
            try
            {
                // 1. Prepares report header (vehicle information)
                DB.VehicleInfo vehicle = new DB.VehicleInfo(sqlExec);
                /// DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],
                /// [MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],
                /// [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],
                /// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],
                /// [VehicleTypeId]
                DataSet dsVehicle = vehicle.GetVehicleInfoByLicensePlate(licensePlate);
                int boxId = VLF.CLS.Def.Const.unassignedIntValue;
                Int64 vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
                if (dsVehicle != null && dsVehicle.Tables.Count > 0 && dsVehicle.Tables[0].Rows.Count > 0)
                {
                    vehicleType = Convert.ToInt16(dsVehicle.Tables[0].Rows[0]["VehicleTypeId"]); // (short)VLF.CLS.Def.Enums.VehicleType.Trailer)

                    dsVehicle.Tables[0].TableName = "TripReportVehicleInfo";
                    DataTable dtVehicleInfo = new DataTable();
                    dtVehicleInfo = dsVehicle.Tables[0].Copy();
                    dsResult.Tables.Add(dtVehicleInfo);
                    boxId = Convert.ToInt32(dsVehicle.Tables[0].Rows[0]["BoxId"]);
                    vehicleId = Convert.ToInt64(dsResult.Tables[0].Rows[0]["VehicleId"]);

                    // 2. Prepares report header (driver information)
                    DB.DriverAssignment driver = new DB.DriverAssignment(sqlExec);
                    //[AssignedDateTime],[PersonId],[DriverLicense],[FirstName],[LastName],[Description]
                    DataSet dsDriverInfo = driver.GetDriverAssignmentForVehicle(vehicleId);
                    if ((dsDriverInfo != null) && (dsDriverInfo.Tables.Count > 0) && (dsDriverInfo.Tables[0].Rows.Count > 0))
                    {
                        DataTable dtDriverInfo = new DataTable();
                        dtDriverInfo = dsDriverInfo.Tables[0].Copy();
                        // add additional column "BoxId"
                        dtDriverInfo.Columns.Add("BoxId", typeof(int));
                        dtDriverInfo.Rows[0]["BoxId"] = boxId;
                        dtDriverInfo.TableName = "TripReportDriverInfo";
                        dsResult.Tables.Add(dtDriverInfo);
                    }
                }

                // 3. Prepares user-defined sensor info
                // [SensorId][SensorName][SensorAction][AlarmLevelOn][AlarmLevelOff]</returns>
                DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
                DataSet dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);
                if ((dsVehicleSensors != null) && (dsVehicleSensors.Tables.Count > 0))
                {
                    tblVehicleSensors = dsVehicleSensors.Tables[0].Copy();
                }

                // 4. Retrieve user preference
                DB.UserPreference userPref = new DB.UserPreference(sqlExec);
                DataSet dsUserPref = userPref.GetUserPreferenceInfo(userId, (int)VLF.CLS.Def.Enums.Preference.MeasurementUnits);
                if (dsUserPref != null && dsUserPref.Tables.Count > 0 && dsUserPref.Tables[0].Rows.Count > 0)
                {
                    measurementUnits = Convert.ToDouble(dsUserPref.Tables[0].Rows[0]["PreferenceValue"]);
                }

                // 6. Retrievs car cost
                carCost = Convert.ToDouble(vehicle.GetVehicleInfoStrFieldByLicensePlate("CostPerMile", licensePlate));
                // 7. Retrieves vehicle geozones
                DB.VehicleGeozone vehicleGeozone = new DB.VehicleGeozone(sqlExec);
                DataSet dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
                if ((dsGeozonesInfo != null) && (dsGeozonesInfo.Tables.Count > 0))
                {
                    tblVehicleGeozones = dsGeozonesInfo.Tables[0].Copy();
                }
            }
            catch
            {
                tblVehicleSensors = null;
                carCost = 1;
                measurementUnits = 1;
            }

        }

        /// <summary>
        /// Fill trip report parameters
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <param name="dsResult"></param>
        /// <param name="userId"></param>
        /// <param name="tblLandmarks"></param>
        /// <param name="tblVehicleSensors"></param>
        /// <param name="tblVehicleGeozones"></param>
        /// <param name="carCost"></param>
        /// <param name="measurementUnits"></param>
        /// <param name="isTrailer"></param>
        protected void FillTripReportParams(string licensePlate,
           DataSet dsResult,
           int userId,
           bool autoResolveLandmarks,
           ref DataTable tblVehicleSensors,
           ref DataTable tblVehicleGeozones,
           ref double carCost,
           ref double measurementUnits,
           ref bool isTrailer)
        {
            try
            {
                // 1. Prepares report header (vehicle information)
                DB.VehicleInfo vehicle = new DB.VehicleInfo(sqlExec);
                /// DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],
                /// [MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],
                /// [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],
                /// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],
                /// [VehicleTypeId]
                DataSet dsVehicle = vehicle.GetVehicleInfoByLicensePlate(licensePlate);
                int boxId = VLF.CLS.Def.Const.unassignedIntValue;
                Int64 vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
                if (dsVehicle != null && dsVehicle.Tables.Count > 0 && dsVehicle.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToInt16(dsVehicle.Tables[0].Rows[0]["VehicleTypeId"]) == (short)VLF.CLS.Def.Enums.VehicleType.Trailer)
                        isTrailer = true;
                    else
                        isTrailer = false;

                    dsVehicle.Tables[0].TableName = "TripReportVehicleInfo";
                    DataTable dtVehicleInfo = new DataTable();
                    dtVehicleInfo = dsVehicle.Tables[0].Copy();
                    dsResult.Tables.Add(dtVehicleInfo);
                    boxId = Convert.ToInt32(dsVehicle.Tables[0].Rows[0]["BoxId"]);
                    vehicleId = Convert.ToInt64(dsResult.Tables[0].Rows[0]["VehicleId"]);

                    // 2. Prepares report header (driver information)
                    DB.DriverAssignment driver = new DB.DriverAssignment(sqlExec);
                    //[AssignedDateTime],[PersonId],[DriverLicense],[FirstName],[LastName],[Description]
                    DataSet dsDriverInfo = driver.GetDriverAssignment(licensePlate);
                    if ((dsDriverInfo != null) && (dsDriverInfo.Tables.Count > 0) && (dsDriverInfo.Tables[0].Rows.Count > 0))
                    {
                        DataTable dtDriverInfo = new DataTable();
                        dtDriverInfo = dsDriverInfo.Tables[0].Copy();
                        // add additional column "BoxId"
                        dtDriverInfo.Columns.Add("BoxId", typeof(int));
                        dtDriverInfo.Rows[0]["BoxId"] = boxId;
                        dtDriverInfo.TableName = "TripReportDriverInfo";
                        dsResult.Tables.Add(dtDriverInfo);
                    }
                }

                // 3. Prepares user-defined sensor info
                // [SensorId][SensorName][SensorAction][AlarmLevelOn][AlarmLevelOff]</returns>
                DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
                DataSet dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);
                if ((dsVehicleSensors != null) && (dsVehicleSensors.Tables.Count > 0))
                {
                    tblVehicleSensors = dsVehicleSensors.Tables[0].Copy();
                }

                // 4. Retrieve user preference
                DB.UserPreference userPref = new DB.UserPreference(sqlExec);
                DataSet dsUserPref = userPref.GetUserPreferenceInfo(userId, (int)VLF.CLS.Def.Enums.Preference.MeasurementUnits);
                if (dsUserPref != null && dsUserPref.Tables.Count > 0 && dsUserPref.Tables[0].Rows.Count > 0)
                {
                    measurementUnits = Convert.ToDouble(dsUserPref.Tables[0].Rows[0]["PreferenceValue"]);
                }

                // 6. Retrievs car cost
                carCost = Convert.ToDouble(vehicle.GetVehicleInfoStrFieldByLicensePlate("CostPerMile", licensePlate));
                // 7. Retrieves vehicle geozones
                DB.VehicleGeozone vehicleGeozone = new DB.VehicleGeozone(sqlExec);
                DataSet dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
                if ((dsGeozonesInfo != null) && (dsGeozonesInfo.Tables.Count > 0))
                {
                    tblVehicleGeozones = dsGeozonesInfo.Tables[0].Copy();
                }
            }
            catch
            {
                tblVehicleSensors = null;
                carCost = 1;
                measurementUnits = 1;
            }
        }

        /// <summary>
        /// Retrieves configuration value
        /// </summary>
        /// <returns>[KeyValue]</returns>
        /// <exception cref="DASAppResultNotFoundException">Thrown if module name does not exist.</exception>
        /// <param name="moduleName"></param>
        /// <param name="groupID"></param>
        /// <param name="paramName"></param>
        /// <param name="defaultVal"></param>
        protected int GetConfigParameter(string moduleName, short groupID, string paramName, int defaultVal)
        {
            string retResult = "";
            // take Module ID in DB
            Configuration sysCfg = new Configuration(sqlExec);
            short moduleID = sysCfg.GetConfigurationModuleTypeId(moduleName);
            if (moduleID == VLF.CLS.Def.Const.unassignedShortValue)
                return defaultVal;

            retResult = sysCfg.GetConfigurationValue(moduleID, groupID, paramName);
            if (retResult == "")
                return defaultVal;
            return Convert.ToInt32(retResult);
        }
        #endregion

        /// <summary>
        /// Format time for landmark report
        /// </summary>
        /// <param name="seconds">Seconds value</param>
        /// <returns>Formatted string</returns>
        private string FormatTimeString(long seconds)
        {
            if (seconds < 60)
                return String.Format("{0} sec", seconds);
            else
            {
                long totalmin = (seconds - seconds % 60) / 60;
                if (totalmin < 60)
                {
                    return String.Format("{0:D2} min {1:D2} sec", totalmin, seconds % 60);
                }
                else
                {
                    int hrs = (int)((totalmin - totalmin % 60) / 60);
                    if (hrs < 24)
                    {
                        return String.Format("{0:D2} hr {1:D2} min {2:D2} sec", hrs, totalmin % 60, seconds % 60);
                    }
                    else
                    {
                        long days = (hrs - hrs % 24) / 24;
                        return String.Format("{0} d {1:D2} hr {2:D2} min {3:D2} sec", days, hrs % 24, totalmin % 60, seconds % 60);
                    }
                }
            }
        }

        /// <summary>
        /// Get Idling Summary Report Per Organization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetIdlingSummaryReportPerOrganization(Int32 userId, Int32 orgId, DateTime fromDateTime, DateTime toDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetIdlingSummaryReportPerOrganization(userId, orgId, fromDateTime, toDateTime);
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Get Activity Summary Report Per Organization
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetActivitySummaryReportPerOrganization_NewTZ(Int32 userId, Int32 orgId, DateTime fromDateTime, DateTime toDateTime, Int16 sensorNum)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetActivitySummaryReportPerOrganization_NewTZ(userId, orgId, fromDateTime, toDateTime, sensorNum);
        }



        // Changes for TimeZone Feature end

        /// <summary>
        /// Get Activity Summary Report Per Organization
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetActivitySummaryReportPerOrganization(Int32 userId, Int32 orgId, DateTime fromDateTime, DateTime toDateTime, Int16 sensorNum)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetActivitySummaryReportPerOrganization(userId, orgId, fromDateTime, toDateTime, sensorNum);
        }


        /// <summary>
        /// Get Activity Summary Report Per Vehicle
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetActivitySummaryReportPerVehicle(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int16 sensorNum)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetActivitySummaryReportPerVehicle(userId, fleetId, fromDateTime, toDateTime, sensorNum);
        }


        /// <summary>
        /// Get Activity Summary Report Per Vehicle
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetActivitySummaryReportPerVehicle_Hierarchy(Int32 userId, string NodeCode, DateTime fromDateTime, DateTime toDateTime, Int16 sensorNum)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetActivitySummaryReportPerVehicle_Hierarchy(userId, NodeCode, fromDateTime, toDateTime, sensorNum);
        }


        /// <summary>
        /// Get Activity Summary Report Per Vehicle Fuel
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetActivitySummaryReportPerVehicle_Fuel(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int16 sensorNum)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetActivitySummaryReportPerVehicle_Fuel(userId, fleetId, fromDateTime, toDateTime, sensorNum);
        }

        /// <summary>
        /// Get Activity Summary Report Per Vehicle
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetActivitySummaryReportPerVehicle_Special(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int16 sensorNum)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetActivitySummaryReportPerVehicle_Special(userId, fleetId, fromDateTime, toDateTime, sensorNum);
        }


        public DataSet GetActivitySummaryReportPerFleet_Special_Hierarchy(Int32 userId, string nodeCode, DateTime fromDateTime, DateTime toDateTime, Int16 sensorNum)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetActivitySummaryReportPerFleet_Special_Hierarchy(userId, nodeCode, fromDateTime, toDateTime, sensorNum);
        }

        public DataSet GetSensorActivityReport(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetSensorActivityReport(userId, fleetId, fromDateTime, toDateTime);
        }


        public DataSet GetSensorActivityReport_Hierarchy(Int32 userId, string nodeCode, DateTime fromDateTime, DateTime toDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetSensorActivityReport_Hierarchy(userId, nodeCode, fromDateTime, toDateTime);
        }

        /// <summary>
        /// Get Activity Summary Report Per Vehicle
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetActivitySummaryReportPerVehicle_Daily(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int16 sensorNum)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetActivitySummaryReportPerVehicle_Daily(userId, fleetId, fromDateTime, toDateTime, sensorNum);
        }

        /// <summary>
        /// Get Time at Landmark
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="landmark"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetTimeAtLandmarkReport(Int32 userId, string licensePlate, string landmark, DateTime fromDateTime, DateTime toDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetTimeAtLandmarkReport(userId, licensePlate, landmark, fromDateTime, toDateTime);
        }

        /// <summary>
        /// Get On-Road vs Off-Road Miles Report Dataset
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetOnOffRoadMilesReport(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetOnOffRoadMilesReport(userId, fleetId, fromDateTime, toDateTime);
        }


        /// <summary>
        /// Get On-Road vs Off-Road Miles Report Dataset
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetOnOffRoadMilesReport(int userId, string NodeCode, DateTime DateFrom, DateTime DateTo)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetOnOffRoadMilesReport(userId, NodeCode, DateFrom, DateTo);
        }


        public DataSet evtViolationsSpeedInLandmark(int userId, int fleetId, DateTime DateFrom, DateTime DateTo)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.evtViolationsSpeedInLandmark(userId, fleetId, DateFrom, DateTo);
        }


        public DataSet evtViolationsSpeedInLandmark_Hierarchy(int userId, string nodeCode, DateTime DateFrom, DateTime DateTo)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.evtViolationsSpeedInLandmark_Hierarchy(userId, nodeCode, DateFrom, DateTo);
        }

        /// <summary>
        /// Garmin Messages Report
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GarminMessages_Report(Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int32 userId)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GarminMessages_Report(fleetId, fromDateTime, toDateTime, userId);
        }


        /// <summary>
        /// evtViolationsFuel
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet evtViolationsFuel(int fleetId, DateTime fromDate, DateTime toDate, int userId)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.evtViolationsFuel(fleetId, fromDate, toDate, userId);
        }

        /// <summary>
        /// Get Time at Landmark
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="landmark"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetTimeAtLandmarkFleetReport(Int32 userId, int fleetId, string landmark, DateTime fromDateTime, DateTime toDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetTimeAtLandmarkFleetReport(userId, fleetId, landmark, fromDateTime, toDateTime);
        }

        #region Extended Utilization Reports
        /// <summary>
        /// Daily Vehicle Utilization Report
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetDailyVehicleUtilizationReport(Int32 userId, Int32 vehicleId, DateTime fromDateTime, DateTime toDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetDailyVehicleUtilizationReport(userId, vehicleId, fromDateTime, toDateTime);
        }
        /// <summary>
        /// Daily Fleet Utilization Report
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetDailyFleetUtilizationReport(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Single Cost)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetDailyFleetUtilizationReport(userId, fleetId, fromDateTime, toDateTime, Cost);
        }
        /// <summary>
        /// Speed Violations Report For Fleet
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="speed1"></param>
        /// <param name="speed2"></param>
        /// <param name="speed3"></param>
        /// <returns></returns>
        public DataSet GetSpeedViolationsReportForFleet(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int32 Type, string ColorFilter)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetSpeedViolationsReportForFleet(userId, fleetId, fromDateTime, toDateTime, Type, ColorFilter);
        }


        /// <summary>
        /// Speed Violations Report For Fleet
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="speed1"></param>
        /// <param name="speed2"></param>
        /// <param name="speed3"></param>
        /// <returns></returns>
        public DataSet GetSpeedViolationsReportForFleet_RoadSpeed(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int32 Type, string ColorFilter)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetSpeedViolationsReportForFleet_RoadSpeed(userId, fleetId, fromDateTime, toDateTime, Type, ColorFilter);
        }




        /// <summary>
        /// Speed Violations Report For Fleet
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="speed1"></param>
        /// <param name="speed2"></param>
        /// <param name="speed3"></param>
        /// <returns></returns>
        public DataSet GetSpeedViolationsSummaryReportForFleet_RoadSpeed(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int32 Type, string ColorFilter, Int16 postedSpeedOnly)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetSpeedViolationsSummaryReportForFleet_RoadSpeed(userId, fleetId, fromDateTime, toDateTime, Type, ColorFilter, postedSpeedOnly);
        }


        /// <summary>
        /// Speed Violations Report For Fleet
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="speed1"></param>
        /// <param name="speed2"></param>
        /// <param name="speed3"></param>
        /// <returns></returns>
        public DataSet GetSpeedViolationsSummaryReportForFleet_RoadSpeed_Hierarchy(Int32 userId, string nodeCode, DateTime fromDateTime, DateTime toDateTime, Int32 Type, string ColorFilter, Int16 postedSpeedOnly)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetSpeedViolationsSummaryReportForFleet_RoadSpeed_Hierarchy(userId, nodeCode, fromDateTime, toDateTime, Type, ColorFilter, postedSpeedOnly);
        }


        /// <summary>
        /// Speed Violations Report For Fleet
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="speed1"></param>
        /// <param name="speed2"></param>
        /// <param name="speed3"></param>
        /// <returns></returns>
        public DataSet GetSpeedViolationsDetailsReport_RoadSpeed(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int16 PostedSpeedOnly, Int32 Delta)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetSpeedViolationsDetailsReport_RoadSpeed(userId, fleetId, fromDateTime, toDateTime, PostedSpeedOnly, Delta);
        }
        /// <summary>
        /// Speed Duration Report
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetSpeedDistributionReport(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetSpeedDistributionReport(userId, fleetId, fromDateTime, toDateTime);
        }


        public DataSet GetSpeedDistributionReport_Hierarchy(Int32 userId, string nodeCode, DateTime fromDateTime, DateTime toDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetSpeedDistributionReport_Hierarchy(userId, nodeCode, fromDateTime, toDateTime);
        }

        public DataSet GetSpeedSummaryViolationsReportForFleet(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int32 Type, string ColorFilter)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetSpeedSummaryViolationsReportForFleet(userId, fleetId, fromDateTime, toDateTime, Type, ColorFilter);
        }

        /// <summary>
        /// Speed Violations Report For Fleet
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="speed1"></param>
        /// <param name="speed2"></param>
        /// <param name="speed3"></param>
        /// <returns></returns>
        public DataSet GetSpeedViolationsDetailsReportForFleet(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int32 Type, string ColorFilter)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetSpeedViolationsDetailsReportForFleet(userId, fleetId, fromDateTime, toDateTime, Type, ColorFilter);
        }


        /// <summary>
        /// Speed Violations Report For Fleet
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="speed1"></param>
        /// <param name="speed2"></param>
        /// <param name="speed3"></param>
        /// <returns></returns>
        public DataSet GetSpeedViolationsDetailsReportForFleet_RoadSpeed(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int32 Type, string ColorFilter)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetSpeedViolationsDetailsReportForFleet_RoadSpeed(userId, fleetId, fromDateTime, toDateTime, Type, ColorFilter);
        }

        /// <summary>
        /// Get CN Fleet Utilization Report
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetCNFleetUtilizationReport(Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, string colorFilter)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetCNFleetUtilizationReport(fleetId, fromDateTime, toDateTime, colorFilter);
        }



        /// <summary>
        /// Get CN Fleet Monthly Utilization Report
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetCNGetMonthlyFleetSummary(Int32 fleetId, DateTime fromDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetCNGetMonthlyFleetSummary(fleetId, fromDateTime);
        }


        /// <summary>
        /// Get CN Summary Activity Per Month 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet CN_GetSummaryActivityPerMonth(Int32 fleetId, DateTime fromDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.CN_GetSummaryActivityPerMonth(fleetId, fromDateTime);
        }

        // Changes for TimeZone Feature start
        /// <summary>
        ///        Get Trips Summary Report
        ///        GUID : 40
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="sensorNum"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetTripsSummaryReport_NewTZ(Int32 userId, string licensePlate, Int32 sensorNum, DateTime fromDateTime, DateTime toDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetTripsSummaryReport_NewTZ(userId, licensePlate, sensorNum, fromDateTime, toDateTime);
        }
        // Changes for TimeZone Feature end

        /// <summary>
        ///        Get Trips Summary Report
        ///        GUID : 40
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="sensorNum"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetTripsSummaryReport(Int32 userId, string licensePlate, Int32 sensorNum, DateTime fromDateTime, DateTime toDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetTripsSummaryReport(userId, licensePlate, sensorNum, fromDateTime, toDateTime);
        }


        /// <summary>
        ///        Get Trips Summary Report
        ///        GUID : 40
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="sensorNum"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetTripsSummaryReport(Int32 userId, string licensePlate, Int32 sensorNum, DateTime fromDateTime, DateTime toDateTime, Int32 DriverId)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetTripsSummaryReport(userId, licensePlate, sensorNum, fromDateTime, toDateTime, DriverId);
        }




        public DataSet GetTripsSummaryReportByFleet(Int32 userId, int fleetId, Int32 sensorNum, DateTime fromDateTime, DateTime toDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetTripsSummaryReportByFleet(userId, fleetId, sensorNum, fromDateTime, toDateTime);
        }


        public DataSet GetTripsSummaryReportByFleet(Int32 userId, int fleetId, Int32 sensorNum, DateTime fromDateTime, DateTime toDateTime, Int32 DriverId)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetTripsSummaryReportByFleet(userId, fleetId, sensorNum, fromDateTime, toDateTime, DriverId);
        }

        public DataSet GetIdlingDriverReport(int fleetId, DateTime dtFrom, DateTime dtTo, int driverId, int idlingThreshold, int userId)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetIdlingDriverReport(fleetId, dtFrom, dtTo, driverId, idlingThreshold, userId);
        }

        public DataSet evtDriverViolations(Int32 userId, Int32 driverId, Int32 maskViolations, DateTime dtFrom, DateTime dtTo, int speed)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.evtDriverViolations(userId, driverId, maskViolations, dtFrom, dtTo, speed);
        }

        public DataSet GetBoxSettingsReport(int OrgId)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetBoxSettingsReport(OrgId);
        }



        public DataSet GetUserSettingsReport(int OrgId)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetUserSettingsReport(OrgId);
        }


        /// <summary>
        ///        Get Transportation Mileage Report
        ///        GUID : 63
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="sensorNum"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetTransportationMileageReport(Int32 userId, string licensePlate, Int32 sensorNum, DateTime fromDateTime, DateTime toDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetTransportationMileageReport(userId, licensePlate, sensorNum, fromDateTime, toDateTime);
        }


        /// <summary>
        /// Report for Brickman - Salt Spreader Summary
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <param name="userId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetActivitySummarySaltSpreader(Int32 userId, int boxId, DateTime fromDateTime, DateTime toDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetActivitySummarySaltSpreader(userId, boxId, fromDateTime, toDateTime);
        }

        /// <summary>
        /// CNElectronicInvoice
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public DataSet CNElectronicInvoice(DateTime from, DateTime to)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.CNElectronicInvoice(from, to);
        }



        /// <summary>
        /// VehicleInfoDataDump
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public DataSet GetVehicleInfoDataDump(Int32 orgId)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetVehicleInfoDataDump(orgId);
        }

        /// <summary>
        /// Get Vehicles Status Report (CN Report)
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        public DataSet GetVehiclesStatusReport(Int32 OrgId)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetVehiclesStatusReport(OrgId);
        }


        /// <summary>
        /// Fleet Membership Report
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        public DataSet GetFleetMembershipReport(Int32 userId)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetFleetMembershipReport(userId);
        }



        /// <summary>
        /// Fleet Membership Report
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        public DataSet GetFleetMembershipReport(Int32 userId, Int16 activeVehicles)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetFleetMembershipReport(userId, activeVehicles);
        }



        /// <summary>
        /// Brickman Fleet Membership Report
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        public DataSet GetBrickmanFleetMembershipReport(Int32 userId, Int32 activeVehicles)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetBrickmanFleetMembershipReport(userId, activeVehicles);
        }


        /// <summary>
        /// Fleet Membership Report
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        public DataSet GetFleetMembershipReportUser(Int32 orgId)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetFleetMembershipReportUser(orgId);
        }

        public DataSet GetActivityAtLandmarkSummaryReportPerFleetOnTheFly(long landmarkId, int fleetId, DateTime dtFrom, DateTime dtTo, int userId, long JobId)
        {
            VLF.DAS.DB.Landmark rpt = new VLF.DAS.DB.Landmark(sqlExec);
            return rpt.GetActivityAtLandmarkSummaryReportPerFleetOnTheFly(landmarkId, fleetId, dtFrom, dtTo, userId, JobId);
        }


        public DataSet GetActivityAtLandmarkSummaryReportPerFleet(int fleetId, DateTime dtFrom, DateTime dtTo, int userId)
        {
            VLF.DAS.DB.Landmark rpt = new VLF.DAS.DB.Landmark(sqlExec);
            return rpt.GetActivityAtLandmarkSummaryReportPerFleet(fleetId, dtFrom, dtTo, userId);
        }


        public DataSet GetActivityAtLandmarkSummaryReportPerFleet(int fleetId, DateTime dtFrom, DateTime dtTo, int userId, Int16 activeVehicles)
        {
            VLF.DAS.DB.Landmark rpt = new VLF.DAS.DB.Landmark(sqlExec);
            return rpt.GetActivityAtLandmarkSummaryReportPerFleet(fleetId, dtFrom, dtTo, userId, activeVehicles);
        }



        public DataSet evtFactEventsYear_Report(int fleetId, DateTime dtFrom, DateTime dtTo, int userId, Int16 activeVehicles)
        {
            VLF.DAS.DB.Landmark rpt = new VLF.DAS.DB.Landmark(sqlExec);
            return rpt.evtFactEventsYear_Report(fleetId, dtFrom, dtTo, userId, activeVehicles);

        }


        public DataSet evtFactEventsYear_Report(int fleetId, DateTime dtFrom, DateTime dtTo, int userId, Int16 activeVehicles, Int16 vehicleTypeId)
        {
            VLF.DAS.DB.Landmark rpt = new VLF.DAS.DB.Landmark(sqlExec);
            return rpt.evtFactEventsYear_Report(fleetId, dtFrom, dtTo, userId, activeVehicles, vehicleTypeId);

        }


        public DataSet GetActivityAtLandmarkSummaryReportPerFleetSnow(int fleetId, DateTime dtFrom, DateTime dtTo, int userId)
        {
            VLF.DAS.DB.Landmark rpt = new VLF.DAS.DB.Landmark(sqlExec);
            return rpt.GetActivityAtLandmarkSummaryReportPerFleetSnow(fleetId, dtFrom, dtTo, userId);
        }





        public DataSet GetActivityAtLandmarkSummaryReportPerFleetSnow(int fleetId, DateTime dtFrom, DateTime dtTo, int userId, Int16 activeVehicles)
        {
            VLF.DAS.DB.Landmark rpt = new VLF.DAS.DB.Landmark(sqlExec);
            return rpt.GetActivityAtLandmarkSummaryReportPerFleetSnow(fleetId, dtFrom, dtTo, userId, activeVehicles);
        }



        public DataSet GetActivityAtLandmarkSummaryReportPerFleetSnow_MediaType(int fleetId, DateTime dtFrom, DateTime dtTo, int userId, Int16 activeVehicles, Int16 MediaTypeId)
        {
            VLF.DAS.DB.Landmark rpt = new VLF.DAS.DB.Landmark(sqlExec);
            return rpt.GetActivityAtLandmarkSummaryReportPerFleetSnow_MediaType(fleetId, dtFrom, dtTo, userId, activeVehicles, MediaTypeId);
        }

        public DataSet GetActivityAtLandmarkSummaryReportPerFleet201107(int fleetId, DateTime dtFrom, DateTime dtTo, int userId)
        {
            VLF.DAS.DB.Landmark rpt = new VLF.DAS.DB.Landmark(sqlExec);
            return rpt.GetActivityAtLandmarkSummaryReportPerFleet201107(fleetId, dtFrom, dtTo, userId);
        }

        public DataSet GetActivityOutsideLandmark(int userId, int fleetId, int vehicleId, DateTime dtFrom, DateTime dtTo)
        {
            VLF.DAS.DB.Report report = new VLF.DAS.DB.Report(sqlExec);
            return report.GetActivityOutsideLandmark(userId, fleetId, vehicleId, dtFrom, dtTo);
        }


        public DataSet GetActivityOutsideLandmark(int userId, int fleetId, int vehicleId, DateTime dtFrom, DateTime dtTo, Int16 activeVehicles)
        {
            VLF.DAS.DB.Report report = new VLF.DAS.DB.Report(sqlExec);
            return report.GetActivityOutsideLandmark(userId, fleetId, vehicleId, dtFrom, dtTo, activeVehicles);
        }

        public DataSet GetActivityInLandmark(int userId, int fleetId, int vehicleId, DateTime dtFrom, DateTime dtTo)
        {
            VLF.DAS.DB.Report report = new VLF.DAS.DB.Report(sqlExec);
            return report.GetActivityInLandmark(userId, fleetId, vehicleId, dtFrom, dtTo);
        }



        public DataSet Dashboard_Idling(int fleetId, DateTime fromDate, DateTime toDate)
        {
            VLF.DAS.DB.Report report = new VLF.DAS.DB.Report(sqlExec);
            return report.Dashboard_Idling(fleetId, fromDate, toDate);
        }


        public DataSet Dashboard_Idling(int userId,int fleetId, DateTime fromDate, DateTime toDate)
        {
            VLF.DAS.DB.Report report = new VLF.DAS.DB.Report(sqlExec);
            return report.Dashboard_Idling(userId,fleetId, fromDate, toDate);
        }


        public DataSet Dashboard_Violations(int fleetId, DateTime fromDate, DateTime toDate, int userId)
        {
            VLF.DAS.DB.Report report = new VLF.DAS.DB.Report(sqlExec);
            return report.Dashboard_Violations(fleetId, fromDate, toDate, userId);
        }


        // Changes for TimeZone Feature start

        public DataSet AHA_Report_NewTZ(int UserId, int Top, int Hours)
        {
            VLF.DAS.DB.Report report = new VLF.DAS.DB.Report(sqlExec);
            return report.AHA_Report_NewTZ(UserId, Top, Hours);
        }

        // Changes for TimeZone Feature end

        public DataSet AHA_Report(int UserId, int Top, int Hours)
        {
            VLF.DAS.DB.Report report = new VLF.DAS.DB.Report(sqlExec);
            return report.AHA_Report(UserId, Top, Hours);
        }

      

        public DataSet evtViolation_Report(int fleetId, DateTime fromDate, DateTime toDate, int userId)
        {
            VLF.DAS.DB.Report report = new VLF.DAS.DB.Report(sqlExec);
            return report.evtViolation_Report(fleetId, fromDate, toDate, userId);
        }


        public DataSet evtDriverViolationsFleet_Report(int fleetId, DateTime fromDate, DateTime toDate, int userId, int maskViolations, string ViolationPoints)
        {
            VLF.DAS.DB.Report report = new VLF.DAS.DB.Report(sqlExec);
            return report.evtDriverViolationsFleet_Report(fleetId, fromDate, toDate, userId, maskViolations, ViolationPoints);
        }


        public DataSet evtFactEvents_Report(int fleetId, DateTime fromDate, DateTime toDate, int userId)
        {
            VLF.DAS.DB.Report report = new VLF.DAS.DB.Report(sqlExec);
            return report.evtFactEvents_Report(fleetId, fromDate, toDate, userId);
        }



        public DataSet evtFactEvents_Report(int fleetId, DateTime fromDate, DateTime toDate, Int16 idlingHrs, int userId)
        {
            VLF.DAS.DB.Report report = new VLF.DAS.DB.Report(sqlExec);
            return report.evtFactEvents_Report(fleetId, fromDate, toDate, idlingHrs, userId);
        }


        public DataSet MaintenanceSpecialReport(Int32 fleetId, Int32 userId)
        {
            VLF.DAS.DB.Report report = new VLF.DAS.DB.Report(sqlExec);
            return report.MaintenanceSpecialReport(fleetId, userId);
        }


        /// <summary>
        /// Get Report Files By ReportId
        /// </summary>
        /// <param name="reportid"></param>
        /// <returns></returns>
        public DataSet ReportBoxGeozone(int UserId)
        {
            VLF.DAS.DB.Report report = new VLF.DAS.DB.Report(sqlExec);
            return report.ReportBoxGeozone(UserId);
        }

        public DataSet evtOverTimeDurationInLandmark(int userId, int fleetId, DateTime DateFrom, DateTime DateTo)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.evtOverTimeDurationInLandmark(userId, fleetId, DateFrom, DateTo);
        }


        public int ReportSchedulesDriver_Status(int DriverId, Int16 ReportGuiId, DateTime FromDate, DateTime ToDate, string Email, string LinkURL, string Status)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.ReportSchedulesDriver_Status( DriverId,  ReportGuiId,  FromDate,  ToDate,  Email,  LinkURL,  Status);
        }


        public DataSet ReportSchedulesDriver_Get(string Status)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.ReportSchedulesDriver_Get( Status);
        }

        #endregion


        #region State Milage Reports
        /// <summary>
        /// Get State Mileage Per Fleet
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="summary"></param>
        /// <returns></returns>
        public DataSet GetStateMileagePerFleet(Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int16 summary)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetStateMileagePerFleet(fleetId, fromDateTime, toDateTime, summary);
        }


        /// <summary>
        /// Get State Mileage Per Fleet
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="summary"></param>
        /// <returns></returns>
        public DataSet GetStateMileagePerFleet_StateBased(Int32 fleetId, DateTime fromDateTime, DateTime toDateTime)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetStateMileagePerFleet_StateBased(fleetId, fromDateTime, toDateTime);
        }

        /// <summary>
        /// Get State Mileage Per Box
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="summary"></param>
        /// <returns></returns>
        public DataSet GetStateMileagePerVehicle(string licensePlate, DateTime fromDateTime, DateTime toDateTime, Int16 summary)
        {
            VLF.DAS.DB.Report rpt = new VLF.DAS.DB.Report(sqlExec);
            return rpt.GetStateMileagePerVehicle(licensePlate, fromDateTime, toDateTime, summary);
        }
        #endregion
    }




    /// <summary>
    /// Scheduled reports
    /// </summary>
    public class ReportScheduler : Das
    {
        DB.ReportScheduler scheduler = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public ReportScheduler(string connectionString)
            : base(connectionString)
        {
            scheduler = new DB.ReportScheduler(base.sqlExec);
        }

        /// <summary>
        /// Add new scheduled report
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="isfleet"></param>
        /// <param name="fleetid"></param>
        /// <param name="parameters"></param>
        /// <param name="email"></param>
        /// <param name="userid"></param>
        /// <param name="guiid"></param>
        /// <param name="status"></param>
        /// <param name="statusdate"></param>
        /// <param name="frequency"></param>
        /// <param name="freqparam"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="deliveryMethod"></param>
        /// <param name="lang"></param>
        public void AddSchedule(DateTime from, DateTime to, bool isfleet, int fleetid, string parameters, string email,
           int userid, int guiid, string status, DateTime statusdate, short frequency, short freqparam, DateTime startdate, DateTime enddate,
           short deliveryMethod, string lang)
        {
            scheduler.AddReportSchedule(from, to, isfleet, fleetid, parameters, email, userid, guiid,
               status, statusdate, frequency, freqparam, startdate, enddate, deliveryMethod, lang);
        }

        /// <summary>
        /// Add new scheduled report, incl. file format
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="isfleet"></param>
        /// <param name="fleetid"></param>
        /// <param name="parameters"></param>
        /// <param name="email"></param>
        /// <param name="userid"></param>
        /// <param name="guiid"></param>
        /// <param name="status"></param>
        /// <param name="statusdate"></param>
        /// <param name="frequency"></param>
        /// <param name="freqparam"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="deliveryMethod"></param>
        /// <param name="lang"></param>
        /// <param name="format"></param>
        public int AddSchedule(DateTime from, DateTime to, bool isfleet, int fleetid, string parameters, string email,
           int userid, int guiid, string status, DateTime statusdate, short frequency, short freqparam, DateTime startdate, DateTime enddate,
           short deliveryMethod, string lang, short format)
        {
            return scheduler.AddReportSchedule(from, to, isfleet, fleetid, parameters, email, userid, guiid,
               status, statusdate, frequency, freqparam, startdate, enddate, deliveryMethod, lang, format);
        }

        /// <summary>
        /// Delete scheduled report
        /// </summary>
        /// <param name="reportid"></param>
        /// <param name="userid"></param>
        public void DeleteSchedule(int reportid, int userid)
        {
            scheduler.DeleteReportSchedule(reportid, userid);
        }

        /// <summary>
        /// Get Scheduled Reports By UserID
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public DataSet GetScheduledReportsByUserID(int userid)
        {
            return scheduler.GetScheduledReportsByUserID(userid);
        }

        /// <summary>
        /// Delete Scheduled Report By ReportID
        /// </summary>
        /// <param name="reportid"></param>
        public void DeleteScheduledReportByReportID(int reportid)
        {
            scheduler.DeleteRowsByIntField("ReportID", reportid, " delete scheduled report");
        }

        /// <summary>
        /// Get Report Files By UserId
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public DataSet GetReportFilesByUserId(int userid)
        {
            return scheduler.GetReportFilesByUserId(userid);
        }


        /// <summary>
        /// Get Report Files By ReportId
        /// </summary>
        /// <param name="reportid"></param>
        /// <returns></returns>
        public DataSet GetReportFilesByReportId(int reportid)
        {
            return scheduler.GetReportFilesByReportId(reportid);
        }





        /// <summary>
        /// Delete Report File by Row Id
        /// </summary>
        /// <param name="rowid"></param>
        public int DeleteReportFile(int rowid)
        {
            return scheduler.DeleteReportFile(rowid);
        }




       



    }
}
