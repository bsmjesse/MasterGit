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
    public partial class frmVehicleExtraInfo : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                
                if (!Page.IsPostBack)
                {
                   this.lblVehicleId.Text  = Request.QueryString["VehicleId"];
                   BoxInfoLoad(Convert.ToInt64(this.lblVehicleId.Text));
                   
                }
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }


        private void BoxInfoLoad(Int64 VehicleId)
        {
            try
            {
                DataSet ds = new DataSet();
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                if (objUtil.ErrCheck(dbv.GetBoxExtraInfo(sn.UserID, sn.SecId, Convert.ToInt64(lblVehicleId.Text), ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetBoxExtraInfo(sn.UserID, sn.SecId, Convert.ToInt64(lblVehicleId.Text), ref xml), true))
                    {
                        return;
                    }


                if (xml == "")
                    return;




                string strPath = Server.MapPath("../Datasets/BoxExtraInfo.xsd");

                ds.ReadXmlSchema(strPath);
                ds.ReadXml(new StringReader(xml));
                sn.Misc.DsBoxExtraInfo = ds;                

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

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
        protected void cmdServices_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleExtraServices.aspx?VehicleId=" + lblVehicleId.Text);
        }
        protected void dgBoxInfo_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            if ((sn.Misc.DsBoxExtraInfo != null) && (sn.Misc.DsBoxExtraInfo.Tables[0] != null))
            {
                // watchFleet.Reset();
                // watchFleet.Start();  
                e.DataSource = sn.Misc.DsBoxExtraInfo;
                //watchFleet.Stop();
                //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<--DataGrid Binding:" + watchFleet.Elapsed.TotalSeconds + " ; parameters:" + sn.UserID));

            }
            else
            {
                e.DataSource = null;
            }
        }
}




}
