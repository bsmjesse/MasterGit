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
using ISNet.WebUI.WebDesktop;
using System.IO;
using System.Globalization;
 
namespace SentinelFM
{
    public partial class frmDashboard : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WebExplorerBar barAlarms = new WebExplorerBar();
            barAlarms.Text = "Alarms :";// +sn.History.DsAlarms.Tables[0].Rows.Count
            barAlarms.ContentMode = NavBarContentMode.UseIFrame;
            barAlarms.ContentURL = "frmDashboardAlarms.aspx";

            WebPane1.Panes.Add(barAlarms);

            WebExplorerBar barMessages = new WebExplorerBar();
            barMessages.Text = "Message";// +sn.History.DsMessages.Tables[0].Rows.Count;
            barMessages.ContentMode = NavBarContentMode.UseIFrame;
            barMessages.ContentURL = "frmDashboardMessages.aspx";

            WebPane1.Panes.Add(barMessages);


            WebExplorerBar barNotifications = new WebExplorerBar();
            barNotifications.Text = "Notifications";
            barNotifications.ContentMode = NavBarContentMode.UseIFrame;
            barNotifications.ContentURL = "frmDashboardNotification.aspx";

            WebPane1.Panes.Add(barNotifications);


            WebExplorerBar barActivitySummary = new WebExplorerBar();
            barActivitySummary.Text = "Activity Summary";// +sn.History.DsMessages.Tables[0].Rows.Count;
            barActivitySummary.ContentMode = NavBarContentMode.UseIFrame;
            barActivitySummary.ContentURL = "frmDashBoardActivitySummary.aspx";

            WebPane1.Panes.Add(barActivitySummary);
            if (!Page.IsPostBack)
            {
                AlarmsList_Fill_NewTZ();
            }

           // this.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
        }

        private void Page_PreRenderComplete(object sender, EventArgs e)
        {
            //if (sn.History.DsMessages == null && sn.History.DsAlarms == null)
            //{
            //    Response.Write("<script language='javascript'>location.reload();</script>");
            //    return; 
            //}
            //if (sn.History.DsMessages==null  )
            //    WebPane1.Panes[1].Collapse = true;
        }

        // Changes for TimeZone Feature start

        private void AlarmsList_Fill_NewTZ()
        {
            try
            {

                ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
                float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);
                string xml = "";


                if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        return;
                    }


                String str = "";
                DataSet dsFleetInfo = new DataSet();
                if (xml == null || xml == "")
                    return;

                StringReader strrXML = new StringReader(xml);
                DataSet ds = new DataSet();
                //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
                //string strPath = MapPath(DataSetPath) + @"\Alarms.xsd";
                string strPath = Server.MapPath("../Datasets/Alarms.xsd");
                ds.ReadXmlSchema(strPath);
                ds.ReadXml(strrXML);
                sn.History.DsAlarms = ds;



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
                string xml = "";


                if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        return; 
                    }

                
                String str = "";
                DataSet dsFleetInfo = new DataSet();
                if (xml == null || xml == "")
                    return;

                StringReader strrXML = new StringReader(xml);
                DataSet ds = new DataSet();
                //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
                //string strPath = MapPath(DataSetPath) + @"\Alarms.xsd";
                string strPath =Server.MapPath("../Datasets/Alarms.xsd");
                ds.ReadXmlSchema(strPath);
                ds.ReadXml(strrXML);
                sn.History.DsAlarms = ds;
               


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

    }
}
