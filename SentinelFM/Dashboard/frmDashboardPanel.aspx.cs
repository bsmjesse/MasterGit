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
using ISNet.WebUI.WebDesktop;
using System.IO;
using System.Text;

namespace SentinelFM
{

    public partial class frmDashboardPanel : SentinelFMBasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                LoadDashboard();
            }
        }

        private void LoadDashboard()
        {
            string xml = "";
            DataSet ds = new DataSet();

            using (ServerDBUser.DBUser dbUser = new ServerDBUser.DBUser())
            {

                if (objUtil.ErrCheck(dbUser.GetUsersDashboards(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbUser.GetUsersDashboards(sn.UserID, sn.SecId, ref xml), true))
                    {
                        this.lblMessage.Text = "No Dashboards";  
                        this.WebPaneDashBoard.Visible = false;   
                        return;
                    }

                if (xml == "")
                {
                    this.lblMessage.Text = "No Dashboards";
                    this.WebPaneDashBoard.Visible = false;
                    return;
                }



                StringReader strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);

            }

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                this.lblMessage.Text = "No Dashboards";
                this.WebPaneDashBoard.Visible = false;
                return;
            }
            this.lblMessage.Text = "";
            this.WebPaneDashBoard.Visible = true;
            int i = 0;

            xml = "<WebGroupPane GroupType='HorizontalTile' Name='RootGroupMain'> <Panes>";

            string xml1 = "";
            xml1 += "<WebGroupPane GroupType='VerticalTile' Name='RootGroup'>";
            xml1 += "<Panes>";

            string xml2 = "";
            xml2 += "<WebGroupPane GroupType='VerticalTile' Name='RootGroup1'>";
            xml2 += "<Panes>";

            string tmp = "";
            string fullUrl = "";
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr["ContentURL"].ToString().Contains("?"))
                    fullUrl = dr["ContentURL"] + "|" + dr["JoinId"];
                else
                    fullUrl = dr["ContentURL"] + "?JoinId=" + dr["JoinId"];

                tmp = "<WebPane ContentMode='UseIFrame' ContentURL='" + fullUrl + "' Name='" + dr["DashboardName"] + dr["JoinId"] + "' Text='" + dr["DashboardName"] + "'/>";
                if (i % 2 == 0)
                    xml1 += tmp;
                else
                    xml2 += tmp;

                i++;
            }

            xml1 += "</Panes>";
            xml1 += "</WebGroupPane>";
            xml2 += "</Panes>";
            xml2 += "</WebGroupPane>";

            xml += xml1;
            xml += xml2;
            xml += "</Panes></WebGroupPane>";


            // convert string to stream 

            byte[] byteArray = Encoding.ASCII.GetBytes(xml);
            MemoryStream stream = new MemoryStream(byteArray);
            this.WebPaneDashBoard.LoadPanesStructureFromXml(stream);
            
        }
    }
}