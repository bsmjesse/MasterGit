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
    public partial class Maintenance_frmMaintenanceSettings : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string VehicleId = Request.QueryString["VehicleID"];

            if (!Page.IsPostBack)
            {
                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                cboTimeZone_Fill();
                cboTimeZone.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboTimeZone_Item_0"), "-1"));
                VehicleInfoLoad_NewTZ(Convert.ToInt64(VehicleId));
            }
        }
        // Changes for TimeZone Feature start

        private void VehicleInfoLoad_NewTZ(Int64 VehicleId)
        {
            DataSet dsv = new DataSet();

            string xml = "";
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            if (objUtil.ErrCheck(dbv.GetVehicleMaintenanceInfoXML_NewTZ(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                if (objUtil.ErrCheck(dbv.GetVehicleMaintenanceInfoXML_NewTZ(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                {
                    return;
                }

            dsv.ReadXml(new StringReader(xml));

            if (dsv.Tables.Count > 0 && dsv.Tables[0].Rows.Count > 0)
            {
                this.lblBoxId.Text = dsv.Tables[0].Rows[0]["BoxId"].ToString();
                this.lblVehicleName.Text = dsv.Tables[0].Rows[0]["Description"].ToString();
                this.lblVehicleId.Text = dsv.Tables[0].Rows[0]["VehicleId"].ToString();
                this.lblCurEngine.Text = dsv.Tables[0].Rows[0]["CurrentEngHrs"].ToString();

                if (dsv.Tables[0].Rows[0]["EngHrsSrvInterval"].ToString() != "0")
                {
                    this.txtEngineLastService.Text = dsv.Tables[0].Rows[0]["LastSrvEngHrs"].ToString();
                    this.txtEngineServiceInterval.Text = dsv.Tables[0].Rows[0]["EngHrsSrvInterval"].ToString();

                    this.txtLastOdomServ.Text = "0";
                    this.txtOdomServiceInterval.Text = "0";
                    this.tblEngineOpt.Visible = true;
                    this.tblOdometerOpt.Visible = false;
                    this.optMaintenanceType.SelectedIndex = 1;

                    this.lblNextEngine.Text = Convert.ToString(Math.Round(Convert.ToSingle(txtEngineLastService.Text) + Convert.ToSingle(txtEngineServiceInterval.Text)));
                }
                else if (dsv.Tables[0].Rows[0]["MaxSrvInterval"].ToString() != "0")
                {
                    this.txtEngineLastService.Text = "0";
                    this.txtEngineServiceInterval.Text = "0";

                    if (sn.User.UnitOfMes == 0.6214)
                    {
                        this.lblOdomCurOdometerValue.Text = this.lblOdomCurOdometerValue.Text + " (mi)";
                        this.lblOdomLastServiced.Text = this.lblOdomLastServiced.Text + " (mi)";
                        this.lblOdomServiceInterval.Text = this.lblOdomServiceInterval.Text + " (mi)";
                        this.txtLastOdomServ.Text = Convert.ToString(Math.Round(Convert.ToSingle(dsv.Tables[0].Rows[0]["LastSrvOdo"].ToString()) * 0.6214, 2));
                        this.txtOdomServiceInterval.Text = Convert.ToString(Math.Round(Convert.ToSingle(dsv.Tables[0].Rows[0]["MaxSrvInterval"].ToString()) * 0.6214, 2));
                        this.lblCurOdom.Text = Convert.ToString(Math.Round(Convert.ToSingle(dsv.Tables[0].Rows[0]["CurrentOdo"].ToString()) * 0.6214, 2));
                    }
                    else
                    {
                        this.lblOdomCurOdometerValue.Text = this.lblOdomCurOdometerValue.Text + " (km)";
                        this.lblOdomLastServiced.Text = this.lblOdomLastServiced.Text + " (km)";
                        this.lblOdomServiceInterval.Text = this.lblOdomServiceInterval.Text + " (km)";

                        this.txtLastOdomServ.Text = dsv.Tables[0].Rows[0]["LastSrvOdo"].ToString();
                        this.txtOdomServiceInterval.Text = dsv.Tables[0].Rows[0]["MaxSrvInterval"].ToString();
                        this.lblCurOdom.Text = dsv.Tables[0].Rows[0]["CurrentOdo"].ToString();
                    }

                    this.lblNextOdom.Text = Convert.ToString(Convert.ToSingle(this.txtLastOdomServ.Text) + Convert.ToSingle(this.txtOdomServiceInterval.Text));
                    this.tblEngineOpt.Visible = false;
                    this.tblOdometerOpt.Visible = true;
                    this.optMaintenanceType.SelectedIndex = 0;
                }
                else
                {
                    this.tblEngineOpt.Visible = false;
                    this.tblOdometerOpt.Visible = true;
                    this.optMaintenanceType.SelectedIndex = 0;
                    this.txtEngineLastService.Text = "0";
                    this.txtEngineServiceInterval.Text = "0";
                    this.txtLastOdomServ.Text = "0";
                    this.txtOdomServiceInterval.Text = "0";
                }

                this.txtEmail.Text = dsv.Tables[0].Rows[0]["Email"].ToString().TrimEnd();
                cboTimeZone_Fill();
                this.cboTimeZone.SelectedIndex = -1;

                if (this.txtEmail.Text != "")
                {
                    for (int i = 0; i < cboTimeZone.Items.Count; i++)
                    {
                        if (Convert.ToSingle(cboTimeZone.Items[i].Value.TrimEnd()) == Convert.ToSingle(dsv.Tables[0].Rows[0]["TimeZone"].ToString().TrimEnd()))
                        {
                            cboTimeZone.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    cboTimeZone.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboTimeZone_Item_0"), "-1"));
                }
                this.chkDayLight.Checked = Convert.ToBoolean(dsv.Tables[0].Rows[0]["AutoAdjustDayLightSaving"].ToString().TrimEnd());
            }
            else
            {
                this.tblEngineOpt.Visible = false;
                this.tblOdometerOpt.Visible = true;
                this.optMaintenanceType.SelectedIndex = 0;
                this.txtEngineLastService.Text = "0";
                this.txtEngineServiceInterval.Text = "0";
                this.txtLastOdomServ.Text = "0";
                this.txtOdomServiceInterval.Text = "0";
                this.txtEmail.Text = "";
                this.lblCurEngine.Text = "0";
                this.lblCurOdom.Text = "0";
            }
        }

        // Changes for TimeZone Feature end


        private void VehicleInfoLoad(Int64 VehicleId)
        {          
            DataSet dsv = new DataSet();

            string xml = "";
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            if (objUtil.ErrCheck(dbv.GetVehicleMaintenanceInfoXML(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                if (objUtil.ErrCheck(dbv.GetVehicleMaintenanceInfoXML(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                {
                    return;
                }

            dsv.ReadXml(new StringReader(xml));

            if (dsv.Tables.Count > 0 && dsv.Tables[0].Rows.Count > 0)
            {
                this.lblBoxId.Text = dsv.Tables[0].Rows[0]["BoxId"].ToString();
                this.lblVehicleName.Text = dsv.Tables[0].Rows[0]["Description"].ToString();
                this.lblVehicleId.Text = dsv.Tables[0].Rows[0]["VehicleId"].ToString();
                this.lblCurEngine.Text = dsv.Tables[0].Rows[0]["CurrentEngHrs"].ToString();

                if (dsv.Tables[0].Rows[0]["EngHrsSrvInterval"].ToString() != "0")
                {
                    this.txtEngineLastService.Text = dsv.Tables[0].Rows[0]["LastSrvEngHrs"].ToString();
                    this.txtEngineServiceInterval.Text = dsv.Tables[0].Rows[0]["EngHrsSrvInterval"].ToString();

                    this.txtLastOdomServ.Text = "0";
                    this.txtOdomServiceInterval.Text = "0";
                    this.tblEngineOpt.Visible = true;
                    this.tblOdometerOpt.Visible = false;
                    this.optMaintenanceType.SelectedIndex = 1;

                    this.lblNextEngine.Text = Convert.ToString(Math.Round(Convert.ToSingle(txtEngineLastService.Text) + Convert.ToSingle(txtEngineServiceInterval.Text)));
                }
                else if (dsv.Tables[0].Rows[0]["MaxSrvInterval"].ToString() != "0")
                {
                    this.txtEngineLastService.Text = "0";
                    this.txtEngineServiceInterval.Text = "0";

                    if (sn.User.UnitOfMes == 0.6214)
                    {
                        this.lblOdomCurOdometerValue.Text = this.lblOdomCurOdometerValue.Text + " (mi)";
                        this.lblOdomLastServiced.Text = this.lblOdomLastServiced.Text + " (mi)";
                        this.lblOdomServiceInterval.Text = this.lblOdomServiceInterval.Text + " (mi)";
                        this.txtLastOdomServ.Text = Convert.ToString(Math.Round(Convert.ToSingle(dsv.Tables[0].Rows[0]["LastSrvOdo"].ToString()) * 0.6214, 2));
                        this.txtOdomServiceInterval.Text = Convert.ToString(Math.Round(Convert.ToSingle(dsv.Tables[0].Rows[0]["MaxSrvInterval"].ToString()) * 0.6214, 2));
                        this.lblCurOdom.Text = Convert.ToString(Math.Round(Convert.ToSingle(dsv.Tables[0].Rows[0]["CurrentOdo"].ToString()) * 0.6214, 2));
                    }
                    else
                    {
                        this.lblOdomCurOdometerValue.Text = this.lblOdomCurOdometerValue.Text + " (km)";
                        this.lblOdomLastServiced.Text = this.lblOdomLastServiced.Text + " (km)";
                        this.lblOdomServiceInterval.Text = this.lblOdomServiceInterval.Text + " (km)";

                        this.txtLastOdomServ.Text = dsv.Tables[0].Rows[0]["LastSrvOdo"].ToString();
                        this.txtOdomServiceInterval.Text = dsv.Tables[0].Rows[0]["MaxSrvInterval"].ToString();
                        this.lblCurOdom.Text = dsv.Tables[0].Rows[0]["CurrentOdo"].ToString();
                    }

                    this.lblNextOdom.Text = Convert.ToString(Convert.ToSingle(this.txtLastOdomServ.Text) + Convert.ToSingle(this.txtOdomServiceInterval.Text));
                    this.tblEngineOpt.Visible = false;
                    this.tblOdometerOpt.Visible = true;
                    this.optMaintenanceType.SelectedIndex = 0;
                }
                else
                {
                    this.tblEngineOpt.Visible = false;
                    this.tblOdometerOpt.Visible = true;
                    this.optMaintenanceType.SelectedIndex = 0;
                    this.txtEngineLastService.Text = "0";
                    this.txtEngineServiceInterval.Text = "0";
                    this.txtLastOdomServ.Text = "0";
                    this.txtOdomServiceInterval.Text = "0";
                }

                this.txtEmail.Text = dsv.Tables[0].Rows[0]["Email"].ToString().TrimEnd();
                cboTimeZone_Fill();
                this.cboTimeZone.SelectedIndex = -1;

                if (this.txtEmail.Text != "")
                {
                    for (int i = 0; i < cboTimeZone.Items.Count; i++)
                    {
                        if (Convert.ToInt16(cboTimeZone.Items[i].Value.TrimEnd()) == Convert.ToInt32(dsv.Tables[0].Rows[0]["TimeZone"].ToString().TrimEnd()))
                        {
                            cboTimeZone.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    cboTimeZone.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboTimeZone_Item_0"), "-1"));
                }
                this.chkDayLight.Checked = Convert.ToBoolean(dsv.Tables[0].Rows[0]["AutoAdjustDayLightSaving"].ToString().TrimEnd());
            }
            else
            {
                this.tblEngineOpt.Visible = false;
                this.tblOdometerOpt.Visible = true;
                this.optMaintenanceType.SelectedIndex = 0;
                this.txtEngineLastService.Text = "0";
                this.txtEngineServiceInterval.Text = "0";
                this.txtLastOdomServ.Text = "0";
                this.txtOdomServiceInterval.Text = "0";
                this.txtEmail.Text = "";
                this.lblCurEngine.Text = "0";
                this.lblCurOdom.Text = "0";
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            if (optMaintenanceType.SelectedItem.Value == "1")
            {
                this.txtEngineLastService.Text = "0";
                this.txtEngineServiceInterval.Text = "0";

                if (!clsUtility.IsNumeric(this.txtLastOdomServ.Text)
                    || !clsUtility.IsNumeric(this.txtOdomServiceInterval.Text))
                {
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidOdometerInfo");
                    return;
                }
            }
            else if (optMaintenanceType.SelectedItem.Value == "2")
            {
                this.txtLastOdomServ.Text = "0";
                this.txtOdomServiceInterval.Text = "0";

                if (!clsUtility.IsNumeric(this.txtEngineLastService.Text)
                    || !clsUtility.IsNumeric(this.txtEngineServiceInterval.Text))
                {
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidEngineInfo");
                    return;
                }
            }
            
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
            Int16 DayLightSaving = Convert.ToInt16(objUtil.IsDayLightSaving(chkDayLight.Checked));

            Single OdomServiceInterval = 0;
            Single LastOdometer = 0;
            Single CurOdometer = 0;

            if (sn.User.UnitOfMes == 0.6214)
            {
                OdomServiceInterval = Convert.ToSingle(Convert.ToSingle(this.txtOdomServiceInterval.Text) / 0.6214);
                LastOdometer = Convert.ToSingle(Convert.ToSingle(this.txtLastOdomServ.Text) / 0.6214);
                CurOdometer = Convert.ToSingle(Convert.ToSingle(this.lblCurOdom.Text) / 0.6214);
            }
            else
            {
                OdomServiceInterval = Convert.ToSingle(this.txtOdomServiceInterval.Text);
                LastOdometer = Convert.ToSingle(this.txtLastOdomServ.Text);
                CurOdometer = Convert.ToSingle(this.lblCurOdom.Text);
            }
            //if (objUtil.ErrCheck(dbv.AddVehicleMaintenanceInfo(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), LastOdometer, CurOdometer, OdomServiceInterval, Convert.ToInt32(this.txtEngineLastService.Text), Convert.ToInt32(this.lblCurEngine.Text), Convert.ToInt32(this.txtEngineServiceInterval.Text), this.txtEmail.Text, Convert.ToInt32(this.cboTimeZone.SelectedItem.Value), DayLightSaving, Convert.ToInt16(this.chkDayLight.Checked)), false))
            //    if (objUtil.ErrCheck(dbv.AddVehicleMaintenanceInfo(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), LastOdometer, CurOdometer, OdomServiceInterval, Convert.ToInt32(this.txtEngineLastService.Text), Convert.ToInt32(this.lblCurEngine.Text), Convert.ToInt32(this.txtEngineServiceInterval.Text), this.txtEmail.Text, Convert.ToInt32(this.cboTimeZone.SelectedItem.Value), DayLightSaving, Convert.ToInt16(this.chkDayLight.Checked)), true ))
            //    {

            //        return;
            //    }
            Response.Write("<script language='javascript'>window.opener.location.href='frmMaintenance.aspx';window.close()</script>");
        }

        protected void optMaintenanceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (optMaintenanceType.SelectedItem.Value == "1")
            {
                this.tblOdometerOpt.Visible = true;
                this.tblEngineOpt.Visible = false;
            }
            else if (optMaintenanceType.SelectedItem.Value == "2")
            {
                this.tblOdometerOpt.Visible = false;
                this.tblEngineOpt.Visible = true;
            }
        }

        protected void cmdCancel_Click(object sender, EventArgs e) { }

        protected void cmdMaintenaceCompl_Click(object sender, EventArgs e)
        {
            this.txtLastOdomServ.Text = this.lblCurOdom.Text;
            this.lblNextOdom.Text = Convert.ToString(Convert.ToSingle(this.txtLastOdomServ.Text) + Convert.ToSingle(this.txtOdomServiceInterval.Text));

            this.txtEngineLastService.Text = this.lblCurEngine.Text;
            this.lblNextEngine.Text = Convert.ToString(Convert.ToSingle(this.txtEngineLastService.Text) + Convert.ToSingle(this.txtEngineServiceInterval.Text));
        }

        protected void cmdEditOdomSrv_Click(object sender, EventArgs e)
        {
            this.cmdCancelOdomServ.Visible = true;
            this.cmdEditOdomSrv.Visible = false;
            this.cmdSaveOdomServ.Visible = true;
            this.txtLastOdomServ.Enabled = true;
        }

        protected void cmdCancelOdomServ_Click(object sender, EventArgs e)
        {
            this.cmdCancelOdomServ.Visible = false;
            this.cmdEditOdomSrv.Visible = true;
            this.cmdSaveOdomServ.Visible = false;
            this.txtLastOdomServ.Enabled = false;
        }

        protected void cmdSaveOdomServ_Click(object sender, EventArgs e)
        {
            if (Convert.ToSingle(this.lblCurOdom.Text) > Convert.ToSingle(this.txtLastOdomServ.Text))
            {
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_TextInvalidLastOdometer");
                return;
            }
            this.lblMessage.Text = "";
            this.cmdCancelOdomServ.Visible = false;
            this.cmdEditOdomSrv.Visible = true;
            this.cmdSaveOdomServ.Visible = false;
            this.txtLastOdomServ.Enabled = false;
        }

        protected void cmdEditEngineSrv_Click(object sender, EventArgs e)
        {
            this.txtEngineLastService.Enabled = true;
            this.cmdSaveEngSrv.Visible = true;
            this.cmdCancelEngSrv.Visible = true;
            this.cmdEditEngineSrv.Visible = false;
        }

        protected void cmdCancelEngSrv_Click(object sender, EventArgs e)
        {
            this.txtEngineLastService.Enabled = false;
            this.cmdSaveEngSrv.Visible = false;
            this.cmdCancelEngSrv.Visible = false;
            this.cmdEditEngineSrv.Visible = true;
        }

        protected void cmdSaveEngSrv_Click(object sender, EventArgs e)
        {
            this.txtEngineLastService.Enabled = false;
            this.cmdSaveEngSrv.Visible = false;
            this.cmdCancelEngSrv.Visible = false;
            this.cmdEditEngineSrv.Visible = true;
        }
    }
}