using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SentinelFM
{
    public partial class Map_frmTest : System.Web.UI.Page 
    {

        private WebRequest _request;

        void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Start-->Page Load"));

            AddOnPreRenderCompleteAsync(
                new BeginEventHandler(BeginAsyncOperation),
                new EndEventHandler(EndAsyncOperation)
            );
            
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "End<--Page Load"));
        }

        IAsyncResult BeginAsyncOperation(object sender, EventArgs e, AsyncCallback cb, object state)
        {

            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Start-->BeginAsyncOperation"));

             SentinelFMSession sn = (SentinelFMSession)Session["SentinelFMSession"];
            // string strURL = Request.Url.ToString().Substring(0, Request.Url.ToString().Length - 12) + "frmFleetInfoNew.aspx";  

             //string strURL = "http://msdn.microsoft.com";
            // string strURL = Request.Url.ToString().Substring(0, Request.Url.ToString().Length - 12) + "frmTest.aspx";  
              string strURL = Request.Url.ToString().Substring(0, Request.Url.ToString().Length - 16) + "Test.aspx";  
            _request = WebRequest.Create(strURL);
            IAsyncResult res = _request.BeginGetResponse(cb, state);

            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "End<--BeginAsyncOperation"));

            return res;
        }

        void EndAsyncOperation(IAsyncResult ar)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Start-->EndAsyncOperation"));


            string text;
            using (WebResponse response = _request.EndGetResponse(ar))
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    text = reader.ReadToEnd();
                }
            }

            Regex regex = new Regex("href\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(text);

            StringBuilder builder = new StringBuilder(1024);
            foreach (Match match in matches)
            {
                builder.Append(match.Groups[1]);
                builder.Append("<br/>");
            }

           this.Label1.Text=builder.ToString();

           System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "End-->EndAsyncOperation"));
        }
    }
}
