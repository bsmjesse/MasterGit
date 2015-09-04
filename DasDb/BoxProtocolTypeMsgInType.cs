using System;
using System.Data.SqlClient;
using System.Data;
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfBoxProtocolTypeMsgInType table.
	/// </summary>
	public class BoxProtocolTypeMsgInType : TblConnect2TblsWithRules
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public BoxProtocolTypeMsgInType(SQLExecuter sqlExec) : base ("vlfBoxProtocolTypeMsgInType",sqlExec)
		{
		}
		/// <summary>
		/// Add new row that connected box protocol type with message in type
		/// </summary>
		/// <param name="boxMsgInTypeId"></param>
		/// <param name="boxProtocolTypeId"></param>
		/// <param name="rules"></param>
		/// <param name="msgInTypeLen"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddRecord(short boxMsgInTypeId,short boxProtocolTypeId,string rules,short msgInTypeLen)
		{
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "INSERT INTO vlfBoxProtocolTypeMsgInType (BoxMsgInTypeId,BoxProtocolTypeId,Rules,MsgInTypeLen)"+
				" VALUES ( " + boxMsgInTypeId + ", " + boxProtocolTypeId + ",'" + rules.Replace("'","''") + "'," +  msgInTypeLen + ")";
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add new msgIn typeId=" + boxMsgInTypeId + " boxProtocolTypeId=" + boxProtocolTypeId + " rules=" + rules + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new msgIn typeId=" + boxMsgInTypeId + " boxProtocolTypeId=" + boxProtocolTypeId + " rules=" + rules + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new msgIn typeId=" + boxMsgInTypeId + " boxProtocolTypeId=" + boxProtocolTypeId + " rules=" + rules + ".";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The protocol type with msg out type for current box already exists.");
			}
		}	
		/// <summary>
		/// Delete exist box message in type by Id
		/// </summary>
		/// <param name="boxMsgInTypeId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecordByBoxMsgInType(short boxMsgInTypeId)
		{
			return DeleteRowsByIntField("BoxMsgInTypeId",boxMsgInTypeId, "box message in type");
		}
		/// <summary>
		/// Delete exist box protocol type by Id
		/// </summary>
		/// <param name="boxProtocolTypeId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecordByBoxProtocolType(short boxProtocolTypeId)
		{
			return DeleteRowsByIntField("BoxProtocolTypeId",boxProtocolTypeId, "box protocol type");
		}
		/// <summary>
		/// Delete exist box protocol type msg in type.
		/// </summary>
		/// <param name="boxMsgInTypeId"></param>
		/// <param name="boxProtocolTypeId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecordByBoxProtocolTypeMsgInType(short boxMsgInTypeId,short boxProtocolTypeId)
		{
			return DeleteRowsByFields("BoxMsgInTypeId",boxMsgInTypeId,"BoxProtocolTypeId",boxProtocolTypeId,"box protocol type message in type");	
		}
		/// <summary>
		/// Retrieves record count from vlfBoxProtocolTypeMsgInType table.
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
	}
}
