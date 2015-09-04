using System;
using VLF.ERR;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.DAS.DB;
using VLF.CLS.Def;
using VLF.CLS;

namespace VLF.DAS.Logic
{
    /// <summary>
    /// Provides interface to message queues and history functionality in database
    /// </summary>
    public class MessageQueue : Das
    {
        private MsgOut msgOut = null;
        private MsgIn msgIn = null;
        private Landmark landmark = null;
        private DB.TxtMsgs textMsg = null;
        private DB.User user = null;
        private DB.MapEngine mapEngine = null;


        #region General Interfaces
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public MessageQueue(string connectionString)
            : base(connectionString)
        {
            msgOut = new MsgOut(sqlExec);
            landmark = new Landmark(sqlExec);
            msgIn = new MsgIn(sqlExec);
            textMsg = new DB.TxtMsgs(sqlExec);
            user = new DB.User(sqlExec);
            mapEngine = new DB.MapEngine(sqlExec);
        }
        /// <summary>
        /// Destructor
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
        }
        #endregion

        #region MsgIn Interfaces
        /// <summary>
        /// Add new Msg in CMFIn format.
        /// </summary>
        /// <exception cref="DASAppDataAlreadyExistsException">Throws DASAppDataAlreadyExistsException after number of attemps to add new information.</exception>
        /// <exception cref="DASException">Throws DASException in other all error cases.</exception>
        /// <param name="cMFIn"></param>
        /// <comment>because MessageQueue could be called my multiple threads I added a critical section
        /// </comment>
        public void AddMsgIn(CMFIn cMFIn)
        {
            lock (this)
            {
                msgIn.AddMsg(cMFIn);
            }
        }
        /// <summary>
        /// Delete all rows.
        /// </summary>
        /// <returns>Rows Affected</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public int DeleteAllMsgInRecords()
        {
            return msgIn.DeleteAllRecords();
        }
        /// <summary>
        /// Removes all messages forMsgIn by specified boxID
        /// </summary>
        /// <exception cref="DASException">Throws DASException in other all error cases.</exception>
        /// <param name="boxID"></param>
        public void FlushBoxMsgIn(int boxID)
        {
            msgIn.FlushBoxMsg(boxID);
        }
        /// <summary>
        /// Returns next message (older one) in the CMF format or null in case of no more results. 	
        /// Add new record to MsgInHst table without Blob data (only blob size).
        /// If record alredy exists, try to add new one with DateTime + VLF.CLS.Def.Const.nextDateTimeMillisecInterval
        /// Deletes old data by DateTimeReceived field.
        /// </summary>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Throws DASAppViolatedIntegrityConstraintsException after max retries (VLF.CLS.Def.Const.violatedIntegrityMaxRetries).</exception>
        /// <exception cref="DASException">Throws DASException in other all error cases.</exception>
        /// <returns>next CMF msg</returns>		
        public bool DrawNextXCmfMsgIn(ref int cnt, ref CMFInEx[] arrCMFIn)
        {
            return msgIn.DrawNextXCmfMsg(ref cnt, ref arrCMFIn);
        }


        public bool[] DrawNextXCmfMsgIn_SLSProcessIn(ref int cnt, ref CMFInEx[] arrCMFIn)
        {
            return msgIn.DrawNextXCmfMsg_SLSProcessIn(ref cnt, ref arrCMFIn);
        }

        public bool DrawNextXCmfMsgIn2(int slsId, ref int cnt, ref CMFIn[] arrCMFIn)
        {
            return msgIn.DrawNextXCmfMsg2(slsId, ref cnt, ref arrCMFIn);
        }

        public bool DrawNextXCmfMsg_SLSTmp(ref int cnt, ref CMFInEx[] arrCMFIn)
        {
            return msgIn.DrawNextXCmfMsg_SLSTmp(ref cnt, ref arrCMFIn);
        }


        public bool[] DrawNextXCmfMsgInQueued(ref int cnt, ref CMFInEx[] arrCMFIn)
        {
            return msgIn.DrawNextXCmfMsgQueued(ref cnt, ref arrCMFIn);
        }

        public DataSet vfMsgInHstQueued_GetNotProcessed(CLS.Def.Enums.HistoryTables tableId)
        {
            return msgIn.vfMsgInHstQueued_GetNotProcessed(tableId);
        }


        public void UpdateProcessedInHistoryQueue(CMFInEx  cMFIn, CLS.Def.Enums.HistoryTables tableId, bool successfulInsertion)
        {
            msgIn.UpdateProcessedInHistoryQueue(cMFIn, tableId, successfulInsertion);
        }


        public DataSet GetMsgInTypesWithPriority()
        {
            return msgIn.GetMsgInTypesWithPriority();
        }


        /// <summary>
        /// Get VehiclesInHistory By Address
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public DataSet GetVehiclesInHistoryByAddress(int fleetId,
                                                   DateTime from,
                                                   DateTime to, string address)
        {
            return msgIn.GetVehiclesInHistoryByAddress(fleetId, from, to, address);
        }

        /// <summary>
        /// Get VehiclesInHistory By Location
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public DataSet GetVehiclesInHistoryByLocation(int fleetId,
                                                    DateTime from,
                                                    DateTime to, double latitude, double longitude)
        {
            return msgIn.GetVehiclesInHistoryByLocation(fleetId, from, to, latitude, longitude);
        }

        #endregion MsgIn Interfaces

        #region MsgOut Interfaces
        /// <summary>
        /// Add new Msg in CMFOut format.
        /// </summary>
        /// <param name="cMFOut"></param>
        /// <param name="priority"></param>
        /// <param name="dclId"></param>
        /// <param name="aslId"></param>
        /// <param name="userId"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Throws DASAppDataAlreadyExistsException after number of attemps to add new information.</exception>
        /// <exception cref="DASException">Throws DASException in other all error cases.</exception>
        public void AddMsgOut(CMFOut cMFOut, SByte priority, short dclId, short aslId, int userId)
        {
            msgOut.AddMsg(cMFOut, priority, dclId, aslId, userId);
        }

        /// <summary>
        /// Delete all rows.
        /// </summary>
        /// <returns>Rows Affected</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public int DeleteAllMsgOutRecords()
        {
            return msgOut.DeleteAllRecords();
        }
        /// <summary>
        /// Returns next message (older one) in CMFOut format or null if result hasn't been found. 	
        /// Add new record to MsgOutHst table.
        /// If record alredy exists, try to add new one with DateTime + VLF.CLS.Def.Const.nextDateTimeMillisecInterval
        /// Deletes old data by DateTime field.
        /// </summary>
        /// <param name="dclId"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Throws DASAppViolatedIntegrityConstraintsException after max retries (VLF.CLS.Def.Const.violatedIntegrityMaxRetries).</exception>
        /// <exception cref="DASException">Throws DASException in other all error cases.</exception>
        /// <returns>next CMF msg</returns>
        public CMFOut DrawNextCmfMsgOut(short dclId)
        {
            return msgOut.DrawNextCmfMsg(dclId);
        }

        public int DrawNextCmfMsgOut(short dclId, ref int cnt, out CMFOut[] arr)
        {
            return msgOut.DrawNextCmfMsg(dclId, ref cnt, out arr);
        }
        /// <summary>
        /// Returns next message (older one) in CMFOut format or null if result hasn't been found. 	
        /// Add new record to MsgOutHst table.
        /// If record alredy exists, try to add new one with DateTime + VLF.CLS.Def.Const.nextDateTimeMillisecInterval
        /// Deletes old data by DateTime field.
        /// </summary>
        /// <param name="dclId"></param>
        /// <param name="currDateTime"></param>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Throws DASAppViolatedIntegrityConstraintsException after max retries (VLF.CLS.Def.Const.violatedIntegrityMaxRetries).</exception>
        /// <exception cref="DASException">Throws DASException in other all error cases.</exception>
        /// <returns>next CMF msg</returns>
        public CMFOut DrawNextCmfMsgOut(short dclId, DateTime currDateTime)
        {
            return msgOut.DrawNextCmfMsg(dclId, currDateTime);
        }
        /// <summary>
        /// Update acknowledged fields 
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="dateTime"></param>
        /// <param name="ackVal"></param>
        public void SetAck(int boxId, DateTime dateTime, string ackVal)
        {
            msgOut.SetAck(boxId, dateTime, ackVal);
        }
        /// <summary>
        /// Update acknowledged fields 
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="commandType"></param>
        /// <param name="ackVal"></param>
        public void SetAck(int boxId, VLF.CLS.Def.Enums.CommandType commandType, string ackVal)
        {
            msgOut.SetAck(boxId, commandType, ackVal);
        }
        /// <summary>
        /// Check message in MsgOut table
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="commandType"></param>
        /// <returns>true if exists, otherwise returns false</returns>
        public bool CheckMessageInMsgOut(int boxId, VLF.CLS.Def.Enums.CommandType commandType)
        {
            return msgOut.CheckMessageInMsgOut(boxId, commandType);
        }
        #endregion MsgOut Interfaces

        #region MsgInOutHistory Interfaces
        /// <summary>
        /// Retrieves last message for current box from the history
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <param name="msgType"></param>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],
        /// [SequenceNum],[BoxArmed]
        /// </returns>
        public DataSet GetLastSingleMessageFromHistory(int userId, int boxId, VLF.CLS.Def.Enums.MessageType msgType)
        {
            DataSet dsResult = msgIn.GetLastSingleMessageFromHistory(userId, boxId, msgType);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "Last" + msgType.ToString();
                }
                dsResult.DataSetName = "MsgInHistory";
            }
            return dsResult;
        }
        /// <summary>
        /// Retrieves last DoS message box id by client ip
        /// </summary>
        /// <param name="clientIp"></param>
        /// <returns> BoxId </returns>
        /// <remarks> If information does not exist, return 0</remarks>
        public int GetLastDoSStartedFromHistory(string clientIp)
        {
            return msgIn.GetLastDoSStartedFromHistory(clientIp);
        }
        /// <summary>
        /// Retrieves messages from history by fleet id
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="includeCoordinate"></param>
        /// <param name="includeSensor"></param>
        /// <param name="includePositionUpdate"></param>
        /// <param name="includeInvalidGps"></param>
        /// <param name="requestOverflowed"></param>
        /// <remarks>
        /// 1. Retrieves only specific messages from the history:
        /// Coordinate,Sensor,Speed,Fence,PositionUpdate,IPUpdate,KeyFobPanic
        /// 2. Parse CustomProp field for Sensor and Fence messages, and add info into MsgDetails field
        /// 3. Incase of IPUpdate message add new IP into MsgDetails field
        /// </remarks>
        /// <returns>
        /// DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],
        /// [SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetFleetMessagesFromHistory(int fleetId,
                                                    int userId,
                                                    DateTime from,
                                                    DateTime to,
                                                    bool includeCoordinate,
                                                    bool includeSensor,
                                                    bool includePositionUpdate,
                                                    bool includeInvalidGps,
                                       string lang,
                                                    ref bool requestOverflowed)
        {
            int totalSqlRecords = 0;
            DataSet dsResult = new DataSet();
            DataSet dsCurrVehicleInfo = null;
            // 2. Retrieve fleet vehicles info (license plates)
            //[LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]
            DB.FleetVehicles fleetVehicles = new DB.FleetVehicles(sqlExec);
            DataSet dsVehicles = fleetVehicles.GetVehiclesInfoByFleetId(fleetId);
            if (dsVehicles != null && dsVehicles.Tables.Count > 0)
            {
                Int64 vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
                foreach (DataRow ittrRow in dsVehicles.Tables[0].Rows)
                {
                    vehicleId = Convert.ToInt64(ittrRow["VehicleId"]);
                    // 3. Retrieves history for every vehicle
                    dsCurrVehicleInfo = GetMessagesFromHistoryByVehicleId(userId,
                                                                        vehicleId,
                                                                        from,
                                                                        to,
                                                                        includeCoordinate,
                                                                        includeSensor,
                                                                        includePositionUpdate,
                                                                        includeInvalidGps, -1, lang, "", "",
                                                                        ref requestOverflowed,
                                                                        ref totalSqlRecords);
                    // 4. Merge vehicle trips info into fleet trips info
                    if (requestOverflowed)
                    {
                        dsResult.Clear();
                        break;
                    }
                    if (dsCurrVehicleInfo != null && dsCurrVehicleInfo.Tables.Count > 0)
                    {
                        // Add VehicleId column to result dataset
                        dsCurrVehicleInfo.Tables[0].Columns.Add("VehicleId", typeof(Int64));
                        // Add vehicle id to every row
                        foreach (DataRow ittr in dsCurrVehicleInfo.Tables[0].Rows)
                            ittr["VehicleId"] = vehicleId;
                    }
                    if (dsResult != null && dsCurrVehicleInfo != null)
                        dsResult.Merge(dsCurrVehicleInfo);
                }
            }
            dsResult.DataSetName = "DsFleetHistory";
            return dsResult;
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves messages from history by vehicle Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="includeCoordinate"></param>
        /// <param name="includeSensor"></param>
        /// <param name="includePositionUpdate"></param>
        /// <param name="includeInvalidGps"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],
        /// [StreetAddress],[SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],
        /// [Acknowledged],[Scheduled]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetMessagesFromHistoryByVehicleId_NewTZ(int userId, Int64 vehicleId,
                                                        DateTime from, DateTime to,
                                                        bool includeCoordinate,
                                                        bool includeSensor,
                                                        bool includePositionUpdate,
                                                        bool includeInvalidGps,
                                                        Int16 DclId,
                                                        string lang,
                                                        string msgList,
                                                        string sqlTopRecords,
                                                        ref bool requestOverflowed,
                                                        ref int totalSqlRecords)
        {
            DB.UserGroupAssignment userGroupAssignment = new DB.UserGroupAssignment(sqlExec);
            bool isSuperUser = userGroupAssignment.IsUserAssignedToUserGroup(userId, 1);

            DataSet dsResult = new DataSet();
            // 2. Retrieves all historical box/vehicleId assignments
            DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
            //[AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]
            //DataSet dsAssignHst = assignHst.GetAllVehiclesAssignmentsBy("VehicleId", vehicleId, from, to, " ORDER BY AssignedDateTime DESC");

            DataSet dsAssignHst = assignHst.GetAllVehiclesAssigmentByVehicleId(vehicleId, from, to);

            if (dsAssignHst != null && dsAssignHst.Tables.Count > 0)
            {
                // 3.1.	For each assignment in the range of from/to datetime mearge results
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
                        currAssignTo = to;

                    if (currAssignFrom.Ticks < from.Ticks)
                        currAssignFrom = new DateTime(from.Ticks);

                    if (currAssignTo.Ticks > Convert.ToDateTime(to).Ticks)
                        currAssignTo = new DateTime(Convert.ToDateTime(to).Ticks);
                    /*
                    #if MSQ_SP1 // testing new method calling stored proc. 2008-03-28 Max
                                   currResult = msgIn.GetMessagesFromHistoryByBoxId_SP(userId,
                                            isSuperUser, Convert.ToInt32(ittr["BoxId"]),
                                            currAssignFrom, currAssignTo,
                                      includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId,lang,
                                            ref requestOverflowed, ref totalSqlRecords);
                    #else
                    */
                    //currResult = msgIn.GetMessagesFromHistoryByBoxId(userId,
                    //         isSuperUser, Convert.ToInt32(ittr["BoxId"]),
                    //         currAssignFrom, currAssignTo,
                    //   includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords,
                    //         ref requestOverflowed, ref totalSqlRecords);

                    currResult = msgIn.GetMessagesFromHistoryByBoxId_NewTZ(userId,
                            isSuperUser, Convert.ToInt32(ittr["BoxId"]),
                            currAssignFrom, currAssignTo,
                      includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords, ittr["VehicleId"].ToString(), ittr["LicensePlate"].ToString(), ittr["Description"].ToString().Replace("'", "`"),
                            ref requestOverflowed, ref totalSqlRecords);

                    //#endif

                    if (dsResult != null && currResult != null)
                        dsResult.Merge(currResult);
                }

                ////////                // 3.2	Add current assignment 
                ////////                // [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
                ////////                DB.VehicleAssignment assign = new DB.VehicleAssignment(sqlExec);
                ////////                DataSet dsAssign = assign.GetVehicleAssignmentBy("VehicleId", vehicleId);
                ////////                if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
                ////////                {
                ////////                    currAssignFrom = Convert.ToDateTime(dsAssign.Tables[0].Rows[0]["AssignedDateTime"]);
                ////////                    if (currAssignFrom.Ticks < from.Ticks)
                ////////                        currAssignFrom = new DateTime(from.Ticks);

                ////////#if MSQ_SP1 // testing new method calling stored proc. 2008-03-28 Max
                ////////               currResult = msgIn.GetMessagesFromHistoryByBoxId_SP(userId, isSuperUser,
                ////////                        Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]),
                ////////                        currAssignFrom, to,
                ////////                        includeCoordinate,includeSensor,includePositionUpdate,includeInvalidGps,DclId,lang, 
                ////////                        ref requestOverflowed,ref totalSqlRecords);
                ////////#else
                ////////                    currResult = msgIn.GetMessagesFromHistoryByBoxId(userId, isSuperUser,
                ////////                             Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]),
                ////////                             currAssignFrom, to,
                ////////                             includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords,
                ////////                             ref requestOverflowed, ref totalSqlRecords);
                ////////#endif

                ////////                    if (dsResult != null && currResult != null)
                ////////                        dsResult.Merge(currResult);
                ////////                }
            }

            DataSet dsFilteredResult = null;
            #region Sort result by OriginDateTime DESC
            if (dsResult != null)
            {
                dsFilteredResult = dsResult.Clone();
                dsFilteredResult.DataSetName = "MsgInHistory";

                if (dsResult.Tables.Count > 0)
                {
                    dsFilteredResult.Tables[0].TableName = "VehicleStatusHistory";
                    DataRow[] filteredRows = dsResult.Tables[0].Select("", "OriginDateTime DESC");
                    foreach (DataRow ittr in filteredRows)
                        dsFilteredResult.Tables[0].ImportRow(ittr);
                }
            }
            #endregion
            return dsFilteredResult;
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves messages from history by vehicle Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="includeCoordinate"></param>
        /// <param name="includeSensor"></param>
        /// <param name="includePositionUpdate"></param>
        /// <param name="includeInvalidGps"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],
        /// [StreetAddress],[SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],
        /// [Acknowledged],[Scheduled]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetMessagesFromHistoryByVehicleId(int userId, Int64 vehicleId,
                                                        DateTime from, DateTime to,
                                                        bool includeCoordinate,
                                                        bool includeSensor,
                                                        bool includePositionUpdate,
                                                        bool includeInvalidGps,
                                                        Int16 DclId,
                                                        string lang,
                                                        string msgList,
                                                        string sqlTopRecords,
                                                        ref bool requestOverflowed,
                                                        ref int totalSqlRecords)
        {
            DB.UserGroupAssignment userGroupAssignment = new DB.UserGroupAssignment(sqlExec);
            bool isSuperUser = userGroupAssignment.IsUserAssignedToUserGroup(userId, 1);

            DataSet dsResult = new DataSet();
            // 2. Retrieves all historical box/vehicleId assignments
            DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
            //[AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]
            //DataSet dsAssignHst = assignHst.GetAllVehiclesAssignmentsBy("VehicleId", vehicleId, from, to, " ORDER BY AssignedDateTime DESC");

            DataSet dsAssignHst = assignHst.GetAllVehiclesAssigmentByVehicleId(vehicleId, from, to);

            if (dsAssignHst != null && dsAssignHst.Tables.Count > 0)
            {
                // 3.1.	For each assignment in the range of from/to datetime mearge results
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
                        currAssignTo = to;

                    if (currAssignFrom.Ticks < from.Ticks)
                        currAssignFrom = new DateTime(from.Ticks);

                    if (currAssignTo.Ticks > Convert.ToDateTime(to).Ticks)
                        currAssignTo = new DateTime(Convert.ToDateTime(to).Ticks);
                    /*
                    #if MSQ_SP1 // testing new method calling stored proc. 2008-03-28 Max
                                   currResult = msgIn.GetMessagesFromHistoryByBoxId_SP(userId,
                                            isSuperUser, Convert.ToInt32(ittr["BoxId"]),
                                            currAssignFrom, currAssignTo,
                                      includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId,lang,
                                            ref requestOverflowed, ref totalSqlRecords);
                    #else
                    */
                    //currResult = msgIn.GetMessagesFromHistoryByBoxId(userId,
                    //         isSuperUser, Convert.ToInt32(ittr["BoxId"]),
                    //         currAssignFrom, currAssignTo,
                    //   includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords,
                    //         ref requestOverflowed, ref totalSqlRecords);

                    currResult = msgIn.GetMessagesFromHistoryByBoxId(userId,
                            isSuperUser, Convert.ToInt32(ittr["BoxId"]),
                            currAssignFrom, currAssignTo,
                      includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords, ittr["VehicleId"].ToString(), ittr["LicensePlate"].ToString(), ittr["Description"].ToString().Replace("'", "`"),
                            ref requestOverflowed, ref totalSqlRecords);

                    //#endif

                    if (dsResult != null && currResult != null)
                        dsResult.Merge(currResult);
                }

                ////////                // 3.2	Add current assignment 
                ////////                // [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
                ////////                DB.VehicleAssignment assign = new DB.VehicleAssignment(sqlExec);
                ////////                DataSet dsAssign = assign.GetVehicleAssignmentBy("VehicleId", vehicleId);
                ////////                if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
                ////////                {
                ////////                    currAssignFrom = Convert.ToDateTime(dsAssign.Tables[0].Rows[0]["AssignedDateTime"]);
                ////////                    if (currAssignFrom.Ticks < from.Ticks)
                ////////                        currAssignFrom = new DateTime(from.Ticks);

                ////////#if MSQ_SP1 // testing new method calling stored proc. 2008-03-28 Max
                ////////               currResult = msgIn.GetMessagesFromHistoryByBoxId_SP(userId, isSuperUser,
                ////////                        Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]),
                ////////                        currAssignFrom, to,
                ////////                        includeCoordinate,includeSensor,includePositionUpdate,includeInvalidGps,DclId,lang, 
                ////////                        ref requestOverflowed,ref totalSqlRecords);
                ////////#else
                ////////                    currResult = msgIn.GetMessagesFromHistoryByBoxId(userId, isSuperUser,
                ////////                             Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]),
                ////////                             currAssignFrom, to,
                ////////                             includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords,
                ////////                             ref requestOverflowed, ref totalSqlRecords);
                ////////#endif

                ////////                    if (dsResult != null && currResult != null)
                ////////                        dsResult.Merge(currResult);
                ////////                }
            }

            DataSet dsFilteredResult = null;
            #region Sort result by OriginDateTime DESC
            if (dsResult != null)
            {
                dsFilteredResult = dsResult.Clone();
                dsFilteredResult.DataSetName = "MsgInHistory";

                if (dsResult.Tables.Count > 0)
                {
                    dsFilteredResult.Tables[0].TableName = "VehicleStatusHistory";
                    DataRow[] filteredRows = dsResult.Tables[0].Select("", "OriginDateTime DESC");
                    foreach (DataRow ittr in filteredRows)
                        dsFilteredResult.Tables[0].ImportRow(ittr);
                }
            }
            #endregion
            return dsFilteredResult;
        }







        /// <summary>
        /// Retrieves messages from history by vehicle Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="includeCoordinate"></param>
        /// <param name="includeSensor"></param>
        /// <param name="includePositionUpdate"></param>
        /// <param name="includeInvalidGps"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],
        /// [StreetAddress],[SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],
        /// [Acknowledged],[Scheduled]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetMessagesFromHistoryByVehicleId_Report(int userId, Int64 vehicleId,
                                                        DateTime from, DateTime to,
                                                        bool includeCoordinate,
                                                        bool includeSensor,
                                                        bool includePositionUpdate,
                                                        bool includeInvalidGps,
                                                        Int16 DclId,
                                                        string lang,
                                                        string msgList,
                                                        string sqlTopRecords,
                                                        ref bool requestOverflowed,
                                                        ref int totalSqlRecords)
        {
            DB.UserGroupAssignment userGroupAssignment = new DB.UserGroupAssignment(sqlExec);
            bool isSuperUser = userGroupAssignment.IsUserAssignedToUserGroup(userId, 1);

            DataSet dsResult = new DataSet();
            // 2. Retrieves all historical box/vehicleId assignments
            DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
            //[AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]
            DataSet dsAssignHst = assignHst.GetAllVehiclesAssignmentsBy("VehicleId", vehicleId, from, to, " ORDER BY AssignedDateTime DESC");
            if (dsAssignHst != null && dsAssignHst.Tables.Count > 0)
            {
                // 3.1.	For each assignment in the range of from/to datetime mearge results
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
                        currAssignTo = to;

                    if (currAssignFrom.Ticks < from.Ticks)
                        currAssignFrom = new DateTime(from.Ticks);

                    if (currAssignTo.Ticks > Convert.ToDateTime(to).Ticks)
                        currAssignTo = new DateTime(Convert.ToDateTime(to).Ticks);
                    /*
                    #if MSQ_SP1 // testing new method calling stored proc. 2008-03-28 Max
                                   currResult = msgIn.GetMessagesFromHistoryByBoxId_SP(userId,
                                            isSuperUser, Convert.ToInt32(ittr["BoxId"]),
                                            currAssignFrom, currAssignTo,
                                      includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId,lang,
                                            ref requestOverflowed, ref totalSqlRecords);
                    #else
                    */
                    currResult = msgIn.GetMessagesFromHistoryByBoxId_Report (userId,
                             isSuperUser, Convert.ToInt32(ittr["BoxId"]),
                             currAssignFrom, currAssignTo,
                       includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords,
                             ref requestOverflowed, ref totalSqlRecords);
                    //#endif

                    if (dsResult != null && currResult != null)
                        dsResult.Merge(currResult);
                }

                // 3.2	Add current assignment 
                // [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
                DB.VehicleAssignment assign = new DB.VehicleAssignment(sqlExec);
                DataSet dsAssign = assign.GetVehicleAssignmentBy("VehicleId", vehicleId);
                if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
                {
                    currAssignFrom = Convert.ToDateTime(dsAssign.Tables[0].Rows[0]["AssignedDateTime"]);
                    if (currAssignFrom.Ticks < from.Ticks)
                        currAssignFrom = new DateTime(from.Ticks);

#if MSQ_SP1 // testing new method calling stored proc. 2008-03-28 Max
               currResult = msgIn.GetMessagesFromHistoryByBoxId_SP(userId, isSuperUser,
						Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]),
						currAssignFrom, to,
						includeCoordinate,includeSensor,includePositionUpdate,includeInvalidGps,DclId,lang, 
						ref requestOverflowed,ref totalSqlRecords);
#else
                    currResult = msgIn.GetMessagesFromHistoryByBoxId_Report (userId, isSuperUser,
                             Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]),
                             currAssignFrom, to,
                             includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords,
                             ref requestOverflowed, ref totalSqlRecords);
#endif

                    if (dsResult != null && currResult != null)
                        dsResult.Merge(currResult);
                }
            }

            DataSet dsFilteredResult = null;
            #region Sort result by OriginDateTime DESC
            if (dsResult != null)
            {
                dsFilteredResult = dsResult.Clone();
                dsFilteredResult.DataSetName = "MsgInHistory";

                if (dsResult.Tables.Count > 0)
                {
                    dsFilteredResult.Tables[0].TableName = "VehicleStatusHistory";
                    DataRow[] filteredRows = dsResult.Tables[0].Select("", "OriginDateTime DESC");
                    foreach (DataRow ittr in filteredRows)
                        dsFilteredResult.Tables[0].ImportRow(ittr);
                }
            }
            #endregion
            return dsFilteredResult;
        }



        public DataSet GetMessagesFromHistoryByVehicleId_Tmp(int userId, Int64 vehicleId,
                                                       DateTime from, DateTime to,
                                                       bool includeCoordinate,
                                                       bool includeSensor,
                                                       bool includePositionUpdate,
                                                       bool includeInvalidGps,
                                                       Int16 DclId,
                                                       string lang,
                                                       string msgList,
                                                       string sqlTopRecords,
                                                       ref bool requestOverflowed,
                                                       ref int totalSqlRecords)
        {
            DB.UserGroupAssignment userGroupAssignment = new DB.UserGroupAssignment(sqlExec);
            bool isSuperUser = userGroupAssignment.IsUserAssignedToUserGroup(userId, 1);

            DataSet dsResult = new DataSet();
            // 2. Retrieves all historical box/vehicleId assignments
            DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
            //[AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]
            DataSet dsAssignHst = assignHst.GetAllVehiclesAssignmentsBy("VehicleId", vehicleId, from, to, " ORDER BY AssignedDateTime DESC");
            if (dsAssignHst != null && dsAssignHst.Tables.Count > 0)
            {
                // 3.1.	For each assignment in the range of from/to datetime mearge results
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
                        currAssignTo = to;

                    if (currAssignFrom.Ticks < from.Ticks)
                        currAssignFrom = new DateTime(from.Ticks);

                    if (currAssignTo.Ticks > Convert.ToDateTime(to).Ticks)
                        currAssignTo = new DateTime(Convert.ToDateTime(to).Ticks);
                    /*
                    #if MSQ_SP1 // testing new method calling stored proc. 2008-03-28 Max
                                   currResult = msgIn.GetMessagesFromHistoryByBoxId_SP(userId,
                                            isSuperUser, Convert.ToInt32(ittr["BoxId"]),
                                            currAssignFrom, currAssignTo,
                                      includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId,lang,
                                            ref requestOverflowed, ref totalSqlRecords);
                    #else
                    */
                    currResult = msgIn.GetMessagesFromHistoryByBoxId_Tmp(userId,
                             isSuperUser, Convert.ToInt32(ittr["BoxId"]),
                             currAssignFrom, currAssignTo,
                       includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords,
                             ref requestOverflowed, ref totalSqlRecords);
                    //#endif

                    if (dsResult != null && currResult != null)
                        dsResult.Merge(currResult);
                }

                // 3.2	Add current assignment 
                // [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
                DB.VehicleAssignment assign = new DB.VehicleAssignment(sqlExec);
                DataSet dsAssign = assign.GetVehicleAssignmentBy("VehicleId", vehicleId);
                if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
                {
                    currAssignFrom = Convert.ToDateTime(dsAssign.Tables[0].Rows[0]["AssignedDateTime"]);
                    if (currAssignFrom.Ticks < from.Ticks)
                        currAssignFrom = new DateTime(from.Ticks);

#if MSQ_SP1 // testing new method calling stored proc. 2008-03-28 Max
               currResult = msgIn.GetMessagesFromHistoryByBoxId_SP(userId, isSuperUser,
						Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]),
						currAssignFrom, to,
						includeCoordinate,includeSensor,includePositionUpdate,includeInvalidGps,DclId,lang, 
						ref requestOverflowed,ref totalSqlRecords);
#else
                    currResult = msgIn.GetMessagesFromHistoryByBoxId_Tmp(userId, isSuperUser,
                             Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]),
                             currAssignFrom, to,
                             includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords,
                             ref requestOverflowed, ref totalSqlRecords);
#endif

                    if (dsResult != null && currResult != null)
                        dsResult.Merge(currResult);
                }
            }

            DataSet dsFilteredResult = null;
            #region Sort result by OriginDateTime DESC
            if (dsResult != null)
            {
                dsFilteredResult = dsResult.Clone();
                dsFilteredResult.DataSetName = "MsgInHistory";

                if (dsResult.Tables.Count > 0)
                {
                    dsFilteredResult.Tables[0].TableName = "VehicleStatusHistory";
                    DataRow[] filteredRows = dsResult.Tables[0].Select("", "OriginDateTime DESC");
                    foreach (DataRow ittr in filteredRows)
                        dsFilteredResult.Tables[0].ImportRow(ittr);
                }
            }
            #endregion
            return dsFilteredResult;
        }


        // Changes for TimeZone Feature start
        public DataSet GetMessagesFromHistoryByVehicleId_ExtendedSearch_NewTZ(int userId, Int64 vehicleId,
                                                       DateTime from, DateTime to,
                                                       bool includeCoordinate,
                                                       bool includeSensor,
                                                       bool includePositionUpdate,
                                                       bool includeInvalidGps,
                                                       Int16 DclId,
                                                       string lang,
                                                       string msgList,
                                                       string sqlTopRecords,
                                                       string address,
                                                       ref bool requestOverflowed,
                                                       ref int totalSqlRecords)
        {
            DB.UserGroupAssignment userGroupAssignment = new DB.UserGroupAssignment(sqlExec);
            bool isSuperUser = userGroupAssignment.IsUserAssignedToUserGroup(userId, 1);

            DataSet dsResult = new DataSet();
            // 2. Retrieves all historical box/vehicleId assignments
            DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
            //[AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]
            DataSet dsAssignHst = assignHst.GetAllVehiclesAssignmentsBy("VehicleId", vehicleId, from, to, " ORDER BY AssignedDateTime DESC");
            if (dsAssignHst != null && dsAssignHst.Tables.Count > 0)
            {
                // 3.1.	For each assignment in the range of from/to datetime mearge results
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
                        currAssignTo = to;

                    if (currAssignFrom.Ticks < from.Ticks)
                        currAssignFrom = new DateTime(from.Ticks);

                    if (currAssignTo.Ticks > Convert.ToDateTime(to).Ticks)
                        currAssignTo = new DateTime(Convert.ToDateTime(to).Ticks);

                    currResult = msgIn.GetMessagesFromHistoryByBoxId_ExtendedSearch_NewTZ(userId,
                                            isSuperUser, Convert.ToInt32(ittr["BoxId"]),
                                            currAssignFrom, currAssignTo,
                                      includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, address,
                                            ref requestOverflowed, ref totalSqlRecords);

                    if (dsResult != null && currResult != null)
                        dsResult.Merge(currResult);
                }

                // 3.2	Add current assignment 
                // [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
                DB.VehicleAssignment assign = new DB.VehicleAssignment(sqlExec);
                DataSet dsAssign = assign.GetVehicleAssignmentBy("VehicleId", vehicleId);
                if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
                {
                    currAssignFrom = Convert.ToDateTime(dsAssign.Tables[0].Rows[0]["AssignedDateTime"]);
                    if (currAssignFrom.Ticks < from.Ticks)
                        currAssignFrom = new DateTime(from.Ticks);


                    currResult = msgIn.GetMessagesFromHistoryByBoxId_ExtendedSearch_NewTZ(userId, isSuperUser,
                             Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]),
                             currAssignFrom, to,
                             includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, address,
                             ref requestOverflowed, ref totalSqlRecords);


                    if (dsResult != null && currResult != null)
                        dsResult.Merge(currResult);
                }
            }

            //DataSet dsFilteredResult = null;
            //#region Sort result by OriginDateTime DESC
            //if (dsResult != null)
            //{
            //    dsFilteredResult = dsResult.Clone();
            //    dsFilteredResult.DataSetName = "MsgInHistory";

            //    if (dsResult.Tables.Count > 0)
            //    {
            //        dsFilteredResult.Tables[0].TableName = "VehicleStatusHistory";
            //        DataRow[] filteredRows = dsResult.Tables[0].Select("", "OriginDateTime DESC");
            //        foreach (DataRow ittr in filteredRows)
            //            dsFilteredResult.Tables[0].ImportRow(ittr);
            //    }
            //}
            //#endregion
            //return dsFilteredResult;

            dsResult.DataSetName = "MsgInHistory";
            dsResult.Tables[0].TableName = "VehicleStatusHistory";
            return dsResult;
        }

        // Changes for TimeZone Feature end


        public DataSet GetMessagesFromHistoryByVehicleId_ExtendedSearch(int userId, Int64 vehicleId,
                                                        DateTime from, DateTime to,
                                                        bool includeCoordinate,
                                                        bool includeSensor,
                                                        bool includePositionUpdate,
                                                        bool includeInvalidGps,
                                                        Int16 DclId,
                                                        string lang,
                                                        string msgList,
                                                        string sqlTopRecords,
                                                        string address,
                                                        ref bool requestOverflowed,
                                                        ref int totalSqlRecords)
        {
            DB.UserGroupAssignment userGroupAssignment = new DB.UserGroupAssignment(sqlExec);
            bool isSuperUser = userGroupAssignment.IsUserAssignedToUserGroup(userId, 1);

            DataSet dsResult = new DataSet();
            // 2. Retrieves all historical box/vehicleId assignments
            DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
            //[AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]
            DataSet dsAssignHst = assignHst.GetAllVehiclesAssignmentsBy("VehicleId", vehicleId, from, to, " ORDER BY AssignedDateTime DESC");
            if (dsAssignHst != null && dsAssignHst.Tables.Count > 0)
            {
                // 3.1.	For each assignment in the range of from/to datetime mearge results
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
                        currAssignTo = to;

                    if (currAssignFrom.Ticks < from.Ticks)
                        currAssignFrom = new DateTime(from.Ticks);

                    if (currAssignTo.Ticks > Convert.ToDateTime(to).Ticks)
                        currAssignTo = new DateTime(Convert.ToDateTime(to).Ticks);

                    currResult = msgIn.GetMessagesFromHistoryByBoxId_ExtendedSearch(userId,
                                            isSuperUser, Convert.ToInt32(ittr["BoxId"]),
                                            currAssignFrom, currAssignTo,
                                      includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, address,
                                            ref requestOverflowed, ref totalSqlRecords);

                    if (dsResult != null && currResult != null)
                        dsResult.Merge(currResult);
                }

                // 3.2	Add current assignment 
                // [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
                DB.VehicleAssignment assign = new DB.VehicleAssignment(sqlExec);
                DataSet dsAssign = assign.GetVehicleAssignmentBy("VehicleId", vehicleId);
                if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
                {
                    currAssignFrom = Convert.ToDateTime(dsAssign.Tables[0].Rows[0]["AssignedDateTime"]);
                    if (currAssignFrom.Ticks < from.Ticks)
                        currAssignFrom = new DateTime(from.Ticks);


                    currResult = msgIn.GetMessagesFromHistoryByBoxId_ExtendedSearch(userId, isSuperUser,
                             Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]),
                             currAssignFrom, to,
                             includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, address,
                             ref requestOverflowed, ref totalSqlRecords);


                    if (dsResult != null && currResult != null)
                        dsResult.Merge(currResult);
                }
            }

            //DataSet dsFilteredResult = null;
            //#region Sort result by OriginDateTime DESC
            //if (dsResult != null)
            //{
            //    dsFilteredResult = dsResult.Clone();
            //    dsFilteredResult.DataSetName = "MsgInHistory";

            //    if (dsResult.Tables.Count > 0)
            //    {
            //        dsFilteredResult.Tables[0].TableName = "VehicleStatusHistory";
            //        DataRow[] filteredRows = dsResult.Tables[0].Select("", "OriginDateTime DESC");
            //        foreach (DataRow ittr in filteredRows)
            //            dsFilteredResult.Tables[0].ImportRow(ittr);
            //    }
            //}
            //#endregion
            //return dsFilteredResult;

            dsResult.DataSetName = "MsgInHistory";
            dsResult.Tables[0].TableName = "VehicleStatusHistory";
            return dsResult;
        }

        // Changes for TimeZone Feature start
        public DataSet GetMessagesFromHistoryByFleetId_ExtendedSearch_NewTZ(int userId, Int32 fleetId,
                                                       DateTime from, DateTime to,
                                                       bool includeCoordinate,
                                                       bool includeSensor,
                                                       bool includePositionUpdate,
                                                       bool includeInvalidGps,
                                                       Int16 DclId,
                                                       string lang,
                                                       string msgList,
                                                       string sqlTopRecords,
                                                       string address,
                                                       ref bool requestOverflowed,
                                                       ref int totalSqlRecords)
        {

            DB.UserGroupAssignment userGroupAssignment = new DB.UserGroupAssignment(sqlExec);
            bool isSuperUser = userGroupAssignment.IsUserAssignedToUserGroup(userId, 1);

            DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
            DataSet dsResult = msgIn.GetMessagesFromHistoryByFleetId_ExtendedSearch(userId, isSuperUser,
                             fleetId,
                             from, to,
                             includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, address,
                             ref requestOverflowed, ref totalSqlRecords);


            dsResult.DataSetName = "MsgInHistory";
            dsResult.Tables[0].TableName = "VehicleStatusHistory";
            return dsResult;
        }
        // Changes for TimeZone Feature end


        public DataSet GetMessagesFromHistoryByFleetId_ExtendedSearch(int userId, Int32 fleetId,
                                                        DateTime from, DateTime to,
                                                        bool includeCoordinate,
                                                        bool includeSensor,
                                                        bool includePositionUpdate,
                                                        bool includeInvalidGps,
                                                        Int16 DclId,
                                                        string lang,
                                                        string msgList,
                                                        string sqlTopRecords,
                                                        string address,
                                                        ref bool requestOverflowed,
                                                        ref int totalSqlRecords)
        {

            DB.UserGroupAssignment userGroupAssignment = new DB.UserGroupAssignment(sqlExec);
            bool isSuperUser = userGroupAssignment.IsUserAssignedToUserGroup(userId, 1);

            DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
            DataSet dsResult = msgIn.GetMessagesFromHistoryByFleetId_ExtendedSearch(userId, isSuperUser,
                             fleetId,
                             from, to,
                             includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, address,
                             ref requestOverflowed, ref totalSqlRecords);


            dsResult.DataSetName = "MsgInHistory";
            dsResult.Tables[0].TableName = "VehicleStatusHistory";
            return dsResult;
        }

        // Changes for TimeZone Feature start
        public DataSet HistoryGetMessagesbyFleetId_NewTZ(int userId,
                                           int fleetId,
                                           DateTime from,
                                           DateTime to,
                                           bool includeCoordinate,
                                           bool includeSensor,
                                           bool includePositionUpdate,
                                           bool includeInvalidGps,
                                           Int16 DclId,
                                           string lang,
                                           ref bool requestOverflowed,
                                           ref int totalSqlRecords)
        {

            DB.UserGroupAssignment userGroupAssignment = new DB.UserGroupAssignment(sqlExec);
            bool isSuperUser = userGroupAssignment.IsUserAssignedToUserGroup(userId, 1);

            DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
            DataSet dsResult = msgIn.HistoryGetMessagesbyFleetId_NewTZ(userId, isSuperUser,
                             fleetId,
                             from, to,
                             includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang,
                             ref requestOverflowed, ref totalSqlRecords);


            dsResult.DataSetName = "MsgInHistory";
            dsResult.Tables[0].TableName = "VehicleStatusHistory";
            return dsResult;
        }
        // Changes for TimeZone Feature end



        public DataSet HistoryGetMessagesbyFleetId(int userId,
                                           int fleetId,
                                           DateTime from,
                                           DateTime to,
                                           bool includeCoordinate,
                                           bool includeSensor,
                                           bool includePositionUpdate,
                                           bool includeInvalidGps,
                                           Int16 DclId,
                                           string lang,
                                           ref bool requestOverflowed,
                                           ref int totalSqlRecords)
        {

            DB.UserGroupAssignment userGroupAssignment = new DB.UserGroupAssignment(sqlExec);
            bool isSuperUser = userGroupAssignment.IsUserAssignedToUserGroup(userId, 1);

            DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
            DataSet dsResult = msgIn.HistoryGetMessagesbyFleetId(userId, isSuperUser,
                             fleetId,
                             from, to,
                             includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang,
                             ref requestOverflowed, ref totalSqlRecords);


            dsResult.DataSetName = "MsgInHistory";
            dsResult.Tables[0].TableName = "VehicleStatusHistory";
            return dsResult;
        }

        /// <summary>
        /// Retrieves off hours information from history by fleet id
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="dayFromHour"></param>
        /// <param name="dayFromMin"></param>
        /// <param name="dayToHour"></param>
        /// <param name="dayToMin"></param>
        /// <param name="weekendFromHour"></param>
        /// <param name="weekendFromMin"></param>
        /// <param name="weekendToHour"></param>
        /// <param name="weekendToMin"></param>
        /// <param name="includeCoordinate"></param>
        /// <param name="includeSensor"></param>
        /// <param name="includePositionUpdate"></param>
        /// <param name="includeInvalidGps"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <remarks>
        /// 1. Retrieves only specific messages from the history:
        /// Coordinate,Sensor,Speed,Fence,PositionUpdate,IPUpdate,KeyFobPanic
        /// 2. Parse CustomProp field for Sensor and Fence messages, and add info into MsgDetails field
        /// 3. Incase of IPUpdate message add new IP into MsgDetails field
        /// </remarks>
        /// <returns>
        /// DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],
        /// [SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetFleetOffHoursInfo(int userId,
                                            int fleetId,
                                            DateTime from,
                                            DateTime to,
                                            short dayFromHour,
                                            short dayFromMin,
                                            short dayToHour,
                                            short dayToMin,
                                            short weekendFromHour,
                                            short weekendFromMin,
                                            short weekendToHour,
                                            short weekendToMin,
                                            bool includeCoordinate,
                                            bool includeSensor,
                                            bool includePositionUpdate,
                                            bool includeInvalidGps,
                                            ref bool requestOverflowed,
                                            ref int totalSqlRecords)
        {
            totalSqlRecords = 0;
            DataSet dsResult = new DataSet();
            DataSet dsCurrVehicleInfo = null;
            // 2. Retrieve fleet vehicles info (license plates)
            //[LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]
            DB.FleetVehicles fleetVehicles = new DB.FleetVehicles(sqlExec);
            DataSet dsVehicles = fleetVehicles.GetVehiclesInfoByFleetId(fleetId);
            if (dsVehicles != null && dsVehicles.Tables.Count > 0)
            {
                Int64 vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
                int currSqlRecords = 0;
                foreach (DataRow ittrRow in dsVehicles.Tables[0].Rows)
                {
                    currSqlRecords = 0;
                    vehicleId = Convert.ToInt64(ittrRow["VehicleId"]);
                    // 3. Retrieves history for every vehicle
                    dsCurrVehicleInfo = GetVehicleOffHoursInfo(userId, vehicleId, from, to, dayFromHour, dayFromMin, dayToHour, dayToMin, weekendFromHour, weekendFromMin, weekendToHour, weekendToMin, includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, ref requestOverflowed, ref currSqlRecords);
                    totalSqlRecords += currSqlRecords;
                    // 4. Merge vehicle trips info into fleet trips info
                    if (requestOverflowed)
                    {
                        dsResult.Clear();
                        break;
                    }
                    if (dsCurrVehicleInfo != null && dsCurrVehicleInfo.Tables.Count > 0)
                    {
                        // Add VehicleId column to result dataset
                        dsCurrVehicleInfo.Tables[0].Columns.Add("VehicleId", typeof(Int64));
                        // Add vehicle id to every row
                        foreach (DataRow ittr in dsCurrVehicleInfo.Tables[0].Rows)
                            ittr["VehicleId"] = vehicleId;
                    }
                    if (dsResult != null && dsCurrVehicleInfo != null)
                        dsResult.Merge(dsCurrVehicleInfo);
                }
            }
            dsResult.DataSetName = "DsFleetHistory";
            return dsResult;
        }
        /// <summary>
        /// Retrieves off hours information from history by vehicle Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="dayFromHour"></param>
        /// <param name="dayFromMin"></param>
        /// <param name="dayToHour"></param>
        /// <param name="dayToMin"></param>
        /// <param name="weekendFromHour"></param>
        /// <param name="weekendFromMin"></param>
        /// <param name="weekendToHour"></param>
        /// <param name="weekendToMin"></param>
        /// <param name="includeCoordinate"></param>
        /// <param name="includeSensor"></param>
        /// <param name="includePositionUpdate"></param>
        /// <param name="includeInvalidGps"></param>
        /// <param name="requestOverflowed"></param>
        /// <param name="totalSqlRecords"></param>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],
        /// [StreetAddress],[SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],
        /// [Acknowledged],[Scheduled]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetVehicleOffHoursInfo(int userId, Int64 vehicleId,
                                            DateTime from, DateTime to,
                                            short dayFromHour,
                                            short dayFromMin,
                                            short dayToHour,
                                            short dayToMin,
                                            short weekendFromHour,
                                            short weekendFromMin,
                                            short weekendToHour,
                                            short weekendToMin,
                                            bool includeCoordinate,
                                            bool includeSensor,
                                            bool includePositionUpdate,
                                            bool includeInvalidGps,
                                            ref bool requestOverflowed,
                                            ref int totalSqlRecords)
        {
            DB.UserGroupAssignment userGroupAssignment = new DB.UserGroupAssignment(sqlExec);
            bool isSuperUser = userGroupAssignment.IsUserAssignedToUserGroup(userId, 1);

            DataSet dsResult = new DataSet();
            // 2. Retrieves all historical box/vehicleId assignments
            DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
            //[AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]
            DataSet dsAssignHst = assignHst.GetAllVehiclesAssignmentsBy("VehicleId", vehicleId, from, to, " ORDER BY AssignedDateTime DESC");
            if (dsAssignHst != null && dsAssignHst.Tables.Count > 0)
            {
                // 3.1.	For each assignment in the range of from/to datetime mearge results
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
                        currAssignTo = to;

                    if (currAssignFrom.Ticks < from.Ticks)
                        currAssignFrom = new DateTime(from.Ticks);

                    if (currAssignTo.Ticks > Convert.ToDateTime(to).Ticks)
                        currAssignTo = new DateTime(Convert.ToDateTime(to).Ticks);

                    currResult = msgIn.GetBoxOffHoursInfo(userId, isSuperUser, Convert.ToInt32(ittr["BoxId"]), currAssignFrom, currAssignTo, dayFromHour, dayFromMin, dayToHour, dayToMin, weekendFromHour, weekendFromMin, weekendToHour, weekendToMin, includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, ref requestOverflowed, ref totalSqlRecords);
                    if (dsResult != null && currResult != null)
                        dsResult.Merge(currResult);
                }

                // 3.2	Add current assignment 
                // [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
                DB.VehicleAssignment assign = new DB.VehicleAssignment(sqlExec);
                DataSet dsAssign = assign.GetVehicleAssignmentBy("VehicleId", vehicleId);
                if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
                {
                    currAssignFrom = Convert.ToDateTime(dsAssign.Tables[0].Rows[0]["AssignedDateTime"]);
                    if (currAssignFrom.Ticks < from.Ticks)
                        currAssignFrom = new DateTime(from.Ticks);
                    currResult = msgIn.GetBoxOffHoursInfo(userId, isSuperUser, Convert.ToInt32(dsAssign.Tables[0].Rows[0]["BoxId"]), currAssignFrom, to, dayFromHour, dayFromMin, dayToHour, dayToMin, weekendFromHour, weekendFromMin, weekendToHour, weekendToMin, includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, ref requestOverflowed, ref totalSqlRecords);
                    if (dsResult != null && currResult != null)
                        dsResult.Merge(currResult);
                }
            }

            DataSet dsFilteredResult = null;
            #region Sort result by OriginDateTime DESC
            if (dsResult != null)
            {
                dsFilteredResult = dsResult.Clone();
                dsFilteredResult.DataSetName = "MsgInHistory";

                if (dsResult.Tables.Count > 0)
                {
                    dsFilteredResult.Tables[0].TableName = "VehicleStatusHistory";
                    DataRow[] filteredRows = dsResult.Tables[0].Select("", "OriginDateTime DESC");
                    foreach (DataRow ittr in filteredRows)
                        dsFilteredResult.Tables[0].ImportRow(ittr);
                }
            }
            #endregion
            return dsFilteredResult;
        }
        /// <summary>
        /// Retrieves message from history by box id and DateTimeReceived
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <param name="originDateTime"></param>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],
        /// [StreetAddress],[SequenceNum],[BoxArmed]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetMessageFromHistory(int userId, int boxId, DateTime originDateTime)
        {
            return msgIn.GetMessageFromHistory(userId, boxId, originDateTime);
        }

        /// <summary>
        /// Retrieves last [num of records] messages from the history
        /// </summary>
        /// <param name="numOfRecords"></param>
        /// <param name="boxId"></param>
        /// <param name="msgType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <remarks>
        /// Purpose:	Debug communication (vlfHistory)
        /// Note:		Does not aggregate user preferences into result (TimeZone,Metric/Imperial units,etc.)
        /// </remarks>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],
        /// [StreetAddress],[SequenceNum],[BoxArmed]
        /// </returns>
        public DataSet GetLastMessagesFromHistory(short numOfRecords, int boxId, short msgType, DateTime from, DateTime to)
        {
            DataSet dsResult = msgIn.GetLastMessagesFromHistory(numOfRecords, boxId, msgType, from, to);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LastMessages";
                }
                dsResult.DataSetName = "MsgInHistory";
            }
            return dsResult;
        }


        /// <summary>
        /// Retrieves last [num of records] messages from the history
        /// </summary>
        /// <param name="numOfRecords"></param>
        /// <param name="boxId"></param>
        /// <param name="msgType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <remarks>
        /// Purpose:	Debug communication (vlfHistory)
        /// Note:		Does not aggregate user preferences into result (TimeZone,Metric/Imperial units,etc.)
        /// </remarks>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],
        /// [StreetAddress],[SequenceNum],[BoxArmed]
        /// </returns>
        public DataSet GetLastMessagesFromHistory(short numOfRecords, int boxId, short msgType, DateTime from, DateTime to, string OrderByDate)
        {
            DataSet dsResult = msgIn.GetLastMessagesFromHistory(numOfRecords, boxId, msgType, from, to, OrderByDate);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LastMessages";
                }
                dsResult.DataSetName = "MsgInHistory";
            }
            return dsResult;
        }




        /// <summary>
        /// Retrieves last [num of records] messages from the history
        /// </summary>
        /// <param name="numOfRecords"></param>
        /// <param name="boxId"></param>
        /// <param name="msgType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <remarks>
        /// Purpose:	Debug communication (vlfHistory)
        /// Note:		Does not aggregate user preferences into result (TimeZone,Metric/Imperial units,etc.)
        /// </remarks>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],
        /// [StreetAddress],[SequenceNum],[BoxArmed]
        /// </returns>
        public DataSet GetLastMessagesFromHistory(short numOfRecords, int boxId, short msgType, DateTime from, DateTime to, string OrderByDate, string whereDateTime)
        {
            DataSet dsResult = msgIn.GetLastMessagesFromHistory(numOfRecords, boxId, msgType, from, to, OrderByDate, whereDateTime);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LastMessages";
                }
                dsResult.DataSetName = "MsgInHistory";
            }
            return dsResult;
        }


        /// <summary>
        /// Retrieves last [num of records] messages from the history
        /// </summary>
        /// <param name="numOfRecords"></param>
        /// <param name="boxId"></param>
        /// <param name="msgType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <remarks>
        /// Purpose:	Debug communication (vlfHistory)
        /// Note:		Does not aggregate user preferences into result (TimeZone,Metric/Imperial units,etc.)
        /// </remarks>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],
        /// [StreetAddress],[SequenceNum],[BoxArmed]
        /// </returns>
        public DataSet GetLastMessagesFromHistory_Brickman201107c(short numOfRecords, int boxId, short msgType, Int64  from, Int64  to)
        {
            DataSet dsResult = msgIn.GetLastMessagesFromHistory_Brickman201107c(numOfRecords, boxId, msgType, from, to);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LastMessages";
                }
                dsResult.DataSetName = "MsgInHistory";
            }
            return dsResult;
        }


        /// <summary>
        /// Retrieves last [num of records] messages from the history By OriginDateTime
        /// </summary>
        /// <param name="numOfRecords"></param>
        /// <param name="boxId"></param>
        /// <param name="msgType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <remarks>
        /// Purpose:	Debug communication (vlfHistory)
        /// Note:		Does not aggregate user preferences into result (TimeZone,Metric/Imperial units,etc.)
        /// </remarks>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],
        /// [StreetAddress],[SequenceNum],[BoxArmed]
        /// </returns>
        public DataSet GetLastMessagesFromHistoryByOriginDateTime(short numOfRecords, int boxId, short msgType, DateTime from, DateTime to)
        {
            DataSet dsResult = msgIn.GetLastMessagesFromHistoryByOriginDateTime(numOfRecords, boxId, msgType, from, to);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LastMessages";
                }
                dsResult.DataSetName = "MsgInHistory";
            }
            return dsResult;
        }


        /// <summary>
        /// Retrieves last [num of records] messages from the history
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="from"></param>
        /// <param name="userId"></param>
        /// <remarks>
        /// Purpose:	Debug communication (vlfHistory)
        /// Note:		Does not aggregate user preferences into result (TimeZone,Metric/Imperial units,etc.)
        /// </remarks>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],
        /// [StreetAddress],[SequenceNum],[BoxArmed]
        /// </returns>
        public DataSet GetLastUploadFirmwareMessageFromHistory(int userId, int boxId, DateTime from)
        {
            DataSet dsResult = msgIn.GetLastUploadFirmwareMessageFromHistory(userId, boxId, from);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LastMessages";
                }
                dsResult.DataSetName = "MsgInHistory";
            }
            return dsResult;
        }


        /// <summary>
        /// Retrieves last [num of records] messages from the history
        /// </summary>
        /// <param name="numOfRecords"></param>
        /// <param name="boxId"></param>
        /// <param name="msgType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <remarks>
        /// Purpose:	Debug communication (vlfHistory)
        /// Note:		Does not aggregate user preferences into result (TimeZone,Metric/Imperial units,etc.)
        /// </remarks>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],
        /// [StreetAddress],[SequenceNum],[BoxArmed]
        /// </returns>
        public DataSet GetLastMessagesFromHistoryByOrganization(short numOfRecords, int orgId, int fleetId, int boxId, short msgType, DateTime from, DateTime to)
        {
            DataSet dsResult = msgIn.GetLastMessagesFromHistoryByOrganization(numOfRecords, orgId, fleetId, boxId, msgType, from, to);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LastMessages";
                }
                dsResult.DataSetName = "MsgInHistory";
            }
            return dsResult;
        }

        /// <summary>
        /// Retrieves last [num of records] messages from the history
        /// </summary>
        /// <param name="numOfRecords"></param>
        /// <param name="boxId"></param>
        /// <param name="cmdType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <remarks>
        /// Purpose:	Debug communication (vlfHistory)
        /// Note:		Does not aggregate user preferences into result (TimeZone,Metric/Imperial units,etc.)
        /// </remarks>
        /// <returns>DataSet [DateTime],[BoxId],[UserId],[Priority],[DclId],[AslId],
        /// [BoxCmdOutTypeId],[BoxCmdOutTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[CustomProp],[SequenceNum],[Acknowledged]
        /// </returns>
        public DataSet GetLastMessagesOutFromHistory(short numOfRecords, int boxId, short cmdType, DateTime from, DateTime to)
        {
            DataSet dsResult = msgOut.GetLastMessagesFromHistory(numOfRecords, boxId, cmdType, from, to);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LastMessages";
                }
                dsResult.DataSetName = "MsgOutHistory";
            }
            return dsResult;
        }


        /// <summary>
        /// Retrieves last [num of records] messages from the history
        /// </summary>
        /// <param name="numOfRecords"></param>
        /// <param name="boxId"></param>
        /// <param name="cmdType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <remarks>
        /// Purpose:	Debug communication (vlfHistory)
        /// Note:		Does not aggregate user preferences into result (TimeZone,Metric/Imperial units,etc.)
        /// </remarks>
        /// <returns>DataSet [DateTime],[BoxId],[UserId],[Priority],[DclId],[AslId],
        /// [BoxCmdOutTypeId],[BoxCmdOutTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[CustomProp],[SequenceNum],[Acknowledged]
        /// </returns>
        public DataSet GetLastMessagesOutFromHistoryByOrganization(short numOfRecords, int orgId, int fleetId, int boxId, short cmdType, DateTime from, DateTime to)
        {
            DataSet dsResult = msgOut.GetLastMessagesFromHistoryByOrganization(numOfRecords, orgId, fleetId, boxId, cmdType, from, to);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LastMessages";
                }
                dsResult.DataSetName = "MsgOutHistory";
            }
            return dsResult;
        }

        /// <summary>
        /// Get last sequence number from the message out history
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="boxProtocolTypeId"></param>
        /// <returns>Last Sequence Number</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int GetMsgOutLastSequenceNumber(int boxId, short boxProtocolTypeId)
        {
            return msgOut.GetLastSequenceNumber(boxId, boxProtocolTypeId);
        }
        /// <summary>
        /// Retrieves last UploadFirmwareStatus command from the history
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="fromDateTime"></param>
        /// <returns>DataSet [Description],[FwName],[FwId],[DateTime],[BoxId],[CustomProp],[Acknowledged]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetLastCommandFromHistory(int boxId, DateTime fromDateTime)
        {
            DataSet dsResult = msgOut.GetLastCommandFromHistory(boxId, fromDateTime);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LastMessage";
                }
                dsResult.DataSetName = "MsgOutHistory";
            }
            return dsResult;
        }
        /// <summary>
        /// Retrieves last [num of records] ignored messages from the history
        /// </summary>
        /// <param name="numOfRecords"></param>
        /// <param name="boxId"></param>
        /// <param name="msgType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <remarks>
        /// Purpose:	Debug communication (vlfHistory)
        /// Note:		Does not aggregate user preferences into result (TimeZone,Metric/Imperial units,etc.)
        /// </remarks>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],
        /// [SequenceNum],[BoxArmed]
        /// </returns>
        public DataSet GetLastMessagesFromHistoryIgnored(short numOfRecords, int boxId, short msgType, DateTime from, DateTime to)
        {
            DataSet dsResult = msgIn.GetLastMessagesFromHistoryIgnored(numOfRecords, boxId, msgType, from, to);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LastMessages";
                }
                dsResult.DataSetName = "MsgInHistory";
            }
            return dsResult;
        }
        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves detailed message in from history by box id and DateTime
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <param name="originDateTime"></param>
        /// <returns>DataSet [BoxId],[DateTime],
        /// [MsgTypeId],[MsgTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress],[BoxArmed],
        /// [UserName],[FirstName],[LastName]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetDetailedMessageInFromHistory_NewTZ(int userId, int boxId, DateTime originDateTime)
        {
            // 1. Retrieves message detailed info
            DataSet dsResult = msgIn.GetDetailedMessageFromHistory_NewTZ(userId, boxId, originDateTime);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "DetailedMessageInFromHistory";
                }
                dsResult.DataSetName = "MsgInHistory";
            }
            return dsResult;
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves detailed message in from history by box id and DateTime
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <param name="originDateTime"></param>
        /// <returns>DataSet [BoxId],[DateTime],
        /// [MsgTypeId],[MsgTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress],[BoxArmed],
        /// [UserName],[FirstName],[LastName]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetDetailedMessageInFromHistory(int userId, int boxId, DateTime originDateTime)
        {
            // 1. Retrieves message detailed info
            DataSet dsResult = msgIn.GetDetailedMessageFromHistory(userId, boxId, originDateTime);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "DetailedMessageInFromHistory";
                }
                dsResult.DataSetName = "MsgInHistory";
            }
            return dsResult;
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves detailed message out from history by box id and DateTime
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <param name="dateTime"></param>
        /// <returns>DataSet [BoxId],[DateTime],
        /// [MsgTypeId],[MsgTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],
        /// [CustomProp],[StreetAddress],[BoxArmed],
        /// [UserName],[FirstName],[LastName]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetDetailedMessageOutFromHistory_NewTZ(int userId, int boxId, DateTime dateTime)
        {
            DataSet dsResult = msgOut.GetDetailedMessageFromHistory_NewTZ(userId, boxId, dateTime);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "DetailedMessageOutFromHistory";
                }
                dsResult.DataSetName = "MsgOutHistory";
            }
            return dsResult;
        }
        // Changes for TimeZone Feature end
        /// <summary>
        /// Retrieves detailed message out from history by box id and DateTime
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <param name="dateTime"></param>
        /// <returns>DataSet [BoxId],[DateTime],
        /// [MsgTypeId],[MsgTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],
        /// [CustomProp],[StreetAddress],[BoxArmed],
        /// [UserName],[FirstName],[LastName]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetDetailedMessageOutFromHistory(int userId, int boxId, DateTime dateTime)
        {
            DataSet dsResult = msgOut.GetDetailedMessageFromHistory(userId, boxId, dateTime);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "DetailedMessageOutFromHistory";
                }
                dsResult.DataSetName = "MsgOutHistory";
            }
            return dsResult;
        }
        /// <summary>
        /// Retrieves records with empty StreetAddress fields from the MsgIn history
        /// </summary>
        /// <returns></returns>
        public DataSet GetEmptyStreetAddressFromHistory(int cmdTimeOut)
        {
            DataSet dsResult = msgIn.GetEmptyStreetAddressFromHistory(cmdTimeOut);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetEmptyStreetAddressFromHistory";
                }
                dsResult.DataSetName = "MsgInHistory";
            }
            return dsResult;
        }
        /// <summary>
        /// Retrieves landmarks info by box id 
        /// </summary>
        /// <returns>
        /// DataSet [OrganizationId],[LandmarkName],[Latitude],[Longitude],
        /// [Description],[ContactPersonName],[ContactPhoneNum],[Radius],
        /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
        /// [StreetAddress]
        /// </returns>
        /// <param name="boxId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetLandMarksInfoByBoxId(int boxId)
        {
            DataSet dsResult = landmark.GetLandMarksInfoByBoxId(boxId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetLandMarksInfoByBoxId";
                }
                dsResult.DataSetName = "Landmarks";
            }
            return dsResult;
        }


        /// <summary>
        /// Retrive Not Processed Messages from History
        /// </summary>
        /// <param name="numOfDays"></param>
        /// <param name="boxId"></param>
        /// <param name="boxMsgInTypeId"></param>
        /// <returns></returns>

        public DataSet GetLastMessagesFromHistoryNotProcessed(Int64 boxId,
                                                    Int16 boxMsgInTypeId)
        {
            DataSet dsResult = msgIn.GetLastMessagesFromHistoryNotProcessed(boxId, boxMsgInTypeId);
            return dsResult;
        }
        /// <summary>
        /// Updates record with street address and/or nearestLandmark
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="originDateTime"></param>
        /// <param name="streetAddress"></param>
        /// <param name="cmdTimeOut"></param>
        /// <param name="usageYear"></param>
        /// <param name="usageMonth"></param>
        /// <param name="nearestLandmark"></param>
        /// <param name="mapId"></param>
        public void UpdateStreetAddressInHistory(int boxId, DateTime originDateTime, string streetAddress, int cmdTimeOut, short usageYear, short usageMonth, string nearestLandmark, short mapId)
        {
            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Update box street address in the history
                msgIn.UpdateStreetAddressInHistory(boxId, originDateTime, streetAddress, cmdTimeOut, nearestLandmark);
                // 3. Add box map usage
                mapEngine.AddMapBoxUsage(boxId, (short)VLF.CLS.Def.Enums.MapTypes.StreetAddress, usageYear, usageMonth, mapId);
                // 4. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Unable to add new box ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
        }
        /// <summary>
        /// Updates record with street address and/or nearestLandmark
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="originDateTime"></param>
        /// <param name="streetAddress"></param>
        /// <param name="cmdTimeOut"></param>
        /// <param name="usageYear"></param>
        /// <param name="usageMonth"></param>
        /// <param name="nearestLandmark"></param>
        /// <param name="mapId"></param>
        public void UpdateStreetAddressInBoxAndHistory(int boxId, DateTime originDateTime, string streetAddress, int cmdTimeOut, short usageYear, short usageMonth, string nearestLandmark, short mapId)
        {
            DB.Box box = new DB.Box(sqlExec);
            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Update box street address in the history
                //Street Address in vlfMsgInHist will be updated only by SLS
                // msgIn.UpdateStreetAddressInHistory(boxId, originDateTime, streetAddress, cmdTimeOut, nearestLandmark);
                // 3. Update box street address
                box.UpdateStreetAddress(boxId, streetAddress, cmdTimeOut, nearestLandmark);
                // 4. Add box map usage
                mapEngine.AddMapBoxUsage(boxId, (short)VLF.CLS.Def.Enums.MapTypes.StreetAddress, usageYear, usageMonth, mapId);
                // 5. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Unable to add new box ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
        }
        /// <summary>
        /// Add new Msg in CMFIn format into the history.
        /// </summary>
        /// <exception cref="DASAppDataAlreadyExistsException">Throws DASAppDataAlreadyExistsException after number of attemps to add new information.</exception>
        /// <exception cref="DASException">Throws DASException in other all error cases.</exception>
        /// <param name="cMFIn"></param>
        public void AddMsgInHst(CMFIn cMFIn)
        {
            msgIn.AddToHistory(cMFIn, "vlfMsgInHst");
        }
        public DataSet GetGetOrgBoxes(int orgId)
        {
            return msgIn.GetGetOrgBoxes(orgId);
        }
        public DataSet GetOrgAllBoxes(int orgId)
        {
            return msgIn.GetOrgAllBoxes(orgId);
        }

        #endregion

        #region TextMessaging Interfaces



        /// <summary>
        /// Add new text message
        /// </summary>
        /// <exception cref="DASAppDataAlreadyExistsException">Throws DASAppDataAlreadyExistsException information already exist.</exception>
        /// <exception cref="DASException">Throws DASException in all other error cases.</exception>
        /// <returns> current message id or -1 in case of error</returns>
        public int AddTextMsg(int boxId, DateTime msgDateTime, short txtMsgTypeId, string msgBody, short msgDirection, int userId, string ack)
        {
            return textMsg.AddMsg(boxId, msgDateTime, txtMsgTypeId, msgBody, msgDirection, userId, ack);
        }


        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves text messages full information
        /// </summary>
        /// <remarks>
        /// all box Ids -> VLF.CLS.Def.Const.unassignedIntValue
        /// from DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// to DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="directionType"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetTextMessagesFullInfo_NewTZ(int userId, int boxId, DateTime fromDateTime, DateTime toDateTime, short directionType)
        {
            DataSet dsResult = new DataSet();
            DB.VehicleAssignment vehicAssgn = new DB.VehicleAssignment(sqlExec);
            Int64 vehicleId = Convert.ToInt64(vehicAssgn.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
            // Retrieve all historical box/licensePlate assignments during report period (from,to)
            DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
            //[AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]
            DataSet dsAssignHst = assignHst.GetAllVehiclesAssignmentsBy("VehicleId", vehicleId, Convert.ToDateTime(fromDateTime), Convert.ToDateTime(toDateTime), " ORDER BY AssignedDateTime DESC");

            if (dsAssignHst != null && dsAssignHst.Tables.Count > 0)
            {
                // 3. For each assignment in the range of from/to datetime mearge results
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
                        currAssignTo = Convert.ToDateTime(toDateTime);

                    if (currAssignFrom.Ticks < Convert.ToDateTime(fromDateTime).Ticks)
                        currAssignFrom = new DateTime(Convert.ToDateTime(fromDateTime).Ticks);

                    // [VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],
                    // [TxtMsgTypeId],[MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],
                    // [UserId]
                    currResult = textMsg.GetMessagesFullInfo_NewTZ(userId,
                                                            Convert.ToInt32(ittr["BoxId"]),
                                                            currAssignFrom,
                                                            currAssignTo,
                                                            directionType);

                    if (currResult != null && currResult.Tables.Count > 0)
                    {
                        // 3. Merge current vehicle text messages
                        if (dsResult != null && currResult != null)
                            dsResult.Merge(currResult);
                    }
                }
                // 3.2	Add current assignment 
                // [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
                DB.VehicleAssignment assign = new DB.VehicleAssignment(sqlExec);
                DataSet dsAssign = assign.GetVehicleAssignmentBy("BoxId", boxId);
                if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
                {
                    currAssignFrom = Convert.ToDateTime(dsAssign.Tables[0].Rows[0]["AssignedDateTime"]);
                    if (currAssignFrom.Ticks < Convert.ToDateTime(fromDateTime).Ticks)
                        currAssignFrom = new DateTime(Convert.ToDateTime(fromDateTime).Ticks);
                    currResult = textMsg.GetMessagesFullInfo_NewTZ(userId,
                                                            boxId,
                                                            currAssignFrom,
                                                            toDateTime,
                                                            directionType);
                    if (dsResult != null && currResult != null)
                        dsResult.Merge(currResult);
                }
            }
            DataSet dsFilteredResult = null;
            #region Sort result by MsgDateTime DESC
            if (dsResult != null)
            {
                dsResult.DataSetName = "TextMessages";
                dsFilteredResult = dsResult.Clone();
                if (dsResult.Tables.Count > 0)
                {
                    DB.VehicleInfo vehicInfo = new DB.VehicleInfo(sqlExec);
                    DataSet dsVehicInfo = vehicInfo.GetVehicleInfoByVehicleId_NewTZ(vehicleId);
                    if (dsVehicInfo != null && dsVehicInfo.Tables.Count > 0 && dsVehicInfo.Tables[0].Rows.Count > 0)
                    {
                        dsFilteredResult.Tables[0].TableName = "VehicleTextMessagesFullInfo";
                        DataRow[] filteredRows = dsResult.Tables[0].Select("", "MsgDateTime DESC");
                        foreach (DataRow ittr in filteredRows)
                        {
                            ittr["VehicleId"] = dsVehicInfo.Tables[0].Rows[0]["VehicleId"].ToString().TrimEnd();
                            ittr["LicensePlate"] = dsVehicInfo.Tables[0].Rows[0]["LicensePlate"].ToString().TrimEnd();
                            if (ittr["MsgDirection"].ToString() == VLF.CLS.Def.Const.MsgInDirectionSign)
                                ittr["From"] = dsVehicInfo.Tables[0].Rows[0]["Description"].ToString().TrimEnd();
                            else if (ittr["MsgDirection"].ToString() == VLF.CLS.Def.Const.MsgOutDirectionSign)
                                ittr["To"] = dsVehicInfo.Tables[0].Rows[0]["Description"].ToString().TrimEnd();
                            dsFilteredResult.Tables[0].ImportRow(ittr);
                        }
                    }
                }
            }
            #endregion
            return dsFilteredResult;
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves text messages full information
        /// </summary>
        /// <remarks>
        /// all box Ids -> VLF.CLS.Def.Const.unassignedIntValue
        /// from DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// to DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="directionType"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetTextMessagesFullInfo(int userId, int boxId, DateTime fromDateTime, DateTime toDateTime, short directionType)
        {
            DataSet dsResult = new DataSet();
            DB.VehicleAssignment vehicAssgn = new DB.VehicleAssignment(sqlExec);
            Int64 vehicleId = Convert.ToInt64(vehicAssgn.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
            // Retrieve all historical box/licensePlate assignments during report period (from,to)
            DB.VehicleAssignmentHst assignHst = new DB.VehicleAssignmentHst(sqlExec);
            //[AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]
            DataSet dsAssignHst = assignHst.GetAllVehiclesAssignmentsBy("VehicleId", vehicleId, Convert.ToDateTime(fromDateTime), Convert.ToDateTime(toDateTime), " ORDER BY AssignedDateTime DESC");

            if (dsAssignHst != null && dsAssignHst.Tables.Count > 0)
            {
                // 3. For each assignment in the range of from/to datetime mearge results
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
                        currAssignTo = Convert.ToDateTime(toDateTime);

                    if (currAssignFrom.Ticks < Convert.ToDateTime(fromDateTime).Ticks)
                        currAssignFrom = new DateTime(Convert.ToDateTime(fromDateTime).Ticks);

                    // [VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],
                    // [TxtMsgTypeId],[MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],
                    // [UserId]
                    currResult = textMsg.GetMessagesFullInfo(userId,
                                                            Convert.ToInt32(ittr["BoxId"]),
                                                            currAssignFrom,
                                                            currAssignTo,
                                                            directionType);

                    if (currResult != null && currResult.Tables.Count > 0)
                    {
                        // 3. Merge current vehicle text messages
                        if (dsResult != null && currResult != null)
                            dsResult.Merge(currResult);
                    }
                }
                // 3.2	Add current assignment 
                // [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
                DB.VehicleAssignment assign = new DB.VehicleAssignment(sqlExec);
                DataSet dsAssign = assign.GetVehicleAssignmentBy("BoxId", boxId);
                if (dsAssign != null && dsAssign.Tables.Count > 0 && dsAssign.Tables[0].Rows.Count > 0)
                {
                    currAssignFrom = Convert.ToDateTime(dsAssign.Tables[0].Rows[0]["AssignedDateTime"]);
                    if (currAssignFrom.Ticks < Convert.ToDateTime(fromDateTime).Ticks)
                        currAssignFrom = new DateTime(Convert.ToDateTime(fromDateTime).Ticks);
                    currResult = textMsg.GetMessagesFullInfo(userId,
                                                            boxId,
                                                            currAssignFrom,
                                                            toDateTime,
                                                            directionType);
                    if (dsResult != null && currResult != null)
                        dsResult.Merge(currResult);
                }
            }
            DataSet dsFilteredResult = null;
            #region Sort result by MsgDateTime DESC
            if (dsResult != null)
            {
                dsResult.DataSetName = "TextMessages";
                dsFilteredResult = dsResult.Clone();
                if (dsResult.Tables.Count > 0)
                {
                    DB.VehicleInfo vehicInfo = new DB.VehicleInfo(sqlExec);
                    DataSet dsVehicInfo = vehicInfo.GetVehicleInfoByVehicleId(vehicleId);
                    if (dsVehicInfo != null && dsVehicInfo.Tables.Count > 0 && dsVehicInfo.Tables[0].Rows.Count > 0)
                    {
                        dsFilteredResult.Tables[0].TableName = "VehicleTextMessagesFullInfo";
                        DataRow[] filteredRows = dsResult.Tables[0].Select("", "MsgDateTime DESC");
                        foreach (DataRow ittr in filteredRows)
                        {
                            ittr["VehicleId"] = dsVehicInfo.Tables[0].Rows[0]["VehicleId"].ToString().TrimEnd();
                            ittr["LicensePlate"] = dsVehicInfo.Tables[0].Rows[0]["LicensePlate"].ToString().TrimEnd();
                            if (ittr["MsgDirection"].ToString() == VLF.CLS.Def.Const.MsgInDirectionSign)
                                ittr["From"] = dsVehicInfo.Tables[0].Rows[0]["Description"].ToString().TrimEnd();
                            else if (ittr["MsgDirection"].ToString() == VLF.CLS.Def.Const.MsgOutDirectionSign)
                                ittr["To"] = dsVehicInfo.Tables[0].Rows[0]["Description"].ToString().TrimEnd();
                            dsFilteredResult.Tables[0].ImportRow(ittr);
                        }
                    }
                }
            }
            #endregion
            return dsFilteredResult;
        }
        /// <summary>
        /// Retrieves message info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="msgId"></param>
        /// <param name="vehicleId"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
        /// [Acknowledged],[UserName],
        /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetMessageFullInfo(int userId, int msgId, Int64 vehicleId)
        {
            // 1. Retrieves text message detailed info
#if MDT_NEW
         DataSet dsResult = textMsg.GetMessageFullInfo((int)vehicleId, userId, msgId);
#else
            DataSet dsResult = textMsg.GetMessageFullInfo(userId, msgId);
#endif
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "VehicleTextMessageFullInfo";
                    if (dsResult.Tables[0].Rows.Count > 0)
                    {
                        DB.VehicleInfo vehicInfo = new DB.VehicleInfo(sqlExec);
                        DataSet dsVehicInfo = vehicInfo.GetVehicleInfoByVehicleId(vehicleId);
                        if (dsVehicInfo != null && dsVehicInfo.Tables.Count > 0 && dsVehicInfo.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow ittr in dsResult.Tables[0].Rows)
                            {
                                ittr["LicensePlate"] = dsVehicInfo.Tables[0].Rows[0]["LicensePlate"].ToString().TrimEnd();
                                if (ittr["MsgDirection"].ToString() == VLF.CLS.Def.Const.MsgInDirectionSign)
                                    ittr["From"] = dsVehicInfo.Tables[0].Rows[0]["Description"].ToString().TrimEnd();
                                else if (ittr["MsgDirection"].ToString() == VLF.CLS.Def.Const.MsgOutDirectionSign)
                                    ittr["To"] = dsVehicInfo.Tables[0].Rows[0]["Description"].ToString().TrimEnd();
                            }
                        }
                    }
                }
                dsResult.DataSetName = "TextMessages";
            }
            return dsResult;
        }

#if MDT_NEW
      /// <summary>
		/// Save message response
		/// </summary>
      /// <param name="boxId"></param>       
		/// <param name="msgId"></param>
		/// <param name="respondDateTime"></param>
		/// <param name="msgResponse"></param>
		/// <returns></returns>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		public int SetMsgResponse(int boxId, int msgId,DateTime respondDateTime,string msgResponse)
		{
			return textMsg.SetMsgResponse(boxId, msgId,respondDateTime,msgResponse);
		}


      /// <summary>
		///         Save message response
		/// </summary>
      /// <param name="boxId"></param>       
		/// <param name="msgId"></param>
		/// <param name="respondDateTime"></param>
		/// <param name="msgResponse"></param>
		/// <returns></returns>
      /// <comment>
      ///         that is the part which enforces using vlfMsgTextNew, it is called only from SLS (I hope)
      ///         
      /// </comment>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		public int SetTextMsgResponse(int boxid, int msgId,DateTime respondDateTime,string msgResponse)
		{
			return textMsg.SetMsgResponse(boxid, msgId,respondDateTime,msgResponse);
		}

      /// <summary>
		/// Set message user
		/// </summary>
      /// <param name="boxId"></param>       
		/// <param name="msgId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		public int SetMsgUserId(int boxid, int msgId,int userId)
		{
         return textMsg.SetMsgUserId(boxid, msgId, userId);
		}
		/// <summary>
		/// Set message ack
		/// </summary>
      /// <param name="boxId"></param>       
		/// <param name="msgId"></param>
		/// <param name="ack"></param>
		/// <returns></returns>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		public int SetTxtMsgAck(int boxid, int msgId,string ack)
		{
			return textMsg.SetMsgAck(boxid, msgId,ack);
		}
#else
        /// <summary>
        /// Save message response
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="respondDateTime"></param>
        /// <param name="msgResponse"></param>
        /// <returns></returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public int SetMsgResponse(int msgId, DateTime respondDateTime, string msgResponse)
        {
            return textMsg.SetMsgResponse(msgId, respondDateTime, msgResponse);
        }

        /// <summary>
        ///         Save message response
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="respondDateTime"></param>
        /// <param name="msgResponse"></param>
        /// <returns></returns>
        /// <comment>
        ///         that is the part which enforces using vlfMsgTextNew, it is called only from SLS (I hope)
        ///         
        /// </comment>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public int SetTextMsgResponse(int msgId, DateTime respondDateTime, string msgResponse)
        {
            return textMsg.SetMsgResponse(msgId, respondDateTime, msgResponse);
        }

        /// <summary>
        /// Set message user
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public int SetMsgUserId(int msgId, int userId)
        {
            return textMsg.SetMsgUserId(msgId, userId);
        }

      


        // <summary>
        /// Set message user
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public int SetMsgUserIdExtended(int msgId, int userId, int msgTypeId, DateTime originDateTime, DateTime touchDateTime, int peripheralId, Int64 checksumId)
        {
            return textMsg.SetMsgUserIdExtended(msgId, userId, msgTypeId, originDateTime, touchDateTime, peripheralId, checksumId);
        }

        /// <summary>
        /// Set message ack
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="ack"></param>
        /// <returns></returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public int SetTxtMsgAck(int msgId, string ack)
        {
            return textMsg.SetMsgAck(msgId, ack);
        }
#endif
        /// <summary>
        /// Retrieves user's messages 
        /// </summary>
        /// <remarks>
        /// from DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// to DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// 1. In messages directions -> VLF.CLS.Def.Enums.TxtMsgType.MdtText
        /// 2. Out messages directions -> VLF.CLS.Def.Enums.TxtMsgType.ClientText
        /// 3. Both messages directions -> MdtText,ClientText (all visible to client messages)
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="directionType"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetUserTextMessagesFullInfo(int userId, DateTime from, DateTime to, short directionType)
        {
            DataSet dsResult = textMsg.GetUserMessagesFullInfo(userId, from, to, directionType);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "UserTextMessagesFullInfo";
                }
                dsResult.DataSetName = "TextMessages";
            }
            return dsResult;
        }
        /// <summary>
        /// Retrieves fleet's messages
        /// </summary>
        /// <remarks>
        /// from DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// to DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// 1. In messages directions -> VLF.CLS.Def.Enums.TxtMsgType.MdtText
        /// 2. Out messages directions -> VLF.CLS.Def.Enums.TxtMsgType.ClientText
        /// 3. Both messages directions -> MdtText,ClientText (all visible to client messages)
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="directionType"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],[Scheduled]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetFleetTextMessagesFullInfo(int userId, int fleetId, DateTime from, DateTime to, short directionType)
        {
            DataSet dsResult = new DataSet();
            DB.FleetVehicles fleetVehicles = new DB.FleetVehicles(sqlExec);
            // [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]
            DataSet dsFleetVehicles = fleetVehicles.GetVehiclesInfoByFleetId(fleetId);
            if (dsFleetVehicles != null && dsFleetVehicles.Tables.Count > 0)
            {
                DataSet currResult = null;
                foreach (DataRow ittr in dsFleetVehicles.Tables[0].Rows)
                {
                    // [VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],
                    // [MsgDateTime],[TxtMsgTypeId],[MsgBody],[MsgDirection],
                    // [MsgResponse],[ResponseDateTime],[UserId],[Scheduled]
                    currResult = GetTextMessagesFullInfo(userId,
                                                        Convert.ToInt32(ittr["BoxId"]),
                                                        from,
                                                        to,
                                                        directionType);
                    // 3. Merge current vehicle text messages
                    if (dsResult != null && currResult != null)
                        dsResult.Merge(currResult);
                }
            }
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    if (dsResult.Tables[0].Rows.Count > 0)
                    {
                        #region Sort result by DateTimeCreated
                        DataRow[] filterResult = dsResult.Tables[0].Select("", "MsgDateTime DESC");
                        DataTable tblResult = dsResult.Tables[0].Clone();
                        foreach (DataRow ittr in filterResult)
                            tblResult.ImportRow(ittr);
                        dsResult.Tables.Clear();
                        dsResult.Tables.Add(tblResult);
                        #endregion
                    }
                    dsResult.Tables[0].TableName = "FleetTextMessagesFullInfo";
                }
                dsResult.DataSetName = "TextMessages";
            }
            return dsResult;
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves user's messages 
        /// </summary>
        /// <remarks>
        /// from DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// to DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// 1. In messages directions -> VLF.CLS.Def.Enums.TxtMsgType.MdtText
        /// 2. Out messages directions -> VLF.CLS.Def.Enums.TxtMsgType.ClientText
        /// 3. Both messages directions -> MdtText,ClientText (all visible to client messages)
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="directionType"></param>
        /// <returns>DataSet [VehicleId],[Description],[LicensePlate],
        /// [MsgId],[BoxId],[MsgDateTime],[MsgBody (20)]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetUserTextMessagesShortInfo_NewTZ(int userId, DateTime from, DateTime to, short directionType)
        {
            DataSet dsResult = textMsg.GetUserMessagesShortInfo_NewTZ(userId, from, to, directionType);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "UserTextMessagesShortInfo";
                }
                dsResult.DataSetName = "TextMessages";
            }
            return dsResult;
        }
        // Changes for TimeZone Feature end


        /// <summary>
        /// Retrieves user's messages 
        /// </summary>
        /// <remarks>
        /// from DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// to DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// 1. In messages directions -> VLF.CLS.Def.Enums.TxtMsgType.MdtText
        /// 2. Out messages directions -> VLF.CLS.Def.Enums.TxtMsgType.ClientText
        /// 3. Both messages directions -> MdtText,ClientText (all visible to client messages)
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="directionType"></param>
        /// <returns>DataSet [VehicleId],[Description],[LicensePlate],
        /// [MsgId],[BoxId],[MsgDateTime],[MsgBody (20)]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetUserTextMessagesShortInfo(int userId, DateTime from, DateTime to, short directionType)
        {
            DataSet dsResult = textMsg.GetUserMessagesShortInfo(userId, from, to, directionType);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "UserTextMessagesShortInfo";
                }
                dsResult.DataSetName = "TextMessages";
            }
            return dsResult;
        }

        // Changes for TimeZone Feature start

        /// <summary>
        /// Retrieves user's messages 
        /// </summary>
        /// <remarks>
        /// from DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// to DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// 1. In messages directions -> VLF.CLS.Def.Enums.TxtMsgType.MdtText
        /// 2. Out messages directions -> VLF.CLS.Def.Enums.TxtMsgType.ClientText
        /// 3. Both messages directions -> MdtText,ClientText (all visible to client messages)
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="directionType"></param>
        /// <returns>DataSet [VehicleId],[Description],[LicensePlate],
        /// [MsgId],[BoxId],[MsgDateTime],[MsgBody (20)]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetUserIncomingTextMessagesShortInfoSP_NewTZ(int userId, DateTime from, DateTime to)
        {
            DataSet dsResult = textMsg.GetIncomingMessagesShortInfoSP_NewTZ(from, to, userId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "UserTextMessagesShortInfo";
                }
                dsResult.DataSetName = "TextMessages";
            }
            return dsResult;
        }


        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves user's messages 
        /// </summary>
        /// <remarks>
        /// from DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// to DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// 1. In messages directions -> VLF.CLS.Def.Enums.TxtMsgType.MdtText
        /// 2. Out messages directions -> VLF.CLS.Def.Enums.TxtMsgType.ClientText
        /// 3. Both messages directions -> MdtText,ClientText (all visible to client messages)
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="directionType"></param>
        /// <returns>DataSet [VehicleId],[Description],[LicensePlate],
        /// [MsgId],[BoxId],[MsgDateTime],[MsgBody (20)]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetUserIncomingTextMessagesShortInfoSP(int userId, DateTime from, DateTime to)
        {
            DataSet dsResult = textMsg.GetIncomingMessagesShortInfoSP(from, to, userId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "UserTextMessagesShortInfo";
                }
                dsResult.DataSetName = "TextMessages";
            }
            return dsResult;
        }



        /// <summary>
        /// Retrieves user's messages 
        /// </summary>
        /// <remarks>
        /// from DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// to DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// 1. In messages directions -> VLF.CLS.Def.Enums.TxtMsgType.MdtText
        /// 2. Out messages directions -> VLF.CLS.Def.Enums.TxtMsgType.ClientText
        /// 3. Both messages directions -> MdtText,ClientText (all visible to client messages)
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="directionType"></param>
        /// <returns>DataSet [VehicleId],[Description],[LicensePlate],
        /// [MsgId],[BoxId],[MsgDateTime],[MsgBody (20)]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetUserIncomingTextMessagesFullInfo(int userId, int msgId, int peripheralId, int msgTypeId, DateTime msgDateTime, int vehicleId)
        {
            DataSet dsResult = textMsg.GetUserIncomingTextMessagesFullInfoSP(userId, msgId, peripheralId, msgTypeId, msgDateTime, vehicleId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "VehicleTextMessagesFullInfo";
                }
                dsResult.DataSetName = "TextMessages";
            }
            return dsResult;
        }


        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves message info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="msgId"></param>
        /// <param name="vehicleId"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
        /// [Acknowledged],[UserName],
        /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetFleetTxtMsgs_NewTZ(int userId, int fleetId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            //  Retrieves text message detailed info
            DataSet dsResult = textMsg.GetFleetTxtMsgs_NewTZ(userId, fleetId, fromDate, toDate, msgDirection);
            return dsResult;
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves message info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="msgId"></param>
        /// <param name="vehicleId"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
        /// [Acknowledged],[UserName],
        /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetFleetTxtMsgs(int userId, int fleetId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            //  Retrieves text message detailed info
            DataSet dsResult = textMsg.GetFleetTxtMsgs(userId, fleetId, fromDate, toDate, msgDirection);
            return dsResult;
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves all message info (MDT+Garmin)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="msgId"></param>
        /// <param name="vehicleId"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
        /// [Acknowledged],[UserName],
        /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetFleetAllTxtMsgs_NewTZ(int userId, int fleetId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            //  Retrieves text message detailed info
            DataSet dsResult = textMsg.GetFleetAllTxtMsgs_NewTZ(userId, fleetId, fromDate, toDate, msgDirection);
            return dsResult;
        }



        // Changes for TimeZone Feature end


        /// <summary>
        /// Retrieves all message info (MDT+Garmin)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="msgId"></param>
        /// <param name="vehicleId"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
        /// [Acknowledged],[UserName],
        /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetFleetAllTxtMsgs(int userId, int fleetId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            //  Retrieves text message detailed info
            DataSet dsResult = textMsg.GetFleetAllTxtMsgs(userId, fleetId, fromDate, toDate, msgDirection);
            return dsResult;
        }


        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves all message info (MDT+Garmin)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="msgId"></param>
        /// <param name="vehicleId"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
        /// [Acknowledged],[UserName],
        /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetFleetAllDestinations_NewTZ(int userId, int fleetId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            //  Retrieves text message detailed info
            DataSet dsResult = textMsg.GetFleetAllDestinations_NewTZ(userId, fleetId, fromDate, toDate, msgDirection);
            return dsResult;
        }



        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves all message info (MDT+Garmin)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="msgId"></param>
        /// <param name="vehicleId"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
        /// [Acknowledged],[UserName],
        /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetFleetAllDestinations(int userId, int fleetId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            //  Retrieves text message detailed info
            DataSet dsResult = textMsg.GetFleetAllDestinations(userId, fleetId, fromDate, toDate, msgDirection);
            return dsResult;
        }

        // Changes for TimeZone Feature start

        /// <summary>
        /// Retrieves all message info (MDT+Garmin) by BoxId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="msgId"></param>
        /// <param name="vehicleId"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
        /// [Acknowledged],[UserName],
        /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetVehicleAllTxtMsgs_NewTZ(int userId, int boxId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            //  Retrieves text message detailed info
            DataSet dsResult = textMsg.GetVehicleAllTxtMsgs_NewTZ(userId, boxId, fromDate, toDate, msgDirection);
            return dsResult;
        }



        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves all message info (MDT+Garmin) by BoxId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="msgId"></param>
        /// <param name="vehicleId"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
        /// [Acknowledged],[UserName],
        /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetVehicleAllTxtMsgs(int userId, int boxId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            //  Retrieves text message detailed info
            DataSet dsResult = textMsg.GetVehicleAllTxtMsgs(userId, boxId, fromDate, toDate, msgDirection);
            return dsResult;
        }

        // Changes for TimeZone Feature start

        /// <summary>
        /// Retrieves all message info (MDT+Garmin) by BoxId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="msgId"></param>
        /// <param name="vehicleId"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
        /// [Acknowledged],[UserName],
        /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetVehicleAllDestinations_NewTZ(int userId, int boxId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            //  Retrieves text message detailed info
            DataSet dsResult = textMsg.GetVehicleAllDestinations_NewTZ(userId, boxId, fromDate, toDate, msgDirection);
            return dsResult;
        }

        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves all message info (MDT+Garmin) by BoxId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="msgId"></param>
        /// <param name="vehicleId"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
        /// [Acknowledged],[UserName],
        /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
        /// <exception cref="DASException">Throws DASException in all error cases.</exception>
        public DataSet GetVehicleAllDestinations(int userId, int boxId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            //  Retrieves text message detailed info
            DataSet dsResult = textMsg.GetVehicleAllDestinations(userId, boxId, fromDate, toDate, msgDirection);
            return dsResult;
        }

        /// <summary>
        /// Retrieves messages from history by fleet Id
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <remarks>
        /// 1. Retrieves only specific messages from the history:
        /// Coordinate,Sensor,Speed,Fence,PositionUpdate,IPUpdate,KeyFobPanic
        /// 2. Parse CustomProp field for Sensor and Fence messages, and add info into MsgDetails field
        /// 3. Incase of IPUpdate message add new IP into MsgDetails field
        /// </remarks>
        /// <returns>
        /// DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],
        /// [SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetMessagesFromHistoryByFleetId(int fleetId,
                                                     DateTime from,
                                                     DateTime to)
        {
            DataSet dsResult = msgIn.GetMessagesFromHistoryByFleetId(fleetId, from, to);
            return dsResult;
        }

        /// <summary>
        ///       add latitude/longitude info for every message
        /// </summary>
        /// <param name="cmfIn"></param>
        /// <param name="txtMsgType"></param>
        /// <param name="msgDirection"></param>
        /// <param name="userId"></param>
        /// <param name="strAck"></param>
        /// <returns></returns>
        public int AddTextMsg(CMFIn cmfIn, short txtMsgType)
        {
            if (null != cmfIn)
            {
                return textMsg.AddMsg(cmfIn.boxID,
                                      cmfIn.originatedDateTime,
                                      cmfIn.latitude,
                                      cmfIn.longitude,
                                      txtMsgType,
                                      CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyMessage, cmfIn.customProperties),
                                      (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In,
                                      VLF.CLS.Def.Const.unassignedIntValue,
                                      VLF.CLS.Def.Const.txtMsgAckNA);
            }
            else
                return -1;
        }

        /*
                /// <summary>
                /// Retrieves MDT Form messages
                /// </summary>
                /// <param name="fromDate"></param>
                /// <param name="toDate"></param>
                /// <param name="fleetId"></param>
                /// <param name="boxId"></param>
        
     
                 public DataSet GetMDTFormsMessages(DateTime fromDate, DateTime toDate, int fleetId, int boxId,int formId)
                 {
                   DataSet dsResult = textMsg.GetMDTFormsMessages(fromDate, toDate, fleetId, boxId,formId);
                   return dsResult;
                 }

               /// <summary>
              /// Retrieves MDT FormSchema
              /// </summary>
              /// <param name="organizationId"></param>
              /// <param name="formId"></param>

              public string GetMDTFormSchema(int organizationId, int formId)
              {
                 string retResult = textMsg.GetMDTFormSchema(organizationId, formId);
                 return retResult;
              }
        */
        #endregion

        #region Diagnostic Interfaces

        /// <summary>
        /// Retrieves statistic for invalid GPS within 24 hours
        /// </summary>
        /// <param name="InvalidGPSPercent"></param>
        /// <param name="Hours"></param>
        /// <returns>
        /// DataSet [BoxId],[LicensePlate],[VehicleId],[Description],
        /// [OrganizationName],[InvalidMsgs],[ValidMsgs],[PercentInvalidMsgs]
        /// </returns>
        public DataSet GetInvalidGPSStatistic(int InvalidGPSPercent, int Hours)
        {
            DataSet dsResult = msgIn.GetInvalidGPSStatistic(InvalidGPSPercent, Hours);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "InvalidMsgsStatistic";
                }
                dsResult.DataSetName = "InvalidMsgsStatistic";
            }
            return dsResult;
        }

        /// <summary>
        /// Retrieves boxes without IP Updates  for all boxes within 24 hours
        /// </summary>
        /// <param name="Hours"></param>
        /// <returns>
        /// DataSet [BoxId],[LicensePlate],[VehicleId],[Description],
        /// [OrganizationName]
        /// </returns>
        public DataSet GetBoxesWithoutIpUpdates(int Hours)
        {
            DataSet dsResult = msgIn.GetBoxesWithoutIpUpdates(Hours);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "BoxesWithoutIpUpdates";
                }
                dsResult.DataSetName = "BoxesWithoutIpUpdates";
            }
            return dsResult;
        }
        /// <summary>
        /// Retrieves boxes reported Frequency  for all boxes within 24 hours
        /// </summary>
        /// <param name="Hours"></param>
        /// <param name="TotalMsg"></param>
        /// <param name="OrganizationId"></param>
        /// <returns>
        /// DataSet [BoxId],[TotalMessages],[OrganizationName]
        /// </returns>
        public DataSet GetBoxesReportedFrequency(int Hours, int TotalMsg, Int16 OrganizationId)
        {
            DataSet dsResult = msgIn.GetBoxesReportedFrequency(Hours, TotalMsg, OrganizationId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetBoxesReportedFrequency";
                }
                dsResult.DataSetName = "GetBoxesReportedFrequency";
            }
            return dsResult;
        }
        #endregion Diagnostic Interfaces
    }
}

