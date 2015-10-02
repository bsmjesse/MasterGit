using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfBox table.
	/// </summary>
	public class BoxSettings : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public BoxSettings (SQLExecuter sqlExec) : base ("vlfBoxSettings", sqlExec)
		{
		}

		/// <summary>
		/// Add new box settings.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxProtocolTypeId"></param>
		/// <param name="commModeId"></param>
		/// <param name="maxMsgs"></param>
		/// <param name="maxTxtMsgs"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if box already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddBoxSettings (int boxId, short boxProtocolTypeId, short commModeId, short maxMsgs, short maxTxtMsgs)
		{
			int rowsAffected = 0;
			try
			{
				// 1. Set SQL command 
				string sql = "INSERT INTO vlfBoxSettings(BoxId, BoxProtocolTypeId, CommModeId, MaxMsgs, MaxTxtMsgs) VALUES (@BoxId, @BoxProtocolTypeId, @CommModeId, @MaxMsgs, @MaxTxtMsgs)";
				
            // 2. Add parameters to SQL statement
				sqlExec.ClearCommandParameters();
				sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
				sqlExec.AddCommandParam("@BoxProtocolTypeId", SqlDbType.SmallInt, boxProtocolTypeId);
				sqlExec.AddCommandParam("@CommModeId", SqlDbType.SmallInt, commModeId);
				sqlExec.AddCommandParam("@MaxMsgs", SqlDbType.SmallInt, maxMsgs);
				sqlExec.AddCommandParam("@MaxTxtMsgs", SqlDbType.SmallInt, maxTxtMsgs);
				
				if(sqlExec.RequiredTransaction())
				{
					// 3. Attaches SQL to transaction
               sqlExec.AttachToTransaction(sql);
				}

				// 4. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add max messages for box: "+ boxId + ", protocol:" + boxProtocolTypeId + " and communication mode:" + commModeId; 
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to add max messages for box:"+ boxId + ", protocol:" + boxProtocolTypeId + " and communication mode:" + commModeId; 
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg =	"Unable to add max messages for box:"+ boxId + ", protocol:" + boxProtocolTypeId + " and communication mode:" + commModeId; 
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This box settings already exists.");
			}
		}		
		
      /// <summary>
		/// Delete existing box.
		/// </summary>
		/// <param name="boxId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteBoxSettings(int boxId)
		{
			return DeleteRowsByIntField("BoxId",boxId, "box");		
		}
		
      /// <summary>
		/// Update box config Id.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxProtocolTypeId"></param>
		/// <param name="commModeId"></param>
		/// <param name="maxMsgs"></param>
		/// <param name="maxTxtMsgs"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if data doesn't exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateBoxSettings(int boxId, short boxProtocolTypeId, short commModeId, short maxMsgs, short maxTxtMsgs)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
					" SET MaxMsgs=" + maxMsgs +
					" ,MaxTxtMsgs=" + maxTxtMsgs +
					" WHERE BoxId=" + boxId +
					" AND BoxProtocolTypeId=" + boxProtocolTypeId +
					" AND CommModeId=" + commModeId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update max messages for box: " + boxId + ", protocol" + boxProtocolTypeId + " and communication mode:" + commModeId; 
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update max messages for box:"+ boxId + ", protocol" + boxProtocolTypeId + " and communication mode:" + commModeId; 
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to update max messages for box: " + boxId + " and protocol " + boxProtocolTypeId; 
				throw new DASAppResultNotFoundException(prefixMsg + " Wrong box id:'" + boxId + "' or protocol type id:'" + boxProtocolTypeId + "'.");
			}
		}

      /// <summary>
      /// 
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="boxProtocolTypeId"></param>
      /// <param name="commModeId"></param>
      public void UpdateProtocolTypeCommMode(int boxId, short boxProtocolTypeId, short commModeId) 
      {
         int rowsAffected = 0;
         try
         {
            //Prepares SQL statement
            string sql = "UPDATE " + tableName + 
                         " SET CommModeId=" + commModeId +
                         " WHERE BoxId=" + boxId + 
                         " AND BoxProtocolTypeId=" + boxProtocolTypeId;

            if (sqlExec.RequiredTransaction())
            {
               // 3. Attaches SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            //Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to UpdateProtocolTypeCommMode box: " + boxId + ", protocol" + boxProtocolTypeId + " and communication mode:" + commModeId;
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to UpdateProtocolTypeCommMode box:" + boxId + ", protocol" + boxProtocolTypeId + " and communication mode:" + commModeId;
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         //Throws exception in case of wrong result
         if (rowsAffected == 0)
         {
            string prefixMsg = "Unable to UpdateProtocolTypeCommMode box: " + boxId + " and protocol " + boxProtocolTypeId;
            throw new DASAppResultNotFoundException(prefixMsg + " Wrong box id:'" + boxId + "' or protocol type id:'" + boxProtocolTypeId + "'.");
         }
      }
		
      /// <summary>
		/// Retrieves box max messages per protocol and communication mode
		/// </summary>
		/// <returns>DataSet [BoxProtocolTypeId],[CommModeId],[MaxMsgs],[MaxTxtMsgs]</returns>
		/// <param name="boxId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetBoxSettingsInfo(int boxId)
		{
			DataSet sqlDataSet = null;
			try
			{
				sqlDataSet = sqlExec.SQLExecuteDataset("SELECT BoxProtocolTypeId,CommModeId,MaxMsgs,MaxTxtMsgs FROM vlfBoxSettings WHERE BoxId=" + boxId);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve box protocol settings. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve box protocol settings. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
	}
}