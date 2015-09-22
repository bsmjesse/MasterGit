using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Web;
using System.Web.Services;
using VLF.ERRSecurity;
using VLF.ERR;
using VLF.DAS.Logic;
using VLF.CLS;

namespace VLF.ASI.Interfaces
{
  [WebService (Namespace = "http://www.sentinelfm.com")]

  /// <summary>
  ///     Services for the Sentinel Administration services. Currently, focus is on network traffic monitoring
  /// </summary>
  public class DBAdmin : System.Web.Services.WebService
  {
    public DBAdmin ()
    {
      //Uncomment the following line if using designed components 
      //InitializeComponent(); 
    }

    #region refactored functions
    /// <summary>
    /// </summary>
    /// <param name="strFormat"></param>
    /// <param name="objects"></param>
    private void Log(string strFormat, params object[] objects)
    {
        try
        {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                 CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                   string.Format(strFormat, objects)));
        }
        catch (Exception exc)
        {

        }
    }

      private void LogException(string strFormat, params object[] objects)
      {
          try
          {
              Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceError,
                   CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Error,
                                     string.Format(strFormat, objects)));
          }
          catch (Exception exc)
          {

          }
      }
    #endregion

    [WebMethod] //Global, per box
    public string Admin_GetTopMessageGenerators (DateTime fromDate, DateTime toDate, ref string xml)
    {
      /*
       * Returns [BoxId]	[OrganizationName] [MessageCounter]
	                8507		 CN			            2017
	                7641		 CN			            1805
       */

      xml = "";

      if (fromDate.CompareTo (toDate) <= 0)
      {
        return xml;
      }

      try
      {
        SqlCommand getTopGenerators = new SqlCommand ("Admin_GetTopMessageGenerators", new SqlConnection
                                      (ConfigurationManager.ConnectionStrings ["SentinelFM"].ConnectionString));
        
        DataSet dsTopGenerators = new DataSet ();

        getTopGenerators.CommandTimeout = 300;
        getTopGenerators.CommandType = CommandType.StoredProcedure;

        getTopGenerators.Parameters.Add ("@fromDate", SqlDbType.DateTime);
        getTopGenerators.Parameters.Add ("@toDate", SqlDbType.DateTime);

        getTopGenerators.Parameters ["@fromDate"].Value = fromDate;
        getTopGenerators.Parameters ["@toDate"].Value = toDate;
        
        SqlDataAdapter sqlAdapter = new SqlDataAdapter (getTopGenerators);
        sqlAdapter.Fill (dsTopGenerators);

        xml = dsTopGenerators.GetXml ();
      }
      catch
      {
        xml = "";
      }

      return xml;
    }

    [WebMethod] //Single global organization, overall incidents (alarms/notifications)
    public string Admin_IncidentsPerOrganization (int orgId, DateTime fromDate, DateTime toDate, ref string xml)
    {
      /*
       * Returns four counters: 
       *   - CntAlarms (alarm counter)
       *   - CntAlarmsAck (alarms acknowledged)
       *   - CntNotifications (notification counter)
       *   - CntNotificationsAck (notifications acknowledged)
       */

      xml = "";

      try
      {
        SqlCommand IncidentsPerOrganization = new SqlCommand ("Admin_IncidentsPerOrganization", new SqlConnection
                                             (ConfigurationManager.ConnectionStrings ["SentinelFM"].ConnectionString));
        DataSet dsIncidents = new DataSet ();

        IncidentsPerOrganization.CommandTimeout = 300;
        IncidentsPerOrganization.CommandType = CommandType.StoredProcedure;

        IncidentsPerOrganization.Parameters.Add ("@organizationId", SqlDbType.Int);
        IncidentsPerOrganization.Parameters.Add ("@fromDate", SqlDbType.DateTime);
        IncidentsPerOrganization.Parameters.Add ("@toDate", SqlDbType.DateTime);

        IncidentsPerOrganization.Parameters ["@organizationId"].Value = orgId;
        IncidentsPerOrganization.Parameters ["@fromDate"].Value = fromDate;
        IncidentsPerOrganization.Parameters ["@toDate"].Value = toDate;

        SqlDataAdapter sqlAdapter = new SqlDataAdapter (IncidentsPerOrganization);
        sqlAdapter.Fill (dsIncidents);

        xml = dsIncidents.GetXml ();
      }
      catch
      {
        xml = "";
      }

      return xml;
    }

    [WebMethod] //Single global box, per message type
    public string Admin_MessageDistributionPerBox (int boxId, DateTime fromDate, DateTime toDate, ref string xml)
    {
      /*
       * Returns [BoxMsgInTypeId]  [BoxMsgInTypeName] [MessageCounter]
                  1			            ScheduleUpdate	   8
                  2			            Sensor		         22
       */

      xml = "";

      try
      {
        SqlCommand MessageDistribution = new SqlCommand ("Admin_MessageDistributionPerBox", new SqlConnection
                                         (ConfigurationManager.ConnectionStrings ["SentinelFM"].ConnectionString));
        DataSet dsDistribution = new DataSet ();

        MessageDistribution.CommandTimeout = 300;
        MessageDistribution.CommandType = CommandType.StoredProcedure;

        MessageDistribution.Parameters.Add ("@boxId", SqlDbType.Int);
        MessageDistribution.Parameters.Add ("@fromDate", SqlDbType.DateTime);
        MessageDistribution.Parameters.Add ("@toDate", SqlDbType.DateTime);

        MessageDistribution.Parameters ["@boxId"].Value = boxId;
        MessageDistribution.Parameters ["@fromDate"].Value = fromDate;
        MessageDistribution.Parameters ["@toDate"].Value = toDate;

        SqlDataAdapter sqlAdapter = new SqlDataAdapter (MessageDistribution);
        sqlAdapter.Fill (dsDistribution);

        xml = dsDistribution.GetXml ();
      }
      catch
      {
        xml = "";
      }

      return xml;
    }

    [WebMethod] //Global, per organization
    public string Admin_TrafficPerOrganization (DateTime fromDate, DateTime toDate, ref string xml)
    {
      /*
       * Returns [OrganizationName] [OrganizationId] [MessageCounter]
                  Bantek	       		 3			          69358
                  CN			           123			        65997
                  ATG-Ryder		       127			        52246
       */

      xml = "";

      if (fromDate.CompareTo (toDate) <= 0)
      {
        return xml;
      }

      try
      {
        SqlCommand TrafficPerOrganization = new SqlCommand ("Admin_TrafficPerOrganization", new SqlConnection
                                    //(ConfigurationManager.ConnectionStrings ["SentinelFM"].ConnectionString));
        ("Initial Catalog=SentinelFM;Data Source=192.168.7.17;User ID=ra;Password=userReads69;Pooling=true;Max Pool Size=300;"));
        
        DataSet dsTraffic = new DataSet ();

        TrafficPerOrganization.CommandTimeout = 300;
        TrafficPerOrganization.CommandType = CommandType.StoredProcedure;

        TrafficPerOrganization.Parameters.Add ("@fromDate", SqlDbType.DateTime);
        TrafficPerOrganization.Parameters.Add ("@toDate", SqlDbType.DateTime);

        TrafficPerOrganization.Parameters ["@fromDate"].Value = fromDate;
        TrafficPerOrganization.Parameters ["@toDate"].Value = toDate;

        SqlDataAdapter sqlAdapter = new SqlDataAdapter (TrafficPerOrganization);
        sqlAdapter.Fill (dsTraffic);

        xml = dsTraffic.GetXml ();
      }
      catch
      {
        xml = "";
      }

      return xml;
    }

    [WebMethod] //Global, per user
    public string Admin_WebUserStatistics (ref string xml)
    {
      xml = "";

      return "xml string here (TODO: Make this method return error code)";
  }


    }
}

