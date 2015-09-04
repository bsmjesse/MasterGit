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

namespace SentinelFM
{
   public partial class Reports_frmReportError : SentinelFMBasePage
   {
      
      
      protected void Page_Load(object sender, EventArgs e)
      {
         
        //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + sn.MessageText + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
      }

   }
}
