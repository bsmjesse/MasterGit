using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Configuration_Equipment_ctlEquipmentMenu : System.Web.UI.UserControl
{
    public String SelectedControl = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(SelectedControl))
        {
            if (FindControl(SelectedControl) is Button)
            {
                ((Button)FindControl(SelectedControl)).CssClass = "selectedbutton";
                ((Button)FindControl(SelectedControl)).OnClientClick = "javascript:return false;";
            }
        }
    }
}