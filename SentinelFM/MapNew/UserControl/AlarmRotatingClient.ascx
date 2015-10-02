<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AlarmRotatingClient.ascx.cs" Inherits="SentinelFM.MapNew_UserControl_AlarmRotatingClient" %>

    <script language="javascript">
			<!--
        function alarm_NewWindow(AlarmId) {
            var mypage = 'Map/frmAlarmInfo.aspx?AlarmId=' + AlarmId
            var myname = 'AlarmInfo';
            var w = 400;
            var h = 580;
            //NewWindow(mypage, myname, w, h);
            parent.parent.frmMain_Top_ViewWindow(mypage, w, h);
        } 
			
	
		
			//-->
    </script>

    <script language="JavaScript">

//-- Begin Scroller's Parameters and message -->

//scroller width: change to your own;
var alarm_swidth="195" //350;

//scroller height: change to your own;
var alarm_sheight = <%=alarmsScreenHight%>;
//background color: change to your own; 
var alarm_sbcolor="white";


//scroller's speed: change to your own;
var alarm_sspeed=3;


var alarm_wholemessage="";
var alarm_resumesspeed=alarm_sspeed;
var alarm_checkSum="";

function alarm_out_ReloadAlarms()
{
    $telerik.$.get('../Map/frmAlarmRotatingServerCall.aspx', 
    function(data)
    {
        alarm_handler(data);
    });
}

function alarm_ReloadAlarms()
{
    $telerik.$.get('../Map/frmAlarmRotatingServerCall.aspx', 
    function(data)
    {
        alarm_handler(data);
        window.setTimeout("alarm_ReloadAlarms()",30000); 
    });
}


function alarm_handler(transport) {
         var alarm_temp = new Array();
         alarm_temp = transport.split('~');
         alarm_wholemessage=alarm_temp[0];
         if (alarm_temp[1]!=undefined)
            document.getElementById('<%= lblTotalAlarms.ClientID %>').innerHTML =alarm_temp[1];
            else
            document.getElementById('<%= lblTotalAlarms.ClientID %>').innerHTML ="0";
            
         if (alarm_checkSum !=alarm_temp[2])
         {
          alarm_checkSum=alarm_temp[2];
          alarm_iemarqueeShort();
         }
}

function alarm_iemarqueeShort()
{
	alarm_iediv=document.getElementById('alarm_slider');
	alarm_iediv.style.pixelTop=alarm_sheight;
	$(alarm_iediv).html(alarm_wholemessage);
	alarm_sizeup=alarm_iediv.offsetHeight;
}

function alarm_start()
{
	if(document.all) 
		alarm_iemarquee(alarm_slider);
	else if(document.getElementById)
		alarm_ns6marquee(document.getElementById('alarm_slider'));
	else if(document.layers)
		alarm_ns4marquee(document.alarm_slider1.document.alarm_slider2);
}

function alarm_iemarquee(alarm_whichdiv)
{
	alarm_iediv=eval(alarm_whichdiv);
	alarm_iediv.style.pixelTop=alarm_sheight;
	$(alarm_iediv).html(alarm_wholemessage);
	alarm_sizeup=alarm_iediv.offsetHeight;
	alarm_ieslide();
}

function alarm_ieslide()
{
	if(alarm_iediv.style.pixelTop>=alarm_sizeup*(-1))
	{
		alarm_iediv.style.pixelTop-=alarm_sspeed;
		setTimeout("alarm_ieslide()",100);
	}
	else
	{
		alarm_iediv.style.pixelTop=alarm_sheight;
		alarm_ieslide();
	}
}

function alarm_ns4marquee(alarm_whichlayer)
{
	alarm_ns4layer=eval(alarm_whichlayer);
	alarm_ns4layer.top=alarm_sheight;
	alarm_ns4layer.document.write(alarm_wholemessage);
	alarm_ns4layer.document.close();
	alarm_sizeup=alarm_ns4layer.document.height;n
	alarm_s4slide();
}

function alarm_ns4slide()
{
	if(alarm_ns4layer.top>=alarm_sizeup*(-1))
	{
		alarm_ns4layer.top-=alarm_sspeed;
		setTimeout("alarm_ns4slide()",100);
	}
	else
	{
		alarm_ns4layer.top=alarm_sheight;
		alarm_ns4slide();
	}
}

function alarm_ns6marquee(alarm_whichdiv){
   alarm_ns6div=eval(alarm_whichdiv);
   //alarm_ns6div.style.top=alarm_sheight;
   //alarm_ns6div.innerHTML=alarm_wholemessage;
   $(alarm_ns6div).html(alarm_wholemessage)
   alarm_sizeup=alarm_ns6div.offsetHeight;
   $(alarm_ns6div).css("top", alarm_sheight);
   alarm_ns6slide();
 }
 
 function alarm_ns6slide(){
   if(parseInt(alarm_ns6div.style.top)>=alarm_sizeup*(-1))
   {
       $(alarm_ns6div).css("top", parseInt(alarm_ns6div.style.top)-alarm_sspeed);
       setTimeout("alarm_ns6slide()",100);
    }
    else
    {
       $(alarm_ns6div).css("top",alarm_sheight);
       alarm_ns6slide();
    }
 }
//-- end Algorithm -->

        $(document).ready(function () {
        if (!isRunSetTopFrameHeigh) {
           SetTopFrameHeigh();
           isRunSetTopFrameHeigh = true;
        }

        alarm_sheight = $("#alarmTable").parent().height() - $("#alarmHeader").height();

        $("#alarmContent").height(alarm_sheight);
        $("#alarmContent").css("overflow","hidden");
        alarm_ReloadAlarms();
        alarm_start();
        })
    </script>

 <table class="tableDoubleBorder" id="alarmTable" align="left" border="0" width="100%" style="z-index: 101; left: 0px;
        top: 4px; width: 100%">
        <tr id="alarmHeader">
            <td style="border-bottom: black 1px outset; color: White; background-repeat: repeat;
                background-color: <%=headerColor%>" bgcolor="gray">
                <font style="font-family: Verdana" color="white">
                    <asp:Label ID="lblAlarmsTitle" runat="server" meta:resourcekey="lblAlarmsTitleResource1">Alarms (24 hr) Total:</asp:Label>
                    <asp:Label  id="lblTotalAlarms"  runat="server" /></font>
            </td>
        </tr>
        <tr>
            <td bgcolor="white">
                <!--Goes between <BODY ... > & </BODY> tags-->
                <!-- begin: body code  -->
                <script language="JavaScript">
                    document.write('<table border="0" align="center"><tr><td width=' + alarm_swidth + '>'); if (document.getElementById || document.all) { document.write('<span style="height:' + alarm_sheight + ';"><div id="alarmContent" style="position:relative;overflow:hidden;width:' + alarm_swidth + ';height:' + alarm_sheight + ';clip:rect(0 ' + alarm_swidth + ' ' + alarm_sheight + ' 0);background-color:' + alarm_sbcolor + ';" onMouseover="alarm_sspeed=0;" onMouseout="alarm_sspeed=alarm_resumesspeed"><div id="alarm_slider" style="position:relative;width:' + alarm_swidth + ';"></div></div></span>') }
                    
                </script>
                <ilayer width="&amp;{alarm_swidth};" height="&amp;{alarm_sheight};" name="alarm_slider1" bgcolor="&amp;{alarm_sbcolor};">
							<layer onmouseover="alarm_sspeed=0;" onmouseout="alarm_sspeed=alarm_resumesspeed" width="&amp;{alarm_swidth};"
								name="alarm_slider2"></layer>
						</ilayer>
                <script type="text/javascript" >
                    document.write('</td></tr></table>');
                </script>
            </td>
        </tr>
    </table>