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
public partial class Map_frmMessageRotatingCallBack :  System.Web.UI.Page, ICallbackEventHandler
{
        public string strMessage;
        protected SentinelFMSession sn = null;
        protected System.Web.UI.WebControls.Label lblTotalAlarms;
        protected clsUtility objUtil;
        public string _xml = "";
        public string _checksum = "";
        public string headerColor = "#009933";

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                //Clear IIS cache
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);

                // Define a StringBuilder to hold messages to output.
                StringBuilder sb = new StringBuilder();

                // Get a ClientScriptManager reference from the Page class.
                ClientScriptManager cs = Page.ClientScript;

                // Define one of the callback script's context.
                // The callback script will be defined in a script block on the page.
                StringBuilder context = new StringBuilder();
                context.Append("function ReceiveServerData(arg)");
                context.Append("{");
                context.Append("var msg_array=arg.split('~');  wholemessage =  msg_array[0]; lblMessTotal.innerHTML=msg_array[1];");
                context.Append("}");

                // Define callback references.
                String cbReference = cs.GetCallbackEventReference(this, "arg",
                    "ReceiveServerData", context.ToString());
                String callbackScript = "function CallTheServer(arg) { " +
                    cbReference + "; }";

                // Register script blocks will perform call to the server.
                cs.RegisterClientScriptBlock(this.GetType(), "CallTheServer",
                    callbackScript, true);

                objUtil = new clsUtility(sn);
                this.PreRenderComplete += new EventHandler(Page_PreRenderComplete);

                if (!Page.IsPostBack)
                {
                   MessagesList_Fill_NewTZ();
                   this.lblMessTotal.Text =  sn.Map.MessagesCount.ToString() ;
                   
                }
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


        public void RaiseCallbackEvent(String eventArgument)
        {
           MessagesList_Fill_NewTZ();
           eventArgument = sn.Map.MessagesHTML + "~" + sn.Map.MessagesCount ;
        }

        string ICallbackEventHandler.GetCallbackResult()
        {
           return sn.Map.MessagesHTML + "~" + sn.Map.MessagesCount;
        }

        // Changes for TimeZone Feature start

        private void MessagesList_Fill_NewTZ()
        {
            try
            {

                if (sn.UserID == 0)
                    return;


                //string strFromDT = "";
                //string strToDT = "";
                //this.lblMessTotal.Text = "0";

                //strFromDT = DateTime.Now.AddHours(-24 - sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");
                //strToDT = DateTime.Now.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");
                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
                float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);

                //       hist.GetUserTextMessagesShortInfoCompleted +=
                //new ServerDBHistory.GetUserTextMessagesShortInfoCompletedEventHandler(GetMessagesXML);
                //       hist.GetUserTextMessagesShortInfoAsync(sn.UserID, sn.SecId, strFromDT, strToDT, Convert.ToInt16(VLF.CLS.Def.Enums.TxtMsgDirectionType.In), _xml);


                _checksum = "";
                hist.GetMsgsShortInfoCheckSumCompleted +=
               new ServerDBHistory.GetMsgsShortInfoCheckSumCompletedEventHandler(GetMessagesXML);
                hist.GetMsgsShortInfoCheckSum_NewTZAsync(sn.UserID, sn.SecId, timeZone, _checksum);



                ////string xml = "";
                ////if (objUtil.ErrCheck(hist.GetUserTextMessagesShortInfo(sn.UserID, sn.SecId, strFromDT, strToDT, Convert.ToInt16(VLF.CLS.Def.Enums.TxtMsgDirectionType.In), ref xml), false))
                ////    if (objUtil.ErrCheck(hist.GetUserTextMessagesShortInfo(sn.UserID, sn.SecId, strFromDT, strToDT, Convert.ToInt16(VLF.CLS.Def.Enums.TxtMsgDirectionType.In), ref xml), true))
                ////    {
                ////        return;
                ////    }

                ////if (xml == "")
                ////{
                ////    return;
                ////}

                //strrXML = new StringReader(xml);

                //DataSet ds = new DataSet();
                //ds.ReadXml(strrXML);

                //string str = "";
                //string strStyle = "";

                ////Unread Messages
                //DataRow[] drCollections = null;
                //drCollections = ds.Tables[0].Select("UserId=-1", "", DataViewRowState.CurrentRows);
                //foreach (DataRow rowItem in drCollections)
                //{
                //    strStyle = "style='{color:#000066;}'";
                //   str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd() + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n","<br>").Replace("\r\n","<br>").Replace("\r","<br>")    + "]" + "</u></a></p><br>");
                //}

                //if (drCollections != null)
                //    this.lblMessTotal.Text = drCollections.Length.ToString();

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
                //        this.lblMessTotal.Text = Convert.ToString((Convert.ToInt32(this.lblMessTotal.Text) + drCollections.Length));
                //}




                //sn.Map.MessagesHTML = str;
                //sn.Map.MessagesCount = Convert.ToInt32(lblMessTotal.Text);
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


                //string strFromDT = "";
                //string strToDT = "";
                //this.lblMessTotal.Text = "0";

                //strFromDT = DateTime.Now.AddHours(-24 - sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");
                //strToDT = DateTime.Now.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");
                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
                Int16 timeZone = Convert.ToInt16(sn.User.TimeZone + sn.User.DayLightSaving);

         //       hist.GetUserTextMessagesShortInfoCompleted +=
         //new ServerDBHistory.GetUserTextMessagesShortInfoCompletedEventHandler(GetMessagesXML);
         //       hist.GetUserTextMessagesShortInfoAsync(sn.UserID, sn.SecId, strFromDT, strToDT, Convert.ToInt16(VLF.CLS.Def.Enums.TxtMsgDirectionType.In), _xml);


                _checksum = "";
                hist.GetMsgsShortInfoCheckSumCompleted +=
               new ServerDBHistory.GetMsgsShortInfoCheckSumCompletedEventHandler(GetMessagesXML);
                hist.GetMsgsShortInfoCheckSumAsync(sn.UserID, sn.SecId, timeZone, _checksum);



                ////string xml = "";
                ////if (objUtil.ErrCheck(hist.GetUserTextMessagesShortInfo(sn.UserID, sn.SecId, strFromDT, strToDT, Convert.ToInt16(VLF.CLS.Def.Enums.TxtMsgDirectionType.In), ref xml), false))
                ////    if (objUtil.ErrCheck(hist.GetUserTextMessagesShortInfo(sn.UserID, sn.SecId, strFromDT, strToDT, Convert.ToInt16(VLF.CLS.Def.Enums.TxtMsgDirectionType.In), ref xml), true))
                ////    {
                ////        return;
                ////    }

                ////if (xml == "")
                ////{
                ////    return;
                ////}

                //strrXML = new StringReader(xml);

                //DataSet ds = new DataSet();
                //ds.ReadXml(strrXML);

                //string str = "";
                //string strStyle = "";

                ////Unread Messages
                //DataRow[] drCollections = null;
                //drCollections = ds.Tables[0].Select("UserId=-1", "", DataViewRowState.CurrentRows);
                //foreach (DataRow rowItem in drCollections)
                //{
                //    strStyle = "style='{color:#000066;}'";
                //   str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd() + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n","<br>").Replace("\r\n","<br>").Replace("\r","<br>")    + "]" + "</u></a></p><br>");
                //}

                //if (drCollections != null)
                //    this.lblMessTotal.Text = drCollections.Length.ToString();

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
                //        this.lblMessTotal.Text = Convert.ToString((Convert.ToInt32(this.lblMessTotal.Text) + drCollections.Length));
                //}



                
                //sn.Map.MessagesHTML = str;
                //sn.Map.MessagesCount = Convert.ToInt32(lblMessTotal.Text);
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
                string strToDT = DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"); // Changes for TimeZone Feature end

                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
                if (objUtil.ErrCheck(hist.GetUserTextMessagesShortInfo_NewTZ(sn.UserID, sn.SecId, strFromDT, strToDT, Convert.ToInt16(VLF.CLS.Def.Enums.TxtMsgDirectionType.In), ref _xml), false))
                    if (objUtil.ErrCheck(hist.GetUserTextMessagesShortInfo_NewTZ(sn.UserID, sn.SecId, strFromDT, strToDT, Convert.ToInt16(VLF.CLS.Def.Enums.TxtMsgDirectionType.In), ref _xml), true))
                    {
                        return;
                    }
            }
            else
                return; 

        if (_xml == null || _xml == "")
            return;


        StringBuilder strBuild = new StringBuilder();
        String str = "";
        DataSet ds = new DataSet();
      

        StringReader strrXML = new StringReader(_xml);
        ds.ReadXml(strrXML);
        string strStyle = "";

        if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            return;


        //Unread Messages
        DataRow[] drCollections = null;
        drCollections = ds.Tables[0].Select("UserId=-1", "", DataViewRowState.CurrentRows);
        foreach (DataRow rowItem in drCollections)
        {
            strStyle = "style='{color:#000066;}'";
            str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd() + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>") + "]" + "</u></a></p><br>");

            //strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1};{2}') >'{3}' '{4}'<br>[{5}]</u></a></p><br>",
            //  strStyle,
            //  rowItem["MsgId"].ToString().TrimEnd(),
            //  rowItem["VehicleId"].ToString().TrimEnd(),
            //  Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString(),
            //  rowItem["Description"].ToString().TrimEnd(),
            //  rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>"));
        }

        if (drCollections != null)
            this.lblMessTotal.Text = drCollections.Length.ToString();

        //Read Messages
        if (sn.User.ShowReadMess == 1)
        {
            ds.Tables[0].Select();
            drCollections = ds.Tables[0].Select("UserId<>-1", "", DataViewRowState.CurrentRows);

            foreach (DataRow rowItem in drCollections)
            {
                strStyle = "style='{color:green;}'";
                str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd() + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>") + "]" + "</u></a></p><br>");

                //strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1};{2}') >'{3}'  '{4}'<br>['{5}']</u></a></p><br>",
                //strStyle,
                //rowItem["MsgId"].ToString().TrimEnd(),
                //rowItem["VehicleId"].ToString().TrimEnd(),
                //Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString(),
                //rowItem["Description"].ToString().TrimEnd(),
                //rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>"));
            }

            if (drCollections != null)
                this.lblMessTotal.Text = Convert.ToString((Convert.ToInt32(this.lblMessTotal.Text) + drCollections.Length));
        }




        //sn.Map.MessagesHTML = strBuild.ToString() ;
        sn.Map.MessagesHTML = str;
        sn.Map.MessagesCount = Convert.ToInt32(lblMessTotal.Text);

    }


    private void Page_PreRenderComplete(object sender, EventArgs e)
    {
        
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
        if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
        {
            RedirectToLogin();
            return;
        }

        if (sn.User.MenuColor != "")
            headerColor = sn.User.MenuColor;

        //HtmlGenericControl hgc = new HtmlGenericControl("style");
        //hgc.Attributes.Add("type", "text/css");

        //String filePath = HttpContext.Current.Server.MapPath("../GlobalStyle.css");

        //StreamReader reader = new StreamReader(filePath);
        //string content = reader.ReadToEnd();
        //reader.Close();

        //if (sn.User.MenuColor != "")
        //    hgc.InnerText = content.Replace("#009933", sn.User.MenuColor);
        //else
        //    hgc.InnerText = content;

        //System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //StringWriter tw = new StringWriter(sb);
        //HtmlTextWriter hw = new HtmlTextWriter(tw);
        //hgc.RenderControl(hw);
        //Response.Write(sb.ToString());

    }


    public void RedirectToLogin()
    {

        Session.Abandon();
        Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('../Login.aspx','_top')</SCRIPT>");
        return;
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

