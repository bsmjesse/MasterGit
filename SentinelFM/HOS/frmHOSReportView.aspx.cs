using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using HOSgraph;
using System.IO; 

namespace SentinelFM
{
    public partial class HOS_frmHOSReportView : SentinelFMBasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Int32 driverId = Convert.ToInt32(Request.QueryString["driverId"]);
            string driverName = Request.QueryString["driverName"];
            string dateHOS = Request.QueryString["dateHOS"];
            this.lblDate.Text = dateHOS;
            this.lblDriver.Text = driverName;
            dsDrivers_Fill_NewTZ(driverId, dateHOS);
            if (sn.User.DsDrivers != null && sn.User.DsDrivers.Tables.Count > 0 && sn.User.DsDrivers.Tables[0].Rows.Count > 1)
            {
                HoSGraph hos = new HoSGraph();
                String graph = hos.drawHOS(sn.User.DsDrivers.Tables[0], "ServiceDate", "StateTypeId", "png");
                this.imgGraph.ImageUrl = graph;
            }
        }

        // Changes for TimeZone Feature start

        private void dsDrivers_Fill_NewTZ(Int32 driverId, string dateHOS)
        {
            //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
            //string strPath = MapPath(DataSetPath) + @"\HOSinfo.xsd";

            string strPath = Server.MapPath("../Datasets/HOSinfo.xsd");


            string xml = "";
            DateTime dDateTimeFrom = Convert.ToDateTime(dateHOS + " " + "00:00").AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving);
            DateTime dDateTimeTo = Convert.ToDateTime(dateHOS + " " + "00:00").AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).AddDays(1);

            using (ServerDBDriver.DBDriver drv = new global::SentinelFM.ServerDBDriver.DBDriver())
            {


                if (objUtil.ErrCheck(drv.GetHOSbyDateTime_NewTZ(sn.UserID, sn.SecId, driverId, dDateTimeFrom, dDateTimeTo, ref xml), false))
                    if (objUtil.ErrCheck(drv.GetHOSbyDateTime_NewTZ(sn.UserID, sn.SecId, driverId, dDateTimeFrom, dDateTimeTo, ref xml), true))
                    {
                        sn.User.DsDrivers = null;
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Failed to get HOS for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        return;
                    }
            }


            if (xml == "")
            {
                sn.User.DsDrivers = null;
                return;
            }

            StringReader strrXML = new StringReader(xml);
            DataSet ds = new DataSet();
            strrXML = new StringReader(xml);
            ds.ReadXmlSchema(strPath);
            ds.ReadXml(strrXML);
            sn.User.DsDrivers = ds;
            dgHOS.DataSource = ds;
            dgHOS.DataBind();


        }

        // Changes for TimeZone Feature end

        private void dsDrivers_Fill(Int32 driverId, string dateHOS)
        {
            //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
            //string strPath = MapPath(DataSetPath) + @"\HOSinfo.xsd";

            string strPath = Server.MapPath("../Datasets/HOSinfo.xsd");


            string xml = "";
            DateTime dDateTimeFrom = Convert.ToDateTime(dateHOS + " " + "00:00").AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);
            DateTime dDateTimeTo = Convert.ToDateTime(dateHOS + " " + "00:00").AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).AddDays(1) ;

            using (ServerDBDriver.DBDriver drv = new global::SentinelFM.ServerDBDriver.DBDriver())
            {


                if (objUtil.ErrCheck(drv.GetHOSbyDateTime(sn.UserID, sn.SecId, driverId, dDateTimeFrom, dDateTimeTo, ref xml), false))
                    if (objUtil.ErrCheck(drv.GetHOSbyDateTime(sn.UserID, sn.SecId, driverId, dDateTimeFrom, dDateTimeTo, ref xml), true))
                    {
                        sn.User.DsDrivers = null;
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Failed to get HOS for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        return;
                    }
            }


            if (xml == "")
            {
                sn.User.DsDrivers = null;
                return;
            }

            StringReader strrXML = new StringReader(xml);
            DataSet ds = new DataSet();
            strrXML = new StringReader(xml);
            ds.ReadXmlSchema(strPath);
            ds.ReadXml(strrXML);
            sn.User.DsDrivers = ds;
            dgHOS.DataSource = ds;
            dgHOS.DataBind();  
          

        }
    }
}
