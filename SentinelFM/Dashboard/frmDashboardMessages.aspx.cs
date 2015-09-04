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
using System.IO;
using System.Globalization;


namespace SentinelFM
{
    public partial class Dashboard_frmDashboardMessages : SentinelFMBasePage 
    {
        public string _xml = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                MessagesList_Fill_NewTZ(); 

            this.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
            this.lblLastUpdated.Text = System.DateTime.Now.ToShortTimeString(); 
        }

        private void Page_PreRenderComplete(object sender, EventArgs e)
        {
            //dgAlarms.ClearCachedDataSource();
            //dgAlarms.RebindDataSource(); 
        }

        // Changes for TimeZone Feature start
        private void MessagesList_Fill_NewTZ()
        {
            try
            {

                string strFromDT = "";
                string strToDT = "";

                strFromDT = DateTime.Now.AddHours(-24 - sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");
                strToDT = DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");

                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();


                hist.GetUserTextMessagesShortInfoCompleted +=
         new ServerDBHistory.GetUserTextMessagesShortInfoCompletedEventHandler(GetMessagesXML);
                hist.GetUserTextMessagesShortInfo_NewTZAsync(sn.UserID, sn.SecId, strFromDT, strToDT, Convert.ToInt16(VLF.CLS.Def.Enums.TxtMsgDirectionType.In), _xml);


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

                string strFromDT = "";
                string strToDT = "";

                strFromDT = DateTime.Now.AddHours(-24 - sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");
                strToDT = DateTime.Now.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");

                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();


                hist.GetUserTextMessagesShortInfoCompleted +=
         new ServerDBHistory.GetUserTextMessagesShortInfoCompletedEventHandler(GetMessagesXML);
                hist.GetUserTextMessagesShortInfoAsync(sn.UserID, sn.SecId, strFromDT, strToDT, Convert.ToInt16(VLF.CLS.Def.Enums.TxtMsgDirectionType.In), _xml);


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




        void GetMessagesXML(Object source, ServerDBHistory.GetUserTextMessagesShortInfoCompletedEventArgs e)
        {

            try
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

                _xml = e.xml;
                if (_xml == null || _xml == "")
                    return;

                DataSet ds = new DataSet();

                StringReader strrXML = new StringReader(_xml);
                ds.ReadXml(strrXML);


                DataColumn MsgDate = new DataColumn("MsgDate", Type.GetType("System.DateTime"));
                DataColumn MsgKey = new DataColumn("MsgKey", Type.GetType("System.String"));
                MsgKey.DefaultValue = "";
                ds.Tables[0].Columns.Add(MsgKey);

                //// Show Combobox
                DataColumn dc = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
                dc.DefaultValue = false;
                ds.Tables[0].Columns.Add(dc);


                ds.Tables[0].Columns.Add(MsgDate);
                string strStreetAddress = "";
                foreach (DataRow rowItem in ds.Tables[0].Rows)
                {
                    rowItem["MsgDate"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString());
                    rowItem["MsgKey"] = rowItem["MsgId"].ToString() + ";" + rowItem["Vehicleid"].ToString();
                }



                sn.History.DsMessages = ds;

                dgMessages.ClearCachedDataSource();
                dgMessages.RebindDataSource();
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


        protected void dgMessages_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            if (sn.History.DsMessages != null)
                e.DataSource = sn.History.DsMessages;
        }
        protected void cmdReadMessage_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
            Int32 BoxId = 0;


            this.lblMessage.Text = "";
            ArrayList rowsCount = dgMessages.RootTable.GetCheckedRows();
            if (rowsCount.Count == 0)
            {
                this.lblMessage.Text = "Please select a Message";
                return;
            }

            foreach (string keyValue in dgMessages.RootTable.GetCheckedRows())
            {

                 #if MDT_NEW
                    string[] tmp = keyValue.Split(';');

                    DataRow[] drArr = sn.Message.DsHistoryMessages.Tables[0].Select("VehicleId='" + tmp[1] + "' and MsgId='" + tmp[0] + "'");
                    if (drArr == null || drArr.Length == 0)
                        continue;
                    BoxId = Convert.ToInt32(drArr[0]["BoxId"].ToString());

                    if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(tmp[0]), BoxId), false))
                        if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(tmp[0]), BoxId), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Error in closing text message. User:" + sn.UserID.ToString() + " Form:frmMessageInfo.aspx"));
                        }
                #else
                if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), false))
                    if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Error in closing text message. User:" + sn.UserID.ToString() + " Form:frmMessageInfo.aspx"));
                    }
                #endif

               
            }

            MessagesList_Fill_NewTZ();

        }
}
}
