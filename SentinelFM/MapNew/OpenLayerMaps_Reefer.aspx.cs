using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Data;
using System.Configuration;
using VLF.PATCH.Logic;

namespace SentinelFM
{
    public partial class MapNew_OpenLayerMaps_Reefer : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;
        public bool ISSECURE = HttpContext.Current.Request.IsSecureConnection;

        public bool ifShowArcgis = false;
        public bool ifShowGoogleStreets = false;
        public bool ifShowGoogleHybrid = false;
        public bool ifShowBingRoads = false;
        public bool ifShowBingHybrid = false;
        public bool ifShowAerial = false;

        public bool ShowAssignToFleet = false;
        public bool ShowMapHistorySearch = false;
        public bool ShowRouteAssignment = false;

        public bool ShowCallTimer = false;
        public string CallTimerSelections = string.Empty;

        public string LastUpdatedOpenLayerMapJs = System.IO.File.GetLastWriteTime(System.Web.HttpContext.Current.Server.MapPath("~/Scripts/OpenLayerMap/OpenLayerMap_Reefer.js")).ToString("yyyyMMddHHmmss");

        protected void Page_Load(object sender, EventArgs e)
        {

            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            VLF.PATCH.Logic.MapLayersManager ml = new VLF.PATCH.Logic.MapLayersManager(sConnectionString);
            DataSet allLayers;

            allLayers = ml.GetAllMapLayersWithDefaultPremiumByOrganizationID(sn.User.OrganizationId);

            ShowAssignToFleet = clsPermission.FeaturePermissionCheck(sn, "GeozoneFleetAssignment");
            ShowMapHistorySearch = clsPermission.FeaturePermissionCheck(sn, "MapAddressHistorySearch");
            ShowRouteAssignment = clsPermission.FeaturePermissionCheck(sn, "RouteAssignment");
            //if ((sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2 || sn.User.UserGroupId == 7) && (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 952 || sn.User.OrganizationId == 999952 || sn.User.OrganizationId == 999954)) //Hgi and Security Administrator user 
            //{
            //    ShowAssignToFleet = true;
            //}

            //if (sn.UserID == 11967)
            //{
            //    ShowAssignToFleet = true;
            //}

            if ((sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2 || sn.User.UserGroupId == 7 || sn.UserID == 11967) && (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 952 || sn.User.OrganizationId == 999952 || sn.User.OrganizationId == 999954)) //Hgi and Security Administrator user 
            {
                //enableTimer = true;
                //trCallTimer.Visible = true;
                ShowCallTimer = true;
                if (!Page.IsPostBack)
                    CboServices_Fill();
            }

            foreach (DataRow dr in allLayers.Tables[0].Rows)
            {
                if (Convert.ToBoolean(dr["Default"].ToString()) || Convert.ToBoolean(dr["Premium"].ToString()))
                {
                    if (dr["LayerName"].ToString().Trim() == "Arcgis")
                        ifShowArcgis = true;
                    else if (dr["LayerName"].ToString().Trim() == "Google Streets")
                        ifShowGoogleStreets = true;
                    else if (dr["LayerName"].ToString().Trim() == "Google Hybrid")
                        ifShowGoogleHybrid = true;
                    else if (dr["LayerName"].ToString().Trim() == "Bing Roads")
                        ifShowBingRoads = true;
                    else if (dr["LayerName"].ToString().Trim() == "Bing Hybrid")
                        ifShowBingHybrid = true;
                    else if (dr["LayerName"].ToString().Trim() == "Aerial")
                        ifShowAerial = true;
                }
            }

            if (!Page.IsPostBack)
            {

            }
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
        }



        public void RedirectToLogin()
        {
            Session.Abandon();
            Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('../Login.aspx','_top')</SCRIPT>");
            return;
        }

        private void CboServices_Fill()
        {
            try
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                VLF.PATCH.Logic.PatchServices _ps = new VLF.PATCH.Logic.PatchServices(sConnectionString);
                DataSet dsServices = new DataSet();
                dsServices = _ps.GetHardcodedCallTimerServices();
                /*cboServices.DataSource = dsServices;
                cboServices.DataBind();

                //cboFleet.Items.Insert(0, new ListItem((string)this.GetLocalResourceObject("cboFleet_Item_0"), "-1"));
                cboServices.Items.Insert(0, new ListItem("Select a service", "-1"));

                setDefaultCallTimer();*/
                CallTimerSelections = "<select id=\"cboServices\" class=\"RegularText\" name=\"cboServices\">";
                CallTimerSelections += "    <option value=\"-1\">Select a service</option>";
                foreach (DataRow dr in dsServices.Tables[0].Rows)
                {
                    CallTimerSelections += "    <option value=\"" + dr["ServiceConfigId"].ToString() + "\">" + dr["RulesApplied"].ToString() + "</option>";
                }
                CallTimerSelections += "</select>";
                CallTimerSelections = CallTimerSelections.Replace("\"", "\\\"");


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