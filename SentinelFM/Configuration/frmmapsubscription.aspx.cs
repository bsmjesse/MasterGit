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
using System.Text;
using VLF.CLS;
using Telerik.Web.UI;
using System.Drawing;
using VLF.DAS.Logic;
using System.Collections.Generic;
using VLF.DAS.DB;
using VLF.PATCH.Logic;

namespace SentinelFM
{
    public partial class Configuration_frmmapsubscription : SentinelFMBasePage
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

        public int VehicleGridPagesize = 100;
        public int HistoryGridPagesize = 100;
        public int HistoryGridNormalPagesize = 200;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                RedirectToLogin();
                return;
            }

            if (!Page.IsPostBack)
            {
                
                GuiSecurity(this);

            }

            VLF.PATCH.Logic.MapLayersManager ml = new VLF.PATCH.Logic.MapLayersManager(sConnectionString);
            VLF.PATCH.Logic.PatchGridPagesize ps = new VLF.PATCH.Logic.PatchGridPagesize(sConnectionString);
            DataSet allLayers;
            string defaultAvailableLayers = string.Empty;
            string defaultLayers = string.Empty;
            string premiumAvailableLayers = string.Empty;
            string premiumLayers = string.Empty;

            string t = targetName.Value;

            if (Page.IsPostBack && sn.User.OrganizationId > 0)
            {

                //if (t != "cmdpushsettings" && t != "cmdSettings" && t != "cmdFuel" && t != "cmdPanicMangement" && t != "cmdOverlaySubscription")
                if (t == "mapsubscription")
                {
                    ml.UpdateDefaultMaplayers(selectedDefaultLayers.Value);
                    ml.UpdatePremiumMaplayers(sn.User.OrganizationId, selectedPremiumLayers.Value);

                    int vehiclegridpagesize = 0;
                    int.TryParse(Request["vehicleGridPageSize"], out vehiclegridpagesize);
                    if (vehiclegridpagesize > 0)
                        ps.UpdateInsertPagesize(sn.User.OrganizationId, "vehiclegridpagesize", vehiclegridpagesize);

                    int historygridpagesize = 0;
                    int.TryParse(Request["historyGridPageSize"], out historygridpagesize);
                    if (historygridpagesize > 0)
                        ps.UpdateInsertPagesize(sn.User.OrganizationId, "historygridpagesize", historygridpagesize);

                    int historygridnormalpagesize = 0;
                    int.TryParse(Request["historyGridNormalPageSize"], out historygridnormalpagesize);
                    if (historygridnormalpagesize > 0)
                        ps.UpdateInsertPagesize(sn.User.OrganizationId, "historygridnormalpagesize", historygridnormalpagesize);
                }
            }

            targetName.Value = "";

            allLayers = ml.GetAllMapLayersWithDefaultPremiumByOrganizationID(sn.User.OrganizationId);
            
            foreach (DataRow dr in allLayers.Tables[0].Rows)
            {
                if (Convert.ToBoolean(dr["Default"].ToString()))
                {
                    defaultLayers += string.Format("{{ name: \"{0}\", layerId: \"{1}\", description: \"{2}\" }}", dr["LayerName"], dr["MapLayerID"], dr["Description"]) + ",";
                }
                else
                {
                    defaultAvailableLayers += string.Format("{{ name: \"{0}\", layerId: \"{1}\", description: \"{2}\" }}", dr["LayerName"], dr["MapLayerID"], dr["Description"]) + ",";
                }

                if (Convert.ToBoolean(dr["Premium"].ToString()))
                {
                    premiumLayers += string.Format("{{ name: \"{0}\", layerId: \"{1}\", description: \"{2}\" }}", dr["LayerName"], dr["MapLayerID"], dr["Description"]) + ",";
                }
                else
                {
                    premiumAvailableLayers += string.Format("{{ name: \"{0}\", layerId: \"{1}\", description: \"{2}\" }}", dr["LayerName"], dr["MapLayerID"], dr["Description"]) + ",";
                }
            }
            if (defaultAvailableLayers != string.Empty)
                defaultAvailableLayers = defaultAvailableLayers.Substring(0, defaultAvailableLayers.Length - 1);
            if (defaultLayers != string.Empty)
                defaultLayers = defaultLayers.Substring(0, defaultLayers.Length - 1);
            if (premiumLayers != string.Empty)
                premiumLayers = premiumLayers.Substring(0, premiumLayers.Length - 1);
            if (premiumAvailableLayers != string.Empty)
                premiumAvailableLayers = premiumAvailableLayers.Substring(0, premiumAvailableLayers.Length - 1);

            defaultLayersHidden.Value = "[" + defaultLayers + "]";
            defaultAvailableLayersHidden.Value = "[" + defaultAvailableLayers + "]";

            premiumLayersHidden.Value = "[" + premiumLayers + "]";
            premiumAvailableLayersHidden.Value = "[" + premiumAvailableLayers + "]";

            DataSet pagesizes = ps.GetPagesizeSettingsByOrganizationId(sn.User.OrganizationId);
            foreach (DataRow dr in pagesizes.Tables[0].Rows)
            {
                if (dr["Type"].ToString().ToLower() == "vehiclegridpagesize")
                {
                    VehicleGridPagesize = Convert.ToInt32(dr["PageSize"].ToString());
                }
                else if (dr["Type"].ToString().ToLower() == "historygridpagesize")
                {
                    HistoryGridPagesize = Convert.ToInt32(dr["PageSize"].ToString());
                }
                else if (dr["Type"].ToString().ToLower() == "historygridnormalpagesize")
                {
                    HistoryGridNormalPagesize = Convert.ToInt32(dr["PageSize"].ToString());
                }
            }
        }
                
        new public void RedirectToLogin()
        {
            Session.Abandon();
            Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('../Login.aspx','_top')</SCRIPT>");
            return;
        }
        
}
}