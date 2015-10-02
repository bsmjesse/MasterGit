using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using System.Collections;	// for ArrayList
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace SentinelHoSIntegration
{
    enum HOSStatus
    {
        NEW,
        HIST,
        DRIVERHIST,
    }

    public class HoSDataIntegration
    {
        string hosDBConnectionStr = ConfigurationManager.ConnectionStrings["HOSConnectionString"].ConnectionString;//System.Web.Configuration.WebConfigurationManager.ConnectionStrings["HOSConnectionString"].ConnectionString;
        SqlConnection hosSqlConnection;
        SqlCommand hosSqlCommand;
        SqlDataAdapter hosSqlDataAdapter = new SqlDataAdapter();
        DataSet hosDataSet = new DataSet();

        LogDateInfoData[] _LogDateInfoData = null;

        bool MoreData = false;

        public HoSDataIntegration()
        {
            hosSqlConnection = new SqlConnection(hosDBConnectionStr);
            hosSqlCommand = hosSqlConnection.CreateCommand();
        }       

        #region Singleton Functionality 
        //private static HoSDataIntegration instance;
        //static object _SYNC = new object();
        /// <summary>
        /// Returns instance of HoSDataIntegration (Singleton Pattern)
        /// </summary>
        /// <returns></returns>
        //public static HoSDataIntegration GetInstance()
        //{
            //if (instance == null)
            //{
                //lock (_SYNC)
                //{
                    //instance = new HoSDataIntegration();
                //}
            //}
            //return instance;
        //}
        #endregion Singleton Functionality 

        public string GetHOSInegrationNewLogData(int userId)
        {
            return GetHOSInegrationLogData(userId, HOSStatus.NEW, null, null, null);
        }

        public string GetHOSInegrationHistLogData(int userId, DateTime iDateTime)//dtFrom, DateTime dtTo)
        {
            return GetHOSInegrationLogData(userId, HOSStatus.HIST, null, iDateTime, iDateTime);
        }

        public string GetHOSInegrationDriverHistLogData(int userId, int driverId, DateTime dtFrom, DateTime dtTo)
        {
            return GetHOSInegrationLogData(userId, HOSStatus.DRIVERHIST, driverId, dtFrom, dtTo);
        }

        string GetSQL(int userId, HOSStatus iHOSStatus, int? driverId, DateTime? dtFrom, DateTime? dtTo)
        {
            string sql = "GetHOSInegrationLogData ";
            string DID = "NULL";
            string DTF = "NULL";
            string DTT = "NULL";

            if (userId > 0)
            {
                if (iHOSStatus == HOSStatus.NEW)
                {
                    sql = string.Format("{0}{1},'{2}',{3},{4},{5}", sql, userId, iHOSStatus, DID, DTF, DTT);
                }
                else
                {
                    DTF = (dtFrom != null ? dtFrom.Value.ToString("yyyy-MM-dd") : DTF);
                    DTT = (dtTo != null ? dtTo.Value.ToString("yyyy-MM-dd") : DTT);

                    if (iHOSStatus == HOSStatus.HIST)
                    {
                        sql = string.Format("{0}{1},'{2}',{3},'{4}','{5}'", sql, userId, iHOSStatus, DID, DTF, DTT);
                    }
                    else //if (iHOSStatus == HOSStatus.DRIVERHIST)
                    {
                        DID = driverId.ToString();
                        sql = string.Format("{0}{1},'{2}',{3},'{4}','{5}'", sql, userId, iHOSStatus, DID, DTF, DTT);
                    }
                }
            }
            else
            {
                sql = string.Format("{0}{1}", sql, userId);
            }

            return sql;
        }

        string GetHOSInegrationLogData(int userId, HOSStatus iHOSStatus, int? driverId, DateTime? dtFrom, DateTime? dtTo)
        {
            string prefixMsg = string.Empty;
            string _xml = string.Empty;
            try
            {

                prefixMsg = string.Format("Unable to retrieve User {0} information.", userId);
                //Prepares SQL statement
                string sql = GetSQL(userId, iHOSStatus, driverId, dtFrom, dtTo);
                hosSqlCommand.CommandText = sql;
                hosSqlDataAdapter.SelectCommand = hosSqlCommand;
                hosSqlConnection.Open();

                hosSqlDataAdapter.TableMappings.Add("Table", "HOSIntegaration");
                hosSqlDataAdapter.TableMappings.Add("Table1", "HOSDutyStatusInfo");
                hosSqlDataAdapter.TableMappings.Add("Table2", "HOSTractorList");

                //Executes SQL statement
                hosSqlDataAdapter.Fill(hosDataSet);
                hosSqlDataAdapter.Dispose();
                hosSqlCommand.Dispose();
                hosSqlConnection.Close();

                _LogDateInfoData = new LogDateInfoData[hosDataSet.Tables[0].Rows.Count];
                // >>>>>>>
                if (hosDataSet == null || hosDataSet.Tables.Count < 1 && hosDataSet.Tables[0].Rows.Count < 1) return null;
                int itr = 0;
                DataTable myDataTable = hosDataSet.Tables[0].Copy();

                //System.Windows.Forms.MessageBox.Show("\nReading from the " + myDataTable.TableName + " DataTable that has " + myDataTable.Rows.Count + " ROWS.");
                foreach (DataRow myDataRow in myDataTable.Rows)
                {
                    //CompanyID,eLogId,DriverID,DriverName,CompanyName,CompanyAddress,
                    //DistanceUnit,DriverCycle,DriverRegion,LogDate,Documents,
                    //CoDriverIDs,CoDrivers,EditCount,SensorFailureCount,OffDuty,SleeperBerth,Driving,
                    //OnDutyNotDriving,OnDutyToday,OnDutyThisWeek,DistanceDriven,TimeZone,
                    //Trailers,DutyStatusInfoCount,TractorCount--,HasSend,SendDate,Status
                    _LogDateInfoData[itr] = new LogDateInfoData();
                    _LogDateInfoData[itr].CompanyID = Convert.ToInt32(myDataRow[0]);
                    _LogDateInfoData[itr].eLogID = Convert.ToInt32(myDataRow[1]);
                    _LogDateInfoData[itr].DriverID = myDataRow[2].ToString();
                    _LogDateInfoData[itr].DriverName = myDataRow[3].ToString();
                    _LogDateInfoData[itr].CompanyName = myDataRow[4].ToString();
                    _LogDateInfoData[itr].CompanyAddress = myDataRow[5].ToString();

                    _LogDateInfoData[itr].DistanceUnit = myDataRow[6].ToString();
                    _LogDateInfoData[itr].DriverCycle = myDataRow[7].ToString();
                    _LogDateInfoData[itr].DriverRegion = myDataRow[8].ToString();
                    _LogDateInfoData[itr].logDate = Convert.ToDateTime(myDataRow[9]);
                    _LogDateInfoData[itr].Documents = myDataRow[10].ToString();

                    _LogDateInfoData[itr].CoDriverIDs = myDataRow[11].ToString();
                    _LogDateInfoData[itr].CoDrivers = myDataRow[12].ToString();
                    _LogDateInfoData[itr].EditCount = Convert.ToInt32(myDataRow[13]);
                    _LogDateInfoData[itr].SensorFailureCount = Convert.ToInt32(myDataRow[14]);
                    _LogDateInfoData[itr].OffDuty = Convert.ToInt32(myDataRow[15]);
                    _LogDateInfoData[itr].SleeperBerth = Convert.ToInt32(myDataRow[16]);
                    _LogDateInfoData[itr].Driving = Convert.ToInt32(myDataRow[17]);

                    _LogDateInfoData[itr].OnDutyNotDriving = Convert.ToInt32(myDataRow[18]);
                    _LogDateInfoData[itr].OnDutyToday = Convert.ToInt32(myDataRow[19]);
                    _LogDateInfoData[itr].OnDutyThisWeek = Convert.ToInt32(myDataRow[20]);
                    _LogDateInfoData[itr].DistanceDriven = Convert.ToInt32(myDataRow[21]);
                    _LogDateInfoData[itr].TimeZone = myDataRow[22].ToString();

                    _LogDateInfoData[itr].Trailers = myDataRow[23].ToString();
                    _LogDateInfoData[itr].DutyStatusInfoCount = Convert.ToInt32(myDataRow[24]);
                    _LogDateInfoData[itr].TractorCount = Convert.ToInt32(myDataRow[25]);

                    _LogDateInfoData[itr].DutyStatusInfoList = GetDutyStatusInfoList(hosDataSet.Tables[1].Select(string.Format("eLogId={0}",_LogDateInfoData[itr].eLogID)).CopyToDataTable());
                    if (IsExist(hosDataSet.Tables[2].Select("eLogId=" + _LogDateInfoData[itr].eLogID)))// check if duty ON/OFF
                        _LogDateInfoData[itr].TractorList = GetTractorInfoList(hosDataSet.Tables[2].Select(string.Format("eLogId={0}",_LogDateInfoData[itr].eLogID)).CopyToDataTable());
                    
                    itr++;
                }

                return SerializeObjectToXML(_LogDateInfoData);
            }

            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        bool IsExist(DataRow[] dr)
        {
            return dr.Length > 0;
        }

        List<DutyStatusInfo> GetDutyStatusInfoList(DataTable iDataTable)
        {
	  DutyStatusInfo objDSI;            
	  List<DutyStatusInfo> _DutyStatusInfo = new List<DutyStatusInfo>();

            //System.Windows.Forms.MessageBox.Show("\nReading from the " + iDataTable.TableName + " DataTable that has " + iDataTable.Rows.Count + " ROWS.");

            foreach (DataRow myDataRow in iDataTable.Rows)
            {
                //StartTime,Duration,CurrentActivity,Location,EditFlag,Confirmed,SensorFailure,DistanceEdit,Comment
		objDSI = new DutyStatusInfo();
                objDSI.StartTime = Convert.ToDateTime(myDataRow[1]);
                objDSI.Duration = Convert.ToInt32(myDataRow[2]);
                objDSI.CurrentActivity = myDataRow[3].ToString();
                objDSI.Location = myDataRow[4].ToString();
                objDSI.EditFlag = Convert.ToBoolean(myDataRow[5]);
                objDSI.Confirmed = Convert.ToBoolean(myDataRow[6]);
                objDSI.SensorFailure = Convert.ToBoolean(myDataRow[7]);
                objDSI.DistanceEdit = Convert.ToBoolean(myDataRow[8]);
                objDSI.Comments = myDataRow[9].ToString();

                _DutyStatusInfo.Add(objDSI);
            }

            return _DutyStatusInfo;
        }

        List<TractorInfo> GetTractorInfoList(DataTable iDataTable)
        {
            TractorInfo objTI;
            List<TractorInfo> _TractorInfo = new List<TractorInfo>();

            //System.Windows.Forms.MessageBox.Show("\nReading from the " + iDataTable.TableName + " DataTable that has " + iDataTable.Rows.Count + " ROWS.");

            foreach (DataRow myDataRow in iDataTable.Rows)
            {
                //TractorID,LicensePlate,OdometerStart,OdometerEnd
		objTI = new TractorInfo();
                objTI.TractorID = myDataRow[1].ToString();
                objTI.LicensePlate = myDataRow[2].ToString();
                objTI.OdometerStart = Convert.ToInt32(myDataRow[3]);
                objTI.OdometerEnd = Convert.ToInt32(myDataRow[4]);

                _TractorInfo.Add(objTI);
            }

            return _TractorInfo;
        }

        string SerializeObjectToXML(object item)
        {
            try
            {
                string xmlText;
                //Get the type of the object
                Type objectType = item.GetType();
                //create serializer object based on the object type
                XmlSerializer xmlSerializer = new XmlSerializer(objectType);
                //Create a memory stream handle the data
                MemoryStream memoryStream = new MemoryStream();
                //Create an XML Text writer to serialize data to
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, System.Text.Encoding.UTF8) { Formatting = Formatting.Indented })
                {
                    //convert the object to xml data
                    xmlSerializer.Serialize(xmlTextWriter, item);
                    //Get reference to memory stream
                    memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                    //Convert memory byte array into xml text
                    xmlText = new System.Text.UTF8Encoding().GetString(memoryStream.ToArray());
                    //clean up memory stream
                    memoryStream.Dispose();
                    return xmlText.Replace("ArrayOfLogDateInfoData", "LogDateInfoDataList");
                    //return xmlText.Replace("ArrayOfLogDateInfoData", "LogDateInfoDataList").Replace("</LogDateInfoDataList>", string.Format("<MoreData> {0} </MoreData></LogDateInfoDataList>",MoreData));
                }
            }
            catch (Exception ex)
            {
                //There are a number of reasons why this function may fail usually because some of the data on the class cannot be serialized.
                System.Diagnostics.Debug.Write(ex.ToString());
                return null;
            }
        }
    }

    /// <summary>
    /// Summary description for HoSInegration
    /// </summary>
    public class LogDateInfoData
    {
        public int CompanyID { get; set; }
        public int eLogID { get; set; }
        public string DriverID { get; set; }
        public string DriverName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string DistanceUnit { get; set; }
        public string DriverCycle { get; set; }
        public string DriverRegion { get; set; }
        public DateTime logDate { get; set; }
        public string Documents { get; set; }
        public string CoDriverIDs { get; set; }
        public string CoDrivers { get; set; }
        public int EditCount { get; set; }
        public int SensorFailureCount { get; set; }
        public int OffDuty { get; set; }
        public int SleeperBerth { get; set; }
        public int Driving { get; set; }
        public int OnDutyNotDriving { get; set; }
        public int OnDutyToday { get; set; }
        public int OnDutyThisWeek { get; set; }
        public int DistanceDriven { get; set; }
        public string TimeZone { get; set; }
        public string Trailers { get; set; }
        public int DutyStatusInfoCount { get; set; }
        public List<DutyStatusInfo> DutyStatusInfoList { get; set; }
        public int TractorCount { get; set; }
        public List<TractorInfo> TractorList { get; set; }
    }

    public class DutyStatusInfo
    {
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string CurrentActivity { get; set; }
        public string Location { get; set; }
        public bool EditFlag { get; set; }
        public bool Confirmed { get; set; }
        public bool SensorFailure { get; set; }
        public bool DistanceEdit { get; set; }
        public string Comments { get; set; }
    }

    public class TractorInfo
    {
        public string TractorID { get; set; }
        public string LicensePlate { get; set; }
        public int OdometerStart { get; set; }
        public int OdometerEnd { get; set; }
    }
}