using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
/// <summary>
/// Summary description for clsHOS
/// </summary>
public class clsHOSManager
{
    private string hosConnectionString = string.Empty;
    private string sentinelFMConnection = string.Empty;
    public static string ManualPPCID = "Manual Log";

	public clsHOSManager()
	{
        hosConnectionString =
            ConfigurationManager.ConnectionStrings["SentinelHOSConnectionString"].ConnectionString;
    }

    public void AddFormAssignment(int formId, int organizationId, int userId)
    {
        SqlConnection connection;
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_AddFormAssignment";
                sqlCmd.Connection = connection;
                SqlParameter sqlPara = new SqlParameter("@FormId", SqlDbType.Int);
                sqlPara.Value = formId;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
                sqlPara.Value = organizationId;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@UserId", SqlDbType.Int);
                sqlPara.Value = userId;
                sqlCmd.Parameters.Add(sqlPara);

                connection.Open();
                sqlCmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void AddOrUpdateFormDefinition(int formId, string formName, string definition, int userId)
    {
        SqlConnection connection;
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_AddOrUpdateFormDefinition";
                sqlCmd.Connection = connection;
                SqlParameter sqlPara = new SqlParameter("@FormId", SqlDbType.Int);
                sqlPara.Value = formId;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@FormName", SqlDbType.VarChar);
                sqlPara.Value = formName;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@Definition", SqlDbType.VarChar);
                sqlPara.Value = definition;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@UserId", SqlDbType.Int);
                sqlPara.Value = userId;
                sqlCmd.Parameters.Add(sqlPara);

                connection.Open();
                sqlCmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void DeleteFormAssignment(int formId, int organizationId, int userId)
    {
        SqlConnection connection;
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_DeleteFormAssignment";
                sqlCmd.Connection = connection;
                SqlParameter sqlPara = new SqlParameter("@FormId", SqlDbType.Int);
                sqlPara.Value = formId;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@OrganizationId", SqlDbType.VarChar);
                sqlPara.Value = organizationId;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@UserId", SqlDbType.Int);
                sqlPara.Value = userId;
                sqlCmd.Parameters.Add(sqlPara);
                connection.Open();
                sqlCmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public int DeleteFormDefinition(int formId)
    {
        int ret = 0;
        SqlConnection connection;
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_DeleteFormDefinition";
                sqlCmd.Connection = connection;
                SqlParameter sqlPara = new SqlParameter("@FormId", SqlDbType.Int);
                sqlPara.Value = formId;
                sqlCmd.Parameters.Add(sqlPara);

                SqlParameter retSqlPara = new SqlParameter("@ReturnVal", SqlDbType.Int);
                retSqlPara.Direction = ParameterDirection.ReturnValue;
                sqlCmd.Parameters.Add(retSqlPara);

                connection.Open();
                sqlCmd.ExecuteNonQuery();
                ret = (int)retSqlPara.Value;
                connection.Close();
            }
        }
        return ret;
    }

    public DataSet GetFormDefinition()
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetFormDefinition";
            adapter.SelectCommand.Connection = connection;
            adapter.Fill(dataSet);
        }
        return dataSet;
    }

    public DataTable  GetFormDefinitionByFormId(int formId)
    {
        DataTable dataSet = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetFormDefinitionByFormId";

            SqlParameter sqlPara = new SqlParameter("@FormId", SqlDbType.Int);
            sqlPara.Value = formId;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            adapter.SelectCommand.Connection = connection;
            adapter.Fill(dataSet);
        }
        return dataSet;
    }

    public DataTable  GetFormAssignment(int? formId, int? organizationId, DateTime? date,  Boolean? isActive)
    {
        DataTable dataSet = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetFormAssignment";

            SqlParameter sqlPara = new SqlParameter("@FormId", SqlDbType.Int);
            if (formId.HasValue)
               sqlPara.Value = formId;
            else sqlPara.Value = DBNull.Value;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            if (organizationId.HasValue)
               sqlPara.Value = organizationId;
            else sqlPara.Value = DBNull.Value;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@Date", SqlDbType.DateTime);
            if (date.HasValue)
               sqlPara.Value = date;
            else sqlPara.Value = new DateTime(1970,1,1);
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@IsActive", SqlDbType.Bit);
            if (isActive.HasValue)
               sqlPara.Value = isActive;
            else sqlPara.Value = DBNull.Value;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            adapter.SelectCommand.Connection = connection;
            adapter.Fill(dataSet);
        }
        return dataSet;
    }
}