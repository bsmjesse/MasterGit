using System;
using System.Data;
using VLF.ERR;
using VLF.CLS;
using System.Data.SqlClient ;	// for SqlException
namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interface to vlfSystemUpdates table.
	/// </summary>
	public class SystemUpdates : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public SystemUpdates(SQLExecuter sqlExec) : base ("vlfSystemUpdates",sqlExec)
		{
		}
		/// <summary>
		/// Adds new system update
		/// </summary>
		/// <param name="systemUpdateDateTime"></param>
		/// <param name="msg"></param>
		/// <param name="systemUpdateType"></param>
		/// <param name="severity"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user with this datetime already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int AddSystemUpdate(DateTime systemUpdateDateTime,
									string msg,string msgFr,
									VLF.CLS.Def.Enums.SystemUpdateType systemUpdateType,
									VLF.CLS.Def.Enums.AlarmSeverity severity,string FontColor,Int16 FontBold)
		{
			int rowsAffected = 0;
			try
			{
				// 3. Prepares SQL statement
				string sql = string.Format("INSERT INTO vlfSystemUpdates"
                    + " (MsgDateTime,Msg,SystemUpdateType,AlarmLevel,FontColor,FontBold,MsgFR)"
                    + " VALUES ( '{0}', '{1}', {2}, {3},'{4}',{5}, '{6}')",
					systemUpdateDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"),
					msg.Replace("'","''"),
					(short)systemUpdateType,
					(short)severity,
                    FontColor, FontBold, msgFr.Replace("'", "''"));
			
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to add system update. ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to add system update. " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				throw new DASAppDataAlreadyExistsException("Unable to add system update.");
			}
			return rowsAffected;
		}


        /// <summary>
        /// Update system update table
        /// </summary>
        /// <param name="MsgId"></param>
        /// <param name="systemUpdateDateTime"></param>
        /// <param name="msg"></param>
        /// <param name="systemUpdateType"></param>
        /// <param name="severity"></param>
        /// <param name="FontColor"></param>
        /// <param name="FontBold"></param>
        /// 
        /// <returns>rows affected</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if user with this datetime already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int UpdateSystemUpdateTable(int MsgId,DateTime systemUpdateDateTime,
                                    string msg,string msgFr,
                                    VLF.CLS.Def.Enums.SystemUpdateType systemUpdateType,
                                    VLF.CLS.Def.Enums.AlarmSeverity severity, string FontColor, Int16 FontBold)
        {
            int rowsAffected = 0;
            try
            {
                // 3. Prepares SQL statement
                string sql = string.Format("Update vlfSystemUpdates set MsgDateTime='{0}',Msg='{1}',SystemUpdateType={2},AlarmLevel={3},FontColor='{4}',FontBold={5},MsgFR='{6}' where MsgId={7}",
                    systemUpdateDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff"),
                    msg.Replace("'", "''"),
                    (short)systemUpdateType,
                    (short)severity,
                    FontColor, FontBold, msgFr.Replace("'", "''"), MsgId);

                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to add system update. ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to add system update. " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                throw new DASAppDataAlreadyExistsException("Unable to add system update.");
            }
            return rowsAffected;
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves system updates.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="systemUpdateType"></param>
        /// <returns>DataSet [MsgId],[Msg],[SystemUpdateType],[AlarmLevel]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetSystemUpdates_NewTZ(int userId, DateTime from, DateTime to, VLF.CLS.Def.Enums.SystemUpdateType systemUpdateType)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sqlWhere = "";
                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere = " WHERE MsgDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (to != VLF.CLS.Def.Const.unassignedDateTime)
                        sqlWhere += " AND MsgDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                        sqlWhere += " AND SystemUpdateType=" + (short)systemUpdateType;
                }
                else if (to != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere = " WHERE MsgDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                        sqlWhere += " AND SystemUpdateType=" + (short)systemUpdateType;
                }
                else if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                {
                    sqlWhere = " WHERE SystemUpdateType=" + (short)systemUpdateType;
                }

                //Prepares SQL statement               

                string sql = "DECLARE @Timezone float" +
                            " DECLARE @DayLightSaving int" +
                            " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                            " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                   " IF @Timezone IS NULL SET @Timezone=0" +
                   " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                   " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                   " SET @Timezone= @Timezone + @DayLightSaving" +

                   " SELECT MsgId,Msg,MsgFr,SystemUpdateType,AlarmLevel as Severity," +
                   "CASE WHEN MsgDateTime IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),MsgDateTime) END AS MsgDateTime" +
                   " FROM vlfSystemUpdates" +
                   sqlWhere +
                   " ORDER BY MsgDateTime DESC";

                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to get system update information.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to get system update information. " + objException.Message);
            }
            return sqlDataSet;
        }

        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves system updates.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="systemUpdateType"></param>
        /// <returns>DataSet [MsgId],[Msg],[SystemUpdateType],[AlarmLevel]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetSystemUpdates(int userId, DateTime from, DateTime to, VLF.CLS.Def.Enums.SystemUpdateType systemUpdateType)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sqlWhere = "";
                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere = " WHERE MsgDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (to != VLF.CLS.Def.Const.unassignedDateTime)
                        sqlWhere += " AND MsgDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                        sqlWhere += " AND SystemUpdateType=" + (short)systemUpdateType;
                }
                else if (to != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere = " WHERE MsgDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                        sqlWhere += " AND SystemUpdateType=" + (short)systemUpdateType;
                }
                else if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                {
                    sqlWhere = " WHERE SystemUpdateType=" + (short)systemUpdateType;
                }

                //Prepares SQL statement
                string sql = "DECLARE @Timezone int" +
                    " DECLARE @DayLightSaving int" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +

                    " SELECT MsgId,Msg,MsgFr,SystemUpdateType,AlarmLevel as Severity," +
                    "CASE WHEN MsgDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,MsgDateTime) END AS MsgDateTime" +
                    " FROM vlfSystemUpdates" +
                    sqlWhere +
                    " ORDER BY MsgDateTime DESC";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to get system update information.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to get system update information. " + objException.Message);
            }
            return sqlDataSet;
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves system updates.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="systemUpdateType"></param>
        /// <returns>DataSet [MsgId],[Msg],[SystemUpdateType],[AlarmLevel]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetSystemUpdatesByLang_NewTZ(int userId, DateTime from, DateTime to, VLF.CLS.Def.Enums.SystemUpdateType systemUpdateType, string lang)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sqlWhere = "";
                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere = " WHERE MsgDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (to != VLF.CLS.Def.Const.unassignedDateTime)
                        sqlWhere += " AND MsgDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                        sqlWhere += " AND SystemUpdateType=" + (short)systemUpdateType;
                }
                else if (to != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere = " WHERE MsgDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                        sqlWhere += " AND SystemUpdateType=" + (short)systemUpdateType;
                }
                else if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                {
                    sqlWhere = " WHERE SystemUpdateType=" + (short)systemUpdateType;
                }

                //Prepares SQL statement

                string sql = "DECLARE @Timezone float" +
                            " DECLARE @DayLightSaving int" +
                            " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                            " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +

                    " SELECT MsgId,Msg" + (lang != "en" ? lang.ToUpper() + " AS Msg" : "") + ",SystemUpdateType,AlarmLevel as Severity," +
                    "CASE WHEN MsgDateTime IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),MsgDateTime) END AS MsgDateTime,FontColor,FontBold " +
                    " FROM vlfSystemUpdates" +
                    sqlWhere +
                    " ORDER BY MsgDateTime DESC";


                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to get system update information.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to get system update information. " + objException.Message);
            }
            return sqlDataSet;
        }

        // Changes for TimeZone Feature end
        /// <summary>
        /// Retrieves system updates.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="systemUpdateType"></param>
        /// <returns>DataSet [MsgId],[Msg],[SystemUpdateType],[AlarmLevel]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetSystemUpdatesByLang(int userId, DateTime from, DateTime to, VLF.CLS.Def.Enums.SystemUpdateType systemUpdateType, string lang)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sqlWhere = "";
                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere = " WHERE MsgDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (to != VLF.CLS.Def.Const.unassignedDateTime)
                        sqlWhere += " AND MsgDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                        sqlWhere += " AND SystemUpdateType=" + (short)systemUpdateType;
                }
                else if (to != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere = " WHERE MsgDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                        sqlWhere += " AND SystemUpdateType=" + (short)systemUpdateType;
                }
                else if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                {
                    sqlWhere = " WHERE SystemUpdateType=" + (short)systemUpdateType;
                }

                //Prepares SQL statement
                string sql = "DECLARE @Timezone int" +
                    " DECLARE @DayLightSaving int" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +

                    " SELECT MsgId,Msg" + (lang != "en" ? lang.ToUpper() + " AS Msg" : "") + ",SystemUpdateType,AlarmLevel as Severity," +
                    "CASE WHEN MsgDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,MsgDateTime) END AS MsgDateTime,FontColor,FontBold " +
                    " FROM vlfSystemUpdates" +
                    sqlWhere +
                    " ORDER BY MsgDateTime DESC";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to get system update information.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to get system update information. " + objException.Message);
            }
            return sqlDataSet;
        }
		/// <summary>
		/// Deletes system update.
		/// </summary>
		/// <param name="msgId"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteSystemUpdate(int msgId)
		{
			return DeleteRowsByIntField("MsgId",msgId,"message id");
		}


        // Changes for TimeZone Feature start
        /// Retrieves system updates.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="systemUpdateType"></param>
        /// <returns>DataSet [MsgId],[Msg],[SystemUpdateType],[AlarmLevel]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetFullInfoSystemUpdates_NewTZ(int userId, DateTime from, DateTime to, VLF.CLS.Def.Enums.SystemUpdateType systemUpdateType)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sqlWhere = "";
                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere = " WHERE MsgDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (to != VLF.CLS.Def.Const.unassignedDateTime)
                        sqlWhere += " AND MsgDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                        sqlWhere += " AND (SystemUpdateType=0 OR SystemUpdateType=" + (short)systemUpdateType + ")";
                }
                else if (to != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere = " WHERE MsgDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                        sqlWhere += " AND (SystemUpdateType=0 OR SystemUpdateType=" + (short)systemUpdateType + ")";
                }
                else if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                {
                    sqlWhere = " WHERE SystemUpdateType=" + (short)systemUpdateType;
                }

                //Prepares SQL statement
                string sql = "DECLARE @Timezone float" +
                    " DECLARE @DayLightSaving int" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +

                    " SELECT MsgId,Msg,MsgFr,SystemUpdateType,AlarmLevel as Severity," +
                    "CASE WHEN MsgDateTime IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),MsgDateTime) END AS MsgDateTime,isnull(FontColor,'') as FontColor,isnull(FontBold,0) as FontBold " +
                    " FROM vlfSystemUpdates" +
                    sqlWhere +
                    " ORDER BY SystemUpdateType DESC,MsgDateTime DESC";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to get system update information.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to get system update information. " + objException.Message);
            }
            return sqlDataSet;
        }
        // Changes for TimeZone Feature end


        /// Retrieves system updates.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="systemUpdateType"></param>
        /// <returns>DataSet [MsgId],[Msg],[SystemUpdateType],[AlarmLevel]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetFullInfoSystemUpdates(int userId, DateTime from, DateTime to, VLF.CLS.Def.Enums.SystemUpdateType systemUpdateType)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sqlWhere = "";
                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere = " WHERE MsgDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (to != VLF.CLS.Def.Const.unassignedDateTime)
                        sqlWhere += " AND MsgDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                        sqlWhere += " AND (SystemUpdateType=0 OR SystemUpdateType=" + (short)systemUpdateType + ")";
                }
                else if (to != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere = " WHERE MsgDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                        sqlWhere += " AND (SystemUpdateType=0 OR SystemUpdateType=" + (short)systemUpdateType + ")";
                }
                else if (systemUpdateType != VLF.CLS.Def.Enums.SystemUpdateType.All)
                {
                    sqlWhere = " WHERE SystemUpdateType=" + (short)systemUpdateType;
                }

                //Prepares SQL statement
                string sql = "DECLARE @Timezone int" +
                    " DECLARE @DayLightSaving int" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +

                    " SELECT MsgId,Msg,MsgFr,SystemUpdateType,AlarmLevel as Severity," +
                    "CASE WHEN MsgDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,MsgDateTime) END AS MsgDateTime,isnull(FontColor,'') as FontColor,isnull(FontBold,0) as FontBold " +
                    " FROM vlfSystemUpdates" +
                    sqlWhere +
                    " ORDER BY SystemUpdateType DESC,MsgDateTime DESC";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to get system update information.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to get system update information. " + objException.Message);
            }
            return sqlDataSet;
        }
    }
}
