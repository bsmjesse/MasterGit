using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VLF.DAS.Logic;
using System.Configuration;
using Telerik.Web.UI;

namespace SentinelFM
{
    public partial class Configuration_EditFuelCategory : System.Web.UI.UserControl
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

    }
}