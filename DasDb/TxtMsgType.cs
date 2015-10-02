using System;
using System.Data;
using System.Data.SqlClient;
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfTxtMsgType table.
	/// </summary>
	public class TxtMsgType : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public TxtMsgType(SQLExecuter sqlExec) : base ("vlfTxtMsgType",sqlExec)
		{
		}
		/// <summary>
		/// Adds new box MsgIn type.
		/// </summary>
		/// <param name="txtMsgTypeId"></param>
		/// <param name="txtMsgTypeName"></param>
		/// <param name="txtDispMsgTypeName"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddRecord(short txtMsgTypeId,string txtMsgTypeName,string txtDispMsgTypeName)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = string.Format("INSERT INTO vlfTxtMsgType ( TxtMsgTypeId, TxtMsgTypeName, DisplaiedTxtMsgTypeName) VALUES ( {0}, '{1}', '{2}')", txtMsgTypeId, txtMsgTypeName.Replace("'","''"),txtDispMsgTypeName.Replace("'","''"));
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
				string prefixMsg = "Unable to add new text message type '" + txtMsgTypeName + ":'" + txtDispMsgTypeName + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new text message type '" + txtMsgTypeName + ":'" + txtDispMsgTypeName + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new text message type '" + txtMsgTypeName + ":'" + txtDispMsgTypeName + ".";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The text message type already exists.");
			}
		}	
		/// <summary>
		/// Deletes existing text message type by name.
		/// </summary>
		/// <param name="txtMsgTypeName"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecord(string txtMsgTypeName)
		{
			return DeleteRowsByStrField("TxtMsgTypeName",txtMsgTypeName, "text message type");
		}
		/// <summary>
		/// Deletes existing text message type by Id
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="txtMsgTypeId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecord(short txtMsgTypeId)
		{
			return DeleteRecord( GetNameById(txtMsgTypeId) );
		}
		/// <summary>
		/// Retrieves text message type name by id from "vlfTxtMsgType" table
		/// Throws exception in case of wrong result (see Tbl2UniqueFields class).
		/// </summary>
		/// <param name="txtMsgTypeId"></param>
		/// <returns>name</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetNameById(short txtMsgTypeId)
		{
			string resultValue = VLF.CLS.Def.Const.unassignedStrValue;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT TxtMsgTypeName FROM " + tableName + " WHERE TxtMsgTypeId=" + txtMsgTypeId;
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
				string prefixMsg = "Unable to retrieve text message type name by id=" + txtMsgTypeId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve text message type name by id=" + txtMsgTypeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultValue;
		}
		/// <summary>
		/// Retrieves text message type name by id from "vlfTxtMsgType" table
		/// </summary>
		/// <param name="txtMsgTypeId"></param>
		/// <returns>DataSet [TxtMsgTypeId],[TxtMsgTypeName],[DisplaiedTxtMsgTypeName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetTxtMsgTypeInfoById(short txtMsgTypeId)
		{
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT * FROM " + tableName + " WHERE TxtMsgTypeId=" + txtMsgTypeId;
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve text message type info by id=" + txtMsgTypeId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve text message type info by id=" + txtMsgTypeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}
	}
}


