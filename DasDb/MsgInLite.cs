using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;			// for CMFIn

namespace VLF.DAS.DB
{   
   /// <summary>
   /// Provides interfaces to vlfMsgIn table.
   /// </summary>
   public class MsgInLite : TblGenInterfaces
   {
      public static DateTime dtReferenceLow  = new DateTime(2000, 01, 01);
      public static DateTime dtReferenceHigh = new DateTime(2020, 01, 01);
      private const string prefixNextMsg = "Unable to retrieve next message. ";

      #region Public MsgIn Interfaces
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public MsgInLite(SQLExecuter sqlExec)
         : base("vlfMsgIn", sqlExec)
      {
      }

      public MsgInLite(bool hist, SQLExecuter sqlExec)
         : base("vlfMsgInHst", sqlExec)
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
      #endregion

      #region Protected Interfaces

      public void DeleteMsgTimeRange(CMFIn cMFIn)
      {
         string sql = "";
         int rowsAffected = 0;
         try
         {
             sql = string.Format("DELETE FROM vlfMsgIn where boxid={0} and OriginDateTime >= '{1}' and OriginDateTime <= '{2}' and BoxMsgInTypeId={3}",
               cMFIn.boxID,
               cMFIn.originatedDateTime.AddMilliseconds(-Const.nextDateTimeMillisecInterval*10).ToString("MM/dd/yyyy HH:mm:ss.fff"),
               cMFIn.originatedDateTime.AddMilliseconds(Const.nextDateTimeMillisecInterval*10).ToString("MM/dd/yyyy HH:mm:ss.fff"), 
               cMFIn.messageTypeID);
#if TESTING
            Util.BTrace(Util.INF0, sql);
#endif
            sqlExec.ClearCommandParameters();
            //Executes SQL statement
             rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

            if (rowsAffected > 1)
            {
               Util.BTrace(Util.INF0, "DeleteMsgTimeRange -> found {0} messages with boxid={1} OriginDateTime={2}",
                  rowsAffected, cMFIn.boxID, cMFIn.originatedDateTime);
            }
            else
               if (rowsAffected == 0)
               {
                   sql = string.Format("DELETE FROM vlfMsgIn_Tmp where boxid={0} and OriginDateTime >= '{1}' and OriginDateTime <= '{2}' and BoxMsgInTypeId={3}",
                  cMFIn.boxID,
                  cMFIn.originatedDateTime.AddMilliseconds(-Const.nextDateTimeMillisecInterval * 10).ToString("MM/dd/yyyy HH:mm:ss.fff"),
                  cMFIn.originatedDateTime.AddMilliseconds(Const.nextDateTimeMillisecInterval * 10).ToString("MM/dd/yyyy HH:mm:ss.fff"),
                  cMFIn.messageTypeID);

                   sqlExec.ClearCommandParameters();
                   rowsAffected=sqlExec.SQLExecuteNonQuery(sql);

                   if (rowsAffected == 0)
                   {
                       string prefixMsg = string.Format("Unable to delete msgIn: recvTime={0}, origTime={1}, boxId={2}, dclId={3}, msgId={4}, protId={5}", cMFIn.receivedDateTime, cMFIn.originatedDateTime, cMFIn.boxID, cMFIn.dclID, cMFIn.messageTypeID, cMFIn.protocolTypeID);
                       throw new DASAppDataAlreadyExistsException(prefixMsg + " The message doesn't exist.");
                   }
               }
         }
         catch (SqlException objException)
         {
            //string prefixMsg = "Unable to add new msg In with date '" + Convert.ToDateTime(cMFIn.receivedDateTime).ToString() + "'.";
            string prefixMsg = string.Format("Unable to delete msgIn: recvTime={0}, origTime={1}, boxId={2}, dclId={3}, msgId={4}, protId={5}", cMFIn.receivedDateTime, cMFIn.originatedDateTime, cMFIn.boxID, cMFIn.dclID, cMFIn.messageTypeID, cMFIn.protocolTypeID);
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = string.Format("Unable to delete msgIn: recvTime={0}, origTime={1}, boxId={2}, dclId={3}, msgId={4}, protId={5}", cMFIn.receivedDateTime, cMFIn.originatedDateTime, cMFIn.boxID, cMFIn.dclID, cMFIn.messageTypeID, cMFIn.protocolTypeID);
            throw new DASException(prefixMsg + " " + objException.Message);
         }
      }

      public void DeleteMsgTimeRange_TmpSLS(CMFIn cMFIn)
      {
          try
          {
              string sql = string.Format("DELETE FROM vlfMsgIn_TmpForNewSLS where boxid={0} and OriginDateTime >= '{1}' and OriginDateTime <= '{2}' and BoxMsgInTypeId={3}",
                 cMFIn.boxID,
                 cMFIn.originatedDateTime.AddMilliseconds(-Const.nextDateTimeMillisecInterval * 10).ToString("MM/dd/yyyy HH:mm:ss.fff"),
                 cMFIn.originatedDateTime.AddMilliseconds(Const.nextDateTimeMillisecInterval * 10).ToString("MM/dd/yyyy HH:mm:ss.fff"),
                 cMFIn.messageTypeID);
#if TESTING
            Util.BTrace(Util.INF0, sql);
#endif
              sqlExec.ClearCommandParameters();
              //Executes SQL statement
              int rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

              if (rowsAffected > 1)
              {
                  Util.BTrace(Util.INF0, "DeleteMsgTimeRange_TmpSLS -> found {0} messages with boxid={1} OriginDateTime={2}",
                     rowsAffected, cMFIn.boxID, cMFIn.originatedDateTime);
              }
              else
                  if (rowsAffected == 0)
                  {
                      string prefixMsg = string.Format("Unable to delete DeleteMsgTimeRange_TmpSLS: recvTime={0}, origTime={1}, boxId={2}, dclId={3}, msgId={4}, protId={5}", cMFIn.receivedDateTime, cMFIn.originatedDateTime, cMFIn.boxID, cMFIn.dclID, cMFIn.messageTypeID, cMFIn.protocolTypeID);
                      throw new DASAppDataAlreadyExistsException(prefixMsg + " The message doesn't exist.");
                  }
          }
          catch (SqlException objException)
          {
              //string prefixMsg = "Unable to add new msg In with date '" + Convert.ToDateTime(cMFIn.receivedDateTime).ToString() + "'.";
              string prefixMsg = string.Format("Unable to delete DeleteMsgTimeRange_TmpSLS: recvTime={0}, origTime={1}, boxId={2}, dclId={3}, msgId={4}, protId={5}", cMFIn.receivedDateTime, cMFIn.originatedDateTime, cMFIn.boxID, cMFIn.dclID, cMFIn.messageTypeID, cMFIn.protocolTypeID);
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = string.Format("Unable to delete DeleteMsgTimeRange_TmpSLS: recvTime={0}, origTime={1}, boxId={2}, dclId={3}, msgId={4}, protId={5}", cMFIn.receivedDateTime, cMFIn.originatedDateTime, cMFIn.boxID, cMFIn.dclID, cMFIn.messageTypeID, cMFIn.protocolTypeID);
              throw new DASException(prefixMsg + " " + objException.Message);
          }
      }


      /** \fn        public void DeleteMsg(CMFIn cMFIn)
       *  \comment   
       */       
      public void DeleteMsg(CMFIn cMFIn)
      {
          string sql = "";
          int rowsAffected = 0;
         try
         {
#if false
//            string sql = "DELETE FROM vlfMsgIn where boxid=@BoxId and DateTimeReceived=@DateTimeReceived and BoxMsgInTypeId=@MsgType";
            string sql = "DELETE FROM vlfMsgIn where boxid=@BoxId and OriginDateTime=@OriginDateTime and BoxMsgInTypeId=@MsgType";
            sqlExec.ClearCommandParameters();
            // Add parameters to SQL statement
            sqlExec.AddCommandParam("@OriginDateTime", SqlDbType.DateTime, cMFIn.originatedDateTime);
//            sqlExec.AddCommandParam("@DateTimeReceived", SqlDbType.BigInt, cMFIn.receivedDateTime.Ticks);
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, cMFIn.boxID);
            sqlExec.AddCommandParam("@MsgType", SqlDbType.SmallInt, cMFIn.messageTypeID);
#endif
            sql = string.Format("DELETE FROM vlfMsgIn where boxid={0} and OriginDateTime='{1}' and BoxMsgInTypeId={2}", 
               cMFIn.boxID, 
               (cMFIn is CMFInEx ? 
                  ((CMFInEx)cMFIn).realOriginDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") :
                  cMFIn.originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff")), cMFIn.messageTypeID);
#if TESTING
            Util.BTrace(Util.INF0, sql);
#endif
            sqlExec.ClearCommandParameters();
            //Executes SQL statement
             rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

            if (rowsAffected > 1)
            {
               Util.BTrace(Util.INF0, "DeleteMsg -> found {0} messages with boxid={1} OriginDateTime={2}",
                  rowsAffected, cMFIn.boxID, cMFIn.originatedDateTime);
            }
            else 
               if (rowsAffected == 0)
               {
                
                   //Try delete from tmp table
                   sql = string.Format("DELETE FROM vlfMsgIn_Tmp where boxid={0} and OriginDateTime='{1}' and BoxMsgInTypeId={2}",
                    cMFIn.boxID,
                        (cMFIn is CMFInEx ?
                            ((CMFInEx)cMFIn).realOriginDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") :
                        cMFIn.originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff")), cMFIn.messageTypeID);

                  sqlExec.ClearCommandParameters();
                  //Executes SQL statement
                  rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

                  if (rowsAffected == 0)
                  {
                      string prefixMsg = string.Format("Unable to delete msgIn: recvTime={0}, origTime={1}, boxId={2}, dclId={3}, msgId={4}, protId={5}", cMFIn.receivedDateTime, cMFIn.originatedDateTime, cMFIn.boxID, cMFIn.dclID, cMFIn.messageTypeID, cMFIn.protocolTypeID);
                      throw new DASAppDataAlreadyExistsException(prefixMsg + " The message doesn't exist.");
                  }

               }
         }
         catch (SqlException objException)
         {
            //string prefixMsg = "Unable to add new msg In with date '" + Convert.ToDateTime(cMFIn.receivedDateTime).ToString() + "'.";
            string prefixMsg = string.Format("Unable to delete msgIn: recvTime={0}, origTime={1}, boxId={2}, dclId={3}, msgId={4}, protId={5}", cMFIn.receivedDateTime, cMFIn.originatedDateTime, cMFIn.boxID, cMFIn.dclID, cMFIn.messageTypeID, cMFIn.protocolTypeID);
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = string.Format("Unable to delete msgIn: recvTime={0}, origTime={1}, boxId={2}, dclId={3}, msgId={4}, protId={5}", cMFIn.receivedDateTime, cMFIn.originatedDateTime, cMFIn.boxID, cMFIn.dclID, cMFIn.messageTypeID, cMFIn.protocolTypeID);
            throw new DASException(prefixMsg + " " + objException.Message);
         }

      }

      public void DeleteMsg_TmpSLS(CMFIn cMFIn)
      {
          try
          {
#if false
//            string sql = "DELETE FROM vlfMsgIn where boxid=@BoxId and DateTimeReceived=@DateTimeReceived and BoxMsgInTypeId=@MsgType";
            string sql = "DELETE FROM vlfMsgIn where boxid=@BoxId and OriginDateTime=@OriginDateTime and BoxMsgInTypeId=@MsgType";
            sqlExec.ClearCommandParameters();
            // Add parameters to SQL statement
            sqlExec.AddCommandParam("@OriginDateTime", SqlDbType.DateTime, cMFIn.originatedDateTime);
//            sqlExec.AddCommandParam("@DateTimeReceived", SqlDbType.BigInt, cMFIn.receivedDateTime.Ticks);
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, cMFIn.boxID);
            sqlExec.AddCommandParam("@MsgType", SqlDbType.SmallInt, cMFIn.messageTypeID);
#endif
              string sql = string.Format("DELETE FROM vlfMsgIn_TmpForNewSLS where boxid={0} and OriginDateTime='{1}' and BoxMsgInTypeId={2}",
                 cMFIn.boxID,
                 (cMFIn is CMFInEx ?
                    ((CMFInEx)cMFIn).realOriginDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") :
                    cMFIn.originatedDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff")), cMFIn.messageTypeID);
#if TESTING
            Util.BTrace(Util.INF0, sql);
#endif
              sqlExec.ClearCommandParameters();
              //Executes SQL statement
              int rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

              if (rowsAffected > 1)
              {
                  Util.BTrace(Util.INF0, "DeleteMsg_TmpSLS -> found {0} messages with boxid={1} OriginDateTime={2}",
                     rowsAffected, cMFIn.boxID, cMFIn.originatedDateTime);
              }
              else
                  if (rowsAffected == 0)
                  {
                      string prefixMsg = string.Format("Unable to delete vlfMsgIn_TmpForNewSLS: recvTime={0}, origTime={1}, boxId={2}, dclId={3}, msgId={4}, protId={5}", cMFIn.receivedDateTime, cMFIn.originatedDateTime, cMFIn.boxID, cMFIn.dclID, cMFIn.messageTypeID, cMFIn.protocolTypeID);
                      throw new DASAppDataAlreadyExistsException(prefixMsg + " The message doesn't exist.");
                  }
          }
          catch (SqlException objException)
          {
              //string prefixMsg = "Unable to add new msg In with date '" + Convert.ToDateTime(cMFIn.receivedDateTime).ToString() + "'.";
              string prefixMsg = string.Format("Unable to delete vlfMsgIn_TmpForNewSLS: recvTime={0}, origTime={1}, boxId={2}, dclId={3}, msgId={4}, protId={5}", cMFIn.receivedDateTime, cMFIn.originatedDateTime, cMFIn.boxID, cMFIn.dclID, cMFIn.messageTypeID, cMFIn.protocolTypeID);
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = string.Format("Unable to delete vlfMsgIn_TmpForNewSLS: recvTime={0}, origTime={1}, boxId={2}, dclId={3}, msgId={4}, protId={5}", cMFIn.receivedDateTime, cMFIn.originatedDateTime, cMFIn.boxID, cMFIn.dclID, cMFIn.messageTypeID, cMFIn.protocolTypeID);
              throw new DASException(prefixMsg + " " + objException.Message);
          }

      }


      public void AppendMsg(CMFIn cMFIn, bool inHistory)
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
               ",CommModeId" +
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
               ",Priority) VALUES ( @DateTimeReceived,@BoxId,@DclId,@CommModeId,@BoxMsgInTypeId," +
               "@BoxProtocolTypeId,@OriginDateTime,@CommInfo1,@CommInfo2,@ValidGps," +
               "@Latitude,@Longitude,@Speed,@Heading,@SensorMask,@CustomProp," +
               "@BlobData,@BlobDataSize,@SequenceNum,@IsArmed,@Priority ) ";

            // Set SQL command
            sqlExec.ClearCommandParameters();
            // Add parameters to SQL statement
            if (inHistory)
               sqlExec.AddCommandParam("@DateTimeReceived", SqlDbType.DateTime, cMFIn.receivedDateTime);
            else
               sqlExec.AddCommandParam("@DateTimeReceived", SqlDbType.BigInt, Convert.ToInt64(Convert.ToDateTime(cMFIn.receivedDateTime).Ticks));

            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, cMFIn.boxID);
            sqlExec.AddCommandParam("@DclId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.dclID));
            sqlExec.AddCommandParam("@CommModeId", SqlDbType.SmallInt, cMFIn.commMode);
            sqlExec.AddCommandParam("@BoxMsgInTypeId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.messageTypeID));
            sqlExec.AddCommandParam("@BoxProtocolTypeId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.protocolTypeID));
            sqlExec.AddCommandParam("@OriginDateTime", SqlDbType.DateTime, cMFIn.originatedDateTime);
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

/*
             sqlExec.ClearCommandParameters();
             //Prepares SQL statement
             sqlExec.AddCommandParam("@DateTimeReceived", SqlDbType.BigInt, Convert.ToInt64(Convert.ToDateTime(cMFIn.receivedDateTime).Ticks));
             sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, cMFIn.boxID);
             sqlExec.AddCommandParam("@DclId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.dclID));
             sqlExec.AddCommandParam("@CommModeId", SqlDbType.SmallInt, cMFIn.commMode);
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
             sqlExec.AddCommandParam("@Priority", SqlDbType.TinyInt, cMFIn.priority);             //Executes SQL statement
             rowsAffected = sqlExec.SPExecuteNonQuery("sp_InsertInMsgIn");      
*/
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
      /// Add new Msg into MsgInHist table.
      /// </summary>
      /// 
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
               ",CommModeId" +
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
               ",Priority) VALUES ( @DateTimeReceived,@BoxId,@DclId,@CommModeId,@BoxMsgInTypeId," +
               "@BoxProtocolTypeId,@OriginDateTime,@CommInfo1,@CommInfo2,@ValidGps," +
               "@Latitude,@Longitude,@Speed,@Heading,@SensorMask,@CustomProp," +
               "@BlobData,@BlobDataSize,@SequenceNum,@IsArmed,@Priority ) ";

            // Set SQL command
            sqlExec.ClearCommandParameters();
            // Add parameters to SQL statement
            sqlExec.AddCommandParam("@DateTimeReceived", SqlDbType.DateTime, cMFIn.receivedDateTime);
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, cMFIn.boxID);
            sqlExec.AddCommandParam("@DclId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.dclID));
            sqlExec.AddCommandParam("@CommModeId", SqlDbType.SmallInt, cMFIn.commMode);
            sqlExec.AddCommandParam("@BoxMsgInTypeId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.messageTypeID));
            sqlExec.AddCommandParam("@BoxProtocolTypeId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.protocolTypeID));
            sqlExec.AddCommandParam("@OriginDateTime", SqlDbType.DateTime, cMFIn.originatedDateTime);
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
 
/*
             sqlExec.ClearCommandParameters();
             //Prepares SQL statement
             sqlExec.AddCommandParam("@DateTimeReceived", SqlDbType.BigInt, Convert.ToInt64(Convert.ToDateTime(cMFIn.receivedDateTime).Ticks));
             sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, cMFIn.boxID);
             sqlExec.AddCommandParam("@DclId", SqlDbType.SmallInt, Convert.ToInt16(cMFIn.dclID));
             sqlExec.AddCommandParam("@CommModeId", SqlDbType.SmallInt, cMFIn.commMode);
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
             sqlExec.AddCommandParam("@Priority", SqlDbType.TinyInt, cMFIn.priority);             //Executes SQL statement
             rowsAffected = sqlExec.SPExecuteNonQuery("sp_InsertInMsgIn");      
*/  
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
      #endregion

      /// <summary>
      ///         save in vlfMsgInHst the messages received from MDTs
      /// </summary>
      /// <param name="myTableName"></param>
      /// <param name="dtOriginated"></param>
      /// <param name="boxID"></param>
      /// <param name="dclID"></param>
      /// <param name="commModeId"></param>
      /// <param name="boxProtocolId"></param>
      /// <param name="customProperties"></param>
      /// <throws>
      ///      DASDbConnectionClosed, DASException, DASAppDataAlreadyExistsException
      /// </throws>
      public void AppendThirdPartyMessage(DateTime dtOriginated, int boxID, 
                                          double latitude, double longitude, int seqNum,
                                          short boxProtocolId, string customProperties)
      {
         int rowsAffected = 0;
        
         try
         {
            // Construct SQL for migration to the MsgIn data target. 
            string sql = "INSERT INTO vlfMsgInHst ( " +
             "DateTimeReceived, BoxId, Latitude, Longitude, SequenceNum, BoxMsgInTypeId, BoxProtocolTypeId, OriginDateTime" +
             ",CustomProp,IsArmed, DclId) VALUES ( @DateTimeReceived, @BoxId, @Latitude, @Longitude, @SeqNum,"+
             "@BoxMsgInTypeId, @BoxProtocolTypeId, @OriginDateTime, @CustomProp, @IsArmed, @DclId)";

            // Set SQL command
            sqlExec.ClearCommandParameters();
            // Add parameters to SQL statement
            sqlExec.AddCommandParam("@DateTimeReceived", SqlDbType.DateTime, DateTime.Now);
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxID);
            sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, latitude);
            sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, longitude);
            sqlExec.AddCommandParam("@SeqNum", SqlDbType.Int, seqNum);      
            sqlExec.AddCommandParam("@BoxMsgInTypeId", SqlDbType.SmallInt, Convert.ToInt16(Enums.MessageType.ThirdPartyPacket));
            sqlExec.AddCommandParam("@BoxProtocolTypeId", SqlDbType.SmallInt, boxProtocolId);
            sqlExec.AddCommandParam("@OriginDateTime", SqlDbType.DateTime, dtOriginated);
            sqlExec.AddCommandParam("@IsArmed", SqlDbType.TinyInt , 0);
            sqlExec.AddCommandParam("@DclId", SqlDbType.SmallInt, 0);
            if (customProperties == null)
               sqlExec.AddCommandParam("@CustomProp", SqlDbType.Char, System.DBNull.Value);
            else
               sqlExec.AddCommandParam("@CustomProp", SqlDbType.Char, customProperties);
            //Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            //string prefixMsg = "Unable to add new msg In with date '" + Convert.ToDateTime(cMFIn.receivedDateTime).ToString() + "'.";
            string prefixMsg = string.Format("AppendThirdPartyMessage -> Unable to add new msg to msgIn: recvTime={0}, origTime={1}, boxId={2}, msgId={3}, protId={4}",
                     DateTime.Now, dtOriginated, boxID, Enums.MessageType.ThirdPartyPacket, boxProtocolId);
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = string.Format("AppendThirdPartyMessage -> Unable to add new msg to msgIn: recvTime={0}, origTime={1}, boxId={2}, msgId={3}, protId={4}",
                     DateTime.Now, dtOriginated, boxID, Enums.MessageType.ThirdPartyPacket, boxProtocolId);
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            string prefixMsg = string.Format("AppendThirdPartyMessage -> Unable to add new msg to msgIn: recvTime={0}, origTime={1}, boxId={2}, msgId={3}, protId={4}",
                     DateTime.Now, dtOriginated, boxID, Enums.MessageType.ThirdPartyPacket, boxProtocolId);
            throw new DASAppDataAlreadyExistsException(prefixMsg + " The message already exists.");
         }
      }


      /** \brief  the message is only to add simulated messages to the history through a web service call
       *  \ret    0 - for success
       *          1 - for duplicate message
       *          2 - box is not present in the system
       *          3 - invalid parameters (more than 20 chars for VINNUM, lat, long invalid or dtOriginated
       */
      public int AppendFakeGPSMessage(DateTime dtOriginated,
                                      string VINNumber, 
                                      double latitude,
                                      double longitude)
      {

         int res = 0;

         if ( string.IsNullOrEmpty(VINNumber) ||
              VINNumber.Length > 20           || 
              (-90 > latitude)  || 
              (90 < latitude)   || 
              (-180 > longitude)|| 
              (180 < longitude) ||
               dtOriginated < dtReferenceLow ||
               dtOriginated > dtReferenceHigh)
            return -3 ;

         try
         {
             ;
            string sql = "sp_AppendFakeGPSMessage";
            // Set SQL command
            sqlExec.ClearCommandParameters();
            // Add parameters to SQL statement
            sqlExec.AddCommandParam("@VinNum", SqlDbType.VarChar, VINNumber);
            sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, latitude);
            sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, longitude);
            sqlExec.AddCommandParam("@OriginDateTime", SqlDbType.DateTime, dtOriginated);
            sqlExec.AddCommandParam("@Res", SqlDbType.Int, ParameterDirection.Output, res);  
            //Executes SQL statement
            sqlExec.SPExecuteNonQuery(sql);

            res = (DBNull.Value == sqlExec.ReadCommandParam("@Res")) ?
                                        0 : Convert.ToInt32(sqlExec.ReadCommandParam("@Res").ToString());
         }
         catch (SqlException objException)
         {
            //string prefixMsg = "Unable to add new msg In with date '" + Convert.ToDateTime(cMFIn.receivedDateTime).ToString() + "'.";
            string prefixMsg = string.Format("AppendFakeGPSMessage -> Unable to add new msg to msgIn: recvTime={0}, origTime={1}, VIN#={2}",
                     DateTime.Now, dtOriginated, VINNumber);
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = string.Format("AppendFakeGPSMessage -> Unable to add new msg to msgIn: recvTime={0}, origTime={1}, VIN#={2}",
                     DateTime.Now, dtOriginated, VINNumber);
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         return res;
      }

      /** \fn     public DataSet RunDynamicSQL(string query, string condition)
       *  \brief  
       * 
       */
      public DataSet RunDynamicSQL(string query, string condition)
      {
         DataSet ds = null;
         try
         {
            Util.BTrace(Util.INF0, "RunDynamicSQL -> {0} {1}", query, condition);
            sqlExec.ClearCommandParameters();
            ds = sqlExec.SQLExecuteDataset(query + " " + condition);
         }
         catch (SqlException objException)
         {
            string prefixMsg = string.Format("RunDynamicSQL -> SqlException {0}", objException.Message);
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = string.Format("RunDynamicSQL -> Exception {0}", objException.Message);
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         return ds;
      }

      public DataSet RunDynamicSQL(string query, string condition, int timeout)
      {
         DataSet ds = null;
         try
         {
            Util.BTrace(Util.INF0, "RunDynamicSQL -> {0} {1}", query, condition);
            sqlExec.ClearCommandParameters();
            sqlExec.CommandTimeout = timeout;
            ds = sqlExec.SQLExecuteDataset(query + " " + condition);
         }
         catch (SqlException objException)
         {
            string prefixMsg = string.Format("RunDynamicSQL -> SqlException {0}", objException.Message);
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = string.Format("RunDynamicSQL -> Exception {0}", objException.Message);
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         return ds;
      }

      /// <summary>
      ///         used for insert to see if it's any difference in speed
      /// </summary>
      /// <param name="query"></param>
      public int RunDynamicSQLNonQuery(string query, string condition)
      {
         int rowsAffected = 0;
         try
         {
            Util.BTrace(Util.INF0, "RunDynamicSQLNonQuery -> {0} {1}", query, condition);
            sqlExec.ClearCommandParameters();
            rowsAffected = sqlExec.SQLExecuteNonQuery(query + " " + condition);
         }
         catch (SqlException objException)
         {
            string prefixMsg = string.Format("RunDynamicSQLNonQuery -> SqlException {0}", objException.Message);
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = string.Format("RunDynamicSQLNonQuery -> Exception {0}", objException.Message);
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         return rowsAffected;
      }

      /**  \fn       public void UpdateAddress(int boxId, DateTime origin, string address)
       *   \brief    just to update the address 
       *   \comment  I should use a store procedure
       */     
      public void UpdateStreetAddressInHistory(int boxId, 
                                               DateTime originDateTime, 
                                               string streetAddress, 
                                               int cmdTimeOut, 
                                               string nearestLandmark)
      {
         int rowsAffected = 0;
         int currCmdTimeOut = sqlExec.CommandTimeout;
         string sql = "";
         try
         {
/*
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
*/
            // Set SQL command
            sqlExec.ClearCommandParameters();
            sql = "UPDATE vlfMsgInHst SET StreetAddress=@StreetAddress " +
                  ((nearestLandmark != null && nearestLandmark.Trim() != "") ? ",NearestLandmark=@NearestLandmark " : "") +
                  "WHERE BoxId=@BoxId " +
                  "AND OriginDateTime between @OriginDateTime and @OriginDateTime2";
            sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, streetAddress);
            if (nearestLandmark != null && nearestLandmark.Trim() != "") 
               sqlExec.AddCommandParam("@NearestLandmark", SqlDbType.Char,nearestLandmark.Trim() ) ;
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
            sqlExec.AddCommandParam("@OriginDateTime", SqlDbType.DateTime, originDateTime.AddSeconds(-1.0));
            sqlExec.AddCommandParam("@OriginDateTime2", SqlDbType.DateTime, originDateTime.AddSeconds(1.0));
                         
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

      /**  \fn       public void UpdateStreetAddressInHistory(...)
       *   \brief    just to update the address 
       *   \comment  the problem is that you have to update everything around that area within 
       *             that second, because the SQLDateTime has a different granularity than DateTime !!!
       *             hence the condition with between 
       */
      public void UpdateStreetAddressInHistory(int boxId,
                                               DateTime originDateTime,
                                               DateTime receivedDateTime,
                                               string streetAddress,
                                               int cmdTimeOut,
                                               string nearestLandmark)
      {
         int rowsAffected = 0;
         int currCmdTimeOut = sqlExec.CommandTimeout;
         string sql = "";
         string prefixMsg = string.Format("UpdateStreetAddressInHistory: Unable to update record Boxid={0} OriginDateTime={1} ReceivedDateTime={2}",
                     boxId, originDateTime, receivedDateTime);

         try
         {
/*
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
*/
            // Set SQL command
            sqlExec.ClearCommandParameters();
            sql = "UPDATE vlfMsgInHst SET StreetAddress=@StreetAddress " +
                  ((nearestLandmark != null && nearestLandmark.Trim() != "") ? ",NearestLandmark=@NearestLandmark " : "") +
                  "WHERE BoxId=@BoxId " +
                  "AND OriginDateTime between @DateTimeReceived and @DateTimeReceived2";
            sqlExec.AddCommandParam("@StreetAddress", SqlDbType.Char, streetAddress);
            if (nearestLandmark != null && nearestLandmark.Trim() != "")
               sqlExec.AddCommandParam("@NearestLandmark", SqlDbType.Char, nearestLandmark.Trim());
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
            sqlExec.AddCommandParam("@DateTimeReceived", SqlDbType.DateTime, originDateTime.AddSeconds(-1));
            sqlExec.AddCommandParam("@DateTimeReceived2", SqlDbType.DateTime, originDateTime.AddSeconds(1));

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
         finally
         {
            sqlExec.CommandTimeout = currCmdTimeOut;
         }
         if (rowsAffected == 0)
         {
            throw new DASAppDataAlreadyExistsException(prefixMsg + "0 records updated");
         }
      }



      /**  \fn       public void UpdateCustomPropInHistory(int boxId, DateTime origin, string customPropertyAddons)
    *   \brief    Added additional info to custom property (SMC read message info for example)
    */


      public void UpdateCustomPropInHistory(int boxId,
                                               DateTime originDateTime,
                                               Int16 boxMsgInTypeId,
                                               string customPropertyAddons,int cmdTimeOut)
      {
         int rowsAffected = 0;
         int currCmdTimeOut = sqlExec.CommandTimeout;
         string sql = "";
         try
         {
        
            // Set SQL command
            sqlExec.ClearCommandParameters();
            sql = "UPDATE vlfMsgInHst SET CustomProp=CustomProp+'|'+@CustomPropertyAddons "+
                  "WHERE BoxId=@BoxId " +
                  "AND OriginDateTime between @OriginDateTime and @OriginDateTime2 "+
                  "AND BoxMsgInTypeId=@BoxMsgInTypeId";
            sqlExec.AddCommandParam("@CustomPropertyAddons", SqlDbType.VarChar , customPropertyAddons);
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
            sqlExec.AddCommandParam("@OriginDateTime", SqlDbType.DateTime, originDateTime);
            sqlExec.AddCommandParam("@OriginDateTime2", SqlDbType.DateTime, originDateTime.AddSeconds(1.0));
            sqlExec.AddCommandParam("@BoxMsgInTypeId", SqlDbType.Int, boxMsgInTypeId);

            sqlExec.CommandTimeout = cmdTimeOut;
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);


            Util.BTrace(Util.INF0, "UpdateCustomPropInHistory -> boxId:{0} origin:{1} msgType:{2} cp:{3} rowsAffected:{4}", boxId, originDateTime, boxMsgInTypeId, customPropertyAddons, rowsAffected);

         }
         catch (SqlException objException)
         {
            string prefixMsg = "Custom property update: Unable to custom property record with Boxid " + boxId + " and OriginDateTime " + originDateTime + ".";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Custom property update: Unable to custom property record with Boxid " + boxId + " and OriginDateTime " + originDateTime + ".";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         finally
         {
            sqlExec.CommandTimeout = currCmdTimeOut;
         }
      }
   }
}