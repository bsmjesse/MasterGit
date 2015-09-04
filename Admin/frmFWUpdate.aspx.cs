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

namespace SentinelFM.Admin
{
	/// <summary>
	/// Summary description for frmFWUpdate.
	/// </summary>
   public partial class frmFWUpdate : System.Web.UI.Page
   {
      protected SentinelFMSession sn = null;
      protected clsUtility objUtil;
      protected DataSet dsFirmware = new DataSet();

      protected void Page_Load(object sender, System.EventArgs e)
      {
         sn = (SentinelFMSession)Session["SentinelFMSession"];
         if ((sn == null) || (sn.UserName == ""))
         {

            Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
            return;
         }

         if (!Page.IsPostBack)
         {

            string strOrgId = Request.QueryString["OrgId"];
            string strFleetId = Request.QueryString["FleetId"];
            string strVehicleId = Request.QueryString["VehicleId"];

            this.tblFW.Visible = false;
            CboFirmware_Fill();
            cboOrganization_Fill();
            int OrgId = sn.User.OrganizationId;
            //				if (strOrgId!=null)
            //				{
            cboOrganization.SelectedIndex = -1;
            cboOrganization.Items.FindByValue(sn.User.OrganizationId.ToString()).Selected = true;
            this.cboOrganization.Enabled = false;
            //				}
            //				else
            //				{
            //					cboOrganization.Items.Insert(0, new ListItem("Please select an Organization",  "-1"));
            //				}

            CboFleet_Fill();
            if ((strFleetId != null) && (strFleetId != "-1"))
            {
               cboFleet.SelectedIndex = -1;
               cboFleet.Items.FindByValue(strFleetId.ToString()).Selected = true;

               CboVehicle_Fill(Convert.ToInt32(strFleetId));
            }
            else
            {
               //cboFleet.Items.Clear(); 
               cboFleet.Items.Insert(0, new ListItem("Please Select a Fleet", "-1"));
            }


            if ((strVehicleId != null) && (strVehicleId != "-1"))
            {
               cboVehicle.SelectedIndex = -1;
               cboVehicle.Items.FindByValue(strVehicleId.ToString()).Selected = true;
            }
            else
            {
               cboVehicle.Visible = false;
               cboVehicle.Items.Clear();
               cboVehicle.Items.Insert(0, new ListItem("Please Select a Vechicle", "-1"));
            }

            if (strOrgId != null)
            {
               ShowData();
               this.tblFW.Visible = true;
            }


         }

      }


      private void cboOrganization_Fill()
      {

         StringReader strrXML = null;
         DataSet ds = new DataSet();
         objUtil = new clsUtility(sn);
         string xml = "";

         ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

         if (objUtil.ErrCheck(dbo.GetAllOrganizationsInfoXML(sn.UserID, sn.SecId, ref xml), false))
            if (objUtil.ErrCheck(dbo.GetAllOrganizationsInfoXML(sn.UserID, sn.SecId, ref xml), true))
            {
               return;
            }

         if (xml == "")
         {
            return;
         }

         strrXML = new StringReader(xml);
         ds.ReadXml(strrXML);
         this.cboOrganization.DataSource = ds;
         this.cboOrganization.DataBind();



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
         this.dgData.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgData_PageIndexChanged);
         this.dgData.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgData_CancelCommand);
         this.dgData.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgData_EditCommand);
         this.dgData.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgData_UpdateCommand);

      }
      #endregion

      protected void cboOrganization_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (Convert.ToInt32(cboOrganization.SelectedItem.Value) != 0)
         {
            CboFleet_Fill();

            this.lblFleets.Visible = true;
            this.lblVehicles.Visible = false;
            this.cboVehicle.Visible = false;
            this.tblFW.Visible = false;
            this.lblMessage.Text = "";
         }
      }


      private void CboFleet_Fill()
      {

         objUtil = new clsUtility(sn);
         DataSet dsFleets = new DataSet();
         StringReader strrXML = null;




         string xml = "";
         ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

         if (objUtil.ErrCheck(dbo.GetFleetsInfoByOrganizationId(sn.UserID, sn.SecId, Convert.ToInt32(this.cboOrganization.SelectedItem.Value), ref xml), false))
            if (objUtil.ErrCheck(dbo.GetFleetsInfoByOrganizationId(sn.UserID, sn.SecId, Convert.ToInt32(this.cboOrganization.SelectedItem.Value), ref xml), true))
            {

               cboFleet.Items.Clear();
               cboFleet.Items.Insert(0, new ListItem("Please Select a Fleet", "-1"));
               return;
            }


         if (xml == "")
         {
            cboFleet.Items.Clear();
            cboFleet.Items.Insert(0, new ListItem("Please Select a Fleet", "-1"));

            return;
         }

         strrXML = new StringReader(xml);
         dsFleets.ReadXml(strrXML);

         cboFleet.DataSource = dsFleets;
         cboFleet.DataBind();
      }


      private void CboVehicle_Fill(int fleetId)
      {
         DataSet dsVehicle;
         dsVehicle = new DataSet();
         objUtil = new clsUtility(sn);
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

      }

      protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (Convert.ToInt32(cboFleet.SelectedItem.Value) != 0)
         {

            cboVehicle.Items.Clear();
            CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
            cboVehicle.Items.Insert(0, new ListItem("Please select a Vehicle", "-1"));
            cboVehicle.SelectedIndex = 0;
            this.lblFleets.Visible = true;
            this.lblVehicles.Visible = true;
            this.cboVehicle.Visible = true;
            this.tblFW.Visible = false;
            this.lblMessage.Text = "";
         }
      }



      private void CboFirmware_Fill()
      {
         cboFirmware.Items.Clear();
         DsFirmware_Fill();
         cboFirmware.Items.Insert(0, new ListItem("--", "-1"));
         cboFirmware.DataSource = dsFirmware;
         cboFirmware.DataBind();
      }



      private void DsFirmware_Fill()
      {

         objUtil = new clsUtility(sn);
         StringReader strrXML = null;


         string xml = "";
         ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();

         if (objUtil.ErrCheck(dbs.GetAllFirmwareInfo(sn.UserID, sn.SecId, ref xml), false))
            if (objUtil.ErrCheck(dbs.GetAllFirmwareInfo(sn.UserID, sn.SecId, ref xml), true))
            {
               return;
            }
         if (xml == "")
         {
            return;
         }

         strrXML = new StringReader(xml);
         dsFirmware.ReadXml(strrXML);
         sn.Admin.DsFirmware = dsFirmware; 

      }

      protected void cmdView_Click(object sender, System.EventArgs e)
      {
         ShowData();
      }



      public int GetFirmware(int FirmwareId)
      {

         DropDownList cboNewFirmware = new DropDownList();
         cboNewFirmware.DataValueField = "FwId";
         cboNewFirmware.DataTextField = "FwName";
         CboFirmware_Fill();
         cboNewFirmware.DataSource = dsFirmware;
         cboNewFirmware.DataBind();

         cboNewFirmware.SelectedIndex = -1;
         if (FirmwareId != -1)
            cboNewFirmware.Items.FindByValue(FirmwareId.ToString()).Selected = true;

         return cboNewFirmware.SelectedIndex;

      }

      private void dgData_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
      {
         SaveCheckBoxes();
         dgData.EditItemIndex = Convert.ToInt32(e.Item.ItemIndex);
         dgData.DataSource = sn.Admin.DsConfigFirmaware;
         dgData.DataBind();
         dgData.SelectedIndex = -1;

      }

      private void dgData_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
      {

         DropDownList NewFrameware;
         Int32 BoxId = Convert.ToInt32(dgData.DataKeys[e.Item.ItemIndex]);
         NewFrameware = (DropDownList)e.Item.FindControl("cboNewFirmware");
         CheckBox chkSchedule = (CheckBox)e.Item.FindControl("chkBoxScheduled");

         for (int i = 0; i < dgData.Items.Count; i++)
         {
            foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
            {
               if (BoxId.ToString() == rowItem["BoxId"].ToString())
               {
                  rowItem["chkBox"] = true;
                  rowItem["chkBoxScheduled"] = chkSchedule.Checked ;
                  rowItem["NewFirmwareId"] = NewFrameware.SelectedItem.Value;
                  rowItem["NewFirmware"] = NewFrameware.SelectedItem.Text;



                  dgData.EditItemIndex = -1;
                  this.dgData.DataSource = sn.Admin.DsConfigFirmaware;
                  this.dgData.DataBind();

                  return;
               }
            }
         }

      }



      private void SaveCheckBoxes()
      {
           foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
            {
                  rowItem["chkBox"] =false;
                  rowItem["chkBoxScheduled"] = false ;
            }

         for (int i = 0; i < dgData.Items.Count; i++)
         {
            CheckBox ch = (CheckBox)(dgData.Items[i].Cells[1].Controls[1]);
            CheckBox chSched = (CheckBox)(dgData.Items[i].Cells[6].Controls[1]);

            foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
            {
               if (dgData.Items[i].Cells[0].Text.ToString() == rowItem["BoxId"].ToString())
               {
                  rowItem["chkBox"] = ch.Checked;
                  rowItem["chkBoxScheduled"] = chSched.Checked;
                  break; 
               }
            }
         }

      }

      private void dgData_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
      {
         dgData.EditItemIndex = -1;
         this.dgData.DataSource = sn.Admin.DsConfigFirmaware;
         this.dgData.DataBind();
      }

      protected void cboVehicle_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (Convert.ToInt32(cboVehicle.SelectedItem.Value) != -1)
         {
            this.tblFW.Visible = false;
            this.lblMessage.Text = "";
         }
      }

      private void dgData_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
      {
         SaveCheckBoxes();
         dgData.CurrentPageIndex = e.NewPageIndex;
         dgData.DataSource = sn.Admin.DsConfigFirmaware;
         dgData.DataBind();
         dgData.SelectedIndex = -1;

      }

      protected void cboFirmware_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         this.lblMessage.Text = "";
         SaveCheckBoxes();

         foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
         {

            if (Convert.ToBoolean(rowItem["chkBox"]))
            {

               rowItem["NewFirmwareId"] = this.cboFirmware.SelectedItem.Value;
               rowItem["NewFirmware"] = cboFirmware.SelectedItem.Text;

            }
         }

         this.dgData.DataSource = sn.Admin.DsConfigFirmaware;
         this.dgData.DataBind();

      }

      protected void cmdUpdate_Click(object sender, System.EventArgs e)
      {
         try
         {
            SaveCheckBoxes();
            this.lblMessage.Text = "";

            foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
            {
               if (Convert.ToBoolean(rowItem["chkBox"]) && (rowItem["NewFirmwareId"].ToString() != "-1"))
               {


                  Int64 sessionTimeOut = 0;
                  short ProtocolId = (short)VLF.CLS.Def.Enums.ProtocolTypes.OTAv10;
                  short CommModeId = Convert.ToInt16(rowItem["CommModeId"]);
                  string paramList = "";
                  paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyFirmwareIdOld, rowItem["FwId"].ToString());
                  paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyFirmwareIdNew, rowItem["NewFirmwareId"].ToString());

                  bool cmdSent = false;
                  LocationMgr.Location dbl = new LocationMgr.Location();
                  objUtil = new clsUtility(sn);


                  Int32 SheduledInterval = 0;
                  Int64 ScheduledPeriod = 0;


                  SheduledInterval = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["ScheduledInterval"]);
                  ScheduledPeriod = Convert.ToInt64(System.Configuration.ConfigurationSettings.AppSettings["ScheduledPeriod"]);

                  if (!Convert.ToBoolean(rowItem["chkBoxScheduled"]))
                  {
                     if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UploadFirmwareStatus), paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false))
                        if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UploadFirmwareStatus), paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true))
                        {

                           this.lblMessage.Text = "Update FW failed.";
                           return;
                        }
                  }
                  else
                  {


                     Int64 taskId = 0;
                     if (objUtil.ErrCheck(dbl.SendAutomaticCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UploadFirmwareStatus), DateTime.Now, paramList, ref ProtocolId, ref CommModeId, ScheduledPeriod, SheduledInterval, false, ref taskId), false))
                        if (objUtil.ErrCheck(dbl.SendAutomaticCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UploadFirmwareStatus), DateTime.Now, paramList, ref ProtocolId, ref CommModeId, ScheduledPeriod, SheduledInterval, false, ref taskId), false))
                        {

                           this.lblMessage.Text = "Update FW failed.";
                           return;
                        }

                     this.lblMessage.Text = "Update FW was scheduled successfully.";

                  }


                  if (cmdSent)
                     this.lblMessage.Text = "Update FW was sent successfully.";


               }
            }
         }
         catch (Exception ex)
         {
            this.lblMessage.Text = ex.ToString();
         }


      }

      protected void dgData_SelectedIndexChanged(object sender, System.EventArgs e)
      {

      }

      protected void cmdSetAllSensors_Click(object sender, System.EventArgs e)
      {
          if (txtFirmwareFilter.Text.Trim() != "")
          {

              for (int i = 0; i < dgData.Items.Count; i++)
              {
                  foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
                  {
                      if (dgData.Items[i].Cells[0].Text.ToString() == rowItem["BoxId"].ToString())
                      {
                          rowItem["chkBox"] = true;
                          rowItem["chkBoxScheduled"] = true;
                          break; 
                      }
                  }
              }

              FilterRows();
          }
          else
          {

              foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
                  rowItem["chkBox"] = true;

              this.dgData.DataSource = sn.Admin.DsConfigFirmaware;
              this.dgData.DataBind();

          }

         

      }

      protected void cmdUnselectAllSensors_Click(object sender, System.EventArgs e)
      {
          foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
          {
              rowItem["chkBox"] = false;
              rowItem["chkBoxScheduled"] = false;
          }

         this.dgData.DataSource = sn.Admin.DsConfigFirmaware;
         this.dgData.DataBind();
      }

      protected void cmdViewResults_Click(object sender, System.EventArgs e)
      {
         Response.Redirect("frmFWResults.aspx?OrgId=" + this.cboOrganization.SelectedItem.Value + "&FleetId=" + this.cboFleet.SelectedItem.Value + "&VehicleId=" + this.cboVehicle.SelectedItem.Value);
      }

      private void ShowData()
      {

         DataSet ds;
         ds = new DataSet();
         objUtil = new clsUtility(sn);
         StringReader strrXML = null;

         string xml = "";



         if (cboVehicle.Items.Count > 0 && Convert.ToInt32(cboVehicle.SelectedItem.Value) != -1)
         {
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            if (objUtil.ErrCheck(dbv.GetActiveVehicleCfgInfo(sn.UserID, sn.SecId, Convert.ToInt64(this.cboVehicle.SelectedItem.Value), ref xml), false))
               if (objUtil.ErrCheck(dbv.GetActiveVehicleCfgInfo(sn.UserID, sn.SecId, Convert.ToInt64(this.cboVehicle.SelectedItem.Value), ref xml), true))
               {
                  return;
               }
         }
         else if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
         {
            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

            if (objUtil.ErrCheck(dbf.GetFleetAllActiveVehiclesCfgInfo(sn.UserID, sn.SecId, Convert.ToInt32(this.cboFleet.SelectedItem.Value), ref xml), false))
               if (objUtil.ErrCheck(dbf.GetFleetAllActiveVehiclesCfgInfo(sn.UserID, sn.SecId, Convert.ToInt32(this.cboFleet.SelectedItem.Value), ref xml), false))
               {
                  return;
               }
         }



         if (xml == "")
         {
            this.tblFW.Visible = false;
            return;
         }

         strrXML = new StringReader(xml);
         ds.ReadXml(strrXML);



         // Show Combobox
         DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
         dc.DefaultValue = false;
         ds.Tables[0].Columns.Add(dc);


         // Show Combobox
         dc = new DataColumn("chkBoxScheduled", Type.GetType("System.Boolean"));
         dc.DefaultValue = false;
         ds.Tables[0].Columns.Add(dc);


         // Command Status (Update Position)
         dc = new DataColumn("NewFirmwareId", Type.GetType("System.Int16"));
         dc.DefaultValue = -1;
         ds.Tables[0].Columns.Add(dc);


         dc = new DataColumn("NewFirmware", Type.GetType("System.String"));
         dc.DefaultValue = "-";
         ds.Tables[0].Columns.Add(dc);


         // Command Status 
         dc = new DataColumn("BoxFirmwareStatus", Type.GetType("System.String"));
         dc.DefaultValue = "Unknown";
         ds.Tables[0].Columns.Add(dc);



         dc = new DataColumn("BoxFirmware", Type.GetType("System.String"));
         dc.DefaultValue = "Unknown";
         ds.Tables[0].Columns.Add(dc);



         dc = new DataColumn("ProtocolTypeId", Type.GetType("System.Int32"));
         dc.DefaultValue = -1;
         ds.Tables[0].Columns.Add(dc);


         dc = new DataColumn("DateTimeSent", Type.GetType("System.DateTime"));
         ds.Tables[0].Columns.Add(dc);


         dc = new DataColumn("DateTimeReceived", Type.GetType("System.DateTime"));
         ds.Tables[0].Columns.Add(dc);

         sn.Admin.DsConfigFirmaware = ds;

         dgData.DataSource = ds;
         dgData.DataBind();
         this.tblFW.Visible = true;


      }

      protected void cmdGetBoxFirmware_Click(object sender, EventArgs e)
      {
         SaveCheckBoxes();
         this.lblMessage.Text = "";

         foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
         {
            if (Convert.ToBoolean(rowItem["chkBox"]))
            {

               Int64 sessionTimeOut = 0;
               short ProtocolId = -1;
               short CommModeId = -1;
               string paramList = "";
               paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyFirmwareIdOld, rowItem["FwId"].ToString());
               paramList += VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyFirmwareIdNew, rowItem["NewFirmwareId"].ToString());

               bool cmdSent = false;
               LocationMgr.Location dbl = new LocationMgr.Location();
               objUtil = new clsUtility(sn);



               if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.GetBoxStatus), paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false))
                  if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.GetBoxStatus), paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true))
                  {

                     this.lblMessage.Text = "Get Box firmware operation failed.";
                     return;
                  }

               if (cmdSent)
               {
                  rowItem["ProtocolTypeId"] = ProtocolId;
                  rowItem["BoxFirmwareStatus"] = "sent";
                  rowItem["DateTimeSent"] = DateTime.Now;  
                  //this.lblMessage.Text = "Get Box FW operation was sent successfully.";
               }
               else
               {
                  rowItem["BoxFirmwareStatus"] = "No communication";
               }

            }
         }

         dgData.DataSource = sn.Admin.DsConfigFirmaware.Tables[0];
         dgData.DataBind(); 
      }








      // private void GetCommandBoxStatusFromServer(Object data)
      //{

      //   LocationMgr.Location dbl = new LocationMgr.Location();

      //   int cmdStatus = (int)CommandStatus.Sent;

      //   do
      //   {

      //      Thread.Sleep(2000);

      //      //Get Command Status for GPRS

      //          foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
      //          {
      //              if (Convert.ToBoolean(rowItem["chkBox"]))
      //              {

      //      if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt32(rowItem["ProtocolTypeId"]), ref cmdStatus), false))
      //         if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt32(rowItem["ProtocolTypeId"]), ref cmdStatus), true))
      //         {
      //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Sent command failed.  User:" + sn.UserID.ToString() + " Form:frmReefer.aspx"));
      //            break;
      //         }

      //   } while ((CommandStatus)cmdStatus == CommandStatus.Sent);

      //   sn.Cmd.Status = (CommandStatus)cmdStatus;

      //   (data as AutoResetEvent).Set();

      //          }     










      protected void cmdRefreshBoxFirmware_Click(object sender, EventArgs e)
      {
          SaveCheckBoxes();
         objUtil = new clsUtility(sn);
         DataSet dsStatus = new DataSet();
         StringReader strrXML = null;

         string xml = "";
         ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
         bool isExtendedVersion = true ; //GEN 6 - false/XS - true


         foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
         {
            //if (Convert.ToBoolean(rowItem["chkBox"]) && rowItem["BoxFirmwareStatus"] == "sent" && rowItem["BoxFirmware"] == "Unknown")
           // if (Convert.ToBoolean(rowItem["chkBox"]) && rowItem["BoxFirmware"].ToString()  == "Unknown")
             if (Convert.ToBoolean(rowItem["chkBox"]))
            {
               if (objUtil.ErrCheck(dbv.GetLastBoxStatusFromHistoryByBoxIdExtended(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), ref xml, isExtendedVersion), false))
                  if (objUtil.ErrCheck(dbv.GetLastBoxStatusFromHistoryByBoxIdExtended(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), ref xml, isExtendedVersion), true))
                  {
                     continue;
                  }


               if (xml == "")
               {
                  continue;
               }
               dsStatus = new DataSet();
               strrXML = new StringReader(xml);
               dsStatus.ReadXml(strrXML);

               string CustomProp = dsStatus.Tables[0].Rows[0]["CustomProp"].ToString();
               rowItem["BoxFirmware"] = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyFirmwareVersion, CustomProp);


               if (rowItem["DateTimeSent"].ToString() != "" && (Convert.ToDateTime(rowItem["DateTimeSent"]) < Convert.ToDateTime(dsStatus.Tables[0].Rows[0]["DateTimeReceived"]).AddHours(sn.User.TimeZone + sn.User.DayLightSaving).AddMinutes(10)))
               {
                   rowItem["BoxFirmwareStatus"] = "from Box";
                   //DataRow[] retResult = sn.Admin.DsFirmware.Tables[0].Select("FwVersion='" + rowItem["BoxFirmware"] + "'");
                   //if (retResult != null && retResult.Length != 0)
                   //    rowItem["BoxFirmware"] = retResult[0]["FwVersion"];
               }
               else
               {
                   rowItem["BoxFirmwareStatus"] = "from Database";
               }

               rowItem["DateTimeReceived"] = dsStatus.Tables[0].Rows[0]["DateTimeReceived"].ToString();
            }
         }

         dgData.DataSource = sn.Admin.DsConfigFirmaware.Tables[0];
         dgData.DataBind(); 
      }

      protected void cmdFilterRecords_Click(object sender, EventArgs e)
      {
          FilterRows();
      }
      protected void cmdClearFilter_Click(object sender, EventArgs e)
      {
          this.dgData.DataSource = sn.Admin.DsConfigFirmaware;
          this.dgData.DataBind();
          this.txtFirmwareFilter.Text = "";  
      }

      private void FilterRows()
      {
          DataRow[] drCollections = null;
          if (sn.Admin.DsConfigFirmaware == null || sn.Admin.DsConfigFirmaware.Tables.Count == 0)
          {
              this.lblMessage.Visible = true;
              this.lblMessage.Text = "Load vehicles!";
              return;
          }

          if (txtFirmwareFilter.Text.Trim() == "")
              return;

          DataTable dt = sn.Admin.DsConfigFirmaware.Tables[0].Clone();

          dgData.CurrentPageIndex = 0;
          string filter = String.Format("FwName like '%{0}%'", this.txtFirmwareFilter.Text.Replace("'", "''"));


          drCollections = sn.Admin.DsConfigFirmaware.Tables[0].Select(filter);
          if (drCollections != null && drCollections.Length > 0)
          {
              foreach (DataRow dr in drCollections)
                  dt.ImportRow(dr);
          }

          this.dgData.DataSource = dt;
          this.dgData.DataBind();
      }
}

   }