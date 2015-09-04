using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;


namespace SentinelFM
{
    public partial class GeoZone_Landmarks_frmVehicleGeozones : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string GeoZoneId = Request.QueryString["GeoZoneId"];
            LoadVehicles(Convert.ToInt16(GeoZoneId));
        }


        private void LoadVehicles(Int16 GeoZoneId)
        {
            try
            {



                StringReader strrXML = null;
                DataSet ds= new DataSet();
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                
                if (objUtil.ErrCheck(dbv.GetAllAssignedGeozonesToVehicle(sn.UserID, sn.SecId, sn.User.OrganizationId, GeoZoneId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetAllAssignedGeozonesToVehicle(sn.UserID, sn.SecId, sn.User.OrganizationId, GeoZoneId, ref xml), true))
                    {
                        this.dgVehicles.DataSource = null;
                        this.dgVehicles.DataBind();  
                    }




                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);
                this.dgVehicles.DataSource = ds;
                this.dgVehicles.DataBind();  

               

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
    }
}
