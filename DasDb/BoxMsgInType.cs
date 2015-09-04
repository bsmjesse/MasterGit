using System;
using System.Data;
using System.Data.SqlClient;
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfBoxMsgInType table.
	/// </summary>
	public class BoxMsgInType : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public BoxMsgInType(SQLExecuter sqlExec) : base ("vlfBoxMsgInType",sqlExec)
		{
		}
		/// <summary>
		/// Add new box MsgIn type.
		/// </summary>
		/// <param name="boxMsgInTypeId"></param>
		/// <param name="boxMsgInTypeName"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddRecord(short boxMsgInTypeId,string boxMsgInTypeName)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = string.Format("INSERT INTO vlfBoxMsgInType ( BoxMsgInTypeId, BoxMsgInTypeName) VALUES ( {0}, '{1}')", boxMsgInTypeId, boxMsgInTypeName.Replace("'","''"));
			try
			{
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
				string prefixMsg = "Unable to add new command '" + boxMsgInTypeName + "'.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new command '" + boxMsgInTypeName + "'.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new command '" + boxMsgInTypeName + "'.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The command already exists.");
			}
		}	
		/// <summary>
		/// Delete exist box Msg In type by name.
		/// </summary>
		/// <param name="boxMsgInTypeName"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecord(string boxMsgInTypeName)
		{
			return DeleteRowsByStrField("BoxMsgInTypeName",boxMsgInTypeName, "message in type");
		}
		/// <summary>
		/// Delete exist box Msg In type by Id
		/// </summary>
		/// <param name="boxMsgInTypeId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecord(short boxMsgInTypeId)
		{
			return DeleteRecord( GetNameById(boxMsgInTypeId) );
		}
		/// <summary>
		/// Retrieves box MsgIn type name by id from "vlfBoxMsgInType" table
		/// </summary>
		/// <param name="boxMsgInTypeId"></param>
		/// <returns>returns message name by id</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetNameById(short boxMsgInTypeId)
		{
			string resultValue = VLF.CLS.Def.Const.unassignedStrValue;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT BoxMsgInTypeName FROM " + tableName + " WHERE BoxMsgInTypeId=" + boxMsgInTypeId;
				//Executes SQL statement
				DataSet resultDataSet = sqlExec.SQLExecuteDataset(sql);
				if(resultDataSet != null && resultDataSet.Tables.Count > 0 && resultDataSet.Tables[0].Rows.Count == 1)
				{
					//Trim speces at the end of result (all char fields in the database have fixed size)
					resultValue = Convert.ToString(resultDataSet.Tables[0].Rows[0][0]).TrimEnd();
				}
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve msgIn type name by id=" + boxMsgInTypeId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve msgIn type name by id=" + boxMsgInTypeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultValue;
		}
        /// <summary>
        /// Set DayLight Savings.
        /// </summary>
        /// <returns>void</returns>
        /// <param name="dayLightSaving"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void SetDayLightSaving(bool dayLightSaving)
        {
            // 1. Prepares SQL statement
            string sql = "UPDATE vlfMsgNotification SET DayLightSaving=" + Convert.ToInt16(dayLightSaving) + " WHERE AutoAdjustDayLightSaving=1";
            try
            {
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update vlfMsgNotification.dayLightSaving=" + dayLightSaving.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update vlfMsgNotification.dayLightSaving=" + dayLightSaving.ToString() + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }
        /// <summary>
        /// Get message notification recipient information
        /// </summary>
        /// <returns>[OrganizationId],[BoxMsgInTypeId],[SensorId],[MsgDetails],[Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        /// <param name="boxId"></param>
        /// <param name="boxMsgInTypeId"></param>
        /// <param name="sensorId"></param>
        /// <param name="msgDetails"></param>
        public DataSet GetMsgNotificationRecipientInfo(int boxId, short boxMsgInTypeId, short sensorId, string msgDetails)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = "SELECT vlfMsgNotification.OrganizationId, vlfMsgNotification.BoxMsgInTypeId, vlfMsgNotification.SensorId, vlfMsgNotification.MsgDetails, vlfMsgNotification.Email, vlfMsgNotification.TimeZone, vlfMsgNotification.DayLightSaving, vlfMsgNotification.AutoAdjustDayLightSaving FROM vlfMsgNotification INNER JOIN vlfBox with (nolock) ON vlfMsgNotification.OrganizationId = vlfBox.OrganizationId WHERE vlfMsgNotification.BoxMsgInTypeId=" + boxMsgInTypeId + " AND vlfBox.BoxId=" + boxId;
                if (sensorId != VLF.CLS.Def.Const.unassignedShortValue)
                    sql += " AND SensorId=" + sensorId;
                if (msgDetails != VLF.CLS.Def.Const.unassignedStrValue && msgDetails != "")
                    sql += " AND MsgDetails like '%" + msgDetails + "%'";
                
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve all unassigned boxes. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve all unassigned boxes. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// <summary>
        /// Get Box MsgIn Types
        /// </summary>
        /// <returns>[BoxMsgInTypeId],[BoxMsgInTypeName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
   
        public DataSet GetBoxMsgInTypes()
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = "SELECT BoxMsgInTypeId,BoxMsgInTypeName from vlfBoxMsgInType where VisibleWeb=1";
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve BoxMsgInTypes. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve all unassigned boxes. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
	}
}
