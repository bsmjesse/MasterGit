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
    public partial class VEMap : SentinelFMBasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            sn.Map.GeozonesMapVE = "";
            sn.Map.LandmarksMapVE = "";
            sn.Map.AvlMapVE = "";
            sn.Map.EditMapVE = "false"; 

            VirtualEarth.AvlPin avl;
            StringBuilder avlDataString = new StringBuilder();
            string icon = "";
            #region Vehicle Data
            if ((sn.Map.DsFleetInfo != null) && (sn.Map.DsFleetInfo.Tables.Count > 0))
            {

                foreach (DataRow dr in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (dr["chkBoxShow"].ToString().ToLower() == "true")
                    {
                        if (Convert.ToDateTime(dr["OriginDateTime"]) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                        {
                            icon = "Grey" + dr["IconTypeName"].ToString().TrimEnd() + ".png"; //+ ".ico";
                        }
                        else
                        {
                            if (dr["Speed"].ToString() != "0")
                                icon = "Green" + dr["IconTypeName"].ToString().TrimEnd() + dr["MyHeading"].ToString().TrimEnd() + ".png"; //+ ".ico";
                            else
                                icon = "Red" + dr["IconTypeName"].ToString().TrimEnd() + ".png"; //+ ".ico";
                        }

                        avl = new VirtualEarth.AvlPin();
                        avl.latitude = Convert.ToDouble(dr["Latitude"]);
                        avl.longitude = Convert.ToDouble(dr["Longitude"]);
                        avl.iconName = icon;
                        avl.speed = dr["CustomSpeed"].ToString().TrimEnd();
                        avl.status = dr["VehicleStatus"].ToString().TrimEnd();
                        avl.duration = "";
                        avl.address = dr["StreetAddress"].ToString().TrimEnd().Replace("'", "`");
                        avl.timestamp = dr["OriginDateTime"].ToString().TrimEnd();
                        avl.description = dr["Description"].ToString().TrimEnd().Replace("'", "`");
                        avl.heading = dr["MyHeading"].ToString().TrimEnd(); 
                        avlDataString.Append(avl.ToVEstring());

                    }
                }

                sn.Map.AvlMapVE = avlDataString.ToString();

            }
            #endregion
        }
    }
}