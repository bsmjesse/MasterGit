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
    public partial class Configuration_frmFuelTranSettings : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!Page.IsPostBack)
            {
                GetOrganizationPreferences();
                GuiSecurity(this);
                
            }
        }

        private void GetOrganizationPreferences()
        {
            StringReader strrXML = null;
            DataSet ds = new DataSet();

            string xml = "";
            ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();


            if (objUtil.ErrCheck(dbo.GetOrganizationPreferences(sn.UserID, sn.SecId, sn.User.OrganizationId  , ref xml), false))
                if (objUtil.ErrCheck(dbo.GetOrganizationPreferences(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true ))
                {
                    return;
                }

            if (xml == "")
            {
                ClearFields();
                return; 


            }

            strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                this.txtGPSRadius.Text = ds.Tables[0].Rows[0]["RadiusForGPS"].ToString();
                this.txtHistoryTimeRange.Text = ds.Tables[0].Rows[0]["HistoryTimeRange"].ToString();
                this.txtMaximumReportingInterval.Text = ds.Tables[0].Rows[0]["MaximumReportingInterval"].ToString();
                this.txtNotificationEmailAddress.Text = ds.Tables[0].Rows[0]["NotificationEmailAddress"].ToString();
                this.txtWaitingPeriodToGetMessages.Text = ds.Tables[0].Rows[0]["WaitingPeriodToGetMessages"].ToString();
                cboTimeZone.SelectedIndex = cboTimeZone.Items.IndexOf(cboTimeZone.Items.FindByValue(ds.Tables[0].Rows[0]["Timezone"].ToString()));
                
            }
        }
       
        protected void cmdDriver_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmdrivers.aspx"); 
        }
        protected void cmdUsers_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmUsers.aspx");
        }
        protected void cmdVehicles_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleInfo.aspx");
        }
        protected void cmdFleets_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmEmails.aspx"); 
        }
        protected void cmdSave(object sender, EventArgs e)
        {

            if (!clsUtility.IsNumeric(this.txtGPSRadius.Text))
            {
                this.lblMessage.Text = "Radius should be numeric";  
                return;
            }

            if (!clsUtility.IsNumeric(this.txtMaximumReportingInterval.Text))
            {
                this.lblMessage.Text = "Reporting interval should be numeric";
                return;
            }

            if (!clsUtility.IsNumeric(this.txtHistoryTimeRange.Text))
            {
                this.lblMessage.Text = "History time range should be numeric";
                return;
            }


            if (!clsUtility.IsNumeric(this.txtWaitingPeriodToGetMessages.Text))
            {
                this.lblMessage.Text = "Waiting Period To Get Messages should be numeric";
                return;
            }

            ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();
            if (objUtil.ErrCheck(dbo.UpdateOrganizationPreferences (sn.UserID, sn.SecId,sn.User.OrganizationId,this.txtNotificationEmailAddress.Text,Convert.ToInt32(this.txtGPSRadius.Text),Convert.ToInt32(this.txtMaximumReportingInterval.Text),Convert.ToInt32(this.txtHistoryTimeRange.Text),Convert.ToInt32(this.txtWaitingPeriodToGetMessages.Text ),Convert.ToInt32(this.cboTimeZone.SelectedItem.Value)), false))
                if (objUtil.ErrCheck(dbo.UpdateOrganizationPreferences(sn.UserID, sn.SecId, sn.User.OrganizationId, this.txtNotificationEmailAddress.Text, Convert.ToInt32(this.txtGPSRadius.Text), Convert.ToInt32(this.txtMaximumReportingInterval.Text), Convert.ToInt32(this.txtHistoryTimeRange.Text), Convert.ToInt32(this.txtWaitingPeriodToGetMessages.Text ), Convert.ToInt32(this.cboTimeZone.SelectedItem.Value)), true ))
                {
                    this.lblMessage.Text = "The operation failed.";
                    return;
                }

            this.lblMessage.Text = "The information was successfully updated.";
            return;
        }

        private void ClearFields()
        {
            this.txtGPSRadius.Text = "0";
            this.txtHistoryTimeRange.Text = "0";
            this.txtMaximumReportingInterval.Text = "0";
            this.txtNotificationEmailAddress.Text = "";
            this.txtWaitingPeriodToGetMessages.Text = "0";  
        }
        

}
}
