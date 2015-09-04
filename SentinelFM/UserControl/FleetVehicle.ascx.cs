using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using Telerik.Web.UI;

namespace SentinelFM
{
    public partial class UserControl_FleetVehicle : System.Web.UI.UserControl
    {
        public event EventHandler Fleet_SelectedIndexChanged;
        public event EventHandler Vehicle_SelectedIndexChanged;

        public RadAjaxManager radAjaxManager1 = null;
        public string radAjaxLoadingPanel1 = null;
        public string radUpdatedControl = null;
        public bool isLoadDefault = false;
        protected SentinelFMSession sn = null;
        clsUtility objUtil = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            objUtil = new clsUtility(sn);
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                return;
            }
            if (!IsPostBack)
            {
                CboFleet_Fill();
                if (isLoadDefault)
                {
                    if (sn.User.DefaultFleet != -1)
                    {
                        cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindItemByValue(sn.User.DefaultFleet.ToString()));
                    }
                    else this.cboFleet.SelectedIndex = 0;
                    CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
                    this.cboVehicle.SelectedIndex = 0;
                }
            }
            if (Vehicle_SelectedIndexChanged != null)
            {
                cboVehicle.AutoPostBack = true;
                cboVehicle.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(Vehicle_SelectedIndexChanged);
            }
            //SetLoadingPanel();

        }

        private void CboFleet_Fill()
        {
            try
            {
                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();
                cboFleet.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("StringSelectFleet"), "0"));
                //cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void CboVehicle_Fill(int fleetId)
        {
            try
            {
                DataSet dsVehicle;
                dsVehicle = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                    {
                        cboVehicle.Items.Clear();
                        cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("StringSelectVehicle"), "0"));
                        return;
                    }
                if (xml == "")
                {
                    cboVehicle.Items.Clear();
                    cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("StringSelectVehicle"), "0"));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();
                cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("EntireFleet"), "-999"));
                cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("StringSelectVehicle"), "0"));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
            else
                this.cboVehicle.Items.Clear();

            if (Fleet_SelectedIndexChanged != null)
            {
                Fleet_SelectedIndexChanged(sender, e);
            }
        }

        private void SetLoadingPanel()
        {
            if (radAjaxManager1 == null || radAjaxLoadingPanel1 == null || radUpdatedControl == null) return;
            if (cboFleet.AutoPostBack && Fleet_SelectedIndexChanged != null)
            {
                AjaxSetting ajs = new AjaxSetting(cboFleet.ID);
                ajs.UpdatedControls.Add(new AjaxUpdatedControl(radUpdatedControl, radAjaxLoadingPanel1));
                radAjaxManager1.AjaxSettings.Add(ajs);
            }

            if (cboVehicle.AutoPostBack)
            {
                AjaxSetting ajs = new AjaxSetting(cboVehicle.ID);
                ajs.UpdatedControls.Add(new AjaxUpdatedControl(radUpdatedControl, radAjaxLoadingPanel1));
                radAjaxManager1.AjaxSettings.Add(ajs);
            }

        }

        public string GetSelectedFleet()
        {
            return cboFleet.SelectedValue;
        }

        public string GetSelectedVehicle()
        {
            return cboVehicle.SelectedValue;
        }

        public string GetAllSelectedVehicle()
        {
            string selectedVehicles = string.Empty;
            if (cboVehicle.SelectedValue != "-999")
            {
                if (cboVehicle.SelectedValue != "0")
                   return cboVehicle.SelectedValue;
                else return string.Empty;
            }
            else
            {
                foreach (RadComboBoxItem radItem in cboVehicle.Items)
                {
                    if (radItem.Value == "-999") continue;
                    if (radItem.Value == "0") continue;
                    if (selectedVehicles == string.Empty) selectedVehicles = radItem.Value;
                    else selectedVehicles = selectedVehicles + "," + radItem.Value;
                }
            }
            return selectedVehicles;
        }


    }
}