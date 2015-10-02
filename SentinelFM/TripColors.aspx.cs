using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class TripColors : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected override void InitializeCulture()
    {
        if (Session["PreferredCulture"] != null)
        {
            string UserCulture = Session["PreferredCulture"].ToString();
            if (UserCulture != "")
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(UserCulture);
            }
        }
    }
}