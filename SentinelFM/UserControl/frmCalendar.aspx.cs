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
using System.Globalization; 

namespace SentinelFM.UserControl
{
	/// <summary>
	/// Summary description for frmCalendar.
	/// </summary>
	public partial class frmCalendar : System.Web.UI.Page// SentinelFMBasePage
	{

        protected override void InitializeCulture()
        {

            SentinelFMSession snMain = (SentinelFMSession)Session["SentinelFMSession"];
            if (snMain == null || snMain.User == null || String.IsNullOrEmpty(snMain.UserName))
            {
               
                snMain = (SentinelFMSession)Session["SentinelFMSession"];

            }

            if (snMain.SelectedLanguage != null)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new
                    CultureInfo("en-US");

                System.Threading.Thread.CurrentThread.CurrentCulture = new
                 CultureInfo(snMain.SelectedLanguage);

                base.InitializeCulture();
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new
                   CultureInfo("en-US");

                base.InitializeCulture();
            }




            //#region HQ French
            //if (snMain.User.OrganizationId == 999763 || snMain.User.OrganizationId == 999956 || snMain.User.OrganizationId == 999957 || snMain.User.OrganizationId == 999955)
            //{
            //    Session["PreferredCulture"] = "fr-CA";
            //    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["PreferredCulture"].ToString());
            //    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["PreferredCulture"].ToString());
            //    base.InitializeCulture();

            //}
            //#endregion

        }

		protected void Page_Load(object sender, System.EventArgs e)
		{

            System.Globalization.CultureInfo ci = new
            System.Globalization.CultureInfo("fr-CA");

            
            

			if (!Page.IsPostBack)
			{
				
				this.lblControlName.Text= Request.QueryString["textbox"].ToString();  
			}
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

		protected void cldTo_SelectionChanged(object sender, System.EventArgs e)
		{
            string strScript = "<script>window.opener.document.forms[0]." + this.lblControlName.Text + ".value = '";
           if (System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName=="fr") 
                strScript += calDate.SelectedDate.ToString("dd/MM/yyyy", System.Threading.Thread.CurrentThread.CurrentCulture);
            else
                strScript += calDate.SelectedDate.ToString("MM/dd/yyyy", System.Threading.Thread.CurrentThread.CurrentCulture);

            strScript += "';self.close()";
            strScript += "</" + "script>";
            RegisterClientScriptBlock("anything", strScript);
		}
	}
}
