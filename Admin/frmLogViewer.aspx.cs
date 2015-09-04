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
using System.Data;
using System.IO;

namespace SentinelFM.Admin
{
    public partial class frmLogViewer : System.Web.UI.Page
    {

        protected SentinelFMSession sn = null;
        protected clsUtility objUtil;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if ((sn == null) || (sn.UserName == ""))
            {
                Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
                return;
            }
            if (!Page.IsPostBack)
            {
                cboHoursFill(ref cboHoursFrom);
                cboHoursFill(ref cboHoursTo);

                this.txtFrom.Text = DateTime.Now.ToString("MM/dd/yyyy");
                this.txtTo.Text = DateTime.Now.ToString("MM/dd/yyyy");
            }
        }


        private void cboHoursFill(ref DropDownList cbo)
        {
            ListItem ls;
            //12 AM
            ls = new ListItem();
            ls.Text = "12" + " AM";
            ls.Value = "0";
            cbo.Items.Add(ls);
            for (int i = 1; i < 12; i++)
            {
                ls = new ListItem();
                if (i < 10)
                {
                    ls.Text = "0" + i.ToString() + " AM";
                    ls.Value = "0" + i.ToString();
                }
                else
                {
                    ls.Text = i.ToString() + " AM";
                    ls.Value = i.ToString();
                }
                cbo.Items.Add(ls);
            }


            //12 PM
            ls = new ListItem();
            ls.Text = "12" + " PM";
            ls.Value = "12";
            cbo.Items.Add(ls);
            //---------

            int nextValue = 0;

            for (int i = 1; i < 12; i++)
            {
                ls = new ListItem();
                ls.Text = i.ToString() + " PM";
                nextValue = i + 12;
                ls.Value = nextValue.ToString();
                cbo.Items.Add(ls);
            }
        }
        protected void cmdView_Click(object sender, EventArgs e)
        {
            ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
            objUtil = new clsUtility(sn);
            string modules = "";
            string xml = "";
            string filter = this.txtFilter.Text;
            string top = this.txtTop.Text;
            string strFromDate = "";
            string strToDate = "";

            //if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
            //    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value +  ":" + this.cboMinutesAdd.SelectedItem.Value;

            //if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
            //    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":" + this.cboMinutesAdd.SelectedItem.Value;

            //if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
            //    strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":" + this.cboMinutesAdd.SelectedItem.Value;

            //if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
            //    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":" + this.cboMinutesAdd.SelectedItem.Value;

            //if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
            //    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":" + this.cboMinutesAdd.SelectedItem.Value;

            //if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
            //    strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":" + this.cboMinutesAdd.SelectedItem.Value;


            strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":" + this.cboMinutesFrom.SelectedItem.Value;
            strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":" + this.cboMinutesTo.SelectedItem.Value;

            for (int i = 0; i < chkModules.Items.Count; i++)
            {
                if (chkModules.Items[i].Selected)
                    modules += chkModules.Items[i].Value + ",";
            }

            if (modules.Length == 0)
            {
                this.lblMessage.Text = "Please select a module"; 
                return;
            }
 
            modules = modules.Substring(0, modules.Length - 1);

            if (objUtil.ErrCheck(dbs.GetLogData(sn.UserID, sn.SecId, Convert.ToDateTime(strFromDate), Convert.ToDateTime(strToDate), modules, filter, top, ref xml), false))
                if (objUtil.ErrCheck(dbs.GetLogData(sn.UserID, sn.SecId, Convert.ToDateTime(strFromDate), Convert.ToDateTime(strToDate), modules, filter, top, ref xml), true))
                {
                    return;
                }

            // string strPath = Server.MapPath("DataSets/logData.xsd");
            if (xml == "")
            {
                return;
            }

            this.lblMessage.Text = "";
            StringReader strrXML = new StringReader(xml);
            DataSet ds = new DataSet();
            //ds.ReadXmlSchema(strPath);


            ds.ReadXml(strrXML);
            sn.Admin.DsLog = ds;
            dgLogger.ClearCachedDataSource();
            dgLogger.RebindDataSource();
        }
        protected void dgLogger_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            if (sn.Admin.DsLog != null && sn.Admin.DsLog.Tables[0] != null)
            {
                e.DataSource = sn.Admin.DsLog;
            }
        }
    }
}
