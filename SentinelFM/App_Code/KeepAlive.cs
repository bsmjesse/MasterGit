using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;

/// <summary>
/// Summary description for KeepAlive
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
[ScriptService]
public class KeepAlive : System.Web.Services.WebService {

    public KeepAlive () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession=true) ]
 
    public bool IsSessionAlive() {
        

        SentinelFM.SentinelFMSession snMain = (SentinelFM.SentinelFMSession)Session["SentinelFMSession"];
        if (snMain == null || snMain.User == null || String.IsNullOrEmpty(snMain.UserName))
        {
            return false;
        }
        else
        {
            Session["KeepSessionAlive"] = DateTime.Now;
            return true;
        }
    }
    
}
