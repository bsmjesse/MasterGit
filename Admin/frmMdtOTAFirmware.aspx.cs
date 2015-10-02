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

namespace SentinelFM.Admin
{
    public partial class frmMdtOTAFirmware : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;
        protected clsUtility objUtil;
        protected DataSet dsUPGFirmwareType = new DataSet();


        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if ((sn == null) || (sn.UserName == ""))
            {

                Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
                return;
            }

            if (!Page.IsPostBack)
            {

                string strOrgId = Request.QueryString["OrgId"];
                string strFleetId = Request.QueryString["FleetId"];
                string strVehicleId = Request.QueryString["VehicleId"];


                DsUPGFirmwareType_Fill();
                cboOrganization_Fill();
                int OrgId = sn.User.OrganizationId;
                
                cboOrganization.SelectedIndex = -1;
                cboOrganization.Items.FindByValue(sn.User.OrganizationId.ToString()).Selected = true;
                this.cboOrganization.Enabled = false;

                CboFleet_Fill();
                if ((strFleetId != null) && (strFleetId != "-1"))
                {
                    cboFleet.SelectedIndex = -1;
                    cboFleet.Items.FindByValue(strFleetId.ToString()).Selected = true;

                    CboVehicle_Fill(Convert.ToInt32(strFleetId));
                }
                else
                {
                    //cboFleet.Items.Clear(); 
                    cboFleet.Items.Insert(0, new ListItem("Please Select a Fleet", "-1"));
                }


                if ((strVehicleId != null) && (strVehicleId != "-1"))
                {
                    cboVehicle.SelectedIndex = -1;
                    cboVehicle.Items.FindByValue(strVehicleId.ToString()).Selected = true;
                }
                else
                {
                    cboVehicle.Visible = false;
                    cboVehicle.Items.Clear();
                    cboVehicle.Items.Insert(0, new ListItem("Please Select a Vechicle", "-1"));
                }

                if (strOrgId != null)
                {
                    ShowData();
                    this.tblFW.Visible = true;  
                }
                else
                   this.tblFW.Visible = false;  

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
        protected void cboOrganization_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Convert.ToInt32(cboOrganization.SelectedItem.Value) != 0)
            {
                CboFleet_Fill();

                this.lblFleets.Visible = true;
                this.lblVehicles.Visible = false;
                this.cboVehicle.Visible = false;
                this.tblFW.Visible = false;
                this.lblMessage.Text = "";
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
        private void CboVehicle_Fill(int fleetId)
        {
            DataSet dsVehicle;
            dsVehicle = new DataSet();
            objUtil = new clsUtility(sn);
            StringReader strrXML = null;


            string xml = "";
            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

            if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                {
                    return;
                }
            if (xml == "")
            {
                return;
            }

            strrXML = new StringReader(xml);
            dsVehicle.ReadXml(strrXML);

            cboVehicle.DataSource = dsVehicle;
            cboVehicle.DataBind();

        }
        protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //if (Convert.ToInt32(cboFleet.SelectedItem.Value) != 0)
            //{

            //    cboVehicle.Items.Clear();
            //    CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
            //    cboVehicle.Items.Insert(0, new ListItem("Please select a Vehicle", "-1"));
            //    cboVehicle.SelectedIndex = 0;
            //    this.lblFleets.Visible = true;
            //    this.lblVehicles.Visible = true;
            //    this.cboVehicle.Visible = true;
            //    this.tblFW.Visible = false;
            //    this.lblMessage.Text = "";
            //}
        }

        protected void cmdView_Click(object sender, EventArgs e)
        {
            this.lblMessage.Text = "";  
            ShowData();
        }

        private void ShowData()
        {

              try
            {
            DataSet ds;
            ds = new DataSet();
            objUtil = new clsUtility(sn);
            StringReader strrXML = null;

            string xml = "";



         
            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

            if (objUtil.ErrCheck(dbf.GetMdtsByFleetId(sn.UserID, sn.SecId, Convert.ToInt32(this.cboFleet.SelectedItem.Value),1, ref xml), false))
                if (objUtil.ErrCheck(dbf.GetMdtsByFleetId(sn.UserID, sn.SecId, Convert.ToInt32(this.cboFleet.SelectedItem.Value),1, ref xml), false))
                {
                    return;
                }
     


            if (xml == "")
            {
                this.tblFW.Visible = false;
                return;
            }

            strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);



            // Show Combobox
            DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
            dc.DefaultValue = false;
            ds.Tables[0].Columns.Add(dc);




            dc = new DataColumn("UPGFirmwareTypeId", Type.GetType("System.Int32"));
            dc.DefaultValue = "1";
            ds.Tables[0].Columns.Add(dc);



            dc = new DataColumn("UPGFirmwareType", Type.GetType("System.String"));
            dc.DefaultValue = "CURRENT";
            ds.Tables[0].Columns.Add(dc);


           

            dc = new DataColumn("DateTimeSent", Type.GetType("System.DateTime"));
            ds.Tables[0].Columns.Add(dc);


            sn.Admin.DsConfigFirmaware = ds;

            dgData.DataSource = ds;
            dgData.DataBind();
            this.tblFW.Visible = true;
        }
        catch (Exception Ex)
        {

        }



        }
        protected void cmdUnselectAllSensors_Click(object sender, EventArgs e)
        {

        }
        protected void cmdSetAllSensors_Click(object sender, EventArgs e)
        {

        }
        protected void cmdGetBoxFirmware_Click(object sender, EventArgs e)
        {

        }
        protected void cmdRefreshBoxFirmware_Click(object sender, EventArgs e)
        {
            ShowData(); 
        }
        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                SaveCheckBoxes();
                this.lblMessage.Text = "";

                foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
                {
                    if (Convert.ToBoolean(rowItem["chkBox"]) )
                    {
                        Int64 sessionTimeOut = 0;
                        string paramList = "";
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMDTUpgradeType, "1");
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMDTUpgradeFirmwareVersion, rowItem["UPGFirmwareType"].ToString() );


                        ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                        objUtil = new clsUtility(sn);


                        if (objUtil.ErrCheck(dbv.AddMdtOTA (sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]),1, paramList), false))
                            if (objUtil.ErrCheck(dbv.AddMdtOTA(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]),1, paramList), true))
                                {

                                    this.lblMessage.Text = "Update FW failed for Box Id:" + rowItem["BoxId"].ToString() ;
                                    return;
                                }

                    }


                }

                if (lblMessage.Text == "")
                    this.lblMessage.Text = "Update FW was sent successfully.";


            }
            catch (Exception ex)
            {
                this.lblMessage.Text = ex.ToString();
            }

        }




        public int GetUPGFirmwareType(string  UPGFirmwareType)
      {

          DropDownList cboUPGFirmwareType = new DropDownList();
          cboUPGFirmwareType.DataValueField = "UPGFirmwareType";
          cboUPGFirmwareType.DataTextField = "UPGFirmwareType";
          DsUPGFirmwareType_Fill();
          cboUPGFirmwareType.DataSource = dsUPGFirmwareType;
          cboUPGFirmwareType.DataBind();

         cboUPGFirmwareType.SelectedIndex = -1;
         if (UPGFirmwareType != "")
             cboUPGFirmwareType.Items.FindByValue(UPGFirmwareType.ToString()).Selected = true;

         return cboUPGFirmwareType.SelectedIndex;

      }



        private void DsUPGFirmwareType_Fill()
        {
            try
            {

                MdtService.MdtService mdt = new MdtService.MdtService();
                string[] firmwareVersion=null;
                mdt.GetAvailableFirmwareVersions(ref firmwareVersion); 
                dsUPGFirmwareType.Clear();

                DataTable tblUPGFirmwareType = dsUPGFirmwareType.Tables.Add("FirmwareType");

                
                tblUPGFirmwareType.Columns.Add("UPGFirmwareType", typeof(string));
                object[] objRow = new object[1];

                for (int i = 0; i < firmwareVersion.Length; i++)
                {
                    objRow[0] = firmwareVersion[i];
                    tblUPGFirmwareType.Rows.Add(objRow);
                }
                
                dsUPGFirmwareType.Tables.Add(tblUPGFirmwareType);  
            }
            catch (Exception Ex)
            {

            }

        }
        protected void dgData_EditCommand(object source, DataGridCommandEventArgs e)
        {
            SaveCheckBoxes();
            dgData.EditItemIndex = Convert.ToInt32(e.Item.ItemIndex);
            dgData.DataSource = sn.Admin.DsConfigFirmaware;
            dgData.DataBind();
            dgData.SelectedIndex = -1;
        }
        protected void dgData_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            dgData.EditItemIndex = -1;
            this.dgData.DataSource = sn.Admin.DsConfigFirmaware;
            this.dgData.DataBind();
        }
        protected void dgData_UpdateCommand(object source, DataGridCommandEventArgs e)
        {

            DropDownList cboUPGFirmwareType;
            Int32 BoxId = Convert.ToInt32(dgData.DataKeys[e.Item.ItemIndex]);
            cboUPGFirmwareType = (DropDownList)e.Item.FindControl("cboUPGFirmwareType");
            

            for (int i = 0; i < dgData.Items.Count; i++)
            {
                foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
                {
                    if (BoxId.ToString() == rowItem["BoxId"].ToString())
                    {
                        rowItem["chkBox"] = true;
                        rowItem["UPGFirmwareType"] = cboUPGFirmwareType.SelectedItem.Text ;
                        dgData.EditItemIndex = -1;
                        this.dgData.DataSource = sn.Admin.DsConfigFirmaware;
                        this.dgData.DataBind();

                        return;
                    }
                }
            }
        }


        private void SaveCheckBoxes()
        {

            for (int i = 0; i < dgData.Items.Count; i++)
            {
                CheckBox ch = (CheckBox)(dgData.Items[i].Cells[1].Controls[1]);

                foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
                {
                    if (dgData.Items[i].Cells[0].Text.ToString() == rowItem["BoxId"].ToString())
                        rowItem["chkBox"] = ch.Checked;
                }
            }

        }

        protected void cmdRebootMDT_Click(object sender, EventArgs e)
        {
            try
            {
                SaveCheckBoxes();
                MdtService.MdtService mdt = new MdtService.MdtService();

                foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
                {

                    if (Convert.ToBoolean(rowItem["chkBox"]))
                        mdt.RebootMDT(Convert.ToInt32(rowItem["BoxId"]));

                }
            }
           catch (Exception ex)
            {
                this.lblMessage.Text = ex.ToString();
            }
        }
}
}
