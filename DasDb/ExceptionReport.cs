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
    /// Generates reporting functionality
    /// </summary>
    public class ExceptionReport
    {
        private DataTable tblReportData = null;
        private SQLExecuter sqlExec = null;
        /// <summary>
        /// Constructor
        /// </summary>
        public ExceptionReport(SQLExecuter sqlExec)
        {
            this.sqlExec = sqlExec;
            #region ReportData table definition
            tblReportData = new DataTable("ReportData");
            tblReportData.Columns.Add("Type", typeof(string));
            tblReportData.Columns.Add("DateTime", typeof(DateTime));
            tblReportData.Columns.Add("VehicleDescription", typeof(string));
            tblReportData.Columns.Add("Remarks1", typeof(string));
            tblReportData.Columns.Add("Remarks2", typeof(string));
            tblReportData.Columns.Add("BoxId", typeof(string));
            tblReportData.Columns.Add("VehicleId", typeof(string));
            #endregion
        }
        /// <summary>
        /// Generate report
        /// </summary>
        /// <param name="sosLimit"></param>
        /// <param name="fromTD"></param>
        /// <param name="toTD"></param>
        /// <param name="noDoorSnsHrs"></param>
        /// <param name="rowInData"></param>
        /// <param name="vehicleId"></param>
        /// <param name="vehicleDescription"></param>
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
        /// <param name="userId"></param>
        /// <param name="driverDoor"></param>
        /// <param name="passengerDoor"></param>
        /// <param name="sideHopperDoor"></param>
        /// <param name="rearHopperDoor"></param>
        /// <param name="includeCurrentTar"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void FillReport(int boxId, short sosLimit, DateTime fromTD, DateTime toTD, int noDoorSnsHrs, DataSet rowInData, Int64 vehicleId, string vehicleDescription, bool includeTar, bool includeMobilize, bool fifteenSecDoorSns, bool leash50, bool mainAndBackupBatterySns, bool tamperSns, bool anyPanicSns, bool threeKeypadAttemptsSns, bool altGPSAntennaSns, bool controllerStatus, bool leashBrokenSns, int userId, bool driverDoor, bool passengerDoor, bool sideHopperDoor, bool rearHopperDoor, bool includeCurrentTar,
            bool Locker1, bool Locker2, bool Locker3, bool Locker4, bool Locker5, bool Locker6, bool Locker7, bool Locker8, bool Locker9)
        {
            #region Process RealTime information
            #region Add Current in TAR mode to report
            if (includeCurrentTar)
            {
                //TimeSpan currTarOnTotalSec = new TimeSpan(0, 0, 0);
                DataSet dsCurrTarOn = GetCurrentTarOnInfo(userId, boxId);
                //if (dsCurrTarOn.Tables[0].Rows[0]["LastCommunicatedDateTime"] == System.DBNull.Value)
                //    dsCurrTarOn.Tables[0].Rows[0]["LastCommunicatedDateTime"] = VLF.CLS.Def.Const.unassignedDateTime;
                //else
                //    currTarOnTotalSec = new TimeSpan(Convert.ToDateTime(dsCurrTarOn.Tables[0].Rows[0]["LastCommunicatedDateTime"]).Ticks - Convert.ToDateTime(dsCurrTarOn.Tables[0].Rows[0]["TarOnDT"]).Ticks);
                if (dsCurrTarOn != null && dsCurrTarOn.Tables.Count > 0 && dsCurrTarOn.Tables[0].Rows.Count > 0)
                {
                    // if vehicle currently is in TAR mode, include into report
                    AddReportRow("Currently in TAR mode",
                            Convert.ToDateTime(dsCurrTarOn.Tables[0].Rows[0]["LastCommunicatedDateTime"]),
                            vehicleDescription,
                            "From: " + Convert.ToDateTime(dsCurrTarOn.Tables[0].Rows[0]["TarOnDT"]).ToString(),
                        /*currTarOnTotalSec.TotalSeconds.ToString()*/ " ",
                            boxId, vehicleId);
                }
            }
            #endregion
            #endregion

            #region Process historical information
            DateTime lastDriverDoorDateTime = VLF.CLS.Def.Const.unassignedDateTime;
            DateTime lastPassengerDoorDateTime = VLF.CLS.Def.Const.unassignedDateTime;
            DateTime lastSideHopperDoorDoorDateTime = VLF.CLS.Def.Const.unassignedDateTime;
            DateTime lastRearHopperDoorDateTime = VLF.CLS.Def.Const.unassignedDateTime;
            DateTime lastLocker1DateTime = VLF.CLS.Def.Const.unassignedDateTime;
            DateTime lastLocker2DateTime = VLF.CLS.Def.Const.unassignedDateTime;
            DateTime lastLocker3DateTime = VLF.CLS.Def.Const.unassignedDateTime;
            DateTime lastLocker4DateTime = VLF.CLS.Def.Const.unassignedDateTime;
            DateTime lastLocker5DateTime = VLF.CLS.Def.Const.unassignedDateTime;
            DateTime lastLocker6DateTime = VLF.CLS.Def.Const.unassignedDateTime;
            DateTime lastLocker7DateTime = VLF.CLS.Def.Const.unassignedDateTime;
            DateTime lastLocker8DateTime = VLF.CLS.Def.Const.unassignedDateTime;
            DateTime lastLocker9DateTime = VLF.CLS.Def.Const.unassignedDateTime;
            try
            {
                if (rowInData != null && rowInData.Tables.Count > 0 && rowInData.Tables[0].Rows.Count > 0)
                {

                    string customProp = "";
                    int sensorId = VLF.CLS.Def.Const.unassignedIntValue;
                    Enums.MessageType msgType;
                    int sosMode = 0;

                    DataSet dsSosEvents = null;
                    foreach (DataRow ittr in rowInData.Tables[0].Rows)
                    {
                        try
                        {
                            boxId = Convert.ToInt32(ittr["BoxId"]);
                            msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);

                            #region Process sensor or alarm message
                            if (msgType == Enums.MessageType.Sensor ||
                                msgType == Enums.MessageType.SensorExtended ||
                                msgType == Enums.MessageType.Alarm ||
                                msgType == Enums.MessageType.MBAlarm)
                            {
                                // Add sensor info into result dataset
                                try
                                {
                                    customProp = ittr["CustomProp"].ToString().TrimEnd();
                                    sensorId = Convert.ToInt32(Util.PairFindValue(VLF.CLS.Def.Const.keySensorNum, customProp));
                                }
                                catch
                                {
                                    customProp = "";
                                    sensorId = VLF.CLS.Def.Const.unassignedIntValue;
                                }

                                #region Add TAR to report
                                if (sensorId == 40 && includeTar)
                                {
                                    string sensorStatus = "";
                                    try
                                    {
                                        sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
                                        if (sensorStatus == VLF.CLS.Def.Const.valOFF)
                                            sensorStatus = "OFF";
                                        else if (sensorStatus == VLF.CLS.Def.Const.valON)
                                            sensorStatus = "ON";
                                        else
                                            sensorStatus = " ";
                                    }
                                    catch (Exception e)
                                    {
                                        // log exception
                                    }
                                    AddReportRow("Any TAR mode events",
                                        Convert.ToDateTime(ittr["OriginDateTime"]),
                                        vehicleDescription,
                                        "Transport and Repair",
                                        sensorStatus,
                                        boxId,
                                        vehicleId);
                                }
                                #endregion

                                #region Calculate Sos
                                else if (sensorId == 18 && sosLimit != VLF.CLS.Def.Const.unassignedShortValue)
                                {
                                    string sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
                                    //if(sensorStatus == VLF.CLS.Def.Const.valOFF)
                                    //	sensorStatus = "Off";
                                    //else if(sensorStatus == VLF.CLS.Def.Const.valON)
                                    //	sensorStatus = "On";
                                    //else
                                    //	sensorStatus = " ";

                                    ++sosMode;
                                    if (dsSosEvents == null)
                                    {
                                        dsSosEvents = new DataSet();
                                        dsSosEvents.Tables.Add();
                                        dsSosEvents.Tables[0].Columns.Add("DT", typeof(DateTime));
                                        dsSosEvents.Tables[0].Columns.Add("Action", typeof(string));
                                    }
                                    object[] objRow = new object[dsSosEvents.Tables[0].Columns.Count];
                                    objRow[0] = ittr["OriginDateTime"];
                                    objRow[1] = sensorStatus;
                                    dsSosEvents.Tables[0].Rows.Add(objRow);
                                }
                                #endregion

                                #region Add Driver Door event
                                else if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && driverDoor && sensorId == 9/*Driver Door*/)
                                {
                                    if (lastDriverDoorDateTime == VLF.CLS.Def.Const.unassignedDateTime) // first time
                                        lastDriverDoorDateTime = fromTD;

                                    TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(ittr["OriginDateTime"]).Ticks - lastDriverDoorDateTime.Ticks);
                                    if (currDuration.TotalHours > noDoorSnsHrs)
                                    {
                                        AddReportRow("No driver door opening/closing event within " + noDoorSnsHrs + " hours (From - To)",
                                            lastDriverDoorDateTime, vehicleDescription,
                                            Convert.ToDateTime(ittr["OriginDateTime"]).ToString(), " ", boxId, vehicleId);
                                    }
                                    lastDriverDoorDateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
                                }
                                #endregion

                                #region Add Passenger Door event
                                else if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && passengerDoor && sensorId == 10)
                                {
                                    if (lastPassengerDoorDateTime == VLF.CLS.Def.Const.unassignedDateTime) // first time
                                        lastPassengerDoorDateTime = fromTD;

                                    TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(ittr["OriginDateTime"]).Ticks - lastPassengerDoorDateTime.Ticks);
                                    if (currDuration.TotalHours > noDoorSnsHrs)
                                    {
                                        AddReportRow("No passenger door opening/closing event within " + noDoorSnsHrs + " hours (From - To)",
                                            lastPassengerDoorDateTime, vehicleDescription,
                                            Convert.ToDateTime(ittr["OriginDateTime"]).ToString(), " ", boxId, vehicleId);
                                    }
                                    lastPassengerDoorDateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
                                }
                                #endregion

                                #region Add Side Hopper Door event
                                else if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && sideHopperDoor && sensorId == 11/*Side Hopper Door*/)
                                {
                                    if (lastSideHopperDoorDoorDateTime == VLF.CLS.Def.Const.unassignedDateTime) // first time
                                        lastSideHopperDoorDoorDateTime = fromTD;

                                    TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(ittr["OriginDateTime"]).Ticks - lastSideHopperDoorDoorDateTime.Ticks);
                                    if (currDuration.TotalHours > noDoorSnsHrs)
                                    {
                                        AddReportRow("No side hopper door opening/closing event within " + noDoorSnsHrs + " hours (From - To)",
                                            lastSideHopperDoorDoorDateTime, vehicleDescription,
                                            Convert.ToDateTime(ittr["OriginDateTime"]).ToString(), " ", boxId, vehicleId);
                                    }
                                    lastSideHopperDoorDoorDateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
                                }
                                #endregion

                                #region Add Rear Hopper Door event
                                else if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && rearHopperDoor && sensorId == 12/*Rear Hopper Door*/)
                                {
                                    if (lastRearHopperDoorDateTime == VLF.CLS.Def.Const.unassignedDateTime) // first time
                                        lastRearHopperDoorDateTime = fromTD;

                                    TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(ittr["OriginDateTime"]).Ticks - lastRearHopperDoorDateTime.Ticks);
                                    if (currDuration.TotalHours > noDoorSnsHrs)
                                    {
                                        AddReportRow("No rear hopper door opening/closing event within " + noDoorSnsHrs + " hours (From - To)",
                                            lastRearHopperDoorDateTime, vehicleDescription,
                                            Convert.ToDateTime(ittr["OriginDateTime"]).ToString(), " ", boxId, vehicleId);
                                    }
                                    lastRearHopperDoorDateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
                                }
                                #endregion

                                #region Add Vehicle Immobilize/Mobilize to report
                                else if (sensorId == 39 && includeMobilize)
                                {
                                    string sensorStatus = "";
                                    try
                                    {
                                        sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
                                        if (sensorStatus == VLF.CLS.Def.Const.valOFF)
                                            sensorStatus = "Mobilized";
                                        else if (sensorStatus == VLF.CLS.Def.Const.valON)
                                            sensorStatus = "Immobilized";
                                        else
                                            sensorStatus = " ";
                                    }
                                    catch (Exception e)
                                    {
                                        // log exception
                                    }
                                    AddReportRow("Vehicle immobilization events",
                                        Convert.ToDateTime(ittr["OriginDateTime"]),
                                        vehicleDescription,
                                        "Vehicle",
                                        sensorStatus,
                                        boxId,
                                        vehicleId);
                                }
                                #endregion

                                #region Add 15 sec Door Sensor
                                else if (sensorId == 36 && fifteenSecDoorSns)
                                {
                                    string sensorStatus = "";
                                    try
                                    {
                                        sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
                                        if (sensorStatus == VLF.CLS.Def.Const.valOFF)
                                            sensorStatus = "No";
                                        else if (sensorStatus == VLF.CLS.Def.Const.valON)
                                            sensorStatus = "Yes";
                                        else
                                            sensorStatus = " ";
                                    }
                                    catch (Exception e)
                                    {
                                        // log exception
                                    }
                                    AddReportRow("15 sec Door Sensor",
                                        Convert.ToDateTime(ittr["OriginDateTime"]),
                                        vehicleDescription, "15s Dr. Door Open", sensorStatus, boxId, vehicleId);
                                }
                                else if (sensorId == 37 && fifteenSecDoorSns)
                                {
                                    string sensorStatus = "";
                                    try
                                    {
                                        sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
                                        if (sensorStatus == VLF.CLS.Def.Const.valOFF)
                                            sensorStatus = "No";
                                        else if (sensorStatus == VLF.CLS.Def.Const.valON)
                                            sensorStatus = "Yes";
                                        else
                                            sensorStatus = " ";
                                    }
                                    catch (Exception e)
                                    {
                                        // log exception
                                    }
                                    AddReportRow("15 sec Door Sensor",
                                        Convert.ToDateTime(ittr["OriginDateTime"]),
                                        vehicleDescription, "15s Pas. Door Open", sensorStatus, boxId, vehicleId);
                                }
                                #endregion

                                #region Add 50% of Leash
                                else if (sensorId == 26 && leash50)
                                {
                                    string sensorStatus = "";
                                    try
                                    {
                                        sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
                                        if (sensorStatus == VLF.CLS.Def.Const.valOFF)
                                            sensorStatus = "No";
                                        else if (sensorStatus == VLF.CLS.Def.Const.valON)
                                            sensorStatus = "Yes";
                                        else
                                            sensorStatus = " ";
                                    }
                                    catch (Exception e)
                                    {
                                        // log exception
                                    }
                                    AddReportRow("50% of leash event",
                                        Convert.ToDateTime(ittr["OriginDateTime"]),
                                        vehicleDescription, "50% of Leash", sensorStatus, boxId, vehicleId);
                                }
                                #endregion

                                #region Main and Backup Battery
                                else if (mainAndBackupBatterySns && (sensorId == 4 || sensorId == 5))
                                {
                                    string sensorStatus = "";
                                    string sensorName = "Main Battery";
                                    if (sensorId == 5)
                                        sensorName = "Backup Battery";
                                    try
                                    {
                                        sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
                                        if (sensorStatus == VLF.CLS.Def.Const.valOFF)
                                            sensorStatus = "Ok";
                                        else if (sensorStatus == VLF.CLS.Def.Const.valON)
                                            sensorStatus = "Low";
                                        else
                                            sensorStatus = " ";
                                    }
                                    catch (Exception e)
                                    {
                                        // log exception
                                    }
                                    AddReportRow("Main or Backup battery event",
                                        Convert.ToDateTime(ittr["OriginDateTime"]),
                                        vehicleDescription, sensorName, sensorStatus, boxId, vehicleId);
                                }
                                #endregion

                                #region Tamper
                                else if (tamperSns && sensorId == 13)
                                {
                                    string sensorStatus = "";
                                    try
                                    {
                                        sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
                                        if (sensorStatus == VLF.CLS.Def.Const.valOFF)
                                            sensorStatus = "Closed";
                                        else if (sensorStatus == VLF.CLS.Def.Const.valON)
                                            sensorStatus = "Open";
                                        else
                                            sensorStatus = " ";
                                    }
                                    catch (Exception e)
                                    {
                                        // log exception
                                    }
                                    AddReportRow("Tamper event",
                                        Convert.ToDateTime(ittr["OriginDateTime"]),
                                        vehicleDescription, "Tamper", sensorStatus, boxId, vehicleId);
                                }
                                #endregion

                                #region Any Panic Event
                                else if (anyPanicSns && (sensorId == 15 || sensorId == 16))
                                {
                                    string sensorStatus = "";
                                    string sensorName = "Unknown";
                                    if (sensorId == 16)
                                        sensorName = "Hopper Panic Switch";
                                    else if (sensorId == 15)
                                        sensorName = "Driver Panic Switch";
                                    try
                                    {
                                        sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
                                    }
                                    catch (Exception e)
                                    {
                                        // log exception
                                    }
                                    AddReportRow("Any Panic Event",
                                        Convert.ToDateTime(ittr["OriginDateTime"]),
                                        vehicleDescription, sensorName, sensorStatus, boxId, vehicleId);
                                }
                                #endregion

                                #region 3 Keypad attempts event
                                else if (threeKeypadAttemptsSns && sensorId == 25)
                                {
                                    string sensorStatus = "";
                                    try
                                    {
                                        sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
                                    }
                                    catch (Exception e)
                                    {
                                        // log exception
                                    }
                                    AddReportRow("3 Keypad attempts event",
                                        Convert.ToDateTime(ittr["OriginDateTime"]),
                                        vehicleDescription, "3 Keypad attempts", "", boxId, vehicleId);
                                }
                                #endregion

                                #region Alternative GPS Antenna event
                                else if (altGPSAntennaSns && sensorId == 38)
                                {
                                    string sensorStatus = "";
                                    try
                                    {
                                        sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
                                        if (sensorStatus == VLF.CLS.Def.Const.valOFF)
                                            sensorStatus = "Ok";
                                        else if (sensorStatus == VLF.CLS.Def.Const.valON)
                                            sensorStatus = "Cut";
                                        else
                                            sensorStatus = " ";
                                    }
                                    catch (Exception e)
                                    {
                                        // log exception
                                    }
                                    AddReportRow("Alternative GPS Antenna event",
                                        Convert.ToDateTime(ittr["OriginDateTime"]),
                                        vehicleDescription, "Alternative GPS Antenna", sensorStatus, boxId, vehicleId);
                                }
                                #endregion

                                #region Leash Broken
                                else if (sensorId == 27 && leashBrokenSns)
                                {
                                    string sensorStatus = "";
                                    try
                                    {
                                        sensorStatus = Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus, customProp);
                                        if (sensorStatus == VLF.CLS.Def.Const.valOFF)
                                            sensorStatus = "No";
                                        else if (sensorStatus == VLF.CLS.Def.Const.valON)
                                            sensorStatus = "Yes";
                                        else
                                            sensorStatus = " ";
                                    }
                                    catch (Exception e)
                                    {
                                        // log exception
                                    }
                                    AddReportRow("Leash Broken event",
                                        Convert.ToDateTime(ittr["OriginDateTime"]),
                                        vehicleDescription, "Leash Broken", sensorStatus, boxId, vehicleId);
                                }
                                #endregion

                                #region Add Locker1
                                else if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker1 && sensorId == 19/*Locker1*/)
                                {
                                    if (lastLocker1DateTime == VLF.CLS.Def.Const.unassignedDateTime) // first time
                                        lastLocker1DateTime = fromTD;

                                    TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(ittr["OriginDateTime"]).Ticks - lastLocker1DateTime.Ticks);
                                    if (currDuration.TotalHours > noDoorSnsHrs)
                                    {
                                        AddReportRow("No Locker 1 event within " + noDoorSnsHrs + " hours (From - To)",
                                            lastLocker1DateTime, vehicleDescription,
                                            Convert.ToDateTime(ittr["OriginDateTime"]).ToString(), " ", boxId, vehicleId);
                                    }
                                    lastLocker1DateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
                                }
                                #endregion

                                #region Add Locker2
                                else if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker2 && sensorId == 20/*Locker2*/)
                                {
                                    if (lastLocker2DateTime == VLF.CLS.Def.Const.unassignedDateTime) // first time
                                        lastLocker2DateTime = fromTD;

                                    TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(ittr["OriginDateTime"]).Ticks - lastLocker2DateTime.Ticks);
                                    if (currDuration.TotalHours > noDoorSnsHrs)
                                    {
                                        AddReportRow("No Locker 2 event within " + noDoorSnsHrs + " hours (From - To)",
                                            lastLocker2DateTime, vehicleDescription,
                                            Convert.ToDateTime(ittr["OriginDateTime"]).ToString(), " ", boxId, vehicleId);
                                    }
                                    lastLocker2DateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
                                }
                                #endregion

                                #region Add Locker3
                                else if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker3 && sensorId == 21/*Locker3*/)
                                {
                                    if (lastLocker3DateTime == VLF.CLS.Def.Const.unassignedDateTime) // first time
                                        lastLocker3DateTime = fromTD;

                                    TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(ittr["OriginDateTime"]).Ticks - lastLocker3DateTime.Ticks);
                                    if (currDuration.TotalHours > noDoorSnsHrs)
                                    {
                                        AddReportRow("No Locker 3 event within " + noDoorSnsHrs + " hours (From - To)",
                                            lastLocker3DateTime, vehicleDescription,
                                            Convert.ToDateTime(ittr["OriginDateTime"]).ToString(), " ", boxId, vehicleId);
                                    }
                                    lastLocker3DateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
                                }
                                #endregion

                                #region Add Locker4
                                else if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker4 && sensorId == 22/*Locker4*/)
                                {
                                    if (lastLocker4DateTime == VLF.CLS.Def.Const.unassignedDateTime) // first time
                                        lastLocker4DateTime = fromTD;

                                    TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(ittr["OriginDateTime"]).Ticks - lastLocker4DateTime.Ticks);
                                    if (currDuration.TotalHours > noDoorSnsHrs)
                                    {
                                        AddReportRow("No Locker 4 event within " + noDoorSnsHrs + " hours (From - To)",
                                            lastLocker4DateTime, vehicleDescription,
                                            Convert.ToDateTime(ittr["OriginDateTime"]).ToString(), " ", boxId, vehicleId);
                                    }
                                    lastLocker4DateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
                                }
                                #endregion


                                #region Add Locker5
                                else if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker5 && sensorId == 23/*Locker5*/)
                                {
                                    if (lastLocker5DateTime == VLF.CLS.Def.Const.unassignedDateTime) // first time
                                        lastLocker5DateTime = fromTD;

                                    TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(ittr["OriginDateTime"]).Ticks - lastLocker5DateTime.Ticks);
                                    if (currDuration.TotalHours > noDoorSnsHrs)
                                    {
                                        AddReportRow("No Locker 4 event within " + noDoorSnsHrs + " hours (From - To)",
                                            lastLocker5DateTime, vehicleDescription,
                                            Convert.ToDateTime(ittr["OriginDateTime"]).ToString(), " ", boxId, vehicleId);
                                    }
                                    lastLocker5DateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
                                }
                                #endregion


                                #region Add Locker6
                                else if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker6 && sensorId == 24/*Locker6*/)
                                {
                                    if (lastLocker6DateTime == VLF.CLS.Def.Const.unassignedDateTime) // first time
                                        lastLocker6DateTime = fromTD;

                                    TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(ittr["OriginDateTime"]).Ticks - lastLocker6DateTime.Ticks);
                                    if (currDuration.TotalHours > noDoorSnsHrs)
                                    {
                                        AddReportRow("No Locker 6 event within " + noDoorSnsHrs + " hours (From - To)",
                                            lastLocker6DateTime, vehicleDescription,
                                            Convert.ToDateTime(ittr["OriginDateTime"]).ToString(), " ", boxId, vehicleId);
                                    }
                                    lastLocker6DateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
                                }
                                #endregion


                                #region Add Locker7
                                else if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker7 && sensorId == 33/*Locker7*/)
                                {
                                    if (lastLocker7DateTime == VLF.CLS.Def.Const.unassignedDateTime) // first time
                                        lastLocker7DateTime = fromTD;

                                    TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(ittr["OriginDateTime"]).Ticks - lastLocker7DateTime.Ticks);
                                    if (currDuration.TotalHours > noDoorSnsHrs)
                                    {
                                        AddReportRow("No Locker 7 event within " + noDoorSnsHrs + " hours (From - To)",
                                            lastLocker7DateTime, vehicleDescription,
                                            Convert.ToDateTime(ittr["OriginDateTime"]).ToString(), " ", boxId, vehicleId);
                                    }
                                    lastLocker7DateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
                                }
                                #endregion

                                #region Add Locker8
                                else if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker8 && sensorId == 34/*Locker8*/)
                                {
                                    if (lastLocker8DateTime == VLF.CLS.Def.Const.unassignedDateTime) // first time
                                        lastLocker8DateTime = fromTD;

                                    TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(ittr["OriginDateTime"]).Ticks - lastLocker8DateTime.Ticks);
                                    if (currDuration.TotalHours > noDoorSnsHrs)
                                    {
                                        AddReportRow("No Locker 8 event within " + noDoorSnsHrs + " hours (From - To)",
                                            lastLocker8DateTime, vehicleDescription,
                                            Convert.ToDateTime(ittr["OriginDateTime"]).ToString(), " ", boxId, vehicleId);
                                    }
                                    lastLocker8DateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
                                }
                                #endregion

                                #region Add Locker9
                                else if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker9 && sensorId == 35/*Locker9*/)
                                {
                                    if (lastLocker9DateTime == VLF.CLS.Def.Const.unassignedDateTime) // first time
                                        lastLocker9DateTime = fromTD;

                                    TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(ittr["OriginDateTime"]).Ticks - lastLocker9DateTime.Ticks);
                                    if (currDuration.TotalHours > noDoorSnsHrs)
                                    {
                                        AddReportRow("No Locker 9 event within " + noDoorSnsHrs + " hours (From - To)",
                                            lastLocker9DateTime, vehicleDescription,
                                            Convert.ToDateTime(ittr["OriginDateTime"]).ToString(), " ", boxId, vehicleId);
                                    }
                                    lastLocker9DateTime = Convert.ToDateTime(ittr["OriginDateTime"]);
                                }
                                #endregion
                            }
                            #endregion

                            #region Any Panic Event
                            else if (anyPanicSns && msgType == Enums.MessageType.KeyFobPanic)
                            {
                                AddReportRow("Any Panic Event",
                                    Convert.ToDateTime(ittr["OriginDateTime"]),
                                    vehicleDescription, "KeyFob Panic", "", boxId, vehicleId);
                            }
                            #endregion

                            #region Controller Status
                            else if (controllerStatus && msgType == Enums.MessageType.ControllerStatus)
                            {
                                AddReportRow("Controller Status",
                                    Convert.ToDateTime(ittr["OriginDateTime"]),
                                    vehicleDescription, "Controller Status", "", boxId, vehicleId);
                            }
                            #endregion

                        }
                        catch
                        {
                            continue;
                        }
                    }
                    // process last one
                    #region Add Special Occasional Stop to report
                    if (sosLimit != VLF.CLS.Def.Const.unassignedShortValue && sosMode >= sosLimit)
                    {
                        if (dsSosEvents != null && dsSosEvents.Tables.Count > 0)
                        {
                            foreach (DataRow sos in dsSosEvents.Tables[0].Rows)
                            {
                                AddReportRow("Excessive SOS mode more than " + sosLimit + " times",
                                    Convert.ToDateTime(sos[0]),
                                    vehicleDescription,
                                    "Special Occasional Stop", sos[1].ToString(),
                                    boxId,
                                    vehicleId);
                            }
                        }
                    }
                    #endregion

                }
                #region Add Driver Door to report
                if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && driverDoor/* && sensorId == 9 Driver Door*/)
                {
                    if (lastDriverDoorDateTime == VLF.CLS.Def.Const.unassignedDateTime)
                        lastDriverDoorDateTime = fromTD;
                    TimeSpan currDur = new TimeSpan(toTD.Ticks - lastDriverDoorDateTime.Ticks);
                    if (currDur.TotalHours > noDoorSnsHrs)
                    {
                        AddReportRow("No driver door opening/closing event within " + noDoorSnsHrs + " hours (From - To)",
                            lastDriverDoorDateTime, vehicleDescription,
                            toTD.ToString(), " ", boxId, vehicleId);
                    }
                }
                #endregion

                #region Add Passenger Door to report
                if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && passengerDoor/* && sensorId == 10*/)
                {
                    if (lastPassengerDoorDateTime == VLF.CLS.Def.Const.unassignedDateTime)
                        lastPassengerDoorDateTime = fromTD;
                    TimeSpan currDur = new TimeSpan(toTD.Ticks - lastPassengerDoorDateTime.Ticks);
                    if (currDur.TotalHours > noDoorSnsHrs)
                    {
                        AddReportRow("No passenger door opening/closing event within " + noDoorSnsHrs + " hours (From - To)",
                            lastPassengerDoorDateTime, vehicleDescription,
                            toTD.ToString(), " ", boxId, vehicleId);
                    }
                }
                #endregion

                #region Add Side Hopper Door to report
                if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && sideHopperDoor/* && sensorId == 11 Side Hopper Door*/)
                {
                    if (lastSideHopperDoorDoorDateTime == VLF.CLS.Def.Const.unassignedDateTime)
                        lastSideHopperDoorDoorDateTime = fromTD;
                    TimeSpan currDur = new TimeSpan(toTD.Ticks - lastSideHopperDoorDoorDateTime.Ticks);
                    if (currDur.TotalHours > noDoorSnsHrs)
                    {
                        AddReportRow("No side hopper door opening/closing event within " + noDoorSnsHrs + " hours (From - To)",
                            lastSideHopperDoorDoorDateTime, vehicleDescription,
                            toTD.ToString(), " ", boxId, vehicleId);
                    }
                }
                #endregion

                #region Add Rear Hopper Door to report
                if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && rearHopperDoor/* && sensorId == 12 Rear Hopper Door*/)
                {
                    if (lastRearHopperDoorDateTime == VLF.CLS.Def.Const.unassignedDateTime)
                        lastRearHopperDoorDateTime = fromTD;
                    TimeSpan currDur = new TimeSpan(toTD.Ticks - lastRearHopperDoorDateTime.Ticks);
                    if (currDur.TotalHours > noDoorSnsHrs)
                    {
                        AddReportRow("No rear hopper door opening/closing event within " + noDoorSnsHrs + " hours (From - To)",
                            lastRearHopperDoorDateTime, vehicleDescription,
                            toTD.ToString(), " ", boxId, vehicleId);
                    }
                }
                #endregion


                #region Locker1
                if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker1/* && sensorId == 19 Locker1*/)
                {
                    if (lastLocker1DateTime == VLF.CLS.Def.Const.unassignedDateTime)
                        lastLocker1DateTime = fromTD;
                    TimeSpan currDur = new TimeSpan(toTD.Ticks - lastLocker1DateTime.Ticks);
                    if (currDur.TotalHours > noDoorSnsHrs)
                    {
                        AddReportRow("No Locker1 event within " + noDoorSnsHrs + " hours (From - To)",
                            lastLocker1DateTime, vehicleDescription,
                            toTD.ToString(), " ", boxId, vehicleId);
                    }
                }
                #endregion

                #region Locker1
                if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker2/* && sensorId == 20 Locker2*/)
                {
                    if (lastLocker2DateTime == VLF.CLS.Def.Const.unassignedDateTime)
                        lastLocker2DateTime = fromTD;
                    TimeSpan currDur = new TimeSpan(toTD.Ticks - lastLocker2DateTime.Ticks);
                    if (currDur.TotalHours > noDoorSnsHrs)
                    {
                        AddReportRow("No Locker2 event within " + noDoorSnsHrs + " hours (From - To)",
                            lastLocker2DateTime, vehicleDescription,
                            toTD.ToString(), " ", boxId, vehicleId);
                    }
                }
                #endregion


                #region Locker3
                if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker3/* && sensorId == 21 Locker3*/)
                {
                    if (lastLocker3DateTime == VLF.CLS.Def.Const.unassignedDateTime)
                        lastLocker3DateTime = fromTD;
                    TimeSpan currDur = new TimeSpan(toTD.Ticks - lastLocker3DateTime.Ticks);
                    if (currDur.TotalHours > noDoorSnsHrs)
                    {
                        AddReportRow("No Locker3 event within " + noDoorSnsHrs + " hours (From - To)",
                            lastLocker3DateTime, vehicleDescription,
                            toTD.ToString(), " ", boxId, vehicleId);
                    }
                }
                #endregion

                #region Locker4
                if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker4/* && sensorId == 22 Locker4*/)
                {
                    if (lastLocker4DateTime == VLF.CLS.Def.Const.unassignedDateTime)
                        lastLocker4DateTime = fromTD;
                    TimeSpan currDur = new TimeSpan(toTD.Ticks - lastLocker4DateTime.Ticks);
                    if (currDur.TotalHours > noDoorSnsHrs)
                    {
                        AddReportRow("No Locker4 event within " + noDoorSnsHrs + " hours (From - To)",
                            lastLocker4DateTime, vehicleDescription,
                            toTD.ToString(), " ", boxId, vehicleId);
                    }
                }
                #endregion

                #region Locker5
                if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker5/* && sensorId == 23 Locker5*/)
                {
                    if (lastLocker5DateTime == VLF.CLS.Def.Const.unassignedDateTime)
                        lastLocker5DateTime = fromTD;
                    TimeSpan currDur = new TimeSpan(toTD.Ticks - lastLocker5DateTime.Ticks);
                    if (currDur.TotalHours > noDoorSnsHrs)
                    {
                        AddReportRow("No Locker5 event within " + noDoorSnsHrs + " hours (From - To)",
                            lastLocker5DateTime, vehicleDescription,
                            toTD.ToString(), " ", boxId, vehicleId);
                    }
                }
                #endregion

                #region Locker6
                if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker6/* && sensorId == 24 Locker6*/)
                {
                    if (lastLocker6DateTime == VLF.CLS.Def.Const.unassignedDateTime)
                        lastLocker6DateTime = fromTD;
                    TimeSpan currDur = new TimeSpan(toTD.Ticks - lastLocker6DateTime.Ticks);
                    if (currDur.TotalHours > noDoorSnsHrs)
                    {
                        AddReportRow("No Locker6 event within " + noDoorSnsHrs + " hours (From - To)",
                            lastLocker5DateTime, vehicleDescription,
                            toTD.ToString(), " ", boxId, vehicleId);
                    }
                }
                #endregion


                #region Locker7
                if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker7/* && sensorId == 33 Locker7*/)
                {
                    if (lastLocker7DateTime == VLF.CLS.Def.Const.unassignedDateTime)
                        lastLocker7DateTime = fromTD;
                    TimeSpan currDur = new TimeSpan(toTD.Ticks - lastLocker7DateTime.Ticks);
                    if (currDur.TotalHours > noDoorSnsHrs)
                    {
                        AddReportRow("No Locker7 event within " + noDoorSnsHrs + " hours (From - To)",
                            lastLocker7DateTime, vehicleDescription,
                            toTD.ToString(), " ", boxId, vehicleId);
                    }
                }
                #endregion


                #region Locker8
                if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker8/* && sensorId == 34 Locker8*/)
                {
                    if (lastLocker8DateTime == VLF.CLS.Def.Const.unassignedDateTime)
                        lastLocker8DateTime = fromTD;
                    TimeSpan currDur = new TimeSpan(toTD.Ticks - lastLocker8DateTime.Ticks);
                    if (currDur.TotalHours > noDoorSnsHrs)
                    {
                        AddReportRow("No Locker8 event within " + noDoorSnsHrs + " hours (From - To)",
                            lastLocker8DateTime, vehicleDescription,
                            toTD.ToString(), " ", boxId, vehicleId);
                    }
                }
                #endregion

                #region Locker9
                if (noDoorSnsHrs != VLF.CLS.Def.Const.unassignedIntValue && Locker9/* && sensorId == 35 Locker9*/)
                {
                    if (lastLocker9DateTime == VLF.CLS.Def.Const.unassignedDateTime)
                        lastLocker9DateTime = fromTD;
                    TimeSpan currDur = new TimeSpan(toTD.Ticks - lastLocker9DateTime.Ticks);
                    if (currDur.TotalHours > noDoorSnsHrs)
                    {
                        AddReportRow("No Locker9 event within " + noDoorSnsHrs + " hours (From - To)",
                            lastLocker9DateTime, vehicleDescription,
                            toTD.ToString(), " ", boxId, vehicleId);
                    }
                }
                #endregion
            }
            catch (ERR.DASDbConnectionClosed exCnn)
            {
                throw new ERR.DASDbConnectionClosed(exCnn.Message);
            }
            finally
            {
            }
            #endregion
        }
        /// <summary>
        /// Returns report 
        /// </summary>
        public DataTable ReportDetailes
        {
            get
            {
                return tblReportData;
            }
        }

        /// <summary>
        /// Add new row to result report
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dateTime"></param>
        /// <param name="vehicleDescription"></param>
        /// <param name="remarks1"></param>
        /// <param name="remarks2"></param>
        /// <param name="boxId"></param>
        /// <param name="vehicleId"></param>
        private void AddReportRow(string type,
                                DateTime dateTime,
                                string vehicleDescription,
                                string remarks1,
                                string remarks2,
                                int boxId,
                                Int64 vehicleId)
        {
            object[] objRow = new object[tblReportData.Columns.Count];
            objRow[0] = type;
            objRow[1] = dateTime;
            objRow[2] = vehicleDescription;
            objRow[3] = remarks1;
            objRow[4] = remarks2;
            objRow[5] = boxId;
            objRow[6] = vehicleId;
            tblReportData.Rows.Add(objRow);
        }
        /// <summary>
        /// Retrieves last TAR mode On OriginDateTime
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <returns>Last TAR mode On OriginDateTime, otherwise returns DateTime.Min </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        private DateTime GetLastTarOnMessageDateTimeFromHistory(int userId, int boxId)
        {
            DateTime retResult = VLF.CLS.Def.Const.unassignedDateTime;
            try
            {
                // 1. Prepares SQL statement
                string sql = "DECLARE @Timezone int DECLARE @DayLightSaving int SELECT @Timezone=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone + " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving + " IF @DayLightSaving IS NULL SET @DayLightSaving=0 SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT top 1 DATEADD(hour,@Timezone,OriginDateTime) AS OriginDateTime FROM vlfMsgInHst with (nolock) WHERE vlfMsgInHst.BoxId=" + boxId +
                    " AND (vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Sensor + " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)VLF.CLS.Def.Enums.MessageType.Alarm + ")" +
                    " AND CustomProp like 'SENSOR_NUM=40;SENSOR_STATUS=ON;%' Order by OriginDateTime Desc";
                // 2. Executes SQL statement
                object obj = sqlExec.SQLExecuteScalar(sql);
                if (obj != System.DBNull.Value)
                    retResult = Convert.ToDateTime(obj);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve last TAR mode ON info from history by box id " + boxId, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve last TAR mode ON info from history by box id " + boxId + " " + objException.Message);
            }
            return retResult;
        }
        /// <summary>
        /// Retrieves crrent TAR ON Information
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <returns>DataSet [VehicleId],[BoxId],[LastCommunicatedDateTime],[TarOnDT]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        private DataSet GetCurrentTarOnInfo(int userId, int boxId)
        {
            DataSet dsResult = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "DECLARE @Timezone int DECLARE @DayLightSaving int SELECT @Timezone=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                " IF @Timezone IS NULL SET @Timezone=0 SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                " IF @DayLightSaving IS NULL SET @DayLightSaving=0 SET @Timezone= @Timezone + @DayLightSaving" +
                " declare @b bigint set @b =549755813888 SELECT vlfVehicleAssignment.VehicleId,vlfBox.BoxId, CASE WHEN vlfBox.LastCommunicatedDateTime IS NOT NULL then DATEADD(hour,@Timezone,vlfBox.LastCommunicatedDateTime) END AS LastCommunicatedDateTime, GETDATE() as TarOnDT FROM vlfBox with (nolock) INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId where vlfBox.BoxId=" + boxId + " and (vlfBox.LastSensorMask & @b) > 0";
                // 2. Executes SQL statement
                dsResult = sqlExec.SQLExecuteDataset(sql);
                if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
                    dsResult.Tables[0].Rows[0]["TarOnDT"] = GetLastTarOnMessageDateTimeFromHistory(userId, boxId);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve current TAR mode ON info from history by box id " + boxId, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve current TAR mode ON info from history by box id " + boxId + " " + objException.Message);
            }
            return dsResult;
        }
    }
}
