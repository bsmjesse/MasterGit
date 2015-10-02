using System;
using System.Collections;	// for ArrayList
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provide interfaces to vlfBoxOutputsCfg table.
	/// </summary>
	public class BoxOutputsCfg : TblConnect2TblsWithIntAdditField
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public BoxOutputsCfg(SQLExecuter sqlExec) : base ("vlfBoxOutputsCfg",sqlExec)
		{
		}
		/// <summary>
		/// Add new output.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxHwTypeId"></param>
		/// <param name="outputId"></param>
		/// <param name="outputName"></param>
		/// <param name="outputAction"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if output id and name alredy exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddOutput(int boxId,short boxHwTypeId,short outputId,string outputName,string outputAction)
		{
			int maxSupportedOutputs = GetMaxSupportedOutputsByHwType(boxHwTypeId);
			int currSupportedOutputs = GetCurrentSupportedOutputsByBoxId(boxId);
			if(currSupportedOutputs + 1 > maxSupportedOutputs)
			{
				string errMsg =	"Unable to add to box " + boxId + 
					" new output id=" +  outputId + 
					"  output name=" +  outputName + 
					" and  output action =" +  outputAction + 
					". Maximal number " + maxSupportedOutputs + 
					" of supported outputs has been reached.";
				throw new DASAppException(errMsg);
			}

			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName + 
				" ( BoxId,OutputId,OutputName,OutputAction)" +
				" VALUES ( {0}, {1} , '{2}','{3}')", 
				boxId,outputId,outputName.Replace("'","''"),outputAction.Replace("'","''"));
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add new output with box id=" + boxId + " output id=" + outputId + " output name=" + outputName + " and output action =" + outputAction + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new output with box id=" + boxId + " output id=" + outputId + " output name=" + outputName + " and output action =" + outputAction + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new output with box id=" + boxId + " output id=" + outputId + " output name=" + outputName + " and output action =" + outputAction + ".";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This output already exists.");
			}
		}	
		/// <summary>
		/// Add new outputs.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxHwTypeId"></param>
		/// <param name="dsOutputsCfg"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if output id and name alredy exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddOutputs(int boxId,short boxHwTypeId,DataSet dsOutputsCfg)
		{
			if((dsOutputsCfg != null) && (dsOutputsCfg.Tables.Count > 0))
			{
				foreach(DataRow ittr in dsOutputsCfg.Tables[0].Rows)
				{
					AddOutput (boxId,
								  boxHwTypeId,
								  Convert.ToInt16(ittr[0]),
								  ittr[1].ToString().TrimEnd(),
								  ittr[2].ToString().TrimEnd());
				}
			}
		}
		/// <summary>
		/// Update output information.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="outputId"></param>
		/// <param name="outputName"></param>
		/// <param name="outputAction"></param>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not have info for current output.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateOutput(int boxId,short outputId,string outputName,string outputAction)
		{
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "UPDATE " + tableName + 
				" SET OutputName='" + outputName.Replace("'","''") + "'" +
				",OutputAction='" + outputAction.Replace("'","''") + "'" +
				" WHERE BoxId=" + boxId +
				" AND OutputId=" + outputId;
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update output info for box=" + boxId + " outputId=" + outputId + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update output info for box=" + boxId + " outputId=" + outputId + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to update output info for box=" + boxId + " outputId=" + outputId + ".";
				throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " Box " + boxId + " does not have info for output " + outputId + ".");
			}
		}		
		
		/// <summary>
		/// Delete all outputs related to the box.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="boxId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if box id does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteOutputsByBoxId(int boxId)
		{
			return DeleteRowsByIntField("BoxId",boxId, "box");		
		}
		/// <summary>
		/// retrieves output info by box id and filter by User Id
		/// </summary>
		/// <param name="boxId"></param> 
		/// <param name="userId"></param> 
		/// <returns>DataSet [OutputId][OutputName][OutputAction]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetOutputsInfoByBoxId(int boxId,int userId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT DISTINCT OutputId,OutputName,OutputAction" +
					" FROM " + tableName + ",vlfUserGroupAssignment,vlfGroupSecurity" +
					" WHERE BoxId=" + boxId +
					" AND vlfUserGroupAssignment.UserId=" + userId +
					" AND vlfUserGroupAssignment.UserGroupId=vlfGroupSecurity.UserGroupId" +
					" AND vlfGroupSecurity.OperationType=" + (int)VLF.CLS.Def.Enums.OperationType.Output +
					" AND vlfGroupSecurity.OperationId=" + tableName + ".OutputId";
				
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve output info by box id=" + boxId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve output info by box id=" + boxId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
		/// <summary>
		/// Retrieves output info structure [OutputId][OutputName][OutputAction] by box id and user id
		/// </summary>
		/// <param name="boxId"></param> 
		/// <param name="userId"></param> 
		/// <returns>string[,] of [OutputId][OutputName][OutputAction]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string[,] GetOutputsInfoStructByBoxId(int boxId,int userId)
		{
			string[,] resultArr = null;
			DataSet sqlDataSet = GetOutputsInfoByBoxId(boxId,userId);
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				resultArr = new string[sqlDataSet.Tables[0].Rows.Count,sqlDataSet.Tables[0].Columns.Count];
				int index = 0;
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					resultArr[index,0] = Convert.ToString(currRow[0]);
					resultArr[index,1] = Convert.ToString(currRow[1]).TrimEnd();
					resultArr[index,2] = Convert.ToString(currRow[2]).TrimEnd();
					++index;
				}
			}
			return resultArr;
		}
		/// <summary>
		/// Retrieves outputs names by box id and user id.
		/// </summary>
		/// <param name="boxId"></param> 
		/// <param name="userId"></param> 
		/// <returns>ArrayList</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public ArrayList GetOutputsNamesByBoxId(int boxId,int userId)
		{
			ArrayList resultList = null;
			DataSet sqlDataSet = GetOutputsInfoByBoxId(boxId,userId);
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				resultList = new ArrayList(sqlDataSet.Tables[0].Rows.Count);
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					resultList.Add(Convert.ToString(currRow[1]).TrimEnd());
				}
			}
			return resultList;
		}
		/// <summary>
		/// retrieves outputs ids by box id and user id
		/// </summary>
		/// <param name="boxId"></param> 
		/// <param name="userId"></param> 
		/// <returns>ArrayList</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public ArrayList GetOutputsIdsArrayByBoxId(int boxId,int userId)
		{
			ArrayList resultList = null;
			DataSet sqlDataSet = GetOutputsInfoByBoxId(boxId,userId);
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
		/// Retrieves max number of supported outputs for specific Hw type.
		/// Note: if box does not exist return 0
		/// </summary>
		/// <param name="boxHwTypeId"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int GetMaxSupportedOutputsByHwType(short boxHwTypeId)
		{
			int maxOutputs = 0;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfBoxHwType.MaxOutputsNum" + 
					" FROM vlfBoxHwType" + 
					" WHERE BoxHwTypeId=" + boxHwTypeId;
				//Executes SQL statement
				maxOutputs = Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve max number supported of outputs for HW type " + boxHwTypeId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve max number supported of outputs for HW type " + boxHwTypeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return maxOutputs;
		}
		/// <summary>
		/// Retrieves current number of supported outputs for specific box id.
		/// Note: if box does not exist return VLF.CLS.Def.Const.unassignedIntValue
		/// </summary>
		/// <param name="boxId"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int GetCurrentSupportedOutputsByBoxId(int boxId)
		{
			int recordCount = 0;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT COUNT(BoxId) FROM " + tableName + " WHERE BoxId=" + boxId;
				//Executes SQL statement
				recordCount =  Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve current number supported of sensors for box id=" + boxId + ". ";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve current number supported of sensors for box id=" + boxId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}				
			return recordCount;
		}
	}
}

