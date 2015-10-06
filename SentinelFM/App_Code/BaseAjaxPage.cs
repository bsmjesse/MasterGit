using System;
using System.Diagnostics;

namespace SentinelFM
{
    public class BaseAjaxPage : System.Web.UI.Page
    {
        private readonly DateTime _startTime = DateTime.Now;
        private DateTime _endTime;

        public void ShowTimeElapsed()
        {
            _endTime = DateTime.Now;
            var elapsed = _endTime.Subtract(_startTime);
            Debug.WriteLine(string.Format("Started at {0}, Ended at {1}, Time Elapsed: {2}", _startTime, _endTime, elapsed.TotalMilliseconds));
        }

        public void LogErrors(Exception ex, string customMessage)
        {
            Debug.WriteLine(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + customMessage + " Form: " + Page.GetType().Name);
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + customMessage + " Form: " + Page.GetType().Name));
        }

        public BaseAjaxPage()
        {

        }

        void Page_Error(object sender, EventArgs e)
        {
            Debug.WriteLine(string.Format("sender:{0}, object{1}", sender.ToString(), e.ToString()));
        }
    }
}