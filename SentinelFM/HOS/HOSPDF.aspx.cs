using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class HOS_HOSPDF : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            byte[] test = (byte[])Session["CurrentPDFFileData"];
                //= PreviousPage.CurrentFileData;
            if (test != null)
            {
                Response.Buffer = false; //transmitfile self buffers
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.ContentType = "application/pdf";
                Response.OutputStream.Write(test, 0, test.Length);
                Response.Flush();
                Session["CurrentPDFFileData"] = null;
            }
        }
    }
}