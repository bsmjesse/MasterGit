using System;
using System.Collections;	// for ArrayList
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfOutput table.
	/// </summary>
	public class Output: TblOneIntPrimaryKey
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public Output(SQLExecuter sqlExec) : base ("vlfOutput",sqlExec)
		{
		}
		/// <summary>
		/// Add new output.
		/// </summary>
		/// <param name="outputName"></param>
		/// <param name="outputAction"></param>
		/// <returns>int next output id</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if output name alredy exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int AddOutput(string outputName,string outputAction)
		{
			int rowsAffected = 0;
			// 1. Get next availible index
			int outputId = (int)GetMaxRecordIndex("OutputId") + 1;
			// 2. Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName 
				+ " (OutputId,OutputName,OutputAction) VALUES ( {0}, '{1}', '{2}' )",
				outputId,outputName.Replace("'","''"),outputAction.Replace("'","''"));
			try
			{
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add new output with output name " + outputName + " .";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new output with output name " + outputName + " .";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new output with output name " + outputName + " .";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This output already exists.");
			}
			return outputId;
		}		
		/// <summary>
		/// Deletes existing output.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="outputName"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if output name does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteOutputByName(string outputName)
		{
			return DeleteRowsByStrField("OutputName",outputName, "output");		
		}
		
		/// <summary>
		/// Deletes existing output.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="outputId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if output id does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteOutputById(short outputId)
		{
			return DeleteRowsByIntField("OutputId",outputId, "output");		
		}
		
		/// <summary>
		/// Returns formated output status
		/// </summary>
		/// <param name="customProp"></param>
		/// <param name="tblUserDefinedOutputs"></param>
		/// <returns>string</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public static string GetOutputDescription(string customProp,DataTable tblUserDefinedOutputs)
		{
			string outputDescription = "";
			if(Util.PairFindValue(VLF.CLS.Def.Const.keySensorNum,customProp) == "")
			{
				outputDescription = customProp;
			}
			else
			{
				int outputId = Convert.ToInt32(Util.PairFindValue(VLF.CLS.Def.Const.keySensorNum,customProp));
				if((tblUserDefinedOutputs == null)||(tblUserDefinedOutputs.Rows.Count == 0))
				{
					outputDescription = outputId.ToString();
					outputDescription +=" ";
					outputDescription += Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus,customProp);
				}
				else
				{
					string outputAction = "";
					int delim = 0;
					foreach(DataRow ittr in tblUserDefinedOutputs.Rows)
					{
						if(outputId == Convert.ToInt32(ittr["OutputId"]))
						{
							outputDescription = ittr["OutputName"].ToString().TrimEnd();
							outputDescription +=" ";

							outputAction = ittr["OutputAction"].ToString().TrimEnd();
							delim = outputAction.IndexOf('/',0);
							if(Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus,customProp) == VLF.CLS.Def.Const.valON)
								outputDescription += outputAction.Substring(0,delim);
							else
								outputDescription += outputAction.Substring(delim + 1,outputAction.Length - delim - 1);
							break;
						}
					}
					if(outputDescription == "")
					{
						outputDescription = outputId.ToString();
						outputDescription +=" ";
						outputDescription += Util.PairFindValue(VLF.CLS.Def.Const.keySensorStatus,customProp);
					}
				}
			}		
			return outputDescription;
		}
	}
}
