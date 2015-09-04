using System;
using System.Collections.Generic;
using System.Configuration; 
using System.Data;
using System.Data.SqlClient;
using System.Linq;

/// <summary>
/// Summary description for SystemConfiguration
/// </summary>
/// 
public class SystemConfiguration
{
    #region Constants

    const string msConfiguredConnectionName = "SentinelFMConnection";

    #endregion

    #region Variables

    DataSet moDataSet = null;
    string msMessage = "";

    // Report variables
    string msReportUri = string.Empty;
    string msReportPath = string.Empty;
    string msReportType = string.Empty;

    #endregion

    #region Properties

        #region general properties

            public string ConfiguredConnectionName
            {
                get { return msConfiguredConnectionName; }
            }
            public DataSet DataSource
            {
                get { return moDataSet; }
            }
            public string Message
            {
                get { return msMessage; }
            }

        #endregion

        #region Report properties
    public string ReportUri {
                get { return msReportUri; }
            }
            public string ReportPath
            {
                get { return msReportPath; }
            }
            public string ReportType
            {
                get { return msReportType; }
            }
        #endregion

    #endregion

    public SystemConfiguration()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public bool FleetList(Int32 OrganizationID)
    {

        string query = "SELECT FleetID AS Value, FleetName AS Label FROM [dbo].[vlfFleet] WHERE OrganizationID =" + OrganizationID.ToString() + " ORDER BY FleetID;";

        try
        {

            SystemDatabase db = new SystemDatabase();

            using (SqlConnection connection = new SqlConnection(db.dbConnectionstring(msConfiguredConnectionName)))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();

                da.SelectCommand = cmd;

                connection.Open();

                da.Fill(ds);

                connection.Close();

                moDataSet = ds;
                
                msMessage = "";
            }
        }
        catch (SqlException Ex)
        {
            msMessage = Ex.Message.ToString();
        }
        catch (Exception Ex)
        {
            msMessage = Ex.Message.ToString();
        }
        finally
        {
            if (msMessage != string.Empty)
                moDataSet = null;
        }

        return (moDataSet != null)? true : false;

    }
    public bool ReportList()
    {

        string query = "SELECT 0 AS Value,  'Select a report' as Label FROM Reports UNION SELECT ReportID AS Value,  ReportName as Label FROM Reports WHERE ReportStatus = 1;";

        try
        {
            SystemDatabase db = new SystemDatabase();

            using (SqlConnection connection = new SqlConnection(db.dbConnectionstring(msConfiguredConnectionName)))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();

                da.SelectCommand = cmd;

                connection.Open();
                da.Fill(ds);
                connection.Close();

                moDataSet = ds;

                msMessage = "";
            }
        }
        catch (SqlException Ex)
        {
            msMessage = Ex.Message.ToString();
        }
        catch (Exception Ex)
        {
            msMessage = Ex.Message.ToString();
        }
        finally
        {
            if (msMessage != string.Empty)
                moDataSet = null;
        }

        return (moDataSet != null)? true : false;

    }
    public bool ReportDetail(string ReportID)
    {

        string query = "SELECT [ReportServer],[ReportPath],[ReportType] FROM Reports WHERE ReportStatus = 1 AND [ReportID] = " + ReportID + ";";

        try
        {
            SystemDatabase db = new SystemDatabase();

            using (SqlConnection connection = new SqlConnection(db.dbConnectionstring(msConfiguredConnectionName)))
            {
                SqlCommand cmd = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader oReader = cmd.ExecuteReader();
                if (oReader.Read())
                {
                    msReportUri = oReader[0].ToString();
                    msReportPath = oReader[1].ToString();
                    msReportType = oReader[2].ToString();
                    msMessage = "";
                }
                else 
                {
                    msReportUri = "";
                    msReportPath = "";
                    msReportType = "";
                    msMessage = "Report not found";
                }

                oReader.Close();
                connection.Close();
            }
        }
        catch (SqlException Ex)
        {
            msMessage = Ex.Message.ToString();
        }
        catch (Exception Ex)
        {
            msMessage = Ex.Message.ToString();
        }
        finally
        {

        }

        return (msMessage != string.Empty)? true : false;

    }
}

