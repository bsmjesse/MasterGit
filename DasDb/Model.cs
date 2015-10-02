using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfModel table
	/// </summary>
	public class Model : TblOneIntPrimaryKey
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public Model(SQLExecuter sqlExec) : base ("vlfModel",sqlExec)
		{
		}
		/// <summary>
		/// Adds new Model.
		/// </summary>
		/// <param name="modelName"></param>
		/// <returns>model id</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if model name alredy exists</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int AddModel(string modelName)
		{
			return AddNewRow("ModelId","ModelName",modelName,"model");
		}
		/// <summary>
		/// Deletes exist box hardware type by Id
		/// </summary>
		/// <param name="modelId"></param> 
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if model id does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteModel(int modelId)
		{
			return DeleteRowsByIntField("ModelId",modelId, "model");
		}
		/// <summary>
		/// Retrieves record count of "vlfModel" table
		/// </summary>
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 RecordCount
		{
			get
			{
				return GetRecordCount("ModelId");
			}
		}

		/// <summary>
		/// Retrieves max record index from "vlfModel" table
		/// </summary>
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 MaxRecordIndex
		{
			get
			{
				return GetMaxRecordIndex("modelId");
			}
		}

		/// <summary>
		/// Retrieves box Protocol type name by id from "vlfModel" table
		/// </summary>
		/// <param name="modelId"></param>
		/// <returns>string</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetModelNameById(int modelId)
		{
			string resultValue = VLF.CLS.Def.Const.unassignedStrValue;
			int recCount = 0;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT ModelName FROM " + tableName + " WHERE ModelId=" + modelId;
				if(sqlExec.RequiredTransaction())
				{
					// Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				//Executes SQL statement
				DataSet resultDataSet = sqlExec.SQLExecuteDataset(sql);
				//Retrieves info from Table[0].[index][0] into resultValue variable if data exists
				foreach(DataRow currRow in resultDataSet.Tables[0].Rows)
				{
					foreach(DataColumn currCol in resultDataSet.Tables[0].Columns)
					{
						resultValue = currRow[currCol].ToString();
						++recCount;
						break;
					}
				}
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve model name by id=" + modelId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve model name by id=" + modelId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			//Trim speces at the end of result (all char fields in the database have fixed size)
			if(resultValue != null)
			{
				return resultValue.TrimEnd();
			}
			else
			{
				return resultValue;
			}
		}

      /// <summary>
      /// Retrieves model id by name
      /// </summary>
      /// <param name="modelName"></param>
      /// <returns>Model Id</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int GetModelIdByName(string modelName)
      {
         int mid = -1;
         if (String.IsNullOrEmpty(modelName))
            return 0;
         object result = sqlExec.SQLExecuteScalar(
            String.Format("SELECT ModelId FROM {0} WHERE ModelName = '{1}'", this.tableName, modelName));
         if (result != null)
            mid = (int)result;

         return mid;
         //return (int)GetRowsByStringField("ModelName", modelName, "Invalid Model").Tables[0].Rows[0]["ModelId"];
      }
   }
}
