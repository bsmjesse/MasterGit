using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using VLF.DasMemory;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;


using System.Diagnostics;

/// <summary>
/// Summary description for AlarmsRemoting
/// </summary>

public class AlarmsRemoting : System.MarshalByRefObject, VLF.CLS.Interfaces.IALarms    
{
    public static MemTable tblAlarm = null;
    public AlarmsRemoting()
    {
    }
    public static void Start()
    {
        try
        {

            TcpServerChannel channel = new TcpServerChannel(9903);
            ChannelServices.RegisterChannel(channel);
            
            RemotingConfiguration.RegisterWellKnownServiceType(
            typeof(AlarmsRemoting),
            "AlarmsRemotingObject",
            WellKnownObjectMode.Singleton);


            Trace.WriteLine(VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.WebInterfaces,
                        string.Format("-------Alarm MemStart-------")));


            DateTime dtFrom = new DateTime();
            dtFrom = DateTime.Now.AddDays(-1);
            //tblAlarm = new MemTable(System.Configuration.ConfigurationSettings.AppSettings["DBConnectionString"], "GetNewAlarmsInfo", dtFrom, DateTime.Now, 10000);
            tblAlarm = new MemTable(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString, "GetNewAlarmsInfo", dtFrom, DateTime.Now, 10000);
            tblAlarm.Start();
        }
        catch (Exception ex)
        {
            Trace.WriteLine(VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.WebInterfaces,
                         string.Format("Failed to start Alarm MemTable ", ex.Message)));
        }
    }

    public DataTable GetAlarms(Int32 userId)
    {
        DataTable dt = null;
        try
        {
            dt = tblAlarm.NewDataTable("UserId=" + userId, null);
        }
        catch (Exception ex)
        {
            Trace.WriteLine(VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.WebInterfaces,
                         string.Format("Failed to Get Alarm Info for User:" + userId, ex.Message)));
        }

        return dt;
    }

    public static void Stop()
    {
        Trace.WriteLine(VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.WebInterfaces,
                        string.Format("-------Alarm MemStop-------")));
        tblAlarm.Stop() ;
        TcpServerChannel channel = new TcpServerChannel(1002);
        ChannelServices.UnregisterChannel(channel);
    }
}
