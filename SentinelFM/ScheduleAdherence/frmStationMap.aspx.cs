using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using VLF.DAS.Logic;
using SentinelFM;

public partial class ScheduleAdherence_frmStationMap : System.Web.UI.Page
{
    protected SentinelFMSession sn;
    public bool ISSECURE = HttpContext.Current.Request.IsSecureConnection;

    public bool ifShowTheme1 = false;
    public bool ifShowTheme2 = false;
    public bool ifShowArcgis = false;
    public bool ifShowGoogleStreets = false;
    public bool ifShowGoogleHybrid = false;
    public bool ifShowBingRoads = false;
    public bool ifShowBingHybrid = false;
    public bool ifShowNavteq = false;
    public bool ifShowNavteqHybrid = false;
    protected void Page_Load(object sender, EventArgs e)
    {
        sn = Session["SentinelFMSession"] as SentinelFMSession;
        if (sn == null)
        {
            Response.Redirect("../Login.aspx");
            return;
        }
        int organizationId = sn.User.OrganizationId;
        if (organizationId == 0)
        {
            Response.Redirect("../Login.aspx");
        }
        GetLayerInfo();
        if (!IsPostBack)
        {
            GetDefaultMapView();
            if (!HaveSASetting())
            {
                Response.Redirect("frmReasonCodeList.aspx");
            }
        }
    }

    private bool HaveSASetting()
    {
        int organizationId = sn.User.OrganizationId;
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet ds = db.GetSASetting(organizationId);
        if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            return false;
        else
            return true;
    }

    private void GetLayerInfo()
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        VLF.PATCH.Logic.MapLayersManager ml = new VLF.PATCH.Logic.MapLayersManager(sConnectionString);
        DataSet allLayers;

        allLayers = ml.GetAllMapLayersWithDefaultPremiumByOrganizationID(sn.User.OrganizationId);
        foreach (DataRow dr in allLayers.Tables[0].Rows)
        {
            if (Convert.ToBoolean(dr["Default"].ToString()) || Convert.ToBoolean(dr["Premium"].ToString()))
            {
                if (dr["LayerName"].ToString().Trim() == "Theme 1")
                    ifShowTheme1 = true;
                else if (dr["LayerName"].ToString().Trim() == "Theme 2")
                    ifShowTheme2 = true;
                else if (dr["LayerName"].ToString().Trim() == "Arcgis")
                    ifShowArcgis = true;
                else if (dr["LayerName"].ToString().Trim() == "Google Streets")
                    ifShowGoogleStreets = true;
                else if (dr["LayerName"].ToString().Trim() == "Google Hybrid")
                    ifShowGoogleHybrid = true;
                else if (dr["LayerName"].ToString().Trim() == "Bing Roads")
                    ifShowBingRoads = true;
                else if (dr["LayerName"].ToString().Trim() == "Bing Hybrid")
                    ifShowBingHybrid = true;
                else if (dr["LayerName"].ToString().Trim() == "Navteq")
                    ifShowNavteq = true;
                else if (dr["LayerName"].ToString().Trim() == "NavteqHybrid")
                    ifShowNavteqHybrid = true;
            }
        }
    }

    //Added By Devin for default map view
    void GetDefaultMapView()
    {
        if (sn.MapCenter == null && sn.MapZoomLevel == null)
        {
            clsUtility objUtil;
            objUtil = new clsUtility(sn);
            string xml = "";
            DataSet dsPref = new DataSet();
            System.IO.StringReader strrXML = null;
            using (SentinelFM.ServerDBUser.DBUser dbu = new SentinelFM.ServerDBUser.DBUser())
            {
                if (objUtil.ErrCheck(dbu.GetUserPreferencesXML_ByUserId(sn.UserID, sn.SecId, sn.UserID, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserPreferencesXML_ByUserId(sn.UserID, sn.SecId, sn.UserID, ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    return;
                }

                strrXML = new System.IO.StringReader(xml);
                dsPref.ReadXml(strrXML);
                foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                {

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == 41)// 41 is Map center ID in [vlfPreference] table
                    {
                        if (rowItem["PreferenceValue"].ToString().Trim() != "")
                        {
                            sn.MapCenter = rowItem["PreferenceValue"].ToString();
                        }
                    }
                    if (Convert.ToInt16(rowItem["PreferenceId"]) == 42)  // 42 is Map center ID in [vlfPreference] table
                    {
                        if (rowItem["PreferenceValue"].ToString().Trim() != "")
                        {
                            sn.MapZoomLevel = rowItem["PreferenceValue"].ToString();
                        }
                    }

                }
            }

            if (sn.MapCenter == null && sn.MapZoomLevel == null)
            {

                using (SentinelFM.ServerDBOrganization.DBOrganization dbOrganization = new SentinelFM.ServerDBOrganization.DBOrganization())
                {
                    if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                        if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                        {
                            return;
                        }
                    if (xml == "")
                    {
                        return;
                    }
                    dsPref = new DataSet();
                    strrXML = new System.IO.StringReader(xml);
                    dsPref.ReadXml(strrXML);
                    foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                    {

                        if (Convert.ToInt16(rowItem["OrgPreferenceId"]) == 15)// 15 is Map center ID in [vlfOrganizationSettingsTypes] table
                        {
                            if (rowItem["PreferenceValue"].ToString().Trim() != "")
                            {
                                sn.MapCenter = rowItem["PreferenceValue"].ToString();
                            }
                        }
                        if (Convert.ToInt16(rowItem["OrgPreferenceId"]) == 16) // 16 is Zoom Level in [vlfOrganizationSettingsTypes] table
                        {
                            if (rowItem["PreferenceValue"].ToString().Trim() != "")
                            {
                                sn.MapZoomLevel = rowItem["PreferenceValue"].ToString();
                            }
                        }

                    }
                }
            }

            if (sn.MapCenter == null) sn.MapCenter = "";
            if (sn.MapZoomLevel == null) sn.MapZoomLevel = "";
        }
    }

    protected string ReportDB_ConnectionString
    {
        get
        {
            return ConfigurationManager.ConnectionStrings["DWH"].ConnectionString;
        }
    }
}