using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;			// Enums

namespace VLF.DAS.DB
{
#if MDT_NEW
   /// <summary> 
   ///         Provides interface to vlfTxtMsgsNew table. 
   /// </summary>
   /// <comment>
   ///      we reached the problem where the index used for MDT messages was restricted to 2 bytes and
   ///      for 2 months of data we reached the maximum
   ///      the solution is to make the id for message UNIQUE per box
   /// </comment>
   public class TxtMsgs : TblGenInterfaces
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public TxtMsgs(SQLExecuter sqlExec)
         : base("vlfTxtMsgsNew", sqlExec)
      {
      }

      /// <summary>
      ///         
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="msgDateTime"></param>
      /// <param name="latitude"></param>
      /// <param name="longitude"></param>
      /// <param name="txtMsgTypeId"></param>
      /// <param name="msgBody"></param>
      /// <param name="msgDirection"></param>
      /// <param name="userId"></param>
      /// <param name="ack"></param>
      /// <returns></returns>
      public int AddMsg(int boxId, 
                        DateTime msgDateTime,
                        double latitude, 
                        double longitude,
                        short txtMsgTypeId, 
                        string msgBody, 
                        short msgDirection, 
                        int userId, 
                        string ack)
      {
         int currentMsgId = VLF.CLS.Def.Const.unassignedIntValue;
         try
         {
            // Set SQL command
            sqlExec.ClearCommandParameters();
            // Add parameters to SQL statement
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
            sqlExec.AddCommandParam("@MsgDateTime", SqlDbType.DateTime, msgDateTime);
            sqlExec.AddCommandParam("@Lat", SqlDbType.Float, latitude);
            sqlExec.AddCommandParam("@Long", SqlDbType.Float, longitude);
            sqlExec.AddCommandParam("@TxtMsgTypeId", SqlDbType.SmallInt, txtMsgTypeId);
            if (msgBody == null)
               sqlExec.AddCommandParam("@MsgBody", SqlDbType.VarChar, System.DBNull.Value);
            else
               sqlExec.AddCommandParam("@MsgBody", SqlDbType.VarChar, msgBody); sqlExec.AddCommandParam("@MsgDirection", SqlDbType.SmallInt, msgDirection);
            sqlExec.AddCommandParam("@Acknowledged", SqlDbType.VarChar, ack);
            sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@MsgId", SqlDbType.Int, ParameterDirection.Output, currentMsgId);

            //Executes SQL statement
            sqlExec.SPExecuteNonQuery("dbo.AddTextMessage") ;

            currentMsgId = Convert.ToInt32(sqlExec.ReadCommandParam("@MsgId"));                         
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to add new text msg.";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to add new text msg.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return currentMsgId;
      }

      /// <summary>
      /// Add new text message
      /// </summary>
      /// <returns> current message id or -1 in case of error</returns>
      /// <param name="boxId"></param>
      /// <param name="msgDateTime"></param>
      /// <param name="txtMsgTypeId"></param>
      /// <param name="msgBody"></param>
      /// <param name="msgDirection"></param>
      /// <param name="userId"></param>
      /// <param name="ack"></param>
      /// <exception cref="DASAppDataAlreadyExistsException">Throws if data already exist.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int AddMsg(int boxId, 
                        DateTime msgDateTime, 
                        short txtMsgTypeId, 
                        string msgBody, 
                        short msgDirection, 
                        int userId, 
                        string ack)
      {
         return AddMsg(boxId, msgDateTime, .0, .0, txtMsgTypeId, msgBody, msgDirection, userId, ack);
      }
      /// <summary>
      /// Save message response
      /// </summary>
      ///  <param name="boxId"></param>
      /// <param name="msgId"></param>
      /// <param name="respondDateTime"></param>
      /// <param name="msgResponse"></param>
      /// <returns></returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int SetMsgResponse(int boxId, int msgId, DateTime respondDateTime, string msgResponse)
      {
         int rowsAffected = 0;
         try
         {
            //Prepares SQL statement
            string sql = string.Format("UPDATE {0} SET ResponseDateTime='{1}',MsgResponse='{2}' WHERE MsgId={3}and BoxId={4}",
                                          tableName,
                                          respondDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"),
                                          msgResponse.Replace("'", "''"), 
                                          msgId,
                                          boxId);
            //Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to update text message response. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to update text message response. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         //Throws exception in case of wrong result
         if (rowsAffected == 0)
         {
            throw new DASAppViolatedIntegrityConstraintsException("Unable to update text message response. ");
         }
         return rowsAffected;
      }
      /// <summary>
      /// Set owner foe the message
      /// </summary>
      /// <param name="boxId"></param>       
      /// <param name="msgId"></param>
      /// <param name="userId"></param>
      /// <returns>rows affected</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int SetMsgUserId(int boxId, int msgId, int userId)
      {
         int rowsAffected = 0;
         try
         {
            //Prepares SQL statement
            string sql = string.Format("UPDATE {0} SET UserId={1} WHERE MsgId={2} and BoxId={3}", 
                                          tableName, userId, msgId, boxId);
            //Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to update text message user id " + userId + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to update text message user id " + userId + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         //Throws exception in case of wrong result
         if (rowsAffected == 0)
         {
            throw new DASAppViolatedIntegrityConstraintsException("Unable to update text message user id. ");
         }
         return rowsAffected;
      }
      /// <summary>
      /// Sets message ack
      /// </summary>
      /// <param name="boxId"></param>              
      /// <param name="msgId"></param>
      /// <param name="ack"></param>
      /// <returns>rows affected</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int SetMsgAck(int boxId, int msgId, string ack)
      {
         int rowsAffected = 0;
         try
         {
            //Prepares SQL statement
            string sql = string.Format("UPDATE {0} SET Acknowledged='{1}' WHERE MsgId={2} and BoxId={3}",
                                       tableName,                                       
                                       ack,
                                       msgId,
                                       boxId);
            //Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to update text message ack " + ack + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to update text message ack " + ack + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         //Throws exception in case of wrong result
         if (rowsAffected == 0)
         {
            throw new DASAppViolatedIntegrityConstraintsException("Unable to update text message ack. ");
         }
         return rowsAffected;
      }
      /// <summary>
      /// Retrieves messages by box id
      /// </summary>
      /// <remarks>
      /// all box Ids -> VLF.CLS.Def.Const.unassignedIntValue
      /// from DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
      /// to DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
      /// 1. In messages directions -> VLF.CLS.Def.Enums.TxtMsgType.MdtText
      /// 2. Out messages directions -> VLF.CLS.Def.Enums.TxtMsgType.ClientText
      /// 3. Both messages directions -> MdtText,ClientText (all visible to client messages)
      /// </remarks>
      /// <param name="userId"></param>
      /// <param name="boxId"></param>
      /// <param name="from"></param>
      /// <param name="to"></param>
      /// <param name="msgDirection"></param>
      /// <param name="tblLandmarks"></param>
      /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
      /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
      /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
      /// [Acknowledged],[UserName]
      /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetMessagesFullInfo(int userId, int boxId, DateTime from, DateTime to, short msgDirection)
      {
         DataSet sqlDataSet = null;
         try
         {
            string sqlWhere = "";
            if ((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.In)
               sqlWhere = " WHERE (TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtText + " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial + ")";
            else if ((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.Out)
               sqlWhere = " WHERE TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.ClientText;
            else
               // Retrieve all visible to client messages
               sqlWhere = " WHERE (TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtText +
                        " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.ClientText +
                        " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial + ")";

            if (boxId != VLF.CLS.Def.Const.unassignedIntValue)
               sqlWhere += " AND vlfTxtMsgsNew.BoxId=" + boxId;
            if (from != VLF.CLS.Def.Const.unassignedDateTime)
               sqlWhere += " AND MsgDateTime>='" + from + "'";
            if (to != VLF.CLS.Def.Const.unassignedDateTime)
               sqlWhere += " AND MsgDateTime<='" + to + "'";

            // 1. Prepares SQL statement
            //string sql = "DECLARE @ResolveLandmark int DECLARE @Timezone int" +
            //            " DECLARE @DayLightSaving int"+
            //            " SELECT @Timezone=PreferenceValue FROM vlfUserPreference"+
            //            " WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.TimeZone +
            //            " IF @Timezone IS NULL SET @Timezone=0"+
            //            " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
            //            " IF @DayLightSaving IS NULL SET @DayLightSaving=0"+
            //            " SET @Timezone= @Timezone + @DayLightSaving"+
            //            " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +

            //        " SELECT DISTINCT vlfVehicleInfo.VehicleId, RTRIM(vlfVehicleAssignment.LicensePlate) as LicensePlate,"+

            //        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then RTRIM(vlfVehicleInfo.Description) ELSE"+
            //        " (SELECT RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) AS NameUser FROM vlfUser INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId WHERE vlfUser.UserId=vlfTxtMsgsNew.UserId)"+
            //        " END AS [From],"+

            //        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then RTRIM(vlfVehicleInfo.Description) ELSE"+
            //        " (SELECT RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) AS NameUser FROM vlfUser INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId WHERE vlfUser.UserId=vlfTxtMsgsNew.UserId)"+
            //        " END AS [To],"+

            //        //"CASE WHEN MsgDateTime IS NULL then '' ELSE convert(varchar,DATEADD(hour,@Timezone,MsgDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,MsgDateTime),108) END AS MsgDateTime,"+
            //        "DATEADD(hour,@Timezone,MsgDateTime) AS MsgDateTime,"+
            //        "MsgId,vlfTxtMsgsNew.BoxId,TxtMsgTypeId,RTRIM(MsgBody) as MsgBody,"+
            //        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then '" + VLF.CLS.Def.Const.MsgInDirectionSign + "'"+
            //            " ELSE CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then '" + VLF.CLS.Def.Const.MsgOutDirectionSign + 
            //            "' ELSE 'N/A' END END AS MsgDirection,"+
            //        " isnull(MsgResponse,'N/A') as MsgResponse,"+
            //        "CASE WHEN ResponseDateTime IS NULL THEN 'N/A' ELSE convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),108) END as ResponseDateTime,vlfTxtMsgsNew.UserId,Acknowledged,isnull(vlfUser.UserName,' ') AS UserName,"+
            //        "CASE WHEN MsgDirection=0 then ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') ELSE ISNULL(StreetAddress,'N/A') END AS StreetAddress," +

            //        "isnull(Latitude,0) as Latitude,"+ 
            //        "isnull(Longitude,0) as Longitude,"+
            //        "isnull(Speed,0) as Speed,"+ 
            //        "isnull(Heading,0) as Heading,"+
            //        "isnull(ValidGps,0) as ValidGps"+
            //        " FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId"+
            //        " INNER JOIN vlfTxtMsgsNew ON vlfVehicleAssignment.BoxId=vlfTxtMsgsNew.BoxId"+
            //        " LEFT JOIN  vlfMsgInHst ON vlfMsgInHst.BoxId=vlfTxtMsgsNew.BoxId AND vlfMsgInHst.OriginDateTime=vlfTxtMsgsNew.MsgDateTime"+
            //        " LEFT JOIN vlfUser ON vlfUser.UserId=vlfTxtMsgsNew.UserId"+
            //        sqlWhere + " ORDER BY vlfTxtMsgsNew.MsgDateTime DESC";



            string sql = "DECLARE @ResolveLandmark int DECLARE @Timezone int" +
                            " DECLARE @DayLightSaving int" +
                            " SELECT @Timezone=PreferenceValue FROM vlfUserPreference" +
                            " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                            " IF @Timezone IS NULL SET @Timezone=0" +
                            " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                            " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                            " SET @Timezone= @Timezone + @DayLightSaving" +
                            " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +

                    " SELECT DISTINCT vlfVehicleInfo.VehicleId, RTRIM(vlfVehicleAssignment.LicensePlate) as LicensePlate," +

                    "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then RTRIM(vlfVehicleInfo.Description) ELSE" +
                        " RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) END AS [From]," +

                    "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then RTRIM(vlfVehicleInfo.Description) ELSE" +
                        " RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) END AS [To]," +

                    "CASE WHEN MsgDateTime IS NULL then '' ELSE convert(varchar,DATEADD(hour,@Timezone,MsgDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,MsgDateTime),108) END AS MsgDateTime," +
                        "DATEADD(hour,@Timezone,MsgDateTime) AS MsgDateTime," +
                        "MsgId,vlfTxtMsgsNew.BoxId,TxtMsgTypeId,RTRIM(MsgBody) as MsgBody," +
                        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then '" + VLF.CLS.Def.Const.MsgInDirectionSign + "'" +
                            " ELSE CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then '" + VLF.CLS.Def.Const.MsgOutDirectionSign +
                            "' ELSE 'N/A' END END AS MsgDirection," +
                        " isnull(MsgResponse,'N/A') as MsgResponse," +
                        "CASE WHEN ResponseDateTime IS NULL THEN 'N/A' ELSE convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),108) END as ResponseDateTime,vlfTxtMsgsNew.UserId,Acknowledged,isnull(vlfUser.UserName,' ') AS UserName," +
               //"CASE WHEN MsgDirection=0 then ISNULL(vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') ELSE ISNULL(StreetAddress,'N/A') END AS StreetAddress," +
                    "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then ISNULL(vlfTxtMsgsNew.LastAddress,'" + VLF.CLS.Def.Const.addressNA + "') ELSE 'N/A'  END AS StreetAddress," +
                    "isnull(Lat,0) as Latitude," +
                        "isnull(Long,0) as Longitude" +
               // "isnull(Speed,0) as Speed," +
               // "isnull(Heading,0) as Heading," +
               // "isnull(ValidGps,0) as ValidGps" +
                        " FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId" +
                        " INNER JOIN vlfTxtMsgsNew ON vlfVehicleAssignment.BoxId=vlfTxtMsgsNew.BoxId" +
               //" LEFT JOIN  vlfMsgInHst ON vlfMsgInHst.BoxId=vlfTxtMsgsNew.BoxId AND vlfMsgInHst.OriginDateTime=vlfTxtMsgsNew.MsgDateTime" +
                        " LEFT JOIN vlfUser ON vlfUser.UserId=vlfTxtMsgsNew.UserId" +
                        " LEFT JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId " +
                        sqlWhere + " ORDER BY vlfTxtMsgsNew.MsgDateTime DESC";

            // 2. Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("Unable to retrieve text messages.", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException("Unable to retrieve text messages. " + objException.Message);
         }
         // 3. Return result
         return sqlDataSet;
      }
      /// <summary>
      /// Retrieves message info
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="userId"></param>
      /// <param name="msgId"></param>
      /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
      /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
      /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
      /// [Acknowledged],[UserName],
      /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetMessageFullInfo(int vehicleId, int userId, int msgId)
      {
         DataSet sqlDataSet = null;
         try
         {
            // 1. Prepares SQL statement
            string sql = "DECLARE @ResolveLandmark int DECLARE @Timezone int" +
           " DECLARE @DayLightSaving int" +
           " SELECT @Timezone=PreferenceValue FROM vlfUserPreference" +
           " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
           " IF @Timezone IS NULL SET @Timezone=0" +
           " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
           " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
           " SET @Timezone= @Timezone + @DayLightSaving" +
                " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +

                " SELECT DISTINCT vlfVehicleInfo.VehicleId, RTRIM(vlfVehicleAssignment.LicensePlate) as LicensePlate," +

           "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then RTRIM(vlfVehicleInfo.Description) ELSE" +
           " (SELECT RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) AS NameUser FROM vlfUser INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId WHERE vlfUser.UserId=vlfTxtMsgsNew.UserId)" +
           " END AS [From]," +

           "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then RTRIM(vlfVehicleInfo.Description) ELSE" +
           " (SELECT RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) AS NameUser FROM vlfUser INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId WHERE vlfUser.UserId=vlfTxtMsgsNew.UserId)" +
           " END AS [To]," +

           "DATEADD(hour,@Timezone,MsgDateTime) AS MsgDateTime," +
           "MsgId,vlfTxtMsgsNew.BoxId,TxtMsgTypeId,RTRIM(MsgBody) as MsgBody," +
           "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then '" + VLF.CLS.Def.Const.MsgInDirectionSign + "'" +
           " ELSE CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then '" + VLF.CLS.Def.Const.MsgOutDirectionSign +
           "' ELSE 'N/A' END END AS MsgDirection," +
           " isnull(MsgResponse,'N/A') as MsgResponse," +
           "CASE WHEN ResponseDateTime IS NULL THEN 'N/A' ELSE convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),108) END as ResponseDateTime,vlfTxtMsgsNew.UserId,vlfTxtMsgsNew.Acknowledged,isnull(vlfUser.UserName,' ') AS UserName," +
                "CASE WHEN MsgDirection=0 then ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') ELSE ISNULL(StreetAddress,'N/A') END AS StreetAddress," +
           "isnull(Latitude,0) as Latitude," +
           "isnull(Longitude,0) as Longitude," +
           "isnull(Speed,0) as Speed," +
           "isnull(Heading,0) as Heading," +
           "isnull(ValidGps,0) as ValidGps" +
           " FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId" +
           " INNER JOIN vlfTxtMsgsNew ON vlfVehicleAssignment.BoxId=vlfTxtMsgsNew.BoxId" +
           " LEFT JOIN  vlfMsgInHst ON vlfMsgInHst.BoxId=vlfTxtMsgsNew.BoxId AND vlfMsgInHst.OriginDateTime=vlfTxtMsgsNew.MsgDateTime" +
           " LEFT JOIN vlfUser ON vlfUser.UserId=vlfTxtMsgsNew.UserId" +
           " WHERE MsgId=" + msgId +
           " AND vlfVehicleInfo.vehicleId=" + vehicleId + 
           " ORDER BY MsgDateTime DESC";

            // 2. Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);

         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("Unable to retrieve text messages.", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException("Unable to retrieve text messages. " + objException.Message);
         }
         // 3. Return result
         return sqlDataSet;
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
      /// <param name="msgDirection"></param>
      /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
      /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
      /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
      /// [Acknowledged]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetUserMessagesFullInfo(int userId, DateTime from, DateTime to, short msgDirection)
      {
         DataSet sqlDataSet = null;
         try
         {
            string sqlWhere = " WHERE vlfFleetUsers.UserId=" + userId;
            if ((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.In)
               sqlWhere += " AND (TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtText + " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial + ")";
            else if ((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.Out)
               sqlWhere += " AND TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.ClientText;
            else
               // Retrieve all visible to client messages
               sqlWhere += " AND (TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtText +
                              " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial +
                              " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.ClientText + ")";

            if (from != VLF.CLS.Def.Const.unassignedDateTime)
               sqlWhere += " AND MsgDateTime>='" + from + "'";
            if (to != VLF.CLS.Def.Const.unassignedDateTime)
               sqlWhere += " AND MsgDateTime<='" + to + "'";

            // 1. Prepares SQL statement
            string sql = "DECLARE @Timezone int" +
               " DECLARE @DayLightSaving int" +
               " SELECT @Timezone=PreferenceValue FROM vlfUserPreference" +
               " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
               " IF @Timezone IS NULL SET @Timezone=0" +
               " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
               " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
               " SET @Timezone= @Timezone + @DayLightSaving" +

                    " SELECT  DISTINCT vlfVehicleInfo.VehicleId,vlfVehicleAssignment.LicensePlate," +

               "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then RTRIM(vlfVehicleInfo.Description) ELSE" +
               " (SELECT RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) AS NameUser FROM vlfUser INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId WHERE vlfUser.UserId=vlfTxtMsgsNew.UserId)" +
               " END AS [From]," +

               "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then RTRIM(vlfVehicleInfo.Description) ELSE" +
               " (SELECT RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) AS NameUser FROM vlfUser INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId WHERE vlfUser.UserId=vlfTxtMsgsNew.UserId)" +
               " END AS [To]," +

               "DATEADD(hour,@Timezone,MsgDateTime) AS MsgDateTime," +
               "MsgId,vlfTxtMsgsNew.BoxId,TxtMsgTypeId,MsgBody," +
               "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then '" + VLF.CLS.Def.Const.MsgInDirectionSign + "'" +
                  " ELSE CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then '" + VLF.CLS.Def.Const.MsgOutDirectionSign +
                     "' ELSE 'N/A' END END AS MsgDirection," +
               "isnull(MsgResponse,'N/A') as MsgResponse," +
               "CASE WHEN ResponseDateTime IS NULL THEN 'N/A' ELSE convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),108) END as ResponseDateTime,isnull(vlfTxtMsgsNew.UserId,-1) AS UserId,vlfTxtMsgsNew.Acknowledged" +
               " FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId" +
               " INNER JOIN vlfFleetVehicles ON vlfVehicleAssignment.VehicleId=vlfFleetVehicles.VehicleId" +
               " INNER JOIN vlfTxtMsgsNew ON vlfVehicleAssignment.BoxId=vlfTxtMsgsNew.BoxId" +
               " INNER JOIN vlfFleetUsers ON vlfFleetVehicles.FleetId=vlfFleetUsers.FleetId" +
               sqlWhere + " ORDER BY MsgDateTime DESC";
            // 2. Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("Unable to retrieve text messages.", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException("Unable to retrieve text messages. " + objException.Message);
         }
         // 3. Return result
         return sqlDataSet;
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
      /// <param name="msgDirection"></param>
      /// <returns>DataSet [VehicleId],[Description],[LicensePlate],
      /// [MsgId],[BoxId],[MsgDateTime],[MsgBody],[UserId],[Acknowledged]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetUserMessagesShortInfo(int userId, DateTime from, DateTime to, short msgDirection)
      {
         DataSet sqlDataSet = null;
         try
         {
            string sqlWhere = " WHERE vlfFleetUsers.UserId=" + userId;
            if ((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.In)
               sqlWhere += " AND (TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtText + " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial + ")";
            else if ((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.Out)
               sqlWhere += " AND TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.ClientText;
            else
               // Retrieve all visible to client messages
               sqlWhere += " AND (TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtText +
                  " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial +
                  " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.ClientText + ")";

            if (from != VLF.CLS.Def.Const.unassignedDateTime)
               sqlWhere += " AND MsgDateTime>='" + from + "'";
            if (to != VLF.CLS.Def.Const.unassignedDateTime)
               sqlWhere += " AND MsgDateTime<='" + to + "'";

            // 1. Prepares SQL statement
            string sql = "DECLARE @Timezone int" +
               " DECLARE @DayLightSaving int" +
               " SELECT @Timezone=PreferenceValue FROM vlfUserPreference" +
               " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
               " IF @Timezone IS NULL SET @Timezone=0" +
               " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
               " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
               " SET @Timezone= @Timezone + @DayLightSaving" +
               " SELECT DISTINCT vlfVehicleInfo.VehicleId,vlfVehicleInfo.Description,vlfVehicleAssignment.LicensePlate," +
               " DATEADD(hour,@Timezone,MsgDateTime) AS MsgDateTime," +
               " MsgId,vlfTxtMsgsNew.BoxId,substring(MsgBody,1,20) AS MsgBody,isnull(vlfTxtMsgsNew.UserId,-1) AS UserId,vlfTxtMsgsNew.Acknowledged" +
               " FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId" +
               " INNER JOIN vlfFleetVehicles ON vlfVehicleAssignment.VehicleId=vlfFleetVehicles.VehicleId" +
               " INNER JOIN vlfTxtMsgsNew ON vlfVehicleAssignment.BoxId=vlfTxtMsgsNew.BoxId" +
               " INNER JOIN vlfFleetUsers ON vlfFleetVehicles.FleetId=vlfFleetUsers.FleetId" +
               sqlWhere + " ORDER BY MsgDateTime DESC";
            // 2. Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("Unable to retrieve text messages.", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException("Unable to retrieve text messages. " + objException.Message);
         }
         // 3. Return result
         return sqlDataSet;
      }
      /// <summary>
      /// Deletes all text messages related to the box
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="where"></param>
      /// <returns>rows affected</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteBoxAllMsgs(int boxId, string where)
      {
         int rowsAffected = 0;
         try
         {
            // 1. Prepares SQL statement
            string sql = "DELETE FROM vlfTxtMsgsNew WHERE BoxId=" + boxId + where;
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
      /// Deletes all text messages related to this user
      /// </summary>
      /// <param name="userId"></param>
      /// <returns>rows affected</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteAllUserMsgs(int userId)
      {
         int rowsAffected = 0;
         try
         {
            // 1. Prepares SQL statement
            string sql = "DELETE FROM vlfTxtMsgsNew WHERE UserId=" + userId;
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
            string prefixMsg = "Unable to delete all user " + userId + " messages.";
            Util.ProcessDbException(prefixMsg, objException);
         }

         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to delete all user " + userId + " messages.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return rowsAffected;
      }
      /// <summary>
      /// Gets configuration parameter
      /// </summary>
      /// <param name="moduleName"></param>
      /// <param name="groupID"></param>
      /// <param name="paramName"></param>
      /// <param name="defaultValue"></param>
      /// <returns>string</returns>
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
            // TODO: log error
            //throw new VLF.ERR.DASException("'" + paramName + "' is not set in DB. Setting up default value: " + paramValue.ToString() + ".");
         }
         return paramValue;
      }

      public DataSet GetFleetTxtMsgs(int userId, int fleetId, DateTime fromDate, DateTime toDate, int msgDirection)
      {
         DataSet resultDataSet = null;
         sqlExec.ClearCommandParameters();
         try
         {
            sqlExec.AddCommandParam("@userID", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@fleetID", SqlDbType.Int, fleetId);
            sqlExec.AddCommandParam("@fromDateTime", SqlDbType.DateTime, fromDate);
            sqlExec.AddCommandParam("@toDateTime", SqlDbType.DateTime, toDate);
            sqlExec.AddCommandParam("@msgDirection", SqlDbType.Int, msgDirection);
            resultDataSet = sqlExec.SPExecuteDataset("sp_GetFleetTxtMsgsNew");
            return resultDataSet;

         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve text messages for fleetID " + fleetId;
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve text messages for fleetID " + fleetId;
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }


      public DataSet GetMDTFormsMessages(DateTime fromDate, DateTime toDate, int fleetId, int boxId, int formId)
      {
         DataSet resultDataSet = null;
         sqlExec.ClearCommandParameters();
         try
         {
            sqlExec.AddCommandParam("@FromDate", SqlDbType.DateTime, fromDate);
            sqlExec.AddCommandParam("@ToDate", SqlDbType.DateTime, toDate);
            sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
            sqlExec.AddCommandParam("@FormId", SqlDbType.Int, formId);
            resultDataSet = sqlExec.SPExecuteDataset("sp_GetMDTFormsMessages");
            return resultDataSet;

         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve MDT Form messages";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve MDT Form messages";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }


      /// <summary>
      /// Retrieves MDT FormSchema
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="formId"></param>

      public string GetMDTFormSchema(int organizationId, int formId)
      {
         string retResult = "";
         try
         {
            // 1. Prepares SQL statement
            string sql = " SELECT FormSchema FROM vlfMDTFormSchema" +
                  " WHERE OrganizationId=" + organizationId +
                  " AND FormId=" + formId;
            // 2. Executes SQL statement
            object obj = sqlExec.SQLExecuteScalar(sql);
            if (obj != null)
               retResult = Convert.ToString(obj);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("Unable to retrieve MDT Form Schema.", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException("Unable to retrieve MDT Form Schema." + objException.Message);
         }
         // 3. Return result
         return retResult;
      }
     
      /// <summary>
      /// Get MDT Manufacturer and Model
       /// </summary>
       /// <param name="MdtTypeId"></param>
       /// <returns></returns>
       public DataSet GetMDTInfoByTypeId(int MdtTypeId)
       {
           DataSet resultDataSet = null;
          
           try
           {
               resultDataSet = sqlExec.SQLExecuteDataset("select * from vlfMdtType where MdtTypeId=" + MdtTypeId);
               return resultDataSet;

           }
           catch (SqlException objException)
           {
               string prefixMsg = "Unable to retrieve MDT Types";
               Util.ProcessDbException(prefixMsg, objException);
           }
           catch (DASDbConnectionClosed exCnn)
           {
               throw new DASDbConnectionClosed(exCnn.Message);
           }
           catch (Exception objException)
           {
               string prefixMsg = "Unable to retrieve MDT Types";
               throw new DASException(prefixMsg + " " + objException.Message);
           }
           return resultDataSet;
       }
   }
#else
    /// <summary>
	/// Provides interface to vlfTxtMsgs table.
	/// </summary>
	public class TxtMsgs : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public TxtMsgs(SQLExecuter sqlExec): base ("vlfTxtMsgs",sqlExec)
		{
		}

      /// <summary>
      ///         
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="msgDateTime"></param>
      /// <param name="latitude"></param>
      /// <param name="longitude"></param>
      /// <param name="txtMsgTypeId"></param>
      /// <param name="msgBody"></param>
      /// <param name="msgDirection"></param>
      /// <param name="userId"></param>
      /// <param name="ack"></param>
      /// <returns></returns>
      public int AddMsg(int boxId, DateTime msgDateTime,
                        double latitude, double longitude,
                        short txtMsgTypeId, string msgBody, short msgDirection, int userId, string ack)
      {
         int rowsAffected = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				// Set SQL command
				string sql = "INSERT INTO vlfTxtMsgs( "  +
								"BoxId" +
								",MsgDateTime" +
                        ",Lat, Long" +
								",TxtMsgTypeId" +
								",MsgBody" +
								",MsgDirection"+
								",Acknowledged";
				if(userId != VLF.CLS.Def.Const.unassignedIntValue)
					sql += ",UserId";
				sql +=") VALUES ( @BoxId,@MsgDateTime,@Lat, @Long,@TxtMsgTypeId,@MsgBody,@MsgDirection,@Acknowledged";
				if(userId != VLF.CLS.Def.Const.unassignedIntValue)
					sql += ",@UserId";
            sql += ") Select Max(MsgId) from vlfTxtMsgs";  // SELECT CAST(scope_identity() AS int)
				sqlExec.ClearCommandParameters();
				// Add parameters to SQL statement
				sqlExec.AddCommandParam("@BoxId",SqlDbType.Int,boxId);
				sqlExec.AddCommandParam("@MsgDateTime",SqlDbType.DateTime,msgDateTime);
				sqlExec.AddCommandParam("@Lat",SqlDbType.Float,latitude);
				sqlExec.AddCommandParam("@Long",SqlDbType.Float,longitude);
				sqlExec.AddCommandParam("@TxtMsgTypeId",SqlDbType.SmallInt,txtMsgTypeId);
				if(msgBody == null)
					sqlExec.AddCommandParam("@MsgBody",SqlDbType.VarChar,System.DBNull.Value);
				else
					sqlExec.AddCommandParam("@MsgBody",SqlDbType.VarChar,msgBody);
				sqlExec.AddCommandParam("@MsgDirection",SqlDbType.SmallInt,msgDirection);
				sqlExec.AddCommandParam("@Acknowledged",SqlDbType.VarChar,ack);
				
				if(userId != VLF.CLS.Def.Const.unassignedIntValue)
					sqlExec.AddCommandParam("@UserId",SqlDbType.Int,userId);
				
				//Executes SQL statement
				object currentMsgId = sqlExec.SQLExecuteScalar(sql);
				if(currentMsgId != null)
					rowsAffected = Convert.ToInt32(currentMsgId);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add new text msg.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new text msg.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Add new text message
		/// </summary>
		/// <returns> current message id or -1 in case of error</returns>
		/// <param name="boxId"></param>
		/// <param name="msgDateTime"></param>
		/// <param name="txtMsgTypeId"></param>
		/// <param name="msgBody"></param>
		/// <param name="msgDirection"></param>
		/// <param name="userId"></param>
		/// <param name="ack"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Throws if data already exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int AddMsg(int boxId,DateTime msgDateTime,short txtMsgTypeId,string msgBody,short msgDirection,int userId,string ack)
		{
			int rowsAffected = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				// Set SQL command
				string sql = "INSERT INTO vlfTxtMsgs( "  +
								"BoxId" +
								",MsgDateTime" +
								",TxtMsgTypeId" +
								",MsgBody" +
								",MsgDirection"+
								",Acknowledged";
				if(userId != VLF.CLS.Def.Const.unassignedIntValue)
					sql += ",UserId";
				sql +=") VALUES ( @BoxId,@MsgDateTime,@TxtMsgTypeId,@MsgBody,@MsgDirection,@Acknowledged";
				if(userId != VLF.CLS.Def.Const.unassignedIntValue)
					sql += ",@UserId";
            sql += ") Select Max(MsgId) from vlfTxtMsgs"; // SELECT CAST(scope_identity() AS int)
				sqlExec.ClearCommandParameters();
				// Add parameters to SQL statement
				sqlExec.AddCommandParam("@BoxId",SqlDbType.Int,boxId);
				sqlExec.AddCommandParam("@MsgDateTime",SqlDbType.DateTime,msgDateTime);
				sqlExec.AddCommandParam("@TxtMsgTypeId",SqlDbType.SmallInt,txtMsgTypeId);
				if(msgBody == null)
					sqlExec.AddCommandParam("@MsgBody",SqlDbType.VarChar,System.DBNull.Value);
				else
					sqlExec.AddCommandParam("@MsgBody",SqlDbType.VarChar,msgBody);
				sqlExec.AddCommandParam("@MsgDirection",SqlDbType.SmallInt,msgDirection);
				sqlExec.AddCommandParam("@Acknowledged",SqlDbType.VarChar,ack);
				
				if(userId != VLF.CLS.Def.Const.unassignedIntValue)
					sqlExec.AddCommandParam("@UserId",SqlDbType.Int,userId);
				
				//Executes SQL statement
				object currentMsgId = sqlExec.SQLExecuteScalar(sql);
				if(currentMsgId != null)
					rowsAffected = Convert.ToInt32(currentMsgId);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add new text msg.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new text msg.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Save message response
		/// </summary>
		/// <param name="msgId"></param>
		/// <param name="respondDateTime"></param>
		/// <param name="msgResponse"></param>
		/// <returns></returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int SetMsgResponse(int msgId,DateTime respondDateTime,string msgResponse)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
					" SET ResponseDateTime='" + respondDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
					",MsgResponse='" +  msgResponse.Replace("'","''") + 
					"' WHERE MsgId=" + msgId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to update text message response. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to update text message response. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				throw new DASAppViolatedIntegrityConstraintsException("Unable to update text message response. ");
			}
			return rowsAffected;
		}
		/// <summary>
		/// Set owner foe the message
		/// </summary>
		/// <param name="msgId"></param>
		/// <param name="userId"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int SetMsgUserId(int msgId,int userId)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE vlfTxtMsgs SET UserId=" + userId + " WHERE MsgId=" + msgId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to update text message user id " + userId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to update text message user id " + userId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				throw new DASAppViolatedIntegrityConstraintsException("Unable to update text message user id. ");
			}
			return rowsAffected;
		}



        /// <summary>
        /// Set owner foe the message
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="userId"></param>
        /// <returns>rows affected</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int SetMsgUserIdExtended(int msgId, int userId, int msgTypeId, DateTime originDateTime, DateTime touchDateTime, int peripheralId,Int64  checksumId)
        {
            int rowsAffected = 0;
            try
            {
                //Prepares SQL statement
                string sql = "";
                if (Convert.ToInt16(msgTypeId)==(Int16)VLF.CLS.Def.Enums.PeripheralTypes.MDT) //MDT
                    sql = "UPDATE vlfTxtMsgs SET UserId=" + userId + " WHERE MsgId=" + msgId;
                else
                    sql = "UPDATE vlfPeripheralMessageHist SET UserId=" + userId + ", touchDateTime='" + touchDateTime + "', status='READ' WHERE checksum(originDateTime)+checksum(vlfPeripheralMessageHist.peripheralId)=" + checksumId;

                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update text message user id " + userId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update text message user id " + userId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            //Throws exception in case of wrong result
            if (rowsAffected == 0)
            {
                throw new DASAppViolatedIntegrityConstraintsException("Unable to update text message user id. ");
            }
            return rowsAffected;
        }

		/// <summary>
		/// Sets message ack
		/// </summary>
		/// <param name="msgId"></param>
		/// <param name="ack"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int SetMsgAck(int msgId,string ack)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE vlfTxtMsgs SET Acknowledged='" + ack + "' WHERE MsgId=" + msgId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to update text message ack " + ack + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to update text message ack " + ack + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				throw new DASAppViolatedIntegrityConstraintsException("Unable to update text message ack. ");
			}
			return rowsAffected;
		}

        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves messages by box id
        /// </summary>
        /// <remarks>
        /// all box Ids -> VLF.CLS.Def.Const.unassignedIntValue
        /// from DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// to DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
        /// 1. In messages directions -> VLF.CLS.Def.Enums.TxtMsgType.MdtText
        /// 2. Out messages directions -> VLF.CLS.Def.Enums.TxtMsgType.ClientText
        /// 3. Both messages directions -> MdtText,ClientText (all visible to client messages)
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="msgDirection"></param>
        /// <param name="tblLandmarks"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
        /// [Acknowledged],[UserName]
        /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetMessagesFullInfo_NewTZ(int userId, int boxId, DateTime from, DateTime to, short msgDirection)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sqlWhere = "";
                if ((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.In)
                    sqlWhere = " WHERE (TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtText + " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial + ")";
                else if ((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.Out)
                    sqlWhere = " WHERE TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.ClientText;
                else
                    // Retrieve all visible to client messages
                    sqlWhere = " WHERE (TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtText +
                                " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.ClientText +
                                " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial + ")";

                if (boxId != VLF.CLS.Def.Const.unassignedIntValue)
                    sqlWhere += " AND vlfTxtMsgs.BoxId=" + boxId;
                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlWhere += " AND MsgDateTime>='" + from + "'";
                if (to != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlWhere += " AND MsgDateTime<='" + to + "'";

                // 1. Prepares SQL statement
                //string sql = "DECLARE @ResolveLandmark int DECLARE @Timezone int" +
                //            " DECLARE @DayLightSaving int"+
                //            " SELECT @Timezone=PreferenceValue FROM vlfUserPreference"+
                //            " WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                //            " IF @Timezone IS NULL SET @Timezone=0"+
                //            " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                //            " IF @DayLightSaving IS NULL SET @DayLightSaving=0"+
                //            " SET @Timezone= @Timezone + @DayLightSaving"+
                //            " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +

                //        " SELECT DISTINCT vlfVehicleInfo.VehicleId, RTRIM(vlfVehicleAssignment.LicensePlate) as LicensePlate,"+

                //        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then RTRIM(vlfVehicleInfo.Description) ELSE"+
                //        " (SELECT RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) AS NameUser FROM vlfUser INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId WHERE vlfUser.UserId=vlfTxtMsgs.UserId)"+
                //        " END AS [From],"+

                //        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then RTRIM(vlfVehicleInfo.Description) ELSE"+
                //        " (SELECT RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) AS NameUser FROM vlfUser INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId WHERE vlfUser.UserId=vlfTxtMsgs.UserId)"+
                //        " END AS [To],"+

                //        //"CASE WHEN MsgDateTime IS NULL then '' ELSE convert(varchar,DATEADD(hour,@Timezone,MsgDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,MsgDateTime),108) END AS MsgDateTime,"+
                //        "DATEADD(hour,@Timezone,MsgDateTime) AS MsgDateTime,"+
                //        "MsgId,vlfTxtMsgs.BoxId,TxtMsgTypeId,RTRIM(MsgBody) as MsgBody,"+
                //        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then '" + VLF.CLS.Def.Const.MsgInDirectionSign + "'"+
                //            " ELSE CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then '" + VLF.CLS.Def.Const.MsgOutDirectionSign + 
                //            "' ELSE 'N/A' END END AS MsgDirection,"+
                //        " isnull(MsgResponse,'N/A') as MsgResponse,"+
                //        "CASE WHEN ResponseDateTime IS NULL THEN 'N/A' ELSE convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),108) END as ResponseDateTime,vlfTxtMsgs.UserId,Acknowledged,isnull(vlfUser.UserName,' ') AS UserName,"+
                //        "CASE WHEN MsgDirection=0 then ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') ELSE ISNULL(StreetAddress,'N/A') END AS StreetAddress," +

                //        "isnull(Latitude,0) as Latitude,"+ 
                //        "isnull(Longitude,0) as Longitude,"+
                //        "isnull(Speed,0) as Speed,"+ 
                //        "isnull(Heading,0) as Heading,"+
                //        "isnull(ValidGps,0) as ValidGps"+
                //        " FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId"+
                //        " INNER JOIN vlfTxtMsgs ON vlfVehicleAssignment.BoxId=vlfTxtMsgs.BoxId"+
                //        " LEFT JOIN  vlfMsgInHst ON vlfMsgInHst.BoxId=vlfTxtMsgs.BoxId AND vlfMsgInHst.OriginDateTime=vlfTxtMsgs.MsgDateTime"+
                //        " LEFT JOIN vlfUser ON vlfUser.UserId=vlfTxtMsgs.UserId"+
                //        sqlWhere + " ORDER BY vlfTxtMsgs.MsgDateTime DESC";



                string sql = "DECLARE @ResolveLandmark int DECLARE @Timezone float" +
                                " DECLARE @DayLightSaving int" +
                                " SELECT @Timezone=PreferenceValue FROM vlfUserPreference" +
                                " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                                " IF @Timezone IS NULL SET @Timezone=0" +
                                " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                                " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                                " SET @Timezone= @Timezone + @DayLightSaving" +
                                " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +

                      //  " SELECT DISTINCT vlfVehicleInfo.VehicleId, RTRIM(vlfVehicleAssignment.LicensePlate) as LicensePlate," +
                        " SELECT vlfVehicleInfo.VehicleId, RTRIM(vlfVehicleAssignment.LicensePlate) as LicensePlate," +

                        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then RTRIM(vlfVehicleInfo.Description) ELSE" +
                            " RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) END AS [From]," +

                        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then RTRIM(vlfVehicleInfo.Description) ELSE" +
                            " RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) END AS [To]," +

                        "CASE WHEN MsgDateTime IS NULL then '' ELSE convert(varchar,DATEADD(minute,(@Timezone * 60),MsgDateTime),101) + ' ' + convert(varchar,DATEADD(minute,(@Timezone * 60),MsgDateTime),108) END AS MsgDateTime," +
                            "DATEADD(minute,(@Timezone * 60),MsgDateTime) AS MsgDateTime," +
                            "MsgId,vlfTxtMsgs.BoxId,TxtMsgTypeId,RTRIM(MsgBody) as MsgBody," +
                            "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then '" + VLF.CLS.Def.Const.MsgInDirectionSign + "'" +
                                " ELSE CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then '" + VLF.CLS.Def.Const.MsgOutDirectionSign +
                                "' ELSE 'N/A' END END AS MsgDirection," +
                            " isnull(MsgResponse,'N/A') as MsgResponse," +
                            "CASE WHEN ResponseDateTime IS NULL THEN 'N/A' ELSE convert(varchar,DATEADD(minute,(@Timezone * 60),ResponseDateTime),101) + ' ' + convert(varchar,DATEADD(minute,(@Timezone * 60),ResponseDateTime),108) END as ResponseDateTime,vlfTxtMsgs.UserId,Acknowledged,isnull(vlfUser.UserName,' ') AS UserName," +
                    //"CASE WHEN MsgDirection=0 then ISNULL(vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') ELSE ISNULL(StreetAddress,'N/A') END AS StreetAddress," +
                        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then ISNULL(vlfTxtMsgs.LastAddress,'" + VLF.CLS.Def.Const.addressNA + "') ELSE 'N/A'  END AS StreetAddress," +
                        "isnull(Lat,0) as Latitude," +
                            "isnull(Long,0) as Longitude" +
                    // "isnull(Speed,0) as Speed," +
                    // "isnull(Heading,0) as Heading," +
                    // "isnull(ValidGps,0) as ValidGps" +
                            " FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId" +
                            " INNER JOIN vlfTxtMsgs ON vlfVehicleAssignment.BoxId=vlfTxtMsgs.BoxId" +
                    //" LEFT JOIN  vlfMsgInHst ON vlfMsgInHst.BoxId=vlfTxtMsgs.BoxId AND vlfMsgInHst.OriginDateTime=vlfTxtMsgs.MsgDateTime" +
                            " LEFT JOIN vlfUser ON vlfUser.UserId=vlfTxtMsgs.UserId" +
                            " LEFT JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId " +
                            sqlWhere + " ORDER BY vlfTxtMsgs.MsgDateTime DESC";

                // 2. Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retrieve text messages.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retrieve text messages. " + objException.Message);
            }
            // 3. Return result
            return sqlDataSet;
        }

        // Changes for TimeZone Feature start
		/// <summary>
		/// Retrieves messages by box id
		/// </summary>
		/// <remarks>
		/// all box Ids -> VLF.CLS.Def.Const.unassignedIntValue
		/// from DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
		/// to DateTime N/A -> VLF.CLS.Def.Const.unassignedDateTime
		/// 1. In messages directions -> VLF.CLS.Def.Enums.TxtMsgType.MdtText
		/// 2. Out messages directions -> VLF.CLS.Def.Enums.TxtMsgType.ClientText
		/// 3. Both messages directions -> MdtText,ClientText (all visible to client messages)
		/// </remarks>
		/// <param name="userId"></param>
		/// <param name="boxId"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="msgDirection"></param>
		/// <param name="tblLandmarks"></param>
		/// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
		/// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
		/// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
		/// [Acknowledged],[UserName]
        /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetMessagesFullInfo(int userId,int boxId,DateTime from, DateTime to,short msgDirection)
		{
			DataSet sqlDataSet = null;
			try
			{
				string sqlWhere = "";
				if((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.In)
					sqlWhere = " WHERE (TxtMsgTypeId=" +(short)VLF.CLS.Def.Enums.TxtMsgType.MdtText + " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial + ")";
				else if((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.Out)
					sqlWhere = " WHERE TxtMsgTypeId="+(short)VLF.CLS.Def.Enums.TxtMsgType.ClientText;
				else
					// Retrieve all visible to client messages
					sqlWhere = " WHERE (TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtText +
								" OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.ClientText + 
								" OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial + ")";

				if(boxId != VLF.CLS.Def.Const.unassignedIntValue)
					sqlWhere += " AND vlfTxtMsgs.BoxId=" + boxId;
				if(from != VLF.CLS.Def.Const.unassignedDateTime)
					sqlWhere += " AND MsgDateTime>='" + from + "'";
				if(to != VLF.CLS.Def.Const.unassignedDateTime)
					sqlWhere += " AND MsgDateTime<='" + to + "'";

                sqlWhere += " AND MsgBody not like '%@@@%'";

				// 1. Prepares SQL statement
                //string sql = "DECLARE @ResolveLandmark int DECLARE @Timezone int" +
                //            " DECLARE @DayLightSaving int"+
                //            " SELECT @Timezone=PreferenceValue FROM vlfUserPreference"+
                //            " WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                //            " IF @Timezone IS NULL SET @Timezone=0"+
                //            " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                //            " IF @DayLightSaving IS NULL SET @DayLightSaving=0"+
                //            " SET @Timezone= @Timezone + @DayLightSaving"+
                //            " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +

                //        " SELECT DISTINCT vlfVehicleInfo.VehicleId, RTRIM(vlfVehicleAssignment.LicensePlate) as LicensePlate,"+

                //        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then RTRIM(vlfVehicleInfo.Description) ELSE"+
                //        " (SELECT RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) AS NameUser FROM vlfUser INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId WHERE vlfUser.UserId=vlfTxtMsgs.UserId)"+
                //        " END AS [From],"+

                //        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then RTRIM(vlfVehicleInfo.Description) ELSE"+
                //        " (SELECT RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) AS NameUser FROM vlfUser INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId WHERE vlfUser.UserId=vlfTxtMsgs.UserId)"+
                //        " END AS [To],"+

                //        //"CASE WHEN MsgDateTime IS NULL then '' ELSE convert(varchar,DATEADD(hour,@Timezone,MsgDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,MsgDateTime),108) END AS MsgDateTime,"+
                //        "DATEADD(hour,@Timezone,MsgDateTime) AS MsgDateTime,"+
                //        "MsgId,vlfTxtMsgs.BoxId,TxtMsgTypeId,RTRIM(MsgBody) as MsgBody,"+
                //        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then '" + VLF.CLS.Def.Const.MsgInDirectionSign + "'"+
                //            " ELSE CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then '" + VLF.CLS.Def.Const.MsgOutDirectionSign + 
                //            "' ELSE 'N/A' END END AS MsgDirection,"+
                //        " isnull(MsgResponse,'N/A') as MsgResponse,"+
                //        "CASE WHEN ResponseDateTime IS NULL THEN 'N/A' ELSE convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),108) END as ResponseDateTime,vlfTxtMsgs.UserId,Acknowledged,isnull(vlfUser.UserName,' ') AS UserName,"+
                //        "CASE WHEN MsgDirection=0 then ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') ELSE ISNULL(StreetAddress,'N/A') END AS StreetAddress," +

                //        "isnull(Latitude,0) as Latitude,"+ 
                //        "isnull(Longitude,0) as Longitude,"+
                //        "isnull(Speed,0) as Speed,"+ 
                //        "isnull(Heading,0) as Heading,"+
                //        "isnull(ValidGps,0) as ValidGps"+
                //        " FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId"+
                //        " INNER JOIN vlfTxtMsgs ON vlfVehicleAssignment.BoxId=vlfTxtMsgs.BoxId"+
                //        " LEFT JOIN  vlfMsgInHst ON vlfMsgInHst.BoxId=vlfTxtMsgs.BoxId AND vlfMsgInHst.OriginDateTime=vlfTxtMsgs.MsgDateTime"+
                //        " LEFT JOIN vlfUser ON vlfUser.UserId=vlfTxtMsgs.UserId"+
                //        sqlWhere + " ORDER BY vlfTxtMsgs.MsgDateTime DESC";



                string sql = "DECLARE @ResolveLandmark int DECLARE @Timezone int" +
                                " DECLARE @DayLightSaving int" +
                                " SELECT @Timezone=PreferenceValue FROM vlfUserPreference" +
                                " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                                " IF @Timezone IS NULL SET @Timezone=0" +
                                " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                                " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                                " SET @Timezone= @Timezone + @DayLightSaving" +
                                " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +

                      //  " SELECT DISTINCT vlfVehicleInfo.VehicleId, RTRIM(vlfVehicleAssignment.LicensePlate) as LicensePlate," +
                        " SELECT vlfVehicleInfo.VehicleId, RTRIM(vlfVehicleAssignment.LicensePlate) as LicensePlate," +

                        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then RTRIM(vlfVehicleInfo.Description) ELSE" +
                            " RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) END AS [From]," +

                        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then RTRIM(vlfVehicleInfo.Description) ELSE" +
                            " RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) END AS [To]," +

                        "CASE WHEN MsgDateTime IS NULL then '' ELSE convert(varchar,DATEADD(hour,@Timezone,MsgDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,MsgDateTime),108) END AS MsgDateTime,"+
                            "DATEADD(hour,@Timezone,MsgDateTime) AS MsgDateTime," +
                            "MsgId,vlfTxtMsgs.BoxId,TxtMsgTypeId,RTRIM(MsgBody) as MsgBody," +
                            "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then '" + VLF.CLS.Def.Const.MsgInDirectionSign + "'" +
                                " ELSE CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then '" + VLF.CLS.Def.Const.MsgOutDirectionSign +
                                "' ELSE 'N/A' END END AS MsgDirection," +
                            " isnull(MsgResponse,'N/A') as MsgResponse," +
                            "CASE WHEN ResponseDateTime IS NULL THEN 'N/A' ELSE convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),108) END as ResponseDateTime,vlfTxtMsgs.UserId,Acknowledged,isnull(vlfUser.UserName,' ') AS UserName," +
                    //"CASE WHEN MsgDirection=0 then ISNULL(vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') ELSE ISNULL(StreetAddress,'N/A') END AS StreetAddress," +
                        "CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then ISNULL(vlfTxtMsgs.LastAddress,'" + VLF.CLS.Def.Const.addressNA + "') ELSE 'N/A'  END AS StreetAddress," +
                        "isnull(Lat,0) as Latitude," +
                            "isnull(Long,0) as Longitude" +
                           // "isnull(Speed,0) as Speed," +
                           // "isnull(Heading,0) as Heading," +
                           // "isnull(ValidGps,0) as ValidGps" +
                            " FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId" +
                            " INNER JOIN vlfTxtMsgs ON vlfVehicleAssignment.BoxId=vlfTxtMsgs.BoxId" +
                            //" LEFT JOIN  vlfMsgInHst ON vlfMsgInHst.BoxId=vlfTxtMsgs.BoxId AND vlfMsgInHst.OriginDateTime=vlfTxtMsgs.MsgDateTime" +
                            " LEFT JOIN vlfUser ON vlfUser.UserId=vlfTxtMsgs.UserId" +
                            " LEFT JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId " + 
                            sqlWhere + " ORDER BY vlfTxtMsgs.MsgDateTime DESC";

				// 2. Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retrieve text messages.", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retrieve text messages. " + objException.Message);
			}
			// 3. Return result
			return sqlDataSet;
		}
		/// <summary>
		/// Retrieves message info
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="msgId"></param>
		/// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
		/// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
		/// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
		/// [Acknowledged],[UserName],
		/// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetMessageFullInfo(int userId,int msgId)
		{
			DataSet sqlDataSet = null;
			try
			{
				// 1. Prepares SQL statement
                string sql = "DECLARE @ResolveLandmark int DECLARE @Timezone int" +
					" DECLARE @DayLightSaving int"+
					" SELECT @Timezone=PreferenceValue FROM vlfUserPreference"+
					" WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.TimeZone +
					" IF @Timezone IS NULL SET @Timezone=0"+
					" SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
					" IF @DayLightSaving IS NULL SET @DayLightSaving=0"+
					" SET @Timezone= @Timezone + @DayLightSaving"+
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +

                    " SELECT DISTINCT vlfVehicleInfo.VehicleId, RTRIM(vlfVehicleAssignment.LicensePlate) as LicensePlate,"+
					
					"CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then RTRIM(vlfVehicleInfo.Description) ELSE"+
					" (SELECT RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) AS NameUser FROM vlfUser INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId WHERE vlfUser.UserId=vlfTxtMsgs.UserId)"+
					" END AS [From],"+

					"CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then RTRIM(vlfVehicleInfo.Description) ELSE"+
					" (SELECT RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) AS NameUser FROM vlfUser INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId WHERE vlfUser.UserId=vlfTxtMsgs.UserId)"+
					" END AS [To],"+

					"DATEADD(hour,@Timezone,MsgDateTime) AS MsgDateTime,"+
					"MsgId,vlfTxtMsgs.BoxId,TxtMsgTypeId,RTRIM(MsgBody) as MsgBody,"+
					"CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then '" + VLF.CLS.Def.Const.MsgInDirectionSign + "'"+
					" ELSE CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then '" + VLF.CLS.Def.Const.MsgOutDirectionSign + 
					"' ELSE 'N/A' END END AS MsgDirection,"+
					" isnull(MsgResponse,'N/A') as MsgResponse,"+
					"CASE WHEN ResponseDateTime IS NULL THEN 'N/A' ELSE convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),108) END as ResponseDateTime,vlfTxtMsgs.UserId,vlfTxtMsgs.Acknowledged,isnull(vlfUser.UserName,' ') AS UserName,"+
                    "CASE WHEN MsgDirection=0 then ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') ELSE ISNULL(StreetAddress,'N/A') END AS StreetAddress," +
					"isnull(Latitude,0) as Latitude,"+ 
					"isnull(Longitude,0) as Longitude,"+
					"isnull(Speed,0) as Speed,"+ 
					"isnull(Heading,0) as Heading,"+
					"isnull(ValidGps,0) as ValidGps"+
					" FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId"+
					" INNER JOIN vlfTxtMsgs ON vlfVehicleAssignment.BoxId=vlfTxtMsgs.BoxId"+
                    " LEFT JOIN  vlfMsgInHst with (nolock) ON vlfMsgInHst.BoxId=vlfTxtMsgs.BoxId AND vlfMsgInHst.OriginDateTime=vlfTxtMsgs.MsgDateTime" +
					" LEFT JOIN vlfUser ON vlfUser.UserId=vlfTxtMsgs.UserId"+
					" WHERE MsgId=" + msgId +
					" ORDER BY vlfTxtMsgs.MsgDateTime DESC";
				
				// 2. Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
				
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retrieve text messages.", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retrieve text messages. " + objException.Message);
			}
			// 3. Return result
			return sqlDataSet;
		}

        /// <summary>
        /// Retrieves message Full info (including Garmin)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="msgId"></param>
        /// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
        /// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
        /// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
        /// [Acknowledged],[UserName],
        /// [StreetAddress],[Latitude],[Longitude],[Speed],[Heading]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserIncomingTextMessagesFullInfoSP(int userId, int msgId, int peripheralId, int msgTypeId, DateTime msgDateTime,int vehicleId)
        {
            DataSet sqlDataSet = null;
            try
            {

                ////Executes SQL statement
                sqlExec.ClearCommandParameters();
                string sql = "sp_GetIncomingMessageFullInfo";

                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@MsgId", SqlDbType.Int, msgId);
                sqlExec.AddCommandParam("@PeripheralId", SqlDbType.Int, peripheralId);
                sqlExec.AddCommandParam("@MsgTypeId", SqlDbType.Int, msgTypeId);
                sqlExec.AddCommandParam("@msgDateTime", SqlDbType.DateTime, msgDateTime);
                sqlExec.AddCommandParam("@VehicleId", SqlDbType.Int, vehicleId);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);

            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retrieve Full text messages info.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retrieve Full text messages info. " + objException.Message);
            }
            // 3. Return result
            return sqlDataSet;
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
		/// <param name="msgDirection"></param>
		/// <returns>DataSet [VehicleId],[LicensePlate],[From],[To],
		/// [MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],
		/// [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],
		/// [Acknowledged]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetUserMessagesFullInfo(int userId,DateTime from, DateTime to,short msgDirection)
		{
			DataSet sqlDataSet = null;
			try
			{
				string sqlWhere = " WHERE vlfFleetUsers.UserId=" + userId;
				if((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.In)
					sqlWhere += " AND (TxtMsgTypeId="+(short)VLF.CLS.Def.Enums.TxtMsgType.MdtText + " OR TxtMsgTypeId="+(short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial + ")";
				else if((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.Out)
					sqlWhere += " AND TxtMsgTypeId="+(short)VLF.CLS.Def.Enums.TxtMsgType.ClientText;
				else
					// Retrieve all visible to client messages
					sqlWhere += " AND (TxtMsgTypeId="+(short)VLF.CLS.Def.Enums.TxtMsgType.MdtText + 
										" OR TxtMsgTypeId="+(short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial + 
										" OR TxtMsgTypeId="+(short)VLF.CLS.Def.Enums.TxtMsgType.ClientText+ ")";

				if(from != VLF.CLS.Def.Const.unassignedDateTime)
					sqlWhere += " AND MsgDateTime>='" + from + "'";
				if(to != VLF.CLS.Def.Const.unassignedDateTime)
					sqlWhere += " AND MsgDateTime<='" + to + "'";
				
				// 1. Prepares SQL statement
				string sql = "DECLARE @Timezone int"+
					" DECLARE @DayLightSaving int"+
					" SELECT @Timezone=PreferenceValue FROM vlfUserPreference"+
					" WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.TimeZone +
					" IF @Timezone IS NULL SET @Timezone=0"+
					" SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
					" IF @DayLightSaving IS NULL SET @DayLightSaving=0"+
					" SET @Timezone= @Timezone + @DayLightSaving"+

                    " SELECT  DISTINCT vlfVehicleInfo.VehicleId,vlfVehicleAssignment.LicensePlate,"+

					"CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then RTRIM(vlfVehicleInfo.Description) ELSE"+
					" (SELECT RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) AS NameUser FROM vlfUser INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId WHERE vlfUser.UserId=vlfTxtMsgs.UserId)"+
					" END AS [From],"+

					"CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then RTRIM(vlfVehicleInfo.Description) ELSE"+
					" (SELECT RTRIM(vlfPersonInfo.FirstName) +' '+ RTRIM(vlfPersonInfo.LastName) AS NameUser FROM vlfUser INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId WHERE vlfUser.UserId=vlfTxtMsgs.UserId)"+
					" END AS [To],"+

					"DATEADD(hour,@Timezone,MsgDateTime) AS MsgDateTime,"+
					"MsgId,vlfTxtMsgs.BoxId,TxtMsgTypeId,MsgBody,"+
					"CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In + " then '" + VLF.CLS.Def.Const.MsgInDirectionSign + "'"+
						" ELSE CASE WHEN MsgDirection=" + (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.Out + " then '" + VLF.CLS.Def.Const.MsgOutDirectionSign + 
							"' ELSE 'N/A' END END AS MsgDirection,"+
					"isnull(MsgResponse,'N/A') as MsgResponse,"+
					"CASE WHEN ResponseDateTime IS NULL THEN 'N/A' ELSE convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,ResponseDateTime),108) END as ResponseDateTime,isnull(vlfTxtMsgs.UserId,-1) AS UserId,vlfTxtMsgs.Acknowledged"+
					" FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId"+
					" INNER JOIN vlfFleetVehicles ON vlfVehicleAssignment.VehicleId=vlfFleetVehicles.VehicleId"+
					" INNER JOIN vlfTxtMsgs ON vlfVehicleAssignment.BoxId=vlfTxtMsgs.BoxId"+
					" INNER JOIN vlfFleetUsers ON vlfFleetVehicles.FleetId=vlfFleetUsers.FleetId"+
					sqlWhere + " ORDER BY vlfTxtMsgs.MsgDateTime DESC";
				// 2. Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retrieve text messages.", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retrieve text messages. " + objException.Message);
			}
			// 3. Return result
			return sqlDataSet;
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
        /// <param name="msgDirection"></param>
        /// <returns>DataSet [VehicleId],[Description],[LicensePlate],
        /// [MsgId],[BoxId],[MsgDateTime],[MsgBody],[UserId],[Acknowledged]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserMessagesShortInfo_NewTZ(int userId, DateTime from, DateTime to, short msgDirection)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sqlWhere = " WHERE vlfFleetUsers.UserId=" + userId;
                if ((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.In)
                    sqlWhere += " AND (TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtText + " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial + ")";
                else if ((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.Out)
                    sqlWhere += " AND TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.ClientText;
                else
                    // Retrieve all visible to client messages
                    sqlWhere += " AND (TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtText +
                        " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial +
                        " OR TxtMsgTypeId=" + (short)VLF.CLS.Def.Enums.TxtMsgType.ClientText + ")";

                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlWhere += " AND MsgDateTime>='" + from + "'";
                if (to != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlWhere += " AND MsgDateTime<='" + to + "'";

                // 1. Prepares SQL statement
                string sql = "DECLARE @Timezone float" +
                    " DECLARE @DayLightSaving int" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference" +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +
               " SELECT DISTINCT vlfVehicleInfo.VehicleId,vlfVehicleInfo.Description,vlfVehicleAssignment.LicensePlate," +
                    " DATEADD(minute,(@Timezone * 60),MsgDateTime) AS MsgDateTime," +
                    " MsgId,vlfTxtMsgs.BoxId,substring(MsgBody,1,20) AS MsgBody,isnull(vlfTxtMsgs.UserId,-1) AS UserId,vlfTxtMsgs.Acknowledged" +
                    " FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId" +
                    " INNER JOIN vlfFleetVehicles ON vlfVehicleAssignment.VehicleId=vlfFleetVehicles.VehicleId" +
                    " INNER JOIN vlfTxtMsgs ON vlfVehicleAssignment.BoxId=vlfTxtMsgs.BoxId" +
                    " INNER JOIN vlfFleetUsers ON vlfFleetVehicles.FleetId=vlfFleetUsers.FleetId" +
                    sqlWhere + " ORDER BY vlfTxtMsgs.MsgDateTime DESC";
                // 2. Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retrieve text messages.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retrieve text messages. " + objException.Message);
            }
            // 3. Return result
            return sqlDataSet;
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
		/// <param name="msgDirection"></param>
		/// <returns>DataSet [VehicleId],[Description],[LicensePlate],
		/// [MsgId],[BoxId],[MsgDateTime],[MsgBody],[UserId],[Acknowledged]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetUserMessagesShortInfo(int userId,DateTime from, DateTime to,short msgDirection)
		{
			DataSet sqlDataSet = null;
			try
			{
				string sqlWhere = " WHERE vlfFleetUsers.UserId=" + userId;
				if((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.In)
					sqlWhere += " AND (TxtMsgTypeId="+(short)VLF.CLS.Def.Enums.TxtMsgType.MdtText + " OR TxtMsgTypeId="+(short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial + ")";
				else if((VLF.CLS.Def.Enums.TxtMsgDirectionType)msgDirection == VLF.CLS.Def.Enums.TxtMsgDirectionType.Out)
					sqlWhere += " AND TxtMsgTypeId="+(short)VLF.CLS.Def.Enums.TxtMsgType.ClientText;
				else
					// Retrieve all visible to client messages
					sqlWhere += " AND (TxtMsgTypeId="+(short)VLF.CLS.Def.Enums.TxtMsgType.MdtText+
						" OR TxtMsgTypeId="+(short)VLF.CLS.Def.Enums.TxtMsgType.MdtSpecial +
						" OR TxtMsgTypeId="+(short)VLF.CLS.Def.Enums.TxtMsgType.ClientText+ ")";

				if(from != VLF.CLS.Def.Const.unassignedDateTime)
					sqlWhere += " AND MsgDateTime>='" + from + "'";
				if(to != VLF.CLS.Def.Const.unassignedDateTime)
					sqlWhere += " AND MsgDateTime<='" + to + "'";
				
				// 1. Prepares SQL statement
				string sql = "DECLARE @Timezone int"+
					" DECLARE @DayLightSaving int"+
					" SELECT @Timezone=PreferenceValue FROM vlfUserPreference"+
					" WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.TimeZone +
					" IF @Timezone IS NULL SET @Timezone=0"+
					" SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
					" IF @DayLightSaving IS NULL SET @DayLightSaving=0"+
					" SET @Timezone= @Timezone + @DayLightSaving"+
               " SELECT DISTINCT vlfVehicleInfo.VehicleId,vlfVehicleInfo.Description,vlfVehicleAssignment.LicensePlate,"+
					" DATEADD(hour,@Timezone,MsgDateTime) AS MsgDateTime,"+
					" MsgId,vlfTxtMsgs.BoxId,substring(MsgBody,1,20) AS MsgBody,isnull(vlfTxtMsgs.UserId,-1) AS UserId,vlfTxtMsgs.Acknowledged"+
					" FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId"+
					" INNER JOIN vlfFleetVehicles ON vlfVehicleAssignment.VehicleId=vlfFleetVehicles.VehicleId"+
					" INNER JOIN vlfTxtMsgs ON vlfVehicleAssignment.BoxId=vlfTxtMsgs.BoxId"+
					" INNER JOIN vlfFleetUsers ON vlfFleetVehicles.FleetId=vlfFleetUsers.FleetId"+
					sqlWhere	+ " ORDER BY vlfTxtMsgs.MsgDateTime DESC";
				// 2. Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retrieve text messages.", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retrieve text messages. " + objException.Message);
			}
			// 3. Return result
			return sqlDataSet;
		}

        // Changes for TimeZone Feature start

        public DataSet GetIncomingMessagesShortInfoSP_NewTZ(DateTime from, DateTime to, int requestUserId)
        {
            DataSet sqlDataSet = null;
            try
            {

                ////Executes SQL statement
                sqlExec.ClearCommandParameters();
                string sql = "sp_GetIncomingMessagesShortInfo_NewTimeZone";


                if (requestUserId != VLF.CLS.Def.Const.unassignedIntValue)
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, requestUserId);
                else
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, VLF.CLS.Def.Const.unassignedIntValue);

                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                else
                    sqlExec.AddCommandParam("1/1/1999", SqlDbType.DateTime, to);


                if (to != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                else
                    sqlExec.AddCommandParam("1/1/2099", SqlDbType.DateTime, to);

                sqlDataSet = sqlExec.SPExecuteDataset(sql);

            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retrieve text messages.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retrieve text messages. " + objException.Message);
            }
            return sqlDataSet;
        }

        // Changes for TimeZone Feature end



        public DataSet GetIncomingMessagesShortInfoSP(DateTime from, DateTime to, int requestUserId)
        {
            DataSet sqlDataSet = null;
            try
            {

                ////Executes SQL statement
                sqlExec.ClearCommandParameters();
                string sql = "sp_GetIncomingMessagesShortInfo";


                if (requestUserId != VLF.CLS.Def.Const.unassignedIntValue)
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, requestUserId);
                else
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, VLF.CLS.Def.Const.unassignedIntValue);

                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                else
                    sqlExec.AddCommandParam("1/1/1999", SqlDbType.DateTime, to);


                if (to != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                else
                    sqlExec.AddCommandParam("1/1/2099", SqlDbType.DateTime, to);

                sqlDataSet = sqlExec.SPExecuteDataset(sql);
              
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to retrieve text messages.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to retrieve text messages. " + objException.Message);
            }
            return sqlDataSet;
        }

        // Changes for TimeZone Feature start
        public string GetMessagesShortInfoCheckSum_NewTZ(DateTime from, DateTime to, int requestUserId)
        {
            string checkSum = "";
            try
            {

                ////Executes SQL statement
                sqlExec.ClearCommandParameters();
                string sql = "sp_GetMessagesShortInfoCheckSum_NewTimeZone";


                if (requestUserId != VLF.CLS.Def.Const.unassignedIntValue)
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, requestUserId);
                else
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, VLF.CLS.Def.Const.unassignedIntValue);

                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                else
                    sqlExec.AddCommandParam("1/1/1999", SqlDbType.DateTime, to);


                if (to != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                else
                    sqlExec.AddCommandParam("1/1/2099", SqlDbType.DateTime, to);

                object obj = sqlExec.SPExecuteScalar(sql);
                if (obj != System.DBNull.Value)
                    checkSum = Convert.ToString(obj);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve messages checksum from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve messages checksum from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return checkSum;
        }
        // Changes for TimeZone Feature end

        public string GetMessagesShortInfoCheckSum(DateTime from, DateTime to, int requestUserId)
        {
            string checkSum = "";
            try
            {

                ////Executes SQL statement
                sqlExec.ClearCommandParameters();
                string sql = "sp_GetMessagesShortInfoCheckSum";


                if (requestUserId != VLF.CLS.Def.Const.unassignedIntValue)
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, requestUserId);
                else
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, VLF.CLS.Def.Const.unassignedIntValue);

                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                else
                    sqlExec.AddCommandParam("1/1/1999", SqlDbType.DateTime, to);


                if (to != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                else
                    sqlExec.AddCommandParam("1/1/2099", SqlDbType.DateTime, to);

                object obj = sqlExec.SPExecuteScalar(sql);
                if (obj != System.DBNull.Value)
                    checkSum = Convert.ToString(obj);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve messages checksum from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve messages checksum from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return checkSum;
        }

		/// <summary>
		/// Deletes all text messages related to the box
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="where"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteBoxAllMsgs(int boxId,string where)
		{
			int rowsAffected = 0;
			try
			{
				// 1. Prepares SQL statement
				string sql = "DELETE FROM vlfTxtMsgs WHERE BoxId=" + boxId + where;
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
		/// Deletes all text messages related to this user
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteAllUserMsgs(int userId)
		{
			int rowsAffected = 0;
			try
			{
				// 1. Prepares SQL statement
				string sql = "DELETE FROM vlfTxtMsgs WHERE UserId=" + userId;
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
				string prefixMsg = "Unable to delete all user " + userId + " messages.";
				Util.ProcessDbException(prefixMsg,objException);
			}

			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete all user " + userId + " messages.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Gets configuration parameter
		/// </summary>
		/// <param name="moduleName"></param>
		/// <param name="groupID"></param>
		/// <param name="paramName"></param>
		/// <param name="defaultValue"></param>
		/// <returns>string</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		private string GetConfigParameter( string moduleName, short groupID, string paramName, string defaultValue)
		{
			string paramValue = defaultValue;
			DB.Configuration config = new DB.Configuration(sqlExec);

			// take Module ID in DB
			short moduleID = config.GetConfigurationModuleTypeId(moduleName);
			if( moduleID == VLF.CLS.Def.Const.unassignedShortValue )
			{
				throw new VLF.ERR.DASAppResultNotFoundException("Cannot find '" + moduleName + "' in DB." );
			}

			// get parameter from DB
			try	
			{ 
				paramValue = config.GetConfigurationValue(moduleID,groupID,paramName); 
			}
			catch 
			{	
				// TODO: log error
				//throw new VLF.ERR.DASException("'" + paramName + "' is not set in DB. Setting up default value: " + paramValue.ToString() + ".");
			}
			return paramValue;
		}

        // Changes for TimeZone Feature start
        public DataSet GetFleetTxtMsgs_NewTZ(int userId, int fleetId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            DataSet resultDataSet = null;
            sqlExec.ClearCommandParameters();
            try
            {
                sqlExec.AddCommandParam("@userID", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fleetID", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDateTime", SqlDbType.DateTime, fromDate);
                sqlExec.AddCommandParam("@toDateTime", SqlDbType.DateTime, toDate);
                sqlExec.AddCommandParam("@msgDirection", SqlDbType.Int, msgDirection);
                resultDataSet = sqlExec.SPExecuteDataset("sp_GetFleetTxtMsgs_NewTimeZone");
                return resultDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve text messages for fleetID " + fleetId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve text messages for fleetID " + fleetId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }
        // Changes for TimeZone Feature end


        public DataSet GetFleetTxtMsgs(int userId, int fleetId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            DataSet resultDataSet = null;
            sqlExec.ClearCommandParameters(); 
            try
            {
                sqlExec.AddCommandParam("@userID", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fleetID", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDateTime", SqlDbType.DateTime, fromDate);
                sqlExec.AddCommandParam("@toDateTime", SqlDbType.DateTime, toDate);
                sqlExec.AddCommandParam("@msgDirection", SqlDbType.Int, msgDirection);
                resultDataSet = sqlExec.SPExecuteDataset("sp_GetFleetTxtMsgs");
                return resultDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve text messages for fleetID " + fleetId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve text messages for fleetID " + fleetId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        // Changes for TimeZone Feature start
        public DataSet GetFleetAllTxtMsgs_NewTZ(int userId, int fleetId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            DataSet resultDataSet = null;
            sqlExec.ClearCommandParameters();
            try
            {
                sqlExec.AddCommandParam("@userID", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fleetID", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDateTime", SqlDbType.DateTime, fromDate);
                sqlExec.AddCommandParam("@toDateTime", SqlDbType.DateTime, toDate);
                sqlExec.AddCommandParam("@msgDirection", SqlDbType.Int, msgDirection);
                resultDataSet = sqlExec.SPExecuteDataset("sp_GetFleetAllTxtMsgs_NewTimeZone");
                return resultDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve text messages for fleetID " + fleetId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve text messages for fleetID " + fleetId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }




        // Changes for TimeZone Feature end

        public DataSet GetFleetAllTxtMsgs(int userId, int fleetId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            DataSet resultDataSet = null;
            sqlExec.ClearCommandParameters();
            try
            {
                sqlExec.AddCommandParam("@userID", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fleetID", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDateTime", SqlDbType.DateTime, fromDate);
                sqlExec.AddCommandParam("@toDateTime", SqlDbType.DateTime, toDate);
                sqlExec.AddCommandParam("@msgDirection", SqlDbType.Int, msgDirection);
                resultDataSet = sqlExec.SPExecuteDataset("sp_GetFleetAllTxtMsgs");
                return resultDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve text messages for fleetID " + fleetId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve text messages for fleetID " + fleetId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        // Changes for TimeZone Feature start

        public DataSet GetFleetAllDestinations_NewTZ(int userId, int fleetId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            DataSet resultDataSet = null;
            sqlExec.ClearCommandParameters();
            try
            {
                sqlExec.AddCommandParam("@userID", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fleetID", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDateTime", SqlDbType.DateTime, fromDate);
                sqlExec.AddCommandParam("@toDateTime", SqlDbType.DateTime, toDate);
                sqlExec.AddCommandParam("@msgDirection", SqlDbType.Int, msgDirection);
                resultDataSet = sqlExec.SPExecuteDataset("sp_GetFleetAllDestinations_NewTimeZone");
                return resultDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Destinationsfor fleetID " + fleetId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Destinations for fleetID " + fleetId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }


        // Changes for TimeZone Feature end

        public DataSet GetFleetAllDestinations(int userId, int fleetId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            DataSet resultDataSet = null;
            sqlExec.ClearCommandParameters();
            try
            {
                sqlExec.AddCommandParam("@userID", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@fleetID", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@fromDateTime", SqlDbType.DateTime, fromDate);
                sqlExec.AddCommandParam("@toDateTime", SqlDbType.DateTime, toDate);
                sqlExec.AddCommandParam("@msgDirection", SqlDbType.Int, msgDirection);
                resultDataSet = sqlExec.SPExecuteDataset("sp_GetFleetAllDestinations");
                return resultDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Destinationsfor fleetID " + fleetId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Destinations for fleetID " + fleetId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        // Changes for TimeZone Feature start

        public DataSet GetVehicleAllTxtMsgs_NewTZ(int userId, int boxId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            DataSet resultDataSet = null;
            sqlExec.ClearCommandParameters();
            try
            {
                sqlExec.AddCommandParam("@userID", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@boxId", SqlDbType.Int, @boxId);
                sqlExec.AddCommandParam("@fromDateTime", SqlDbType.DateTime, fromDate);
                sqlExec.AddCommandParam("@toDateTime", SqlDbType.DateTime, toDate);
                sqlExec.AddCommandParam("@msgDirection", SqlDbType.Int, msgDirection);
                resultDataSet = sqlExec.SPExecuteDataset("sp_GetVehicleAllTxtMsgs_NewTimeZone");
                return resultDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve text messages for boxId " + boxId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve text messages for boxId " + boxId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }



        // Changes for TimeZone Feature end


        public DataSet GetVehicleAllTxtMsgs(int userId, int boxId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            DataSet resultDataSet = null;
            sqlExec.ClearCommandParameters();
            try
            {
                sqlExec.AddCommandParam("@userID", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@boxId", SqlDbType.Int, @boxId);
                sqlExec.AddCommandParam("@fromDateTime", SqlDbType.DateTime, fromDate);
                sqlExec.AddCommandParam("@toDateTime", SqlDbType.DateTime, toDate);
                sqlExec.AddCommandParam("@msgDirection", SqlDbType.Int, msgDirection);
                resultDataSet = sqlExec.SPExecuteDataset("sp_GetVehicleAllTxtMsgs");
                return resultDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve text messages for boxId " + boxId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve text messages for boxId " + boxId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        // Changes for TimeZone Feature start

        public DataSet GetVehicleAllDestinations_NewTZ(int userId, int boxId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            DataSet resultDataSet = null;
            sqlExec.ClearCommandParameters();
            try
            {
                sqlExec.AddCommandParam("@userID", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@boxId", SqlDbType.Int, @boxId);
                sqlExec.AddCommandParam("@fromDateTime", SqlDbType.DateTime, fromDate);
                sqlExec.AddCommandParam("@toDateTime", SqlDbType.DateTime, toDate);
                sqlExec.AddCommandParam("@msgDirection", SqlDbType.Int, msgDirection);
                resultDataSet = sqlExec.SPExecuteDataset("sp_GetVehicleAllDestinations_NewTimeZone");
                return resultDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Destinations for boxId " + boxId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Destinations for boxId " + boxId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }


        // Changes for TimeZone Feature end

        public DataSet GetVehicleAllDestinations(int userId, int boxId, DateTime fromDate, DateTime toDate, int msgDirection)
        {
            DataSet resultDataSet = null;
            sqlExec.ClearCommandParameters();
            try
            {
                sqlExec.AddCommandParam("@userID", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@boxId", SqlDbType.Int, @boxId);
                sqlExec.AddCommandParam("@fromDateTime", SqlDbType.DateTime, fromDate);
                sqlExec.AddCommandParam("@toDateTime", SqlDbType.DateTime, toDate);
                sqlExec.AddCommandParam("@msgDirection", SqlDbType.Int, msgDirection);
                resultDataSet = sqlExec.SPExecuteDataset("sp_GetVehicleAllDestinations");
                return resultDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Destinations for boxId " + boxId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Destinations for boxId " + boxId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }


      public DataSet GetMDTFormsMessages(DateTime fromDate, DateTime toDate, int fleetId,int boxId,int formId)
      {
         DataSet resultDataSet = null;
         sqlExec.ClearCommandParameters();
         try
         {
            sqlExec.AddCommandParam("@FromDate", SqlDbType.DateTime, fromDate);
            sqlExec.AddCommandParam("@ToDate", SqlDbType.DateTime, toDate);
            sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
            sqlExec.AddCommandParam("@FormId", SqlDbType.Int, formId);
            resultDataSet = sqlExec.SPExecuteDataset("sp_GetMDTFormsMessages");
            return resultDataSet;

         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve MDT Form messages";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve MDT Form messages";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }


      /// <summary>
      /// Retrieves MDT FormSchema
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="formId"></param>

      public string GetMDTFormSchema(int organizationId, int formId)
      {
         string retResult = "";
         try
         {
            // 1. Prepares SQL statement
            string sql = " SELECT FormSchema FROM vlfMDTFormSchema" +
                  " WHERE OrganizationId=" + organizationId +
                  " AND FormId=" + formId;
            // 2. Executes SQL statement
            object obj = sqlExec.SQLExecuteScalar(sql);
            if (obj != null)
               retResult = Convert.ToString(obj);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("Unable to retrieve MDT Form Schema.", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException("Unable to retrieve MDT Form Schema." + objException.Message);
         }
         // 3. Return result
         return retResult;
      }


      /// <summary>
      /// Get MDT Manufacturer and Model
      /// </summary>
      /// <param name="MdtTypeId"></param>
      /// <returns></returns>
      public DataSet GetMDTInfoByTypeId(int MdtTypeId)
      {
          DataSet resultDataSet = null;

          try
          {
              resultDataSet = sqlExec.SQLExecuteDataset("select * from vlfMdtType where MdtTypeId=" + MdtTypeId);
              return resultDataSet;

          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve MDT Types";
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve MDT Types";
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return resultDataSet;
      }
	}
#endif



}
