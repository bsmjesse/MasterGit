using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using VLF.ERRSecurity;
using VLF.Reports;
using System.Configuration;
using System.Text;
using VLF.CLS;

namespace SentinelFM
{
    public partial class Reports_frmReport : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void cboReports_SelectedIndexChanged(object sender, System.EventArgs e)
        { }
        protected void chkWeekend_CheckedChanged(object sender, System.EventArgs e)
        { }
        protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
        { }

        /// <summary>
        /// Get user reports dataset from session, if not valid - use web method
        /// </summary>
        private void GetUserReportsTypes()
        {
            string xml = "";

            DataSet dsReports = new DataSet();
            using (ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem())
            {
                if (objUtil.ErrCheck(dbs.GetUserReportsByLang(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbs.GetUserReportsByLang(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No GUI Controls setttngs for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        return;
                    }
            }

            if (String.IsNullOrEmpty(xml))
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "No GUI Controls settings for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                return;
            }


            dsReports.ReadXml(new StringReader(xml));
            //dsReports.Tables[0].DefaultView.Sort = "GUIName";   
            this.cboReports.DataSource = dsReports; //dsReports.Tables[0].DefaultView;
            sn.Report.UserReportsDataSet = dsReports;


            //}

            this.cboReports.DataBind();

            //if (sn.User.UserGroupId != 1)
            //{
            //    cboReports.Items.Add(new ListItem("Activity Summary Report for Organization", "38"));
            //    cboReports.Items.Add(new ListItem("Activity Summary Report per Vehicle", "39"));
            //}

        }
    }
}