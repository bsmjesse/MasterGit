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
using VLF.DAS.Logic;
using VLF.CLS;
using VLF.CLS.Def;
using VLF.CLS.Interfaces;
using System.IO;
using System.Globalization; 

namespace SentinelFM
{
    public partial class Map_CommandsControls_BoxSetup : System.Web.UI.UserControl
    {

        ServerDBFleet.DBFleet dbFleet = new ServerDBFleet.DBFleet();
        private SentinelFMSession sn = null;
        protected clsUtility objUtil;
        protected DataSet dsTrace = new DataSet();


        public int cboFreguencySelected
        {
            get { return Convert.ToInt32(this.cboFreguency.SelectedValue); }
            set
            {
                if (this.cboFreguency.Items.FindByValue(value.ToString()) != null)
                    this.cboFreguency.SelectedValue = value.ToString();
            }
        }


        public int cboGeoSelected
        {
            get { return Convert.ToInt32(this.cboGeo.SelectedValue); }
            set
            {
                if (this.cboGeo.Items.FindByValue(value.ToString()) != null)
                    this.cboGeo.SelectedValue = value.ToString();
            }
        }


        public int cboTracePeriodSetupSelected
        {
            get { return Convert.ToInt32(this.cboTracePeriodSetup.SelectedValue); }
            set
            {
                if (this.cboTracePeriodSetup.Items.FindByValue(value.ToString()) != null)
                    this.cboTracePeriodSetup.SelectedValue = value.ToString();
            }
        }


        public int cboSpeedSelected
        {
            get { return Convert.ToInt32(this.cboSpeed.SelectedValue); }
            set
            {
                if (this.cboSpeed.Items.FindByValue(value.ToString()) != null)
                    this.cboSpeed.SelectedValue = value.ToString();
            }
        }


        public int cboCommModeSelected
        {
            get { return Convert.ToInt32(this.cboCommMode.SelectedValue); }
            set
            {
                if (this.cboCommMode.Items.FindByValue(value.ToString()) != null)
                    this.cboCommMode.SelectedValue = value.ToString();
            }
        }



        public int cboTraceIntervalSetupSelected
        {
            get { return Convert.ToInt32(this.cboTraceIntervalSetup.SelectedValue); }
            set
            {
                if (this.cboTraceIntervalSetup.Items.FindByValue(value.ToString()) != null)
                    this.cboTraceIntervalSetup.SelectedValue = value.ToString();
            }
        }



        public string setupHarshAccelerationValue
        {
            get { return txtsetupHarshAcceleration.Text ; }
            set{txtsetupHarshAcceleration.Text = value; }
        }

        public string setupHarshBrakingValue
        {
            get { return txtsetupHarshBraking.Text; }
            set { txtsetupHarshBraking.Text = value; }
        }

        public string setupExtremeAccelerationValue
        {
            get { return txtsetupExtremeAcceleration.Text; }
            set { txtsetupExtremeAcceleration.Text = value; }
        }


        public string setupExtremeBrakingValue
        {
            get { return txtsetupExtremeBraking.Text; }
            set { txtsetupExtremeBraking.Text = value; }
        }



        public DataGrid  dgBoxSetupValues
        {
            get { return this.dgBoxSetupSensors; }
            
        }


        

        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn == null || sn.UserName == "")
            {
                return;
            }
            objUtil = new clsUtility(sn);

            //if (!Page.IsPostBack)
            //{
                DgBoxSetupSensors_Fill(sn.Cmd.SelectedVehicleLicensePlate);
                CboSpeed_Fill();
                CboGeo_Fill();
                CboReportingFreq_Fill();
                cboCommMode_Fill();

                if (sn.Cmd.CommandId == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.ExtendedSetup))
                    tblViolations.Visible = true;
                else
                    tblViolations.Visible = false;
           //}
        }
        protected void cboTracePeriodSetup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTracePeriodSetup.SelectedItem.Value == "0")
            {
                this.cboTraceIntervalSetup.Enabled = false;
                this.cboTraceIntervalSetup.SelectedIndex = -1;
                cboTraceIntervalSetup.SelectedItem.Value = "0";
            }
            else
                this.cboTraceIntervalSetup.Enabled = true;
        }
        protected void cmdUnselect_Click(object sender, EventArgs e)
        {
            UnselectAllSensors();
            this.dgBoxSetupSensors.DataSource = sn.Map.SetupSensors;
            this.dgBoxSetupSensors.DataBind();
        }
        protected void cmdSelectAll_Click(object sender, EventArgs e)
        {
            SelectAllSensors();
            this.dgBoxSetupSensors.DataSource = sn.Map.SetupSensors;
            this.dgBoxSetupSensors.DataBind();
        }

        private void SelectAllSensors()
        {
            try
            {
                DgBoxSetupSensors_Fill(sn.Cmd.SelectedVehicleLicensePlate);

                foreach (DataRow rowItem in sn.Map.SetupSensors.Rows)
                {
                    rowItem["chkSet"] = true;

                }


            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void UnselectAllSensors()
        {
            try
            {
                DgBoxSetupSensors_Fill(sn.Cmd.SelectedVehicleLicensePlate);

                foreach (DataRow rowItem in sn.Map.SetupSensors.Rows)
                {
                    rowItem["chkSet"] = false;

                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                

            }
        }

        private void DgBoxSetupSensors_Fill(string LicensePlate)
        {
            try
            {
              
                string xml = "";

                DataSet dsSensors = new DataSet(); 
                StringReader strrXML = null;
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, LicensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {
                    return;
                }

                strrXML = new StringReader(xml);
                dsSensors.ReadXml(strrXML);


                // Show Combobox
                DataColumn dc = new DataColumn();
                dc.ColumnName = "chkSet";
                dc.DataType = Type.GetType("System.Boolean");
                dc.DefaultValue = true;
                dsSensors.Tables[0].Columns.Add(dc);

                DataColumn TraceStateId = new DataColumn("TraceStateId", Type.GetType("System.Int16"));
                TraceStateId.DefaultValue = VLF.CLS.Def.Enums.SensorsTraceState.Disable;
                dsSensors.Tables[0].Columns.Add(TraceStateId);
                DataColumn TraceStateName = new DataColumn("TraceStateName", Type.GetType("System.String"));
                TraceStateName.DefaultValue = VLF.CLS.Def.Enums.SensorsTraceState.Disable;
                dsSensors.Tables[0].Columns.Add(TraceStateName);

                if (sn.Map.SetupSensors != null)
                {
                    foreach (DataRow rowSN in sn.Map.SetupSensors.Rows)
                    {
                        if (rowSN["chkSet"].ToString().ToLower() == "true")
                        {

                            foreach (DataRow rowLast in dsSensors.Tables[0].Rows)
                            {
                                if (rowLast["SensorId"].ToString() == rowSN["SensorId"].ToString())
                                {
                                    rowLast["chkSet"] = "true";
                                    break;
                                }
                            }
                        }
                    }
                }



                DataView myView = dsSensors.Tables[0].DefaultView;
                myView.RowFilter = "SensorName not like '%" + VLF.CLS.Def.Const.keySensorNotInUse + "%'";

                this.dgBoxSetupSensors.DataSource = myView;
                this.dgBoxSetupSensors.DataBind();
                sn.Map.SetupSensors = dsSensors.Tables[0] ;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));

            }

        }


        public int GetTrace(short TraceStateId)
        {
            try
            {
                DropDownList cboTraceState = new DropDownList();
                cboTraceState.DataValueField = "TraceStateId";
                cboTraceState.DataTextField = "TraceStateName";
                DsTrace_Fill();
                cboTraceState.DataSource = dsTrace;
                cboTraceState.DataBind();

                cboTraceState.SelectedIndex = -1;
                cboTraceState.Items.FindByValue(TraceStateId.ToString()).Selected = true;
                return cboTraceState.SelectedIndex;
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                return 0;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                return 0;
            }

        }

        private void DsTrace_Fill()
        {
            try
            {
                DataTable tblTrace = dsTrace.Tables.Add("TraceState");
                tblTrace.Columns.Add("TraceStateId", typeof(short));
                tblTrace.Columns.Add("TraceStateName", typeof(string));

                Array enmArr = Enum.GetValues(typeof(Enums.SensorsTraceState));
                string TraceState;
                object[] objRow;
                foreach (Enums.SensorsTraceState ittr in enmArr)
                {
                    TraceState = Enum.GetName(typeof(Enums.SensorsTraceState), ittr);
                    objRow = new object[2];
                    objRow[0] = Convert.ToInt16(ittr);
                    objRow[1] = TraceState;
                    dsTrace.Tables[0].Rows.Add(objRow);

                }
            }


            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        private void CboSpeed_Fill()
        {
            try
            {
                ListItem ls;
                this.cboSpeed.Items.Clear();

                ls = new ListItem();
                ls.Value = "0";
                ls.Text = "Disabled";
                this.cboSpeed.Items.Add(ls);


                //DataSet ds = new DataSet();
                //StringReader strrXML = null;
                //string xml = "";
                //ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                //if (objUtil.ErrCheck(dbv.GetBoxConfigInfo(sn.UserID, sn.SecId, Convert.ToInt32(this.lblBoxId.Text), ref xml), false))
                //    if (objUtil.ErrCheck(dbv.GetBoxConfigInfo(sn.UserID, sn.SecId, Convert.ToInt32(this.lblBoxId.Text), ref xml), true))
                //    {
                //        return;
                //    }

                //if (xml == "")
                //{
                //    return;
                //}

                //strrXML = new StringReader(xml);
                //ds.ReadXml(strrXML);

                //Int16 BoxProtocolTypeId = Convert.ToInt16(ds.Tables[0].Rows[0]["BoxProtocolTypeId"].ToString());


                //if ((BoxProtocolTypeId == Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.HGIv40)) ||
                //    (BoxProtocolTypeId == Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.HGIv60)) ||
                //    (BoxProtocolTypeId == Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.HGIv70)) ||
                //    (BoxProtocolTypeId == Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.HGIv80)) ||
                //    (BoxProtocolTypeId == Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.XSv10)))
                //{

                    if (sn.User.UnitOfMes == 1)
                    {
                        foreach (string str in VLF.CLS.Def.Const.SpeedTriggerKMH)
                        {
                            ls = new ListItem();
                            ls.Value = str;
                            ls.Text = str + " kmh";
                            this.cboSpeed.Items.Add(ls);

                        }
                    }
                    else
                    {
                        foreach (string str in VLF.CLS.Def.Const.SpeedTriggerMPH)
                        {
                            ls = new ListItem();
                            ls.Value = Convert.ToString(Convert.ToInt32(Math.Round(Convert.ToInt32(str) * 1.6094)));
                            ls.Text = str + " mph";
                            this.cboSpeed.Items.Add(ls);
                        }
                    }
                //}
                //else
                //{

                //    foreach (string str in VLF.CLS.Def.Const.SpeedTrigger)
                //    {
                //        ls = new ListItem();
                //        ls.Value = str;
                //        ls.Text = str + " kmh/" + Convert.ToInt32(Convert.ToInt32(str) / 1.6094).ToString() + " mph";
                //        this.cboSpeed.Items.Add(ls);

                //    }
                //}

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));

            }
        }

        private void CboGeo_Fill()
        {
            try
            {
                ListItem ls;
                this.cboGeo.Items.Clear();
                int i = 1;
                int GeoVal = 0;
                int mtr = 0;

                //ls = new ListItem();
                //if (Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[0]["BoxProtocolTypeId"]) != Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.HGIv50))
                //{
                //    ls.Value = "0";
                //    ls.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                //    this.cboGeo.Items.Add(ls);
                //}

                for (i = 1; i <= 15; i++)
                {
                    ls = new ListItem();
                    GeoVal = i * 200;
                    mtr = Convert.ToInt32(GeoVal * 0.3048);
                    ls.Value = mtr.ToString();
                    ls.Text = GeoVal.ToString() + " ft/" + mtr.ToString() + " m";
                    this.cboGeo.Items.Add(ls);
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void CboReportingFreq_Fill()
        {
            //short ProtocolId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[0]["BoxProtocolTypeId"]);
            //if (ProtocolId == Convert.ToInt16(VLF.CLS.Def.Enums.ProtocolTypes.TCPv10))
            //{
                
            //    cboFreguency.Items.Clear();
            //    ListItem ls = new ListItem();
            //    ls.Value = "0";
            //    ls.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                
            //    cboFreguency.Items.Add(ls);
            //    ListItem ls1 = new ListItem();
            //    ls1.Value = "60";
            //    ls1.Text = "1 " + (string)base.GetLocalResourceObject("Text_Minutes");
                
            //    cboFreguency.Items.Add(ls1);
            //    ListItem ls2 = new ListItem();
            //    ls2.Value = "300";
            //    ls2.Text = "5 " + (string)base.GetLocalResourceObject("Text_Minutes");
            //    ls2.Selected = true;
                
            //    cboFreguency.Items.Add(ls2);
            //    ListItem ls3 = new ListItem();
            //    ls3.Value = "9000";
            //    ls3.Text = "15 " + (string)base.GetLocalResourceObject("Text_Minutes");
                
            //    cboFreguency.Items.Add(ls3);
            //    ListItem ls4 = new ListItem();
            //    ls4.Value = "12000";
            //    ls4.Text = "20 " + (string)base.GetLocalResourceObject("Text_Minutes");
                
            //    cboFreguency.Items.Add(ls4);
            //    ListItem ls5 = new ListItem();
            //    ls5.Value = "18000";
            //    ls5.Text = "30 " + (string)base.GetLocalResourceObject("Text_Minutes");
                
            //    cboFreguency.Items.Add(ls5);
            //    ListItem ls6 = new ListItem();
            //    ls6.Value = "3600";
            //    ls6.Text = "1 " + (string)base.GetLocalResourceObject("Text_Hour");
                
            //    cboFreguency.Items.Add(ls6);
            //    ListItem ls7 = new ListItem();
            //    ls7.Value = "7200";
            //    ls7.Text = "2 " + (string)base.GetLocalResourceObject("Text_Hours");
                
            //    cboFreguency.Items.Add(ls7);
            //    ListItem ls8 = new ListItem();
            //    ls8.Value = "10800";
            //    ls8.Text = "3 " + (string)base.GetLocalResourceObject("Text_Hours");
                
            //    cboFreguency.Items.Add(ls8);
            //    ListItem ls9 = new ListItem();
            //    ls9.Value = "14400";
            //    ls9.Text = "4 " + (string)base.GetLocalResourceObject("Text_Hours");
                
            //    cboFreguency.Items.Add(ls9);
            //}
            //else
            //{
                
                cboFreguency.Items.Clear();
                cboGPSFrequency.Items.Clear();
                ListItem ls = new ListItem();
                ls.Value = "0";
                ls.Text = (string)base.GetLocalResourceObject("Text_Disabled");
                
                cboFreguency.Items.Add(ls);
                cboGPSFrequency.Items.Add(ls);
                ListItem ls1 = new ListItem();
                ls1.Value = "30";
                ls1.Text = "30 " + (string)base.GetLocalResourceObject("Text_Seconds");
                
                cboFreguency.Items.Add(ls1);
                cboGPSFrequency.Items.Add(ls1);
                ListItem ls2 = new ListItem();
                ls2.Value = "120";
                ls2.Text = "2 " + (string)base.GetLocalResourceObject("Text_Minutes");
                
                cboFreguency.Items.Add(ls2);
                cboGPSFrequency.Items.Add(ls2);
                ListItem ls3 = new ListItem();
                ls3.Value = "300";
                ls3.Text = "5 " + (string)base.GetLocalResourceObject("Text_Minutes");
                ls3.Selected = true;
                
                cboFreguency.Items.Add(ls3);
                cboGPSFrequency.Items.Add(ls3);
                ListItem ls4 = new ListItem();
                ls4.Value = "600";
                ls4.Text = "10 " + (string)base.GetLocalResourceObject("Text_Minutes");
                
                cboFreguency.Items.Add(ls4);
                cboGPSFrequency.Items.Add(ls4);
                ListItem ls5 = new ListItem();
                ls5.Value = "900";
                ls5.Text = "15 " + (string)base.GetLocalResourceObject("Text_Minutes");
                
                cboFreguency.Items.Add(ls5);
                cboGPSFrequency.Items.Add(ls5);
                ListItem ls6 = new ListItem();
                ls6.Value = "1800";
                ls6.Text = "30 " + (string)base.GetLocalResourceObject("Text_Minutes");
                
                cboFreguency.Items.Add(ls6);
                cboGPSFrequency.Items.Add(ls6);
                ListItem ls7 = new ListItem();
                ls7.Value = "3600";
                ls7.Text = "1 " + (string)base.GetLocalResourceObject("Text_Hour");
                
                cboFreguency.Items.Add(ls7);
                cboGPSFrequency.Items.Add(ls7);
                ListItem ls8 = new ListItem();
                ls8.Value = "7200";
                ls8.Text = "2 " + (string)base.GetLocalResourceObject("Text_Hours");
                
                cboFreguency.Items.Add(ls8);
                cboGPSFrequency.Items.Add(ls8);
                ListItem ls9 = new ListItem();
                ls9.Value = "14400";
                ls9.Text = "4 " + (string)base.GetLocalResourceObject("Text_Hours");
                
                cboFreguency.Items.Add(ls9);
                cboGPSFrequency.Items.Add(ls9);
                ListItem ls10 = new ListItem();
                ls10.Value = "21600";
                ls10.Text = "6 " + (string)base.GetLocalResourceObject("Text_Hours");
                
                cboFreguency.Items.Add(ls10);
                cboGPSFrequency.Items.Add(ls10);
                ListItem ls11 = new ListItem();
                ls11.Value = "28800";
                ls11.Text = "8 " + (string)base.GetLocalResourceObject("Text_Hours");
                
                cboFreguency.Items.Add(ls11);
                cboGPSFrequency.Items.Add(ls11);
                ListItem ls12 = new ListItem();
                ls12.Value = "36000";
                ls12.Text = "10 " + (string)base.GetLocalResourceObject("Text_Hours");
                
                cboFreguency.Items.Add(ls12);
                cboGPSFrequency.Items.Add(ls12);
                ListItem ls13 = new ListItem();
                ls13.Value = "43200";
                ls13.Text = "12 " + (string)base.GetLocalResourceObject("Text_Hours");
                
                cboGPSFrequency.Items.Add(ls13);
                cboFreguency.Items.Add(ls13);
                ListItem ls14 = new ListItem();
                ls14.Value = "86400";
                ls14.Text = "24 " + (string)base.GetLocalResourceObject("Text_Hours");
                
                cboFreguency.Items.Add(ls14);
                cboGPSFrequency.Items.Add(ls14);
            //}

        }


        protected void dgBoxSetupSensors_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            dgBoxSetupSensors.EditItemIndex = -1;
            this.dgBoxSetupSensors.DataSource = sn.Map.SetupSensors;
            this.dgBoxSetupSensors.DataBind();
        }

        protected void dgBoxSetupSensors_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            
            dgBoxSetupSensors.EditItemIndex = Convert.ToInt32(e.Item.ItemIndex);
            this.dgBoxSetupSensors.DataSource = sn.Map.SetupSensors;
            this.dgBoxSetupSensors.DataBind();
            dgBoxSetupSensors.SelectedIndex = -1;
        }

        protected void dgBoxSetupSensors_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {
                DropDownList cboTrace;
                string SensorId = dgBoxSetupSensors.DataKeys[e.Item.ItemIndex].ToString();
                cboTrace = (DropDownList)e.Item.FindControl("cboTraceSetup");
                foreach (DataRow rowItem in sn.Map.SetupSensors.Rows)
                {
                    if (rowItem["SensorId"].ToString().TrimEnd() == SensorId)
                    {
                        rowItem["TraceStateName"] = cboTrace.SelectedItem.Text;
                        rowItem["TraceStateId"] = cboTrace.SelectedItem.Value;
                    }
                }

                dgBoxSetupSensors.EditItemIndex = -1;
                this.dgBoxSetupSensors.DataSource = sn.Map.SetupSensors;
                this.dgBoxSetupSensors.DataBind();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }


        private void cboCommMode_Fill()
        {
            try
            {

                DataSet ds = new DataSet();

                StringReader strrXML = null;
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetBoxConfigInfo(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetBoxConfigInfo(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No configuration info for Box:" + sn.Cmd.BoxId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        return;
                    }

                if (xml == "")
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No configuration info for Box:" + sn.Cmd.BoxId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);

               
                this.cboCommMode.DataSource = ds;
                this.cboCommMode.DataBind();


               

            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

    }
}
