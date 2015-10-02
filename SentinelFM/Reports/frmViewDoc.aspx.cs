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
using Telerik.Web.UI;
using System.Drawing;
namespace SentinelFM
{

    public partial class Reports_frmViewDoc : SentinelFMBasePage
    {
        public string messageUrl = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string id = Request.QueryString["ID"];

                VLF3.Services.ActiveState.ReportRepositoryService rrs = VLF3.Services.ActiveState.ReportRepositoryService.GetInstance(sn.UserID);

                VLF3.Domain.ActiveState.ReportRepository reportRepository = rrs.GetReportRepositoryById(long.Parse(id));
                bool isRightUser = false;
                if (reportRepository != null)
                {
                    if (reportRepository.UserId == sn.UserID) { isRightUser = true; }
                }

                if (!isRightUser) RedirectToLogin(); 
                else
                {
                    if (!string.IsNullOrEmpty(reportRepository.Path))
                    {
                        messageUrl = reportRepository.Path + "?#zoom=100";
                    }
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
                RedirectToLogin();
            }
        }
    }
}