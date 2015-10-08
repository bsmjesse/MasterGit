using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections;
using SentinelFM;

/// <summary>
/// This class is for HOS
/// </summary>
/// 
public class clsHOSManager
{
    private string hosConnectionString = string.Empty;
    private string sentinelFMConnection = string.Empty;
    public static string ManualPPCID = "Manual Log";
    public static int QuestionSetlevel = 7;
	public clsHOSManager()
	{
        hosConnectionString =
            ConfigurationManager.ConnectionStrings["SentinelHOSConnectionString"].ConnectionString;
        sentinelFMConnection =
            ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
    }

    public DataSet GetHoursAlertEmail(int fleetId, int organizationId)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "GetHoursAlertEmail";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@FleetId", SqlDbType.Int);
            sqlPara.Value = fleetId;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            adapter.Fill(dataSet);
        }
        return dataSet;
    }

    public void UpdateHoursAlertEmail(int rowID,
         int      fleetId,
         string   email,
         int      organizationId,
         int      timezone,
         Boolean  autoAdjustDayLightSaving,
         Boolean  hoursViolation,
         Boolean  unsignInDriving,
         Boolean  preTripInspection,
         Boolean  postTripInspection,
         Boolean  drivingWithMajorDefect,
         Boolean  dpl_email_notification,
         Boolean driverLogsNotReceived3,
         Boolean driverLogsNotReceived2,
         Boolean logSheet,
         Boolean inspection
    )
    {
        SqlConnection connection;
        using (connection = new SqlConnection(hosConnectionString))
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = connection;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = "UpdateHoursAlertEmail";
            sqlCmd.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@rowID", SqlDbType.Int);
            sqlPara.Value = rowID;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@FleetId", SqlDbType.Int);
            sqlPara.Value = fleetId;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@Email", SqlDbType.VarChar);
            sqlPara.Value = email;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@Timezone", SqlDbType.Int);
            sqlPara.Value = timezone;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@AutoAdjustDayLightSaving", SqlDbType.Bit);
            sqlPara.Value = autoAdjustDayLightSaving;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@HoursViolation", SqlDbType.Bit);
            sqlPara.Value = hoursViolation;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@UnsignInDriving", SqlDbType.Bit);
            sqlPara.Value = unsignInDriving;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@PreTripInspection", SqlDbType.Bit);
            sqlPara.Value = preTripInspection;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@PostTripInspection", SqlDbType.Bit);
            sqlPara.Value = postTripInspection;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@DrivingWithMajorDefect", SqlDbType.Bit);
            sqlPara.Value = drivingWithMajorDefect;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@DPLEmailNotif", SqlDbType.Bit);
            sqlPara.Value = dpl_email_notification;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@DriverLogNotReceived3Days", SqlDbType.Bit);
            sqlPara.Value = driverLogsNotReceived3;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@DriverLogNotReceived2Days", SqlDbType.Bit);
            sqlPara.Value = driverLogsNotReceived2;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@LogSheet", SqlDbType.Bit);
            sqlPara.Value = logSheet;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@Inspection", SqlDbType.Bit);
            sqlPara.Value = inspection;
            sqlCmd.Parameters.Add(sqlPara);

            connection.Open();
            sqlCmd.ExecuteNonQuery();
        }

    }

    public void AddHoursAlertEmail(
         int fleetId,
         string email,
         int organizationId,
         int timezone,
         Boolean autoAdjustDayLightSaving,
         Boolean hoursViolation,
         Boolean unsignInDriving,
         Boolean preTripInspection,
         Boolean postTripInspection,
         Boolean drivingWithMajorDefect,
         Boolean dpl_email_notification,        
         Boolean driverLogsNotReceived3,
         Boolean driverLogsNotReceived2,
         Boolean logSheet,
         Boolean inspection
    )
    {
        SqlConnection connection;
        using (connection = new SqlConnection(hosConnectionString))
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = connection;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = "AddHoursAlertEmail";
            sqlCmd.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@FleetId", SqlDbType.Int);
            sqlPara.Value = fleetId;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@Email", SqlDbType.VarChar);
            sqlPara.Value = email;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@Timezone", SqlDbType.Int);
            sqlPara.Value = timezone;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@AutoAdjustDayLightSaving", SqlDbType.Bit);
            sqlPara.Value = autoAdjustDayLightSaving;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@HoursViolation", SqlDbType.Bit);
            sqlPara.Value = hoursViolation;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@UnsignInDriving", SqlDbType.Bit);
            sqlPara.Value = unsignInDriving;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@PreTripInspection", SqlDbType.Bit);
            sqlPara.Value = preTripInspection;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@PostTripInspection", SqlDbType.Bit);
            sqlPara.Value = postTripInspection;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@DrivingWithMajorDefect", SqlDbType.Bit);
            sqlPara.Value = drivingWithMajorDefect;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@DPLEmailNotif", SqlDbType.Bit);
            sqlPara.Value = dpl_email_notification;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@DriverLogNotReceived3Days", SqlDbType.Bit);
            sqlPara.Value = driverLogsNotReceived3;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@DriverLogNotReceived2Days", SqlDbType.Bit);
            sqlPara.Value = driverLogsNotReceived2;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@LogSheet", SqlDbType.Bit);
            sqlPara.Value = logSheet;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@Inspection", SqlDbType.Bit);
            sqlPara.Value = inspection;
            sqlCmd.Parameters.Add(sqlPara);

            connection.Open();
            sqlCmd.ExecuteNonQuery();
        }

    }

    public void DeleteHoursAlertEmail(
         int rowID,
         int organizationId
    )
    {
        SqlConnection connection;
        using (connection = new SqlConnection(hosConnectionString))
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = connection;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = "DeleteHoursAlertEmail";
            sqlCmd.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@rowID", SqlDbType.Int);
            sqlPara.Value = rowID;
            sqlCmd.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            sqlCmd.Parameters.Add(sqlPara);

            connection.Open();
            sqlCmd.ExecuteNonQuery();
        }

    }

    public DataSet GetVehiclesHOSVersion(int fleetId, int organizationId)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "GetVehiclesHOSVersion";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@FleetId", SqlDbType.Int);
            sqlPara.Value = fleetId;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            adapter.Fill(dataSet);
        }
        return dataSet;
    }

    public DataSet Get_LogData_Fuel(int BoxId, DateTime DateFrom, DateTime DateTo)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "Get_LogData_Fuel";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@BoxId", SqlDbType.Int);
            sqlPara.Value = BoxId;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@DateFrom", SqlDbType.DateTime);
            sqlPara.Value = DateFrom;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@DateTo", SqlDbType.DateTime);
            sqlPara.Value = DateTo;
            adapter.SelectCommand.Parameters.Add(sqlPara);


            adapter.Fill(dataSet);
        }
        return dataSet;
    }

    public DataSet GetLogbookStop(int type)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetLogbookStop";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@type", SqlDbType.Int);
            sqlPara.Value = type;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            adapter.Fill(dataSet);
        }
        return dataSet;
    }


    public DataSet GetStates()
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetStates";
            adapter.SelectCommand.Connection = connection;
            adapter.Fill(dataSet);
        }
        return dataSet;
    }

    public DataSet GetCities(int state)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetCities";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@State", SqlDbType.Int);
            sqlPara.Value = state;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            adapter.Fill(dataSet);
        }
        return dataSet;
    }

    public DataSet GetManualLog(string driver, DateTime logTimeFrom, DateTime LogTimeTo, string PPCID)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetManualLog";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@Driver", SqlDbType.VarChar);
            sqlPara.Value = driver;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@LogTimeFrom", SqlDbType.DateTime);
            sqlPara.Value = logTimeFrom;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@LogTimeTo", SqlDbType.DateTime);
            sqlPara.Value = LogTimeTo;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@PPCID", SqlDbType.VarChar);
            sqlPara.Value = PPCID;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            adapter.Fill(dataSet);
        }
        return dataSet;
    }

    public DataSet GetEmployee(int organizationId, Boolean isActive)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "VerigoManager_GetEmployeeNew";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@companyid", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@isActive", SqlDbType.Bit);
            sqlPara.Value = isActive;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            adapter.Fill(dataSet);
        }
        return dataSet;
    }

    // New method to for Fetching the ReportLogSheet on the basis of fleet id

    public DataSet GetReportLogSheet(int CompanyId, DateTime dateFrom, DateTime dateTo, int  fleetId)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "dbo.VerigoManager_GetReportLogSheet_ByFleet";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@companyId", SqlDbType.Int);
            sqlPara.Value = CompanyId;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@Start", SqlDbType.DateTime);
            sqlPara.Value = dateFrom;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@stop", SqlDbType.DateTime);
            sqlPara.Value = dateTo;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@fleetId", SqlDbType.VarChar);
            sqlPara.Value = fleetId;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            adapter.Fill(dataSet);
        }
        return dataSet;
    }

    public DataSet GetReportLogSheetByDriver(int CompanyId, DateTime dateFrom, DateTime dateTo, string diverId)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "dbo.VerigoManager_GetReportLogSheet_ByDriverNoFleet";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@companyId", SqlDbType.Int);
            sqlPara.Value = CompanyId;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@Start", SqlDbType.DateTime);
            sqlPara.Value = dateFrom;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@stop", SqlDbType.DateTime);
            sqlPara.Value = dateTo;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@driverId", SqlDbType.VarChar);
            sqlPara.Value = diverId;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            adapter.Fill(dataSet);
        }
        return dataSet;
    }
    // Changes for TimeZone Feature start

    public DataSet GetLogData_Event_NewTZ(int boxid, DateTime dateFrom, DateTime dateTo, float timeZone, string driver)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetLogData_Event_NewTimeZone";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@BoxId", SqlDbType.Int);
            sqlPara.Value = boxid;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@DateFrom", SqlDbType.DateTime);
            sqlPara.Value = dateFrom;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@DateTo", SqlDbType.DateTime);
            sqlPara.Value = dateTo;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@TimeZone", SqlDbType.Int);
            sqlPara.Value = timeZone;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@Driver", SqlDbType.VarChar);
            sqlPara.Value = driver;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            adapter.Fill(dataSet);
        }
        return dataSet;
    }

    // Changes for TimeZone Feature end

    public DataSet GetLogData_Event(int boxid, DateTime dateFrom, DateTime dateTo, int timeZone, string driver)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetLogData_Event";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@BoxId", SqlDbType.Int);
            sqlPara.Value = boxid;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@DateFrom", SqlDbType.DateTime);
            sqlPara.Value = dateFrom;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@DateTo", SqlDbType.DateTime);
            sqlPara.Value = dateTo;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@TimeZone", SqlDbType.Int);
            sqlPara.Value = timeZone;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@Driver", SqlDbType.VarChar);
            sqlPara.Value = driver;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            adapter.Fill(dataSet);
        }
        return dataSet;
    }


    public string GetDataByDaysForFlatFile(DateTime dateStart, DateTime dateEnd, String driver, Boolean isIncludename, 
        SentinelFMSession sn)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        string firstName = string.Empty;
        string lastName = string.Empty;
        string companyName = string.Empty;
        DateTime daylightStart = DateTime.MinValue;
        DateTime daylightEnd = DateTime.MinValue;
        List<FlatFileData> flatFileDatas = new List<FlatFileData>();
        String noTimeZone = string.Empty;
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetDataByDaysForFlatFile";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@dateStart", SqlDbType.DateTime);
            sqlPara.Value = dateStart;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@dateEnd", SqlDbType.DateTime);
            sqlPara.Value = dateEnd;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@driver", SqlDbType.VarChar);
            sqlPara.Value = driver;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@firstname", SqlDbType.VarChar, 50);
            sqlPara.Direction = ParameterDirection.Output;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@lastname", SqlDbType.VarChar, 50);
            sqlPara.Direction = ParameterDirection.Output;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@companyname", SqlDbType.VarChar, 200);
            sqlPara.Direction = ParameterDirection.Output;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@daylightStart", SqlDbType.DateTime);
            sqlPara.Direction = ParameterDirection.Output;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@daylightEnd", SqlDbType.DateTime);
            sqlPara.Direction = ParameterDirection.Output;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            adapter.Fill(dataSet);

            if (!(adapter.SelectCommand.Parameters["@firstname"].Value is DBNull) && isIncludename)
                firstName = adapter.SelectCommand.Parameters["@firstname"].Value.ToString();

            if (!(adapter.SelectCommand.Parameters["@lastname"].Value is DBNull) && isIncludename)
                lastName = adapter.SelectCommand.Parameters["@lastname"].Value.ToString();

            if (!(adapter.SelectCommand.Parameters["@companyName"].Value is DBNull))
                companyName = adapter.SelectCommand.Parameters["@companyName"].Value.ToString();

            if (!(adapter.SelectCommand.Parameters["@daylightStart"].Value is DBNull))
                daylightStart = (DateTime)adapter.SelectCommand.Parameters["@daylightStart"].Value;

            if (!(adapter.SelectCommand.Parameters["@daylightEnd"].Value is DBNull))
                daylightEnd = (DateTime)adapter.SelectCommand.Parameters["@daylightEnd"].Value;

            if (dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dataSet.Tables[0].Rows)
                {
                    int rowIndex = 0;
                    int refid = -1;
                    int boxID = -1;
                    DateTime refDate = DateTime.MinValue;
                    float timezone = -999;
                    string vinNum = string.Empty;
                    string equipmentNo = string.Empty;
                    Dictionary<int, string> remarks = new Dictionary<int, string>();
                    List<DateTime> drivingTimes = new List<DateTime>();
                    if (dr["boxId"] != DBNull.Value)
                        boxID = int.Parse(dr["boxId"].ToString());

                    if (dr["timezone"] != DBNull.Value)
                        timezone = float.Parse(dr["timezone"].ToString());

                    if (dr["refid"] != DBNull.Value)
                        refid = int.Parse(dr["refid"].ToString());

                    if (dr["Date"] != DBNull.Value)
                        refDate = (DateTime)dr["Date"];

                    if (dr["VinNum"] != DBNull.Value)
                        vinNum = dr["VinNum"].ToString();

                    if (dr["Equipment"] != DBNull.Value)
                        equipmentNo = dr["Equipment"].ToString();

                    if (timezone == -999 && refDate != DateTime.MinValue)
                    { 
                        if (noTimeZone == string.Empty ) noTimeZone = String.Format("{0:yyyyMMdd}", refDate);
                        else noTimeZone = noTimeZone + ","+ String.Format("{0:yyyyMMdd}", refDate);
                    }
                    string coDriverId = string.Empty; 
                    string coDriverFirstName = string.Empty;
                    string coDriverLastName = string.Empty;
                    DateTime dayEnd = DateTime.MinValue;

                    if (boxID != -1 && dataSet.Tables[2].Rows.Count > 0)
                    {
                        DataRow[] codrivers = dataSet.Tables[1].Select("RefID=" + refid);
                        DataRow[] timelogs = dataSet.Tables[2].Select("RefID=" + refid);
                        DataRow[] trips = dataSet.Tables[3].Select("RefID=" + refid);
                        DataRow[] modifies = dataSet.Tables[5].Select("RefID=" + refid);
                        DataRow[] errors = dataSet.Tables[6].Select("refid=" + refid);
                        if (timelogs.Length == 0) continue;
                        foreach(DataRow drcoworker in codrivers)
                        {
                            if (drcoworker["coDriverID"] != DBNull.Value)
                            {
                                if (coDriverId != string.Empty) coDriverId = drcoworker["coDriverID"].ToString();
                                else coDriverId = coDriverId + "," + drcoworker["coDriverID"].ToString();
                            }

                            if (isIncludename)
                            {
                                if (drcoworker["firstname"] != DBNull.Value)
                                {
                                    if (coDriverFirstName != string.Empty) coDriverFirstName = drcoworker["firstname"].ToString();
                                    else coDriverFirstName = coDriverFirstName + "," + drcoworker["firstname"].ToString();
                                }
                                if (drcoworker["lastname"] != DBNull.Value)
                                {
                                    if (coDriverLastName != string.Empty) coDriverLastName = drcoworker["lastname"].ToString();
                                    else coDriverLastName = coDriverLastName + "," + drcoworker["lastname"].ToString();
                                }
                            }
                        }

                        //timelog
                        DateTime logMinTime = DateTime.MaxValue;
                        DateTime logMaxTime = DateTime.MinValue;
                        DateTime prevoiusDrivingTime = DateTime.MinValue;
                        
                        for(int logIndex = 0; logIndex < timelogs.Length; logIndex++)
                        {
                            DataRow drlogs = timelogs[logIndex];
                            if (logIndex == 0 && 
                                drlogs["Type"] != DBNull.Value && 
                                drlogs["Type"].ToString() == "1" &&
                                drlogs["event"] != DBNull.Value && 
                                drlogs["event"].ToString() == "101")
                                continue;
                            FlatFileData flatData = new FlatFileData();
                            flatData.EventSequenceID = rowIndex++;
                            flatData.CarrierName = companyName;
                            flatData.CoDriverFirstName = coDriverFirstName;
                            flatData.CoDriverID = coDriverId;
                            flatData.CoDriverLastName = coDriverLastName;
                            flatData.DriverFirstName = firstName;
                            flatData.DriverLastName = lastName;
                            flatData.DriverPin = driver;
                            flatData.EventDateTime = ((DateTime)drlogs["logtime"]);
                            flatData.Vin = vinNum;
                            if (dayEnd == DateTime.MinValue) dayEnd = flatData.EventDateTime.Date.AddHours(24);
                            if (timezone != -999)
                            {
                                flatData.EventDateTime = flatData.EventDateTime.AddHours(-1 * timezone);
                                dayEnd = dayEnd.AddHours(-1 * timezone);
                                if (flatData.EventDateTime >= daylightStart && flatData.EventDateTime < daylightEnd)
                                {
                                    flatData.EventDateTime = flatData.EventDateTime.AddHours(-1);
                                    dayEnd = dayEnd.AddHours(-1);
                                }
                            }
                             if (logMinTime > flatData.EventDateTime) 
                                 logMinTime = flatData.EventDateTime;

                             if (logMaxTime < flatData.EventDateTime) 
                                 logMaxTime = flatData.EventDateTime;

                            flatData.EventDate = int.Parse(string.Format("{0:yyyyMMdd}", flatData.EventDateTime));
                            flatData.EventTime = int.Parse(string.Format("{0:HHmmss}", flatData.EventDateTime));
                            if (drlogs["Latitude"] != DBNull.Value)
                                flatData.EventLatitude = float.Parse(drlogs["Latitude"].ToString());
                            if (drlogs["Longitude"] != DBNull.Value)
                                flatData.EventLongitude = float.Parse(drlogs["Longitude"].ToString());
                            if (drlogs["Address"] != DBNull.Value)
                               flatData.EventPlace = drlogs["Address"].ToString();
                            else
                            {
                                if (drlogs["city"] != DBNull.Value)
                                    flatData.EventPlace = drlogs["city"].ToString();
                            }

                            //For 60 minutes interval when in motion
                            if (prevoiusDrivingTime != DateTime.MinValue && 
                                flatData.EventDateTime.Subtract(prevoiusDrivingTime).TotalMinutes > 50)
                            {
                                DateTime tmpDateTime = prevoiusDrivingTime.AddMinutes(30);
                                while (tmpDateTime < flatData.EventDateTime)
                                {
                                    if (!drivingTimes.Contains(tmpDateTime))
                                        drivingTimes.Add(tmpDateTime);
                                    prevoiusDrivingTime = tmpDateTime;
                                    tmpDateTime = tmpDateTime.AddMinutes(30);
                                }
                            }

                            if (drlogs["event"] != DBNull.Value)
                            {
                                if (drlogs["event"].ToString() == "101") flatData.EventStatusCode = "OFF";
                                if (drlogs["event"].ToString() == "102") flatData.EventStatusCode = "SB";
                                if (drlogs["event"].ToString() == "103")
                                {
                                    flatData.EventStatusCode = "D";
                                    prevoiusDrivingTime = flatData.EventDateTime;
                                    if (logIndex == timelogs.Length - 1)
                                    {
                                        DateTime tmpDateTime = prevoiusDrivingTime.AddMinutes(30);
                                        while (tmpDateTime < dayEnd)
                                        {
                                            if (!drivingTimes.Contains(tmpDateTime))
                                                drivingTimes.Add(tmpDateTime);
                                            prevoiusDrivingTime = tmpDateTime;
                                            tmpDateTime = tmpDateTime.AddMinutes(30);
                                        }

                                    }
                                }
                                else  prevoiusDrivingTime = DateTime.MinValue;
                                if (drlogs["event"].ToString() == "104") flatData.EventStatusCode = "ON";
                            }

                            if (drlogs["odometer"] != DBNull.Value)
                            {
                                flatData.PlaceTotalDistance = int.Parse(drlogs["odometer"].ToString());
                            }

                            List<string> equipments = new List<string>();
                            List<string> tractors = new List<string>();
                            List<string> docs = new List<string>();

                            foreach (DataRow drTrip in trips)
                            { 
                                DateTime tripStart = (DateTime)drTrip["logtime"];
                                if (timezone != -999)
                                {
                                    tripStart = tripStart.AddHours(-1 * timezone);
                                    if (tripStart >= daylightStart && tripStart < daylightEnd)
                                    {
                                        tripStart = tripStart.AddHours(-1);
                                    }
                                }
                                if (flatData.EventDateTime >= tripStart  )
                                {
                                    if (drTrip["blnumber"] != DBNull.Value)
                                    {
                                        string doc = drTrip["blnumber"].ToString();
                                        if (doc != string.Empty && !docs.Contains(doc)) docs.Add(doc);
                                    }

                                    if (drTrip["truck"] != DBNull.Value)
                                    {
                                        string equiStr = drTrip["truck"].ToString();
                                        string equipment = string.Empty;
                                        string tractor = string.Empty;
                                        if (equiStr != null)
                                        {
                                            equiStr = equiStr.Trim();
                                            ArrayList ar = ParseLicenseandtractor(equiStr);
                                            equipment = (String)ar[0];
                                            tractor = (String)ar[2];
                                        }
                                        if (equipment != string.Empty && !equipments.Contains(equipment)) equipments.Add(equipment);
                                        if (tractor != string.Empty && !tractors.Contains(tractor)) tractors.Add(tractor);
                                    }
                                }
                            }
                            foreach (string equip in equipments)
                            {
                                if (flatData.Tractor == string.Empty) flatData.Tractor = equip;
                                else flatData.Tractor = flatData.Tractor + ";" + equip;
                            }

                            if (flatData.Tractor == string.Empty)
                                flatData.Tractor = equipmentNo;
                            foreach (string tractor in tractors)
                            {
                                if (flatData.Trail == string.Empty) flatData.Trail = tractor;
                                else flatData.Trail = flatData.Trail + ";" + tractor;
                            }

                            foreach (string doc in docs)
                            {
                                if (flatData.ShippingDocument == string.Empty) flatData.ShippingDocument = doc;
                                else flatData.ShippingDocument = flatData.ShippingDocument + "/" + doc;
                            }
                            
                            flatData.EventUpdateStatusCode = "C";
                            if (drlogs["Remark"] != DBNull.Value && 
                                drlogs["Remark"].ToString().Trim() != string.Empty && 
                                drlogs["TLID"] != DBNull.Value )
                            {
                                remarks.Add(int.Parse(drlogs["TLID"].ToString()), drlogs["Remark"].ToString()); 
                            }

                            flatFileDatas.Add(flatData);
                        }

                        if (logMinTime != DateTime.MinValue && logMaxTime != DateTime.MinValue && flatFileDatas.Count > 0 &&
                            dayEnd != DateTime.MinValue)
                        {
                            if (flatFileDatas[flatFileDatas.Count - 1].EventStatusCode != "OFF") logMaxTime = dayEnd;
                        }

                        //ModifyCode
                        foreach(DataRow drModifies in modifies)
                        {
                            FlatFileData flatData = new FlatFileData();
                            flatData.EventSequenceID = rowIndex++;
                            flatData.CarrierName = companyName;
                            flatData.CoDriverFirstName = coDriverFirstName;
                            flatData.CoDriverID = coDriverId;
                            flatData.CoDriverLastName = coDriverLastName;
                            flatData.DriverFirstName = firstName;
                            flatData.DriverLastName = lastName;
                            flatData.DriverPin = driver;
                            flatData.EventDateTime = DateTime.ParseExact(drModifies["logTime"].ToString(), "yyyyMMddHHmmss", new System.Globalization.CultureInfo("en-US"));
                                
                            flatData.Vin = vinNum;
                            if (timezone != -999)
                            {
                                flatData.EventDateTime = flatData.EventDateTime.AddHours(-1 * timezone);

                                if (flatData.EventDateTime >= daylightStart && flatData.EventDateTime < daylightEnd)
                                {
                                    flatData.EventDateTime = flatData.EventDateTime.AddHours(-1);
                                }
                            }
                            flatData.EventDate = int.Parse(string.Format("{0:yyyyMMdd}", flatData.EventDateTime));
                            flatData.EventTime = int.Parse(string.Format("{0:HHmmss}", flatData.EventDateTime));
                            string address = "";
                            if (drModifies["location"] != DBNull.Value)
                            {
                                address = drModifies["location"].ToString();
                                string Address = string.Empty;
                                if (address.Contains(";"))
                                {
                                    string[] locDetails = address.Split(';');
                                    if (locDetails.Length > 2)
                                    {
                                        float Latitude = 0;
                                        float.TryParse(locDetails[1], out Latitude);
                                        float Longitude = 0;
                                        float.TryParse(locDetails[2], out Longitude);
                                        if (Longitude != 0 && Latitude != 0)
                                        {
                                            flatData.EventLatitude = Latitude;
                                            flatData.EventLongitude = Longitude;
                                        }
                                    }
                                    address = locDetails[0];
                                }
                            }

                            if (drModifies["Address"] != DBNull.Value)
                            {
                                flatData.EventPlace = drModifies["Address"].ToString();
                            }

                            if (drModifies["eventStr"] != DBNull.Value)
                            {
                                flatData.EventStatusCode = drModifies["eventStr"].ToString();
                                if (flatData.EventStatusCode == "OffDuty") flatData.EventStatusCode = "OFF";
                                if (flatData.EventStatusCode == "OnDuty") flatData.EventStatusCode = "ON";
                                if (flatData.EventStatusCode == "Sleeping") flatData.EventStatusCode = "SB";
                                if (flatData.EventStatusCode == "Driving") flatData.EventStatusCode = "D";
                            }

                            if (drModifies["odometer"] != DBNull.Value)
                            {
                                flatData.PlaceTotalDistance = int.Parse(drModifies["odometer"].ToString());
                            }

                            flatData.EventUpdateStatusCode = "H";

                            if (drModifies["TLID"] != DBNull.Value)
                            {
                                int sourceTlic = int.Parse(drModifies["TLID"].ToString());
                                if (remarks.ContainsKey(sourceTlic))
                                    flatData.EventUpdatedText = remarks[sourceTlic];
                            }

                            flatData.EventUpdatedPersonId = driver;
                            flatFileDatas.Add(flatData);
                        }
                        //End modify

                        foreach(DataRow eventRow in dataSet.Tables[4].Rows)
                        {
                            DateTime eDateTime = (DateTime)eventRow["EventDateTime"];

                            if (eDateTime >= logMinTime && eDateTime <= logMaxTime)
                            {
                                FlatFileData flatData = new FlatFileData();
                                flatData.EventSequenceID = rowIndex++;
                                flatData.CarrierName = companyName;
                                flatData.CoDriverFirstName = coDriverFirstName;
                                flatData.CoDriverID = coDriverId;
                                flatData.CoDriverLastName = coDriverLastName;
                                flatData.DriverFirstName = firstName;
                                flatData.DriverLastName = lastName;
                                flatData.DriverPin = driver;
                                flatData.EventDateTime = eDateTime;
                                flatData.Vin = vinNum;
                                flatData.EventDate = int.Parse(string.Format("{0:yyyyMMdd}", flatData.EventDateTime));
                                flatData.EventTime = int.Parse(string.Format("{0:HHmmss}", flatData.EventDateTime));
                                flatData.EventStatusCode = "DG";
                                if (eventRow["EventId"].ToString() == "20")
                                {
                                    flatData.DiagnosticEventCode = "TESTOK";
                                }
                                if (eventRow["EventId"].ToString() == "22" || 
                                    eventRow["EventId"].ToString() == "21")
                                {
                                    flatData.DiagnosticEventCode = "DLOADY";
                                }
                                if (eventRow["EventId"].ToString() == "24")
                                {
                                    flatData.DiagnosticEventCode = "NODRID";
                                }
                                if (eventRow["EventId"].ToString() == "25")
                                {
                                    flatData.DiagnosticEventCode = "INTFUL";
                                }
                            }
                        }
                        //End
                        
                        //History Start
                         if (logMinTime != DateTime.MaxValue && logMaxTime != DateTime.MinValue && timezone != -999
                             && logMaxTime.Subtract(logMinTime).TotalHours <= 24)
                         {
                             DataTable dt = GetHistoryDataForHOSFlatFile(boxID, logMinTime.AddHours(-2), logMaxTime.AddHours(2));
                              DateTime invalidDT = DateTime.MinValue;
                              foreach(DataRow hisDr in dt.Rows)
                              {
                                  DateTime eventDateTime = (DateTime)hisDr["OriginDateTime"];
                                  //Power off and on
                                  if (int.Parse(hisDr["BoxMsgInTypeId"].ToString()) == 76)
                                  {
                                      FlatFileData flatData = new FlatFileData();
                                      flatData.EventSequenceID = rowIndex++;
                                      flatData.CarrierName = companyName;
                                      flatData.CoDriverFirstName = coDriverFirstName;
                                      flatData.CoDriverID = coDriverId;
                                      flatData.CoDriverLastName = coDriverLastName;
                                      flatData.DriverFirstName = firstName;
                                      flatData.DriverLastName = lastName;
                                      flatData.DriverPin = driver;
                                      flatData.EventDateTime = eventDateTime;
                                      if (hisDr["Latitude"] != DBNull.Value)
                                          flatData.EventLatitude = float.Parse(hisDr["Latitude"].ToString());
                                      if (hisDr["Longitude"] != DBNull.Value)
                                          flatData.EventLongitude = float.Parse(hisDr["Longitude"].ToString());
                                      if (hisDr["StreetAddress"] != DBNull.Value)
                                          flatData.EventPlace = hisDr["StreetAddress"].ToString();

                                      flatData.Vin = vinNum;
                                      flatData.EventDate = int.Parse(string.Format("{0:yyyyMMdd}", flatData.EventDateTime));
                                      flatData.EventTime = int.Parse(string.Format("{0:HHmmss}", flatData.EventDateTime));
                                      flatData.EventStatusCode = "DG";
                                      string customProp = string.Empty;
                                      if (hisDr["CustomProp"] != DBNull.Value)
                                      {
                                          customProp = hisDr["CustomProp"].ToString();
                                          if (customProp.IndexOf("Tethered=ON") >= 0)
                                              flatData.DiagnosticEventCode = "PWR_ON";
                                          if (customProp.IndexOf("Tethered=OFF") >= 0)
                                              flatData.DiagnosticEventCode = "PWROFF";
                                          
                                      }
                                      if (hisDr["Odometer"] != DBNull.Value)
                                      {
                                          Double odoDouble = 0;
                                          if (double.TryParse(hisDr["Odometer"].ToString().Trim(), out odoDouble))
                                          {
                                              if (odoDouble != 0)
                                              {
                                                  flatData.PlaceTotalDistance = (int)odoDouble;
                                                  flatData.PlaceTotalDistance = (int)(flatData.PlaceTotalDistance * sn.User.UnitOfMes);
                                              }
                                          }
                                      }

                                      flatFileDatas.Add(flatData);
                                  }

                                  //for 60 minutes motion
                                  for(int curIndex  = 0; curIndex < drivingTimes.Count; curIndex++)
                                  {
                                      DateTime drivingTime = drivingTimes[curIndex];
                                      if (drivingTime != DateTime.MinValue &&
                                          (
                                            (eventDateTime >= drivingTime && eventDateTime.Subtract(drivingTime).TotalMinutes < 10) ||
                                            (drivingTime >= eventDateTime && drivingTime.Subtract(eventDateTime).TotalMinutes < 10)
                                          )
                                         )
                                      {
                                          drivingTimes[curIndex] = DateTime.MinValue;
                                          FlatFileData flatData = new FlatFileData();
                                          flatData.EventSequenceID = rowIndex++;
                                          flatData.CarrierName = companyName;
                                          flatData.CoDriverFirstName = coDriverFirstName;
                                          flatData.CoDriverID = coDriverId;
                                          flatData.CoDriverLastName = coDriverLastName;
                                          flatData.DriverFirstName = firstName;
                                          flatData.DriverLastName = lastName;
                                          flatData.DriverPin = driver;
                                          flatData.EventDateTime = eventDateTime;
                                          if (hisDr["Latitude"] != DBNull.Value)
                                              flatData.EventLatitude = float.Parse(hisDr["Latitude"].ToString());
                                          if (hisDr["Longitude"] != DBNull.Value)
                                              flatData.EventLongitude = float.Parse(hisDr["Longitude"].ToString());
                                          if (hisDr["StreetAddress"] != DBNull.Value)
                                              flatData.EventPlace = hisDr["StreetAddress"].ToString();

                                          flatData.Vin = vinNum;
                                          flatData.EventDate = int.Parse(string.Format("{0:yyyyMMdd}", flatData.EventDateTime));
                                          flatData.EventTime = int.Parse(string.Format("{0:HHmmss}", flatData.EventDateTime));
                                          flatData.EventStatusCode = "D";
                                          string customProp = string.Empty;
                                          if (hisDr["Odometer"] != DBNull.Value)
                                          {
                                              Double odoDouble = 0;
                                              if (double.TryParse(hisDr["Odometer"].ToString().Trim(), out odoDouble))
                                              {
                                                  if (odoDouble != 0)
                                                  {
                                                      flatData.PlaceTotalDistance = (int)odoDouble;
                                                      flatData.PlaceTotalDistance = (int)(flatData.PlaceTotalDistance * sn.User.UnitOfMes);
                                                  }
                                              }
                                          }

                                          flatFileDatas.Add(flatData);
                                      }
                                  }

                                  //InvalidGPS
                                  if (hisDr["ValidGps"] != DBNull.Value)
                                  {
                                      if (hisDr["ValidGps"].ToString() == "1" && eventDateTime >= logMinTime && eventDateTime <= logMaxTime)
                                      {
                                          if (invalidDT != DateTime.MinValue &&
                                              eventDateTime.Subtract(invalidDT).TotalMinutes > 30)
                                          {
                                              FlatFileData flatData = new FlatFileData();
                                              flatData.EventSequenceID = rowIndex++;
                                              flatData.CarrierName = companyName;
                                              flatData.CoDriverFirstName = coDriverFirstName;
                                              flatData.CoDriverID = coDriverId;
                                              flatData.CoDriverLastName = coDriverLastName;
                                              flatData.DriverFirstName = firstName;
                                              flatData.DriverLastName = lastName;
                                              flatData.DriverPin = driver;
                                              flatData.EventDateTime = eventDateTime;
                                              if (hisDr["Latitude"] != DBNull.Value)
                                                  flatData.EventLatitude = float.Parse(hisDr["Latitude"].ToString());
                                              if (hisDr["Longitude"] != DBNull.Value)
                                                  flatData.EventLongitude = float.Parse(hisDr["Longitude"].ToString());
                                              if (hisDr["StreetAddress"] != DBNull.Value)
                                                  flatData.EventPlace = hisDr["StreetAddress"].ToString();

                                              flatData.Vin = vinNum;
                                              flatData.EventDate = int.Parse(string.Format("{0:yyyyMMdd}", flatData.EventDateTime));
                                              flatData.EventTime = int.Parse(string.Format("{0:HHmmss}", flatData.EventDateTime));
                                              flatData.EventStatusCode = "DG";
                                              string customProp = string.Empty;
                                              flatData.DiagnosticEventCode = "NOLTLN";
                                              if (hisDr["Odometer"] != DBNull.Value)
                                              {
                                                  Double odoDouble = 0;
                                                  if (double.TryParse(hisDr["Odometer"].ToString().Trim(), out odoDouble))
                                                  {
                                                      if (odoDouble != 0)
                                                      {
                                                          flatData.PlaceTotalDistance = (int)odoDouble;
                                                          flatData.PlaceTotalDistance = (int)(flatData.PlaceTotalDistance * sn.User.UnitOfMes);
                                                      }
                                                  }
                                              }

                                              flatFileDatas.Add(flatData);
                                          }
                                          else
                                          {
                                              if (invalidDT == DateTime.MinValue) invalidDT = eventDateTime;
                                          }
                                      }
                                      else invalidDT = DateTime.MinValue;
                                  }

                              }
                         }
                         //History End 

                    }
                }
            }
        }
        return CreateFlatFile(flatFileDatas, noTimeZone);
    }

    private string TractorSymbol = "@@@@T:";
    private string LicenseSymbol = "@@@@L:";
    private ArrayList ParseLicenseandtractor(string equip)
    {
        string license = string.Empty;
        string tractor = string.Empty;
        int i_pos = equip.LastIndexOf(TractorSymbol);
        if (i_pos >= 0 && equip.Length > i_pos + TractorSymbol.Length)
        {
            tractor = equip.Substring(i_pos + TractorSymbol.Length);
            equip = equip.Substring(0, i_pos);
        }
        i_pos = equip.LastIndexOf(LicenseSymbol);
        if (i_pos >= 0 && equip.Length > i_pos + LicenseSymbol.Length)
        {
            license = equip.Substring(i_pos + LicenseSymbol.Length);
            equip = equip.Substring(0, i_pos);
        }
        ArrayList al = new ArrayList();
        al.Add(equip);
        al.Add(license);
        al.Add(tractor);
        return al;
    }

    private DataTable GetHistoryDataForHOSFlatFile(int boxID, DateTime dtFrom, DateTime dtTo)
    {
        DataTable dataSet = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(sentinelFMConnection))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_GetHistoryDataForHOSFlatFile";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@BoxId", SqlDbType.Int);
            sqlPara.Value = boxID;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@dtFrom", SqlDbType.DateTime);
            sqlPara.Value = dtFrom;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@dtTo", SqlDbType.DateTime);
            sqlPara.Value = dtTo;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            adapter.Fill(dataSet);
        }
        return dataSet;
    }

    private string CreateFlatFile(List<FlatFileData> flatFileDatas, string noTimeZone)
    {
        flatFileDatas.Sort(
            delegate(FlatFileData p1, FlatFileData p2)
            {
                if (p1.EventDateTime != p2.EventDateTime)
                  return p1.EventDateTime.CompareTo(p2.EventDateTime);
                else return p1.EventSequenceID.CompareTo(p2.EventSequenceID);
            }
            );


        StringBuilder strTmp = new StringBuilder();
        int index = 0;
        strTmp.Append("The flat file is delimited by '@@'." + Environment.NewLine);
        if (noTimeZone != string.Empty )
            strTmp.Append("Data are loca time for date:" + noTimeZone + "." + Environment.NewLine);
        strTmp.Append(Environment.NewLine);
        foreach (FlatFileData flatFileRecord in flatFileDatas)
        {
            flatFileRecord.EventSequenceID = index++;
            strTmp.Append(GenerateFlatFileRecord(flatFileRecord));
        }
        return strTmp.ToString();
    }

    private string GenerateFlatFileRecord(FlatFileData flatFileRecord)
    {
        StringBuilder curRecord = new StringBuilder();
        curRecord.Append(GenerateFlatFileField(flatFileRecord.DriverFirstName, 'A', 35));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.DriverLastName, 'A', 35));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.DriverPin, 'A', 40));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.Tractor, 'A', 10));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.Trail, 'A', 10));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.Vin, 'A', 17));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.CoDriverFirstName, 'A', 35));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.CoDriverLastName, 'A', 35));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.CoDriverID, 'A', 40));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.USDOT.ToString(), 'N', 8));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.CarrierName, 'A', 120));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.ShippingDocument, 'A', 40));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.EventSequenceID.ToString(), 'N', 4));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.EventStatusCode, 'A', 3));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.EventDate.ToString(), 'N', 8));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.EventTime.ToString(), 'N', 6));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.EventLatitude.ToString(), 'N', 9));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.EventLongitude.ToString(), 'N', 10));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.EventPlace.ToString(), 'N', 5));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.PlaceDistance.ToString(), 'N', 4));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.PlaceTotalDistance.ToString(), 'N', 7));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.EventUpdateStatusCode, 'A', 1));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.DiagnosticEventCode, 'A', 2));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.EventErrorCode, 'A', 2));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.EventUpdatedDate.ToString(), 'N', 8));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.EventUpdatedTime.ToString(), 'N', 6));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.EventUpdatedPersonId.ToString(), 'A', 40));
        curRecord.Append("@@" + GenerateFlatFileField(flatFileRecord.EventUpdatedText.ToString(), 'A', 60));
        curRecord.Append(Environment.NewLine);
        return curRecord.ToString();
    }

    private string GenerateFlatFileField(string eventContent, char eventType, int eventLength)
    {
        eventContent = eventContent.Replace("@@", "");
        //eventContent = eventContent.Trim();
        //if (eventContent.Length > eventLength) eventContent.Substring(0, eventLength);
        //if (eventContent.Length < eventLength) eventContent.PadLeft(eventLength);
        return eventContent;
    }

    public String CreateHOSCompany(int organizationId)
    {
        string ppcidSeed = "";
        SqlConnection connection;
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_CreateHOSCompany";
                sqlCmd.Connection = connection;
                SqlParameter sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
                sqlPara.Value = organizationId;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@PPCIDSeed", SqlDbType.VarChar, 100);
                sqlPara.Direction = ParameterDirection.Output;
                sqlCmd.Parameters.Add(sqlPara);
                connection.Open();
                sqlCmd.ExecuteNonQuery();
                ppcidSeed = sqlCmd.Parameters["@PPCIDSeed"].Value.ToString();
            }
        }
        return ppcidSeed;
    }
    public void UpdateVehicleAssignment(int boxId, String ppcid, int sentinelOrganizationId)
    { 
        SqlConnection connection;
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_UpdateVehicleAssignment";
                sqlCmd.Connection = connection;
                SqlParameter sqlPara = new SqlParameter("@SentinelVehicleId", SqlDbType.Int);
                sqlPara.Value = boxId;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@HOSVehicleId", SqlDbType.VarChar);
                sqlPara.Value = ppcid;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@SentinelOrganizationId", SqlDbType.Int);
                sqlPara.Value = sentinelOrganizationId;
                sqlCmd.Parameters.Add(sqlPara);
                connection.Open();
                sqlCmd.ExecuteNonQuery();
            }
        }
    }

    public int AddOrUpdateLogData_InspectionGroup(int groupId, string name, int type, int organizationId)
    {
        SqlConnection connection;
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_AddOrUpdateLogData_InspectionGroup";
                SqlParameter sqlPara = new SqlParameter("@GroupId", SqlDbType.Int);
                sqlPara.Direction = ParameterDirection.InputOutput;
                sqlPara.Value = groupId;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@Name", SqlDbType.VarChar);
                sqlPara.Value = name;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@Type", SqlDbType.TinyInt);
                sqlPara.Value = type;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
                sqlPara.Value = organizationId;
                sqlCmd.Parameters.Add(sqlPara);
                connection.Open();
                sqlCmd.ExecuteNonQuery();
                if (groupId == -1)
                {
                    groupId = Int32.Parse(sqlCmd.Parameters["@GroupId"].Value.ToString());
                }
            }
        }
        return groupId;
    }

    public void AddLogdata_SMCSCode(int defectLevel, string SMCSCode, int organizationId)
    {
        SqlConnection connection;
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_AddLogdata_SMCSCode";                
                SqlParameter sqlPara = new SqlParameter("@defectLevel", SqlDbType.Int);
                sqlPara.Value = defectLevel;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@SMCSCode", SqlDbType.VarChar);
                sqlPara.Value = SMCSCode;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
                sqlPara.Value = organizationId;
                sqlCmd.Parameters.Add(sqlPara);
                connection.Open();
                sqlCmd.ExecuteNonQuery();
            }
        }
    }

    public void AddOrUpdateLogdata_Question(int rowID, string defect, int defectLevel, string SMCSCode, int organizationId)
    {
        SqlConnection connection;
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_AddOrUpdateLogdata_Question";
                SqlParameter sqlPara = new SqlParameter("@rowID", SqlDbType.Int);
                sqlPara.Value = rowID;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@defect", SqlDbType.VarChar);
                sqlPara.Value = defect;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@defectLevel", SqlDbType.Int);
                sqlPara.Value = defectLevel;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@SMCSCode", SqlDbType.VarChar);
                sqlPara.Value = SMCSCode;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
                sqlPara.Value = organizationId;
                sqlCmd.Parameters.Add(sqlPara);
                connection.Open();
                sqlCmd.ExecuteNonQuery();
            }
        }
    }
    public void DeleteLogdata_Question(int rowID, int organizationId)
    {
        SqlConnection connection;
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_DeleteLogdata_Question";
                SqlParameter sqlPara = new SqlParameter("@rowID", SqlDbType.Int);
                sqlPara.Value = rowID;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
                sqlPara.Value = organizationId;
                sqlCmd.Parameters.Add(sqlPara);
                connection.Open();
                sqlCmd.ExecuteNonQuery();
            }
        }
    }
    public DataTable GetLogdata_Question(int organizationId)
    {
        DataTable dataSet = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetLogdata_Question";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            adapter.Fill(dataSet);
        }
        return dataSet;
    }
    public DataTable GetLogData_InspectionGroup(int organizationId)
    {
        DataTable dataSet = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetLogData_InspectionGroup";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            adapter.Fill(dataSet);
        }
        return dataSet;
    }
    public Dictionary<string, string> AddOrUpdateLogData_InspectionGroupItem(int groupId, int categoryid, int inspectionitemId, int parentItemId, int questionID, Boolean isCategory, int organizationId, string path,
        Boolean scannable, string location)
    {
        SqlConnection connection;
        int ret = -1;
        Dictionary<string, string> rettbl = new Dictionary<string, string>();
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_AddOrUpdateLogData_InspectionGroupItem";
                SqlParameter sqlPara = new SqlParameter("@GroupId", SqlDbType.Int);
                sqlPara.Value = groupId;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@CategoryId", SqlDbType.Int);
                sqlPara.Value = categoryid;
                sqlPara.Direction = ParameterDirection.InputOutput;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@InspectionitemId", SqlDbType.Int);
                sqlPara.Value = inspectionitemId;
                sqlPara.Direction = ParameterDirection.InputOutput;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@ParentItemId", SqlDbType.Int);
                if (parentItemId > 0)
                    sqlPara.Value = parentItemId;
                else sqlPara.Value = DBNull.Value;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@QuestionId", SqlDbType.Int);
                sqlPara.Value = questionID;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@Path", SqlDbType.VarChar);
                sqlPara.Value = path;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@IsCategory", SqlDbType.Bit );
                sqlPara.Value = isCategory;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
                sqlPara.Value = organizationId;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@Scannable", SqlDbType.Bit);
                sqlPara.Value = scannable;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@Location", SqlDbType.VarChar);
                sqlPara.Value = (scannable == true)?location.Trim():"";
                sqlCmd.Parameters.Add(sqlPara);
                connection.Open();
                SqlDataReader sdr = sqlCmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        rettbl.Add("Defect", sdr.GetString(0));
                        rettbl.Add("SMCS", sdr.GetString(1));
                        rettbl.Add("DefectLevel", sdr.GetString(2));
                        break;
                    }
                }
                sdr.Close();
                if (rettbl.Count > 0)
                {
                    if (isCategory)
                    {
                        if (categoryid == -1)
                            ret = Int32.Parse(sqlCmd.Parameters["@CategoryId"].Value.ToString());
                        else
                            ret = categoryid;
                    }
                    else
                    {
                        if (inspectionitemId == -1) ret = Int32.Parse(sqlCmd.Parameters["@InspectionitemId"].Value.ToString());
                        else ret = inspectionitemId;
                    }
                    rettbl.Add("Id", ret.ToString());
                }
            }
        }
        return rettbl;
    }
    public void DeleteLogData_InspectionGroupItem(int groupId, int categoryid, int inspectionitemId, Boolean isCategory, int organizationId)
    {
        SqlConnection connection;
        Dictionary<string, string> rettbl = new Dictionary<string, string>();
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_DeleteLogData_InspectionGroupItem";
                SqlParameter sqlPara = new SqlParameter("@GroupId", SqlDbType.Int);
                sqlPara.Value = groupId;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@CategoryId", SqlDbType.Int);
                sqlPara.Value = categoryid;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@InspectionitemId", SqlDbType.Int);
                sqlPara.Value = inspectionitemId;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@IsCategory", SqlDbType.Bit);
                sqlPara.Value = isCategory;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
                sqlPara.Value = organizationId;
                sqlCmd.Parameters.Add(sqlPara);
                connection.Open();
                sqlCmd.ExecuteNonQuery();
            }
        }
    }
    public void DeleteLogData_InspectionGroup(int groupId, int organizationId)
    {
        SqlConnection connection;
        Dictionary<string, string> rettbl = new Dictionary<string, string>();
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_DeleteLogData_InspectionGroup";
                SqlParameter sqlPara = new SqlParameter("@GroupId", SqlDbType.Int);
                sqlPara.Value = groupId;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
                sqlPara.Value = organizationId;
                sqlCmd.Parameters.Add(sqlPara);
                connection.Open();
                sqlCmd.ExecuteNonQuery();
            }
        }
    }
    public DataSet GetLogData_InspectionGroupItem(int groupId, int organizationId)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetLogData_InspectionGroupItem";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@GroupId", SqlDbType.Int);
            sqlPara.Value = groupId;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            adapter.Fill(dataSet);
        }
        return dataSet;
    }
    public void AddOrUpdateLogData_InspectionGroupAssignment(int groupId, int boxId, string nodeCode, int organizationId, bool isAdd, string DOTNbr, int dotType, int fleetId)
    {
        SqlConnection connection;
        Dictionary<string, string> rettbl = new Dictionary<string, string>();
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_AddOrUpdateLogData_InspectionGroupAssignment";
                SqlParameter sqlPara = new SqlParameter("@GroupId", SqlDbType.Int);
                sqlPara.Value = groupId;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
                sqlPara.Value = organizationId;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@BoxId", SqlDbType.Int);
                sqlPara.Value = boxId;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@NodeCode", SqlDbType.VarChar);
                sqlPara.Value = nodeCode;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@isAdd", SqlDbType.Bit);
                sqlPara.Value = isAdd;
                sqlCmd.Parameters.Add(sqlPara);
                if (!String.IsNullOrEmpty(DOTNbr) && DOTNbr.Trim().Length > 0)
                {
                    sqlPara = new SqlParameter("@DOTNbr", SqlDbType.VarChar);
                    sqlPara.Value = DOTNbr;
                    sqlCmd.Parameters.Add(sqlPara);
                    sqlPara = new SqlParameter("@DotType", SqlDbType.Int);
                    sqlPara.Value = dotType;
                    sqlCmd.Parameters.Add(sqlPara);
                }
                sqlPara = new SqlParameter("@SelectedFleetId", SqlDbType.Int);
                sqlPara.Value = fleetId;
                sqlCmd.Parameters.Add(sqlPara);
                connection.Open();
                sqlCmd.ExecuteNonQuery();
            }
        }
    }
    public DataTable GetInspectionGroupByHierarchy(int organizationId, string nodeCode, int boxId, string DOTNbr, int dotType, int fleetId)
    {
        DataTable dataSet = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetInspectionGroupByHierarchy";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            if (!String.IsNullOrEmpty(nodeCode) && nodeCode.Length > 0)
            {
                sqlPara = new SqlParameter("@NodeCode", SqlDbType.VarChar);
                sqlPara.Value = nodeCode;
                adapter.SelectCommand.Parameters.Add(sqlPara);
            }
            if (boxId > 0)
            {
                sqlPara = new SqlParameter("@BoxId", SqlDbType.Int );
                sqlPara.Value = boxId;
                adapter.SelectCommand.Parameters.Add(sqlPara);
            }
            if (!String.IsNullOrEmpty(DOTNbr) && DOTNbr.Trim().Length > 0)
            {
                sqlPara = new SqlParameter("@DOTNbr", SqlDbType.VarChar);
                sqlPara.Value = DOTNbr;
                adapter.SelectCommand.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@DotType", SqlDbType.Int);
                sqlPara.Value = dotType;
                adapter.SelectCommand.Parameters.Add(sqlPara);
            }
            if (fleetId > 0)
            {
                sqlPara = new SqlParameter("@FleetId", SqlDbType.Int);
                sqlPara.Value = fleetId;
                adapter.SelectCommand.Parameters.Add(sqlPara);
            }
            adapter.Fill(dataSet);
        }
        return dataSet;
    }
    public DataTable GetSMCSByOrganizationId(int organizationId)
    {
        DataTable dataSet = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetSMCSByOrganizationId";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            adapter.Fill(dataSet);
        }
        return dataSet;
    }
    public DataTable GetOrganizationDOT(int organizationId)
    {
        DataTable dataSet = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(sentinelFMConnection))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_GetOrganizationDOT";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            adapter.Fill(dataSet);
        }
        return dataSet;
    }
    public DataSet GetVehiclesInfoByDOTByPage(int organizationId, string DOTNbr, int PageSize, int Page, string Filter)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(sentinelFMConnection))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "GetVehiclesInfoByDOTByPage";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            sqlPara = new SqlParameter("@DOTNbr", SqlDbType.VarChar);
            sqlPara.Value = DOTNbr;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            sqlPara = new SqlParameter("@PageSize", SqlDbType.Int);
            sqlPara.Value = PageSize;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            sqlPara = new SqlParameter("@Page", SqlDbType.Int);
            sqlPara.Value = Page;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            sqlPara = new SqlParameter("@Filter", SqlDbType.VarChar);
            sqlPara.Value = Filter;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            adapter.Fill(dataSet);
        }
        return dataSet;
    }
    public int GetVehiclesInfoTotalNumberByDOT(int organizationId, string DOTNbr, string Filter)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(sentinelFMConnection))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "GetVehiclesInfoTotalNumberByDOT";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            sqlPara = new SqlParameter("@DOTNbr", SqlDbType.VarChar);
            sqlPara.Value = DOTNbr;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            sqlPara = new SqlParameter("@Filter", SqlDbType.VarChar);
            sqlPara.Value = Filter;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            adapter.Fill(dataSet);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                return (int)dataSet.Tables[0].Rows[0][0];
        }
        return 0;
    }
    public DataTable GetInspectionDefects(Int64 refId, Int64 rowId)
    {
        DataTable dt = new DataTable();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Connection = connection;
            adapter.SelectCommand.CommandText = "GetInspectionDefects";
            adapter.SelectCommand.Parameters.Clear();
            SqlParameter param = new SqlParameter("@refid", SqlDbType.Int);
            param.Value = refId;
            adapter.SelectCommand.Parameters.Add(param);
            param = new SqlParameter("@rowid", SqlDbType.Int);
            param.Value = rowId;
            adapter.SelectCommand.Parameters.Add(param);
            adapter.Fill(dt);
        }
        return dt;
    }
    public int GetCompanyConfigurationMobile(int organizationId, int configurationId)
    {
        DataTable dt = new DataTable();
        int ret = 0;
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Connection = connection;
            adapter.SelectCommand.CommandText = "usp_hos_GetCompanyConfigurationMobile";
            adapter.SelectCommand.Parameters.Clear();
            SqlParameter param = new SqlParameter("@OrganizationId", SqlDbType.Int);
            param.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(param);
            param = new SqlParameter("@ConfigurationId", SqlDbType.Int);
            param.Value = configurationId;
            adapter.SelectCommand.Parameters.Add(param);
            adapter.Fill(dt);
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["Value"] != DBNull.Value)
                int.TryParse(dt.Rows[0]["Value"].ToString(), out ret);
        }
        if (ret == 0) ret = 3;
        return ret;
    }
    public DataSet GetInspectionCategory(int boxId)
    {
        DataSet dt = new DataSet();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Connection = connection;
            adapter.SelectCommand.CommandText = "usp_hos_GetInspectionCategory";
            adapter.SelectCommand.Parameters.Clear();
            SqlParameter param = new SqlParameter("@BoxId", SqlDbType.Int);
            param.Value = boxId;
            adapter.SelectCommand.Parameters.Add(param);
            adapter.Fill(dt);
        }
        return dt;
    }
    public DataTable GetInspectionItem(int boxId)
    {
        DataTable dt = new DataTable();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Connection = connection;
            adapter.SelectCommand.CommandText = "usp_hos_GetInspectionItem";
            adapter.SelectCommand.Parameters.Clear();
            SqlParameter param = new SqlParameter("@BoxId", SqlDbType.Int);
            param.Value = boxId;
            adapter.SelectCommand.Parameters.Add(param);
            adapter.Fill(dt);
        }
        return dt;
    }
    public void AddOrUpdateBarcode(int boxId, string barcodes)
    {
        SqlConnection connection;
        Dictionary<string, string> rettbl = new Dictionary<string, string>();
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_AddOrUpdateBarcode";
                SqlParameter sqlPara = new SqlParameter("@BoxID", SqlDbType.Int);
                sqlPara.Value = boxId;
                sqlCmd.Parameters.Add(sqlPara);
                sqlPara = new SqlParameter("@BarCodes", SqlDbType.VarChar);
                sqlPara.Value = barcodes;
                sqlCmd.Parameters.Add(sqlPara);
                connection.Open();
                sqlCmd.ExecuteNonQuery();
            }
        }
    }
    public DataTable GetOrganizationVehicleType(int organizationId)
    {
        DataTable dataSet = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(sentinelFMConnection))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_GetOrganizationVehicleType";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            adapter.Fill(dataSet);
        }
        return dataSet;
    }
    public DataSet GetVehiclesInfoByTypeByPage(int organizationId, string ObjectType, int PageSize, int Page, string Filter)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(sentinelFMConnection))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "GetVehiclesInfoByTypeByPage";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            sqlPara = new SqlParameter("@ObjectType", SqlDbType.VarChar);
            sqlPara.Value = ObjectType;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            sqlPara = new SqlParameter("@PageSize", SqlDbType.Int);
            sqlPara.Value = PageSize;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            sqlPara = new SqlParameter("@Page", SqlDbType.Int);
            sqlPara.Value = Page;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            sqlPara = new SqlParameter("@Filter", SqlDbType.VarChar);
            sqlPara.Value = Filter;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            adapter.Fill(dataSet);
        }
        return dataSet;
    }
    public int GetVehiclesInfoTotalNumberByType(int organizationId, string ObjectType, string Filter)
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(sentinelFMConnection))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "GetVehiclesInfoTotalNumberByType";
            adapter.SelectCommand.Connection = connection;
            SqlParameter sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            sqlPara = new SqlParameter("@ObjectType", SqlDbType.VarChar);
            sqlPara.Value = ObjectType;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            sqlPara = new SqlParameter("@Filter", SqlDbType.VarChar);
            sqlPara.Value = Filter;
            adapter.SelectCommand.Parameters.Add(sqlPara);
            adapter.Fill(dataSet);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                return (int)dataSet.Tables[0].Rows[0][0];
        }
        return 0;
    }

    public void AddOrUpdateLogData_CycleAssignment(int RuleId, int RuleType, int BoxId, String NodeCode, int OrganizationId, Boolean isAdd, String DOTNbr, int dotType)
    {
        SqlConnection connection;
        Dictionary<string, string> rettbl = new Dictionary<string, string>();
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_AddOrUpdateLogData_CycleAssignment";

                SqlParameter sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
                sqlPara.Value = OrganizationId;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@RuleId", SqlDbType.SmallInt);
                sqlPara.Value = RuleId;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@RuleType", SqlDbType.SmallInt);
                sqlPara.Value = RuleType;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@BoxId", SqlDbType.Int);
                sqlPara.Value = BoxId;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@NodeCode", SqlDbType.VarChar);
                sqlPara.Value = NodeCode;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@isAdd", SqlDbType.Bit);
                sqlPara.Value = isAdd;
                sqlCmd.Parameters.Add(sqlPara);

                if (!String.IsNullOrEmpty(DOTNbr) && DOTNbr.Trim().Length > 0)
                {
                    sqlPara = new SqlParameter("@DOTNbr", SqlDbType.VarChar);
                    sqlPara.Value = DOTNbr;
                    sqlCmd.Parameters.Add(sqlPara);

                    sqlPara = new SqlParameter("@DotType", SqlDbType.Int);
                    sqlPara.Value = dotType;
                    sqlCmd.Parameters.Add(sqlPara);

                }

                connection.Open();
                sqlCmd.ExecuteNonQuery();
            }
        }
    }

    public DataTable GellAllCyclesandExceptions(int? type)
    {
        DataTable dataSet = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GellAllCyclesandExceptions";
            adapter.SelectCommand.Connection = connection;

            if (type.HasValue)
            {
                SqlParameter sqlPara = new SqlParameter("@Type", SqlDbType.SmallInt);
                sqlPara.Value = type;
                adapter.SelectCommand.Parameters.Add(sqlPara);
            }

            adapter.Fill(dataSet);
        }
        return dataSet;
    }

    public DataTable GetCycleAndExceptionByHierarchy(int organizationId, string nodeCode, int boxId, string DOTNbr, int dotType)
    {
        DataTable dataSet = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetCycleAndExceptionByHierarchy";
            adapter.SelectCommand.Connection = connection;

            SqlParameter sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            if (!String.IsNullOrEmpty(nodeCode) && nodeCode.Length > 0)
            {
                sqlPara = new SqlParameter("@NodeCode", SqlDbType.VarChar);
                sqlPara.Value = nodeCode;
                adapter.SelectCommand.Parameters.Add(sqlPara);
            }

            if (boxId > 0)
            {
                sqlPara = new SqlParameter("@BoxId", SqlDbType.Int );
                sqlPara.Value = boxId;
                adapter.SelectCommand.Parameters.Add(sqlPara);
            }

            if (!String.IsNullOrEmpty(DOTNbr) && DOTNbr.Trim().Length > 0)
            {
                sqlPara = new SqlParameter("@DOTNbr", SqlDbType.VarChar);
                sqlPara.Value = DOTNbr;
                adapter.SelectCommand.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@DotType", SqlDbType.Int);
                sqlPara.Value = dotType;
                adapter.SelectCommand.Parameters.Add(sqlPara);

            }

            adapter.Fill(dataSet);
        }
        return dataSet;

    }

    public DataTable GetCompanyConfigurationMobileByOrgId(int organizationId)
    {
        DataTable dataSet = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetCompanyConfigurationMobileByOrgId";
            adapter.SelectCommand.Connection = connection;

            SqlParameter sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            adapter.Fill(dataSet);
        }
        return dataSet;

    }


    public DataTable GetOrganizationInspections(int organizationId, DateTime start, DateTime stop, String driverId)
    {
        DataTable dataSet = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();
   
        using (SqlConnection connection = new SqlConnection(hosConnectionString))
        {
            adapter.SelectCommand = new SqlCommand();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.CommandText = "usp_hos_GetOrganizationInspections";
            adapter.SelectCommand.Connection = connection;
            adapter.SelectCommand.CommandTimeout = 60;
            SqlParameter sqlPara = new SqlParameter("@organization", SqlDbType.Int);
            sqlPara.Value = organizationId;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@start", SqlDbType.DateTime);
            sqlPara.Value = start;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            sqlPara = new SqlParameter("@stop", SqlDbType.DateTime);
            sqlPara.Value = stop;
            adapter.SelectCommand.Parameters.Add(sqlPara);

            if (!String.IsNullOrEmpty(driverId))
            {
                sqlPara = new SqlParameter("driverId", SqlDbType.VarChar);
                sqlPara.Value = driverId;
                adapter.SelectCommand.Parameters.Add(sqlPara);
            }
            adapter.Fill(dataSet);
        }
        return dataSet;

    }

    public void AddorUpdateCompanyConfigurationMobile(int organizationId, int configurationId, String value)
    {
        SqlConnection connection;
        Dictionary<string, string> rettbl = new Dictionary<string, string>();
        using (connection = new SqlConnection(hosConnectionString))
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "usp_hos_AddorUpdateCompanyConfigurationMobile";
                SqlParameter sqlPara = new SqlParameter("@OrganizationId", SqlDbType.Int);
                sqlPara.Value = organizationId;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@ConfigurationId", SqlDbType.Int);
                sqlPara.Value = configurationId;
                sqlCmd.Parameters.Add(sqlPara);

                sqlPara = new SqlParameter("@Value", SqlDbType.VarChar);
                sqlPara.Value = value;
                sqlCmd.Parameters.Add(sqlPara);

                connection.Open();
                sqlCmd.ExecuteNonQuery();
            }
        }
    }
}


public class FlatFileData
{
    public string DriverFirstName = string.Empty;
    public string DriverLastName = string.Empty;
    public string DriverPin = string.Empty;
    public string Tractor = string.Empty;
    public string Trail = string.Empty;
    public string Vin = string.Empty;
    public string CoDriverFirstName = string.Empty;
    public string CoDriverLastName = string.Empty;
    public string CoDriverID = string.Empty;
    public int USDOT = 0;
    public string CarrierName = string.Empty;
    public string ShippingDocument = string.Empty;
    public int EventSequenceID = -1;
    public string EventStatusCode = string.Empty;
    public int EventDate = 0;
    public int EventTime = 0;
    public float EventLatitude = 0;
    public float EventLongitude = 0;
    public string EventPlace = string.Empty;
    public int PlaceDistance = 0;
    public int PlaceTotalDistance = 0;
    public string EventUpdateStatusCode = string.Empty;
    public string DiagnosticEventCode = string.Empty;
    public string EventErrorCode = string.Empty;
    public int EventUpdatedDate = 0;
    public int EventUpdatedTime = 0;
    public string EventUpdatedPersonId = string.Empty;
    public string EventUpdatedText = string.Empty;
    public DateTime EventDateTime;

}
