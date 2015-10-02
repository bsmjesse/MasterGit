using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;


namespace SentinelFM
{
    public partial class Responsive_ResponsiveLogin2 : System.Web.UI.Page
    {       
        protected void Page_Load(object sender, System.EventArgs e)
        {
            //design settings
            string ParentConDiv = "images/ResponsiveLoginBG_1.png";
            //parentConDiv.Style["background-image"] = Page.ResolveUrl(ParentConDiv);
            //parentConDiv.Style["background-color"] = "gray";
            if (!IsPostBack)
            {
                DataSet ds = new DataSet();
                ds = ReadXML();
                Uri url = Request.Url;
                SetUserSettings(url, ds);
            }
            

        }

        protected DataSet ReadXML()
        {
            XmlReader xmlFile;
            xmlFile = XmlReader.Create(System.Web.HttpContext.Current.Server.MapPath("UserSettingData/UserSettings.xml"), new XmlReaderSettings());
            DataSet ds = new DataSet();
            ds.ReadXml(xmlFile);
            xmlFile.Close();
            return ds;
        }

        protected void SetUserSettings(Uri url, DataSet ds)
        {
            foreach (DataRow rowItem in ds.Tables[0].Rows)
            {
                if (url.ToString().TrimEnd() == rowItem["url"].ToString().TrimEnd())
                {
                    foreach (DataColumn dataCol in ds.Tables[0].Columns)
                    {
                        string fieldValue = rowItem[dataCol].ToString();
                        switch (dataCol.ToString())
                        {
                            case "BodyBackgroundColor":
                                if (rowItem["BodyBackgroundColor"].ToString().TrimEnd() != "")
                                    parentConDiv.Style["background-color"] = fieldValue;
                                break;

                            case "BodyBackgroungImage":
                                if (rowItem["BodyBackgroungImage"].ToString().TrimEnd() != "")
                                    parentConDiv.Style["background-image"] = Page.ResolveUrl(fieldValue);
                                break;

                            case "LoginButtonColor":
                                if (rowItem["LoginButtonColor"].ToString().TrimEnd() != "")
                                    btnLogin.Style["background"] = fieldValue;
                                break;

                            case "LoginLinksColor":
                                if (rowItem["LoginLinksColor"].ToString().TrimEnd() != "")
                                    loginlinksDiv.Style["background"] = fieldValue;
                                break;


                        }
                    }
                }
            }
        }

    }
}

