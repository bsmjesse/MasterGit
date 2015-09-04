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

namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmMessages.
    /// </summary>
    public partial class frmMessages : SentinelFMBasePage
    {
        
        

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {

                this.txtFrom.Text = Request[this.txtFrom.UniqueID];

                if (!Page.IsPostBack)
                {
                    GuiSecurity(this);
                    this.tblNoData.Visible = false;
                    clsMisc.cboHoursFill(ref cboHoursFrom);
                    clsMisc.cboHoursFill(ref cboHoursTo);
                    CboFleet_Fill();



                    //this.txtFrom.Text=DateTime.Now.AddDays(-1).ToShortDateString() ;
                    //sn.Message.FromDate=DateTime.Now.AddDays(-1).ToShortDateString() ;

                    //this.txtTo.Text=DateTime.Now.AddDays(1).ToShortDateString();
                    //sn.Message.ToDate=DateTime.Now.AddDays(1).ToShortDateString();


                    this.txtFrom.Text = DateTime.Now.AddHours(-12).ToShortDateString();
                    sn.Message.FromDate = DateTime.Now.AddHours(-12).ToShortDateString();

                    this.txtTo.Text = DateTime.Now.ToShortDateString();
                    sn.Message.ToDate = DateTime.Now.ToShortDateString();


                    //this.cboHoursFrom.SelectedIndex = -1;
                    //    for (int i=0;i<=cboHoursFrom.Items.Count-1;i++)
                    //    {
                    //        if (Convert.ToInt32(cboHoursFrom.Items[i].Value) ==8)
                    //        {
                    //            cboHoursFrom.Items[i].Selected = true;
                    //            sn.History.FromHours=Convert.ToString(8);
                    //            break;
                    //        }
                    //    }

                    //this.cboHoursTo.SelectedIndex = -1;
                    //    for (int i=0;i<=cboHoursTo.Items.Count-1;i++)
                    //    {
                    //        if (cboHoursTo.Items[i].Value == DateTime.Now.AddHours(1).Hour.ToString())
                    //        {
                    //            cboHoursTo.Items[i].Selected = true;
                    //            break;
                    //        }
                    //    }




                    this.cboHoursFrom.SelectedIndex = -1;
                    for (int i = 0; i <= cboHoursFrom.Items.Count - 1; i++)
                    {
                        if (Convert.ToInt32(cboHoursFrom.Items[i].Value) == DateTime.Now.AddHours(-12).Hour)
                        {
                            cboHoursFrom.Items[i].Selected = true;
                            sn.History.FromHours = DateTime.Now.AddHours(-12).Hour.ToString();
                            break;
                        }
                    }

                    this.cboHoursTo.SelectedIndex = -1;
                    for (int i = 0; i <= cboHoursTo.Items.Count - 1; i++)
                    {
                        if (Convert.ToInt32(cboHoursTo.Items[i].Value) == DateTime.Now.AddHours(1).Hour)
                        {
                            cboHoursTo.Items[i].Selected = true;
                            sn.History.ToHours = DateTime.Now.AddHours(1).Hour.ToString();
                            break;
                        }
                    }



                    if (sn.Message.FleetId != 0)
                    {
                        this.cboFleet.SelectedIndex = -1;
                        for (int i = 0; i <= cboFleet.Items.Count - 1; i++)
                        {
                            if (cboFleet.Items[i].Value == sn.Message.FleetId.ToString())
                                cboFleet.Items[i].Selected = true;
                        }
                    }

                    else
                    {
                        if (sn.User.DefaultFleet != -1)
                        {
                            cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                            CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));
                            this.lblVehicleName.Visible = true;
                            this.cboVehicle.Visible = true;
                        }
                    }


                    if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                    {

                        CboVehicle_Fill(Convert.ToInt32(sn.Message.FleetId));

                        this.cboVehicle.SelectedIndex = -1;

                        for (int i = 0; i <= cboVehicle.Items.Count - 1; i++)
                        {
                            if (cboVehicle.Items[i].Value == sn.Message.BoxId.ToString())
                                cboVehicle.Items[i].Selected = true;
                        }

                        this.lblVehicleName.Visible = true;
                        this.cboVehicle.Visible = true;


                        SetHours();
                        SetDirection();


                        sn.Message.FromDate = Convert.ToDateTime(sn.Message.FromDate).ToShortDateString() + " " + sn.Message.FromHours;
                        sn.Message.ToDate = Convert.ToDateTime(sn.Message.ToDate).ToShortDateString() + " " + sn.Message.ToHours;
                        dgMessages_Fill_NewTZ();

                    }
                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
        }

        protected void cmdNewMessage_Click(object sender, System.EventArgs e)
        {
            string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
            strUrl = strUrl + "	var myname='Message';";
            strUrl = strUrl + " var w=560;";
            strUrl = strUrl + " var h=520;";
            strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
            strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
            strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,toolbar=0,scrollbars=0,menubar=0,'; ";
            strUrl = strUrl + " win = window.open(mypage, myname, winprops);} ";

            strUrl = strUrl + " NewWindow('frmNewMessageMain.aspx');</script>";
            Response.Write(strUrl);
        }



        // Changes for TimeZone Feature start
        private void dgMessages_Fill_NewTZ()
        {
            try
            {


                StringReader strrXML = null;

                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();

                string strFromDT = sn.Message.FromDate;
                string strToDT = sn.Message.ToDate;
                string xml = "";

                if ((Convert.ToInt32(this.cboFleet.SelectedItem.Value) != -1) && (Convert.ToInt32(this.cboVehicle.SelectedItem.Value) == 0))
                {
                    if (objUtil.ErrCheck(hist.GetFleetTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDT).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                        if (objUtil.ErrCheck(hist.GetFleetTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDT).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
                        {
                            this.tblNoData.Visible = true;
                            return;
                        }
                }
                else
                {
                    if (objUtil.ErrCheck(hist.GetTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                        if (objUtil.ErrCheck(hist.GetTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
                        {
                            this.tblNoData.Visible = true;
                            return;
                        }
                }

                if (xml == "")
                {
                    this.dgMessages.Visible = false;
                    this.tblNoData.Visible = true;
                    return;
                }

                strrXML = new StringReader(xml);
                this.tblNoData.Visible = false;
                this.dgMessages.Visible = true;

                DataSet ds = new DataSet();
                ds.ReadXml(strrXML);



                DataColumn MsgKey = new DataColumn("MsgKey", Type.GetType("System.String"));
                MsgKey.DefaultValue = "";
                ds.Tables[0].Columns.Add(MsgKey);

                if (ds.Tables[0].Columns.IndexOf("To") == -1)
                {
                    DataColumn colTo = new DataColumn("To", Type.GetType("System.String"));
                    colTo.DefaultValue = "";
                    ds.Tables[0].Columns.Add(colTo);
                }

                foreach (DataRow rowItem in ds.Tables[0].Rows)
                {
                    rowItem["MsgKey"] = rowItem["MsgId"].ToString() + ";" + rowItem["Vehicleid"].ToString();
                    rowItem["MsgDateTime"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString());
                }

                this.dgMessages.DataSource = ds;
                this.dgMessages.DataBind();

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
        // Changes for TimeZone Feature end


        private void dgMessages_Fill()
        {
            try
            {

                
                StringReader strrXML = null;

                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();

                string strFromDT = sn.Message.FromDate;
                string strToDT = sn.Message.ToDate;
                string xml = "";

                if ((Convert.ToInt32(this.cboFleet.SelectedItem.Value) != -1) && (Convert.ToInt32(this.cboVehicle.SelectedItem.Value) == 0))
                {
                    if (objUtil.ErrCheck(hist.GetFleetTextMessagesFullInfo(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDT).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                        if (objUtil.ErrCheck(hist.GetFleetTextMessagesFullInfo(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDT).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
                        {
                            this.tblNoData.Visible = true;
                            return;
                        }
                }
                else
                {
                    if (objUtil.ErrCheck(hist.GetTextMessagesFullInfo(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                        if (objUtil.ErrCheck(hist.GetTextMessagesFullInfo(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
                        {
                            this.tblNoData.Visible = true;
                            return;
                        }
                }

                if (xml == "")
                {
                    this.dgMessages.Visible = false;
                    this.tblNoData.Visible = true;
                    return;
                }

                strrXML = new StringReader(xml);
                this.tblNoData.Visible = false;
                this.dgMessages.Visible = true;

                DataSet ds = new DataSet();
                ds.ReadXml(strrXML);



                DataColumn MsgKey = new DataColumn("MsgKey", Type.GetType("System.String"));
                MsgKey.DefaultValue = "";
                ds.Tables[0].Columns.Add(MsgKey);

                if (ds.Tables[0].Columns.IndexOf("To") == -1)
                {
                    DataColumn colTo = new DataColumn("To", Type.GetType("System.String"));
                    colTo.DefaultValue = "";
                    ds.Tables[0].Columns.Add(colTo);
                }

                foreach (DataRow rowItem in ds.Tables[0].Rows)
                {
                    rowItem["MsgKey"] = rowItem["MsgId"].ToString() + ";" + rowItem["Vehicleid"].ToString();
                    rowItem["MsgDateTime"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString());
                }

                this.dgMessages.DataSource = ds;
                this.dgMessages.DataBind();

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

                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));

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

        private void dgMessages_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                if (e.Item.Cells[4].Text.Length > 57)
                {
                    e.Item.Cells[4].ToolTip = e.Item.Cells[4].Text;
                    e.Item.Cells[4].Text = e.Item.Cells[4].Text.Substring(0, 57) + "...";
                }
                else
                {
                    e.Item.Cells[4].ToolTip = e.Item.Cells[4].Text;
                }

                if (e.Item.Cells[5].Text.TrimEnd().Length > 40)
                {
                    e.Item.Cells[5].ToolTip = e.Item.Cells[5].Text.TrimEnd();
                    e.Item.Cells[5].Text = e.Item.Cells[5].Text.TrimEnd().Substring(0, 40) + "...";
                }
                else
                {
                    e.Item.Cells[5].ToolTip = e.Item.Cells[5].Text.TrimEnd();
                }
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
            this.dgMessages.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgMessages_PageIndexChanged);

        }
        #endregion

        protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            {
                this.cboVehicle.Visible = true;
                this.lblVehicleName.Visible = true;
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
            }
        }

        protected void cmdShowMessages_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strFromDate = "";
                string strToDate = "";
                dgMessages.SelectedIndex = -1;
                dgMessages.CurrentPageIndex = 0;

                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
                    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
                    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
                    strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
                    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
                    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
                    strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";



                this.lblMessage.Text = "";

                if (Convert.ToDateTime(strFromDate) > Convert.ToDateTime(strToDate))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_InvalidDateError");
                    return;
                }
                else
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "";
                }


                sn.Message.FromDate = strFromDate;
                sn.Message.ToDate = strToDate;
                sn.Message.FromHours = this.cboHoursFrom.SelectedItem.Value;
                sn.Message.ToHours = this.cboHoursTo.SelectedItem.Value;
                sn.Message.DsMessages = null;
                sn.Message.VehicleName = this.cboVehicle.SelectedItem.Text.Replace("'", "''");
                sn.Message.FleetName = this.cboFleet.SelectedItem.Text.Replace("'", "''");
                sn.Message.BoxId = Convert.ToInt32(this.cboVehicle.SelectedItem.Value);

                SetDirection();

                dgMessages_Fill_NewTZ();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
        }


        private void SetDirection()
        {
            if (Convert.ToInt16(this.cboDirection.SelectedItem.Value) == 0)
                sn.Message.MessageDirectionId = Convert.ToInt32(VLF.CLS.Def.Enums.TxtMsgDirectionType.Both);

            if (Convert.ToInt16(this.cboDirection.SelectedItem.Value) == 1)
                sn.Message.MessageDirectionId = Convert.ToInt32(VLF.CLS.Def.Enums.TxtMsgDirectionType.In);

            if (Convert.ToInt16(this.cboDirection.SelectedItem.Value) == 2)
                sn.Message.MessageDirectionId = Convert.ToInt32(VLF.CLS.Def.Enums.TxtMsgDirectionType.Out);

        }

      


        private void SetHours()
        {
            if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
                sn.Message.FromHours = this.cboHoursFrom.SelectedItem.Value + ":00 AM";

            if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
                sn.Message.FromHours = this.cboHoursFrom.SelectedItem.Value + ":00 PM";


            if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
                sn.Message.FromHours = Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

            if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
                sn.Message.ToHours = this.cboHoursTo.SelectedItem.Value + ":00 AM";

            if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
                sn.Message.ToHours = this.cboHoursTo.SelectedItem.Value + ":00 PM";


            if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
                sn.Message.ToHours = Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";
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

        protected void dgMessages_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            dgMessages.CurrentPageIndex = e.NewPageIndex;
            dgMessages_Fill_NewTZ();
            dgMessages.SelectedIndex = -1;
        }

        protected void cboVehicle_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }

    }
}
