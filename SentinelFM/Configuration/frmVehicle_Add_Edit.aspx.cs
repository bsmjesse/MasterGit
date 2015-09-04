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
using System.Configuration;
using VLF.DAS.Logic;
using System.Web.Script.Serialization;

namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmVehicle_Add_Edit.
    /// </summary>
    public partial class frmVehicle_Add_Edit : SentinelFMBasePage
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

        string selectFuel = "No fuel category";
        private VLF.PATCH.Logic.PatchVehicle _vehicle;


        public string CustomIcon = string.Empty;
        public string SelectIcon = string.Empty;

        protected void Page_Load(object sender, System.EventArgs e)
        {


            //Clear IIS cache
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now);

            string LicensePlate = Request.QueryString["LicensePlate"];
            _vehicle = new VLF.PATCH.Logic.PatchVehicle(sConnectionString);

            SelectIcon = (string)base.GetLocalResourceObject("SelectIconResource1");

            /*if (sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999957)
            {
                trCustomIcon.Visible = false;
            }*/
            

            if (!Page.IsPostBack)
            {
                if (!sn.User.ControlEnable(sn, 114))
                    cmdVehicleStatus.Visible = false;

                //Devin
                if (sn.User.OrganizationId == 999630 || sn.User.OrganizationId == 480 || sn.User.ControlEnable(sn, 61))
                {
                    FuelCategoryMgr fuelCategoryMgr = new FuelCategoryMgr(sConnectionString);
                    cboFuelCategory.DataSource = fuelCategoryMgr.FuelCategory_Select(-1, sn.User.OrganizationId, false);
                    cboFuelCategory.DataBind();
                    cboFuelCategory.Items.Insert(0, new ListItem(selectFuel));
                    cboFuelCategory.SelectedIndex = 0;
                    trfuelCategory.Visible = true;
                }

                //Salman Mar 05, 2013
                cboPostedSpeed_Fill();
                //cboPostedSpeed.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectPostedSpeed"), "-1"));
                cboPostedSpeed.Items.Insert(0, new ListItem("SelectPostedSpeed", "-1"));
                if (sn.User.OrganizationId != 951 && sn.User.OrganizationId != 480)
                {
                    lblPostedSpeed.Visible = false;
                    cboPostedSpeed.Visible = false;
                }

                //Salman Aug 06, 2013 (HQ CR 17)
                if (sn.SelectedLanguage == "en-US" && (sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999957))
                {
                    lblDescription.Text = "Vehicle Number :";
                }

                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmVehicle_Add_EditForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                GuiSecurity(this);
                lblUnit.Text = sn.User.UnitOfMes == 1 ? "km" : "mi";
                if (sn.User.SmsSupport)
                    txtPhone.Enabled = true;

                if ((LicensePlate != null) && (LicensePlate != "undefined"))
                {
                    cboTimeZone_Fill();
                    GetVehicleInfo_NewTZ(LicensePlate);
                }
                else
                {
                    ClearFields();
                    CboMake_Fill();
                    CboBox_Fill("");
                    cboMake.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectMake"), "-1"));
                    CboProvince_Fill();
                    CboVehicleType_Fill();
                    cboVehicleType.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectVehicleType"), "-1"));
                    cboTimeZone_Fill();
                    cboTimeZone.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectTimeZone"), "-1"));
                }

                GuiSecurity(this);
                getCustomIcon();
            }
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

        // Changes for TimeZone Feature start


        protected void cmdSave_Click(object sender, System.EventArgs e)
        {
            try
            {

                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();


                int nPos = this.txtLicensePlate.Text.IndexOf("'");
                if (nPos > 0)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("LicensePlateQuote");
                    return;
                }

                if (sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999957)
                {
                    if (validateDescriptionForHQ(txtDescription.Text))
                    {
                        lblMessage.Visible = true;
                        //lblMessage.Text = "Utilisation de l'espace en nombre est limité.";//"Use of space in number is restricted.";
                        lblMessage.Text = (string)base.GetLocalResourceObject("DescriptionCustomValidationText");
                        return;
                    }
                }

                double cost = 0;

                if (sn.User.UnitOfMes == 1)
                    cost = Convert.ToDouble(txtCost.Text);
                else if (sn.User.UnitOfMes == 0.6214)
                    cost = Math.Round(Convert.ToDouble(this.txtCost.Text) * 0.6214, 2);


                if ((this.txtEmail.Text != "") && (Convert.ToSingle(this.cboTimeZone.SelectedItem.Value) == -1))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectTimeZone");
                    return;

                }

                if ((this.chkCritical.Checked) || (this.chkWarning.Checked) || (this.chkNotify.Checked))
                {
                    if ((this.txtEmail.Text == "") || (Convert.ToSingle(this.cboTimeZone.SelectedItem.Value) == -1))
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectEmailTimeZone");
                        return;

                    }
                }

                //DumpBeforeCall(sn, string.Format("frmVehicleAdd_Edit -- cmdSave_Click : LicensePlate={0}", txtLicensePlate.Text));

                Int64 VehicleId = 0;

                bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight.Checked));

                //Devin
                short fuelType = 0;
                if (cboFuelCategory.SelectedIndex > 0)
                    fuelType = short.Parse(cboFuelCategory.SelectedValue);
                
                if (this.lblVehicleId.Text == "")
                {
                    // Changes for TimeZone Feature start
                    if (objUtil.ErrCheck(dbv.AddVehicle_NewTZ(sn.UserID, sn.SecId, this.txtVinNum.Text, Convert.ToInt32(this.cboModel.SelectedItem.Value), Convert.ToInt16(this.cboVehicleType.SelectedItem.Value), this.cboProvince.SelectedItem.Value, Convert.ToInt16(this.txtYear.Text), this.txtColor.Text, this.txtDescription.Text, cost, sn.User.OrganizationId, this.txtLicensePlate.Text.Trim(), Convert.ToInt32(this.cboBox.SelectedItem.Value), Convert.ToInt16(this.optVehicleIcons.SelectedItem.Value), this.txtEmail.Text, this.txtPhone.Text, Convert.ToSingle(this.cboTimeZone.SelectedItem.Value), DayLightSaving, fuelType, this.chkNotify.Checked, this.chkWarning.Checked, this.chkCritical.Checked, chkDayLight.Checked, chkMaintenance.Checked, Convert.ToInt32(this.cboPostedSpeed.SelectedItem.Value), this.txtClass.Text, ref VehicleId), false))
                        if (objUtil.ErrCheck(dbv.AddVehicle_NewTZ(sn.UserID, sn.SecId, this.txtVinNum.Text, Convert.ToInt32(this.cboModel.SelectedItem.Value), Convert.ToInt16(this.cboVehicleType.SelectedItem.Value), this.cboProvince.SelectedItem.Value, Convert.ToInt16(this.txtYear.Text), this.txtColor.Text, this.txtDescription.Text, cost, sn.User.OrganizationId, this.txtLicensePlate.Text.Trim(), Convert.ToInt32(this.cboBox.SelectedItem.Value), Convert.ToInt16(this.optVehicleIcons.SelectedItem.Value), this.txtEmail.Text, this.txtPhone.Text, Convert.ToSingle(this.cboTimeZone.SelectedItem.Value), DayLightSaving, fuelType, this.chkNotify.Checked, this.chkWarning.Checked, this.chkCritical.Checked, chkDayLight.Checked, chkMaintenance.Checked, Convert.ToInt32(this.cboPostedSpeed.SelectedItem.Value), this.txtClass.Text, ref VehicleId), true))
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("AddVehicleFailed");
                            return;
                        }

                    this.lblVehicleId.Text = VehicleId.ToString();

                }

                else
                {
                  
                    if (objUtil.ErrCheck(dbv.UpdateVehicleInfo_NewTZ(sn.UserID, sn.SecId, this.lblOldLicense.Text, this.txtLicensePlate.Text.Trim(), this.txtColor.Text, cost, this.txtDescription.Text, Convert.ToInt32(this.cboModel.SelectedItem.Value), Convert.ToInt16(this.txtYear.Text), this.cboProvince.SelectedItem.Value, Convert.ToInt16(this.cboVehicleType.SelectedItem.Value), this.txtVinNum.Text, Convert.ToInt64(this.lblVehicleId.Text), Convert.ToInt32(this.lblOldBox.Text), Convert.ToInt32(this.cboBox.SelectedItem.Value), Convert.ToInt16(this.optVehicleIcons.SelectedItem.Value), this.txtEmail.Text, this.txtPhone.Text, Convert.ToSingle(this.cboTimeZone.SelectedItem.Value), DayLightSaving, fuelType, this.chkNotify.Checked, this.chkWarning.Checked, this.chkCritical.Checked, chkDayLight.Checked, chkMaintenance.Checked, Convert.ToInt32(this.cboPostedSpeed.SelectedItem.Value), this.txtClass.Text), false))
                        if (objUtil.ErrCheck(dbv.UpdateVehicleInfo_NewTZ(sn.UserID, sn.SecId, this.lblOldLicense.Text, this.txtLicensePlate.Text.Trim(), this.txtColor.Text, cost, this.txtDescription.Text, Convert.ToInt32(this.cboModel.SelectedItem.Value), Convert.ToInt16(this.txtYear.Text), this.cboProvince.SelectedItem.Value, Convert.ToInt16(this.cboVehicleType.SelectedItem.Value), this.txtVinNum.Text, Convert.ToInt64(this.lblVehicleId.Text), Convert.ToInt32(this.lblOldBox.Text), Convert.ToInt32(this.cboBox.SelectedItem.Value), Convert.ToInt16(this.optVehicleIcons.SelectedItem.Value), this.txtEmail.Text, this.txtPhone.Text, Convert.ToSingle(this.cboTimeZone.SelectedItem.Value), DayLightSaving, fuelType, this.chkNotify.Checked, this.chkWarning.Checked, this.chkCritical.Checked, chkDayLight.Checked, chkMaintenance.Checked, Convert.ToInt32(this.cboPostedSpeed.SelectedItem.Value), this.txtClass.Text), true)) // Changes for TimeZone Feature end
                        {
                            this.lblMessage.Visible = true;
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("UpdateVehicleFailed");
                            return;
                        }
                }

                //Devin
                //Devin
                try
                {
                var selectedboxid = Convert.ToInt32(this.cboBox.SelectedItem.Value);
                if (VehicleId == 0)
                        VehicleId = Convert.ToInt64(this.lblVehicleId.Text);

                if (sn.User.OrganizationId == 999630 || sn.User.OrganizationId == 480)
                {
                    FuelCategoryMgr fuelCategoryMgr = new FuelCategoryMgr(sConnectionString);

                    fuelCategoryMgr.FuelCategory_UpdateVehicleCo2(fuelType, sn.User.OrganizationId, long.Parse(lblVehicleId.Text.Trim()));
                }

                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("VehicleInfoUpdated");

                    if (sn.User.OrganizationId == 18 || sn.User.OrganizationId == 480)
                    {
                        int equipment_Id = -1;
                        if (cboVehicleType.SelectedValue == "207") equipment_Id = 2;
                        if (cboVehicleType.SelectedValue == "208") equipment_Id = 1;
                        //if (equipment_Id == 1 || equipment_Id == 2)
                        {
                        string hosConnectionString =
           ConfigurationManager.ConnectionStrings["SentinelHOSConnectionString"].ConnectionString;
                        System.Data.SqlClient.SqlConnection connection;
                        using (connection = new System.Data.SqlClient.SqlConnection(hosConnectionString))
                        {
                            System.Data.SqlClient.SqlCommand sqlCmd = new System.Data.SqlClient.SqlCommand();
                            try
                            {
                                sqlCmd.Connection = connection;
                                sqlCmd.CommandType = CommandType.StoredProcedure;
                                sqlCmd.CommandText = "usp_ant_Update_Box_Equipment_Assignment";
                                sqlCmd.Connection = connection;
                                System.Data.SqlClient.SqlParameter sqlPara = new System.Data.SqlClient.SqlParameter("@VehicleId", SqlDbType.BigInt);
                                sqlPara.Value = VehicleId;
                                sqlCmd.Parameters.Add(sqlPara);

                                sqlPara = new System.Data.SqlClient.SqlParameter("@Equipment_Id", SqlDbType.Int);
                                sqlPara.Value = equipment_Id;
                                sqlCmd.Parameters.Add(sqlPara);

                                sqlPara = new System.Data.SqlClient.SqlParameter("@OrganizationID", SqlDbType.Int);
                                sqlPara.Value = sn.User.OrganizationId;
                                sqlCmd.Parameters.Add(sqlPara);


                                sqlPara = new System.Data.SqlClient.SqlParameter("@UnitId", SqlDbType.Int);
                                sqlPara.Value = selectedboxid;
                                sqlCmd.Parameters.Add(sqlPara);

                                connection.Open();
                                sqlCmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                this.lblMessage.Text = (string)base.GetLocalResourceObject("UpdateVehicleFailed");
                            }
                            finally
                            {
                                if (connection.State == ConnectionState.Open) connection.Close();
                            }
                        }
                      }
                    }

                    //update custom icon
                    string customIconPath = CustomIconPath.Value;
                    if (string.IsNullOrEmpty(customIconPath)) customIconPath = string.Empty;
                    if (VehicleId > 0)
                    {
                        _vehicle.SaveImagePath(VehicleId, customIconPath);
                    }

                }
                catch (Exception ex) {
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("UpdateVehicleFailed");
                }
                Response.Write("<script language='javascript'>window.opener.location.href='frmvehicleinfo.aspx';window.close()</script>");




                //Netistix.OBDII_Service.VIUPointSynchUp wsVIUPointSynchUp = new Netistix.OBDII_Service.VIUPointSynchUp();
                //Netistix.OBDII_Service.DataSetVehiclesExtended myDataSet = new Netistix.OBDII_Service.DataSetVehiclesExtended();

                //wsVIUPointSynchUp.setVehiclesExtended(myDataSet); 


            }


            catch (Exception Ex)
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("Add-UpdateFailed");
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        protected void cboMake_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.cboModel.DataSource = null;
            this.cboModel.DataBind();
            CboModel_Fill();
        }


        private void CboMake_Fill()
        {
            try
            {
                DataSet dsMake;
                dsMake = new DataSet();


                StringReader strrXML = null;


                string xml = "";
                ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();

                if (objUtil.ErrCheck(dbs.GetAllMakesInfo(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbs.GetAllMakesInfo(sn.UserID, sn.SecId, ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    this.cboMake.DataSource = null;
                    this.cboMake.DataBind();
                    return;
                }

                if (sn.SelectedLanguage == "fr-CA")
                    xml = xml.Replace("Not Listed", "Pas dans la liste");

                strrXML = new StringReader(xml);
                dsMake.ReadXml(strrXML);

                dsMake.Tables[0].DefaultView.Sort = "MakeName";
                this.cboMake.DataSource =dsMake.Tables[0].DefaultView ;
                cboMake.DataBind();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void CboProvince_Fill()
        {
            try
            {
                DataSet dsProvince;
                dsProvince = new DataSet();


                StringReader strrXML = null;


                string xml = "";
                ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();

                if (objUtil.ErrCheck(dbs.GetAllStateProvinces(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbs.GetAllStateProvinces(sn.UserID, sn.SecId, ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    return;
                }

                strrXML = new StringReader(xml);
                dsProvince.ReadXml(strrXML);

                this.cboProvince.DataSource = dsProvince;
                cboProvince.DataBind();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        private void CboVehicleType_Fill()
        {
            try
            {
                DataSet dsVehicleType;
                dsVehicleType = new DataSet();


                StringReader strrXML = null;


                string xml = "";
                ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();

                if (objUtil.ErrCheck(dbs.GetAllVehicleTypes(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbs.GetAllVehicleTypes(sn.UserID, sn.SecId, ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    return;
                }


                if (sn.SelectedLanguage == "fr-CA")
                    xml = xml.Replace("Truck", "Camion");

                strrXML = new StringReader(xml);
                dsVehicleType.ReadXml(strrXML);

                this.cboVehicleType.DataSource = dsVehicleType;
                cboVehicleType.DataBind();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        private void CboModel_Fill()
        {
            try
            {
                DataSet dsModel;
                dsModel = new DataSet();


                StringReader strrXML = null;


                string xml = "";
                ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();

                if (objUtil.ErrCheck(dbs.GetModelsInfoByMakeId(sn.UserID, sn.SecId, Convert.ToInt32(this.cboMake.SelectedItem.Value), ref xml), false))
                    if (objUtil.ErrCheck(dbs.GetModelsInfoByMakeId(sn.UserID, sn.SecId, Convert.ToInt32(this.cboMake.SelectedItem.Value), ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    this.cboModel.Items.Clear();
                    this.cboModel.DataSource = null;
                    this.cboModel.DataBind();
                    return;
                }


                if (sn.SelectedLanguage == "fr-CA")
                    xml = xml.Replace("Not Listed", "Pas dans la liste");

                strrXML = new StringReader(xml);
                dsModel.ReadXml(strrXML);

                this.cboModel.DataSource = dsModel;
                cboModel.DataBind();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        // Changes for TimeZone Feature start
        private void GetVehicleInfo_NewTZ(string LicensePlate)
        {
            try
            {
                StringReader strrXML = null;
                DataSet dsVehicle = new DataSet();

                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetVehicleInfoXML_NewTZ(sn.UserID, sn.SecId, LicensePlate, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXML_NewTZ(sn.UserID, sn.SecId, LicensePlate, ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {

                    ClearFields();
                    this.tblVehicleInfo.Visible = false;
                    return;
                }


                if (sn.SelectedLanguage == "fr-CA")
                {
                    xml = xml.Replace("Not Listed", "Pas dans la liste");
                    xml = xml.Replace("Truck", "Camion");
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                if (dsVehicle.Tables[0].Rows.Count > 0)
                {
                    this.txtColor.Text = dsVehicle.Tables[0].Rows[0]["Color"].ToString();
                    this.txtDescription.Text = dsVehicle.Tables[0].Rows[0]["Description"].ToString();
                    this.txtClass.Text = dsVehicle.Tables[0].Rows[0]["Class"].ToString();
                    this.txtYear.Text = dsVehicle.Tables[0].Rows[0]["ModelYear"].ToString();
                    if (sn.User.UnitOfMes == 1)
                        this.txtCost.Text = dsVehicle.Tables[0].Rows[0]["CostPerMile"].ToString();
                    else if (sn.User.UnitOfMes == 0.6214)
                        this.txtCost.Text = Convert.ToString(Math.Round(Convert.ToDouble(dsVehicle.Tables[0].Rows[0]["CostPerMile"]) / 0.6214, 2));

                    this.lblVehicleId.Text = dsVehicle.Tables[0].Rows[0]["VehicleId"].ToString();
                    txtVinNum.Text = dsVehicle.Tables[0].Rows[0]["VinNum"].ToString();
                    this.txtLicensePlate.Text = LicensePlate;
                    this.lblOldLicense.Text = LicensePlate;
                    CboProvince_Fill();
                    this.cboProvince.SelectedIndex = -1;
                    //cboProvince.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["StateProvince"].ToString()).Selected = true;

                    for (int i = 0; i < cboProvince.Items.Count; i++)
                    {
                        if (cboProvince.Items[i].Text.TrimEnd() == dsVehicle.Tables[0].Rows[0]["StateProvince"].ToString().TrimEnd())
                        {
                            cboProvince.SelectedIndex = i;
                            break;
                        }
                    }


                    CboVehicleType_Fill();
                    this.cboVehicleType.SelectedIndex = -1;
                    for (int i = 0; i < cboVehicleType.Items.Count; i++)
                    {
                        if (cboVehicleType.Items[i].Text.TrimEnd() == dsVehicle.Tables[0].Rows[0]["VehicleTypeName"].ToString().TrimEnd())
                        {
                            cboVehicleType.SelectedIndex = i;
                            break;
                        }
                    }


                    CboMake_Fill();
                    this.cboMake.SelectedIndex = -1;
                    for (int i = 0; i < cboMake.Items.Count; i++)
                    {
                        if (cboMake.Items[i].Text.TrimEnd() == dsVehicle.Tables[0].Rows[0]["MakeName"].ToString().TrimEnd())
                        {
                            cboMake.SelectedIndex = i;
                            break;
                        }
                    }


                    CboModel_Fill();
                    this.cboModel.SelectedIndex = -1;
                    cboModel.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["MakeModelId"].ToString()).Selected = true;


                    CboBox_Fill(dsVehicle.Tables[0].Rows[0]["BoxId"].ToString());
                    this.cboBox.SelectedIndex = -1;
                    cboBox.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["BoxId"].ToString()).Selected = true;
                    this.lblOldBox.Text = dsVehicle.Tables[0].Rows[0]["BoxId"].ToString();

                    this.txtEmail.Text = dsVehicle.Tables[0].Rows[0]["Email"].ToString().TrimEnd();
                    this.txtPhone.Text = dsVehicle.Tables[0].Rows[0]["Phone"].ToString().TrimEnd();
                    cboTimeZone_Fill();
                    try
                    {
                        this.cboTimeZone.SelectedIndex = -1;
                        for (int i = 0; i < cboTimeZone.Items.Count; i++)
                        {
                            if (Convert.ToSingle(cboTimeZone.Items[i].Value.TrimEnd()) == Convert.ToSingle(dsVehicle.Tables[0].Rows[0]["TimeZone"].ToString().TrimEnd()))
                            {
                                cboTimeZone.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    catch
                    {
                    }

                    this.chkDayLight.Checked = Convert.ToBoolean(dsVehicle.Tables[0].Rows[0]["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                    this.chkNotify.Checked = Convert.ToBoolean(dsVehicle.Tables[0].Rows[0]["Notify"].ToString().TrimEnd());
                    this.chkWarning.Checked = Convert.ToBoolean(dsVehicle.Tables[0].Rows[0]["Warning"].ToString().TrimEnd());
                    this.chkCritical.Checked = Convert.ToBoolean(dsVehicle.Tables[0].Rows[0]["Critical"].ToString().TrimEnd());
                    this.chkMaintenance.Checked = Convert.ToBoolean(dsVehicle.Tables[0].Rows[0]["Maintenance"].ToString().TrimEnd());

                    this.tblVehicleInfo.Visible = true;
                    optVehicleIcons.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["IconTypeId"].ToString()).Selected = true;

                    //Devin
                    if (dsVehicle.Tables[0].Rows[0]["FormatType"] != DBNull.Value)
                    {
                        try
                        {
                            cboFuelCategory.ClearSelection();
                            cboFuelCategory.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["FormatType"].ToString()).Selected = true;
                        }
                        catch (Exception ex) { }
                    }

                    //Salman Mar 06, 2013
                    this.cboPostedSpeed.SelectedIndex = -1;
                    if (dsVehicle.Tables[0].Rows[0]["ServiceConfigID"] != DBNull.Value)
                    {
                        try
                        {
                            cboPostedSpeed.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["ServiceConfigID"].ToString()).Selected = true;
                        }
                        catch { }
                    }

                }
                else
                {
                    ClearFields();
                    this.tblVehicleInfo.Visible = false;
                }
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
        // Changes for TimeZone Feature end
        private void GetVehicleInfo(string LicensePlate)
        {
            try
            {
                StringReader strrXML = null;
                DataSet dsVehicle = new DataSet();

                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetVehicleInfoXML(sn.UserID, sn.SecId, LicensePlate, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXML(sn.UserID, sn.SecId, LicensePlate, ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {

                    ClearFields();
                    this.tblVehicleInfo.Visible = false;
                    return;
                }


              if (sn.SelectedLanguage == "fr-CA")
               {
                   xml = xml.Replace("Not Listed", "Pas dans la liste");
                   xml = xml.Replace("Truck", "Camion");
               }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                if (dsVehicle.Tables[0].Rows.Count > 0)
                {
                    this.txtColor.Text = dsVehicle.Tables[0].Rows[0]["Color"].ToString();
                    this.txtDescription.Text = dsVehicle.Tables[0].Rows[0]["Description"].ToString();
                    this.txtClass.Text = dsVehicle.Tables[0].Rows[0]["Class"].ToString();
                    this.txtYear.Text = dsVehicle.Tables[0].Rows[0]["ModelYear"].ToString();
                    if (sn.User.UnitOfMes == 1)
                        this.txtCost.Text = dsVehicle.Tables[0].Rows[0]["CostPerMile"].ToString();
                    else if (sn.User.UnitOfMes == 0.6214)
                        this.txtCost.Text = Convert.ToString(Math.Round(Convert.ToDouble(dsVehicle.Tables[0].Rows[0]["CostPerMile"]) / 0.6214, 2));

                    this.lblVehicleId.Text = dsVehicle.Tables[0].Rows[0]["VehicleId"].ToString();
                    txtVinNum.Text = dsVehicle.Tables[0].Rows[0]["VinNum"].ToString();
                    this.txtLicensePlate.Text = LicensePlate;
                    this.lblOldLicense.Text = LicensePlate;
                    CboProvince_Fill();
                    this.cboProvince.SelectedIndex = -1;
                    //cboProvince.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["StateProvince"].ToString()).Selected = true;

                    for (int i = 0; i < cboProvince.Items.Count; i++)
                    {
                        if (cboProvince.Items[i].Text.TrimEnd() == dsVehicle.Tables[0].Rows[0]["StateProvince"].ToString().TrimEnd())
                        {
                            cboProvince.SelectedIndex = i;
                            break;
                        }
                    }


                    CboVehicleType_Fill();
                    this.cboVehicleType.SelectedIndex = -1;
                    for (int i = 0; i < cboVehicleType.Items.Count; i++)
                    {
                        if (cboVehicleType.Items[i].Text.TrimEnd() == dsVehicle.Tables[0].Rows[0]["VehicleTypeName"].ToString().TrimEnd())
                        {
                            cboVehicleType.SelectedIndex = i;
                            break;
                        }
                    }


                    CboMake_Fill();
                    this.cboMake.SelectedIndex = -1;
                    for (int i = 0; i < cboMake.Items.Count; i++)
                    {
                        if (cboMake.Items[i].Text.TrimEnd() == dsVehicle.Tables[0].Rows[0]["MakeName"].ToString().TrimEnd())
                        {
                            cboMake.SelectedIndex = i;
                            break;
                        }
                    }


                    CboModel_Fill();
                    this.cboModel.SelectedIndex = -1;
                    cboModel.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["MakeModelId"].ToString()).Selected = true;


                    CboBox_Fill(dsVehicle.Tables[0].Rows[0]["BoxId"].ToString());
                    this.cboBox.SelectedIndex = -1;
                    cboBox.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["BoxId"].ToString()).Selected = true;
                    this.lblOldBox.Text = dsVehicle.Tables[0].Rows[0]["BoxId"].ToString();

                    this.txtEmail.Text = dsVehicle.Tables[0].Rows[0]["Email"].ToString().TrimEnd();
                    this.txtPhone.Text = dsVehicle.Tables[0].Rows[0]["Phone"].ToString().TrimEnd();
                    cboTimeZone_Fill();
                    try
                    {
                        this.cboTimeZone.SelectedIndex = -1;
                        for (int i = 0; i < cboTimeZone.Items.Count; i++)
                        {
                            if (Convert.ToInt16(cboTimeZone.Items[i].Value.TrimEnd()) == Convert.ToInt32(dsVehicle.Tables[0].Rows[0]["TimeZone"].ToString().TrimEnd()))
                            {
                                cboTimeZone.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    catch
                    {
                    }

                    this.chkDayLight.Checked = Convert.ToBoolean(dsVehicle.Tables[0].Rows[0]["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                    this.chkNotify.Checked = Convert.ToBoolean(dsVehicle.Tables[0].Rows[0]["Notify"].ToString().TrimEnd());
                    this.chkWarning.Checked = Convert.ToBoolean(dsVehicle.Tables[0].Rows[0]["Warning"].ToString().TrimEnd());
                    this.chkCritical.Checked = Convert.ToBoolean(dsVehicle.Tables[0].Rows[0]["Critical"].ToString().TrimEnd());
                    this.chkMaintenance.Checked = Convert.ToBoolean(dsVehicle.Tables[0].Rows[0]["Maintenance"].ToString().TrimEnd());

                    this.tblVehicleInfo.Visible = true;
                    optVehicleIcons.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["IconTypeId"].ToString()).Selected = true;

                    //Devin
                    if (dsVehicle.Tables[0].Rows[0]["FormatType"] != DBNull.Value)
                    {
                        try
                        {
                            cboFuelCategory.ClearSelection();
                            cboFuelCategory.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["FormatType"].ToString()).Selected = true;
                        }
                        catch (Exception ex) { }
                    }

                    //Salman Mar 06, 2013
                    this.cboPostedSpeed.SelectedIndex = -1;
                    if (dsVehicle.Tables[0].Rows[0]["ServiceConfigID"] != DBNull.Value)
                    {
                        try
                        {
                            cboPostedSpeed.Items.FindByValue(dsVehicle.Tables[0].Rows[0]["ServiceConfigID"].ToString()).Selected = true;
                        }
                        catch { }
                    }

                }
                else
                {
                    ClearFields();
                    this.tblVehicleInfo.Visible = false;
                }
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        
        private void ClearFields()
        {
            this.txtColor.Text = "";
            this.txtCost.Text = "";
            this.txtDescription.Text = "";
            this.txtYear.Text = "";
            this.txtClass.Text = "";
            this.lblVehicleId.Text = "";
            this.txtLicensePlate.Text = "";
            txtVinNum.Text = "";
            this.cboMake.Items.Clear();
            this.cboModel.Items.Clear();
            this.cboProvince.Items.Clear();
            this.cboVehicleType.Items.Clear();
            this.cboTimeZone.SelectedIndex = -1;
            this.chkCritical.Checked = false;
            this.chkNotify.Checked = false;
            this.chkWarning.Checked = false;
            this.txtEmail.Text = "";
            this.chkDayLight.Checked = false;
            //Salman Mar 06, 2013
            this.cboPostedSpeed.SelectedIndex = -1;
        }


        private void CboBox_Fill(string BoxId)
        {
            try
            {
                DataSet dsBox;
                dsBox = new DataSet();


                StringReader strrXML = null;


                string xml = "";
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                if (objUtil.ErrCheck(dbo.GetOrganizationAllUnassignedBoxIdsXml(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetOrganizationAllUnassignedBoxIdsXml(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    if (BoxId != "")
                    {
                        ListItem ls = new ListItem();
                        ls.Text = BoxId;
                        ls.Value = BoxId;
                        cboBox.Items.Add(ls);
                    }

                    return;
                }

                strrXML = new StringReader(xml);
                dsBox.ReadXml(strrXML);

                if (BoxId != "")
                {
                    dsBox.Tables[0].NewRow();
                    DataRow dr = dsBox.Tables[0].NewRow();
                    dr[0] = BoxId;
                    dsBox.Tables[0].Rows.Add(dr);
                    this.cboBox.DataSource = dsBox;
                    cboBox.DataBind();
                }

                this.cboBox.DataSource = dsBox;
                cboBox.DataBind();


            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }


        //Salman Mar 05, 2013
        private void cboPostedSpeed_Fill()
        {
            //try
            //{
            //    cboPostedSpeed.SelectedIndex = -1;
            //    cboPostedSpeed.DataSource = null;

            //    DataTable tblPostedSpeed = new DataTable();
            //    tblPostedSpeed.Columns.Add("ServiceConfigId", typeof(int));
            //    tblPostedSpeed.Columns.Add("ServiceName", typeof(string));
            //    tblPostedSpeed.Rows.Add(40, "PostSpeedOver5");
            //    tblPostedSpeed.Rows.Add(41, "PostSpeedOver10");
            //    tblPostedSpeed.Rows.Add(42, "PostSpeedOver15");

            //    this.cboPostedSpeed.DataSource = tblPostedSpeed;
            //    this.cboPostedSpeed.DataBind();
            //}
            try
            {
                DataSet dsPostedSpeed = new DataSet();
                StringReader strXML = null;


                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetPostedSpeedServiceConfiguration(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetPostedSpeedServiceConfiguration(sn.UserID, sn.SecId, ref xml), false))
                    {
                        return;
                    }
                if (xml == "")
                {
                    return;
                }

                strXML = new StringReader(xml);
                dsPostedSpeed.ReadXml(strXML);

                this.cboPostedSpeed.DataSource = dsPostedSpeed;
                cboPostedSpeed.DataBind();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }



        private void cboTimeZone_Fill()
        {
            try
            {
                cboTimeZone.SelectedIndex = -1;
                cboTimeZone.DataSource = null;

                DataTable tblTimeZone = new DataTable();
                tblTimeZone.Columns.Add("TimeZoneId", typeof(float));
                tblTimeZone.Columns.Add("TimeZoneName", typeof(string));

                object[] objRow;
                for (int i = -12; i < 14; i++)
                {

                    objRow = new object[2];
                    objRow[0] = i;
                    if (i != 0)
                    {
                        if (i < 0)
                        {
                            objRow[1] = "GMT" + i.ToString();
                        }
                        else
                        {
                            objRow[1] = "GMT+" + i.ToString();
                        }
                    }
                    else
                    {
                        objRow[1] = "GMT";
                    }

                    tblTimeZone.Rows.Add(objRow);
                    this.cboTimeZone.DataSource = tblTimeZone;
                    this.cboTimeZone.DataBind();

                    
                }

                //  Changes For TimeZone Feature start
                //  Adding New Time Zone GMT - 3:30 (-3.5)  | NewFoundLand
                
                objRow = new object[2];
                DataRow dtRow = tblTimeZone.NewRow();
                dtRow[0] = -3.5;
                dtRow[1] = "GMT-3:30";
                tblTimeZone.Rows.InsertAt(dtRow, 9);
                this.cboTimeZone.DataSource = tblTimeZone;
                this.cboTimeZone.DataBind();
                //  Changes For TimeZone Feature end
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        protected void cmdCustomFields_Click(object sender, EventArgs e)
        {
            if (this.lblVehicleId.Text == "")
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("PleaseSaveInfo");
                return;
            }

            Response.Redirect("frmvehicle_customfields.aspx?VehicleId=" + lblVehicleId.Text + "&LicensePlate=" + this.txtLicensePlate.Text);
        }

        protected void cmdWorkingHours_Click(object sender, EventArgs e)
        {
            if (this.lblVehicleId.Text == "")
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("PleaseSaveInfo");
                return;
            }

            Response.Redirect("frmvehicle_workinghours.aspx");
        }

        protected void cmdVehicleStatus_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleStatus.aspx?LicensePlate=" + lblLicensePlate.Text + "&VehicleId=" + lblVehicleId.Text);
        }

        protected void cboIconTypes_InitializeDataSource(object sender, ISNet.WebUI.WebCombo.DataSourceEventArgs e)
        {
            ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
            string xml = "";

            if (objUtil.ErrCheck(dbs.GetIconsInfo(sn.UserID, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbs.GetIconsInfo(sn.UserID, sn.SecId, ref xml), false))
                {
                    return;
                }

            DataSet ds = new DataSet();
            StringReader strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);
            e.DataSource = ds;


        }
        protected void cboIconTypes_InitializeRow(object sender, ISNet.WebUI.WebCombo.RowEventArgs e)
        {
            e.Row.Cells.GetNamedItem("IconTypeName").Image = e.Row.Cells.GetNamedItem("DefaultIconUrl").Text;

        }

        private void getCustomIcon()
        {
            string path = Server.MapPath("../images/CustomIcon");
            string[] icons = Directory.GetFiles(path, "Grey*.*");
            string[] organizationIcons = new string[] { };
            if (Directory.Exists(path + "\\" + sn.User.OrganizationId))
            {
                organizationIcons = Directory.GetFiles(path + "\\" + sn.User.OrganizationId, "Grey*.*");
            }
            //else
            //    organizationIcons = new string[] { };
            string selectedImagePath = string.Empty;

            if (!string.IsNullOrEmpty(this.lblVehicleId.Text))
            {
                Int64 VehicleId = Convert.ToInt64(this.lblVehicleId.Text);
                if (VehicleId > 0)
                    selectedImagePath = _vehicle.GetImagePath(VehicleId);
            }

            customIcon[] cs = new customIcon[icons.Length + organizationIcons.Length + 1];

            int i = 0;

            customIcon c0 = new customIcon();
            c0.text = "Select a Custom Icon";
            c0.value = "";
            c0.selected = false;
            c0.description = "";
            c0.imageSrc = "";

            cs[0] = c0;
            i++;

            foreach (string s in icons)
            {
                customIcon c = new customIcon();
                string shortPath = s.Replace(path + "\\", "");
                c.text = getIconText(shortPath);
                c.value = shortPath.Substring(shortPath.IndexOf("Grey") + 4);
                c.selected = selectedImagePath == c.value;
                c.description = "";
                c.imageSrc = "../images/CustomIcon/" + shortPath;

                cs[i] = c;
                i++;
            }

            foreach (string s in organizationIcons)
            {
                customIcon c = new customIcon();
                string shortPath = s.Replace(path + "\\", "");
                c.text = getIconText(shortPath);
                c.value = shortPath.Split('\\')[0] + "\\" + c.text + "." + shortPath.Split('\\')[1].Split('.')[1];
                c.selected = selectedImagePath == c.value;
                c.description = "";
                c.imageSrc = "../images/CustomIcon/" + shortPath;

                cs[i] = c;
                i++;
            }

            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            CustomIcon = serializer.Serialize(cs);


        }

        private class customIcon
        {
            public string text;
            public string value;
            public bool selected;
            public string description;
            public string imageSrc;
        }

        private string getIconText(string s)
        {
            s = s.Substring(s.IndexOf("Grey") + 4).Split('.')[0];
            return s;
        }

        bool validateDescriptionForHQ(string input)
        {
            return input.Contains(" ");
        }
    }
}
