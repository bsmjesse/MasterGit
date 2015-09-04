using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interface to vlfPersonInfo table.
	/// </summary>
	public class Preference : TblOneIntPrimaryKey
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public Preference(SQLExecuter sqlExec) : base ("vlfPreference",sqlExec)
		{
		}
		/// <summary>
		/// Add new preference.
		/// </summary>
		/// <param name="preferenceName"></param>
		/// <param name="preferenceRule"></param>
		/// <returns>int next preference id</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if preference alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int AddPreference(string preferenceName,string preferenceRule)
		{
			int preferenceId = VLF.CLS.Def.Const.unassignedIntValue;
			// 1. validates parameters
			if(	(preferenceName == VLF.CLS.Def.Const.unassignedStrValue)||
				(preferenceRule == VLF.CLS.Def.Const.unassignedStrValue))
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: preference name=" + 
					preferenceRule + " preference rule=" + preferenceRule);
			}

			int rowsAffected = 0;
			// 2. Get next availible index
			preferenceId = (int)GetMaxRecordIndex("preferenceId") + 1;
			
			// 3. Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName 
				+ " (PreferenceId,PreferenceName,PreferenceRule)"
				+ " VALUES ( {0}, '{1}', '{2}')",
				preferenceId,preferenceName.Replace("'","''"),preferenceRule.Replace("'","''"));
			try
			{
				// 4. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add new organization.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new preference.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new preference.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This preference already exists.");
			}
			return preferenceId;
		}		
		/// <summary>
		/// Deletes existing preference.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="preferenceName"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if preference does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeletePreferenceByPreferenceName(string preferenceName)
		{
			return DeleteRowsByStrField("PreferenceName",preferenceName, "preference name");		
		}
		/// <summary>
		/// Deletes existing preference.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="preferenceId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if preference id does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeletePreferenceByPreferenceId(int preferenceId)
		{
			return DeleteRowsByIntField("PreferenceId",preferenceId, "preference id");		
		}		
		/// <summary>
		/// Retrieves Preference info
		/// </summary>
		/// <returns>DataSet [PreferenceId], [PreferenceName],[PreferenceRule]</returns>
		/// <param name="preferenceId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetPreferenceInfo(int preferenceId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT * FROM " + tableName + 
					" WHERE PreferenceId=" + preferenceId;
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve info by preference Id=" + preferenceId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve info by preference Id=" + preferenceId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
	}
}
