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



//public partial class LoginAmeco : System.Web.UI.Page
//{
//    protected void Page_Load(object sender, EventArgs e)
//    {

//    }
//}
namespace SentinelFM
{

    public delegate void CaptchaEventHandler();

    /// <summary>
    /// Summary description for Login.
    /// </summary>
    public partial class CaptchaLogin : System.Web.UI.Page
    {
        protected ServerDBUser.DBUser dbu;
        protected SentinelFMSession sn = null;
        protected clsUtility objUtil;

        private string color = "#ffffff";
        protected string style;
        private event CaptchaEventHandler success;
        private event CaptchaEventHandler failure;

        public short AuthenticationFailed = 0;

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
                string u = Request.ServerVariables["HTTP_USER_AGENT"];
                System.Text.RegularExpressions.Regex b = new System.Text.RegularExpressions.Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
                System.Text.RegularExpressions.Regex v = new System.Text.RegularExpressions.Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
                if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))) && (Request["f"] == null || Request["f"].ToString() != "1"))
                {
                    Response.Redirect("~/mobile");
                }

                int vs = 0;
                if (ViewState["failcount"] == null)
                    ViewState["failcount"] = 0;
                else
                    vs = Convert.ToInt32(ViewState["failcount"]);

                Uri uri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
                string host = uri.Host;

                Label3.Text = "(Server: " + host + " )";
                Label3.Visible = false;


                Session["Host"] = HttpContext.Current.Request.Url.Host;

                string[] tmp = Session["Host"].ToString().ToLowerInvariant().Split('.');

                if (tmp.Length <= 3)
                {
                    if (tmp.Length == 1)
                        Session["Host"] = tmp[0];
                    else
                        Session["Host"] = tmp[tmp.Length - 2];
                }
                else
                    Session["Host"] = HttpContext.Current.Request.Url.Host.Replace(".", "_");



                if (!Page.IsPostBack)
                {
                    string hydroquebecLink = ConfigurationManager.AppSettings["HydroQuebecLink"];

                    ///if (hydroquebecLink != string.Empty && host.Contains(hydroquebecLink))
                    if (hydroquebecLink != null && hydroquebecLink != string.Empty && host.ToLower() == hydroquebecLink.ToLower())
                    {
                        //lblCustomerSupport.Text = "Soutien aux utilisateurs Hydro-Québec: ";
                        //lblCustomerSupportPhone.Text = "interne : 0-204-7170 ou externe : 40-441-7200 poste 7170";

                        if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                        {
                            lblCustomerSupport.Text = "<span style='font-weight:bold'>Soutien aux utilisateurs Hydro-Québec :</span>";
                            lblCustomerSupportPhone.Text = " Courriel : <a href='mailto:hqdgps@hydro.qc.ca' title='hqdgps@hydro.qc.ca' alt='hqdgps@hydro.qc.ca'>>HQD GPS</a>";
                        }
                        else
                        {
                            lblCustomerSupport.Text = "<span style='font-weight:bold'>Support to Hydro-Quebec users : </span>";
                            lblCustomerSupportPhone.Text = " Email : <a href='mailto:hqdgps@hydro.qc.ca' title='hqdgps@hydro.qc.ca' alt='hqdgps@hydro.qc.ca'>>HQD GPS</a>";
                        }

                        lblTollFreeNumber.Visible = false;
                        this.lblEmail.Visible = false;
                        this.lblEmailurl.Visible = false;

                    }

                    if (Session["Host"].ToString() != "sentinelfm")
                    {
                        lblCustomerSupport.Visible = false;
                        lblCustomerSupportPhone.Visible = false;
                        lblTollFreeNumber.Visible = false;
                        this.lblEmail.Visible = false;
                        this.lblEmailurl.Visible = false;
                    }

                    if (Request.QueryString["errormsg"] != null)
                    {
                        lblMessageError.Visible = true;
                    }

                   if (ViewState["auth_seed"] == null)
                        ViewState["auth_seed"] = "1";// Convert.ToString(new System.Random().NextDouble());


                    string strPathxxx = "c:\\";

                    // Check User License and cookies
                    string Logo = Request.Form["Logo"];
                    string URL = Request.Form["URL"];
                    string showLogo = Request.QueryString["showLogo"];

                    this.imgProdLogo.ImageUrl = "SentinelFM_Themes/" + Session["Host"].ToString() + "/images/amecologo.png";


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


                    //if (showLogo != null)
                    //    this.imgProdLogo.Visible = Convert.ToBoolean(showLogo);

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

                if (AuthenticationFailed == 1)
                    this.txtPassword.Text = "";

                lblCustomerSupport.Visible = false;
                lblCustomerSupportPhone.Visible = false;
            }

            catch (System.Threading.ThreadAbortException) { return; }
            catch (Exception Ex)
            {
                //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" Form:Login.aspx"));    
            }



            //if (Request.Url.ToString().ToLower().Contains("http:"))
            //{
            //    string script = "try {if (window.location.href.toLowerCase().indexOf('https:') < 0) {window.location.href = window.location.href.toLowerCase().replace(/http:/, 'https:');}} catch(err){};";
            //    ScriptManager scriptManager = ScriptManager.GetCurrent(this);
            //    if (scriptManager != null && scriptManager.IsInAsyncPostBack)
            //    {
            //        //if a MS AJAX request, use the Scriptmanager class
            //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "httpsscript", script, true);
            //    }
            //    else
            //    {
            //        //if a standard postback, use the standard ClientScript method
            //        ClientScript.RegisterClientScriptBlock(this.GetType(), "httpsscript", script, true);
            //    }
            //}


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
            ServerDBUser.DBUser vdbu = new ServerDBUser.DBUser();

            try
            {
                if (Convert.ToInt16(ViewState["failcount"]) > 2)
                {
                    if (!CaptchaPanel.Visible)
                        SetCaptcha();
                    else
                        if (!TestCaptcha())
                            return;
                }
            }
            catch (Exception Ex)
            {
                //lblMessage.Text = Ex.Message.ToString();
                //lblMessage.Visible = true;
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:Login.aspx"));
            }

            int uid = -1;
            string secId = "";
            int superOrganizationId = 1;
            string Email = string.Empty;
            bool isDisclaimer = false;
            //string IpAddr = Request.ServerVariables["remote_addr"];


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
            valPassword.Visible = true;

            try
            {
                if (Request.QueryString["Password"] != null)
                {
                    strUsername = Request.QueryString["Username"].ToString();
                    HashPassword = Request.QueryString["Password"].ToString();


                    HttpCookie Sentinel = new HttpCookie("SentinelCompany");
                    Sentinel.Name = "SentinelCompany";
                    Sentinel.Value = String.Format("{0}|{1}", strUsername, HashPassword);
                    Response.Cookies.Add(Sentinel);

                }
                else
                    HashPassword = Request.Form["txtHash"].ToString().TrimEnd();
            }
            catch (Exception Ex)
            {
                //lblMessage.Text = Ex.Message.ToString();
                //lblMessage.Visible = true;
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:Login.aspx"));
            }

            //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info , "DoLogin - IP:" + IpAddr + " Psw: " + HashPassword ));


            #region start check IP for hgi user
            try
            {
                if (strUsername.Contains("hgi") == true && IpAddr.Contains("127.0.0.1") == false && IpAddr.Contains("::1") == false)
                {
                    string[] ip;
                    if (IpAddr.Contains("184.94.19") == true)
                    {
                        //ip = IpAddr.Split('.');
                        //if (Convert.ToInt16(ip[3]) < 97 || Convert.ToInt16(ip[3]) > 110)
                        // {
                        //        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AuthFailed");
                        //        this.lblMessage.Visible = true;
                        //        return;
                        //}
                    }
                    else if (IpAddr.Contains("192.168.199") == true)
                    {
                        ip = IpAddr.Split('.');
                        if (Convert.ToInt16(ip[3]) < 1 || Convert.ToInt16(ip[3]) > 254)
                        {
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AuthFailed");
                            this.lblMessage.Visible = true;
                            AuthenticationFailed = 1;
                            return;
                        }
                    }
                    else if (IpAddr.Contains("67.70.185") == true)
                    {
                        ip = IpAddr.Split('.');
                        if (Convert.ToInt16(ip[3]) < 121 || Convert.ToInt16(ip[3]) > 126)
                        {
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AuthFailed");
                            this.lblMessage.Visible = true;
                            AuthenticationFailed = 1;
                            return;
                        }
                    }
                    else if (IpAddr.Contains("172.16.3") == true)
                    {
                        ip = IpAddr.Split('.');
                        if (Convert.ToInt16(ip[3]) < 1 || Convert.ToInt16(ip[3]) > 254)
                        {
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AuthFailed");
                            this.lblMessage.Visible = true;
                            AuthenticationFailed = 1;
                            return;
                        }
                    }

                    else if (IpAddr.Contains("184.94.19") == false || IpAddr.Contains("192.168.199") == false || IpAddr.Contains("67.70.185") == false || IpAddr.Contains("172.16.3") == false)
                    {
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AuthFailed");
                        this.lblMessage.Visible = true;
                        AuthenticationFailed = 1;
                        return;

                    }
                }
                else if ((strUsername.ToLower().Trim() == "ncc1" || strUsername.ToLower().Trim() == "ncc2") && IpAddr.Contains("209.171.44.131") == false && IpAddr.Contains("206.162.182.113") == false)
                {
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AuthFailed");
                    this.lblMessage.Visible = true;
                    AuthenticationFailed = 1;
                    return;
                }
            }
            catch
            {
            }

            #endregion check IP




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
                //errCode = sec.LoginMD5ByDBName(ViewState["auth_seed"].ToString(), strUsername, HashPassword, IpAddr, conString, ref uid, ref secId, ref superOrganizationId);

                errCode = sec.LoginMD5Extended(ViewState["auth_seed"].ToString(), strUsername, HashPassword, IpAddr, ref uid, ref secId, ref superOrganizationId, ref Email, ref isDisclaimer);

                if (errCode != (int)VLF.ERRSecurity.InterfaceError.NoError)
                {
                    valPassword.Visible = false;
                    this.lblMessage.Text = "Authentication failed";
                    this.lblMessage.Visible = true;
                    AuthenticationFailed = 1;
                    //sec.AddFailedEntry(txtUserName.Text, IpAddr);
                    //captcha
                    if (ViewState["failcount"] == null)
                        ViewState["failcount"] = 1;
                    else
                        ViewState["failcount"] = Convert.ToInt16(ViewState["failcount"]) + 1;

                    //captcha

                    vdbu.RecordUserAction("User", 0, 0, "vlfUser", null, "Login failed", IpAddr, this.Context.Request.RawUrl, string.Format("{0}:{1} at IP:{2}login failed", strUsername, HashPassword, IpAddr));

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
                    sn.UserName = txtUserName.Text;
                    sn.Password = HashPassword;
                    sn.Key = ViewState["auth_seed"].ToString();
                    sn.User.IPAddr = IpAddr;
                    sn.SuperOrganizationId = superOrganizationId;

                    //sn.MapSolute.ClientID = uid.ToString() + DateTime.Now.Ticks;
                    Session.Add("SentinelFMSession", sn);
                    ////////
                   // sn.User.ScreenHeight = Convert.ToInt32(Request.Form["txtHeight"]);
                   // sn.User.ScreenWidth = Convert.ToInt32(Request.Form["txtWidth"]);



                    if (Session["PreferredCulture"] != null)
                        sn.SelectedLanguage = Session["PreferredCulture"].ToString();






                    //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Login -> Login Session:" + Session.SessionID.ToString() + "-" + Session.GetHashCode() + ", UserName: " + sn.UserName+", IP: "+IpAddr));
                    //FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, sn.UserName, DateTime.Now, DateTime.Now.AddMinutes(180), false, sn.ToString());
                    //FormsAuthentication.RedirectFromLoginPage(uid.ToString(), false);








                    //if (rblSFM.SelectedValue == "1") //&& (sn.InterfacePrefrence == (Int16)VLF.CLS.Def.Enums.InterfacePrefrence.Lite || sn.InterfacePrefrence == (Int16)VLF.CLS.Def.Enums.InterfacePrefrence.Both))
                    //{
                    //    string qs = sn.GetSerializedBase();
                    //    //string key = clsUtility.SetSessionBaseToMemcahed(qs);
                    //    string key = clsUtility.SetSessionBaseToTxtFile(qs);
                    //    destination = string.Format((string)ConfigurationManager.AppSettings["SentinelLite_URL"], key);

                    //    Response.Redirect(destination);
                    //}
                    //else
                    //{
                        sn.User.ExistingPreference(sn);
                        sn.User.GetGuiControlsInfo(sn);


                        #region Organization IP restriction

                        //try
                        //{
                        //    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                        //    VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(sConnectionString);
                        //    if (org.OrganizationIP_Validate(sn.User.OrganizationId, IpAddr) != 0)
                        //    {
                        //        this.lblMessage.Text = "Access Prohibited!. Please contact your account manager.";
                        //        this.lblMessage.Visible = true;
                        //        return;
                        //    }
                        //}
                        //catch
                        //{
                        //}

                        #endregion Organization IP restriction


                        if (sn.SuperOrganizationId == sn.User.OrganizationId)
                            destination = "frmSuperOrganizationMenu.aspx";
                        else
                            destination = "frmMain_Top.aspx";


                    //}




                    // Redirect for SSL Security - Release
                    //string sDomainName = @ConfigurationSettings.AppSettings["DomainName"];
                    //string sPath = "http://" + sDomainName + "/frmMain.htm";
                    //Response.Redirect(sPath);  



                }
            }


            catch (Exception Ex)
            {
                //lblMessage.Text = Ex.Message.ToString();
               // lblMessage.Visible = true;

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:Login.aspx"));
            }


            if (uid > 0)
            {

                int validateResult = vdbu.ValidateLockedUser(uid);
                if (validateResult == 1)
                {
                    //Response.Write("<script>alert('Your account is locked, please reset your password'); __doPostBack('mnuMenu','Configuration/frmPreference.aspx')</script>");
                    Response.Redirect("Configuration/frmPreference.aspx?errormsg=1");
                    return;
                }
                else if (validateResult == 2)
                {
                    FormsAuthentication.SignOut();
                    Response.Redirect("login.aspx?errormsg=2");
                }
            }



            if (!string.IsNullOrEmpty(destination))
                Response.Redirect(destination);


        }

        private void DrawLogo()
        {
            Session["CompanyLogo"] = ConfigurationSettings.AppSettings["DefaultLogo"];

            //if (Session["CompanyLogo"] != null)
            //    this.imgProdLogo.ImageUrl = "images/" + Session["CompanyLogo"].ToString().TrimEnd();
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
            this.lblEmail.Visible = true;
            this.lblEmailurl.Visible = true;
        }

        protected void lnkFrench_Click(object sender, EventArgs e)
        {
            this.lblEmail.Visible = false;
            this.lblEmailurl.Visible = false;
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

        //public bool IsReusable
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}


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





    }
}
