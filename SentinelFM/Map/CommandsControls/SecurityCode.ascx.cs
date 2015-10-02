using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Map_CommandsControls_SecurityCode : System.Web.UI.UserControl
{

    public string txtGlobalUnarmCodeField
    {
        get { return this.txtGlobalUnarmCode.Text; }
    }


    public string txtTARCodeField
    {
        get { return this.txtTARCode.Text; }
    }


    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
