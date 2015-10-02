using System;
using System.Collections.Generic;
using System.Configuration; 
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration; 

/// <summary>
/// Summary description for SystemDatabase
/// </summary>
public class SystemDatabase
{
    //Constants
    const string csSQLAuthenticationString = "server=%S%;database=%D%;uid=%U%;password=%P%;";
    const string csWNDAuthenticationString = "server=%S%;database=%D%;Integrated Security=SSPI;";
    //Variables
    string msConfiguredConnectionString= "Default";
    string msConnectionString = "";
    string msQuery = "";

    SqlConnection msConnection;

    public SystemDatabase()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public SqlConnection dbConnection() {
        return new SqlConnection(msConnectionString);
    }
    public SqlConnection dbConnection(string ConnectionString) {
        msConnectionString = ConnectionString;
        return dbConnection();
    }
    public string dbConnectionString() {
        if (msConfiguredConnectionString != string.Empty)
            return getConfiguredConnectionString();
        else
            return msConnectionString;
    }
    public string dbConnectionstring(string ConfiguredStringName) {
        msConfiguredConnectionString = ConfiguredStringName;
        return getConfiguredConnectionString(); 
    }
    public string dbConnectionstring(string Server, string Instance, bool Integrated)
    {
        msConnectionString = "server=192.168.9.41;database=SentinelFM;uid=sa;password=BSMwireless1;";
        return getConfiguredConnectionString();
    }
    public SqlDataReader dbDataReader() {

        SqlCommand command = new SqlCommand(msQuery, msConnection);
        msConnection.Open();

        return command.ExecuteReader();

    }
    public SqlDataReader dbDataReader(string ConnectionString, string Query)
    {
        return dbDataReader(dbConnection(ConnectionString), Query);
    }
    public SqlDataReader dbDataReader(SqlConnection Connection, string Query)
    {
        msQuery = Query;
        msConnection = Connection;
        return dbDataReader();
    }
    private string getConfiguredConnectionString() {
        return ConfigurationManager.ConnectionStrings[msConfiguredConnectionString].ConnectionString;
    }
}