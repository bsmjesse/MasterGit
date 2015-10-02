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
    public partial class MapNew_frmDrawGeoZone : SentinelMapBasePage
    {
        public string errorSave = "Save failed.";
        public string MaximunPoints = "3";
        public string drawRectangleText = ""; //"Holding down the control (Ctrl) key, click to draw the geozone. Release the control key to pan the map." +
               //"Click at each of two corners, following the diagonal of the rectangle." ;
        public string drawPolygon ="";// "Holding down the control (Ctrl) key, click to draw the geozone. Release the control key to pan the map." +
              //"Click at each corner of the polygon, clockwise.";

        public string MinimumPointsRectangleError = string.Empty;
        public string MinimumPointsPolygonError = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {

            drawRectangleText = (string)GetLocalResourceObject("drawRectangleText");
            drawPolygon = (string)GetLocalResourceObject("drawPolygon");

            if (!Page.IsPostBack)
            {
                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                GuiSecurity(this);

                if (sn.GeoZone.AddMode)
                {
                    this.cmdShowEditGeoZone.Visible = false;
                    this.tblGeoZone.Visible = true;
                }

                if (sn.GeoZone.ShowEditGeoZoneTable)
                    this.tblGeoZone.Visible = true;
                else
                    this.tblGeoZone.Visible = false;

                CboLandmarks_Fill();
                if (sn.GeoZone.SetGeoZone)
                {
                    this.cmdDrawGeoZone.BorderStyle = BorderStyle.Inset;
                    this.cmdDrawGeoZone.Text = (string)base.GetLocalResourceObject("cmdDrawGeozone_Text_CompleteDrawingGeozone");
                    this.GeoZoneOptions.Enabled = false;
                    this.lblMessage.Visible = true;

                    //if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Polygon)
                    //    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_ClickToCreateGeozone");
                    //else if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                    //    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_DrawDesiredRectangle");


                }
                else
                {

                    cmdDrawGeoZone.BorderStyle = BorderStyle.Outset;
                    this.cmdDrawGeoZone.Text = (string)base.GetLocalResourceObject("cmdDrawGeozone_Text_StartDrawing");
                    this.lblMessage.Visible = true;

                    if (sn.GeoZone.ShowEditGeoZoneTable)
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_NavigateMapToGeozone");


                    this.GeoZoneOptions.Enabled = true;

                    //if (sn.User.MapEngine[0].MapEngineID == VLF.MAP.MapType.GeoMicro)
                    //    this.GeoZoneOptions.Enabled = true ;
                    //else
                    //    this.GeoZoneOptions.Enabled = false;
                }

                GeoZoneOptions.SelectedIndex = GeoZoneOptions.Items.IndexOf(GeoZoneOptions.Items.FindByValue(Convert.ToInt16(sn.GeoZone.GeozoneTypeId).ToString()));


            }

            MinimumPointsRectangleError = GetScriptEscapeString((string)base.GetLocalResourceObject("lblMessage_Text_MinimumPointsRectangleError"));
            MinimumPointsPolygonError = GetScriptEscapeString((string)base.GetLocalResourceObject("lblMessage_Text_MinimumPointsPolygonError"));

            MaximunPoints = System.Configuration.ConfigurationManager.AppSettings["MaxGeoZonePoints"].ToString();
        }
        protected void cboLandmarks_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
            {
                if (dr["LandmarkName"].ToString().TrimEnd() == this.cboLandmarks.SelectedItem.Value.ToString().TrimEnd())
                {


                    Dictionary<string, string> lankMark = new Dictionary<string, string>();
                    lankMark.Add("id", "1");
                    lankMark.Add("lat", dr["Latitude"] is DBNull ? string.Empty : dr["Latitude"].ToString());
                    lankMark.Add("lon", dr["Longitude"] is DBNull ? string.Empty : dr["Longitude"].ToString());
                    lankMark.Add("desc", dr["LandmarkName"] is DBNull ? string.Empty : dr["LandmarkName"].ToString().Trim());
                    lankMark.Add("rad", dr["Radius"] is DBNull ? string.Empty : dr["Radius"].ToString());
                    string icon = "Landmark.ico";
                    lankMark.Add("icon", icon);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    js.MaxJsonLength = int.MaxValue;
                    string javaScript = js.Serialize(lankMark);
                    javaScript = string.Format("eval('var geolandmarkData = {0}'); myMap.ShowOneLandMark(geolandmarkData); CleanDrawGeoZone();", javaScript);
                    RadAjaxManager1.ResponseScripts.Add(javaScript);
                    ResetControl();
                    return;
                }

            }
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

        private void CboLandmarks_Fill()
        {
            try
            {

                cboLandmarks.Items.Clear();

                if (sn.DsLandMarks.Tables[0].Rows.Count == 0)
                {
                    StringReader strrXML = null;
                    DataSet dsLandmarks = new DataSet();

                    string xml = "";
                    ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                    if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                        if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                        {
                            return;
                        }

                    if (xml == "")
                    {
                        this.cboLandmarks.DataSource = null;
                        this.cboLandmarks.DataBind();
                        return;
                    }


                    strrXML = new StringReader(xml);
                    dsLandmarks.ReadXml(strrXML);
                    sn.DsLandMarks = dsLandmarks;
                }

                this.cboLandmarks.DataSource = sn.DsLandMarks;
                this.cboLandmarks.DataBind();
                cboLandmarks.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboLandmarks_Item_0"), "-1"));

            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
        protected void cmdShowEditGeoZone_Click(object sender, System.EventArgs e)
        {
            this.tblGeoZone.Visible = true;
            sn.GeoZone.ShowEditGeoZoneTable = true;
            ResetControl();
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                sn.GeoZone.DsGeoDetails.Tables[0].Rows.Clear();
                if (hidPoints.Value.Trim() != string.Empty)
                {
                    string[] points = hidPoints.Value.Trim().Split(',');
                    foreach (string myPoints in points)
                    {
                        if (myPoints.Trim() != string.Empty)
                        {
                            string[] latLon = myPoints.Split('|');
                            DataRow dr1 = sn.GeoZone.DsGeoDetails.Tables[0].NewRow();
                            dr1["Latitude"] = double.Parse(latLon[0]);
                            dr1["Longitude"] = double.Parse(latLon[1]);
                            dr1["SequenceNum"] = sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count + 1;
                            sn.GeoZone.DsGeoDetails.Tables[0].Rows.Add(dr1);
                        }
                    }
                    sn.GeoZone.GeozoneTypeId = (VLF.CLS.Def.Enums.GeozoneType)Convert.ToInt16(this.GeoZoneOptions.SelectedItem.Value);

                    string strJavaScript = " window.opener.document.forms[0].txtMapMessage.value='" + (string)base.GetLocalResourceObject("txtMapMessage_Value_OnWindowOpen") + "';";
                    strJavaScript += " self.close();";
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