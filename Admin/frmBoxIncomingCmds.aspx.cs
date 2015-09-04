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
using System.IO;
using VLF.CLS.Def;

namespace SentinelFM.Admin
{
    /// <summary>
    /// Summary description for frmFWUpdate.
    /// </summary>
    public partial class frmBoxSentCommands : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;
        protected clsUtility objUtil;
        protected DataSet dsFirmware = new DataSet();

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
                cboOrganization_Fill();
                cboOrganization.SelectedIndex = -1;
                cboOrganization.Items.FindByValue(sn.User.OrganizationId.ToString()).Selected = true;


                if (Convert.ToInt32(cboOrganization.SelectedItem.Value) != 0)
                {
                    CboFleet_Fill();
                    this.lblFleets.Visible = true;
                    cboFleet.Items.Insert(0, new ListItem("Please Select a Fleet", "-1"));
                }
            }


        }


        private void cboOrganization_Fill()
        {

            StringReader strrXML = null;
            DataSet ds = new DataSet();
            objUtil = new clsUtility(sn);
            string xml = "";

            ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

            if (objUtil.ErrCheck(dbo.GetAllOrganizationsInfoXML(sn.UserID, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbo.GetAllOrganizationsInfoXML(sn.UserID, sn.SecId, ref xml), true))
                {
                    return;
                }

            if (xml == "")
            {
                return;
            }

            strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);
            this.cboOrganization.DataSource = ds;
            this.cboOrganization.DataBind();



        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

        protected void cboOrganization_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Convert.ToInt32(cboOrganization.SelectedItem.Value) != 0)
            {
                CboFleet_Fill();
                this.lblFleets.Visible = true;
                cboFleet.Items.Insert(0, new ListItem("Please Select a Fleet", "-1"));
            }
        }


        private void CboFleet_Fill()
        {

            objUtil = new clsUtility(sn);
            DataSet dsFleets = new DataSet();
            StringReader strrXML = null;




            string xml = "";
            ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

            if (objUtil.ErrCheck(dbo.GetFleetsInfoByOrganizationId(sn.UserID, sn.SecId, Convert.ToInt32(this.cboOrganization.SelectedItem.Value), ref xml), false))
                if (objUtil.ErrCheck(dbo.GetFleetsInfoByOrganizationId(sn.UserID, sn.SecId, Convert.ToInt32(this.cboOrganization.SelectedItem.Value), ref xml), true))
                {

                    cboFleet.Items.Clear();
                    cboFleet.Items.Insert(0, new ListItem("Please Select a Fleet", "-1"));
                    return;
                }


            if (xml == "")
            {
                cboFleet.Items.Clear();
                cboFleet.Items.Insert(0, new ListItem("Please Select a Fleet", "-1"));

                return;
            }

            strrXML = new StringReader(xml);
            dsFleets.ReadXml(strrXML);

            cboFleet.DataSource = dsFleets;
            cboFleet.DataBind();
        }

        protected void cmdView_Click(object sender, System.EventArgs e)
        {
            ShowData();
        }

        private void ShowData()
        {
            DataSet ds;
            ds = new DataSet();
            objUtil = new clsUtility(sn);
            StringReader strrXML = null;

            string xml = "";

            ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();
            int fleetId = Convert.ToInt32(this.cboFleet.SelectedItem.Value);

            if (objUtil.ErrCheck(dbh.GetCmdRec(sn.UserID, sn.SecId, fleetId, Convert.ToInt32( CmdTypeList.SelectedValue), ref xml), false))
                if (objUtil.ErrCheck(dbh.GetCmdRec(sn.UserID, sn.SecId, fleetId, Convert.ToInt32(CmdTypeList.SelectedValue), ref xml), true))
                {
                    return;
                }

            if (xml == "")
            {
                return;
            }

            strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);
            foreach (DataRow rowItem in ds.Tables[0].Rows)
            {
                rowItem["Timestamp"] = Convert.ToDateTime(rowItem["Timestamp"]).ToString();
                if (VLF.CLS.Util.PairFindValue(Const.keyFirmwareVersion, rowItem["CustomProp"].ToString().Trim()) != "")
                    rowItem["CustomProp"] = VLF.CLS.Util.PairFindValue(Const.keyFirmwareVersion, rowItem["CustomProp"].ToString().Trim());
                
            }
            
            gvData.DataSource = ds;
            gvData.DataBind();
            ////
            cmdToExcel.Visible = true;
           
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            return;
        }

        protected void cmdToExcel_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=IncomingCmds.xls");
            Response.Charset = "";
            this.EnableViewState = false;

            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);

            gvData.RenderControl(htw);

            Response.Write(sw.ToString());
            Response.End();
        }
}
}