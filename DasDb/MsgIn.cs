using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using System.Diagnostics;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;
using System.Text;			// for CMFIn

namespace VLF.DAS.DB
{
    /// <summary>
    /// "vlfMsgIn" table structure
    /// </summary>
    public enum MsgInTblStruct
    {
        DateTimeReceived,
        BoxId,
        DclId,
        CommModeId,
        BoxMsgInTypeId,
        BoxProtocolTypeId,
        OriginDateTime,
        CommInfo1,
        CommInfo2,
        ValidGps,
        Latitude,
        Longitude,
        Speed,
        Heading,
        SensorMask,
        CustomProp,
        BlobData,
        BlobDataSize,
        SequenceNum,
        IsArmed,
        Priority,
        MaxColumn
    }
    /// <summary>
    /// Provides interfaces to vlfMsgIn table.
    /// </summary>
    public class MsgIn : TblGenInterfaces
    {
#if BULK_SUPPORT
      static StringBuilder    strNonQueryStatements ;       ///< add operations to this string
      static System.Threading.Timer tmrExecutor ;           ///< the timer is coming every 30 seconds and send the messages to the server
      unsigned int            requests ;                    ///< how many updates/inserts are in strNonQueryStatements
#endif

        private const string prefixNextMsg = "Unable to retrieve next message. ";

        #region Public MsgIn Interfaces
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public MsgIn(SQLExecuter sqlExec)
            : base("vlfMsgIn", sqlExec)
        {
        }
        /// <summary>
        /// Add new Msg.
        /// </summary>
        /// <param name="cMFIn"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists (after number of ritries).</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AddMsg(CMFIn cMFIn)
        {
            int currRetries = 0;
            while (currRetries < Const.violatedIntegrityMaxRetries)
            {
                try
                {
                    AppendMsg(cMFIn);
                    currRetries = Const.violatedIntegrityMaxRetries;
                }
                catch (DASAppViolatedIntegrityConstraintsException e)
                {
                    ++currRetries;
                    if (currRetries < Const.violatedIntegrityMaxRetries)
                    {
                        cMFIn.receivedDateTime = (DateTime)cMFIn.receivedDateTime.AddMilliseconds(Const.nextDateTimeMillisecInterval);
                    }
                    else
                    {
                        string text = "Information with '" + (DateTime)cMFIn.receivedDateTime +
                                    "' already exists in MsgIn table. Unable to add new data into MsgIn table. Maximal number of retries "
                                    + currRetries + " has been reached. ";
                        throw new DASAppDataAlreadyExistsException(text + e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves record count of "vlfMsgIn" table
        /// </summary>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public Int64 RecordCount
        {
            get
            {
                return GetRecordCount();
            }
        }

        public bool DrawNextXCmfMsg2(int slsId, ref int cnt, ref CMFIn[] arrCMFIn)
        {
            // TODO: Implement box-ASL subscription
            // Retrieves next message
            // replace it with a store procedure to move data from vlfMsgIn and to not call DeleteMsg
            DataSet sqlDataSet = RetrievesNextMsg(slsId, cnt, prefixNextMsg);

            // Retrieves first row from SQL result.
            if (sqlDataSet != null && sqlDataSet.Tables[0].Rows.Count > 0)
            {
                cnt = sqlDataSet.Tables[0].Rows.Count;
                if (cnt > 8)
                    Util.BTrace(Util.INF0, "-- DrawNextXCmfMsg -> {0} MSGS", cnt);

                int i = 0;
                foreach (DataRow currRow in sqlDataSet.Tables[0].Rows)
                {
                    if (sqlDataSet.Tables[0].Columns.Count != Convert.ToInt32(MsgInTblStruct.MaxColumn))
                    {
                        throw new DASAppViolatedIntegrityConstraintsException(prefixNextMsg +
                           " Wrong number of parameters [" +
                           sqlDataSet.Tables[0].Columns.Count + "].");
                    }
                    /*                // this is solved by the  sp_GetTopMsgHs SP
                                      // 1. Deletes current record by DateTimeReceived and box id fields from MsgIn table.
                                      int res = DeleteMsg(Convert.ToInt64(currRow[(int)MsgInTblStruct.DateTimeReceived]),
                                                     Convert.ToInt32(currRow[(int)MsgInTblStruct.BoxId]));
                    */
                    // 2. Build result in CMF format.
                    //cMFInResult = new CMFIn();
                    arrCMFIn[i].blobSize = Convert.ToInt16(currRow[(int)MsgInTblStruct.BlobDataSize]);
                    if (arrCMFIn[i].blobSize > 0)
                    {
                        arrCMFIn[i].blobData = new byte[arrCMFIn[i].blobSize];
                        Array.Copy((byte[])currRow[(int)MsgInTblStruct.BlobData], arrCMFIn[i].blobData, arrCMFIn[i].blobSize);
                    }
                    else
                        arrCMFIn[i].blobData = null;

                    arrCMFIn[i].boxID = Convert.ToInt32(currRow[(int)MsgInTblStruct.BoxId]);
                    arrCMFIn[i].commInfo1 = Convert.ToString(currRow[(int)MsgInTblStruct.CommInfo1]).TrimEnd();
                    arrCMFIn[i].commInfo2 = Convert.ToString(currRow[(int)MsgInTblStruct.CommInfo2]).TrimEnd();
                    arrCMFIn[i].customProperties = Convert.ToString(currRow[(int)MsgInTblStruct.CustomProp]).TrimEnd();
                    arrCMFIn[i].dclID = Convert.ToInt16(currRow[(int)MsgInTblStruct.DclId]);
                    arrCMFIn[i].heading = Convert.ToInt16(currRow[(int)MsgInTblStruct.Heading]);
                    arrCMFIn[i].latitude = Convert.ToDouble(currRow[(int)MsgInTblStruct.Latitude]);
                    arrCMFIn[i].longitude = Convert.ToDouble(currRow[(int)MsgInTblStruct.Longitude]);
                    arrCMFIn[i].messageTypeID = Convert.ToInt16(currRow[(int)MsgInTblStruct.BoxMsgInTypeId]);
                    arrCMFIn[i].originatedDateTime = Convert.ToDateTime(currRow[(int)MsgInTblStruct.OriginDateTime]);
                    arrCMFIn[i].commMode = Convert.ToInt16(currRow[(int)MsgInTblStruct.CommModeId]);
                    arrCMFIn[i].protocolTypeID = Convert.ToInt16(currRow[(int)MsgInTblStruct.BoxProtocolTypeId]);
                    arrCMFIn[i].receivedDateTime = Convert.ToDateTime(currRow[(int)MsgInTblStruct.DateTimeReceived]);
                    arrCMFIn[i].sensorMask = Convert.ToInt64(currRow[(int)MsgInTblStruct.SensorMask]);
                    arrCMFIn[i].speed = Convert.ToInt16(currRow[(int)MsgInTblStruct.Speed]);
                    arrCMFIn[i].validGPS = Convert.ToSByte(currRow[(int)MsgInTblStruct.ValidGps]);
                    arrCMFIn[i].sequenceNum = Convert.ToInt32(currRow[(int)MsgInTblStruct.SequenceNum]);
                    arrCMFIn[i].isArmed = Convert.ToInt16(currRow[(int)MsgInTblStruct.IsArmed]);
                    arrCMFIn[i].priority = Convert.ToByte(currRow[(int)MsgInTblStruct.Priority]);


                    // 2. Try to backup exist message (Add new record to MsgInHst table without Blob data (only blob size))
                    // Const.violatedIntegrityMaxRetries times.
                    int currRetries = 0;
                    while (currRetries < Const.violatedIntegrityMaxRetries)
                    {
                        try
                        {
                            AddToHistory(arrCMFIn[i], "vlfMsgInHst");



                            // On success, exit
                            currRetries = Const.violatedIntegrityMaxRetries;
                        }
                        catch (DASAppViolatedIntegrityConstraintsException e)
                        {
                            // Retrieves same msg from MsgInHst table 
                            //		[DclId],[BoxMsgInTypeId],[BoxProtocolTypeId],
                            //		[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
                            //		[Speed],[Heading],[SensorMask],[CustomProp]
                            DataSet dsSameHstRec = GetMessageFromHistory(arrCMFIn[i].boxID,
                                                                         arrCMFIn[i].originatedDateTime);
                            // Compares two msgs (if they same,ignore it)
                            if (dsSameHstRec != null && dsSameHstRec.Tables.Count > 0 && dsSameHstRec.Tables[0].Rows.Count > 0)
                            {
                                if ((Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["BoxMsgInTypeId"]) == arrCMFIn[i].messageTypeID) &&
                                   (Convert.ToDouble(dsSameHstRec.Tables[0].Rows[0]["Latitude"]) == arrCMFIn[i].latitude) &&
                                   (Convert.ToDouble(dsSameHstRec.Tables[0].Rows[0]["Longitude"]) == arrCMFIn[i].longitude) &&
                                   (Convert.ToInt64(dsSameHstRec.Tables[0].Rows[0]["SensorMask"]) == arrCMFIn[i].sensorMask) &&
                                   (Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["Heading"]) == arrCMFIn[i].heading) &&
                                   (Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["ValidGps"]) == arrCMFIn[i].validGPS) &&
                                    //(Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["BoxProtocolTypeId"]) == arrCMFIn[i].protocolTypeID)&&
                                    //(Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["DclId"]) == arrCMFIn[i].dclID)&&
                                    //(dsSameHstRec.Tables[0].Rows[0]["CommInfo1"].ToString() == arrCMFIn[i].commInfo1.ToString())&&
                                    //(dsSameHstRec.Tables[0].Rows[0]["CommInfo2"].ToString() == arrCMFIn[i].commInfo2.ToString())&&
                                   (dsSameHstRec.Tables[0].Rows[0]["CustomProp"].ToString() == arrCMFIn[i].customProperties.ToString())
                                   )
                                {
                                    // add ignored msg to database
                                    AddToHistory(arrCMFIn[i], "vlfMsgInHstIgnored");
                                    // inform upper levels for error occurence
                                    // string text = "Duplicated message for box " + arrCMFIn[i].boxID + ". ";
                                    // throw new DASAppDataAlreadyExistsException(text + e.Message);
                                    // throw new DASAppDuplicatedMessageException(text + e.Message);
                                    Util.BTrace(Util.WARN1, "  MsgIn.DrawNextXCmfMsg -> Duplicated message (1) for boxId[{0}]", arrCMFIn[i].boxID);
                                    arrCMFIn[i].isDuplicatedMsg = true;
                                    break;

                                }
                            }
                            ++currRetries;
                            if (currRetries < Const.violatedIntegrityMaxRetries)
                            {
                                arrCMFIn[i].originatedDateTime = arrCMFIn[i].originatedDateTime.AddMilliseconds(Const.nextDateTimeMillisecInterval);
                            }
                            else
                            {

                                // add ignored msg to database
                                AddToHistory(arrCMFIn[i], "vlfMsgInHstIgnored");
                                // inform upper levels for error occurence
                                // string text = "Maximal number of retries(" + currRetries + ") has been exceeded for box " + arrCMFIn[i].boxID + ". ";
                                // throw new DASAppViolatedIntegrityConstraintsException(text + e.Message);
                                Util.BTrace(Util.WARN1, "  MsgIn.DrawNextXCmfMsg -> Duplicated message (2) for boxId[{0}]", arrCMFIn[i].boxID);
                                arrCMFIn[i].isDuplicatedMsg = true;
                                break;
                            }
                        }

                    } // end while (currRetries < Const.violatedIntegrityMaxRetries

                    // 4. Skip to the next row.
                    ++i;
                }
            }
            else cnt = 0;
            return true;

        }

        /** \fn       public bool DrawNextXCmfMsg(ref int cnt, ref CMFIn[] arrCMFIn)
         *  \comment  previously, after processing you deleted the message from vlfMsgIn
         *            moved it to history
         *  \comment  2009/04/22 (gb)
         *            it is much easier to move the whole CPU/IO intensive activity into a separate table/database 
         *            and just mark the records after they were processed by one stage
         *            DCL --> vlfMsgIn
         *                      |----> SLS(i) --> afterStep1 (ordered only by DateTime)
         *                                           |-----> dispatcher --> vlfMsgInHst
         *            the dispatcher collects first all messages not present in history and 
         *            put them in a separate table/buffer (vlfMsgInIgnored
         *            
         *            
         */
        public bool DrawNextXCmfMsg(ref int cnt, ref CMFInEx[] arrCMFIn)
        {
            // Retrieves next message
            // replace it with a store procedure to move data from vlfMsgIn and do not call DeleteMsg
            DataSet sqlDataSet = RetrievesNextMsg(cnt, prefixNextMsg);

            // Retrieves first row from SQL result.
            if (sqlDataSet != null && sqlDataSet.Tables[0].Rows.Count > 0)
            {
                cnt = sqlDataSet.Tables[0].Rows.Count;
                if (cnt > 8)
                    Util.BTrace(Util.INF0, "-- DrawNextXCmfMsg -> {0} MSGS", cnt);

                int i = 0;
                foreach (DataRow currRow in sqlDataSet.Tables[0].Rows)
                {
                    if (sqlDataSet.Tables[0].Columns.Count != Convert.ToInt32(MsgInTblStruct.MaxColumn))
                    {
                        throw new DASAppViolatedIntegrityConstraintsException(prefixNextMsg +
                           " Wrong number of parameters [" +
                           sqlDataSet.Tables[0].Columns.Count + "].");
                    }
                    /*                // this is solved by the  sp_GetTopMsgHs SP
                                      // 1. Deletes current record by DateTimeReceived and box id fields from MsgIn table.
                                      int res = DeleteMsg(Convert.ToInt64(currRow[(int)MsgInTblStruct.DateTimeReceived]),
                                                     Convert.ToInt32(currRow[(int)MsgInTblStruct.BoxId]));
                    */
                    // 2. Build result in CMF format.
                    //cMFInResult = new CMFIn();
                    arrCMFIn[i].blobSize = Convert.ToInt16(currRow[(int)MsgInTblStruct.BlobDataSize]);
                    if (arrCMFIn[i].blobSize > 0)
                    {
                        arrCMFIn[i].blobData = new byte[arrCMFIn[i].blobSize];
                        Array.Copy((byte[])currRow[(int)MsgInTblStruct.BlobData], arrCMFIn[i].blobData, arrCMFIn[i].blobSize);
                    }
                    else
                        arrCMFIn[i].blobData = null;

                    arrCMFIn[i].boxID = Convert.ToInt32(currRow[(int)MsgInTblStruct.BoxId]);
                    arrCMFIn[i].commInfo1 = Convert.ToString(currRow[(int)MsgInTblStruct.CommInfo1]).TrimEnd();
                    arrCMFIn[i].commInfo2 = Convert.ToString(currRow[(int)MsgInTblStruct.CommInfo2]).TrimEnd();
                    arrCMFIn[i].customProperties = Convert.ToString(currRow[(int)MsgInTblStruct.CustomProp]).TrimEnd();
                    arrCMFIn[i].dclID = Convert.ToInt16(currRow[(int)MsgInTblStruct.DclId]);
                    arrCMFIn[i].heading = Convert.ToInt16(currRow[(int)MsgInTblStruct.Heading]);
                    arrCMFIn[i].latitude = Convert.ToDouble(currRow[(int)MsgInTblStruct.Latitude]);
                    arrCMFIn[i].longitude = Convert.ToDouble(currRow[(int)MsgInTblStruct.Longitude]);
                    arrCMFIn[i].messageTypeID = Convert.ToInt16(currRow[(int)MsgInTblStruct.BoxMsgInTypeId]);
                    arrCMFIn[i].realOriginDateTime = arrCMFIn[i].originatedDateTime = Convert.ToDateTime(currRow[(int)MsgInTblStruct.OriginDateTime]);
                    arrCMFIn[i].commMode = Convert.ToInt16(currRow[(int)MsgInTblStruct.CommModeId]);
                    arrCMFIn[i].protocolTypeID = Convert.ToInt16(currRow[(int)MsgInTblStruct.BoxProtocolTypeId]);
                    arrCMFIn[i].receivedDateTime = Convert.ToDateTime(currRow[(int)MsgInTblStruct.DateTimeReceived]);
                    arrCMFIn[i].sensorMask = Convert.ToInt64(currRow[(int)MsgInTblStruct.SensorMask]);
                    arrCMFIn[i].speed = Convert.ToInt16(currRow[(int)MsgInTblStruct.Speed]);
                    arrCMFIn[i].validGPS = Convert.ToSByte(currRow[(int)MsgInTblStruct.ValidGps]);
                    arrCMFIn[i].sequenceNum = Convert.ToInt32(currRow[(int)MsgInTblStruct.SequenceNum]);
                    arrCMFIn[i].isArmed = Convert.ToInt16(currRow[(int)MsgInTblStruct.IsArmed]);
                    arrCMFIn[i].priority = Convert.ToByte(currRow[(int)MsgInTblStruct.Priority]);
                    if (arrCMFIn[i].priority == CMFIn.DUPLICATE)
                    {
                        arrCMFIn[i].isDuplicatedMsg = true;
                        continue;
                    }

                    // 2. Try to backup exist message (Add new record to MsgInHst table without Blob data (only blob size))
                    // Const.violatedIntegrityMaxRetries times.
                    int currRetries = 0;
                    while (currRetries < Const.violatedIntegrityMaxRetries)
                    {
                        try
                        {
                            // this operation is very expensive because you open/close the connection for every 
                            // message instead of using two separate commands for insertion in both table (hist/histIgnored)

                            AddToHistory(arrCMFIn[i], "vlfMsgInHst");


                            // AddToHistorySLS(arrCMFIn[i]); 

                            // On success, exit
                            currRetries = Const.violatedIntegrityMaxRetries;
                        }
                        catch (DASAppViolatedIntegrityConstraintsException e)
                        {
                            // Retrieves same msg from MsgInHst table 
                            //		[DclId],[BoxMsgInTypeId],[BoxProtocolTypeId],
                            //		[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
                            //		[Speed],[Heading],[SensorMask],[CustomProp]
                            DataSet dsSameHstRec = GetMessageFromHistory(arrCMFIn[i].boxID,
                                                                         arrCMFIn[i].originatedDateTime);
                            // Compares two msgs (if they are the same,ignore it)
                            if (dsSameHstRec != null &&
                                dsSameHstRec.Tables.Count > 0 &&
                                dsSameHstRec.Tables[0].Rows.Count > 0)
                            {
                                if ((Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["BoxMsgInTypeId"]) == arrCMFIn[i].messageTypeID) &&
                                   (Convert.ToDouble(dsSameHstRec.Tables[0].Rows[0]["Latitude"]) == arrCMFIn[i].latitude) &&
                                   (Convert.ToDouble(dsSameHstRec.Tables[0].Rows[0]["Longitude"]) == arrCMFIn[i].longitude) &&
                                   (Convert.ToInt64(dsSameHstRec.Tables[0].Rows[0]["SensorMask"]) == arrCMFIn[i].sensorMask) &&
                                   (Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["Heading"]) == arrCMFIn[i].heading) &&
                                   (Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["ValidGps"]) == arrCMFIn[i].validGPS) &&
                                    //(Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["BoxProtocolTypeId"]) == arrCMFIn[i].protocolTypeID)&&
                                    //(Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["DclId"]) == arrCMFIn[i].dclID)&&
                                    //(dsSameHstRec.Tables[0].Rows[0]["CommInfo1"].ToString() == arrCMFIn[i].commInfo1.ToString())&&
                                    //(dsSameHstRec.Tables[0].Rows[0]["CommInfo2"].ToString() == arrCMFIn[i].commInfo2.ToString())&&
                                   (dsSameHstRec.Tables[0].Rows[0]["CustomProp"].ToString() == arrCMFIn[i].customProperties.ToString())
                                   )
                                {
                                    // add ignored msg to database
                                    AddToHistory(arrCMFIn[i], "vlfMsgInHstIgnored");
                                    // inform upper levels for error occurence
                                    // string text = "Duplicated message for box " + arrCMFIn[i].boxID + ". ";
                                    // throw new DASAppDataAlreadyExistsException(text + e.Message);
                                    // throw new DASAppDuplicatedMessageException(text + e.Message);
                                    Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsg -> Duplicate message boxId[{0}] OriginDT[{1}]",
                                                   arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"));
                                    arrCMFIn[i].isDuplicatedMsg = true;
                                    break;

                                }
                            }
                            ++currRetries;
                            if (currRetries < Const.violatedIntegrityMaxRetries)
                            {
                                // here you have to compare the datetime of the received date/time
                                // and modify accordingly                           
                                if (arrCMFIn[i].receivedDateTime >
                                     Convert.ToDateTime(dsSameHstRec.Tables[0].Rows[0]["DateTimeReceived"]))
                                {
                                    Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsg -> +10MSECS for boxId[{0}] OriginDT[{1} retries={2}]",
                                                   arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"), currRetries);
                                    arrCMFIn[i].originatedDateTime = arrCMFIn[i].originatedDateTime.AddMilliseconds(Const.nextDateTimeMillisecInterval);
                                }
                                else
                                {
                                    Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsg -> -10MSECS for boxId[{0}] OriginDT[{1} retries={2}]",
                                                 arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"), currRetries);
                                    arrCMFIn[i].originatedDateTime = arrCMFIn[i].originatedDateTime.AddMilliseconds(-Const.nextDateTimeMillisecInterval);
                                }
                            }
                            else
                            {

                                // add ignored msg to database
                                AddToHistory(arrCMFIn[i], "vlfMsgInHstIgnored");
                                // inform upper levels for error occurence
                                // string text = "Maximal number of retries(" + currRetries + ") has been exceeded for box " + arrCMFIn[i].boxID + ". ";
                                // throw new DASAppViolatedIntegrityConstraintsException(text + e.Message);
                                Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsg -> Duplicate message (MAX) boxId[{0}] OriginDT[{1}]",
                                            arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime);
                                arrCMFIn[i].isDuplicatedMsg = true;
                                break;
                            }
                        }

                    } // end while (currRetries < Const.violatedIntegrityMaxRetries

                    // 4. Skip to the next row.
                    ++i;
                }
            }
            else cnt = 0;
            return true;
        }

        public bool[] DrawNextXCmfMsgQueued(ref int cnt, ref CMFInEx[] arrCMFIn)
        {
            // Retrieves next message
            // replace it with a store procedure to move data from vlfMsgIn and do not call DeleteMsg
            DataSet sqlDataSet = RetrievesNextMsg(cnt, prefixNextMsg);
            bool[] arrayMsg = new bool[arrCMFIn.Length];

            // Retrieves first row from SQL result.
            if (sqlDataSet != null && sqlDataSet.Tables[0].Rows.Count > 0)
            {
                cnt = sqlDataSet.Tables[0].Rows.Count;
                if (cnt > 8)
                    Util.BTrace(Util.INF0, "-- DrawNextXCmfMsgQueued -> {0} MSGS", cnt);

                int i = 0;


                foreach (DataRow currRow in sqlDataSet.Tables[0].Rows)
                {
                    if (sqlDataSet.Tables[0].Columns.Count != Convert.ToInt32(MsgInTblStruct.MaxColumn))
                    {
                        throw new DASAppViolatedIntegrityConstraintsException(prefixNextMsg +
                           " Wrong number of parameters [" +
                           sqlDataSet.Tables[0].Columns.Count + "].");
                    }
                    /*                // this is solved by the  sp_GetTopMsgHs SP
                                      // 1. Deletes current record by DateTimeReceived and box id fields from MsgIn table.
                                      int res = DeleteMsg(Convert.ToInt64(currRow[(int)MsgInTblStruct.DateTimeReceived]),
                                                     Convert.ToInt32(currRow[(int)MsgInTblStruct.BoxId]));
                    */
                    // 2. Build result in CMF format.
                    //cMFInResult = new CMFIn();
                    arrCMFIn[i].blobSize = Convert.ToInt16(currRow[(int)MsgInTblStruct.BlobDataSize]);
                    if (arrCMFIn[i].blobSize > 0)
                    {
                        arrCMFIn[i].blobData = new byte[arrCMFIn[i].blobSize];
                        Array.Copy((byte[])currRow[(int)MsgInTblStruct.BlobData], arrCMFIn[i].blobData, arrCMFIn[i].blobSize);
                    }
                    else
                        arrCMFIn[i].blobData = null;

                    //arrCMFIn[i].ID = Convert.ToInt64(currRow["ID"]);

                    arrCMFIn[i].boxID = Convert.ToInt32(currRow[(int)MsgInTblStruct.BoxId]);
                    arrCMFIn[i].commInfo1 = Convert.ToString(currRow[(int)MsgInTblStruct.CommInfo1]).TrimEnd();
                    arrCMFIn[i].commInfo2 = Convert.ToString(currRow[(int)MsgInTblStruct.CommInfo2]).TrimEnd();
                    arrCMFIn[i].customProperties = Convert.ToString(currRow[(int)MsgInTblStruct.CustomProp]).TrimEnd();
                    arrCMFIn[i].dclID = Convert.ToInt16(currRow[(int)MsgInTblStruct.DclId]);
                    arrCMFIn[i].heading = Convert.ToInt16(currRow[(int)MsgInTblStruct.Heading]);
                    arrCMFIn[i].latitude = Convert.ToDouble(currRow[(int)MsgInTblStruct.Latitude]);
                    arrCMFIn[i].longitude = Convert.ToDouble(currRow[(int)MsgInTblStruct.Longitude]);
                    arrCMFIn[i].messageTypeID = Convert.ToInt16(currRow[(int)MsgInTblStruct.BoxMsgInTypeId]);
                    arrCMFIn[i].realOriginDateTime = arrCMFIn[i].originatedDateTime = Convert.ToDateTime(currRow[(int)MsgInTblStruct.OriginDateTime]);
                    arrCMFIn[i].commMode = Convert.ToInt16(currRow[(int)MsgInTblStruct.CommModeId]);
                    arrCMFIn[i].protocolTypeID = Convert.ToInt16(currRow[(int)MsgInTblStruct.BoxProtocolTypeId]);
                    arrCMFIn[i].receivedDateTime = Convert.ToDateTime(currRow[(int)MsgInTblStruct.DateTimeReceived]);
                    arrCMFIn[i].sensorMask = Convert.ToInt64(currRow[(int)MsgInTblStruct.SensorMask]);
                    arrCMFIn[i].speed = Convert.ToInt16(currRow[(int)MsgInTblStruct.Speed]);
                    arrCMFIn[i].validGPS = Convert.ToSByte(currRow[(int)MsgInTblStruct.ValidGps]);
                    arrCMFIn[i].sequenceNum = Convert.ToInt32(currRow[(int)MsgInTblStruct.SequenceNum]);
                    arrCMFIn[i].isArmed = Convert.ToInt16(currRow[(int)MsgInTblStruct.IsArmed]);
                    arrCMFIn[i].priority = Convert.ToByte(currRow[(int)MsgInTblStruct.Priority]);
                    if (arrCMFIn[i].priority == CMFIn.DUPLICATE)
                    {
                        arrCMFIn[i].isDuplicatedMsg = true;
                        arrayMsg[i] = false;
                        continue;
                    }

                    // 2. Try to backup exist message (Add new record to MsgInHst table without Blob data (only blob size))
                    // Const.violatedIntegrityMaxRetries times.
                    int currRetries = 0;
                    while (currRetries < Const.violatedIntegrityMaxRetries)
                    {
                        try
                        {
                            // this operation is very expensive because you open/close the connection for every 
                            // message instead of using two separate commands for insertion in both table (hist/histIgnored)

                            //arrCMFIn[i].ID = AddToHistoryTable(arrCMFIn[i], CLS.Def.Enums.HistoryTables.vlfMsgInHst.ToString());
                            //arrCMFIn[i].ID = AddToHistoryQueue(arrCMFIn[i]);

                            arrCMFIn[i].ID = AddToHistoryQueue(arrCMFIn[i]);

                            // AddToHistorySLS(arrCMFIn[i]); 

                            // On success, exit
                            currRetries = Const.violatedIntegrityMaxRetries;
                            arrayMsg[i] = arrCMFIn[i].ID > 0; //== 1;
                        }
                        catch (DASAppViolatedIntegrityConstraintsException e)
                        {
                            // Retrieves same msg from MsgInHst table 
                            //		[DclId],[BoxMsgInTypeId],[BoxProtocolTypeId],
                            //		[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
                            //		[Speed],[Heading],[SensorMask],[CustomProp]
                            DataSet dsSameHstRec = GetMessageFromHistoryQueue(arrCMFIn[i].boxID,
                                                                         arrCMFIn[i].originatedDateTime);
                            // Compares two msgs (if they are the same,ignore it)
                            if (dsSameHstRec != null &&
                                dsSameHstRec.Tables.Count > 0 &&
                                dsSameHstRec.Tables[0].Rows.Count > 0)
                            {
                                if ((Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["BoxMsgInTypeId"]) == arrCMFIn[i].messageTypeID) &&
                                   (Convert.ToDouble(dsSameHstRec.Tables[0].Rows[0]["Latitude"]) == arrCMFIn[i].latitude) &&
                                   (Convert.ToDouble(dsSameHstRec.Tables[0].Rows[0]["Longitude"]) == arrCMFIn[i].longitude) &&
                                   (Convert.ToInt64(dsSameHstRec.Tables[0].Rows[0]["SensorMask"]) == arrCMFIn[i].sensorMask) &&
                                   (Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["Heading"]) == arrCMFIn[i].heading) &&
                                   (Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["ValidGps"]) == arrCMFIn[i].validGPS) &&
                                    //(Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["BoxProtocolTypeId"]) == arrCMFIn[i].protocolTypeID)&&
                                    //(Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["DclId"]) == arrCMFIn[i].dclID)&&
                                    //(dsSameHstRec.Tables[0].Rows[0]["CommInfo1"].ToString() == arrCMFIn[i].commInfo1.ToString())&&
                                    //(dsSameHstRec.Tables[0].Rows[0]["CommInfo2"].ToString() == arrCMFIn[i].commInfo2.ToString())&&
                                   (dsSameHstRec.Tables[0].Rows[0]["CustomProp"].ToString() == arrCMFIn[i].customProperties.ToString())
                                   )
                                {
                                    // add ignored msg to database
                                    AddToHistory(arrCMFIn[i], "vlfMsgInHstIgnored");
                                    // inform upper levels for error occurence
                                    // string text = "Duplicated message for box " + arrCMFIn[i].boxID + ". ";
                                    // throw new DASAppDataAlreadyExistsException(text + e.Message);
                                    // throw new DASAppDuplicatedMessageException(text + e.Message);
                                    Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsgQueued -> Duplicate message boxId[{0}] OriginDT[{1}]",
                                                   arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"));
                                    arrCMFIn[i].isDuplicatedMsg = true;
                                    arrayMsg[i] = false;
                                    break;

                                }
                            }
                            ++currRetries;
                            if (currRetries < Const.violatedIntegrityMaxRetries)
                            {
                                // here you have to compare the datetime of the received date/time
                                // and modify accordingly                           
                                if (arrCMFIn[i].receivedDateTime >
                                     Convert.ToDateTime(dsSameHstRec.Tables[0].Rows[0]["DateTimeReceived"]))
                                {
                                    Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsgQueued -> +10MSECS for boxId[{0}] OriginDT[{1} retries={2}]",
                                                   arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"), currRetries);
                                    arrCMFIn[i].originatedDateTime = arrCMFIn[i].originatedDateTime.AddMilliseconds(Const.nextDateTimeMillisecInterval);
                                }
                                else
                                {
                                    Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsgQueued -> -10MSECS for boxId[{0}] OriginDT[{1} retries={2}]",
                                                 arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"), currRetries);
                                    arrCMFIn[i].originatedDateTime = arrCMFIn[i].originatedDateTime.AddMilliseconds(-Const.nextDateTimeMillisecInterval);
                                }
                            }
                            else
                            {

                                // add ignored msg to database
                                AddToHistory(arrCMFIn[i], "vlfMsgInHstIgnored");
                                // inform upper levels for error occurence
                                // string text = "Maximal number of retries(" + currRetries + ") has been exceeded for box " + arrCMFIn[i].boxID + ". ";
                                // throw new DASAppViolatedIntegrityConstraintsException(text + e.Message);
                                Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsgQueued -> Duplicate message (MAX) boxId[{0}] OriginDT[{1}]",
                                            arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime);
                                arrCMFIn[i].isDuplicatedMsg = true;
                                arrayMsg[i] = false;
                                break;
                            }
                        }

                    } // end while (currRetries < Const.violatedIntegrityMaxRetries

                    // 4. Skip to the next row.
                    ++i;

                }
            }
            else cnt = 0;

            return arrayMsg;

        }


        public DataSet vfMsgInHstQueued_GetNotProcessed(CLS.Def.Enums.HistoryTables tableId)
        {
            DataSet dsResult = null;
            string errorMsg = "Cannot retrieve messages from queue (vfMsgInHstQueued_GetNotProcessed)";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@Table", Convert.ToInt16(tableId));


                sqlExec.CommandTimeout = 600;
                dsResult = sqlExec.SPExecuteDataset("vlfMsgInHstQueued_GetNotProcessed", sqlParams);

            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(errorMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(String.Format("{0}:\n{1}", errorMsg, objException.Message));
            }

            return dsResult;
        }

        public DataSet vfMsgInHstQueued_GetNotProcessed(long lastId, int maxRecords, CLS.Def.Enums.HistoryTables tableId)
        {
            DataSet dsResult = null;
            string errorMsg = "Cannot retrieve messages from queue (vfMsgInHstQueued_GetNotProcessed)";
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@Table", Convert.ToInt16(tableId));
                sqlParams[1] = new SqlParameter("@ID", lastId);
                sqlParams[2] = new SqlParameter("@maxRecords", maxRecords);


                sqlExec.CommandTimeout = 600;
                dsResult = sqlExec.SPExecuteDataset("vlfMsgInHstQueued_GetNotProcessed", sqlParams);

            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(errorMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(String.Format("{0}:\n{1}", errorMsg, objException.Message));
            }

            return dsResult;
        }




        /** \fn       public bool DrawNextXCmfMsg(ref int cnt, ref CMFIn[] arrCMFIn)
       *  \comment  previously, after processing you deleted the message from vlfMsgIn
       *            moved it to history
       *  \comment  2009/04/22 (gb)
       *            it is much easier to move the whole CPU/IO intensive activity into a separate table/database 
       *            and just mark the records after they were processed by one stage
       *            DCL --> vlfMsgIn
       *                      |----> SLS(i) --> afterStep1 (ordered only by DateTime)
       *                                           |-----> dispatcher --> vlfMsgInHst
       *            the dispatcher collects first all messages not present in history and 
       *            put them in a separate table/buffer (vlfMsgInIgnored
       *            
       *            
       */
        public bool[] DrawNextXCmfMsg_SLSProcessIn(ref int cnt, ref CMFInEx[] arrCMFIn)
        {
            int count = 0;
            DateTime oTime = new DateTime();
            // Retrieves next message
            // replace it with a store procedure to move data from vlfMsgIn and do not call DeleteMsg
            DataSet sqlDataSet = RetrievesNextMsg_SLSProcessIn(cnt, prefixNextMsg);
            bool[] arrayMsg = new bool[arrCMFIn.Length];

            // Retrieves first row from SQL result.
            if (sqlDataSet != null && sqlDataSet.Tables[0].Rows.Count > 0)
            {
                cnt = sqlDataSet.Tables[0].Rows.Count;
                if (cnt > 8)
                    Util.BTrace(Util.INF0, "-- DrawNextXCmfMsg_SLSProcessIn -> {0} MSGS", cnt);

                int i = 0;
                foreach (DataRow currRow in sqlDataSet.Tables[0].Rows)
                {
                    if (sqlDataSet.Tables[0].Columns.Count != Convert.ToInt32(MsgInTblStruct.MaxColumn))
                    {
                        throw new DASAppViolatedIntegrityConstraintsException(prefixNextMsg +
                           " Wrong number of parameters [" +
                           sqlDataSet.Tables[0].Columns.Count + "].");
                    }
                    /*                // this is solved by the  sp_GetTopMsgHs SP
                                      // 1. Deletes current record by DateTimeReceived and box id fields from MsgIn table.
                                      int res = DeleteMsg(Convert.ToInt64(currRow[(int)MsgInTblStruct.DateTimeReceived]),
                                                     Convert.ToInt32(currRow[(int)MsgInTblStruct.BoxId]));
                    */
                    // 2. Build result in CMF format.
                    //cMFInResult = new CMFIn();
                    arrCMFIn[i].blobSize = Convert.ToInt16(currRow[(int)MsgInTblStruct.BlobDataSize]);
                    if (arrCMFIn[i].blobSize > 0)
                    {
                        arrCMFIn[i].blobData = new byte[arrCMFIn[i].blobSize];
                        Array.Copy((byte[])currRow[(int)MsgInTblStruct.BlobData], arrCMFIn[i].blobData, arrCMFIn[i].blobSize);
                    }
                    else
                        arrCMFIn[i].blobData = null;

                    arrCMFIn[i].boxID = Convert.ToInt32(currRow[(int)MsgInTblStruct.BoxId]);
                    arrCMFIn[i].commInfo1 = Convert.ToString(currRow[(int)MsgInTblStruct.CommInfo1]).TrimEnd();
                    arrCMFIn[i].commInfo2 = Convert.ToString(currRow[(int)MsgInTblStruct.CommInfo2]).TrimEnd();
                    arrCMFIn[i].customProperties = Convert.ToString(currRow[(int)MsgInTblStruct.CustomProp]).TrimEnd();
                    arrCMFIn[i].dclID = Convert.ToInt16(currRow[(int)MsgInTblStruct.DclId]);
                    arrCMFIn[i].heading = Convert.ToInt16(currRow[(int)MsgInTblStruct.Heading]);
                    arrCMFIn[i].latitude = Convert.ToDouble(currRow[(int)MsgInTblStruct.Latitude]);
                    arrCMFIn[i].longitude = Convert.ToDouble(currRow[(int)MsgInTblStruct.Longitude]);
                    arrCMFIn[i].messageTypeID = Convert.ToInt16(currRow[(int)MsgInTblStruct.BoxMsgInTypeId]);
                    arrCMFIn[i].realOriginDateTime = arrCMFIn[i].originatedDateTime = Convert.ToDateTime(currRow[(int)MsgInTblStruct.OriginDateTime]);
                    arrCMFIn[i].commMode = Convert.ToInt16(currRow[(int)MsgInTblStruct.CommModeId]);
                    arrCMFIn[i].protocolTypeID = Convert.ToInt16(currRow[(int)MsgInTblStruct.BoxProtocolTypeId]);
                    arrCMFIn[i].receivedDateTime = Convert.ToDateTime(currRow[(int)MsgInTblStruct.DateTimeReceived]);
                    arrCMFIn[i].sensorMask = Convert.ToInt64(currRow[(int)MsgInTblStruct.SensorMask]);
                    arrCMFIn[i].speed = Convert.ToInt16(currRow[(int)MsgInTblStruct.Speed]);
                    arrCMFIn[i].validGPS = Convert.ToSByte(currRow[(int)MsgInTblStruct.ValidGps]);
                    arrCMFIn[i].sequenceNum = Convert.ToInt32(currRow[(int)MsgInTblStruct.SequenceNum]);
                    arrCMFIn[i].isArmed = Convert.ToInt16(currRow[(int)MsgInTblStruct.IsArmed]);
                    arrCMFIn[i].priority = Convert.ToByte(currRow[(int)MsgInTblStruct.Priority]);
                    if (arrCMFIn[i].priority == CMFIn.DUPLICATE)
                    {
                        arrCMFIn[i].isDuplicatedMsg = true;
                        continue;
                    }

                    if (oTime != arrCMFIn[i].originatedDateTime)
                        count = 1;
                    else
                        count++;

                    // 2. Try to backup exist message (Add new record to MsgInHst table without Blob data (only blob size))
                    // Const.violatedIntegrityMaxRetries times.
                    int currRetries = 0;
                    while (currRetries < Const.violatedIntegrityMaxRetries)
                    {
                        try
                        {
                            // this operation is very expensive because you open/close the connection for every 
                            // message instead of using two separate commands for insertion in both table (hist/histIgnored)

                            //AddToHistory(arrCMFIn[i], "vlfMsgInHst");




                            //// On success, exit
                            //currRetries = Const.violatedIntegrityMaxRetries;



                            arrCMFIn[i].ID = AddToHistoryQueue(arrCMFIn[i]);


                            // AddToHistorySLS(arrCMFIn[i]); 

                            // On success, exit
                            currRetries = Const.violatedIntegrityMaxRetries;
                            arrayMsg[i] = arrCMFIn[i].ID > 0; //== 1;


                        }
                        catch (DASAppViolatedIntegrityConstraintsException e)
                        {
                            // Retrieves same msg from MsgInHst table 
                            //		[DclId],[BoxMsgInTypeId],[BoxProtocolTypeId],
                            //		[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
                            //		[Speed],[Heading],[SensorMask],[CustomProp]
                            DataSet dsSameHstRec = GetMessageFromHistory(arrCMFIn[i].boxID,
                                                                         arrCMFIn[i].originatedDateTime);
                            // Compares two msgs (if they are the same,ignore it)
                            if (dsSameHstRec != null &&
                                dsSameHstRec.Tables.Count > 0 &&
                                dsSameHstRec.Tables[0].Rows.Count > 0)
                            {
                                if ((Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["BoxMsgInTypeId"]) == arrCMFIn[i].messageTypeID) &&
                                   (Convert.ToDouble(dsSameHstRec.Tables[0].Rows[0]["Latitude"]) == arrCMFIn[i].latitude) &&
                                   (Convert.ToDouble(dsSameHstRec.Tables[0].Rows[0]["Longitude"]) == arrCMFIn[i].longitude) &&
                                   (Convert.ToInt64(dsSameHstRec.Tables[0].Rows[0]["SensorMask"]) == arrCMFIn[i].sensorMask) &&
                                   (Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["Heading"]) == arrCMFIn[i].heading) &&
                                   (Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["ValidGps"]) == arrCMFIn[i].validGPS) &&
                                    //(Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["BoxProtocolTypeId"]) == arrCMFIn[i].protocolTypeID)&&
                                    //(Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["DclId"]) == arrCMFIn[i].dclID)&&
                                    //(dsSameHstRec.Tables[0].Rows[0]["CommInfo1"].ToString() == arrCMFIn[i].commInfo1.ToString())&&
                                    //(dsSameHstRec.Tables[0].Rows[0]["CommInfo2"].ToString() == arrCMFIn[i].commInfo2.ToString())&&
                                   (dsSameHstRec.Tables[0].Rows[0]["CustomProp"].ToString() == arrCMFIn[i].customProperties.ToString())
                                   )
                                {
                                    // add ignored msg to database
                                    AddToHistory(arrCMFIn[i], "vlfMsgInHstIgnored");
                                    // inform upper levels for error occurence
                                    // string text = "Duplicated message for box " + arrCMFIn[i].boxID + ". ";
                                    // throw new DASAppDataAlreadyExistsException(text + e.Message);
                                    // throw new DASAppDuplicatedMessageException(text + e.Message);
                                    Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsg_SLSProcessIn -> Duplicate message boxId[{0}] OriginDT[{1}]",
                                                   arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"));
                                    arrCMFIn[i].isDuplicatedMsg = true;
                                    break;

                                }
                            }
                            ++currRetries;
                            if (currRetries < Const.violatedIntegrityMaxRetries)
                            {
                                // here you have to compare the datetime of the received date/time
                                // and modify accordingly                           
                                if (arrCMFIn[i].receivedDateTime >
                                     Convert.ToDateTime(dsSameHstRec.Tables[0].Rows[0]["DateTimeReceived"]))
                                {
                                    Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsg_SLSProcessIn -> +10MSECS for boxId[{0}] OriginDT[{1} retries={2}]",
                                                   arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"), currRetries);
                                    arrCMFIn[i].originatedDateTime = arrCMFIn[i].originatedDateTime.AddMilliseconds(Const.nextDateTimeMillisecInterval); // * count);
                                }
                                else
                                {
                                    Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsg_SLSProcessIn -> -10MSECS for boxId[{0}] OriginDT[{1} retries={2}]",
                                                 arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"), currRetries);
                                    arrCMFIn[i].originatedDateTime = arrCMFIn[i].originatedDateTime.AddMilliseconds(-Const.nextDateTimeMillisecInterval);
                                }
                            }
                            else
                            {

                                // add ignored msg to database
                                AddToHistory(arrCMFIn[i], "vlfMsgInHstIgnored");
                                // inform upper levels for error occurence
                                // string text = "Maximal number of retries(" + currRetries + ") has been exceeded for box " + arrCMFIn[i].boxID + ". ";
                                // throw new DASAppViolatedIntegrityConstraintsException(text + e.Message);
                                Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsg_SLSProcessIn -> Duplicate message (MAX) boxId[{0}] OriginDT[{1}]",
                                            arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime);
                                arrCMFIn[i].isDuplicatedMsg = true;
                                break;
                            }
                        }

                    } // end while (currRetries < Const.violatedIntegrityMaxRetries

                    // 4. Skip to the next row.
                    ++i;
                }
            }
            else cnt = 0;
            return arrayMsg;
        }



        /** \fn       public bool DrawNextXCmfMsg(ref int cnt, ref CMFIn[] arrCMFIn)
   *  \comment  previously, after processing you deleted the message from vlfMsgIn
   *            moved it to history
   *  \comment  2009/04/22 (gb)
   *            it is much easier to move the whole CPU/IO intensive activity into a separate table/database 
   *            and just mark the records after they were processed by one stage
   *            DCL --> vlfMsgIn
   *                      |----> SLS(i) --> afterStep1 (ordered only by DateTime)
   *                                           |-----> dispatcher --> vlfMsgInHst
   *            the dispatcher collects first all messages not present in history and 
   *            put them in a separate table/buffer (vlfMsgInIgnored
   *            
   *            
   */
        public bool DrawNextXCmfMsg_SLSTmp(ref int cnt, ref CMFInEx[] arrCMFIn)
        {
            // Retrieves next message
            // replace it with a store procedure to move data from vlfMsgIn and do not call DeleteMsg
            DataSet sqlDataSet = RetrievesNextMsg(cnt, prefixNextMsg);

            // Retrieves first row from SQL result.
            if (sqlDataSet != null && sqlDataSet.Tables[0].Rows.Count > 0)
            {
                cnt = sqlDataSet.Tables[0].Rows.Count;
                if (cnt > 8)
                    Util.BTrace(Util.INF0, "-- DrawNextXCmfMsg_SLSTmp -> {0} MSGS", cnt);

                int i = 0;
                foreach (DataRow currRow in sqlDataSet.Tables[0].Rows)
                {
                    if (sqlDataSet.Tables[0].Columns.Count != Convert.ToInt32(MsgInTblStruct.MaxColumn))
                    {
                        throw new DASAppViolatedIntegrityConstraintsException(prefixNextMsg +
                           " Wrong number of parameters [" +
                           sqlDataSet.Tables[0].Columns.Count + "].");
                    }
                    /*                // this is solved by the  sp_GetTopMsgHs SP
                                      // 1. Deletes current record by DateTimeReceived and box id fields from MsgIn table.
                                      int res = DeleteMsg(Convert.ToInt64(currRow[(int)MsgInTblStruct.DateTimeReceived]),
                                                     Convert.ToInt32(currRow[(int)MsgInTblStruct.BoxId]));
                    */
                    // 2. Build result in CMF format.
                    //cMFInResult = new CMFIn();
                    arrCMFIn[i].blobSize = Convert.ToInt16(currRow[(int)MsgInTblStruct.BlobDataSize]);
                    if (arrCMFIn[i].blobSize > 0)
                    {
                        arrCMFIn[i].blobData = new byte[arrCMFIn[i].blobSize];
                        Array.Copy((byte[])currRow[(int)MsgInTblStruct.BlobData], arrCMFIn[i].blobData, arrCMFIn[i].blobSize);
                    }
                    else
                        arrCMFIn[i].blobData = null;

                    arrCMFIn[i].boxID = Convert.ToInt32(currRow[(int)MsgInTblStruct.BoxId]);
                    arrCMFIn[i].commInfo1 = Convert.ToString(currRow[(int)MsgInTblStruct.CommInfo1]).TrimEnd();
                    arrCMFIn[i].commInfo2 = Convert.ToString(currRow[(int)MsgInTblStruct.CommInfo2]).TrimEnd();
                    arrCMFIn[i].customProperties = Convert.ToString(currRow[(int)MsgInTblStruct.CustomProp]).TrimEnd();
                    arrCMFIn[i].dclID = Convert.ToInt16(currRow[(int)MsgInTblStruct.DclId]);
                    arrCMFIn[i].heading = Convert.ToInt16(currRow[(int)MsgInTblStruct.Heading]);
                    arrCMFIn[i].latitude = Convert.ToDouble(currRow[(int)MsgInTblStruct.Latitude]);
                    arrCMFIn[i].longitude = Convert.ToDouble(currRow[(int)MsgInTblStruct.Longitude]);
                    arrCMFIn[i].messageTypeID = Convert.ToInt16(currRow[(int)MsgInTblStruct.BoxMsgInTypeId]);
                    arrCMFIn[i].realOriginDateTime = arrCMFIn[i].originatedDateTime = Convert.ToDateTime(currRow[(int)MsgInTblStruct.OriginDateTime]);
                    arrCMFIn[i].commMode = Convert.ToInt16(currRow[(int)MsgInTblStruct.CommModeId]);
                    arrCMFIn[i].protocolTypeID = Convert.ToInt16(currRow[(int)MsgInTblStruct.BoxProtocolTypeId]);
                    arrCMFIn[i].receivedDateTime = Convert.ToDateTime(currRow[(int)MsgInTblStruct.DateTimeReceived]);
                    arrCMFIn[i].sensorMask = Convert.ToInt64(currRow[(int)MsgInTblStruct.SensorMask]);
                    arrCMFIn[i].speed = Convert.ToInt16(currRow[(int)MsgInTblStruct.Speed]);
                    arrCMFIn[i].validGPS = Convert.ToSByte(currRow[(int)MsgInTblStruct.ValidGps]);
                    arrCMFIn[i].sequenceNum = Convert.ToInt32(currRow[(int)MsgInTblStruct.SequenceNum]);
                    arrCMFIn[i].isArmed = Convert.ToInt16(currRow[(int)MsgInTblStruct.IsArmed]);
                    arrCMFIn[i].priority = Convert.ToByte(currRow[(int)MsgInTblStruct.Priority]);
                    if (arrCMFIn[i].priority == CMFIn.DUPLICATE)
                    {
                        arrCMFIn[i].isDuplicatedMsg = true;
                        continue;
                    }

                    // 2. Try to backup exist message (Add new record to MsgInHst table without Blob data (only blob size))
                    // Const.violatedIntegrityMaxRetries times.
                    int currRetries = 0;
                    while (currRetries < Const.violatedIntegrityMaxRetries)
                    {
                        try
                        {
                            // this operation is very expensive because you open/close the connection for every 
                            // message instead of using two separate commands for insertion in both table (hist/histIgnored)

                            AddToHistorySLS(arrCMFIn[i]);


                            // AddToHistorySLS(arrCMFIn[i]); 

                            // On success, exit
                            currRetries = Const.violatedIntegrityMaxRetries;
                        }
                        catch (DASAppViolatedIntegrityConstraintsException e)
                        {
                            // Retrieves same msg from MsgInHst table 
                            //		[DclId],[BoxMsgInTypeId],[BoxProtocolTypeId],
                            //		[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
                            //		[Speed],[Heading],[SensorMask],[CustomProp]
                            DataSet dsSameHstRec = GetMessageFromHistory(arrCMFIn[i].boxID,
                                                                         arrCMFIn[i].originatedDateTime);
                            // Compares two msgs (if they are the same,ignore it)
                            if (dsSameHstRec != null &&
                                dsSameHstRec.Tables.Count > 0 &&
                                dsSameHstRec.Tables[0].Rows.Count > 0)
                            {
                                if ((Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["BoxMsgInTypeId"]) == arrCMFIn[i].messageTypeID) &&
                                   (Convert.ToDouble(dsSameHstRec.Tables[0].Rows[0]["Latitude"]) == arrCMFIn[i].latitude) &&
                                   (Convert.ToDouble(dsSameHstRec.Tables[0].Rows[0]["Longitude"]) == arrCMFIn[i].longitude) &&
                                   (Convert.ToInt64(dsSameHstRec.Tables[0].Rows[0]["SensorMask"]) == arrCMFIn[i].sensorMask) &&
                                   (Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["Heading"]) == arrCMFIn[i].heading) &&
                                   (Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["ValidGps"]) == arrCMFIn[i].validGPS) &&
                                    //(Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["BoxProtocolTypeId"]) == arrCMFIn[i].protocolTypeID)&&
                                    //(Convert.ToInt32(dsSameHstRec.Tables[0].Rows[0]["DclId"]) == arrCMFIn[i].dclID)&&
                                    //(dsSameHstRec.Tables[0].Rows[0]["CommInfo1"].ToString() == arrCMFIn[i].commInfo1.ToString())&&
                                    //(dsSameHstRec.Tables[0].Rows[0]["CommInfo2"].ToString() == arrCMFIn[i].commInfo2.ToString())&&
                                   (dsSameHstRec.Tables[0].Rows[0]["CustomProp"].ToString() == arrCMFIn[i].customProperties.ToString())
                                   )
                                {
                                    // add ignored msg to database
                                    AddToHistory(arrCMFIn[i], "vlfMsgInHstIgnored");
                                    // inform upper levels for error occurence
                                    // string text = "Duplicated message for box " + arrCMFIn[i].boxID + ". ";
                                    // throw new DASAppDataAlreadyExistsException(text + e.Message);
                                    // throw new DASAppDuplicatedMessageException(text + e.Message);
                                    Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsg_SLSTmp -> Duplicate message boxId[{0}] OriginDT[{1}]",
                                                   arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"));
                                    arrCMFIn[i].isDuplicatedMsg = true;
                                    break;

                                }
                            }
                            ++currRetries;
                            if (currRetries < Const.violatedIntegrityMaxRetries)
                            {
                                // here you have to compare the datetime of the received date/time
                                // and modify accordingly                           
                                if (arrCMFIn[i].receivedDateTime >
                                     Convert.ToDateTime(dsSameHstRec.Tables[0].Rows[0]["DateTimeReceived"]))
                                {
                                    Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsg_SLSTmp -> +10MSECS for boxId[{0}] OriginDT[{1} retries={2}]",
                                                   arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"), currRetries);
                                    arrCMFIn[i].originatedDateTime = arrCMFIn[i].originatedDateTime.AddMilliseconds(Const.nextDateTimeMillisecInterval);
                                }
                                else
                                {
                                    Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsg_SLSTmp -> -10MSECS for boxId[{0}] OriginDT[{1} retries={2}]",
                                                 arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"), currRetries);
                                    arrCMFIn[i].originatedDateTime = arrCMFIn[i].originatedDateTime.AddMilliseconds(-Const.nextDateTimeMillisecInterval);
                                }
                            }
                            else
                            {

                                // add ignored msg to database
                                AddToHistory(arrCMFIn[i], "vlfMsgInHstIgnored");
                                // inform upper levels for error occurence
                                // string text = "Maximal number of retries(" + currRetries + ") has been exceeded for box " + arrCMFIn[i].boxID + ". ";
                                // throw new DASAppViolatedIntegrityConstraintsException(text + e.Message);
                                Util.BTrace(Util.WARN1, "- MsgIn.DrawNextXCmfMsg_SLSTmp -> Duplicate message (MAX) boxId[{0}] OriginDT[{1}]",
                                            arrCMFIn[i].boxID, arrCMFIn[i].originatedDateTime);
                                arrCMFIn[i].isDuplicatedMsg = true;
                                break;
                            }
                        }

                    } // end while (currRetries < Const.violatedIntegrityMaxRetries

                    // 4. Skip to the next row.
                    ++i;
                }
            }
            else cnt = 0;
            return true;
        }


        /// <summary>
        /// Removes all messages for specified boxID
        /// </summary>
        /// <param name="boxID"></param>
        /// <returns>rows affected</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int FlushBoxMsg(int boxID)
        {
            return DeleteRowsByIntField("BoxID", boxID, "BoxID");
        }

        #endregion

        #region Public MsgInHistory Interfaces
        /// <summary>
        /// Retrieves last [num of records] messages from the history by Received DateTime
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetLastMessagesFromHistory(short numOfRecords, int boxId, short msgType, DateTime from, DateTime to)
        {


            DataSet resultDataSet = null;
            try
            {
                string sql = "GetLastMessagesFromHistory";

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@numRecords", SqlDbType.Int, numOfRecords);
                sqlExec.AddCommandParam("@boxId", SqlDbType.Int, boxId);
                sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, from);
                sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, to);
                resultDataSet = sqlExec.SPExecuteDataset(sql);

            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            return resultDataSet;


            //string sqlAddWhere = "";
            //if (boxId != Const.unassignedIntValue)
            //{
            //    sqlAddWhere += " AND vlfMsgInHst.BoxId=" + boxId;
            //}
            //if (msgType != Const.unassignedIntValue)
            //{
            //    sqlAddWhere += " AND vlfMsgInHst.BoxMsgInTypeId=" + msgType;
            //}
            //if (from != Const.unassignedDateTime)
            //{
            //    sqlAddWhere += " AND DateTimeReceived>='" + from + "'";
            //}
            //if (to != Const.unassignedDateTime)
            //{
            //    sqlAddWhere += " AND DateTimeReceived<='" + to + "'";
            //}
            //return GetLastMessagesFromHistory(numOfRecords, sqlAddWhere, "vlfMsgInHst");
        }



        public DataSet GetLastMessagesFromHistory(short numOfRecords, int boxId, short msgType, DateTime from, DateTime to, string OrderByDate)
        {
            string sqlAddWhere = "";
            if (boxId != Const.unassignedIntValue)
            {
                sqlAddWhere += " AND vlfMsgInHst.BoxId=" + boxId;
            }
            if (msgType != Const.unassignedIntValue)
            {
                sqlAddWhere += " AND vlfMsgInHst.BoxMsgInTypeId=" + msgType;
            }
            if (from != Const.unassignedDateTime)
            {
                sqlAddWhere += " AND DateTimeReceived>='" + from + "'";
            }
            if (to != Const.unassignedDateTime)
            {
                sqlAddWhere += " AND DateTimeReceived<='" + to + "'";
            }
            return GetLastMessagesFromHistory(numOfRecords, sqlAddWhere, "vlfMsgInHst", OrderByDate);
        }


        public DataSet GetLastMessagesFromHistory(short numOfRecords, int boxId, short msgType, DateTime from, DateTime to, string OrderByDate, string whereDateTime)
        {
            string sqlAddWhere = "";
            if (boxId != Const.unassignedIntValue)
            {
                sqlAddWhere += " AND vlfMsgInHst.BoxId=" + boxId;
            }
            if (msgType != Const.unassignedIntValue)
            {
                sqlAddWhere += " AND vlfMsgInHst.BoxMsgInTypeId=" + msgType;
            }
            if (from != Const.unassignedDateTime)
            {
                sqlAddWhere += " AND " + whereDateTime + ">='" + from + "'";
            }
            if (to != Const.unassignedDateTime)
            {
                sqlAddWhere += " AND " + whereDateTime + "<='" + to + "'";
            }
            return GetLastMessagesFromHistory(numOfRecords, sqlAddWhere, "vlfMsgInHst", OrderByDate);
        }




        /// <summary>
        /// Retrieves last [num of records] messages from the history by Received DateTime
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetLastMessagesFromHistory_Brickman201107c(short numOfRecords, int boxId, short msgType, DateTime from, DateTime to)
        {
            string sqlAddWhere = "";

            if (boxId != Const.unassignedIntValue)
            {
                sqlAddWhere += " AND vlfMsgInHstBrickman201107c.BoxId=" + boxId;
            }
            if (msgType != Const.unassignedIntValue)
            {
                sqlAddWhere += " AND vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + msgType;
            }
            if (from != Const.unassignedDateTime)
            {
                sqlAddWhere += " AND DateTimeReceived>='" + from + "'";
            }
            if (to != Const.unassignedDateTime)
            {
                sqlAddWhere += " AND DateTimeReceived<='" + to + "'";
            }
            return GetLastMessagesFromHistory_Brickman201107c(numOfRecords, sqlAddWhere, "vlfMsgInHstBrickman201107c");
        }



        /// <summary>
        /// Retrieves last [num of records] messages from the history by Received DateTime
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetLastMessagesFromHistory_Brickman201107c(short numOfRecords, int boxId, short msgType, Int64 fromTicks, Int64 toTicks)
        {
            string sqlAddWhere = "";
            DateTime from = new DateTime(fromTicks);
            DateTime to = new DateTime(toTicks);

            if (boxId != Const.unassignedIntValue)
            {
                sqlAddWhere += " AND vlfMsgInHstBrickman201107c.BoxId=" + boxId;
            }
            if (msgType != Const.unassignedIntValue)
            {
                sqlAddWhere += " AND vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + msgType;
            }

            sqlAddWhere += " AND OriginDateTime between '" + from + "' and '" + to + "'";

            return GetLastMessagesFromHistory_Brickman201107c(numOfRecords, sqlAddWhere, "vlfMsgInHstBrickman201107c");
        }

        /// <summary>
        /// Retrieves last [num of records] messages from the history by OriginDateTime
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetLastMessagesFromHistoryByOriginDateTime(short numOfRecords, int boxId, short msgType, DateTime from, DateTime to)
        {
            string sqlAddWhere = "";
            if (boxId != Const.unassignedIntValue)
            {
                sqlAddWhere += " AND vlfMsgInHst.BoxId=" + boxId;
            }
            if (msgType != Const.unassignedIntValue)
            {
                sqlAddWhere += " AND vlfMsgInHst.BoxMsgInTypeId=" + msgType;
            }
            if (from != Const.unassignedDateTime)
            {
                sqlAddWhere += " AND OriginDateTime>'" + from + "'";
            }
            if (to != Const.unassignedDateTime)
            {
                sqlAddWhere += " AND OriginDateTime<='" + to + "'";
            }
            return GetLastMessagesFromHistory(numOfRecords, sqlAddWhere, "vlfMsgInHst");
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
        /// <returns>DataSet 
        ///       [BoxId],[DateTime],[Description] -- vehicle description,
        ///       [CustomProp], 
        ///       [Acknowledged] [FwName], [FwVersion], [BoxProtocolTypeId],[MsgDirection],
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetLastUploadFirmwareMessageFromHistory(int userId, int boxId, DateTime from)
        {
            DataSet resultDataSet = null;
            try
            {

                sqlExec.ClearCommandParameters();
                //Prepares SQL statement
                sqlExec.AddCommandParam("@Date", SqlDbType.DateTime, from);
                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
                //Executes SQL statement
                sqlExec.CommandTimeout = 36000;
                resultDataSet = sqlExec.SPExecuteDataset("sp_FirmwareOTAStatus");
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            return resultDataSet;

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
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],
        /// [SequenceNum],[BoxArmed]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetLastMessagesFromHistoryByOrganization(short numOfRecords, int orgId, int fleetId, int boxId, short msgType, DateTime from, DateTime to)
        {
            string sqlAddWhere = "";
            DataSet resultDataSet = new DataSet();

            if (boxId != Const.unassignedIntValue)
            {
                sqlAddWhere += " AND vlfMsgInHst.BoxId=" + boxId;
            }
            else
            {
                if (orgId != Const.unassignedIntValue)
                {
                    sqlAddWhere += " AND vlfBox.OrganizationId=" + orgId;
                }

                if (fleetId != Const.unassignedIntValue)
                {
                    sqlAddWhere += " AND vlfFleetVehicles.FleetId=" + fleetId;
                }
            }

            if (msgType != Const.unassignedIntValue)
            {
                sqlAddWhere += " AND vlfMsgInHst.BoxMsgInTypeId=" + msgType;
            }
            if (from != Const.unassignedDateTime)
            {
                sqlAddWhere += " AND DateTimeReceived>='" + from + "'";
            }
            if (to != Const.unassignedDateTime)
            {
                sqlAddWhere += " AND DateTimeReceived<='" + to + "'";
            }


            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT top " + numOfRecords + " " + " vlfMsgInHst.BoxId, dbo.vlfMsgInHst.DateTimeReceived, " +
            "DclId,vlfBoxMsgInType.BoxMsgInTypeId,BoxMsgInTypeName," +
            "vlfBoxProtocolType.BoxProtocolTypeId,BoxProtocolTypeName," +
            "CASE WHEN CommInfo1 IS NULL then '' ELSE RTRIM(CommInfo1) END AS CommInfo1," +
            "CASE WHEN CommInfo2 IS NULL then '' ELSE RTRIM(CommInfo2) END AS CommInfo2," +
            "ValidGps,Latitude,Longitude,Heading,SensorMask," +
            "ISNULL(CustomProp,'') AS CustomProp," +
            "BlobDataSize,SequenceNum," +
            "ISNULL(vlfMsgInHst.StreetAddress,'  N/A ') AS StreetAddress," +
              "ISNULL(vlfMsgInHst.NearestLandmark ,'  N/A ') AS Landmark," +
            "ISNULL(OriginDateTime,'') AS OriginDateTime," +
            "ISNULL(Speed,0) AS Speed," +
            "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed" +
            " FROM dbo.vlfMsgInHst with (nolock) INNER JOIN " +
            " dbo.vlfBox with (nolock) ON dbo.vlfMsgInHst.BoxId = dbo.vlfBox.BoxId INNER JOIN " +
            " dbo.vlfVehicleAssignment ON dbo.vlfMsgInHst.BoxId = dbo.vlfVehicleAssignment.BoxId INNER JOIN " +
            " dbo.vlfBoxMsgInType ON dbo.vlfMsgInHst.BoxMsgInTypeId = dbo.vlfBoxMsgInType.BoxMsgInTypeId INNER JOIN " +
            " dbo.vlfBoxProtocolType ON dbo.vlfMsgInHst.BoxProtocolTypeId = dbo.vlfBoxProtocolType.BoxProtocolTypeId " +
            " WHERE 1=1 " + sqlAddWhere +
            " ORDER BY DateTimeReceived DESC,vlfBox.BoxId";

                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);

            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }

            return resultDataSet;
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetLastMessagesFromHistoryIgnored(short numOfRecords, int boxId, short msgType, DateTime from, DateTime to)
        {
            string sqlAddWhere = "";
            if (boxId != Const.unassignedIntValue)
            {
                sqlAddWhere += " AND vlfMsgInHst.BoxId=" + boxId;
            }
            if (msgType != Const.unassignedIntValue)
            {
                sqlAddWhere += " AND vlfMsgInHst.BoxMsgInTypeId=" + msgType;
            }
            if (from != Const.unassignedDateTime)
            {
                sqlAddWhere += " AND DateTimeReceived>='" + from + "'";
            }
            if (to != Const.unassignedDateTime)
            {
                sqlAddWhere += " AND DateTimeReceived<='" + to + "'";
            }
            return GetLastMessagesFromHistory(numOfRecords, sqlAddWhere, "vlfMsgInHstIgnored");
        }
        /// <summary>
        /// Retrieves last [num of records] messages from the history
        /// </summary>
        /// <remarks>
        /// Purpose:	Debug communication (vlfHistory)
        /// Note:		Does not aggregate user preferences into result (TimeZone,Metric/Imperial units,etc.)
        /// </remarks>
        /// <param name="numOfRecords"></param>
        /// <param name="sqlAddWhere"></param>
        /// <param name="tableName"></param>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],
        /// [SequenceNum],[BoxArmed]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        protected DataSet GetLastMessagesFromHistory(short numOfRecords, string sqlAddWhere, string tableName)
        {
            DataSet resultDataSet = null;



            sqlAddWhere += " AND (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speed +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speeding +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoFence +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MBAlarm +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobArm +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobDisarm +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobPanic +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Alarm +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTResponse +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTSpecialMessage +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTTextMessage +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxSetup +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxStatus +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.StoredPosition +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Idling +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtendedIdling +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZone +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntenna +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOK +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneIDs +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSetup +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VCROffDelay +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobStatus +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TARMode +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ControllerStatus +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ServiceRequired +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.LowBattery +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxReset +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputOneOn +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputTwoOn +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Odometer +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaShort +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOpen +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BreakIn +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TowAwayAlert +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Collision +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PanicButton +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SleepModeFailure +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AckWithPosition +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.UpdateTelusPRLDone +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MainBatteryDisconnected +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessages +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessagesXS +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendSMC +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendDTC +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.DeviceTestResult +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SMCWriteDone +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTAck +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.FuelTransaction +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReverseExcessSpeed +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReverseExcessDistance +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MovementAlert +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Landmark +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VirtualLandmark +
            " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReverseHyRailExcessSpeed +
            ") ";

            try
            {


                // 1. Prepares SQL statement
                //string sql = "SELECT DISTINCT top " + numOfRecords + " " + tableName + ".BoxId,DateTimeReceived," +
                string sql = "SELECT top " + numOfRecords + " " + tableName + ".BoxId,DateTimeReceived," +
                "DclId,vlfBoxMsgInType.BoxMsgInTypeId,BoxMsgInTypeName," +
                "vlfBoxProtocolType.BoxProtocolTypeId,BoxProtocolTypeName," +
                "CASE WHEN CommInfo1 IS NULL then '' ELSE RTRIM(CommInfo1) END AS CommInfo1," +
                "CASE WHEN CommInfo2 IS NULL then '' ELSE RTRIM(CommInfo2) END AS CommInfo2," +
                "ValidGps,Latitude,Longitude,Heading,SensorMask," +
                "ISNULL(CustomProp,'') AS CustomProp," +
                "BlobDataSize,SequenceNum," +
                "ISNULL(vlfMsgInHst.StreetAddress,'  N/A ') AS StreetAddress," +
                "ISNULL(vlfMsgInHst.NearestLandmark ,'  N/A ') AS Landmark, " +
                "ISNULL(OriginDateTime,'') AS OriginDateTime," +
                "ISNULL(Speed,0) AS Speed," +
                "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed" +
                " FROM " + tableName + " with (nolock),vlfBox with (nolock),vlfVehicleAssignment, vlfBoxMsgInType,vlfBoxProtocolType" +
                " WHERE " + tableName + ".BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId and vlfBox.BoxId=" + tableName + ".BoxId" + " AND vlfBox.BoxId=vlfVehicleAssignment.BoxId " +
                " AND " + tableName + ".BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId" +
                sqlAddWhere +
                " ORDER BY DateTimeReceived DESC," +
                " vlfMsgInHst.BoxId,DclId, " +
                " vlfBoxMsgInType.BoxMsgInTypeId,BoxMsgInTypeName,vlfBoxProtocolType.BoxProtocolTypeId," +
                " BoxProtocolTypeName, CommInfo1,CommInfo2,ValidGps,Latitude,Longitude,Heading,SensorMask,CustomProp,BlobDataSize,SequenceNum, " +
                " StreetAddress,Landmark,OriginDateTime,Speed,BoxArmed ";

                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            return resultDataSet;
        }


        protected DataSet GetLastMessagesFromHistory(short numOfRecords, string sqlAddWhere, string tableName, string OrderByDate)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                //string sql = "SELECT DISTINCT top " + numOfRecords + " " + tableName + ".BoxId,DateTimeReceived," +
                string sql = "SELECT top " + numOfRecords + " " + tableName + ".BoxId,DateTimeReceived," +
                "DclId,vlfBoxMsgInType.BoxMsgInTypeId,BoxMsgInTypeName," +
                "vlfBoxProtocolType.BoxProtocolTypeId,BoxProtocolTypeName," +
                "CASE WHEN CommInfo1 IS NULL then '' ELSE RTRIM(CommInfo1) END AS CommInfo1," +
                "CASE WHEN CommInfo2 IS NULL then '' ELSE RTRIM(CommInfo2) END AS CommInfo2," +
                "ValidGps,Latitude,Longitude,Heading,SensorMask," +
                "ISNULL(CustomProp,'') AS CustomProp," +
                "BlobDataSize,SequenceNum," +
                "ISNULL(vlfMsgInHst.StreetAddress,'  N/A ') AS StreetAddress," +
                "ISNULL(vlfMsgInHst.NearestLandmark ,'  N/A ') AS Landmark, " +
                "ISNULL(OriginDateTime,'') AS OriginDateTime," +
                "ISNULL(Speed,0) AS Speed," +
                "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed" +
                " FROM " + tableName + " with (nolock),vlfBox with (nolock),vlfVehicleAssignment, vlfBoxMsgInType,vlfBoxProtocolType" +
                " WHERE " + tableName + ".BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId and vlfBox.BoxId=" + tableName + ".BoxId" + " AND vlfBox.BoxId=vlfVehicleAssignment.BoxId " +
                " AND " + tableName + ".BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId" +
                sqlAddWhere +
                " ORDER BY " + OrderByDate + " DESC";


                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            return resultDataSet;
        }



        protected DataSet GetLastMessagesFromHistory_Brickman201107c(short numOfRecords, string sqlAddWhere, string tableName)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT DISTINCT top " + numOfRecords + " " + tableName + ".BoxId,DateTimeReceived," +
                "DclId,vlfBoxMsgInType.BoxMsgInTypeId,BoxMsgInTypeName," +
                "vlfBoxProtocolType.BoxProtocolTypeId,BoxProtocolTypeName," +
                "CASE WHEN CommInfo1 IS NULL then '' ELSE RTRIM(CommInfo1) END AS CommInfo1," +
                "CASE WHEN CommInfo2 IS NULL then '' ELSE RTRIM(CommInfo2) END AS CommInfo2," +
                "ValidGps,Latitude,Longitude,Heading,SensorMask," +
                "ISNULL(CustomProp,'') AS CustomProp," +
                "BlobDataSize,SequenceNum," +
                "ISNULL(" + tableName + ".StreetAddress,'  N/A ') AS StreetAddress," +
                "ISNULL(" + tableName + ".NearestLandmark ,'  N/A ') AS Landmark, " +
                "ISNULL(OriginDateTime,'') AS OriginDateTime," +
                "ISNULL(Speed,0) AS Speed," +
                "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed" +
                " FROM " + tableName + " with (nolock) ,vlfBox with (nolock),vlfVehicleAssignment, vlfBoxMsgInType,vlfBoxProtocolType" +
                " WHERE " + tableName + ".BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId and vlfBox.BoxId=" + tableName + ".BoxId" + " AND vlfBox.BoxId=vlfVehicleAssignment.BoxId " +
                " AND " + tableName + ".BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId" +
                sqlAddWhere +
                " ORDER BY DateTimeReceived DESC," +
                   tableName + ".BoxId,DclId, " +
                " vlfBoxMsgInType.BoxMsgInTypeId,BoxMsgInTypeName,vlfBoxProtocolType.BoxProtocolTypeId," +
                " BoxProtocolTypeName, CommInfo1,CommInfo2,ValidGps,Latitude,Longitude,Heading,SensorMask,CustomProp,BlobDataSize,SequenceNum, " +
                " StreetAddress,Landmark,OriginDateTime,Speed,BoxArmed ";

                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            return resultDataSet;
        }

        /// <summary>
        /// Retrieves last DoS message box id by client ip
        /// </summary>
        /// <param name="clientIp"></param>
        /// <returns> BoxId </returns>
        /// <remarks> If information does not exist, return 0</remarks>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int GetLastDoSStartedFromHistory(string clientIp)
        {
            int boxId = 0;
            try
            {
                // 1. Prepares SQL statement
                string sql = " SELECT TOP 1 BoxId FROM vlfMsgInHst with (nolock) " +
                   " WHERE BoxMsgInTypeId=" + (short)Enums.MessageType.DoSStarted +
                   " AND CommInfo1 = '" + clientIp.TrimEnd() + "' ORDER BY DateTimeReceived DESC";
                object obj = sqlExec.SQLExecuteScalar(sql);
                if (obj != null)
                    boxId = Convert.ToInt32(obj);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve last box id by clientIp=" + clientIp, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve last box id by clientIp=" + clientIp + " " + objException.Message);
            }
            return boxId;
        }
        // Changes for TimeZone Feature start
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetLastSingleMessageFromHistory_NewTZ(int userId, int boxId, Enums.MessageType msgType)
        {
            DataSet sqlDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "DECLARE @ResolveLandmark int DECLARE @Timezone float" +
               " DECLARE @Unit real" +
               " DECLARE @DayLightSaving int" +
               " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
               " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZoneNew +
               " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference" +
               " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
               " IF @Timezone IS NULL SET @Timezone=0" +
               " IF @Unit IS NULL SET @Unit=1" +
               " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving +
               " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
               " SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +

               " SELECT top 1 vlfMsgInHst.BoxId,DateTimeReceived,DclId,vlfBoxMsgInType.BoxMsgInTypeId," +
               "BoxMsgInTypeName,vlfBoxProtocolType.BoxProtocolTypeId," +
               "BoxProtocolTypeName,CommInfo1,CommInfo2,ValidGps,Latitude,Longitude," +
               "Heading,SensorMask,vlfMsgInHst.CustomProp,BlobDataSize,vlfMsgInHst.SequenceNum," +
                    "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + Const.addressNA + "') AS StreetAddress," +
                    "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),OriginDateTime) END AS OriginDateTime," +
               "CASE WHEN vlfMsgInHst.Speed IS NULL then 0 ELSE ROUND(vlfMsgInHst.Speed * @Unit,1) END AS Speed," +
               "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed" +
                    //" FROM vlfMsgInHst,vlfBoxMsgInType,vlfBoxProtocolType,vlfBox" +
                    //" WHERE vlfMsgInHst.BoxId=" + boxId +
                    //" AND vlfMsgInHst.BoxMsgInTypeId=" + (short)msgType +
                    //" AND vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId" +
                    //" AND vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId " +
                    //" Order by OriginDateTime Desc"; 

               " FROM vlfBox " +
               " INNER JOIN vlfMsgInHst with(nolock) ON vlfMsgInHst.BoxId=vlfBox.BoxID" +
                 " INNER JOIN vlfBoxMsgInType ON vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId " +
                 " INNER JOIN vlfBoxProtocolType ON vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId " +
               " WHERE vlfMsgInHst.BoxId=" + boxId +
               " AND vlfMsgInHst.BoxMsgInTypeId=" + (short)msgType +
               " AND OriginDateTime>DATEADD( ww,-1,GETUTCDATE())" +
               " Order by OriginDateTime Desc";


                // 2. Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve last message by box id " + boxId, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve last message by box id " + boxId + " " + objException.Message);
            }
            return sqlDataSet;
        }

        // Changes for TimeZone Feature end
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetLastSingleMessageFromHistory(int userId, int boxId, Enums.MessageType msgType)
        {
            DataSet sqlDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "DECLARE @ResolveLandmark int DECLARE @Timezone int" +
               " DECLARE @Unit real" +
               " DECLARE @DayLightSaving int" +
               " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
               " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZone +
               " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference" +
               " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
               " IF @Timezone IS NULL SET @Timezone=0" +
               " IF @Unit IS NULL SET @Unit=1" +
               " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving +
               " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
               " SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +

               " SELECT top 1 vlfMsgInHst.BoxId,DateTimeReceived,DclId,vlfBoxMsgInType.BoxMsgInTypeId," +
               "BoxMsgInTypeName,vlfBoxProtocolType.BoxProtocolTypeId," +
               "BoxProtocolTypeName,CommInfo1,CommInfo2,ValidGps,Latitude,Longitude," +
               "Heading,SensorMask,vlfMsgInHst.CustomProp,BlobDataSize,vlfMsgInHst.SequenceNum," +
                    "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + Const.addressNA + "') AS StreetAddress," +
                    "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,OriginDateTime) END AS OriginDateTime," +
               "CASE WHEN vlfMsgInHst.Speed IS NULL then 0 ELSE ROUND(vlfMsgInHst.Speed * @Unit,1) END AS Speed," +
               "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed" +
                    //" FROM vlfMsgInHst,vlfBoxMsgInType,vlfBoxProtocolType,vlfBox" +
                    //" WHERE vlfMsgInHst.BoxId=" + boxId +
                    //" AND vlfMsgInHst.BoxMsgInTypeId=" + (short)msgType +
                    //" AND vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId" +
                    //" AND vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId " +
                    //" Order by OriginDateTime Desc"; 

               " FROM vlfBox " +
               " INNER JOIN vlfMsgInHst with(nolock) ON vlfMsgInHst.BoxId=vlfBox.BoxID" +
                 " INNER JOIN vlfBoxMsgInType ON vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId " +
                 " INNER JOIN vlfBoxProtocolType ON vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId " +
               " WHERE vlfMsgInHst.BoxId=" + boxId +
               " AND vlfMsgInHst.BoxMsgInTypeId=" + (short)msgType +
               " AND OriginDateTime>DATEADD( ww,-1,GETUTCDATE())" +
               " Order by OriginDateTime Desc";


                // 2. Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve last message by box id " + boxId, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve last message by box id " + boxId + " " + objException.Message);
            }
            return sqlDataSet;
        }
        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves message from history by box id and DateTimeReceived
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <param name="dateTime"></param>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],
        /// [SequenceNum],[BoxArmed]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetMessageFromHistory_NewTZ(int userId, int boxId, DateTime dateTime)
        {
            string sqlWhere = " WHERE vlfMsgInHst.BoxId=" + boxId +
                           " AND OriginDateTime='" + dateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
            return GetMessagesFromHistory_NewTZ(userId, "", sqlWhere);

        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves message from history by box id and DateTimeReceived
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <param name="dateTime"></param>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],
        /// [SequenceNum],[BoxArmed]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetMessageFromHistory(int userId, int boxId, DateTime dateTime)
        {
            string sqlWhere = " WHERE vlfMsgInHst.BoxId=" + boxId +
                           " AND OriginDateTime='" + dateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
            return GetMessagesFromHistory(userId, "", sqlWhere);

        }



        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves message from history by box id and DateTime
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetDetailedMessageFromHistory_NewTZ(int userId, int boxId, DateTime originDateTime)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = " DECLARE @ResolveLandmark int DECLARE @Timezone float" +
             " DECLARE @Unit real" +
             " DECLARE @DayLightSaving int" +
             " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
             " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZoneNew +
             " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference" +
             " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
             " IF @Timezone IS NULL SET @Timezone=0" +
             " IF @Unit IS NULL SET @Unit=1" +
             " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving +
             " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
             " SET @Timezone= @Timezone + @DayLightSaving" +
                  " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +

             " SELECT distinct vlfMsgInHst.BoxId," +
             "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),OriginDateTime) END AS DateTime," +
             "vlfBoxMsgInType.BoxMsgInTypeId AS MsgTypeId,RTRIM(BoxMsgInTypeName) AS MsgTypeName," +
             "vlfBoxProtocolType.BoxProtocolTypeId,RTRIM(BoxProtocolTypeName) AS BoxProtocolTypeName," +
             "ValidGps,Latitude,Longitude,Heading,SensorMask," +
             "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                  "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + Const.addressNA + "') AS StreetAddress," +

             "CASE WHEN vlfMsgInHst.Speed IS NULL then 0 ELSE ROUND(vlfMsgInHst.Speed * @Unit,1) END AS Speed," +
             "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed," +
             "'N/A' AS UserName,'N/A' AS FirstName,'N/A' AS LastName" +
             " FROM vlfMsgInHst with (nolock) ,vlfBoxMsgInType,vlfBoxProtocolType,vlfBox" +
             " WHERE vlfMsgInHst.BoxId=" + boxId +
             " AND OriginDateTime='" + originDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
             " AND vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId" +
             " AND vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";


                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages detailed info from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages detailed info from history" + objException.Message);
            }
            return resultDataSet;
        }
        // Changes For TimeZone Feature end

        /// <summary>
        /// Retrieves message from history by box id and DateTime
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetDetailedMessageFromHistory(int userId, int boxId, DateTime originDateTime)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = " DECLARE @ResolveLandmark int DECLARE @Timezone int" +
               " DECLARE @Unit real" +
               " DECLARE @DayLightSaving int" +
               " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
               " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZone +
               " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference" +
               " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
               " IF @Timezone IS NULL SET @Timezone=0" +
               " IF @Unit IS NULL SET @Unit=1" +
               " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving +
               " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
               " SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +

               " SELECT distinct vlfMsgInHst.BoxId," +
               "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,OriginDateTime) END AS DateTime," +
               "vlfBoxMsgInType.BoxMsgInTypeId AS MsgTypeId,RTRIM(BoxMsgInTypeName) AS MsgTypeName," +
               "vlfBoxProtocolType.BoxProtocolTypeId,RTRIM(BoxProtocolTypeName) AS BoxProtocolTypeName," +
               "ValidGps,Latitude,Longitude,Heading,SensorMask," +
               "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                    "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + Const.addressNA + "') AS StreetAddress," +

               "CASE WHEN vlfMsgInHst.Speed IS NULL then 0 ELSE ROUND(vlfMsgInHst.Speed * @Unit,1) END AS Speed," +
               "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed," +
               "'N/A' AS UserName,'N/A' AS FirstName,'N/A' AS LastName" +
               " FROM vlfMsgInHst with (nolock) ,vlfBoxMsgInType,vlfBoxProtocolType,vlfBox" +
               " WHERE vlfMsgInHst.BoxId=" + boxId +
               " AND OriginDateTime='" + originDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
               " AND vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId" +
               " AND vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";
                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages detailed info from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages detailed info from history" + objException.Message);
            }
            return resultDataSet;
        }
        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves messages from history by vehicle Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isSuperUser"></param>
        /// <param name="boxId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="tblLandmarks"></param>
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetMessagesFromHistoryByBoxId_Report_NewTZ(int userId,
                                                     bool isSuperUser,
                                                     int boxId,
                                                     DateTime from,
                                                     DateTime to,
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

            //localization settings
            Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

            DataSet dsResult = null;
            requestOverflowed = false;
            string sqlWhere = " WHERE vlfMsgInHst.BoxId=" + boxId;
            if (from != Const.unassignedDateTime)
                sqlWhere += " AND OriginDateTime>=dbo.fn_ClientDSTAdjust('" + from + "'," + userId + ")";
            if (to != Const.unassignedDateTime)
                sqlWhere += " AND OriginDateTime<=dbo.fn_ClientDSTAdjust('" + to + "'," + userId + ")";


            //if (from != Const.unassignedDateTime)
            //    sqlWhere += " AND AssignedDateTime<='" + from + "'";
            //if (to != Const.unassignedDateTime)
            //    sqlWhere += " AND '" + to + "'<=ISNULL(DeletedDateTime,'01/01/2099 01:00:00 AM')";




            if (msgList == "")
            {

                // gb can we have a range for those messages instead of this huge string ??
                sqlWhere += " AND (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speed +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speeding +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoFence +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MBAlarm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobArm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobDisarm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobPanic +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Alarm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTResponse +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTSpecialMessage +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTTextMessage +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxSetup +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxStatus +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.StoredPosition +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Idling +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtendedIdling +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZone +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntenna +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOK +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneIDs +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSetup +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VCROffDelay +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobStatus +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TARMode +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ControllerStatus +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ServiceRequired +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.LowBattery +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxReset +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputOneOn +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputTwoOn +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Odometer +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaShort +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOpen +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BreakIn +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TowAwayAlert +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Collision +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PanicButton +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SleepModeFailure +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AckWithPosition +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.UpdateTelusPRLDone +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MainBatteryDisconnected +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessages +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessagesXS +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendSMC +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendDTC +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.DeviceTestResult +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SMCWriteDone +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTAck +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.FuelTransaction +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReverseExcessSpeed +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReverseExcessDistance +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MovementAlert +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Landmark +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VirtualLandmark +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSpeedStart +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSpeedEnd;



                if (isSuperUser)
                {
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.RFIDCode;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdate;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdateBantek;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendEEPROMData;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendExtendedSetup;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendExtendedStatus;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferFuelAlarm;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferSensorAlarm;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferStatusReport;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferTemperatureAlarm;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendReeferSetup;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TetheredState;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Heartbeat;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferInitialPreTrip;

                }

                if (includeSensor)
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Sensor +
                                " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SensorExtended;
                if (includeCoordinate)//"Scheduled Update" is displayed name
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate;
                    else
                        sqlWhere += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate + " AND ValidGps=0)";
                }
                if (includePositionUpdate)
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate;
                    else
                        sqlWhere += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate + " AND ValidGps=0)";
                }
                sqlWhere += ")";
            }
            else
            {
                string[] msgTypeId = msgList.Split(',');
                if (msgTypeId.Length > 0)
                {
                    sqlWhere += " AND (";
                    for (int i = 0; i < msgTypeId.Length; i++)
                    {
                        sqlWhere += " vlfMsgInHst.BoxMsgInTypeId=" + msgTypeId[i] + " OR ";
                    }

                    sqlWhere = sqlWhere.Substring(0, sqlWhere.Length - 4);
                    sqlWhere += ")";
                }
            }





            if (DclId != -1)
                sqlWhere += " AND DclId=" + DclId.ToString();

            try
            {
                // 1. Prepares SQL statement

                string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone float" +
                    " DECLARE @Unit real" +
                    " DECLARE @DayLightSaving int,@DST_Change bit" +
                    " declare @lang varchar(8) " +
                    " set @lang= dbo.udf_UserPreference_Text (" + userId + ", 37) " +
                    " if @lang=null or @lang='' set @lang='en-US' " +
                    " set @DST_Change=0 " +
                    " IF dbo.fn_GetDSTOffset('" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "')<>dbo.fn_GetDSTOffset('" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "')" +
                    " SET @DST_Change=1" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZoneNew +
                    " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference" +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " IF @Unit IS NULL SET @Unit=1" +
                    //" SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving +
                    " SELECT @DayLightSaving=dbo.fn_GetDSTOffset('" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "') " +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " IF @DST_Change=0 SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0";

                string sqlFooterSelect1 = "SELECT " + sqlTopRecords + " vlfMsgInHst.BoxId,vlfVehicleAssignment.VehicleId,vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.Description, " +
                   "CASE WHEN @DST_Change=1  then  dbo.udf_DateTimeAdjustFormat(DATEADD(minute,(@Timezone * 60) +(dbo.fn_GetDSTOffset(DateTimeReceived)*60) ,DateTimeReceived),0,@lang) ELSE  dbo.udf_DateTimeAdjustFormat(DATEADD(minute,(@Timezone * 60),DateTimeReceived),0,@lang)  END AS DateTimeReceived," +
                   "DclId,vlfBoxMsgInType.BoxMsgInTypeId," +
                   "CASE WHEN BoxMsgInTypeName IS NOT NULL then RTRIM(BoxMsgInTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "convert(nvarchar,ValidGps) AS ValidGps," +
                   "LTRIM(RTRIM(STR(ROUND(Latitude,5),10,5))) AS Latitude," +
                   "LTRIM(RTRIM(STR(ROUND(Longitude,6),10,6))) AS Longitude," +
                   "convert(nvarchar,Heading) AS Heading," +
                   "convert(nvarchar,SensorMask) AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "convert(nvarchar,BlobDataSize) AS BlobDataSize,SequenceNum," +
                        "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + Const.addressNA + "') AS StreetAddress," +
                   "CASE WHEN @DST_Change=1 then dbo.udf_DateTimeAdjustFormat(DATEADD(minute,(@Timezone * 60) +(dbo.fn_GetDSTOffset(OriginDateTime)*60) ,OriginDateTime),0,@lang) ELSE dbo.udf_DateTimeAdjustFormat(DATEADD(minute,(@Timezone * 60),OriginDateTime),0,@lang) END AS OriginDateTime_str,OriginDateTime," +
                   "CASE WHEN vlfMsgInHst.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHst.Speed * @Unit,1)) END AS Speed," +
                    //"convert(nvarchar,CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END) AS BoxArmed,"+
                   "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed," +
                   "0 AS MsgDirection," + // in
                   "' ' AS Acknowledged," +
                   "' ' AS Scheduled";

                string sqlFooter1 = "FROM vlfMsgInHst with (nolock),vlfBoxMsgInType,vlfBoxProtocolType,vlfVehicleAssignment,vlfVehicleInfo WITH (NOLOCK)" +
                   sqlWhere +
                   " AND vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId" +
                   " AND vlfMsgInHst.BoxId=vlfVehicleAssignment.BoxId" +
                   " AND vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId" +
                   " AND vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";
                // retrieve all commands/outputs from vlfMsgOutHst
                sqlWhere = "";
                //if (from != Const.unassignedDateTime)
                //    sqlWhere += " AND AssignedDateTime<='" + from + "'";
                //if (to != Const.unassignedDateTime)
                //    sqlWhere += " AND '" + to + "'<=ISNULL(DeletedDateTime,'01/01/2099 01:00:00 AM')";



                if (from != Const.unassignedDateTime)
                    sqlWhere += " AND DateTime>=dbo.fn_ClientDSTAdjust('" + from + "'," + userId + ")";

                if (to != Const.unassignedDateTime)
                    sqlWhere += " AND DateTime<=dbo.fn_ClientDSTAdjust('" + to + "'," + userId + ")";



                sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.Ack +
                         " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.MDTAck;
                if (!isSuperUser)
                {
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.WakeUpByte;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.UploadFirmware;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.UploadFirmwareStatus;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.ThirdPartyPacket;
                }
                if (DclId != -1)
                    sqlWhere += " AND DclId=" + DclId.ToString();


                string sqlFooterSelect2 = "SELECT  " + sqlTopRecords + " vlfMsgOutHst.BoxId,vlfVehicleAssignment.VehicleId,vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.Description, " +
                   "dbo.udf_DateTimeAdjustFormat(DATEADD(minute,(@Timezone * 60),DateTime),0,@lang) AS DateTimeReceived,DclId," +
                   "vlfBoxCmdOutType.BoxCmdOutTypeId AS BoxMsgInTypeId," +
                   "CASE WHEN BoxCmdOutTypeName IS NOT NULL then RTRIM(BoxCmdOutTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "'" + Const.blankValue + "' AS ValidGps," +
                   "'" + Const.blankValue + "' AS Latitude," +
                   "'" + Const.blankValue + "' AS Longitude," +
                   "'" + Const.blankValue + "' AS Heading," +
                   "'" + Const.blankValue + "' AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "'" + Const.blankValue + "' As BlobDataSize," +
                   "SequenceNum," +
                   "'" + Const.blankValue + "' AS StreetAddress," +
                   "dbo.udf_DateTimeAdjustFormat(DATEADD(minute,(@Timezone * 60),DateTime),0,@lang) AS OriginDateTime_Str,[DateTime] as OriginDateTime," +
                   "'" + Const.blankValue + "' AS Speed," +
                   "'" + Const.blankValue + "' AS BoxArmed," +
                   "1 AS MsgDirection," + // out
                   "CASE WHEN Acknowledged IS NOT NULL then CASE WHEN RTRIM(Acknowledged) IS NOT NULL then RTRIM(Acknowledged) END ELSE '" + Const.cmdNotAck + "' END AS Acknowledged," +
                   "CASE WHEN Scheduled IS NOT NULL then CASE WHEN Scheduled=0 then 'No' END ELSE 'Yes' END AS Scheduled";
                string sqlFooter2 = "FROM vlfMsgOutHst,vlfBoxCmdOutType,vlfBoxProtocolType,vlfVehicleAssignment,vlfVehicleInfo WITH (NOLOCK)" +
                   " WHERE vlfMsgOutHst.BoxId=" + boxId +
                   sqlWhere +
                   " AND vlfMsgOutHst.BoxCmdOutTypeId=vlfBoxCmdOutType.BoxCmdOutTypeId" +
                   " AND vlfMsgOutHst.BoxId=vlfVehicleAssignment.BoxId" +
                   " AND vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId" +
                   " AND vlfMsgOutHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";
                if (DclId != -1)
                    sqlWhere += " AND DclId=" + DclId.ToString();

                string sql = "";
                if (msgList == "" && sqlTopRecords == "")
                {
                    sql = sqlHeader + " " + sqlFooterSelect1 + " " + sqlFooter1 +
                       " UNION " + sqlFooterSelect2 + " " + sqlFooter2 +
                       " ORDER BY OriginDateTime DESC";
                }
                else
                {
                    sql = sqlHeader + " " + sqlFooterSelect1 + " " + sqlFooter1 +
                       " ORDER BY OriginDateTime DESC";
                }

                try
                {
                    int sqlTop = GetConfigParameter("ASI", (short)Enums.ConfigurationGroups.Common, "History Max Records", 2000);
                    if (sqlTop != Const.unassignedIntValue)
                    {
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter1));
                        if ((sqlTop - totalSqlRecords) < 0)
                        {
                            requestOverflowed = true;
                            return null;
                        }
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter2));
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
                    throw new DASException("Unable to retieve messages from history" + exp.Message);
                }
                // 2. Executes SQL statement
                dsResult = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                // 1. Add additional column to result dataset
                dsResult.Tables[0].Columns.Add("MsgDetails", typeof(string));
                //int boxId = Const.unassignedIntValue;
                DataSet dsVehicleSensors = null;
                DataSet dsVehicleOutputs = null;
                string fenceDirection = "";

                // 2. Get user-defined sensors info (name,action)
                //[SensorId][SensorName][SensorAction][AlarmLevel]
                DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
                dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);


                //localization Sensors

                Localization local = new Localization(sqlExec.ConnectionString.ToString());

                Box boxCnf = new DB.Box(sqlExec);
                BoxHwDefaultSensorsCfg defSensors = new DB.BoxHwDefaultSensorsCfg(sqlExec);


                Int16 hwTypeId = 0;
                DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
                if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
                {
                    // TODO: Today box has only one HW type
                    hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);


                    if (dsVehicleSensors == null || dsVehicleSensors.Tables.Count == 0 || dsVehicleSensors.Tables[0].Rows.Count == 0)
                        dsVehicleSensors = defSensors.GetSensorsInfoByHwTypeId(hwTypeId);
                }
                else
                {
                    // TODO: write to log (cannot retrieve HW type for specific box)
                }


                if ((lang != "en") && (lang != null))
                {
                    local.LocalizationData(lang, "SensorId", "SensorName", "Sensors", hwTypeId, ref dsVehicleSensors);
                    local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
                }



                // 3. Get user-defined outputs info 
                //[OutputId][OutputName][OutputAction]
                DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
                dsVehicleOutputs = outputs.GetOutputsInfoByBoxId(boxId, userId);

                //localization outputs
                if ((lang != "en") && (lang != null))
                {

                    local.LocalizationData(lang, "OutputId", "OutputName", "Outputs", hwTypeId, ref dsVehicleOutputs);
                    local.LocalizationDataAction(lang, "OutputId", "OutputAction", "Outputs", hwTypeId, ref dsVehicleOutputs);
                }


                //localization Commands
                DB.BoxProtocolTypeCmdOutType cmds = new DB.BoxProtocolTypeCmdOutType(sqlExec);
                DataSet dsVehicleCommands = cmds.GetAllSupportedCommands(boxId, userId);
                if ((lang != "en") && (lang != null))
                    local.LocalizationData(lang, "BoxCmdOutTypeId", "BoxCmdOutTypeName", "CommandType", ref dsVehicleCommands);


                // 4. Resolve vehicle geozones
                DB.VehicleAssignment vehicleAssignment = null;
                DB.VehicleGeozone vehicleGeozone = null;
                DataSet dsGeozonesInfo = null;
                Int64 vehicleId = Const.unassignedIntValue;




                foreach (DataRow ittr in dsResult.Tables[0].Rows)
                {
                    // a simple change which should make the code quicker and more readable 
                    string customProp = ittr["CustomProp"].ToString().TrimEnd();

                    if (Convert.ToInt16(ittr["MsgDirection"]) == Const.MsgOutDirectionValue)// process outgoing messages
                    {
                        switch ((Enums.CommandType)Convert.ToInt16(ittr["BoxMsgInTypeId"]))
                        {
                            case Enums.CommandType.Output:
                                ittr["MsgDetails"] = DB.Output.GetOutputDescription(customProp, dsVehicleOutputs.Tables[0]);
                                break;

                            case Enums.CommandType.MDTTextMessage:
                                ittr["BoxMsgInTypeName"] = Resources.Const.TxtMessage;
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                ittr["MsgDetails"] += Convert.ToChar(13).ToString();
                                ittr["MsgDetails"] += Convert.ToChar(10).ToString();
                                ittr["MsgDetails"] += Resources.Const.TxtMsgAnswers + ":" + Util.PairFindValue(Const.keyAnswer, customProp).Replace("~", ",");
                                break;

                            default:
                                ittr["MsgDetails"] = ittr["BoxMsgInTypeName"];
                                //localization commands
                                if ((lang != "en") && (lang != null))
                                {
                                    foreach (DataRow dr in dsVehicleCommands.Tables[0].Rows)
                                    {
                                        if (Convert.ToInt16(ittr["BoxMsgInTypeId"]) == Convert.ToInt16(dr["BoxCmdOutTypeId"]))
                                        {
                                            ittr["MsgDetails"] = dr["BoxCmdOutTypeName"];
                                            break;
                                        }

                                    }
                                }
                                ittr["BoxMsgInTypeName"] = Resources.Const.Command;
                                break;
                        }
                        ittr["StreetAddress"] = Const.blankValue; // not applicable

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgOutDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                    else // process incomming messages
                    {
                        Enums.MessageType msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);

                        switch (msgType)
                        {
                            case Enums.MessageType.Sensor:
                            case Enums.MessageType.SensorExtended:
                            case Enums.MessageType.Alarm:
                            case Enums.MessageType.MBAlarm:
                            case Enums.MessageType.ServiceRequired:
                                if (dsVehicleSensors != null && dsVehicleSensors.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, dsVehicleSensors.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, null);
                                break;
                            case Enums.MessageType.TetheredState:
                                {
                                    string duration = Util.PairFindValue(Const.keyTetheredState, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = duration.Equals(Const.valON) ? Resources.Const.alright : Resources.Const.extinct;
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.Speeding:      // was Speeding
                                {
                                    string duration = Util.PairFindValue(Const.keySpeedDuration, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.GeoFence:
                                //fenceNo = Util.PairFindValue(Const.keyFenceNo,customProp);
                                //if(fenceNo == null || fenceNo == "")
                                //	fenceNo = "Unknown";

                                fenceDirection = Util.PairFindValue(Const.keyFenceDir, customProp);
                                if (fenceDirection == null || fenceDirection == "")
                                    fenceDirection = Resources.Const.fenceDirection_unknown;
                                else if (Const.valFenceDirIn == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_inbroken;
                                else if (Const.valFenceDirOut == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_outbroken;

                                ittr["MsgDetails"] = fenceDirection;
                                break;
                            case Enums.MessageType.GeoZone:
                                if (vehicleAssignment == null)
                                {
                                    vehicleAssignment = new DB.VehicleAssignment(sqlExec);
                                    vehicleId = Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
                                }
                                if (vehicleGeozone == null)
                                {
                                    vehicleGeozone = new DB.VehicleGeozone(sqlExec);
                                    dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
                                }

                                if (dsGeozonesInfo != null && dsGeozonesInfo.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, dsGeozonesInfo.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, null);
                                break;
                            case Enums.MessageType.IPUpdate:
                                ittr["MsgDetails"] = ittr["CommInfo1"].ToString() + ":" + ittr["CommInfo2"].ToString().TrimEnd();
                                ittr["StreetAddress"] = Const.blankValue;
                                ittr["Speed"] = Const.blankValue;
                                break;
                            case Enums.MessageType.Idling:
                            case Enums.MessageType.ExtendedIdling:
                            case Enums.MessageType.HarshBraking:
                            case Enums.MessageType.ExtremeBraking:
                            case Enums.MessageType.HarshAcceleration:
                            case Enums.MessageType.ExtremeAcceleration:
                            case Enums.MessageType.SeatBelt:
                                string speedDuration = "";
                                if (Enums.MessageType.Idling == msgType || Enums.MessageType.ExtendedIdling == msgType)
                                    speedDuration = Util.PairFindValue(Const.keyIdleDuration, customProp);
                                else
                                    speedDuration = Util.PairFindValue(Const.keyDuration, customProp);
                                if (speedDuration != "")
                                {
                                    try
                                    {
                                        ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(speedDuration) * TimeSpan.TicksPerSecond).ToString();
                                    }
                                    catch
                                    {
                                    }
                                }
                                break;
                            case Enums.MessageType.GPSAntenna:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyGPSAntenna, customProp);
                                break;

                            case Enums.MessageType.MDTResponse:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyAnswer, customProp);
                                break;
                            case Enums.MessageType.MDTSpecialMessage:
                                ittr["MsgDetails"] = customProp;
                                break;
                            case Enums.MessageType.MDTTextMessage:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                break;
                            case Enums.MessageType.Status:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyPower, customProp);
                                break;
                            case Enums.MessageType.RFIDCode:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.extFacilityCode, customProp) + "-" +
                                                      Util.PairFindValue(Const.extIDNumber, customProp);
                                break;
                            case Enums.MessageType.UpdateTelusPRLDone:
                                ittr["MsgDetails"] = "PRL=" + Util.PairFindValue(Const.keyPRLNumber, customProp);
                                break;
                            default:// do nothing
                                ittr["MsgDetails"] = " ";
                                break;
                        }
                        // in case of invalid GPS for current message set defaults
                        if (Convert.ToInt16(ittr["ValidGps"]) == 1)
                        {
                            ittr["Latitude"] = 0;
                            ittr["Longitude"] = 0;
                            // ittr["Speed"] = Const.blankValue;
                            ittr["Heading"] = Const.blankValue;
                        }

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgInDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                }
            }
            return dsResult;
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves messages from history by vehicle Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isSuperUser"></param>
        /// <param name="boxId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="tblLandmarks"></param>
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetMessagesFromHistoryByBoxId_Report(int userId,
                                                     bool isSuperUser,
                                                     int boxId,
                                                     DateTime from,
                                                     DateTime to,
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

            //localization settings
            Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

            DataSet dsResult = null;
            requestOverflowed = false;
            string sqlWhere = " WHERE vlfMsgInHst.BoxId=" + boxId;
            if (from != Const.unassignedDateTime)
                sqlWhere += " AND OriginDateTime>=dbo.fn_ClientDSTAdjust('" + from + "'," + userId + ")";
            if (to != Const.unassignedDateTime)
                sqlWhere += " AND OriginDateTime<=dbo.fn_ClientDSTAdjust('" + to + "'," + userId + ")";


            //if (from != Const.unassignedDateTime)
            //    sqlWhere += " AND AssignedDateTime<='" + from + "'";
            //if (to != Const.unassignedDateTime)
            //    sqlWhere += " AND '" + to + "'<=ISNULL(DeletedDateTime,'01/01/2099 01:00:00 AM')";




            if (msgList == "")
            {

                // gb can we have a range for those messages instead of this huge string ??
                sqlWhere += " AND (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speed +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speeding +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoFence +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MBAlarm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobArm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobDisarm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobPanic +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Alarm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTResponse +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTSpecialMessage +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTTextMessage +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxSetup +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxStatus +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.StoredPosition +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Idling +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtendedIdling +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZone +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntenna +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOK +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneIDs +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSetup +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VCROffDelay +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobStatus +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TARMode +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ControllerStatus +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ServiceRequired +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.LowBattery +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxReset +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputOneOn +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputTwoOn +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Odometer +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaShort +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOpen +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BreakIn +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TowAwayAlert +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Collision +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PanicButton +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SleepModeFailure +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AckWithPosition +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.UpdateTelusPRLDone +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MainBatteryDisconnected +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessages +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessagesXS +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendSMC +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendDTC +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.DeviceTestResult +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SMCWriteDone +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTAck +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.FuelTransaction +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReverseExcessSpeed +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReverseExcessDistance +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MovementAlert +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Landmark +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VirtualLandmark +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSpeedStart +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSpeedEnd;



                if (isSuperUser)
                {
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.RFIDCode;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdate;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdateBantek;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendEEPROMData;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendExtendedSetup;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendExtendedStatus;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferFuelAlarm;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferSensorAlarm;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferStatusReport;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferTemperatureAlarm;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendReeferSetup;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TetheredState;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Heartbeat;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferInitialPreTrip;

                }

                if (includeSensor)
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Sensor +
                                " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SensorExtended;
                if (includeCoordinate)//"Scheduled Update" is displayed name
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate;
                    else
                        sqlWhere += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate + " AND ValidGps=0)";
                }
                if (includePositionUpdate)
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate;
                    else
                        sqlWhere += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate + " AND ValidGps=0)";
                }
                sqlWhere += ")";
            }
            else
            {
                string[] msgTypeId = msgList.Split(',');
                if (msgTypeId.Length > 0)
                {
                    sqlWhere += " AND (";
                    for (int i = 0; i < msgTypeId.Length; i++)
                    {
                        sqlWhere += " vlfMsgInHst.BoxMsgInTypeId=" + msgTypeId[i] + " OR ";
                    }

                    sqlWhere = sqlWhere.Substring(0, sqlWhere.Length - 4);
                    sqlWhere += ")";
                }
            }





            if (DclId != -1)
                sqlWhere += " AND DclId=" + DclId.ToString();

            try
            {
                // 1. Prepares SQL statement
                string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone int" +
                    " DECLARE @Unit real" +
                    " DECLARE @DayLightSaving int" +
                    " declare @lang varchar(8) " +
                    " set @lang= dbo.udf_UserPreference_Text (" + userId + ", 37) " +
                    " if @lang=null or @lang='' set @lang='en-US' " +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZone +
                    " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference" +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " IF @Unit IS NULL SET @Unit=1" +
                    //" SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving +
                    " SELECT @DayLightSaving=dbo.fn_GetDSTOffset('" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "') " +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0";

                string sqlFooterSelect1 = "SELECT " + sqlTopRecords + " vlfMsgInHst.BoxId,vlfVehicleAssignment.VehicleId,vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.Description, " +
                   "CASE WHEN DateTimeReceived IS NULL then '' ELSE dbo.udf_DateTimeAdjustFormat(DATEADD(hour,@Timezone,DateTimeReceived),0,@lang)  END AS DateTimeReceived," +
                   "DclId,vlfBoxMsgInType.BoxMsgInTypeId," +
                   "CASE WHEN BoxMsgInTypeName IS NOT NULL then RTRIM(BoxMsgInTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "convert(nvarchar,ValidGps) AS ValidGps," +
                   "LTRIM(RTRIM(STR(ROUND(Latitude,5),10,5))) AS Latitude," +
                   "LTRIM(RTRIM(STR(ROUND(Longitude,6),10,6))) AS Longitude," +
                   "convert(nvarchar,Heading) AS Heading," +
                   "convert(nvarchar,SensorMask) AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "convert(nvarchar,BlobDataSize) AS BlobDataSize,SequenceNum," +
                        "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + Const.addressNA + "') AS StreetAddress," +


                   "CASE WHEN OriginDateTime IS NULL then '' ELSE dbo.udf_DateTimeAdjustFormat(DATEADD(hour,@Timezone,OriginDateTime),0,@lang) END AS OriginDateTime_str,OriginDateTime," +
                   "CASE WHEN vlfMsgInHst.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHst.Speed * @Unit,1)) END AS Speed," +
                    //"convert(nvarchar,CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END) AS BoxArmed,"+
                   "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed," +
                   "0 AS MsgDirection," + // in
                   "' ' AS Acknowledged," +
                   "' ' AS Scheduled";

                string sqlFooter1 = "FROM vlfMsgInHst with (nolock),vlfBoxMsgInType,vlfBoxProtocolType,vlfVehicleAssignment,vlfVehicleInfo WITH (NOLOCK)" +
                   sqlWhere +
                   " AND vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId" +
                   " AND vlfMsgInHst.BoxId=vlfVehicleAssignment.BoxId" +
                   " AND vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId" +
                   " AND vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";
                // retrieve all commands/outputs from vlfMsgOutHst
                sqlWhere = "";
                //if (from != Const.unassignedDateTime)
                //    sqlWhere += " AND AssignedDateTime<='" + from + "'";
                //if (to != Const.unassignedDateTime)
                //    sqlWhere += " AND '" + to + "'<=ISNULL(DeletedDateTime,'01/01/2099 01:00:00 AM')";



                if (from != Const.unassignedDateTime)
                    sqlWhere += " AND DateTime>=dbo.fn_ClientDSTAdjust('" + from + "'," + userId + ")";

                if (to != Const.unassignedDateTime)
                    sqlWhere += " AND DateTime<=dbo.fn_ClientDSTAdjust('" + to + "'," + userId + ")";



                sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.Ack +
                         " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.MDTAck;
                if (!isSuperUser)
                {
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.WakeUpByte;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.UploadFirmware;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.UploadFirmwareStatus;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.ThirdPartyPacket;
                }
                if (DclId != -1)
                    sqlWhere += " AND DclId=" + DclId.ToString();


                string sqlFooterSelect2 = "SELECT  " + sqlTopRecords + " vlfMsgOutHst.BoxId,vlfVehicleAssignment.VehicleId,vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.Description, " +
                   "dbo.udf_DateTimeAdjustFormat(DATEADD(hour,@Timezone,DateTime),0,@lang) AS DateTimeReceived,DclId," +
                   "vlfBoxCmdOutType.BoxCmdOutTypeId AS BoxMsgInTypeId," +
                   "CASE WHEN BoxCmdOutTypeName IS NOT NULL then RTRIM(BoxCmdOutTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "'" + Const.blankValue + "' AS ValidGps," +
                   "'" + Const.blankValue + "' AS Latitude," +
                   "'" + Const.blankValue + "' AS Longitude," +
                   "'" + Const.blankValue + "' AS Heading," +
                   "'" + Const.blankValue + "' AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "'" + Const.blankValue + "' As BlobDataSize," +
                   "SequenceNum," +
                   "'" + Const.blankValue + "' AS StreetAddress," +
                   "dbo.udf_DateTimeAdjustFormat(DATEADD(hour,@Timezone,DateTime),0,@lang) AS OriginDateTime_Str,[DateTime] as OriginDateTime," +
                   "'" + Const.blankValue + "' AS Speed," +
                   "'" + Const.blankValue + "' AS BoxArmed," +
                   "1 AS MsgDirection," + // out
                   "CASE WHEN Acknowledged IS NOT NULL then CASE WHEN RTRIM(Acknowledged) IS NOT NULL then RTRIM(Acknowledged) END ELSE '" + Const.cmdNotAck + "' END AS Acknowledged," +
                   "CASE WHEN Scheduled IS NOT NULL then CASE WHEN Scheduled=0 then 'No' END ELSE 'Yes' END AS Scheduled";
                string sqlFooter2 = "FROM vlfMsgOutHst,vlfBoxCmdOutType,vlfBoxProtocolType,vlfVehicleAssignment,vlfVehicleInfo WITH (NOLOCK)" +
                   " WHERE vlfMsgOutHst.BoxId=" + boxId +
                   sqlWhere +
                   " AND vlfMsgOutHst.BoxCmdOutTypeId=vlfBoxCmdOutType.BoxCmdOutTypeId" +
                   " AND vlfMsgOutHst.BoxId=vlfVehicleAssignment.BoxId" +
                   " AND vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId" +
                   " AND vlfMsgOutHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";
                if (DclId != -1)
                    sqlWhere += " AND DclId=" + DclId.ToString();

                string sql = "";
                if (msgList == "" && sqlTopRecords == "")
                {
                    sql = sqlHeader + " " + sqlFooterSelect1 + " " + sqlFooter1 +
                       " UNION " + sqlFooterSelect2 + " " + sqlFooter2 +
                       " ORDER BY OriginDateTime DESC";
                }
                else
                {
                    sql = sqlHeader + " " + sqlFooterSelect1 + " " + sqlFooter1 +
                       " ORDER BY OriginDateTime DESC";
                }

                try
                {
                    int sqlTop = GetConfigParameter("ASI", (short)Enums.ConfigurationGroups.Common, "History Max Records", 2000);
                    if (sqlTop != Const.unassignedIntValue)
                    {
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter1));
                        if ((sqlTop - totalSqlRecords) < 0)
                        {
                            requestOverflowed = true;
                            return null;
                        }
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter2));
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
                    throw new DASException("Unable to retieve messages from history" + exp.Message);
                }
                // 2. Executes SQL statement
                dsResult = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                // 1. Add additional column to result dataset
                dsResult.Tables[0].Columns.Add("MsgDetails", typeof(string));
                //int boxId = Const.unassignedIntValue;
                DataSet dsVehicleSensors = null;
                DataSet dsVehicleOutputs = null;
                string fenceDirection = "";

                // 2. Get user-defined sensors info (name,action)
                //[SensorId][SensorName][SensorAction][AlarmLevel]
                DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
                dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);


                //localization Sensors

                Localization local = new Localization(sqlExec.ConnectionString.ToString());

                Box boxCnf = new DB.Box(sqlExec);
                BoxHwDefaultSensorsCfg defSensors = new DB.BoxHwDefaultSensorsCfg(sqlExec);


                Int16 hwTypeId = 0;
                DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
                if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
                {
                    // TODO: Today box has only one HW type
                    hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);


                    if (dsVehicleSensors == null || dsVehicleSensors.Tables.Count == 0 || dsVehicleSensors.Tables[0].Rows.Count == 0)
                        dsVehicleSensors = defSensors.GetSensorsInfoByHwTypeId(hwTypeId);
                }
                else
                {
                    // TODO: write to log (cannot retrieve HW type for specific box)
                }


                if ((lang != "en") && (lang != null))
                {
                    local.LocalizationData(lang, "SensorId", "SensorName", "Sensors", hwTypeId, ref dsVehicleSensors);
                    local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
                }



                // 3. Get user-defined outputs info 
                //[OutputId][OutputName][OutputAction]
                DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
                dsVehicleOutputs = outputs.GetOutputsInfoByBoxId(boxId, userId);

                //localization outputs
                if ((lang != "en") && (lang != null))
                {

                    local.LocalizationData(lang, "OutputId", "OutputName", "Outputs", hwTypeId, ref dsVehicleOutputs);
                    local.LocalizationDataAction(lang, "OutputId", "OutputAction", "Outputs", hwTypeId, ref dsVehicleOutputs);
                }


                //localization Commands
                DB.BoxProtocolTypeCmdOutType cmds = new DB.BoxProtocolTypeCmdOutType(sqlExec);
                DataSet dsVehicleCommands = cmds.GetAllSupportedCommands(boxId, userId);
                if ((lang != "en") && (lang != null))
                    local.LocalizationData(lang, "BoxCmdOutTypeId", "BoxCmdOutTypeName", "CommandType", ref dsVehicleCommands);


                // 4. Resolve vehicle geozones
                DB.VehicleAssignment vehicleAssignment = null;
                DB.VehicleGeozone vehicleGeozone = null;
                DataSet dsGeozonesInfo = null;
                Int64 vehicleId = Const.unassignedIntValue;




                foreach (DataRow ittr in dsResult.Tables[0].Rows)
                {
                    // a simple change which should make the code quicker and more readable 
                    string customProp = ittr["CustomProp"].ToString().TrimEnd();

                    if (Convert.ToInt16(ittr["MsgDirection"]) == Const.MsgOutDirectionValue)// process outgoing messages
                    {
                        switch ((Enums.CommandType)Convert.ToInt16(ittr["BoxMsgInTypeId"]))
                        {
                            case Enums.CommandType.Output:
                                ittr["MsgDetails"] = DB.Output.GetOutputDescription(customProp, dsVehicleOutputs.Tables[0]);
                                break;

                            case Enums.CommandType.MDTTextMessage:
                                ittr["BoxMsgInTypeName"] = Resources.Const.TxtMessage;
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                ittr["MsgDetails"] += Convert.ToChar(13).ToString();
                                ittr["MsgDetails"] += Convert.ToChar(10).ToString();
                                ittr["MsgDetails"] += Resources.Const.TxtMsgAnswers + ":" + Util.PairFindValue(Const.keyAnswer, customProp).Replace("~", ",");
                                break;

                            default:
                                ittr["MsgDetails"] = ittr["BoxMsgInTypeName"];
                                //localization commands
                                if ((lang != "en") && (lang != null))
                                {
                                    foreach (DataRow dr in dsVehicleCommands.Tables[0].Rows)
                                    {
                                        if (Convert.ToInt16(ittr["BoxMsgInTypeId"]) == Convert.ToInt16(dr["BoxCmdOutTypeId"]))
                                        {
                                            ittr["MsgDetails"] = dr["BoxCmdOutTypeName"];
                                            break;
                                        }

                                    }
                                }
                                ittr["BoxMsgInTypeName"] = Resources.Const.Command;
                                break;
                        }
                        ittr["StreetAddress"] = Const.blankValue; // not applicable

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgOutDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                    else // process incomming messages
                    {
                        Enums.MessageType msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);

                        switch (msgType)
                        {
                            case Enums.MessageType.Sensor:
                            case Enums.MessageType.SensorExtended:
                            case Enums.MessageType.Alarm:
                            case Enums.MessageType.MBAlarm:
                            case Enums.MessageType.ServiceRequired:
                                if (dsVehicleSensors != null && dsVehicleSensors.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, dsVehicleSensors.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, null);
                                break;
                            case Enums.MessageType.TetheredState:
                                {
                                    string duration = Util.PairFindValue(Const.keyTetheredState, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = duration.Equals(Const.valON) ? Resources.Const.alright : Resources.Const.extinct;
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.Speeding:      // was Speeding
                                {
                                    string duration = Util.PairFindValue(Const.keySpeedDuration, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.GeoFence:
                                //fenceNo = Util.PairFindValue(Const.keyFenceNo,customProp);
                                //if(fenceNo == null || fenceNo == "")
                                //	fenceNo = "Unknown";

                                fenceDirection = Util.PairFindValue(Const.keyFenceDir, customProp);
                                if (fenceDirection == null || fenceDirection == "")
                                    fenceDirection = Resources.Const.fenceDirection_unknown;
                                else if (Const.valFenceDirIn == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_inbroken;
                                else if (Const.valFenceDirOut == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_outbroken;

                                ittr["MsgDetails"] = fenceDirection;
                                break;
                            case Enums.MessageType.GeoZone:
                                if (vehicleAssignment == null)
                                {
                                    vehicleAssignment = new DB.VehicleAssignment(sqlExec);
                                    vehicleId = Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
                                }
                                if (vehicleGeozone == null)
                                {
                                    vehicleGeozone = new DB.VehicleGeozone(sqlExec);
                                    dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
                                }

                                if (dsGeozonesInfo != null && dsGeozonesInfo.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, dsGeozonesInfo.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, null);
                                break;
                            case Enums.MessageType.IPUpdate:
                                ittr["MsgDetails"] = ittr["CommInfo1"].ToString() + ":" + ittr["CommInfo2"].ToString().TrimEnd();
                                ittr["StreetAddress"] = Const.blankValue;
                                ittr["Speed"] = Const.blankValue;
                                break;
                            case Enums.MessageType.Idling:
                            case Enums.MessageType.ExtendedIdling:
                            case Enums.MessageType.HarshBraking:
                            case Enums.MessageType.ExtremeBraking:
                            case Enums.MessageType.HarshAcceleration:
                            case Enums.MessageType.ExtremeAcceleration:
                            case Enums.MessageType.SeatBelt:
                                string speedDuration = "";
                                if (Enums.MessageType.Idling == msgType || Enums.MessageType.ExtendedIdling == msgType)
                                    speedDuration = Util.PairFindValue(Const.keyIdleDuration, customProp);
                                else
                                    speedDuration = Util.PairFindValue(Const.keyDuration, customProp);
                                if (speedDuration != "")
                                {
                                    try
                                    {
                                        ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(speedDuration) * TimeSpan.TicksPerSecond).ToString();
                                    }
                                    catch
                                    {
                                    }
                                }
                                break;
                            case Enums.MessageType.GPSAntenna:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyGPSAntenna, customProp);
                                break;

                            case Enums.MessageType.MDTResponse:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyAnswer, customProp);
                                break;
                            case Enums.MessageType.MDTSpecialMessage:
                                ittr["MsgDetails"] = customProp;
                                break;
                            case Enums.MessageType.MDTTextMessage:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                break;
                            case Enums.MessageType.Status:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyPower, customProp);
                                break;
                            case Enums.MessageType.RFIDCode:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.extFacilityCode, customProp) + "-" +
                                                      Util.PairFindValue(Const.extIDNumber, customProp);
                                break;
                            case Enums.MessageType.UpdateTelusPRLDone:
                                ittr["MsgDetails"] = "PRL=" + Util.PairFindValue(Const.keyPRLNumber, customProp);
                                break;
                            default:// do nothing
                                ittr["MsgDetails"] = " ";
                                break;
                        }
                        // in case of invalid GPS for current message set defaults
                        if (Convert.ToInt16(ittr["ValidGps"]) == 1)
                        {
                            ittr["Latitude"] = 0;
                            ittr["Longitude"] = 0;
                            // ittr["Speed"] = Const.blankValue;
                            ittr["Heading"] = Const.blankValue;
                        }

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgInDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                }
            }
            return dsResult;
        }







        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves messages from history by vehicle Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isSuperUser"></param>
        /// <param name="boxId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="tblLandmarks"></param>
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetMessagesFromHistoryByBoxId_NewTZ(int userId,
                                                     bool isSuperUser,
                                                     int boxId,
                                                     DateTime from,
                                                     DateTime to,
                                                     bool includeCoordinate,
                                                     bool includeSensor,
                                                     bool includePositionUpdate,
                                                     bool includeInvalidGps,
                                                     Int16 DclId,
                                                     string lang,
                                                     string msgList,
                                                     string sqlTopRecords,
                                                     string vehicleIdparam,
                                                     string LicensePlateparam,
                                                     string vehicleDescriptionparam,
                                                     ref bool requestOverflowed,
                                                     ref int totalSqlRecords)
        {

            //localization settings
            Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

            DataSet dsResult = null;
            requestOverflowed = false;
            string sqlWhere = " WHERE vlfMsgInHst.BoxId=" + boxId;
            //if (from != Const.unassignedDateTime)
            //    sqlWhere += " AND OriginDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
            //if (to != Const.unassignedDateTime)
            //    sqlWhere += " AND OriginDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";



            if (from != Const.unassignedDateTime)
                sqlWhere += " AND OriginDateTime>=dbo.fn_ClientDSTAdjust('" + from + "'," + userId + ")";
            if (to != Const.unassignedDateTime)
                sqlWhere += " AND OriginDateTime<=dbo.fn_ClientDSTAdjust('" + to + "'," + userId + ")";


            if (msgList == "")
            {

                // gb can we have a range for those messages instead of this huge string ??
                sqlWhere += " AND (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speed +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speeding +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoFence +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MBAlarm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobArm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobDisarm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobPanic +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Alarm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTResponse +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTSpecialMessage +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTTextMessage +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxSetup +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxStatus +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.StoredPosition +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Idling +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtendedIdling +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZone +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntenna +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOK +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneIDs +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSetup +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VCROffDelay +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobStatus +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TARMode +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ControllerStatus +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ServiceRequired +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.LowBattery +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxReset +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputOneOn +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputTwoOn +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Odometer +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaShort +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOpen +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BreakIn +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TowAwayAlert +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Collision +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PanicButton +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SleepModeFailure +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AckWithPosition +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.UpdateTelusPRLDone +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MainBatteryDisconnected +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessages +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessagesXS +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendSMC +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendDTC +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.DeviceTestResult +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SMCWriteDone +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTAck +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.FuelTransaction +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReverseExcessSpeed +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReverseExcessDistance +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MovementAlert +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Landmark +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VirtualLandmark +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSpeedStart +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSpeedEnd +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HighRailSpeed +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferInitialPreTrip +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VIN_Changed +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Force_America +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.DallasKey +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Harsh_Drive +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Turn +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReverseHyRailExcessSpeed  +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Spreader;


                if (isSuperUser)
                {
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.RFIDCode;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdate;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdateBantek;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendEEPROMData;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendExtendedSetup;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendExtendedStatus;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferFuelAlarm;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferSensorAlarm;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferStatusReport;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferTemperatureAlarm;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendReeferSetup;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TetheredState;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Heartbeat;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BatteryTrending;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPS_Module_Initialized_OK;
                }

                if (includeSensor)
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Sensor +
                                " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SensorExtended;
                if (includeCoordinate)//"Scheduled Update" is displayed name
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate;
                    else
                        sqlWhere += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate + " AND ValidGps=0)";
                }
                if (includePositionUpdate)
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate;
                    else
                        sqlWhere += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate + " AND ValidGps=0)";
                }
                sqlWhere += ")";
            }
            else
            {
                string[] msgTypeId = msgList.Split(',');
                if (msgTypeId.Length > 0)
                {
                    sqlWhere += " AND (";
                    for (int i = 0; i < msgTypeId.Length; i++)
                    {
                        sqlWhere += " vlfMsgInHst.BoxMsgInTypeId=" + msgTypeId[i] + " OR ";
                    }

                    sqlWhere = sqlWhere.Substring(0, sqlWhere.Length - 4);
                    sqlWhere += ")";
                }
            }





            if (DclId != -1)
                sqlWhere += " AND DclId=" + DclId.ToString();

            try
            {
                // 1. Prepares SQL statement
                string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone float" +
                    " DECLARE @Unit real" +
                    " DECLARE @DayLightSaving int" +
                    " declare @lang varchar(8) " +
                    "declare @driverName varchar(100),@KeyFobId varchar(100),@CheckTripDriver bit,@DST_Change bit " +
                    " set @DST_Change=0 " +
                    " set @CheckTripDriver=0 " +
                    " set @lang= dbo.udf_UserPreference_Text (" + userId + ", 37) " +
                    " if @lang=null or @lang='' set @lang='en-US' " +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZoneNew +
                    " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference" +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " IF @Unit IS NULL SET @Unit=1" +
                    //" SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving +
                    " IF dbo.fn_GetDSTOffset('" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "')<>dbo.fn_GetDSTOffset('" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "')" +
                    " SET @DST_Change=1" +
                    " SELECT @DayLightSaving=dbo.fn_GetDSTOffset('" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "') " +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " IF @DST_Change=0 SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +
                    //---Get Driver
                    " SELECT     @driverName=LastName +' '+ FirstName , @KeyFobId=vlfDriver.KeyFobId FROM         vlfDriverAssignment INNER JOIN " +
                    " vlfDriver ON vlfDriverAssignment.DriverId = vlfDriver.DriverId  WHERE vlfDriverAssignment.AssignedDateTime <  DATEADD(hour,-@DayLightSaving,'" + from + "')" +
                    " and vlfDriverAssignment.VehicleId=" + vehicleIdparam +
                    " IF  @driverName is null and EXISTS( select driverId from vlfDriverAssignmentHst_1 where  vlfDriverAssignmentHst_1.VehicleId=" + vehicleIdparam + ")" +
                        " set @CheckTripDriver=1";


                string sqlFooterSelect1 = "SELECT " + sqlTopRecords + " vlfMsgInHst.BoxId," + vehicleIdparam + " as VehicleId,'" + LicensePlateparam + "' as LicensePlate,'" + vehicleDescriptionparam + "' as Description, " +
                   "CASE WHEN @DST_Change=1  then DATEADD(minute,(@Timezone * 60) +(dbo.fn_GetDSTOffset(DateTimeReceived)*60) ,DateTimeReceived) ELSE  DATEADD(minute,(@Timezone * 60),DateTimeReceived) END AS DateTimeReceived," +
                   "DclId,vlfBoxMsgInType.BoxMsgInTypeId," +
                   "CASE WHEN BoxMsgInTypeName IS NOT NULL then RTRIM(BoxMsgInTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "convert(nvarchar,ValidGps) AS ValidGps," +
                   "LTRIM(RTRIM(STR(ROUND(Latitude,5),10,5))) AS Latitude," +
                   "LTRIM(RTRIM(STR(ROUND(Longitude,6),10,6))) AS Longitude," +
                   "convert(nvarchar,Heading) AS Heading," +
                   "convert(nvarchar,SensorMask) AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "convert(nvarchar,BlobDataSize) AS BlobDataSize,SequenceNum," +
                        "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + Const.addressNA + "') AS StreetAddress," +
                   "CASE WHEN @DST_Change=1 then DATEADD(minute,(@Timezone * 60) +(dbo.fn_GetDSTOffset(OriginDateTime)*60) ,OriginDateTime) ELSE DATEADD(minute,(@Timezone * 60),OriginDateTime) END AS OriginDateTime," +
                   "CASE WHEN vlfMsgInHst.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHst.Speed * @Unit,1)) END AS Speed," +
                    //"convert(nvarchar,CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END) AS BoxArmed,"+
                   "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed," +
                   "0 AS MsgDirection," + // in
                   "' ' AS Acknowledged," +
                   "' ' AS Scheduled," +
                    //"dbo.GetTripDriver(''," + vehicleIdparam + ",'" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "','" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "') as Driver, " +
                   "case when @CheckTripDriver=1 then dbo.GetTripDriver(ISNULL(dbo.GetStringValueFromCustomProperties(CustomProp, 'DrID'),'')," + vehicleIdparam + ",vlfMsgInHst.OriginDateTime,vlfMsgInHst.OriginDateTime) ELSE ISNULL(@driverName,'') END as Driver, " +
                    "case when @CheckTripDriver=1 then dbo.GetTripDriverKeyFobId(ISNULL(dbo.GetStringValueFromCustomProperties(CustomProp, 'DrID'),'')," + vehicleIdparam + ",vlfMsgInHst.OriginDateTime,vlfMsgInHst.OriginDateTime ) ELSE ISNULL(@KeyFobId,'') END as DriverHIDCard, " +
                    //"dbo.GetKeyFobIdByVehicleId(" + vehicleIdparam + ",'" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "','" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "') as DriverHIDCard, " +
                  "CASE WHEN vlfMsgInHst.RoadSpeed IS NULL THEN 0 WHEN vlfMsgInHst.RoadSpeed = 255 THEN 0 ELSE CASE WHEN vlfMsgInHst.SpeedZoneType = 0 OR vlfMsgInHst.SpeedZoneType = 1 THEN ROUND(vlfMsgInHst.RoadSpeed * @Unit, 0) ELSE ROUND(vlfMsgInHst.RoadSpeed * 1.60934 * @Unit, 0) END END AS RoadSpeed," +
                  "ISNULL(dbo.GetStringValueFromCustomProperties(CustomProp,'RPM'),'') as RPM";


                //string sqlFooter1 = "FROM vlfMsgInHst with (nolock),vlfBoxMsgInType,vlfBoxProtocolType WITH (NOLOCK)" +
                //   sqlWhere +
                //   " AND vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId" +
                //   " AND vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";




                string sqlFooter1 = "FROM vlfMsgInHst with (nolock) " +
                            " inner join vlfBoxMsgInType on  vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId " +
                            " inner join vlfBoxProtocolType on  vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId " +
                   sqlWhere;



                // retrieve all commands/outputs from vlfMsgOutHst
                sqlWhere = "";
                //if (from != Const.unassignedDateTime)
                //    sqlWhere += " AND AssignedDateTime<='" + from + "'";
                //if (to != Const.unassignedDateTime)
                //    sqlWhere += " AND '" + to + "'<=ISNULL(DeletedDateTime,'01/01/2099 01:00:00 AM')";


                //if (from != Const.unassignedDateTime)
                //    sqlWhere += " AND DateTime>='" + from + "'";

                //if (to != Const.unassignedDateTime)
                //    sqlWhere += " AND DateTime<='" + to + "'";



                if (from != Const.unassignedDateTime)
                    sqlWhere += " AND DateTime>=dbo.fn_ClientDSTAdjust('" + from + "'," + userId + ")";

                if (to != Const.unassignedDateTime)
                    sqlWhere += " AND DateTime<=dbo.fn_ClientDSTAdjust('" + to + "'," + userId + ")";


                sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.Ack +
                         " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.MDTAck;
                if (!isSuperUser)
                {
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.WakeUpByte;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.UploadFirmware;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.UploadFirmwareStatus;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.ThirdPartyPacket;
                }
                if (DclId != -1)
                    sqlWhere += " AND DclId=" + DclId.ToString();


                string sqlFooterSelect2 = "SELECT  " + sqlTopRecords + " vlfMsgOutHst.BoxId," + vehicleIdparam + " as VehicleId,'" + LicensePlateparam + "' as LicensePlate,'" + vehicleDescriptionparam + "' as Description, " +
                   //"DATEADD(minute,(@Timezone * 60),DateTime) AS DateTimeReceived,DclId," +
                   "CASE WHEN @DST_Change=1  then DATEADD(minute,(@Timezone* 60) +(dbo.fn_GetDSTOffset(DateTime)* 60) ,DateTime) ELSE DATEADD(minute,(@Timezone * 60),DateTime) END AS DateTimeReceived,DclId," +
                   "vlfBoxCmdOutType.BoxCmdOutTypeId AS BoxMsgInTypeId," +
                   "CASE WHEN BoxCmdOutTypeName IS NOT NULL then RTRIM(BoxCmdOutTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "'" + Const.blankValue + "' AS ValidGps," +
                   "'" + Const.blankValue + "' AS Latitude," +
                   "'" + Const.blankValue + "' AS Longitude," +
                   "'" + Const.blankValue + "' AS Heading," +
                   "'" + Const.blankValue + "' AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "'" + Const.blankValue + "' As BlobDataSize," +
                   "SequenceNum," +
                   "'" + Const.blankValue + "' AS StreetAddress," +
                   "CASE WHEN @DST_Change=1  then DATEADD(hour,(@Timezone* 60) +(dbo.fn_GetDSTOffset(DateTime)* 60) ,DateTime) ELSE DATEADD(minute,(@Timezone * 60),DateTime) END AS OriginDateTime," +
                   "'" + Const.blankValue + "' AS Speed," +
                   "'" + Const.blankValue + "' AS BoxArmed," +
                   "1 AS MsgDirection," + // out
                   "CASE WHEN Acknowledged IS NOT NULL then CASE WHEN RTRIM(Acknowledged) IS NOT NULL then RTRIM(Acknowledged) END ELSE '" + Const.cmdNotAck + "' END AS Acknowledged," +
                   "CASE WHEN Scheduled IS NOT NULL then CASE WHEN Scheduled=0 then 'No' END ELSE 'Yes' END AS Scheduled,'' as Driver,'' as DriverHIDCard," +
                   "0 AS RoadSpeed," +
                   "'' as RPM";
                string sqlFooter2 = "FROM vlfMsgOutHst,vlfBoxCmdOutType,vlfBoxProtocolType WITH (NOLOCK)" +
                   " WHERE vlfMsgOutHst.BoxId=" + boxId +
                   sqlWhere +
                   " AND vlfMsgOutHst.BoxCmdOutTypeId=vlfBoxCmdOutType.BoxCmdOutTypeId" +
                   " AND vlfMsgOutHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";

                if (DclId != -1)
                    sqlWhere += " AND DclId=" + DclId.ToString();

                string sql = "";
                if (msgList == "" && sqlTopRecords == "")
                {
                    sql = sqlHeader + " " + sqlFooterSelect1 + " " + sqlFooter1 +
                       " UNION " + sqlFooterSelect2 + " " + sqlFooter2 +
                       " ORDER BY OriginDateTime DESC";
                }
                else
                {
                    sql = sqlHeader + " " + sqlFooterSelect1 + " " + sqlFooter1 +
                       " ORDER BY OriginDateTime DESC";
                }

                try
                {
                    int sqlTop = GetConfigParameter("ASI", (short)Enums.ConfigurationGroups.Common, "History Max Records", 2000);
                    if (sqlTop != Const.unassignedIntValue)
                    {
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter1));
                        if ((sqlTop - totalSqlRecords) < 0)
                        {
                            requestOverflowed = true;
                            return null;
                        }
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter2));
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
                    throw new DASException("Unable to retieve messages from history" + exp.Message);
                }
                // 2. Executes SQL statement
                dsResult = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                // 1. Add additional column to result dataset
                dsResult.Tables[0].Columns.Add("MsgDetails", typeof(string));
                //int boxId = Const.unassignedIntValue;
                DataSet dsVehicleSensors = null;
                DataSet dsVehicleOutputs = null;
                string fenceDirection = "";

                // 2. Get user-defined sensors info (name,action)
                //[SensorId][SensorName][SensorAction][AlarmLevel]
                DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
                dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);


                //localization Sensors

                Localization local = new Localization(sqlExec.ConnectionString.ToString());

                Box boxCnf = new DB.Box(sqlExec);
                BoxHwDefaultSensorsCfg defSensors = new DB.BoxHwDefaultSensorsCfg(sqlExec);


                Int16 hwTypeId = 0;
                DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
                if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
                {
                    // TODO: Today box has only one HW type
                    hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);


                    if (dsVehicleSensors == null || dsVehicleSensors.Tables.Count == 0 || dsVehicleSensors.Tables[0].Rows.Count == 0)
                        dsVehicleSensors = defSensors.GetSensorsInfoByHwTypeId(hwTypeId);
                }
                else
                {
                    // TODO: write to log (cannot retrieve HW type for specific box)
                }


                if ((lang != "en") && (lang != null))
                {
                    local.LocalizationData(lang, "SensorId", "SensorName", "Sensors", hwTypeId, ref dsVehicleSensors);
                    local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
                }



                // 3. Get user-defined outputs info 
                //[OutputId][OutputName][OutputAction]
                DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
                dsVehicleOutputs = outputs.GetOutputsInfoByBoxId(boxId, userId);

                //localization outputs
                if ((lang != "en") && (lang != null))
                {

                    local.LocalizationData(lang, "OutputId", "OutputName", "Outputs", hwTypeId, ref dsVehicleOutputs);
                    local.LocalizationDataAction(lang, "OutputId", "OutputAction", "Outputs", hwTypeId, ref dsVehicleOutputs);
                }


                //localization Commands
                DB.BoxProtocolTypeCmdOutType cmds = new DB.BoxProtocolTypeCmdOutType(sqlExec);
                DataSet dsVehicleCommands = cmds.GetAllSupportedCommands(boxId, userId);
                if ((lang != "en") && (lang != null))
                    local.LocalizationData(lang, "BoxCmdOutTypeId", "BoxCmdOutTypeName", "CommandType", ref dsVehicleCommands);


                // 4. Resolve vehicle geozones
                DB.VehicleAssignment vehicleAssignment = null;
                DB.VehicleGeozone vehicleGeozone = null;
                DataSet dsGeozonesInfo = null;
                Int64 vehicleId = Const.unassignedIntValue;




                foreach (DataRow ittr in dsResult.Tables[0].Rows)
                {
                    // a simple change which should make the code quicker and more readable 
                    string customProp = ittr["CustomProp"].ToString().TrimEnd();

                    if (Convert.ToInt16(ittr["MsgDirection"]) == Const.MsgOutDirectionValue)// process outgoing messages
                    {
                        switch ((Enums.CommandType)Convert.ToInt16(ittr["BoxMsgInTypeId"]))
                        {
                            case Enums.CommandType.Output:
                                ittr["MsgDetails"] = DB.Output.GetOutputDescription(customProp, dsVehicleOutputs.Tables[0]);
                                break;

                            case Enums.CommandType.MDTTextMessage:
                                ittr["BoxMsgInTypeName"] = Resources.Const.TxtMessage;
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                ittr["MsgDetails"] += Convert.ToChar(13).ToString();
                                ittr["MsgDetails"] += Convert.ToChar(10).ToString();
                                ittr["MsgDetails"] += Resources.Const.TxtMsgAnswers + ":" + Util.PairFindValue(Const.keyAnswer, customProp).Replace("~", ",");
                                break;

                            default:
                                ittr["MsgDetails"] = ittr["BoxMsgInTypeName"];
                                //localization commands
                                if ((lang != "en") && (lang != null))
                                {
                                    foreach (DataRow dr in dsVehicleCommands.Tables[0].Rows)
                                    {
                                        if (Convert.ToInt16(ittr["BoxMsgInTypeId"]) == Convert.ToInt16(dr["BoxCmdOutTypeId"]))
                                        {
                                            ittr["MsgDetails"] = dr["BoxCmdOutTypeName"];
                                            break;
                                        }

                                    }
                                }
                                ittr["BoxMsgInTypeName"] = Resources.Const.Command;
                                break;
                        }
                        ittr["StreetAddress"] = Const.blankValue; // not applicable

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgOutDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                    else // process incomming messages
                    {
                        Enums.MessageType msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);

                        switch (msgType)
                        {
                            case Enums.MessageType.Sensor:
                            case Enums.MessageType.SensorExtended:
                            case Enums.MessageType.Alarm:
                            case Enums.MessageType.MBAlarm:
                            case Enums.MessageType.ServiceRequired:
                                if (dsVehicleSensors != null && dsVehicleSensors.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, dsVehicleSensors.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, null);
                                break;
                            case Enums.MessageType.TetheredState:
                                {
                                    string duration = Util.PairFindValue(Const.keyTetheredState, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = duration.Equals(Const.valON) ? Resources.Const.ON : Resources.Const.OFF;
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.Speeding:      // was Speeding
                                {
                                    string duration = Util.PairFindValue(Const.keySpeedDuration, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.GeoFence:
                                //fenceNo = Util.PairFindValue(Const.keyFenceNo,customProp);
                                //if(fenceNo == null || fenceNo == "")
                                //	fenceNo = "Unknown";

                                fenceDirection = Util.PairFindValue(Const.keyFenceDir, customProp);
                                if (fenceDirection == null || fenceDirection == "")
                                    fenceDirection = Resources.Const.fenceDirection_unknown;
                                else if (Const.valFenceDirIn == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_inbroken;
                                else if (Const.valFenceDirOut == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_outbroken;

                                ittr["MsgDetails"] = fenceDirection;
                                break;
                            case Enums.MessageType.GeoZone:
                                if (vehicleAssignment == null)
                                {
                                    vehicleAssignment = new DB.VehicleAssignment(sqlExec);
                                    vehicleId = Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
                                }
                                if (vehicleGeozone == null)
                                {
                                    vehicleGeozone = new DB.VehicleGeozone(sqlExec);
                                    dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
                                }

                                if (dsGeozonesInfo != null && dsGeozonesInfo.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, dsGeozonesInfo.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, null);
                                break;
                            case Enums.MessageType.IPUpdate:
                                ittr["MsgDetails"] = ittr["CommInfo1"].ToString() + ":" + ittr["CommInfo2"].ToString().TrimEnd();
                                ittr["StreetAddress"] = Const.blankValue;
                                ittr["Speed"] = Const.blankValue;
                                break;
                            case Enums.MessageType.Idling:
                            case Enums.MessageType.ExtendedIdling:
                            case Enums.MessageType.HarshBraking:
                            case Enums.MessageType.ExtremeBraking:
                            case Enums.MessageType.HarshAcceleration:
                            case Enums.MessageType.ExtremeAcceleration:
                            case Enums.MessageType.SeatBelt:
                                string speedDuration = "";
                                if (Enums.MessageType.Idling == msgType || Enums.MessageType.ExtendedIdling == msgType)
                                    speedDuration = Util.PairFindValue(Const.keyIdleDuration, customProp);
                                else
                                    speedDuration = Util.PairFindValue(Const.keyDuration, customProp);
                                if (speedDuration != "")
                                {
                                    try
                                    {
                                        ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(speedDuration) * TimeSpan.TicksPerSecond).ToString();
                                    }
                                    catch
                                    {
                                    }
                                }
                                break;
                            case Enums.MessageType.GPSAntenna:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyGPSAntenna, customProp);
                                break;

                            case Enums.MessageType.MDTResponse:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyAnswer, customProp);
                                break;
                            case Enums.MessageType.MDTSpecialMessage:
                                ittr["MsgDetails"] = customProp;
                                break;
                            case Enums.MessageType.MDTTextMessage:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                break;
                            case Enums.MessageType.Status:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyPower, customProp);
                                break;
                            case Enums.MessageType.RFIDCode:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.extFacilityCode, customProp) + "-" +
                                                      Util.PairFindValue(Const.extIDNumber, customProp);
                                break;
                            case Enums.MessageType.UpdateTelusPRLDone:
                                ittr["MsgDetails"] = "PRL=" + Util.PairFindValue(Const.keyPRLNumber, customProp);
                                break;
                            default:// do nothing
                                ittr["MsgDetails"] = " ";
                                break;
                        }
                        // in case of invalid GPS for current message set defaults
                        if (Convert.ToInt16(ittr["ValidGps"]) == 1)
                        {
                            ittr["Latitude"] = 0;
                            ittr["Longitude"] = 0;
                            // ittr["Speed"] = Const.blankValue;
                            ittr["Heading"] = Const.blankValue;
                        }

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgInDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                }
            }
            return dsResult;
        }


        
        /// <summary>
        /// Retrieves messages from history by vehicle Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isSuperUser"></param>
        /// <param name="boxId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="tblLandmarks"></param>
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetMessagesFromHistoryByBoxId(int userId,
                                                     bool isSuperUser,
                                                     int boxId,
                                                     DateTime from,
                                                     DateTime to,
                                                     bool includeCoordinate,
                                                     bool includeSensor,
                                                     bool includePositionUpdate,
                                                     bool includeInvalidGps,
                                                     Int16 DclId,
                                                     string lang,
                                                     string msgList,
                                                     string sqlTopRecords,
                                                     string vehicleIdparam,
                                                     string LicensePlateparam,
                                                     string vehicleDescriptionparam,
                                                     ref bool requestOverflowed,
                                                     ref int totalSqlRecords)
        {

            //localization settings
            Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

            DataSet dsResult = null;
            requestOverflowed = false;
            string sqlWhere = " WHERE vlfMsgInHst.BoxId=" + boxId;
            //if (from != Const.unassignedDateTime)
            //    sqlWhere += " AND OriginDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
            //if (to != Const.unassignedDateTime)
            //    sqlWhere += " AND OriginDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";



            if (from != Const.unassignedDateTime)
                sqlWhere += " AND OriginDateTime>=dbo.fn_ClientDSTAdjust('" + from + "'," + userId + ")";
            if (to != Const.unassignedDateTime)
                sqlWhere += " AND OriginDateTime<=dbo.fn_ClientDSTAdjust('" + to + "'," + userId + ")";


            if (msgList == "")
            {

                // gb can we have a range for those messages instead of this huge string ??
                sqlWhere += " AND (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speed +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speeding +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoFence +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MBAlarm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobArm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobDisarm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobPanic +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Alarm +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTResponse +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTSpecialMessage +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTTextMessage +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxSetup +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxStatus +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.StoredPosition +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Idling +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtendedIdling +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZone +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntenna +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOK +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneIDs +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSetup +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VCROffDelay +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobStatus +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TARMode +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ControllerStatus +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ServiceRequired +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.LowBattery +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxReset +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputOneOn +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputTwoOn +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Odometer +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaShort +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOpen +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BreakIn +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TowAwayAlert +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Collision +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PanicButton +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SleepModeFailure +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AckWithPosition +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.UpdateTelusPRLDone +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MainBatteryDisconnected +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessages +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessagesXS +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendSMC +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendDTC +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.DeviceTestResult +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SMCWriteDone +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTAck +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.FuelTransaction +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReverseExcessSpeed +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReverseExcessDistance +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MovementAlert +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Landmark +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VirtualLandmark +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSpeedStart +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSpeedEnd +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HighRailSpeed +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferInitialPreTrip +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VIN_Changed +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Force_America +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.DallasKey +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Harsh_Drive +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Turn +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReverseHyRailExcessSpeed +
                     " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Spreader;


                if (isSuperUser)
                {
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.RFIDCode;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdate;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdateBantek;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendEEPROMData;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendExtendedSetup;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendExtendedStatus;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferFuelAlarm;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferSensorAlarm;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferStatusReport;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferTemperatureAlarm;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendReeferSetup;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TetheredState;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Heartbeat;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BatteryTrending;
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPS_Module_Initialized_OK;

                }

                if (includeSensor)
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Sensor +
                                " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SensorExtended;
                if (includeCoordinate)//"Scheduled Update" is displayed name
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate;
                    else
                        sqlWhere += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate + " AND ValidGps=0)";
                }
                if (includePositionUpdate)
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate;
                    else
                        sqlWhere += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate + " AND ValidGps=0)";
                }
                sqlWhere += ")";
            }
            else
            {
                string[] msgTypeId = msgList.Split(',');
                if (msgTypeId.Length > 0)
                {
                    sqlWhere += " AND (";
                    for (int i = 0; i < msgTypeId.Length; i++)
                    {
                        sqlWhere += " vlfMsgInHst.BoxMsgInTypeId=" + msgTypeId[i] + " OR ";
                    }

                    sqlWhere = sqlWhere.Substring(0, sqlWhere.Length - 4);
                    sqlWhere += ")";
                }
            }





            if (DclId != -1)
                sqlWhere += " AND DclId=" + DclId.ToString();

            try
            {
                // 1. Prepares SQL statement
                string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone int" +
                    " DECLARE @Unit real" +
                    " DECLARE @DayLightSaving int" +
                    " declare @lang varchar(8) " +
                    "declare @driverName varchar(100),@KeyFobId varchar(100),@CheckTripDriver bit,@DST_Change bit " +
                    " set @DST_Change=0 " +
                    " set @CheckTripDriver=0 " +
                    " set @lang= dbo.udf_UserPreference_Text (" + userId + ", 37) " +
                    " if @lang=null or @lang='' set @lang='en-US' " +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZone +
                    " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference" +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " IF @Unit IS NULL SET @Unit=1" +
                    //" SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving +
                    " IF dbo.fn_GetDSTOffset('" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "')<>dbo.fn_GetDSTOffset('" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "')" +
                    " SET @DST_Change=1" +
                    " SELECT @DayLightSaving=dbo.fn_GetDSTOffset('" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "') " +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " IF @DST_Change=0 SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +
                    //---Get Driver
                    " SELECT     @driverName=LastName +' '+ FirstName , @KeyFobId=vlfDriver.KeyFobId FROM         vlfDriverAssignment INNER JOIN " +
                    " vlfDriver ON vlfDriverAssignment.DriverId = vlfDriver.DriverId  WHERE vlfDriverAssignment.AssignedDateTime <  DATEADD(hour,-@DayLightSaving,'" + from + "')" +
                    " and vlfDriverAssignment.VehicleId=" + vehicleIdparam +
                    " IF  @driverName is null and EXISTS( select driverId from vlfDriverAssignmentHst_1 where  vlfDriverAssignmentHst_1.VehicleId=" + vehicleIdparam + ")" +
                        " set @CheckTripDriver=1";


                string sqlFooterSelect1 = "SELECT " + sqlTopRecords + " vlfMsgInHst.BoxId," + vehicleIdparam + " as VehicleId,'" + LicensePlateparam + "' as LicensePlate,'" + vehicleDescriptionparam + "' as Description, " +
                   "CASE WHEN @DST_Change=1  then DATEADD(hour,@Timezone +dbo.fn_GetDSTOffset(DateTimeReceived) ,DateTimeReceived) ELSE DATEADD(hour,@Timezone,DateTimeReceived) END AS DateTimeReceived," +
                   "DclId,vlfBoxMsgInType.BoxMsgInTypeId," +
                   "CASE WHEN BoxMsgInTypeName IS NOT NULL then RTRIM(BoxMsgInTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "convert(nvarchar,ValidGps) AS ValidGps," +
                   "LTRIM(RTRIM(STR(ROUND(Latitude,5),10,5))) AS Latitude," +
                   "LTRIM(RTRIM(STR(ROUND(Longitude,6),10,6))) AS Longitude," +
                   "convert(nvarchar,Heading) AS Heading," +
                   "convert(nvarchar,SensorMask) AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "convert(nvarchar,BlobDataSize) AS BlobDataSize,SequenceNum," +
                        "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + Const.addressNA + "') AS StreetAddress," +


                   "CASE WHEN @DST_Change=1 then DATEADD(hour,@Timezone +dbo.fn_GetDSTOffset(OriginDateTime) ,OriginDateTime) ELSE DATEADD(hour,@Timezone,OriginDateTime) END AS OriginDateTime," +
                   "CASE WHEN vlfMsgInHst.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHst.Speed * @Unit,1)) END AS Speed," +
                    //"convert(nvarchar,CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END) AS BoxArmed,"+
                   "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed," +
                   "0 AS MsgDirection," + // in
                   "' ' AS Acknowledged," +
                   "' ' AS Scheduled," +
                    //"dbo.GetTripDriver(''," + vehicleIdparam + ",'" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "','" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "') as Driver, " +
                   "case when @CheckTripDriver=1 then dbo.GetTripDriver(ISNULL(dbo.GetStringValueFromCustomProperties(CustomProp, 'DrID'),'')," + vehicleIdparam + ",vlfMsgInHst.OriginDateTime,vlfMsgInHst.OriginDateTime) ELSE ISNULL(@driverName,'') END as Driver, " +
                    "case when @CheckTripDriver=1 then dbo.GetTripDriverKeyFobId(ISNULL(dbo.GetStringValueFromCustomProperties(CustomProp, 'DrID'),'')," + vehicleIdparam + ",vlfMsgInHst.OriginDateTime,vlfMsgInHst.OriginDateTime ) ELSE ISNULL(@KeyFobId,'') END as DriverHIDCard, " +
                    //"dbo.GetKeyFobIdByVehicleId(" + vehicleIdparam + ",'" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "','" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "') as DriverHIDCard, " +
                  "CASE WHEN vlfMsgInHst.RoadSpeed IS NULL THEN 0 WHEN vlfMsgInHst.RoadSpeed = 255 THEN 0 ELSE CASE WHEN vlfMsgInHst.SpeedZoneType = 0 OR vlfMsgInHst.SpeedZoneType = 1 THEN ROUND(vlfMsgInHst.RoadSpeed * @Unit, 0) ELSE ROUND(vlfMsgInHst.RoadSpeed * 1.60934 * @Unit, 0) END END AS RoadSpeed," +
                  "ISNULL(dbo.GetStringValueFromCustomProperties(CustomProp,'RPM'),'') as RPM";


                //string sqlFooter1 = "FROM vlfMsgInHst with (nolock),vlfBoxMsgInType,vlfBoxProtocolType WITH (NOLOCK)" +
                //   sqlWhere +
                //   " AND vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId" +
                //   " AND vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";




                string sqlFooter1 = "FROM vlfMsgInHst with (nolock) " +
                            " inner join vlfBoxMsgInType on  vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId " +
                            " inner join vlfBoxProtocolType on  vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId " +
                   sqlWhere;



                // retrieve all commands/outputs from vlfMsgOutHst
                sqlWhere = "";
                //if (from != Const.unassignedDateTime)
                //    sqlWhere += " AND AssignedDateTime<='" + from + "'";
                //if (to != Const.unassignedDateTime)
                //    sqlWhere += " AND '" + to + "'<=ISNULL(DeletedDateTime,'01/01/2099 01:00:00 AM')";


                //if (from != Const.unassignedDateTime)
                //    sqlWhere += " AND DateTime>='" + from + "'";

                //if (to != Const.unassignedDateTime)
                //    sqlWhere += " AND DateTime<='" + to + "'";



                if (from != Const.unassignedDateTime)
                    sqlWhere += " AND DateTime>=dbo.fn_ClientDSTAdjust('" + from + "'," + userId + ")";

                if (to != Const.unassignedDateTime)
                    sqlWhere += " AND DateTime<=dbo.fn_ClientDSTAdjust('" + to + "'," + userId + ")";


                sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.Ack +
                         " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.MDTAck;
                if (!isSuperUser)
                {
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.WakeUpByte;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.UploadFirmware;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.UploadFirmwareStatus;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.ThirdPartyPacket;
                }
                if (DclId != -1)
                    sqlWhere += " AND DclId=" + DclId.ToString();


                string sqlFooterSelect2 = "SELECT  " + sqlTopRecords + " vlfMsgOutHst.BoxId," + vehicleIdparam + " as VehicleId,'" + LicensePlateparam + "' as LicensePlate,'" + vehicleDescriptionparam + "' as Description, " +
                   //"DATEADD(hour,@Timezone,DateTime) AS DateTimeReceived,DclId," +
                   "CASE WHEN @DST_Change=1  then DATEADD(hour,@Timezone +dbo.fn_GetDSTOffset(DateTime) ,DateTime) ELSE DATEADD(hour,@Timezone,DateTime) END AS DateTimeReceived,DclId," +
                   "vlfBoxCmdOutType.BoxCmdOutTypeId AS BoxMsgInTypeId," +
                   "CASE WHEN BoxCmdOutTypeName IS NOT NULL then RTRIM(BoxCmdOutTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "'" + Const.blankValue + "' AS ValidGps," +
                   "'" + Const.blankValue + "' AS Latitude," +
                   "'" + Const.blankValue + "' AS Longitude," +
                   "'" + Const.blankValue + "' AS Heading," +
                   "'" + Const.blankValue + "' AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "'" + Const.blankValue + "' As BlobDataSize," +
                   "SequenceNum," +
                   "'" + Const.blankValue + "' AS StreetAddress," +
                   "CASE WHEN @DST_Change=1  then DATEADD(hour,@Timezone +dbo.fn_GetDSTOffset(DateTime) ,DateTime) ELSE DATEADD(hour,@Timezone,DateTime) END AS OriginDateTime," +
                   "'" + Const.blankValue + "' AS Speed," +
                   "'" + Const.blankValue + "' AS BoxArmed," +
                   "1 AS MsgDirection," + // out
                   "CASE WHEN Acknowledged IS NOT NULL then CASE WHEN RTRIM(Acknowledged) IS NOT NULL then RTRIM(Acknowledged) END ELSE '" + Const.cmdNotAck + "' END AS Acknowledged," +
                   "CASE WHEN Scheduled IS NOT NULL then CASE WHEN Scheduled=0 then 'No' END ELSE 'Yes' END AS Scheduled,'' as Driver,'' as DriverHIDCard," +
                   "0 AS RoadSpeed," +
                   "'' as RPM";
                string sqlFooter2 = "FROM vlfMsgOutHst,vlfBoxCmdOutType,vlfBoxProtocolType WITH (NOLOCK)" +
                   " WHERE vlfMsgOutHst.BoxId=" + boxId +
                   sqlWhere +
                   " AND vlfMsgOutHst.BoxCmdOutTypeId=vlfBoxCmdOutType.BoxCmdOutTypeId" +
                   " AND vlfMsgOutHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";

                if (DclId != -1)
                    sqlWhere += " AND DclId=" + DclId.ToString();

                string sql = "";
                if (msgList == "" && sqlTopRecords == "")
                {
                    sql = sqlHeader + " " + sqlFooterSelect1 + " " + sqlFooter1 +
                       " UNION " + sqlFooterSelect2 + " " + sqlFooter2 +
                       " ORDER BY OriginDateTime DESC";
                }
                else
                {
                    sql = sqlHeader + " " + sqlFooterSelect1 + " " + sqlFooter1 +
                       " ORDER BY OriginDateTime DESC";
                }

                try
                {
                    int sqlTop = GetConfigParameter("ASI", (short)Enums.ConfigurationGroups.Common, "History Max Records", 2000);
                    if (sqlTop != Const.unassignedIntValue)
                    {
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter1));
                        if ((sqlTop - totalSqlRecords) < 0)
                        {
                            requestOverflowed = true;
                            return null;
                        }
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter2));
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
                    throw new DASException("Unable to retieve messages from history" + exp.Message);
                }
                // 2. Executes SQL statement
                dsResult = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                // 1. Add additional column to result dataset
                dsResult.Tables[0].Columns.Add("MsgDetails", typeof(string));
                //int boxId = Const.unassignedIntValue;
                DataSet dsVehicleSensors = null;
                DataSet dsVehicleOutputs = null;
                string fenceDirection = "";

                // 2. Get user-defined sensors info (name,action)
                //[SensorId][SensorName][SensorAction][AlarmLevel]
                DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
                dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);


                //localization Sensors

                Localization local = new Localization(sqlExec.ConnectionString.ToString());

                Box boxCnf = new DB.Box(sqlExec);
                BoxHwDefaultSensorsCfg defSensors = new DB.BoxHwDefaultSensorsCfg(sqlExec);


                Int16 hwTypeId = 0;
                DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
                if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
                {
                    // TODO: Today box has only one HW type
                    hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);


                    if (dsVehicleSensors == null || dsVehicleSensors.Tables.Count == 0 || dsVehicleSensors.Tables[0].Rows.Count == 0)
                        dsVehicleSensors = defSensors.GetSensorsInfoByHwTypeId(hwTypeId);
                }
                else
                {
                    // TODO: write to log (cannot retrieve HW type for specific box)
                }


                if ((lang != "en") && (lang != null))
                {
                    local.LocalizationData(lang, "SensorId", "SensorName", "Sensors", hwTypeId, ref dsVehicleSensors);
                    local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
                }



                // 3. Get user-defined outputs info 
                //[OutputId][OutputName][OutputAction]
                DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
                dsVehicleOutputs = outputs.GetOutputsInfoByBoxId(boxId, userId);

                //localization outputs
                if ((lang != "en") && (lang != null))
                {

                    local.LocalizationData(lang, "OutputId", "OutputName", "Outputs", hwTypeId, ref dsVehicleOutputs);
                    local.LocalizationDataAction(lang, "OutputId", "OutputAction", "Outputs", hwTypeId, ref dsVehicleOutputs);
                }


                //localization Commands
                DB.BoxProtocolTypeCmdOutType cmds = new DB.BoxProtocolTypeCmdOutType(sqlExec);
                DataSet dsVehicleCommands = cmds.GetAllSupportedCommands(boxId, userId);
                if ((lang != "en") && (lang != null))
                    local.LocalizationData(lang, "BoxCmdOutTypeId", "BoxCmdOutTypeName", "CommandType", ref dsVehicleCommands);


                // 4. Resolve vehicle geozones
                DB.VehicleAssignment vehicleAssignment = null;
                DB.VehicleGeozone vehicleGeozone = null;
                DataSet dsGeozonesInfo = null;
                Int64 vehicleId = Const.unassignedIntValue;




                foreach (DataRow ittr in dsResult.Tables[0].Rows)
                {
                    // a simple change which should make the code quicker and more readable 
                    string customProp = ittr["CustomProp"].ToString().TrimEnd();

                    if (Convert.ToInt16(ittr["MsgDirection"]) == Const.MsgOutDirectionValue)// process outgoing messages
                    {
                        switch ((Enums.CommandType)Convert.ToInt16(ittr["BoxMsgInTypeId"]))
                        {
                            case Enums.CommandType.Output:
                                ittr["MsgDetails"] = DB.Output.GetOutputDescription(customProp, dsVehicleOutputs.Tables[0]);
                                break;

                            case Enums.CommandType.MDTTextMessage:
                                ittr["BoxMsgInTypeName"] = Resources.Const.TxtMessage;
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                ittr["MsgDetails"] += Convert.ToChar(13).ToString();
                                ittr["MsgDetails"] += Convert.ToChar(10).ToString();
                                ittr["MsgDetails"] += Resources.Const.TxtMsgAnswers + ":" + Util.PairFindValue(Const.keyAnswer, customProp).Replace("~", ",");
                                break;

                            default:
                                ittr["MsgDetails"] = ittr["BoxMsgInTypeName"];
                                //localization commands
                                if ((lang != "en") && (lang != null))
                                {
                                    foreach (DataRow dr in dsVehicleCommands.Tables[0].Rows)
                                    {
                                        if (Convert.ToInt16(ittr["BoxMsgInTypeId"]) == Convert.ToInt16(dr["BoxCmdOutTypeId"]))
                                        {
                                            ittr["MsgDetails"] = dr["BoxCmdOutTypeName"];
                                            break;
                                        }

                                    }
                                }
                                ittr["BoxMsgInTypeName"] = Resources.Const.Command;
                                break;
                        }
                        ittr["StreetAddress"] = Const.blankValue; // not applicable

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgOutDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                    else // process incomming messages
                    {
                        Enums.MessageType msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);

                        switch (msgType)
                        {
                            case Enums.MessageType.Sensor:
                            case Enums.MessageType.SensorExtended:
                            case Enums.MessageType.Alarm:
                            case Enums.MessageType.MBAlarm:
                            case Enums.MessageType.ServiceRequired:
                                if (dsVehicleSensors != null && dsVehicleSensors.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, dsVehicleSensors.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, null);
                                break;
                            case Enums.MessageType.TetheredState:
                                {
                                    string duration = Util.PairFindValue(Const.keyTetheredState, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = duration.Equals(Const.valON) ? Resources.Const.ON : Resources.Const.OFF;
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.Speeding:      // was Speeding
                                {
                                    string duration = Util.PairFindValue(Const.keySpeedDuration, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.GeoFence:
                                //fenceNo = Util.PairFindValue(Const.keyFenceNo,customProp);
                                //if(fenceNo == null || fenceNo == "")
                                //	fenceNo = "Unknown";

                                fenceDirection = Util.PairFindValue(Const.keyFenceDir, customProp);
                                if (fenceDirection == null || fenceDirection == "")
                                    fenceDirection = Resources.Const.fenceDirection_unknown;
                                else if (Const.valFenceDirIn == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_inbroken;
                                else if (Const.valFenceDirOut == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_outbroken;

                                ittr["MsgDetails"] = fenceDirection;
                                break;
                            case Enums.MessageType.GeoZone:
                                if (vehicleAssignment == null)
                                {
                                    vehicleAssignment = new DB.VehicleAssignment(sqlExec);
                                    vehicleId = Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
                                }
                                if (vehicleGeozone == null)
                                {
                                    vehicleGeozone = new DB.VehicleGeozone(sqlExec);
                                    dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
                                }

                                if (dsGeozonesInfo != null && dsGeozonesInfo.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, dsGeozonesInfo.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, null);
                                break;
                            case Enums.MessageType.IPUpdate:
                                ittr["MsgDetails"] = ittr["CommInfo1"].ToString() + ":" + ittr["CommInfo2"].ToString().TrimEnd();
                                ittr["StreetAddress"] = Const.blankValue;
                                ittr["Speed"] = Const.blankValue;
                                break;
                            case Enums.MessageType.Idling:
                            case Enums.MessageType.ExtendedIdling:
                            case Enums.MessageType.HarshBraking:
                            case Enums.MessageType.ExtremeBraking:
                            case Enums.MessageType.HarshAcceleration:
                            case Enums.MessageType.ExtremeAcceleration:
                            case Enums.MessageType.SeatBelt:
                                string speedDuration = "";
                                if (Enums.MessageType.Idling == msgType || Enums.MessageType.ExtendedIdling == msgType)
                                    speedDuration = Util.PairFindValue(Const.keyIdleDuration, customProp);
                                else
                                    speedDuration = Util.PairFindValue(Const.keyDuration, customProp);
                                if (speedDuration != "")
                                {
                                    try
                                    {
                                        ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(speedDuration) * TimeSpan.TicksPerSecond).ToString();
                                    }
                                    catch
                                    {
                                    }
                                }
                                break;
                            case Enums.MessageType.GPSAntenna:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyGPSAntenna, customProp);
                                break;

                            case Enums.MessageType.MDTResponse:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyAnswer, customProp);
                                break;
                            case Enums.MessageType.MDTSpecialMessage:
                                ittr["MsgDetails"] = customProp;
                                break;
                            case Enums.MessageType.MDTTextMessage:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                break;
                            case Enums.MessageType.Status:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyPower, customProp);
                                break;
                            case Enums.MessageType.RFIDCode:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.extFacilityCode, customProp) + "-" +
                                                      Util.PairFindValue(Const.extIDNumber, customProp);
                                break;
                            case Enums.MessageType.UpdateTelusPRLDone:
                                ittr["MsgDetails"] = "PRL=" + Util.PairFindValue(Const.keyPRLNumber, customProp);
                                break;
                            default:// do nothing
                                ittr["MsgDetails"] = " ";
                                break;
                        }
                        // in case of invalid GPS for current message set defaults
                        if (Convert.ToInt16(ittr["ValidGps"]) == 1)
                        {
                            ittr["Latitude"] = 0;
                            ittr["Longitude"] = 0;
                            // ittr["Speed"] = Const.blankValue;
                            ittr["Heading"] = Const.blankValue;
                        }

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgInDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                }
            }
            return dsResult;
        }


        ///// <summary>
        ///// Retrieves messages from history by vehicle Id
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="isSuperUser"></param>
        ///// <param name="boxId"></param>
        ///// <param name="from"></param>
        ///// <param name="to"></param>
        ///// <param name="tblLandmarks"></param>
        ///// <param name="includeCoordinate"></param>
        ///// <param name="includeSensor"></param>
        ///// <param name="includePositionUpdate"></param>
        ///// <param name="includeInvalidGps"></param>
        ///// <param name="requestOverflowed"></param>
        ///// <param name="totalSqlRecords"></param>
        ///// <remarks>
        ///// 1. Retrieves only specific messages from the history:
        ///// Coordinate,Sensor,Speed,Fence,PositionUpdate,IPUpdate,KeyFobPanic
        ///// 2. Parse CustomProp field for Sensor and Fence messages, and add info into MsgDetails field
        ///// 3. Incase of IPUpdate message add new IP into MsgDetails field
        ///// </remarks>
        ///// <returns>
        ///// DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        ///// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        ///// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        ///// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],
        ///// [SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]
        ///// </returns>
        ///// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        ///// <exception cref="DASException">Thrown in all other exception cases.</exception>
        //public DataSet GetMessagesFromHistoryByBoxId(int userId,
        //                                             bool isSuperUser,
        //                                             int boxId,
        //                                             DateTime from,
        //                                             DateTime to,
        //                                             bool includeCoordinate,
        //                                             bool includeSensor,
        //                                             bool includePositionUpdate,
        //                                             bool includeInvalidGps,
        //                                             Int16 DclId,
        //                                             string lang,
        //                                             string msgList,
        //                                             string sqlTopRecords,
        //                                             ref bool requestOverflowed,
        //                                             ref int totalSqlRecords)
        //{

        //    //localization settings
        //    Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

        //    DataSet dsResult = null;
        //    requestOverflowed = false;
        //    string sqlWhere = " WHERE vlfMsgInHst.BoxId=" + boxId;
        //    if (from != Const.unassignedDateTime)
        //        sqlWhere += " AND OriginDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
        //    if (to != Const.unassignedDateTime)
        //        sqlWhere += " AND OriginDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";

        //    //if (from != Const.unassignedDateTime)
        //    //    sqlWhere += " AND AssignedDateTime<='" + from + "'";
        //    //if (to != Const.unassignedDateTime)
        //    //    sqlWhere += " AND '" + to + "'<=ISNULL(DeletedDateTime,'01/01/2099 01:00:00 AM')";




        //    if (msgList == "")
        //    {

        //        // gb can we have a range for those messages instead of this huge string ??
        //        sqlWhere += " AND (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speed +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speeding +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoFence +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MBAlarm +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobArm +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobDisarm +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobPanic +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Alarm +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTResponse +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTSpecialMessage +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTTextMessage +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxSetup +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxStatus +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.StoredPosition +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Idling +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtendedIdling +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZone +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntenna +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOK +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneIDs +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSetup +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VCROffDelay +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobStatus +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TARMode +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ControllerStatus +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ServiceRequired +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.LowBattery +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxReset +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputOneOn +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputTwoOn +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Odometer +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaShort +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOpen +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BreakIn +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TowAwayAlert +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Collision +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PanicButton +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SleepModeFailure +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AckWithPosition +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.UpdateTelusPRLDone +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MainBatteryDisconnected +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessages +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessagesXS +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendSMC +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendDTC +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.DeviceTestResult +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SMCWriteDone +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTAck +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.FuelTransaction +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReverseExcessSpeed +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReverseExcessDistance +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MovementAlert +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Landmark +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VirtualLandmark +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSpeedStart +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSpeedEnd +
        //             " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HighRailSpeed;


        //        if (isSuperUser)
        //        {
        //            sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.RFIDCode;
        //            sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdate;
        //            sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdateBantek;
        //            sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendEEPROMData;
        //            sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendExtendedSetup;
        //            sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendExtendedStatus;
        //            sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferFuelAlarm;
        //            sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferSensorAlarm;
        //            sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferStatusReport;
        //            sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferTemperatureAlarm;
        //            sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SendReeferSetup;
        //            sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TetheredState;
        //            sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Heartbeat;
        //        }

        //        if (includeSensor)
        //            sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Sensor +
        //                        " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SensorExtended;
        //        if (includeCoordinate)//"Scheduled Update" is displayed name
        //        {
        //            if (includeInvalidGps)
        //                sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate;
        //            else
        //                sqlWhere += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate + " AND ValidGps=0)";
        //        }
        //        if (includePositionUpdate)
        //        {
        //            if (includeInvalidGps)
        //                sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate;
        //            else
        //                sqlWhere += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate + " AND ValidGps=0)";
        //        }
        //        sqlWhere += ")";
        //    }
        //    else
        //    {
        //        string[] msgTypeId = msgList.Split(',');
        //        if (msgTypeId.Length > 0)
        //        {
        //            sqlWhere += " AND (";
        //            for (int i = 0; i < msgTypeId.Length; i++)
        //            {
        //                sqlWhere += " vlfMsgInHst.BoxMsgInTypeId=" + msgTypeId[i] + " OR ";
        //            }

        //            sqlWhere = sqlWhere.Substring(0, sqlWhere.Length - 4);
        //            sqlWhere += ")";
        //        }
        //    }





        //    if (DclId != -1)
        //        sqlWhere += " AND DclId=" + DclId.ToString();

        //    try
        //    {
        //        // 1. Prepares SQL statement
        //        string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone int" +
        //            " DECLARE @Unit real" +
        //            " DECLARE @DayLightSaving int" +
        //            " declare @lang varchar(8) " +
        //            " set @lang= dbo.udf_UserPreference_Text (" + userId + ", 37) " +
        //            " if @lang=null or @lang='' set @lang='en-US' " +
        //            " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
        //            " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZone +
        //            " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference" +
        //            " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
        //            " IF @Timezone IS NULL SET @Timezone=0" +
        //            " IF @Unit IS NULL SET @Unit=1" +
        //            //" SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving +
        //            " SELECT @DayLightSaving=dbo.fn_GetDSTOffset('" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "') " +
        //            " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
        //            " SET @Timezone= @Timezone + @DayLightSaving" +
        //            " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0";

        //        string sqlFooterSelect1 = "SELECT " + sqlTopRecords + " vlfMsgInHst.BoxId,vlfVehicleAssignment.VehicleId,vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.Description, " +
        //           "CASE WHEN DateTimeReceived IS NULL then '' ELSE DATEADD(hour,@Timezone,DateTimeReceived) END AS DateTimeReceived," +
        //           "DclId,vlfBoxMsgInType.BoxMsgInTypeId," +
        //           "CASE WHEN BoxMsgInTypeName IS NOT NULL then RTRIM(BoxMsgInTypeName) END AS BoxMsgInTypeName," +
        //           "vlfBoxProtocolType.BoxProtocolTypeId," +
        //           "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
        //           "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
        //           "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
        //           "convert(nvarchar,ValidGps) AS ValidGps," +
        //           "LTRIM(RTRIM(STR(ROUND(Latitude,5),10,5))) AS Latitude," +
        //           "LTRIM(RTRIM(STR(ROUND(Longitude,6),10,6))) AS Longitude," +
        //           "convert(nvarchar,Heading) AS Heading," +
        //           "convert(nvarchar,SensorMask) AS SensorMask," +
        //           "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
        //           "convert(nvarchar,BlobDataSize) AS BlobDataSize,SequenceNum," +
        //                "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + Const.addressNA + "') AS StreetAddress," +


        //           "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,OriginDateTime) END AS OriginDateTime," +
        //           "CASE WHEN vlfMsgInHst.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHst.Speed * @Unit,1)) END AS Speed," +
        //            //"convert(nvarchar,CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END) AS BoxArmed,"+
        //           "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed," +
        //           "0 AS MsgDirection," + // in
        //           "' ' AS Acknowledged," +
        //           "' ' AS Scheduled";

        //        string sqlFooter1 = "FROM vlfMsgInHst with (nolock),vlfBoxMsgInType,vlfBoxProtocolType,vlfVehicleAssignment,vlfVehicleInfo WITH (NOLOCK)" +
        //           sqlWhere +
        //           " AND vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId" +
        //           " AND vlfMsgInHst.BoxId=vlfVehicleAssignment.BoxId" +
        //           " AND vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId" +
        //           " AND vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";

        //        // retrieve all commands/outputs from vlfMsgOutHst
        //        sqlWhere = "";
        //        //if (from != Const.unassignedDateTime)
        //        //    sqlWhere += " AND AssignedDateTime<='" + from + "'";
        //        //if (to != Const.unassignedDateTime)
        //        //    sqlWhere += " AND '" + to + "'<=ISNULL(DeletedDateTime,'01/01/2099 01:00:00 AM')";


        //        if (from != Const.unassignedDateTime)
        //            sqlWhere += " AND DateTime>='" + from + "'";

        //        if (to != Const.unassignedDateTime)
        //            sqlWhere += " AND DateTime<='" + to + "'";


        //        sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.Ack +
        //                 " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.MDTAck;
        //        if (!isSuperUser)
        //        {
        //            sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.WakeUpByte;
        //            sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.UploadFirmware;
        //            sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.UploadFirmwareStatus;
        //            sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.ThirdPartyPacket;
        //        }
        //        if (DclId != -1)
        //            sqlWhere += " AND DclId=" + DclId.ToString();


        //        string sqlFooterSelect2 = "SELECT  " + sqlTopRecords + " vlfMsgOutHst.BoxId,vlfVehicleAssignment.VehicleId,vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.Description, " +
        //           "DATEADD(hour,@Timezone,DateTime) AS DateTimeReceived,DclId," +
        //           "vlfBoxCmdOutType.BoxCmdOutTypeId AS BoxMsgInTypeId," +
        //           "CASE WHEN BoxCmdOutTypeName IS NOT NULL then RTRIM(BoxCmdOutTypeName) END AS BoxMsgInTypeName," +
        //           "vlfBoxProtocolType.BoxProtocolTypeId," +
        //           "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
        //           "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
        //           "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
        //           "'" + Const.blankValue + "' AS ValidGps," +
        //           "'" + Const.blankValue + "' AS Latitude," +
        //           "'" + Const.blankValue + "' AS Longitude," +
        //           "'" + Const.blankValue + "' AS Heading," +
        //           "'" + Const.blankValue + "' AS SensorMask," +
        //           "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
        //           "'" + Const.blankValue + "' As BlobDataSize," +
        //           "SequenceNum," +
        //           "'" + Const.blankValue + "' AS StreetAddress," +
        //           "DATEADD(hour,@Timezone,DateTime) AS OriginDateTime," +
        //           "'" + Const.blankValue + "' AS Speed," +
        //           "'" + Const.blankValue + "' AS BoxArmed," +
        //           "1 AS MsgDirection," + // out
        //           "CASE WHEN Acknowledged IS NOT NULL then CASE WHEN RTRIM(Acknowledged) IS NOT NULL then RTRIM(Acknowledged) END ELSE '" + Const.cmdNotAck + "' END AS Acknowledged," +
        //           "CASE WHEN Scheduled IS NOT NULL then CASE WHEN Scheduled=0 then 'No' END ELSE 'Yes' END AS Scheduled";
        //        string sqlFooter2 = "FROM vlfMsgOutHst,vlfBoxCmdOutType,vlfBoxProtocolType,vlfVehicleAssignment,vlfVehicleInfo WITH (NOLOCK)" +
        //           " WHERE vlfMsgOutHst.BoxId=" + boxId +
        //           sqlWhere +
        //           " AND vlfMsgOutHst.BoxCmdOutTypeId=vlfBoxCmdOutType.BoxCmdOutTypeId" +
        //           " AND vlfMsgOutHst.BoxId=vlfVehicleAssignment.BoxId" +
        //           " AND vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId" +
        //           " AND vlfMsgOutHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";

        //        if (DclId != -1)
        //            sqlWhere += " AND DclId=" + DclId.ToString();

        //        string sql = "";
        //        if (msgList == "" && sqlTopRecords == "")
        //        {
        //            sql = sqlHeader + " " + sqlFooterSelect1 + " " + sqlFooter1 +
        //               " UNION " + sqlFooterSelect2 + " " + sqlFooter2 +
        //               " ORDER BY OriginDateTime DESC";
        //        }
        //        else
        //        {
        //            sql = sqlHeader + " " + sqlFooterSelect1 + " " + sqlFooter1 +
        //               " ORDER BY OriginDateTime DESC";
        //        }

        //        try
        //        {
        //            int sqlTop = GetConfigParameter("ASI", (short)Enums.ConfigurationGroups.Common, "History Max Records", 2000);
        //            if (sqlTop != Const.unassignedIntValue)
        //            {
        //                totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter1));
        //                if ((sqlTop - totalSqlRecords) < 0)
        //                {
        //                    requestOverflowed = true;
        //                    return null;
        //                }
        //                totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter2));
        //                if ((sqlTop - totalSqlRecords) < 0)
        //                {
        //                    requestOverflowed = true;
        //                    return null;
        //                }
        //            }
        //        }
        //        catch (DASDbConnectionClosed exCnn)
        //        {
        //            throw new DASDbConnectionClosed(exCnn.Message);
        //        }
        //        catch (Exception exp)
        //        {
        //            throw new DASException("Unable to retieve messages from history" + exp.Message);
        //        }
        //        // 2. Executes SQL statement
        //        dsResult = sqlExec.SQLExecuteDataset(sql);
        //    }
        //    catch (SqlException objException)
        //    {
        //        Util.ProcessDbException("Unable to retieve messages from history", objException);
        //    }
        //    catch (DASDbConnectionClosed exCnn)
        //    {
        //        throw new DASDbConnectionClosed(exCnn.Message);
        //    }
        //    catch (Exception objException)
        //    {
        //        throw new DASException("Unable to retieve messages from history" + objException.Message);
        //    }
        //    if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
        //    {
        //        // 1. Add additional column to result dataset
        //        dsResult.Tables[0].Columns.Add("MsgDetails", typeof(string));
        //        //int boxId = Const.unassignedIntValue;
        //        DataSet dsVehicleSensors = null;
        //        DataSet dsVehicleOutputs = null;
        //        string fenceDirection = "";

        //        // 2. Get user-defined sensors info (name,action)
        //        //[SensorId][SensorName][SensorAction][AlarmLevel]
        //        DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
        //        dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);


        //        //localization Sensors

        //        Localization local = new Localization(sqlExec.ConnectionString.ToString());

        //        Box boxCnf = new DB.Box(sqlExec);
        //        BoxHwDefaultSensorsCfg defSensors = new DB.BoxHwDefaultSensorsCfg(sqlExec);


        //        Int16 hwTypeId = 0;
        //        DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
        //        if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
        //        {
        //            // TODO: Today box has only one HW type
        //            hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);


        //            if (dsVehicleSensors == null || dsVehicleSensors.Tables.Count == 0 || dsVehicleSensors.Tables[0].Rows.Count == 0)
        //                dsVehicleSensors = defSensors.GetSensorsInfoByHwTypeId(hwTypeId);
        //        }
        //        else
        //        {
        //            // TODO: write to log (cannot retrieve HW type for specific box)
        //        }


        //        if ((lang != "en") && (lang != null))
        //        {
        //            local.LocalizationData(lang, "SensorId", "SensorName", "Sensors", hwTypeId, ref dsVehicleSensors);
        //            local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
        //        }



        //        // 3. Get user-defined outputs info 
        //        //[OutputId][OutputName][OutputAction]
        //        DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
        //        dsVehicleOutputs = outputs.GetOutputsInfoByBoxId(boxId, userId);

        //        //localization outputs
        //        if ((lang != "en") && (lang != null))
        //        {

        //            local.LocalizationData(lang, "OutputId", "OutputName", "Outputs", hwTypeId, ref dsVehicleOutputs);
        //            local.LocalizationDataAction(lang, "OutputId", "OutputAction", "Outputs", hwTypeId, ref dsVehicleOutputs);
        //        }


        //        //localization Commands
        //        DB.BoxProtocolTypeCmdOutType cmds = new DB.BoxProtocolTypeCmdOutType(sqlExec);
        //        DataSet dsVehicleCommands = cmds.GetAllSupportedCommands(boxId, userId);
        //        if ((lang != "en") && (lang != null))
        //            local.LocalizationData(lang, "BoxCmdOutTypeId", "BoxCmdOutTypeName", "CommandType", ref dsVehicleCommands);


        //        // 4. Resolve vehicle geozones
        //        DB.VehicleAssignment vehicleAssignment = null;
        //        DB.VehicleGeozone vehicleGeozone = null;
        //        DataSet dsGeozonesInfo = null;
        //        Int64 vehicleId = Const.unassignedIntValue;




        //        foreach (DataRow ittr in dsResult.Tables[0].Rows)
        //        {
        //            // a simple change which should make the code quicker and more readable 
        //            string customProp = ittr["CustomProp"].ToString().TrimEnd();

        //            if (Convert.ToInt16(ittr["MsgDirection"]) == Const.MsgOutDirectionValue)// process outgoing messages
        //            {
        //                switch ((Enums.CommandType)Convert.ToInt16(ittr["BoxMsgInTypeId"]))
        //                {
        //                    case Enums.CommandType.Output:
        //                        ittr["MsgDetails"] = DB.Output.GetOutputDescription(customProp, dsVehicleOutputs.Tables[0]);
        //                        break;

        //                    case Enums.CommandType.MDTTextMessage:
        //                        ittr["BoxMsgInTypeName"] = Resources.Const.TxtMessage;
        //                        ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
        //                        ittr["MsgDetails"] += Convert.ToChar(13).ToString();
        //                        ittr["MsgDetails"] += Convert.ToChar(10).ToString();
        //                        ittr["MsgDetails"] += Resources.Const.TxtMsgAnswers + ":" + Util.PairFindValue(Const.keyAnswer, customProp).Replace("~", ",");
        //                        break;

        //                    default:
        //                        ittr["MsgDetails"] = ittr["BoxMsgInTypeName"];
        //                        //localization commands
        //                        if ((lang != "en") && (lang != null))
        //                        {
        //                            foreach (DataRow dr in dsVehicleCommands.Tables[0].Rows)
        //                            {
        //                                if (Convert.ToInt16(ittr["BoxMsgInTypeId"]) == Convert.ToInt16(dr["BoxCmdOutTypeId"]))
        //                                {
        //                                    ittr["MsgDetails"] = dr["BoxCmdOutTypeName"];
        //                                    break;
        //                                }

        //                            }
        //                        }
        //                        ittr["BoxMsgInTypeName"] = Resources.Const.Command;
        //                        break;
        //                }
        //                ittr["StreetAddress"] = Const.blankValue; // not applicable

        //                // add message direction to message type
        //                ittr["BoxMsgInTypeName"] = Const.MsgOutDirectionSign + " " + ittr["BoxMsgInTypeName"];
        //            }
        //            else // process incomming messages
        //            {
        //                Enums.MessageType msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);

        //                switch (msgType)
        //                {
        //                    case Enums.MessageType.Sensor:
        //                    case Enums.MessageType.SensorExtended:
        //                    case Enums.MessageType.Alarm:
        //                    case Enums.MessageType.MBAlarm:
        //                    case Enums.MessageType.ServiceRequired:
        //                        if (dsVehicleSensors != null && dsVehicleSensors.Tables.Count > 0)
        //                            ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, dsVehicleSensors.Tables[0]);
        //                        else
        //                            ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, null);
        //                        break;
        //                    case Enums.MessageType.TetheredState:
        //                        {
        //                            string duration = Util.PairFindValue(Const.keyTetheredState, customProp);
        //                            if (duration != "")
        //                            {
        //                                try
        //                                {
        //                                    ittr["MsgDetails"] = duration.Equals(Const.valON) ? Resources.Const.ON : Resources.Const.OFF;
        //                                }
        //                                catch
        //                                {
        //                                }
        //                            }
        //                            break;
        //                        }
        //                    case Enums.MessageType.Speeding:      // was Speeding
        //                        {
        //                            string duration = Util.PairFindValue(Const.keySpeedDuration, customProp);
        //                            if (duration != "")
        //                            {
        //                                try
        //                                {
        //                                    ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
        //                                }
        //                                catch
        //                                {
        //                                }
        //                            }
        //                            break;
        //                        }
        //                    case Enums.MessageType.GeoFence:
        //                        //fenceNo = Util.PairFindValue(Const.keyFenceNo,customProp);
        //                        //if(fenceNo == null || fenceNo == "")
        //                        //	fenceNo = "Unknown";

        //                        fenceDirection = Util.PairFindValue(Const.keyFenceDir, customProp);
        //                        if (fenceDirection == null || fenceDirection == "")
        //                            fenceDirection = Resources.Const.fenceDirection_unknown;
        //                        else if (Const.valFenceDirIn == fenceDirection)
        //                            fenceDirection = Resources.Const.fenceDirection_inbroken;
        //                        else if (Const.valFenceDirOut == fenceDirection)
        //                            fenceDirection = Resources.Const.fenceDirection_outbroken;

        //                        ittr["MsgDetails"] = fenceDirection;
        //                        break;
        //                    case Enums.MessageType.GeoZone:
        //                        if (vehicleAssignment == null)
        //                        {
        //                            vehicleAssignment = new DB.VehicleAssignment(sqlExec);
        //                            vehicleId = Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
        //                        }
        //                        if (vehicleGeozone == null)
        //                        {
        //                            vehicleGeozone = new DB.VehicleGeozone(sqlExec);
        //                            dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
        //                        }

        //                        if (dsGeozonesInfo != null && dsGeozonesInfo.Tables.Count > 0)
        //                            ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, dsGeozonesInfo.Tables[0]);
        //                        else
        //                            ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, null);
        //                        break;
        //                    case Enums.MessageType.IPUpdate:
        //                        ittr["MsgDetails"] = ittr["CommInfo1"].ToString() + ":" + ittr["CommInfo2"].ToString().TrimEnd();
        //                        ittr["StreetAddress"] = Const.blankValue;
        //                        ittr["Speed"] = Const.blankValue;
        //                        break;
        //                    case Enums.MessageType.Idling:
        //                    case Enums.MessageType.ExtendedIdling:
        //                    case Enums.MessageType.HarshBraking:
        //                    case Enums.MessageType.ExtremeBraking:
        //                    case Enums.MessageType.HarshAcceleration:
        //                    case Enums.MessageType.ExtremeAcceleration:
        //                    case Enums.MessageType.SeatBelt:
        //                        string speedDuration = "";
        //                        if (Enums.MessageType.Idling == msgType || Enums.MessageType.ExtendedIdling == msgType)
        //                            speedDuration = Util.PairFindValue(Const.keyIdleDuration, customProp);
        //                        else
        //                            speedDuration = Util.PairFindValue(Const.keyDuration, customProp);
        //                        if (speedDuration != "")
        //                        {
        //                            try
        //                            {
        //                                ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(speedDuration) * TimeSpan.TicksPerSecond).ToString();
        //                            }
        //                            catch
        //                            {
        //                            }
        //                        }
        //                        break;
        //                    case Enums.MessageType.GPSAntenna:
        //                        ittr["MsgDetails"] = Util.PairFindValue(Const.keyGPSAntenna, customProp);
        //                        break;

        //                    case Enums.MessageType.MDTResponse:
        //                        ittr["MsgDetails"] = Util.PairFindValue(Const.keyAnswer, customProp);
        //                        break;
        //                    case Enums.MessageType.MDTSpecialMessage:
        //                        ittr["MsgDetails"] = customProp;
        //                        break;
        //                    case Enums.MessageType.MDTTextMessage:
        //                        ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
        //                        break;
        //                    case Enums.MessageType.Status:
        //                        ittr["MsgDetails"] = Util.PairFindValue(Const.keyPower, customProp);
        //                        break;
        //                    case Enums.MessageType.RFIDCode:
        //                        ittr["MsgDetails"] = Util.PairFindValue(Const.extFacilityCode, customProp) + "-" +
        //                                              Util.PairFindValue(Const.extIDNumber, customProp);
        //                        break;
        //                    case Enums.MessageType.UpdateTelusPRLDone:
        //                        ittr["MsgDetails"] = "PRL=" + Util.PairFindValue(Const.keyPRLNumber, customProp);
        //                        break;
        //                    default:// do nothing
        //                        ittr["MsgDetails"] = " ";
        //                        break;
        //                }
        //                // in case of invalid GPS for current message set defaults
        //                if (Convert.ToInt16(ittr["ValidGps"]) == 1)
        //                {
        //                    ittr["Latitude"] = 0;
        //                    ittr["Longitude"] = 0;
        //                    // ittr["Speed"] = Const.blankValue;
        //                    ittr["Heading"] = Const.blankValue;
        //                }

        //                // add message direction to message type
        //                ittr["BoxMsgInTypeName"] = Const.MsgInDirectionSign + " " + ittr["BoxMsgInTypeName"];
        //            }
        //        }
        //    }
        //    return dsResult;
        //}



        // Changes for TimeZone Feature start
        public DataSet GetMessagesFromHistoryByBoxId_Tmp_NewTZ(int userId,
                                                    bool isSuperUser,
                                                    int boxId,
                                                    DateTime from,
                                                    DateTime to,
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

            //localization settings
            Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

            DataSet dsResult = null;
            requestOverflowed = false;
            string sqlWhere = " WHERE vlfMsgInHstBrickman201107c_2.BoxId=" + boxId;
            if (from != Const.unassignedDateTime)
                sqlWhere += " AND OriginDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
            if (to != Const.unassignedDateTime)
                sqlWhere += " AND OriginDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";

            //if (from != Const.unassignedDateTime)
            //    sqlWhere += " AND AssignedDateTime<='" + from + "'";
            //if (to != Const.unassignedDateTime)
            //    sqlWhere += " AND '" + to + "'<=ISNULL(DeletedDateTime,'01/01/2099 01:00:00 AM')";




            if (msgList == "")
            {

                // gb can we have a range for those messages instead of this huge string ??
                sqlWhere += " AND (vlfMsgInHstBrickman201107c_2.BoxMsgInTypeId=" + (short)Enums.MessageType.Speed +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Speeding +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoFence +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MBAlarm +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobArm +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobDisarm +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobPanic +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Alarm +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTResponse +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTSpecialMessage +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTTextMessage +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxSetup +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxStatus +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.StoredPosition +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Idling +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtendedIdling +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZone +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntenna +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOK +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneIDs +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSetup +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.VCROffDelay +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobStatus +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.TARMode +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ControllerStatus +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ServiceRequired +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.LowBattery +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxReset +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputOneOn +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputTwoOn +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Odometer +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaShort +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOpen +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.BreakIn +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.TowAwayAlert +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Collision +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.PanicButton +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SleepModeFailure +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.AckWithPosition +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.UpdateTelusPRLDone +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MainBatteryDisconnected +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessages +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessagesXS +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SendSMC +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SendDTC +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.DeviceTestResult +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SMCWriteDone +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTAck +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.FuelTransaction +
                    " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MovementAlert;

                if (isSuperUser)
                {
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.RFIDCode;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdate;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdateBantek;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SendEEPROMData;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SendExtendedSetup;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SendExtendedStatus;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferFuelAlarm;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferSensorAlarm;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferStatusReport;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferTemperatureAlarm;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SendReeferSetup;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.TetheredState;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Heartbeat;
                }

                if (includeSensor)
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Sensor +
                                " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SensorExtended;
                if (includeCoordinate)//"Scheduled Update" is displayed name
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate;
                    else
                        sqlWhere += " OR (vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate + " AND ValidGps=0)";
                }
                if (includePositionUpdate)
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate;
                    else
                        sqlWhere += " OR (vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate + " AND ValidGps=0)";
                }
                sqlWhere += ")";
            }
            else
            {
                string[] msgTypeId = msgList.Split(',');
                if (msgTypeId.Length > 0)
                {
                    sqlWhere += " AND (";
                    for (int i = 0; i < msgTypeId.Length; i++)
                    {
                        sqlWhere += " vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + msgTypeId[i] + " OR ";
                    }

                    sqlWhere = sqlWhere.Substring(0, sqlWhere.Length - 4);
                    sqlWhere += ")";
                }
            }





            if (DclId != -1)
                sqlWhere += " AND DclId=" + DclId.ToString();

            try
            {
                // 1. Prepares SQL statement
                string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone float" +
                   " DECLARE @Unit real" +
                   " DECLARE @DayLightSaving int" +
                   " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                   " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZoneNew +
                   " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference" +
                   " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
                   " IF @Timezone IS NULL SET @Timezone=0" +
                   " IF @Unit IS NULL SET @Unit=1" +
                   " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving +
                   " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                   " SET @Timezone= @Timezone + @DayLightSaving" +
                   " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0";


                string sqlFooterSelect1 = "SELECT " + sqlTopRecords + " vlfMsgInHstBrickman201107c.BoxId,vlfVehicleAssignment.VehicleId,vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.Description, " +
                   "CASE WHEN DateTimeReceived IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),DateTimeReceived) END AS DateTimeReceived," +
                   "DclId,vlfBoxMsgInType.BoxMsgInTypeId," +
                   "CASE WHEN BoxMsgInTypeName IS NOT NULL then RTRIM(BoxMsgInTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "convert(nvarchar,ValidGps) AS ValidGps," +
                   "LTRIM(RTRIM(STR(ROUND(Latitude,5),10,5))) AS Latitude," +
                   "LTRIM(RTRIM(STR(ROUND(Longitude,6),10,6))) AS Longitude," +
                   "convert(nvarchar,Heading) AS Heading," +
                   "convert(nvarchar,SensorMask) AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "convert(nvarchar,BlobDataSize) AS BlobDataSize,SequenceNum," +
                        "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHstBrickman201107c.StreetAddress ELSE CASE WHEN vlfMsgInHstBrickman201107c.NearestLandmark IS NULL then vlfMsgInHstBrickman201107c.StreetAddress ELSE vlfMsgInHstBrickman201107c.NearestLandmark END END,'" + Const.addressNA + "') AS StreetAddress," +


                   "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),OriginDateTime) END AS OriginDateTime," +
                   "CASE WHEN vlfMsgInHstBrickman201107c.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHstBrickman201107c.Speed * @Unit,1)) END AS Speed," +
                    //"convert(nvarchar,CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END) AS BoxArmed,"+
                   "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed," +
                   "0 AS MsgDirection," + // in
                   "' ' AS Acknowledged," +
                   "' ' AS Scheduled";

                string sqlFooter1 = "FROM vlfMsgInHstBrickman201107c with (nolock),vlfBoxMsgInType,vlfBoxProtocolType,vlfVehicleAssignment,vlfVehicleInfo WITH (NOLOCK)" +
                   sqlWhere +
                   " AND vlfMsgInHstBrickman201107c.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId" +
                   " AND vlfMsgInHstBrickman201107c.BoxId=vlfVehicleAssignment.BoxId" +
                   " AND vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId" +
                   " AND vlfMsgInHstBrickman201107c.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";
                // retrieve all commands/outputs from vlfMsgOutHst
                sqlWhere = "";
                //if (from != Const.unassignedDateTime)
                //    sqlWhere += " AND AssignedDateTime<='" + from + "'";
                //if (to != Const.unassignedDateTime)
                //    sqlWhere += " AND '" + to + "'<=ISNULL(DeletedDateTime,'01/01/2099 01:00:00 AM')";


                if (from != Const.unassignedDateTime)
                    sqlWhere += " AND DateTime>='" + from + "'";

                if (to != Const.unassignedDateTime)
                    sqlWhere += " AND DateTime<='" + to + "'";


                sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.Ack +
                         " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.MDTAck;
                if (!isSuperUser)
                {
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.WakeUpByte;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.UploadFirmware;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.UploadFirmwareStatus;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.ThirdPartyPacket;
                }
                if (DclId != -1)
                    sqlWhere += " AND DclId=" + DclId.ToString();


                string sqlFooterSelect2 = "SELECT  " + sqlTopRecords + " vlfMsgOutHst.BoxId,vlfVehicleAssignment.VehicleId,vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.Description, " +
                   "DATEADD(minute,(@Timezone * 60),DateTime) AS DateTimeReceived,DclId," +
                   "vlfBoxCmdOutType.BoxCmdOutTypeId AS BoxMsgInTypeId," +
                   "CASE WHEN BoxCmdOutTypeName IS NOT NULL then RTRIM(BoxCmdOutTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "'" + Const.blankValue + "' AS ValidGps," +
                   "'" + Const.blankValue + "' AS Latitude," +
                   "'" + Const.blankValue + "' AS Longitude," +
                   "'" + Const.blankValue + "' AS Heading," +
                   "'" + Const.blankValue + "' AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "'" + Const.blankValue + "' As BlobDataSize," +
                   "SequenceNum," +
                   "'" + Const.blankValue + "' AS StreetAddress," +
                   "DATEADD(minute,(@Timezone * 60),DateTime) AS OriginDateTime," +
                   "'" + Const.blankValue + "' AS Speed," +
                   "'" + Const.blankValue + "' AS BoxArmed," +
                   "1 AS MsgDirection," + // out
                   "CASE WHEN Acknowledged IS NOT NULL then CASE WHEN RTRIM(Acknowledged) IS NOT NULL then RTRIM(Acknowledged) END ELSE '" + Const.cmdNotAck + "' END AS Acknowledged," +
                   "CASE WHEN Scheduled IS NOT NULL then CASE WHEN Scheduled=0 then 'No' END ELSE 'Yes' END AS Scheduled";
                string sqlFooter2 = "FROM vlfMsgOutHst,vlfBoxCmdOutType,vlfBoxProtocolType,vlfVehicleAssignment,vlfVehicleInfo WITH (NOLOCK)" +
                   " WHERE vlfMsgOutHst.BoxId=" + boxId +
                   sqlWhere +
                   " AND vlfMsgOutHst.BoxCmdOutTypeId=vlfBoxCmdOutType.BoxCmdOutTypeId" +
                   " AND vlfMsgOutHst.BoxId=vlfVehicleAssignment.BoxId" +
                   " AND vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId" +
                   " AND vlfMsgOutHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";
                if (DclId != -1)
                    sqlWhere += " AND DclId=" + DclId.ToString();

                string sql = "";
                if (msgList == "" && sqlTopRecords == "")
                {
                    sql = sqlHeader + " " + sqlFooterSelect1 + " " + sqlFooter1 +
                       " UNION " + sqlFooterSelect2 + " " + sqlFooter2 +
                       " ORDER BY OriginDateTime DESC, DateTimeReceived DESC";
                }
                else
                {
                    sql = sqlHeader + " " + sqlFooterSelect1 + " " + sqlFooter1 +
                       " ORDER BY OriginDateTime DESC, DateTimeReceived DESC";
                }

                try
                {
                    int sqlTop = GetConfigParameter("ASI", (short)Enums.ConfigurationGroups.Common, "History Max Records", 2000);
                    if (sqlTop != Const.unassignedIntValue)
                    {
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter1));
                        if ((sqlTop - totalSqlRecords) < 0)
                        {
                            requestOverflowed = true;
                            return null;
                        }
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter2));
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
                    throw new DASException("Unable to retieve messages from history" + exp.Message);
                }
                // 2. Executes SQL statement
                dsResult = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                // 1. Add additional column to result dataset
                dsResult.Tables[0].Columns.Add("MsgDetails", typeof(string));
                //int boxId = Const.unassignedIntValue;
                DataSet dsVehicleSensors = null;
                DataSet dsVehicleOutputs = null;
                string fenceDirection = "";

                // 2. Get user-defined sensors info (name,action)
                //[SensorId][SensorName][SensorAction][AlarmLevel]
                DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
                dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);


                //localization Sensors

                Localization local = new Localization(sqlExec.ConnectionString.ToString());

                Box boxCnf = new DB.Box(sqlExec);
                BoxHwDefaultSensorsCfg defSensors = new DB.BoxHwDefaultSensorsCfg(sqlExec);


                Int16 hwTypeId = 0;
                DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
                if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
                {
                    // TODO: Today box has only one HW type
                    hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);


                    if (dsVehicleSensors == null || dsVehicleSensors.Tables.Count == 0 || dsVehicleSensors.Tables[0].Rows.Count == 0)
                        dsVehicleSensors = defSensors.GetSensorsInfoByHwTypeId(hwTypeId);
                }
                else
                {
                    // TODO: write to log (cannot retrieve HW type for specific box)
                }


                if ((lang != "en") && (lang != null))
                {
                    local.LocalizationData(lang, "SensorId", "SensorName", "Sensors", hwTypeId, ref dsVehicleSensors);
                    local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
                }



                // 3. Get user-defined outputs info 
                //[OutputId][OutputName][OutputAction]
                DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
                dsVehicleOutputs = outputs.GetOutputsInfoByBoxId(boxId, userId);

                //localization outputs
                if ((lang != "en") && (lang != null))
                {

                    local.LocalizationData(lang, "OutputId", "OutputName", "Outputs", hwTypeId, ref dsVehicleOutputs);
                    local.LocalizationDataAction(lang, "OutputId", "OutputAction", "Outputs", hwTypeId, ref dsVehicleOutputs);
                }


                //localization Commands
                DB.BoxProtocolTypeCmdOutType cmds = new DB.BoxProtocolTypeCmdOutType(sqlExec);
                DataSet dsVehicleCommands = cmds.GetAllSupportedCommands(boxId, userId);
                if ((lang != "en") && (lang != null))
                    local.LocalizationData(lang, "BoxCmdOutTypeId", "BoxCmdOutTypeName", "CommandType", ref dsVehicleCommands);


                // 4. Resolve vehicle geozones
                DB.VehicleAssignment vehicleAssignment = null;
                DB.VehicleGeozone vehicleGeozone = null;
                DataSet dsGeozonesInfo = null;
                Int64 vehicleId = Const.unassignedIntValue;




                foreach (DataRow ittr in dsResult.Tables[0].Rows)
                {
                    // a simple change which should make the code quicker and more readable 
                    string customProp = ittr["CustomProp"].ToString().TrimEnd();

                    if (Convert.ToInt16(ittr["MsgDirection"]) == Const.MsgOutDirectionValue)// process outgoing messages
                    {
                        switch ((Enums.CommandType)Convert.ToInt16(ittr["BoxMsgInTypeId"]))
                        {
                            case Enums.CommandType.Output:
                                ittr["MsgDetails"] = DB.Output.GetOutputDescription(customProp, dsVehicleOutputs.Tables[0]);
                                break;

                            case Enums.CommandType.MDTTextMessage:
                                ittr["BoxMsgInTypeName"] = Resources.Const.TxtMessage;
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                ittr["MsgDetails"] += Convert.ToChar(13).ToString();
                                ittr["MsgDetails"] += Convert.ToChar(10).ToString();
                                ittr["MsgDetails"] += Resources.Const.TxtMsgAnswers + ":" + Util.PairFindValue(Const.keyAnswer, customProp).Replace("~", ",");
                                break;

                            default:
                                ittr["MsgDetails"] = ittr["BoxMsgInTypeName"];
                                //localization commands
                                if ((lang != "en") && (lang != null))
                                {
                                    foreach (DataRow dr in dsVehicleCommands.Tables[0].Rows)
                                    {
                                        if (Convert.ToInt16(ittr["BoxMsgInTypeId"]) == Convert.ToInt16(dr["BoxCmdOutTypeId"]))
                                        {
                                            ittr["MsgDetails"] = dr["BoxCmdOutTypeName"];
                                            break;
                                        }

                                    }
                                }
                                ittr["BoxMsgInTypeName"] = Resources.Const.Command;
                                break;
                        }
                        ittr["StreetAddress"] = Const.blankValue; // not applicable

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgOutDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                    else // process incomming messages
                    {
                        Enums.MessageType msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);

                        switch (msgType)
                        {
                            case Enums.MessageType.Sensor:
                            case Enums.MessageType.SensorExtended:
                            case Enums.MessageType.Alarm:
                            case Enums.MessageType.MBAlarm:
                            case Enums.MessageType.ServiceRequired:
                                if (dsVehicleSensors != null && dsVehicleSensors.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, dsVehicleSensors.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, null);
                                break;
                            case Enums.MessageType.TetheredState:
                                {
                                    string duration = Util.PairFindValue(Const.keyTetheredState, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = duration.Equals(Const.valON) ? Resources.Const.ON : Resources.Const.OFF;
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.Speeding:      // was Speeding
                                {
                                    string duration = Util.PairFindValue(Const.keySpeedDuration, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.GeoFence:
                                //fenceNo = Util.PairFindValue(Const.keyFenceNo,customProp);
                                //if(fenceNo == null || fenceNo == "")
                                //	fenceNo = "Unknown";

                                fenceDirection = Util.PairFindValue(Const.keyFenceDir, customProp);
                                if (fenceDirection == null || fenceDirection == "")
                                    fenceDirection = Resources.Const.fenceDirection_unknown;
                                else if (Const.valFenceDirIn == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_inbroken;
                                else if (Const.valFenceDirOut == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_outbroken;

                                ittr["MsgDetails"] = fenceDirection;
                                break;
                            case Enums.MessageType.GeoZone:
                                if (vehicleAssignment == null)
                                {
                                    vehicleAssignment = new DB.VehicleAssignment(sqlExec);
                                    vehicleId = Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
                                }
                                if (vehicleGeozone == null)
                                {
                                    vehicleGeozone = new DB.VehicleGeozone(sqlExec);
                                    dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
                                }

                                if (dsGeozonesInfo != null && dsGeozonesInfo.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, dsGeozonesInfo.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, null);
                                break;
                            case Enums.MessageType.IPUpdate:
                                ittr["MsgDetails"] = ittr["CommInfo1"].ToString() + ":" + ittr["CommInfo2"].ToString().TrimEnd();
                                ittr["StreetAddress"] = Const.blankValue;
                                ittr["Speed"] = Const.blankValue;
                                break;
                            case Enums.MessageType.Idling:
                            case Enums.MessageType.ExtendedIdling:
                            case Enums.MessageType.HarshBraking:
                            case Enums.MessageType.ExtremeBraking:
                            case Enums.MessageType.HarshAcceleration:
                            case Enums.MessageType.ExtremeAcceleration:
                            case Enums.MessageType.SeatBelt:
                                string speedDuration = "";
                                if (Enums.MessageType.Idling == msgType || Enums.MessageType.ExtendedIdling == msgType)
                                    speedDuration = Util.PairFindValue(Const.keyIdleDuration, customProp);
                                else
                                    speedDuration = Util.PairFindValue(Const.keyDuration, customProp);
                                if (speedDuration != "")
                                {
                                    try
                                    {
                                        ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(speedDuration) * TimeSpan.TicksPerSecond).ToString();
                                    }
                                    catch
                                    {
                                    }
                                }
                                break;
                            case Enums.MessageType.GPSAntenna:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyGPSAntenna, customProp);
                                break;

                            case Enums.MessageType.MDTResponse:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyAnswer, customProp);
                                break;
                            case Enums.MessageType.MDTSpecialMessage:
                                ittr["MsgDetails"] = customProp;
                                break;
                            case Enums.MessageType.MDTTextMessage:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                break;
                            case Enums.MessageType.Status:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyPower, customProp);
                                break;
                            case Enums.MessageType.RFIDCode:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.extFacilityCode, customProp) + "-" +
                                                      Util.PairFindValue(Const.extIDNumber, customProp);
                                break;
                            case Enums.MessageType.UpdateTelusPRLDone:
                                ittr["MsgDetails"] = "PRL=" + Util.PairFindValue(Const.keyPRLNumber, customProp);
                                break;
                            default:// do nothing
                                ittr["MsgDetails"] = " ";
                                break;
                        }
                        // in case of invalid GPS for current message set defaults
                        if (Convert.ToInt16(ittr["ValidGps"]) == 1)
                        {
                            ittr["Latitude"] = 0;
                            ittr["Longitude"] = 0;
                            ittr["Speed"] = Const.blankValue;
                            ittr["Heading"] = Const.blankValue;
                        }

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgInDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                }
            }
            return dsResult;
        }
        // Changes for TimeZone Feature end



        public DataSet GetMessagesFromHistoryByBoxId_Tmp(int userId,
                                                    bool isSuperUser,
                                                    int boxId,
                                                    DateTime from,
                                                    DateTime to,
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

            //localization settings
            Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

            DataSet dsResult = null;
            requestOverflowed = false;
            string sqlWhere = " WHERE vlfMsgInHstBrickman201107c_2.BoxId=" + boxId;
            if (from != Const.unassignedDateTime)
                sqlWhere += " AND OriginDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
            if (to != Const.unassignedDateTime)
                sqlWhere += " AND OriginDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";

            //if (from != Const.unassignedDateTime)
            //    sqlWhere += " AND AssignedDateTime<='" + from + "'";
            //if (to != Const.unassignedDateTime)
            //    sqlWhere += " AND '" + to + "'<=ISNULL(DeletedDateTime,'01/01/2099 01:00:00 AM')";




            if (msgList == "")
            {

                // gb can we have a range for those messages instead of this huge string ??
                sqlWhere += " AND (vlfMsgInHstBrickman201107c_2.BoxMsgInTypeId=" + (short)Enums.MessageType.Speed +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Speeding +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoFence +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MBAlarm +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobArm +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobDisarm +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobPanic +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Alarm +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTResponse +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTSpecialMessage +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTTextMessage +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxSetup +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxStatus +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.StoredPosition +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Idling +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtendedIdling +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZone +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntenna +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOK +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneIDs +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSetup +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.VCROffDelay +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobStatus +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.TARMode +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ControllerStatus +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ServiceRequired +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.LowBattery +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxReset +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputOneOn +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputTwoOn +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Odometer +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaShort +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOpen +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.BreakIn +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.TowAwayAlert +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Collision +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.PanicButton +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SleepModeFailure +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.AckWithPosition +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.UpdateTelusPRLDone +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MainBatteryDisconnected +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessages +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MultipleStandardMessagesXS +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SendSMC +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SendDTC +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.DeviceTestResult +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SMCWriteDone +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTAck +
                     " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.FuelTransaction +
                    " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.MovementAlert;

                if (isSuperUser)
                {
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.RFIDCode;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdate;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdateBantek;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SendEEPROMData;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SendExtendedSetup;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SendExtendedStatus;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferFuelAlarm;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferSensorAlarm;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferStatusReport;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.ReeferTemperatureAlarm;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SendReeferSetup;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.TetheredState;
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Heartbeat;
                }

                if (includeSensor)
                    sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Sensor +
                                " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.SensorExtended;
                if (includeCoordinate)//"Scheduled Update" is displayed name
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate;
                    else
                        sqlWhere += " OR (vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate + " AND ValidGps=0)";
                }
                if (includePositionUpdate)
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate;
                    else
                        sqlWhere += " OR (vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate + " AND ValidGps=0)";
                }
                sqlWhere += ")";
            }
            else
            {
                string[] msgTypeId = msgList.Split(',');
                if (msgTypeId.Length > 0)
                {
                    sqlWhere += " AND (";
                    for (int i = 0; i < msgTypeId.Length; i++)
                    {
                        sqlWhere += " vlfMsgInHstBrickman201107c.BoxMsgInTypeId=" + msgTypeId[i] + " OR ";
                    }

                    sqlWhere = sqlWhere.Substring(0, sqlWhere.Length - 4);
                    sqlWhere += ")";
                }
            }





            if (DclId != -1)
                sqlWhere += " AND DclId=" + DclId.ToString();

            try
            {
                // 1. Prepares SQL statement
                string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone int" +
                    " DECLARE @Unit real" +
                    " DECLARE @DayLightSaving int" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZone +
                    " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference" +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " IF @Unit IS NULL SET @Unit=1" +
                    " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0";

                string sqlFooterSelect1 = "SELECT " + sqlTopRecords + " vlfMsgInHstBrickman201107c.BoxId,vlfVehicleAssignment.VehicleId,vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.Description, " +
                   "CASE WHEN DateTimeReceived IS NULL then '' ELSE DATEADD(hour,@Timezone,DateTimeReceived) END AS DateTimeReceived," +
                   "DclId,vlfBoxMsgInType.BoxMsgInTypeId," +
                   "CASE WHEN BoxMsgInTypeName IS NOT NULL then RTRIM(BoxMsgInTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "convert(nvarchar,ValidGps) AS ValidGps," +
                   "LTRIM(RTRIM(STR(ROUND(Latitude,5),10,5))) AS Latitude," +
                   "LTRIM(RTRIM(STR(ROUND(Longitude,6),10,6))) AS Longitude," +
                   "convert(nvarchar,Heading) AS Heading," +
                   "convert(nvarchar,SensorMask) AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "convert(nvarchar,BlobDataSize) AS BlobDataSize,SequenceNum," +
                        "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHstBrickman201107c.StreetAddress ELSE CASE WHEN vlfMsgInHstBrickman201107c.NearestLandmark IS NULL then vlfMsgInHstBrickman201107c.StreetAddress ELSE vlfMsgInHstBrickman201107c.NearestLandmark END END,'" + Const.addressNA + "') AS StreetAddress," +


                   "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,OriginDateTime) END AS OriginDateTime," +
                   "CASE WHEN vlfMsgInHstBrickman201107c.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHstBrickman201107c.Speed * @Unit,1)) END AS Speed," +
                    //"convert(nvarchar,CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END) AS BoxArmed,"+
                   "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed," +
                   "0 AS MsgDirection," + // in
                   "' ' AS Acknowledged," +
                   "' ' AS Scheduled";

                string sqlFooter1 = "FROM vlfMsgInHstBrickman201107c with (nolock),vlfBoxMsgInType,vlfBoxProtocolType,vlfVehicleAssignment,vlfVehicleInfo WITH (NOLOCK)" +
                   sqlWhere +
                   " AND vlfMsgInHstBrickman201107c.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId" +
                   " AND vlfMsgInHstBrickman201107c.BoxId=vlfVehicleAssignment.BoxId" +
                   " AND vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId" +
                   " AND vlfMsgInHstBrickman201107c.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";
                // retrieve all commands/outputs from vlfMsgOutHst
                sqlWhere = "";
                //if (from != Const.unassignedDateTime)
                //    sqlWhere += " AND AssignedDateTime<='" + from + "'";
                //if (to != Const.unassignedDateTime)
                //    sqlWhere += " AND '" + to + "'<=ISNULL(DeletedDateTime,'01/01/2099 01:00:00 AM')";


                if (from != Const.unassignedDateTime)
                    sqlWhere += " AND DateTime>='" + from + "'";

                if (to != Const.unassignedDateTime)
                    sqlWhere += " AND DateTime<='" + to + "'";


                sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.Ack +
                         " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.MDTAck;
                if (!isSuperUser)
                {
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.WakeUpByte;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.UploadFirmware;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.UploadFirmwareStatus;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.ThirdPartyPacket;
                }
                if (DclId != -1)
                    sqlWhere += " AND DclId=" + DclId.ToString();


                string sqlFooterSelect2 = "SELECT  " + sqlTopRecords + " vlfMsgOutHst.BoxId,vlfVehicleAssignment.VehicleId,vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.Description, " +
                   "DATEADD(hour,@Timezone,DateTime) AS DateTimeReceived,DclId," +
                   "vlfBoxCmdOutType.BoxCmdOutTypeId AS BoxMsgInTypeId," +
                   "CASE WHEN BoxCmdOutTypeName IS NOT NULL then RTRIM(BoxCmdOutTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "'" + Const.blankValue + "' AS ValidGps," +
                   "'" + Const.blankValue + "' AS Latitude," +
                   "'" + Const.blankValue + "' AS Longitude," +
                   "'" + Const.blankValue + "' AS Heading," +
                   "'" + Const.blankValue + "' AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "'" + Const.blankValue + "' As BlobDataSize," +
                   "SequenceNum," +
                   "'" + Const.blankValue + "' AS StreetAddress," +
                   "DATEADD(hour,@Timezone,DateTime) AS OriginDateTime," +
                   "'" + Const.blankValue + "' AS Speed," +
                   "'" + Const.blankValue + "' AS BoxArmed," +
                   "1 AS MsgDirection," + // out
                   "CASE WHEN Acknowledged IS NOT NULL then CASE WHEN RTRIM(Acknowledged) IS NOT NULL then RTRIM(Acknowledged) END ELSE '" + Const.cmdNotAck + "' END AS Acknowledged," +
                   "CASE WHEN Scheduled IS NOT NULL then CASE WHEN Scheduled=0 then 'No' END ELSE 'Yes' END AS Scheduled";
                string sqlFooter2 = "FROM vlfMsgOutHst,vlfBoxCmdOutType,vlfBoxProtocolType,vlfVehicleAssignment,vlfVehicleInfo WITH (NOLOCK)" +
                   " WHERE vlfMsgOutHst.BoxId=" + boxId +
                   sqlWhere +
                   " AND vlfMsgOutHst.BoxCmdOutTypeId=vlfBoxCmdOutType.BoxCmdOutTypeId" +
                   " AND vlfMsgOutHst.BoxId=vlfVehicleAssignment.BoxId" +
                   " AND vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId" +
                   " AND vlfMsgOutHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId";
                if (DclId != -1)
                    sqlWhere += " AND DclId=" + DclId.ToString();

                string sql = "";
                if (msgList == "" && sqlTopRecords == "")
                {
                    sql = sqlHeader + " " + sqlFooterSelect1 + " " + sqlFooter1 +
                       " UNION " + sqlFooterSelect2 + " " + sqlFooter2 +
                       " ORDER BY OriginDateTime DESC, DateTimeReceived DESC";
                }
                else
                {
                    sql = sqlHeader + " " + sqlFooterSelect1 + " " + sqlFooter1 +
                       " ORDER BY OriginDateTime DESC, DateTimeReceived DESC";
                }

                try
                {
                    int sqlTop = GetConfigParameter("ASI", (short)Enums.ConfigurationGroups.Common, "History Max Records", 2000);
                    if (sqlTop != Const.unassignedIntValue)
                    {
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter1));
                        if ((sqlTop - totalSqlRecords) < 0)
                        {
                            requestOverflowed = true;
                            return null;
                        }
                        totalSqlRecords += Convert.ToInt32(sqlExec.SQLExecuteScalar("SELECT count(*) " + sqlFooter2));
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
                    throw new DASException("Unable to retieve messages from history" + exp.Message);
                }
                // 2. Executes SQL statement
                dsResult = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                // 1. Add additional column to result dataset
                dsResult.Tables[0].Columns.Add("MsgDetails", typeof(string));
                //int boxId = Const.unassignedIntValue;
                DataSet dsVehicleSensors = null;
                DataSet dsVehicleOutputs = null;
                string fenceDirection = "";

                // 2. Get user-defined sensors info (name,action)
                //[SensorId][SensorName][SensorAction][AlarmLevel]
                DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
                dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);


                //localization Sensors

                Localization local = new Localization(sqlExec.ConnectionString.ToString());

                Box boxCnf = new DB.Box(sqlExec);
                BoxHwDefaultSensorsCfg defSensors = new DB.BoxHwDefaultSensorsCfg(sqlExec);


                Int16 hwTypeId = 0;
                DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
                if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
                {
                    // TODO: Today box has only one HW type
                    hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);


                    if (dsVehicleSensors == null || dsVehicleSensors.Tables.Count == 0 || dsVehicleSensors.Tables[0].Rows.Count == 0)
                        dsVehicleSensors = defSensors.GetSensorsInfoByHwTypeId(hwTypeId);
                }
                else
                {
                    // TODO: write to log (cannot retrieve HW type for specific box)
                }


                if ((lang != "en") && (lang != null))
                {
                    local.LocalizationData(lang, "SensorId", "SensorName", "Sensors", hwTypeId, ref dsVehicleSensors);
                    local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
                }



                // 3. Get user-defined outputs info 
                //[OutputId][OutputName][OutputAction]
                DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
                dsVehicleOutputs = outputs.GetOutputsInfoByBoxId(boxId, userId);

                //localization outputs
                if ((lang != "en") && (lang != null))
                {

                    local.LocalizationData(lang, "OutputId", "OutputName", "Outputs", hwTypeId, ref dsVehicleOutputs);
                    local.LocalizationDataAction(lang, "OutputId", "OutputAction", "Outputs", hwTypeId, ref dsVehicleOutputs);
                }


                //localization Commands
                DB.BoxProtocolTypeCmdOutType cmds = new DB.BoxProtocolTypeCmdOutType(sqlExec);
                DataSet dsVehicleCommands = cmds.GetAllSupportedCommands(boxId, userId);
                if ((lang != "en") && (lang != null))
                    local.LocalizationData(lang, "BoxCmdOutTypeId", "BoxCmdOutTypeName", "CommandType", ref dsVehicleCommands);


                // 4. Resolve vehicle geozones
                DB.VehicleAssignment vehicleAssignment = null;
                DB.VehicleGeozone vehicleGeozone = null;
                DataSet dsGeozonesInfo = null;
                Int64 vehicleId = Const.unassignedIntValue;




                foreach (DataRow ittr in dsResult.Tables[0].Rows)
                {
                    // a simple change which should make the code quicker and more readable 
                    string customProp = ittr["CustomProp"].ToString().TrimEnd();

                    if (Convert.ToInt16(ittr["MsgDirection"]) == Const.MsgOutDirectionValue)// process outgoing messages
                    {
                        switch ((Enums.CommandType)Convert.ToInt16(ittr["BoxMsgInTypeId"]))
                        {
                            case Enums.CommandType.Output:
                                ittr["MsgDetails"] = DB.Output.GetOutputDescription(customProp, dsVehicleOutputs.Tables[0]);
                                break;

                            case Enums.CommandType.MDTTextMessage:
                                ittr["BoxMsgInTypeName"] = Resources.Const.TxtMessage;
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                ittr["MsgDetails"] += Convert.ToChar(13).ToString();
                                ittr["MsgDetails"] += Convert.ToChar(10).ToString();
                                ittr["MsgDetails"] += Resources.Const.TxtMsgAnswers + ":" + Util.PairFindValue(Const.keyAnswer, customProp).Replace("~", ",");
                                break;

                            default:
                                ittr["MsgDetails"] = ittr["BoxMsgInTypeName"];
                                //localization commands
                                if ((lang != "en") && (lang != null))
                                {
                                    foreach (DataRow dr in dsVehicleCommands.Tables[0].Rows)
                                    {
                                        if (Convert.ToInt16(ittr["BoxMsgInTypeId"]) == Convert.ToInt16(dr["BoxCmdOutTypeId"]))
                                        {
                                            ittr["MsgDetails"] = dr["BoxCmdOutTypeName"];
                                            break;
                                        }

                                    }
                                }
                                ittr["BoxMsgInTypeName"] = Resources.Const.Command;
                                break;
                        }
                        ittr["StreetAddress"] = Const.blankValue; // not applicable

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgOutDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                    else // process incomming messages
                    {
                        Enums.MessageType msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);

                        switch (msgType)
                        {
                            case Enums.MessageType.Sensor:
                            case Enums.MessageType.SensorExtended:
                            case Enums.MessageType.Alarm:
                            case Enums.MessageType.MBAlarm:
                            case Enums.MessageType.ServiceRequired:
                                if (dsVehicleSensors != null && dsVehicleSensors.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, dsVehicleSensors.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, null);
                                break;
                            case Enums.MessageType.TetheredState:
                                {
                                    string duration = Util.PairFindValue(Const.keyTetheredState, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = duration.Equals(Const.valON) ? Resources.Const.ON : Resources.Const.OFF;
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.Speeding:      // was Speeding
                                {
                                    string duration = Util.PairFindValue(Const.keySpeedDuration, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.GeoFence:
                                //fenceNo = Util.PairFindValue(Const.keyFenceNo,customProp);
                                //if(fenceNo == null || fenceNo == "")
                                //	fenceNo = "Unknown";

                                fenceDirection = Util.PairFindValue(Const.keyFenceDir, customProp);
                                if (fenceDirection == null || fenceDirection == "")
                                    fenceDirection = Resources.Const.fenceDirection_unknown;
                                else if (Const.valFenceDirIn == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_inbroken;
                                else if (Const.valFenceDirOut == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_outbroken;

                                ittr["MsgDetails"] = fenceDirection;
                                break;
                            case Enums.MessageType.GeoZone:
                                if (vehicleAssignment == null)
                                {
                                    vehicleAssignment = new DB.VehicleAssignment(sqlExec);
                                    vehicleId = Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
                                }
                                if (vehicleGeozone == null)
                                {
                                    vehicleGeozone = new DB.VehicleGeozone(sqlExec);
                                    dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
                                }

                                if (dsGeozonesInfo != null && dsGeozonesInfo.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, dsGeozonesInfo.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, null);
                                break;
                            case Enums.MessageType.IPUpdate:
                                ittr["MsgDetails"] = ittr["CommInfo1"].ToString() + ":" + ittr["CommInfo2"].ToString().TrimEnd();
                                ittr["StreetAddress"] = Const.blankValue;
                                ittr["Speed"] = Const.blankValue;
                                break;
                            case Enums.MessageType.Idling:
                            case Enums.MessageType.ExtendedIdling:
                            case Enums.MessageType.HarshBraking:
                            case Enums.MessageType.ExtremeBraking:
                            case Enums.MessageType.HarshAcceleration:
                            case Enums.MessageType.ExtremeAcceleration:
                            case Enums.MessageType.SeatBelt:
                                string speedDuration = "";
                                if (Enums.MessageType.Idling == msgType || Enums.MessageType.ExtendedIdling == msgType)
                                    speedDuration = Util.PairFindValue(Const.keyIdleDuration, customProp);
                                else
                                    speedDuration = Util.PairFindValue(Const.keyDuration, customProp);
                                if (speedDuration != "")
                                {
                                    try
                                    {
                                        ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(speedDuration) * TimeSpan.TicksPerSecond).ToString();
                                    }
                                    catch
                                    {
                                    }
                                }
                                break;
                            case Enums.MessageType.GPSAntenna:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyGPSAntenna, customProp);
                                break;

                            case Enums.MessageType.MDTResponse:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyAnswer, customProp);
                                break;
                            case Enums.MessageType.MDTSpecialMessage:
                                ittr["MsgDetails"] = customProp;
                                break;
                            case Enums.MessageType.MDTTextMessage:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                break;
                            case Enums.MessageType.Status:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyPower, customProp);
                                break;
                            case Enums.MessageType.RFIDCode:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.extFacilityCode, customProp) + "-" +
                                                      Util.PairFindValue(Const.extIDNumber, customProp);
                                break;
                            case Enums.MessageType.UpdateTelusPRLDone:
                                ittr["MsgDetails"] = "PRL=" + Util.PairFindValue(Const.keyPRLNumber, customProp);
                                break;
                            default:// do nothing
                                ittr["MsgDetails"] = " ";
                                break;
                        }
                        // in case of invalid GPS for current message set defaults
                        if (Convert.ToInt16(ittr["ValidGps"]) == 1)
                        {
                            ittr["Latitude"] = 0;
                            ittr["Longitude"] = 0;
                            ittr["Speed"] = Const.blankValue;
                            ittr["Heading"] = Const.blankValue;
                        }

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgInDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                }
            }
            return dsResult;
        }








        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves messages from history by vehicle Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isSuperUser"></param>
        /// <param name="boxId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="tblLandmarks"></param>
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetMessagesFromHistoryByBoxId_ExtendedSearch_NewTZ(int userId,
                                                     bool isSuperUser,
                                                     int boxId,
                                                     DateTime from,
                                                     DateTime to,
                                                     bool includeCoordinate,
                                                     bool includeSensor,
                                                     bool includePositionUpdate,
                                                     bool includeInvalidGps,
                                                     Int16 DclId,
                                                     string lang,
                                                     string address,
                                                     ref bool requestOverflowed,
                                                     ref int totalSqlRecords)
        {
            DataSet dsResult = null;
            string errorMsg = "Cannot retrieve messages from history";
            try
            {
                //localization settings
                Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

                requestOverflowed = false;

                // 2. Executes SQL statement
                SqlParameter[] sqlParams = new SqlParameter[14];

                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@boxId", boxId);
                sqlParams[2] = new SqlParameter("@dtFrom", from);
                sqlParams[3] = new SqlParameter("@dtTo", to);
                sqlParams[4] = new SqlParameter("@isSuperUser", isSuperUser);
                sqlParams[5] = new SqlParameter("@coordinate", includeCoordinate);
                sqlParams[6] = new SqlParameter("@sensor", includeSensor);
                sqlParams[7] = new SqlParameter("@posUpdate", includePositionUpdate);
                sqlParams[8] = new SqlParameter("@invalidGps", includeInvalidGps);
                sqlParams[9] = new SqlParameter("@dclId", DclId);
                sqlParams[10] = new SqlParameter("@lang", lang);
                sqlParams[11] = new SqlParameter("@address", address);
                sqlParams[12] = new SqlParameter("@total", totalSqlRecords);
                sqlParams[12].Direction = ParameterDirection.Output;
                sqlParams[13] = new SqlParameter("@over", requestOverflowed);
                sqlParams[13].Direction = ParameterDirection.Output;

                dsResult = sqlExec.SPExecuteDataset("HistoryGetMessagesbyStreetAddress_BoxId_NewTimeZone", sqlParams);
                //int sqlTop = GetConfigParameter("ASI", (short)Enums.ConfigurationGroups.Common, "Report Max Records", 10000);
                if (requestOverflowed)
                {
                    return null;
                }
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(errorMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(String.Format("{0}:\n{1}", errorMsg, objException.Message));
            }

            if (Util.IsDataSetValid(dsResult))
            {
                // 1. Add additional column to result dataset
                dsResult.Tables[0].Columns.Add("MsgDetails", typeof(string));
                //int boxId = Const.unassignedIntValue;
                DataSet dsVehicleSensors = null;
                DataSet dsVehicleOutputs = null;
                string fenceDirection = "";

                // 2. Get user-defined sensors info (name,action)
                //[SensorId][SensorName][SensorAction][AlarmLevel]
                DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
                dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);

                //localization Sensors

                Localization local = new Localization(sqlExec.ConnectionString.ToString());

                Box boxCnf = new DB.Box(sqlExec);
                BoxHwDefaultSensorsCfg defSensors = new DB.BoxHwDefaultSensorsCfg(sqlExec);

                Int16 hwTypeId = 0;
                DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
                if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
                {
                    // TODO: Today box has only one HW type
                    hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);

                    if (dsVehicleSensors == null || dsVehicleSensors.Tables.Count == 0 || dsVehicleSensors.Tables[0].Rows.Count == 0)
                        defSensors.GetSensorsInfoByHwTypeId(hwTypeId);


                }
                else
                {
                    // TODO: write to log (cannot retrieve HW type for specific box)
                }

                if ((lang != "en") && (lang != null))
                {


                    local.LocalizationData(lang, "SensorId", "SensorName", "Sensors", hwTypeId, ref dsVehicleSensors);
                    local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
                }



                // 3. Get user-defined outputs info 
                //[OutputId][OutputName][OutputAction]
                DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
                dsVehicleOutputs = outputs.GetOutputsInfoByBoxId(boxId, userId);

                //localization outputs
                if ((lang != "en") && (lang != null))
                {



                    local.LocalizationData(lang, "OutputId", "OutputName", "Outputs", hwTypeId, ref dsVehicleOutputs);
                    local.LocalizationDataAction(lang, "OutputId", "OutputAction", "Outputs", hwTypeId, ref dsVehicleOutputs);
                }


                //localization Commands
                DB.BoxProtocolTypeCmdOutType cmds = new DB.BoxProtocolTypeCmdOutType(sqlExec);
                DataSet dsVehicleCommands = cmds.GetAllSupportedCommands(boxId, userId);
                if ((lang != "en") && (lang != null))
                    local.LocalizationData(lang, "BoxCmdOutTypeId", "BoxCmdOutTypeName", "CommandType", ref dsVehicleCommands);


                // 4. Resolve vehicle geozones
                DB.VehicleAssignment vehicleAssignment = null;
                DB.VehicleGeozone vehicleGeozone = null;
                DataSet dsGeozonesInfo = null;
                Int64 vehicleId = Const.unassignedIntValue;




                foreach (DataRow ittr in dsResult.Tables[0].Rows)
                {
                    // a simple change which should make the code quicker and more readable 
                    string customProp = ittr["CustomProp"].ToString().TrimEnd();

                    if (Convert.ToInt16(ittr["MsgDirection"]) == Const.MsgOutDirectionValue)// process outgoing messages
                    {
                        switch ((Enums.CommandType)Convert.ToInt16(ittr["BoxMsgInTypeId"]))
                        {
                            case Enums.CommandType.Output:
                                ittr["MsgDetails"] = DB.Output.GetOutputDescription(customProp, dsVehicleOutputs.Tables[0]);
                                break;

                            case Enums.CommandType.MDTTextMessage:
                                ittr["BoxMsgInTypeName"] = Resources.Const.TxtMessage;
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                ittr["MsgDetails"] += Convert.ToChar(13).ToString();
                                ittr["MsgDetails"] += Convert.ToChar(10).ToString();
                                ittr["MsgDetails"] += Resources.Const.TxtMsgAnswers + ":" + Util.PairFindValue(Const.keyAnswer, customProp).Replace("~", ",");
                                break;

                            default:
                                ittr["MsgDetails"] = ittr["BoxMsgInTypeName"];
                                //localization commands
                                if ((lang != "en") && (lang != null))
                                {
                                    foreach (DataRow dr in dsVehicleCommands.Tables[0].Rows)
                                    {
                                        if (Convert.ToInt16(ittr["BoxMsgInTypeId"]) == Convert.ToInt16(dr["BoxCmdOutTypeId"]))
                                        {
                                            ittr["MsgDetails"] = dr["BoxCmdOutTypeName"];
                                            break;
                                        }

                                    }
                                }
                                ittr["BoxMsgInTypeName"] = Resources.Const.Command;
                                break;
                        }
                        ittr["StreetAddress"] = Const.blankValue; // not applicable

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgOutDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                    else // process incomming messages
                    {
                        Enums.MessageType msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);

                        switch (msgType)
                        {
                            case Enums.MessageType.Sensor:
                            case Enums.MessageType.SensorExtended:
                            case Enums.MessageType.Alarm:
                            case Enums.MessageType.MBAlarm:
                            case Enums.MessageType.ServiceRequired:
                                if (dsVehicleSensors != null && dsVehicleSensors.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, dsVehicleSensors.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, null);
                                break;
                            case Enums.MessageType.Speeding:      // was Speeding
                                {
                                    string duration = Util.PairFindValue(Const.keySpeedDuration, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.TetheredState:
                                {
                                    string duration = Util.PairFindValue(Const.keyTetheredState, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = duration.Equals(Const.valON) ? Resources.Const.ON : Resources.Const.OFF;
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.GeoFence:
                                //fenceNo = Util.PairFindValue(Const.keyFenceNo,customProp);
                                //if(fenceNo == null || fenceNo == "")
                                //	fenceNo = "Unknown";

                                fenceDirection = Util.PairFindValue(Const.keyFenceDir, customProp);
                                if (fenceDirection == null || fenceDirection == "")
                                    fenceDirection = Resources.Const.fenceDirection_unknown;
                                else if (Const.valFenceDirIn == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_inbroken;
                                else if (Const.valFenceDirOut == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_outbroken;

                                ittr["MsgDetails"] = fenceDirection;
                                break;
                            case Enums.MessageType.GeoZone:
                                if (vehicleAssignment == null)
                                {
                                    vehicleAssignment = new DB.VehicleAssignment(sqlExec);
                                    vehicleId = Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
                                }
                                if (vehicleGeozone == null)
                                {
                                    vehicleGeozone = new DB.VehicleGeozone(sqlExec);
                                    dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
                                }

                                if (dsGeozonesInfo != null && dsGeozonesInfo.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, dsGeozonesInfo.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, null);
                                break;
                            case Enums.MessageType.IPUpdate:
                                ittr["MsgDetails"] = ittr["CommInfo1"].ToString() + ":" + ittr["CommInfo2"].ToString().TrimEnd();
                                ittr["StreetAddress"] = Const.blankValue;
                                ittr["Speed"] = Const.blankValue;
                                break;
                            case Enums.MessageType.Idling:
                            case Enums.MessageType.ExtendedIdling:
                            case Enums.MessageType.HarshBraking:
                            case Enums.MessageType.ExtremeBraking:
                            case Enums.MessageType.HarshAcceleration:
                            case Enums.MessageType.ExtremeAcceleration:
                            case Enums.MessageType.SeatBelt:
                                string speedDuration = "";
                                if (Enums.MessageType.Idling == msgType || Enums.MessageType.ExtendedIdling == msgType)
                                    speedDuration = Util.PairFindValue(Const.keyIdleDuration, customProp);
                                else
                                    speedDuration = Util.PairFindValue(Const.keyDuration, customProp);
                                if (speedDuration != "")
                                {
                                    try
                                    {
                                        ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(speedDuration) * TimeSpan.TicksPerSecond).ToString();
                                    }
                                    catch
                                    {
                                    }
                                }
                                break;
                            case Enums.MessageType.GPSAntenna:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyGPSAntenna, customProp);
                                break;

                            case Enums.MessageType.MDTResponse:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyAnswer, customProp);
                                break;
                            case Enums.MessageType.MDTSpecialMessage:
                                ittr["MsgDetails"] = customProp;
                                break;
                            case Enums.MessageType.MDTTextMessage:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                break;
                            case Enums.MessageType.Status:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyPower, customProp);
                                break;
                            case Enums.MessageType.RFIDCode:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.extFacilityCode, customProp) + "-" +
                                                      Util.PairFindValue(Const.extIDNumber, customProp);
                                break;
                            case Enums.MessageType.UpdateTelusPRLDone:
                                ittr["MsgDetails"] = "PRL=" + Util.PairFindValue(Const.keyPRLNumber, customProp);
                                break;
                            default:// do nothing
                                ittr["MsgDetails"] = " ";
                                break;
                        }
                        // in case of invalid GPS for current message set defaults
                        if (Convert.ToInt16(ittr["ValidGps"]) == 1)
                        {
                            ittr["Latitude"] = 0;
                            ittr["Longitude"] = 0;
                            ittr["Speed"] = Const.blankValue;
                            ittr["Heading"] = Const.blankValue;
                        }

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgInDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                }
            }
            return dsResult;
        }

        // Changes for TimeZone Feature end






        /// <summary>
        /// Retrieves messages from history by vehicle Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isSuperUser"></param>
        /// <param name="boxId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="tblLandmarks"></param>
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetMessagesFromHistoryByBoxId_ExtendedSearch(int userId,
                                                     bool isSuperUser,
                                                     int boxId,
                                                     DateTime from,
                                                     DateTime to,
                                                     bool includeCoordinate,
                                                     bool includeSensor,
                                                     bool includePositionUpdate,
                                                     bool includeInvalidGps,
                                                     Int16 DclId,
                                                     string lang,
                                                     string address,
                                                     ref bool requestOverflowed,
                                                     ref int totalSqlRecords)
        {
            DataSet dsResult = null;
            string errorMsg = "Cannot retrieve messages from history";
            try
            {
                //localization settings
                Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

                requestOverflowed = false;

                // 2. Executes SQL statement
                SqlParameter[] sqlParams = new SqlParameter[14];

                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@boxId", boxId);
                sqlParams[2] = new SqlParameter("@dtFrom", from);
                sqlParams[3] = new SqlParameter("@dtTo", to);
                sqlParams[4] = new SqlParameter("@isSuperUser", isSuperUser);
                sqlParams[5] = new SqlParameter("@coordinate", includeCoordinate);
                sqlParams[6] = new SqlParameter("@sensor", includeSensor);
                sqlParams[7] = new SqlParameter("@posUpdate", includePositionUpdate);
                sqlParams[8] = new SqlParameter("@invalidGps", includeInvalidGps);
                sqlParams[9] = new SqlParameter("@dclId", DclId);
                sqlParams[10] = new SqlParameter("@lang", lang);
                sqlParams[11] = new SqlParameter("@address", address);
                sqlParams[12] = new SqlParameter("@total", totalSqlRecords);
                sqlParams[12].Direction = ParameterDirection.Output;
                sqlParams[13] = new SqlParameter("@over", requestOverflowed);
                sqlParams[13].Direction = ParameterDirection.Output;

                dsResult = sqlExec.SPExecuteDataset("HistoryGetMessagesbyStreetAddress_BoxId", sqlParams);
                //int sqlTop = GetConfigParameter("ASI", (short)Enums.ConfigurationGroups.Common, "Report Max Records", 10000);
                if (requestOverflowed)
                {
                    return null;
                }
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(errorMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(String.Format("{0}:\n{1}", errorMsg, objException.Message));
            }

            if (Util.IsDataSetValid(dsResult))
            {
                // 1. Add additional column to result dataset
                dsResult.Tables[0].Columns.Add("MsgDetails", typeof(string));
                //int boxId = Const.unassignedIntValue;
                DataSet dsVehicleSensors = null;
                DataSet dsVehicleOutputs = null;
                string fenceDirection = "";

                // 2. Get user-defined sensors info (name,action)
                //[SensorId][SensorName][SensorAction][AlarmLevel]
                DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
                dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);

                //localization Sensors

                Localization local = new Localization(sqlExec.ConnectionString.ToString());

                Box boxCnf = new DB.Box(sqlExec);
                BoxHwDefaultSensorsCfg defSensors = new DB.BoxHwDefaultSensorsCfg(sqlExec);

                Int16 hwTypeId = 0;
                DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
                if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
                {
                    // TODO: Today box has only one HW type
                    hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);

                    if (dsVehicleSensors == null || dsVehicleSensors.Tables.Count == 0 || dsVehicleSensors.Tables[0].Rows.Count == 0)
                        defSensors.GetSensorsInfoByHwTypeId(hwTypeId);


                }
                else
                {
                    // TODO: write to log (cannot retrieve HW type for specific box)
                }

                if ((lang != "en") && (lang != null))
                {


                    local.LocalizationData(lang, "SensorId", "SensorName", "Sensors", hwTypeId, ref dsVehicleSensors);
                    local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
                }



                // 3. Get user-defined outputs info 
                //[OutputId][OutputName][OutputAction]
                DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
                dsVehicleOutputs = outputs.GetOutputsInfoByBoxId(boxId, userId);

                //localization outputs
                if ((lang != "en") && (lang != null))
                {



                    local.LocalizationData(lang, "OutputId", "OutputName", "Outputs", hwTypeId, ref dsVehicleOutputs);
                    local.LocalizationDataAction(lang, "OutputId", "OutputAction", "Outputs", hwTypeId, ref dsVehicleOutputs);
                }


                //localization Commands
                DB.BoxProtocolTypeCmdOutType cmds = new DB.BoxProtocolTypeCmdOutType(sqlExec);
                DataSet dsVehicleCommands = cmds.GetAllSupportedCommands(boxId, userId);
                if ((lang != "en") && (lang != null))
                    local.LocalizationData(lang, "BoxCmdOutTypeId", "BoxCmdOutTypeName", "CommandType", ref dsVehicleCommands);


                // 4. Resolve vehicle geozones
                DB.VehicleAssignment vehicleAssignment = null;
                DB.VehicleGeozone vehicleGeozone = null;
                DataSet dsGeozonesInfo = null;
                Int64 vehicleId = Const.unassignedIntValue;




                foreach (DataRow ittr in dsResult.Tables[0].Rows)
                {
                    // a simple change which should make the code quicker and more readable 
                    string customProp = ittr["CustomProp"].ToString().TrimEnd();

                    if (Convert.ToInt16(ittr["MsgDirection"]) == Const.MsgOutDirectionValue)// process outgoing messages
                    {
                        switch ((Enums.CommandType)Convert.ToInt16(ittr["BoxMsgInTypeId"]))
                        {
                            case Enums.CommandType.Output:
                                ittr["MsgDetails"] = DB.Output.GetOutputDescription(customProp, dsVehicleOutputs.Tables[0]);
                                break;

                            case Enums.CommandType.MDTTextMessage:
                                ittr["BoxMsgInTypeName"] = Resources.Const.TxtMessage;
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                ittr["MsgDetails"] += Convert.ToChar(13).ToString();
                                ittr["MsgDetails"] += Convert.ToChar(10).ToString();
                                ittr["MsgDetails"] += Resources.Const.TxtMsgAnswers + ":" + Util.PairFindValue(Const.keyAnswer, customProp).Replace("~", ",");
                                break;

                            default:
                                ittr["MsgDetails"] = ittr["BoxMsgInTypeName"];
                                //localization commands
                                if ((lang != "en") && (lang != null))
                                {
                                    foreach (DataRow dr in dsVehicleCommands.Tables[0].Rows)
                                    {
                                        if (Convert.ToInt16(ittr["BoxMsgInTypeId"]) == Convert.ToInt16(dr["BoxCmdOutTypeId"]))
                                        {
                                            ittr["MsgDetails"] = dr["BoxCmdOutTypeName"];
                                            break;
                                        }

                                    }
                                }
                                ittr["BoxMsgInTypeName"] = Resources.Const.Command;
                                break;
                        }
                        ittr["StreetAddress"] = Const.blankValue; // not applicable

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgOutDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                    else // process incomming messages
                    {
                        Enums.MessageType msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);

                        switch (msgType)
                        {
                            case Enums.MessageType.Sensor:
                            case Enums.MessageType.SensorExtended:
                            case Enums.MessageType.Alarm:
                            case Enums.MessageType.MBAlarm:
                            case Enums.MessageType.ServiceRequired:
                                if (dsVehicleSensors != null && dsVehicleSensors.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, dsVehicleSensors.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, null);
                                break;
                            case Enums.MessageType.Speeding:      // was Speeding
                                {
                                    string duration = Util.PairFindValue(Const.keySpeedDuration, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.TetheredState:
                                {
                                    string duration = Util.PairFindValue(Const.keyTetheredState, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = duration.Equals(Const.valON) ? Resources.Const.ON : Resources.Const.OFF;
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.GeoFence:
                                //fenceNo = Util.PairFindValue(Const.keyFenceNo,customProp);
                                //if(fenceNo == null || fenceNo == "")
                                //	fenceNo = "Unknown";

                                fenceDirection = Util.PairFindValue(Const.keyFenceDir, customProp);
                                if (fenceDirection == null || fenceDirection == "")
                                    fenceDirection = Resources.Const.fenceDirection_unknown;
                                else if (Const.valFenceDirIn == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_inbroken;
                                else if (Const.valFenceDirOut == fenceDirection)
                                    fenceDirection = Resources.Const.fenceDirection_outbroken;

                                ittr["MsgDetails"] = fenceDirection;
                                break;
                            case Enums.MessageType.GeoZone:
                                if (vehicleAssignment == null)
                                {
                                    vehicleAssignment = new DB.VehicleAssignment(sqlExec);
                                    vehicleId = Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
                                }
                                if (vehicleGeozone == null)
                                {
                                    vehicleGeozone = new DB.VehicleGeozone(sqlExec);
                                    dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
                                }

                                if (dsGeozonesInfo != null && dsGeozonesInfo.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, dsGeozonesInfo.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, null);
                                break;
                            case Enums.MessageType.IPUpdate:
                                ittr["MsgDetails"] = ittr["CommInfo1"].ToString() + ":" + ittr["CommInfo2"].ToString().TrimEnd();
                                ittr["StreetAddress"] = Const.blankValue;
                                ittr["Speed"] = Const.blankValue;
                                break;
                            case Enums.MessageType.Idling:
                            case Enums.MessageType.ExtendedIdling:
                            case Enums.MessageType.HarshBraking:
                            case Enums.MessageType.ExtremeBraking:
                            case Enums.MessageType.HarshAcceleration:
                            case Enums.MessageType.ExtremeAcceleration:
                            case Enums.MessageType.SeatBelt:
                                string speedDuration = "";
                                if (Enums.MessageType.Idling == msgType || Enums.MessageType.ExtendedIdling == msgType)
                                    speedDuration = Util.PairFindValue(Const.keyIdleDuration, customProp);
                                else
                                    speedDuration = Util.PairFindValue(Const.keyDuration, customProp);
                                if (speedDuration != "")
                                {
                                    try
                                    {
                                        ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(speedDuration) * TimeSpan.TicksPerSecond).ToString();
                                    }
                                    catch
                                    {
                                    }
                                }
                                break;
                            case Enums.MessageType.GPSAntenna:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyGPSAntenna, customProp);
                                break;

                            case Enums.MessageType.MDTResponse:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyAnswer, customProp);
                                break;
                            case Enums.MessageType.MDTSpecialMessage:
                                ittr["MsgDetails"] = customProp;
                                break;
                            case Enums.MessageType.MDTTextMessage:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                break;
                            case Enums.MessageType.Status:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyPower, customProp);
                                break;
                            case Enums.MessageType.RFIDCode:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.extFacilityCode, customProp) + "-" +
                                                      Util.PairFindValue(Const.extIDNumber, customProp);
                                break;
                            case Enums.MessageType.UpdateTelusPRLDone:
                                ittr["MsgDetails"] = "PRL=" + Util.PairFindValue(Const.keyPRLNumber, customProp);
                                break;
                            default:// do nothing
                                ittr["MsgDetails"] = " ";
                                break;
                        }
                        // in case of invalid GPS for current message set defaults
                        if (Convert.ToInt16(ittr["ValidGps"]) == 1)
                        {
                            ittr["Latitude"] = 0;
                            ittr["Longitude"] = 0;
                            ittr["Speed"] = Const.blankValue;
                            ittr["Heading"] = Const.blankValue;
                        }

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgInDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                }
            }
            return dsResult;
        }






        // Changes for TimeZone Feature start
        public DataSet GetMessagesFromHistoryByFleetId_ExtendedSearch_NewTZ(int userId,
                                                   bool isSuperUser,
                                                   int fleetId,
                                                   DateTime from,
                                                   DateTime to,
                                                   bool includeCoordinate,
                                                   bool includeSensor,
                                                   bool includePositionUpdate,
                                                   bool includeInvalidGps,
                                                   Int16 DclId,
                                                   string lang,
                                                   string address,
                                                   ref bool requestOverflowed,
                                                   ref int totalSqlRecords)
        {
            DataSet dsResult = null;
            string errorMsg = "Cannot retrieve messages from history";
            try
            {
                //localization settings
                Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

                requestOverflowed = false;

                // 2. Executes SQL statement
                SqlParameter[] sqlParams = new SqlParameter[14];

                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@fleetId", fleetId);
                sqlParams[2] = new SqlParameter("@dtFrom", from);
                sqlParams[3] = new SqlParameter("@dtTo", to);
                sqlParams[4] = new SqlParameter("@isSuperUser", isSuperUser);
                sqlParams[5] = new SqlParameter("@coordinate", includeCoordinate);
                sqlParams[6] = new SqlParameter("@sensor", includeSensor);
                sqlParams[7] = new SqlParameter("@posUpdate", includePositionUpdate);
                sqlParams[8] = new SqlParameter("@invalidGps", includeInvalidGps);
                sqlParams[9] = new SqlParameter("@dclId", DclId);
                sqlParams[10] = new SqlParameter("@lang", lang);
                sqlParams[11] = new SqlParameter("@address", address);
                sqlParams[12] = new SqlParameter("@total", totalSqlRecords);
                sqlParams[12].Direction = ParameterDirection.Output;
                sqlParams[13] = new SqlParameter("@over", requestOverflowed);
                sqlParams[13].Direction = ParameterDirection.Output;

                sqlExec.CommandTimeout = 600;
                dsResult = sqlExec.SPExecuteDataset("HistoryGetMessagesbyStreetAddress_FleetId_NewTimeZone", sqlParams);
                //int sqlTop = GetConfigParameter("ASI", (short)Enums.ConfigurationGroups.Common, "Report Max Records", 10000);
                if (requestOverflowed)
                {
                    return null;
                }
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(errorMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(String.Format("{0}:\n{1}", errorMsg, objException.Message));
            }

            //if (Util.IsDataSetValid(dsResult))
            //{
            //    // 1. Add additional column to result dataset
            //    dsResult.Tables[0].Columns.Add("MsgDetails", typeof(string));
            //    //int boxId = Const.unassignedIntValue;
            //    DataSet dsVehicleSensors = null;
            //    DataSet dsVehicleOutputs = null;
            //    string fenceDirection = "";

            //    // 2. Get user-defined sensors info (name,action)
            //    //[SensorId][SensorName][SensorAction][AlarmLevel]
            //    DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
            //    dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);

            //    //localization Sensors

            //    Localization local = new Localization(sqlExec.ConnectionString.ToString());

            //    Box boxCnf = new DB.Box(sqlExec);
            //    if ((lang != "en") && (lang != null))
            //    {

            //        Int16 hwTypeId = 0;
            //        DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
            //        if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
            //        {
            //            // TODO: Today box has only one HW type
            //            hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);
            //        }
            //        else
            //        {
            //            // TODO: write to log (cannot retrieve HW type for specific box)
            //        }
            //        local.LocalizationData(lang, "SensorId", "SensorName", "Sensors", hwTypeId, ref dsVehicleSensors);
            //        local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
            //    }



            //    // 3. Get user-defined outputs info 
            //    //[OutputId][OutputName][OutputAction]
            //    DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
            //    dsVehicleOutputs = outputs.GetOutputsInfoByBoxId(boxId, userId);

            //    //localization outputs
            //    if ((lang != "en") && (lang != null))
            //    {

            //        Int16 hwTypeId = 0;
            //        DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
            //        if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
            //        {
            //            // TODO: Today box has only one HW type
            //            hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);
            //        }
            //        else
            //        {
            //            // TODO: write to log (cannot retrieve HW type for specific box)
            //        }

            //        local.LocalizationData(lang, "OutputId", "OutputName", "Outputs", hwTypeId, ref dsVehicleOutputs);
            //        local.LocalizationDataAction(lang, "OutputId", "OutputAction", "Outputs", hwTypeId, ref dsVehicleOutputs);
            //    }


            //    //localization Commands
            //    DB.BoxProtocolTypeCmdOutType cmds = new DB.BoxProtocolTypeCmdOutType(sqlExec);
            //    DataSet dsVehicleCommands = cmds.GetAllSupportedCommands(boxId, userId);
            //    if ((lang != "en") && (lang != null))
            //        local.LocalizationData(lang, "BoxCmdOutTypeId", "BoxCmdOutTypeName", "CommandType", ref dsVehicleCommands);


            //    // 4. Resolve vehicle geozones
            //    DB.VehicleAssignment vehicleAssignment = null;
            //    DB.VehicleGeozone vehicleGeozone = null;
            //    DataSet dsGeozonesInfo = null;
            //    Int64 vehicleId = Const.unassignedIntValue;




            //    foreach (DataRow ittr in dsResult.Tables[0].Rows)
            //    {
            //        // a simple change which should make the code quicker and more readable 
            //        string customProp = ittr["CustomProp"].ToString().TrimEnd();

            //        if (Convert.ToInt16(ittr["MsgDirection"]) == Const.MsgOutDirectionValue)// process outgoing messages
            //        {
            //            switch ((Enums.CommandType)Convert.ToInt16(ittr["BoxMsgInTypeId"]))
            //            {
            //                case Enums.CommandType.Output:
            //                    ittr["MsgDetails"] = DB.Output.GetOutputDescription(customProp, dsVehicleOutputs.Tables[0]);
            //                    break;

            //                case Enums.CommandType.MDTTextMessage:
            //                    ittr["BoxMsgInTypeName"] = Resources.Const.TxtMessage;
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
            //                    ittr["MsgDetails"] += Convert.ToChar(13).ToString();
            //                    ittr["MsgDetails"] += Convert.ToChar(10).ToString();
            //                    ittr["MsgDetails"] += Resources.Const.TxtMsgAnswers + ":" + Util.PairFindValue(Const.keyAnswer, customProp).Replace("~", ",");
            //                    break;

            //                default:
            //                    ittr["MsgDetails"] = ittr["BoxMsgInTypeName"];
            //                    //localization commands
            //                    if ((lang != "en") && (lang != null))
            //                    {
            //                        foreach (DataRow dr in dsVehicleCommands.Tables[0].Rows)
            //                        {
            //                            if (Convert.ToInt16(ittr["BoxMsgInTypeId"]) == Convert.ToInt16(dr["BoxCmdOutTypeId"]))
            //                            {
            //                                ittr["MsgDetails"] = dr["BoxCmdOutTypeName"];
            //                                break;
            //                            }

            //                        }
            //                    }
            //                    ittr["BoxMsgInTypeName"] = Resources.Const.Command;
            //                    break;
            //            }
            //            ittr["StreetAddress"] = Const.blankValue; // not applicable

            //            // add message direction to message type
            //            ittr["BoxMsgInTypeName"] = Const.MsgOutDirectionSign + " " + ittr["BoxMsgInTypeName"];
            //        }
            //        else // process incomming messages
            //        {
            //            Enums.MessageType msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);

            //            switch (msgType)
            //            {
            //                case Enums.MessageType.Sensor:
            //                case Enums.MessageType.SensorExtended:
            //                case Enums.MessageType.Alarm:
            //                case Enums.MessageType.MBAlarm:
            //                case Enums.MessageType.ServiceRequired:
            //                    if (dsVehicleSensors != null && dsVehicleSensors.Tables.Count > 0)
            //                        ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, dsVehicleSensors.Tables[0]);
            //                    else
            //                        ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, null);
            //                    break;
            //                case Enums.MessageType.Speeding:      // was Speeding
            //                    {
            //                        string duration = Util.PairFindValue(Const.keySpeedDuration, customProp);
            //                        if (duration != "")
            //                        {
            //                            try
            //                            {
            //                                ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
            //                            }
            //                            catch
            //                            {
            //                            }
            //                        }
            //                        break;
            //                    }
            //                case Enums.MessageType.TetheredState:
            //                    {
            //                        string duration = Util.PairFindValue(Const.keyTetheredState, customProp);
            //                        if (duration != "")
            //                        {
            //                            try
            //                            {
            //                                ittr["MsgDetails"] = duration.Equals(Const.valON) ? Resources.Const.ON : Resources.Const.OFF;
            //                            }
            //                            catch
            //                            {
            //                            }
            //                        }
            //                        break;
            //                    }
            //                case Enums.MessageType.GeoFence:
            //                    //fenceNo = Util.PairFindValue(Const.keyFenceNo,customProp);
            //                    //if(fenceNo == null || fenceNo == "")
            //                    //	fenceNo = "Unknown";

            //                    fenceDirection = Util.PairFindValue(Const.keyFenceDir, customProp);
            //                    if (fenceDirection == null || fenceDirection == "")
            //                        fenceDirection = Resources.Const.fenceDirection_unknown;
            //                    else if (Const.valFenceDirIn == fenceDirection)
            //                        fenceDirection = Resources.Const.fenceDirection_inbroken;
            //                    else if (Const.valFenceDirOut == fenceDirection)
            //                        fenceDirection = Resources.Const.fenceDirection_outbroken;

            //                    ittr["MsgDetails"] = fenceDirection;
            //                    break;
            //                case Enums.MessageType.GeoZone:
            //                    if (vehicleAssignment == null)
            //                    {
            //                        vehicleAssignment = new DB.VehicleAssignment(sqlExec);
            //                        vehicleId = Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
            //                    }
            //                    if (vehicleGeozone == null)
            //                    {
            //                        vehicleGeozone = new DB.VehicleGeozone(sqlExec);
            //                        dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
            //                    }

            //                    if (dsGeozonesInfo != null && dsGeozonesInfo.Tables.Count > 0)
            //                        ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, dsGeozonesInfo.Tables[0]);
            //                    else
            //                        ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, null);
            //                    break;
            //                case Enums.MessageType.IPUpdate:
            //                    ittr["MsgDetails"] = ittr["CommInfo1"].ToString() + ":" + ittr["CommInfo2"].ToString().TrimEnd();
            //                    ittr["StreetAddress"] = Const.blankValue;
            //                    ittr["Speed"] = Const.blankValue;
            //                    break;
            //                case Enums.MessageType.Idling:
            //                case Enums.MessageType.ExtendedIdling:
            //                case Enums.MessageType.HarshBraking:
            //                case Enums.MessageType.ExtremeBraking:
            //                case Enums.MessageType.HarshAcceleration:
            //                case Enums.MessageType.ExtremeAcceleration:
            //                case Enums.MessageType.SeatBelt:
            //                    string speedDuration = "";
            //                    if (Enums.MessageType.Idling == msgType || Enums.MessageType.ExtendedIdling == msgType)
            //                        speedDuration = Util.PairFindValue(Const.keyIdleDuration, customProp);
            //                    else
            //                        speedDuration = Util.PairFindValue(Const.keyDuration, customProp);
            //                    if (speedDuration != "")
            //                    {
            //                        try
            //                        {
            //                            ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(speedDuration) * TimeSpan.TicksPerSecond).ToString();
            //                        }
            //                        catch
            //                        {
            //                        }
            //                    }
            //                    break;
            //                case Enums.MessageType.GPSAntenna:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyGPSAntenna, customProp);
            //                    break;

            //                case Enums.MessageType.MDTResponse:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyAnswer, customProp);
            //                    break;
            //                case Enums.MessageType.MDTSpecialMessage:
            //                    ittr["MsgDetails"] = customProp;
            //                    break;
            //                case Enums.MessageType.MDTTextMessage:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
            //                    break;
            //                case Enums.MessageType.Status:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyPower, customProp);
            //                    break;
            //                case Enums.MessageType.RFIDCode:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.extFacilityCode, customProp) + "-" +
            //                                          Util.PairFindValue(Const.extIDNumber, customProp);
            //                    break;
            //                case Enums.MessageType.UpdateTelusPRLDone:
            //                    ittr["MsgDetails"] = "PRL=" + Util.PairFindValue(Const.keyPRLNumber, customProp);
            //                    break;
            //                default:// do nothing
            //                    ittr["MsgDetails"] = " ";
            //                    break;
            //            }
            //            // in case of invalid GPS for current message set defaults
            //            if (Convert.ToInt16(ittr["ValidGps"]) == 1)
            //            {
            //                ittr["Latitude"] = 0;
            //                ittr["Longitude"] = 0;
            //                ittr["Speed"] = Const.blankValue;
            //                ittr["Heading"] = Const.blankValue;
            //            }

            //            // add message direction to message type
            //            ittr["BoxMsgInTypeName"] = Const.MsgInDirectionSign + " " + ittr["BoxMsgInTypeName"];
            //        }
            //    }
            //}
            return dsResult;
        }
        // Changes for TimeZone Feature end



        public DataSet GetMessagesFromHistoryByFleetId_ExtendedSearch(int userId,
                                                   bool isSuperUser,
                                                   int fleetId,
                                                   DateTime from,
                                                   DateTime to,
                                                   bool includeCoordinate,
                                                   bool includeSensor,
                                                   bool includePositionUpdate,
                                                   bool includeInvalidGps,
                                                   Int16 DclId,
                                                   string lang,
                                                   string address,
                                                   ref bool requestOverflowed,
           
            
            ref int totalSqlRecords)
        {
            DataSet dsResult = null;
            string errorMsg = "Cannot retrieve messages from history";
            try
            {
                //localization settings
                Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

                requestOverflowed = false;

                // 2. Executes SQL statement
                SqlParameter[] sqlParams = new SqlParameter[14];

                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@fleetId", fleetId);
                sqlParams[2] = new SqlParameter("@dtFrom", from);
                sqlParams[3] = new SqlParameter("@dtTo", to);
                sqlParams[4] = new SqlParameter("@isSuperUser", isSuperUser);
                sqlParams[5] = new SqlParameter("@coordinate", includeCoordinate);
                sqlParams[6] = new SqlParameter("@sensor", includeSensor);
                sqlParams[7] = new SqlParameter("@posUpdate", includePositionUpdate);
                sqlParams[8] = new SqlParameter("@invalidGps", includeInvalidGps);
                sqlParams[9] = new SqlParameter("@dclId", DclId);
                sqlParams[10] = new SqlParameter("@lang", lang);
                sqlParams[11] = new SqlParameter("@address", address);
                sqlParams[12] = new SqlParameter("@total", totalSqlRecords);
                sqlParams[12].Direction = ParameterDirection.Output;
                sqlParams[13] = new SqlParameter("@over", requestOverflowed);
                sqlParams[13].Direction = ParameterDirection.Output;

                sqlExec.CommandTimeout = 600;
                dsResult = sqlExec.SPExecuteDataset("HistoryGetMessagesbyStreetAddress_FleetId", sqlParams);
                //int sqlTop = GetConfigParameter("ASI", (short)Enums.ConfigurationGroups.Common, "Report Max Records", 10000);
                if (requestOverflowed)
                {
                    return null;
                }
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(errorMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(String.Format("{0}:\n{1}", errorMsg, objException.Message));
            }

            //if (Util.IsDataSetValid(dsResult))
            //{
            //    // 1. Add additional column to result dataset
            //    dsResult.Tables[0].Columns.Add("MsgDetails", typeof(string));
            //    //int boxId = Const.unassignedIntValue;
            //    DataSet dsVehicleSensors = null;
            //    DataSet dsVehicleOutputs = null;
            //    string fenceDirection = "";

            //    // 2. Get user-defined sensors info (name,action)
            //    //[SensorId][SensorName][SensorAction][AlarmLevel]
            //    DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
            //    dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);

            //    //localization Sensors

            //    Localization local = new Localization(sqlExec.ConnectionString.ToString());

            //    Box boxCnf = new DB.Box(sqlExec);
            //    if ((lang != "en") && (lang != null))
            //    {

            //        Int16 hwTypeId = 0;
            //        DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
            //        if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
            //        {
            //            // TODO: Today box has only one HW type
            //            hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);
            //        }
            //        else
            //        {
            //            // TODO: write to log (cannot retrieve HW type for specific box)
            //        }
            //        local.LocalizationData(lang, "SensorId", "SensorName", "Sensors", hwTypeId, ref dsVehicleSensors);
            //        local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
            //    }



            //    // 3. Get user-defined outputs info 
            //    //[OutputId][OutputName][OutputAction]
            //    DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
            //    dsVehicleOutputs = outputs.GetOutputsInfoByBoxId(boxId, userId);

            //    //localization outputs
            //    if ((lang != "en") && (lang != null))
            //    {

            //        Int16 hwTypeId = 0;
            //        DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
            //        if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
            //        {
            //            // TODO: Today box has only one HW type
            //            hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);
            //        }
            //        else
            //        {
            //            // TODO: write to log (cannot retrieve HW type for specific box)
            //        }

            //        local.LocalizationData(lang, "OutputId", "OutputName", "Outputs", hwTypeId, ref dsVehicleOutputs);
            //        local.LocalizationDataAction(lang, "OutputId", "OutputAction", "Outputs", hwTypeId, ref dsVehicleOutputs);
            //    }


            //    //localization Commands
            //    DB.BoxProtocolTypeCmdOutType cmds = new DB.BoxProtocolTypeCmdOutType(sqlExec);
            //    DataSet dsVehicleCommands = cmds.GetAllSupportedCommands(boxId, userId);
            //    if ((lang != "en") && (lang != null))
            //        local.LocalizationData(lang, "BoxCmdOutTypeId", "BoxCmdOutTypeName", "CommandType", ref dsVehicleCommands);


            //    // 4. Resolve vehicle geozones
            //    DB.VehicleAssignment vehicleAssignment = null;
            //    DB.VehicleGeozone vehicleGeozone = null;
            //    DataSet dsGeozonesInfo = null;
            //    Int64 vehicleId = Const.unassignedIntValue;




            //    foreach (DataRow ittr in dsResult.Tables[0].Rows)
            //    {
            //        // a simple change which should make the code quicker and more readable 
            //        string customProp = ittr["CustomProp"].ToString().TrimEnd();

            //        if (Convert.ToInt16(ittr["MsgDirection"]) == Const.MsgOutDirectionValue)// process outgoing messages
            //        {
            //            switch ((Enums.CommandType)Convert.ToInt16(ittr["BoxMsgInTypeId"]))
            //            {
            //                case Enums.CommandType.Output:
            //                    ittr["MsgDetails"] = DB.Output.GetOutputDescription(customProp, dsVehicleOutputs.Tables[0]);
            //                    break;

            //                case Enums.CommandType.MDTTextMessage:
            //                    ittr["BoxMsgInTypeName"] = Resources.Const.TxtMessage;
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
            //                    ittr["MsgDetails"] += Convert.ToChar(13).ToString();
            //                    ittr["MsgDetails"] += Convert.ToChar(10).ToString();
            //                    ittr["MsgDetails"] += Resources.Const.TxtMsgAnswers + ":" + Util.PairFindValue(Const.keyAnswer, customProp).Replace("~", ",");
            //                    break;

            //                default:
            //                    ittr["MsgDetails"] = ittr["BoxMsgInTypeName"];
            //                    //localization commands
            //                    if ((lang != "en") && (lang != null))
            //                    {
            //                        foreach (DataRow dr in dsVehicleCommands.Tables[0].Rows)
            //                        {
            //                            if (Convert.ToInt16(ittr["BoxMsgInTypeId"]) == Convert.ToInt16(dr["BoxCmdOutTypeId"]))
            //                            {
            //                                ittr["MsgDetails"] = dr["BoxCmdOutTypeName"];
            //                                break;
            //                            }

            //                        }
            //                    }
            //                    ittr["BoxMsgInTypeName"] = Resources.Const.Command;
            //                    break;
            //            }
            //            ittr["StreetAddress"] = Const.blankValue; // not applicable

            //            // add message direction to message type
            //            ittr["BoxMsgInTypeName"] = Const.MsgOutDirectionSign + " " + ittr["BoxMsgInTypeName"];
            //        }
            //        else // process incomming messages
            //        {
            //            Enums.MessageType msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);

            //            switch (msgType)
            //            {
            //                case Enums.MessageType.Sensor:
            //                case Enums.MessageType.SensorExtended:
            //                case Enums.MessageType.Alarm:
            //                case Enums.MessageType.MBAlarm:
            //                case Enums.MessageType.ServiceRequired:
            //                    if (dsVehicleSensors != null && dsVehicleSensors.Tables.Count > 0)
            //                        ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, dsVehicleSensors.Tables[0]);
            //                    else
            //                        ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, null);
            //                    break;
            //                case Enums.MessageType.Speeding:      // was Speeding
            //                    {
            //                        string duration = Util.PairFindValue(Const.keySpeedDuration, customProp);
            //                        if (duration != "")
            //                        {
            //                            try
            //                            {
            //                                ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
            //                            }
            //                            catch
            //                            {
            //                            }
            //                        }
            //                        break;
            //                    }
            //                case Enums.MessageType.TetheredState:
            //                    {
            //                        string duration = Util.PairFindValue(Const.keyTetheredState, customProp);
            //                        if (duration != "")
            //                        {
            //                            try
            //                            {
            //                                ittr["MsgDetails"] = duration.Equals(Const.valON) ? Resources.Const.ON : Resources.Const.OFF;
            //                            }
            //                            catch
            //                            {
            //                            }
            //                        }
            //                        break;
            //                    }
            //                case Enums.MessageType.GeoFence:
            //                    //fenceNo = Util.PairFindValue(Const.keyFenceNo,customProp);
            //                    //if(fenceNo == null || fenceNo == "")
            //                    //	fenceNo = "Unknown";

            //                    fenceDirection = Util.PairFindValue(Const.keyFenceDir, customProp);
            //                    if (fenceDirection == null || fenceDirection == "")
            //                        fenceDirection = Resources.Const.fenceDirection_unknown;
            //                    else if (Const.valFenceDirIn == fenceDirection)
            //                        fenceDirection = Resources.Const.fenceDirection_inbroken;
            //                    else if (Const.valFenceDirOut == fenceDirection)
            //                        fenceDirection = Resources.Const.fenceDirection_outbroken;

            //                    ittr["MsgDetails"] = fenceDirection;
            //                    break;
            //                case Enums.MessageType.GeoZone:
            //                    if (vehicleAssignment == null)
            //                    {
            //                        vehicleAssignment = new DB.VehicleAssignment(sqlExec);
            //                        vehicleId = Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
            //                    }
            //                    if (vehicleGeozone == null)
            //                    {
            //                        vehicleGeozone = new DB.VehicleGeozone(sqlExec);
            //                        dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
            //                    }

            //                    if (dsGeozonesInfo != null && dsGeozonesInfo.Tables.Count > 0)
            //                        ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, dsGeozonesInfo.Tables[0]);
            //                    else
            //                        ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, null);
            //                    break;
            //                case Enums.MessageType.IPUpdate:
            //                    ittr["MsgDetails"] = ittr["CommInfo1"].ToString() + ":" + ittr["CommInfo2"].ToString().TrimEnd();
            //                    ittr["StreetAddress"] = Const.blankValue;
            //                    ittr["Speed"] = Const.blankValue;
            //                    break;
            //                case Enums.MessageType.Idling:
            //                case Enums.MessageType.ExtendedIdling:
            //                case Enums.MessageType.HarshBraking:
            //                case Enums.MessageType.ExtremeBraking:
            //                case Enums.MessageType.HarshAcceleration:
            //                case Enums.MessageType.ExtremeAcceleration:
            //                case Enums.MessageType.SeatBelt:
            //                    string speedDuration = "";
            //                    if (Enums.MessageType.Idling == msgType || Enums.MessageType.ExtendedIdling == msgType)
            //                        speedDuration = Util.PairFindValue(Const.keyIdleDuration, customProp);
            //                    else
            //                        speedDuration = Util.PairFindValue(Const.keyDuration, customProp);
            //                    if (speedDuration != "")
            //                    {
            //                        try
            //                        {
            //                            ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(speedDuration) * TimeSpan.TicksPerSecond).ToString();
            //                        }
            //                        catch
            //                        {
            //                        }
            //                    }
            //                    break;
            //                case Enums.MessageType.GPSAntenna:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyGPSAntenna, customProp);
            //                    break;

            //                case Enums.MessageType.MDTResponse:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyAnswer, customProp);
            //                    break;
            //                case Enums.MessageType.MDTSpecialMessage:
            //                    ittr["MsgDetails"] = customProp;
            //                    break;
            //                case Enums.MessageType.MDTTextMessage:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
            //                    break;
            //                case Enums.MessageType.Status:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyPower, customProp);
            //                    break;
            //                case Enums.MessageType.RFIDCode:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.extFacilityCode, customProp) + "-" +
            //                                          Util.PairFindValue(Const.extIDNumber, customProp);
            //                    break;
            //                case Enums.MessageType.UpdateTelusPRLDone:
            //                    ittr["MsgDetails"] = "PRL=" + Util.PairFindValue(Const.keyPRLNumber, customProp);
            //                    break;
            //                default:// do nothing
            //                    ittr["MsgDetails"] = " ";
            //                    break;
            //            }
            //            // in case of invalid GPS for current message set defaults
            //            if (Convert.ToInt16(ittr["ValidGps"]) == 1)
            //            {
            //                ittr["Latitude"] = 0;
            //                ittr["Longitude"] = 0;
            //                ittr["Speed"] = Const.blankValue;
            //                ittr["Heading"] = Const.blankValue;
            //            }

            //            // add message direction to message type
            //            ittr["BoxMsgInTypeName"] = Const.MsgInDirectionSign + " " + ittr["BoxMsgInTypeName"];
            //        }
            //    }
            //}
            return dsResult;
        }

        // Changes for TimeZone Feature start
        public DataSet HistoryGetMessagesbyFleetId_NewTZ(int userId,
                                                 bool isSuperUser,
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
            DataSet dsResult = null;
            string errorMsg = "Cannot retrieve messages from history";
            try
            {
                //localization settings
                Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

                requestOverflowed = false;

                // 2. Executes SQL statement
                SqlParameter[] sqlParams = new SqlParameter[13];

                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@fleetId", fleetId);
                sqlParams[2] = new SqlParameter("@dtFrom", from);
                sqlParams[3] = new SqlParameter("@dtTo", to);
                sqlParams[4] = new SqlParameter("@isSuperUser", isSuperUser);
                sqlParams[5] = new SqlParameter("@coordinate", includeCoordinate);
                sqlParams[6] = new SqlParameter("@sensor", includeSensor);
                sqlParams[7] = new SqlParameter("@posUpdate", includePositionUpdate);
                sqlParams[8] = new SqlParameter("@invalidGps", includeInvalidGps);
                sqlParams[9] = new SqlParameter("@dclId", DclId);
                sqlParams[10] = new SqlParameter("@lang", lang);
                sqlParams[11] = new SqlParameter("@total", totalSqlRecords);
                sqlParams[11].Direction = ParameterDirection.Output;
                sqlParams[12] = new SqlParameter("@over", requestOverflowed);
                sqlParams[12].Direction = ParameterDirection.Output;

                sqlExec.CommandTimeout = 600;
                dsResult = sqlExec.SPExecuteDataset("HistoryGetMessagesbyFleetId_NewTimeZone", sqlParams);
                //int sqlTop = GetConfigParameter("ASI", (short)Enums.ConfigurationGroups.Common, "Report Max Records", 10000);
                if (requestOverflowed)
                {
                    return null;
                }
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(errorMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(String.Format("{0}:\n{1}", errorMsg, objException.Message));
            }

            //if (Util.IsDataSetValid(dsResult))
            //{
            //    // 1. Add additional column to result dataset
            //    dsResult.Tables[0].Columns.Add("MsgDetails", typeof(string));
            //    //int boxId = Const.unassignedIntValue;
            //    DataSet dsVehicleSensors = null;
            //    DataSet dsVehicleOutputs = null;
            //    string fenceDirection = "";

            //    // 2. Get user-defined sensors info (name,action)
            //    //[SensorId][SensorName][SensorAction][AlarmLevel]
            //    DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
            //    dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);

            //    //localization Sensors

            //    Localization local = new Localization(sqlExec.ConnectionString.ToString());

            //    Box boxCnf = new DB.Box(sqlExec);
            //    if ((lang != "en") && (lang != null))
            //    {

            //        Int16 hwTypeId = 0;
            //        DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
            //        if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
            //        {
            //            // TODO: Today box has only one HW type
            //            hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);
            //        }
            //        else
            //        {
            //            // TODO: write to log (cannot retrieve HW type for specific box)
            //        }
            //        local.LocalizationData(lang, "SensorId", "SensorName", "Sensors", hwTypeId, ref dsVehicleSensors);
            //        local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
            //    }



            //    // 3. Get user-defined outputs info 
            //    //[OutputId][OutputName][OutputAction]
            //    DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
            //    dsVehicleOutputs = outputs.GetOutputsInfoByBoxId(boxId, userId);

            //    //localization outputs
            //    if ((lang != "en") && (lang != null))
            //    {

            //        Int16 hwTypeId = 0;
            //        DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
            //        if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
            //        {
            //            // TODO: Today box has only one HW type
            //            hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);
            //        }
            //        else
            //        {
            //            // TODO: write to log (cannot retrieve HW type for specific box)
            //        }

            //        local.LocalizationData(lang, "OutputId", "OutputName", "Outputs", hwTypeId, ref dsVehicleOutputs);
            //        local.LocalizationDataAction(lang, "OutputId", "OutputAction", "Outputs", hwTypeId, ref dsVehicleOutputs);
            //    }


            //    //localization Commands
            //    DB.BoxProtocolTypeCmdOutType cmds = new DB.BoxProtocolTypeCmdOutType(sqlExec);
            //    DataSet dsVehicleCommands = cmds.GetAllSupportedCommands(boxId, userId);
            //    if ((lang != "en") && (lang != null))
            //        local.LocalizationData(lang, "BoxCmdOutTypeId", "BoxCmdOutTypeName", "CommandType", ref dsVehicleCommands);


            //    // 4. Resolve vehicle geozones
            //    DB.VehicleAssignment vehicleAssignment = null;
            //    DB.VehicleGeozone vehicleGeozone = null;
            //    DataSet dsGeozonesInfo = null;
            //    Int64 vehicleId = Const.unassignedIntValue;




            //    foreach (DataRow ittr in dsResult.Tables[0].Rows)
            //    {
            //        // a simple change which should make the code quicker and more readable 
            //        string customProp = ittr["CustomProp"].ToString().TrimEnd();

            //        if (Convert.ToInt16(ittr["MsgDirection"]) == Const.MsgOutDirectionValue)// process outgoing messages
            //        {
            //            switch ((Enums.CommandType)Convert.ToInt16(ittr["BoxMsgInTypeId"]))
            //            {
            //                case Enums.CommandType.Output:
            //                    ittr["MsgDetails"] = DB.Output.GetOutputDescription(customProp, dsVehicleOutputs.Tables[0]);
            //                    break;

            //                case Enums.CommandType.MDTTextMessage:
            //                    ittr["BoxMsgInTypeName"] = Resources.Const.TxtMessage;
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
            //                    ittr["MsgDetails"] += Convert.ToChar(13).ToString();
            //                    ittr["MsgDetails"] += Convert.ToChar(10).ToString();
            //                    ittr["MsgDetails"] += Resources.Const.TxtMsgAnswers + ":" + Util.PairFindValue(Const.keyAnswer, customProp).Replace("~", ",");
            //                    break;

            //                default:
            //                    ittr["MsgDetails"] = ittr["BoxMsgInTypeName"];
            //                    //localization commands
            //                    if ((lang != "en") && (lang != null))
            //                    {
            //                        foreach (DataRow dr in dsVehicleCommands.Tables[0].Rows)
            //                        {
            //                            if (Convert.ToInt16(ittr["BoxMsgInTypeId"]) == Convert.ToInt16(dr["BoxCmdOutTypeId"]))
            //                            {
            //                                ittr["MsgDetails"] = dr["BoxCmdOutTypeName"];
            //                                break;
            //                            }

            //                        }
            //                    }
            //                    ittr["BoxMsgInTypeName"] = Resources.Const.Command;
            //                    break;
            //            }
            //            ittr["StreetAddress"] = Const.blankValue; // not applicable

            //            // add message direction to message type
            //            ittr["BoxMsgInTypeName"] = Const.MsgOutDirectionSign + " " + ittr["BoxMsgInTypeName"];
            //        }
            //        else // process incomming messages
            //        {
            //            Enums.MessageType msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);

            //            switch (msgType)
            //            {
            //                case Enums.MessageType.Sensor:
            //                case Enums.MessageType.SensorExtended:
            //                case Enums.MessageType.Alarm:
            //                case Enums.MessageType.MBAlarm:
            //                case Enums.MessageType.ServiceRequired:
            //                    if (dsVehicleSensors != null && dsVehicleSensors.Tables.Count > 0)
            //                        ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, dsVehicleSensors.Tables[0]);
            //                    else
            //                        ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, null);
            //                    break;
            //                case Enums.MessageType.Speeding:      // was Speeding
            //                    {
            //                        string duration = Util.PairFindValue(Const.keySpeedDuration, customProp);
            //                        if (duration != "")
            //                        {
            //                            try
            //                            {
            //                                ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
            //                            }
            //                            catch
            //                            {
            //                            }
            //                        }
            //                        break;
            //                    }
            //                case Enums.MessageType.TetheredState:
            //                    {
            //                        string duration = Util.PairFindValue(Const.keyTetheredState, customProp);
            //                        if (duration != "")
            //                        {
            //                            try
            //                            {
            //                                ittr["MsgDetails"] = duration.Equals(Const.valON) ? Resources.Const.ON : Resources.Const.OFF;
            //                            }
            //                            catch
            //                            {
            //                            }
            //                        }
            //                        break;
            //                    }
            //                case Enums.MessageType.GeoFence:
            //                    //fenceNo = Util.PairFindValue(Const.keyFenceNo,customProp);
            //                    //if(fenceNo == null || fenceNo == "")
            //                    //	fenceNo = "Unknown";

            //                    fenceDirection = Util.PairFindValue(Const.keyFenceDir, customProp);
            //                    if (fenceDirection == null || fenceDirection == "")
            //                        fenceDirection = Resources.Const.fenceDirection_unknown;
            //                    else if (Const.valFenceDirIn == fenceDirection)
            //                        fenceDirection = Resources.Const.fenceDirection_inbroken;
            //                    else if (Const.valFenceDirOut == fenceDirection)
            //                        fenceDirection = Resources.Const.fenceDirection_outbroken;

            //                    ittr["MsgDetails"] = fenceDirection;
            //                    break;
            //                case Enums.MessageType.GeoZone:
            //                    if (vehicleAssignment == null)
            //                    {
            //                        vehicleAssignment = new DB.VehicleAssignment(sqlExec);
            //                        vehicleId = Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
            //                    }
            //                    if (vehicleGeozone == null)
            //                    {
            //                        vehicleGeozone = new DB.VehicleGeozone(sqlExec);
            //                        dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
            //                    }

            //                    if (dsGeozonesInfo != null && dsGeozonesInfo.Tables.Count > 0)
            //                        ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, dsGeozonesInfo.Tables[0]);
            //                    else
            //                        ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, null);
            //                    break;
            //                case Enums.MessageType.IPUpdate:
            //                    ittr["MsgDetails"] = ittr["CommInfo1"].ToString() + ":" + ittr["CommInfo2"].ToString().TrimEnd();
            //                    ittr["StreetAddress"] = Const.blankValue;
            //                    ittr["Speed"] = Const.blankValue;
            //                    break;
            //                case Enums.MessageType.Idling:
            //                case Enums.MessageType.ExtendedIdling:
            //                case Enums.MessageType.HarshBraking:
            //                case Enums.MessageType.ExtremeBraking:
            //                case Enums.MessageType.HarshAcceleration:
            //                case Enums.MessageType.ExtremeAcceleration:
            //                case Enums.MessageType.SeatBelt:
            //                    string speedDuration = "";
            //                    if (Enums.MessageType.Idling == msgType || Enums.MessageType.ExtendedIdling == msgType)
            //                        speedDuration = Util.PairFindValue(Const.keyIdleDuration, customProp);
            //                    else
            //                        speedDuration = Util.PairFindValue(Const.keyDuration, customProp);
            //                    if (speedDuration != "")
            //                    {
            //                        try
            //                        {
            //                            ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(speedDuration) * TimeSpan.TicksPerSecond).ToString();
            //                        }
            //                        catch
            //                        {
            //                        }
            //                    }
            //                    break;
            //                case Enums.MessageType.GPSAntenna:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyGPSAntenna, customProp);
            //                    break;

            //                case Enums.MessageType.MDTResponse:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyAnswer, customProp);
            //                    break;
            //                case Enums.MessageType.MDTSpecialMessage:
            //                    ittr["MsgDetails"] = customProp;
            //                    break;
            //                case Enums.MessageType.MDTTextMessage:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
            //                    break;
            //                case Enums.MessageType.Status:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyPower, customProp);
            //                    break;
            //                case Enums.MessageType.RFIDCode:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.extFacilityCode, customProp) + "-" +
            //                                          Util.PairFindValue(Const.extIDNumber, customProp);
            //                    break;
            //                case Enums.MessageType.UpdateTelusPRLDone:
            //                    ittr["MsgDetails"] = "PRL=" + Util.PairFindValue(Const.keyPRLNumber, customProp);
            //                    break;
            //                default:// do nothing
            //                    ittr["MsgDetails"] = " ";
            //                    break;
            //            }
            //            // in case of invalid GPS for current message set defaults
            //            if (Convert.ToInt16(ittr["ValidGps"]) == 1)
            //            {
            //                ittr["Latitude"] = 0;
            //                ittr["Longitude"] = 0;
            //                ittr["Speed"] = Const.blankValue;
            //                ittr["Heading"] = Const.blankValue;
            //            }

            //            // add message direction to message type
            //            ittr["BoxMsgInTypeName"] = Const.MsgInDirectionSign + " " + ittr["BoxMsgInTypeName"];
            //        }
            //    }
            //}
            return dsResult;
        }
        // Changes for TimeZone Feature end



        public DataSet HistoryGetMessagesbyFleetId(int userId,
                                                  bool isSuperUser,
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
            DataSet dsResult = null;
            string errorMsg = "Cannot retrieve messages from history";
            try
            {
                //localization settings
                Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

                requestOverflowed = false;

                // 2. Executes SQL statement
                SqlParameter[] sqlParams = new SqlParameter[13];

                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@fleetId", fleetId);
                sqlParams[2] = new SqlParameter("@dtFrom", from);
                sqlParams[3] = new SqlParameter("@dtTo", to);
                sqlParams[4] = new SqlParameter("@isSuperUser", isSuperUser);
                sqlParams[5] = new SqlParameter("@coordinate", includeCoordinate);
                sqlParams[6] = new SqlParameter("@sensor", includeSensor);
                sqlParams[7] = new SqlParameter("@posUpdate", includePositionUpdate);
                sqlParams[8] = new SqlParameter("@invalidGps", includeInvalidGps);
                sqlParams[9] = new SqlParameter("@dclId", DclId);
                sqlParams[10] = new SqlParameter("@lang", lang);
                sqlParams[11] = new SqlParameter("@total", totalSqlRecords);
                sqlParams[11].Direction = ParameterDirection.Output;
                sqlParams[12] = new SqlParameter("@over", requestOverflowed);
                sqlParams[12].Direction = ParameterDirection.Output;

                sqlExec.CommandTimeout = 600;
                dsResult = sqlExec.SPExecuteDataset("HistoryGetMessagesbyFleetId", sqlParams);
                //int sqlTop = GetConfigParameter("ASI", (short)Enums.ConfigurationGroups.Common, "Report Max Records", 10000);
                if (requestOverflowed)
                {
                    return null;
                }
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(errorMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(String.Format("{0}:\n{1}", errorMsg, objException.Message));
            }

            //if (Util.IsDataSetValid(dsResult))
            //{
            //    // 1. Add additional column to result dataset
            //    dsResult.Tables[0].Columns.Add("MsgDetails", typeof(string));
            //    //int boxId = Const.unassignedIntValue;
            //    DataSet dsVehicleSensors = null;
            //    DataSet dsVehicleOutputs = null;
            //    string fenceDirection = "";

            //    // 2. Get user-defined sensors info (name,action)
            //    //[SensorId][SensorName][SensorAction][AlarmLevel]
            //    DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
            //    dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);

            //    //localization Sensors

            //    Localization local = new Localization(sqlExec.ConnectionString.ToString());

            //    Box boxCnf = new DB.Box(sqlExec);
            //    if ((lang != "en") && (lang != null))
            //    {

            //        Int16 hwTypeId = 0;
            //        DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
            //        if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
            //        {
            //            // TODO: Today box has only one HW type
            //            hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);
            //        }
            //        else
            //        {
            //            // TODO: write to log (cannot retrieve HW type for specific box)
            //        }
            //        local.LocalizationData(lang, "SensorId", "SensorName", "Sensors", hwTypeId, ref dsVehicleSensors);
            //        local.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsVehicleSensors);
            //    }



            //    // 3. Get user-defined outputs info 
            //    //[OutputId][OutputName][OutputAction]
            //    DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
            //    dsVehicleOutputs = outputs.GetOutputsInfoByBoxId(boxId, userId);

            //    //localization outputs
            //    if ((lang != "en") && (lang != null))
            //    {

            //        Int16 hwTypeId = 0;
            //        DataSet dsBoxConfig = boxCnf.GetBoxConfigInfo(boxId);
            //        if (dsBoxConfig != null && dsBoxConfig.Tables.Count > 0 && dsBoxConfig.Tables[0].Rows.Count > 0)
            //        {
            //            // TODO: Today box has only one HW type
            //            hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);
            //        }
            //        else
            //        {
            //            // TODO: write to log (cannot retrieve HW type for specific box)
            //        }

            //        local.LocalizationData(lang, "OutputId", "OutputName", "Outputs", hwTypeId, ref dsVehicleOutputs);
            //        local.LocalizationDataAction(lang, "OutputId", "OutputAction", "Outputs", hwTypeId, ref dsVehicleOutputs);
            //    }


            //    //localization Commands
            //    DB.BoxProtocolTypeCmdOutType cmds = new DB.BoxProtocolTypeCmdOutType(sqlExec);
            //    DataSet dsVehicleCommands = cmds.GetAllSupportedCommands(boxId, userId);
            //    if ((lang != "en") && (lang != null))
            //        local.LocalizationData(lang, "BoxCmdOutTypeId", "BoxCmdOutTypeName", "CommandType", ref dsVehicleCommands);


            //    // 4. Resolve vehicle geozones
            //    DB.VehicleAssignment vehicleAssignment = null;
            //    DB.VehicleGeozone vehicleGeozone = null;
            //    DataSet dsGeozonesInfo = null;
            //    Int64 vehicleId = Const.unassignedIntValue;




            //    foreach (DataRow ittr in dsResult.Tables[0].Rows)
            //    {
            //        // a simple change which should make the code quicker and more readable 
            //        string customProp = ittr["CustomProp"].ToString().TrimEnd();

            //        if (Convert.ToInt16(ittr["MsgDirection"]) == Const.MsgOutDirectionValue)// process outgoing messages
            //        {
            //            switch ((Enums.CommandType)Convert.ToInt16(ittr["BoxMsgInTypeId"]))
            //            {
            //                case Enums.CommandType.Output:
            //                    ittr["MsgDetails"] = DB.Output.GetOutputDescription(customProp, dsVehicleOutputs.Tables[0]);
            //                    break;

            //                case Enums.CommandType.MDTTextMessage:
            //                    ittr["BoxMsgInTypeName"] = Resources.Const.TxtMessage;
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
            //                    ittr["MsgDetails"] += Convert.ToChar(13).ToString();
            //                    ittr["MsgDetails"] += Convert.ToChar(10).ToString();
            //                    ittr["MsgDetails"] += Resources.Const.TxtMsgAnswers + ":" + Util.PairFindValue(Const.keyAnswer, customProp).Replace("~", ",");
            //                    break;

            //                default:
            //                    ittr["MsgDetails"] = ittr["BoxMsgInTypeName"];
            //                    //localization commands
            //                    if ((lang != "en") && (lang != null))
            //                    {
            //                        foreach (DataRow dr in dsVehicleCommands.Tables[0].Rows)
            //                        {
            //                            if (Convert.ToInt16(ittr["BoxMsgInTypeId"]) == Convert.ToInt16(dr["BoxCmdOutTypeId"]))
            //                            {
            //                                ittr["MsgDetails"] = dr["BoxCmdOutTypeName"];
            //                                break;
            //                            }

            //                        }
            //                    }
            //                    ittr["BoxMsgInTypeName"] = Resources.Const.Command;
            //                    break;
            //            }
            //            ittr["StreetAddress"] = Const.blankValue; // not applicable

            //            // add message direction to message type
            //            ittr["BoxMsgInTypeName"] = Const.MsgOutDirectionSign + " " + ittr["BoxMsgInTypeName"];
            //        }
            //        else // process incomming messages
            //        {
            //            Enums.MessageType msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);

            //            switch (msgType)
            //            {
            //                case Enums.MessageType.Sensor:
            //                case Enums.MessageType.SensorExtended:
            //                case Enums.MessageType.Alarm:
            //                case Enums.MessageType.MBAlarm:
            //                case Enums.MessageType.ServiceRequired:
            //                    if (dsVehicleSensors != null && dsVehicleSensors.Tables.Count > 0)
            //                        ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, dsVehicleSensors.Tables[0]);
            //                    else
            //                        ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, null);
            //                    break;
            //                case Enums.MessageType.Speeding:      // was Speeding
            //                    {
            //                        string duration = Util.PairFindValue(Const.keySpeedDuration, customProp);
            //                        if (duration != "")
            //                        {
            //                            try
            //                            {
            //                                ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
            //                            }
            //                            catch
            //                            {
            //                            }
            //                        }
            //                        break;
            //                    }
            //                case Enums.MessageType.TetheredState:
            //                    {
            //                        string duration = Util.PairFindValue(Const.keyTetheredState, customProp);
            //                        if (duration != "")
            //                        {
            //                            try
            //                            {
            //                                ittr["MsgDetails"] = duration.Equals(Const.valON) ? Resources.Const.ON : Resources.Const.OFF;
            //                            }
            //                            catch
            //                            {
            //                            }
            //                        }
            //                        break;
            //                    }
            //                case Enums.MessageType.GeoFence:
            //                    //fenceNo = Util.PairFindValue(Const.keyFenceNo,customProp);
            //                    //if(fenceNo == null || fenceNo == "")
            //                    //	fenceNo = "Unknown";

            //                    fenceDirection = Util.PairFindValue(Const.keyFenceDir, customProp);
            //                    if (fenceDirection == null || fenceDirection == "")
            //                        fenceDirection = Resources.Const.fenceDirection_unknown;
            //                    else if (Const.valFenceDirIn == fenceDirection)
            //                        fenceDirection = Resources.Const.fenceDirection_inbroken;
            //                    else if (Const.valFenceDirOut == fenceDirection)
            //                        fenceDirection = Resources.Const.fenceDirection_outbroken;

            //                    ittr["MsgDetails"] = fenceDirection;
            //                    break;
            //                case Enums.MessageType.GeoZone:
            //                    if (vehicleAssignment == null)
            //                    {
            //                        vehicleAssignment = new DB.VehicleAssignment(sqlExec);
            //                        vehicleId = Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
            //                    }
            //                    if (vehicleGeozone == null)
            //                    {
            //                        vehicleGeozone = new DB.VehicleGeozone(sqlExec);
            //                        dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
            //                    }

            //                    if (dsGeozonesInfo != null && dsGeozonesInfo.Tables.Count > 0)
            //                        ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, dsGeozonesInfo.Tables[0]);
            //                    else
            //                        ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, null);
            //                    break;
            //                case Enums.MessageType.IPUpdate:
            //                    ittr["MsgDetails"] = ittr["CommInfo1"].ToString() + ":" + ittr["CommInfo2"].ToString().TrimEnd();
            //                    ittr["StreetAddress"] = Const.blankValue;
            //                    ittr["Speed"] = Const.blankValue;
            //                    break;
            //                case Enums.MessageType.Idling:
            //                case Enums.MessageType.ExtendedIdling:
            //                case Enums.MessageType.HarshBraking:
            //                case Enums.MessageType.ExtremeBraking:
            //                case Enums.MessageType.HarshAcceleration:
            //                case Enums.MessageType.ExtremeAcceleration:
            //                case Enums.MessageType.SeatBelt:
            //                    string speedDuration = "";
            //                    if (Enums.MessageType.Idling == msgType || Enums.MessageType.ExtendedIdling == msgType)
            //                        speedDuration = Util.PairFindValue(Const.keyIdleDuration, customProp);
            //                    else
            //                        speedDuration = Util.PairFindValue(Const.keyDuration, customProp);
            //                    if (speedDuration != "")
            //                    {
            //                        try
            //                        {
            //                            ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(speedDuration) * TimeSpan.TicksPerSecond).ToString();
            //                        }
            //                        catch
            //                        {
            //                        }
            //                    }
            //                    break;
            //                case Enums.MessageType.GPSAntenna:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyGPSAntenna, customProp);
            //                    break;

            //                case Enums.MessageType.MDTResponse:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyAnswer, customProp);
            //                    break;
            //                case Enums.MessageType.MDTSpecialMessage:
            //                    ittr["MsgDetails"] = customProp;
            //                    break;
            //                case Enums.MessageType.MDTTextMessage:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
            //                    break;
            //                case Enums.MessageType.Status:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.keyPower, customProp);
            //                    break;
            //                case Enums.MessageType.RFIDCode:
            //                    ittr["MsgDetails"] = Util.PairFindValue(Const.extFacilityCode, customProp) + "-" +
            //                                          Util.PairFindValue(Const.extIDNumber, customProp);
            //                    break;
            //                case Enums.MessageType.UpdateTelusPRLDone:
            //                    ittr["MsgDetails"] = "PRL=" + Util.PairFindValue(Const.keyPRLNumber, customProp);
            //                    break;
            //                default:// do nothing
            //                    ittr["MsgDetails"] = " ";
            //                    break;
            //            }
            //            // in case of invalid GPS for current message set defaults
            //            if (Convert.ToInt16(ittr["ValidGps"]) == 1)
            //            {
            //                ittr["Latitude"] = 0;
            //                ittr["Longitude"] = 0;
            //                ittr["Speed"] = Const.blankValue;
            //                ittr["Heading"] = Const.blankValue;
            //            }

            //            // add message direction to message type
            //            ittr["BoxMsgInTypeName"] = Const.MsgInDirectionSign + " " + ittr["BoxMsgInTypeName"];
            //        }
            //    }
            //}
            return dsResult;
        }
        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves off hours information from history by box Id
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
        /// <param name="tblLandmarks"></param>
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetBoxOffHoursInfo_NewTZ(int userId,
                                   bool isSuperUser,
                                   int boxId,
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
            DataSet dsResult = null;
            try
            {
                //Prepares SQL statement
                string sql = " DECLARE @ResolveLandmark int DECLARE @Timezone float DECLARE @Unit real DECLARE @DayLightSaving int SELECT @Timezone=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZoneNew + " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
                " IF @Timezone IS NULL SET @Timezone=0 IF @Unit IS NULL SET @Unit=1 SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving + " IF @DayLightSaving IS NULL SET @DayLightSaving=0 SET @Timezone= @Timezone + @DayLightSaving" +
                     " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +
                     " DECLARE @FromDate datetime DECLARE @ToDate datetime DECLARE @DayFromH smallint DECLARE @DayFromM smallint DECLARE @DayToH smallint DECLARE @DayToM smallint DECLARE @WeekendFromH smallint DECLARE @WeekendFromM smallint DECLARE @WeekendToH smallint DECLARE @WeekendToM smallint" +
                " SET @FromDate='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "' SET @ToDate='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                   " SET @DayFromH=" + dayFromHour + " SET @DayFromM=" + dayFromMin + " SET @DayToH=" + dayToHour + " SET @DayToM=" + dayToMin + " SET @WeekendFromH=" + weekendFromHour + " SET @WeekendFromM=" + weekendFromMin + " SET @WeekendToH=" + weekendToHour + " SET @WeekendToM=" + weekendToMin;
                string sqlWhere = "AND (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speed +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speeding +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoFence +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MBAlarm +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobArm +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobDisarm +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobPanic +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Alarm +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTResponse +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTSpecialMessage +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTTextMessage +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxSetup +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxStatus +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.StoredPosition +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Idling +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtendedIdling +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZone +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntenna +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneIDs +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSetup +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VCROffDelay +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobStatus +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TARMode +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ControllerStatus +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ServiceRequired +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.LowBattery +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxReset +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputOneOn +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputTwoOn +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Odometer +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaShort +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOpen +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BreakIn +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TowAwayAlert +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Collision +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PanicButton +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SleepModeFailure +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MainBatteryDisconnected +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TetheredState;
                // string sql = " DECLARE @ResolveLandmark int DECLARE @Timezone int DECLARE @Unit real DECLARE @DayLightSaving int SELECT @Timezone=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZone + " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
                //" IF @Timezone IS NULL SET @Timezone=0 IF @Unit IS NULL SET @Unit=1 SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving + " IF @DayLightSaving IS NULL SET @DayLightSaving=0 SET @Timezone= @Timezone + @DayLightSaving" +
                //     " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +
                //     " DECLARE @FromDate datetime DECLARE @ToDate datetime DECLARE @DayFromH smallint DECLARE @DayFromM smallint DECLARE @DayToH smallint DECLARE @DayToM smallint DECLARE @WeekendFromH smallint DECLARE @WeekendFromM smallint DECLARE @WeekendToH smallint DECLARE @WeekendToM smallint" +
                //" SET @FromDate='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "' SET @ToDate='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                //   " SET @DayFromH=" + dayFromHour + " SET @DayFromM=" + dayFromMin + " SET @DayToH=" + dayToHour + " SET @DayToM=" + dayToMin + " SET @WeekendFromH=" + weekendFromHour + " SET @WeekendFromM=" + weekendFromMin + " SET @WeekendToH=" + weekendToHour + " SET @WeekendToM=" + weekendToMin;
                // string sqlWhere = "AND (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speed +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speeding +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoFence +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MBAlarm +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobArm +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobDisarm +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobPanic +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Alarm +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTResponse +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTSpecialMessage +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTTextMessage +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxSetup +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxStatus +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.StoredPosition +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Idling +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtendedIdling +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZone +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntenna +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneIDs +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSetup +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VCROffDelay +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobStatus +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TARMode +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ControllerStatus +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ServiceRequired +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.LowBattery +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxReset +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputOneOn +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputTwoOn +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Odometer +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaShort +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOpen +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BreakIn +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TowAwayAlert +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Collision +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PanicButton +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SleepModeFailure +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MainBatteryDisconnected +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
                //       " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TetheredState;


                if (isSuperUser)
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdate;


                if (includeSensor)
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Sensor +
                                " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SensorExtended;

                if (includeCoordinate)//"Scheduled Update" is displayed name
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate;
                    else
                        sqlWhere += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate + " AND ValidGps=0)";
                }
                if (includePositionUpdate)
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate;
                    else
                        sqlWhere += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate + " AND ValidGps=0)";
                }
                sqlWhere += ")";

                sql += " SELECT vlfMsgInHst.BoxId," +
                   "CASE WHEN DateTimeReceived IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),DateTimeReceived) END AS DateTimeReceived," +
                   "DclId,vlfBoxMsgInType.BoxMsgInTypeId," +
                   "CASE WHEN BoxMsgInTypeName IS NOT NULL then RTRIM(BoxMsgInTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "convert(nvarchar,ValidGps) AS ValidGps," +
                   "convert(nvarchar,Latitude) AS Latitude," +
                   "convert(nvarchar,Longitude) AS Longitude," +
                   "convert(nvarchar,Heading) AS Heading," +
                   "convert(nvarchar,SensorMask) AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "convert(nvarchar,BlobDataSize) AS BlobDataSize,SequenceNum," +
                        "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + Const.addressNA + "') AS StreetAddress," +

                   "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),OriginDateTime) END AS OriginDateTime," +
                   "CASE WHEN vlfMsgInHst.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHst.Speed * @Unit,1)) END AS Speed," +
                   "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed," +
                   "0 AS MsgDirection," + // in
                   "' ' AS Acknowledged,' ' AS Scheduled,0 as InRange" +
                   " into #tmpIn FROM vlfMsgInHst with (nolock) INNER JOIN vlfBoxMsgInType ON vlfMsgInHst.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId INNER JOIN vlfBoxProtocolType ON vlfMsgInHst.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " WHERE vlfMsgInHst.BoxId =" + boxId + " AND OriginDateTime>=@FromDate AND OriginDateTime<=@ToDate " + sqlWhere;

                sqlWhere = " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.Ack +
                   " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.MDTAck;
                if (!isSuperUser)
                {
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.WakeUpByte;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.ThirdPartyPacket;
                }
                sql += " SELECT vlfMsgOutHst.BoxId," +
                   "DATEADD(minute,(@Timezone * 60),DateTime) AS DateTimeReceived,DclId," +
                   "vlfBoxCmdOutType.BoxCmdOutTypeId AS BoxMsgInTypeId," +
                   "CASE WHEN BoxCmdOutTypeName IS NOT NULL then RTRIM(BoxCmdOutTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "'" + Const.blankValue + "' AS ValidGps," +
                   "'" + Const.blankValue + "' AS Latitude," +
                   "'" + Const.blankValue + "' AS Longitude," +
                   "'" + Const.blankValue + "' AS Heading," +
                   "'" + Const.blankValue + "' AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "'" + Const.blankValue + "' As BlobDataSize," +
                   "SequenceNum," +
                   "'" + Const.blankValue + "' AS StreetAddress," +
                   "DATEADD(minute,(@Timezone * 60),DateTime) AS OriginDateTime," +
                   "'" + Const.blankValue + "' AS Speed," +
                   "'" + Const.blankValue + "' AS BoxArmed," +
                   "1 AS MsgDirection," + // out
                   "CASE WHEN Acknowledged IS NOT NULL then CASE WHEN RTRIM(Acknowledged) IS NOT NULL then RTRIM(Acknowledged) END ELSE '" + Const.cmdNotAck + "' END AS Acknowledged," +
                   "CASE WHEN Scheduled IS NOT NULL then CASE WHEN Scheduled=0 then 'No' END ELSE 'Yes' END AS Scheduled,0 as InRange" +
                   " into #tmpOut FROM vlfMsgOutHst INNER JOIN vlfBoxCmdOutType ON vlfMsgOutHst.BoxCmdOutTypeId = vlfBoxCmdOutType.BoxCmdOutTypeId INNER JOIN vlfBoxProtocolType ON vlfMsgOutHst.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " WHERE vlfMsgOutHst.BoxId=" + boxId + " AND DateTime>=@FromDate AND DateTime<=@ToDate " + sqlWhere;

                sql += " DECLARE @OriginDateTime datetime DECLARE @BoxId int" +
                   " DECLARE tmp_Cursor CURSOR FOR" +
                   " SELECT OriginDateTime,BoxId FROM #tmpIn" +
                   " OPEN tmp_Cursor" +
                   " FETCH NEXT FROM tmp_Cursor into @OriginDateTime,@BoxId" +
                   " WHILE @@FETCH_STATUS = 0" +
                   " BEGIN" +
                      " IF DATENAME(weekday,@OriginDateTime) in ('Saturday','Sunday')" +
                         " BEGIN" +
                         " IF DATENAME(hh,@OriginDateTime) < @WeekendFromH  or DATENAME(hh,@OriginDateTime)> @WeekendToH" +
                            " UPDATE #tmpIn SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                         " ELSE" +
                            " IF DATENAME(hh,@OriginDateTime) = @WeekendFromH AND DATENAME(mi,@OriginDateTime) < @WeekendFromM" +
                               " UPDATE #tmpIn SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                            " ELSE IF DATENAME(hh,@OriginDateTime) = @WeekendToH AND DATENAME(mi,@OriginDateTime) > @WeekendToM" +
                               " UPDATE #tmpIn SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                         " END" +
                      " ELSE" +
                      " BEGIN" +
                         " IF DATENAME(hh,@OriginDateTime) < @DayFromH  or DATENAME(hh,@OriginDateTime)> @DayToH" +
                            " UPDATE #tmpIn SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                         " ELSE " +
                            " IF DATENAME(hh,@OriginDateTime) = @DayFromH AND DATENAME(mi,@OriginDateTime) < @DayFromM" +
                               " UPDATE #tmpIn SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                            " ELSE IF DATENAME(hh,@OriginDateTime) = @DayToH AND DATENAME(mi,@OriginDateTime) > @DayToM" +
                               " UPDATE #tmpIn SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                      " END" +
                      " FETCH NEXT FROM tmp_Cursor into @OriginDateTime,@BoxId" +
                   " END" +
                   " CLOSE tmp_Cursor" +
                   " DEALLOCATE tmp_Cursor" +

                   " DECLARE tmp_Cursor CURSOR FOR" +
                   " SELECT OriginDateTime,BoxId FROM #tmpOut" +
                   " OPEN tmp_Cursor" +
                   " FETCH NEXT FROM tmp_Cursor into @OriginDateTime,@BoxId" +
                   " WHILE @@FETCH_STATUS = 0" +
                   " BEGIN" +
                      " IF DATENAME(weekday,@OriginDateTime) in ('Saturday','Sunday')" +
                      " BEGIN" +
                         " IF DATENAME(hh,@OriginDateTime) < @WeekendFromH  or DATENAME(hh,@OriginDateTime)> @WeekendToH" +
                            " UPDATE #tmpOut SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                         " ELSE" +
                            " IF DATENAME(hh,@OriginDateTime) = @WeekendFromH AND DATENAME(mi,@OriginDateTime) < @WeekendFromM" +
                               " UPDATE #tmpOut SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                            " ELSE IF DATENAME(hh,@OriginDateTime) = @WeekendToH AND DATENAME(mi,@OriginDateTime) > @WeekendToM" +
                               " UPDATE #tmpOut SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                      " END" +
                      " ELSE" +
                      " BEGIN" +
                         " IF DATENAME(hh,@OriginDateTime) < @DayFromH  or DATENAME(hh,@OriginDateTime)> @DayToH" +
                            " UPDATE #tmpOut SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                         " ELSE" +
                            " IF DATENAME(hh,@OriginDateTime) = @DayFromH AND DATENAME(mi,@OriginDateTime) < @DayFromM" +
                               " UPDATE #tmpOut SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                            " ELSE IF DATENAME(hh,@OriginDateTime) = @DayToH AND DATENAME(mi,@OriginDateTime) > @DayToM" +
                               " UPDATE #tmpOut SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                      " END" +
                      " FETCH NEXT FROM tmp_Cursor into @OriginDateTime,@BoxId" +
                   " END" +
                   " CLOSE tmp_Cursor" +
                   " DEALLOCATE tmp_Cursor" +

                   " select * from #tmpIn where InRange=1 UNION select * from #tmpOut where InRange=1 order by OriginDateTime DESC, DateTimeReceived DESC" +
                   " drop table #tmpIn drop table #tmpOut";

                // 2. Executes SQL statement
                dsResult = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                // 1. Add additional column to result dataset
                dsResult.Tables[0].Columns.Add("MsgDetails", typeof(string));
                //int boxId = Const.unassignedIntValue;
                DataSet dsVehicleSensors = null;
                DataSet dsVehicleOutputs = null;
                string fenceDirection = "";

                // 2. Get user-defined sensors info (name,action)
                //[SensorId][SensorName][SensorAction][AlarmLevel]
                DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
                dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);
                // 3. Get user-defined outputs info 
                //[OutputId][OutputName][OutputAction]
                DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
                dsVehicleOutputs = outputs.GetOutputsInfoByBoxId(boxId, userId);

                // 4. Resolve vehicle geozones
                DB.VehicleAssignment vehicleAssignment = null;
                DB.VehicleGeozone vehicleGeozone = null;
                DataSet dsGeozonesInfo = null;
                Int64 vehicleId = Const.unassignedIntValue;

                foreach (DataRow ittr in dsResult.Tables[0].Rows)
                {

                    string customProp = ittr["CustomProp"].ToString().TrimEnd();

                    if (Convert.ToInt16(ittr["MsgDirection"]) == Const.MsgOutDirectionValue)// process outgoing messages
                    {
                        switch ((Enums.CommandType)Convert.ToInt16(ittr["BoxMsgInTypeId"]))
                        {
                            case Enums.CommandType.Output:
                                ittr["MsgDetails"] = DB.Output.GetOutputDescription(customProp, dsVehicleOutputs.Tables[0]);
                                break;

                            case Enums.CommandType.MDTTextMessage:
                                ittr["BoxMsgInTypeName"] = "Text Message";
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                ittr["MsgDetails"] += Convert.ToChar(13).ToString();
                                ittr["MsgDetails"] += Convert.ToChar(10).ToString();
                                ittr["MsgDetails"] += " Answers:" + Util.PairFindValue(Const.keyAnswer, customProp).Replace("~", ",");
                                break;

                            default:
                                ittr["MsgDetails"] = ittr["BoxMsgInTypeName"];
                                ittr["BoxMsgInTypeName"] = "Command";
                                break;
                        }
                        ittr["StreetAddress"] = Const.blankValue; // not applicable

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgOutDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                    else // process incomming messages
                    {
                        Enums.MessageType msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);
                        switch (msgType)
                        {
                            case Enums.MessageType.Sensor:
                            case Enums.MessageType.SensorExtended:
                            case Enums.MessageType.Alarm:
                            case Enums.MessageType.MBAlarm:
                                if (dsVehicleSensors != null && dsVehicleSensors.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, dsVehicleSensors.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, null);
                                break;
                            case Enums.MessageType.Speeding:      // was Speeding
                                {
                                    string duration = Util.PairFindValue(Const.keySpeedDuration, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.TetheredState:
                                {
                                    string duration = Util.PairFindValue(Const.keyTetheredState, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = duration.Equals(Const.valON) ? Resources.Const.ON : Resources.Const.OFF;
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.GeoFence:
                                //fenceNo = Util.PairFindValue(Const.keyFenceNo,ittr["CustomProp"].ToString().TrimEnd());
                                //if(fenceNo == null || fenceNo == "")
                                //	fenceNo = "Unknown";

                                fenceDirection = Util.PairFindValue(Const.keyFenceDir, customProp);
                                if (fenceDirection == null || fenceDirection == "")
                                    fenceDirection = "unknown direction";
                                else if (Const.valFenceDirIn == fenceDirection)
                                    fenceDirection = "in broken";
                                else if (Const.valFenceDirOut == fenceDirection)
                                    fenceDirection = "out broken";

                                ittr["MsgDetails"] = fenceDirection;
                                break;
                            case Enums.MessageType.GeoZone:
                                if (vehicleAssignment == null)
                                {
                                    vehicleAssignment = new DB.VehicleAssignment(sqlExec);
                                    vehicleId = Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
                                }
                                if (vehicleGeozone == null)
                                {
                                    vehicleGeozone = new DB.VehicleGeozone(sqlExec);
                                    dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
                                }

                                if (dsGeozonesInfo != null && dsGeozonesInfo.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, dsGeozonesInfo.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, null);
                                break;
                            case Enums.MessageType.IPUpdate:
                                ittr["MsgDetails"] = ittr["CommInfo1"].ToString() + ":" + ittr["CommInfo2"].ToString().TrimEnd();
                                break;
                            case Enums.MessageType.Idling:
                            case Enums.MessageType.ExtendedIdling:
                            case Enums.MessageType.HarshBraking:
                            case Enums.MessageType.ExtremeBraking:
                            case Enums.MessageType.HarshAcceleration:
                            case Enums.MessageType.ExtremeAcceleration:
                            case Enums.MessageType.SeatBelt:
                                string speedDuration = "";
                                if (Enums.MessageType.Idling == msgType || Enums.MessageType.ExtendedIdling == msgType)
                                    speedDuration = Util.PairFindValue(Const.keyIdleDuration, customProp);
                                else
                                    speedDuration = Util.PairFindValue(Const.keyDuration, customProp);
                                if (speedDuration != "")
                                {
                                    try
                                    {
                                        ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(speedDuration) * TimeSpan.TicksPerSecond).ToString();
                                    }
                                    catch
                                    {
                                    }
                                }
                                break;
                            case Enums.MessageType.GPSAntenna:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyGPSAntenna, customProp);
                                break;

                            case Enums.MessageType.MDTResponse:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyAnswer, customProp);
                                break;
                            case Enums.MessageType.MDTSpecialMessage:
                                ittr["MsgDetails"] = customProp;
                                break;
                            case Enums.MessageType.MDTTextMessage:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                break;
                            case Enums.MessageType.Status:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyPower, customProp);
                                break;
                            case Enums.MessageType.ServiceRequired:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyBadSensorNum, customProp);
                                break;
                            default:// do nothing
                                ittr["MsgDetails"] = " ";
                                break;
                        }
                        // in case of invalid GPS for current message set defaults
                        if (Convert.ToInt16(ittr["ValidGps"]) == 1)
                        {
                            ittr["Latitude"] = 0;
                            ittr["Longitude"] = 0;
                            ittr["Speed"] = Const.blankValue;
                            ittr["Heading"] = Const.blankValue;
                        }

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgInDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                }
            }
            return dsResult;
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves off hours information from history by box Id
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
        /// <param name="tblLandmarks"></param>
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetBoxOffHoursInfo(int userId,
                                   bool isSuperUser,
                                   int boxId,
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
            DataSet dsResult = null;
            try
            {
                string sql = " DECLARE @ResolveLandmark int DECLARE @Timezone int DECLARE @Unit real DECLARE @DayLightSaving int SELECT @Timezone=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZone + " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
               " IF @Timezone IS NULL SET @Timezone=0 IF @Unit IS NULL SET @Unit=1 SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving + " IF @DayLightSaving IS NULL SET @DayLightSaving=0 SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +
                    " DECLARE @FromDate datetime DECLARE @ToDate datetime DECLARE @DayFromH smallint DECLARE @DayFromM smallint DECLARE @DayToH smallint DECLARE @DayToM smallint DECLARE @WeekendFromH smallint DECLARE @WeekendFromM smallint DECLARE @WeekendToH smallint DECLARE @WeekendToM smallint" +
               " SET @FromDate='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "' SET @ToDate='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                  " SET @DayFromH=" + dayFromHour + " SET @DayFromM=" + dayFromMin + " SET @DayToH=" + dayToHour + " SET @DayToM=" + dayToMin + " SET @WeekendFromH=" + weekendFromHour + " SET @WeekendFromM=" + weekendFromMin + " SET @WeekendToH=" + weekendToHour + " SET @WeekendToM=" + weekendToMin;
                string sqlWhere = "AND (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speed +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Speeding +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoFence +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MBAlarm +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobArm +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobDisarm +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobPanic +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Alarm +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTResponse +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTSpecialMessage +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MDTTextMessage +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxSetup +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxStatus +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.StoredPosition +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Idling +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtendedIdling +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZone +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntenna +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneIDs +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GeoZoneSetup +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.VCROffDelay +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.KeyFobStatus +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TARMode +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ControllerStatus +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ServiceRequired +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.LowBattery +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BoxReset +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputOneOn +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.AuxInputTwoOn +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Odometer +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaShort +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.GPSAntennaOpen +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BreakIn +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TowAwayAlert +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Collision +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PanicButton +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SleepModeFailure +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.MainBatteryDisconnected +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.BadSensor +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Status +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshBraking +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeBraking +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.HarshAcceleration +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.ExtremeAcceleration +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SeatBelt +
                      " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.TetheredState;


                if (isSuperUser)
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.IPUpdate;


                if (includeSensor)
                    sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Sensor +
                                " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.SensorExtended;

                if (includeCoordinate)//"Scheduled Update" is displayed name
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate;
                    else
                        sqlWhere += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.Coordinate + " AND ValidGps=0)";
                }
                if (includePositionUpdate)
                {
                    if (includeInvalidGps)
                        sqlWhere += " OR vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate;
                    else
                        sqlWhere += " OR (vlfMsgInHst.BoxMsgInTypeId=" + (short)Enums.MessageType.PositionUpdate + " AND ValidGps=0)";
                }
                sqlWhere += ")";

                sql += " SELECT vlfMsgInHst.BoxId," +
                   "CASE WHEN DateTimeReceived IS NULL then '' ELSE DATEADD(hour,@Timezone,DateTimeReceived) END AS DateTimeReceived," +
                   "DclId,vlfBoxMsgInType.BoxMsgInTypeId," +
                   "CASE WHEN BoxMsgInTypeName IS NOT NULL then RTRIM(BoxMsgInTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "convert(nvarchar,ValidGps) AS ValidGps," +
                   "convert(nvarchar,Latitude) AS Latitude," +
                   "convert(nvarchar,Longitude) AS Longitude," +
                   "convert(nvarchar,Heading) AS Heading," +
                   "convert(nvarchar,SensorMask) AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "convert(nvarchar,BlobDataSize) AS BlobDataSize,SequenceNum," +
                        "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + Const.addressNA + "') AS StreetAddress," +

                   "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,OriginDateTime) END AS OriginDateTime," +
                   "CASE WHEN vlfMsgInHst.Speed IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfMsgInHst.Speed * @Unit,1)) END AS Speed," +
                   "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed," +
                   "0 AS MsgDirection," + // in
                   "' ' AS Acknowledged,' ' AS Scheduled,0 as InRange" +
                   " into #tmpIn FROM vlfMsgInHst with (nolock) INNER JOIN vlfBoxMsgInType ON vlfMsgInHst.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId INNER JOIN vlfBoxProtocolType ON vlfMsgInHst.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " WHERE vlfMsgInHst.BoxId =" + boxId + " AND OriginDateTime>=@FromDate AND OriginDateTime<=@ToDate " + sqlWhere;

                sqlWhere = " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.Ack +
                   " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.MDTAck;
                if (!isSuperUser)
                {
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.WakeUpByte;
                    sqlWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId<>" + (short)Enums.CommandType.ThirdPartyPacket;
                }
                sql += " SELECT vlfMsgOutHst.BoxId," +
                   "DATEADD(hour,@Timezone,DateTime) AS DateTimeReceived,DclId," +
                   "vlfBoxCmdOutType.BoxCmdOutTypeId AS BoxMsgInTypeId," +
                   "CASE WHEN BoxCmdOutTypeName IS NOT NULL then RTRIM(BoxCmdOutTypeName) END AS BoxMsgInTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId," +
                   "CASE WHEN BoxProtocolTypeName IS NOT NULL then RTRIM(BoxProtocolTypeName) END AS BoxProtocolTypeName," +
                   "CASE WHEN CommInfo1 IS NOT NULL then RTRIM(CommInfo1) END AS CommInfo1," +
                   "CASE WHEN CommInfo2 IS NOT NULL then RTRIM(CommInfo2) END AS CommInfo2," +
                   "'" + Const.blankValue + "' AS ValidGps," +
                   "'" + Const.blankValue + "' AS Latitude," +
                   "'" + Const.blankValue + "' AS Longitude," +
                   "'" + Const.blankValue + "' AS Heading," +
                   "'" + Const.blankValue + "' AS SensorMask," +
                   "CASE WHEN CustomProp IS NOT NULL then RTRIM(CustomProp) END AS CustomProp," +
                   "'" + Const.blankValue + "' As BlobDataSize," +
                   "SequenceNum," +
                   "'" + Const.blankValue + "' AS StreetAddress," +
                   "DATEADD(hour,@Timezone,DateTime) AS OriginDateTime," +
                   "'" + Const.blankValue + "' AS Speed," +
                   "'" + Const.blankValue + "' AS BoxArmed," +
                   "1 AS MsgDirection," + // out
                   "CASE WHEN Acknowledged IS NOT NULL then CASE WHEN RTRIM(Acknowledged) IS NOT NULL then RTRIM(Acknowledged) END ELSE '" + Const.cmdNotAck + "' END AS Acknowledged," +
                   "CASE WHEN Scheduled IS NOT NULL then CASE WHEN Scheduled=0 then 'No' END ELSE 'Yes' END AS Scheduled,0 as InRange" +
                   " into #tmpOut FROM vlfMsgOutHst INNER JOIN vlfBoxCmdOutType ON vlfMsgOutHst.BoxCmdOutTypeId = vlfBoxCmdOutType.BoxCmdOutTypeId INNER JOIN vlfBoxProtocolType ON vlfMsgOutHst.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                   " WHERE vlfMsgOutHst.BoxId=" + boxId + " AND DateTime>=@FromDate AND DateTime<=@ToDate " + sqlWhere;

                sql += " DECLARE @OriginDateTime datetime DECLARE @BoxId int" +
                   " DECLARE tmp_Cursor CURSOR FOR" +
                   " SELECT OriginDateTime,BoxId FROM #tmpIn" +
                   " OPEN tmp_Cursor" +
                   " FETCH NEXT FROM tmp_Cursor into @OriginDateTime,@BoxId" +
                   " WHILE @@FETCH_STATUS = 0" +
                   " BEGIN" +
                      " IF DATENAME(weekday,@OriginDateTime) in ('Saturday','Sunday')" +
                         " BEGIN" +
                         " IF DATENAME(hh,@OriginDateTime) < @WeekendFromH  or DATENAME(hh,@OriginDateTime)> @WeekendToH" +
                            " UPDATE #tmpIn SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                         " ELSE" +
                            " IF DATENAME(hh,@OriginDateTime) = @WeekendFromH AND DATENAME(mi,@OriginDateTime) < @WeekendFromM" +
                               " UPDATE #tmpIn SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                            " ELSE IF DATENAME(hh,@OriginDateTime) = @WeekendToH AND DATENAME(mi,@OriginDateTime) > @WeekendToM" +
                               " UPDATE #tmpIn SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                         " END" +
                      " ELSE" +
                      " BEGIN" +
                         " IF DATENAME(hh,@OriginDateTime) < @DayFromH  or DATENAME(hh,@OriginDateTime)> @DayToH" +
                            " UPDATE #tmpIn SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                         " ELSE " +
                            " IF DATENAME(hh,@OriginDateTime) = @DayFromH AND DATENAME(mi,@OriginDateTime) < @DayFromM" +
                               " UPDATE #tmpIn SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                            " ELSE IF DATENAME(hh,@OriginDateTime) = @DayToH AND DATENAME(mi,@OriginDateTime) > @DayToM" +
                               " UPDATE #tmpIn SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                      " END" +
                      " FETCH NEXT FROM tmp_Cursor into @OriginDateTime,@BoxId" +
                   " END" +
                   " CLOSE tmp_Cursor" +
                   " DEALLOCATE tmp_Cursor" +

                   " DECLARE tmp_Cursor CURSOR FOR" +
                   " SELECT OriginDateTime,BoxId FROM #tmpOut" +
                   " OPEN tmp_Cursor" +
                   " FETCH NEXT FROM tmp_Cursor into @OriginDateTime,@BoxId" +
                   " WHILE @@FETCH_STATUS = 0" +
                   " BEGIN" +
                      " IF DATENAME(weekday,@OriginDateTime) in ('Saturday','Sunday')" +
                      " BEGIN" +
                         " IF DATENAME(hh,@OriginDateTime) < @WeekendFromH  or DATENAME(hh,@OriginDateTime)> @WeekendToH" +
                            " UPDATE #tmpOut SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                         " ELSE" +
                            " IF DATENAME(hh,@OriginDateTime) = @WeekendFromH AND DATENAME(mi,@OriginDateTime) < @WeekendFromM" +
                               " UPDATE #tmpOut SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                            " ELSE IF DATENAME(hh,@OriginDateTime) = @WeekendToH AND DATENAME(mi,@OriginDateTime) > @WeekendToM" +
                               " UPDATE #tmpOut SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                      " END" +
                      " ELSE" +
                      " BEGIN" +
                         " IF DATENAME(hh,@OriginDateTime) < @DayFromH  or DATENAME(hh,@OriginDateTime)> @DayToH" +
                            " UPDATE #tmpOut SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                         " ELSE" +
                            " IF DATENAME(hh,@OriginDateTime) = @DayFromH AND DATENAME(mi,@OriginDateTime) < @DayFromM" +
                               " UPDATE #tmpOut SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                            " ELSE IF DATENAME(hh,@OriginDateTime) = @DayToH AND DATENAME(mi,@OriginDateTime) > @DayToM" +
                               " UPDATE #tmpOut SET InRange=1  WHERE OriginDateTime=@OriginDateTime AND BoxId=@BoxId" +
                      " END" +
                      " FETCH NEXT FROM tmp_Cursor into @OriginDateTime,@BoxId" +
                   " END" +
                   " CLOSE tmp_Cursor" +
                   " DEALLOCATE tmp_Cursor" +

                   " select * from #tmpIn where InRange=1 UNION select * from #tmpOut where InRange=1 order by OriginDateTime DESC, DateTimeReceived DESC" +
                   " drop table #tmpIn drop table #tmpOut";

                // 2. Executes SQL statement
                dsResult = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                // 1. Add additional column to result dataset
                dsResult.Tables[0].Columns.Add("MsgDetails", typeof(string));
                //int boxId = Const.unassignedIntValue;
                DataSet dsVehicleSensors = null;
                DataSet dsVehicleOutputs = null;
                string fenceDirection = "";

                // 2. Get user-defined sensors info (name,action)
                //[SensorId][SensorName][SensorAction][AlarmLevel]
                DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
                dsVehicleSensors = sensors.GetSensorsInfoByBoxId(boxId);
                // 3. Get user-defined outputs info 
                //[OutputId][OutputName][OutputAction]
                DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
                dsVehicleOutputs = outputs.GetOutputsInfoByBoxId(boxId, userId);

                // 4. Resolve vehicle geozones
                DB.VehicleAssignment vehicleAssignment = null;
                DB.VehicleGeozone vehicleGeozone = null;
                DataSet dsGeozonesInfo = null;
                Int64 vehicleId = Const.unassignedIntValue;

                foreach (DataRow ittr in dsResult.Tables[0].Rows)
                {

                    string customProp = ittr["CustomProp"].ToString().TrimEnd();

                    if (Convert.ToInt16(ittr["MsgDirection"]) == Const.MsgOutDirectionValue)// process outgoing messages
                    {
                        switch ((Enums.CommandType)Convert.ToInt16(ittr["BoxMsgInTypeId"]))
                        {
                            case Enums.CommandType.Output:
                                ittr["MsgDetails"] = DB.Output.GetOutputDescription(customProp, dsVehicleOutputs.Tables[0]);
                                break;

                            case Enums.CommandType.MDTTextMessage:
                                ittr["BoxMsgInTypeName"] = "Text Message";
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                ittr["MsgDetails"] += Convert.ToChar(13).ToString();
                                ittr["MsgDetails"] += Convert.ToChar(10).ToString();
                                ittr["MsgDetails"] += " Answers:" + Util.PairFindValue(Const.keyAnswer, customProp).Replace("~", ",");
                                break;

                            default:
                                ittr["MsgDetails"] = ittr["BoxMsgInTypeName"];
                                ittr["BoxMsgInTypeName"] = "Command";
                                break;
                        }
                        ittr["StreetAddress"] = Const.blankValue; // not applicable

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgOutDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                    else // process incomming messages
                    {
                        Enums.MessageType msgType = (Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]);
                        switch (msgType)
                        {
                            case Enums.MessageType.Sensor:
                            case Enums.MessageType.SensorExtended:
                            case Enums.MessageType.Alarm:
                            case Enums.MessageType.MBAlarm:
                                if (dsVehicleSensors != null && dsVehicleSensors.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, dsVehicleSensors.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.Sensor.GetSensorDescription(customProp, null);
                                break;
                            case Enums.MessageType.Speeding:      // was Speeding
                                {
                                    string duration = Util.PairFindValue(Const.keySpeedDuration, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(duration) * TimeSpan.TicksPerSecond).ToString();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.TetheredState:
                                {
                                    string duration = Util.PairFindValue(Const.keyTetheredState, customProp);
                                    if (duration != "")
                                    {
                                        try
                                        {
                                            ittr["MsgDetails"] = duration.Equals(Const.valON) ? Resources.Const.ON : Resources.Const.OFF;
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                }
                            case Enums.MessageType.GeoFence:
                                //fenceNo = Util.PairFindValue(Const.keyFenceNo,ittr["CustomProp"].ToString().TrimEnd());
                                //if(fenceNo == null || fenceNo == "")
                                //	fenceNo = "Unknown";

                                fenceDirection = Util.PairFindValue(Const.keyFenceDir, customProp);
                                if (fenceDirection == null || fenceDirection == "")
                                    fenceDirection = "unknown direction";
                                else if (Const.valFenceDirIn == fenceDirection)
                                    fenceDirection = "in broken";
                                else if (Const.valFenceDirOut == fenceDirection)
                                    fenceDirection = "out broken";

                                ittr["MsgDetails"] = fenceDirection;
                                break;
                            case Enums.MessageType.GeoZone:
                                if (vehicleAssignment == null)
                                {
                                    vehicleAssignment = new DB.VehicleAssignment(sqlExec);
                                    vehicleId = Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));
                                }
                                if (vehicleGeozone == null)
                                {
                                    vehicleGeozone = new DB.VehicleGeozone(sqlExec);
                                    dsGeozonesInfo = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
                                }

                                if (dsGeozonesInfo != null && dsGeozonesInfo.Tables.Count > 0)
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, dsGeozonesInfo.Tables[0]);
                                else
                                    ittr["MsgDetails"] = DB.VehicleGeozone.GetGeoZoneDescription(customProp, null);
                                break;
                            case Enums.MessageType.IPUpdate:
                                ittr["MsgDetails"] = ittr["CommInfo1"].ToString() + ":" + ittr["CommInfo2"].ToString().TrimEnd();
                                break;
                            case Enums.MessageType.Idling:
                            case Enums.MessageType.ExtendedIdling:
                            case Enums.MessageType.HarshBraking:
                            case Enums.MessageType.ExtremeBraking:
                            case Enums.MessageType.HarshAcceleration:
                            case Enums.MessageType.ExtremeAcceleration:
                            case Enums.MessageType.SeatBelt:
                                string speedDuration = "";
                                if (Enums.MessageType.Idling == msgType || Enums.MessageType.ExtendedIdling == msgType)
                                    speedDuration = Util.PairFindValue(Const.keyIdleDuration, customProp);
                                else
                                    speedDuration = Util.PairFindValue(Const.keyDuration, customProp);
                                if (speedDuration != "")
                                {
                                    try
                                    {
                                        ittr["MsgDetails"] = Resources.Const.Duration + new TimeSpan(Convert.ToInt32(speedDuration) * TimeSpan.TicksPerSecond).ToString();
                                    }
                                    catch
                                    {
                                    }
                                }
                                break;
                            case Enums.MessageType.GPSAntenna:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyGPSAntenna, customProp);
                                break;

                            case Enums.MessageType.MDTResponse:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyAnswer, customProp);
                                break;
                            case Enums.MessageType.MDTSpecialMessage:
                                ittr["MsgDetails"] = customProp;
                                break;
                            case Enums.MessageType.MDTTextMessage:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyMessage, customProp);
                                break;
                            case Enums.MessageType.Status:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyPower, customProp);
                                break;
                            case Enums.MessageType.ServiceRequired:
                                ittr["MsgDetails"] = Util.PairFindValue(Const.keyBadSensorNum, customProp);
                                break;
                            default:// do nothing
                                ittr["MsgDetails"] = " ";
                                break;
                        }
                        // in case of invalid GPS for current message set defaults
                        if (Convert.ToInt16(ittr["ValidGps"]) == 1)
                        {
                            ittr["Latitude"] = 0;
                            ittr["Longitude"] = 0;
                            ittr["Speed"] = Const.blankValue;
                            ittr["Heading"] = Const.blankValue;
                        }

                        // add message direction to message type
                        ittr["BoxMsgInTypeName"] = Const.MsgInDirectionSign + " " + ittr["BoxMsgInTypeName"];
                    }
                }
            }
            return dsResult;
        }

        /// <summary>
        /// Deletes all messages from the history related to the box
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <returns>rows affected</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteBoxAllMsgs(int boxId, string tableName, string where)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Prepares SQL statement
                string sql = "DELETE FROM " + tableName + " WHERE BoxId=" + boxId + where;
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to delete all box " + boxId + " messages.";
                Util.ProcessDbException(prefixMsg, objException);
            }

            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to delete all box " + boxId + " messages.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }
        /// <summary>
        /// Retrieves records with empty StreetAddress fields from the MsgIn history
        /// </summary>
        /// <param name="cmdTimeOut"></param>
        /// <returns>DataSet [BoxId],[OriginDateTime],[Latitude],[Longitude]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetEmptyStreetAddressFromHistory(int cmdTimeOut)
        {
            DataSet sqlDataSet = null;
            int currCmdTimeOut = sqlExec.CommandTimeout;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT TOP 500 BoxId,OriginDateTime,Latitude,Longitude FROM vlfMsgInHst with (nolock) WHERE BoxId > 0 AND StreetAddress IS NULL AND Latitude <> 0 AND Longitude <> 0";
                //             string sql = "SELECT TOP 5000 BoxId,OriginDateTime,Latitude,Longitude FROM vlfMsgInHst WHERE BoxId > 0 AND BoxId < 4000 and DateTimeReceived > '2006/08/01' and DateTimeReceived < '2006/08/05 12:00:00 PM' AND Latitude <> 0 AND Longitude <> 0 and StreetAddress like 'lat=%'";
                //             string sql = "SELECT TOP 500 BoxId,OriginDateTime,Latitude,Longitude FROM vlfMsgInHst WHERE BoxId > 0 and BoxId < 4000 AND OriginDateTime > '2006/08/01' OriginDateTime < '2006/08/05' AND Latitude <> 0 AND Longitude <> 0 order by boxid desc";
                //             string sql = "SELECT TOP 500 BoxId,OriginDateTime,Latitude,Longitude FROM vlfMsgInHst WHERE BoxId > 0 AND boxid < 3100 and StreetAddress IS NULL AND Latitude <> 0 AND Longitude <> 0 order by boxid desc";
                //             string sql = "select BoxId,OriginDateTime,Latitude,Longitude from vlfMsgInHst where DateTimeReceived  < '2006/08/4 10:40:00' and DateTimeReceived  > '2006/08/2' and boxid >= 3001 and boxid < 4000 order by boxid asc";
                sqlExec.CommandTimeout = cmdTimeOut;
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);

            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve records with empty StreetAddress.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve records with empty StreetAddress. " + objException.Message);
            }
            finally
            {
                sqlExec.CommandTimeout = currCmdTimeOut;
            }
            // 3. Return result
            return sqlDataSet;
        }
        /// <summary>
        /// Updates record with street address and/or nearestLandmark
        /// </summary>
        /// <remarks>
        /// BoxId and DateTimeReceived are index fields
        /// </remarks>
        /// <param name="boxId"></param>
        /// <param name="originDateTime"></param>
        /// <param name="streetAddress"></param>
        /// <param name="cmdTimeOut"></param>
        /// <param name="nearestLandmark"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateStreetAddressInHistory(int boxId, DateTime originDateTime, string streetAddress, int cmdTimeOut, string nearestLandmark)
        {
            int rowsAffected = 0;
            int currCmdTimeOut = sqlExec.CommandTimeout;
            string sql = "";
            try
            {
                // 1. Prepares SQL statement
                sql = "UPDATE vlfMsgInHst SET StreetAddress='" + streetAddress.Replace("'", "''") + "'";
                if (nearestLandmark != "")
                    sql += ",NearestLandmark='" + nearestLandmark.Replace("'", "''") + "'";
                sql += " WHERE BoxId=" + boxId;
                sql += " AND OriginDateTime='" + originDateTime.Month + "/" +
                   originDateTime.Day + "/" +
                   originDateTime.Year + " " +
                   originDateTime.Hour + ":" +
                   originDateTime.Minute + ":" +
                   originDateTime.Second + ":" +
                   originDateTime.Millisecond + "'";

                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                sqlExec.CommandTimeout = cmdTimeOut;
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Street Address Update: Unable to update record with Boxid " + boxId + " and OriginDateTime " + originDateTime + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Street Address Update: Unable to update record with Boxid " + boxId + " and OriginDateTime " + originDateTime + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            finally
            {
                sqlExec.CommandTimeout = currCmdTimeOut;
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Street Address Update: Unable to update record with Boxid " + boxId + " and OriginDateTime " + originDateTime + ".";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " Street address already exists.");
            }
        }

        /// <summary>
        /// Updates record with street address and/or nearestLandmark
        /// </summary>
        /// <remarks>
        /// BoxId and DateTimeReceived are index fields
        /// </remarks>
        /// <param name="boxId"></param>
        /// <param name="originDateTime"></param>
        /// <param name="streetAddress"></param>
        /// <param name="cmdTimeOut"></param>
        /// <param name="nearestLandmark"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateNearestLandmarkInHistory(int boxId, DateTime originDateTime, string nearestLandmark)
        {
            int rowsAffected = 0;
            int currCmdTimeOut = sqlExec.CommandTimeout;
            string sql = "";
            try
            {
                // 1. Prepares SQL statement
                sql = "UPDATE vlfMsgInHst SET NearestLandmark='" + nearestLandmark.Replace("'", "''") + "'";
                sql += " WHERE BoxId=" + boxId;
                sql += " AND OriginDateTime='" + originDateTime.Month + "/" +
                   originDateTime.Day + "/" +
                   originDateTime.Year + " " +
                   originDateTime.Hour + ":" +
                   originDateTime.Minute + ":" +
                   originDateTime.Second + ":" +
                   originDateTime.Millisecond + "'";

                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                sqlExec.CommandTimeout = 600;
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Street Address Update: Unable to update record with Boxid " + boxId + " and OriginDateTime " + originDateTime + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Street Address Update: Unable to update record with Boxid " + boxId + " and OriginDateTime " + originDateTime + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            finally
            {
                sqlExec.CommandTimeout = currCmdTimeOut;
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Street Address Update: Unable to update record with Boxid " + boxId + " and OriginDateTime " + originDateTime + ".";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " Street address already exists.");
            }
        }
        /// <summary>
        /// Retrieves records with empty Landmark fields from the MsgIn history
        /// </summary>
        /// <param name="cmdTimeOut"></param>
        /// <returns>DataSet [BoxId],[OriginDateTime],[Latitude],[Longitude]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetEmptyNearestLandmarkFromHistory()
        {
            DataSet sqlDataSet = null;
            int currCmdTimeOut = sqlExec.CommandTimeout;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT BoxId,OriginDateTime,Latitude,Longitude FROM vlfMsgInHst with (nolock) WHERE BoxId > 0 AND NearestLandmark IS NULL AND Latitude <> 0 AND Longitude <> 0";
                // 2. Executes SQL statement
                sqlExec.CommandTimeout = 600;
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);

            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve records with empty StreetAddress.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve records with empty StreetAddress. " + objException.Message);
            }
            finally
            {
                sqlExec.CommandTimeout = currCmdTimeOut;
            }
            // 3. Return result
            return sqlDataSet;
        }
        #endregion

        #region Protected Interfaces
        /// <summary>
        /// Add new Msg into MsgIn table.
        /// </summary>
        /// <param name="cMFIn"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AppendMsg(CMFIn cMFIn)
        {
            int rowsAffected = 0;
            /*
                        if (cMFIn.messageTypeID != (short)Enums.MessageType.MDTSpecialMessage)
                            ValidateProtocolTypeByMsgInType(Convert.ToInt16(cMFIn.protocolTypeID),
                                                    Convert.ToInt16(cMFIn.messageTypeID));
             */
            try
            {
                // Construct SQL for migration to the MsgIn data target. 
                string sql = "INSERT INTO " + tableName + "( " +
                   "DateTimeReceived" +
                   ",BoxId" +
                   ",DclId" +
                   ",BoxMsgInTypeId" +
                   ",BoxProtocolTypeId" +
                   ",OriginDateTime" +
                   ",CommInfo1" +
                   ",CommInfo2" +
                   ",ValidGps" +
                   ",Latitude" +
                   ",Longitude" +
                   ",Speed" +
                   ",Heading" +
                   ",SensorMask" +
                   ",CustomProp" +
                   ",BlobData" +
                   ",BlobDataSize" +
                   ",SequenceNum" +
                   ",IsArmed" +
                   ",Priority) VALUES ( @DateTimeReceived,@BoxId,@DclId,@BoxMsgInTypeId," +
                   "@BoxProtocolTypeId,@OriginDateTime,@CommInfo1,@CommInfo2,@ValidGps," +
                   "@Latitude,@Longitude,@Speed,@Heading,@SensorMask,@CustomProp," +
                   "@BlobData,@BlobDataSize,@SequenceNum,@IsArmed,@Priority ) ";

                // Set SQL command
                sqlExec.ClearCommandParameters();
                // Add parameters to SQL statement
                sqlExec.AddCommandParam("@DateTimeReceived", SqlDbType.BigInt, Convert.ToInt64(Convert.ToDateTime(cMFIn.receivedDateTime).Ticks));
                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, cMFIn.boxID);
                sqlExec.AddCommandParam("@DclId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.dclID));
                sqlExec.AddCommandParam("@BoxMsgInTypeId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.messageTypeID));
                sqlExec.AddCommandParam("@BoxProtocolTypeId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.protocolTypeID));
                sqlExec.AddCommandParam("@OriginDateTime", SqlDbType.DateTime, Convert.ToDateTime(cMFIn.originatedDateTime));
                sqlExec.AddCommandParam("@CommInfo1", SqlDbType.Char, cMFIn.commInfo1);
                if (cMFIn.commInfo2 == null)
                    sqlExec.AddCommandParam("@CommInfo2", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@CommInfo2", SqlDbType.Char, cMFIn.commInfo2);
                sqlExec.AddCommandParam("@ValidGps", SqlDbType.SmallInt, Convert.ToSByte(cMFIn.validGPS));

                sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, cMFIn.latitude);
                sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, cMFIn.longitude);
                sqlExec.AddCommandParam("@Speed", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.speed));
                sqlExec.AddCommandParam("@Heading", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.heading));
                sqlExec.AddCommandParam("@SensorMask", SqlDbType.BigInt, cMFIn.sensorMask);
                if (cMFIn.customProperties == null)
                    sqlExec.AddCommandParam("@CustomProp", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@CustomProp", SqlDbType.Char, cMFIn.customProperties);
                sqlExec.AddCommandParam("@BlobData", SqlDbType.Binary, cMFIn.blobData, Convert.ToInt16(cMFIn.blobSize));
                sqlExec.AddCommandParam("@BlobDataSize", SqlDbType.Int, Convert.ToInt16(cMFIn.blobSize));
                sqlExec.AddCommandParam("@SequenceNum", SqlDbType.Int, cMFIn.sequenceNum);
                sqlExec.AddCommandParam("@IsArmed", SqlDbType.TinyInt, cMFIn.isArmed);
                sqlExec.AddCommandParam("@Priority", SqlDbType.TinyInt, cMFIn.priority);
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                //string prefixMsg = "Unable to add new msg In with date '" + Convert.ToDateTime(cMFIn.receivedDateTime).ToString() + "'.";
                string prefixMsg = string.Format("Unable to add new msg to msgIn: recvTime={0}, origTime={1}, boxId={2}, dclId={3}, msgId={4}, protId={5} SqlException={6}",
                    cMFIn.receivedDateTime, cMFIn.originatedDateTime, cMFIn.boxID,
                    cMFIn.dclID, cMFIn.messageTypeID, cMFIn.protocolTypeID, objException.Message);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to add new msg to msgIn: recvTime={0}, origTime={1}, boxId={2}, dclId={3}, msgId={4}, protId={5} Exception={6}",
                   cMFIn.receivedDateTime, cMFIn.originatedDateTime, cMFIn.boxID,
                   cMFIn.dclID, cMFIn.messageTypeID, cMFIn.protocolTypeID, objException);
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to add new msg to msgIn: recvTime={0}, origTime={1}, boxId={2}, dclId={3}, msgId={4}, protId={5}", cMFIn.receivedDateTime, cMFIn.originatedDateTime, cMFIn.boxID, cMFIn.dclID, cMFIn.messageTypeID, cMFIn.protocolTypeID);
                throw new DASAppDataAlreadyExistsException(prefixMsg + " The message already exists.");
            }
        }

        /// <summary>
        /// Deletes exist message.
        /// </summary>
        /// <param name="dateTimeReceived"></param>
        /// <param name="boxId"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        protected int DeleteMsg(Int64 dateTimeReceived, int boxId)
        {
            string prefixMsg = "Unable to delete by box " + boxId + " and '" + new DateTime(dateTimeReceived) + "' received datetime";
            //Prepares SQL statement
            string sql = "DELETE FROM " + tableName +
                     " WHERE DateTimeReceived=" + dateTimeReceived +
                     " AND BoxId=" + boxId;
            return DeleteRowsBySql(sql, prefixMsg, "received datetime");
        }
        /// <summary>
        /// Prevents inconsistent insert of the "boxProtocolTypeId" field to the 
        /// "vlfMsgIn" table by checking valid dependency in the 
        /// "vlfBoxMsgInType,vlfBoxProtocolTypeMsgInType,vlfBoxProtocolType" tables.
        /// </summary>
        /// <param name="boxProtocolTypeId"></param>
        /// <param name="boxMsgInTypeId"></param>
        /// <exception cref="DASAppResultNotFoundException">Thrown if data does not exist.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        protected void ValidateProtocolTypeByMsgInType(short boxProtocolTypeId, short boxMsgInTypeId)
        {
            //Prepares SQL statement
            string sql = "SELECT COUNT(vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId) AS BoxProtocolTypeId" +
                     " FROM vlfBoxMsgInType INNER JOIN vlfBoxProtocolTypeMsgInType ON vlfBoxMsgInType.BoxMsgInTypeId = vlfBoxProtocolTypeMsgInType.BoxMsgInTypeId INNER JOIN vlfBoxProtocolType ON vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId" +
                     " WHERE (vlfBoxProtocolType.BoxProtocolTypeId = " + boxProtocolTypeId + ") AND (vlfBoxMsgInType.BoxMsgInTypeId = " + boxMsgInTypeId + ") GROUP BY vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId";

            int recordCount = 0;
            try
            {
                //Executes SQL statement
                recordCount = (int)sqlExec.SQLExecuteScalar(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Validation warning. Wrong protocol type="
                   + boxProtocolTypeId + " for message in type=" + boxMsgInTypeId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Validation warning. Wrong protocol type="
                   + boxProtocolTypeId + " for message in type=" + boxMsgInTypeId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            if (recordCount <= 0)
            {
                string prefixMsg = "Validation warning. Wrong protocol type="
                   + boxProtocolTypeId + " for message in type=" + boxMsgInTypeId + ". ";
                throw new DASAppResultNotFoundException(prefixMsg);
            }
        }
        /// <summary>
        /// Backup Msg to the MsgIn history table.
        /// Delete exist message.
        /// </summary>
        /// <param name="cMFIn"></param>
        /// <param name="currTableName"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AddToHistory(CMFIn cMFIn, string currTableName)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Util.BTrace(Util.INF0, ">> AddToHistory -> {0} {1} {2}", cMFIn.boxID,
                  cMFIn.originatedDateTime, cMFIn.messageTypeID);

            int rowsAffected = 0;
            try
            {
                string sql = "INSERT INTO " + currTableName + " ( " +
                   "BoxId" +
                   ",DateTimeReceived" +
                   ",DclId" +
                   ",BoxMsgInTypeId" +
                   ",BoxProtocolTypeId" +
                   ",OriginDateTime" +
                   ",CommInfo1" +
                   ",CommInfo2" +
                   ",ValidGps" +
                   ",Latitude" +
                   ",Longitude" +
                   ",Speed" +
                   ",Heading" +
                   ",SensorMask" +
                   ",CustomProp" +
                   ",BlobDataSize" +
                   ",SequenceNum" +
                   ",StreetAddress" +
                   ",IsArmed ) VALUES ( @BoxId,@DateTimeReceived,@DclId,@BoxMsgInTypeId,@BoxProtocolTypeId,@OriginDateTime,@CommInfo1,@CommInfo2,@ValidGps,@Latitude,@Longitude,@Speed,@Heading,@SensorMask,@CustomProp,@BlobDataSize,@SequenceNum,@StreetAddress,@isArmed) ";

                // Set SQL command
                sqlExec.ClearCommandParameters();
                // Add parameters to SQL statement
                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, cMFIn.boxID);
                sqlExec.AddCommandParam("@DateTimeReceived", SqlDbType.DateTime, Convert.ToDateTime(cMFIn.receivedDateTime));
                sqlExec.AddCommandParam("@DclId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.dclID));
                sqlExec.AddCommandParam("@BoxMsgInTypeId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.messageTypeID));
                sqlExec.AddCommandParam("@BoxProtocolTypeId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.protocolTypeID));
                sqlExec.AddCommandParam("@OriginDateTime", SqlDbType.DateTime, Convert.ToDateTime(cMFIn.originatedDateTime));
                sqlExec.AddCommandParam("@CommInfo1", SqlDbType.Char, cMFIn.commInfo1);
                if (cMFIn.commInfo2 == null)
                    sqlExec.AddCommandParam("@CommInfo2", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@CommInfo2", SqlDbType.Char, cMFIn.commInfo2);
                sqlExec.AddCommandParam("@ValidGps", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.validGPS));
                sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, Convert.ToDouble(cMFIn.latitude));
                sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, Convert.ToDouble(cMFIn.longitude));
                sqlExec.AddCommandParam("@Speed", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.speed));
                sqlExec.AddCommandParam("@Heading", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.heading));
                sqlExec.AddCommandParam("@SensorMask", SqlDbType.BigInt, Convert.ToInt64(cMFIn.sensorMask));
                if (cMFIn.customProperties == null)
                    sqlExec.AddCommandParam("@CustomProp", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@CustomProp", SqlDbType.Char, cMFIn.customProperties);
                sqlExec.AddCommandParam("@BlobDataSize", SqlDbType.Int, cMFIn.blobSize);
                sqlExec.AddCommandParam("@SequenceNum", SqlDbType.Int, cMFIn.sequenceNum);
                if (cMFIn.validGPS == 1)
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, Const.noValidAddress);
                else if (cMFIn.latitude == 0 || cMFIn.longitude == 0)
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, Const.noGPSData);
                else
                    //sqlExec.AddCommandParam("@StreetAddress",SqlDbType.Char,null);
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, DBNull.Value);

                sqlExec.AddCommandParam("@isArmed", SqlDbType.TinyInt, cMFIn.isArmed);
                //                sqlExec.AddCommandParam("@Priority", SqlDbType.TinyInt, cMFIn.priority);

                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "(SqlException) Unable to add new msg into MsgIn history with date '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "'.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                Util.BTrace(Util.ERR0, "(DASDbConnectionClosed) ");
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "(Exception) Unable to add new msg into MsgIn history with date '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "'.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "AddToHistory (0):Unable to add new msg into MsgIn history with date '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "'.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " The message with datetime '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "' already exists.");
            }

            Util.BTrace(Util.INF0, "<< AddToHistory -> {0} {1} {2} TE={3}", cMFIn.boxID,
               cMFIn.originatedDateTime, cMFIn.messageTypeID, stopWatch.ElapsedMilliseconds);

        }


        /// <summary>
        /// Backup Msg to the MsgIn history table.
        /// Delete exist message.
        /// </summary>
        /// <param name="cMFIn"></param>
        /// <param name="currTableName"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int AddToHistoryTable(CMFIn cMFIn, string tableName)
        {
            //           Util.BTrace(Util.INF0, " - AddToHistoryQueue -> {0}", cMFIn.ToString());
            int rowsAffected = 0;
            try
            {
                string sql = "INSERT INTO " + tableName + " ( " +
                   "BoxId" +
                   ",DateTimeReceived" +
                   ",DclId" +
                   ",BoxMsgInTypeId" +
                   ",BoxProtocolTypeId" +
                   ",OriginDateTime" +
                   ",CommInfo1" +
                   ",CommInfo2" +
                   ",ValidGps" +
                   ",Latitude" +
                   ",Longitude" +
                   ",Speed" +
                   ",Heading" +
                   ",SensorMask" +
                   ",CustomProp" +
                   ",BlobDataSize" +
                   ",SequenceNum" +
                   ",StreetAddress" +
                   ",IsArmed ) VALUES ( @BoxId,@DateTimeReceived,@DclId,@BoxMsgInTypeId,@BoxProtocolTypeId,@OriginDateTime,@CommInfo1,@CommInfo2,@ValidGps,@Latitude,@Longitude,@Speed,@Heading,@SensorMask,@CustomProp,@BlobDataSize,@SequenceNum,@StreetAddress,@isArmed) ";

                // Set SQL command
                sqlExec.ClearCommandParameters();
                // Add parameters to SQL statement
                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, cMFIn.boxID);
                sqlExec.AddCommandParam("@DateTimeReceived", SqlDbType.DateTime, Convert.ToDateTime(cMFIn.receivedDateTime));
                sqlExec.AddCommandParam("@DclId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.dclID));
                sqlExec.AddCommandParam("@BoxMsgInTypeId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.messageTypeID));
                sqlExec.AddCommandParam("@BoxProtocolTypeId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.protocolTypeID));
                sqlExec.AddCommandParam("@OriginDateTime", SqlDbType.DateTime, Convert.ToDateTime(cMFIn.originatedDateTime));
                sqlExec.AddCommandParam("@CommInfo1", SqlDbType.Char, cMFIn.commInfo1);
                if (cMFIn.commInfo2 == null)
                    sqlExec.AddCommandParam("@CommInfo2", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@CommInfo2", SqlDbType.Char, cMFIn.commInfo2);
                sqlExec.AddCommandParam("@ValidGps", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.validGPS));
                sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, Convert.ToDouble(cMFIn.latitude));
                sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, Convert.ToDouble(cMFIn.longitude));
                sqlExec.AddCommandParam("@Speed", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.speed));
                sqlExec.AddCommandParam("@Heading", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.heading));
                sqlExec.AddCommandParam("@SensorMask", SqlDbType.BigInt, Convert.ToInt64(cMFIn.sensorMask));
                if (cMFIn.customProperties == null)
                    sqlExec.AddCommandParam("@CustomProp", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@CustomProp", SqlDbType.Char, cMFIn.customProperties);
                sqlExec.AddCommandParam("@BlobDataSize", SqlDbType.Int, cMFIn.blobSize);
                sqlExec.AddCommandParam("@SequenceNum", SqlDbType.Int, cMFIn.sequenceNum);
                if (cMFIn.validGPS == 1)
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, Const.noValidAddress);
                else if (cMFIn.latitude == 0 || cMFIn.longitude == 0)
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, Const.noGPSData);
                else
                    //sqlExec.AddCommandParam("@StreetAddress",SqlDbType.Char,null);
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, DBNull.Value);

                sqlExec.AddCommandParam("@isArmed", SqlDbType.TinyInt, cMFIn.isArmed);
                //                sqlExec.AddCommandParam("@Priority", SqlDbType.TinyInt, cMFIn.priority);

                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "(SqlException) Unable to add new msg into AddToHistoryTable  with date '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "'.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                Util.BTrace(Util.ERR0, "(DASDbConnectionClosed) ");
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "(Exception) Unable to add new msg into AddToHistoryTable h date '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "'.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "AddToHistory (0):Unable to add new msg into AddToHistoryTable history with date '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "'.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " The message with datetime '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "' already exists.");
            }

            return rowsAffected;
        }



        /// <summary>
        /// Backup Msg to the MsgIn history table.
        /// Delete exist message.
        /// </summary>
        /// <param name="cMFIn"></param>
        /// <param name="currTableName"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public Int64 AddToHistoryQueue(CMFIn cMFIn)
        {
            //           Util.BTrace(Util.INF0, " - AddToHistoryQueue -> {0}", cMFIn.ToString());
            Int64 rowsAffected = 0;
            try
            {
                string sql = "vlfMsgInHstQueued_Insert";

                // Set SQL command
                sqlExec.ClearCommandParameters();
                // Add parameters to SQL statement
                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, cMFIn.boxID);
                sqlExec.AddCommandParam("@DateTimeReceived", SqlDbType.DateTime, Convert.ToDateTime(cMFIn.receivedDateTime));
                sqlExec.AddCommandParam("@DclId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.dclID));
                sqlExec.AddCommandParam("@BoxMsgInTypeId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.messageTypeID));
                sqlExec.AddCommandParam("@BoxProtocolTypeId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.protocolTypeID));
                sqlExec.AddCommandParam("@OriginDateTime", SqlDbType.DateTime, Convert.ToDateTime(cMFIn.originatedDateTime));
                sqlExec.AddCommandParam("@CommInfo1", SqlDbType.Char, cMFIn.commInfo1);
                if (cMFIn.commInfo2 == null)
                    sqlExec.AddCommandParam("@CommInfo2", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@CommInfo2", SqlDbType.Char, cMFIn.commInfo2);
                sqlExec.AddCommandParam("@ValidGps", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.validGPS));
                sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, Convert.ToDouble(cMFIn.latitude));
                sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, Convert.ToDouble(cMFIn.longitude));
                sqlExec.AddCommandParam("@Speed", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.speed));
                sqlExec.AddCommandParam("@Heading", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.heading));
                sqlExec.AddCommandParam("@SensorMask", SqlDbType.BigInt, Convert.ToInt64(cMFIn.sensorMask));
                if (cMFIn.customProperties == null)
                    sqlExec.AddCommandParam("@CustomProp", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@CustomProp", SqlDbType.Char, cMFIn.customProperties);
                sqlExec.AddCommandParam("@BlobDataSize", SqlDbType.Int, cMFIn.blobSize);
                sqlExec.AddCommandParam("@SequenceNum", SqlDbType.Int, cMFIn.sequenceNum);
                if (cMFIn.validGPS == 1)
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, Const.noValidAddress);
                else if (cMFIn.latitude == 0 || cMFIn.longitude == 0)
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, Const.noGPSData);
                else
                    //sqlExec.AddCommandParam("@StreetAddress",SqlDbType.Char,null);
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, DBNull.Value);

                sqlExec.AddCommandParam("@isArmed", SqlDbType.TinyInt, cMFIn.isArmed);
                //                sqlExec.AddCommandParam("@Priority", SqlDbType.TinyInt, cMFIn.priority);

                //Executes SQL statement
                //rowsAffected = sqlExec.SPExecuteNonQuery(sql);
                rowsAffected = Convert.ToInt64(sqlExec.SPExecuteScalar(sql));
            }
            catch (SqlException objException)
            {
                string prefixMsg = "(SqlException) Unable to add new msg into AddToHistoryQueue  with date '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "'.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                Util.BTrace(Util.ERR0, "(DASDbConnectionClosed) ");
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "(Exception) Unable to add new msg into AddToHistoryQueue h date '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "'.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "AddToHistory (0):Unable to add new msg into AddToHistoryQueue history with date '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "'.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " The message with datetime '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "' already exists.");
            }
            return rowsAffected;

        }



        /// <summary>
        /// Update Processed In History Queue
        /// </summary>
        /// <param name="cMFIn"></param>
        /// <param name="tableId"></param>
        public void UpdateProcessedInHistoryQueue(CMFInEx cMFIn, CLS.Def.Enums.HistoryTables tableId, bool successfullyInserted)
        {
            //           Util.BTrace(Util.INF0, " - AddToHistoryQueue -> {0}", cMFIn.ToString());
            int rowsAffected = 0;
            try
            {
                string sql = "vlfMsgInHstQueued_Update";

                // Set SQL command
                sqlExec.ClearCommandParameters();
                // Add parameters to SQL statement
                // sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, cMFIn.boxID);
                // sqlExec.AddCommandParam("@OriginDateTime", SqlDbType.DateTime, Convert.ToDateTime(cMFIn.originatedDateTime));
                sqlExec.AddCommandParam("@ID", SqlDbType.BigInt, cMFIn.ID);
                sqlExec.AddCommandParam("@Table", SqlDbType.SmallInt, Convert.ToInt16(tableId));
                sqlExec.AddCommandParam("@Success", SqlDbType.SmallInt, successfullyInserted ? 1 : 2);

                //Executes SQL statement
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "(SqlException) Unable to update MsgQueued processed with ID: '" + cMFIn.ID.ToString() + "'.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                Util.BTrace(Util.ERR0, "(DASDbConnectionClosed) ");
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "(SqlException) Unable to update MsgQueued processed with ID: '" + cMFIn.ID.ToString() + "'.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "(SqlException) Unable to update MsgQueued processed with ID: '" + cMFIn.ID.ToString() + "'.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " The message with ID: '" + cMFIn.ID.ToString() + "' does not exist!");
            }
        }

        /// <summary>
        /// Backup Msg to the vlfMsgInHstSLS history table.
        /// Delete exist message.
        /// </summary>
        /// <param name="cMFIn"></param>
        /// <param name="currTableName"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AddToHistorySLS(CMFIn cMFIn)
        {
            //           Util.BTrace(Util.INF0, " - AddToHistoryQueue -> {0}", cMFIn.ToString());
            int rowsAffected = 0;
            try
            {
                string sql = "INSERT INTO vlfMsgInHstSLS ( " +
                   "BoxId" +
                   ",DateTimeReceived" +
                   ",DclId" +
                   ",BoxMsgInTypeId" +
                   ",BoxProtocolTypeId" +
                   ",OriginDateTime" +
                   ",CommInfo1" +
                   ",CommInfo2" +
                   ",ValidGps" +
                   ",Latitude" +
                   ",Longitude" +
                   ",Speed" +
                   ",Heading" +
                   ",SensorMask" +
                   ",CustomProp" +
                   ",BlobDataSize" +
                   ",SequenceNum" +
                   ",StreetAddress" +
                   ",IsArmed ) VALUES ( @BoxId,@DateTimeReceived,@DclId,@BoxMsgInTypeId,@BoxProtocolTypeId,@OriginDateTime,@CommInfo1,@CommInfo2,@ValidGps,@Latitude,@Longitude,@Speed,@Heading,@SensorMask,@CustomProp,@BlobDataSize,@SequenceNum,@StreetAddress,@isArmed) ";

                // Set SQL command
                sqlExec.ClearCommandParameters();
                // Add parameters to SQL statement
                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, cMFIn.boxID);
                sqlExec.AddCommandParam("@DateTimeReceived", SqlDbType.DateTime, Convert.ToDateTime(cMFIn.receivedDateTime));
                sqlExec.AddCommandParam("@DclId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.dclID));
                sqlExec.AddCommandParam("@BoxMsgInTypeId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.messageTypeID));
                sqlExec.AddCommandParam("@BoxProtocolTypeId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.protocolTypeID));
                sqlExec.AddCommandParam("@OriginDateTime", SqlDbType.DateTime, Convert.ToDateTime(cMFIn.originatedDateTime));
                sqlExec.AddCommandParam("@CommInfo1", SqlDbType.Char, cMFIn.commInfo1);
                if (cMFIn.commInfo2 == null)
                    sqlExec.AddCommandParam("@CommInfo2", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@CommInfo2", SqlDbType.Char, cMFIn.commInfo2);
                sqlExec.AddCommandParam("@ValidGps", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.validGPS));
                sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, Convert.ToDouble(cMFIn.latitude));
                sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, Convert.ToDouble(cMFIn.longitude));
                sqlExec.AddCommandParam("@Speed", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.speed));
                sqlExec.AddCommandParam("@Heading", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.heading));
                sqlExec.AddCommandParam("@SensorMask", SqlDbType.BigInt, Convert.ToInt64(cMFIn.sensorMask));
                if (cMFIn.customProperties == null)
                    sqlExec.AddCommandParam("@CustomProp", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@CustomProp", SqlDbType.Char, cMFIn.customProperties);
                sqlExec.AddCommandParam("@BlobDataSize", SqlDbType.Int, cMFIn.blobSize);
                sqlExec.AddCommandParam("@SequenceNum", SqlDbType.Int, cMFIn.sequenceNum);
                if (cMFIn.validGPS == 1)
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, Const.noValidAddress);
                else if (cMFIn.latitude == 0 || cMFIn.longitude == 0)
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, Const.noGPSData);
                else
                    //sqlExec.AddCommandParam("@StreetAddress",SqlDbType.Char,null);
                    sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, DBNull.Value);

                sqlExec.AddCommandParam("@isArmed", SqlDbType.TinyInt, cMFIn.isArmed);
                //                sqlExec.AddCommandParam("@Priority", SqlDbType.TinyInt, cMFIn.priority);

                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "(SqlException) Unable to add new msg into MsgInQueued  with date '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "'.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                Util.BTrace(Util.ERR0, "(DASDbConnectionClosed) ");
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "(Exception) Unable to add new msg into MsgInQueued h date '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "'.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "AddToHistory (0):Unable to add new msg into MsgInQueued history with date '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "'.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " The message with datetime '" + Convert.ToDateTime(cMFIn.originatedDateTime).ToString() + "' already exists.");
            }
        }

        protected DataSet RetrievesNextMsg(int slsId, int cnt, string prefixMsg)
        {
            DataSet sqlDataSet = null;
            try
            {
                sqlExec.ClearCommandParameters();
                //Prepares SQL statement
                sqlExec.AddCommandParam("@SLSId", SqlDbType.Int, slsId);
                sqlExec.AddCommandParam("@RowNum", SqlDbType.Int, cnt);
                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset("sp_GetTopMsgHstForSLS");
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

            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves next record.
        /// </summary>
        /// <param name="prefixMsg"></param>
        /// <returns>DataSet [DateTimeReceived],[BoxId],[DclId],[BoxMsgInTypeId],
        /// [BoxProtocolTypeId],[OriginDateTime],[CommInfo1],[CommInfo2],[ValidGps],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [BlobData],[BlobDataSize],[SequenceNum],[IsArmed]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>

        protected DataSet RetrievesNextMsg(int cnt, string prefixMsg)
        {
            DataSet sqlDataSet = null;
            try
            {
                sqlExec.ClearCommandParameters();
                //Prepares SQL statement
                sqlExec.AddCommandParam("@RowNum", SqlDbType.Int, cnt);
                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset("sp_GetTopMsgHstInMem");  // sp_GetTopMsgHstNew2
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

            return sqlDataSet;
        }

        protected DataSet RetrievesNextMsg_SLSProcessIn(int cnt, string prefixMsg)
        {
            DataSet sqlDataSet = null;
            try
            {
                sqlExec.ClearCommandParameters();
                //Prepares SQL statement
                sqlExec.AddCommandParam("@RowNum", SqlDbType.Int, cnt);
                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset("sp_GetTopMsgHst_SLSNew");
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

            return sqlDataSet;
        }


        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves messages from history by filter
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sqlAddFrom"></param>
        /// <param name="sqlWhere"></param>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],
        /// [SequenceNum],[BoxArmed]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        protected DataSet GetMessagesFromHistory_NewTZ(int userId, string sqlAddFrom, string sqlWhere)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement

                string sql = " DECLARE @ResolveLandmark int DECLARE @Timezone float" + // Changes for TimeZone feature ends
                     " DECLARE @Unit real" +
                     " DECLARE @DayLightSaving int" +
                     " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                     " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZoneNew +
                     " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference" +
                        " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
                     " IF @Timezone IS NULL SET @Timezone=0" +
                     " IF @Unit IS NULL SET @Unit=1" +
                     " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving +
                     " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                     " SET @Timezone= @Timezone + @DayLightSaving" +
                            " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +

                     " SELECT vlfMsgInHst.BoxId,DateTimeReceived,DclId," +
                     "vlfBoxMsgInType.BoxMsgInTypeId,BoxMsgInTypeName," +
                     "vlfBoxProtocolType.BoxProtocolTypeId,BoxProtocolTypeName," +
                     "CommInfo1,CommInfo2,ValidGps,Latitude,Longitude,Heading,SensorMask," +
                     "CustomProp,BlobDataSize,SequenceNum," +
                            "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + Const.addressNA + "') AS StreetAddress," +

                     "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),OriginDateTime) END AS OriginDateTime," +
                     "CASE WHEN vlfMsgInHst.Speed IS NULL then 0 ELSE ROUND(vlfMsgInHst.Speed * @Unit,1) END AS Speed," +
                     "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed" +
                     " FROM vlfMsgInHst with (nolock) ,vlfBoxMsgInType,vlfBoxProtocolType,vlfBox with (nolock)" +
                     sqlAddFrom + sqlWhere +
                     " AND vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId" +
                     " AND vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId" +
                     " ORDER BY OriginDateTime DESC, DateTimeReceived DESC";

                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            return resultDataSet;
        }
        // Changes for TimeZone Feature end


        /// <summary>
        /// Retrieves messages from history by filter
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sqlAddFrom"></param>
        /// <param name="sqlWhere"></param>
        /// <returns>DataSet [BoxId],[DateTimeReceived],[OriginDateTime],[DclId],
        /// [BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],
        /// [Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],
        /// [SequenceNum],[BoxArmed]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        protected DataSet GetMessagesFromHistory(int userId, string sqlAddFrom, string sqlWhere)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = " DECLARE @ResolveLandmark int DECLARE @Timezone int" +
                     " DECLARE @Unit real" +
                     " DECLARE @DayLightSaving int" +
                     " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                        " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZone +
                     " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference" +
                        " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.MeasurementUnits +
                     " IF @Timezone IS NULL SET @Timezone=0" +
                     " IF @Unit IS NULL SET @Unit=1" +
                     " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving +
                     " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                     " SET @Timezone= @Timezone + @DayLightSaving" +
                            " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +

                     " SELECT vlfMsgInHst.BoxId,DateTimeReceived,DclId," +
                     "vlfBoxMsgInType.BoxMsgInTypeId,BoxMsgInTypeName," +
                     "vlfBoxProtocolType.BoxProtocolTypeId,BoxProtocolTypeName," +
                     "CommInfo1,CommInfo2,ValidGps,Latitude,Longitude,Heading,SensorMask," +
                     "CustomProp,BlobDataSize,SequenceNum," +
                            "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + Const.addressNA + "') AS StreetAddress," +

                     "CASE WHEN OriginDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,OriginDateTime) END AS OriginDateTime," +
                     "CASE WHEN vlfMsgInHst.Speed IS NULL then 0 ELSE ROUND(vlfMsgInHst.Speed * @Unit,1) END AS Speed," +
                     "CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed" +
                     " FROM vlfMsgInHst with (nolock) ,vlfBoxMsgInType,vlfBoxProtocolType,vlfBox with (nolock)" +
                     sqlAddFrom + sqlWhere +
                     " AND vlfMsgInHst.BoxMsgInTypeId=vlfBoxMsgInType.BoxMsgInTypeId" +
                     " AND vlfMsgInHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId" +
                     " ORDER BY OriginDateTime DESC, DateTimeReceived DESC";
                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            return resultDataSet;
        }
        /// <summary>
        /// Retrieves specific messages from history by box and originated time
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="originDateTime"></param>
        /// <returns>DataSet [DclId],[BoxMsgInTypeId],[BoxProtocolTypeId],
        /// [CommInfo1],[CommInfo2],
        /// [ValidGps],[Latitude],[Longitude],[Speed],[Heading],
        /// [SensorMask],[CustomProp],[BoxArmed]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        protected DataSet GetMessageFromHistory(int boxId, DateTime originDateTime)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = @"SELECT DclId,BoxMsgInTypeId,BoxProtocolTypeId,
                               RTRIM(CommInfo1) AS CommInfo1,RTRIM(CommInfo2) AS CommInfo2,
                               ValidGps,Latitude,Longitude,Speed,Heading,DateTimeReceived,
                               SensorMask,RTRIM(CustomProp) AS CustomProp,
                               CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed" +
                              " FROM vlfMsgInHst with (nolock) WHERE BoxId=" + boxId +
                              " AND OriginDateTime='" + originDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            return resultDataSet;
        }


        protected DataSet GetMessageFromHistoryQueue(int boxId, DateTime originDateTime)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = @"SELECT DclId,BoxMsgInTypeId,BoxProtocolTypeId,
                               RTRIM(CommInfo1) AS CommInfo1,RTRIM(CommInfo2) AS CommInfo2,
                               ValidGps,Latitude,Longitude,Speed,Heading,DateTimeReceived,
                               SensorMask,RTRIM(CustomProp) AS CustomProp,
                               CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.False + " then 'false' ELSE CASE WHEN IsArmed=" + (short)Enums.ArmedStatus.True + " then 'true' ELSE ' ' END END AS BoxArmed" +
                              " FROM vlfMsgInHstQueued1 WHERE BoxId=" + boxId +
                              " AND OriginDateTime='" + originDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            return resultDataSet;
        }
        #endregion

        #region Configuration Methods
        private int GetConfigParameter(string moduleName, short groupID, string paramName, int defaultValue)
        {
            int paramValue = defaultValue;
            DB.Configuration config = new DB.Configuration(sqlExec);

            // take Module ID in DB
            short moduleID = config.GetConfigurationModuleTypeId(moduleName);
            if (moduleID == Const.unassignedShortValue)
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

        private string GetConfigParameter(string moduleName, short groupID, string paramName, string defaultValue)
        {
            string paramValue = defaultValue;
            DB.Configuration config = new DB.Configuration(sqlExec);

            // take Module ID in DB
            short moduleID = config.GetConfigurationModuleTypeId(moduleName);
            if (moduleID == Const.unassignedShortValue)
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
                // TODO: log error
                //throw new VLF.ERR.DASException("'" + paramName + "' is not set in DB. Setting up default value: " + paramValue.ToString() + ".");
            }
            return paramValue;
        }
        #endregion Configuration Methods

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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetInvalidGPSStatistic(int InvalidGPSPercent, int Hours)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "CREATE TABLE  #ValidMsgs(BoxId  int,Msgs  int) " +
                          "CREATE TABLE  #InvalidMsgs(BoxId  int,Msgs  int) " +
                          "INSERT INTO #ValidMsgs SELECT BoxId,Count(BoxId) as ValidMsg FROM vlfMsgInHst with (nolock) where DateTimeReceived>dateadd(hour,-" + Hours + ",getdate()) AND ValidGPS=0 GROUP BY BoxId " +
                          "INSERT INTO #InvalidMsgs SELECT BoxId,Count(BoxId) as ValidMsg FROM vlfMsgInHst with (nolock) WHERE DateTimeReceived>dateadd(hour,-" + Hours + ",getdate()) AND ValidGPS=1 GROUP BY BoxId " +
                          "SELECT     vlfBox.BoxId,vlfVehicleAssignment.LicensePlate,vlfVehicleAssignment.VehicleId, " +
                          "vlfVehicleInfo.Description,vlfOrganization.OrganizationName," +
                           "#InvalidMsgs.Msgs as InvalidMsgs,#ValidMsgs.Msgs as ValidMsgs," +
                            "ROUND( #InvalidMsgs.Msgs/cast(#ValidMsgs.Msgs+#InvalidMsgs.Msgs as real)*100,1) as PercentInvalidMsgs " +
                          "FROM   vlfBox with (nolock) INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId " +
                          "INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId " +
                          "INNER JOIN vlfOrganization ON vlfVehicleInfo.OrganizationId = vlfOrganization.OrganizationId " +
                          "LEFT JOIN #ValidMsgs ON vlfBox.BoxId = #ValidMsgs.BoxId " +
                          "LEFT JOIN #InvalidMsgs ON vlfBox.BoxId = #InvalidMsgs.BoxId " +
                          "WHERE #InvalidMsgs.Msgs IS NOT NULL " +
                          " AND (#InvalidMsgs.Msgs/CAST((#ValidMsgs.Msgs+#InvalidMsgs.Msgs) AS REAL))*100>=" + InvalidGPSPercent.ToString() +
                            " DROP TABLE #ValidMsgs DROP TABLE #InvalidMsgs";

                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            return resultDataSet;

        }

        /// <summary>
        /// Retrieves boxes without IP Updates  for all boxes within 24 hours
        /// </summary>
        /// <param name="Hours"></param>
        /// <returns>
        /// DataSet [BoxId],[LicensePlate],[VehicleId],[Description],
        /// [OrganizationName]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetBoxesWithoutIpUpdates(int Hours)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT   vlfBox.BoxId,vlfVehicleAssignment.LicensePlate,vlfVehicleAssignment.VehicleId, " +
                         "vlfVehicleInfo.Description,vlfOrganization.OrganizationName" +
                         " FROM   vlfBox with (nolock) INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId  " +
                         "INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId " +
                         "INNER JOIN vlfOrganization ON vlfVehicleInfo.OrganizationId = vlfOrganization.OrganizationId " +
                         "where  vlfBox.BoxId NOT IN (SELECT BoxId FROM vlfMsgInHst with (nolock) WHERE DateTimeReceived>dateadd(hour,-" + Hours + ",getdate()))";
                //"where  vlfBox.BoxId NOT IN (SELECT BoxId FROM vlfMsgInHst with (nolock) WHERE DateTimeReceived>dateadd(hour,-"+Hours+",getdate()) and BoxMsgInTypeId ="+Convert.ToInt16(Enums.MessageType.IPUpdate) +")";
                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            return resultDataSet;
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetBoxesReportedFrequency(int Hours, int TotalMsg, Int16 OrganizationId)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT     vlfBox.BoxId,COUNT(vlfBox.BoxId) as MsgInCount " +
                         " INTO #tmpIn FROM    vlfBox with (nolock) " +
                         " INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId " +
                         " INNER JOIN vlfMsgInHst with (nolock) ON vlfBox.BoxId = vlfMsgInHst.BoxId " +
                         " WHERE	 vlfMsgInHst.DateTimeReceived>DATEADD(HOUR,-24,GETDATE()) " +
                         " and vlfMsgInHst.BoxMsgInTypeId<>" + Convert.ToInt16(Enums.MessageType.Ack) +
                         " and vlfMsgInHst.BoxMsgInTypeId<>" + Convert.ToInt16(Enums.MessageType.MDTAck) +
                         " and vlfMsgInHst.BoxMsgInTypeId<>" + Convert.ToInt16(Enums.MessageType.NAck);

                if (OrganizationId != Const.unassignedShortValue)
                    sql += " and vlfBox.OrganizationId=" + OrganizationId;

                sql += " GROUP BY  vlfBox.BoxId " +

          " SELECT     vlfBox.BoxId,COUNT(vlfBox.BoxId) as MsgInCount " +
                " INTO #tmpInIgnored FROM    vlfBox  with (nolock)	" +
                " INNER JOIN  vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId " +
                " INNER JOIN  vlfMsgInHstIgnored ON vlfBox.BoxId = vlfMsgInHstIgnored.BoxId " +
                " WHERE	 vlfMsgInHstIgnored.DateTimeReceived>dateadd(hour,-" + Hours + ",getdate()) " +
                " and vlfMsgInHstIgnored.BoxMsgInTypeId<>" + Convert.ToInt16(Enums.MessageType.Ack) +
                " and vlfMsgInHstIgnored.BoxMsgInTypeId<>" + Convert.ToInt16(Enums.MessageType.MDTAck) +
                " and vlfMsgInHstIgnored.BoxMsgInTypeId<>" + Convert.ToInt16(Enums.MessageType.NAck);
                if (OrganizationId != Const.unassignedShortValue)
                    sql += " and vlfBox.OrganizationId=" + OrganizationId;

                sql += " GROUP BY  vlfBox.BoxId " +

                " SELECT  vlfBox.BoxId,SUM(ISNULL(#tmpIn.MsgInCount,0)+ISNULL(#tmpInIgnored.MsgInCount,0)) as TotalMessages,vlfOrganization.OrganizationName" +
                " FROM vlfBox with (nolock) INNER JOIN vlfOrganization ON vlfBox.OrganizationId=vlfOrganization.OrganizationId " +
                " LEFT JOIN #tmpIn ON vlfBox.BoxId = #tmpIn.BoxId " +
                " LEFT JOIN #tmpInIgnored ON vlfBox.BoxId = #tmpInIgnored.BoxId ";
                if (OrganizationId != Const.unassignedShortValue)
                    sql += " and vlfBox.OrganizationId=" + OrganizationId;
                sql += " GROUP BY vlfBox.BoxId,vlfOrganization.OrganizationName" +
                " HAVING SUM(ISNULL(#tmpIn.MsgInCount,0)+ISNULL(#tmpInIgnored.MsgInCount,0))>" + TotalMsg +
                " DROP TABLE #tmpIn" +
                " DROP TABLE #tmpInIgnored";
                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve messages from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve messages from history" + objException.Message);
            }
            return resultDataSet;
        }
        #endregion Diagnostic Interfaces

        public DataSet GetGetOrgBoxes(int orgId)
        {
            DataSet dsResult = null;
            // 1. Prepares SQL statement
            string sql = "create table #tmp (OrganizationName varchar(50),boxId int,OriginDateTime datetime,CustomProp varchar(1024)) DECLARE @FromBoxId int DECLARE @OrganizationId int SET @OrganizationId=" + orgId + " DECLARE boxIds_cursor CURSOR FOR SELECT vlfBox.BoxId FROM vlfBox INNER JOIN vlfOrganization ON vlfBox.OrganizationId = vlfOrganization.OrganizationId INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId WHERE vlfOrganization.OrganizationId = @OrganizationId and vlfBox.BoxId<>0 ORDER BY vlfBox.BoxId OPEN boxIds_cursor FETCH NEXT FROM boxIds_cursor INTO @FromBoxId WHILE @@FETCH_STATUS = 0 BEGIN insert into #tmp SELECT TOP 1 OrganizationName,vlfBox.BoxId, OriginDateTime, SUBSTRING(CustomProp,PATINDEX('%Firmware Version=%',CustomProp)+17,20) FROM vlfMsgInHst with (nolock) INNER JOIN vlfBox ON vlfMsgInHst.BoxId = vlfBox.BoxId INNER JOIN vlfOrganization ON vlfBox.OrganizationId = vlfOrganization.OrganizationId WHERE BoxMsgInTypeId = 13 AND vlfMsgInHst.BoxId =@FromBoxId ORDER BY OriginDateTime DESC FETCH NEXT FROM boxIds_cursor INTO @FromBoxId END CLOSE boxIds_cursor DEALLOCATE boxIds_cursor SELECT vlfBox.BoxId FROM vlfBox INNER JOIN vlfOrganization ON vlfBox.OrganizationId = vlfOrganization.OrganizationId INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId WHERE vlfOrganization.OrganizationId = @OrganizationId and vlfBox.BoxId<>0 and vlfBox.BoxId NOT IN (select BoxId from #tmp) ORDER BY vlfBox.BoxId drop table #tmp";

            try
            {
                dsResult = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("GetGetOrgBoxes", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception exp)
            {
                throw new DASException("GetGetOrgBoxes" + exp.Message);
            }

            return dsResult;
        }

        public DataSet GetOrgAllBoxes(int orgId)
        {
            DataSet dsResult = null;

            // 1. Prepares SQL statement
            string sql = "SELECT vlfBox.BoxId FROM vlfBox with (nolock) INNER JOIN vlfOrganization ON vlfBox.OrganizationId = vlfOrganization.OrganizationId WHERE vlfOrganization.OrganizationId =" + orgId + " AND vlfBox.BoxId <> 0 ORDER BY vlfBox.BoxId";
            try
            {
                dsResult = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("GetGetOrgBoxes", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception exp)
            {
                throw new DASException("GetGetOrgBoxes" + exp.Message);
            }

            return dsResult;
        }



        // Changes for TimeZone Feature start
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
        public DataSet GetMessagesFromHistoryByFleetId_NewTZ(int fleetId,
                                                     DateTime from,
                                                     DateTime to)
        {
            DataSet sqlDataSet = null;
            try
            {

                sqlExec.ClearCommandParameters();
                string sql = "sp_GetFleetStatusHistoryByFleetId_NewTimeZone";


                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
                return sqlDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info from=" + from + " to=" + to + " for fleetid=" + fleetId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info from=" + from + " to=" + to + " for fleetid=" + fleetId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
        // Changes for TimeZone Feature end


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
            DataSet sqlDataSet = null;
            try
            {

                sqlExec.ClearCommandParameters();
                string sql = "sp_GetFleetStatusHistoryByFleetId";


                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
                return sqlDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info from=" + from + " to=" + to + " for fleetid=" + fleetId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info from=" + from + " to=" + to + " for fleetid=" + fleetId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
            DataSet sqlDataSet = null;
            try
            {

                sqlExec.ClearCommandParameters();
                string sql = "GetLastMessagesFromHistoryNotProcessed";

                sqlExec.AddCommandParam("@boxId", SqlDbType.BigInt, boxId);
                sqlExec.AddCommandParam("@boxMsgInTypeId", SqlDbType.SmallInt, boxMsgInTypeId);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
                return sqlDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "GetLastMessagesFromHistoryNotProcessed -> Unable to retrieve for BoxId:" + boxId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "GetLastMessagesFromHistoryNotProcessed -> Unable to retrieve for BoxId:" + boxId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
            DataSet sqlDataSet = null;
            try
            {

                sqlExec.ClearCommandParameters();
                string sql = "GetVehiclesInHistoryByAddress";


                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                sqlExec.AddCommandParam("@address", SqlDbType.VarChar, address);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
                return sqlDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetVehiclesInHistoryByAddress from=" + from + " to=" + to + " for fleetid=" + fleetId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetVehiclesInHistoryByAddress from=" + from + " to=" + to + " for fleetid=" + fleetId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
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
            DataSet sqlDataSet = null;
            try
            {

                sqlExec.ClearCommandParameters();
                string sql = "GetVehiclesInHistoryByLocation";


                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                sqlExec.AddCommandParam("@latitude", SqlDbType.Float, latitude);
                sqlExec.AddCommandParam("@longitude", SqlDbType.Float, longitude);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
                return sqlDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetVehiclesInHistoryByAddress from=" + from + " to=" + to + " for fleetid=" + fleetId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetVehiclesInHistoryByAddress from=" + from + " to=" + to + " for fleetid=" + fleetId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }



        public DataSet GetMsgInTypesWithPriority()
        {
            DataSet sqlDataSet = null;
            try
            {

                sqlExec.ClearCommandParameters();
                string sql = "GetBoxMsgInTypes";
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
                return sqlDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to MsgInTypes with Priority. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to MsgInTypes with Priority. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


    }
}

