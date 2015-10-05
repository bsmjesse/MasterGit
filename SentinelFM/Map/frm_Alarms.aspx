<%@ Page Language="C#" AutoEventWireup="true" Async="true" EnableViewState="false"
    CodeFile="frm_Alarms.aspx.cs" Inherits="SentinelFM.frm_Alarms" %>


<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title>Alarm Information</title>

    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link rel="stylesheet" type="text/css" href="../Styles/SentinelFM/css/ext-all-g4s.css" />



</head>
<body>
    <div id='alarms-grid'></div>
    <%if (SourcePage == "newmap")
        { %>
    <div style="position: absolute; left: 10px; top: 310px;"><a href="javascript:void(0)" onclick="clearLandmarkGeometry();">Clear Mapping</a></div>
    <%} %>
    <div id="sessiontimeout" class="popupwindow" style="display: none; left: 100px; top: 150px; height: 170px;">
        <div class="popupwindowTitle">
            <span>Session Timeout</span>
        </div>
        <div class="popupwindowMain">
            <span>Your session is time out.
                <br />
                <br />
                Click OK to re-login or you will be redirect to login page in 5 seconds</span>
            <div style="margin-top: 20px; margin-left: 120px;">
                <a href='javascript:void(0)' onclick='redirectToLogin()' style='margin: 0 10px;'>OK</a>
            </div>
        </div>

    </div>

    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>



    <script type="text/javascript" src="../ExtJS/bootstrap.js"></script>
    <script src="./ExtJS/Alarms_list.js?v=20140409" type="text/javascript"></script>


    <script language="javascript"> 
     
        //rraj added for date time format issue
        var userDate ='<%=sn.User.DateFormat %>';
        var userTime ='<%=sn.User.TimeFormat %>';
        function getSenchaDateFormat()
        {
            if(userDate == 'dd/MM/yyyy')
                userDate = 'd/m/Y';
            else if(userDate == 'd/M/yyyy')
                userDate ='j/n/Y';
            else if(userDate == 'dd/MM/yy')
                userDate ='d/m/y';
            else if(userDate == 'd/M/yy')
                userDate ='j/n/y';
            else if(userDate == 'd MMM yyyy')
                userDate ='j M Y';
            else if(userDate == 'MM/dd/yyyy')
                userDate ='m/d/Y';
            else if(userDate == 'M/d/yyyy')
                userDate = 'n/j/Y';
            else if(userDate == 'MM/dd/yy')
                userDate = 'm/d/y';
            else if(userDate == 'M/d/yy')
                userDate = 'n/j/y';
            else if(userDate == 'MMMM d yy')
                userDate = 'M j y';
            else if(userDate == 'yyyy/MM/dd')
                userDate = 'Y/m/d';
            if(userTime =="hh:mm:ss tt")
                userTime="h:i:s A";
            else
                userTime="H:i:s";
            return userDate+" "+userTime;
        }
        var userdateformat = getSenchaDateFormat();
        //--------------------------------------
        function NewWindow(AlarmId) {
            var target = "<%=alarmDetailPage%>";
            var mypage = target + '?AlarmId=' + AlarmId + '&s=<%=SourcePage %>';                    
            var myname='AlarmInfo';
            var w=<%=windowWidth%>;
         var h=<%=windowHeight%>;
            var winl = (screen.width - w) / 2; 
            var wint = (screen.height - h) / 2; 
            if(AlarmId==-1 || AlarmId==-2)
                h = 200;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,';
            win = window.open(mypage, myname, winprops); 
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }

        function ClearAlarms(t) {
            var alarmlist = '';
            alarmgrid.getStore().each(function (record) {
                if (t == -1 && record.data.AlarmLevel.toLowerCase().replace(/^\s+|\s+$/g, '') != 'critical' && record.data.AlarmDescription.toLowerCase().replace('cia:', '') == record.data.AlarmDescription.toLowerCase()) {
                    alarmlist += record.data.AlarmId + ',';
                }
                else if (t == -2) {
                    alarmlist += record.data.AlarmId + ',';
            
                }

            });

            if (alarmlist.length > 1)
                alarmlist = alarmlist.substring(0, alarmlist.length - 1);
            else {
                alert('You have no alarms to clear.');
                return;
            }

            var target = "<%=alarmDetailPage%>";
         var mypage = target + '?AlarmId=' + t + '&al=' + alarmlist;                    
         var myname='AlarmInfo';
         var w=<%=windowWidth%>;
             var h = 200;
             var winl = (screen.width - w) / 2; 
             var wint = (screen.height - h) / 2;         
            
             winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
             win = window.open(mypage, myname, winprops) 
             if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
         }

    

         function redirectToLogin() {
             top.document.all('TopFrame').cols = '0,*';
             window.open('../Login.aspx', '_top');
         }

         function showLanbmarkGeometry(){
             var landmarkId = $('#landmarkId').val();
             parent.showAlarm(null, 0, 0, landmarkId);
         }

         function showAlarm(alarmVehicleDescription, alarmLon, alarmLat, alarmLandmarkId){
    
             parent.showAlarm(alarmVehicleDescription, alarmLon, alarmLat, alarmLandmarkId);
         }

         function clearLandmarkGeometry()
         {
             parent.clearLandmarkGeometry();
         }

    </script>
</body>
</html>
