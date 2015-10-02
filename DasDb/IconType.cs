using System;
using System.Data;
using System.Data.SqlClient;
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfVehicleType table.
	/// </summary>
	public class IconType : Tbl2UniqueFields
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public IconType(SQLExecuter sqlExec) : base ("vlfIconType",sqlExec)
		{
		}
		/// <summary>
		/// Retrieves icons information
		/// </summary>
		/// <returns>DataSet [IconTypeId],[IconTypeName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetIconsInfo()
		{
			DataSet dsResult = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT * FROM vlfIconType ORDER BY IconTypeName";
				//Executes SQL statement
				dsResult = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve icons information. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve icons information. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return dsResult;
		}
	}
}
