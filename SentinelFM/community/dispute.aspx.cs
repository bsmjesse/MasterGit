using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using CaptchaDotNet2.Security.Cryptography;

using Newtonsoft.Json;
using VLF.DAS.Logic;
using VLF3.DataAccess;

public partial class dispute_speed : System.Web.UI.Page
{
    private static string color = "#ffffff";
    public string VlatLon = null;
    public string ViolationId = null;
    public string ErrorSpeed = null;
    public string YourSpeed = null;
    public string Objects = null;
    public string ObjectId = null;
    public string Address = null;
    public bool IsMobile = false;
    public bool Submitted = false;
    public string ErrorReason = string.Empty;
    public static string Nid = null;
    public static bool UseCaptcha = false;
    public static int ErrorThreshold = 0;
    
    protected void Page_Load(object sender, EventArgs e)
    {        
        IsMobile = IamMobile();
        Submitted = Convert.ToBoolean(Request.QueryString["submitted"] ?? "false");
        if (Convert.ToInt32(Session["errorthreshold"] ?? "0") > 2)
        {
            UseCaptcha = true;
        }
        if (!Submitted)
        {


            VlatLon = Request.QueryString["q"] ?? Request.Form["q"];
            if (string.IsNullOrEmpty(VlatLon))
            {
                throw new Exception("Please give coordinate params");
            }
            ViolationId = Request.QueryString["infractionid"] ?? Request.Form["infractionid"];
            if (string.IsNullOrEmpty(ViolationId))
            {
                throw new Exception("Please specify the violation id");
            }
            ErrorSpeed = Request.QueryString["errorspeed"] ?? Request.Form["errorspeed"];
            if (string.IsNullOrEmpty(ErrorSpeed))
            {
                throw new Exception("Please give error speed value");
            }
            Objects = Request.QueryString["objects"] ?? Request.Form["objects"];
            ObjectId = Request.QueryString["objectid"] ?? Request.Form["objectid"];
            Address = Request.QueryString["address"] ?? Request.Form["address"];
            Nid = Request.QueryString["nid"] ?? Request.Form["nid"];
            
            YourSpeed = Request.QueryString["yourspeed"] ?? Request.Form["yourspeed"];

            string changeCaptcha = Request.QueryString["changeCaptcha"] ?? Request.Form["changeCaptcha"];


            if (IsPostBack &&( !"1".Equals(changeCaptcha) || !UseCaptcha))
            {
               
                string actualSpeed = Request.QueryString["actualspeed"] ?? Request.Form["actualspeed"];
                if (string.IsNullOrEmpty(actualSpeed))
                {
                    actualSpeed = "0";
                }
                string notes = Request.QueryString["notes"] ?? Request.Form["notes"];
                string myCaptcha = txtCaptchaM.Text;
                string name = Request.QueryString["name"] ?? Request.Form["name"];
                string metric = Request.QueryString["metric"] ?? Request.Form["metric"];
                string email = Request.QueryString["email"] ?? Request.Form["email"];
                string result = ProcessDisputeTicket(ViolationId, VlatLon, actualSpeed, notes, myCaptcha, name, metric,
                    ErrorSpeed, YourSpeed, Objects, ObjectId, email);
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                Submitted = true;
                if (Convert.ToInt32(actualSpeed) > 0 && !string.IsNullOrEmpty(name))
                {
                    if (!values["status"].Equals("success"))
                    {
                        if (values["reason"].Contains("dispute"))
                        {
                            ErrorReason = "You cannot submit the duplicated point";
                        }
                        else
                        {
                            ErrorReason = values["reason"];
                        }                        
                        Session["errorthreshold"] = Convert.ToInt32(Session["errorthreshold"] ?? "0") + 1;
                    }
                    else
                    {
                        Session["errorthreshold"] = 0;
                    }
                }
                else
                {
                    ErrorReason = "Speed and name cannot be empty.";
                    Session["errorthreshold"] = Convert.ToInt32(Session["errorthreshold"] ?? "0") + 1;
                }
                
            }
            else
            {
                if (UseCaptcha)
                {
                    SetCaptcha();   
                }                
            }
        }
    }

    protected void btnRegen_Click(object s, EventArgs e)
    {
        SetCaptcha();
    }


    [System.Web.Services.WebMethod(EnableSession = true)]
    [ScriptMethod()] 
    public static string SubmitTicket(string infractionid, string q, string actualspeed, string notes, string txtCaptcha, string name, string metric, string errorspeed, string yourspeed, string objects, string objectid, string email)
    {
        return ProcessDisputeTicket(infractionid, q, actualspeed, notes, txtCaptcha, name, metric, errorspeed, yourspeed,
            objects, objectid, email);
    }

    [System.Web.Services.WebMethod(EnableSession = true)]
    [ScriptMethod()]
    public static string GenerateCaptcha()
    {
        string s = RandomText.Generate();

        // Encrypt
        string ens = Encryptor.Encrypt(s, "srgerg$%^bg", Convert.FromBase64String("srfjuoxp"));

        // Save to session
        HttpContext.Current.Session["captcha"] = s.ToLower();
        return "../Captcha.ashx?w=305&h=92&c=" + ens + "&bc=" + color;
    }

    public static string ProcessDisputeTicket(string infractionid, string q, string actualspeed, string notes, string txtCaptcha, string name, string metric, string errorspeed, string yourspeed, string objects, string objectid, string email)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        try
        {
            string captcharStr = txtCaptcha;
            string captcha1 = (HttpContext.Current.Session["captcha"] != null ? HttpContext.Current.Session["captcha"].ToString() : null);

            if (captcharStr.ToLower() == captcha1 && !string.IsNullOrEmpty(captcha1) || !UseCaptcha)
            {                
                string drivername = name;
                int vid = Convert.ToInt32(infractionid);
                int speed = Convert.ToInt32(actualspeed);
                int myObjectId = Convert.ToInt32(objectid);
                string latlon = q;
                double lat = Convert.ToDouble(latlon.Split(',').First().Trim());
                double lon = Convert.ToDouble(latlon.Split(',').Last().Trim());
                int metricVal = Convert.ToInt32(metric);
                int mynid = Convert.ToInt32(Nid ?? "0");
                if (speed > 0 && !string.IsNullOrEmpty(drivername))
                {
                    if (PostGisLandmark.UpdateViolationDispute(vid, speed, lat, lon, metricVal, notes, drivername, errorspeed, objects, myObjectId, yourspeed, mynid, email))
                    {
                        HttpContext.Current.Session["errorthreshold"] = 0;
                        UseCaptcha = false;
                        result.Add("status", "success");
                        HttpContext.Current.Session.Clear();
                    }
                    else
                    {
                        result.Add("status", "failed");
                        result.Add("reason", "The ticked might have been submitted before, you cannot re-submit, please contact with customer service.");
                        HttpContext.Current.Session["errorthreshold"] = Convert.ToInt32(HttpContext.Current.Session["errorthreshold"] ?? "0") + 1;
                    } 
                }
                else
                {
                    result.Add("status", "failed");
                    result.Add("reason", "Speed and driver name cannot be empty.");
                    HttpContext.Current.Session["errorthreshold"] = Convert.ToInt32(HttpContext.Current.Session["errorthreshold"] ?? "0") + 1;
                }
                
            }
            else
            {
                result.Add("status", "failed");
                result.Add("reason", "Your captcha is not correct.");
            }



        }
        catch (Exception exception)
        {
            HttpContext.Current.Session["errorthreshold"] = Convert.ToInt32(HttpContext.Current.Session["errorthreshold"] ?? "0") + 1;
            if (Convert.ToInt32(HttpContext.Current.Session["errorthreshold"] ?? "0") % 3 == 0 && Convert.ToInt32(HttpContext.Current.Session["errorthreshold"] ?? "0") > 0)
            {
                result.Add("status", "reload");

            }
            else
            {
                result.Add("status", "failed");
                if (exception.Message.Contains("duplicate"))
                {
                    result.Add("reason", "The point you have submitted is duplicated.");
                }
                else
                {
                    result.Add("reason", exception.Message);
                } 
            }            
                       
        }
        if (result.Any())
        {
            var oSerializer = new JavaScriptSerializer();
            string json = oSerializer.Serialize(result);
            return json;
        }
        return "Failed";
    }

    private void SetCaptcha()
    {
        // Set image
        string s = RandomText.Generate();

        // Encrypt
        string ens = Encryptor.Encrypt(s, "srgerg$%^bg", Convert.FromBase64String("srfjuoxp"));

        // Save to session
        Session["captcha"] = s.ToLower();
        if (IsMobile)
        {
            

            // Set url
            imgCaptchaM.ImageUrl = "~/Captcha.ashx?w=305&h=92&c=" + ens + "&bc=" + color;
            //CaptchaPanel.Visible = true;
        }
        else
        {
           

            // Set url
            imgCaptcha.ImageUrl = "~/Captcha.ashx?w=305&h=92&c=" + ens + "&bc=" + color;
            CaptchaPanel.Visible = true;
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

    public bool IamMobile()
    {
        string u = Request.ServerVariables["HTTP_USER_AGENT"];
        System.Text.RegularExpressions.Regex b = new System.Text.RegularExpressions.Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
        System.Text.RegularExpressions.Regex v = new System.Text.RegularExpressions.Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
        if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))) && (Request["f"] == null || Request["f"].ToString() != "1"))
        {
            return true;
        }
        return false;
    }
}