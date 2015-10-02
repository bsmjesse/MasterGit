using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;

namespace SentinelFM
{

    public partial class VE : SentinelFMBasePage
    {

public string jsversion = "v1.1.3g2";

        protected void Page_Load(object sender, EventArgs e)
        {
	    

            if (!Page.IsPostBack)
            {
                try
                {
                    string strUnitOfMes = sn.User.UnitOfMes == 1 ? "Km" : "Mi";
                    string token = clsUtility.ValidateVEToken();

                    StringBuilder sb = new StringBuilder("<script type=\"text/javascript\">");
                    sb.AppendFormat("var avls = \"{0}\";", sn.Map.AvlMapVE);
                    sb.AppendFormat("var org = \"{0}\";", sn.User.OrganizationId);
                    sb.AppendFormat("var unitOfMes = \"{0}\";", strUnitOfMes);
                    sb.AppendFormat("var logo = \"{0}\";", "bsm_logo.gif");
                    sb.AppendFormat("var geoZone = \"{0}\";", sn.Map.GeozonesMapVE);
                    sb.AppendFormat("var landmark = \"{0}\";", sn.Map.LandmarksMapVE);
                    sb.AppendFormat("var token = \"{0}\";", token);
                    sb.AppendFormat("var edit = {0};", sn.Map.EditMapVE);
                    sb.AppendFormat("var uid = \"{0}\";", sn.UserID);
                    sb.AppendFormat("var sid = \"{0}\";", sn.SecId);
                    sb.AppendLine("</script>");
                    this.VEScriptLiteral.Text = sb.ToString();
                }
                catch (Exception exc)
                {
                    this.VEScriptLiteral.Text = "<script type=\"text/javascript\">alert('" + exc.Message + "');</script>";
                }

            }

            if ((Request.Form["MapType"] != null) && (Request.Form["MapType"] != ""))
            {
                if ((Request.Form["MapType"] == "Landmark") && (Request.Form["Points"] != null && Request.Form["Points"] != ""))
                {
                    ReadLandmarkPoints();
                    return;
                }
                else if ((Request.Form["MapType"] == "GeoZone") && (Request.Form["Points"] != null && Request.Form["Points"] != ""))
                {
                    ReadGeoZonePoints();
                    return;
                }

            }
        }


        private void ReadLandmarkPoints()
        {
            string strJavaScript = "";
            Int32 LandmarkId = 0;
            string points = Request.Form["Points"];
            string[] tmpPoints = points.Split(',');

            if (tmpPoints.Length == 0)
            {
                strJavaScript += "<script language='javascript'>window.top.self.close();</script>";
                Response.Write(strJavaScript);
                return;
            }

            strJavaScript += "<script language='javascript'>";
            strJavaScript += " window.top.opener.document.forms[0].txtX.value=" + tmpPoints[1] + ";";
            strJavaScript += " window.top.opener.document.forms[0].txtY.value=" + tmpPoints[0] + ";";
            strJavaScript += " window.top.self.close();</script>";
            Response.Write(strJavaScript);


        }

        private void ReadGeoZonePoints()
        {



            string strJavaScript = "";
            Int32 LandmarkId = 0;
            string points = Request.Form["Points"];
            string[] tmpPoints = points.Split(',');

            if (tmpPoints.Length == 0)
            {
                strJavaScript += "<script language='javascript'>window.top.self.close();</script>";
                Response.Write(strJavaScript);
                return;
            }


            try
            {
                if (sn.GeoZone.DsGeoDetails.Tables[0] != null && sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count > 0)
                    sn.GeoZone.DsGeoDetails.Tables[0].Rows.Clear();

                if (tmpPoints.Length == 4)
                    sn.GeoZone.GeozoneTypeId = VLF.CLS.Def.Enums.GeozoneType.Rectangle;
                else if (tmpPoints.Length > 4)
                    sn.GeoZone.GeozoneTypeId = VLF.CLS.Def.Enums.GeozoneType.Polygon;
                else
                    sn.GeoZone.GeozoneTypeId = VLF.CLS.Def.Enums.GeozoneType.Unknown;

                int i = 0;
                while (i < tmpPoints.Length)
                {
                    DataRow dr = sn.GeoZone.DsGeoDetails.Tables[0].NewRow();
                    dr["Latitude"] = tmpPoints[i];
                    dr["Longitude"] = tmpPoints[i + 1];
                    dr["SequenceNum"] = sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count + 1;
                    sn.GeoZone.DsGeoDetails.Tables[0].Rows.Add(dr);
                    i += 2;
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                strJavaScript += "<script language='javascript'>";
                strJavaScript += " window.top.opener.document.forms[0].txtMapMessage.value='GeoZone add failed.'; window.top.self.close();</script>";
                return;
            }

            strJavaScript += "<script language='javascript'>";
            strJavaScript += " window.top.opener.document.forms[0].txtMapMessage.value='GeoZone was drawn successfully.'; window.top.self.close();</script>";
            Response.Write(strJavaScript);
        }
    }
}
