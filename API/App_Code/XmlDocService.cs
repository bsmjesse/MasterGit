using System;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json;

/// <summary>
/// Summary description for xml
/// </summary>
[WebService(Namespace = "http://sentinelfm.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
public class XmlDocService : System.Web.Services.WebService
{

    public XmlDocService()
    {
        //InitializeComponent(); 
    }


    [WebMethod(EnableSession = true)]
    public string GetXml()
    {
        string xmlstr = string.Empty;
        string path = HttpContext.Current.Server.MapPath("../sample/result.xml");
        using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8))
        {
            xmlstr = sr.ReadToEnd();
        }
        return JsonConvert.SerializeObject(xmlstr);
    }

}
