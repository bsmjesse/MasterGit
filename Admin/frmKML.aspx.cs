using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class frmKML : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.Params["count"] == null
            || Request.Params["boxid"] == null
            || Request.Params["from"] == null
            || Request.Params["to"] == null)
        {
            Response.Write("Incorrect usage of this page. This page requires parameters: \"count\", \"boxid\", \"from\" and \"to\".");
            return;
        }

        try
        {
            SentinelFM.SentinelFMSession sn = null;
            SentinelFM.clsUtility objUtil;
            ServerDBHistory.DBHistory dbHistory = new ServerDBHistory.DBHistory();
            short sMsgCount;
            int intBoxID;
            DateTime dtFrom, dtTo;
            string strXML = "", strOutput = "";
            StringReader srXML = null;
            DataSet dsHistory = new DataSet();
            StreamWriter swKML = new StreamWriter(Response.OutputStream, Encoding.UTF8);
            bool blnValidDataFound = false;

            try
            {
                sMsgCount = short.Parse(HttpUtility.HtmlDecode(Request.Params["count"]));
                intBoxID = int.Parse(HttpUtility.HtmlDecode(Request.Params["boxid"]));
                dtFrom = DateTime.Parse(HttpUtility.HtmlDecode(Request.Params["from"]));
                dtTo = DateTime.Parse(HttpUtility.HtmlDecode(Request.Params["to"]));
            }

            catch
            {
                Response.Write("Error loading parameters. Please go <a href='#' onClick='history.back()'>back</a> and verify values.");
                return;
            }


            sn = (SentinelFM.SentinelFMSession)Session["SentinelFMSession"];

            if (sn == null || sn.UserName == "")
            {
                Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
                return;
            }

            objUtil = new SentinelFM.clsUtility(sn);

            try
            {
                if (objUtil.ErrCheck(dbHistory.GetLastMessagesFromHistory(sn.UserID, sn.SecId, sMsgCount, intBoxID, -1, dtFrom, dtTo, ref strXML), false))
                    if (objUtil.ErrCheck(dbHistory.GetLastMessagesFromHistory(sn.UserID, sn.SecId, sMsgCount, intBoxID, -1, dtFrom, dtTo, ref strXML), true))
                    {
                        return;
                    }
            }

            catch (Exception ex)
            {
                Response.Write("Error retrieving data from database:<br /><br />");
                throw ex;
            }

            if (strXML == "")
            {
                Response.Write("No data was found for the given parameters. Please go <a href='#' onClick='history.back()'>back</a> and edit values.");
                return;
            }

            srXML = new StringReader(strXML);
            dsHistory.ReadXml(srXML);
            strOutput = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><kml xmlns=\"http://earth.google.com/kml/2.1\"><Document>";

            foreach (DataRow dr in dsHistory.Tables[0].Rows)
            {
                if (((string)dr["ValidGps"]) == "0")
                {
                    strOutput += CreateKMLPlacemark((string)dr["BoxId"],
                                                    ((string)dr["BoxMsgInTypeName"]).Trim(),
                                                    DateTime.Parse((string)dr["OriginDateTime"]).ToString(),
                                                    (string)dr["Latitude"],
                                                    (string)dr["Longitude"]);

                    blnValidDataFound = true;
                }
            }

            strOutput += "</Document></kml>";

            if (!blnValidDataFound)
            {
                Response.Write("Data was found, but they had invalid GPS. Please go <a href='#' onClick='history.back()'>back</a> and edit values.");
                return;
            }

            Response.ContentType = "text/xml";
            Response.AddHeader("Content-Disposition", "attachment; filename=untitled.kml");
            swKML.Write(strOutput);
            swKML.Close();
        }

        catch (Exception ex)
        {
            while (ex != null)
            {
                Response.Write(ex.Message + "<br />");
                ex = ex.InnerException;
            }

            Response.Write("<br /><a href='#' onClick='history.back()'>Go Back</a>");
        }
    }

    private string CreateKMLPlacemark(string boxID, string msgType, string datetime, string latitude, string longitude)
    {
        return "<Placemark><name>" + boxID + " - " + datetime + "</name><description>" + msgType
               + "</description><Point><coordinates>" + longitude + "," + latitude
               + ",0</coordinates></Point></Placemark>";
    }
}
