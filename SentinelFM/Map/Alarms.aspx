<%@ Page Language="C#" AutoEventWireup="true" Async="true" EnableViewState="false"
    CodeFile="Alarms.aspx.cs" Inherits="SentinelFM.Alarms" %>


<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title>Alarm Information</title>
    <style>
        .grid-row-red
        {
          background-color: red;
        }
        .grid-row-yellow
        {
          background-color: yellow;
        }
    </style>
    
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">

  
    <link rel="stylesheet" type="text/css" href="../Styles/resources/css/ext-all-g4s.css" />
    <%--<link rel="stylesheet" type="text/css" href="./Alarm_list.css" />--%>
<%--    <link rel="stylesheet" type="text/css" href="../ExtJS/examples/shared/example.css" />--%>
    <script type="text/javascript" src="../ExtJS/bootstrap.js"></script>    
    <script src="./ExtJS/Alarms_list.js" type="text/javascript"></script>
    <%--<script src="./ExtJS/status_animation.js" type="text/javascript"></script>--%>
  <%--<script type="text/javascript" src="HTMLDom.js"></script>--%>

 <script language="javascript">
         function NewWindow(AlarmId) {
		    var target = "<%=alarmDetailPage%>";
                    var mypage = target + '?AlarmId=' + AlarmId;                    
					var myname='AlarmInfo';
					var w=<%=windowWidth%>;
					var h=<%=windowHeight%>;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
	}

//	var soundObject = null;
//	function PlaySound() {
//	    if (soundObject != null) {
//	        document.body.removeChild(soundObject);
//	        soundObject.removed = true;
//	        soundObject = null;
//	    }
//	    soundObject = document.createElement("embed");
//	    soundObject.setAttribute("src", "./sounds/FireAlarm.wav");
//	    soundObject.setAttribute("hidden", true);
//	    soundObject.setAttribute("autostart", true);
//	    document.body.appendChild(soundObject);
//	}

//	function EvalSound(soundobj) {
//	    var thissound = eval("document." + soundobj);
//	    thissound.Play();
//	}

 </script>

</head>
  <body>      
    <div id='alarms-grid'></div>    
</body>
</html>
