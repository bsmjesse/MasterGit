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
using WebChart;
using System.Drawing;
namespace SentinelFM
{
    
    public partial class Dashboard_frmDashboardAlarms : SentinelFMBasePage
    {
        public string _xml = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                AlarmsList_Fill_NewTZ();

            this.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
            this.lblLastUpdated.Text = System.DateTime.Now.ToShortTimeString(); 
        }

        private void Page_PreRenderComplete(object sender, EventArgs e)
        {
            //dgAlarms.ClearCachedDataSource();
            //dgAlarms.RebindDataSource(); 
        }

        // Changes for TimeZone Feature start

        private void AlarmsList_Fill_NewTZ()
        {
            try
            {

                ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
                float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);

                alarms.GetAlarmsShortInfoXMLByLang_NewTZCompleted +=
               new ServerAlarms.GetAlarmsShortInfoXMLByLang_NewTZCompletedEventHandler(GetAlarmsXML_NewTZ);
                alarms.GetAlarmsShortInfoXMLByLang_NewTZAsync(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, _xml);



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

        private void AlarmsList_Fill()
        {
            try
            {

                ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
                Int16 timeZone = Convert.ToInt16(sn.User.TimeZone + sn.User.DayLightSaving);

                alarms.GetAlarmsShortInfoXMLByLangCompleted +=
               new ServerAlarms.GetAlarmsShortInfoXMLByLangCompletedEventHandler(GetAlarmsXML);
                alarms.GetAlarmsShortInfoXMLByLangAsync(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, _xml);

               

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

        void GetAlarmsXML_NewTZ(Object source, ServerAlarms.GetAlarmsShortInfoXMLByLang_NewTZCompletedEventArgs e)
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
                        AlarmsList_Fill_NewTZ();
                    }
                }

                _xml = e.xml;
                if (_xml == null || _xml == "")
                    return;


                String str = "";
                DataSet dsFleetInfo = new DataSet();
                if (_xml == null || _xml == "")
                    return;

                StringReader strrXML = new StringReader(_xml);

                DataSet ds = new DataSet();


                //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
                //string strPath = MapPath(DataSetPath) + @"\Alarms.xsd";
                string strPath = Server.MapPath("../Datasets/Alarms.xsd");
                ds.ReadXmlSchema(strPath);
                ds.ReadXml(strrXML);


                //// Show Combobox
                DataColumn dc = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
                dc.DefaultValue = false;
                ds.Tables[0].Columns.Add(dc);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
                        ds.Tables[0].Rows[i].Delete();

                }

                sn.History.DsAlarms = ds;
                dgAlarms.ClearCachedDataSource();
                dgAlarms.RebindDataSource();
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

        void GetAlarmsXML(Object source, ServerAlarms.GetAlarmsShortInfoXMLByLangCompletedEventArgs e)
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
                        AlarmsList_Fill();
                    }
                }

                _xml = e.xml;
                if (_xml == null || _xml == "")
                    return;


                String str = "";
                DataSet dsFleetInfo = new DataSet();
                if (_xml == null || _xml == "")
                    return;

                StringReader strrXML = new StringReader(_xml);

                DataSet ds = new DataSet();


                //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
                //string strPath = MapPath(DataSetPath) + @"\Alarms.xsd";
                string strPath = Server.MapPath("../Datasets/Alarms.xsd");
                ds.ReadXmlSchema(strPath);
                ds.ReadXml(strrXML);


                //// Show Combobox
                DataColumn dc = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
                dc.DefaultValue = false;
                ds.Tables[0].Columns.Add(dc);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
                        ds.Tables[0].Rows[i].Delete();  
                 
                }
  
                sn.History.DsAlarms = ds;
                dgAlarms.ClearCachedDataSource();
                dgAlarms.RebindDataSource();
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



        protected void dgAlarms_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            if (sn.History.DsAlarms != null)
                e.DataSource = sn.History.DsAlarms;
        }
       
        protected void cmdAcceptAlarm_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
            this.lblMessage.Text = "";
            ArrayList rowsCount = dgAlarms.RootTable.GetCheckedRows();
            if (rowsCount.Count == 0)
            {
                this.lblMessage.Text = "Please select an Alarm";
                return; 
            }

            foreach (string keyValue in dgAlarms.RootTable.GetCheckedRows())
            {
                if (objUtil.ErrCheck(alarms.AcceptCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), false))
                    if (objUtil.ErrCheck(alarms.AcceptCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), true))
                    {
                        this.lblMessage.Text = "Alarm(s) accept"; 
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Alarm Accept failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }

            }
            AlarmsList_Fill_NewTZ();

            this.lblMessage.Text = "Alarm(s) have been accepted"; 
        }
        protected void cmdCloseAlarm_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();

            this.lblMessage.Text = "";
            ArrayList rowsCount = dgAlarms.RootTable.GetCheckedRows();
            if (rowsCount.Count == 0)
            {
                this.lblMessage.Text = "Please select an Alarm";
                return;
            }

            foreach (string keyValue in dgAlarms.RootTable.GetCheckedRows())
            {
                if (objUtil.ErrCheck(alarms.CloseCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), false))
                    if (objUtil.ErrCheck(alarms.CloseCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Alarm Accept failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }

            }

            this.lblMessage.Text = "Alarm(s) have been closed";
            AlarmsList_Fill_NewTZ();
        }
       
}
}