using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfCommModeAddressType table
	/// </summary>
	public class CommModeAddressType  : TblConnect2TblsWithoutRules
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public CommModeAddressType(SQLExecuter sqlExec) : base ("vlfCommModeAddressType",sqlExec)
		{
		}
		/// <summary>
		/// Add new row that connected communication mode and address type
		/// </summary>
		/// <param name="commModeId"></param>
		/// <param name="addressTypeId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddRecord(short commModeId,short addressTypeId)
		{
			AddNewRow("CommModeId",commModeId,"CommAddressTypeId",addressTypeId,"communication mode address type");
		}	
		/// <summary>
		/// Delete exist communication mode by Id
		/// </summary>
		/// <param name="commModeId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecordByCommMode(short commModeId)
		{
			return DeleteRowsByIntField("CommModeId",commModeId, "communication mode address type");
		}
		/// <summary>
		/// Delete exist communication address type by Id
		/// </summary>
		/// <param name="commAddressTypeId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecordByCommAddressType(short commAddressTypeId)
		{
			return DeleteRowsByIntField("CommAddressTypeId",commAddressTypeId, "communication mode address type");
		}
		/// <summary>
		/// Delete exist CommModeAddressType
		/// </summary>
		/// <param name="commModeId"></param> 
		/// <param name="commAddressTypeId"></param> 
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecordByCommModeAddressType(short commModeId,short commAddressTypeId)
		{
			return DeleteRowsByFields("CommModeId",commModeId,"CommAddressTypeId",commAddressTypeId,"communication mode address type");	
		}
		/// <summary>
		/// Retrieves record count from vlfCommModeAddressType table.
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
		/// <summary>
		/// Get communication address types by communication mode. 
		/// </summary>
		/// <param name="commModeId"></param>
		/// <returns>DataSet [CommAddressTypeId],[CommAddressTypeName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetCommModeAddressTypesInfo(short commModeId)
		{
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfCommAddressType.CommAddressTypeId,vlfCommAddressType.CommAddressTypeName" +
					" FROM vlfCommAddressType,vlfCommModeAddressType" +
					" WHERE vlfCommModeAddressType.CommModeId=" + commModeId +
					" AND vlfCommModeAddressType.CommAddressTypeId=vlfCommAddressType.CommAddressTypeId" + 
					" ORDER BY vlfCommAddressType.CommAddressTypeId";
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve communication address types by communication mode id " + commModeId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve communication address types by communication mode id " + commModeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}	
	}
}
