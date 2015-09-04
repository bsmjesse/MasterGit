using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SentinelFM;
using VLF.CLS.Def;
using VLF.ERR;
using VLF.CLS;
//Author: HL

namespace VLF.DAS.DB
{
  /// <summary>
  /// Provides interfaces to vlfBox table.
  /// </summary>
  public class BoxProfileDB : TblGenInterfaces
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sqlExec"></param>
    public BoxProfileDB(SQLExecuter sqlExec)
      : base("vlfBox", sqlExec)
    {
    }

    /// <summary>
    /// GetBoxLatestBoxStatus by BoxID, this method for checking if box is a Linux Wi-Fi Box.
    /// </summary>
    /// <param name="boxId"></param>
    /// <exception cref="DASAppWrongResultException">Thrown DASAppWrongResultException if Unexpected multiple rows result.</exception>
    /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
    /// <exception cref="DASException">Thrown in all other exception cases.</exception>
    public string GetBoxLatestBoxStatus(int boxId)
    {
      string boxStatus = string.Empty;
      DataSet resultDataSet = null;

      try
      {
        sqlExec.ClearCommandParameters();
        //Prepares SQL sp adding params

        sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
        sqlExec.AddCommandParam("@MessagetTypeId", SqlDbType.Int, (int)Enums.MessageType.BoxStatus);

        //Executes SQL statement
        resultDataSet = sqlExec.SPExecuteDataset("[dbo].[aota_GetBoxLatestBoxStatus]");
      }
      catch (SqlException objException)
      {
        string prefixMsg = "Unable to retrieve box id=" + boxId + ". ";
        Util.ProcessDbException(prefixMsg, objException);
      }
      catch (DASDbConnectionClosed exCnn)
      {
        throw new DASDbConnectionClosed(exCnn.Message);
      }
      catch (Exception objException)
      {
        string prefixMsg = "Unable to retrieve box id=" + boxId + ". ";
        throw new DASException(prefixMsg + " " + objException.Message);
      }

      //return resultDataSet;
      if (resultDataSet != null && resultDataSet.Tables.Count > 0 && resultDataSet.Tables[0].Rows.Count > 0)
      {
        if (resultDataSet.Tables[0].Rows.Count == 1)
        {
           boxStatus = Convert.ToString(resultDataSet.Tables[0].Rows[0]["CustomProp"]);
        }
        else
        {
          throw new DASAppWrongResultException("Unexpected multiple rows result.");
        }
      }

      return boxStatus;
    }

    /// <summary>
    /// Get Wi-Fi Commands from Database
    /// </summary>
    /// <param name="boxMode"></param>
    /// <returns></returns>
     public List<WiFiUpdateCommand> GetWiFiUpdateCommandList(int boxMode)
    {
      List<WiFiUpdateCommand> commandList = new List<WiFiUpdateCommand>();
  
      string boxStatus = string.Empty;
      DataSet resultDataSet = null;

      try
      {
        sqlExec.ClearCommandParameters();
        //Prepares SQL sp adding params

        sqlExec.AddCommandParam("@boxModel", SqlDbType.Int, boxMode);
        //Executes SQL statement
        resultDataSet = sqlExec.SPExecuteDataset("[dbo].[usp_GetWiFiUpdateCommandList]");
      }
      catch (SqlException objException)
      {
        string prefixMsg = "Unable to WiFiUpdateCommand for modelId=" + boxMode + ". ";
        Util.ProcessDbException(prefixMsg, objException);
      }
      catch (DASDbConnectionClosed exCnn)
      {
        throw new DASDbConnectionClosed(exCnn.Message);
      }
      catch (Exception objException)
      {
        string prefixMsg = "Unable to retrieve WiFI command for BoxModel=" + boxMode + ". ";
        throw new DASException(prefixMsg + " " + objException.Message);
      }

      //return resultDataSet;
      if (resultDataSet != null && resultDataSet.Tables.Count > 0 && resultDataSet.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow dr in resultDataSet.Tables[0].Rows)
        {
          WiFiUpdateCommand wifiCommand = new WiFiUpdateCommand();
      
          wifiCommand.WiFiCommandId = Convert.ToInt32(dr["WiFiCommandId"]);
          wifiCommand.WiFiCommandCode = Convert.ToInt32(dr["WiFiCommandCode"]);
          wifiCommand.WiFiCommandName = Convert.ToString(dr["WiFiCommandName"]);
          wifiCommand.WiFiCommandDefault = dr["WiFiCommandDefault"] == null? "": Convert.ToString(dr["WiFiCommandDefault"]);

          commandList.Add(wifiCommand);
        }
      }

      return commandList;
    }

    /// <summary>
    /// Call Stored Procedure to get WiFi Commands
    /// </summary>
    /// <param name="boxMode"></param>
    /// <param name="ConnectionString"></param>
    /// <returns></returns>
    public List<WiFiUpdateCommand> GetWiFiUpdateCommandListSP(int boxMode, string ConnectionString)
    {
      List<WiFiUpdateCommand> commandList = new List<WiFiUpdateCommand>();
      string storedProcedure = "[dbo].[usp_GetWiFiUpdateCommandList]";

      try
      {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
          if (connection.State != ConnectionState.Open)
          {
            connection.Open();
          }

          using (SqlCommand command = new SqlCommand(storedProcedure, connection))
          {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@boxModel", boxMode));

            using (SqlDataReader reader = command.ExecuteReader())
            {
              if (reader.HasRows)
              {
                while (reader.Read())
                {
                  WiFiUpdateCommand wifiCommand = new WiFiUpdateCommand();
                  wifiCommand.WiFiCommandId = Convert.ToInt32(reader["WiFiCommandId"]);
                  wifiCommand.WiFiCommandCode = Convert.ToInt32(reader["WiFiCommandCode"]);
                  wifiCommand.WiFiCommandName = (string)reader["WiFiCommandName"];
                  wifiCommand.WiFiCommandDefault = reader["WiFiCommandDefault"] == null ? "" : Convert.ToString(reader["WiFiCommandDefault"]);
                  commandList.Add(wifiCommand);
                }
              } //if
            }//using reader
          }//using command
        } //using conn
      }
      catch (Exception ex)
      {
        //LogUtil.Error("Error in aota_GetBoxLatestBoxStatus stored procedure:");
        //LogUtil.Error(": Error : " + ex.Message);
      }

      return commandList;
    }






    /// <summary>
    /// Template, no function call this method now.
    /// </summary>
    /// <param name="boxId"></param>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    private string GetBoxLatestBoxStatusBySP(int boxId, string connectionString)
    {
      string boxStatus = string.Empty;

      string storedProcedure = "[dbo].[aota_GetBoxLatestBoxStatus]";

      try
      {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
          if (connection.State != ConnectionState.Open)
          {
            connection.Open();
          }

          using (SqlCommand command = new SqlCommand(storedProcedure, connection))
          {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@BoxId", boxId));
            command.Parameters.Add(new SqlParameter("@MessagetTypeId", (int)Enums.MessageType.BoxStatus));

            using (SqlDataReader reader = command.ExecuteReader())
            {
              if (reader.HasRows)
              {
                while (reader.Read())
                {
                  boxStatus = (string)reader["CustomProp"];
                }
              } //if
            }//using reader
          }//using command
        } //using conn
      }
      catch (Exception ex)
      {

      }

      return boxStatus;
    }


  }
}
