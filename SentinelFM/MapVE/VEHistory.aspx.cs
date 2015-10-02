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
    public partial class VEHistory : SentinelFMBasePage 
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            VirtualEarth.AvlHistoryPin avl;
            StringBuilder avlDataString = new StringBuilder();
            string icon = "";
            string IconType = "Circle";
            string strUnitOfMes = sn.User.UnitOfMes == 1 ? "Km" : "Mi";

            #region Vehicle Data
            if (sn.History.VehicleId == 0)
                IconType = "Circle";

            sn.Map.EditMapVE = "false"; 
            sn.Map.GeozonesMapVE = "";
            sn.Map.LandmarksMapVE = "";
            sn.Map.AvlMapVE = "";


            if (!sn.History.ShowStops && !sn.History.ShowStopsAndIdle && !sn.History.ShowIdle)
            {
                if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables.Count > 0))
                {
                    DataSet ds = new DataSet();
                    DataView dv = new DataView();
 
                    if (sn.History.ShowTrips)
                    {
                        sn.History.DsHistoryInfo.Tables[1].DefaultView.Sort = "Description ASC,OriginDateTime Desc";
                        dv = sn.History.DsHistoryInfo.Tables[1].DefaultView;
                    }
                    else
                    {
                        sn.History.DsHistoryInfo.Tables[0].DefaultView.Sort = "Description ASC,OriginDateTime Desc";
                        dv = sn.History.DsHistoryInfo.Tables[0].DefaultView;
                    }
                    

                    foreach (DataRowView dr in dv)
                    {
                        if ((!clsUtility.IsNumeric(dr["Latitude"].ToString() )) || (!clsUtility.IsNumeric(dr["Latitude"].ToString() )) || (Convert.ToDouble(dr["Longitude"])==0) || (Convert.ToDouble(dr["Longitude"])==0))
                            continue ;

                        if (Convert.ToBoolean(dr["chkBoxShow"]) && (dr["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (dr["Longitude"].ToString().TrimEnd() != VLF.CLS.Def.Const.blankValue))
                        {
                            if (dr["Speed"].ToString() != "0")
                                icon = "Green" + IconType + dr["MyHeading"].ToString().TrimEnd() + ".png";
                            else
                                icon = "Red" + IconType + ".png";

                            avl = new VirtualEarth.AvlHistoryPin();
                            avl.latitude = Convert.ToDouble(dr["Latitude"]);
                            avl.longitude = Convert.ToDouble(dr["Longitude"]);
                            avl.iconName = icon;
                            avl.speed = dr["speed"].ToString().TrimEnd() + " " + strUnitOfMes + "/h";
                            avl.messageType = dr["BoxMsgInTypeName"].ToString().TrimEnd();
                            avl.address = dr["StreetAddress"].ToString().TrimEnd();
                            avl.timestamp = dr["MyDateTime"].ToString().TrimEnd();
                            avl.description = dr["Description"].ToString().TrimEnd();
                            avl.customProp = dr["CustomProp"].ToString().TrimEnd().Replace("|", " ");
                            avl.heading = dr["MyHeading"].ToString().TrimEnd(); 
                            avlDataString.Append(avl.ToVEstring());

                        }
                    }


                     sn.Map.AvlMapVE = avlDataString.ToString();
                }
            }
            else if (sn.History.DsHistoryInfo.Tables["StopData"] != null)
            {
                sn.History.DsHistoryInfo.Tables["StopData"].DefaultView.Sort = "ArrivalDateTime Desc";

                foreach (DataRowView dr in sn.History.DsHistoryInfo.Tables["StopData"].DefaultView)
                {

                    if (Convert.ToDouble(dr["Latitude"]) == 0 || Convert.ToDouble(dr["Longitude"]) == 0)
                        continue;


                    TimeSpan TripIndling;
                    TripIndling = new TimeSpan(Convert.ToInt64(dr["StopDurationVal"]) * TimeSpan.TicksPerSecond);
                    double StopDurationVal = TripIndling.TotalMinutes;

                    if (Convert.ToBoolean(dr["chkBoxShow"]))
                    {
                        if (dr["Remarks"].ToString() == "Idling")
                            icon = "Idle_SI.png";
                        else if (StopDurationVal < 15)
                            icon = "Stop_3_SI.png";
                        else if ((StopDurationVal > 15) && (StopDurationVal < 60))
                            icon = "Stop_15_SI.png";
                        else if (StopDurationVal > 60)
                            icon = "Stop_60_SI.png";


                        if (dr["Remarks"].ToString() == "Stopped")
                            icon = "Red" + IconType + ".png";


                        avl = new VirtualEarth.AvlHistoryPin();
                        avl.latitude = Convert.ToDouble(dr["Latitude"]);
                        avl.longitude = Convert.ToDouble(dr["Longitude"]);
                        avl.iconName = icon;
                        avl.speed = "0 " + strUnitOfMes + "/h";
                        avl.messageType = dr["Remarks"].ToString().TrimEnd();
                        avl.address = dr["Location"].ToString().TrimEnd();
                        avl.timestamp = dr["ArrivalDateTime"].ToString().TrimEnd();
                        avl.description = "Duration:" + dr["StopDuration"].ToString().TrimEnd();

                        avlDataString.Append(avl.ToVEstring());

                    }
                }
                sn.Map.AvlMapVE = avlDataString.ToString();
            }

            #endregion
        }
    }
}
