using System;
using System.Data;
using System.Data.SqlClient;
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfBoxCmdOutType table.
	/// </summary>
	public class BoxCmdOutType : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public BoxCmdOutType(SQLExecuter sqlExec) : base ("vlfBoxCmdOutType",sqlExec)
		{
		}
		/// <summary>
		/// Add new box command out type.
		/// </summary>
		/// <param name="boxCmdOutTypeId"></param>
		/// <param name="boxCmdOutTypeName"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if command already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddRecord(short boxCmdOutTypeId,string boxCmdOutTypeName)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = string.Format("INSERT INTO vlfBoxCmdOutType ( BoxCmdOutTypeId, BoxCmdOutTypeName) VALUES ( {0}, '{1}')", boxCmdOutTypeId, boxCmdOutTypeName.Replace("'","''"));
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
				string prefixMsg = "Unable to add new command '" + boxCmdOutTypeName + "'.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new command '" + boxCmdOutTypeName + "'.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new command '" + boxCmdOutTypeName + "'.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The command already exists.");
			}
		}	
		/// <summary>
		/// Delete exist box command out type by name.
		/// </summary>
		/// <param name="boxCmdOutTypeName"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecord(string boxCmdOutTypeName)
		{
			return DeleteRowsByStrField("BoxCmdOutTypeName",boxCmdOutTypeName, "command out type");
		}
		/// <summary>
		/// Delete exist box command out type by Id
		/// </summary>
		/// <param name="boxCmdOutTypeId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecord(short boxCmdOutTypeId)
		{
			return DeleteRecord( GetNameById(boxCmdOutTypeId) );
		}
		/// <summary>
		/// retrieves box command out type name by id from "vlfBoxCmdOutType" table
		/// </summary>
		/// <param name="boxCmdOutTypeId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <returns>command name</returns>
		public string GetNameById(short boxCmdOutTypeId)
		{
			string resultValue = VLF.CLS.Def.Const.unassignedStrValue;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT BoxCmdOutTypeName FROM " + tableName + " WHERE BoxCmdOutTypeId=" + boxCmdOutTypeId;
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
				string prefixMsg = "Unable to retrieve command type name by id=" + boxCmdOutTypeId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve command type name by id=" + boxCmdOutTypeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultValue;
		}
	}
}
