using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SentinelFM;
using VLF.CLS;
using VLF.CLS.Def;
using VLF.DAS.DB;
using VLF.ERR;
//Author: HL

namespace VLF.DAS.Logic
{
  public class BoxProfile : Box
  {
    private DB.BoxProfileDB boxProfileDb;

    public BoxProfile(string connectionString) : base(connectionString)
		{
		  boxProfileDb = new DB.BoxProfileDB(sqlExec);
    }
    /// <summary>
    ///Get Wi-Fi Command List from DB to fill Wi-fi command dropdownlist in Send Command Page
    /// </summary>
    /// <param name="boxMode"></param>
    /// <returns></returns>
    public List<WiFiUpdateCommand> GetWiFiUpdateCommandList(int boxMode)
    {
        return boxProfileDb.GetWiFiUpdateCommandList(boxMode); 
    }

    /// <summary>
    /// Retieves box BoxStatus string from vlfBox or vlfMsgInHst
    /// </summary>
    /// <param name="boxId"></param>
    /// <returns></returns>
    public string GetBoxLatestBoxStatus(int boxId)
    {
      return boxProfileDb.GetBoxLatestBoxStatus(boxId);
    }
    /// <summary>
    /// Check Box is Linux Wi-Fi box
    /// </summary>
    /// <param name="boxId"></param>
    /// <returns></returns>
    public bool IsLinuxWiFiSupportBox(int boxId)
    {
      string boxStatus = GetBoxLatestBoxStatus(boxId);
      return boxStatus.Contains("WIFI1");
    }
  }
}
