using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Text;
using System.Web;

public partial class GarminDestinationHandler : System.Web.UI.Page
{
    public static long _RequestID = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        string output = string.Empty;
        Response.StatusCode = GarminHandlerService.Get.Send(Request, out _RequestID, out output);
        Response.Write(output);
        Response.End();
    }


}