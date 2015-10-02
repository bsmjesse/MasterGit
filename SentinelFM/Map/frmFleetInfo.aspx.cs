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
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.Configuration;
using System.Globalization;

namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmFleetInfo.
    /// </summary>
    public partial class frmFleetInfo : SentinelFMBasePage
    {
        
        
        public string redirectURL;
        private DataSet dsFleetInfo;
        public VLF.MAP.ClientMapProxy map;
        public VLF.MAP.ClientMapProxy geoMap;
        public string strGeoMicroURL;
        public bool chkShowAutoPostBack = false;
        public int AutoRefreshTimer = 60000;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {


                AutoRefreshTimer = sn.User.GeneralRefreshFrequency;

                if (!Page.IsPostBack)
                {


                   if (sn.Map.SelectAllVehciles)
                   {
                      string MenuSelectAll = mnuOptions.Items[2].ChildItems[2].Text;
                      mnuOptions.Items[2].ChildItems[2].Text = MenuSelectAll.Substring(0, (MenuSelectAll.Length) - 3) + "[x]";
                   }


                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmFleet, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);   
                     
                    //if (sn.User.MapEngine[0].MapEngineID == MapType.NotSet)
                    //    this.chkAutoUpdate.Visible = false;


                    //					if (sn.User.MapId == VLF.MAP.MapType.MapPointWeb)
                    //						this.chkAutoUpdate.Visible=false;
                    //					else
                    //						this.chkAutoUpdate.Visible=true;



                    GuiSecurity(this);
                    if (sn.Map.MapRefresh)
                    {
                       string MapRefresh = mnuOptions.Items[3].Text;
                       mnuOptions.Items[3].Text = MapRefresh.Substring(0, (MapRefresh.Length) - 3) + "[x]";
                       chkShowAutoPostBack = true;
                       Response.Write("<script language='javascript'>window.setTimeout('AutoReloadDetails()'," + AutoRefreshTimer.ToString() + ")</script>");
                       //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                       //    sn.MapSolute.LoadVehicles(sn, sn.Map.DsFleetInfo.Tables[0], "Map");
                    }
                    else
                    {
                        chkShowAutoPostBack = false;
                        Response.Write("<script language='javascript'> clearTimeout();</script>");
                    }

                    this.dgFleetInfo.Visible = true;
                    this.tblWait.Visible = false;



                    //Get Vehicles Info
                    CboFleet_Fill();
                    FindExistingPreference();

                    if (sn.Map.SelectedFleetID != 0)
                    {
                        cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.Map.SelectedFleetID.ToString()));
                        DgFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));
                    }
                    else
                    {
                        if (sn.User.DefaultFleet != -1)
                        {
                            cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                            DgFleetInfo_Fill(Convert.ToInt32(sn.User.DefaultFleet));
                            sn.Map.SelectedFleetID = sn.User.DefaultFleet;
                        }
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
                        ShowErrorMessage();
                    }



                    if (sn.Map.ReloadMap)
                    {

                        Response.Write("<script language='javascript'> parent.frmVehicleMap.location.href='frmvehiclemap.aspx' </script>");
                        sn.Map.ReloadMap = false;

                       //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                       //{
                       //   sn.MapSolute.LoadVehicles(sn, sn.Map.DsFleetInfo.Tables[0], "Map");
                       //   sn.Map.ReloadMap = false;
                       //}
                       //else
                       //{
                       //   Response.Write("<script language='javascript'> parent.frmVehicleMap.location.href='frmvehiclemap.aspx' </script>");
                       //   sn.Map.ReloadMap = false;
                       //}
                    }

                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
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

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
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
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "GetVehiclesLastKnownPositionInfo.User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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

                dgFleetInfo.Columns[4].HeaderText = "Date " + ViewState["TimeZone"];
                dsFleetInfo.Tables[0].DefaultView.Sort = "chkBoxShow" + " DESC";
                dgFleetInfo.DataSource = dsFleetInfo.Tables[0];
                dgFleetInfo.DataBind();

                sn.Map.DsFleetInfo = dsFleetInfo;



            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }



        protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {

                sn.Map.ShowDefaultMap = true;
                sn.Map.DsFleetInfo = null;
                dgFleetInfo.CurrentPageIndex = 0;
                dgFleetInfo.SelectedIndex = -1;
                string MenuSelectAll = mnuOptions.Items[2].ChildItems[2].Text;
                mnuOptions.Items[2].ChildItems[2].Text = MenuSelectAll.Substring(0, (MenuSelectAll.Length) - 3) + "[ ]";
                

                if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                {
                    sn.Map.SelectedFleetID = Convert.ToInt32(cboFleet.SelectedItem.Value);
                    DgFleetInfo_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
                }
                else
                {
                    sn.Map.DsFleetInfo = null;
                    dgFleetInfo.DataBind();
                }

                if (sn.User.MapType != VLF.MAP.MapType.MapsoluteWeb)
                  Response.Write("<script language='javascript'> parent.frmVehicleMap.location.href='frmvehiclemap.aspx' </script>");
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        private void dgFleetInfo_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {

            try
            {
                //Header tooltip

                if (e.Item.ItemType == ListItemType.Header)
                {

                    //					e.Item.Cells[1].ToolTip ="Show on map" ;
                    //					e.Item.Cells[2].ToolTip ="Vehicle Description" ;
                    //					e.Item.Cells[3].ToolTip ="Last known address" ;

                }

                if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
                {

                    //Show Vehicle Info

                    e.Item.Cells[3].ToolTip = (string)base.GetLocalResourceObject("dgFleetInfo_Tooltip_BoxId") + ": " + e.Item.Cells[11].Text.ToString() + "; " + (string)base.GetLocalResourceObject("dgFleetInfo_Tooltip_LicensePlate") + ": " + e.Item.Cells[12].Text.ToString().TrimEnd();

                    //Show vehicle status

                    string VehicleStatus = e.Item.Cells[11].Text.ToString();
                    string OriginDateTime = e.Item.Cells[8].Text.ToString();
                    string LastCommunicatedDateTime = e.Item.Cells[15].Text.ToString();


                    e.Item.Cells[4].ToolTip = (string)base.GetLocalResourceObject("dgFleetInfo_StreetAddress");
                    e.Item.Cells[5].ToolTip = (string)base.GetLocalResourceObject("dgFleetInfo_MyDateTime");
                    e.Item.Cells[6].ToolTip = (string)base.GetLocalResourceObject("dgFleetInfo_CustomSpeed");
                    e.Item.Cells[7].ToolTip = (string)base.GetLocalResourceObject("dgFleetInfo_BoxArmed");
                    e.Item.Cells[11].ToolTip = (string)base.GetLocalResourceObject("dgFleetInfo_VehicleStatus");
                    e.Item.Cells[14].ToolTip = (string)base.GetLocalResourceObject("dgFleetInfo_CurrentStatus");


                    if (Convert.ToDateTime(LastCommunicatedDateTime) > Convert.ToDateTime(OriginDateTime))
                    {
                        //e.Item.Cells[10].Text+="*";
                        e.Item.Cells[11].ToolTip = (string)base.GetLocalResourceObject("dgFleetInfo_Tooltip_NoGPSAvailable");
                    }

                    if ((VehicleStatus == Resources.Const.VehicleStatus_Parked) || (VehicleStatus == Resources.Const.VehicleStatus_Parked + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Untethered ) || (VehicleStatus == Resources.Const.VehicleStatus_Untethered+"*"))
                    {
                        e.Item.Cells[11].ForeColor = Color.Red;
                    }

                    if ((VehicleStatus == Resources.Const.VehicleStatus_Idling) || (VehicleStatus == Resources.Const.VehicleStatus_Idling + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Tethered ) || (VehicleStatus == Resources.Const.VehicleStatus_Tethered +"*"))
                    {
                        e.Item.Cells[11].ForeColor = Color.DarkOrange;
                    }

                    if ((VehicleStatus == Resources.Const.VehicleStatus_Moving ) || (VehicleStatus == Resources.Const.VehicleStatus_Moving + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Ignition_ON + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Ignition_ON))
                    {
                        e.Item.Cells[11].ForeColor = Color.Green;
                    }

                    if (Convert.ToDateTime(OriginDateTime) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                    {
                        e.Item.Cells[5].ForeColor = Color.SlateGray;
                        e.Item.Cells[5].ToolTip = (string)base.GetLocalResourceObject("dgFleetInfo_Tooltip_OldPositionInfoWarning");
                    }


                    if (Convert.ToInt16(e.Item.Cells[16].Text) == Convert.ToInt16(VLF.CLS.Def.Enums.FirmwareType.SkyWave))
                    {
                        e.Item.Cells[11].Text = "";
                        e.Item.Cells[11].ToolTip = "";
                    }


                    if (Convert.ToBoolean(e.Item.Cells[17].Text.ToString().TrimEnd()))
                    {
                        e.Item.Cells[11].ForeColor = Color.SandyBrown;
                    }

                }


            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }




        protected void cmdMapIT()
        {
            try
            {
                if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                {

                    sn.MessageText = "";
                    SaveShowCheckBoxes();

                    sn.Map.SelectedFleetID = Convert.ToInt32(cboFleet.SelectedItem.Value);
                    DgFleetInfo_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));

                    sn.Map.DsFleetInfo.Tables[0].DefaultView.Sort = "chkBoxShow" + " DESC";
                    this.dgFleetInfo.DataSource = sn.Map.DsFleetInfo.Tables[0];
                    this.dgFleetInfo.DataBind();


                    bool VehicleSelected = false;
                    foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                       if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                       {
                          VehicleSelected = true;
                          break; 
                       }
                    }

                    if (VehicleSelected)
                    {
                       //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                       //   sn.MapSolute.LoadVehicles(sn, sn.Map.DsFleetInfo.Tables[0], "Map");
                       //else
                          Response.Write("<script language='javascript'> parent.frmVehicleMap.location.href='frmvehiclemap.aspx' </script>");

                        dgFleetInfo.CurrentPageIndex = 0;
                        dgFleetInfo.DataSource = sn.Map.DsFleetInfo.Tables[0];
                        dgFleetInfo.DataBind();
                        dgFleetInfo.SelectedIndex = -1;



                        //Check for Old DateTime
                        string strMessage = "";
                        strMessage = CheckDateTime();
                        if (strMessage != "")
                        {

                            sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_PositionInfoForVehicle1") + ":" + strMessage + " " + (string)base.GetLocalResourceObject("sn_MessageText_PositionInfoForVehicle2");
                            ShowErrorMessage();
                         }

                    }
                    else
                    {

                        sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle");
                        ShowErrorMessage();
                        return;
                    }

                }
                else
                {
                    sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
                    ShowErrorMessage();
                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        private void ShowErrorMessage()
        {
            //Create pop up message
            string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
            strUrl = strUrl + "	var myname='Message';";
            strUrl = strUrl + " var w=370;";
            strUrl = strUrl + " var h=50;";
            strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
            strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
            strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
            strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);}";

            strUrl = strUrl + " NewWindow('../frmMessage.aspx');</script>";

            Response.Write(strUrl);
        }


        protected void cmdUpdatePosition()
        {
            try
            {


                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                DataColumn dc;

                dc = new DataColumn("Freq", Type.GetType("System.Int64"));
                dc.DefaultValue = 0;
                dt.Columns.Add(dc);


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
                    Int64 sessionTimeOut = 0;




                    foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                        if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                        {
                            short ProtocolId = -1;
                            short CommModeId = -1;
                            string errMsg = "";
                            if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "", ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false,System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                                if (errMsg == "")
                                {
                                    if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "", ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true,System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                                    {
                                        if (errMsg != "")
                                            sn.MessageText = errMsg;
                                        else
                                            sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SendCommandFailedError") + ": " + rowItem["Description"];

                                        ShowErrorMessage();
                                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Update position failed. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                                        return;
                                    }
                                }
                                else
                                {
                                    sn.MessageText = errMsg;
                                    ShowErrorMessage();
                                    return;
                                }


                            DataRow dr = dt.NewRow();

                            if (sessionTimeOut > 0)
                                dr["Freq"] = Convert.ToInt64(Math.Round(sessionTimeOut / 15.0)) * 1000;
                            else
                                dr["Freq"] = 2000;

                            dt.Rows.Add(dr);

                            rowItem["ProtocolId"] = ProtocolId;
                            sn.Cmd.ProtocolTypeId = ProtocolId;

                            if (cmdSent)
                            {
                                ShowTimer = true;
                            }
                            else
                            {

                                //Create pop up message
                                sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SendCommandToVehicle1") + " :" + rowItem["Description"].ToString().TrimEnd() + " " + (string)base.GetLocalResourceObject("sn_MessageText_SendCommandToVehicle2");
                                ShowErrorMessage();

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


                            DataRow[] drCollections = null;
                            drCollections = sn.Map.DsFleetInfo.Tables[0].Select("chkBoxShow=true", "", DataViewRowState.CurrentRows);
                            if (drCollections.Length == 1)
                            {
                                if (sessionTimeOut > 60)
                                {
                                    Int64 SessionTime = Convert.ToInt64(Math.Round(sessionTimeOut / 60.0));
                                    this.lblUpdatePosition.Text = (string)base.GetLocalResourceObject("lblUpdatePosition_Text_Minutes1") + " " + SessionTime.ToString() + " " + (string)base.GetLocalResourceObject("lblUpdatePosition_Text_Minutes2");
                                }
                                else if (sessionTimeOut == 0)
                                    sn.Cmd.GetCommandStatusRefreshFreq = 15000;
                                else
                                    this.lblUpdatePosition.Text = (string)base.GetLocalResourceObject("lblUpdatePosition_Text_Seconds1") + " " + sessionTimeOut + " " + (string)base.GetLocalResourceObject("lblUpdatePosition_Text_Seconds2");
                            }

                            try
                            {
                                ds.Tables.Add(dt);
                                DataView dv = ds.Tables[0].DefaultView;
                                dv.Sort = "Freq" + " DESC";
                                sn.Cmd.GetCommandStatusRefreshFreq = Convert.ToInt64(dv[0].Row[0]);
                            }
                            catch
                            {
                                sn.Cmd.GetCommandStatusRefreshFreq = 1000;
                            }

                            sn.Map.TimerStatus = true;
                            this.mnuOptions.Enabled = false;  
                            sn.Cmd.UpdatePositionSend = true;
                            Response.Write("<script language='javascript'> parent.frmStatus.location.href='frmTimerPosition.aspx' </script>");
                        }
                    }
                    else
                    {

                        sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle");
                        ShowErrorMessage();
                        this.tblWait.Visible = false;
                        return;
                    }


                }
                else
                {

                    sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
                    ShowErrorMessage();

                    this.tblWait.Visible = false;
                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        protected void dgFleetInfo_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {

            try
            {


                SaveShowCheckBoxes();
                dgFleetInfo.CurrentPageIndex = e.NewPageIndex;


                if ((ViewState["VehicleSort"] == null) && (ViewState["DateTimeSort"] == null) && (ViewState["StatusSort"] == null))
                    sn.Map.DsFleetInfo.Tables[0].DefaultView.Sort = "chkBoxShow" + " DESC";


                dgFleetInfo.DataSource = sn.Map.DsFleetInfo.Tables[0];
                dgFleetInfo.DataBind();

                dgFleetInfo.SelectedIndex = -1;

                if (sn.Map.MapRefresh)
                {
                    chkShowAutoPostBack = true;
                    Response.Write("<script language='javascript'>window.setTimeout('AutoReloadDetails()'," + AutoRefreshTimer.ToString() + ")</script>");
                }
                else
                {
                    chkShowAutoPostBack = false;
                    Response.Write("<script language='javascript'> clearTimeout();</script>");
                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }


        private void cmdRefresh_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                {
                    SaveShowCheckBoxes();
                    DgFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));

                    //Check if vehicles selected
                    bool VehicleSelected = false;
                    foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                       if (rowItem["chkBoxShow"].ToString().ToLower()  == "true")
                            VehicleSelected = true;
                    }

                    if (VehicleSelected)
                       //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                       //   sn.MapSolute.LoadVehicles(sn, sn.Map.DsFleetInfo.Tables[0], "Map");
                       //else
                          Response.Write("<script language='javascript'> parent.frmVehicleMap.location.href='frmvehiclemap.aspx' </script>");
                

                }

                else
                {
                    sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
                    ShowErrorMessage();
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        private void dgFleetInfo_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
        {
            if (sn.Map.DsFleetInfo != null)
            {
                sn.Map.DsFleetInfo.Tables[0].DefaultView.Sort = "Description";
                this.dgFleetInfo.DataSource = sn.Map.DsFleetInfo.Tables[0];
                this.dgFleetInfo.DataBind();
            }
        }


        private void FindExistingPreference()
        {
            try
            {

                // Changes for TimeZone Feature start
                if (sn.User.NewFloatTimeZone < 0)
                    ViewState["TimeZone"] = "(GMT-" + sn.User.NewFloatTimeZone.ToString() + ")";
                else
                    ViewState["TimeZone"] = "(GMT+" + sn.User.NewFloatTimeZone.ToString() + ")";    // Changes for TimeZone Feature end
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
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
                                CancelCommand();
                                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, " cannot Cancel command for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                                return;
                            }

                    }
                }

                CancelCommand();
            }

            catch (NullReferenceException Ex)
            {
                CancelCommand();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                CancelCommand();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }


        private void CancelCommand()
        {
            this.dgFleetInfo.Visible = true;
            this.tblWait.Visible = false;
            this.mnuOptions.Enabled = true;  
            sn.Map.TimerStatus = false;
        }


        private bool MapIt()
        {
            try
            {
                bool VehicleSelected = false;

                for (int i = 0; i < dgFleetInfo.Items.Count; i++)
                {
                    CheckBox ch = (CheckBox)(dgFleetInfo.Items[i].Cells[1].Controls[1]);
                    if (ch.Checked)
                        VehicleSelected = true;

                    foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                        if (dgFleetInfo.Items[i].Cells[0].Text.ToString() == rowItem["VehicleId"].ToString())
                            rowItem["chkBoxShow"] = ch.Checked;

                    }

                }


                sn.Map.DsFleetInfo.Tables[0].DefaultView.Sort = "chkBoxShow" + " DESC";
                this.dgFleetInfo.DataSource = sn.Map.DsFleetInfo.Tables[0];
                this.dgFleetInfo.DataBind();

                if (VehicleSelected)
                   //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                   //   sn.MapSolute.LoadVehicles(sn, sn.Map.DsFleetInfo.Tables[0], "Map");
                   //else
                      Response.Write("<script language='javascript'> parent.frmVehicleMap.location.href='frmvehiclemap.aspx' </script>");
                

                return VehicleSelected;
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

                return false;
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                return false;
            }

        }

        public void SaveShowChecks(object sender, System.EventArgs e)
        {


            sn.Map.DrawAllVehicles = true;
            chkShowAutoPostBack = false;

            //Disable Show All if any disabled
            CheckBox chk;
            chk = (CheckBox)sender;

          


            if (sn.Map.MapRefresh)
            {
                chkShowAutoPostBack = true;
                SaveShowCheckBoxes();
                Response.Write("<script language='javascript'>window.setTimeout('AutoReloadDetails()'," + AutoRefreshTimer.ToString() + ")</script>");
            }
        }

        protected void chkAutoUpdateChanged(bool  Refresh)
        {
           if (Refresh)
           {
              if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
              {

                 sn.Map.MapRefresh = Refresh;
                 sn.Map.SelectedFleetID = Convert.ToInt32(cboFleet.SelectedItem.Value);
                 chkShowAutoPostBack = Refresh;
                 RefreshPosition();
                 Response.Write("<script language='javascript'>window.setTimeout('AutoReloadDetails()'," + AutoRefreshTimer.ToString() + ")</script>");
              }
              else
              {
                 chkShowAutoPostBack = Refresh;
                 sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
                 string MapRefresh = mnuOptions.Items[3].Text;
                 mnuOptions.Items[3].Text = MapRefresh.Substring(0, (MapRefresh.Length) - 3) + "[ ]";
                 ShowErrorMessage();
                 Response.Write("<script language='javascript'> clearTimeout();</script>");
              }

           }
           else
           {
              sn.Map.MapRefresh = false;
              RefreshPosition();
           }

        }


        private void RefreshPosition()
        {
            try
            {

                SaveShowCheckBoxes();
                DgFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));


                //Check if vehicles selected
                bool VehicleSelected = false;
                foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (rowItem["chkBoxShow"].ToString().ToLower()  == "true")
                        VehicleSelected = true;
                }

                if (VehicleSelected)
                   //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                   //   sn.MapSolute.LoadVehicles(sn, sn.Map.DsFleetInfo.Tables[0] , "Map");
                   //else
                      Response.Write("<script language='javascript'> parent.frmVehicleMap.location.href='frmvehiclemap.aspx' </script>");
                

            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
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
                string strSQL = "Status=" + Convert.ToString((int)CommandStatus.CommTimeout);
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
                strSQL = "Status=" + Convert.ToString((int)CommandStatus.Pending);
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
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
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
                    sn.MessageText += "\n_________________________________________\n";




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
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }



        protected void cmdBigMap()
        {
            try
            {
                string strUrl = "";

                if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                {

                    sn.Map.SelectedFleetID = Convert.ToInt32(cboFleet.SelectedItem.Value);
                    SaveShowCheckBoxes();

                    bool VehicleSelected = false;
                    foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                        if (rowItem["chkBoxShow"].ToString() == "true")
                            VehicleSelected = true;
                    }

                    if (VehicleSelected)
                    {
                        sn.Map.DsFleetInfo.Tables[0].DefaultView.Sort = "chkBoxShow" + " DESC";
                        dgFleetInfo.CurrentPageIndex = 0;
                        dgFleetInfo.DataSource = sn.Map.DsFleetInfo.Tables[0];
                        dgFleetInfo.DataBind();
                        dgFleetInfo.SelectedIndex = -1;


                        //Check for Old DateTime
                        string strMessage = "";
                        strMessage = CheckDateTime();
                        if (strMessage != "")
                        {
                            sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_PositionInfoForVehicle1") + ":" + strMessage + " " + (string)base.GetLocalResourceObject("sn_MessageText_PositionInfoForVehicle2");
                            ShowErrorMessage();
                        }

                    }
                    else
                    {
                        sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle");
                        ShowErrorMessage();
                        return;
                    }

                    strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                    strUrl = strUrl + "	var myname='';";
                    strUrl = strUrl + " var w=950;";
                    strUrl = strUrl + " var h=650;";
                    strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                    strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                    strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=auto,fullscreen=yes,menubar=0,'; ";
                    strUrl = strUrl + " win = window.open(mypage, myname, winprops);} ";
                    strUrl = strUrl + " NewWindow('frmBigMapWait.aspx');</script>";
                    LoadBigDefaultMap();
                    Response.Write(strUrl);
                    return;
                }
                else
                {

                    sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
                    ShowErrorMessage();
                    return;
                }

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }



        protected void cmdBigDetails()
        {
            try
            {
                if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                {
                    sn.Map.SelectedFleetID = Convert.ToInt32(cboFleet.SelectedItem.Value);
                    SaveShowCheckBoxes();
                }

                string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                strUrl = strUrl + "	var myname='';";
                strUrl = strUrl + " var w=950;";
                strUrl = strUrl + " var h=650;";
                strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=auto,fullscreen=yes,menubar=0,'; ";
                strUrl = strUrl + " win = window.open(mypage, myname, winprops); }";

                strUrl = strUrl + " NewWindow('frmBigDetailsFrame.htm');</script>";
                Response.Write(strUrl);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
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

        protected void chkShowAllChanged(bool status)
        {
            try
            {
                if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                {

                    sn.MessageText = "";
                   
                    DgFleetInfo_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));

                    foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                          rowItem["chkBoxShow"] = status;
                    }

                    this.dgFleetInfo.DataSource = sn.Map.DsFleetInfo;
                    this.dgFleetInfo.DataBind();



                    if (sn.Map.MapRefresh)
                    {
                        sn.Map.DrawAllVehicles = true;
                        chkShowAutoPostBack = true;
                    }

                }

                else
                {
                    sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
                    ShowErrorMessage();
                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
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
                   {
                      if (ch.Checked)
                        rowItem["chkBoxShow"] = true;
                      else
                        rowItem["chkBoxShow"] = false;

                      continue; 
                   }
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



        private string CheckDateTime()
        {
            string OldDateVehicleNames = "";

            foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
            {
                if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                {
                    if (Convert.ToDateTime(rowItem["OriginDateTime"]) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                        OldDateVehicleNames += rowItem["Description"].ToString().TrimEnd() + ",";
                }

            }

            if (OldDateVehicleNames.Length > 0)
                OldDateVehicleNames = OldDateVehicleNames.Substring(0, OldDateVehicleNames.Length - 1);


            return OldDateVehicleNames;

        }


        public void LoadBigDefaultMap()
        {
            try
            {
                // create ClientMapProxy only for mapping
                map = new VLF.MAP.ClientMapProxy(sn.User.MapEngine);
                if (map == null)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                    return;
                }
                map.Vehicles.Clear();
                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
                map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;
                map.GetDefaultBigMap();
                sn.Map.SavesMapStateToViewState(sn, map);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        protected void lnkDateTime_Click()
        {


            if (ViewState["DateTimeSort"] == null)
                ViewState["DateTimeSort"] = "ASC";
            else if (ViewState["DateTimeSort"].ToString() == "ASC")
                ViewState["DateTimeSort"] = "DESC";
            else if (ViewState["DateTimeSort"].ToString() == "DESC")
                ViewState["DateTimeSort"] = "ASC";

            if ((sn.Map.DsFleetInfo != null) && (sn.Map.DsFleetInfo.Tables.Count > 0))
            {
                sn.Map.DsFleetInfo.Tables[0].DefaultView.Sort = "MyDateTime " + ViewState["DateTimeSort"].ToString();
                this.dgFleetInfo.DataSource = sn.Map.DsFleetInfo.Tables[0];
                this.dgFleetInfo.DataBind();
            }
        }

        protected void lnkStatus_Click()
        {

            if (ViewState["StatusSort"] == null)
                ViewState["StatusSort"] = "ASC";
            else if (ViewState["StatusSort"].ToString() == "ASC")
                ViewState["StatusSort"] = "DESC";
            else if (ViewState["StatusSort"].ToString() == "DESC")
                ViewState["StatusSort"] = "ASC";

            if ((sn.Map.DsFleetInfo != null) && (sn.Map.DsFleetInfo.Tables.Count > 0))
            {
                sn.Map.DsFleetInfo.Tables[0].DefaultView.Sort = "VehicleStatus " + ViewState["StatusSort"].ToString();
                this.dgFleetInfo.DataSource = sn.Map.DsFleetInfo.Tables[0].DefaultView;
                this.dgFleetInfo.DataBind();
            }

        }

        protected void lnkCommand_Click()
        {

            if (ViewState["VehicleSort"] == null)
                ViewState["VehicleSort"] = "ASC";
            else if (ViewState["VehicleSort"].ToString() == "ASC")
                ViewState["VehicleSort"] = "DESC";
            else if (ViewState["VehicleSort"].ToString() == "DESC")
                ViewState["VehicleSort"] = "ASC";

            if ((sn.Map.DsFleetInfo != null) && (sn.Map.DsFleetInfo.Tables.Count > 0))
            {
                sn.Map.DsFleetInfo.Tables[0].DefaultView.Sort = "Description " + ViewState["VehicleSort"].ToString();
                this.dgFleetInfo.DataSource = sn.Map.DsFleetInfo.Tables[0];
                this.dgFleetInfo.DataBind();
            }
        }


       protected void mnuFleetActions_MenuItemClick(object sender, MenuEventArgs e)
       {
          if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
          {
             sn.Map.SelectedFleetID = Convert.ToInt32(cboFleet.SelectedItem.Value);
             SaveShowCheckBoxes();
             bool vehicleSelected = false;
             foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
             {
                if (rowItem["chkBoxShow"].ToString().ToLower()  == "true")
                {
                   vehicleSelected = true;
                   break;
                }
             }

             if (!vehicleSelected)
             {
                sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle");
                ShowErrorMessage();
                return; 
             }

             
             switch (mnuFleetActions.SelectedValue)
             {
                case "1":
                   foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                   {
                      if (rowItem["chkBoxShow"].ToString().ToLower()  == "true")
                      {
                         string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                         strUrl = strUrl + "	var myname='Message';";
                         strUrl = strUrl + " var w=370;";
                         strUrl = strUrl + " var h=50;";
                         strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                         strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                         strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
                         strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);}";

                         strUrl = strUrl + " NewWindow('frmFleetVehicles.aspx');</script>";

                         Response.Write(strUrl);
                         return;
                      }
                   }

                   break;
                case "2":

                   if (cboFleet.SelectedItem.Text.TrimEnd() == Resources.Const.defaultFleetName)
                   {
                      sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_CannotDeleteVehicles") + Resources.Const.defaultFleetName;
                      ShowErrorMessage();
                      return;
                   }

                   
                   ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                   foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                   {
                      if (rowItem["chkBoxShow"].ToString().ToLower()  == "true")
                      {
                         if (objUtil.ErrCheck(dbf.DeleteVehicleFromFleet(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToInt32(rowItem["vehicleId"])), false))
                            if (objUtil.ErrCheck(dbf.DeleteVehicleFromFleet(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToInt32(rowItem["vehicleId"])), false))
                            {
                               return;
                            }
                      }
                   }

                   DgFleetInfo_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
                   break;
             }
          }
          else
          {
                sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
                ShowErrorMessage();
                return;
          }
       }

       protected void cmdSearch_Click(object sender, EventArgs e)
       {

       }
       protected void mnuOptions_MenuItemClick(object sender, MenuEventArgs e)
       {
          switch (e.Item.Value)
          {
             case "UpdatePosition":
                cmdUpdatePosition();
                break;
             case "MapIt":
                //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                //  sn.MapSolute.ClearMapObjects(sn.MapSolute.MapObjectsList);    
                cmdMapIT();

                break;
             case "AutoRefresh":

                if (e.Item.Text.Substring(e.Item.Text.Length - 3) == "[x]")
                {
                   chkAutoUpdateChanged(false);
                   e.Item.Text = e.Item.Text.Substring(0, e.Item.Text.Length - 3) + "[ ]";
                }
                else
                {
                   e.Item.Text = e.Item.Text.Substring(0, (e.Item.Text.Length) - 3) + "[x]";
                   chkAutoUpdateChanged(true);
                }
                break; 
             case "SelectAll":

                if (Convert.ToInt32(cboFleet.SelectedItem.Value) == -1)
                {
                   sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
                   ShowErrorMessage();
                   return; 
                }

                if (e.Item.Text.Substring(e.Item.Text.Length - 3) == "[x]")
                {
                   sn.Map.SelectAllVehciles = false;    
                   chkShowAllChanged(false);
                   e.Item.Text = e.Item.Text.Substring(0,e.Item.Text.Length - 3) + "[ ]";
                }
                else
                {
                   sn.Map.SelectAllVehciles = true;
                   e.Item.Text = e.Item.Text.Substring(0, (e.Item.Text.Length) - 3) + "[x]";
                   chkShowAllChanged(true);
                }
                break; 
             case "FullMap":
                cmdBigMap();
                break;
             case "FullGrid":
                cmdBigDetails();
                break; 
             case "SortDateTime"   :
                lnkDateTime_Click();
                break;
             case "SortVehicle":
                lnkCommand_Click();
                break;
             case "SortStatus":
                lnkStatus_Click();
                break;
          }

       }
       protected void dgFleetInfo_SelectedIndexChanged(object sender, EventArgs e)
       {

            sn.History.FleetId =Convert.ToInt64(this.cboFleet.SelectedItem.Value);
            sn.History.VehicleId = Convert.ToInt64(dgFleetInfo.SelectedItem.Cells[0].Text);
            sn.History.FromDate = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
            sn.History.ToDate = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
            sn.History.FromHours = "08" ;
            sn.History.ToHours = DateTime.Now.AddHours(1).Hour.ToString() ;
            sn.History.RedirectFromMapScreen = true;   
            Response.Write("<SCRIPT Language='javascript'>parent.parent.main.window.location='../History/frmhistmain.aspx' </SCRIPT>");
       }
}

}




