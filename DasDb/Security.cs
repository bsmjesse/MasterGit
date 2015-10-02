using System;
using System.Collections;	// for ArrayList
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to security tables.
	/// </summary>
	public class Security : TblGenInterfaces
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlExec"></param>
		public Security(SQLExecuter sqlExec) : base ("SECURITY",sqlExec)
		{
		}
		/// <summary>
		/// Chacks box-user authorization
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="userId"></param>
		/// <returns>true if authorized, otherwise false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool CheckBoxUserAuthorization(int boxId,int userId)
		{
			bool retResult = false;
			try
			{
				// 1. Prepares SQL statement
            string sql = "SELECT COUNT(*) FROM vlfBox with (nolock) INNER JOIN vlfOrganization ON vlfBox.OrganizationId = vlfOrganization.OrganizationId INNER JOIN vlfUser ON vlfOrganization.OrganizationId = vlfUser.OrganizationId WHERE vlfBox.BoxId=" + boxId + " AND vlfUser.UserId=" + userId;
				// 3. Executes SQL statement
				if( Convert.ToInt16(sqlExec.SQLExecuteScalar(sql)) > 0)
					retResult = true;

			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve box-user authorization by box id=" + boxId + " user id=" + userId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve box-user authorization by box id=" + boxId + " user id=" + userId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}	
	}
}
