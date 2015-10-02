using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.IO;
using System.Globalization; 
namespace SentinelFM
{
    public partial class Map_frmMessageRotatingServer : System.Web.UI.Page
{
        public string strMessage;
        protected System.Web.UI.WebControls.Label lblTotalAlarms;
        public string _xml = "";
        public string _checksum = "";
        public string headerColor = "#009933";
        protected SentinelFMSession sn = null;
        protected clsUtility objUtil;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                MessagesList_Fill_NewTZ();
                  
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        // Changes for TimeZone Feature start
        private void MessagesList_Fill_NewTZ()
        {
            try
            {

                if (sn.UserID == 0)
                    return;

                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
                float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);

                _checksum = "";
                hist.GetMsgsShortInfoCheckSumCompleted +=
               new ServerDBHistory.GetMsgsShortInfoCheckSumCompletedEventHandler(GetMessagesXML);
                hist.GetMsgsShortInfoCheckSum_NewTZAsync(sn.UserID, sn.SecId, timeZone, _checksum);


            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
        // Changes for TimeZone Feature end
       
        private void MessagesList_Fill()
        {
            try
            {

                if (sn.UserID == 0)
                    return;

                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
                Int16 timeZone = Convert.ToInt16(sn.User.TimeZone + sn.User.DayLightSaving);

                _checksum = "";
                hist.GetMsgsShortInfoCheckSumCompleted +=
               new ServerDBHistory.GetMsgsShortInfoCheckSumCompletedEventHandler(GetMessagesXML);
                hist.GetMsgsShortInfoCheckSumAsync(sn.UserID, sn.SecId, timeZone, _checksum);


            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }




    void GetMessagesXML(Object source, ServerDBHistory.GetMsgsShortInfoCheckSumCompletedEventArgs   e)
    {
        objUtil = new clsUtility(sn);
        //Validate if key expired
        if ((VLF.ERRSecurity.InterfaceError)e.Result == VLF.ERRSecurity.InterfaceError.PassKeyExpired)
        {
            SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager();
            string secId = "";
            int result = sec.ReloginMD5ByDBName(sn.UserID, sn.Key, sn.UserName, sn.Password, sn.User.IPAddr, ref secId);
            if (result != 0)
            {
                sn.SecId = secId;
                MessagesList_Fill_NewTZ();
            }
        }

            _checksum  = e.checksum;
            if (_checksum == null || _checksum == "")
                return;

            if (sn.Message.MsgsCheckSum != _checksum)
            {
                sn.Message.MsgsCheckSum = _checksum;
                // Changes for TimeZone Feature start
                string strFromDT = DateTime.Now.AddHours(-24 - sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");
                string strToDT = DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");// Changes for TimeZone Feature end

                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
                if (objUtil.ErrCheck(hist.GetUserIncomingTextMessagesShortInfo_NewTZ(sn.UserID, sn.SecId, strFromDT, strToDT, ref _xml), false))
                    if (objUtil.ErrCheck(hist.GetUserIncomingTextMessagesShortInfo_NewTZ(sn.UserID, sn.SecId, strFromDT, strToDT, ref _xml), true))
                    {
                        return;
                    }
            }
            else
            {
                Response.Write(sn.Map.MessagesHTML + "~" + sn.Map.MessagesCount + "~" + sn.Message.MsgsCheckSum);
                return;
            }

        if (_xml == null || _xml == "")
            return;


        StringBuilder strBuild = new StringBuilder();
        String str = "";
        DataSet ds = new DataSet();
        int MessageTotals = 0;

        StringReader strrXML = new StringReader(_xml);
        ds.ReadXml(strrXML);
        string strStyle = "";

        if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            return;


        ////Unread Messages
        //DataRow[] drCollections = null;
        //drCollections = ds.Tables[0].Select("UserId=-1", "", DataViewRowState.CurrentRows);
        //foreach (DataRow rowItem in drCollections)
        //{
        //    strStyle = "style='{color:#000066;}'";
        //    str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd() + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>") + "]" + "</u></a></p><br>");

        //}

        //if (drCollections != null)
        //    MessageTotals = drCollections.Length.ToString();

        ////Read Messages
        //if (sn.User.ShowReadMess == 1)
        //{
        //    ds.Tables[0].Select();
        //    drCollections = ds.Tables[0].Select("UserId<>-1", "", DataViewRowState.CurrentRows);

        //    foreach (DataRow rowItem in drCollections)
        //    {
        //        strStyle = "style='{color:green;}'";
        //        str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd() + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>") + "]" + "</u></a></p><br>");

        //    }

        //    if (drCollections != null)
        //        MessageTotals = Convert.ToString((Convert.ToInt32(MessageTotals) + drCollections.Length));
        //}

        StringBuilder _messageXML = new StringBuilder();
        _messageXML.Append("<Message>");

        string MsgKey = "";
        foreach (DataRow rowItem in ds.Tables[0].Rows)
        {
            if (VLF.CLS.Util.PairFindValue("TXT", rowItem["MsgBody"].ToString()) != "")
                rowItem["MsgBody"] = VLF.CLS.Util.PairFindValue("TXT", rowItem["MsgBody"].ToString());

            if (rowItem["UserId"].ToString() == "-1")
            {
                MessageTotals++;
                strStyle = "style='{color:#000066;}'";
                
            }
            else if (sn.User.ShowReadMess == 1 && rowItem["UserId"].ToString() != "-1")
            {
                MessageTotals++;
                strStyle = "style='{color:green;}'";

            }
            else if (sn.User.ShowReadMess != 1 && rowItem["UserId"].ToString() != "-1")
            {
                continue;
            }


            MsgKey = rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + ";" + rowItem["peripheralId"].ToString().TrimEnd() + ";" + rowItem["MsgTypeId"].ToString().TrimEnd() + ";" + rowItem["MsgDateTime"].ToString().TrimEnd() + ";" + rowItem["checksumId"].ToString().TrimEnd();
            str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + MsgKey + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd() + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>") + "]" + "</u></a></p><br>");


            //if (sn.User.ShowReadMess == 1 && rowItem["UserId"].ToString()  != "-1")
            //{
            //    strStyle = "style='{color:green;}'";
            //    str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd() + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>") + "]" + "</u></a></p><br>");
            //    MessageTotals++;
            //    continue; 
            //}
            //else if  (rowItem["UserId"].ToString()  == "-1")
            //{
            //    MessageTotals++;
            //    strStyle = "style='{color:#000066;}'";
            //    str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd() + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>") + "]" + "</u></a></p><br>");
            //}

            _messageXML.Append("<MessageInfo>");
            _messageXML.Append("<MessageId>" + rowItem["MsgId"].ToString().TrimEnd() + "</MessageId>");
            _messageXML.Append("<MsgDateTime>" + rowItem["MsgDateTime"].ToString().TrimEnd() + "</MsgDateTime>");
            _messageXML.Append("<MsgKey>" + MsgKey + "</MsgKey>");
            _messageXML.Append("<VehicleId>" + rowItem["VehicleId"].ToString().TrimEnd() + "</VehicleId>");
            _messageXML.Append("<peripheralId>" + rowItem["peripheralId"].ToString().TrimEnd() + "</peripheralId>");
            _messageXML.Append("<MsgTypeId>" + rowItem["MsgTypeId"].ToString().TrimEnd() + "</MsgTypeId>");
            _messageXML.Append("<checksumId>" + rowItem["checksumId"].ToString().TrimEnd() + "</checksumId>");
            _messageXML.Append("<UserId>" + rowItem["UserId"].ToString().TrimEnd() + "</UserId>");
            _messageXML.Append("<Description>" + rowItem["Description"].ToString().TrimEnd() + "</Description>");
            _messageXML.Append("<MsgBody>" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", " ").Replace("\r\n", " ").Replace("\r", " ") + "</MsgBody>");
            _messageXML.Append("<Acknowledged>" + rowItem["Acknowledged"].ToString().TrimEnd() + "</Acknowledged>");
            _messageXML.Append("</MessageInfo>");
            

        }

        _messageXML.Append("</Message>");
        sn.Map.MessagesXML = _messageXML.ToString();

        //sn.Map.MessagesHTML = strBuild.ToString() ;
        sn.Map.MessagesHTML = str;
        sn.Map.MessagesCount = MessageTotals;

        Response.Write(sn.Map.MessagesHTML + "~" + sn.Map.MessagesCount+"~"+sn.Message.MsgsCheckSum );
        Response.End();  
    }


  

    protected override void InitializeCulture()
    {

        if (Session["PreferredCulture"] != null)
        {
            string UserCulture = Session["PreferredCulture"].ToString();
            if (UserCulture != "")
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(UserCulture);
            }
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {

        sn = (SentinelFMSession)Session["SentinelFMSession"];
      
        if (sn.User.MenuColor != "")
            headerColor = sn.User.MenuColor;

      

    }


        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion


    }
}

