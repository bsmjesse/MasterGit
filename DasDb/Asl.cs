using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfAsl table.
	/// Note: all three fields are unique.
	/// </summary>
	public class Asl : Tbl2UniqueFields
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public Asl(SQLExecuter sqlExec) : base ("vlfAsl",sqlExec)
		{
		}
		/// <summary>
		/// Add new ASL type.
		/// Return new ASL id.
		/// </summary>
		/// <param name="aslType"></param>
		/// <param name="aslName"></param>
		/// <param name="description"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public short AddAsl(short aslType,string aslName,string description)
		{
            
			int rowsAffected = 0;
			short nextAslId = (short)(MaxRecordIndex + 1);
			string prefixMsg = "Unable to add new ASL type='" + nextAslId + "'.";
			//Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName + 
				" (AslId,AslType,AslName,Description) VALUES ({0},{1},'{2}','{3}')", 
				nextAslId,aslType,aslName.Replace("'","''"),description.Replace("'","''"));
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
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The ASL type already exists.");
			}
			return nextAslId;
		}	
		/// <summary>
		/// Delete exist ASL type.
		/// </summary>
		/// <param name="aslId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteAsl(short aslId)
		{
			return DeleteRowsByIntField("AslId",aslId, "ASL type");
		}
		/// <summary>
		/// Delete exist ASL type.
		/// </summary>
		/// <param name="aslName"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteAsl(string aslName)
		{
			return DeleteRowsByStrField("AslName",aslName, "ASL type");
		}
		/// <summary>
		/// Update ASL type info.
		/// </summary>
		/// <param name="aslId"></param>
		/// <param name="aslType"></param>
		/// <param name="aslName"></param>
		/// <param name="description"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if wrond data has been transfared to update.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateAslInfo(short aslId,short aslType,string aslName,string description)
		{
			int rowsAffected = 0;
			string prefixMsg = "Unable to add new ASL type='" + aslId + "'.";
			//Prepares SQL statement
			string sql = "UPDATE " + tableName + 
				" SET AslType=" + aslType + 
				",AslName='" + aslName.Replace("'","''") + "'" +
				",Description='" + description.Replace("'","''") + "'" +
				" WHERE AslId=" + aslId;
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
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The ASL type already exists.");
			}
		}	
		/// <summary>
		/// retrieves record count from "vlfAsl" table
		/// </summary>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 RecordCount
		{
			get
			{
				return GetRecordCount("AslId");
			}
		}
		/// <summary>
		/// retrieves max record index from "vlfAsl" table
		/// </summary>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 MaxRecordIndex
		{
			get
			{
				return GetMaxRecordIndex("AslId");
			}
		}
		/// <summary>
		/// retrieves ASL info from "vlfAsl" table
		/// </summary>
		/// <param name="aslId"></param>
		/// <returns>DataSet [AslId],[AslType],[AslName],[Description]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAslInfo(short aslId)
		{
			DataSet resultDataSet = null;
			string prefixMsg = "Unable to retrieve asl Id=" + aslId + ". ";
			try
			{
				//Prepares SQL statement
				string sql = "SELECT * FROM " + tableName + " WHERE AslId=" + aslId;
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}
		/// <summary>
		/// retrieves ASL type id from "vlfAsl" table
		/// </summary>
		/// <param name="aslId"></param>
		/// <returns>asl id</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public short GetAslTypeId(short aslId)
		{
			return Convert.ToInt16(GetFieldValueByRowId("AslId",aslId,"AslType","ASL type name"));
		}
		/// <summary>
		/// retrieves ASL type name from "vlfAsl" table
		/// </summary>
		/// <param name="aslId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <returns>asl name</returns>
		public string GetAslTypeName(short aslId)
		{
			return GetFieldValueByRowId("AslId",aslId,"AslName","ASL type name");
		}
		/// <summary>
		/// retrieves ASL description from "vlfAsl" table
		/// </summary>
		/// <param name="aslId"></param>
		/// <returns>asl description</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetAslDescription(short aslId)
		{
			return GetFieldValueByRowId("AslId",aslId,"Description","ASL description");
		}
		/// <summary>
		/// Updates ASL type id from "vlfAsl" table
		/// </summary>
		/// <param name="aslId"></param>
		/// <param name="aslType"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if wrond data has been transfared to update.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetAslTypeId(short aslId, short aslType)
		{
			int rowsAffected = 0;
			string prefixMsg = "Unable to set new ASL type=" + aslType + ". ";
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
							" SET AslType=" + aslType + 
							" WHERE AslId=" + aslId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException(prefixMsg, objException);
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
				throw new DASAppResultNotFoundException(prefixMsg + " The ASL '" + aslId + "' does not exists.");
			}
		}
		/// <summary>
		/// Updates ASL type name from "vlfAsl" table
		/// </summary>
		/// <param name="aslId"></param>
		/// <param name="aslName"></param>
		/// <returns></returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if wrond data has been transfared to update.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetAslTypeName(short aslId, string aslName)
		{
			int rowsAffected = 0;
			string prefixMsg = "Unable to set new ASL type name '" + aslName + "'. ";
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
							" SET AslName='" + aslName.Replace("'","''") + "'" + 
							" WHERE AslId=" + aslId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				throw new DASAppResultNotFoundException(prefixMsg + " The ASL '" + aslId + "' does not exists.");
			}
		}
	}
}
