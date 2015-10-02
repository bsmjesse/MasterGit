// This web service is for report function and created by Devin
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Services;
using System.Web.Script.Services;

using SentinelFM;

using Telerik.Web.UI;
using Telerik.Web.UI.GridExcelBuilder;

using VLF.CLS;
using VLF3.Domain.Class;
using VLF3.Domain.ActiveState;
using VLF3.Domain.ActiveState.Reports;
using VLF3.Services.ActiveState;


/// <summary>
/// Summary description for ReportWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
[ScriptService]    
public class ReportWebService : System.Web.Services.WebService
{
    protected SentinelFMSession sn = null;
    public static string return_no_login = "-1";
    public static string return_success = "1";
    public static string return_fake_user = "2";
    public static string return_fail = "0";
    private int MaximunPageSize = 200000;
    private string ScheduleReportSql = "SELECT distinct vlfReportTypes.GuiName, vlfUser.UserName, "
      + "vlfReportSchedules.DateFrom as DateFrom,"
      + "vlfReportSchedules.DateTo as DateTo,"
      + "vlfFleet.FleetName,"
      + "DATEADD(hour, @timezone, vlfReportSchedules.StartScheduledDate) as StartScheduledDate,"
      + "DATEADD(hour, @timezone, vlfReportSchedules.EndScheduledDate) as EndScheduledDate,"
      + "vlfReportSchedules.Status,"
      + "vlfReportSchedules.ReportID,"
      + "case when DeliveryMethod=0 then vlfReportSchedules.Email else 'Store to Disk' end DeliveryMethodType, DeliveryMethod "
      + "FROM vlfReportSchedules INNER JOIN vlfFleet on vlfReportSchedules.FleetID = vlfFleet.FleetId "
      + "INNER JOIN vlfReportTypes on vlfReportSchedules.ReportGuiID = vlfReportTypes.GuiId "
      + "INNER JOIN vlfUser ON vlfUser.userid = vlfReportSchedules.UserID  "
      + "where  "
      + " vlfReportSchedules.UserID in ({0}) "
      + " and StartScheduledDate>DATEADD(month,-1,getdate()) ";

    private string ScheduleReportSqlHierarchy = " SELECT distinct vlfReportTypes.GuiName, vlfUser.UserName, "
      + "vlfReportSchedules.DateFrom as DateFrom,"
      + "vlfReportSchedules.DateTo as DateTo,"
      + "case when vlfReportSchedules.fleetid=0 then dbo.ufn_GetListOfFleets(vlfReportSchedules.params) else isnull (vlfFleet.FleetName,'') end FleetName,"
      + "DATEADD(hour, @timezone, vlfReportSchedules.StartScheduledDate) as StartScheduledDate, "
      + "DATEADD(hour, @timezone, vlfReportSchedules.EndScheduledDate) as EndScheduledDate, "
      + "vlfReportSchedules.Status, "
      + "vlfReportSchedules.ReportID, "
      + "case when DeliveryMethod=0 then vlfReportSchedules.Email else 'Store to Disk' end DeliveryMethodType, DeliveryMethod "
      + "FROM vlfReportSchedules left outer join vlfFleet on vlfReportSchedules.FleetID = vlfFleet.FleetId " 
      + " INNER JOIN vlfReportTypes on vlfReportSchedules.ReportGuiID = vlfReportTypes.GuiId "
      + " INNER JOIN vlfUser ON vlfUser.userid = vlfReportSchedules.UserID " 
      + " and vlfReportSchedules.UserID in ({0}) "
      + " and StartScheduledDate>DATEADD(month,-1,getdate()) ";
    
    private string GroupUsersSql = "select {0} as UserID "
                                 + " union "
                                 + "select u.userid from vlfUserGroupAssignment u "
                                 + "inner join vlfUserGroup g on g.UserGroupID = u.UserGroupID "
                                 + "where u.userid in (select userid from vlfUser where organizationid = {1}) ";
                                 //+ "and g.SecurityLevel > {2} ";

    private string ScheduleReportSql_fix = "DECLARE @timezone int; SET @timezone = dbo.GetTimeZoneDayLight({0}); IF @timezone IS NULL SET @timezone = 0;";

    private string ScheduleReportFilesSql_1 = "SELECT  vlfReportFiles.RowID, "
            + "vlfReportFiles.ReportFileName, "
            + " DATEADD(hour, dbo.GetTimeZoneDayLight(UserID), vlfReportFiles.DateCreated) AS DateCreated, "
            + "vlfReportTypes.GuiName,"
            + "@TypeId as DeliveryMethod "
            + "FROM vlfReportFiles, vlfReportTypes where  vlfReportFiles.ReportGuid = vlfReportTypes.GuiId and ReportID = {0} and vlfReportFiles.UserID = {1}";

    private string ScheduleReportFilesSql_2 = "SELECT DISTINCT 1 as RowID, "
            + "ISNULL(vlfReportHistory.LinkUrl,'') as ReportFileName,"
            + "DATEADD(hour, dbo.GetTimeZoneDayLight(vlfReportSchedules.UserID),vlfReportSchedules.StatusDate) AS DateCreated, "
            + "vlfReportTypes.GuiName,"
            + "@TypeId as DeliveryMethod  "
            + "FROM   vlfReportSchedules "
            + "INNER JOIN vlfFleet ON vlfReportSchedules.FleetID = vlfFleet.FleetId "
            + "INNER JOIN vlfReportTypes ON vlfReportSchedules.ReportGuiID = vlfReportTypes.GuiId "
            + "LEFT JOIN vlfReportHistory ON vlfReportSchedules.ReportId=vlfReportHistory.ReportId "
            + "and vlfReportSchedules.DateFrom=vlfReportHistory.DateFrom "
            + "and vlfReportSchedules.DateTo=vlfReportHistory.DateTo "
            + "where vlfReportSchedules.ReportId={0} and vlfReportSchedules.UserID = {1} ";
    private string ScheduleReportFilesSql_fix = " DECLARE @TypeId int; SELECT @TypeId=DeliveryMethod from vlfReportSchedules where ReportID= {0}; ";
    private string FindScheduleReportCountSql = "Select count(*) from vlfReportSchedules where ReportID = {0} and UserID = {1}";

    private string vsScheduleConnection = string.Empty;
    private string vsRepositoryConnection = string.Empty;

    public ReportWebService()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
        vsScheduleConnection = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
    }

    /// <summary>
    /// This Web Service is for getting report repository from database
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="maximumRows"></param>
    /// <param name="sortExpressions"></param>
    /// <param name="filterExpressions"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true), Description("Gets DataSet of stock quote for a specific symbol")]
    public List<clsReportRepository> GetReportRepositoryData(int startIndex, int maximumRows,
            string sortExpressions, List<GridFilterExpression> filterExpressions)
    {

        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];
        else return null;

        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return null;

        //try
        //{
        //    VLF3.Services.ReportingService reportService = VLF3.Services.ReportingService.GetInstance();
        //    VLF3.Domain.ActiveState.Reports.ReportRepositoryReportParam reportPara = new VLF3.Domain.ActiveState.Reports.ReportRepositoryReportParam();
        //    reportPara.PageIndex = startIndex / maximumRows + 1;
        //    reportPara.PageSize = maximumRows;
        //    if (string.IsNullOrEmpty(sortExpressions))
        //    {
        //        reportPara.SortingCriteria = "ReportRepositoryId";
        //        reportPara.SortingDirection = VLF3.Domain.ActiveState.Reports.ReportRepositoryReportParam.DESC;
        //    }
        //    else reportPara.SortingCriteria = sortExpressions;

        //    GetReportRepositoryFilter(filterExpressions, reportPara);
        //    reportPara.UserId = sn.UserID;
        //    reportPara.UserIdOperator = ParamOperator.Equal;
        //    List<VLF3.Domain.ActiveState.Reports.ReportRepositoryReportData> reporData = reportService.GenerateReportRepositoryReport(reportPara).ToList();
        //    if (reporData == null) reporData = new List<ReportRepositoryReportData>();
        //    foreach (ReportRepositoryReportData repository in reporData)
        //    {
        //        if (repository.Completed.HasValue) repository.Completed = repository.Completed.Value.AddHours(sn.User.TimeZone + sn.User.DayLightSaving);
        //        repository.Requested = repository.Requested.AddHours(sn.User.TimeZone + sn.User.DayLightSaving);
        //    }
        //    return reporData;
        //}
        //catch (Exception ex)
        //{
        //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetReportRepositoryData"));
        //    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

        //    return new List<ReportRepositoryReportData>();
        //}


        List<clsReportRepository> list = new List<clsReportRepository>();
        StringBuilder sqlBuilderLast = new StringBuilder();
        StringBuilder sqlBuilder = new StringBuilder();
        List<DbParameter> parameters = new List<DbParameter>();

        Int16 categoryId = 0;

        if (sn.User.OrganizationId == 123) //CN
            categoryId = 1;
        if (sn.User.OrganizationId == 570) //Brickman
            categoryId = 4;
        else if (sn.SuperOrganizationId == 382) //Wex
            categoryId = 2;
        else if (sn.User.OrganizationId == 622) //CP Rail
            categoryId = 3;
        else if (sn.User.OrganizationId == 18) //Aecon
            categoryId = 5;
        else if (sn.User.OrganizationId == 951) //UP
            categoryId = 6;
        else if (sn.User.OrganizationId == 327) //Badger Daylighting Inc
            categoryId = 7;
        else if (sn.User.OrganizationId == 489) //Graham Construction
            categoryId = 8;
        else if (sn.User.OrganizationId == 999620) //Datum Exploration Ltd.
            categoryId = 10;
        else if (sn.User.OrganizationId == 698) //CNTL
            categoryId = 11;
        else if (sn.User.OrganizationId == 999630) //MTSAllstream
            categoryId = 12;
        else if (sn.User.OrganizationId == 999603) //E80 Plus Constructors
            categoryId = 13;
        else if (sn.User.OrganizationId == 999693) //Mr. Rooter of Ottawa
            categoryId = 14;
        else if (sn.User.OrganizationId == 480) //SFM 2000
            categoryId = 15;
        else if (sn.User.OrganizationId == 999620) //SA Exploration (Canada) Ltd (Datum)
            categoryId = 16;
        else if (sn.User.OrganizationId == 999692) //Willbros
            categoryId = 17;
        else if (sn.User.OrganizationId == 999650) //Transport SN
            categoryId = 18;
        else if (sn.User.OrganizationId == 952) //G4S
            categoryId = 19;
        else if (sn.User.OrganizationId == 999695) //VanHoute 
            categoryId = 20;
        else if (sn.User.OrganizationId == 999700) //MTO 
            categoryId = 22;
        else if (sn.User.OrganizationId == 999746) //BATO
            categoryId = 23;
        else if (sn.User.OrganizationId == 999994) //BNSF Railway
            categoryId = 24;
	   else if (sn.User.OrganizationId == 1000010) //Sperry
                categoryId = 25;
            else if (sn.User.OrganizationId == 655) //Edge Oil
                categoryId = 26;
            else if (sn.User.OrganizationId == 999988) //TDSB
                categoryId = 27;
	    else if (sn.User.OrganizationId == 1000065) //Ameco
                categoryId = 29;
        else if (sn.User.OrganizationId == 563) //Cummins Eastern Canada LP
            categoryId = 32;	
	    else if (sn.User.OrganizationId == 1000026) //Bell Canada Inc
                categoryId = 33;
	  else if (sn.User.OrganizationId == 1000051) //LIRR
                categoryId = 34;		
	  else if (sn.User.OrganizationId == 1000076) //Metro North
                categoryId = 35;	
        else if (sn.User.OrganizationId == 1000088) //City of St. John's
              categoryId = 36;
        else if (sn.User.OrganizationId == 1000096) //Bridges & Tunnels
              categoryId = 37;
        else if (sn.User.OrganizationId == 1000110) //Bell Aliant
              categoryId = 38;
        else if (sn.User.OrganizationId == 1000097) //OmniTrax
            categoryId = 39;
        else if (sn.User.OrganizationId == 1000120) //Beacon Roofing Supply Canada
            categoryId = 40;
        else if (sn.User.OrganizationId == 999722) //Superior Plus Winroc
            categoryId = 41;
        else if (sn.User.OrganizationId == 1000056) //Jean Fournier Inc
            categoryId = 42;
        else if (sn.User.OrganizationId == 342) //Strongco Inc
            categoryId = 43;
        else if (sn.User.OrganizationId == 1000142) //Railworks
            categoryId = 44;
        else if (sn.User.OrganizationId == 1000148) //DiCAN
            categoryId = 45;
        else if (sn.User.OrganizationId == 1000144) //Ville De Pointe-Claire (Securite)
            categoryId = 46;
        else if (sn.User.OrganizationId == 999646) //Ville de Vaudreuil-Dorion
            categoryId = 47;
        else if (sn.User.OrganizationId == 1000164) //Town of Georgina
            categoryId = 48;
        else if (sn.User.OrganizationId == 664) //PVS
            categoryId = 49;
        else if (sn.User.OrganizationId == 1000141) //Gazzola
            categoryId = 50;
        else if (sn.User.OrganizationId == 1000176) //Guard-X Inc.
            categoryId = 51;
        else if (sn.User.OrganizationId == 999991) //BSM Test
            categoryId = 52;

        try
        {
            //sqlBuilder.Append("(");
            //if (!String.IsNullOrEmpty(sortExpressions))
            //{
            //    string firstExpression = sortExpressions.Split(',')[0];
            //    sqlBuilder.Append(String.Format("SELECT ReportRepository.*, ReportType.GuiName, ROW_NUMBER() OVER (Order By {0} ) as RowNum FROM ReportRepository, ReportType  ", firstExpression));
            //}
            //else
            //    sqlBuilder.Append("SELECT ReportRepository.*, ReportType.GuiName, ROW_NUMBER() OVER (Order By ReportRepositoryId Desc) as RowNum FROM ReportRepository, ReportType  ");

            //int counter = 0;
            //sqlBuilder.Append(string.Format(" WHERE UserID = {0} and Requested>DATEADD(month,-1,getdate()) and ReportRepository.ReportTypeId = ReportType.GuiId and (Category={1} or Category = 0  or Category = 999) ", sn.UserID, categoryId));


            sqlBuilder.Append("(");
            if (!String.IsNullOrEmpty(sortExpressions))
            {
                string firstExpression = sortExpressions.Split(',')[0];
                //sqlBuilder.Append(String.Format("SELECT ReportRepository.*,GuiName =(SELECT TOP 1 GuiName from [SentinelFM].[dbo].[vlfReportTypes] where ReportRepository.ReportTypeId = [SentinelFM].[dbo].[vlfReportTypes].GuiId), ROW_NUMBER() OVER (Order By {0} ) as RowNum FROM ReportRepository inner join [SentinelFM].[dbo].[vlfReportTypes] on ReportRepository.ReportTypeId = [SentinelFM].[dbo].[vlfReportTypes].GuiId ", firstExpression));
                //sqlBuilder.Append(String.Format("SELECT ReportRepository.*,GuiName =(SELECT TOP 1 GuiName from ReportType where ReportRepository.ReportTypeId = ReportType.GuiId), ROW_NUMBER() OVER (Order By {0} ) as RowNum FROM ReportRepository  ", firstExpression));
                sqlBuilder.Append(String.Format("SELECT ReportRepository.*,GuiName =(SELECT TOP 1 GuiName from ReportType where ReportRepository.ReportTypeId = ReportType.GuiId), ROW_NUMBER() OVER (Order By {0} ) as RowNum FROM ReportRepository join (select ReportRepository.ReportRepositoryId, max(category) category, GuiName from ReportRepository inner join ReportType on ReportRepository.ReportTypeId = ReportType.GuiId where category in (0,999,{1}) group by ReportRepository.ReportRepositoryId, GuiName) t1 on ReportRepository.ReportRepositoryId=t1.ReportRepositoryId", firstExpression, categoryId));
            }
            else
            {
                //sqlBuilder.Append("SELECT ReportRepository.*, GuiName =(SELECT TOP 1 GuiName from [SentinelFM].[dbo].[vlfReportTypes] where ReportRepository.ReportTypeId = [SentinelFM].[dbo].[vlfReportTypes].GuiId), ROW_NUMBER() OVER (Order By ReportRepositoryId Desc) as RowNum FROM ReportRepository inner join [SentinelFM].[dbo].[vlfReportTypes] on ReportRepository.ReportTypeId = [SentinelFM].[dbo].[vlfReportTypes].GuiId ");
                //sqlBuilder.Append("SELECT ReportRepository.*, GuiName =(SELECT TOP 1 GuiName from ReportType where ReportRepository.ReportTypeId = ReportType.GuiId), ROW_NUMBER() OVER (Order By ReportRepositoryId Desc) as RowNum FROM ReportRepository inner join ReportType on ReportRepository.ReportTypeId = ReportType.GuiId ");
                sqlBuilder.Append(string.Format("SELECT ReportRepository.*, GuiName =(SELECT TOP 1 GuiName from ReportType where ReportRepository.ReportTypeId = ReportType.GuiId), ROW_NUMBER() OVER (Order By ReportRepository.ReportRepositoryId Desc) as RowNum FROM ReportRepository join (select ReportRepository.ReportRepositoryId, max(category) category, GuiName from ReportRepository inner join ReportType on ReportRepository.ReportTypeId = ReportType.GuiId where category in (0,999,{0}) group by ReportRepository.ReportRepositoryId, GuiName) t1 on ReportRepository.ReportRepositoryId=t1.ReportRepositoryId ", categoryId));
            }
 
            int counter = 0;
            sqlBuilder.Append(string.Format(" WHERE UserID = {0} and Requested>DATEADD(month,-1,getdate())  ", sn.UserID));
            //sqlBuilder.Append(string.Format(" WHERE UserID = {0} and ReportType.Category in (0, {1}) and Requested>DATEADD(month,-1,getdate())  ", sn.UserID, categoryId));
            

            StringBuilder sqlBuildertmp = new StringBuilder();
            foreach (GridFilterExpression expression in filterExpressions)
            {
                counter++;
                Pair parameter = BuildParameter(expression);
                if (parameter == null) continue;
                parameters.AddRange((List<DbParameter>)parameter.Second);

                sqlBuildertmp.AppendFormat((string)parameter.First);
                if (counter < filterExpressions.Count)
                {
                    sqlBuildertmp.AppendFormat(" And ");
                }
            }

            if (sqlBuildertmp.Length > 0)
            {
                sqlBuilder.Append(" and ");
                sqlBuilder.Append(sqlBuildertmp.ToString());
            }

            
            sqlBuilder.Append(") a");


            sqlBuilderLast.Append(string.Format("SELECT * FROM {0} Where RowNum > {1} and RowNum <= {2}", sqlBuilder.ToString(), startIndex, startIndex + maximumRows));

            if (!String.IsNullOrEmpty(sortExpressions))
            {
                sqlBuilderLast.Append(String.Format(" Order By {0}", sortExpressions));
            }
            else
                sqlBuilderLast.Append(" Order By ReportRepositoryId Desc");
        }
        catch (Exception ex)
        {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetReportRepositoryData"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

        }

        using (SqlConnection connection =
            new SqlConnection(
                ConfigurationManager.ConnectionStrings["ActiveStateConnectionString"].ConnectionString))
        {

            SqlDataReader reader = null;
            try
            {
                SqlCommand command = new SqlCommand(sqlBuilderLast.ToString(), connection);

                parameters.ForEach(
                                               delegate(DbParameter parameter) { if (parameter != null) command.Parameters.Add(parameter); });
                connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    clsReportRepository newRepository = new clsReportRepository();
                    //DataRow dr = dt.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        switch (reader.GetName(i))
                        {
                            case "GuiName":
                                if (reader[i] != DBNull.Value)
                                {
                                    newRepository.GuiName = reader.GetString(i);
                                    if (sn.User.OrganizationId == 951)
                                        newRepository.GuiName = newRepository.GuiName.Replace("Violation", "Infraction"); 

                                    //dr["ID"] = reader.GetInt64(i).ToString();
                                }
                                break;

                            case "ReportRepositoryId":
                                if (reader[i] != DBNull.Value)
                                {
                                    newRepository.ReportRepositoryId = reader.GetInt64(i);
                                    //dr["ID"] = reader.GetInt64(i).ToString();
                                }
                                break;
                            case "Path":
                                if (reader[i] != DBNull.Value)
                                {
                                    newRepository.Path = reader.GetString(i);
                                    //dr["Path"] = reader.GetString(i);
                                }
                                break;
                            case "Requested":
                                if (reader[i] != DBNull.Value)
                                {
                                    newRepository.Requested = reader.GetDateTime(i);
                                    newRepository.Requested = newRepository.Requested.AddHours(sn.User.TimeZone + sn.User.DayLightSaving);
                                    newRepository.RequestedStr = newRepository.Requested.ToString("yyyyMMddHHmmss");
                                    //dr["Requested"] = reader.GetDateTime(i).ToString("MM/dd/yyyy HH:mm:ss");

                                    //dr["Requested"] = reader.GetDateTime(i).ToString("MM/dd/yyyy HH:mm:ss");
                                }
                                break;
                            case "Completed":
                                if (reader[i] != DBNull.Value)
                                {
                                    newRepository.Completed = reader.GetDateTime(i);
                                    if (newRepository.Completed.HasValue)
                                        newRepository.Completed = newRepository.Completed.Value.AddHours(sn.User.TimeZone + sn.User.DayLightSaving);
                                    newRepository.CompletedStr = newRepository.Completed.Value.ToString("yyyyMMddHHmmss");
                                }
                                //dr["Completed"] = reader.GetDateTime(i).ToString("MM/dd/yyyy HH:mm:ss");
                                break;

                            case "KeyValues":
                                if (reader[i] != DBNull.Value)
                                    newRepository.KeyValues = reader.GetString(i);
                                //dr["Completed"] = reader.GetDateTime(i).ToString("MM/dd/yyyy HH:mm:ss");
                                break;

                            default:
                                break;
                        }
                    }
                    //dt.Rows.Add(dr);
                    list.Add(newRepository);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetReportRepositoryData"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

            }
            finally { if (reader != null) reader.Close(); }
        }

        return list;
        //return null;
    }
    /// <summary>
    /// Generate filter parameters for report repository 
    /// </summary>
    /// <param name="filterExpressions"></param>
    /// <param name="reportPara"></param>
    private void GetReportRepositoryFilter(List<GridFilterExpression> filterExpressions, VLF3.Domain.ActiveState.Reports.ReportRepositoryReportParam reportPara)
    {
        foreach (GridFilterExpression expression in filterExpressions)
        {
            Pair filtPair = BuildParameterForFilter(expression);
            if (expression.FieldName.ToLower() == "GuiName".ToLower())
            {
                reportPara.GuiNameOperator = filtPair.First.ToString();
                List<Object> paras = (List<Object>)filtPair.Second;
                if (paras.Count > 0) reportPara.GuiName = paras[0].ToString();
            }

            if (expression.FieldName.ToLower() == "KeyValues".ToLower())
            {
                reportPara.KeyValuesOperator = filtPair.First.ToString();
                List<Object> paras = (List<Object>)filtPair.Second;
                if (paras.Count > 0) reportPara.KeyValues = paras[0].ToString();
            }

            if (expression.FieldName.ToLower() == "Requested".ToLower())
            {
                reportPara.RequestedOperator = filtPair.First.ToString();
                List<Object> paras = (List<Object>)filtPair.Second;
                if (paras.Count > 0)
                {
                    if ((DateTime)paras[0] == clsAsynGenerateReport.DateTimeFilterMinDate) reportPara.Requested = null;
                    else reportPara.Requested = (DateTime)paras[0];
                }
                if (paras.Count > 1) reportPara.RequestedTo = (DateTime)paras[1];
            }

            if (expression.FieldName.ToLower() == "Completed".ToLower())
            {
                reportPara.CompletedOperator = filtPair.First.ToString();
                List<Object> paras = (List<Object>)filtPair.Second;
                if (paras.Count > 0)
                {
                    if ((DateTime)paras[0] == clsAsynGenerateReport.DateTimeFilterMinDate) reportPara.Completed = null;
                    else reportPara.Completed = (DateTime)paras[0];
                }
                if (paras.Count > 1) reportPara.CompletedTo = (DateTime)paras[1];
            }
            
        }
    }

    /// <summary>
    /// Generate filter parameters for user report
    /// </summary>
    /// <param name="filterExpressions"></param>
    /// <param name="reportPara"></param>
    private void GetUserReportFilter(List<GridFilterExpression> filterExpressions, VLF3.Domain.InfoStore.Reports.UserReportReportParam reportPara)
    {
        foreach (GridFilterExpression expression in filterExpressions)
        {
            Pair filtPair = BuildParameterForFilter(expression);
            if (expression.FieldName.ToLower() == "Name".ToLower())
            {
                reportPara.NameOperator = filtPair.First.ToString();
                List<Object> paras = (List<Object>)filtPair.Second;
                if (paras.Count > 0) reportPara.Name = paras[0].ToString();
            }

            if (expression.FieldName.ToLower() == "Description".ToLower())
            {
                reportPara.DescriptionOperator = filtPair.First.ToString();
                List<Object> paras = (List<Object>)filtPair.Second;
                if (paras.Count > 0) reportPara.Description = paras[0].ToString();
            }
        }
    }


    [WebMethod(EnableSession = true)]
    public int GetReportRepositoryCount(List<GridFilterExpression> filterExpressions)
    {
        int count = 0;

        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];
        else return count;

        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return count;


        //try
        //{
        //    VLF3.Services.ReportingService reportService = VLF3.Services.ReportingService.GetInstance();
        //    VLF3.Domain.ActiveState.Reports.ReportRepositoryReportParam reportPara = new VLF3.Domain.ActiveState.Reports.ReportRepositoryReportParam();
        //    reportPara.UserId = sn.UserID;
        //    reportPara.UserIdOperator = ParamOperator.Equal;
        //    GetReportRepositoryFilter(filterExpressions, reportPara);
        //    int sum = reportService.CountReportRepositoryReportItem(reportPara);
        //    return sum;
        //}
        //catch (Exception ex)
        //{
        //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetReportRepositoryCount"));
        //    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

        //    return 0;
        //}


        Int16 categoryId = 0;

             if (sn.User.OrganizationId == 123) //CN
                categoryId = 1;
            if (sn.User.OrganizationId == 570) //Brickman
                categoryId = 4;
            else if (sn.SuperOrganizationId == 382 && sn.User.OrganizationId != 999746) //Wex
                categoryId = 2;
            else if (sn.User.OrganizationId == 622) //CP Rail
                categoryId = 3;
            else if (sn.User.OrganizationId == 18) //Aecon
                categoryId = 5;
            else if (sn.User.OrganizationId == 951) //UP
                categoryId = 6;
            else if (sn.User.OrganizationId == 327) //Badger Daylighting Inc
                categoryId = 7;
            else if (sn.User.OrganizationId == 489) //Graham Construction
                categoryId = 8;
            else if (sn.User.OrganizationId == 999620) //Datum Exploration Ltd.
                categoryId = 10;
            else if (sn.User.OrganizationId == 698) //CNTL
                categoryId = 11;
            else if (sn.User.OrganizationId == 999630) //MTSAllstream
                categoryId = 12;
            else if (sn.User.OrganizationId == 999603) //E80 Plus Constructors
                categoryId = 13;
            else if (sn.User.OrganizationId == 999693) //Mr. Rooter of Ottawa
                categoryId = 14;
            else if (sn.User.OrganizationId == 480) //SFM 2000
                categoryId = 15;
            else if (sn.User.OrganizationId == 999620) //SA Exploration (Canada) Ltd (Datum)
                categoryId = 16;
            else if (sn.User.OrganizationId == 999692) //Willbros
                categoryId = 17;
            else if (sn.User.OrganizationId == 999650) //Transport SN
                categoryId = 18;
            else if (sn.User.OrganizationId == 952) //G4S
                categoryId = 19;
            else if (sn.User.OrganizationId == 999695) //VanHoute 
                categoryId = 20;
            else if (sn.User.OrganizationId == 999700) //MTO 
                categoryId = 22;
            else if (sn.User.OrganizationId == 999746) //BATO
                categoryId = 23;
            else if (sn.User.OrganizationId == 999994) //BNSF Railway
                categoryId = 24;
            else if (sn.User.OrganizationId == 1000010) //Sperry
                categoryId = 25;
            else if (sn.User.OrganizationId == 655) //Edge Oil
                categoryId = 26;
            else if (sn.User.OrganizationId == 999988) //TDSB
                categoryId = 27;
	    else if (sn.User.OrganizationId == 1000065) //Ameco
                categoryId = 29;
	    else if (sn.User.OrganizationId == 1000026) //Bell Canada Inc
                categoryId = 33;	
	  else if (sn.User.OrganizationId == 1000051) //LIRR
                categoryId = 34;		
	  else if (sn.User.OrganizationId == 1000076) //Metro North
                categoryId = 35;	
        else if (sn.User.OrganizationId == 1000088) //City of St. John's
              categoryId = 36;
        else if (sn.User.OrganizationId == 1000096) //Bridges & Tunnels
              categoryId = 37;
        else if (sn.User.OrganizationId == 1000110) //Bell Aliant
              categoryId = 38;	
        else if (sn.User.OrganizationId == 1000097) //OmniTrax
              categoryId = 39;
            else if (sn.User.OrganizationId == 1000120) //Beacon Roofing Supply Canada
                categoryId = 40;
            else if (sn.User.OrganizationId == 999722) //Superior Plus Winroc
                categoryId = 41;	



        SqlDataReader reader = null;
        try
        {
            using (SqlConnection connection =
                new SqlConnection(
                    ConfigurationManager.ConnectionStrings["ActiveStateConnectionString"].ConnectionString))
            {
                int counter = 0;
                List<DbParameter> parameters = new List<DbParameter>();

                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT COUNT(*) FROM ReportRepository, ReportType ");

                sqlBuilder.Append(string.Format(" WHERE UserID = {0} and Requested>DATEADD(month,-1,getdate()) and ReportRepository.ReportTypeId = ReportType.GuiId and (Category={1} or Category = 0 or Category = 999) ", sn.UserID, categoryId));

                //sqlBuilder.Append(string.Format(" WHERE UserID = {0}  and ReportRepository.ReportTypeId = ReportType.GuiId and (Category={1} or Category = 0 or Category = 999) ", sn.UserID, categoryId));

                StringBuilder sqlBuildertmp = new StringBuilder();
                foreach (GridFilterExpression expression in filterExpressions)
                {
                    counter++;
                    Pair parameter = BuildParameter(expression);
                    if (parameter == null) continue;
                    parameters.AddRange((List<DbParameter>)parameter.Second);

                    sqlBuildertmp.AppendFormat((string)parameter.First);
                    if (counter < filterExpressions.Count)
                    {
                        sqlBuildertmp.AppendFormat(" And ");
                    }
                }

                if (sqlBuildertmp.Length > 0)
                {
                    sqlBuilder.Append(" and ");
                    sqlBuilder.Append(sqlBuildertmp.ToString());
                }


                //if (filterExpressions.Count > 0)
                //    sqlBuilder.Append(" and ");

                //foreach (GridFilterExpression expression in filterExpressions)
                //{
                //    counter++;
                //    Pair parameter = BuildParameter(expression);
                //    if (parameter == null) continue;
                //    parameters.AddRange((List<DbParameter>)parameter.Second);

                //    sqlBuilder.AppendFormat((string)parameter.First);
                //    if (counter < filterExpressions.Count)
                //    {
                //        sqlBuilder.AppendFormat(" AND ");
                //    }
                //    hasFilter = true;
                //}

                //if (filterExpressions.Count > 0 && !hasFilter)
                //    sqlBuilder.Remove(sqlBuilder.Length - 1, 1);



                SqlCommand command = new SqlCommand(sqlBuilder.ToString(),
                        connection);

                parameters.ForEach(
                    delegate(DbParameter parameter) { if (parameter != null) command.Parameters.Add(parameter); });

                connection.Open();

                 reader = command.ExecuteReader();

                while (reader.Read())
                {
                    count = reader.GetInt32(0);
                }


            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetReportRepositoryCount"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

        }
        finally { if (reader != null) reader.Close(); }
        return count;
    }


    /// <summary>
    /// This Web Service is for getting user report from database
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="maximumRows"></param>
    /// <param name="sortExpressions"></param>
    /// <param name="filterExpressions"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true), Description("Gets DataSet of stock quote for a specific symbol")]
    public List<clsUserReport> GetUserReportData(int startIndex, int maximumRows,
            string sortExpressions, List<GridFilterExpression> filterExpressions)
    {

        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];
        else return null;

        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return null;

        //try
        //{
        //    VLF3.Services.ReportingService reportService = VLF3.Services.ReportingService.GetInstance();
        //    VLF3.Domain.InfoStore.Reports.UserReportReportParam reportPara = new VLF3.Domain.InfoStore.Reports.UserReportReportParam();
        //    reportPara.PageIndex = startIndex / maximumRows + 1;
        //    reportPara.PageSize = maximumRows;
        //    if (string.IsNullOrEmpty(sortExpressions))
        //    {
        //        reportPara.SortingCriteria = "UserReportId";
        //        reportPara.SortingDirection = VLF3.Domain.ActiveState.Reports.ReportRepositoryReportParam.DESC;
        //    }
        //    else reportPara.SortingCriteria = sortExpressions;

        //    GetUserReportFilter(filterExpressions, reportPara);
        //    reportPara.UserId = sn.UserID;
        //    reportPara.UserIdOperator = ParamOperator.Equal;
        //    List<VLF3.Domain.InfoStore.Reports.UserReportReportData> reporData = reportService.GenerateUserReportReport(reportPara).ToList();
        //    if (reporData == null) reporData = new List<VLF3.Domain.InfoStore.Reports.UserReportReportData>();

        //    List<clsUserReport> clsUserReportList = new List<SentinelFM.clsUserReport>();
        //    //DataRow dr = dt.NewRow();
        //    foreach (VLF3.Domain.InfoStore.Reports.UserReportReportData userReport in reporData)
        //    {
        //        clsUserReport newUserReport = new clsUserReport();
        //        newUserReport.Name = userReport.Name;
        //        newUserReport.UserReportId = userReport.UserReportId;
        //        newUserReport.Description = userReport.Description;
        //        newUserReport.From = DateTime.Now.Date.AddMinutes(userReport.Start);
        //        if (newUserReport.From.HasValue) newUserReport.To = newUserReport.From.Value.AddMinutes(userReport.Period);
        //        else newUserReport.To = DateTime.Now.Date.AddMinutes(userReport.Period);
        //        newUserReport.FormatId = userReport.FormatId;
        //        newUserReport.Category = clsAsynGenerateReport.PairFindValue(clsStandardReport.category_N, userReport.CustomProp);
        //        clsUserReportList.Add(newUserReport);
        //    }
        //    //dt.Rows.Add(dr);
        //    return clsUserReportList;
        //}
        //catch (Exception ex)
        //{
        //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetReportRepositoryData"));
        //    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

        //    return new List<SentinelFM.clsUserReport>();
        //}

        
        List<clsUserReport> list = new List<clsUserReport>();

        StringBuilder sqlBuilder = new StringBuilder();
        List<DbParameter> parameters = new List<DbParameter>();
        StringBuilder sqlBuilderLast = new StringBuilder();
        try
        {
            string sql = string.Format("(SELECT UserReportId, Name, Description, DATEADD(minute, ISNULL(start, 0), @currentDate) as [DateFrom] , DATEADD(minute, ISNULL(period, 0), DATEADD(minute, ISNULL(start, 0), @currentDate) ) as [DateTo] ,CustomProp,FormatId FROM UserReport where UserID = {0}) userReports ", sn.UserID); 

            sqlBuilder.Append("(");
            if (!String.IsNullOrEmpty(sortExpressions))
            {
                string firstExpression = sortExpressions.Split(',')[0];
                sqlBuilder.Append(String.Format("SELECT *, ROW_NUMBER() OVER (Order By {0} ) as RowNum FROM {1} ", firstExpression, sql));
            }
            else
                sqlBuilder.Append(String.Format("SELECT *, ROW_NUMBER() OVER (Order By UserReportId Desc) as RowNum FROM {0} ", sql));

            int counter = 0;

            sqlBuilder.Append(") a");
            sqlBuilderLast.Append("Declare @currentDate as datetime; set @currentDate = DATEADD(dd, DATEDIFF(dd, 0, getutcdate()), 0);");
            sqlBuilderLast.Append(string.Format("SELECT * FROM {0} Where RowNum > {1} and RowNum <= {2}", sqlBuilder.ToString(), startIndex, startIndex + maximumRows));

            if (filterExpressions.Count > 0)
                sqlBuilderLast.Append(" and ");

            foreach (GridFilterExpression expression in filterExpressions)
            {
                counter++;
                Pair parameter = BuildParameter(expression);
                parameters.AddRange((List<DbParameter>)parameter.Second);

                sqlBuilderLast.AppendFormat((string)parameter.First);
                if (counter < filterExpressions.Count)
                {
                    sqlBuilderLast.AppendFormat(" And ");
                }
            }

        }

        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetUserReportData"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
        }

        using (SqlConnection connection =
            new SqlConnection(
                ConfigurationManager.ConnectionStrings["InfoStoreConnectionString"].ConnectionString))
        {
            SqlDataReader reader = null;
            try
            {
                SqlCommand command = new SqlCommand(sqlBuilderLast.ToString(), connection);

                parameters.ForEach(
                                               delegate(DbParameter parameter) { if (parameter != null) command.Parameters.Add(parameter); });
                connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    clsUserReport newUserReport = new clsUserReport();
                    //DataRow dr = dt.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        switch (reader.GetName(i))
                        {
                            case "Name":
                                if (reader[i] != DBNull.Value)
                                {
                                    newUserReport.Name = reader.GetString(i);
                                    //dr["ID"] = reader.GetInt64(i).ToString();
                                }
                                break;

                            case "UserReportId":
                                if (reader[i] != DBNull.Value)
                                {
                                    newUserReport.UserReportId = reader.GetInt64(i);
                                    //dr["ID"] = reader.GetInt64(i).ToString();
                                }
                                break;
                            case "Description":
                                if (reader[i] != DBNull.Value)
                                {
                                    newUserReport.Description = reader.GetString(i);
                                    //dr["Path"] = reader.GetString(i);
                                }
                                break;
                            case "DateFrom":
                                if (reader[i] != DBNull.Value)
                                {
                                    newUserReport.DateFrom = reader.GetDateTime(i);
                                    newUserReport.DateFromStr = newUserReport.DateFrom.Value.ToString("yyyyMMddHHmm");
                                    //dr["Requested"] = reader.GetDateTime(i).ToString("MM/dd/yyyy HH:mm:ss");
                                }
                                break;
                            case "DateTo":
                                if (reader[i] != DBNull.Value)
                                {
                                    newUserReport.DateTo = reader.GetDateTime(i);
                                    newUserReport.DateToStr = newUserReport.DateTo.Value.ToString("yyyyMMddHHmm");
                                }
                                //dr["Completed"] = reader.GetDateTime(i).ToString("MM/dd/yyyy HH:mm:ss");
                                break;
                            case "FormatId":
                                if (reader[i] != DBNull.Value)
                                {
                                    newUserReport.FormatId = int.Parse(reader[i].ToString());
                                    //dr["Requested"] = reader.GetDateTime(i).ToString("MM/dd/yyyy HH:mm:ss");
                                }
                                break;
                            case "CustomProp":
                                if (reader[i] != DBNull.Value)
                                {
                                    newUserReport.Category = clsAsynGenerateReport.PairFindValue(clsStandardReport.category_N, reader[i].ToString());
                                    //dr["Requested"] = reader.GetDateTime(i).ToString("MM/dd/yyyy HH:mm:ss");
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    //dt.Rows.Add(dr);
                    list.Add(newUserReport);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetUserReportData"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        return list;
    }

    [WebMethod(EnableSession = true)]
    public int GetUserReportCount(List<GridFilterExpression> filterExpressions)
    {

        int count = 0;
        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return count;

        //try
        //{
        //    VLF3.Services.ReportingService reportService = VLF3.Services.ReportingService.GetInstance();
        //    VLF3.Domain.InfoStore.Reports.UserReportReportParam reportPara = new VLF3.Domain.InfoStore.Reports.UserReportReportParam();
        //    reportPara.UserId = sn.UserID;
        //    reportPara.UserIdOperator = ParamOperator.Equal;
        //    int sum = reportService.CountUserReportReportItem(reportPara);
        //    return sum;
        //}
        //catch (Exception ex)
        //{
        //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetReportRepositoryCount"));
        //    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

        //    return 0;
        //}


        try
        {
            using (SqlConnection connection =
                new SqlConnection(
                    ConfigurationManager.ConnectionStrings["InfoStoreConnectionString"].ConnectionString))
            {
                 SqlDataReader reader = null;
                try
                {

                    int counter = 0;
                    List<DbParameter> parameters = new List<DbParameter>();

                    StringBuilder sqlBuilder = new StringBuilder();
                    sqlBuilder.Append("(");
                    sqlBuilder.Append("SELECT UserReportId, Name, Description, DATEADD(minute, ISNULL(start, 0), @currentDate) as [DateFrom] , DATEADD(minute, ISNULL(period, 0), DATEADD(minute, ISNULL(start, 0), @currentDate) ) as [DateTo] ,CustomProp,FormatId FROM UserReport ");

                   sqlBuilder.Append(string.Format(" WHERE UserID = {0} ", sn.UserID));

                    sqlBuilder.Append(") a");


                    StringBuilder sqlBuilderLast = new StringBuilder();

                    sqlBuilderLast.Append("Declare @currentDate as datetime; set @currentDate = DATEADD(dd, DATEDIFF(dd, 0, getutcdate()), 0);");
                    sqlBuilderLast.Append(string.Format("SELECT count(*) FROM {0} ", sqlBuilder.ToString()));

                    if (filterExpressions.Count > 0)
                        sqlBuilderLast.Append(" where ");

                    foreach (GridFilterExpression expression in filterExpressions)
                    {
                        counter++;
                        Pair parameter = BuildParameter(expression);
                        parameters.AddRange((List<DbParameter>)parameter.Second);

                        sqlBuilderLast.AppendFormat((string)parameter.First);
                        if (counter < filterExpressions.Count)
                        {
                            sqlBuilderLast.AppendFormat(" And ");
                        }
                    }


                    SqlCommand command = new SqlCommand(sqlBuilderLast.ToString(),
                            connection);

                    parameters.ForEach(
                        delegate(DbParameter parameter) { if (parameter != null) command.Parameters.Add(parameter); });

                    connection.Open();

                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }


                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetUserReportCount"));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

                }
                finally { if (reader != null) reader.Close(); }

            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetUserReportCount"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

        }

        
        return count;
    }

    /// <summary>
    /// This Web Service is for getting ScheduleReport from database
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="maximumRows"></param>
    /// <param name="sortExpressions"></param>
    /// <param name="filterExpressions"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true), Description("Gets DataSet of stock quote for a specific symbol")]
    public List<clsScheduledReport> GetScheduleReportData(int startIndex, int maximumRows,
            string sortExpressions, List<GridFilterExpression> filterExpressions)
    {

        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];
        else return null;

        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return null;


        List<clsScheduledReport> list = new List<clsScheduledReport>();

        StringBuilder sqlBuilder = new StringBuilder();
        StringBuilder sqlBuilderFilter = new StringBuilder();
        StringBuilder sqlBuilderLast = new StringBuilder();
        StringBuilder sqlBuilderLastHiearachy = new StringBuilder();
        StringBuilder sqlBuilderFilterHiearchy = new StringBuilder();
        StringBuilder sqlBuilderHiearachy = new StringBuilder();
        List<DbParameter> parameters = new List<DbParameter>();
        try
        {
            //sqlBuilder.Append("(");
            sqlBuilderFilter.Length = 0;
            sqlBuilderFilter.Append("(");

            string ScheduleReportSql_1 = string.Format(ScheduleReportSql, sn.UserID);

            if ((sn.User.OrganizationId == 951 && (sn.User.ParentUserGroupId == 2 || sn.User.UserGroupId == 2)) || sn.User.UserGroupId == 1)
                sqlBuilderFilter.Append(string.Format(ScheduleReportSql, string.Format(GroupUsersSql, sn.UserID, sn.User.OrganizationId))); //, sn.User.UserGroupId

            else
                sqlBuilderFilter.Append(string.Format(ScheduleReportSql, sn.UserID));

            sqlBuilderFilter.Append(")");
            sqlBuilderFilter.Append(" UNION ");
            sqlBuilderFilter.Append("(");
            if ((sn.User.OrganizationId == 951 && (sn.User.ParentUserGroupId == 2 || sn.User.UserGroupId == 2)) || sn.User.UserGroupId == 1)
                sqlBuilderFilter.Append(string.Format(ScheduleReportSqlHierarchy, string.Format(GroupUsersSql, sn.UserID, sn.User.OrganizationId))); //, sn.User.UserGroupId

            else
                sqlBuilderFilter.Append(string.Format(ScheduleReportSqlHierarchy, sn.UserID));
            sqlBuilderFilter.Append(")");


            if (!String.IsNullOrEmpty(sortExpressions))
            {

                sqlBuilder.Append(string.Format("WITH T AS (Select schedule.*, ROW_NUMBER() OVER (Order By {1} ) as RowNum from ({0}) schedule ", sqlBuilderFilter.ToString(), sortExpressions));
            }
            else
                sqlBuilder.Append(string.Format("WITH T AS (Select schedule.*, ROW_NUMBER() OVER (Order By {1} ) as RowNum from ({0}) schedule ", sqlBuilderFilter.ToString(), "StartScheduledDate DESC"));


            int counter = 0;

            if (filterExpressions.Count > 0)
            {
                sqlBuilderFilter = new StringBuilder();
                sqlBuilderFilter.Append(" where ");
                foreach (GridFilterExpression expression in filterExpressions)
                {
                    counter++;
                    Pair parameter = BuildParameter(expression);
                    parameters.AddRange((List<DbParameter>)parameter.Second);
                    string filterExpression = (string)parameter.First;
                    sqlBuilderFilter.AppendFormat(filterExpression);
                    if (counter < filterExpressions.Count)
                    {
                        sqlBuilderFilter.AppendFormat(" And ");
                    }
                }
                sqlBuilder.Append(sqlBuilderFilter.ToString());
            }
            //sqlBuilder.Append(") a");


            sqlBuilderLast.Append(string.Format(ScheduleReportSql_fix, sn.UserID));
            //sqlBuilderLast.Append(string.Format("SELECT * FROM {0} Where RowNum > {1} and RowNum <= {2}", sqlBuilder.ToString(), startIndex, startIndex + maximumRows));
            sqlBuilderLast.Append(string.Format("{0} ) SELECT * FROM T Where T.RowNum > {1} and T.RowNum <= {2}", sqlBuilder.ToString(), startIndex, startIndex + maximumRows));

            

            //if (!String.IsNullOrEmpty(sortExpressions))
            //{
            //    sqlBuilderLast.Append(String.Format(" Order By {0}", sortExpressions));
            //}
            //else
            //    sqlBuilderLast.Append(" Order By ReportRepositoryId Desc");

        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetScheduleReportData"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
        }

        using (SqlConnection connection =
            new SqlConnection(
                ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString))
        {
            SqlDataReader reader = null;
            try
            {

                SqlCommand command = new SqlCommand(sqlBuilderLast.ToString(), connection);

                parameters.ForEach(
                                               delegate(DbParameter parameter) { if (parameter != null) command.Parameters.Add(parameter); });
                connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    clsScheduledReport newScheduledReport = new clsScheduledReport();
                    //DataRow dr = dt.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        switch (reader.GetName(i))
                        {
                            case "GuiName":
                                if (reader[i] != DBNull.Value)
                                {
                                    newScheduledReport.GuiName = reader.GetString(i);
                                    if (sn.User.OrganizationId == 951)
                                        newScheduledReport.GuiName = newScheduledReport.GuiName.Replace("Violation", "Infraction");
                                    //dr["ID"] = reader.GetInt64(i).ToString();
                                }
                                break;

                            case "DateFrom":
                                if (reader[i] != DBNull.Value)
                                {
                                    newScheduledReport.DateFrom = reader.GetDateTime(i);
                                    newScheduledReport.DateFromStr = newScheduledReport.DateFrom.ToString("yyyyMMddHHmm");
                                    //dr["ID"] = reader.GetInt64(i).ToString();
                                }
                                break;
                            case "DateTo":
                                if (reader[i] != DBNull.Value)
                                {
                                    newScheduledReport.DateTo = reader.GetDateTime(i);
                                    newScheduledReport.DateToStr = newScheduledReport.DateTo.ToString("yyyyMMddHHmm");
                                    //dr["Path"] = reader.GetString(i);
                                }
                                break;
                            case "FleetName":
                                if (reader[i] != DBNull.Value)
                                {
                                    newScheduledReport.FleetName = reader.GetString(i);
                                }
                                break;
                            case "StartScheduledDate":
                                if (reader[i] != DBNull.Value)
                                    newScheduledReport.StartScheduledDate = reader.GetDateTime(i);
                                newScheduledReport.StartScheduledDateStr = newScheduledReport.StartScheduledDate.ToString("yyyyMMddHHmm");
                                break;
                            case "EndScheduledDate":
                                if (reader[i] != DBNull.Value)
                                    newScheduledReport.EndScheduledDate = reader.GetDateTime(i);
                                newScheduledReport.EndScheduledDateStr = newScheduledReport.EndScheduledDate.ToString("yyyyMMddHHmm");
                                break;

                            case "Status":
                                if (reader[i] != DBNull.Value)
                                    newScheduledReport.Status = reader.GetString(i);
                                break;

                            case "ReportID":
                                if (reader[i] != DBNull.Value)
                                    newScheduledReport.ReportID = long.Parse(reader[i].ToString());
                                break;

                            case "DeliveryMethodType":
                                if (reader[i] != DBNull.Value)
                                    newScheduledReport.DeliveryMethodType = reader.GetString(i);
                                break;

                            case "DeliveryMethod":
                                if (reader[i] != DBNull.Value)
                                    newScheduledReport.DeliveryMethod = reader[i].ToString();
                                break;

                            case "UserName":
                                if (reader[i] != DBNull.Value)
                                    newScheduledReport.UserName = reader[i].ToString();
                                break;

                            default:
                                break;
                        }
                    }
                    //dt.Rows.Add(dr);
                    list.Add(newScheduledReport);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetScheduleReportData"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }
        return list;
    }

    [WebMethod(EnableSession = true)]
    public int GetScheduleReportCount(List<GridFilterExpression> filterExpressions)
    {
        int count = 0;
        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];

        try
        {
            using (SqlConnection connection =
                new SqlConnection(
                    ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString))
            {
                SqlDataReader reader = null;

                try
                {
                    int counter = 0;
                    List<DbParameter> parameters = new List<DbParameter>();

                    StringBuilder sqlBuilder = new StringBuilder();

                    StringBuilder sqlBuilderFilter = new StringBuilder();
                    string ScheduleReportSql_1 = string.Format(ScheduleReportSql, sn.UserID);

                    sqlBuilderFilter.Append(ScheduleReportSql_1);


                    sqlBuilder.Append(string.Format(ScheduleReportSql_fix, sn.UserID));

                    sqlBuilder.Append(string.Format("SELECT count(*) FROM ({0}) a ", sqlBuilderFilter.ToString()));

                    if (filterExpressions.Count > 0)
                    {
                        sqlBuilderFilter = new StringBuilder();
                        sqlBuilderFilter.Append(" where ");

                        foreach (GridFilterExpression expression in filterExpressions)
                        {
                            counter++;
                            Pair parameter = BuildParameter(expression);
                            parameters.AddRange((List<DbParameter>)parameter.Second);
                            string filterExpression = (string)parameter.First;
                            sqlBuilderFilter.AppendFormat(filterExpression);
                            if (counter < filterExpressions.Count)
                            {
                                sqlBuilderFilter.AppendFormat(" And ");
                            }
                        }
                        sqlBuilder.Append(sqlBuilderFilter.ToString());
                    }
                    SqlCommand command = new SqlCommand(sqlBuilder.ToString(),
                            connection);

                    parameters.ForEach(
                        delegate(DbParameter parameter) { if (parameter != null) command.Parameters.Add(parameter); });

                    connection.Open();

                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetScheduleReportCount"));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
        }
        catch (Exception ex)
        {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetScheduleReportCount"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

        }
        return count;
    }



    /// <summary>
    /// This Web Service is for getting ScheduleReport file from database
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="maximumRows"></param>
    /// <param name="sortExpressions"></param>
    /// <param name="filterExpressions"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true), Description("Gets DataSet of stock quote for a specific symbol")]
    public List<clsScheduledReportFile> GetScheduleReportFileData(int startIndex, int maximumRows,
            string sortExpressions, List<GridFilterExpression> filterExpressions, long reportID)
    {

        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];
        else return null;

        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return null;

        List<clsScheduledReportFile> list = new List<clsScheduledReportFile>();

        StringBuilder sqlBuilder_1 = new StringBuilder();
        StringBuilder sqlBuilder_2 = new StringBuilder();
        StringBuilder sqlBuilderFilter_1 = new StringBuilder();
        StringBuilder sqlBuilderFilter_2 = new StringBuilder();
        List<DbParameter> parameters = new List<DbParameter>();
        StringBuilder sqlBuilderLast = new StringBuilder();
        try
        {
            sqlBuilder_1.Append("(");
            sqlBuilder_2.Append("(");

            string ScheduleReportSql_1 = string.Format(ScheduleReportFilesSql_1, reportID, sn.UserID);
            string ScheduleReportSql_2 = string.Format(ScheduleReportFilesSql_2, reportID, sn.UserID);

            sqlBuilderFilter_1.Append(ScheduleReportSql_1);
            sqlBuilderFilter_2.Append(ScheduleReportSql_2);



            if (!String.IsNullOrEmpty(sortExpressions))
            {
                //string firstExpression = sortExpressions.Split(',')[0];
                sqlBuilder_1.Append(string.Format("Select schedule.*, ROW_NUMBER() OVER (Order By {1} ) as RowNum from ({0}) schedule ", sqlBuilderFilter_1.ToString(), sortExpressions));
                sqlBuilder_2.Append(string.Format("Select schedule.*, ROW_NUMBER() OVER (Order By {1} ) as RowNum from ({0}) schedule ", sqlBuilderFilter_2.ToString(), sortExpressions));
            }
            else
            {
                sqlBuilder_1.Append(string.Format("Select schedule.*, ROW_NUMBER() OVER (Order By {1} ) as RowNum from ({0}) schedule ", sqlBuilderFilter_1.ToString(), "DateCreated DESC"));
                sqlBuilder_2.Append(string.Format("Select schedule.*, ROW_NUMBER() OVER (Order By {1} ) as RowNum from ({0}) schedule ", sqlBuilderFilter_2.ToString(), "DateCreated DESC"));
            }

            int counter = 0;
            sqlBuilderFilter_1 = new StringBuilder();
            sqlBuilderFilter_2 = new StringBuilder();
            if (filterExpressions.Count > 0)
            {
                sqlBuilderFilter_1.Append(" where ");
                sqlBuilderFilter_2.Append(" where ");
                foreach (GridFilterExpression expression in filterExpressions)
                {
                    counter++;
                    Pair parameter = BuildParameter(expression);
                    parameters.AddRange((List<DbParameter>)parameter.Second);
                    string filterExpression = (string)parameter.First;
                    sqlBuilderFilter_1.AppendFormat(filterExpression);
                    sqlBuilderFilter_2.AppendFormat(filterExpression);
                    if (counter < filterExpressions.Count)
                    {
                        sqlBuilderFilter_1.AppendFormat(" And ");
                        sqlBuilderFilter_2.AppendFormat(" And ");
                    }
                }
                sqlBuilder_1.Append(sqlBuilderFilter_1.ToString());
                sqlBuilder_2.Append(sqlBuilderFilter_2.ToString());

            }

            
            sqlBuilder_1.Append(") a");
            sqlBuilder_2.Append(") a");
            sqlBuilderLast.Append(string.Format(ScheduleReportFilesSql_fix, reportID));
            sqlBuilderLast.Append(string.Format("if @TypeId=1 SELECT * FROM {2} Where RowNum > {0} and RowNum <= {1}  else SELECT * FROM {3} Where RowNum > {0} and RowNum <= {1}",
               startIndex, startIndex + maximumRows, sqlBuilder_1.ToString(), sqlBuilder_2.ToString()));

            //if (!String.IsNullOrEmpty(sortExpressions))
            //{
            //    sqlBuilderLast.Append(String.Format(" Order By {0}", sortExpressions));
            //}
            //else
            //    sqlBuilderLast.Append(" Order By ReportRepositoryId Desc");
        }
        catch (ExecutionEngineException ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetScheduleReportFileData"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

        }

        using (SqlConnection connection =
            new SqlConnection(
                ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString))
        {
            SqlDataReader reader = null;
            try
            {


                SqlCommand command = new SqlCommand(sqlBuilderLast.ToString(), connection);

                parameters.ForEach(
                                               delegate(DbParameter parameter) { if (parameter != null) command.Parameters.Add(parameter); });
                connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    clsScheduledReportFile newScheduledReportFile = new clsScheduledReportFile();
                    //DataRow dr = dt.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        switch (reader.GetName(i))
                        {
                            case "GuiName":
                                if (reader[i] != DBNull.Value)
                                {
                                    newScheduledReportFile.GuiName = reader.GetString(i);
                                    //dr["ID"] = reader.GetInt64(i).ToString();
                                }
                                break;

                            case "ReportFileName":
                                if (reader[i] != DBNull.Value)
                                {
                                    newScheduledReportFile.ReportFileName = reader.GetString(i);
                                    //dr["ID"] = reader.GetInt64(i).ToString();
                                }
                                break;
                            case "DateCreated":
                                if (reader[i] != DBNull.Value)
                                {
                                    newScheduledReportFile.DateCreated = reader.GetDateTime(i);
                                    //dr["Path"] = reader.GetString(i);
                                }
                                break;
                            case "DeliveryMethod":
                                if (reader[i] != DBNull.Value)
                                {
                                    newScheduledReportFile.DeliveryMethod = int.Parse(reader[i].ToString());
                                }
                                break;


                            case "RowID":
                                if (reader[i] != DBNull.Value)
                                    newScheduledReportFile.RowID = long.Parse(reader[i].ToString());
                                break;


                            default:
                                break;
                        }
                    }
                    //dt.Rows.Add(dr);
                    list.Add(newScheduledReportFile);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetScheduleReportFileData"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }
        return list;
    }

    [WebMethod(EnableSession = true)]
    public int GetScheduleReportFileCount(List<GridFilterExpression> filterExpressions, long reportID)
    {
        int count = 0;
        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];

        try
        {
            using (SqlConnection connection =
                new SqlConnection(
                    ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString))
            {
                SqlDataReader reader = null;

                try
                {


                    List<clsScheduledReportFile> list = new List<clsScheduledReportFile>();

                    StringBuilder sqlBuilder_1 = new StringBuilder();
                    StringBuilder sqlBuilder_2 = new StringBuilder();
                    StringBuilder sqlBuilderFilter_1 = new StringBuilder();
                    StringBuilder sqlBuilderFilter_2 = new StringBuilder();
                    List<DbParameter> parameters = new List<DbParameter>();
                    StringBuilder sqlBuilderLast = new StringBuilder();
                    sqlBuilder_1.Append("(");
                    sqlBuilder_2.Append("(");

                    string ScheduleReportSql_1 = string.Format(ScheduleReportFilesSql_1, reportID, sn.UserID);
                    string ScheduleReportSql_2 = string.Format(ScheduleReportFilesSql_2, reportID, sn.UserID);

                    sqlBuilderFilter_1.Append(ScheduleReportSql_1);
                    sqlBuilderFilter_2.Append(ScheduleReportSql_2);



                    sqlBuilder_1.Append(string.Format("Select schedule.* from ({0}) schedule ", sqlBuilderFilter_1.ToString()));
                    sqlBuilder_2.Append(string.Format("Select schedule.* from ({0}) schedule ", sqlBuilderFilter_2.ToString()));


                    int counter = 0;
                    sqlBuilderFilter_1 = new StringBuilder();
                    sqlBuilderFilter_2 = new StringBuilder();
                    if (filterExpressions.Count > 0)
                    {
                        sqlBuilderFilter_1.Append(" where ");
                        sqlBuilderFilter_2.Append(" where ");
                        foreach (GridFilterExpression expression in filterExpressions)
                        {
                            counter++;
                            Pair parameter = BuildParameter(expression);
                            parameters.AddRange((List<DbParameter>)parameter.Second);
                            string filterExpression = (string)parameter.First;
                            sqlBuilderFilter_1.AppendFormat(filterExpression);
                            sqlBuilderFilter_2.AppendFormat(filterExpression);
                            if (counter < filterExpressions.Count)
                            {
                                sqlBuilderFilter_1.AppendFormat(" And ");
                                sqlBuilderFilter_2.AppendFormat(" And ");
                            }
                        }
                        sqlBuilder_1.Append(sqlBuilderFilter_1.ToString());
                        sqlBuilder_2.Append(sqlBuilderFilter_2.ToString());

                    }


                    sqlBuilder_1.Append(") a");
                    sqlBuilder_2.Append(") a");

                    sqlBuilderLast.Append(string.Format(ScheduleReportFilesSql_fix, reportID));
                    sqlBuilderLast.Append(string.Format("if @TypeId=1 SELECT count(*) FROM {0}   else SELECT count(*) FROM {1} ", sqlBuilder_1.ToString(), sqlBuilder_2.ToString()));


                    SqlCommand command = new SqlCommand(sqlBuilderLast.ToString(), connection);

                    parameters.ForEach(
                        delegate(DbParameter parameter) { if (parameter != null) command.Parameters.Add(parameter); });

                    connection.Open();

                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetScheduleReportFileCount"));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

                }
                finally { if (reader != null) reader.Close(); }

            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetScheduleReportFileCount"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

        }
        return count;
    }
//End

    private Pair BuildParameterForFilter(GridFilterExpression expression)
    {
        string fieldName = expression.FieldName.Trim().Split(' ')[0];
        GridKnownFunction filterFunction =
            (GridKnownFunction)Enum.Parse(typeof(GridKnownFunction), expression.FilterFunction);

        object filterValue_1 = null;
        object filterValue_2 = null;
        string filterOperator = string.Empty;
        switch (filterFunction)
        {
            case GridKnownFunction.NoFilter:
                filterOperator = "";
                break;
            case GridKnownFunction.Contains:
                filterOperator = "Like";
                filterValue_1 = string.Format("%{0}%", Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName)));
                break;
            case GridKnownFunction.DoesNotContain:
                filterOperator = "NOT LIKE";
                filterValue_1 = string.Format("%{0}%", Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName)));
                break;
            case GridKnownFunction.StartsWith:
                filterOperator = "LIKE";
                filterValue_1 = string.Format("{0}%", Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName)));
                break;
            case GridKnownFunction.EndsWith:
                filterOperator = "LIKE";
                filterValue_1 = string.Format("%{0}", Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName)));
                break;
            case GridKnownFunction.EqualTo:
                if (expression.DataTypeName == "System.DateTime")
                {
                    filterOperator = ParamOperator.Between;
                    filterValue_1 = ((DateTime)Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName)));
                    filterValue_2 = ((DateTime)Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName))).AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                else
                {
                    filterOperator = ParamOperator.Equal;
                    filterValue_1 = Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName));
                }
                break;
            case GridKnownFunction.NotEqualTo:
                filterOperator = ParamOperator.NotEqual;
                filterValue_1 = Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName));
                break;
            case GridKnownFunction.GreaterThan:
                if (expression.DataTypeName == "System.DateTime")
                {
                    filterOperator = ParamOperator.Greater;
                    filterValue_1 = ((DateTime)Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName))).AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                else
                {
                    filterOperator = ParamOperator.Greater;
                    filterValue_1 = Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName));
                }
                break;
            case GridKnownFunction.LessThan:
                filterOperator = ParamOperator.Less;
                filterValue_1 = Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName));
                break;
            case GridKnownFunction.GreaterThanOrEqualTo:
                filterOperator = ParamOperator.GreaterEqual;
                filterValue_1 = Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName));
                break;
            case GridKnownFunction.LessThanOrEqualTo:
                if (expression.DataTypeName == "System.DateTime")
                {
                    filterOperator = ParamOperator.LessEqual;
                    filterValue_1 = ((DateTime)Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName))).AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                else
                {
                    filterOperator = ParamOperator.LessEqual;
                    filterValue_1 = Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName));
                }
                break;
            case GridKnownFunction.Between:
                string[] values = expression.FieldValue.Split('#');
                filterOperator = ParamOperator.Between;
                if (expression.FieldValue.EndsWith(clsAsynGenerateReport.DateTimeFilterFlage))
                {
                    if (values[0] == clsAsynGenerateReport.DateTimeFilterMin && values[1] == clsAsynGenerateReport.DateTimeFilterMax) { filterOperator = string.Empty; break; };

                    filterValue_1 = new DateTime(int.Parse(values[0].Substring(0, 4)), int.Parse(values[0].Substring(4, 2)), int.Parse(values[0].Substring(6, 2)));
                    filterValue_2 = new DateTime(int.Parse(values[1].Substring(0, 4)), int.Parse(values[1].Substring(4, 2)), int.Parse(values[1].Substring(6, 2))).AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                break;
            case GridKnownFunction.NotBetween:
                string[] values_no = expression.FieldValue.Split('#');
                filterOperator = " Not in ";
                if (expression.FieldValue.EndsWith(clsAsynGenerateReport.DateTimeFilterFlage))
                {
                    if (values_no[0] == clsAsynGenerateReport.DateTimeFilterMin && values_no[1] == clsAsynGenerateReport.DateTimeFilterMax) { filterOperator = string.Empty; break; };

                    filterValue_1 = new DateTime(int.Parse(values_no[0].Substring(0, 4)), int.Parse(values_no[0].Substring(4, 2)), int.Parse(values_no[0].Substring(6, 2)));
                    filterValue_2 = new DateTime(int.Parse(values_no[1].Substring(0, 4)), int.Parse(values_no[1].Substring(4, 2)), int.Parse(values_no[1].Substring(6, 2))).AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                break;
            case GridKnownFunction.IsEmpty:
                filterOperator = ParamOperator.Equal;
                filterValue_1 = string.Empty;
                break;
            case GridKnownFunction.NotIsEmpty:
                filterOperator = ParamOperator.NotEqual;
                filterValue_1 = string.Empty;
                break;
            case GridKnownFunction.IsNull:
                filterOperator = ParamOperator.Is;
                filterValue_1 = null;
                break;
            case GridKnownFunction.NotIsNull:
                filterOperator = ParamOperator.IsNot;
                filterValue_1 = null;
                break;
        }

        List<object> Parameters = new List<object>();
        if (Session["SentinelFMSession"] != null && sn.User != null && !String.IsNullOrEmpty(sn.UserName))
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (filterValue_1 != null)
            {
                if (filterValue_1 is DateTime)
                {
                    filterValue_1 = ((DateTime)filterValue_1).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);
                }
            }
            if (filterValue_2 != null)
            {
                if (filterValue_2 is DateTime)
                {
                    filterValue_2 = ((DateTime)filterValue_2).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);
                }
            }
        }
        if (filterValue_1 != null) Parameters.Add(filterValue_1);
        if (filterValue_2 != null) Parameters.Add(filterValue_2);
        return new Pair(filterOperator, Parameters);
    }

    private static Pair BuildParameter(GridFilterExpression expression)
    {
        string fieldName = expression.FieldName.Trim().Split(' ')[0];
        GridKnownFunction filterFunction =
            (GridKnownFunction)Enum.Parse(typeof(GridKnownFunction), expression.FilterFunction);

        SqlParameter sqlParameter = null;
        SqlParameter sqlParameter1 = null;
        string filterExpression = string.Empty;
        switch (filterFunction)
        {
            case GridKnownFunction.NoFilter:
                filterExpression = "1 = 1";
                break;
            case GridKnownFunction.Contains:
                filterExpression = string.Format("[{0}] LIKE @{0}", fieldName);
                sqlParameter =
                    new SqlParameter(string.Format("@{0}", fieldName),
                                     string.Format("%{0}%", Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName))));
                break;
            case GridKnownFunction.DoesNotContain:
                filterExpression = string.Format("[{0}] NOT LIKE @{0}", fieldName);
                sqlParameter =
                    new SqlParameter(string.Format("@{0}", fieldName),
                                     string.Format("%{0}%", Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName))));
                break;
            case GridKnownFunction.StartsWith:
                filterExpression = string.Format("[{0}] LIKE @{0}", fieldName);
                sqlParameter =
                    new SqlParameter(string.Format("@{0}", fieldName),
                                     string.Format("{0}%", Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName))));
                break;
            case GridKnownFunction.EndsWith:
                filterExpression = string.Format("[{0}] LIKE @{0}", fieldName);
                sqlParameter =
                    new SqlParameter(string.Format("@{0}", fieldName),
                                     string.Format("%{0}", Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName))));
                break;
            case GridKnownFunction.EqualTo:
                if (expression.DataTypeName == "System.DateTime")
                {
                    filterExpression = string.Format("  CONVERT(VarCHAR(8), [{0}], 112) =   CONVERT(VarCHAR(8), @{0}, 112) ", fieldName);
                    sqlParameter =
                        new SqlParameter(string.Format("@{0}", fieldName),
                                         Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName)));
                }
                else
                {
                    filterExpression = string.Format("[{0}] = @{0}", fieldName);
                    sqlParameter =
                        new SqlParameter(string.Format("@{0}", fieldName),
                                         Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName)));
                }
                break;
            case GridKnownFunction.NotEqualTo:
                filterExpression = string.Format("[{0}] <> @{0}", fieldName);
                sqlParameter =
                    new SqlParameter(string.Format("@{0}", fieldName),
                                     Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName)));
                break;
            case GridKnownFunction.GreaterThan:
                if (expression.DataTypeName == "System.DateTime")
                {
                    filterExpression = string.Format(" CONVERT(VarCHAR(8), [{0}], 112) >CONVERT(VarCHAR(8), @{0}, 112)", fieldName);
                    sqlParameter =
                        new SqlParameter(string.Format("@{0}", fieldName),
                                        Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName)));
                }
                else
                {
                    filterExpression = string.Format("[{0}] > @{0}", fieldName);
                    sqlParameter =
                        new SqlParameter(string.Format("@{0}", fieldName),
                                        Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName)));
                }
                break;
            case GridKnownFunction.LessThan:
                filterExpression = string.Format("[{0}] < @{0}", fieldName);
                sqlParameter =
                    new SqlParameter(string.Format("@{0}", fieldName),
                                     Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName)));
                break;
            case GridKnownFunction.GreaterThanOrEqualTo:
                    filterExpression = string.Format("[{0}] >= @{0}", fieldName);
                    sqlParameter =
                        new SqlParameter(string.Format("@{0}", fieldName),
                                        Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName)));
                break;
            case GridKnownFunction.LessThanOrEqualTo:
                if (expression.DataTypeName == "System.DateTime")
                {
                    filterExpression = string.Format(" CONVERT(VarCHAR(8), [{0}], 112) <= CONVERT(VarCHAR(8), @{0}, 112)", fieldName);
                    sqlParameter =
                        new SqlParameter(string.Format("@{0}", fieldName),
                                         Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName)));
                }
                else
                {

                    filterExpression = string.Format("[{0}] <= @{0}", fieldName);
                    sqlParameter =
                        new SqlParameter(string.Format("@{0}", fieldName),
                                         Convert.ChangeType(expression.FieldValue, Type.GetType(expression.DataTypeName)));
                }
                break;
            case GridKnownFunction.Between:
                if (expression.FieldValue.EndsWith(clsAsynGenerateReport.DateTimeFilterFlage))
                {
                    string[] values = expression.FieldValue.Split('#');
                    if (values[0] == clsAsynGenerateReport.DateTimeFilterMin && values[1] == clsAsynGenerateReport.DateTimeFilterMax) break;
                    
                    if (values[0] == clsAsynGenerateReport.DateTimeFilterMin)
                        filterExpression = string.Format(" ([{0}] is null or ( CONVERT(VarCHAR(8), [{0}], 112) <= @{2}) )", fieldName, fieldName + "_min", fieldName + "_max");
                    else
                        filterExpression = string.Format(" ( CONVERT(VarCHAR(8), [{0}], 112) >= @{1}) AND ( CONVERT(VarCHAR(8), [{0}], 112) <= @{2})", fieldName, fieldName + "_min", fieldName + "_max");
                    sqlParameter = new SqlParameter(string.Format("@{0}", fieldName + "_min"), values[0]);

                    sqlParameter1 = new SqlParameter(string.Format("@{0}", fieldName + "_max"), values[1]);

                }
                break;
            case GridKnownFunction.IsEmpty:
                filterExpression = string.Format(" [{0}] = @{0}", fieldName);
                sqlParameter =
                    new SqlParameter(string.Format("@{0}", fieldName),
                                     string.Empty);
                break;
            case GridKnownFunction.NotIsEmpty:
                filterExpression = string.Format(" [{0}] <> @{0}", fieldName);
                sqlParameter =
                    new SqlParameter(string.Format("@{0}", fieldName),
                                      string.Empty);
                break;
            case GridKnownFunction.IsNull:
                filterExpression = string.Format(" [{0}] = @{0}", fieldName);
                sqlParameter =
                    new SqlParameter(string.Format("@{0}", fieldName),
                                     DBNull.Value);
                break;
            case GridKnownFunction.NotIsNull:
                filterExpression = string.Format(" [{0}] <> @{0}", fieldName);
                sqlParameter =
                    new SqlParameter(string.Format("@{0}", fieldName),
                                     DBNull.Value);
                break;
        }

        if (sqlParameter == null && sqlParameter1 == null) return null;
        List<DbParameter> sqlParameters = new List<DbParameter>();
        if (sqlParameter != null) sqlParameters.Add(sqlParameter);
        if (sqlParameter1 != null) sqlParameters.Add(sqlParameter1);
        return new Pair(filterExpression,
                        sqlParameters);
    }

    //Return Value:  -1 means no login, 1 means call successfully, 0 means call fail, 2 means fake user
    [WebMethod(EnableSession = true)]
    public string DeleteReportRepositoryByID(long ReportRepositoryID, string PageName)
    {
        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];
        else return return_no_login;
        
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return return_no_login;
        try
        {
            ///VLF3.Services.ActiveState.ReportRepositoryService rrs = VLF3.Services.ActiveState.ReportRepositoryService.GetInstance(sn.UserID);

            //Will be removed by devin begin in the futrue
            //VLF3.Domain.ActiveState.ReportRepository reportRepository = rrs.GetReportRepositoryById(ReportRepositoryID);
            //if (reportRepository != null) if (reportRepository.UserId != sn.UserID) return return_fake_user;
            //end
            clsAsynGenerateReport cAsyn = new clsAsynGenerateReport();
            DataSet ds = cAsyn.GetReportRepositoryByReportRepositoryIdandUserId(ReportRepositoryID, sn.UserID);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {

                cAsyn.DeleteReportRepository(ReportRepositoryID);
            }
            else return return_fake_user;
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            return return_fail;
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + PageName));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            return return_fail;
        }

        return return_success;
    }


       //Return Value:  -1 means no login, 1 means call successfully, 0 means call fail
    [WebMethod(EnableSession = true)]
    public string DeleteUserReportByID(long UserReportID, string PageName)
    {
        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];
        else return return_no_login;
        
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return return_no_login;
        try
        {
            //VLF3.Services.InfoStore.UserService uss = VLF3.Services.InfoStore.UserService.GetInstance(sn.UserID);
            clsAsynGenerateReport uss = new clsAsynGenerateReport();
            //Will be removed by devin begin in the futrue
            VLF3.Domain.InfoStore.UserReport userReport = uss.GetUserReportById(UserReportID);
            if (userReport != null)
            {
                //Temporary comment
                //if (userReport.User == null) return return_fake_user;
                //else
                //{
                //    //for testing by devin
                //    string myUserID = userReport.User.ToString();
                //    if (userReport.User.UserId != sn.UserID ) return return_fake_user;
                //}
            }
            //end

            uss.DeleteUserReport(UserReportID);
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            return return_fail;
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + PageName));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            return return_fail;
        }

        return return_success;
    }

    [WebMethod(EnableSession = true)]
    public string DeleteScheduledReportByID(int ReportId, string PageName)
    {
        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];
        else return return_no_login;
        
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return return_no_login;

        if (!FindScheduleReportCount(ReportId, sn.UserID)) return return_success;

        try
        {
            SentinelFM.ServerReports.Reports reportProxy = new SentinelFM.ServerReports.Reports();

           //using (SentinelFM.ServerReport.Reports reportProxy = new SentinelFM.ServerReport.Reports())
           {
                clsUtility objUtil = new clsUtility(sn);
                if (objUtil.ErrCheck(reportProxy.DeleteScheduledReportByReportID(sn.UserID, sn.SecId, ReportId), false))
                    if (objUtil.ErrCheck(reportProxy.DeleteScheduledReportByReportID(sn.UserID, sn.SecId, ReportId), true))
                {
                    return return_fail;
                }
            }

            //confirm = "";
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message + " User:" + sn.UserID.ToString() + "RowID: " + ReportId.ToString() + " Form:" + PageName));
            return return_fail;
        }
        return return_success;
    }

    [WebMethod(EnableSession = true)]
    public string DeleteScheduledReportFileByID(int ReportId, int RowId, string PageName)
    {
        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];
        else return return_no_login;

        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return return_no_login;

        if (!FindScheduleReportCount(ReportId, sn.UserID)) return return_success;

        try
        {
            SentinelFM.ServerReports.Reports reportProxy = new SentinelFM.ServerReports.Reports();

            //using (SentinelFM.ServerReport.Reports reportProxy = new SentinelFM.ServerReport.Reports())
            {
                clsUtility objUtil = new clsUtility(sn);
                if (objUtil.ErrCheck(reportProxy.DeleteReportFileByRowID(sn.UserID, sn.SecId, RowId), false))
                    if (objUtil.ErrCheck(reportProxy.DeleteReportFileByRowID(sn.UserID, sn.SecId, RowId), true))
                    {
                        return return_fail;
                    }
            }

            //confirm = "";
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message + " User:" + sn.UserID.ToString() + "RowID: " + RowId.ToString() + " Form:" + PageName));
            return return_fail;
        }
        return return_success;
    }


    private Boolean FindScheduleReportCount(int ReportId, int userID)
    {
        int count = 0;
        try
        {
            using (SqlConnection connection =
                new SqlConnection(
                    ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString))
            {
                SqlDataReader reader = null;

                try
                {

                    string sql = string.Format(FindScheduleReportCountSql, ReportId, userID);
                    SqlCommand command = new SqlCommand(sql, connection);

                    connection.Open();

                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + userID.ToString() + "Web method: FindScheduleReportCount"));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                    return false;
                }
                finally { if (reader != null) reader.Close(); }

            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + userID.ToString() + "Web method: FindScheduleReportCount"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return false;
        }
        if (count > 0) return true;
        else return false;
    }

    //Return Value:  -1 means no login, 1 means call successfully, 0 means call fail
    [WebMethod(EnableSession = true)]
    public string ExcuteUserReport(long UserReportID, string PageName)
    {
        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];
        else return return_no_login;

        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return return_no_login;
        try
        {
            //VLF3.Services.InfoStore.UserService uss = VLF3.Services.InfoStore.UserService.GetInstance(sn.UserID);
            clsAsynGenerateReport uss = new clsAsynGenerateReport();
            VLF3.Domain.InfoStore.UserReport userReport = uss.GetUserReportById(UserReportID);


            //Will be removed by devin begin in the futrue
            if (userReport != null)
            {
                ////Temporary comment
                //if (userReport.User == null) return return_fake_user;
                //else
                //{
                //    //for testing by devin

                //    string myUserID = userReport.User.ToString();
                //    if (userReport.User.UserId != sn.UserID) return return_fake_user;
                //}
            }
            //end


            if (!string.IsNullOrEmpty(userReport.CustomProp))
            {
                string customProp = userReport.CustomProp.Trim();

                string category = clsAsynGenerateReport.PairFindValue(clsStandardReport.category_N, customProp);
                List<int> fleetIds = null;
                string keyValue = string.Empty;
                if (category == "0")
                {
                    //Standard Report 
                    clsStandardReport standardReport = new clsStandardReport();
                    standardReport.GetCustomProperty(customProp);
                    standardReport.sn = sn;
                    if (standardReport.cboReports == "10")
                        standardReport.CreateReportParams_MaintenanceReport();
                    else
                    {
                        DateTime from = DateTime.Now.ToUniversalTime().Date.AddMinutes(userReport.Start);
                        DateTime to = from.AddMinutes(userReport.Period);
                        standardReport.txtFrom = from.Date;
                        standardReport.cboHoursFrom = from;
                        standardReport.txtTo = to.Date;
                        standardReport.cboHoursTo = to;
                        standardReport.CreateReportParams(null);
                    }
                    keyValue = standardReport.keyValue;
                }
                else
                {
                    //Extended Report 
                    clsExtendedReport extendedReport = new clsExtendedReport();
                    extendedReport.GetCustomProperty(customProp);
                    extendedReport.sn = sn;
                    DateTime from = DateTime.Now.ToUniversalTime().Date.AddMinutes(userReport.Start);
                    DateTime to = from.AddMinutes(userReport.Period);
                    extendedReport.txtFrom = from.Date;
                    extendedReport.txtTo = to.Date;
                    extendedReport.CreateReportParams();
                    if (!string.IsNullOrEmpty(extendedReport.selectedFleets))
                    {

                        string[] fleets = extendedReport.selectedFleets.Split(',');
                        foreach (string fleet in fleets)
                        {
                            if (!string.IsNullOrEmpty(fleet))
                            {
                                string fleetStr = fleet.Trim();
                                int fleetInt = 0;
                                if (int.TryParse(fleetStr, out fleetInt))
                                {
                                    if (fleetIds == null) fleetIds = new List<int>();
                                    fleetIds.Add(fleetInt);
                                }
                            }
                        }
                    }
                    keyValue = extendedReport.keyValue;
                }
                clsAsynGenerateReport genReport = new clsAsynGenerateReport();
                if (fleetIds != null) genReport.fleetIdsList = fleetIds;
                if (!genReport.CallReportService(sn, null, null, keyValue))
                {
                    throw new Exception("Generate User Report Error." + " User Report ID:" + UserReportID);
                }

            }
            else
            {
                throw new Exception("Custom property is empty.");
            }
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            return return_fail;
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + PageName));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            return return_fail;
        }

        return return_success;
    }

    /// <summary>
    /// This Web Service is for getting report repository from database for report message page
    /// </summary>
    /// <returns></returns>
    [WebMethod(EnableSession = true), Description("Gets DataSet of stock quote for a specific symbol")]
    public List<clsReportMessage> GetReportRepositoryDataForMessage()
    {

        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];
        else return null;

        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return null;

        List<clsReportMessage> reporData = new List<clsReportMessage>();
        using (SqlConnection connection =
            new SqlConnection(
                ConfigurationManager.ConnectionStrings["ActiveStateConnectionString"].ConnectionString))
        {
            SqlDataReader reader = null;
            try
            {
                string sql = string.Format("Select ReportRepositoryId as MessageID, GuiName as MessageName, {0} as MessageType, Completed as MessageDateTime, path as MessagePath from ReportRepository, ReportType where ReportRepository.ReportTypeId = ReportType.ReportTypeId and completed is not null and  Path is not null and path <> '' and isRead = 0 and UserId = {1} order by MessageDateTime Desc", clsReportMessage.Report_Type, sn.UserID);
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    clsReportMessage reportMessage = new clsReportMessage();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        switch (reader.GetName(i))
                        {
                            case "MessageName":
                                if (reader[i] != DBNull.Value)
                                {
                                    reportMessage.MessageName = reader.GetString(i);
                                }
                                break;

                            case "MessageID":
                                if (reader[i] != DBNull.Value)
                                {
                                    reportMessage.MessageID = reader[i].ToString();
                                }
                                break;
                            case "MessageType":
                                if (reader[i] != DBNull.Value)
                                {
                                    reportMessage.MessageType = reader[i].ToString();
                                }
                                break;
                            case "MessageDateTime":
                                if (reader[i] != DBNull.Value)
                                {
                                    reportMessage.MessageDateTime = reader.GetDateTime(i);
                                    reportMessage.MessageDateTime = reportMessage.MessageDateTime.AddHours(sn.User.TimeZone + sn.User.DayLightSaving);
                                }
                                break;
                            case "MessagePath":
                                if (reader[i] != DBNull.Value)
                                {
                                    reportMessage.MessagePath = reader.GetString(i);
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    reporData.Add(reportMessage);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetReportRepositoryDataForMessage"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }
        return reporData;
    }

    /// <summary>
    /// This Web Service is for getting report repository count from database for report message page
    /// </summary>
    /// <returns></returns>
    [WebMethod(EnableSession = true), Description("Gets DataSet of stock quote for a specific symbol")]
    public int GetReportRepositoryCountForMessage()
    {

        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];
        else return 0;

        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return 0;

        int count = 0;
        List<clsReportMessage> reporData = new List<clsReportMessage>();
        using (SqlConnection connection =
            new SqlConnection(
                ConfigurationManager.ConnectionStrings["ActiveStateConnectionString"].ConnectionString))
        {
            SqlDataReader reader = null;
            try
            {
                string sql = string.Format("Select count(ReportRepositoryId) from ReportRepository, ReportType where ReportRepository.ReportTypeId = ReportType.ReportTypeId and completed is not null and  Path is not null and path <> ''  and isRead = 0 and UserId = {1} ", clsReportMessage.Report_Type, sn.UserID);
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    count = reader.GetInt32(0);
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetReportRepositoryCountForMessage"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }
        return count;
    }   

    /// <summary>
    /// This Web Service is for updating read status by reportRepositoryid for report message page
    /// </summary>
    /// <returns></returns>
    [WebMethod(EnableSession = true), Description("Gets DataSet of stock quote for a specific symbol")]
    public string UpdateReportRepositoryReadStatusByID(long reportRepositoryID)
    {
        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];
        else return return_no_login;

        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return return_no_login;

        //VLF3.Services.ActiveState.ReportRepositoryService rrs = VLF3.Services.ActiveState.ReportRepositoryService.GetInstance(sn.UserID);
        clsAsynGenerateReport rrs = new clsAsynGenerateReport();
        try
        {
            VLF3.Domain.ActiveState.ReportRepository reportRepository = rrs.GetReportRepositoryById(reportRepositoryID);
            bool isRightUser = false;
            if (reportRepository != null)
            {
                if (reportRepository.UserId == sn.UserID) { isRightUser = true; }
            }

            if (!isRightUser) return return_fake_user;

            rrs.UpdateReadStatus(reportRepositoryID, true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: UpdateReportRepositoryReadStatusByID"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return return_fail;
        }
        
        return return_success;
    }

    /// <summary>
    /// This Web Service is for updating read status by reportRepositoryid string for report message page
    /// </summary>
    /// <returns></returns>
    [WebMethod(EnableSession = true), Description("Gets DataSet of stock quote for a specific symbol")]
    public string UpdateReportRepositoryReadStatusByIDString(string reportRepositoryIDs, string messageIDs)
    {
        if (Session["SentinelFMSession"] != null) sn = (SentinelFMSession)Session["SentinelFMSession"];
        else return return_no_login;

        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return return_no_login;


        //VLF3.Services.ActiveState.ReportRepositoryService rrs = VLF3.Services.ActiveState.ReportRepositoryService.GetInstance(sn.UserID);

        using (SqlConnection connection =
            new SqlConnection(
                ConfigurationManager.ConnectionStrings["ActiveStateConnectionString"].ConnectionString))
        {
            try
            {

                string sql = string.Format("update ReportRepository set isread = 1 where UserId = {0} and ReportRepositoryId IN ({1})", sn.UserID, reportRepositoryIDs);
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: UpdateReportRepositoryReadStatusByIDString"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return return_fail;
            }
        }
        return return_success;
    }

    /// <summary>
    /// This Web Service is for getting report repository count from database for report message page
    /// </summary>
    /// <returns></returns>
    [WebMethod(EnableSession = true), Description("Gets DataSet of stock quote for a specific symbol")]
    public bool ReportScheduleRecordUpload(int ScheduleID, out string ScheduleInfo, out string Message)
    {
        Message = "";
        ScheduleInfo = "";

        try
        {
            using (SqlConnection connection = new SqlConnection(vsScheduleConnection))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "usp_ReportScheduleDetail";
                    command.Connection = connection;

                    command.Parameters.Add(new SqlParameter("@ScheduleID", ScheduleID));

                    connection.Open();

                    SqlDataReader dr = command.ExecuteReader();

                    if (dr.Read())
                    {
                        StringBuilder sb = new StringBuilder();

                        for (int i = 0; i < dr.FieldCount; i++) 
                        {
                            sb.Append("<" + dr.GetName(i) + ">" + dr.GetValue(i).ToString() + "</" + dr.GetName(i) + ">");
                        }

                        ScheduleInfo = "<Schedule>" + sb.ToString() + "</Schedule>";
                    }

                    connection.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetReportRepositoryCountForMessage"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
        }
        finally
        {
            if (string.IsNullOrEmpty(ScheduleInfo) && string.IsNullOrEmpty(Message))
                Message = "Report Schedule[" + ScheduleID.ToString() + "] not Found.";
        }

        return string.IsNullOrEmpty(Message);

    }
    /// <summary>
    /// This Web Service is for getting report repository count from database for report message page
    /// </summary>
    /// <returns></returns>
    [WebMethod(EnableSession = true), Description("Gets DataSet of stock quote for a specific symbol")]
    public bool ReportScheduleRecordUpDate(int UserID, int ScheduleID, string Parameters, out string Message)
    {
        Message = "";

        if (string.IsNullOrEmpty(Parameters))
        {
            Message = "Parameters is null or empty";
            return false;
        }
        else
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            
            try
            {
                DataSet dsParameters = xmlStringToDataset(Parameters);

                if (dsParameters.Tables.Count > 0)
                {
                    DataRow dr = dsParameters.Tables[0].Rows[0];

                    using (SqlConnection connection = new SqlConnection(vsScheduleConnection))
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "usp_ScheduledReportUpdate";
                            command.Connection = connection;

                            command.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = dr["ScheduleID"].ToString();       // xmlDoc.CreateNode("ScheduleID", ScheduleID);
                            command.Parameters.Add("@UserID", SqlDbType.Int).Value = sn.UserID;
                            command.Parameters.Add("@DateFrom", SqlDbType.DateTime).Value = dr["FromDate"].ToString();      // xmlDoc.CreateNode("FromDate", Convert.ToDateTime(sn.Report.FromDate, new CultureInfo("en-US")));
                            command.Parameters.Add("@DateTo", SqlDbType.DateTime).Value = dr["ToDate"];                     // xmlDoc.CreateNode("ToDate", Convert.ToDateTime(sn.Report.ToDate, new CultureInfo("en-US")));
                            command.Parameters.Add("@IsFleet", SqlDbType.Bit).Value = dr["IsFleet"];                        // xmlDoc.CreateNode("IsFleet", sn.Report.IsFleet);
                            command.Parameters.Add("@FleetID", SqlDbType.Int).Value = dr["FleetId"];                        // xmlDoc.CreateNode("FleetId", sn.Report.FleetId);
                            command.Parameters.Add("@Params", SqlDbType.VarChar, 1024).Value = dr["XmlParams"];             //xmlDoc.CreateNode("XmlParams", sn.Report.XmlParams);
                            command.Parameters.Add("@Email", SqlDbType.VarChar, 255).Value = dr["Email"];                   // xmlDoc.CreateNode("Email", this.txtEmail.Text);
                            command.Parameters.Add("@LinkUrl", SqlDbType.VarChar, 128).Value = "";
                            command.Parameters.Add("@ReportGuiID", SqlDbType.Int).Value = dr["GuiId"];                      // xmlDoc.CreateNode("GuiId", sn.Report.GuiId);
                            command.Parameters.Add("@Status", SqlDbType.VarChar, 16).Value = dr["Status"];                  // xmlDoc.CreateNode("Status", "New");
                            command.Parameters.Add("@StatusDate", SqlDbType.DateTime).Value = dr["StatusDate"];             // xmlDoc.CreateNode("StatusDate", DateTime.Now);
                            command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = dr["StartDate"];               // xmlDoc.CreateNode("StartDate", SchedStart);
                            command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = dr["EndDate"];                   // xmlDoc.CreateNode("EndDate", SchedEnd);
                            command.Parameters.Add("@FrequencyType", SqlDbType.SmallInt).Value = dr["Frequency"];           // xmlDoc.CreateNode("Frequency", iScheduleType);
                            command.Parameters.Add("@FrequencyParam", SqlDbType.SmallInt).Value = dr["FrequencyParam"];     // xmlDoc.CreateNode("FrequencyParam", ifreqParam);
                            command.Parameters.Add("@DeliveryMethod", SqlDbType.SmallInt).Value = dr["DeliveryMethod"];     // xmlDoc.CreateNode("DeliveryMethod"
                            command.Parameters.Add("@ReportLanguage", SqlDbType.VarChar, 16).Value = dr["ReportLanguage"];  // xmlDoc.CreateNode("ReportLanguage", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                            command.Parameters.Add("@ReportFormat", SqlDbType.SmallInt).Value = dr["ReportFormat"];         // xmlDoc.CreateNode("ReportFormat", sn.Report.ReportFormat); 

                            connection.Open();

                            int iRows = command.ExecuteNonQuery();

                            if (iRows > 0)
                                Message = "";
                            else
                                Message = "Update Schedule[" + ScheduleID + "] failed";
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message.ToString();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: GetReportRepositoryCountForMessage"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            }
            finally
            {
            }
            return string.IsNullOrEmpty(Message);
        }
    }

    private DataSet xmlStringToDataset(string xmlString) 
    {
        DataSet ds = new DataSet();
        try
        {
            ds.ReadXml(new StringReader(xmlString));
        }
        catch (Exception ex)
        {
        }
        return ds;
    }
}
