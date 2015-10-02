using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using VLF.CLS;
using System.Globalization;

namespace SentinelFM
{
    public partial class MapNew_frmGetVehicleInfo : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;
        protected clsUtility objUtil;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            objUtil = new clsUtility(sn);

            if (Request.QueryString["LicensePlate"] != null)
            {
                VehicleInfoLoad(Request.QueryString["LicensePlate"]);
            }
        }

        protected override void InitializeCulture()
        {
            if (Session["PreferredCulture"] != null)
            {
                string UserCulture = Session["PreferredCulture"].ToString();
                if (UserCulture != "")
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(UserCulture);
                }
            }
        }

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
                    
                    this.lblBoxId.Text = drVehicleInfo[1].ToString();
                    this.lblPlate.Text = drVehicleInfo[0].ToString();
                    this.lblMake.Text = drVehicleInfo[5].ToString();
                    this.lblModel.Text = drVehicleInfo[6].ToString();
                    this.lblVehicleInfo.Text = drVehicleInfo[11].ToString();
                    this.lblVin.Text = drVehicleInfo[3].ToString();
                    this.lblYear.Text = drVehicleInfo[9].ToString();
                    this.lblColor.Text = drVehicleInfo[10].ToString();
                    this.lblType.Text = drVehicleInfo[7].ToString();
                }                
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
    }
}