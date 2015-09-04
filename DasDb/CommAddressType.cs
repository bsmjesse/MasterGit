using System;
using VLF.ERR;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Setup/Retrieve communication address type settings
	/// </summary>
	public class CommAddressType : Tbl2UniqueFields
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public CommAddressType(SQLExecuter sqlExec) : base ("vlfCommAddressType",sqlExec)
		{
		}
		/// <summary>
		/// Add new communication address type.
		/// </summary>
		/// <param name="commAddressTypeName"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddRecord(string commAddressTypeName)
		{
			AddNewRow("CommAddressTypeId","CommAddressTypeName",commAddressTypeName,"communication address type");
		}	
		/// <summary>
		/// Delete exist communication address type by name.
		/// </summary>
		/// <param name="commAddressTypeName"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecord(string commAddressTypeName)
		{
			return DeleteRowsByStrField("CommAddressTypeName",commAddressTypeName, "communication address type");
		}
		/// <summary>
		/// Delete exist communication address type by Id
		/// </summary>
		/// <param name="commAddressTypeId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecord(short commAddressTypeId)
		{
			return DeleteRecord( GetNameById(commAddressTypeId) );
		}
		/// <summary>
		/// Retrieves record count of "vlfCommAddressType" table
		/// </summary>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 RecordCount
		{
			get
			{
				return GetRecordCount("CommAddressTypeId");
			}
		}
		/// <summary>
		/// Retrieves max record index from "vlfCommAddressType" table
		/// </summary>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 MaxRecordIndex
		{
			get
			{
				return GetMaxRecordIndex("CommAddressTypeId");
			}
		}
		/// <summary>
		/// Retrieves communication address type by id from "vlfCommAddressType" table
		/// </summary>
		/// <param name="commAddressTypeId"></param>
		/// <returns>communication address type name by id</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetNameById(short commAddressTypeId)
		{
			return GetFieldValueByRowId("CommAddressTypeId",commAddressTypeId,"CommAddressTypeName","communication address type");
		}
	}
}
