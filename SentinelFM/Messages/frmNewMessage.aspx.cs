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
using VLF.CLS.Interfaces;

namespace SentinelFM.Messages
{
    /// <summary>
    /// Summary description for frmNewMessage.
    /// </summary>
    public partial class frmNewMessage : SentinelFMBasePage
    {
        
        

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

        public bool ShowOrganizationHierarchy;
        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string LoadVehiclesBasedOn = "fleet";
        public string OrganizationHierarchyPath = "";
        VLF.PATCH.Logic.PatchOrganizationHierarchy poh;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            string defaultnodecode = string.Empty;

            ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

            string xml = "";
            if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                {

                }

            StringReader strrXML = new StringReader(xml);
            DataSet dsPref = new DataSet();
            dsPref.ReadXml(strrXML);

            foreach (DataRow rowItem in dsPref.Tables[0].Rows)
            {
                if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode))
                {
                    defaultnodecode = rowItem["PreferenceValue"].ToString().TrimEnd();

                }

                if (Convert.ToInt16(rowItem["PreferenceId"]) == 36)
                {
                    string d = rowItem["PreferenceValue"].ToString().ToLower();
                    if (d == "hierarchy")
                        LoadVehiclesBasedOn = "hierarchy";
                    else
                        LoadVehiclesBasedOn = "fleet";
                }
            }

            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

            poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
            if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                ShowOrganizationHierarchy = true;
            else
            {
                ShowOrganizationHierarchy = false;
                LoadVehiclesBasedOn = "fleet";
            }

            //LoadVehiclesBasedOn = "hierarchy";

            if (LoadVehiclesBasedOn == "hierarchy")
            {
                defaultnodecode = defaultnodecode ?? string.Empty;
                if (defaultnodecode == string.Empty)
                {
                    if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 480)
                        defaultnodecode = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID);
                    else
                        defaultnodecode = poh.GetRootNodeCode(sn.User.OrganizationId);

                }

                if (!IsPostBack)
                {
                    DefaultOrganizationHierarchyNodeCode = defaultnodecode;
                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                    OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);
                    hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;
                }
                else
                {
                    DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();
                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                }

                btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == string.Empty ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                lblFleetTitle.Visible = false;
                cboFleet.Visible = false;
                valFleet.Enabled = false;
                lblOhTitle.Visible = true;
                btnOrganizationHierarchyNodeCode.Visible = true;
                hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
            }

            if (!Page.IsPostBack)
            {
                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmNewMessageForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                GuiSecurity(this);
                this.tblWait.Visible = false;
                this.tblMain.Visible = true;
                this.lblMessage.Text = "";

                CboFleet_Fill();
                sn.Cmd.DualComm = false;


                if (sn.Message.MessageSent)
                {
                    if (sn.Message.DtSendMessageFails.Rows.Count > 0)
                    {

                        this.txtMessage.Text = sn.Message.Message;


                        this.cboFleet.SelectedIndex = -1;
                        for (int i = 0; i <= cboFleet.Items.Count - 1; i++)
                        {
                            if (cboFleet.Items[i].Value == sn.Message.FleetId.ToString())
                            {
                                this.cboFleet.SelectedIndex = i;
                                break;
                            }
                        }


                        CboVehicle_Fill(Convert.ToInt32(sn.Message.FleetId));

                        this.cboVehicle.SelectedIndex = -1;
                        for (int i = 0; i <= cboVehicle.Items.Count - 1; i++)
                        {
                            if (cboVehicle.Items[i].Value == sn.Message.VehicleId.ToString())
                            {
                                this.cboVehicle.SelectedIndex = i;
                                break;
                            }
                        }


                        this.cboVehicle.Visible = true;

                        ListItem ls;
                        foreach (string lstItem in sn.Message.Response.Split('~'))
                        {
                            ls = new ListItem();
                            ls.Text = lstItem;
                            this.lslResponse.Items.Add(ls);

                        }

                        if (cboVehicle.SelectedIndex != -1)
                        {
                            this.optCommMode.Enabled = true;
                            CheckCommMode();
                        }

                        sn.MessageText = "";

                        ShowSendMessagesTimeOuts();


                        if ((sn.MessageText != "") && (sn.Message.ToString().Length > 0))
                        {

                            string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                            strUrl = strUrl + "	var myname='Message';";
                            strUrl = strUrl + " var w=370;";
                            strUrl = strUrl + " var h=50;";
                            strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                            strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                            strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
                            strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);} ";

                            strUrl = strUrl + " NewWindow('../frmMessage.aspx');</script>";
                            Response.Write(strUrl);

                        }
                    }
                    else
                    {
                        this.lblCommandStatus.ForeColor = Color.Green;

                        if (sn.Message.MessageQueued)
                        {
                            this.lblCommandStatus.Text = (string)base.GetLocalResourceObject("lblCommandStatus_Text_MessageQueued");
                            return;
                        }

                        if ((sn.Cmd.CommModeId == 0) || (sn.Cmd.CommModeId == -1))
                            this.lblCommandStatus.Text = (string)base.GetLocalResourceObject("lblCommandStatus_Text_MessageSuccess");
                        else
                        {
                            this.lblCommandStatus.Text = (string)base.GetLocalResourceObject("lblCommandStatus_Text_MessageSuccessOtherMode") + " " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId;
                            sn.Cmd.CommModeId = 0;
                        }


                    }
                    sn.Message.MessageSent = false;
                }

                if (LoadVehiclesBasedOn == "hierarchy")
                {
                    int fleetId = DefaultOrganizationHierarchyFleetId;
                    if (fleetId != -1)
                    {
                        this.cboVehicle.Visible = true;
                        this.lblVehicleName.Visible = true;
                        CboVehicle_Fill(fleetId);
                    }
                }
                else
                {
                    if ((sn.User.DefaultFleet != -1) && (cboFleet.SelectedIndex == -1))
                    {
                        cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));

                        this.cboVehicle.Visible = true;
                        this.lblVehicleName.Visible = true;
                        CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));

                    }
                }
            }
        }

        protected void cmdAddResponse_Click(object sender, System.EventArgs e)
        {
            this.lblMessage.Text = "";

            if (this.txtResponse.Text == "")
            {
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_EmptyResponseError");
                this.lblMessage.Visible = true;
                return;
            }

            ListItem ls = new ListItem();
            ls.Text = this.txtResponse.Text;

            if (this.lslResponse.Items.Count > 5)
            {
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_TooManyResponseError");
                this.lblMessage.Visible = true;
                return;
            }

            this.lslResponse.Items.Add(ls);
            this.txtResponse.Text = "";
        }

        protected void cmdRemove_Click(object sender, System.EventArgs e)
        {
            this.lslResponse.Items.Remove(this.lslResponse.SelectedItem);
        }


        private void CboFleet_Fill()
        {
            try
            {

                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();
                cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));




            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }

        protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            /*if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            {
                this.cboVehicle.Visible = true;
                this.lblVehicleName.Visible = true;
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
            }*/
            refillCboVehicle();
        }

        protected void hidOrganizationHierarchyPostBack_Click(object sender, EventArgs e)
        {
            refillCboVehicle();
        }

        private void refillCboVehicle()
        {
            int fleetId = -1;
            if (LoadVehiclesBasedOn == "hierarchy")
            {
                int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
            }
            else
            {
                fleetId = Convert.ToInt32(cboFleet.SelectedItem.Value);
            }

            if (fleetId != -1)
            {
                this.cboVehicle.Visible = true;
                this.lblVehicleName.Visible = true;
                CboVehicle_Fill(fleetId);
            }
            else
                this.cboVehicle.Items.Clear();
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

                if (objUtil.ErrCheck(dbf.GetVehiclesPeripheralInfoByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesPeripheralInfoByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                    {
                        cboVehicle.Items.Clear();
                        cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                        //cboVehicle.Items.Insert(cboVehicle.Items.Count, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                        return;
                    }
                if (xml == "")
                {
                    cboVehicle.Items.Clear();
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                    //cboVehicle.Items.Insert(cboVehicle.Items.Count, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();

                sn.Message.DsVehicles = dsVehicle;

                cboVehicle.Items.Insert(cboVehicle.Items.Count, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_01"), "-1"));
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }


        }

        protected void cmdSend_Click(object sender, System.EventArgs e)
        {

            string strMessage = "";
            string strResponse = "";
            string txtMessage = "";

            sn.Message.DtSendMessageBoxes.Rows.Clear();
            sn.Message.DtSendMessageFails.Rows.Clear();

            //txtMessage = System.Convert.ToChar(13) + this.txtMessage.Text;
            //txtMessage = "\r\n" + this.txtMessage.Text;

            txtMessage = " " + this.txtMessage.Text+"\r\n";


            /*
            byte[] mb = System.Text.Encoding.ASCII.GetBytes(strMessage);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for(int a = 0; a < mb.Length; sb.AppendFormat("{0:x2} ", mb[a++]));
            System.Diagnostics.Trace.WriteLine(string.Format("Message:{0}", sb.ToString()));
            */


            //strMessage = VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMessage, this.txtMessage.Text);
            strMessage = VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMessage, txtMessage);

            int EnterPos = 0;
            int TotalEnters = 0;


            if ((this.cboSchInterval.SelectedItem.Value != "0" && this.cboSchPeriod.SelectedItem.Value == "0")
                || (this.cboSchInterval.SelectedItem.Value == "0" && this.cboSchPeriod.SelectedItem.Value != "0"))
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidScheduledPeriodIntervalError");
                return;
            }

            while (strMessage.IndexOf("\n", EnterPos + 1) != -1)
            {
                EnterPos = strMessage.IndexOf("\n", EnterPos + 1);
                if (EnterPos != -1)
                    TotalEnters++;
            }

            if (TotalEnters > 5)
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_TooManyLinesError");
                return;
            }

            if (this.cboVehicle.SelectedItem.Value == "-1")
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("cboVehicle_Item_01");
                return;
            }

            this.lblMessage.Text = "";
            if (this.lslResponse.Items.Count > 0)
            {
                foreach (ListItem li in this.lslResponse.Items)
                {
                    strResponse += li.Text + "~";
                }
                strResponse = strResponse.Substring(0, strResponse.Length - 1);
                sn.Message.Response = strResponse;
            }
            else
            {
                sn.Message.Response = "";
            }

            int fleetId = -1;
            if (LoadVehiclesBasedOn == "hierarchy")
            {
                int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
            }
            else
            {
                fleetId = Convert.ToInt32(this.cboFleet.SelectedItem.Value);
            }

            sn.Message.Message = txtMessage;
            sn.Message.FleetId = fleetId;
            sn.Message.VehicleId = Convert.ToInt64(this.cboVehicle.SelectedItem.Value);


            this.lblMessage.Text = "";

            strMessage += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyAnswer, strResponse);

            LocationMgr.Location dbl = new LocationMgr.Location();


            bool cmdSent = false;


            if (Convert.ToInt32(this.cboVehicle.SelectedItem.Value) != 0)
            {
                //--- Create table of Boxes for Message Send
                DataRow dr;
                dr = sn.Message.DtSendMessageBoxes.NewRow();
                dr["BoxId"] = Convert.ToInt32(this.cboVehicle.SelectedItem.Value);
                dr["VehicleDesc"] = this.cboVehicle.SelectedItem.Text;
                dr["Updated"] = -1;
                sn.Message.DtSendMessageBoxes.Rows.Add(dr);
                if (this.optCommMode.SelectedItem.Value == "2")
                    sn.Cmd.DualComm = true;
                else
                    sn.Cmd.DualComm = false;
                sn.Cmd.BoxId = Convert.ToInt32(this.cboVehicle.SelectedItem.Value);
                sn.Cmd.CommandId = Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage);
                //------------------------------------------

            }
            else
            {

                sn.Cmd.DualComm = false;
                foreach (DataRow rowVehicle in sn.Message.DsVehicles.Tables[0].Rows)
                {
                    //--- Create table of Boxes for Message Send
                    DataRow dr;
                    dr = sn.Message.DtSendMessageBoxes.NewRow();
                    dr["BoxId"] = Convert.ToInt32(rowVehicle["BoxId"]);
                    dr["VehicleDesc"] = rowVehicle["Description"].ToString().TrimEnd();
                    dr["Updated"] = -1;
                    sn.Message.DtSendMessageBoxes.Rows.Add(dr);
                    //------------------------------------------

                }

            }

            sn.Cmd.CommandParams = strMessage;
            sn.Cmd.CommModeId = -1;
            sn.Cmd.ProtocolTypeId = -1;

            if ((cboSchInterval.SelectedItem.Value != "0") && (cboSchPeriod.SelectedItem.Value != "0"))
            {
                sn.Cmd.SchCommand = true;
                sn.Cmd.SchInterval = Convert.ToInt32(this.cboSchInterval.SelectedItem.Value);
                sn.Cmd.SchPeriod = Convert.ToInt64(this.cboSchPeriod.SelectedItem.Value);
            }
            else
            {
                sn.Cmd.SchCommand = false;
                sn.Cmd.SchInterval = 0;
                sn.Cmd.SchPeriod = 0;
            }


            

            foreach (DataRow rowVehicle in sn.Message.DtSendMessageBoxes.Rows)
            {
                short ProtocolId = -1;
                short CommModeId = -1;
                Int64 sessionTimeOut = 0;
                if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowVehicle["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), strMessage, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false))
                    if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowVehicle["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), strMessage, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true))
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SendFailedError");
                        return;
                    }
                sn.Cmd.CommModeId = CommModeId;
                rowVehicle["ComModeId"] = CommModeId;
                rowVehicle["ProtocolId"] = ProtocolId;



            }



            this.tblMain.Visible = false;
            this.tblWait.Visible = true;
            this.lblMessage.Text = "";

            sn.Message.MessageSent = true;
            sn.Message.TimerStatus = true;
            Response.Write("<script language='javascript'> parent.frmNewMessageTimer.location.href='frmNewMessageTimer.aspx' </script>");

        }


        protected void cmdCancelSend_Click(object sender, System.EventArgs e)
        {
            try
            {
                LocationMgr.Location dbl = new LocationMgr.Location();

                foreach (DataRow rowItem in sn.Message.DtSendMessageBoxes.Rows)
                {

                    if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(rowItem["ProtocolId"])), false))
                        if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(rowItem["ProtocolId"])), true))
                        {
                            CancelCommand();
                            return;
                        }
                }


                CancelCommand();
            }

            catch (Exception Ex)
            {
                CancelCommand();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }


        private void CancelCommand()
        {
            this.tblWait.Visible = false;
            this.tblMain.Visible = true;
            sn.Message.TimerStatus = false;
        }
        private void ShowSendMessagesTimeOuts()
        {
            try
            {

                if (sn.MessageText != "")
                {
                    sn.MessageText += "\n__________________________________  \n";
                }

                sn.MessageText += (string)base.GetLocalResourceObject("SentinelFMSession_MessageText_CommunicationWithVehicles") + ": ";
                int i = 0;
                foreach (DataRow rowItem in sn.Message.DtSendMessageFails.Rows)
                {
                    i++;
                    if (i == sn.Message.DtSendMessageFails.Rows.Count)
                        sn.MessageText += rowItem["VehicleDesc"].ToString().TrimEnd();
                    else
                        sn.MessageText += rowItem["VehicleDesc"].ToString().TrimEnd() + ",";

                }
                sn.MessageText += " " + (string)base.GetLocalResourceObject("SentinelFMSession_MessageText_NoCommunication");
                sn.Message.DtSendMessageFails.Rows.Clear();

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        private void CheckCommMode()
        {
            try
            {
                
                DataSet ds = new DataSet();

                StringReader strrXML = null;
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetBoxConfigInfo(sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicle.SelectedItem.Value), ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetBoxConfigInfo(sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicle.SelectedItem.Value), ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {
                    return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);

                sn.Cmd.DsProtocolTypes = ds;
                if (ds.Tables[0].Rows.Count > 1)
                    this.optCommMode.Items[1].Enabled = true;

                else
                    this.optCommMode.Items[1].Enabled = false;

            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }

        protected void cboVehicle_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            if (Convert.ToInt32(this.cboVehicle.SelectedItem.Value) != 0)
            {
                DataRow[] drArr = sn.Message.DsVehicles.Tables[0].Select("BoxId='" + cboVehicle.SelectedItem.Value + "'");
                if (drArr != null && drArr.Length > 0)
                {
                    if (Convert.ToInt16(drArr[0]["TypeId"]) == (Int16)VLF.CLS.Def.Enums.PeripheralTypes.Garmin)
                    {
                        this.MultiviewMessages.ActiveViewIndex = 1;
                    }
                    else
                    {
                        this.MultiviewMessages.ActiveViewIndex = 0;

                        this.optCommMode.SelectedIndex = 0;
                        this.cboSchInterval.SelectedIndex = 0;
                        this.cboSchPeriod.SelectedIndex = 0;

                        if (this.cboVehicle.SelectedItem.Value != "0")
                            CheckCommMode();
                        else
                            this.optCommMode.Items[1].Enabled = false;
                    }
                }
            }
            else
            {
                DataRow[] drArr = sn.Message.DsVehicles.Tables[0].Select("TypeId='" + (Int16)VLF.CLS.Def.Enums.PeripheralTypes.Garmin + "'");
                if (drArr != null && drArr.Length > 0)
                {
                    this.MultiviewMessages.ActiveViewIndex = 1;
                }
                else
                {
                    this.optCommMode.SelectedIndex = 0;
                    this.cboSchInterval.SelectedIndex = 0;
                    this.cboSchPeriod.SelectedIndex = 0;
                    this.optCommMode.Items[1].Enabled = false;
                }
            }
            
        }





        protected void optCommMode_SelectedIndexChanged(object sender, EventArgs e)
        {

            cboSchInterval.SelectedIndex = 0;
            cboSchPeriod.SelectedIndex = 0;

            if (this.optCommMode.SelectedItem.Value == "1")
            {
                sn.Cmd.DualComm = false;
                this.cboSchInterval.Enabled = true;
                this.cboSchPeriod.Enabled = true;  
            }
            else
            {
                sn.Cmd.DualComm = true;
                this.cboSchInterval.Enabled = false;
                this.cboSchPeriod.Enabled = false;  
            }
        }


        protected void cboSchPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSchPeriod.SelectedItem.Value == "0")
                this.cboSchInterval.SelectedIndex = 0; 
            
        }
        protected void cboSchInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSchInterval.SelectedItem.Value == "0")
                this.cboSchPeriod.SelectedIndex = 0; 
        }
        protected void cmdSendGarminMsg_Click(object sender, EventArgs e)
        {

            if (this.txtMessageGarmin.Text == "")
            {
                this.lblGarminMessage.Text = "Please enter a Message";
                return; 
            }
            else
                this.lblGarminMessage.Text = "";

            string paramList = VLF.CLS.Util.MakePair(GarminFMI.JKeyWords.TXT.ToString() , this.txtMessageGarmin.Text);
            paramList += VLF.CLS.Util.MakePair(GarminFMI.JKeyWords.YESNO.ToString(), "false");
            paramList += VLF.CLS.Util.MakePair(GarminFMI.JKeyWords.RETRYINTERVAL.ToString(), Convert.ToString(Convert.ToInt32(this.cboSchIntervalGarmin.SelectedItem.Value) / 60));
            paramList += VLF.CLS.Util.MakePair(GarminFMI.JKeyWords.LIFETIME.ToString(), Convert.ToString(Convert.ToInt32(this.cboSchPeriodGarmin.SelectedItem.Value) / 60));

               
            using (DBGarmin.Garmin garmin = new DBGarmin.Garmin())
            {


                if (Convert.ToInt32(this.cboVehicle.SelectedItem.Value) != 0)
                {
                    if (objUtil.ErrCheck(garmin.SendTextMessage(sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicle.SelectedItem.Value), Convert.ToInt32(this.cboMessageType.SelectedItem.Value), paramList), false))
                        if (objUtil.ErrCheck(garmin.SendTextMessage(sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicle.SelectedItem.Value), Convert.ToInt32(this.cboMessageType.SelectedItem.Value), paramList), true))
                        {
                            this.lblMessage.Text = "Send message failed";
                            return;
                        }

                }
                else
                {
                    SendMessageToFleetWithGarminDevice(paramList);
                }
                

            }

            Response.Write("<script language='javascript'>parent.window.close()</script>");
        }


        private void SendMessageToFleetWithGarminDevice(string GarminParamList)
        {


            string txtMessage = " " + this.txtMessageGarmin.Text + "\r\n";
            string strMessage = VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyMessage, txtMessage);

            int EnterPos = 0;
            int TotalEnters = 0;


            while (strMessage.IndexOf("\n", EnterPos + 1) != -1)
            {
                EnterPos = strMessage.IndexOf("\n", EnterPos + 1);
                if (EnterPos != -1)
                    TotalEnters++;
            }

            if (TotalEnters > 5)
            {
                this.lblGarminMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_TooManyLinesError");
                return;
            }
            else
            {
                lblGarminMessage.Text = "";
            }

            if (txtMessageGarmin.Text.Length > 230)
            {
                this.lblGarminMessage.Text = "Message too big. Maximum message lenth is 230 characters.";
                return;
            }




            LocationMgr.Location dbl = new LocationMgr.Location();
            DBGarmin.Garmin garmin = new DBGarmin.Garmin();

            Int64  schPeriod = Convert.ToInt64(this.cboSchPeriodGarmin.SelectedItem.Value)    ;
            int schInterval = Convert.ToInt32(this.cboSchIntervalGarmin.SelectedItem.Value);

                foreach (DataRow dr in sn.Message.DsVehicles.Tables[0].Rows)
                {
                    if (Convert.ToInt16(dr["TypeId"]) == (Int16)VLF.CLS.Def.Enums.PeripheralTypes.Garmin)
                    {
                        if (objUtil.ErrCheck(garmin.SendTextMessage(sn.UserID, sn.SecId, Convert.ToInt32(dr["boxId"]), Convert.ToInt32(this.cboMessageType.SelectedItem.Value), GarminParamList), false))
                            if (objUtil.ErrCheck(garmin.SendTextMessage(sn.UserID, sn.SecId, Convert.ToInt32(dr["boxId"]), Convert.ToInt32(this.cboMessageType.SelectedItem.Value), GarminParamList), true))
                            {
                                continue;
                            }
                    }
                    else if (Convert.ToInt16(dr["TypeId"]) == (Int16)VLF.CLS.Def.Enums.PeripheralTypes.MDT)
                    {

                        Int16 commModeId = Convert.ToInt16(dr["CommModeId"]);
                        Int16 protocolTypeId = Convert.ToInt16(dr["BoxProtocolTypeId"]);
                        Int64 TaskId = -1;


                        if (objUtil.ErrCheck(dbl.SendAutomaticCommand(sn.UserID, sn.SecId, Convert.ToInt32(dr["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), DateTime.Now, strMessage, ref protocolTypeId, ref commModeId, schPeriod, schInterval, false, ref TaskId), false))
                            if (objUtil.ErrCheck(dbl.SendAutomaticCommand(sn.UserID, sn.SecId, Convert.ToInt32(dr["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), DateTime.Now, strMessage, ref protocolTypeId, ref commModeId, schPeriod, schInterval, false, ref TaskId), true))
                            {
                                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Sent Automatic command failed " + Convert.ToString((VLF.CLS.Def.Enums.CommMode)commModeId) + ". User:" + sn.UserID.ToString() + " Form:frmNewMessageTimer.aspx"));
                            }
                    }

                }

          
        }


      
}
}