using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.IO;
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace SentinelFM
{
    public partial class MapNew_frmDrawMap : SentinelMapBasePage
    {
        public string errorSave = "Save failed.";
        public string drawCircleText ="";// "Holding down the control (Ctrl) key, click the mouse button to center the circle, release control (Ctrl) key and move the mouse to draw the Circle. Release the mouse button to pan the map.";

        public string drawRectangleText ="";// "Holding down the control (Ctrl) key, click to draw the geozone fence. Release the control key to pan the map." +
               //"Click at each of two corners, following the diagonal of the rectangle.";
        public string drawPolygon ="";// "Holding down the control (Ctrl) key, click to draw the geozone fence. Release the control key to pan the map." +
              //"Click at each corner of the polygon, clockwise.";

        public string errorCircle = "Please draw a circle.";
        public string MaximunPoints = "50";
        public string MinimumPointsRectangleError = string.Empty;
        public string MinimumPointsPolygonError = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {

            drawRectangleText = (string)GetLocalResourceObject("drawRectangleText");
            drawPolygon = (string)GetLocalResourceObject("drawPolygon");
            drawCircleText = (string)GetLocalResourceObject("drawCircleText");

            if (!IsPostBack)
            {
                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                GuiSecurity(this);

                if (sn.Landmark.AddLandmarkMode)
                {
                    this.cmdShowEditGeoZone.Visible = false;
                    this.tblGeoZone.Visible = true;
                }

                if (sn.Landmark.EditLandmarkMode)
                {
                    this.cmdShowEditGeoZone.Visible = true;
                    this.tblGeoZone.Visible = false;
                    cmdSave.Visible = false;
                }

                if (sn.Landmark.LandmarkType == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                    LandmarkOptions.SelectedValue = "1";
                if (sn.Landmark.LandmarkType == VLF.CLS.Def.Enums.GeozoneType.Polygon)
                    LandmarkOptions.SelectedValue = "2";
                if (sn.Landmark.LandmarkType == VLF.CLS.Def.Enums.GeozoneType.None)
                    LandmarkOptions.SelectedValue = "0";

            }
            MinimumPointsRectangleError = GetScriptEscapeString((string)base.GetLocalResourceObject("lblMessage_Text_MinimumPointsRectangleError"));
            MinimumPointsPolygonError = GetScriptEscapeString((string)base.GetLocalResourceObject("lblMessage_Text_MinimumPointsPolygonError"));
        }


        protected void cmdCancel_Click(object sender, System.EventArgs e)
        {
            RadAjaxManager1.ResponseScripts.Add("window.close();");
        }

        //protected void cmdSave_Click(object sender, System.EventArgs e)
        //{

        //    string strJavaScript = "";

        //    sn.GeoZone.SetGeoZone = false;

        //    if (sn.Landmark.RefreshFormName == "Landmark")
        //    {
        //        strJavaScript += "<script language='javascript'>";
        //        strJavaScript += " window.opener.document.forms[0].txtX.value=" + sn.Landmark.X.ToString() + ";";
        //        strJavaScript += " window.opener.document.forms[0].txtY.value=" + sn.Landmark.Y.ToString() + ";";
        //        strJavaScript += " self.close();</script>";
        //        Response.Write(strJavaScript);

        //    }
        //    if (sn.Landmark.RefreshFormName == "Geozone")
        //    {

        //        strJavaScript += "<script language='javascript'>";
        //        strJavaScript += " window.opener.document.forms[0].txtMapMessage.value='" + (string)base.GetLocalResourceObject("txtMapMessage_Value_OnWindowOpen") + "';";
        //        strJavaScript += " self.close();</script>";
        //        Response.Write(strJavaScript);

        //    }

        //}
        protected void cmdShowEditGeoZone_Click(object sender, EventArgs e)
        {
            this.tblGeoZone.Visible = true;
            sn.GeoZone.ShowEditGeoZoneTable = true;
            cmdSave.Visible = true;
            cmdShowEditGeoZone.Visible = false;
            ResetControl();
        }

        private void ResetControl()
        {
            if (hidStartDraw.Value == "1")
            {
                cmdDrawGeoZone.Enabled = false;
                if (HidDrawType.Value == "1") lblMessage.Text = drawRectangleText;
                if (HidDrawType.Value == "2") lblMessage.Text = drawPolygon;
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (sn.Landmark.DsLandmarkDetails == null)
                {
                    sn.Landmark.DsLandmarkDetails = new DataTable();
                    
                    sn.Landmark.DsLandmarkDetails.Columns.Add("Latitude", typeof(System.Double));
                    sn.Landmark.DsLandmarkDetails.Columns.Add("Longitude", typeof(System.Double));
                }
                sn.Landmark.DsLandmarkDetails.Rows.Clear();
                if (hidPoints.Value.Trim() != string.Empty)
                {
                    string[] points = hidPoints.Value.Trim().Split(',');
                    if (LandmarkOptions.SelectedValue == "1" || LandmarkOptions.SelectedValue == "2")
                    {
                        double minX = 999999;
                        double maxX = -999999;
                        double minY = 999999;
                        double maxY = -999999;

                        foreach (string myPoints in points)
                        {
                            if (myPoints.Trim() != string.Empty)
                            {
                                string[] latLon = myPoints.Split('|');
                                DataRow dr1 = sn.Landmark.DsLandmarkDetails.NewRow();
                                dr1["Latitude"] = double.Parse(latLon[0]);
                                dr1["Longitude"] = double.Parse(latLon[1]);
                                if (minY > (double)dr1["Latitude"]) minY = (double)dr1["Latitude"];
                                if (maxY < (double)dr1["Latitude"]) maxY = (double)dr1["Latitude"];

                                if (minX > (double)dr1["Longitude"]) minX = (double)dr1["Longitude"];
                                if (maxX < (double)dr1["Longitude"]) maxX = (double)dr1["Longitude"];

                                sn.Landmark.DsLandmarkDetails.Rows.Add(dr1);
                            }
                        }
                        if (LandmarkOptions.SelectedValue == "1")
                            sn.Landmark.LandmarkType = VLF.CLS.Def.Enums.GeozoneType.Rectangle;
                        if (LandmarkOptions.SelectedValue == "2")
                            sn.Landmark.LandmarkType = VLF.CLS.Def.Enums.GeozoneType.Polygon;

                        sn.Landmark.Y = (minY + maxY) / 2;
                        sn.Landmark.X = (minX + maxX) / 2;
                    }
                    if (LandmarkOptions.SelectedValue == "0")
                    {
                        string[] xyValues= hidPoints.Value.Trim().Split('|');
                        sn.Landmark.Y = double.Parse(xyValues[0]);
                        sn.Landmark.X = double.Parse(xyValues[1]);
                        sn.Landmark.Radius = (int)(double.Parse(xyValues[2]));
                        sn.Landmark.LandmarkType = VLF.CLS.Def.Enums.GeozoneType.None;
                    }

                    string strJavaScript = "self.close(); window.opener.ReFreshWindow();";
                    RadAjaxManager1.ResponseScripts.Add(strJavaScript);
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                RadAjaxManager1.ResponseScripts.Add("alert('" + errorSave + "')");
            }
        }
}
}