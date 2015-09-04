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
    public partial class frmMdtOTAForms : System.Web.UI.Page
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

            if (objUtil.ErrCheck(dbf.GetMdtsByFleetId(sn.UserID, sn.SecId, Convert.ToInt32(this.cboFleet.SelectedItem.Value),2, ref xml), false))
                if (objUtil.ErrCheck(dbf.GetMdtsByFleetId(sn.UserID, sn.SecId, Convert.ToInt32(this.cboFleet.SelectedItem.Value),2, ref xml), false))
                {
                    this.lblMessage.Text = "Error in GetMdtsByFleetId ";   
                    return;
                }
     


            if (xml == "")
            {
                this.lblMessage.Text = "No records - GetMdtsByFleetId ";   
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
            dc.DefaultValue = "Firmware";
            ds.Tables[0].Columns.Add(dc);


          
            dc = new DataColumn("DateTimeSent", Type.GetType("System.DateTime"));
            ds.Tables[0].Columns.Add(dc);


            dc = new DataColumn("chkWipe", Type.GetType("System.Boolean"));
            dc.DefaultValue = true;
            ds.Tables[0].Columns.Add(dc);

            dc = new DataColumn("chkDLS", Type.GetType("System.Boolean"));
            dc.DefaultValue = false;
            ds.Tables[0].Columns.Add(dc);


            dc = new DataColumn("chkBIM", Type.GetType("System.Boolean"));
            dc.DefaultValue = false;
            ds.Tables[0].Columns.Add(dc);

            dc = new DataColumn("chkOVR", Type.GetType("System.Boolean"));
            dc.DefaultValue = false;
            ds.Tables[0].Columns.Add(dc);

            dc = new DataColumn("Title", Type.GetType("System.String"));
            dc.DefaultValue = this.cboOrganization.SelectedItem.Text;
            ds.Tables[0].Columns.Add(dc);
   

            sn.Admin.DsConfigFirmaware = ds;

            dgData.DataSource = ds;
            dgData.DataBind();
            this.tblFW.Visible = true;
        }
        catch (Exception Ex)
        {
            this.lblMessage.Text = Ex.Message;   
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
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMDTUpgradeType, "2");
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMDTUpgradeFormsetId, this.cboFleet.SelectedItem.Value.ToString());
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMDTUpgradeFirmwareVersion, "CURRENT");
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMDTUpgradeDaylightSaving, (Convert.ToBoolean(rowItem["chkDLS"]) ? "1" : "0"));
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMDTUpgradeBlankInMotion, (Convert.ToBoolean(rowItem["chkBIM"]) ? "1" : "0"));
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMDTUpgradeOverwriteSettings, (Convert.ToBoolean(rowItem["chkOVR"]) ? "1" : "0"));
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMDTUpgradeWipeFormsMemory, (Convert.ToBoolean(rowItem["chkWipe"]) ? "1" : "0"));
                        paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMDTUpgradeTitle, rowItem["Title"].ToString() );


                        ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                        objUtil = new clsUtility(sn);


                        if (objUtil.ErrCheck(dbv.AddMdtOTA (sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]),2, paramList), false))
                            if (objUtil.ErrCheck(dbv.AddMdtOTA(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]),2, paramList), true))
                                {

                                    this.lblMessage.Text = "Update Forms failed for Box Id:" + rowItem["BoxId"].ToString() ;
                                    return;
                                }

                    }


                }

                if (lblMessage.Text == "")
                    this.lblMessage.Text = "Update Forms was sent successfully.";


            }
            catch (Exception ex)
            {
                this.lblMessage.Text = ex.ToString();
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
            CheckBox chkWipe = (CheckBox)e.Item.FindControl("chkWipe");
            CheckBox chkDLS = (CheckBox)e.Item.FindControl("chkDLS");
            CheckBox chkBIM = (CheckBox)e.Item.FindControl("chkBIM");
            CheckBox chkOVR = (CheckBox)e.Item.FindControl("chkOVR");
            TextBox txtTitle = (TextBox)e.Item.FindControl("txtTitle");

            for (int i = 0; i < dgData.Items.Count; i++)
            {
                foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
                {
                    if (BoxId.ToString() == rowItem["BoxId"].ToString())
                    {
                        rowItem["chkBox"] = true;
                        rowItem["chkWipe"] = chkWipe.Checked;
                        rowItem["chkDLS"] = chkDLS.Checked;
                        rowItem["chkOVR"] = chkOVR.Checked;
                        rowItem["chkBIM"] = chkBIM.Checked;
                        rowItem["Title"] = txtTitle.Text;
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
                CheckBox chkWipe = (CheckBox)(dgData.Items[i].Cells[8].Controls[1]);
                CheckBox chkDLS = (CheckBox)(dgData.Items[i].Cells[9].Controls[1]);
                CheckBox chkBIM = (CheckBox)(dgData.Items[i].Cells[10].Controls[1]);
                CheckBox chkOVR = (CheckBox)(dgData.Items[i].Cells[11].Controls[1]);

                foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
                {
                    if (dgData.Items[i].Cells[0].Text.ToString() == rowItem["BoxId"].ToString())
                    {
                        rowItem["chkBox"] = ch.Checked;
                        rowItem["chkWipe"] = chkWipe.Checked;
                        rowItem["chkDLS"] = chkDLS.Checked;
                        rowItem["chkOVR"] = chkOVR.Checked;
                        rowItem["chkBIM"] = chkBIM.Checked;
                    }
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
