using System;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//using Com.Mapsolute.Webservices.MapletRemoteControl;

namespace SentinelFM
{
    /// <summary>
    /// Summary description for SentinelFMSession.
    /// </summary>
    /// 

    public class SentinelFMSession : SentinelFMSessionBase
    {

        string token;
        byte checkSum;
        byte securityLevel;


        private Command cmd;
        private clsReportParam report;
        private clsMap map;
        private DataSet dslandmarks = null;
        private clsHistory history;
        private clsMessage message;
        private clsUser user;
        private clsLandmark landmark;
        private clsGeoZone geozone;
        //private clsMapViewer mapViewer;
        private clsAdmin admin;
        private clsMaintenance maint;
        
        //private clsMapSolute mapSolute;
        protected string messageText = "";
        private clsMisc misc;

        private DataSet _dsLandmarkPoints = new DataSet();

        private DataSet _rootOH = null;
        private string _rootNodeCode = string.Empty;


        public static SentinelFMSessionBase GetSessionBase(SentinelFMSession session)
        {
            SentinelFMSessionBase snb = new SentinelFMSessionBase();
            snb.Key = session.key;
            snb.Password = session.password;
            snb.SecId = session.secId;
            snb.SuperOrganizationId = session.superOrganizationId;
            snb.UserID = session.userID;
            snb.UserName = session.userName;
            snb.CompanyURL = session.companyURL;
            snb.HomePagePicture = session.homePagePicture;
            snb.SelectedLanguage = session.selectedLanguage;
            snb.LoginUserID = session.loginUserID;
            snb.LoginUserSecId = session.LoginUserSecId;

            return snb;
        }

        public SentinelFMSessionBase GetSessionBase()
        {
            SentinelFMSessionBase snb = new SentinelFMSessionBase();
            snb.Key = base.key;
            snb.Password = base.password;
            snb.SecId = base.secId;
            snb.SuperOrganizationId = base.superOrganizationId;
            snb.UserID = base.userID;
            snb.UserName = base.userName;
            snb.CompanyURL = base.companyURL;
            snb.HomePagePicture = base.homePagePicture;
            snb.SelectedLanguage = base.selectedLanguage;
            snb.LoginUserID = base.loginUserID;
            snb.LoginUserSecId = base.LoginUserSecId;

            return snb;
        }

        public string GetSerializedBase()
        {
            byte[] buffer;
            try
            {
                SentinelFMSessionBase snb = this.GetSessionBase();
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    formatter.Serialize(stream, snb);
                    buffer = stream.GetBuffer();
                    stream.Close();
                }
                return Convert.ToBase64String(buffer);
            }
            catch  { }
            return string.Empty;
        }

        public static SentinelFMSessionBase GetSessionBase(string encoded)
        {

            SentinelFMSessionBase snb = null;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(encoded)))
                {
                    snb = (SentinelFMSessionBase)formatter.Deserialize(stream);
                    stream.Close();
                }
            }
            catch  { }
            return snb;
        }

        public static SentinelFMSessionBase GetSessionBaseByKey(string key)
        {

            //string encoded = clsUtility.GetSessionBaseFromMemcahed(key);
            string encoded = clsUtility.GetSessionBaseFromTxtFile(key);
            SentinelFMSessionBase snb = null;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(encoded)))
                {
                    snb = (SentinelFMSessionBase)formatter.Deserialize(stream);
                    stream.Close();
                }
            }
            catch  { }
            return snb;
        }

        public clsMisc Misc
        {
            get { return misc; }
            set { misc = value; }
        }


        public clsAdmin Admin
        {
            get { return admin; }
            set { admin = value; }
        }

        //public clsMapViewer MapViewer
        //{
        //    get{return mapViewer;}
        //    set{mapViewer = value;}
        //}

        public string MessageText
        {
            get { return messageText; }
            set { messageText = value; }
        }

        public clsGeoZone GeoZone
        {
            get { return geozone; }
            set { geozone = value; }
        }

        public clsReportParam Report
        {
            get { return report; }
            set { report = value; }
        }

        public DataSet RootOrganizationHierarchy
        {
            get { return _rootOH; }
            set { _rootOH = value; }
        }

        public string RootOrganizationHierarchyNodeCode
        {
            get { return _rootNodeCode; }
            set { _rootNodeCode = value; }
        }

        public clsUser User
        {
            get { return user; }
            set { user = value; }
        }

        public clsHistory History
        {
            get { return history; }
            set { history = value; }
        }


        public clsMessage Message
        {
            get { return message; }
            set { message = value; }
        }

        public clsMap Map
        {
            get { return map; }
            set { map = value; }
        }

        public DataSet DsLandMarks
        {
            get { return dslandmarks; }
            set { dslandmarks = value; }
        }

        public DataSet DsLandmarkPoints
        { 
            get { return _dsLandmarkPoints; }
            set { _dsLandmarkPoints = value; }
        }

        public Command Cmd
        {
            get { return cmd; }
            set { cmd = value; }
        }


        public clsLandmark Landmark
        {
            get { return landmark; }
            set { landmark = value; }
        }

        public clsMaintenance Maint
        {
            get { return maint; }
            set { maint = value; }
        }


        public byte CheckSum
        {
            get { return checkSum; }
            set { checkSum = value; }
        }

        public byte SecurityLevel
        {
            get { return securityLevel; }
            set { securityLevel = value; }
        }

        public string Token
        {
            get { return token; }
            set { token = value; }
        }

        public int AlarmCount { get; set; }
        public DateTime AlarmLastTimeCreated { get; set; }
        public int AlarmAccepted { get; set; }
        public int AlarmClosed { get; set; }

        //Devin Added for map default view
        public string MapCenter { get; set; }
        public string MapZoomLevel { get; set; }

        //public string SecId
        //{
        //    get { return base.secId; }
        //    set
        //    {
        //        base.secId = value;
        //        CheckSum = GetValidationChecksum(value);
        //    }
        //}

        public bool ChecksumMatch(byte value)
        {
            return value == checkSum;
        }

        byte GetValidationChecksum(string sid)
        {
            byte cs = 0;
            char[] ca = sid.ToCharArray();
            for (int i = 0; i < ca.Length; cs ^= Convert.ToByte(ca[i++])) ;
            return cs;
        }


        


        //public clsMapSolute MapSolute
        //{
        //   get { return mapSolute; }
        //   set { mapSolute = value; }
        //}

        public SentinelFMSession() : this(new SentinelFMSessionBase()) { }
        public SentinelFMSession(SentinelFMSessionBase sessionBase)
        {

            try
            {
                if (sessionBase == null)
                    return;

                base.key = sessionBase.Key;
                base.password = sessionBase.Password;
                base.secId = sessionBase.SecId;
                base.superOrganizationId = sessionBase.SuperOrganizationId;
                base.userID = sessionBase.UserID;
                base.userName = sessionBase.UserName;
                base.selectedLanguage = sessionBase.SelectedLanguage;
                base.loginUserID = sessionBase.LoginUserID;
                base.LoginUserSecId = sessionBase.LoginUserSecId;

                history = new clsHistory();
                user = new clsUser();
                cmd = new Command();
                //reportClear=clsReports.GetInstance()  ;
                report = new clsReportParam();
                map = new clsMap();
                message = new clsMessage();
                landmark = new clsLandmark();
                geozone = new clsGeoZone();
                //mapViewer=new clsMapViewer();     
                //config= new AppConfig.GetInstance(); 
                admin = new clsAdmin();
                maint = new clsMaintenance();
                misc = new clsMisc();
                // mapSolute = new clsMapSolute(); 

            }
            catch
            {
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.Redirect("redirect.aspx");
            }
        }



        
    }
}



