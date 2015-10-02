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
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;



public partial class Map_frmAlarmsTest : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string remoteIP = "localhost";
        string remotePort = "1002";
        string remoteURL = @"tcp://" + remoteIP + ":" + remotePort + "/AlarmsRemotingObject";

        //TcpClientChannel chan = new TcpClientChannel();
        //ChannelServices.RegisterChannel(chan);
        VLF.CLS.Interfaces.IALarms alarmsRemoting = (VLF.CLS.Interfaces.IALarms)Activator.GetObject(typeof(VLF.CLS.Interfaces.IALarms),
                          remoteURL);


        DataTable dt=alarmsRemoting.GetAlarms(4);
    }
}


