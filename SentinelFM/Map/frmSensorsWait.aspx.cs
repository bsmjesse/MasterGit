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
using System.Configuration;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace SentinelFM.Map
{
    /// <summary>
    /// Summary description for frmSensorsWait.
    /// </summary>
    public partial class frmSensorsWait : SentinelFMBasePage
    {
        protected System.Web.UI.WebControls.Image Image1;
        public string MyDestinationUrl = "";
        protected void Page_Load(object sender, System.EventArgs e)
        {
            string LicensePlate = Request.QueryString["LicensePlate"];
            string destinationUrl = "frmSensorsInfo.aspx?LicensePlate=" + LicensePlate;
            //string destinationUrl = "frmSensorsInfo_Reefer.aspx?LicensePlate=" + LicensePlate;

            //if (sn != null && sn.User != null && (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 1000026 || sn.User.OrganizationId == 999737))
            string ReeferOrganizationId = ConfigurationManager.AppSettings["ReeferOrganizationId"];
            char[] delimiters = new char[] { ',', ';' };
            if (!String.IsNullOrEmpty(ReeferOrganizationId))
            {
                List<int> organizations = ReeferOrganizationId.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Where(x => Regex.IsMatch(x, @"\d")).Select(n => int.Parse(n)).ToList();

                if (organizations.Contains(sn.User.OrganizationId))
                {
                    destinationUrl = "frmSensorsInfo_Reefer.aspx?LicensePlate=" + LicensePlate;
                }
            }
            
            MyDestinationUrl = destinationUrl;
            //MyDestinationUrl = "frmSensorInfoNew.aspx?LicensePlate=" + LicensePlate;
            Response.AppendHeader("Refresh", "1; url='"+MyDestinationUrl);
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
    }
}
