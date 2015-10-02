using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace SentinelFM
{
    public partial class Widgets_fleet : SentinelFMBasePage
    {
        public string FleetId = string.Empty;
        public string Caller = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            FleetId = Request["fleetId"];
            Caller = Request["f"];

            if (FleetId == null || FleetId == string.Empty)
                FleetId = "-1";

            if (!Page.IsPostBack)
            {
                CboFleet_Fill();
             
            }
        }

        private void CboFleet_Fill()
        {
            try
            {

                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();
                cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));

                //cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindItemByValue(sn.Report.GuiId.ToString()));
                cboFleet.SelectedValue = FleetId;
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }
    }
}