using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Contains functions to perform queries on SQL database and return DataSet objects.
/// </summary>
public static class DBQueries
{
    public static DataSet GetMsgsAndCmds(short protocol, bool messages, bool commands)
    {
        SqlCommand sqlGetMsgsAndCmds = new SqlCommand("sp_TV_GetMsgsAndCmds", new SqlConnection(ConfigurationManager.ConnectionStrings["SentinelFM"].ConnectionString));
        SqlDataAdapter sqlAdapter;
        DataSet dsGetMsgsAndCmds = new DataSet();

        sqlGetMsgsAndCmds.CommandTimeout = 300;
        sqlGetMsgsAndCmds.CommandType = CommandType.StoredProcedure;
        sqlGetMsgsAndCmds.Parameters.Clear();
        sqlGetMsgsAndCmds.Parameters.Add("@intProtocol", SqlDbType.SmallInt);
        sqlGetMsgsAndCmds.Parameters.Add("@blnMessages", SqlDbType.Bit);
        sqlGetMsgsAndCmds.Parameters.Add("@blnCommands", SqlDbType.Bit);
        sqlGetMsgsAndCmds.Parameters["@intProtocol"].Value = protocol;
        sqlGetMsgsAndCmds.Parameters["@blnMessages"].Value = messages;
        sqlGetMsgsAndCmds.Parameters["@blnCommands"].Value = commands;
        sqlAdapter = new SqlDataAdapter(sqlGetMsgsAndCmds);
        sqlAdapter.Fill(dsGetMsgsAndCmds);
        return dsGetMsgsAndCmds;
    }

    public static DataSet AdvancedSearch(DateTime from, DateTime to, int direction, object frequency, object protocol, object organization, object fleet, object box, bool bytes, object msgTypes, object cmdTypes)
    {
        SqlCommand sqlAdvancedSearch = new SqlCommand("sp_TV_AdvancedSearch", new SqlConnection(ConfigurationManager.ConnectionStrings["SentinelFM"].ConnectionString));
        SqlDataAdapter sqlAdapter;
        DataSet dsAdvancedSearch = new DataSet();

        sqlAdvancedSearch.CommandTimeout = 300;
        sqlAdvancedSearch.CommandType = CommandType.StoredProcedure;
        sqlAdvancedSearch.Parameters.Clear();
        sqlAdvancedSearch.Parameters.Add("@dtFrom", SqlDbType.DateTime);
        sqlAdvancedSearch.Parameters.Add("@dtTo", SqlDbType.DateTime);
        sqlAdvancedSearch.Parameters.Add("@intDirection", SqlDbType.Int);
        sqlAdvancedSearch.Parameters.Add("@intFrequency", SqlDbType.Int);
        sqlAdvancedSearch.Parameters.Add("@intProtocol", SqlDbType.SmallInt);
        sqlAdvancedSearch.Parameters.Add("@intOrganization", SqlDbType.Int);
        sqlAdvancedSearch.Parameters.Add("@intFleet", SqlDbType.Int);
        sqlAdvancedSearch.Parameters.Add("@intBox", SqlDbType.Int);
        sqlAdvancedSearch.Parameters.Add("@blnBytes", SqlDbType.Bit);
        sqlAdvancedSearch.Parameters.Add("@strMsgTypes", SqlDbType.VarChar, 1000);
        sqlAdvancedSearch.Parameters.Add("@strCmdTypes", SqlDbType.VarChar, 1000);
        sqlAdvancedSearch.Parameters["@dtFrom"].Value = from;
        sqlAdvancedSearch.Parameters["@dtTo"].Value = to;
        sqlAdvancedSearch.Parameters["@intDirection"].Value = direction;
        sqlAdvancedSearch.Parameters["@intFrequency"].Value = frequency;
        sqlAdvancedSearch.Parameters["@intProtocol"].Value = protocol;
        sqlAdvancedSearch.Parameters["@intOrganization"].Value = organization;
        sqlAdvancedSearch.Parameters["@intFleet"].Value = fleet;
        sqlAdvancedSearch.Parameters["@intBox"].Value = box;
        sqlAdvancedSearch.Parameters["@blnBytes"].Value = bytes;
        sqlAdvancedSearch.Parameters["@strMsgTypes"].Value = msgTypes;
        sqlAdvancedSearch.Parameters["@strCmdTypes"].Value = cmdTypes;
        sqlAdapter = new SqlDataAdapter(sqlAdvancedSearch);
        sqlAdapter.Fill(dsAdvancedSearch);
        return dsAdvancedSearch;
    }

    public static DataSet GetTopUsers(DateTime from, DateTime to, int userType, int topCount, object organizationForFleet, bool bytes)
    {
        SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SentinelFM"].ConnectionString);
        SqlCommand sqlGetTopUsers = new SqlCommand();
        SqlDataAdapter sqlAdapter;
        DataSet dsGetTopUsers = new DataSet();

        sqlGetTopUsers.CommandText = "sp_TV_GetTopUsers";
        sqlGetTopUsers.CommandTimeout = 300;
        sqlGetTopUsers.CommandType = CommandType.StoredProcedure;
        sqlGetTopUsers.Connection = sqlConn;
        sqlGetTopUsers.Parameters.Clear();
        sqlGetTopUsers.Parameters.Add("@dtFrom", SqlDbType.DateTime);
        sqlGetTopUsers.Parameters.Add("@dtTo", SqlDbType.DateTime);
        sqlGetTopUsers.Parameters.Add("@intUserType", SqlDbType.Int);
        sqlGetTopUsers.Parameters.Add("@intTopCount", SqlDbType.Int);
        sqlGetTopUsers.Parameters.Add("@intOrganizationForFleet", SqlDbType.Int);
        sqlGetTopUsers.Parameters.Add("@blnBytes", SqlDbType.Bit);
        sqlGetTopUsers.Parameters["@dtFrom"].Value = from;
        sqlGetTopUsers.Parameters["@dtTo"].Value = to;
        sqlGetTopUsers.Parameters["@intUserType"].Value = userType;
        sqlGetTopUsers.Parameters["@intTopCount"].Value = topCount;
        sqlGetTopUsers.Parameters["@intOrganizationForFleet"].Value = organizationForFleet;
        sqlGetTopUsers.Parameters["@blnBytes"].Value = bytes;
        sqlAdapter = new SqlDataAdapter(sqlGetTopUsers);
        sqlAdapter.Fill(dsGetTopUsers);
        return dsGetTopUsers;
    }

    public static DataSet GetWebLogins(DateTime from, DateTime to, int granularity, object organization, object userid)
    {
        SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SentinelFM"].ConnectionString);
        SqlCommand sqlGetWebLogins = new SqlCommand();
        SqlDataAdapter sqlAdapter;
        DataSet dsGetWebLogins = new DataSet();

        sqlGetWebLogins.CommandText = "sp_TV_GetWebLogins";
        sqlGetWebLogins.CommandTimeout = 300;
        sqlGetWebLogins.CommandType = CommandType.StoredProcedure;
        sqlGetWebLogins.Connection = sqlConn;
        sqlGetWebLogins.Parameters.Clear();
        sqlGetWebLogins.Parameters.Add("@dtFrom", SqlDbType.DateTime);
        sqlGetWebLogins.Parameters.Add("@dtTo", SqlDbType.DateTime);
        sqlGetWebLogins.Parameters.Add("@intGranularity", SqlDbType.Int);
        sqlGetWebLogins.Parameters.Add("@intOrganization", SqlDbType.Int);
        sqlGetWebLogins.Parameters.Add("@intUserId", SqlDbType.Int);
        sqlGetWebLogins.Parameters["@dtFrom"].Value = from;
        sqlGetWebLogins.Parameters["@dtTo"].Value = to;
        sqlGetWebLogins.Parameters["@intGranularity"].Value = granularity;
        sqlGetWebLogins.Parameters["@intOrganization"].Value = organization;
        sqlGetWebLogins.Parameters["@intUserId"].Value = userid;
        sqlAdapter = new SqlDataAdapter(sqlGetWebLogins);
        sqlAdapter.Fill(dsGetWebLogins);
        return dsGetWebLogins;
    }

    public static DataSet GetIncomingPacketStats(DateTime from, DateTime to)
    {
        SqlCommand sqlGetIncomingPacketStats = new SqlCommand("sp_TV_GetIncomingPacketStats", new SqlConnection(ConfigurationManager.ConnectionStrings["SentinelFM"].ConnectionString));
        SqlDataAdapter sqlAdapter;
        DataSet dsGetIncomingPacketStats = new DataSet();

        sqlGetIncomingPacketStats.CommandTimeout = 300;
        sqlGetIncomingPacketStats.CommandType = CommandType.StoredProcedure;
        sqlGetIncomingPacketStats.Parameters.Add("@dtFrom", SqlDbType.DateTime);
        sqlGetIncomingPacketStats.Parameters.Add("@dtTo", SqlDbType.DateTime);
        sqlGetIncomingPacketStats.Parameters["@dtFrom"].Value = from;
        sqlGetIncomingPacketStats.Parameters["@dtTo"].Value = to;
        sqlAdapter = new SqlDataAdapter(sqlGetIncomingPacketStats);
        sqlAdapter.Fill(dsGetIncomingPacketStats);
        return dsGetIncomingPacketStats;
    }
}
