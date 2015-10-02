using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class HOS_frmImage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
         string hosImagePath = System.Configuration.ConfigurationManager.AppSettings["HOSImagePath"];
         string file =  Request["p"].ToString();
         //Response.ContentType = "image/png";
         using (Bitmap image = new Bitmap(hosImagePath + file))
         {
             using (System.IO.MemoryStream ms = new MemoryStream())
             {
                 image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                 ms.WriteTo(Response.OutputStream);
             }
         }
    }
}