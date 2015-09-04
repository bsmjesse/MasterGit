<%@ Page Language="C#" AutoEventWireup="true" CodeFile="VideoViewer.aspx.cs" Inherits="SentinelFM.Widgets_VideoViewer" %>
<%if(ShowHTML) { %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />

    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>  
    <script type="text/javascript" src="../sencha/extjs-4.1.0/ext-all.js"></script>
    <script type="text/javascript" src="../Scripts/videoviewer.js?v=20141203"></script>
    <script type="text/javascript">
        var VedioList = "<%=VedioList %>";
        function DownloadVideo(filename) {
            var url = 'VideoViewer.aspx?st=download&fn=' + filename;
            $('#downloadframe').prop('src', url)
        }

        var userDate = '<%=sn.User.DateFormat %>';
        var userTime = '<%=sn.User.TimeFormat %>';
        function getSenchaDateFormat() {
            if (userDate == 'dd/MM/yyyy')
                userDate = 'd/m/Y';
            else if (userDate == 'd/M/yyyy')
                userDate = 'j/n/Y';
            else if (userDate == 'dd/MM/yy')
                userDate = 'd/m/y';
            else if (userDate == 'd/M/yy')
                userDate = 'j/n/y';
            else if (userDate == 'd MMM yyyy')
                userDate = 'j M Y';
            else if (userDate == 'MM/dd/yyyy')
                userDate = 'm/d/Y';
            else if (userDate == 'M/d/yyyy')
                userDate = 'n/j/Y';
            else if (userDate == 'MM/dd/yy')
                userDate = 'm/d/y';
            else if (userDate == 'M/d/yy')
                userDate = 'n/j/y';
            else if (userDate == 'MMMM d yy')
                userDate = 'M j y';
            else if (userDate == 'yyyy/MM/dd')
                userDate = 'Y/m/d';
            if (userTime == "hh:mm:ss tt")
                userTime = "h:i:s A";
            else
                userTime = "H:i:s";
            return userDate + " " + userTime;
        }
        var userdateformat = getSenchaDateFormat();
    </script>
    <title></title>
</head>
<body>
    <div id='videos-grid'></div> 
    <iframe id="downloadframe" name="downloadframe" style="display:none"></iframe>
</body>
</html>
<%} %>