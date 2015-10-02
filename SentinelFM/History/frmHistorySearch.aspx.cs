using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

namespace SentinelFM
{
    public partial class History_frmHistorySearch : SentinelFMBasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            SearchHistory_NewTZ();
        }

        // Changes for TimeZone Feature start
        private void SearchHistory_NewTZ()
        {
            DataSet dsHistory = new DataSet();
            StringReader strrXML = null;

            string xml = "";
            ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();

            string strAddress = "%" + this.txtIntrStreet1.Text + "%";// +" | " + this.txtIntrCity.Text + ", " + this.txtIntrState.Text + ", " + this.cbolblIntrCountry.SelectedItem.Value;

            if (objUtil.ErrCheck(dbh.GetVehiclesInHistoryByAddress(sn.UserID, sn.SecId, Convert.ToInt32(sn.History.FleetId), Convert.ToDateTime(sn.History.FromDateTime).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(sn.History.ToDateTime).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), strAddress, ref xml), false))
                if (objUtil.ErrCheck(dbh.GetVehiclesInHistoryByAddress(sn.UserID, sn.SecId, Convert.ToInt32(sn.History.FleetId), Convert.ToDateTime(sn.History.FromDateTime).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(sn.History.ToDateTime).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), strAddress, ref xml), true))
                {
                    return;
                }

            strrXML = new StringReader(xml);
            if (xml == "")
                return;

            //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
            //string strPath = MapPath(DataSetPath) + @"\HistSearch.xsd";

            string strPath = Server.MapPath("../Datasets/HistSearch.xsd");

            dsHistory.ReadXmlSchema(strPath);
            dsHistory.ReadXml(strrXML);

            if (sn.History.DsHistoryInfo.Tables.Count > 2)
                sn.History.DsHistoryInfo.Tables[1].Clear();

            sn.History.DsHistoryInfo.Tables.Add(dsHistory.Tables[0].Copy());
            sn.History.DsHistoryInfo.Relations.Add("SearchResults_BoxId",
                                sn.History.DsHistoryInfo.Tables[0].Columns["BoxId"],
                                sn.History.DsHistoryInfo.Tables[1].Columns["BoxId"],
                                false);
            sn.History.DsHistoryInfo.Relations.Add("SearchResults_OriginDateTime",
                                sn.History.DsHistoryInfo.Tables[0].Columns["OriginDateTime"],
                                sn.History.DsHistoryInfo.Tables[1].Columns["OriginDateTime"],
                                false);
        }
        // Changes for TimeZone Feature end

        private void SearchHistory()
        {
            DataSet dsHistory = new DataSet();
            StringReader strrXML = null;

            string xml = "";
            ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();

            string strAddress = "%" + this.txtIntrStreet1.Text + "%";// +" | " + this.txtIntrCity.Text + ", " + this.txtIntrState.Text + ", " + this.cbolblIntrCountry.SelectedItem.Value;

            if (objUtil.ErrCheck(dbh.GetVehiclesInHistoryByAddress(sn.UserID, sn.SecId, Convert.ToInt32(sn.History.FleetId), Convert.ToDateTime(sn.History.FromDateTime).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), Convert.ToDateTime(sn.History.ToDateTime).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), strAddress, ref xml), false))
                if (objUtil.ErrCheck(dbh.GetVehiclesInHistoryByAddress(sn.UserID, sn.SecId, Convert.ToInt32(sn.History.FleetId), Convert.ToDateTime(sn.History.FromDateTime).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), Convert.ToDateTime(sn.History.ToDateTime).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), strAddress, ref xml), true))
                {
                    return;
                }

            strrXML = new StringReader(xml);
            if (xml == "")
                return;

            //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
            //string strPath = MapPath(DataSetPath) + @"\HistSearch.xsd";

            string strPath = Server.MapPath("../Datasets/HistSearch.xsd");

            dsHistory.ReadXmlSchema(strPath);
            dsHistory.ReadXml(strrXML);

            if (sn.History.DsHistoryInfo.Tables.Count > 2)
                sn.History.DsHistoryInfo.Tables[1].Clear();

            sn.History.DsHistoryInfo.Tables.Add(dsHistory.Tables[0].Copy());
            sn.History.DsHistoryInfo.Relations.Add("SearchResults_BoxId",
                                sn.History.DsHistoryInfo.Tables[0].Columns["BoxId"],
                                sn.History.DsHistoryInfo.Tables[1].Columns["BoxId"],
                                false);
            sn.History.DsHistoryInfo.Relations.Add("SearchResults_OriginDateTime",
                                sn.History.DsHistoryInfo.Tables[0].Columns["OriginDateTime"],
                                sn.History.DsHistoryInfo.Tables[1].Columns["OriginDateTime"],
                                false);
        }
    }
}
