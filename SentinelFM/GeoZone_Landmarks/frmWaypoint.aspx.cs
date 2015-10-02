using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;

namespace SentinelFM
{
    public partial class GeoZone_Landmarks_frmWaypoint : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.tblWaypointAdd.Visible = false;   
        }

        protected void cmdLandmark_Click(object sender, EventArgs e)
        {

        }
        protected void cmdGeoZone_Click(object sender, EventArgs e)
        {

        }
        protected void cmdVehicleGeoZone_Click(object sender, EventArgs e)
        {

        }
        protected void cmdMap_Click(object sender, EventArgs e)
        {

        }
        protected void lstAddOptions_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void DgWaypoints_Fill()
        {
            try
            {
                StringReader strrXML = null;
                DataSet dsLandmarks = new DataSet();

                string xml = "";
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                if (objUtil.ErrCheck(dbo.GetOrganizationWaypoints(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetOrganizationWaypoints(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                    {
                        return;
                    }

                //if (xml == "")
                //{
                //    this.dgLandmarks.DataSource = null;
                //    this.dgLandmarks.DataBind();
                //    return;
                //}


                strrXML = new StringReader(xml);
                dsLandmarks.ReadXml(strrXML);

                sn.DsLandMarks = dsLandmarks;
                // Current Status
                DataColumn dc = new DataColumn("LandmarkId", Type.GetType("System.Int32"));
                dc.DefaultValue = 0;
                sn.DsLandMarks.Tables[0].Columns.Add(dc);


                Int32 i = 0;
                foreach (DataRow rowItem in dsLandmarks.Tables[0].Rows)
                {
                    rowItem["Radius"] = Convert.ToInt32(Convert.ToInt32(rowItem["Radius"]) * Convert.ToDecimal(ViewState["UnitValue"]));
                    i++;
                    rowItem["LandmarkId"] = i;
                }





                //this.dgLandmarks.DataSource = sn.DsLandMarks;
                //this.dgLandmarks.DataBind();
                sn.Misc.ConfDtLandmarks = sn.DsLandMarks.Tables[0];

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
}
}
