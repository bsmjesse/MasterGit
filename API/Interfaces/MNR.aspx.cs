using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MNR : System.Web.UI.Page
{
    //VLF.ASI.Interfaces.SentinelWSI obj = new VLF.ASI.Interfaces.SentinelWSI();
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // Create byte array to hold request bytes
            byte[] inputStream = new byte[HttpContext.Current.Request.ContentLength];

            // Read entire request inputstream
            HttpContext.Current.Request.InputStream.Read(inputStream, 0, inputStream.Length);

            //Set stream back to beginning
            HttpContext.Current.Request.InputStream.Position = 0;

            //Get  XML request
            string requestString = System.Text.ASCIIEncoding.ASCII.GetString(inputStream);

            string output = string.Empty;
            if ((requestString.Length > 0) && ValidateAuthentication())
            {
                //HttpContext.Current.Request.QueryString.Add("uId", "WSI_MNR");
                //HttpContext.Current.Request.QueryString.Add("pwd", "TWpJNU1EZz0=1000050@Gps1$us");
                using (VLF.ASI.Interfaces.SentinelWSI iSentinelWSI = new VLF.ASI.Interfaces.SentinelWSI())
                {
                    iSentinelWSI.GetDevicesLastKnownPositionPreTemplatedInfo_AutomotedAgent(requestString);
                }
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
	HttpContext.Current.Response.Flush();
        //Response.Write("output");
        Response.End();
    }

    bool ValidateAuthentication()
    {
        System.Collections.Specialized.NameValueCollection query = HttpUtility.ParseQueryString(HttpContext.Current.Request.QueryString.ToString());

        if (query.Count < 2)
            Response.Write("User authentication error!");

        return query.Count == 2;//IsValid;
    }
}