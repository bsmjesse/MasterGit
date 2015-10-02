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
    public partial class frmViewVehicleGeozones : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string GeoZoneId = Request.QueryString["GeoZoneId"];
            LoadVehicles(Convert.ToInt16(GeoZoneId));

            dgVehicles.Columns[0].HeaderText = GetLocalResourceObject("dgVehicles.Columns0.Header.Text").ToString();
            dgVehicles.Columns[1].HeaderText = GetLocalResourceObject("dgVehicles.Columns1.Header.Text").ToString();
            dgVehicles.Columns[2].HeaderText = GetLocalResourceObject("dgVehicles.Columns2.Header.Text").ToString();
            dgVehicles.DataBind();
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
                        NoGeoZones();
                    }


                if (xml == "")
                    NoGeoZones();
 
                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    this.lblMessage.Visible = false;   
                    this.dgVehicles.DataSource = ds;
                    this.dgVehicles.DataBind();  
                }
                else
                {
                    NoGeoZones();
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void NoGeoZones()
        {
            this.dgVehicles.DataSource = null;
            this.dgVehicles.DataBind();
            this.lblMessage.Visible = true;
            this.lblMessage.Text = "No Assignments";
        }
    }
}
