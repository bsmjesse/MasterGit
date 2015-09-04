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
    public partial class VELandmark : SentinelFMBasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Int32 LandmarkId = 0;
            StringBuilder ladmarkString = new StringBuilder();
            sn.Map.GeozonesMapVE = "";
            sn.Map.LandmarksMapVE = "";
            sn.Map.AvlMapVE = "";
            sn.Map.EditMapVE = "false"; 

            #region Read Landmarks points
            if (Request.Form["Points"] != null && Request.Form["Points"] != "")
            {
                ReadPointsInfo();
                return;
            }
            #endregion

            #region View Existing Landmark - Edit mode
            if (Request.QueryString["FormName"] != null && Request.QueryString["FormName"] == "Landmark")
            {
                sn.Map.EditMapVE = "true"; 
                VirtualEarth.LandmarkPin landmark = new VirtualEarth.LandmarkPin();
                landmark.latitude = sn.Landmark.Y;
                landmark.longitude = sn.Landmark.X;
                landmark.description = sn.Landmark.LandmarkName.ToString().TrimEnd().Replace("'", "`");
                landmark.radius = sn.User.UnitOfMes == 1 ? sn.Landmark.Radius : Convert.ToInt32(Math.Round(sn.Landmark.Radius * 3.28, 0)); ;
                ladmarkString.Append(landmark.ToVEstring());
                sn.Map.LandmarksMapVE = ladmarkString.ToString();
                  
                return;
            }
            #endregion
        }


        private void ReadPointsInfo()
        {
            string strJavaScript = "";
            Int32 LandmarkId = 0;
            string points = Request.Form["Points"];
            string[] tmpPoints = points.Split(',');

                if (tmpPoints.Length == 0)
                {
                    strJavaScript += "<script language='javascript'>self.close();</script>";
                    Response.Write(strJavaScript);
                    return;
                }

                strJavaScript += "<script language='javascript'>";
                strJavaScript += " window.opener.document.forms[0].txtX.value=" + tmpPoints[1] + ";";
                strJavaScript += " window.opener.document.forms[0].txtY.value=" + tmpPoints[0] + ";";
                strJavaScript += " self.close();</script>";
                Response.Write(strJavaScript);

               
        }
    }
}
