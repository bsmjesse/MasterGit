<%@ WebService Language="C#" Class="Session" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Web.Script.Services;

    [WebService(Namespace = "http://www.sentinelfm.com")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.Web.Script.Services.ScriptService]

    public class Session : System.Web.Services.WebService
    {
       // protected SentinelFMSession sn = null;

        //[WebMethod(EnableSession = true), Description("Clear Session")]
        //public void ClearSession()
        //{

        //    System.Web.Security.FormsAuthentication.SignOut();
            
        //    Session["SentinelFMSession"] = null;
        //    Session.RemoveAll();
        //    Session.Clear();
        //    Session.Abandon();
           
        //}

        //[WebMethod]
        //[ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        //public string HelloWorld()
        //{
        //    return "HELLO";
        //    //JavaScriptSerializer js = new JavaScriptSerializer();// Use this when formatting the data as JSON
        //    // return js.Serialize("Hello World");        
        //}
    }

