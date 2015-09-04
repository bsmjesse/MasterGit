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
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace SentinelFM
{

    public partial class frmTopMenu_newmenu : System.Web.UI.Page
    {
        protected string ReportServerURL;
        protected SentinelFMSession sn = null;
        protected clsUtility objUtil;

        public bool HistoryMenu;
        public bool MaintenanceMenu;
        public bool MessagesMenu;
        public bool LandmarkGeozonesMenu;
        public bool ReportsMenu;
        public bool AdministrationMenu;
        public bool LiteMenu;

        public string MessagesLocation = "Messages/frmMesssagesExtendedNew.aspx";
        public string ReportsLocation;
        public string LiteLocation;

        public string UserName;
        public string OrganizationName;

        public bool IsHydroQuebec = false;
        public bool HosEnabled = true;
        public bool ShowNewMap = true;
        public bool ShowAlarms = true;
        public bool ShowGuide = false;

        public string NewMapUrl = "NewMap.aspx";
        public string NewMapCaption = "Map <sup>New";

        //Devin
        public string IsAnt = "0";  

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                sn = (SentinelFMSession)Session["SentinelFMSession"];

                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    if (Page.Request.QueryString["sn"] != null)
                    {
                        SentinelFMSessionBase snb = SentinelFMSession.GetSessionBaseByKey((string)Page.Request.QueryString["sn"]);
                        sn = new SentinelFMSession(snb);
                        if (sn == null)
                            return;
                        Session["SentinelFMSession"] = sn;
                    }
                    else if (Page.Request.UrlReferrer != null && Page.Request.UrlReferrer.Query != null && Page.Request.UrlReferrer.Query.StartsWith("?sn="))
                    {
                        string qs = Page.Request.UrlReferrer.Query.Substring(4);
                        SentinelFMSessionBase snb = SentinelFMSession.GetSessionBaseByKey(qs);
                        sn = new SentinelFMSession(snb);
                        if (sn == null)
                            return;
                        Session["SentinelFMSession"] = sn;
                    }
                    else
                    {
                        RedirectToLogin();
                        return;
                    }

                    sn.User.ExistingPreference(sn);
                    sn.User.GetGuiControlsInfo(sn);
                }
                //if (Session["CompanyLogo"] != null)
                //  this.imgProdLogo.ImageUrl = "images/" + Session["CompanyLogo"].ToString().TrimEnd();

                string ReeferOrganizationId = ConfigurationManager.AppSettings["ReeferOrganizationId"];
                char[] delimiters = new char[] { ',', ';' };
                List<int> organizations = ReeferOrganizationId.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Where(x => Regex.IsMatch(x, @"\d")).Select(n => int.Parse(n)).ToList();
                //if (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 1000026 || sn.User.OrganizationId == 999737)
                if (organizations.Contains(sn.User.OrganizationId))
                {
                    NewMapUrl = "ReeferMap.aspx";
                    NewMapCaption = "Reefer Map";
                }

                if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999956)
                    IsHydroQuebec = true;
                else
                    IsHydroQuebec = false;

                if (!Page.IsPostBack)
                {
                    if (sn.User.OrganizationName == "" || sn.User.OrganizationName == null)
                    {
                        sn.User.ExistingPreference(sn);
                        sn.User.GetGuiControlsInfo(sn);
                    }
                }

                //Devin
                if (sn.User.OrganizationId == 18 || sn.User.OrganizationId == 480) IsAnt = "1";

                HtmlGenericControl hgc = new HtmlGenericControl("style");
                hgc.Attributes.Add("type", "text/css");

                String filePath = HttpContext.Current.Server.MapPath("GlobalStyle.css");

                StreamReader reader = new StreamReader(filePath);
                string content = reader.ReadToEnd();
                reader.Close();
                if (sn.User.MenuColor != "")
                    hgc.InnerText = content.Replace("#009933", sn.User.MenuColor);
                else
                    hgc.InnerText = content;

                if (sn.InterfacePrefrence == (Int16)VLF.CLS.Def.Enums.InterfacePrefrence.Lite || sn.InterfacePrefrence == (Int16)VLF.CLS.Def.Enums.InterfacePrefrence.Both)
                    LiteMenu = true;
                else
                    LiteMenu = false;

                //LiteMenu = true;

                if (LiteMenu)
                {
                    string qs = sn.GetSerializedBase();
                    //string key = clsUtility.SetSessionBaseToMemcahed(qs);
                    string key = clsUtility.SetSessionBaseToTxtFile(qs);
                    LiteLocation = string.Format((string)ConfigurationManager.AppSettings["SentinelLite_URL"], key);
                    //Session["SentinelFMSession"] = null;
                    //Session.Abandon();                    
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                StringWriter tw = new StringWriter(sb);
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                hgc.RenderControl(hw);
                //Response.Write(sb.ToString());
                Page.Header.Controls.Add(
                    new LiteralControl(sb.ToString())
                );
                /*Page.Header.Controls.Add(new LiteralControl("<script language='javascript' type='text/javascript'>alert('test');</script>"));*/
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

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                objUtil = new clsUtility(sn);

                ServerDBUser.DBUser vdbu = new ServerDBUser.DBUser();
                if (vdbu.ValidateLockedUser(sn.UserID) == 1)
                {
                    //Response.Write("<script>alert('Your account is locked, please reset your password'); __doPostBack('mnuMenu','Configuration/frmPreference.aspx')</script>");
                    Response.Redirect("Configuration/frmPreference.aspx?errormsg=1");
                    return;
                }


                Session["CompanyLogo"] = sn.User.CompanyLogo;
                Session["CompanyURL"] = sn.CompanyURL;

                string sURL = @ConfigurationSettings.AppSettings["ReportServerURL"];
                ReportServerURL = sURL + "Login.aspx";

                /*if (sn.User.OrganizationName.TrimEnd().Length > 25)
                    lnkUser.Text = sn.UserName + "," + sn.User.OrganizationName.TrimEnd().Substring(0, 25) + "...";

                else
                    lnkUser.Text = sn.UserName + "," + sn.User.OrganizationName.TrimEnd();

                lnkUser.ToolTip = sn.UserName + "," + sn.User.OrganizationName.TrimEnd();*/

                UserName = sn.UserName;
                if (sn.User.OrganizationName.TrimEnd().Length > 25)
                    OrganizationName = sn.User.OrganizationName.TrimEnd().Substring(0, 25) + "...";

                else
                    OrganizationName = sn.User.OrganizationName.TrimEnd();

                if (!Page.IsPostBack)
                {
                    if (Session["PreferredCulture"] != null)
                        sn.SelectedLanguage = Session["PreferredCulture"].ToString();

                    //LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmMenu, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                    //History View security check
                    HistoryMenu = !sn.User.ControlEnable(sn, 41) ? false : true;

                    //Maintenance security check
                    MaintenanceMenu = !sn.User.ControlEnable(sn, 46) ? false : true;

                    //Messages View security check
                    MessagesMenu = !sn.User.ControlEnable(sn, 24) ? false : true;

                    //string OrganizationsWithNewMessagesScreen=ConfigurationManager.AppSettings["OrganizationsWithNewMessagesScreen"].ToString();
                    //string[] tmp = OrganizationsWithNewMessagesScreen.Split(';');
                    //for (int i=0;i < tmp.Length;i++)
                    //{
                    //    if (clsUtility.IsNumeric(tmp[i]) && Convert.ToUInt32(tmp[i])==sn.User.OrganizationId)
                    //    {
                    //        mnuMenu.Items[5].Value = "EmailSystem/EmailSystem.aspx";
                    //        break;
                    //    }
                    //}

                    if (sn.SuperOrganizationId == 754)
                    {
                        MessagesLocation = "EmailSystem/EmailSystem.aspx";
                    }


                    //Landmark and Geozones View security check
                    LandmarkGeozonesMenu = !sn.User.ControlEnable(sn, 42) ? false : true;

                    if (sn.User.UserGroupId == 27 || sn.User.UserGroupId == 28)
                    {
                        ShowNewMap = false;

                        if (sn.User.UserGroupId == 28)
                            ShowAlarms = false;
                        if (sn.User.UserGroupId == 27)
                            LandmarkGeozonesMenu = false;

                    }

                    if (sn.User.HosEnabled && !IsHydroQuebec)
                    {
                        HosEnabled = !sn.User.ControlEnable(sn, 47) ? false : true;
                        HosEnabled = !sn.User.ControlEnable(sn, 47) ? false : true;
                    }
                    else
                    {
                        HosEnabled = false;
                        HosEnabled = false;
                    }

                    if (sn.SelectedLanguage == "fr-CA")
                    {
                        ShowGuide = true;
                    }
                    else
                    {
                        ShowGuide = false;
                    }

                    //Reports security check
                    ReportsMenu = !sn.User.ControlEnable(sn, 44) ? false : true;

                    string DisableReports = ConfigurationManager.AppSettings["DisableReports"].ToString();

                    if (sn.User.OrganizationId == 999630 || sn.User.OrganizationId == 951)
                        ReportsLocation = "Reports/frmReports_new.aspx";
                    else
                        ReportsLocation = "Reports/frmReportsNav.aspx";	
                    
                    //HOS View security check
                    /*if (sn.User.HosEnabled)
                        mnuMenu.Items[8].Enabled = !sn.User.ControlEnable(sn, 47) ? false : true;
                    else
                        mnuMenu.Items[8].Enabled = false;
                    //Syed ask devin enable
                    mnuMenu.Items[8].Enabled = true;
                    */

                    //Configuration View security check
                    AdministrationMenu = !sn.User.ControlEnable(sn, 16) ? false : true;

                    /*if (Convert.ToInt32(Session["superOrganizationId"]) == sn.SuperOrganizationId)
                    {
                        MenuItem mnu = new MenuItem();
                        mnu.Text = "Organizations Menu";
                        if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca") mnu.Text = "Menu organisationnel";
                        mnu.Value = "frmSuperOrganizationMenu.aspx";
                        mnuMenu.Items.Add(mnu);
                    }*/
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
            catch { }

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

        protected void lnkHelp_Click(object sender, EventArgs e)
        {
            Response.Write("<SCRIPT Language='javascript'>parent.main.window.location='Help/frmHelp.aspx' </SCRIPT>");
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Logout Session:" + Session.SessionID));

                FormsAuthentication.SignOut();
                Session["SentinelFMSession"] = null;
                Session.Abandon();
                string destination = "Login.aspx";
                //if (sn.InterfacePrefrence==(Int16)VLF.CLS.Def.Enums.InterfacePrefrence.Lite ||  sn.InterfacePrefrence==(Int16)VLF.CLS.Def.Enums.InterfacePrefrence.Both)
                //destination = (string)ConfigurationManager.AppSettings["SentinelLite_URL"];

                Response.Write("<SCRIPT Language='javascript'>window.open('" + destination + "','_top') </SCRIPT>");

            }
            catch { }
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

        protected void lnkLite_Click(object sender, EventArgs e)
        {
            string qs = sn.GetSerializedBase();
            //string key = clsUtility.SetSessionBaseToMemcahed(qs);
            string key = clsUtility.SetSessionBaseToTxtFile(qs);
            string destination = string.Format((string)ConfigurationManager.AppSettings["SentinelLite_URL"], key);
            Session["SentinelFMSession"] = null;
            Session.Abandon();
            Response.Write("<SCRIPT Language='javascript'>window.open('" + destination + "','_top') </SCRIPT>");
        }
    }
}
