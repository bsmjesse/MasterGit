using System;
using VLF.ERR;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Setup/retrieve communication mode settings
	/// </summary>
	public class CommMode : Tbl2UniqueFields
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public CommMode(SQLExecuter sqlExec) : base ("vlfCommMode",sqlExec)
		{
		}
		/// <summary>
		/// Add new communication mode.
		/// </summary>
		/// <param name="commModeName"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddRecord(string commModeName)
		{
			AddNewRow("CommModeId","CommModeName",commModeName,"communication mode");
		}
		/// <summary>
		/// Delete exist communication mode by name.
		/// </summary>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <param name="commModeName"></param>
		/// <returns>rows affected</returns>
		public int DeleteRecord(string commModeName)
		{
			return DeleteRowsByStrField("CommModeName",commModeName, "communication mode");
		}
		/// <summary>
		/// Delete exist communication mode by Id
		/// </summary>
		/// <param name="commModeId"></param> 
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecord(short commModeId)
		{
			return DeleteRecord( GetNameById(commModeId) );
		}
		/// <summary>
		/// Retrieves record count of "vlfCommMode" table
		/// </summary>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 RecordCount
		{
			get
			{
				return GetRecordCount("CommModeId");
			}
		}
		/// <summary>
		/// Retrieves max record index from "vlfCommMode" table
		/// </summary>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 MaxRecordIndex
		{
			get
			{
				return GetMaxRecordIndex("CommModeId");
			}
		}
		/// <summary>
		/// Retrieves communication mode by id from "vlfCommMode" table
		/// </summary>
		/// <param name="commModeId"></param>
		/// <returns>communication name</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetNameById(short commModeId)
		{
			return GetFieldValueByRowId("CommModeId",commModeId,"CommModeName","communication mode");
		}
	}
}
