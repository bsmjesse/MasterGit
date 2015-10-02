using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for OleDbDataReader
using System.Text;
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
   /// <summary>
   /// Provides interfaces to vlfFirmware table.
   /// </summary>
   public class BoxConfig : TblGenInterfaces
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public BoxConfig(SQLExecuter sqlExec)
         : base("vlfFirmware", sqlExec)
      {
      }
      /// <summary>
      /// Retrieves configurations info. 	
      /// </summary>
      /// <param name="fwChId "></param>
      /// <returns>DataSet [FwChId],[ChPriority],[FwId],[FwName],[ChId],[ChName],[CommModeId],[CommModeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[BoxHwTypeId],[BoxHwTypeName],[OAPPort]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetConfigInfo(short fwChId)
      {

         DataSet resultDataSet = null;
         try
         {
            // 1. Prepares SQL statement
            string sql = "SELECT vlfFirmwareChannelReference.FwChId, vlfFirmwareChannels.ChPriority, vlfFirmware.FwId,vlfFirmware.FwName, vlfChannels.ChId, vlfChannels.ChName, vlfCommMode.CommModeId, vlfCommMode.CommModeName,vlfBoxProtocolType.BoxProtocolTypeId, vlfBoxProtocolType.BoxProtocolTypeName, vlfBoxHwType.BoxHwTypeId,vlfBoxHwType.BoxHwTypeName, OAPPort FROM vlfFirmwareChannelReference INNER JOIN vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId INNER JOIN vlfBoxProtocolType ON vlfChannels.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId INNER JOIN vlfBoxHwType ON vlfFirmware.BoxHwTypeId = vlfBoxHwType.BoxHwTypeId WHERE vlfFirmwareChannelReference.FwChId=" + fwChId + " ORDER BY vlfFirmwareChannels.ChPriority";

            if (sqlExec.RequiredTransaction())
            {
               // 2. Attaches SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve box configuration by fwChId=" + fwChId + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve box configuration by fwChId=" + fwChId + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Retrieves configurations info. 	
      /// </summary>
      /// <param name="fwChId "></param>
      /// <returns>DataSet [FwChId],[ChPriority],[ChName]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetConfigInfoByFwId(short fwId)
      {

         DataSet resultDataSet = null;
         try
         {
            // 1. Prepares SQL statement
            string sql = String.Format("SELECT vlfFirmwareChannels.FwChId AS [ID], vlfFirmwareChannels.ChPriority AS [Priority], vlfChannels.ChName AS [Channel Name] FROM vlfFirmwareChannelReference INNER JOIN vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId INNER JOIN vlfBoxProtocolType ON vlfChannels.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId INNER JOIN vlfBoxHwType ON vlfFirmware.BoxHwTypeId = vlfBoxHwType.BoxHwTypeId WHERE vlfFirmwareChannels.FwId={0} ORDER BY vlfFirmwareChannels.FwChId, vlfFirmwareChannels.ChPriority", fwId);

            if (sqlExec.RequiredTransaction())
            {
               // 2. Attaches SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve box configuration by fwId=" + fwId + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve box configuration by fwId=" + fwId + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Get box default communication info
      /// </summary>
      /// <param name="fwChId"></param>
      /// <returns>DataSet [CommAddressTypeId],[CommAddressTypeName],[CommAddressValue],[FwChId],[BoxHwTypeName],[BoxProtocolTypeId],[CommModeId],[ChId],[ChName],[ChPriority],[OAPPort] </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetDefCommInfo(short fwChId)
      {
         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT vlfCommAddressType.CommAddressTypeId, vlfCommAddressType.CommAddressTypeName, '' AS CommAddressValue, vlfFirmwareChannels.FwChId, vlfBoxHwType.BoxHwTypeName, vlfChannels.BoxProtocolTypeId, vlfCommMode.CommModeId, vlfChannels.ChId, vlfChannels.ChName, vlfFirmwareChannels.ChPriority, OAPPort FROM vlfCommModeAddressType INNER JOIN vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId ON vlfCommModeAddressType.CommModeId = vlfCommMode.CommModeId INNER JOIN vlfCommAddressType ON vlfCommModeAddressType.CommAddressTypeId = vlfCommAddressType.CommAddressTypeId INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN vlfBoxHwType ON vlfFirmware.BoxHwTypeId = vlfBoxHwType.BoxHwTypeId WHERE vlfFirmwareChannels.FwChId=" + fwChId + " ORDER BY vlfFirmwareChannels.ChPriority";
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve default communication information by fwChId=" + fwChId + " . ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve default communication information by fwChId=" + fwChId + " . ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Get box configuration information. 
      /// </summary>
      /// <returns>DataSet[FwChId],[FwId],[FwName],[ChId],[ChName],[ChPriority],[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum],[OAPPort]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllConfigInfo()
      {
         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT vlfFirmwareChannels.FwChId, vlfFirmwareChannels.FwId, vlfFirmware.FwName, vlfChannels.ChId, vlfChannels.ChName, vlfFirmwareChannels.ChPriority, vlfBoxHwType.BoxHwTypeId, vlfBoxHwType.BoxHwTypeName, vlfBoxHwType.MaxSensorsNum, vlfBoxHwType.MaxOutputsNum, OAPPort FROM vlfFirmwareChannels INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfBoxHwType ON vlfFirmware.BoxHwTypeId = vlfBoxHwType.BoxHwTypeId";
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all configuration information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all configuration information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Get all primary channels by FwId
      /// </summary>
      /// <param name="fwId"></param>
      /// <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[CommModeId],[CommModeName]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllPrimaryChannelsByFwId(short fwId)
      {
         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT DISTINCT vlfBoxProtocolType.BoxProtocolTypeId, vlfBoxProtocolType.BoxProtocolTypeName, vlfCommMode.CommModeId, vlfCommMode.CommModeName FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfBoxProtocolType ON vlfChannels.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId WHERE vlfFirmwareChannels.ChPriority=0 AND vlfFirmwareChannels.FwId =" + fwId + " ORDER BY vlfCommMode.CommModeName";

            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all channels information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all channels information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Get all channels by FwId, both primary and secondary
      /// </summary>
      /// <param name="fwId">Firmware Id</param>
      /// <returns>DataSet [FwChId], [FwId], [ChId], [ChName], [ChPriority]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllChannelsByFwId(short fwId)
      {
         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement with parameters
            string sql = "SELECT DISTINCT vlfFirmwareChannels.FwChId, vlfFirmwareChannels.FwId, vlfChannels.ChId, vlfChannels.ChName, vlfFirmwareChannels.ChPriority FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfFirmwareChannels.FwId = @fwId ORDER BY vlfFirmwareChannels.FwChId, vlfFirmwareChannels.ChPriority";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@fwId", SqlDbType.SmallInt, fwId);
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all channels information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all channels information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Get all channels by FwId
      /// </summary>
      /// <param name="fwId">Firmware Id</param>
      /// <param name="chPriority">Channel priority (0 - primary, 1 - secondary)</param>
      /// <returns>DataSet [FwChId], [FwId], [ChId], [ChName]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllChannelsByFwId(short fwId, short chPriority)
      {
         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement with parameters
            string sql = "SELECT DISTINCT vlfFirmwareChannels.FwChId, vlfFirmwareChannels.FwId, vlfchannels.ChId, vlfchannels.ChName FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfFirmwareChannels.ChPriority=@chPriority AND vlfFirmwareChannels.FwId = @fwId ORDER BY vlfFirmwareChannels.FwChId";
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@fwId", SqlDbType.SmallInt, fwId);
            sqlExec.AddCommandParam("@chPriority", SqlDbType.SmallInt, chPriority);
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all channels information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all channels information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      ///         
      /// </summary>
      /// <param name="protocolType"></param>
      /// <param name="commMode"></param>
      /// <returns></returns>
      public short GetChannelId(short protocolType, short commMode)
      {
         Util.BTrace(Util.INF0, "-- BoxConfig.GetChannelId -> protocolType[{0}] commMode[{1}", protocolType, commMode);

         short ret = -1;
         try
         {
            //Prepares SQL statement
            string sql = string.Format("SELECT ChId from vlfChannels WHERE BoxProtocolTypeId={0} AND CommModeId={1}", protocolType, commMode );
            Util.BTrace(Util.INF0, "-- BoxConfig.GetChannelId -> sql[{0}]", sql);
            //Executes SQL statement
            ret = Convert.ToInt16( sqlExec.SQLExecuteScalar(sql) );
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve channelID (1)";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve channelID (2)";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return ret;
      }

      /// <summary>
      ///      returns [FwId], [ChId], [ChPriority]
      /// </summary>
      /// <param name="boxId"></param>
      /// <returns></returns>
      public DataSet GetAllChannelsByBoxId(int boxId)
      {
         Util.BTrace(Util.INF0, "-- BoxConfig.GetAllChannelsByBoxId -> boxId[{0}]", boxId);

         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
             string sql = "SELECT FwId, ChId, ChPriority from vlfFirmwareChannels  INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannels.FwChId = vlfBox.FwChId where BoxId=" + boxId.ToString() + " order by ChPriority";
            Util.BTrace(Util.INF0, "-- BoxConfig.GetAllChannelsByBoxId -> sql[{0}]", sql);
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all channels information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all channels information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      ///      returns [FwId], [ChId], [ChPriority]
      /// </summary>
      /// <param name="fwChId"></param>
      /// <returns></returns>
      public DataSet GetAllChannelsByFwChId(short fwChId)
      {
         Util.BTrace(Util.INF0, "-- BoxConfig.GetAllChannelsByFwChId -> FwIChd[{0}]", fwChId);

         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT FwId, ChId, ChPriority from vlfFirmwareChannels where FwChId=" + fwChId.ToString();
            Util.BTrace(Util.INF0, "-- BoxConfig.GetAllSecondaryChannelsByFwId -> sql[{0}]", sql);
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all channels information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all channels information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      ///      look after FwChId based on  { [ChId], [FwId], [ChPriority] }*
      /// </summary>
      /// <comment>
      ///   select  FwChId from vlfFirmwareChannels 
      ///   group by FwChid
      ///   having FwChid in 
      ///   (
      ///     select FwChid from vlfFirmwareChannels where (Chid = 19 AND Fwid= 89 and ChPriority=0) 
      ///   ) and count(FwChid) = 2
      /// </comment>
      /// <param name="ds"></param>
      /// <returns></returns>
      public short GetFirmwareChannelIdByChannels(DataSet ds)
      {
         Util.BTrace(Util.INF0, "-- BoxConfig.GetFirmwareChannelIdByChannels ");

         short result = (-1) ;
         try
         {
            StringBuilder cond = new StringBuilder();
            string matrix = "select FwChid from vlfFirmwareChannels where (Chid = {0} AND Fwid={1} and ChPriority={2})";
            // iterate through the DataTable and build the conditions
            int i = 0, cnt = ds.Tables[0].Rows.Count ;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {  
               if (0==i)
                  cond.AppendFormat(matrix, dr["ChId"].ToString(), dr["FwId"].ToString(), dr["ChPriority"].ToString());
               else
                  if (i < cnt)
                     cond.Append(" AND FwChId in (").AppendFormat(matrix, dr["ChId"].ToString(), dr["FwId"].ToString(), dr["ChPriority"].ToString()).Append(")");
               ++i;
            }
            string sql = String.Format("select FwChId from vlfFirmwareChannels group by FwChid having FwChid in ( {0} ) and count(FwChId)={1}",
               cond.ToString(), cnt);

            //Prepares SQL statement
            Util.BTrace(Util.INF0, "-- BoxConfig.GetFirmwareChannelIdByChannels -> sql[{0}]", sql);// cond.ToString());
            //Executes SQL statement
            result = Convert.ToInt16(sqlExec.SQLExecuteScalar(sql));//cond.ToString()));
         }
         catch (SqlException objException)
         {
            string prefixMsg = "GetFirmwareChannelIdByChannels->Unable to retrieve all channels information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "GetFirmwareChannelIdByChannels->Unable to retrieve all channels information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return result;
      }

      /// <summary>
      /// Get all secondary channels by FwId
      /// </summary>
      /// <param name="fwId"></param>
      // <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[CommModeId],[CommModeName]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllSecondaryChannelsByFwId(short fwId)
      {
         Util.BTrace(Util.INF0, "-- BoxConfig.GetAllSecondaryChannelsByFwId -> FwId[{0}]", fwId);
         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT DISTINCT vlfBoxProtocolType.BoxProtocolTypeId, vlfBoxProtocolType.BoxProtocolTypeName, vlfCommMode.CommModeId,vlfCommMode.CommModeName FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfBoxProtocolType ON vlfChannels.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId WHERE vlfFirmwareChannels.ChPriority=1 AND vlfFirmwareChannels.FwId=" + fwId + " ORDER BY vlfCommMode.CommModeName";
            Util.BTrace(Util.INF0, "-- BoxConfig.GetAllSecondaryChannelsByFwId -> sql[{0}]", sql);
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all channels information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all channels information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Get all secondary channels by FwId
      /// </summary>
      /// <param name="fwId"></param>
      // <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[CommModeId],[CommModeName]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllSecondaryChannelsByFwChId(short fwChId)
      {
         Util.BTrace(Util.INF0, "-- BoxConfig.GetAllSecondaryChannelsByFwChId -> FwId[{0}]", fwChId);
         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT DISTINCT vlfBoxProtocolType.BoxProtocolTypeId, vlfBoxProtocolType.BoxProtocolTypeName, vlfCommMode.CommModeId,vlfCommMode.CommModeName FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfBoxProtocolType ON vlfChannels.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId WHERE vlfFirmwareChannels.ChPriority=1 AND vlfFirmwareChannels.FwChId=" + fwChId + " ORDER BY vlfCommMode.CommModeName";
            Util.BTrace(Util.INF0, "-- BoxConfig.GetAllSecondaryChannelsByFwId -> sql[{0}]", sql);
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all channels information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all channels information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Get all secondary channels
      /// </summary>
      /// <param name="fwId"></param>
      /// <param name="commModeId"></param>
      // <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[CommModeId],[CommModeName]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllSecondaryChannels(short fwId, short commModeId)
      {
         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT DISTINCT vlfCommMode.CommModeId, vlfCommMode.CommModeName, vlfBoxProtocolType.BoxProtocolTypeId, vlfBoxProtocolType.BoxProtocolTypeName FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId INNER JOIN vlfBoxProtocolType ON vlfChannels.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId WHERE vlfFirmwareChannels.ChPriority=1" +
            " AND vlfFirmwareChannels.FwChId IN (SELECT vlfFirmwareChannels.FwChId FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfFirmwareChannels.FwId=" + fwId + " AND vlfChannels.CommModeId=" + commModeId + " AND vlfFirmwareChannels.ChPriority = 0) ORDER BY vlfCommMode.CommModeName";


            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all secondary channels information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all secondary channels information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Get firmwar channel info
      /// </summary>
      /// <param name="fwId"></param>
      /// <param name="commMode"></param>
      // <returns>DataSet [FwChId],[ChId],[FwId],[ChPriority]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetPrimaryFirmwareChannelInfo(short fwId, short commMode)
      {
         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "";
            if (commMode != VLF.CLS.Def.Const.unassignedShortValue)
               sql = "SELECT FwChId, ChId, FwId, ChPriority FROM vlfFirmwareChannels WHERE ChPriority=0 AND FwChId IN (SELECT vlfFirmwareChannels.FwChId FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfFirmwareChannels.ChPriority = 1 AND vlfFirmwareChannels.FwId =" + fwId + " AND vlfChannels.CommModeId =" + commMode + ")";
            else
               sql = "SELECT FwChId, ChId, FwId, ChPriority FROM vlfFirmwareChannels WHERE FwId=" + fwId + " AND ChPriority=0 AND FwChId NOT IN (SELECT FwChId FROM vlfFirmwareChannels WHERE FwId=" + fwId + " AND ChPriority=1)";


            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve primary channel information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve primary channel information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Get Firmware Information. 
      /// </summary>
      /// <param name="fwId"></param>
      /// <returns>DataSet [FwChId],[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommModeId],[CommModeName],[ChPriority],[FwTypeId],[FwLocalPath],[FwOAPPath],[FwDateReleased],[MaxGeozones],[OAPPort]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetFirmwareInfo(short fwId)
      {
         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT vlfFirmwareChannels.FwChId, vlfBoxHwType.BoxHwTypeId, vlfBoxHwType.BoxHwTypeName, vlfBoxHwType.MaxSensorsNum, vlfBoxHwType.MaxOutputsNum, vlfBoxProtocolType.BoxProtocolTypeId, vlfBoxProtocolType.BoxProtocolTypeName, vlfCommMode.CommModeId, vlfCommMode.CommModeName, vlfFirmwareChannels.ChPriority, vlfFirmware.FwTypeId, vlfFirmware.FwLocalPath, vlfFirmware.FwOAPPath, vlfFirmware.FwDateReleased, vlfFirmware.MaxGeozones, OAPPort FROM vlfFirmwareChannels INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfBoxHwType ON vlfFirmware.BoxHwTypeId = vlfBoxHwType.BoxHwTypeId INNER JOIN vlfBoxProtocolType ON vlfChannels.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId WHERE vlfFirmwareChannels.FwId =" + fwId + " ORDER BY vlfFirmwareChannels.ChPriority";
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve fwId=" + fwId + " information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve fwId=" + fwId + " information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Get Firmware Information. 
      /// </summary>
      /// <param name="fwId"></param>
      /// <returns>DataSet [FwId],[BoxHwTypeId],[FwName],[FwTypeId],[FwLocalPath],[FwOAPPath],[FwDateReleased],[MaxGeozones],[BoxHwTypeName],[OAPPort]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetFirmwareInfoOnly(short fwId)
      {
         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT vlfFirmware.FwId, vlfFirmware.BoxHwTypeId, vlfFirmware.FwName, vlfFirmware.FwTypeId, vlfFirmware.FwLocalPath, vlfFirmware.FwOAPPath, vlfFirmware.FwDateReleased, vlfFirmware.MaxGeozones, vlfFirmware.FwAttributes1, vlfBoxHwType.BoxHwTypeName,OAPPort FROM vlfBoxHwType INNER JOIN vlfFirmware ON vlfBoxHwType.BoxHwTypeId = vlfFirmware.BoxHwTypeId WHERE vlfFirmware.FwId =" + fwId;
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve fwId=" + fwId + " information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve fwId=" + fwId + " information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Get box configuration information. 
      /// </summary>
      /// <param name="selectedFwTypeId"></param>
      /// <returns>DataSet [FwId],[BoxHwTypeId],[FwName],[FwTypeId],[FwLocalPath],[FwOAPPath],[FwDateReleased],[MaxGeozones],[BoxHwTypeName],[OAPPort]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllFirmwareInfo(short selectedFwTypeId)
      {
         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
             string sql = "SELECT vlfFirmware.FwId, vlfFirmware.BoxHwTypeId, vlfFirmware.FwName, vlfFirmware.FwTypeId, vlfFirmware.FwLocalPath, vlfFirmware.FwOAPPath, vlfFirmware.FwDateReleased, vlfFirmware.MaxGeozones, vlfBoxHwType.BoxHwTypeName, OAPPort,FwVersion,vlfFirmware.FwName+' [Port:'+convert(varchar,OAPPort)+']' as FwName_Port FROM vlfFirmware INNER JOIN vlfBoxHwType ON vlfFirmware.BoxHwTypeId = vlfBoxHwType.BoxHwTypeId";
            if (selectedFwTypeId != 0)
               sql += " WHERE vlfFirmware.FwTypeId = " + selectedFwTypeId;
            sql += " ORDER BY vlfFirmware.FwDateReleased DESC";
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all configuration information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all configuration information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Retrieves all supported messages by FW channel
      /// </summary>
      /// <param name="fwChId"></param>
      /// <returns>DataSet [BoxMsgInTypeId],[BoxMsgInTypeName],[AlarmLevel]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllSupportedMessagesByFwChId(short fwChId)
      {
         Util.BTrace(Util.INF0, "-- BoxConfig.GetAllSupportedMessagesByFwChId -> FwChId[{0}]", fwChId);

         DataSet sqlDataSet = null;
         try
         {
            // 1. Prepares SQL statement
            string sql = "SELECT DISTINCT vlfBoxMsgInType.BoxMsgInTypeId, vlfBoxMsgInType.BoxMsgInTypeName, ISNULL(vlfBoxProtocolTypeMsgInType.DefaultAlarmLevel,0) AS AlarmLevel FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfBoxProtocolTypeMsgInType ON vlfChannels.BoxProtocolTypeId = vlfBoxProtocolTypeMsgInType.BoxProtocolTypeId INNER JOIN vlfBoxMsgInType ON vlfBoxProtocolTypeMsgInType.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId WHERE vlfFirmwareChannels.FwChId =" + fwChId + " AND vlfBoxMsgInType.Visible=1 ORDER BY vlfBoxMsgInType.BoxMsgInTypeName";
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attaches SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve list of all supported messages by fwChId=" + fwChId + ".";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve list of all supported messages by fwChId=" + fwChId + ".";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return sqlDataSet;
      }

      /// <summary>
      /// Retrieves primary communication info
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="userId"></param>
      /// <param name="commandID"></param>
      /// <returns>DataSet [BoxProtocolTypeId],[CommModeId]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetPrimaryCommInfo(int boxId, int userId, short commandID)
      {
         DataSet sqlDataSet = null;
         try
         {
            //Prepares SQL statement
             string sql = "SELECT DISTINCT vlfChannels.BoxProtocolTypeId, vlfChannels.CommModeId FROM vlfFirmwareChannelReference INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannelReference.FwChId = vlfBox.FwChId INNER JOIN vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfBoxProtocolTypeCmdOutType INNER JOIN vlfUserGroupAssignment INNER JOIN vlfGroupSecurity ON vlfUserGroupAssignment.UserGroupId = vlfGroupSecurity.UserGroupId ON vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId = vlfGroupSecurity.OperationId INNER JOIN vlfChannels ON vlfBoxProtocolTypeCmdOutType.BoxProtocolTypeId = vlfChannels.BoxProtocolTypeId ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfUserGroupAssignment.UserId=" + userId + " AND vlfBox.BoxId=" + boxId + " AND vlfGroupSecurity.OperationType=2 AND vlfGroupSecurity.OperationId=" + commandID + " AND vlfFirmwareChannels.ChPriority = 0";
            //Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve primary communication.";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve primary communication.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return sqlDataSet;
      }

      /// <summary>
      /// Retrieves secondary communication info
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="userId"></param>
      /// <param name="commandID"></param>
      /// <returns>DataSet [BoxProtocolTypeId],[CommModeId]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetSecondaryCommInfo(int boxId, int userId, short commandID)
      {
         DataSet sqlDataSet = null;
         try
         {
            //Prepares SQL statement
             string sql = "SELECT DISTINCT vlfChannels.BoxProtocolTypeId, vlfChannels.CommModeId FROM vlfFirmwareChannelReference INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannelReference.FwChId = vlfBox.FwChId INNER JOIN vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfBoxProtocolTypeCmdOutType INNER JOIN vlfUserGroupAssignment INNER JOIN vlfGroupSecurity ON vlfUserGroupAssignment.UserGroupId = vlfGroupSecurity.UserGroupId ON vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId = vlfGroupSecurity.OperationId INNER JOIN vlfChannels ON vlfBoxProtocolTypeCmdOutType.BoxProtocolTypeId = vlfChannels.BoxProtocolTypeId ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfUserGroupAssignment.UserId=" + userId + " AND vlfBox.BoxId=" + boxId + " AND vlfGroupSecurity.OperationType=2 AND vlfGroupSecurity.OperationId=" + commandID + " AND vlfFirmwareChannels.ChPriority = 1";
            //Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve secondary communication.";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve secondary communication.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return sqlDataSet;
      }

      /// <summary>
      ///      what is trying to do is to extract the channels defined by a firmwareId & commModeId 
      ///      as one record in record of vlfFirmwareChannels
      ///      and see if these channels are defined in the new firmwareID - in another record of vlfFirmwareChannels
      /// </summary>
      /// <param name="currFWID"></param>
      /// <param name="newFWID"></param>
      /// <param name="currCommMode"></param>
      /// <returns></returns>
      public bool IsCompatibleFirmware(short currFWID, short newFWID, short currCommMode)
      {
         Util.BTrace(Util.INF0, ">> BoxConfig.IsCompatibleFirmware -> currFWID[{0}] newFWID[{1}] CommModeId[{2}]", currFWID, newFWID, currCommMode);
         int ret = 0;

         try
         {
            using (SqlConnection conn = new SqlConnection(sqlExec.ConnectionString))
            {
               conn.Open();
               using (SqlCommand cmd = new SqlCommand("sp_IsCompatibleFirmware", conn))
               {
                  cmd.CommandType = CommandType.StoredProcedure;
                  cmd.Parameters.Add("@CurrFwId", SqlDbType.Int);
                  cmd.Parameters["@CurrFwId"].Value = currFWID;

                  cmd.Parameters.Add("@NewFwId", SqlDbType.Int);
                  cmd.Parameters["@NewFwId"].Value = newFWID;

                  cmd.Parameters.Add("@CommModeId", SqlDbType.Int);
                  cmd.Parameters["@CommModeId"].Value = currCommMode;

                  cmd.Parameters.Add("@Ret", SqlDbType.Int);
                  cmd.Parameters["@Ret"].Value = ret;
                  cmd.Parameters["@Ret"].Direction = ParameterDirection.Output;

                  cmd.ExecuteScalar();
                  Util.BTrace(Util.INF0, "BoxConfig.IsCompatibleFirmware -> after store procedure ");
                  ret = Convert.ToInt16(cmd.Parameters["@Ret"].Value);

               }
            }
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("BoxConfig.IsCompatibleFirmware -> SQLException", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException("BoxConfig.IsCompatibleFirmware -> Exception" + objException.Message);
         }

         Util.BTrace(Util.INF0, "<< BoxConfig.IsCompatibleFirmware -> RET [{0}]", ret);
         return (ret > 0);
      }

      /// <summary>
      /// Get all firmware types information. 
      /// </summary>
      /// <returns>DataSet[FwTypeId],[FwTypeName]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet FirmwareTypes()
      {
         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT FwTypeId, FwTypeName FROM vlfFirmwareType ORDER BY FwTypeName";
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all configuration information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all configuration information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Get all hardware types information. 
      /// </summary>
      /// <returns>DataSet[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet HardwareTypes()
      {
         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT BoxHwTypeId, BoxHwTypeName, MaxSensorsNum, MaxOutputsNum FROM vlfBoxHwType WHERE BoxHwTypeId > 0 ORDER BY BoxHwTypeName";
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all configuration information. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all configuration information. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Retrieves Channels info. 	
      /// </summary>
      /// <returns>DataSet [ChId],[ChName],[BoxProtocolTypeId],[CommModeId]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetChannelsInfo()
      {

         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT ChId, ChName, BoxProtocolTypeId, CommModeId FROM vlfChannels WHERE ChId > 0";

            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all CommMode info";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all CommMode info";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// </summary>
      /// <returns>DataSet [CommModeId],[CommModeName]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetCommModesInfo()
      {

         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT CommModeId, CommModeName FROM vlfCommMode WHERE CommModeId > 0 ORDER BY CommModeName DESC";

            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve all CommMode info";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve all CommMode info";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Checks if both FW have the same primary protocol
      /// </summary>
      /// <param name="oldFwChId "></param>
      /// <param name="newFwChId "></param>
      /// <returns>true if it is same, otherwise returns false </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public bool IsSamePrimaryProtocol(short oldFwChId, short newFwChId)
      {
         //         return true;

         bool retResult = false;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT COUNT(*) FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfFirmwareChannels.FwId=" + oldFwChId.ToString() + " AND vlfFirmwareChannels.ChPriority=0 AND vlfChannels.BoxProtocolTypeId IN (SELECT DISTINCT vlfChannels.BoxProtocolTypeId FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfFirmwareChannels.FwId = " + newFwChId.ToString() + " AND vlfFirmwareChannels.ChPriority = 0)";

            //Executes SQL statement
            int recordCount = (int)sqlExec.SQLExecuteScalar(sql);
            if (recordCount > 0)
               retResult = true;
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to check box configuration by oldFwChId=" + oldFwChId + " and newFwChId " + newFwChId + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to check box configuration by oldFwChId=" + oldFwChId + " and newFwChId " + newFwChId + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return retResult;

      }

      /// <summary>
      /// Get firmware CH id channels by FwId and CommModeId
      /// </summary>
      /// <param name="fwId"></param>
      /// <param name="commModeId"></param>
      /// <param name="isDual"></param>
      /// <returns>Fw Ch Id</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public short GetFwChId(short fwId, short commModeId, bool isDual)
      {
         Util.BTrace(Util.INF0, ">> BoxConfig.GetFwChId -> FwId[{0}] CommModeId[{1}] IsDual[{2}]", fwId, commModeId, isDual);
         short retResult = VLF.CLS.Def.Const.unassignedShortValue;
#if  false
			try 
			{
				//Prepares SQL statement
				string sql = "DECLARE @FwId INT DECLARE @CommModeId INT SET @FwId=" + fwId + " SET @CommModeId=" + commModeId + " SELECT vlfFirmwareChannels.FwChId FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE (vlfFirmwareChannels.FwId = @FwId) AND (vlfFirmwareChannels.FwChId IN (SELECT vlfFirmwareChannels.FwChId FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE (vlfFirmwareChannels.FwId = @FwId) AND (vlfChannels.CommModeId = @CommModeId))) AND (vlfFirmwareChannels.ChPriority = 0)";
				if(isDual)
					sql += " AND vlfFirmwareChannels.FwChId NOT IN";
				else
					sql += " AND vlfFirmwareChannels.FwChId  IN";
				sql += " ( SELECT vlfFirmwareChannels.FwChId FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE (vlfFirmwareChannels.FwId = @FwId) AND (vlfFirmwareChannels.FwChId IN (SELECT vlfFirmwareChannels.FwChId FROM vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE (vlfFirmwareChannels.FwId = @FwId) AND (vlfChannels.CommModeId = @CommModeId))) AND (vlfFirmwareChannels.ChPriority = 1) ) ORDER BY vlfFirmwareChannels.FwChId";

            Util.BTrace(Util.INF1, sql);

				//Executes SQL statement
				object obj = sqlExec.SQLExecuteScalar(sql);
				if(obj != System.DBNull.Value)
					retResult = Convert.ToInt16(obj);
#else
         /// this is the implementation using a store procedure 
         /// @FwId INT, @CommModeId INT, @IsDual BIT, @Ret int out
         int ret = 0;

         try
         {
            using (SqlConnection conn = new SqlConnection(sqlExec.ConnectionString))
            {
               conn.Open();
               using (SqlCommand cmd = new SqlCommand("sp_GetFirmwareChannelID", conn))
               {
                  cmd.CommandType = CommandType.StoredProcedure;
                  cmd.Parameters.Add("@FwId", SqlDbType.Int);
                  cmd.Parameters["@FwId"].Value = fwId;

                  cmd.Parameters.Add("@CommModeId", SqlDbType.Int);
                  cmd.Parameters["@CommModeId"].Value = commModeId;

                  cmd.Parameters.Add("@IsDual", SqlDbType.Bit);
                  cmd.Parameters["@IsDual"].Value = isDual ? 1 : 0;

                  cmd.Parameters.Add("@Ret", SqlDbType.Int);
                  cmd.Parameters["@Ret"].Value = ret;
                  cmd.Parameters["@Ret"].Direction = ParameterDirection.Output;

                  cmd.ExecuteNonQuery();
                  Util.BTrace(Util.INF0, "---- after store procedure ----");
                  retResult = Convert.ToInt16(cmd.Parameters["@Ret"].Value);

               }
            }
#endif
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve firmware channel information by fwId=" + fwId + " commModeId=" + commModeId + " isDual=" + isDual + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve firmware channel information by fwId=" + fwId + " commModeId=" + commModeId + " isDual=" + isDual + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         Util.BTrace(Util.INF0, "<< BoxConfig.GetFwChId -> RET FwChId[{0}]", retResult);
         return retResult;
      }

      /// <summary>
      /// Retrieves max record index from specific table
      /// </summary>
      /// <param name="primaryKeyFieldName"></param>
      /// <returns>max record index</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      protected short GetMaxFWId()
      {
         short maxRecordIndex = -1;
         string prefixMsg = "Unable to retrieve max record index from 'vlfFirmware' table.";
         try
         {
            Object result = GetMaxValue("FwId");
            if (result != null)
            {
               maxRecordIndex = Convert.ToInt16(result);
            }
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return maxRecordIndex;
      }

      /// <summary>
      /// Add new firmware.
      /// </summary>
      /// <param name="boxHwTypeId"></param>
      /// <param name="fwName"></param>
      /// <param name="fwTypeId"></param>
      /// <param name="fwLocalPath"></param>
      /// <param name="fwOAPPath"></param>
      /// <param name="fwDateReleased"></param>
      /// <param name="maxGeozones"></param>
      /// <param name="oAPPort"></param>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void AddFirmware(short boxHwTypeId, string fwName, short fwTypeId, string fwLocalPath, string fwOAPPath, string fwDateReleased, int maxGeozones, short oAPPort)
      {
         int rowsAffected = 0;
         // 4. Prepares SQL statement
         string sql = string.Format("INSERT INTO vlfFirmware (FwId,BoxHwTypeId,FwName,FwTypeId,FwLocalPath,FwOAPPath,FwDateReleased,MaxGeozones,OAPPort)" +
                 " VALUES ( {0},{1},'{2}', {3},'{4}','{5}','{6}',{7},{8})",
                 GetMaxFWId() + 1,
                 boxHwTypeId,
                 fwName.Replace("'", "''"),
                 fwTypeId,
                 fwLocalPath.Replace("'", "''"),
                 fwOAPPath.Replace("'", "''"),
                 fwDateReleased,
                 maxGeozones,
                 oAPPort);
         try
         {
            if (sqlExec.RequiredTransaction())
            {
               // 5. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }

            // 6. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to add new firmware.";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to add new firmware.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            string prefixMsg = "Unable to add new firmware.";
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This firmware already exists.");
         }
      }

      /// <summary>
      /// Add new firmware overloaded - includes feature mask
      /// </summary>
      /// <param name="boxHwTypeId"></param>
      /// <param name="fwName"></param>
      /// <param name="fwTypeId"></param>
      /// <param name="fwLocalPath"></param>
      /// <param name="fwOAPPath"></param>
      /// <param name="fwDateReleased"></param>
      /// <param name="maxGeozones"></param>
      /// <param name="oAPPort"></param>
      /// <param name="featureMask"></param>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void AddFirmware(short boxHwTypeId, string fwName, short fwTypeId, string fwLocalPath, string fwOAPPath,
         string fwDateReleased, int maxGeozones, short oAPPort, long featureMask)
      {
         int rowsAffected = 0;
         string prefixMsg = "Unable to add new firmware.";
         try
         {
            short fwId = GetMaxFWId();
            if (fwId < 0)
               throw new Exception("Error getting Max Fw ID value");
            // next id
            fwId++;
            // 4. Prepares SQL statement
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@fwId", SqlDbType.SmallInt, fwId);
            sqlExec.AddCommandParam("@hwTypeId", SqlDbType.SmallInt, boxHwTypeId);
            sqlExec.AddCommandParam("@fwName", SqlDbType.VarChar, fwName.Replace("'", "''"));
            sqlExec.AddCommandParam("@fwTypeId", SqlDbType.SmallInt, fwTypeId);
            sqlExec.AddCommandParam("@oapPath", SqlDbType.VarChar, fwOAPPath.Replace("'", "''"));
            sqlExec.AddCommandParam("@localPath", SqlDbType.VarChar, fwLocalPath.Replace("'", "''"));
            sqlExec.AddCommandParam("@dtReleased", SqlDbType.DateTime, fwDateReleased);
            sqlExec.AddCommandParam("@maxGeozones", SqlDbType.Int, maxGeozones);
            sqlExec.AddCommandParam("@oapPort", SqlDbType.SmallInt, oAPPort);
            sqlExec.AddCommandParam("@featMask", SqlDbType.BigInt, featureMask);

            string sql = 
               String.Format(
               "INSERT INTO vlfFirmware (FwId, BoxHwTypeId, FwName, FwTypeId, FwLocalPath, FwOAPPath, FwDateReleased, MaxGeozones, OAPPort, FwAttributes1)" +
               " VALUES (@fwId, @hwTypeId, @fwName, @fwTypeId, @localPath, @oapPath, @dtReleased, @maxGeozones, @oapPort, @featMask)");

            if (sqlExec.RequiredTransaction())
            {
               // 5. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }

            // 6. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This firmware already exists.");
         }
      }

      /// <summary>
      /// Update firmware info.
      /// </summary>
      /// <param name="fwId"></param>
      /// <param name="fwLocalPath"></param>
      /// <param name="fwOAPPath"></param>
      /// <param name="maxGeozones"></param>
      /// <param name="oAPPort"></param>
      /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if FW does not exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void UpdateFirmwareInfo(short fwId, string fwLocalPath, string fwOAPPath, int maxGeozones, short oAPPort)
      {
         int rowsAffected = 0;
         string sql = "UPDATE vlfFirmware SET FwLocalPath='" + fwLocalPath.Replace("'", "''") + "'" +
             ",FwOAPPath='" + fwOAPPath.Replace("'", "''") + "'" +
             ",MaxGeozones=" + maxGeozones +
             ",OAPPort=" + oAPPort +
             " WHERE FwId=" + fwId;
         try
         {
            if (sqlExec.RequiredTransaction())
            {
               // 5. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }

            // 6. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to update firmware info.";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to update firmware info.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            string prefixMsg = "Unable to update firmware info.";
            throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This firmware does not exist.");
         }
      }

      /// <summary>
      /// Add new FwChId.
      /// </summary>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public short AddFwChId()
      {
         short fwChId = VLF.CLS.Def.Const.unassignedIntValue;
         string sql = "DECLARE @FwChId int  SELECT @FwChId=MAX(FwChId)+1 FROM vlfFirmwareChannelReference INSERT INTO vlfFirmwareChannelReference (FwChId) VALUES (@FwChId) SELECT @FwChId";
         try
         {
            sqlExec.ClearCommandParameters();
            if (sqlExec.RequiredTransaction())
            {
               // 5. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }

            object obj = sqlExec.SQLExecuteScalar(sql);
            if (obj != System.DBNull.Value)
               fwChId = Convert.ToInt16(obj);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to add new firmware Ch Id.";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to add new firmware Ch Id.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (fwChId < 0)
         {
            string prefixMsg = "Unable to add new firmware Ch Id.";
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This firmware Ch Id already exists.");
         }
         return fwChId;
      }

      /// <summary>
      /// Add new firmware configuration
      /// </summary>
      /// <param name="fwChId"></param>
      /// <param name="chId"></param>
      /// <param name="fwId"></param>
      /// <param name="chPriority">0 - primary, 1 - secondary</param>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void AddFirmwareCfg(short fwChId, short chId, short fwId, short chPriority)
      {
         int rowsAffected = 0;
         string sql = "INSERT INTO vlfFirmwareChannels (FwChId, ChId, FwId, ChPriority) VALUES (@fwChId, @chId, @fwId, @chPriority)";
         try
         {
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@fwChId", SqlDbType.SmallInt, fwChId);
            sqlExec.AddCommandParam("@chId", SqlDbType.SmallInt, chId);
            sqlExec.AddCommandParam("@fwId", SqlDbType.SmallInt, fwId);
            sqlExec.AddCommandParam("@chPriority", SqlDbType.SmallInt, chPriority);
            if (sqlExec.RequiredTransaction())
            {
               // 5. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }

            // 6. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to add new firmware cfg.";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to add new firmware cfg.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            string prefixMsg = "Unable to add new firmware cfg.";
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This firmware cfg already exists.");
         }
      }

      /// <summary>
      /// Delete Fw Id.
      /// </summary>
      /// <returns>void</returns>
      /// <param name="fwId"></param> 
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteFirmware(short fwId)
      {
         return DeleteRowsByIntField("FwId", fwId, "fw id");
      }

      /// <summary>
      /// Deletes firmware channel configuration
      /// </summary>
      /// <param name="fwChId">Firmware-channel Id</param>
      /// <returns>Number of rows affected</returns>
      /// <exception cref="DASAppResultNotFoundException">Thrown if data does not exist.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteFirmwareCfg(short fwChId)
      {
         int rowsAffected = 0;
         string prefixMsg = "Error deleting firmware channel Id = " + fwChId;
         object result = null;
         string sql = "";
         try
         {
            // 1. Prepares SQL statement
             sql = "SELECT COUNT(*) AS fwChIdCount FROM vlfFirmwareChannels INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannels.FwChId = vlfBox.FwChId WHERE vlfFirmwareChannels.FwChId =" + fwChId;
            // 2. Executes SQL statement - result > 0 if boxes with that fwchid were found
            result = sqlExec.SQLExecuteScalar(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         if (result == System.DBNull.Value || Convert.ToInt16(result) != 0)
            throw new VLF.ERR.DASAppViolatedIntegrityConstraintsException("Unable to delete " + fwChId + " FwChId. There are boxes using this configuration.");

         try
         {
            // 3. Executes SQL statement - delete configuration if there are no boxes using it
            sqlExec.BeginTransaction();
            sql = String.Format("DELETE FROM vlfFirmwareChannels WHERE FwChId={0} DELETE FROM vlfFirmwareChannelReference WHERE FwChId={0}", fwChId);
            if (sqlExec.RequiredTransaction())
            {
               // 4. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

            sqlExec.CommitTransaction();
         }
         catch (SqlException objException)
         {
            sqlExec.RollbackTransaction();
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (Exception objException)
         {
            sqlExec.RollbackTransaction();
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return rowsAffected;
      }
   }
}
