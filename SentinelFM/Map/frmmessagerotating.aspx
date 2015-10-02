<%@ Page Language="c#" Inherits="SentinelFM.frmMesssageRotating" CodeFile="frmMessageRotating.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Alarm Information</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
    <style type="text/css">BODY { SCROLLBAR-FACE-COLOR: #ffffff; SCROLLBAR-HIGHLIGHT-COLOR: #ffffff; SCROLLBAR-SHADOW-COLOR: #ffffff; SCROLLBAR-3DLIGHT-COLOR: #ffffff; SCROLLBAR-ARROW-COLOR: #ffffff; SCROLLBAR-DARKSHADOW-COLOR: #ffffff }
		</style>
    <%--</STYLE>--%>
    <style> A { FONT-SIZE: 11px; COLOR: #0000aa; FONT-FAMILY: Tahoma, Verdana; TEXT-DECORATION: none } A:link { FONT-SIZE: 11px; COLOR: #0000aa; FONT-FAMILY: Tahoma, Verdana; TEXT-DECORATION: none } A:visited { FONT-SIZE: 11px; COLOR: #0000aa; FONT-FAMILY: Tahoma, Verdana; TEXT-DECORATION: none } A:active { FONT-SIZE: 11px; COLOR: #0000aa; FONT-FAMILY: Tahoma, Verdana; TEXT-DECORATION: none } A:hover { FONT-SIZE: 12px; COLOR: #ff0000; FONT-FAMILY: Tahoma, Verdana; TEXT-DECORATION: none } P { FONT-SIZE: 11px; COLOR: #000000; FONT-FAMILY: Tahoma, Verdana } TR { FONT-SIZE: 11px; COLOR: #000000; FONT-FAMILY: Tahoma, Verdana } TD { FONT-SIZE: 11px; COLOR: #000000; FONT-FAMILY: Tahoma, Verdana } UL { FONT-SIZE: 11px; COLOR: #000000; FONT-FAMILY: Tahoma, Verdana } LI { FONT-SIZE: 11px; COLOR: #000000; FONT-FAMILY: Tahoma, Verdana } FORM { MARGIN: 5px } .header1 { PADDING-RIGHT: 2px; PADDING-LEFT: 2px; FONT-WEIGHT: bold; FONT-SIZE: 13px; BACKGROUND: #4682b4; PADDING-BOTTOM: 2px; MARGIN: 0px; COLOR: #ffffff; PADDING-TOP: 2px; FONT-FAMILY: Tahoma, Verdana } H1 { PADDING-RIGHT: 2px; PADDING-LEFT: 2px; FONT-WEIGHT: bold; FONT-SIZE: 13px; BACKGROUND: #4682b4; PADDING-BOTTOM: 2px; MARGIN: 0px; COLOR: #ffffff; PADDING-TOP: 2px; FONT-FAMILY: Tahoma, Verdana } .header2 { FONT-WEIGHT: bold; FONT-SIZE: 12px; BACKGROUND: #dbeaf5; COLOR: #000000; FONT-FAMILY: Tahoma, Verdana } H2 { FONT-WEIGHT: bold; FONT-SIZE: 12px; BACKGROUND: #dbeaf5; COLOR: #000000; FONT-FAMILY: Tahoma, Verdana } .intd { PADDING-LEFT: 15px; FONT-SIZE: 11px; COLOR: #000000; FONT-FAMILY: Tahoma, Verdana } </style>

    <script language="javascript">
			<!--
			
			function NewWindow(MsgKey) { 
					var mypage='frmMessageInfo.aspx?MsgKey='+MsgKey
					var myname='MessageInfo';
					var w=335;
					var h=400;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				} 





			//setTimeout('location.reload(true)','<%=sn.User.GeneralRefreshFrequency%>')
		

			//-->
    </script>

    <!--Goes between <head runat="server"> & </HEAD> tags-->

    <script language="JavaScript">

	//-- Begin Scroller's Parameters and message -->

	//scroller width: change to your own;
	var swidth="195" //350;

	//scroller height: change to your own;
	var sheight=140//80;

	//background color: change to your own; 
	var sbcolor="white";


	//scroller's speed: change to your own;
	var sspeed=2;


var wholemessage="<%=strMessage %>";

var resumesspeed=sspeed;function start()
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

    <!-- End of Script between <head runat="server"> & </head> tags-->
</head>
<body bgcolor="white" onload="start();">
    <form id="WebForm1" method="post" runat="server">
        <table class="tableDoubleBorder" style="z-index: 101;left: 0px;position: absolute; top: 2px; width: 100%" align="left" border="0">
            <tr>
                 <td class="hbg" style="border-bottom: black 1px outset" bgcolor="gray">
                    <font style="font-family: Verdana" color="white">
                    <asp:Label ID="lblMessageTitle" runat="server" meta:resourcekey="lblMessageTitleResource1">Messages (24 hr) Total:</asp:Label>
                        <asp:Label ID="lblMessTotal" runat="server" meta:resourcekey="lblMessTotalResource1"></asp:Label></font></td>
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
