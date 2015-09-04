using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;			// for CMFOut

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfMsgOut table.
	/// </summary>
	public class MsgOut: TblGenInterfaces
   {
#if BULK_SUPPORT
      static StringBuilder    strNonQueryStatements ;       ///< add operations to this string
      static System.Threading.Timer tmrExecutor ;           ///< the timer is coming every 30 seconds and send the messages to the server
      unsigned int            requests ;                    ///< how many updates/inserts are in strNonQueryStatements
#endif

      private const string prefixNextMsg = "Unable to retrieve next message. ";
      private char[] ONLY_BLANK = new char[] { ' ' };
		
		#region Public Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public MsgOut(SQLExecuter sqlExec) : base ("vlfMsgOut",sqlExec)
		{
		}

		/// <summary>
		/// Add new Msg.
		/// </summary>
		/// <param name="cMFOut"></param>
		/// <param name="priority"></param>
		/// <param name="dclId"></param>
		/// <param name="aslId"></param>
		/// <param name="userId"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists (after number of attemps).</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddMsg(CMFOut cMFOut,SByte priority,short dclId,short aslId,int userId)
		{
			Int64 dateTime = cMFOut.timeSent.Ticks;
			int currRetries = 0;
			while(currRetries < Const.violatedIntegrityMaxRetries)
			{
				try
				{
					AppendMsg(cMFOut,dateTime,priority,dclId,aslId,userId);
					currRetries = Const.violatedIntegrityMaxRetries;
				}
				catch(DASAppViolatedIntegrityConstraintsException e)
				{
					DateTime tmpDateTime = new DateTime(dateTime);
					++currRetries;
					if(currRetries < Const.violatedIntegrityMaxRetries)
               {
                  Util.BTrace(Util.INF1, " MsgOut.AddMsg -> DASAppViolatedIntegrityConstraintsException {0} times", currRetries);
						dateTime = tmpDateTime.AddMilliseconds(Const.nextDateTimeMillisecInterval).Ticks;
					}
					else
					{
						string text = "Information with '" + tmpDateTime + "' already exists in MsgOut table. Unable to add new data into MsgIn table. Maximal number of retries " + 
										currRetries + " has been reached. ";
						throw new DASAppDataAlreadyExistsException(text + e.Message);
					}
				}
			}
		}

		/// <summary>
		/// Retrieves record count of "vlfMsgOut" table
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


      /// <summary>
		/// Returns next message (older one) in CMFOut format or null if result hasn't been found. 	
		/// Add new record to MsgOutHst table.
		/// If record alredy exists, try to add new one with DateTime + Const.nextDateTimeMillisecInterval
		/// Deletes old data by DateTime field.
		/// </summary>
		/// <returns>
		/// CMFOut,In case of error or after max retries (Const.violatedIntegrityMaxRetries) throws DASAppViolatedIntegrityConstraintsException exception.
		/// </returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DrawNextCmfMsg(short dclId, ref int cnt, out CMFOut[] arrCMFOut)
		{
         DataSet sqlDataSet = RetrievesNextMsg(dclId, cnt, prefixNextMsg);
         // Retrieves first row from SQL result.
         if ((sqlDataSet != null) && (sqlDataSet.Tables[0].Rows.Count > 0))
         {
            cnt = sqlDataSet.Tables[0].Rows.Count;
            arrCMFOut = new CMFOut[cnt];
            int idx = 0;
            foreach (DataRow currRow in sqlDataSet.Tables[0].Rows)
            {
               try
               {
                  // 1. Fill result structure in CMF format.
                  arrCMFOut[idx] = new CMFOut();
                  arrCMFOut[idx].boxID = Convert.ToInt32(currRow["BoxId"]);
                  arrCMFOut[idx].commandTypeID = Convert.ToInt16(currRow["BoxCmdOutTypeId"]);
                  arrCMFOut[idx].commInfo1 = Convert.ToString(currRow["CommInfo1"]).TrimEnd();
                  arrCMFOut[idx].commInfo2 = Convert.ToString(currRow["CommInfo2"]).TrimEnd();
                  arrCMFOut[idx].customProperties = Convert.ToString(currRow["CustomProp"]).TrimEnd(ONLY_BLANK);
                  arrCMFOut[idx].protocolTypeID = Convert.ToInt16(currRow["BoxProtocolTypeId"]);
                  arrCMFOut[idx].sequenceNum = Convert.ToInt32(currRow["SequenceNum"]);
                  arrCMFOut[idx].ack = Convert.ToString(currRow["Acknowledged"]).TrimEnd();
                  arrCMFOut[idx].scheduled = Convert.ToBoolean(currRow["Scheduled"]);

                  // 2. Try to backup exist message (Add new record to MsgOutHst table)
                  // Const.violatedIntegrityMaxRetries times.
                  int currRetries = 0;
                  DateTime dateTime = new DateTime(Convert.ToInt64(currRow["DateTime"]));
                  while (currRetries < Const.violatedIntegrityMaxRetries)
                  {
                     try
                     {
                        AddToHistory(arrCMFOut[idx],
                           dateTime,
                           Convert.ToSByte(currRow["Priority"]),
                           Convert.ToInt16(currRow["DclId"]),
                           Convert.ToInt16(currRow["AslId"]),
                           Convert.ToInt16(currRow["UserId"]));
                        // On success, exit
                        currRetries = Const.violatedIntegrityMaxRetries;
                     }
                     catch (DASAppViolatedIntegrityConstraintsException e)
                     {
                        ++currRetries;
                        if (currRetries < Const.violatedIntegrityMaxRetries)
                        {
                           dateTime = dateTime.AddMilliseconds(Const.nextDateTimeMillisecInterval);
                        }
                        else
                        {
                           string text = "Information with '" + dateTime +
                              "' already exists in MsgIn table. Unable to add new data into MsgOut table. Maximal number of retries "
                              + currRetries + " has been reached. ";
                           throw new DASAppDataAlreadyExistsException(text + e.Message);
                        }
                     }
                  }

                  ++idx;
               }
               //catch(DASException objException)
               //{
               //	throw new DASException(prefixMsg + " " + objException.Message);
               //}
               finally
               {
                  // 3. Deletes current record by DateTimeReceived and box id fields from MsgOut table.
                  DeleteMsg(Convert.ToInt64(currRow["DateTime"]), Convert.ToInt32(currRow["BoxId"]));

               }

            } // end foreach

            return cnt;
         }
         else
            cnt = 0;

         arrCMFOut = null;
         return 0;
      }

		/// <summary>
		/// Returns next message (older one) in CMFOut format or null if result hasn't been found. 	
		/// Add new record to MsgOutHst table.
		/// If record alredy exists, try to add new one with DateTime + Const.nextDateTimeMillisecInterval
		/// Deletes old data by DateTime field.
		/// </summary>
		/// <returns>
		/// CMFOut,In case of error or after max retries (Const.violatedIntegrityMaxRetries) throws DASAppViolatedIntegrityConstraintsException exception.
		/// </returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public CMFOut DrawNextCmfMsg(short dclId)
		{
			CMFOut cMFOut = null;
			// Retrieves next message
			DataSet sqlDataSet = RetrievesNextMsg(dclId,1,prefixNextMsg);
			
			// Retrieves first row from SQL result.
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					try
					{
						// 1. Fill result structure in CMF format.
						cMFOut = new CMFOut();
						cMFOut.boxID = Convert.ToInt32(currRow["BoxId"]);
						cMFOut.commandTypeID = Convert.ToInt16(currRow["BoxCmdOutTypeId"]);
						cMFOut.commInfo1 = Convert.ToString(currRow["CommInfo1"]).TrimEnd();
						cMFOut.commInfo2 = Convert.ToString(currRow["CommInfo2"]).TrimEnd();
						cMFOut.customProperties = Convert.ToString(currRow["CustomProp"]).TrimEnd();
						cMFOut.protocolTypeID = Convert.ToInt16(currRow["BoxProtocolTypeId"]);
						cMFOut.sequenceNum = Convert.ToInt32(currRow["SequenceNum"]);
						cMFOut.ack = Convert.ToString(currRow["Acknowledged"]).TrimEnd();
						cMFOut.scheduled = Convert.ToBoolean(currRow["Scheduled"]);
												
						// 2. Try to backup exist message (Add new record to MsgOutHst table)
						// Const.violatedIntegrityMaxRetries times.
						int currRetries = 0;
						DateTime dateTime = new DateTime(Convert.ToInt64(currRow["DateTime"]));
						while(currRetries < Const.violatedIntegrityMaxRetries)
						{
							try
							{
								AddToHistory(cMFOut,
									dateTime,
									Convert.ToSByte(currRow["Priority"]),
									Convert.ToInt16(currRow["DclId"]),
									Convert.ToInt16(currRow["AslId"]),
									Convert.ToInt16(currRow["UserId"]));
								// On success, exit
								currRetries = Const.violatedIntegrityMaxRetries;
							}
							catch(DASAppViolatedIntegrityConstraintsException e)
							{
								++currRetries;
								if(currRetries < Const.violatedIntegrityMaxRetries)
								{
									dateTime = dateTime.AddMilliseconds(Const.nextDateTimeMillisecInterval);
								}
								else
								{
									string text = "Information with '" + dateTime + 
										"' already exists in MsgIn table. Unable to add new data into MsgOut table. Maximal number of retries " 
										+ currRetries + " has been reached. ";
									throw new DASAppDataAlreadyExistsException(text + e.Message);
								}
							}
						}
					}
					//catch(DASException objException)
					//{
					//	throw new DASException(prefixMsg + " " + objException.Message);
					//}
					finally
					{
						// 3. Deletes current record by DateTimeReceived and box id fields from MsgOut table.
						DeleteMsg(Convert.ToInt64(currRow["DateTime"]), Convert.ToInt32(currRow["BoxId"]));

					}
					// 4. Retrieves only first row.
					break;
				}
			}
			return cMFOut;
		}
		/// <summary>
		/// Returns next message (older one) in CMFOut format or null if result hasn't been found. 	
		/// Add new record to MsgOutHst table.
		/// If record alredy exists, try to add new one with DateTime + Const.nextDateTimeMillisecInterval
		/// Deletes old data by DateTime field.
		/// </summary>
		/// <remarks>
		/// In case of error or after max retries (Const.violatedIntegrityMaxRetries)
		/// throws DASAppViolatedIntegrityConstraintsException exception.
		/// </remarks>
		/// <returns>CMFOut</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public CMFOut DrawNextCmfMsg(short dclId,DateTime currTime)
		{
			CMFOut cMFOut = null;
			// Retrieves next message
			DataSet sqlDataSet = RetrievesNextMsg(dclId,currTime,prefixNextMsg);
			
			// Retrieves first row from SQL result.
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					try
					{
						// 1. Fill result structure in CMF format.
						cMFOut = new CMFOut();
						cMFOut.boxID = Convert.ToInt32(currRow["BoxId"]);
						cMFOut.commandTypeID = Convert.ToInt16(currRow["BoxCmdOutTypeId"]);
						cMFOut.commInfo1 = Convert.ToString(currRow["CommInfo1"]).TrimEnd();
						cMFOut.commInfo2 = Convert.ToString(currRow["CommInfo2"]).TrimEnd();
						cMFOut.customProperties = Convert.ToString(currRow["CustomProp"]).TrimEnd();
						cMFOut.protocolTypeID = Convert.ToInt16(currRow["BoxProtocolTypeId"]);
						cMFOut.sequenceNum = Convert.ToInt32(currRow["SequenceNum"]);
						cMFOut.ack = Convert.ToString(currRow["Acknowledged"]).TrimEnd();
						cMFOut.scheduled = Convert.ToBoolean(currRow["Scheduled"]);
												
						// 2. Try to backup exist message (Add new record to MsgOutHst table)
						// Const.violatedIntegrityMaxRetries times.
						int currRetries = 0;
						DateTime dateTime = new DateTime(Convert.ToInt64(currRow["DateTime"]));
						while(currRetries < Const.violatedIntegrityMaxRetries)
						{
							try
							{
								AddToHistory(cMFOut,
									          dateTime,
									          Convert.ToSByte(currRow["Priority"]),
									          Convert.ToInt16(currRow["DclId"]),
									          Convert.ToInt16(currRow["AslId"]),
									          Convert.ToInt16(currRow["UserId"]));
								// On success, exit
								currRetries = Const.violatedIntegrityMaxRetries;
							}
							catch(DASAppViolatedIntegrityConstraintsException e)
							{
								++currRetries;
								if(currRetries < Const.violatedIntegrityMaxRetries)
								{
									dateTime = dateTime.AddMilliseconds(Const.nextDateTimeMillisecInterval);
								}
								else
								{
									string text = "Information with '" + dateTime + 
										"' already exists in MsgIn table. Unable to add new data into MsgOut table. Maximal number of retries " 
										+ currRetries + " has been reached. ";
									throw new DASAppDataAlreadyExistsException(text + e.Message);
								}
							}
						}
					}
						//catch(DASException objException)
						//{
						//	throw new DASException(prefixMsg + " " + objException.Message);
						//}
					finally
					{
						// 3. Deletes current record by DateTimeReceived and box id fields from MsgOut table.
						DeleteMsg(Convert.ToInt64(currRow["DateTime"]),
							Convert.ToInt32(currRow["BoxId"]));

					}
					// 4. Retrieves only first row.
					break;
				}
			}
			return cMFOut;
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
		public int DeleteBoxAllMsgs(int boxId,string tableName,string where)
		{
			int rowsAffected = 0;
			try
			{
				// 1. Prepares SQL statement
				string sql = "DELETE FROM " + tableName + " WHERE BoxId=" + boxId + where;
				if(sqlExec.RequiredTransaction())
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
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete all box " + boxId + " messages.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		
		/// <summary>
		/// Update acknowledged fields 
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="dateTime"></param>
		/// <param name="ackVal"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetAck(int boxId,DateTime dateTime,string ackVal)
		{
			int rowsAffected = 0;
			string sql = "";
			string fromDT = dateTime.AddSeconds(-1).ToString("MM/dd/yyyy HH:mm:ss.fff");
			string toDT = dateTime.AddSeconds(1).ToString("MM/dd/yyyy HH:mm:ss.fff");
			try
			{
				// 1. Prepares SQL statement
				sql = "UPDATE vlfMsgOutHst SET Acknowledged='" + ackVal.Replace("'","''") + "'" +
					" WHERE BoxId=" + boxId +
					" AND (DateTime>='" + fromDT + "' AND DateTime<='" + toDT + "')";

				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

			}
			catch (SqlException objException) 
			{
				string prefixMsg = "SqlException: Unable to update ack field=" + ackVal + " for box id=" + boxId + " and DateTime from: " + fromDT + " to: " + toDT + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Exception: Unable to update ack field=" + ackVal + " for box id=" + boxId + " and DateTime from: " + fromDT + " to: " + toDT + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
		}
		/// <summary>
		/// Update acknowledged fields 
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="commandType"></param>
		/// <param name="ackVal"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetAck(int boxId,Enums.CommandType commandType,string ackVal)
		{
			int rowsAffected = 0;
			string sql = "";
			try
			{
				// 1. Prepares SQL statement
                sql = "DECLARE @BoxCmdOutTypeId int DECLARE @BoxId int DECLARE @DT datetime SET @BoxCmdOutTypeId=" + (short)commandType + " SET @BoxId =" + boxId + " SET @DT = (SELECT TOP 1 DateTime FROM vlfMsgOutHst  with(nolock) WHERE BoxId = @BoxId AND BoxCmdOutTypeId = @BoxCmdOutTypeId ORDER BY DateTime DESC) UPDATE vlfMsgOutHst SET Acknowledged = '" + ackVal + "' WHERE BoxId=@BoxId AND BoxCmdOutTypeId = @BoxCmdOutTypeId  AND DateTime >= DATEADD(ss,-1,@DT) AND DateTime <= DATEADD(ss,1,@DT)";

				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

			}
			catch (SqlException objException) 
			{
				string prefixMsg = "SqlException: Unable to update ack field=" + ackVal + " for box id=" + boxId + " and commandType=" + commandType.ToString() + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Exception: Unable to update ack field=" + ackVal + " for box id=" + boxId + " and commandType=" + commandType.ToString() + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
		}
		/// <summary>
		/// Check message in MsgOut table
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="commandType"></param>
		/// <returns>true if exists, otherwise returns false</returns>
		public bool CheckMessageInMsgOut(int boxId,Enums.CommandType commandType)
		{
			int rowsAffected = 0;
			bool retResult = false;
			try
			{
				// Executes SQL statement
                rowsAffected = (int)sqlExec.SQLExecuteScalar("SELECT COUNT(*) FROM vlfMsgOut with(nolock) WHERE BoxId = " + boxId + " AND BoxCmdOutTypeId = " + (short)commandType);
				if(rowsAffected > 0)
					retResult = true;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "SqlException: Unable to check if message exists in MsgOut table by boxId=" + boxId + " and commandType=" + commandType.ToString() + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "SqlException: Unable to check if message exists in MsgOut table by boxId=" + boxId + " and commandType=" + commandType.ToString() + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}
        // Changes for Timezone Feature start
        /// <summary>
        /// Retrieves message from history by box id and DateTime
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetDetailedMessageFromHistory_NewTZ(int userId, int boxId, DateTime dateTime)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = " DECLARE @Timezone float" +
                   " DECLARE @DayLightSaving int" +
                   " SELECT @Timezone=PreferenceValue FROM vlfUserPreference with(nolock) " +
                   " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZoneNew +
                   " IF @Timezone IS NULL SET @Timezone=0" +
                   " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference with(nolock)  WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving +
                   " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                   " SET @Timezone= @Timezone + @DayLightSaving" +

                   " SELECT vlfMsgOutHst.BoxId," +
                   "CASE WHEN vlfMsgOutHst.DateTime IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),vlfMsgOutHst.DateTime) END AS DateTime," +
                   "vlfBoxCmdOutType.BoxCmdOutTypeId AS MsgTypeId,RTRIM(BoxCmdOutTypeName) AS MsgTypeName," +
                   "vlfBoxProtocolType.BoxProtocolTypeId,RTRIM(BoxProtocolTypeName) AS BoxProtocolTypeName," +
                   "'" + Const.blankValue + "' AS ValidGps," +
                   "'" + Const.blankValue + "' AS Latitude," +
                   "'" + Const.blankValue + "' AS Longitude," +
                   "'" + Const.blankValue + "' AS Heading," +
                   "'" + Const.blankValue + "' AS SensorMask," +
                   "CASE WHEN vlfMsgOutHst.CustomProp IS NOT NULL then RTRIM(vlfMsgOutHst.CustomProp) END AS CustomProp," +
                   "'" + Const.blankValue + "' AS StreetAddress," +
                   "'" + Const.blankValue + "' AS Speed," +
                   "'N/A' AS BoxArmed," +
                   "CASE WHEN UserName IS NOT NULL then RTRIM(UserName) END AS UserName," +
                   "CASE WHEN FirstName IS NOT NULL then RTRIM(FirstName) END AS FirstName," +
                   "CASE WHEN LastName IS NOT NULL then RTRIM(LastName) END AS LastName" +
                   " FROM vlfMsgOutHst with (nolock),vlfBoxCmdOutType,vlfBoxProtocolType,vlfBox with (nolock),vlfUser,vlfPersonInfo  with(nolock) " +
                   " WHERE vlfMsgOutHst.BoxId=" + boxId +
                   " AND vlfMsgOutHst.DateTime='" + dateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                   " AND vlfMsgOutHst.BoxCmdOutTypeId = vlfBoxCmdOutType.BoxCmdOutTypeId" +
                   " AND vlfMsgOutHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId" +
                   " AND vlfMsgOutHst.BoxId=vlfBox.BoxId" +
                   " AND vlfMsgOutHst.UserId=vlfUser.UserId" +
                   " AND vlfUser.PersonId=vlfPersonInfo.PersonId";


                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve message detailed info from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve message detailed info from history" + objException.Message);
            }
            return resultDataSet;
        }

        // Changes for TimeZone Feature end
        /// <summary>
        /// Retrieves message from history by box id and DateTime
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetDetailedMessageFromHistory(int userId, int boxId, DateTime dateTime)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = " DECLARE @Timezone int" +
                    " DECLARE @DayLightSaving int" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference with(nolock) " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.TimeZone +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference with(nolock)  WHERE UserId=" + userId + " AND PreferenceId=" + (short)Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +

                    " SELECT vlfMsgOutHst.BoxId," +
                    "CASE WHEN vlfMsgOutHst.DateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfMsgOutHst.DateTime) END AS DateTime," +
                    "vlfBoxCmdOutType.BoxCmdOutTypeId AS MsgTypeId,RTRIM(BoxCmdOutTypeName) AS MsgTypeName," +
                    "vlfBoxProtocolType.BoxProtocolTypeId,RTRIM(BoxProtocolTypeName) AS BoxProtocolTypeName," +
                    "'" + Const.blankValue + "' AS ValidGps," +
                    "'" + Const.blankValue + "' AS Latitude," +
                    "'" + Const.blankValue + "' AS Longitude," +
                    "'" + Const.blankValue + "' AS Heading," +
                    "'" + Const.blankValue + "' AS SensorMask," +
                    "CASE WHEN vlfMsgOutHst.CustomProp IS NOT NULL then RTRIM(vlfMsgOutHst.CustomProp) END AS CustomProp," +
                    "'" + Const.blankValue + "' AS StreetAddress," +
                    "'" + Const.blankValue + "' AS Speed," +
                    "'N/A' AS BoxArmed," +
                    "CASE WHEN UserName IS NOT NULL then RTRIM(UserName) END AS UserName," +
                    "CASE WHEN FirstName IS NOT NULL then RTRIM(FirstName) END AS FirstName," +
                    "CASE WHEN LastName IS NOT NULL then RTRIM(LastName) END AS LastName" +
                    " FROM vlfMsgOutHst with (nolock),vlfBoxCmdOutType,vlfBoxProtocolType,vlfBox with (nolock),vlfUser,vlfPersonInfo  with(nolock) " +
                    " WHERE vlfMsgOutHst.BoxId=" + boxId +
                    " AND vlfMsgOutHst.DateTime='" + dateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
                    " AND vlfMsgOutHst.BoxCmdOutTypeId = vlfBoxCmdOutType.BoxCmdOutTypeId" +
                    " AND vlfMsgOutHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId" +
                    " AND vlfMsgOutHst.BoxId=vlfBox.BoxId" +
                    " AND vlfMsgOutHst.UserId=vlfUser.UserId" +
                    " AND vlfUser.PersonId=vlfPersonInfo.PersonId";

                // 2. Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retieve message detailed info from history", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retieve message detailed info from history" + objException.Message);
            }
            return resultDataSet;
        }
		/// <summary>
		/// Retrieves last [num of records] messages from the history
		/// </summary>
		/// <param name="numOfRecords"></param>
		/// <param name="boxId"></param>
		/// <param name="cmdType"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns>DataSet [DateTime],[BoxId],[UserId],[Priority],[DclId],[AslId],
		/// [BoxCmdOutTypeId],[BoxCmdOutTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
		/// [CommInfo1],[CommInfo2],[CustomProp],[SequenceNum],[Acknowledged]
		/// </returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetLastMessagesFromHistory(short numOfRecords,int boxId,short cmdType,DateTime from,DateTime to)
		{
			string sqlAddWhere = "";
			if(boxId != Const.unassignedIntValue)
			{
				sqlAddWhere += " AND vlfMsgOutHst.BoxId=" + boxId;
			}
			if(cmdType != Const.unassignedIntValue)
			{
				sqlAddWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId=" + cmdType;
			}
			if(from != Const.unassignedDateTime)
			{
				sqlAddWhere += " AND DateTime>='" + from + "'";
			}
			if(to != Const.unassignedDateTime)
			{
				sqlAddWhere += " AND DateTime<='" + to + "'";
			}
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT TOP " + numOfRecords + " DateTime,BoxId,UserId,Priority,DclId,AslId,"+
							"vlfMsgOutHst.BoxCmdOutTypeId,RTRIM(BoxCmdOutTypeName) AS BoxCmdOutTypeName,"+
							"vlfMsgOutHst.BoxProtocolTypeId,RTRIM(BoxProtocolTypeName) AS BoxProtocolTypeName,"+
							"CASE WHEN CommInfo1 IS NULL then '' ELSE RTRIM(CommInfo1) END AS CommInfo1,"+
							"CASE WHEN CommInfo2 IS NULL then '' ELSE RTRIM(CommInfo2) END AS CommInfo2,"+
							"CASE WHEN CustomProp IS NULL then '' ELSE RTRIM(CustomProp) END AS CustomProp,"+
							"SequenceNum,Acknowledged"+
							" into #tmpOut" +
                            " FROM vlfMsgOutHst with(nolock),vlfBoxProtocolType,vlfBoxCmdOutType  with(nolock) " +
							" WHERE vlfMsgOutHst.BoxProtocolTypeId=vlfBoxProtocolType.BoxProtocolTypeId"+
							" AND vlfMsgOutHst.BoxCmdOutTypeId=vlfBoxCmdOutType.BoxCmdOutTypeId"+
							sqlAddWhere +
							" SELECT * from #tmpOut" +
							" ORDER BY DateTime DESC" +
							" drop table #tmpOut";

				// 2. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve messages from history", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
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
      /// <param name="cmdType"></param>
      /// <param name="from"></param>
      /// <param name="to"></param>
      /// <returns>DataSet [DateTime],[BoxId],[UserId],[Priority],[DclId],[AslId],
      /// [BoxCmdOutTypeId],[BoxCmdOutTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
      /// [CommInfo1],[CommInfo2],[CustomProp],[SequenceNum],[Acknowledged]
      /// </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetLastMessagesFromHistoryByOrganization(short numOfRecords, int orgId,int fleetId,int boxId, short cmdType, DateTime from, DateTime to)
      {
         string sqlAddWhere = "";
         if (boxId != Const.unassignedIntValue)
         {
            sqlAddWhere += " AND vlfMsgOutHst.BoxId=" + boxId;
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

         if (cmdType != Const.unassignedIntValue)
         {
            sqlAddWhere += " AND vlfMsgOutHst.BoxCmdOutTypeId=" + cmdType;
         }
         if (from != Const.unassignedDateTime)
         {
            sqlAddWhere += " AND DateTime>='" + from + "'";
         }
         if (to != Const.unassignedDateTime)
         {
            sqlAddWhere += " AND DateTime<='" + to + "'";
         }
         DataSet resultDataSet = null;
         try
         {
            // 1. Prepares SQL statement
            string sql = "SELECT TOP " + numOfRecords + " DateTime,vlfBox.BoxId,UserId,Priority,DclId,AslId," +
                     "vlfMsgOutHst.BoxCmdOutTypeId,RTRIM(BoxCmdOutTypeName) AS BoxCmdOutTypeName," +
                     "vlfMsgOutHst.BoxProtocolTypeId,RTRIM(vlfBoxProtocolType.BoxProtocolTypeName) AS BoxProtocolTypeName," +
                     "CASE WHEN CommInfo1 IS NULL then '' ELSE RTRIM(CommInfo1) END AS CommInfo1," +
                     "CASE WHEN CommInfo2 IS NULL then '' ELSE RTRIM(CommInfo2) END AS CommInfo2," +
                     "CASE WHEN CustomProp IS NULL then '' ELSE RTRIM(CustomProp) END AS CustomProp," +
                     "SequenceNum,Acknowledged" +
                     " FROM         dbo.vlfBox  with(nolock) INNER JOIN " +
                     " dbo.vlfFleetVehicles INNER JOIN " +
                     " dbo.vlfVehicleAssignment ON dbo.vlfFleetVehicles.VehicleId = dbo.vlfVehicleAssignment.VehicleId INNER JOIN " +
                     " dbo.vlfMsgOutHst with (nolock) ON dbo.vlfVehicleAssignment.BoxId = dbo.vlfMsgOutHst.BoxId ON dbo.vlfBox.BoxId = dbo.vlfMsgOutHst.BoxId INNER JOIN " +
                     " dbo.vlfBoxProtocolTypeCmdOutType INNER JOIN " +
                     " dbo.vlfBoxProtocolType ON dbo.vlfBoxProtocolTypeCmdOutType.BoxProtocolTypeId = dbo.vlfBoxProtocolType.BoxProtocolTypeId ON " +
                     " dbo.vlfMsgOutHst.BoxCmdOutTypeId = dbo.vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId " +
                     " INNER JOIN dbo.vlfBoxCmdOutType ON dbo.vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId = dbo.vlfBoxCmdOutType.BoxCmdOutTypeId " +
                      sqlAddWhere +
                     " ORDER BY DateTime DESC"; 

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
		/// Retrieves last UploadFirmwareStatus command from the history
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="fromDateTime"></param>
        /// <returns>DataSet [Description],[FwName],[FwId],[DateTime],[BoxId],[CustomProp],[Acknowledged],[OAPPort]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetLastCommandFromHistory(int boxId,DateTime fromDateTime)
		{
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
                string sql = "SELECT TOP 1 vlfVehicleInfo.Description, vlfFirmware.FwName, vlfFirmware.FwId,Convert(varchar,vlfMsgOutHst.DateTime,101) + ' ' + Convert(varchar, vlfMsgOutHst.DateTime,108) as DateTime,vlfMsgOutHst.BoxId,CASE WHEN CustomProp IS NULL then '' ELSE RTRIM(CustomProp) END AS CustomProp,CASE WHEN Acknowledged IS NULL then '' ELSE RTRIM(Acknowledged) END AS Acknowledged,OAPPort,vlfMsgOutHst.BoxProtocolTypeId FROM vlfMsgOutHst  with(nolock) INNER JOIN vlfVehicleAssignment ON vlfMsgOutHst.BoxId = vlfVehicleAssignment.BoxId INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN vlfBox with (nolock) ON vlfVehicleAssignment.BoxId = vlfBox.BoxId INNER JOIN vlfFirmwareChannels ON vlfBox.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId WHERE vlfMsgOutHst.BoxCmdOutTypeId = " + (short)Enums.CommandType.UploadFirmwareStatus + " AND vlfMsgOutHst.BoxId =" + boxId + " AND vlfMsgOutHst.DateTime>='" + fromDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "' ORDER BY dbo.vlfMsgOutHst.DateTime DESC";
				// 2. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve messages from history by boxId=" + boxId + " fromDateTime=" + fromDateTime.ToString(), objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve messages from history by boxId=" + boxId + " fromDateTime=" + fromDateTime.ToString() + objException.Message);
			}
			return resultDataSet;
		}
		#endregion

		#region Protected Interfaces
		/// <summary>
		/// Add new Msg.
		/// </summary>
		/// <param name="cMFOut"></param>
		/// <param name="dateTime"></param>
		/// <param name="priority"></param>
		/// <param name="dclId"></param>
		/// <param name="aslId"></param>
		/// <param name="userId"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected void AppendMsg(CMFOut cMFOut,Int64 dateTime,SByte priority,
								short dclId,short aslId,int userId)
		{
			int rowsAffected = 0;
			ValidateProtocolTypeByCmdOutType(Convert.ToInt16(cMFOut.protocolTypeID),
											Convert.ToInt16(cMFOut.commandTypeID));
			try
			{
				// Construct SQL for migration to the data target. 
				string sql = "INSERT INTO " + tableName + "( "  +
					"DateTime"			+
					",BoxId"            +
					",UserId"			+
					",Priority"			+
					",DclId"            +
					",AslId"            +
					",BoxCmdOutTypeId"  +
					",BoxProtocolTypeId"+
					",CommInfo1"		+
					",CommInfo2"		+
					",CustomProp"		+
					",SequenceNum"		+
					",Acknowledged"		+
					",Scheduled) VALUES ( @DateTime,@BoxId,@UserId,@Priority,@DclId,@AslId,@BoxCmdOutTypeId,@BoxProtocolTypeId,@CommInfo1,@CommInfo2,@CustomProp,@SequenceNum,@Acknowledged,@Scheduled ) ";
				// Set SQL command
				sqlExec.ClearCommandParameters();
				// Add parameters to SQL statement
				sqlExec.AddCommandParam("@DateTime",SqlDbType.BigInt,dateTime);
				sqlExec.AddCommandParam("@BoxId",SqlDbType.Int,cMFOut.boxID);
				sqlExec.AddCommandParam("@UserId",SqlDbType.Int,userId);
				sqlExec.AddCommandParam("@Priority",SqlDbType.TinyInt,priority);
				sqlExec.AddCommandParam("@DclId",SqlDbType.SmallInt,dclId);
				sqlExec.AddCommandParam("@AslId",SqlDbType.SmallInt,aslId);
				sqlExec.AddCommandParam("@BoxCmdOutTypeId",SqlDbType.SmallInt,Convert.ToInt16(cMFOut.commandTypeID));
				sqlExec.AddCommandParam("@BoxProtocolTypeId",SqlDbType.SmallInt,Convert.ToInt16(cMFOut.protocolTypeID));
				sqlExec.AddCommandParam("@CommInfo1",SqlDbType.Char,cMFOut.commInfo1);
				if(cMFOut.commInfo2 == null)
					sqlExec.AddCommandParam("@CommInfo2",SqlDbType.Char,System.DBNull.Value);
				else
					sqlExec.AddCommandParam("@CommInfo2",SqlDbType.Char,cMFOut.commInfo2);
				if(cMFOut.customProperties == null)
					sqlExec.AddCommandParam("@CustomProp",SqlDbType.Char,System.DBNull.Value);
				else
					sqlExec.AddCommandParam("@CustomProp",SqlDbType.Char,cMFOut.customProperties);
				sqlExec.AddCommandParam("@SequenceNum",SqlDbType.Int,cMFOut.sequenceNum);
				if(cMFOut.ack == null)
					sqlExec.AddCommandParam("@Acknowledged",SqlDbType.Char,System.DBNull.Value);
				else
					sqlExec.AddCommandParam("@Acknowledged",SqlDbType.Char,cMFOut.ack);
				sqlExec.AddCommandParam("@Scheduled",SqlDbType.TinyInt,Convert.ToInt16(cMFOut.scheduled));
				
				
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
            string prefixMsg = "AppendMsg -> (SqlException)Unable to add new msg Out" + cMFOut.ToString();
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
            string prefixMsg = "AppendMsg -> (Exception) Unable to add new msg Out" + cMFOut.ToString();
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
            string prefixMsg = "AppendMsg -> (rows=0) Unable to add new msg Out" + cMFOut.ToString();
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The message with datetime '" + dateTime.ToString() + "' already exists.");
			}
		}	
		/// <summary>
		/// Deletes exist message.
		/// </summary>
		/// <param name="dateTime"></param>
		/// <param name="boxId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected int DeleteMsg(Int64 dateTime, int boxId)
		{
			string prefixMsg = "Unable to delete by box " + boxId + " and '" + new DateTime(dateTime) + "' datetime";
			//Prepares SQL statement
			string sql = "DELETE FROM " + tableName + 
						" WHERE DateTime=" + dateTime + 
						" AND BoxId=" + boxId;
			return DeleteRowsBySql(sql,prefixMsg,"datetime");
		}
		/// <summary>
		/// Prevents inconsistent insert of the "boxProtocolTypeId" field to the 
		/// "vlfMsgOut" table by checking valid dependency in the 
		/// "vlfBoxCmdOutType,vlfBoxProtocolTypeCmdOutType,vlfBoxProtocolType" tables.
		/// </summary>
		/// <param name="boxProtocolTypeId"></param>
		/// <param name="boxCmdOutTypeId"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if data does not exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected void ValidateProtocolTypeByCmdOutType(short boxProtocolTypeId,short boxCmdOutTypeId)
		{
			//Prepares SQL statement
			string sql = "SELECT COUNT(*)" +
                " FROM vlfBoxCmdOutType,vlfBoxProtocolTypeCmdOutType,vlfBoxProtocolType with(nolock) " + 
				" WHERE (vlfBoxCmdOutType.BoxCmdOutTypeId = vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId)" +
				" AND (vlfBoxProtocolTypeCmdOutType.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId)" +
				" AND (vlfBoxProtocolType.BoxProtocolTypeId = " + boxProtocolTypeId + ")" +
				" AND (vlfBoxCmdOutType.BoxCmdOutTypeId = " + boxCmdOutTypeId + ")";				 
			int recordCount = 0;
			try
			{
				//Executes SQL statement
				recordCount = (int)sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Validation warning. Wrong protocol type=" 
					+ boxProtocolTypeId + " for command out type=" + boxCmdOutTypeId + ". ";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Validation warning. Wrong protocol type=" 
					+ boxProtocolTypeId + " for command out type=" + boxCmdOutTypeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			if(recordCount <= 0) 
			{
				string prefixMsg = "Validation warning. Wrong protocol type=" 
					+ boxProtocolTypeId + " for command out type=" + boxCmdOutTypeId + ". ";
				throw new DASAppResultNotFoundException(prefixMsg);
			}
		}
		/// <summary>
		/// Add Msg to the history.
		/// </summary>
		/// <param name="cMFOut"></param>
		/// <param name="dateTime"></param>
		/// <param name="priority"></param>
		/// <param name="dclId"></param>
		/// <param name="aslId"></param>
		/// <param name="userId"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected void AddToHistory(CMFOut cMFOut,DateTime dateTime,SByte priority,
									short dclId,short aslId,int userId)
		{
			int rowsAffected = 0;
			try
			{
				// Construct SQL for migration to the data target. 
				string sql = "INSERT INTO " + tableName + "Hst ( "  +
					"BoxId"             +
					",DateTime"			+
					",UserId"			+
					",Priority"         +
					",DclId"            +
					",AslId"            +
					",BoxCmdOutTypeId"  +
					",BoxProtocolTypeId"+
					",CommInfo1"		+
					",CommInfo2"		+
					",CustomProp"		+
					",SequenceNum"		+
					",Acknowledged"		+
					",Scheduled) VALUES (@BoxId,@DateTime,@UserId,@Priority,@DclId,@AslId,@BoxCmdOutTypeId,@BoxProtocolTypeId,@CommInfo1,@CommInfo2,@CustomProp,@SequenceNum,@Acknowledged,@Scheduled ) ";
	
				// Set SQL command
				sqlExec.ClearCommandParameters();
				
				// Add parameters to SQL statement
				sqlExec.AddCommandParam("@BoxId",SqlDbType.Int,cMFOut.boxID);
				sqlExec.AddCommandParam("@DateTime",SqlDbType.DateTime,dateTime);
				sqlExec.AddCommandParam("@UserId",SqlDbType.Int,userId);
				sqlExec.AddCommandParam("@Priority",SqlDbType.TinyInt,priority);
				sqlExec.AddCommandParam("@DclId",SqlDbType.SmallInt,dclId);
				sqlExec.AddCommandParam("@AslId",SqlDbType.SmallInt,aslId);
				sqlExec.AddCommandParam("@BoxCmdOutTypeId",SqlDbType.SmallInt,Convert.ToInt16(cMFOut.commandTypeID));
				sqlExec.AddCommandParam("@BoxProtocolTypeId",SqlDbType.SmallInt,Convert.ToInt16(cMFOut.protocolTypeID));
				sqlExec.AddCommandParam("@CommInfo1",SqlDbType.Char,cMFOut.commInfo1);
				if(cMFOut.commInfo2 == null)
					sqlExec.AddCommandParam("@CommInfo2",SqlDbType.Char,System.DBNull.Value);
				else
					sqlExec.AddCommandParam("@CommInfo2",SqlDbType.Char,cMFOut.commInfo2);
				if(cMFOut.customProperties == null)
					sqlExec.AddCommandParam("@CustomProp",SqlDbType.Char,System.DBNull.Value);
				else
					sqlExec.AddCommandParam("@CustomProp",SqlDbType.Char,cMFOut.customProperties);
				sqlExec.AddCommandParam("@SequenceNum",SqlDbType.Int,cMFOut.sequenceNum);
				if(cMFOut.ack == null)
					sqlExec.AddCommandParam("@Acknowledged",SqlDbType.Char,System.DBNull.Value);
				else
					sqlExec.AddCommandParam("@Acknowledged",SqlDbType.Char,cMFOut.ack);
				sqlExec.AddCommandParam("@Scheduled",SqlDbType.TinyInt,cMFOut.scheduled);
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
            string prefixMsg = "AddToHistory -> (SqlException) Unable to add new msg Out history with date '" + dateTime.ToString() + "'.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
            string prefixMsg = "AddToHistory -> (Exception) Unable to add new msg Out history with date '" + dateTime.ToString() + "'.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
            string prefixMsg = "AddToHistory -> (rows=0) Unable to add new msg Out history with date '" + dateTime.ToString() + "'.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The message with datetime '" + dateTime.ToString() + "' already exists.");
			}
		}
	
		/// <summary>
		/// Retrieves next record by dcl id
		/// </summary>
		/// <param name="dclId"></param>
		/// <param name="prefixMsg"></param>
		/// <returns>DataSet [DateTime],[BoxId],[UserId],[Priority],[DclId],[AslId],
		/// [BoxCmdOutTypeId],[BoxCmdOutTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
		/// [CommInfo1],[CommInfo2],[CustomProp],[SequenceNum],[Acknowledged]
		/// </returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected DataSet RetrievesNextMsg(short dclId,int max, string prefixMsg)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
                string sql = "SELECT TOP " + max + " * FROM vlfMsgOut  with(nolock) WHERE DclId=" + dclId + " ORDER BY DateTime";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			return sqlDataSet;
		}
		/// <summary>
		/// Retrieves next record by dcl id
		/// </summary>
		/// <param name="dclId"></param>
		/// <param name="currTime"></param>
		/// <param name="prefixMsg"></param>
		/// <returns>DataSet [DateTime],[BoxId],[UserId],[Priority],[DclId],[AslId],
		/// [BoxCmdOutTypeId],[BoxCmdOutTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],
		/// [CommInfo1],[CommInfo2],[CustomProp],[SequenceNum],[Acknowledged]
		/// </returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected DataSet RetrievesNextMsg(short dclId,DateTime currTime,string prefixMsg)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
                string sql = "SELECT TOP 1 * FROM vlfMsgOut  with(nolock) WHERE DclId=" + dclId + 
							" AND DateTime<=" + currTime.Ticks + 
							" ORDER BY DateTime";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			return sqlDataSet;
		}
		#endregion
		/// <summary>
		/// Get last sequence number from the history
		/// </summary>
		/// <param name="boxId"></param>
        /// <param name="boxProtocolTypeId"></param>
		/// <returns>Last Sequence Number</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int GetLastSequenceNumber(int boxId, short boxProtocolTypeId)
        {
            int lastSequenceNumber = Const.unassignedIntValue;
            DataSet sqlDataSet = null;
            string prefixMsg = string.Format("Unable to GetLastSequenceNumber({0}, {1})", boxId, boxProtocolTypeId);
            try
            {
                //Prepares SQL statement
                string sql = "SELECT TOP 1 SequenceNum FROM vlfMsgOutHst with(nolock) "
                                + " WHERE BoxId=" + boxId
                                + " AND BoxProtocolTypeId=" + boxProtocolTypeId
                                + " ORDER BY DateTime DESC";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
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
			if((sqlDataSet != null) && (sqlDataSet.Tables.Count > 0) && (sqlDataSet.Tables[0].Rows.Count > 0))
			{
				lastSequenceNumber = Convert.ToInt32(sqlDataSet.Tables[0].Rows[0][0]);
			}
            return lastSequenceNumber;
        }
	}
}
