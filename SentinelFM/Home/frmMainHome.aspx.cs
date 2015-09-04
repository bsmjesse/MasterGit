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
using System.IO ; 

namespace SentinelFM.ServicesUpdates
{
	/// <summary>
	/// Summary description for frmMainHome.
	/// </summary>
	public partial class frmMainHome : SentinelFMBasePage
	{

        public string mainURL = "";

	


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

        protected void Page_Init(object sender, EventArgs e)
        {
          
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            GetCompanyHomePage();

            if (sn.HomePagePicture != "")
                mainURL = "frmCustomPage.aspx";
            else
                mainURL = sn.CompanyURL;
        }


        private void GetCompanyHomePage()
        {
            //Company Logo and home Page
            string xml = "";
            clsUtility objUtil = new clsUtility(sn);
            using (ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization())
            {
                if (objUtil.ErrCheck(dbo.GetOrganizationInfoXMLByUserId(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetOrganizationInfoXMLByUserId(sn.UserID, sn.SecId, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetOrganizationInfoXMLByUserId. User:" + sn.UserID.ToString() + " Form:clsUser "));
                    }

                if (xml == "")
                {
                    sn.User.CompanyLogo = ConfigurationSettings.AppSettings["DefaultLogo"];
                    sn.CompanyURL = ConfigurationSettings.AppSettings["DefaultCompanyURL"];
                }
                else
                {

                    DataSet dsCompany = new DataSet();
                    StringReader strrXML = new StringReader(xml);
                    dsCompany.ReadXml(strrXML);

                    try
                    {
                        sn.CompanyURL = dsCompany.Tables[0].Rows[0]["HomePageName"].ToString().TrimEnd();
                    }
                    catch
                    {
                        sn.CompanyURL = ConfigurationSettings.AppSettings["DefaultCompanyURL"];
                    }

                }
            }
        }
	}
}
