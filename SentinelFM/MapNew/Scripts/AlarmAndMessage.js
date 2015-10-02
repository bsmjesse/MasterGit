function alarm_NewWindow(AlarmId) {
    var mypage = '../Map/frmAlarmInfo.aspx?AlarmId=' + AlarmId
    var myname = 'AlarmInfo';
    var w = 380;
    var h = 500;
    NewWindow(mypage, myname, w, h);
} 
			


//-- Begin Scroller's Parameters and message -->

//scroller width: change to your own;
var alarm_swidth="195" //350;

//scroller height: change to your own;


//background color: change to your own; 
var alarm_sbcolor="white";


//scroller's speed: change to your own;
var alarm_sspeed=3;


var alarm_wholemessage="";
var alarm_resumesspeed=alarm_sspeed;
var alarm_checkSum="";

function alarm_ReloadAlarms()
{
    $telerik.$.get('../Map/frmAlarmRotatingServerCall.aspx', 
    function(data)
    {
        alarm_handler(data);
    });
}


function alarm_handler(transport) {
         var alarm_temp = new Array();
        
         alarm_temp = transport.split('~');
         alarm_wholemessage=alarm_temp[0];
         
         if (alarm_temp[1]!=undefined)
            document.getElementById('lblTotalAlarms').innerHTML =alarm_temp[1];
            else
            document.getElementById('lblTotalAlarms').innerHTML ="0";
            
         if (alarm_checkSum !=alarm_temp[2])
         {
          alarm_checkSum=alarm_temp[2];
          alarm_iemarqueeShort();
         }
          
         window.setTimeout("alarm_ReloadAlarms()",30000); 

}

function alarm_iemarqueeShort()
{
	alarm_iediv=document.getElementById('alarm_slider');
	alarm_iediv.style.pixelTop=alarm_sheight;
	alarm_iediv.innerHTML=alarm_wholemessage;
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
	alarm_iediv.innerHTML=alarm_wholemessage;
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
   alarm_ns6div.style.top=alarm_sheight;
   alarm_ns6div.innerHTML=alarm_wholemessage;
   alarm_sizeup=alarm_ns6div.offsetHeight;
   alarm_ns6slide();
 }
 
 function alarm_ns6slide(){
   if(parseInt(alarm_ns6div.style.top)>=alarm_sizeup*(-1))
   {
       alarm_ns6div.style.top=parseInt(alarm_ns6div.style.top)-alarm_sspeed;
       setTimeout("alarm_ns6slide()",100);
    }
    else
    {
       alarm_ns6div.style.top=alarm_sheight;
       alarm_ns6slide();
    }
 }
//-- end Algorithm -->


  function Message_NewWindow(MsgKey) {
            var mypage = 'frmMessageInfo.aspx?MsgKey=' + MsgKey
            var myname = 'MessageInfo';
            var w = 426;
            var h = 400;
            NewWindow(mypage, myname, w, h);
        } 


	//-- Begin Scroller's Parameters and message -->

	//scroller width: change to your own;
	var message_swidth="195" //350;

	//scroller height: change to your own;
	
    
	//background color: change to your own; 
	var message_sbcolor="white";


	//scroller's speed: change to your own;
	var message_sspeed=2;


var message_wholemessage="";
var message_resumesspeed=message_sspeed;
var message_checkSum="";


    function message_ReloadMessages()
    {
        $telerik.$.get('../Map/frmMessageRotatingServer.aspx', 
        function(data)
        {
            message_handler(data);
        });

     }
     
     function message_handler(transport) {
         var message_temp = new Array();
         
         message_temp = transport.split('~');
         message_wholemessage=message_temp[0];
         
         if (message_temp[1]!=undefined)
            document.getElementById('lblTotalMsgs').innerHTML =message_temp[1];
            else
            document.getElementById('lblTotalMsgs').innerHTML ="0";
            
         if (message_checkSum !=message_temp[2])
         {
          message_checkSum=message_temp[2];
          message_iemarqueeShort();
         }
         
         window.setTimeout("message_ReloadMessages()",30000); 

}


function message_iemarqueeShort()
{
	message_iediv=document.getElementById('message_slider');
	message_iediv.style.pixelTop=message_sheight;
	message_iediv.innerHTML=message_wholemessage;
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
	message_iediv.innerHTML=message_wholemessage;
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
   message_ns6div.style.top=message_sheight;
   message_ns6div.innerHTML=message_wholemessage;
   message_sizeup=message_ns6div.offsetHeight;
   message_ns6slide();
 }
 
 function message_ns6slide()
 {
     if(parseInt(message_ns6div.style.top)>=message_sizeup*(-1))
     {
        message_ns6div.style.top=parseInt(message_ns6div.style.top)-message_sspeed;
        setTimeout("message_ns6slide()",100);
     }
     else
     {
        message_ns6div.style.top=message_sheight;
        message_ns6slide();
      }
 }
//-- end Algorithm -->