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
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.Globalization;

namespace SentinelFM.Map
{
    /// <summary>
    /// Summary description for frmBigDetails.
    /// </summary>
    public partial class frmBigDetails : SentinelFMBasePage
    {

        
        
        private DataSet dsFleetInfo;
        public ClientMapProxy map;
        public int AutoRefreshTimer = 40000;


        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
               
               // Response.Write("<script language='javascript'>window.setTimeout('AutoReloadDetails()'," + AutoRefreshTimer.ToString() + ")</script>");

                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmBigDetailsForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);


                    this.tblWait.Visible = false;

                    CboFleet_Fill();

                    FindExistingPreference();

                    if (sn.Map.SelectedFleetID != 0)   
                    {
                       cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.Map.SelectedFleetID.ToString()));
                       if (sn.Map.DsFleetInfo == null)
                          DgFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));
                       else
                       {
                          this.dgFleetInfo.DataSource = sn.Map.DsFleetInfo.Tables[0];
                          this.dgFleetInfo.DataBind();
                       }
                    }
                    else
                    {
                        this.dgFleetInfo.DataSource = null;
                        this.dgFleetInfo.DataBind();
                    }




                    //--- Check Results after update position

                    sn.MessageText = "";

                    if (sn.Cmd.DtUpdatePositionFails.Rows.Count > 0)
                        ShowUpdatePositionsTimeOuts();


                    if (sn.Cmd.UpdatePositionSend)
                    {
                        ShowUpdatePositionsNotValid();
                        sn.Cmd.UpdatePositionSend = false;
                    }


                    if (sn.Cmd.DtUpdatePositionFails.Rows.Count > 0)
                        sn.Cmd.DtUpdatePositionFails.Rows.Clear();

                    //----------------------------------



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
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }


        private void ShowUpdatePositionsTimeOuts()
        {
            try
            {

                sn.MessageText = "";

                // TimeOut Messages
                string strSQL = "Status='" + Convert.ToString((int)CommandStatus.CommTimeout) + "'";
                DataRow[] foundRows = sn.Cmd.DtUpdatePositionFails.Select(strSQL);

                if (foundRows.Length > 0)
                {
                    sn.MessageText += (string)base.GetLocalResourceObject("sn_MessageText_CommunicationWithVehicle1") + ": ";
                    foreach (DataRow rowItem in foundRows)
                    {
                        sn.MessageText += rowItem["VehicleDesc"].ToString().TrimEnd() + ",";
                    }


                    if (sn.MessageText.Length > 0)
                        sn.MessageText = sn.MessageText.Substring(0, sn.MessageText.Length - 1);

                    sn.MessageText += " " + (string)base.GetLocalResourceObject("sn_MessageText_CommunicationWithVehicle2");

                }

                //clear filter
                sn.Cmd.DtUpdatePositionFails.Select();

                //Queued Messages 
                strSQL = "Status='" + Convert.ToString((int)CommandStatus.Pending) + "'";
                foundRows = sn.Cmd.DtUpdatePositionFails.Select(strSQL);


                if (foundRows.Length > 0)
                {
                    sn.MessageText += " " + (string)base.GetLocalResourceObject("sn_MessageText_UpdatePositionForVehicle1") + ": ";
                    foreach (DataRow rowItem in foundRows)
                    {
                        sn.MessageText += rowItem["VehicleDesc"].ToString().TrimEnd() + ",";
                    }

                    if (sn.MessageText.Length > 0)
                        sn.MessageText = sn.MessageText.Substring(0, sn.MessageText.Length - 1);

                    sn.MessageText += " " + (string)base.GetLocalResourceObject("sn_MessageText_UpdatePositionForVehicle2");
                }



            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }


        private void ShowUpdatePositionsNotValid()
        {
            try
            {


                if (sn.MessageText != "")
                {
                    sn.MessageText += "\n_________________________________________\n";
                }



                bool InvalidExists = false;

                foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                    {
                        if (Convert.ToDateTime(rowItem["LastCommunicatedDateTime"]) > Convert.ToDateTime(rowItem["OriginDateTime"]))
                        {
                            InvalidExists = true;
                            break;
                        }
                    }
                }



                //Delay for resolving "Store Position"
                System.Threading.Thread.Sleep(2000);


                if (InvalidExists)
                {
                    DgFleetInfo_Fill(Convert.ToInt32(this.cboFleet.SelectedItem.Value));

                    bool ShowTitle = false;

                    foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                        if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                        {
                            if (Convert.ToDateTime(rowItem["LastCommunicatedDateTime"]) > Convert.ToDateTime(rowItem["OriginDateTime"]))
                            {

                                bool UpdatePositionFails = false;

                                foreach (DataRow rw in sn.Cmd.DtUpdatePositionFails.Rows)
                                {
                                    if (rw["VehicleDesc"].ToString().TrimEnd() == rowItem["VehicleDesc"].ToString().TrimEnd())
                                    {
                                        UpdatePositionFails = true;
                                        break;
                                    }
                                }

                                // If not exist Error message for this vehicle
                                if (!UpdatePositionFails)
                                {
                                    if (!ShowTitle)
                                    {
                                        sn.MessageText += (string)base.GetLocalResourceObject("sn_MessageText_GPSPositionForVehicle1") + ": ";
                                        ShowTitle = true;
                                    }

                                    sn.MessageText += rowItem["Description"].ToString().TrimEnd() + ",";
                                }
                            }
                        }
                    }
                }


                if ((sn.MessageText.Length > 0) && (sn.MessageText.Substring(sn.MessageText.Length - 1, 1) == ","))
                {
                    sn.MessageText = sn.MessageText.Substring(0, sn.MessageText.Length - 1);
                    sn.MessageText += "\n " + (string)base.GetLocalResourceObject("sn_MessageText_GPSPositionForVehicle2");
                }

                sn.Cmd.DtUpdatePositionFails.Rows.Clear();

            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }


        private void FindExistingPreference()
        {
            try
            {

               ViewState["UnitOfMes"]=sn.User.UnitOfMes == 1 ? "(km" : "(mi";

               // Changes for TimeZone Feature start
                if (sn.User.NewFloatTimeZone < 0)
                    ViewState["TimeZone"] = "(GMT-" + sn.User.NewFloatTimeZone.ToString() + ")";
                else
                    ViewState["TimeZone"] = "(GMT+" + sn.User.NewFloatTimeZone.ToString() + ")"; // Changes for TimeZone Feature end
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }

        private void DgFleetInfo_Fill(int fleetId)
        {
            try
            {

                dsFleetInfo = new DataSet();

                
                StringReader strrXML = null;


                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetVehiclesLastKnownPositionInfo.User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                        return;
                    }
                if (xml == "")
                {
                    this.dgFleetInfo.DataSource = null;
                    this.dgFleetInfo.DataBind();
                    sn.Map.DsFleetInfo = null;
                    return;
                }

                strrXML = new StringReader(xml);
                dsFleetInfo.ReadXml(strrXML);


                sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);

                //dgFleetInfo.Columns[4].HeaderText="Date " +ViewState["TimeZone"]; 
                ///dgFleetInfo.Columns[5].HeaderText="Speed "+ViewState["UnitOfMes"]+"/h)";  

                //dsFleetInfo.Tables[0].DefaultView.Sort = "chkBoxShow" + " DESC";
                dgFleetInfo.DataSource = dsFleetInfo.Tables[0];
                dgFleetInfo.DataBind();

                sn.Map.DsFleetInfo = dsFleetInfo;


            }


            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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
            this.dgFleetInfo.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgFleetInfo_PageIndexChanged);
            this.dgFleetInfo.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgFleetInfo_ItemDataBound);

        }
        #endregion

        private void dgFleetInfo_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            try
            {
                if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
                {

                    //Show Vehicle Info

                    e.Item.Cells[2].ToolTip = (string)base.GetLocalResourceObject("dgFleetInfo_Tooltip_BoxId") + ": " + e.Item.Cells[11].Text.ToString() + "; " + (string)base.GetLocalResourceObject("dgFleetInfo_Tooltip_LicensePlate") + ": " + e.Item.Cells[12].Text.ToString().TrimEnd();

                    //Show vehicle status

                    string VehicleStatus = e.Item.Cells[10].Text.ToString();
                    string OriginDateTime = e.Item.Cells[7].Text.ToString();
                    string LastCommunicatedDateTime = e.Item.Cells[14].Text.ToString();

                    if (Convert.ToDateTime(LastCommunicatedDateTime) > Convert.ToDateTime(OriginDateTime))
                    {
                        //e.Item.Cells[10].Text+="*";
                        e.Item.Cells[10].ToolTip = (string)base.GetLocalResourceObject("dgFleetInfo_Tooltip_NoGPSAvailable");
                    }



                    if (Convert.ToDateTime(OriginDateTime) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                    {
                        e.Item.Cells[4].ForeColor = Color.SlateGray;
                        e.Item.Cells[4].ToolTip = (string)base.GetLocalResourceObject("dgFleetInfo_Tooltip_PositionInfoWarning");
                    }

                    if ((VehicleStatus == Resources.Const.VehicleStatus_Parked) || (VehicleStatus == Resources.Const.VehicleStatus_Parked + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Untethered) || (VehicleStatus == Resources.Const.VehicleStatus_Untethered + "*"))
                    {
                        e.Item.Cells[10].ForeColor = Color.Red;
                    }

                    if ((VehicleStatus == Resources.Const.VehicleStatus_Idling) || (VehicleStatus == Resources.Const.VehicleStatus_Idling + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Tethered) || (VehicleStatus == Resources.Const.VehicleStatus_Tethered + "*"))
                    {
                        e.Item.Cells[10].ForeColor = Color.DarkOrange;
                    }


                    if ((VehicleStatus == Resources.Const.VehicleStatus_Moving) || (VehicleStatus == Resources.Const.VehicleStatus_Moving + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Ignition_ON + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Ignition_ON))
                    {
                        e.Item.Cells[10].ForeColor = Color.Green;
                    }



                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }



        protected void dgFleetInfo_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            SaveShowCheckBoxes();
            dgFleetInfo.CurrentPageIndex = e.NewPageIndex;
            DgFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));
            dgFleetInfo.SelectedIndex = -1;
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
            sn.Map.SelectedFleetID = Convert.ToInt32(cboFleet.SelectedItem.Value);
            sn.Map.DsFleetInfo = null;
            dgFleetInfo.CurrentPageIndex = 0;
            dgFleetInfo.SelectedIndex = -1;
            DgFleetInfo_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
        }

        protected void cmdUpdatePosition_Click(object sender, System.EventArgs e)
        {
            try
            {
                bool cmdSent = false;

                if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                {
                    sn.MessageText = "";
                    

                    LocationMgr.Location dbl = new LocationMgr.Location();
                    bool VehicleSelected = false;

                    SaveShowCheckBoxes();


                    //Delete old timeouts
                    sn.Cmd.DtUpdatePositionFails.Rows.Clear();

                    bool ShowTimer = false;

                    foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                        if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                        {
                            short ProtocolId = -1;
                            short CommModeId = -1;
                            Int64 sessionTimeOut = 0;
                            if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "", ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false))
                                if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "", ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true))
                                {
                                    return;
                                }

                            rowItem["ProtocolId"] = ProtocolId;

                            if (cmdSent)
                            {
                                ShowTimer = true;
                            }
                            else
                            {

                                //Create pop up message
                                string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                                strUrl = strUrl + "	var myname='Message';";
                                strUrl = strUrl + " var w=370;";
                                strUrl = strUrl + " var h=50;";
                                strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                                strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                                strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
                                strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);} ";
                                strUrl = strUrl + " NewWindow('../frmMessage.aspx');</script>";
                                sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SendingCommandToVehicle1") + " :" + rowItem["Description"] + " " + (string)base.GetLocalResourceObject("sn_MessageText_SendingCommandToVehicle2");

                                Response.Write(strUrl);

                            }

                            VehicleSelected = true;
                            rowItem["Updated"] = CommandStatus.Sent;
                        }

                    }


                    sn.Map.DsFleetInfo.Tables[0].DefaultView.Sort = "chkBoxShow" + " DESC";

                    this.dgFleetInfo.DataSource = sn.Map.DsFleetInfo.Tables[0];
                    this.dgFleetInfo.DataBind();

                    if (VehicleSelected)
                    {
                        if (ShowTimer)
                        {
                            this.dgFleetInfo.Visible = false;
                            this.tblWait.Visible = true;
                            sn.Map.TimerStatus = true;
                            this.cmdUpdatePosition.Enabled = false;
                            sn.Cmd.UpdatePositionSend = true;
                            this.cboFleet.Enabled = false;
                            Response.Write("<script language='javascript'> parent.frametimer.location.href='frmTimerPositionBigDetails.aspx' </script>");
                        }
                    }
                    else
                    {

                        //this.lblMessage.Text="Please select a Vehicle!"  ;
                        //this.lblMessage.Visible=true;  

                        //Create pop up message
                        string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                        strUrl = strUrl + "	var myname='Message';";
                        strUrl = strUrl + " var w=370;";
                        strUrl = strUrl + " var h=50;";
                        strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                        strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                        strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
                        strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);} ";

                        strUrl = strUrl + " NewWindow('../frmMessage.aspx');</script>";
                        sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectAVehicle");

                        Response.Write(strUrl);

                        this.tblWait.Visible = false;
                        return;
                    }


                }
                else
                {
                    //this.lblMessage.Text="Please select a Fleet!"  ;

                    //Create pop up message
                    string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                    strUrl = strUrl + "	var myname='Message';";
                    strUrl = strUrl + " var w=370;";
                    strUrl = strUrl + " var h=50;";
                    strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                    strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                    strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
                    strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);} ";

                    strUrl = strUrl + " NewWindow('../frmMessage.aspx');</script>";
                    sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectAFleet");

                    Response.Write(strUrl);


                    this.tblWait.Visible = false;
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        protected void cmdCancelUpdatePos_Click(object sender, System.EventArgs e)
        {
            try
            {
                LocationMgr.Location dbl = new LocationMgr.Location();
                

                foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                    {
                        if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), sn.Cmd.ProtocolTypeId), false))
                            if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), sn.Cmd.ProtocolTypeId), true))
                            {
                                //return;
                                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " cannot Cancel command for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                            }

                    }
                }


                this.dgFleetInfo.Visible = true;
                this.tblWait.Visible = false;
                this.cmdUpdatePosition.Enabled = true;
                this.cboFleet.Enabled = true;
                sn.Map.TimerStatus = false;
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        protected void cmdSelectAll_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                {
                    sn.MessageText = "";

                    DgFleetInfo_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));

                    foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                        rowItem["chkBoxShow"] = true;

                    }

                    this.dgFleetInfo.DataSource = sn.Map.DsFleetInfo;
                    this.dgFleetInfo.DataBind();
                }

                else
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
                    sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectAFleet");
                    Response.Write(strUrl);
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        protected void cmdUnselect_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                {
                    sn.MessageText = "";

                    DgFleetInfo_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));

                    foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                        rowItem["chkBoxShow"] = false;

                    }

                    this.dgFleetInfo.DataSource = sn.Map.DsFleetInfo;
                    this.dgFleetInfo.DataBind();

                }

                else
                {

                    //Create pop up message
                    string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                    strUrl = strUrl + "	var myname='Message';";
                    strUrl = strUrl + " var w=370;";
                    strUrl = strUrl + " var h=50;";
                    strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                    strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                    strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
                    strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);} ";

                    strUrl = strUrl + " NewWindow('../frmMessage.aspx');</script>";
                    sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectAFleet");
                    Response.Write(strUrl);

                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        private void SaveShowCheckBoxes()
        {
            for (int i = 0; i < dgFleetInfo.Items.Count; i++)
            {
                CheckBox ch = (CheckBox)(dgFleetInfo.Items[i].Cells[1].Controls[1]);

                foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (dgFleetInfo.Items[i].Cells[0].Text.ToString() == rowItem["VehicleId"].ToString())
                        rowItem["chkBoxShow"] = ch.Checked;
                }
            }
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

    }
}
