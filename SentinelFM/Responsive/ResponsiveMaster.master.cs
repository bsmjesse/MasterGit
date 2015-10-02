using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml;
using System.Net;

namespace SentinelFM
{
    public partial class Responsive_ResponsiveMaster : System.Web.UI.MasterPage
    {
        //public Label LblCustomerSupportPhone
        //{
        //    get { return lblCustomerSupportPhone; }
        //    set { lblCustomerSupportPhone = value; }
        //}
        //public Label LblCustomerSupport
        //{
        //    get { return lblCustomerSupport; }
        //    set { lblCustomerSupport = value; }
        //}
        //public Label LblTollFreeNumber
        //{
        //    get { return lblTollFreeNumber; }
        //    set { lblTollFreeNumber = value; }
        //}
        //public Label LblEmail
        //{
        //    get { return lblEmail; }
        //    set { lblEmail = value; }
        //}
        //public HyperLink LblEmailurl
        //{
        //    get { return lblEmailurl; }
        //    set { lblEmailurl = value; }
        //}
        //public Label LblMessageError
        //{
        //    get { return lblMessageError; }
        //    set { lblMessageError = value; }
        //}
        //public Label LblLabel3
        //{
        //    get { return Label3; }
        //    set { Label3 = value; }
        //}
        
        

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                //Get the client Machine Name                
                //lblMachineName.Text = Environment.MachineName;

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
                if (url.ToString().ToLower().TrimEnd() == rowItem["url"].ToString().ToLower().TrimEnd())
                {
                    foreach (DataColumn dataCol in ds.Tables[0].Columns)
                    {
                        string fieldValue = rowItem[dataCol].ToString();
                        switch (dataCol.ToString())
                        {
                            case "HeaderLogoPosition":
                                if (fieldValue == "Left")
                                    HeaderLogoPositionCss.Href = "css/LogoHeader/left_HeaderLogo.css";
                                else if (fieldValue == "Right")
                                    HeaderLogoPositionCss.Href = "css/LogoHeader/right_HeaderLogo.css";
                                else if (fieldValue == "Center")
                                    HeaderLogoPositionCss.Href = "css/LogoHeader/middle_HeaderLogo.css";
                                break;

                            case "HeaderHeight":
                                if (rowItem["HeaderHeight"].ToString().TrimEnd() != "")
                                    headerDiv.Style.Add(HtmlTextWriterStyle.Height, fieldValue);
                                break;

                            case "LoginBannerPosition":
                                if (fieldValue == "LeftLoginRightBanner")
                                    LoginBannerPositionCss.Href = "css/positionContainer/rs_ll.css";
                                else if (fieldValue == "RightLoginLeftBanner")
                                    LoginBannerPositionCss.Href = "css/positionContainer/ls_rl.css";
                                else if (fieldValue == "TopLoginBottomBanner")
                                    LoginBannerPositionCss.Href = "css/positionContainer/tl_bs.css";
                                else if (fieldValue == "TopLoginNoBanner")
                                    LoginBannerPositionCss.Href = "css/positionContainer/nb_bs.css";
                                break;

                            case "LoginLogoPosition":
                                if (fieldValue == "Left")
                                    LoginLogoPositionCss.Href = "css/logoPostion/left_logo.css";
                                else if (fieldValue == "Right")
                                    LoginLogoPositionCss.Href = "css/logoPostion/right_logo.css";
                                else if (fieldValue == "Center")
                                    LoginLogoPositionCss.Href = "css/logoPostion/middle_logo.css";
                                break;

                            case "FooterPosition":
                                if (fieldValue == "Left")
                                    FooterPositionCss.Href = "css/footer/footer_left.css";
                                else if (fieldValue == "Right")
                                    FooterPositionCss.Href = "css/footer/footer_right.css";
                                else if (fieldValue == "Center")
                                    FooterPositionCss.Href = "css/footer/footer_center.css";

                                break;
                        }
                    }
                }

            }
        }
    }
}
