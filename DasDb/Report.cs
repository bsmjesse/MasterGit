using System;
using System.Data;
using System.Data.SqlClient;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;

namespace VLF.DAS.DB
{
    /// <summary>
    /// Provides interface to all reports in the system
    /// </summary>
    /// <remarks>
    /// This class unlike other DB classes does not connected to the table directly
    /// In this case in constructor, first parameter "table name" is empty.
    /// </remarks>
    public class Report : TblGenInterfaces
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public Report(SQLExecuter sqlExec)
            : base("", sqlExec)
        {
        }



        #region Reports
        // Changes for TimeZone Feature start
        /// <summary>
        /// Returns history information from vlfMsgInHst table. 
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="userId"></param>
        /// <param name="dsParams"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>
        /// DataSet [BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
        /// [BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetMsgInHistoryInformation_NewTZ(string licensePlate,
                                         string from,
                                         string to,
                                         int userId,
                                         DataSet dsParams,
                                         ref bool requestOverflowed,
                                         ref int totalSqlRecords)
        {
            DataSet dsResult = new DataSet();
            // 1. Lookup all box ids by vehicle id
            Int64 vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
            // [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
            DB.VehicleAssignment assign = new DB.VehicleAssignment(sqlExec);
            DataSet dsAssign = assign.GetVehicleAssignmentBy("LicensePlate", licensePlate);
            if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
            {
                vehicleId = Convert.ToInt64(dsAssign.Tables[0].Rows[0]["VehicleId"]);

                // 2. Retrieve all historical box/licensePlate assignments during report period (from,to)
                DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
                //[AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]
                DataSet dsAssignHst = assignHst.GetAllVehiclesAssignmentsBy("VehicleId", vehicleId, Convert.ToDateTime(from), Convert.ToDateTime(to), " ORDER BY AssignedDateTime ASC");
                if (dsAssignHst != null && dsAssignHst.Tables.Count > 0)
                {
                    // 3.1	For each assignment in the range of from/to datetime mearge results
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
                            currAssignTo = Convert.ToDateTime(to);

                        if (currAssignFrom.Ticks < Convert.ToDateTime(from).Ticks)
                            currAssignFrom = new DateTime(Convert.ToDateTime(from).Ticks);

                        if (currAssignTo.Ticks > Convert.ToDateTime(to).Ticks)
                            currAssignTo = new DateTime(Convert.ToDateTime(to).Ticks);

                        if (dsParams != null && dsParams.Tables.Count > 0 && dsParams.Tables[0].Rows.Count > 0)
                            currResult = GetMsgInHistoryExceptionInformation_NewTZ(Convert.ToInt32(ittr["BoxId"]), currAssignFrom.ToString(), currAssignTo.ToString(), userId, dsParams, ref requestOverflowed, ref totalSqlRecords);
                        else
                            currResult = GetMsgInHistoryInformation_NewTZ(Convert.ToInt32(ittr["BoxId"]), currAssignFrom.ToString(), currAssignTo.ToString(), userId, ref requestOverflowed, ref totalSqlRecords);
                        if (dsResult != null && currResult != null)
                            dsResult.Merge(currResult);
                    }
                    // 3.2	Add current assignment
                    if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
                    {
                        currAssignFrom = Convert.ToDateTime(dsAssign.Tables[0].Rows[0]["AssignedDateTime"]);
                        if (currAssignFrom.Ticks < Convert.ToDateTime(from).Ticks)
                            currAssignFrom = new DateTime(Convert.ToDateTime(from).Ticks);
                        if (dsParams != null && dsParams.Tables.Count > 0 && dsParams.Tables[0].Rows.Count > 0)
                            currResult = GetMsgInHistoryExceptionInformation_NewTZ(Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]), currAssignFrom.ToString(), to, userId, dsParams, ref requestOverflowed, ref totalSqlRecords);
                        else
                            currResult = GetMsgInHistoryInformation_NewTZ(Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]), currAssignFrom.ToString(), to, userId, ref requestOverflowed, ref totalSqlRecords);
                        if (dsResult != null && currResult != null)
                            dsResult.Merge(currResult);
                    }
                }
            }
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "VehicleHistory";
                    // Add VehicleId column to result dataset
                    dsResult.Tables[0].Columns.Add("VehicleId", typeof(Int64));
                    // Ad vehicle id to every row
                    foreach (DataRow ittr in dsResult.Tables[0].Rows)
                        ittr["VehicleId"] = vehicleId;
                }
                dsResult.DataSetName = "MsgInHistory";
            }
            return dsResult;
        }
        // Changes for TimeZone Feature end
        /// <summary>
        /// Returns history information from vlfMsgInHst table. 
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="userId"></param>
        /// <param name="dsParams"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>
        /// DataSet [BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
        /// [BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetMsgInHistoryInformation(string licensePlate,
                                         string from,
                                         string to,
                                         int userId,
                                         DataSet dsParams,
                                         ref bool requestOverflowed,
                                         ref int totalSqlRecords)
        {
            DataSet dsResult = new DataSet();
            // 1. Lookup all box ids by vehicle id
            Int64 vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
            // [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
            DB.VehicleAssignment assign = new DB.VehicleAssignment(sqlExec);
            DataSet dsAssign = assign.GetVehicleAssignmentBy("LicensePlate", licensePlate);
            if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
            {
                vehicleId = Convert.ToInt64(dsAssign.Tables[0].Rows[0]["VehicleId"]);

                // 2. Retrieve all historical box/licensePlate assignments during report period (from,to)
                DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
                //[AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]
                DataSet dsAssignHst = assignHst.GetAllVehiclesAssignmentsBy("VehicleId", vehicleId, Convert.ToDateTime(from), Convert.ToDateTime(to), " ORDER BY AssignedDateTime ASC");
                if (dsAssignHst != null && dsAssignHst.Tables.Count > 0)
                {
                    // 3.1	For each assignment in the range of from/to datetime mearge results
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
                            currAssignTo = Convert.ToDateTime(to);

                        if (currAssignFrom.Ticks < Convert.ToDateTime(from).Ticks)
                            currAssignFrom = new DateTime(Convert.ToDateTime(from).Ticks);

                        if (currAssignTo.Ticks > Convert.ToDateTime(to).Ticks)
                            currAssignTo = new DateTime(Convert.ToDateTime(to).Ticks);

                        if (dsParams != null && dsParams.Tables.Count > 0 && dsParams.Tables[0].Rows.Count > 0)
                            currResult = GetMsgInHistoryExceptionInformation(Convert.ToInt32(ittr["BoxId"]), currAssignFrom.ToString(), currAssignTo.ToString(), userId, dsParams, ref requestOverflowed, ref totalSqlRecords);
                        else
                            currResult = GetMsgInHistoryInformation(Convert.ToInt32(ittr["BoxId"]), currAssignFrom.ToString(), currAssignTo.ToString(), userId, ref requestOverflowed, ref totalSqlRecords);
                        if (dsResult != null && currResult != null)
                            dsResult.Merge(currResult);
                    }
                    // 3.2	Add current assignment
                    if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
                    {
                        currAssignFrom = Convert.ToDateTime(dsAssign.Tables[0].Rows[0]["AssignedDateTime"]);
                        if (currAssignFrom.Ticks < Convert.ToDateTime(from).Ticks)
                            currAssignFrom = new DateTime(Convert.ToDateTime(from).Ticks);
                        if (dsParams != null && dsParams.Tables.Count > 0 && dsParams.Tables[0].Rows.Count > 0)
                            currResult = GetMsgInHistoryExceptionInformation(Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]), currAssignFrom.ToString(), to, userId, dsParams, ref requestOverflowed, ref totalSqlRecords);
                        else
                            currResult = GetMsgInHistoryInformation(Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]), currAssignFrom.ToString(), to, userId, ref requestOverflowed, ref totalSqlRecords);
                        if (dsResult != null && currResult != null)
                            dsResult.Merge(currResult);
                    }
                }
            }
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "VehicleHistory";
                    // Add VehicleId column to result dataset
                    dsResult.Tables[0].Columns.Add("VehicleId", typeof(Int64));
                    // Ad vehicle id to every row
                    foreach (DataRow ittr in dsResult.Tables[0].Rows)
                        ittr["VehicleId"] = vehicleId;
                }
                dsResult.DataSetName = "MsgInHistory";
            }
            return dsResult;
        }


        /// <summary>
        /// Returns history information from vlfMsgInHst table. 
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="userId"></param>
        /// <param name="dsParams"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>
        /// DataSet [BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
        /// [BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetMsgInHistoryInformationBrickman(string licensePlate,
                                         string from,
                                         string to,
                                         int userId,
                                         DataSet dsParams,
                                         ref bool requestOverflowed,
                                         ref int totalSqlRecords)
        {
            DataSet dsResult = new DataSet();
            // 1. Lookup all box ids by vehicle id
            Int64 vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
            // [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
            DB.VehicleAssignment assign = new DB.VehicleAssignment(sqlExec);
            DataSet dsAssign = assign.GetVehicleAssignmentBy("LicensePlate", licensePlate);
            if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
            {
                vehicleId = Convert.ToInt64(dsAssign.Tables[0].Rows[0]["VehicleId"]);

                // 2. Retrieve all historical box/licensePlate assignments during report period (from,to)
                DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
                //[AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]
                DataSet dsAssignHst = assignHst.GetAllVehiclesAssignmentsBy("VehicleId", vehicleId, Convert.ToDateTime(from), Convert.ToDateTime(to), " ORDER BY AssignedDateTime ASC");
                if (dsAssignHst != null && dsAssignHst.Tables.Count > 0)
                {
                    // 3.1	For each assignment in the range of from/to datetime mearge results
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
                            currAssignTo = Convert.ToDateTime(to);

                        if (currAssignFrom.Ticks < Convert.ToDateTime(from).Ticks)
                            currAssignFrom = new DateTime(Convert.ToDateTime(from).Ticks);

                        if (currAssignTo.Ticks > Convert.ToDateTime(to).Ticks)
                            currAssignTo = new DateTime(Convert.ToDateTime(to).Ticks);

                        if (dsParams != null && dsParams.Tables.Count > 0 && dsParams.Tables[0].Rows.Count > 0)
                            currResult = GetMsgInHistoryExceptionInformationBrickman(Convert.ToInt32(ittr["BoxId"]), currAssignFrom.ToString(), currAssignTo.ToString(), userId, dsParams, ref requestOverflowed, ref totalSqlRecords);
                        else
                            currResult = GetMsgInHistoryInformationBrickman(Convert.ToInt32(ittr["BoxId"]), currAssignFrom.ToString(), currAssignTo.ToString(), userId, ref requestOverflowed, ref totalSqlRecords);
                        if (dsResult != null && currResult != null)
                            dsResult.Merge(currResult);
                    }
                    // 3.2	Add current assignment
                    if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
                    {
                        currAssignFrom = Convert.ToDateTime(dsAssign.Tables[0].Rows[0]["AssignedDateTime"]);
                        if (currAssignFrom.Ticks < Convert.ToDateTime(from).Ticks)
                            currAssignFrom = new DateTime(Convert.ToDateTime(from).Ticks);
                        if (dsParams != null && dsParams.Tables.Count > 0 && dsParams.Tables[0].Rows.Count > 0)
                            currResult = GetMsgInHistoryExceptionInformationBrickman(Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]), currAssignFrom.ToString(), to, userId, dsParams, ref requestOverflowed, ref totalSqlRecords);
                        else
                            currResult = GetMsgInHistoryInformationBrickman(Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]), currAssignFrom.ToString(), to, userId, ref requestOverflowed, ref totalSqlRecords);
                        if (dsResult != null && currResult != null)
                            dsResult.Merge(currResult);
                    }
                }
            }
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "VehicleHistory";
                    // Add VehicleId column to result dataset
                    dsResult.Tables[0].Columns.Add("VehicleId", typeof(Int64));
                    // Ad vehicle id to every row
                    foreach (DataRow ittr in dsResult.Tables[0].Rows)
                        ittr["VehicleId"] = vehicleId;
                }
                dsResult.DataSetName = "MsgInHistory";
            }
            return dsResult;
        }
        // Changes for TimeZone Feature start
        /// <summary>
        /// Returns history information from vlfMsgInHst table. 
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>
        /// DataSet [BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
        /// [BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        private DataSet GetMsgInHistoryInformation_NewTZ(int boxId,
                                         string fromDateTime,
                                         string toDateTime,
                                         int userId,
                                         ref bool requestOverflowed,
                                         ref int totalSqlRecords)
        {
            DataSet resultDataSet = null;
            requestOverflowed = false;
            string whereFilter = "";
            try
            {
                if (fromDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter = " AND vlfMsgInHst.OriginDateTime>='" + fromDateTime + "'";
                if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter += " AND vlfMsgInHst.OriginDateTime<='" + toDateTime + "'";

                whereFilter += " AND (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speed +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speeding +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.SensorExtended +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoFence +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobArm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobDisarm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobPanic +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Coordinate +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.PositionUpdate +
                    //" OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.StoredPosition +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Idling +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ExtendedIdling +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoZone +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GPSAntenna +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Status +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
                    // " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TetheredState +
                   ")";

                string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone float" +
                   " DECLARE @Unit real" +
                   " DECLARE @DayLightSaving int,@DST_Change bit" +
                   " set @DST_Change=0 " +
                    " IF dbo.fn_GetDSTOffset('" + Convert.ToDateTime(fromDateTime).ToString("MM/dd/yyyy HH:mm:ss.fff") + "')<>dbo.fn_GetDSTOffset('" + Convert.ToDateTime(toDateTime).ToString("MM/dd/yyyy HH:mm:ss.fff") + "')" +
                    " SET @DST_Change=1" +
                  " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                    //" SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.MeasurementUnits +
                   " IF @Timezone IS NULL SET @Timezone=0" +
                    //" IF @Unit IS NULL SET @Unit=1"+
                   " SET @Unit=1" + // Measurement units (km/mile) will be later converted in report
                    // " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                   " SELECT @DayLightSaving=dbo.fn_GetDSTOffset('" + Convert.ToDateTime(fromDateTime).ToString("MM/dd/yyyy HH:mm:ss.fff") + "') " +
                   " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                   " IF @DST_Change=0 SET @Timezone= @Timezone + @DayLightSaving" +
                   " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0";

                string sqlSelectFooter = "SELECT vlfMsgInHst.BoxId,DateTimeReceived,vlfBoxMsgInType.BoxMsgInTypeId," +
                    "BoxMsgInTypeName AS \"MsgType\",vlfBoxProtocolType.BoxProtocolTypeId," +
                    "BoxProtocolTypeName AS \"ProtocolType\"," +
                    "CASE WHEN @DST_Change=1 then DATEADD(minute,(@Timezone * 60) +(dbo.fn_GetDSTOffset(OriginDateTime)*60) ,OriginDateTime) ELSE DATEADD(minute,(@Timezone * 60),OriginDateTime) END AS OriginDateTime," +
                    "ValidGps,Latitude,Longitude,SensorMask,CustomProp," +
                    "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark" + "+'*'" + " END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +

                    "CASE WHEN vlfMsgInHst.Heading IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,vlfMsgInHst.Heading) END AS Heading," +
                    "CASE WHEN vlfMsgInHst.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHst.Speed * @Unit,1)) END AS Speed," +
                    "CASE WHEN vlfBox.BoxArmed=0 then 'false' ELSE 'true' END AS BoxArmed," +
                    "ISNULL(vlfMsgInHst.NearestLandmark,'" + VLF.CLS.Def.Const.addressNA + "') AS NearestLandmark," +
                    "OriginDateTime as GMT_OriginDateTime";

                string sqlFooter = "FROM vlfMsgInHst with (nolock) ,vlfBoxMsgInType, vlfBoxProtocolType, vlfBox with (nolock)" +
                   " WHERE vlfMsgInHst.BoxId=" + boxId +
                   " AND vlfMsgInHst.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId" +
                   " AND vlfMsgInHst.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " AND vlfMsgInHst.BoxId=vlfBox.BoxId" +
                   whereFilter;
                string sql = sqlHeader + " " + sqlSelectFooter + " " + sqlFooter;
                try
                {
                    int sqlTop = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Report Max Records", 1000);
                    if (sqlTop != VLF.CLS.Def.Const.unassignedIntValue)
                    {
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter));
                        if ((sqlTop - totalSqlRecords) < 0)
                        {
                            requestOverflowed = true;
                            return null;
                        }
                    }
                }
                catch (DASDbConnectionClosed exCnn)
                {
                    throw new DASDbConnectionClosed(exCnn.Message);
                }
                catch (Exception exp)
                {
                    throw new DASException("Unable to retrieve messages from history" + exp.Message);
                }
                //Executes SQL statement
                //sql += " ORDER BY vlfMsgInHst.BoxId,OriginDateTime";
                sql += " ORDER BY vlfMsgInHst.BoxId,DateTimeReceived";
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }
        // Changes for TimeZone Feature end


        /// <summary>
        /// Returns history information from vlfMsgInHst table. 
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>
        /// DataSet [BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
        /// [BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        private DataSet GetMsgInHistoryInformation(int boxId,
                                         string fromDateTime,
                                         string toDateTime,
                                         int userId,
                                         ref bool requestOverflowed,
                                         ref int totalSqlRecords)
        {
            DataSet resultDataSet = null;
            requestOverflowed = false;
            string whereFilter = "";
            try
            {
                if (fromDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter = " AND vlfMsgInHst.OriginDateTime>='" + fromDateTime + "'";
                if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter += " AND vlfMsgInHst.OriginDateTime<='" + toDateTime + "'";

                whereFilter += " AND (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speed +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speeding +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.SensorExtended +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoFence +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobArm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobDisarm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobPanic +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Coordinate +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.PositionUpdate +
                    //" OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.StoredPosition +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Idling +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ExtendedIdling +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoZone +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GPSAntenna +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Status +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
                    // " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TetheredState +
                   ")";


                string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone int" +
                    " DECLARE @Unit real" +
                    " DECLARE @DayLightSaving int" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                    //" SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.MeasurementUnits +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    //" IF @Unit IS NULL SET @Unit=1"+
                    " SET @Unit=1" + // Measurement units (km/mile) will be later converted in report
                    // " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " SELECT @DayLightSaving=dbo.fn_GetDSTOffset('" + Convert.ToDateTime(fromDateTime).ToString("MM/dd/yyyy HH:mm:ss.fff") + "') " +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0";

                string sqlSelectFooter = "SELECT vlfMsgInHst.BoxId,DateTimeReceived,vlfBoxMsgInType.BoxMsgInTypeId," +
                    "BoxMsgInTypeName AS \"MsgType\",vlfBoxProtocolType.BoxProtocolTypeId," +
                    "BoxProtocolTypeName AS \"ProtocolType\"," +
                    "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,OriginDateTime) END AS OriginDateTime," +
                    "ValidGps,Latitude,Longitude,SensorMask,CustomProp," +
                    "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark" + "+'*'" + " END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +

                    "CASE WHEN vlfMsgInHst.Heading IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,vlfMsgInHst.Heading) END AS Heading," +
                    "CASE WHEN vlfMsgInHst.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHst.Speed * @Unit,1)) END AS Speed," +
                    "CASE WHEN vlfBox.BoxArmed=0 then 'false' ELSE 'true' END AS BoxArmed," +
                    "ISNULL(vlfMsgInHst.NearestLandmark,'" + VLF.CLS.Def.Const.addressNA + "') AS NearestLandmark," +
                    "OriginDateTime as GMT_OriginDateTime";

                string sqlFooter = "FROM vlfMsgInHst with (nolock) ,vlfBoxMsgInType, vlfBoxProtocolType, vlfBox with (nolock)" +
                   " WHERE vlfMsgInHst.BoxId=" + boxId +
                   " AND vlfMsgInHst.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId" +
                   " AND vlfMsgInHst.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " AND vlfMsgInHst.BoxId=vlfBox.BoxId" +
                   whereFilter;
                string sql = sqlHeader + " " + sqlSelectFooter + " " + sqlFooter;
                try
                {
                    int sqlTop = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Report Max Records", 1000);
                    if (sqlTop != VLF.CLS.Def.Const.unassignedIntValue)
                    {
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter));
                        if ((sqlTop - totalSqlRecords) < 0)
                        {
                            requestOverflowed = true;
                            return null;
                        }
                    }
                }
                catch (DASDbConnectionClosed exCnn)
                {
                    throw new DASDbConnectionClosed(exCnn.Message);
                }
                catch (Exception exp)
                {
                    throw new DASException("Unable to retrieve messages from history" + exp.Message);
                }
                //Executes SQL statement
                //sql += " ORDER BY vlfMsgInHst.BoxId,OriginDateTime";
                sql += " ORDER BY vlfMsgInHst.BoxId,DateTimeReceived";
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Returns history information from vlfMsgInHst table. 
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>
        /// DataSet [BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
        /// [BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        private DataSet GetMsgInHistoryInformationBrickman_NewTZ(int boxId,
                                         string fromDateTime,
                                         string toDateTime,
                                         int userId,
                                         ref bool requestOverflowed,
                                         ref int totalSqlRecords)
        {
            DataSet resultDataSet = null;
            requestOverflowed = false;
            string whereFilter = "";
            try
            {
                if (fromDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter = " AND vlfMsgInHstBrickmanAUGUST.OriginDateTime>='" + fromDateTime + "'";
                if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter += " AND vlfMsgInHstBrickmanAUGUST.OriginDateTime<='" + toDateTime + "'";

                whereFilter += " AND (vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speed +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speeding +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.SensorExtended +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoFence +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobArm +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobDisarm +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobPanic +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Coordinate +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.PositionUpdate +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.StoredPosition +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Idling +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ExtendedIdling +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoZone +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GPSAntenna +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Status +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.TetheredState +
                   ")";

                //Prepares SQL statement               

                string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone float" +
                   " DECLARE @Unit real" +
                   " DECLARE @DayLightSaving int" +
                   " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                   " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                    //" SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.MeasurementUnits +
                   " IF @Timezone IS NULL SET @Timezone=0" +
                    //" IF @Unit IS NULL SET @Unit=1"+
                   " SET @Unit=1" + // Measurement units (km/mile) will be later converted in report
                   " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                   " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                   " SET @Timezone= @Timezone + @DayLightSaving" +
                   " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0";

                string sqlSelectFooter = "SELECT DISTINCT vlfMsgInHstBrickmanAUGUST.BoxId,DateTimeReceived,vlfBoxMsgInType.BoxMsgInTypeId," +
                    "BoxMsgInTypeName AS \"MsgType\",vlfBoxProtocolType.BoxProtocolTypeId," +
                    "BoxProtocolTypeName AS \"ProtocolType\"," +
                    "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),OriginDateTime) END AS OriginDateTime," +
                    "ValidGps,Latitude,Longitude,SensorMask,CustomProp," +
                    "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHstBrickmanAUGUST.StreetAddress ELSE CASE WHEN vlfMsgInHstBrickmanAUGUST.NearestLandmark IS NULL then vlfMsgInHstBrickmanAUGUST.StreetAddress ELSE vlfMsgInHstBrickmanAUGUST.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +

                    "CASE WHEN vlfMsgInHstBrickmanAUGUST.Heading IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,vlfMsgInHstBrickmanAUGUST.Heading) END AS Heading," +
                    "CASE WHEN vlfMsgInHstBrickmanAUGUST.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHstBrickmanAUGUST.Speed * @Unit,1)) END AS Speed," +
                    "CASE WHEN vlfBox.BoxArmed=0 then 'false' ELSE 'true' END AS BoxArmed," +
                    "ISNULL(vlfMsgInHstBrickmanAUGUST.NearestLandmark,'" + VLF.CLS.Def.Const.addressNA + "') AS NearestLandmark";

                string sqlFooter = "FROM vlfMsgInHstBrickmanAUGUST with (nolock) ,vlfBoxMsgInType, vlfBoxProtocolType, vlfBox with (nolock)" +
                   " WHERE vlfMsgInHstBrickmanAUGUST.BoxId=" + boxId +
                   " AND vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId" +
                   " AND vlfMsgInHstBrickmanAUGUST.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " AND vlfMsgInHstBrickmanAUGUST.BoxId=vlfBox.BoxId" +
                   whereFilter;
                string sql = sqlHeader + " " + sqlSelectFooter + " " + sqlFooter;
                try
                {
                    int sqlTop = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Report Max Records", 1000);
                    if (sqlTop != VLF.CLS.Def.Const.unassignedIntValue)
                    {
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter));
                        if ((sqlTop - totalSqlRecords) < 0)
                        {
                            requestOverflowed = true;
                            return null;
                        }
                    }
                }
                catch (DASDbConnectionClosed exCnn)
                {
                    throw new DASDbConnectionClosed(exCnn.Message);
                }
                catch (Exception exp)
                {
                    throw new DASException("Unable to retrieve messages from history" + exp.Message);
                }
                //Executes SQL statement
                sql += " ORDER BY vlfMsgInHstBrickmanAUGUST.BoxId,OriginDateTime,vlfMsgInHstBrickmanAUGUST.DateTimeReceived";
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }
        // Changes for TimeZone Feature end


        /// <summary>
        /// Returns history information from vlfMsgInHst table. 
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>
        /// DataSet [BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
        /// [BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        private DataSet GetMsgInHistoryInformationBrickman(int boxId,
                                         string fromDateTime,
                                         string toDateTime,
                                         int userId,
                                         ref bool requestOverflowed,
                                         ref int totalSqlRecords)
        {
            DataSet resultDataSet = null;
            requestOverflowed = false;
            string whereFilter = "";
            try
            {
                if (fromDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter = " AND vlfMsgInHstBrickmanAUGUST.OriginDateTime>='" + fromDateTime + "'";
                if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter += " AND vlfMsgInHstBrickmanAUGUST.OriginDateTime<='" + toDateTime + "'";

                whereFilter += " AND (vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speed +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speeding +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.SensorExtended +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoFence +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobArm +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobDisarm +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobPanic +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Coordinate +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.PositionUpdate +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.StoredPosition +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Idling +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ExtendedIdling +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoZone +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GPSAntenna +
                   " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Status +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
                    " OR vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId=" + (short)Enums.MessageType.TetheredState +
                   ")";


                string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone int" +
                    " DECLARE @Unit real" +
                    " DECLARE @DayLightSaving int" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                    //" SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.MeasurementUnits +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    //" IF @Unit IS NULL SET @Unit=1"+
                    " SET @Unit=1" + // Measurement units (km/mile) will be later converted in report
                    " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0";

                string sqlSelectFooter = "SELECT DISTINCT vlfMsgInHstBrickmanAUGUST.BoxId,DateTimeReceived,vlfBoxMsgInType.BoxMsgInTypeId," +
                    "BoxMsgInTypeName AS \"MsgType\",vlfBoxProtocolType.BoxProtocolTypeId," +
                    "BoxProtocolTypeName AS \"ProtocolType\"," +
                    "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,OriginDateTime) END AS OriginDateTime," +
                    "ValidGps,Latitude,Longitude,SensorMask,CustomProp," +
                    "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHstBrickmanAUGUST.StreetAddress ELSE CASE WHEN vlfMsgInHstBrickmanAUGUST.NearestLandmark IS NULL then vlfMsgInHstBrickmanAUGUST.StreetAddress ELSE vlfMsgInHstBrickmanAUGUST.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +

                    "CASE WHEN vlfMsgInHstBrickmanAUGUST.Heading IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,vlfMsgInHstBrickmanAUGUST.Heading) END AS Heading," +
                    "CASE WHEN vlfMsgInHstBrickmanAUGUST.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHstBrickmanAUGUST.Speed * @Unit,1)) END AS Speed," +
                    "CASE WHEN vlfBox.BoxArmed=0 then 'false' ELSE 'true' END AS BoxArmed," +
                    "ISNULL(vlfMsgInHstBrickmanAUGUST.NearestLandmark,'" + VLF.CLS.Def.Const.addressNA + "') AS NearestLandmark";

                string sqlFooter = "FROM vlfMsgInHstBrickmanAUGUST with (nolock) ,vlfBoxMsgInType, vlfBoxProtocolType, vlfBox with (nolock)" +
                   " WHERE vlfMsgInHstBrickmanAUGUST.BoxId=" + boxId +
                   " AND vlfMsgInHstBrickmanAUGUST.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId" +
                   " AND vlfMsgInHstBrickmanAUGUST.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " AND vlfMsgInHstBrickmanAUGUST.BoxId=vlfBox.BoxId" +
                   whereFilter;
                string sql = sqlHeader + " " + sqlSelectFooter + " " + sqlFooter;
                try
                {
                    int sqlTop = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Report Max Records", 1000);
                    if (sqlTop != VLF.CLS.Def.Const.unassignedIntValue)
                    {
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter));
                        if ((sqlTop - totalSqlRecords) < 0)
                        {
                            requestOverflowed = true;
                            return null;
                        }
                    }
                }
                catch (DASDbConnectionClosed exCnn)
                {
                    throw new DASDbConnectionClosed(exCnn.Message);
                }
                catch (Exception exp)
                {
                    throw new DASException("Unable to retrieve messages from history" + exp.Message);
                }
                //Executes SQL statement
                sql += " ORDER BY vlfMsgInHstBrickmanAUGUST.BoxId,OriginDateTime,vlfMsgInHstBrickmanAUGUST.DateTimeReceived";
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Returns history information from vlfMsgInHst table. 
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="dsParams"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>
        /// DataSet [BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
        /// [BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        private DataSet GetMsgInHistoryExceptionInformation_NewTZ(int boxId,
                                               string fromDateTime,
                                               string toDateTime,
                                               int userId,
                                               DataSet dsParams,
                                               ref bool requestOverflowed,
                                               ref int totalSqlRecords)
        {
            DataSet resultDataSet = null;
            requestOverflowed = false;
            string whereFilter = "";
            try
            {
                if (fromDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter = " AND vlfMsgInHst.OriginDateTime>='" + fromDateTime + "'";
                if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter += " AND vlfMsgInHst.OriginDateTime<='" + toDateTime + "'";

                string addFilter = "";
                if (dsParams != null && dsParams.Tables.Count > 0 && dsParams.Tables[0].Rows.Count > 0)
                {
                    #region Sos Limit
                    if (Convert.ToInt16(dsParams.Tables[0].Rows[0]["SosLimit"]) > 0)
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=18;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=18;%')";
                    }
                    #endregion
                    #region NoDoorSnsHrs
                    if (Convert.ToInt16(dsParams.Tables[0].Rows[0]["NoDoorSnsHrs"]) > 0)
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=1;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=9;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=10;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=11;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=12;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=19;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=20;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=21;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=22;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=23;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=24;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=33;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=34;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=35;%')";

                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=1;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=9;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=10;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=11;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=12;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=19;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=20;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=21;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=22;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=23;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=24;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=33;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=34;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=35;%')";
                    }
                    #endregion
                    #region TAR
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["IncludeTar"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=40;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=40;%')";
                    }
                    #endregion
                    #region IncludeMobilize
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["IncludeMobilize"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=39;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=39;%')";
                    }
                    #endregion
                    #region FifteenSecDoorSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["FifteenSecDoorSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=36;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=37;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=36;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=37;%')";
                    }
                    #endregion
                    #region Leash50
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["Leash50"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=26;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=26;%')";
                    }
                    #endregion
                    #region MainAndBackupBatterySns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["MainAndBackupBatterySns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=4;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=5;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=4;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=5;%')";
                    }
                    #endregion
                    #region TamperSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["TamperSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=13;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=13;%')";
                    }
                    #endregion
                    #region ThreeKeypadAttemptsSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["ThreeKeypadAttemptsSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=25;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=25;%')";
                    }
                    #endregion
                    #region AltGPSAntennaSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["AltGPSAntennaSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=38;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=38;%')";
                    }
                    #endregion
                    #region LeashBrokenSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["LeashBrokenSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=27;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=27;%')";
                    }
                    #endregion
                    #region AnyPanicSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["AnyPanicSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=15;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=16;%')";

                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=15;%')" +
                                     " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=16;%')";

                        addFilter += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobPanic;
                    }
                    #endregion
                    #region ControllerStatus
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["ControllerStatus"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ControllerStatus;
                        else
                            addFilter += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ControllerStatus;
                    }
                    #endregion
                    if (addFilter != "")
                        whereFilter += addFilter + ")";
                    else
                        return resultDataSet; // In case of empty filter exit with no data
                }

                string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone float" +
                   " DECLARE @Unit real" +
                   " DECLARE @DayLightSaving int" +
                   " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                   " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                   " IF @Timezone IS NULL SET @Timezone=0" +
                   " SET @Unit=1" + // Measurement units (km/mile) will be later converted in report
                   " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                   " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                   " SET @Timezone= @Timezone + @DayLightSaving" +
                   " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0";


                string sqlSelectFooter = "SELECT  vlfMsgInHst.BoxId,DateTimeReceived,vlfBoxMsgInType.BoxMsgInTypeId," +
                   "BoxMsgInTypeName AS \"MsgType\",vlfBoxProtocolType.BoxProtocolTypeId," +
                   "BoxProtocolTypeName AS \"ProtocolType\"," +
                   "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),OriginDateTime) END AS OriginDateTime," +
                   "ValidGps,Latitude,Longitude,SensorMask,CustomProp," +
                        "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark" + "+'*'" + " END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +

               "CASE WHEN vlfMsgInHst.Heading IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,vlfMsgInHst.Heading) END AS Heading," +
               "CASE WHEN vlfMsgInHst.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHst.Speed * @Unit,1)) END AS Speed," +
               "CASE WHEN vlfBox.BoxArmed=0 then 'false' ELSE 'true' END AS BoxArmed";
                string sqlFooter = "FROM vlfMsgInHst with (nolock) ,vlfBoxMsgInType,vlfBoxProtocolType,vlfBox with (nolock)" +
                   " WHERE vlfMsgInHst.BoxId=" + boxId +
                   " AND vlfMsgInHst.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId" +
                   " AND vlfMsgInHst.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " AND vlfMsgInHst.BoxId=vlfBox.BoxId" +
                   whereFilter;
                string sql = sqlHeader + " " + sqlSelectFooter + " " + sqlFooter;
                try
                {
                    int sqlTop = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Report Max Records", 1000);
                    if (sqlTop != VLF.CLS.Def.Const.unassignedIntValue)
                    {
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter));
                        if ((sqlTop - totalSqlRecords) < 0)
                        {
                            requestOverflowed = true;
                            return null;
                        }
                    }
                }
                catch (DASDbConnectionClosed exCnn)
                {
                    throw new DASDbConnectionClosed(exCnn.Message);
                }
                catch (Exception exp)
                {
                    throw new DASException("Unable to retrieve messages from history" + exp.Message);
                }
                //Executes SQL statement
                sql += " ORDER BY vlfMsgInHst.BoxId,OriginDateTime";
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Returns history information from vlfMsgInHst table. 
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="dsParams"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>
        /// DataSet [BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
        /// [BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        private DataSet GetMsgInHistoryExceptionInformation(int boxId,
                                               string fromDateTime,
                                               string toDateTime,
                                               int userId,
                                               DataSet dsParams,
                                               ref bool requestOverflowed,
                                               ref int totalSqlRecords)
        {
            DataSet resultDataSet = null;
            requestOverflowed = false;
            string whereFilter = "";
            try
            {
                if (fromDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter = " AND vlfMsgInHst.OriginDateTime>='" + fromDateTime + "'";
                if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter += " AND vlfMsgInHst.OriginDateTime<='" + toDateTime + "'";

                string addFilter = "";
                if (dsParams != null && dsParams.Tables.Count > 0 && dsParams.Tables[0].Rows.Count > 0)
                {
                    #region Sos Limit
                    if (Convert.ToInt16(dsParams.Tables[0].Rows[0]["SosLimit"]) > 0)
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=18;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=18;%')";
                    }
                    #endregion
                    #region NoDoorSnsHrs
                    if (Convert.ToInt16(dsParams.Tables[0].Rows[0]["NoDoorSnsHrs"]) > 0)
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=1;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=9;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=10;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=11;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=12;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=19;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=20;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=21;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=22;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=23;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=24;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=33;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=34;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=35;%')";

                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=1;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=9;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=10;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=11;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=12;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=19;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=20;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=21;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=22;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=23;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=24;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=33;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=34;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=35;%')";
                    }
                    #endregion
                    #region TAR
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["IncludeTar"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=40;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=40;%')";
                    }
                    #endregion
                    #region IncludeMobilize
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["IncludeMobilize"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=39;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=39;%')";
                    }
                    #endregion
                    #region FifteenSecDoorSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["FifteenSecDoorSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=36;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=37;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=36;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=37;%')";
                    }
                    #endregion
                    #region Leash50
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["Leash50"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=26;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=26;%')";
                    }
                    #endregion
                    #region MainAndBackupBatterySns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["MainAndBackupBatterySns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=4;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=5;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=4;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=5;%')";
                    }
                    #endregion
                    #region TamperSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["TamperSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=13;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=13;%')";
                    }
                    #endregion
                    #region ThreeKeypadAttemptsSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["ThreeKeypadAttemptsSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=25;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=25;%')";
                    }
                    #endregion
                    #region AltGPSAntennaSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["AltGPSAntennaSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=38;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=38;%')";
                    }
                    #endregion
                    #region LeashBrokenSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["LeashBrokenSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=27;%')";
                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=27;%')";
                    }
                    #endregion
                    #region AnyPanicSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["AnyPanicSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=15;%')" +
                               " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=16;%')";

                        else
                            addFilter += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=15;%')" +
                                     " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHst.CustomProp LIKE 'SENSOR_NUM=16;%')";

                        addFilter += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobPanic;
                    }
                    #endregion
                    #region ControllerStatus
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["ControllerStatus"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ControllerStatus;
                        else
                            addFilter += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ControllerStatus;
                    }
                    #endregion
                    if (addFilter != "")
                        whereFilter += addFilter + ")";
                    else
                        return resultDataSet; // In case of empty filter exit with no data
                }

                string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone int" +
                    " DECLARE @Unit real" +
                    " DECLARE @DayLightSaving int" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SET @Unit=1" + // Measurement units (km/mile) will be later converted in report
                    " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0";

                string sqlSelectFooter = "SELECT  vlfMsgInHst.BoxId,DateTimeReceived,vlfBoxMsgInType.BoxMsgInTypeId," +
                   "BoxMsgInTypeName AS \"MsgType\",vlfBoxProtocolType.BoxProtocolTypeId," +
                   "BoxProtocolTypeName AS \"ProtocolType\"," +
                   "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,OriginDateTime) END AS OriginDateTime," +
                   "ValidGps,Latitude,Longitude,SensorMask,CustomProp," +
                        "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark" + "+'*'" + " END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +

               "CASE WHEN vlfMsgInHst.Heading IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,vlfMsgInHst.Heading) END AS Heading," +
               "CASE WHEN vlfMsgInHst.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHst.Speed * @Unit,1)) END AS Speed," +
               "CASE WHEN vlfBox.BoxArmed=0 then 'false' ELSE 'true' END AS BoxArmed";
                string sqlFooter = "FROM vlfMsgInHst with (nolock) ,vlfBoxMsgInType,vlfBoxProtocolType,vlfBox with (nolock)" +
                   " WHERE vlfMsgInHst.BoxId=" + boxId +
                   " AND vlfMsgInHst.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId" +
                   " AND vlfMsgInHst.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " AND vlfMsgInHst.BoxId=vlfBox.BoxId" +
                   whereFilter;
                string sql = sqlHeader + " " + sqlSelectFooter + " " + sqlFooter;
                try
                {
                    int sqlTop = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Report Max Records", 1000);
                    if (sqlTop != VLF.CLS.Def.Const.unassignedIntValue)
                    {
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter));
                        if ((sqlTop - totalSqlRecords) < 0)
                        {
                            requestOverflowed = true;
                            return null;
                        }
                    }
                }
                catch (DASDbConnectionClosed exCnn)
                {
                    throw new DASDbConnectionClosed(exCnn.Message);
                }
                catch (Exception exp)
                {
                    throw new DASException("Unable to retrieve messages from history" + exp.Message);
                }
                //Executes SQL statement
                sql += " ORDER BY vlfMsgInHst.BoxId,OriginDateTime";
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }


        // Changes for TimeZone Feature start
        /// <summary>
        /// Returns history information from vlfMsgInHst table. 
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="dsParams"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>
        /// DataSet [BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
        /// [BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        private DataSet GetMsgInHistoryExceptionInformationBrickman_NewTZ(int boxId,
                                               string fromDateTime,
                                               string toDateTime,
                                               int userId,
                                               DataSet dsParams,
                                               ref bool requestOverflowed,
                                               ref int totalSqlRecords)
        {
            DataSet resultDataSet = null;
            requestOverflowed = false;
            string whereFilter = "";
            try
            {
                if (fromDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter = " AND vlfMsgInHstBrickmanTEST.OriginDateTime>='" + fromDateTime + "'";
                if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter += " AND vlfMsgInHstBrickmanTEST.OriginDateTime<='" + toDateTime + "'";

                string addFilter = "";
                if (dsParams != null && dsParams.Tables.Count > 0 && dsParams.Tables[0].Rows.Count > 0)
                {
                    #region Sos Limit
                    if (Convert.ToInt16(dsParams.Tables[0].Rows[0]["SosLimit"]) > 0)
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=18;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=18;%')";
                    }
                    #endregion
                    #region NoDoorSnsHrs
                    if (Convert.ToInt16(dsParams.Tables[0].Rows[0]["NoDoorSnsHrs"]) > 0)
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=1;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=9;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=10;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=11;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=12;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=19;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=20;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=21;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=22;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=23;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=24;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=33;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=34;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=35;%')";

                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=1;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=9;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=10;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=11;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=12;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=19;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=20;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=21;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=22;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=23;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=24;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=33;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=34;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=35;%')";
                    }
                    #endregion
                    #region TAR
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["IncludeTar"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=40;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=40;%')";
                    }
                    #endregion
                    #region IncludeMobilize
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["IncludeMobilize"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=39;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=39;%')";
                    }
                    #endregion
                    #region FifteenSecDoorSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["FifteenSecDoorSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=36;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=37;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=36;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=37;%')";
                    }
                    #endregion
                    #region Leash50
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["Leash50"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=26;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=26;%')";
                    }
                    #endregion
                    #region MainAndBackupBatterySns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["MainAndBackupBatterySns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=4;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=5;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=4;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=5;%')";
                    }
                    #endregion
                    #region TamperSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["TamperSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=13;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=13;%')";
                    }
                    #endregion
                    #region ThreeKeypadAttemptsSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["ThreeKeypadAttemptsSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=25;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=25;%')";
                    }
                    #endregion
                    #region AltGPSAntennaSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["AltGPSAntennaSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=38;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=38;%')";
                    }
                    #endregion
                    #region LeashBrokenSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["LeashBrokenSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=27;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=27;%')";
                    }
                    #endregion
                    #region AnyPanicSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["AnyPanicSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=15;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=16;%')";

                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=15;%')" +
                                     " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=16;%')";

                        addFilter += " OR vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobPanic;
                    }
                    #endregion
                    #region ControllerStatus
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["ControllerStatus"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ControllerStatus;
                        else
                            addFilter += " OR vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ControllerStatus;
                    }
                    #endregion
                    if (addFilter != "")
                        whereFilter += addFilter + ")";
                    else
                        return resultDataSet; // In case of empty filter exit with no data
                }

                string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone float" +
                    " DECLARE @Unit real" +
                    " DECLARE @DayLightSaving int" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SET @Unit=1" + // Measurement units (km/mile) will be later converted in report
                    " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0";

                string sqlSelectFooter = "SELECT DISTINCT vlfMsgInHstBrickmanTEST.BoxId,DateTimeReceived,vlfBoxMsgInType.BoxMsgInTypeId," +
                   "BoxMsgInTypeName AS \"MsgType\",vlfBoxProtocolType.BoxProtocolTypeId," +
                   "BoxProtocolTypeName AS \"ProtocolType\"," +
                   "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),OriginDateTime) END AS OriginDateTime," +
                   "ValidGps,Latitude,Longitude,SensorMask,CustomProp," +
                        "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHstBrickmanTEST.StreetAddress ELSE CASE WHEN vlfMsgInHstBrickmanTEST.NearestLandmark IS NULL then vlfMsgInHstBrickmanTEST.StreetAddress ELSE vlfMsgInHstBrickmanTEST.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +

               "CASE WHEN vlfMsgInHstBrickmanTEST.Heading IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,vlfMsgInHstBrickmanTEST.Heading) END AS Heading," +
               "CASE WHEN vlfMsgInHstBrickmanTEST.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHstBrickmanTEST.Speed * @Unit,1)) END AS Speed," +
               "CASE WHEN vlfBox.BoxArmed=0 then 'false' ELSE 'true' END AS BoxArmed";
                string sqlFooter = "FROM vlfMsgInHstBrickmanTEST with (nolock) ,vlfBoxMsgInType,vlfBoxProtocolType,vlfBox with (nolock)" +
                   " WHERE vlfMsgInHstBrickmanTEST.BoxId=" + boxId +
                   " AND vlfMsgInHstBrickmanTEST.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId" +
                   " AND vlfMsgInHstBrickmanTEST.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " AND vlfMsgInHstBrickmanTEST.BoxId=vlfBox.BoxId" +
                   whereFilter;
                string sql = sqlHeader + " " + sqlSelectFooter + " " + sqlFooter;
                try
                {
                    int sqlTop = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Report Max Records", 1000);
                    if (sqlTop != VLF.CLS.Def.Const.unassignedIntValue)
                    {
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter));
                        if ((sqlTop - totalSqlRecords) < 0)
                        {
                            requestOverflowed = true;
                            return null;
                        }
                    }
                }
                catch (DASDbConnectionClosed exCnn)
                {
                    throw new DASDbConnectionClosed(exCnn.Message);
                }
                catch (Exception exp)
                {
                    throw new DASException("Unable to retrieve messages from history" + exp.Message);
                }
                //Executes SQL statement
                sql += " ORDER BY vlfMsgInHstBrickmanTEST.BoxId,OriginDateTime,vlfMsgInHstBrickmanTEST.DateTimeReceived";
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }
        // Changes for TimeZone Feature end


        /// <summary>
        /// Returns history information from vlfMsgInHst table. 
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="dsParams"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>
        /// DataSet [BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
        /// [BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        private DataSet GetMsgInHistoryExceptionInformationBrickman(int boxId,
                                               string fromDateTime,
                                               string toDateTime,
                                               int userId,
                                               DataSet dsParams,
                                               ref bool requestOverflowed,
                                               ref int totalSqlRecords)
        {
            DataSet resultDataSet = null;
            requestOverflowed = false;
            string whereFilter = "";
            try
            {
                if (fromDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter = " AND vlfMsgInHstBrickmanTEST.OriginDateTime>='" + fromDateTime + "'";
                if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter += " AND vlfMsgInHstBrickmanTEST.OriginDateTime<='" + toDateTime + "'";

                string addFilter = "";
                if (dsParams != null && dsParams.Tables.Count > 0 && dsParams.Tables[0].Rows.Count > 0)
                {
                    #region Sos Limit
                    if (Convert.ToInt16(dsParams.Tables[0].Rows[0]["SosLimit"]) > 0)
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=18;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=18;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=18;%')";
                    }
                    #endregion
                    #region NoDoorSnsHrs
                    if (Convert.ToInt16(dsParams.Tables[0].Rows[0]["NoDoorSnsHrs"]) > 0)
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=1;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=9;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=10;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=11;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=12;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=19;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=20;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=21;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=22;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=23;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=24;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=33;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=34;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=35;%')";

                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=1;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=1;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=9;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=9;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=10;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=10;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=11;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=11;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=12;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=12;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=19;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=19;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=20;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=20;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=21;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=21;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=22;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=22;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=23;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=23;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=24;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=24;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=33;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=33;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=34;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=34;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=35;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=35;%')";
                    }
                    #endregion
                    #region TAR
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["IncludeTar"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=40;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=40;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=40;%')";
                    }
                    #endregion
                    #region IncludeMobilize
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["IncludeMobilize"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=39;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=39;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=39;%')";
                    }
                    #endregion
                    #region FifteenSecDoorSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["FifteenSecDoorSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=36;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=37;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=36;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=36;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=37;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=37;%')";
                    }
                    #endregion
                    #region Leash50
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["Leash50"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=26;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=26;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=26;%')";
                    }
                    #endregion
                    #region MainAndBackupBatterySns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["MainAndBackupBatterySns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=4;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=5;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=4;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=4;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=5;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=5;%')";
                    }
                    #endregion
                    #region TamperSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["TamperSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=13;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=13;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=13;%')";
                    }
                    #endregion
                    #region ThreeKeypadAttemptsSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["ThreeKeypadAttemptsSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=25;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=25;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=25;%')";
                    }
                    #endregion
                    #region AltGPSAntennaSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["AltGPSAntennaSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=38;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=38;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=38;%')";
                    }
                    #endregion
                    #region LeashBrokenSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["LeashBrokenSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=27;%')";
                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=27;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=27;%')";
                    }
                    #endregion
                    #region AnyPanicSns
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["AnyPanicSns"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND ((vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=15;%')" +
                               " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=16;%')";

                        else
                            addFilter += " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=15;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=15;%')" +
                                     " OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=16;%') OR (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm + " AND vlfMsgInHstBrickmanTEST.CustomProp LIKE 'SENSOR_NUM=16;%')";

                        addFilter += " OR vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobPanic;
                    }
                    #endregion
                    #region ControllerStatus
                    if (Convert.ToBoolean(dsParams.Tables[0].Rows[0]["ControllerStatus"]))
                    {
                        if (addFilter == "")
                            addFilter = " AND (vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ControllerStatus;
                        else
                            addFilter += " OR vlfMsgInHstBrickmanTEST.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ControllerStatus;
                    }
                    #endregion
                    if (addFilter != "")
                        whereFilter += addFilter + ")";
                    else
                        return resultDataSet; // In case of empty filter exit with no data
                }

                string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone int" +
                    " DECLARE @Unit real" +
                    " DECLARE @DayLightSaving int" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SET @Unit=1" + // Measurement units (km/mile) will be later converted in report
                    " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0";

                string sqlSelectFooter = "SELECT DISTINCT vlfMsgInHstBrickmanTEST.BoxId,DateTimeReceived,vlfBoxMsgInType.BoxMsgInTypeId," +
                   "BoxMsgInTypeName AS \"MsgType\",vlfBoxProtocolType.BoxProtocolTypeId," +
                   "BoxProtocolTypeName AS \"ProtocolType\"," +
                   "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,OriginDateTime) END AS OriginDateTime," +
                   "ValidGps,Latitude,Longitude,SensorMask,CustomProp," +
                        "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHstBrickmanTEST.StreetAddress ELSE CASE WHEN vlfMsgInHstBrickmanTEST.NearestLandmark IS NULL then vlfMsgInHstBrickmanTEST.StreetAddress ELSE vlfMsgInHstBrickmanTEST.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +

               "CASE WHEN vlfMsgInHstBrickmanTEST.Heading IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,vlfMsgInHstBrickmanTEST.Heading) END AS Heading," +
               "CASE WHEN vlfMsgInHstBrickmanTEST.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHstBrickmanTEST.Speed * @Unit,1)) END AS Speed," +
               "CASE WHEN vlfBox.BoxArmed=0 then 'false' ELSE 'true' END AS BoxArmed";
                string sqlFooter = "FROM vlfMsgInHstBrickmanTEST with (nolock) ,vlfBoxMsgInType,vlfBoxProtocolType,vlfBox with (nolock)" +
                   " WHERE vlfMsgInHstBrickmanTEST.BoxId=" + boxId +
                   " AND vlfMsgInHstBrickmanTEST.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId" +
                   " AND vlfMsgInHstBrickmanTEST.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " AND vlfMsgInHstBrickmanTEST.BoxId=vlfBox.BoxId" +
                   whereFilter;
                string sql = sqlHeader + " " + sqlSelectFooter + " " + sqlFooter;
                try
                {
                    int sqlTop = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Report Max Records", 1000);
                    if (sqlTop != VLF.CLS.Def.Const.unassignedIntValue)
                    {
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter));
                        if ((sqlTop - totalSqlRecords) < 0)
                        {
                            requestOverflowed = true;
                            return null;
                        }
                    }
                }
                catch (DASDbConnectionClosed exCnn)
                {
                    throw new DASDbConnectionClosed(exCnn.Message);
                }
                catch (Exception exp)
                {
                    throw new DASException("Unable to retrieve messages from history" + exp.Message);
                }
                //Executes SQL statement
                sql += " ORDER BY vlfMsgInHstBrickmanTEST.BoxId,OriginDateTime,vlfMsgInHstBrickmanTEST.DateTimeReceived";
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by box Id =" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }
        /// <summary>
        /// Returns Latency information from vlfMsgInHst table. 
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
        public DataSet GetVehicleLatencyInformation(Int64 vehicleId, string fromDateTime, string toDateTime, DataSet dsParams)
        {
            DataSet resultDataSet = null;
            string whereFilter = "";
            try
            {
                if (fromDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                {
                    whereFilter = " vlfMsgInHst.OriginDateTime>='" + fromDateTime + "'";
                    if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                        whereFilter += " AND vlfMsgInHst.OriginDateTime<='" + toDateTime + "'";
                }
                else if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter += " vlfMsgInHst.OriginDateTime<='" + toDateTime + "'";


                if (whereFilter != "")
                    whereFilter += " AND";
                #region Filter by messages
                whereFilter += " (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.PositionUpdate +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Coordinate +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.SensorExtended +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoFence +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoZone +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GPSAntenna +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speed +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobArm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobDisarm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobPanic +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.StoredPosition +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Idling +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ExtendedIdling +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speeding +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Status +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.LowBattery +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.AuxInputOneOn +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.AuxInputTwoOn +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MDTAck +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MDTResponse +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MDTTextMessage +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
                    " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TetheredState +
                   ")";
                #endregion
                #region Filter by commModes
                if (dsParams != null && dsParams.Tables.Count > 0 && dsParams.Tables[0].Rows.Count > 0)
                {
                    if (whereFilter != "")
                        whereFilter += " AND ";
                    whereFilter += "(";
                    for (int index = 0; index < dsParams.Tables[0].Rows.Count; ++index)
                    {
                        if (index == 0)
                            whereFilter += "vlfCommMode.CommModeId=" + dsParams.Tables[0].Rows[index]["CommModeId"].ToString();
                        else
                            whereFilter += " OR vlfCommMode.CommModeId=" + dsParams.Tables[0].Rows[index]["CommModeId"].ToString();
                    }
                    whereFilter += ")";
                }
                #endregion

                if (whereFilter != "")
                    whereFilter = " WHERE " + whereFilter;
                string sql = "SELECT vlfOrganization.OrganizationName, vlfVehicleInfo.Description, vlfMsgInHst.BoxId, vlfCommMode.CommModeId,vlfCommMode.CommModeName, COUNT(*) AS NumOfMsgs, AVG(ABS(DATEDIFF(ss, vlfMsgInHst.OriginDateTime, vlfMsgInHst.DateTimeReceived))) AS DiffInSec FROM vlfMsgInHst with (nolock) INNER JOIN vlfFirmwareChannelReference INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannelReference.FwChId = vlfBox.FwChId ON vlfMsgInHst.BoxId = vlfBox.BoxId INNER JOIN vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId AND vlfMsgInHst.BoxProtocolTypeId = vlfChannels.BoxProtocolTypeId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN vlfOrganization ON vlfVehicleInfo.OrganizationId = vlfOrganization.OrganizationId" + whereFilter + " GROUP BY vlfMsgInHst.BoxId, vlfCommMode.CommModeId, vlfCommMode.CommModeName, vlfVehicleInfo.Description, vlfOrganization.OrganizationName, vlfVehicleInfo.VehicleId HAVING vlfVehicleInfo.VehicleId=" + vehicleId;
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by vehicleId =" + vehicleId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by vehicleId =" + vehicleId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }
        /// <summary>
        /// Returns Latency information from vlfMsgInHst table. 
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
            DataSet resultDataSet = null;
            string whereFilter = "";
            try
            {
                if (fromDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                {
                    whereFilter = " vlfMsgInHst.OriginDateTime>='" + fromDateTime + "'";
                    if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                        whereFilter += " AND vlfMsgInHst.OriginDateTime<='" + toDateTime + "'";
                }
                else if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter += " vlfMsgInHst.OriginDateTime<='" + toDateTime + "'";


                if (whereFilter != "")
                    whereFilter += " AND";
                #region Filter by messages
                whereFilter += " (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.PositionUpdate +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Coordinate +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.SensorExtended +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoFence +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoZone +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GPSAntenna +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speed +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobArm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobDisarm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobPanic +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.StoredPosition +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Idling +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ExtendedIdling +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speeding +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Status +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.LowBattery +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.AuxInputOneOn +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.AuxInputTwoOn +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MDTAck +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MDTResponse +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MDTTextMessage +
                   ")";
                #endregion
                #region Filter by commModes
                if (dsParams != null && dsParams.Tables.Count > 0 && dsParams.Tables[0].Rows.Count > 0)
                {
                    if (whereFilter != "")
                        whereFilter += " AND ";
                    whereFilter += "(";
                    for (int index = 0; index < dsParams.Tables[0].Rows.Count; ++index)
                    {
                        if (index == 0)
                            whereFilter += "vlfCommMode.CommModeId=" + dsParams.Tables[0].Rows[index]["CommModeId"].ToString();
                        else
                            whereFilter += " OR vlfCommMode.CommModeId=" + dsParams.Tables[0].Rows[index]["CommModeId"].ToString();
                    }
                    whereFilter += ")";
                }
                #endregion

                if (whereFilter != "")
                    whereFilter = " WHERE " + whereFilter;
                string sql = "SELECT vlfOrganization.OrganizationName, vlfVehicleInfo.Description, vlfMsgInHst.BoxId, vlfCommMode.CommModeId, vlfCommMode.CommModeName, COUNT(*) AS NumOfMsgs, AVG(ABS(DATEDIFF(ss, vlfMsgInHst.OriginDateTime, vlfMsgInHst.DateTimeReceived))) AS DiffInSec FROM vlfMsgInHst with (nolock) INNER JOIN vlfFirmwareChannelReference INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannelReference.FwChId = vlfBox.FwChId ON vlfMsgInHst.BoxId = vlfBox.BoxId INNER JOIN vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId AND vlfMsgInHst.BoxProtocolTypeId = vlfChannels.BoxProtocolTypeId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN vlfOrganization ON vlfVehicleInfo.OrganizationId = vlfOrganization.OrganizationId INNER JOIN vlfFleetVehicles ON vlfVehicleInfo.VehicleId = vlfFleetVehicles.VehicleId" + whereFilter + " GROUP BY vlfMsgInHst.BoxId, vlfCommMode.CommModeId, vlfCommMode.CommModeName, vlfVehicleInfo.Description, vlfOrganization.OrganizationName, vlfFleetVehicles.FleetId HAVING vlfFleetVehicles.FleetId = " + fleetId;
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by fleetId =" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by fleetId =" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }
        /// <summary>
        /// Populates vlfReportSchedules table. 
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
        public int InsertSecheduledReport(int userID, DateTime periodStart, DateTime periodEnd,
                                          DateTime deliveryDeadLine, string xmlParams, string emails,
                                          string url, int reportType, DateTime statusDate, string deliveryPeriod, bool fleet)
        {

            int rowsAffected = 0;

            // 1. Prepares SQL statement
            // Set SQL command
            string sql = "INSERT INTO vlfReportSchedules (ReportID,DateFrom,DateTo,DeliveryDeadline,Param,Email,Url,UserID,ReportTypeID,Status,StatusDate,DeliveryPeriod,fleet)" +
                        " VALUES (@ReportID,@DateFrom,@DateTo,@DeliveryDeadline,@Param,@Email,@Url,@UserID,@ReportTypeID,@Status,@StatusDate,@DeliveryPeriod,@fleet)";

            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@ReportID", SqlDbType.Int, this.GetMaxReportID() + 1);
            sqlExec.AddCommandParam("@DateFrom", SqlDbType.DateTime, periodStart);
            sqlExec.AddCommandParam("@DateTo", SqlDbType.DateTime, periodEnd);
            sqlExec.AddCommandParam("@DeliveryDeadline", SqlDbType.DateTime, deliveryDeadLine);
            sqlExec.AddCommandParam("@Param", SqlDbType.VarChar, xmlParams);
            sqlExec.AddCommandParam("@Email", SqlDbType.VarChar, emails);
            sqlExec.AddCommandParam("@Url", SqlDbType.VarChar, url);
            sqlExec.AddCommandParam("@UserID", SqlDbType.Int, userID);
            sqlExec.AddCommandParam("@ReportTypeID", SqlDbType.Int, reportType);
            sqlExec.AddCommandParam("@Status", SqlDbType.SmallInt, 0);
            sqlExec.AddCommandParam("@StatusDate", SqlDbType.DateTime, statusDate);
            sqlExec.AddCommandParam("@DeliveryPeriod", SqlDbType.VarChar, deliveryPeriod);
            sqlExec.AddCommandParam("@fleet", SqlDbType.Bit, fleet);
            if (sqlExec.RequiredTransaction())
            {
                // 2. Attach current command SQL to transaction
                sqlExec.AttachToTransaction(sql);
            }

            try
            {
                // 3. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                if (sqlExec.RequiredTransaction())
                {
                    sqlExec.RollbackTransaction();
                }
                string prefixMsg = "Unable to add new Report with Deadline =   '" + deliveryDeadLine.ToString() +
                                    ", for UserID" + userID.ToString() + " .";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                if (sqlExec.RequiredTransaction())
                {
                    sqlExec.RollbackTransaction();
                }
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                if (sqlExec.RequiredTransaction())
                {
                    sqlExec.RollbackTransaction();
                }
                string prefixMsg = "Unable to add new Report with Deadline =   '" + deliveryDeadLine.ToString() +
                                    ", for UserID" + userID.ToString() + " .";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add new Report with Deadline =   '" + deliveryDeadLine.ToString() +
                                    ", for UserID" + userID.ToString() + " .";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " ReportID already exists.");
            }
            return rowsAffected;
        }
        /// <summary>
        /// gets all Scheduled reports for the specied user. 
        /// </summary>
        /// <param name="userID"></param>        
        /// <returns>
        /// DataSet 
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetScheduledReportsByUser(int userID)
        {
            DataSet resultDataSet = null;
            string prefixMsg = "Unable to retrieve report for UserID=" + userID + ". ";
            try
            {
                //Prepares SQL statement
                string sql = "SELECT * FROM vlfReportSchedules WHERE UserID=" + userID;
                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
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
            return resultDataSet;
        }
        /// <summary>
        /// gets all Scheduled reports by status . 
        /// </summary>
        /// <param name="status"></param>        
        /// <returns>
        /// DataSet 
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetScheduledReportsByStatus(int status)
        {
            DataSet resultDataSet = null;
            string prefixMsg = "Unable to retrieve report for status=" + status + ". ";
            try
            {
                //Prepares SQL statement
                string sql = "SELECT * FROM vlfReportSchedules WHERE Status=" + status;
                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
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
            return resultDataSet;
        }
        /// <summary>
        /// gets all Scheduled reports by status . 
        /// </summary>
        /// <param name="originalReportRow"></param>   
        /// <param name="newDelivery"></param> 
        /// <param name="newFrom"></param> 
        /// <param name="newTo"></param> 
        /// <returns>
        /// int 
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int UpdateScheduledReportDatesAndURL(int reportID, DateTime newDelivery, DateTime newFrom, DateTime newTo, string Url, int Status)
        {
            int rowsAffected = 0;
            //Prepares SQL statement
            string sql = "UPDATE vlfReportSchedules SET DateFrom= '" + ConvertDateToString(newFrom) +
                         "', DateTo='" + ConvertDateToString(newTo) +
                         "' DeliveryDeadline= '" + ConvertDateToString(newDelivery) +
                         " Url ='" + Url + " Status = " + Status.ToString() +
                         " WHERE ReportID=" + reportID.ToString();
            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update record for ReportID =" + reportID.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update record for ReportID =" + reportID.ToString() + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update record for ReportID =" + reportID.ToString() + ".";
                throw new DASAppResultNotFoundException(prefixMsg + " This alarm does not exist.");
            }

            return rowsAffected;
        }
        private string ConvertDateToString(DateTime time)
        {
            string strFrom = time.Month.ToString() + "/" + time.Day.ToString() + "/" + time.Year;
            if (time.Hour >= 12)
            {
                if (time.Hour == 12)
                {
                    strFrom += " 12:" + time.Minute.ToString() + ":" + time.Second.ToString() + " PM";
                }
                else
                {
                    strFrom += " " + time.Hour.ToString() + ":" + time.Minute.ToString() + ":" + time.Second.ToString() + " PM";
                }
            }
            else
            {
                if (time.Hour == 0)
                {
                    strFrom += " 12:" + time.Minute.ToString() + ":" + time.Second.ToString() + " AM";
                }
                else
                {
                    strFrom += " " + time.Hour.ToString() + ":" + time.Minute.ToString() + ":" + time.Second.ToString() + " AM";
                }
            }
            return strFrom;
        }
        /// <summary>
        /// gets gui report name reports. 
        /// </summary>
        /// <param name="guiID"></param>        
        /// <returns>
        /// DataSet 
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetReportsName(int guiID)
        {
            DataSet resultDataSet = null;
            string prefixMsg = "Unable to retrieve report type for guiID=" + guiID + ". ";
            try
            {
                //Prepares SQL statement
                string sql = "SELECT GuiName FROM vlfReportTypes WHERE guiID=" + guiID;
                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
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
            return resultDataSet;
        }

        /// <summary>
        /// gets gui report name reports. 
        /// </summary>
        /// <param name="guiID"></param>        
        /// <returns>
        /// DataSet 
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public string GetReportName(int guiID)
        {
            string name = "";
            string prefixMsg = "Unable to retrieve report type for guiID=" + guiID + ". ";
            try
            {
                //Prepares SQL statement
                string sql = "SELECT GuiName FROM vlfReportTypes WHERE guiID=" + guiID;
                //Executes SQL statement
                object result = sqlExec.SQLExecuteScalar(sql);
                if (result != null)
                    name = result.ToString();
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
            return name;
        }

        /// <summary>
        /// Deletes a Scheduled report. 
        /// </summary>
        /// <param name="reportID"></param>        
        /// <returns>
        /// int 
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteByReportID(int reportID)
        {
            int rowsAffected = 0;
            //Prepares SQL statement
            string sql = "DELETE FROM vlfReportSchedules WHERE ReportID=" + reportID;
            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Cannot delete Scheduled Report by id=" + reportID;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Cannot delete Scheduled Report by id=" + reportID;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }
        private int GetMaxReportID()
        {
            int maxRecordIndex = 0;

            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT ReportID FROM vlfReportSchedules ORDER BY ReportID DESC";
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                DataSet resultDataSet = sqlExec.SQLExecuteDataset(sql);
                if (resultDataSet.Tables[0].Rows.Count > 0)
                {
                    maxRecordIndex = Convert.ToInt32(resultDataSet.Tables[0].Rows[0][0]);
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Cannot retrieve max record index from '" + tableName + "' table.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Cannot retrieve max record index from '" + tableName + "' table.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return maxRecordIndex;
        }
        /// <summary>
        /// Returns Latency information from vlfMsgInHst table. 
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
            DataSet resultDataSet = null;
            string whereFilter = "";
            try
            {
                if (fromDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                {
                    whereFilter = " vlfMsgInHst.OriginDateTime>='" + fromDateTime + "'";
                    if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                        whereFilter += " AND vlfMsgInHst.OriginDateTime<='" + toDateTime + "'";
                }
                else if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    whereFilter += " vlfMsgInHst.OriginDateTime<='" + toDateTime + "'";


                if (whereFilter != "")
                    whereFilter += " AND";
                #region Filter by messages
                whereFilter += " (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.PositionUpdate +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Coordinate +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.SensorExtended +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoFence +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GeoZone +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.GPSAntenna +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speed +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobArm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobDisarm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.KeyFobPanic +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.StoredPosition +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MBAlarm +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Idling +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.ExtendedIdling +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Speeding +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Status +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.LowBattery +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.AuxInputOneOn +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.AuxInputTwoOn +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MDTAck +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MDTResponse +
                   " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.MDTTextMessage +
                   ")";
                #endregion
                #region Filter by commModes
                if (dsParams != null && dsParams.Tables.Count > 0 && dsParams.Tables[0].Rows.Count > 0)
                {
                    if (whereFilter != "")
                        whereFilter += " AND ";
                    whereFilter += "(";
                    for (int index = 0; index < dsParams.Tables[0].Rows.Count; ++index)
                    {
                        if (index == 0)
                            whereFilter += "vlfCommMode.CommModeId=" + dsParams.Tables[0].Rows[index]["CommModeId"].ToString();
                        else
                            whereFilter += " OR vlfCommMode.CommModeId=" + dsParams.Tables[0].Rows[index]["CommModeId"].ToString();
                    }
                    whereFilter += ")";
                }
                #endregion

                if (whereFilter != "")
                    whereFilter = " WHERE " + whereFilter;
                string sql = "SELECT vlfOrganization.OrganizationName, vlfVehicleInfo.Description, vlfMsgInHst.BoxId, vlfCommMode.CommModeId, vlfCommMode.CommModeName, COUNT(*) AS NumOfMsgs, AVG(ABS(DATEDIFF(ss, vlfMsgInHst.OriginDateTime, vlfMsgInHst.DateTimeReceived))) AS DiffInSec FROM vlfMsgInHst with (nolock) INNER JOIN vlfFirmwareChannelReference INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannelReference.FwChId = vlfBox.FwChId ON vlfMsgInHst.BoxId = vlfBox.BoxId INNER JOIN vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId AND vlfMsgInHst.BoxProtocolTypeId = vlfChannels.BoxProtocolTypeId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN vlfOrganization ON vlfVehicleInfo.OrganizationId = vlfOrganization.OrganizationId"
                   + whereFilter + " GROUP BY vlfMsgInHst.BoxId, vlfCommMode.CommModeId, vlfCommMode.CommModeName, vlfVehicleInfo.Description, vlfOrganization.OrganizationName";
                if (organizationId != VLF.CLS.Def.Const.unassignedIntValue)
                    sql += ",vlfOrganization.OrganizationId HAVING vlfOrganization.OrganizationId=" + organizationId;
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by organizationId =" + organizationId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve messages from history by organizationId =" + organizationId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }
#if !NEW_REPORTS
      /// <summary>
      /// Prepares detailed trip report
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
      /// </remarks>
      /// <param name="licensePlate"></param>
      /// <param name="fromDateTime"></param>
      /// <param name="toDateTime"></param>
      /// <param name="tblLandmarks"></param>
      /// <param name="tblVehicleSensors"></param>
      /// <param name="tblVehicleGeozones"></param>
      /// <param name="includeStreetAddress"></param>
      /// <param name="includeSensors"></param>
      /// <param name="includePosition"></param>
      /// <param name="includeIdleTime"></param>
      /// <param name="includeSummary"></param>
      /// <param name="showLastStoredPosition"></param>
      /// <param name="carCost"></param>
      /// <param name="userId"></param>
      /// <param name="measurementUnits"></param>
      /// <param name="dsResult"></param>
      /// <param name="requestOverflowed"></param>
      /// <param name="isTrailer"></param>
      /// <param name="totalSqlRecords"></param>
      /// <returns> DataSet [TripIndex],[Reson],[Date/Time],[Location],[Speed],[Description] </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void GetDetailedTripReport(string licensePlate,
                        string fromDateTime,
                        string toDateTime,
                        DataTable tblVehicleSensors,
                        DataTable tblVehicleGeozones,
                        bool includeStreetAddress,
                        bool includeSensors,
                        bool includePosition,
                        bool includeIdleTime,
                        bool includeSummary,
                        bool showLastStoredPosition,
                        double carCost,
                        int userId,
                        double measurementUnits,
                        DataSet dsResult,
                        bool isTrailer,string lang,
                        ref bool requestOverflowed,
                        ref int totalSqlRecords)
      {
         //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
         //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
         //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
         DataSet dsHistory = GetMsgInHistoryInformation(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
         if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
         {
            ReportVehicleTrip report = new ReportVehicleTrip(GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100), sqlExec);

            report.FillReport(tblVehicleSensors,
                        tblVehicleGeozones,
                        includeStreetAddress,
                        includeSensors,
                        includePosition,
                        includeIdleTime,
                        showLastStoredPosition,
                        carCost,
                        dsHistory,
                        measurementUnits,
                        Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                        userId, isTrailer,lang);
            dsResult.Tables.Add(report.TripReportDetailes);
            if (includeSummary == true)
            {
               dsResult.Tables.Add(report.TripsStart);
               dsResult.Tables.Add(report.TripsAverageSpeed);
               dsResult.Tables.Add(report.TripsCost);
               dsResult.Tables.Add(report.TripsDistance);
               dsResult.Tables.Add(report.TripsDuration);
               dsResult.Tables.Add(report.TripsStopsDuration);
               dsResult.Tables.Add(report.TripsFuelCons);
               dsResult.Tables.Add(report.TripsEnd);
        
               if (report.TripsEnd.Rows.Count > 0)
               {
                  dsResult.Tables.Add(GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd));
               }
            }
         }
      }

      
      /// <summary>
      /// Prepares trip report
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
      /// <param name="tblVehicleSensors"></param>
      /// <param name="tblVehicleGeozones"></param>
      /// <param name="carCost"></param>
      /// <param name="userId"></param>
      /// <param name="measurementUnits"></param>
      /// <param name="dsResult"></param>
      /// <param name="isTrailer"></param>
      /// <param name="showLastStoredPosition"></param>
      /// <param name="requestOverflowed"></param>
      /// <param name="totalSqlRecords"></param>
      /// <returns> DataSet [TripIndex],[Reson],[Date/Time],[Location],[Speed],[Description] </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void GetTripReport(string licensePlate,
                           string fromDateTime,
                           string toDateTime,
                           DataTable tblVehicleSensors,
                           DataTable tblVehicleGeozones,
                           double carCost,
                           int userId,
                           double measurementUnits,
                           DataSet dsResult,
                           bool showLastStoredPosition,
                           bool isTrailer,string lang,
                           ref bool requestOverflowed,
                           ref int totalSqlRecords)
      {
         //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
         //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
         //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
         DataSet dsHistory = GetMsgInHistoryInformation(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
         if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
         {
            DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
            ReportVehicleTrip report = new ReportVehicleTrip(GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100), sqlExec);

            report.FillReport(tblVehicleSensors,
                           tblVehicleGeozones,
                           true,
                           false,
                           false,
                           false,
                           showLastStoredPosition,
                           carCost,
                           dsHistory,
                           measurementUnits,
                           Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                           userId, isTrailer,lang );

            // rows 
            dsResult.Tables.Add(report.TripsStart);
            dsResult.Tables.Add(report.TripsEnd);
            dsResult.Tables.Add(report.TripsDuration);
            dsResult.Tables.Add(report.TripsDistance);
            dsResult.Tables.Add(report.TripsStopsDuration);
            // total
            dsResult.Tables.Add(report.TripsAverageSpeed);
            dsResult.Tables.Add(report.TripsCost);
            dsResult.Tables.Add(report.TripsFuelCons);
            dsResult.Tables.Add(GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd));
         }
      }
      /// <summary>
      /// Prepares stop report
      /// </summary>
      /// <param name="licensePlate"></param>
      /// <param name="fromDateTime"></param>
      /// <param name="toDateTime"></param>
      /// <param name="tblVehicleSensors"></param>
      /// <param name="tblVehicleGeozones"></param>
      /// <param name="carCost"></param>
      /// <param name="userId"></param>
      /// <param name="measurementUnits"></param>
      /// <param name="dsResult"></param>
      /// <param name="showLastStoredPosition"></param>
      /// <param name="minStopDuration"></param>
      /// <param name="isTrailer"></param>
      /// <param name="requestOverflowed"></param>
      /// <param name="totalSqlRecords"></param>
      /// <returns> DataSet [StopIndex],[ArrivalDateTime],[Location],[DepartureDateTime],
      /// [StopDuration],[Remarks],[Latitude],[Longitude] </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void GetStopReport(string licensePlate,
                        string fromDateTime,
                        string toDateTime,
                        DataTable tblVehicleSensors,
                        DataTable tblVehicleGeozones,
                        double carCost,
                        int userId,
                        double measurementUnits,
                        DataSet dsResult,
                        bool showLastStoredPosition,
                        int minStopDuration,//in sec
                        bool isTrailer,
                        bool inclStop,
                        bool inclIdling,string lang,
                        ref bool requestOverflowed,
                        ref int totalSqlRecords)
      {
         //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
         //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
         //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
         DataSet dsHistory = GetMsgInHistoryInformation(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
         if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
         {
            short idlingRange = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100);
            DataRow[] foundRows = null;
            DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
            ReportVehicleTrip report = new ReportVehicleTrip(idlingRange, sqlExec);

            report.FillReport(tblVehicleSensors,
                           tblVehicleGeozones,
                           true,//always include street address
                           false,
                           false,
                           true,
                           showLastStoredPosition,
                           carCost,
                           dsHistory,
                           measurementUnits,
                           Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                           userId, isTrailer,lang );

        #region Define result dataset
            DataTable tblStopData = new DataTable("StopData");
            tblStopData.Columns.Add("StopIndex", typeof(string));
            tblStopData.Columns.Add("BoxId", typeof(string));
            tblStopData.Columns.Add("ArrivalDateTime", typeof(DateTime));
            tblStopData.Columns.Add("Location", typeof(string));
            tblStopData.Columns.Add("DepartureDateTime", typeof(DateTime));
            tblStopData.Columns.Add("StopDuration", typeof(string));
            tblStopData.Columns.Add("Remarks", typeof(string));
            tblStopData.Columns.Add("Latitude", typeof(string));
            tblStopData.Columns.Add("Longitude", typeof(string));
            tblStopData.Columns.Add("StopDurationVal", typeof(string));
            tblStopData.Columns.Add("VehicleId", typeof(string));
            tblStopData.Columns.Add("IsLandmark", typeof(bool));
        #endregion

            if (inclIdling)
            {
        #region Add idling time to stop report
               // Retrieve idling time info in all trips
               //[TripIndex],
               //[Reason],								<- Engine Idle
               //[Date/Time],[Location],[Speed],
               //[Description],						<- Duration 00:04:59
               //[BoxId],[Latitude],[Longitude],
               //[Remarks]								<- 499
               DataTable tblReportDetailes = report.TripReportDetailes.Clone();
               foundRows = report.TripReportDetailes.Select("Reason='Engine Idle'");
               foreach (DataRow newRow in foundRows)
                  tblReportDetailes.ImportRow(newRow);
               if (tblReportDetailes != null ||
                       tblReportDetailes.Rows.Count > 0)
               {
                  TimeSpan timeTick;
                  foreach (DataRow ittr in tblReportDetailes.Rows)
                  {
                     timeTick = new TimeSpan(Convert.ToInt64(ittr["Remarks"]) * TimeSpan.TicksPerSecond);
                     if (timeTick.TotalSeconds >= idlingRange && timeTick.TotalSeconds >= minStopDuration)
                     {
                        object[] objRow = new object[tblStopData.Columns.Count];
                        objRow[0] = "-1";
                        objRow[1] = ittr["BoxId"].ToString();
                        objRow[2] = ittr["Date/Time"];
                        objRow[3] = ittr["Location"].ToString();
                        objRow[4] = new DateTime(Convert.ToDateTime(ittr["Date/Time"]).Ticks + timeTick.Ticks);
                        objRow[5] = timeTick.ToString();
                        objRow[6] = Resources.Const.Idling;
                        objRow[7] = ittr["Latitude"].ToString();
                        objRow[8] = ittr["Longitude"].ToString();
                        objRow[9] = timeTick.TotalSeconds.ToString();
                        objRow[10] = ittr["VehicleId"].ToString();
                        objRow[11] = ittr["IsLandmark"].ToString();
                        tblStopData.Rows.Add(objRow);
                     }
                  }
               }
        #endregion
            }

            if (inclStop)
            {
        #region Add stopped time to stop report
               // Retieve stop info between trips
               //[TripIndex],
               //[Summary],							<- 00:00:00
               //[Remarks],							<- 0
               //[BoxId],[Date/Time],[Location],[Latitude],[Longitude]
               DataTable tblStopDurationBetweenTrips = GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd);
               if (tblStopDurationBetweenTrips != null ||
                   tblStopDurationBetweenTrips.Rows.Count > 0)
               {
                  TimeSpan timeTick;
                  for (int index = 0; index < tblStopDurationBetweenTrips.Rows.Count; ++index)
                  {
                     DataRow ittr = tblStopDurationBetweenTrips.Rows[index];
                     timeTick = new TimeSpan(Convert.ToInt64(ittr["Remarks"]) * TimeSpan.TicksPerSecond);
                     if (timeTick.TotalSeconds >= minStopDuration)
                     {
                        object[] objRow = new object[tblStopData.Columns.Count];
                        objRow[0] = "-1";
                        objRow[1] = ittr["BoxId"].ToString();
                        objRow[2] = ittr["Date/Time"];
                        objRow[3] = ittr["Location"].ToString();
                        // check if las trip been completed
                        if (index == tblStopDurationBetweenTrips.Rows.Count - 1 && timeTick.TotalSeconds == 0)
                           objRow[4] = VLF.CLS.Def.Const.unassignedDateTime;
                        else
                           objRow[4] = new DateTime(Convert.ToDateTime(ittr["Date/Time"]).Ticks + timeTick.Ticks);
                        objRow[5] = timeTick.ToString();
                        objRow[6] =Resources.Const.Stopped;
                        objRow[7] = ittr["Latitude"].ToString();
                        objRow[8] = ittr["Longitude"].ToString();
                        objRow[9] = timeTick.TotalSeconds.ToString();
                        objRow[10] = ittr["VehicleId"].ToString();
                        objRow[11] = ittr["IsLandmark"].ToString();
                        tblStopData.Rows.Add(objRow);
                     }
                  }
               }
        #endregion
            }

        #region Sort result by ArrivalDateTime and update StopIndex field
            //foundRows = tblStopData.Select("","ArrivalDateTime ASC");
            foundRows = tblStopData.Select("", "ArrivalDateTime DESC");
            //tblStopData.Clear();
            DataTable tblResult = tblStopData.Clone();
            //int newRowIndex = 0;
            int newRowIndex = foundRows.Length;
            foreach (DataRow updateRow in foundRows)
            {
               //updateRow["StopIndex"] = ++newRowIndex;
               updateRow["StopIndex"] = newRowIndex--;
               tblResult.ImportRow(updateRow);
            }
        #endregion

            dsResult.Tables.Add(tblResult);
         }
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
      /// <param name="tblVehicleSensors"></param>
      /// <param name="tblVehicleGeozones"></param>
      /// <param name="carCost"></param>
      /// <param name="userId"></param>
      /// <param name="measurementUnits"></param>
      /// <param name="dsResult"></param>
      /// <param name="showLastStoredPosition"></param>
      /// <param name="isTrailer"></param>
      /// <param name="requestOverflowed"></param>
      /// <param name="totalSqlRecords"></param>
      /// <returns> DataSet [TripIndex],[Reson],[Date/Time],[Location],[Speed],[Description] </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void GetTripActivityReport(string licensePlate,
                                 string fromDateTime,
                                 string toDateTime,
                                 DataTable tblVehicleSensors,
                                 DataTable tblVehicleGeozones,
                                 double carCost,
                                 int userId,
                                 double measurementUnits,
                                 DataSet dsResult,
                                 bool showLastStoredPosition,
                                 bool isTrailer,string lang,
                                 ref bool requestOverflowed,
                                 ref int totalSqlRecords)
      {
         //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
         //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
         //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
         DataSet dsHistory = GetMsgInHistoryInformation(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
         if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
         {
            DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
            ReportVehicleTrip report = new ReportVehicleTrip(GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100), sqlExec);

            report.FillReport(tblVehicleSensors,
                           tblVehicleGeozones,
                           true,
                           false,
                           false,
                           false,
                           showLastStoredPosition,
                           carCost,
                           dsHistory,
                           measurementUnits,
                           Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                           userId, isTrailer,lang);

            // rows 
            dsResult.Tables.Add(report.TripsStart);
            dsResult.Tables.Add(report.TripsStopsDuration);
            dsResult.Tables.Add(report.TripsDuration);
            dsResult.Tables.Add(report.TripsDistance);
            dsResult.Tables.Add(report.TripsCost);

            //dsResult.Tables.Add(report.TripsEnd);
            //dsResult.Tables.Add(report.TripsAverageSpeed);
            dsResult.Tables.Add(GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd));
         }
      }
#else
        public void GetStopReport(string licensePlate,
                                   string fromDateTime,
                                   string toDateTime,
                                   DataTable tblVehicleSensors,
                                   DataTable tblVehicleGeozones,
                                   double carCost,
                                   int userId,
                                   double measurementUnits,
                                   DataSet dsResult,
                                   bool showLastStoredPosition,
                                   int minStopDuration,//in sec
                                   short vehicleType,
                                   bool inclStop,
                                   bool inclIdling, string lang,
                                   ref bool requestOverflowed,
                                   ref int totalSqlRecords)
        {
            //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
            //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
            //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
            DataSet dsHistory = GetMsgInHistoryInformation(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
            if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
            {
                short idlingRange = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100);
                DataRow[] foundRows = null;
                DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
                ReportVehicleTrip report = new ReportVehicleTrip(idlingRange, sqlExec);

                report.FillReport(tblVehicleSensors,
                               tblVehicleGeozones,
                               true,//always include street address
                               false,
                               false,
                               true,
                               showLastStoredPosition,
                               carCost,
                               dsHistory,
                               measurementUnits,
                               Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                               userId, vehicleType, lang);

                #region Define result dataset
                DataTable tblStopData = new DataTable("StopData");
                tblStopData.Columns.Add("StopIndex", typeof(string));
                tblStopData.Columns.Add("BoxId", typeof(string));
                tblStopData.Columns.Add("ArrivalDateTime", typeof(DateTime));
                tblStopData.Columns.Add("Location", typeof(string));
                tblStopData.Columns.Add("DepartureDateTime", typeof(DateTime));
                tblStopData.Columns.Add("StopDuration", typeof(string));
                tblStopData.Columns.Add("Remarks", typeof(string));
                tblStopData.Columns.Add("Latitude", typeof(string));
                tblStopData.Columns.Add("Longitude", typeof(string));
                tblStopData.Columns.Add("StopDurationVal", typeof(string));
                tblStopData.Columns.Add("VehicleId", typeof(string));
                tblStopData.Columns.Add("IsLandmark", typeof(bool));
                #endregion


                Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

                if (inclIdling)
                {
                    #region Add idling time to stop report
                    // Retrieve idling time info in all trips
                    //[TripIndex],
                    //[Reason],								<- Engine Idle
                    //[Date/Time],[Location],[Speed],
                    //[Description],						<- Duration 00:04:59
                    //[BoxId],[Latitude],[Longitude],
                    //[Remarks]								<- 499
                    DataTable tblReportDetailes = report.TripReportDetailes.Clone();
                    foundRows = report.TripReportDetailes.Select("Reason='" + Resources.Const.EngineIdle + "'");
                    foreach (DataRow newRow in foundRows)
                        tblReportDetailes.ImportRow(newRow);
                    if (tblReportDetailes != null ||
                            tblReportDetailes.Rows.Count > 0)
                    {
                        TimeSpan timeTick;
                        foreach (DataRow ittr in tblReportDetailes.Rows)
                        {
                            timeTick = new TimeSpan(Convert.ToInt64(ittr["Remarks"]) * TimeSpan.TicksPerSecond);
                            if (timeTick.TotalSeconds >= idlingRange && timeTick.TotalSeconds >= minStopDuration)
                            {
                                object[] objRow = new object[tblStopData.Columns.Count];
                                objRow[0] = "-1";
                                objRow[1] = ittr["BoxId"].ToString();
                                objRow[2] = ittr["Date/Time"];
                                objRow[3] = ittr["Location"].ToString();
                                objRow[4] = new DateTime(Convert.ToDateTime(ittr["Date/Time"]).Ticks + timeTick.Ticks);
                                objRow[5] = timeTick.ToString();
                                objRow[6] = Resources.Const.Idling;
                                objRow[7] = ittr["Latitude"].ToString();
                                objRow[8] = ittr["Longitude"].ToString();
                                objRow[9] = timeTick.TotalSeconds.ToString();
                                objRow[10] = ittr["VehicleId"].ToString();
                                objRow[11] = ittr["IsLandmark"].ToString();
                                tblStopData.Rows.Add(objRow);
                            }
                        }
                    }
                    #endregion
                }

                if (inclStop)
                {
                    #region Add stopped time to stop report
                    // Retieve stop info between trips
                    //[TripIndex],
                    //[Summary],							<- 00:00:00
                    //[Remarks],							<- 0
                    //[BoxId],[Date/Time],[Location],[Latitude],[Longitude]
                    DataTable tblStopDurationBetweenTrips = GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd);
                    if (tblStopDurationBetweenTrips != null ||
                        tblStopDurationBetweenTrips.Rows.Count > 0)
                    {
                        TimeSpan timeTick;
                        for (int index = 0; index < tblStopDurationBetweenTrips.Rows.Count; ++index)
                        {
                            DataRow ittr = tblStopDurationBetweenTrips.Rows[index];
                            timeTick = new TimeSpan(Convert.ToInt64(ittr["Remarks"]) * TimeSpan.TicksPerSecond);
                            if (timeTick.TotalSeconds >= minStopDuration)
                            {
                                object[] objRow = new object[tblStopData.Columns.Count];
                                objRow[0] = "-1";
                                objRow[1] = ittr["BoxId"].ToString();
                                objRow[2] = ittr["Date/Time"];
                                objRow[3] = ittr["Location"].ToString();
                                // check if las trip been completed
                                if (index == tblStopDurationBetweenTrips.Rows.Count - 1 && timeTick.TotalSeconds == 0)
                                    objRow[4] = VLF.CLS.Def.Const.unassignedDateTime;
                                else
                                    objRow[4] = new DateTime(Convert.ToDateTime(ittr["Date/Time"]).Ticks + timeTick.Ticks);
                                objRow[5] = timeTick.ToString();
                                objRow[6] = Resources.Const.Stopped;
                                objRow[7] = ittr["Latitude"].ToString();
                                objRow[8] = ittr["Longitude"].ToString();
                                objRow[9] = timeTick.TotalSeconds.ToString();
                                objRow[10] = ittr["VehicleId"].ToString();
                                objRow[11] = ittr["IsLandmark"].ToString();
                                tblStopData.Rows.Add(objRow);
                            }
                        }
                    }
                    #endregion
                }

                #region Sort result by ArrivalDateTime and update StopIndex field
                //foundRows = tblStopData.Select("","ArrivalDateTime ASC");
                foundRows = tblStopData.Select("", "ArrivalDateTime DESC");
                //tblStopData.Clear();
                DataTable tblResult = tblStopData.Clone();
                //int newRowIndex = 0;
                int newRowIndex = foundRows.Length;
                foreach (DataRow updateRow in foundRows)
                {
                    //updateRow["StopIndex"] = ++newRowIndex;
                    updateRow["StopIndex"] = newRowIndex--;
                    tblResult.ImportRow(updateRow);
                }
                #endregion

                dsResult.Tables.Add(tblResult);
            }
        }
        // Changes for TimeZone Feature start
        public void GetStopReport_NewTZ(string licensePlate,
                                   string fromDateTime,
                                   string toDateTime,
                                   DataTable tblVehicleSensors,
                                   DataTable tblVehicleGeozones,
                                   double carCost,
                                   int userId,
                                   double measurementUnits,
                                   DataSet dsResult,
                                   bool showLastStoredPosition,
                                   int minStopDuration,//in sec
                                   short vehicleType,
                                   bool inclStop,
                                   bool inclIdling,
                                   int sensorId,
                                   string lang,
                                   ref bool requestOverflowed,
                                   ref int totalSqlRecords)
        {
            //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
            //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
            //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
            DataSet dsHistory = GetMsgInHistoryInformation_NewTZ(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
            if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
            {
                short idlingRange = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100);
                DataRow[] foundRows = null;
                DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
                ReportVehicleTrip report = new ReportVehicleTrip(idlingRange, sqlExec);

                report.FillReport(tblVehicleSensors,
                               tblVehicleGeozones,
                               true,//always include street address
                               false,
                               false,
                               true,
                               showLastStoredPosition,
                               carCost,
                               dsHistory,
                               measurementUnits,
                               Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                               userId, sensorId, vehicleType, lang);

                #region Define result dataset
                DataTable tblStopData = new DataTable("StopData");
                tblStopData.Columns.Add("StopIndex", typeof(string));
                tblStopData.Columns.Add("BoxId", typeof(string));
                tblStopData.Columns.Add("ArrivalDateTime", typeof(DateTime));
                tblStopData.Columns.Add("Location", typeof(string));
                tblStopData.Columns.Add("DepartureDateTime", typeof(DateTime));
                tblStopData.Columns.Add("StopDuration", typeof(string));
                tblStopData.Columns.Add("Remarks", typeof(string));
                tblStopData.Columns.Add("Latitude", typeof(string));
                tblStopData.Columns.Add("Longitude", typeof(string));
                tblStopData.Columns.Add("StopDurationVal", typeof(string));
                tblStopData.Columns.Add("VehicleId", typeof(string));
                tblStopData.Columns.Add("IsLandmark", typeof(bool));
                tblStopData.Columns.Add("Driver", typeof(string));
                tblStopData.Columns.Add("DriverHIDCard", typeof(string));
                #endregion

                Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

                Driver driver = new Driver(sqlExec);

                if (inclIdling)
                {
                    #region Add idling time to stop report
                    // Retrieve idling time info in all trips
                    //[TripIndex],
                    //[Reason],								<- Engine Idle
                    //[Date/Time],[Location],[Speed],
                    //[Description],						<- Duration 00:04:59
                    //[BoxId],[Latitude],[Longitude],
                    //[Remarks]								<- 499
                    DataTable tblReportDetailes = report.TripReportDetailes.Clone();
                    foundRows = report.TripReportDetailes.Select("Reason='" + Resources.Const.EngineIdle + "'");
                    foreach (DataRow newRow in foundRows)
                        tblReportDetailes.ImportRow(newRow);
                    if (tblReportDetailes != null ||
                            tblReportDetailes.Rows.Count > 0)
                    {
                        TimeSpan timeTick;
                        foreach (DataRow ittr in tblReportDetailes.Rows)
                        {
                            timeTick = new TimeSpan(Convert.ToInt64(ittr["Remarks"]) * TimeSpan.TicksPerSecond);
                            if (timeTick.TotalSeconds >= idlingRange && timeTick.TotalSeconds >= minStopDuration)
                            {
                                object[] objRow = new object[tblStopData.Columns.Count];
                                objRow[0] = "-1";
                                objRow[1] = ittr["BoxId"].ToString();
                                objRow[2] = ittr["Date/Time"];
                                objRow[3] = ittr["Location"].ToString();
                                objRow[4] = new DateTime(Convert.ToDateTime(ittr["Date/Time"]).Ticks + timeTick.Ticks);

                                objRow[5] = timeTick.ToString();
                                objRow[6] = Resources.Const.Idling;
                                objRow[7] = ittr["Latitude"].ToString();
                                objRow[8] = ittr["Longitude"].ToString();
                                objRow[9] = timeTick.TotalSeconds.ToString();
                                objRow[10] = ittr["VehicleId"].ToString();
                                objRow[11] = ittr["IsLandmark"].ToString();
                                objRow[12] = driver.GetTripDriverWithTimezone_NewTZ(userId, "", Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]), Convert.ToDateTime(ittr["Date/Time"]), Convert.ToDateTime(ittr["Date/Time"]));
                                objRow[13] = driver.GetTripDriverKeyFobIdWithTimezone_NewTZ(userId, "", Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]), Convert.ToDateTime(ittr["Date/Time"]), Convert.ToDateTime(ittr["Date/Time"]));
                                tblStopData.Rows.Add(objRow);
                            }
                        }
                    }
                    #endregion
                }

                if (inclStop)
                {
                    #region Add stopped time to stop report
                    // Retieve stop info between trips
                    //[TripIndex],
                    //[Summary],							<- 00:00:00
                    //[Remarks],							<- 0
                    //[BoxId],[Date/Time],[Location],[Latitude],[Longitude]
                    DataTable tblStopDurationBetweenTrips = GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd);
                    if (tblStopDurationBetweenTrips != null ||
                        tblStopDurationBetweenTrips.Rows.Count > 0)
                    {
                        TimeSpan timeTick;
                        for (int index = 0; index < tblStopDurationBetweenTrips.Rows.Count; ++index)
                        {
                            DataRow ittr = tblStopDurationBetweenTrips.Rows[index];
                            timeTick = new TimeSpan(Convert.ToInt64(ittr["Remarks"]) * TimeSpan.TicksPerSecond);
                            if (timeTick.TotalSeconds >= minStopDuration)
                            {
                                object[] objRow = new object[tblStopData.Columns.Count];
                                objRow[0] = "-1";
                                objRow[1] = ittr["BoxId"].ToString();
                                objRow[2] = ittr["Date/Time"];
                                objRow[3] = ittr["Location"].ToString();
                                // check if las trip been completed
                                if (index == tblStopDurationBetweenTrips.Rows.Count - 1 && timeTick.TotalSeconds == 0)
                                    objRow[4] = VLF.CLS.Def.Const.unassignedDateTime;
                                else
                                    objRow[4] = new DateTime(Convert.ToDateTime(ittr["Date/Time"]).Ticks + timeTick.Ticks);
                                objRow[5] = timeTick.ToString();
                                objRow[6] = Resources.Const.Stopped;
                                objRow[7] = ittr["Latitude"].ToString();
                                objRow[8] = ittr["Longitude"].ToString();
                                objRow[9] = timeTick.TotalSeconds.ToString();
                                objRow[10] = ittr["VehicleId"].ToString();
                                objRow[11] = ittr["IsLandmark"].ToString();
                                objRow[12] = driver.GetTripDriverWithTimezone_NewTZ(userId, "", Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]), Convert.ToDateTime(ittr["Date/Time"]), Convert.ToDateTime(ittr["Date/Time"]));
                                objRow[13] = driver.GetTripDriverKeyFobIdWithTimezone_NewTZ(userId, "", Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]), Convert.ToDateTime(ittr["Date/Time"]), Convert.ToDateTime(ittr["Date/Time"]));
                                tblStopData.Rows.Add(objRow);
                            }
                        }
                    }
                    #endregion
                }

                #region Sort result by ArrivalDateTime and update StopIndex field
                //foundRows = tblStopData.Select("","ArrivalDateTime ASC");
                foundRows = tblStopData.Select("", "ArrivalDateTime DESC");
                //tblStopData.Clear();
                DataTable tblResult = tblStopData.Clone();
                //int newRowIndex = 0;
                int newRowIndex = foundRows.Length;
                foreach (DataRow updateRow in foundRows)
                {
                    //updateRow["StopIndex"] = ++newRowIndex;
                    updateRow["StopIndex"] = newRowIndex--;
                    tblResult.ImportRow(updateRow);
                }
                #endregion

                dsResult.Tables.Add(tblResult);
            }
        }

        public void GetStopReport(string licensePlate,
                                         string fromDateTime,
                                         string toDateTime,
                                         DataTable tblVehicleSensors,
                                         DataTable tblVehicleGeozones,
                                         double carCost,
                                         int userId,
                                         double measurementUnits,
                                         DataSet dsResult,
                                         bool showLastStoredPosition,
                                         int minStopDuration,//in sec
                                         short vehicleType,
                                         bool inclStop,
                                         bool inclIdling,
                                         int sensorId,
                                         string lang,
                                         ref bool requestOverflowed,
                                         ref int totalSqlRecords)
        {
            //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
            //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
            //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
            DataSet dsHistory = GetMsgInHistoryInformation(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
            if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
            {
                short idlingRange = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100);
                DataRow[] foundRows = null;
                DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
                ReportVehicleTrip report = new ReportVehicleTrip(idlingRange, sqlExec);

                report.FillReport(tblVehicleSensors,
                               tblVehicleGeozones,
                               true,//always include street address
                               false,
                               false,
                               true,
                               showLastStoredPosition,
                               carCost,
                               dsHistory,
                               measurementUnits,
                               Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                               userId, sensorId, vehicleType, lang);

                #region Define result dataset
                DataTable tblStopData = new DataTable("StopData");
                tblStopData.Columns.Add("StopIndex", typeof(string));
                tblStopData.Columns.Add("BoxId", typeof(string));
                tblStopData.Columns.Add("ArrivalDateTime", typeof(DateTime));
                tblStopData.Columns.Add("Location", typeof(string));
                tblStopData.Columns.Add("DepartureDateTime", typeof(DateTime));
                tblStopData.Columns.Add("StopDuration", typeof(string));
                tblStopData.Columns.Add("Remarks", typeof(string));
                tblStopData.Columns.Add("Latitude", typeof(string));
                tblStopData.Columns.Add("Longitude", typeof(string));
                tblStopData.Columns.Add("StopDurationVal", typeof(string));
                tblStopData.Columns.Add("VehicleId", typeof(string));
                tblStopData.Columns.Add("IsLandmark", typeof(bool));
                tblStopData.Columns.Add("Driver", typeof(string));
                tblStopData.Columns.Add("DriverHIDCard", typeof(string));
                #endregion

                Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

                Driver driver = new Driver(sqlExec);

                if (inclIdling)
                {
                    #region Add idling time to stop report
                    // Retrieve idling time info in all trips
                    //[TripIndex],
                    //[Reason],								<- Engine Idle
                    //[Date/Time],[Location],[Speed],
                    //[Description],						<- Duration 00:04:59
                    //[BoxId],[Latitude],[Longitude],
                    //[Remarks]								<- 499
                    DataTable tblReportDetailes = report.TripReportDetailes.Clone();
                    foundRows = report.TripReportDetailes.Select("Reason='" + Resources.Const.EngineIdle + "'");
                    foreach (DataRow newRow in foundRows)
                        tblReportDetailes.ImportRow(newRow);
                    if (tblReportDetailes != null ||
                            tblReportDetailes.Rows.Count > 0)
                    {
                        TimeSpan timeTick;
                        foreach (DataRow ittr in tblReportDetailes.Rows)
                        {
                            timeTick = new TimeSpan(Convert.ToInt64(ittr["Remarks"]) * TimeSpan.TicksPerSecond);
                            if (timeTick.TotalSeconds >= idlingRange && timeTick.TotalSeconds >= minStopDuration)
                            {
                                object[] objRow = new object[tblStopData.Columns.Count];
                                objRow[0] = "-1";
                                objRow[1] = ittr["BoxId"].ToString();
                                objRow[2] = ittr["Date/Time"];
                                objRow[3] = ittr["Location"].ToString();
                                objRow[4] = new DateTime(Convert.ToDateTime(ittr["Date/Time"]).Ticks + timeTick.Ticks);

                                objRow[5] = timeTick.ToString();
                                objRow[6] = Resources.Const.Idling;
                                objRow[7] = ittr["Latitude"].ToString();
                                objRow[8] = ittr["Longitude"].ToString();
                                objRow[9] = timeTick.TotalSeconds.ToString();
                                objRow[10] = ittr["VehicleId"].ToString();
                                objRow[11] = ittr["IsLandmark"].ToString();
                                objRow[12] = driver.GetTripDriverWithTimezone(userId, "", Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]), Convert.ToDateTime(ittr["Date/Time"]), Convert.ToDateTime(ittr["Date/Time"]));
                                objRow[13] = driver.GetTripDriverKeyFobIdWithTimezone(userId, "", Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]), Convert.ToDateTime(ittr["Date/Time"]), Convert.ToDateTime(ittr["Date/Time"]));
                                tblStopData.Rows.Add(objRow);
                            }
                        }
                    }
                    #endregion
                }

                if (inclStop)
                {
                    #region Add stopped time to stop report
                    // Retieve stop info between trips
                    //[TripIndex],
                    //[Summary],							<- 00:00:00
                    //[Remarks],							<- 0
                    //[BoxId],[Date/Time],[Location],[Latitude],[Longitude]
                    DataTable tblStopDurationBetweenTrips = GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd);
                    if (tblStopDurationBetweenTrips != null ||
                        tblStopDurationBetweenTrips.Rows.Count > 0)
                    {
                        TimeSpan timeTick;
                        for (int index = 0; index < tblStopDurationBetweenTrips.Rows.Count; ++index)
                        {
                            DataRow ittr = tblStopDurationBetweenTrips.Rows[index];
                            timeTick = new TimeSpan(Convert.ToInt64(ittr["Remarks"]) * TimeSpan.TicksPerSecond);
                            if (timeTick.TotalSeconds >= minStopDuration)
                            {
                                object[] objRow = new object[tblStopData.Columns.Count];
                                objRow[0] = "-1";
                                objRow[1] = ittr["BoxId"].ToString();
                                objRow[2] = ittr["Date/Time"];
                                objRow[3] = ittr["Location"].ToString();
                                // check if las trip been completed
                                if (index == tblStopDurationBetweenTrips.Rows.Count - 1 && timeTick.TotalSeconds == 0)
                                    objRow[4] = VLF.CLS.Def.Const.unassignedDateTime;
                                else
                                    objRow[4] = new DateTime(Convert.ToDateTime(ittr["Date/Time"]).Ticks + timeTick.Ticks);
                                objRow[5] = timeTick.ToString();
                                objRow[6] = Resources.Const.Stopped;
                                objRow[7] = ittr["Latitude"].ToString();
                                objRow[8] = ittr["Longitude"].ToString();
                                objRow[9] = timeTick.TotalSeconds.ToString();
                                objRow[10] = ittr["VehicleId"].ToString();
                                objRow[11] = ittr["IsLandmark"].ToString();
                                objRow[12] = driver.GetTripDriverWithTimezone(userId, "", Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]), Convert.ToDateTime(ittr["Date/Time"]), Convert.ToDateTime(ittr["Date/Time"]));
                                objRow[13] = driver.GetTripDriverKeyFobIdWithTimezone(userId, "", Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]), Convert.ToDateTime(ittr["Date/Time"]), Convert.ToDateTime(ittr["Date/Time"]));
                                tblStopData.Rows.Add(objRow);
                            }
                        }
                    }
                    #endregion
                }

                #region Sort result by ArrivalDateTime and update StopIndex field
                //foundRows = tblStopData.Select("","ArrivalDateTime ASC");
                foundRows = tblStopData.Select("", "ArrivalDateTime DESC");
                //tblStopData.Clear();
                DataTable tblResult = tblStopData.Clone();
                //int newRowIndex = 0;
                int newRowIndex = foundRows.Length;
                foreach (DataRow updateRow in foundRows)
                {
                    //updateRow["StopIndex"] = ++newRowIndex;
                    updateRow["StopIndex"] = newRowIndex--;
                    tblResult.ImportRow(updateRow);
                }
                #endregion

                dsResult.Tables.Add(tblResult);
            }
        }

        public void GetTripActivityReport(string licensePlate,
                                  string fromDateTime,
                                  string toDateTime,
                                  DataTable tblVehicleSensors,
                                  DataTable tblVehicleGeozones,
                                  double carCost,
                                  int userId,
                                  double measurementUnits,
                                  DataSet dsResult,
                                  bool showLastStoredPosition,
                                  short vehicleType, string lang,
                                  ref bool requestOverflowed,
                                  ref int totalSqlRecords)
        {
            //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
            //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
            //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
            DataSet dsHistory = GetMsgInHistoryInformation(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
            if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
            {
                DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
                ReportVehicleTrip report = new ReportVehicleTrip(GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100), sqlExec);

                report.FillReport(tblVehicleSensors,
                               tblVehicleGeozones,
                               true,
                               false,
                               false,
                               false,
                               showLastStoredPosition,
                               carCost,
                               dsHistory,
                               measurementUnits,
                               Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                               userId, vehicleType, lang);

                // rows 
                dsResult.Tables.Add(report.TripsStart);
                dsResult.Tables.Add(report.TripsStopsDuration);
                dsResult.Tables.Add(report.TripsDuration);
                dsResult.Tables.Add(report.TripsDistance);
                dsResult.Tables.Add(report.TripsCost);

                //dsResult.Tables.Add(report.TripsEnd);
                //dsResult.Tables.Add(report.TripsAverageSpeed);
                dsResult.Tables.Add(GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd));
            }
        }
        // Changes for TimeZone Feature start
        public void GetTripActivityReport_NewTZ(string licensePlate,
                                string fromDateTime,
                                string toDateTime,
                                DataTable tblVehicleSensors,
                                DataTable tblVehicleGeozones,
                                double carCost,
                                int userId,
                                double measurementUnits,
                                DataSet dsResult,
                                bool showLastStoredPosition,
                                short vehicleType,
                                int sensorId,
                                string lang,
                                ref bool requestOverflowed,
                                ref int totalSqlRecords)
        {
            //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
            //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
            //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
            DataSet dsHistory = GetMsgInHistoryInformation_NewTZ(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
            if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
            {
                DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
                ReportVehicleTrip report = new ReportVehicleTrip(GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100), sqlExec);

                report.FillReport(tblVehicleSensors,
                               tblVehicleGeozones,
                               true,
                               false,
                               false,
                               false,
                               showLastStoredPosition,
                               carCost,
                               dsHistory,
                               measurementUnits,
                               Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                               userId, sensorId, vehicleType, lang);

                // rows 
                dsResult.Tables.Add(report.TripsStart);
                dsResult.Tables.Add(report.TripsStopsDuration);
                dsResult.Tables.Add(report.TripsDuration);
                dsResult.Tables.Add(report.TripsDistance);
                dsResult.Tables.Add(report.TripsCost);
                dsResult.Tables.Add(report.TripsCost);

                //dsResult.Tables.Add(report.TripsEnd);
                //dsResult.Tables.Add(report.TripsAverageSpeed);
                dsResult.Tables.Add(GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd));
            }
        }
        // Changes for TimeZone Feature end
        

        public void GetTripActivityReport(string licensePlate,
                                  string fromDateTime,
                                  string toDateTime,
                                  DataTable tblVehicleSensors,
                                  DataTable tblVehicleGeozones,
                                  double carCost,
                                  int userId,
                                  double measurementUnits,
                                  DataSet dsResult,
                                  bool showLastStoredPosition,
                                  short vehicleType,
                                  int sensorId,
                                  string lang,
                                  ref bool requestOverflowed,
                                  ref int totalSqlRecords)
        {
            //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
            //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
            //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
            DataSet dsHistory = GetMsgInHistoryInformation(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
            if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
            {
                DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
                ReportVehicleTrip report = new ReportVehicleTrip(GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100), sqlExec);

                report.FillReport(tblVehicleSensors,
                               tblVehicleGeozones,
                               true,
                               false,
                               false,
                               false,
                               showLastStoredPosition,
                               carCost,
                               dsHistory,
                               measurementUnits,
                               Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                               userId, sensorId, vehicleType, lang);

                // rows 
                dsResult.Tables.Add(report.TripsStart);
                dsResult.Tables.Add(report.TripsStopsDuration);
                dsResult.Tables.Add(report.TripsDuration);
                dsResult.Tables.Add(report.TripsDistance);
                dsResult.Tables.Add(report.TripsCost);
                dsResult.Tables.Add(report.TripsCost);

                //dsResult.Tables.Add(report.TripsEnd);
                //dsResult.Tables.Add(report.TripsAverageSpeed);
                dsResult.Tables.Add(GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd));
            }
        }

        public void GetTripReport(string licensePlate,
                                   string fromDateTime,
                                   string toDateTime,
                                   DataTable tblVehicleSensors,
                                   DataTable tblVehicleGeozones,
                                   double carCost,
                                   int userId,
                                   double measurementUnits,
                                   DataSet dsResult,
                                   bool showLastStoredPosition,
                                   short vehicleType, string lang,
                                   ref bool requestOverflowed,
                                   ref int totalSqlRecords)
        {
            //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
            //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
            //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
            DataSet dsHistory = GetMsgInHistoryInformation(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
            if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
            {
                DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
                ReportVehicleTrip report = new ReportVehicleTrip(GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100), sqlExec);

                report.FillReport(tblVehicleSensors,
                               tblVehicleGeozones,
                               true,
                               false,
                               false,
                               false,
                               showLastStoredPosition,
                               carCost,
                               dsHistory,
                               measurementUnits,
                               Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                               userId, vehicleType, lang);

                // rows 
                dsResult.Tables.Add(report.TripsStart);
                dsResult.Tables.Add(report.TripsEnd);
                dsResult.Tables.Add(report.TripsDuration);
                dsResult.Tables.Add(report.TripsDistance);
                dsResult.Tables.Add(report.TripsStopsDuration);
                // total
                dsResult.Tables.Add(report.TripsAverageSpeed);
                dsResult.Tables.Add(report.TripsCost);
                dsResult.Tables.Add(report.TripsFuelCons);

                dsResult.Tables.Add(GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd));
            }
        }

        // Changes for TimeZone Feature start
        public void GetTripReport_NewTZ(string licensePlate,
                                      string fromDateTime,
                                      string toDateTime,
                                      DataTable tblVehicleSensors,
                                      DataTable tblVehicleGeozones,
                                      double carCost,
                                      int userId,
                                      double measurementUnits,
                                      DataSet dsResult,
                                      bool showLastStoredPosition,
                                      short vehicleType,
                                      int sensorId,
                                      string lang,
                                      ref bool requestOverflowed,
                                      ref int totalSqlRecords)
        {
            //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
            //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
            //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
            DataSet dsHistory = GetMsgInHistoryInformation_NewTZ(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
            if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
            {
                DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
                ReportVehicleTrip report = new ReportVehicleTrip(GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100), sqlExec);

                report.FillReport(tblVehicleSensors,
                               tblVehicleGeozones,
                               true,
                               false,
                               false,
                               false,
                               showLastStoredPosition,
                               carCost,
                               dsHistory,
                               measurementUnits,
                               Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                               userId, sensorId, vehicleType, lang);

                // rows 
                dsResult.Tables.Add(report.TripsStart);
                dsResult.Tables.Add(report.TripsEnd);
                dsResult.Tables.Add(report.TripsDuration);
                dsResult.Tables.Add(report.TripsDistance);
                dsResult.Tables.Add(report.TripsStopsDuration);
                // total
                dsResult.Tables.Add(report.TripsAverageSpeed);
                dsResult.Tables.Add(report.TripsCost);
                dsResult.Tables.Add(report.TripsFuelCons);
                dsResult.Tables.Add(GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd));
            }
        }
        // Changes for TimeZone Feature end

        public void GetTripReport(string licensePlate,
                                         string fromDateTime,
                                         string toDateTime,
                                         DataTable tblVehicleSensors,
                                         DataTable tblVehicleGeozones,
                                         double carCost,
                                         int userId,
                                         double measurementUnits,
                                         DataSet dsResult,
                                         bool showLastStoredPosition,
                                         short vehicleType,
                                         int sensorId,
                                         string lang,
                                         ref bool requestOverflowed,
                                         ref int totalSqlRecords)
        {
            //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
            //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
            //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
            DataSet dsHistory = GetMsgInHistoryInformation(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
            if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
            {
                DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
                ReportVehicleTrip report = new ReportVehicleTrip(GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100), sqlExec);

                report.FillReport(tblVehicleSensors,
                               tblVehicleGeozones,
                               true,
                               false,
                               false,
                               false,
                               showLastStoredPosition,
                               carCost,
                               dsHistory,
                               measurementUnits,
                               Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                               userId, sensorId, vehicleType, lang);

                // rows 
                dsResult.Tables.Add(report.TripsStart);
                dsResult.Tables.Add(report.TripsEnd);
                dsResult.Tables.Add(report.TripsDuration);
                dsResult.Tables.Add(report.TripsDistance);
                dsResult.Tables.Add(report.TripsStopsDuration);
                // total
                dsResult.Tables.Add(report.TripsAverageSpeed);
                dsResult.Tables.Add(report.TripsCost);
                dsResult.Tables.Add(report.TripsFuelCons);
                dsResult.Tables.Add(GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd));
            }
        }

        public void GetDetailedTripReport(string licensePlate,
                          string fromDateTime,
                          string toDateTime,
                          DataTable tblVehicleSensors,
                          DataTable tblVehicleGeozones,
                          bool includeStreetAddress,
                          bool includeSensors,
                          bool includePosition,
                          bool includeIdleTime,
                          bool includeSummary,
                          bool showLastStoredPosition,
                          double carCost,
                          int userId,
                          double measurementUnits,
                          DataSet dsResult,
                          short vehicleType, string lang,
                          ref bool requestOverflowed,
                          ref int totalSqlRecords)
        {
            //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
            //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
            //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
            DataSet dsHistory = GetMsgInHistoryInformation(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
            if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
            {
                ReportVehicleTrip report = new ReportVehicleTrip(GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100), sqlExec);

                report.FillReport(tblVehicleSensors,
                            tblVehicleGeozones,
                            includeStreetAddress,
                            includeSensors,
                            includePosition,
                            includeIdleTime,
                            showLastStoredPosition,
                            carCost,
                            dsHistory,
                            measurementUnits,
                            Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                            userId, vehicleType, lang);
                dsResult.Tables.Add(report.TripReportDetailes);
                if (includeSummary == true)
                {
                    dsResult.Tables.Add(report.TripsStart);
                    dsResult.Tables.Add(report.TripsAverageSpeed);
                    dsResult.Tables.Add(report.TripsCost);
                    dsResult.Tables.Add(report.TripsDistance);
                    dsResult.Tables.Add(report.TripsDuration);
                    dsResult.Tables.Add(report.TripsStopsDuration);
                    dsResult.Tables.Add(report.TripsFuelCons);
                    dsResult.Tables.Add(report.TripsEnd);
                    if (report.TripsEnd.Rows.Count > 0)
                    {
                        dsResult.Tables.Add(GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd));
                    }
                }
            }
        }

        // Changes for TimeZone Feature start
        public void GetDetailedTripReport_NewTZ(string licensePlate,
                        string fromDateTime,
                        string toDateTime,
                        DataTable tblVehicleSensors,
                        DataTable tblVehicleGeozones,
                        bool includeStreetAddress,
                        bool includeSensors,
                        bool includePosition,
                        bool includeIdleTime,
                        bool includeSummary,
                        bool showLastStoredPosition,
                        double carCost,
                        int userId,
                        double measurementUnits,
                        DataSet dsResult,
                        short vehicleType,
                        int sensorId,
                        string lang,
                        ref bool requestOverflowed,
                        ref int totalSqlRecords)
        {
            //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
            //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
            //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
            DataSet dsHistory = GetMsgInHistoryInformation_NewTZ(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
            //DataSet dsHistory = GetMsgInHistoryInformationBrickman  (licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
            if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
            {
                ReportVehicleTrip report = new ReportVehicleTrip(GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100), sqlExec);

                report.FillReport(tblVehicleSensors,
                            tblVehicleGeozones,
                            includeStreetAddress,
                            includeSensors,
                            includePosition,
                            includeIdleTime,
                            showLastStoredPosition,
                            carCost,
                            dsHistory,
                            measurementUnits,
                            Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                            userId, sensorId, vehicleType, lang);
                dsResult.Tables.Add(report.TripReportDetailes);
                if (includeSummary == true)
                {
                    dsResult.Tables.Add(report.TripsStart);
                    dsResult.Tables.Add(report.TripsAverageSpeed);
                    dsResult.Tables.Add(report.TripsCost);
                    dsResult.Tables.Add(report.TripsDistance);
                    dsResult.Tables.Add(report.TripsDuration);
                    dsResult.Tables.Add(report.TripsStopsDuration);
                    dsResult.Tables.Add(report.TripsFuelCons);
                    dsResult.Tables.Add(report.TripsEnd);
                    if (report.TripsEnd.Rows.Count > 0)
                    {
                        dsResult.Tables.Add(GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd));
                    }
                }
            }
        }
        // Changes for TimeZone feature end

        public void GetDetailedTripReport(string licensePlate,
                                string fromDateTime,
                                string toDateTime,
                                DataTable tblVehicleSensors,
                                DataTable tblVehicleGeozones,
                                bool includeStreetAddress,
                                bool includeSensors,
                                bool includePosition,
                                bool includeIdleTime,
                                bool includeSummary,
                                bool showLastStoredPosition,
                                double carCost,
                                int userId,
                                double measurementUnits,
                                DataSet dsResult,
                                short vehicleType,
                                int sensorId,
                                string lang,
                                ref bool requestOverflowed,
                                ref int totalSqlRecords)
        {
            //[BoxId],[DateTimeReceived],[BoxMsgInTypeId],[MsgType],
            //[BoxProtocolTypeId],[ProtocolType],[OriginDateTime],[ValidGps],
            //[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress]
            DataSet dsHistory = GetMsgInHistoryInformation(licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
            //DataSet dsHistory = GetMsgInHistoryInformationBrickman  (licensePlate, fromDateTime, toDateTime, userId, null, ref requestOverflowed, ref totalSqlRecords);
            if ((dsHistory != null) && (dsHistory.Tables.Count > 0) && (dsHistory.Tables[0].Rows.Count > 0))
            {
                ReportVehicleTrip report = new ReportVehicleTrip(GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS, "Idling Range", (short)100), sqlExec);

                report.FillReport(tblVehicleSensors,
                            tblVehicleGeozones,
                            includeStreetAddress,
                            includeSensors,
                            includePosition,
                            includeIdleTime,
                            showLastStoredPosition,
                            carCost,
                            dsHistory,
                            measurementUnits,
                            Convert.ToInt64(dsHistory.Tables[0].Rows[0]["VehicleId"]),
                            userId, sensorId, vehicleType, lang);
                dsResult.Tables.Add(report.TripReportDetailes);
                if (includeSummary == true)
                {
                    dsResult.Tables.Add(report.TripsStart);
                    dsResult.Tables.Add(report.TripsAverageSpeed);
                    dsResult.Tables.Add(report.TripsCost);
                    dsResult.Tables.Add(report.TripsDistance);
                    dsResult.Tables.Add(report.TripsDuration);
                    dsResult.Tables.Add(report.TripsStopsDuration);
                    dsResult.Tables.Add(report.TripsFuelCons);
                    dsResult.Tables.Add(report.TripsEnd);
                    if (report.TripsEnd.Rows.Count > 0)
                    {
                        dsResult.Tables.Add(GetStopDurationBetweenTrips(report.TripsStart, report.TripsEnd));
                    }
                }
            }
        }
#endif

        /// <summary>
        /// Retrieves alarm report. 
        /// </summary>
        /// <returns>
        /// DataSet [AlarmId],[DateTimeCreated],[AlarmSeverity],[AlarmType],
        /// [DateTimeAck],[DateTimeClosed],[Description],
        /// [BoxId],[OriginDateTime],[ValidGps],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],
        /// [BoxProtocolTypeId],[BoxProtocolTypeName]
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress],
        /// [UserId],[UserName],[DriverLicense],[FirstName],[LastName]
        /// </returns>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetAlarmReport(int userId, int boxId, string fromDateTime, string toDateTime, ref bool isOverflowed)
        {


            DataSet resultDataSet = new DataSet();
            string sql = "";

            sqlExec.CommandTimeout = 300;
            sqlExec.ClearCommandParameters();
            SqlParameter[] paramList = new SqlParameter[5];
            int paramIndex = 0;
            paramList[paramIndex] = new SqlParameter("@userId", userId); paramList[paramIndex].Direction = ParameterDirection.Input;
            paramIndex++;
            paramList[paramIndex] = new SqlParameter("@boxId", boxId); paramList[paramIndex].Direction = ParameterDirection.Input;
            paramIndex++;
            paramList[paramIndex] = new SqlParameter("@fromDateTime", fromDateTime); paramList[paramIndex].Direction = ParameterDirection.Input;
            paramIndex++;
            paramList[paramIndex] = new SqlParameter("@toDateTime", toDateTime); paramList[paramIndex].Direction = ParameterDirection.Input;
            paramIndex++;
            paramList[paramIndex] = new SqlParameter("@overflowed", isOverflowed); paramList[paramIndex].Direction = ParameterDirection.Output;

            //sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
            //sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
            //sqlExec.AddCommandParam("@fromDateTime", SqlDbType.DateTime, fromDateTime);
            //sqlExec.AddCommandParam("@toDateTime", SqlDbType.DateTime, toDateTime);

            // sql = "ReportAlarms4Fleet";
            sql = "ReportAlarms4VehicleNew";


            if (sqlExec.RequiredTransaction())
                sqlExec.AttachToTransaction(sql);

            return sqlExec.SPExecuteDataset(sql, paramList);


            //DataSet resultDataSet = null;
            //requestOverflowed = false;
            //string whereFilter = "";
            //try
            //{
            //   if (fromDateTime != VLF.CLS.Def.Const.unassignedStrValue)
            //      whereFilter = " AND (vlfMsgInHst.OriginDateTime>='" + fromDateTime + "')";
            //   if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
            //      whereFilter += " AND (vlfMsgInHst.OriginDateTime<='" + toDateTime + "')";



            //   string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone int" +
            //               " DECLARE @Unit real" +
            //               " DECLARE @DayLightSaving int" +
            //               " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
            //               " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
            //               " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference" +
            //               " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.MeasurementUnits +
            //               " IF @Timezone IS NULL SET @Timezone=0" +
            //               " IF @Unit IS NULL SET @Unit=1" +
            //               " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
            //               " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
            //               " SET @Timezone= @Timezone + @DayLightSaving" +
            //               " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0";


            //   string sqlSelectFooter = "SELECT vlfAlarm.AlarmId," +
            //            "CASE WHEN vlfAlarm.DateTimeCreated IS NOT NULL then DATEADD(hour,@Timezone,vlfAlarm.DateTimeCreated) END AS DateTimeCreated," +
            //              "vlfAlarm.AlarmType,convert(smallint,dbo.vlfAlarm.AlarmLevel) AS AlarmSeverity, vlfSeverity.AlarmLevelName as AlarmSeverityName, " +
            //            "CASE WHEN vlfAlarm.DateTimeAck IS NOT NULL then DATEADD(hour,@Timezone,vlfAlarm.DateTimeAck) END AS DateTimeAck," +
            //            "CASE WHEN vlfAlarm.DateTimeClosed IS NOT NULL then DATEADD(hour,@Timezone,vlfAlarm.DateTimeClosed) END AS DateTimeClosed," +
            //            "ISNULL(vlfAlarm.[Description],' ') AS [Description]," +
            //            "vlfAlarm.BoxId,vlfMsgInHst.OriginDateTime,vlfMsgInHst.ValidGps," +
            //            "vlfMsgInHst.BoxMsgInTypeId,vlfBoxMsgInType.BoxMsgInTypeName," +
            //            "vlfMsgInHst.BoxProtocolTypeId,vlfBoxProtocolType.BoxProtocolTypeName," +
            //            "CASE WHEN vlfMsgInHst.ValidGps=1 then 0 ELSE vlfMsgInHst.Latitude END AS Latitude," +
            //            "CASE WHEN vlfMsgInHst.ValidGps=1 then 0 ELSE vlfMsgInHst.Longitude END AS Longitude," +
            //            "CASE WHEN vlfMsgInHst.ValidGps=1 then '" + VLF.CLS.Def.Const.blankValue + "' ELSE convert(nvarchar,ROUND(vlfMsgInHst.Speed * @Unit,1)) END AS Speed," +
            //            "CASE WHEN vlfMsgInHst.ValidGps=1 then '" + VLF.CLS.Def.Const.blankValue + "' ELSE convert(nvarchar,vlfMsgInHst.Heading) END AS Heading," +
            //            "vlfMsgInHst.SensorMask,vlfMsgInHst.CustomProp," +

            //                   "CASE WHEN vlfMsgInHst.ValidGps=1 then '" + VLF.CLS.Def.Const.noValidAddress + "' ELSE ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') END AS StreetAddress," +

            //            "vlfAlarm.UserId," +
            //            "ISNULL(vlfUser.UserName,'" + VLF.CLS.Def.Const.blankValue + "') AS UserName," +
            //            "ISNULL(vlfPersonInfo.DriverLicense,'" + VLF.CLS.Def.Const.blankValue + "') AS DriverLicense," +
            //            "ISNULL(vlfPersonInfo.FirstName,'" + VLF.CLS.Def.Const.blankValue + "') AS FirstName," +
            //            "ISNULL(vlfPersonInfo.LastName,'" + VLF.CLS.Def.Const.blankValue + "') AS LastName";
            //   string sqlFooter = "FROM  vlfMsgInHst" +
            //      " INNER JOIN vlfAlarm ON vlfMsgInHst.BoxId=vlfAlarm.BoxId" +
            //            " AND vlfMsgInHst.OriginDateTime=vlfAlarm.DateTimeCreated" +
            //      " INNER JOIN vlfBoxProtocolType ON vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId" +
            //      " INNER JOIN vlfBoxMsgInType ON vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId" +
            //      " INNER JOIN vlfSeverity ON vlfAlarm.AlarmLevel = vlfSeverity.AlarmLevel "+
            //      " LEFT JOIN vlfUser ON vlfAlarm.UserId=vlfUser.UserId" +
            //      " LEFT JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId" +
            //      " WHERE vlfMsgInHst.BoxId=" + boxId + whereFilter;
            //   string sql = sqlHeader + " " + sqlSelectFooter + " " + sqlFooter;
            //   try
            //   {
            //      int sqlTop = GetConfigParameter("ASI", (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Report Max Records", 1000);
            //      if (sqlTop != VLF.CLS.Def.Const.unassignedIntValue)
            //      {
            //         totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter));
            //         if ((sqlTop - totalSqlRecords) < 0)
            //         {
            //            requestOverflowed = true;
            //            return null;
            //         }
            //      }
            //   }
            //   catch (DASDbConnectionClosed exCnn)
            //   {
            //      throw new DASDbConnectionClosed(exCnn.Message);
            //   }
            //   catch (Exception exp)
            //   {
            //      throw new DASException("Unable to retieve messages from history" + exp.Message);
            //   }
            //   sql += " ORDER BY vlfAlarm.DateTimeCreated DESC";
            //   //Executes SQL statement
            //   sqlExec.CommandTimeout = 600;
            //   resultDataSet = sqlExec.SQLExecuteDataset(sql);
            //}
            //catch (SqlException objException)
            //{
            //   string prefixMsg = "Unable to retrieve alarms report by box id=" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
            //   Util.ProcessDbException(prefixMsg, objException);
            //}
            //catch (DASDbConnectionClosed exCnn)
            //{
            //   throw new DASDbConnectionClosed(exCnn.Message);
            //}
            //catch (Exception objException)
            //{
            //   string prefixMsg = "Unable to retrieve alarms report by box id=" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
            //   throw new DASException(prefixMsg + " " + objException.Message);
            //}
            return resultDataSet;
        }

        /// <summary>
        /// Get vehicle alarms report
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licPlate"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="IsOverflowed"></param>
        /// <returns></returns>
        public DataSet GetVehicleAlarmReport(int userId, string licPlate, string fromDateTime, string toDateTime)
        {
            DataSet dsFleetAlarms = new DataSet("BoxAlarmsReport");
            string sql = "";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@licPlate", SqlDbType.VarChar, licPlate, 20);
            sqlExec.AddCommandParam("@fromDateTime", SqlDbType.DateTime, fromDateTime);
            sqlExec.AddCommandParam("@toDateTime", SqlDbType.DateTime, toDateTime);

            sql = "ReportAlarms4Vehicle";

            if (sqlExec.RequiredTransaction())
                sqlExec.AttachToTransaction(sql);

            return sqlExec.SPExecuteDataset(sql);

        }

        /// <summary>
        /// Get fleet alarms report
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="fleetId">Fleet Id</param>
        /// <param name="fromDateTime">From dt</param>
        /// <param name="toDateTime">To dt</param>
        /// <param name="IsOverflowed">True if too many records to get</param>
        /// <returns>Alarms DataSet</returns>
        public DataSet GetFleetAlarmReport(int userId, int fleetId, string fromDateTime, string toDateTime, ref bool isOverflowed)
        {
            DataSet dsFleetAlarms = new DataSet("FleetAlarmsReport");
            string sql = "";

            sqlExec.CommandTimeout = 300;
            sqlExec.ClearCommandParameters();
            SqlParameter[] paramList = new SqlParameter[5];
            int paramIndex = 0;
            paramList[paramIndex] = new SqlParameter("@userId", userId); paramList[paramIndex].Direction = ParameterDirection.Input;
            paramIndex++;
            paramList[paramIndex] = new SqlParameter("@fleetId", fleetId); paramList[paramIndex].Direction = ParameterDirection.Input;
            paramIndex++;
            paramList[paramIndex] = new SqlParameter("@fromDateTime", fromDateTime); paramList[paramIndex].Direction = ParameterDirection.Input;
            paramIndex++;
            paramList[paramIndex] = new SqlParameter("@toDateTime", toDateTime); paramList[paramIndex].Direction = ParameterDirection.Input;
            paramIndex++;
            paramList[paramIndex] = new SqlParameter("@overflowed", isOverflowed); paramList[paramIndex].Direction = ParameterDirection.Output;

            //sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
            //sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
            //sqlExec.AddCommandParam("@fromDateTime", SqlDbType.DateTime, fromDateTime);
            //sqlExec.AddCommandParam("@toDateTime", SqlDbType.DateTime, toDateTime);

            // sql = "ReportAlarms4Fleet";
            sql = "ReportAlarms4FleetNew";


            if (sqlExec.RequiredTransaction())
                sqlExec.AttachToTransaction(sql);

            return sqlExec.SPExecuteDataset(sql, paramList);
        }

        /// <summary>
        /// Retrieves system usage exseption report. 
        /// </summary>
        /// <returns>
        /// DataSet [OrganizationName],[Description],[BoxId],[MaxMsgs],[MaxTxtMsgs],[BoxProtocolTypeName],[OrganizationId],[TotalMsgInSize],[TotalMsgOutSize]
        /// </returns>
        /// <remarks> order by OrganizationName</remarks>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetSystemUsageExceptionReportForAllOrganizations(string fromDateTime, string toDateTime)
        {
            int prevCommandTimeout = sqlExec.CommandTimeout;

            DataSet resultDataSet = null;
            string msgInDateTimeFilter = "";
            string msgOutDateTimeFilter = "";
            try
            {
                if (fromDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                {
                    msgInDateTimeFilter = "DateTimeReceived>='" + fromDateTime + "' AND ";
                    msgOutDateTimeFilter = "vlfMsgOutHst.[DateTime]>='" + fromDateTime + "'";
                    if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                    {
                        msgInDateTimeFilter += " DateTimeReceived<='" + toDateTime + "' AND ";
                        msgOutDateTimeFilter += " AND vlfMsgOutHst.[DateTime]<='" + toDateTime + "'";
                    }
                }
                else if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                {
                    msgInDateTimeFilter += "DateTimeReceived<='" + toDateTime + "' AND ";
                    msgOutDateTimeFilter += "vlfMsgOutHst.[DateTime]<='" + toDateTime + "'";
                }
                string sql = "CREATE TABLE #tmpSysUsage(DT datetime,[Description] nvarchar(100),BoxId int,MaxMsgs int,MaxTxtMsgs int,ProtocolTypeName nvarchar(100),MsgInType int,MsgIn int,MsgOutType int,MsgOut int,OrganizationId int)" +
                    //--vlfMsgInHst
                   " INSERT INTO #tmpSysUsage SELECT DISTINCT DateTimeReceived,ISNULL(vlfVehicleInfo.Description,'N/A') AS [Description], vlfBox.BoxId,ISNULL(MaxMsgs,0) AS MaxMsgs,ISNULL(MaxTxtMsgs,0) AS MaxTxtMsgs,ISNULL(vlfBoxProtocolType.BoxProtocolTypeName,'N/A') AS ProtocolTypeName,ISNULL(vlfBoxProtocolTypeMsgInType.BoxMsgInTypeId,-1) AS MsgInType,ISNULL(vlfBoxProtocolTypeMsgInType.MsgInTypeLen,0) AS MsgIn,-1 as MsgOutType,0 as MsgOut,vlfBox.OrganizationId" +
                   " FROM vlfVehicleInfo INNER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId" +
                   " RIGHT OUTER JOIN vlfMsgInHst with (nolock) INNER JOIN vlfBox ON vlfMsgInHst.BoxId = vlfBox.BoxId" +
                   " INNER JOIN vlfBoxMsgInType ON vlfMsgInHst.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId" +
                   " INNER JOIN vlfBoxProtocolType ON vlfMsgInHst.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " INNER JOIN vlfBoxProtocolTypeMsgInType ON vlfBoxMsgInType.BoxMsgInTypeId = vlfBoxProtocolTypeMsgInType.BoxMsgInTypeId AND vlfBoxProtocolType.BoxProtocolTypeId = vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId ON vlfVehicleAssignment.BoxId = vlfBox.BoxId" +
                   " LEFT JOIN vlfBoxSettings ON vlfBoxSettings.BoxId = vlfBox.BoxId AND vlfBoxSettings.BoxProtocolTypeId = vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId" +
                   " WHERE " + msgInDateTimeFilter +
                   " vlfMsgInHst.BoxMsgInTypeId<>" + (short)Enums.MessageType.MDTSpecialMessage +

                   //--vlfMsgInHstIgnored
                   " INSERT INTO #tmpSysUsage SELECT DISTINCT DateTimeReceived,ISNULL(vlfVehicleInfo.Description,'N/A') AS [Description], vlfBox.BoxId,ISNULL(MaxMsgs,0) AS MaxMsgs,ISNULL(MaxTxtMsgs,0) AS MaxTxtMsgs,ISNULL(vlfBoxProtocolType.BoxProtocolTypeName,'-----') AS ProtocolTypeName,ISNULL(vlfBoxProtocolTypeMsgInType.BoxMsgInTypeId,-1) AS MsgInType,ISNULL(vlfBoxProtocolTypeMsgInType.MsgInTypeLen,0) AS MsgIn,-1 as MsgOutType,0 as MsgOut,vlfBox.OrganizationId" +
                   " FROM vlfVehicleInfo INNER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId" +
                   " RIGHT OUTER JOIN vlfMsgInHstIgnored INNER JOIN vlfBox ON vlfMsgInHstIgnored.BoxId = vlfBox.BoxId" +
                   " INNER JOIN vlfBoxMsgInType ON vlfMsgInHstIgnored.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId" +
                   " INNER JOIN vlfBoxProtocolType ON vlfMsgInHstIgnored.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " INNER JOIN vlfBoxProtocolTypeMsgInType ON vlfBoxMsgInType.BoxMsgInTypeId = vlfBoxProtocolTypeMsgInType.BoxMsgInTypeId AND vlfBoxProtocolType.BoxProtocolTypeId = vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId ON vlfVehicleAssignment.BoxId = vlfBox.BoxId" +
                   " LEFT JOIN vlfBoxSettings ON vlfBoxSettings.BoxId = vlfBox.BoxId AND vlfBoxSettings.BoxProtocolTypeId = vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId" +
                   " WHERE " + msgInDateTimeFilter +
                   " vlfMsgInHstIgnored.BoxMsgInTypeId<>" + (short)Enums.MessageType.MDTSpecialMessage +

                   //--vlfMsgOutHst
                   " INSERT INTO #tmpSysUsage SELECT DISTINCT vlfMsgOutHst.[DateTime],ISNULL(vlfVehicleInfo.Description,'N/A') AS [Description], vlfBox.BoxId,ISNULL(MaxMsgs,0) AS MaxMsgs,ISNULL(MaxTxtMsgs,0) AS MaxTxtMsgs,ISNULL(vlfBoxProtocolType.BoxProtocolTypeName,'-----') AS ProtocolTypeName,-1 AS MsgInType,0 AS MsgIn,ISNULL(vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId,-1) AS MsgOutType,ISNULL(vlfBoxProtocolTypeCmdOutType.CmdOutTypeLen,0) AS MsgOut,vlfBox.OrganizationId" +
                   " FROM vlfVehicleInfo INNER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId" +
                   " RIGHT OUTER JOIN vlfBox INNER JOIN vlfMsgOutHst ON vlfBox.BoxId = vlfMsgOutHst.BoxId" +
                   " INNER JOIN vlfBoxCmdOutType ON vlfMsgOutHst.BoxCmdOutTypeId = vlfBoxCmdOutType.BoxCmdOutTypeId" +
                   " INNER JOIN vlfBoxProtocolType ON vlfMsgOutHst.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " INNER JOIN vlfBoxProtocolTypeCmdOutType ON vlfBoxCmdOutType.BoxCmdOutTypeId = vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId AND vlfBoxProtocolType.BoxProtocolTypeId = vlfBoxProtocolTypeCmdOutType.BoxProtocolTypeId ON vlfVehicleAssignment.BoxId = vlfBox.BoxId" +
                   " LEFT JOIN vlfBoxSettings ON vlfBoxSettings.BoxId = vlfBox.BoxId AND vlfBoxSettings.BoxProtocolTypeId = vlfBoxProtocolTypeCmdOutType.BoxProtocolTypeId" +
                   " WHERE " + msgOutDateTimeFilter +

                   //------------------------ IN -------------------------------------
                    //--IN MSGS without Ack and all TxtMsgs
                   " SELECT Description,BoxId,ProtocolTypeName,Count(MsgIn) AS TotalInMsgs" +
                   " INTO #tmpSysUsageInCount FROM #tmpSysUsage" +
                   " WHERE MsgInType<>" + (short)Enums.MessageType.MDTAck +
                   " AND MsgInType<>" + (short)Enums.MessageType.MDTResponse +
                    //" AND MsgInType<>" + (short)Enums.MessageType.MDTSpecialMessage +
                   " AND MsgInType<>" + (short)Enums.MessageType.MDTTextMessage +
                   " AND MsgInType<>-1 AND MsgInType<>" + (short)Enums.MessageType.Ack +
                   " GROUP BY Description,BoxId,ProtocolTypeName" +

                   //--IN MSGS without all TxtMsgs
                   " SELECT Description,BoxId,ProtocolTypeName,SUM(MsgIn) AS TotalInBytes" +
                   " INTO #tmpSysUsageInByte FROM #tmpSysUsage" +
                   " WHERE MsgInType<>" + (short)Enums.MessageType.MDTAck +
                   " AND MsgInType<>" + (short)Enums.MessageType.MDTResponse +
                    //" AND MsgInType<>" + (short)Enums.MessageType.MDTSpecialMessage +
                   " AND MsgInType<>" + (short)Enums.MessageType.MDTTextMessage +
                   " AND MsgInType<>-1 GROUP BY Description,BoxId,ProtocolTypeName" +

                   //--IN TXT MSGS without MDTAck
                   " SELECT Description,BoxId,ProtocolTypeName,Count(MsgIn) AS TotalInTxtMsgs" +
                   " INTO #tmpSysUsageInTxtCount FROM #tmpSysUsage" +
                   " WHERE MsgInType=" + (short)Enums.MessageType.MDTResponse +
                    //" OR MsgInType=" + (short)Enums.MessageType.MDTSpecialMessage +
                   " OR MsgInType=" + (short)Enums.MessageType.MDTTextMessage +
                   " GROUP BY Description,BoxId,ProtocolTypeName" +

                   //--IN MSGS with all TxtMsgs only
                   " SELECT Description,BoxId,ProtocolTypeName,SUM(MsgIn) AS TotalInTxtBytes" +
                   " INTO #tmpSysUsageInTxtByte FROM #tmpSysUsage" +
                   " WHERE MsgInType=" + (short)Enums.MessageType.MDTAck +
                   " OR MsgInType=" + (short)Enums.MessageType.MDTResponse +
                    //" OR MsgInType=" + (short)Enums.MessageType.MDTSpecialMessage +
                   " OR MsgInType=" + (short)Enums.MessageType.MDTTextMessage +
                   " GROUP BY Description,BoxId,ProtocolTypeName" +

                   //---------------------- OUT ------------------------------
                    //-- do not include ACK and all TxtMsgs
                   " SELECT Description,BoxId,ProtocolTypeName,Count(MsgOut) AS TotalOutMsgs" +
                   " INTO #tmpSysUsageOutCount FROM #tmpSysUsage" +
                   " WHERE MsgOutType<>" + (short)Enums.CommandType.MDTAck +
                   " AND MsgOutType<>" + (short)Enums.CommandType.MDTTextMessage +
                   " AND MsgOutType<>-1" +
                   " AND MsgOutType<>" + (short)Enums.CommandType.Ack +
                   " GROUP BY Description,BoxId,ProtocolTypeName" +

                   //--do not include all TextMsgs
                   " SELECT Description,BoxId,ProtocolTypeName,SUM(MsgOut) AS TotalOutBytes" +
                   " INTO #tmpSysUsageOutByte FROM #tmpSysUsage" +
                   " WHERE MsgOutType<>" + (short)Enums.CommandType.MDTAck +
                   " AND MsgOutType<>" + (short)Enums.CommandType.MDTTextMessage +
                   " AND MsgOutType<>-1" +
                   " GROUP BY Description,BoxId,ProtocolTypeName" +

                   //-- Only TxtMsgs,do not include MDTAck
                   " SELECT Description,BoxId,ProtocolTypeName,Count(MsgOut) AS TotalOutTxtMsgs" +
                   " INTO #tmpSysUsageOutTxtCount FROM #tmpSysUsage" +
                   " WHERE MsgOutType=" + (short)Enums.CommandType.MDTTextMessage +
                   " GROUP BY Description,BoxId,ProtocolTypeName" +

                   //--include all TextMsgs only
                   " SELECT Description,BoxId,ProtocolTypeName,SUM(MsgOut) AS TotalOutTxtBytes" +
                   " INTO #tmpSysUsageOutTxtByte FROM #tmpSysUsage" +
                   " WHERE MsgOutType=" + (short)Enums.CommandType.MDTAck +
                   " OR MsgOutType=" + (short)Enums.CommandType.MDTTextMessage +
                   " GROUP BY Description,BoxId,ProtocolTypeName" +

                   //----------------------------------------------------------------
                   " SELECT DISTINCT #tmpSysUsage.Description,#tmpSysUsage.BoxId,MaxMsgs,MaxTxtMsgs,#tmpSysUsage.ProtocolTypeName,OrganizationId," +
                   " ISNULL(TotalInBytes,0) as TotalInBytes," +
                   " ISNULL(TotalInMsgs,0) as TotalInMsgs," +
                   " ISNULL(TotalOutBytes,0) as TotalOutBytes," +
                   " ISNULL(TotalOutMsgs,0) as TotalOutMsgs," +
                   " ISNULL(TotalInTxtBytes,0) as TotalInTxtBytes," +
                   " ISNULL(TotalInTxtMsgs,0) as TotalInTxtMsgs," +
                   " ISNULL(TotalOutTxtBytes,0) as TotalOutTxtBytes," +
                   " ISNULL(TotalOutTxtMsgs,0) as TotalOutTxtMsgs" +
                   " INTO #tmpTotals FROM #tmpSysUsage" +
                   " LEFT JOIN #tmpSysUsageInCount ON #tmpSysUsageInCount.Description=#tmpSysUsage.Description AND #tmpSysUsageInCount.ProtocolTypeName=#tmpSysUsage.ProtocolTypeName" +
                   " LEFT JOIN #tmpSysUsageInByte ON #tmpSysUsageInByte.Description=#tmpSysUsage.Description AND #tmpSysUsageInByte.ProtocolTypeName=#tmpSysUsage.ProtocolTypeName" +
                   " LEFT JOIN #tmpSysUsageOutCount ON #tmpSysUsage.Description=#tmpSysUsageOutCount.Description AND #tmpSysUsage.ProtocolTypeName=#tmpSysUsageOutCount.ProtocolTypeName" +
                   " LEFT JOIN #tmpSysUsageOutByte ON #tmpSysUsage.Description=#tmpSysUsageOutByte.Description AND #tmpSysUsage.ProtocolTypeName=#tmpSysUsageOutByte.ProtocolTypeName" +
                   " LEFT JOIN #tmpSysUsageInTxtCount ON #tmpSysUsageInTxtCount.Description=#tmpSysUsage.Description AND #tmpSysUsageInTxtCount.ProtocolTypeName=#tmpSysUsage.ProtocolTypeName" +
                   " LEFT JOIN #tmpSysUsageInTxtByte ON #tmpSysUsageInTxtByte.Description=#tmpSysUsage.Description AND #tmpSysUsageInTxtByte.ProtocolTypeName=#tmpSysUsage.ProtocolTypeName" +
                   " LEFT JOIN #tmpSysUsageOutTxtCount ON #tmpSysUsage.Description=#tmpSysUsageOutTxtCount.Description AND #tmpSysUsage.ProtocolTypeName=#tmpSysUsageOutTxtCount.ProtocolTypeName" +
                   " LEFT JOIN #tmpSysUsageOutTxtByte ON #tmpSysUsage.Description=#tmpSysUsageOutTxtByte.Description AND #tmpSysUsage.ProtocolTypeName=#tmpSysUsageOutTxtByte.ProtocolTypeName" +

                   " SELECT *,SUM(ISNULL(TotalInBytes,0)) + SUM(ISNULL(TotalOutBytes,0)) as TotalBytes," +
                   " SUM(ISNULL(TotalInMsgs,0))+ SUM(ISNULL(TotalOutMsgs,0)) as TotalMsgs," +
                   " SUM(ISNULL(TotalInTxtBytes,0)) + SUM(ISNULL(TotalOutTxtBytes,0)) as TotalTxtBytes," +
                   " SUM(ISNULL(TotalInTxtMsgs,0))+ SUM(ISNULL(TotalOutTxtMsgs,0)) as TotalTxtMsgs" +
                   " INTO #tmpExceptionTotals from #tmpTotals" +
                   " GROUP BY OrganizationId,Description,BoxId,ProtocolTypeName,TotalInBytes,TotalInMsgs,TotalOutBytes,TotalOutMsgs," +
                   " TotalInTxtBytes,TotalInTxtMsgs,TotalOutTxtBytes,TotalOutTxtMsgs,MaxMsgs,MaxTxtMsgs" +

                   " SELECT OrganizationName,#tmpExceptionTotals.* FROM #tmpExceptionTotals INNER JOIN vlfOrganization ON vlfOrganization.OrganizationId = #tmpExceptionTotals.OrganizationId" +
                   " WHERE TotalMsgs>MaxMsgs or TotalTxtMsgs>MaxTxtMsgs GROUP BY OrganizationName,#tmpExceptionTotals.Description,#tmpExceptionTotals.BoxId,MaxMsgs,MaxTxtMsgs,ProtocolTypeName,#tmpExceptionTotals.OrganizationId,TotalInBytes,TotalInMsgs,TotalOutBytes,TotalOutMsgs,TotalInTxtBytes,TotalInTxtMsgs,TotalOutTxtBytes,TotalOutTxtMsgs,TotalBytes,TotalMsgs,TotalTxtBytes,TotalTxtMsgs" +
                   " ORDER BY OrganizationName" +

                   " DROP TABLE #tmpSysUsageInCount" +
                   " DROP TABLE #tmpSysUsageInByte" +
                   " DROP TABLE #tmpSysUsageInTxtCount" +
                   " DROP TABLE #tmpSysUsageInTxtByte" +
                   " DROP TABLE #tmpSysUsageOutCount" +
                   " DROP TABLE #tmpSysUsageOutByte" +
                   " DROP TABLE #tmpSysUsageOutTxtCount" +
                   " DROP TABLE #tmpSysUsageOutTxtByte" +
                   " DROP TABLE #tmpSysUsage" +
                   " DROP TABLE #tmpTotals" +
                   " DROP TABLE #tmpExceptionTotals";

                //Executes SQL statement
                sqlExec.CommandTimeout = 600;
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                sqlExec.CommandTimeout = prevCommandTimeout;
                string prefixMsg = "Unable to retrieve system usage exception report FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                sqlExec.CommandTimeout = prevCommandTimeout;
                string prefixMsg = "Unable to retrieve system usage exception report FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            sqlExec.CommandTimeout = prevCommandTimeout;
            return resultDataSet;
        }

        /// <summary>
        /// Retrieves system usage report. 
        /// </summary>
        /// <returns>
        /// DataSet [Description],[BoxId],[MaxMsgs],[MaxTxtMsgs],[BoxProtocolTypeName],[TotalMsgInSize],[TotalMsgOutSize]
        /// </returns>
        /// <param name="organizationId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="showExceptionOnly"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        [Obsolete("Not in use")]
        public DataSet GetSystemUsageReportByOrganization(int organizationId, string fromDateTime, string toDateTime, bool showExceptionOnly)
        {
            int prevCommandTimeout = sqlExec.CommandTimeout;

            DataSet resultDataSet = null;
            string msgInDateTimeFilter = "";
            string msgOutDateTimeFilter = "";
            try
            {
                if (fromDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                {
                    msgInDateTimeFilter = " AND DateTimeReceived >= '" + fromDateTime + "'";
                    msgOutDateTimeFilter = " AND vlfMsgOutHst.[DateTime]>='" + fromDateTime + "'";
                }
                if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                {
                    msgInDateTimeFilter += " AND DateTimeReceived<='" + toDateTime + "'";
                    msgOutDateTimeFilter += " AND vlfMsgOutHst.[DateTime]<='" + toDateTime + "'";
                }
                string sql = "CREATE TABLE #tmpSysUsage(DT datetime,[Description] nvarchar(100),BoxId int,MaxMsgs int,MaxTxtMsgs int,ProtocolTypeName nvarchar(100),MsgInType int,MsgIn int,MsgOutType int,MsgOut int)" +
                    //--vlfMsgInHst
                " INSERT INTO #tmpSysUsage SELECT DISTINCT DateTimeReceived,ISNULL(vlfVehicleInfo.Description,'N/A') AS [Description], vlfBox.BoxId,ISNULL(MaxMsgs,0) AS MaxMsgs,ISNULL(MaxTxtMsgs,0) AS MaxTxtMsgs,ISNULL(vlfBoxProtocolType.BoxProtocolTypeName,'N/A') AS ProtocolTypeName,ISNULL(vlfBoxProtocolTypeMsgInType.BoxMsgInTypeId,-1) AS MsgInType,ISNULL(vlfBoxProtocolTypeMsgInType.MsgInTypeLen,0) AS MsgIn,-1 as MsgOutType,0 as MsgOut" +
                " FROM vlfVehicleInfo INNER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId" +
                " RIGHT OUTER JOIN vlfMsgInHst with (nolock) INNER JOIN vlfBox with (nolock) ON vlfMsgInHst.BoxId = vlfBox.BoxId" +
                " INNER JOIN vlfBoxMsgInType ON vlfMsgInHst.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId" +
                " INNER JOIN vlfBoxProtocolType ON vlfMsgInHst.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                " INNER JOIN vlfBoxProtocolTypeMsgInType ON vlfBoxMsgInType.BoxMsgInTypeId = vlfBoxProtocolTypeMsgInType.BoxMsgInTypeId AND vlfBoxProtocolType.BoxProtocolTypeId = vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId ON vlfVehicleAssignment.BoxId = vlfBox.BoxId" +
                " LEFT JOIN vlfBoxSettings ON vlfBoxSettings.BoxId = vlfBox.BoxId AND vlfBoxSettings.BoxProtocolTypeId = vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId" +
                " WHERE vlfBox.BoxId<>0 AND vlfBox.OrganizationId=" + organizationId + msgInDateTimeFilter +
                " AND vlfMsgInHst.BoxMsgInTypeId<>" + (short)Enums.MessageType.MDTSpecialMessage +

                //--vlfMsgInHstIgnored
                " INSERT INTO #tmpSysUsage SELECT DISTINCT DateTimeReceived,ISNULL(vlfVehicleInfo.Description,'N/A') AS [Description], vlfBox.BoxId,ISNULL(MaxMsgs,0) AS MaxMsgs,ISNULL(MaxTxtMsgs,0) AS MaxTxtMsgs,ISNULL(vlfBoxProtocolType.BoxProtocolTypeName,'-----') AS ProtocolTypeName,ISNULL(vlfBoxProtocolTypeMsgInType.BoxMsgInTypeId,-1) AS MsgInType,ISNULL(vlfBoxProtocolTypeMsgInType.MsgInTypeLen,0) AS MsgIn,-1 as MsgOutType,0 as MsgOut" +
                " FROM vlfVehicleInfo INNER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId" +
                " RIGHT OUTER JOIN vlfMsgInHstIgnored INNER JOIN vlfBox with (nolock) ON vlfMsgInHstIgnored.BoxId = vlfBox.BoxId" +
                " INNER JOIN vlfBoxMsgInType ON vlfMsgInHstIgnored.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId" +
                " INNER JOIN vlfBoxProtocolType ON vlfMsgInHstIgnored.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                " INNER JOIN vlfBoxProtocolTypeMsgInType ON vlfBoxMsgInType.BoxMsgInTypeId = vlfBoxProtocolTypeMsgInType.BoxMsgInTypeId AND vlfBoxProtocolType.BoxProtocolTypeId = vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId ON vlfVehicleAssignment.BoxId = vlfBox.BoxId" +
                " LEFT JOIN vlfBoxSettings ON vlfBoxSettings.BoxId = vlfBox.BoxId AND vlfBoxSettings.BoxProtocolTypeId = vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId" +
                " WHERE vlfBox.BoxId<>0 AND vlfBox.OrganizationId=" + organizationId + msgInDateTimeFilter +
                " AND vlfMsgInHstIgnored.BoxMsgInTypeId<>" + (short)Enums.MessageType.MDTSpecialMessage +

                //--vlfMsgOutHst
                " INSERT INTO #tmpSysUsage SELECT DISTINCT vlfMsgOutHst.[DateTime],ISNULL(vlfVehicleInfo.Description,'N/A') AS [Description], vlfBox.BoxId,ISNULL(MaxMsgs,0) AS MaxMsgs,ISNULL(MaxTxtMsgs,0) AS MaxTxtMsgs,ISNULL(vlfBoxProtocolType.BoxProtocolTypeName,'-----') AS ProtocolTypeName,-1 AS MsgInType,0 AS MsgIn,ISNULL(vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId,-1) AS MsgOutType,ISNULL(vlfBoxProtocolTypeCmdOutType.CmdOutTypeLen,0) AS MsgOut" +
                " FROM vlfVehicleInfo INNER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId" +
                " RIGHT OUTER JOIN vlfBox with (nolock) INNER JOIN vlfMsgOutHst ON vlfBox.BoxId = vlfMsgOutHst.BoxId" +
                " INNER JOIN vlfBoxCmdOutType ON vlfMsgOutHst.BoxCmdOutTypeId = vlfBoxCmdOutType.BoxCmdOutTypeId" +
                " INNER JOIN vlfBoxProtocolType ON vlfMsgOutHst.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                " INNER JOIN vlfBoxProtocolTypeCmdOutType ON vlfBoxCmdOutType.BoxCmdOutTypeId = vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId AND vlfBoxProtocolType.BoxProtocolTypeId = vlfBoxProtocolTypeCmdOutType.BoxProtocolTypeId ON vlfVehicleAssignment.BoxId = vlfBox.BoxId" +
                " LEFT JOIN vlfBoxSettings ON vlfBoxSettings.BoxId = vlfBox.BoxId AND vlfBoxSettings.BoxProtocolTypeId = vlfBoxProtocolTypeCmdOutType.BoxProtocolTypeId" +
                " WHERE vlfBox.BoxId<>0 AND vlfBox.OrganizationId=" + organizationId + msgOutDateTimeFilter +

                //------------------------ IN -------------------------------------
                    //--IN MSGS without Ack and all TxtMsgs
                " SELECT Description,BoxId,ProtocolTypeName,Count(MsgIn) AS TotalInMsgs" +
                " INTO #tmpSysUsageInCount FROM #tmpSysUsage" +
                " WHERE MsgInType<>" + (short)Enums.MessageType.MDTAck +
                " AND MsgInType<>" + (short)Enums.MessageType.MDTResponse +
                    //" AND MsgInType<>" + (short)Enums.MessageType.MDTSpecialMessage +
                " AND MsgInType<>" + (short)Enums.MessageType.MDTTextMessage +
                " AND MsgInType<>-1 AND MsgInType<>" + (short)Enums.MessageType.Ack +
                " GROUP BY Description,BoxId,ProtocolTypeName" +

                //--IN MSGS without all TxtMsgs
                " SELECT Description,BoxId,ProtocolTypeName,SUM(MsgIn) AS TotalInBytes" +
                " INTO #tmpSysUsageInByte FROM #tmpSysUsage" +
                " WHERE MsgInType<>" + (short)Enums.MessageType.MDTAck +
                " AND MsgInType<>" + (short)Enums.MessageType.MDTResponse +
                    //" AND MsgInType<>" + (short)Enums.MessageType.MDTSpecialMessage +
                " AND MsgInType<>" + (short)Enums.MessageType.MDTTextMessage +
                " AND MsgInType<>-1 GROUP BY Description,BoxId,ProtocolTypeName" +

                //--IN TXT MSGS without MDTAck
                " SELECT Description,BoxId,ProtocolTypeName,Count(MsgIn) AS TotalInTxtMsgs" +
                " INTO #tmpSysUsageInTxtCount FROM #tmpSysUsage" +
                " WHERE MsgInType=" + (short)Enums.MessageType.MDTResponse +
                    //" OR MsgInType=" + (short)Enums.MessageType.MDTSpecialMessage +
                " OR MsgInType=" + (short)Enums.MessageType.MDTTextMessage +
                " GROUP BY Description,BoxId,ProtocolTypeName" +

                //--IN MSGS with all TxtMsgs only
                " SELECT Description,BoxId,ProtocolTypeName,SUM(MsgIn) AS TotalInTxtBytes" +
                " INTO #tmpSysUsageInTxtByte FROM #tmpSysUsage" +
                " WHERE MsgInType=" + (short)Enums.MessageType.MDTAck +
                " OR MsgInType=" + (short)Enums.MessageType.MDTResponse +
                    //" OR MsgInType=" + (short)Enums.MessageType.MDTSpecialMessage +
                " OR MsgInType=" + (short)Enums.MessageType.MDTTextMessage +
                " GROUP BY Description,BoxId,ProtocolTypeName" +

                //---------------------- OUT ------------------------------
                    //-- do not include ACK and all TxtMsgs
                " SELECT Description,BoxId,ProtocolTypeName,Count(MsgOut) AS TotalOutMsgs" +
                " INTO #tmpSysUsageOutCount FROM #tmpSysUsage" +
                " WHERE MsgOutType<>" + (short)Enums.CommandType.MDTAck +
                " AND MsgOutType<>" + (short)Enums.CommandType.MDTTextMessage +
                " AND MsgOutType<>-1" +
                " AND MsgOutType<>" + (short)Enums.CommandType.Ack +
                " GROUP BY Description,BoxId,ProtocolTypeName" +

                //--do not include all TextMsgs
                " SELECT Description,BoxId,ProtocolTypeName,SUM(MsgOut) AS TotalOutBytes" +
                " INTO #tmpSysUsageOutByte FROM #tmpSysUsage" +
                " WHERE MsgOutType<>" + (short)Enums.CommandType.MDTAck +
                " AND MsgOutType<>" + (short)Enums.CommandType.MDTTextMessage +
                " AND MsgOutType<>-1" +
                " GROUP BY Description,BoxId,ProtocolTypeName" +

                //-- Only TxtMsgs,do not include MDTAck
                " SELECT Description,BoxId,ProtocolTypeName,Count(MsgOut) AS TotalOutTxtMsgs" +
                " INTO #tmpSysUsageOutTxtCount FROM #tmpSysUsage" +
                " WHERE MsgOutType=" + (short)Enums.CommandType.MDTTextMessage +
                " GROUP BY Description,BoxId,ProtocolTypeName" +

                //--include all TextMsgs only
                " SELECT Description,BoxId,ProtocolTypeName,SUM(MsgOut) AS TotalOutTxtBytes" +
                " INTO #tmpSysUsageOutTxtByte FROM #tmpSysUsage" +
                " WHERE MsgOutType=" + (short)Enums.CommandType.MDTAck +
                " OR MsgOutType=" + (short)Enums.CommandType.MDTTextMessage +
                " GROUP BY Description,BoxId,ProtocolTypeName" +

                //----------------------------------------------------------------
                " SELECT DISTINCT #tmpSysUsage.Description,#tmpSysUsage.BoxId,MaxMsgs,MaxTxtMsgs,#tmpSysUsage.ProtocolTypeName," +
                " ISNULL(TotalInBytes,0) as TotalInBytes," +
                " ISNULL(TotalInMsgs,0) as TotalInMsgs," +
                " ISNULL(TotalOutBytes,0) as TotalOutBytes," +
                " ISNULL(TotalOutMsgs,0) as TotalOutMsgs," +
                " ISNULL(TotalInTxtBytes,0) as TotalInTxtBytes," +
                " ISNULL(TotalInTxtMsgs,0) as TotalInTxtMsgs," +
                " ISNULL(TotalOutTxtBytes,0) as TotalOutTxtBytes," +
                " ISNULL(TotalOutTxtMsgs,0) as TotalOutTxtMsgs" +
                " INTO #tmpTotals FROM #tmpSysUsage" +
                " LEFT JOIN #tmpSysUsageInCount ON #tmpSysUsageInCount.Description=#tmpSysUsage.Description AND #tmpSysUsageInCount.ProtocolTypeName=#tmpSysUsage.ProtocolTypeName" +
                " LEFT JOIN #tmpSysUsageInByte ON #tmpSysUsageInByte.Description=#tmpSysUsage.Description AND #tmpSysUsageInByte.ProtocolTypeName=#tmpSysUsage.ProtocolTypeName" +
                " LEFT JOIN #tmpSysUsageOutCount ON #tmpSysUsage.Description=#tmpSysUsageOutCount.Description AND #tmpSysUsage.ProtocolTypeName=#tmpSysUsageOutCount.ProtocolTypeName" +
                " LEFT JOIN #tmpSysUsageOutByte ON #tmpSysUsage.Description=#tmpSysUsageOutByte.Description AND #tmpSysUsage.ProtocolTypeName=#tmpSysUsageOutByte.ProtocolTypeName" +
                " LEFT JOIN #tmpSysUsageInTxtCount ON #tmpSysUsageInTxtCount.Description=#tmpSysUsage.Description AND #tmpSysUsageInTxtCount.ProtocolTypeName=#tmpSysUsage.ProtocolTypeName" +
                " LEFT JOIN #tmpSysUsageInTxtByte ON #tmpSysUsageInTxtByte.Description=#tmpSysUsage.Description AND #tmpSysUsageInTxtByte.ProtocolTypeName=#tmpSysUsage.ProtocolTypeName" +
                " LEFT JOIN #tmpSysUsageOutTxtCount ON #tmpSysUsage.Description=#tmpSysUsageOutTxtCount.Description AND #tmpSysUsage.ProtocolTypeName=#tmpSysUsageOutTxtCount.ProtocolTypeName" +
                " LEFT JOIN #tmpSysUsageOutTxtByte ON #tmpSysUsage.Description=#tmpSysUsageOutTxtByte.Description AND #tmpSysUsage.ProtocolTypeName=#tmpSysUsageOutTxtByte.ProtocolTypeName" +

                " SELECT *,SUM(ISNULL(TotalInBytes,0)) + SUM(ISNULL(TotalOutBytes,0)) as TotalBytes," +
                " SUM(ISNULL(TotalInMsgs,0))+ SUM(ISNULL(TotalOutMsgs,0)) as TotalMsgs," +
                " SUM(ISNULL(TotalInTxtBytes,0)) + SUM(ISNULL(TotalOutTxtBytes,0)) as TotalTxtBytes," +
                " SUM(ISNULL(TotalInTxtMsgs,0))+ SUM(ISNULL(TotalOutTxtMsgs,0)) as TotalTxtMsgs";
                if (showExceptionOnly)
                    sql += " INTO #tmpExceptionTotals";
                sql += " from #tmpTotals" +
                " GROUP BY Description,BoxId,ProtocolTypeName,TotalInBytes,TotalInMsgs,TotalOutBytes,TotalOutMsgs," +
                " TotalInTxtBytes,TotalInTxtMsgs,TotalOutTxtBytes,TotalOutTxtMsgs,MaxMsgs,MaxTxtMsgs";

                if (showExceptionOnly)
                    sql += " SELECT * FROM #tmpExceptionTotals WHERE TotalMsgs>MaxMsgs or TotalTxtMsgs>MaxTxtMsgs";

                sql += " DROP TABLE #tmpSysUsageInCount" +
                " DROP TABLE #tmpSysUsageInByte" +
                " DROP TABLE #tmpSysUsageInTxtCount" +
                " DROP TABLE #tmpSysUsageInTxtByte" +
                " DROP TABLE #tmpSysUsageOutCount" +
                " DROP TABLE #tmpSysUsageOutByte" +
                " DROP TABLE #tmpSysUsageOutTxtCount" +
                " DROP TABLE #tmpSysUsageOutTxtByte" +
                " DROP TABLE #tmpSysUsage" +
                " DROP TABLE #tmpTotals";
                if (showExceptionOnly)
                    sql += " DROP TABLE #tmpExceptionTotals";

                //Executes SQL statement
                sqlExec.CommandTimeout = 1800;
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                sqlExec.CommandTimeout = prevCommandTimeout;
                string prefixMsg = "Unable to retrieve system usage report by organization id=" + organizationId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                sqlExec.CommandTimeout = prevCommandTimeout;
                string prefixMsg = "Unable to retrieve system usage report by organization id=" + organizationId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            sqlExec.CommandTimeout = prevCommandTimeout;
            return resultDataSet;
        }

        /// <summary>
        /// Retrieves system usage report. 
        /// </summary>
        /// <returns>
        /// DataSet [Description],[BoxId],[MaxMsgs],[MaxTxtMsgs],[BoxProtocolTypeName],[TotalMsgInSize],[TotalMsgOutSize]
        /// </returns>
        /// <param name="boxId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="showExceptionOnly"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        [Obsolete("Not in use")]
        public DataSet GetSystemUsageReportByBox(int boxId, string fromDateTime, string toDateTime, bool showExceptionOnly)
        {
            int prevCommandTimeout = sqlExec.CommandTimeout;

            DataSet resultDataSet = null;
            string msgInDateTimeFilter = "";
            string msgOutDateTimeFilter = "";
            try
            {
                if (fromDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                {
                    msgInDateTimeFilter = " AND DateTimeReceived>='" + fromDateTime + "'";
                    msgOutDateTimeFilter = " AND vlfMsgOutHst.[DateTime]>='" + fromDateTime + "'";
                }
                if (toDateTime != VLF.CLS.Def.Const.unassignedStrValue)
                {
                    msgInDateTimeFilter += " AND DateTimeReceived<='" + toDateTime + "'";
                    msgOutDateTimeFilter += " AND vlfMsgOutHst.[DateTime]<='" + toDateTime + "'";
                }

                string sql = "CREATE TABLE #tmpSysUsage(DT datetime,[Description] nvarchar(100),BoxId int,MaxMsgs int,MaxTxtMsgs int,ProtocolTypeName nvarchar(100),MsgInType int,MsgIn int,MsgOutType int,MsgOut int)" +
                    //--vlfMsgInHst
                   " INSERT INTO #tmpSysUsage SELECT DISTINCT DateTimeReceived,ISNULL(vlfVehicleInfo.Description,'N/A') AS [Description],vlfMsgInHst.BoxId,ISNULL(MaxMsgs,0) AS MaxMsgs,ISNULL(MaxTxtMsgs,0) AS MaxTxtMsgs,ISNULL(vlfBoxProtocolType.BoxProtocolTypeName,'-----') AS ProtocolTypeName,ISNULL(vlfBoxProtocolTypeMsgInType.BoxMsgInTypeId,-1) AS MsgInType,ISNULL(vlfBoxProtocolTypeMsgInType.MsgInTypeLen,0) AS MsgIn,-1 as MsgOutType,0 as MsgOut" +
                   " FROM vlfVehicleInfo INNER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId" +
                   " RIGHT OUTER JOIN vlfBox with (nolock) INNER JOIN vlfMsgInHst with (nolock) ON vlfBox.BoxId = vlfMsgInHst.BoxId" +
                   " INNER JOIN vlfBoxMsgInType ON vlfMsgInHst.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId" +
                   " INNER JOIN vlfBoxProtocolType ON vlfMsgInHst.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " INNER JOIN vlfBoxProtocolTypeMsgInType ON vlfBoxMsgInType.BoxMsgInTypeId = vlfBoxProtocolTypeMsgInType.BoxMsgInTypeId AND vlfBoxProtocolType.BoxProtocolTypeId = vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId ON vlfVehicleAssignment.BoxId = vlfBox.BoxId" +
                   " LEFT JOIN vlfBoxSettings ON vlfBoxSettings.BoxId = vlfBox.BoxId AND vlfBoxSettings.BoxProtocolTypeId = vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId" +
                   " WHERE vlfBox.BoxId=" + boxId + msgInDateTimeFilter +
                   " AND vlfMsgInHst.BoxMsgInTypeId<>" + (short)Enums.MessageType.MDTSpecialMessage +

                   //--vlfMsgInHstIgnored
                   " INSERT INTO #tmpSysUsage SELECT DISTINCT DateTimeReceived,ISNULL(vlfVehicleInfo.Description,'N/A') AS [Description],vlfMsgInHstIgnored.BoxId,ISNULL(MaxMsgs,0) AS MaxMsgs,ISNULL(MaxTxtMsgs,0) AS MaxTxtMsgs,ISNULL(vlfBoxProtocolType.BoxProtocolTypeName,'-----') AS ProtocolTypeName,ISNULL(vlfBoxProtocolTypeMsgInType.BoxMsgInTypeId,-1) AS MsgInType,ISNULL(vlfBoxProtocolTypeMsgInType.MsgInTypeLen,0) AS MsgIn,-1 as MsgOutType,0 as MsgOut" +
                   " FROM vlfVehicleInfo INNER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId" +
                   " RIGHT OUTER JOIN vlfBox with (nolock) INNER JOIN vlfMsgInHstIgnored ON vlfBox.BoxId = vlfMsgInHstIgnored.BoxId" +
                   " INNER JOIN vlfBoxMsgInType ON vlfMsgInHstIgnored.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId" +
                   " INNER JOIN vlfBoxProtocolType ON vlfMsgInHstIgnored.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " INNER JOIN vlfBoxProtocolTypeMsgInType ON vlfBoxMsgInType.BoxMsgInTypeId = vlfBoxProtocolTypeMsgInType.BoxMsgInTypeId AND vlfBoxProtocolType.BoxProtocolTypeId = vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId ON vlfVehicleAssignment.BoxId = vlfBox.BoxId" +
                   " LEFT JOIN vlfBoxSettings ON vlfBoxSettings.BoxId = vlfBox.BoxId AND vlfBoxSettings.BoxProtocolTypeId = vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId" +
                   " WHERE vlfBox.BoxId=" + boxId + msgInDateTimeFilter +
                   " AND vlfMsgInHstIgnored.BoxMsgInTypeId<>" + (short)Enums.MessageType.MDTSpecialMessage +

                   //--vlfMsgOutHst
                   " INSERT INTO #tmpSysUsage SELECT DISTINCT vlfMsgOutHst.[DateTime],ISNULL(vlfVehicleInfo.Description,'N/A') AS [Description],vlfMsgOutHst.BoxId,ISNULL(MaxMsgs,0) AS MaxMsgs,ISNULL(MaxTxtMsgs,0) AS MaxTxtMsgs,ISNULL(vlfBoxProtocolType.BoxProtocolTypeName,'-----') AS ProtocolTypeName,-1 AS MsgInType,0 AS MsgIn,ISNULL(vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId,-1) AS MsgOutType,ISNULL(vlfBoxProtocolTypeCmdOutType.CmdOutTypeLen,0) AS MsgOut" +
                   " FROM vlfVehicleInfo INNER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId" +
                   " RIGHT OUTER JOIN vlfBox with (nolock) INNER JOIN vlfMsgOutHst with (nolock) ON vlfBox.BoxId = vlfMsgOutHst.BoxId" +
                   " INNER JOIN vlfBoxCmdOutType ON vlfMsgOutHst.BoxCmdOutTypeId = vlfBoxCmdOutType.BoxCmdOutTypeId" +
                   " INNER JOIN vlfBoxProtocolType ON vlfMsgOutHst.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " INNER JOIN vlfBoxProtocolTypeCmdOutType ON vlfBoxCmdOutType.BoxCmdOutTypeId = vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId AND vlfBoxProtocolType.BoxProtocolTypeId = vlfBoxProtocolTypeCmdOutType.BoxProtocolTypeId ON vlfVehicleAssignment.BoxId = vlfBox.BoxId" +
                   " LEFT JOIN vlfBoxSettings ON vlfBoxSettings.BoxId = vlfBox.BoxId AND vlfBoxSettings.BoxProtocolTypeId = vlfBoxProtocolTypeCmdOutType.BoxProtocolTypeId" +
                   " WHERE vlfBox.BoxId=" + boxId + msgOutDateTimeFilter +

                   //------------------------ IN -------------------------------------
                    //--IN MSGS without Ack and all TxtMsgs
                   " SELECT Description,BoxId,ProtocolTypeName,Count(MsgIn) AS TotalInMsgs" +
                   " INTO #tmpSysUsageInCount FROM #tmpSysUsage" +
                   " WHERE MsgInType<>" + (short)Enums.MessageType.MDTAck +
                   " AND MsgInType<>" + (short)Enums.MessageType.MDTResponse +
                    //" AND MsgInType<>" + (short)Enums.MessageType.MDTSpecialMessage +
                   " AND MsgInType<>" + (short)Enums.MessageType.MDTTextMessage +
                   " AND MsgInType<>-1 AND MsgInType<>" + (short)Enums.MessageType.Ack +
                   " GROUP BY Description,BoxId,ProtocolTypeName" +

                   //--IN MSGS without all TxtMsgs
                   " SELECT Description,BoxId,ProtocolTypeName,SUM(MsgIn) AS TotalInBytes" +
                   " INTO #tmpSysUsageInByte FROM #tmpSysUsage" +
                   " WHERE MsgInType<>" + (short)Enums.MessageType.MDTAck +
                   " AND MsgInType<>" + (short)Enums.MessageType.MDTResponse +
                    //" AND MsgInType<>" + (short)Enums.MessageType.MDTSpecialMessage +
                   " AND MsgInType<>" + (short)Enums.MessageType.MDTTextMessage +
                   " AND MsgInType<>-1 GROUP BY Description,BoxId,ProtocolTypeName" +

                   //--IN TXT MSGS without MDTAck
                   " SELECT Description,BoxId,ProtocolTypeName,Count(MsgIn) AS TotalInTxtMsgs" +
                   " INTO #tmpSysUsageInTxtCount FROM #tmpSysUsage" +
                   " WHERE MsgInType=" + (short)Enums.MessageType.MDTResponse +
                    //" OR MsgInType=" + (short)Enums.MessageType.MDTSpecialMessage +
                   " OR MsgInType=" + (short)Enums.MessageType.MDTTextMessage +
                   " GROUP BY Description,BoxId,ProtocolTypeName" +

                   //--IN MSGS with all TxtMsgs only
                   " SELECT Description,BoxId,ProtocolTypeName,SUM(MsgIn) AS TotalInTxtBytes" +
                   " INTO #tmpSysUsageInTxtByte FROM #tmpSysUsage" +
                   " WHERE MsgInType=" + (short)Enums.MessageType.MDTAck +
                   " OR MsgInType=" + (short)Enums.MessageType.MDTResponse +
                    //" OR MsgInType=" + (short)Enums.MessageType.MDTSpecialMessage +
                   " OR MsgInType=" + (short)Enums.MessageType.MDTTextMessage +
                   " GROUP BY Description,BoxId,ProtocolTypeName" +

                   //---------------------- OUT ------------------------------
                    //-- do not include ACK and all TxtMsgs
                   " SELECT Description,BoxId,ProtocolTypeName,Count(MsgOut) AS TotalOutMsgs" +
                   " INTO #tmpSysUsageOutCount FROM #tmpSysUsage" +
                   " WHERE MsgOutType<>" + (short)Enums.CommandType.MDTAck +
                   " AND MsgOutType<>" + (short)Enums.CommandType.MDTTextMessage +
                   " AND MsgOutType<>-1" +
                   " AND MsgOutType<>" + (short)Enums.CommandType.Ack +
                   " GROUP BY Description,BoxId,ProtocolTypeName" +

                   //--do not include all TextMsgs
                   " SELECT Description,BoxId,ProtocolTypeName,SUM(MsgOut) AS TotalOutBytes" +
                   " INTO #tmpSysUsageOutByte FROM #tmpSysUsage" +
                   " WHERE MsgOutType<>" + (short)Enums.CommandType.MDTAck +
                   " AND MsgOutType<>" + (short)Enums.CommandType.MDTTextMessage +
                   " AND MsgOutType<>-1" +
                   " GROUP BY Description,BoxId,ProtocolTypeName" +

                   //-- Only TxtMsgs,do not include MDTAck
                   " SELECT Description,BoxId,ProtocolTypeName,Count(MsgOut) AS TotalOutTxtMsgs" +
                   " INTO #tmpSysUsageOutTxtCount FROM #tmpSysUsage" +
                   " WHERE MsgOutType=" + (short)Enums.CommandType.MDTTextMessage +
                   " GROUP BY Description,BoxId,ProtocolTypeName" +

                   //--include all TextMsgs only
                   " SELECT Description,BoxId,ProtocolTypeName,SUM(MsgOut) AS TotalOutTxtBytes" +
                   " INTO #tmpSysUsageOutTxtByte FROM #tmpSysUsage" +
                   " WHERE MsgOutType=" + (short)Enums.CommandType.MDTAck +
                   " OR MsgOutType=" + (short)Enums.CommandType.MDTTextMessage +
                   " GROUP BY Description,BoxId,ProtocolTypeName" +

                   //----------------------------------------------------------------
                   " SELECT DISTINCT #tmpSysUsage.Description,#tmpSysUsage.BoxId,MaxMsgs,MaxTxtMsgs,#tmpSysUsage.ProtocolTypeName," +
                   " ISNULL(TotalInBytes,0) as TotalInBytes," +
                   " ISNULL(TotalInMsgs,0) as TotalInMsgs," +
                   " ISNULL(TotalOutBytes,0) as TotalOutBytes," +
                   " ISNULL(TotalOutMsgs,0) as TotalOutMsgs," +
                   " ISNULL(TotalInTxtBytes,0) as TotalInTxtBytes," +
                   " ISNULL(TotalInTxtMsgs,0) as TotalInTxtMsgs," +
                   " ISNULL(TotalOutTxtBytes,0) as TotalOutTxtBytes," +
                   " ISNULL(TotalOutTxtMsgs,0) as TotalOutTxtMsgs" +
                   " INTO #tmpTotals FROM #tmpSysUsage" +
                   " LEFT JOIN #tmpSysUsageInCount ON #tmpSysUsageInCount.Description=#tmpSysUsage.Description AND #tmpSysUsageInCount.ProtocolTypeName=#tmpSysUsage.ProtocolTypeName" +
                   " LEFT JOIN #tmpSysUsageInByte ON #tmpSysUsageInByte.Description=#tmpSysUsage.Description AND #tmpSysUsageInByte.ProtocolTypeName=#tmpSysUsage.ProtocolTypeName" +
                   " LEFT JOIN #tmpSysUsageOutCount ON #tmpSysUsage.Description=#tmpSysUsageOutCount.Description AND #tmpSysUsage.ProtocolTypeName=#tmpSysUsageOutCount.ProtocolTypeName" +
                   " LEFT JOIN #tmpSysUsageOutByte ON #tmpSysUsage.Description=#tmpSysUsageOutByte.Description AND #tmpSysUsage.ProtocolTypeName=#tmpSysUsageOutByte.ProtocolTypeName" +
                   " LEFT JOIN #tmpSysUsageInTxtCount ON #tmpSysUsageInTxtCount.Description=#tmpSysUsage.Description AND #tmpSysUsageInTxtCount.ProtocolTypeName=#tmpSysUsage.ProtocolTypeName" +
                   " LEFT JOIN #tmpSysUsageInTxtByte ON #tmpSysUsageInTxtByte.Description=#tmpSysUsage.Description AND #tmpSysUsageInTxtByte.ProtocolTypeName=#tmpSysUsage.ProtocolTypeName" +
                   " LEFT JOIN #tmpSysUsageOutTxtCount ON #tmpSysUsage.Description=#tmpSysUsageOutTxtCount.Description AND #tmpSysUsage.ProtocolTypeName=#tmpSysUsageOutTxtCount.ProtocolTypeName" +
                   " LEFT JOIN #tmpSysUsageOutTxtByte ON #tmpSysUsage.Description=#tmpSysUsageOutTxtByte.Description AND #tmpSysUsage.ProtocolTypeName=#tmpSysUsageOutTxtByte.ProtocolTypeName" +

                   " SELECT *,SUM(ISNULL(TotalInBytes,0)) + SUM(ISNULL(TotalOutBytes,0)) as TotalBytes," +
                   " SUM(ISNULL(TotalInMsgs,0))+ SUM(ISNULL(TotalOutMsgs,0)) as TotalMsgs," +
                   " SUM(ISNULL(TotalInTxtBytes,0)) + SUM(ISNULL(TotalOutTxtBytes,0)) as TotalTxtBytes," +
                   " SUM(ISNULL(TotalInTxtMsgs,0))+ SUM(ISNULL(TotalOutTxtMsgs,0)) as TotalTxtMsgs";

                if (showExceptionOnly)
                    sql += " INTO #tmpExceptionTotals";

                sql += " from #tmpTotals" +
                   " GROUP BY Description,BoxId,ProtocolTypeName,TotalInBytes,TotalInMsgs,TotalOutBytes,TotalOutMsgs," +
                   " TotalInTxtBytes,TotalInTxtMsgs,TotalOutTxtBytes,TotalOutTxtMsgs,MaxMsgs,MaxTxtMsgs";

                if (showExceptionOnly)
                    sql += " SELECT * FROM #tmpExceptionTotals WHERE TotalMsgs>MaxMsgs or TotalTxtMsgs>MaxTxtMsgs";

                sql += " DROP TABLE #tmpSysUsageInCount" +
                   " DROP TABLE #tmpSysUsageInByte" +
                   " DROP TABLE #tmpSysUsageInTxtCount" +
                   " DROP TABLE #tmpSysUsageInTxtByte" +
                   " DROP TABLE #tmpSysUsageOutCount" +
                   " DROP TABLE #tmpSysUsageOutByte" +
                   " DROP TABLE #tmpSysUsageOutTxtCount" +
                   " DROP TABLE #tmpSysUsageOutTxtByte" +
                   " DROP TABLE #tmpSysUsage" +
                   " DROP TABLE #tmpTotals";

                if (showExceptionOnly)
                    sql += " DROP TABLE #tmpExceptionTotals";

                //Executes SQL statement
                sqlExec.CommandTimeout = 600;
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                sqlExec.CommandTimeout = prevCommandTimeout;
                string prefixMsg = "Unable to retrieve system usage report by organization id=" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                sqlExec.CommandTimeout = prevCommandTimeout;
                string prefixMsg = "Unable to retrieve system usage report by organization id=" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            sqlExec.CommandTimeout = prevCommandTimeout;
            return resultDataSet;
        }

        ///<summary>
        /// Retrieves exception report by license plate
        ///</summary>
        /// <param name="licensePlate"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="sosLimit"></param>
        /// <param name="noDoorSnsHrs"></param>
        /// <param name="dsResult"></param>
        /// <param name="vehicleDescription"></param>
        /// <param name="vehicleId"></param>
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
        /// <param name="userTimezone"></param>
        /// <param name="driverDoor"></param>
        /// <param name="passengerDoor"></param>
        /// <param name="sideHopperDoor"></param>
        /// <param name="rearHopperDoor"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void GetExceptionReport(string licensePlate,
                                         int boxId,
                                         string fromDateTime,
                                         string toDateTime,
                                         int userId,
                                         short sosLimit,
                                         int noDoorSnsHrs,
                                         DataSet dsResult,
                                         string vehicleDescription,
                                         Int64 vehicleId,
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
                                         short userTimezone,
                                         bool driverDoor,
                                         bool passengerDoor,
                                         bool sideHopperDoor,
                                         bool rearHopperDoor,
                                         bool includeCurrentTar,
                                         bool locker1, bool locker2, bool locker3, bool locker4, bool locker5, bool locker6,
                                         bool locker7, bool locker8, bool locker9,
                                         ref bool requestOverflowed,
                                         ref int totalSqlRecords)
        {
            DataSet rowInData = null;

            DataSet dsParams = new DataSet();
            dsParams.Tables.Add();
            dsParams.Tables[0].Columns.Add("SosLimit", typeof(short));
            dsParams.Tables[0].Columns.Add("NoDoorSnsHrs", typeof(short));
            dsParams.Tables[0].Columns.Add("IncludeTar", typeof(bool));
            dsParams.Tables[0].Columns.Add("IncludeMobilize", typeof(bool));
            dsParams.Tables[0].Columns.Add("FifteenSecDoorSns", typeof(bool));
            dsParams.Tables[0].Columns.Add("Leash50", typeof(bool));
            dsParams.Tables[0].Columns.Add("MainAndBackupBatterySns", typeof(bool));
            dsParams.Tables[0].Columns.Add("TamperSns", typeof(bool));
            dsParams.Tables[0].Columns.Add("AnyPanicSns", typeof(bool));
            dsParams.Tables[0].Columns.Add("ThreeKeypadAttemptsSns", typeof(bool));
            dsParams.Tables[0].Columns.Add("AltGPSAntennaSns", typeof(bool));
            dsParams.Tables[0].Columns.Add("ControllerStatus", typeof(bool));
            dsParams.Tables[0].Columns.Add("LeashBrokenSns", typeof(bool));

            object[] objRow = new object[dsParams.Tables[0].Columns.Count];
            objRow[0] = sosLimit;
            objRow[1] = noDoorSnsHrs;
            objRow[2] = includeTar;
            objRow[3] = includeMobilize;
            objRow[4] = fifteenSecDoorSns;
            objRow[5] = leash50;
            objRow[6] = mainAndBackupBatterySns;
            objRow[7] = tamperSns;
            objRow[8] = anyPanicSns;
            objRow[9] = threeKeypadAttemptsSns;
            objRow[10] = altGPSAntennaSns;
            objRow[11] = controllerStatus;
            objRow[12] = leashBrokenSns;
            dsParams.Tables[0].Rows.Add(objRow);


            //if(	sosLimit != VLF.CLS.Def.Const.unassignedShortValue || noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue)
            rowInData = GetMsgInHistoryInformation(licensePlate, fromDateTime, toDateTime, userId, dsParams, ref requestOverflowed, ref totalSqlRecords);

            ExceptionReport report = new ExceptionReport(sqlExec);

            report.FillReport(boxId,
                              sosLimit,
                              Convert.ToDateTime(fromDateTime).AddHours(userTimezone),
                              Convert.ToDateTime(toDateTime).AddHours(userTimezone),
                              noDoorSnsHrs,
                              rowInData,
                              vehicleId,
                              vehicleDescription,
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
                              userId, driverDoor, passengerDoor, sideHopperDoor, rearHopperDoor, includeCurrentTar,
                              locker1, locker2, locker3, locker4, locker5, locker6,
                                           locker7, locker8, locker9);

            dsResult.Tables.Add(report.ReportDetailes);
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
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                //string sql = "GetActivitySummaryReportPerOrganization2";
                string sql = "GetActivitySummaryReportPerOrganization_New_NewTZ";
                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, orgId);
                sqlExec.AddCommandParam("@sensorNum", SqlDbType.Int, sensorNum);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.CommandTimeout = 72000;
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummaryReportPerOrganization report by org id=" + orgId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummaryReportPerOrganization report by org id=" + orgId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                //string sql = "GetActivitySummaryReportPerOrganization2";
                string sql = "GetActivitySummaryReportPerOrganization_New";
                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, orgId);
                sqlExec.AddCommandParam("@sensorNum", SqlDbType.Int, sensorNum);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.CommandTimeout = 72000;
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummaryReportPerOrganization report by org id=" + orgId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummaryReportPerOrganization report by org id=" + orgId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Get Activity Summary Report Per Vehicle
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="sensorNum"></param>
        /// <returns></returns>
        public DataSet GetActivitySummaryReportPerVehicle(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int16 sensorNum)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                string sql = "GetActivitySummaryReportPerFleet_New";
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@sensorNum", SqlDbType.Int, sensorNum);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@summary", SqlDbType.Int, 0);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.CommandTimeout = 72000;
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummaryReportPerVehicle report by fleetId=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummaryReportPerVehicle report by fleetId=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }




        /// <summary>
        /// Get Activity Summary Report Per Vehicle
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="sensorNum"></param>
        /// <returns></returns>
        public DataSet GetActivitySummaryReportPerVehicle_Hierarchy(Int32 userId, string NodeCode, DateTime fromDateTime, DateTime toDateTime, Int16 sensorNum)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                string sql = "GetActivitySummaryReportPerFleet_New_Hierarchy";
                sqlExec.AddCommandParam("@NodeCode", SqlDbType.VarChar, NodeCode);
                sqlExec.AddCommandParam("@sensorNum", SqlDbType.Int, sensorNum);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@summary", SqlDbType.Int, 0);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.CommandTimeout = 72000;
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummaryReportPerVehicle_Hierarchy report by NodeCode='" + NodeCode + "' FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummaryReportPerVehicle_Hierarchy report by NodeCode='" + NodeCode + "' FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// <summary>
        /// Get Activity Summary Report Per Vehicle Fuel
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="sensorNum"></param>
        /// <returns></returns>
        public DataSet GetActivitySummaryReportPerVehicle_Fuel(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int16 sensorNum)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                string sql = "GetActivitySummaryReportPerFleet_Fuel";
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@sensorNum", SqlDbType.Int, sensorNum);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.CommandTimeout = 72000;
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummaryReportPerVehicle_Fuel report by fleetId=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummaryReportPerVehicle_Fuel report by fleetId=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// <summary>
        /// Get Activity Summary Report Per Vehicle
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="sensorNum"></param>
        /// <returns></returns>
        public DataSet GetActivitySummaryReportPerVehicle_Special(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int16 sensorNum)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                string sql = "GetActivitySummaryReportPerFleet_Special";
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@sensorNum", SqlDbType.Int, sensorNum);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@summary", SqlDbType.Int, 0);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.CommandTimeout = 72000;
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummaryReportPerVehicle_Special report by fleetId=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummaryReportPerVehicle_Special report by fleetId=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Get Activity Summary Report Per Vehicle
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="sensorNum"></param>
        /// <returns></returns>
        public DataSet GetSensorActivityReport(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                string sql = "SensorActivityReport";
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.CommandTimeout = 600;
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetSensorActivityReport report by fleetId=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetSensorActivityReport report by fleetId=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }




        /// <summary>
        /// Get Activity Summary Report Per Vehicle
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="sensorNum"></param>
        /// <returns></returns>
        public DataSet GetActivitySummaryReportPerVehicle_Daily(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int16 sensorNum)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                string sql = "GetActivitySummaryReportPerFleet_Daily";
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@sensorNum", SqlDbType.Int, sensorNum);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@summary", SqlDbType.Int, 0);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.CommandTimeout = 72000;
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummaryReportPerVehicle_Daily report by fleetId=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummaryReportPerVehicle_Daily report by fleetId=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                //string sql = "GetStateMileagePerFleet";
                string sql = "GetStateMileageWithEnterExitDatePerFleet";
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@summary", SqlDbType.Bit, summary);
                sqlExec.CommandTimeout = 600;
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetStateMileagePerFleet report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetStateMileagePerFleet report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// Get State Mileage Per Fleet based on State/Province
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="summary"></param>
        /// <returns></returns>

        public DataSet GetStateMileagePerFleet_StateBased(Int32 fleetId, DateTime fromDateTime, DateTime toDateTime)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                string sql = "GetStateMileagePerFleet_StateBased";
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.CommandTimeout = 600;
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetStateMileagePerFleet_StateBased report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetStateMileagePerFleet_StateBased report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                // string sql = "GetStateMileagePerVehicle";

                string sql = "GetStateMileageWithEnterExitDatePerVehicle";
                sqlExec.AddCommandParam("@licensePlate", SqlDbType.VarChar, licensePlate);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.CommandTimeout = 600;
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetStateMileagePerBox report by licensePlate=" + licensePlate + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetStateMileagePerBox report by licensePlate=" + licensePlate + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
        #endregion

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
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                string sql = "sp_GetDailyUtilizationReport";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@vehicleId", SqlDbType.Int, vehicleId);
                sqlExec.AddCommandParam("@From", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@To", SqlDbType.DateTime, toDateTime);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve DailyVehicleUtilization report by Vehicle id=" + vehicleId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve DailyVehicleUtilization report by Vehicle id=" + vehicleId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                string sql = "sp_GetDailyFleetUtilizationReport";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@From", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@To", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@cost", SqlDbType.Float, Cost);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Daily Fleet Utilization report by Fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Daily Fleet Utilization report by Fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Get Idling Summary Report PerOrganization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>

        public DataSet GetIdlingSummaryReportPerOrganization(Int32 userId, Int32 orgId, DateTime fromDateTime, DateTime toDateTime)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 7200;
                string sql = "GetActivitySummaryReportPerOrganization";
                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, orgId);
                sqlExec.AddCommandParam("@sensorNum", SqlDbType.Int, 3);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetIdlingSummaryReportPerOrganization report by Organization id=" + orgId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetIdlingSummaryReportPerOrganization report by Organization id=" + orgId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
        public DataSet GetSpeedViolationsReportForFleet(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int32 Type, string colorFilter)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 7200;
                string sql = "CNGetSpeedViolationsReportForFleet_New";

                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@Type", SqlDbType.Int, Type);
                sqlExec.AddCommandParam("@colorFilter", SqlDbType.VarChar, colorFilter);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve SpeedViolationsReportForFleet report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve SpeedViolationsReportForFleet report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
        public DataSet GetSpeedViolationsReportForFleet_RoadSpeed(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int32 Type, string colorFilter)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 720000;
                string sql = "CNGetSpeedViolationsReportForFleet_RoadSpeed";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@Type", SqlDbType.Int, Type);
                sqlExec.AddCommandParam("@colorFilter", SqlDbType.VarChar, colorFilter);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve SpeedViolationsReportForFleet report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve SpeedViolationsReportForFleet report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }




        public DataSet GetSpeedViolationsSummaryReportForFleet_RoadSpeed(int userId, int fleetId, DateTime fromDateTime, DateTime toDateTime, int Type, string colorFilter, short postedSpeedOnly)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                this.sqlExec.ClearCommandParameters();
                this.sqlExec.CommandTimeout = 720000;
                string sql = "GetSpeedViolationsSummaryReportForFleet_RoadSpeed";
                this.sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                this.sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                this.sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                this.sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                this.sqlExec.AddCommandParam("@Type", SqlDbType.Int, Type);
                this.sqlExec.AddCommandParam("@colorFilter", SqlDbType.VarChar, colorFilter);
                this.sqlExec.AddCommandParam("@PostedSpeedOnly", SqlDbType.Bit, postedSpeedOnly);
                sqlDataSet = this.sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Concat(new object[]
				{
					"Unable to retrieve GetSpeedViolationsSummaryReportForFleet_RoadSpeed report by fleet id=",
					fleetId,
					" FromDateTime=",
					fromDateTime,
					" ToDateTime=",
					toDateTime,
					". "
				});
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException2)
            {
                string prefixMsg = string.Concat(new object[]
				{
					"Unable to retrieve GetSpeedViolationsSummaryReportForFleet_RoadSpeed report by fleet id=",
					fleetId,
					" FromDateTime=",
					fromDateTime,
					" ToDateTime=",
					toDateTime,
					". "
				});
                throw new DASException(prefixMsg + " " + objException2.Message);
            }
            return sqlDataSet;
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
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 720000;
                string sql = "GetSpeedViolationsReportForFleet_RoadSpeed";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@PostedSpeedOnly", SqlDbType.SmallInt, PostedSpeedOnly);
                sqlExec.AddCommandParam("@Delta", SqlDbType.Int, Delta);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve SpeedViolationsReportForFleet report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve SpeedViolationsReportForFleet report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
        /// <summary>
        /// LSZ 2012-10-04
        /// Dataset for speed duration Report
        /// Parameter: FleetId, DateFr, DateTo
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="DateFrom"></param>
        /// <param name="DateTo"></param>
        /// <returns>
        /// FleetId, BoxID, VehicleID, VehicleDesc, and Duration for each speed range in minute
        /// </returns>
        public DataSet GetSpeedDistributionReport(int userId, int fleetId, DateTime DateFrom, DateTime DateTo)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[4];
                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@fleet", fleetId);
                sqlParams[2] = new SqlParameter("@dateFr", DateFrom);
                sqlParams[3] = new SqlParameter("@dateTo", DateTo);


                // SQL statement
                string sql = "evtViolationsSpeedDistribution";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }

        public DataSet GetSpeedSummaryViolationsReportForFleet(Int32 userId, Int32 fleetId, DateTime fromDateTime, DateTime toDateTime, Int32 Type, string colorFilter)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "CNGetSpeedSummaryViolationsReportForFleet";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@Type", SqlDbType.Int, Type);
                sqlExec.AddCommandParam("@colorFilter", SqlDbType.VarChar, colorFilter);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve CNGetSpeedSummaryViolationsReportForFleet report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve CNGetSpeedSummaryViolationsReportForFleet report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "CNGetSpeedViolationsDetailsReportForFleet_New";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@Type", SqlDbType.Int, Type);
                sqlExec.AddCommandParam("@colorFilter", SqlDbType.VarChar, ColorFilter);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetSpeedViolationsDetailsReportForFleet report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetSpeedViolationsDetailsReportForFleet report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "CNGetSpeedViolationsDetailsReportForFleet_RoadSpeed";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@Type", SqlDbType.Int, Type);
                sqlExec.AddCommandParam("@colorFilter", SqlDbType.VarChar, ColorFilter);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetSpeedViolationsDetailsReportForFleet report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetSpeedViolationsDetailsReportForFleet report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "CN_GetActivitySummaryReportPerFleet_New";
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@colorFilter", SqlDbType.VarChar, colorFilter);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetCNFleetUtilizationReport report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetCNFleetUtilizationReport report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "CNGetMonthlyFleetSummary";
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetCNGetMonthlyFleetSummary report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetCNGetMonthlyFleetSummary report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "CN_GetSummaryActivityPerMonth";
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetCNGetMonthlyFleetSummary report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetCNGetMonthlyFleetSummary report by fleet id=" + fleetId + " FromDateTime=" + fromDateTime;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Get Trips Summary Report
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="sensorNum"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetTripsSummaryReport_NewTZ(Int32 userId, string licensePlate, Int32 sensorNum, DateTime fromDateTime, DateTime toDateTime)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "GetTripsSummaryReportLicensePlateNew_NewTimeZone";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@sensorNum", SqlDbType.Int, sensorNum);
                sqlExec.AddCommandParam("@licensePlate", SqlDbType.VarChar, licensePlate);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetTripsSummaryReport report by licensePlate=" + licensePlate + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetTripsSummaryReport report by licensePlate=" + licensePlate + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        // Changes for TimeZone Feature end

        /// <summary>
        /// Get Trips Summary Report
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="sensorNum"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetTripsSummaryReport(Int32 userId, string licensePlate, Int32 sensorNum, DateTime fromDateTime, DateTime toDateTime)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "GetTripsSummaryReportLicensePlateNew";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@sensorNum", SqlDbType.Int, sensorNum);
                sqlExec.AddCommandParam("@licensePlate", SqlDbType.VarChar, licensePlate);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetTripsSummaryReport report by licensePlate=" + licensePlate + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetTripsSummaryReport report by licensePlate=" + licensePlate + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }



        /// <summary>
        /// Get Trips Summary Report
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="sensorNum"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetTripsSummaryReport(Int32 userId, string licensePlate, Int32 sensorNum, DateTime fromDateTime, DateTime toDateTime, Int32 driverId)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "GetTripsSummaryReportLicensePlateNew";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@sensorNum", SqlDbType.Int, sensorNum);
                sqlExec.AddCommandParam("@licensePlate", SqlDbType.VarChar, licensePlate);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetTripsSummaryReport report by licensePlate=" + licensePlate + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetTripsSummaryReport report by licensePlate=" + licensePlate + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }



        /// <summary>
        /// Get Trips Summary Report
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="sensorNum"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetTripsSummaryReportByFleet(Int32 userId, int FleetId, Int32 sensorNum, DateTime fromDateTime, DateTime toDateTime)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "GetTripsSummaryReportFleetNew";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@sensorNum", SqlDbType.Int, sensorNum);
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, FleetId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetTripsSummaryReport report by FleetId=" + FleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetTripsSummaryReport report by FleetId=" + FleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }




        /// <summary>
        /// Get Trips Summary Report
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="sensorNum"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetTripsSummaryReportByFleet(Int32 userId, int FleetId, Int32 sensorNum, DateTime fromDateTime, DateTime toDateTime, Int32 driverId)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "GetTripsSummaryReportFleetNew_Driver";
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, FleetId);
                sqlExec.AddCommandParam("@sensorNum", SqlDbType.Int, sensorNum);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetTripsSummaryReport report by FleetId=" + FleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetTripsSummaryReport report by FleetId=" + FleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        public DataSet GetIdlingDriverReport(int fleetId, DateTime dtFrom, DateTime dtTo, int driverId, int idlingThreshold, Int32 userId)
        {
            DataSet resultSet = null;
            string prefixMsg = "";
            try
            {
                prefixMsg = "GetIdlingDriverReport->Unable to .";
                SqlParameter[] sqlParams = new SqlParameter[6];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@fromDate", dtFrom);
                sqlParams[2] = new SqlParameter("@toDate", dtTo);
                sqlParams[3] = new SqlParameter("@driverId", driverId);
                sqlParams[4] = new SqlParameter("@idlingThreshold", idlingThreshold);
                sqlParams[5] = new SqlParameter("@userId", userId);


                // SQL statement

                string sql = "[evtFactEventsDriver_Report]";

                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }




        public DataSet GetUserSettingsReport(int OrgId)
        {
            DataSet resultSet = null;
            string prefixMsg = "";
            try
            {
                prefixMsg = "GetUserSettingsReport->Unable to data.";
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@OrgId", OrgId);
                // SQL statement

                string sql = "[ReportUserSettings]";

                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }



        public DataSet GetBoxSettingsReport(int OrgId)
        {
            DataSet resultSet = null;
            string prefixMsg = "";
            try
            {
                prefixMsg = "GetBoxSettingsReport->Unable to data.";
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@OrgId", OrgId);
                // SQL statement

                string sql = "[ReportBoxSettings]";

                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }



        /// <summary>
        /// Get Transportation Mileage Report
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="sensorNum"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetTransportationMileageReport(Int32 userId, string licensePlate, Int32 sensorNum, DateTime fromDateTime, DateTime toDateTime)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "TransportationMileageReport";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@sensorNum", SqlDbType.Int, sensorNum);
                sqlExec.AddCommandParam("@licensePlate", SqlDbType.VarChar, licensePlate);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve TransportationMileageReport report by licensePlate=" + licensePlate + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve TransportationMileageReport report by licensePlate=" + licensePlate + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Returns the first message and the last message having the same landmark name
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="landmark"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetTimeAtLandmarkReport(Int32 userId, string licensePlate, string landmark, DateTime fromDateTime, DateTime toDateTime)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "ReportLandmarkExtended";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@licensePlate", SqlDbType.VarChar, licensePlate);
                sqlExec.AddCommandParam("@landmark", SqlDbType.VarChar, landmark);
                sqlExec.AddCommandParam("@dtFrom", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@dtTo", SqlDbType.DateTime, toDateTime);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetTimeAtLandmarkReport report by licensePlate=" + licensePlate + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetTimeAtLandmarkReport report by licensePlate=" + licensePlate + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Returns the first message and the last message having the same landmark name
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="landmark"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet GetTimeAtLandmarkFleetReport(Int32 userId, Int32 fleetId, string landmark, DateTime fromDateTime, DateTime toDateTime)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "ReportLandmarkExtended4Fleet";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@landmark", SqlDbType.VarChar, landmark);
                sqlExec.AddCommandParam("@dtFrom", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@dtTo", SqlDbType.DateTime, toDateTime);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetTimeAtLandmarkFleetReport report by fleetId=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetTimeAtLandmarkFleetReport report by fleetId=" + fleetId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// <summary>
        /// Get Vehicles Status Report (CN Report)
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        public DataSet GetVehiclesStatusReport(Int32 OrgId)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "sp_VehiclesStatusReport";
                sqlExec.AddCommandParam("@OrgId", SqlDbType.Int, OrgId);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetVehiclesStatusReport report by OrgId=" + OrgId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetVehiclesStatusReport report by OrgId=" + OrgId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 72000;
                string sql = "GetBrickmanActivitySummaryReportPerBox";
                sqlExec.AddCommandParam("@boxId", SqlDbType.Int, boxId);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummarySaltSpreader report by boxId=" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetActivitySummarySaltSpreader report by boxId=" + boxId + " FromDateTime=" + fromDateTime + " ToDateTime=" + toDateTime + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }



        /// <summary>
        /// VehicleInfoDataDump
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public DataSet GetVehicleInfoDataDump(Int32 orgId)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 600;
                string sql = "sp_VehicleInfoDataDump";
                sqlExec.AddCommandParam("@orgId", SqlDbType.Int, orgId);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetVehicleInfoDataDump report by orgId=" + orgId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetVehicleInfoDataDump report by orgId=" + orgId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }



        /// <summary>
        /// CNElectronicInvoice
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public DataSet CNElectronicInvoice(DateTime from, DateTime to)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 600;
                string sql = "CNElectronicInvoice";
                sqlExec.AddCommandParam("@from", SqlDbType.DateTime, from);
                sqlExec.AddCommandParam("@to", SqlDbType.DateTime, to);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve CNElectronicInvoice report ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve CNElectronicInvoice report ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }



        /// <summary>
        /// Fleet Membership Report
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetFleetMembershipReport(Int32 userId)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 600;
                string sql = "sp_FleetMembershipReport";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetFleetMembershipReport report by userId=" + userId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetFleetMembershipReport report by userId=" + userId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }



        /// <summary>
        /// Fleet Membership Report
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetFleetMembershipReport(Int32 userId, Int16 activeVehicles)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 600;
                string sql = "sp_FleetMembershipReport_ActiveVehicles";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@activeVehicles", SqlDbType.Int, activeVehicles);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetFleetMembershipReport report by userId=" + userId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetFleetMembershipReport report by userId=" + userId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }



        /// <summary>
        /// Brickman Fleet Membership Report
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetBrickmanFleetMembershipReport(Int32 userId, Int32 activeVehicles)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 600;
                string sql = "BrickmanFleetMembershipReport";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@activeVehicles", SqlDbType.Int, activeVehicles);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetBrickmanFleetMembershipReport report by userId=" + userId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetBrickmanFleetMembershipReport report by userId=" + userId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// <summary>
        /// Fleet Membership Report
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetFleetMembershipReportUser(Int32 orgId)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 600;
                string sql = "sp_FleetMembershipReportUser";
                sqlExec.AddCommandParam("@orgId", SqlDbType.Int, orgId);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetFleetMembershipReportUser report by orgId=" + orgId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetFleetMembershipReportUser report by user=" + orgId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        #endregion

        #region Static Functions
        /// <summary>
        /// Replace street address with organization landmark
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="tblLandMarks"></param>
        /// <param name="geoMap"></param>
        /// <param name="streetAddress"></param>
        /// <returns>false in case of invalid position, otherwise returns true</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public static bool PrepareStreetAddress(double lat, double lon, DataTable tblLandMarks, VLF.MAP.ClientMapProxy geoMap, ref string streetAddress)
        {
            if (lat == 0 || lon == 0)
                return false; // invalid posiption

            int nearestLandmarkDistance = int.MaxValue;
            short landmarkSearchRadius = VLF.CLS.Def.Const.unassignedIntValue;
            VLF.MAP.GeoPoint currCoord = new VLF.MAP.GeoPoint(lat, lon);

            if (tblLandMarks != null && tblLandMarks.Rows.Count > 0 /*&& streetAddress != ""*/) // include street address
            {
                // Distance calculation variables
                VLF.MAP.GeoPoint landmarkCoord = new VLF.MAP.GeoPoint();
                int distanceBetweenGPS = 0;
                // Lookup for nearest landmark
                //DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
                foreach (DataRow ittr in tblLandMarks.Rows)
                {
                    landmarkCoord.Latitude = Convert.ToDouble(ittr["Latitude"]);
                    landmarkCoord.Longitude = Convert.ToDouble(ittr["Longitude"]);
                    landmarkSearchRadius = Convert.ToInt16(ittr["Radius"]);
                    // 1. Calculates distance between landmark and current location
                    distanceBetweenGPS = (int)(Math.Round(geoMap.GetDistance(landmarkCoord, currCoord)));
                    // 2. Replace street addresss with landmark name if position is inside the landmark
                    if ((distanceBetweenGPS > VLF.CLS.Def.Const.unassignedIntValue) &&
                       (distanceBetweenGPS <= landmarkSearchRadius) &&
                       (distanceBetweenGPS < nearestLandmarkDistance)
                       )
                    {
                        streetAddress = ittr["LandmarkName"].ToString().TrimEnd();
                        nearestLandmarkDistance = distanceBetweenGPS;
                    }
                }
            }
            return true;
        }


        /// <summary>
        ///            this is the function used for extracting email notifications from vlfLandmarks
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="tblLandMarks"></param>
        /// <param name="geoMap"></param>
        /// <param name="streetAddress"></param>
        /// <param name="description"></param>
        /// <param name="emailTo"></param>
        /// <returns></returns>
        public static bool PrepareStreetAddress(double lat, double lon,
                                                DataTable tblLandMarks,
                                                VLF.MAP.ClientMapProxy geoMap,
                                                ref string streetAddress,
                                                ref string description,
                                                ref string emailTo)
        {
            if (lat == 0 || lon == 0)
                return false; // invalid posiption

            int nearestLandmarkDistance = int.MaxValue;
            short landmarkSearchRadius = VLF.CLS.Def.Const.unassignedIntValue;
            VLF.MAP.GeoPoint currCoord = new VLF.MAP.GeoPoint(lat, lon);

            if (tblLandMarks != null && tblLandMarks.Rows.Count > 0 /*&& streetAddress != ""*/) // include street address
            {
                // Distance calculation variables
                VLF.MAP.GeoPoint landmarkCoord = new VLF.MAP.GeoPoint();
                int distanceBetweenGPS = 0;
                // Lookup for nearest landmark
                //DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
                foreach (DataRow ittr in tblLandMarks.Rows)
                {
                    landmarkCoord.Latitude = Convert.ToDouble(ittr["Latitude"]);
                    landmarkCoord.Longitude = Convert.ToDouble(ittr["Longitude"]);
                    landmarkSearchRadius = Convert.ToInt16(ittr["Radius"]);
                    // 1. Calculates distance between landmark and current location
                    distanceBetweenGPS = (int)(Math.Round(geoMap.GetDistance(landmarkCoord, currCoord)));
                    // 2. Replace street addresss with landmark name if position is inside the landmark
                    if (distanceBetweenGPS > VLF.CLS.Def.Const.unassignedIntValue &&
                          distanceBetweenGPS <= landmarkSearchRadius &&
                          distanceBetweenGPS < nearestLandmarkDistance
                        )
                    {
                        streetAddress = ittr["LandmarkName"].ToString().TrimEnd();
                        description = ittr["Description"].ToString().TrimEnd();
                        emailTo = ittr["Email"].ToString().TrimEnd();
                        nearestLandmarkDistance = distanceBetweenGPS;
                    }
                }
            }
            return true;
        }
        #endregion

        #region Protected Functions
        /// <summary>
        /// Retrieves stop duration between trips
        /// </summary>
        /// <param name="tblTripsStart"></param>
        /// <param name="tblTripsEnd"></param>
        /// <returns>DataSet [TripIndex],[Summary],[Remarks],[BoxId],
        /// [Date/Time],[Location],[Latitude],[Longitude],[VehicleId]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        protected DataTable GetStopDurationBetweenTrips(DataTable tblTripsStart, DataTable tblTripsEnd)
        {
            // calculates stop time between every trip (Note: last one always 0)
            TimeSpan interval;
            DataTable tblStopDurationBetweenTrips = new DataTable("StopDurationBetweenTrips");
            tblStopDurationBetweenTrips.Columns.Add("TripIndex", typeof(string));
            tblStopDurationBetweenTrips.Columns.Add("Summary", typeof(string));
            tblStopDurationBetweenTrips.Columns.Add("Remarks", typeof(string));
            tblStopDurationBetweenTrips.Columns.Add("BoxId", typeof(string));
            tblStopDurationBetweenTrips.Columns.Add("Date/Time", typeof(string));
            tblStopDurationBetweenTrips.Columns.Add("Location", typeof(string));
            tblStopDurationBetweenTrips.Columns.Add("Latitude", typeof(string));
            tblStopDurationBetweenTrips.Columns.Add("Longitude", typeof(string));
            tblStopDurationBetweenTrips.Columns.Add("VehicleId", typeof(string));
            tblStopDurationBetweenTrips.Columns.Add("IsLandmark", typeof(bool));


            object[] objRow = null;
            if (tblTripsEnd.Rows.Count == 1)
            {
                objRow = new object[tblStopDurationBetweenTrips.Columns.Count];
                objRow[0] = "1";
                objRow[1] = "00:00:00";
                objRow[2] = "0";
                objRow[3] = tblTripsEnd.Rows[0]["BoxId"].ToString();
                objRow[4] = tblTripsEnd.Rows[0]["Summary"].ToString();
                objRow[5] = tblTripsEnd.Rows[0]["Remarks"].ToString();
                objRow[6] = tblTripsEnd.Rows[0]["Latitude"].ToString();
                objRow[7] = tblTripsEnd.Rows[0]["Longitude"].ToString();
                objRow[8] = tblTripsEnd.Rows[0]["VehicleId"].ToString();
                objRow[9] = Convert.ToBoolean(tblTripsEnd.Rows[0]["IsLandmark"]);
                tblStopDurationBetweenTrips.Rows.Add(objRow);
            }
            else if (tblTripsEnd.Rows.Count > 1)
            {
                // default is two minutes
                int minStopInterval = -1;//GetConfigParameter("ASI",(short)VLF.CLS.Def.Enums.ConfigurationGroups.DAS,"Stop Interval",120);
                int reportTripIndex = 0;
                int tripIndex = 1;
                DateTime fromDate;
                DateTime toDate;
                for (; tripIndex <= tblTripsEnd.Rows.Count; ++tripIndex)
                {
                    // calculate interval between to trips
                    if (tblTripsStart.Rows.Count < tripIndex)
                    {
                        // wrong situation, TripsStart table cannot contain less rows than TripsEnd table
                        tblStopDurationBetweenTrips.Clear();
                        break;
                    }
                    fromDate = Convert.ToDateTime(tblTripsEnd.Rows[tripIndex - 1]["Summary"]);
                    if (tripIndex == tblTripsEnd.Rows.Count &&
                       tripIndex >= tblTripsStart.Rows.Count)
                    {
                        // last one 0 if new trip hasn't begun yet
                        interval = new TimeSpan(0, 0, 0);
                    }
                    else
                    {
                        toDate = Convert.ToDateTime(tblTripsStart.Rows[tripIndex]["Summary"]);
                        interval = new TimeSpan(toDate.Ticks - fromDate.Ticks);
                        // clear millisec
                        if (interval.Milliseconds != 0)
                            interval = interval = new TimeSpan(interval.Ticks - interval.Milliseconds * TimeSpan.TicksPerMillisecond);
                    }
                    if (interval.TotalSeconds > minStopInterval ||
                       (interval.TotalSeconds == 0 && tripIndex == tblTripsEnd.Rows.Count))
                    {
                        objRow = new object[tblStopDurationBetweenTrips.Columns.Count];
                        objRow[0] = ++reportTripIndex;
                        objRow[1] = interval.ToString();
                        objRow[2] = interval.TotalSeconds.ToString();
                        objRow[3] = tblTripsEnd.Rows[tripIndex - 1]["BoxId"].ToString();
                        objRow[4] = tblTripsEnd.Rows[tripIndex - 1]["Summary"].ToString();
                        objRow[5] = tblTripsEnd.Rows[tripIndex - 1]["Remarks"].ToString();
                        objRow[6] = tblTripsEnd.Rows[tripIndex - 1]["Latitude"].ToString();
                        objRow[7] = tblTripsEnd.Rows[tripIndex - 1]["Longitude"].ToString();
                        objRow[8] = tblTripsEnd.Rows[tripIndex - 1]["VehicleId"].ToString();
                        objRow[9] = Convert.ToBoolean(tblTripsEnd.Rows[tripIndex - 1]["IsLandmark"]);
                        tblStopDurationBetweenTrips.Rows.Add(objRow);
                    }
                }
            }
            return tblStopDurationBetweenTrips;
        }
        #endregion

        #region Configuration Methods
        /// <summary>
        /// Gets configuration parameter
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="groupID"></param>
        /// <param name="paramName"></param>
        /// <param name="defaultValue"></param>
        /// <returns>int value</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        private int GetConfigParameter(string moduleName, short groupID, string paramName, int defaultValue)
        {
            int paramValue = defaultValue;
            DB.Configuration config = new DB.Configuration(sqlExec);

            // take Module ID in DB
            short moduleID = config.GetConfigurationModuleTypeId(moduleName);
            if (moduleID == VLF.CLS.Def.Const.unassignedShortValue)
            {
                throw new VLF.ERR.DASAppResultNotFoundException("Cannot find '" + moduleName + "' in DB.");
            }

            // get parameter from DB
            try
            {
                paramValue = Convert.ToInt32(config.GetConfigurationValue(moduleID, groupID, paramName));
            }
            catch
            {
                // TODO: log error
                //throw new VLF.ERR.DASException("'" + paramName + "' is not set in DB. Setting up default value: " + paramValue.ToString() + ".");
            }
            return paramValue;
        }

        /// <summary>
        /// Gets configuration parameter
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="groupID"></param>
        /// <param name="paramName"></param>
        /// <param name="defaultValue"></param>
        /// <returns>string value</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        private string GetConfigParameter(string moduleName, short groupID, string paramName, string defaultValue)
        {
            string paramValue = defaultValue;
            DB.Configuration config = new DB.Configuration(sqlExec);

            // take Module ID in DB
            short moduleID = config.GetConfigurationModuleTypeId(moduleName);
            if (moduleID == VLF.CLS.Def.Const.unassignedShortValue)
            {
                throw new VLF.ERR.DASAppResultNotFoundException("Cannot find '" + moduleName + "' in DB.");
            }

            // get parameter from DB
            try
            {
                paramValue = config.GetConfigurationValue(moduleID, groupID, paramName);
            }
            catch
            {
                throw new VLF.ERR.DASException("'" + paramName + "' is not set in DB. Setting up default value: " + paramValue.ToString() + ".");
            }
            return paramValue;
        }

        /// <summary>
        /// Gets configuration parameter
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="groupID"></param>
        /// <param name="paramName"></param>
        /// <param name="defaultValue"></param>
        /// <returns>short value</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        private short GetConfigParameter(string moduleName, short groupID, string paramName, short defaultValue)
        {
            short paramValue = defaultValue;
            DB.Configuration config = new DB.Configuration(sqlExec);

            // take Module ID in DB
            short moduleID = config.GetConfigurationModuleTypeId(moduleName);
            if (moduleID == VLF.CLS.Def.Const.unassignedShortValue)
            {
                throw new VLF.ERR.DASAppResultNotFoundException("Cannot find '" + moduleName + "' in DB.");
            }

            // get parameter from DB
            try
            {
                paramValue = Convert.ToInt16(config.GetConfigurationValue(moduleID, groupID, paramName));
            }
            catch
            {
                throw new VLF.ERR.DASException("'" + paramName + "' is not set in DB. Setting up default value: " + paramValue.ToString() + ".");
            }
            return paramValue;
        }

        #endregion Configuration Methods



        public DataSet GetBoxStatusReport(int fleetId)
        {
            DataSet resultDataSet = null;
            try
            {
                string sql = "";

                //Executes SQL statement
                sqlExec.CommandTimeout = 600;
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve get box status by fleet id=" + fleetId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {

                string prefixMsg = "Unable to retrieve get box status by fleet id=" + fleetId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return resultDataSet;
        }

        // Changes for TimeZone Feature start

        public DataSet GetVehiclesStatusReport_NewTZ(int userId, int fleetId)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[2];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@userId", userId);


                // SQL statement
                string sql = "VehiclesStatusReport_NewTimeZone";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }

        // Changes for TimeZone Feature end


        public DataSet GetVehiclesStatusReport(int userId, int fleetId)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[2];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@userId", userId);


                // SQL statement
                string sql = "VehiclesStatusReport";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }

        // Changes for TimeZone Feature start
        public DataSet GetVehiclesStatusReport_NewTZ(int userId, int fleetId, Int16 activeVehicles)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@userId", userId);
                sqlParams[2] = new SqlParameter("@activeVehicles", activeVehicles);


                // SQL statement
                string sql = "VehiclesStatusReport_ActiveVehicles_NewTimeZone";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }



        // Changes for TimeZone Feature end


        public DataSet GetVehiclesStatusReport(int userId, int fleetId, Int16 activeVehicles)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@userId", userId);
                sqlParams[2] = new SqlParameter("@activeVehicles", activeVehicles);


                // SQL statement
                string sql = "VehiclesStatusReport_ActiveVehicles";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }







        public DataSet GetBoxStartEndOdometerEngHrs(DateTime fromDate, DateTime toDate, int fleetId)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@fromDate", fromDate);
                sqlParams[1] = new SqlParameter("@toDate", toDate);
                sqlParams[2] = new SqlParameter("@fleetId", fleetId);


                // SQL statement
                string sql = "GetBoxStartEndOdometerEngHrs";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }

        public DataSet GetOnOffRoadMilesReport(int userId, int fleetId, DateTime DateFrom, DateTime DateTo)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[]
				{
					new SqlParameter("@User", userId),
					new SqlParameter("@Fleet", fleetId),
					new SqlParameter("@DateFr", DateFrom),
					new SqlParameter("@DateTo", DateTo)
				};
                string sql = "evtOnOff_Road_Miles";
                resultSet = this.sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException2)
            {
                throw new DASException(prefixMsg + " " + objException2.Message);
            }
            return resultSet;
        }
        public DataSet GetOnOffRoadMilesReport(int userId, string NodeCode, DateTime DateFrom, DateTime DateTo)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[]
				{
					new SqlParameter("@User", userId),
					new SqlParameter("@NodeCode", NodeCode),
					new SqlParameter("@DateFr", DateFrom),
					new SqlParameter("@DateTo", DateTo)
				};
                string sql = "evtOnOff_Road_Miles_Hierarchy";
                resultSet = this.sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException2)
            {
                throw new DASException(prefixMsg + " " + objException2.Message);
            }
            return resultSet;
        }


        public DataSet DashBoard_HeartBeat(int userId)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@userId", userId);


                // SQL statement
                string sql = "DashBoard_HeartBeat";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }




        public DataSet DashBoard_AHA(int userId, int Top, int Hours)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@Top", Top);
                sqlParams[2] = new SqlParameter("@Hours", Hours);


                // SQL statement
                string sql = "DashBoard_AHA_NEW";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }



        public DataSet OrganizationHistoryDateRangeValidation(int userId, DateTime fromDate, DateTime toDate)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@fromDate", fromDate);
                sqlParams[2] = new SqlParameter("@toDate", toDate);


                // SQL statement
                string sql = "OrganizationHistoryDateRangeValidation";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }


        public DataSet GetActivityOutsideLandmark(int userId, int fleetId, int vehicleId, DateTime fromDate, DateTime toDate)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[4];
                string sql = "";
                if (vehicleId != 0)
                {
                    sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);
                    sql = "GetActivityOutsideLandmarkPerVehicle";
                }
                else
                {
                    sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                    sql = "GetActivityOutsideLandmarkPerFleet";

                }

                sqlParams[1] = new SqlParameter("@fromDate", fromDate);
                sqlParams[2] = new SqlParameter("@toDate", toDate);
                sqlParams[3] = new SqlParameter("@userId", userId);

                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }



        public DataSet GetActivityOutsideLandmark(int userId, int fleetId, int vehicleId, DateTime fromDate, DateTime toDate, Int16 activeVehicles)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[5];
                string sql = "";
                if (vehicleId != 0)
                {
                    sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);
                    sql = "GetActivityOutsideLandmarkPerVehicle";
                }
                else
                {
                    sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                    sql = "GetActivityOutsideLandmarkPerFleet_ActiveVehicles";

                }

                sqlParams[1] = new SqlParameter("@fromDate", fromDate);
                sqlParams[2] = new SqlParameter("@toDate", toDate);
                sqlParams[3] = new SqlParameter("@userId", userId);
                sqlParams[4] = new SqlParameter("@activeVehicles", activeVehicles);

                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }




        public DataSet GetActivityInLandmark(int userId, int fleetId, int vehicleId, DateTime fromDate, DateTime toDate)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[4];
                string sql = "";
                if (vehicleId != 0)
                {
                    sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);
                    sql = "GetActivityInLandmarkPerVehicle";
                }
                else
                {
                    sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                    sql = "GetActivityInLandmarkPerFleet";

                }

                sqlParams[1] = new SqlParameter("@fromDate", fromDate);
                sqlParams[2] = new SqlParameter("@toDate", toDate);
                sqlParams[3] = new SqlParameter("@userId", userId);

                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }



        public DataSet Dashboard_Idling(int fleetId, DateTime fromDate, DateTime toDate)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@fromDate", fromDate);
                sqlParams[2] = new SqlParameter("@toDate", toDate);


                // SQL statement
                string sql = "Dashboard_Idling";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }



        public DataSet Dashboard_Idling(int userId, int fleetId, DateTime fromDate, DateTime toDate)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[4];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@fromDate", fromDate);
                sqlParams[2] = new SqlParameter("@toDate", toDate);
                sqlParams[3] = new SqlParameter("@userId", userId);


                // SQL statement
                string sql = "Dashboard_Idling";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }


        public DataSet Dashboard_Violations(int fleetId, DateTime fromDate, DateTime toDate, int userId)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[4];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@fromDate", fromDate);
                sqlParams[2] = new SqlParameter("@toDate", toDate);
                sqlParams[3] = new SqlParameter("@userId", userId);


                // SQL statement
                string sql = "Dashboard_Violations";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }

        // Changes for TimeZone Feature start

        public DataSet AHA_Report_NewTZ(int UserId, int Top, int Hours)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@UserId", UserId);
                sqlParams[1] = new SqlParameter("@Top", Top);
                sqlParams[2] = new SqlParameter("@Hours", Hours);



                // SQL statement
                string sql = "DashBoard_AHA_Report_NewTimeZone";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }

        // Changes for TimeZone Feature end

        public DataSet AHA_Report(int UserId, int Top, int Hours)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@UserId", UserId);
                sqlParams[1] = new SqlParameter("@Top", Top);
                sqlParams[2] = new SqlParameter("@Hours", Hours);



                // SQL statement
                string sql = "DashBoard_AHA_Report";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }
        

        public DataSet evtViolation_Report(int fleetId, DateTime fromDate, DateTime toDate, int userId)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[4];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@fromDate", fromDate);
                sqlParams[2] = new SqlParameter("@toDate", toDate);
                sqlParams[3] = new SqlParameter("@userId", userId);



                // SQL statement
                string sql = "Dashboard_Violations_Report";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }



        public DataSet evtFactEvents_Report(int fleetId, DateTime fromDate, DateTime toDate, int userId)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[4];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@fromDate", fromDate);
                sqlParams[2] = new SqlParameter("@toDate", toDate);
                sqlParams[3] = new SqlParameter("@userId", userId);




                // SQL statement
                string sql = "evtFactEvents_Report";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }




        public DataSet evtFactEvents_Report(int fleetId, DateTime fromDate, DateTime toDate, Int16 idlingHrs, int userId)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[5];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@fromDate", fromDate);
                sqlParams[2] = new SqlParameter("@toDate", toDate);
                sqlParams[3] = new SqlParameter("@idlingHrs", idlingHrs);
                sqlParams[4] = new SqlParameter("@userId", userId);




                // SQL statement
                string sql = "evtFactEventsByIdling_Report";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }


        public DataSet evtDriverViolations(Int32 userId, Int32 driverId, Int32 maskViolations, DateTime dtFrom, DateTime dtTo, int speed)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[6];
                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@driverId", driverId);
                sqlParams[2] = new SqlParameter("@maskViolations", maskViolations);
                sqlParams[3] = new SqlParameter("@fromDate", dtFrom);
                sqlParams[4] = new SqlParameter("@toDate", dtTo);
                sqlParams[5] = new SqlParameter("@speed", speed);




                // SQL statement
                string sql = "evtDriverViolations";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }




        public DataSet GarminMessages_Report(int fleetId, DateTime fromDate, DateTime toDate, int userId)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[4];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@fromDate", fromDate);
                sqlParams[2] = new SqlParameter("@toDate", toDate);
                sqlParams[3] = new SqlParameter("@userId", userId);




                // SQL statement
                string sql = "GarminMessages";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }


        /// <summary>
        /// MaintenanceSpecialReport
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="sensorNum"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public DataSet MaintenanceSpecialReport(Int32 fleetId, Int32 userId)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 600;
                string sql = "MaintenanceSpecialReport";
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceSpecialReport report by fleetId=" + fleetId + " userId=" + userId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceSpecialReport report by fleetId=" + fleetId + " userId=" + userId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
            DataSet sqlDataSet = new DataSet();
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.CommandTimeout = 600;
                string sql = "evtViolationsFuel";
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDate);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDate);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve evtViolationsFuel report by fleetId=" + fleetId + " userId=" + userId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve evtViolationsFuel report by fleetId=" + fleetId + " userId=" + userId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet evtDriverViolationsFleet_Report(int fleetId, DateTime fromDate, DateTime toDate, int userId, int maskViolations, string ViolationPoints)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            string[] points = ViolationPoints.Split(new char[]
			{
				';'
			});
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[]
				{
					new SqlParameter("@fleetId", fleetId),
					new SqlParameter("@fromDate", fromDate),
					new SqlParameter("@toDate", toDate),
					new SqlParameter("@userId", userId),
					new SqlParameter("@maskViolations", maskViolations),
					new SqlParameter("@Speed120", points[0]),
					new SqlParameter("@Speed130", points[1]),
					new SqlParameter("@Speed140", points[2]),
					new SqlParameter("@AccExtreme", points[3]),
					new SqlParameter("@AccHarsh", points[4]),
					new SqlParameter("@BrakingExtreme", points[5]),
					new SqlParameter("@BrakingHarsh", points[6]),
					new SqlParameter("@SeatBelt", points[7])
				};
                string sql = "evtDriverViolationsFleet_Report";
                sqlExec.CommandTimeout = 72000;
                resultSet = this.sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException2)
            {
                throw new DASException(prefixMsg + " " + objException2.Message);
            }
            return resultSet;
        }


        public DataSet evtOverTimeDurationInLandmark(int userId, int fleetId, DateTime DateFrom, DateTime DateTo)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[]
				{
					new SqlParameter("@fleetId", fleetId),
					new SqlParameter("@fromDate", DateFrom),
					new SqlParameter("@toDate", DateTo),
					new SqlParameter("@userId", userId)
				};
                string sql = "evtOverTimeDurationInLandmark_Report";
                resultSet = this.sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException2)
            {
                throw new DASException(prefixMsg + " " + objException2.Message);
            }
            return resultSet;
        }
        public DataSet ReportBoxGeozone(int UserId)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[]
				{
					new SqlParameter("@UserId", UserId)
				};
                string sql = "ReportBoxGeozone";
                resultSet = this.sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException2)
            {
                throw new DASException(prefixMsg + " " + objException2.Message);
            }
            return resultSet;
        }


        public DataSet evtViolationsSpeedInLandmark(int userId, int fleetId, DateTime DateFrom, DateTime DateTo)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[]
				{
					new SqlParameter("@fleetId", fleetId),
					new SqlParameter("@fromDate", DateFrom),
					new SqlParameter("@toDate", DateTo),
					new SqlParameter("@userId", userId)
				};
                string sql = "evtViolationsSpeedInLandmark";
                resultSet = this.sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException2)
            {
                throw new DASException(prefixMsg + " " + objException2.Message);
            }
            return resultSet;
        }
        public DataSet evtViolationsSpeedInLandmark_Hierarchy(int userId, string nodeCode, DateTime DateFrom, DateTime DateTo)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[]
				{
					new SqlParameter("@nodeCode", nodeCode),
					new SqlParameter("@fromDate", DateFrom),
					new SqlParameter("@toDate", DateTo),
					new SqlParameter("@userId", userId)
				};
                string sql = "evtViolationsSpeedInLandmark_Hierarchy";
                resultSet = this.sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException2)
            {
                throw new DASException(prefixMsg + " " + objException2.Message);
            }
            return resultSet;
        }


        public DataSet GetActivitySummaryReportPerFleet_Special_Hierarchy(int userId, string nodeCode, DateTime fromDateTime, DateTime toDateTime, short sensorNum)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                this.sqlExec.ClearCommandParameters();
                string sql = "GetActivitySummaryReportPerFleet_Special_Hierarchy";
                this.sqlExec.AddCommandParam("@fleetId", SqlDbType.VarChar, nodeCode);
                this.sqlExec.AddCommandParam("@sensorNum", SqlDbType.Int, sensorNum);
                this.sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                this.sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                this.sqlExec.AddCommandParam("@summary", SqlDbType.Int, 0);
                this.sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                this.sqlExec.CommandTimeout = 72000;
                sqlDataSet = this.sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Concat(new object[]
				{
					"Unable to retrieve GetActivitySummaryReportPerFleet_Special_Hierarchy report by nodeCode='",
					nodeCode,
					"' FromDateTime=",
					fromDateTime,
					" ToDateTime=",
					toDateTime,
					". "
				});
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException2)
            {
                string prefixMsg = string.Concat(new object[]
				{
					"Unable to retrieve GetActivitySummaryReportPerFleet_Special_Hierarchy report by nodeCode='",
					nodeCode,
					"' FromDateTime=",
					fromDateTime,
					" ToDateTime=",
					toDateTime,
					". "
				});
                throw new DASException(prefixMsg + " " + objException2.Message);
            }
            return sqlDataSet;
        }


        public DataSet GetSensorActivityReport_Hierarchy(int userId, string nodeCode, DateTime fromDateTime, DateTime toDateTime)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                this.sqlExec.ClearCommandParameters();
                string sql = "SensorActivityReport_Hierarchy";
                this.sqlExec.AddCommandParam("@nodeCode", SqlDbType.VarChar, nodeCode);
                this.sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                this.sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                this.sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                this.sqlExec.CommandTimeout = 600;
                sqlDataSet = this.sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Concat(new object[]
				{
					"Unable to retrieve GetSensorActivityReport_Hierarchy report by nodeCode=",
					nodeCode,
					" FromDateTime=",
					fromDateTime,
					" ToDateTime=",
					toDateTime,
					". "
				});
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException2)
            {
                string prefixMsg = string.Concat(new object[]
				{
					"Unable to retrieve GetSensorActivityReport_Hierarchy report by nodeCode=",
					nodeCode,
					" FromDateTime=",
					fromDateTime,
					" ToDateTime=",
					toDateTime,
					". "
				});
                throw new DASException(prefixMsg + " " + objException2.Message);
            }
            return sqlDataSet;
        }


        public DataSet GetSpeedDistributionReport_Hierarchy(int userId, string nodeCode, DateTime DateFrom, DateTime DateTo)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[]
				{
					new SqlParameter("@userId", userId),
					new SqlParameter("@nodeCode", nodeCode),
					new SqlParameter("@dateFr", DateFrom),
					new SqlParameter("@dateTo", DateTo)
				};
                string sql = "evtViolationsSpeedDistribution_Hierarchy";
                resultSet = this.sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException2)
            {
                throw new DASException(prefixMsg + " " + objException2.Message);
            }
            return resultSet;
        }


        public DataSet GetSpeedViolationsSummaryReportForFleet_RoadSpeed_Hierarchy(int userId, string nodeCode, DateTime fromDateTime, DateTime toDateTime, int Type, string colorFilter, short postedSpeedOnly)
        {
            DataSet sqlDataSet = new DataSet();
            try
            {
                this.sqlExec.ClearCommandParameters();
                this.sqlExec.CommandTimeout = 720000;
                string sql = "GetSpeedViolationsSummaryReportForFleet_RoadSpeed_Hierarchy";
                this.sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                this.sqlExec.AddCommandParam("@nodeCode", SqlDbType.Int, nodeCode);
                this.sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
                this.sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
                this.sqlExec.AddCommandParam("@Type", SqlDbType.Int, Type);
                this.sqlExec.AddCommandParam("@colorFilter", SqlDbType.VarChar, colorFilter);
                this.sqlExec.AddCommandParam("@postedSpeedOnly", SqlDbType.Bit, postedSpeedOnly);
                sqlDataSet = this.sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Concat(new object[]
				{
					"Unable to retrieve GetSpeedViolationsSummaryReportForFleet_RoadSpeed report by nodeCode ='",
					nodeCode,
					"' FromDateTime=",
					fromDateTime,
					" ToDateTime=",
					toDateTime,
					". "
				});
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException2)
            {
                string prefixMsg = string.Concat(new object[]
				{
					"Unable to retrieve GetSpeedViolationsSummaryReportForFleet_RoadSpeed report by nodeCode ='",
					nodeCode,
					"' FromDateTime=",
					fromDateTime,
					" ToDateTime=",
					toDateTime,
					". "
				});
                throw new DASException(prefixMsg + " " + objException2.Message);
            }
            return sqlDataSet;
        }


        public int ReportSchedulesDriver_Status(int DriverId, Int16 ReportGuiId, DateTime FromDate, DateTime ToDate, string Email, string LinkURL, string Status)
        {
            int rowsAffected = 0;
            try
            {
                //Prepares SQL statement
                string sql = "ReportSchedulesDriver_Status";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, DriverId);
                sqlExec.AddCommandParam("@ReportGuiId ", SqlDbType.SmallInt, ReportGuiId);
                sqlExec.AddCommandParam("@FromDate", SqlDbType.DateTime, FromDate);
                sqlExec.AddCommandParam("@ToDate", SqlDbType.DateTime, ToDate);
                sqlExec.AddCommandParam("@Email", SqlDbType.VarChar, Email);
                sqlExec.AddCommandParam("@LinkURL", SqlDbType.VarChar, LinkURL);
                sqlExec.AddCommandParam("@Status", SqlDbType.VarChar, Status);

                //Executes SQL statement
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("ReportSchedulesDriver_Status -> SqlException:  DriverId={0}", DriverId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("ReportSchedulesDriver_Status -> Exception:  DriverId={0}", DriverId);
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected; ;
        }




        public DataSet ReportSchedulesDriver_Get(string Status)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[]
				{
					new SqlParameter("@Status", Status),
				};
                string sql = "ReportSchedulesDriver_Get";
                resultSet = this.sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException2)
            {
                throw new DASException(prefixMsg + " " + objException2.Message);
            }
            return resultSet;
        }


    }
}

