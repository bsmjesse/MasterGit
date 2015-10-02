using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Data;

namespace SentinelFM
{
    public partial class Widgets_VideoViewer : SentinelFMBasePage
    {
        public string VedioList = "";
        public bool ShowHTML = true;

        private string ftpServer = "camera.sentinelfm.com";
        private string ftpUserName = "camera_web";
        private string ftpPassword = "BSMwire1";

        private int BoxId = 0;
        //private int VehicleTimezoneDaylightSaving = 0;
        private DateTime _datetime = new DateTime();

        protected void Page_Load(object sender, EventArgs e)
        {
            string request = Request.QueryString["boxid"];
            if (!string.IsNullOrEmpty(request))
            {
                int.TryParse(request, out BoxId);
            }

            request = Request.QueryString["dt"];
            if (!string.IsNullOrEmpty(request))
            {
                //_datetime = Convert.ToDateTime(request);
                _datetime = DateTime.ParseExact(request, sn.User.DateFormat + " " + sn.User.TimeFormat, System.Globalization.CultureInfo.InvariantCulture);
                //conver to UTC time
                //_datetime = _datetime.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);

                // Changes for TimeZone Feature start
                _datetime = _datetime.AddHours(-sn.User.NewFloatTimeZone - (TimeZoneInfo.Local.IsDaylightSavingTime(_datetime) ? 1 : 0));
                // Changes for TimeZone Feature start
            }

            request = Request.QueryString["st"];
            if (!string.IsNullOrEmpty(request) && request.Trim().ToLower() == "download")
            {
                ShowHTML = false;
                DownloadVideo(Request.QueryString["fn"]);

            }
            else
            {
                VideoList_NewTZ();
            }

                      
        }

        // Changes for TimeZone Feature start
        public void VideoList_NewTZ()
        {
            //if (BoxId > 0)
            //{
            //    ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            //    string xml = string.Empty;

            //    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByBoxId(sn.UserID, sn.SecId, BoxId, ref xml), false))
            //        if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByBoxId(sn.UserID, sn.SecId, BoxId, ref xml), true))
            //        {
            //            return;
            //        }

            //    if (xml == "")
            //    {
            //        return;
            //    }

            //    DataSet ds = new DataSet();
            //    ds.ReadXml(new StringReader(xml));
            //    if (ds.Tables[0].Rows.Count > 0)
            //    {
            //        bool daylightSaving = false;
            //        int.TryParse(ds.Tables[0].Rows[0]["TimeZone"].ToString(), out VehicleTimezoneDaylightSaving);
            //        bool.TryParse(ds.Tables[0].Rows[0]["DayLightSaving"].ToString(), out daylightSaving);
            //        if (daylightSaving)
            //            VehicleTimezoneDaylightSaving++;
            //    }
            //}


            VedioList = GetFileList_NewTZ();
        }
        // Changes for TimeZone Feature end

        public void VideoList()
        {
            //if (BoxId > 0)
            //{
            //    ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            //    string xml = string.Empty;

            //    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByBoxId(sn.UserID, sn.SecId, BoxId, ref xml), false))
            //        if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByBoxId(sn.UserID, sn.SecId, BoxId, ref xml), true))
            //        {
            //            return;
            //        }

            //    if (xml == "")
            //    {
            //        return;
            //    }

            //    DataSet ds = new DataSet();
            //    ds.ReadXml(new StringReader(xml));
            //    if (ds.Tables[0].Rows.Count > 0)
            //    {
            //        bool daylightSaving = false;
            //        int.TryParse(ds.Tables[0].Rows[0]["TimeZone"].ToString(), out VehicleTimezoneDaylightSaving);
            //        bool.TryParse(ds.Tables[0].Rows[0]["DayLightSaving"].ToString(), out daylightSaving);
            //        if (daylightSaving)
            //            VehicleTimezoneDaylightSaving++;
            //    }
            //}


            VedioList = GetFileList();  
        }

        // Changes for TimeZone Feature start
        public string GetFileList_NewTZ()
        {
            StringBuilder result = new StringBuilder();
            WebResponse response = null;
            StreamReader reader = null;
            string emptyResult = "<Video></Video>";
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServer + "/videos"));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                //reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = false;
                response = reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string vediolist = "<Video>";
                string line = reader.ReadLine();
                string pattern = @"^(?<BoxId>\d+)-(?<Year>\d{4})-(?<Month>\d{1,2})-(?<Day>\d{1,2})-F(?<Hour>\d{2})(?<Minute>\d{2})(?<Second>\d{2})[a-zA-Z]\d{1}[a-zA-Z](?<Channel>\d{1}).*";
                DateTime dtFrom = _datetime.AddHours(-2);
                DateTime dtTo = _datetime.AddHours(2);
                string channel = "";
                string channelDesc = "";
                while (line != null)
                {
                    string[] buf = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int filesize = 0;
                    if (buf.Length >= 4 && int.TryParse(buf[2], out filesize))
                    {
                        try
                        {
                            Match match = Regex.Match(buf[3], pattern);
                            int _boxId = -1;
                            int.TryParse(match.Groups["BoxId"].Value, out _boxId);

                            if (_boxId == BoxId)
                            {
                                DateTime startRecordDateTime = new DateTime(int.Parse(match.Groups["Year"].Value), int.Parse(match.Groups["Month"].Value), int.Parse(match.Groups["Day"].Value), int.Parse(match.Groups["Hour"].Value), int.Parse(match.Groups["Minute"].Value), int.Parse(match.Groups["Second"].Value));
                                // Convert to UTC time
                                int daylightSaving = TimeZoneInfo.Local.IsDaylightSavingTime(startRecordDateTime) ? 1 : 0;
                                startRecordDateTime = startRecordDateTime.AddHours(5 - daylightSaving); // the ftp server's timezone is -5 (EST)
                                channel = match.Groups["Channel"].Value;
                                if (channel == "1")
                                    channelDesc = "Forward Facing";
                                else if (channel == "2")
                                    channelDesc = "Rear Facing";
                                else if (channel == "3")
                                    channelDesc = "Driver Facing";
                                if (startRecordDateTime > dtFrom && startRecordDateTime < dtTo)
                                {
                                    vediolist += "<VideoInfo>";
                                    vediolist += "<FileName>" + buf[3] + "</FileName>";
                                    vediolist += "<FileSize>" + filesize.ToString() + "</FileSize>";
                                    vediolist += "<UnitId>" + _boxId.ToString() + "</UnitId>";
                                    //vediolist += "<RecordingDateTime>" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", startRecordDateTime.AddHours(sn.User.TimeZone+sn.User.DayLightSaving)) + "</RecordingDateTime>";
                                    vediolist += "<RecordingDateTime>" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", startRecordDateTime.AddHours(sn.User.NewFloatTimeZone + daylightSaving)) + "</RecordingDateTime>";
                                    vediolist += "<Channel>" + channelDesc + "</Channel>";
                                    vediolist += "</VideoInfo>";
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                    line = reader.ReadLine();
                }

                vediolist += "</Video>";
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                return vediolist;
            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }

                return emptyResult + "<msg>" + ex.Message + "</msg>";
            }
        }

        // Changes for TimeZone Feature end

        public string GetFileList()
        {
            StringBuilder result = new StringBuilder();
            WebResponse response = null;
            StreamReader reader = null;
            string emptyResult = "<Video></Video>";
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServer + "/videos"));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                //reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = false;
                response = reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string vediolist = "<Video>";
                string line = reader.ReadLine();
                string pattern = @"^(?<BoxId>\d+)-(?<Year>\d{4})-(?<Month>\d{1,2})-(?<Day>\d{1,2})-F(?<Hour>\d{2})(?<Minute>\d{2})(?<Second>\d{2})[a-zA-Z]\d{1}[a-zA-Z](?<Channel>\d{1}).*";
                DateTime dtFrom = _datetime.AddHours(-2);
                DateTime dtTo = _datetime.AddHours(2);
                string channel = "";
                string channelDesc = "";
                while (line != null)
                {
                    string[] buf = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int filesize = 0;
                    if (buf.Length >= 4 && int.TryParse(buf[2], out filesize))
                    {
                        try
                        {
                            Match match = Regex.Match(buf[3], pattern);
                            int _boxId = -1;
                            int.TryParse(match.Groups["BoxId"].Value, out _boxId);
                            
                            if (_boxId == BoxId)
                            {
                                DateTime startRecordDateTime = new DateTime(int.Parse(match.Groups["Year"].Value), int.Parse(match.Groups["Month"].Value), int.Parse(match.Groups["Day"].Value), int.Parse(match.Groups["Hour"].Value), int.Parse(match.Groups["Minute"].Value), int.Parse(match.Groups["Second"].Value));
                                // Convert to UTC time
                                int daylightSaving = TimeZoneInfo.Local.IsDaylightSavingTime(startRecordDateTime) ? 1 : 0;
                                startRecordDateTime = startRecordDateTime.AddHours(5 - daylightSaving); // the ftp server's timezone is -5 (EST)
                                channel = match.Groups["Channel"].Value;
                                if (channel == "1")
                                    channelDesc = "Forward Facing";
                                else if (channel == "2")
                                    channelDesc = "Rear Facing";
                                else if (channel == "3")
                                    channelDesc = "Driver Facing";
                                if (startRecordDateTime > dtFrom && startRecordDateTime < dtTo)
                                {
                                    vediolist += "<VideoInfo>";
                                    vediolist += "<FileName>" + buf[3] + "</FileName>";
                                    vediolist += "<FileSize>" + filesize.ToString() + "</FileSize>";
                                    vediolist += "<UnitId>" + _boxId.ToString() + "</UnitId>";
                                    //vediolist += "<RecordingDateTime>" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", startRecordDateTime.AddHours(sn.User.TimeZone+sn.User.DayLightSaving)) + "</RecordingDateTime>";
                                    vediolist += "<RecordingDateTime>" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", startRecordDateTime.AddHours(sn.User.TimeZone + daylightSaving)) + "</RecordingDateTime>";
                                    vediolist += "<Channel>" + channelDesc + "</Channel>";
                                    vediolist += "</VideoInfo>";
                                }
                            }
                        }
                        catch {
                            
                        }
                    }
                    line = reader.ReadLine();
                }
                
                vediolist += "</Video>";
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                return vediolist;
            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                
                return emptyResult + "<msg>" + ex.Message + "</msg>";
            }
        }

        private void DownloadVideo(string filename)
        {
            try
            {                
                string uri = "ftp://" + ftpServer + "/videos/" + filename;
                Uri serverUri = new Uri(uri);
                if (serverUri.Scheme != Uri.UriSchemeFtp)
                {
                    return;
                }       
                FtpWebRequest reqFTP;                
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));                                
                reqFTP.Credentials = new NetworkCredential(ftpUserName, ftpPassword);                
                reqFTP.KeepAlive = false;                
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;                                
                reqFTP.UseBinary = true;
                reqFTP.Proxy = null;                 
                reqFTP.UsePassive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream responseStream = response.GetResponseStream();


                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
                //Response.AddHeader("Content-Length", file.Length.ToString());
                Response.ContentType = "video/x-ms-asf";
                Response.Flush();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    int Length = 2048;
                    Byte[] buffer = new Byte[Length];
                    int bytesRead = responseStream.Read(buffer, 0, Length);
                    while (bytesRead > 0)
                    {
                        memoryStream.Write(buffer, 0, bytesRead);
                        bytesRead = responseStream.Read(buffer, 0, Length);
                    }                           
                    
                    memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                    memoryStream.Close();
                }
                HttpContext.Current.Response.End();
                //Response.End();


                //FileStream writeStream = new FileStream(localDestnDir + "\" + file, FileMode.Create);                
                         
                //writeStream.Close();
                //response.Close(); 
            }
            catch (WebException wEx)
            {
                //MessageBox.Show(wEx.Message, "Download Error");
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Download Error");
            }
        }
    }
}