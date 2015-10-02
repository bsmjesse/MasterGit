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
   public partial class Maintenance_frmMaintenanceComplete : SentinelFMBasePage
   {
      private ServiceType _serviceType;
 
      protected void Page_Load(object sender, EventArgs e)
      {
         try
         {
            string VehicleId = Request.QueryString["VehicleID"];
            string typeid = Request.QueryString["TypeId"];

            if (!String.IsNullOrEmpty(typeid))
               VehicleServiceType = (ServiceType)Convert.ToInt16(typeid);

            if (!Page.IsPostBack)
            {
               LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

               if (!String.IsNullOrEmpty(VehicleId))
                  VehicleMaintenanceInfoLoad(Convert.ToInt64(VehicleId));
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, 
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            return;
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            return;
         }
      }

      private void VehicleMaintenanceInfoLoad(Int64 VehicleId)
      {
         DataSet dsv = new DataSet();

         try
         {
            string xml = "";
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            if (objUtil.ErrCheck(dbv.GetVehicleMaintenanceInfoXML(sn.UserID, sn.SecId, VehicleId, ref xml), false))
               if (objUtil.ErrCheck(dbv.GetVehicleMaintenanceInfoXML(sn.UserID, sn.SecId, VehicleId, ref xml), true))
               {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                     " User:" + sn.UserID.ToString() + " Form:frmMaintenanceComplete.aspx->GetVehicleMaintenanceInfoXML error"));
                  return;
               }

            dsv.ReadXml(new StringReader(xml));

            if (VLF.CLS.Util.IsDataSetValid(dsv))
            {
               DataRow drMaintenance = dsv.Tables[0].Rows[0];
               // store vehicle id
               this.lblVehicleId.Text = drMaintenance["VehicleId"].ToString();

               # region Store Odometer data into lables

               if ((drMaintenance["MaxSrvInterval"].ToString() != "0") && (drMaintenance["MaxSrvInterval"].ToString() != ""))
               {
                  this.lblOdometer.Text = drMaintenance["CurrentOdo"].ToString();
               }
               else
                  this.lblOdometer.Text = "0";

               if ((drMaintenance["CurrentOdo"].ToString() != "0") && (drMaintenance["CurrentOdo"].ToString() != ""))
               {
                  this.lblLastSrvOdo.Text = drMaintenance["CurrentOdo"].ToString();
               }
               else
                  this.lblLastSrvOdo.Text = "0";

               # endregion

               # region Store Engine hours data into labels

               if ((drMaintenance["EngHrsSrvInterval"].ToString() != "0") && (drMaintenance["EngHrsSrvInterval"].ToString() != ""))
               {
                  this.lblEngineHours.Text = drMaintenance["CurrentEngHrs"].ToString();
               }
               else
                  this.lblEngineHours.Text = "0";

               if ((drMaintenance["CurrentEngHrs"].ToString() != "0") && (drMaintenance["CurrentEngHrs"].ToString() != ""))
               {
                  this.lblLastEngineHours.Text = drMaintenance["CurrentEngHrs"].ToString();
               }
               else
                  this.lblLastEngineHours.Text = "0";

               # endregion

               cboServiceFill(Convert.ToInt16(drMaintenance["VehicleTypeId"]));

               this.txtThisService.Text = drMaintenance["NextServiceDescription"].ToString().TrimEnd();
               this.txtNextService.Text = "";
            }
            else
            {
               cboServiceFill(Convert.ToInt16(VLF.CLS.Def.Enums.VehicleType.Car));
               this.lblCurOdom.Text = "0";
               this.txtNextService.Text = "";
               this.txtThisService.Text = "";
            }
         }

         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      protected void cmdSave_Click(object sender, EventArgs e)
      {
         try
         {           
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
            //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = ci;

            if (VehicleServiceType == ServiceType.EngineHours)
            {
               if (objUtil.ErrCheck(dbv.AddVehicleEngineMaintenanceHst(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), DateTime.Now,
                 this.txtThisService.Text, Convert.ToInt32(this.lblEngineHours.Text)), false))
                  if (objUtil.ErrCheck(dbv.AddVehicleEngineMaintenanceHst(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), DateTime.Now,
                     this.txtThisService.Text, Convert.ToInt32(this.lblEngineHours.Text)), true))
                  {
                     this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateMaintenanceFailed");
                     return;
                  }

               if (objUtil.ErrCheck(dbv.UpdateVehicleEngineMaintenanceShortInfo(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text),
                  Convert.ToInt32(this.lblLastEngineHours.Text), this.txtNextService.Text), false))
                  if (objUtil.ErrCheck(dbv.UpdateVehicleEngineMaintenanceShortInfo(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text),
                     Convert.ToInt32(this.lblLastEngineHours.Text), this.txtNextService.Text), true))
                  {
                     this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateMaintenanceFailed");
                     return;
                  }
            }
            else
            {
               if (objUtil.ErrCheck(dbv.AddVehicleMaintenanceHst(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), DateTime.Now,
                  this.txtThisService.Text, Convert.ToDouble(this.lblOdometer.Text)), false))
                  if (objUtil.ErrCheck(dbv.AddVehicleMaintenanceHst(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), DateTime.Now,
                     this.txtThisService.Text, Convert.ToDouble(this.lblOdometer.Text)), true))
                  {
                     this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateMaintenanceFailed");
                     return;
                  }

               if (objUtil.ErrCheck(dbv.UpdateVehicleMaintenanceShortInfo(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text),
                  Convert.ToDouble(this.lblLastSrvOdo.Text), this.txtNextService.Text), false))
                  if (objUtil.ErrCheck(dbv.UpdateVehicleMaintenanceShortInfo(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text),
                     Convert.ToDouble(this.lblLastSrvOdo.Text), this.txtNextService.Text), true))
                  {
                     this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateMaintenanceFailed");
                     return;
                  }
            }

            Response.Write("<script language='javascript'>window.opener.location.href='frmMaintenance.aspx';window.close()</script>");
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_UpdateMaintenanceFailed");
            return;
         }
      }

      protected void cmdAddThisService_Click(object sender, EventArgs e)
      {
         if (this.cboServiceThis.SelectedItem.Value == "-1")
            return;

         this.txtThisService.Text = this.txtThisService.Text + Convert.ToChar(13) + Convert.ToChar(10) + this.cboServiceThis.SelectedItem.Text;
      }

      protected void cmdAddNextService_Click(object sender, EventArgs e)
      {
         if (this.cboServiceNext.SelectedItem.Value == "-1")
            return;

         this.txtNextService.Text = this.txtNextService.Text + Convert.ToChar(13) + Convert.ToChar(10) + this.cboServiceNext.SelectedItem.Text;
      }

      /// <summary>
      /// Fill services combo according to the vehicle type
      /// </summary>
      /// <param name="VehicleTypeId"></param>
      private void cboServiceFill(Int16 VehicleTypeId)
      {
         if (VehicleTypeId == Convert.ToInt16(VLF.CLS.Def.Enums.VehicleType.Trailer))
         {
            cboServiceNext.Items.Clear();
            cboServiceNext.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_0"), "-1"));
            cboServiceNext.Items.Insert(1, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_1"), "Tires - Tread Depth Recording"));
            cboServiceNext.Items.Insert(2, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_2"), "Tires - Pressure Adjust"));
            cboServiceNext.Items.Insert(3, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_3"), "Brakes - Adjust (manual slack adjuster)"));
            cboServiceNext.Items.Insert(4, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_4"), "Brakes - Inspect/Adjust(Automatic slack adjuster)"));
            cboServiceNext.Items.Insert(5, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_5"), "Brakes - Lining Measure"));
            cboServiceNext.Items.Insert(6, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_6"), "Brakes - Drums/Rotor Measure"));
            cboServiceNext.Items.Insert(7, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_7"), "Brakes - Hoses Inspect"));
            cboServiceNext.Items.Insert(8, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_8"), "Brakes - Antilock System Check"));
            cboServiceNext.Items.Insert(9, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_9"), "Air System - Test for Leaks"));
            cboServiceNext.Items.Insert(10, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_10"), "Air System - Test Pressure Build Time to 60psi"));
            cboServiceNext.Items.Insert(11, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_11"), "Air System - Test Pressure Build Time to 90psi"));
            cboServiceNext.Items.Insert(12, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_12"), "Suspension - Inspect"));
            cboServiceNext.Items.Insert(13, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_13"), "Suspension - Adjust Ride Height(air ride)"));
            cboServiceNext.Items.Insert(14, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_14"), "Doors - Barn - Check Hinges"));
            cboServiceNext.Items.Insert(15, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_15"), "Doors - Barn - Customs Regulations"));
            cboServiceNext.Items.Insert(16, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_16"), "Doors - Roll-up - Check Tracks and Mechanism"));
            cboServiceNext.Items.Insert(17, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_17"), "General Box/Roof Inspect"));
            cboServiceNext.Items.Insert(18, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_18"), "Lighting - Check All"));
            cboServiceNext.Items.Insert(19, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_19"), "Lighting - Inspect Trailer Cord"));
            cboServiceNext.Items.Insert(20, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_20"), "Refrigeration Unit - Follow PM"));
            //cboServiceNext.Items.Insert(0, new ListItem("Select a Service", "-1"));
            //cboServiceNext.Items.Insert(1, new ListItem("Tires - Tread Depth Recording", "Tires - Tread Depth Recording"));
            //cboServiceNext.Items.Insert(2, new ListItem("Tires - Pressure Adjust", "Tires - Pressure Adjust"));
            //cboServiceNext.Items.Insert(3, new ListItem("Brakes - Adjust (manual slack adjuster)", "Brakes - Adjust (manual slack adjuster)"));
            //cboServiceNext.Items.Insert(4, new ListItem("Brakes - Inspect/Adjust(Automatic slack adjuster)", "Brakes - Inspect/Adjust(Automatic slack adjuster)"));
            //cboServiceNext.Items.Insert(5, new ListItem("Brakes - Lining Measure", "Brakes - Lining Measure"));
            //cboServiceNext.Items.Insert(6, new ListItem("Brakes - Drums/Rotor Measure", "Brakes - Drums/Rotor Measure"));
            //cboServiceNext.Items.Insert(7, new ListItem("Brakes - Hoses Inspect", "Brakes - Hoses Inspect"));
            //cboServiceNext.Items.Insert(8, new ListItem("Brakes - Antilock System Check", "Brakes - Antilock System Check"));
            //cboServiceNext.Items.Insert(9, new ListItem("Air System - Test for Leaks", "Air System - Test for Leaks"));
            //cboServiceNext.Items.Insert(10, new ListItem("Air System - Test Pressure Build Time to 60psi", "Air System - Test Pressure Build Time to 60psi"));
            //cboServiceNext.Items.Insert(11, new ListItem("Air System - Test Pressure Build Time to 90psi", "Air System - Test Pressure Build Time to 90psi"));
            //cboServiceNext.Items.Insert(12, new ListItem("Suspension - Inspect", "Suspension - Inspect"));
            //cboServiceNext.Items.Insert(13, new ListItem("Suspension - Adjust Ride Height(air ride)", "Suspension - Adjust Ride Height(air ride)"));
            //cboServiceNext.Items.Insert(14, new ListItem("Doors - Barn - Check Hinges", "Doors - Barn - Check Hinges"));
            //cboServiceNext.Items.Insert(15, new ListItem("Doors - Barn - Customs Regulations", "Doors - Barn - Customs Regulations"));
            //cboServiceNext.Items.Insert(16, new ListItem("Doors - Roll-up - Check Tracks and Mechanism", "Doors - Roll-up - Check Tracks and Mechanism"));
            //cboServiceNext.Items.Insert(17, new ListItem("General Box/Roof Inspect", "General Box/Roof Inspect"));
            //cboServiceNext.Items.Insert(18, new ListItem("Lighting - Check All", "Lighting - Check All"));
            //cboServiceNext.Items.Insert(19, new ListItem("Lighting - Inspect Trailer Cord", "Lighting - Inspect Trailer Cord"));
            //cboServiceNext.Items.Insert(20, new ListItem("Refrigeration Unit - Follow PM", "Refrigeration Unit - Follow PM"));

            cboServiceThis.Items.Clear();
            cboServiceThis.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_0"), "-1"));
            cboServiceThis.Items.Insert(1, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_1"), "Tires - Tread Depth Recording"));
            cboServiceThis.Items.Insert(2, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_2"), "Tires - Pressure Adjust"));
            cboServiceThis.Items.Insert(3, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_3"), "Brakes - Adjust (manual slack adjuster)"));
            cboServiceThis.Items.Insert(4, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_4"), "Brakes - Inspect/Adjust(Automatic slack adjuster)"));
            cboServiceThis.Items.Insert(5, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_5"), "Brakes - Lining Measure"));
            cboServiceThis.Items.Insert(6, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_6"), "Brakes - Drums/Rotor Measure"));
            cboServiceThis.Items.Insert(7, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_7"), "Brakes - Hoses Inspect"));
            cboServiceThis.Items.Insert(8, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_8"), "Brakes - Antilock System Check"));
            cboServiceThis.Items.Insert(9, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_9"), "Air System - Test for Leaks"));
            cboServiceThis.Items.Insert(10, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_10"), "Air System - Test Pressure Build Time to 60psi"));
            cboServiceThis.Items.Insert(11, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_11"), "Air System - Test Pressure Build Time to 90psi"));
            cboServiceThis.Items.Insert(12, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_12"), "Suspension - Inspect"));
            cboServiceThis.Items.Insert(13, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_13"), "Suspension - Adjust Ride Height(air ride)"));
            cboServiceThis.Items.Insert(14, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_14"), "Doors - Barn - Check Hinges"));
            cboServiceThis.Items.Insert(15, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_15"), "Doors - Barn - Customs Regulations"));
            cboServiceThis.Items.Insert(16, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_16"), "Doors - Roll-up - Check Tracks and Mechanism"));
            cboServiceThis.Items.Insert(17, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_17"), "General Box/Roof Inspect"));
            cboServiceThis.Items.Insert(18, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_18"), "Lighting - Check All"));
            cboServiceThis.Items.Insert(19, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_19"), "Lighting - Inspect Trailer Cord"));
            cboServiceThis.Items.Insert(20, new ListItem((string)base.GetLocalResourceObject("cboServiceA_Item_20"), "Refrigeration Unit - Follow PM"));
            //cboServiceThis.Items.Insert(0, new ListItem("Select a Service", "-1"));
            //cboServiceThis.Items.Insert(1, new ListItem("Tires - Tread Depth Recording", "Tires - Tread Depth Recording"));
            //cboServiceThis.Items.Insert(2, new ListItem("Tires - Pressure Adjust", "Tires - Pressure Adjust"));
            //cboServiceThis.Items.Insert(3, new ListItem("Brakes - Adjust (manual slack adjuster)", "Brakes - Adjust (manual slack adjuster)"));
            //cboServiceThis.Items.Insert(4, new ListItem("Brakes - Inspect/Adjust(Automatic slack adjuster)", "Brakes - Inspect/Adjust(Automatic slack adjuster)"));
            //cboServiceThis.Items.Insert(5, new ListItem("Brakes - Lining Measure", "Brakes - Lining Measure"));
            //cboServiceThis.Items.Insert(6, new ListItem("Brakes - Drums/Rotor Measure", "Brakes - Drums/Rotor Measure"));
            //cboServiceThis.Items.Insert(7, new ListItem("Brakes - Hoses Inspect", "Brakes - Hoses Inspect"));
            //cboServiceThis.Items.Insert(8, new ListItem("Brakes - Antilock System Check", "Brakes - Antilock System Check"));
            //cboServiceThis.Items.Insert(9, new ListItem("Air System - Test for Leaks", "Air System - Test for Leaks"));
            //cboServiceThis.Items.Insert(10, new ListItem("Air System - Test Pressure Build Time to 60psi", "Air System - Test Pressure Build Time to 60psi"));
            //cboServiceThis.Items.Insert(11, new ListItem("Air System - Test Pressure Build Time to 90psi", "Air System - Test Pressure Build Time to 90psi"));
            //cboServiceThis.Items.Insert(12, new ListItem("Suspension - Inspect", "Suspension - Inspect"));
            //cboServiceThis.Items.Insert(13, new ListItem("Suspension - Adjust Ride Height(air ride)", "Suspension - Adjust Ride Height(air ride)"));
            //cboServiceThis.Items.Insert(14, new ListItem("Doors - Barn - Check Hinges", "Doors - Barn - Check Hinges"));
            //cboServiceThis.Items.Insert(15, new ListItem("Doors - Barn - Customs Regulations", "Doors - Barn - Customs Regulations"));
            //cboServiceThis.Items.Insert(16, new ListItem("Doors - Roll-up - Check Tracks and Mechanism", "Doors - Roll-up - Check Tracks and Mechanism"));
            //cboServiceThis.Items.Insert(17, new ListItem("General Box/Roof Inspect", "General Box/Roof Inspect"));
            //cboServiceThis.Items.Insert(18, new ListItem("Lighting - Check All", "Lighting - Check All"));
            //cboServiceThis.Items.Insert(19, new ListItem("Lighting - Inspect Trailer Cord", "Lighting - Inspect Trailer Cord"));
            //cboServiceThis.Items.Insert(20, new ListItem("Refrigeration Unit - Follow PM", "Refrigeration Unit - Follow PM"));
         }
         else
         {
            cboServiceNext.Items.Clear();
            cboServiceNext.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_0"), "-1"));
            cboServiceNext.Items.Insert(1, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_1"), "Oil - Change"));
            cboServiceNext.Items.Insert(2, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_2"), "Oil Filter - Change"));
            cboServiceNext.Items.Insert(3, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_3"), "Fuel Filter - Drain Water"));
            cboServiceNext.Items.Insert(4, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_4"), "Fuel Filter - Change"));
            cboServiceNext.Items.Insert(5, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_5"), "Air Filter - Change"));
            cboServiceNext.Items.Insert(6, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_6"), "Engine Coolant - Inspect Freeze Point & PH"));
            cboServiceNext.Items.Insert(7, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_7"), "Engine Coolant - Replace"));
            cboServiceNext.Items.Insert(8, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_8"), "Engine Exhaust - Inspect"));
            cboServiceNext.Items.Insert(9, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_9"), "Engine Mechanical - Valve Lash"));
            cboServiceNext.Items.Insert(10, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_10"), "Engine Fan - Fan Drive"));
            cboServiceNext.Items.Insert(11, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_11"), "Engine Fan - Drive Belts"));
            cboServiceNext.Items.Insert(12, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_12"), "Steering - Alignment"));
            cboServiceNext.Items.Insert(13, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_13"), "Steering - Hub Oil Level"));
            cboServiceNext.Items.Insert(14, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_14"), "Steering - Bearing Adjustment"));
            cboServiceNext.Items.Insert(15, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_15"), "Tires - Steer Tire Rotation"));
            cboServiceNext.Items.Insert(16, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_16"), "Tires - Drive Tire Rotation"));
            cboServiceNext.Items.Insert(17, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_17"), "Tires - Tread Depth Recording"));
            cboServiceNext.Items.Insert(18, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_18"), "Tires - Pressure Adjust"));
            cboServiceNext.Items.Insert(19, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_19"), "Brakes - Adjust (manual slack adjuster)"));
            cboServiceNext.Items.Insert(20, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_20"), "Brakes - Inspect/Adjust(Automatic slack adjuster)"));
            cboServiceNext.Items.Insert(21, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_21"), "Brakes - Lining Measure"));
            cboServiceNext.Items.Insert(22, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_22"), "Brakes - Drums/Rotor Measure"));
            cboServiceNext.Items.Insert(23, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_23"), "Brakes - Hoses Inspect"));
            cboServiceNext.Items.Insert(24, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_24"), "Brakes - Antilock System Check"));
            cboServiceNext.Items.Insert(25, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_25"), "Transmission - Oil Level"));
            cboServiceNext.Items.Insert(26, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_26"), "Transmission - Oil Change"));
            cboServiceNext.Items.Insert(27, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_27"), "Transmission - Filter Change"));
            cboServiceNext.Items.Insert(28, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_28"), "Drive Axle - Oil"));
            cboServiceNext.Items.Insert(29, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_29"), "Drive Axle - Filter"));
            cboServiceNext.Items.Insert(30, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_30"), "Air System - Test for Leaks"));
            cboServiceNext.Items.Insert(31, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_31"), "Air System - Test Pressure Build Time to 60psi"));
            cboServiceNext.Items.Insert(32, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_32"), "Air System - Test Pressure Build Time to 90psi"));
            cboServiceNext.Items.Insert(33, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_33"), "Air System - Dryer Change desiccant"));
            cboServiceNext.Items.Insert(34, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_34"), "Air System - Air Lines Inspect"));
            cboServiceNext.Items.Insert(35, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_35"), "Lighting - Check All"));
            cboServiceNext.Items.Insert(36, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_36"), "Lighting - Inspect Trailer Socket"));
            cboServiceNext.Items.Insert(37, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_37"), "Electrical - Alternator Output"));
            cboServiceNext.Items.Insert(38, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_38"), "Electrical - Battery Load Test"));
            cboServiceNext.Items.Insert(39, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_39"), "Electrical - Battery Check Levels"));
            cboServiceNext.Items.Insert(40, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_40"), "Electrical - Starter Draw Test and/or Voltage Drops"));
            cboServiceNext.Items.Insert(41, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_41"), "Fifth Wheel - Lube"));
            cboServiceNext.Items.Insert(42, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_42"), "Fifth Wheel - Adjust"));
            cboServiceNext.Items.Insert(43, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_43"), "Fifth Wheel - Inspect Mounting"));
            cboServiceNext.Items.Insert(44, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_44"), "Suspension - Inspect"));
            cboServiceNext.Items.Insert(45, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_45"), "Suspension - Adjust Ride Height(air ride)"));
            cboServiceNext.Items.Insert(46, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_46"), "Window Glass - Inspect Windshield"));
            //cboServiceNext.Items.Insert(0, new ListItem("Select a Service", "-1"));
            //cboServiceNext.Items.Insert(1, new ListItem("Oil - Change", "Oil - Change"));
            //cboServiceNext.Items.Insert(2, new ListItem("Oil Filter - Change", "Oil Filter - Change"));
            //cboServiceNext.Items.Insert(3, new ListItem("Fuel Filter - Drain Water", "Fuel Filter - Drain Water"));
            //cboServiceNext.Items.Insert(4, new ListItem("Fuel Filter - Change", "Fuel Filter - Change"));
            //cboServiceNext.Items.Insert(5, new ListItem("Air Filter - Change", "Air Filter - Change"));
            //cboServiceNext.Items.Insert(6, new ListItem("Engine Coolant - Inspect Freeze Point & PH", "Engine Coolant - Inspect Freeze Point & PH"));
            //cboServiceNext.Items.Insert(7, new ListItem("Engine Coolant - Replace", "Engine Coolant - Replace"));
            //cboServiceNext.Items.Insert(8, new ListItem("Engine Exhaust - Inspect", "Engine Exhaust - Inspect"));
            //cboServiceNext.Items.Insert(9, new ListItem("Engine Mechanical - Valve Lash", "Engine Mechanical - Valve Lash"));
            //cboServiceNext.Items.Insert(10, new ListItem("Engine Fan - Fan Drive", "Engine Fan - Fan Drive"));
            //cboServiceNext.Items.Insert(11, new ListItem("Engine Fan - Drive Belts", "Engine Fan - Drive Belts"));
            //cboServiceNext.Items.Insert(12, new ListItem("Steering - Alignment", "Steering - Alignment"));
            //cboServiceNext.Items.Insert(13, new ListItem("Steering - Hub Oil Level", "Steering - Hub Oil Level"));
            //cboServiceNext.Items.Insert(14, new ListItem("Steering - Bearing Adjustment", "Steering - Bearing Adjustment"));
            //cboServiceNext.Items.Insert(15, new ListItem("Tires - Steer Tire Rotation", "Tires - Steer Tire Rotation"));
            //cboServiceNext.Items.Insert(16, new ListItem("Tires - Drive Tire Rotation", "Tires - Drive Tire Rotation"));
            //cboServiceNext.Items.Insert(17, new ListItem("Tires - Tread Depth Recording", "Tires - Tread Depth Recording"));
            //cboServiceNext.Items.Insert(18, new ListItem("Tires - Pressure Adjust", "Tires - Pressure Adjust"));
            //cboServiceNext.Items.Insert(19, new ListItem("Brakes - Adjust (manual slack adjuster)", "Brakes - Adjust (manual slack adjuster)"));
            //cboServiceNext.Items.Insert(20, new ListItem("Brakes - Inspect/Adjust(Automatic slack adjuster)", "Brakes - Inspect/Adjust(Automatic slack adjuster)"));
            //cboServiceNext.Items.Insert(21, new ListItem("Brakes - Lining Measure", "Brakes - Lining Measure"));
            //cboServiceNext.Items.Insert(22, new ListItem("Brakes - Drums/Rotor Measure", "Brakes - Drums/Rotor Measure"));
            //cboServiceNext.Items.Insert(23, new ListItem("Brakes - Hoses Inspect", "Brakes - Hoses Inspect"));
            //cboServiceNext.Items.Insert(24, new ListItem("Brakes - Antilock System Check", "Brakes - Antilock System Check"));
            //cboServiceNext.Items.Insert(25, new ListItem("Transmission - Oil Level", "Transmission - Oil Level"));
            //cboServiceNext.Items.Insert(26, new ListItem("Transmission - Oil Change", "Transmission - Oil Change"));
            //cboServiceNext.Items.Insert(27, new ListItem("Transmission - Filter Change", "Transmission - Filter Change"));
            //cboServiceNext.Items.Insert(28, new ListItem("Drive Axle - Oil", "Drive Axle - Oil"));
            //cboServiceNext.Items.Insert(29, new ListItem("Drive Axle - Filter", "Drive Axle - Filter"));
            //cboServiceNext.Items.Insert(30, new ListItem("Air System - Test for Leaks", "Air System - Test for Leaks"));
            //cboServiceNext.Items.Insert(31, new ListItem("Air System - Test Pressure Build Time to 60psi", "Air System - Test Pressure Build Time to 60psi"));
            //cboServiceNext.Items.Insert(32, new ListItem("Air System - Test Pressure Build Time to 90psi", "Air System - Test Pressure Build Time to 90psi"));
            //cboServiceNext.Items.Insert(33, new ListItem("Air System - Dryer Change desiccant", "Air System - Dryer Change desiccant"));
            //cboServiceNext.Items.Insert(34, new ListItem("Air System - Air Lines Inspect", "Air System - Air Lines Inspect"));
            //cboServiceNext.Items.Insert(35, new ListItem("Lighting - Check All", "Lighting - Check All"));
            //cboServiceNext.Items.Insert(36, new ListItem("Lighting - Inspect Trailer Socket", "Lighting - Inspect Trailer Socket"));
            //cboServiceNext.Items.Insert(37, new ListItem("Electrical - Alternator Output", "Electrical - Alternator Output"));
            //cboServiceNext.Items.Insert(38, new ListItem("Electrical - Battery Load Test", "Electrical - Battery Load Test"));
            //cboServiceNext.Items.Insert(39, new ListItem("Electrical - Battery Check Levels", "Electrical - Battery Check Levels"));
            //cboServiceNext.Items.Insert(40, new ListItem("Electrical - Starter Draw Test and/or Voltage Drops", "Electrical - Starter Draw Test and/or Voltage Drops"));
            //cboServiceNext.Items.Insert(41, new ListItem("Fifth Wheel - Lube", "Fifth Wheel - Lube"));
            //cboServiceNext.Items.Insert(42, new ListItem("Fifth Wheel - Adjust", "Fifth Wheel - Adjust"));
            //cboServiceNext.Items.Insert(43, new ListItem("Fifth Wheel - Inspect Mounting", "Fifth Wheel - Inspect Mounting"));
            //cboServiceNext.Items.Insert(44, new ListItem("Suspension - Inspect", "Suspension - Inspect"));
            //cboServiceNext.Items.Insert(45, new ListItem("Suspension - Adjust Ride Height(air ride)", "Suspension - Adjust Ride Height(air ride)"));
            //cboServiceNext.Items.Insert(46, new ListItem("Window Glass - Inspect Windshield", "Window Glass - Inspect Windshield"));

            cboServiceThis.Items.Clear();
            cboServiceThis.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_0"), "-1"));
            cboServiceThis.Items.Insert(1, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_1"), "Oil - Change"));
            cboServiceThis.Items.Insert(2, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_2"), "Oil Filter - Change"));
            cboServiceThis.Items.Insert(3, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_3"), "Fuel Filter - Drain Water"));
            cboServiceThis.Items.Insert(4, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_4"), "Fuel Filter - Change"));
            cboServiceThis.Items.Insert(5, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_5"), "Air Filter - Change"));
            cboServiceThis.Items.Insert(6, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_6"), "Engine Coolant - Inspect Freeze Point & PH"));
            cboServiceThis.Items.Insert(7, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_7"), "Engine Coolant - Replace"));
            cboServiceThis.Items.Insert(8, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_8"), "Engine Exhaust - Inspect"));
            cboServiceThis.Items.Insert(9, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_9"), "Engine Mechanical - Valve Lash"));
            cboServiceThis.Items.Insert(10, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_10"), "Engine Fan - Fan Drive"));
            cboServiceThis.Items.Insert(11, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_11"), "Engine Fan - Drive Belts"));
            cboServiceThis.Items.Insert(12, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_12"), "Steering - Alignment"));
            cboServiceThis.Items.Insert(13, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_13"), "Steering - Hub Oil Level"));
            cboServiceThis.Items.Insert(14, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_14"), "Steering - Bearing Adjustment"));
            cboServiceThis.Items.Insert(15, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_15"), "Tires - Steer Tire Rotation"));
            cboServiceThis.Items.Insert(16, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_16"), "Tires - Drive Tire Rotation"));
            cboServiceThis.Items.Insert(17, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_17"), "Tires - Tread Depth Recording"));
            cboServiceThis.Items.Insert(18, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_18"), "Tires - Pressure Adjust"));
            cboServiceThis.Items.Insert(19, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_19"), "Brakes - Adjust (manual slack adjuster)"));
            cboServiceThis.Items.Insert(20, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_20"), "Brakes - Inspect/Adjust(Automatic slack adjuster)"));
            cboServiceThis.Items.Insert(21, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_21"), "Brakes - Lining Measure"));
            cboServiceThis.Items.Insert(22, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_22"), "Brakes - Drums/Rotor Measure"));
            cboServiceThis.Items.Insert(23, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_23"), "Brakes - Hoses Inspect"));
            cboServiceThis.Items.Insert(24, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_24"), "Brakes - Antilock System Check"));
            cboServiceThis.Items.Insert(25, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_25"), "Transmission - Oil Level"));
            cboServiceThis.Items.Insert(26, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_26"), "Transmission - Oil Change"));
            cboServiceThis.Items.Insert(27, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_27"), "Transmission - Filter Change"));
            cboServiceThis.Items.Insert(28, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_28"), "Drive Axle - Oil"));
            cboServiceThis.Items.Insert(29, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_29"), "Drive Axle - Filter"));
            cboServiceThis.Items.Insert(30, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_30"), "Air System - Test for Leaks"));
            cboServiceThis.Items.Insert(31, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_31"), "Air System - Test Pressure Build Time to 60psi"));
            cboServiceThis.Items.Insert(32, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_32"), "Air System - Test Pressure Build Time to 90psi"));
            cboServiceThis.Items.Insert(33, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_33"), "Air System - Dryer Change desiccant"));
            cboServiceThis.Items.Insert(34, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_34"), "Air System - Air Lines Inspect"));
            cboServiceThis.Items.Insert(35, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_35"), "Lighting - Check All"));
            cboServiceThis.Items.Insert(36, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_36"), "Lighting - Inspect Trailer Socket"));
            cboServiceThis.Items.Insert(37, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_37"), "Electrical - Alternator Output"));
            cboServiceThis.Items.Insert(38, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_38"), "Electrical - Battery Load Test"));
            cboServiceThis.Items.Insert(39, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_39"), "Electrical - Battery Check Levels"));
            cboServiceThis.Items.Insert(40, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_40"), "Electrical - Starter Draw Test and/or Voltage Drops"));
            cboServiceThis.Items.Insert(41, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_41"), "Fifth Wheel - Lube"));
            cboServiceThis.Items.Insert(42, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_42"), "Fifth Wheel - Adjust"));
            cboServiceThis.Items.Insert(43, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_43"), "Fifth Wheel - Inspect Mounting"));
            cboServiceThis.Items.Insert(44, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_44"), "Suspension - Inspect"));
            cboServiceThis.Items.Insert(45, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_45"), "Suspension - Adjust Ride Height(air ride)"));
            cboServiceThis.Items.Insert(46, new ListItem((string)base.GetLocalResourceObject("cboServiceB_Item_46"), "Window Glass - Inspect Windshield"));
            //cboServiceThis.Items.Insert(0, new ListItem("Select a Service", "-1"));
            //cboServiceThis.Items.Insert(1, new ListItem("Oil - Change", "Oil - Change"));
            //cboServiceThis.Items.Insert(2, new ListItem("Oil Filter - Change", "Oil Filter - Change"));
            //cboServiceThis.Items.Insert(3, new ListItem("Fuel Filter - Drain Water", "Fuel Filter - Drain Water"));
            //cboServiceThis.Items.Insert(4, new ListItem("Fuel Filter - Change", "Fuel Filter - Change"));
            //cboServiceThis.Items.Insert(5, new ListItem("Air Filter - Change", "Air Filter - Change"));
            //cboServiceThis.Items.Insert(6, new ListItem("Engine Coolant - Inspect Freeze Point & PH", "Engine Coolant - Inspect Freeze Point & PH"));
            //cboServiceThis.Items.Insert(7, new ListItem("Engine Coolant - Replace", "Engine Coolant - Replace"));
            //cboServiceThis.Items.Insert(8, new ListItem("Engine Exhaust - Inspect", "Engine Exhaust - Inspect"));
            //cboServiceThis.Items.Insert(9, new ListItem("Engine Mechanical - Valve Lash", "Engine Mechanical - Valve Lash"));
            //cboServiceThis.Items.Insert(10, new ListItem("Engine Fan - Fan Drive", "Engine Fan - Fan Drive"));
            //cboServiceThis.Items.Insert(11, new ListItem("Engine Fan - Drive Belts", "Engine Fan - Drive Belts"));
            //cboServiceThis.Items.Insert(12, new ListItem("Steering - Alignment", "Steering - Alignment"));
            //cboServiceThis.Items.Insert(13, new ListItem("Steering - Hub Oil Level", "Steering - Hub Oil Level"));
            //cboServiceThis.Items.Insert(14, new ListItem("Steering - Bearing Adjustment", "Steering - Bearing Adjustment"));
            //cboServiceThis.Items.Insert(15, new ListItem("Tires - Steer Tire Rotation", "Tires - Steer Tire Rotation"));
            //cboServiceThis.Items.Insert(16, new ListItem("Tires - Drive Tire Rotation", "Tires - Drive Tire Rotation"));
            //cboServiceThis.Items.Insert(17, new ListItem("Tires - Tread Depth Recording", "Tires - Tread Depth Recording"));
            //cboServiceThis.Items.Insert(18, new ListItem("Tires - Pressure Adjust", "Tires - Pressure Adjust"));
            //cboServiceThis.Items.Insert(19, new ListItem("Brakes - Adjust (manual slack adjuster)", "Brakes - Adjust (manual slack adjuster)"));
            //cboServiceThis.Items.Insert(20, new ListItem("Brakes - Inspect/Adjust(Automatic slack adjuster)", "Brakes - Inspect/Adjust(Automatic slack adjuster)"));
            //cboServiceThis.Items.Insert(21, new ListItem("Brakes - Lining Measure", "Brakes - Lining Measure"));
            //cboServiceThis.Items.Insert(22, new ListItem("Brakes - Drums/Rotor Measure", "Brakes - Drums/Rotor Measure"));
            //cboServiceThis.Items.Insert(23, new ListItem("Brakes - Hoses Inspect", "Brakes - Hoses Inspect"));
            //cboServiceThis.Items.Insert(24, new ListItem("Brakes - Antilock System Check", "Brakes - Antilock System Check"));
            //cboServiceThis.Items.Insert(25, new ListItem("Transmission - Oil Level", "Transmission - Oil Level"));
            //cboServiceThis.Items.Insert(26, new ListItem("Transmission - Oil Change", "Transmission - Oil Change"));
            //cboServiceThis.Items.Insert(27, new ListItem("Transmission - Filter Change", "Transmission - Filter Change"));
            //cboServiceThis.Items.Insert(28, new ListItem("Drive Axle - Oil", "Drive Axle - Oil"));
            //cboServiceThis.Items.Insert(29, new ListItem("Drive Axle - Filter", "Drive Axle - Filter"));
            //cboServiceThis.Items.Insert(30, new ListItem("Air System - Test for Leaks", "Air System - Test for Leaks"));
            //cboServiceThis.Items.Insert(31, new ListItem("Air System - Test Pressure Build Time to 60psi", "Air System - Test Pressure Build Time to 60psi"));
            //cboServiceThis.Items.Insert(32, new ListItem("Air System - Test Pressure Build Time to 90psi", "Air System - Test Pressure Build Time to 90psi"));
            //cboServiceThis.Items.Insert(33, new ListItem("Air System - Dryer Change desiccant", "Air System - Dryer Change desiccant"));
            //cboServiceThis.Items.Insert(34, new ListItem("Air System - Air Lines Inspect", "Air System - Air Lines Inspect"));
            //cboServiceThis.Items.Insert(35, new ListItem("Lighting - Check All", "Lighting - Check All"));
            //cboServiceThis.Items.Insert(36, new ListItem("Lighting - Inspect Trailer Socket", "Lighting - Inspect Trailer Socket"));
            //cboServiceThis.Items.Insert(37, new ListItem("Electrical - Alternator Output", "Electrical - Alternator Output"));
            //cboServiceThis.Items.Insert(38, new ListItem("Electrical - Battery Load Test", "Electrical - Battery Load Test"));
            //cboServiceThis.Items.Insert(39, new ListItem("Electrical - Battery Check Levels", "Electrical - Battery Check Levels"));
            //cboServiceThis.Items.Insert(40, new ListItem("Electrical - Starter Draw Test and/or Voltage Drops", "Electrical - Starter Draw Test and/or Voltage Drops"));
            //cboServiceThis.Items.Insert(41, new ListItem("Fifth Wheel - Lube", "Fifth Wheel - Lube"));
            //cboServiceThis.Items.Insert(42, new ListItem("Fifth Wheel - Adjust", "Fifth Wheel - Adjust"));
            //cboServiceThis.Items.Insert(43, new ListItem("Fifth Wheel - Inspect Mounting", "Fifth Wheel - Inspect Mounting"));
            //cboServiceThis.Items.Insert(44, new ListItem("Suspension - Inspect", "Suspension - Inspect"));
            //cboServiceThis.Items.Insert(45, new ListItem("Suspension - Adjust Ride Height(air ride)", "Suspension - Adjust Ride Height(air ride)"));
            //cboServiceThis.Items.Insert(46, new ListItem("Window Glass - Inspect Windshield", "Window Glass - Inspect Windshield"));
         }
      }

      /// <summary>
      /// Service type property passed by the frmMaintenance.aspx
      /// </summary>
      public ServiceType VehicleServiceType
      {
         get { return _serviceType; }
         set { _serviceType = value; }
      }

      public enum ServiceType
      {
         EngineHours = 0,
         Odometer
      }
   }
}