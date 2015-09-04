<%@ Page Language="C#" AutoEventWireup="true" Async="true" EnableViewState="false"
    CodeFile="frmAlarmRotatingClient.aspx.cs" Inherits="SentinelFM.frmAlarmRotatingClient" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title>Alarm Information</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">

    <script src="../Scripts/prototype.js" type="text/javascript"></script>

    <link href="../GlobalStyle.css" type="text/css" rel="stylesheet">
    <style type="text/css">
        BODY
        {
            scrollbar-face-color: #ffffff;
            scrollbar-highlight-color: #ffffff;
            scrollbar-shadow-color: #ffffff;
            scrollbar-3dlight-color: #ffffff;
            scrollbar-arrow-color: #ffffff;
            scrollbar-darkshadow-color: #ffffff;
        }
 
        A
        {
            font-size: 12px;
            color: #0000aa;
            font-family: Verdana;
            text-decoration: none;
        }
        A:link
        {
            font-size: 12px;
            color: #0000aa;
            font-family: Verdana;
            text-decoration: none;
        }
        A:visited
        {
            font-size: 12px;
            color: #0000aa;
            font-family: Verdana;
            text-decoration: none;
        }
        A:active
        {
            font-size: 12px;
            color: #0000aa;
            font-family: Verdana;
            text-decoration: none;
        }
        A:hover
        {
            font-size: 13px;
            color: #ff0000;
            font-family: Verdana;
            text-decoration: none;
        }
        P
        {
            font-size: 11px;
            color: #000000;
            font-family: Verdana;
        }
        TR
        {
            font-size: 11px;
            color: #000000;
            font-family: Verdana;
        }
        TD
        {
            font-size: 11px;
            color: #000000;
            font-family: Verdana;
        }
        UL
        {
            font-size: 11px;
            color: #000000;
            font-family: Verdana;
        }
        LI
        {
            font-size: 11px;
            color: #000000;
            font-family: Verdana;
        }
        FORM
        {
            margin: 5px;
        }
        .header1
        {
            padding-right: 2px;
            padding-left: 2px;
            font-size: 13px;
            background: #4682b4;
            padding-bottom: 2px;
            margin: 0px;
            color: #ffffff;
            padding-top: 2px;
            font-family: Verdana;
        }
        H1
        {
            padding-right: 2px;
            padding-left: 2px;
            font-size: 13px;
            background: #4682b4;
            padding-bottom: 2px;
            margin: 0px;
            color: #ffffff;
            padding-top: 2px;
            font-family: Verdana;
        }
        .header2
        {
            font-size: 12px;
            background: #dbeaf5;
            color: #000000;
            font-family: Verdana;
        }
        H2
        {
            font-size: 12px;
            background: #dbeaf5;
            color: #000000;
            font-family: Verdana;
        }
        .intd
        {
            padding-left: 15px;
            font-size: 11px;
            color: #000000;
            font-family: Verdana;
        }
    </style>

    <script language="javascript">
			<!--
				function NewWindow(AlarmId) { 
					var mypage='frmAlarmInfo.aspx?AlarmId='+AlarmId
					var myname='AlarmInfo';
					var w=380;
					var h=500;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				} 
			
			
		
			//-->
    </script>

    <script language="JavaScript">

//-- Begin Scroller's Parameters and message -->

//scroller width: change to your own;
var swidth="195" //350;

//scroller height: change to your own;
var sheight=<%=alarmsScreenHight%>//130;

//background color: change to your own; 
var sbcolor="white";


//scroller's speed: change to your own;
var sspeed=3;


var wholemessage="";
var resumesspeed=sspeed;
var checkSum="";

function ReloadAlarms()
    {
            new Ajax.Request('frmAlarmRotatingServerCall.aspx', {
                evalJS: false,
                onSuccess: function(transport) { handler(transport); }
                });

     }


function handler(transport) {
         var temp = new Array();
        
         temp = transport.responseText.split('~');
         wholemessage=temp[0];
      
         if (temp[1]!=undefined)
            document.getElementById('lblTotalAlarms').innerHTML =temp[1];
            else
            document.getElementById('lblTotalAlarms').innerHTML ="0";
            
         //if (checkSum !=temp[2])
         //{
          checkSum=temp[2];
          iemarqueeShort();
         //}
          
         window.setTimeout("ReloadAlarms()",30000); 

}

function iemarqueeShort()
{
	iediv=document.getElementById('slider');
	iediv.style.pixelTop=sheight;
	iediv.innerHTML=wholemessage;
	sizeup=iediv.offsetHeight;
}

function start()
{
	if(document.all) 
		iemarquee(slider);
	else if(document.getElementById)
		ns6marquee(document.getElementById('slider'));
	else if(document.layers)
		ns4marquee(document.slider1.document.slider2);
}

function iemarquee(whichdiv)
{
	iediv=eval(whichdiv);
	iediv.style.pixelTop=sheight;
	iediv.innerHTML=wholemessage;
	sizeup=iediv.offsetHeight;
	ieslide();
}

function ieslide()
{
	if(iediv.style.pixelTop>=sizeup*(-1))
	{
		iediv.style.pixelTop-=sspeed;
		setTimeout("ieslide()",100);
	}
	else
	{
		iediv.style.pixelTop=sheight;
		ieslide();
	}
}

function ns4marquee(whichlayer)
{
	ns4layer=eval(whichlayer);
	ns4layer.top=sheight;
	ns4layer.document.write(wholemessage);
	ns4layer.document.close();
	sizeup=ns4layer.document.height;n
	s4slide();
}

function ns4slide()
{
	if(ns4layer.top>=sizeup*(-1))
	{
		ns4layer.top-=sspeed;
		setTimeout("ns4slide()",100);
	}
	else
	{
		ns4layer.top=sheight;
		ns4slide();
	}
}
function ns6marquee(whichdiv){ns6div=eval(whichdiv);ns6div.style.top=sheight;ns6div.innerHTML=wholemessage;sizeup=ns6div.offsetHeight;ns6slide();}function ns6slide(){if(parseInt(ns6div.style.top)>=sizeup*(-1)){ns6div.style.top=parseInt(ns6div.style.top)-sspeed;setTimeout("ns6slide()",100);}else{ns6div.style.top=sheight;ns6slide();}}
//-- end Algorithm -->
    </script>

</head>
<body bgcolor="white" onload="ReloadAlarms();start();">
    <form id="WebForm1" method="post" runat="server">
    <table class="tableDoubleBorder" style="z-index: 101; left: 0px; position: absolute;
        top: 4px; width: 100%" align="left" border="0">
        <tr>
            <td style="border-bottom: black 1px outset; color: White; background-repeat: repeat;
                background-color: <%=headerColor%>" bgcolor="gray">
                <font style="font-family: Verdana" color="white">
                    <asp:Label ID="lblAlarmsTitle" runat="server" meta:resourcekey="lblAlarmsTitleResource1">Alarms (24 hr) Total:</asp:Label>
                    <label  id="lblTotalAlarms" name="lblTotalAlarms"/></font>
            </td>
        </tr>
        <tr>
            <td bgcolor="white">
                <!--Goes between <BODY ... > & </BODY> tags-->
                <!-- begin: body code  -->

                <script language="JavaScript">document.write('<table border="0" align="center"><tr><td width='+swidth+'>');if (document.getElementById || document.all){document.write('<span style="height:'+sheight+';"><div style="position:relative;overflow:hidden;width:'+swidth+';height:'+sheight+';clip:rect(0 '+swidth+' '+sheight+' 0);background-color:'+sbcolor+';" onMouseover="sspeed=0;" onMouseout="sspeed=resumesspeed"><div id="slider" style="position:relative;width:'+swidth+';"></div></div></span>')}</script>

                <ilayer width="&amp;{swidth};" height="&amp;{sheight};" name="slider1" bgcolor="&amp;{sbcolor};">
							<layer onmouseover="sspeed=0;" onmouseout="sspeed=resumesspeed" width="&amp;{swidth};"
								name="slider2"></layer>
						</ilayer>
            </td>
        </tr>
    </table>
    <!-- end -->
    </form>
</body>
</html>
