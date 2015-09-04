using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentinelFM
{
    public partial class Messaging_Admin_MessageDetail : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int pmt = 0;
            int mid = 0;
            int mode = 0;
            int.TryParse(Request.QueryString["mt"], out pmt);
            int.TryParse(Request.QueryString["id"], out mid);
            int.TryParse(Request.QueryString["mode"], out mode);
            OutboundView(pmt, mid,mode);
        }

        void OutboundView(int peripheralMessageType, int messageId,int mode)
        {
            bool isOutbound = true;
            switch (peripheralMessageType)
            {
                case 4:
                    MDHeader.Text = "Text Message (A604 Open Server to Client)";
                    MDLocationDiv.Style.Clear();
                    MDLocationDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
                    break;
                case 9:
                case 10:
                    MDHeader.Text = "Stop Message";
                    break;
                case 23:
                    MDHeader.Text = "Text Message (A607 Open Client to Server)";
                    MDToDiv.Style.Clear();
                    MDToDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
                    MDStatusDiv.Style.Clear();
                    MDStatusDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
                    isOutbound = false;
                    break;
                default:
                    MDHeader.Text = "Text Message";
                    MDLocationDiv.Style.Clear();
                    MDLocationDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
                    break;
            }
            //Dictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters.Add("type", peripheralMessageType);
            //parameters.Add("id", messageId);
            //DataTable dt = ((QueryResult)DataConnector.ExecuteRequest(sn.UserID, Context, "PeripheralMessageHistory", parameters)).Data;
            
            DataRow[] drArr=null;

            if (mode==2)
                drArr = sn.Message.DsGarminLocations.Tables[0].Select("peripheralMessageTypeId=" + peripheralMessageType + " and messageId=" + messageId);
            else
                drArr = sn.Message.DsGarminMessages.Tables[0].Select("peripheralMessageTypeId=" + peripheralMessageType + " and messageId=" + messageId);

            if (drArr != null && drArr.Length > 0)
            {
                DataRow dr = drArr[0];
                MDTimestampValue.Text = Convert.ToDateTime(dr["originDateTime"]).ToString("dddd, MMM dd, yyyy HH:mm:ss");

                if (isOutbound)
                {
                    MDFromValue.Text = (string)dr["UserName"];
                    MDToValue.Text = (string)dr["GarminName"];
                }
                else
                    MDFromValue.Text = (string)dr["GarminName"];

                string[] props = ((string)dr["properties"]).Split(';');
                SortedList<string, string> keys = new SortedList<string, string>();
                foreach (string s in props)
                {
                    if (string.IsNullOrEmpty(s)) continue;
                    string[] value = s.Split('=');
                    keys.Add(value[0], value[1]);
                }

                MDBodyValue.Text = keys["TXT"];
                if (keys.ContainsKey("LNAME"))
                {
                    MDLandmarkValue.Text = keys["LNAME"];
                    MDLandmarkValue.Style.Clear();
                    MDLandmarkValue.Style.Add(HtmlTextWriterStyle.Display, "block");
                }
                else if (keys.ContainsKey("ADD"))
                {
                    MDAddressValue.Text = keys["ADD"];
                    MDAddressValue.Style.Clear();
                    MDAddressValue.Style.Add(HtmlTextWriterStyle.Display, "block");
                }
                else if (keys.ContainsKey("LAT") && keys.ContainsKey("LON"))
                {
                    MDLatLonValue.Text = string.Format("{0}, {1}", keys["LAT"], keys["LON"]);
                    MDLatLonValue.Style.Clear();
                    MDLatLonValue.Style.Add(HtmlTextWriterStyle.Display, "block");
                }
                if (typeof(DBNull) != dr["status"].GetType())
                {
                    MDStatusValue.Text = (string)dr["status"];
                    MDStatusTimeValue.Text = "Unknown"; // Convert.ToDateTime(dr[""]).ToLongDateString();
                }
            }

        }



    }
}