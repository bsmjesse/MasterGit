using System;
using System.Collections;	// for ArrayList
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Summary description for MakeModel.
	/// </summary>
	public class MakeModel : TblOneIntPrimaryKey
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public MakeModel(SQLExecuter sqlExec) : base ("vlfMakeModel",sqlExec)
		{
		}
		/// <summary>
		/// Adds new row that connected make and model
		/// </summary>
		/// <param name="makeId"></param>
		/// <param name="modelId"></param>
		/// <returns>make model id</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if model id and make id alredy exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int AddMakeModel(int makeId,int modelId)
		{
			int nextMakeModel = (int)GetMaxRecordIndex("MakeModelId") + 1;
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName + 
						" (MakeModelId, MakeId,ModelId ) VALUES ( {0},{1},{2} )", 
							nextMakeModel,makeId, modelId);
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
				rowsAffected = nextMakeModel;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add new MakeId:" + makeId + " ModelId:" + modelId + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new MakeId:" + makeId + " ModelId:" + modelId + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new MakeId:" + makeId + " ModelId:" + modelId + ".";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The make id model id already exists.");
			}
			return rowsAffected;
		}	
		/// <summary>
		/// Deletes all model ids associated with make Id
		/// </summary>
		/// <param name="makeId"></param> 
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if make id does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteByMakeId(int makeId)
		{
			return DeleteRowsByIntField("MakeId",makeId, "make id");
		}
		/// <summary>
		/// Deletes exist model assosiated with make id.
		/// </summary>
		/// <param name="makeId"></param>
		/// <param name="modelId"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if make id  or model id do not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteByMakeIdModelId(int makeId,int modelId)
		{
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "DELETE FROM " + tableName + " WHERE (MakeId=" 
				+ makeId + ") AND (ModelId=" + modelId + ")";
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to delete by make id=" + makeId + " model id=" + modelId;
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete by make id=" + makeId + " model id=" + modelId;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Retrieves record count from vlfMakeModel table.
		/// </summary>
		/// <returns>int</returns>
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
		/// Retrieves all model ids related to specific make.
		/// </summary>
		/// <param name="makeId"></param>
		/// <returns>int[]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int[] GetModelIdsByMakeId(int makeId)
		{
			int[] resultList = null;
			DataSet sqlDataSet = null;
			int index = 0;
			//Prepares SQL statement
			try
			{
				//Prepares SQL statement
				string sql = "SELECT ModelId FROM " + tableName + 
					" WHERE MakeId=" + makeId +
					" ORDER BY ModelId ASC";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve list of model ids by make id=" + makeId + ".";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve list of model ids by make id=" + makeId + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				resultList = new int[sqlDataSet.Tables[0].Rows.Count];
				//Retrieves info from Table[0].[0][0]
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					resultList[index++] = Convert.ToInt32(currRow[0]);
				}
			}
			return resultList;
		}	
		/// <summary>
		/// Retrieves all model names related to specific make.
		/// </summary>
		/// <param name="makeId"></param>
		/// <returns>ArrayList [string]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public ArrayList GetModelNamesByMakeId(int makeId)
		{
			ArrayList resultList = null;
			DataSet sqlDataSet = GetModelsInfoByMakeId(makeId);
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				resultList = new ArrayList(sqlDataSet.Tables[0].Rows.Count);
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					resultList.Add(Convert.ToString(currRow["ModelName"]).TrimEnd());
				}
			}
			return resultList;
		}	
		/// <summary>
		/// Retrieves all models information related to specific make.
		/// </summary>
		/// <param name="makeId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetModelsInfoByMakeId(int makeId)
		{
			DataSet sqlDataSet = null;
			//Prepares SQL statement
			try
			{
				//Prepares SQL statement
				string sql = "SELECT  DISTINCT vlfModel.ModelId,vlfModel.ModelName,vlfMakeModel.MakeModelId" + 
							" FROM vlfMakeModel,vlfMake,vlfModel" +
							" WHERE (vlfMakeModel.MakeId=" + makeId + ")" +
							" AND (vlfMakeModel.ModelId=vlfModel.ModelId)" +
							" ORDER BY ModelName ASC";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve list of models info by make id=" + makeId + ".";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve list of models info by make id=" + makeId + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

      /// <summary>
      /// Get makemodelid based on make id and model id
      /// </summary>
      /// <param name="makeId"></param>
      /// <param name="modelId"></param>
      /// <returns>MakeModelId</returns>
      public int GetMakeModelIdByMakeIdModelId(int makeId, int modelId)
      {
         int mid = -1;
         object result = sqlExec.SQLExecuteScalar(
            String.Format("SELECT MakeModelId FROM {0} WHERE MakeId = {1} AND ModelId = {2}", this.tableName, makeId, modelId));
         if (result != null)
            mid = (int)result;

         return mid;
      }

      /// <summary>
      /// Get makemodelid based on make name and model name
      /// </summary>
      /// <param name="makeName"></param>
      /// <param name="modelName"></param>
      /// <returns>MakeModelId</returns>
      public int GetMakeModelIdByMakeNameModelName(string makeName, string modelName)
      {
         int mid = -1;
         object result = sqlExec.SQLExecuteScalar(
            String.Format("SELECT MakeModelId FROM {0} WHERE MakeId = (SELECT MakeId FROM vlfMake WHERE MakeName = '{1}') AND ModelId = (SELECT ModelId FROM vlfModel WHERE ModelName = '{2}')", 
            this.tableName, makeName, modelName));
         if (result != null)
            mid = (int)result;

         return mid;
      }
	}
}
