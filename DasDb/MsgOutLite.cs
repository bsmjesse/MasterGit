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
	public class MsgOutLite: TblGenInterfaces
   {

      private const string prefixNextMsg = "Unable to retrieve next message. ";
		
		#region Public Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public MsgOutLite(SQLExecuter sqlExec) : base ("vlfMsgOut",sqlExec)
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
                  Util.BTrace(Util.INF1, " MsgOutLite.AddMsg -> DASAppViolatedIntegrityConstraintsException {0} times", currRetries);
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
      #endregion

      #region Protected Interfaces
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
      protected void ValidateProtocolTypeByCmdOutType(short boxProtocolTypeId, short boxCmdOutTypeId)
      {
         //Prepares SQL statement
         string sql = "SELECT COUNT(*)" +
            " FROM vlfBoxCmdOutType,vlfBoxProtocolTypeCmdOutType,vlfBoxProtocolType" +
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
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Validation warning. Wrong protocol type="
               + boxProtocolTypeId + " for command out type=" + boxCmdOutTypeId + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         if (recordCount <= 0)
         {
            string prefixMsg = "Validation warning. Wrong protocol type="
               + boxProtocolTypeId + " for command out type=" + boxCmdOutTypeId + ". ";
            throw new DASAppResultNotFoundException(prefixMsg);
         }
      }
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
         if ( cMFOut.commandTypeID != (int)Enums.CommandType.ThirdPartyPacket )
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
               sqlExec.AddCommandParam("@CustomProp", SqlDbType.Char, cMFOut.customProperties, cMFOut.customProperties.Length );

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
            string prefixMsg = "MsgOutLite.AppendMsg ->(SqlException) Unable to add new msg Out with date '" + new DateTime(dateTime) + "'.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
            string prefixMsg = "MsgOutLite.AppendMsg ->(Exception) Unable to add new msg Out with date '" + new DateTime(dateTime) + "'.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
            string prefixMsg = "MsgOutLite.AppendMsg ->(rows=0) Unable to add new msg Out with date '" + new DateTime(dateTime) + "'.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The message with datetime '" + dateTime.ToString() + "' already exists.");
			}
      }
      #endregion
   }
}