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
using VLF.CLS.Def;
using System.IO;

namespace SentinelFM
{
    public partial class frmVehicleExtraServices : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                
                if (!Page.IsPostBack)
                {
                   this.lblVehicleId.Text  = Request.QueryString["VehicleId"];
                   VehicleInfoLoad(Convert.ToInt64(this.lblVehicleId.Text));
                   
                }
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }


        private void VehicleInfoLoad(Int64 VehicleId)
        {
            try
            {
                DataSet ds = new DataSet();
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                if (objUtil.ErrCheck(dbv.GetVehicleExtraServiceHistoryByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(lblVehicleId.Text), ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleExtraServiceHistoryByVehicleId(sn.UserID, sn.SecId, Convert.ToInt64(lblVehicleId.Text), ref xml), true))
                    {
                        return;
                    }


                if (xml == "")
                    return;
 
                ds.ReadXml(new StringReader(xml));
                if (ds != null || ds.Tables.Count > 0 || ds.Tables[0].Rows.Count > 0)
                {
                    this.txtField1.Text = ds.Tables[0].Rows[0]["Field1"].ToString();
                    this.txtField2.Text = ds.Tables[0].Rows[0]["Field2"].ToString();
                    this.txtField3.Text = ds.Tables[0].Rows[0]["Field3"].ToString();
                    this.txtField4.Text = ds.Tables[0].Rows[0]["Field4"].ToString();
                    this.txtField5.Text = ds.Tables[0].Rows[0]["Field5"].ToString();
                    this.txtField6.Text = ds.Tables[0].Rows[0]["Field6"].ToString();
                    this.txtField7.Text = ds.Tables[0].Rows[0]["Field7"].ToString();

                    this.txtField8.Text = ds.Tables[0].Rows[0]["Field8"].ToString();
                    this.txtField9.Text = ds.Tables[0].Rows[0]["Field9"].ToString();
                    this.txtField10.Text = ds.Tables[0].Rows[0]["Field10"].ToString();
                    this.txtField11.Text = ds.Tables[0].Rows[0]["Field11"].ToString();
                    this.txtField12.Text = ds.Tables[0].Rows[0]["Field12"].ToString();
                    this.txtField13.Text = ds.Tables[0].Rows[0]["Field13"].ToString();
                    this.txtField14.Text = ds.Tables[0].Rows[0]["Field14"].ToString();
                    this.txtField15.Text = ds.Tables[0].Rows[0]["Field15"].ToString();
                    this.txtField16.Text = ds.Tables[0].Rows[0]["Field16"].ToString();
                }
                
                


            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }


        protected void cmdSave_Click(object sender, EventArgs e)
        {
            
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
            if (objUtil.ErrCheck(dbv.VehicleExtraServiceHistory_Add_Update(sn.UserID, sn.SecId, Convert.ToInt64(lblVehicleId.Text), this.txtField1.Text, this.txtField2.Text, this.txtField3.Text, this.txtField4.Text, this.txtField5.Text, this.txtField6.Text, this.txtField7.Text, this.txtField8.Text, this.txtField9.Text, this.txtField10.Text, this.txtField11.Text, this.txtField12.Text, this.txtField13.Text, this.txtField14.Text, this.txtField15.Text, this.txtField16.Text), false))
                if (objUtil.ErrCheck(dbv.VehicleExtraServiceHistory_Add_Update(sn.UserID, sn.SecId, Convert.ToInt64(lblVehicleId.Text), this.txtField1.Text, this.txtField2.Text, this.txtField3.Text, this.txtField4.Text, this.txtField5.Text, this.txtField6.Text, this.txtField7.Text, this.txtField8.Text, this.txtField9.Text, this.txtField10.Text, this.txtField11.Text, this.txtField12.Text, this.txtField13.Text, this.txtField14.Text, this.txtField15.Text, this.txtField16.Text), true ))
                {
                    this.lblMessage.Text = "Update failed.";  
                    return;
                }

            ClientScript.RegisterClientScriptBlock(this.GetType(), "CloseWin","<script language='javascript'>WinClose()</script>", false);
        }

        protected void cmdVehicleInfo_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleDescription.aspx?VehicleId=" + lblVehicleId.Text);
        }
        protected void cmdSensorCommands_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmSensorMain.aspx?LicensePlate=" +sn.Cmd.SelectedVehicleLicensePlate);
        }

        protected void cmdSettings_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleAttributes.aspx?VehicleId=" + lblVehicleId.Text);
        }
        protected void cmdUnitInfo_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleExtraInfo.aspx?VehicleId=" + lblVehicleId.Text);
        }
}




}
