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
using VLF.CLS;



namespace SentinelFM
{
   /// <summary>
   /// Summary description for frmVehicleDescription.
   /// </summary>
    public partial class frmVehicleDescription : SentinelFMBasePage
   {
      protected System.Web.UI.HtmlControls.HtmlTable tblWait;
      
      public string redirectURL;

      protected void Page_Load(object sender, System.EventArgs e)
      {
         try
         {
            Int64 VehicleId = 0;
           

            if (!Page.IsPostBack)
            {

                this.cmdSettings.Visible = !sn.User.ControlEnable(sn, 49) ? false : true;
                this.cmdServices.Visible = !sn.User.ControlEnable(sn, 52) ? false : true;
                this.cmdUnitInfo.Visible = !sn.User.ControlEnable(sn, 53) ? false : true; 
                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmVehicleDescriptionForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

               if (Request.QueryString["VehicleId"] != null)
               {
                  VehicleId = Convert.ToInt64(Request.QueryString["VehicleId"]);
                  VehicleInfoLoad(VehicleId);
               }
               else
                  if (Request.QueryString["LicensePlate"] != null)
                  {
                     VehicleInfoLoad(Request.QueryString["LicensePlate"]);
                  }
            }

            
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

      }
      #endregion

      private void VehicleInfoLoad(Int64 VehicleId)
      {
         try
         {
            DataSet ds = new DataSet();
            string xml = "";
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
            //DumpBeforeCall(sn, string.Format("VehicleInfoLoad : VehicleId = {0}", VehicleId));
            if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), false))
               if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), true))
               {
                  return;
               }

            if (xml == "")
            {
               return;
            }

            ds.ReadXml(new StringReader(xml));

            this.lblVehicleId.Text = Convert.ToString(ds.Tables[0].Rows[0]["VehicleId"]);
            this.lblBoxId.Text = Convert.ToString(ds.Tables[0].Rows[0].ItemArray[1]);
            this.lblPlate.Text = Convert.ToString(ds.Tables[0].Rows[0].ItemArray[0]);
            this.lblMake.Text = Convert.ToString(ds.Tables[0].Rows[0].ItemArray[5]);
            this.lblModel.Text = Convert.ToString(ds.Tables[0].Rows[0].ItemArray[6]);
            this.lblVehicleInfo.Text = Convert.ToString(ds.Tables[0].Rows[0].ItemArray[11]);
            this.lblVin.Text = Convert.ToString(ds.Tables[0].Rows[0].ItemArray[3]);
            this.lblYear.Text = Convert.ToString(ds.Tables[0].Rows[0].ItemArray[9]);
            this.lblColor.Text = Convert.ToString(ds.Tables[0].Rows[0].ItemArray[10]);
            this.lblType.Text = Convert.ToString(ds.Tables[0].Rows[0].ItemArray[7]);

            
            if (objUtil.ErrCheck(dbv.GetVehicleAdditionalInfoXML(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), ref xml), false))
               if (objUtil.ErrCheck(dbv.GetVehicleAdditionalInfoXML(sn.UserID, sn.SecId, Convert.ToInt64(this.lblVehicleId.Text), ref xml), true))
               {
                  return;
               }

            if (xml != "")
            {

               DataSet dsInfo = new DataSet();
               dsInfo.ReadXml(new StringReader(xml));

               if (dsInfo.Tables.Count > 0 && dsInfo.Tables[0].Rows.Count > 0)
               {
                  this.lblField1.Text = dsInfo.Tables[0].Rows[0]["Field1"].ToString();
                  this.lblField2.Text = dsInfo.Tables[0].Rows[0]["Field2"].ToString();
                  this.lblField3.Text = dsInfo.Tables[0].Rows[0]["Field3"].ToString();
                  this.lblField4.Text = dsInfo.Tables[0].Rows[0]["Field4"].ToString();
                  this.lblField5.Text = dsInfo.Tables[0].Rows[0]["Field5"].ToString();
               }
            }


            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
            DataSet dsFleet = new DataSet();
            
            if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByVehicleId (sn.UserID, sn.SecId, VehicleId, ref xml), false))
               if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), true))
               {
                  return;
               }

            if (xml == "")
            {
               return;
            }

            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower() == "fr")
                xml = xml.Replace("All Vehicles", "Tous les véhicules"); 

            dsFleet.ReadXml(new StringReader(xml));

            dgFleets.DataSource = dsFleet;
            dgFleets.DataBind(); 
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }

      }

      /// <summary>
      /// Load vehicle data
      /// </summary>
      /// <param name="licensePlate"></param>
      private void VehicleInfoLoad(string licensePlate)
      {
         try
         {
            if (String.IsNullOrEmpty(licensePlate)) return;

            DataSet ds = new DataSet();
            string xml = "";
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
            
            if (objUtil.ErrCheck(dbv.GetVehicleInfoXML(sn.UserID, sn.SecId, licensePlate, ref xml), false))
               if (objUtil.ErrCheck(dbv.GetVehicleInfoXML(sn.UserID, sn.SecId, licensePlate, ref xml), true))
               {
                  return;
               }

            if (xml == "")
            {
               return;
            }

            ds.ReadXml(new StringReader(xml));

            if (Util.IsDataSetValid(ds))
            {
               DataRow drVehicleInfo = ds.Tables[0].Rows[0];
               this.lblVehicleId.Text = drVehicleInfo["VehicleId"].ToString();
               this.lblBoxId.Text = drVehicleInfo[1].ToString();
               this.lblPlate.Text = drVehicleInfo[0].ToString();
               this.lblMake.Text = drVehicleInfo[5].ToString();
               this.lblModel.Text = drVehicleInfo[6].ToString();
               this.lblVehicleInfo.Text = drVehicleInfo[11].ToString();
               this.lblVin.Text = drVehicleInfo[3].ToString();
               this.lblYear.Text = drVehicleInfo[9].ToString();
               this.lblColor.Text = drVehicleInfo[10].ToString();
               this.lblType.Text = drVehicleInfo[7].ToString();

               this.lblField1.Text = drVehicleInfo["Field1"].ToString();
               this.lblField2.Text = drVehicleInfo["Field2"].ToString();
               this.lblField3.Text = drVehicleInfo["Field3"].ToString();
               this.lblField4.Text = drVehicleInfo["Field4"].ToString();
            }


            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
            DataSet dsFleet = new DataSet();
            
            if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(lblVehicleId.Text), ref xml), false))
               if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(lblVehicleId.Text), ref xml), true))
               {
                  return;
               }

            if (xml == "")
            {
               return;
            }

            dsFleet.ReadXml(new StringReader(xml));

            dgFleets.DataSource = dsFleet;
            dgFleets.DataBind(); 
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }

      }

      protected void cmdSensorCommands_Click(object sender, System.EventArgs e)
      {
         Response.Redirect("frmSensorMain.aspx?LicensePlate=" + this.lblPlate.Text);
      }

      protected void cmdReefer_Click(object sender, EventArgs e)
      {
         Response.Redirect("frmReefer.aspx?LicensePlate=" + this.lblPlate.Text);
      }
        protected void cmdSettings_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleAttributes.aspx?VehicleId=" + lblVehicleId.Text);
        }
        protected void cmdServices_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleExtraServices.aspx?VehicleId=" + lblVehicleId.Text);
        }
        protected void cmdUnitInfo_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleExtraInfo.aspx?VehicleId=" + lblVehicleId.Text);
        }
}
}
