using System;
using System.Collections;	//for ArrayList
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfMake table
	/// </summary>
	public class Make : Tbl2UniqueFields
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public Make(SQLExecuter sqlExec) : base ("vlfMake",sqlExec)
		{
		}
		/// <summary>
		/// Adds new Make.
		/// </summary>
		/// <param name="makeName"></param>
		/// <returns>make id</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if make name alredy exists</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int AddMake(string makeName)
		{
			return AddNewRow("MakeId","MakeName",makeName,"make");
		}
		/// <summary>
		/// Deletes exist make by name.
		/// </summary>
		/// <param name="makeName"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if make name does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteMake(string makeName)
		{
			return DeleteRowsByStrField("MakeName",makeName, "make");
		}
		/// <summary>
		/// Deletes exist box hardware type by Id
		/// </summary>
		/// <param name="makeId"></param> 
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if make id does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteMake(int makeId)
		{
			return DeleteMake( GetMakeNameById(makeId) );
		}
		/// <summary>
		/// Retrieves record count of "vlfMake" table
		/// </summary>
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 RecordCount
		{
			get
			{
				return GetRecordCount("MakeId");
			}
		}

		/// <summary>
		/// Retrieves max record index from "vlfMake" table
		/// </summary>
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 MaxRecordIndex
		{
			get
			{
				return GetMaxRecordIndex("makeId");
			}
		}
		/// <summary>
		/// Retrieves make name by id
		/// </summary>
		/// <param name="makeId"></param>
		/// <returns>string</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetMakeNameById(int makeId)
		{
			return GetFieldValueByRowId("MakeId",makeId,"MakeName","make");
		}
		/// <summary>
		/// Retrieves all makes names.
		/// </summary>
		/// <returns>ArrayList [string]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public ArrayList GetAllMakesNames()
		{
			ArrayList resultList = null;
			DataSet sqlDataSet = null;
			//Prepares SQL statement
			try
			{
				//Prepares SQL statement
				string sql = "SELECT MakeName" + 
					" FROM " + tableName +
					" ORDER BY MakeName ASC";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve list of makes names.";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve list of makes names.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				resultList = new ArrayList(sqlDataSet.Tables[0].Rows.Count);
				//Retrieves info from Table[0].[0][0]
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					resultList.Add(Convert.ToString(currRow[0]).TrimEnd());
				}
			}
			return resultList;
		}

      /// <summary>
      /// Retrieves make id by name
      /// </summary>
      /// <param name="makeName"></param>
      /// <returns>Make Id</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int GetMakeIdByName(string makeName)
      {
         int mid = -1;
         if (String.IsNullOrEmpty(makeName))
            return 0;
         object result = sqlExec.SQLExecuteScalar(
            String.Format("SELECT MakeId FROM {0} WHERE MakeName = '{1}'", this.tableName, makeName));
         if (result != null)
            mid = (int)result;

         return mid;
         //return (int)GetRowsByStringField("MakeName", makeName, "Invalid Make").Tables[0].Rows[0]["MakeId"];
      }
	}
}