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
using System.Configuration;
using System.IO;
using System.Globalization;
using VLF.CLS;

namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmVehicleInfo.
    /// </summary>
    public partial class frmVehicleInfo : SentinelFMBasePage
    {
        protected System.Web.UI.HtmlControls.HtmlForm Form1;



        public string AddVehicleView = "";
        public string UpdateVehicleView = "";
        string confirm;

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
            this.dgVehicles.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgVehicles_PageIndexChanged);
            this.dgVehicles.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgVehicles_DeleteCommand);
            this.dgVehicles.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgVehicles_ItemDataBound);

        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);
                //Enable Search on pressing Enter Key
                txtSearchParam.Attributes.Add("onkeypress", "return clickButton(event,'" + cmdSearch.ClientID + "')");



                if ((sn == null) || (sn.UserName == ""))
                {
                    RedirectToLogin();
                    return;
                }



                if (sn != null && !sn.UserName.ToLower().Contains("hgi_"))
                {
                    HideButtons();
                }


                if (!Page.IsPostBack)
                {
                    //Salman Nov 06, 2013 Mantis# 2624
                    if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956)
                    {
                        cboSearchType.Items.Remove(cboSearchType.Items.FindByText("Description"));//FindByValue
                        //cboSearchType.Items.RemoveAt(0);
                        if (sn.SelectedLanguage.Contains("fr"))
                        {
                            cboSearchType.Items.Insert(0, new ListItem("Véhicule", "0"));
                        }
                        else
                        {
                            cboSearchType.Items.Insert(0, new ListItem("Vehicle", "0"));
                        }
                    }

                    //Salman Aug 25, 2014
                    if (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 999718)
                    {
                        for (int i = 4; i < 10; i++)
                            cboSearchType.Items[i].Enabled = true;
                    }

                    //Devin
                    //if (sn.User.OrganizationId == 999630 || sn.User.OrganizationId == 480)
                    //{
                    // btnFuelCategory.Visible = true;
                    //}
                    //if (sn.User.SuperOrganizationId == 382)
                    //   this.btnEquipmentAssignment.Visible = true;
                    //else
                    //  this.btnEquipmentAssignment.Visible = false ;

                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmEmails, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    ViewState["ConfirmDelete"] = "0";
                    dgVehicles_Fill(true);


                    //Add Vehicle View
                    if (!sn.User.ControlEnable(sn, 28))
                        AddVehicleView = "none";
                    else
                        AddVehicleView = "";


                    //Update Vehicle 
                    if (!sn.User.ControlEnable(sn, 29))
                        UpdateVehicleView = "none";
                    else
                        UpdateVehicleView = "";

                }

                if (sn.User.OrganizationId == 480)
                    this.cmdBoxFirmware.Visible = true;
                else
                    this.cmdBoxFirmware.Visible = false;

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void cmdAlarms_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmAlarms.aspx");
        }

        protected void cmdBoxFirmware_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmVehicleBoxFirmwareStatus.aspx");
        }
        protected void cmdOutputs_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmOutputs.aspx");
        }

        private void cmdLandmarks_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmEmails.aspx");
        }

        private void cmdPreference_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmPreference.aspx");
        }

        protected void cmdFleetVehicle_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmVehicleFleet.aspx");
        }

        /// <summary>
        /// Fill vehicles grid
        /// </summary>
        /// <param name="forceUpdate">True if force getting data from database</param>
        private void dgVehicles_Fill(bool forceUpdate)
        {
            try
            {
                // call web method only if a dataset is not valid
                if (forceUpdate || !Util.IsDataSetValid(sn.Misc.ConfDsvehicles))
                {
                    string xml = "";

                    DataSet dsVehicle = new DataSet();
                    ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                    if (objUtil.ErrCheck(dbo.GetOrganizationAllActiveVehiclesXmlByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                        if (objUtil.ErrCheck(dbo.GetOrganizationAllActiveVehiclesXmlByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                        {
                            return;
                        }

                    if (xml == "")
                    {
                        this.dgVehicles.DataSource = null;
                        this.dgVehicles.DataBind();
                        return;
                    }


                    if (sn.SelectedLanguage == "fr-CA")
                    {
                        xml = xml.Replace("Not Listed", "Pas dans la liste");
                        xml = xml.Replace("Truck", "Camion");
                    }

                    dsVehicle.ReadXml(new StringReader(xml));

                    sn.Misc.ConfDsvehicles = dsVehicle;
                }

                try
                {
                    // full data set
                    if (String.IsNullOrEmpty(this.txtSearchParam.Text))
                    {
                        this.dgVehicles.DataSource = sn.Misc.ConfDsvehicles.Tables[0];
                    }
                    else // filtered data
                    {
                        this.dgVehicles.DataSource = GetFilteredData();
                    }
                    if (sn.Misc.ConfVehiclesSelectedGridPage < this.dgVehicles.PageCount)
                    {
                        this.dgVehicles.CurrentPageIndex = sn.Misc.ConfVehiclesSelectedGridPage;
                        //this.dgVehicles.SelectedIndex = -1;
                    }
                    else
                        this.dgVehicles.CurrentPageIndex = 0;

                    this.dgVehicles.DataBind(); // exception if result page count < page index
                }
                catch (HttpException ex)
                {
                    this.dgVehicles.CurrentPageIndex = 0;
                }
                dgVehicles.SelectedIndex = -1;
                this.dgVehicles.DataBind();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void dgVehicles_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            dgVehicles.CurrentPageIndex = e.NewPageIndex;
            sn.Misc.ConfVehiclesSelectedGridPage = e.NewPageIndex;
            dgVehicles_Fill(false);
            //dgVehicles.SelectedIndex = -1;
        }

        protected void dgVehicles_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {
                //Check security
                bool cmdDelete = sn.User.ControlEnable(sn, 30);
                if (!cmdDelete)
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_NoPermission");
                    return;
                }

                if (confirm == "")
                    return;

                lblMessage.Visible = true;


                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();


                string xml = "";

                if (objUtil.ErrCheck(dbv.GetVehicleInfoXML(sn.UserID, sn.SecId, dgVehicles.DataKeys[e.Item.ItemIndex].ToString(), ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXML(sn.UserID, sn.SecId, dgVehicles.DataKeys[e.Item.ItemIndex].ToString(), ref xml), true))
                    {
                        return;
                    }


                int VehicleId = 0;

                StringReader strrXML = new StringReader(xml);
                DataSet dsVehicle = new DataSet();
                dsVehicle.ReadXml(strrXML);


                if (dsVehicle.Tables[0].Rows.Count > 0)
                    VehicleId = Convert.ToInt32(dsVehicle.Tables[0].Rows[0]["VehicleId"].ToString());


                //	int rowsAffected = 0 ;
                if (objUtil.ErrCheck(dbv.DeleteVehicle(sn.UserID, sn.SecId, VehicleId), false))
                    if (objUtil.ErrCheck(dbv.DeleteVehicle(sn.UserID, sn.SecId, VehicleId), true))
                    {
                        return;
                    }


                dgVehicles.SelectedIndex = -1;
                lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_VehicleDeleted");
                dgVehicles.CurrentPageIndex = 0;
                dgVehicles_Fill(true);
                confirm = "";

            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }


        private void dgVehicles_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                e.Item.Cells[11].ToolTip = (string)base.GetLocalResourceObject("dgVehicles_Tooltip_Edit");
                
                LinkButton DeleteButton = (LinkButton)e.Item.Cells[12].Controls[0];
                if (sn.User.ControlEnable(sn, 30))
                {
                    e.Item.Cells[12].ToolTip = (string)base.GetLocalResourceObject("dgVehicles_Tooltip_Delete");
                }
                else
                {
                    DeleteButton.Visible = false;
                }   
            }
            //				 if (e.Item.Cells[9].Text.ToString()=="1")
            //					e.Item.Cells[9].Text="&lt;img src=../images/circle.bmp border=0&gt;";
            //				else if (e.Item.Cells[9].Text.ToString()=="2")
            //					e.Item.Cells[9].Text="&lt;img src=../images/square.bmp border=0&gt;";
            //				 else if (e.Item.Cells[9].Text.ToString()=="3")
            //					 e.Item.Cells[9].Text="&lt;img src=../images/diamond.bmp border=0&gt;";
        }

        private void cmdVehicleGeoZone_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmVehicleGeoZoneAss.htm");
        }

        protected void cmdFleets_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmEmails.aspx");
        }

        private void GuiSecurity(System.Web.UI.Control obj)
        {

            foreach (System.Web.UI.Control ctl in obj.Controls)
            {
                try
                {
                    if (ctl.HasControls())
                        GuiSecurity(ctl);

                    System.Web.UI.WebControls.Button CmdButton = (System.Web.UI.WebControls.Button)ctl;
                    bool CmdStatus = false;
                    if (CmdButton.CommandName != "")
                    {
                        CmdStatus = sn.User.ControlEnable(sn, Convert.ToInt32(CmdButton.CommandName));
                        CmdButton.Enabled = CmdStatus;
                    }

                }
                catch
                {
                }
            }
        }
        protected void cmdUsers_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmUsers.aspx");
        }

        protected void dgVehicles_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            bool DeleteEnabled = sn.User.ControlEnable(sn, 30);
            if (((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item)) && DeleteEnabled)
            {
                confirm = "return confirm('" + (string)base.GetLocalResourceObject("Text_ConfirmDeletion") + "')";
                LinkButton deleteBtn = (LinkButton)e.Item.Cells[12].Controls[0];
                deleteBtn.Attributes.Add("onclick", confirm);
            }
        }

        protected void cboSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtSearchParam.Text = "";
            this.dgVehicles.DataSource = sn.Misc.ConfDsvehicles.Tables[0];
            this.dgVehicles.DataBind();
            this.txtSearchParam.Focus();
        }

        protected void cmdSearch_Click(object sender, EventArgs e)
        {

            dgVehicles_Fill(false);
        }

        private DataTable GetFilteredData()
        {
            DataRow[] drCollections = null;
            DataTable dt = sn.Misc.ConfDsvehicles.Tables[0].Clone();
            string filter = "";

            if (!string.IsNullOrEmpty(this.txtSearchParam.Text.Trim()))
            {
                string searchContent = this.txtSearchParam.Text.Trim().Replace("'", "''");

                switch (cboSearchType.SelectedItem.Value.Trim())
                {
                    case "0":
                        filter = String.Format("Description like '%{0}%'", searchContent);
                        break;

                    case "1":
                        filter = String.Format("LicensePlate like '%{0}%'", searchContent);
                        break;

                    case "2":

                        filter = String.Format("BoxId = '{0}'", searchContent);
                        break;

                    case "3":

                        filter = String.Format("VinNum like '%{0}%'", searchContent);

                        //foreach (DataRow rowItem in sn.Misc.ConfDsvehicles.Tables[0].Rows)
                        //{
                        //   if (Convert.ToInt64(rowItem["BoxId"]) == Convert.ToInt64(this.txtSearchParam.Text))
                        //   {
                        //      dt.ImportRow(rowItem);
                        //      break;
                        //   }
                        //}

                        break;

                    case "4":
                        filter = string.Format("[Equip Number] LIKE '%{0}%'", searchContent);
                        break;

                    case "5":
                        filter = string.Format("[Legacy Equip#] LIKE '%{0}%'", searchContent);
                        break;

                    case "6":
                        filter = string.Format("[SAP Equip#] LIKE '%{0}%'", searchContent);
                        break;

                    case "7":
                        filter = string.Format("[Object Type] LIKE '%{0}%'", searchContent);
                        break;

                    case "8":
                        filter = string.Format("[DOT Number] LIKE '%{0}%'", searchContent);
                        break;

                    case "9":
                        filter = string.Format("[Project Nbr] LIKE '%{0}%'", searchContent);
                        break;
                }
            }

            drCollections = sn.Misc.ConfDsvehicles.Tables[0].Select(filter, "Description");
            if (drCollections != null && drCollections.Length > 0)
            {
                foreach (DataRow dr in drCollections)
                {
                    dt.ImportRow(dr);
                }
            }

            return dt;
        }

        protected void cmdClear_Click(object sender, EventArgs e)
        {
            this.txtSearchParam.Text = "";
            this.dgVehicles.DataSource = sn.Misc.ConfDsvehicles.Tables[0];
            this.dgVehicles.DataBind();
        }
        protected void cmdScheduledTasks_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmTaskScheduler.aspx");
        }


        private void HideButtons()
        {

            if (sn.User.OrganizationId == 999630)
            {
                this.cmdAlarms.Visible = false;
                this.cmdOutputs.Visible = false;

            }

        }
    }
}
