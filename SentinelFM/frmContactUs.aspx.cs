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
using System.Globalization;


namespace SentinelFM
{
    public partial class frmContactUs : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (sn.User.OrganizationId == 999956)
            {
                if (sn.SelectedLanguage == "fr-CA")
                {
                    lblCustomSupport.Text = "Soutien aux utilisateurs Hydro-Québec <br />par courriel : <span style='font-weight:normal;'> <a href='mailto:hqdgps@hydro.qc.ca' title='hqdgps@hydro.qc.ca' alt='hqdgps@hydro.qc.ca'>>HQD GPS</a> <br /></span>";
                    lblTollFree.Text = "<br />ou <a href='mailto:hqdgps@hydro.qc.ca' title='hqdgps@hydro.qc.ca' alt='hqdgps@hydro.qc.ca'>hqdgps@hydro.qc.ca</a>";
                    lblEmailLabel.Text = "<br /><br />";
                }
                else
                {
                    lblCustomSupport.Text = "Support to Hydro-Quebec users <br />by email : <span style='font-weight:normal;'> <a href='mailto:hqdgps@hydro.qc.ca' title='hqdgps@hydro.qc.ca' alt='hqdgps@hydro.qc.ca'>>HQD GPS</a> <br /></span>";
                    lblTollFree.Text = "<br />or <a href='mailto:hqdgps@hydro.qc.ca' title='hqdgps@hydro.qc.ca' alt='hqdgps@hydro.qc.ca'>hqdgps@hydro.qc.ca</a>";
                    lblEmailLabel.Text = "<br /><br />";
                }
                lblTollFreeNumber.Visible = false;
                lblEmail.Visible = false;
            }

        }

        protected override void InitializeCulture()
        {

            sn = (SentinelFMSession)Session["SentinelFMSession"];
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(sn.SelectedLanguage);
            base.InitializeCulture();
        }
    }
}
