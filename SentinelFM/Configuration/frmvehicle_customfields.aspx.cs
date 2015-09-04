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
    public partial class Configuration_frmvehicle_customfields : SentinelFMBasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {  
            if (!Page.IsPostBack)
            {
                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                if (!sn.User.ControlEnable(sn, 114))
                    cmdVehicleStatus.Visible = false;

                string VehicleId = Request.QueryString["VehicleId"];
                string LicensePlate = Request.QueryString["LicensePlate"];


                //SALMAN Mar 04,2013

                if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 999955)
                {
                    this.txtField1.Visible = false;
                    CboField1_Fill();
                }
                else
                {
                    this.cboField1.Visible = false;
                }

                if (VehicleId != null)
                {
                    ViewState["VehicleId"] = VehicleId;
                    ViewState["LicensePlate"] = LicensePlate;
                    this.lblVehicleId.Text = VehicleId;
                    this.lblLicensePlate.Text = LicensePlate;
                    LoadCustomFields();
                    Load3rdPartyVehicleAdditionalInfo();
                }
                else
                {
                    ClearFields();
                    ClearAdditionalFields();
                }
            }
        }

        //SALMAN Mar 04,2013
        private void CboField1_Fill()
        {
            try
            {
                DataSet dsField1 = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();


                if (objUtil.ErrCheck(dbo.GetUsersNameInfoByOrganizationByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetUsersNameInfoByOrganizationByLang(sn.UserID, sn.SecId, sn.User.OrganizationId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        cboField1.DataSource = null;
                        cboField1.DataBind();
                        cboField1.Items.Insert(0, new ListItem("Select User Name", "-1"));
                        return;
                    }

                strrXML = new StringReader(xml);
                dsField1.ReadXml(strrXML);

                cboField1.DataSource = dsField1;
                cboField1.DataBind();

                //cboField1.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectUserName"), "-1"));
                cboField1.Items.Insert(0, new ListItem("Select User Name", "-1"));
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

        private void LoadCustomFields()
        {
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
            
            string xml = "";
            if (objUtil.ErrCheck(dbv.GetVehicleAdditionalInfoXML(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), ref xml), false))
                if (objUtil.ErrCheck(dbv.GetVehicleAdditionalInfoXML(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), ref xml), true))
                {
                }

            if (xml == "")
            {
                ClearFields();
                return;
            }

            DataSet ds = new DataSet();
            StringReader strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string field1Text = ds.Tables[0].Rows[0]["Field1"].ToString();
                if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 999955)
                {
                    cboField1.SelectedIndex = cboField1.Items.IndexOf(cboField1.Items.FindByText(field1Text));
                    //cboField1.Items.FindByText(ds.Tables[0].Rows[0]["Field1"].ToString()).Selected = true;
                }

                //this.txtField1.Text = ds.Tables[0].Rows[0]["Field1"].ToString();
                this.txtField1.Text = field1Text;
                this.txtField2.Text = ds.Tables[0].Rows[0]["Field2"].ToString();
                this.txtField3.Text = ds.Tables[0].Rows[0]["Field3"].ToString();
                this.txtField4.Text = ds.Tables[0].Rows[0]["Field4"].ToString();
                this.txtField5.Text = ds.Tables[0].Rows[0]["Field5"].ToString();
                this.txtVehicleWeight.Text = ds.Tables[0].Rows[0]["VehicleWeight"].ToString();
                string vwu = "Select Unit";
                vwu = (DBNull.Value == ds.Tables[0].Rows[0]["VehicleWtUnit"] || "" == ds.Tables[0].Rows[0]["VehicleWtUnit"]) ? vwu : ds.Tables[0].Rows[0]["VehicleWtUnit"].ToString();
                cboVehicleWtUnit.SelectedIndex = cboVehicleWtUnit.Items.IndexOf(cboVehicleWtUnit.Items.FindByText(vwu));
                this.txtFuelCapacity.Text = ds.Tables[0].Rows[0]["FuelCapacity"].ToString();
                this.txtFuelBurnRate.Text = ds.Tables[0].Rows[0]["FuelBurnRate"].ToString();
            }
            else
            {
                ClearFields();
            }

        }

        private void Load3rdPartyVehicleAdditionalInfo()
        {
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
            
            string xml = "";
            if (objUtil.ErrCheck(dbv.Get3rdPartyVehicleAdditionalInfoXML(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), ref xml), false))
                if (objUtil.ErrCheck(dbv.Get3rdPartyVehicleAdditionalInfoXML(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), ref xml), true))
                {
                    plh3rdPartyVAI.Visible = false;
                }

            if (xml == "")
            {
                ClearAdditionalFields();
                return;
            }

            DataSet ds = new DataSet();
            StringReader strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                plh3rdPartyVAI.Visible = true;

                this.txtEquipNumber.Text = ds.Tables[0].Rows[0]["EquipNbr"].ToString(); ;
                this.txtSAPEquipNumber.Text = ds.Tables[0].Rows[0]["SAPEquipNbr"].ToString();
                this.txtLegacyEquipNumber.Text = ds.Tables[0].Rows[0]["LegacyEquipNbr"].ToString();
                this.txtObjectType.Text = ds.Tables[0].Rows[0]["ObjectType"].ToString();
                this.txtDOTNumber.Text = ds.Tables[0].Rows[0]["DOTNbr"].ToString();
                this.txtEquipCategory.Text = ds.Tables[0].Rows[0]["EquipCategory"].ToString();
                //this.txtAcquireDate.Text = (DBNull.Value== ds.Tables[0].Rows[0]["AcquireDate"]) ? string.Empty: ds.Tables[0].Rows[0]["AcquireDate"].ToString();//??
                //this.txtRetireDate.Text = (DBNull.Value == ds.Tables[0].Rows[0]["RetireDate"]) ? string.Empty: ds.Tables[0].Rows[0]["RetireDate"].ToString();//??
                //this.txtSoldDate.Text = (DBNull.Value == ds.Tables[0].Rows[0]["SoldDate"]) ? string.Empty: ds.Tables[0].Rows[0]["SoldDate"].ToString();//??
                
                if (ds.Tables[0].Columns.Contains("AcquireDate") && DBNull.Value != ds.Tables[0].Rows[0]["AcquireDate"])
                {
                    //string AD = ds.Tables[0].Rows[0]["AcquireDate"].ToString();//??
                    //this.txtAcquireDate.Text = Convert.ToDateTime(AD).ToString(base.GetLocalResourceObject("resDateFormat").ToString());
                    //Convert.ToDateTime(row["AcquireDate"]).ToString(base.GetLocalResourceObject("resDateFormat").ToString());

                    this.txtAcquireDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["AcquireDate"].ToString()).ToString(base.GetLocalResourceObject("resDateFormat").ToString());
                }
                else
                {
                    this.txtAcquireDate.Text = string.Empty;//?? 
                }

                if (ds.Tables[0].Columns.Contains("RetireDate") && DBNull.Value != ds.Tables[0].Rows[0]["RetireDate"])
                {
                    this.txtRetireDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["RetireDate"].ToString()).ToString(base.GetLocalResourceObject("resDateFormat").ToString());
                }
                else
                {
                    this.txtRetireDate.Text = string.Empty;//??
                }

                if (ds.Tables[0].Columns.Contains("SoldDate") && DBNull.Value != ds.Tables[0].Rows[0]["SoldDate"])
                {
                    this.txtSoldDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["SoldDate"].ToString()).ToString(base.GetLocalResourceObject("resDateFormat").ToString());
                }
                else
                {
                    this.txtSoldDate.Text = string.Empty;//??
                }

                this.txtObjectPrefix.Text = ds.Tables[0].Rows[0]["ObjectPrefix"].ToString();
                this.txtOwningDistrict.Text = ds.Tables[0].Rows[0]["OwningDistrict"].ToString();
                this.txtProjectNumber.Text = ds.Tables[0].Rows[0]["ProjectNbr"].ToString();
                this.txtTotalCtrReading_1.Text = ds.Tables[0].Rows[0]["TotalCtrReading_1"].ToString();
                this.txtTotalCtrReading_2.Text = ds.Tables[0].Rows[0]["TotalCtrReading_2"].ToString();
                this.txtCtrReadingUom_1.Text = ds.Tables[0].Rows[0]["CtrReadingUom_1"].ToString();
                this.txtCtrReadingUom_2.Text = ds.Tables[0].Rows[0]["CtrReadingUom_2"].ToString();
                this.txtShortDesc.Text = ds.Tables[0].Rows[0]["ShortDesc"].ToString();
            }
            else
            {
                ClearAdditionalFields();
            }

        }

        private void ClearFields()
        {
            if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 999955)
            {
                cboField1.SelectedIndex = -1;
            }
            
            this.txtField1.Text = "";
            this.txtField2.Text = "";
            this.txtField3.Text = "";
            this.txtField4.Text = "";
            this.txtField5.Text = "";
            this.txtVehicleWeight.Text = "";
            this.txtFuelCapacity.Text = "";
            this.txtFuelBurnRate.Text = "";
        }

        void ClearAdditionalFields()
        {
            this.txtEquipNumber.Text = string.Empty;
            this.txtSAPEquipNumber.Text = string.Empty;
            this.txtLegacyEquipNumber.Text = string.Empty;
            this.txtObjectType.Text = string.Empty;
            this.txtDOTNumber.Text = string.Empty;
            this.txtEquipCategory.Text = string.Empty;
            this.txtAcquireDate.Text = string.Empty;
            this.txtRetireDate.Text = string.Empty;
            this.txtSoldDate.Text = string.Empty;
            this.txtObjectPrefix.Text = string.Empty;
            this.txtOwningDistrict.Text = string.Empty;
            this.txtProjectNumber.Text = string.Empty;
            this.txtTotalCtrReading_1.Text = string.Empty;
            this.txtTotalCtrReading_2.Text = string.Empty;
            this.txtCtrReadingUom_1.Text = string.Empty;
            this.txtCtrReadingUom_2.Text = string.Empty;
            this.txtShortDesc.Text = string.Empty;

            plh3rdPartyVAI.Visible = false;
        }

        protected void cmdInfo_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmvehicle_add_edit.aspx?LicensePlate=" + lblLicensePlate.Text);
        }

        protected void cmdVehicleStatus_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleStatus.aspx?LicensePlate=" + lblLicensePlate.Text + "&VehicleId=" + lblVehicleId.Text);
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            if (sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999957)
            {
                if (!validateField2ForHQ(txtField2.Text))
                {
                    //lblMessage.Visible = true;
                    //lblMessage.Text = "Seul un soutien D1-D9 ou le modèle P1-P9 ou T1-T9.";//"Only support D1-D9 or P1-P9 or T1-T9 pattern.";
                    lblMessage.Text = (string)base.GetLocalResourceObject("Field2CustomValidationText");
                    return;
                }
            }
            //string xml = "";
            if (!ValidateWeight())
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = string.Format("{0} [{1}]", (string)base.GetLocalResourceObject("lblMessage_Text_UpdateFailed"),"Check Select Unit");
                return;
            }
            //if (objUtil.ErrCheck(dbv.UpdateVehicleAdditionalInfo(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), this.txtField1.Text, this.txtField2.Text, this.txtField3.Text, txtField4.Text,txtField5.Text ), false))
            if (objUtil.ErrCheck(dbv.UpdateVehicleAdditionalInfo(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), /*this.txtField1.Text*/ GetField1Text(), this.txtField2.Text, this.txtField3.Text, txtField4.Text, txtField5.Text, GetVehicleWeight(), GetVehicleWtUnit(), GetFuelCapacity(), GetFuelBurnRate()), false))
                if (objUtil.ErrCheck(dbv.UpdateVehicleAdditionalInfo(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), /*this.txtField1.Text*/ GetField1Text(), this.txtField2.Text, this.txtField3.Text, txtField4.Text, txtField5.Text, GetVehicleWeight(), GetVehicleWtUnit(), GetFuelCapacity(), GetFuelBurnRate()), false))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateFailed");
                    return;
                }

            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SaveSuccess");

            //3rd Party Vehicle Additional Info
            if (plh3rdPartyVAI.Visible)
            {
                DateTime? acquireDate = GetDateTime(this.txtAcquireDate.Text);//new DateTime?();
                DateTime? retireDate = GetDateTime(this.txtRetireDate.Text);//new DateTime?();
                DateTime? soldDate = GetDateTime(this.txtSoldDate.Text);//new DateTime?();

                //DateTime? acquireDate = new DateTime?();
                //DateTime? retireDate = new DateTime?();
                //DateTime? soldDate = new DateTime?();

                //if (!string.IsNullOrEmpty(this.txtAcquireDate.Text))
                //    acquireDate = Convert.ToDateTime(this.txtAcquireDate.Text).ToUniversalTime();

                //if (!string.IsNullOrEmpty(this.txtRetireDate.Text))
                //    retireDate = Convert.ToDateTime(this.txtRetireDate.Text).ToUniversalTime();

                //if (!string.IsNullOrEmpty(this.txtSoldDate.Text))
                //    soldDate = Convert.ToDateTime(this.txtSoldDate.Text).ToUniversalTime();

                //acquireDate = Convert.ToDateTime(DateTime.Now);
                //retireDate = acquireDate;
                //soldDate = acquireDate;

                if (objUtil.ErrCheck(dbv.Update3rdPartyVehicleAdditionalInfo(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), this.txtEquipNumber.Text, this.txtSAPEquipNumber.Text, this.txtLegacyEquipNumber.Text, this.txtObjectType.Text, this.txtDOTNumber.Text, this.txtEquipCategory.Text, acquireDate, retireDate, soldDate, this.txtObjectPrefix.Text, this.txtOwningDistrict.Text, this.txtProjectNumber.Text, GetTotalCtrReading(this.txtTotalCtrReading_1.Text), GetTotalCtrReading(this.txtTotalCtrReading_2.Text), this.txtCtrReadingUom_1.Text, this.txtCtrReadingUom_2.Text, this.txtShortDesc.Text), false))
                    if (objUtil.ErrCheck(dbv.Update3rdPartyVehicleAdditionalInfo(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), this.txtEquipNumber.Text, this.txtSAPEquipNumber.Text, this.txtLegacyEquipNumber.Text, this.txtObjectType.Text, this.txtDOTNumber.Text, this.txtEquipCategory.Text, acquireDate, retireDate, soldDate, this.txtObjectPrefix.Text, this.txtOwningDistrict.Text, this.txtProjectNumber.Text, GetTotalCtrReading(this.txtTotalCtrReading_1.Text), GetTotalCtrReading(this.txtTotalCtrReading_2.Text), this.txtCtrReadingUom_1.Text, this.txtCtrReadingUom_2.Text, this.txtShortDesc.Text), false))
                    {
                        //this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateFailed");
                        return;
                    }
                //Update3rdPartyVehicleAdditionalInfo
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SaveSuccess");
            }
        }

        protected void cmdWorkingHours_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmvehicle_workinghours.aspx");
        }

        private string GetField1Text()
        {
            string ret = this.txtField1.Text;
            if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 999955)
            {
                //string t = cboField1.SelectedItem.Text;
                ret = (cboField1.SelectedIndex > 0 ? cboField1.Items[cboField1.SelectedIndex].Text : string.Empty);
            }
            return ret;
        }

        bool validateField2ForHQ(string input)
        {
            bool res = false;
            var regex =  @"^[DPTdpt]{1}\d{1}$";

            //var regex = @"^\w{1}\d{1}$";
            var match = System.Text.RegularExpressions.Regex.Match(input, regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            if (match.Success)
            {
                if (input.Contains("0")) 
                    res = false;
                else
                    res = true;
            }
            return res;
        }

        int? GetVehicleWeight()
        {
            int _vw;
            int.TryParse(this.txtVehicleWeight.Text, out _vw);
            //int? vw = null;
            //if (_vw > 0)
            //    vw = _vw;
            //int? t = (_vw > 0 ? (int?)_vw : null);
            int? vw = (_vw > 0 ? (int?)_vw : null);

            //string ret = this.txtVehicleWeight.Text;
            //if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 999955)
            //{
            //    ret = (cboField1.SelectedIndex > 0 ? cboField1.Items[cboField1.SelectedIndex].Text : string.Empty);
            //}
            return vw;
        }

        string GetVehicleWtUnit()
        {
            string ret = (cboVehicleWtUnit.SelectedIndex > 0 ? cboVehicleWtUnit.Items[cboVehicleWtUnit.SelectedIndex].Text : null);
            return ret;
        }

        float? GetFuelCapacity()
        {
            float _fc;
            float.TryParse(this.txtFuelCapacity.Text, out _fc);
            float? fc = (_fc > 0 ? (float?)_fc : null);
            return fc;
        }

        float? GetFuelBurnRate()
        {
            float _fbr;
            float.TryParse(this.txtFuelBurnRate.Text, out _fbr);
            float? fbr = (_fbr > 0 ? (float?)_fbr : null);
            return fbr;
        }

        bool ValidateWeight()
        {
            bool ret = false;
            int vw;
            int.TryParse(this.txtVehicleWeight.Text, out vw);

            bool u = cboVehicleWtUnit.SelectedIndex > 0;
            if (vw > 0 && cboVehicleWtUnit.SelectedIndex > 0)
                ret = true;
            else if (vw == -1 && cboVehicleWtUnit.SelectedIndex == 0)
                ret = true;
            else if (vw == 0 && cboVehicleWtUnit.SelectedIndex == 0)
                ret = true;
            
            return ret;
        }

        decimal? GetTotalCtrReading(string input)
        {
            decimal _tcr;
            decimal.TryParse(input, out _tcr);
            decimal? tcr = (_tcr > 0 ? (decimal?)_tcr : null);
            return _tcr;
        }

        DateTime? GetDateTime(string input)
        {
            DateTime? _Date = new DateTime?();

            //if (!string.IsNullOrEmpty(this.txtAcquireDate.Text))
            //    _Date = Convert.ToDateTime(this.txtAcquireDate.Text).ToUniversalTime();

            //if (!string.IsNullOrEmpty(this.txtRetireDate.Text))
            //    _Date = Convert.ToDateTime(this.txtRetireDate.Text).ToUniversalTime();

            //if (!string.IsNullOrEmpty(this.txtSoldDate.Text))
            //    _Date = Convert.ToDateTime(this.txtSoldDate.Text).ToUniversalTime();
            try
            {
                if (!string.IsNullOrEmpty(input))
                    _Date = Convert.ToDateTime(input);//.ToUniversalTime();
            }
            catch{}
            return _Date;
        }
    }
}