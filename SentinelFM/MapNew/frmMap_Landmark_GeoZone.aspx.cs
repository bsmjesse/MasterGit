using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.IO;
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace SentinelFM
{

    public partial class MapNew_frmMap_Landmark_GeoZone : SentinelMapBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                GuiSecurity(this);
                //Clear Tooltips
            }
        }


        

}
}