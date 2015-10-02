using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Reports_TestI : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SentinelFM.SecurityManager.SecurityManager securityManager = new SentinelFM.SecurityManager.SecurityManager();
        securityManager.UseDefaultCredentials = true; 
        string SID = string.Empty;
        //securityManager.ValidateandReloginMD5ByDBName(1, "1", "1", "1", "1", ref SID);
        System.Reflection.MethodInfo[] body =  securityManager.GetType().GetMethods();
        List<string> methods = new List<string>();
        foreach (System.Reflection.MethodInfo info in body)
        {
            methods.Add(info.Name);
        }
       methods.Sort();
        lst.DataSource = methods;
        lst.DataBind();
        string sid="";
        //securityManager.ValidateandReloginMD5ByDBName(0, null, null, null, null, ref sid);
        Response.Write(securityManager.Url);

    }
}