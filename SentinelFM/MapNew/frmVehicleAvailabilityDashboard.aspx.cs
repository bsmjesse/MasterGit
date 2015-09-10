using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using VLF.PATCH.Logic;
using System.Globalization;
using System.IO;

namespace SentinelFM
{

    public partial class frmVehicleAvailabilityDashboard : System.Web.UI.Page
    {
        public string FleetName = "";
        public int FleetId = 0;
        public int LandmarkCategoryId = 0;
        public string NodeCode = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

            using (VLF.DAS.Logic.Fleet dbFleet = new VLF.DAS.Logic.Fleet(sConnectionString))
            {
                //int FleetId = 0;
                string fid = "";
                if (!string.IsNullOrEmpty(Request["FleetId"]))
                {
                    fid = Request["FleetId"].ToString().Split(',')[0];
                }
                fid = fid.Split(',')[0];
                int.TryParse(fid, out FleetId);
                int.TryParse(Request["LandmarkCategoryId"], out LandmarkCategoryId);
                SentinelFMSession sn = (SentinelFMSession)Session["SentinelFMSession"];

                DataSet dsResult = dbFleet.GetFleetInfoByFleetId(FleetId);
                DataTable oneDT = dsResult.Tables[0];
                if (oneDT.Rows.Count > 0)
                {
                    FleetName = oneDT.Rows[0]["FleetName"].ToString();
                }

                if (sn.User.LoadVehiclesBasedOn == "fleet")
                {
                    trBasedOnNormalFleet.Visible = true;
                    trBasedOnHierarchyFleet.Visible = false;
                }
                else
                {
                    trBasedOnNormalFleet.Visible = false;
                    trBasedOnHierarchyFleet.Visible = true;

                    NodeCode = oneDT.Rows[0]["NodeCode"].ToString();
                    btnOrganizationHierarchyNodeCode.Text = FleetName;
                }
            }

            if (!Page.IsPostBack)
            {
                CboFleet_Fill();                
            }
        }

        private void CboFleet_Fill()
        {
            SentinelFMSession sn = (SentinelFMSession)Session["SentinelFMSession"];
            clsUtility objUtil;
            objUtil = new clsUtility(sn);
            
            try
            {
                ServerDBFleet.DBFleet dbf;
                DataSet dsFleets = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                dbf = new ServerDBFleet.DBFleet();
                if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        cboFleet.DataSource = null;
                        return;
                    }
                strrXML = new StringReader(xml);
                dsFleets.ReadXml(strrXML);
                DataView FleetView = dsFleets.Tables[0].DefaultView;
                FleetView.RowFilter = "FleetType<>'oh'";
                //cboFleet.DataSource=dsFleets  ;
                cboFleet.DataSource = FleetView;
                cboFleet.DataBind();
                cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectFleet"), "-1"));
                //cboFleet.SelectedValue = FleetId.ToString();
                cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(FleetId.ToString()));
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
