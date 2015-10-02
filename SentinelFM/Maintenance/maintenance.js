function EnableEndValue(status) {
    var txt = document.getElementById("TextEndValue");
    if (txt != null) {
        if (status) txt.value = "0";
        txt.readOnly = status;
    }
    else
        alert("End Value Object is NULL!");
}

function VehicleMaintenaceClose(VehicleId, ServiceId) {
    var mypage = 'frmMaintenaceClose.aspx?VehicleId=' + VehicleId + '&ServiceId=' + ServiceId;
    var myname = 'Sensors';
    var w = 330;
    var h = 370;
    var winl = (screen.width - w) / 2;
    var wint = (screen.height - h) / 2;
    winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1';
    win = window.open(mypage, myname, winprops);
    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
}

function InfoWindow(NotificationId) {
    var mypage = '../Dashboard/frmNotificationInfo.aspx?NotificationId=' + NotificationId;
    var myname = '';
    var w = 450;
    var h = 300;
    var winl = (screen.width - w) / 2;
    var wint = (screen.height - h) / 2;
    winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,';
    win = window.open(mypage, myname, winprops);
    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
}