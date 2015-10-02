using System;
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfStateProvince table.
	/// </summary>
	public class StateProvince : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public StateProvince(SQLExecuter sqlExec) : base ("vlfStateProvince",sqlExec)
		{
		}
		/// <summary>
		/// Add new State/Province.
		/// </summary>
		/// <param name="stateProvinceName"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddStateProvince(string stateProvinceName)
		{
			int rowsAffected = 0;
			string prefixMsg = "Unable to add new '" + stateProvinceName + "' state/province.";
			//Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName + " (StateProvince) VALUES ('{0}')", stateProvinceName.Replace("'","''"));
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The state/province already exists.");
			}
		}	
		/// <summary>
		/// Deletes exist State/Province.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="stateProvinceName"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteStateProvince(string stateProvinceName)
		{
			return DeleteRowsByStrField("StateProvince",stateProvinceName, "state/province name");
		}
		/// <summary>
		/// Retrieves record count of "vlfStateProvince" table
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
