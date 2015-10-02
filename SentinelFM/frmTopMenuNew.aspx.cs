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
using System.Globalization;  
namespace SentinelFM
{
    public partial class frmTopMenuNew : System.Web.UI.Page
    {
        
        protected string ReportServerURL;
        protected SentinelFMSession sn = null;
        protected clsUtility objUtil;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
               

                sn = (SentinelFMSession)Session["SentinelFMSession"];

                if (Session.Contents == null || sn == null || String.IsNullOrEmpty(sn.UserName))
                {
                    RedirectToLogin();
                    return;
                }

                objUtil = new clsUtility(sn);

                Session["CompanyLogo"] = sn.User.CompanyLogo;
                Session["CompanyURL"] =sn.CompanyURL;

                string sURL = @ConfigurationSettings.AppSettings["ReportServerURL"];
                ReportServerURL = sURL + "Login.aspx";

                if (sn.User.OrganizationName.TrimEnd().Length > 25)
                    lnkUser.Text = sn.UserName + "," + sn.User.OrganizationName.TrimEnd().Substring(0, 25) + "...";

                else
                    lnkUser.Text = sn.UserName + "," + sn.User.OrganizationName.TrimEnd();

                lnkUser.ToolTip = sn.UserName + "," + sn.User.OrganizationName.TrimEnd();

                if (!Page.IsPostBack)
                {
                
 
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmMenu, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                    //History View security check
                    mnuMenu.Items[2].Enabled = !sn.User.ControlEnable(sn, 41) ? false : true;

                    //Maintenance security check
                    mnuMenu.Items[3].Enabled = !sn.User.ControlEnable(sn, 46) ? false : true;

                    //Messages View security check
                    mnuMenu.Items[4].Enabled = !sn.User.ControlEnable(sn, 24) ? false : true;

                    //Landmark and Geozones View security check
                    mnuMenu.Items[5].Enabled = !sn.User.ControlEnable(sn, 42) ? false : true;


                    //Reports security check
                    mnuMenu.Items[6].Enabled = !sn.User.ControlEnable(sn, 44) ? false : true;
                    mnuMenu.Items[6].Value = "Reports/frmReportsNav.aspx";
                      
                    //HOS View security check
                    if (sn.User.HosEnabled)
                        mnuMenu.Items[7].Enabled = !sn.User.ControlEnable(sn, 47) ? false : true;
                    else
                        mnuMenu.Items[7].Enabled = false; 



                    //Configuration View security check
                    mnuMenu.Items[8].Enabled = !sn.User.ControlEnable(sn, 16) ? false : true;


                    if (Convert.ToInt32(Session["superOrganizationId"]) == sn.SuperOrganizationId)
                   {
                       MenuItem mnu = new MenuItem();
                       mnu.Text = "Organizations Menu";
                       mnu.Value = "frmSuperOrganizationMenu.aspx"; 
                       mnuMenu.Items.Add(mnu);
                   }


                }
            }

            catch
            {
                RedirectToLogin();
            }
        }

        protected void mnuMenu_MenuItemClick(object sender, MenuEventArgs e)
        {

             //if  ( (e.Item.Value == "Map/frmMain.aspx"
             //           || e.Item.Value == "History/frmhistmain.aspx"
             //           || e.Item.Value == "Configuration/frmEmails.aspx" 
             //           || e.Item.Value == "GeoZone_Landmarks/frmLandmark.aspx")
             //           || e.Item.Value == "Logout")
             //{

             //    if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
             //      sn.MapSolute.LoadDefaultMap(sn);
             //}

            //clsUtility.LogUserAction(sn.UserID, e.Item.Value); 
            if (e.Item.Value == "frmSuperOrganizationMenu.aspx")
            {
                Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('frmSuperOrganizationMenu.aspx','_top')</SCRIPT>");
            }
            else if (e.Item.Value == "Logout")
            {

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Logout Session:" + Session.SessionID.ToString()));
                FormsAuthentication.SignOut();
                Session["SentinelFMSession"] = null;
                Session.Abandon();
                Response.Write("<SCRIPT Language='javascript'>window.open('Login.aspx','_top') </SCRIPT>");
            }

            else
                Response.Write("<SCRIPT Language='javascript'>parent.main.window.location='" + e.Item.Value + "' </SCRIPT>");
        }

        protected void lnkUser_Click(object sender, EventArgs e)
        {
            foreach (MenuItem mi in mnuMenu.Items)
                mi.Selected = false;

            Response.Write("<SCRIPT Language='javascript'>parent.main.window.location='Configuration/frmPreference.aspx' </SCRIPT>");
        }

        private void RedirectToLogin()
        {

            int UserId = 0;
            string frmName = "";

            //Session Check
            try
            {
                SentinelFMSession snMain = (SentinelFMSession)Session["SentinelFMSession"];
                if (snMain != null && snMain.UserID != null)
                    UserId = snMain.UserID;
                else
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  Session null , Form:" + frmName));
            }
            catch
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  Session null , Form:" + frmName));
            }

            //Form Name
            try
            {
                frmName = Page.GetType().Name;
            }
            catch
            {
            }

            //Get Last Error if exists
            try
            {
                if (Server.GetLastError() != null)
                {
                    Exception ex = Server.GetLastError().GetBaseException();

                    string Excp = "SentinelFM " +
                        "MESSAGE: " + ex.Message +
                        "\nSOURCE: " + ex.Source +
                        "\nFORM: " + frmName +
                        "\nQUERYSTRING: " + Request.QueryString.ToString() +
                        "\nTARGETSITE: " + ex.TargetSite +
                        "\nSTACKTRACE: " + ex.StackTrace;

                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin - UserId:" + UserId.ToString() + " , Form:" + frmName + " , Error :" + Excp));
                }
                else
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  UserId:" + UserId.ToString() + " , Form:" + frmName));
            }
            catch
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin - not exception info,  Form:" + frmName));
            }

            Session.Abandon();
            Response.Write("<SCRIPT Language='javascript'>window.open('Login.aspx','_top') </SCRIPT>");
            return;
            
            
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

       
    }
}