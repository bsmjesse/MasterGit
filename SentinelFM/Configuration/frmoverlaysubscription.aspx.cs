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

    public partial class Configuration_frmoverlaysubscription : SentinelFMBasePage
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        public bool AllOrganizationOverlayLayersSettingreadonly = true;

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

            if (sn.User.ControlEnable(sn, 96))
                AllOrganizationOverlayLayersSettingreadonly = false;

            VLF.PATCH.Logic.MapLayersManager ml = new VLF.PATCH.Logic.MapLayersManager(sConnectionString);
            DataSet allOverlays;
            string defaultAvailableOverlays = string.Empty;
            string defaultOverlays = string.Empty;
            string premiumAvailableOverlays = string.Empty;
            string premiumOverlays = string.Empty;
            string overlayVisibility = string.Empty;

            string t = targetName.Value;

            if (Page.IsPostBack && sn.User.OrganizationId > 0)
            {
                bool isHgiUser = (sn != null && sn.User.UserGroupId == 1) ? true : false;

                //if (t != "cmdpushsettings" && t != "cmdSettings" && t != "cmdFuel" && t != "cmdPanicMangement" && t != "cmdMapSubscription")
                if (t == "overlaysubscription")
                {
                    if (isHgiUser || sn.User.ControlEnable(sn, 95))
                        ml.UpdateDefaultMaplayers("Overlay", selectedDefaultOverlays.Value);

                    ml.UpdatePremiumMaplayers("Overlay", sn.User.OrganizationId, selectedPremiumOverlays.Value);
                    ml.UpdateOverlayVisibility(selectedVisibleOverlays.Value);
                }
            }

            targetName.Value = "";

            allOverlays = ml.GetAllMapLayersWithDefaultPremiumByOrganizationID("Overlay", sn.User.OrganizationId);

            foreach (DataRow dr in allOverlays.Tables[0].Rows)
            {
                if (Convert.ToBoolean(dr["Default"].ToString()))
                {
                    defaultOverlays += string.Format("{{ name: \"{0}\", layerId: \"{1}\" }}", dr["Description"], dr["MapLayerID"]) + ",";
                }
                else
                {
                    defaultAvailableOverlays += string.Format("{{ name: \"{0}\", layerId: \"{1}\" }}", dr["Description"], dr["MapLayerID"]) + ",";
                }

                if (Convert.ToBoolean(dr["Premium"].ToString()))
                {
                    premiumOverlays += string.Format("{{ name: \"{0}\", layerId: \"{1}\" }}", dr["Description"], dr["MapLayerID"]) + ",";
                }
                else
                {
                    premiumAvailableOverlays += string.Format("{{ name: \"{0}\", layerId: \"{1}\" }}", dr["Description"], dr["MapLayerID"]) + ",";
                }

                overlayVisibility += string.Format("{{ name: \"{0}\", layerId: \"{1}\", visibility: \"{2}\" }}", dr["Description"], dr["MapLayerID"], dr["Visibility"].ToString()) + ",";

            }
            if (defaultAvailableOverlays != string.Empty)
                defaultAvailableOverlays = defaultAvailableOverlays.Substring(0, defaultAvailableOverlays.Length - 1);
            if (defaultOverlays != string.Empty)
                defaultOverlays = defaultOverlays.Substring(0, defaultOverlays.Length - 1);
            if (premiumOverlays != string.Empty)
                premiumOverlays = premiumOverlays.Substring(0, premiumOverlays.Length - 1);
            if (premiumAvailableOverlays != string.Empty)
                premiumAvailableOverlays = premiumAvailableOverlays.Substring(0, premiumAvailableOverlays.Length - 1);
            if (overlayVisibility != string.Empty)
                overlayVisibility = overlayVisibility.Substring(0, overlayVisibility.Length - 1);

            defaultOverlaysHidden.Value = "[" + defaultOverlays + "]";
            defaultAvailableOverlaysHidden.Value = "[" + defaultAvailableOverlays + "]";

            premiumOverlaysHidden.Value = "[" + premiumOverlays + "]";
            premiumAvailableOverlaysHidden.Value = "[" + premiumAvailableOverlays + "]";

            overlayVisibilityDataHidden.Value = "[" + overlayVisibility + "]";
        }

        new public void RedirectToLogin()
        {
            Session.Abandon();
            Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('../Login.aspx','_top')</SCRIPT>");
            return;
        }

        
    }
}