using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SystemInfor
/// </summary>
public class SystemUser
{
    //const string msConfiguredConnectionName = "SentinelFM.41";

    string msConfiguredConnectionName;
    string msConnectionString;
     Int32 miOrganizationID;
    string msOrganizationName;
    string msOrganizationLogo;
    Int32 miUserID;
    string msUsername;
    string msPassword;
    string msUserTimeZone;
    bool mbIsHashPassword;

    string msMessage;

    public int OrganizationID {
        get { return miOrganizationID; }
        set { miOrganizationID = value; }
    }
    public string OrganizationName {
        get { return msOrganizationName; }
        set { msOrganizationName = value; }
    }
    public string OrganizationLogo {
        get { return msOrganizationLogo; }
    }
    public Int32 UserID {
        get { return miUserID; }
        set { miUserID = value; }
    }
    public string UserName {
        get { return msUsername; }
        set { msUsername = value; }
    }
    public string Password {
        set { msPassword = value; }
    }
    public bool IsHashPassword {
        set { mbIsHashPassword = value; }
    }
    public string UserTimeZone{
        get { return ("UTC" + ((msUserTimeZone != string.Empty) ? msUserTimeZone : UserTimeZoneHours(miUserID))); }
        set { msUserTimeZone = value; }
    }
    public string Message {
        get { return msMessage; }
    }
	public SystemUser()
	{
        SystemConfiguration osc = new SystemConfiguration();
        msConfiguredConnectionName = osc.ConfiguredConnectionName;

        SystemDatabase osd = new SystemDatabase();
        msConnectionString = osd.dbConnectionstring(msConfiguredConnectionName); 

        miOrganizationID = 0;
        msOrganizationName = "";
        miUserID = 0;
        msUserTimeZone = "UTC 0";
    }
    public bool UserInformation() {
        if ((msUsername != string.Empty) && (msPassword != string.Empty))
        {
            msMessage = "";
            return UserInfor(msUsername, msPassword, mbIsHashPassword);
        }
        else
        {
            msMessage = "Invalid User name or Password.";
            return false;
        }
    }
    public bool UserInformation(string username, string password, bool hashedpassword)
    {
        msUsername = username;
        msPassword = password;
        mbIsHashPassword = hashedpassword;

       return UserInformation();
    }

    #region Private Methods

    private bool UserInfor(string username, string password, bool Hashed)
    {
        try 
        {
            string query = "SELECT ISNULL([UserId], -1) AS [UserId], ISNULL([OrganizationID], 0) AS [OrganizationID] FROM [dbo].[vlfUser] WHERE [UserName] = '" + username + "'"; 
                
            //if (Hashed)
            //    query = query + "' AND [HashPassword] = '" + password + "';";
            //else
            //    query = query + "' AND [Password] = '" + password + "';";

            SystemDatabase db = new SystemDatabase();
            using (SqlConnection connection = new SqlConnection(db.dbConnectionstring(msConfiguredConnectionName)))
            {
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                // Call Read before accessing data. 
                if (reader.HasRows)
                {
                    reader.Read();

                    miUserID = (Int32)reader[0];
                    miOrganizationID = (Int32)reader[1];
                    msMessage = "";

                }
                else {
                    miUserID = -1;
                    miOrganizationID = -1;
                    msMessage = "User name, " + username + ", not found";
                }
                // Call Close when done reading.
                reader.Close();
                connection.Close();

                if (miUserID > 0)
                {
                    OrganizationInfor(miOrganizationID);
                    msUserTimeZone = UserTimeZoneHours(miUserID);  
                }
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
            if (msMessage != string.Empty) {
                miUserID = -1;
                miOrganizationID = -1;
            }
        }

        return (miUserID > 0)? true : false;

    }
    private bool OrganizationInfor(Int32 OrganizationID)
    {

        try
        {

            string query = "SELECT [OrganizationName], [LogoName] FROM [dbo].[vlfOrganization] WHERE [OrganizationId] = " + OrganizationID;

            SystemDatabase db = new SystemDatabase();
            using (SqlConnection connection = new SqlConnection(db.dbConnectionstring(msConfiguredConnectionName)))
            {
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                // Call Read before accessing data. 
                if (reader.HasRows)
                {
                    reader.Read();

                    msOrganizationName = reader[0].ToString();
                    msOrganizationLogo = reader[1].ToString();
                    msMessage = "";
                }
                else {
                    msOrganizationName = "";
                    msOrganizationLogo = "";
                    msMessage = "Organization not found";
                }
                // Call Close when done reading.
                reader.Close();
                connection.Close();
            }
        }
        catch (SqlException Ex)
        {
            msMessage = Ex.Message;
            msOrganizationName = "";
            msOrganizationLogo = "";
        }
        finally
        {

        }

        return (msMessage != string.Empty) ? true : false;

    }
    private bool UserOrganizationInfor(Int32 UserID) {

        try {

            string query = "SELECT ISNULL([OrganizationId], -1) AS [OrganizationId], ISNULL([OrganizationName], '') AS [OrganizationName], LogoName FROM [dbo].[vlfUser] WHERE [UserId] = " + UserID;
            
            SystemDatabase db = new SystemDatabase();
            using (SqlConnection connection = new SqlConnection(db.dbConnectionstring(msConfiguredConnectionName)))
            {
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                // Call Read before accessing data. 
                msMessage = "";
                miOrganizationID = (Int32)reader[0];
                msOrganizationName = reader[1].ToString();
                msOrganizationLogo = reader[2].ToString();

                // Call Close when done reading.
                reader.Close();
                connection.Close();
            }

        }catch (SqlException Ex){
            msMessage = Ex.Message;
            miOrganizationID = -1;
            msOrganizationName = "";
        }
        finally{
        
        }

        return (miOrganizationID > 0)? true : false;
    }
    private string UserTimeZoneHours(Int32 UserId) {

        string sTimeZone = "";
        string sQuery = "SELECT [PreferenceValue] FROM [dbo].[vlfUserPreference] WHERE [PreferenceId] = 1 AND [UserId] = " + UserId;

        try
        {
            SystemDatabase db = new SystemDatabase();
            using (SqlConnection connection = new SqlConnection(db.dbConnectionstring(msConfiguredConnectionName)))
            {
                SqlCommand command = new SqlCommand(sQuery, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                // Call Read before accessing data. 
                if (reader.HasRows)
                {
                    reader.Read();
                    sTimeZone = reader[0].ToString();
                    msMessage = "";
                }
                else
                {
                    sTimeZone = "";
                    msMessage = "";
                }
                // Call Close when done reading.
                reader.Close();
                connection.Close();
            }
        }
        catch (SqlException Ex)
        {
            msMessage = "";
        }
        finally
        {

        }
        return sTimeZone;
    }
    #endregion

    //private string ReadSingleRow(IDataRecord record, Int32 FieldIndex)
    //{
    //    return record[FieldIndex].ToString();
    //}
    //private Int32 ReadSingleRow(IDataRecord record, Int32 FieldIndex)
    //{
    //    return (Int32)record[FieldIndex];
    //}


}