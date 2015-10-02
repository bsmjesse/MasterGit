<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MessageRotatingClient.ascx.cs" Inherits="SentinelFM.MapNew_UserControl_MessageRotatingClient" %>
    <script language="javascript">
			<!--

        function Message_NewWindow(MsgKey) {
            var mypage = 'Map/frmMessageInfo.aspx?MsgKey=' + MsgKey
            var myname = 'MessageInfo';
            var w = 445;
            var h = 440;
            //NewWindow(mypage, myname, w, h);
            parent.parent.frmMain_Top_ViewWindow(mypage, w, h);
        } 





			

			//-->
    </script>



    <script language="JavaScript">

	//-- Begin Scroller's Parameters and message -->

	//scroller width: change to your own;
	var message_swidth="195" //350;

	//scroller height: change to your own;
	
    var message_sheight=<%=MDTMessagesScrollingHight%>//105;

	//background color: change to your own; 
	var message_sbcolor="white";


	//scroller's speed: change to your own;
	var message_sspeed=2;


var message_wholemessage="";
var message_resumesspeed=message_sspeed;
var message_checkSum="";


function message_Out_ReloadMessages()
{
        $telerik.$.get('../Map/frmMessageRotatingServer.aspx', 
        function(data)
        {
            message_handler(data);
        });

}


    function message_ReloadMessages()
    {
        $telerik.$.get('../Map/frmMessageRotatingServer.aspx', 
        function(data)
        {
            message_handler(data);
            window.setTimeout("message_ReloadMessages()",30000); 
        });

     }
     
     function message_handler(transport) {
         var message_temp = new Array();
         
         message_temp = transport.split('~');
         message_wholemessage=message_temp[0];
         
         if (message_temp[1]!=undefined)
            document.getElementById('<%= lblTotalMsgs.ClientID %>').innerHTML =message_temp[1];
            else
            document.getElementById('<%= lblTotalMsgs.ClientID %>').innerHTML ="0";
            
         if (message_checkSum !=message_temp[2])
         {
          message_checkSum=message_temp[2];
          message_iemarqueeShort();
         }
}


function message_iemarqueeShort()
{
	message_iediv=document.getElementById('message_slider');
	message_iediv.style.pixelTop=message_sheight;
	$(message_iediv).html(message_wholemessage);
	message_sizeup=message_iediv.offsetHeight;
}


function message_start()
{
	if(document.all) 
		message_iemarquee(message_slider);
	else if(document.getElementById)
		message_ns6marquee(document.getElementById('message_slider'));
	else if(document.layers)
		message_ns4marquee(document.message_slider1.document.message_slider2);
}

function message_iemarquee(message_whichdiv)
{
	message_iediv=eval(message_whichdiv);
	message_iediv.style.pixelTop=message_sheight;
	$(message_iediv).html(message_wholemessage);
	message_sizeup=message_iediv.offsetHeight;
	message_ieslide();
}

function message_ieslide()
{
	if(message_iediv.style.pixelTop>=message_sizeup*(-1))
	{
		message_iediv.style.pixelTop-=message_sspeed;
		setTimeout("message_ieslide()",100);
	}
	else
	{
		message_iediv.style.pixelTop=message_sheight;
		message_ieslide();
	}
}

function message_ns4marquee(message_whichlayer)
{
	message_ns4layer=eval(message_whichlayer);
	message_ns4layer.top=message_sheight;
	message_ns4layer.document.write(message_wholemessage);
	message_ns4layer.document.close();
	message_sizeup=message_ns4layer.document.height;n
	message_s4slide();
}

function message_ns4slide()
{
	if(message_ns4layer.top>=message_sizeup*(-1))
	{
		message_ns4layer.top-=message_sspeed;
		setTimeout("message_ns4slide()",100);
	}
	else
	{
		message_ns4layer.top=message_sheight;
		message_ns4slide();
	}
}
function message_ns6marquee(message_whichdiv)
{
   message_ns6div=eval(message_whichdiv);
   //message_ns6div.style.top=message_sheight;
   $(message_ns6div).html(message_wholemessage);
   message_sizeup=message_ns6div.offsetHeight;
   $(message_ns6div).css("top",message_sheight);
   message_ns6slide();
 }
 
 function message_ns6slide()
 {
     if(parseInt(message_ns6div.style.top)>=message_sizeup*(-1))
     {
        $(message_ns6div).css("top", parseInt(message_ns6div.style.top)-message_sspeed);
        setTimeout("message_ns6slide()",100);
     }
     else
     {
        $(message_ns6div).css("top", message_sheight);
        message_ns6slide();
      }
 }
//-- end Algorithm -->
        $(document).ready(function () {
        if (!isRunSetTopFrameHeigh) {
           SetTopFrameHeigh();
           isRunSetTopFrameHeigh = true;
        }

        message_sheight = $("#messageTable").parent().height() - $("#messageHeader").height();
        $("#messageContent").height(message_sheight);
        $("#messageContent").css("overflow","hidden");
        message_ReloadMessages();
        message_start();
        })

    </script>


  <table class="tableDoubleBorder" id="messageTable" align="left" border="0" style="z-index: 101;left: 0px; top: 2px; width: 100%">
            <tr id="messageHeader">
                 <td style="border-bottom: black 1px outset;color:White;background-repeat: repeat;background-color:<%=headerColor%>" bgcolor="gray">
                    <font style="font-family: Verdana" color="white">
                    <asp:Label ID="lblMessageTitle" runat="server" meta:resourcekey="lblMessageTitleResource1">Messages (24 hr) Total:</asp:Label>
                         <asp:Label  id="lblTotalMsgs" runat="server" name="lblTotalMsgs" /></font></td>
            </tr>
            <tr>
                <td bgcolor="white">
                    <!--Goes between <BODY ... > & </BODY> tags-->
                    <!-- begin: body code  -->
                    <script language="JavaScript">
                        document.write('<table border="0" align="center"><tr><td width=' + message_swidth + '>');
                        if (document.getElementById || document.all) {
                            document.write('<span style="height:' + message_sheight + ';"><div id="messageContent" style="position:relative;overflow:hidden;width:' + message_swidth + ';height:' + message_sheight + ';clip:rect(0 ' + message_swidth + ' ' + message_sheight + ' 0);background-color:' + message_sbcolor + ';" onMouseover="message_sspeed=0;" onMouseout="message_sspeed=message_resumesspeed"><div id="message_slider" style="position:relative;width:' + message_swidth + ';"></div></div></span>')
                        }
                        </script>

                    <ilayer width="&amp;{message_swidth};" height="&amp;{message_sheight};" name="message_slider1" bgcolor="&amp;{message_sbcolor};">
							<layer onmouseover="message_sspeed=0;" onmouseout="message_sspeed=message_resumesspeed" width="&amp;{message_swidth};"
								name="message_slider2"></layer>
						</ilayer>
                   <script type="text/javascript">
                       document.write('</td></tr></table>');
                   </script>
                </td>
            </tr>
        </table>