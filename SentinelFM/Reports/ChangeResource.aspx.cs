using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

public partial class ChangeResource : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnbyckey_Click(object sender, EventArgs e)
    {
        
        string file_SE = Server.MapPath("");
        string file_SF= Server.MapPath("");
        string file_d = Server.MapPath("");

        XmlDocument dResource = new XmlDocument();
        XmlDocument SFResource = new XmlDocument();
        dResource.Load(file_d);
        SFResource.Load(file_SF);

        XmlNodeList  nodes = dResource.SelectNodes("root/data");

        foreach (XmlNode item in nodes)
        {
            string name = item.Attributes["name"].Value;
            
             XmlNode sValue = SFResource.SelectSingleNode("root/data[@name='" + name + "']/value");
            if (sValue != null)
            {
                XmlNode dValue = item.SelectSingleNode("value");
                if (dValue != null)
                {
                    dValue.InnerText = sValue.InnerText;
                }
            }
        }

        dResource.Save(file_d);
    }
    protected void btnbyValue_Click(object sender, EventArgs e)
    {
        string file_SE = Server.MapPath("../App_LocalResources/frmTopMenu.aspx.resx");
        string file_SF = Server.MapPath("../App_LocalResources/frmTopMenu.aspx.fr.resx");
        string file_d = Server.MapPath("../UserControl/App_LocalResources/TopMenu.ascx.fr.resx");

        XmlDocument dResource = new XmlDocument();
        XmlDocument SFResource = new XmlDocument();
        XmlDocument SEResource = new XmlDocument();
        SEResource.Load(file_SE);
        SFResource.Load(file_SF);
        dResource.Load(file_d);
        XmlNodeList nodes = dResource.SelectNodes("root/data");

        foreach (XmlNode item in nodes)
        {
            XmlNode dValue = item.SelectSingleNode("value");
            if (dValue != null)
            {
                XmlNode SEValue = SEResource.SelectSingleNode(string.Format("root/data/value[text()='{0}']", dValue.InnerText));
                if (SEValue != null)
                {
                    string name = SEValue.ParentNode.Attributes["name"].Value;
                    XmlNode SFValue = SFResource.SelectSingleNode("root/data[@name='" + name + "']/value");
                    if (SFValue != null) dValue.InnerText = SFValue.InnerText;
                } 
            }
        }

        dResource.Save(file_d);

    }
}
