using System;
using VLF.ERR;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Setup/Retrieve box protocol type settings
	/// </summary>
	public class BoxProtocolType : Tbl2UniqueFields
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public BoxProtocolType(SQLExecuter sqlExec) : base ("vlfBoxProtocolType",sqlExec)
		{
		}
		/// <summary>
		/// Add new box protocol type.
		/// </summary>
		/// <param name="boxProtocolTypeName"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		//public void AddRecord(string boxProtocolTypeName)
		//{
		//	AddNewRow("BoxProtocolTypeId","BoxProtocolTypeName",boxProtocolTypeName,"protocol type");
		//}
		/// <summary>
		/// Delete exist box protocol type by name.
		/// </summary>
		/// <param name="boxProtocolTypeName"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecord(string boxProtocolTypeName)
		{
			return DeleteRowsByStrField("BoxProtocolTypeName",boxProtocolTypeName, "protocol type");
		}
		/// <summary>
		/// Delete exist box hardware type by Id
		/// </summary>
		/// <param name="boxProtocolTypeId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecord(short boxProtocolTypeId)
		{
			return DeleteRecord( GetNameById(boxProtocolTypeId) );
		}
		/// <summary>
		/// Retrieves record count of "vlfBoxProtocolType" table
		/// </summary>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 RecordCount
		{
			get
			{
				return GetRecordCount("BoxProtocolTypeId");
			}
		}

		/// <summary>
		/// Retrieves max record index from "vlfBoxProtocolType" table
		/// </summary>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 MaxRecordIndex
		{
			get
			{
				return GetMaxRecordIndex("BoxProtocolTypeId");
			}
		}
		/// <summary>
		/// Retrieves box Protocol type name by id from "vlfBoxProtocolType" table
		/// </summary>
		/// <param name="boxProtocolTypeId"></param>
		/// <returns>returns protocol type name by id</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetNameById(short boxProtocolTypeId)
		{
			return GetFieldValueByRowId("BoxProtocolTypeId",boxProtocolTypeId,"BoxProtocolTypeName","protocol type");
		}
	}
}
