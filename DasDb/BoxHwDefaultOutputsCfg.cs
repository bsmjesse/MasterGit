using System;
using System.Collections;	// for ArrayList
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfBoxHwDefaultOutputsCfg table.
	/// </summary>
	public class BoxHwDefaultOutputsCfg : TblConnect2TblsWithoutRules
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public BoxHwDefaultOutputsCfg(SQLExecuter sqlExec) : base ("vlfBoxHwDefaultOutputsCfg", sqlExec)
		{
		}

		/// <summary>
		/// Add new outputs.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="boxHwTypeId"></param>
		/// <param name="outputId"></param>
		/// <param name="outputName"></param>
		/// <param name="outputAction"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if output name alredy exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddOutput(short boxHwTypeId, short outputId, string outputName, string outputAction)
		{
			int maxSupportedOutputs = GetMaxSupportedOutputsByHwType(boxHwTypeId);
			int currSupportedOutputs = GetCurrentSupportedOutputsByHwType(boxHwTypeId);
			if(currSupportedOutputs + 1 > maxSupportedOutputs)
			{
				string errMsg =	"Unable to add new output id=" +  outputId + 
					"  HW type=" +  boxHwTypeId + 
					". Maximal number " + maxSupportedOutputs + 
					" of supported outputs has been reached.";
				throw new DASAppException(errMsg);
			}
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = string.Format("INSERT INTO vlfBoxHwDefaultOutputsCfg( BoxHwTypeId, OutputId, OutputName, OutputAction) VALUES ( {0}, {1}, '{2}', '{3}')", boxHwTypeId, outputId, outputName, outputAction);
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
				string prefixMsg = "Unable to add new output.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new output 'boxHwTypeId="+boxHwTypeId+" outputId="+outputId+" outputName="+outputName+" outputAction="+outputAction;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new output 'boxHwTypeId="+boxHwTypeId+" outputId="+outputId+" outputName="+outputName+" outputAction="+outputAction;
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The output already exists.");
			}	
		}		
		
        /// <summary>
		/// Delete all outputs related to box hardware type.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="boxHwTypeId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteOutputsByHwTypeId(short boxHwTypeId)
		{
			return DeleteRowsByIntField("BoxHwTypeId", boxHwTypeId, "box hardware type");		
		}
		
		/// <summary>
		/// retrieves output info by hardware type id
		/// </summary>
		/// <param name="boxHwTypeId"></param> 
		/// <returns>DataSet [OutputId][OutputName][OutputAction]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetOutputsInfoByHwTypeId(short boxHwTypeId)
		{
			DataSet sqlDataSet = null;
			//Prepares SQL statement
			try
			{
				//Prepares SQL statement
				string sql = "SELECT OutputId,OutputName,OutputAction" +
					" FROM vlfBoxHwDefaultOutputsCfg WHERE BoxHwTypeId = " + boxHwTypeId + 
					" ORDER BY OutputId ASC";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve list of outputs names.";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve list of outputs names.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
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
				string sql = "SELECT MaxOutputsNum" + 
					" FROM vlfBoxHwType" + 
					" WHERE BoxHwTypeId=" + boxHwTypeId;
				//Executes SQL statement
				maxOutputs = Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve max number supported of outputs for HW type=" + boxHwTypeId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve max number supported of outputs for HW type=" + boxHwTypeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return maxOutputs;
		}

		/// <summary>
		/// Retrieves current number of supported outputs for specific HW type.
		/// Note: if box does not exist return 0
		/// </summary>
		/// <param name="boxHwTypeId"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int GetCurrentSupportedOutputsByHwType(short boxHwTypeId)
		{
			int recordCount = 0;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT COUNT(BoxHwTypeId) FROM vlfBoxHwDefaultOutputsCfg WHERE BoxHwTypeId=" + boxHwTypeId;
				//Executes SQL statement
				recordCount =  Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve current number supported of sensors for HW type=" + boxHwTypeId + ". ";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve current number supported of sensors for HW type=" + boxHwTypeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}				
			return recordCount;
		}

        /// <summary>
        /// Get all outputs from DB
        /// </summary>
        /// <returns></returns>
        public DataSet GetAllOutputs()
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                //Prepares SQL statement
                string sql = "SELECT OutputId As ID, OutputName As Name, OutputAction As Action FROM vlfOutput ORDER BY OutputId ASC";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve outputs list";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve outputs list";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
	}
}

