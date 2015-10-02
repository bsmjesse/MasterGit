using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.IO;
using System.Configuration;
using System.Security.Cryptography;
using VLF.CLS;
using System.Globalization;
using CaptchaDotNet2.Security.Cryptography;
using CaptchaDotNet2;

namespace SentinelFM
{

   

    /// <summary>
    /// Summary description for Login.
    /// </summary>
    public partial class CaptchaTimeOut : Page
    {

        public delegate void CaptchaEventHandler();
        protected ServerDBUser.DBUser dbu;
        protected SentinelFMSession sn = null;
        protected clsUtility objUtil;

        private string color = "#ffffff";
        protected string style;
        private event CaptchaEventHandler success;
        private event CaptchaEventHandler failure;

        //protected static int failcount = 0;

        public string Message
        {
            // We don't set message in page_load, because it prevents us from changing message in failure event
            set { lblMessage.Text = value; }
            get { return lblMessage.Text; }
        }
        public string BackgroundColor
        {
            set { color = value.Trim("#".ToCharArray()); }
            get { return color; }
        }
        public string Style
        {
            set { style = value; }
            get { return style; }
        }
        public event CaptchaEventHandler Success
        {
            add { success += value; }
            remove { success += null; }
        }
        public event CaptchaEventHandler Failure
        {
            add { failure += value; }
            remove { failure += null; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {

            try
            {
                int vs = 0;
                if (ViewState["failcount"] == null)
                    ViewState["failcount"] = 0;
                else
                    vs = Convert.ToInt32(ViewState["failcount"]);

                if (!Page.IsPostBack)
                {

                    if (ViewState["auth_seed"] == null)
                        ViewState["auth_seed"] = Convert.ToString(new System.Random().NextDouble());

                    string strPathxxx = "c:\\";

                    // Check User License and cookies
                    string Logo = Request.Form["Logo"];
                    string URL = Request.Form["URL"];
                    string showLogo = Request.QueryString["showLogo"];


                    //// Redirect for SSL Security - Release
                    //string sDomainName = @ConfigurationSettings.AppSettings["DomainName"];
                    //string sPath = ""; 
                    //if (!Request.IsSecureConnection)
                    //{
                    //   sPath = "https://" + sDomainName + "/login.aspx";
                    //   Response.Redirect(sPath);
                    //}


                    string IpAddr = Request.ServerVariables["remote_addr"];
                    // System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Login page access - IP:" + IpAddr ));


                 
                    //if (Logo != null)
                    //{
                    //    Session["CompanyLogo"] = Logo;
                    //    Session["CompanyURL"] = URL;
                    //    Response.Cookies.Clear();
                    //    HttpCookie Sentinel = new HttpCookie("SentinelCompany");
                    //    Sentinel.Name = "SentinelCompany";
                    //    Sentinel.Value = String.Format("{0}|{1}", Logo, URL);
                    //    Response.Cookies.Add(Sentinel);

                    //}
                    //else
                    //{
                    //    HttpCookieCollection SentinelCookieColl;
                    //    HttpCookie SentinelCookie;
                    //    SentinelCookieColl = Request.Cookies;

                    //    if (SentinelCookieColl.Count > 0)
                    //    {
                    //        string[] arrCookies = SentinelCookieColl.AllKeys;
                    //        for (int i = 0; i < arrCookies.Length; i++)
                    //        {
                    //            SentinelCookie = SentinelCookieColl[arrCookies[i]];
                    //            if (SentinelCookie.Name == "SentinelCompany")
                    //            {
                    //                string[] values = SentinelCookie.Value.ToString().Split('|');
                    //                Session["CompanyLogo"] = values[0];
                    //                Session["CompanyURL"] = values[1];
                    //                break;
                    //            }
                    //        }
                    //    }
                    //    else
                    //        Session["CompanyLogo"] = ConfigurationSettings.AppSettings["DefaultLogo"];
                    //}


                    DrawLogo();

                    if (Request.QueryString["Password"] != null)
                    {
                        if (vs < 3)

                            DoLogin();
                        else
                            TestCaptcha();
                    }
                }
                else
                {
                    if (Request.QueryString["Password"] != null)
                        DoLogin();
                    else
                        if (this.txtPassword.Text != "")
                            DoLogin();
                }

            }

            catch (System.Threading.ThreadAbortException) { return; }
            catch (Exception Ex)
            {
                //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" Form:Login.aspx"));    
            }


        }


        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
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

        protected void OnSuccess()
        {
            ViewState["failcount"] = 0;
            DoLogin();
        }

        protected void OnFailure()
        {
            lblMessage.Text = "The text you entered was not correct. Please try again:";
            lblMessage.Visible = true;
        }


        private void DoLogin()
        {
            string destination = string.Empty;

            if (Convert.ToInt16(ViewState["failcount"]) > 2)
            {
                if (!CaptchaPanel.Visible)
                    SetCaptcha();
                else
                    if (!TestCaptcha())
                        return;
            }

            int uid = -1;
            string secId = "";
            int superOrganizationId = 1;

           // string IpAddr = Request.ServerVariables["remote_addr"];


            string IpAddr = "";
            try
            {
                if (HttpContext.Current.Request.UserHostAddress.Trim() != "")
                    IpAddr = HttpContext.Current.Request.UserHostAddress.Trim();
                else if (HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].Trim() != "")
                    IpAddr = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].Trim();
                else if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Trim() != "")
                    IpAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Trim();

            }
            catch
            {
            }


            string HashPassword = "";
            string strUsername = txtUserName.Text;


            if (Request.QueryString["Password"] != null)
            {
                strUsername = Request.QueryString["Username"].ToString();
                HashPassword = Request.QueryString["Password"].ToString();
            }
            else
                HashPassword = Request.Form["txtHash"].ToString().TrimEnd();

            //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info , "DoLogin - IP:" + IpAddr + " Psw: " + HashPassword ));


            try
            {

                SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager();
                //bool isLocked = false;
                //sec.GetIpStatus(IpAddr, ref isLocked);
                //if (isLocked)
                //{
                //    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AuthFailed");
                //    this.lblMessage.Visible = true;
                //    return;
                //}


                int errCode = 0;

                //if (Request.QueryString["Password"] != null)
                //   errCode = sec.LoginSHA( strUsername, HashPassword, IpAddr, ref uid, ref secId);
                //else

                string conString = this.cboDataBaseName.SelectedItem.Value;
                errCode = sec.LoginMD5ByDBName(ViewState["auth_seed"].ToString(), strUsername, HashPassword, IpAddr, conString, ref uid, ref secId, ref superOrganizationId);

                if (errCode != (int)VLF.ERRSecurity.InterfaceError.NoError)
                {

                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AuthFailed");
                    this.lblMessage.Visible = true;
                    //sec.AddFailedEntry(txtUserName.Text, IpAddr);
                    //captcha
                    if (ViewState["failcount"] == null)
                        ViewState["failcount"] = 1;
                    else
                        ViewState["failcount"] = Convert.ToInt16(ViewState["failcount"]) + 1;

                    //captcha

                }
                else
                {
                    //captcha
                    CaptchaPanel.Visible = false;
                    //captcha
                    this.lblMessage.Visible = false;
                    // sec.ClearFailedLoginsByIP(IpAddr);
                    SentinelFMSession sn = new SentinelFMSession();
                    sn.UserID = uid;
                    sn.LoginUserID = uid;
                    sn.SecId = secId;
                    sn.LoginUserSecId = secId;
                    sn.SuperOrganizationId = superOrganizationId;

                    try
                    {
                        int SuperOrganizationPermitted = Convert.ToInt32(ConfigurationSettings.AppSettings["SuperOrganizationPermitted"]);
                        if ((SuperOrganizationPermitted != -1) && (SuperOrganizationPermitted != superOrganizationId))
                        {
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AuthFailed");
                            this.lblMessage.Visible = true;
                            return;
                        }

                    }
                    catch
                    {
                    }

                    sn.UserName = txtUserName.Text;
                    sn.Password = HashPassword;
                    sn.Key = ViewState["auth_seed"].ToString();
                    sn.User.IPAddr = IpAddr;
                    sn.SuperOrganizationId = superOrganizationId;
                   // sn.MapSolute.ClientID = uid.ToString() + DateTime.Now.Ticks;
                    Session.Add("SentinelFMSession", sn);
                    sn.User.ScreenHeight = Convert.ToInt32(Request.Form["txtHeight"]);
                    sn.User.ScreenWidth = Convert.ToInt32(Request.Form["txtWidth"]);

                   

                    //sn.User.ExistingPreference(sn);

                    //DataSet dsFleets = sn.User.GetUserFleets(sn);
                    //if (dsFleets == null)
                    //{
                    //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Login ->This user account does not have a default fleet assigned. UserName: " + sn.UserName));
                    //    this.lblMessage.Text = Resources.Const.UserNoFleets;
                    //    this.lblMessage.Visible = true;
                    //    return;
                    //}

                    //sn.User.GetGuiControlsInfo(sn);

                    if (Session["PreferredCulture"] != null)
                        sn.SelectedLanguage = Session["PreferredCulture"].ToString();


                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Login -> Login Session:" + Session.SessionID.ToString() + "-" + Session.GetHashCode() + ", UserName: " + sn.UserName + ", IP: " + IpAddr));
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, sn.UserName, DateTime.Now, DateTime.Now.AddMinutes(180), false, sn.ToString());
                    FormsAuthentication.RedirectFromLoginPage(uid.ToString(), false);


                    if (sn.SuperOrganizationId == sn.User.OrganizationId)
                        destination = "frmSuperOrganizationMenu.aspx";
                    else
                        destination = "frmMain.htm";


                    // Redirect for SSL Security - Release
                    //string sDomainName = @ConfigurationSettings.AppSettings["DomainName"];
                    //string sPath = "http://" + sDomainName + "/frmMain.htm";
                    //Response.Redirect(sPath);  



                }
            }


            catch (Exception Ex)
            {

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:Login.aspx"));
            }

            if (!string.IsNullOrEmpty(destination))
                Response.Redirect(destination);


        }

      



        protected void opLanguageSelect_SelectedIndexChanged(object sender, EventArgs e)
        {


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
        protected void lnkEnglish_Click(object sender, EventArgs e)
        {
            Session["PreferredCulture"] = "en-US";
            Response.Redirect(Request.Url.LocalPath);
        }

        protected void lnkFrench_Click(object sender, EventArgs e)
        {
            Session["PreferredCulture"] = "fr-CA";
            Response.Redirect(Request.Url.LocalPath);
        }



        protected void btnRegen_Click(object s, EventArgs e)
        {
            SetCaptcha();
        }


        private void SetCaptcha()
        {
            // Set image
            string s = RandomText.Generate();

            // Encrypt
            string ens = Encryptor.Encrypt(s, "srgerg$%^bg", Convert.FromBase64String("srfjuoxp"));

            // Save to session
            Session["captcha"] = s.ToLower();

            // Set url
            imgCaptcha.ImageUrl = "~/Captcha.ashx?w=305&h=92&c=" + ens + "&bc=" + color;
            CaptchaPanel.Visible = true;

        }

        //public override void ProcessRequest(HttpContext context)
        //{
        //    context.Response.ContentType = "image/jpeg";
        //    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    context.Response.BufferOutput = false;

        //    // Get text
        //    string s = "No Text";
        //    if (context.Request.QueryString["c"] != null &&
        //        context.Request.QueryString["c"] != "")
        //    {
        //        string enc = context.Request.QueryString["c"].ToString();

        //        // space was replaced with + to prevent error
        //        enc = enc.Replace(" ", "+");
        //        try
        //        {
        //            s = Encryptor.Decrypt(enc, "srgerg$%^bg", Convert.FromBase64String("srfjuoxp"));
        //        }
        //        catch { }
        //    }
        //    // Get dimensions
        //    int w = 120;
        //    int h = 50;
        //    // Width
        //    if (context.Request.QueryString["w"] != null &&
        //        context.Request.QueryString["w"] != "")
        //    {
        //        try
        //        {
        //            w = Convert.ToInt32(context.Request.QueryString["w"]);
        //        }
        //        catch { }
        //    }
        //    // Height
        //    if (context.Request.QueryString["h"] != null &&
        //        context.Request.QueryString["h"] != "")
        //    {
        //        try
        //        {
        //            h = Convert.ToInt32(context.Request.QueryString["h"]);
        //        }
        //        catch { }
        //    }
        //    // Color
        //    Color Bc = Color.White;
        //    if (context.Request.QueryString["bc"] != null &&
        //        context.Request.QueryString["bc"] != "")
        //    {
        //        try
        //        {
        //            string bc = context.Request.QueryString["bc"].ToString().Insert(0, "#");
        //            Bc = ColorTranslator.FromHtml(bc);
        //        }
        //        catch { }
        //    }
        //    // Generate image
        //    CaptchaImage ci = new CaptchaImage(s, Bc, w, h);

        //    // Return
        //    ci.Image.Save(context.Response.OutputStream, ImageFormat.Jpeg);
        //    // Dispose
        //    ci.Dispose();
        //}

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }


        public bool TestCaptcha()
        {
            if (Session["captcha"] != null && txtCaptcha.Text.ToLower() == Session["captcha"].ToString())
            {
                // if (success != null) success();
                //CaptchaPanel.Visible = false;
                // ViewState["failcount"] = 0;
                return true;

            }
            else
            {
                txtCaptcha.Text = "";
                SetCaptcha();
                return false;
                //if (failure != null) failure();
            }
        }

        private void DrawLogo()
        {
            Session["CompanyLogo"] = ConfigurationSettings.AppSettings["DefaultLogo"];

            if (Session["CompanyLogo"] != null)
                this.imgProdLogo.ImageUrl = "images/" + Session["CompanyLogo"].ToString().TrimEnd();
        }

    
    }
}

