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

public partial class UserControl_MapNavigation : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
       if (!Page.IsPostBack)
       {
          return; 
       }
    }
   protected void cmdZoomOut_Click(object sender, ImageClickEventArgs e)
   {
      int i = Convert.ToInt32(optMapLevel.SelectedIndex);
      if (i > 0)
      {
         i--;
         optMapLevel.SelectedIndex = i;
      }
   }

   protected void cmdZoomIn_Click(object sender, ImageClickEventArgs e)
   {
      int i = Convert.ToInt32(optMapLevel.SelectedIndex);
      if (i < optMapLevel.Items.Count - 1)
      {
         i++;
         optMapLevel.SelectedIndex  = i;
      }
   }
}
