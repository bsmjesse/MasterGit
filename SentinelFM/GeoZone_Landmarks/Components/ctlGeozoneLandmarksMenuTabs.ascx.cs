using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentinelFM.Components
{
    public partial class GeoZone_Landmarks_Components_ctlGeozoneLandmarksMenuTabs : BaseControl
    {
        public String SelectedControl = string.Empty;
        private SentinelFMSession sn;
        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (!string.IsNullOrEmpty(SelectedControl))
            {
                if (FindControl(SelectedControl) is Button)
                {
                    ((Button)FindControl(SelectedControl)).CssClass = "selectedbutton";
                    ((Button)FindControl(SelectedControl)).OnClientClick = "javascript:return false;";
                }
            }

            //if (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 952 || sn.User.OrganizationId == 999952 || sn.User.OrganizationId == 999954)
            //if ((sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2 || sn.User.UserGroupId == 7) && (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 952 || sn.User.OrganizationId == 999952 || sn.User.OrganizationId == 999954)) //Hgi and Security Administrator user 
            //{
            //    cmdFleetGeozone.Visible = true;
            //    cmdFleetLandmark.Visible = true;
            //}

            //if (sn.UserID == 11967)
            //{
            //    cmdFleetLandmark.Visible = true;
            //}

            //if (sn.User.UserGroupId == 36 && sn.User.OrganizationId == 952)
            //{
            //    cmdGeoZone.Visible = false;
            //    cmdVehicleGeoZone.Visible = false;
            //    cmdWaypoints.Visible = false;
            //    cmdMap.Visible = false;
            //    cmdFleetGeozone.Visible = false;
            //    cmdFleetLandmark.Visible = false;
            //    return;
            //}

            //if (sn.User.UserGroupId == 27)
            //{
            //    cmdGeoZone.Enabled = false;
            //    return;
            //}

            //if (sn.User.UserGroupId == 28)
            //    cmdGeoZone.Enabled = false;

            if (!sn.User.ControlEnable(sn, 102))
                cmdLandmark.Visible = false;
            if (!sn.User.ControlEnable(sn, 103))
                cmdGeoZone.Visible = false;
            if (!sn.User.ControlEnable(sn, 104))
                cmdVehicleGeoZone.Visible = false;
            if (!sn.User.ControlEnable(sn, 105))
                cmdMap.Visible = false;
            if (!sn.User.ControlEnable(sn, 106))
                cmdFleetGeozone.Visible = false;
            if (!sn.User.ControlEnable(sn, 107))
                cmdFleetLandmark.Visible = false;
        }

        protected void cmdLandmark_Click(object sender, System.EventArgs e)
        {
            try
            {
                sn.GeoZone.ImgConfPath = "";
                sn.Landmark.AddLandmarkMode = false;
                sn.Landmark.EditLandmarkMode = false;
                sn.GeoZone.AddMode = false;
                sn.GeoZone.EditMode = false;
                Response.Redirect("~/GeoZone_Landmarks/frmLandmark.aspx");
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

        }

        protected void cmdGeoZone_Click(object sender, System.EventArgs e)
        {
            try
            {
                sn.GeoZone.ImgConfPath = "";
                sn.Landmark.AddLandmarkMode = false;
                sn.Landmark.EditLandmarkMode = false;
                sn.GeoZone.AddMode = false;
                sn.GeoZone.EditMode = false;
                Response.Redirect("~/GeoZone_Landmarks/frmGeoZone.aspx");
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

        }

        protected void cmdVehicleGeoZone_Click(object sender, System.EventArgs e)
        {
            try
            {
                sn.Landmark.AddLandmarkMode = false;
                sn.Landmark.EditLandmarkMode = false;
                sn.GeoZone.AddMode = false;
                sn.GeoZone.EditMode = false;
                sn.Cmd.BoxId = 0;
                Response.Redirect("~/GeoZone_Landmarks/frmVehicleGeoZoneAss.htm");
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

        }

        protected void cmdWaypoints_Click(object sender, EventArgs e)
        {
            sn.GeoZone.ImgConfPath = "";
            sn.Landmark.AddLandmarkMode = false;
            sn.Landmark.EditLandmarkMode = false;
            sn.GeoZone.AddMode = false;
            sn.GeoZone.EditMode = false;
            Response.Redirect("~/GeoZone_Landmarks/frmWaypoint.aspx");
        }

        protected void cmdMap_Click(object sender, System.EventArgs e)
        {
            try
            {
                sn.Landmark.AddLandmarkMode = false;
                sn.Landmark.EditLandmarkMode = false;
                sn.GeoZone.AddMode = false;
                sn.GeoZone.EditMode = false;
                sn.Map.XInCoord = 0;
                sn.Map.YInCoord = 0;
                //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                //{
                //   sn.MapSolute.LoadDefaultMapWithoutLandmarks(sn);
                //   Response.Redirect("frmMap_land_geo_mapsolute.aspx");
                //}
                if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                {
                    Response.Redirect("~/GeoZone_Landmarks/frmMap_Landmark_GeoZoneVE.aspx");
                }
                else
                    Response.Redirect("~/GeoZone_Landmarks/frmMap_Landmark_GeoZone.aspx");
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

        }

        protected void cmdFleetGeozone_Click(object sender, EventArgs e)
        {
            sn.GeoZone.ImgConfPath = "";
            sn.Landmark.AddLandmarkMode = false;
            sn.Landmark.EditLandmarkMode = false;
            sn.GeoZone.AddMode = false;
            sn.GeoZone.EditMode = false;
            Response.Redirect("~/GeoZone_Landmarks/frmFleetGeozone.aspx");
        }
        protected void cmdFleetLandmark_Click(object sender, EventArgs e)
        {
            try
            {
                sn.GeoZone.ImgConfPath = "";
                sn.Landmark.AddLandmarkMode = false;
                sn.Landmark.EditLandmarkMode = false;
                sn.GeoZone.AddMode = false;
                sn.GeoZone.EditMode = false;
                Response.Redirect("~/GeoZone_Landmarks/frmFleetLandmark.aspx");
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
        }


    }
}